VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "PreNotificationPresenter"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: PreNotificationPresenter
' Layer: Presentation
' Responsibility: User notification message presentation.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Constants
'=========================================================================

Private Const PreValidationSuccessMessage As String = "Validation completed successfully."
Private Const PreBuildSuccessMessage As String = "Build completed successfully."

'=========================================================================
' Public API
'=========================================================================

' Returns a user-facing validation result message.
Public Function PreShowValidationResult(ByVal Result As ComResult) As String
    PreShowValidationResult = FormatResult(Result, PreValidationSuccessMessage)
End Function

' Returns a user-facing build result message.
Public Function PreShowBuildResult(ByVal Result As ComResult) As String
    PreShowBuildResult = FormatResult(Result, PreBuildSuccessMessage)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function FormatResult(ByVal Result As ComResult, ByVal SuccessMessage As String) As String
    If Result Is Nothing Then
        Err.Raise ComErrInvalidArgument, "PreNotificationPresenter", "Result is required."
    End If

    If Result.IsSuccess Then
        FormatResult = SuccessMessage
    Else
        FormatResult = Result.Message
    End If
End Function
