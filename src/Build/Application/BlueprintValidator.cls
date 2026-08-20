VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BlueprintValidator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: BlueprintValidator
' Layer: Application
' Responsibility: Candidate B Blueprint v0.1 validation.
' Dependencies: Application
'=========================================================================

Private Const BlueprintVersionV0_1 As String = "0.1"
Private Const StatusDraft As String = "draft"
Private Const StatusReview As String = "review"
Private Const StatusApproved As String = "approved"
Private Const StatusRejected As String = "rejected"
Private Const StatusSuperseded As String = "superseded"
Private Const ApprovalNotApproved As String = "notApproved"
Private Const ApprovalApproved As String = "approved"
Private Const ApprovalRejected As String = "rejected"
Private Const TargetKindWorkbook As String = "workbook"
Private Const TargetKindAddIn As String = "addin"
Private Const ModuleKindStandard As String = "standard"
Private Const ModuleKindClass As String = "class"
Private Const ProcedureKindSub As String = "Sub"
Private Const ProcedureKindFunction As String = "Function"
Private Const VisibilityPublic As String = "Public"
Private Const VisibilityPrivate As String = "Private"

Public Function Validate(ByVal ParsedBlueprint As Object) As BlueprintValidationResult
    Dim Diagnostics As Collection
    Dim ResultKind As String
    Dim Generatable As Boolean

    Set Diagnostics = New Collection

    If ParsedBlueprint Is Nothing Then
        AddError Diagnostics, BP001(), BlueprintCategoryMissingRequiredField(), "blueprintId"
        AddError Diagnostics, BP002(), BlueprintCategoryMissingRequiredField(), "version"
        AddError Diagnostics, BP003(), BlueprintCategoryMissingRequiredField(), "status"
        AddError Diagnostics, BP004(), BlueprintCategoryMissingRequiredField(), "approval"
        AddError Diagnostics, BP005(), BlueprintCategoryMissingRequiredField(), "target"
        AddError Diagnostics, BP006(), BlueprintCategoryMissingRequiredField(), "modules"
        AddError Diagnostics, BP007(), BlueprintCategoryMissingRequiredField(), "generationPolicy"
    Else
        ValidateTopLevel ParsedBlueprint, Diagnostics
        ValidateVersion ParsedBlueprint, Diagnostics
        ValidateStatus ParsedBlueprint, Diagnostics
        ValidateApproval ParsedBlueprint, Diagnostics
        ValidateStatusApprovalConsistency ParsedBlueprint, Diagnostics
        ValidateTarget ParsedBlueprint, Diagnostics
        ValidateModules ParsedBlueprint, Diagnostics
        ValidateGenerationPolicy ParsedBlueprint, Diagnostics
    End If

    If ContainsError(Diagnostics) Then
        ResultKind = BlueprintValidationResultInvalid()
        Generatable = False
    ElseIf IsApprovedForGeneration(ParsedBlueprint) Then
        ResultKind = BlueprintValidationResultValidGeneratable()
        Generatable = True
    Else
        ResultKind = BlueprintValidationResultValidNotGeneratable()
        Generatable = False
        AddDiagnostic Diagnostics, BP204(), BlueprintCategoryGenerationIneligible(), BlueprintValidationSeverityInfo(), "status"
    End If

    Set Validate = CreateResult(ResultKind, Generatable, Diagnostics)
End Function

Private Sub ValidateTopLevel(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    If Not HasKey(Blueprint, "blueprintId") Then AddError Diagnostics, BP001(), BlueprintCategoryMissingRequiredField(), "blueprintId"
    If Not HasKey(Blueprint, "version") Then AddError Diagnostics, BP002(), BlueprintCategoryMissingRequiredField(), "version"
    If Not HasKey(Blueprint, "status") Then AddError Diagnostics, BP003(), BlueprintCategoryMissingRequiredField(), "status"
    If Not HasKey(Blueprint, "approval") Then AddError Diagnostics, BP004(), BlueprintCategoryMissingRequiredField(), "approval"
    If Not HasKey(Blueprint, "target") Then AddError Diagnostics, BP005(), BlueprintCategoryMissingRequiredField(), "target"
    If Not HasKey(Blueprint, "modules") Then AddError Diagnostics, BP006(), BlueprintCategoryMissingRequiredField(), "modules"
    If Not HasKey(Blueprint, "generationPolicy") Then AddError Diagnostics, BP007(), BlueprintCategoryMissingRequiredField(), "generationPolicy"
End Sub

Private Sub ValidateVersion(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    If Not HasKey(Blueprint, "version") Then Exit Sub
    If CStr(Blueprint("version")) <> BlueprintVersionV0_1 Then
        AddError Diagnostics, BP101(), BlueprintCategoryUnsupportedEnumValue(), "version"
    End If
End Sub

Private Sub ValidateStatus(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim StatusValue As String

    If Not HasKey(Blueprint, "status") Then Exit Sub
    StatusValue = CStr(Blueprint("status"))
    If Not IsAllowedValue(StatusValue, Array(StatusDraft, StatusReview, StatusApproved, StatusRejected, StatusSuperseded), False) Then
        AddError Diagnostics, BP102(), BlueprintCategoryUnsupportedEnumValue(), "status"
    End If
End Sub

Private Sub ValidateApproval(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim Approval As Object
    Dim StateValue As String

    If Not HasObject(Blueprint, "approval") Then Exit Sub
    Set Approval = Blueprint("approval")

    If Not HasKey(Approval, "state") Then
        AddError Diagnostics, BP008(), BlueprintCategoryMissingRequiredField(), "approval.state"
        Exit Sub
    End If

    StateValue = CStr(Approval("state"))
    If Not IsAllowedValue(StateValue, Array(ApprovalNotApproved, ApprovalApproved, ApprovalRejected), False) Then
        AddError Diagnostics, BP103(), BlueprintCategoryUnsupportedEnumValue(), "approval.state"
        Exit Sub
    End If

    If StateValue = ApprovalApproved Then
        If Not HasText(Approval, "approvedBy") Then AddError Diagnostics, BP202(), BlueprintCategoryApprovalConflict(), "approval.approvedBy"
        If Not HasText(Approval, "approvedAt") Then AddError Diagnostics, BP203(), BlueprintCategoryApprovalConflict(), "approval.approvedAt"
    End If
End Sub

Private Sub ValidateStatusApprovalConsistency(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim StatusValue As String
    Dim Approval As Object
    Dim StateValue As String

    If Not HasKey(Blueprint, "status") Then Exit Sub
    If Not HasObject(Blueprint, "approval") Then Exit Sub
    Set Approval = Blueprint("approval")
    If Not HasKey(Approval, "state") Then Exit Sub

    StatusValue = CStr(Blueprint("status"))
    StateValue = CStr(Approval("state"))
    If Not IsAllowedValue(StatusValue, Array(StatusDraft, StatusReview, StatusApproved, StatusRejected, StatusSuperseded), False) Then Exit Sub
    If Not IsAllowedValue(StateValue, Array(ApprovalNotApproved, ApprovalApproved, ApprovalRejected), False) Then Exit Sub

    Select Case StatusValue
        Case StatusDraft, StatusReview
            If StateValue <> ApprovalNotApproved Then AddError Diagnostics, BP201(), BlueprintCategoryApprovalConflict(), "approval.state"
        Case StatusApproved
            If StateValue <> ApprovalApproved Then AddError Diagnostics, BP201(), BlueprintCategoryApprovalConflict(), "approval.state"
        Case StatusRejected
            If StateValue <> ApprovalRejected Then AddError Diagnostics, BP201(), BlueprintCategoryApprovalConflict(), "approval.state"
    End Select
End Sub

Private Sub ValidateTarget(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim Target As Object

    If Not HasObject(Blueprint, "target") Then Exit Sub
    Set Target = Blueprint("target")

    If Not HasKey(Target, "kind") Then
        AddError Diagnostics, BP301(), BlueprintCategoryInvalidTarget(), "target.kind"
    ElseIf Not IsAllowedValue(CStr(Target("kind")), Array(TargetKindWorkbook, TargetKindAddIn), False) Then
        AddError Diagnostics, BP104(), BlueprintCategoryUnsupportedEnumValue(), "target.kind"
    End If

    If Not HasKey(Target, "name") Then
        AddError Diagnostics, BP302(), BlueprintCategoryInvalidTarget(), "target.name"
    ElseIf Not HasText(Target, "name") Then
        AddError Diagnostics, BP303(), BlueprintCategoryInvalidTarget(), "target.name"
    End If
End Sub

Private Sub ValidateModules(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim Modules As Collection
    Dim ModuleValue As Variant
    Dim ModuleIndex As Long

    If Not HasObject(Blueprint, "modules") Then Exit Sub
    Set Modules = Blueprint("modules")

    If Modules.Count = 0 Then
        AddError Diagnostics, BP321(), BlueprintCategoryInvalidModule(), "modules"
        Exit Sub
    End If

    ModuleIndex = 0
    For Each ModuleValue In Modules
        ModuleIndex = ModuleIndex + 1
        If IsObject(ModuleValue) Then
            ValidateModule ModuleValue, ModuleIndex, Diagnostics
        Else
            AddError Diagnostics, BP008(), BlueprintCategoryMissingRequiredField(), "modules[" & CStr(ModuleIndex - 1) & "]"
        End If
    Next ModuleValue
End Sub

Private Sub ValidateModule(ByVal ModuleInfo As Object, ByVal ModuleIndex As Long, ByVal Diagnostics As Collection)
    Dim Procedures As Collection
    Dim ProcedureValue As Variant
    Dim ProcedureIndex As Long
    Dim Prefix As String

    Prefix = "modules[" & CStr(ModuleIndex - 1) & "]"

    If Not HasKey(ModuleInfo, "name") Then
        AddError Diagnostics, BP322(), BlueprintCategoryInvalidModule(), Prefix & ".name"
    ElseIf Not HasText(ModuleInfo, "name") Then
        AddError Diagnostics, BP325(), BlueprintCategoryInvalidModule(), Prefix & ".name"
    End If

    If Not HasKey(ModuleInfo, "kind") Then
        AddError Diagnostics, BP323(), BlueprintCategoryInvalidModule(), Prefix & ".kind"
    ElseIf Not IsAllowedValue(CStr(ModuleInfo("kind")), Array(ModuleKindStandard, ModuleKindClass), False) Then
        AddError Diagnostics, BP105(), BlueprintCategoryUnsupportedEnumValue(), Prefix & ".kind"
    End If

    If Not HasText(ModuleInfo, "responsibility") Then AddError Diagnostics, BP324(), BlueprintCategoryInvalidModule(), Prefix & ".responsibility"

    If Not HasObject(ModuleInfo, "procedures") Then
        AddError Diagnostics, BP008(), BlueprintCategoryMissingRequiredField(), Prefix & ".procedures"
        Exit Sub
    End If

    Set Procedures = ModuleInfo("procedures")
    ProcedureIndex = 0
    For Each ProcedureValue In Procedures
        ProcedureIndex = ProcedureIndex + 1
        If IsObject(ProcedureValue) Then
            ValidateProcedure ProcedureValue, Prefix & ".procedures[" & CStr(ProcedureIndex - 1) & "]", Diagnostics
        Else
            AddError Diagnostics, BP008(), BlueprintCategoryMissingRequiredField(), Prefix & ".procedures[" & CStr(ProcedureIndex - 1) & "]"
        End If
    Next ProcedureValue
End Sub

Private Sub ValidateProcedure(ByVal ProcedureInfo As Object, ByVal Prefix As String, ByVal Diagnostics As Collection)
    If Not HasKey(ProcedureInfo, "name") Then
        AddError Diagnostics, BP401(), BlueprintCategoryInvalidProcedure(), Prefix & ".name"
    ElseIf Not HasText(ProcedureInfo, "name") Then
        AddError Diagnostics, BP405(), BlueprintCategoryInvalidProcedure(), Prefix & ".name"
    End If

    If Not HasKey(ProcedureInfo, "kind") Then
        AddError Diagnostics, BP402(), BlueprintCategoryInvalidProcedure(), Prefix & ".kind"
    ElseIf Not IsAllowedValue(CStr(ProcedureInfo("kind")), Array(ProcedureKindSub, ProcedureKindFunction), False) Then
        AddError Diagnostics, BP106(), BlueprintCategoryUnsupportedEnumValue(), Prefix & ".kind"
    End If

    If Not HasKey(ProcedureInfo, "visibility") Then
        AddError Diagnostics, BP403(), BlueprintCategoryInvalidProcedure(), Prefix & ".visibility"
    ElseIf Not IsAllowedValue(CStr(ProcedureInfo("visibility")), Array(VisibilityPublic, VisibilityPrivate), False) Then
        AddError Diagnostics, BP107(), BlueprintCategoryUnsupportedEnumValue(), Prefix & ".visibility"
    End If

    If Not HasText(ProcedureInfo, "responsibility") Then AddError Diagnostics, BP404(), BlueprintCategoryInvalidProcedure(), Prefix & ".responsibility"

    If HasKey(ProcedureInfo, "kind") Then
        Select Case CStr(ProcedureInfo("kind"))
            Case ProcedureKindFunction
                ValidateFunctionReturnValue ProcedureInfo, Prefix, Diagnostics
            Case ProcedureKindSub
                If HasObject(ProcedureInfo, "returnValue") Then AddError Diagnostics, BP423(), BlueprintCategoryInvalidReturnValue(), Prefix & ".returnValue"
        End Select
    End If
End Sub

Private Sub ValidateFunctionReturnValue(ByVal ProcedureInfo As Object, ByVal Prefix As String, ByVal Diagnostics As Collection)
    Dim ReturnValue As Object

    If Not HasObject(ProcedureInfo, "returnValue") Then
        AddError Diagnostics, BP421(), BlueprintCategoryInvalidReturnValue(), Prefix & ".returnValue"
        Exit Sub
    End If

    Set ReturnValue = ProcedureInfo("returnValue")
    If Not HasText(ReturnValue, "type") Then AddError Diagnostics, BP422(), BlueprintCategoryInvalidReturnValue(), Prefix & ".returnValue.type"
End Sub

Private Sub ValidateGenerationPolicy(ByVal Blueprint As Object, ByVal Diagnostics As Collection)
    Dim Policy As Object

    If Not HasObject(Blueprint, "generationPolicy") Then Exit Sub
    Set Policy = Blueprint("generationPolicy")

    If Not HasKey(Policy, "allowOverwrite") Then
        AddError Diagnostics, BP601(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.allowOverwrite"
    ElseIf Not IsBooleanTrue(Policy("allowOverwrite")) Then
        AddError Diagnostics, BP602(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.allowOverwrite"
    End If

    If Not HasKey(Policy, "encoding") Then
        AddError Diagnostics, BP603(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.encoding"
    ElseIf LCase$(CStr(Policy("encoding"))) <> "utf-8" Then
        AddError Diagnostics, BP604(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.encoding"
    End If

    If Not HasKey(Policy, "missingDirectoryPolicy") Then
        AddError Diagnostics, BP605(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.missingDirectoryPolicy"
    ElseIf CStr(Policy("missingDirectoryPolicy")) <> "error" Then
        AddError Diagnostics, BP606(), BlueprintCategoryInvalidGenerationPolicy(), "generationPolicy.missingDirectoryPolicy"
    End If
End Sub

Private Function IsApprovedForGeneration(ByVal Blueprint As Object) As Boolean
    If Blueprint Is Nothing Then Exit Function
    If Not HasObject(Blueprint, "approval") Then Exit Function
    If Not HasKey(Blueprint, "status") Then Exit Function
    If Not HasKey(Blueprint("approval"), "state") Then Exit Function

    IsApprovedForGeneration = (CStr(Blueprint("status")) = StatusApproved _
        And CStr(Blueprint("approval")("state")) = ApprovalApproved)
End Function

Private Function HasKey(ByVal Source As Object, ByVal KeyName As String) As Boolean
    If Source Is Nothing Then Exit Function
    HasKey = Source.Exists(KeyName)
End Function

Private Function HasObject(ByVal Source As Object, ByVal KeyName As String) As Boolean
    If Not HasKey(Source, KeyName) Then Exit Function
    HasObject = IsObject(Source(KeyName))
End Function

Private Function HasText(ByVal Source As Object, ByVal KeyName As String) As Boolean
    If Not HasKey(Source, KeyName) Then Exit Function
    HasText = (Len(Trim$(CStr(Source(KeyName)))) > 0)
End Function

Private Function IsAllowedValue(ByVal Value As String, ByVal AllowedValues As Variant, ByVal IgnoreCase As Boolean) As Boolean
    Dim Item As Variant
    Dim CompareMode As VbCompareMethod

    CompareMode = vbBinaryCompare
    If IgnoreCase Then CompareMode = vbTextCompare

    For Each Item In AllowedValues
        If StrComp(Value, CStr(Item), CompareMode) = 0 Then
            IsAllowedValue = True
            Exit Function
        End If
    Next Item
End Function

Private Function IsBooleanTrue(ByVal Value As Variant) As Boolean
    If VarType(Value) = vbBoolean Then
        IsBooleanTrue = CBool(Value)
    Else
        IsBooleanTrue = (LCase$(CStr(Value)) = "true")
    End If
End Function

Private Sub AddError(ByVal Diagnostics As Collection, ByVal Code As String, ByVal Category As String, ByVal FieldPath As String)
    AddDiagnostic Diagnostics, Code, Category, BlueprintValidationSeverityError(), FieldPath
End Sub

Private Sub AddDiagnostic(ByVal Diagnostics As Collection, ByVal Code As String, ByVal Category As String, ByVal Severity As String, ByVal FieldPath As String)
    Dim Diagnostic As BlueprintValidationDiagnostic

    Set Diagnostic = New BlueprintValidationDiagnostic
    Diagnostic.AppInitialize Code, Category, Severity, FieldPath
    Diagnostics.Add Diagnostic
End Sub

Private Function ContainsError(ByVal Diagnostics As Collection) As Boolean
    Dim Diagnostic As BlueprintValidationDiagnostic

    For Each Diagnostic In Diagnostics
        If LCase$(Diagnostic.Severity) = BlueprintValidationSeverityError() Then
            ContainsError = True
            Exit Function
        End If
    Next Diagnostic
End Function

Private Function CreateResult(ByVal ResultKind As String, ByVal Generatable As Boolean, ByVal Diagnostics As Collection) As BlueprintValidationResult
    Set CreateResult = New BlueprintValidationResult
    CreateResult.AppInitialize ResultKind, Generatable, Diagnostics
End Function
