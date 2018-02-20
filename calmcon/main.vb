Imports System.Reflection
Imports System.IO
Imports System.Threading
Imports captainalm.calmcon.api

Public Class main
    Public cmdpos As cmd_gui_position = cmd_gui_position.top
    Public allowenter As Boolean = False
    Private aboutbx As New AboutBx
    Private license As String = ""
    Private description As String = ""
    Private checkchkbxthread As Thread
    Private flags_thread As Thread
    Private flags_threadabx As Thread
    Private flags_threaddel As Thread
    Private command_thread As Thread
    Private commands_init As Boolean = False
    Private prrun As Boolean = True
    Private lib_load_t As Thread
    Private hook_start_t As Thread
    Private hook_stop_t As Thread
    Private s_shown As Thread
    Private ll As Boolean = False
    Private hook_running As Boolean = False
    Private at_end As Boolean = False
    Private shutdown_hook_ran As Boolean = False

    Friend Sub render_outtxt(ByVal rtfbx As RichTextBox, ByVal optxt As OutputText)
        If Not rtfbx.InvokeRequired Then
            Dim blocks As OutputTextBlock() = optxt.ToOutputTextBlocks()
            For Each c_block As OutputTextBlock In blocks
                rtfbx.Select(rtfbx.TextLength, 0)
                rtfbx.SelectionColor = c_block.forecolor
                Dim fstyle_flag As FontStyle = FontStyle.Regular
                If c_block.bold Then
                    fstyle_flag += FontStyle.Bold
                End If
                If c_block.italic Then
                    fstyle_flag += FontStyle.Italic
                End If
                If c_block.underline Then
                    fstyle_flag += FontStyle.Underline
                End If
                If c_block.strikeout Then
                    fstyle_flag += FontStyle.Strikeout
                End If
                rtfbx.SelectionFont = New Font("Consolas", 8.25, fstyle_flag)
                rtfbx.AppendText(c_block.text)
            Next
            rtfbx.Select(rtfbx.TextLength, 0)
            rtfbx.SelectionColor = Color.Black
            rtfbx.SelectionFont = New Font("Consolas", 8.25, FontStyle.Regular)
        Else
            calloncontrol(rtfbx, Sub() render_outtxt(rtfbx, optxt))
        End If
    End Sub

    Private Sub main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        contrvis(False)
        disablechkbx = True

        cmdpos = gui_pos_edit(cmd_gui_position.top)

        contrvis(False)

        If File.Exists(assemblydir & "\license.txt") Then
            license = loadfile(assemblydir & "\license.txt")
        End If

        If File.Exists(assemblydir & "\description.txt") Then
            description = loadfile(assemblydir & "\description.txt")
        End If

        aboutbx.setupdata(description, license)

        logpath = assemblydir

        form_instance = Me

        txtbxlog.AutoWordSelection = False

        checkchkbxthread = New Thread(New ThreadStart(AddressOf chkchkbxtsub))
        flags_thread = New Thread(New ThreadStart(AddressOf flags_sub))
        command_thread = New Thread(New ThreadStart(AddressOf command_sub))
        flags_threadabx = New Thread(New ThreadStart(AddressOf flagsabx_sub))
        flags_threaddel = New Thread(New ThreadStart(AddressOf flagdel_sub))
        lib_load_t = New Thread(New ThreadStart(AddressOf lcjbrimm))
        s_shown = New Thread(New ThreadStart(AddressOf ceholmes))
        hook_start_t = New Thread(New ThreadStart(AddressOf starthookexecutor))
        hook_stop_t = New Thread(New ThreadStart(AddressOf stophookexecutor))
        checkchkbxthread.IsBackground = True
        flags_thread.IsBackground = True
        command_thread.IsBackground = True
        flags_threadabx.IsBackground = True
        flags_threaddel.IsBackground = True
        lib_load_t.IsBackground = True
        s_shown.IsBackground = True
        hook_start_t.IsBackground = True
        hook_stop_t.IsBackground = True
        checkchkbxthread.Start()
        flags_thread.Start()
        flags_threaddel.Start()
    End Sub
    ''' <summary>
    ''' This subroutine for the lib load t is dedicated to Lily Charlotte Jaine Brimm
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub lcjbrimm()
        Try
            callonform(Sub()
                           pgrsbarstatus.Style = ProgressBarStyle.Marquee
                           lblstatus.Text = "Loading..."
                       End Sub)
            init_snx()
            init_cmd()
            Try
                ll = True
                callonform(Sub()
                               butstop.Enabled = True
                               lblstatus.Text = "Loading Libraries..."
                           End Sub)
                get_dlls()
                load_dlls()
                load_libraries()
                cancel_action_thread()
                init_libraries()
                cancel_action_thread()
            Catch ex As Exception
                cancel_action = False
            End Try
        Catch ex As Exception
        Finally
            commands_init = True
            callonform(Sub()
                           butstop.Enabled = False
                           lblstatus.Text = ""
                           pgrsbarstatus.Style = ProgressBarStyle.Continuous
                       End Sub)
            ll = False
        End Try
    End Sub
    ''' <summary>
    ''' This subroutine for s show is dedicated to Charlotte Erin Holmes
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ceholmes()
        Dim cmdargs As String() = Environment.GetCommandLineArgs()
        Dim l_stack As New Stack(Of String)
        For i As Integer = 1 To cmdargs.Count - 1 Step 1
            If cmdargs(i) <> "" Then
                l_stack.Push(cmdargs(i))
            End If
        Next
        For i As Integer = 1 To l_stack.Count Step 1
            Dim comcmd As String = l_stack.Pop()
            command_stack.Push(comcmd)
        Next
        ll = True
        lib_load_t.Start()
        Try
            lib_load_t.Join()
        Catch ex As ThreadStateException
        End Try
        While ll
            Thread.Sleep(100)
        End While
        hook_running = True
        hook_start_t.Start()
        Try
            hook_start_t.Join()
        Catch ex As ThreadStateException
        End Try
        While hook_running
            Thread.Sleep(100)
        End While
        flags_threadabx.Start()
        command_thread.Start()
        disablechkbx = False
        callonform(Sub() contrvis(True))
    End Sub

    Private Sub cancel_action_thread()
        If cancel_action Then
            cancel_action = False
            Thread.CurrentThread.Abort()
        End If
    End Sub

    Private Sub starthookexecutor()
        callonform(Sub()
                       pgrsbarstatus.Style = ProgressBarStyle.Marquee
                       lblstatus.Text = "Executing Starting Hooks..."
                       butstop.Enabled = True
                   End Sub)
        For Each ihook As HookInfo In hooks_info.Values
            Try
                cancel_action_thread()
                If Not ihook.hook_runcommand Is Nothing Then
                    ihook.hook_runcommand.Invoke(runcommandhook)
                End If
                cancel_action_thread()
                If Not ihook.hook_readoutput Is Nothing Then
                    ihook.hook_readoutput.Invoke(readoutputhook)
                End If
                cancel_action_thread()
                If Not ihook.hook_writeoutput Is Nothing Then
                    ihook.hook_writeoutput.Invoke(writeoutputhook)
                End If
                cancel_action_thread()
                If Not ihook.hook_commandstack Is Nothing Then
                    ihook.hook_commandstack.Invoke(command_stack)
                End If
                cancel_action_thread()
                If Not ihook.hook_variabledictionary Is Nothing Then
                    ihook.hook_variabledictionary.Invoke(var_dict)
                End If
                cancel_action_thread()
                If Not ihook.hook_form Is Nothing Then
                    ihook.hook_form.Invoke(form_instance)
                End If
                cancel_action_thread()
                If Not ihook.hook_out_txtbx Is Nothing Then
                    ihook.hook_out_txtbx.Invoke(txtbxlog)
                End If
                cancel_action_thread()
                If Not ihook.hook_syntaxname Is Nothing Then
                    ihook.hook_syntaxname.Invoke(syntax_mode)
                End If
                cancel_action_thread()
                If Not ihook.hook_programstart Is Nothing Then
                    ihook.hook_programstart.Invoke()
                End If
                cancel_action_thread()
            Catch ex As ThreadAbortException
                cancel_action = False
                callonform(Sub()
                               butstop.Enabled = False
                               lblstatus.Text = ""
                               pgrsbarstatus.Style = ProgressBarStyle.Continuous
                           End Sub)
                hook_running = False
                Throw ex
            Catch ex As Exception
            End Try
        Next
        callonform(Sub()
                       butstop.Enabled = False
                       lblstatus.Text = ""
                       pgrsbarstatus.Style = ProgressBarStyle.Continuous
                   End Sub)
        hook_running = False
        cancel_action = False
    End Sub

    Private Sub stophookexecutor()
        callonform(Sub()
                       pgrsbarstatus.Style = ProgressBarStyle.Marquee
                       lblstatus.Text = "Executing Ending Hooks..."
                       butstop.Enabled = True
                   End Sub)
        For Each ihook As HookInfo In hooks_info.Values
            Try
                cancel_action_thread()
                If Not ihook.hook_programstop Is Nothing Then
                    ihook.hook_programstart.Invoke()
                End If
                cancel_action_thread()
            Catch ex As ThreadAbortException
                cancel_action = False
                callonform(Sub()
                               butstop.Enabled = False
                               lblstatus.Text = ""
                               pgrsbarstatus.Style = ProgressBarStyle.Continuous
                           End Sub)
                hook_running = False
                Throw ex
            Catch ex As Exception
            End Try
        Next
        callonform(Sub()
                       butstop.Enabled = False
                       lblstatus.Text = ""
                       pgrsbarstatus.Style = ProgressBarStyle.Continuous
                   End Sub)
        hook_running = False
        shutdown_hook_ran = True
        cancel_action = False
        callonform(Sub() Me.Close())
    End Sub

    Private Sub butmcmdarr_Click(sender As Object, e As EventArgs) Handles butmcmdarr.Click
        If cmdpos = cmd_gui_position.top Then
            cmdpos = gui_pos_edit(cmd_gui_position.bottom)
        ElseIf cmdpos = cmd_gui_position.bottom Then
            cmdpos = gui_pos_edit(cmd_gui_position.top)
        End If
    End Sub

    Public Function gui_pos_edit(newpos As cmd_gui_position) As cmd_gui_position
        contrvis(False)
        If newpos = cmd_gui_position.bottom Then
            butmcmdarr.Invoke(Sub() butmcmdarr.Text = "Move Command Area To The Top")
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.RowStyles(0).Height = 65)
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.RowStyles(1).Height = 26)
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.SetRow(TableLayoutPanel2, 1))
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.SetRow(GroupBoxLog, 0))
        ElseIf newpos = cmd_gui_position.top Then
            butmcmdarr.Invoke(Sub() butmcmdarr.Text = "Move Command Area To The Bottom")
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.RowStyles(0).Height = 26)
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.RowStyles(1).Height = 65)
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.SetRow(TableLayoutPanel2, 0))
            TableLayoutPanel1.Invoke(Sub() TableLayoutPanel1.SetRow(GroupBoxLog, 1))
        End If
        contrvis(True)
        Return newpos
    End Function

    Public Sub callonform(subtocall As [Delegate])
        Me.Invoke(subtocall)
    End Sub

    Public Sub calloncontrol(contr As Control, subtocall As [Delegate])
        contr.Invoke(subtocall)
    End Sub

    Private Sub butreset_Click(sender As Object, e As EventArgs) Handles butreset.Click
        contrvis(False)
        Process.Start(assemblypath, convertcmdlinetocmdlinestr(Environment.GetCommandLineArgs(), 1))
        Me.Close()
    End Sub

    Private Function convertcmdlinetocmdlinestr(cmdline As String(), start As Integer) As String
        Dim toreturn As String = ""
        Try
            For i As Integer = start To cmdline.Length - 1 Step 1
                toreturn = toreturn & ControlChars.Quote & cmdline(i) & ControlChars.Quote
                If i <> cmdline.Length - 1 Then
                    toreturn = toreturn & " "
                End If
            Next
        Catch ex As Exception
        End Try
        Return toreturn
    End Function

    Private Sub butexit_Click(sender As Object, e As EventArgs) Handles butexit.Click
        contrvis(False)
        Me.Close()
    End Sub

    Private Sub txtbxcmd_KeyDown(sender As Object, e As KeyEventArgs) Handles txtbxcmd.KeyDown
        If (e.KeyCode = Keys.A And e.Control And Not txtbxcmd.SelectionLength >= txtbxcmd.Text.Length) Then
            e.SuppressKeyPress = True
            txtbxcmd.SelectAll()
            e.Handled = True
            'ElseIf e.KeyCode = Keys.A And e.Control Then
            '    e.SuppressKeyPress = True
            '    e.Handled = True
        ElseIf e.KeyCode = Keys.Return And Not allowenter Then
            e.SuppressKeyPress = True
            e.Handled = True
            contrcmdvis(False)
            cmd_inter(txtbxcmd.Text)
            contrcmdvis(True)
        ElseIf e.KeyCode = Keys.Up And e.Alt And command_buffer_shortcuts_enabled Then
            e.SuppressKeyPress = True
            e.Handled = True
            command_buffer_index -= 1
            If command_buffer_index > command_buffer_limit Then
                command_buffer_index = command_buffer_limit
            End If
            If command_buffer_index > command_buffer.Count Then
                command_buffer_index = command_buffer.Count
            End If
            If command_buffer_index <= 0 Then
                command_buffer_index = 1
            End If
            If command_buffer_limit > 0 Then
                If Not command_buffer.Count = 0 Then
                    txtbxcmd.Text = command_buffer(command_buffer_index - 1)
                End If
            End If
        ElseIf e.KeyCode = Keys.Down And e.Alt And command_buffer_shortcuts_enabled Then
            e.SuppressKeyPress = True
            e.Handled = True
            command_buffer_index += 1
            If command_buffer_index > command_buffer_limit Then
                command_buffer_index = command_buffer_limit
            End If
            If command_buffer_index > command_buffer.Count Then
                command_buffer_index = command_buffer.Count
            End If
            If command_buffer_index <= 0 Then
                command_buffer_index = 1
            End If
            If command_buffer_limit > 0 Then
                If Not command_buffer.Count = 0 Then
                    txtbxcmd.Text = command_buffer(command_buffer_index - 1)
                End If
            End If
        End If
    End Sub

    Private Sub txtbxlog_KeyDown(sender As Object, e As KeyEventArgs) Handles txtbxlog.KeyDown
        If (e.KeyCode = Keys.A And e.Control And Not txtbxlog.SelectionLength >= txtbxlog.Text.Length) Then
            e.SuppressKeyPress = True
            txtbxlog.SelectAll()
            e.Handled = True
            'ElseIf e.KeyCode = Keys.A And e.Control Then
            '    e.SuppressKeyPress = True
            '    e.Handled = True
        End If
    End Sub

    Private Sub chkbxenter_CheckedChanged(sender As Object, e As EventArgs) Handles chkbxenter.CheckedChanged
        If txtbxcmd.Text.Length = 0 Then
            allowenter = chkbxenter.Checked
        Else
            chkbxenter.Checked = allowenter
        End If
    End Sub

    Private Sub butabout_Click(sender As Object, e As EventArgs) Handles butabout.Click
        contrvis(False)
        aboutbx_showing = True
        aboutbx.ShowDialog()
        aboutbx_showing = False
        contrvis(True)
    End Sub

    Private Sub contrvis(vis As Boolean)
        txtbxcmd.Enabled = vis
        txtbxlog.Enabled = vis
        chkbxenter.Enabled = vis
        butabout.Enabled = vis
        butenter.Enabled = vis
        butexit.Enabled = vis
        butmcmdarr.Enabled = vis
        butreset.Enabled = vis
        butstop.Enabled = vis
    End Sub

    Private Sub contrcmdvis(vis As Boolean)
        txtbxcmd.Enabled = vis
        txtbxlog.Enabled = True
        chkbxenter.Enabled = vis
        butabout.Enabled = vis
        butenter.Enabled = vis
        butexit.Enabled = vis
        butmcmdarr.Enabled = vis
        butreset.Enabled = vis
    End Sub

    Private Sub chkchkbxtsub()
        While prrun
threadstart1:
            Try
                Try
                    callonform(Sub()
                                   If txtbxcmd.Text.Length = 0 And Not aboutbx_showing And Not disablechkbx Then
                                       If chkbxenter.Enabled = False Then
                                           chkbxenter.Enabled = True
                                       End If
                                   Else
                                       If chkbxenter.Enabled = True Then
                                           chkbxenter.Enabled = False
                                       End If
                                   End If
                               End Sub)
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
                Thread.Sleep(50)
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                GoTo threadstart1
            End Try
        End While
    End Sub

    Private Sub flags_sub()
        While prrun
threadstart2:
            Try
                Try
                    callonform(Sub()
                                   If tocleartxt Then
                                       txtbxlog.Clear()
                                       tocleartxt = False
                                   End If
                                   If movetobottom Then
                                       cmdpos = gui_pos_edit(cmd_gui_position.bottom)
                                       movetobottom = False
                                   End If
                                   If movetotop Then
                                       cmdpos = gui_pos_edit(cmd_gui_position.top)
                                       movetotop = False
                                   End If
                                   If quit Then
                                       contrvis(False)
                                       Me.Close()
                                       quit = False
                                   End If
                                   If restart Then
                                       contrvis(False)
                                       If restart_have_args Then
                                           If restart_custom Then
                                               Dim cargstoformat As String = convertcmdlinetocmdlinestr(restart_custom_args, 0)
                                               If restart_admin Then
                                                   Process.Start(New ProcessStartInfo(assemblypath, cargstoformat) With {.Verb = "runas"})
                                               Else
                                                   Process.Start(assemblypath, cargstoformat)
                                               End If
                                           Else
                                               If restart_admin Then
                                                   Process.Start(New ProcessStartInfo(assemblypath, convertcmdlinetocmdlinestr(Environment.GetCommandLineArgs(), 1)) With {.Verb = "runas"})
                                               Else
                                                   Process.Start(assemblypath, convertcmdlinetocmdlinestr(Environment.GetCommandLineArgs(), 1))
                                               End If
                                           End If
                                       Else
                                           If restart_admin Then
                                               Process.Start(New ProcessStartInfo(assemblypath) With {.Verb = "runas"})
                                           Else
                                               Process.Start(assemblypath)
                                           End If
                                       End If
                                       Me.Close()
                                       restart = False
                                       restart_admin = False
                                   End If
                                   If tochangeenter Then
                                       contrvis(False)
                                       allowenter = changeenterto
                                       chkbxenter.Checked = allowenter
                                       contrvis(True)
                                       tochangeenter = False
                                   End If
                                   If toappendtext Then
                                       'txtbxlog.AppendText(appendtext)
                                       render_outtxt(txtbxlog, appendtext)
                                       If loged Then
                                           log = log & appendtext
                                       End If
                                       toappendtext = False
                                       appendtext = ""
                                   End If
                               End Sub)
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                    If restart_admin Then
                        restart_admin = False
                    End If
                End Try
                Thread.Sleep(50)
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                GoTo threadstart2
            End Try
        End While
    End Sub

    Private Sub flagsabx_sub()
        While prrun
threadstart4:
            Try
                Try
                    callonform(Sub()
                                   If showabout Then
                                       contrvis(False)
                                       aboutbx_showing = True
                                       aboutbx.ShowDialog()
                                       aboutbx_showing = False
                                       contrvis(True)
                                       showabout = False
                                   End If
                               End Sub)
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
                Thread.Sleep(50)
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                GoTo threadstart4
            End Try
        End While
    End Sub

    Private Sub flagdel_sub()
        While prrun
threadstart5:
            Try
                Try
                    If rundelegate Then
                        If Not deltorun Is Nothing Then
                            callonform(deltorun)
                            deltorun = Nothing
                        End If
                        rundelegate = False
                    End If
                    If ui_queue.Count > 0 Then
                        Dim cdel As [Delegate] = ui_queue.Dequeue()
                        callonform(cdel)
                    End If
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
                Thread.Sleep(50)
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                GoTo threadstart5
            End Try
        End While
    End Sub

    Private Sub main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        disablechkbx = True
        contrvis(False)
        If shutdown_hook_ran And Not hook_running Then
            prrun = False
            If checkchkbxthread.IsAlive Then
                checkchkbxthread.Abort()
                Try
                    checkchkbxthread.Join(10000)
                Catch ex As ThreadStateException
                End Try
            End If
            If flags_thread.IsAlive Then
                flags_thread.Abort()
                Try
                    flags_thread.Join(10000)
                Catch ex As ThreadStateException
                End Try
            End If
            If flags_threadabx.IsAlive Then
                flags_threadabx.Abort()
                Try
                    flags_threadabx.Join(10000)
                Catch ex As ThreadStateException
                End Try
            End If
            If flags_threaddel.IsAlive Then
                flags_threaddel.Abort()
                Try
                    flags_threaddel.Join(10000)
                Catch ex As ThreadStateException
                End Try
            End If
            If command_thread.IsAlive Then
                command_thread.Abort()
                Try
                    command_thread.Join(10000)
                Catch ex As ThreadStateException
                End Try
            End If
            If log <> "" Then
                savefile(logpath & "\calm_cmd-" & DateTime.Now.Hour & "-" & DateTime.Now.Minute & "-" & DateTime.Now.Second & "-" & DateTime.Now.Day & "-" & DateTime.Now.Month & "-" & DateTime.Now.Year & "-" & ".txt", log)
                log = ""
            End If
            e.Cancel = False
        ElseIf Not hook_running Then
            at_end = True
            hook_running = True
            hook_stop_t.Start()
            e.Cancel = True
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub butenter_Click(sender As Object, e As EventArgs) Handles butenter.Click
        contrcmdvis(False)
        cmd_inter(txtbxcmd.Text)
        contrcmdvis(True)
    End Sub

    Private Sub cmd_inter(icmd As String)
        Dim ccmd As String = icmd.Replace(ControlChars.Lf, ControlChars.CrLf)

        If command_buffer.Count <> 0 Then
            command_buffer.Insert(command_buffer_index, icmd)
            command_buffer_index += 1
        Else
            command_buffer.Add(icmd)
            command_buffer_index = 1
        End If
        If command_buffer.Count > command_buffer_limit And command_buffer_limit > 0 Then
            command_buffer.RemoveAt(0)
        End If
        If command_buffer_index > command_buffer_limit Then
            command_buffer_index = command_buffer_limit
        End If
        If command_buffer_index > command_buffer.Count Then
            command_buffer_index = command_buffer.Count
        End If
        If command_buffer_index <= 0 Then
            command_buffer_index = 1
        End If

        If ccmd <> "" Then
                Dim ccom As String = ""
                Dim cuchar As String = ""
                Dim lchar As String = ""
                Dim comlist As New List(Of String)
                For i As Integer = 0 To cCmd.Length - 1 Step 1
                    cuchar = cCmd.Substring(i, 1)
                    If (cuchar = Chr(10) And lchar = Chr(13)) Then
                        comlist.Add(ccom)
                        ccom = ""
                    ElseIf i = ccmd.Length - 1 Then
                        If cuchar <> Chr(10) And cuchar <> Chr(13) Then
                            ccom = ccom & cuchar
                        End If
                        comlist.Add(ccom)
                        ccom = ""
                    Else
                        If cuchar <> Chr(10) And cuchar <> Chr(13) Then
                            ccom = ccom & cuchar
                        End If
                    End If
                    lchar = cuchar
                Next
                Dim l_stack As New Stack(Of String)
                For Each curcom As String In comlist
                    If curcom <> "" Then
                        l_stack.Push(curcom)
                    End If
                Next
                For i As Integer = 1 To l_stack.Count Step 1
                    Dim comcmd As String = l_stack.Pop()
                    command_stack.Push(comcmd)
                Next
            End If
        txtbxcmd.Text = ""
    End Sub

    Private Sub command_sub()
        While prrun
threadstart3:
            Try
                Try
                    callonform(Sub()
                                   If command_stack.Count > 0 And commands_init And butstop.Enabled = False Then
                                       butstop.Enabled = True
                                   ElseIf butstop.Enabled And command_stack.Count < 1 Then
                                       butstop.Enabled = False
                                   End If
                                   If command_stack.Count > 0 And commands_init And pgrsbarstatus.Style <> ProgressBarStyle.Marquee Then
                                       pgrsbarstatus.Style = ProgressBarStyle.Marquee
                                   ElseIf pgrsbarstatus.Style = ProgressBarStyle.Marquee And command_stack.Count < 1 Then
                                       pgrsbarstatus.Style = ProgressBarStyle.Continuous
                                   End If
                                   If command_stack.Count > 0 And commands_init And lblstatus.Text = "" Then
                                       lblstatus.Text = "Executing Commands: " & command_stack.Count & " Commands Left..."
                                   ElseIf lblstatus.Text.StartsWith("Executing Commands:") And command_stack.Count < 1 Then
                                       lblstatus.Text = ""
                                   End If
                               End Sub)
                    While command_stack.Count > 0 And commands_init
                        Dim curcom As String = command_stack.Pop()
                        Dim retfromruncmd As OutputText = run_cmd(curcom)
                        If retfromruncmd <> "" Then
                            If loged Then
                                log = log & retfromruncmd & ControlChars.CrLf
                            End If
                            callonform(Sub()
                                           'txtbxlog.AppendText(retfromruncmd & ControlChars.CrLf)
                                           render_outtxt(txtbxlog, retfromruncmd & ControlChars.CrLf)
                                           lblstatus.Text = "Executing Commands: " & command_stack.Count & " Commands Left..."
                                       End Sub)
                        End If
                        If cancel_action Then
                            cancel_action = False
                            command_stack.Clear()
                        End If
                        Thread.Sleep(25)
                    End While
                    If log.Length > 999999 And loged Then
                        If savefile(logpath & "\calm_cmd-" & DateTime.Now.Hour & "-" & DateTime.Now.Minute & "-" & DateTime.Now.Second & "-" & DateTime.Now.Day & "-" & DateTime.Now.Month & "-" & DateTime.Now.Year & "-" & ".txt", log) Then
                            log = ""
                        Else
                            callonform(Sub() MsgBox("Error Saving Log To: " & logpath & ".", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "calm_cmd Error"))
                        End If
                    End If
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
                Thread.Sleep(50)
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                GoTo threadstart3
            End Try
        End While
    End Sub

    Private Sub main_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        s_shown.Start()
    End Sub

    Private Sub main_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
    End Sub

    Private Sub butstop_Click(sender As Object, e As EventArgs) Handles butstop.Click
        butstop.Enabled = False
        If ll Then
            If Not lib_load_t Is Nothing Then
                If lib_load_t.IsAlive Then
                    lib_load_t.Abort()
                    Try
                        lib_load_t.Join(10000)
                    Catch ex As ThreadStateException
                    End Try
                End If
            End If
        ElseIf hook_running Then
            If at_end Then
                If Not hook_stop_t Is Nothing Then
                    If hook_stop_t.IsAlive Then
                        cancel_action = True
                        hook_stop_t.Abort()
                        Try
                            hook_stop_t.Join(10000)
                        Catch ex As ThreadStateException
                        End Try
                        hook_running = False
                        shutdown_hook_ran = True
                        Me.Close()
                    End If
                End If
            Else
                If Not hook_start_t Is Nothing Then
                    If hook_start_t.IsAlive Then
                        cancel_action = True
                        hook_start_t.Abort()
                        Try
                            hook_start_t.Join(10000)
                        Catch ex As ThreadStateException
                        End Try
                        hook_running = False
                    End If
                End If
            End If
        Else
            cancel_action = True
        End If
    End Sub

    Private Sub UndoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UndoToolStripMenuItem.Click
        Dim casted As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim cms As ContextMenuStrip = casted.GetCurrentParent()
        If cms.SourceControl.Name = txtbxcmd.Name Then
            If txtbxcmd.CanUndo Then
                txtbxcmd.Undo()
            End If
        End If
    End Sub

    Private Sub ContextMenuStripRtb_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStripRtb.Opening
        Dim cms As ContextMenuStrip = CType(sender, ContextMenuStrip)
        If cms.SourceControl.Name = txtbxcmd.Name Then
            If txtbxcmd.SelectedText <> "" Then
                CutToolStripMenuItem.Enabled = True
                CopyToolStripMenuItem.Enabled = True
            Else
                CutToolStripMenuItem.Enabled = False
                CopyToolStripMenuItem.Enabled = False
            End If
            If Clipboard.ContainsText() Then
                PasteToolStripMenuItem.Enabled = True
            Else
                PasteToolStripMenuItem.Enabled = False
            End If
            If txtbxcmd.CanUndo Then
                UndoToolStripMenuItem.Enabled = True
            Else
                UndoToolStripMenuItem.Enabled = False
            End If
        Else
            UndoToolStripMenuItem.Enabled = False
            CutToolStripMenuItem.Enabled = False
            PasteToolStripMenuItem.Enabled = False
            If txtbxlog.SelectedText <> "" Then
                CopyToolStripMenuItem.Enabled = True
            Else
                CopyToolStripMenuItem.Enabled = False
            End If
        End If
    End Sub

    Private Sub CutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CutToolStripMenuItem.Click
        Dim casted As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim cms As ContextMenuStrip = casted.GetCurrentParent()
        If cms.SourceControl.Name = txtbxcmd.Name And txtbxcmd.SelectedText <> "" Then
            Clipboard.SetText(txtbxcmd.SelectedText.Replace(ControlChars.Lf, ControlChars.CrLf), TextDataFormat.UnicodeText)
            'Clipboard.SetText(txtbxcmd.SelectedText.Replace(ControlChars.Lf, ControlChars.CrLf))
            txtbxcmd.Text = txtbxcmd.Text.Remove(txtbxcmd.SelectionStart, txtbxcmd.SelectionLength)
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Dim casted As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim cms As ContextMenuStrip = casted.GetCurrentParent()
        If cms.SourceControl.Name = txtbxcmd.Name And txtbxcmd.SelectedText <> "" Then
            Clipboard.SetText(txtbxcmd.SelectedText.Replace(ControlChars.Lf, ControlChars.CrLf), TextDataFormat.UnicodeText)
            'Clipboard.SetText(txtbxcmd.SelectedText.Replace(ControlChars.Lf, ControlChars.CrLf))
        ElseIf cms.SourceControl.Name = txtbxlog.Name And txtbxlog.SelectedText <> "" Then
            Clipboard.SetText(txtbxlog.SelectedText.Replace(ControlChars.Lf, ControlChars.CrLf), TextDataFormat.UnicodeText)
        End If
    End Sub

    Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripMenuItem.Click
        Dim casted As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim cms As ContextMenuStrip = casted.GetCurrentParent()
        If cms.SourceControl.Name = txtbxcmd.Name Then
            If Clipboard.ContainsText() Then
                Dim sinx As Integer = txtbxcmd.SelectionStart
                If txtbxcmd.SelectedText <> "" Then
                    txtbxcmd.Text = txtbxcmd.Text.Remove(sinx, txtbxcmd.SelectionLength)
                End If
                Dim clip As String = Clipboard.GetText(TextDataFormat.UnicodeText)
                txtbxcmd.Text = txtbxcmd.Text.Insert(sinx, clip)
                txtbxcmd.SelectionStart = sinx + clip.Length
            End If
        End If
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        Dim casted As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim cms As ContextMenuStrip = casted.GetCurrentParent()
        If cms.SourceControl.Name = txtbxcmd.Name Then
            txtbxcmd.SelectAll()
        ElseIf cms.SourceControl.Name = txtbxlog.Name Then
            txtbxlog.SelectAll()
        End If
    End Sub
End Class

Public Enum cmd_gui_position As Integer
    top = 0
    bottom = 1
End Enum
