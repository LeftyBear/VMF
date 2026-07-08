Option Explicit
Attribute VB_Name = "PreUiPhase5Tests"

'=========================================================================
' Module: PreUiPhase5Tests
' Layer: Presentation
' Responsibility: Focused verification tests for Phase 5 UI contracts.
' Dependencies: Common, Application, Infrastructure, Presentation
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const PreExpectedValidationMessage As String = "Validation completed successfully."
Private Const PreExpectedBuildMessage As String = "Build completed successfully."
Private Const PreExpectedGenerateMessage As String = "Module generated successfully."
Private Const PreTestAssertTrueErrorNumber As Long = vbObjectError + 9400
Private Const PreTestAssertEqualsErrorNumber As Long = vbObjectError + 9401

'=========================================================================
' Public API
'=========================================================================

' Runs the Phase 5 UI verification tests.
Public Sub PreRunUiPhase5Tests()
    VerifyInitialize
    VerifyValidationMessage
    VerifyBuildMessage
    VerifyGenerateModuleMessage
    VerifyRibbonLoaded
End Sub

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub VerifyInitialize()
    AssertTrue PreInitialize(), "PreInitialize should return True."
    PreShutdown
End Sub

Private Sub VerifyValidationMessage()
    AssertEquals _
        PreExpectedValidationMessage, _
        PreShowValidationResult(ComCreateSuccess()), _
        "Validation success message should match."
End Sub

Private Sub VerifyBuildMessage()
    AssertEquals _
        PreExpectedBuildMessage, _
        PreShowBuildResult(ComCreateSuccess()), _
        "Build success message should match."
End Sub

Private Sub VerifyGenerateModuleMessage()
    Dim Result As ComResult
    Dim ProjectProvider As InfVbaProjectProvider

    Set ProjectProvider = InfCreateVbaProjectProvider()
    ProjectProvider.InfRemoveModule "VMF_TestModule_UI_Phase5"
    Set Result = AppGenerateModule("VMF_TestModule_UI_Phase5")

    AssertEquals _
        PreExpectedGenerateMessage, _
        PreShowGenerateModuleResult(Result), _
        "Generate module presentation message should match."

    ProjectProvider.InfRemoveModule "VMF_TestModule_UI_Phase5"
End Sub

Private Sub VerifyRibbonLoaded()
    AssertTrue PreRibbonLoaded().IsSuccess, "Ribbon load result should be success."
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise PreTestAssertTrueErrorNumber, "PreUiPhase5Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise PreTestAssertEqualsErrorNumber, "PreUiPhase5Tests", Message
    End If
End Sub
