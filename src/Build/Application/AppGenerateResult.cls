VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGenerateResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppGenerateResult
' Layer: Application
' Responsibility: Project-level manifest generate result.
' Dependencies: Common, Application
'=========================================================================

Private Type TMember
    StartedAt As Date
    FinishedAt As Date
    SuccessCount As Long
    WarningCount As Long
    FailureCount As Long
    SkippedCount As Long
    OutputDirectory As String
    ModuleResults As Collection
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize(ByVal StartedAt As Date, ByVal OutputDirectory As String)
    ComRequireText OutputDirectory, "OutputDirectory", "AppGenerateResult.AppInitialize"

    This.StartedAt = StartedAt
    This.FinishedAt = StartedAt
    This.SuccessCount = 0
    This.WarningCount = 0
    This.FailureCount = 0
    This.SkippedCount = 0
    This.OutputDirectory = OutputDirectory
    Set This.ModuleResults = New Collection
    This.IsInitialized = True
End Sub

Public Sub AppAddModuleResult(ByVal ModuleResult As AppModuleGenerateResult)
    EnsureInitialized
    If ModuleResult Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppGenerateResult.AppAddModuleResult", "ModuleResult is required."
    End If

    This.ModuleResults.Add ModuleResult
    Select Case ModuleResult.Status
        Case "Generated"
            This.SuccessCount = This.SuccessCount + 1
        Case "Skipped"
            This.WarningCount = This.WarningCount + 1
            This.SkippedCount = This.SkippedCount + 1
        Case "Failed"
            This.FailureCount = This.FailureCount + 1
        Case Else
            This.WarningCount = This.WarningCount + 1
    End Select
End Sub

Public Sub AppFinish(ByVal FinishedAt As Date)
    EnsureInitialized
    This.FinishedAt = FinishedAt
End Sub

Public Property Get StartedAt() As Date
    EnsureInitialized
    StartedAt = This.StartedAt
End Property

Public Property Get FinishedAt() As Date
    EnsureInitialized
    FinishedAt = This.FinishedAt
End Property

Public Property Get SuccessCount() As Long
    EnsureInitialized
    SuccessCount = This.SuccessCount
End Property

Public Property Get WarningCount() As Long
    EnsureInitialized
    WarningCount = This.WarningCount
End Property

Public Property Get FailureCount() As Long
    EnsureInitialized
    FailureCount = This.FailureCount
End Property

Public Property Get SkippedCount() As Long
    EnsureInitialized
    SkippedCount = This.SkippedCount
End Property

Public Property Get OutputDirectory() As String
    EnsureInitialized
    OutputDirectory = This.OutputDirectory
End Property

Public Property Get ModuleResults() As Collection
    EnsureInitialized
    Set ModuleResults = This.ModuleResults
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppGenerateResult", "Generate result is not initialized."
    End If
End Sub
