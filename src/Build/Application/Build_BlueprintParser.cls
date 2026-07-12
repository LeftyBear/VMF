VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Build_BlueprintParser"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: Build_BlueprintParser
' Layer: Application
' Responsibility: Parse Build v1.1 blueprint text into generation metadata.
' Dependencies: Common, Infrastructure
'==========================================================================

Private Const SectionBlueprint As String = "blueprint"
Private Const SectionLayers As String = "layers"
Private Const SectionModules As String = "modules"

Private Const ModuleGroupStandardModules As String = "StandardModules"
Private Const ModuleGroupClasses As String = "Classes"

Private Type TMember
    BlueprintValues As Object
    DefinedLayers As Object
    ItemsByLayer As Object
    ItemKeys As Object
    SectionSeen As Object
    IsInitialized As Boolean
End Type

Private This As TMember

Public Function BuildInitializeFromContent(ByVal BlueprintContent As String) As ComResult
    On Error GoTo ErrHandler

    ComRequireText BlueprintContent, "BlueprintContent", "Build_BlueprintParser.BuildInitializeFromContent"

    Set This.BlueprintValues = CreateObject("Scripting.Dictionary")
    Set This.DefinedLayers = CreateObject("Scripting.Dictionary")
    Set This.ItemsByLayer = CreateObject("Scripting.Dictionary")
    Set This.ItemKeys = CreateObject("Scripting.Dictionary")
    Set This.SectionSeen = CreateObject("Scripting.Dictionary")
    This.IsInitialized = False

    ParseBlueprintContent BlueprintContent
    ValidateBlueprint

    This.IsInitialized = True
    Set BuildInitializeFromContent = ComCreateSuccess("Blueprint parsed.")
    Exit Function

ErrHandler:
    Set BuildInitializeFromContent = ComCreateFailureFromErr( _
        "Build_BlueprintParser.BuildInitializeFromContent", _
        Err.Description, _
        "Error", _
        False)
End Function

Public Function BuildHasLayer(ByVal LayerName As String) As Boolean
    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_BlueprintParser.BuildHasLayer"

    BuildHasLayer = This.DefinedLayers.Exists(LayerName)
End Function

Public Function BuildGetLayerItems(ByVal LayerName As String) As Collection
    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_BlueprintParser.BuildGetLayerItems"

    If This.ItemsByLayer.Exists(LayerName) Then
        Set BuildGetLayerItems = This.ItemsByLayer(LayerName)
    Else
        Set BuildGetLayerItems = New Collection
    End If
End Function

Public Function BuildGetBlueprintValue(ByVal KeyName As String) As String
    EnsureInitialized
    ComRequireText KeyName, "KeyName", "Build_BlueprintParser.BuildGetBlueprintValue"

    If This.BlueprintValues.Exists(KeyName) Then
        BuildGetBlueprintValue = CStr(This.BlueprintValues(KeyName))
    Else
        BuildGetBlueprintValue = vbNullString
    End If
End Function

Public Function BuildGenerateManifestContent(ByVal LayerName As String) As String
    Dim Items As Collection
    Dim Item As ManifestItem
    Dim Content As String

    EnsureInitialized
    ComRequireText LayerName, "LayerName", "Build_BlueprintParser.BuildGenerateManifestContent"

    If Not This.DefinedLayers.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint layer is not defined: " & LayerName
    End If

    Content = "# ModuleName,ModuleType,LayerName,TemplatePath"
    Set Items = BuildGetLayerItems(LayerName)
    If Items.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint layer contains no generation modules: " & LayerName
    End If

    For Each Item In Items
        Content = Content & vbCrLf & FormatManifestItem(Item)
    Next Item

    BuildGenerateManifestContent = Content
End Function

Private Sub ParseBlueprintContent(ByVal BlueprintContent As String)
    Dim Lines() As String
    Dim LineIndex As Long
    Dim RawLine As String
    Dim LineText As String
    Dim CurrentSection As String
    Dim CurrentLayerName As String
    Dim CurrentModuleGroup As String

    Lines = Split(NormalizeLineBreaks(BlueprintContent), vbLf)

    For LineIndex = LBound(Lines) To UBound(Lines)
        RawLine = Lines(LineIndex)
        LineText = Trim$(StripInlineComment(RawLine))

        If Not ComIsBlankText(LineText) Then
            ValidateIndentation RawLine

            If LeadingSpaceCount(RawLine) = 0 Then
                CurrentSection = ParseTopLevelSection(LineText)
                CurrentLayerName = vbNullString
                CurrentModuleGroup = vbNullString
            Else
                Select Case CurrentSection
                    Case SectionBlueprint
                        ParseBlueprintLine RawLine, LineText
                    Case SectionLayers
                        ParseLayersLine RawLine, LineText
                    Case SectionModules
                        ParseModulesLine RawLine, LineText, CurrentLayerName, CurrentModuleGroup
                    Case Else
                        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint content must be inside a supported top-level section."
                End Select
            End If
        End If
    Next LineIndex
End Sub

Private Function ParseTopLevelSection(ByVal LineText As String) As String
    Dim SectionName As String

    If Right$(LineText, 1) <> ":" Or InStr(1, LineText, ":", vbBinaryCompare) <> Len(LineText) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Top-level section must use section_name: syntax."
    End If

    SectionName = RemoveTrailingColon(LineText)
    ValidateTopLevelSection SectionName

    If This.SectionSeen.Exists(SectionName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Duplicate blueprint section: " & SectionName
    End If

    This.SectionSeen.Add SectionName, True
    ParseTopLevelSection = SectionName
End Function

Private Sub ParseBlueprintLine(ByVal RawLine As String, ByVal LineText As String)
    Dim KeyName As String
    Dim KeyValue As String

    RequireIndent RawLine, 2, "blueprint entries must use two-space indentation."
    ParseKeyValue LineText, KeyName, KeyValue

    Select Case KeyName
        Case "name", "version"
            RequireValue KeyValue, "blueprint." & KeyName
            AddUniqueValue This.BlueprintValues, KeyName, KeyValue, "blueprint"
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Unsupported blueprint key: " & KeyName
    End Select
End Sub

Private Sub ParseLayersLine(ByVal RawLine As String, ByVal LineText As String)
    Dim LayerName As String

    RequireIndent RawLine, 2, "layers list items must use two-space indentation."
    If Left$(LineText, 1) <> "-" Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "layers entries must use - item syntax."
    End If

    LayerName = Trim$(Mid$(LineText, 2))
    ValidateLayerName LayerName
    RegisterLayer LayerName
End Sub

Private Sub ParseModulesLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentLayerName As String, _
    ByRef CurrentModuleGroup As String)

    Select Case LeadingSpaceCount(RawLine)
        Case 2
            If Right$(LineText, 1) <> ":" Then
                Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "modules layer entries must use layer_name: syntax."
            End If

            CurrentLayerName = RemoveTrailingColon(LineText)
            ValidateLayerName CurrentLayerName
            If Not This.DefinedLayers.Exists(CurrentLayerName) Then
                Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "modules references undefined layer: " & CurrentLayerName
            End If
            CurrentModuleGroup = vbNullString

        Case 4
            ComRequireText CurrentLayerName, "CurrentLayerName", "Build_BlueprintParser.ParseModulesLine"
            If Right$(LineText, 1) <> ":" Then
                Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "module group entries must use group_name: syntax."
            End If

            CurrentModuleGroup = RemoveTrailingColon(LineText)
            ValidateModuleGroup CurrentModuleGroup

        Case 6
            ComRequireText CurrentLayerName, "CurrentLayerName", "Build_BlueprintParser.ParseModulesLine"
            ComRequireText CurrentModuleGroup, "CurrentModuleGroup", "Build_BlueprintParser.ParseModulesLine"
            AddListManifestItem LineText, CurrentLayerName, CurrentModuleGroup

        Case Else
            Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Invalid modules structure."
    End Select
End Sub

Private Sub AddListManifestItem(ByVal LineText As String, ByVal LayerName As String, ByVal GroupName As String)
    Dim ModuleName As String

    If Left$(LineText, 1) <> "-" Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Module entries must use - item syntax."
    End If

    ModuleName = Trim$(Mid$(LineText, 2))
    If InStr(1, ModuleName, ":", vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Module entries must be scalar values."
    End If

    AddModuleItem LayerName, GroupName, ModuleName
End Sub

Private Sub AddModuleItem(ByVal LayerName As String, ByVal GroupName As String, ByVal ModuleName As String)
    Dim Item As ManifestItem
    Dim ItemKey As String
    Dim Items As Collection

    ValidateLayerName LayerName
    ValidateModuleGroup GroupName
    ValidateModuleName ModuleName

    ItemKey = LCase$(LayerName & "." & ModuleName)
    If This.ItemKeys.Exists(ItemKey) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Duplicate blueprint module: " & ModuleName
    End If

    Set Item = New ManifestItem
    Item.InfInitialize ModuleName, ResolveModuleType(GroupName), LayerName, ResolveTemplatePath(LayerName, GroupName)

    EnsureLayerItems LayerName
    Set Items = This.ItemsByLayer(LayerName)
    Items.Add Item
    This.ItemKeys.Add ItemKey, True
End Sub

Private Sub ValidateBlueprint()
    Dim LayerName As Variant

    RequireSection SectionBlueprint
    RequireSection SectionLayers
    RequireSection SectionModules

    RequireDictionaryValue This.BlueprintValues, "name", "blueprint.name"
    RequireDictionaryValue This.BlueprintValues, "version", "blueprint.version"

    If This.DefinedLayers.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint contains no layers."
    End If

    If This.ItemKeys.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint contains no generation modules."
    End If

    For Each LayerName In This.DefinedLayers.Keys
        If Not This.ItemsByLayer.Exists(CStr(LayerName)) Then
            Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint generation layer contains no modules: " & CStr(LayerName)
        End If
    Next LayerName
End Sub

Private Sub RegisterLayer(ByVal LayerName As String)
    ValidateLayerName LayerName

    If This.DefinedLayers.Exists(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Duplicate blueprint layer: " & LayerName
    End If

    This.DefinedLayers.Add LayerName, True
End Sub

Private Sub EnsureLayerItems(ByVal LayerName As String)
    If Not This.ItemsByLayer.Exists(LayerName) Then
        This.ItemsByLayer.Add LayerName, New Collection
    End If
End Sub

Private Function FormatManifestItem(ByVal Item As ManifestItem) As String
    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "ManifestItem is required."
    End If

    If Not Item.InfValidate() Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "ManifestItem is invalid."
    End If

    FormatManifestItem = _
        Item.InfGetModuleName() & "," & _
        Item.InfGetModuleType() & "," & _
        Item.InfGetLayerName() & "," & _
        Item.InfGetTemplatePath()
End Function

Private Function ResolveModuleType(ByVal GroupName As String) As String
    Select Case GroupName
        Case ModuleGroupStandardModules
            ResolveModuleType = "StandardModule"
        Case ModuleGroupClasses
            ResolveModuleType = "ClassModule"
        Case Else
            Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Unsupported module group: " & GroupName
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

Private Sub ValidateTopLevelSection(ByVal SectionName As String)
    Select Case SectionName
        Case SectionBlueprint, SectionLayers, SectionModules
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Unsupported blueprint section: " & SectionName
End Sub

Private Sub ValidateModuleGroup(ByVal GroupName As String)
    Select Case GroupName
        Case ModuleGroupStandardModules, ModuleGroupClasses
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Unsupported module group: " & GroupName
End Sub

Private Sub ValidateLayerName(ByVal LayerName As String)
    ValidateIdentifier LayerName, "LayerName"
End Sub

Private Sub ValidateModuleName(ByVal ModuleName As String)
    ValidateIdentifier ModuleName, "ModuleName"
End Sub

Private Sub ValidateIdentifier(ByVal Identifier As String, ByVal ValueName As String)
    Dim Position As Long
    Dim Character As String

    ComRequireText Identifier, ValueName, "Build_BlueprintParser.ValidateIdentifier"

    Character = Mid$(Identifier, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", ValueName & " must start with a letter."
    End If

    For Position = 1 To Len(Identifier)
        Character = Mid$(Identifier, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", ValueName & " must contain only letters, numbers, or underscore."
        End If
    Next Position
End Sub

Private Sub ParseKeyValue(ByVal LineText As String, ByRef KeyName As String, ByRef KeyValue As String)
    Dim SeparatorIndex As Long

    SeparatorIndex = InStr(1, LineText, ":", vbBinaryCompare)
    If SeparatorIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint entry must use key: value syntax."
    End If

    KeyName = Trim$(Left$(LineText, SeparatorIndex - 1))
    KeyValue = Trim$(Mid$(LineText, SeparatorIndex + 1))

    If ComIsBlankText(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint key is required."
    End If
End Sub

Private Sub AddUniqueValue(ByVal Values As Object, ByVal KeyName As String, ByVal KeyValue As Variant, ByVal SectionName As String)
    If Values.Exists(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Duplicate " & SectionName & " key: " & KeyName
    End If

    Values.Add KeyName, KeyValue
End Sub

Private Sub RequireSection(ByVal SectionName As String)
    If Not This.SectionSeen.Exists(SectionName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint section is required: " & SectionName
    End If
End Sub

Private Sub RequireDictionaryValue(ByVal Values As Object, ByVal KeyName As String, ByVal ValueName As String)
    If Not Values.Exists(KeyName) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", ValueName & " is required."
    End If
End Sub

Private Sub RequireValue(ByVal KeyValue As String, ByVal ValueName As String)
    If ComIsBlankText(KeyValue) Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", ValueName & " is required."
    End If
End Sub

Private Sub RequireIndent(ByVal RawLine As String, ByVal ExpectedIndent As Long, ByVal ErrorMessage As String)
    If LeadingSpaceCount(RawLine) <> ExpectedIndent Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", ErrorMessage
    End If
End Sub

Private Sub ValidateIndentation(ByVal RawLine As String)
    If InStr(1, RawLine, vbTab, vbBinaryCompare) > 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint indentation must use spaces."
    End If

    If LeadingSpaceCount(RawLine) Mod 2 <> 0 Then
        Err.Raise ComErrInvalidArgument, "Build_BlueprintParser", "Blueprint indentation must use two-space levels."
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
        Err.Raise ComErrInvalidState, "Build_BlueprintParser", "Build_BlueprintParser is not initialized."
    End If
End Sub
