VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppStudioSettings"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppStudioSettings
' Layer: Application
' Responsibility: Editable VMF Studio settings model.
' Dependencies: Common
'=========================================================================

Private Type TMember
    ManifestDirectory As String
    TemplateDirectory As String
    OutputDirectory As String
    LogDirectory As String
    BackupDirectory As String
    DefaultTargetScope As String
    DefaultOverwriteMode As String
    ContinueOnError As Boolean
    CreateBackup As Boolean
    OpenOutputAfterGenerate As Boolean
    ValidateBeforeGenerate As Boolean
    PreviewFontName As String
    PreviewFontSize As Long
    ShowLineNumbers As Boolean
    PreserveBlankLines As Boolean
    ConfirmOnUnsavedChanges As Boolean
    LastProjectPath As String
    LastSelectedModule As String
    LastSelectedTemplate As String
    WindowWidth As Long
    WindowHeight As Long
    ActivePage As String
End Type

Private This As TMember

Public Sub AppLoadDefaults()
    This.ManifestDirectory = BuildPathResolver.CombinePath(BuildPathResolver.RepositoryRootPath(), "src\Build")
    This.TemplateDirectory = BuildPathResolver.TemplatesDirectoryPath()
    This.OutputDirectory = "GeneratedSource"
    This.LogDirectory = "Logs"
    This.BackupDirectory = "Backups"
    This.DefaultTargetScope = "CurrentModule"
    This.DefaultOverwriteMode = "Confirm"
    This.ContinueOnError = False
    This.CreateBackup = False
    This.OpenOutputAfterGenerate = False
    This.ValidateBeforeGenerate = True
    This.PreviewFontName = "Consolas"
    This.PreviewFontSize = 9
    This.ShowLineNumbers = False
    This.PreserveBlankLines = True
    This.ConfirmOnUnsavedChanges = True
    This.LastProjectPath = vbNullString
    This.LastSelectedModule = vbNullString
    This.LastSelectedTemplate = vbNullString
    This.WindowWidth = 980
    This.WindowHeight = 520
    This.ActivePage = "Manifest"
End Sub

Public Function AppClone() As AppStudioSettings
    Dim Settings As AppStudioSettings
    Set Settings = New AppStudioSettings
    Settings.ManifestDirectory = This.ManifestDirectory
    Settings.TemplateDirectory = This.TemplateDirectory
    Settings.OutputDirectory = This.OutputDirectory
    Settings.LogDirectory = This.LogDirectory
    Settings.BackupDirectory = This.BackupDirectory
    Settings.DefaultTargetScope = This.DefaultTargetScope
    Settings.DefaultOverwriteMode = This.DefaultOverwriteMode
    Settings.ContinueOnError = This.ContinueOnError
    Settings.CreateBackup = This.CreateBackup
    Settings.OpenOutputAfterGenerate = This.OpenOutputAfterGenerate
    Settings.ValidateBeforeGenerate = This.ValidateBeforeGenerate
    Settings.PreviewFontName = This.PreviewFontName
    Settings.PreviewFontSize = This.PreviewFontSize
    Settings.ShowLineNumbers = This.ShowLineNumbers
    Settings.PreserveBlankLines = This.PreserveBlankLines
    Settings.ConfirmOnUnsavedChanges = This.ConfirmOnUnsavedChanges
    Settings.LastProjectPath = This.LastProjectPath
    Settings.LastSelectedModule = This.LastSelectedModule
    Settings.LastSelectedTemplate = This.LastSelectedTemplate
    Settings.WindowWidth = This.WindowWidth
    Settings.WindowHeight = This.WindowHeight
    Settings.ActivePage = This.ActivePage
    Set AppClone = Settings
End Function

Public Function AppToText() As String
    AppToText = _
        "ManifestDirectory=" & This.ManifestDirectory & vbCrLf & _
        "TemplateDirectory=" & This.TemplateDirectory & vbCrLf & _
        "OutputDirectory=" & This.OutputDirectory & vbCrLf & _
        "LogDirectory=" & This.LogDirectory & vbCrLf & _
        "BackupDirectory=" & This.BackupDirectory & vbCrLf & _
        "DefaultTargetScope=" & This.DefaultTargetScope & vbCrLf & _
        "DefaultOverwriteMode=" & This.DefaultOverwriteMode & vbCrLf & _
        "ContinueOnError=" & CStr(This.ContinueOnError) & vbCrLf & _
        "CreateBackup=" & CStr(This.CreateBackup) & vbCrLf & _
        "OpenOutputAfterGenerate=" & CStr(This.OpenOutputAfterGenerate) & vbCrLf & _
        "ValidateBeforeGenerate=" & CStr(This.ValidateBeforeGenerate) & vbCrLf & _
        "PreviewFontName=" & This.PreviewFontName & vbCrLf & _
        "PreviewFontSize=" & CStr(This.PreviewFontSize) & vbCrLf & _
        "ShowLineNumbers=" & CStr(This.ShowLineNumbers) & vbCrLf & _
        "PreserveBlankLines=" & CStr(This.PreserveBlankLines) & vbCrLf & _
        "ConfirmOnUnsavedChanges=" & CStr(This.ConfirmOnUnsavedChanges) & vbCrLf & _
        "LastProjectPath=" & This.LastProjectPath & vbCrLf & _
        "LastSelectedModule=" & This.LastSelectedModule & vbCrLf & _
        "LastSelectedTemplate=" & This.LastSelectedTemplate & vbCrLf & _
        "WindowWidth=" & CStr(This.WindowWidth) & vbCrLf & _
        "WindowHeight=" & CStr(This.WindowHeight) & vbCrLf & _
        "ActivePage=" & This.ActivePage & vbCrLf
End Function

Public Property Get ManifestDirectory() As String: ManifestDirectory = This.ManifestDirectory: End Property
Public Property Let ManifestDirectory(ByVal Value As String): This.ManifestDirectory = Trim$(Value): End Property
Public Property Get TemplateDirectory() As String: TemplateDirectory = This.TemplateDirectory: End Property
Public Property Let TemplateDirectory(ByVal Value As String): This.TemplateDirectory = Trim$(Value): End Property
Public Property Get OutputDirectory() As String: OutputDirectory = This.OutputDirectory: End Property
Public Property Let OutputDirectory(ByVal Value As String): This.OutputDirectory = Trim$(Value): End Property
Public Property Get LogDirectory() As String: LogDirectory = This.LogDirectory: End Property
Public Property Let LogDirectory(ByVal Value As String): This.LogDirectory = Trim$(Value): End Property
Public Property Get BackupDirectory() As String: BackupDirectory = This.BackupDirectory: End Property
Public Property Let BackupDirectory(ByVal Value As String): This.BackupDirectory = Trim$(Value): End Property
Public Property Get DefaultTargetScope() As String: DefaultTargetScope = This.DefaultTargetScope: End Property
Public Property Let DefaultTargetScope(ByVal Value As String): This.DefaultTargetScope = Trim$(Value): End Property
Public Property Get DefaultOverwriteMode() As String: DefaultOverwriteMode = This.DefaultOverwriteMode: End Property
Public Property Let DefaultOverwriteMode(ByVal Value As String): This.DefaultOverwriteMode = Trim$(Value): End Property
Public Property Get ContinueOnError() As Boolean: ContinueOnError = This.ContinueOnError: End Property
Public Property Let ContinueOnError(ByVal Value As Boolean): This.ContinueOnError = Value: End Property
Public Property Get CreateBackup() As Boolean: CreateBackup = This.CreateBackup: End Property
Public Property Let CreateBackup(ByVal Value As Boolean): This.CreateBackup = Value: End Property
Public Property Get OpenOutputAfterGenerate() As Boolean: OpenOutputAfterGenerate = This.OpenOutputAfterGenerate: End Property
Public Property Let OpenOutputAfterGenerate(ByVal Value As Boolean): This.OpenOutputAfterGenerate = Value: End Property
Public Property Get ValidateBeforeGenerate() As Boolean: ValidateBeforeGenerate = This.ValidateBeforeGenerate: End Property
Public Property Let ValidateBeforeGenerate(ByVal Value As Boolean): This.ValidateBeforeGenerate = Value: End Property
Public Property Get PreviewFontName() As String: PreviewFontName = This.PreviewFontName: End Property
Public Property Let PreviewFontName(ByVal Value As String): This.PreviewFontName = Trim$(Value): End Property
Public Property Get PreviewFontSize() As Long: PreviewFontSize = This.PreviewFontSize: End Property
Public Property Let PreviewFontSize(ByVal Value As Long): This.PreviewFontSize = Value: End Property
Public Property Get ShowLineNumbers() As Boolean: ShowLineNumbers = This.ShowLineNumbers: End Property
Public Property Let ShowLineNumbers(ByVal Value As Boolean): This.ShowLineNumbers = Value: End Property
Public Property Get PreserveBlankLines() As Boolean: PreserveBlankLines = This.PreserveBlankLines: End Property
Public Property Let PreserveBlankLines(ByVal Value As Boolean): This.PreserveBlankLines = Value: End Property
Public Property Get ConfirmOnUnsavedChanges() As Boolean: ConfirmOnUnsavedChanges = This.ConfirmOnUnsavedChanges: End Property
Public Property Let ConfirmOnUnsavedChanges(ByVal Value As Boolean): This.ConfirmOnUnsavedChanges = Value: End Property
Public Property Get LastProjectPath() As String: LastProjectPath = This.LastProjectPath: End Property
Public Property Let LastProjectPath(ByVal Value As String): This.LastProjectPath = Trim$(Value): End Property
Public Property Get LastSelectedModule() As String: LastSelectedModule = This.LastSelectedModule: End Property
Public Property Let LastSelectedModule(ByVal Value As String): This.LastSelectedModule = Trim$(Value): End Property
Public Property Get LastSelectedTemplate() As String: LastSelectedTemplate = This.LastSelectedTemplate: End Property
Public Property Let LastSelectedTemplate(ByVal Value As String): This.LastSelectedTemplate = Trim$(Value): End Property
Public Property Get WindowWidth() As Long: WindowWidth = This.WindowWidth: End Property
Public Property Let WindowWidth(ByVal Value As Long): This.WindowWidth = Value: End Property
Public Property Get WindowHeight() As Long: WindowHeight = This.WindowHeight: End Property
Public Property Let WindowHeight(ByVal Value As Long): This.WindowHeight = Value: End Property
Public Property Get ActivePage() As String: ActivePage = This.ActivePage: End Property
Public Property Let ActivePage(ByVal Value As String): This.ActivePage = Trim$(Value): End Property
