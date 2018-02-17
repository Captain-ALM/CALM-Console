Imports captainalm.calmcon.api

Module hooks
    Public runcommandhook As New RunCommandHook(AddressOf comrun)
    Public readoutputhook As New ReadOutputHook(AddressOf rdout)
    Public writeoutputhook As New WriteOutputHook(AddressOf wtout)

    Private slockcomrun As New Object()
    Private slockoutput As New Object()

    Public Sub wtout(ByVal text As OutputText)
        wtoutint(text)
    End Sub

    Private Sub wtoutint(ByVal text As OutputText)
        SyncLock slockoutput
            If Not form_instance Is Nothing Then
                'form_instance.callonform(Sub() form_instance.txtbxlog.AppendText(text))
                form_instance.render_outtxt(form_instance.txtbxlog, text)
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
                toret = form_instance.txtbxlog.Text.Replace(ControlChars.Lf, ControlChars.CrLf)
            Else
                Throw New NullReferenceException("The form is not initialised.")
            End If
        End SyncLock
        Return toret
    End Function

    Public Function comrun(ByVal com As String, ByVal args As String()) As OutputText
        Return comrunint(com, args)
    End Function

    Private Function comrunint(ByVal com As String, ByVal args As String()) As OutputText
        Dim toret As OutputText = ""
        SyncLock slockcomrun
            Dim command As New int_command(com, args)
            toret = command.execute()
        End SyncLock
        Return toret
    End Function
End Module
