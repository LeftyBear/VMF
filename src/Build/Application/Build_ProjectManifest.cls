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

Private Const SectionModules As String = "Modules"
Private Const SectionGeneration As String = "Generation"
Private Const ModuleGroupStandardModules As String = "StandardModules"
Private Const ModuleGroupClasses As String = "Classes"
Private Const ModuleGroupEnums As String = "Enums"

Private Type TMember
    ItemsByLayer As Object
    DefinedLayers As Object
    GenerationEnabled As Object
    ItemKeys As Object
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
    This.SourceRoot = "src"
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
    Dim SectionName As String
    Dim CurrentLayer As String
    Dim CurrentGroup As String

    Lines = Split(NormalizeLineBreaks(ManifestContent), vbLf)

    For LineIndex = LBound(Lines) To UBound(Lines)
        RawLine = Lines(LineIndex)
        LineText = Trim$(StripInlineComment(RawLine))

        If Not ComIsBlankText(LineText) Then
            If IsTopLevelKey(RawLine) Then
                SectionName = Trim$(RemoveTrailingColon(LineText))
                CurrentLayer = vbNullString
                CurrentGroup = vbNullString

                If Left$(LineText, Len("SourceRoot:")) = "SourceRoot:" Then
                    This.SourceRoot = Trim$(Mid$(LineText, Len("SourceRoot:") + 1))
                    If ComIsBlankText(This.SourceRoot) Then
                        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "SourceRoot is required when defined."
                    End If
                End If
            ElseIf SectionName = SectionModules Then
                ParseModulesLine RawLine, LineText, CurrentLayer, CurrentGroup
            ElseIf SectionName = SectionGeneration Then
                ParseGenerationLine RawLine, LineText
            End If
        End If
    Next LineIndex
End Sub

Private Sub ParseModulesLine( _
    ByVal RawLine As String, _
    ByVal LineText As String, _
    ByRef CurrentLayer As String, _
    ByRef CurrentGroup As String)

    Dim Indent As Long
    Dim LayerName As String

    Indent = LeadingSpaceCount(RawLine)

    If Indent = 2 And Right$(LineText, 1) = ":" Then
        LayerName = RemoveTrailingColon(LineText)
        RegisterLayer LayerName
        CurrentLayer = LayerName
        CurrentGroup = vbNullString
    ElseIf Indent = 4 And Right$(LineText, 1) = ":" Then
        CurrentGroup = RemoveTrailingColon(LineText)
        ValidateModuleGroup CurrentGroup
    ElseIf Indent = 4 And IsInlineEmptyList(LineText) Then
        CurrentGroup = Left$(LineText, InStr(1, LineText, ":", vbBinaryCompare) - 1)
        ValidateModuleGroup CurrentGroup
    ElseIf Indent = 6 And Left$(LineText, 1) = "-" Then
        AddModuleItem CurrentLayer, CurrentGroup, Trim$(Mid$(LineText, 2))
    ElseIf Indent >= 4 Then
        ValidateSupportedModuleMetadata CurrentLayer, LineText
    End If
End Sub

Private Sub ParseGenerationLine(ByVal RawLine As String, ByVal LineText As String)
    Dim SeparatorIndex As Long
    Dim LayerName As String
    Dim LayerValue As String

    If LeadingSpaceCount(RawLine) <> 2 Then
        Exit Sub
    End If

    SeparatorIndex = InStr(1, LineText, ":", vbBinaryCompare)
    If SeparatorIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Generation entry must use LayerName: true/false."
    End If

    LayerName = Trim$(Left$(LineText, SeparatorIndex - 1))
    LayerValue = LCase$(Trim$(Mid$(LineText, SeparatorIndex + 1)))
    If LayerValue <> "true" And LayerValue <> "false" Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Generation value must be true or false."
    End If

    This.GenerationEnabled(LayerName) = (LayerValue = "true")
End Sub

Private Sub AddModuleItem(ByVal LayerName As String, ByVal GroupName As String, ByVal ModuleName As String)
    Dim Item As ManifestItem
    Dim ItemKey As String
    Dim Items As Collection

    ComRequireText LayerName, "LayerName", "Build_ProjectManifest.AddModuleItem"
    ComRequireText GroupName, "GroupName", "Build_ProjectManifest.AddModuleItem"
    ComRequireText ModuleName, "ModuleName", "Build_ProjectManifest.AddModuleItem"

    If GroupName = ModuleGroupEnums Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Enum generation is not supported by Build v1.0.2."
    End If

    If Not IsSupportedGenerationLayer(LayerName) Then
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

    If Not This.DefinedLayers.Exists(LayerName) Then
        This.DefinedLayers.Add LayerName, True
    End If
End Sub

Private Sub EnsureLayerItems(ByVal LayerName As String)
    If Not This.ItemsByLayer.Exists(LayerName) Then
        This.ItemsByLayer.Add LayerName, New Collection
    End If
End Sub

Private Sub ValidateManifest()
    Dim LayerName As Variant

    If This.DefinedLayers.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Modules section is required."
    End If

    If This.ItemKeys.Count = 0 Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Manifest contains no supported generation targets."
    End If

    For Each LayerName In This.GenerationEnabled.Keys
        If CBool(This.GenerationEnabled(LayerName)) And Not IsSupportedGenerationLayer(CStr(LayerName)) Then
            Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported generation layer: " & CStr(LayerName)
        End If
    Next LayerName

    For Each LayerName In This.DefinedLayers.Keys
        If IsSupportedGenerationLayer(CStr(LayerName)) And IsLayerGenerationEnabled(CStr(LayerName)) Then
            If Not This.ItemsByLayer.Exists(CStr(LayerName)) Then
                Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Generation layer contains no targets: " & CStr(LayerName)
            End If
        End If
    Next LayerName
End Sub

Private Sub ValidateModuleGroup(ByVal GroupName As String)
    Select Case GroupName
        Case ModuleGroupStandardModules, ModuleGroupClasses, ModuleGroupEnums
            Exit Sub
    End Select

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported module group: " & GroupName
End Sub

Private Sub ValidateSupportedModuleMetadata(ByVal LayerName As String, ByVal LineText As String)
    If ComIsBlankText(LayerName) Then
        Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Module metadata must be defined inside a layer."
    End If

    If Left$(LineText, Len("Status:")) = "Status:" Then
        Exit Sub
    End If

    If Left$(LineText, Len("GenerateCommand:")) = "GenerateCommand:" Then
        Exit Sub
    End If

    Err.Raise ComErrInvalidArgument, "Build_ProjectManifest", "Unsupported manifest definition: " & LineText
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
    If GroupName = ModuleGroupStandardModules Then
        ResolveTemplatePath = "templates\ModuleTemplate.txt"
    ElseIf LayerName = "Domain" Then
        ResolveTemplatePath = "templates\DomainClassTemplate.txt"
    Else
        ResolveTemplatePath = "templates\ClassTemplate.txt"
    End If
End Function

Private Function IsSupportedGenerationLayer(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Common", "Manifest", "Infrastructure", "Domain", "Application", "Presentation"
            IsSupportedGenerationLayer = True
        Case Else
            IsSupportedGenerationLayer = False
    End Select
End Function

Private Function IsLayerGenerationEnabled(ByVal LayerName As String) As Boolean
    If This.GenerationEnabled.Exists(LayerName) Then
        IsLayerGenerationEnabled = CBool(This.GenerationEnabled(LayerName))
    Else
        IsLayerGenerationEnabled = True
    End If
End Function

Private Function IsTopLevelKey(ByVal RawLine As String) As Boolean
    IsTopLevelKey = (LeadingSpaceCount(RawLine) = 0 And InStr(1, Trim$(RawLine), ":", vbBinaryCompare) > 0)
End Function

Private Function IsInlineEmptyList(ByVal LineText As String) As Boolean
    IsInlineEmptyList = (InStr(1, LineText, ":", vbBinaryCompare) > 0 And Right$(LineText, 2) = "[]")
End Function

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
