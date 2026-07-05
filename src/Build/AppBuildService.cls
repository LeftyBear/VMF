VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppBuildService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppBuildService
' Layer: Application
' Responsibility: Build use case coordination.
' Dependencies: Common, Application
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Builds project metadata by validating and exporting it.
Public Function AppBuildProject( _
    ByVal ExportService As AppExportService, _
    ByVal ValidationService As AppValidationService, _
    ByVal ProjectName As String, _
    ByVal ProjectRootPath As String, _
    ByVal OutputFilePath As String) As ComResult

    Dim ValidationResult As ComResult

    If ExportService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppBuildProject", "ExportService is required."
    End If

    If ValidationService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppBuildProject", "ValidationService is required."
    End If

    ComRequireText ProjectName, "ProjectName", "AppBuildProject"
    ComRequireText ProjectRootPath, "ProjectRootPath", "AppBuildProject"
    ComRequireText OutputFilePath, "OutputFilePath", "AppBuildProject"

    Set ValidationResult = ValidationService.AppValidateProject(ProjectName, ProjectRootPath)
    If ValidationResult.IsFailure Then
        Set AppBuildProject = ValidationResult
        Exit Function
    End If

    Set AppBuildProject = ExportService.AppExportProject(ValidationService, ProjectName, ProjectRootPath, OutputFilePath)
End Function
