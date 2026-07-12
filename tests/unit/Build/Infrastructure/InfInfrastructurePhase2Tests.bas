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
    VerifyTemplateValidation
    VerifyTemplateDrivenGeneration
    VerifyBodyInsertionAndSectionContract
    VerifyCustomLayerManifestGeneration
    VerifyBuildManifestFormatCoverage
    VerifyManifestTemplatePathResolution
    VerifyMissingManifestTemplatePathResolution
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

Private Sub VerifyBodyInsertionAndSectionContract()
    Dim TemplateProvider As InfTemplateProvider
    Dim Replacer As InfTokenReplacer
    Dim Template As InfTemplate
    Dim Manifest As Object
    Dim GeneratedCode As String
    Dim TemplateText As String

    Set TemplateProvider = New InfTemplateProvider
    TemplateProvider.InfInitialize New InfFileSystemProvider
    Set Replacer = New InfTokenReplacer
    Set Manifest = CreateObject("Scripting.Dictionary")

    Manifest("ModuleName") = "ContractModule"
    Manifest("Layer") = "Application"
    Manifest("BODY") = "Public Sub Run()" & vbCrLf & "End Sub"
    TemplateText = "Option Explicit" & vbCrLf & _
        "' Module: {{ModuleName}}" & vbCrLf & _
        "' Layer: {{Layer}}" & vbCrLf & _
        "{{BODY}}"

    Set Template = TemplateProvider.InfCreateTemplateFromText("BodyTemplate", TemplateText)
    GeneratedCode = Replacer.InfReplaceTokens(Template, Manifest)

    AssertTrue InStr(1, GeneratedCode, "Public Sub Run()", vbTextCompare) > 0, "{{BODY}} should insert non-empty body content."
    AssertFalse InStr(1, GeneratedCode, "{{BODY}}", vbTextCompare) > 0, "{{BODY}} token should not remain in generated output."

    Manifest.Remove "BODY"
    TemplateText = "Option Explicit" & vbCrLf & _
        "' @section BODY" & vbCrLf & _
        "{{BODY}}" & vbCrLf & _
        "' @endsection" & vbCrLf & _
        "Private Const Marker As String = ""done"""

    Set Template = TemplateProvider.InfCreateTemplateFromText("SectionTemplate", TemplateText)
    GeneratedCode = Replacer.InfReplaceTokens(Template, Manifest)

    AssertFalse InStr(1, GeneratedCode, "@section", vbTextCompare) > 0, "Empty @section markers should not be generated."
    AssertFalse InStr(1, GeneratedCode, "{{BODY}}", vbTextCompare) > 0, "Empty @section body placeholder should not be generated."
    AssertTrue InStr(1, GeneratedCode, "Private Const Marker", vbTextCompare) > 0, "Content outside empty @section should remain."
End Sub

Private Sub VerifyCustomLayerManifestGeneration()
    Dim FileSystem As Object
    Dim FileProvider As InfFileSystemProvider
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim ManifestDir As String
    Dim ManifestPath As String
    Dim GeneratedCode As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ManifestDir = FileSystem.BuildPath(GetTestFolderPath(), "custom-layer")
    ManifestPath = FileSystem.BuildPath(ManifestDir, "Core.manifest")

    Set FileProvider = New InfFileSystemProvider
    FileProvider.InfWriteText ManifestPath, _
        "# ModuleName,ModuleType,LayerName,TemplatePath" & vbCrLf & _
        "CoreBootstrap,StandardModule,Core," & ResolveTemplatePath()

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ManifestPath)

    AssertEquals "1", CStr(Items.Count), "Custom Core manifest should load one item."

    Set Item = Items.Item(1)
    AssertEquals "Core", Item.InfGetLayerName(), "Custom manifest item should preserve the Core layer."
    AssertTrue Item.InfValidate(), "Custom Core manifest item should be valid."

    Set Generator = InfCreateGenerator()
    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue InStr(1, GeneratedCode, "Module: CoreBootstrap", vbTextCompare) > 0, "Custom layer generation should replace ModuleName."
    AssertTrue InStr(1, GeneratedCode, "Layer: Core", vbTextCompare) > 0, "Custom layer generation should replace the Core layer."
End Sub

Private Sub VerifyTemplateValidation()
    Dim TemplateProvider As InfTemplateProvider

    Set TemplateProvider = New InfTemplateProvider
    TemplateProvider.InfInitialize New InfFileSystemProvider

    AssertTrue TemplateProvider.InfValidateTemplateFile(ResolveTemplatePath()).IsSuccess, "ModuleTemplate.txt should pass template validation."
    AssertTrue TemplateProvider.InfValidateTemplateFile(ResolveTemplateFilePath("ClassTemplate.txt")).IsSuccess, "ClassTemplate.txt should pass template validation."
    AssertTrue TemplateProvider.InfValidateTemplateFile(ResolveTemplateFilePath("DomainClassTemplate.txt")).IsSuccess, "DomainClassTemplate.txt should pass template validation."
    AssertTrue TemplateProvider.InfValidateTemplateFile(ResolveTemplateFilePath("DomainModuleTemplate.txt")).IsSuccess, "DomainModuleTemplate.txt should pass template validation."
End Sub

Private Sub VerifyBuildManifestFormatCoverage()
    Dim FileSystem As Object
    Dim ManifestProvider As InfManifestProvider
    Dim TemplateProvider As InfTemplateProvider
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim ManifestNames As Variant
    Dim ManifestName As Variant
    Dim ManifestPath As String
    Dim ExpectedTemplatePath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    Set ManifestProvider = InfCreateManifestProvider()
    Set TemplateProvider = New InfTemplateProvider
    TemplateProvider.InfInitialize New InfFileSystemProvider
    ManifestNames = Array( _
        "Common.manifest", _
        "Manifest.manifest", _
        "Infrastructure.manifest", _
        "Domain.manifest", _
        "Application.manifest", _
        "Presentation.manifest")

    For Each ManifestName In ManifestNames
        ManifestPath = ResolveBuildManifestPath(CStr(ManifestName))
        AssertTrue ManifestProvider.InfValidateManifestFile(ManifestPath).IsSuccess, CStr(ManifestName) & " should pass manifest validation."

        Set Items = ManifestProvider.InfLoadManifestItems(ManifestPath)

        AssertTrue Items.Count > 0, CStr(ManifestName) & " should contain generation items."

        For Each Item In Items
            AssertTrue Item.InfValidate(), CStr(ManifestName) & " should contain valid manifest items."
            AssertTrue IsSupportedManifestModuleType(Item.InfGetModuleType()), CStr(ManifestName) & " should use a supported module type."
            AssertTrue IsSupportedManifestLayerName(Item.InfGetLayerName()), CStr(ManifestName) & " should use a supported layer name."
            AssertTrue Len(Item.InfGetModuleName()) > 0, CStr(ManifestName) & " should define ModuleName."
            AssertTrue Len(Item.InfGetTemplatePath()) > 0, CStr(ManifestName) & " should define TemplatePath."

            ExpectedTemplatePath = FileSystem.GetAbsolutePathName(Item.InfGetTemplatePath())
            AssertTrue FileSystem.FileExists(ExpectedTemplatePath), CStr(ManifestName) & " should reference an existing template."
            AssertTrue TemplateProvider.InfValidateTemplateFile(ExpectedTemplatePath).IsSuccess, CStr(ManifestName) & " should reference a valid template."
        Next Item
    Next ManifestName
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

Private Sub VerifyMissingManifestTemplatePathResolution()
    Dim FileSystem As Object
    Dim FileProvider As InfFileSystemProvider
    Dim Provider As InfManifestProvider
    Dim Items As Collection
    Dim ManifestDir As String
    Dim ManifestPath As String
    Dim MissingTemplatePath As String

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

    AssertEquals MissingTemplatePath, Items.Item(1).InfGetTemplatePath(), "Missing template path should still resolve from the manifest directory."
    AssertFalse FileSystem.FileExists(Items.Item(1).InfGetTemplatePath()), "Missing template should remain absent for InfCreateTemplate to report."
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

Private Function ResolveTemplateFilePath(ByVal TemplateFileName As String) As String
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
        Err.Raise vbObjectError + 9105, "InfInfrastructurePhase2Tests", "Template root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveTemplateFilePath = RootPath & Application.PathSeparator & "templates" & Application.PathSeparator & TemplateFileName
End Function

Private Function ResolveBuildManifestPath(ByVal ManifestFileName As String) As String
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
        Err.Raise vbObjectError + 9104, "InfInfrastructurePhase2Tests", "Manifest root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveBuildManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & ManifestFileName
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

Private Function IsSupportedManifestModuleType(ByVal ModuleType As String) As Boolean
    Select Case ModuleType
        Case "StandardModule", "ClassModule"
            IsSupportedManifestModuleType = True
        Case Else
            IsSupportedManifestModuleType = False
    End Select
End Function

Private Function IsSupportedManifestLayerName(ByVal LayerName As String) As Boolean
    Dim Position As Long
    Dim Character As String

    If ComIsBlankText(LayerName) Then
        IsSupportedManifestLayerName = False
        Exit Function
    End If

    Character = Mid$(LayerName, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        IsSupportedManifestLayerName = False
        Exit Function
    End If

    For Position = 1 To Len(LayerName)
        Character = Mid$(LayerName, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            IsSupportedManifestLayerName = False
            Exit Function
        End If
    Next Position

    IsSupportedManifestLayerName = True
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
