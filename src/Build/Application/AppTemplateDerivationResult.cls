VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateDerivationResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateDerivationResult
' Layer: Application
' Responsibility: Represent atomic Template Derivation success or failure.
' Dependencies: Common, Application
'=========================================================================

Private Type TMember
    IsSuccess As Boolean
    Items As Collection
    Diagnostics As Collection
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize(ByVal IsSuccess As Boolean, ByVal Items As Collection, ByVal Diagnostics As Collection)
    If Items Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationResult.AppInitialize", "Items is required."
    End If
    If Diagnostics Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationResult.AppInitialize", "Diagnostics is required."
    End If

    If IsSuccess Then
        If Items.Count = 0 Or Diagnostics.Count <> 0 Then
            Err.Raise ComErrInvalidState, "AppTemplateDerivationResult.AppInitialize", "Successful result state is inconsistent."
        End If
    ElseIf Items.Count <> 0 Or Diagnostics.Count = 0 Then
        Err.Raise ComErrInvalidState, "AppTemplateDerivationResult.AppInitialize", "Failed result state is inconsistent."
    End If

    This.IsSuccess = IsSuccess
    Set This.Items = Items
    Set This.Diagnostics = Diagnostics
    This.IsInitialized = True
End Sub

Public Property Get IsSuccess() As Boolean
    EnsureInitialized
    IsSuccess = This.IsSuccess
End Property

Public Property Get Items() As Collection
    EnsureInitialized
    Set Items = This.Items
End Property

Public Property Get Diagnostics() As Collection
    EnsureInitialized
    Set Diagnostics = This.Diagnostics
End Property

Public Function HasDiagnosticCode(ByVal Code As String) As Boolean
    Dim Diagnostic As AppTemplateDerivationDiagnostic

    EnsureInitialized
    For Each Diagnostic In This.Diagnostics
        If Diagnostic.Code = Code Then
            HasDiagnosticCode = True
            Exit Function
        End If
    Next Diagnostic
End Function

Public Function ErrorCount() As Long
    Dim Diagnostic As AppTemplateDerivationDiagnostic

    EnsureInitialized
    For Each Diagnostic In This.Diagnostics
        If StrComp(Diagnostic.Severity, "Error", vbTextCompare) = 0 Then
            ErrorCount = ErrorCount + 1
        End If
    Next Diagnostic
End Function

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppTemplateDerivationResult", "Template derivation result is not initialized."
    End If
End Sub
