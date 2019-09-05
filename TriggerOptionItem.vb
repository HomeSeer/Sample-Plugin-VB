﻿''' <summary>
''' A simple Key-Value pair used by HTML liquid tags for representing a Trigger Option checkbox item
'''  on the Sample Trigger Feature Page.
''' </summary>
<Serializable>
Public Class TriggerOptionItem
    Public Property Id As Integer
    Public Property Name As String

    Public Sub New(ByVal id As Integer, ByVal name As String)
        Id = id
        Name = name
    End Sub

    Public Sub New()
    End Sub
End Class