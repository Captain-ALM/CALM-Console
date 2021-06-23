Imports captainalm.calmcon.api

Module hooks
    Public runcommandhook As New RunCommandHook(AddressOf comrun)
    Public readoutputhook As New ReadOutputHook(AddressOf rdout)
    Public writeoutputhook As New WriteOutputHook(AddressOf wtout)
    Public addecmdhook As New AddExternalCommandHook(AddressOf addecmd)
    Public listecmdshook As New ListExternalCommandsHook(AddressOf listecmd)
    Public removeecmdhook As New RemoveExternalCommandHook(AddressOf removeecmd)
    Public addesnxhook As New AddExternalSyntaxHook(AddressOf addesnx)
    Public listesnxshook As New ListExternalSyntaxesHook(AddressOf listesnx)
    Public removeesnxhook As New RemoveExternalSyntaxHook(AddressOf removeesnx)

    Public e_cmd_reg As New List(Of e_cmd)
    Public e_snx_reg As New List(Of e_snx)
    Private cukeycmd As Integer = 0
    Private cukeysnx As Integer = 0

    Private slockkeygen As New Object()
    Private slockcomrun As New Object()
    Private slockoutput As New Object()
    Private slockmansnx As New Object()
    Private slockmancmd As New Object()

    Private Function listesnxint(libname As String) As String()
        Dim toret As New List(Of String)
        SyncLock slockmansnx
            Try
                For Each e As e_snx In e_snx_reg
                    If e.owner = libname Then
                        toret.Add(e.id & " : " & e.name)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                toret = Nothing
            End Try
        End SyncLock
        If toret Is Nothing Then _
        Return toret.ToArray _
        Else _
        Return Nothing
    End Function

    Public Function listesnx(ByVal libname As String) As String()
        Return listesnxint(libname)
    End Function

    Private Function listecmdint(libname As String) As String()
        Dim toret As New List(Of String)
        SyncLock slockmancmd
            Try
                For Each e As e_cmd In e_cmd_reg
                    If e.owner = libname Then
                        toret.Add(e.id & " : " & e.name)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                toret = Nothing
            End Try
        End SyncLock
        If toret Is Nothing Then _
        Return toret.ToArray _
        Else _
        Return Nothing
    End Function

    Public Function listecmd(ByVal libname As String) As String()
        Return listecmdint(libname)
    End Function

    Private Function removeesnxint(libname As String, id As Integer) As Boolean
        Dim toret As Boolean = False
        SyncLock slockmansnx
            Try
               toret = True
                For Each e As e_snx In e_snx_reg
                    If e.id = id And e.owner = libname Then
                        toret = toret And e_snx_reg.Remove(e)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                toret = False
            End Try
        End SyncLock
        Return toret
    End Function

    Public Function removeesnx(ByVal libname As String, ByVal id As Integer) As Boolean
        Return removeesnxint(libname, id)
    End Function

    Private Function removeecmdint(libname As String, id As Integer) As Boolean
        Dim toret As Boolean = False
        SyncLock slockmancmd
            Try
                toret = True
                For Each e As e_cmd In e_cmd_reg
                    If e.id = id And e.owner = libname Then
                        If Not e.hasonlydomainname Then
                            toret = toret And commands.Remove(e.name)
                        End If
                        toret = toret And commands.Remove(e.owner & "." & e.name)
                        toret = toret And e_cmd_reg.Remove(e)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                toret = False
            End Try
        End SyncLock
        Return toret
    End Function

    Public Function removeecmd(ByVal libname As String, ByVal id As Integer) As Boolean
        Return removeecmdint(libname, id)
    End Function

    Private Function addesnxint(libname As String, syntax As ISyntax) As Integer
        Dim toret As Integer = 0
        SyncLock slockmansnx
            Try
                newkeysnx()
                Dim ss As New e_snx
                ss.name = syntax.name()
                ss.id = cukeysnx
                ss.owner = libname
                If Not syntaxes.ContainsKey(ss.name) Then
                    syntaxes.Add(ss.name, syntax)
                Else
                    Throw New Exception("Syntax Already Exists!")
                End If
                e_snx_reg.Add(ss)
                toret = ss.id
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                If cukeysnx <> 0 Then _
                cukeysnx -= 1

                toret = 0
            End Try
        End SyncLock
        Return toret
    End Function

    Public Function addesnx(ByVal libname As String, ByVal syntax As ISyntax) As Integer
        Return addesnxint(libname, syntax)
    End Function

    Private Function addecmdint(libname As String, command As Command) As Integer
        Dim toret As Integer = 0
        SyncLock slockmancmd
            Try
                newkeycmd()
                Dim cs As New e_cmd
                cs.name = command.name
                cs.help = command.help
                cs.id = cukeycmd
                cs.owner = libname
                If Not commands.ContainsKey(command.name) Then
                    commands.Add(command.name, New executable_command(command.name, command.command))
                    cs.hasonlydomainname = False
                    commands.Add(libname & "." & command.name, New executable_command(libname & "." & command.name, command.command))
                Else
                    cs.hasonlydomainname = True
                    If Not commands.ContainsKey(libname & "." & command.name) Then
                        commands.Add(libname & "." & command.name, New executable_command(libname & "." & command.name, command.command))
                    Else
                        Throw New Exception("Command Already Exists!")
                    End If
                End If
                e_cmd_reg.Add(cs)
                toret = cs.id
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
                If cukeycmd <> 0 Then _
                cukeycmd -= 1

                toret = 0
            End Try
        End SyncLock
        Return toret
    End Function

    Public Function addecmd(ByVal libname As String, ByVal command As Command) As Integer
        Return addecmdint(libname, command)
    End Function

    Friend Sub newkeycmd()
        SyncLock slockkeygen
            If cukeycmd = Integer.MaxValue Then
                cukeycmd = Integer.MinValue
            Else
                cukeycmd += 1
            End If
            If cukeycmd = 0 Then
                Throw New Exception("Key Reached Limit!")
            End If
        End SyncLock
    End Sub

    Friend Sub newkeysnx()
        SyncLock slockkeygen
            If cukeysnx = Integer.MaxValue Then
                cukeysnx = Integer.MinValue
            Else
                cukeysnx += 1
            End If
            If cukeysnx = 0 Then
                Throw New Exception("Key Reached Limit!")
            End If
        End SyncLock
    End Sub

    Friend Function help_lst() As List(Of String)
        Dim ret As New List(Of String)
        SyncLock slockmancmd
            For Each s As e_cmd In e_cmd_reg
                If s.help <> "" And Not s.hasonlydomainname Then _
                    ret.Add(s.help)
            Next
        End SyncLock
        Return ret
    End Function

    Friend Function help_lst_ext() As List(Of String)
        Dim ret As New List(Of String)
        SyncLock slockmancmd
            For Each s As e_cmd In e_cmd_reg
                If s.help <> "" And Not s.hasonlydomainname Then _
                    ret.Add(s.help)

                If s.help <> "" Then _
                    ret.Add(s.owner & "." & s.help)
            Next
        End SyncLock
        Return ret
    End Function

    Public Sub wtout(ByVal text As OutputText)
        wtoutint(text)
    End Sub

    Private Sub wtoutint(ByVal text As OutputText)
        SyncLock slockoutput
            If Not form_instance Is Nothing Then
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
                toret = normalizeLineEndings(form_instance.txtbxlog.Text)
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

Public Structure e_cmd
    Public owner As String
    Public name As String
    Public id As Integer
    Public help As String
    Public hasonlydomainname As Boolean
End Structure

Public Structure e_snx
    Public owner As String
    Public name As String
    Public id As Integer
End Structure
