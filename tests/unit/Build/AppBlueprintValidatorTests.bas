Option Explicit
Attribute VB_Name = "AppBlueprintValidatorTests"

'=========================================================================
' Module: AppBlueprintValidatorTests
' Layer: Application
' Responsibility: Focused tests for Blueprint Validator Candidate B.
' Dependencies: Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9320

Public Sub AppRunBlueprintValidatorTests()
    VerifyMinimalApprovedBlueprintIsGeneratable
    VerifyFunctionWithReturnValueIsGeneratable
    VerifyDraftBlueprintIsValidNotGeneratable
    VerifyMissingBlueprintIdIsInvalid
    VerifyUnsupportedVersionIsInvalid
    VerifyApprovalConflictIsInvalid
    VerifyApprovedWithoutApproverIsInvalid
    VerifyTargetErrorsAreInvalid
    VerifyModuleAndProcedureErrorsAreInvalid
    VerifyReturnValueErrorsAreInvalid
    VerifyGenerationPolicyErrorsAreInvalid
End Sub

Private Sub VerifyMinimalApprovedBlueprintIsGeneratable()
    Dim Result As BlueprintValidationResult

    Set Result = ValidateBlueprint(CreateValidBlueprint())

    AssertEquals BlueprintValidationResultValidGeneratable(), Result.ResultKind, "Approved Blueprint should be validGeneratable."
    AssertTrue Result.Generatable, "Approved Blueprint should be generatable."
    AssertEquals 0, Result.Diagnostics.Count, "Approved Blueprint should have no diagnostics."
End Sub

Private Sub VerifyFunctionWithReturnValueIsGeneratable()
    Dim Blueprint As Object
    Dim ProcedureInfo As Object
    Dim ReturnValue As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set ProcedureInfo = FirstProcedure(Blueprint)
    ProcedureInfo("kind") = "Function"
    Set ReturnValue = CreateObject("Scripting.Dictionary")
    ReturnValue.Add "type", "String"
    ProcedureInfo.Add "returnValue", ReturnValue

    Set Result = ValidateBlueprint(Blueprint)

    AssertEquals BlueprintValidationResultValidGeneratable(), Result.ResultKind, "Function with returnValue should be validGeneratable."
    AssertTrue Result.Generatable, "Function with returnValue should be generatable."
    AssertEquals 0, Result.Diagnostics.Count, "Function with returnValue should have no diagnostics."
End Sub

Private Sub VerifyDraftBlueprintIsValidNotGeneratable()
    Dim Blueprint As Object
    Dim Approval As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Blueprint("status") = "draft"
    Set Approval = Blueprint("approval")
    Approval("state") = "notApproved"
    Approval.Remove "approvedBy"
    Approval.Remove "approvedAt"

    Set Result = ValidateBlueprint(Blueprint)

    AssertEquals BlueprintValidationResultValidNotGeneratable(), Result.ResultKind, "Draft Blueprint should be validNotGeneratable."
    AssertFalse Result.Generatable, "Draft Blueprint should not be generatable."
    AssertDiagnosticCode Result, BP204()
    AssertEquals BlueprintValidationSeverityInfo(), Result.Diagnostics.Item(1).Severity, "BP204 should be informational."
End Sub

Private Sub VerifyMissingBlueprintIdIsInvalid()
    Dim Blueprint As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Blueprint.Remove "blueprintId"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP001()
End Sub

Private Sub VerifyUnsupportedVersionIsInvalid()
    Dim Blueprint As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Blueprint("version") = "9.9"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP101()
End Sub

Private Sub VerifyApprovalConflictIsInvalid()
    Dim Blueprint As Object
    Dim Approval As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set Approval = Blueprint("approval")
    Approval("state") = "notApproved"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP201()
End Sub

Private Sub VerifyApprovedWithoutApproverIsInvalid()
    Dim Blueprint As Object
    Dim Approval As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set Approval = Blueprint("approval")
    Approval.Remove "approvedBy"
    Approval.Remove "approvedAt"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP202()
    AssertDiagnosticCode Result, BP203()
End Sub

Private Sub VerifyTargetErrorsAreInvalid()
    Dim Blueprint As Object
    Dim Target As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set Target = Blueprint("target")
    Target.Remove "kind"
    Target("name") = vbNullString

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP301()
    AssertDiagnosticCode Result, BP303()
End Sub

Private Sub VerifyModuleAndProcedureErrorsAreInvalid()
    Dim Blueprint As Object
    Dim ModuleInfo As Object
    Dim ProcedureInfo As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set ModuleInfo = FirstModule(Blueprint)
    ModuleInfo("name") = vbNullString
    Set ProcedureInfo = FirstProcedure(Blueprint)
    ProcedureInfo.Remove "kind"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP325()
    AssertDiagnosticCode Result, BP402()
End Sub

Private Sub VerifyReturnValueErrorsAreInvalid()
    Dim Blueprint As Object
    Dim ProcedureInfo As Object
    Dim ReturnValue As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set ProcedureInfo = FirstProcedure(Blueprint)
    ProcedureInfo("kind") = "Function"

    Set Result = ValidateBlueprint(Blueprint)
    AssertInvalidWithCode Result, BP421()

    Set Blueprint = CreateValidBlueprint()
    Set ProcedureInfo = FirstProcedure(Blueprint)
    ProcedureInfo("kind") = "Function"
    Set ReturnValue = CreateObject("Scripting.Dictionary")
    ProcedureInfo.Add "returnValue", ReturnValue

    Set Result = ValidateBlueprint(Blueprint)
    AssertInvalidWithCode Result, BP422()

    Set Blueprint = CreateValidBlueprint()
    Set ProcedureInfo = FirstProcedure(Blueprint)
    Set ReturnValue = CreateObject("Scripting.Dictionary")
    ReturnValue.Add "type", "String"
    ProcedureInfo.Add "returnValue", ReturnValue

    Set Result = ValidateBlueprint(Blueprint)
    AssertInvalidWithCode Result, BP423()
End Sub

Private Sub VerifyGenerationPolicyErrorsAreInvalid()
    Dim Blueprint As Object
    Dim Policy As Object
    Dim Result As BlueprintValidationResult

    Set Blueprint = CreateValidBlueprint()
    Set Policy = Blueprint("generationPolicy")
    Policy("allowOverwrite") = False
    Policy("encoding") = "shift_jis"
    Policy("missingDirectoryPolicy") = "create"

    Set Result = ValidateBlueprint(Blueprint)

    AssertInvalidWithCode Result, BP602()
    AssertDiagnosticCode Result, BP604()
    AssertDiagnosticCode Result, BP606()
End Sub

Private Function ValidateBlueprint(ByVal Blueprint As Object) As BlueprintValidationResult
    Dim Validator As BlueprintValidator

    Set Validator = New BlueprintValidator
    Set ValidateBlueprint = Validator.Validate(Blueprint)
End Function

Private Function CreateValidBlueprint() As Object
    Dim Blueprint As Object
    Dim Approval As Object
    Dim Target As Object
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Procedures As Collection
    Dim ProcedureInfo As Object
    Dim Policy As Object

    Set Blueprint = CreateObject("Scripting.Dictionary")
    Blueprint.Add "blueprintId", "BP-TEST-001"
    Blueprint.Add "version", "0.1"
    Blueprint.Add "status", "approved"

    Set Approval = CreateObject("Scripting.Dictionary")
    Approval.Add "state", "approved"
    Approval.Add "approvedBy", "reviewer"
    Approval.Add "approvedAt", "2026-08-20T00:00:00Z"
    Blueprint.Add "approval", Approval

    Set Target = CreateObject("Scripting.Dictionary")
    Target.Add "kind", "workbook"
    Target.Add "name", "SampleWorkbook"
    Blueprint.Add "target", Target

    Set ProcedureInfo = CreateObject("Scripting.Dictionary")
    ProcedureInfo.Add "name", "SayHello"
    ProcedureInfo.Add "kind", "Sub"
    ProcedureInfo.Add "visibility", "Public"
    ProcedureInfo.Add "responsibility", "Display a greeting message."

    Set Procedures = New Collection
    Procedures.Add ProcedureInfo

    Set ModuleInfo = CreateObject("Scripting.Dictionary")
    ModuleInfo.Add "name", "modHello"
    ModuleInfo.Add "kind", "standard"
    ModuleInfo.Add "responsibility", "Provide a simple greeting macro."
    ModuleInfo.Add "procedures", Procedures

    Set Modules = New Collection
    Modules.Add ModuleInfo
    Blueprint.Add "modules", Modules

    Set Policy = CreateObject("Scripting.Dictionary")
    Policy.Add "allowOverwrite", True
    Policy.Add "encoding", "utf-8"
    Policy.Add "missingDirectoryPolicy", "error"
    Blueprint.Add "generationPolicy", Policy

    Set CreateValidBlueprint = Blueprint
End Function

Private Function FirstModule(ByVal Blueprint As Object) As Object
    Set FirstModule = Blueprint("modules").Item(1)
End Function

Private Function FirstProcedure(ByVal Blueprint As Object) As Object
    Set FirstProcedure = FirstModule(Blueprint)("procedures").Item(1)
End Function

Private Sub AssertInvalidWithCode(ByVal Result As BlueprintValidationResult, ByVal Code As String)
    AssertEquals BlueprintValidationResultInvalid(), Result.ResultKind, "Result should be invalid."
    AssertFalse Result.Generatable, "Invalid Blueprint should not be generatable."
    AssertDiagnosticCode Result, Code
End Sub

Private Sub AssertDiagnosticCode(ByVal Result As BlueprintValidationResult, ByVal Code As String)
    AssertTrue Result.HasDiagnosticCode(Code), "Expected diagnostic code: " & Code
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppBlueprintValidatorTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppBlueprintValidatorTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppBlueprintValidatorTests", Message & " Expected=" & CStr(Expected) & " Actual=" & CStr(Actual)
    End If
End Sub
