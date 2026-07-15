VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppSelfCheckService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppSelfCheckService
' Layer: Application
' Responsibility: Run reproducible VMF Studio self checks using production services.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private FileProvider As InfFileSystemProvider

Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider)
    If ProviderFileSystem Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppSelfCheckService.AppInitialize", "FileSystemProvider is required."
    End If
    Set FileProvider = ProviderFileSystem
End Sub

Public Function AppRunAllSelfChecks() As Collection
    Set AppRunAllSelfChecks = RunSelfChecks(vbNullString)
End Function

Public Function AppRunSelfChecksByCategory(ByVal CategoryFilter As String) As Collection
    Set AppRunSelfChecksByCategory = RunSelfChecks(CategoryFilter)
End Function

Private Function RunSelfChecks(ByVal CategoryFilter As String) As Collection
    Dim Results As Collection
    Dim RootPath As String

    Set Results = New Collection
    RootPath = CreateTempRoot()

    RunOne Results, "SC-001", "Self Check", "Settings can load defaults", CategoryFilter, RootPath
    RunOne Results, "SC-002", "Self Check", "Studio directories are accessible", CategoryFilter, RootPath
    RunOne Results, "MF-001", "Manifest", "Manifest load/save round trip", CategoryFilter, RootPath
    RunOne Results, "MF-002", "Manifest", "Manifest validation detects invalid input", CategoryFilter, RootPath
    RunOne Results, "TP-001", "Template", "Template placeholders and sections parse", CategoryFilter, RootPath
    RunOne Results, "TP-002", "Template", "Template validation detects bad template", CategoryFilter, RootPath
    RunOne Results, "PG-001", "Preview/Generate", "Preview and generate match", CategoryFilter, RootPath
    RunOne Results, "PG-002", "Preview/Generate", "Unresolved placeholder fails preview", CategoryFilter, RootPath
    RunOne Results, "VL-001", "Validation", "Validation rules detect module/member errors", CategoryFilter, RootPath
    RunOne Results, "ST-001", "Settings", "Settings save and reload", CategoryFilter, RootPath
    RunOne Results, "ST-002", "Settings", "Unknown setting key is warning", CategoryFilter, RootPath
    RunOne Results, "BK-001", "Backup/Restore", "Backup and restore round trip", CategoryFilter, RootPath

    CleanupTempRoot Results, RootPath
    Set RunSelfChecks = Results
End Function

Private Sub RunOne(ByVal Results As Collection, ByVal TestID As String, ByVal Category As String, ByVal TestName As String, ByVal CategoryFilter As String, ByVal RootPath As String)
    Dim Result As AppSelfCheckResult
    Dim StartedAt As Date

    If Not ComIsBlankText(CategoryFilter) Then
        If StrComp(CategoryFilter, Category, vbTextCompare) <> 0 Then Exit Sub
    End If

    StartedAt = Now
    Set Result = New AppSelfCheckResult
    Result.AppInitialize TestID, Category, TestName, StartedAt
    On Error GoTo Unexpected
    ExecuteTest TestID, Result, RootPath
    Results.Add Result
    Exit Sub

Unexpected:
    Result.AppFinish "Failed", "Unexpected Error: " & Err.Description, "No unexpected error", Err.Description, "VMF-SC-UNEXPECTED"
    Results.Add Result
End Sub

Private Sub ExecuteTest(ByVal TestID As String, ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Select Case TestID
        Case "SC-001": CheckSettingsDefaults Result, RootPath
        Case "SC-002": CheckStudioDirectories Result, RootPath
        Case "MF-001": CheckManifestRoundTrip Result, RootPath
        Case "MF-002": CheckManifestInvalid Result, RootPath
        Case "TP-001": CheckTemplateParse Result, RootPath
        Case "TP-002": CheckTemplateInvalid Result, RootPath
        Case "PG-001": CheckPreviewGenerateMatch Result, RootPath
        Case "PG-002": CheckPreviewUnresolvedPlaceholder Result, RootPath
        Case "VL-001": CheckValidationRules Result, RootPath
        Case "ST-001": CheckSettingsRoundTrip Result, RootPath
        Case "ST-002": CheckSettingsUnknownKey Result, RootPath
        Case "BK-001": CheckBackupRestore Result, RootPath
        Case Else: Result.AppFinish "Skipped", "Unknown test id.", "Known test id", TestID, "VMF-SC-SKIP"
    End Select
End Sub

Private Sub CheckSettingsDefaults(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppStudioSettingsService
    Dim Settings As AppStudioSettings
    Dim Issues As Collection
    Set Service = CreateSettingsService(RootPath)
    Service.AppLoadSettings Settings, Issues
    If Settings Is Nothing Then
        Result.AppFinish "Failed", "Settings did not load.", "Settings model", "Nothing", "VMF-SC-SET-001"
    Else
        Result.AppFinish "Passed", "Settings defaults loaded.", "Settings model", Settings.DefaultOverwriteMode, vbNullString
    End If
End Sub

Private Sub CheckStudioDirectories(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    If FolderExists(BuildPathResolver.TemplatesDirectoryPath()) And FolderExists(BuildPathResolver.RepositoryRootPath()) Then
        Result.AppFinish "Passed", "Repository and template directories are accessible.", "Accessible directories", "Accessible", vbNullString
    Else
        Result.AppFinish "Failed", "Required directories are not accessible.", "Accessible directories", "Missing", "VMF-SC-DIR-001"
    End If
End Sub

Private Sub CheckManifestRoundTrip(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppManifestEditorService
    Dim Modules As Collection
    Dim Reloaded As Collection
    Dim ManifestPath As String
    Dim ModuleInfo As Object
    Dim Members As Collection

    PrepareManifestFixture RootPath, ManifestPath
    Set Service = New AppManifestEditorService
    AssertSuccess Service.AppLoadManifestEditorModel(ManifestPath, Modules), "Manifest load"
    Set ModuleInfo = Modules.Item(1)
    Set Members = ModuleInfo("Members")
    Members.Add Service.AppCreateManifestEditorMember("Name", "String", "GetLet", "vbNullString", False)
    AssertSuccess Service.AppSaveManifestEditorModel(ManifestPath, Modules), "Manifest save"
    AssertSuccess Service.AppLoadManifestEditorModel(ManifestPath, Reloaded), "Manifest reload"
    If Reloaded.Item(1)("Members").Count = 1 Then
        Result.AppFinish "Passed", "Manifest load/save round trip passed.", "1 member", CStr(Reloaded.Item(1)("Members").Count), vbNullString
    Else
        Result.AppFinish "Failed", "Member count mismatch after reload.", "1 member", CStr(Reloaded.Item(1)("Members").Count), "VMF-SC-MF-001"
    End If
End Sub

Private Sub CheckManifestInvalid(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppManifestEditorService
    Dim Validation As AppManifestValidationService
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Issues As Collection

    Set Service = New AppManifestEditorService
    Set Validation = New AppManifestValidationService
    Set Modules = New Collection
    Set ModuleInfo = Service.AppCreateManifestEditorModule("", "Application", "ClassModule", FixtureTemplatePath(RootPath))
    Modules.Add ModuleInfo
    Validation.AppValidateManifestEditorModel FixtureManifestPath(RootPath), Modules, Issues
    If Validation.AppIssuesContainErrors(Issues) Then
        Result.AppFinish "Passed", "Invalid manifest was detected.", "Validation error", "Validation error", vbNullString
    Else
        Result.AppFinish "Failed", "Invalid manifest was not detected.", "Validation error", "No error", "VMF-SC-MF-002"
    End If
End Sub

Private Sub CheckTemplateParse(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Template As AppTemplateModel
    Dim Service As AppTemplateService
    Set Service = CreateTemplateService(RootPath)
    Set Template = Service.AppCreateTemplateModel("FixtureClassTemplate", "Class", FixtureTemplatePath(RootPath), FixtureClassTemplate())
    Service.AppAnalyzeTemplate Template
    If Template.Placeholders.Count > 0 And Template.Sections.Count > 0 Then
        Result.AppFinish "Passed", "Template placeholders and sections parsed.", "Placeholders and sections", CStr(Template.Placeholders.Count) & "/" & CStr(Template.Sections.Count), vbNullString
    Else
        Result.AppFinish "Failed", "Template analysis returned empty results.", "Non-empty analysis", CStr(Template.Placeholders.Count) & "/" & CStr(Template.Sections.Count), "VMF-SC-TP-001"
    End If
End Sub

Private Sub CheckTemplateInvalid(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Template As AppTemplateModel
    Dim Service As AppTemplateService
    Dim Issues As Collection
    Set Service = CreateTemplateService(RootPath)
    Set Template = Service.AppCreateTemplateModel("BadTemplate", "Class", FixtureTemplatePath(RootPath), "Option Explicit" & vbCrLf & "@section" & vbCrLf & "{{UnknownPlaceholder}}")
    Service.AppValidateTemplate Template, Issues
    If Service.AppIssuesContainErrors(Issues) Then
        Result.AppFinish "Passed", "Invalid template was detected.", "Template validation error", "Template validation error", vbNullString
    Else
        Result.AppFinish "Failed", "Invalid template was not detected.", "Template validation error", "No error", "VMF-SC-TP-002"
    End If
End Sub

Private Sub CheckPreviewGenerateMatch(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Editor As AppManifestEditorService
    Dim Preview As AppCodePreviewService
    Dim Generate As AppManifestGenerateService
    Dim ManifestPath As String
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim PreviewText As String
    Dim SelectedNames As Collection
    Dim Request As AppGenerateRequest
    Dim GenerateResult As AppGenerateResult
    Dim OutputPath As String
    Dim GeneratedText As String

    PrepareManifestFixture RootPath, ManifestPath
    Set Editor = New AppManifestEditorService
    AssertSuccess Editor.AppLoadManifestEditorModel(ManifestPath, Modules), "Manifest load"
    Set ModuleInfo = Modules.Item(1)
    ModuleInfo("Members").Add Editor.AppCreateManifestEditorMember("Name", "String", "GetLet", "vbNullString", False)
    ModuleInfo("Members").Add Editor.AppCreateManifestEditorMember("Parent", "SampleParent", "GetSet", vbNullString, True)

    Set Preview = New AppCodePreviewService
    Preview.AppInitialize InfCreateGenerator(), Editor, New AppManifestValidationService
    AssertSuccess Preview.AppPreviewManifestEditorModule(ManifestPath, ModuleInfo, PreviewText), "Preview"

    Set SelectedNames = New Collection
    SelectedNames.Add CStr(ModuleInfo("ModuleName"))
    Set Generate = CreateGenerateService(RootPath)
    Set Request = Generate.AppCreateGenerateRequest("CurrentModule", SelectedNames, BuildPathResolver.CombinePath(RootPath, "out"), "Overwrite", False)
    AssertSuccess Generate.AppGenerateManifestEditorModel(ManifestPath, Modules, Request, GenerateResult), "Generate"
    OutputPath = BuildPathResolver.CombinePath(BuildPathResolver.CombinePath(RootPath, "out"), CStr(ModuleInfo("ModuleName")) & ".cls")
    GeneratedText = FileProvider.InfReadText(OutputPath)
    If NormalizeForCompare(PreviewText) = NormalizeForCompare(GeneratedText) Then
        Result.AppFinish "Passed", "Preview and generate output match.", "Equal code", "Equal code", vbNullString
    Else
        Result.AppFinish "Failed", "Preview and generate output differ.", "Equal code", "Different code", "VMF-SC-PG-001"
    End If
End Sub

Private Sub CheckPreviewUnresolvedPlaceholder(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppTemplateService
    Dim Editor As AppManifestEditorService
    Dim ManifestPath As String
    Dim Modules As Collection
    Dim Template As AppTemplateModel
    Dim PreviewText As String
    Dim CheckResult As ComResult

    PrepareManifestFixture RootPath, ManifestPath
    Set Editor = New AppManifestEditorService
    Editor.AppLoadManifestEditorModel ManifestPath, Modules
    Set Service = CreateTemplateService(RootPath)
    Set Template = Service.AppCreateTemplateModel("BadPreview", "Class", FixtureTemplatePath(RootPath), FixtureClassTemplate() & vbCrLf & "{{UnknownPlaceholder}}")
    Set CheckResult = Service.AppPreviewTemplateWithModule(ManifestPath, Template, Modules.Item(1), PreviewText)
    If CheckResult.IsFailure Then
        Result.AppFinish "Passed", "Unresolved placeholder failed preview.", "Preview failure", "Preview failure", vbNullString
    Else
        Result.AppFinish "Failed", "Unresolved placeholder did not fail preview.", "Preview failure", "Preview success", "VMF-SC-PG-002"
    End If
End Sub

Private Sub CheckValidationRules(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Editor As AppManifestEditorService
    Dim Validation As AppManifestValidationService
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Issues As Collection

    Set Editor = New AppManifestEditorService
    Set Validation = New AppManifestValidationService
    Set Modules = New Collection
    Set ModuleInfo = Editor.AppCreateManifestEditorModule("ValidationModule", "Application", "ClassModule", FixtureTemplatePath(RootPath))
    ModuleInfo("Members").Add Editor.AppCreateManifestEditorMember("Child", "SampleChild", "GetLet", vbNullString, True)
    Modules.Add ModuleInfo
    Modules.Add Editor.AppCreateManifestEditorModule("ValidationModule", "Application", "ClassModule", FixtureTemplatePath(RootPath))
    Validation.AppValidateManifestEditorModel FixtureManifestPath(RootPath), Modules, Issues
    If Validation.AppIssuesContainErrors(Issues) Then
        Result.AppFinish "Passed", "Validation rules detected errors.", "Validation error", "Validation error", vbNullString
    Else
        Result.AppFinish "Failed", "Validation rules did not detect errors.", "Validation error", "No error", "VMF-SC-VL-001"
    End If
End Sub

Private Sub CheckSettingsRoundTrip(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppStudioSettingsService
    Dim Settings As AppStudioSettings
    Dim Reloaded As AppStudioSettings
    Dim Issues As Collection
    Set Service = CreateSettingsService(RootPath)
    Set Settings = Service.AppCreateDefaultSettings()
    Settings.BackupDirectory = BuildPathResolver.CombinePath(RootPath, "backups")
    Settings.DefaultOverwriteMode = "Skip"
    AssertSuccess Service.AppSaveSettings(Settings, Issues), "Settings save"
    AssertSuccess Service.AppLoadSettings(Reloaded, Issues), "Settings reload"
    If Reloaded.DefaultOverwriteMode = "Skip" Then
        Result.AppFinish "Passed", "Settings saved and reloaded.", "Skip", Reloaded.DefaultOverwriteMode, vbNullString
    Else
        Result.AppFinish "Failed", "Settings reload mismatch.", "Skip", Reloaded.DefaultOverwriteMode, "VMF-SC-ST-001"
    End If
End Sub

Private Sub CheckSettingsUnknownKey(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Service As AppStudioSettingsService
    Dim Settings As AppStudioSettings
    Dim Issues As Collection
    Set Service = CreateSettingsService(RootPath)
    FileProvider.InfWriteText BuildPathResolver.CombinePath(RootPath, "settings.ini"), "UnknownFutureKey=1" & vbCrLf & "PreviewFontSize=bad"
    Service.AppLoadSettings Settings, Issues
    If Not Issues Is Nothing And Issues.Count > 0 Then
        Result.AppFinish "Passed", "Unknown/bad settings are reported without blocking load.", "Warning", "Warning", vbNullString
    Else
        Result.AppFinish "Failed", "Unknown/bad settings were not reported.", "Warning", "No issue", "VMF-SC-ST-002"
    End If
End Sub

Private Sub CheckBackupRestore(ByVal Result As AppSelfCheckResult, ByVal RootPath As String)
    Dim Backup As AppBackupService
    Dim FilePath As String
    Dim BackupPath As String
    Dim CheckResult As ComResult

    FilePath = BuildPathResolver.CombinePath(RootPath, "BackupTemplate.txt")
    FileProvider.InfWriteText FilePath, FixtureClassTemplate()
    Set Backup = CreateBackupService(RootPath)
    AssertSuccess Backup.AppSafeSaveText("Template", FilePath, FixtureClassTemplate() & vbCrLf & "' changed", "Update", BackupPath), "Safe save"
    Set CheckResult = Backup.AppRestoreBackup(BackupPath)
    If CheckResult.IsSuccess And InStr(1, FileProvider.InfReadText(FilePath), "' changed", vbTextCompare) = 0 Then
        Result.AppFinish "Passed", "Backup restore round trip passed.", "Original content", "Original content", vbNullString
    Else
        Result.AppFinish "Failed", "Backup restore failed.", "Restore success", CheckResult.Message, "VMF-SC-BK-001"
    End If
End Sub

Private Sub PrepareManifestFixture(ByVal RootPath As String, ByRef ManifestPath As String)
    FileProvider.InfEnsureFolder BuildPathResolver.CombinePath(RootPath, "templates")
    FileProvider.InfWriteText FixtureTemplatePath(RootPath), FixtureClassTemplate()
    ManifestPath = FixtureManifestPath(RootPath)
    FileProvider.InfWriteText ManifestPath, "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "SelfCheckClass,ClassModule,Application,templates\ClassTemplate.txt"
End Sub

Private Function FixtureClassTemplate() As String
    FixtureClassTemplate = _
        "Option Explicit" & vbCrLf & _
        "@section ModuleDeclaration" & vbCrLf & "{{ModuleDeclaration}}" & vbCrLf & "@endsection" & vbCrLf & _
        "' Class: {{ModuleName}}" & vbCrLf & _
        "' Layer: {{Layer}}" & vbCrLf & _
        "@section MemberBlock" & vbCrLf & "{{MemberBlock}}" & vbCrLf & "@endsection" & vbCrLf & _
        "@section Properties" & vbCrLf & "{{Properties}}" & vbCrLf & "@endsection" & vbCrLf & _
        "Private Sub Class_Initialize()" & vbCrLf & _
        "@section InitializeBody" & vbCrLf & "{{InitializeBody}}" & vbCrLf & "@endsection" & vbCrLf & _
        "End Sub" & vbCrLf & _
        "Private Sub Class_Terminate()" & vbCrLf & _
        "@section TerminateBody" & vbCrLf & "{{TerminateBody}}" & vbCrLf & "@endsection" & vbCrLf & _
        "End Sub" & vbCrLf & _
        "{{BODY}}" & vbCrLf
End Function

Private Function FixtureManifestPath(ByVal RootPath As String) As String
    FixtureManifestPath = BuildPathResolver.CombinePath(RootPath, "selfcheck.manifest")
End Function

Private Function FixtureTemplatePath(ByVal RootPath As String) As String
    FixtureTemplatePath = BuildPathResolver.CombinePath(BuildPathResolver.CombinePath(RootPath, "templates"), "ClassTemplate.txt")
End Function

Private Function CreateTempRoot() As String
    CreateTempRoot = BuildPathResolver.CombinePath(Environ$("TEMP"), "VMF_SelfCheck_" & Format$(Now, "yyyymmdd_hhnnss") & "_" & CStr(Int(Timer * 1000)))
    FileProvider.InfEnsureFolder CreateTempRoot
End Function

Private Sub CleanupTempRoot(ByVal Results As Collection, ByVal RootPath As String)
    On Error GoTo CleanupFailed
    If FolderExists(RootPath) Then CreateObject("Scripting.FileSystemObject").DeleteFolder RootPath, True
    Exit Sub
CleanupFailed:
    Dim Result As AppSelfCheckResult
    Set Result = New AppSelfCheckResult
    Result.AppInitialize "CL-001", "Cleanup", "Temporary file cleanup", Now
    Result.AppFinish "Warning", "Cleanup Error: " & Err.Description, "Temporary directory removed", RootPath, "VMF-SC-CLEANUP"
    Results.Add Result
End Sub

Private Function CreateSettingsService(ByVal RootPath As String) As AppStudioSettingsService
    Set CreateSettingsService = New AppStudioSettingsService
    CreateSettingsService.AppInitialize FileProvider, BuildPathResolver.CombinePath(RootPath, "settings.ini")
End Function

Private Function CreateTemplateService(ByVal RootPath As String) As AppTemplateService
    Set CreateTemplateService = New AppTemplateService
    CreateTemplateService.AppInitialize InfCreateGenerator(), New AppManifestEditorService, FileProvider, CreateSettingsService(RootPath)
End Function

Private Function CreateGenerateService(ByVal RootPath As String) As AppManifestGenerateService
    Set CreateGenerateService = New AppManifestGenerateService
    CreateGenerateService.AppInitialize InfCreateGenerator(), New AppManifestEditorService, New AppManifestValidationService, FileProvider, CreateSettingsService(RootPath)
End Function

Private Function CreateBackupService(ByVal RootPath As String) As AppBackupService
    Dim SettingsService As AppStudioSettingsService
    Dim TemplateService As AppTemplateService
    Set SettingsService = CreateSettingsService(RootPath)
    Set TemplateService = New AppTemplateService
    TemplateService.AppInitialize InfCreateGenerator(), New AppManifestEditorService, FileProvider, SettingsService
    Set CreateBackupService = New AppBackupService
    CreateBackupService.AppInitialize FileProvider, SettingsService, New AppManifestEditorService, New AppManifestValidationService, TemplateService, BuildPathResolver.CombinePath(RootPath, "backups")
End Function

Private Sub AssertSuccess(ByVal Result As ComResult, ByVal OperationName As String)
    If Result Is Nothing Then Err.Raise ComErrInvalidState, "AppSelfCheckService", OperationName & " returned no result."
    If Result.IsFailure Then Err.Raise ComErrInvalidState, "AppSelfCheckService", OperationName & " failed: " & Result.Message
End Sub

Private Function NormalizeForCompare(ByVal TextValue As String) As String
    Dim Result As String
    Result = Replace(TextValue, vbCrLf, vbLf)
    Result = Replace(Result, vbCr, vbLf)
    Do While InStr(1, Result, " " & vbLf, vbBinaryCompare) > 0
        Result = Replace(Result, " " & vbLf, vbLf)
    Loop
    Do While Right$(Result, 1) = vbLf Or Right$(Result, 1) = " "
        Result = Left$(Result, Len(Result) - 1)
    Loop
    NormalizeForCompare = Result
End Function

Private Function FolderExists(ByVal FolderPath As String) As Boolean
    FolderExists = CreateObject("Scripting.FileSystemObject").FolderExists(FolderPath)
End Function
