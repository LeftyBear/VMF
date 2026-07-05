Option Explicit
Attribute VB_Name = "AppGenerateAppPhase3_5Tests"

'=========================================================================
' Module: AppGenerateAppPhase3_5Tests
' Layer: Application
' Responsibility: Verification tests for Phase 3-5 manifest-driven Application generation.
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9800
Private Const AppTestAssertEqualsErrorNumber As Long = vbObjectError + 9801

Public Sub AppRunGenerateApplicationPhase3_5Tests()
    VerifyApplicationManifestItems
    VerifyApplicationManifestItemGeneration
End Sub

Private Sub VerifyApplicationManifestItems()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveApplicationManifestPath())

    AssertTrue Items.Count > 0, "Application.manifest should contain generation items."

    For Each Item In Items
        AssertEquals "Application", Item.InfGetLayerName(), "ManifestItem should target the Application layer."
        AssertTrue Len(Item.InfGetModuleName()) > 0, "ManifestItem should expose ModuleName."
        AssertTrue Len(Item.InfGetModuleType()) > 0, "ManifestItem should expose ModuleType."
        AssertTrue Len(Item.InfGetTemplatePath()) > 0, "ManifestItem should expose TemplatePath."
    Next Item
End Sub

Private Sub VerifyApplicationManifestItemGeneration()
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim GeneratedCode As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Generator = InfCreateGenerator()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolveApplicationManifestPath())
    Set Item = Items.Item(1)

    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue Len(GeneratedCode) > 0, "Generated Application code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated Application code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Layer: Application", vbTextCompare) > 0, "Generated Application code should include Application layer token replacement."
    AssertTrue InStr(1, GeneratedCode, Item.InfGetModuleName(), vbTextCompare) > 0, "Generated Application code should include the manifest module name."
End Sub

Private Function ResolveApplicationManifestPath() As String
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
        Err.Raise vbObjectError + 9802, "AppGenerateAppPhase3_5Tests", "Manifest root path is unavailable."
    End If

    ResolveApplicationManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "Application.manifest"
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertTrueErrorNumber, "AppGenerateAppPhase3_5Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertEqualsErrorNumber, "AppGenerateAppPhase3_5Tests", Message
    End If
End Sub
