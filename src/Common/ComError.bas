Option Explicit
Attribute VB_Name = "ComError"

'=========================================================================
' Module: ComError
' Layer: Common
' Responsibility: Common layer standardized error identifiers.
' Dependencies: None
'=========================================================================

'=========================================================================
' Public Enums
'=========================================================================

Public Enum ComErrNum
    ComErrInvalidArgument = vbObjectError + 1000
    ComErrInvalidState = vbObjectError + 1001
End Enum
