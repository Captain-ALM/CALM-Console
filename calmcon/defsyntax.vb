Public Structure calmconsole_type
    Implements api.ISyntax

    Public Function decrypt(cmdstr As String, commands As List(Of String)) As List(Of String) Implements api.ISyntax.decrypt
        Dim commandlst As New List(Of String)
        Dim carg As String = ""
        Dim incommand As Boolean = False
        Dim inarg As Boolean = False
        Dim isescape As Boolean = False
        Dim escapestr As String = ""
        Dim custr As String = ""

        isescape = False
        For i As Integer = 0 To cmdstr.Length - 1 Step 1
            custr = cmdstr.Substring(i, 1)
            If Not isescape Then
                If custr = "," Then
                    If commandlst.Count = 0 Then
                        commandlst.Add(carg)
                    Else
                        If commandlst(0) = "str" Or commandlst(0) = "string" Or commandlst(0) = "int" Or commandlst(0) = "integer" Or commandlst(0) = "dec" Or commandlst(0) = "decimal" Then
                            commandlst.Add(Me.literalconvert(carg, True))
                        Else
                            commandlst.Add(Me.literalconvert(carg, False))
                        End If
                    End If
                    carg = ""
                ElseIf custr = "/" Then
                    isescape = True
                Else
                    carg = carg & custr
                End If
            Else
                carg = carg & custr
                isescape = False
            End If
        Next
        If carg <> "" Then
            If commandlst.Count = 0 Then
                commandlst.Add(carg)
            Else
                If commandlst(0) = "str" Or commandlst(0) = "string" Or commandlst(0) = "int" Or commandlst(0) = "integer" Or commandlst(0) = "dec" Or commandlst(0) = "decimal" Then
                    commandlst.Add(Me.literalconvert(carg, True))
                Else
                    commandlst.Add(Me.literalconvert(carg, False))
                End If
            End If
        End If

        Return commandlst
    End Function

    Public Function literalconvert(toconv As String, is_Converted As Boolean) As String
        Dim isinteger As Boolean = False
        Dim isdecimal As Boolean = False
        Dim isstring As Boolean = False

        Dim returned As New Object

        If ((toconv.StartsWith(ControlChars.Quote) And toconv.EndsWith(ControlChars.Quote)) Or (toconv.StartsWith("'") And toconv.EndsWith("'"))) Then
            If is_Converted Then
                isstring = False
                Return toconv
            Else
                isstring = True
                toconv = toconv.Substring(1, toconv.Length - 2)
            End If
        End If

            Try
                isinteger = Integer.TryParse(toconv, returned)
            Catch ex As Threading.ThreadAbortException
                Threading.Thread.CurrentThread.Abort()
            Catch ex As Exception
                isinteger = False
            End Try
            If Not isinteger Then
                Try
                    isdecimal = Decimal.TryParse(toconv, returned)
                Catch ex As Threading.ThreadAbortException
                    Threading.Thread.CurrentThread.Abort()
                Catch ex As Exception
                    isdecimal = False
                End Try
                If Not isdecimal Then
                    isstring = True
                Else
                    isstring = False
                End If
            Else
                isdecimal = False
            End If

        If isinteger Then
            Return "int,'" & toconv & "'"
        ElseIf isdecimal Then
            Return "dec,'" & toconv & "'"
        ElseIf isstring Then
            Return "str,'" & toconv.Replace("/", "//").Replace(",", "/,") & "'"
        End If
        Return toconv
    End Function

    Public Function name() As String Implements api.ISyntax.name
        Return "calm_console_default_type"
    End Function
End Structure