VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGenerateRequest"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppGenerateRequest
' Layer: Application
' Responsibility: Manifest Editor generate request.
' Dependencies: Common
'=========================================================================

Private Type TMember
    TargetScope As String
    SelectedModuleNames As Collection
    OutputDirectory As String
    OverwriteMode As String
    ContinueOnError As Boolean
    RequestedAt As Date
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal TargetScope As String, _
    ByVal SelectedModuleNames As Collection, _
    ByVal OutputDirectory As String, _
    ByVal OverwriteMode As String, _
    ByVal ContinueOnError As Boolean, _
    ByVal RequestedAt As Date)

    ComRequireText TargetScope, "TargetScope", "AppGenerateRequest.AppInitialize"
    ComRequireText OutputDirectory, "OutputDirectory", "AppGenerateRequest.AppInitialize"
    ComRequireText OverwriteMode, "OverwriteMode", "AppGenerateRequest.AppInitialize"

    If SelectedModuleNames Is Nothing Then
        Set This.SelectedModuleNames = New Collection
    Else
        Set This.SelectedModuleNames = SelectedModuleNames
    End If

    This.TargetScope = TargetScope
    This.OutputDirectory = OutputDirectory
    This.OverwriteMode = OverwriteMode
    This.ContinueOnError = ContinueOnError
    This.RequestedAt = RequestedAt
    This.IsInitialized = True
End Sub

Public Property Get TargetScope() As String
    EnsureInitialized
    TargetScope = This.TargetScope
End Property

Public Property Get SelectedModuleNames() As Collection
    EnsureInitialized
    Set SelectedModuleNames = This.SelectedModuleNames
End Property

Public Property Get OutputDirectory() As String
    EnsureInitialized
    OutputDirectory = This.OutputDirectory
End Property

Public Property Get OverwriteMode() As String
    EnsureInitialized
    OverwriteMode = This.OverwriteMode
End Property

Public Property Get ContinueOnError() As Boolean
    EnsureInitialized
    ContinueOnError = This.ContinueOnError
End Property

Public Property Get RequestedAt() As Date
    EnsureInitialized
    RequestedAt = This.RequestedAt
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppGenerateRequest", "Generate request is not initialized."
    End If
End Sub
