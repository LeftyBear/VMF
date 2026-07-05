Option Explicit
Attribute VB_Name = "InfFacade"

'=========================================================================
' Module: InfFacade
' Layer: Infrastructure
' Responsibility: Published entry point for Infrastructure capabilities.
' Dependencies: Common, InfCompositionRoot, InfFileSystemProvider,
'               InfWorkbookProvider
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Infrastructure services.
'
' Parameters:
'   None.
'
' Return value:
'   True when Infrastructure initialization completes.
'
' Raised errors:
'   Propagates Infrastructure initialization errors.
Public Function InfInitialize() As Boolean
    Dim CompositionRoot As InfCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    InfInitialize = CompositionRoot.InfInitialize()
End Function

' Shuts down Infrastructure services.
'
' Parameters:
'   None.
'
' Raised errors:
'   Propagates Infrastructure shutdown errors.
Public Sub InfShutdown()
    Dim CompositionRoot As InfCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    CompositionRoot.InfShutdown
End Sub

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
    Dim FileSystemProvider As InfFileSystemProvider

    Set FileSystemProvider = CreateCompositionRoot().InfCreateFileSystemProvider()
    InfFileExists = FileSystemProvider.InfFileExists(FilePath)
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
    Dim FileSystemProvider As InfFileSystemProvider

    Set FileSystemProvider = CreateCompositionRoot().InfCreateFileSystemProvider()
    InfFolderExists = FileSystemProvider.InfFolderExists(FolderPath)
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
    Dim FileSystemProvider As InfFileSystemProvider

    Set FileSystemProvider = CreateCompositionRoot().InfCreateFileSystemProvider()
    Set InfEnsureFolder = FileSystemProvider.InfEnsureFolder(FolderPath)
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
    Dim FileSystemProvider As InfFileSystemProvider

    Set FileSystemProvider = CreateCompositionRoot().InfCreateFileSystemProvider()
    InfReadText = FileSystemProvider.InfReadText(FilePath)
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
    Dim FileSystemProvider As InfFileSystemProvider

    Set FileSystemProvider = CreateCompositionRoot().InfCreateFileSystemProvider()
    Set InfWriteText = FileSystemProvider.InfWriteText(FilePath, TextContent)
End Function

' Returns the full path of the supplied workbook-like object.
'
' Parameters:
'   Workbook: Object exposing a FullName property.
'
' Return value:
'   Workbook full path.
'
' Raised errors:
'   ComErrInvalidArgument when Workbook is Nothing or FullName is blank.
Public Function InfGetWorkbookPath(ByVal Workbook As Object) As String
    Dim WorkbookProvider As InfWorkbookProvider

    Set WorkbookProvider = CreateCompositionRoot().InfCreateWorkbookProvider()
    InfGetWorkbookPath = WorkbookProvider.InfGetWorkbookPath(Workbook)
End Function

' Creates a template-driven generator for module generation.
Public Function InfCreateGenerator() As InfGenerator
    Set InfCreateGenerator = CreateCompositionRoot().InfCreateGenerator()
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateCompositionRoot() As InfCompositionRoot
    Set CreateCompositionRoot = New InfCompositionRoot
End Function
