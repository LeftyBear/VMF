Option Explicit
Attribute VB_Name = "AppOutputWriteBoundaryTests"

'=========================================================================
' Module: AppOutputWriteBoundaryTests
' Layer: Application
' Responsibility: Focused tests for post-Generator output-write boundary.
' Dependencies: Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9360
Private Const ComponentTypeStandardModule As Long = 1
Private Const ComponentTypeClassModule As Long = 2

Public Sub AppRunOutputWriteBoundaryTests()
    VerifyOutputWriteAcceptsSuccessfulGeneratorOutput
    VerifyFailedGeneratorOutputHardStopsBeforeWrite
    VerifyMissingGeneratedSourceHardStopsBeforeWrite
    VerifyFallbackTemplateSelectionHardStopsBeforeWrite
    VerifyImplicitTemplateSelectionHardStopsBeforeWrite
    VerifyHardStopProducesNoWriteUnits
    VerifyApprovedPlanWritesGeneratedOutputToLocalFolder
    VerifyFailedPlanDoesNotWriteGeneratedOutput
    VerifyExistingOutputFileHardStopsBeforeWrite
    VerifyApprovedPlanAppliesToLocalTarget
    VerifyExistingLocalTargetModuleHardStopsWithoutMutation
    VerifyPathBearingMutationFileNameHardStopsWithoutMutation
    VerifyApprovedPlanAppliesToRealVBProjectFixture
    VerifyAuthorizedWorkbookLifecycleHandsOffToRealVBProject
    VerifyWorkbookLifecycleIdentityMismatchHardStopsBeforeMutation
    VerifyMissingWorkbookLifecycleAuthorizationHardStopsBeforeMutation
    VerifyUnauthorizedWorkbookLifecycleSaveHardStopsBeforeMutation
    VerifyNonAlphabeticRealVBProjectPlanAppliesDeterministically
    VerifyDuplicateRealVBProjectPlanHardStopsBeforeMutation
    VerifyUnsupportedRealVBProjectModuleKindHardStopsBeforeMutation
    VerifyMissingRealVBProjectGeneratedSourceHardStopsBeforeMutation
    VerifyBlankRealVBProjectGeneratedSourceHardStopsBeforeMutation
    VerifyRealVBProjectComponentAccessFailureHardStopsBeforeMutation
    VerifyRealVBProjectCreationFailureAfterFirstCreateRollsBack
    VerifyRealVBProjectRollbackRemovalFailureRequiresOperatorReview
    VerifyRealVBProjectReadbackMissingComponentRollsBack
    VerifyRealVBProjectReadbackMismatchedSourceRollsBack
    VerifyLaterExistingRealVBProjectModuleHardStopsBeforeMutation
    VerifyUnrelatedExistingRealVBProjectModuleIsPreserved
    VerifyExistingRealVBProjectModuleHardStopsWithoutMutation
End Sub

Private Sub VerifyOutputWriteAcceptsSuccessfulGeneratorOutput()
    Dim Result As Object
    Dim WriteUnits As Collection
    Dim Unit As Object

    Set Result = BuildPlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Result("WriteUnits")
    Set Unit = WriteUnits.Item(1)

    AssertTrue CBool(Result("Success")), "Output write plan should accept complete successful Generator output."
    AssertEquals "Success", CStr(Result("Classification")), "Output write classification should be Success."
    AssertEquals 2, WriteUnits.Count, "Output write plan should create one write unit per generated unit."
    AssertEquals "GeneratedSubject", CStr(Unit("moduleName")), "moduleName should be carried."
    AssertEquals "ClassModule", CStr(Unit("moduleType")), "moduleType should be carried."
    AssertEquals "DomainClassTemplate", CStr(Unit("templateKey")), "templateKey should be carried."
    AssertEquals "GeneratedSubject.cls", CStr(Unit("fileName")), "Class module file name should be planned."
    AssertContains CStr(Unit("generatedSource")), "Option Explicit", "generatedSource should be carried."
    AssertEquals "Planned", CStr(Unit("writeStatus")), "Focused implementation should plan, not write."
End Sub

Private Sub VerifyFailedGeneratorOutputHardStopsBeforeWrite()
    Dim Output As Object
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Output("Success") = False

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Failed Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "successful", "Hard-stop should identify failed Generator output."
    AssertEquals 0, Result("WriteUnits").Count, "Failed Generator output should produce no write units."
End Sub

Private Sub VerifyMissingGeneratedSourceHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1).Remove "generatedSource"

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Missing generatedSource should hard-stop."
    AssertContains CStr(Result("Message")), "generatedSource", "Hard-stop should identify missing generated source."
    AssertEquals 0, Result("WriteUnits").Count, "Partial Generator output should produce no write units."
End Sub

Private Sub VerifyFallbackTemplateSelectionHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1)("isFallbackDerived") = True

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Fallback-derived Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "Fallback", "Hard-stop should reject fallback Template selection."
    AssertEquals 0, Result("WriteUnits").Count, "Fallback output should produce no write units."
End Sub

Private Sub VerifyImplicitTemplateSelectionHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1)("isImplicitlySelected") = True

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Implicitly selected Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "Implicit", "Hard-stop should reject implicit Template selection."
    AssertEquals 0, Result("WriteUnits").Count, "Implicit output should produce no write units."
End Sub

Private Sub VerifyHardStopProducesNoWriteUnits()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1).Remove "moduleName"

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Hard-stop should fail."
    AssertEquals 0, Result("WriteUnits").Count, "Hard-stop should produce no output write units."
    AssertContains CStr(Result("Message")), "Output write hard-stop", "Hard-stop should remain at output-write boundary."
End Sub

Private Sub VerifyApprovedPlanWritesGeneratedOutputToLocalFolder()
    Dim Service As AppOutputWriteService
    Dim FileSystem As Object
    Dim FolderPath As String
    Dim Plan As Object
    Dim Result As Object
    Dim FirstFilePath As String

    Set Service = New AppOutputWriteService
    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    FolderPath = CreateTempOutputFolderPath(FileSystem)

    On Error GoTo Cleanup
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppWriteGeneratedOutput(Plan, FolderPath)
    FirstFilePath = FileSystem.BuildPath(FolderPath, "GeneratedSubject.cls")

    AssertTrue CBool(Result("Success")), "Approved write plan should write generated output."
    AssertEquals "Success", CStr(Result("Classification")), "Successful write classification should be Success."
    AssertEquals 2, Result("WrittenFiles").Count, "One file should be written for each write unit."
    AssertTrue FileSystem.FileExists(FirstFilePath), "Generated class file should be written."
    AssertContains ReadTextFile(FileSystem, FirstFilePath), "Option Explicit", "Written file should contain generated source."

Cleanup:
    DeleteFolderIfExists FileSystem, FolderPath
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyFailedPlanDoesNotWriteGeneratedOutput()
    Dim Service As AppOutputWriteService
    Dim FileSystem As Object
    Dim FolderPath As String
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    FolderPath = CreateTempOutputFolderPath(FileSystem)

    On Error GoTo Cleanup
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Plan("Success") = False
    Set Result = Service.AppWriteGeneratedOutput(Plan, FolderPath)

    AssertFalse CBool(Result("Success")), "Failed write plan should hard-stop."
    AssertContains CStr(Result("Message")), "successful", "Hard-stop should identify failed write plan."
    AssertFalse FileSystem.FolderExists(FolderPath), "Failed write plan should not create output folder."
    AssertEquals 0, Result("WrittenFiles").Count, "Failed write plan should report no written files."

Cleanup:
    DeleteFolderIfExists FileSystem, FolderPath
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyExistingOutputFileHardStopsBeforeWrite()
    Dim Service As AppOutputWriteService
    Dim FileSystem As Object
    Dim FolderPath As String
    Dim ExistingFilePath As String
    Dim TextFile As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    FolderPath = CreateTempOutputFolderPath(FileSystem)

    On Error GoTo Cleanup
    FileSystem.CreateFolder FolderPath
    ExistingFilePath = FileSystem.BuildPath(FolderPath, "GeneratedSubject.cls")
    Set TextFile = FileSystem.CreateTextFile(ExistingFilePath, True, False)
    TextFile.Write "existing"
    TextFile.Close

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppWriteGeneratedOutput(Plan, FolderPath)

    AssertFalse CBool(Result("Success")), "Existing output file should hard-stop before write."
    AssertContains CStr(Result("Message")), "already exists", "Hard-stop should identify existing output file."
    AssertEquals "existing", ReadTextFile(FileSystem, ExistingFilePath), "Existing output file should remain unchanged."
    AssertFalse FileSystem.FileExists(FileSystem.BuildPath(FolderPath, "GeneratedSchedule.bas")), "Preflight failure should not write later files."

Cleanup:
    DeleteFolderIfExists FileSystem, FolderPath
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyApprovedPlanAppliesToLocalTarget()
    Dim Service As AppOutputWriteService
    Dim TargetProject As Object
    Dim Modules As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set TargetProject = CreateLocalTargetProject()
    Set Modules = TargetProject("Modules")
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())

    Set Result = Service.AppApplyGeneratedOutputToLocalTarget(Plan, TargetProject)

    AssertTrue CBool(Result("Success")), "Approved write plan should apply to local target."
    AssertEquals "Success", CStr(Result("Classification")), "Local target mutation classification should be Success."
    AssertEquals 2, CLng(Result("MutatedModules")), "One local target module should be mutated for each write unit."
    AssertTrue Modules.Exists("GeneratedSubject"), "GeneratedSubject should be created in the local target."
    AssertContains CStr(Modules("GeneratedSubject")), "Option Explicit", "Generated source should be carried unchanged."
End Sub

Private Sub VerifyExistingLocalTargetModuleHardStopsWithoutMutation()
    Dim Service As AppOutputWriteService
    Dim TargetProject As Object
    Dim Modules As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set TargetProject = CreateLocalTargetProject()
    Set Modules = TargetProject("Modules")
    Modules("GeneratedSubject") = "existing"
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())

    Set Result = Service.AppApplyGeneratedOutputToLocalTarget(Plan, TargetProject)

    AssertFalse CBool(Result("Success")), "Existing local target module should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "Existing module conflict", "Hard-stop should identify existing module conflict."
    AssertEquals "existing", CStr(Modules("GeneratedSubject")), "Existing local target module should remain unchanged."
    AssertFalse Modules.Exists("GeneratedSchedule"), "Preflight failure should not mutate later modules."
End Sub

Private Sub VerifyPathBearingMutationFileNameHardStopsWithoutMutation()
    Dim Service As AppOutputWriteService
    Dim TargetProject As Object
    Dim Modules As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set TargetProject = CreateLocalTargetProject()
    Set Modules = TargetProject("Modules")
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("fileName") = "unsafe\GeneratedSubject.cls"

    Set Result = Service.AppApplyGeneratedOutputToLocalTarget(Plan, TargetProject)

    AssertFalse CBool(Result("Success")), "Path-bearing fileName should hard-stop before local target mutation."
    AssertContains CStr(Result("Message")), "fileName", "Hard-stop should identify unsafe fileName."
    AssertEquals 0, Modules.Count, "Unsafe fileName should not mutate the local target."
End Sub

Private Sub VerifyApprovedPlanAppliesToRealVBProjectFixture()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertTrue CBool(Result("Success")), "Approved write plan should apply to a real test-owned VBProject."
    AssertEquals "Success", CStr(Result("Classification")), "Real VBProject mutation classification should be Success."
    AssertEquals 2, CLng(Result("MutatedModules")), "One real VBProject module should be mutated for each write unit."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "GeneratedSubject should be created in the real fixture."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "GeneratedSchedule should be created in the real fixture."
    AssertEquals ComponentTypeClassModule, RealVBProjectModuleType(TargetVBProject, "GeneratedSubject"), "Class module kind should be preserved."
    AssertEquals ComponentTypeStandardModule, RealVBProjectModuleType(TargetVBProject, "GeneratedSchedule"), "Standard module kind should be preserved."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSubject"), "Option Explicit", "Generated source should be readable from the real fixture."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSchedule"), "Option Explicit", "Generated standard module source should be readable from the real fixture."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyAuthorizedWorkbookLifecycleHandsOffToRealVBProject()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim Plan As Object
    Dim Authorization As Object
    Dim Result As Object
    Dim Evidence As Object
    Dim TargetVBProject As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Authorization = CreateWorkbookLifecycleAuthorization(WorkbookFixture)
    Set Result = Service.AppApplyGeneratedOutputToAuthorizedWorkbook(Plan, WorkbookFixture, Authorization)
    Set Evidence = Result("WorkbookLifecycleEvidence")

    AssertTrue CBool(Result("Success")), "Authorized workbook lifecycle should hand off to real VBProject mutation."
    AssertEquals "Success", CStr(Result("Classification")), "Authorized workbook lifecycle classification should be Success."
    AssertTrue CBool(Evidence("WorkbookIdentityConfirmed")), "Lifecycle evidence should confirm exact workbook identity."
    AssertEquals "TestOwned", CStr(Evidence("WorkbookOwnership")), "Lifecycle evidence should report test-owned fixture ownership."
    AssertTrue CBool(Evidence("WorkbookNewlyCreated")), "Lifecycle evidence should report newly-created fixture state."
    AssertTrue CBool(Evidence("WorkbookEditable")), "Lifecycle evidence should report editable fixture state."
    AssertTrue CBool(Evidence("WorkbookVBProjectAccessible")), "Lifecycle evidence should report VBProject access posture."
    AssertEquals 2, Evidence("OperationHistory").Count, "Lifecycle operation history should record identity and VBProject handoff."
    AssertEquals "ConfirmWorkbookIdentity", CStr(Evidence("OperationHistory").Item(1)), "Lifecycle should confirm identity before VBProject handoff."
    AssertEquals "ObtainVBProject", CStr(Evidence("OperationHistory").Item(2)), "Lifecycle should record VBProject handoff."
    AssertEquals 1, Evidence("RemainingAuthorizedLifecycleOperations").Count, "Lifecycle evidence should retain only no-save close authorization."
    AssertEquals "CloseNoSave", CStr(Evidence("RemainingAuthorizedLifecycleOperations").Item(1)), "Lifecycle evidence should authorize only fixture no-save close."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Authorized lifecycle handoff should create requested class module."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Authorized lifecycle handoff should create requested standard module."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyWorkbookLifecycleIdentityMismatchHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim OtherWorkbookFixture As Object
    Dim Plan As Object
    Dim Authorization As Object
    Dim Result As Object
    Dim TargetVBProject As Object
    Dim Evidence As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add
    Set OtherWorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Authorization = CreateWorkbookLifecycleAuthorization(OtherWorkbookFixture)
    Set Result = Service.AppApplyGeneratedOutputToAuthorizedWorkbook(Plan, WorkbookFixture, Authorization)
    Set Evidence = Result("WorkbookLifecycleEvidence")

    AssertFalse CBool(Result("Success")), "Mismatched workbook identity should hard-stop."
    AssertEquals "HardStop", CStr(Result("Classification")), "Mismatched workbook identity should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Workbook identity mismatch", "Hard-stop should identify workbook identity mismatch."
    AssertFalse CBool(Evidence("WorkbookIdentityConfirmed")), "Failure evidence should not confirm mismatched workbook identity."
    AssertEquals 0, Evidence("OperationHistory").Count, "Identity mismatch should stop before lifecycle operations."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Identity mismatch should not mutate the fixture VBProject."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Identity mismatch should not mutate later modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    CloseWorkbookFixture OtherWorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyMissingWorkbookLifecycleAuthorizationHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim Plan As Object
    Dim Result As Object
    Dim TargetVBProject As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToAuthorizedWorkbook(Plan, WorkbookFixture, Nothing)

    AssertFalse CBool(Result("Success")), "Missing lifecycle authorization should hard-stop."
    AssertEquals "HardStop", CStr(Result("Classification")), "Missing lifecycle authorization should remain a hard-stop."
    AssertContains CStr(Result("Message")), "authorization is required", "Hard-stop should identify missing lifecycle authorization."
    AssertEquals 0, Result("WorkbookLifecycleEvidence")("OperationHistory").Count, "Missing authorization should stop before lifecycle operations."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Missing authorization should not mutate the fixture VBProject."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Missing authorization should not mutate later modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyUnauthorizedWorkbookLifecycleSaveHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim Plan As Object
    Dim Authorization As Object
    Dim Result As Object
    Dim TargetVBProject As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Authorization = CreateWorkbookLifecycleAuthorization(WorkbookFixture)
    Authorization("AllowSave") = True
    Set Result = Service.AppApplyGeneratedOutputToAuthorizedWorkbook(Plan, WorkbookFixture, Authorization)

    AssertFalse CBool(Result("Success")), "Unauthorized Save should hard-stop."
    AssertEquals "HardStop", CStr(Result("Classification")), "Unauthorized Save should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Save is not authorized", "Hard-stop should identify unauthorized Save."
    AssertEquals 0, Result("WorkbookLifecycleEvidence")("OperationHistory").Count, "Unauthorized Save should stop before lifecycle operations."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Unauthorized Save should not mutate the fixture VBProject."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Unauthorized Save should not mutate later modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyNonAlphabeticRealVBProjectPlanAppliesDeterministically()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateNonAlphabeticGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertTrue CBool(Result("Success")), "Non-alphabetic write units should apply successfully."
    AssertEquals 2, CLng(Result("MutatedModules")), "Non-alphabetic write units should all be counted."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "First non-alphabetic module should be created."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Second non-alphabetic module should be created."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSchedule"), "' Module: GeneratedSchedule", "GeneratedSchedule source should match readback."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSubject"), "' Module: GeneratedSubject", "GeneratedSubject source should match readback."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyDuplicateRealVBProjectPlanHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateDuplicateGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Duplicate real VBProject write units should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "Duplicate module mutation", "Hard-stop should identify duplicate module mutation."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Duplicate preflight should not create the duplicated module."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Duplicate preflight should not create unrelated requested modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyUnsupportedRealVBProjectModuleKindHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("moduleType") = "DocumentModule"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Unsupported real VBProject module kind should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "ModuleType is not supported", "Hard-stop should identify unsupported module kind."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Unsupported kind preflight should not create the invalid module."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Unsupported kind preflight should not create later requested modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyMissingRealVBProjectGeneratedSourceHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1).Remove "generatedSource"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Missing real VBProject generatedSource should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "generatedSource", "Hard-stop should identify missing generatedSource."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Missing source preflight should not create the invalid module."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Missing source preflight should not create later requested modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyBlankRealVBProjectGeneratedSourceHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("generatedSource") = " "
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Blank real VBProject generatedSource should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "generatedSource", "Hard-stop should identify blank generatedSource."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Blank source preflight should not create the invalid module."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Blank source preflight should not create later requested modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyRealVBProjectComponentAccessFailureHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim TargetVBProject As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set TargetVBProject = CreateObject("Scripting.Dictionary")
    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())

    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "VBComponents access failure should hard-stop before mutation."
    AssertEquals "HardStop", CStr(Result("Classification")), "VBComponents access failure should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Real VBProject mutation hard-stop", "Hard-stop should remain at the real VBProject mutation boundary."
    AssertEquals 0, CLng(Result("MutatedModules")), "Preflight component access failure should create no modules."
End Sub

Private Sub VerifyRealVBProjectCreationFailureAfterFirstCreateRollsBack()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "ExistingUtility"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing utility"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(2)("controlledCreationFault") = "AddComponentFailure"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Later component creation failure should fail after mutation starts."
    AssertEquals "HardStop", CStr(Result("Classification")), "Creation failure should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Controlled component creation failure", "Hard-stop should identify controlled creation failure."
    AssertEquals 0, CLng(Result("MutatedModules")), "Creation failure should not report partial mutation."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Rollback should remove the first current-operation component."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Failed later component should not remain created."
    AssertContains RealVBProjectModuleText(TargetVBProject, "ExistingUtility"), "existing utility", "Rollback should preserve unrelated pre-existing components."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyRealVBProjectRollbackRemovalFailureRequiresOperatorReview()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "ExistingUtility"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing utility"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("controlledRollbackRemovalFault") = "RemoveComponentFailure"
    WriteUnits.Item(2)("controlledCreationFault") = "AddComponentFailure"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Incomplete rollback should fail."
    AssertEquals "HardStop", CStr(Result("Classification")), "Incomplete rollback should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Controlled component creation failure", "Hard-stop should preserve the rollback trigger evidence."
    AssertContains CStr(Result("Message")), "Incomplete rollback evidence", "Hard-stop should report incomplete rollback evidence."
    AssertContains CStr(Result("Message")), "operator-review-required", "Incomplete rollback should require operator review."
    AssertContains CStr(Result("Message")), "Controlled rollback removal failure", "Removal error should not be swallowed."
    AssertEquals 0, CLng(Result("MutatedModules")), "Incomplete rollback should not report partial mutation."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Failed rollback removal should leave the current-operation component as evidence."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Failed later component should not remain created."
    AssertContains RealVBProjectModuleText(TargetVBProject, "ExistingUtility"), "existing utility", "Rollback failure should preserve unrelated pre-existing components."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyRealVBProjectReadbackMissingComponentRollsBack()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "ExistingUtility"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing utility"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("controlledReadbackFault") = "MissingComponent"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Missing created component during readback should fail."
    AssertEquals "HardStop", CStr(Result("Classification")), "Readback failure should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Real VBProject mutation hard-stop", "Readback failure should remain at the mutation boundary."
    AssertEquals 0, CLng(Result("MutatedModules")), "Readback failure should not report partial mutation."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Missing readback fault target should not remain created."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Rollback should remove other current-operation components."
    AssertContains RealVBProjectModuleText(TargetVBProject, "ExistingUtility"), "existing utility", "Rollback should preserve unrelated pre-existing components."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyRealVBProjectReadbackMismatchedSourceRollsBack()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "ExistingUtility"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing utility"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Plan("WriteUnits")
    WriteUnits.Item(1)("controlledReadbackFault") = "MismatchedSource"
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Mismatched created component readback should fail."
    AssertEquals "HardStop", CStr(Result("Classification")), "Readback mismatch should remain a hard-stop."
    AssertContains CStr(Result("Message")), "Readback verification failed", "Hard-stop should identify readback verification failure."
    AssertEquals 0, CLng(Result("MutatedModules")), "Readback mismatch should not report partial mutation."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Rollback should remove mismatched current-operation component."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Rollback should remove other current-operation components."
    AssertContains RealVBProjectModuleText(TargetVBProject, "ExistingUtility"), "existing utility", "Rollback should preserve unrelated pre-existing components."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyLaterExistingRealVBProjectModuleHardStopsBeforeMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "GeneratedSchedule"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing schedule"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Later existing real VBProject module should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "Existing module conflict", "Hard-stop should identify later existing real module conflict."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Earlier missing module should not be created before complete preflight."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSchedule"), "existing schedule", "Existing later real fixture module should remain unchanged."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyUnrelatedExistingRealVBProjectModuleIsPreserved()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "ExistingUtility"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing utility"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertTrue CBool(Result("Success")), "Unrelated existing real VBProject modules should not block create-only missing modules."
    AssertEquals 2, CLng(Result("MutatedModules")), "Only requested missing modules should be counted as mutations."
    AssertContains RealVBProjectModuleText(TargetVBProject, "ExistingUtility"), "existing utility", "Unrelated existing module should remain unchanged."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSubject"), "Requested missing class module should be created."
    AssertTrue RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Requested missing standard module should be created."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Sub VerifyExistingRealVBProjectModuleHardStopsWithoutMutation()
    Dim Service As AppOutputWriteService
    Dim WorkbookFixture As Object
    Dim TargetVBProject As Object
    Dim ExistingComponent As Object
    Dim Plan As Object
    Dim Result As Object

    Set Service = New AppOutputWriteService
    Set WorkbookFixture = Application.Workbooks.Add

    On Error GoTo Cleanup
    Set TargetVBProject = WorkbookFixture.VBProject
    Set ExistingComponent = TargetVBProject.VBComponents.Add(ComponentTypeStandardModule)
    ExistingComponent.Name = "GeneratedSubject"
    ExistingComponent.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' existing"

    Set Plan = Service.AppBuildOutputWritePlan(CreateSuccessfulGeneratorOutput())
    Set Result = Service.AppApplyGeneratedOutputToRealVBProject(Plan, TargetVBProject)

    AssertFalse CBool(Result("Success")), "Existing real VBProject module should hard-stop before mutation."
    AssertContains CStr(Result("Message")), "Existing module conflict", "Hard-stop should identify existing real module conflict."
    AssertContains RealVBProjectModuleText(TargetVBProject, "GeneratedSubject"), "existing", "Existing real fixture module should remain unchanged."
    AssertFalse RealVBProjectModuleExists(TargetVBProject, "GeneratedSchedule"), "Preflight failure should not mutate later real fixture modules."

Cleanup:
    CloseWorkbookFixture WorkbookFixture
    If Err.Number <> 0 Then Err.Raise Err.Number, Err.Source, Err.Description
End Sub

Private Function BuildPlan(ByVal GeneratorOutput As Object) As Object
    Dim Service As AppOutputWriteService

    Set Service = New AppOutputWriteService
    Set BuildPlan = Service.AppBuildOutputWritePlan(GeneratorOutput)
End Function

Private Function CreateSuccessfulGeneratorOutput() As Object
    Dim Output As Object
    Dim Units As Collection

    Set Units = New Collection
    Units.Add CreateGeneratedUnit(1, "GeneratedSubject", "ClassModule", "DomainClassTemplate")
    Units.Add CreateGeneratedUnit(2, "GeneratedSchedule", "StandardModule", "ModuleTemplate")

    Set Output = CreateObject("Scripting.Dictionary")
    Output("Success") = True
    Output("Classification") = "Success"
    Output("Message") = "Generator output constructed."
    Output.Add "GeneratedUnits", Units

    Set CreateSuccessfulGeneratorOutput = Output
End Function

Private Function CreateNonAlphabeticGeneratorOutput() As Object
    Dim Output As Object
    Dim Units As Collection

    Set Units = New Collection
    Units.Add CreateGeneratedUnit(1, "GeneratedSchedule", "StandardModule", "ModuleTemplate")
    Units.Add CreateGeneratedUnit(2, "GeneratedSubject", "ClassModule", "DomainClassTemplate")

    Set Output = CreateObject("Scripting.Dictionary")
    Output("Success") = True
    Output("Classification") = "Success"
    Output("Message") = "Generator output constructed."
    Output.Add "GeneratedUnits", Units

    Set CreateNonAlphabeticGeneratorOutput = Output
End Function

Private Function CreateDuplicateGeneratorOutput() As Object
    Dim Output As Object
    Dim Units As Collection

    Set Units = New Collection
    Units.Add CreateGeneratedUnit(1, "GeneratedSubject", "ClassModule", "DomainClassTemplate")
    Units.Add CreateGeneratedUnit(2, "GeneratedSubject", "ClassModule", "DomainClassTemplate")

    Set Output = CreateObject("Scripting.Dictionary")
    Output("Success") = True
    Output("Classification") = "Success"
    Output("Message") = "Generator output constructed."
    Output.Add "GeneratedUnits", Units

    Set CreateDuplicateGeneratorOutput = Output
End Function

Private Function CreateWorkbookLifecycleAuthorization(ByVal WorkbookFixture As Object) As Object
    Dim Authorization As Object

    Set Authorization = CreateObject("Scripting.Dictionary")
    Authorization.Add "Workbook", WorkbookFixture
    Authorization("IsTestOwned") = True
    Authorization("IsNewlyCreated") = True
    Authorization("AllowObtainVBProject") = True
    Authorization("AllowCloseNoSave") = True
    Authorization("AllowSave") = False
    Authorization("AllowSaveAs") = False

    Set CreateWorkbookLifecycleAuthorization = Authorization
End Function

Private Function CreateGeneratedUnit( _
    ByVal UnitOrder As Long, _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal TemplateKey As String _
) As Object

    Dim Unit As Object

    Set Unit = CreateObject("Scripting.Dictionary")
    Unit("order") = UnitOrder
    Unit("moduleName") = ModuleName
    Unit("moduleType") = ModuleType
    Unit("templateKey") = TemplateKey
    Unit("generatedSource") = "Option Explicit" & vbCrLf & "' Module: " & ModuleName
    Unit("isFallbackDerived") = False
    Unit("isImplicitlySelected") = False

    Set CreateGeneratedUnit = Unit
End Function

Private Function CreateLocalTargetProject() As Object
    Dim TargetProject As Object
    Dim Modules As Object

    Set TargetProject = CreateObject("Scripting.Dictionary")
    Set Modules = CreateObject("Scripting.Dictionary")
    TargetProject.Add "Modules", Modules

    Set CreateLocalTargetProject = TargetProject
End Function

Private Function CreateTempOutputFolderPath(ByVal FileSystem As Object) As String
    CreateTempOutputFolderPath = FileSystem.BuildPath(Environ$("TEMP"), "vmf-p6-07-" & Replace(CStr(Timer), ".", ""))
End Function

Private Function RealVBProjectModuleExists(ByVal TargetVBProject As Object, ByVal ModuleName As String) As Boolean
    Dim Component As Object

    For Each Component In TargetVBProject.VBComponents
        If StrComp(CStr(Component.Name), ModuleName, vbBinaryCompare) = 0 Then
            RealVBProjectModuleExists = True
            Exit Function
        End If
    Next Component

    RealVBProjectModuleExists = False
End Function

Private Function RealVBProjectModuleText(ByVal TargetVBProject As Object, ByVal ModuleName As String) As String
    Dim Component As Object

    Set Component = TargetVBProject.VBComponents.Item(ModuleName)
    RealVBProjectModuleText = Component.CodeModule.Lines(1, Component.CodeModule.CountOfLines)
End Function

Private Function RealVBProjectModuleType(ByVal TargetVBProject As Object, ByVal ModuleName As String) As Long
    Dim Component As Object

    Set Component = TargetVBProject.VBComponents.Item(ModuleName)
    RealVBProjectModuleType = CLng(Component.Type)
End Function

Private Sub CloseWorkbookFixture(ByVal WorkbookFixture As Object)
    On Error Resume Next
    If Not WorkbookFixture Is Nothing Then
        WorkbookFixture.Close False
    End If
    On Error GoTo 0
End Sub

Private Function ReadTextFile(ByVal FileSystem As Object, ByVal FilePath As String) As String
    Dim TextFile As Object

    Set TextFile = FileSystem.OpenTextFile(FilePath, 1, False)
    ReadTextFile = TextFile.ReadAll
    TextFile.Close
End Function

Private Sub DeleteFolderIfExists(ByVal FileSystem As Object, ByVal FolderPath As String)
    If Len(FolderPath) = 0 Then
        Exit Sub
    End If

    If FileSystem.FolderExists(FolderPath) Then
        FileSystem.DeleteFolder FolderPath, True
    End If
End Sub

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message & " Expected=" & CStr(Expected) & " Actual=" & CStr(Actual)
    End If
End Sub
