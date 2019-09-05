Module Program
    
    Dim _plugin As HSPI
    
    Sub Main(ByVal args As String())
        
        _plugin = New HSPI()
        
        _plugin.Connect(args)
        
    End Sub
End Module
