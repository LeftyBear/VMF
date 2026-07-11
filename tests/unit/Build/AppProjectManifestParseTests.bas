Option Explicit
Attribute VB_Name = "AppProjectManifestParseTests"

'=========================================================================
' Module: AppProjectManifestParseTests
' Layer: Application
' Responsibility: Verification tests for application manifest parsing.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9300

Public Sub AppRunProjectManifestParseTests()
    VerifyCurrentManifestParses
End Sub

Private Sub VerifyCurrentManifestParses()
    Dim Manifest As Build_ProjectManifest
    Dim Result As ComResult

    Set Manifest = New Build_ProjectManifest
    Set Result = Manifest.BuildInitializeFromContent(ReadCurrentManifestContent())

    AssertTrue Result.IsSuccess, "Current manifest.yaml should parse."
    AssertEquals "Source", Manifest.BuildGetSourceRoot(), "Source root should be derived from layer paths."
    AssertTrue Manifest.BuildHasLayer("Common"), "Common layer should be defined."
    AssertTrue Manifest.BuildHasLayer("Core"), "Core layer should be defined."
    AssertTrue Manifest.BuildIsGenerationEnabled("Core"), "Core generation flag should parse."
    AssertEquals "7", CStr(Manifest.BuildGetLayerItems("Domain").Count), "Domain entities should become class items."
    AssertEquals "4", CStr(Manifest.BuildGetLayerItems("Application").Count), "Application services should become class items."
    AssertEquals "6", CStr(Manifest.BuildGetLayerItems("Infrastructure").Count), "Infrastructure repositories should become class items."
    AssertEquals "4", CStr(Manifest.BuildGetLayerItems("Presentation").Count), "Presentation entries should become items."
End Sub

Private Function ReadCurrentManifestContent() As String
    Dim FileSystem As Object
    Dim TextFile As Object
    Dim ManifestPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ManifestPath = BuildPathResolver.ManifestFilePath("SchoolTimetable")

    Set TextFile = FileSystem.OpenTextFile(ManifestPath, 1, False)
    ReadCurrentManifestContent = TextFile.ReadAll
    TextFile.Close
End Function

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppProjectManifestParseTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As String, ByVal Actual As String, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppProjectManifestParseTests", Message & " Expected=" & Expected & " Actual=" & Actual
    End If
End Sub
