VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DomCompositionRoot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: DomCompositionRoot
' Layer: Domain
' Responsibility: VMF Core composition boundary.
' Dependencies: Common, Domain
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes VMF Core services.
Public Function DomInitialize() As Boolean
    DomInitialize = True
End Function

' Shuts down VMF Core services.
Public Sub DomShutdown()
End Sub

' Creates a project model.
Public Function DomCreateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As DomProject
    Dim Project As DomProject

    Set Project = New DomProject
    Project.DomInitializeProject ProjectName, ProjectRootPath

    Set DomCreateProject = Project
End Function

' Creates a module metadata model.
Public Function DomCreateModuleInfo( _
    ByVal ModuleName As String, _
    ByVal LayerName As String, _
    ByVal FilePath As String) As DomModuleInfo

    Dim ModuleInfo As DomModuleInfo

    Set ModuleInfo = New DomModuleInfo
    ModuleInfo.DomInitializeModuleInfo ModuleName, LayerName, FilePath

    Set DomCreateModuleInfo = ModuleInfo
End Function

' Creates a manifest model.
Public Function DomCreateManifest(ByVal Project As DomProject) As DomManifest
    Dim Manifest As DomManifest

    Set Manifest = New DomManifest
    Manifest.DomInitializeManifest Project

    Set DomCreateManifest = Manifest
End Function

' Creates a domain validator.
Public Function DomCreateValidator() As DomValidator
    Set DomCreateValidator = New DomValidator
End Function
