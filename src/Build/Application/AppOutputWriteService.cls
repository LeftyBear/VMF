VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppOutputWriteService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppOutputWriteService
' Layer: Application
' Responsibility: Validate Generator output and build local output-write plans.
' Dependencies: Common
'=========================================================================

Private Const ResultSuccess As String = "Success"
Private Const ResultHardStop As String = "HardStop"
Private Const ComponentTypeStandardModule As Long = 1
Private Const ComponentTypeClassModule As Long = 2
Private Const P9FixtureRelativePath As String = "tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm"
Private Const P9FixtureExpectedLength As Long = 3532
Private Const P9FixtureExpectedSha256 As String = "BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E"

' Builds a deterministic local write plan from complete successful Generator output.
Public Function AppBuildOutputWritePlan(ByVal GeneratorOutput As Object) As Object
    Dim Result As Object

    On Error GoTo ErrHandler

    Set Result = CreateResult(False, ResultHardStop, "Generator output is required.")
    If GeneratorOutput Is Nothing Then
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    If Not HasBooleanValue(GeneratorOutput, "Success", True) Then
        Result("Message") = "Generator output must be successful."
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    If Not HasCollection(GeneratorOutput, "GeneratedUnits") Then
        Result("Message") = "Generator output units are required."
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    Set AppBuildOutputWritePlan = BuildSuccessfulPlan(GeneratorOutput("GeneratedUnits"))
    Exit Function

ErrHandler:
    Set AppBuildOutputWritePlan = CreateResult(False, ResultHardStop, "Output write hard-stop: " & Err.Description)
End Function

' Writes approved output-write plan units to a deterministic local folder.
Public Function AppWriteGeneratedOutput(ByVal OutputWritePlan As Object, ByVal OutputFolderPath As String) As Object
    Dim WriteUnits As Collection
    Dim FileSystem As Object
    Dim WrittenFiles As Collection

    On Error GoTo ErrHandler

    If OutputWritePlan Is Nothing Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write plan is required.")
        Exit Function
    End If

    If Not HasBooleanValue(OutputWritePlan, "Success", True) Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write plan must be successful.")
        Exit Function
    End If

    If Not HasCollection(OutputWritePlan, "WriteUnits") Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write units are required.")
        Exit Function
    End If

    If ComIsBlankText(OutputFolderPath) Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output folder path is required.")
        Exit Function
    End If

    Set WriteUnits = OutputWritePlan("WriteUnits")
    If WriteUnits.Count = 0 Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write units are absent.")
        Exit Function
    End If

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    PreflightWriteUnits WriteUnits, FileSystem, OutputFolderPath

    If Not FileSystem.FolderExists(OutputFolderPath) Then
        FileSystem.CreateFolder OutputFolderPath
    End If

    Set WrittenFiles = WriteAllUnits(WriteUnits, FileSystem, OutputFolderPath)
    Set AppWriteGeneratedOutput = CreateSuccessfulWriteResult(WrittenFiles)
    Exit Function

ErrHandler:
    Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Generated output write hard-stop: " & Err.Description)
End Function

' Applies approved write-plan units to a local test-controlled target only.
Public Function AppApplyGeneratedOutputToLocalTarget(ByVal OutputWritePlan As Object, ByVal TargetProject As Object) As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    On Error GoTo ErrHandler

    If OutputWritePlan Is Nothing Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write plan is required.")
        Exit Function
    End If

    If TargetProject Is Nothing Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Local target project is required.")
        Exit Function
    End If

    If Not HasBooleanValue(OutputWritePlan, "Success", True) Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write plan must be successful.")
        Exit Function
    End If

    If Not HasCollection(OutputWritePlan, "WriteUnits") Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write units are required.")
        Exit Function
    End If

    Set WriteUnits = OutputWritePlan("WriteUnits")
    If WriteUnits.Count = 0 Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write units are absent.")
        Exit Function
    End If

    PreflightLocalTargetMutation WriteUnits, TargetProject
    ApplyLocalTargetMutation WriteUnits, TargetProject

    Set Result = CreateMutationResult(True, ResultSuccess, "Generated output applied to local target.")
    Result("MutatedModules") = WriteUnits.Count
    Set AppApplyGeneratedOutputToLocalTarget = Result
    Exit Function

ErrHandler:
    Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Target VBA project mutation hard-stop: " & Err.Description)
End Function

' Applies approved write-plan units to an explicitly authorized real VBProject target.
Public Function AppApplyGeneratedOutputToRealVBProject(ByVal OutputWritePlan As Object, ByVal TargetVBProject As Object) As Object
    Dim WriteUnits As Collection
    Dim CreatedComponents As Collection
    Dim Result As Object
    Dim MutationError As String

    On Error GoTo ErrHandler

    If OutputWritePlan Is Nothing Then
        Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Output write plan is required.")
        Exit Function
    End If

    If TargetVBProject Is Nothing Then
        Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Target VBProject is required.")
        Exit Function
    End If

    If Not HasBooleanValue(OutputWritePlan, "Success", True) Then
        Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Output write plan must be successful.")
        Exit Function
    End If

    If Not HasCollection(OutputWritePlan, "WriteUnits") Then
        Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Output write units are required.")
        Exit Function
    End If

    Set WriteUnits = OutputWritePlan("WriteUnits")
    If WriteUnits.Count = 0 Then
        Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Output write units are absent.")
        Exit Function
    End If

    PreflightRealVBProjectMutation WriteUnits, TargetVBProject
    Set CreatedComponents = ApplyRealVBProjectMutation(WriteUnits, TargetVBProject)
    ApplyControlledReadbackFault WriteUnits, TargetVBProject
    VerifyRealVBProjectReadback WriteUnits, TargetVBProject

    Set Result = CreateMutationResult(True, ResultSuccess, "Generated output applied to real VBProject.")
    Result("MutatedModules") = CreatedComponents.Count
    Set AppApplyGeneratedOutputToRealVBProject = Result
    Exit Function

ErrHandler:
    MutationError = Err.Description
    If Not CreatedComponents Is Nothing Then
        On Error GoTo RollbackErr
        RollbackCreatedComponents TargetVBProject, CreatedComponents, WriteUnits
        On Error GoTo 0
    End If
    Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Real VBProject mutation hard-stop: " & MutationError)
    Exit Function

RollbackErr:
    Set AppApplyGeneratedOutputToRealVBProject = CreateMutationResult(False, ResultHardStop, "Real VBProject mutation hard-stop: " & MutationError & " Incomplete rollback evidence: operator-review-required: " & Err.Description)
End Function

' Applies approved write-plan units through an explicitly authorized workbook lifecycle handoff.
Public Function AppApplyGeneratedOutputToAuthorizedWorkbook(ByVal OutputWritePlan As Object, ByVal WorkbookFixture As Object, ByVal LifecycleAuthorization As Object) As Object
    Dim TargetVBProject As Object
    Dim Result As Object
    Dim Evidence As Object
    Dim OperationHistory As Collection

    On Error GoTo ErrHandler

    Set OperationHistory = New Collection
    ValidateWorkbookLifecycleAuthorization WorkbookFixture, LifecycleAuthorization, OperationHistory
    Set TargetVBProject = WorkbookFixture.VBProject
    OperationHistory.Add "ObtainVBProject"
    Set Evidence = CreateWorkbookLifecycleEvidence(WorkbookFixture, OperationHistory)

    Set Result = AppApplyGeneratedOutputToRealVBProject(OutputWritePlan, TargetVBProject)
    AttachWorkbookLifecycleEvidence Result, Evidence
    Set AppApplyGeneratedOutputToAuthorizedWorkbook = Result
    Exit Function

ErrHandler:
    Set Result = CreateMutationResult(False, ResultHardStop, "Workbook lifecycle hard-stop: " & Err.Description)
    AttachWorkbookLifecycleEvidence Result, CreateFailedWorkbookLifecycleEvidence(OperationHistory)
    Set AppApplyGeneratedOutputToAuthorizedWorkbook = Result
End Function

' Runs the P9 read-only existing-workbook lifecycle against the fixed repository fixture.
Public Function AppRunReadOnlyWorkbookLifecycle(ByVal RepositoryRootPath As String) As Object
    Dim FileSystem As Object
    Dim Evidence As Object
    Dim FixturePath As String
    Dim WorkbookFixture As Object
    Dim OpenAttempted As Boolean

    On Error GoTo ErrHandler

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    Set Evidence = CreateReadOnlyLifecycleEvidence()
    Evidence("RepositoryRoot") = ComTrimText(RepositoryRootPath)

    FixturePath = ResolveP9FixturePath(FileSystem, RepositoryRootPath)
    Evidence("ResolvedFixturePath") = FixturePath
    VerifyP9FixtureIdentity FileSystem, FixturePath, Evidence, "PreOpen"
    Evidence("OpenMode") = "ReadOnly"

    OpenAttempted = True
    Set WorkbookFixture = Application.Workbooks.Open(Filename:=FixturePath, UpdateLinks:=0, ReadOnly:=True, AddToMru:=False)
    Evidence("WorkbookIdentityConfirmed") = (StrComp(CStr(WorkbookFixture.FullName), FixturePath, vbTextCompare) = 0)
    If Not CBool(Evidence("WorkbookIdentityConfirmed")) Then
        Err.Raise ComErrInvalidState, "AppOutputWriteService.AppRunReadOnlyWorkbookLifecycle", "Workbook identity reconfirmation failed."
    End If
    If Not CBool(WorkbookFixture.ReadOnly) Then
        Err.Raise ComErrInvalidState, "AppOutputWriteService.AppRunReadOnlyWorkbookLifecycle", "Writable workbook open is not authorized."
    End If
    Evidence("WorkbookReadOnly") = True
    AddReadOnlyLifecycleOperation Evidence, "OpenReadOnly"

    WorkbookFixture.Close False
    Set WorkbookFixture = Nothing
    Evidence("ClosedWithoutSaving") = True
    AddReadOnlyLifecycleOperation Evidence, "CloseNoSave"
    VerifyP9FixtureIdentity FileSystem, FixturePath, Evidence, "PostClose"

    Set AppRunReadOnlyWorkbookLifecycle = CreateLifecycleResult(True, ResultSuccess, "Read-only workbook lifecycle completed.", Evidence)
    Exit Function

ErrHandler:
    If Not WorkbookFixture Is Nothing Then
        On Error Resume Next
        WorkbookFixture.Close False
        On Error GoTo 0
    End If
    If OpenAttempted Then
        Evidence("OpenFailure") = Err.Description
        AddReadOnlyLifecycleOperation Evidence, "OpenReadOnlyFailed"
    End If
    Set AppRunReadOnlyWorkbookLifecycle = CreateLifecycleResult(False, ResultHardStop, "Read-only workbook lifecycle hard-stop: " & Err.Description, Evidence)
End Function

Private Function BuildSuccessfulPlan(ByVal GeneratedUnits As Collection) As Object
    Dim Result As Object
    Dim WriteUnits As Collection
    Dim Unit As Variant

    If GeneratedUnits Is Nothing Then
        Set BuildSuccessfulPlan = CreateResult(False, ResultHardStop, "Generator output units are required.")
        Exit Function
    End If

    If GeneratedUnits.Count = 0 Then
        Set BuildSuccessfulPlan = CreateResult(False, ResultHardStop, "Generator output units are absent.")
        Exit Function
    End If

    Set WriteUnits = New Collection
    For Each Unit In GeneratedUnits
        WriteUnits.Add CreateWriteUnit(Unit)
    Next Unit

    Set Result = CreateResult(True, ResultSuccess, "Output write plan constructed.")
    Result.Remove "WriteUnits"
    Result.Add "WriteUnits", WriteUnits
    Set BuildSuccessfulPlan = Result
End Function

Private Function ResolveP9FixturePath(ByVal FileSystem As Object, ByVal RepositoryRootPath As String) As String
    Dim RootPath As String
    Dim FixturePath As String

    RootPath = ComTrimText(RepositoryRootPath)
    If ComIsBlankText(RootPath) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Repository root is required."
    End If

    If Not FileSystem.FolderExists(RootPath) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Repository root does not exist."
    End If

    RootPath = FileSystem.GetAbsolutePathName(RootPath)
    If StrComp(RootPath, RepositoryRootPath, vbBinaryCompare) <> 0 Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Repository root must be an explicit absolute path."
    End If

    If Not FileSystem.FileExists(FileSystem.BuildPath(RootPath, "AGENTS.md")) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Repository root is not the VMF checkout root."
    End If

    FixturePath = FileSystem.GetAbsolutePathName(FileSystem.BuildPath(RootPath, P9FixtureRelativePath))
    If InStr(1, FixturePath, RootPath & Application.PathSeparator, vbTextCompare) <> 1 Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Resolved fixture path is outside the repository root."
    End If

    If Not FileSystem.FileExists(FixturePath) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ResolveP9FixturePath", "Authorized P9 workbook fixture is missing."
    End If

    ResolveP9FixturePath = FixturePath
End Function

Private Sub VerifyP9FixtureIdentity(ByVal FileSystem As Object, ByVal FixturePath As String, ByVal Evidence As Object, ByVal PhaseName As String)
    Dim FileItem As Object
    Dim ActualSha256 As String

    Set FileItem = FileSystem.GetFile(FixturePath)
    ActualSha256 = ComputeFileSha256(FixturePath)

    Evidence(PhaseName & "Length") = CLng(FileItem.Size)
    Evidence(PhaseName & "Sha256") = ActualSha256

    If CLng(FileItem.Size) <> P9FixtureExpectedLength Then
        Err.Raise ComErrInvalidState, "AppOutputWriteService.VerifyP9FixtureIdentity", "Authorized P9 workbook fixture length mismatch."
    End If

    If StrComp(ActualSha256, P9FixtureExpectedSha256, vbTextCompare) <> 0 Then
        Err.Raise ComErrInvalidState, "AppOutputWriteService.VerifyP9FixtureIdentity", "Authorized P9 workbook fixture SHA-256 mismatch."
    End If

    Evidence(PhaseName & "IdentityConfirmed") = True
    AddReadOnlyLifecycleOperation Evidence, PhaseName & "IdentityCheck"
End Sub

Private Function ComputeFileSha256(ByVal FilePath As String) As String
    Dim Shell As Object
    Dim ExecResult As Object
    Dim OutputText As String
    Dim Lines() As String
    Dim Index As Long
    Dim Candidate As String

    Set Shell = CreateObject("WScript.Shell")
    Set ExecResult = Shell.Exec("cmd.exe /c certutil -hashfile " & QuoteShellArgument(FilePath) & " SHA256")
    OutputText = ExecResult.StdOut.ReadAll
    If Len(ExecResult.StdErr.ReadAll) > 0 Or ExecResult.ExitCode <> 0 Then
        Err.Raise ComErrInvalidState, "AppOutputWriteService.ComputeFileSha256", "Fixture SHA-256 check failed."
    End If

    Lines = Split(OutputText, vbCrLf)
    For Index = LBound(Lines) To UBound(Lines)
        Candidate = UCase$(Replace(Trim$(Lines(Index)), " ", vbNullString))
        If Len(Candidate) = 64 And IsHexText(Candidate) Then
            ComputeFileSha256 = Candidate
            Exit Function
        End If
    Next Index

    Err.Raise ComErrInvalidState, "AppOutputWriteService.ComputeFileSha256", "Fixture SHA-256 evidence is missing."
End Function

Private Function QuoteShellArgument(ByVal TextValue As String) As String
    QuoteShellArgument = Chr$(34) & Replace(TextValue, Chr$(34), Chr$(34) & Chr$(34)) & Chr$(34)
End Function

Private Function IsHexText(ByVal TextValue As String) As Boolean
    Dim Index As Long
    Dim Code As Integer

    For Index = 1 To Len(TextValue)
        Code = Asc(Mid$(TextValue, Index, 1))
        If Not ((Code >= 48 And Code <= 57) Or (Code >= 65 And Code <= 70)) Then
            IsHexText = False
            Exit Function
        End If
    Next Index

    IsHexText = True
End Function

Private Function CreateReadOnlyLifecycleEvidence() As Object
    Dim Evidence As Object

    Set Evidence = CreateObject("Scripting.Dictionary")
    Evidence("RepositoryRoot") = vbNullString
    Evidence("ResolvedFixturePath") = vbNullString
    Evidence("OpenMode") = "None"
    Evidence("WorkbookIdentityConfirmed") = False
    Evidence("WorkbookReadOnly") = False
    Evidence("ClosedWithoutSaving") = False
    Evidence("OpenFailure") = vbNullString
    Evidence("PreOpenIdentityConfirmed") = False
    Evidence("PostCloseIdentityConfirmed") = False
    Evidence.Add "OperationHistory", New Collection

    Set CreateReadOnlyLifecycleEvidence = Evidence
End Function

Private Function CreateLifecycleResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String, ByVal Evidence As Object) As Object
    Dim Result As Object

    Set Result = CreateMutationResult(IsSuccess, Classification, Message)
    Result("MutatedModules") = 0
    Result.Add "ReadOnlyLifecycleEvidence", Evidence

    Set CreateLifecycleResult = Result
End Function

Private Sub AddReadOnlyLifecycleOperation(ByVal Evidence As Object, ByVal OperationName As String)
    Dim OperationHistory As Collection

    Set OperationHistory = Evidence("OperationHistory")
    OperationHistory.Add OperationName
End Sub

Private Sub ValidateWorkbookLifecycleAuthorization(ByVal WorkbookFixture As Object, ByVal LifecycleAuthorization As Object, ByVal OperationHistory As Collection)
    Dim AuthorizedWorkbook As Object

    If WorkbookFixture Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Workbook fixture is required."
    End If

    If LifecycleAuthorization Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Workbook lifecycle authorization is required."
    End If

    Set AuthorizedWorkbook = RequiredObject(LifecycleAuthorization, "Workbook", "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization")
    If AuthorizedWorkbook Is WorkbookFixture Then
        ' Exact object identity is the only authorized lifecycle target.
    Else
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Workbook identity mismatch is not authorized."
    End If

    If Not HasBooleanValue(LifecycleAuthorization, "IsTestOwned", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Test-owned workbook authorization is required."
    End If

    If Not HasBooleanValue(LifecycleAuthorization, "IsNewlyCreated", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Newly-created workbook authorization is required."
    End If

    If Not HasBooleanValue(LifecycleAuthorization, "AllowObtainVBProject", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "VBProject handoff authorization is required."
    End If

    If Not HasBooleanValue(LifecycleAuthorization, "AllowCloseNoSave", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "No-save close authorization is required."
    End If

    If HasBooleanValue(LifecycleAuthorization, "AllowSave", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "Save is not authorized."
    End If

    If HasBooleanValue(LifecycleAuthorization, "AllowSaveAs", True) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ValidateWorkbookLifecycleAuthorization", "SaveAs is not authorized."
    End If

    OperationHistory.Add "ConfirmWorkbookIdentity"
End Sub

Private Function RequiredObject(ByVal Source As Object, ByVal FieldName As String, ByVal SourceName As String) As Object
    If Source Is Nothing Then
        Err.Raise ComErrInvalidArgument, SourceName, "Source item is required."
    End If

    If Not Source.Exists(FieldName) Then
        Err.Raise ComErrInvalidArgument, SourceName, "Required object field is missing: " & FieldName
    End If

    If Not IsObject(Source.Item(FieldName)) Then
        Err.Raise ComErrInvalidArgument, SourceName, "Required object field is invalid: " & FieldName
    End If

    Set RequiredObject = Source.Item(FieldName)
End Function

Private Function CreateWorkbookLifecycleEvidence(ByVal WorkbookFixture As Object, ByVal OperationHistory As Collection) As Object
    Dim Evidence As Object
    Dim RemainingOperations As Collection

    Set RemainingOperations = New Collection
    RemainingOperations.Add "CloseNoSave"

    Set Evidence = CreateObject("Scripting.Dictionary")
    Evidence("WorkbookIdentityConfirmed") = True
    Evidence("WorkbookOwnership") = "TestOwned"
    Evidence("WorkbookNewlyCreated") = True
    Evidence("WorkbookEditable") = True
    Evidence("WorkbookSavedState") = CStr(WorkbookFixture.Saved)
    Evidence("WorkbookVBProjectAccessible") = True
    Evidence.Add "OperationHistory", OperationHistory
    Evidence.Add "RemainingAuthorizedLifecycleOperations", RemainingOperations

    Set CreateWorkbookLifecycleEvidence = Evidence
End Function

Private Function CreateFailedWorkbookLifecycleEvidence(ByVal OperationHistory As Collection) As Object
    Dim Evidence As Object
    Dim RemainingOperations As Collection

    Set RemainingOperations = New Collection
    Set Evidence = CreateObject("Scripting.Dictionary")
    Evidence("WorkbookIdentityConfirmed") = False
    Evidence("WorkbookOwnership") = "Unknown"
    Evidence("WorkbookNewlyCreated") = False
    Evidence("WorkbookEditable") = False
    Evidence("WorkbookSavedState") = "Unknown"
    Evidence("WorkbookVBProjectAccessible") = False
    If OperationHistory Is Nothing Then
        Evidence.Add "OperationHistory", New Collection
    Else
        Evidence.Add "OperationHistory", OperationHistory
    End If
    Evidence.Add "RemainingAuthorizedLifecycleOperations", RemainingOperations

    Set CreateFailedWorkbookLifecycleEvidence = Evidence
End Function

Private Sub AttachWorkbookLifecycleEvidence(ByVal Result As Object, ByVal Evidence As Object)
    If Result.Exists("WorkbookLifecycleEvidence") Then
        Result.Remove "WorkbookLifecycleEvidence"
    End If
    Result.Add "WorkbookLifecycleEvidence", Evidence
End Sub

Private Function CreateWriteUnit(ByVal Unit As Variant) As Object
    Dim Source As Object
    Dim WriteUnit As Object
    Dim ModuleType As String

    If Not IsObject(Unit) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.CreateWriteUnit", "Generator output unit is invalid."
    End If

    Set Source = Unit
    RequireCompleteGeneratedUnit Source

    ModuleType = RequiredText(Source, "moduleType")

    Set WriteUnit = CreateObject("Scripting.Dictionary")
    WriteUnit("order") = CLng(Source("order"))
    WriteUnit("moduleName") = RequiredText(Source, "moduleName")
    WriteUnit("moduleType") = ModuleType
    WriteUnit("templateKey") = RequiredText(Source, "templateKey")
    WriteUnit("fileName") = RequiredText(Source, "moduleName") & ModuleExtension(ModuleType)
    WriteUnit("generatedSource") = RequiredText(Source, "generatedSource")
    WriteUnit("writeStatus") = "Planned"

    Set CreateWriteUnit = WriteUnit
End Function

Private Sub PreflightWriteUnits(ByVal WriteUnits As Collection, ByVal FileSystem As Object, ByVal OutputFolderPath As String)
    Dim Unit As Variant
    Dim FileName As String
    Dim FilePath As String

    For Each Unit In WriteUnits
        If Not IsObject(Unit) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightWriteUnits", "Output write unit is invalid."
        End If

        FileName = RequiredText(Unit, "fileName")
        RequireSafeFileName FileName, "AppOutputWriteService.PreflightWriteUnits"
        RequiredText Unit, "generatedSource"
        FilePath = FileSystem.BuildPath(OutputFolderPath, FileName)
        If FileSystem.FileExists(FilePath) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightWriteUnits", "Output file already exists: " & FileName
        End If
    Next Unit
End Sub

Private Function WriteAllUnits(ByVal WriteUnits As Collection, ByVal FileSystem As Object, ByVal OutputFolderPath As String) As Collection
    Dim Unit As Variant
    Dim TextFile As Object
    Dim WrittenFiles As Collection
    Dim FilePath As String

    Set WrittenFiles = New Collection
    For Each Unit In WriteUnits
        FilePath = FileSystem.BuildPath(OutputFolderPath, RequiredText(Unit, "fileName"))
        Set TextFile = FileSystem.CreateTextFile(FilePath, False, False)
        TextFile.Write RequiredText(Unit, "generatedSource")
        TextFile.Close
        WrittenFiles.Add FilePath
    Next Unit

    Set WriteAllUnits = WrittenFiles
End Function

Private Sub PreflightLocalTargetMutation(ByVal WriteUnits As Collection, ByVal TargetProject As Object)
    Dim Unit As Variant
    Dim Modules As Object
    Dim Seen As Object
    Dim ModuleName As String

    If Not TargetProject.Exists("Modules") Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Local target project Modules collection is required."
    End If

    If Not IsObject(TargetProject("Modules")) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Local target project Modules collection is invalid."
    End If

    Set Modules = TargetProject("Modules")
    Set Seen = CreateObject("Scripting.Dictionary")

    For Each Unit In WriteUnits
        If Not IsObject(Unit) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Output write unit is invalid."
        End If

        ModuleName = RequiredText(Unit, "moduleName")
        RequireSafeFileName RequiredText(Unit, "fileName"), "AppOutputWriteService.PreflightLocalTargetMutation"
        RequiredText Unit, "generatedSource"

        If Seen.Exists(ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Duplicate module mutation is prohibited: " & ModuleName
        End If

        If Modules.Exists(ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Existing module conflict is not authorized: " & ModuleName
        End If

        Seen(ModuleName) = True
    Next Unit
End Sub

Private Sub ApplyLocalTargetMutation(ByVal WriteUnits As Collection, ByVal TargetProject As Object)
    Dim Unit As Variant
    Dim Modules As Object

    Set Modules = TargetProject("Modules")

    For Each Unit In WriteUnits
        Modules(RequiredText(Unit, "moduleName")) = RequiredText(Unit, "generatedSource")
    Next Unit
End Sub

Private Sub PreflightRealVBProjectMutation(ByVal WriteUnits As Collection, ByVal TargetVBProject As Object)
    Dim Components As Object
    Dim Unit As Variant
    Dim Seen As Object
    Dim ModuleName As String

    Set Components = RequireVBComponents(TargetVBProject, "AppOutputWriteService.PreflightRealVBProjectMutation")
    Set Seen = CreateObject("Scripting.Dictionary")

    For Each Unit In WriteUnits
        If Not IsObject(Unit) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightRealVBProjectMutation", "Output write unit is invalid."
        End If

        ModuleName = RequiredText(Unit, "moduleName")
        RequireSafeFileName RequiredText(Unit, "fileName"), "AppOutputWriteService.PreflightRealVBProjectMutation"
        RequiredText Unit, "generatedSource"
        RequireSupportedModuleType RequiredText(Unit, "moduleType"), "AppOutputWriteService.PreflightRealVBProjectMutation"

        If Seen.Exists(ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightRealVBProjectMutation", "Duplicate module mutation is prohibited: " & ModuleName
        End If

        If RealVBProjectModuleExists(Components, ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightRealVBProjectMutation", "Existing module conflict is not authorized: " & ModuleName
        End If

        Seen(ModuleName) = True
    Next Unit
End Sub

Private Function ApplyRealVBProjectMutation(ByVal WriteUnits As Collection, ByVal TargetVBProject As Object) As Collection
    Dim Components As Object
    Dim CreatedComponents As Collection
    Dim Unit As Variant
    Dim Component As Object

    Set Components = RequireVBComponents(TargetVBProject, "AppOutputWriteService.ApplyRealVBProjectMutation")
    Set CreatedComponents = New Collection

    For Each Unit In WriteUnits
        ApplyControlledCreationFault Unit
        Set Component = Components.Add(ComponentTypeForModuleType(RequiredText(Unit, "moduleType")))
        Component.Name = RequiredText(Unit, "moduleName")
        Component.CodeModule.AddFromString RequiredText(Unit, "generatedSource")
        CreatedComponents.Add Component
    Next Unit

    Set ApplyRealVBProjectMutation = CreatedComponents
End Function

Private Sub ApplyControlledCreationFault(ByVal Unit As Variant)
    Dim FaultKind As String

    If Not IsObject(Unit) Then
        Exit Sub
    End If

    If Not Unit.Exists("controlledCreationFault") Then
        Exit Sub
    End If

    FaultKind = Trim$(CStr(Unit("controlledCreationFault")))
    Select Case FaultKind
        Case "AddComponentFailure"
            Err.Raise ComErrInvalidState, "AppOutputWriteService.ApplyControlledCreationFault", "Controlled component creation failure."
        Case ""
        Case Else
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ApplyControlledCreationFault", "Controlled creation fault is not supported."
    End Select
End Sub

Private Sub ApplyControlledReadbackFault(ByVal WriteUnits As Collection, ByVal TargetVBProject As Object)
    Dim Components As Object
    Dim Unit As Variant
    Dim Component As Object
    Dim FaultKind As String

    Set Components = RequireVBComponents(TargetVBProject, "AppOutputWriteService.ApplyControlledReadbackFault")

    For Each Unit In WriteUnits
        If IsObject(Unit) Then
            If Unit.Exists("controlledReadbackFault") Then
                FaultKind = Trim$(CStr(Unit("controlledReadbackFault")))
                Select Case FaultKind
                    Case "MissingComponent"
                        Set Component = Components.Item(RequiredText(Unit, "moduleName"))
                        Components.Remove Component
                    Case "MismatchedSource"
                        Set Component = Components.Item(RequiredText(Unit, "moduleName"))
                        If Component.CodeModule.CountOfLines > 0 Then
                            Component.CodeModule.DeleteLines 1, Component.CodeModule.CountOfLines
                        End If
                        Component.CodeModule.AddFromString "Option Explicit" & vbCrLf & "' controlled readback mismatch"
                    Case ""
                    Case Else
                        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ApplyControlledReadbackFault", "Controlled readback fault is not supported."
                End Select
            End If
        End If
    Next Unit
End Sub

Private Sub VerifyRealVBProjectReadback(ByVal WriteUnits As Collection, ByVal TargetVBProject As Object)
    Dim Components As Object
    Dim Unit As Variant
    Dim Component As Object
    Dim CodeText As String
    Dim ExpectedComponentType As Long

    Set Components = RequireVBComponents(TargetVBProject, "AppOutputWriteService.VerifyRealVBProjectReadback")

    For Each Unit In WriteUnits
        Set Component = Components.Item(RequiredText(Unit, "moduleName"))
        ExpectedComponentType = ComponentTypeForModuleType(RequiredText(Unit, "moduleType"))
        If CLng(Component.Type) <> ExpectedComponentType Then
            Err.Raise ComErrInvalidState, "AppOutputWriteService.VerifyRealVBProjectReadback", "Readback verification failed: " & RequiredText(Unit, "moduleName")
        End If
        CodeText = Component.CodeModule.Lines(1, Component.CodeModule.CountOfLines)
        If InStr(1, CodeText, RequiredText(Unit, "generatedSource"), vbBinaryCompare) = 0 Then
            Err.Raise ComErrInvalidState, "AppOutputWriteService.VerifyRealVBProjectReadback", "Readback verification failed: " & RequiredText(Unit, "moduleName")
        End If
    Next Unit
End Sub

Private Function RequireVBComponents(ByVal TargetVBProject As Object, ByVal SourceName As String) As Object
    Dim Components As Object
    Dim ComponentCount As Long

    If TargetVBProject Is Nothing Then
        Err.Raise ComErrInvalidArgument, SourceName, "Target VBProject is required."
    End If

    Set Components = TargetVBProject.VBComponents
    ComponentCount = Components.Count
    Set RequireVBComponents = Components
End Function

Private Function RealVBProjectModuleExists(ByVal Components As Object, ByVal ModuleName As String) As Boolean
    Dim Component As Object

    For Each Component In Components
        If StrComp(CStr(Component.Name), ModuleName, vbBinaryCompare) = 0 Then
            RealVBProjectModuleExists = True
            Exit Function
        End If
    Next Component

    RealVBProjectModuleExists = False
End Function

Private Sub RollbackCreatedComponents(ByVal TargetVBProject As Object, ByVal CreatedComponents As Collection, ByVal WriteUnits As Collection)
    Dim Components As Object
    Dim Component As Variant

    Set Components = TargetVBProject.VBComponents
    For Each Component In CreatedComponents
        ApplyControlledRollbackRemovalFault Component, WriteUnits
        Components.Remove Component
    Next Component
End Sub

Private Sub ApplyControlledRollbackRemovalFault(ByVal Component As Variant, ByVal WriteUnits As Collection)
    Dim Unit As Variant
    Dim FaultKind As String

    If WriteUnits Is Nothing Then
        Exit Sub
    End If

    For Each Unit In WriteUnits
        If IsObject(Unit) Then
            If StrComp(RequiredText(Unit, "moduleName"), CStr(Component.Name), vbBinaryCompare) = 0 Then
                If Unit.Exists("controlledRollbackRemovalFault") Then
                    FaultKind = Trim$(CStr(Unit("controlledRollbackRemovalFault")))
                    Select Case FaultKind
                        Case "RemoveComponentFailure"
                            Err.Raise ComErrInvalidState, "AppOutputWriteService.ApplyControlledRollbackRemovalFault", "Controlled rollback removal failure: " & CStr(Component.Name)
                        Case ""
                        Case Else
                            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ApplyControlledRollbackRemovalFault", "Controlled rollback removal fault is not supported."
                    End Select
                End If
                Exit Sub
            End If
        End If
    Next Unit
End Sub

Private Sub RequireSupportedModuleType(ByVal ModuleType As String, ByVal SourceName As String)
    Select Case ModuleType
        Case "StandardModule", "ClassModule"
        Case Else
            Err.Raise ComErrInvalidArgument, SourceName, "ModuleType is not supported."
    End Select
End Sub

Private Function ComponentTypeForModuleType(ByVal ModuleType As String) As Long
    Select Case ModuleType
        Case "StandardModule"
            ComponentTypeForModuleType = ComponentTypeStandardModule
        Case "ClassModule"
            ComponentTypeForModuleType = ComponentTypeClassModule
        Case Else
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.ComponentTypeForModuleType", "ModuleType is not supported."
    End Select
End Function

Private Sub RequireCompleteGeneratedUnit(ByVal Unit As Object)
    If Unit Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Generator output unit is required."
    End If

    If Not HasNumericValue(Unit, "order") Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Generator output unit order is required."
    End If

    If Not HasBooleanValue(Unit, "isFallbackDerived", False) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Fallback Template selection is prohibited."
    End If

    If Not HasBooleanValue(Unit, "isImplicitlySelected", False) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Implicit Template selection is prohibited."
    End If

    RequiredText Unit, "moduleName"
    RequiredText Unit, "moduleType"
    RequiredText Unit, "templateKey"
    RequiredText Unit, "generatedSource"
End Sub

Private Function RequiredText(ByVal Source As Object, ByVal FieldName As String) As String
    If Source Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Source item is required."
    End If

    If Not Source.Exists(FieldName) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Required Generator output field is missing: " & FieldName
    End If

    If ComIsBlankText(CStr(Source.Item(FieldName))) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Required Generator output field is blank: " & FieldName
    End If

    RequiredText = Trim$(CStr(Source.Item(FieldName)))
End Function

Private Function ModuleExtension(ByVal ModuleType As String) As String
    If StrComp(ModuleType, "ClassModule", vbTextCompare) = 0 Then
        ModuleExtension = ".cls"
    Else
        ModuleExtension = ".bas"
    End If
End Function

Private Sub RequireSafeFileName(ByVal FileName As String, ByVal SourceName As String)
    If InStr(1, FileName, "\", vbBinaryCompare) > 0 Or InStr(1, FileName, "/", vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, SourceName, "Output write fileName must not include a path."
    End If
End Sub

Private Function HasBooleanValue(ByVal Source As Object, ByVal FieldName As String, ByVal ExpectedValue As Boolean) As Boolean
    If Source Is Nothing Then
        HasBooleanValue = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasBooleanValue = False
        Exit Function
    End If

    If VarType(Source.Item(FieldName)) <> vbBoolean Then
        HasBooleanValue = False
        Exit Function
    End If

    HasBooleanValue = (CBool(Source.Item(FieldName)) = ExpectedValue)
End Function

Private Function HasCollection(ByVal Source As Object, ByVal FieldName As String) As Boolean
    If Source Is Nothing Then
        HasCollection = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasCollection = False
        Exit Function
    End If

    HasCollection = TypeName(Source.Item(FieldName)) = "Collection"
End Function

Private Function HasNumericValue(ByVal Source As Object, ByVal FieldName As String) As Boolean
    If Source Is Nothing Then
        HasNumericValue = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasNumericValue = False
        Exit Function
    End If

    HasNumericValue = IsNumeric(Source.Item(FieldName))
End Function

Private Function CreateResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Result.Add "WriteUnits", New Collection

    Set CreateResult = Result
End Function

Private Function CreateWriteResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Result.Add "WrittenFiles", New Collection

    Set CreateWriteResult = Result
End Function

Private Function CreateSuccessfulWriteResult(ByVal WrittenFiles As Collection) As Object
    Dim Result As Object

    Set Result = CreateWriteResult(True, ResultSuccess, "Generated output written.")
    Result.Remove "WrittenFiles"
    Result.Add "WrittenFiles", WrittenFiles
    Set CreateSuccessfulWriteResult = Result
End Function

Private Function CreateMutationResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Result("MutatedModules") = 0

    Set CreateMutationResult = Result
End Function
