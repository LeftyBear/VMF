VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "BuildGenerationEngine"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: BuildGenerationEngine
' Layer: Application
' Responsibility: Orchestrate Build VBA component generation.
' Dependencies: Common, Application
'=========================================================================

Private ProjectProviderService As ProjectProvider
Private ComponentFactoryService As ComponentFactory
Private HeaderNormalizerService As HeaderNormalizer
Private AttributeWriterService As AttributeWriter
Private SourceWriterService As SourceWriter
Private VerificationServiceInstance As VerificationService

' Initializes the generation engine with its collaborators.
Public Sub AppInitialize( _
    ByVal Provider As ProjectProvider, _
    ByVal Factory As ComponentFactory, _
    ByVal Normalizer As HeaderNormalizer, _
    ByVal AttributeService As AttributeWriter, _
    ByVal SourceService As SourceWriter, _
    ByVal Verifier As VerificationService)

    If Provider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "ProjectProvider is required."
    End If

    If Factory Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "ComponentFactory is required."
    End If

    If Normalizer Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "HeaderNormalizer is required."
    End If

    If AttributeService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "AttributeWriter is required."
    End If

    If SourceService Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "SourceWriter is required."
    End If

    If Verifier Is Nothing Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.AppInitialize", "VerificationService is required."
    End If

    Set ProjectProviderService = Provider
    Set ComponentFactoryService = Factory
    Set HeaderNormalizerService = Normalizer
    Set AttributeWriterService = AttributeService
    Set SourceWriterService = SourceService
    Set VerificationServiceInstance = Verifier
End Sub

' Generates one component through the canonical generation flow.
Public Function GenerateComponent( _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal CodeText As String) As ComResult

    Dim VBProject As Object
    Dim Component As Object
    Dim NormalizedText As String
    Dim Result As ComResult

    On Error GoTo ErrHandler

    EnsureInitialized
    ComRequireText ModuleName, "ModuleName", "BuildGenerationEngine.GenerateComponent"
    ComRequireText ModuleType, "ModuleType", "BuildGenerationEngine.GenerateComponent"
    ComRequireText CodeText, "CodeText", "BuildGenerationEngine.GenerateComponent"

    Set VBProject = ProjectProviderService.ResolveTargetProject()
    If ProjectProviderService.ModuleExists(VBProject, ModuleName) Then
        Err.Raise ComErrInvalidArgument, "BuildGenerationEngine.GenerateComponent", "Module already exists: " & ModuleName
    End If

    Set Component = ComponentFactoryService.CreateComponent(VBProject, ModuleType)
    NormalizedText = HeaderNormalizerService.NormalizeSource(CodeText)
    AttributeWriterService.WriteAttributes Component, ModuleName
    SourceWriterService.WriteSource Component, NormalizedText

    Set Result = VerificationServiceInstance.VerifyResult(ProjectProviderService, VBProject, ModuleName, ModuleType, NormalizedText)
    If Result.IsFailure Then
        Err.Raise ComErrInvalidState, "BuildGenerationEngine.GenerateComponent", Result.Message
    End If

    Select Case ModuleType
        Case "StandardModule"
            Set GenerateComponent = ComCreateSuccess("Module created: " & ModuleName)
        Case "ClassModule"
            Set GenerateComponent = ComCreateSuccess("Class module created: " & ModuleName)
        Case Else
            Set GenerateComponent = ComCreateSuccess("Component created: " & ModuleName)
    End Select
    Exit Function

ErrHandler:
    Dim Description As String

    Description = Err.Description

    On Error Resume Next
    If Not VBProject Is Nothing Then
        ProjectProviderService.RemoveComponent VBProject, ModuleName
    End If
    On Error GoTo 0

    Set GenerateComponent = ComCreateFailureFromErr("BuildGenerationEngine.GenerateComponent", Description, "Error", False)
End Function

Private Sub EnsureInitialized()
    If ProjectProviderService Is Nothing _
        Or ComponentFactoryService Is Nothing _
        Or HeaderNormalizerService Is Nothing _
        Or AttributeWriterService Is Nothing _
        Or SourceWriterService Is Nothing _
        Or VerificationServiceInstance Is Nothing Then

        Err.Raise ComErrInvalidState, "BuildGenerationEngine", "BuildGenerationEngine is not initialized."
    End If
End Sub
