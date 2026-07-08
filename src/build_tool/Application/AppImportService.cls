VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppImportService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppImportService
' Layer: Application
' Responsibility: Source import coordination.
' Dependencies: Common, Infrastructure
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Imports project metadata from a text manifest file.
Public Function AppImportProject(ByVal InputFilePath As String) As ComResult
    Dim ManifestContent As String

    ComRequireText InputFilePath, "InputFilePath", "AppImportProject"

    If Not InfFileExists(InputFilePath) Then
        Set AppImportProject = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidArgument, "AppImportProject", "InputFilePath does not exist."))
        Exit Function
    End If

    ManifestContent = InfReadText(InputFilePath)

    If ComIsBlankText(ManifestContent) Then
        Set AppImportProject = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidArgument, "AppImportProject", "Manifest content is empty."))
        Exit Function
    End If

    Set AppImportProject = ComCreateSuccess("Project metadata was imported.")
End Function
