VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfManifestProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfManifestProvider
' Layer: Infrastructure
' Responsibility: Build and load manifest values for generation requests.
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

' Creates a manifest dictionary for the supplied generation inputs.
Public Function InfCreateManifest( _
    ByVal ModuleName As String, _
    Optional ByVal TemplatePath As String = vbNullString, _
    Optional ByVal LayerName As String = "Application") As Object

    Dim Manifest As Object

    ComRequireText ModuleName, "ModuleName", "InfCreateManifest"
    ComRequireText LayerName, "LayerName", "InfCreateManifest"
    ValidateLayerName LayerName

    Set Manifest = CreateObject("Scripting.Dictionary")
    Manifest("ModuleName") = ModuleName
    Manifest("Layer") = LayerName
    If Not ComIsBlankText(TemplatePath) Then
        Manifest("TemplatePath") = TemplatePath
    End If

    Set InfCreateManifest = Manifest
End Function

' Loads manifest-driven generation items from a manifest file.
Public Function InfLoadManifestItems(ByVal ManifestPath As String) As Collection
    Dim Content As String
    Dim Lines() As String
    Dim LineText As String
    Dim Items As Collection
    Dim LineIndex As Long

    ComRequireText ManifestPath, "ManifestPath", "InfLoadManifestItems"

    If FileSystemProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfLoadManifestItems", "ManifestProvider is not initialized."
    End If

    If Not FileSystemProvider.InfFileExists(ManifestPath) Then
        Err.Raise ComErrInvalidArgument, "InfLoadManifestItems", "ManifestPath does not exist."
    End If

    Content = NormalizeLineBreaks(FileSystemProvider.InfReadText(ManifestPath))
    Lines = Split(Content, vbLf)

    Set Items = New Collection
    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(Lines(LineIndex))
        If IsManifestDataLine(LineText) Then
            Items.Add CreateManifestItemFromLine(LineText, ManifestPath)
        End If
    Next LineIndex

    If Items.Count = 0 Then
        Err.Raise ComErrInvalidState, "InfLoadManifestItems", "Manifest contains no generation items."
    End If

    Set InfLoadManifestItems = Items
End Function

Private Sub ValidateLayerName(ByVal LayerName As String)
    Select Case LayerName
        Case "Presentation", "Application", "Domain", "Infrastructure", "Common"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "InfCreateManifest", "LayerName is not defined by LayerSpecification."
End Sub

Private Function CreateManifestItemFromLine(ByVal LineText As String, ByVal ManifestPath As String) As ManifestItem
    Dim Fields() As String
    Dim Item As ManifestItem

    Fields = Split(LineText, ",")
    If UBound(Fields) <> 3 Then
        Err.Raise ComErrInvalidArgument, "InfLoadManifestItems", "Manifest line must contain ModuleName, ModuleType, LayerName and TemplatePath."
    End If

    Set Item = New ManifestItem
    Item.InfInitialize _
        Trim$(Fields(0)), _
        Trim$(Fields(1)), _
        Trim$(Fields(2)), _
        ResolveManifestRelativePath(Trim$(Fields(3)), ManifestPath)

    Set CreateManifestItemFromLine = Item
End Function

Private Function IsManifestDataLine(ByVal LineText As String) As Boolean
    If ComIsBlankText(LineText) Then
        IsManifestDataLine = False
        Exit Function
    End If

    If Left$(LineText, 1) = "#" Then
        IsManifestDataLine = False
        Exit Function
    End If

    IsManifestDataLine = True
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    NormalizeLineBreaks = Replace(TextValue, vbCrLf, vbLf)
    NormalizeLineBreaks = Replace(NormalizeLineBreaks, vbCr, vbLf)
End Function

Private Function ResolveManifestRelativePath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    If IsAbsolutePath(FilePath) Then
        ResolveManifestRelativePath = FilePath
        Exit Function
    End If

    ResolveManifestRelativePath = GetParentFolderPath(ManifestPath) & Application.PathSeparator & FilePath
End Function

Private Function IsAbsolutePath(ByVal FilePath As String) As Boolean
    IsAbsolutePath = (InStr(1, FilePath, ":\", vbTextCompare) > 0 Or Left$(FilePath, 2) = "\\")
End Function

Private Function GetParentFolderPath(ByVal FilePath As String) As String
    Dim SeparatorIndex As Long

    SeparatorIndex = InStrRev(FilePath, "\")
    If SeparatorIndex = 0 Then
        SeparatorIndex = InStrRev(FilePath, "/")
    End If

    If SeparatorIndex = 0 Then
        GetParentFolderPath = "."
    Else
        GetParentFolderPath = Left$(FilePath, SeparatorIndex - 1)
    End If
End Function
