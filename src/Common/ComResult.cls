VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ComResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ComResult
' Layer: Common
' Responsibility: Operation result value with success/failure state.
' Dependencies: ComError, ComErrorInfo
'=========================================================================

'=========================================================================
' Private Types
'=========================================================================

Private Type TMember
    IsInitialized As Boolean
    IsSuccess As Boolean
    Message As String
    Failure As ComErrorInfo
End Type

'=========================================================================
' Module Variables
'=========================================================================

Private This As TMember

'=========================================================================
' Public API
'=========================================================================

' Initializes this result as a successful result.
'
' Parameters:
'   ResultMessage: Optional consumer-safe success message.
'
' Raised errors:
'   None.
Public Sub ComInitializeSuccess(Optional ByVal ResultMessage As String = vbNullString)
    This.IsSuccess = True
    This.Message = ResultMessage
    Set This.Failure = Nothing
    This.IsInitialized = True
End Sub

' Initializes this result as a failed result.
'
' Parameters:
'   Failure: Required Common error information.
'
' Raised errors:
'   Raises a standardized Common validation error when Failure is Nothing.
Public Sub ComInitializeFailure(ByVal Failure As ComErrorInfo)
    If Failure Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ComResult", "Failure is required."
    End If

    This.IsSuccess = False
    This.Message = Failure.Description
    Set This.Failure = Failure
    This.IsInitialized = True
End Sub

' Returns True when the operation completed successfully.
Public Property Get IsSuccess() As Boolean
    EnsureInitialized
    IsSuccess = This.IsSuccess
End Property

' Returns True when the operation failed.
Public Property Get IsFailure() As Boolean
    EnsureInitialized
    IsFailure = Not This.IsSuccess
End Property

' Returns the consumer-safe result message.
Public Property Get Message() As String
    EnsureInitialized
    Message = This.Message
End Property

' Returns the failure details for a failed result.
'
' Raised errors:
'   Raises a standardized Common validation error when this result is successful.
Public Property Get Failure() As ComErrorInfo
    EnsureInitialized

    If This.IsSuccess Then
        Err.Raise ComErrInvalidState, "ComResult", "Successful results do not contain failure details."
    End If

    Set Failure = This.Failure
End Property

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "ComResult", "ComResult is not initialized."
    End If
End Sub
