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

' Generates a module body using template and manifest data.
Public Function InfGenerateModule(ByVal ModuleName As String, ByVal TemplatePath As String) As String
    Dim Template As InfTemplate
    Dim Manifest As Object
    Dim GeneratedText As String

    ComRequireText ModuleName, "ModuleName", "InfGenerateModule"
    ComRequireText TemplatePath, "TemplatePath", "InfGenerateModule"

    If TemplateProvider Is Nothing Or ManifestProvider Is Nothing Or TokenReplacer Is Nothing Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Generator is not initialized."
    End If

    Set Template = TemplateProvider.InfCreateTemplate(TemplatePath)
    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Template is invalid."
    End If

    Set Manifest = ManifestProvider.InfCreateManifest(ModuleName, TemplatePath)
    GeneratedText = TokenReplacer.InfReplaceTokens(Template, Manifest)

    If ComIsBlankText(GeneratedText) Then
        Err.Raise ComErrInvalidState, "InfGenerateModule", "Generated text is empty."
    End If

    InfGenerateModule = GeneratedText
End Function
