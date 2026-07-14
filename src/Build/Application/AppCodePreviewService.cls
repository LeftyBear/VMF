VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppCodePreviewService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppCodePreviewService
' Layer: Application
' Responsibility: Render unsaved manifest editor module previews.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Generator As InfGenerator
Private ManifestEditorService As AppManifestEditorService

' Initializes the preview service with canonical generation collaborators.
Public Sub AppInitialize( _
    ByVal ModuleGenerator As InfGenerator, _
    ByVal EditorService As AppManifestEditorService)

    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppCodePreviewService.AppInitialize", "ModuleGenerator is required."
    End If

    If EditorService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppCodePreviewService.AppInitialize", "ManifestEditorService is required."
    End If

    Set Generator = ModuleGenerator
    Set ManifestEditorService = EditorService
End Sub

' Renders one unsaved manifest editor module without writing manifest or source files.
Public Function AppPreviewManifestEditorModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByRef PreviewText As String) As ComResult

    Dim Item As ManifestItem
    Dim CodeText As String

    On Error GoTo ErrHandler

    PreviewText = vbNullString
    ComRequireText ManifestPath, "ManifestPath", "AppPreviewManifestEditorModule"

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppPreviewManifestEditorModule", "GeneratorService is not initialized."
    End If

    ValidateModuleInfo ModuleInfo
    Set Item = CreateManifestItem(ManifestPath, ModuleInfo)
    CodeText = Generator.InfGenerateManifestItem(Item)

    If ComIsBlankText(CodeText) Then
        Err.Raise ComErrInvalidState, "AppPreviewManifestEditorModule", "Generated text is empty."
    End If

    If HasUnresolvedPlaceholder(CodeText) Then
        Set AppPreviewManifestEditorModule = CreatePreviewFailure("Unresolved placeholder: generated code still contains template placeholders.")
        Exit Function
    End If

    PreviewText = CodeText
    Set AppPreviewManifestEditorModule = ComCreateSuccess("Preview generated.")
    Exit Function

ErrHandler:
    Set AppPreviewManifestEditorModule = CreatePreviewFailure(ClassifyPreviewError(Err.Source, Err.Description))
End Function

Private Function CreateManifestItem(ByVal ManifestPath As String, ByVal ModuleInfo As Object) As ManifestItem
    Dim Item As ManifestItem
    Dim FieldText As Variant
    Dim SeparatorIndex As Long
    Dim SectionName As String
    Dim SectionSourcePath As String

    Set Item = New ManifestItem
    Item.InfInitialize _
        CStr(ModuleInfo("ModuleName")), _
        CStr(ModuleInfo("ModuleType")), _
        CStr(ModuleInfo("Layer")), _
        ResolveManifestRelativePath(CStr(ModuleInfo("TemplatePath")), ManifestPath), _
        ResolveOptionalManifestPath(CStr(ModuleInfo("BodySourcePath")), ManifestPath)

    For Each FieldText In ModuleInfo("SectionFields")
        SeparatorIndex = InStr(1, CStr(FieldText), "=", vbBinaryCompare)
        If SeparatorIndex <= 1 Or SeparatorIndex = Len(CStr(FieldText)) Then
            Err.Raise ComErrInvalidArgument, "AppCodePreviewService.CreateManifestItem", "SectionSource entry must use SectionName=SectionSourcePath."
        End If

        SectionName = Trim$(Left$(CStr(FieldText), SeparatorIndex - 1))
        SectionSourcePath = Trim$(Mid$(CStr(FieldText), SeparatorIndex + 1))
        Item.InfAddSectionSourcePath SectionName, ResolveManifestRelativePath(SectionSourcePath, ManifestPath)
    Next FieldText

    If ModuleInfo("Members").Count > 0 Then
        Item.InfSetMemberSourceText ManifestEditorService.AppRenderManifestEditorMemberDefinitions(ModuleInfo("Members"))
    ElseIf Not ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) Then
        Item.InfSetMemberSourcePath ResolveManifestRelativePath(CStr(ModuleInfo("MemberSourcePath")), ManifestPath)
    End If

    Set CreateManifestItem = Item
End Function

Private Sub ValidateModuleInfo(ByVal ModuleInfo As Object)
    If ModuleInfo Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppCodePreviewService.ValidateModuleInfo", "Manifest module is required."
    End If

    ValidateRequiredKey ModuleInfo, "ModuleName"
    ValidateRequiredKey ModuleInfo, "Layer"
    ValidateRequiredKey ModuleInfo, "ModuleType"
    ValidateRequiredKey ModuleInfo, "TemplatePath"

    ValidateIdentifier CStr(ModuleInfo("ModuleName")), "ModuleName"
    ValidateIdentifier CStr(ModuleInfo("Layer")), "Layer"
    Select Case CStr(ModuleInfo("ModuleType"))
        Case "StandardModule", "ClassModule"
        Case Else
            Err.Raise ComErrInvalidArgument, "AppCodePreviewService.ValidateModuleInfo", "ModuleType must be StandardModule or ClassModule."
    End Select
End Sub

Private Sub ValidateRequiredKey(ByVal Values As Object, ByVal KeyName As String)
    If Not Values.Exists(KeyName) Then
        Err.Raise ComErrInvalidArgument, "AppCodePreviewService.ValidateRequiredKey", KeyName & " is required."
    End If

    ComRequireText CStr(Values(KeyName)), KeyName, "AppCodePreviewService.ValidateRequiredKey"
End Sub

Private Sub ValidateIdentifier(ByVal Identifier As String, ByVal ValueName As String)
    Dim Position As Long
    Dim Character As String

    ComRequireText Identifier, ValueName, "AppCodePreviewService.ValidateIdentifier"

    Character = Mid$(Identifier, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        Err.Raise ComErrInvalidArgument, "AppCodePreviewService.ValidateIdentifier", ValueName & " must start with a letter."
    End If

    For Position = 1 To Len(Identifier)
        Character = Mid$(Identifier, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            Err.Raise ComErrInvalidArgument, "AppCodePreviewService.ValidateIdentifier", ValueName & " must contain only letters, numbers, or underscore."
        End If
    Next Position
End Sub

Private Function ClassifyPreviewError(ByVal ErrorSource As String, ByVal ErrorDescription As String) As String
    Dim SourceText As String
    Dim DescriptionText As String
    Dim CombinedText As String

    SourceText = CStr(ErrorSource)
    DescriptionText = Trim$(CStr(ErrorDescription))
    CombinedText = LCase$(SourceText & " " & DescriptionText)

    If InStr(1, CombinedText, "template", vbTextCompare) > 0 Then
        ClassifyPreviewError = "Template load failed: " & DescriptionText
    ElseIf InStr(1, CombinedText, "placeholder", vbTextCompare) > 0 Then
        ClassifyPreviewError = "Unresolved placeholder: " & DescriptionText
    ElseIf InStr(1, CombinedText, "section", vbTextCompare) > 0 Then
        ClassifyPreviewError = "Section insertion failed: " & DescriptionText
    ElseIf InStr(1, CombinedText, "manifest") > 0 _
        Or InStr(1, CombinedText, "modulename") > 0 _
        Or InStr(1, CombinedText, "moduletype") > 0 _
        Or InStr(1, CombinedText, "layer") > 0 _
        Or InStr(1, CombinedText, "member") > 0 Then
        ClassifyPreviewError = "Manifest input invalid: " & DescriptionText
    Else
        ClassifyPreviewError = "Internal generation error: " & DescriptionText
    End If
End Function

Private Function CreatePreviewFailure(ByVal Description As String) As ComResult
    Set CreatePreviewFailure = ComCreateFailure( _
        ComCreateErrorInfo(ComErrInvalidState, "AppCodePreviewService", Description))
End Function

Private Function HasUnresolvedPlaceholder(ByVal CodeText As String) As Boolean
    HasUnresolvedPlaceholder = (InStr(1, CodeText, "{{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}}", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}", vbBinaryCompare) > 0)
End Function

Private Function ResolveOptionalManifestPath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    If ComIsBlankText(FilePath) Then
        ResolveOptionalManifestPath = vbNullString
    Else
        ResolveOptionalManifestPath = ResolveManifestRelativePath(FilePath, ManifestPath)
    End If
End Function

Private Function ResolveManifestRelativePath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    Dim ManifestDirectoryPath As String
    Dim NormalizedFilePath As String
    Dim FileSystem As Object

    If IsAbsolutePath(FilePath) Then
        ResolveManifestRelativePath = FilePath
        Exit Function
    End If

    ManifestDirectoryPath = GetParentFolderPath(ManifestPath)
    NormalizedFilePath = Replace(FilePath, "/", "\")

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ResolveManifestRelativePath = FileSystem.GetAbsolutePathName( _
        BuildPathResolver.CombinePath(ManifestDirectoryPath, NormalizedFilePath))
End Function

Private Function IsAbsolutePath(ByVal FilePath As String) As Boolean
    IsAbsolutePath = ((Len(FilePath) >= 3 _
            And (Mid$(FilePath, 2, 2) = ":\" Or Mid$(FilePath, 2, 2) = ":/")) _
        Or Left$(FilePath, 2) = "\\" _
        Or Left$(FilePath, 2) = "//")
End Function

Private Function GetParentFolderPath(ByVal FilePath As String) As String
    Dim SeparatorIndex As Long

    SeparatorIndex = InStrRev(FilePath, "\")
    If SeparatorIndex = 0 Then
        SeparatorIndex = InStrRev(FilePath, "/")
    End If

    If SeparatorIndex = 0 Then
        GetParentFolderPath = "."
    Else
        GetParentFolderPath = Left$(FilePath, SeparatorIndex - 1)
    End If
End Function
