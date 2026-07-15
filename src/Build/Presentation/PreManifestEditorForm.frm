VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} PreManifestEditorForm 
   Caption         =   "VMF Studio"
   ClientHeight    =   6300
   ClientLeft      =   110
   ClientTop       =   450
   ClientWidth     =   9000
   OleObjectBlob   =   "PreManifestEditorForm.frx":0000
   StartUpPosition =   1  'オーナー フォームの中央
End
Attribute VB_Name = "PreManifestEditorForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' UserForm: PreManifestEditorForm
' Layer: Presentation
' Responsibility: Manifest editor view and user interaction.
'=========================================================================

Private Modules As Collection
Private SelectedModuleIndex As Long
Private SelectedMemberIndex As Long
Private WithEvents PreviewButton As MSForms.CommandButton
Private PreviewCodeTextBox As MSForms.TextBox
Private PreviewErrorTextBox As MSForms.TextBox
Private WithEvents PreviewRefreshButton As MSForms.CommandButton
Private WithEvents PreviewCopyButton As MSForms.CommandButton
Private WithEvents PreviewCloseButton As MSForms.CommandButton
Private WithEvents ValidationButton As MSForms.CommandButton
Private WithEvents ValidationListBox As MSForms.ListBox
Private WithEvents ValidationCloseButton As MSForms.CommandButton
Private ValidationIssues As Collection
Private WithEvents GenerateAllButton As MSForms.CommandButton
Private WithEvents GenerateSelectedButton As MSForms.CommandButton
Private WithEvents BuildLogClearButton As MSForms.CommandButton
Private WithEvents BuildLogOpenFolderButton As MSForms.CommandButton
Private BuildLogListBox As MSForms.ListBox
Private BuildLogSummaryLabel As MSForms.Label
Private LastGenerateResult As AppGenerateResult
Private GenerateInProgress As Boolean
Private IsDirty As Boolean
Private SuppressDirtyEvents As Boolean
Private CurrentUiState As String
Private ValidationStatusByModule As Object
Private GenerateTargetByModule As Object
Private Templates As Collection
Private SelectedTemplateIndex As Long
Private TemplateIssues As Collection
Private WithEvents TemplateManagerButton As MSForms.CommandButton
Private WithEvents TemplateListBox As MSForms.ListBox
Private WithEvents TemplateContentTextBox As MSForms.TextBox
Private WithEvents TemplateNewButton As MSForms.CommandButton
Private WithEvents TemplateOpenButton As MSForms.CommandButton
Private WithEvents TemplateSaveButton As MSForms.CommandButton
Private WithEvents TemplateSaveAsButton As MSForms.CommandButton
Private WithEvents TemplateReloadButton As MSForms.CommandButton
Private WithEvents TemplateValidateButton As MSForms.CommandButton
Private WithEvents TemplatePreviewButton As MSForms.CommandButton
Private WithEvents TemplateCloseButton As MSForms.CommandButton
Private TemplateAnalysisListBox As MSForms.ListBox
Private TemplateValidationListBox As MSForms.ListBox
Private SuppressTemplateEvents As Boolean
Private StudioSettings As AppStudioSettings
Private EditingStudioSettings As AppStudioSettings
Private SettingsIssues As Collection
Private WithEvents SettingsButton As MSForms.CommandButton
Private WithEvents SettingsPageListBox As MSForms.ListBox
Private WithEvents SettingsListBox As MSForms.ListBox
Private WithEvents SettingsValueTextBox As MSForms.TextBox
Private WithEvents SettingsBooleanCheckBox As MSForms.CheckBox
Private WithEvents SettingsOkButton As MSForms.CommandButton
Private WithEvents SettingsApplyButton As MSForms.CommandButton
Private WithEvents SettingsCancelButton As MSForms.CommandButton
Private WithEvents SettingsDefaultsButton As MSForms.CommandButton
Private WithEvents SettingsBrowseButton As MSForms.CommandButton
Private SettingsIssueListBox As MSForms.ListBox
Private SuppressSettingsEvents As Boolean
Private WithEvents BackupButton As MSForms.CommandButton
Private WithEvents BackupTypeListBox As MSForms.ListBox
Private WithEvents BackupListBox As MSForms.ListBox
Private WithEvents BackupHistoryListBox As MSForms.ListBox
Private WithEvents BackupRestoreButton As MSForms.CommandButton
Private WithEvents BackupDeleteButton As MSForms.CommandButton
Private WithEvents BackupOpenFolderButton As MSForms.CommandButton
Private WithEvents BackupRefreshButton As MSForms.CommandButton
Private WithEvents BackupCloseButton As MSForms.CommandButton
Private WithEvents SelfCheckButton As MSForms.CommandButton
Private WithEvents SelfCheckRunAllButton As MSForms.CommandButton
Private WithEvents SelfCheckRunSelectedButton As MSForms.CommandButton
Private WithEvents SelfCheckCategoryListBox As MSForms.ListBox
Private WithEvents SelfCheckResultListBox As MSForms.ListBox
Private WithEvents SelfCheckCopyButton As MSForms.CommandButton
Private WithEvents SelfCheckClearButton As MSForms.CommandButton
Private WithEvents SelfCheckCloseButton As MSForms.CommandButton
Private SelfCheckDetailTextBox As MSForms.TextBox
Private SelfCheckResults As Collection
Private SelfCheckInProgress As Boolean
Public Sub PreOpenManifest(ByVal ManifestPath As String)
    txtManifestPath.Text = ManifestPath
    LoadManifest
End Sub

Private Sub UserForm_Initialize()
    SelectedModuleIndex = 0
    SelectedMemberIndex = 0
    Set Modules = New Collection
    Set ValidationStatusByModule = CreateObject("Scripting.Dictionary")
    Set GenerateTargetByModule = CreateObject("Scripting.Dictionary")
    Set Templates = New Collection
    SelectedTemplateIndex = 0
    LoadStudioSettingsState
    CurrentUiState = "NoProject"
    IsDirty = False

    Me.Width = StudioSettings.WindowWidth
    Me.Height = StudioSettings.WindowHeight
    lblTemplatePath.Width = 78
    txtTemplatePath.Left = lblTemplatePath.Left + lblTemplatePath.Width + 8
    txtTemplatePath.Width = 196
    btnModuleAdd.Height = 22
    btnModuleDelete.Height = 22
    btnModuleApply.Height = 22
    btnLoad.Caption = "Browse..."
    CreatePreviewButton
    CreateValidationButton
    CreateGenerateButtons
    CreateTemplateManagerButton
    CreateSettingsButton
    CreateBackupButton
    CreateSelfCheckButton

    cboModuleType.Clear
    cboModuleType.AddItem "ClassModule"
    cboModuleType.AddItem "StandardModule"

    cboAccessor.Clear
    cboAccessor.AddItem "GetLet"
    cboAccessor.AddItem "GetSet"
    cboAccessor.AddItem "GetOnly"

    lstModules.ColumnCount = 6
    lstModules.ColumnWidths = "42 pt;80 pt;140 pt;90 pt;48 pt;58 pt"
    lstMembers.ColumnCount = 5
    lstMembers.ColumnWidths = "110 pt;110 pt;80 pt;110 pt;80 pt"
    RefreshCommandState
End Sub

Private Sub btnLoad_Click()
    Dim SelectedPath As Variant

    SelectedPath = Application.GetOpenFilename( _
        "Manifest Files (*.manifest),*.manifest,All Files (*.*),*.*", _
        , _
        "Select Manifest")
    If VarType(SelectedPath) = vbBoolean Then
        If Not CBool(SelectedPath) Then
            Exit Sub
        End If
    End If

    txtManifestPath.Text = CStr(SelectedPath)
    LoadManifest
End Sub

Private Sub btnSave_Click()
    Dim Result As ComResult

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Result = ValidateProjectModel()
    If AppValidationIssuesContainErrors(ValidationIssues) Then
        ShowValidationPane ValidationIssues
        MsgBox Result.Message, vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    Set Result = AppSaveManifestEditorModel(txtManifestPath.Text, Modules)
    If Result.IsSuccess Then
        IsDirty = False
        CurrentUiState = "Ready"
        RefreshProjectExplorer
        RefreshCommandState
    End If
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Manifest Editor"
End Sub

Private Sub btnClose_Click()
    Unload Me
End Sub

Private Sub lstModules_Click()
    If lstModules.ListIndex < 0 Then
        Exit Sub
    End If

    SelectModuleFromExplorer lstModules.ListIndex + 1
End Sub

Private Sub lstMembers_Click()
    If lstMembers.ListIndex < 0 Then
        Exit Sub
    End If

    SelectedMemberIndex = lstMembers.ListIndex + 1
    ShowSelectedMember
End Sub

Private Sub btnModuleAdd_Click()
    Dim ModuleInfo As Object

    Set ModuleInfo = AppCreateManifestEditorModule( _
        txtModuleName.Text, _
        txtLayer.Text, _
        cboModuleType.Text, _
        txtTemplatePath.Text)

    Modules.Add ModuleInfo
    SelectedModuleIndex = Modules.Count
    SetSingleGenerateTarget CStr(ModuleInfo("ModuleName"))
    MarkDirty
    RefreshProjectExplorer
    lstModules.ListIndex = SelectedModuleIndex - 1
    ShowSelectedModule
End Sub

Private Sub btnModuleApply_Click()
    ApplyModuleFieldsToSelection
    MarkDirty
    RefreshProjectExplorer
End Sub

Private Sub PreviewButton_Click()
    Dim Result As ComResult

    If SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Result = ValidateSelectedModule()
    If AppValidationIssuesContainErrors(ValidationIssues) Then
        ShowValidationPane ValidationIssues
        MsgBox Result.Message, vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    ShowPreviewPane
End Sub

Private Sub ValidationButton_Click()
    Dim Result As ComResult

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Result = ValidateProjectModel()
    ShowValidationPane ValidationIssues
End Sub

Private Sub ValidationCloseButton_Click()
    HideValidationPane
End Sub

Private Sub ValidationListBox_DblClick(ByVal Cancel As MSForms.ReturnBoolean)
    NavigateToValidationIssue
End Sub

Private Sub GenerateAllButton_Click()
    GenerateFromEditor "AllModules"
End Sub

Private Sub GenerateSelectedButton_Click()
    GenerateFromEditor "CurrentModule"
End Sub

Private Sub BuildLogClearButton_Click()
    EnsureBuildLogControls
    BuildLogListBox.Clear
    BuildLogSummaryLabel.Caption = "Success: 0  Warning: 0  Failed: 0"
End Sub

Private Sub BuildLogOpenFolderButton_Click()
    If LastGenerateResult Is Nothing Then
        MsgBox "No output folder is available.", vbInformation, "Manifest Editor"
        Exit Sub
    End If

    Shell "explorer.exe " & Chr$(34) & LastGenerateResult.OutputDirectory & Chr$(34), vbNormalFocus
End Sub
Private Sub PreviewRefreshButton_Click()
    RefreshPreviewPane
End Sub

Private Sub PreviewCopyButton_Click()
    CopyPreviewText
End Sub

Private Sub PreviewCloseButton_Click()
    HidePreviewPane
End Sub

Private Sub btnModuleDelete_Click()
    If SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    Modules.Remove SelectedModuleIndex
    If SelectedModuleIndex > Modules.Count Then
        SelectedModuleIndex = Modules.Count
    End If

    RefreshProjectExplorer
    If SelectedModuleIndex > 0 Then
        lstModules.ListIndex = SelectedModuleIndex - 1
        ShowSelectedModule
    Else
        ClearModuleFields
        RefreshMemberList Nothing
    End If
End Sub

Private Sub btnMemberAdd_Click()
    Dim ModuleInfo As Object
    Dim MemberInfo As Object
    Dim Members As Collection

    If SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Members = ModuleInfo("Members")
    Set MemberInfo = CreateMemberFromFields()
    Members.Add MemberInfo
    SelectedMemberIndex = Members.Count
    RefreshMemberList ModuleInfo
    lstMembers.ListIndex = SelectedMemberIndex - 1
End Sub

Private Sub btnMemberEdit_Click()
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object

    If SelectedModuleIndex <= 0 Or SelectedMemberIndex <= 0 Then
        MsgBox "Select a member first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Members = ModuleInfo("Members")
    Set MemberInfo = CreateMemberFromFields()
    Members.Remove SelectedMemberIndex
    If SelectedMemberIndex > Members.Count Then
        Members.Add MemberInfo
    Else
        Members.Add MemberInfo, , SelectedMemberIndex
    End If

    RefreshMemberList ModuleInfo
    lstMembers.ListIndex = SelectedMemberIndex - 1
End Sub

Private Sub btnMemberDelete_Click()
    Dim ModuleInfo As Object
    Dim Members As Collection

    If SelectedModuleIndex <= 0 Or SelectedMemberIndex <= 0 Then
        MsgBox "Select a member first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Members = ModuleInfo("Members")
    Members.Remove SelectedMemberIndex
    If SelectedMemberIndex > Members.Count Then
        SelectedMemberIndex = Members.Count
    End If

    RefreshMemberList ModuleInfo
    If SelectedMemberIndex > 0 Then
        lstMembers.ListIndex = SelectedMemberIndex - 1
        ShowSelectedMember
    Else
        ClearMemberFields
    End If
End Sub

Private Sub LoadManifest()
    Dim Result As ComResult

    SetUiState "Loading"
    SelectedModuleIndex = 0
    SelectedMemberIndex = 0
    Set Result = AppLoadManifestEditorModel(txtManifestPath.Text, Modules)
    If Result.IsFailure Then
        SetUiState "Error"
        MsgBox Result.Message, vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    IsDirty = False
    ResetStudioState
    RefreshProjectExplorer
    If Modules.Count > 0 Then
        lstModules.ListIndex = 0
        SelectModuleFromExplorer 1
    End If
End Sub

Private Sub ApplyModuleFieldsToSelection()
    Dim ModuleInfo As Object

    If SelectedModuleIndex <= 0 Then
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    ModuleInfo("ModuleName") = Trim$(txtModuleName.Text)
    ModuleInfo("Layer") = Trim$(txtLayer.Text)
    ModuleInfo("ModuleType") = Trim$(cboModuleType.Text)
    ModuleInfo("TemplatePath") = Trim$(txtTemplatePath.Text)
End Sub

Private Sub ApplyMemberFieldsToSelection()
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object

    If SelectedModuleIndex <= 0 Or SelectedMemberIndex <= 0 Then
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Members = ModuleInfo("Members")
    If SelectedMemberIndex > Members.Count Then
        Exit Sub
    End If

    Set MemberInfo = CreateMemberFromFields()
    Members.Remove SelectedMemberIndex
    If SelectedMemberIndex > Members.Count Then
        Members.Add MemberInfo
    Else
        Members.Add MemberInfo, , SelectedMemberIndex
    End If
End Sub

Private Sub CreatePreviewButton()
    On Error Resume Next
    Set PreviewButton = Me.Controls("btnPreview")
    On Error GoTo 0

    If PreviewButton Is Nothing Then
        Set PreviewButton = Me.Controls.Add("Forms.CommandButton.1", "btnPreview", True)
    End If

    PreviewButton.Caption = "Preview"
    PreviewButton.Width = 72
    PreviewButton.Height = 22
    PreviewButton.Left = btnModuleApply.Left + btnModuleApply.Width + 8
    PreviewButton.Top = btnModuleApply.Top
End Sub

Private Sub CreateValidationButton()
    On Error Resume Next
    Set ValidationButton = Me.Controls("btnValidate")
    On Error GoTo 0

    If ValidationButton Is Nothing Then
        Set ValidationButton = Me.Controls.Add("Forms.CommandButton.1", "btnValidate", True)
    End If

    ValidationButton.Caption = "Validate"
    ValidationButton.Width = 72
    ValidationButton.Height = 22
    ValidationButton.Left = PreviewButton.Left + PreviewButton.Width + 8
    ValidationButton.Top = PreviewButton.Top
End Sub

Private Sub CreateGenerateButtons()
    On Error Resume Next
    Set GenerateAllButton = Me.Controls("btnGenerateAll")
    Set GenerateSelectedButton = Me.Controls("btnGenerateSelected")
    On Error GoTo 0

    If GenerateAllButton Is Nothing Then
        Set GenerateAllButton = Me.Controls.Add("Forms.CommandButton.1", "btnGenerateAll", True)
    End If
    GenerateAllButton.Caption = "Generate All"
    GenerateAllButton.Width = 92
    GenerateAllButton.Height = 22
    GenerateAllButton.Left = ValidationButton.Left + ValidationButton.Width + 8
    GenerateAllButton.Top = ValidationButton.Top

    If GenerateSelectedButton Is Nothing Then
        Set GenerateSelectedButton = Me.Controls.Add("Forms.CommandButton.1", "btnGenerateSelected", True)
    End If
    GenerateSelectedButton.Caption = "Generate Selected"
    GenerateSelectedButton.Width = 118
    GenerateSelectedButton.Height = 22
    GenerateSelectedButton.Left = GenerateAllButton.Left + GenerateAllButton.Width + 8
    GenerateSelectedButton.Top = GenerateAllButton.Top
End Sub

Private Sub CreateTemplateManagerButton()
    On Error Resume Next
    Set TemplateManagerButton = Me.Controls("btnTemplateManager")
    On Error GoTo 0

    If TemplateManagerButton Is Nothing Then
        Set TemplateManagerButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateManager", True)
    End If

    TemplateManagerButton.Caption = "Templates"
    TemplateManagerButton.Width = 82
    TemplateManagerButton.Height = 22
    TemplateManagerButton.Left = GenerateSelectedButton.Left + GenerateSelectedButton.Width + 8
    TemplateManagerButton.Top = GenerateSelectedButton.Top
End Sub

Private Sub TemplateManagerButton_Click()
    ShowTemplateManagerPane
End Sub

Private Sub TemplateListBox_Click()
    If TemplateListBox.ListIndex < 0 Then Exit Sub
    SelectTemplate TemplateListBox.ListIndex + 1
End Sub

Private Sub TemplateContentTextBox_Change()
    If SuppressTemplateEvents Then Exit Sub
    If SelectedTemplateIndex <= 0 Then Exit Sub

    Templates.Item(SelectedTemplateIndex).Content = TemplateContentTextBox.Text
    RenderTemplateList
End Sub

Private Sub TemplateNewButton_Click()
    Dim Template As AppTemplateModel
    Dim FilePath As String

    FilePath = AppDefaultTemplateManagerFilePath("NewTemplate.txt")
    Set Template = AppCreateTemplateManagerTemplate("NewTemplate", "Class", FilePath, _
        "Option Explicit" & vbCrLf & _
        "' Class: {{ModuleName}}" & vbCrLf & _
        "' Layer: {{Layer}}" & vbCrLf & _
        "{{BODY}}")
    Templates.Add Template
    RenderTemplateList
    TemplateListBox.ListIndex = Templates.Count - 1
    SelectTemplate Templates.Count
End Sub

Private Sub TemplateOpenButton_Click()
    Dim SelectedPath As Variant
    Dim Template As AppTemplateModel
    Dim Result As ComResult

    SelectedPath = Application.GetOpenFilename("Template Files (*.txt),*.txt,All Files (*.*),*.*", , "Open Template")
    If VarType(SelectedPath) = vbBoolean Then
        If Not CBool(SelectedPath) Then Exit Sub
    End If

    Set Result = AppLoadTemplateManagerTemplateFile(CStr(SelectedPath), Template)
    If Result.IsFailure Then
        MsgBox Result.Message, vbExclamation, "Template Manager"
        Exit Sub
    End If

    Templates.Add Template
    RenderTemplateList
    TemplateListBox.ListIndex = Templates.Count - 1
    SelectTemplate Templates.Count
End Sub

Private Sub TemplateSaveButton_Click()
    SaveSelectedTemplate False
End Sub

Private Sub TemplateSaveAsButton_Click()
    SaveSelectedTemplate True
End Sub

Private Sub TemplateReloadButton_Click()
    If SelectedTemplateIndex <= 0 Then Exit Sub
    LoadTemplateManagerTemplates
End Sub

Private Sub TemplateValidateButton_Click()
    ValidateSelectedTemplate
End Sub

Private Sub TemplatePreviewButton_Click()
    PreviewSelectedTemplate
End Sub

Private Sub TemplateCloseButton_Click()
    HideTemplateManagerPane
End Sub

Private Sub CreateSelfCheckButton()
    On Error Resume Next
    Set SelfCheckButton = Me.Controls("btnSelfCheck")
    On Error GoTo 0

    If SelfCheckButton Is Nothing Then
        Set SelfCheckButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheck", True)
    End If

    SelfCheckButton.Caption = "Self Check"
    SelfCheckButton.Width = 84
    SelfCheckButton.Height = 22
    SelfCheckButton.Left = BackupButton.Left + BackupButton.Width + 8
    SelfCheckButton.Top = BackupButton.Top
End Sub

Private Sub SelfCheckButton_Click()
    ShowSelfCheckPane
End Sub

Private Sub SelfCheckRunAllButton_Click()
    RunSelfChecks False
End Sub

Private Sub SelfCheckRunSelectedButton_Click()
    RunSelfChecks True
End Sub

Private Sub SelfCheckResultListBox_Click()
    ShowSelectedSelfCheckDetail
End Sub

Private Sub SelfCheckCopyButton_Click()
    CopySelfCheckResults
End Sub

Private Sub SelfCheckClearButton_Click()
    Set SelfCheckResults = New Collection
    RenderSelfCheckResults
    SelfCheckDetailTextBox.Text = vbNullString
End Sub

Private Sub SelfCheckCloseButton_Click()
    HideSelfCheckPane
End Sub

Private Sub ShowSelfCheckPane()
    HideBackupPane
    HideSettingsPane
    HideTemplateManagerPane
    HidePreviewPane
    HideValidationPane
    HideBuildLogPane
    EnsureSelfCheckControls
    Me.Width = 1120
    SelfCheckCategoryListBox.Visible = True
    SelfCheckResultListBox.Visible = True
    SelfCheckDetailTextBox.Visible = True
    SelfCheckRunAllButton.Visible = True
    SelfCheckRunSelectedButton.Visible = True
    SelfCheckCopyButton.Visible = True
    SelfCheckClearButton.Visible = True
    SelfCheckCloseButton.Visible = True
    If SelfCheckCategoryListBox.ListIndex < 0 Then SelfCheckCategoryListBox.ListIndex = 0
    RenderSelfCheckResults
End Sub

Private Sub HideSelfCheckPane()
    If SelfCheckCategoryListBox Is Nothing Then Exit Sub
    SelfCheckCategoryListBox.Visible = False
    SelfCheckResultListBox.Visible = False
    SelfCheckDetailTextBox.Visible = False
    SelfCheckRunAllButton.Visible = False
    SelfCheckRunSelectedButton.Visible = False
    SelfCheckCopyButton.Visible = False
    SelfCheckClearButton.Visible = False
    SelfCheckCloseButton.Visible = False
    Me.Width = 620
End Sub

Private Sub EnsureSelfCheckControls()
    If Not SelfCheckCategoryListBox Is Nothing Then Exit Sub

    Set SelfCheckCategoryListBox = Me.Controls.Add("Forms.ListBox.1", "lstSelfCheckCategories", True)
    SelfCheckCategoryListBox.Left = 620
    SelfCheckCategoryListBox.Top = 30
    SelfCheckCategoryListBox.Width = 112
    SelfCheckCategoryListBox.Height = 138
    SelfCheckCategoryListBox.AddItem "All"
    SelfCheckCategoryListBox.AddItem "Self Check"
    SelfCheckCategoryListBox.AddItem "Manifest"
    SelfCheckCategoryListBox.AddItem "Template"
    SelfCheckCategoryListBox.AddItem "Preview/Generate"
    SelfCheckCategoryListBox.AddItem "Validation"
    SelfCheckCategoryListBox.AddItem "Settings"
    SelfCheckCategoryListBox.AddItem "Backup/Restore"
    SelfCheckCategoryListBox.Visible = False

    Set SelfCheckResultListBox = Me.Controls.Add("Forms.ListBox.1", "lstSelfCheckResults", True)
    SelfCheckResultListBox.Left = 740
    SelfCheckResultListBox.Top = 30
    SelfCheckResultListBox.Width = 350
    SelfCheckResultListBox.Height = 230
    SelfCheckResultListBox.ColumnCount = 6
    SelfCheckResultListBox.ColumnWidths = "48 pt;82 pt;120 pt;58 pt;180 pt;54 pt"
    SelfCheckResultListBox.Visible = False

    Set SelfCheckDetailTextBox = Me.Controls.Add("Forms.TextBox.1", "txtSelfCheckDetail", True)
    SelfCheckDetailTextBox.Left = 740
    SelfCheckDetailTextBox.Top = 268
    SelfCheckDetailTextBox.Width = 350
    SelfCheckDetailTextBox.Height = 132
    SelfCheckDetailTextBox.MultiLine = True
    SelfCheckDetailTextBox.ScrollBars = fmScrollBarsVertical
    SelfCheckDetailTextBox.Locked = True
    SelfCheckDetailTextBox.Visible = False

    Set SelfCheckRunAllButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheckRunAll", True)
    SelfCheckRunAllButton.Caption = "Run All"
    SelfCheckRunAllButton.Left = 740
    SelfCheckRunAllButton.Top = 414
    SelfCheckRunAllButton.Width = 70
    SelfCheckRunAllButton.Height = 24
    SelfCheckRunAllButton.Visible = False

    Set SelfCheckRunSelectedButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheckRunSelected", True)
    SelfCheckRunSelectedButton.Caption = "Run Selected"
    SelfCheckRunSelectedButton.Left = 818
    SelfCheckRunSelectedButton.Top = 414
    SelfCheckRunSelectedButton.Width = 96
    SelfCheckRunSelectedButton.Height = 24
    SelfCheckRunSelectedButton.Visible = False

    Set SelfCheckCopyButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheckCopy", True)
    SelfCheckCopyButton.Caption = "Copy"
    SelfCheckCopyButton.Left = 922
    SelfCheckCopyButton.Top = 414
    SelfCheckCopyButton.Width = 58
    SelfCheckCopyButton.Height = 24
    SelfCheckCopyButton.Visible = False

    Set SelfCheckClearButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheckClear", True)
    SelfCheckClearButton.Caption = "Clear"
    SelfCheckClearButton.Left = 988
    SelfCheckClearButton.Top = 414
    SelfCheckClearButton.Width = 56
    SelfCheckClearButton.Height = 24
    SelfCheckClearButton.Visible = False

    Set SelfCheckCloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnSelfCheckClose", True)
    SelfCheckCloseButton.Caption = "Close"
    SelfCheckCloseButton.Left = 1050
    SelfCheckCloseButton.Top = 414
    SelfCheckCloseButton.Width = 56
    SelfCheckCloseButton.Height = 24
    SelfCheckCloseButton.Visible = False

    Set SelfCheckResults = New Collection
End Sub

Private Sub RunSelfChecks(ByVal SelectedOnly As Boolean)
    Dim CategoryName As String
    If SelfCheckInProgress Then
        MsgBox "Self Check is already running.", vbInformation, "Self Check"
        Exit Sub
    End If

    SelfCheckInProgress = True
    SelfCheckRunAllButton.Enabled = False
    SelfCheckRunSelectedButton.Enabled = False
    On Error GoTo RunFailed
    If SelectedOnly Then
        If SelfCheckCategoryListBox.ListIndex < 0 Or SelfCheckCategoryListBox.List(SelfCheckCategoryListBox.ListIndex) = "All" Then
            Set SelfCheckResults = AppRunStudioSelfChecks()
        Else
            CategoryName = SelfCheckCategoryListBox.List(SelfCheckCategoryListBox.ListIndex)
            Set SelfCheckResults = AppRunStudioSelfChecksByCategory(CategoryName)
        End If
    Else
        Set SelfCheckResults = AppRunStudioSelfChecks()
    End If
    RenderSelfCheckResults

RunDone:
    SelfCheckInProgress = False
    SelfCheckRunAllButton.Enabled = True
    SelfCheckRunSelectedButton.Enabled = True
    Exit Sub

RunFailed:
    MsgBox Err.Description, vbExclamation, "Self Check"
    Resume RunDone
End Sub

Private Sub RenderSelfCheckResults()
    Dim Result As AppSelfCheckResult
    Dim RowIndex As Long
    EnsureSelfCheckControls
    SelfCheckResultListBox.Clear
    If SelfCheckResults Is Nothing Then Set SelfCheckResults = New Collection
    For Each Result In SelfCheckResults
        SelfCheckResultListBox.AddItem Result.TestID
        RowIndex = SelfCheckResultListBox.ListCount - 1
        SelfCheckResultListBox.List(RowIndex, 1) = Result.Category
        SelfCheckResultListBox.List(RowIndex, 2) = Result.TestName
        SelfCheckResultListBox.List(RowIndex, 3) = Result.Status
        SelfCheckResultListBox.List(RowIndex, 4) = Result.Message
        SelfCheckResultListBox.List(RowIndex, 5) = Format$(Result.ElapsedTime, "0.0")
    Next Result
    If SelfCheckResultListBox.ListCount > 0 Then SelfCheckResultListBox.ListIndex = 0
    ShowSelectedSelfCheckDetail
End Sub

Private Sub ShowSelectedSelfCheckDetail()
    Dim Result As AppSelfCheckResult
    If SelfCheckResults Is Nothing Then Exit Sub
    If SelfCheckResultListBox.ListIndex < 0 Then Exit Sub
    If SelfCheckResultListBox.ListIndex + 1 > SelfCheckResults.Count Then Exit Sub
    Set Result = SelfCheckResults.Item(SelfCheckResultListBox.ListIndex + 1)
    SelfCheckDetailTextBox.Text = _
        "TestID: " & Result.TestID & vbCrLf & _
        "Category: " & Result.Category & vbCrLf & _
        "Name: " & Result.TestName & vbCrLf & _
        "Status: " & Result.Status & vbCrLf & _
        "Message: " & Result.Message & vbCrLf & _
        "Expected: " & Result.Expected & vbCrLf & _
        "Actual: " & Result.Actual & vbCrLf & _
        "ErrorCode: " & Result.ErrorCode
End Sub

Private Sub CopySelfCheckResults()
    Dim Clipboard As Object
    Dim Result As AppSelfCheckResult
    Dim TextValue As String
    If SelfCheckResults Is Nothing Then Exit Sub
    For Each Result In SelfCheckResults
        TextValue = TextValue & Result.TestID & vbTab & Result.Category & vbTab & Result.TestName & vbTab & Result.Status & vbTab & Result.Message & vbCrLf
    Next Result
    Set Clipboard = CreateObject("Forms.DataObject.1")
    Clipboard.SetText TextValue
    Clipboard.PutInClipboard
    SelfCheckDetailTextBox.Text = "Self Check results copied."
End Sub

Private Sub CreateBackupButton()
    On Error Resume Next
    Set BackupButton = Me.Controls("btnBackups")
    On Error GoTo 0

    If BackupButton Is Nothing Then
        Set BackupButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackups", True)
    End If

    BackupButton.Caption = "Backups"
    BackupButton.Width = 74
    BackupButton.Height = 22
    BackupButton.Left = SettingsButton.Left + SettingsButton.Width + 8
    BackupButton.Top = SettingsButton.Top
End Sub

Private Sub BackupButton_Click()
    ShowBackupPane
End Sub

Private Sub BackupTypeListBox_Click()
    RenderBackupList
End Sub

Private Sub BackupRefreshButton_Click()
    RenderBackupList
    RenderBackupHistory
End Sub

Private Sub BackupRestoreButton_Click()
    RestoreSelectedBackup
End Sub

Private Sub BackupDeleteButton_Click()
    DeleteSelectedBackup
End Sub

Private Sub BackupOpenFolderButton_Click()
    Shell "explorer.exe " & Chr$(34) & AppResolveStudioSettingsPath(BuildPathResolver.RepositoryRootPath(), StudioSettings.BackupDirectory) & Chr$(34), vbNormalFocus
End Sub

Private Sub BackupCloseButton_Click()
    HideBackupPane
End Sub

Private Sub ShowBackupPane()
    HideSettingsPane
    HideTemplateManagerPane
    HidePreviewPane
    HideValidationPane
    HideBuildLogPane
    EnsureBackupControls
    Me.Width = 1120
    BackupTypeListBox.Visible = True
    BackupListBox.Visible = True
    BackupHistoryListBox.Visible = True
    BackupRestoreButton.Visible = True
    BackupDeleteButton.Visible = True
    BackupOpenFolderButton.Visible = True
    BackupRefreshButton.Visible = True
    BackupCloseButton.Visible = True
    If BackupTypeListBox.ListIndex < 0 Then BackupTypeListBox.ListIndex = 0
    RenderBackupList
    RenderBackupHistory
End Sub

Private Sub HideBackupPane()
    If BackupTypeListBox Is Nothing Then Exit Sub
    BackupTypeListBox.Visible = False
    BackupListBox.Visible = False
    BackupHistoryListBox.Visible = False
    BackupRestoreButton.Visible = False
    BackupDeleteButton.Visible = False
    BackupOpenFolderButton.Visible = False
    BackupRefreshButton.Visible = False
    BackupCloseButton.Visible = False
    Me.Width = 620
End Sub

Private Sub EnsureBackupControls()
    If Not BackupTypeListBox Is Nothing Then Exit Sub

    Set BackupTypeListBox = Me.Controls.Add("Forms.ListBox.1", "lstBackupTypes", True)
    BackupTypeListBox.Left = 620
    BackupTypeListBox.Top = 30
    BackupTypeListBox.Width = 86
    BackupTypeListBox.Height = 92
    BackupTypeListBox.AddItem "All"
    BackupTypeListBox.AddItem "Manifest"
    BackupTypeListBox.AddItem "Template"
    BackupTypeListBox.AddItem "Settings"
    BackupTypeListBox.Visible = False

    Set BackupListBox = Me.Controls.Add("Forms.ListBox.1", "lstBackups", True)
    BackupListBox.Left = 714
    BackupListBox.Top = 30
    BackupListBox.Width = 376
    BackupListBox.Height = 190
    BackupListBox.ColumnCount = 6
    BackupListBox.ColumnWidths = "70 pt;62 pt;110 pt;90 pt;54 pt;220 pt"
    BackupListBox.Visible = False

    Set BackupHistoryListBox = Me.Controls.Add("Forms.ListBox.1", "lstBackupHistory", True)
    BackupHistoryListBox.Left = 714
    BackupHistoryListBox.Top = 228
    BackupHistoryListBox.Width = 376
    BackupHistoryListBox.Height = 180
    BackupHistoryListBox.ColumnCount = 6
    BackupHistoryListBox.ColumnWidths = "70 pt;62 pt;72 pt;78 pt;58 pt;180 pt"
    BackupHistoryListBox.Visible = False

    Set BackupRestoreButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackupRestore", True)
    BackupRestoreButton.Caption = "Restore"
    BackupRestoreButton.Left = 714
    BackupRestoreButton.Top = 420
    BackupRestoreButton.Width = 72
    BackupRestoreButton.Height = 24
    BackupRestoreButton.Visible = False

    Set BackupDeleteButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackupDelete", True)
    BackupDeleteButton.Caption = "Delete"
    BackupDeleteButton.Left = 794
    BackupDeleteButton.Top = 420
    BackupDeleteButton.Width = 66
    BackupDeleteButton.Height = 24
    BackupDeleteButton.Visible = False

    Set BackupOpenFolderButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackupOpenFolder", True)
    BackupOpenFolderButton.Caption = "Open Folder"
    BackupOpenFolderButton.Left = 868
    BackupOpenFolderButton.Top = 420
    BackupOpenFolderButton.Width = 92
    BackupOpenFolderButton.Height = 24
    BackupOpenFolderButton.Visible = False

    Set BackupRefreshButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackupRefresh", True)
    BackupRefreshButton.Caption = "Refresh"
    BackupRefreshButton.Left = 968
    BackupRefreshButton.Top = 420
    BackupRefreshButton.Width = 68
    BackupRefreshButton.Height = 24
    BackupRefreshButton.Visible = False

    Set BackupCloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnBackupClose", True)
    BackupCloseButton.Caption = "Close"
    BackupCloseButton.Left = 1042
    BackupCloseButton.Top = 420
    BackupCloseButton.Width = 58
    BackupCloseButton.Height = 24
    BackupCloseButton.Visible = False
End Sub

Private Sub RenderBackupList()
    Dim FilterText As String
    Dim Backups As Collection
    Dim Backup As AppBackupEntry
    Dim RowIndex As Long

    EnsureBackupControls
    BackupListBox.Clear
    If BackupTypeListBox.ListIndex < 0 Or BackupTypeListBox.List(BackupTypeListBox.ListIndex) = "All" Then
        FilterText = vbNullString
    Else
        FilterText = BackupTypeListBox.List(BackupTypeListBox.ListIndex)
    End If
    Set Backups = AppListBackups(FilterText)
    For Each Backup In Backups
        BackupListBox.AddItem Backup.BackupID
        RowIndex = BackupListBox.ListCount - 1
        BackupListBox.List(RowIndex, 1) = Backup.TargetType
        BackupListBox.List(RowIndex, 2) = Backup.TargetName
        BackupListBox.List(RowIndex, 3) = Format$(Backup.CreatedAt, "yyyy/mm/dd hh:nn:ss")
        BackupListBox.List(RowIndex, 4) = CStr(Backup.SizeBytes)
        BackupListBox.List(RowIndex, 5) = Backup.BackupPath
    Next Backup
End Sub

Private Sub RenderBackupHistory()
    Dim HistoryItems As Collection
    Dim Item As AppBackupHistoryEntry
    Dim RowIndex As Long
    EnsureBackupControls
    BackupHistoryListBox.Clear
    Set HistoryItems = AppListBackupHistory()
    For Each Item In HistoryItems
        BackupHistoryListBox.AddItem Format$(Item.Timestamp, "yyyy/mm/dd hh:nn:ss")
        RowIndex = BackupHistoryListBox.ListCount - 1
        BackupHistoryListBox.List(RowIndex, 1) = Item.TargetType
        BackupHistoryListBox.List(RowIndex, 2) = Item.TargetName
        BackupHistoryListBox.List(RowIndex, 3) = Item.Action
        BackupHistoryListBox.List(RowIndex, 4) = Item.Result
        BackupHistoryListBox.List(RowIndex, 5) = Item.Message
    Next Item
End Sub

Private Sub RestoreSelectedBackup()
    Dim Result As ComResult
    If BackupListBox.ListIndex < 0 Then Exit Sub
    If MsgBox("Restore selected backup? Current file will be backed up first.", vbOKCancel + vbQuestion, "Backups") <> vbOK Then Exit Sub
    Set Result = AppRestoreBackup(BackupListBox.List(BackupListBox.ListIndex, 5))
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Backups"
    RenderBackupList
    RenderBackupHistory
End Sub

Private Sub DeleteSelectedBackup()
    Dim Result As ComResult
    If BackupListBox.ListIndex < 0 Then Exit Sub
    If MsgBox("Delete selected backup?", vbOKCancel + vbQuestion, "Backups") <> vbOK Then Exit Sub
    Set Result = AppDeleteBackup(BackupListBox.List(BackupListBox.ListIndex, 5))
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Backups"
    RenderBackupList
    RenderBackupHistory
End Sub

Private Sub LoadStudioSettingsState()
    Dim Result As ComResult
    Set Result = AppLoadStudioSettings(StudioSettings, SettingsIssues)
    If StudioSettings Is Nothing Then Set StudioSettings = AppCreateDefaultStudioSettings()
End Sub

Private Sub CreateSettingsButton()
    On Error Resume Next
    Set SettingsButton = Me.Controls("btnSettings")
    On Error GoTo 0

    If SettingsButton Is Nothing Then
        Set SettingsButton = Me.Controls.Add("Forms.CommandButton.1", "btnSettings", True)
    End If

    SettingsButton.Caption = "Settings"
    SettingsButton.Width = 74
    SettingsButton.Height = 22
    SettingsButton.Left = TemplateManagerButton.Left + TemplateManagerButton.Width + 8
    SettingsButton.Top = TemplateManagerButton.Top
End Sub

Private Sub SettingsButton_Click()
    ShowSettingsPane
End Sub

Private Sub SettingsPageListBox_Click()
    RenderSettingsList
End Sub

Private Sub SettingsListBox_Click()
    ShowSelectedSettingValue
End Sub

Private Sub SettingsValueTextBox_Change()
    If SuppressSettingsEvents Then Exit Sub
    ApplySelectedSettingValue SettingsValueTextBox.Text
End Sub

Private Sub SettingsBooleanCheckBox_Click()
    If SuppressSettingsEvents Then Exit Sub
    ApplySelectedSettingValue CStr(SettingsBooleanCheckBox.Value)
End Sub

Private Sub SettingsOkButton_Click()
    If ApplySettingsChanges() Then HideSettingsPane
End Sub

Private Sub SettingsApplyButton_Click()
    ApplySettingsChanges
End Sub

Private Sub SettingsCancelButton_Click()
    HideSettingsPane
End Sub

Private Sub SettingsDefaultsButton_Click()
    Set EditingStudioSettings = AppCreateDefaultStudioSettings()
    RenderSettingsList
End Sub

Private Sub SettingsBrowseButton_Click()
    BrowseSelectedSettingPath
End Sub

Private Sub ShowSettingsPane()
    HidePreviewPane
    HideValidationPane
    HideBuildLogPane
    HideTemplateManagerPane
    EnsureSettingsControls
    Set EditingStudioSettings = StudioSettings.AppClone()
    Me.Width = 1120
    SettingsPageListBox.Visible = True
    SettingsListBox.Visible = True
    SettingsValueTextBox.Visible = True
    SettingsBooleanCheckBox.Visible = True
    SettingsIssueListBox.Visible = True
    SettingsOkButton.Visible = True
    SettingsApplyButton.Visible = True
    SettingsCancelButton.Visible = True
    SettingsDefaultsButton.Visible = True
    SettingsBrowseButton.Visible = True
    If SettingsPageListBox.ListIndex < 0 Then SettingsPageListBox.ListIndex = 0
    RenderSettingsList
End Sub

Private Sub HideSettingsPane()
    If SettingsPageListBox Is Nothing Then Exit Sub
    SettingsPageListBox.Visible = False
    SettingsListBox.Visible = False
    SettingsValueTextBox.Visible = False
    SettingsBooleanCheckBox.Visible = False
    SettingsIssueListBox.Visible = False
    SettingsOkButton.Visible = False
    SettingsApplyButton.Visible = False
    SettingsCancelButton.Visible = False
    SettingsDefaultsButton.Visible = False
    SettingsBrowseButton.Visible = False
    Me.Width = 620
End Sub

Private Sub EnsureSettingsControls()
    If Not SettingsPageListBox Is Nothing Then Exit Sub

    Set SettingsPageListBox = Me.Controls.Add("Forms.ListBox.1", "lstSettingsPages", True)
    SettingsPageListBox.Left = 620
    SettingsPageListBox.Top = 30
    SettingsPageListBox.Width = 86
    SettingsPageListBox.Height = 100
    SettingsPageListBox.AddItem "Paths"
    SettingsPageListBox.AddItem "Generate"
    SettingsPageListBox.AddItem "Editor"
    SettingsPageListBox.AddItem "Studio"
    SettingsPageListBox.Visible = False

    Set SettingsListBox = Me.Controls.Add("Forms.ListBox.1", "lstStudioSettings", True)
    SettingsListBox.Left = 714
    SettingsListBox.Top = 30
    SettingsListBox.Width = 376
    SettingsListBox.Height = 220
    SettingsListBox.ColumnCount = 3
    SettingsListBox.ColumnWidths = "130 pt;190 pt;54 pt"
    SettingsListBox.Visible = False

    Set SettingsValueTextBox = Me.Controls.Add("Forms.TextBox.1", "txtStudioSettingValue", True)
    SettingsValueTextBox.Left = 714
    SettingsValueTextBox.Top = 260
    SettingsValueTextBox.Width = 296
    SettingsValueTextBox.Height = 22
    SettingsValueTextBox.Visible = False

    Set SettingsBooleanCheckBox = Me.Controls.Add("Forms.CheckBox.1", "chkStudioSettingValue", True)
    SettingsBooleanCheckBox.Left = 714
    SettingsBooleanCheckBox.Top = 260
    SettingsBooleanCheckBox.Width = 120
    SettingsBooleanCheckBox.Height = 22
    SettingsBooleanCheckBox.Caption = "Enabled"
    SettingsBooleanCheckBox.Visible = False

    Set SettingsBrowseButton = Me.Controls.Add("Forms.CommandButton.1", "btnStudioSettingsBrowse", True)
    SettingsBrowseButton.Caption = "Browse"
    SettingsBrowseButton.Left = 1018
    SettingsBrowseButton.Top = 258
    SettingsBrowseButton.Width = 72
    SettingsBrowseButton.Height = 24
    SettingsBrowseButton.Visible = False

    Set SettingsIssueListBox = Me.Controls.Add("Forms.ListBox.1", "lstStudioSettingsIssues", True)
    SettingsIssueListBox.Left = 714
    SettingsIssueListBox.Top = 292
    SettingsIssueListBox.Width = 376
    SettingsIssueListBox.Height = 120
    SettingsIssueListBox.ColumnCount = 4
    SettingsIssueListBox.ColumnWidths = "58 pt;90 pt;110 pt;200 pt"
    SettingsIssueListBox.Visible = False

    Set SettingsOkButton = Me.Controls.Add("Forms.CommandButton.1", "btnStudioSettingsOk", True)
    SettingsOkButton.Caption = "OK"
    SettingsOkButton.Left = 714
    SettingsOkButton.Top = 426
    SettingsOkButton.Width = 62
    SettingsOkButton.Height = 24
    SettingsOkButton.Visible = False

    Set SettingsApplyButton = Me.Controls.Add("Forms.CommandButton.1", "btnStudioSettingsApply", True)
    SettingsApplyButton.Caption = "Apply"
    SettingsApplyButton.Left = 784
    SettingsApplyButton.Top = 426
    SettingsApplyButton.Width = 62
    SettingsApplyButton.Height = 24
    SettingsApplyButton.Visible = False

    Set SettingsDefaultsButton = Me.Controls.Add("Forms.CommandButton.1", "btnStudioSettingsDefaults", True)
    SettingsDefaultsButton.Caption = "Restore Defaults"
    SettingsDefaultsButton.Left = 854
    SettingsDefaultsButton.Top = 426
    SettingsDefaultsButton.Width = 110
    SettingsDefaultsButton.Height = 24
    SettingsDefaultsButton.Visible = False

    Set SettingsCancelButton = Me.Controls.Add("Forms.CommandButton.1", "btnStudioSettingsCancel", True)
    SettingsCancelButton.Caption = "Cancel"
    SettingsCancelButton.Left = 972
    SettingsCancelButton.Top = 426
    SettingsCancelButton.Width = 70
    SettingsCancelButton.Height = 24
    SettingsCancelButton.Visible = False
End Sub

Private Sub RenderSettingsList()
    Dim PageName As String
    If EditingStudioSettings Is Nothing Then Exit Sub
    If SettingsPageListBox.ListIndex < 0 Then SettingsPageListBox.ListIndex = 0
    PageName = SettingsPageListBox.List(SettingsPageListBox.ListIndex)
    SettingsListBox.Clear
    Select Case PageName
        Case "Paths"
            AddSettingRow "ManifestDirectory", EditingStudioSettings.ManifestDirectory, "Path"
            AddSettingRow "TemplateDirectory", EditingStudioSettings.TemplateDirectory, "Path"
            AddSettingRow "OutputDirectory", EditingStudioSettings.OutputDirectory, "Path"
            AddSettingRow "LogDirectory", EditingStudioSettings.LogDirectory, "Path"
            AddSettingRow "BackupDirectory", EditingStudioSettings.BackupDirectory, "Path"
            AddSettingRow "MaxBackupCountPerFile", CStr(EditingStudioSettings.MaxBackupCountPerFile), "Text"
            AddSettingRow "MaxBackupAgeDays", CStr(EditingStudioSettings.MaxBackupAgeDays), "Text"
        Case "Generate"
            AddSettingRow "DefaultTargetScope", EditingStudioSettings.DefaultTargetScope, "Text"
            AddSettingRow "DefaultOverwriteMode", EditingStudioSettings.DefaultOverwriteMode, "Text"
            AddSettingRow "ContinueOnError", CStr(EditingStudioSettings.ContinueOnError), "Bool"
            AddSettingRow "CreateBackup", CStr(EditingStudioSettings.CreateBackup), "Bool"
            AddSettingRow "OpenOutputAfterGenerate", CStr(EditingStudioSettings.OpenOutputAfterGenerate), "Bool"
            AddSettingRow "ValidateBeforeGenerate", CStr(EditingStudioSettings.ValidateBeforeGenerate), "Bool"
        Case "Editor"
            AddSettingRow "PreviewFontName", EditingStudioSettings.PreviewFontName, "Text"
            AddSettingRow "PreviewFontSize", CStr(EditingStudioSettings.PreviewFontSize), "Text"
            AddSettingRow "ShowLineNumbers", CStr(EditingStudioSettings.ShowLineNumbers), "Bool"
            AddSettingRow "PreserveBlankLines", CStr(EditingStudioSettings.PreserveBlankLines), "Bool"
            AddSettingRow "ConfirmOnUnsavedChanges", CStr(EditingStudioSettings.ConfirmOnUnsavedChanges), "Bool"
        Case "Studio"
            AddSettingRow "LastProjectPath", EditingStudioSettings.LastProjectPath, "Path"
            AddSettingRow "LastSelectedModule", EditingStudioSettings.LastSelectedModule, "Text"
            AddSettingRow "LastSelectedTemplate", EditingStudioSettings.LastSelectedTemplate, "Text"
            AddSettingRow "WindowWidth", CStr(EditingStudioSettings.WindowWidth), "Text"
            AddSettingRow "WindowHeight", CStr(EditingStudioSettings.WindowHeight), "Text"
            AddSettingRow "ActivePage", EditingStudioSettings.ActivePage, "Text"
    End Select
    If SettingsListBox.ListCount > 0 Then SettingsListBox.ListIndex = 0
    ShowSelectedSettingValue
    RenderSettingsIssues SettingsIssues
End Sub

Private Sub AddSettingRow(ByVal KeyName As String, ByVal ValueText As String, ByVal ValueType As String)
    SettingsListBox.AddItem KeyName
    SettingsListBox.List(SettingsListBox.ListCount - 1, 1) = ValueText
    SettingsListBox.List(SettingsListBox.ListCount - 1, 2) = ValueType
End Sub

Private Sub ShowSelectedSettingValue()
    Dim ValueType As String
    Dim ValueText As String
    If SettingsListBox.ListIndex < 0 Then Exit Sub
    ValueText = SettingsListBox.List(SettingsListBox.ListIndex, 1)
    ValueType = SettingsListBox.List(SettingsListBox.ListIndex, 2)
    SuppressSettingsEvents = True
    SettingsValueTextBox.Visible = (ValueType <> "Bool")
    SettingsBooleanCheckBox.Visible = (ValueType = "Bool")
    SettingsBrowseButton.Visible = (ValueType = "Path")
    If ValueType = "Bool" Then
        SettingsBooleanCheckBox.Value = (StrComp(ValueText, "True", vbTextCompare) = 0)
    Else
        SettingsValueTextBox.Text = ValueText
    End If
    SuppressSettingsEvents = False
End Sub

Private Sub ApplySelectedSettingValue(ByVal ValueText As String)
    Dim KeyName As String
    If SettingsListBox.ListIndex < 0 Then Exit Sub
    KeyName = SettingsListBox.List(SettingsListBox.ListIndex, 0)
    SetStudioSettingValue EditingStudioSettings, KeyName, ValueText
    SettingsListBox.List(SettingsListBox.ListIndex, 1) = GetStudioSettingValue(EditingStudioSettings, KeyName)
End Sub

Private Function ApplySettingsChanges() As Boolean
    Dim Result As ComResult
    CaptureSelectedSettingValue
    Set Result = AppSaveStudioSettings(EditingStudioSettings, SettingsIssues)
    RenderSettingsIssues SettingsIssues
    If Result.IsFailure Or AppStudioSettingsIssuesContainErrors(SettingsIssues) Then
        MsgBox Result.Message, vbExclamation, "Studio Settings"
        ApplySettingsChanges = False
        Exit Function
    End If

    Set StudioSettings = EditingStudioSettings.AppClone()
    ApplyStudioSettingsToUi
    MsgBox Result.Message, vbInformation, "Studio Settings"
    ApplySettingsChanges = True
End Function

Private Sub CaptureSelectedSettingValue()
    If SettingsListBox.ListIndex < 0 Then Exit Sub
    If SettingsListBox.List(SettingsListBox.ListIndex, 2) = "Bool" Then
        ApplySelectedSettingValue CStr(SettingsBooleanCheckBox.Value)
    Else
        ApplySelectedSettingValue SettingsValueTextBox.Text
    End If
End Sub

Private Sub BrowseSelectedSettingPath()
    Dim Dialog As Object
    Dim SelectedPath As String
    If SettingsListBox.ListIndex < 0 Then Exit Sub
    If SettingsListBox.List(SettingsListBox.ListIndex, 2) <> "Path" Then Exit Sub

    Set Dialog = Application.FileDialog(4)
    Dialog.Title = "Select Folder"
    Dialog.AllowMultiSelect = False
    If Len(SettingsValueTextBox.Text) > 0 Then
        On Error Resume Next
        Dialog.InitialFileName = SettingsValueTextBox.Text
        On Error GoTo 0
    End If

    If Dialog.Show <> -1 Then Exit Sub
    SelectedPath = CStr(Dialog.SelectedItems(1))
    SettingsValueTextBox.Text = SelectedPath
    ApplySelectedSettingValue SelectedPath
End Sub

Private Sub RenderSettingsIssues(ByVal Issues As Collection)
    Dim Issue As AppStudioSettingsIssue
    SettingsIssueListBox.Clear
    If Issues Is Nothing Or Issues.Count = 0 Then
        SettingsIssueListBox.AddItem "Information"
        SettingsIssueListBox.List(0, 1) = "VMF-SET-OK"
        SettingsIssueListBox.List(0, 2) = vbNullString
        SettingsIssueListBox.List(0, 3) = "Studio settings are valid."
        Exit Sub
    End If
    For Each Issue In Issues
        SettingsIssueListBox.AddItem Issue.SeverityText
        SettingsIssueListBox.List(SettingsIssueListBox.ListCount - 1, 1) = Issue.Code
        SettingsIssueListBox.List(SettingsIssueListBox.ListCount - 1, 2) = Issue.SettingKey
        SettingsIssueListBox.List(SettingsIssueListBox.ListCount - 1, 3) = Issue.Message
    Next Issue
End Sub

Private Sub ApplyStudioSettingsToUi()
    If StudioSettings Is Nothing Then Exit Sub
    Me.Width = StudioSettings.WindowWidth
    Me.Height = StudioSettings.WindowHeight
    If Not PreviewCodeTextBox Is Nothing Then
        PreviewCodeTextBox.Font.Name = StudioSettings.PreviewFontName
        PreviewCodeTextBox.Font.Size = StudioSettings.PreviewFontSize
    End If
    If Not TemplateContentTextBox Is Nothing Then
        TemplateContentTextBox.Font.Name = StudioSettings.PreviewFontName
        TemplateContentTextBox.Font.Size = StudioSettings.PreviewFontSize
    End If
End Sub

Private Function GetStudioSettingValue(ByVal Settings As AppStudioSettings, ByVal KeyName As String) As String
    Select Case KeyName
        Case "ManifestDirectory": GetStudioSettingValue = Settings.ManifestDirectory
        Case "TemplateDirectory": GetStudioSettingValue = Settings.TemplateDirectory
        Case "OutputDirectory": GetStudioSettingValue = Settings.OutputDirectory
        Case "LogDirectory": GetStudioSettingValue = Settings.LogDirectory
        Case "BackupDirectory": GetStudioSettingValue = Settings.BackupDirectory
        Case "MaxBackupCountPerFile": GetStudioSettingValue = CStr(Settings.MaxBackupCountPerFile)
        Case "MaxBackupAgeDays": GetStudioSettingValue = CStr(Settings.MaxBackupAgeDays)
        Case "DefaultTargetScope": GetStudioSettingValue = Settings.DefaultTargetScope
        Case "DefaultOverwriteMode": GetStudioSettingValue = Settings.DefaultOverwriteMode
        Case "ContinueOnError": GetStudioSettingValue = CStr(Settings.ContinueOnError)
        Case "CreateBackup": GetStudioSettingValue = CStr(Settings.CreateBackup)
        Case "OpenOutputAfterGenerate": GetStudioSettingValue = CStr(Settings.OpenOutputAfterGenerate)
        Case "ValidateBeforeGenerate": GetStudioSettingValue = CStr(Settings.ValidateBeforeGenerate)
        Case "PreviewFontName": GetStudioSettingValue = Settings.PreviewFontName
        Case "PreviewFontSize": GetStudioSettingValue = CStr(Settings.PreviewFontSize)
        Case "ShowLineNumbers": GetStudioSettingValue = CStr(Settings.ShowLineNumbers)
        Case "PreserveBlankLines": GetStudioSettingValue = CStr(Settings.PreserveBlankLines)
        Case "ConfirmOnUnsavedChanges": GetStudioSettingValue = CStr(Settings.ConfirmOnUnsavedChanges)
        Case "LastProjectPath": GetStudioSettingValue = Settings.LastProjectPath
        Case "LastSelectedModule": GetStudioSettingValue = Settings.LastSelectedModule
        Case "LastSelectedTemplate": GetStudioSettingValue = Settings.LastSelectedTemplate
        Case "WindowWidth": GetStudioSettingValue = CStr(Settings.WindowWidth)
        Case "WindowHeight": GetStudioSettingValue = CStr(Settings.WindowHeight)
        Case "ActivePage": GetStudioSettingValue = Settings.ActivePage
    End Select
End Function

Private Sub SetStudioSettingValue(ByVal Settings As AppStudioSettings, ByVal KeyName As String, ByVal ValueText As String)
    On Error Resume Next
    Select Case KeyName
        Case "ManifestDirectory": Settings.ManifestDirectory = ValueText
        Case "TemplateDirectory": Settings.TemplateDirectory = ValueText
        Case "OutputDirectory": Settings.OutputDirectory = ValueText
        Case "LogDirectory": Settings.LogDirectory = ValueText
        Case "BackupDirectory": Settings.BackupDirectory = ValueText
        Case "MaxBackupCountPerFile": Settings.MaxBackupCountPerFile = CLng(ValueText)
        Case "MaxBackupAgeDays": Settings.MaxBackupAgeDays = CLng(ValueText)
        Case "DefaultTargetScope": Settings.DefaultTargetScope = ValueText
        Case "DefaultOverwriteMode": Settings.DefaultOverwriteMode = ValueText
        Case "ContinueOnError": Settings.ContinueOnError = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "CreateBackup": Settings.CreateBackup = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "OpenOutputAfterGenerate": Settings.OpenOutputAfterGenerate = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "ValidateBeforeGenerate": Settings.ValidateBeforeGenerate = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "PreviewFontName": Settings.PreviewFontName = ValueText
        Case "PreviewFontSize": Settings.PreviewFontSize = CLng(ValueText)
        Case "ShowLineNumbers": Settings.ShowLineNumbers = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "PreserveBlankLines": Settings.PreserveBlankLines = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "ConfirmOnUnsavedChanges": Settings.ConfirmOnUnsavedChanges = (StrComp(ValueText, "True", vbTextCompare) = 0)
        Case "LastProjectPath": Settings.LastProjectPath = ValueText
        Case "LastSelectedModule": Settings.LastSelectedModule = ValueText
        Case "LastSelectedTemplate": Settings.LastSelectedTemplate = ValueText
        Case "WindowWidth": Settings.WindowWidth = CLng(ValueText)
        Case "WindowHeight": Settings.WindowHeight = CLng(ValueText)
        Case "ActivePage": Settings.ActivePage = ValueText
    End Select
    On Error GoTo 0
End Sub

Private Sub GenerateFromEditor(ByVal TargetScope As String)
    Dim SelectedNames As Collection
    Dim Request As AppGenerateRequest
    Dim Result As ComResult
    Dim GenerateResult As AppGenerateResult
    Dim OutputDirectory As String
    Dim OverwriteMode As String

    If GenerateInProgress Then
        MsgBox "Generation is already running.", vbInformation, "Manifest Editor"
        Exit Sub
    End If

    If TargetScope <> "AllModules" And SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    If StudioSettings.ConfirmOnUnsavedChanges Then
        If MsgBox("Generate from the current editor values? Unsaved edits are included, but the manifest file is not saved.", vbOKCancel + vbQuestion, "Manifest Editor") <> vbOK Then
            Exit Sub
        End If
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    If StudioSettings.ValidateBeforeGenerate Then
        Set Result = ValidateProjectModel()
        If AppValidationIssuesContainErrors(ValidationIssues) Then
            ShowValidationPane ValidationIssues
            MsgBox Result.Message, vbExclamation, "Manifest Editor"
            Exit Sub
        End If
    End If

    OverwriteMode = ResolveOverwriteMode()
    If ComIsBlankText(OverwriteMode) Then
        Exit Sub
    End If

    OutputDirectory = AppDefaultManifestGenerateOutputDirectory(txtManifestPath.Text)
    Set SelectedNames = CreateSelectedModuleNames(TargetScope)
    Set Request = AppCreateManifestGenerateRequest(TargetScope, SelectedNames, OutputDirectory, OverwriteMode, StudioSettings.ContinueOnError)

    GenerateInProgress = True
    SetUiState "Generating"
    SetGenerateButtonsEnabled False
    On Error GoTo GenerateFailed
    Set Result = AppGenerateManifestEditorModel(txtManifestPath.Text, Modules, Request, GenerateResult)
    Set LastGenerateResult = GenerateResult
    ShowBuildLogPane GenerateResult
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Manifest Editor"
    If Result.IsSuccess And StudioSettings.OpenOutputAfterGenerate Then
        Shell "explorer.exe " & Chr$(34) & GenerateResult.OutputDirectory & Chr$(34), vbNormalFocus
    End If

GenerateDone:
    GenerateInProgress = False
    SetUiState IIf(IsDirty, "Dirty", "Ready")
    SetGenerateButtonsEnabled True
    Exit Sub

GenerateFailed:
    MsgBox Err.Description, vbExclamation, "Manifest Editor"
    Resume GenerateDone
End Sub

Private Function ResolveOverwriteMode() As String
    Dim Answer As VbMsgBoxResult

    If StudioSettings.DefaultOverwriteMode <> "Confirm" Then
        ResolveOverwriteMode = StudioSettings.DefaultOverwriteMode
        Exit Function
    End If

    Answer = MsgBox("Overwrite existing generated files?" & vbCrLf & "Yes: overwrite" & vbCrLf & "No: skip existing files" & vbCrLf & "Cancel: abort", vbYesNoCancel + vbQuestion, "Manifest Editor")
    Select Case Answer
        Case vbYes
            ResolveOverwriteMode = "Overwrite"
        Case vbNo
            ResolveOverwriteMode = "Skip"
        Case Else
            ResolveOverwriteMode = vbNullString
    End Select
End Function

Private Function CreateSelectedModuleNames(ByVal TargetScope As String) As Collection
    Dim SelectedNames As Collection
    Dim Key As Variant

    Set SelectedNames = New Collection
    If TargetScope <> "AllModules" Then
        If Not GenerateTargetByModule Is Nothing Then
            For Each Key In GenerateTargetByModule.Keys
                SelectedNames.Add CStr(Key)
            Next Key
        ElseIf SelectedModuleIndex > 0 Then
            SelectedNames.Add CStr(Modules.Item(SelectedModuleIndex)("ModuleName"))
        End If
    End If

    Set CreateSelectedModuleNames = SelectedNames
End Function

Private Sub SetGenerateButtonsEnabled(ByVal IsEnabled As Boolean)
    If Not GenerateAllButton Is Nothing Then
        GenerateAllButton.Enabled = IsEnabled
    End If
    If Not GenerateSelectedButton Is Nothing Then
        GenerateSelectedButton.Enabled = IsEnabled
    End If
End Sub
Private Sub ShowPreviewPane()
    HideSettingsPane
    HideTemplateManagerPane
    HideValidationPane
    HideBuildLogPane
    EnsurePreviewControls
    Me.Width = 980
    PreviewCodeTextBox.Visible = True
    PreviewErrorTextBox.Visible = True
    PreviewRefreshButton.Visible = True
    PreviewCopyButton.Visible = True
    PreviewCloseButton.Visible = True
    RefreshPreviewPane
End Sub

Private Sub HidePreviewPane()
    If PreviewCodeTextBox Is Nothing Then
        Exit Sub
    End If

    PreviewCodeTextBox.Visible = False
    PreviewErrorTextBox.Visible = False
    PreviewRefreshButton.Visible = False
    PreviewCopyButton.Visible = False
    PreviewCloseButton.Visible = False
    Me.Width = 620
End Sub

Private Sub RefreshPreviewPane()
    Dim Result As ComResult
    Dim PreviewText As String
    Dim ModuleInfo As Object

    EnsurePreviewControls
    PreviewCodeTextBox.Text = vbNullString
    PreviewErrorTextBox.Text = vbNullString

    If SelectedModuleIndex <= 0 Then
        PreviewErrorTextBox.Text = "Manifest input invalid: select a module first."
        Exit Sub
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Result = AppPreviewManifestEditorModule(txtManifestPath.Text, ModuleInfo, PreviewText)
    If Result.IsSuccess Then
        PreviewCodeTextBox.Text = PreviewText
    Else
        PreviewErrorTextBox.Text = Result.Message
    End If
End Sub

Private Sub CopyPreviewText()
    Dim Clipboard As Object

    EnsurePreviewControls
    If ComIsBlankText(PreviewCodeTextBox.Text) Then
        PreviewErrorTextBox.Text = "No preview code to copy."
        Exit Sub
    End If

    Set Clipboard = CreateObject("Forms.DataObject.1")
    Clipboard.SetText PreviewCodeTextBox.Text
    Clipboard.PutInClipboard
    PreviewErrorTextBox.Text = "Preview copied."
End Sub

Private Sub EnsurePreviewControls()
    If Not PreviewCodeTextBox Is Nothing Then
        Exit Sub
    End If

    Set PreviewCodeTextBox = Me.Controls.Add("Forms.TextBox.1", "txtCodePreview", True)
    PreviewCodeTextBox.Left = 620
    PreviewCodeTextBox.Top = 30
    PreviewCodeTextBox.Width = 330
    PreviewCodeTextBox.Height = 310
    PreviewCodeTextBox.MultiLine = True
    PreviewCodeTextBox.ScrollBars = fmScrollBarsBoth
    PreviewCodeTextBox.WordWrap = False
    PreviewCodeTextBox.Locked = True
    PreviewCodeTextBox.Font.Name = StudioSettings.PreviewFontName
    PreviewCodeTextBox.Font.Size = StudioSettings.PreviewFontSize
    PreviewCodeTextBox.Visible = False

    Set PreviewErrorTextBox = Me.Controls.Add("Forms.TextBox.1", "txtCodePreviewError", True)
    PreviewErrorTextBox.Left = 620
    PreviewErrorTextBox.Top = 350
    PreviewErrorTextBox.Width = 330
    PreviewErrorTextBox.Height = 46
    PreviewErrorTextBox.MultiLine = True
    PreviewErrorTextBox.ScrollBars = fmScrollBarsVertical
    PreviewErrorTextBox.Locked = True
    PreviewErrorTextBox.Visible = False

    Set PreviewRefreshButton = Me.Controls.Add("Forms.CommandButton.1", "btnCodePreviewRefresh", True)
    PreviewRefreshButton.Caption = "Refresh"
    PreviewRefreshButton.Left = 620
    PreviewRefreshButton.Top = 406
    PreviewRefreshButton.Width = 78
    PreviewRefreshButton.Height = 24
    PreviewRefreshButton.Visible = False

    Set PreviewCopyButton = Me.Controls.Add("Forms.CommandButton.1", "btnCodePreviewCopy", True)
    PreviewCopyButton.Caption = "Copy"
    PreviewCopyButton.Left = 708
    PreviewCopyButton.Top = 406
    PreviewCopyButton.Width = 72
    PreviewCopyButton.Height = 24
    PreviewCopyButton.Visible = False

    Set PreviewCloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnCodePreviewClose", True)
    PreviewCloseButton.Caption = "Close"
    PreviewCloseButton.Left = 790
    PreviewCloseButton.Top = 406
    PreviewCloseButton.Width = 78
    PreviewCloseButton.Height = 24
    PreviewCloseButton.Visible = False
End Sub
Private Sub ShowTemplateManagerPane()
    HidePreviewPane
    HideValidationPane
    HideBuildLogPane
    EnsureTemplateManagerControls
    Me.Width = 1120
    TemplateListBox.Visible = True
    TemplateContentTextBox.Visible = True
    TemplateAnalysisListBox.Visible = True
    TemplateValidationListBox.Visible = True
    TemplateSaveButton.Visible = True
    TemplateSaveAsButton.Visible = True
    TemplateReloadButton.Visible = True
    TemplateValidateButton.Visible = True
    TemplatePreviewButton.Visible = True
    TemplateCloseButton.Visible = True
    LoadTemplateManagerTemplates
End Sub

Private Sub HideTemplateManagerPane()
    If TemplateListBox Is Nothing Then Exit Sub

    TemplateListBox.Visible = False
    TemplateContentTextBox.Visible = False
    TemplateAnalysisListBox.Visible = False
    TemplateValidationListBox.Visible = False
    TemplateSaveButton.Visible = False
    TemplateSaveAsButton.Visible = False
    TemplateReloadButton.Visible = False
    TemplateValidateButton.Visible = False
    TemplatePreviewButton.Visible = False
    TemplateCloseButton.Visible = False
    Me.Width = 620
End Sub

Private Sub EnsureTemplateManagerControls()
    If Not TemplateListBox Is Nothing Then Exit Sub

    Set TemplateListBox = Me.Controls.Add("Forms.ListBox.1", "lstTemplates", True)
    TemplateListBox.Left = 620
    TemplateListBox.Top = 30
    TemplateListBox.Width = 470
    TemplateListBox.Height = 92
    TemplateListBox.ColumnCount = 5
    TemplateListBox.ColumnWidths = "110 pt;90 pt;160 pt;58 pt;46 pt"
    TemplateListBox.Visible = False

    Set TemplateContentTextBox = Me.Controls.Add("Forms.TextBox.1", "txtTemplateContent", True)
    TemplateContentTextBox.Left = 620
    TemplateContentTextBox.Top = 128
    TemplateContentTextBox.Width = 470
    TemplateContentTextBox.Height = 250
    TemplateContentTextBox.MultiLine = True
    TemplateContentTextBox.ScrollBars = fmScrollBarsBoth
    TemplateContentTextBox.WordWrap = False
    TemplateContentTextBox.Font.Name = "Consolas"
    TemplateContentTextBox.Font.Size = 9
    TemplateContentTextBox.Visible = False

    Set TemplateAnalysisListBox = Me.Controls.Add("Forms.ListBox.1", "lstTemplateAnalysis", True)
    TemplateAnalysisListBox.Left = 620
    TemplateAnalysisListBox.Top = 384
    TemplateAnalysisListBox.Width = 232
    TemplateAnalysisListBox.Height = 90
    TemplateAnalysisListBox.ColumnCount = 4
    TemplateAnalysisListBox.ColumnWidths = "76 pt;38 pt;40 pt;54 pt"
    TemplateAnalysisListBox.Visible = False

    Set TemplateValidationListBox = Me.Controls.Add("Forms.ListBox.1", "lstTemplateValidation", True)
    TemplateValidationListBox.Left = 858
    TemplateValidationListBox.Top = 384
    TemplateValidationListBox.Width = 232
    TemplateValidationListBox.Height = 90
    TemplateValidationListBox.ColumnCount = 4
    TemplateValidationListBox.ColumnWidths = "54 pt;74 pt;36 pt;150 pt"
    TemplateValidationListBox.Visible = False

    Set TemplateSaveButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateSave", True)
    TemplateSaveButton.Caption = "Save"
    TemplateSaveButton.Left = 620
    TemplateSaveButton.Top = 484
    TemplateSaveButton.Width = 58
    TemplateSaveButton.Height = 24
    TemplateSaveButton.Visible = False

    Set TemplateSaveAsButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateSaveAs", True)
    TemplateSaveAsButton.Caption = "Save As"
    TemplateSaveAsButton.Left = 804
    TemplateSaveAsButton.Top = 484
    TemplateSaveAsButton.Width = 70
    TemplateSaveAsButton.Height = 24
    TemplateSaveAsButton.Visible = False

    Set TemplateReloadButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateReload", True)
    TemplateReloadButton.Caption = "Reload"
    TemplateReloadButton.Left = 880
    TemplateReloadButton.Top = 484
    TemplateReloadButton.Width = 66
    TemplateReloadButton.Height = 24
    TemplateReloadButton.Visible = False

    Set TemplateValidateButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateValidate", True)
    TemplateValidateButton.Caption = "Validate"
    TemplateValidateButton.Left = 952
    TemplateValidateButton.Top = 484
    TemplateValidateButton.Width = 72
    TemplateValidateButton.Height = 24
    TemplateValidateButton.Visible = False

    Set TemplatePreviewButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplatePreview", True)
    TemplatePreviewButton.Caption = "Preview"
    TemplatePreviewButton.Left = 1030
    TemplatePreviewButton.Top = 484
    TemplatePreviewButton.Width = 72
    TemplatePreviewButton.Height = 24
    TemplatePreviewButton.Visible = False

    Set TemplateCloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnTemplateClose", True)
    TemplateCloseButton.Caption = "Close"
    TemplateCloseButton.Left = 1030
    TemplateCloseButton.Top = 454
    TemplateCloseButton.Width = 72
    TemplateCloseButton.Height = 24
    TemplateCloseButton.Visible = False
End Sub

Private Sub LoadTemplateManagerTemplates()
    Dim Result As ComResult

    EnsureTemplateManagerControls
    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Result = AppLoadTemplateManagerTemplates(txtManifestPath.Text, Modules, Templates)
    If Result.IsFailure Then
        TemplateValidationListBox.Clear
        TemplateValidationListBox.AddItem "Error"
        TemplateValidationListBox.List(0, 1) = "VMF-TPL-LOAD"
        TemplateValidationListBox.List(0, 2) = "0"
        TemplateValidationListBox.List(0, 3) = Result.Message
        Exit Sub
    End If

    RenderTemplateList
    If Templates.Count > 0 Then
        TemplateListBox.ListIndex = 0
        SelectTemplate 1
    End If
End Sub

Private Sub SelectTemplate(ByVal TemplateIndex As Long)
    If TemplateIndex <= 0 Or TemplateIndex > Templates.Count Then Exit Sub
    SelectedTemplateIndex = TemplateIndex
    RenderSelectedTemplate
End Sub

Private Sub RenderTemplateList()
    Dim Template As AppTemplateModel
    Dim RowIndex As Long

    EnsureTemplateManagerControls
    TemplateListBox.Clear
    For Each Template In Templates
        TemplateListBox.AddItem Template.TemplateName
        RowIndex = TemplateListBox.ListCount - 1
        TemplateListBox.List(RowIndex, 1) = Template.TemplateType
        TemplateListBox.List(RowIndex, 2) = Template.FilePath
        TemplateListBox.List(RowIndex, 3) = TemplateValidationState(Template)
        TemplateListBox.List(RowIndex, 4) = IIf(Template.IsDirty, "Dirty", vbNullString)
    Next Template
End Sub

Private Sub RenderSelectedTemplate()
    Dim Template As AppTemplateModel
    Dim Result As ComResult

    If SelectedTemplateIndex <= 0 Then Exit Sub
    Set Template = Templates.Item(SelectedTemplateIndex)
    Set Result = AppAnalyzeTemplateModel(Template)

    SuppressTemplateEvents = True
    TemplateContentTextBox.Text = Template.Content
    SuppressTemplateEvents = False
    RenderTemplateAnalysis Template
    RenderTemplateValidation Template.ValidationResults
End Sub

Private Sub RenderTemplateAnalysis(ByVal Template As AppTemplateModel)
    Dim Placeholder As Object
    Dim SectionInfo As Object

    TemplateAnalysisListBox.Clear
    For Each Placeholder In Template.Placeholders
        TemplateAnalysisListBox.AddItem CStr(Placeholder("Name"))
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 1) = CStr(Placeholder("OccurrenceCount"))
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 2) = CStr(Placeholder("LineNumber"))
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 3) = IIf(CBool(Placeholder("IsKnown")), "Known", "Unknown")
    Next Placeholder

    For Each SectionInfo In Template.Sections
        TemplateAnalysisListBox.AddItem "@" & CStr(SectionInfo("Name"))
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 1) = "Section"
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 2) = CStr(SectionInfo("LineNumber"))
        TemplateAnalysisListBox.List(TemplateAnalysisListBox.ListCount - 1, 3) = IIf(CBool(SectionInfo("IsValid")), "Valid", "Error")
    Next SectionInfo
End Sub

Private Sub RenderTemplateValidation(ByVal Issues As Collection)
    Dim Issue As AppTemplateValidationIssue

    TemplateValidationListBox.Clear
    If Issues Is Nothing Then
        TemplateValidationListBox.AddItem "Information"
        TemplateValidationListBox.List(0, 1) = "VMF-TPL-OK"
        TemplateValidationListBox.List(0, 2) = "0"
        TemplateValidationListBox.List(0, 3) = "Template validation not run."
        Exit Sub
    End If

    If Issues.Count = 0 Then
        TemplateValidationListBox.AddItem "Information"
        TemplateValidationListBox.List(0, 1) = "VMF-TPL-OK"
        TemplateValidationListBox.List(0, 2) = "0"
        TemplateValidationListBox.List(0, 3) = "Template validation passed."
        Exit Sub
    End If

    For Each Issue In Issues
        TemplateValidationListBox.AddItem Issue.SeverityText
        TemplateValidationListBox.List(TemplateValidationListBox.ListCount - 1, 1) = Issue.Code
        TemplateValidationListBox.List(TemplateValidationListBox.ListCount - 1, 2) = CStr(Issue.LineNumber)
        TemplateValidationListBox.List(TemplateValidationListBox.ListCount - 1, 3) = Issue.Message
    Next Issue
End Sub

Private Function TemplateValidationState(ByVal Template As AppTemplateModel) As String
    If Template.ValidationResults Is Nothing Then
        TemplateValidationState = "[-]"
    ElseIf Template.ValidationResults.Count = 0 Then
        TemplateValidationState = "[V]"
    ElseIf AppTemplateIssuesContainErrors(Template.ValidationResults) Then
        TemplateValidationState = "[E]"
    Else
        TemplateValidationState = "[W]"
    End If
End Function

Private Sub ValidateSelectedTemplate()
    Dim Template As AppTemplateModel
    Dim Result As ComResult

    If SelectedTemplateIndex <= 0 Then Exit Sub
    Set Template = Templates.Item(SelectedTemplateIndex)
    Template.Content = TemplateContentTextBox.Text
    Set Result = AppValidateTemplateModel(Template, TemplateIssues)
    RenderTemplateAnalysis Template
    RenderTemplateValidation TemplateIssues
    RenderTemplateList
    TemplateListBox.ListIndex = SelectedTemplateIndex - 1
End Sub

Private Sub SaveSelectedTemplate(ByVal SaveAsNew As Boolean)
    Dim Template As AppTemplateModel
    Dim Result As ComResult
    Dim SelectedPath As Variant
    Dim TemplateName As String

    If SelectedTemplateIndex <= 0 Then Exit Sub
    Set Template = Templates.Item(SelectedTemplateIndex)
    Template.Content = TemplateContentTextBox.Text

    If SaveAsNew Then
        SelectedPath = Application.GetSaveAsFilename(Template.FilePath, "Template Files (*.txt),*.txt,All Files (*.*),*.*", , "Save Template As")
        If VarType(SelectedPath) = vbBoolean Then
            If Not CBool(SelectedPath) Then Exit Sub
        End If
        TemplateName = Mid$(CStr(SelectedPath), InStrRev(CStr(SelectedPath), Application.PathSeparator) + 1)
        If InStrRev(TemplateName, ".") > 0 Then TemplateName = Left$(TemplateName, InStrRev(TemplateName, ".") - 1)
        Set Result = AppSaveTemplateModelAs(Template, TemplateName, CStr(SelectedPath))
    Else
        Set Result = AppSaveTemplateModel(Template)
    End If

    If Result.IsFailure Then
        RenderTemplateValidation Template.ValidationResults
        MsgBox Result.Message, vbExclamation, "Template Manager"
    Else
        MsgBox Result.Message, vbInformation, "Template Manager"
    End If
    RenderTemplateList
    TemplateListBox.ListIndex = SelectedTemplateIndex - 1
End Sub

Private Sub PreviewSelectedTemplate()
    Dim Template As AppTemplateModel
    Dim Result As ComResult
    Dim PreviewText As String

    If SelectedTemplateIndex <= 0 Then Exit Sub
    If SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Template Manager"
        Exit Sub
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Template = Templates.Item(SelectedTemplateIndex)
    Template.Content = TemplateContentTextBox.Text
    Set Result = AppPreviewTemplateModelWithModule(txtManifestPath.Text, Template, Modules.Item(SelectedModuleIndex), PreviewText)
    RenderTemplateValidation Template.ValidationResults
    If Result.IsFailure Then
        MsgBox Result.Message, vbExclamation, "Template Manager"
        Exit Sub
    End If

    ShowPreviewPane
    PreviewCodeTextBox.Text = PreviewText
    PreviewErrorTextBox.Text = "Template preview generated from unsaved template content."
End Sub

Private Function ValidateProjectModel() As ComResult
    SetUiState "Validating"
    Set ValidateProjectModel = AppValidateManifestEditorModel(txtManifestPath.Text, Modules, ValidationIssues)
    UpdateValidationStatusFromIssues ValidationIssues, vbNullString
    SetUiState IIf(IsDirty, "Dirty", "Ready")
End Function

Private Function ValidateSelectedModule() As ComResult
    If SelectedModuleIndex <= 0 Then
        Set ValidationIssues = New Collection
        Set ValidateSelectedModule = ComCreateSuccess("Manifest validation failed.")
        Exit Function
    End If

    SetUiState "Validating"
    Set ValidateSelectedModule = AppValidateManifestEditorModule(txtManifestPath.Text, Modules.Item(SelectedModuleIndex), ValidationIssues)
    UpdateValidationStatusFromIssues ValidationIssues, CStr(Modules.Item(SelectedModuleIndex)("ModuleName"))
    SetUiState IIf(IsDirty, "Dirty", "Ready")
End Function

Private Sub ShowValidationPane(ByVal Issues As Collection)
    HideSettingsPane
    HideTemplateManagerPane
    HidePreviewPane
    HideBuildLogPane
    EnsureValidationControls
    Me.Width = 980
    ValidationListBox.Visible = True
    ValidationCloseButton.Visible = True
    RenderValidationIssues Issues
End Sub

Private Sub ShowBuildLogPane(ByVal GenerateResult As AppGenerateResult)
    HideSettingsPane
    HideTemplateManagerPane
    HidePreviewPane
    HideValidationPane
    EnsureBuildLogControls
    Me.Width = 980
    BuildLogListBox.Visible = True
    BuildLogClearButton.Visible = True
    BuildLogOpenFolderButton.Visible = True
    BuildLogSummaryLabel.Visible = True
    RenderBuildLog GenerateResult
End Sub

Private Sub HideBuildLogPane()
    If BuildLogListBox Is Nothing Then
        Exit Sub
    End If

    BuildLogListBox.Visible = False
    BuildLogClearButton.Visible = False
    BuildLogOpenFolderButton.Visible = False
    BuildLogSummaryLabel.Visible = False
End Sub

Private Sub EnsureBuildLogControls()
    If Not BuildLogListBox Is Nothing Then
        Exit Sub
    End If

    Set BuildLogListBox = Me.Controls.Add("Forms.ListBox.1", "lstBuildLog", True)
    BuildLogListBox.Left = 620
    BuildLogListBox.Top = 30
    BuildLogListBox.Width = 330
    BuildLogListBox.Height = 330
    BuildLogListBox.ColumnCount = 6
    BuildLogListBox.ColumnWidths = "50 pt;58 pt;86 pt;70 pt;150 pt;220 pt"
    BuildLogListBox.Visible = False

    Set BuildLogSummaryLabel = Me.Controls.Add("Forms.Label.1", "lblBuildLogSummary", True)
    BuildLogSummaryLabel.Left = 620
    BuildLogSummaryLabel.Top = 366
    BuildLogSummaryLabel.Width = 330
    BuildLogSummaryLabel.Height = 18
    BuildLogSummaryLabel.Caption = "Success: 0  Warning: 0  Failed: 0"
    BuildLogSummaryLabel.Visible = False

    Set BuildLogClearButton = Me.Controls.Add("Forms.CommandButton.1", "btnBuildLogClear", True)
    BuildLogClearButton.Caption = "Clear Log"
    BuildLogClearButton.Left = 620
    BuildLogClearButton.Top = 406
    BuildLogClearButton.Width = 86
    BuildLogClearButton.Height = 24
    BuildLogClearButton.Visible = False

    Set BuildLogOpenFolderButton = Me.Controls.Add("Forms.CommandButton.1", "btnBuildLogOpenFolder", True)
    BuildLogOpenFolderButton.Caption = "Open Output"
    BuildLogOpenFolderButton.Left = 716
    BuildLogOpenFolderButton.Top = 406
    BuildLogOpenFolderButton.Width = 100
    BuildLogOpenFolderButton.Height = 24
    BuildLogOpenFolderButton.Visible = False
End Sub

Private Sub RenderBuildLog(ByVal GenerateResult As AppGenerateResult)
    Dim ModuleResult As AppModuleGenerateResult

    EnsureBuildLogControls
    BuildLogListBox.Clear
    If GenerateResult Is Nothing Then
        BuildLogSummaryLabel.Caption = "Success: 0  Warning: 0  Failed: 0"
        Exit Sub
    End If

    For Each ModuleResult In GenerateResult.ModuleResults
        BuildLogListBox.AddItem Format$(ModuleResult.LoggedAt, "hh:nn:ss")
        BuildLogListBox.List(BuildLogListBox.ListCount - 1, 1) = SeverityForGenerateStatus(ModuleResult.Status)
        BuildLogListBox.List(BuildLogListBox.ListCount - 1, 2) = ModuleResult.ModuleName
        BuildLogListBox.List(BuildLogListBox.ListCount - 1, 3) = ModuleResult.Status
        BuildLogListBox.List(BuildLogListBox.ListCount - 1, 4) = ModuleResult.Message
        BuildLogListBox.List(BuildLogListBox.ListCount - 1, 5) = ModuleResult.OutputPath
    Next ModuleResult

    BuildLogSummaryLabel.Caption = "Success: " & CStr(GenerateResult.SuccessCount) & _
        "  Warning: " & CStr(GenerateResult.WarningCount) & _
        "  Failed: " & CStr(GenerateResult.FailureCount)
End Sub

Private Function SeverityForGenerateStatus(ByVal Status As String) As String
    Select Case Status
        Case "Generated"
            SeverityForGenerateStatus = "INFO"
        Case "Skipped"
            SeverityForGenerateStatus = "WARNING"
        Case "Failed"
            SeverityForGenerateStatus = "ERROR"
        Case Else
            SeverityForGenerateStatus = "INFO"
    End Select
End Function
Private Sub HideValidationPane()
    If ValidationListBox Is Nothing Then
        Exit Sub
    End If

    ValidationListBox.Visible = False
    ValidationCloseButton.Visible = False
End Sub

Private Sub EnsureValidationControls()
    If Not ValidationListBox Is Nothing Then
        Exit Sub
    End If

    Set ValidationListBox = Me.Controls.Add("Forms.ListBox.1", "lstValidationIssues", True)
    ValidationListBox.Left = 620
    ValidationListBox.Top = 30
    ValidationListBox.Width = 330
    ValidationListBox.Height = 330
    ValidationListBox.ColumnCount = 5
    ValidationListBox.ColumnWidths = "58 pt;72 pt;74 pt;62 pt;190 pt"
    ValidationListBox.Visible = False

    Set ValidationCloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnValidationClose", True)
    ValidationCloseButton.Caption = "Close"
    ValidationCloseButton.Left = 872
    ValidationCloseButton.Top = 370
    ValidationCloseButton.Width = 78
    ValidationCloseButton.Height = 24
    ValidationCloseButton.Visible = False
End Sub

Private Sub RenderValidationIssues(ByVal Issues As Collection)
    Dim Issue As AppManifestValidationIssue

    EnsureValidationControls
    ValidationListBox.Clear
    If Issues Is Nothing Then
        ValidationListBox.AddItem "Information"
        ValidationListBox.List(0, 1) = "VMF-VAL-000"
        ValidationListBox.List(0, 2) = vbNullString
        ValidationListBox.List(0, 3) = vbNullString
        ValidationListBox.List(0, 4) = "Manifest validation passed."
        Exit Sub
    End If

    If Issues.Count = 0 Then
        ValidationListBox.AddItem "Information"
        ValidationListBox.List(0, 1) = "VMF-VAL-000"
        ValidationListBox.List(0, 2) = vbNullString
        ValidationListBox.List(0, 3) = vbNullString
        ValidationListBox.List(0, 4) = "Manifest validation passed."
        Exit Sub
    End If

    For Each Issue In Issues
        ValidationListBox.AddItem Issue.SeverityText
        ValidationListBox.List(ValidationListBox.ListCount - 1, 1) = Issue.Code
        ValidationListBox.List(ValidationListBox.ListCount - 1, 2) = Issue.ModuleName
        ValidationListBox.List(ValidationListBox.ListCount - 1, 3) = Issue.MemberName
        ValidationListBox.List(ValidationListBox.ListCount - 1, 4) = Issue.Message
    Next Issue
End Sub

Private Sub NavigateToValidationIssue()
    Dim Issue As AppManifestValidationIssue
    Dim ModuleIndex As Long
    Dim MemberIndex As Long
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object

    If ValidationIssues Is Nothing Then
        Exit Sub
    End If

    If ValidationListBox.ListIndex < 0 Then
        Exit Sub
    End If

    If ValidationListBox.ListIndex + 1 > ValidationIssues.Count Then
        Exit Sub
    End If

    Set Issue = ValidationIssues.Item(ValidationListBox.ListIndex + 1)
    If ComIsBlankText(Issue.ModuleName) Then
        Exit Sub
    End If

    For ModuleIndex = 1 To Modules.Count
        Set ModuleInfo = Modules.Item(ModuleIndex)
        If StrComp(CStr(ModuleInfo("ModuleName")), Issue.ModuleName, vbTextCompare) = 0 Then
            SelectedModuleIndex = ModuleIndex
            lstModules.ListIndex = ModuleIndex - 1
            ShowSelectedModule
            If Not ComIsBlankText(Issue.MemberName) Then
                Set Members = ModuleInfo("Members")
                For MemberIndex = 1 To Members.Count
                    Set MemberInfo = Members.Item(MemberIndex)
                    If StrComp(CStr(MemberInfo("Name")), Issue.MemberName, vbTextCompare) = 0 Then
                        SelectedMemberIndex = MemberIndex
                        lstMembers.ListIndex = MemberIndex - 1
                        ShowSelectedMember
                        Exit For
                    End If
                Next MemberIndex
            End If
            Exit For
        End If
    Next ModuleIndex
End Sub
Private Function CreateMemberFromFields() As Object
    Set CreateMemberFromFields = AppCreateManifestEditorMember( _
        txtMemberName.Text, _
        txtMemberTypeName.Text, _
        cboAccessor.Text, _
        txtInitialValue.Text, _
        chkCreateInstance.Value)
End Function

Private Sub SelectModuleFromExplorer(ByVal ModuleIndex As Long)
    If ModuleIndex <= 0 Or ModuleIndex > Modules.Count Then
        SetUiState "Error"
        MsgBox "Module selection failed.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    SelectedModuleIndex = ModuleIndex
    SelectedMemberIndex = 0
    SetSingleGenerateTarget CStr(Modules.Item(SelectedModuleIndex)("ModuleName"))
    RefreshProjectExplorer
    ShowSelectedModule
    If Not PreviewCodeTextBox Is Nothing Then
        If PreviewCodeTextBox.Visible Then RefreshPreviewPane
    End If
End Sub

Private Sub ResetStudioState()
    Set ValidationStatusByModule = CreateObject("Scripting.Dictionary")
    Set GenerateTargetByModule = CreateObject("Scripting.Dictionary")
    CurrentUiState = "Ready"
End Sub

Private Sub SetUiState(ByVal StateName As String)
    CurrentUiState = StateName
    RefreshCommandState
End Sub

Private Sub MarkDirty()
    If SuppressDirtyEvents Then Exit Sub
    IsDirty = True
    CurrentUiState = "Dirty"
    RefreshProjectExplorer
    RefreshCommandState
End Sub

Private Sub RefreshCommandState()
    Dim HasProject As Boolean
    Dim HasModule As Boolean
    Dim Busy As Boolean
    Dim HasErrors As Boolean

    HasProject = False
    If Not Modules Is Nothing Then HasProject = (Modules.Count > 0)
    HasModule = (SelectedModuleIndex > 0 And HasProject)
    Busy = (CurrentUiState = "Loading" Or CurrentUiState = "Validating" Or CurrentUiState = "Generating" Or GenerateInProgress)
    HasErrors = HasValidationErrors()

    btnSave.Enabled = HasProject And Not Busy
    btnModuleAdd.Enabled = Not Busy
    btnModuleApply.Enabled = HasModule And Not Busy
    btnModuleDelete.Enabled = HasModule And Not Busy
    btnMemberAdd.Enabled = HasModule And Not Busy
    btnMemberEdit.Enabled = HasModule And SelectedMemberIndex > 0 And Not Busy
    btnMemberDelete.Enabled = HasModule And SelectedMemberIndex > 0 And Not Busy
    If Not PreviewButton Is Nothing Then PreviewButton.Enabled = HasModule And Not Busy
    If Not ValidationButton Is Nothing Then ValidationButton.Enabled = HasProject And Not Busy
    If Not GenerateAllButton Is Nothing Then GenerateAllButton.Enabled = HasProject And Not Busy And Not HasErrors
    If Not GenerateSelectedButton Is Nothing Then GenerateSelectedButton.Enabled = HasModule And Not Busy And Not HasErrors
    If Not TemplateManagerButton Is Nothing Then TemplateManagerButton.Enabled = HasProject And Not Busy
    If Not SettingsButton Is Nothing Then SettingsButton.Enabled = Not Busy
    If Not BackupButton Is Nothing Then BackupButton.Enabled = Not Busy
    If Not SelfCheckButton Is Nothing Then SelfCheckButton.Enabled = Not Busy

    txtModuleName.Enabled = HasModule And Not Busy
    txtLayer.Enabled = HasModule And Not Busy
    cboModuleType.Enabled = HasModule And Not Busy
    txtTemplatePath.Enabled = HasModule And Not Busy
    lstModules.Enabled = Not Busy
    lstMembers.Enabled = HasModule And Not Busy
End Sub

Private Function HasValidationErrors() As Boolean
    Dim Key As Variant
    If ValidationStatusByModule Is Nothing Then Exit Function
    For Each Key In ValidationStatusByModule.Keys
        If CStr(ValidationStatusByModule.Item(CStr(Key))) = "Error" Then
            HasValidationErrors = True
            Exit Function
        End If
    Next Key
End Function

Private Sub UpdateValidationStatusFromIssues(ByVal Issues As Collection, ByVal ScopeModuleName As String)
    Dim ModuleInfo As Object
    Dim Issue As AppManifestValidationIssue
    Dim ModuleName As String

    If ValidationStatusByModule Is Nothing Then Set ValidationStatusByModule = CreateObject("Scripting.Dictionary")
    If ComIsBlankText(ScopeModuleName) Then
        For Each ModuleInfo In Modules
            ValidationStatusByModule(CStr(ModuleInfo("ModuleName"))) = "Valid"
        Next ModuleInfo
    Else
        ValidationStatusByModule(ScopeModuleName) = "Valid"
    End If

    If Not Issues Is Nothing Then
        For Each Issue In Issues
            ModuleName = Issue.ModuleName
            If ComIsBlankText(ModuleName) Then ModuleName = ScopeModuleName
            If Not ComIsBlankText(ModuleName) Then
                If Issue.SeverityText = "Error" Then
                    ValidationStatusByModule(ModuleName) = "Error"
                ElseIf Issue.SeverityText = "Warning" Then
                    If Not ValidationStatusByModule.Exists(ModuleName) Then
                        ValidationStatusByModule(ModuleName) = "Warning"
                    ElseIf ValidationStatusByModule(ModuleName) <> "Error" Then
                        ValidationStatusByModule(ModuleName) = "Warning"
                    End If
                End If
            End If
        Next Issue
    End If
    RefreshProjectExplorer
End Sub

Private Function ValidationMarker(ByVal ModuleName As String) As String
    Dim StatusText As String
    StatusText = "NotValidated"
    If Not ValidationStatusByModule Is Nothing Then
        If ValidationStatusByModule.Exists(ModuleName) Then StatusText = CStr(ValidationStatusByModule(ModuleName))
    End If

    Select Case StatusText
        Case "Error"
            ValidationMarker = "[E]"
        Case "Warning"
            ValidationMarker = "[W]"
        Case "Valid"
            ValidationMarker = "[V]"
        Case Else
            ValidationMarker = "[-]"
    End Select
End Function

Private Sub SetSingleGenerateTarget(ByVal ModuleName As String)
    Set GenerateTargetByModule = CreateObject("Scripting.Dictionary")
    If Not ComIsBlankText(ModuleName) Then GenerateTargetByModule(ModuleName) = True
End Sub

Private Function IsGenerateTarget(ByVal ModuleName As String) As Boolean
    If GenerateTargetByModule Is Nothing Then Exit Function
    IsGenerateTarget = GenerateTargetByModule.Exists(ModuleName)
End Function

Private Function IsDirtyModule(ByVal ModuleName As String) As Boolean
    IsDirtyModule = IsDirty
End Function
Private Sub RefreshProjectExplorer()
    Dim ModuleInfo As Object
    Dim ModuleName As String
    Dim RowIndex As Long

    lstModules.Clear
    For Each ModuleInfo In Modules
        ModuleName = CStr(ModuleInfo("ModuleName"))
        lstModules.AddItem ValidationMarker(ModuleName)
        RowIndex = lstModules.ListCount - 1
        lstModules.List(RowIndex, 1) = CStr(ModuleInfo("Layer"))
        lstModules.List(RowIndex, 2) = ModuleName & IIf(IsDirtyModule(ModuleName), " *", vbNullString)
        lstModules.List(RowIndex, 3) = CStr(ModuleInfo("ModuleType"))
        lstModules.List(RowIndex, 4) = IIf(IsDirtyModule(ModuleName), "Dirty", vbNullString)
        lstModules.List(RowIndex, 5) = IIf(IsGenerateTarget(ModuleName), "Target", vbNullString)
    Next ModuleInfo
    RefreshCommandState
End Sub

Private Sub RefreshMemberList(ByVal ModuleInfo As Object)
    Dim Members As Collection
    Dim MemberInfo As Object

    lstMembers.Clear
    If ModuleInfo Is Nothing Then
        Exit Sub
    End If

    Set Members = ModuleInfo("Members")
    For Each MemberInfo In Members
        lstMembers.AddItem CStr(MemberInfo("Name"))
        lstMembers.List(lstMembers.ListCount - 1, 1) = CStr(MemberInfo("TypeName"))
        lstMembers.List(lstMembers.ListCount - 1, 2) = CStr(MemberInfo("Accessor"))
        lstMembers.List(lstMembers.ListCount - 1, 3) = CStr(MemberInfo("InitialValue"))
        lstMembers.List(lstMembers.ListCount - 1, 4) = CStr(MemberInfo("CreateInstance"))
    Next MemberInfo
End Sub

Private Sub ShowSelectedModule()
    Dim ModuleInfo As Object

    SuppressDirtyEvents = True
    If SelectedModuleIndex <= 0 Then
        ClearModuleFields
        RefreshMemberList Nothing
        SuppressDirtyEvents = False
        RefreshCommandState
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    txtModuleName.Text = CStr(ModuleInfo("ModuleName"))
    txtLayer.Text = CStr(ModuleInfo("Layer"))
    cboModuleType.Text = CStr(ModuleInfo("ModuleType"))
    txtTemplatePath.Text = CStr(ModuleInfo("TemplatePath"))
    ClearMemberFields
    RefreshMemberList ModuleInfo
    SuppressDirtyEvents = False
    RefreshCommandState
End Sub

Private Sub ShowSelectedMember()
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object

    SuppressDirtyEvents = True

    If SelectedModuleIndex <= 0 Or SelectedMemberIndex <= 0 Then
        ClearMemberFields
        SuppressDirtyEvents = False
        RefreshCommandState
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    Set Members = ModuleInfo("Members")
    Set MemberInfo = Members.Item(SelectedMemberIndex)
    txtMemberName.Text = CStr(MemberInfo("Name"))
    txtMemberTypeName.Text = CStr(MemberInfo("TypeName"))
    cboAccessor.Text = CStr(MemberInfo("Accessor"))
    txtInitialValue.Text = CStr(MemberInfo("InitialValue"))
    chkCreateInstance.Value = (StrComp(CStr(MemberInfo("CreateInstance")), "True", vbTextCompare) = 0)
    SuppressDirtyEvents = False
    RefreshCommandState
End Sub

Private Sub txtModuleName_Change()
    MarkDirty
End Sub

Private Sub txtLayer_Change()
    MarkDirty
End Sub

Private Sub cboModuleType_Change()
    MarkDirty
End Sub

Private Sub txtTemplatePath_Change()
    MarkDirty
End Sub

Private Sub txtMemberName_Change()
    MarkDirty
End Sub

Private Sub txtMemberTypeName_Change()
    MarkDirty
End Sub

Private Sub cboAccessor_Change()
    MarkDirty
End Sub

Private Sub txtInitialValue_Change()
    MarkDirty
End Sub

Private Sub chkCreateInstance_Click()
    MarkDirty
End Sub
Private Sub ClearModuleFields()
    txtModuleName.Text = vbNullString
    txtLayer.Text = vbNullString
    cboModuleType.Text = vbNullString
    txtTemplatePath.Text = vbNullString
End Sub

Private Sub ClearMemberFields()
    txtMemberName.Text = vbNullString
    txtMemberTypeName.Text = vbNullString
    cboAccessor.Text = vbNullString
    txtInitialValue.Text = vbNullString
    chkCreateInstance.Value = False
End Sub























