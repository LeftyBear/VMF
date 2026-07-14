VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} PreCodePreviewForm
   Caption         =   "Code Preview"
   ClientHeight    =   7200
   ClientLeft      =   110
   ClientTop       =   450
   ClientWidth     =   9600
   StartUpPosition =   1  'CenterOwner
End
Attribute VB_Name = "PreCodePreviewForm"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' UserForm: PreCodePreviewForm
' Layer: Presentation
' Responsibility: Read-only generated code preview.
'=========================================================================

Private ManifestPath As String
Private ModuleInfo As Object
Private CodeTextBox As MSForms.TextBox
Private ErrorTextBox As MSForms.TextBox
Private WithEvents RefreshButton As MSForms.CommandButton
Private WithEvents CopyButton As MSForms.CommandButton
Private WithEvents CloseButton As MSForms.CommandButton

Public Sub PreOpenPreview(ByVal SourceManifestPath As String, ByVal SourceModuleInfo As Object)
    ManifestPath = SourceManifestPath
    Set ModuleInfo = SourceModuleInfo
    RefreshPreview
End Sub

Private Sub UserForm_Initialize()
    Me.Width = 700
    Me.Height = 520
    CreateControls
End Sub

Private Sub RefreshButton_Click()
    RefreshPreview
End Sub

Private Sub CopyButton_Click()
    Dim Clipboard As Object

    If CodeTextBox Is Nothing Then
        Exit Sub
    End If

    If ComIsBlankText(CodeTextBox.Text) Then
        ErrorTextBox.Text = "No preview code to copy."
        Exit Sub
    End If

    Set Clipboard = CreateObject("Forms.DataObject.1")
    Clipboard.SetText CodeTextBox.Text
    Clipboard.PutInClipboard
    ErrorTextBox.Text = "Preview copied."
End Sub

Private Sub CloseButton_Click()
    Unload Me
End Sub

Private Sub RefreshPreview()
    Dim Result As ComResult
    Dim PreviewText As String

    If CodeTextBox Is Nothing Then
        Exit Sub
    End If

    CodeTextBox.Text = vbNullString
    ErrorTextBox.Text = vbNullString
    Set Result = AppPreviewManifestEditorModule(ManifestPath, ModuleInfo, PreviewText)
    If Result.IsSuccess Then
        CodeTextBox.Text = PreviewText
    Else
        ErrorTextBox.Text = Result.Message
    End If
End Sub

Private Sub CreateControls()
    Dim CodeLabel As MSForms.Label
    Dim ErrorLabel As MSForms.Label

    Set CodeLabel = Me.Controls.Add("Forms.Label.1", "lblPreviewCode", True)
    CodeLabel.Caption = "Code"
    CodeLabel.Left = 12
    CodeLabel.Top = 10
    CodeLabel.Width = 80
    CodeLabel.Height = 16

    Set CodeTextBox = Me.Controls.Add("Forms.TextBox.1", "txtPreviewCode", True)
    CodeTextBox.Left = 12
    CodeTextBox.Top = 30
    CodeTextBox.Width = 652
    CodeTextBox.Height = 340
    CodeTextBox.MultiLine = True
    CodeTextBox.ScrollBars = fmScrollBarsBoth
    CodeTextBox.WordWrap = False
    CodeTextBox.Locked = True
    CodeTextBox.Font.Name = "Consolas"
    CodeTextBox.Font.Size = 9

    Set ErrorLabel = Me.Controls.Add("Forms.Label.1", "lblPreviewError", True)
    ErrorLabel.Caption = "Error"
    ErrorLabel.Left = 12
    ErrorLabel.Top = 382
    ErrorLabel.Width = 80
    ErrorLabel.Height = 16

    Set ErrorTextBox = Me.Controls.Add("Forms.TextBox.1", "txtPreviewError", True)
    ErrorTextBox.Left = 12
    ErrorTextBox.Top = 402
    ErrorTextBox.Width = 652
    ErrorTextBox.Height = 42
    ErrorTextBox.MultiLine = True
    ErrorTextBox.ScrollBars = fmScrollBarsVertical
    ErrorTextBox.Locked = True

    Set RefreshButton = Me.Controls.Add("Forms.CommandButton.1", "btnPreviewRefresh", True)
    RefreshButton.Caption = "Refresh"
    RefreshButton.Left = 412
    RefreshButton.Top = 456
    RefreshButton.Width = 78
    RefreshButton.Height = 24

    Set CopyButton = Me.Controls.Add("Forms.CommandButton.1", "btnPreviewCopy", True)
    CopyButton.Caption = "Copy"
    CopyButton.Left = 502
    CopyButton.Top = 456
    CopyButton.Width = 72
    CopyButton.Height = 24

    Set CloseButton = Me.Controls.Add("Forms.CommandButton.1", "btnPreviewClose", True)
    CloseButton.Caption = "Close"
    CloseButton.Left = 586
    CloseButton.Top = 456
    CloseButton.Width = 78
    CloseButton.Height = 24
End Sub
