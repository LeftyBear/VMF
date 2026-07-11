VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Build_ProjectManifest"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: Build_ProjectManifest
' Layer: Application
' Responsibility: Parse and expose VMF v1.0 project manifest generation targets.
' Dependencies: Common, Infrastructure
'==========================================================================

Private Const SectionApplication As String = "application"
Private Const SectionGeneration As String = "generation"
Private Const SectionPolicy As String = "policy"
Private Const SectionLayers As String = "layers"
Private Const SectionDomain As String = "domain"
Private Const SectionApplicationServices As String = "application_services"
Private Const SectionInfrastructure As String = "infrastructure"
Private Const SectionPresentation As String = "presentation"

Private Const ModuleGroupStandardModules As String = "StandardModules"
Private Const ModuleGroupClasses As String = "Classes"

Private Type TMember
    ItemsByLayer As Object
    DefinedLayers As Object
    GenerationEnabled As Object
    ItemKeys As Object
    SectionSeen As Object
    ApplicationValues As Object
    PolicyValues As Object
    LayerPaths As Object
    SourceRoot As String
    IsInitialized As Boolean
End Type

Private This As TMember

Public Function BuildInitializeFromContent(ByVal ManifestContent As String) As ComResult
    On Error GoTo ErrHandler

    ComRequireText ManifestContent, "ManifestContent", "Build_ProjectManifest.BuildInitializeFromContent"

    Set This.ItemsByLayer = CreateObject("Scripting.Dictionary")
    Set This.DefinedLayers = CreateObject("Scripting.Dictionary")
    Set This.GenerationEnabled = CreateObject("Scripting.Dictionary")
    Set This.ItemKeys = CreateObject("Scripting.Dictionary")
    Set This.SectionSeen = CreateObject("Scripting.Dictionary")
    Set This.ApplicationValues = CreateObject("Scripting.Dictionary")
    Set This.PolicyValues = CreateObject("Scripting.Dictionary")
    Set This.LayerPaths = CreateObject("Scripting.Dictionary")
    This.SourceRoot = vbNullString
    This.IsInitialized = False

    ParseManifestContent ManifestContent
    ValidateManifest

    This.IsInitialized = True
    Set BuildInitializeFromContent = ComCreateSuccess("Project manifest parsed.")
    Exit Function

ErrHandler:
    Set BuildInitializeFromContent = ComCreateFailureFromErr( _
        "Build_ProjectManifest.BuildInitializeFromContent", _
        Err.Description, _
        "Error", _
        False)
End Function

Public Function BuildGetLayerItems(ByVal LayerName As String) As Collection
    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.BuildGetLayerItems"

    If This.ItemsByLayer.Exists(LayerName) Then
        Set BuildGetLayerItems = This.ItemsByLayer(LayerName)
    Else
        Set BuildGetLayerItems = New Collection
    End If
End Function

Public Function BuildHasLayer(ByVal LayerName As String) As Boolean
    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.BuildHasLayer"

    BuildHasLayer = This.DefinedLayers.Exists(LayerName)
End Function

Public Function BuildIsGenerationEnabled(ByVal LayerName As String) As Boolean
    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.BuildIsGenerationEnabled"

    If This.GenerationEnabled.Exists(LayerName) Then
        BuildIsGenerationEnabled = CBool(This.GenerationEnabled(LayerName))
    Else
        BuildIsGenerationEnabled = BuildHasLayer(LayerName)
    End If
End Function

Public Function BuildGetSourceRoot() As String
    EnsureInitialized
    BuildGetSourceRoot = This.SourceRoot
End Function

Private Sub ParseManifestContent(ByVal ManifestContent As String)
    Dim Lines() As String
    Dim LineIndex As Long
    Dim RawLine As String
    Dim LineText As String
    Dim CurrentSection As String
    Dim CurrentSubSection As String
    Dim CurrentLayerName As String

    Lines = Split(NormalizeLineBreaks(ManifestContent), vbLf)

    For LineIndex = LBound(Lines) To UBound(Lines)
        RawLine = Lines(LineIndex)
        LineText = Trim$(StripInlineComment(RawLine))

        If Not ComIsBlankText(LineText) Then
            ValidateIndentation RawLine

            If LeadingSpaceCount(RawLine) = 0 Then
                CurrentSection = ParseTopLevelSection(LineText)
                CurrentSubSection = vbNullString
                CurrentLayerName = vbNullString
            Else
                If ComIsBlankText(CurrentSection) Then
                    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest content must be inside a top-level section."
                End If

                Select Case CurrentSection
                    Case SectionApplication
                        ParseApplicationLine RawLine, LineText
                    Case SectionGeneration
                        ParseGenerationLine RawLine, LineText
                    Case SectionPolicy
                        ParsePolicyLine RawLine, LineText
                    Case SectionLayers
                        ParseLayersLine RawLine, LineText, CurrentLayerName
                    Case SectionDomain
                        ParseDomainLine RawLine, LineText, CurrentSubSection
                    Case SectionApplicationServices
                        ParseApplicationServicesLine RawLine, LineText
                    Case SectionInfrastructure
                        ParseInfrastructureLine RawLine, LineText, CurrentSubSection
                    Case SectionPresentation
                        ParsePresentationLine RawLine, LineText, CurrentSubSection
                    Case Else
                        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported manifest section: " & CurrentSection
                End Select
            End If
        End If
    Next LineIndex
End Sub

Private Function ParseTopLevelSection(ByVal LineText As String) As String
    Dim SectionName As String

    If Right$(LineText, 1) <> ":" Or InStr(1, LineText, ":", vbBinaryCompare) <> Len(LineText) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Top-level section must use section_name: syntax."
    End If

    SectionName = RemoveTrailingColon(LineText)
    ValidateTopLevelSection SectionName

    If This.SectionSeen.Exists(SectionName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Duplicate manifest section: " & SectionName
    End If

    This.SectionSeen.Add SectionName, True
    ParseTopLevelSection = SectionName
End Function

Private Sub ParseApplicationLine(ByVal RawLine As String, ByVal LineText As String)
    Dim KeyName As String
    Dim KeyValue As String

    RequireIndent RawLine, 2, "application entries must use two-space indentation."
    ParseKeyValue LineText, KeyName, KeyValue

    Select Case KeyName
        Case "name", "version", "vmf_version", "build_version"
            RequireValue KeyValue, "application." & KeyName
            AddUniqueValue This.ApplicationValues, KeyName, KeyValue, "application"
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported application key: " & KeyName
    End Select
End Sub

Private Sub ParseGenerationLine(ByVal RawLine As String, ByVal LineText As String)
    Dim KeyName As String
    Dim KeyValue As String
    Dim LayerName As String

    RequireIndent RawLine, 2, "generation entries must use two-space indentation."
    ParseKeyValue LineText, KeyName, KeyValue
    ValidateGenerationKey KeyName

    LayerName = CanonicalLayerName(KeyName)
    If This.GenerationEnabled.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Duplicate generation key: " & KeyName
    End If

    This.GenerationEnabled.Add LayerName, ParseStrictBoolean(KeyValue, "generation." & KeyName)
End Sub

Private Sub ParsePolicyLine(ByVal RawLine As String, ByVal LineText As String)
    Dim KeyName As String
    Dim KeyValue As String

    RequireIndent RawLine, 2, "policy entries must use two-space indentation."
    ParseKeyValue LineText, KeyName, KeyValue
    ValidatePolicyKey KeyName
    AddUniqueValue This.PolicyValues, KeyName, ParseStrictBoolean(KeyValue, "policy." & KeyName), "policy"
End Sub

Private Sub ParseLayersLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentLayerName As String)

    Dim KeyName As String
    Dim KeyValue As String
    Dim LayerName As String

    Select Case LeadingSpaceCount(RawLine)
        Case 2
            If Right$(LineText, 1) <> ":" Then
                Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Layer entry must use layer_name: syntax."
            End If

            KeyName = RemoveTrailingColon(LineText)
            ValidateLayerKey KeyName
            LayerName = CanonicalLayerName(KeyName)
            RegisterLayer LayerName
            CurrentLayerName = LayerName

        Case 4
            ComRequireText CurrentLayerName, "CurrentLayerName", "Build_ProjectManifest.ParseLayersLine"
            ParseKeyValue LineText, KeyName, KeyValue
            If KeyName <> "path" Then
                Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported layer key: " & KeyName
            End If

            RequireValue KeyValue, "layers." & CurrentLayerName & ".path"
            AddUniqueValue This.LayerPaths, CurrentLayerName, KeyValue, "layers"

        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Invalid layers structure."
    End Select
End Sub

Private Sub ParseDomainLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentSubSection As String)

    Select Case LeadingSpaceCount(RawLine)
        Case 2
            CurrentSubSection = ParseRequiredSubSection(LineText, "entities", "domain")
        Case 4
            If CurrentSubSection <> "entities" Then
                Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "domain list item must be inside entities."
            End If

            AddListManifestItem LineText, "Domain", ModuleGroupClasses
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Invalid domain structure."
    End Select
End Sub

Private Sub ParseApplicationServicesLine(ByVal RawLine As String, ByVal LineText As String)
    RequireIndent RawLine, 2, "application_services list items must use two-space indentation."
    AddListManifestItem LineText, "Application", ModuleGroupClasses
End Sub

Private Sub ParseInfrastructureLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentSubSection As String)

    Select Case LeadingSpaceCount(RawLine)
        Case 2
            CurrentSubSection = ParseRequiredSubSection(LineText, "repositories", "infrastructure")
        Case 4
            If CurrentSubSection <> "repositories" Then
                Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "infrastructure list item must be inside repositories."
            End If

            AddListManifestItem LineText, "Infrastructure", ModuleGroupClasses
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Invalid infrastructure structure."
    End Select
End Sub

Private Sub ParsePresentationLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentSubSection As String)

    Select Case LeadingSpaceCount(RawLine)
        Case 2
            CurrentSubSection = ParsePresentationSubSection(LineText)
        Case 4
            Select Case CurrentSubSection
                Case "forms"
                    AddListManifestItem LineText, "Presentation", ModuleGroupClasses
                Case "modules"
                    AddListManifestItem LineText, "Presentation", ModuleGroupStandardModules
                Case Else
                    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "presentation list item must be inside forms or modules."
            End Select
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Invalid presentation structure."
    End Select
End Sub

Private Function ParseRequiredSubSection( _
    ByVal LineText As String, _
    ByVal ExpectedName As String, _
    ByVal ParentSection As String) As String

    If Right$(LineText, 1) <> ":" Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", ParentSection & " subsection must use subsection_name: syntax."
    End If

    ParseRequiredSubSection = RemoveTrailingColon(LineText)
    If ParseRequiredSubSection <> ExpectedName Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported " & ParentSection & " subsection: " & ParseRequiredSubSection
    End If
End Function

Private Function ParsePresentationSubSection(ByVal LineText As String) As String
    If Right$(LineText, 1) <> ":" Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "presentation subsection must use subsection_name: syntax."
    End If

    ParsePresentationSubSection = RemoveTrailingColon(LineText)
    Select Case ParsePresentationSubSection
        Case "forms", "modules"
            Exit Function
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported presentation subsection: " & ParsePresentationSubSection
End Function

Private Sub AddListManifestItem(ByVal LineText As String, ByVal LayerName As String, ByVal GroupName As String)
    Dim ModuleName As String

    If Left$(LineText, 1) <> "-" Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest list item must use - item syntax."
    End If

    ModuleName = Trim$(Mid$(LineText, 2))
    If InStr(1, ModuleName, ":", vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest list item must be a scalar value."
    End If

    AddModuleItem LayerName, GroupName, ModuleName
End Sub

Private Sub AddModuleItem(ByVal LayerName As String, ByVal GroupName As String, ByVal ModuleName As String)
    Dim Item As ManifestItem
    Dim ItemKey As String
    Dim Items As Collection

    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.AddModuleItem"
    ComRequireText GroupName, "GroupName", "Build_ProjectManifest.AddModuleItem"
    ComRequireText ModuleName, "ModuleName", "Build_ProjectManifest.AddModuleItem"

    If Not IsSupportedGenerationTargetLayer(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported generation layer: " & LayerName
    End If

    ItemKey = LCase$(LayerName & "." & ModuleName)
    If This.ItemKeys.Exists(ItemKey) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Duplicate manifest module: " & ModuleName
    End If

    Set Item = New ManifestItem
    Item.InfInitialize ModuleName, ResolveModuleType(GroupName), LayerName, ResolveTemplatePath(LayerName, GroupName)

    EnsureLayerItems LayerName
    Set Items = This.ItemsByLayer(LayerName)
    Items.Add Item
    This.ItemKeys.Add ItemKey, True
End Sub

Private Sub RegisterLayer(ByVal LayerName As String)
    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.RegisterLayer"

    If Not IsKnownManifestLayer(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported manifest layer: " & LayerName
    End If

    If This.DefinedLayers.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Duplicate manifest layer: " & LayerName
    End If

    This.DefinedLayers.Add LayerName, True
End Sub

Private Sub EnsureLayerItems(ByVal LayerName As String)
    If Not This.ItemsByLayer.Exists(LayerName) Then
        This.ItemsByLayer.Add LayerName, New Collection
    End If
End Sub

Private Sub ValidateManifest()
    RequireSection SectionApplication
    RequireSection SectionGeneration
    RequireSection SectionPolicy
    RequireSection SectionLayers
    RequireSection SectionDomain
    RequireSection SectionApplicationServices
    RequireSection SectionInfrastructure
    RequireSection SectionPresentation

    RequireDictionaryValue This.ApplicationValues, "name", "application.name"
    RequireDictionaryValue This.ApplicationValues, "version", "application.version"
    RequireDictionaryValue This.ApplicationValues, "vmf_version", "application.vmf_version"
    RequireDictionaryValue This.ApplicationValues, "build_version", "application.build_version"

    RequireGenerationKey "common"
    RequireGenerationKey "core"
    RequireGenerationKey "domain"
    RequireGenerationKey "application"
    RequireGenerationKey "infrastructure"
    RequireGenerationKey "presentation"

    RequireDictionaryValue This.PolicyValues, "common_editable", "policy.common_editable"
    RequireDictionaryValue This.PolicyValues, "core_editable", "policy.core_editable"
    RequireDictionaryValue This.PolicyValues, "build_design_frozen", "policy.build_design_frozen"
    RequireDictionaryValue This.PolicyValues, "vmf_design_frozen", "policy.vmf_design_frozen"

    RequireLayerPath "Common"
    RequireLayerPath "Core"
    RequireLayerPath "Domain"
    RequireLayerPath "Application"
    RequireLayerPath "Infrastructure"
    RequireLayerPath "Presentation"

    This.SourceRoot = DeriveSourceRoot()

    If This.ItemKeys.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest contains no supported generation targets."
    End If
End Sub

Private Sub ValidateTopLevelSection(ByVal SectionName As String)
    Select Case SectionName
        Case SectionApplication, SectionGeneration, SectionPolicy, SectionLayers, _
            SectionDomain, SectionApplicationServices, SectionInfrastructure, SectionPresentation
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported top-level manifest section: " & SectionName
End Sub

Private Sub ValidateGenerationKey(ByVal KeyName As String)
    Select Case KeyName
        Case "common", "core", "domain", "application", "infrastructure", "presentation"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported generation key: " & KeyName
End Sub

Private Sub ValidatePolicyKey(ByVal KeyName As String)
    Select Case KeyName
        Case "common_editable", "core_editable", "build_design_frozen", "vmf_design_frozen"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported policy key: " & KeyName
End Sub

Private Sub ValidateLayerKey(ByVal KeyName As String)
    Select Case KeyName
        Case "common", "core", "domain", "application", "infrastructure", "presentation"
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported layer key: " & KeyName
End Sub

Private Function ResolveModuleType(ByVal GroupName As String) As String
    Select Case GroupName
        Case ModuleGroupStandardModules
            ResolveModuleType = "StandardModule"
        Case ModuleGroupClasses
            ResolveModuleType = "ClassModule"
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported module group: " & GroupName
    End Select
End Function

Private Function ResolveTemplatePath(ByVal LayerName As String, ByVal GroupName As String) As String
    Dim TemplateFileName As String

    If GroupName = ModuleGroupStandardModules Then
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

Private Function IsSupportedGenerationTargetLayer(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Common", "Infrastructure", "Domain", "Application", "Presentation"
            IsSupportedGenerationTargetLayer = True
        Case Else
            IsSupportedGenerationTargetLayer = False
    End Select
End Function

Private Function IsKnownManifestLayer(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Common", "Core", "Infrastructure", "Domain", "Application", "Presentation"
            IsKnownManifestLayer = True
        Case Else
            IsKnownManifestLayer = False
    End Select
End Function

Private Function CanonicalLayerName(ByVal LayerKey As String) As String
    Select Case LayerKey
        Case "common"
            CanonicalLayerName = "Common"
        Case "core"
            CanonicalLayerName = "Core"
        Case "domain"
            CanonicalLayerName = "Domain"
        Case "application"
            CanonicalLayerName = "Application"
        Case "infrastructure"
            CanonicalLayerName = "Infrastructure"
        Case "presentation"
            CanonicalLayerName = "Presentation"
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported layer key: " & LayerKey
    End Select
End Function

Private Sub ParseKeyValue(ByVal LineText As String, ByRef KeyName As String, ByRef KeyValue As String)
    Dim SeparatorIndex As Long

    SeparatorIndex = InStr(1, LineText, ":", vbBinaryCompare)
    If SeparatorIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest entry must use key: value syntax."
    End If

    KeyName = Trim$(Left$(LineText, SeparatorIndex - 1))
    KeyValue = Trim$(Mid$(LineText, SeparatorIndex + 1))

    If ComIsBlankText(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest key is required."
    End If
End Sub

Private Function ParseStrictBoolean(ByVal TextValue As String, ByVal ValueName As String) As Boolean
    Select Case TextValue
        Case "true"
            ParseStrictBoolean = True
        Case "false"
            ParseStrictBoolean = False
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", ValueName & " must be true or false."
    End Select
End Function

Private Sub AddUniqueValue(ByVal Values As Object, ByVal KeyName As String, ByVal KeyValue As Variant, ByVal SectionName As String)
    If Values.Exists(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Duplicate " & SectionName & " key: " & KeyName
    End If

    Values.Add KeyName, KeyValue
End Sub

Private Sub RequireValue(ByVal KeyValue As String, ByVal ValueName As String)
    If ComIsBlankText(KeyValue) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", ValueName & " is required."
    End If
End Sub

Private Sub RequireSection(ByVal SectionName As String)
    If Not This.SectionSeen.Exists(SectionName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest section is required: " & SectionName
    End If
End Sub

Private Sub RequireDictionaryValue(ByVal Values As Object, ByVal KeyName As String, ByVal ValueName As String)
    If Not Values.Exists(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", ValueName & " is required."
    End If
End Sub

Private Sub RequireGenerationKey(ByVal KeyName As String)
    If Not This.GenerationEnabled.Exists(CanonicalLayerName(KeyName)) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "generation." & KeyName & " is required."
    End If
End Sub

Private Sub RequireLayerPath(ByVal LayerName As String)
    If Not This.DefinedLayers.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "layers." & LayerName & " is required."
    End If

    If Not This.LayerPaths.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "layers." & LayerName & ".path is required."
    End If
End Sub

Private Function DeriveSourceRoot() As String
    Dim CommonPath As String
    Dim SeparatorIndex As Long

    If This.LayerPaths.Exists("Common") Then
        CommonPath = CStr(This.LayerPaths("Common"))
        SeparatorIndex = InStrRev(CommonPath, "/")
        If SeparatorIndex = 0 Then
            SeparatorIndex = InStrRev(CommonPath, "\")
        End If

        If SeparatorIndex > 1 Then
            DeriveSourceRoot = Left$(CommonPath, SeparatorIndex - 1)
        Else
            DeriveSourceRoot = CommonPath
        End If
    Else
        DeriveSourceRoot = vbNullString
    End If
End Function

Private Sub RequireIndent(ByVal RawLine As String, ByVal ExpectedIndent As Long, ByVal ErrorMessage As String)
    If LeadingSpaceCount(RawLine) <> ExpectedIndent Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", ErrorMessage
    End If
End Sub

Private Sub ValidateIndentation(ByVal RawLine As String)
    If InStr(1, RawLine, vbTab, vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest indentation must use spaces."
    End If

    If LeadingSpaceCount(RawLine) Mod 2 <> 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest indentation must use two-space levels."
    End If
End Sub

Private Function RemoveTrailingColon(ByVal TextValue As String) As String
    If Right$(TextValue, 1) = ":" Then
        RemoveTrailingColon = Left$(TextValue, Len(TextValue) - 1)
    Else
        RemoveTrailingColon = TextValue
    End If
End Function

Private Function StripInlineComment(ByVal TextValue As String) As String
    Dim CommentIndex As Long

    CommentIndex = InStr(1, TextValue, "#", vbBinaryCompare)
    If CommentIndex > 0 Then
        StripInlineComment = Left$(TextValue, CommentIndex - 1)
    Else
        StripInlineComment = TextValue
    End If
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    NormalizeLineBreaks = Replace(TextValue, vbCrLf, vbLf)
    NormalizeLineBreaks = Replace(NormalizeLineBreaks, vbCr, vbLf)
End Function

Private Function LeadingSpaceCount(ByVal TextValue As String) As Long
    Dim Position As Long

    For Position = 1 To Len(TextValue)
        If Mid$(TextValue, Position, 1) <> " " Then
            LeadingSpaceCount = Position - 1
            Exit Function
        End If
    Next Position

    LeadingSpaceCount = Len(TextValue)
End Function

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "Build_ProjectManifest", "Build_ProjectManifest is not initialized."
    End If
End Sub
