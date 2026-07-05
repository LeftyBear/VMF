Option Explicit
Attribute VB_Name = "PreGenerateModulePhase6Tests"

'=========================================================================
' Module: PreGenerateModulePhase6Tests
' Layer: Presentation
' Responsibility: Verification tests for module generation UI flow.
' Dependencies: Common, Application, Infrastructure, Presentation
'=========================================================================

Private Const PreGenerateTestModuleName As String = "VMF_TestModule_UI_Phase6"
Private Const PreGenerateSuccessMessage As String = "Module generated successfully."
Private Const PreGenerateErrorNumber As Long = vbObjectError + 9500

Public Sub PreRunGenerateModulePhase6Tests()
    VerifyPreGenerateModuleMessage
End Sub

Private Sub VerifyPreGenerateModuleMessage()
    Dim Result As ComResult
    Dim Expected As String
    Dim ProjectProvider As InfVbaProjectProvider

    Set ProjectProvider = InfCreateVbaProjectProvider()
    ProjectProvider.InfRemoveModule PreGenerateTestModuleName

    Set Result = AppGenerateModule(PreGenerateTestModuleName)
    If Result.IsFailure Then
        Err.Raise PreGenerateErrorNumber, "PreGenerateModulePhase6Tests", "Module generation failed: " & Result.Message
    End If

    Expected = PreGenerateSuccessMessage
    If PreShowGenerateModuleResult(Result) <> Expected Then
        Err.Raise PreGenerateErrorNumber + 1, "PreGenerateModulePhase6Tests", "Unexpected presentation message."
    End If

    ProjectProvider.InfRemoveModule PreGenerateTestModuleName
End Sub
