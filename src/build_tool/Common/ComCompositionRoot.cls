VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "ComCompositionRoot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: ComCompositionRoot
' Layer: Common
' Responsibility: Common layer lifecycle composition boundary.
' Dependencies: None
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Common layer services.
'
' Parameters:
'   None.
'
' Return value:
'   True when Common initialization completes.
'
' Raised errors:
'   None. The current Common phase has no external resources to initialize.
Public Function ComInitialize() As Boolean
    ComInitialize = True
End Function

' Shuts down Common layer services.
'
' Parameters:
'   None.
'
' Raised errors:
'   None. The current Common phase owns no external resources.
Public Sub ComShutdown()
End Sub
