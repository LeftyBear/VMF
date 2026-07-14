VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppModuleGenerateResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppModuleGenerateResult
' Layer: Application
' Responsibility: Per-module generate result.
' Dependencies: Common
'=========================================================================

Private Type TMember
    ModuleName As String
    ModuleType As String
    OutputPath As String
    Status As String
    Message As String
    ErrorCode As String
    ElapsedTime As Double
    WasOverwritten As Boolean
    LoggedAt As Date
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal OutputPath As String, _
    ByVal Status As String, _
    ByVal Message As String, _
    ByVal ErrorCode As String, _
    ByVal ElapsedTime As Double, _
    ByVal WasOverwritten As Boolean, _
    ByVal LoggedAt As Date)

    ComRequireText ModuleName, "ModuleName", "AppModuleGenerateResult.AppInitialize"
    ComRequireText ModuleType, "ModuleType", "AppModuleGenerateResult.AppInitialize"
    ComRequireText Status, "Status", "AppModuleGenerateResult.AppInitialize"
    ComRequireText Message, "Message", "AppModuleGenerateResult.AppInitialize"

    This.ModuleName = ModuleName
    This.ModuleType = ModuleType
    This.OutputPath = OutputPath
    This.Status = Status
    This.Message = Message
    This.ErrorCode = ErrorCode
    This.ElapsedTime = ElapsedTime
    This.WasOverwritten = WasOverwritten
    This.LoggedAt = LoggedAt
    This.IsInitialized = True
End Sub

Public Property Get ModuleName() As String
    EnsureInitialized
    ModuleName = This.ModuleName
End Property

Public Property Get ModuleType() As String
    EnsureInitialized
    ModuleType = This.ModuleType
End Property

Public Property Get OutputPath() As String
    EnsureInitialized
    OutputPath = This.OutputPath
End Property

Public Property Get Status() As String
    EnsureInitialized
    Status = This.Status
End Property

Public Property Get Message() As String
    EnsureInitialized
    Message = This.Message
End Property

Public Property Get ErrorCode() As String
    EnsureInitialized
    ErrorCode = This.ErrorCode
End Property

Public Property Get ElapsedTime() As Double
    EnsureInitialized
    ElapsedTime = This.ElapsedTime
End Property

Public Property Get WasOverwritten() As Boolean
    EnsureInitialized
    WasOverwritten = This.WasOverwritten
End Property

Public Property Get LoggedAt() As Date
    EnsureInitialized
    LoggedAt = This.LoggedAt
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppModuleGenerateResult", "Module generate result is not initialized."
    End If
End Sub
