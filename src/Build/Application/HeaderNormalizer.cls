VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "HeaderNormalizer"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: HeaderNormalizer
' Layer: Application
' Responsibility: Normalize generated source headers before writing.
' Dependencies: Common
'=========================================================================

' Returns source text with exactly one Option Explicit and no Attribute lines.
Public Function NormalizeSource(ByVal CodeText As String) As String
    Dim Lines() As String
    Dim Result As String
    Dim LineText As String
    Dim NormalizedText As String
    Dim i As Long
    Dim HasBody As Boolean

    ComRequireText CodeText, "CodeText", "HeaderNormalizer.NormalizeSource"

    NormalizedText = Replace(CodeText, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)
    Lines = Split(NormalizedText, vbLf)

    Result = "Option Explicit"
    HasBody = False

    For i = LBound(Lines) To UBound(Lines)
        LineText = Lines(i)

        If IsOptionExplicitLine(LineText) Then
            ' Skip all source declarations; this service writes the canonical one.
        ElseIf IsAttributeLine(LineText) Then
            ' Attributes are handled separately by AttributeWriter.
        ElseIf Not HasBody And Len(Trim$(LineText)) = 0 Then
            ' Drop leading blank lines before the canonical header.
        Else
            If Not HasBody Then
                Result = Result & vbCrLf & vbCrLf
                HasBody = True
            Else
                Result = Result & vbCrLf
            End If

            Result = Result & LineText
        End If
    Next i

    NormalizeSource = Result
End Function

Private Function IsOptionExplicitLine(ByVal LineText As String) As Boolean
    IsOptionExplicitLine = (StrComp(Trim$(LineText), "Option Explicit", vbTextCompare) = 0)
End Function

Private Function IsAttributeLine(ByVal LineText As String) As Boolean
    IsAttributeLine = (StrComp(Left$(Trim$(LineText), 10), "Attribute ", vbTextCompare) = 0)
End Function
