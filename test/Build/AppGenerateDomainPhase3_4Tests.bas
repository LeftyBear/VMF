Option Explicit
Attribute VB_Name = "AppGenerateDomainPhase3_4Tests"

'=========================================================================
' Module: AppGenerateDomainPhase3_4Tests
' Layer: Application
' Responsibility: Verification tests for Phase 3-4 manifest-driven Domain generation.
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9700
Private Const AppTestAssertEqualsErrorNumber As Long = vbObjectError + 9701

Public Sub AppRunGenerateDomainPhase3_4Tests()
    VerifyDomainManifestItems
    VerifyDomainManifestItemGeneration
End Sub

Private Sub VerifyDomainManifestItems()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveDomainManifestPath())

    AssertTrue Items.Count > 0, "Domain.manifest should contain generation items."

    For Each Item In Items
        AssertEquals "Domain", Item.InfGetLayerName(), "ManifestItem should target the Domain layer."
        AssertTrue Len(Item.InfGetModuleName()) > 0, "ManifestItem should expose ModuleName."
        AssertTrue Len(Item.InfGetModuleType()) > 0, "ManifestItem should expose ModuleType."
        AssertTrue Len(Item.InfGetTemplatePath()) > 0, "ManifestItem should expose TemplatePath."
    Next Item
End Sub

Private Sub VerifyDomainManifestItemGeneration()
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim GeneratedCode As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Generator = InfCreateGenerator()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveDomainManifestPath())
    Set Item = Items.Item(1)

    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue Len(GeneratedCode) > 0, "Generated Domain code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated Domain code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Layer: Domain", vbTextCompare) > 0, "Generated Domain code should include Domain layer token replacement."
    AssertTrue InStr(1, GeneratedCode, Item.InfGetModuleName(), vbTextCompare) > 0, "Generated Domain code should include the manifest module name."
End Sub

Private Function ResolveDomainManifestPath() As String
    Dim AddinWorkbook As Object
    Dim RootPath As String

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
        Err.Raise vbObjectError + 9702, "AppGenerateDomainPhase3_4Tests", "Manifest root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveDomainManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "Domain.manifest"
End Function

Private Function ResolveWorkspaceRootPath(ByVal CandidatePath As String) As String
    Dim FileSystem As Object
    Dim ParentPath As String
    Dim GrandParentPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")

    If FileSystem.FolderExists(FileSystem.BuildPath(CandidatePath, "src\Build")) Then
        ResolveWorkspaceRootPath = CandidatePath
        Exit Function
    End If

    ParentPath = FileSystem.GetParentFolderName(CandidatePath)
    If Len(ParentPath) > 0 Then
        If FileSystem.FolderExists(FileSystem.BuildPath(ParentPath, "src\Build")) Then
            ResolveWorkspaceRootPath = ParentPath
            Exit Function
        End If
    End If

    GrandParentPath = FileSystem.GetParentFolderName(ParentPath)
    If Len(GrandParentPath) > 0 Then
        If FileSystem.FolderExists(FileSystem.BuildPath(GrandParentPath, "src\Build")) Then
            ResolveWorkspaceRootPath = GrandParentPath
            Exit Function
        End If
    End If

    ResolveWorkspaceRootPath = CandidatePath
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertTrueErrorNumber, "AppGenerateDomainPhase3_4Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertEqualsErrorNumber, "AppGenerateDomainPhase3_4Tests", Message
    End If
End Sub
