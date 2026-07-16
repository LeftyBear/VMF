# Public API

Product : VMF Studio
Version : 1.1.0
Status  : v1.1 design freeze inventory

This document lists Public VBA members exported by src/Build at the VMF Studio v1.1.0 release freeze. Public members are compatibility-sensitive unless explicitly documented as internal factory/model support.

## Review Summary

- The stable external entry points are the facade modules: ComFacade, DomFacade, InfFacade, AppFacade, and PreFacade.
- Structured result/model classes expose Public initialization methods and properties so UI and services can exchange typed data without parsing display strings.
- No v1.0 public API removals were made for v1.1.0.
- UI-only behavior remains concentrated in Presentation; PreManifestEditorForm.PreOpenManifest is public only for form bootstrapping.

## Public Members

### `src\Build\Application\AppBackupEntry.cls`

- Line 32: `Public Sub AppInitialize(ByVal BackupID As String, ByVal TargetType As String, ByVal TargetName As String, ByVal FilePath As String, ByVal BackupPath As String, ByVal CreatedAt As Date, ByVal SizeBytes As Double, ByVal Action As String)`
- Line 43: `Public Property Get BackupID() As String: BackupID = This.BackupID: End Property`
- Line 44: `Public Property Get TargetType() As String: TargetType = This.TargetType: End Property`
- Line 45: `Public Property Get TargetName() As String: TargetName = This.TargetName: End Property`
- Line 46: `Public Property Get FilePath() As String: FilePath = This.FilePath: End Property`
- Line 47: `Public Property Get BackupPath() As String: BackupPath = This.BackupPath: End Property`
- Line 48: `Public Property Get CreatedAt() As Date: CreatedAt = This.CreatedAt: End Property`
- Line 49: `Public Property Get SizeBytes() As Double: SizeBytes = This.SizeBytes: End Property`
- Line 50: `Public Property Get Action() As String: Action = This.Action: End Property`

### `src\Build\Application\AppBackupHistoryEntry.cls`

- Line 35: `Public Sub AppInitialize(ByVal HistoryID As String, ByVal Timestamp As Date, ByVal TargetType As String, ByVal TargetName As String, ByVal FilePath As String, ByVal Action As String, ByVal BackupPath As String, ByVal Result As String, ByVal Message As String, ByVal UserName As String, ByVal StudioVersion As String)`
- Line 49: `Public Property Get HistoryID() As String: HistoryID = This.HistoryID: End Property`
- Line 50: `Public Property Get Timestamp() As Date: Timestamp = This.Timestamp: End Property`
- Line 51: `Public Property Get TargetType() As String: TargetType = This.TargetType: End Property`
- Line 52: `Public Property Get TargetName() As String: TargetName = This.TargetName: End Property`
- Line 53: `Public Property Get FilePath() As String: FilePath = This.FilePath: End Property`
- Line 54: `Public Property Get Action() As String: Action = This.Action: End Property`
- Line 55: `Public Property Get BackupPath() As String: BackupPath = This.BackupPath: End Property`
- Line 56: `Public Property Get Result() As String: Result = This.Result: End Property`
- Line 57: `Public Property Get Message() As String: Message = This.Message: End Property`
- Line 58: `Public Property Get UserName() As String: UserName = This.UserName: End Property`
- Line 59: `Public Property Get StudioVersion() As String: StudioVersion = This.StudioVersion: End Property`

### `src\Build\Application\AppBackupService.cls`

- Line 26: `Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider, ByVal ProviderSettings As AppStudioSettingsService, ByVal EditorService As AppManifestEditorService, ByVal ValidationService As AppManifestValidationService, ByVal ProviderTemplateService As AppTemplateService, Optional ByVal OverrideBackupDirectory As String = vbNullString)`
- Line 35: `Public Function AppSafeSaveText(ByVal TargetType As String, ByVal FilePath As String, ByVal NewContent As String, ByVal ActionName As String, ByRef BackupPath As String) As ComResult`
- Line 84: `Public Function AppCreateBackup(ByVal TargetType As String, ByVal FilePath As String, ByVal ActionName As String, ByRef BackupPath As String) As ComResult`
- Line 108: `Public Function AppListBackups(Optional ByVal TargetTypeFilter As String = vbNullString) As Collection`
- Line 134: `Public Function AppListHistory() As Collection`
- Line 165: `Public Function AppRestoreBackup(ByVal BackupPath As String) As ComResult`
- Line 205: `Public Function AppDeleteBackup(ByVal BackupPath As String) As ComResult`

### `src\Build\Application\AppBuildLogService.cls`

- Line 19: `Public Function AppRenderBuildLog(ByVal GenerateResult As AppGenerateResult) As String`

### `src\Build\Application\AppBuildService.cls`

- Line 24: `Public Function AppBuildProject( _`

### `src\Build\Application\AppCodePreviewService.cls`

- Line 24: `Public Sub AppInitialize( _`
- Line 47: `Public Function AppPreviewManifestEditorModule( _`

### `src\Build\Application\AppCompositionRoot.cls`

- Line 24: `Public Function AppInitialize() As Boolean`
- Line 29: `Public Sub AppShutdown()`
- Line 33: `Public Function AppCreateBuildService() As AppBuildService`
- Line 38: `Public Function AppCreateExportService() As AppExportService`
- Line 43: `Public Function AppCreateImportService() As AppImportService`
- Line 48: `Public Function AppCreateValidationService() As AppValidationService`
- Line 53: `Public Function AppCreateGeneratorService() As AppGeneratorService`
- Line 68: `Public Function AppCreateManifestEditorService() As AppManifestEditorService`
- Line 78: `Public Function AppCreateManifestValidationService() As AppManifestValidationService`
- Line 83: `Public Function AppCreateCodePreviewService() As AppCodePreviewService`
- Line 93: `Public Function AppCreateManifestGenerateService() As AppManifestGenerateService`
- Line 108: `Public Function AppCreateBuildLogService() As AppBuildLogService`
- Line 113: `Public Function AppCreateTemplateService() As AppTemplateService`
- Line 123: `Public Function AppCreateStudioSettingsService() As AppStudioSettingsService`
- Line 133: `Public Function AppCreateBackupService() As AppBackupService`
- Line 156: `Public Function AppCreateSelfCheckService() As AppSelfCheckService`
- Line 166: `Public Function AppCreateGenerationEngine() As BuildGenerationEngine`
- Line 193: `Public Function AppCreateProjectProvider() As ProjectProvider`

### `src\Build\Application\AppExportService.cls`

- Line 30: `Public Function AppExportProject( _`

### `src\Build\Application\AppFacade.bas`

- Line 16: `Public Function AppInitialize() As Boolean`
- Line 24: `Public Sub AppShutdown()`
- Line 32: `Public Function AppValidateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As ComResult`
- Line 40: `Public Function AppExportProject( _`
- Line 54: `Public Function AppImportProject(ByVal InputFilePath As String) As ComResult`
- Line 62: `Public Function AppBuildProject( _`
- Line 78: `Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult`
- Line 86: `Public Function AppGenerateClass(ByVal ClassName As String) As ComResult`
- Line 94: `Public Function AppGenerateCommon() As ComResult`
- Line 102: `Public Function AppGenerateManifest() As ComResult`
- Line 110: `Public Function AppGenerateInfrastructure() As ComResult`
- Line 118: `Public Function AppGenerateDomain() As ComResult`
- Line 126: `Public Function AppGenerateApplication() As ComResult`
- Line 134: `Public Function AppGeneratePresentation() As ComResult`
- Line 142: `Public Function AppGenerateProject() As ComResult`
- Line 150: `Public Function AppPreviewBuildLayer(ByVal LayerName As String, ByRef PreviewText As String) As ComResult`
- Line 158: `Public Function AppLoadManifestEditorModel( _`
- Line 169: `Public Function AppSaveManifestEditorModel( _`
- Line 180: `Public Function AppCreateManifestEditorModule( _`
- Line 193: `Public Function AppCreateManifestEditorMember( _`
- Line 212: `Public Function AppPreviewManifestEditorModule( _`
- Line 224: `Public Function AppValidateManifestEditorModel( _`
- Line 236: `Public Function AppValidateManifestEditorModule( _`
- Line 248: `Public Function AppValidationIssuesContainErrors(ByVal Issues As Collection) As Boolean`
- Line 256: `Public Function AppCreateManifestGenerateRequest( _`
- Line 275: `Public Function AppGenerateManifestEditorModel( _`
- Line 292: `Public Function AppRenderBuildLog(ByVal GenerateResult As AppGenerateResult) As String`
- Line 300: `Public Function AppDefaultManifestGenerateOutputDirectory(ByVal ManifestPath As String) As String`
- Line 308: `Public Function AppLoadTemplateManagerTemplates( _`
- Line 320: `Public Function AppLoadTemplateManagerTemplateFile( _`
- Line 331: `Public Function AppCreateTemplateManagerTemplate( _`
- Line 344: `Public Function AppDefaultTemplateManagerFilePath(ByVal TemplateFileName As String) As String`
- Line 352: `Public Function AppAnalyzeTemplateModel(ByVal Template As AppTemplateModel) As ComResult`
- Line 360: `Public Function AppValidateTemplateModel(ByVal Template As AppTemplateModel, ByRef Issues As Collection) As ComResult`
- Line 368: `Public Function AppSaveTemplateModel(ByVal Template As AppTemplateModel) As ComResult`
- Line 376: `Public Function AppSaveTemplateModelAs( _`
- Line 388: `Public Function AppPreviewTemplateModelWithModule( _`
- Line 401: `Public Function AppTemplateIssuesContainErrors(ByVal Issues As Collection) As Boolean`
- Line 409: `Public Function AppLoadStudioSettings(ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 417: `Public Function AppSaveStudioSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 425: `Public Function AppValidateStudioSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 433: `Public Function AppCreateDefaultStudioSettings() As AppStudioSettings`
- Line 441: `Public Function AppStudioSettingsIssuesContainErrors(ByVal Issues As Collection) As Boolean`
- Line 449: `Public Function AppResolveStudioSettingsPath(ByVal BasePath As String, ByVal PathValue As String) As String`
- Line 457: `Public Function AppListBackups(Optional ByVal TargetTypeFilter As String = vbNullString) As Collection`
- Line 462: `Public Function AppListBackupHistory() As Collection`
- Line 467: `Public Function AppRestoreBackup(ByVal BackupPath As String) As ComResult`
- Line 472: `Public Function AppDeleteBackup(ByVal BackupPath As String) As ComResult`
- Line 477: `Public Function AppRunStudioSelfChecks() As Collection`
- Line 482: `Public Function AppRunStudioSelfChecksByCategory(ByVal CategoryName As String) As Collection`

### `src\Build\Application\AppGenerateRequest.cls`

- Line 31: `Public Sub AppInitialize( _`
- Line 57: `Public Property Get TargetScope() As String`
- Line 62: `Public Property Get SelectedModuleNames() As Collection`
- Line 67: `Public Property Get OutputDirectory() As String`
- Line 72: `Public Property Get OverwriteMode() As String`
- Line 77: `Public Property Get ContinueOnError() As Boolean`
- Line 82: `Public Property Get RequestedAt() As Date`

### `src\Build\Application\AppGenerateResult.cls`

- Line 33: `Public Sub AppInitialize(ByVal StartedAt As Date, ByVal OutputDirectory As String)`
- Line 47: `Public Sub AppAddModuleResult(ByVal ModuleResult As AppModuleGenerateResult)`
- Line 67: `Public Sub AppFinish(ByVal FinishedAt As Date)`
- Line 72: `Public Property Get StartedAt() As Date`
- Line 77: `Public Property Get FinishedAt() As Date`
- Line 82: `Public Property Get SuccessCount() As Long`
- Line 87: `Public Property Get WarningCount() As Long`
- Line 92: `Public Property Get FailureCount() As Long`
- Line 97: `Public Property Get SkippedCount() As Long`
- Line 102: `Public Property Get OutputDirectory() As String`
- Line 107: `Public Property Get ModuleResults() As Collection`

### `src\Build\Application\AppGeneratorService.cls`

- Line 28: `Public Sub AppInitialize( _`
- Line 67: `Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult`
- Line 108: `Public Function AppGenerateClass(ByVal ClassName As String) As ComResult`
- Line 145: `Public Function AppGenerateCommon() As ComResult`
- Line 150: `Public Function AppGenerateManifest() As ComResult`
- Line 155: `Public Function AppGenerateInfrastructure() As ComResult`
- Line 160: `Public Function AppGenerateDomain() As ComResult`
- Line 165: `Public Function AppGenerateApplication() As ComResult`
- Line 170: `Public Function AppGeneratePresentation() As ComResult`
- Line 175: `Public Function AppGenerateProject() As ComResult`
- Line 180: `Public Function AppPreviewBuildLayer(ByVal LayerName As String, ByRef PreviewText As String) As ComResult`

### `src\Build\Application\AppImportService.cls`

- Line 24: `Public Function AppImportProject(ByVal InputFilePath As String) As ComResult`

### `src\Build\Application\AppManifestEditorService.cls`

- Line 25: `Public Sub AppInitialize(Optional ByVal ProviderBackup As AppBackupService = Nothing)`
- Line 29: `Public Function AppLoadManifestEditorModel( _`
- Line 50: `Public Function AppSaveManifestEditorModel( _`
- Line 79: `Public Function AppCreateManifestEditorModule( _`
- Line 100: `Public Function AppCreateManifestEditorMember( _`
- Line 122: `Public Function AppRenderManifestEditorMemberDefinitions(ByVal Members As Collection) As String`

### `src\Build\Application\AppManifestGenerateService.cls`

- Line 32: `Public Sub AppInitialize( _`
- Line 59: `Public Function AppCreateGenerateRequest( _`
- Line 73: `Public Function AppGenerateManifestEditorModel( _`
- Line 151: `Public Function AppRenderManifestEditorModule(ByVal ManifestPath As String, ByVal ModuleInfo As Object) As String`
- Line 155: `Public Function AppDefaultOutputDirectory(ByVal ManifestPath As String) As String`

### `src\Build\Application\AppManifestValidationIssue.cls`

- Line 32: `Public Sub AppInitialize( _`
- Line 55: `Public Property Get Severity() As Long`
- Line 60: `Public Property Get SeverityText() As String`
- Line 74: `Public Property Get Code() As String`
- Line 79: `Public Property Get Message() As String`
- Line 84: `Public Property Get ModuleName() As String`
- Line 89: `Public Property Get MemberName() As String`
- Line 94: `Public Property Get PropertyName() As String`
- Line 99: `Public Property Get SortOrder() As Long`

### `src\Build\Application\AppManifestValidationService.cls`

- Line 23: `Public Function AppSeverityError() As Long`
- Line 27: `Public Function AppSeverityWarning() As Long`
- Line 31: `Public Function AppSeverityInformation() As Long`
- Line 35: `Public Function AppValidateManifestEditorModel( _`
- Line 53: `Public Function AppValidateManifestEditorModule( _`
- Line 78: `Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean`
- Line 96: `Public Function AppCreateFailureFromIssues(ByVal Issues As Collection, ByVal OperationName As String) As ComResult`

### `src\Build\Application\AppModuleGenerateResult.cls`

- Line 34: `Public Sub AppInitialize( _`
- Line 62: `Public Property Get ModuleName() As String`
- Line 67: `Public Property Get ModuleType() As String`
- Line 72: `Public Property Get OutputPath() As String`
- Line 77: `Public Property Get Status() As String`
- Line 82: `Public Property Get Message() As String`
- Line 87: `Public Property Get ErrorCode() As String`
- Line 92: `Public Property Get ElapsedTime() As Double`
- Line 97: `Public Property Get WasOverwritten() As Boolean`
- Line 102: `Public Property Get LoggedAt() As Date`

### `src\Build\Application\AppReleaseInfo.bas`

- Line 17: `Public Function AppProductName() As String`
- Line 21: `Public Function AppProductVersion() As String`
- Line 25: `Public Function AppManifestSchemaVersion() As String`
- Line 29: `Public Function AppTemplateSchemaVersion() As String`
- Line 33: `Public Function AppMinimumSupportedVersion() As String`
- Line 37: `Public Function AppBuildDate() As String`

### `src\Build\Application\AppSelfCheckResult.cls`

- Line 35: `Public Sub AppInitialize(ByVal TestID As String, ByVal Category As String, ByVal TestName As String, ByVal StartedAt As Date)`
- Line 43: `Public Sub AppFinish(ByVal Status As String, ByVal Message As String, ByVal Expected As String, ByVal Actual As String, ByVal ErrorCode As String)`
- Line 53: `Public Property Get TestID() As String: TestID = This.TestID: End Property`
- Line 54: `Public Property Get Category() As String: Category = This.Category: End Property`
- Line 55: `Public Property Get TestName() As String: TestName = This.TestName: End Property`
- Line 56: `Public Property Get Status() As String: Status = This.Status: End Property`
- Line 57: `Public Property Get Message() As String: Message = This.Message: End Property`
- Line 58: `Public Property Get Expected() As String: Expected = This.Expected: End Property`
- Line 59: `Public Property Get Actual() As String: Actual = This.Actual: End Property`
- Line 60: `Public Property Get StartedAt() As Date: StartedAt = This.StartedAt: End Property`
- Line 61: `Public Property Get FinishedAt() As Date: FinishedAt = This.FinishedAt: End Property`
- Line 62: `Public Property Get ElapsedTime() As Double: ElapsedTime = This.ElapsedTime: End Property`
- Line 63: `Public Property Get ErrorCode() As String: ErrorCode = This.ErrorCode: End Property`

### `src\Build\Application\AppSelfCheckService.cls`

- Line 21: `Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider)`
- Line 28: `Public Function AppRunAllSelfChecks() As Collection`
- Line 32: `Public Function AppRunSelfChecksByCategory(ByVal CategoryFilter As String) As Collection`

### `src\Build\Application\AppStudioSettings.cls`

- Line 48: `Public Sub AppLoadDefaults()`
- Line 75: `Public Function AppClone() As AppStudioSettings`
- Line 105: `Public Function AppToText() As String`
- Line 133: `Public Property Get ManifestDirectory() As String: ManifestDirectory = This.ManifestDirectory: End Property`
- Line 134: `Public Property Let ManifestDirectory(ByVal Value As String): This.ManifestDirectory = Trim$(Value): End Property`
- Line 135: `Public Property Get TemplateDirectory() As String: TemplateDirectory = This.TemplateDirectory: End Property`
- Line 136: `Public Property Let TemplateDirectory(ByVal Value As String): This.TemplateDirectory = Trim$(Value): End Property`
- Line 137: `Public Property Get OutputDirectory() As String: OutputDirectory = This.OutputDirectory: End Property`
- Line 138: `Public Property Let OutputDirectory(ByVal Value As String): This.OutputDirectory = Trim$(Value): End Property`
- Line 139: `Public Property Get LogDirectory() As String: LogDirectory = This.LogDirectory: End Property`
- Line 140: `Public Property Let LogDirectory(ByVal Value As String): This.LogDirectory = Trim$(Value): End Property`
- Line 141: `Public Property Get BackupDirectory() As String: BackupDirectory = This.BackupDirectory: End Property`
- Line 142: `Public Property Let BackupDirectory(ByVal Value As String): This.BackupDirectory = Trim$(Value): End Property`
- Line 143: `Public Property Get MaxBackupCountPerFile() As Long: MaxBackupCountPerFile = This.MaxBackupCountPerFile: End Property`
- Line 144: `Public Property Let MaxBackupCountPerFile(ByVal Value As Long): This.MaxBackupCountPerFile = Value: End Property`
- Line 145: `Public Property Get MaxBackupAgeDays() As Long: MaxBackupAgeDays = This.MaxBackupAgeDays: End Property`
- Line 146: `Public Property Let MaxBackupAgeDays(ByVal Value As Long): This.MaxBackupAgeDays = Value: End Property`
- Line 147: `Public Property Get DefaultTargetScope() As String: DefaultTargetScope = This.DefaultTargetScope: End Property`
- Line 148: `Public Property Let DefaultTargetScope(ByVal Value As String): This.DefaultTargetScope = Trim$(Value): End Property`
- Line 149: `Public Property Get DefaultOverwriteMode() As String: DefaultOverwriteMode = This.DefaultOverwriteMode: End Property`
- Line 150: `Public Property Let DefaultOverwriteMode(ByVal Value As String): This.DefaultOverwriteMode = Trim$(Value): End Property`
- Line 151: `Public Property Get ContinueOnError() As Boolean: ContinueOnError = This.ContinueOnError: End Property`
- Line 152: `Public Property Let ContinueOnError(ByVal Value As Boolean): This.ContinueOnError = Value: End Property`
- Line 153: `Public Property Get CreateBackup() As Boolean: CreateBackup = This.CreateBackup: End Property`
- Line 154: `Public Property Let CreateBackup(ByVal Value As Boolean): This.CreateBackup = Value: End Property`
- Line 155: `Public Property Get OpenOutputAfterGenerate() As Boolean: OpenOutputAfterGenerate = This.OpenOutputAfterGenerate: End Property`
- Line 156: `Public Property Let OpenOutputAfterGenerate(ByVal Value As Boolean): This.OpenOutputAfterGenerate = Value: End Property`
- Line 157: `Public Property Get ValidateBeforeGenerate() As Boolean: ValidateBeforeGenerate = This.ValidateBeforeGenerate: End Property`
- Line 158: `Public Property Let ValidateBeforeGenerate(ByVal Value As Boolean): This.ValidateBeforeGenerate = Value: End Property`
- Line 159: `Public Property Get PreviewFontName() As String: PreviewFontName = This.PreviewFontName: End Property`
- Line 160: `Public Property Let PreviewFontName(ByVal Value As String): This.PreviewFontName = Trim$(Value): End Property`
- Line 161: `Public Property Get PreviewFontSize() As Long: PreviewFontSize = This.PreviewFontSize: End Property`
- Line 162: `Public Property Let PreviewFontSize(ByVal Value As Long): This.PreviewFontSize = Value: End Property`
- Line 163: `Public Property Get ShowLineNumbers() As Boolean: ShowLineNumbers = This.ShowLineNumbers: End Property`
- Line 164: `Public Property Let ShowLineNumbers(ByVal Value As Boolean): This.ShowLineNumbers = Value: End Property`
- Line 165: `Public Property Get PreserveBlankLines() As Boolean: PreserveBlankLines = This.PreserveBlankLines: End Property`
- Line 166: `Public Property Let PreserveBlankLines(ByVal Value As Boolean): This.PreserveBlankLines = Value: End Property`
- Line 167: `Public Property Get ConfirmOnUnsavedChanges() As Boolean: ConfirmOnUnsavedChanges = This.ConfirmOnUnsavedChanges: End Property`
- Line 168: `Public Property Let ConfirmOnUnsavedChanges(ByVal Value As Boolean): This.ConfirmOnUnsavedChanges = Value: End Property`
- Line 169: `Public Property Get LastProjectPath() As String: LastProjectPath = This.LastProjectPath: End Property`
- Line 170: `Public Property Let LastProjectPath(ByVal Value As String): This.LastProjectPath = Trim$(Value): End Property`
- Line 171: `Public Property Get LastSelectedModule() As String: LastSelectedModule = This.LastSelectedModule: End Property`
- Line 172: `Public Property Let LastSelectedModule(ByVal Value As String): This.LastSelectedModule = Trim$(Value): End Property`
- Line 173: `Public Property Get LastSelectedTemplate() As String: LastSelectedTemplate = This.LastSelectedTemplate: End Property`
- Line 174: `Public Property Let LastSelectedTemplate(ByVal Value As String): This.LastSelectedTemplate = Trim$(Value): End Property`
- Line 175: `Public Property Get WindowWidth() As Long: WindowWidth = This.WindowWidth: End Property`
- Line 176: `Public Property Let WindowWidth(ByVal Value As Long): This.WindowWidth = Value: End Property`
- Line 177: `Public Property Get WindowHeight() As Long: WindowHeight = This.WindowHeight: End Property`
- Line 178: `Public Property Let WindowHeight(ByVal Value As Long): This.WindowHeight = Value: End Property`
- Line 179: `Public Property Get ActivePage() As String: ActivePage = This.ActivePage: End Property`
- Line 180: `Public Property Let ActivePage(ByVal Value As String): This.ActivePage = Trim$(Value): End Property`

### `src\Build\Application\AppStudioSettingsIssue.cls`

- Line 30: `Public Sub AppInitialize(ByVal Severity As Long, ByVal Code As String, ByVal Message As String, ByVal SettingKey As String, ByVal SortOrder As Long)`
- Line 41: `Public Property Get Severity() As Long`
- Line 46: `Public Property Get SeverityText() As String`
- Line 56: `Public Property Get Code() As String`
- Line 61: `Public Property Get Message() As String`
- Line 66: `Public Property Get SettingKey() As String`
- Line 71: `Public Property Get SortOrder() As Long`

### `src\Build\Application\AppStudioSettingsService.cls`

- Line 28: `Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider, Optional ByVal SettingsFilePath As String = vbNullString)`
- Line 36: `Public Function AppCreateDefaultSettings() As AppStudioSettings`
- Line 43: `Public Function AppGetSettingsFilePath() As String`
- Line 56: `Public Function AppLoadSettings(ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 84: `Public Function AppLoadSettingsFromPath(ByVal SettingsPath As String, ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 106: `Public Function AppValidateSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 119: `Public Function AppSaveSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult`
- Line 173: `Public Function AppRestoreDefaults() As AppStudioSettings`
- Line 177: `Public Function AppResolvePath(ByVal BasePath As String, ByVal PathValue As String) As String`
- Line 194: `Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean`

### `src\Build\Application\AppTemplateModel.cls`

- Line 33: `Public Sub AppInitialize( _`
- Line 50: `Public Property Get TemplateName() As String`
- Line 54: `Public Property Let TemplateName(ByVal Value As String)`
- Line 59: `Public Property Get TemplateType() As String`
- Line 63: `Public Property Let TemplateType(ByVal Value As String)`
- Line 68: `Public Property Get FilePath() As String`
- Line 72: `Public Property Let FilePath(ByVal Value As String)`
- Line 76: `Public Property Get Content() As String`
- Line 80: `Public Property Let Content(ByVal Value As String)`
- Line 85: `Public Property Get IsDirty() As Boolean`
- Line 89: `Public Property Let IsDirty(ByVal Value As Boolean)`
- Line 93: `Public Property Get HasValidated() As Boolean`
- Line 97: `Public Property Let HasValidated(ByVal Value As Boolean)`
- Line 101: `Public Property Get Placeholders() As Collection`
- Line 105: `Public Property Set Placeholders(ByVal Value As Collection)`
- Line 109: `Public Property Get Sections() As Collection`
- Line 113: `Public Property Set Sections(ByVal Value As Collection)`
- Line 117: `Public Property Get ValidationResults() As Collection`
- Line 121: `Public Property Set ValidationResults(ByVal Value As Collection)`

### `src\Build\Application\AppTemplateService.cls`

- Line 28: `Public Sub AppInitialize( _`
- Line 50: `Public Function AppLoadTemplates(ByVal ManifestPath As String, ByVal Modules As Collection, ByRef Templates As Collection) As ComResult`
- Line 92: `Public Function AppLoadTemplateFile(ByVal FilePath As String, ByRef Template As AppTemplateModel) As ComResult`
- Line 104: `Public Function AppCreateTemplateModel( _`
- Line 118: `Public Function AppAnalyzeTemplate(ByVal Template As AppTemplateModel) As ComResult`
- Line 128: `Public Function AppValidateTemplate(ByVal Template As AppTemplateModel, ByRef Issues As Collection) As ComResult`
- Line 147: `Public Function AppSaveTemplate(ByVal Template As AppTemplateModel) As ComResult`
- Line 190: `Public Function AppSaveTemplateAs( _`
- Line 221: `Public Function AppPreviewTemplateWithModule( _`
- Line 258: `Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean`
- Line 276: `Public Function AppDefaultTemplateFilePath(ByVal TemplateFileName As String) As String`

### `src\Build\Application\AppTemplateValidationIssue.cls`

- Line 32: `Public Sub AppInitialize( _`
- Line 54: `Public Property Get Severity() As Long`
- Line 59: `Public Property Get SeverityText() As String`
- Line 73: `Public Property Get Code() As String`
- Line 78: `Public Property Get Message() As String`
- Line 83: `Public Property Get TemplateName() As String`
- Line 88: `Public Property Get ItemName() As String`
- Line 93: `Public Property Get LineNumber() As Long`
- Line 98: `Public Property Get SortOrder() As Long`

### `src\Build\Application\AppValidationService.cls`

- Line 24: `Public Function AppValidateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As ComResult`

### `src\Build\Application\AttributeWriter.cls`

- Line 20: `Public Sub WriteAttributes(ByVal Component As Object, ByVal ModuleName As String)`

### `src\Build\Application\Build_BlueprintParser.cls`

- Line 37: `Public Function BuildInitializeFromContent(ByVal BlueprintContent As String) As ComResult`
- Line 64: `Public Function BuildHasLayer(ByVal LayerName As String) As Boolean`
- Line 71: `Public Function BuildGetLayerItems(ByVal LayerName As String) As Collection`
- Line 82: `Public Function BuildGetBlueprintValue(ByVal KeyName As String) As String`
- Line 93: `Public Function BuildGenerateManifestContent(ByVal LayerName As String) As String`

### `src\Build\Application\Build_ProjectManifest.cls`

- Line 46: `Public Function BuildInitializeFromContent(ByVal ManifestContent As String) As ComResult`
- Line 77: `Public Function BuildGetLayerItems(ByVal LayerName As String) As Collection`
- Line 88: `Public Function BuildHasLayer(ByVal LayerName As String) As Boolean`
- Line 95: `Public Function BuildIsGenerationEnabled(ByVal LayerName As String) As Boolean`
- Line 106: `Public Function BuildGetSourceRoot() As String`

### `src\Build\Application\BuildGenerationEngine.cls`

- Line 27: `Public Sub AppInitialize( _`
- Line 68: `Public Function GenerateComponent( _`

### `src\Build\Application\ComponentFactory.cls`

- Line 23: `Public Function CreateComponent(ByVal VBProject As Object, ByVal ModuleType As String) As Object`

### `src\Build\Application\HeaderNormalizer.cls`

- Line 20: `Public Function NormalizeSource(ByVal CodeText As String) As String`

### `src\Build\Application\ProjectProvider.cls`

- Line 22: `Public Sub AppInitialize(ByVal Provider As InfVbaProjectProvider)`
- Line 31: `Public Function ResolveTargetProject() As Object`
- Line 40: `Public Function ModuleExists(ByVal VBProject As Object, ByVal ModuleName As String) As Boolean`
- Line 49: `Public Sub RemoveComponent(ByVal VBProject As Object, ByVal ModuleName As String)`
- Line 58: `Public Function GetModuleText(ByVal VBProject As Object, ByVal ModuleName As String) As String`

### `src\Build\Application\SourceWriter.cls`

- Line 20: `Public Sub WriteSource(ByVal Component As Object, ByVal CodeText As String)`

### `src\Build\Application\VerificationService.cls`

- Line 20: `Public Function VerifyResult( _`

### `src\Build\Common\BuildPathResolver.bas`

- Line 15: `Public Function RepositoryRootPath() As String`
- Line 87: `Public Function TemplatesDirectoryPath() As String`
- Line 91: `Public Function ApplicationsDirectoryPath() As String`
- Line 95: `Public Function ApplicationDirectoryPath(ByVal applicationName As String) As String`
- Line 100: `Public Function ManifestFilePath(ByVal applicationName As String) As String`
- Line 115: `Public Function CandidatesDirectoryPath() As String`
- Line 119: `Public Function SpecsDirectoryPath() As String`
- Line 123: `Public Function CombinePath(ByVal basePath As String, ByVal childPath As String) As String`

### `src\Build\Common\ComCompositionRoot.cls`

- Line 33: `Public Function ComInitialize() As Boolean`
- Line 44: `Public Sub ComShutdown()`

### `src\Build\Common\ComError.bas`

- Line 15: `Public Enum ComErrNum`

### `src\Build\Common\ComErrorInfo.cls`

- Line 55: `Public Sub ComInitializeErrorInfo( _`
- Line 75: `Public Property Get Number() As Long`
- Line 81: `Public Property Get Source() As String`
- Line 87: `Public Property Get Description() As String`
- Line 93: `Public Property Get Severity() As String`
- Line 99: `Public Property Get IsRecoverable() As Boolean`
- Line 105: `Public Function ToText() As String`

### `src\Build\Common\ComFacade.bas`

- Line 35: `Public Function ComCreateErrorInfo( _`
- Line 62: `Public Sub ComRaiseError(ByVal ErrorInfo As ComErrorInfo)`
- Line 80: `Public Function ComCreateSuccess(Optional ByVal ResultMessage As String = vbNullString) As ComResult`
- Line 99: `Public Function ComCreateFailure(ByVal Failure As ComErrorInfo) As ComResult`
- Line 121: `Public Function ComCreateFailureFromErr( _`
- Line 157: `Public Function ComIsBlankText(ByVal TextValue As String) As Boolean`
- Line 170: `Public Sub ComRequireText( _`
- Line 198: `Public Function ComTrimText(ByVal TextValue As String) As String`

### `src\Build\Common\ComResult.cls`

- Line 47: `Public Sub ComInitializeSuccess(Optional ByVal ResultMessage As String = vbNullString)`
- Line 61: `Public Sub ComInitializeFailure(ByVal Failure As ComErrorInfo)`
- Line 73: `Public Property Get IsSuccess() As Boolean`
- Line 79: `Public Property Get IsFailure() As Boolean`
- Line 85: `Public Property Get Message() As String`
- Line 94: `Public Property Get Failure() As ComErrorInfo`

### `src\Build\Domain\DomCompositionRoot.cls`

- Line 24: `Public Function DomInitialize() As Boolean`
- Line 29: `Public Sub DomShutdown()`
- Line 33: `Public Function DomCreateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As DomProject`
- Line 43: `Public Function DomCreateModuleInfo( _`
- Line 57: `Public Function DomCreateManifest(ByVal Project As DomProject) As DomManifest`
- Line 67: `Public Function DomCreateValidator() As DomValidator`

### `src\Build\Domain\DomFacade.bas`

- Line 16: `Public Function DomInitialize() As Boolean`
- Line 24: `Public Sub DomShutdown()`
- Line 32: `Public Function DomCreateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As DomProject`
- Line 40: `Public Function DomCreateModuleInfo( _`
- Line 52: `Public Function DomCreateManifest(ByVal Project As DomProject) As DomManifest`
- Line 60: `Public Function DomValidateProject(ByVal Project As DomProject) As ComResult`
- Line 68: `Public Function DomValidateModuleInfo(ByVal ModuleInfo As DomModuleInfo) As ComResult`
- Line 76: `Public Function DomValidateManifest(ByVal Manifest As DomManifest) As ComResult`

### `src\Build\Domain\DomManifest.cls`

- Line 39: `Public Sub DomInitializeManifest(ByVal Project As DomProject)`
- Line 53: `Public Property Get Project() As DomProject`
- Line 59: `Public Property Get IsInitialized() As Boolean`

### `src\Build\Domain\DomModuleInfo.cls`

- Line 41: `Public Sub DomInitializeModuleInfo( _`
- Line 57: `Public Property Get Name() As String`
- Line 63: `Public Property Get Layer() As String`
- Line 69: `Public Property Get Path() As String`
- Line 75: `Public Property Get IsInitialized() As Boolean`

### `src\Build\Domain\DomProject.cls`

- Line 40: `Public Sub DomInitializeProject(ByVal ProjectName As String, ByVal ProjectRootPath As String)`
- Line 50: `Public Property Get Name() As String`
- Line 56: `Public Property Get RootPath() As String`
- Line 62: `Public Property Get IsInitialized() As Boolean`

### `src\Build\Domain\DomValidator.cls`

- Line 33: `Public Function DomValidateProject(ByVal Project As DomProject) As ComResult`
- Line 58: `Public Function DomValidateModuleInfo(ByVal ModuleInfo As DomModuleInfo) As ComResult`
- Line 88: `Public Function DomValidateManifest(ByVal Manifest As DomManifest) As ComResult`

### `src\Build\Infrastructure\InfClassCodeGenerator.cls`

- Line 34: `Public Function InfRenderClassMemberSections(ByVal MemberDefinitionText As String) As Object`

### `src\Build\Infrastructure\InfCompositionRoot.cls`

- Line 33: `Public Function InfInitialize() As Boolean`
- Line 44: `Public Sub InfShutdown()`
- Line 57: `Public Function InfCreateFileSystemProvider() As InfFileSystemProvider`
- Line 71: `Public Function InfCreateWorkbookProvider() As InfWorkbookProvider`
- Line 76: `Public Function InfCreateVbaProjectProvider() As InfVbaProjectProvider`
- Line 81: `Public Function InfCreateTemplateProvider() As InfTemplateProvider`
- Line 91: `Public Function InfCreateManifestProvider() As InfManifestProvider`
- Line 101: `Public Function InfCreateTokenReplacer() As InfTokenReplacer`
- Line 106: `Public Function InfCreateGenerator() As InfGenerator`

### `src\Build\Infrastructure\InfFacade.bas`

- Line 26: `Public Function InfInitialize() As Boolean`
- Line 40: `Public Sub InfShutdown()`
- Line 57: `Public Function InfFileExists(ByVal FilePath As String) As Boolean`
- Line 74: `Public Function InfFolderExists(ByVal FolderPath As String) As Boolean`
- Line 91: `Public Function InfEnsureFolder(ByVal FolderPath As String) As ComResult`
- Line 108: `Public Function InfReadText(ByVal FilePath As String) As String`
- Line 126: `Public Function InfWriteText(ByVal FilePath As String, ByVal TextContent As String) As ComResult`
- Line 143: `Public Function InfGetWorkbookPath(ByVal Workbook As Object) As String`
- Line 151: `Public Function InfCreateGenerator() As InfGenerator`
- Line 156: `Public Function InfCreateManifestProvider() As InfManifestProvider`
- Line 161: `Public Function InfValidateTemplateFile(ByVal TemplatePath As String) As ComResult`
- Line 169: `Public Function InfValidateManifestFile(ByVal ManifestPath As String) As ComResult`
- Line 177: `Public Function InfCreateVbaProjectProvider() As InfVbaProjectProvider`

### `src\Build\Infrastructure\InfFileSystemProvider.cls`

- Line 42: `Public Function InfFileExists(ByVal FilePath As String) As Boolean`
- Line 58: `Public Function InfFolderExists(ByVal FolderPath As String) As Boolean`
- Line 74: `Public Function InfEnsureFolder(ByVal FolderPath As String) As ComResult`
- Line 95: `Public Function InfReadText(ByVal FilePath As String) As String`
- Line 122: `Public Function InfWriteText(ByVal FilePath As String, ByVal TextContent As String) As ComResult`

### `src\Build\Infrastructure\InfGenerator.cls`

- Line 36: `Public Sub InfInitialize(ByVal Provider As InfTemplateProvider, ByVal ProviderManifest As InfManifestProvider, ByVal Replacer As InfTokenReplacer)`
- Line 55: `Public Function InfGenerateStandardModule(ByVal ModuleName As String, ByVal LayerName As String) As String`
- Line 67: `Public Function InfGenerateStandardClass(ByVal ClassName As String, ByVal LayerName As String) As String`
- Line 79: `Public Function InfGenerateModule(ByVal ModuleName As String, ByVal TemplatePath As String, Optional ByVal LayerName As String = "Application") As String`
- Line 115: `Public Function InfGenerateManifestItem(ByVal Item As ManifestItem) As String`
- Line 158: `Public Function InfGenerateManifestItemFromTemplateText( _`

### `src\Build\Infrastructure\InfManifestProvider.cls`

- Line 22: `Public Sub InfInitialize(ByVal Provider As InfFileSystemProvider)`
- Line 31: `Public Function InfCreateManifest( _`
- Line 53: `Public Function InfLoadManifestItems(ByVal ManifestPath As String) As Collection`
- Line 89: `Public Function InfValidateManifestFile(ByVal ManifestPath As String) As ComResult`
- Line 110: `Public Function InfLoadBodySource(ByVal BodySourcePath As String) As String`
- Line 125: `Public Function InfLoadSectionSource(ByVal SectionSourcePath As String) As String`
- Line 140: `Public Function InfLoadMemberSource(ByVal MemberSourcePath As String) As String`

### `src\Build\Infrastructure\InfTemplate.cls`

- Line 27: `Public Sub InfSetTemplateName(ByVal TemplateName As String)`
- Line 33: `Public Sub InfSetTemplateText(ByVal TemplateText As String)`
- Line 42: `Public Function InfGetTemplateName() As String`
- Line 51: `Public Function InfGetTemplateText() As String`
- Line 60: `Public Function InfHasContent() As Boolean`
- Line 65: `Public Function InfValidate() As Boolean`

### `src\Build\Infrastructure\InfTemplateProvider.cls`

- Line 22: `Public Sub InfInitialize(ByVal Provider As InfFileSystemProvider)`
- Line 31: `Public Function InfCreateTemplate(ByVal TemplatePath As String) As InfTemplate`
- Line 55: `Public Function InfValidateTemplateFile(ByVal TemplatePath As String) As ComResult`
- Line 71: `Public Function InfCreateTemplateFromText(ByVal TemplateName As String, ByVal TemplateText As String) As InfTemplate`
- Line 84: `Public Function InfGetModuleTemplatePath() As String`
- Line 91: `Public Function InfGetClassTemplatePath() As String`

### `src\Build\Infrastructure\InfTokenReplacer.cls`

- Line 20: `Public Function InfReplaceTokens(ByVal Template As InfTemplate, ByVal Manifest As Object) As String`

### `src\Build\Infrastructure\InfVbaProjectProvider.cls`

- Line 25: `Public Function InfAddStandardModule(ByVal ModuleName As String, ByVal CodeText As String) As ComResult`
- Line 68: `Public Function InfAddClassModule(ByVal ClassName As String, ByVal CodeText As String) As ComResult`
- Line 111: `Public Function InfModuleExists(ByVal ModuleName As String) As Boolean`
- Line 117: `Public Function InfRemoveModule(ByVal ModuleName As String) As ComResult`
- Line 142: `Public Function InfGetModuleText(ByVal ModuleName As String) As String`
- Line 158: `Public Function InfResolveTargetVBProject() As Object`
- Line 163: `Public Function InfModuleExistsInProject(ByVal VBProj As Object, ByVal ModuleName As String) As Boolean`
- Line 173: `Public Sub InfRemoveComponentFromProject(ByVal VBProj As Object, ByVal ModuleName As String)`
- Line 191: `Public Function InfGetModuleTextFromProject(ByVal VBProj As Object, ByVal ModuleName As String) As String`

### `src\Build\Infrastructure\InfWorkbookProvider.cls`

- Line 33: `Public Function InfGetWorkbookPath(ByVal Workbook As Object) As String`

### `src\Build\Infrastructure\ManifestItem.cls`

- Line 36: `Public Sub InfInitialize( _`
- Line 63: `Public Sub InfAddSectionSourcePath(ByVal SectionName As String, ByVal SectionSourcePath As String)`
- Line 81: `Public Sub InfSetMemberSourcePath(ByVal MemberSourcePath As String)`
- Line 89: `Public Sub InfSetMemberSourceText(ByVal MemberSourceText As String)`
- Line 96: `Public Function InfGetModuleName() As String`
- Line 102: `Public Function InfGetModuleType() As String`
- Line 108: `Public Function InfGetLayerName() As String`
- Line 114: `Public Function InfGetTemplatePath() As String`
- Line 120: `Public Function InfGetBodySourcePath() As String`
- Line 126: `Public Function InfGetMemberSourcePath() As String`
- Line 132: `Public Function InfGetMemberSourceText() As String`
- Line 138: `Public Function InfGetSectionSourcePaths() As Object`
- Line 149: `Public Function InfToManifest() As Object`
- Line 170: `Public Function InfValidate() As Boolean`

### `src\Build\Presentation\PreCompositionRoot.cls`

- Line 24: `Public Function PreInitialize() As Boolean`
- Line 29: `Public Sub PreShutdown()`
- Line 34: `Public Function PreCreateNotificationPresenter() As PreNotificationPresenter`
- Line 39: `Public Function PreCreateManifestEditorForm() As PreManifestEditorForm`

### `src\Build\Presentation\PreFacade.bas`

- Line 16: `Public Function PreInitialize() As Boolean`
- Line 24: `Public Sub PreShutdown()`
- Line 32: `Public Function PreShowValidationResult(ByVal Result As ComResult) As String`
- Line 40: `Public Function PreShowBuildResult(ByVal Result As ComResult) As String`
- Line 48: `Public Function PreShowGenerateModuleResult(ByVal Result As ComResult) As String`
- Line 56: `Public Function PreGenerateModule() As ComResult`
- Line 71: `Public Function PreGenerateModuleMessage() As String`
- Line 79: `Public Sub PreOpenManifestEditor(ByVal ManifestPath As String)`

### `src\Build\Presentation\PreInputDialog.bas`

- Line 12: `Public Function PreRequestModuleName() As String`

### `src\Build\Presentation\PreManifestEditorForm.frm`

- Line 103: `Public Sub PreOpenManifest(ByVal ManifestPath As String)`

### `src\Build\Presentation\PreNotificationPresenter.cls`

- Line 32: `Public Function PreShowValidationResult(ByVal Result As ComResult) As String`
- Line 37: `Public Function PreShowBuildResult(ByVal Result As ComResult) As String`
- Line 42: `Public Function PreShowGenerateModuleResult(ByVal Result As ComResult) As String`

### `src\Build\Presentation\PreRibbon.bas`

- Line 16: `Public Function PreRibbonLoaded() As ComResult`

