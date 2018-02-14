Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.InteropServices

Module utils
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

    Public Function converintegertoboolean(int As Integer) As Boolean
        If (int = 1) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function converbooleantointeger(bool As Boolean) As Integer
        If (bool) Then
            Return 1
        Else
            Return 0
        End If
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

    Private KeyLengthBits As Integer = 256

    Private SaltLength As Integer = 8

    Private IterationCount As Integer = 2000

    Private rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()

    Public Function DecryptString(ciphertext As String, passphrase As String) As String
        Try
            Dim expr_11 As String() = ciphertext.Split(":".ToCharArray(), 3)
            Dim iv As Byte() = Convert.FromBase64String(expr_11(0))
            Dim salt As Byte() = Convert.FromBase64String(expr_11(1))
            Dim arg_35_0 As Byte() = Convert.FromBase64String(expr_11(2))
            Dim key As Byte() = DeriveKeyFromPassphrase(passphrase, salt)
            Dim bytes As Byte() = DoCryptoOperation(arg_35_0, key, iv, False)
            Return Encoding.UTF8.GetString(bytes)
        Catch ex As Exception

        End Try
        Return ""
    End Function

    Public Function EncryptString(plaintext As String, passphrase As String) As String
        Try
            Dim array As Byte() = GenerateRandomBytes(SaltLength)
            Dim array2 As Byte() = GenerateRandomBytes(16)
            Dim key As Byte() = DeriveKeyFromPassphrase(passphrase, array)
            Dim inArray As Byte() = DoCryptoOperation(Encoding.UTF8.GetBytes(plaintext), key, array2, True)
            Return String.Format("{0}:{1}:{2}", Convert.ToBase64String(array2), Convert.ToBase64String(array), Convert.ToBase64String(inArray))
        Catch ex As Exception

        End Try
        Return ""
    End Function

    Private Function DeriveKeyFromPassphrase(passphrase As String, salt As Byte()) As Byte()
        Return New Rfc2898DeriveBytes(passphrase, salt, IterationCount).GetBytes(KeyLengthBits / 8)
    End Function

    Private Function GenerateRandomBytes(lengthBytes As Integer) As Byte()
        Dim array As Byte() = New Byte(lengthBytes - 1) {}
        rng.GetBytes(array)
        Return array
    End Function

    Private Function DoCryptoOperation(inputData As Byte(), key As Byte(), iv As Byte(), encrypt As Boolean) As Byte()
        Dim result As Byte()
        Using aesCryptoServiceProvider As AesCryptoServiceProvider = New AesCryptoServiceProvider()
            Using memoryStream As MemoryStream = New MemoryStream()
                Dim transform As ICryptoTransform = If(encrypt, aesCryptoServiceProvider.CreateEncryptor(key, iv), aesCryptoServiceProvider.CreateDecryptor(key, iv))
                Try
                    Using cryptoStream As CryptoStream = New CryptoStream(memoryStream, transform, CryptoStreamMode.Write)
                        cryptoStream.Write(inputData, 0, inputData.Length)
                    End Using
                    result = memoryStream.ToArray()
                Catch ex_5B As Exception
                    result = New Byte(-1) {}
                End Try
            End Using
        End Using
        Return result
    End Function

    Public Declare Function SetForegroundWindow Lib "user32.dll" (hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    Public Declare Function SetActiveWindow Lib "user32.dll" (hWnd As IntPtr) As IntPtr
    Public Declare Function FindWindow Lib "user32.dll" (lpClassName As String, lpWindowName As String) As IntPtr
    Public Declare Function GetActiveWindow Lib "user32.dll" () As IntPtr
End Module
