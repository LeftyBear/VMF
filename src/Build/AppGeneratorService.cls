VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGeneratorService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: AppGeneratorService
' Layer: Application
' Responsibility: Module generation service (Phase 1: Generate Module)
' Dependencies: Common, Infrastructure (VBIDE via late-binding)
'==========================================================================

Private Const vbext_ct_StdModule As Long = 1
Private Generator As InfGenerator

' Initializes the service with required generation infrastructure.
Public Sub AppInitialize(ByVal ModuleGenerator As InfGenerator)
    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ModuleGenerator is required."
    End If

    Set Generator = ModuleGenerator
End Sub

' Generates a standard module with the required template.
' Parameters:
'   ModuleName: Desired module name.
' Return value:
'   ComResult describing success or failure.
Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult
    Dim VBProj As Object
    Dim NewComp As Object
    Dim Comp As Object
    Dim CodeText As String
    Dim Exists As Boolean

    On Error GoTo ErrHandler

    ' Validate input
    ComRequireText ModuleName, "ModuleName", "AppGenerateModule"

    ' Basic name validation: starts with letter, contains only A-Z0-9_ (case-insensitive)
    If Not ModuleNameLikeValid(ModuleName) Then
        Err.Raise vbObjectError + 9100, "AppGenerateModule", "ModuleName is invalid. Use letters, numbers and underscore; start with a letter."
    End If

    ' Acquire VBProject for the active workbook
    Set VBProj = GetActiveVBProject()

    ' Check for existing component with same name
    Exists = False
    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            Exists = True
            Exit For
        End If
    Next Comp

    If Exists Then
        Err.Raise vbObjectError + 9101, "AppGenerateModule", "Module already exists: " & ModuleName
    End If

    ' Add standard module
    Set NewComp = VBProj.VBComponents.Add(vbext_ct_StdModule)

    ' Set module name
    NewComp.Name = ModuleName

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "GeneratorService is not initialized."
    End If

    CodeText = Generator.InfGenerateModule(ModuleName, GetModuleTemplatePath())

    ' Write template
    NewComp.CodeModule.AddFromString CodeText
    ' Attempt full project compile when possible (requires Trust access to VBA project model).
    If Not FullProjectCompile() Then
        ' Fall back to lightweight verification if full compile is unavailable or failed
        If Not CompileAndVerify(VBProj, ModuleName) Then
            Err.Raise vbObjectError + 9120, "AppGenerateModule", "Generated module failed verification. Enable 'Trust access to the VBA project object model' to allow full compile checks."
        End If
    End If

    ' Successful result
    Set AppGenerateModule = ComCreateSuccess("Module created: " & ModuleName)
    Exit Function

ErrHandler:
    ' Attempt rollback
    On Error Resume Next
    If Not NewComp Is Nothing Then
        VBProj.VBComponents.Remove NewComp
    End If

    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set AppGenerateModule = ComCreateFailureFromErr("AppGeneratorService.AppGenerateModule", Desc, "Error", False)
End Function

' Validates basic module name rules.
Private Function ModuleNameLikeValid(ByVal Name As String) As Boolean
    Dim i As Long
    Dim ch As String

    If ComIsBlankText(Name) Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    Name = Trim$(Name)

    ' First char must be a letter
    ch = Mid$(Name, 1, 1)
    If Not ch Like "[A-Za-z]" Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    ' Remaining chars must be letters, digits, or underscore
    For i = 1 To Len(Name)
        ch = Mid$(Name, i, 1)
        If Not (ch Like "[A-Za-z0-9_]") Then
            ModuleNameLikeValid = False
            Exit Function
        End If
    Next i

    ModuleNameLikeValid = True
End Function

Private Function GetActiveVBProject() As Object
    Dim WB As Object

    ' Use ActiveWorkbook where possible; fall back to ThisWorkbook
    On Error Resume Next
    Set WB = Application.ActiveWorkbook
    If WB Is Nothing Then
        Set WB = ThisWorkbook
    End If

    If WB Is Nothing Then
        Err.Raise vbObjectError + 9110, "AppGeneratorService", "No active workbook available."
    End If

    Set GetActiveVBProject = CallByName(WB, "VBProject", VbGet)
End Function


' Performs a lightweight verification of the generated module.
' This does not perform a full VBA compiler run but checks for required
' header elements and non-empty content to reduce obvious syntax issues.
Private Function CompileAndVerify(ByVal VBProj As Object, ByVal ModuleName As String) As Boolean
    Dim Comp As Object
    Dim Text As String

    On Error GoTo Fail

    Set Comp = VBProj.VBComponents(ModuleName)
    If Comp Is Nothing Then
        CompileAndVerify = False
        Exit Function
    End If

    With Comp.CodeModule
        If .CountOfLines = 0 Then
            CompileAndVerify = False
            Exit Function
        End If
        Text = .Lines(1, .CountOfLines)
    End With

    ' Basic checks: Option Explicit and module header with module name
    If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
        CompileAndVerify = False
        Exit Function
    End If

    If InStr(1, Text, "Module: " & ModuleName, vbTextCompare) = 0 Then
        CompileAndVerify = False
        Exit Function
    End If

    CompileAndVerify = True
    Exit Function

Fail:
    CompileAndVerify = False
End Function


' Attempts to trigger a full VBAProject compile using the VBE UI command.
' Returns True when the compile command executed without raising an error.
' Note: This requires that the host allows access to the VBA project model and
' that VBE command bars are available. When not permitted, this function
' returns False and the caller should fall back to lightweight checks.
Private Function GetModuleTemplatePath() As String
    Dim RootPath As String
    Dim AddinWorkbook As Object

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
        Err.Raise ComErrInvalidState, "AppGeneratorService", "Template root path is unavailable."
    End If

    GetModuleTemplatePath = RootPath & Application.PathSeparator & "src" & Application.PathSeparator & "Build" & Application.PathSeparator & "ModuleTemplate.txt"
End Function

Private Function FullProjectCompile() As Boolean
    On Error GoTo ErrHandler

    ' Try to invoke the Debug->Compile menu command. This is best-effort and
    ' may fail depending on host security settings or localization.
    Application.VBE.CommandBars("Menu Bar").Controls("Debug").Controls("Compile VBAProject").Execute

    FullProjectCompile = True
    Exit Function

ErrHandler:
    FullProjectCompile = False
End Function
