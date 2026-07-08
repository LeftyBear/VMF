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

' Creates a VBA project provider.
Public Function InfCreateVbaProjectProvider() As InfVbaProjectProvider
    Set InfCreateVbaProjectProvider = New InfVbaProjectProvider
End Function

' Creates a template provider.
Public Function InfCreateTemplateProvider() As InfTemplateProvider
    Dim TemplateProvider As InfTemplateProvider

    Set TemplateProvider = New InfTemplateProvider
    TemplateProvider.InfInitialize InfCreateFileSystemProvider()

    Set InfCreateTemplateProvider = TemplateProvider
End Function

' Creates a manifest provider.
Public Function InfCreateManifestProvider() As InfManifestProvider
    Dim ManifestProvider As InfManifestProvider

    Set ManifestProvider = New InfManifestProvider
    ManifestProvider.InfInitialize InfCreateFileSystemProvider()

    Set InfCreateManifestProvider = ManifestProvider
End Function

' Creates a token replacer.
Public Function InfCreateTokenReplacer() As InfTokenReplacer
    Set InfCreateTokenReplacer = New InfTokenReplacer
End Function

' Creates a generator.
Public Function InfCreateGenerator() As InfGenerator
    Dim Generator As InfGenerator

    Set Generator = New InfGenerator
    Generator.InfInitialize InfCreateTemplateProvider(), InfCreateManifestProvider(), InfCreateTokenReplacer()

    Set InfCreateGenerator = Generator
End Function
