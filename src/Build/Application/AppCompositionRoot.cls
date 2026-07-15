VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppCompositionRoot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppCompositionRoot
' Layer: Application
' Responsibility: Build composition boundary.
' Dependencies: Common, Domain, Infrastructure, Application
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Build services.
Public Function AppInitialize() As Boolean
    AppInitialize = True
End Function

' Shuts down Build services.
Public Sub AppShutdown()
End Sub

' Creates a build service.
Public Function AppCreateBuildService() As AppBuildService
    Set AppCreateBuildService = New AppBuildService
End Function

' Creates an export service.
Public Function AppCreateExportService() As AppExportService
    Set AppCreateExportService = New AppExportService
End Function

' Creates an import service.
Public Function AppCreateImportService() As AppImportService
    Set AppCreateImportService = New AppImportService
End Function

' Creates a validation service.
Public Function AppCreateValidationService() As AppValidationService
    Set AppCreateValidationService = New AppValidationService
End Function

' Creates a generator service.
Public Function AppCreateGeneratorService() As AppGeneratorService
    Dim GeneratorService As AppGeneratorService

    Set GeneratorService = New AppGeneratorService
    GeneratorService.AppInitialize _
        InfCreateGenerator(), _
        AppCreateGenerationEngine(), _
        InfCreateManifestProvider(), _
        AppCreateManifestValidationService(), _
        AppCreateManifestEditorService()

    Set AppCreateGeneratorService = GeneratorService
End Function

' Creates a manifest editor service.
Public Function AppCreateManifestEditorService() As AppManifestEditorService
    Set AppCreateManifestEditorService = New AppManifestEditorService
End Function

' Creates a manifest validation service.
Public Function AppCreateManifestValidationService() As AppManifestValidationService
    Set AppCreateManifestValidationService = New AppManifestValidationService
End Function

' Creates a code preview service.
Public Function AppCreateCodePreviewService() As AppCodePreviewService
    Dim PreviewService As AppCodePreviewService

    Set PreviewService = New AppCodePreviewService
    PreviewService.AppInitialize InfCreateGenerator(), AppCreateManifestEditorService(), AppCreateManifestValidationService()

    Set AppCreateCodePreviewService = PreviewService
End Function

' Creates a manifest generate service.
Public Function AppCreateManifestGenerateService() As AppManifestGenerateService
    Dim GenerateService As AppManifestGenerateService

    Set GenerateService = New AppManifestGenerateService
    GenerateService.AppInitialize _
        InfCreateGenerator(), _
        AppCreateManifestEditorService(), _
        AppCreateManifestValidationService(), _
        New InfFileSystemProvider, _
        AppCreateStudioSettingsService()

    Set AppCreateManifestGenerateService = GenerateService
End Function

' Creates a build log service.
Public Function AppCreateBuildLogService() As AppBuildLogService
    Set AppCreateBuildLogService = New AppBuildLogService
End Function

' Creates a template manager service.
Public Function AppCreateTemplateService() As AppTemplateService
    Dim TemplateService As AppTemplateService

    Set TemplateService = New AppTemplateService
    TemplateService.AppInitialize InfCreateGenerator(), AppCreateManifestEditorService(), New InfFileSystemProvider, AppCreateStudioSettingsService()

    Set AppCreateTemplateService = TemplateService
End Function

' Creates a Studio settings service.
Public Function AppCreateStudioSettingsService() As AppStudioSettingsService
    Dim SettingsService As AppStudioSettingsService

    Set SettingsService = New AppStudioSettingsService
    SettingsService.AppInitialize New InfFileSystemProvider

    Set AppCreateStudioSettingsService = SettingsService
End Function

' Creates the Build generation engine.
Public Function AppCreateGenerationEngine() As BuildGenerationEngine
    Dim Engine As BuildGenerationEngine
    Dim Factory As ComponentFactory
    Dim Normalizer As HeaderNormalizer
    Dim AttributeService As AttributeWriter
    Dim SourceService As SourceWriter
    Dim Verifier As VerificationService

    Set Engine = New BuildGenerationEngine
    Set Factory = New ComponentFactory
    Set Normalizer = New HeaderNormalizer
    Set AttributeService = New AttributeWriter
    Set SourceService = New SourceWriter
    Set Verifier = New VerificationService

    Engine.AppInitialize _
        AppCreateProjectProvider(), _
        Factory, _
        Normalizer, _
        AttributeService, _
        SourceService, _
        Verifier

    Set AppCreateGenerationEngine = Engine
End Function

' Creates a project provider for generation.
Public Function AppCreateProjectProvider() As ProjectProvider
    Dim Provider As ProjectProvider

    Set Provider = New ProjectProvider
    Provider.AppInitialize InfCreateVbaProjectProvider()

    Set AppCreateProjectProvider = Provider
End Function
