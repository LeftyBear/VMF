Option Explicit
Attribute VB_Name = "AppGenerateContextTests"

'=========================================================================
' Module: AppGenerateContextTests
' Layer: Application
' Responsibility: Focused tests for GenerateContext boundary construction.
' Dependencies: Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9340

Public Sub AppRunGenerateContextTests()
    VerifySuccessfulGenerateContextConstruction
    VerifyApprovedOrderingIsPreserved
    VerifyMissingTemplateDerivationOutputHardStops
    VerifyMissingRequiredFieldHardStops
    VerifyBlankManifestFactHardStops
    VerifyUnapprovedTemplateDerivationHardStops
    VerifyNonGeneratableTemplateDerivationHardStops
    VerifyFallbackTemplateSelectionHardStops
    VerifyImplicitTemplateSelectionHardStops
    VerifyHardStopProducesNoGeneratorInput
End Sub

Private Sub VerifySuccessfulGenerateContextConstruction()
    Dim Result As Object
    Dim Units As Collection
    Dim Unit As Object

    Set Result = BuildContext(CreateApprovedItems())
    Set Units = Result("GenerationUnits")
    Set Unit = Units.Item(1)

    AssertTrue CBool(Result("Success")), "GenerateContext should succeed for complete approved input."
    AssertEquals "Success", CStr(Result("Classification")), "GenerateContext classification should be Success."
    AssertEquals 2, Units.Count, "GenerateContext should create one generation unit per item."
    AssertEquals "Subject", CStr(Unit("moduleName")), "moduleName should be carried."
    AssertEquals "ClassModule", CStr(Unit("moduleType")), "moduleType should be carried."
    AssertEquals "Domain", CStr(Unit("layerName")), "layerName should be carried."
    AssertEquals "DomainClassTemplate", CStr(Unit("templateKey")), "templateKey should be carried."
    AssertEquals "..\..\templates\DomainClassTemplate.txt", CStr(Unit("templatePath")), "templatePath should be carried."
    AssertEquals "DomainClass", CStr(Unit("templateRole")), "templateRole should be carried."
    AssertEquals "P5-02-DOMAIN-CLASS", CStr(Unit("selectionRuleId")), "selectionRuleId should be carried."
    AssertEquals "approved manifest module type and layer", CStr(Unit("derivationReason")), "derivationReason should be carried."
End Sub

Private Sub VerifyApprovedOrderingIsPreserved()
    Dim Result As Object
    Dim Units As Collection

    Set Result = BuildContext(CreateApprovedItems())
    Set Units = Result("GenerationUnits")

    AssertEquals "Subject", CStr(Units.Item(1)("moduleName")), "First approved item should remain first."
    AssertEquals "ScheduleService", CStr(Units.Item(2)("moduleName")), "Second approved item should remain second."
    AssertEquals 1, CLng(Units.Item(1)("order")), "First unit order should be preserved."
    AssertEquals 2, CLng(Units.Item(2)("order")), "Second unit order should be preserved."
End Sub

Private Sub VerifyMissingTemplateDerivationOutputHardStops()
    Dim Result As Object
    Dim Items As Collection

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Absent Template Derivation output should hard-stop."
    AssertEquals "HardStop", CStr(Result("Classification")), "Absent input should be classified as hard-stop."
    AssertContains CStr(Result("Message")), "required", "Hard-stop should explain missing input."
End Sub

Private Sub VerifyMissingRequiredFieldHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1).Remove "templatePath"

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Missing required field should hard-stop."
    AssertContains CStr(Result("Message")), "templatePath", "Hard-stop should identify missing field."
End Sub

Private Sub VerifyBlankManifestFactHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1)("layerName") = vbNullString

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Blank manifest fact should hard-stop."
    AssertContains CStr(Result("Message")), "layerName", "Hard-stop should identify blank manifest fact."
End Sub

Private Sub VerifyUnapprovedTemplateDerivationHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1)("isApproved") = False

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Unapproved Template Derivation output should hard-stop."
    AssertContains CStr(Result("Message")), "unapproved", "Hard-stop should preserve unapproved classification."
End Sub

Private Sub VerifyNonGeneratableTemplateDerivationHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1)("isGeneratable") = False

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Non-generatable Template Derivation output should hard-stop."
    AssertContains CStr(Result("Message")), "non-generatable", "Hard-stop should preserve non-generatable classification."
End Sub

Private Sub VerifyFallbackTemplateSelectionHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1)("isFallbackDerived") = True

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Fallback Template selection should hard-stop."
    AssertContains CStr(Result("Message")), "Fallback", "Hard-stop should reject fallback Template selection."
End Sub

Private Sub VerifyImplicitTemplateSelectionHardStops()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1)("isImplicitlySelected") = True

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Implicit Template selection should hard-stop."
    AssertContains CStr(Result("Message")), "Implicit", "Hard-stop should reject implicit Template selection."
End Sub

Private Sub VerifyHardStopProducesNoGeneratorInput()
    Dim Items As Collection
    Dim Result As Object

    Set Items = CreateApprovedItems()
    Items.Item(1).Remove "moduleName"

    Set Result = BuildContext(Items)

    AssertFalse CBool(Result("Success")), "Hard-stop should fail."
    AssertEquals 0, Result("GenerationUnits").Count, "Hard-stop should produce no Generator input."
    AssertContains CStr(Result("Message")), "GenerateContext hard-stop", "Hard-stop should remain before Generator."
End Sub

Private Function BuildContext(ByVal Items As Collection) As Object
    Dim Builder As AppGenerateContextBuilder

    Set Builder = New AppGenerateContextBuilder
    Set BuildContext = Builder.AppBuildGenerateContext(Items)
End Function

Private Function CreateApprovedItems() As Collection
    Dim Items As Collection

    Set Items = New Collection
    Items.Add CreateApprovedItem("Subject", "ClassModule", "Domain", "DomainClassTemplate", "..\..\templates\DomainClassTemplate.txt", "DomainClass", "P5-02-DOMAIN-CLASS")
    Items.Add CreateApprovedItem("ScheduleService", "StandardModule", "Application", "ModuleTemplate", "..\..\templates\ModuleTemplate.txt", "StandardModule", "P5-02-STANDARD")

    Set CreateApprovedItems = Items
End Function

Private Function CreateApprovedItem( _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal LayerName As String, _
    ByVal TemplateKey As String, _
    ByVal TemplatePath As String, _
    ByVal TemplateRole As String, _
    ByVal SelectionRuleId As String _
) As Object

    Dim Item As Object

    Set Item = CreateObject("Scripting.Dictionary")
    Item("moduleName") = ModuleName
    Item("moduleType") = ModuleType
    Item("layerName") = LayerName
    Item("templateKey") = TemplateKey
    Item("templatePath") = TemplatePath
    Item("templateRole") = TemplateRole
    Item("selectionRuleId") = SelectionRuleId
    Item("derivationReason") = "approved manifest module type and layer"
    Item("isApproved") = True
    Item("isGeneratable") = True
    Item("isFallbackDerived") = False
    Item("isImplicitlySelected") = False

    Set CreateApprovedItem = Item
End Function

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppGenerateContextTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppGenerateContextTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppGenerateContextTests", Message & " Expected=" & CStr(Expected) & " Actual=" & CStr(Actual)
    End If
End Sub
