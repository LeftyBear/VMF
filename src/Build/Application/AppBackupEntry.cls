VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppBackupEntry"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppBackupEntry
' Layer: Application
' Responsibility: Structured backup file entry.
' Dependencies: Common
'=========================================================================

Private Type TMember
    BackupID As String
    TargetType As String
    TargetName As String
    FilePath As String
    BackupPath As String
    CreatedAt As Date
    SizeBytes As Double
    Action As String
End Type

Private This As TMember

Public Sub AppInitialize(ByVal BackupID As String, ByVal TargetType As String, ByVal TargetName As String, ByVal FilePath As String, ByVal BackupPath As String, ByVal CreatedAt As Date, ByVal SizeBytes As Double, ByVal Action As String)
    This.BackupID = BackupID
    This.TargetType = TargetType
    This.TargetName = TargetName
    This.FilePath = FilePath
    This.BackupPath = BackupPath
    This.CreatedAt = CreatedAt
    This.SizeBytes = SizeBytes
    This.Action = Action
End Sub

Public Property Get BackupID() As String: BackupID = This.BackupID: End Property
Public Property Get TargetType() As String: TargetType = This.TargetType: End Property
Public Property Get TargetName() As String: TargetName = This.TargetName: End Property
Public Property Get FilePath() As String: FilePath = This.FilePath: End Property
Public Property Get BackupPath() As String: BackupPath = This.BackupPath: End Property
Public Property Get CreatedAt() As Date: CreatedAt = This.CreatedAt: End Property
Public Property Get SizeBytes() As Double: SizeBytes = This.SizeBytes: End Property
Public Property Get Action() As String: Action = This.Action: End Property
