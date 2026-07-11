Option Explicit
Attribute VB_Name = "AppGenerateInfraPhase3_3Tests"

'=========================================================================
' Module: AppGenerateInfraPhase3_3Tests
' Layer: Application
' Responsibility: Verification tests for Phase 3-3 manifest-driven Infrastructure generation.
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9600
Private Const AppTestAssertEqualsErrorNumber As Long = vbObjectError + 9601

Public Sub AppRunGenerateInfrastructurePhase3_3Tests()
    VerifyInfrastructureManifestItems
    VerifyInfrastructureManifestItemGeneration
End Sub

Private Sub VerifyInfrastructureManifestItems()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveInfrastructureManifestPath())

    AssertTrue Items.Count > 0, "Infrastructure.manifest should contain generation items."

    For Each Item In Items
        AssertEquals "Infrastructure", Item.InfGetLayerName(), "ManifestItem should target the Infrastructure layer."
        AssertTrue Len(Item.InfGetModuleName()) > 0, "ManifestItem should expose ModuleName."
        AssertTrue Len(Item.InfGetModuleType()) > 0, "ManifestItem should expose ModuleType."
        AssertTrue Len(Item.InfGetTemplatePath()) > 0, "ManifestItem should expose TemplatePath."
    Next Item
End Sub

Private Sub VerifyInfrastructureManifestItemGeneration()
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim GeneratedCode As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Generator = InfCreateGenerator()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveInfrastructureManifestPath())
    Set Item = Items.Item(1)

    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue Len(GeneratedCode) > 0, "Generated Infrastructure code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated Infrastructure code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Layer: Infrastructure", vbTextCompare) > 0, "Generated Infrastructure code should include Infrastructure layer token replacement."
    AssertTrue InStr(1, GeneratedCode, Item.InfGetModuleName(), vbTextCompare) > 0, "Generated Infrastructure code should include the manifest module name."
End Sub

Private Function ResolveInfrastructureManifestPath() As String
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
        Err.Raise vbObjectError + 9602, "AppGenerateInfraPhase3_3Tests", "Manifest root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveInfrastructureManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "Infrastructure.manifest"
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
        Err.Raise AppTestAssertTrueErrorNumber, "AppGenerateInfraPhase3_3Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertEqualsErrorNumber, "AppGenerateInfraPhase3_3Tests", Message
    End If
End Sub
