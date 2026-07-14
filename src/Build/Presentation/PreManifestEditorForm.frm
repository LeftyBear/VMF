VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} PreManifestEditorForm 
   Caption         =   "Manifest Editor"
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
Public Sub PreOpenManifest(ByVal ManifestPath As String)
    txtManifestPath.Text = ManifestPath
    LoadManifest
End Sub

Private Sub UserForm_Initialize()
    SelectedModuleIndex = 0
    SelectedMemberIndex = 0
    Set Modules = New Collection

    Me.Width = 620
    Me.Height = 460
    lblTemplatePath.Width = 78
    txtTemplatePath.Left = lblTemplatePath.Left + lblTemplatePath.Width + 8
    txtTemplatePath.Width = 196
    btnModuleAdd.Height = 22
    btnModuleDelete.Height = 22
    btnModuleApply.Height = 22
    btnLoad.Caption = "Browse..."
    CreatePreviewButton

    cboModuleType.Clear
    cboModuleType.AddItem "ClassModule"
    cboModuleType.AddItem "StandardModule"

    cboAccessor.Clear
    cboAccessor.AddItem "GetLet"
    cboAccessor.AddItem "GetSet"
    cboAccessor.AddItem "GetOnly"

    lstModules.ColumnCount = 3
    lstModules.ColumnWidths = "150 pt;90 pt;110 pt"
    lstMembers.ColumnCount = 5
    lstMembers.ColumnWidths = "110 pt;110 pt;80 pt;110 pt;80 pt"
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
    Set Result = AppSaveManifestEditorModel(txtManifestPath.Text, Modules)
    MsgBox Result.Message, IIf(Result.IsSuccess, vbInformation, vbExclamation), "Manifest Editor"
End Sub

Private Sub btnClose_Click()
    Unload Me
End Sub

Private Sub lstModules_Click()
    If lstModules.ListIndex < 0 Then
        Exit Sub
    End If

    SelectedModuleIndex = lstModules.ListIndex + 1
    SelectedMemberIndex = 0
    ShowSelectedModule
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
    RefreshModuleList
    lstModules.ListIndex = SelectedModuleIndex - 1
    ShowSelectedModule
End Sub

Private Sub btnModuleApply_Click()
    ApplyModuleFieldsToSelection
    RefreshModuleList
End Sub

Private Sub PreviewButton_Click()
    If SelectedModuleIndex <= 0 Then
        MsgBox "Select a module first.", vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    ShowPreviewPane
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

    RefreshModuleList
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

    SelectedModuleIndex = 0
    SelectedMemberIndex = 0
    Set Result = AppLoadManifestEditorModel(txtManifestPath.Text, Modules)
    If Result.IsFailure Then
        MsgBox Result.Message, vbExclamation, "Manifest Editor"
        Exit Sub
    End If

    RefreshModuleList
    If Modules.Count > 0 Then
        SelectedModuleIndex = 1
        lstModules.ListIndex = 0
        ShowSelectedModule
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

Private Sub ShowPreviewPane()
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
Private Function CreateMemberFromFields() As Object
    Set CreateMemberFromFields = AppCreateManifestEditorMember( _
        txtMemberName.Text, _
        txtMemberTypeName.Text, _
        cboAccessor.Text, _
        txtInitialValue.Text, _
        chkCreateInstance.Value)
End Function

Private Sub RefreshModuleList()
    Dim ModuleInfo As Object

    lstModules.Clear
    For Each ModuleInfo In Modules
        lstModules.AddItem CStr(ModuleInfo("ModuleName"))
        lstModules.List(lstModules.ListCount - 1, 1) = CStr(ModuleInfo("Layer"))
        lstModules.List(lstModules.ListCount - 1, 2) = CStr(ModuleInfo("ModuleType"))
    Next ModuleInfo
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

    If SelectedModuleIndex <= 0 Then
        ClearModuleFields
        RefreshMemberList Nothing
        Exit Sub
    End If

    Set ModuleInfo = Modules.Item(SelectedModuleIndex)
    txtModuleName.Text = CStr(ModuleInfo("ModuleName"))
    txtLayer.Text = CStr(ModuleInfo("Layer"))
    cboModuleType.Text = CStr(ModuleInfo("ModuleType"))
    txtTemplatePath.Text = CStr(ModuleInfo("TemplatePath"))
    ClearMemberFields
    RefreshMemberList ModuleInfo
End Sub

Private Sub ShowSelectedMember()
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object

    If SelectedModuleIndex <= 0 Or SelectedMemberIndex <= 0 Then
        ClearMemberFields
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


