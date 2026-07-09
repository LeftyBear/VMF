VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfVbaProjectProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfVbaProjectProvider
' Layer: Infrastructure
' Responsibility: VBA project module operations.
' Dependencies: Common
'=========================================================================

Private Const InfVbextCtStdModule As Long = 1
Private Const InfVbextCtClassModule As Long = 2
Private Const InfBuildWorkbookName As String = "Build.xlam"
Private Const InfDefaultTargetWorkbookName As String = "VMF.xlam"

' Adds a standard module to the target workbook and writes CodeText.
Public Function InfAddStandardModule(ByVal ModuleName As String, ByVal CodeText As String) As ComResult
    Dim VBProj As Object
    Dim NewComp As Object
    Dim VerificationResult As ComResult

    On Error GoTo ErrHandler

    ComRequireText ModuleName, "ModuleName", "InfAddStandardModule"
    ComRequireText CodeText, "CodeText", "InfAddStandardModule"

    Set VBProj = GetActiveVBProject()

    If ModuleExistsInProject(VBProj, ModuleName) Then
        Err.Raise ComErrInvalidArgument, "InfAddStandardModule", "Module already exists: " & ModuleName
    End If

    Set NewComp = VBProj.VBComponents.Add(InfVbextCtStdModule)
    NewComp.Name = ModuleName
    WriteComponentSource NewComp, NormalizeGeneratedSource(CodeText)

    Set VerificationResult = VerifyGeneratedModule(VBProj, ModuleName, CodeText)
    If VerificationResult.IsFailure Then
        Err.Raise ComErrInvalidState, "InfAddStandardModule", VerificationResult.Message
    End If

    Set InfAddStandardModule = ComCreateSuccess("Module created: " & ModuleName)
    Exit Function

ErrHandler:
    Dim Description As String

    Description = Err.Description

    On Error Resume Next
    If Not NewComp Is Nothing Then
        VBProj.VBComponents.Remove NewComp
    End If
    On Error GoTo 0

    Set InfAddStandardModule = ComCreateFailureFromErr("InfVbaProjectProvider.InfAddStandardModule", Description, "Error", False)
End Function

' Adds a class module to the target workbook and writes CodeText.
Public Function InfAddClassModule(ByVal ClassName As String, ByVal CodeText As String) As ComResult
    Dim VBProj As Object
    Dim NewComp As Object
    Dim VerificationResult As ComResult

    On Error GoTo ErrHandler

    ComRequireText ClassName, "ClassName", "InfAddClassModule"
    ComRequireText CodeText, "CodeText", "InfAddClassModule"

    Set VBProj = GetActiveVBProject()

    If ModuleExistsInProject(VBProj, ClassName) Then
        Err.Raise ComErrInvalidArgument, "InfAddClassModule", "Module already exists: " & ClassName
    End If

    Set NewComp = VBProj.VBComponents.Add(InfVbextCtClassModule)
    NewComp.Name = ClassName
    WriteComponentSource NewComp, NormalizeGeneratedSource(CodeText)

    Set VerificationResult = VerifyGeneratedClass(VBProj, ClassName, CodeText)
    If VerificationResult.IsFailure Then
        Err.Raise ComErrInvalidState, "InfAddClassModule", VerificationResult.Message
    End If

    Set InfAddClassModule = ComCreateSuccess("Class module created: " & ClassName)
    Exit Function

ErrHandler:
    Dim Description As String

    Description = Err.Description

    On Error Resume Next
    If Not NewComp Is Nothing Then
        VBProj.VBComponents.Remove NewComp
    End If
    On Error GoTo 0

    Set InfAddClassModule = ComCreateFailureFromErr("InfVbaProjectProvider.InfAddClassModule", Description, "Error", False)
End Function

' Returns True when a module exists in the target workbook.
Public Function InfModuleExists(ByVal ModuleName As String) As Boolean
    ComRequireText ModuleName, "ModuleName", "InfModuleExists"
    InfModuleExists = ModuleExistsInProject(GetActiveVBProject(), ModuleName)
End Function

' Removes a module from the target workbook when present.
Public Function InfRemoveModule(ByVal ModuleName As String) As ComResult
    Dim VBProj As Object
    Dim Comp As Object

    On Error GoTo ErrHandler

    ComRequireText ModuleName, "ModuleName", "InfRemoveModule"
    Set VBProj = GetActiveVBProject()

    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            VBProj.VBComponents.Remove Comp
            Set InfRemoveModule = ComCreateSuccess("Module removed: " & ModuleName)
            Exit Function
        End If
    Next Comp

    Set InfRemoveModule = ComCreateSuccess("Module was not present: " & ModuleName)
    Exit Function

ErrHandler:
    Set InfRemoveModule = ComCreateFailureFromErr("InfVbaProjectProvider.InfRemoveModule", Err.Description, "Error", False)
End Function

' Returns all code text from a module in the target workbook.
Public Function InfGetModuleText(ByVal ModuleName As String) As String
    Dim VBProj As Object
    Dim Comp As Object

    ComRequireText ModuleName, "ModuleName", "InfGetModuleText"
    Set VBProj = GetActiveVBProject()
    Set Comp = VBProj.VBComponents(ModuleName)

    If Comp.CodeModule.CountOfLines = 0 Then
        InfGetModuleText = vbNullString
    Else
        InfGetModuleText = Comp.CodeModule.Lines(1, Comp.CodeModule.CountOfLines)
    End If
End Function

' Resolves the external target VBProject for generation.
Public Function InfResolveTargetVBProject() As Object
    Set InfResolveTargetVBProject = GetActiveVBProject()
End Function

' Returns True when ModuleName exists in VBProj.
Public Function InfModuleExistsInProject(ByVal VBProj As Object, ByVal ModuleName As String) As Boolean
    If VBProj Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfModuleExistsInProject", "VBProj is required."
    End If

    ComRequireText ModuleName, "ModuleName", "InfModuleExistsInProject"
    InfModuleExistsInProject = ModuleExistsInProject(VBProj, ModuleName)
End Function

' Removes ModuleName from VBProj when present.
Public Sub InfRemoveComponentFromProject(ByVal VBProj As Object, ByVal ModuleName As String)
    Dim Comp As Object

    If VBProj Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfRemoveComponentFromProject", "VBProj is required."
    End If

    ComRequireText ModuleName, "ModuleName", "InfRemoveComponentFromProject"

    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            VBProj.VBComponents.Remove Comp
            Exit Sub
        End If
    Next Comp
End Sub

' Returns all code text from ModuleName in VBProj.
Public Function InfGetModuleTextFromProject(ByVal VBProj As Object, ByVal ModuleName As String) As String
    If VBProj Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfGetModuleTextFromProject", "VBProj is required."
    End If

    ComRequireText ModuleName, "ModuleName", "InfGetModuleTextFromProject"
    InfGetModuleTextFromProject = GetModuleTextFromProject(VBProj, ModuleName)
End Function

Private Function GetActiveVBProject() As Object
    Dim WB As Object

    Set WB = ResolveTargetWorkbook()

    If WB Is Nothing Then
        Err.Raise ComErrInvalidState, "InfVbaProjectProvider", "No target workbook available."
    End If

    Set GetActiveVBProject = CallByName(WB, "VBProject", VbGet)
End Function

Private Sub WriteComponentSource(ByVal Comp As Object, ByVal CodeText As String)
    Dim CodeModule As Object

    Set CodeModule = Comp.CodeModule
    If CodeModule.CountOfLines > 0 Then
        CodeModule.DeleteLines 1, CodeModule.CountOfLines
    End If

    CodeModule.AddFromString CodeText
End Sub

Private Function NormalizeGeneratedSource(ByVal CodeText As String) As String
    Dim Lines() As String
    Dim Result As String
    Dim LineText As String
    Dim NormalizedText As String
    Dim i As Long
    Dim HasBody As Boolean

    NormalizedText = Replace(CodeText, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)
    Lines = Split(NormalizedText, vbLf)

    Result = "Option Explicit"
    HasBody = False

    For i = LBound(Lines) To UBound(Lines)
        LineText = Lines(i)

        If StrComp(Trim$(LineText), "Option Explicit", vbTextCompare) = 0 Then
            ' The canonical declaration is written once at the top.
        ElseIf StrComp(Left$(Trim$(LineText), 10), "Attribute ", vbTextCompare) = 0 Then
            ' Component attributes are managed by the VBE component.
        ElseIf Not HasBody And Len(Trim$(LineText)) = 0 Then
            ' Drop leading blank lines before the canonical header.
        Else
            If Not HasBody Then
                Result = Result & vbCrLf & vbCrLf
                HasBody = True
            Else
                Result = Result & vbCrLf
            End If

            Result = Result & LineText
        End If
    Next i

    NormalizeGeneratedSource = Result
End Function

Private Function ResolveTargetWorkbook() As Object
    Dim WB As Object

    Set WB = TryGetActiveWorkbook()
    If IsGenerationTargetWorkbook(WB) Then
        Set ResolveTargetWorkbook = WB
        Exit Function
    End If

    Set WB = TryGetWorkbookByName(InfDefaultTargetWorkbookName)
    If IsGenerationTargetWorkbook(WB) Then
        Set ResolveTargetWorkbook = WB
        Exit Function
    End If

    On Error Resume Next
    For Each WB In Application.Workbooks
        If IsGenerationTargetWorkbook(WB) Then
            Set ResolveTargetWorkbook = WB
            Exit Function
        End If
    Next WB
    On Error GoTo 0

    Set ResolveTargetWorkbook = Nothing
End Function

Private Function TryGetActiveWorkbook() As Object
    On Error Resume Next
    Set TryGetActiveWorkbook = Application.ActiveWorkbook
    On Error GoTo 0
End Function

Private Function TryGetWorkbookByName(ByVal WorkbookName As String) As Object
    On Error Resume Next
    Set TryGetWorkbookByName = Application.Workbooks(WorkbookName)
    On Error GoTo 0
End Function

Private Function IsGenerationTargetWorkbook(ByVal WB As Object) As Boolean
    If WB Is Nothing Then
        IsGenerationTargetWorkbook = False
        Exit Function
    End If

    If IsBuildWorkbook(WB) Then
        IsGenerationTargetWorkbook = False
        Exit Function
    End If

    IsGenerationTargetWorkbook = True
End Function

Private Function IsBuildWorkbook(ByVal WB As Object) As Boolean
    Dim WorkbookName As String

    If WB Is Nothing Then
        IsBuildWorkbook = False
        Exit Function
    End If

    On Error Resume Next
    WorkbookName = CStr(CallByName(WB, "Name", VbGet))
    On Error GoTo 0

    IsBuildWorkbook = (StrComp(WorkbookName, InfBuildWorkbookName, vbTextCompare) = 0)
End Function

Private Function ModuleExistsInProject(ByVal VBProj As Object, ByVal ModuleName As String) As Boolean
    Dim Comp As Object

    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            ModuleExistsInProject = True
            Exit Function
        End If
    Next Comp

    ModuleExistsInProject = False
End Function

Private Function VerifyGeneratedModule(ByVal VBProj As Object, ByVal ModuleName As String, ByVal ExpectedText As String) As ComResult
    Dim Text As String

    Text = GetModuleTextFromProject(VBProj, ModuleName)

    If CountOptionExplicit(Text) <> 1 Then
        Set VerifyGeneratedModule = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated module must contain exactly one Option Explicit."))
        Exit Function
    End If

    If InStr(1, Text, "Module: " & ModuleName, vbTextCompare) = 0 Then
        Set VerifyGeneratedModule = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated module is missing its module header."))
        Exit Function
    End If

    If Len(ExpectedText) > 0 And Len(Text) = 0 Then
        Set VerifyGeneratedModule = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated module text is empty."))
        Exit Function
    End If

    Set VerifyGeneratedModule = ComCreateSuccess()
End Function

Private Function VerifyGeneratedClass(ByVal VBProj As Object, ByVal ClassName As String, ByVal ExpectedText As String) As ComResult
    Dim Text As String

    Text = GetModuleTextFromProject(VBProj, ClassName)

    If CountOptionExplicit(Text) <> 1 Then
        Set VerifyGeneratedClass = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated class must contain exactly one Option Explicit."))
        Exit Function
    End If

    If InStr(1, Text, "Class: " & ClassName, vbTextCompare) = 0 Then
        Set VerifyGeneratedClass = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated class is missing its class header."))
        Exit Function
    End If

    If Len(ExpectedText) > 0 And Len(Text) = 0 Then
        Set VerifyGeneratedClass = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated class text is empty."))
        Exit Function
    End If

    Set VerifyGeneratedClass = ComCreateSuccess()
End Function

Private Function GetModuleTextFromProject(ByVal VBProj As Object, ByVal ModuleName As String) As String
    Dim Comp As Object

    Set Comp = VBProj.VBComponents(ModuleName)

    If Comp.CodeModule.CountOfLines = 0 Then
        GetModuleTextFromProject = vbNullString
    Else
        GetModuleTextFromProject = Comp.CodeModule.Lines(1, Comp.CodeModule.CountOfLines)
    End If
End Function

Private Function CountOptionExplicit(ByVal Text As String) As Long
    Dim Lines() As String
    Dim NormalizedText As String
    Dim i As Long

    NormalizedText = Replace(Text, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)
    Lines = Split(NormalizedText, vbLf)

    For i = LBound(Lines) To UBound(Lines)
        If StrComp(Trim$(Lines(i)), "Option Explicit", vbTextCompare) = 0 Then
            CountOptionExplicit = CountOptionExplicit + 1
        End If
    Next i
End Function
