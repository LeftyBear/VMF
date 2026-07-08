VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfTokenReplacer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfTokenReplacer
' Layer: Infrastructure
' Responsibility: Replace template placeholders using manifest values.
' Dependencies: Common, Infrastructure
'==========================================================================

' Replaces placeholders in an InfTemplate using values from a manifest.
Public Function InfReplaceTokens(ByVal Template As InfTemplate, ByVal Manifest As Object) As String
    Dim TemplateText As String
    Dim ManifestKey As Variant
    Dim ManifestValue As Variant

    If Template Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfReplaceTokens", "Template is required."
    End If

    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfReplaceTokens", "Template is invalid."
    End If

    TemplateText = Template.InfGetTemplateText()

    If Manifest Is Nothing Then
        InfReplaceTokens = TemplateText
        Exit Function
    End If

    For Each ManifestKey In Manifest.Keys
        ManifestValue = Manifest.Item(CStr(ManifestKey))
        TemplateText = Replace(TemplateText, "{" & CStr(ManifestKey) & "}", CStr(ManifestValue))
    Next ManifestKey

    InfReplaceTokens = TemplateText
End Function
