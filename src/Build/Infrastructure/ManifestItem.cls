VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ManifestItem"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ManifestItem
' Layer: Infrastructure
' Responsibility: Represent one manifest-driven generation target.
' Dependencies: Common
'==========================================================================

Private Const ManifestModuleTypeStandard As String = "StandardModule"
Private Const ManifestModuleTypeClass As String = "ClassModule"

Private Type TMember
    ModuleName As String
    ModuleType As String
    LayerName As String
    TemplatePath As String
    BodySourcePath As String
    MemberSourcePath As String
End Type

Private This As TMember
Private SectionSourcePaths As Object

' Initializes a manifest item from parsed manifest values.
Public Sub InfInitialize( _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByVal TemplatePath As String, _
    Optional ByVal BodySourcePath As String = vbNullString)

    ComRequireText ModuleName, "ModuleName", "ManifestItem.InfInitialize"
    ComRequireText ModuleType, "ModuleType", "ManifestItem.InfInitialize"
    ComRequireText LayerName, "LayerName", "ManifestItem.InfInitialize"
    ComRequireText TemplatePath, "TemplatePath", "ManifestItem.InfInitialize"

    This.ModuleName = Trim$(ModuleName)
    This.ModuleType = Trim$(ModuleType)
    This.LayerName = Trim$(LayerName)
    This.TemplatePath = Trim$(TemplatePath)
    This.BodySourcePath = Trim$(BodySourcePath)
    This.MemberSourcePath = vbNullString
    Set SectionSourcePaths = CreateObject("Scripting.Dictionary")

    If Not InfValidate() Then
        Err.Raise ComErrInvalidArgument, "ManifestItem.InfInitialize", "Manifest item is invalid."
    End If
End Sub

' Adds an optional section source path for this target.
Public Sub InfAddSectionSourcePath(ByVal SectionName As String, ByVal SectionSourcePath As String)
    RequireValidState "InfAddSectionSourcePath"
    ComRequireText SectionName, "SectionName", "ManifestItem.InfAddSectionSourcePath"
    ComRequireText SectionSourcePath, "SectionSourcePath", "ManifestItem.InfAddSectionSourcePath"

    SectionName = Trim$(SectionName)
    If Not IsValidSectionName(SectionName) Then
        Err.Raise ComErrInvalidArgument, "ManifestItem.InfAddSectionSourcePath", "SectionName is invalid."
    End If

    If SectionSourcePaths.Exists(SectionName) Then
        Err.Raise ComErrInvalidArgument, "ManifestItem.InfAddSectionSourcePath", "Duplicate SectionName: " & SectionName
    End If

    SectionSourcePaths.Add SectionName, Trim$(SectionSourcePath)
End Sub

' Sets an optional class member definition source path for this target.
Public Sub InfSetMemberSourcePath(ByVal MemberSourcePath As String)
    RequireValidState "InfSetMemberSourcePath"
    ComRequireText MemberSourcePath, "MemberSourcePath", "ManifestItem.InfSetMemberSourcePath"

    This.MemberSourcePath = Trim$(MemberSourcePath)
End Sub

' Returns the target VBA module name.
Public Function InfGetModuleName() As String
    RequireValidState "InfGetModuleName"
    InfGetModuleName = This.ModuleName
End Function

' Returns the target VBA module kind.
Public Function InfGetModuleType() As String
    RequireValidState "InfGetModuleType"
    InfGetModuleType = This.ModuleType
End Function

' Returns the target canonical layer name.
Public Function InfGetLayerName() As String
    RequireValidState "InfGetLayerName"
    InfGetLayerName = This.LayerName
End Function

' Returns the template path for this target.
Public Function InfGetTemplatePath() As String
    RequireValidState "InfGetTemplatePath"
    InfGetTemplatePath = This.TemplatePath
End Function

' Returns the optional body source path for this target.
Public Function InfGetBodySourcePath() As String
    RequireValidState "InfGetBodySourcePath"
    InfGetBodySourcePath = This.BodySourcePath
End Function

' Returns the optional class member definition source path.
Public Function InfGetMemberSourcePath() As String
    RequireValidState "InfGetMemberSourcePath"
    InfGetMemberSourcePath = This.MemberSourcePath
End Function

' Returns optional section source paths keyed by section name.
Public Function InfGetSectionSourcePaths() As Object
    RequireValidState "InfGetSectionSourcePaths"

    If SectionSourcePaths Is Nothing Then
        Set SectionSourcePaths = CreateObject("Scripting.Dictionary")
    End If

    Set InfGetSectionSourcePaths = SectionSourcePaths
End Function

' Creates the token manifest consumed by InfTokenReplacer.
Public Function InfToManifest() As Object
    Dim Manifest As Object

    RequireValidState "InfToManifest"

    Set Manifest = CreateObject("Scripting.Dictionary")
    Manifest("ModuleName") = This.ModuleName
    Manifest("ModuleType") = This.ModuleType
    Manifest("Layer") = This.LayerName
    Manifest("TemplatePath") = This.TemplatePath
    If Not ComIsBlankText(This.BodySourcePath) Then
        Manifest("BodySourcePath") = This.BodySourcePath
    End If
    If Not ComIsBlankText(This.MemberSourcePath) Then
        Manifest("MemberSourcePath") = This.MemberSourcePath
    End If

    Set InfToManifest = Manifest
End Function

' Returns True when this item contains a supported generation target.
Public Function InfValidate() As Boolean
    If ComIsBlankText(This.ModuleName) Then
        InfValidate = False
        Exit Function
    End If

    If Not IsSupportedModuleType(This.ModuleType) Then
        InfValidate = False
        Exit Function
    End If

    If Not IsValidLayerName(This.LayerName) Then
        InfValidate = False
        Exit Function
    End If

    If ComIsBlankText(This.TemplatePath) Then
        InfValidate = False
        Exit Function
    End If

    If SectionSourcePaths Is Nothing Then
        Set SectionSourcePaths = CreateObject("Scripting.Dictionary")
    End If

    InfValidate = True
End Function

Private Sub RequireValidState(ByVal OperationName As String)
    If Not InfValidate() Then
        Err.Raise ComErrInvalidState, "ManifestItem." & OperationName, "Manifest item is not initialized."
    End If
End Sub

Private Function IsSupportedModuleType(ByVal ModuleType As String) As Boolean
    Select Case ModuleType
        Case ManifestModuleTypeStandard, ManifestModuleTypeClass
            IsSupportedModuleType = True
        Case Else
            IsSupportedModuleType = False
    End Select
End Function

Private Function IsValidLayerName(ByVal LayerName As String) As Boolean
    Dim Position As Long
    Dim Character As String

    If ComIsBlankText(LayerName) Then
        IsValidLayerName = False
        Exit Function
    End If

    Character = Mid$(LayerName, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        IsValidLayerName = False
        Exit Function
    End If

    For Position = 1 To Len(LayerName)
        Character = Mid$(LayerName, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            IsValidLayerName = False
            Exit Function
        End If
    Next Position

    IsValidLayerName = True
End Function

Private Function IsValidSectionName(ByVal SectionName As String) As Boolean
    IsValidSectionName = IsValidLayerName(SectionName)
End Function
