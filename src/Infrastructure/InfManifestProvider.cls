VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfManifestProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfManifestProvider
' Layer: Infrastructure
' Responsibility: Build manifest values for generation requests.
' Dependencies: Common
'==========================================================================

' Creates a manifest dictionary for the supplied generation inputs.
Public Function InfCreateManifest( _
    ByVal ModuleName As String, _
    Optional ByVal TemplatePath As String = vbNullString, _
    Optional ByVal LayerName As String = "Application") As Object

    Dim Manifest As Object

    ComRequireText ModuleName, "ModuleName", "InfCreateManifest"
    ComRequireText LayerName, "LayerName", "InfCreateManifest"
    ValidateLayerName LayerName

    Set Manifest = CreateObject("Scripting.Dictionary")
    Manifest("ModuleName") = ModuleName
    Manifest("Layer") = LayerName
    If Not ComIsBlankText(TemplatePath) Then
        Manifest("TemplatePath") = TemplatePath
    End If

    Set InfCreateManifest = Manifest
End Function

Private Sub ValidateLayerName(ByVal LayerName As String)
    Select Case LayerName
        Case "Presentation", "Application", "Domain", "Infrastructure", "Common"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "InfCreateManifest", "LayerName is not defined by LayerSpecification."
End Sub
