VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppTemplateDeriver"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppTemplateDeriver
' Layer: Application
' Responsibility: Derive exact logical Template bindings from ManifestItem.
' Dependencies: Common, Infrastructure, Application
'=========================================================================

Private Const PathInvalid As String = "#INVALID#"

Public Function AppDeriveTemplateBindings(ByVal ManifestItems As Collection) As AppTemplateDerivationResult
    Dim Diagnostics As Collection
    Dim Candidates As Collection
    Dim Result As AppTemplateDerivationResult
    Dim ItemIndex As Long
    Dim KnownModuleNames As Object

    Set Diagnostics = New Collection
    Set Candidates = New Collection
    Set KnownModuleNames = CreateObject("Scripting.Dictionary")
    KnownModuleNames.CompareMode = vbBinaryCompare

    If ManifestItems Is Nothing Then
        AddDiagnostic Diagnostics, "TD001", "InputRequired", 0, vbNullString, vbNullString, "ManifestItems is required."
        Set AppDeriveTemplateBindings = CreateFailureResult(Diagnostics)
        Exit Function
    End If
    If ManifestItems.Count = 0 Then
        AddDiagnostic Diagnostics, "TD002", "InputEmpty", 0, vbNullString, vbNullString, "ManifestItems must not be empty."
        Set AppDeriveTemplateBindings = CreateFailureResult(Diagnostics)
        Exit Function
    End If

    For ItemIndex = 1 To ManifestItems.Count
        ValidateItem ManifestItems.Item(ItemIndex), ItemIndex, Diagnostics, Candidates, KnownModuleNames
    Next ItemIndex
    Set Diagnostics = FinalizeDiagnostics(Diagnostics)

    If Diagnostics.Count > 0 Then
        Set AppDeriveTemplateBindings = CreateFailureResult(Diagnostics)
        Exit Function
    End If

    If Not CandidateOutputIsComplete(Candidates, ManifestItems.Count) Then
        AddDiagnostic Diagnostics, "TD017", "OutputInvariantFailure", 0, vbNullString, vbNullString, "Template Derivation output invariant could not be satisfied."
        Set Diagnostics = FinalizeDiagnostics(Diagnostics)
        Set AppDeriveTemplateBindings = CreateFailureResult(Diagnostics)
        Exit Function
    End If

    Set Result = New AppTemplateDerivationResult
    Result.AppInitialize True, Candidates, Diagnostics
    Set AppDeriveTemplateBindings = Result
End Function

Private Sub ValidateItem( _
    ByVal RawItem As Variant, _
    ByVal ItemIndex As Long, _
    ByVal Diagnostics As Collection, _
    ByVal Candidates As Collection, _
    ByVal KnownModuleNames As Object)
    Dim Manifest As ManifestItem
    Dim ModuleName As String
    Dim ModuleType As String
    Dim LayerName As String
    Dim ManifestTemplatePath As String
    Dim NormalizedPath As String
    Dim TemplateKey As String
    Dim CanonicalPath As String
    Dim TemplateRole As String
    Dim RuleId As String
    Dim RuleCount As Long

    If Not IsObject(RawItem) Then
        AddDiagnostic Diagnostics, "TD003", "InvalidItemType", ItemIndex, vbNullString, vbNullString, "Collection item must be a ManifestItem."
        Exit Sub
    End If
    If RawItem Is Nothing Then
        AddDiagnostic Diagnostics, "TD003", "InvalidItemType", ItemIndex, vbNullString, vbNullString, "Collection item must be a ManifestItem."
        Exit Sub
    End If
    If TypeName(RawItem) <> "ManifestItem" Then
        AddDiagnostic Diagnostics, "TD003", "InvalidItemType", ItemIndex, vbNullString, vbNullString, "Collection item must be a ManifestItem."
        Exit Sub
    End If

    Set Manifest = RawItem
    If Not TryReadManifest(Manifest, ModuleName, ModuleType, LayerName, ManifestTemplatePath) Then
        AddDiagnostic Diagnostics, "TD004", "InvalidItemState", ItemIndex, vbNullString, vbNullString, "ManifestItem state is invalid."
        Exit Sub
    End If

    If Not IsApprovedLayer(LayerName) Then
        AddDiagnostic Diagnostics, "TD009", "UnsupportedManifestFact", ItemIndex, ModuleName, "LayerName", "LayerName is not supported for Template Derivation."
    End If

    NormalizedPath = NormalizeTemplatePath(ManifestTemplatePath)
    If NormalizedPath = PathInvalid Then
        AddDiagnostic Diagnostics, "TD011", "InvalidTemplatePath", ItemIndex, ModuleName, "TemplatePath", "TemplatePath cannot be lexically normalized within its path root."
    End If

    If IsApprovedLayer(LayerName) Then
        ResolveMapping ModuleType, LayerName, RuleCount, TemplateKey, CanonicalPath, TemplateRole, RuleId
        If RuleCount = 0 Then
            AddDiagnostic Diagnostics, "TD014", "TemplateRuleNotFound", ItemIndex, ModuleName, vbNullString, "No approved Template Derivation rule matches the ManifestItem."
        ElseIf RuleCount > 1 Then
            AddDiagnostic Diagnostics, "TD015", "TemplateRuleNotUnique", ItemIndex, ModuleName, vbNullString, "More than one approved Template Derivation rule matches the ManifestItem."
        End If
    End If

    If NormalizedPath <> PathInvalid And IsDomainModuleTemplatePath(NormalizedPath) Then
        AddDiagnostic Diagnostics, "TD013", "ManifestOnlyTemplateMisuse", ItemIndex, ModuleName, "TemplatePath", "DomainModuleTemplate is not an approved Template Derivation identity."
    ElseIf NormalizedPath <> PathInvalid And RuleCount = 1 Then
        If Not TemplatePathsMatch(NormalizedPath, CanonicalPath) Then
            AddDiagnostic Diagnostics, "TD012", "TemplatePathConflict", ItemIndex, ModuleName, "TemplatePath", "Manifest TemplatePath conflicts with the derived canonical TemplatePath."
        End If
    End If

    If KnownModuleNames.Exists(ModuleName) Then
        AddDiagnostic Diagnostics, "TD016", "DuplicateModuleName", ItemIndex, ModuleName, "ModuleName", "ModuleName duplicates an earlier ManifestItem."
    Else
        KnownModuleNames.Add ModuleName, True
    End If

    If Not HasItemDiagnostic(Diagnostics, ItemIndex) And RuleCount = 1 Then
        Candidates.Add CreateItem(ItemIndex, ModuleName, ModuleType, LayerName, TemplateKey, CanonicalPath, TemplateRole, RuleId)
    End If
End Sub

Private Function TryReadManifest( _
    ByVal Manifest As ManifestItem, _
    ByRef ModuleName As String, _
    ByRef ModuleType As String, _
    ByRef LayerName As String, _
    ByRef TemplatePath As String) As Boolean

    On Error GoTo InvalidState
    If Not Manifest.InfValidate() Then Exit Function
    ModuleName = Manifest.InfGetModuleName()
    ModuleType = Manifest.InfGetModuleType()
    LayerName = Manifest.InfGetLayerName()
    TemplatePath = Manifest.InfGetTemplatePath()
    TryReadManifest = True
    Exit Function

InvalidState:
    Err.Clear
End Function

Private Sub ResolveMapping( _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByRef RuleCount As Long, _
    ByRef TemplateKey As String, _
    ByRef TemplatePath As String, _
    ByRef TemplateRole As String, _
    ByRef RuleId As String)

    RuleCount = 0
    If ModuleType = "StandardModule" And IsApprovedLayer(LayerName) Then
        RuleCount = RuleCount + 1
        TemplateKey = "ModuleTemplate"
        TemplatePath = "templates/ModuleTemplate.txt"
        TemplateRole = "StandardModuleTemplate"
        RuleId = "TD-P5-02-STANDARD-MODULE"
    End If
    If ModuleType = "ClassModule" And LayerName = "Domain" Then
        RuleCount = RuleCount + 1
        TemplateKey = "DomainClassTemplate"
        TemplatePath = "templates/DomainClassTemplate.txt"
        TemplateRole = "DomainClassModuleTemplate"
        RuleId = "TD-P5-02-DOMAIN-CLASS"
    End If
    If ModuleType = "ClassModule" And IsApprovedNonDomainClassLayer(LayerName) Then
        RuleCount = RuleCount + 1
        TemplateKey = "ClassTemplate"
        TemplatePath = "templates/ClassTemplate.txt"
        TemplateRole = "ClassModuleTemplate"
        RuleId = "TD-P5-02-CLASS"
    End If
End Sub

Private Function CreateItem( _
    ByVal SourceIndex As Long, _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByVal TemplateKey As String, _
    ByVal TemplatePath As String, _
    ByVal TemplateRole As String, _
    ByVal RuleId As String) As AppTemplateDerivationItem

    Dim Item As AppTemplateDerivationItem
    Dim Reason As String

    Reason = "ModuleType=" & ModuleType & "; LayerName=" & LayerName & "; RuleId=" & RuleId
    Set Item = New AppTemplateDerivationItem
    Item.AppInitialize SourceIndex, ModuleName, ModuleType, LayerName, TemplateKey, TemplatePath, TemplateRole, RuleId, Reason, True, vbNullString
    Set CreateItem = Item
End Function

Private Function NormalizeTemplatePath(ByVal Value As String) As String
    Dim TextValue As String
    Dim Parts As Variant
    Dim Stack() As String
    Dim StackCount As Long
    Dim Part As Variant
    Dim Prefix As String
    Dim IsAbsolute As Boolean
    Dim Index As Long
    Dim Result As String

    TextValue = Trim$(Replace(Value, "\", "/"))
    If Len(TextValue) = 0 Then
        NormalizeTemplatePath = PathInvalid
        Exit Function
    End If

    If Left$(TextValue, 1) = "/" Then
        IsAbsolute = True
        Prefix = "/"
    ElseIf Len(TextValue) >= 3 And Mid$(TextValue, 2, 1) = ":" And Mid$(TextValue, 3, 1) = "/" Then
        IsAbsolute = True
        Prefix = Left$(TextValue, 2) & "/"
        TextValue = Mid$(TextValue, 4)
    End If

    Do While InStr(1, TextValue, "//", vbBinaryCompare) > 0
        TextValue = Replace(TextValue, "//", "/")
    Loop
    Parts = Split(TextValue, "/")
    ReDim Stack(0 To UBound(Parts))

    For Each Part In Parts
        If Len(CStr(Part)) = 0 Or CStr(Part) = "." Then
            ' Skip empty and current-directory segments.
        ElseIf CStr(Part) = ".." Then
            If StackCount = 0 Then
                NormalizeTemplatePath = PathInvalid
                Exit Function
            End If
            StackCount = StackCount - 1
        Else
            Stack(StackCount) = CStr(Part)
            StackCount = StackCount + 1
        End If
    Next Part

    Result = Prefix
    For Index = 0 To StackCount - 1
        If Len(Result) > 0 And Right$(Result, 1) <> "/" Then Result = Result & "/"
        Result = Result & Stack(Index)
    Next Index
    If Len(Result) > 1 And Right$(Result, 1) = "/" Then Result = Left$(Result, Len(Result) - 1)
    If Len(Result) = 0 Then Result = IIf(IsAbsolute, Prefix, ".")
    NormalizeTemplatePath = Result
End Function

Private Function TemplatePathsMatch(ByVal NormalizedManifestPath As String, ByVal CanonicalPath As String) As Boolean
    Dim Offset As Long

    If StrComp(NormalizedManifestPath, CanonicalPath, vbTextCompare) = 0 Then
        TemplatePathsMatch = True
        Exit Function
    End If
    If Not IsAbsolutePath(NormalizedManifestPath) Then Exit Function
    If Len(NormalizedManifestPath) <= Len(CanonicalPath) Then Exit Function

    Offset = Len(NormalizedManifestPath) - Len(CanonicalPath) + 1
    If StrComp(Mid$(NormalizedManifestPath, Offset), CanonicalPath, vbTextCompare) = 0 Then
        TemplatePathsMatch = Mid$(NormalizedManifestPath, Offset - 1, 1) = "/"
    End If
End Function

Private Function IsAbsolutePath(ByVal Value As String) As Boolean
    IsAbsolutePath = Left$(Value, 1) = "/" Or (Len(Value) >= 3 And Mid$(Value, 2, 1) = ":" And Mid$(Value, 3, 1) = "/")
End Function

Private Function IsDomainModuleTemplatePath(ByVal Value As String) As Boolean
    Dim Parts As Variant
    Parts = Split(Value, "/")
    IsDomainModuleTemplatePath = StrComp(CStr(Parts(UBound(Parts))), "DomainModuleTemplate.txt", vbTextCompare) = 0
End Function

Private Function IsApprovedLayer(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Common", "Core", "Domain", "Application", "Infrastructure", "Presentation"
            IsApprovedLayer = True
    End Select
End Function

Private Function IsApprovedNonDomainClassLayer(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Common", "Core", "Application", "Infrastructure", "Presentation"
            IsApprovedNonDomainClassLayer = True
    End Select
End Function

Private Function CandidateOutputIsComplete(ByVal Candidates As Collection, ByVal ExpectedCount As Long) As Boolean
    Dim Item As AppTemplateDerivationItem
    Dim ExpectedIndex As Long

    If Candidates.Count <> ExpectedCount Then Exit Function
    ExpectedIndex = 1
    For Each Item In Candidates
        If Item.SourceIndex <> ExpectedIndex Then Exit Function
        If Not Item.IsGeneratable Or Not ComIsBlankText(Item.UnsupportedReason) Then Exit Function
        ExpectedIndex = ExpectedIndex + 1
    Next Item
    CandidateOutputIsComplete = True
End Function

Private Function HasItemDiagnostic(ByVal Diagnostics As Collection, ByVal ItemIndex As Long) As Boolean
    Dim Diagnostic As AppTemplateDerivationDiagnostic
    For Each Diagnostic In Diagnostics
        If Diagnostic.ItemIndex = ItemIndex Then
            HasItemDiagnostic = True
            Exit Function
        End If
    Next Diagnostic
End Function

Private Sub AddDiagnostic( _
    ByVal Diagnostics As Collection, _
    ByVal Code As String, _
    ByVal Category As String, _
    ByVal ItemIndex As Long, _
    ByVal ModuleName As String, _
    ByVal FieldName As String, _
    ByVal Message As String)

    Dim Existing As AppTemplateDerivationDiagnostic
    Dim Diagnostic As AppTemplateDerivationDiagnostic

    For Each Existing In Diagnostics
        If Existing.ItemIndex = ItemIndex And Existing.Code = Code And Existing.FieldName = FieldName Then Exit Sub
    Next Existing

    Set Diagnostic = New AppTemplateDerivationDiagnostic
    Diagnostic.AppInitialize Code, Category, ItemIndex, ModuleName, FieldName, Message, 0
    Diagnostics.Add Diagnostic
End Sub

Private Function FinalizeDiagnostics(ByVal Diagnostics As Collection) As Collection
    Dim Sorted As Collection
    Dim Used() As Boolean
    Dim Position As Long
    Dim CandidateIndex As Long
    Dim BestIndex As Long
    Dim Diagnostic As AppTemplateDerivationDiagnostic
    Dim Reordered As AppTemplateDerivationDiagnostic

    Set Sorted = New Collection
    If Diagnostics.Count = 0 Then
        Set FinalizeDiagnostics = Sorted
        Exit Function
    End If
    ReDim Used(1 To Diagnostics.Count)

    For Position = 1 To Diagnostics.Count
        BestIndex = 0
        For CandidateIndex = 1 To Diagnostics.Count
            If Not Used(CandidateIndex) Then
                If BestIndex = 0 Then
                    BestIndex = CandidateIndex
                ElseIf DiagnosticComesBefore(Diagnostics.Item(CandidateIndex), Diagnostics.Item(BestIndex)) Then
                    BestIndex = CandidateIndex
                End If
            End If
        Next CandidateIndex
        Used(BestIndex) = True
        Set Diagnostic = Diagnostics.Item(BestIndex)
        Set Reordered = New AppTemplateDerivationDiagnostic
        Reordered.AppInitialize Diagnostic.Code, Diagnostic.Category, Diagnostic.ItemIndex, Diagnostic.ModuleName, Diagnostic.FieldName, Diagnostic.Message, Position
        Sorted.Add Reordered
    Next Position
    Set FinalizeDiagnostics = Sorted
End Function

Private Function DiagnosticComesBefore(ByVal LeftValue As AppTemplateDerivationDiagnostic, ByVal RightValue As AppTemplateDerivationDiagnostic) As Boolean
    If LeftValue.ItemIndex <> RightValue.ItemIndex Then
        DiagnosticComesBefore = LeftValue.ItemIndex < RightValue.ItemIndex
    Else
        DiagnosticComesBefore = DiagnosticStep(LeftValue.Code) < DiagnosticStep(RightValue.Code)
    End If
End Function

Private Function DiagnosticStep(ByVal Code As String) As Long
    Select Case Code
        Case "TD001": DiagnosticStep = 1
        Case "TD002": DiagnosticStep = 2
        Case "TD003": DiagnosticStep = 3
        Case "TD004": DiagnosticStep = 4
        Case "TD005": DiagnosticStep = 5
        Case "TD006", "TD007": DiagnosticStep = 6
        Case "TD008", "TD009": DiagnosticStep = 7
        Case "TD010", "TD011": DiagnosticStep = 8
        Case "TD014", "TD015": DiagnosticStep = 9
        Case "TD013": DiagnosticStep = 11
        Case "TD012": DiagnosticStep = 12
        Case "TD016": DiagnosticStep = 13
        Case "TD017": DiagnosticStep = 14
    End Select
End Function

Private Function CreateFailureResult(ByVal Diagnostics As Collection) As AppTemplateDerivationResult
    Dim Result As AppTemplateDerivationResult
    Dim Items As Collection

    Set Items = New Collection
    Set Result = New AppTemplateDerivationResult
    Result.AppInitialize False, Items, Diagnostics
    Set CreateFailureResult = Result
End Function
