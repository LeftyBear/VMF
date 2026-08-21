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
    VerifyBlueprintParserCreatesGenerationMetadata
    VerifyBlueprintParserGeneratesManifestContent
    VerifyBlueprintValidationFailureStopsManifestGeneration
    VerifyParserFailureDoesNotRunBlueprintValidation
    VerifyBlueprintParserRejectsEmptyLayerManifest
End Sub

Private Sub VerifyCurrentManifestParses()
    Dim Manifest As Build_ProjectManifest
    Dim Result As ComResult
    Dim DomainItems As Collection

    Set Manifest = New Build_ProjectManifest
    Set Result = Manifest.BuildInitializeFromContent(ReadCurrentManifestContent())

    AssertTrue Result.IsSuccess, "Current manifest.yaml should parse."
    AssertEquals "src", Manifest.BuildGetSourceRoot(), "Source root should be derived from layer paths."
    AssertTrue Manifest.BuildHasLayer("Common"), "Common layer should be defined."
    AssertTrue Manifest.BuildHasLayer("Core"), "Core layer should be defined."
    AssertTrue Manifest.BuildIsGenerationEnabled("Core"), "Core generation flag should parse."
    AssertEquals "7", CStr(Manifest.BuildGetLayerItems("Domain").Count), "Domain entities should become class items."
    AssertEquals "4", CStr(Manifest.BuildGetLayerItems("Application").Count), "Application services should become class items."
    AssertEquals "6", CStr(Manifest.BuildGetLayerItems("Infrastructure").Count), "Infrastructure repositories should become class items."
    AssertEquals "4", CStr(Manifest.BuildGetLayerItems("Presentation").Count), "Presentation entries should become items."

    Set DomainItems = Manifest.BuildGetLayerItems("Domain")
    AssertTrue IsAbsolutePath(DomainItems.Item(1).InfGetTemplatePath()), "Project manifest template path should be absolute."
    AssertTrue CreateObject("Scripting.FileSystemObject").FileExists(DomainItems.Item(1).InfGetTemplatePath()), "Project manifest template path should exist."
End Sub

Private Sub VerifyBlueprintParserCreatesGenerationMetadata()
    Dim Parser As Build_BlueprintParser
    Dim Result As ComResult
    Dim CoreItems As Collection
    Dim DomainItems As Collection
    Dim Item As ManifestItem

    Set Parser = New Build_BlueprintParser
    Set Result = Parser.BuildInitializeFromContent(CreateBlueprintContent())

    AssertTrue Result.IsSuccess, "Blueprint parser should parse valid blueprint content."
    AssertEquals "SchoolTimetable", Parser.BuildGetBlueprintValue("name"), "Blueprint name should parse."
    AssertEquals "0.1", Parser.BuildGetBlueprintValue("version"), "Blueprint version should parse."
    AssertTrue Parser.BuildHasLayer("Core"), "Blueprint parser should register custom Core layer."
    AssertTrue Parser.BuildHasLayer("Domain"), "Blueprint parser should register Domain layer."

    Set CoreItems = Parser.BuildGetLayerItems("Core")
    Set DomainItems = Parser.BuildGetLayerItems("Domain")

    AssertEquals "1", CStr(CoreItems.Count), "Core blueprint modules should become generation metadata."
    AssertEquals "2", CStr(DomainItems.Count), "Domain blueprint classes should become generation metadata."

    Set Item = CoreItems.Item(1)
    AssertEquals "CoreBootstrap", Item.InfGetModuleName(), "Core module name should parse."
    AssertEquals "StandardModule", Item.InfGetModuleType(), "Core module type should parse."
    AssertEquals "Core", Item.InfGetLayerName(), "Core layer should be preserved."
    AssertTrue CreateObject("Scripting.FileSystemObject").FileExists(Item.InfGetTemplatePath()), "Core template path should exist."

    Set Item = DomainItems.Item(1)
    AssertEquals "Subject", Item.InfGetModuleName(), "Domain class name should parse."
    AssertEquals "ClassModule", Item.InfGetModuleType(), "Domain class type should parse."
    AssertEquals "Domain", Item.InfGetLayerName(), "Domain layer should be preserved."
End Sub

Private Sub VerifyBlueprintParserGeneratesManifestContent()
    Dim Parser As Build_BlueprintParser
    Dim Result As ComResult
    Dim ManifestContent As String
    Dim FileSystem As Object
    Dim FileProvider As InfFileSystemProvider
    Dim ManifestProvider As InfManifestProvider
    Dim ManifestPath As String
    Dim Items As Collection
    Dim Item As ManifestItem

    Set Parser = New Build_BlueprintParser
    Set Result = Parser.BuildInitializeFromContent(CreateBlueprintContent())

    AssertTrue Result.IsSuccess, "Blueprint parser should parse before manifest generation."

    ManifestContent = Parser.BuildGenerateManifestContent("Domain")
    AssertTrue InStr(1, ManifestContent, "# ModuleName,ModuleType,LayerName,TemplatePath", vbTextCompare) > 0, "Generated manifest should include the manifest header."
    AssertTrue InStr(1, ManifestContent, "Subject,ClassModule,Domain,", vbTextCompare) > 0, "Generated manifest should include the Subject class entry."
    AssertTrue InStr(1, ManifestContent, "Teacher,ClassModule,Domain,", vbTextCompare) > 0, "Generated manifest should include the Teacher class entry."

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    ManifestPath = FileSystem.BuildPath(GetTestFolderPath(), "Domain.manifest")

    Set FileProvider = New InfFileSystemProvider
    Set Result = FileProvider.InfWriteText(ManifestPath, ManifestContent)
    AssertTrue Result.IsSuccess, "Generated manifest content should be writable."

    Set ManifestProvider = InfCreateManifestProvider()
    Set Items = ManifestProvider.InfLoadManifestItems(ManifestPath)

    AssertEquals "2", CStr(Items.Count), "Generated Domain manifest should round-trip through InfManifestProvider."

    Set Item = Items.Item(1)
    AssertEquals "Subject", Item.InfGetModuleName(), "Generated manifest should preserve module name."
    AssertEquals "ClassModule", Item.InfGetModuleType(), "Generated manifest should preserve module type."
    AssertEquals "Domain", Item.InfGetLayerName(), "Generated manifest should preserve layer."
    AssertTrue FileSystem.FileExists(Item.InfGetTemplatePath()), "Generated manifest should preserve an existing template path."
End Sub

Private Sub VerifyBlueprintValidationFailureStopsManifestGeneration()
    Dim Parser As Build_BlueprintParser
    Dim Result As ComResult
    Dim ManifestContent As String
    Dim ErrorText As String

    Set Parser = New Build_BlueprintParser
    Set Result = Parser.BuildInitializeFromContent(CreateBlueprintContentWithInvalidValidationVersion())

    AssertTrue Result.IsSuccess, "Parser should succeed before validation integration runs."
    ManifestContent = TryGenerateManifestContent(Parser, "Domain", ErrorText)

    AssertEquals vbNullString, ManifestContent, "Validation failure should not produce manifest content."
    AssertTrue InStr(1, ErrorText, "Blueprint validation failed before Manifest generation.", vbTextCompare) > 0, "Validation failure should hard stop before manifest generation."
    AssertTrue InStr(1, ErrorText, "ResultKind=invalid", vbTextCompare) > 0, "Validation failure should preserve ResultKind."
    AssertTrue InStr(1, ErrorText, "Generatable=False", vbTextCompare) > 0, "Validation failure should preserve Generatable."
    AssertTrue InStr(1, ErrorText, "Code=BP101", vbTextCompare) > 0, "Validation failure should preserve diagnostic code."
    AssertTrue InStr(1, ErrorText, "Category=UnsupportedEnumValue", vbTextCompare) > 0, "Validation failure should preserve diagnostic category."
    AssertTrue InStr(1, ErrorText, "Severity=error", vbTextCompare) > 0, "Validation failure should preserve diagnostic severity."
    AssertTrue InStr(1, ErrorText, "FieldPath=version", vbTextCompare) > 0, "Validation failure should preserve diagnostic field path."
    AssertTrue InStr(1, ErrorText, "Message=", vbTextCompare) > 0, "Validation failure should report an empty optional message safely."
End Sub

Private Sub VerifyParserFailureDoesNotRunBlueprintValidation()
    Dim Parser As Build_BlueprintParser
    Dim Result As ComResult
    Dim ManifestContent As String
    Dim ErrorText As String

    Set Parser = New Build_BlueprintParser
    Set Result = Parser.BuildInitializeFromContent(CreateBlueprintContentWithParseError())

    AssertTrue Result.IsFailure, "Parser failure should stop before validation."
    ManifestContent = TryGenerateManifestContent(Parser, "Domain", ErrorText)

    AssertEquals vbNullString, ManifestContent, "Parser failure should not produce manifest content."
    AssertTrue InStr(1, ErrorText, "not initialized", vbTextCompare) > 0, "Parser failure should remain a parser initialization stop instead of validation failure."
    AssertTrue InStr(1, ErrorText, "Blueprint validation failed", vbTextCompare) = 0, "Parser failure should not be converted to validation failure."
End Sub

Private Sub VerifyBlueprintParserRejectsEmptyLayerManifest()
    Dim Parser As Build_BlueprintParser
    Dim Result As ComResult

    Set Parser = New Build_BlueprintParser
    Set Result = Parser.BuildInitializeFromContent(CreateBlueprintContentWithEmptyLayer())

    AssertTrue Result.IsFailure, "Empty generation layer should not parse as a valid blueprint."
End Sub

Private Function CreateBlueprintContent() As String
    CreateBlueprintContent = _
        "blueprint:" & vbCrLf & _
        "  name: SchoolTimetable" & vbCrLf & _
        "  version: 0.1" & vbCrLf & _
        "layers:" & vbCrLf & _
        "  - Core" & vbCrLf & _
        "  - Domain" & vbCrLf & _
        "modules:" & vbCrLf & _
        "  Core:" & vbCrLf & _
        "    StandardModules:" & vbCrLf & _
        "      - CoreBootstrap" & vbCrLf & _
        "  Domain:" & vbCrLf & _
        "    Classes:" & vbCrLf & _
        "      - Subject" & vbCrLf & _
        "      - Teacher"
End Function

Private Function CreateBlueprintContentWithInvalidValidationVersion() As String
    CreateBlueprintContentWithInvalidValidationVersion = _
        "blueprint:" & vbCrLf & _
        "  name: SchoolTimetable" & vbCrLf & _
        "  version: 9.9" & vbCrLf & _
        "layers:" & vbCrLf & _
        "  - Domain" & vbCrLf & _
        "modules:" & vbCrLf & _
        "  Domain:" & vbCrLf & _
        "    Classes:" & vbCrLf & _
        "      - Subject"
End Function

Private Function CreateBlueprintContentWithParseError() As String
    CreateBlueprintContentWithParseError = _
        "blueprint:" & vbCrLf & _
        "  name: SchoolTimetable" & vbCrLf & _
        "  version: 0.1" & vbCrLf & _
        "layers:" & vbCrLf & _
        "  - Domain"
End Function

Private Function CreateBlueprintContentWithEmptyLayer() As String
    CreateBlueprintContentWithEmptyLayer = _
        "blueprint:" & vbCrLf & _
        "  name: SchoolTimetable" & vbCrLf & _
        "  version: 1.1" & vbCrLf & _
        "layers:" & vbCrLf & _
        "  - Core" & vbCrLf & _
        "  - Domain" & vbCrLf & _
        "modules:" & vbCrLf & _
        "  Domain:" & vbCrLf & _
        "    Classes:" & vbCrLf & _
        "      - Subject"
End Function

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

Private Function GetTestFolderPath() As String
    GetTestFolderPath = Environ$("TEMP") & "\VMF_AppProjectManifestParseTests"
End Function

Private Function TryGenerateManifestContent( _
    ByVal Parser As Build_BlueprintParser, _
    ByVal LayerName As String, _
    ByRef ErrorText As String) As String

    On Error GoTo ErrHandler

    TryGenerateManifestContent = Parser.BuildGenerateManifestContent(LayerName)
    ErrorText = vbNullString
    Exit Function

ErrHandler:
    ErrorText = Err.Description
    TryGenerateManifestContent = vbNullString
    Err.Clear
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

Private Function IsAbsolutePath(ByVal FilePath As String) As Boolean
    IsAbsolutePath = ((Len(FilePath) >= 3 _
            And (Mid$(FilePath, 2, 2) = ":\" Or Mid$(FilePath, 2, 2) = ":/")) _
        Or Left$(FilePath, 2) = "\\" _
        Or Left$(FilePath, 2) = "//")
End Function
