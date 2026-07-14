VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppManifestEditorService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppManifestEditorService
' Layer: Application
' Responsibility: Load, validate, and save editable Build manifest models.
' Dependencies: Common, Infrastructure
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

Public Function AppLoadManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByRef Modules As Collection) As ComResult

    On Error GoTo ErrHandler

    ComRequireText ManifestPath, "ManifestPath", "AppLoadManifestEditorModel"

    Set Modules = LoadManifestModules(ManifestPath)
    Set AppLoadManifestEditorModel = ComCreateSuccess("Manifest loaded.")
    Exit Function

ErrHandler:
    Set Modules = Nothing
    Set AppLoadManifestEditorModel = ComCreateFailureFromErr( _
        "AppManifestEditorService.AppLoadManifestEditorModel", _
        Err.Description, _
        "Error", _
        False)
End Function

Public Function AppSaveManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection) As ComResult

    Dim Result As ComResult

    On Error GoTo ErrHandler

    ComRequireText ManifestPath, "ManifestPath", "AppSaveManifestEditorModel"
    Set Result = ValidateBeforeSave(ManifestPath, Modules)
    If Result.IsFailure Then
        Set AppSaveManifestEditorModel = Result
        Exit Function
    End If

    SaveMemberSources ManifestPath, Modules
    WriteManifestContent ManifestPath, Modules

    Set AppSaveManifestEditorModel = ComCreateSuccess("Manifest saved.")
    Exit Function

ErrHandler:
    Set AppSaveManifestEditorModel = ComCreateFailureFromErr( _
        "AppManifestEditorService.AppSaveManifestEditorModel", _
        Err.Description, _
        "Error", _
        False)
End Function

Public Function AppCreateManifestEditorModule( _
    ByVal ModuleName As String, _
    ByVal LayerName As String, _
    ByVal ModuleType As String, _
    Optional ByVal TemplatePath As String = vbNullString) As Object

    Dim ModuleInfo As Object

    Set ModuleInfo = CreateObject("Scripting.Dictionary")
    ModuleInfo("ModuleName") = Trim$(ModuleName)
    ModuleInfo("Layer") = Trim$(LayerName)
    ModuleInfo("ModuleType") = Trim$(ModuleType)
    ModuleInfo("TemplatePath") = Trim$(TemplatePath)
    ModuleInfo("BodySourcePath") = vbNullString
    ModuleInfo("MemberSourcePath") = vbNullString
    Set ModuleInfo("SectionFields") = New Collection
    Set ModuleInfo("Members") = New Collection

    Set AppCreateManifestEditorModule = ModuleInfo
End Function

Public Function AppCreateManifestEditorMember( _
    ByVal MemberName As String, _
    ByVal TypeName As String, _
    ByVal Accessor As String, _
    ByVal InitialValue As String, _
    ByVal CreateInstance As Boolean) As Object

    Dim MemberInfo As Object

    Set MemberInfo = CreateObject("Scripting.Dictionary")
    MemberInfo("Name") = Trim$(MemberName)
    MemberInfo("TypeName") = Trim$(TypeName)
    MemberInfo("Accessor") = Trim$(Accessor)
    MemberInfo("InitialValue") = Trim$(InitialValue)
    MemberInfo("CreateInstance") = IIf(CreateInstance, "True", "False")
    MemberInfo("IsObject") = IIf(CreateInstance Or StrComp(Trim$(Accessor), "GetSet", vbTextCompare) = 0, "True", "False")
    MemberInfo("ReadOnly") = "False"

    Set AppCreateManifestEditorMember = MemberInfo
End Function

' Renders editable member definitions using the same format saved by Manifest Editor.
Public Function AppRenderManifestEditorMemberDefinitions(ByVal Members As Collection) As String
    SetRequiredMembers Members
    AppRenderManifestEditorMemberDefinitions = RenderMemberDefinitions(Members)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function LoadManifestModules(ByVal ManifestPath As String) As Collection
    Dim FileProvider As InfFileSystemProvider
    Dim Content As String
    Dim Lines() As String
    Dim LineIndex As Long
    Dim LineText As String
    Dim Modules As Collection

    Set FileProvider = New InfFileSystemProvider
    If Not FileProvider.InfFileExists(ManifestPath) Then
        Err.Raise ComErrInvalidArgument, "LoadManifestModules", "ManifestPath does not exist."
    End If

    Content = NormalizeLineBreaks(FileProvider.InfReadText(ManifestPath))
    Lines = Split(Content, vbLf)
    Set Modules = New Collection

    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(Lines(LineIndex))
        If IsManifestDataLine(LineText) Then
            Modules.Add ParseManifestLine(LineText, ManifestPath)
        End If
    Next LineIndex

    If Modules.Count = 0 Then
        Err.Raise ComErrInvalidState, "LoadManifestModules", "Manifest contains no modules."
    End If

    Set LoadManifestModules = Modules
End Function

Private Function ValidateBeforeSave(ByVal ManifestPath As String, ByVal Modules As Collection) As ComResult
    Dim ValidationService As AppManifestValidationService
    Dim Issues As Collection

    Set ValidationService = New AppManifestValidationService
    Set ValidateBeforeSave = ValidationService.AppValidateManifestEditorModel(ManifestPath, Modules, Issues)
    If ValidationService.AppIssuesContainErrors(Issues) Then
        Set ValidateBeforeSave = ValidationService.AppCreateFailureFromIssues(Issues, "AppManifestEditorService.AppSaveManifestEditorModel")
    End If
End Function

Private Function ParseManifestLine(ByVal LineText As String, ByVal ManifestPath As String) As Object
    Dim Fields() As String
    Dim ModuleInfo As Object
    Dim FieldIndex As Long
    Dim FieldText As String
    Dim SeparatorIndex As Long
    Dim SectionName As String
    Dim SectionSourcePath As String

    Fields = Split(LineText, ",")
    If UBound(Fields) < 3 Then
        Err.Raise ComErrInvalidArgument, "ParseManifestLine", "Manifest line must contain ModuleName, ModuleType, LayerName, and TemplatePath."
    End If

    Set ModuleInfo = AppCreateManifestEditorModule(Fields(0), Fields(2), Fields(1), Fields(3))

    If UBound(Fields) >= 4 Then
        ModuleInfo("BodySourcePath") = Trim$(Fields(4))
    End If

    For FieldIndex = 5 To UBound(Fields)
        FieldText = Trim$(Fields(FieldIndex))
        If Not ComIsBlankText(FieldText) Then
            SeparatorIndex = InStr(1, FieldText, "=", vbBinaryCompare)
            If SeparatorIndex <= 1 Or SeparatorIndex = Len(FieldText) Then
                Err.Raise ComErrInvalidArgument, "ParseManifestLine", "SectionSource entry must use SectionName=SectionSourcePath."
            End If

            SectionName = Trim$(Left$(FieldText, SeparatorIndex - 1))
            SectionSourcePath = Trim$(Mid$(FieldText, SeparatorIndex + 1))
            If StrComp(SectionName, "Members", vbTextCompare) = 0 _
                Or StrComp(SectionName, "MemberSource", vbTextCompare) = 0 Then
                ModuleInfo("MemberSourcePath") = SectionSourcePath
                Set ModuleInfo("Members") = LoadMembers(ResolveManifestRelativePath(SectionSourcePath, ManifestPath))
            Else
                ModuleInfo("SectionFields").Add FieldText
            End If
        End If
    Next FieldIndex

    Set ParseManifestLine = ModuleInfo
End Function

Private Function LoadMembers(ByVal MemberSourcePath As String) As Collection
    Dim FileProvider As InfFileSystemProvider
    Dim Members As Collection
    Dim Content As String
    Dim Lines() As String
    Dim LineIndex As Long
    Dim LineText As String

    Set Members = New Collection
    Set FileProvider = New InfFileSystemProvider

    If ComIsBlankText(MemberSourcePath) Then
        Set LoadMembers = Members
        Exit Function
    End If

    If Not FileProvider.InfFileExists(MemberSourcePath) Then
        Set LoadMembers = Members
        Exit Function
    End If

    Content = NormalizeLineBreaks(FileProvider.InfReadText(MemberSourcePath))
    Lines = Split(Content, vbLf)
    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(StripInlineComment(Lines(LineIndex)))
        If Not ComIsBlankText(LineText) Then
            Members.Add ParseMemberLine(LineText)
        End If
    Next LineIndex

    Set LoadMembers = Members
End Function

Private Function ParseMemberLine(ByVal LineText As String) As Object
    Dim MemberInfo As Object
    Dim Parts() As String
    Dim Part As Variant
    Dim SeparatorIndex As Long
    Dim KeyName As String
    Dim KeyValue As String

    Set MemberInfo = CreateObject("Scripting.Dictionary")
    MemberInfo("Name") = vbNullString
    MemberInfo("TypeName") = vbNullString
    MemberInfo("Accessor") = vbNullString
    MemberInfo("InitialValue") = vbNullString
    MemberInfo("CreateInstance") = "False"
    MemberInfo("IsObject") = "False"
    MemberInfo("ReadOnly") = "False"

    Parts = Split(LineText, ";")
    For Each Part In Parts
        SeparatorIndex = InStr(1, CStr(Part), "=", vbBinaryCompare)
        If SeparatorIndex <= 1 Then
            Err.Raise ComErrInvalidArgument, "ParseMemberLine", "Member definition entry must use Key=Value syntax."
        End If

        KeyName = Trim$(Left$(CStr(Part), SeparatorIndex - 1))
        KeyValue = Trim$(Mid$(CStr(Part), SeparatorIndex + 1))
        Select Case KeyName
            Case "Name", "TypeName", "Accessor", "InitialValue", "CreateInstance", "IsObject", "ReadOnly"
                MemberInfo(KeyName) = KeyValue
            Case Else
                Err.Raise ComErrInvalidArgument, "ParseMemberLine", "Unsupported member key: " & KeyName
        End Select
    Next Part

    Set ParseMemberLine = MemberInfo
End Function

Private Sub SetRequiredMembers(ByVal Members As Collection)
    If Members Is Nothing Then
        Err.Raise ComErrInvalidArgument, "SetRequiredMembers", "Members are required."
    End If
End Sub

Private Sub SaveMemberSources(ByVal ManifestPath As String, ByVal Modules As Collection)
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberSourcePath As String
    Dim AbsoluteMemberSourcePath As String
    Dim FileProvider As InfFileSystemProvider

    Set FileProvider = New InfFileSystemProvider
    For Each ModuleInfo In Modules
        Set Members = ModuleInfo("Members")
        If Members.Count > 0 Or Not ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) Then
            If ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) Then
                ModuleInfo("MemberSourcePath") = BuildDefaultMemberSourcePath(CStr(ModuleInfo("ModuleName")))
            End If

            MemberSourcePath = CStr(ModuleInfo("MemberSourcePath"))
            AbsoluteMemberSourcePath = ResolveManifestRelativePath(MemberSourcePath, ManifestPath)
            FileProvider.InfWriteText AbsoluteMemberSourcePath, RenderMemberDefinitions(Members)
        End If
    Next ModuleInfo
End Sub

Private Sub WriteManifestContent(ByVal ManifestPath As String, ByVal Modules As Collection)
    Dim FileProvider As InfFileSystemProvider

    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText ManifestPath, RenderManifestContent(Modules)
End Sub

Private Function RenderManifestContent(ByVal Modules As Collection) As String
    Dim Result As String
    Dim ModuleInfo As Object

    Result = "# ModuleName,ModuleType,LayerName,TemplatePath"
    For Each ModuleInfo In Modules
        Result = Result & vbCrLf & RenderManifestModuleLine(ModuleInfo)
    Next ModuleInfo

    RenderManifestContent = Result & vbCrLf
End Function

Private Function RenderManifestModuleLine(ByVal ModuleInfo As Object) As String
    Dim Fields As Collection
    Dim SectionFields As Collection
    Dim FieldText As Variant

    Set Fields = New Collection
    Fields.Add CStr(ModuleInfo("ModuleName"))
    Fields.Add CStr(ModuleInfo("ModuleType"))
    Fields.Add CStr(ModuleInfo("Layer"))
    Fields.Add CStr(ModuleInfo("TemplatePath"))

    Set SectionFields = ModuleInfo("SectionFields")
    If Not ComIsBlankText(CStr(ModuleInfo("BodySourcePath"))) _
        Or Not ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) _
        Or SectionFields.Count > 0 Then
        Fields.Add CStr(ModuleInfo("BodySourcePath"))
    End If

    For Each FieldText In SectionFields
        Fields.Add CStr(FieldText)
    Next FieldText

    If Not ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) Then
        Fields.Add "Members=" & CStr(ModuleInfo("MemberSourcePath"))
    End If

    RenderManifestModuleLine = JoinCollection(Fields, ",")
End Function

Private Function RenderMemberDefinitions(ByVal Members As Collection) As String
    Dim Result As String
    Dim MemberInfo As Object

    For Each MemberInfo In Members
        If Len(Result) > 0 Then
            Result = Result & vbCrLf
        End If

        Result = Result & RenderMemberLine(MemberInfo)
    Next MemberInfo

    If Len(Result) > 0 Then
        Result = Result & vbCrLf
    End If

    RenderMemberDefinitions = Result
End Function

Private Function RenderMemberLine(ByVal MemberInfo As Object) As String
    RenderMemberLine = _
        "Name=" & CStr(MemberInfo("Name")) & _
        ";TypeName=" & CStr(MemberInfo("TypeName")) & _
        ";IsObject=" & CStr(MemberInfo("IsObject")) & _
        ";Accessor=" & CStr(MemberInfo("Accessor")) & _
        ";InitialValue=" & CStr(MemberInfo("InitialValue")) & _
        ";CreateInstance=" & CStr(MemberInfo("CreateInstance")) & _
        ";ReadOnly=" & CStr(MemberInfo("ReadOnly"))
End Function

Private Function JoinCollection(ByVal Values As Collection, ByVal Separator As String) As String
    Dim Value As Variant
    Dim Result As String

    For Each Value In Values
        If Len(Result) > 0 Then
            Result = Result & Separator
        End If

        Result = Result & CStr(Value)
    Next Value

    JoinCollection = Result
End Function

Private Function BuildDefaultMemberSourcePath(ByVal ModuleName As String) As String
    BuildDefaultMemberSourcePath = "MemberSource\" & ModuleName & ".members"
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

Private Function StripInlineComment(ByVal TextValue As String) As String
    Dim CommentIndex As Long

    CommentIndex = InStr(1, TextValue, "#", vbBinaryCompare)
    If CommentIndex > 0 Then
        StripInlineComment = Left$(TextValue, CommentIndex - 1)
    Else
        StripInlineComment = TextValue
    End If
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
