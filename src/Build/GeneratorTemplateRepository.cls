VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "GeneratorTemplateRepository"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: GeneratorTemplateRepository
' Layer: Application
' Responsibility: Provides generator templates from external files.
' Dependencies: Common, Infrastructure
'==========================================================================

' Returns the rendered module template for the supplied name.
Public Function GetModuleTemplate(ByVal ModuleName As String) As String
    Dim TemplatePath As String
    Dim TemplateText As String

    TemplatePath = GetModuleTemplatePath()
    TemplateText = InfReadText(TemplatePath)
    GetModuleTemplate = Replace(TemplateText, "{ModuleName}", ModuleName)
End Function

' Returns the file path for the default module template.
Private Function GetModuleTemplatePath() As String
    Dim RootPath As String
    Dim AddinWorkbook As Object

    RootPath = vbNullString

    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    On Error GoTo 0

    If Not AddinWorkbook Is Nothing Then
        RootPath = AddinWorkbook.Path
    Else
        RootPath = ThisWorkbook.Path
    End If

    If ComIsBlankText(RootPath) Then
        Err.Raise ComErrInvalidState, "GeneratorTemplateRepository", "Template root path is unavailable."
    End If

    GetModuleTemplatePath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "ModuleTemplate.txt"
End Function
