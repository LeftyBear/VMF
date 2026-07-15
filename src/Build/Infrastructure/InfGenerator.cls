VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfGenerator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfGenerator
' Layer: Infrastructure
' Responsibility: Mediate manifest and template contracts for generation.
' Dependencies: Common, Infrastructure
'==========================================================================

Private Const InsertModuleDeclaration As String = "ModuleDeclaration"
Private Const InsertDeclarationSectionEnd As String = "DeclarationSectionEnd"
Private Const InsertMemberDeclarations As String = "MemberDeclarations"
Private Const InsertMemberBlock As String = "MemberBlock"
Private Const InsertInitializeBody As String = "InitializeBody"
Private Const InsertTerminateBody As String = "TerminateBody"
Private Const InsertPropertyGroupStart As String = "PropertyGroupStart"
Private Const InsertProperties As String = "Properties"
Private Const InsertPropertyGroupEnd As String = "PropertyGroupEnd"
Private Const InsertProcedureGroupStart As String = "ProcedureGroupStart"
Private Const InsertProcedureGroupEnd As String = "ProcedureGroupEnd"

Private TemplateProvider As InfTemplateProvider
Private ManifestProvider As InfManifestProvider
Private TokenReplacer As InfTokenReplacer

' Initializes the generator with its collaborators.
Public Sub InfInitialize(ByVal Provider As InfTemplateProvider, ByVal ProviderManifest As InfManifestProvider, ByVal Replacer As InfTokenReplacer)
    If Provider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfInitialize", "TemplateProvider is required."
    End If

    If ProviderManifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfInitialize", "ManifestProvider is required."
    End If

    If Replacer Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfInitialize", "TokenReplacer is required."
    End If

    Set TemplateProvider = Provider
    Set ManifestProvider = ProviderManifest
    Set TokenReplacer = Replacer
End Sub

' Generates a standard module body using the default module template.
Public Function InfGenerateStandardModule(ByVal ModuleName As String, ByVal LayerName As String) As String
    ComRequireText ModuleName, "ModuleName", "InfGenerateStandardModule"
    ComRequireText LayerName, "LayerName", "InfGenerateStandardModule"

    If TemplateProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateStandardModule", "Generator is not initialized."
    End If

    InfGenerateStandardModule = InfGenerateModule(ModuleName, TemplateProvider.InfGetModuleTemplatePath(), LayerName)
End Function

' Generates a class module body using the default class template.
Public Function InfGenerateStandardClass(ByVal ClassName As String, ByVal LayerName As String) As String
    ComRequireText ClassName, "ClassName", "InfGenerateStandardClass"
    ComRequireText LayerName, "LayerName", "InfGenerateStandardClass"

    If TemplateProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateStandardClass", "Generator is not initialized."
    End If

    InfGenerateStandardClass = InfGenerateModule(ClassName, TemplateProvider.InfGetClassTemplatePath(), LayerName)
End Function

' Generates a module body using template and manifest data.
Public Function InfGenerateModule(ByVal ModuleName As String, ByVal TemplatePath As String, Optional ByVal LayerName As String = "Application") As String
    Dim Template As InfTemplate
    Dim Manifest As Object
    Dim GeneratedText As String
    Dim ValidationResult As ComResult

    ComRequireText ModuleName, "ModuleName", "InfGenerateModule"
    ComRequireText TemplatePath, "TemplatePath", "InfGenerateModule"
    ComRequireText LayerName, "LayerName", "InfGenerateModule"

    If TemplateProvider Is Nothing Or ManifestProvider Is Nothing Or TokenReplacer Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Generator is not initialized."
    End If

    Set ValidationResult = TemplateProvider.InfValidateTemplateFile(TemplatePath)
    If ValidationResult.IsFailure Then
        Err.Raise ComErrInvalidArgument, "InfGenerateModule", ValidationResult.Message
    End If

    Set Template = TemplateProvider.InfCreateTemplate(TemplatePath)
    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Template is invalid."
    End If

    Set Manifest = ManifestProvider.InfCreateManifest(ModuleName, TemplatePath, LayerName)
    EnsureInsertionSections Manifest
    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Manifest)

    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Generated text is empty."
    End If

    InfGenerateModule = GeneratedText
End Function

' Generates a module body from a manifest-driven generation item.
Public Function InfGenerateManifestItem(ByVal Item As ManifestItem) As String
    Dim Template As InfTemplate
    Dim Manifest As Object
    Dim GeneratedText As String
    Dim ValidationResult As ComResult

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfGenerateManifestItem", "ManifestItem is required."
    End If

    If Not Item.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "ManifestItem is invalid."
    End If

    If TemplateProvider Is Nothing Or ManifestProvider Is Nothing Or TokenReplacer Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "Generator is not initialized."
    End If

    Set ValidationResult = TemplateProvider.InfValidateTemplateFile(Item.InfGetTemplatePath())
    If ValidationResult.IsFailure Then
        Err.Raise ComErrInvalidArgument, "InfGenerateManifestItem", ValidationResult.Message
    End If

    Set Template = TemplateProvider.InfCreateTemplate(Item.InfGetTemplatePath())
    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "Template is invalid."
    End If

    Set Manifest = Item.InfToManifest()
    ApplyBodySource Manifest, Item
    ApplySectionSources Manifest, Item
    ApplyClassMemberSource Manifest, Item
    EnsureInsertionSections Manifest

    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Manifest)
    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "Generated text is empty."
    End If

    InfGenerateManifestItem = GeneratedText
End Function

' Generates a module body from a manifest item and supplied template text.
Public Function InfGenerateManifestItemFromTemplateText( _
    ByVal Item As ManifestItem, _
    ByVal TemplateName As String, _
    ByVal TemplateText As String) As String

    Dim Template As InfTemplate
    Dim Manifest As Object
    Dim GeneratedText As String

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfGenerateManifestItemFromTemplateText", "ManifestItem is required."
    End If

    If Not Item.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItemFromTemplateText", "ManifestItem is invalid."
    End If

    If TemplateProvider Is Nothing Or ManifestProvider Is Nothing Or TokenReplacer Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItemFromTemplateText", "Generator is not initialized."
    End If

    Set Template = TemplateProvider.InfCreateTemplateFromText(TemplateName, TemplateText)
    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItemFromTemplateText", "Template is invalid."
    End If

    Set Manifest = Item.InfToManifest()
    ApplyBodySource Manifest, Item
    ApplySectionSources Manifest, Item
    ApplyClassMemberSource Manifest, Item
    EnsureInsertionSections Manifest

    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Manifest)
    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItemFromTemplateText", "Generated text is empty."
    End If

    InfGenerateManifestItemFromTemplateText = GeneratedText
End Function

Private Sub ApplyBodySource(ByVal Manifest As Object, ByVal Item As ManifestItem)
    Dim BodySourcePath As String

    If Manifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplyBodySource", "Manifest is required."
    End If

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplyBodySource", "ManifestItem is required."
    End If

    BodySourcePath = Item.InfGetBodySourcePath()
    If ComIsBlankText(BodySourcePath) Then
        Exit Sub
    End If

    Manifest("BODY") = ManifestProvider.InfLoadBodySource(BodySourcePath)
End Sub

Private Sub ApplyClassMemberSource(ByVal Manifest As Object, ByVal Item As ManifestItem)
    Dim MemberSourcePath As String
    Dim MemberSourceText As String
    Dim ClassCodeGenerator As InfClassCodeGenerator
    Dim ClassSections As Object
    Dim SectionName As Variant

    If Manifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplyClassMemberSource", "Manifest is required."
    End If

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplyClassMemberSource", "ManifestItem is required."
    End If

    MemberSourceText = Item.InfGetMemberSourceText()
    MemberSourcePath = Item.InfGetMemberSourcePath()
    If ComIsBlankText(MemberSourceText) And ComIsBlankText(MemberSourcePath) Then
        Exit Sub
    End If

    If ComIsBlankText(MemberSourceText) Then
        MemberSourceText = ManifestProvider.InfLoadMemberSource(MemberSourcePath)
    End If

    Set ClassCodeGenerator = New InfClassCodeGenerator
    Set ClassSections = ClassCodeGenerator.InfRenderClassMemberSections(MemberSourceText)

    For Each SectionName In ClassSections.Keys
        Manifest(CStr(SectionName)) = CStr(ClassSections.Item(CStr(SectionName)))
        Manifest("Section:" & CStr(SectionName)) = CStr(ClassSections.Item(CStr(SectionName)))
    Next SectionName
End Sub

Private Sub ApplySectionSources(ByVal Manifest As Object, ByVal Item As ManifestItem)
    Dim SectionSourcePaths As Object
    Dim SectionName As Variant
    Dim SectionBody As String
    Dim InsertedBlocks As Object

    If Manifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplySectionSources", "Manifest is required."
    End If

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplySectionSources", "ManifestItem is required."
    End If

    Set SectionSourcePaths = Item.InfGetSectionSourcePaths()
    Set InsertedBlocks = CreateObject("Scripting.Dictionary")
    For Each SectionName In SectionSourcePaths.Keys
        SectionBody = NormalizeInsertionText( _
            ManifestProvider.InfLoadSectionSource(CStr(SectionSourcePaths.Item(CStr(SectionName)))))
        ApplySectionSource Manifest, CStr(SectionName), SectionBody, InsertedBlocks
    Next SectionName
End Sub

Private Sub EnsureInsertionSections(ByVal Manifest As Object)
    If Manifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "EnsureInsertionSections", "Manifest is required."
    End If

    EnsureInsertionSection Manifest, InsertModuleDeclaration
    EnsureInsertionSection Manifest, InsertDeclarationSectionEnd
    EnsureInsertionSection Manifest, InsertMemberDeclarations
    EnsureInsertionSection Manifest, InsertMemberBlock
    EnsureInsertionSection Manifest, InsertInitializeBody
    EnsureInsertionSection Manifest, InsertTerminateBody
    EnsureInsertionSection Manifest, InsertPropertyGroupStart
    EnsureInsertionSection Manifest, InsertProperties
    EnsureInsertionSection Manifest, InsertPropertyGroupEnd
    EnsureInsertionSection Manifest, InsertProcedureGroupStart
    EnsureInsertionSection Manifest, InsertProcedureGroupEnd
End Sub

Private Sub EnsureInsertionSection(ByVal Manifest As Object, ByVal SectionName As String)
    ComRequireText SectionName, "SectionName", "EnsureInsertionSection"

    If Not Manifest.Exists(SectionName) Then
        Manifest(SectionName) = vbNullString
    End If

    If Not Manifest.Exists("Section:" & SectionName) Then
        Manifest("Section:" & SectionName) = Manifest.Item(SectionName)
    End If
End Sub

Private Sub ApplySectionSource( _
    ByVal Manifest As Object, _
    ByVal SectionName As String, _
    ByVal SectionBody As String, _
    ByVal InsertedBlocks As Object)

    Dim BlockKey As String

    If Manifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ApplySectionSource", "Manifest is required."
    End If

    ComRequireText SectionName, "SectionName", "ApplySectionSource"

    If ComIsBlankText(SectionBody) Then
        Exit Sub
    End If

    BlockKey = NormalizeInsertionKey(SectionBody)
    If InsertedBlocks.Exists(BlockKey) Then
        Manifest(SectionName) = vbNullString
        Manifest("Section:" & SectionName) = vbNullString
        Exit Sub
    End If

    InsertedBlocks.Add BlockKey, True
    Manifest(SectionName) = SectionBody
    Manifest("Section:" & SectionName) = SectionBody
End Sub

Private Function NormalizeInsertionText(ByVal SourceText As String) As String
    Dim Lines() As String
    Dim FirstLine As Long
    Dim LastLine As Long
    Dim LineIndex As Long
    Dim Result As String

    SourceText = Replace(SourceText, vbCrLf, vbLf)
    SourceText = Replace(SourceText, vbCr, vbLf)
    Lines = Split(SourceText, vbLf)

    FirstLine = LBound(Lines)
    LastLine = UBound(Lines)

    Do While FirstLine <= LastLine And ComIsBlankText(Lines(FirstLine))
        FirstLine = FirstLine + 1
    Loop

    Do While LastLine >= FirstLine And ComIsBlankText(Lines(LastLine))
        LastLine = LastLine - 1
    Loop

    If FirstLine > LastLine Then
        NormalizeInsertionText = vbNullString
        Exit Function
    End If

    For LineIndex = FirstLine To LastLine
        If Len(Result) > 0 Then
            Result = Result & vbCrLf
        End If
        Result = Result & Lines(LineIndex)
    Next LineIndex

    NormalizeInsertionText = Result
End Function

Private Function NormalizeInsertionKey(ByVal SourceText As String) As String
    NormalizeInsertionKey = UCase$(Trim$(NormalizeInsertionText(SourceText)))
End Function
