VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppManifestGenerateService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppManifestGenerateService
' Layer: Application
' Responsibility: Generate source files from editable manifest models.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Generator As InfGenerator
Private ManifestEditorService As AppManifestEditorService
Private ManifestValidationService As AppManifestValidationService
Private FileProvider As InfFileSystemProvider

Private Const TargetScopeAllModules As String = "AllModules"
Private Const TargetScopeSelectedModules As String = "SelectedModules"
Private Const TargetScopeCurrentModule As String = "CurrentModule"
Private Const OverwriteModeConfirm As String = "Confirm"
Private Const OverwriteModeOverwrite As String = "Overwrite"
Private Const OverwriteModeSkip As String = "Skip"

Public Sub AppInitialize( _
    ByVal ModuleGenerator As InfGenerator, _
    ByVal EditorService As AppManifestEditorService, _
    ByVal ValidationService As AppManifestValidationService, _
    ByVal ProviderFileSystem As InfFileSystemProvider)

    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppManifestGenerateService.AppInitialize", "ModuleGenerator is required."
    End If
    If EditorService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppManifestGenerateService.AppInitialize", "ManifestEditorService is required."
    End If
    If ValidationService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppManifestGenerateService.AppInitialize", "ManifestValidationService is required."
    End If
    If ProviderFileSystem Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppManifestGenerateService.AppInitialize", "FileSystemProvider is required."
    End If

    Set Generator = ModuleGenerator
    Set ManifestEditorService = EditorService
    Set ManifestValidationService = ValidationService
    Set FileProvider = ProviderFileSystem
End Sub

Public Function AppCreateGenerateRequest( _
    ByVal TargetScope As String, _
    ByVal SelectedModuleNames As Collection, _
    ByVal OutputDirectory As String, _
    ByVal OverwriteMode As String, _
    ByVal ContinueOnError As Boolean) As AppGenerateRequest

    Dim Request As AppGenerateRequest

    Set Request = New AppGenerateRequest
    Request.AppInitialize TargetScope, SelectedModuleNames, OutputDirectory, OverwriteMode, ContinueOnError, Now
    Set AppCreateGenerateRequest = Request
End Function

Public Function AppGenerateManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByVal Request As AppGenerateRequest, _
    ByRef GenerateResult As AppGenerateResult) As ComResult

    Dim Issues As Collection
    Dim ValidationResult As ComResult
    Dim StartedAt As Date
    Dim Targets As Collection
    Dim ModuleInfo As Object
    Dim ModuleResult As AppModuleGenerateResult

    On Error GoTo ErrHandler

    ComRequireText ManifestPath, "ManifestPath", "AppGenerateManifestEditorModel"
    If Request Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppGenerateManifestEditorModel", "Generate request is required."
    End If

    StartedAt = Now
    Set GenerateResult = New AppGenerateResult
    GenerateResult.AppInitialize StartedAt, Request.OutputDirectory

    Set ValidationResult = ManifestValidationService.AppValidateManifestEditorModel(ManifestPath, Modules, Issues)
    If ValidationResult.IsFailure Then
        Set AppGenerateManifestEditorModel = ValidationResult
        GenerateResult.AppFinish Now
        Exit Function
    End If

    If ManifestValidationService.AppIssuesContainErrors(Issues) Then
        AddProjectFailure GenerateResult, Request.OutputDirectory, "VMF-GEN-VALIDATION", "Validation Error: generation was not started."
        GenerateResult.AppFinish Now
        Set AppGenerateManifestEditorModel = ManifestValidationService.AppCreateFailureFromIssues(Issues, "AppManifestGenerateService.AppGenerateManifestEditorModel")
        Exit Function
    End If

    Set Targets = SelectTargetModules(Modules, Request)
    Set ValidationResult = FileProvider.InfEnsureFolder(Request.OutputDirectory)
    If ValidationResult.IsFailure Then
        AddProjectFailure GenerateResult, Request.OutputDirectory, "VMF-GEN-WRITE", "File Write Error: " & ValidationResult.Message
        GenerateResult.AppFinish Now
        Set AppGenerateManifestEditorModel = ValidationResult
        Exit Function
    End If

    For Each ModuleInfo In Targets
        Set ModuleResult = GenerateOneModule(ManifestPath, ModuleInfo, Request)
        GenerateResult.AppAddModuleResult ModuleResult
        If ModuleResult.Status = "Failed" And Not Request.ContinueOnError Then
            Exit For
        End If
    Next ModuleInfo

    GenerateResult.AppFinish Now
    If GenerateResult.FailureCount > 0 Then
        Set AppGenerateManifestEditorModel = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "AppManifestGenerateService", "Generation completed with failures."))
    Else
        Set AppGenerateManifestEditorModel = ComCreateSuccess("Generation completed.")
    End If
    Exit Function

ErrHandler:
    If GenerateResult Is Nothing Then
        Set GenerateResult = New AppGenerateResult
        If Request Is Nothing Then
            GenerateResult.AppInitialize Now, "."
        Else
            GenerateResult.AppInitialize Now, Request.OutputDirectory
        End If
    End If
    AddProjectFailure GenerateResult, GenerateResult.OutputDirectory, "VMF-GEN-UNEXPECTED", "Unexpected Error: " & Err.Description
    GenerateResult.AppFinish Now
    Set AppGenerateManifestEditorModel = ComCreateFailureFromErr("AppManifestGenerateService.AppGenerateManifestEditorModel", Err.Description, "Error", False)
End Function

Public Function AppRenderManifestEditorModule(ByVal ManifestPath As String, ByVal ModuleInfo As Object) As String
    AppRenderManifestEditorModule = Generator.InfGenerateManifestItem(CreateManifestItem(ManifestPath, ModuleInfo))
End Function

Public Function AppDefaultOutputDirectory(ByVal ManifestPath As String) As String
    AppDefaultOutputDirectory = BuildPathResolver.CombinePath(GetParentFolderPath(ManifestPath), "GeneratedSource")
End Function

Private Function SelectTargetModules(ByVal Modules As Collection, ByVal Request As AppGenerateRequest) As Collection
    Dim Targets As Collection
    Dim ModuleInfo As Object
    Dim SelectedNames As Object
    Dim NameValue As Variant

    Set Targets = New Collection
    If Request.TargetScope = TargetScopeAllModules Then
        For Each ModuleInfo In Modules
            Targets.Add ModuleInfo
        Next ModuleInfo
    Else
        Set SelectedNames = CreateObject("Scripting.Dictionary")
        For Each NameValue In Request.SelectedModuleNames
            SelectedNames(LCase$(CStr(NameValue))) = True
        Next NameValue

        For Each ModuleInfo In Modules
            If SelectedNames.Exists(LCase$(CStr(ModuleInfo("ModuleName")))) Then
                Targets.Add ModuleInfo
            End If
        Next ModuleInfo
    End If

    Set SelectTargetModules = Targets
End Function

Private Function GenerateOneModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByVal Request As AppGenerateRequest) As AppModuleGenerateResult

    Dim StartedAt As Single
    Dim OutputPath As String
    Dim CodeText As String
    Dim WasOverwritten As Boolean
    Dim Result As ComResult

    On Error GoTo ErrHandler

    StartedAt = Timer
    OutputPath = BuildOutputPath(Request.OutputDirectory, CStr(ModuleInfo("ModuleName")), CStr(ModuleInfo("ModuleType")))
    WasOverwritten = FileProvider.InfFileExists(OutputPath)

    If WasOverwritten And Request.OverwriteMode = OverwriteModeSkip Then
        Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Skipped", "Existing File Skip: existing file was not overwritten.", "VMF-GEN-SKIP", Timer - StartedAt, False)
        Exit Function
    End If

    If WasOverwritten And Request.OverwriteMode = OverwriteModeConfirm Then
        Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Skipped", "Existing File Skip: overwrite was not confirmed.", "VMF-GEN-SKIP", Timer - StartedAt, False)
        Exit Function
    End If

    On Error GoTo RenderFailed
    CodeText = AppRenderManifestEditorModule(ManifestPath, ModuleInfo)
    On Error GoTo ErrHandler

    If HasUnresolvedPlaceholder(CodeText) Then
        Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "Render Error: unresolved placeholder.", "VMF-GEN-RENDER", Timer - StartedAt, False)
        Exit Function
    End If

    Set Result = FileProvider.InfWriteText(OutputPath, CodeText)
    If Result.IsFailure Then
        Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "File Write Error: " & Result.Message, "VMF-GEN-WRITE", Timer - StartedAt, False)
    Else
        Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Generated", "Generated", vbNullString, Timer - StartedAt, WasOverwritten)
    End If
    Exit Function

RenderFailed:
    Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", ClassifyRenderError(Err.Source, Err.Description), "VMF-GEN-RENDER", Timer - StartedAt, False)
    Exit Function

ErrHandler:
    Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "Unexpected Error: " & Err.Description, "VMF-GEN-UNEXPECTED", Timer - StartedAt, False)
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
            Err.Raise ComErrInvalidArgument, "AppManifestGenerateService.CreateManifestItem", "SectionSource entry must use SectionName=SectionSourcePath."
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

Private Function BuildOutputPath(ByVal OutputDirectory As String, ByVal ModuleName As String, ByVal ModuleType As String) As String
    Dim ExtensionName As String

    If ModuleType = "ClassModule" Then
        ExtensionName = ".cls"
    Else
        ExtensionName = ".bas"
    End If

    BuildOutputPath = BuildPathResolver.CombinePath(OutputDirectory, ModuleName & ExtensionName)
End Function

Private Function CreateModuleResult( _
    ByVal ModuleInfo As Object, _
    ByVal OutputPath As String, _
    ByVal Status As String, _
    ByVal Message As String, _
    ByVal ErrorCode As String, _
    ByVal ElapsedTime As Double, _
    ByVal WasOverwritten As Boolean) As AppModuleGenerateResult

    Dim ModuleResult As AppModuleGenerateResult

    Set ModuleResult = New AppModuleGenerateResult
    ModuleResult.AppInitialize _
        CStr(ModuleInfo("ModuleName")), _
        CStr(ModuleInfo("ModuleType")), _
        OutputPath, _
        Status, _
        Message, _
        ErrorCode, _
        ElapsedTime, _
        WasOverwritten, _
        Now
    Set CreateModuleResult = ModuleResult
End Function

Private Sub AddProjectFailure(ByVal GenerateResult As AppGenerateResult, ByVal OutputPath As String, ByVal ErrorCode As String, ByVal Message As String)
    Dim ModuleResult As AppModuleGenerateResult

    Set ModuleResult = New AppModuleGenerateResult
    ModuleResult.AppInitialize "Project", "Project", OutputPath, "Failed", Message, ErrorCode, 0, False, Now
    GenerateResult.AppAddModuleResult ModuleResult
End Sub

Private Function ClassifyRenderError(ByVal ErrorSource As String, ByVal ErrorDescription As String) As String
    Dim CombinedText As String

    CombinedText = LCase$(CStr(ErrorSource) & " " & CStr(ErrorDescription))
    If InStr(1, CombinedText, "template", vbTextCompare) > 0 Then
        ClassifyRenderError = "Template Load Error: " & ErrorDescription
    ElseIf InStr(1, CombinedText, "section", vbTextCompare) > 0 Then
        ClassifyRenderError = "Render Error: section insertion failed. " & ErrorDescription
    ElseIf InStr(1, CombinedText, "placeholder", vbTextCompare) > 0 Then
        ClassifyRenderError = "Render Error: unresolved placeholder. " & ErrorDescription
    Else
        ClassifyRenderError = "Render Error: " & ErrorDescription
    End If
End Function

Private Function HasUnresolvedPlaceholder(ByVal CodeText As String) As Boolean
    HasUnresolvedPlaceholder = (InStr(1, CodeText, "{{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}}", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "{", vbBinaryCompare) > 0 _
        Or InStr(1, CodeText, "}", vbBinaryCompare) > 0)
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
