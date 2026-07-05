Option Explicit
Attribute VB_Name = "AppGeneratePrePhase3_6Tests"

'=========================================================================
' Module: AppGeneratePrePhase3_6Tests
' Layer: Application
' Responsibility: Verification tests for Phase 3-6 manifest-driven Presentation generation.
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Private Const AppTestAssertTrueErrorNumber As Long = vbObjectError + 9900
Private Const AppTestAssertEqualsErrorNumber As Long = vbObjectError + 9901

Public Sub AppRunGeneratePresentationPhase3_6Tests()
    VerifyPresentationManifestItems
    VerifyPresentationManifestItemGeneration
End Sub

Private Sub VerifyPresentationManifestItems()
    Dim ManifestProvider As InfManifestProvider
    Dim Items As Collection
    Dim Item As ManifestItem

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolvePresentationManifestPath())

    AssertTrue Items.Count > 0, "Presentation.manifest should contain generation items."

    For Each Item In Items
        AssertEquals "Presentation", Item.InfGetLayerName(), "ManifestItem should target the Presentation layer."
        AssertTrue Len(Item.InfGetModuleName()) > 0, "ManifestItem should expose ModuleName."
        AssertTrue Len(Item.InfGetModuleType()) > 0, "ManifestItem should expose ModuleType."
        AssertTrue Len(Item.InfGetTemplatePath()) > 0, "ManifestItem should expose TemplatePath."
    Next Item
End Sub

Private Sub VerifyPresentationManifestItemGeneration()
    Dim ManifestProvider As InfManifestProvider
    Dim Generator As InfGenerator
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim GeneratedCode As String

    Set ManifestProvider = InfCreateManifestProvider()
    Set Generator = InfCreateGenerator()
    Set Items = ManifestProvider.InfLoadManifestItems(ResolvePresentationManifestPath())
    Set Item = Items.Item(1)

    GeneratedCode = Generator.InfGenerateManifestItem(Item)

    AssertTrue Len(GeneratedCode) > 0, "Generated Presentation code should not be empty."
    AssertTrue InStr(1, GeneratedCode, "Option Explicit", vbTextCompare) > 0, "Generated Presentation code should include Option Explicit."
    AssertTrue InStr(1, GeneratedCode, "Layer: Presentation", vbTextCompare) > 0, "Generated Presentation code should include Presentation layer token replacement."
    AssertTrue InStr(1, GeneratedCode, Item.InfGetModuleName(), vbTextCompare) > 0, "Generated Presentation code should include the manifest module name."
End Sub

Private Function ResolvePresentationManifestPath() As String
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
        Err.Raise vbObjectError + 9902, "AppGeneratePrePhase3_6Tests", "Manifest root path is unavailable."
    End If

    ResolvePresentationManifestPath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "Presentation.manifest"
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertTrueErrorNumber, "AppGeneratePrePhase3_6Tests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertEqualsErrorNumber, "AppGeneratePrePhase3_6Tests", Message
    End If
End Sub
