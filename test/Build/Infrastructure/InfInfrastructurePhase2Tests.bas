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
    ResolveTemplatePath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "ModuleTemplate.txt"
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

Private Function GetTestFolderPath() As String
    GetTestFolderPath = Environ$("TEMP") & "\" & InfTestFolderName
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
