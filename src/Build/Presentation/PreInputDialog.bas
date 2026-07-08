Option Explicit
Attribute VB_Name = "PreInputDialog"

'=========================================================================
' Module: PreInputDialog
' Layer: Presentation
' Responsibility: Basic input dialog helper.
' Dependencies: Common
'=========================================================================

' Prompts the user for a module name.
Public Function PreRequestModuleName() As String
    Dim ModuleName As String

    ModuleName = InputBox("Enter the module name:", "Generate Module")
    PreRequestModuleName = Trim$(ModuleName)
End Function
