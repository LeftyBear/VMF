VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGeneratorService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'==========================================================================
' Class: AppGeneratorService
' Layer: Application
' Responsibility: Module generation service (Phase 1: Generate Module)
' Dependencies: Common, Infrastructure
'==========================================================================

Private Generator As InfGenerator
Private VbaProjectProvider As InfVbaProjectProvider

' Initializes the service with required generation infrastructure.
Public Sub AppInitialize(ByVal ModuleGenerator As InfGenerator, ByVal ProjectProvider As InfVbaProjectProvider)
    If ModuleGenerator Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ModuleGenerator is required."
    End If

    If ProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppInitialize", "ProjectProvider is required."
    End If

    Set Generator = ModuleGenerator
    Set VbaProjectProvider = ProjectProvider
End Sub

' Generates a standard module with the required template.
' Parameters:
'   ModuleName: Desired module name.
' Return value:
'   ComResult describing success or failure.
Public Function AppGenerateModule(ByVal ModuleName As String) As ComResult
    Dim CodeText As String
    Dim LayerName As String

    On Error GoTo ErrHandler

    ComRequireText ModuleName, "ModuleName", "AppGenerateModule"

    If Not ModuleNameLikeValid(ModuleName) Then
        Err.Raise vbObjectError + 9100, "AppGenerateModule", "ModuleName is invalid. Use letters, numbers and underscore; start with a letter."
    End If

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "GeneratorService is not initialized."
    End If

    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateModule", "VbaProjectProvider is not initialized."
    End If

    LayerName = ResolveLayerName(ModuleName)
    CodeText = Generator.InfGenerateStandardModule(ModuleName, LayerName)
    Set AppGenerateModule = VbaProjectProvider.InfAddStandardModule(ModuleName, CodeText)
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set AppGenerateModule = ComCreateFailureFromErr("AppGeneratorService.AppGenerateModule", Desc, "Error", False)
End Function

' Generates a class module with the required template.
' Parameters:
'   ClassName: Desired class name.
' Return value:
'   ComResult describing success or failure.
Public Function AppGenerateClass(ByVal ClassName As String) As ComResult
    Dim CodeText As String
    Dim LayerName As String

    On Error GoTo ErrHandler

    ComRequireText ClassName, "ClassName", "AppGenerateClass"

    If Not ModuleNameLikeValid(ClassName) Then
        Err.Raise vbObjectError + 9100, "AppGenerateClass", "ClassName is invalid. Use letters, numbers and underscore; start with a letter."
    End If

    If Generator Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateClass", "GeneratorService is not initialized."
    End If

    If VbaProjectProvider Is Nothing Then
        Err.Raise ComErrInvalidState, "AppGenerateClass", "VbaProjectProvider is not initialized."
    End If

    LayerName = ResolveLayerName(ClassName)
    CodeText = Generator.InfGenerateStandardClass(ClassName, LayerName)
    Set AppGenerateClass = VbaProjectProvider.InfAddClassModule(ClassName, CodeText)
    Exit Function

ErrHandler:
    Dim Desc As String

    Desc = Trim$(Err.Description)
    If Len(Desc) = 0 Then
        Desc = vbNullString
    End If

    Set AppGenerateClass = ComCreateFailureFromErr("AppGeneratorService.AppGenerateClass", Desc, "Error", False)
End Function

' Validates basic module name rules.
Private Function ModuleNameLikeValid(ByVal Name As String) As Boolean
    Dim i As Long
    Dim ch As String

    If ComIsBlankText(Name) Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    Name = Trim$(Name)

    ' First char must be a letter
    ch = Mid$(Name, 1, 1)
    If Not ch Like "[A-Za-z]" Then
        ModuleNameLikeValid = False
        Exit Function
    End If

    ' Remaining chars must be letters, digits, or underscore
    For i = 1 To Len(Name)
        ch = Mid$(Name, i, 1)
        If Not (ch Like "[A-Za-z0-9_]") Then
            ModuleNameLikeValid = False
            Exit Function
        End If
    Next i

    ModuleNameLikeValid = True
End Function

Private Function ResolveLayerName(ByVal ModuleName As String) As String
    Select Case Left$(ModuleName, 3)
        Case "Pre"
            ResolveLayerName = "Presentation"
        Case "App"
            ResolveLayerName = "Application"
        Case "Dom"
            ResolveLayerName = "Domain"
        Case "Inf"
            ResolveLayerName = "Infrastructure"
        Case "Com"
            ResolveLayerName = "Common"
        Case Else
            ResolveLayerName = "Application"
    End Select
End Function
