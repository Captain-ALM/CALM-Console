Imports System.Threading
Imports captainalm.calmcon.api

Public Module cmdpr
    Public commands As New Dictionary(Of String, executable_command)
    Public syntaxes As New Dictionary(Of String, ISyntax)

    Public Function run_cmd(cmd As String) As OutputText
        Try
            Dim com As int_command = New int_command(cmd, SyntaxMode, False)
            For Each ihook As HookInfo In hooks_info.Values
                Try
                    If Not ihook.hook_command_preexecute Is Nothing Then
                        ihook.hook_command_preexecute.Invoke(cmd)
                    End If
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
            Next
            com.init()
            Dim toret As OutputText = Nothing
            If echo_command Then
                toret = cmd & ControlChars.CrLf & com.execute()
            Else
                toret = com.execute()
            End If
            For Each ihook As HookInfo In hooks_info.Values
                Try
                    If Not ihook.hook_command_postexecute Is Nothing Then
                        ihook.hook_command_postexecute.Invoke(cmd, toret)
                    End If
                Catch ex As ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
            Next
            Return toret
        Catch ex As ThreadAbortException
            Throw ex
        Catch ex As Exception
            If echo_command Then
                Return cmd & ControlChars.CrLf
            Else
                Return ""
            End If
        End Try
    End Function

    Public Sub init_snx()
        syntaxes.Add("calm_console_default_type", New calmconsole_type)
        SyntaxMode = "calm_console_default_type"
    End Sub

    Public Sub init_cmd()
        'help
        commands.Add("help", New executable_command("help", New Cmd(AddressOf help)))
        commandhelplst.Add("help%*(string)[optional command name]% : provides help for commands.")
        'help_ext / help_extended
        commands.Add("help_ext", New executable_command("help_ext", New Cmd(AddressOf helpext)))
        commands.Add("help_extended", New executable_command("help_extended", New Cmd(AddressOf helpext)))
        commandhelplst.Add("help_ext/help_extened%*(string)[optional command name]% : provides help for commands, including '.' commands.")
        'invalid
        commands.Add("invalid", New executable_command("invalid", New Cmd(AddressOf invalid)))
        commandhelplst.Add("invalid : provides an invalid command error message.")
        'str / string
        commands.Add("str", New executable_command("str", New Cmd(AddressOf str)))
        commands.Add("string", New executable_command("string", New Cmd(AddressOf str)))
        commandhelplst.Add("str/string%string% : provides a converter to string for functions.")
        'int / integer
        commands.Add("int", New executable_command("int", New Cmd(AddressOf int)))
        commands.Add("integer", New executable_command("integer", New Cmd(AddressOf int)))
        commandhelplst.Add("int/integer%string% : provides a converter to integer for functions.")
        'dec / decimal
        commands.Add("dec", New executable_command("dec", New Cmd(AddressOf dec)))
        commands.Add("decimal", New executable_command("decimal", New Cmd(AddressOf dec)))
        commandhelplst.Add("dec/decimal%string% : provides a converter to decimal for functions.")
        'cls / clear
        commands.Add("cls", New executable_command("cls", New Cmd(AddressOf cls)))
        commands.Add("clear", New executable_command("clear", New Cmd(AddressOf cls)))
        commandhelplst.Add("cls/clear : clears the operation log.")
        'move_gui
        commands.Add("move_gui", New executable_command("move_gui", New Cmd(AddressOf move_gui)))
        commandhelplst.Add("move_gui%top/bottom/up/down% : allows the command area portion of the gui to move.")
        'exit / quit / end / close / terminate
        commands.Add("exit", New executable_command("exit", New Cmd(AddressOf quitp)))
        commands.Add("quit", New executable_command("quit", New Cmd(AddressOf quitp)))
        commands.Add("end", New executable_command("end", New Cmd(AddressOf quitp)))
        commands.Add("close", New executable_command("close", New Cmd(AddressOf quitp)))
        commands.Add("terminate", New executable_command("terminate", New Cmd(AddressOf quitp)))
        commandhelplst.Add("exit/quit/end/close/terminate : closes and then ends the program.")
        'reset / restart
        commands.Add("reset", New executable_command("reset", New Cmd(AddressOf restartp)))
        commands.Add("restart", New executable_command("restart", New Cmd(AddressOf restartp)))
        commandhelplst.Add("reset/restart%false[no arguments]/true[use current program arguments]/*(args)...[this + later arguments are each command line argument to be passed to the restarted program]% : restarts the program.")
        'about
        commands.Add("about", New executable_command("about", New Cmd(AddressOf aboutsh)))
        commandhelplst.Add("about : shows the about box.")
        'allow_enter
        commands.Add("allow_enter", New executable_command("allow_enter", New Cmd(AddressOf chenter)))
        commandhelplst.Add("allow_enter%true/false% : allow enter in the command box : if true multiline commands will run, false an enter will execute the current command.")
        'lang / language / syntax
        commands.Add("lang", New executable_command("lang", New Cmd(AddressOf changelang)))
        commands.Add("language", New executable_command("language", New Cmd(AddressOf changelang)))
        commands.Add("syntax", New executable_command("syntax", New Cmd(AddressOf changelang)))
        commandhelplst.Add("lang/language/syntax%syntax name% : switches the program's command language/syntax.")
        'lang / language / syntax
        commands.Add("langs", New executable_command("langs", New Cmd(AddressOf langs)))
        commands.Add("languages", New executable_command("languages", New Cmd(AddressOf langs)))
        commands.Add("syntaxes", New executable_command("syntaxes", New Cmd(AddressOf langs)))
        commandhelplst.Add("langs/languages/syntaxes : Gets a list of the program's command languages/syntaxes.")
        'char_conv / character_convert
        commands.Add("char_conv", New executable_command("char_conv", New Cmd(AddressOf charconv)))
        commands.Add("character_convert", New executable_command("character_convert", New Cmd(AddressOf charconv)))
        commandhelplst.Add("char_conv/character_convert%string(as a number)% : converts an integer to a character.")
        'run / execute
        commands.Add("run", New executable_command("run", New Cmd(AddressOf runfile)))
        commands.Add("execute", New executable_command("execute", New Cmd(AddressOf runfile)))
        commandhelplst.Add("run/execute%string(file path)% : executes a script from a file.")
        'log / logger
        commands.Add("log", New executable_command("log", New Cmd(AddressOf logger)))
        commands.Add("logger", New executable_command("logger", New Cmd(AddressOf logger)))
        commandhelplst.Add("log/logger%true/on/enabled/false/off/disabled/default(default folder for log)/dump/clear/*(folder for log)% : performs command for the log.")
        'set / set_var / set_variable
        commands.Add("set", New executable_command("set", New Cmd(AddressOf var_set)))
        commands.Add("set_var", New executable_command("set_var", New Cmd(AddressOf var_set)))
        commands.Add("set_variable", New executable_command("set_variable", New Cmd(AddressOf var_set)))
        commandhelplst.Add("set/set_var/set_variable%*(string)[name]%%*(any)[value]% : sets a value on the internal variable dictionary.")
        'get / get_var / get_variable
        commands.Add("get", New executable_command("get", New Cmd(AddressOf var_get)))
        commands.Add("get_var", New executable_command("get_var", New Cmd(AddressOf var_get)))
        commands.Add("get_variable", New executable_command("get_variable", New Cmd(AddressOf var_get)))
        commandhelplst.Add("get/get_var/get_variable%*(string)[name]% : returns a value from the internal variable dictionary.")
        'lib / libs / library / libraries
        commands.Add("lib", New executable_command("lib", New Cmd(AddressOf lib_man)))
        commands.Add("libs", New executable_command("libs", New Cmd(AddressOf lib_man)))
        commands.Add("library", New executable_command("library", New Cmd(AddressOf lib_man)))
        commands.Add("libraries", New executable_command("libraries", New Cmd(AddressOf lib_man)))
        commandhelplst.Add("lib/libs/library/libraries%list/add/remove%%*(string)[path]%%internal/*% : manages the loaded libraries, adding or removing libraries requires program restart.")
        'reset_as_admin / restart_as_admin
        commands.Add("reset_as_admin", New executable_command("reset_as_admin", New Cmd(AddressOf adminrestart)))
        commands.Add("restart_as_admin", New executable_command("restart_as_admin", New Cmd(AddressOf adminrestart)))
        commandhelplst.Add("reset_as_admin/restart_as_admin%false[no arguments]/true[use current program arguments]/*(args)...[this + later arguments are each command line argument to be passed to the restarted program]% : restarts the program as administrator.")
        'write
        commands.Add("write", New executable_command("write", New Cmd(AddressOf write)))
        commandhelplst.Add("write%*(string)[text]% : writes a line of text to the outputbox on the GUI.")
        'writeline /write_line
        commands.Add("writeline", New executable_command("writeline", New Cmd(AddressOf writeline)))
        commands.Add("write_line", New executable_command("write_line", New Cmd(AddressOf writeline)))
        commandhelplst.Add("writeline/write_line%*(string)[text]% : writes a line of text to the outputbox on the GUI and a new line character.")
        'beep / bell
        commands.Add("beep", New executable_command("beep", New Cmd(AddressOf commands_data.beep)))
        commands.Add("bell", New executable_command("bell", New Cmd(AddressOf commands_data.beep)))
        commandhelplst.Add("beep/bell%*(integer)[frequency, make it 0 to play system bell]%%*(integer)[duration in milliseconds, make it 0 to play the system bell]% : plays a tone out of the machine speaker or the system beep (bell) sound.")
        'echo / echo_mode
        commands.Add("echo", New executable_command("echo", New Cmd(AddressOf echo)))
        commands.Add("echo_mode", New executable_command("echo_mode", New Cmd(AddressOf echo)))
        commandhelplst.Add("echo/echo_mode%true/false[if echo is enabled]% : enables and disables command echoing.")
        'buffer / cmd_buffer / command_buffer
        commands.Add("buffer", New executable_command("buffer", New Cmd(AddressOf cmdbuffer)))
        commands.Add("cmd_buffer", New executable_command("cmd_buffer", New Cmd(AddressOf cmdbuffer)))
        commands.Add("command_buffer", New executable_command("command_buffer", New Cmd(AddressOf cmdbuffer)))
        commandhelplst.Add("buffer/cmd_buffer/command_buffer%limit/number/mode/clear%%*(integer/boolean/none)[limit and number : limit of the buffer, mode : the shortcut mode in the textbox, clear : clears the buffer] : manages the command buffer, alt+up/alt+down to cycle through the buffer.")
    End Sub
End Module

Public Class int_command
    Private name As String = ""
    Private cmdstr As String = ""
    Private arguments As New List(Of int_command)
    Private cmdtype As String = "calm_console_default_type"
    Private log As String = ""
    Private ecmd As executable_command
    Private args As String() = Nothing
    Public constructed_without_syntax As Boolean = False

    Public Sub New(cmdstrp As String, cmdtypearg As String, Optional initnow As Boolean = False)
        constructed_without_syntax = False
        cmdstr = cmdstrp
        cmdtype = cmdtypearg
        If initnow Then
            init()
        End If
    End Sub

    Public Sub New(cmd As String, _args As String())
        Try
            ecmd = commands(cmd)
            args = _args
        Catch ex As ThreadAbortException
            Throw ex
        Catch ex As Exception
            args = Nothing
            ecmd = commands("invalid")
        End Try
        constructed_without_syntax = True
    End Sub

    Public Sub init()
        If Not constructed_without_syntax Then
            Try
                If name = "" And cmdstr <> "" Then
                    Dim argumentsstr As List(Of String) = dcmd(cmdstr, cmdtype)
                    name = argumentsstr(0)
                    If argumentsstr.Count > 1 Then
                        If args Is Nothing Then
                            args = New String() {}
                        End If
                        ReDim Preserve args(argumentsstr.Count - 2)
                        For i As Integer = 1 To argumentsstr.Count - 1 Step 1
                            If ((argumentsstr(i).StartsWith(ControlChars.Quote) And argumentsstr(i).EndsWith(ControlChars.Quote)) Or (argumentsstr(i).StartsWith("'") And argumentsstr(i).EndsWith("'"))) Then
                                args(i - 1) = argumentsstr(i).Substring(1, argumentsstr(i).Length - 2)
                            Else
                                arguments.Add(New int_command(argumentsstr(i), cmdtype, True))
                                args(i - 1) = arguments(arguments.Count - 1).execute()
                            End If
                        Next
                    Else
                        args = Nothing
                    End If
                    If commands.ContainsKey(name) Then
                        ecmd = commands(name)
                    Else
                        ecmd = commands("invalid")
                    End If
                End If
            Catch ex As ThreadAbortException
                Throw ex
            Catch ex As Exception
                args = Nothing
                ecmd = commands("invalid")
            End Try
        Else
            Throw New InvalidOperationException("The command must be constructed with syntax in order to be inited.")
        End If
    End Sub

    Public Function execute() As OutputText
        Dim toret As OutputText = ecmd.execute_and_return(args)
        ecmd.flush()
        Return toret
    End Function

    Public Function dcmd(ByVal cmdstr As String, ByVal cmdtype As String) As List(Of String)
        Dim lst As New List(Of String)
        If Not syntaxes Is Nothing Then
            If syntaxes.ContainsKey(cmdtype) Then
                Dim sntx As ISyntax = syntaxes(cmdtype)
                If sntx.name = cmdtype Then
                    Return sntx.decrypt(cmdstr, commands.Keys.ToList())
                Else
                    lst.Add("invalid")
                End If
            Else
                lst.Add("invalid")
            End If
        Else
            lst.Add("invalid")
        End If
        Return lst
    End Function
End Class

Public Class executable_command
    Private _name As String = ""
    Private _intdel As [Delegate]
    Private _isthread As Boolean = False
    Private _thread As Thread
    Private _args As String() = Nothing
    Private _retur As OutputText = Nothing

    Sub New(name As String, passed_delegate As [Delegate], Optional isthreaded As Boolean = False)
        _name = name
        _intdel = passed_delegate
        _isthread = isthreaded
    End Sub

    Public ReadOnly Property name As String
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property is_threaded As Boolean
        Get
            Return _isthread
        End Get
    End Property

    Public Property arguments As String()
        Get
            Return _args
        End Get
        Set(value As String())
            _args = value
        End Set
    End Property

    Public ReadOnly Property returned_values As OutputText
        Get
            Return _retur
        End Get
    End Property

    Public Sub execute(Optional args As String() = Nothing)
        If Not args Is Nothing Then
            _args = args
        Else
            _args = New String() {}
        End If
        If _isthread Then
            If Not _thread Is Nothing Then
                If _thread.IsAlive = False Then
                    _thread = New Thread(New ThreadStart(AddressOf execdel))
                    _thread.IsBackground = True
                    _thread.Start()
                End If
            End If
        Else
            execdel()
        End If
    End Sub

    Public Function execute_and_return(Optional args As String() = Nothing) As OutputText
        Me.execute(args)
        If _isthread Then
            If Not _thread Is Nothing Then
                If _thread.IsAlive Then
                    _thread.Join()
                End If
            End If
        End If
        Return _retur
    End Function

    Public Sub thead_abort()
        If _isthread And Not Not _thread Is Nothing Then
            If _thread.IsAlive Then
                _thread.Abort()
            End If
        End If
    End Sub

    Public Sub flush_arguments()
        If Not _args Is Nothing Then
            _args = Nothing
        End If
    End Sub

    Public Sub flush_return()
        If Not _retur Is Nothing Then
            _retur = Nothing
        End If
    End Sub

    Public Sub flush()
        Me.flush_arguments()
        Me.flush_return()
    End Sub

    Private Sub execdel()
        Try
            Dim _dtr As Cmd = _intdel
            _retur = _dtr.Invoke(_args)
        Catch ex As ThreadAbortException
            _retur = "Thread Aborted!"
            If Not _isthread Then
                Throw ex
            End If
        Catch ex As Exception
            _retur = "Exception: " & ex.GetType.ToString & " : " & ex.Message
        End Try
    End Sub
End Class