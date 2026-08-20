VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BlueprintValidationDiagnostic"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: BlueprintValidationDiagnostic
' Layer: Application
' Responsibility: Blueprint validator diagnostic result.
' Dependencies: Common
'=========================================================================

Private Type TMember
    Code As String
    Category As String
    Severity As String
    FieldPath As String
    Message As String
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal Code As String, _
    ByVal Category As String, _
    ByVal Severity As String, _
    Optional ByVal FieldPath As String = vbNullString, _
    Optional ByVal Message As String = vbNullString)

    ComRequireText Code, "Code", "BlueprintValidationDiagnostic.AppInitialize"
    ComRequireText Category, "Category", "BlueprintValidationDiagnostic.AppInitialize"
    ComRequireText Severity, "Severity", "BlueprintValidationDiagnostic.AppInitialize"

    This.Code = Code
    This.Category = Category
    This.Severity = Severity
    This.FieldPath = FieldPath
    This.Message = Message
    This.IsInitialized = True
End Sub

Public Property Get Code() As String
    EnsureInitialized
    Code = This.Code
End Property

Public Property Get Category() As String
    EnsureInitialized
    Category = This.Category
End Property

Public Property Get Severity() As String
    EnsureInitialized
    Severity = This.Severity
End Property

Public Property Get FieldPath() As String
    EnsureInitialized
    FieldPath = This.FieldPath
End Property

Public Property Get Message() As String
    EnsureInitialized
    Message = This.Message
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "BlueprintValidationDiagnostic", "Blueprint validation diagnostic is not initialized."
    End If
End Sub
