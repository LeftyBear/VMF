Option Explicit
Attribute VB_Name = "AppGeneratorPhase1Tests"

'=========================================================================
' Module: AppGeneratorPhase1Tests
' Layer: Application
' Responsibility: Verification tests for Module generator Phase 1
' Dependencies: Common, Build
'=========================================================================

Public Sub AppRunGeneratorPhase1Tests()
    VerifyGenerateModule
End Sub

Private Sub VerifyGenerateModule()
    Dim ModuleName As String
    Dim Result As ComResult
    Dim VBProj As Object
    Dim Comp As Object
    Dim Found As Boolean
    Dim Text As String
    Dim LinesToCheck As Long

    ModuleName = "VMF_TestModule_Phase1"

    ' Ensure cleanup from previous runs
    On Error Resume Next
    Set VBProj = ThisWorkbook.VBProject
    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            VBProj.VBComponents.Remove Comp
            Exit For
        End If
    Next Comp
    On Error GoTo 0

    Set Result = AppGenerateModule(ModuleName)
    If Not Result.IsSuccess Then
        Err.Raise vbObjectError + 9200, "AppGeneratorPhase1Tests", "AppGenerateModule returned failure: " & Result.Message
    End If

    ' Verify existence
    Found = False
    For Each Comp In VBProj.VBComponents
        If StrComp(Comp.Name, ModuleName, vbTextCompare) = 0 Then
            Found = True
            With Comp.CodeModule
                LinesToCheck = IIf(.CountOfLines >= 6, 6, .CountOfLines)
                Text = .Lines(1, LinesToCheck)
            End With
            Exit For
        End If
    Next Comp

    If Not Found Then
        Err.Raise vbObjectError + 9201, "AppGeneratorPhase1Tests", "Generated module not found."
    End If

    If InStr(1, Text, "Option Explicit", vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9202, "AppGeneratorPhase1Tests", "Template missing Option Explicit."
    End If

    If InStr(1, Text, "Module: " & ModuleName, vbTextCompare) = 0 Then
        Err.Raise vbObjectError + 9203, "AppGeneratorPhase1Tests", "Template missing module header with module name."
    End If

    ' Cleanup
    On Error Resume Next
    VBProj.VBComponents.Remove Comp
    On Error GoTo 0
End Sub
