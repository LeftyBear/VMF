Option Explicit
Attribute VB_Name = "AppGeneratorPhase2Tests"

'=========================================================================
' Module: AppGeneratorPhase2Tests
' Layer: Application
' Responsibility: Verification tests for Module generator Phase 2
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Public Sub AppRunGeneratorPhase2Tests()
    VerifyGenerateClass
End Sub

Private Sub VerifyGenerateClass()
    Dim ClassName As String
    Dim Result As ComResult
    Dim ProjectProvider As InfVbaProjectProvider
    Dim Text As String

    ClassName = "VMF_TestClass_Phase2"
    Set ProjectProvider = InfCreateVbaProjectProvider()

    ProjectProvider.InfRemoveModule ClassName

    Set Result = AppGenerateClass(ClassName)
    If Not Result.IsSuccess Then
        Err.Raise vbObjectError + 9300, "AppGeneratorPhase2Tests", "AppGenerateClass returned failure: " & Result.Message
    End If

    If Not ProjectProvider.InfModuleExists(ClassName) Then
        Err.Raise vbObjectError + 9301, "AppGeneratorPhase2Tests", "Generated class not found."
    End If

    Text = ProjectProvider.InfGetModuleText(ClassName)

    If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9302, "AppGeneratorPhase2Tests", "Template missing Option Explicit."
    End If

    If CountOptionExplicit(Text) <> 1 Then
        Err.Raise vbObjectError + 9305, "AppGeneratorPhase2Tests", "Generated class should contain exactly one Option Explicit."
    End If

    If InStr(1, Text, "Class: " & ClassName, vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9303, "AppGeneratorPhase2Tests", "Template missing class header with class name."
    End If

    If InStr(1, Text, "Layer: Application", vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9304, "AppGeneratorPhase2Tests", "Template missing canonical layer header."
    End If

    ProjectProvider.InfRemoveModule ClassName
End Sub

Private Function CountOptionExplicit(ByVal Text As String) As Long
    Dim Lines() As String
    Dim NormalizedText As String
    Dim i As Long

    NormalizedText = Replace(Text, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)
    Lines = Split(NormalizedText, vbLf)

    For i = LBound(Lines) To UBound(Lines)
        If StrComp(Trim$(Lines(i)), "Option Explicit", vbTextCompare) = 0 Then
            CountOptionExplicit = CountOptionExplicit + 1
        End If
    Next i
End Function
