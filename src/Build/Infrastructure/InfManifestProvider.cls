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

' Validates that a manifest file can be loaded as manifest-driven generation items.
Public Function InfValidateManifestFile(ByVal ManifestPath As String) As ComResult
    Dim Items As Collection

    On Error GoTo ErrHandler

    Set Items = InfLoadManifestItems(ManifestPath)
    ValidateSourcePaths Items
    ValidateSectionContracts Items

    If Items.Count = 0 Then
        Err.Raise ComErrInvalidState, "InfValidateManifestFile", "Manifest contains no generation items."
    End If

    Set InfValidateManifestFile = ComCreateSuccess("Manifest is valid.")
    Exit Function

ErrHandler:
    Set InfValidateManifestFile = ComCreateFailureFromErr("InfManifestProvider.InfValidateManifestFile", Err.Description, "Error", False)
End Function

' Loads optional body source content for a manifest item.
Public Function InfLoadBodySource(ByVal BodySourcePath As String) As String
    ComRequireText BodySourcePath, "BodySourcePath", "InfLoadBodySource"

    If FileSystemProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfLoadBodySource", "ManifestProvider is not initialized."
    End If

    If Not FileSystemProvider.InfFileExists(BodySourcePath) Then
        Err.Raise ComErrInvalidArgument, "InfLoadBodySource", "BodySourcePath does not exist."
    End If

    InfLoadBodySource = FileSystemProvider.InfReadText(BodySourcePath)
End Function

' Loads optional section source content for a manifest item.
Public Function InfLoadSectionSource(ByVal SectionSourcePath As String) As String
    ComRequireText SectionSourcePath, "SectionSourcePath", "InfLoadSectionSource"

    If FileSystemProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "InfLoadSectionSource", "ManifestProvider is not initialized."
    End If

    If Not FileSystemProvider.InfFileExists(SectionSourcePath) Then
        Err.Raise ComErrInvalidArgument, "InfLoadSectionSource", "SectionSourcePath does not exist."
    End If

    InfLoadSectionSource = FileSystemProvider.InfReadText(SectionSourcePath)
End Function

Private Sub ValidateLayerName(ByVal LayerName As String)
    Dim Position As Long
    Dim Character As String

    If ComIsBlankText(LayerName) Then
        Err.Raise ComErrInvalidArgument, "InfCreateManifest", "LayerName is required."
    End If

    Character = Mid$(LayerName, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        Err.Raise ComErrInvalidArgument, "InfCreateManifest", "LayerName must start with a letter."
    End If

    For Position = 1 To Len(LayerName)
        Character = Mid$(LayerName, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            Err.Raise ComErrInvalidArgument, "InfCreateManifest", "LayerName must contain only letters, numbers, or underscore."
        End If
    Next Position
End Sub

Private Function CreateManifestItemFromLine(ByVal LineText As String, ByVal ManifestPath As String) As ManifestItem
    Dim Fields() As String
    Dim Item As ManifestItem
    Dim BodySourcePath As String
    Dim FieldIndex As Long

    Fields = Split(LineText, ",")
    If UBound(Fields) < 3 Then
        Err.Raise ComErrInvalidArgument, "InfLoadManifestItems", "Manifest line must contain ModuleName, ModuleType, LayerName, TemplatePath, optional BodySourcePath and optional SectionSource entries."
    End If

    BodySourcePath = vbNullString
    If UBound(Fields) >= 4 Then
        If Not ComIsBlankText(Trim$(Fields(4))) Then
            BodySourcePath = ResolveManifestRelativePath(Trim$(Fields(4)), ManifestPath)
        End If
    End If

    Set Item = New ManifestItem
    Item.InfInitialize _
        Trim$(Fields(0)), _
        Trim$(Fields(1)), _
        Trim$(Fields(2)), _
        ResolveManifestRelativePath(Trim$(Fields(3)), ManifestPath), _
        BodySourcePath

    For FieldIndex = 5 To UBound(Fields)
        AddSectionSourceFromField Item, Trim$(Fields(FieldIndex)), ManifestPath
    Next FieldIndex

    Set CreateManifestItemFromLine = Item
End Function

Private Sub AddSectionSourceFromField( _
    ByVal Item As ManifestItem, _
    ByVal FieldText As String, _
    ByVal ManifestPath As String)

    Dim SeparatorIndex As Long
    Dim SectionName As String
    Dim SectionSourcePath As String

    If ComIsBlankText(FieldText) Then
        Exit Sub
    End If

    SeparatorIndex = InStr(1, FieldText, "=", vbBinaryCompare)
    If SeparatorIndex <= 1 Or SeparatorIndex = Len(FieldText) Then
        Err.Raise ComErrInvalidArgument, "InfLoadManifestItems", "SectionSource entry must use SectionName=SectionSourcePath."
    End If

    SectionName = Trim$(Left$(FieldText, SeparatorIndex - 1))
    SectionSourcePath = Trim$(Mid$(FieldText, SeparatorIndex + 1))
    Item.InfAddSectionSourcePath SectionName, ResolveManifestRelativePath(SectionSourcePath, ManifestPath)
End Sub

Private Sub ValidateSourcePaths(ByVal Items As Collection)
    Dim Item As ManifestItem
    Dim BodySourcePath As String
    Dim SectionSourcePaths As Object
    Dim SectionName As Variant

    If Items Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ValidateSourcePaths", "Manifest items are required."
    End If

    For Each Item In Items
        BodySourcePath = Item.InfGetBodySourcePath()
        If Not ComIsBlankText(BodySourcePath) Then
            If Not FileSystemProvider.InfFileExists(BodySourcePath) Then
                Err.Raise ComErrInvalidArgument, "ValidateSourcePaths", "BodySourcePath does not exist: " & BodySourcePath
            End If
        End If

        Set SectionSourcePaths = Item.InfGetSectionSourcePaths()
        For Each SectionName In SectionSourcePaths.Keys
            If Not FileSystemProvider.InfFileExists(CStr(SectionSourcePaths.Item(CStr(SectionName)))) Then
                Err.Raise ComErrInvalidArgument, "ValidateSourcePaths", "SectionSourcePath does not exist: " & CStr(SectionSourcePaths.Item(CStr(SectionName)))
            End If
        Next SectionName
    Next Item
End Sub

Private Sub ValidateSectionContracts(ByVal Items As Collection)
    Dim Item As ManifestItem
    Dim SectionSourcePaths As Object
    Dim TemplateSections As Object
    Dim SectionName As Variant

    If Items Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ValidateSectionContracts", "Manifest items are required."
    End If

    For Each Item In Items
        Set SectionSourcePaths = Item.InfGetSectionSourcePaths()
        If SectionSourcePaths.Count > 0 Then
            If Not FileSystemProvider.InfFileExists(Item.InfGetTemplatePath()) Then
                Err.Raise ComErrInvalidArgument, "ValidateSectionContracts", "TemplatePath does not exist: " & Item.InfGetTemplatePath()
            End If

            Set TemplateSections = ExtractTemplateSections(FileSystemProvider.InfReadText(Item.InfGetTemplatePath()))
            For Each SectionName In SectionSourcePaths.Keys
                If Not TemplateSections.Exists(CStr(SectionName)) Then
                    Err.Raise ComErrInvalidArgument, "ValidateSectionContracts", "Template section does not exist: " & CStr(SectionName)
                End If
            Next SectionName
        End If
    Next Item
End Sub

Private Function ExtractTemplateSections(ByVal TemplateText As String) As Object
    Dim Sections As Object
    Dim NormalizedText As String
    Dim SectionStart As Long
    Dim SectionLineStart As Long
    Dim SectionLineEnd As Long
    Dim SectionEnd As Long
    Dim SectionLine As String
    Dim SectionName As String

    Set Sections = CreateObject("Scripting.Dictionary")
    NormalizedText = NormalizeLineBreaks(TemplateText)

    SectionStart = InStr(1, NormalizedText, "@section", vbTextCompare)
    Do While SectionStart > 0
        SectionLineStart = InStrRev(NormalizedText, vbLf, SectionStart)
        If SectionLineStart = 0 Then
            SectionLineStart = 1
        Else
            SectionLineStart = SectionLineStart + 1
        End If

        SectionLineEnd = InStr(SectionStart, NormalizedText, vbLf, vbBinaryCompare)
        If SectionLineEnd = 0 Then
            SectionLineEnd = Len(NormalizedText) + 1
        End If

        SectionEnd = InStr(SectionLineEnd, NormalizedText, "@endsection", vbTextCompare)
        If SectionEnd = 0 Then
            Err.Raise ComErrInvalidArgument, "ExtractTemplateSections", "Template section is missing @endsection."
        End If

        SectionLine = Mid$(NormalizedText, SectionLineStart, SectionLineEnd - SectionLineStart)
        SectionName = ParseTemplateSectionName(SectionLine)
        If Not Sections.Exists(SectionName) Then
            Sections.Add SectionName, True
        End If

        SectionStart = InStr(SectionEnd + Len("@endsection"), NormalizedText, "@section", vbTextCompare)
    Loop

    Set ExtractTemplateSections = Sections
End Function

Private Function ParseTemplateSectionName(ByVal SectionLine As String) As String
    Dim MarkerIndex As Long
    Dim SectionName As String

    MarkerIndex = InStr(1, SectionLine, "@section", vbTextCompare)
    If MarkerIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "ParseTemplateSectionName", "Template section marker is invalid."
    End If

    SectionName = Mid$(SectionLine, MarkerIndex + Len("@section"))
    SectionName = Replace(SectionName, vbCr, vbNullString)
    SectionName = Replace(SectionName, vbLf, vbNullString)
    SectionName = Trim$(SectionName)
    ComRequireText SectionName, "SectionName", "ParseTemplateSectionName"

    ParseTemplateSectionName = SectionName
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
    Dim ManifestDirectoryPath As String
    Dim NormalizedFilePath As String
    Dim FileSystem As Object

    If IsAbsolutePath(FilePath) Then
        ResolveManifestRelativePath = FilePath
        Exit Function
    End If

    ManifestDirectoryPath = GetParentFolderPath(ManifestPath)
    NormalizedFilePath = Replace(FilePath, "/", "\")

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ResolveManifestRelativePath = FileSystem.GetAbsolutePathName( _
        BuildPathResolver.CombinePath(ManifestDirectoryPath, NormalizedFilePath))
End Function

Private Function IsAbsolutePath(ByVal FilePath As String) As Boolean
    IsAbsolutePath = ((Len(FilePath) >= 3 _
            And (Mid$(FilePath, 2, 2) = ":\" Or Mid$(FilePath, 2, 2) = ":/")) _
        Or Left$(FilePath, 2) = "\\" _
        Or Left$(FilePath, 2) = "//")
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
