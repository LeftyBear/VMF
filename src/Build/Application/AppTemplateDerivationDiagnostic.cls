VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateDerivationDiagnostic"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateDerivationDiagnostic
' Layer: Application
' Responsibility: Represent one deterministic Template Derivation failure.
' Dependencies: Common
'=========================================================================

Private Type TMember
    Code As String
    Category As String
    ItemIndex As Long
    ModuleName As String
    FieldName As String
    Message As String
    SortOrder As Long
    Severity As String
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal Code As String, _
    ByVal Category As String, _
    ByVal ItemIndex As Long, _
    ByVal ModuleName As String, _
    ByVal FieldName As String, _
    ByVal Message As String, _
    ByVal SortOrder As Long)

    ComRequireText Code, "Code", "AppTemplateDerivationDiagnostic.AppInitialize"
    ComRequireText Category, "Category", "AppTemplateDerivationDiagnostic.AppInitialize"
    ComRequireText Message, "Message", "AppTemplateDerivationDiagnostic.AppInitialize"
    If ItemIndex < 0 Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationDiagnostic.AppInitialize", "ItemIndex must not be negative."
    End If
    If SortOrder < 0 Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationDiagnostic.AppInitialize", "SortOrder must not be negative."
    End If

    This.Code = Code
    This.Category = Category
    This.ItemIndex = ItemIndex
    This.ModuleName = ModuleName
    This.FieldName = FieldName
    This.Message = Message
    This.SortOrder = SortOrder
    This.Severity = "Error"
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

Public Property Get ItemIndex() As Long
    EnsureInitialized
    ItemIndex = This.ItemIndex
End Property

Public Property Get ModuleName() As String
    EnsureInitialized
    ModuleName = This.ModuleName
End Property

Public Property Get FieldName() As String
    EnsureInitialized
    FieldName = This.FieldName
End Property

Public Property Get Message() As String
    EnsureInitialized
    Message = This.Message
End Property

Public Property Get SortOrder() As Long
    EnsureInitialized
    SortOrder = This.SortOrder
End Property

Public Property Get Severity() As String
    EnsureInitialized
    Severity = This.Severity
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppTemplateDerivationDiagnostic", "Template derivation diagnostic is not initialized."
    End If
End Sub
