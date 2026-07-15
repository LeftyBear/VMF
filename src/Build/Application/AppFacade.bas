Option Explicit
Attribute VB_Name = "AppFacade"

'=========================================================================
' Module: AppFacade
' Layer: Application
' Responsibility: Published entry point for Build capabilities.
' Dependencies: Common, Domain, Infrastructure, Application
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Build services.
Public Function AppInitialize() As Boolean
    Dim CompositionRoot As AppCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    AppInitialize = CompositionRoot.AppInitialize()
End Function

' Shuts down Build services.
Public Sub AppShutdown()
    Dim CompositionRoot As AppCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    CompositionRoot.AppShutdown
End Sub

' Validates a project.
Public Function AppValidateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As ComResult
    Dim ValidationService As AppValidationService

    Set ValidationService = CreateCompositionRoot().AppCreateValidationService()
    Set AppValidateProject = ValidationService.AppValidateProject(ProjectName, ProjectRootPath)
End Function

' Exports a project.
Public Function AppExportProject( _
    ByVal ProjectName As String, _
    ByVal ProjectRootPath As String, _
    ByVal OutputFilePath As String) As ComResult

    Dim ExportService As AppExportService
    Dim ValidationService As AppValidationService

    Set ExportService = CreateCompositionRoot().AppCreateExportService()
    Set ValidationService = CreateCompositionRoot().AppCreateValidationService()
    Set AppExportProject = ExportService.AppExportProject(ValidationService, ProjectName, ProjectRootPath, OutputFilePath)
End Function

' Imports a project.
Public Function AppImportProject(ByVal InputFilePath As String) As ComResult
    Dim ImportService As AppImportService

    Set ImportService = CreateCompositionRoot().AppCreateImportService()
    Set AppImportProject = ImportService.AppImportProject(InputFilePath)
End Function

' Builds a project.
Public Function AppBuildProject( _
    ByVal ProjectName As String, _
    ByVal ProjectRootPath As String, _
    ByVal OutputFilePath As String) As ComResult

    Dim BuildService As AppBuildService
    Dim ExportService As AppExportService
    Dim ValidationService As AppValidationService

    Set BuildService = CreateCompositionRoot().AppCreateBuildService()
    Set ExportService = CreateCompositionRoot().AppCreateExportService()
    Set ValidationService = CreateCompositionRoot().AppCreateValidationService()
    Set AppBuildProject = BuildService.AppBuildProject(ExportService, ValidationService, ProjectName, ProjectRootPath, OutputFilePath)
End Function

' Generates a standard module in the active workbook.
Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateModule = GeneratorService.AppGenerateModule(ModuleName)
End Function

' Generates a class module in the active workbook.
Public Function AppGenerateClass(ByVal ClassName As String) As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateClass = GeneratorService.AppGenerateClass(ClassName)
End Function

' Generates the Common layer in the active workbook from Common.manifest.
Public Function AppGenerateCommon() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateCommon = GeneratorService.AppGenerateCommon()
End Function

' Generates the Manifest layer in the active workbook from Manifest.manifest.
Public Function AppGenerateManifest() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateManifest = GeneratorService.AppGenerateManifest()
End Function

' Generates the Infrastructure layer in the active workbook from Infrastructure.manifest.
Public Function AppGenerateInfrastructure() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateInfrastructure = GeneratorService.AppGenerateInfrastructure()
End Function

' Generates the Domain layer in the active workbook from Domain.manifest.
Public Function AppGenerateDomain() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateDomain = GeneratorService.AppGenerateDomain()
End Function

' Generates the Application layer in the active workbook from Application.manifest.
Public Function AppGenerateApplication() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateApplication = GeneratorService.AppGenerateApplication()
End Function

' Generates the Presentation layer in the active workbook from Presentation.manifest.
Public Function AppGeneratePresentation() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGeneratePresentation = GeneratorService.AppGeneratePresentation()
End Function

' Generates all Build layers in the active workbook.
Public Function AppGenerateProject() As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppGenerateProject = GeneratorService.AppGenerateProject()
End Function

' Previews a Build layer without generating components.
Public Function AppPreviewBuildLayer(ByVal LayerName As String, ByRef PreviewText As String) As ComResult
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = CreateCompositionRoot().AppCreateGeneratorService()
    Set AppPreviewBuildLayer = GeneratorService.AppPreviewBuildLayer(LayerName, PreviewText)
End Function

' Loads an editable Build manifest model.
Public Function AppLoadManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByRef Modules As Collection) As ComResult

    Dim ManifestEditorService As AppManifestEditorService

    Set ManifestEditorService = CreateCompositionRoot().AppCreateManifestEditorService()
    Set AppLoadManifestEditorModel = ManifestEditorService.AppLoadManifestEditorModel(ManifestPath, Modules)
End Function

' Saves an editable Build manifest model.
Public Function AppSaveManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection) As ComResult

    Dim ManifestEditorService As AppManifestEditorService

    Set ManifestEditorService = CreateCompositionRoot().AppCreateManifestEditorService()
    Set AppSaveManifestEditorModel = ManifestEditorService.AppSaveManifestEditorModel(ManifestPath, Modules)
End Function

' Creates an editable manifest module model.
Public Function AppCreateManifestEditorModule( _
    ByVal ModuleName As String, _
    ByVal LayerName As String, _
    ByVal ModuleType As String, _
    Optional ByVal TemplatePath As String = vbNullString) As Object

    Dim ManifestEditorService As AppManifestEditorService

    Set ManifestEditorService = CreateCompositionRoot().AppCreateManifestEditorService()
    Set AppCreateManifestEditorModule = ManifestEditorService.AppCreateManifestEditorModule(ModuleName, LayerName, ModuleType, TemplatePath)
End Function

' Creates an editable manifest member model.
Public Function AppCreateManifestEditorMember( _
    ByVal MemberName As String, _
    ByVal TypeName As String, _
    ByVal Accessor As String, _
    ByVal InitialValue As String, _
    ByVal CreateInstance As Boolean) As Object

    Dim ManifestEditorService As AppManifestEditorService

    Set ManifestEditorService = CreateCompositionRoot().AppCreateManifestEditorService()
    Set AppCreateManifestEditorMember = ManifestEditorService.AppCreateManifestEditorMember( _
        MemberName, _
        TypeName, _
        Accessor, _
        InitialValue, _
        CreateInstance)
End Function

' Previews generated code from one unsaved Manifest Editor module.
Public Function AppPreviewManifestEditorModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByRef PreviewText As String) As ComResult

    Dim PreviewService As AppCodePreviewService

    Set PreviewService = CreateCompositionRoot().AppCreateCodePreviewService()
    Set AppPreviewManifestEditorModule = PreviewService.AppPreviewManifestEditorModule(ManifestPath, ModuleInfo, PreviewText)
End Function

' Validates an editable Build manifest model.
Public Function AppValidateManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByRef Issues As Collection) As ComResult

    Dim ValidationService As AppManifestValidationService

    Set ValidationService = CreateCompositionRoot().AppCreateManifestValidationService()
    Set AppValidateManifestEditorModel = ValidationService.AppValidateManifestEditorModel(ManifestPath, Modules, Issues)
End Function

' Validates one editable Build manifest module.
Public Function AppValidateManifestEditorModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByRef Issues As Collection) As ComResult

    Dim ValidationService As AppManifestValidationService

    Set ValidationService = CreateCompositionRoot().AppCreateManifestValidationService()
    Set AppValidateManifestEditorModule = ValidationService.AppValidateManifestEditorModule(ManifestPath, ModuleInfo, Issues)
End Function

' Returns True when validation issues contain at least one Error.
Public Function AppValidationIssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim ValidationService As AppManifestValidationService

    Set ValidationService = CreateCompositionRoot().AppCreateManifestValidationService()
    AppValidationIssuesContainErrors = ValidationService.AppIssuesContainErrors(Issues)
End Function

' Creates a Manifest Editor generate request.
Public Function AppCreateManifestGenerateRequest( _
    ByVal TargetScope As String, _
    ByVal SelectedModuleNames As Collection, _
    ByVal OutputDirectory As String, _
    ByVal OverwriteMode As String, _
    ByVal ContinueOnError As Boolean) As AppGenerateRequest

    Dim GenerateService As AppManifestGenerateService

    Set GenerateService = CreateCompositionRoot().AppCreateManifestGenerateService()
    Set AppCreateManifestGenerateRequest = GenerateService.AppCreateGenerateRequest( _
        TargetScope, _
        SelectedModuleNames, _
        OutputDirectory, _
        OverwriteMode, _
        ContinueOnError)
End Function

' Generates source files from an editable Manifest Editor model.
Public Function AppGenerateManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByVal Request As AppGenerateRequest, _
    ByRef GenerateResult As AppGenerateResult) As ComResult

    Dim GenerateService As AppManifestGenerateService

    Set GenerateService = CreateCompositionRoot().AppCreateManifestGenerateService()
    Set AppGenerateManifestEditorModel = GenerateService.AppGenerateManifestEditorModel( _
        ManifestPath, _
        Modules, _
        Request, _
        GenerateResult)
End Function

' Renders a build log from structured generate results.
Public Function AppRenderBuildLog(ByVal GenerateResult As AppGenerateResult) As String
    Dim BuildLogService As AppBuildLogService

    Set BuildLogService = CreateCompositionRoot().AppCreateBuildLogService()
    AppRenderBuildLog = BuildLogService.AppRenderBuildLog(GenerateResult)
End Function

' Resolves the default Manifest Editor output directory.
Public Function AppDefaultManifestGenerateOutputDirectory(ByVal ManifestPath As String) As String
    Dim GenerateService As AppManifestGenerateService

    Set GenerateService = CreateCompositionRoot().AppCreateManifestGenerateService()
    AppDefaultManifestGenerateOutputDirectory = GenerateService.AppDefaultOutputDirectory(ManifestPath)
End Function

' Loads templates available to Template Manager.
Public Function AppLoadTemplateManagerTemplates( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByRef Templates As Collection) As ComResult

    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppLoadTemplateManagerTemplates = TemplateService.AppLoadTemplates(ManifestPath, Modules, Templates)
End Function

' Loads one template file for Template Manager.
Public Function AppLoadTemplateManagerTemplateFile( _
    ByVal FilePath As String, _
    ByRef Template As AppTemplateModel) As ComResult

    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppLoadTemplateManagerTemplateFile = TemplateService.AppLoadTemplateFile(FilePath, Template)
End Function

' Creates a new editable Template Manager model.
Public Function AppCreateTemplateManagerTemplate( _
    ByVal TemplateName As String, _
    ByVal TemplateType As String, _
    ByVal FilePath As String, _
    ByVal Content As String) As AppTemplateModel

    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppCreateTemplateManagerTemplate = TemplateService.AppCreateTemplateModel(TemplateName, TemplateType, FilePath, Content)
End Function

' Resolves the default template file path from Studio settings.
Public Function AppDefaultTemplateManagerFilePath(ByVal TemplateFileName As String) As String
    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    AppDefaultTemplateManagerFilePath = TemplateService.AppDefaultTemplateFilePath(TemplateFileName)
End Function

' Analyzes one template model.
Public Function AppAnalyzeTemplateModel(ByVal Template As AppTemplateModel) As ComResult
    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppAnalyzeTemplateModel = TemplateService.AppAnalyzeTemplate(Template)
End Function

' Validates one template model.
Public Function AppValidateTemplateModel(ByVal Template As AppTemplateModel, ByRef Issues As Collection) As ComResult
    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppValidateTemplateModel = TemplateService.AppValidateTemplate(Template, Issues)
End Function

' Saves a template model after validation and verification.
Public Function AppSaveTemplateModel(ByVal Template As AppTemplateModel) As ComResult
    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppSaveTemplateModel = TemplateService.AppSaveTemplate(Template)
End Function

' Saves a template model with a new name and path.
Public Function AppSaveTemplateModelAs( _
    ByVal Template As AppTemplateModel, _
    ByVal TemplateName As String, _
    ByVal FilePath As String) As ComResult

    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppSaveTemplateModelAs = TemplateService.AppSaveTemplateAs(Template, TemplateName, FilePath)
End Function

' Previews code using the unsaved Template Manager content and selected module.
Public Function AppPreviewTemplateModelWithModule( _
    ByVal ManifestPath As String, _
    ByVal Template As AppTemplateModel, _
    ByVal ModuleInfo As Object, _
    ByRef PreviewText As String) As ComResult

    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    Set AppPreviewTemplateModelWithModule = TemplateService.AppPreviewTemplateWithModule(ManifestPath, Template, ModuleInfo, PreviewText)
End Function

' Returns True when template validation issues contain at least one Error.
Public Function AppTemplateIssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim TemplateService As AppTemplateService

    Set TemplateService = CreateCompositionRoot().AppCreateTemplateService()
    AppTemplateIssuesContainErrors = TemplateService.AppIssuesContainErrors(Issues)
End Function

' Loads VMF Studio settings.
Public Function AppLoadStudioSettings(ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    Set AppLoadStudioSettings = SettingsService.AppLoadSettings(Settings, Issues)
End Function

' Saves VMF Studio settings.
Public Function AppSaveStudioSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    Set AppSaveStudioSettings = SettingsService.AppSaveSettings(Settings, Issues)
End Function

' Validates VMF Studio settings.
Public Function AppValidateStudioSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    Set AppValidateStudioSettings = SettingsService.AppValidateSettings(Settings, Issues)
End Function

' Creates default VMF Studio settings.
Public Function AppCreateDefaultStudioSettings() As AppStudioSettings
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    Set AppCreateDefaultStudioSettings = SettingsService.AppCreateDefaultSettings()
End Function

' Returns True when Studio settings issues contain at least one Error.
Public Function AppStudioSettingsIssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    AppStudioSettingsIssuesContainErrors = SettingsService.AppIssuesContainErrors(Issues)
End Function

' Resolves a Studio settings path against a base directory.
Public Function AppResolveStudioSettingsPath(ByVal BasePath As String, ByVal PathValue As String) As String
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = CreateCompositionRoot().AppCreateStudioSettingsService()
    AppResolveStudioSettingsPath = SettingsService.AppResolvePath(BasePath, PathValue)
End Function

' Lists available backups.
Public Function AppListBackups(Optional ByVal TargetTypeFilter As String = vbNullString) As Collection
    Set AppListBackups = CreateCompositionRoot().AppCreateBackupService().AppListBackups(TargetTypeFilter)
End Function

' Lists backup and restore history.
Public Function AppListBackupHistory() As Collection
    Set AppListBackupHistory = CreateCompositionRoot().AppCreateBackupService().AppListHistory()
End Function

' Restores one backup after validation.
Public Function AppRestoreBackup(ByVal BackupPath As String) As ComResult
    Set AppRestoreBackup = CreateCompositionRoot().AppCreateBackupService().AppRestoreBackup(BackupPath)
End Function

' Deletes one backup.
Public Function AppDeleteBackup(ByVal BackupPath As String) As ComResult
    Set AppDeleteBackup = CreateCompositionRoot().AppCreateBackupService().AppDeleteBackup(BackupPath)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateCompositionRoot() As AppCompositionRoot
    Set CreateCompositionRoot = New AppCompositionRoot
End Function
