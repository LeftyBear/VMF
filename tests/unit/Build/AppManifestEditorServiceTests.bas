Option Explicit
Attribute VB_Name = "AppManifestEditorServiceTests"

'=========================================================================
' Module: AppManifestEditorServiceTests
' Layer: Application
' Responsibility: Verification tests for Manifest Editor service.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9910

Public Sub AppRunManifestEditorServiceTests()
    VerifyManifestEditorRoundTrip
    VerifyManifestEditorPreviewUsesUnsavedModel
    VerifyManifestValidationFindsModuleAndMemberErrors
    VerifyManifestValidationWarningsDoNotBlockSave
    VerifyManifestEditorRejectsDuplicateModules
End Sub

Private Sub VerifyManifestEditorRoundTrip()
    Dim FileProvider As InfFileSystemProvider
    Dim Service As AppManifestEditorService
    Dim ManifestPath As String
    Dim TemplatePath As String
    Dim Modules As Collection
    Dim ReloadedModules As Collection
    Dim Result As ComResult
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim Item As ManifestItem
    Dim Items As Collection
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim GeneratedCode As String

    ManifestPath = GetTestFolderPath() & "\ManifestEditor.manifest"
    TemplatePath = GetTestFolderPath() & "\templates\ClassTemplate.txt"
    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText TemplatePath, ReadTemplateContent("ClassTemplate.txt")
    FileProvider.InfWriteText ManifestPath, _
        "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "SampleService,ClassModule,Application,templates\ClassTemplate.txt"

    Set Service = New AppManifestEditorService
    Set Result = Service.AppLoadManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsSuccess, "Manifest should load."

    Set ModuleInfo = Modules.Item(1)
    Set Members = ModuleInfo("Members")
    Members.Add Service.AppCreateManifestEditorMember("Name", "String", "GetLet", "vbNullString", False)
    Members.Add Service.AppCreateManifestEditorMember("Parent", "SampleParent", "GetSet", vbNullString, True)

    Set Result = Service.AppSaveManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsSuccess, "Manifest should save."

    Set Result = Service.AppLoadManifestEditorModel(ManifestPath, ReloadedModules)
    AssertTrue Result.IsSuccess, "Saved manifest should reload."
    AssertEquals "1", CStr(ReloadedModules.Count), "Reloaded manifest should keep one module."
    AssertEquals "2", CStr(ReloadedModules.Item(1)("Members").Count), "Reloaded manifest should keep members."

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ManifestPath)
    Set Item = Items.Item(1)
    AssertTrue Len(Item.InfGetMemberSourcePath()) > 0, "Saved manifest should define MemberSourcePath."

    Set Generator = InfCreateGenerator()
    GeneratedCode = Generator.InfGenerateManifestItem(Item)
    AssertTrue InStr(1, GeneratedCode, "Public Property Get Name()", vbTextCompare) > 0, "Saved member should generate property code."
    AssertTrue InStr(1, GeneratedCode, "Set This.Parent = New SampleParent", vbTextCompare) > 0, "CreateInstance should generate initialization code."
End Sub

Private Sub VerifyManifestEditorRejectsDuplicateModules()
    Dim Service As AppManifestEditorService
    Dim Modules As Collection
    Dim Result As ComResult

    Set Service = New AppManifestEditorService
    Set Modules = New Collection
    Modules.Add Service.AppCreateManifestEditorModule("DuplicateModule", "Application", "ClassModule", "templates\ClassTemplate.txt")
    Modules.Add Service.AppCreateManifestEditorModule("DuplicateModule", "Application", "ClassModule", "templates\ClassTemplate.txt")

    Set Result = Service.AppSaveManifestEditorModel(GetTestFolderPath() & "\Duplicate.manifest", Modules)
    AssertTrue Result.IsFailure, "Duplicate module names should fail validation."
End Sub

Private Sub VerifyManifestEditorPreviewUsesUnsavedModel()
    Dim FileProvider As InfFileSystemProvider
    Dim Service As AppManifestEditorService
    Dim PreviewService As AppCodePreviewService
    Dim ManifestPath As String
    Dim TemplatePath As String
    Dim Modules As Collection
    Dim Result As ComResult
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim PreviewText As String
    Dim ManifestBefore As String
    Dim ManifestAfterPreview As String
    Dim ReloadedItems As Collection
    Dim GeneratedText As String
    Dim Generator As InfGenerator

    ManifestPath = GetTestFolderPath() & "\ManifestEditorPreview.manifest"
    TemplatePath = GetTestFolderPath() & "\templates\ClassTemplate.txt"
    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText TemplatePath, ReadTemplateContent("ClassTemplate.txt")
    FileProvider.InfWriteText ManifestPath, _
        "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "PreviewService,ClassModule,Application,templates\ClassTemplate.txt"

    Set Service = New AppManifestEditorService
    Set Result = Service.AppLoadManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsSuccess, "Manifest should load for preview."

    Set ModuleInfo = Modules.Item(1)
    Set Members = ModuleInfo("Members")
    Members.Add Service.AppCreateManifestEditorMember("Name", "String", "GetLet", "vbNullString", False)
    Members.Add Service.AppCreateManifestEditorMember("Parent", "SampleParent", "GetSet", vbNullString, True)

    ManifestBefore = FileProvider.InfReadText(ManifestPath)
    Set PreviewService = New AppCodePreviewService
    PreviewService.AppInitialize InfCreateGenerator(), Service, New AppManifestValidationService
    Set Result = PreviewService.AppPreviewManifestEditorModule(ManifestPath, ModuleInfo, PreviewText)
    AssertTrue Result.IsSuccess, "Preview should render from unsaved module model."
    AssertTrue InStr(1, PreviewText, "Public Property Get Name()", vbTextCompare) > 0, "Preview should include unsaved member code."

    ManifestAfterPreview = FileProvider.InfReadText(ManifestPath)
    AssertEquals ManifestBefore, ManifestAfterPreview, "Preview must not update the manifest file."

    Set Result = Service.AppSaveManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsSuccess, "Saving previewed model should succeed."

    Set Generator = InfCreateGenerator()
    Set ReloadedItems = InfCreateManifestProvider().InfLoadManifestItems(ManifestPath)
    GeneratedText = Generator.InfGenerateManifestItem(ReloadedItems.Item(1))
    AssertEquals GeneratedText, PreviewText, "Preview and saved generation should produce identical code."
End Sub

Private Sub VerifyManifestValidationFindsModuleAndMemberErrors()
    Dim FileProvider As InfFileSystemProvider
    Dim Service As AppManifestEditorService
    Dim ValidationService As AppManifestValidationService
    Dim PreviewService As AppCodePreviewService
    Dim ManifestPath As String
    Dim TemplatePath As String
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim Issues As Collection
    Dim Result As ComResult
    Dim PreviewText As String

    ManifestPath = GetTestFolderPath() & "\ManifestValidationErrors.manifest"
    TemplatePath = GetTestFolderPath() & "\templates\ClassTemplate.txt"
    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText TemplatePath, ReadTemplateContent("ClassTemplate.txt")

    Set Service = New AppManifestEditorService
    Set ValidationService = New AppManifestValidationService
    Set Modules = New Collection
    Set ModuleInfo = Service.AppCreateManifestEditorModule("Invalid-Module", "Application", "ClassModule", "templates\ClassTemplate.txt")
    Set Members = ModuleInfo("Members")
    Members.Add Service.AppCreateManifestEditorMember("Child", "SampleChild", "GetLet", vbNullString, True)
    Modules.Add ModuleInfo

    Set Result = ValidationService.AppValidateManifestEditorModel(ManifestPath, Modules, Issues)
    AssertTrue Result.IsSuccess, "Validation should return a result."
    AssertTrue ValidationService.AppIssuesContainErrors(Issues), "Validation should find errors."
    AssertIssueCode Issues, "VMF-MOD-002"
    AssertIssueCode Issues, "VMF-MEM-009"

    Set Result = Service.AppSaveManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsFailure, "Save should stop when validation has errors."

    Set PreviewService = New AppCodePreviewService
    PreviewService.AppInitialize InfCreateGenerator(), Service, ValidationService
    Set Result = PreviewService.AppPreviewManifestEditorModule(ManifestPath, ModuleInfo, PreviewText)
    AssertTrue Result.IsFailure, "Preview should stop when validation has errors."
End Sub

Private Sub VerifyManifestValidationWarningsDoNotBlockSave()
    Dim FileProvider As InfFileSystemProvider
    Dim Service As AppManifestEditorService
    Dim ValidationService As AppManifestValidationService
    Dim ManifestPath As String
    Dim TemplatePath As String
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Members As Collection
    Dim MemberInfo As Object
    Dim Issues As Collection
    Dim Result As ComResult

    ManifestPath = GetTestFolderPath() & "\ManifestValidationWarnings.manifest"
    TemplatePath = GetTestFolderPath() & "\templates\ClassTemplate.txt"
    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText TemplatePath, ReadTemplateContent("ClassTemplate.txt")

    Set Service = New AppManifestEditorService
    Set ValidationService = New AppManifestValidationService
    Set Modules = New Collection
    Set ModuleInfo = Service.AppCreateManifestEditorModule("WarningService", "Application", "ClassModule", "templates\ClassTemplate.txt")
    Set Members = ModuleInfo("Members")
    Set MemberInfo = Service.AppCreateManifestEditorMember("Child", "SampleChild", "GetSet", "ExistingChild", True)
    Members.Add MemberInfo
    Modules.Add ModuleInfo

    Set Result = ValidationService.AppValidateManifestEditorModel(ManifestPath, Modules, Issues)
    AssertTrue Result.IsSuccess, "Warning validation should complete."
    AssertFalse ValidationService.AppIssuesContainErrors(Issues), "Warnings should not count as errors."
    AssertIssueCode Issues, "VMF-MEM-013"

    Set Result = Service.AppSaveManifestEditorModel(ManifestPath, Modules)
    AssertTrue Result.IsSuccess, "Warnings should not block save."
End Sub

Private Function ReadTemplateContent(ByVal TemplateFileName As String) As String
    Dim AddinWorkbook As Object
    Dim FileSystem As Object
    Dim TextFile As Object
    Dim RootPath As String
    Dim TemplatePath As String

    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    On Error GoTo 0

    If Not AddinWorkbook Is Nothing Then
        RootPath = AddinWorkbook.Path
    Else
        RootPath = ThisWorkbook.Path
    End If

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    TemplatePath = ResolveWorkspaceRootPath(RootPath) & Application.PathSeparator & "templates" & Application.PathSeparator & TemplateFileName
    Set TextFile = FileSystem.OpenTextFile(TemplatePath, 1, False)
    ReadTemplateContent = TextFile.ReadAll
    TextFile.Close
End Function

Private Function GetTestFolderPath() As String
    GetTestFolderPath = Environ$("TEMP") & "\VMF_AppManifestEditorServiceTests"
End Function

Private Function ResolveWorkspaceRootPath(ByVal CandidatePath As String) As String
    Dim FileSystem As Object
    Dim CurrentPath As String
    Dim ParentPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    CurrentPath = FileSystem.GetAbsolutePathName(CandidatePath)

    Do
        If FileSystem.FileExists(FileSystem.BuildPath(CurrentPath, ".vmf-root")) Then
            ResolveWorkspaceRootPath = CurrentPath
            Exit Function
        End If

        ParentPath = FileSystem.GetParentFolderName(CurrentPath)
        If Len(ParentPath) = 0 Or StrComp(ParentPath, CurrentPath, vbTextCompare) = 0 Then
            Exit Do
        End If

        CurrentPath = ParentPath
    Loop

    ResolveWorkspaceRootPath = CandidatePath
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppManifestEditorServiceTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    AssertTrue Not Condition, Message
End Sub

Private Sub AssertIssueCode(ByVal Issues As Collection, ByVal ExpectedCode As String)
    Dim Issue As AppManifestValidationIssue

    If Issues Is Nothing Then
        Err.Raise AppTestAssertErrorNumber, "AppManifestEditorServiceTests", "Validation issues are required."
    End If

    For Each Issue In Issues
        If Issue.Code = ExpectedCode Then
            Exit Sub
        End If
    Next Issue

    Err.Raise AppTestAssertErrorNumber, "AppManifestEditorServiceTests", "Expected validation code was not found: " & ExpectedCode
End Sub

Private Sub AssertEquals(ByVal Expected As String, ByVal Actual As String, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppManifestEditorServiceTests", Message & " Expected=" & Expected & " Actual=" & Actual
    End If
End Sub
