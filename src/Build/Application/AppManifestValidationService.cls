VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppManifestValidationService"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppManifestValidationService
' Layer: Application
' Responsibility: Validate editable manifest models before save, preview, and generation.
' Dependencies: Common, Infrastructure
'=========================================================================

Private Const SeverityError As Long = 1
Private Const SeverityWarning As Long = 2
Private Const SeverityInformation As Long = 3

Public Function AppSeverityError() As Long
    AppSeverityError = SeverityError
End Function

Public Function AppSeverityWarning() As Long
    AppSeverityWarning = SeverityWarning
End Function

Public Function AppSeverityInformation() As Long
    AppSeverityInformation = SeverityInformation
End Function

Public Function AppValidateManifestEditorModel( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByRef Issues As Collection) As ComResult

    On Error GoTo ErrHandler

    Set Issues = New Collection
    ValidateModules ManifestPath, Modules, Issues, 0
    Set AppValidateManifestEditorModel = ComCreateSuccess(CreateValidationMessage(Issues))
    Exit Function

ErrHandler:
    Set Issues = New Collection
    AddIssue Issues, SeverityError, "VMF-VAL-001", Err.Description, vbNullString, vbNullString, "Internal", 1
    Set AppValidateManifestEditorModel = ComCreateSuccess(CreateValidationMessage(Issues))
End Function

Public Function AppValidateManifestEditorModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByRef Issues As Collection) As ComResult

    Dim Modules As Collection

    On Error GoTo ErrHandler

    Set Modules = New Collection
    If Not ModuleInfo Is Nothing Then
        Modules.Add ModuleInfo
    End If

    Set Issues = New Collection
    ValidateModules ManifestPath, Modules, Issues, 0
    Set AppValidateManifestEditorModule = ComCreateSuccess(CreateValidationMessage(Issues))
    Exit Function

ErrHandler:
    Set Issues = New Collection
    AddIssue Issues, SeverityError, "VMF-VAL-001", Err.Description, vbNullString, vbNullString, "Internal", 1
    Set AppValidateManifestEditorModule = ComCreateSuccess(CreateValidationMessage(Issues))
End Function

Public Function AppIssuesContainErrors(ByVal Issues As Collection) As Boolean
    Dim Issue As AppManifestValidationIssue

    If Issues Is Nothing Then
        AppIssuesContainErrors = False
        Exit Function
    End If

    For Each Issue In Issues
        If Issue.Severity = SeverityError Then
            AppIssuesContainErrors = True
            Exit Function
        End If
    Next Issue

    AppIssuesContainErrors = False
End Function

Public Function AppCreateFailureFromIssues(ByVal Issues As Collection, ByVal OperationName As String) As ComResult
    Dim Message As String

    ComRequireText OperationName, "OperationName", "AppCreateFailureFromIssues"
    Message = CreateValidationMessage(Issues)
    Set AppCreateFailureFromIssues = ComCreateFailure( _
        ComCreateErrorInfo(ComErrInvalidArgument, OperationName, Message))
End Function

Private Sub ValidateModules( _
    ByVal ManifestPath As String, _
    ByVal Modules As Collection, _
    ByVal Issues As Collection, _
    ByVal SortOffset As Long)

    Dim ModuleNames As Object
    Dim ModuleInfo As Object
    Dim ModuleIndex As Long
    Dim ModuleKey As String

    If Modules Is Nothing Then
        AddIssue Issues, SeverityError, "VMF-MOD-000", "At least one module is required.", vbNullString, vbNullString, "Modules", SortOffset + 1
        Exit Sub
    End If

    If Modules.Count = 0 Then
        AddIssue Issues, SeverityError, "VMF-MOD-000", "At least one module is required.", vbNullString, vbNullString, "Modules", SortOffset + 1
        Exit Sub
    End If

    Set ModuleNames = CreateObject("Scripting.Dictionary")
    ModuleIndex = 0
    For Each ModuleInfo In Modules
        ModuleIndex = ModuleIndex + 1
        ValidateModule ManifestPath, ModuleInfo, Issues, SortOffset + ModuleIndex * 1000
        If Not ModuleInfo Is Nothing Then
            ModuleKey = LCase$(Trim$(CStr(GetValue(ModuleInfo, "ModuleName"))))
            If Len(ModuleKey) > 0 Then
                If ModuleNames.Exists(ModuleKey) Then
                    AddIssue Issues, SeverityError, "VMF-MOD-003", "Duplicate ModuleName: " & CStr(GetValue(ModuleInfo, "ModuleName")), CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "ModuleName", SortOffset + ModuleIndex * 1000 + 3
                Else
                    ModuleNames.Add ModuleKey, True
                End If
            End If
        End If
    Next ModuleInfo
End Sub

Private Sub ValidateModule( _
    ByVal ManifestPath As String, _
    ByVal ModuleInfo As Object, _
    ByVal Issues As Collection, _
    ByVal SortBase As Long)

    Dim ModuleName As String
    Dim LayerName As String
    Dim ModuleType As String
    Dim TemplatePath As String
    Dim ResolvedTemplatePath As String

    If ModuleInfo Is Nothing Then
        AddIssue Issues, SeverityError, "VMF-MOD-001", "Module definition is required.", vbNullString, vbNullString, "Module", SortBase + 1
        Exit Sub
    End If

    ModuleName = Trim$(CStr(GetValue(ModuleInfo, "ModuleName")))
    LayerName = Trim$(CStr(GetValue(ModuleInfo, "Layer")))
    ModuleType = Trim$(CStr(GetValue(ModuleInfo, "ModuleType")))
    TemplatePath = Trim$(CStr(GetValue(ModuleInfo, "TemplatePath")))

    If ComIsBlankText(ModuleName) Then
        AddIssue Issues, SeverityError, "VMF-MOD-001", "ModuleName is required.", ModuleName, vbNullString, "ModuleName", SortBase + 1
    ElseIf Not IsValidVbaIdentifier(ModuleName) Then
        AddIssue Issues, SeverityError, "VMF-MOD-002", "ModuleName must be a valid VBA identifier.", ModuleName, vbNullString, "ModuleName", SortBase + 2
    End If

    If ComIsBlankText(LayerName) Then
        AddIssue Issues, SeverityError, "VMF-MOD-004", "Layer is required.", ModuleName, vbNullString, "Layer", SortBase + 4
    ElseIf Not IsValidVbaIdentifier(LayerName) Then
        AddIssue Issues, SeverityError, "VMF-MOD-005", "Layer must be a valid identifier.", ModuleName, vbNullString, "Layer", SortBase + 5
    End If

    If ComIsBlankText(ModuleType) Then
        AddIssue Issues, SeverityError, "VMF-MOD-006", "ModuleType is required.", ModuleName, vbNullString, "ModuleType", SortBase + 6
    ElseIf Not IsSupportedModuleType(ModuleType) Then
        AddIssue Issues, SeverityError, "VMF-MOD-007", "ModuleType must be StandardModule or ClassModule.", ModuleName, vbNullString, "ModuleType", SortBase + 7
    End If

    If ComIsBlankText(TemplatePath) Then
        AddIssue Issues, SeverityError, "VMF-MOD-008", "TemplatePath is required.", ModuleName, vbNullString, "TemplatePath", SortBase + 8
    Else
        ResolvedTemplatePath = ResolveManifestRelativePath(TemplatePath, ManifestPath)
        If Not FileExists(ResolvedTemplatePath) Then
            AddIssue Issues, SeverityError, "VMF-MOD-009", "TemplatePath does not exist: " & TemplatePath, ModuleName, vbNullString, "TemplatePath", SortBase + 9
        Else
            ValidateTemplatePlaceholders ResolvedTemplatePath, ModuleInfo, Issues, SortBase + 10
        End If
    End If

    ValidateMemberCollection ModuleInfo, Issues, SortBase + 500
End Sub

Private Sub ValidateMemberCollection(ByVal ModuleInfo As Object, ByVal Issues As Collection, ByVal SortBase As Long)
    Dim Members As Collection
    Dim MemberNames As Object
    Dim MemberInfo As Object
    Dim MemberIndex As Long
    Dim MemberKey As String
    Dim ModuleName As String

    ModuleName = CStr(GetValue(ModuleInfo, "ModuleName"))
    Set Members = ModuleInfo("Members")
    Set MemberNames = CreateObject("Scripting.Dictionary")

    MemberIndex = 0
    For Each MemberInfo In Members
        MemberIndex = MemberIndex + 1
        ValidateMember ModuleName, MemberInfo, Issues, SortBase + MemberIndex * 100
        MemberKey = LCase$(Trim$(CStr(GetValue(MemberInfo, "Name"))))
        If Len(MemberKey) > 0 Then
            If MemberNames.Exists(MemberKey) Then
                AddIssue Issues, SeverityError, "VMF-MEM-003", "Duplicate member name: " & CStr(GetValue(MemberInfo, "Name")), ModuleName, CStr(GetValue(MemberInfo, "Name")), "Name", SortBase + MemberIndex * 100 + 3
            Else
                MemberNames.Add MemberKey, True
            End If
        End If
    Next MemberInfo
End Sub

Private Sub ValidateMember( _
    ByVal ModuleName As String, _
    ByVal MemberInfo As Object, _
    ByVal Issues As Collection, _
    ByVal SortBase As Long)

    Dim MemberName As String
    Dim TypeName As String
    Dim Accessor As String
    Dim InitialValue As String
    Dim CreateInstanceText As String
    Dim IsObjectText As String
    Dim ReadOnlyText As String
    Dim CreateInstance As Boolean
    Dim IsObject As Boolean
    Dim IsReadOnly As Boolean

    If MemberInfo Is Nothing Then
        AddIssue Issues, SeverityError, "VMF-MEM-000", "Member definition is required.", ModuleName, vbNullString, "Member", SortBase
        Exit Sub
    End If

    MemberName = Trim$(CStr(GetValue(MemberInfo, "Name")))
    TypeName = Trim$(CStr(GetValue(MemberInfo, "TypeName")))
    Accessor = Trim$(CStr(GetValue(MemberInfo, "Accessor")))
    InitialValue = Trim$(CStr(GetValue(MemberInfo, "InitialValue")))
    CreateInstanceText = Trim$(CStr(GetValue(MemberInfo, "CreateInstance")))
    IsObjectText = Trim$(CStr(GetValue(MemberInfo, "IsObject")))
    ReadOnlyText = Trim$(CStr(GetValue(MemberInfo, "ReadOnly")))

    If ComIsBlankText(MemberName) Then
        AddIssue Issues, SeverityError, "VMF-MEM-001", "Member.Name is required.", ModuleName, MemberName, "Name", SortBase + 1
    ElseIf Not IsValidVbaIdentifier(MemberName) Then
        AddIssue Issues, SeverityError, "VMF-MEM-002", "Member.Name must be a valid VBA identifier.", ModuleName, MemberName, "Name", SortBase + 2
    End If

    If ComIsBlankText(TypeName) Then
        AddIssue Issues, SeverityError, "VMF-MEM-004", "Member.TypeName is required.", ModuleName, MemberName, "TypeName", SortBase + 4
    End If

    If Not IsSupportedAccessor(Accessor) Then
        AddIssue Issues, SeverityError, "VMF-MEM-005", "Accessor must be GetLet, GetSet, or GetOnly.", ModuleName, MemberName, "Accessor", SortBase + 5
    End If

    If Not TryParseBoolean(CreateInstanceText, CreateInstance) Then
        AddIssue Issues, SeverityError, "VMF-MEM-006", "CreateInstance must be True or False.", ModuleName, MemberName, "CreateInstance", SortBase + 6
    End If

    If Not TryParseBoolean(IsObjectText, IsObject) Then
        AddIssue Issues, SeverityError, "VMF-MEM-007", "IsObject must be True or False.", ModuleName, MemberName, "IsObject", SortBase + 7
    End If

    If Not TryParseBoolean(ReadOnlyText, IsReadOnly) Then
        AddIssue Issues, SeverityError, "VMF-MEM-008", "ReadOnly must be True or False.", ModuleName, MemberName, "ReadOnly", SortBase + 8
    End If

    If IsSupportedAccessor(Accessor) Then
        If IsObject And Accessor = "GetLet" Then
            AddIssue Issues, SeverityError, "VMF-MEM-009", "Object members must not use Property Let.", ModuleName, MemberName, "Accessor", SortBase + 9
        End If

        If Not IsObject And Accessor = "GetSet" Then
            AddIssue Issues, SeverityError, "VMF-MEM-010", "Value members must not use Property Set.", ModuleName, MemberName, "Accessor", SortBase + 10
        End If

        If IsReadOnly And Accessor <> "GetOnly" Then
            AddIssue Issues, SeverityError, "VMF-MEM-011", "ReadOnly members must use GetOnly.", ModuleName, MemberName, "ReadOnly", SortBase + 11
        End If
    End If

    If CreateInstance And Not IsObject Then
        AddIssue Issues, SeverityError, "VMF-MEM-012", "CreateInstance=True requires IsObject=True.", ModuleName, MemberName, "CreateInstance", SortBase + 12
    End If

    If CreateInstance And Not ComIsBlankText(InitialValue) Then
        AddIssue Issues, SeverityWarning, "VMF-MEM-013", "InitialValue is ignored when CreateInstance=True.", ModuleName, MemberName, "InitialValue", SortBase + 13
    End If
End Sub

Private Sub ValidateTemplatePlaceholders( _
    ByVal TemplatePath As String, _
    ByVal ModuleInfo As Object, _
    ByVal Issues As Collection, _
    ByVal SortBase As Long)

    Dim TextValue As String
    Dim Token As String
    Dim Position As Long
    Dim EndPosition As Long
    Dim SortIndex As Long

    On Error GoTo TemplateReadFailed

    TextValue = ReadText(TemplatePath)
    On Error GoTo 0

    ValidateTemplateSectionContracts TextValue, ModuleInfo, Issues, SortBase + 100

    Position = InStr(1, TextValue, "{", vbBinaryCompare)
    Do While Position > 0
        If Mid$(TextValue, Position, 2) = "{{" Then
            EndPosition = InStr(Position + 2, TextValue, "}}", vbBinaryCompare)
            If EndPosition = 0 Then
                AddIssue Issues, SeverityError, "VMF-MOD-010", "Template contains an unresolved placeholder.", CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex
                Exit Sub
            End If
            Token = Mid$(TextValue, Position + 2, EndPosition - Position - 2)
            Position = EndPosition + 2
        Else
            EndPosition = InStr(Position + 1, TextValue, "}", vbBinaryCompare)
            If EndPosition = 0 Then
                AddIssue Issues, SeverityError, "VMF-MOD-010", "Template contains an unresolved placeholder.", CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex
                Exit Sub
            End If
            Token = Mid$(TextValue, Position + 1, EndPosition - Position - 1)
            Position = EndPosition + 1
        End If

        Token = Trim$(Token)
        If Not IsKnownTemplateToken(Token) Then
            SortIndex = SortIndex + 1
            AddIssue Issues, SeverityError, "VMF-MOD-010", "Template placeholder is not supported: " & Token, CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex
        End If
        Position = InStr(Position, TextValue, "{", vbBinaryCompare)
    Loop
    Exit Sub

TemplateReadFailed:
    AddIssue Issues, SeverityError, "VMF-MOD-011", "Template could not be read: " & Err.Description, CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase
End Sub

Private Sub ValidateTemplateSectionContracts( _
    ByVal TemplateText As String, _
    ByVal ModuleInfo As Object, _
    ByVal Issues As Collection, _
    ByVal SortBase As Long)

    Dim TemplateSections As Object
    Dim SectionSources As Object
    Dim SectionName As Variant
    Dim ModuleName As String
    Dim Result As ComResult

    ModuleName = CStr(GetValue(ModuleInfo, "ModuleName"))
    Set Result = ExtractTemplateSections(TemplateText, TemplateSections)
    If Result.IsFailure Then
        AddIssue Issues, SeverityError, "VMF-MOD-012", Result.Failure.Description, ModuleName, vbNullString, "TemplatePath", SortBase
        Exit Sub
    End If

    If Not ModuleInfo Is Nothing Then
        If ModuleInfo.Exists("SectionSources") Then
            Set SectionSources = ModuleInfo("SectionSources")
            For Each SectionName In SectionSources.Keys
                If Not TemplateSections.Exists(CStr(SectionName)) Then
                    AddIssue Issues, SeverityError, "VMF-MOD-012", "Template section does not exist: " & CStr(SectionName), ModuleName, vbNullString, "SectionSources", SortBase + TemplateSections.Count + 1
                End If
            Next SectionName
        End If
    End If
End Sub

Private Function ExtractTemplateSections(ByVal TemplateText As String, ByRef Sections As Object) As ComResult
    Dim NormalizedText As String
    Dim SectionStart As Long
    Dim SectionLineStart As Long
    Dim SectionLineEnd As Long
    Dim SectionEnd As Long
    Dim SectionLine As String
    Dim SectionName As String

    On Error GoTo ErrHandler

    Set Sections = CreateObject("Scripting.Dictionary")
    NormalizedText = Replace(TemplateText, vbCrLf, vbLf)
    NormalizedText = Replace(NormalizedText, vbCr, vbLf)

    SectionStart = InStr(1, NormalizedText, "@section", vbTextCompare)
    Do While SectionStart > 0
        SectionLineStart = InStrRev(NormalizedText, vbLf, SectionStart)
        If SectionLineStart = 0 Then
            SectionLineStart = 1
        Else
            SectionLineStart = SectionLineStart + 1
        End If

        SectionLineEnd = InStr(SectionStart, NormalizedText, vbLf, vbBinaryCompare)
        If SectionLineEnd = 0 Then
            SectionLineEnd = Len(NormalizedText) + 1
        End If

        SectionEnd = InStr(SectionLineEnd, NormalizedText, "@endsection", vbTextCompare)
        If SectionEnd = 0 Then
            Err.Raise ComErrInvalidArgument, "ExtractTemplateSections", "Template section is missing @endsection."
        End If

        SectionLine = Mid$(NormalizedText, SectionLineStart, SectionLineEnd - SectionLineStart)
        SectionName = ParseTemplateSectionName(SectionLine)
        If Not Sections.Exists(SectionName) Then
            Sections.Add SectionName, True
        End If

        SectionStart = InStr(SectionEnd + Len("@endsection"), NormalizedText, "@section", vbTextCompare)
    Loop

    Set ExtractTemplateSections = ComCreateSuccess("Template sections extracted.")
    Exit Function

ErrHandler:
    Set ExtractTemplateSections = ComCreateFailureFromErr("AppManifestValidationService.ExtractTemplateSections", Err.Description, "Error", False)
End Function

Private Function ParseTemplateSectionName(ByVal SectionLine As String) As String
    Dim MarkerIndex As Long
    Dim SectionName As String

    MarkerIndex = InStr(1, SectionLine, "@section", vbTextCompare)
    If MarkerIndex = 0 Then
        Err.Raise ComErrInvalidArgument, "ParseTemplateSectionName", "Template section marker is invalid."
    End If

    SectionName = Mid$(SectionLine, MarkerIndex + Len("@section"))
    SectionName = Replace(SectionName, vbCr, vbNullString)
    SectionName = Replace(SectionName, vbLf, vbNullString)
    SectionName = Trim$(SectionName)
    ComRequireText SectionName, "SectionName", "ParseTemplateSectionName"

    ParseTemplateSectionName = SectionName
End Function

Private Function IsKnownTemplateToken(ByVal Token As String) As Boolean
    Select Case Token
        Case "ModuleName", "ModuleType", "Layer", "TemplatePath", "BODY", _
            "ModuleDeclaration", "DeclarationSectionEnd", "MemberDeclarations", _
            "MemberBlock", "InitializeBody", "TerminateBody", "PropertyGroupStart", _
            "Properties", "PropertyGroupEnd", "ProcedureGroupStart", "ProcedureGroupEnd"
            IsKnownTemplateToken = True
        Case Else
            If Left$(Token, Len("Section:")) = "Section:" Then
                IsKnownTemplateToken = True
            Else
                IsKnownTemplateToken = False
            End If
    End Select
End Function

Private Sub AddIssue( _
    ByVal Issues As Collection, _
    ByVal Severity As Long, _
    ByVal Code As String, _
    ByVal Message As String, _
    ByVal ModuleName As String, _
    ByVal MemberName As String, _
    ByVal PropertyName As String, _
    ByVal SortOrder As Long)

    Dim Issue As AppManifestValidationIssue

    Set Issue = New AppManifestValidationIssue
    Issue.AppInitialize Severity, Code, Message, ModuleName, MemberName, PropertyName, SortOrder
    Issues.Add Issue
End Sub

Private Function CreateValidationMessage(ByVal Issues As Collection) As String
    If Issues Is Nothing Then
        CreateValidationMessage = "Manifest validation passed."
    ElseIf Issues.Count = 0 Then
        CreateValidationMessage = "Manifest validation passed."
    ElseIf AppIssuesContainErrors(Issues) Then
        CreateValidationMessage = "Manifest validation failed."
    Else
        CreateValidationMessage = "Manifest validation completed with warnings."
    End If
End Function

Private Function GetValue(ByVal Values As Object, ByVal KeyName As String) As Variant
    If Values Is Nothing Then
        GetValue = vbNullString
    ElseIf Values.Exists(KeyName) Then
        GetValue = Values(KeyName)
    Else
        GetValue = vbNullString
    End If
End Function

Private Function IsValidVbaIdentifier(ByVal Identifier As String) As Boolean
    Dim Position As Long
    Dim Character As String

    If ComIsBlankText(Identifier) Then
        IsValidVbaIdentifier = False
        Exit Function
    End If

    Character = Mid$(Identifier, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        IsValidVbaIdentifier = False
        Exit Function
    End If

    For Position = 1 To Len(Identifier)
        Character = Mid$(Identifier, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            IsValidVbaIdentifier = False
            Exit Function
        End If
    Next Position

    IsValidVbaIdentifier = True
End Function

Private Function IsSupportedModuleType(ByVal ModuleType As String) As Boolean
    Select Case ModuleType
        Case "StandardModule", "ClassModule"
            IsSupportedModuleType = True
        Case Else
            IsSupportedModuleType = False
    End Select
End Function

Private Function IsSupportedAccessor(ByVal Accessor As String) As Boolean
    Select Case Accessor
        Case "GetLet", "GetSet", "GetOnly"
            IsSupportedAccessor = True
        Case Else
            IsSupportedAccessor = False
    End Select
End Function

Private Function TryParseBoolean(ByVal TextValue As String, ByRef ParsedValue As Boolean) As Boolean
    Select Case LCase$(Trim$(TextValue))
        Case "true"
            ParsedValue = True
            TryParseBoolean = True
        Case "false"
            ParsedValue = False
            TryParseBoolean = True
        Case Else
            ParsedValue = False
            TryParseBoolean = False
    End Select
End Function

Private Function FileExists(ByVal FilePath As String) As Boolean
    FileExists = CreateObject("Scripting.FileSystemObject").FileExists(FilePath)
End Function

Private Function ReadText(ByVal FilePath As String) As String
    Dim TextFile As Object

    Set TextFile = CreateObject("Scripting.FileSystemObject").OpenTextFile(FilePath, 1, False)
    ReadText = TextFile.ReadAll
    TextFile.Close
End Function

Private Function ResolveManifestRelativePath(ByVal FilePath As String, ByVal ManifestPath As String) As String
    Dim ManifestDirectoryPath As String
    Dim NormalizedFilePath As String

    If IsAbsolutePath(FilePath) Then
        ResolveManifestRelativePath = FilePath
        Exit Function
    End If

    ManifestDirectoryPath = GetParentFolderPath(ManifestPath)
    NormalizedFilePath = Replace(FilePath, "/", "\")
    ResolveManifestRelativePath = CreateObject("Scripting.FileSystemObject").GetAbsolutePathName( _
        BuildPathResolver.CombinePath(ManifestDirectoryPath, NormalizedFilePath))
End Function

Private Function IsAbsolutePath(ByVal FilePath As String) As Boolean
    IsAbsolutePath = ((Len(FilePath) >= 3 _
            And (Mid$(FilePath, 2, 2) = ":\" Or Mid$(FilePath, 2, 2) = ":/")) _
        Or Left$(FilePath, 2) = "\\" _
        Or Left$(FilePath, 2) = "//")
End Function

Private Function GetParentFolderPath(ByVal FilePath As String) As String
    Dim SeparatorIndex As Long

    SeparatorIndex = InStrRev(FilePath, "\")
    If SeparatorIndex = 0 Then
        SeparatorIndex = InStrRev(FilePath, "/")
    End If

    If SeparatorIndex = 0 Then
        GetParentFolderPath = "."
    Else
        GetParentFolderPath = Left$(FilePath, SeparatorIndex - 1)
    End If
End Function
