Public Module Devices
    
    Public ReadOnly Property SampleDeviceTypeList As List(Of String)
        Get
            Return New List(Of String) From {
                "Line-powered switch",
                "Line-powered sensor"
                }
        End Get
    End Property

    Public ReadOnly Property SampleDeviceTypeFeatures As List(Of String())
        Get
            Return New List(Of String()) From {
                LinePoweredSwitchFeatures,
                LinePoweredSensorFeatures
                }
        End Get
    End Property

    Public ReadOnly Property LinePoweredSwitchFeatures As String()
        Get
            Return {"On-Off control feature"}
        End Get
    End Property

    Public ReadOnly Property LinePoweredSensorFeatures As String()
        Get
            Return {"Open-Close status feature"}
        End Get
    End Property
    
End Module