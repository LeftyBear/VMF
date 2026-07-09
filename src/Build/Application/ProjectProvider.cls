VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ProjectProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ProjectProvider
' Layer: Application
' Responsibility: Resolve and protect the generation target VBProject.
' Dependencies: Common, Infrastructure
'=========================================================================

Private VbaProjectProvider As InfVbaProjectProvider

' Initializes the provider with Infrastructure project operations.
Public Sub AppInitialize(ByVal Provider As InfVbaProjectProvider)
    If Provider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "ProjectProvider.AppInitialize", "Provider is required."
    End If

    Set VbaProjectProvider = Provider
End Sub

' Resolves the external target VBProject.
Public Function ResolveTargetProject() As Object
    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "ProjectProvider.ResolveTargetProject", "ProjectProvider is not initialized."
    End If

    Set ResolveTargetProject = VbaProjectProvider.InfResolveTargetVBProject()
End Function

' Returns True when ModuleName already exists in VBProject.
Public Function ModuleExists(ByVal VBProject As Object, ByVal ModuleName As String) As Boolean
    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "ProjectProvider.ModuleExists", "ProjectProvider is not initialized."
    End If

    ModuleExists = VbaProjectProvider.InfModuleExistsInProject(VBProject, ModuleName)
End Function

' Removes a component from VBProject when it exists.
Public Sub RemoveComponent(ByVal VBProject As Object, ByVal ModuleName As String)
    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "ProjectProvider.RemoveComponent", "ProjectProvider is not initialized."
    End If

    VbaProjectProvider.InfRemoveComponentFromProject VBProject, ModuleName
End Sub

' Returns all source text from ModuleName in VBProject.
Public Function GetModuleText(ByVal VBProject As Object, ByVal ModuleName As String) As String
    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "ProjectProvider.GetModuleText", "ProjectProvider is not initialized."
    End If

    GetModuleText = VbaProjectProvider.InfGetModuleTextFromProject(VBProject, ModuleName)
End Function
