VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BlueprintManifestDeriver"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: BlueprintManifestDeriver
' Layer: Application
' Responsibility: Derive existing Manifest content from a Validated Blueprint.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const BlueprintVersionV0_1 As String = "0.1"
Private Const TargetKindWorkbook As String = "workbook"
Private Const TargetKindAddIn As String = "addin"
Private Const ModuleKindStandard As String = "standard"
Private Const ModuleKindClass As String = "class"
Private Const ProcedureKindSub As String = "Sub"
Private Const ProcedureKindFunction As String = "Function"
Private Const VisibilityPublic As String = "Public"
Private Const VisibilityPrivate As String = "Private"

Public Function AppDeriveManifestContent(ByVal ValidatedBlueprint As Object, ByVal ValidationResult As BlueprintValidationResult) As String
    Dim Modules As Collection
    Dim ModuleInfo As Object
    Dim Content As String

    RequireValidatedBlueprint ValidatedBlueprint, ValidationResult
    RequireSupportedTarget ValidatedBlueprint("target")
    RequireSupportedGenerationPolicy ValidatedBlueprint("generationPolicy")

    Set Modules = ValidatedBlueprint("modules")
    If Modules.Count = 0 Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete Blueprint modules."
    End If

    Content = "# ModuleName,ModuleType,LayerName,TemplatePath"
    For Each ModuleInfo In Modules
        Content = Content & vbCrLf & FormatManifestItem(DeriveManifestItem(ModuleInfo))
    Next ModuleInfo

    AppDeriveManifestContent = Content
End Function

Private Sub RequireValidatedBlueprint(ByVal Blueprint As Object, ByVal ValidationResult As BlueprintValidationResult)
    If Blueprint Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: input is not a Validated Blueprint."
    End If
    If ValidationResult Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: validation result is required."
    End If
    If Not ValidationResult.Generatable Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: Blueprint is non-generatable."
    End If
    If ValidationResult.ErrorCount() > 0 Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: Blueprint validation diagnostics contain errors."
    End If

    RequireDictionaryText Blueprint, "blueprintId", "blueprintId"
    RequireDictionaryText Blueprint, "version", "version"
    If CStr(Blueprint("version")) <> BlueprintVersionV0_1 Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported Blueprint version."
    End If
    RequireDictionaryObject Blueprint, "target", "target"
    RequireDictionaryObject Blueprint, "modules", "modules"
    RequireDictionaryObject Blueprint, "generationPolicy", "generationPolicy"
End Sub

Private Function DeriveManifestItem(ByVal ModuleInfo As Object) As ManifestItem
    Dim Item As ManifestItem
    Dim ModuleName As String
    Dim ModuleType As String
    Dim LayerName As String

    If ModuleInfo Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete module."
    End If

    ModuleName = RequiredTextValue(ModuleInfo, "name", "modules[].name")
    ModuleType = ResolveModuleType(RequiredTextValue(ModuleInfo, "kind", "modules[].kind"))
    LayerName = RequiredTextValue(ModuleInfo, "layer", "modules[].layer")

    RequireSupportedLayer LayerName
    RequireSupportedProcedures ModuleInfo

    Set Item = New ManifestItem
    Item.InfInitialize ModuleName, ModuleType, LayerName, ResolveTemplatePath(LayerName, ModuleType)
    Set DeriveManifestItem = Item
End Function

Private Sub RequireSupportedTarget(ByVal Target As Object)
    Dim TargetKind As String

    TargetKind = RequiredTextValue(Target, "kind", "target.kind")
    Select Case TargetKind
        Case TargetKindWorkbook, TargetKindAddIn
            Exit Sub
    End Select

    Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported target kind."
End Sub

Private Sub RequireSupportedGenerationPolicy(ByVal Policy As Object)
    If Not HasKey(Policy, "allowOverwrite") Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete generation policy."
    End If
    If Not IsBooleanTrue(Policy("allowOverwrite")) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported generation policy."
    End If
    If LCase$(RequiredTextValue(Policy, "encoding", "generationPolicy.encoding")) <> "utf-8" Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported generation policy."
    End If
    If RequiredTextValue(Policy, "missingDirectoryPolicy", "generationPolicy.missingDirectoryPolicy") <> "error" Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported generation policy."
    End If
End Sub

Private Sub RequireSupportedLayer(ByVal LayerName As String)
    Select Case LayerName
        Case "Common", "Core", "Domain", "Application", "Infrastructure", "Presentation"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported layer."
End Sub

Private Sub RequireSupportedProcedures(ByVal ModuleInfo As Object)
    Dim Procedures As Collection
    Dim ProcedureInfo As Object

    RequireDictionaryObject ModuleInfo, "procedures", "modules[].procedures"
    Set Procedures = ModuleInfo("procedures")
    If Procedures.Count = 0 Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete procedures."
    End If

    For Each ProcedureInfo In Procedures
        RequireSupportedProcedure ProcedureInfo
    Next ProcedureInfo
End Sub

Private Sub RequireSupportedProcedure(ByVal ProcedureInfo As Object)
    Dim ProcedureKind As String
    Dim Visibility As String

    If ProcedureInfo Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete procedure."
    End If

    RequiredTextValue ProcedureInfo, "name", "procedures[].name"
    ProcedureKind = RequiredTextValue(ProcedureInfo, "kind", "procedures[].kind")
    Select Case ProcedureKind
        Case ProcedureKindSub, ProcedureKindFunction
        Case Else
            Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported procedure kind."
    End Select

    Visibility = RequiredTextValue(ProcedureInfo, "visibility", "procedures[].visibility")
    Select Case Visibility
        Case VisibilityPublic, VisibilityPrivate
        Case Else
            Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported procedure visibility."
    End Select

    RequiredTextValue ProcedureInfo, "responsibility", "procedures[].responsibility"
    If ProcedureKind = ProcedureKindFunction Then
        RequireDictionaryObject ProcedureInfo, "returnValue", "procedures[].returnValue"
        RequiredTextValue ProcedureInfo("returnValue"), "type", "returnValue.type"
    End If
End Sub

Private Function ResolveModuleType(ByVal ModuleKind As String) As String
    Select Case ModuleKind
        Case ModuleKindStandard
            ResolveModuleType = "StandardModule"
        Case ModuleKindClass
            ResolveModuleType = "ClassModule"
        Case Else
            Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: unsupported module kind."
    End Select
End Function

Private Function ResolveTemplatePath(ByVal LayerName As String, ByVal ModuleType As String) As String
    Dim TemplateFileName As String

    If ModuleType = "StandardModule" Then
        TemplateFileName = "ModuleTemplate.txt"
    ElseIf LayerName = "Domain" Then
        TemplateFileName = "DomainClassTemplate.txt"
    Else
        TemplateFileName = "ClassTemplate.txt"
    End If

    ResolveTemplatePath = BuildPathResolver.CombinePath( _
        BuildPathResolver.TemplatesDirectoryPath(), _
        TemplateFileName)
End Function

Private Function FormatManifestItem(ByVal Item As ManifestItem) As String
    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BlueprintManifestDeriver", "ManifestItem is required."
    End If
    If Not Item.InfValidate() Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete Manifest item."
    End If

    FormatManifestItem = _
        Item.InfGetModuleName() & "," & _
        Item.InfGetModuleType() & "," & _
        Item.InfGetLayerName() & "," & _
        Item.InfGetTemplatePath()
End Function

Private Sub RequireDictionaryText(ByVal Source As Object, ByVal KeyName As String, ByVal FieldPath As String)
    RequiredTextValue Source, KeyName, FieldPath
End Sub

Private Sub RequireDictionaryObject(ByVal Source As Object, ByVal KeyName As String, ByVal FieldPath As String)
    If Source Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete " & FieldPath & "."
    End If
    If Not HasKey(Source, KeyName) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete " & FieldPath & "."
    End If
    If Not IsObject(Source(KeyName)) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: ambiguous " & FieldPath & "."
    End If
End Sub

Private Function RequiredTextValue(ByVal Source As Object, ByVal KeyName As String, ByVal FieldPath As String) As String
    If Source Is Nothing Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete " & FieldPath & "."
    End If
    If Not HasKey(Source, KeyName) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete " & FieldPath & "."
    End If
    If IsObject(Source(KeyName)) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: ambiguous " & FieldPath & "."
    End If

    RequiredTextValue = Trim$(CStr(Source(KeyName)))
    If ComIsBlankText(RequiredTextValue) Then
        Err.Raise ComErrInvalidState, "BlueprintManifestDeriver", "Manifest derivation hard-stop: incomplete " & FieldPath & "."
    End If
End Function

Private Function HasKey(ByVal Source As Object, ByVal KeyName As String) As Boolean
    If Source Is Nothing Then Exit Function
    HasKey = Source.Exists(KeyName)
End Function

Private Function IsBooleanTrue(ByVal Value As Variant) As Boolean
    If VarType(Value) = vbBoolean Then
        IsBooleanTrue = CBool(Value)
    Else
        IsBooleanTrue = (LCase$(CStr(Value)) = "true")
    End If
End Function
