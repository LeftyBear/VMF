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
End Type

Private This As TMember

' Initializes a manifest item from parsed manifest values.
Public Sub InfInitialize( _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByVal TemplatePath As String)

    ComRequireText ModuleName, "ModuleName", "ManifestItem.InfInitialize"
    ComRequireText ModuleType, "ModuleType", "ManifestItem.InfInitialize"
    ComRequireText LayerName, "LayerName", "ManifestItem.InfInitialize"
    ComRequireText TemplatePath, "TemplatePath", "ManifestItem.InfInitialize"

    This.ModuleName = Trim$(ModuleName)
    This.ModuleType = Trim$(ModuleType)
    This.LayerName = Trim$(LayerName)
    This.TemplatePath = Trim$(TemplatePath)

    If Not InfValidate() Then
        Err.Raise ComErrInvalidArgument, "ManifestItem.InfInitialize", "Manifest item is invalid."
    End If
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

' Creates the token manifest consumed by InfTokenReplacer.
Public Function InfToManifest() As Object
    Dim Manifest As Object

    RequireValidState "InfToManifest"

    Set Manifest = CreateObject("Scripting.Dictionary")
    Manifest("ModuleName") = This.ModuleName
    Manifest("ModuleType") = This.ModuleType
    Manifest("Layer") = This.LayerName
    Manifest("TemplatePath") = This.TemplatePath

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

    If Not IsSupportedLayerName(This.LayerName) Then
        InfValidate = False
        Exit Function
    End If

    If ComIsBlankText(This.TemplatePath) Then
        InfValidate = False
        Exit Function
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

Private Function IsSupportedLayerName(ByVal LayerName As String) As Boolean
    Select Case LayerName
        Case "Presentation", "Application", "Domain", "Infrastructure", "Common", "Manifest"
            IsSupportedLayerName = True
        Case Else
            IsSupportedLayerName = False
    End Select
End Function
