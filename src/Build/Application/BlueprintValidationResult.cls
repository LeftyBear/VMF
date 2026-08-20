VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BlueprintValidationResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: BlueprintValidationResult
' Layer: Application
' Responsibility: Blueprint validator result.
' Dependencies: Application
'=========================================================================

Private Type TMember
    ResultKind As String
    Generatable As Boolean
    Diagnostics As Collection
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize(ByVal ResultKind As String, ByVal Generatable As Boolean, ByVal Diagnostics As Collection)
    ComRequireText ResultKind, "ResultKind", "BlueprintValidationResult.AppInitialize"
    If Diagnostics Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BlueprintValidationResult.AppInitialize", "Diagnostics is required."
    End If

    This.ResultKind = ResultKind
    This.Generatable = Generatable
    Set This.Diagnostics = Diagnostics
    This.IsInitialized = True
End Sub

Public Property Get ResultKind() As String
    EnsureInitialized
    ResultKind = This.ResultKind
End Property

Public Property Get Generatable() As Boolean
    EnsureInitialized
    Generatable = This.Generatable
End Property

Public Property Get Diagnostics() As Collection
    EnsureInitialized
    Set Diagnostics = This.Diagnostics
End Property

Public Function HasDiagnosticCode(ByVal Code As String) As Boolean
    Dim Diagnostic As BlueprintValidationDiagnostic

    EnsureInitialized
    For Each Diagnostic In This.Diagnostics
        If Diagnostic.Code = Code Then
            HasDiagnosticCode = True
            Exit Function
        End If
    Next Diagnostic
End Function

Public Function ErrorCount() As Long
    Dim Diagnostic As BlueprintValidationDiagnostic

    EnsureInitialized
    For Each Diagnostic In This.Diagnostics
        If LCase$(Diagnostic.Severity) = BlueprintValidationSeverityError() Then
            ErrorCount = ErrorCount + 1
        End If
    Next Diagnostic
End Function

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "BlueprintValidationResult", "Blueprint validation result is not initialized."
    End If
End Sub
