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
    GeneratorService.AppInitialize InfCreateGenerator(), InfCreateVbaProjectProvider()

    Set AppCreateGeneratorService = GeneratorService
End Function
