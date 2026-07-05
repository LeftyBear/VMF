Option Explicit
Attribute VB_Name = "PreFacade"

'=========================================================================
' Module: PreFacade
' Layer: Presentation
' Responsibility: Published entry point for Presentation capabilities.
' Dependencies: Common, Application, Presentation
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Presentation services.
Public Function PreInitialize() As Boolean
    Dim CompositionRoot As PreCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    PreInitialize = CompositionRoot.PreInitialize()
End Function

' Shuts down Presentation services.
Public Sub PreShutdown()
    Dim CompositionRoot As PreCompositionRoot

    Set CompositionRoot = CreateCompositionRoot()
    CompositionRoot.PreShutdown
End Sub

' Returns a user-facing validation result message.
Public Function PreShowValidationResult(ByVal Result As ComResult) As String
    Dim Presenter As PreNotificationPresenter

    Set Presenter = CreateCompositionRoot().PreCreateNotificationPresenter()
    PreShowValidationResult = Presenter.PreShowValidationResult(Result)
End Function

' Returns a user-facing build result message.
Public Function PreShowBuildResult(ByVal Result As ComResult) As String
    Dim Presenter As PreNotificationPresenter

    Set Presenter = CreateCompositionRoot().PreCreateNotificationPresenter()
    PreShowBuildResult = Presenter.PreShowBuildResult(Result)
End Function

' Generates a module by presenting an input dialog and delegating to AppFacade.
Public Function PreGenerateModule() As ComResult
    Dim ModuleName As String
    Dim Result As ComResult

    ModuleName = PreRequestModuleName()
    If ComIsBlankText(ModuleName) Then
        Err.Raise ComErrInvalidArgument, "PreGenerateModule", "ModuleName is required."
    End If

    Set Result = AppGenerateModule(ModuleName)
    Set PreGenerateModule = Result
End Function

' Returns the generated module result as a user-facing message.
Public Function PreGenerateModuleMessage() As String
    Dim Result As ComResult

    Set Result = PreGenerateModule()
    PreGenerateModuleMessage = PreShowBuildResult(Result)
End Function

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Function CreateCompositionRoot() As PreCompositionRoot
    Set CreateCompositionRoot = New PreCompositionRoot
End Function
