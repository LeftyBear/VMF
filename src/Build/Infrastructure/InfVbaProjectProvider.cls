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
    NewComp.CodeModule.AddFromString CodeText

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
    NewComp.CodeModule.AddFromString CodeText

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

Private Function GetActiveVBProject() As Object
    Dim WB As Object

    Set WB = ResolveTargetWorkbook()

    If WB Is Nothing Then
        Err.Raise ComErrInvalidState, "InfVbaProjectProvider", "No target workbook available."
    End If

    Set GetActiveVBProject = CallByName(WB, "VBProject", VbGet)
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

    If Not FullProjectCompile() Then
        Text = GetModuleTextFromProject(VBProj, ModuleName)

        If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
            Set VerifyGeneratedModule = ComCreateFailure( _
                ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated module is missing Option Explicit."))
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
    End If

    Set VerifyGeneratedModule = ComCreateSuccess()
End Function

Private Function VerifyGeneratedClass(ByVal VBProj As Object, ByVal ClassName As String, ByVal ExpectedText As String) As ComResult
    Dim Text As String

    If Not FullProjectCompile() Then
        Text = GetModuleTextFromProject(VBProj, ClassName)

        If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
            Set VerifyGeneratedClass = ComCreateFailure( _
                ComCreateErrorInfo(ComErrInvalidState, "InfVbaProjectProvider", "Generated class is missing Option Explicit."))
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

Private Function FullProjectCompile() As Boolean
    On Error GoTo ErrHandler

    Application.VBE.CommandBars("Menu Bar").Controls("Debug").Controls("Compile VBAProject").Execute

    FullProjectCompile = True
    Exit Function

ErrHandler:
    FullProjectCompile = False
End Function
