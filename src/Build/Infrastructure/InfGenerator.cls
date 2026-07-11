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
' Responsibility: Orchestrate template-driven generation.
' Dependencies: Common, Infrastructure
'==========================================================================

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
    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Manifest)

    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Generated text is empty."
    End If

    InfGenerateModule = GeneratedText
End Function

' Generates a module body from a manifest-driven generation item.
Public Function InfGenerateManifestItem(ByVal Item As ManifestItem) As String
    Dim Template As InfTemplate
    Dim GeneratedText As String
    Dim ValidationResult As ComResult

    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfGenerateManifestItem", "ManifestItem is required."
    End If

    If Not Item.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "ManifestItem is invalid."
    End If

    If TemplateProvider Is Nothing Or TokenReplacer Is Nothing Then
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

    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Item.InfToManifest())
    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateManifestItem", "Generated text is empty."
    End If

    InfGenerateManifestItem = GeneratedText
End Function
