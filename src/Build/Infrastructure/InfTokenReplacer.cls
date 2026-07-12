VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfTokenReplacer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfTokenReplacer
' Layer: Infrastructure
' Responsibility: Render template tokens and optional sections from manifest values.
' Dependencies: Common, Infrastructure
'==========================================================================

' Replaces placeholders in an InfTemplate using values from a manifest.
Public Function InfReplaceTokens(ByVal Template As InfTemplate, ByVal Manifest As Object) As String
    Dim TemplateText As String
    Dim ManifestKey As Variant
    Dim ManifestValue As Variant

    If Template Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfReplaceTokens", "Template is required."
    End If

    If Not Template.InfValidate() Then
        Err.Raise ComErrInvalidState, "InfReplaceTokens", "Template is invalid."
    End If

    TemplateText = Template.InfGetTemplateText()
    TemplateText = RenderSectionContracts(TemplateText, Manifest)

    If Manifest Is Nothing Then
        TemplateText = Replace(TemplateText, "{{BODY}}", vbNullString)
        TemplateText = Replace(TemplateText, "{BODY}", vbNullString)
        InfReplaceTokens = TemplateText
        Exit Function
    End If

    For Each ManifestKey In Manifest.Keys
        ManifestValue = Manifest.Item(CStr(ManifestKey))
        TemplateText = Replace(TemplateText, "{{" & CStr(ManifestKey) & "}}", CStr(ManifestValue))
        TemplateText = Replace(TemplateText, "{" & CStr(ManifestKey) & "}", CStr(ManifestValue))
    Next ManifestKey

    TemplateText = Replace(TemplateText, "{{BODY}}", vbNullString)
    TemplateText = Replace(TemplateText, "{BODY}", vbNullString)

    InfReplaceTokens = TemplateText
End Function

Private Function RenderSectionContracts(ByVal TemplateText As String, ByVal Manifest As Object) As String
    Dim SectionStart As Long
    Dim SectionLineStart As Long
    Dim SectionLineEnd As Long
    Dim SectionEnd As Long
    Dim SectionEndLineEnd As Long
    Dim SectionLine As String
    Dim SectionName As String
    Dim SectionBody As String
    Dim BeforeSection As String
    Dim AfterSection As String

    SectionStart = InStr(1, TemplateText, "@section", vbTextCompare)
    Do While SectionStart > 0
        SectionLineStart = InStrRev(TemplateText, vbLf, SectionStart)
        If SectionLineStart = 0 Then
            SectionLineStart = 1
        Else
            SectionLineStart = SectionLineStart + 1
        End If

        SectionLineEnd = InStr(SectionStart, TemplateText, vbLf, vbBinaryCompare)
        If SectionLineEnd = 0 Then
            SectionLineEnd = Len(TemplateText) + 1
        End If

        SectionEnd = InStr(SectionLineEnd, TemplateText, "@endsection", vbTextCompare)
        If SectionEnd = 0 Then
            Err.Raise ComErrInvalidArgument, "InfTokenReplacer.RenderSectionContracts", "Template section is missing @endsection."
        End If

        SectionEndLineEnd = InStr(SectionEnd, TemplateText, vbLf, vbBinaryCompare)
        If SectionEndLineEnd = 0 Then
            SectionEndLineEnd = Len(TemplateText)
        End If

        SectionLine = Mid$(TemplateText, SectionLineStart, SectionLineEnd - SectionLineStart)
        SectionName = ParseSectionName(SectionLine)
        SectionBody = ResolveSectionBody(Manifest, SectionName)

        BeforeSection = Left$(TemplateText, SectionLineStart - 1)
        AfterSection = Mid$(TemplateText, SectionEndLineEnd + 1)

        If ComIsBlankText(SectionBody) Then
            TemplateText = BeforeSection & AfterSection
        Else
            TemplateText = BeforeSection & SectionBody & vbCrLf & AfterSection
        End If

        SectionStart = InStr(1, TemplateText, "@section", vbTextCompare)
    Loop

    RenderSectionContracts = TemplateText
End Function

Private Function ParseSectionName(ByVal SectionLine As String) As String
    Dim MarkerIndex As Long
    Dim SectionName As String

    MarkerIndex = InStr(1, SectionLine, "@section", vbTextCompare)
    If MarkerIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "InfTokenReplacer.ParseSectionName", "Template section marker is invalid."
    End If

    SectionName = Mid$(SectionLine, MarkerIndex + Len("@section"))
    SectionName = Replace(SectionName, vbCr, vbNullString)
    SectionName = Replace(SectionName, vbLf, vbNullString)
    SectionName = Trim$(SectionName)
    ComRequireText SectionName, "SectionName", "InfTokenReplacer.ParseSectionName"

    ParseSectionName = SectionName
End Function

Private Function ResolveSectionBody(ByVal Manifest As Object, ByVal SectionName As String) As String
    If Manifest Is Nothing Then
        ResolveSectionBody = vbNullString
        Exit Function
    End If

    If Manifest.Exists(SectionName) Then
        ResolveSectionBody = CStr(Manifest.Item(SectionName))
        Exit Function
    End If

    If Manifest.Exists("Section:" & SectionName) Then
        ResolveSectionBody = CStr(Manifest.Item("Section:" & SectionName))
        Exit Function
    End If

    ResolveSectionBody = vbNullString
End Function
