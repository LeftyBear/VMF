VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppGenerateContextBuilder"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppGenerateContextBuilder
' Layer: Application
' Responsibility: Build Generator-ready context from approved Template Derivation output.
' Dependencies: Common
'=========================================================================

Private Const ResultSuccess As String = "Success"
Private Const ResultHardStop As String = "HardStop"

' Builds a GenerateContext from complete, approved, generatable Template Derivation items.
Public Function AppBuildGenerateContext(ByVal TemplateDerivationItems As Collection) As Object
    Dim Result As Object

    On Error GoTo ErrHandler

    Set Result = CreateResult(False, ResultHardStop, "GenerateContext input is required.")
    If TemplateDerivationItems Is Nothing Then
        Set AppBuildGenerateContext = Result
        Exit Function
    End If

    If TemplateDerivationItems.Count = 0 Then
        Result("Message") = "GenerateContext input is absent."
        Set AppBuildGenerateContext = Result
        Exit Function
    End If

    Set AppBuildGenerateContext = BuildSuccessfulContext(TemplateDerivationItems)
    Exit Function

ErrHandler:
    Set AppBuildGenerateContext = CreateResult(False, ResultHardStop, "GenerateContext hard-stop: " & Err.Description)
End Function

Private Function BuildSuccessfulContext(ByVal TemplateDerivationItems As Collection) As Object
    Dim Context As Object
    Dim Units As Collection
    Dim Item As Variant
    Dim Index As Long

    Set Units = New Collection
    Index = 0

    For Each Item In TemplateDerivationItems
        Index = Index + 1
        Units.Add CreateGenerationUnit(Item, Index)
    Next Item

    Set Context = CreateResult(True, ResultSuccess, "GenerateContext constructed.")
    Set Context("GenerationUnits") = Units
    Set BuildSuccessfulContext = Context
End Function

Private Function CreateGenerationUnit(ByVal Item As Variant, ByVal UnitOrder As Long) As Object
    Dim Source As Object
    Dim Unit As Object

    If Not IsObject(Item) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.CreateGenerationUnit", "Template Derivation item is invalid."
    End If

    Set Source = Item
    RequireCompleteApprovedGeneratableItem Source

    Set Unit = CreateObject("Scripting.Dictionary")
    Unit("order") = UnitOrder
    Unit("moduleName") = RequiredText(Source, "moduleName")
    Unit("moduleType") = RequiredText(Source, "moduleType")
    Unit("layerName") = RequiredText(Source, "layerName")
    Unit("templateKey") = RequiredText(Source, "templateKey")
    Unit("templatePath") = RequiredText(Source, "templatePath")
    Unit("templateRole") = RequiredText(Source, "templateRole")
    Unit("selectionRuleId") = RequiredText(Source, "selectionRuleId")
    Unit("derivationReason") = RequiredText(Source, "derivationReason")

    Set CreateGenerationUnit = Unit
End Function

Private Sub RequireCompleteApprovedGeneratableItem(ByVal Item As Object)
    If Item Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequireCompleteApprovedGeneratableItem", "Template Derivation item is required."
    End If

    If Not HasBooleanValue(Item, "isApproved", True) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequireCompleteApprovedGeneratableItem", "Template Derivation item is unapproved."
    End If

    If Not HasBooleanValue(Item, "isGeneratable", True) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequireCompleteApprovedGeneratableItem", "Template Derivation item is non-generatable."
    End If

    If Not HasBooleanValue(Item, "isFallbackDerived", False) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequireCompleteApprovedGeneratableItem", "Fallback Template selection is prohibited."
    End If

    If Not HasBooleanValue(Item, "isImplicitlySelected", False) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequireCompleteApprovedGeneratableItem", "Implicit Template selection is prohibited."
    End If

    RequiredText Item, "moduleName"
    RequiredText Item, "moduleType"
    RequiredText Item, "layerName"
    RequiredText Item, "templateKey"
    RequiredText Item, "templatePath"
    RequiredText Item, "templateRole"
    RequiredText Item, "selectionRuleId"
    RequiredText Item, "derivationReason"
End Sub

Private Function RequiredText(ByVal Source As Object, ByVal FieldName As String) As String
    If Source Is Nothing Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequiredText", "Source item is required."
    End If

    If Not Source.Exists(FieldName) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequiredText", "Required GenerateContext field is missing: " & FieldName
    End If

    If ComIsBlankText(CStr(Source.Item(FieldName))) Then
        Err.Raise ComErrInvalidArgument, "AppGenerateContextBuilder.RequiredText", "Required GenerateContext field is blank: " & FieldName
    End If

    RequiredText = Trim$(CStr(Source.Item(FieldName)))
End Function

Private Function HasBooleanValue(ByVal Source As Object, ByVal FieldName As String, ByVal ExpectedValue As Boolean) As Boolean
    If Source Is Nothing Then
        HasBooleanValue = False
        Exit Function
    End If

    If Not Source.Exists(FieldName) Then
        HasBooleanValue = False
        Exit Function
    End If

    If VarType(Source.Item(FieldName)) <> vbBoolean Then
        HasBooleanValue = False
        Exit Function
    End If

    HasBooleanValue = (CBool(Source.Item(FieldName)) = ExpectedValue)
End Function

Private Function CreateResult(ByVal IsSuccess As Boolean, ByVal Classification As String, ByVal Message As String) As Object
    Dim Result As Object

    Set Result = CreateObject("Scripting.Dictionary")
    Result("Success") = IsSuccess
    Result("Classification") = Classification
    Result("Message") = Message
    Set Result("GenerationUnits") = New Collection

    Set CreateResult = Result
End Function
