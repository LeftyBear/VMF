VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ComponentFactory"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ComponentFactory
' Layer: Application
' Responsibility: Create VBA components for generation.
' Dependencies: Common
'=========================================================================

Private Const ComponentTypeStandardModule As Long = 1
Private Const ComponentTypeClassModule As Long = 2

' Creates a VBA component of ModuleType in VBProject.
Public Function CreateComponent(ByVal VBProject As Object, ByVal ModuleType As String) As Object
    If VBProject Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ComponentFactory.CreateComponent", "VBProject is required."
    End If

    ComRequireText ModuleType, "ModuleType", "ComponentFactory.CreateComponent"

    Select Case ModuleType
        Case "StandardModule"
            Set CreateComponent = VBProject.VBComponents.Add(ComponentTypeStandardModule)
        Case "ClassModule"
            Set CreateComponent = VBProject.VBComponents.Add(ComponentTypeClassModule)
        Case Else
            Err.Raise ComErrInvalidArgument, "ComponentFactory.CreateComponent", "ModuleType is not supported."
    End Select
End Function
