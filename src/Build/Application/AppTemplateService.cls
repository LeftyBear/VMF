VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateService
' Layer: Application
' Responsibility: Load, analyze, validate, save, and preview VMF templates.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const SeverityError As Long = 1
Private Const SeverityWarning As Long = 2
Private Const SeverityInformation As Long = 3

Private Generator As InfGenerator
Private ManifestEditorService As AppManifestEditorService
Private FileProvider As InfFileSystemProvider
Private SettingsService As AppStudioSettingsService

Public Sub AppInitialize( _
    ByVal ModuleGenerator As InfGenerator, _
    ByVal EditorService As AppManifestEditorService, _
    ByVal ProviderFileSystem As InfFileSystemProvider, _
    Optional ByVal ProviderSettings As AppStudioSettingsService = Nothing)

    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppTemplateService.AppInitialize", "ModuleGenerator is required."
    End If
    If EditorService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppTemplateService.AppInitialize", "ManifestEditorService is required."
    End If
    If ProviderFileSystem Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppTemplateService.AppInitialize", "FileSystemProvider is required."
    End If

    Set Generator = ModuleGenerator
    Set ManifestEditorService = EditorService
    Set FileProvider = ProviderFileSystem
    Set SettingsService = ProviderSettings
End Sub

Public Function AppLoadTemplates(ByVal ManifestPath As String, ByVal Modules As Collection, ByRef Templates As Collection) As ComResult
    Dim TemplatePaths As Object
    Dim ModuleInfo As Object
    Dim FileName As String
    Dim TemplatePath As Variant
    Dim Template As AppTemplateModel

    On Error GoTo ErrHandler

    Set Templates = New Collection
    Set TemplatePaths = CreateObject("Scripting.Dictionary")

    FileName = Dir$(BuildPathResolver.CombinePath(ResolveTemplateDirectory(), "*.txt"))
    Do While Len(FileName) > 0
        TemplatePaths(LCase$(BuildPathResolver.CombinePath(ResolveTemplateDirectory(), FileName))) = _
            BuildPathResolver.CombinePath(ResolveTemplateDirectory(), FileName)
        FileName = Dir$()
    Loop

    If Not Modules Is Nothing Then
        For Each ModuleInfo In Modules
            If HasKey(ModuleInfo, "TemplatePath") Then
                TemplatePaths(LCase$(ResolveManifestRelativePath(CStr(ModuleInfo("TemplatePath")), ManifestPath))) = _
                    ResolveManifestRelativePath(CStr(ModuleInfo("TemplatePath")), ManifestPath)
            End If
        Next ModuleInfo
    End If

    For Each TemplatePath In TemplatePaths.Items
        Set Template = LoadTemplateModel(CStr(TemplatePath))
        AnalyzeTemplate Template
        Templates.Add Template
    Next TemplatePath

    Set AppLoadTemplates = ComCreateSuccess("Templates loaded.")
    Exit Function

ErrHandler:
    Set Templates = New Collection
    Set AppLoadTemplates = ComCreateFailureFromErr("AppTemplateService.AppLoadTemplates", Err.Description, "Error", False)
End Function

Public Function AppLoadTemplateFile(ByVal FilePath As String, ByRef Template As AppTemplateModel) As ComResult
    On Error GoTo ErrHandler
    Set Template = LoadTemplateModel(FilePath)
    AnalyzeTemplate Template
    Set AppLoadTemplateFile = ComCreateSuccess("Template loaded.")
    Exit Function

ErrHandler:
    Set Template = Nothing
    Set AppLoadTemplateFile = ComCreateFailureFromErr("AppTemplateService.AppLoadTemplateFile", Err.Description, "Error", False)
End Function

Public Function AppCreateTemplateModel( _
    ByVal TemplateName As String, _
    ByVal TemplateType As String, _
    ByVal FilePath As String, _
    ByVal Content As String) As AppTemplateModel

    Dim Template As AppTemplateModel
    Set Template = New AppTemplateModel
    Template.AppInitialize TemplateName, TemplateType, FilePath, Content
    Template.IsDirty = True
    AnalyzeTemplate Template
    Set AppCreateTemplateModel = Template
End Function

Public Function AppAnalyzeTemplate(ByVal Template As AppTemplateModel) As ComResult
    On Error GoTo ErrHandler
    AnalyzeTemplate Template
    Set AppAnalyzeTemplate = ComCreateSuccess("Template analyzed.")
    Exit Function

ErrHandler:
    Set AppAnalyzeTemplate = ComCreateFailureFromErr("AppTemplateService.AppAnalyzeTemplate", Err.Description, "Error", False)
End Function

Public Function AppValidateTemplate(ByVal Template As AppTemplateModel, ByRef Issues As Collection) As ComResult
    On Error GoTo ErrHandler
    Set Issues = New Collection
    ValidateTemplate Template, Issues
    Set Template.ValidationResults = Issues
    Template.HasValidated = True
    Set AppValidateTemplate = ComCreateSuccess(CreateValidationMessage(Issues))
    Exit Function

ErrHandler:
    Set Issues = New Collection
    AddIssue Issues, SeverityError, "VMF-TPL-000", "Template validation failed: " & Err.Description, vbNullString, vbNullString, 0, 1
    If Not Template Is Nothing Then
        Set Template.ValidationResults = Issues
        Template.HasValidated = True
    End If
    Set AppValidateTemplate = ComCreateSuccess(CreateValidationMessage(Issues))
End Function

Public Function AppSaveTemplate(ByVal Template As AppTemplateModel) As ComResult
    Dim Issues As Collection
    Dim Result As ComResult
    Dim TempPath As String
    Dim Reloaded As String

    On Error GoTo ErrHandler

    If Template Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppSaveTemplate", "Template is required."
    End If

    Set Result = AppValidateTemplate(Template, Issues)
    If IssuesContainErrors(Issues) Then
        Set AppSaveTemplate = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppTemplateService.AppSaveTemplate", CreateValidationMessage(Issues)))
        Exit Function
    End If

    TempPath = Template.FilePath & ".tmp"
    Set Result = FileProvider.InfWriteText(TempPath, Template.Content)
    If Result.IsFailure Then
        AddIssue Issues, SeverityError, "VMF-TPL-005", "Template file save failed: " & Result.Message, Template.TemplateName, Template.FilePath, 0, 900
        Set AppSaveTemplate = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppTemplateService.AppSaveTemplate", Result.Message))
        Exit Function
    End If

    Reloaded = FileProvider.InfReadText(TempPath)
    If Reloaded <> Template.Content Then
        Kill TempPath
        AddIssue Issues, SeverityError, "VMF-TPL-006", "Template save verification failed.", Template.TemplateName, Template.FilePath, 0, 901
        Set AppSaveTemplate = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppTemplateService.AppSaveTemplate", "Template save verification failed."))
        Exit Function
    End If

    If FileProvider.InfFileExists(Template.FilePath) Then Kill Template.FilePath
    Name TempPath As Template.FilePath
    Template.IsDirty = False
    Set AppSaveTemplate = ComCreateSuccess("Template saved.")
    Exit Function

ErrHandler:
    On Error Resume Next
    If Len(TempPath) > 0 Then
        If FileProvider.InfFileExists(TempPath) Then Kill TempPath
    End If
    On Error GoTo 0
    Set AppSaveTemplate = ComCreateFailureFromErr("AppTemplateService.AppSaveTemplate", Err.Description, "Error", False)
End Function

Public Function AppSaveTemplateAs( _
    ByVal Template As AppTemplateModel, _
    ByVal TemplateName As String, _
    ByVal FilePath As String) As ComResult

    On Error GoTo ErrHandler
    If Template Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppSaveTemplateAs", "Template is required."
    End If
    If ComIsBlankText(TemplateName) Then
        Set AppSaveTemplateAs = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppTemplateService.AppSaveTemplateAs", "Template name is required."))
        Exit Function
    End If
    If ComIsBlankText(FilePath) Then
        Set AppSaveTemplateAs = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppTemplateService.AppSaveTemplateAs", "Template file path is required."))
        Exit Function
    End If
    If FileProvider.InfFileExists(FilePath) Then
        Set AppSaveTemplateAs = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppTemplateService.AppSaveTemplateAs", "Template file already exists."))
        Exit Function
    End If

    Template.TemplateName = TemplateName
    Template.FilePath = FilePath
    Set AppSaveTemplateAs = AppSaveTemplate(Template)
    Exit Function

ErrHandler:
    Set AppSaveTemplateAs = ComCreateFailureFromErr("AppTemplateService.AppSaveTemplateAs", Err.Description, "Error", False)
End Function

Public Function AppPreviewTemplateWithModule( _
    ByVal ManifestPath As String, _
    ByVal Template As AppTemplateModel, _
    ByVal ModuleInfo As Object, _
    ByRef PreviewText As String) As ComResult

    Dim Item As ManifestItem
    Dim Issues As Collection
    Dim CodeText As String

    On Error GoTo ErrHandler

    PreviewText = vbNullString
    Set Issues = New Collection
    ValidateTemplate Template, Issues
    Set Template.ValidationResults = Issues
    Template.HasValidated = True
    If IssuesContainErrors(Issues) Then
        Set AppPreviewTemplateWithModule = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidArgument, "AppTemplateService.AppPreviewTemplateWithModule", CreateValidationMessage(Issues)))
        Exit Function
    End If

    Set Item = CreateManifestItem(ManifestPath, ModuleInfo)
    CodeText = Generator.InfGenerateManifestItemFromTemplateText(Item, Template.TemplateName, Template.Content)
    If HasUnresolvedPlaceholder(CodeText) Then
        Set AppPreviewTemplateWithModule = ComCreateFailure(ComCreateErrorInfo(ComErrInvalidState, "AppTemplateService.AppPreviewTemplateWithModule", "Unresolved placeholder: generated code still contains template placeholders."))
        Exit Function
    End If

    PreviewText = CodeText
    Set AppPreviewTemplateWithModule = ComCreateSuccess("Template preview generated.")
    Exit Function

ErrHandler:
    Set AppPreviewTemplateWithModule = ComCreateFailureFromErr("AppTemplateService.AppPreviewTemplateWithModule", Err.Description, "Error", False)
End Function

Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean
    AppIssuesContainErrors = IssuesContainErrors(Issues)
End Function

Private Function LoadTemplateModel(ByVal FilePath As String) As AppTemplateModel
    Dim Template As AppTemplateModel
    Dim Content As String

    If Not FileProvider.InfFileExists(FilePath) Then
        Err.Raise ComErrInvalidArgument, "LoadTemplateModel", "Template file does not exist: " & FilePath
    End If

    Content = FileProvider.InfReadText(FilePath)
    Set Template = New AppTemplateModel
    Template.AppInitialize GetTemplateName(FilePath), DetectTemplateType(FilePath, Content), FilePath, Content
    Set LoadTemplateModel = Template
End Function

Public Function AppDefaultTemplateFilePath(ByVal TemplateFileName As String) As String
    AppDefaultTemplateFilePath = BuildPathResolver.CombinePath(ResolveTemplateDirectory(), TemplateFileName)
End Function

Private Sub AnalyzeTemplate(ByVal Template As AppTemplateModel)
    Set Template.Placeholders = ParsePlaceholders(Template.Content)
    Set Template.Sections = ParseSections(Template.Content)
End Sub

Private Sub ValidateTemplate(ByVal Template As AppTemplateModel, ByVal Issues As Collection)
    Dim Placeholder As Object
    Dim SectionInfo As Object
    Dim RequiredName As Variant
    Dim KnownPlaceholders As Object
    Dim RequiredPlaceholders As Collection
    Dim SortIndex As Long

    If Template Is Nothing Then
        AddIssue Issues, SeverityError, "VMF-TPL-001", "Template is required.", vbNullString, vbNullString, 0, 1
        Exit Sub
    End If

    AnalyzeTemplate Template
    Set KnownPlaceholders = CreateKnownPlaceholders()
    Set RequiredPlaceholders = CreateRequiredPlaceholders(Template.TemplateType)

    If ComIsBlankText(Template.Content) Then
        AddIssue Issues, SeverityError, "VMF-TPL-001", "Template content is empty.", Template.TemplateName, "Content", 0, 1
    End If
    If ComIsBlankText(Template.TemplateType) Then
        AddIssue Issues, SeverityError, "VMF-TPL-002", "TemplateType is required.", Template.TemplateName, "TemplateType", 0, 2
    End If
    If InStr(1, Template.Content, vbLf, vbBinaryCompare) > 0 And InStr(1, Template.Content, vbCrLf, vbBinaryCompare) = 0 Then
        AddIssue Issues, SeverityWarning, "VMF-TPL-007", "Template line endings are not CRLF.", Template.TemplateName, "LineEnding", 0, 7
    End If

    For Each Placeholder In Template.Placeholders
        If Not KnownPlaceholders.Exists(CStr(Placeholder("Name"))) Then
            AddIssue Issues, SeverityError, "VMF-TPL-PH-001", "Unknown placeholder: " & CStr(Placeholder("Name")), Template.TemplateName, CStr(Placeholder("Name")), CLng(Placeholder("LineNumber")), 100 + CLng(Placeholder("LineNumber"))
        End If
    Next Placeholder

    For Each RequiredName In RequiredPlaceholders
        If Not PlaceholderExists(Template.Placeholders, CStr(RequiredName)) Then
            AddIssue Issues, SeverityError, "VMF-TPL-PH-002", "Required placeholder is missing: " & CStr(RequiredName), Template.TemplateName, CStr(RequiredName), 0, 200 + SortIndex
        End If
        SortIndex = SortIndex + 1
    Next RequiredName

    For Each SectionInfo In Template.Sections
        If Not CBool(SectionInfo("IsValid")) Then
            AddIssue Issues, SeverityError, CStr(SectionInfo("Code")), CStr(SectionInfo("Message")), Template.TemplateName, CStr(SectionInfo("Name")), CLng(SectionInfo("LineNumber")), 300 + CLng(SectionInfo("LineNumber"))
        End If
    Next SectionInfo

    If StrComp(Template.TemplateType, "Class", vbTextCompare) = 0 Then
        ValidateClassTemplate Template, Issues
    End If
End Sub

Private Sub ValidateClassTemplate(ByVal Template As AppTemplateModel, ByVal Issues As Collection)
    If InStr(1, Template.Content, "Option Explicit", vbTextCompare) = 0 Then
        AddIssue Issues, SeverityError, "VMF-TPL-CLS-001", "Class template requires Option Explicit.", Template.TemplateName, "Option Explicit", 0, 500
    End If
    If Not PlaceholderExists(Template.Placeholders, "ModuleName") Then
        AddIssue Issues, SeverityError, "VMF-TPL-CLS-002", "Class template requires ModuleName.", Template.TemplateName, "ModuleName", 0, 501
    End If
    If Not PlaceholderExists(Template.Placeholders, "BODY") Then
        AddIssue Issues, SeverityError, "VMF-TPL-CLS-003", "Class template requires BODY insertion point.", Template.TemplateName, "BODY", 0, 502
    End If
    If Not PlaceholderExists(Template.Placeholders, "MemberBlock") Then
        AddIssue Issues, SeverityWarning, "VMF-TPL-CLS-004", "Class template has no member declaration insertion point.", Template.TemplateName, "MemberBlock", 0, 503
    End If
    If Not PlaceholderExists(Template.Placeholders, "Properties") Then
        AddIssue Issues, SeverityWarning, "VMF-TPL-CLS-005", "Class template has no property insertion point.", Template.TemplateName, "Properties", 0, 504
    End If
End Sub

Private Function ParsePlaceholders(ByVal Content As String) As Collection
    Dim Placeholders As Collection
    Dim Index As Long
    Dim EndIndex As Long
    Dim Name As String
    Dim PlaceholderMap As Object
    Dim Placeholder As Object
    Dim LineNumber As Long

    Set Placeholders = New Collection
    Set PlaceholderMap = CreateObject("Scripting.Dictionary")
    Index = InStr(1, Content, "{{", vbBinaryCompare)
    Do While Index > 0
        EndIndex = InStr(Index + 2, Content, "}}", vbBinaryCompare)
        If EndIndex = 0 Then Exit Do

        Name = Trim$(Mid$(Content, Index + 2, EndIndex - Index - 2))
        LineNumber = CountLines(Left$(Content, Index))
        If PlaceholderMap.Exists(Name) Then
            Set Placeholder = PlaceholderMap(Name)
            Placeholder("OccurrenceCount") = CLng(Placeholder("OccurrenceCount")) + 1
        Else
            Set Placeholder = CreateObject("Scripting.Dictionary")
            Placeholder("Name") = Name
            Placeholder("OccurrenceCount") = 1
            Placeholder("LineNumber") = LineNumber
            Placeholder("IsKnown") = CreateKnownPlaceholders().Exists(Name)
            Placeholder("IsRequired") = False
            PlaceholderMap.Add Name, Placeholder
            Placeholders.Add Placeholder
        End If
        Index = InStr(EndIndex + 2, Content, "{{", vbBinaryCompare)
    Loop

    Set ParsePlaceholders = Placeholders
End Function

Private Function ParseSections(ByVal Content As String) As Collection
    Dim Sections As Collection
    Dim Lines() As String
    Dim LineIndex As Long
    Dim LineText As String
    Dim SectionName As String
    Dim ActiveName As String
    Dim Seen As Object
    Dim SectionInfo As Object

    Set Sections = New Collection
    Set Seen = CreateObject("Scripting.Dictionary")
    Lines = Split(NormalizeLineBreaks(Content), vbLf)

    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(Lines(LineIndex))
        If InStr(1, LineText, "@section", vbTextCompare) > 0 Then
            SectionName = Trim$(Mid$(LineText, InStr(1, LineText, "@section", vbTextCompare) + Len("@section")))
            If ComIsBlankText(SectionName) Then
                Sections.Add CreateSectionInfo(vbNullString, LineIndex + 1, False, "VMF-TPL-SEC-002", "Section name is required.")
            ElseIf Len(ActiveName) > 0 Then
                Sections.Add CreateSectionInfo(SectionName, LineIndex + 1, False, "VMF-TPL-SEC-004", "Nested sections are not supported.")
            ElseIf Seen.Exists(LCase$(SectionName)) Then
                Sections.Add CreateSectionInfo(SectionName, LineIndex + 1, False, "VMF-TPL-SEC-003", "Duplicate section name: " & SectionName)
                ActiveName = SectionName
            Else
                Seen.Add LCase$(SectionName), True
                Sections.Add CreateSectionInfo(SectionName, LineIndex + 1, True, vbNullString, vbNullString)
                ActiveName = SectionName
            End If
        ElseIf InStr(1, LineText, "@endsection", vbTextCompare) > 0 Then
            If Len(ActiveName) = 0 Then
                Sections.Add CreateSectionInfo(vbNullString, LineIndex + 1, False, "VMF-TPL-SEC-001", "@endsection has no matching @section.")
            Else
                ActiveName = vbNullString
            End If
        End If
    Next LineIndex

    If Len(ActiveName) > 0 Then
        Sections.Add CreateSectionInfo(ActiveName, UBound(Lines) + 1, False, "VMF-TPL-SEC-005", "Section is not closed: " & ActiveName)
    End If

    Set ParseSections = Sections
End Function

Private Function CreateSectionInfo(ByVal Name As String, ByVal LineNumber As Long, ByVal IsValid As Boolean, ByVal Code As String, ByVal Message As String) As Object
    Dim Info As Object
    Set Info = CreateObject("Scripting.Dictionary")
    Info("Name") = Name
    Info("LineNumber") = LineNumber
    Info("IsValid") = IsValid
    Info("Code") = Code
    Info("Message") = Message
    Set CreateSectionInfo = Info
End Function

Private Function CreateKnownPlaceholders() As Object
    Dim Known As Object
    Set Known = CreateObject("Scripting.Dictionary")
    Known.CompareMode = vbTextCompare
    Known("ModuleName") = True
    Known("ModuleType") = True
    Known("Layer") = True
    Known("TemplatePath") = True
    Known("BODY") = True
    Known("ModuleDeclaration") = True
    Known("DeclarationSectionEnd") = True
    Known("MemberDeclarations") = True
    Known("MemberBlock") = True
    Known("InitializeBody") = True
    Known("TerminateBody") = True
    Known("PropertyGroupStart") = True
    Known("Properties") = True
    Known("PropertyGroupEnd") = True
    Known("ProcedureGroupStart") = True
    Known("ProcedureGroupEnd") = True
    Set CreateKnownPlaceholders = Known
End Function

Private Function CreateRequiredPlaceholders(ByVal TemplateType As String) As Collection
    Dim Required As Collection
    Set Required = New Collection
    Required.Add "ModuleName"
    Required.Add "Layer"
    Required.Add "BODY"
    If StrComp(TemplateType, "Class", vbTextCompare) = 0 Then
        Required.Add "MemberBlock"
        Required.Add "Properties"
    End If
    Set CreateRequiredPlaceholders = Required
End Function

Private Function PlaceholderExists(ByVal Placeholders As Collection, ByVal Name As String) As Boolean
    Dim Placeholder As Object
    For Each Placeholder In Placeholders
        If StrComp(CStr(Placeholder("Name")), Name, vbTextCompare) = 0 Then
            PlaceholderExists = True
            Exit Function
        End If
    Next Placeholder
End Function

Private Function CreateManifestItem(ByVal ManifestPath As String, ByVal ModuleInfo As Object) As ManifestItem
    Dim Item As ManifestItem
    Dim FieldText As Variant
    Dim SeparatorIndex As Long
    Dim SectionName As String
    Dim SectionSourcePath As String

    Set Item = New ManifestItem
    Item.InfInitialize _
        CStr(ModuleInfo("ModuleName")), _
        CStr(ModuleInfo("ModuleType")), _
        CStr(ModuleInfo("Layer")), _
        ResolveManifestRelativePath(CStr(ModuleInfo("TemplatePath")), ManifestPath), _
        ResolveOptionalManifestPath(CStr(ModuleInfo("BodySourcePath")), ManifestPath)

    For Each FieldText In ModuleInfo("SectionFields")
        SeparatorIndex = InStr(1, CStr(FieldText), "=", vbBinaryCompare)
        If SeparatorIndex <= 1 Or SeparatorIndex = Len(CStr(FieldText)) Then
            Err.Raise ComErrInvalidArgument, "AppTemplateService.CreateManifestItem", "SectionSource entry must use SectionName=SectionSourcePath."
        End If

        SectionName = Trim$(Left$(CStr(FieldText), SeparatorIndex - 1))
        SectionSourcePath = Trim$(Mid$(CStr(FieldText), SeparatorIndex + 1))
        Item.InfAddSectionSourcePath SectionName, ResolveManifestRelativePath(SectionSourcePath, ManifestPath)
    Next FieldText

    If ModuleInfo("Members").Count > 0 Then
        Item.InfSetMemberSourceText ManifestEditorService.AppRenderManifestEditorMemberDefinitions(ModuleInfo("Members"))
    ElseIf Not ComIsBlankText(CStr(ModuleInfo("MemberSourcePath"))) Then
        Item.InfSetMemberSourcePath ResolveManifestRelativePath(CStr(ModuleInfo("MemberSourcePath")), ManifestPath)
    End If

    Set CreateManifestItem = Item
End Function

Private Function DetectTemplateType(ByVal FilePath As String, ByVal Content As String) As String
    Dim FileName As String
    FileName = LCase$(GetTemplateName(FilePath))
    If InStr(1, FileName, "class", vbTextCompare) > 0 Then
        DetectTemplateType = "Class"
    ElseIf InStr(1, FileName, "module", vbTextCompare) > 0 Then
        DetectTemplateType = "Standard Module"
    ElseIf InStr(1, FileName, "interface", vbTextCompare) > 0 Then
        DetectTemplateType = "Interface"
    ElseIf InStr(1, FileName, "enum", vbTextCompare) > 0 Then
        DetectTemplateType = "Enum"
    ElseIf InStr(1, Content, "Private Sub Class_Initialize", vbTextCompare) > 0 Then
        DetectTemplateType = "Class"
    Else
        DetectTemplateType = "Standard Module"
    End If
End Function

Private Function ResolveTemplateDirectory() As String
    Dim Settings As AppStudioSettings
    Dim Issues As Collection
    If Not SettingsService Is Nothing Then
        SettingsService.AppLoadSettings Settings, Issues
        If Not Settings Is Nothing Then
            ResolveTemplateDirectory = SettingsService.AppResolvePath(BuildPathResolver.RepositoryRootPath(), Settings.TemplateDirectory)
            Exit Function
        End If
    End If
    ResolveTemplateDirectory = BuildPathResolver.TemplatesDirectoryPath()
End Function

Private Function GetTemplateName(ByVal FilePath As String) As String
    Dim FileName As String
    Dim SeparatorIndex As Long
    Dim ExtensionIndex As Long
    SeparatorIndex = InStrRev(FilePath, "\")
    If SeparatorIndex = 0 Then SeparatorIndex = InStrRev(FilePath, "/")
    If SeparatorIndex > 0 Then
        FileName = Mid$(FilePath, SeparatorIndex + 1)
    Else
        FileName = FilePath
    End If
    ExtensionIndex = InStrRev(FileName, ".")
    If ExtensionIndex > 0 Then FileName = Left$(FileName, ExtensionIndex - 1)
    GetTemplateName = FileName
End Function

Private Function CountLines(ByVal TextValue As String) As Long
    CountLines = UBound(Split(NormalizeLineBreaks(TextValue), vbLf)) + 1
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    Dim Result As String
    Result = Replace(TextValue, vbCrLf, vbLf)
    Result = Replace(Result, vbCr, vbLf)
    NormalizeLineBreaks = Result
End Function

Private Function ResolveOptionalManifestPath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    If ComIsBlankText(FilePath) Then
        ResolveOptionalManifestPath = vbNullString
    Else
        ResolveOptionalManifestPath = ResolveManifestRelativePath(FilePath, ManifestPath)
    End If
End Function

Private Function ResolveManifestRelativePath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    Dim ManifestDirectoryPath As String
    Dim NormalizedFilePath As String

    NormalizedFilePath = Replace(FilePath, "/", Application.PathSeparator)
    If Len(NormalizedFilePath) >= 2 Then
        If Mid$(NormalizedFilePath, 2, 1) = ":" Then
            ResolveManifestRelativePath = NormalizedFilePath
            Exit Function
        End If
    End If
    If Left$(NormalizedFilePath, 2) = "\\" Then
        ResolveManifestRelativePath = NormalizedFilePath
        Exit Function
    End If

    ManifestDirectoryPath = Left$(ManifestPath, InStrRev(ManifestPath, Application.PathSeparator) - 1)
    ResolveManifestRelativePath = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName( _
        BuildPathResolver.CombinePath(ManifestDirectoryPath, NormalizedFilePath))
End Function

Private Function HasKey(ByVal Values As Object, ByVal KeyName As String) As Boolean
    On Error GoTo NotFound
    HasKey = Values.Exists(KeyName)
    Exit Function
NotFound:
    HasKey = False
End Function

Private Function HasUnresolvedPlaceholder(ByVal CodeText As String) As Boolean
    HasUnresolvedPlaceholder = (InStr(1, CodeText, "{{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}}", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}", vbBinaryCompare) > 0)
End Function

Private Function IssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim Issue As AppTemplateValidationIssue
    If Issues Is Nothing Then Exit Function
    For Each Issue In Issues
        If Issue.Severity = SeverityError Then
            IssuesContainErrors = True
            Exit Function
        End If
    Next Issue
End Function

Private Function CreateValidationMessage(ByVal Issues As Collection) As String
    If Issues Is Nothing Then
        CreateValidationMessage = "Template validation passed."
    ElseIf Issues.Count = 0 Then
        CreateValidationMessage = "Template validation passed."
    ElseIf IssuesContainErrors(Issues) Then
        CreateValidationMessage = "Template validation failed."
    Else
        CreateValidationMessage = "Template validation completed with warnings."
    End If
End Function

Private Sub AddIssue( _
    ByVal Issues As Collection, _
    ByVal Severity As Long, _
    ByVal Code As String, _
    ByVal Message As String, _
    ByVal TemplateName As String, _
    ByVal ItemName As String, _
    ByVal LineNumber As Long, _
    ByVal SortOrder As Long)

    Dim Issue As AppTemplateValidationIssue
    Set Issue = New AppTemplateValidationIssue
    Issue.AppInitialize Severity, Code, Message, TemplateName, ItemName, LineNumber, SortOrder
    Issues.Add Issue
End Sub
