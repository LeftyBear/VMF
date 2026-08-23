Option Explicit
Attribute VB_Name = "AppGeneratorContextBoundaryTests"

'=========================================================================
' Module: AppGeneratorContextBoundaryTests
' Layer: Application
' Responsibility: Focused tests for Generator GenerateContext input boundary.
' Dependencies: Application, Infrastructure
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9350

Public Sub AppRunGeneratorContextBoundaryTests()
    VerifyGeneratorAcceptsSuccessfulGenerateContext
    VerifyMissingGenerateContextHardStopsBeforeGeneration
    VerifyFailedGenerateContextHardStopsBeforeGeneration
    VerifyPartialGenerateContextHardStopsBeforeGeneration
    VerifyMissingGenerationOrderHardStopsBeforeGeneration
End Sub

Private Sub VerifyGeneratorAcceptsSuccessfulGenerateContext()
    Dim ModuleName As String
    Dim Result As ComResult
    Dim ProjectProvider As InfVbaProjectProvider
    Dim Text As String

    ModuleName = "VMF_P5_11_ContextAccepted"
    Set ProjectProvider = InfCreateVbaProjectProvider()
    ActivateTestTargetWorkbook
    ProjectProvider.InfRemoveModule ModuleName

    Set Result = CreateGeneratorService().AppGenerateFromContext(CreateSuccessfulGenerateContext(ModuleName))

    AssertTrue Result.IsSuccess, "Generator should accept complete successful GenerateContext input. Message=" & Result.Message
    AssertTrue ProjectProvider.InfModuleExists(ModuleName), "Generator should create module for accepted context."

    Text = ProjectProvider.InfGetModuleText(ModuleName)
    AssertContains Text, "Module: " & ModuleName, "Generated module should use GenerateContext moduleName."
    AssertContains Text, "Layer: Application", "Generated module should use GenerateContext layerName."

    ProjectProvider.InfRemoveModule ModuleName
End Sub

Private Sub VerifyMissingGenerateContextHardStopsBeforeGeneration()
    Dim ModuleName As String
    Dim Context As Object
    Dim Result As ComResult

    ModuleName = "VMF_P5_11_MissingContext"

    Set Result = CreateGeneratorService().AppGenerateFromContext(Context)

    AssertFalse Result.IsSuccess, "Missing GenerateContext should hard-stop."
    AssertContains Result.Message, "GenerateContext input is required", "Missing input should be identified."
    AssertModuleAbsent ModuleName, "Missing GenerateContext should not generate output."
End Sub

Private Sub VerifyFailedGenerateContextHardStopsBeforeGeneration()
    Dim ModuleName As String
    Dim Context As Object
    Dim Result As ComResult

    ModuleName = "VMF_P5_11_FailedContext"
    Set Context = CreateSuccessfulGenerateContext(ModuleName)
    Context("Success") = False

    Set Result = CreateGeneratorService().AppGenerateFromContext(Context)

    AssertFalse Result.IsSuccess, "Failed GenerateContext should hard-stop."
    AssertContains Result.Message, "must be successful", "Failed context should be identified."
    AssertModuleAbsent ModuleName, "Failed GenerateContext should not generate output."
End Sub

Private Sub VerifyPartialGenerateContextHardStopsBeforeGeneration()
    Dim ModuleName As String
    Dim Context As Object
    Dim Units As Collection
    Dim Result As ComResult

    ModuleName = "VMF_P5_11_PartialContext"
    Set Context = CreateSuccessfulGenerateContext(ModuleName)
    Set Units = Context("GenerationUnits")
    Units.Item(1).Remove "templatePath"

    Set Result = CreateGeneratorService().AppGenerateFromContext(Context)

    AssertFalse Result.IsSuccess, "Partial GenerateContext should hard-stop."
    AssertContains Result.Message, "templatePath", "Missing Template binding should be identified."
    AssertModuleAbsent ModuleName, "Partial GenerateContext should not generate output."
End Sub

Private Sub VerifyMissingGenerationOrderHardStopsBeforeGeneration()
    Dim ModuleName As String
    Dim Context As Object
    Dim Units As Collection
    Dim Result As ComResult

    ModuleName = "VMF_P5_11_MissingOrder"
    Set Context = CreateSuccessfulGenerateContext(ModuleName)
    Set Units = Context("GenerationUnits")
    Units.Item(1).Remove "order"

    Set Result = CreateGeneratorService().AppGenerateFromContext(Context)

    AssertFalse Result.IsSuccess, "Missing deterministic generation order should hard-stop."
    AssertContains Result.Message, "order", "Missing generation order should be identified."
    AssertModuleAbsent ModuleName, "Missing generation order should not generate output."
End Sub

Private Function CreateSuccessfulGenerateContext(ByVal ModuleName As String) As Object
    Dim Builder As AppGenerateContextBuilder
    Dim Items As Collection

    Set Builder = New AppGenerateContextBuilder
    Set Items = New Collection
    Items.Add CreateApprovedTemplateDerivationItem(ModuleName)

    Set CreateSuccessfulGenerateContext = Builder.AppBuildGenerateContext(Items)
End Function

Private Function CreateGeneratorService() As AppGeneratorService
    Dim CompositionRoot As AppCompositionRoot

    Set CompositionRoot = New AppCompositionRoot
    Set CreateGeneratorService = CompositionRoot.AppCreateGeneratorService()
End Function

Private Function CreateApprovedTemplateDerivationItem(ByVal ModuleName As String) As Object
    Dim Item As Object

    Set Item = CreateObject("Scripting.Dictionary")
    Item("moduleName") = ModuleName
    Item("moduleType") = "StandardModule"
    Item("layerName") = "Application"
    Item("templateKey") = "ModuleTemplate"
    Item("templatePath") = BuildPathResolver.CombinePath(BuildPathResolver.TemplatesDirectoryPath(), "ModuleTemplate.txt")
    Item("templateRole") = "StandardModule"
    Item("selectionRuleId") = "P5-02-STANDARD"
    Item("derivationReason") = "approved manifest module type and layer"
    Item("isApproved") = True
    Item("isGeneratable") = True
    Item("isFallbackDerived") = False
    Item("isImplicitlySelected") = False

    Set CreateApprovedTemplateDerivationItem = Item
End Function

Private Sub ActivateTestTargetWorkbook()
    On Error Resume Next
    Application.Workbooks("VMF.xlam").Activate
    On Error GoTo 0
End Sub

Private Sub AssertModuleAbsent(ByVal ModuleName As String, ByVal Message As String)
    Dim ProjectProvider As InfVbaProjectProvider

    ActivateTestTargetWorkbook
    Set ProjectProvider = InfCreateVbaProjectProvider()
    If ProjectProvider.InfModuleExists(ModuleName) Then
        ProjectProvider.InfRemoveModule ModuleName
        Err.Raise AppTestAssertErrorNumber, "AppGeneratorContextBoundaryTests", Message
    End If
End Sub

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppGeneratorContextBoundaryTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppGeneratorContextBoundaryTests", Message
    End If
End Sub
