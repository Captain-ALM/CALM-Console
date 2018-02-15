Imports captainalm.calmcon.api

Module hooks
    Public runcommandhook As New RunCommandHook(AddressOf comrun)
    Public readoutputhook As New ReadOutputHook(AddressOf rdout)
    Public writeoutputhook As New WriteOutputHook(AddressOf wtout)

    Private slockcomrun As New Object()
    Private slockoutput As New Object()

    Public Sub wtout(ByVal text As String)
        wtoutint(text)
    End Sub

    Private Sub wtoutint(ByVal text As String)
        SyncLock slockoutput
            If Not form_instance Is Nothing Then
                form_instance.callonform(Sub() form_instance.txtbxlog.AppendText(text))
            Else
                Throw New NullReferenceException("The form is not initialised.")
            End If
        End SyncLock
    End Sub

    Public Function rdout() As String
        Return rdoutint()
    End Function

    Private Function rdoutint() As String
        Dim toret As String = ""
        SyncLock slockoutput
            If Not form_instance Is Nothing Then
                toret = form_instance.txtbxlog.Text
            Else
                Throw New NullReferenceException("The form is not initialised.")
            End If
        End SyncLock
        Return toret
    End Function

    Public Function comrun(ByVal com As String, ByVal args As String()) As String
        Return comrunint(com, args)
    End Function

    Private Function comrunint(ByVal com As String, ByVal args As String()) As String
        Dim toret As String = ""
        SyncLock slockcomrun
            Dim command As New int_command(com, args)
            toret = command.execute()
        End SyncLock
        Return toret
    End Function
End Module
