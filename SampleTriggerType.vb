Imports HomeSeer.Jui.Views
Imports HomeSeer.PluginSdk.Events

Public Class SampleTriggerType
    Inherits AbstractTriggerType

    Public Const TriggerNumber As Integer = 1
    Private Const TriggerName As String = "Sample Trigger"

    Private ReadOnly Property OptionCountSlId As String
        Get
            Return $"{PageId}-optioncountsl"
        End Get
    End Property

    Private Const OptionCountSlName As String = "Number of Options Checked"

    Private ReadOnly Property OptionNumSlId As String
        Get
            Return $"{PageId}-optionnumsl"
        End Get
    End Property

    Private Const OptionNumSlName As String = "Required Option"

    Public Function ShouldTriggerFire(ParamArray triggerOptions As Boolean()) As Boolean
        Select Case SelectedSubTriggerIndex
            Case 0
                Dim numRequiredOptions = GetSelectedOptionCount() + 1
                Return numRequiredOptions <> 0 AndAlso triggerOptions.Any(Function(triggerOption) triggerOption)
            Case 1
                Dim specificRequiredOption = GetSelectedSpecificOptionNum()

                If triggerOptions.Length < specificRequiredOption + 1 Then
                    Return False
                End If

                Return triggerOptions(specificRequiredOption)
            Case 2
                Return Not triggerOptions.Any(Function(triggerOption) triggerOption)
            Case 3
                Return triggerOptions.Any(Function(triggerOption) triggerOption)
            Case Else
                Return False
        End Select
    End Function

    Public Sub New(ByVal trigInfo As TrigActInfo)
        MyBase.New(trigInfo)
    End Sub

    Public Sub New(ByVal id As Integer, ByVal eventRef As Integer, ByVal selectedSubTriggerIndex As Integer, ByVal dataIn As Byte())
        MyBase.New(id, eventRef, selectedSubTriggerIndex, dataIn)
    End Sub

    Public Sub New()
    End Sub

    Protected Overrides Property SubTriggerTypeNames As List(Of String) = New List(Of String) From {
        "Button click with X options checked",
        "Button click with specific option checked",
        "Button click with no options checked",
        "Button click with any options checked"
    }

    Protected Overrides Function GetName() As String
        Return TriggerName
    End Function

    Protected Overrides Sub OnNewTrigger()
        Select Case SelectedSubTriggerIndex
            Case 0
                ConfigPage = InitializeXOptionsPage().Page
            Case 1
                ConfigPage = InitializeSpecificOptionPage().Page
            Case Else
                ConfigPage = InitializeDefaultPage().Page
        End Select
    End Sub

    Public Overrides Function IsFullyConfigured() As Boolean
        Select Case SelectedSubTriggerIndex
            Case 0
                Return GetSelectedOptionCount() >= 0
            Case 1
                Return GetSelectedSpecificOptionNum() >= 0
            Case Else
                Return True
        End Select
    End Function

    Protected Overrides Function OnConfigItemUpdate(ByVal configViewChange As AbstractView) As Boolean
        Return True
    End Function

    Public Overrides Function GetPrettyString() As String
        Select Case SelectedSubTriggerIndex
            Case 0

                Try
                    Dim optionCountSl = TryCast(ConfigPage?.GetViewById(OptionCountSlId), SelectListView)
                    Return $"the button on the Sample Plugin Trigger Feature page is clicked and {(If(optionCountSl?.GetSelectedOption(), "???"))} options are checked"
                Catch exception As Exception
                    Console.WriteLine(exception)
                    Return $"the button on the Sample Plugin Trigger Feature page is clicked and ??? options are checked"
                End Try

            Case 1

                Try
                    Dim optionNumSl = TryCast(ConfigPage?.GetViewById(OptionNumSlId), SelectListView)
                    Return $"the button on the Sample Plugin Trigger Feature page is clicked and option number {(If(optionNumSl?.GetSelectedOption(), "???"))} is checked"
                Catch exception As Exception
                    Console.WriteLine(exception)
                    Return $"the button on the Sample Plugin Trigger Feature page is clicked and option number ??? is checked"
                End Try

            Case 2
                Return $"the button the Sample Plugin Trigger Feature page is clicked and no options are checked"
            Case Else
                Return $"the button the Sample Plugin Trigger Feature page is clicked"
        End Select
    End Function

    Public Overrides Function IsTriggerTrue(ByVal isCondition As Boolean) As Boolean
        Return True
    End Function

    Public Overrides Function ReferencesDeviceOrFeature(ByVal devOrFeatRef As Integer) As Boolean
        Return False
    End Function

    Private Function InitializeXOptionsPage() As PageFactory
        Dim cpf = InitializeDefaultPage()
        cpf.WithDropDownSelectList(OptionCountSlId, OptionCountSlName, {"1", "2", "3", "4"}.ToList())
        Return cpf
    End Function

    Private Function InitializeSpecificOptionPage() As PageFactory
        Dim cpf = InitializeDefaultPage()
        cpf.WithDropDownSelectList(OptionNumSlId, OptionNumSlName, {"1", "2", "3", "4"}.ToList())
        Return cpf
    End Function

    Private Function InitializeDefaultPage() As PageFactory
        Dim cpf = PageFactory.CreateEventTriggerPage(PageId, TriggerName)
        Return cpf
    End Function

    Private Function GetSelectedSpecificOptionNum() As Integer
        Try
            Dim optionNumSl = TryCast(ConfigPage?.GetViewById(OptionNumSlId), SelectListView)
            Return If(optionNumSl?.Selection, -1)
        Catch exception As Exception
            Console.WriteLine(exception)
            Return -1
        End Try
    End Function

    Private Function GetSelectedOptionCount() As Integer
        Try
            Dim optionCountSl = TryCast(ConfigPage?.GetViewById(OptionCountSlId), SelectListView)
            Return (If(optionCountSl?.Selection, -1))
        Catch exception As Exception
            Console.WriteLine(exception)
            Return -1
        End Try
    End Function
End Class