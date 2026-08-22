Option Explicit
Attribute VB_Name = "AppBlueprintManifestDeriverTests"

'=========================================================================
' Module: AppBlueprintManifestDeriverTests
' Layer: Application
' Responsibility: Focused tests for Manifest Derivation from Validated Blueprint.
' Dependencies: Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9330

Public Sub AppRunBlueprintManifestDeriverTests()
    VerifyValidatedBlueprintDerivesManifest
    VerifyDerivationIsDeterministic
    VerifyDerivationDoesNotAddDesignIntent
    VerifyIncompleteBlueprintIsRejected
    VerifyAmbiguousBlueprintIsRejected
    VerifyUnsupportedBlueprintIsRejected
    VerifyUnapprovedBlueprintIsRejected
    VerifyNonGeneratableBlueprintIsRejected
    VerifyDerivationFailureStopsBeforeGenerator
End Sub

Private Sub VerifyValidatedBlueprintDerivesManifest()
    Dim ManifestContent As String

    ManifestContent = DeriveManifest(CreateValidBlueprint(), CreateGeneratableResult())

    AssertTrue InStr(1, ManifestContent, "# ModuleName,ModuleType,LayerName,TemplatePath", vbBinaryCompare) > 0, "Manifest header should be derived."
    AssertTrue InStr(1, ManifestContent, "Subject,ClassModule,Domain,", vbBinaryCompare) > 0, "Manifest module entry should be derived."
    AssertTrue InStr(1, ManifestContent, "DomainClassTemplate.txt", vbBinaryCompare) > 0, "Manifest template path should be deterministic."
End Sub

Private Sub VerifyDerivationIsDeterministic()
    Dim Blueprint As Object
    Dim FirstManifest As String
    Dim SecondManifest As String

    Set Blueprint = CreateValidBlueprint()
    FirstManifest = DeriveManifest(Blueprint, CreateGeneratableResult())
    SecondManifest = DeriveManifest(Blueprint, CreateGeneratableResult())

    AssertEquals FirstManifest, SecondManifest, "Same Validated Blueprint should derive same Manifest."
End Sub

Private Sub VerifyDerivationDoesNotAddDesignIntent()
    Dim ManifestContent As String

    ManifestContent = DeriveManifest(CreateValidBlueprint(), CreateGeneratableResult())

    AssertTrue InStr(1, ManifestContent, "Teach domain scheduling intent", vbBinaryCompare) = 0, "Responsibility prose should not become Manifest behavior."
    AssertTrue InStr(1, ManifestContent, "SayHello", vbBinaryCompare) = 0, "Procedure intent should not add Manifest entries."
    AssertTrue InStr(1, ManifestContent, "UnlistedModule", vbBinaryCompare) = 0, "Unlisted modules should not be invented."
End Sub

Private Sub VerifyIncompleteBlueprintIsRejected()
    Dim Blueprint As Object
    Dim ErrorText As String

    Set Blueprint = CreateValidBlueprint()
    FirstModule(Blueprint).Remove "layer"

    AssertEquals vbNullString, TryDeriveManifest(Blueprint, CreateGeneratableResult(), ErrorText), "Incomplete Blueprint should not derive Manifest."
    AssertContains ErrorText, "incomplete", "Incomplete Blueprint should hard-stop."
End Sub

Private Sub VerifyAmbiguousBlueprintIsRejected()
    Dim Blueprint As Object
    Dim ErrorText As String
    Dim AmbiguousName As Object

    Set Blueprint = CreateValidBlueprint()
    Set AmbiguousName = CreateObject("Scripting.Dictionary")
    AmbiguousName.Add "value", "Subject"
    FirstModule(Blueprint).Remove "name"
    FirstModule(Blueprint).Add "name", AmbiguousName

    AssertEquals vbNullString, TryDeriveManifest(Blueprint, CreateGeneratableResult(), ErrorText), "Ambiguous Blueprint should not derive Manifest."
    AssertContains ErrorText, "ambiguous", "Ambiguous Blueprint should hard-stop."
End Sub

Private Sub VerifyUnsupportedBlueprintIsRejected()
    Dim Blueprint As Object
    Dim ErrorText As String

    Set Blueprint = CreateValidBlueprint()
    FirstModule(Blueprint)("kind") = "form"

    AssertEquals vbNullString, TryDeriveManifest(Blueprint, CreateGeneratableResult(), ErrorText), "Unsupported Blueprint should not derive Manifest."
    AssertContains ErrorText, "unsupported", "Unsupported Blueprint should hard-stop."
End Sub

Private Sub VerifyUnapprovedBlueprintIsRejected()
    Dim ErrorText As String

    AssertEquals vbNullString, TryDeriveManifest(CreateValidBlueprint(), CreateNotGeneratableResult(), ErrorText), "Unapproved Blueprint should not derive Manifest."
    AssertContains ErrorText, "non-generatable", "Unapproved Blueprint should hard-stop through Validator result."
End Sub

Private Sub VerifyNonGeneratableBlueprintIsRejected()
    Dim ErrorText As String

    AssertEquals vbNullString, TryDeriveManifest(CreateValidBlueprint(), CreateInvalidResult(), ErrorText), "Invalid Blueprint should not derive Manifest."
    AssertContains ErrorText, "non-generatable", "Invalid Blueprint should hard-stop through Validator result."
End Sub

Private Sub VerifyDerivationFailureStopsBeforeGenerator()
    Dim Blueprint As Object
    Dim ErrorText As String
    Dim ManifestContent As String

    Set Blueprint = CreateValidBlueprint()
    FirstModule(Blueprint).Remove "layer"

    ManifestContent = TryDeriveManifest(Blueprint, CreateGeneratableResult(), ErrorText)

    AssertEquals vbNullString, ManifestContent, "Failed derivation should produce no Generator input."
    AssertContains ErrorText, "Manifest derivation hard-stop", "Derivation failure should remain before Generator execution."
End Sub

Private Function DeriveManifest(ByVal Blueprint As Object, ByVal Result As BlueprintValidationResult) As String
    Dim Deriver As BlueprintManifestDeriver

    Set Deriver = New BlueprintManifestDeriver
    DeriveManifest = Deriver.AppDeriveManifestContent(Blueprint, Result)
End Function

Private Function TryDeriveManifest(ByVal Blueprint As Object, ByVal Result As BlueprintValidationResult, ByRef ErrorText As String) As String
    On Error GoTo ErrHandler

    TryDeriveManifest = DeriveManifest(Blueprint, Result)
    ErrorText = vbNullString
    Exit Function

ErrHandler:
    ErrorText = Err.Description
    TryDeriveManifest = vbNullString
    Err.Clear
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
    ModuleInfo.Add "name", "Subject"
    ModuleInfo.Add "kind", "class"
    ModuleInfo.Add "layer", "Domain"
    ModuleInfo.Add "responsibility", "Teach domain scheduling intent."
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

Private Function CreateGeneratableResult() As BlueprintValidationResult
    Set CreateGeneratableResult = CreateResult(BlueprintValidationResultValidGeneratable(), True, New Collection)
End Function

Private Function CreateNotGeneratableResult() As BlueprintValidationResult
    Dim Diagnostics As Collection
    Dim Diagnostic As BlueprintValidationDiagnostic

    Set Diagnostics = New Collection
    Set Diagnostic = New BlueprintValidationDiagnostic
    Diagnostic.AppInitialize BP204(), BlueprintCategoryGenerationIneligible(), BlueprintValidationSeverityInfo(), "status"
    Diagnostics.Add Diagnostic

    Set CreateNotGeneratableResult = CreateResult(BlueprintValidationResultValidNotGeneratable(), False, Diagnostics)
End Function

Private Function CreateInvalidResult() As BlueprintValidationResult
    Dim Diagnostics As Collection
    Dim Diagnostic As BlueprintValidationDiagnostic

    Set Diagnostics = New Collection
    Set Diagnostic = New BlueprintValidationDiagnostic
    Diagnostic.AppInitialize BP101(), BlueprintCategoryUnsupportedEnumValue(), BlueprintValidationSeverityError(), "version"
    Diagnostics.Add Diagnostic

    Set CreateInvalidResult = CreateResult(BlueprintValidationResultInvalid(), False, Diagnostics)
End Function

Private Function CreateResult(ByVal ResultKind As String, ByVal Generatable As Boolean, ByVal Diagnostics As Collection) As BlueprintValidationResult
    Set CreateResult = New BlueprintValidationResult
    CreateResult.AppInitialize ResultKind, Generatable, Diagnostics
End Function

Private Function FirstModule(ByVal Blueprint As Object) As Object
    Set FirstModule = Blueprint("modules").Item(1)
End Function

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppBlueprintManifestDeriverTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As String, ByVal Actual As String, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppBlueprintManifestDeriverTests", Message & " Expected=" & Expected & " Actual=" & Actual
    End If
End Sub
