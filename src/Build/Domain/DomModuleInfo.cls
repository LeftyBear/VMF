VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "DomModuleInfo"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: DomModuleInfo
' Layer: Domain
' Responsibility: Module metadata.
' Dependencies: Common
'=========================================================================

'=========================================================================
' Private Types
'=========================================================================

Private Type TMember
    ModuleName As String
    LayerName As String
    FilePath As String
    IsInitialized As Boolean
End Type

'=========================================================================
' Module Variables
'=========================================================================

Private This As TMember

'=========================================================================
' Public API
'=========================================================================

' Initializes module metadata.
Public Sub DomInitializeModuleInfo( _
    ByVal ModuleName As String, _
    ByVal LayerName As String, _
    ByVal FilePath As String)

    ComRequireText ModuleName, "ModuleName", "DomInitializeModuleInfo"
    ComRequireText LayerName, "LayerName", "DomInitializeModuleInfo"
    ComRequireText FilePath, "FilePath", "DomInitializeModuleInfo"

    This.ModuleName = ComTrimText(ModuleName)
    This.LayerName = ComTrimText(LayerName)
    This.FilePath = ComTrimText(FilePath)
    This.IsInitialized = True
End Sub

' Returns the module name.
Public Property Get Name() As String
    EnsureInitialized
    Name = This.ModuleName
End Property

' Returns the canonical layer name.
Public Property Get Layer() As String
    EnsureInitialized
    Layer = This.LayerName
End Property

' Returns the source file path.
Public Property Get Path() As String
    EnsureInitialized
    Path = This.FilePath
End Property

' Returns True when the instance is initialized.
Public Property Get IsInitialized() As Boolean
    IsInitialized = This.IsInitialized
End Property

'=========================================================================
' Private Helper Functions
'=========================================================================

Private Sub EnsureInitialized()
    If Not This.IsInitialized Then
        Err.Raise ComErrInvalidState, "DomModuleInfo", "DomModuleInfo is not initialized."
    End If
End Sub
