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
    CurrentUiState = "NoProject"
    IsDirty = False

    Me.Width = 980
    Me.Height = 520
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

    FilePath = BuildPathResolver.CombinePath(BuildPathResolver.TemplatesDirectoryPath(), "NewTemplate.txt")
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

    If MsgBox("Generate from the current editor values? Unsaved edits are included, but the manifest file is not saved.", vbOKCancel + vbQuestion, "Manifest Editor") <> vbOK Then
        Exit Sub
    End If

    ApplyModuleFieldsToSelection
    ApplyMemberFieldsToSelection
    Set Result = ValidateProjectModel()
    If AppValidationIssuesContainErrors(ValidationIssues) Then
        ShowValidationPane ValidationIssues
        MsgBox Result.Message, vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    OverwriteMode = ResolveOverwriteMode()
    If ComIsBlankText(OverwriteMode) Then
        Exit Sub
    End If

    OutputDirectory = AppDefaultManifestGenerateOutputDirectory(txtManifestPath.Text)
    Set SelectedNames = CreateSelectedModuleNames(TargetScope)
    Set Request = AppCreateManifestGenerateRequest(TargetScope, SelectedNames, OutputDirectory, OverwriteMode, False)

    GenerateInProgress = True
    SetUiState "Generating"
    SetGenerateButtonsEnabled False
    On Error GoTo GenerateFailed
    Set Result = AppGenerateManifestEditorModel(txtManifestPath.Text, Modules, Request, GenerateResult)
    Set LastGenerateResult = GenerateResult
    ShowBuildLogPane GenerateResult
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Manifest Editor"

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
    PreviewCodeTextBox.Font.Name = "Consolas"
    PreviewCodeTextBox.Font.Size = 9
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














