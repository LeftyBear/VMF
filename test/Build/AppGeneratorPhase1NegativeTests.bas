Option Explicit
Attribute VB_Name = "AppGeneratorPhase1NegativeTests"

'=========================================================================
' Module: AppGeneratorPhase1NegativeTests
' Layer: Application
' Responsibility: Negative tests for Module generator Phase 1
' Dependencies: Common, Build
'=========================================================================

Public Sub AppRunGeneratorPhase1NegativeTests()
    VerifyDuplicateModuleIsRejected
End Sub

Private Sub VerifyDuplicateModuleIsRejected()
    Dim ModuleName As String
    Dim Result As ComResult
    Dim VBProj As Object
    Dim Comp As Object

    ModuleName = "VMF_TestModule_Duplicate"

    ' Ensure a pre-existing module with the same name
    On Error Resume Next
    Set VBProj = ThisWorkbook.VBProject
    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            VBProj.VBComponents.Remove Comp
            Exit For
        End If
    Next Comp
    On Error GoTo 0

    ' Create a module with the target name to simulate duplicate
    Set Comp = VBProj.VBComponents.Add(1) ' std module
    Comp.Name = ModuleName

    ' Attempt generation - should fail
    Set Result = AppGenerateModule(ModuleName)
    If Result.IsSuccess Then
        ' Cleanup then fail test
        On Error Resume Next
        VBProj.VBComponents.Remove VBProj.VBComponents(ModuleName)
        On Error GoTo 0
        Err.Raise vbObjectError + 9300, "AppGeneratorPhase1NegativeTests", "AppGenerateModule should have failed due to duplicate module but returned success."
    End If

    ' Cleanup
    On Error Resume Next
    VBProj.VBComponents.Remove VBProj.VBComponents(ModuleName)
    On Error GoTo 0
End Sub
