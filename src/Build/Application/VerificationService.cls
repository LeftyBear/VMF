VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "VerificationService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: VerificationService
' Layer: Application
' Responsibility: Verify generated VBA component results.
' Dependencies: Common
'=========================================================================

' Verifies the generated component text and header invariants.
Public Function VerifyResult( _
    ByVal ProjectProvider As ProjectProvider, _
    ByVal VBProject As Object, _
    ByVal ModuleName As String, _
    ByVal ModuleType As String, _
    ByVal ExpectedText As String) As ComResult

    Dim Text As String

    If ProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "VerificationService.VerifyResult", "ProjectProvider is required."
    End If

    If VBProject Is Nothing Then
        Err.Raise ComErrInvalidArgument, "VerificationService.VerifyResult", "VBProject is required."
    End If

    ComRequireText ModuleName, "ModuleName", "VerificationService.VerifyResult"
    ComRequireText ModuleType, "ModuleType", "VerificationService.VerifyResult"
    ComRequireText ExpectedText, "ExpectedText", "VerificationService.VerifyResult"

    If Not ProjectProvider.ModuleExists(VBProject, ModuleName) Then
        Set VerifyResult = CreateVerificationFailure("Generated component was not found: " & ModuleName)
        Exit Function
    End If

    Text = ProjectProvider.GetModuleText(VBProject, ModuleName)

    If Len(Text) = 0 Then
        Set VerifyResult = CreateVerificationFailure("Generated component text is empty.")
        Exit Function
    End If

    If CountOptionExplicit(Text) <> 1 Then
        Set VerifyResult = CreateVerificationFailure("Generated component must contain exactly one Option Explicit.")
        Exit Function
    End If

    If Not HasExpectedHeader(Text, ModuleName, ModuleType) Then
        Set VerifyResult = CreateVerificationFailure("Generated component is missing its module header.")
        Exit Function
    End If

    Set VerifyResult = ComCreateSuccess()
End Function

Private Function HasExpectedHeader(ByVal Text As String, ByVal ModuleName As String, ByVal ModuleType As String) As Boolean
    Select Case ModuleType
        Case "StandardModule"
            HasExpectedHeader = (InStr(1, Text, "Module: " & ModuleName, vbTextCompare) > 0)
        Case "ClassModule"
            HasExpectedHeader = (InStr(1, Text, "Class: " & ModuleName, vbTextCompare) > 0)
        Case Else
            HasExpectedHeader = False
    End Select
End Function

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

Private Function CreateVerificationFailure(ByVal Description As String) As ComResult
    Set CreateVerificationFailure = ComCreateFailure( _
        ComCreateErrorInfo(ComErrInvalidState, "VerificationService", Description))
End Function
