VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DomProject"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: DomProject
' Layer: Domain
' Responsibility: Project identity and root metadata.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Private Types
'=========================================================================

Private Type TMember
    ProjectName As String
    ProjectRootPath As String
    IsInitialized As Boolean
End Type

'=========================================================================
' Module Variables
'=========================================================================

Private This As TMember

'=========================================================================
' Public API
'=========================================================================

' Initializes project metadata.
Public Sub DomInitializeProject(ByVal ProjectName As String, ByVal ProjectRootPath As String)
    ComRequireText ProjectName, "ProjectName", "DomInitializeProject"
    ComRequireText ProjectRootPath, "ProjectRootPath", "DomInitializeProject"

    This.ProjectName = ComTrimText(ProjectName)
    This.ProjectRootPath = ComTrimText(ProjectRootPath)
    This.IsInitialized = True
End Sub

' Returns the project name.
Public Property Get Name() As String
    EnsureInitialized
    Name = This.ProjectName
End Property

' Returns the project root path.
Public Property Get RootPath() As String
    EnsureInitialized
    RootPath = This.ProjectRootPath
End Property

' Returns True when the instance is initialized.
Public Property Get IsInitialized() As Boolean
    IsInitialized = This.IsInitialized
End Property

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "DomProject", "DomProject is not initialized."
    End If
End Sub
