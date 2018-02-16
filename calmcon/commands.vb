Imports System.IO
Imports captainalm.calmcon.api

Module commands_data
    Public commandhelplst As New List(Of String)

    Public Function beep(args As String()) As String
        If Not args Is Nothing Then
            If args.Length >= 1 Then
                Dim freq As Integer = convertstringtoint(args(0))
                If args.Length = 1 Then
                    If freq >= 37 And freq <= 32767 Then
                        utils.Beep(freq, 200)
                    Else
                        System.Media.SystemSounds.Beep.Play()
                    End If
                ElseIf args.Length >= 2 Then
                    Dim length As Integer = convertstringtoint(args(1))
                    If freq >= 37 And freq <= 32767 And length > 0 Then
                        utils.Beep(freq, length)
                    Else
                        System.Media.SystemSounds.Beep.Play()
                    End If
                End If
            End If
        Else
            utils.Beep(800, 200)
        End If
        Return "Beep!"
    End Function

    Public Function write(args As String()) As String
        If Not args Is Nothing Then
            If args.Length >= 1 Then
                Return args(0)
            End If
        End If
        Return ""
    End Function

    Public Function writeline(args As String()) As String
        If Not args Is Nothing Then
            If args.Length >= 1 Then
                Return args(0) & ControlChars.CrLf
            End If
        End If
        Return ""
    End Function

    Public Function lib_man(args As String()) As String
        Dim ret As String = ""
        If Not args Is Nothing Then
            If args.Length >= 1 Then
                Select Case args(0).ToLower
                    Case Is = "list"
                        ret = "Library List:" & ControlChars.CrLf
                        For Each c As String In dll_paths
                            ret = ret & c & ControlChars.CrLf
                        Next
                        If dll_paths.Count = 1 Then
                            ret &= dll_paths.Count & " Library Loaded."
                        ElseIf dll_paths.Count = 0 Then
                            ret &= "No Libraries Loaded."
                        Else
                            ret &= dll_paths.Count & " Libraries Loaded."
                        End If
                    Case Is = "add"
                        If args.Length >= 2 Then
                            If args.Length >= 3 Then
                                If args(2).ToLower = "internal" Then
                                    If File.Exists(args(1)) And Path.GetExtension(args(1)) = ".dll" Then
                                        Try
                                            If Not Directory.Exists(assemblydir & "\lib") Then
                                                Directory.CreateDirectory(assemblydir & "\lib")
                                            End If
                                            File.Copy(args(1), assemblydir & "\lib\" & Path.GetFileName(args(1)))
                                            ret = "Added Library: " & args(1) & ", You must now restart the program."
                                        Catch ex As Threading.ThreadAbortException
                                            Throw ex
                                        Catch ex As Exception
                                            ret = "Library Addition Failed!"
                                        End Try
                                    Else
                                        ret = "You need to specify a valid dll path."
                                    End If
                                Else
                                    If File.Exists(args(1)) And Path.GetExtension(args(1)) = ".dll" Then
                                        dll_ext.Add(args(1))
                                        Try
                                            Dim str As String = convertobjecttostring(dll_ext)
                                            File.WriteAllText(assemblydir & "\extlibs.lst", str)
                                            ret = "Added Library: " & args(1) & ", You must now restart the program."
                                        Catch ex As Threading.ThreadAbortException
                                            Throw ex
                                        Catch ex As Exception
                                            ret = "Library Addition Failed!"
                                        End Try
                                    Else
                                        ret = "You need to specify a valid dll path."
                                    End If
                                End If
                            Else
                                If File.Exists(args(1)) And Path.GetExtension(args(1)) = ".dll" Then
                                    dll_ext.Add(args(1))
                                    Try
                                        Dim str As String = convertobjecttostring(dll_ext)
                                        File.WriteAllText(assemblydir & "\extlibs.lst", str)
                                        ret = "Added Library: " & args(1) & ", You must now restart the program."
                                    Catch ex As Threading.ThreadAbortException
                                        Throw ex
                                    Catch ex As Exception
                                        ret = "Library Addition Failed!"
                                    End Try
                                Else
                                    ret = "You need to specify a valid dll path."
                                End If
                            End If
                        Else
                            ret = "Add Library Error: You must specify a path."
                        End If
                    Case Is = "remove"
                        If args.Length >= 2 Then
                            If doeslibexist(args(1)) Then
                                If islibexternal(args(1)) Then
                                    dll_ext.Remove(args(1))
                                    Try
                                        Dim str As String = convertobjecttostring(dll_ext)
                                        File.WriteAllText(assemblydir & "\extlibs.lst", str)
                                        ret = "Removed Library: " & args(1) & ", You must now restart the program."
                                    Catch ex As Threading.ThreadAbortException
                                        Throw ex
                                    Catch ex As Exception
                                        ret = "Library Removal Failed!"
                                    End Try
                                Else
                                    Try
                                        File.Delete(args(1))
                                        ret = "Deleted Library: " & args(1) & ", You must now restart the program."
                                    Catch ex As Threading.ThreadAbortException
                                        Throw ex
                                    Catch ex As Exception
                                        ret = "Library Removal Failed!"
                                    End Try
                                End If
                            Else
                                ret = "The library path was not valid or the library is not loaded."
                            End If
                        Else
                            ret = "Remove Library Error: You must specify a path."
                        End If
                End Select
            End If
        End If
        Return ret
    End Function

    Public Function var_get(args As String()) As String
        If Not args Is Nothing Then
            Dim noa As Integer = numberofindexes(args)
            If noa >= 1 Then
                If var_dict.ContainsKey(args(0)) Then
                    Return var_dict(args(0))
                Else
                    Return ""
                End If
            End If
        End If
        Return "No variable name specified."
    End Function

    Public Function var_set(args As String()) As String
        If Not args Is Nothing Then
            Dim noa As Integer = numberofindexes(args)
            If noa >= 2 Then
                If var_dict.ContainsKey(args(0)) Then
                    var_dict(args(0)) = args(1)
                    Return args(1)
                Else
                    var_dict.Add(args(0), args(1))
                    Return args(1)
                End If
            End If
        End If
        Return "No variable name and/or value specified."
    End Function

    Public Function langs(args As String()) As String
        Dim retstr As String = "Languages/Syntaxes:" & ControlChars.CrLf
        Dim i As Integer = 0
        For Each str As String In syntaxes.Keys
            retstr = retstr & str
            If i <> syntaxes.Keys.Count - 1 Then
                retstr = retstr & ControlChars.CrLf
            End If
            i += 1
        Next
        Return retstr
    End Function

    Public Function help(args As String()) As String
        If args Is Nothing Then
            Dim helpstr As String = "Help:" & ControlChars.CrLf
            For i As Integer = 0 To commandhelplst.Count - 1 Step 1
                helpstr = helpstr & commandhelplst(i)
                If i <> commandhelplst.Count - 1 Then
                    helpstr = helpstr & ControlChars.CrLf
                End If
            Next
            Return helpstr
        Else
            Try
                Dim commandhelplst_local As List(Of String) = commandhelplst.FindAll(Function(x) x.Contains(args(0)))
                Dim helpstr As String = "Help:" & ControlChars.CrLf & "Help Returned For '" & args(0) & "'" & ControlChars.CrLf
                For i As Integer = 0 To commandhelplst_local.Count - 1 Step 1
                    helpstr = helpstr & commandhelplst_local(i)
                    If i <> commandhelplst_local.Count - 1 Then
                        helpstr = helpstr & ControlChars.CrLf
                    End If
                Next
                Return helpstr
            Catch ex As Exception
                Return "Command Error: " & ex.GetType.ToString & " : " & ex.Message
            End Try
        End If
    End Function

    Public Function invalid(args As String()) As String
        Return "Invalid Command"
    End Function

    Public Function str(args As String()) As String
        Return args(0)
    End Function

    Public Function int(args As String()) As String
        Try
            Return Integer.Parse(args(0))
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function dec(args As String()) As String
        Try
            Return Decimal.Parse(args(0))
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function cls(args As String()) As String
        tocleartxt = True
        Return ""
    End Function

    Public Function charconv(args As String()) As String
        Try
            Return ChrW(Integer.Parse(args(0)))
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function move_gui(args As String()) As String
        If args(0).ToString.ToLower = "top" Or args(0) = "0" Or args(0).ToString.ToLower = "up" Then
            movetotop = True
        ElseIf args(0).ToString.ToLower = "bottom" Or args(0) = "1" Or args(0).ToString.ToLower = "down" Then
            movetobottom = True
        End If
        Return "GUI Positioned: " & args(0)
    End Function

    Public Function quitp(args As String()) As String
        quit = True
        Return "Ending..."
    End Function

    Public Function adminrestart(args As String()) As String
        Dim numofargs As Integer = numberofindexes(args)
        If numofargs = 1 Then
            If args(0).ToString.ToLower = "true" Then
                restart_have_args = True
                restart_custom = False
                restart_admin = True
            ElseIf args(0).ToString.ToLower = "false" Then
                restart_have_args = False
                restart_custom = False
                restart_admin = True
            Else
                ReDim Preserve restart_custom_args(0)
                restart_custom_args(0) = args(0).ToString.Replace(ControlChars.Quote, "'")
                restart_custom = True
                restart_admin = True
            End If
        ElseIf numofargs >= 2 Then
            ReDim Preserve restart_custom_args(numofargs - 1)
            For i As Integer = 0 To numofargs - 1 Step 1
                restart_custom_args(i) = args(i).ToString.Replace(ControlChars.Quote, "'")
            Next
            restart_custom = True
            restart_admin = True
        End If
        restart_admin = True
        restart = True
        Return "Restarting As Admin..."
    End Function

    Public Function restartp(args As String()) As String
        Dim numofargs As Integer = numberofindexes(args)
        If numofargs = 1 Then
            If args(0).ToString.ToLower = "true" Then
                restart_have_args = True
                restart_custom = False
            ElseIf args(0).ToString.ToLower = "false" Then
                restart_have_args = False
                restart_custom = False
            Else
                ReDim Preserve restart_custom_args(0)
                restart_custom_args(0) = args(0).ToString.Replace(ControlChars.Quote, "'")
                restart_custom = True
            End If
        ElseIf numofargs >= 2 Then
            ReDim Preserve restart_custom_args(numofargs - 1)
            For i As Integer = 0 To numofargs - 1 Step 1
                restart_custom_args(i) = args(i).ToString.Replace(ControlChars.Quote, "'")
            Next
            restart_custom = True
        End If
        restart = True
        Return "Restarting..."
    End Function

    Public Function aboutsh(args As String()) As String
        showabout = True
        Return "About Shown."
    End Function

    Public Function chenter(args As String()) As String
        tochangeenter = True
        Dim lastch = changeenterto
        Try
            If args(0).ToString.ToLower = "true" Or args(0) = "1" Then
                changeenterto = True
            Else
                changeenterto = False
            End If
        Catch ex As Exception
            changeenterto = lastch
        End Try
        Return "Enter Mode Changed."
    End Function

    Public Function changelang(args As String()) As String
        Dim lastcmd As String = syntax_mode
        Dim numofargs As Integer = numberofindexes(args)
        Try
            If syntaxes.ContainsKey(args(0)) Then
                syntax_mode = args(0)
            Else
                Try
                    Dim i As Integer = Integer.Parse(args(0))
                    Dim col As Dictionary(Of String, ISyntax).KeyCollection.Enumerator = syntaxes.Keys.GetEnumerator
                    For j As Integer = 0 To i Step 1
                        col.MoveNext()
                    Next
                    syntax_mode = col.Current
                Catch ex As Exception
                    Throw ex
                End Try
            End If
        Catch ex As Exception
            syntax_mode = lastcmd
        End Try
        Return "Syntax Language Changed to: " & syntax_mode
    End Function

    Public Function runfile(args As String()) As String
        Dim numofargs As Integer = numberofindexes(args)
        If numofargs >= 1 Then
            Try
                If File.Exists(args(0)) Then
                    Dim data As String() = loadfilelines(args(0))
                    Dim l_stack As New Stack(Of String)
                    For Each comcmd As String In data
                        If comcmd <> "" Then
                            l_stack.Push(comcmd)
                        End If
                    Next
                    For i As Integer = 1 To l_stack.Count Step 1
                        Dim comcmd As String = l_stack.Pop()
                        command_stack.Push(comcmd)
                    Next
                Else
                    Return "File Does Not Exist: " & args(0) & "."
                End If
            Catch ex As Exception
            End Try
        End If
        Return ""
    End Function

    Public Function logger(args As String()) As String
        Dim numofargs As Integer = numberofindexes(args)
        If numofargs >= 1 Then
            Try
                If args(0).ToString.ToLower = "true" Or args(0) = "1" Or args(0).ToString.ToLower = "on" Or args(0).ToString.ToLower = "enabled" Then
                    loged = True
                ElseIf args(0).ToString.ToLower = "false" Or args(0) = "0" Or args(0).ToString.ToLower = "off" Or args(0).ToString.ToLower = "disabled" Then
                    loged = False
                ElseIf args(0).ToString.ToLower = "clear" Then
                    log = ""
                ElseIf args(0).ToString.ToLower = "dump" Then
                    If log <> "" Then
                        savefile(logpath & "\msg_cmd_cs-" & DateTime.Now.Hour & "-" & DateTime.Now.Minute & "-" & DateTime.Now.Second & "-" & DateTime.Now.Day & "-" & DateTime.Now.Month & "-" & DateTime.Now.Year & "-" & ".txt", log)
                        log = ""
                    End If
                ElseIf args(0).ToString.ToLower = "default" Then
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly.FullName)
                Else
                    If savefile(args(0) & "\test.txt", "test") Then
                        File.Delete(args(0) & "\test.txt")
                        logpath = args(0)
                    End If
                End If
            Catch ex As Exception
            End Try
        End If
        Return "Logger Settings Changed."
    End Function

    Public Function convertstringtoint(str As String) As Integer
        Dim ret As Integer = 0
        Try
            ret = Integer.Parse(str)
        Catch ex As InvalidCastException
            ret = 0
        End Try
        Return ret
    End Function

    Public Function convertobjectargstostringargs(args As String()) As String()
        Dim toret(0) As String
        If IsNothing(args) Then
            Return Nothing
        End If
        Try
            Dim numofi As Integer = numberofindexes(args)
            ReDim toret(numofi - 1)
            For i As Integer = 0 To numofi - 1 Step 1
                toret(i) = args(i).ToString
            Next
        Catch ex As Exception
            Return Nothing
        End Try
        Return toret
    End Function

    Public Function numberofindexes(args As String()) As Integer
        If args Is Nothing Then
            Return 0
        Else
            Return args.Length
        End If
        'old code
        'Try
        '    Dim i As Integer = 0
        '    Try
        '        While "True"
        '            Dim tmp As Object = args(i)
        '            tmp = Nothing
        '            i = i + 1
        '        End While
        '        Return 0
        '    Catch ex As Exception
        '        Return i
        '    End Try
        'Catch ex As Exception
        '    Return 0
        'End Try
    End Function
End Module
