VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateValidationIssue"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateValidationIssue
' Layer: Application
' Responsibility: Immutable template validation issue.
' Dependencies: Common
'=========================================================================

Private Type TMember
    Severity As Long
    Code As String
    Message As String
    TemplateName As String
    ItemName As String
    LineNumber As Long
    SortOrder As Long
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal Severity As Long, _
    ByVal Code As String, _
    ByVal Message As String, _
    ByVal TemplateName As String, _
    ByVal ItemName As String, _
    ByVal LineNumber As Long, _
    ByVal SortOrder As Long)

    ComRequireText Code, "Code", "AppTemplateValidationIssue.AppInitialize"
    ComRequireText Message, "Message", "AppTemplateValidationIssue.AppInitialize"

    This.Severity = Severity
    This.Code = Code
    This.Message = Message
    This.TemplateName = TemplateName
    This.ItemName = ItemName
    This.LineNumber = LineNumber
    This.SortOrder = SortOrder
    This.IsInitialized = True
End Sub

Public Property Get Severity() As Long
    EnsureInitialized
    Severity = This.Severity
End Property

Public Property Get SeverityText() As String
    EnsureInitialized
    Select Case This.Severity
        Case 1
            SeverityText = "Error"
        Case 2
            SeverityText = "Warning"
        Case 3
            SeverityText = "Information"
        Case Else
            SeverityText = "Unknown"
    End Select
End Property

Public Property Get Code() As String
    EnsureInitialized
    Code = This.Code
End Property

Public Property Get Message() As String
    EnsureInitialized
    Message = This.Message
End Property

Public Property Get TemplateName() As String
    EnsureInitialized
    TemplateName = This.TemplateName
End Property

Public Property Get ItemName() As String
    EnsureInitialized
    ItemName = This.ItemName
End Property

Public Property Get LineNumber() As Long
    EnsureInitialized
    LineNumber = This.LineNumber
End Property

Public Property Get SortOrder() As Long
    EnsureInitialized
    SortOrder = This.SortOrder
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppTemplateValidationIssue", "Template validation issue is not initialized."
    End If
End Sub
