Option Explicit
Attribute VB_Name = "AppGeneratorPhase1NegativeTests"

'=========================================================================
' Module: AppGeneratorPhase1NegativeTests
' Layer: Application
' Responsibility: Negative tests for Module generator Phase 1
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Public Sub AppRunGeneratorPhase1NegativeTests()
    VerifyDuplicateModuleIsRejected
End Sub

Private Sub VerifyDuplicateModuleIsRejected()
    Dim ModuleName As String
    Dim Result As ComResult
    Dim ProjectProvider As InfVbaProjectProvider

    ModuleName = "VMF_TestModule_Duplicate"
    Set ProjectProvider = InfCreateVbaProjectProvider()

    ProjectProvider.InfRemoveModule ModuleName

    Set Result = ProjectProvider.InfAddStandardModule( _
        ModuleName, _
        "Option Explicit" & vbCrLf & vbCrLf & _
        "'=========================================================================" & vbCrLf & _
        "' Module: " & ModuleName & vbCrLf & _
        "' Layer: Application" & vbCrLf & _
        "' Responsibility:" & vbCrLf & _
        "'=========================================================================" & vbCrLf)
    If Result.IsFailure Then
        Err.Raise vbObjectError + 9301, "AppGeneratorPhase1NegativeTests", "Duplicate setup failed: " & Result.Message
    End If

    Set Result = AppGenerateModule(ModuleName)
    If Result.IsSuccess Then
        ProjectProvider.InfRemoveModule ModuleName
        Err.Raise vbObjectError + 9300, "AppGeneratorPhase1NegativeTests", "AppGenerateModule should have failed due to duplicate module but returned success."
    End If

    ProjectProvider.InfRemoveModule ModuleName
End Sub
