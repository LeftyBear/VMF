VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DomManifest"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: DomManifest
' Layer: Domain
' Responsibility: Manifest metadata.
' Dependencies: Common, DomProject
'=========================================================================

'=========================================================================
' Private Types
'=========================================================================

Private Type TMember
    Project As DomProject
    IsInitialized As Boolean
End Type

'=========================================================================
' Module Variables
'=========================================================================

Private This As TMember

'=========================================================================
' Public API
'=========================================================================

' Initializes manifest metadata.
Public Sub DomInitializeManifest(ByVal Project As DomProject)
    If Project Is Nothing Then
        Err.Raise ComErrInvalidArgument, "DomInitializeManifest", "Project is required."
    End If

    If Not Project.IsInitialized Then
        Err.Raise ComErrInvalidArgument, "DomInitializeManifest", "Project must be initialized."
    End If

    Set This.Project = Project
    This.IsInitialized = True
End Sub

' Returns the manifest project.
Public Property Get Project() As DomProject
    EnsureInitialized
    Set Project = This.Project
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
        Err.Raise ComErrInvalidState, "DomManifest", "DomManifest is not initialized."
    End If
End Sub
