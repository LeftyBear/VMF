VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "SourceWriter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: SourceWriter
' Layer: Application
' Responsibility: Write normalized source into a VBA component.
' Dependencies: Common
'=========================================================================

' Replaces generated component source with CodeText.
Public Sub WriteSource(ByVal Component As Object, ByVal CodeText As String)
    Dim CodeModule As Object

    If Component Is Nothing Then
        Err.Raise ComErrInvalidArgument, "SourceWriter.WriteSource", "Component is required."
    End If

    ComRequireText CodeText, "CodeText", "SourceWriter.WriteSource"

    Set CodeModule = Component.CodeModule
    If CodeModule.CountOfLines > 0 Then
        CodeModule.DeleteLines 1, CodeModule.CountOfLines
    End If

    CodeModule.AddFromString CodeText
End Sub
