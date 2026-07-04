VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfFileSystemProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfFileSystemProvider
' Layer: Infrastructure
' Responsibility: File system operations.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const InfFileSystemObjectProgId As String = "Scripting.FileSystemObject"
Private Const InfOpenForReading As Long = 1
Private Const InfDoNotCreateMissingFile As Boolean = False
Private Const InfCreateAsAscii As Boolean = False

'=========================================================================
' Public API
'=========================================================================

' Returns True when FilePath exists and is a file.
'
' Parameters:
'   FilePath: Path to inspect.
'
' Return value:
'   True when the file exists.
'
' Raised errors:
'   ComErrInvalidArgument when FilePath is blank.
Public Function InfFileExists(ByVal FilePath As String) As Boolean
    ComRequireText FilePath, "FilePath", "InfFileExists"

    InfFileExists = CreateFileSystemObject().FileExists(FilePath)
End Function

' Returns True when FolderPath exists and is a folder.
'
' Parameters:
'   FolderPath: Path to inspect.
'
' Return value:
'   True when the folder exists.
'
' Raised errors:
'   ComErrInvalidArgument when FolderPath is blank.
Public Function InfFolderExists(ByVal FolderPath As String) As Boolean
    ComRequireText FolderPath, "FolderPath", "InfFolderExists"

    InfFolderExists = CreateFileSystemObject().FolderExists(FolderPath)
End Function

' Ensures FolderPath exists.
'
' Parameters:
'   FolderPath: Folder path to create when missing.
'
' Return value:
'   Successful ComResult when the folder exists or was created.
'
' Raised errors:
'   ComErrInvalidArgument when FolderPath is blank.
Public Function InfEnsureFolder(ByVal FolderPath As String) As ComResult
    Dim FileSystem As Object

    ComRequireText FolderPath, "FolderPath", "InfEnsureFolder"

    Set FileSystem = CreateFileSystemObject()
    EnsureFolderCore FileSystem, FolderPath

    Set InfEnsureFolder = ComCreateSuccess("Folder is available.")
End Function

' Reads all text from FilePath.
'
' Parameters:
'   FilePath: Path to the text file.
'
' Return value:
'   Complete file content.
'
' Raised errors:
'   ComErrInvalidArgument when FilePath is blank.
Public Function InfReadText(ByVal FilePath As String) As String
    Dim FileSystem As Object
    Dim TextFile As Object

    ComRequireText FilePath, "FilePath", "InfReadText"

    Set FileSystem = CreateFileSystemObject()
    If Not FileSystem.FileExists(FilePath) Then
        Err.Raise ComErrInvalidArgument, "InfReadText", "FilePath does not exist."
    End If

    Set TextFile = FileSystem.OpenTextFile(FilePath, InfOpenForReading, InfDoNotCreateMissingFile)
    InfReadText = TextFile.ReadAll
    TextFile.Close
End Function

' Writes TextContent to FilePath, replacing existing content.
'
' Parameters:
'   FilePath: Path to the text file.
'   TextContent: Text content to write.
'
' Return value:
'   Successful ComResult when content was written.
'
' Raised errors:
'   ComErrInvalidArgument when FilePath is blank.
Public Function InfWriteText(ByVal FilePath As String, ByVal TextContent As String) As ComResult
    Dim FileSystem As Object
    Dim TextFile As Object

    ComRequireText FilePath, "FilePath", "InfWriteText"

    Set FileSystem = CreateFileSystemObject()
    EnsureFolderCore FileSystem, FileSystem.GetParentFolderName(FilePath)

    Set TextFile = FileSystem.CreateTextFile(FilePath, True, InfCreateAsAscii)
    TextFile.Write TextContent
    TextFile.Close

    Set InfWriteText = ComCreateSuccess("Text content was written.")
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateFileSystemObject() As Object
    Set CreateFileSystemObject = CreateObject(InfFileSystemObjectProgId)
End Function

Private Sub EnsureFolderCore(ByVal FileSystem As Object, ByVal FolderPath As String)
    Dim ParentFolderPath As String

    If ComIsBlankText(FolderPath) Then
        Exit Sub
    End If

    If FileSystem.FolderExists(FolderPath) Then
        Exit Sub
    End If

    ParentFolderPath = FileSystem.GetParentFolderName(FolderPath)

    If Not ComIsBlankText(ParentFolderPath) Then
        EnsureFolderCore FileSystem, ParentFolderPath
    End If

    If Not FileSystem.FolderExists(FolderPath) Then
        FileSystem.CreateFolder FolderPath
    End If
End Sub
