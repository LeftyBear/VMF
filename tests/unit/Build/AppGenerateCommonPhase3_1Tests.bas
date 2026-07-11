Option Explicit
Attribute VB_Name = "AppGenerateCommonPhase3_1Tests"

'=========================================================================
' Module: AppGenerateCommonPhase3_1Tests
' Layer: Application
' Responsibility: Verification tests for Phase 3-1 manifest-driven Common generation.
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9400
Private Const AppTestAssertEqualsErrorNumber As Long = vbObjectError + 9401

Public Sub AppRunGenerateCommonPhase3_1Tests()
    VerifyCommonManifestItems
    VerifyCommonManifestItemGeneration
    VerifyCommonManifestPreview
End Sub

Private Sub VerifyCommonManifestItems()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveCommonManifestPath())

    AssertTrue Items.Count > 0, "Common.manifest should contain generation items."

    For Each Item In Items
        AssertEquals "Common", Item.InfGetLayerName(), "ManifestItem should target the Common layer."
        AssertTrue Len(Item.InfGetModuleName()) > 0, "ManifestItem should expose ModuleName."
        AssertTrue Len(Item.InfGetTemplatePath()) > 0, "ManifestItem should expose TemplatePath."
    Next Item
End Sub

Private Sub VerifyCommonManifestItemGeneration()
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim GeneratedCode As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Generator = InfCreateGenerator()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveCommonManifestPath())
    Set Item = Items.Item(1)

    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue Len(GeneratedCode) > 0, "Generated Common code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated Common code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Layer: Common", vbTextCompare) > 0, "Generated Common code should include Common layer token replacement."
    AssertTrue InStr(1, GeneratedCode, Item.InfGetModuleName(), vbTextCompare) > 0, "Generated Common code should include the manifest module name."
End Sub

Private Sub VerifyCommonManifestPreview()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim PreviewText As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveCommonManifestPath())
    Set Item = Items.Item(1)

    PreviewText = AppPreviewBuildLayer("Common")

    AssertTrue Len(PreviewText) > 0, "Common preview should not be empty."
    AssertTrue InStr(1, PreviewText, "# Preview: " & Item.InfGetModuleName(), vbTextCompare) > 0, "Common preview should identify the previewed module."
    AssertTrue InStr(1, PreviewText, "Layer: Common", vbTextCompare) > 0, "Common preview should include generated Common source."
    AssertTrue InStr(1, PreviewText, "Module created:", vbTextCompare) = 0, "Common preview should not return a mutation result message."
End Sub

Private Function ResolveCommonManifestPath() As String
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
        Err.Raise vbObjectError + 9402, "AppGenerateCommonPhase3_1Tests", "Manifest root path is unavailable."
    End If

    RootPath = ResolveWorkspaceRootPath(RootPath)
    ResolveCommonManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "Common.manifest"
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
        Err.Raise AppTestAssertTrueErrorNumber, "AppGenerateCommonPhase3_1Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertEqualsErrorNumber, "AppGenerateCommonPhase3_1Tests", Message
    End If
End Sub
