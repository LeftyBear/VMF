VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppExportService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppExportService
' Layer: Application
' Responsibility: Source export coordination.
' Dependencies: Common, Domain, Infrastructure
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const AppLineBreak As String = vbCrLf

'=========================================================================
' Public API
'=========================================================================

' Exports project metadata to a text manifest file.
Public Function AppExportProject( _
    ByVal ValidationService As AppValidationService, _
    ByVal ProjectName As String, _
    ByVal ProjectRootPath As String, _
    ByVal OutputFilePath As String) As ComResult

    Dim ValidationResult As ComResult
    Dim ManifestContent As String

    If ValidationService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppExportProject", "ValidationService is required."
    End If

    Set ValidationResult = ValidationService.AppValidateProject(ProjectName, ProjectRootPath)

    If ValidationResult.IsFailure Then
        Set AppExportProject = ValidationResult
        Exit Function
    End If

    ComRequireText OutputFilePath, "OutputFilePath", "AppExportProject"
    ManifestContent = BuildManifestText(ProjectName, ProjectRootPath)

    Set AppExportProject = InfWriteText(OutputFilePath, ManifestContent)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function BuildManifestText(ByVal ProjectName As String, ByVal ProjectRootPath As String) As String
    BuildManifestText = _
        "ProjectName=" & ComTrimText(ProjectName) & AppLineBreak & _
        "ProjectRootPath=" & ComTrimText(ProjectRootPath) & AppLineBreak
End Function
