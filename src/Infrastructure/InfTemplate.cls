VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfTemplate"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfTemplate
' Layer: Infrastructure
' Responsibility: Represent a single template payload.
' Dependencies: Common
'==========================================================================

Private Type TMember
    TemplateName As String
    TemplateText As String
End Type

Private This As TMember

' Sets the template name.
Public Sub InfSetTemplateName(ByVal TemplateName As String)
    ComRequireText TemplateName, "TemplateName", "InfSetTemplateName"
    This.TemplateName = TemplateName
End Sub

' Sets the template text content.
Public Sub InfSetTemplateText(ByVal TemplateText As String)
    If ComIsBlankText(TemplateText) Then
        Err.Raise ComErrInvalidArgument, "InfSetTemplateText", "TemplateText is required."
    End If

    This.TemplateText = TemplateText
End Sub

' Returns the template name.
Public Function InfGetTemplateName() As String
    If Not InfValidate() Then
        Err.Raise ComErrInvalidState, "InfTemplate", "Template is not initialized."
    End If

    InfGetTemplateName = This.TemplateName
End Function

' Returns the template text content.
Public Function InfGetTemplateText() As String
    If Not InfValidate() Then
        Err.Raise ComErrInvalidState, "InfTemplate", "Template is not initialized."
    End If

    InfGetTemplateText = This.TemplateText
End Function

' Returns True when the template has usable content.
Public Function InfHasContent() As Boolean
    InfHasContent = InfValidate()
End Function

' Validates the template state.
Public Function InfValidate() As Boolean
    If ComIsBlankText(This.TemplateName) Then
        InfValidate = False
        Exit Function
    End If

    If ComIsBlankText(This.TemplateText) Then
        InfValidate = False
        Exit Function
    End If

    InfValidate = True
End Function
