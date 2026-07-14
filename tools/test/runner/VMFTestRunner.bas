Option Explicit
Attribute VB_Name = "VMFTestRunner"

'=========================================================================
' Module: VMFTestRunner
' Layer: Tooling
' Responsibility: Persistent trusted VBA unit test runner.
'=========================================================================

Private Const VbExtCtDocument As Long = 100

Private Type TestResult
    Runner As String
    Status As String
    ErrorText As String
End Type

Private LogFilePath As String

Public Function VMFRunAllTests(ByVal RepositoryRootPath As String) As String
    Dim Runners As Variant
    Dim Results() As TestResult
    Dim RunnerIndex As Long
    Dim FailureCount As Long
    Dim Summary As String

    On Error GoTo ErrHandler

    LogFilePath = BuildLogFilePath(RepositoryRootPath)
    ResetLog
    AppendLog "VMFRunAllTests started."

    Runners = Array( _
        "ComRunCommonPhase1Tests", _
        "InfRunInfrastructurePhase2Tests", _
        "DomRunVmfCorePhase3Tests", _
        "AppRunGeneratorPhase1Tests", _
        "AppRunGeneratorPhase1NegativeTests", _
        "AppRunGeneratorPhase2Tests", _
        "AppRunProjectManifestParseTests", _
        "AppRunManifestEditorServiceTests", _
        "AppRunGenerateCommonPhase3_1Tests", _
        "AppRunGenerateManifestPhase3_2Tests", _
        "AppRunGenerateInfrastructurePhase3_3Tests", _
        "AppRunGenerateDomainPhase3_4Tests", _
        "AppRunGenerateApplicationPhase3_5Tests", _
        "AppRunGeneratePresentationPhase3_6Tests", _
        "AppRunBuildPhase4Tests", _
        "PreRunUiPhase5Tests", _
        "PreRunGenerateModulePhase6Tests")

    AppendLog "Using source and test modules embedded in the trusted runner workbook."

    ReDim Results(LBound(Runners) To UBound(Runners))
    For RunnerIndex = LBound(Runners) To UBound(Runners)
        AppendLog "Running " & CStr(Runners(RunnerIndex))
        Results(RunnerIndex) = RunOneTest(CStr(Runners(RunnerIndex)))
        AppendLog CStr(Runners(RunnerIndex)) & ": " & Results(RunnerIndex).Status
        If Results(RunnerIndex).Status = "Failed" Then
            FailureCount = FailureCount + 1
        End If
    Next RunnerIndex

    Summary = RenderSummary(Results)
    WriteSummarySheet Results
    AppendLog "Summary written."

    VMFRunAllTests = Summary
    Exit Function

ErrHandler:
    AppendLog "ERROR: " & Err.Description
    VMFRunAllTests = "Failed: " & Err.Description
End Function

Private Function RunOneTest(ByVal RunnerName As String) As TestResult
    Dim Result As TestResult
    Dim MacroNames As Collection
    Dim MacroName As Variant
    Dim ErrorText As String

    Result.Runner = RunnerName
    Set MacroNames = ResolveMacroNames(RunnerName)

    For Each MacroName In MacroNames
        AppendLog "Trying macro: " & CStr(MacroName)
        If TryRunMacro(CStr(MacroName), ErrorText) Then
            Result.Status = "Passed"
            Result.ErrorText = vbNullString
            RunOneTest = Result
            Exit Function
        End If
        AppendLog "Macro failed: " & ErrorText
    Next MacroName

    Result.Status = "Failed"
    Result.ErrorText = ErrorText
    RunOneTest = Result
End Function

Private Function TryRunMacro(ByVal MacroName As String, ByRef ErrorText As String) As Boolean
    On Error GoTo ErrHandler
    Application.Run MacroName
    TryRunMacro = True
    ErrorText = vbNullString
    Exit Function

ErrHandler:
    TryRunMacro = False
    ErrorText = Err.Description
    Err.Clear
End Function

Private Function BuildLogFilePath(ByVal RepositoryRootPath As String) As String
    Dim FileSystem As Object
    Dim DebugFolderPath As String

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    DebugFolderPath = FileSystem.BuildPath(RepositoryRootPath, "dist\debug")
    If Not FileSystem.FolderExists(DebugFolderPath) Then
        FileSystem.CreateFolder DebugFolderPath
    End If

    BuildLogFilePath = FileSystem.BuildPath(DebugFolderPath, "VMFTestRunner.log")
End Function

Private Sub ResetLog()
    Dim FileSystem As Object
    Dim TextFile As Object

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    Set TextFile = FileSystem.CreateTextFile(LogFilePath, True, False)
    TextFile.WriteLine Format$(Now, "yyyy-mm-dd hh:nn:ss") & " Log reset."
    TextFile.Close
End Sub

Private Sub AppendLog(ByVal Message As String)
    Dim FileSystem As Object
    Dim TextFile As Object

    If Len(LogFilePath) = 0 Then
        Exit Sub
    End If

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    Set TextFile = FileSystem.OpenTextFile(LogFilePath, 8, True)
    TextFile.WriteLine Format$(Now, "yyyy-mm-dd hh:nn:ss") & " " & Message
    TextFile.Close
End Sub

Private Function ResolveMacroNames(ByVal RunnerName As String) As Collection
    Dim MacroNames As Collection
    Dim Component As Object
    Dim TextValue As String
    Dim FoundComponentName As String

    Set MacroNames = New Collection

    For Each Component In ThisWorkbook.VBProject.VBComponents
        If Component.Type <> VbExtCtDocument Then
            If Component.CodeModule.CountOfLines > 0 Then
                TextValue = Component.CodeModule.Lines(1, Component.CodeModule.CountOfLines)
                If InStr(1, TextValue, "Sub " & RunnerName, vbTextCompare) > 0 _
                    Or InStr(1, TextValue, "Function " & RunnerName, vbTextCompare) > 0 Then
                    FoundComponentName = Component.Name
                    Exit For
                End If
            End If
        End If
    Next Component

    If Len(FoundComponentName) > 0 Then
        MacroNames.Add "'" & ThisWorkbook.Name & "'!" & FoundComponentName & "." & RunnerName
        MacroNames.Add FoundComponentName & "." & RunnerName
    End If

    MacroNames.Add "'" & ThisWorkbook.Name & "'!" & RunnerName
    MacroNames.Add RunnerName
    Set ResolveMacroNames = MacroNames
End Function

Private Function RenderSummary(ByRef Results() As TestResult) As String
    Dim Index As Long
    Dim TextValue As String

    TextValue = "Test run summary:"
    For Index = LBound(Results) To UBound(Results)
        TextValue = TextValue & vbCrLf & Results(Index).Runner & ": " & Results(Index).Status
        If Len(Results(Index).ErrorText) > 0 Then
            TextValue = TextValue & " - " & Results(Index).ErrorText
        End If
    Next Index

    RenderSummary = TextValue
End Function

Private Sub WriteSummarySheet(ByRef Results() As TestResult)
    Dim Sheet As Object
    Dim Index As Long

    Set Sheet = EnsureSummarySheet()
    Sheet.Cells.Clear
    Sheet.Cells(1, 1).Value = "Runner"
    Sheet.Cells(1, 2).Value = "Status"
    Sheet.Cells(1, 3).Value = "Error"

    For Index = LBound(Results) To UBound(Results)
        Sheet.Cells(Index + 2, 1).Value = Results(Index).Runner
        Sheet.Cells(Index + 2, 2).Value = Results(Index).Status
        Sheet.Cells(Index + 2, 3).Value = Results(Index).ErrorText
    Next Index

    ThisWorkbook.Save
End Sub

Private Function EnsureSummarySheet() As Object
    Dim Sheet As Object

    For Each Sheet In ThisWorkbook.Worksheets
        If Sheet.Name = "Results" Then
            Set EnsureSummarySheet = Sheet
            Exit Function
        End If
    Next Sheet

    Set EnsureSummarySheet = ThisWorkbook.Worksheets.Add
    EnsureSummarySheet.Name = "Results"
End Function
