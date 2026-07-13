VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "InfClassCodeGenerator"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: InfClassCodeGenerator
' Layer: Infrastructure
' Responsibility: Render class member, property, and lifecycle code sections.
' Dependencies: Common, Infrastructure
'==========================================================================

Private Const SectionMemberDeclarations As String = "MemberDeclarations"
Private Const SectionMemberBlock As String = "MemberBlock"
Private Const SectionProperties As String = "Properties"
Private Const SectionInitializeBody As String = "InitializeBody"
Private Const SectionTerminateBody As String = "TerminateBody"

Private Const MemberKeyName As String = "Name"
Private Const MemberKeyTypeName As String = "TypeName"
Private Const MemberKeyIsObject As String = "IsObject"
Private Const MemberKeyAccessor As String = "Accessor"
Private Const MemberKeyInitialValue As String = "InitialValue"
Private Const MemberKeyCreateInstance As String = "CreateInstance"
Private Const MemberKeyReadOnly As String = "ReadOnly"

' Renders template section values from line-based class member definitions.
Public Function InfRenderClassMemberSections(ByVal MemberDefinitionText As String) As Object
    Dim Sections As Object
    Dim Members As Collection

    Set Sections = CreateObject("Scripting.Dictionary")
    Sections(SectionMemberDeclarations) = vbNullString
    Sections(SectionMemberBlock) = vbNullString
    Sections(SectionProperties) = vbNullString
    Sections(SectionInitializeBody) = vbNullString
    Sections(SectionTerminateBody) = vbNullString

    Set Members = ParseMemberDefinitions(MemberDefinitionText)
    If Members.Count = 0 Then
        Set InfRenderClassMemberSections = Sections
        Exit Function
    End If

    Sections(SectionMemberDeclarations) = RenderMemberDeclarations(Members)
    Sections(SectionMemberBlock) = RenderMemberBlock(CStr(Sections(SectionMemberDeclarations)))
    Sections(SectionProperties) = RenderProperties(Members)
    Sections(SectionInitializeBody) = RenderInitializeBody(Members)
    Sections(SectionTerminateBody) = RenderTerminateBody(Members)

    Set InfRenderClassMemberSections = Sections
End Function

Private Function ParseMemberDefinitions(ByVal MemberDefinitionText As String) As Collection
    Dim Members As Collection
    Dim MemberNames As Object
    Dim Lines() As String
    Dim LineIndex As Long
    Dim LineText As String
    Dim Member As Object
    Dim MemberName As String

    Set Members = New Collection
    Set MemberNames = CreateObject("Scripting.Dictionary")

    MemberDefinitionText = NormalizeLineBreaks(MemberDefinitionText)
    Lines = Split(MemberDefinitionText, vbLf)

    For LineIndex = LBound(Lines) To UBound(Lines)
        LineText = Trim$(StripInlineComment(Lines(LineIndex)))
        If Not ComIsBlankText(LineText) Then
            Set Member = ParseMemberDefinitionLine(LineText)
            MemberName = LCase$(CStr(Member(MemberKeyName)))
            If Not MemberNames.Exists(MemberName) Then
                MemberNames.Add MemberName, True
                Members.Add Member
            End If
        End If
    Next LineIndex

    Set ParseMemberDefinitions = Members
End Function

Private Function ParseMemberDefinitionLine(ByVal LineText As String) As Object
    Dim Member As Object
    Dim Parts() As String
    Dim Part As Variant
    Dim KeyName As String
    Dim KeyValue As String
    Dim SeparatorIndex As Long

    Set Member = CreateObject("Scripting.Dictionary")
    Member(MemberKeyIsObject) = "False"
    Member(MemberKeyAccessor) = vbNullString
    Member(MemberKeyInitialValue) = vbNullString
    Member(MemberKeyCreateInstance) = "False"
    Member(MemberKeyReadOnly) = "False"

    Parts = Split(LineText, ";")
    For Each Part In Parts
        SeparatorIndex = InStr(1, CStr(Part), "=", vbBinaryCompare)
        If SeparatorIndex <= 1 Then
            Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.ParseMemberDefinitionLine", "Member definition entry must use Key=Value syntax."
        End If

        KeyName = Trim$(Left$(CStr(Part), SeparatorIndex - 1))
        KeyValue = Trim$(Mid$(CStr(Part), SeparatorIndex + 1))
        AddMemberValue Member, KeyName, KeyValue
    Next Part

    RequireMemberValue Member, MemberKeyName
    RequireMemberValue Member, MemberKeyTypeName
    ValidateIdentifier CStr(Member(MemberKeyName)), "Name"
    If ComIsBlankText(CStr(Member(MemberKeyAccessor))) Then
        If ParseBooleanValue(CStr(Member(MemberKeyIsObject)), MemberKeyIsObject) Then
            Member(MemberKeyAccessor) = "GetSet"
        Else
            Member(MemberKeyAccessor) = "GetLet"
        End If
    End If

    Set ParseMemberDefinitionLine = Member
End Function

Private Sub AddMemberValue(ByVal Member As Object, ByVal KeyName As String, ByVal KeyValue As String)
    Select Case KeyName
        Case MemberKeyName, MemberKeyTypeName, MemberKeyAccessor, MemberKeyInitialValue, _
            MemberKeyIsObject, MemberKeyCreateInstance, MemberKeyReadOnly
            Member(KeyName) = KeyValue
        Case Else
            Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.AddMemberValue", "Unsupported member key: " & KeyName
    End Select
End Sub

Private Sub RequireMemberValue(ByVal Member As Object, ByVal KeyName As String)
    If Not Member.Exists(KeyName) Or ComIsBlankText(CStr(Member(KeyName))) Then
        Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.RequireMemberValue", "Member key is required: " & KeyName
    End If
End Sub

Private Function RenderMemberDeclarations(ByVal Members As Collection) As String
    Dim Member As Object
    Dim Result As String

    For Each Member In Members
        AppendLine Result, "    " & CStr(Member(MemberKeyName)) & " As " & CStr(Member(MemberKeyTypeName))
    Next Member

    RenderMemberDeclarations = Result
End Function

Private Function RenderMemberBlock(ByVal MemberDeclarations As String) As String
    Dim Result As String

    If ComIsBlankText(MemberDeclarations) Then
        RenderMemberBlock = vbNullString
        Exit Function
    End If

    AppendLine Result, "Private Type Member"
    AppendLine Result, MemberDeclarations
    AppendLine Result, "End Type"
    AppendLine Result, vbNullString
    AppendLine Result, "Private This As Member"

    RenderMemberBlock = Result
End Function

Private Function RenderProperties(ByVal Members As Collection) As String
    Dim Member As Object
    Dim Result As String
    Dim PropertyKey As String
    Dim RenderedKeys As Object

    Set RenderedKeys = CreateObject("Scripting.Dictionary")
    For Each Member In Members
        PropertyKey = LCase$(CStr(Member(MemberKeyName)))
        If Not RenderedKeys.Exists(PropertyKey) Then
            AppendBlock Result, RenderProperty(Member)
            RenderedKeys.Add PropertyKey, True
        End If
    Next Member

    RenderProperties = Result
End Function

Private Function RenderProperty(ByVal Member As Object) As String
    Dim PropertyName As String
    Dim TypeName As String
    Dim Accessor As String
    Dim IsObject As Boolean
    Dim IsReadOnly As Boolean
    Dim Result As String

    PropertyName = CStr(Member(MemberKeyName))
    TypeName = CStr(Member(MemberKeyTypeName))
    Accessor = CStr(Member(MemberKeyAccessor))
    IsObject = ParseBooleanValue(CStr(Member(MemberKeyIsObject)), MemberKeyIsObject)
    IsReadOnly = ParseBooleanValue(CStr(Member(MemberKeyReadOnly)), MemberKeyReadOnly)

    AppendBlock Result, RenderPropertyGet(PropertyName, TypeName, IsObject)
    If Not IsReadOnly Then
        Select Case Accessor
            Case "GetLet"
                AppendBlock Result, RenderPropertyLet(PropertyName, TypeName)
            Case "GetSet"
                AppendBlock Result, RenderPropertySet(PropertyName, TypeName)
            Case "GetOnly"
            Case Else
                Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.RenderProperty", "Unsupported Accessor: " & Accessor
        End Select
    End If

    RenderProperty = Result
End Function

Private Function RenderPropertyGet(ByVal PropertyName As String, ByVal TypeName As String, ByVal IsObject As Boolean) As String
    Dim Result As String

    AppendLine Result, "Public Property Get " & PropertyName & "() As " & TypeName
    If IsObject Then
        AppendLine Result, "    Set " & PropertyName & " = This." & PropertyName
    Else
        AppendLine Result, "    " & PropertyName & " = This." & PropertyName
    End If
    AppendLine Result, "End Property"

    RenderPropertyGet = Result
End Function

Private Function RenderPropertyLet(ByVal PropertyName As String, ByVal TypeName As String) As String
    Dim Result As String

    AppendLine Result, "Public Property Let " & PropertyName & "(ByVal Value As " & TypeName & ")"
    AppendLine Result, "    This." & PropertyName & " = Value"
    AppendLine Result, "End Property"

    RenderPropertyLet = Result
End Function

Private Function RenderPropertySet(ByVal PropertyName As String, ByVal TypeName As String) As String
    Dim Result As String

    AppendLine Result, "Public Property Set " & PropertyName & "(ByVal Value As " & TypeName & ")"
    AppendLine Result, "    Set This." & PropertyName & " = Value"
    AppendLine Result, "End Property"

    RenderPropertySet = Result
End Function

Private Function RenderInitializeBody(ByVal Members As Collection) As String
    Dim Member As Object
    Dim Result As String
    Dim MemberName As String
    Dim TypeName As String
    Dim InitialValue As String
    Dim IsObject As Boolean

    For Each Member In Members
        MemberName = CStr(Member(MemberKeyName))
        TypeName = CStr(Member(MemberKeyTypeName))
        InitialValue = CStr(Member(MemberKeyInitialValue))
        IsObject = ParseBooleanValue(CStr(Member(MemberKeyIsObject)), MemberKeyIsObject)

        If IsObject And ParseBooleanValue(CStr(Member(MemberKeyCreateInstance)), MemberKeyCreateInstance) Then
            AppendLine Result, "    Set This." & MemberName & " = New " & TypeName
        ElseIf Not ComIsBlankText(InitialValue) Then
            If IsObject Then
                AppendLine Result, "    Set This." & MemberName & " = " & InitialValue
            Else
                AppendLine Result, "    This." & MemberName & " = " & InitialValue
            End If
        End If
    Next Member

    RenderInitializeBody = Result
End Function

Private Function RenderTerminateBody(ByVal Members As Collection) As String
    Dim Member As Object
    Dim Result As String

    For Each Member In Members
        If ParseBooleanValue(CStr(Member(MemberKeyIsObject)), MemberKeyIsObject) Then
            AppendLine Result, "    Set This." & CStr(Member(MemberKeyName)) & " = Nothing"
        End If
    Next Member

    RenderTerminateBody = Result
End Function

Private Sub AppendLine(ByRef TextValue As String, ByVal LineText As String)
    If Len(TextValue) > 0 Then
        TextValue = TextValue & vbCrLf
    End If

    TextValue = TextValue & LineText
End Sub

Private Sub AppendBlock(ByRef TextValue As String, ByVal BlockText As String)
    If ComIsBlankText(BlockText) Then
        Exit Sub
    End If

    If Len(TextValue) > 0 Then
        TextValue = TextValue & vbCrLf & vbCrLf
    End If

    TextValue = TextValue & BlockText
End Sub

Private Function ParseBooleanValue(ByVal TextValue As String, ByVal ValueName As String) As Boolean
    Select Case LCase$(Trim$(TextValue))
        Case "true"
            ParseBooleanValue = True
        Case "false"
            ParseBooleanValue = False
        Case Else
            Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.ParseBooleanValue", ValueName & " must be True or False."
    End Select
End Function

Private Sub ValidateIdentifier(ByVal Identifier As String, ByVal ValueName As String)
    Dim Position As Long
    Dim Character As String

    ComRequireText Identifier, ValueName, "InfClassCodeGenerator.ValidateIdentifier"

    Character = Mid$(Identifier, 1, 1)
    If Not Character Like "[A-Za-z]" Then
        Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.ValidateIdentifier", ValueName & " must start with a letter."
    End If

    For Position = 1 To Len(Identifier)
        Character = Mid$(Identifier, Position, 1)
        If Not (Character Like "[A-Za-z0-9_]") Then
            Err.Raise ComErrInvalidArgument, "InfClassCodeGenerator.ValidateIdentifier", ValueName & " must contain only letters, numbers, or underscore."
        End If
    Next Position
End Sub

Private Function StripInlineComment(ByVal TextValue As String) As String
    Dim CommentIndex As Long

    CommentIndex = InStr(1, TextValue, "#", vbBinaryCompare)
    If CommentIndex > 0 Then
        StripInlineComment = Left$(TextValue, CommentIndex - 1)
    Else
        StripInlineComment = TextValue
    End If
End Function

Private Function NormalizeLineBreaks(ByVal TextValue As String) As String
    NormalizeLineBreaks = Replace(TextValue, vbCrLf, vbLf)
    NormalizeLineBreaks = Replace(NormalizeLineBreaks, vbCr, vbLf)
End Function
