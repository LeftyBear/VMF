Option Explicit
Attribute VB_Name = "PreGenerateModulePhase6Tests"

'=========================================================================
' Module: PreGenerateModulePhase6Tests
' Layer: Presentation
' Responsibility: Verification tests for module generation UI flow.
' Dependencies: Common, Application, Presentation
'=========================================================================

Private Const PreGenerateTestModuleName As String = "VMF_TestModule_UI_Phase6"
Private Const PreGenerateSuccessMessage As String = "Build completed successfully."
Private Const PreGenerateErrorNumber As Long = vbObjectError + 9500

Public Sub PreRunGenerateModulePhase6Tests()
    VerifyPreGenerateModuleMessage
End Sub

Private Sub VerifyPreGenerateModuleMessage()
    Dim Result As ComResult
    Dim Expected As String
    Dim VBProj As Object
    Dim Comp As Object

    ' Clean existing test module if present
    On Error Resume Next
    Set VBProj = ThisWorkbook.VBProject
    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, PreGenerateTestModuleName, vbTextCompare) = 0 Then
            VBProj.VBComponents.Remove Comp
            Exit For
        End If
    Next Comp
    On Error GoTo 0

    ' Simulate input dialog result by replacing PreRequestModuleName at runtime.
    ' Since InputBox cannot be mocked easily in this environment, directly
    ' call AppGenerateModule and verify the presentation message here.
    Set Result = AppGenerateModule(PreGenerateTestModuleName)
    If Result.IsFailure Then
        Err.Raise PreGenerateErrorNumber, "PreGenerateModulePhase6Tests", "Module generation failed: " & Result.Message
    End If

    Expected = PreGenerateSuccessMessage
    If PreShowBuildResult(Result) <> Expected Then
        Err.Raise PreGenerateErrorNumber + 1, "PreGenerateModulePhase6Tests", "Unexpected presentation message."
    End If

    ' Cleanup
    On Error Resume Next
    VBProj.VBComponents.Remove VBProj.VBComponents(PreGenerateTestModuleName)
    On Error GoTo 0
End Sub
