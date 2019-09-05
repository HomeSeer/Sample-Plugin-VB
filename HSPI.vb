Imports System.Text
Imports HomeSeer.Jui.Types
Imports HomeSeer.Jui.Views
Imports HomeSeer.PluginSdk
Imports HomeSeer.PluginSdk.Logging
Imports Newtonsoft.Json

''' <inheritdoc cref="AbstractPlugin"/>
''' <summary>
''' The plugin class for HomeSeer Sample Plugin that implements the <see cref="AbstractPlugin"/> base class.
''' </summary>
''' <remarks>
''' This class is accessed by HomeSeer and requires that its name be "HSPI" and be located in a namespace
'''  that corresponds to the name of the executable. For this plugin, "HomeSeerSamplePlugin_VB" the executable
'''  file is "HSPI_HomeSeerSamplePlugin_VB.exe" and this class is HSPI_HomeSeerSamplePlugin_VB.HSPI
''' <para>
''' If HomeSeer is unable to find this class, the plugin will not start.
''' </para>
''' </remarks>
Public Class HSPI
    Inherits AbstractPlugin
    Implements WriteLogSampleActionType.IWriteLogActionListener
    
   ''' <inheritdoc />
   ''' <remarks>
   ''' This ID is used to identify the plugin and should be unique across all plugins
   ''' <para>
   ''' This must match the MSBuild property $(PluginId) as this will be used to copy
   '''  all of the HTML feature pages located in .\html\ to a relative directory
   '''  within the HomeSeer html folder.
   ''' </para>
   ''' <para>
   ''' The relative address for all of the HTML pages will end up looking like this:
   '''  ..\Homeseer\Homeseer\html\HomeSeerSamplePlugin_VB\
   ''' </para>
   ''' </remarks>
    Public Overrides ReadOnly Property Id As String
        Get
            Return "HomeSeerSamplePlugin_VB"
        End Get
    End Property
   
    ''' <inheritdoc />
    ''' <remarks>
    ''' This is the readable name for the plugin that is displayed throughout HomeSeer
    ''' </remarks>
    Public Overrides ReadOnly Property Name As String
        Get 
            Return "HomeSeerSamplePlugin-VB"
        End Get
    End Property

    ''' <inheritdoc />
    Protected Overrides ReadOnly Property SettingsFileName As String
        Get 
            Return "HomeSeerSamplePlugin-VB.ini"
        End Get
    End Property

    Public Sub New()
        'Initialize the plugin 

        'Enable internal debug logging to console
        LogDebug = True
        'Setup anything that needs to be configured before a connection to HomeSeer is established
        ' like initializing the starting state of anything needed for the operation of the plugin
            
        'Such as initializing the settings pages presented to the user (currently saved state is loaded later)
        InitializeSettingsPages()
        
        'Or adding an event action or trigger type definition to the list of types supported by your plugin
        ActionTypes.AddActionType(GetType(WriteLogSampleActionType))
        TriggerTypes.AddTriggerType(GetType(SampleTriggerType))
    End Sub
    
    ''' <summary>
    ''' Initialize the starting state of the settings pages for the HomeSeerSamplePlugin.
    '''  This constructs the framework that the user configurable settings for the plugin live in.
    '''  Any saved configuration options are loaded later in <see cref="Initialize"/> using
    '''  <see cref="AbstractPlugin.LoadSettingsFromIni"/>
    ''' </summary>
    ''' <remarks>
    ''' For ease of use throughout the plugin, all of the view IDs, names, and values (non-volatile data)
    '''  are stored in the <see cref="HSPI_HomeSeerSamplePlugin_VB.Constants.Settings"/> static class.
    ''' </remarks>
    Private Sub InitializeSettingsPages()
        'Initialize the first settings page
        ' This page is used to manipulate the behavior of the sample plugin
        
        'Start a PageFactory to construct the Page
        Dim settingsPage1 = PageFactory.CreateSettingsPage(Constants.Settings.SettingsPage1Id, Constants.Settings.SettingsPage1Name)
        'Add a LabelView to the page
        settingsPage1.WithLabel(Constants.Settings.Sp1ColorLabelId, Nothing, Constants.Settings.Sp1ColorLabelValue)
        'Create a list of ToggleViews using the keys and values stored in Constants.Settings.ColorMap as
        ' the IDs and Names respectively
        Dim colorToggles = Constants.Settings.ColorMap.[Select](Function(kvp) New ToggleView(kvp.Key, kvp.Value, True) With {
            .ToggleType = EToggleType.Checkbox
        }).ToList()
        'Add a ViewGroup containing all of the ToggleViews to the page
        settingsPage1.WithGroup(Constants.Settings.Sp1ColorGroupId, Constants.Settings.Sp1ColorGroupName, colorToggles)
        'Create 2 ToggleViews for controlling the visibility of the other two settings pages
        Dim pageToggles = New List(Of ToggleView) From {
            New ToggleView(Constants.Settings.Sp1PageVisToggle1Id, Constants.Settings.Sp1PageVisToggle1Name, True),
            New ToggleView(Constants.Settings.Sp1PageVisToggle2Id, Constants.Settings.Sp1PageVisToggle2Name, True)
        }
        'Add a ViewGroup containing all of the ToggleViews to the page
        settingsPage1.WithGroup(Constants.Settings.Sp1PageToggleGroupId, Constants.Settings.Sp1PageToggleGroupName, pageToggles)
        'Add the first page to the list of plugin settings pages
        Settings.Add(settingsPage1.Page)
        
        'Initialize the second settings page
        ' This page is used to visually demonstrate all of the available JUI views except for InputViews.
        ' None of these views interact with the plugin and are merely for show.
        
        'Start a PageFactory to construct the Page
        Dim settingsPage2 = PageFactory.CreateSettingsPage(Constants.Settings.SettingsPage2Id, Constants.Settings.SettingsPage2Name)
        'Add a LabelView with a title to the page
        settingsPage2.WithLabel(Constants.Settings.Sp2LabelWTitleId, Constants.Settings.Sp2LabelWTitleName, Constants.Settings.Sp2LabelWTitleValue)
        'Add a LabelView without a title to the page
        settingsPage2.WithLabel(Constants.Settings.Sp2LabelWoTitleId, Nothing, Constants.Settings.Sp2LabelWoTitleValue)
        'Add a toggle switch to the page
        settingsPage2.WithToggle(Constants.Settings.Sp2SampleToggleId, Constants.Settings.Sp2SampleToggleName)
        'Add a checkbox to the page
        settingsPage2.WithCheckBox(Constants.Settings.Sp2SampleCheckBoxId, Constants.Settings.Sp2SampleCheckBoxName)
        'Add a drop down select list to the page
        settingsPage2.WithDropDownSelectList(Constants.Settings.Sp2SelectListId, Constants.Settings.Sp2SelectListName, Constants.Settings.Sp2SelectListOptions)
        'Add a radio select list to the page
        settingsPage2.WithRadioSelectList(Constants.Settings.Sp2RadioSlId, Constants.Settings.Sp2RadioSlName, Constants.Settings.Sp2SelectListOptions)
        'Add the second page to the list of plugin settings pages
        Settings.Add(settingsPage2.Page)
        
        'Initialize the third settings page
        ' This page is used to visually demonstrate the different types of JUI InputViews.
        
        'Start a PageFactory to construct the Page
        Dim settingsPage3 = PageFactory.CreateSettingsPage(Constants.Settings.SettingsPage3Id, Constants.Settings.SettingsPage3Name)
        'Add a text InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput1Id, Constants.Settings.Sp3SampleInput1Name)
        'Add a number InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput2Id, Constants.Settings.Sp3SampleInput2Name, EInputType.Number)
        'Add an email InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput3Id, Constants.Settings.Sp3SampleInput3Name, EInputType.Email)
        'Add a URL InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput4Id, Constants.Settings.Sp3SampleInput4Name, EInputType.Url)
        'Add a password InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput5Id, Constants.Settings.Sp3SampleInput5Name, EInputType.Password)
        'Add a decimal InputView to the page
        settingsPage3.WithInput(Constants.Settings.Sp3SampleInput6Id, Constants.Settings.Sp3SampleInput6Name, EInputType.Decimal)
        'Add the third page to the list of plugin settings pages
        Settings.Add(settingsPage3.Page)
    End Sub

    Protected Overrides Sub Initialize()
        'Load the state of Settings saved to INI if there are any.
        LoadSettingsFromIni()
        If LogDebug Then
            Console.WriteLine("Registering feature pages")
        End If
        'Initialize feature pages
        HomeSeerSystem.RegisterFeaturePage(Id, "sample-guided-process.html", "Sample Guided Process")
        HomeSeerSystem.RegisterFeaturePage(Id, "sample-blank.html", "Sample Blank Page")
        HomeSeerSystem.RegisterFeaturePage(Id, "sample-trigger-feature.html", "Trigger Feature Page")
        Console.WriteLine("Initialized")
        Status = PluginStatus.Ok()
    End Sub

    Protected Overrides Function OnSettingChange(pageId As String, currentView As AbstractView, changedView As AbstractView) As Boolean
        
        'React to the toggles that control the visibility of the last 2 settings pages
        If changedView.Id = Constants.Settings.Sp1PageVisToggle1Id Then
            'Make sure the changed view is a ToggleView
            Dim tView As ToggleView = TryCast(changedView, ToggleView)
            If tView Is Nothing Then
                Return False
            End If
            
            'Show/Hide the second page based on the new state of the toggle
            If tView.IsEnabled Then
                Settings.ShowPageById(Constants.Settings.SettingsPage2Id)
            Else 
                Settings.HidePageById(Constants.Settings.SettingsPage2Id)
            End If
        ElseIf changedView.Id = Constants.Settings.Sp1PageVisToggle2Id Then
            'Make sure the changed view is a ToggleView
            Dim tView As ToggleView = TryCast(changedView, ToggleView)
            If tView Is Nothing Then
                Return False
            End If
            
            'Show/Hide the second page based on the new state of the toggle
            If tView.IsEnabled Then
                Settings.ShowPageById(Constants.Settings.SettingsPage3Id)
            Else 
                Settings.HidePageById(Constants.Settings.SettingsPage3Id)
            End If
        Else 
            If LogDebug Then
                Console.WriteLine($"View ID {changedView.Id} does not match any views on the page.")
            End If
        End If
        
        Return True
    End Function

    ''' <inheritdoc />
    ''' <remarks>
    ''' This plugin does not have a shifting operational state; so this method is not used.
    ''' </remarks>
    Protected Overrides Sub BeforeReturnStatus()
    End Sub
   
    ''' <inheritdoc />
    ''' <remarks>
    ''' Process any HTTP POST requests targeting pages registered to your plugin.
    ''' <para>
    ''' This is a very flexible process that does not have a predefined structure. The form <see cref="data"/> sends
    '''  from a page is entirely up to you and what works for you.  JSON and Base64 strings are encouraged because
    '''  of how readily available resources are to translate to/from these types. In Javascript, see JSON.stringify();
    '''  and window.btoa();
    ''' </para>
    ''' </remarks>
    Public Overrides Function PostBackProc(page As String, data As String, user As String, userRights As Integer) As String
        If LogDebug Then
            Console.WriteLine("PostBack")
        End If
        
        Dim response = ""

        Select Case page
            Case "sample-trigger-feature.html"

                'Handle the Trigger Feature page
                Try
                    Dim triggerOptions = JsonConvert.DeserializeObject(Of List(Of Boolean))(data)
                    
                    'Get all triggers configured on the HomeSeer system that are of the SampleTriggerType
                    Dim configuredTriggers = HomeSeerSystem.GetTriggersByType(Name, SampleTriggerType.TriggerNumber)

                    If configuredTriggers.Length = 0 Then
                        Return "No triggers configured to fire."
                    End If

                    'Handle each trigger that matches
                    For Each configuredTrigger In configuredTriggers
                        Dim trig = New SampleTriggerType(configuredTrigger)

                        If trig.ShouldTriggerFire(triggerOptions.ToArray()) Then
                            HomeSeerSystem.TriggerFire(Name, configuredTrigger)
                        End If
                    Next

                Catch exception As JsonSerializationException
                    If LogDebug Then
                        Console.WriteLine(exception)
                    End If
                    response = $"Error while deserializing data: {exception.Message}"
                End Try

            Case "sample-guided-process.html"

                'Handle the Guided Process page
                Try
                    Dim postData = JsonConvert.DeserializeObject(Of SampleGuidedProcessData)(data)
                    If LogDebug Then
                        Console.WriteLine("Post back from sample-guided-process page")
                    End If
                    response = postData.GetResponse()
                Catch exception As JsonSerializationException
                    If LogDebug Then
                        Console.WriteLine(exception.Message)
                    End If
                    response = "error"
                End Try

            Case Else
                response = "error"
        End Select

        Return response
    End Function
    
    ''' <summary>
    ''' Called by the sample guided process feature page through a liquid tag to provide the list of available colors
    ''' <para>
    ''' {{plugin_function 'HomeSeerSamplePlugin' 'GetSampleSelectList' []}}
    ''' </para>
    ''' </summary>
    ''' <returns>The HTML for the list of select list options</returns>
    Public Function GetSampleSelectList() As String
        If LogDebug Then
            Console.WriteLine("Getting sample select list for sample-guided-process page")
        End If
        Dim sb = New StringBuilder("<select class=""mdb-select md-form"" id=""step3SampleSelectList"">")
        sb.Append(Environment.NewLine)
        sb.Append("<option value="""" disabled selected>Color</option>")
        sb.Append(Environment.NewLine)
        Dim colorList = New List(Of String)()
       

        Try
            Dim colorSettings = Settings("settings-page2").GetViewById("settings-page2.colorgroup")
            Dim colorViewGroup As ViewGroup = TryCast(colorSettings, ViewGroup)
            Dim colorView As ToggleView

            If colorViewGroup Is Nothing Then
                Throw New ViewTypeMismatchException("No View Group found containing colors")
            End If

            For Each view In colorViewGroup.Views

                colorView = TryCast(view, ToggleView)
                If colorView Is Nothing Then
                    Continue For
                End If

                colorList.Add(If(colorView.IsEnabled, colorView.Name, ""))
            Next

        Catch exception As Exception
            If LogDebug Then
                Console.WriteLine(exception)
            End If
            colorList = Constants.Settings.ColorMap.Values.ToList()
        End Try

        For i = 0 To colorList.Count - 1
            Dim color = colorList(i)

            If String.IsNullOrEmpty(color) Then
                Continue For
            End If

            sb.Append("<option value=""")
            sb.Append(i)
            sb.Append(""">")
            sb.Append(color)
            sb.Append("</option>")
            sb.Append(Environment.NewLine)
        Next

        sb.Append("</select>")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Called by the sample trigger feature page to get the HTML for a list of checkboxes to use a trigger options
    ''' <para>
    ''' {{list=plugin_function 'HomeSeerSamplePlugin' 'GetTriggerOptionsHtml' [2]}}
    ''' </para>
    ''' </summary>
    ''' <param name="numTriggerOptions">The number of checkboxes to generate</param>
    ''' <returns>
    ''' A List of HTML strings representing checkbox input elements
    ''' </returns>
    Public Function GetTriggerOptionsHtml(ByVal numTriggerOptions As Integer) As List(Of String)
        Dim triggerOptions = New List(Of String)()

        For i = 1 To numTriggerOptions
            Dim cbTrigOpt = New ToggleView($"checkbox-triggeroption{i}", $"Trigger Option {i}") With {
                .ToggleType = EToggleType.Checkbox
            }
            triggerOptions.Add(cbTrigOpt.ToHtml())
        Next

        Return triggerOptions
    End Function

    ''' <summary>
    ''' Called by the sample trigger feature page to get trigger option items as a list to populate HTML on the page.
    ''' <para>
    ''' {{list2=plugin_function 'HomeSeerSamplePlugin' 'GetTriggerOptions' [2]}}
    ''' </para>
    ''' </summary>
    ''' <param name="numTriggerOptions">The number of trigger options to generate.</param>
    ''' <returns>
    ''' A List of <see cref="TriggerOptionItem"/>s used for checkbox input HTML element IDs and Names
    ''' </returns>
    Public Function GetTriggerOption(ByVal numTriggerOptions As Integer) As List(Of TriggerOptionItem)
        Dim triggerOptions = New List(Of TriggerOptionItem)()

        For i = 1 To numTriggerOptions
            triggerOptions.Add(New TriggerOptionItem(i, $"Trigger Option {i}"))
        Next

        Return triggerOptions
    End Function

   '<inheritdoc />
    Public Sub WriteLog(ByVal logType As ELogType, ByVal message As String) Implements WriteLogSampleActionType.IWriteLogActionListener.WriteLog
        HomeSeerSystem.WriteLog(logType, message, Name)
    End Sub
End Class