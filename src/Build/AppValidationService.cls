VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppValidationService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppValidationService
' Layer: Application
' Responsibility: Application-level validation coordination.
' Dependencies: Common, Domain
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Validates project inputs by creating and validating a Domain project model.
Public Function AppValidateProject(ByVal ProjectName As String, ByVal ProjectRootPath As String) As ComResult
    Dim Project As DomProject
    Dim ValidationResult As ComResult

    ComRequireText ProjectName, "ProjectName", "AppValidateProject"
    ComRequireText ProjectRootPath, "ProjectRootPath", "AppValidateProject"

    Set Project = DomCreateProject(ProjectName, ProjectRootPath)
    Set ValidationResult = DomValidateProject(Project)

    If ValidationResult.IsFailure Then
        Set AppValidateProject = ValidationResult
        Exit Function
    End If

    Set AppValidateProject = ComCreateSuccess("Project is valid.")
End Function
