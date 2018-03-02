Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.InteropServices

Module utils
    Public Function convertstringtobool(str As String) As Boolean
        Dim ret As Boolean = False
        Try
            ret = Boolean.Parse(str)
        Catch ex As InvalidCastException
            ret = False
        Catch ex As FormatException
            ret = False
        End Try
        Return ret
    End Function

    Public Function convertstringtoint(str As String) As Integer
        Dim ret As Integer = 0
        Try
            ret = Integer.Parse(str)
        Catch ex As InvalidCastException
            ret = 0
        Catch ex As FormatException
            ret = 0
        End Try
        Return ret
    End Function

    Public Function numberofindexes(args As String()) As Integer
        If args Is Nothing Then
            Return 0
        Else
            Return args.Length
        End If
    End Function

    Public Function loadfile(filepath As String) As String
        Try
            Dim filecontents As String = ""
            Using StreamReader As New StreamReader(filepath)
                filecontents = StreamReader.ReadToEnd()
            End Using
            Return filecontents
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function loadfilelines(filepath As String) As String()
        Try
            Dim filecontents As String()
            filecontents = File.ReadAllLines(filepath)
            Return filecontents
        Catch ex As Exception
            Dim ret(0) As String
            ret(0) = ""
            Return ret
        End Try
    End Function

    Public Function savefile(filepath As String, contents As String) As Boolean
        Try
            Using Streamwriter As New StreamWriter(filepath)
                Streamwriter.Write(contents)
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function convertobjecttostring(obj As Object) As String
        Try
            Dim memorysteam As New MemoryStream
            Dim formatter As New BinaryFormatter()
            formatter.Serialize(memorysteam, obj)
            Dim toreturn As String = Convert.ToBase64String(memorysteam.ToArray)
            formatter = Nothing
            memorysteam.Dispose()
            memorysteam = Nothing
            Return toreturn
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function convertstringtoobject(str As String) As Object
        Try
            Dim memorysteam As MemoryStream = New MemoryStream(Convert.FromBase64String(str))
            Dim formatter As BinaryFormatter = New BinaryFormatter()
            Dim retobj As Object = formatter.Deserialize(memorysteam)
            formatter = Nothing
            memorysteam.Dispose()
            memorysteam = Nothing
            Return retobj
        Catch ex As Exception
            Return New Object
        End Try
    End Function

    Public Declare Function Beep Lib "kernel32.dll" (dwFreq As Int32, dwDuration As Int32) As <MarshalAs(UnmanagedType.Bool)> Boolean
End Module
