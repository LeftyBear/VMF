VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AttributeWriter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AttributeWriter
' Layer: Application
' Responsibility: Apply component-level VBA attributes.
' Dependencies: Common
'=========================================================================

' Applies the generated module name to the VBA component.
Public Sub WriteAttributes(ByVal Component As Object, ByVal ModuleName As String)
    If Component Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AttributeWriter.WriteAttributes", "Component is required."
    End If

    ComRequireText ModuleName, "ModuleName", "AttributeWriter.WriteAttributes"
    Component.Name = ModuleName
End Sub
