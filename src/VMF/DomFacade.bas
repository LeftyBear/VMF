Option Explicit
Attribute VB_Name = "DomFacade"

'=========================================================================
' Module: DomFacade
' Layer: Domain
' Responsibility: Published entry point for VMF Core capabilities.
' Dependencies: Common, Domain
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes VMF Core services.
Public Function DomInitialize() As Boolean
    Dim CompositionRoot As DomCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    DomInitialize = CompositionRoot.DomInitialize()
End Function

' Shuts down VMF Core services.
Public Sub DomShutdown()
    Dim CompositionRoot As DomCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    CompositionRoot.DomShutdown
End Sub

' Creates a project model.
Public Function DomCreateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As DomProject
    Dim CompositionRoot As DomCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    Set DomCreateProject = CompositionRoot.DomCreateProject(ProjectName, ProjectRootPath)
End Function

' Creates a module metadata model.
Public Function DomCreateModuleInfo( _
    ByVal ModuleName As String, _
    ByVal LayerName As String, _
    ByVal FilePath As String) As DomModuleInfo

    Dim CompositionRoot As DomCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    Set DomCreateModuleInfo = CompositionRoot.DomCreateModuleInfo(ModuleName, LayerName, FilePath)
End Function

' Creates a manifest model.
Public Function DomCreateManifest(ByVal Project As DomProject) As DomManifest
    Dim CompositionRoot As DomCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    Set DomCreateManifest = CompositionRoot.DomCreateManifest(Project)
End Function

' Validates project metadata.
Public Function DomValidateProject(ByVal Project As DomProject) As ComResult
    Dim Validator As DomValidator

    Set Validator = CreateCompositionRoot().DomCreateValidator()
    Set DomValidateProject = Validator.DomValidateProject(Project)
End Function

' Validates module metadata.
Public Function DomValidateModuleInfo(ByVal ModuleInfo As DomModuleInfo) As ComResult
    Dim Validator As DomValidator

    Set Validator = CreateCompositionRoot().DomCreateValidator()
    Set DomValidateModuleInfo = Validator.DomValidateModuleInfo(ModuleInfo)
End Function

' Validates manifest metadata.
Public Function DomValidateManifest(ByVal Manifest As DomManifest) As ComResult
    Dim Validator As DomValidator

    Set Validator = CreateCompositionRoot().DomCreateValidator()
    Set DomValidateManifest = Validator.DomValidateManifest(Manifest)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateCompositionRoot() As DomCompositionRoot
    Set CreateCompositionRoot = New DomCompositionRoot
End Function
