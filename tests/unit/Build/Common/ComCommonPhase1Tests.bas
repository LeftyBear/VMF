Option Explicit
Attribute VB_Name = "ComCommonPhase1Tests"

'=========================================================================
' Module: ComCommonPhase1Tests
' Layer: Common
' Responsibility: Focused verification tests for Phase 1 Common contracts.
' Dependencies: ComFacade, ComErrorInfo, ComResult
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const ComTestErrorNumber As Long = vbObjectError + 2000
Private Const ComTestFailureNumber As Long = vbObjectError + 2001
Private Const ComTestAssertTrueErrorNumber As Long = vbObjectError + 9000
Private Const ComTestAssertFalseErrorNumber As Long = vbObjectError + 9001
Private Const ComTestAssertEqualsErrorNumber As Long = vbObjectError + 9002

'=========================================================================
' Public API
'=========================================================================

' Runs the Phase 1 Common verification tests.
'
' Parameters:
'   None.
'
' Raised errors:
'   Raises a test failure error when an assertion fails.
Public Sub ComRunCommonPhase1Tests()
    VerifyInitialize
    VerifyErrorInfo
    VerifySuccessResult
    VerifyFailureResult
    VerifyTextValidation
End Sub

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub VerifyInitialize()
    Dim CompositionRoot As ComCompositionRoot

    Set CompositionRoot = New ComCompositionRoot

    AssertTrue CompositionRoot.ComInitialize(), "ComInitialize should return True."
    CompositionRoot.ComShutdown
End Sub

Private Sub VerifyErrorInfo()
    Dim ErrorInfo As ComErrorInfo

    Set ErrorInfo = ComCreateErrorInfo(ComTestErrorNumber, "ComCommonPhase1Tests", "Expected failure.")

    AssertEquals ComTestErrorNumber, ErrorInfo.Number, "Error number should match."
    AssertEquals "ComCommonPhase1Tests", ErrorInfo.Source, "Error source should match."
    AssertEquals "Expected failure.", ErrorInfo.Description, "Error description should match."
    AssertEquals "Error", ErrorInfo.Severity, "Default severity should match."
    AssertFalse ErrorInfo.IsRecoverable, "Default recoverability should be False."
End Sub

Private Sub VerifySuccessResult()
    Dim Result As ComResult

    Set Result = ComCreateSuccess("Completed.")

    AssertTrue Result.IsSuccess, "Success result should report success."
    AssertFalse Result.IsFailure, "Success result should not report failure."
    AssertEquals "Completed.", Result.Message, "Success message should match."
End Sub

Private Sub VerifyFailureResult()
    Dim ErrorInfo As ComErrorInfo
    Dim Result As ComResult

    Set ErrorInfo = ComCreateErrorInfo(ComTestFailureNumber, "ComCommonPhase1Tests", "Failed.")
    Set Result = ComCreateFailure(ErrorInfo)

    AssertFalse Result.IsSuccess, "Failure result should not report success."
    AssertTrue Result.IsFailure, "Failure result should report failure."
    AssertEquals "Failed.", Result.Message, "Failure message should match."
    AssertEquals ErrorInfo.Description, Result.Failure.Description, "Failure details should match."
End Sub

Private Sub VerifyTextValidation()
    AssertTrue ComIsBlankText("   "), "Spaces should be blank."
    AssertFalse ComIsBlankText("value"), "Non-empty text should not be blank."
    AssertEquals "value", ComTrimText(" value "), "Trimmed text should match."
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise ComTestAssertTrueErrorNumber, "ComCommonPhase1Tests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise ComTestAssertFalseErrorNumber, "ComCommonPhase1Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise ComTestAssertEqualsErrorNumber, "ComCommonPhase1Tests", Message
    End If
End Sub
