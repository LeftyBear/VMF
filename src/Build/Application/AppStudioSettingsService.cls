VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppStudioSettingsService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppStudioSettingsService
' Layer: Application
' Responsibility: Load, validate, and save VMF Studio settings.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const SeverityError As Long = 1
Private Const SeverityWarning As Long = 2
Private Const SeverityInformation As Long = 3
Private Const SettingsFolderName As String = "VMF"
Private Const SettingsFileName As String = "StudioSettings.ini"

Private FileProvider As InfFileSystemProvider
Private SettingsFilePathOverride As String

Public Sub AppInitialize(ByVal ProviderFileSystem As InfFileSystemProvider, Optional ByVal SettingsFilePath As String = vbNullString)
    If ProviderFileSystem Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppStudioSettingsService.AppInitialize", "FileSystemProvider is required."
    End If
    Set FileProvider = ProviderFileSystem
    SettingsFilePathOverride = Trim$(SettingsFilePath)
End Sub

Public Function AppCreateDefaultSettings() As AppStudioSettings
    Dim Settings As AppStudioSettings
    Set Settings = New AppStudioSettings
    Settings.AppLoadDefaults
    Set AppCreateDefaultSettings = Settings
End Function

Public Function AppGetSettingsFilePath() As String
    Dim RootPath As String
    If Not ComIsBlankText(SettingsFilePathOverride) Then
        AppGetSettingsFilePath = SettingsFilePathOverride
        Exit Function
    End If

    RootPath = Environ$("APPDATA")
    If ComIsBlankText(RootPath) Then RootPath = Environ$("LOCALAPPDATA")
    If ComIsBlankText(RootPath) Then RootPath = ThisWorkbook.Path
    AppGetSettingsFilePath = BuildPathResolver.CombinePath(BuildPathResolver.CombinePath(RootPath, SettingsFolderName), SettingsFileName)
End Function

Public Function AppLoadSettings(ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim SettingsPath As String
    Dim Content As String

    On Error GoTo ErrHandler
    Set Issues = New Collection
    Set Settings = AppCreateDefaultSettings()
    SettingsPath = AppGetSettingsFilePath()

    If Not FileProvider.InfFileExists(SettingsPath) Then
        AddIssue Issues, SeverityInformation, "VMF-SET-000", "Settings file does not exist. Defaults were used.", "SettingsFile", 1
        Set AppLoadSettings = ComCreateSuccess("Studio settings loaded from defaults.")
        Exit Function
    End If

    Content = FileProvider.InfReadText(SettingsPath)
    ApplySettingsText Settings, Content, Issues
    ValidateSettings Settings, Issues
    Set AppLoadSettings = ComCreateSuccess("Studio settings loaded.")
    Exit Function

ErrHandler:
    Set Settings = AppCreateDefaultSettings()
    Set Issues = New Collection
    AddIssue Issues, SeverityWarning, "VMF-SET-LOAD-001", "Settings file could not be loaded. Defaults were used: " & Err.Description, "SettingsFile", 1
    Set AppLoadSettings = ComCreateSuccess("Studio settings loaded from defaults.")
End Function

Public Function AppLoadSettingsFromPath(ByVal SettingsPath As String, ByRef Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim Content As String
    On Error GoTo ErrHandler
    Set Issues = New Collection
    Set Settings = AppCreateDefaultSettings()
    If Not FileProvider.InfFileExists(SettingsPath) Then
        AddIssue Issues, SeverityError, "VMF-SET-LOAD-002", "Settings file does not exist: " & SettingsPath, "SettingsFile", 1
        Set AppLoadSettingsFromPath = ComCreateSuccess("Studio settings loaded from defaults.")
        Exit Function
    End If
    Content = FileProvider.InfReadText(SettingsPath)
    ApplySettingsText Settings, Content, Issues
    ValidateSettings Settings, Issues
    Set AppLoadSettingsFromPath = ComCreateSuccess("Studio settings loaded.")
    Exit Function
ErrHandler:
    Set Settings = AppCreateDefaultSettings()
    Set Issues = New Collection
    AddIssue Issues, SeverityWarning, "VMF-SET-LOAD-001", "Settings file could not be loaded. Defaults were used: " & Err.Description, "SettingsFile", 1
    Set AppLoadSettingsFromPath = ComCreateSuccess("Studio settings loaded from defaults.")
End Function

Public Function AppValidateSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    On Error GoTo ErrHandler
    Set Issues = New Collection
    ValidateSettings Settings, Issues
    Set AppValidateSettings = ComCreateSuccess(CreateValidationMessage(Issues))
    Exit Function

ErrHandler:
    Set Issues = New Collection
    AddIssue Issues, SeverityError, "VMF-SET-VAL-001", "Settings validation failed: " & Err.Description, "Internal", 1
    Set AppValidateSettings = ComCreateSuccess(CreateValidationMessage(Issues))
End Function

Public Function AppSaveSettings(ByVal Settings As AppStudioSettings, ByRef Issues As Collection) As ComResult
    Dim SettingsPath As String
    Dim SettingsFolder As String
    Dim TextValue As String
    Dim Result As ComResult
    Dim BackupPath As String
    Dim BackupService As AppBackupService

    On Error GoTo ErrHandler
    Set Issues = New Collection
    ValidateSettings Settings, Issues
    If AppIssuesContainErrors(Issues) Then
        Set AppSaveSettings = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppStudioSettingsService.AppSaveSettings", CreateValidationMessage(Issues)))
        Exit Function
    End If

    SettingsPath = AppGetSettingsFilePath()
    SettingsFolder = Left$(SettingsPath, InStrRev(SettingsPath, Application.PathSeparator) - 1)
    Set Result = FileProvider.InfEnsureFolder(SettingsFolder)
    If Result.IsFailure Then
        AddIssue Issues, SeverityError, "VMF-SET-SAVE-001", "Settings folder could not be created: " & Result.Message, "SettingsFile", 900
        Set AppSaveSettings = Result
        Exit Function
    End If

    TextValue = Settings.AppToText()
    Set BackupService = CreateBackupService(Settings.BackupDirectory)
    Set Result = BackupService.AppSafeSaveText("Settings", SettingsPath, TextValue, "Update", BackupPath)
    If Result.IsFailure Then
        Set AppSaveSettings = Result
        Exit Function
    End If
    Set AppSaveSettings = ComCreateSuccess("Studio settings saved.")
    Exit Function

ErrHandler:
    Set AppSaveSettings = ComCreateFailureFromErr("AppStudioSettingsService.AppSaveSettings", Err.Description, "Error", False)
End Function

Private Function CreateBackupService(ByVal BackupDirectory As String) As AppBackupService
    Dim BackupService As AppBackupService
    Dim LocalSettings As AppStudioSettingsService
    Dim EditorService As AppManifestEditorService
    Dim TemplateService As AppTemplateService
    Set LocalSettings = New AppStudioSettingsService
    LocalSettings.AppInitialize FileProvider, SettingsFilePathOverride
    Set EditorService = New AppManifestEditorService
    Set TemplateService = New AppTemplateService
    TemplateService.AppInitialize InfCreateGenerator(), EditorService, FileProvider, LocalSettings
    Set BackupService = New AppBackupService
    BackupService.AppInitialize FileProvider, LocalSettings, EditorService, New AppManifestValidationService, TemplateService, BackupDirectory
    Set CreateBackupService = BackupService
End Function

Public Function AppRestoreDefaults() As AppStudioSettings
    Set AppRestoreDefaults = AppCreateDefaultSettings()
End Function

Public Function AppResolvePath(ByVal BasePath As String, ByVal PathValue As String) As String
    Dim Normalized As String
    Normalized = Trim$(PathValue)
    If ComIsBlankText(Normalized) Then
        AppResolvePath = vbNullString
        Exit Function
    End If
    Normalized = Replace(Normalized, "/", Application.PathSeparator)
    If IsAbsolutePath(Normalized) Then
        AppResolvePath = Normalized
    ElseIf ComIsBlankText(BasePath) Then
        AppResolvePath = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName(Normalized)
    Else
        AppResolvePath = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName(BuildPathResolver.CombinePath(BasePath, Normalized))
    End If
End Function

Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim Issue As AppStudioSettingsIssue
    If Issues Is Nothing Then Exit Function
    For Each Issue In Issues
        If Issue.Severity = SeverityError Then
            AppIssuesContainErrors = True
            Exit Function
        End If
    Next Issue
End Function

Private Sub ApplySettingsText(ByVal Settings As AppStudioSettings, ByVal Content As String, ByVal Issues As Collection)
    Dim Lines() As String
    Dim LineIndex As Long
    Dim LineText As String
    Dim SeparatorIndex As Long
    Dim KeyName As String
    Dim KeyValue As String
    Dim SeenKeys As Object

    Set SeenKeys = CreateObject("Scripting.Dictionary")
    SeenKeys.CompareMode = vbTextCompare
    Lines = Split(NormalizeLineBreaks(Content), vbLf)
    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(Lines(LineIndex))
        If Len(LineText) = 0 Or Left$(LineText, 1) = "#" Or Left$(LineText, 1) = ";" Then
            GoTo NextLine
        End If

        SeparatorIndex = InStr(1, LineText, "=", vbBinaryCompare)
        If SeparatorIndex <= 1 Then
            AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-001", "Settings line is invalid and was ignored.", "Line" & CStr(LineIndex + 1), 100 + LineIndex
            GoTo NextLine
        End If

        KeyName = Trim$(Left$(LineText, SeparatorIndex - 1))
        KeyValue = Trim$(Mid$(LineText, SeparatorIndex + 1))
        If SeenKeys.Exists(KeyName) Then
            AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-002", "Duplicate setting key was ignored: " & KeyName, KeyName, 200 + LineIndex
            GoTo NextLine
        End If
        SeenKeys.Add KeyName, True
        ApplySetting Settings, KeyName, KeyValue, Issues, LineIndex + 1

NextLine:
    Next LineIndex
End Sub

Private Sub ApplySetting(ByVal Settings As AppStudioSettings, ByVal KeyName As String, ByVal KeyValue As String, ByVal Issues As Collection, ByVal LineNumber As Long)
    On Error GoTo ConversionFailed
    Select Case KeyName
        Case "ManifestDirectory": Settings.ManifestDirectory = KeyValue
        Case "TemplateDirectory": Settings.TemplateDirectory = KeyValue
        Case "OutputDirectory": Settings.OutputDirectory = KeyValue
        Case "LogDirectory": Settings.LogDirectory = KeyValue
        Case "BackupDirectory": Settings.BackupDirectory = KeyValue
        Case "MaxBackupCountPerFile": Settings.MaxBackupCountPerFile = CLng(KeyValue)
        Case "MaxBackupAgeDays": Settings.MaxBackupAgeDays = CLng(KeyValue)
        Case "DefaultTargetScope": Settings.DefaultTargetScope = KeyValue
        Case "DefaultOverwriteMode": Settings.DefaultOverwriteMode = KeyValue
        Case "ContinueOnError": Settings.ContinueOnError = ParseBoolean(KeyValue)
        Case "CreateBackup": Settings.CreateBackup = ParseBoolean(KeyValue)
        Case "OpenOutputAfterGenerate": Settings.OpenOutputAfterGenerate = ParseBoolean(KeyValue)
        Case "ValidateBeforeGenerate": Settings.ValidateBeforeGenerate = ParseBoolean(KeyValue)
        Case "PreviewFontName": Settings.PreviewFontName = KeyValue
        Case "PreviewFontSize": Settings.PreviewFontSize = CLng(KeyValue)
        Case "ShowLineNumbers": Settings.ShowLineNumbers = ParseBoolean(KeyValue)
        Case "PreserveBlankLines": Settings.PreserveBlankLines = ParseBoolean(KeyValue)
        Case "ConfirmOnUnsavedChanges": Settings.ConfirmOnUnsavedChanges = ParseBoolean(KeyValue)
        Case "LastProjectPath": Settings.LastProjectPath = KeyValue
        Case "LastSelectedModule": Settings.LastSelectedModule = KeyValue
        Case "LastSelectedTemplate": Settings.LastSelectedTemplate = KeyValue
        Case "WindowWidth": Settings.WindowWidth = CLng(KeyValue)
        Case "WindowHeight": Settings.WindowHeight = CLng(KeyValue)
        Case "ActivePage": Settings.ActivePage = KeyValue
        Case Else
            AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-003", "Unknown setting key was ignored: " & KeyName, KeyName, 300 + LineNumber
    End Select
    Exit Sub

ConversionFailed:
    AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-004", "Setting value conversion failed: " & KeyName, KeyName, 400 + LineNumber
End Sub

Private Sub ValidateSettings(ByVal Settings As AppStudioSettings, ByVal Issues As Collection)
    If Settings Is Nothing Then
        AddIssue Issues, SeverityError, "VMF-SET-001", "Settings are required.", "Settings", 1
        Exit Sub
    End If

    ValidateExistingDirectory Settings.ManifestDirectory, "ManifestDirectory", Issues, 10
    ValidateExistingDirectory Settings.TemplateDirectory, "TemplateDirectory", Issues, 20
    ValidateOutputDirectory Settings.OutputDirectory, "OutputDirectory", Issues, 30
    ValidateOutputDirectory Settings.LogDirectory, "LogDirectory", Issues, 40
    ValidateOutputDirectory Settings.BackupDirectory, "BackupDirectory", Issues, 50
    If Settings.MaxBackupCountPerFile < 1 Or Settings.MaxBackupCountPerFile > 999 Then
        AddIssue Issues, SeverityError, "VMF-SET-BKP-001", "MaxBackupCountPerFile must be between 1 and 999.", "MaxBackupCountPerFile", 60
    End If
    If Settings.MaxBackupAgeDays < 1 Or Settings.MaxBackupAgeDays > 3650 Then
        AddIssue Issues, SeverityError, "VMF-SET-BKP-002", "MaxBackupAgeDays must be between 1 and 3650.", "MaxBackupAgeDays", 61
    End If

    If Not IsSupportedTargetScope(Settings.DefaultTargetScope) Then
        AddIssue Issues, SeverityError, "VMF-SET-GEN-001", "DefaultTargetScope is not supported.", "DefaultTargetScope", 100
    End If
    If Not IsSupportedOverwriteMode(Settings.DefaultOverwriteMode) Then
        AddIssue Issues, SeverityError, "VMF-SET-GEN-002", "DefaultOverwriteMode is not supported.", "DefaultOverwriteMode", 101
    End If
    If ComIsBlankText(Settings.PreviewFontName) Then
        AddIssue Issues, SeverityError, "VMF-SET-EDT-001", "PreviewFontName is required.", "PreviewFontName", 200
    End If
    If Settings.PreviewFontSize < 6 Or Settings.PreviewFontSize > 24 Then
        AddIssue Issues, SeverityError, "VMF-SET-EDT-002", "PreviewFontSize must be between 6 and 24.", "PreviewFontSize", 201
    End If
    If Settings.WindowWidth < 620 Or Settings.WindowWidth > 1600 Then
        AddIssue Issues, SeverityWarning, "VMF-SET-STU-001", "WindowWidth is outside the recommended range.", "WindowWidth", 300
    End If
    If Settings.WindowHeight < 420 Or Settings.WindowHeight > 1200 Then
        AddIssue Issues, SeverityWarning, "VMF-SET-STU-002", "WindowHeight is outside the recommended range.", "WindowHeight", 301
    End If
End Sub

Private Sub ValidateExistingDirectory(ByVal PathValue As String, ByVal KeyName As String, ByVal Issues As Collection, ByVal SortOrder As Long)
    Dim ResolvedPath As String
    If ComIsBlankText(PathValue) Then
        AddIssue Issues, SeverityError, "VMF-SET-PATH-001", KeyName & " is required.", KeyName, SortOrder
        Exit Sub
    End If

    ResolvedPath = AppResolvePath(BuildPathResolver.RepositoryRootPath(), PathValue)
    If Not FolderExists(ResolvedPath) Then
        AddIssue Issues, SeverityError, "VMF-SET-PATH-002", KeyName & " does not exist: " & ResolvedPath, KeyName, SortOrder + 1
    End If
End Sub

Private Sub ValidateOutputDirectory(ByVal PathValue As String, ByVal KeyName As String, ByVal Issues As Collection, ByVal SortOrder As Long)
    Dim ResolvedPath As String
    If ComIsBlankText(PathValue) Then
        AddIssue Issues, SeverityError, "VMF-SET-PATH-001", KeyName & " is required.", KeyName, SortOrder
        Exit Sub
    End If

    ResolvedPath = AppResolvePath(BuildPathResolver.RepositoryRootPath(), PathValue)
    If Not FolderExists(ResolvedPath) Then
        If Not CanWriteFolder(GetParentFolderPath(ResolvedPath)) Then
            AddIssue Issues, SeverityError, "VMF-SET-PATH-004", KeyName & " cannot be created: " & ResolvedPath, KeyName, SortOrder + 2
        Else
            AddIssue Issues, SeverityWarning, "VMF-SET-PATH-005", KeyName & " does not exist and will be created when needed: " & ResolvedPath, KeyName, SortOrder + 1
        End If
    ElseIf Not CanWriteFolder(ResolvedPath) Then
        AddIssue Issues, SeverityError, "VMF-SET-PATH-003", KeyName & " is not writable: " & ResolvedPath, KeyName, SortOrder + 1
    End If
End Sub

Private Function FolderExists(ByVal FolderPath As String) As Boolean
    FolderExists = CreateObject("Scripting.FileSystemObject").FolderExists(FolderPath)
End Function

Private Function CanWriteFolder(ByVal FolderPath As String) As Boolean
    Dim TestPath As String
    On Error GoTo CannotWrite
    TestPath = BuildPathResolver.CombinePath(FolderPath, "~vmf_write_test.tmp")
    Open TestPath For Output As #1
    Print #1, "test"
    Close #1
    Kill TestPath
    CanWriteFolder = True
    Exit Function
CannotWrite:
    On Error Resume Next
    Close #1
    On Error GoTo 0
    CanWriteFolder = False
End Function

Private Function GetParentFolderPath(ByVal FilePath As String) As String
    GetParentFolderPath = CreateObject("Scripting.FileSystemObject").GetParentFolderName(FilePath)
End Function

Private Function ParseBoolean(ByVal Value As String) As Boolean
    Select Case LCase$(Trim$(Value))
        Case "true", "yes", "1", "-1"
            ParseBoolean = True
        Case "false", "no", "0"
            ParseBoolean = False
        Case Else
            Err.Raise ComErrInvalidArgument, "ParseBoolean", "Invalid Boolean value: " & Value
    End Select
End Function

Private Function IsSupportedTargetScope(ByVal Value As String) As Boolean
    Select Case Value
        Case "AllModules", "SelectedModules", "CurrentModule"
            IsSupportedTargetScope = True
    End Select
End Function

Private Function IsSupportedOverwriteMode(ByVal Value As String) As Boolean
    Select Case Value
        Case "Confirm", "Overwrite", "Skip"
            IsSupportedOverwriteMode = True
    End Select
End Function

Private Function IsAbsolutePath(ByVal PathValue As String) As Boolean
    IsAbsolutePath = (Len(PathValue) >= 2 And Mid$(PathValue, 2, 1) = ":") Or Left$(PathValue, 2) = "\\"
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    Dim Result As String
    Result = Replace(TextValue, vbCrLf, vbLf)
    Result = Replace(Result, vbCr, vbLf)
    NormalizeLineBreaks = Result
End Function

Private Function CreateValidationMessage(ByVal Issues As Collection) As String
    If Issues Is Nothing Or Issues.Count = 0 Then
        CreateValidationMessage = "Studio settings are valid."
    ElseIf AppIssuesContainErrors(Issues) Then
        CreateValidationMessage = "Studio settings are invalid."
    Else
        CreateValidationMessage = "Studio settings validation completed with warnings."
    End If
End Function

Private Sub AddIssue(ByVal Issues As Collection, ByVal Severity As Long, ByVal Code As String, ByVal Message As String, ByVal SettingKey As String, ByVal SortOrder As Long)
    Dim Issue As AppStudioSettingsIssue
    Set Issue = New AppStudioSettingsIssue
    Issue.AppInitialize Severity, Code, Message, SettingKey, SortOrder
    Issues.Add Issue
End Sub
