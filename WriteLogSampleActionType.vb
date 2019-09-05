Imports HomeSeer.Jui.Views
Imports HomeSeer.PluginSdk.Events
Imports HomeSeer.PluginSdk.Logging

Public Class WriteLogSampleActionType
    Inherits AbstractActionType

    Private Const ActionName As String = "Sample Plugin Sample Action - Write to Log"

    Private ReadOnly Property InstructionsLabelId As String
        Get
            Return $"{PageId}-instructlabel"
        End Get
    End Property

    Private Const InstructionsLabelValue As String = "Write a message to the log with a type of..."

    Private ReadOnly Property LogTypeSelectListId As String
        Get
            Return $"{PageId}-logtypesl"
        End Get
    End Property

    Private ReadOnly Property LogMessageInputId As String
        Get
            Return $"{PageId}-messageinput"
        End Get
    End Property

    Private LogTypeOptions As List(Of String) = New List(Of String) From {
        "Trace",
        "Debug",
        "Info",
        "Warning",
        "Error"
    }

    Private ReadOnly Property _listener As IWriteLogActionListener
        Get
            Return TryCast(ActionListener, IWriteLogActionListener)
        End Get
    End Property

    Public Sub New(ByVal id As Integer, ByVal eventRef As Integer, ByVal dataIn As Byte())
        MyBase.New(id, eventRef, dataIn)
    End Sub

    Public Sub New()
    End Sub

    Public Overrides Function IsFullyConfigured() As Boolean
        Select Case ConfigPage.ViewCount
            Case 3
                Dim inputView = TryCast(ConfigPage.GetViewById(LogMessageInputId), InputView)
                Return (If(inputView?.Value?.Length, 0)) > 0
            Case Else
                Return False
        End Select
    End Function

    Public Overrides Function GetPrettyString() As String
        Dim selectList = TryCast(ConfigPage.GetViewById(LogTypeSelectListId), SelectListView)
        Dim message = If(ConfigPage?.GetViewById(LogMessageInputId)?.GetStringValue(), "Error retrieving log message")
        Return $"write the message '{message}' to the log with the type of {If(selectList?.GetSelectedOption(), "Unknown Selection")}"
    End Function

    Public Overrides Function OnRunAction() As Boolean
        Dim iLogType = If((TryCast(ConfigPage?.GetViewById(LogTypeSelectListId), SelectListView))?.Selection, 0)
        Dim logType = ELogType.Info

        Select Case iLogType
            Case 0
                logType = ELogType.Trace
            Case 1
                logType = ELogType.Debug
            Case 2
                logType = ELogType.Info
            Case 3
                logType = ELogType.Warning
            Case 4
                logType = ELogType.[Error]
            Case Else
                logType = ELogType.Info
        End Select

        Dim message = If(ConfigPage?.GetViewById(LogMessageInputId)?.GetStringValue(), "Error retrieving log message")
        _listener?.WriteLog(logType, message)
        Return True
    End Function

    Public Overrides Function ReferencesDeviceOrFeature(ByVal devOrFeatRef As Integer) As Boolean
        Return False
    End Function

    Protected Overrides Function OnConfigItemUpdate(ByVal configViewChange As AbstractView) As Boolean
        If configViewChange.Id <> LogTypeSelectListId Then
            Return True
        End If

        Dim changedLogTypeSl As SelectListView = TryCast(configViewChange, SelectListView)

        If changedLogTypeSl Is Nothing Then
            Return False
        End If

        Dim currentLogTypeSl As SelectListView = TryCast(ConfigPage.GetViewById(LogTypeSelectListId), SelectListView)

        If currentLogTypeSl Is Nothing Then
            Return False
        End If

        If currentLogTypeSl.Selection = changedLogTypeSl.Selection Then
            Return False
        End If

        Dim newConfPage = InitConfigPageWithInput()
        ConfigPage = newConfPage.Page
        Return True
    End Function

    Protected Overrides Function GetName() As String
        Return ActionName
    End Function

    Protected Overrides Sub OnNewAction()
        Dim confPage = InitNewConfigPage()
        ConfigPage = confPage.Page
    End Sub

    Private Function InitNewConfigPage() As PageFactory
        Dim confPage = PageFactory.CreateEventActionPage(PageId, ActionName)
        confPage.WithLabel(InstructionsLabelId, Nothing, InstructionsLabelValue)
        confPage.WithDropDownSelectList(LogTypeSelectListId, "Log Type", LogTypeOptions)
        Return confPage
    End Function

    Private Function InitConfigPageWithInput() As PageFactory
        Dim confPage = InitNewConfigPage()
        confPage.WithInput(LogMessageInputId, "Message")
        Return confPage
    End Function

    Interface IWriteLogActionListener
        Inherits ActionTypeCollection.IActionTypeListener

        Sub WriteLog(ByVal logType As ELogType, ByVal message As String)
    End Interface

End Class