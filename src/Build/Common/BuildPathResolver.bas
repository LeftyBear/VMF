Option Explicit
Attribute VB_Name = "BuildPathResolver"

'=========================================================================
' Module: BuildPathResolver
' Layer: Common
' Responsibility: Resolve VMF repository paths from the Build add-in location.
' Dependencies: None
'=========================================================================

Private Const RepositoryRootMarkerFileName As String = ".vmf-root"

Public Function RepositoryRootPath() As String
    Dim FileSystem As Object
    Dim StartPath As String
    Dim FallbackPath As String
    Dim ResolvedPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    StartPath = ThisWorkbook.Path
    If TryResolveRepositoryRootPath(FileSystem, StartPath, ResolvedPath) Then
        RepositoryRootPath = ResolvedPath
        Exit Function
    End If

    FallbackPath = ResolveOpenBuildWorkbookPath()
    If StrComp(FallbackPath, StartPath, vbTextCompare) <> 0 Then
        If TryResolveRepositoryRootPath(FileSystem, FallbackPath, ResolvedPath) Then
            RepositoryRootPath = ResolvedPath
            Exit Function
        End If
    End If

    Err.Raise ComErrInvalidState, "BuildPathResolver.RepositoryRootPath", _
        "VMF repository root could not be resolved from:" & vbCrLf & StartPath
End Function

Private Function TryResolveRepositoryRootPath( _
    ByVal FileSystem As Object, _
    ByVal StartPath As String, _
    ByRef ResolvedPath As String) As Boolean

    Dim CurrentPath As String
    Dim ParentPath As String

    If Len(Trim$(StartPath)) = 0 Then
        TryResolveRepositoryRootPath = False
        Exit Function
    End If

    CurrentPath = FileSystem.GetAbsolutePathName(StartPath)

    Do
        If FileSystem.FileExists(FileSystem.BuildPath(CurrentPath, RepositoryRootMarkerFileName)) Then
            ResolvedPath = CurrentPath
            TryResolveRepositoryRootPath = True
            Exit Function
        End If

        ParentPath = FileSystem.GetParentFolderName(CurrentPath)
        If Len(ParentPath) = 0 Or StrComp(ParentPath, CurrentPath, vbTextCompare) = 0 Then
            Exit Do
        End If

        CurrentPath = ParentPath
    Loop

    TryResolveRepositoryRootPath = False
End Function

Private Function ResolveOpenBuildWorkbookPath() As String
    Dim AddinWorkbook As Object

    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    On Error GoTo 0

    If Not AddinWorkbook Is Nothing Then
        ResolveOpenBuildWorkbookPath = AddinWorkbook.Path
    Else
        ResolveOpenBuildWorkbookPath = vbNullString
    End If
End Function

Public Function TemplatesDirectoryPath() As String
    TemplatesDirectoryPath = CombinePath(RepositoryRootPath(), "templates")
End Function

Public Function ApplicationsDirectoryPath() As String
    ApplicationsDirectoryPath = CombinePath(RepositoryRootPath(), "applications")
End Function

Public Function ApplicationDirectoryPath(ByVal applicationName As String) As String
    ComRequireText applicationName, "applicationName", "BuildPathResolver.ApplicationDirectoryPath"
    ApplicationDirectoryPath = CombinePath(ApplicationsDirectoryPath(), applicationName)
End Function

Public Function ManifestFilePath(ByVal applicationName As String) As String
    ManifestFilePath = CombinePath(ApplicationDirectoryPath(applicationName), "manifest.yaml")
End Function

Public Function CandidatesDirectoryPath() As String
    CandidatesDirectoryPath = CombinePath(RepositoryRootPath(), "candidates")
End Function

Public Function SpecsDirectoryPath() As String
    SpecsDirectoryPath = CombinePath(RepositoryRootPath(), "specs")
End Function

Public Function CombinePath(ByVal basePath As String, ByVal childPath As String) As String
    Dim FileSystem As Object

    ComRequireText basePath, "basePath", "BuildPathResolver.CombinePath"
    ComRequireText childPath, "childPath", "BuildPathResolver.CombinePath"

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    CombinePath = FileSystem.BuildPath(basePath, childPath)
End Function
