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
Private VbaProjectProvider As InfVbaProjectProvider
Private ManifestProvider As InfManifestProvider

' Initializes the service with required generation infrastructure.
Public Sub AppInitialize( _
    ByVal ModuleGenerator As InfGenerator, _
    ByVal ProjectProvider As InfVbaProjectProvider, _
    ByVal ProviderManifest As InfManifestProvider)

    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ModuleGenerator is required."
    End If

    If ProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ProjectProvider is required."
    End If

    If ProviderManifest Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ManifestProvider is required."
    End If

    Set Generator = ModuleGenerator
    Set VbaProjectProvider = ProjectProvider
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

    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "VbaProjectProvider is not initialized."
    End If

    LayerName = ResolveLayerName(ModuleName)
    CodeText = Generator.InfGenerateStandardModule(ModuleName, LayerName)
    Set AppGenerateModule = VbaProjectProvider.InfAddStandardModule(ModuleName, CodeText)
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

    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateClass", "VbaProjectProvider is not initialized."
    End If

    LayerName = ResolveLayerName(ClassName)
    CodeText = Generator.InfGenerateStandardClass(ClassName, LayerName)
    Set AppGenerateClass = VbaProjectProvider.InfAddClassModule(ClassName, CodeText)
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
    Set AppGenerateCommon = GenerateLayer("Common")
End Function

' Generates the Manifest layer from Manifest.manifest.
Public Function AppGenerateManifest() As ComResult
    Set AppGenerateManifest = GenerateLayer("Manifest")
End Function

' Generates the Infrastructure layer from Infrastructure.manifest.
Public Function AppGenerateInfrastructure() As ComResult
    Set AppGenerateInfrastructure = GenerateLayer("Infrastructure")
End Function

' Generates the Domain layer from Domain.manifest.
Public Function AppGenerateDomain() As ComResult
    Set AppGenerateDomain = GenerateLayer("Domain")
End Function

' Generates the Application layer from Application.manifest.
Public Function AppGenerateApplication() As ComResult
    Set AppGenerateApplication = GenerateLayer("Application")
End Function

' Generates the Presentation layer from Presentation.manifest.
Public Function AppGeneratePresentation() As ComResult
    Set AppGeneratePresentation = GenerateLayer("Presentation")
End Function

' Generates all Build layers in canonical dependency order.
Public Function AppGenerateProject() As ComResult
    Set AppGenerateProject = GenerateProject()
End Function

Private Function GenerateProject() As ComResult
    Dim Result As ComResult

    Set Result = GenerateLayer("Common")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Manifest")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Infrastructure")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Domain")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Application")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set Result = GenerateLayer("Presentation")
    If Result.IsFailure Then
        Set GenerateProject = Result
        Exit Function
    End If

    Set GenerateProject = ComCreateSuccess("Project generated.")
End Function

Private Function GenerateLayer(ByVal LayerName As String) As ComResult
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

    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "VbaProjectProvider is not initialized."
    End If

    If ManifestProvider Is Nothing Then
        Err.Raise ComErrInvalidState, OperationName, "ManifestProvider is not initialized."
    End If

    Set Items = ManifestProvider.InfLoadManifestItems(ResolveBuildManifestPath(LayerName & ".manifest"))
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
    Select Case Item.InfGetModuleType()
        Case "StandardModule"
            Set AddGeneratedManifestItem = VbaProjectProvider.InfAddStandardModule(Item.InfGetModuleName(), CodeText)
        Case "ClassModule"
            Set AddGeneratedManifestItem = VbaProjectProvider.InfAddClassModule(Item.InfGetModuleName(), CodeText)
        Case Else
            Err.Raise ComErrInvalidArgument, "AddGeneratedManifestItem", "ModuleType is not supported."
    End Select
End Function

Private Function ResolveBuildManifestPath(ByVal ManifestFileName As String) As String
    Dim AddinWorkbook As Object
    Dim RootPath As String

    ComRequireText ManifestFileName, "ManifestFileName", "ResolveBuildManifestPath"

    RootPath = vbNullString

    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    On Error GoTo 0

    If Not AddinWorkbook Is Nothing Then
        RootPath = AddinWorkbook.Path
    Else
        RootPath = ThisWorkbook.Path
    End If

    If ComIsBlankText(RootPath) Then
        Err.Raise ComErrInvalidState, "ResolveBuildManifestPath", "Manifest root path is unavailable."
    End If

    ResolveBuildManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & ManifestFileName
End Function
