VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfTemplateProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfTemplateProvider
' Layer: Infrastructure
' Responsibility: Create template objects from external template files.
' Dependencies: Common, Infrastructure
'==========================================================================

Private FileSystemProvider As InfFileSystemProvider

' Initializes the provider with required infrastructure collaborators.
Public Sub InfInitialize(ByVal Provider As InfFileSystemProvider)
    If Provider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfInitialize", "FileSystemProvider is required."
    End If

    Set FileSystemProvider = Provider
End Sub

' Creates a template from a path to a template file.
Public Function InfCreateTemplate(ByVal TemplatePath As String) As InfTemplate
    Dim TemplateText As String
    Dim Template As InfTemplate

    ComRequireText TemplatePath, "TemplatePath", "InfCreateTemplate"

    If FileSystemProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfCreateTemplate", "TemplateProvider is not initialized."
    End If

    If Not FileSystemProvider.InfFileExists(TemplatePath) Then
        Err.Raise ComErrInvalidArgument, "InfCreateTemplate", "TemplatePath does not exist."
    End If

    TemplateText = FileSystemProvider.InfReadText(TemplatePath)

    Set Template = New InfTemplate
    Template.InfSetTemplateName GetTemplateNameFromPath(TemplatePath)
    Template.InfSetTemplateText TemplateText

    Set InfCreateTemplate = Template
End Function

' Creates a template directly from supplied text content.
Public Function InfCreateTemplateFromText(ByVal TemplateName As String, ByVal TemplateText As String) As InfTemplate
    Dim Template As InfTemplate

    ComRequireText TemplateName, "TemplateName", "InfCreateTemplateFromText"

    Set Template = New InfTemplate
    Template.InfSetTemplateName TemplateName
    Template.InfSetTemplateText TemplateText

    Set InfCreateTemplateFromText = Template
End Function

' Returns the default module template path.
Public Function InfGetModuleTemplatePath() As String
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
        Err.Raise ComErrInvalidState, "InfTemplateProvider", "Template root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    InfGetModuleTemplatePath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "ModuleTemplate.txt"
End Function

' Returns the default class template path.
Public Function InfGetClassTemplatePath() As String
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
        Err.Raise ComErrInvalidState, "InfTemplateProvider", "Template root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    InfGetClassTemplatePath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "ClassTemplate.txt"
End Function

Private Function ResolveWorkspaceRootPath(ByVal CandidatePath As String) As String
    Dim FileSystem As Object
    Dim ParentPath As String
    Dim GrandParentPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")

    If FileSystem.FolderExists(FileSystem.BuildPath(CandidatePath, "src\Build")) Then
        ResolveWorkspaceRootPath = CandidatePath
        Exit Function
    End If

    ParentPath = FileSystem.GetParentFolderName(CandidatePath)
    If Len(ParentPath) > 0 Then
        If FileSystem.FolderExists(FileSystem.BuildPath(ParentPath, "src\Build")) Then
            ResolveWorkspaceRootPath = ParentPath
            Exit Function
        End If
    End If

    GrandParentPath = FileSystem.GetParentFolderName(ParentPath)
    If Len(GrandParentPath) > 0 Then
        If FileSystem.FolderExists(FileSystem.BuildPath(GrandParentPath, "src\Build")) Then
            ResolveWorkspaceRootPath = GrandParentPath
            Exit Function
        End If
    End If

    ResolveWorkspaceRootPath = CandidatePath
End Function

Private Function GetTemplateNameFromPath(ByVal TemplatePath As String) As String
    Dim FileName As String
    Dim SeparatorIndex As Long
    Dim ExtensionIndex As Long

    SeparatorIndex = InStrRev(TemplatePath, "\")
    If SeparatorIndex = 0 Then
        SeparatorIndex = InStrRev(TemplatePath, "/")
    End If

    If SeparatorIndex > 0 Then
        FileName = Mid$(TemplatePath, SeparatorIndex + 1)
    Else
        FileName = TemplatePath
    End If

    ExtensionIndex = InStrRev(FileName, ".")
    If ExtensionIndex > 0 Then
        FileName = Left$(FileName, ExtensionIndex - 1)
    End If

    GetTemplateNameFromPath = FileName
End Function
