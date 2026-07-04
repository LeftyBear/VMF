Option Explicit
Attribute VB_Name = "DomVmfCorePhase3Tests"

'=========================================================================
' Module: DomVmfCorePhase3Tests
' Layer: Domain
' Responsibility: Focused verification tests for Phase 3 VMF Core contracts.
' Dependencies: Common, Domain
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const DomTestProjectName As String = "Build.xlam"
Private Const DomTestRootPath As String = "C:\VMF"
Private Const DomTestModuleName As String = "ComFacade"
Private Const DomTestLayerName As String = "Common"
Private Const DomTestFilePath As String = "src\Common\ComFacade.bas"
Private Const DomTestAssertTrueErrorNumber As Long = vbObjectError + 9200
Private Const DomTestAssertEqualsErrorNumber As Long = vbObjectError + 9201

'=========================================================================
' Public API
'=========================================================================

' Runs the Phase 3 VMF Core verification tests.
Public Sub DomRunVmfCorePhase3Tests()
    VerifyInitialize
    VerifyProjectValidation
    VerifyModuleInfoValidation
    VerifyManifestValidation
End Sub

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub VerifyInitialize()
    AssertTrue DomInitialize(), "DomInitialize should return True."
    DomShutdown
End Sub

Private Sub VerifyProjectValidation()
    Dim Project As DomProject
    Dim Result As ComResult

    Set Project = DomCreateProject(DomTestProjectName, DomTestRootPath)
    Set Result = DomValidateProject(Project)

    AssertTrue Result.IsSuccess, "Project should be valid."
    AssertEquals DomTestProjectName, Project.Name, "Project name should match."
End Sub

Private Sub VerifyModuleInfoValidation()
    Dim ModuleInfo As DomModuleInfo
    Dim Result As ComResult

    Set ModuleInfo = DomCreateModuleInfo(DomTestModuleName, DomTestLayerName, DomTestFilePath)
    Set Result = DomValidateModuleInfo(ModuleInfo)

    AssertTrue Result.IsSuccess, "ModuleInfo should be valid."
    AssertEquals DomTestModuleName, ModuleInfo.Name, "Module name should match."
End Sub

Private Sub VerifyManifestValidation()
    Dim Project As DomProject
    Dim Manifest As DomManifest
    Dim Result As ComResult

    Set Project = DomCreateProject(DomTestProjectName, DomTestRootPath)
    Set Manifest = DomCreateManifest(Project)
    Set Result = DomValidateManifest(Manifest)

    AssertTrue Result.IsSuccess, "Manifest should be valid."
    AssertEquals DomTestProjectName, Manifest.Project.Name, "Manifest project should match."
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise DomTestAssertTrueErrorNumber, "DomVmfCorePhase3Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise DomTestAssertEqualsErrorNumber, "DomVmfCorePhase3Tests", Message
    End If
End Sub
