VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DomValidator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: DomValidator
' Layer: Domain
' Responsibility: Domain validation rules.
' Dependencies: Common, DomProject, DomModuleInfo, DomManifest
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const DomValidationSource As String = "DomValidator"
Private Const DomProjectRequiredMessage As String = "Project is required."
Private Const DomModuleInfoRequiredMessage As String = "ModuleInfo is required."
Private Const DomManifestRequiredMessage As String = "Manifest is required."

'=========================================================================
' Public API
'=========================================================================

' Validates project metadata.
Public Function DomValidateProject(ByVal Project As DomProject) As ComResult
    If Project Is Nothing Then
        Set DomValidateProject = CreateValidationFailure(DomProjectRequiredMessage)
        Exit Function
    End If

    If Not Project.IsInitialized Then
        Set DomValidateProject = CreateValidationFailure("Project must be initialized.")
        Exit Function
    End If

    If ComIsBlankText(Project.Name) Then
        Set DomValidateProject = CreateValidationFailure("Project.Name is required.")
        Exit Function
    End If

    If ComIsBlankText(Project.RootPath) Then
        Set DomValidateProject = CreateValidationFailure("Project.RootPath is required.")
        Exit Function
    End If

    Set DomValidateProject = ComCreateSuccess("Project is valid.")
End Function

' Validates module metadata.
Public Function DomValidateModuleInfo(ByVal ModuleInfo As DomModuleInfo) As ComResult
    If ModuleInfo Is Nothing Then
        Set DomValidateModuleInfo = CreateValidationFailure(DomModuleInfoRequiredMessage)
        Exit Function
    End If

    If Not ModuleInfo.IsInitialized Then
        Set DomValidateModuleInfo = CreateValidationFailure("ModuleInfo must be initialized.")
        Exit Function
    End If

    If ComIsBlankText(ModuleInfo.Name) Then
        Set DomValidateModuleInfo = CreateValidationFailure("ModuleInfo.Name is required.")
        Exit Function
    End If

    If ComIsBlankText(ModuleInfo.Layer) Then
        Set DomValidateModuleInfo = CreateValidationFailure("ModuleInfo.Layer is required.")
        Exit Function
    End If

    If ComIsBlankText(ModuleInfo.Path) Then
        Set DomValidateModuleInfo = CreateValidationFailure("ModuleInfo.Path is required.")
        Exit Function
    End If

    Set DomValidateModuleInfo = ComCreateSuccess("ModuleInfo is valid.")
End Function

' Validates manifest metadata.
Public Function DomValidateManifest(ByVal Manifest As DomManifest) As ComResult
    If Manifest Is Nothing Then
        Set DomValidateManifest = CreateValidationFailure(DomManifestRequiredMessage)
        Exit Function
    End If

    If Not Manifest.IsInitialized Then
        Set DomValidateManifest = CreateValidationFailure("Manifest must be initialized.")
        Exit Function
    End If

    Set DomValidateManifest = DomValidateProject(Manifest.Project)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateValidationFailure(ByVal Description As String) As ComResult
    Dim ErrorInfo As ComErrorInfo

    Set ErrorInfo = ComCreateErrorInfo(ComErrInvalidArgument, DomValidationSource, Description)
    Set CreateValidationFailure = ComCreateFailure(ErrorInfo)
End Function
