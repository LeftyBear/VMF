Option Explicit
Attribute VB_Name = "InfInfrastructurePhase2Tests"

'=========================================================================
' Module: InfInfrastructurePhase2Tests
' Layer: Infrastructure
' Responsibility: Focused verification tests for Phase 2 Infrastructure contracts.
' Dependencies: Common, Infrastructure
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const InfTestFolderName As String = "VMF_InfPhase2Tests"
Private Const InfTestFileName As String = "sample.txt"
Private Const InfTestContent As String = "Infrastructure phase 2"
Private Const InfTestAssertTrueErrorNumber As Long = vbObjectError + 9100
Private Const InfTestAssertFalseErrorNumber As Long = vbObjectError + 9101
Private Const InfTestAssertEqualsErrorNumber As Long = vbObjectError + 9102

'=========================================================================
' Public API
'=========================================================================

' Runs the Phase 2 Infrastructure verification tests.
'
' Parameters:
'   None.
'
' Raised errors:
'   Raises a test failure error when an assertion fails.
Public Sub InfRunInfrastructurePhase2Tests()
    VerifyInitialize
    VerifyFileSystemOperations
    VerifyTemplateDrivenGeneration
    VerifyManifestTemplatePathResolution
    VerifyMissingManifestTemplateUsesTemplateProviderError
    VerifyVbaProjectProviderSkipsBuildWorkbook
End Sub

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub VerifyInitialize()
    AssertTrue InfInitialize(), "InfInitialize should return True."
    InfShutdown
End Sub

Private Sub VerifyFileSystemOperations()
    Dim FolderPath As String
    Dim FilePath As String
    Dim WriteResult As ComResult

    FolderPath = GetTestFolderPath()
    FilePath = FolderPath & "\" & InfTestFileName

    Set WriteResult = InfWriteText(FilePath, InfTestContent)

    AssertTrue WriteResult.IsSuccess, "InfWriteText should return success."
    AssertTrue InfFolderExists(FolderPath), "Test folder should exist."
    AssertTrue InfFileExists(FilePath), "Test file should exist."
    AssertEquals InfTestContent, InfReadText(FilePath), "Text content should round-trip."
End Sub

Private Sub VerifyTemplateDrivenGeneration()
    Dim Generator As InfGenerator
    Dim CompositionRoot As InfCompositionRoot
    Dim TemplatePath As String
    Dim GeneratedCode As String

    Set CompositionRoot = New InfCompositionRoot
    Set Generator = CompositionRoot.InfCreateGenerator()

    TemplatePath = ResolveTemplatePath()
    GeneratedCode = Generator.InfGenerateModule("VMF_TestTemplate", TemplatePath)

    AssertTrue Len(GeneratedCode) > 0, "Generated code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Module: VMF_TestTemplate", vbTextCompare) > 0, "Generated code should include the module header."
    AssertTrue InStr(1, GeneratedCode, "Layer: Application", vbTextCompare) > 0, "Generated code should include the canonical layer token replacement result."
End Sub

Private Sub VerifyManifestTemplatePathResolution()
    Dim FileSystem As Object
    Dim Provider As InfManifestProvider
    Dim FileProvider As InfFileSystemProvider
    Dim Items As Collection
    Dim ManifestDir As String
    Dim ManifestPath As String
    Dim RelativeTemplatePath As String
    Dim AbsoluteTemplatePath As String
    Dim SlashTemplatePath As String
    Dim ManifestText As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ManifestDir = FileSystem.BuildPath(GetTestFolderPath(), "manifest-paths\manifests")
    ManifestPath = FileSystem.BuildPath(ManifestDir, "templates.manifest")
    RelativeTemplatePath = FileSystem.GetAbsolutePathName(FileSystem.BuildPath(ManifestDir, "templates\RelativeTemplate.txt"))
    AbsoluteTemplatePath = FileSystem.GetAbsolutePathName(FileSystem.BuildPath(GetTestFolderPath(), "absolute\AbsoluteTemplate.txt"))
    SlashTemplatePath = FileSystem.GetAbsolutePathName(FileSystem.BuildPath(ManifestDir, "templates\SlashTemplate.txt"))

    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText RelativeTemplatePath, "relative"
    FileProvider.InfWriteText AbsoluteTemplatePath, "absolute"
    FileProvider.InfWriteText SlashTemplatePath, "slash"

    ManifestText = "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "RelativeModule,StandardModule,Application,templates\RelativeTemplate.txt" & vbCrLf & _
        "AbsoluteModule,StandardModule,Application," & AbsoluteTemplatePath & vbCrLf & _
        "SlashModule,StandardModule,Application,templates/SlashTemplate.txt"
    FileProvider.InfWriteText ManifestPath, ManifestText

    Set Provider = InfCreateManifestProvider()
    Set Items = Provider.InfLoadManifestItems(ManifestPath)

    AssertEquals RelativeTemplatePath, Items.Item(1).InfGetTemplatePath(), "Relative TemplatePath should resolve from the manifest directory."
    AssertEquals AbsoluteTemplatePath, Items.Item(2).InfGetTemplatePath(), "Absolute TemplatePath should be preserved."
    AssertEquals SlashTemplatePath, Items.Item(3).InfGetTemplatePath(), "Forward slash TemplatePath should resolve from the manifest directory."
End Sub

Private Sub VerifyMissingManifestTemplateUsesTemplateProviderError()
    Dim FileSystem As Object
    Dim FileProvider As InfFileSystemProvider
    Dim Provider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim ManifestDir As String
    Dim ManifestPath As String
    Dim MissingTemplatePath As String
    Dim GeneratedCode As String
    Dim ErrorSource As String
    Dim ErrorDescription As String
    Dim ErrorNumber As Long

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ManifestDir = FileSystem.BuildPath(GetTestFolderPath(), "missing-template")
    ManifestPath = FileSystem.BuildPath(ManifestDir, "missing.manifest")
    MissingTemplatePath = FileSystem.GetAbsolutePathName(FileSystem.BuildPath(ManifestDir, "templates\MissingTemplate.txt"))

    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText ManifestPath, _
        "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "MissingModule,StandardModule,Application,templates\MissingTemplate.txt"

    Set Provider = InfCreateManifestProvider()
    Set Items = Provider.InfLoadManifestItems(ManifestPath)
    Set Generator = InfCreateGenerator()

    On Error Resume Next
    GeneratedCode = Generator.InfGenerateManifestItem(Items.Item(1))
    ErrorNumber = Err.Number
    ErrorSource = Err.Source
    ErrorDescription = Err.Description
    Err.Clear
    On Error GoTo 0

    AssertTrue ErrorNumber <> 0, "Missing template should raise the existing InfCreateTemplate error."
    AssertEquals "InfCreateTemplate", ErrorSource, "Missing template error should come from InfCreateTemplate."
    AssertTrue InStr(1, ErrorDescription, "TemplatePath does not exist:", vbTextCompare) > 0, "Missing template error should keep the existing message."
    AssertTrue InStr(1, ErrorDescription, MissingTemplatePath, vbTextCompare) > 0, "Missing template error should include the resolved absolute path."
End Sub

Private Sub VerifyVbaProjectProviderSkipsBuildWorkbook()
    Dim AddinWorkbook As Object
    Dim Provider As InfVbaProjectProvider
    Dim ModuleName As String
    Dim Result As ComResult
    Dim Text As String

    Set AddinWorkbook = Nothing
    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    If Not AddinWorkbook Is Nothing Then
        AddinWorkbook.Activate
    End If
    On Error GoTo 0

    ModuleName = "VMF_Test_TargetProjectPatch"
    Set Provider = InfCreateVbaProjectProvider()

    Provider.InfRemoveModule ModuleName
    Set Result = Provider.InfAddStandardModule( _
        ModuleName, _
        "Option Explicit" & vbCrLf & vbCrLf & _
        "'=========================================================================" & vbCrLf & _
        "' Module: " & ModuleName & vbCrLf & _
        "' Layer: Infrastructure" & vbCrLf & _
        "' Responsibility:" & vbCrLf & _
        "'=========================================================================" & vbCrLf)

    AssertTrue Result.IsSuccess, "Provider should generate into the external target workbook when Build.xlam is active."
    AssertTrue Provider.InfModuleExists(ModuleName), "Generated module should exist in the target workbook."
    AssertFalse ModuleExistsInWorkbook(AddinWorkbook, ModuleName), "Generated module should not be added to Build.xlam."

    Text = Provider.InfGetModuleText(ModuleName)
    AssertEquals 1, CountOptionExplicit(Text), "Generated module should contain exactly one Option Explicit."

    Provider.InfRemoveModule ModuleName
End Sub

Private Function ResolveTemplatePath() As String
    Dim AddinWorkbook As Object
    Dim RootPath As String

    On Error Resume Next
    Set AddinWorkbook = Application.Workbooks("Build.xlam")
    On Error GoTo 0

    If Not AddinWorkbook Is Nothing Then
        RootPath = AddinWorkbook.Path
    Else
        RootPath = ThisWorkbook.Path
    End If

    If ComIsBlankText(RootPath) Then
        Err.Raise vbObjectError + 9103, "InfInfrastructurePhase2Tests", "Template root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveTemplatePath = RootPath & Application.PathSeparator & "templates" & Application.PathSeparator & "ModuleTemplate.txt"
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

Private Function GetTestFolderPath() As String
    GetTestFolderPath = Environ$("TEMP") & "\" & InfTestFolderName
End Function

Private Function ModuleExistsInWorkbook(ByVal Workbook As Object, ByVal ModuleName As String) As Boolean
    Dim Comp As Object

    If Workbook Is Nothing Then
        ModuleExistsInWorkbook = False
        Exit Function
    End If

    On Error Resume Next
    For Each Comp In Workbook.VBProject.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            ModuleExistsInWorkbook = True
            Exit Function
        End If
    Next Comp
    On Error GoTo 0

    ModuleExistsInWorkbook = False
End Function

Private Function CountOptionExplicit(ByVal Text As String) As Long
    Dim Lines() As String
    Dim NormalizedText As String
    Dim i As Long

    NormalizedText = Replace(Text, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)
    Lines = Split(NormalizedText, vbLf)

    For i = LBound(Lines) To UBound(Lines)
        If StrComp(Trim$(Lines(i)), "Option Explicit", vbTextCompare) = 0 Then
            CountOptionExplicit = CountOptionExplicit + 1
        End If
    Next i
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise InfTestAssertTrueErrorNumber, "InfInfrastructurePhase2Tests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise InfTestAssertFalseErrorNumber, "InfInfrastructurePhase2Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise InfTestAssertEqualsErrorNumber, "InfInfrastructurePhase2Tests", Message
    End If
End Sub
