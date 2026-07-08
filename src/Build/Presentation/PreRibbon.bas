Option Explicit
Attribute VB_Name = "PreRibbon"

'=========================================================================
' Module: PreRibbon
' Layer: Presentation
' Responsibility: Ribbon callback boundary.
' Dependencies: Common, Presentation
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Handles ribbon loaded callback initialization.
Public Function PreRibbonLoaded() As ComResult
    If PreInitialize() Then
        Set PreRibbonLoaded = ComCreateSuccess("Ribbon loaded.")
    Else
        Set PreRibbonLoaded = ComCreateFailure( _
            ComCreateErrorInfo(ComErrInvalidState, "PreRibbonLoaded", "Presentation initialization failed."))
    End If
End Function
