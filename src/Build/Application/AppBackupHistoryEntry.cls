VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppBackupHistoryEntry"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppBackupHistoryEntry
' Layer: Application
' Responsibility: Structured backup and restore history entry.
' Dependencies: Common
'=========================================================================

Private Type TMember
    HistoryID As String
    Timestamp As Date
    TargetType As String
    TargetName As String
    FilePath As String
    Action As String
    BackupPath As String
    Result As String
    Message As String
    UserName As String
    StudioVersion As String
End Type

Private This As TMember

Public Sub AppInitialize(ByVal HistoryID As String, ByVal Timestamp As Date, ByVal TargetType As String, ByVal TargetName As String, ByVal FilePath As String, ByVal Action As String, ByVal BackupPath As String, ByVal Result As String, ByVal Message As String, ByVal UserName As String, ByVal StudioVersion As String)
    This.HistoryID = HistoryID
    This.Timestamp = Timestamp
    This.TargetType = TargetType
    This.TargetName = TargetName
    This.FilePath = FilePath
    This.Action = Action
    This.BackupPath = BackupPath
    This.Result = Result
    This.Message = Message
    This.UserName = UserName
    This.StudioVersion = StudioVersion
End Sub

Public Property Get HistoryID() As String: HistoryID = This.HistoryID: End Property
Public Property Get Timestamp() As Date: Timestamp = This.Timestamp: End Property
Public Property Get TargetType() As String: TargetType = This.TargetType: End Property
Public Property Get TargetName() As String: TargetName = This.TargetName: End Property
Public Property Get FilePath() As String: FilePath = This.FilePath: End Property
Public Property Get Action() As String: Action = This.Action: End Property
Public Property Get BackupPath() As String: BackupPath = This.BackupPath: End Property
Public Property Get Result() As String: Result = This.Result: End Property
Public Property Get Message() As String: Message = This.Message: End Property
Public Property Get UserName() As String: UserName = This.UserName: End Property
Public Property Get StudioVersion() As String: StudioVersion = This.StudioVersion: End Property
