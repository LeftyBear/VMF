VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppStudioSettingsIssue"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppStudioSettingsIssue
' Layer: Application
' Responsibility: Immutable Studio settings validation issue.
' Dependencies: Common
'=========================================================================

Private Type TMember
    Severity As Long
    Code As String
    Message As String
    SettingKey As String
    SortOrder As Long
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize(ByVal Severity As Long, ByVal Code As String, ByVal Message As String, ByVal SettingKey As String, ByVal SortOrder As Long)
    ComRequireText Code, "Code", "AppStudioSettingsIssue.AppInitialize"
    ComRequireText Message, "Message", "AppStudioSettingsIssue.AppInitialize"
    This.Severity = Severity
    This.Code = Code
    This.Message = Message
    This.SettingKey = SettingKey
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
        Case 1: SeverityText = "Error"
        Case 2: SeverityText = "Warning"
        Case 3: SeverityText = "Information"
        Case Else: SeverityText = "Unknown"
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

Public Property Get SettingKey() As String
    EnsureInitialized
    SettingKey = This.SettingKey
End Property

Public Property Get SortOrder() As Long
    EnsureInitialized
    SortOrder = This.SortOrder
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppStudioSettingsIssue", "Studio settings issue is not initialized."
    End If
End Sub
