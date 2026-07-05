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
Public Function InfCreateManifest(ByVal ModuleName As String, Optional ByVal TemplatePath As String = vbNullString) As Object
    Dim Manifest As Object

    ComRequireText ModuleName, "ModuleName", "InfCreateManifest"

    Set Manifest = CreateObject("Scripting.Dictionary")
    Manifest("ModuleName") = ModuleName
    Manifest("Layer") = "Standard"
    If Not ComIsBlankText(TemplatePath) Then
        Manifest("TemplatePath") = TemplatePath
    End If

    Set InfCreateManifest = Manifest
End Function
