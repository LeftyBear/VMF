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

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateCompositionRoot() As AppCompositionRoot
    Set CreateCompositionRoot = New AppCompositionRoot
End Function
