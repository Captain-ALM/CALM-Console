Imports captainalm.calmcon.api
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Public Class main
    Public messages As New Dictionary(Of String, String)
    Public shown As Boolean = False
    Public tboxout As Windows.Forms.TextBoxBase = Nothing
    Public wouthook As WriteOutputHook = Nothing
    <SetupMethod>
    Public Function library() As LibrarySetup
        Dim hi As New HookInfo("special_msgs:0", Nothing, Nothing, New PreCommandExecuteHook(AddressOf todays_message), Nothing, Nothing, New OutputTextBoxHook(AddressOf oh_old), Nothing, New GetWriteOutputHook(AddressOf oh))
        Dim coms(5) As Command
        coms(0) = New Command("get_message", New Cmd(AddressOf get_msg), "get_message%(string)[optional : date]% : gets the current message")
        coms(1) = New Command("set_message", New Cmd(AddressOf set_msg), "set_message%(string)[message]%%(string)[optional : date]% : sets the current/given message")
        coms(2) = New Command("reset_message", New Cmd(AddressOf reset_shown), "reset_message : resets the current message showing")
        coms(3) = New Command("load_messages", New Cmd(AddressOf load_msg), "load_messages%(string)[path]% : loads the a set of messages")
        coms(4) = New Command("save_messages", New Cmd(AddressOf save_msg), "save_messages%(string)[path]% : saves the current set of messages")
        coms(5) = New Command("current_date", New Cmd(AddressOf current_date), "current_date : gets the current date")
        Return New LibrarySetup("special_messages", 0, hi, coms)
    End Function

    Public Function reset_shown(ByVal args As String()) As String
        shown = False
        Return "Shown Reset."
    End Function

    Public Function save_msg(ByVal args As String()) As String
        If args.Length >= 1 Then
            File.WriteAllText(args(0), convertobjecttostring(messages))
            Return "Saved Messages to: " & args(0)
        Else
            Return "Need Parameters."
        End If
    End Function

    Public Function load_msg(ByVal args As String()) As String
        If args.Length >= 1 Then
            messages = convertstringtoobject(File.ReadAllText(args(0)))
            If messages Is Nothing Then messages = New Dictionary(Of String, String)
            Return "Loaded Messages from: " & args(0)
        Else
            Return "Need Parameters."
        End If
    End Function

    Public Function get_msg(ByVal args As String()) As String
        Dim today As String = Date.Now.Date.ToShortDateString
        If args.Length >= 1 Then
            If messages.ContainsKey(args(0)) Then
                Return messages(args(0))
            Else
                Return "No Messages."
            End If
        Else
            If messages.ContainsKey(today) Then
                Return messages(today)
            Else
                Return "No Messages."
            End If
        End If
    End Function

    Public Function set_msg(ByVal args As String()) As String
        Dim today As String = Date.Now.Date.ToShortDateString
        If args.Length = 1 Then
            If messages.ContainsKey(today) Then
                messages(today) = args(0)
            Else
                messages.Add(today, args(0))
            End If
            Return "Message Added to " & today & " : " & args(0)
        ElseIf args.Length >= 2 Then
            If messages.ContainsKey(args(1)) Then
                messages(args(1)) = args(0)
            Else
                messages.Add(args(1), args(0))
            End If
            Return "Message Added to " & args(1) & " : " & args(0)
        Else
            Return "Need Parameters."
        End If
    End Function

    Public Function current_date(ByVal args As String()) As String
        Return Date.Now.Date.ToShortDateString
    End Function

    Public Sub oh(ByRef hook As WriteOutputHook)
        wouthook = hook
    End Sub

    Public Sub oh_old(ByRef t As Windows.Forms.TextBoxBase)
        tboxout = t
    End Sub

    Public Sub todays_message(command As String)
        If Not shown Then
            Dim today As String = Date.Now.Date.ToShortDateString
            If messages.ContainsKey(today) Then
                If wouthook Is Nothing Then
                    tboxout.Invoke(Sub() tboxout.AppendText(messages(today) & ControlChars.CrLf))
                Else
                    wouthook.Invoke(messages(today) & ControlChars.CrLf)
                End If
            End If
            shown = True
        End If
    End Sub

    Public Function convertobjecttostring(obj As Object) As String
        Using MemoryStream As New MemoryStream
            Try
                Dim formatter As New BinaryFormatter()
                formatter.Serialize(MemoryStream, obj)
                Dim toreturn As String = Convert.ToBase64String(MemoryStream.ToArray)
                formatter = Nothing
                Return toreturn
            Catch ex As Exception
                Return ""
            End Try
        End Using
    End Function

    Public Function convertstringtoobject(str As String) As Object
        Try
            Using MemoryStream As MemoryStream = New MemoryStream(Convert.FromBase64String(str))
                Dim formatter As BinaryFormatter = New BinaryFormatter()
                Dim retobj As Object = formatter.Deserialize(MemoryStream)
                formatter = Nothing
                Return retobj
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
