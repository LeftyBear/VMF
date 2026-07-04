VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "PreCompositionRoot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: PreCompositionRoot
' Layer: Presentation
' Responsibility: Presentation composition boundary.
' Dependencies: Common, Application, Presentation
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Presentation services.
Public Function PreInitialize() As Boolean
    PreInitialize = AppInitialize()
End Function

' Shuts down Presentation services.
Public Sub PreShutdown()
    AppShutdown
End Sub

' Creates a notification presenter.
Public Function PreCreateNotificationPresenter() As PreNotificationPresenter
    Set PreCreateNotificationPresenter = New PreNotificationPresenter
End Function
