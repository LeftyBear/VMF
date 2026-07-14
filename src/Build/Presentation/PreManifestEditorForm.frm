VERSION 5.00
Begin VB.UserForm PreManifestEditorForm
   Caption         =   "Manifest Editor"
   ClientHeight    =   7200
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   10200
   StartUpPosition =   1  'CenterOwner
   Begin MSForms.TextBox txtManifestPath
      Height          =   300
      Left            =   120
      TabIndex        =   0
      Top             =   240
      Width           =   7680
   End
   Begin MSForms.CommandButton btnLoad
      Caption         =   "Load"
      Height          =   300
      Left            =   7920
      TabIndex        =   1
      Top             =   240
      Width           =   960
   End
   Begin MSForms.ListBox lstModules
      Height          =   3000
      Left            =   120
      TabIndex        =   2
      Top             =   840
      Width           =   4200
   End
   Begin MSForms.TextBox txtModuleName
      Height          =   300
      Left            =   5520
      TabIndex        =   3
      Top             =   840
      Width           =   2520
   End
   Begin MSForms.TextBox txtLayer
      Height          =   300
      Left            =   5520
      TabIndex        =   4
      Top             =   1260
      Width           =   2520
   End
   Begin MSForms.ComboBox cboModuleType
      Height          =   300
      Left            =   5520
      TabIndex        =   5
      Top             =   1680
      Width           =   2520
   End
   Begin MSForms.TextBox txtTemplatePath
      Height          =   300
      Left            =   5520
      TabIndex        =   6
      Top             =   2100
      Width           =   4200
   End
   Begin MSForms.CommandButton btnModuleAdd
      Caption         =   "Add Module"
      Height          =   300
      Left            =   5520
      TabIndex        =   7
      Top             =   2580
      Width           =   1320
   End
   Begin MSForms.CommandButton btnModuleDelete
      Caption         =   "Delete Module"
      Height          =   300
      Left            =   6960
      TabIndex        =   8
      Top             =   2580
      Width           =   1320
   End
   Begin MSForms.CommandButton btnModuleApply
      Caption         =   "Apply Module"
      Height          =   300
      Left            =   8400
      TabIndex        =   9
      Top             =   2580
      Width           =   1320
   End
   Begin MSForms.ListBox lstMembers
      Height          =   2160
      Left            =   120
      TabIndex        =   10
      Top             =   4320
      Width           =   5160
   End
   Begin MSForms.TextBox txtMemberName
      Height          =   300
      Left            =   6480
      TabIndex        =   11
      Top             =   4320
      Width           =   2040
   End
   Begin MSForms.TextBox txtMemberTypeName
      Height          =   300
      Left            =   6480
      TabIndex        =   12
      Top             =   4680
      Width           =   2040
   End
   Begin MSForms.ComboBox cboAccessor
      Height          =   300
      Left            =   6480
      TabIndex        =   13
      Top             =   5040
      Width           =   2040
   End
   Begin MSForms.TextBox txtInitialValue
      Height          =   300
      Left            =   6480
      TabIndex        =   14
      Top             =   5400
      Width           =   2040
   End
   Begin MSForms.CheckBox chkCreateInstance
      Caption         =   "CreateInstance"
      Height          =   300
      Left            =   6480
      TabIndex        =   15
      Top             =   5760
      Width           =   1680
   End
   Begin MSForms.CommandButton btnMemberAdd
      Caption         =   "Add Member"
      Height          =   300
      Left            =   5400
      TabIndex        =   16
      Top             =   6240
      Width           =   1320
   End
   Begin MSForms.CommandButton btnMemberEdit
      Caption         =   "Edit Member"
      Height          =   300
      Left            =   6840
      TabIndex        =   17
      Top             =   6240
      Width           =   1320
   End
   Begin MSForms.CommandButton btnMemberDelete
      Caption         =   "Delete Member"
      Height          =   300
      Left            =   8280
      TabIndex        =   18
      Top             =   6240
      Width           =   1320
   End
   Begin MSForms.CommandButton btnSave
      Caption         =   "Save"
      Height          =   360
      Left            =   7560
      TabIndex        =   19
      Top             =   6720
      Width           =   960
   End
   Begin MSForms.CommandButton btnClose
      Caption         =   "Close"
      Height          =   360
      Left            =   8640
      TabIndex        =   20
      Top             =   6720
      Width           =   960
   End
   Begin MSForms.Label lblModuleName
      Caption         =   "ModuleName"
      Height          =   240
      Left            =   4440
      Top             =   900
      Width           =   960
   End
   Begin MSForms.Label lblLayer
      Caption         =   "Layer"
      Height          =   240
      Left            =   4440
      Top             =   1320
      Width           =   960
   End
   Begin MSForms.Label lblModuleType
      Caption         =   "ModuleType"
      Height          =   240
      Left            =   4440
      Top             =   1740
      Width           =   960
   End
   Begin MSForms.Label lblTemplatePath
      Caption         =   "TemplatePath"
      Height          =   240
      Left            =   4440
      Top             =   2160
      Width           =   960
   End
   Begin MSForms.Label lblMemberName
      Caption         =   "Name"
      Height          =   240
      Left            =   5400
      Top             =   4380
      Width           =   960
   End
   Begin MSForms.Label lblMemberTypeName
      Caption         =   "TypeName"
      Height          =   240
      Left            =   5400
      Top             =   4740
      Width           =   960
   End
   Begin MSForms.Label lblAccessor
      Caption         =   "Accessor"
      Height          =   240
      Left            =   5400
      Top             =   5100
      Width           =   960
   End
   Begin MSForms.Label lblInitialValue
      Caption         =   "InitialValue"
      Height          =   240
      Left            =   5400
      Top             =   5460
      Width           =   960
   End
End
Attribute VB_Name = "PreManifestEditorForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
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

Public Sub PreOpenManifest(ByVal ManifestPath As String)
    txtManifestPath.Text = ManifestPath
    LoadManifest
End Sub

Private Sub UserForm_Initialize()
    SelectedModuleIndex = 0
    SelectedMemberIndex = 0
    Set Modules = New Collection

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
    LoadManifest
End Sub

Private Sub btnSave_Click()
    Dim Result As ComResult

    ApplyModuleFieldsToSelection
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
