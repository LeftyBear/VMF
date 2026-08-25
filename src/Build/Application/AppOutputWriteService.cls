VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppOutputWriteService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppOutputWriteService
' Layer: Application
' Responsibility: Validate Generator output and build local output-write plans.
' Dependencies: Common
'=========================================================================

Private Const ResultSuccess As String = "Success"
Private Const ResultHardStop As String = "HardStop"

' Builds a deterministic local write plan from complete successful Generator output.
Public Function AppBuildOutputWritePlan(ByVal GeneratorOutput As Object) As Object
    Dim Result As Object

    On Error GoTo ErrHandler

    Set Result = CreateResult(False, ResultHardStop, "Generator output is required.")
    If GeneratorOutput Is Nothing Then
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    If Not HasBooleanValue(GeneratorOutput, "Success", True) Then
        Result("Message") = "Generator output must be successful."
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    If Not HasCollection(GeneratorOutput, "GeneratedUnits") Then
        Result("Message") = "Generator output units are required."
        Set AppBuildOutputWritePlan = Result
        Exit Function
    End If

    Set AppBuildOutputWritePlan = BuildSuccessfulPlan(GeneratorOutput("GeneratedUnits"))
    Exit Function

ErrHandler:
    Set AppBuildOutputWritePlan = CreateResult(False, ResultHardStop, "Output write hard-stop: " & Err.Description)
End Function

' Writes approved output-write plan units to a deterministic local folder.
Public Function AppWriteGeneratedOutput(ByVal OutputWritePlan As Object, ByVal OutputFolderPath As String) As Object
    Dim WriteUnits As Collection
    Dim FileSystem As Object
    Dim WrittenFiles As Collection

    On Error GoTo ErrHandler

    If OutputWritePlan Is Nothing Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write plan is required.")
        Exit Function
    End If

    If Not HasBooleanValue(OutputWritePlan, "Success", True) Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write plan must be successful.")
        Exit Function
    End If

    If Not HasCollection(OutputWritePlan, "WriteUnits") Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write units are required.")
        Exit Function
    End If

    If ComIsBlankText(OutputFolderPath) Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output folder path is required.")
        Exit Function
    End If

    Set WriteUnits = OutputWritePlan("WriteUnits")
    If WriteUnits.Count = 0 Then
        Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Output write units are absent.")
        Exit Function
    End If

    Set FileSystem = CreateObject("Scripting.FileSystemObject")
    PreflightWriteUnits WriteUnits, FileSystem, OutputFolderPath

    If Not FileSystem.FolderExists(OutputFolderPath) Then
        FileSystem.CreateFolder OutputFolderPath
    End If

    Set WrittenFiles = WriteAllUnits(WriteUnits, FileSystem, OutputFolderPath)
    Set AppWriteGeneratedOutput = CreateSuccessfulWriteResult(WrittenFiles)
    Exit Function

ErrHandler:
    Set AppWriteGeneratedOutput = CreateWriteResult(False, ResultHardStop, "Generated output write hard-stop: " & Err.Description)
End Function

' Applies approved write-plan units to a local test-controlled target only.
Public Function AppApplyGeneratedOutputToLocalTarget(ByVal OutputWritePlan As Object, ByVal TargetProject As Object) As Object
    Dim WriteUnits As Collection
    Dim Result As Object

    On Error GoTo ErrHandler

    If OutputWritePlan Is Nothing Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write plan is required.")
        Exit Function
    End If

    If TargetProject Is Nothing Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Local target project is required.")
        Exit Function
    End If

    If Not HasBooleanValue(OutputWritePlan, "Success", True) Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write plan must be successful.")
        Exit Function
    End If

    If Not HasCollection(OutputWritePlan, "WriteUnits") Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write units are required.")
        Exit Function
    End If

    Set WriteUnits = OutputWritePlan("WriteUnits")
    If WriteUnits.Count = 0 Then
        Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Output write units are absent.")
        Exit Function
    End If

    PreflightLocalTargetMutation WriteUnits, TargetProject
    ApplyLocalTargetMutation WriteUnits, TargetProject

    Set Result = CreateMutationResult(True, ResultSuccess, "Generated output applied to local target.")
    Result("MutatedModules") = WriteUnits.Count
    Set AppApplyGeneratedOutputToLocalTarget = Result
    Exit Function

ErrHandler:
    Set AppApplyGeneratedOutputToLocalTarget = CreateMutationResult(False, ResultHardStop, "Target VBA project mutation hard-stop: " & Err.Description)
End Function

Private Function BuildSuccessfulPlan(ByVal GeneratedUnits As Collection) As Object
    Dim Result As Object
    Dim WriteUnits As Collection
    Dim Unit As Variant

    If GeneratedUnits Is Nothing Then
        Set BuildSuccessfulPlan = CreateResult(False, ResultHardStop, "Generator output units are required.")
        Exit Function
    End If

    If GeneratedUnits.Count = 0 Then
        Set BuildSuccessfulPlan = CreateResult(False, ResultHardStop, "Generator output units are absent.")
        Exit Function
    End If

    Set WriteUnits = New Collection
    For Each Unit In GeneratedUnits
        WriteUnits.Add CreateWriteUnit(Unit)
    Next Unit

    Set Result = CreateResult(True, ResultSuccess, "Output write plan constructed.")
    Set Result("WriteUnits") = WriteUnits
    Set BuildSuccessfulPlan = Result
End Function

Private Function CreateWriteUnit(ByVal Unit As Variant) As Object
    Dim Source As Object
    Dim WriteUnit As Object
    Dim ModuleType As String

    If Not IsObject(Unit) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.CreateWriteUnit", "Generator output unit is invalid."
    End If

    Set Source = Unit
    RequireCompleteGeneratedUnit Source

    ModuleType = RequiredText(Source, "moduleType")

    Set WriteUnit = CreateObject("Scripting.Dictionary")
    WriteUnit("order") = CLng(Source("order"))
    WriteUnit("moduleName") = RequiredText(Source, "moduleName")
    WriteUnit("moduleType") = ModuleType
    WriteUnit("templateKey") = RequiredText(Source, "templateKey")
    WriteUnit("fileName") = RequiredText(Source, "moduleName") & ModuleExtension(ModuleType)
    WriteUnit("generatedSource") = RequiredText(Source, "generatedSource")
    WriteUnit("writeStatus") = "Planned"

    Set CreateWriteUnit = WriteUnit
End Function

Private Sub PreflightWriteUnits(ByVal WriteUnits As Collection, ByVal FileSystem As Object, ByVal OutputFolderPath As String)
    Dim Unit As Variant
    Dim FileName As String
    Dim FilePath As String

    For Each Unit In WriteUnits
        If Not IsObject(Unit) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightWriteUnits", "Output write unit is invalid."
        End If

        FileName = RequiredText(Unit, "fileName")
        RequireSafeFileName FileName, "AppOutputWriteService.PreflightWriteUnits"
        RequiredText Unit, "generatedSource"
        FilePath = FileSystem.BuildPath(OutputFolderPath, FileName)
        If FileSystem.FileExists(FilePath) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightWriteUnits", "Output file already exists: " & FileName
        End If
    Next Unit
End Sub

Private Function WriteAllUnits(ByVal WriteUnits As Collection, ByVal FileSystem As Object, ByVal OutputFolderPath As String) As Collection
    Dim Unit As Variant
    Dim TextFile As Object
    Dim WrittenFiles As Collection
    Dim FilePath As String

    Set WrittenFiles = New Collection
    For Each Unit In WriteUnits
        FilePath = FileSystem.BuildPath(OutputFolderPath, RequiredText(Unit, "fileName"))
        Set TextFile = FileSystem.CreateTextFile(FilePath, False, False)
        TextFile.Write RequiredText(Unit, "generatedSource")
        TextFile.Close
        WrittenFiles.Add FilePath
    Next Unit

    Set WriteAllUnits = WrittenFiles
End Function

Private Sub PreflightLocalTargetMutation(ByVal WriteUnits As Collection, ByVal TargetProject As Object)
    Dim Unit As Variant
    Dim Modules As Object
    Dim Seen As Object
    Dim ModuleName As String

    If Not TargetProject.Exists("Modules") Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Local target project Modules collection is required."
    End If

    If Not IsObject(TargetProject("Modules")) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Local target project Modules collection is invalid."
    End If

    Set Modules = TargetProject("Modules")
    Set Seen = CreateObject("Scripting.Dictionary")

    For Each Unit In WriteUnits
        If Not IsObject(Unit) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Output write unit is invalid."
        End If

        ModuleName = RequiredText(Unit, "moduleName")
        RequireSafeFileName RequiredText(Unit, "fileName"), "AppOutputWriteService.PreflightLocalTargetMutation"
        RequiredText Unit, "generatedSource"

        If Seen.Exists(ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Duplicate module mutation is prohibited: " & ModuleName
        End If

        If Modules.Exists(ModuleName) Then
            Err.Raise ComErrInvalidArgument, "AppOutputWriteService.PreflightLocalTargetMutation", "Existing module conflict is not authorized: " & ModuleName
        End If

        Seen(ModuleName) = True
    Next Unit
End Sub

Private Sub ApplyLocalTargetMutation(ByVal WriteUnits As Collection, ByVal TargetProject As Object)
    Dim Unit As Variant
    Dim Modules As Object

    Set Modules = TargetProject("Modules")

    For Each Unit In WriteUnits
        Modules(RequiredText(Unit, "moduleName")) = RequiredText(Unit, "generatedSource")
    Next Unit
End Sub

Private Sub RequireCompleteGeneratedUnit(ByVal Unit As Object)
    If Unit Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Generator output unit is required."
    End If

    If Not HasNumericValue(Unit, "order") Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Generator output unit order is required."
    End If

    If Not HasBooleanValue(Unit, "isFallbackDerived", False) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Fallback Template selection is prohibited."
    End If

    If Not HasBooleanValue(Unit, "isImplicitlySelected", False) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequireCompleteGeneratedUnit", "Implicit Template selection is prohibited."
    End If

    RequiredText Unit, "moduleName"
    RequiredText Unit, "moduleType"
    RequiredText Unit, "templateKey"
    RequiredText Unit, "generatedSource"
End Sub

Private Function RequiredText(ByVal Source As Object, ByVal FieldName As String) As String
    If Source Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Source item is required."
    End If

    If Not Source.Exists(FieldName) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Required Generator output field is missing: " & FieldName
    End If

    If ComIsBlankText(CStr(Source.Item(FieldName))) Then
        Err.Raise ComErrInvalidArgument, "AppOutputWriteService.RequiredText", "Required Generator output field is blank: " & FieldName
    End If

    RequiredText = Trim$(CStr(Source.Item(FieldName)))
End Function

Private Function ModuleExtension(ByVal ModuleType As String) As String
    If StrComp(ModuleType, "ClassModule", vbTextCompare) = 0 Then
        ModuleExtension = ".cls"
    Else
        ModuleExtension = ".bas"
    End If
End Function

Private Sub RequireSafeFileName(ByVal FileName As String, ByVal SourceName As String)
    If InStr(1, FileName, "\", vbBinaryCompare) > 0 Or InStr(1, FileName, "/", vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, SourceName, "Output write fileName must not include a path."
    End If
End Sub

Private Function HasBooleanValue(ByVal Source As Object, ByVal FieldName As String, ByVal ExpectedValue As Boolean) As Boolean
    If Source Is Nothing Then
        HasBooleanValue = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasBooleanValue = False
        Exit Function
    End If

    If VarType(Source.Item(FieldName)) <> vbBoolean Then
        HasBooleanValue = False
        Exit Function
    End If

    HasBooleanValue = (CBool(Source.Item(FieldName)) = ExpectedValue)
End Function

Private Function HasCollection(ByVal Source As Object, ByVal FieldName As String) As Boolean
    If Source Is Nothing Then
        HasCollection = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasCollection = False
        Exit Function
    End If

    HasCollection = TypeName(Source.Item(FieldName)) = "Collection"
End Function

Private Function HasNumericValue(ByVal Source As Object, ByVal FieldName As String) As Boolean
    If Source Is Nothing Then
        HasNumericValue = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasNumericValue = False
        Exit Function
    End If

    HasNumericValue = IsNumeric(Source.Item(FieldName))
End Function

Private Function CreateResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Set Result("WriteUnits") = New Collection

    Set CreateResult = Result
End Function

Private Function CreateWriteResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Set Result("WrittenFiles") = New Collection

    Set CreateWriteResult = Result
End Function

Private Function CreateSuccessfulWriteResult(ByVal WrittenFiles As Collection) As Object
    Dim Result As Object

    Set Result = CreateWriteResult(True, ResultSuccess, "Generated output written.")
    Set Result("WrittenFiles") = WrittenFiles
    Set CreateSuccessfulWriteResult = Result
End Function

Private Function CreateMutationResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Result("MutatedModules") = 0

    Set CreateMutationResult = Result
End Function
