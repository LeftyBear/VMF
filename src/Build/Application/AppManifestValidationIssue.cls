VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppManifestValidationIssue"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppManifestValidationIssue
' Layer: Application
' Responsibility: Immutable manifest validation issue.
' Dependencies: Common
'=========================================================================

Private Type TMember
    Severity As Long
    Code As String
    Message As String
    ModuleName As String
    MemberName As String
    PropertyName As String
    SortOrder As Long
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal Severity As Long, _
    ByVal Code As String, _
    ByVal Message As String, _
    ByVal ModuleName As String, _
    ByVal MemberName As String, _
    ByVal PropertyName As String, _
    ByVal SortOrder As Long)

    ComRequireText Code, "Code", "AppManifestValidationIssue.AppInitialize"
    ComRequireText Message, "Message", "AppManifestValidationIssue.AppInitialize"
    ComRequireText PropertyName, "PropertyName", "AppManifestValidationIssue.AppInitialize"

    This.Severity = Severity
    This.Code = Code
    This.Message = Message
    This.ModuleName = ModuleName
    This.MemberName = MemberName
    This.PropertyName = PropertyName
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

Public Property Get ModuleName() As String
    EnsureInitialized
    ModuleName = This.ModuleName
End Property

Public Property Get MemberName() As String
    EnsureInitialized
    MemberName = This.MemberName
End Property

Public Property Get PropertyName() As String
    EnsureInitialized
    PropertyName = This.PropertyName
End Property

Public Property Get SortOrder() As Long
    EnsureInitialized
    SortOrder = This.SortOrder
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppManifestValidationIssue", "Validation issue is not initialized."
    End If
End Sub
