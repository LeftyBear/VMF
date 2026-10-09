VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateDerivationItem"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateDerivationItem
' Layer: Application
' Responsibility: Represent one approved Template Derivation binding.
' Dependencies: Common
'=========================================================================

Private Type TMember
    SourceIndex As Long
    ModuleName As String
    ModuleType As String
    LayerName As String
    TemplateKey As String
    TemplatePath As String
    TemplateRole As String
    SelectionRuleId As String
    DerivationReason As String
    IsGeneratable As Boolean
    UnsupportedReason As String
    IsInitialized As Boolean
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal SourceIndex As Long, _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByVal TemplateKey As String, _
    ByVal TemplatePath As String, _
    ByVal TemplateRole As String, _
    ByVal SelectionRuleId As String, _
    ByVal DerivationReason As String, _
    ByVal IsGeneratable As Boolean, _
    ByVal UnsupportedReason As String)

    If SourceIndex < 1 Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationItem.AppInitialize", "SourceIndex must be positive."
    End If
    ComRequireText ModuleName, "ModuleName", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText ModuleType, "ModuleType", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText LayerName, "LayerName", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText TemplateKey, "TemplateKey", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText TemplatePath, "TemplatePath", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText TemplateRole, "TemplateRole", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText SelectionRuleId, "SelectionRuleId", "AppTemplateDerivationItem.AppInitialize"
    ComRequireText DerivationReason, "DerivationReason", "AppTemplateDerivationItem.AppInitialize"
    If Not IsGeneratable Or Not ComIsBlankText(UnsupportedReason) Then
        Err.Raise ComErrInvalidArgument, "AppTemplateDerivationItem.AppInitialize", "A returned derivation item must be generatable and supported."
    End If

    This.SourceIndex = SourceIndex
    This.ModuleName = ModuleName
    This.ModuleType = ModuleType
    This.LayerName = LayerName
    This.TemplateKey = TemplateKey
    This.TemplatePath = TemplatePath
    This.TemplateRole = TemplateRole
    This.SelectionRuleId = SelectionRuleId
    This.DerivationReason = DerivationReason
    This.IsGeneratable = True
    This.UnsupportedReason = vbNullString
    This.IsInitialized = True
End Sub

Public Property Get SourceIndex() As Long
    EnsureInitialized
    SourceIndex = This.SourceIndex
End Property

Public Property Get ModuleName() As String
    EnsureInitialized
    ModuleName = This.ModuleName
End Property

Public Property Get ModuleType() As String
    EnsureInitialized
    ModuleType = This.ModuleType
End Property

Public Property Get LayerName() As String
    EnsureInitialized
    LayerName = This.LayerName
End Property

Public Property Get TemplateKey() As String
    EnsureInitialized
    TemplateKey = This.TemplateKey
End Property

Public Property Get TemplatePath() As String
    EnsureInitialized
    TemplatePath = This.TemplatePath
End Property

Public Property Get TemplateRole() As String
    EnsureInitialized
    TemplateRole = This.TemplateRole
End Property

Public Property Get SelectionRuleId() As String
    EnsureInitialized
    SelectionRuleId = This.SelectionRuleId
End Property

Public Property Get DerivationReason() As String
    EnsureInitialized
    DerivationReason = This.DerivationReason
End Property

Public Property Get IsGeneratable() As Boolean
    EnsureInitialized
    IsGeneratable = This.IsGeneratable
End Property

Public Property Get UnsupportedReason() As String
    EnsureInitialized
    UnsupportedReason = This.UnsupportedReason
End Property

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "AppTemplateDerivationItem", "Template derivation item is not initialized."
    End If
End Sub
