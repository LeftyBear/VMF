VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AppSelfCheckResult"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

'=========================================================================
' Class: AppSelfCheckResult
' Layer: Application
' Responsibility: Structured VMF Studio self check result.
' Dependencies: Common
'=========================================================================

Private Type TMember
    TestID As String
    Category As String
    TestName As String
    Status As String
    Message As String
    Expected As String
    Actual As String
    StartedAt As Date
    FinishedAt As Date
    ElapsedTime As Double
    ErrorCode As String
End Type

Private This As TMember

Public Sub AppInitialize(ByVal TestID As String, ByVal Category As String, ByVal TestName As String, ByVal StartedAt As Date)
    This.TestID = TestID
    This.Category = Category
    This.TestName = TestName
    This.StartedAt = StartedAt
    This.Status = "Skipped"
End Sub

Public Sub AppFinish(ByVal Status As String, ByVal Message As String, ByVal Expected As String, ByVal Actual As String, ByVal ErrorCode As String)
    This.FinishedAt = Now
    This.ElapsedTime = DateDiff("s", This.StartedAt, This.FinishedAt)
    This.Status = Status
    This.Message = Message
    This.Expected = Expected
    This.Actual = Actual
    This.ErrorCode = ErrorCode
End Sub

Public Property Get TestID() As String: TestID = This.TestID: End Property
Public Property Get Category() As String: Category = This.Category: End Property
Public Property Get TestName() As String: TestName = This.TestName: End Property
Public Property Get Status() As String: Status = This.Status: End Property
Public Property Get Message() As String: Message = This.Message: End Property
Public Property Get Expected() As String: Expected = This.Expected: End Property
Public Property Get Actual() As String: Actual = This.Actual: End Property
Public Property Get StartedAt() As Date: StartedAt = This.StartedAt: End Property
Public Property Get FinishedAt() As Date: FinishedAt = This.FinishedAt: End Property
Public Property Get ElapsedTime() As Double: ElapsedTime = This.ElapsedTime: End Property
Public Property Get ErrorCode() As String: ErrorCode = This.ErrorCode: End Property
