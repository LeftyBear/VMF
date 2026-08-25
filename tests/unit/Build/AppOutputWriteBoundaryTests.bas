Option Explicit
Attribute VB_Name = "AppOutputWriteBoundaryTests"

'=========================================================================
' Module: AppOutputWriteBoundaryTests
' Layer: Application
' Responsibility: Focused tests for post-Generator output-write boundary.
' Dependencies: Application
'=========================================================================

Private Const AppTestAssertErrorNumber As Long = vbObjectError + 9360

Public Sub AppRunOutputWriteBoundaryTests()
    VerifyOutputWriteAcceptsSuccessfulGeneratorOutput
    VerifyFailedGeneratorOutputHardStopsBeforeWrite
    VerifyMissingGeneratedSourceHardStopsBeforeWrite
    VerifyFallbackTemplateSelectionHardStopsBeforeWrite
    VerifyImplicitTemplateSelectionHardStopsBeforeWrite
    VerifyHardStopProducesNoWriteUnits
End Sub

Private Sub VerifyOutputWriteAcceptsSuccessfulGeneratorOutput()
    Dim Result As Object
    Dim WriteUnits As Collection
    Dim Unit As Object

    Set Result = BuildPlan(CreateSuccessfulGeneratorOutput())
    Set WriteUnits = Result("WriteUnits")
    Set Unit = WriteUnits.Item(1)

    AssertTrue CBool(Result("Success")), "Output write plan should accept complete successful Generator output."
    AssertEquals "Success", CStr(Result("Classification")), "Output write classification should be Success."
    AssertEquals 2, WriteUnits.Count, "Output write plan should create one write unit per generated unit."
    AssertEquals "GeneratedSubject", CStr(Unit("moduleName")), "moduleName should be carried."
    AssertEquals "ClassModule", CStr(Unit("moduleType")), "moduleType should be carried."
    AssertEquals "DomainClassTemplate", CStr(Unit("templateKey")), "templateKey should be carried."
    AssertEquals "GeneratedSubject.cls", CStr(Unit("fileName")), "Class module file name should be planned."
    AssertContains CStr(Unit("generatedSource")), "Option Explicit", "generatedSource should be carried."
    AssertEquals "Planned", CStr(Unit("writeStatus")), "Focused implementation should plan, not write."
End Sub

Private Sub VerifyFailedGeneratorOutputHardStopsBeforeWrite()
    Dim Output As Object
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Output("Success") = False

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Failed Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "successful", "Hard-stop should identify failed Generator output."
    AssertEquals 0, Result("WriteUnits").Count, "Failed Generator output should produce no write units."
End Sub

Private Sub VerifyMissingGeneratedSourceHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1).Remove "generatedSource"

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Missing generatedSource should hard-stop."
    AssertContains CStr(Result("Message")), "generatedSource", "Hard-stop should identify missing generated source."
    AssertEquals 0, Result("WriteUnits").Count, "Partial Generator output should produce no write units."
End Sub

Private Sub VerifyFallbackTemplateSelectionHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1)("isFallbackDerived") = True

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Fallback-derived Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "Fallback", "Hard-stop should reject fallback Template selection."
    AssertEquals 0, Result("WriteUnits").Count, "Fallback output should produce no write units."
End Sub

Private Sub VerifyImplicitTemplateSelectionHardStopsBeforeWrite()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1)("isImplicitlySelected") = True

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Implicitly selected Generator output should hard-stop."
    AssertContains CStr(Result("Message")), "Implicit", "Hard-stop should reject implicit Template selection."
    AssertEquals 0, Result("WriteUnits").Count, "Implicit output should produce no write units."
End Sub

Private Sub VerifyHardStopProducesNoWriteUnits()
    Dim Output As Object
    Dim Units As Collection
    Dim Result As Object

    Set Output = CreateSuccessfulGeneratorOutput()
    Set Units = Output("GeneratedUnits")
    Units.Item(1).Remove "moduleName"

    Set Result = BuildPlan(Output)

    AssertFalse CBool(Result("Success")), "Hard-stop should fail."
    AssertEquals 0, Result("WriteUnits").Count, "Hard-stop should produce no output write units."
    AssertContains CStr(Result("Message")), "Output write hard-stop", "Hard-stop should remain at output-write boundary."
End Sub

Private Function BuildPlan(ByVal GeneratorOutput As Object) As Object
    Dim Service As AppOutputWriteService

    Set Service = New AppOutputWriteService
    Set BuildPlan = Service.AppBuildOutputWritePlan(GeneratorOutput)
End Function

Private Function CreateSuccessfulGeneratorOutput() As Object
    Dim Output As Object
    Dim Units As Collection

    Set Units = New Collection
    Units.Add CreateGeneratedUnit(1, "GeneratedSubject", "ClassModule", "DomainClassTemplate")
    Units.Add CreateGeneratedUnit(2, "GeneratedSchedule", "StandardModule", "ModuleTemplate")

    Set Output = CreateObject("Scripting.Dictionary")
    Output("Success") = True
    Output("Classification") = "Success"
    Output("Message") = "Generator output constructed."
    Set Output("GeneratedUnits") = Units

    Set CreateSuccessfulGeneratorOutput = Output
End Function

Private Function CreateGeneratedUnit( _
    ByVal UnitOrder As Long, _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal TemplateKey As String _
) As Object

    Dim Unit As Object

    Set Unit = CreateObject("Scripting.Dictionary")
    Unit("order") = UnitOrder
    Unit("moduleName") = ModuleName
    Unit("moduleType") = ModuleType
    Unit("templateKey") = TemplateKey
    Unit("generatedSource") = "Option Explicit" & vbCrLf & "' Module: " & ModuleName
    Unit("isFallbackDerived") = False
    Unit("isImplicitlySelected") = False

    Set CreateGeneratedUnit = Unit
End Function

Private Sub AssertContains(ByVal TextValue As String, ByVal ExpectedText As String, ByVal Message As String)
    AssertTrue InStr(1, TextValue, ExpectedText, vbTextCompare) > 0, Message & " Text=" & TextValue
End Sub

Private Sub AssertTrue(ByVal Condition As Boolean, ByVal Message As String)
    If Not Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message
    End If
End Sub

Private Sub AssertFalse(ByVal Condition As Boolean, ByVal Message As String)
    If Condition Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message
    End If
End Sub

Private Sub AssertEquals(ByVal Expected As Variant, ByVal Actual As Variant, ByVal Message As String)
    If Expected <> Actual Then
        Err.Raise AppTestAssertErrorNumber, "AppOutputWriteBoundaryTests", Message & " Expected=" & CStr(Expected) & " Actual=" & CStr(Actual)
    End If
End Sub
