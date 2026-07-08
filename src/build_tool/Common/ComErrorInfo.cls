VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ComErrorInfo"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ComErrorInfo
' Layer: Common
' Responsibility: Immutable project error information value.
' Dependencies: ComError
'=========================================================================

Private Const ComDefaultSeverity As String = "Error"

'=========================================================================
' Private Types
'=========================================================================

Private Type TMember
    ErrorNumber As Long
    ErrorSource As String
    ErrorDescription As String
    ErrorSeverity As String
    IsRecoverable As Boolean
    IsInitialized As Boolean
End Type

'=========================================================================
' Module Variables
'=========================================================================

Private This As TMember

'=========================================================================
' Public API
'=========================================================================

' Initializes this error information value.
'
' Parameters:
'   ErrorNumber: Stable project error number.
'   ErrorSource: Logical source that detected or owns the error.
'   ErrorDescription: Consumer-safe description of the failure.
'   ErrorSeverity: Error severity. Defaults to "Error".
'   IsRecoverable: True only when a defined recovery strategy exists.
'
' Raised errors:
'   Raises a standardized Common validation error when required values are invalid.
Public Sub ComInitializeErrorInfo( _
    ByVal ErrorNumber As Long, _
    ByVal ErrorSource As String, _
    ByVal ErrorDescription As String, _
    Optional ByVal ErrorSeverity As String = ComDefaultSeverity, _
    Optional ByVal IsRecoverable As Boolean = False)

    ValidateRequiredText ErrorSource, "ErrorSource"
    ValidateRequiredText ErrorDescription, "ErrorDescription"
    ValidateRequiredText ErrorSeverity, "ErrorSeverity"

    This.ErrorNumber = ErrorNumber
    This.ErrorSource = ErrorSource
    This.ErrorDescription = ErrorDescription
    This.ErrorSeverity = ErrorSeverity
    This.IsRecoverable = IsRecoverable
    This.IsInitialized = True
End Sub

' Returns the stable project error number.
Public Property Get Number() As Long
    EnsureInitialized
    Number = This.ErrorNumber
End Property

' Returns the logical source that detected or owns the error.
Public Property Get Source() As String
    EnsureInitialized
    Source = This.ErrorSource
End Property

' Returns the consumer-safe failure description.
Public Property Get Description() As String
    EnsureInitialized
    Description = This.ErrorDescription
End Property

' Returns the error severity label.
Public Property Get Severity() As String
    EnsureInitialized
    Severity = This.ErrorSeverity
End Property

' Returns True only when a defined recovery strategy exists.
Public Property Get IsRecoverable() As Boolean
    EnsureInitialized
    IsRecoverable = This.IsRecoverable
End Property

' Returns a deterministic single-line representation for diagnostics.
Public Function ToText() As String
    EnsureInitialized
    ToText = This.ErrorSource & ": " & This.ErrorDescription
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "ComErrorInfo", "ComErrorInfo is not initialized."
    End If
End Sub

Private Sub ValidateRequiredText(ByVal TextValue As String, ByVal ParameterName As String)
    If Len(Trim$(TextValue)) = 0 Then
        Err.Raise ComErrInvalidArgument, "ComErrorInfo", ParameterName & " is required."
    End If
End Sub
