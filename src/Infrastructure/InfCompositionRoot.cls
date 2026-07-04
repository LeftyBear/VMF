VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfCompositionRoot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfCompositionRoot
' Layer: Infrastructure
' Responsibility: Infrastructure layer object composition boundary.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

' Initializes Infrastructure layer services.
'
' Parameters:
'   None.
'
' Return value:
'   True when Infrastructure initialization completes.
'
' Raised errors:
'   None. Providers are currently stateless and require no startup resources.
Public Function InfInitialize() As Boolean
    InfInitialize = True
End Function

' Shuts down Infrastructure layer services.
'
' Parameters:
'   None.
'
' Raised errors:
'   None. Providers are currently stateless and own no persistent resources.
Public Sub InfShutdown()
End Sub

' Creates a file system provider.
'
' Parameters:
'   None.
'
' Return value:
'   New InfFileSystemProvider instance.
'
' Raised errors:
'   None.
Public Function InfCreateFileSystemProvider() As InfFileSystemProvider
    Set InfCreateFileSystemProvider = New InfFileSystemProvider
End Function

' Creates a workbook provider.
'
' Parameters:
'   None.
'
' Return value:
'   New InfWorkbookProvider instance.
'
' Raised errors:
'   None.
Public Function InfCreateWorkbookProvider() As InfWorkbookProvider
    Set InfCreateWorkbookProvider = New InfWorkbookProvider
End Function
