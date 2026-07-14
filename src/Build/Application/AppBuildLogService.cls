VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppBuildLogService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppBuildLogService
' Layer: Application
' Responsibility: Build log projection from structured generate results.
' Dependencies: Common, Application
'=========================================================================

Public Function AppRenderBuildLog(ByVal GenerateResult As AppGenerateResult) As String
    Dim ModuleResult As AppModuleGenerateResult
    Dim TextValue As String

    If GenerateResult Is Nothing Then
        AppRenderBuildLog = "No build log."
        Exit Function
    End If

    For Each ModuleResult In GenerateResult.ModuleResults
        AppendLine TextValue, _
            Format$(ModuleResult.LoggedAt, "hh:nn:ss") & "  " & _
            PadRight(SeverityForStatus(ModuleResult.Status), 8) & " " & _
            PadRight(ModuleResult.ModuleName, 20) & " " & _
            PadRight(ModuleResult.Status, 10) & " " & _
            ModuleResult.Message & "  " & ModuleResult.OutputPath
    Next ModuleResult

    If ComIsBlankText(TextValue) Then
        TextValue = Format$(GenerateResult.FinishedAt, "hh:nn:ss") & "  INFO     Project              Skipped    No modules selected.  " & GenerateResult.OutputDirectory
    End If

    AppRenderBuildLog = TextValue
End Function

Private Function SeverityForStatus(ByVal Status As String) As String
    Select Case Status
        Case "Generated"
            SeverityForStatus = "INFO"
        Case "Skipped"
            SeverityForStatus = "WARNING"
        Case "Failed"
            SeverityForStatus = "ERROR"
        Case Else
            SeverityForStatus = "INFO"
    End Select
End Function

Private Function PadRight(ByVal TextValue As String, ByVal Width As Long) As String
    If Len(TextValue) >= Width Then
        PadRight = TextValue
    Else
        PadRight = TextValue & Space$(Width - Len(TextValue))
    End If
End Function

Private Sub AppendLine(ByRef TextValue As String, ByVal LineText As String)
    If Len(TextValue) > 0 Then
        TextValue = TextValue & vbCrLf
    End If
    TextValue = TextValue & LineText
End Sub
