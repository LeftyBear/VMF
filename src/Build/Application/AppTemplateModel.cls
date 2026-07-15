VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateModel"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateModel
' Layer: Application
' Responsibility: Editable VMF template model for Template Manager.
' Dependencies: Common
'=========================================================================

Private Type TMember
    TemplateName As String
    TemplateType As String
    FilePath As String
    Content As String
    IsDirty As Boolean
    HasValidated As Boolean
    Placeholders As Collection
    Sections As Collection
    ValidationResults As Collection
End Type

Private This As TMember

Public Sub AppInitialize( _
    ByVal TemplateName As String, _
    ByVal TemplateType As String, _
    ByVal FilePath As String, _
    ByVal Content As String)

    This.TemplateName = Trim$(TemplateName)
    This.TemplateType = Trim$(TemplateType)
    This.FilePath = Trim$(FilePath)
    This.Content = Content
    This.IsDirty = False
    This.HasValidated = False
    Set This.Placeholders = New Collection
    Set This.Sections = New Collection
    Set This.ValidationResults = New Collection
End Sub

Public Property Get TemplateName() As String
    TemplateName = This.TemplateName
End Property

Public Property Let TemplateName(ByVal Value As String)
    This.TemplateName = Trim$(Value)
    This.IsDirty = True
End Property

Public Property Get TemplateType() As String
    TemplateType = This.TemplateType
End Property

Public Property Let TemplateType(ByVal Value As String)
    This.TemplateType = Trim$(Value)
    This.IsDirty = True
End Property

Public Property Get FilePath() As String
    FilePath = This.FilePath
End Property

Public Property Let FilePath(ByVal Value As String)
    This.FilePath = Trim$(Value)
End Property

Public Property Get Content() As String
    Content = This.Content
End Property

Public Property Let Content(ByVal Value As String)
    This.Content = Value
    This.IsDirty = True
End Property

Public Property Get IsDirty() As Boolean
    IsDirty = This.IsDirty
End Property

Public Property Let IsDirty(ByVal Value As Boolean)
    This.IsDirty = Value
End Property

Public Property Get HasValidated() As Boolean
    HasValidated = This.HasValidated
End Property

Public Property Let HasValidated(ByVal Value As Boolean)
    This.HasValidated = Value
End Property

Public Property Get Placeholders() As Collection
    Set Placeholders = This.Placeholders
End Property

Public Property Set Placeholders(ByVal Value As Collection)
    Set This.Placeholders = Value
End Property

Public Property Get Sections() As Collection
    Set Sections = This.Sections
End Property

Public Property Set Sections(ByVal Value As Collection)
    Set This.Sections = Value
End Property

Public Property Get ValidationResults() As Collection
    Set ValidationResults = This.ValidationResults
End Property

Public Property Set ValidationResults(ByVal Value As Collection)
    Set This.ValidationResults = Value
End Property
