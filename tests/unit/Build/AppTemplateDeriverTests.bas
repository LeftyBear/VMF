Option Explicit
Attribute VB_Name = "AppTemplateDeriverTests"

'=========================================================================
' Module: AppTemplateDeriverTests
' Layer: Application
' Responsibility: Focused tests for exact PG-01 Template Derivation.
' Dependencies: Application, Infrastructure
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9380

Public Sub AppRunTemplateDeriverTests()
    VerifyApprovedMappings
    VerifyOrderAndDeterminism
    VerifyPathNormalization
    VerifyAbsolutePathSuffix
    VerifyPathFailures
    VerifyCollectionFailures
    VerifyInvalidItems
    VerifyUnsupportedLayer
    VerifyDomainModuleTemplateRejected
    VerifyDuplicateModuleName
    VerifyAccumulatedDiagnosticsAndOrdering
    VerifyNoCrossItemDeduplication
    VerifyAtomicFailure
    VerifyUninitializedContracts
    VerifyResultInvariantRejection
End Sub

Private Sub VerifyApprovedMappings()
    Dim Layers As Variant
    Dim Index As Long
    Dim Result As AppTemplateDerivationResult
    Dim Item As AppTemplateDerivationItem

    Set Result = DeriveOne(CreateManifest("Standard", "StandardModule", "Domain", "templates/ModuleTemplate.txt"))
    AssertTrue Result.IsSuccess, "StandardModule mapping should succeed."
    Set Item = Result.Items.Item(1)
    AssertEquals "ModuleTemplate", Item.TemplateKey, "StandardModule template key."
    AssertEquals "StandardModuleTemplate", Item.TemplateRole, "StandardModule template role."
    AssertEquals "TD-P5-02-STANDARD-MODULE", Item.SelectionRuleId, "StandardModule rule."

    Set Result = DeriveOne(CreateManifest("DomainClass", "ClassModule", "Domain", "templates/DomainClassTemplate.txt"))
    Set Item = Result.Items.Item(1)
    AssertEquals "DomainClassTemplate", Item.TemplateKey, "Domain class template key."
    AssertEquals "TD-P5-02-DOMAIN-CLASS", Item.SelectionRuleId, "Domain class rule."

    Layers = Array("Common", "Core", "Application", "Infrastructure", "Presentation")
    For Index = LBound(Layers) To UBound(Layers)
        Set Result = DeriveOne(CreateManifest("Class" & CStr(Index), "ClassModule", CStr(Layers(Index)), "templates/ClassTemplate.txt"))
        Set Item = Result.Items.Item(1)
        AssertEquals "ClassTemplate", Item.TemplateKey, "Non-Domain class template key."
        AssertEquals "TD-P5-02-CLASS", Item.SelectionRuleId, "Non-Domain class rule."
        AssertEquals "ModuleType=ClassModule; LayerName=" & CStr(Layers(Index)) & "; RuleId=TD-P5-02-CLASS", Item.DerivationReason, "Derivation reason."
    Next Index
End Sub

Private Sub VerifyOrderAndDeterminism()
    Dim Items As Collection
    Dim FirstResult As AppTemplateDerivationResult
    Dim SecondResult As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add CreateManifest("First", "StandardModule", "Common", "templates/ModuleTemplate.txt")
    Items.Add CreateManifest("Second", "ClassModule", "Domain", "templates/DomainClassTemplate.txt")
    Set FirstResult = Derive(Items)
    Set SecondResult = Derive(Items)

    AssertEquals 2, FirstResult.Items.Count, "One output per input."
    AssertEquals "First", FirstResult.Items.Item(1).ModuleName, "First item order."
    AssertEquals "Second", FirstResult.Items.Item(2).ModuleName, "Second item order."
    AssertEquals FirstResult.Items.Item(1).DerivationReason, SecondResult.Items.Item(1).DerivationReason, "Repeated derivation should be deterministic."
End Sub

Private Sub VerifyPathNormalization()
    Dim Result As AppTemplateDerivationResult

    Set Result = DeriveOne(CreateManifest("Slash", "StandardModule", "Common", " templates\\ModuleTemplate.txt "))
    AssertTrue Result.IsSuccess, "Slash normalization should match."
    AssertEquals "templates/ModuleTemplate.txt", Result.Items.Item(1).TemplatePath, "Output path remains canonical."

    Set Result = DeriveOne(CreateManifest("Dots", "ClassModule", "Common", "templates/parts/.././ClassTemplate.txt/"))
    AssertTrue Result.IsSuccess, "Dot-segment normalization should match."
End Sub

Private Sub VerifyAbsolutePathSuffix()
    Dim Result As AppTemplateDerivationResult

    Set Result = DeriveOne(CreateManifest("Absolute", "ClassModule", "Domain", "C:\repo\templates\DomainClassTemplate.txt"))
    AssertTrue Result.IsSuccess, "Absolute canonical suffix should match."
End Sub

Private Sub VerifyPathFailures()
    Dim Result As AppTemplateDerivationResult

    Set Result = DeriveOne(CreateManifest("BaseOnly", "ClassModule", "Common", "ClassTemplate.txt"))
    AssertDiagnostic Result, "TD012", "Basename-only path should conflict."

    Set Result = DeriveOne(CreateManifest("Escape", "StandardModule", "Common", "../templates/ModuleTemplate.txt"))
    AssertDiagnostic Result, "TD011", "Root escape should be rejected."

    Set Result = DeriveOne(CreateManifest("Conflict", "ClassModule", "Domain", "templates/ClassTemplate.txt"))
    AssertDiagnostic Result, "TD012", "Conflicting path should be rejected."
End Sub

Private Sub VerifyCollectionFailures()
    Dim Missing As Collection
    Dim EmptyItems As Collection
    Dim Result As AppTemplateDerivationResult

    Set Result = Derive(Missing)
    AssertDiagnostic Result, "TD001", "Nothing collection should fail."

    Set EmptyItems = New Collection
    Set Result = Derive(EmptyItems)
    AssertDiagnostic Result, "TD002", "Empty collection should fail."
End Sub

Private Sub VerifyInvalidItems()
    Dim Items As Collection
    Dim MissingManifest As ManifestItem
    Dim InvalidManifest As ManifestItem
    Dim Result As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add MissingManifest
    Set Result = Derive(Items)
    AssertDiagnostic Result, "TD003", "Nothing ManifestItem should fail."

    Set Items = New Collection
    Items.Add New Collection
    Set Result = Derive(Items)
    AssertDiagnostic Result, "TD003", "Non-ManifestItem should fail."

    Set Items = New Collection
    Set InvalidManifest = New ManifestItem
    Items.Add InvalidManifest
    Set Result = Derive(Items)
    AssertDiagnostic Result, "TD004", "Invalid ManifestItem state should fail at the boundary."
End Sub

Private Sub VerifyUnsupportedLayer()
    Dim Result As AppTemplateDerivationResult

    Set Result = DeriveOne(CreateManifest("UnknownLayer", "ClassModule", "Unknown", "templates/ClassTemplate.txt"))
    AssertDiagnostic Result, "TD009", "Unsupported layer should fail."
    AssertFalse Result.HasDiagnosticCode("TD014"), "Dependent rule check should be skipped."
End Sub

Private Sub VerifyDomainModuleTemplateRejected()
    Dim Result As AppTemplateDerivationResult

    Set Result = DeriveOne(CreateManifest("Forbidden", "StandardModule", "Domain", "templates/DomainModuleTemplate.txt"))
    AssertDiagnostic Result, "TD013", "DomainModuleTemplate must be rejected."
End Sub

Private Sub VerifyDuplicateModuleName()
    Dim Items As Collection
    Dim Result As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add CreateManifest("Duplicate", "StandardModule", "Common", "templates/ModuleTemplate.txt")
    Items.Add CreateManifest("Duplicate", "ClassModule", "Domain", "templates/DomainClassTemplate.txt")
    Set Result = Derive(Items)
    AssertDiagnostic Result, "TD016", "Duplicate module identity should fail."
    AssertEquals 2, Result.Diagnostics.Item(1).ItemIndex, "Duplicate diagnostic belongs to the later item."
End Sub

Private Sub VerifyAccumulatedDiagnosticsAndOrdering()
    Dim Items As Collection
    Dim Result As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add CreateManifest("Repeated", "ClassModule", "Unknown", "../ClassTemplate.txt")
    Items.Add CreateManifest("Repeated", "ClassModule", "Domain", "templates/ClassTemplate.txt")
    Set Result = Derive(Items)

    AssertFalse Result.IsSuccess, "Multiple independent failures should fail atomically."
    AssertTrue Result.Diagnostics.Count >= 4, "Independent diagnostics should accumulate."
    AssertEquals "TD009", Result.Diagnostics.Item(1).Code, "Per-item validation order should be stable."
    AssertEquals "TD011", Result.Diagnostics.Item(2).Code, "Path diagnostic should follow layer diagnostic."
    AssertEquals 1, Result.Diagnostics.Item(1).SortOrder, "SortOrder starts at one."
    AssertEquals Result.Diagnostics.Count, Result.Diagnostics.Item(Result.Diagnostics.Count).SortOrder, "SortOrder is sequential."
End Sub

Private Sub VerifyNoCrossItemDeduplication()
    Dim Items As Collection
    Dim Result As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add CreateManifest("One", "ClassModule", "Unknown", "templates/ClassTemplate.txt")
    Items.Add CreateManifest("Two", "ClassModule", "Unknown", "templates/ClassTemplate.txt")
    Set Result = Derive(Items)
    AssertEquals 2, CountDiagnostic(Result, "TD009"), "Same diagnostic on different items must be retained."
End Sub

Private Sub VerifyAtomicFailure()
    Dim Items As Collection
    Dim Result As AppTemplateDerivationResult

    Set Items = New Collection
    Items.Add CreateManifest("Valid", "StandardModule", "Common", "templates/ModuleTemplate.txt")
    Items.Add CreateManifest("Invalid", "ClassModule", "Domain", "templates/ClassTemplate.txt")
    Set Result = Derive(Items)
    AssertFalse Result.IsSuccess, "Later failure should fail the whole operation."
    AssertEquals 0, Result.Items.Count, "Failure must expose no partial output."
End Sub

Private Sub VerifyUninitializedContracts()
    Dim Result As AppTemplateDerivationResult
    Dim Item As AppTemplateDerivationItem
    Dim Diagnostic As AppTemplateDerivationDiagnostic

    Set Result = New AppTemplateDerivationResult
    AssertAccessFails Result, "Result"
    Set Item = New AppTemplateDerivationItem
    AssertAccessFails Item, "Item"
    Set Diagnostic = New AppTemplateDerivationDiagnostic
    AssertAccessFails Diagnostic, "Diagnostic"
End Sub

Private Sub VerifyResultInvariantRejection()
    Dim Result As AppTemplateDerivationResult
    Dim Items As Collection
    Dim Diagnostics As Collection
    Dim ErrorText As String

    Set Result = New AppTemplateDerivationResult
    Set Items = New Collection
    Set Diagnostics = New Collection
    On Error Resume Next
    Result.AppInitialize False, Items, Diagnostics
    ErrorText = Err.Description
    Err.Clear
    On Error GoTo 0
    AssertContains ErrorText, "inconsistent", "Failure without diagnostics should be rejected."
End Sub

Private Function CreateManifest(ByVal ModuleName As String, ByVal ModuleType As String, ByVal LayerName As String, ByVal TemplatePath As String) As ManifestItem
    Set CreateManifest = New ManifestItem
    CreateManifest.InfInitialize ModuleName, ModuleType, LayerName, TemplatePath
End Function

Private Function DeriveOne(ByVal Item As ManifestItem) As AppTemplateDerivationResult
    Dim Items As Collection
    Set Items = New Collection
    Items.Add Item
    Set DeriveOne = Derive(Items)
End Function

Private Function Derive(ByVal Items As Collection) As AppTemplateDerivationResult
    Dim Deriver As AppTemplateDeriver
    Set Deriver = New AppTemplateDeriver
    Set Derive = Deriver.AppDeriveTemplateBindings(Items)
End Function

Private Function CountDiagnostic(ByVal Result As AppTemplateDerivationResult, ByVal Code As String) As Long
    Dim Diagnostic As AppTemplateDerivationDiagnostic
    For Each Diagnostic In Result.Diagnostics
        If Diagnostic.Code = Code Then CountDiagnostic = CountDiagnostic + 1
    Next Diagnostic
End Function

Private Sub AssertAccessFails(ByVal Value As Object, ByVal ContractName As String)
    Dim Ignored As Variant
    Dim ErrorText As String

    On Error Resume Next
    Select Case ContractName
        Case "Result": Ignored = Value.IsSuccess
        Case "Item": Ignored = Value.SourceIndex
        Case "Diagnostic": Ignored = Value.Code
    End Select
    ErrorText = Err.Description
    Err.Clear
    On Error GoTo 0
    AssertContains ErrorText, "not initialized", ContractName & " access should require initialization."
End Sub

Private Sub AssertDiagnostic(ByVal Result As AppTemplateDerivationResult, ByVal Code As String, ByVal Message As String)
    AssertFalse Result.IsSuccess, Message
    AssertTrue Result.HasDiagnosticCode(Code), Message & " Missing " & Code
    AssertEquals Result.ErrorCount, Result.Diagnostics.Count, "All derivation diagnostics are errors."
End Sub

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then Err.Raise AppTestAssertErrorNumber, "AppTemplateDeriverTests", Message
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then Err.Raise AppTestAssertErrorNumber, "AppTemplateDeriverTests", Message
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppTemplateDeriverTests", Message & " Expected=" & CStr(Expected) & " Actual=" & CStr(Actual)
    End If
End Sub
