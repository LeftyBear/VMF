Option Explicit
Attribute VB_Name = "AppBuildPhase4Tests"

'=========================================================================
' Module: AppBuildPhase4Tests
' Layer: Application
' Responsibility: Focused verification tests for Phase 4 Build contracts.
' Dependencies: Common, Domain, Infrastructure, Application
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const AppTestProjectName As String = "Build.xlam"
Private Const AppTestRootPath As String = "C:\VMF"
Private Const AppTestFolderName As String = "VMF_AppPhase4Tests"
Private Const AppTestFileName As String = "manifest.txt"
Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9300

'=========================================================================
' Public API
'=========================================================================

' Runs the Phase 4 Build verification tests.
Public Sub AppRunBuildPhase4Tests()
    VerifyInitialize
    VerifyValidateProject
    VerifyBuildProject
    VerifyImportProject
End Sub

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub VerifyInitialize()
    AssertTrue AppInitialize(), "AppInitialize should return True."
    AppShutdown
End Sub

Private Sub VerifyValidateProject()
    AssertTrue AppValidateProject(AppTestProjectName, AppTestRootPath).IsSuccess, "Project should be valid."
End Sub

Private Sub VerifyBuildProject()
    Dim Result As ComResult

    Set Result = AppBuildProject(AppTestProjectName, AppTestRootPath, GetTestFilePath())

    AssertTrue Result.IsSuccess, "Build should succeed."
    AssertTrue InfFileExists(GetTestFilePath()), "Build output file should exist."
End Sub

Private Sub VerifyImportProject()
    Dim Result As ComResult

    Set Result = AppImportProject(GetTestFilePath())

    AssertTrue Result.IsSuccess, "Import should succeed."
End Sub

Private Function GetTestFilePath() As String
    GetTestFilePath = Environ$("TEMP") & "\" & AppTestFolderName & "\" & AppTestFileName
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertTrueErrorNumber, "AppBuildPhase4Tests", Message
    End If
End Sub
