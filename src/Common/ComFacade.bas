Option Explicit
Attribute VB_Name = "ComFacade"

'=========================================================================
' Module: ComFacade
' Layer: Common
' Responsibility: Published entry point for Common layer capabilities.
' Dependencies: ComError, ComErrorInfo, ComResult
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const ComDefaultSeverity As String = "Error"

'=========================================================================
' Public API
'=========================================================================

' Creates standardized Common error information.
'
' Parameters:
'   ErrorNumber: Stable project error number.
'   ErrorSource: Logical source that detected or owns the error.
'   ErrorDescription: Consumer-safe description of the failure.
'   ErrorSeverity: Error severity. Defaults to "Error".
'   IsRecoverable: True only when a defined recovery strategy exists.
'
' Return value:
'   Initialized ComErrorInfo instance.
'
' Raised errors:
'   Raises a standardized Common validation error when required values are invalid.
Public Function ComCreateErrorInfo( _
    ByVal ErrorNumber As Long, _
    ByVal ErrorSource As String, _
    ByVal ErrorDescription As String, _
    Optional ByVal ErrorSeverity As String = ComDefaultSeverity, _
    Optional ByVal IsRecoverable As Boolean = False) As ComErrorInfo

    Dim ErrorInfo As ComErrorInfo

    ' Normalize ErrorDescription to ensure ComErrorInfo validation does not fail
    If ComIsBlankText(ErrorDescription) Then
        ErrorDescription = "An unexpected error occurred."
    End If

    Set ErrorInfo = New ComErrorInfo
    ErrorInfo.ComInitializeErrorInfo ErrorNumber, ErrorSource, ErrorDescription, ErrorSeverity, IsRecoverable

    Set ComCreateErrorInfo = ErrorInfo
End Function

' Raises the supplied standardized Common error.
'
' Parameters:
'   ErrorInfo: Required Common error information.
'
' Raised errors:
'   Raises ErrorInfo as a VBA error. Raises a validation error when ErrorInfo is Nothing.
Public Sub ComRaiseError(ByVal ErrorInfo As ComErrorInfo)
    If ErrorInfo Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ComFacade", "ErrorInfo is required."
    End If

    Err.Raise ErrorInfo.Number, ErrorInfo.Source, ErrorInfo.Description
End Sub

' Creates a successful operation result.
'
' Parameters:
'   ResultMessage: Optional consumer-safe success message.
'
' Return value:
'   Initialized successful ComResult instance.
'
' Raised errors:
'   None.
Public Function ComCreateSuccess(Optional ByVal ResultMessage As String = vbNullString) As ComResult
    Dim Result As ComResult

    Set Result = New ComResult
    Result.ComInitializeSuccess ResultMessage

    Set ComCreateSuccess = Result
End Function

' Creates a failed operation result.
'
' Parameters:
'   Failure: Required Common error information.
'
' Return value:
'   Initialized failed ComResult instance.
'
' Raised errors:
'   Raises a standardized Common validation error when Failure is Nothing.
Public Function ComCreateFailure(ByVal Failure As ComErrorInfo) As ComResult
    Dim Result As ComResult

    Set Result = New ComResult
    Result.ComInitializeFailure Failure

    Set ComCreateFailure = Result
End Function

' Creates a failure result from the current VBA Err state.
'
' Parameters:
'   ErrorSource: Logical source that detected or owns the error.
'   ErrorDescription: Optional consumer-safe description of the failure.
'   ErrorSeverity: Error severity. Defaults to "Error".
'   IsRecoverable: True only when a defined recovery strategy exists.
'
' Return value:
'   Initialized failed ComResult instance.
'
' Raised errors:
'   Raises a standardized Common validation error when ErrorSource is blank.
Public Function ComCreateFailureFromErr( _
    ByVal ErrorSource As String, _
    Optional ByVal ErrorDescription As String = vbNullString, _
    Optional ByVal ErrorSeverity As String = ComDefaultSeverity, _
    Optional ByVal IsRecoverable As Boolean = False) As ComResult

    Dim ErrorInfo As ComErrorInfo
    Dim Description As String

    If ComIsBlankText(ErrorSource) Then
        Err.Raise ComErrInvalidArgument, "ComCreateFailureFromErr", "ErrorSource is required."
    End If

    Description = ErrorDescription
    If ComIsBlankText(Description) Then
        Description = Err.Description
    End If

    If ComIsBlankText(Description) Then
        Description = "An unexpected error occurred. (Err.Number=" & CStr(Err.Number) & ")"
    End If

    Set ErrorInfo = ComCreateErrorInfo(Err.Number, ErrorSource, Description, ErrorSeverity, IsRecoverable)
    Set ComCreateFailureFromErr = ComCreateFailure(ErrorInfo)
End Function

' Returns True when TextValue is empty after trimming.
'
' Parameters:
'   TextValue: Text to inspect.
'
' Return value:
'   True when TextValue contains no non-space characters.
'
' Raised errors:
'   None.
Public Function ComIsBlankText(ByVal TextValue As String) As Boolean
    ComIsBlankText = (Len(Trim$(TextValue)) = 0)
End Function

' Raises a standardized validation error when TextValue is blank.
'
' Parameters:
'   TextValue: Text to validate.
'   ParameterName: Name of the parameter being validated.
'   OperationName: Logical operation performing validation.
'
' Raised errors:
'   Raises a standardized Common validation error when TextValue or ParameterName is blank.
Public Sub ComRequireText( _
    ByVal TextValue As String, _
    ByVal ParameterName As String, _
    ByVal OperationName As String)

    If ComIsBlankText(ParameterName) Then
        Err.Raise ComErrInvalidArgument, "ComFacade", "ParameterName is required."
    End If

    If ComIsBlankText(OperationName) Then
        Err.Raise ComErrInvalidArgument, "ComFacade", "OperationName is required."
    End If

    If ComIsBlankText(TextValue) Then
        Err.Raise ComErrInvalidArgument, OperationName, ParameterName & " is required."
    End If
End Sub

' Returns text with leading and trailing spaces removed.
'
' Parameters:
'   TextValue: Text to normalize.
'
' Return value:
'   Trimmed text.
'
' Raised errors:
'   None.
Public Function ComTrimText(ByVal TextValue As String) As String
    ComTrimText = Trim$(TextValue)
End Function
