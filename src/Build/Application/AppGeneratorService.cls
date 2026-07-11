VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGeneratorService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: AppGeneratorService
' Layer: Application
' Responsibility: Module generation service (Phase 1: Generate Module)
' Dependencies: Common, Infrastructure
'==========================================================================

Private Generator As InfGenerator
Private GenerationEngine As BuildGenerationEngine
Private ManifestProvider As InfManifestProvider

Private Const DefaultApplicationName As String = "SchoolTimetable"

' Initializes the service with required generation infrastructure.
Public Sub AppInitialize( _
    ByVal ModuleGenerator As InfGenerator, _
    ByVal Engine As BuildGenerationEngine, _
    ByVal ProviderManifest As InfManifestProvider)

    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ModuleGenerator is required."
    End If

    If Engine Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "BuildGenerationEngine is required."
    End If

    If ProviderManifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ManifestProvider is required."
    End If

    Set Generator = ModuleGenerator
    Set GenerationEngine = Engine
    Set ManifestProvider = ProviderManifest
End Sub

' Generates a standard module with the required template.
' Parameters:
'   ModuleName: Desired module name.
' Return value:
'   ComResult describing success or failure.
Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult
    Dim CodeText As String
    Dim LayerName As String

    On Error GoTo ErrHandler

    ComRequireText ModuleName, "ModuleName", "AppGenerateModule"

    If Not ModuleNameLikeValid(ModuleName) Then
        Err.Raise vbObjectError + 9100, "AppGenerateModule", "ModuleName is invalid. Use letters, numbers and underscore; start with a letter."
    End If

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "GeneratorService is not initialized."
    End If

    If GenerationEngine Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "BuildGenerationEngine is not initialized."
    End If

    LayerName = ResolveLayerName(ModuleName)
    CodeText = Generator.InfGenerateStandardModule(ModuleName, LayerName)
    Set AppGenerateModule = GenerationEngine.GenerateComponent(ModuleName, "StandardModule", CodeText)
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set AppGenerateModule = ComCreateFailureFromErr("AppGeneratorService.AppGenerateModule", Desc, "Error", False)
End Function

' Generates a class module with the required template.
' Parameters:
'   ClassName: Desired class name.
' Return value:
'   ComResult describing success or failure.
Public Function AppGenerateClass(ByVal ClassName As String) As ComResult
    Dim CodeText As String
    Dim LayerName As String

    On Error GoTo ErrHandler

    ComRequireText ClassName, "ClassName", "AppGenerateClass"

    If Not ModuleNameLikeValid(ClassName) Then
        Err.Raise vbObjectError + 9100, "AppGenerateClass", "ClassName is invalid. Use letters, numbers and underscore; start with a letter."
    End If

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateClass", "GeneratorService is not initialized."
    End If

    If GenerationEngine Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateClass", "BuildGenerationEngine is not initialized."
    End If

    LayerName = ResolveLayerName(ClassName)
    CodeText = Generator.InfGenerateStandardClass(ClassName, LayerName)
    Set AppGenerateClass = GenerationEngine.GenerateComponent(ClassName, "ClassModule", CodeText)
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set AppGenerateClass = ComCreateFailureFromErr("AppGeneratorService.AppGenerateClass", Desc, "Error", False)
End Function

' Generates the Common layer from Common.manifest.
Public Function AppGenerateCommon() As ComResult
    Set AppGenerateCommon = GenerateBuildLayerManifest("Common")
End Function

' Generates the Manifest layer from Manifest.manifest.
Public Function AppGenerateManifest() As ComResult
    Set AppGenerateManifest = GenerateBuildLayerManifest("Manifest")
End Function

' Generates the Infrastructure layer from Infrastructure.manifest.
Public Function AppGenerateInfrastructure() As ComResult
    Set AppGenerateInfrastructure = GenerateBuildLayerManifest("Infrastructure")
End Function

' Generates the Domain layer from Domain.manifest.
Public Function AppGenerateDomain() As ComResult
    Set AppGenerateDomain = GenerateBuildLayerManifest("Domain")
End Function

' Generates the Application layer from Application.manifest.
Public Function AppGenerateApplication() As ComResult
    Set AppGenerateApplication = GenerateBuildLayerManifest("Application")
End Function

' Generates the Presentation layer from Presentation.manifest.
Public Function AppGeneratePresentation() As ComResult
    Set AppGeneratePresentation = GenerateBuildLayerManifest("Presentation")
End Function

' Generates all Build layers in canonical dependency order.
Public Function AppGenerateProject() As ComResult
    Set AppGenerateProject = GenerateProject()
End Function

Private Function GenerateProject() As ComResult
    Dim ManifestPath As String
    Dim ManifestContent As String
    Dim ProjectManifest As Build_ProjectManifest
    Dim Result As ComResult

    On Error GoTo ErrHandler

    ManifestPath = BuildPathResolver.ManifestFilePath(ResolveApplicationName())
    ManifestContent = LoadProjectManifest(ManifestPath)

    Set Result = CreateProjectManifest(ManifestContent, ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Common", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Manifest", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Infrastructure", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Domain", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Application", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Presentation", ProjectManifest)
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set GenerateProject = ComCreateSuccess("Project generated.")
    Exit Function

ErrHandler:
    Set GenerateProject = ComCreateFailureFromErr("AppGeneratorService.GenerateProject", Err.Description, "Error", False)
End Function

Private Function ResolveApplicationName() As String
    ResolveApplicationName = DefaultApplicationName
End Function

Private Function LoadProjectManifest( _
    ByVal ManifestPath As String _
) As String
    Dim FileSystem As Object
    Dim ManifestFile As Object
    Dim Content As String

    ComRequireText ManifestPath, "ManifestPath", "LoadProjectManifest"

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    If Not FileSystem.FileExists(ManifestPath) Then
        Err.Raise ComErrInvalidState, "LoadProjectManifest", "ManifestPath does not exist."
    End If

    Set ManifestFile = FileSystem.OpenTextFile(ManifestPath, 1, False)
    Content = ManifestFile.ReadAll
    ManifestFile.Close

    If ComIsBlankText(Content) Then
        Err.Raise ComErrInvalidState, "LoadProjectManifest", "Manifest contains no content."
    End If

    LoadProjectManifest = Content
End Function

Private Function CreateProjectManifest( _
    ByVal ManifestContent As String, _
    ByRef ProjectManifest As Build_ProjectManifest _
) As ComResult

    Set ProjectManifest = New Build_ProjectManifest
    Set CreateProjectManifest = ProjectManifest.BuildInitializeFromContent(ManifestContent)
End Function

Private Function GenerateLayer( _
    ByVal LayerName As String, _
    ByVal ProjectManifest As Build_ProjectManifest _
) As ComResult
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim CodeText As String
    Dim Result As ComResult
    Dim OperationName As String

    On Error GoTo ErrHandler

    OperationName = "AppGenerate" & LayerName

    ComRequireText LayerName, "LayerName", OperationName
    If ProjectManifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, OperationName, "ProjectManifest is required."
    End If

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "GeneratorService is not initialized."
    End If

    If GenerationEngine Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "BuildGenerationEngine is not initialized."
    End If

    If Not ProjectManifest.BuildHasLayer(LayerName) Or Not ProjectManifest.BuildIsGenerationEnabled(LayerName) Then
        Set GenerateLayer = ComCreateSuccess(LayerName & " layer skipped.")
        Exit Function
    End If

    Set Items = ProjectManifest.BuildGetLayerItems(LayerName)
    For Each Item In Items
        CodeText = Generator.InfGenerateManifestItem(Item)
        Set Result = AddGeneratedManifestItem(Item, CodeText)
        If Result.IsFailure Then
            Set GenerateLayer = Result
            Exit Function
        End If
    Next Item

    Set GenerateLayer = ComCreateSuccess(LayerName & " layer generated.")
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set GenerateLayer = ComCreateFailureFromErr("AppGeneratorService." & OperationName, Desc, "Error", False)
End Function

Private Function GenerateBuildLayerManifest(ByVal LayerName As String) As ComResult
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim CodeText As String
    Dim Result As ComResult
    Dim OperationName As String

    On Error GoTo ErrHandler

    OperationName = "AppGenerate" & LayerName

    ComRequireText LayerName, "LayerName", OperationName

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "GeneratorService is not initialized."
    End If

    If GenerationEngine Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "BuildGenerationEngine is not initialized."
    End If

    If ManifestProvider Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "ManifestProvider is not initialized."
    End If

    Set Items = ManifestProvider.InfLoadManifestItems(ResolveBuildManifestPath(LayerName & ".manifest"))
    For Each Item In Items
        CodeText = Generator.InfGenerateManifestItem(Item)
        Set Result = AddGeneratedManifestItem(Item, CodeText)
        If Result.IsFailure Then
            Set GenerateBuildLayerManifest = Result
            Exit Function
        End If
    Next Item

    Set GenerateBuildLayerManifest = ComCreateSuccess(LayerName & " layer generated.")
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set GenerateBuildLayerManifest = ComCreateFailureFromErr("AppGeneratorService." & OperationName, Desc, "Error", False)
End Function

' Validates basic module name rules.
Private Function ModuleNameLikeValid(ByVal Name As String) As Boolean
    Dim i As Long
    Dim ch As String

    If ComIsBlankText(Name) Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    Name = Trim$(Name)

    ' First char must be a letter
    ch = Mid$(Name, 1, 1)
    If Not ch Like "[A-Za-z]" Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    ' Remaining chars must be letters, digits, or underscore
    For i = 1 To Len(Name)
        ch = Mid$(Name, i, 1)
        If Not (ch Like "[A-Za-z0-9_]") Then
            ModuleNameLikeValid = False
            Exit Function
        End If
    Next i

    ModuleNameLikeValid = True
End Function

Private Function ResolveLayerName(ByVal ModuleName As String) As String
    Select Case Left$(ModuleName, 3)
        Case "Pre"
            ResolveLayerName = "Presentation"
        Case "App"
            ResolveLayerName = "Application"
        Case "Dom"
            ResolveLayerName = "Domain"
        Case "Inf"
            ResolveLayerName = "Infrastructure"
        Case "Com"
            ResolveLayerName = "Common"
        Case Else
            ResolveLayerName = "Application"
    End Select
End Function

Private Function AddGeneratedManifestItem(ByVal Item As ManifestItem, ByVal CodeText As String) As ComResult
    Set AddGeneratedManifestItem = GenerationEngine.GenerateComponent( _
        Item.InfGetModuleName(), _
        Item.InfGetModuleType(), _
        CodeText)
End Function

Private Function ResolveBuildManifestPath(ByVal ManifestFileName As String) As String
    ComRequireText ManifestFileName, "ManifestFileName", "ResolveBuildManifestPath"

    ResolveBuildManifestPath = BuildPathResolver.CombinePath( _
        BuildPathResolver.CombinePath( _
            BuildPathResolver.CombinePath(BuildPathResolver.RepositoryRootPath(), "src"), _
            "Build"), _
        ManifestFileName)
End Function
