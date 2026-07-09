Option Explicit
Attribute VB_Name = "AppGeneratorPhase1Tests"

'=========================================================================
' Module: AppGeneratorPhase1Tests
' Layer: Application
' Responsibility: Verification tests for Module generator Phase 1
' Dependencies: Common, Build, Infrastructure
'=========================================================================

Public Sub AppRunGeneratorPhase1Tests()
    VerifyGenerateModule
End Sub

Private Sub VerifyGenerateModule()
    Dim ModuleName As String
    Dim Result As ComResult
    Dim ProjectProvider As InfVbaProjectProvider
    Dim Text As String

    ModuleName = "VMF_TestModule_Phase1"
    Set ProjectProvider = InfCreateVbaProjectProvider()

    ProjectProvider.InfRemoveModule ModuleName

    Set Result = AppGenerateModule(ModuleName)
    If Not Result.IsSuccess Then
        Err.Raise vbObjectError + 9200, "AppGeneratorPhase1Tests", "AppGenerateModule returned failure: " & Result.Message
    End If

    If Not ProjectProvider.InfModuleExists(ModuleName) Then
        Err.Raise vbObjectError + 9201, "AppGeneratorPhase1Tests", "Generated module not found."
    End If

    Text = ProjectProvider.InfGetModuleText(ModuleName)

    If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9202, "AppGeneratorPhase1Tests", "Template missing Option Explicit."
    End If

    If CountOptionExplicit(Text) <> 1 Then
        Err.Raise vbObjectError + 9205, "AppGeneratorPhase1Tests", "Generated module should contain exactly one Option Explicit."
    End If

    If InStr(1, Text, "Module: " & ModuleName, vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9203, "AppGeneratorPhase1Tests", "Template missing module header with module name."
    End If

    If InStr(1, Text, "Layer: Application", vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9204, "AppGeneratorPhase1Tests", "Template missing canonical layer header."
    End If

    ProjectProvider.InfRemoveModule ModuleName
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
