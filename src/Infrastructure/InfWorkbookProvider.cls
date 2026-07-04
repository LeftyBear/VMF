VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfWorkbookProvider"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfWorkbookProvider
' Layer: Infrastructure
' Responsibility: Workbook access operations.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Returns the full path of the supplied workbook-like object.
'
' Parameters:
'   Workbook: Object exposing a FullName property.
'
' Return value:
'   Workbook full path.
'
' Raised errors:
'   ComErrInvalidArgument when Workbook is Nothing or FullName is blank.
Public Function InfGetWorkbookPath(ByVal Workbook As Object) As String
    Dim WorkbookPath As String

    If Workbook Is Nothing Then
        Err.Raise ComErrInvalidArgument, "InfGetWorkbookPath", "Workbook is required."
    End If

    WorkbookPath = CStr(CallByName(Workbook, "FullName", VbGet))
    ComRequireText WorkbookPath, "Workbook.FullName", "InfGetWorkbookPath"

    InfGetWorkbookPath = WorkbookPath
End Function
