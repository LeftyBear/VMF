VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppBackupService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppBackupService
' Layer: Application
' Responsibility: Safe save, backup, restore, and history management.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private FileProvider As InfFileSystemProvider
Private SettingsService As AppStudioSettingsService
Private ManifestEditorService As AppManifestEditorService
Private ManifestValidationService As AppManifestValidationService
Private TemplateService As AppTemplateService
Private BackupDirectoryOverride As String

Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider, ByVal ProviderSettings As AppStudioSettingsService, ByVal EditorService As AppManifestEditorService, ByVal ValidationService As AppManifestValidationService, ByVal ProviderTemplateService As AppTemplateService, Optional ByVal OverrideBackupDirectory As String = vbNullString)
    Set FileProvider = ProviderFileSystem
    Set SettingsService = ProviderSettings
    Set ManifestEditorService = EditorService
    Set ManifestValidationService = ValidationService
    Set TemplateService = ProviderTemplateService
    BackupDirectoryOverride = Trim$(OverrideBackupDirectory)
End Sub

Public Function AppSafeSaveText(ByVal TargetType As String, ByVal FilePath As String, ByVal NewContent As String, ByVal ActionName As String, ByRef BackupPath As String) As ComResult
    Dim TempPath As String
    Dim Reloaded As String
    Dim Result As ComResult

    On Error GoTo ErrHandler
    ComRequireText TargetType, "TargetType", "AppSafeSaveText"
    ComRequireText FilePath, "FilePath", "AppSafeSaveText"
    BackupPath = vbNullString

    If FileProvider.InfFileExists(FilePath) Then
        Set Result = AppCreateBackup(TargetType, FilePath, ActionName, BackupPath)
        If Result.IsFailure Then
            Set AppSafeSaveText = Result
            Exit Function
        End If
    End If

    TempPath = FilePath & ".tmp"
    Set Result = FileProvider.InfWriteText(TempPath, NewContent)
    If Result.IsFailure Then
        RecordHistory TargetType, FilePath, ActionName, BackupPath, "Failed", "Backup Write Error: " & Result.Message
        Set AppSafeSaveText = Result
        Exit Function
    End If

    Reloaded = FileProvider.InfReadText(TempPath)
    If Reloaded <> NewContent Then
        Kill TempPath
        RecordHistory TargetType, FilePath, ActionName, BackupPath, "Failed", "Backup Write Error: verification failed."
        Set AppSafeSaveText = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppBackupService.AppSafeSaveText", "Safe save verification failed."))
        Exit Function
    End If

    If FileProvider.InfFileExists(FilePath) Then Kill FilePath
    Name TempPath As FilePath
    RecordHistory TargetType, FilePath, ActionName, BackupPath, "Success", "Saved."
    CleanupBackups TargetType, FilePath
    Set AppSafeSaveText = ComCreateSuccess("Saved with backup.")
    Exit Function

ErrHandler:
    On Error Resume Next
    If Len(TempPath) > 0 Then If FileProvider.InfFileExists(TempPath) Then Kill TempPath
    On Error GoTo 0
    RecordHistory TargetType, FilePath, ActionName, BackupPath, "Failed", "Unexpected Error: " & Err.Description
    Set AppSafeSaveText = ComCreateFailureFromErr("AppBackupService.AppSafeSaveText", Err.Description, "Error", False)
End Function

Public Function AppCreateBackup(ByVal TargetType As String, ByVal FilePath As String, ByVal ActionName As String, ByRef BackupPath As String) As ComResult
    Dim BackupDirectory As String
    Dim BackupID As String
    Dim Result As ComResult

    On Error GoTo ErrHandler
    BackupDirectory = ResolveBackupDirectory()
    Set Result = FileProvider.InfEnsureFolder(BackupDirectory)
    If Result.IsFailure Then
        Set AppCreateBackup = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppBackupService.AppCreateBackup", "Backup Create Error: " & Result.Message))
        Exit Function
    End If

    BackupID = Format$(Now, "yyyymmdd_hhnnss") & "_" & Right$("000" & CStr(Timer * 1000 Mod 1000), 3)
    BackupPath = BuildPathResolver.CombinePath(BackupDirectory, GetFileName(FilePath) & "." & TargetType & "." & BackupID & ".bak")
    FileCopy FilePath, BackupPath
    RecordHistory TargetType, FilePath, "Create", BackupPath, "Success", "Backup created."
    Set AppCreateBackup = ComCreateSuccess("Backup created.")
    Exit Function

ErrHandler:
    Set AppCreateBackup = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppBackupService.AppCreateBackup", "Backup Create Error: " & Err.Description))
End Function

Public Function AppListBackups(Optional ByVal TargetTypeFilter As String = vbNullString) As Collection
    Dim Items As Collection
    Dim BackupDirectory As String
    Dim FileName As String
    Dim Entry As AppBackupEntry
    Dim BackupPath As String

    Set Items = New Collection
    BackupDirectory = ResolveBackupDirectory()
    If Not FolderExists(BackupDirectory) Then
        Set AppListBackups = Items
        Exit Function
    End If

    FileName = Dir$(BuildPathResolver.CombinePath(BackupDirectory, "*.bak"))
    Do While Len(FileName) > 0
        BackupPath = BuildPathResolver.CombinePath(BackupDirectory, FileName)
        Set Entry = ParseBackupEntry(BackupPath)
        If ComIsBlankText(TargetTypeFilter) Or StrComp(Entry.TargetType, TargetTypeFilter, vbTextCompare) = 0 Then
            Items.Add Entry
        End If
        FileName = Dir$()
    Loop
    Set AppListBackups = Items
End Function

Public Function AppListHistory() As Collection
    Dim Items As Collection
    Dim HistoryPath As String
    Dim Content As String
    Dim Lines() As String
    Dim LineIndex As Long
    Dim Fields() As String
    Dim Entry As AppBackupHistoryEntry

    Set Items = New Collection
    HistoryPath = HistoryFilePath()
    If Not FileProvider.InfFileExists(HistoryPath) Then
        Set AppListHistory = Items
        Exit Function
    End If

    Content = NormalizeLineBreaks(FileProvider.InfReadText(HistoryPath))
    Lines = Split(Content, vbLf)
    For LineIndex = LBound(Lines) To UBound(Lines)
        If Len(Trim$(Lines(LineIndex))) > 0 Then
            Fields = Split(Lines(LineIndex), vbTab)
            If UBound(Fields) >= 10 Then
                Set Entry = New AppBackupHistoryEntry
                Entry.AppInitialize Fields(0), CDate(Fields(1)), Fields(2), Fields(3), Fields(4), Fields(5), Fields(6), Fields(7), Fields(8), Fields(9), Fields(10)
                Items.Add Entry
            End If
        End If
    Next LineIndex
    Set AppListHistory = Items
End Function

Public Function AppRestoreBackup(ByVal BackupPath As String) As ComResult
    Dim Entry As AppBackupEntry
    Dim Content As String
    Dim BackupBeforeRestore As String
    Dim Result As ComResult

    On Error GoTo ErrHandler
    If Not FileProvider.InfFileExists(BackupPath) Then
        Set AppRestoreBackup = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppBackupService.AppRestoreBackup", "Backup Read Error: backup file does not exist."))
        Exit Function
    End If

    Set Entry = ParseBackupEntry(BackupPath)
    Content = FileProvider.InfReadText(BackupPath)
    Set Result = ValidateRestoreContent(Entry.TargetType, Entry.FilePath, Content)
    If Result.IsFailure Then
        RecordHistory Entry.TargetType, Entry.FilePath, "Restore", BackupPath, "Failed", "Restore Validation Error: " & Result.Message
        Set AppRestoreBackup = Result
        Exit Function
    End If

    If FileProvider.InfFileExists(Entry.FilePath) Then
        Set Result = AppCreateBackup(Entry.TargetType, Entry.FilePath, "Restore", BackupBeforeRestore)
        If Result.IsFailure Then
            Set AppRestoreBackup = Result
            Exit Function
        End If
    End If

    Set Result = AppSafeSaveText(Entry.TargetType, Entry.FilePath, Content, "Restore", BackupBeforeRestore)
    If Result.IsSuccess Then
        RecordHistory Entry.TargetType, Entry.FilePath, "Restore", BackupPath, "Success", "Restored from backup."
    End If
    Set AppRestoreBackup = Result
    Exit Function

ErrHandler:
    Set AppRestoreBackup = ComCreateFailureFromErr("AppBackupService.AppRestoreBackup", Err.Description, "Error", False)
End Function

Public Function AppDeleteBackup(ByVal BackupPath As String) As ComResult
    Dim Entry As AppBackupEntry
    On Error GoTo ErrHandler
    If Not FileProvider.InfFileExists(BackupPath) Then
        Set AppDeleteBackup = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppBackupService.AppDeleteBackup", "Backup Read Error: backup file does not exist."))
        Exit Function
    End If
    Set Entry = ParseBackupEntry(BackupPath)
    Kill BackupPath
    RecordHistory Entry.TargetType, Entry.FilePath, "Delete", BackupPath, "Success", "Backup deleted."
    Set AppDeleteBackup = ComCreateSuccess("Backup deleted.")
    Exit Function
ErrHandler:
    Set AppDeleteBackup = ComCreateFailureFromErr("AppBackupService.AppDeleteBackup", Err.Description, "Error", False)
End Function

Private Function ValidateRestoreContent(ByVal TargetType As String, ByVal FilePath As String, ByVal Content As String) As ComResult
    Dim TempPath As String
    Dim Modules As Collection
    Dim Issues As Collection
    Dim Template As AppTemplateModel
    Dim Settings As AppStudioSettings

    TempPath = FilePath & ".restorecheck.tmp"
    Select Case TargetType
        Case "Manifest"
            FileProvider.InfWriteText TempPath, Content
            Set ValidateRestoreContent = ManifestEditorService.AppLoadManifestEditorModel(TempPath, Modules)
            If ValidateRestoreContent.IsSuccess Then
                Set ValidateRestoreContent = ManifestValidationService.AppValidateManifestEditorModel(TempPath, Modules, Issues)
                If ManifestValidationService.AppIssuesContainErrors(Issues) Then
                    Set ValidateRestoreContent = ManifestValidationService.AppCreateFailureFromIssues(Issues, "AppBackupService.ValidateRestoreContent")
                End If
            End If
            Kill TempPath
        Case "Template"
            Set Template = TemplateService.AppCreateTemplateModel(GetFileName(FilePath), DetectTemplateType(FilePath), FilePath, Content)
            Set ValidateRestoreContent = TemplateService.AppValidateTemplate(Template, Issues)
            If TemplateService.AppIssuesContainErrors(Issues) Then
                Set ValidateRestoreContent = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppBackupService.ValidateRestoreContent", "Template validation failed."))
            End If
        Case "Settings"
            FileProvider.InfWriteText TempPath, Content
            Set ValidateRestoreContent = SettingsService.AppLoadSettingsFromPath(TempPath, Settings, Issues)
            If SettingsService.AppIssuesContainErrors(Issues) Then
                Set ValidateRestoreContent = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppBackupService.ValidateRestoreContent", "Settings validation failed."))
            End If
            Kill TempPath
        Case Else
            Set ValidateRestoreContent = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppBackupService.ValidateRestoreContent", "Unsupported target type."))
    End Select
End Function

Private Sub CleanupBackups(ByVal TargetType As String, ByVal FilePath As String)
    Dim Settings As AppStudioSettings
    Dim Issues As Collection
    Dim Items As Collection
    Dim Entry As AppBackupEntry
    Dim Matching As Collection

    On Error GoTo CleanupFailed
    SettingsService.AppLoadSettings Settings, Issues
    Set Items = AppListBackups(TargetType)
    Set Matching = New Collection
    For Each Entry In Items
        If StrComp(Entry.FilePath, FilePath, vbTextCompare) = 0 Then Matching.Add Entry
    Next Entry
    Do While Matching.Count > Settings.MaxBackupCountPerFile
        Set Entry = OldestBackup(Matching)
        Kill Entry.BackupPath
        RemoveBackupFromCollection Matching, Entry.BackupPath
    Loop
    Exit Sub
CleanupFailed:
    RecordHistory TargetType, FilePath, "Delete", vbNullString, "Failed", "Cleanup Error: " & Err.Description
End Sub

Private Function OldestBackup(ByVal Items As Collection) As AppBackupEntry
    Dim Entry As AppBackupEntry
    For Each Entry In Items
        If OldestBackup Is Nothing Then
            Set OldestBackup = Entry
        ElseIf Entry.CreatedAt < OldestBackup.CreatedAt Then
            Set OldestBackup = Entry
        End If
    Next Entry
End Function

Private Sub RemoveBackupFromCollection(ByVal Items As Collection, ByVal BackupPath As String)
    Dim Index As Long
    For Index = Items.Count To 1 Step -1
        If StrComp(Items.Item(Index).BackupPath, BackupPath, vbTextCompare) = 0 Then Items.Remove Index
    Next Index
End Sub

Private Function ParseBackupEntry(ByVal BackupPath As String) As AppBackupEntry
    Dim FileName As String
    Dim Parts() As String
    Dim TargetType As String
    Dim BackupID As String
    Dim TargetName As String
    Dim Entry As AppBackupEntry
    Dim SourcePath As String

    FileName = GetFileName(BackupPath)
    Parts = Split(FileName, ".")
    If UBound(Parts) >= 3 Then
        TargetType = Parts(UBound(Parts) - 2)
        BackupID = Parts(UBound(Parts) - 1)
        TargetName = Left$(FileName, Len(FileName) - Len("." & TargetType & "." & BackupID & ".bak"))
    Else
        TargetType = "Unknown"
        TargetName = FileName
        BackupID = Format$(FileDateTime(BackupPath), "yyyymmdd_hhnnss")
    End If
    SourcePath = ResolveOriginalPath(BackupPath, TargetName)
    Set Entry = New AppBackupEntry
    Entry.AppInitialize BackupID, TargetType, TargetName, SourcePath, BackupPath, FileDateTime(BackupPath), FileLen(BackupPath), "Backup"
    Set ParseBackupEntry = Entry
End Function

Private Function ResolveOriginalPath(ByVal BackupPath As String, ByVal TargetName As String) As String
    Dim HistoryItems As Collection
    Dim Entry As AppBackupHistoryEntry
    Set HistoryItems = AppListHistory()
    For Each Entry In HistoryItems
        If StrComp(Entry.BackupPath, BackupPath, vbTextCompare) = 0 Then
            ResolveOriginalPath = Entry.FilePath
            Exit Function
        End If
    Next Entry
    ResolveOriginalPath = TargetName
End Function

Private Sub RecordHistory(ByVal TargetType As String, ByVal FilePath As String, ByVal ActionName As String, ByVal BackupPath As String, ByVal ResultText As String, ByVal Message As String)
    Dim HistoryPath As String
    Dim LineText As String
    On Error GoTo HistoryFailed
    HistoryPath = HistoryFilePath()
    FileProvider.InfEnsureFolder Left$(HistoryPath, InStrRev(HistoryPath, Application.PathSeparator) - 1)
    LineText = CreateID() & vbTab & CStr(Now) & vbTab & TargetType & vbTab & GetFileName(FilePath) & vbTab & FilePath & vbTab & ActionName & vbTab & BackupPath & vbTab & ResultText & vbTab & Replace(Message, vbTab, " ") & vbTab & Environ$("USERNAME") & vbTab & "1.1"
    AppendText HistoryPath, LineText & vbCrLf
    Exit Sub
HistoryFailed:
End Sub

Private Sub AppendText(ByVal FilePath As String, ByVal TextValue As String)
    Dim FileNumber As Integer
    FileNumber = FreeFile
    Open FilePath For Append As #FileNumber
    Print #FileNumber, TextValue;
    Close #FileNumber
End Sub

Private Function ResolveBackupDirectory() As String
    Dim Settings As AppStudioSettings
    Dim Issues As Collection
    If Not ComIsBlankText(BackupDirectoryOverride) Then
        ResolveBackupDirectory = SettingsService.AppResolvePath(BuildPathResolver.RepositoryRootPath(), BackupDirectoryOverride)
        Exit Function
    End If
    SettingsService.AppLoadSettings Settings, Issues
    ResolveBackupDirectory = SettingsService.AppResolvePath(BuildPathResolver.RepositoryRootPath(), Settings.BackupDirectory)
End Function

Private Function HistoryFilePath() As String
    HistoryFilePath = BuildPathResolver.CombinePath(ResolveBackupDirectory(), "history.tsv")
End Function

Private Function CreateID() As String
    CreateID = Format$(Now, "yyyymmdd_hhnnss") & "_" & Right$("000" & CStr(Timer * 1000 Mod 1000), 3)
End Function

Private Function GetFileName(ByVal FilePath As String) As String
    GetFileName = CreateObject("Scripting.FileSystemObject").GetFileName(FilePath)
End Function

Private Function FolderExists(ByVal FolderPath As String) As Boolean
    FolderExists = CreateObject("Scripting.FileSystemObject").FolderExists(FolderPath)
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    Dim Result As String
    Result = Replace(TextValue, vbCrLf, vbLf)
    Result = Replace(Result, vbCr, vbLf)
    NormalizeLineBreaks = Result
End Function

Private Function DetectTemplateType(ByVal FilePath As String) As String
    If InStr(1, FilePath, "Class", vbTextCompare) > 0 Then
        DetectTemplateType = "Class"
    Else
        DetectTemplateType = "Standard Module"
    End If
End Function
