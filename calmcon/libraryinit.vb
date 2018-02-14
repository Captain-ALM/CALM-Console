Imports System.Reflection
Imports captainalm.calmcon.api
Imports System.IO

Module libraryinit
    Public dll_ext As New List(Of String)
    Public dll_paths As New List(Of String)
    Public dlls As New Dictionary(Of String, Assembly)
    Public libraries As New Dictionary(Of String, internal_lib)
    Public library_info As New Dictionary(Of String, LibrarySetup)
    Public hooks_info As New Dictionary(Of String, HookInfo)

    Public Sub init_libraries()
        For Each nom As String In libraries.Keys
            Try
                Dim clib As internal_lib = libraries(nom)

                'get setup data
                Dim smethod As MethodInfo = clib.type.GetMethod("setup", BindingFlags.Public Or BindingFlags.Instance)
                Dim libstupd As LibrarySetup = smethod.Invoke(clib.instance, Nothing)
                'add lib
                If Not library_info.ContainsKey(nom) Then
                    library_info.Add(nom, libstupd)
                End If
                'add hook set
                If Not hooks_info.ContainsKey(libstupd.hook_info.name) Then
                    hooks_info.Add(libstupd.hook_info.name, libstupd.hook_info)
                End If
                'add syntaxes if any
                If Not libstupd.syntaxes Is Nothing Then
                    For Each s As ISyntax In libstupd.syntaxes
                        If Not syntaxes.ContainsKey(s.name) Then
                            syntaxes.Add(s.name, s)
                        End If
                    Next
                End If
                'add commands if any
                If Not libstupd.commands Is Nothing Then
                    For Each coms As command In libstupd.commands
                        If Not commands.ContainsKey(coms.name) Then
                            commands.Add(coms.name, New executable_command(coms.name, coms.command))
                            commandhelplst.Add(coms.help)
                        End If
                        commands.Add(libstupd.name & "." & coms.name, New executable_command(libstupd.name & "." & coms.name, coms.command))
                        commandhelplst.Add(libstupd.name & "." & coms.help)
                    Next
                End If
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Sub load_libraries()
        For Each nom As String In dlls.Keys
            Dim ass As Assembly = dlls(nom)
            For Each clib As Type In ass.GetTypes()
                Try
                    Dim clib_nom As String = clib.Name
                    Dim cintlib As New internal_lib(clib_nom, ass.CreateInstance(clib.FullName))
                    If Not libraries.ContainsKey(clib_nom) Then
                        libraries.Add(clib_nom, cintlib)
                    End If
                Catch ex As Threading.ThreadAbortException
                    Throw ex
                Catch ex As Exception
                End Try
            Next
        Next
    End Sub

    Public Sub load_dlls()
        For Each c_dll As String In dll_paths
            Try
                Dim bytes As Byte() = File.ReadAllBytes(c_dll)
                If dlls.ContainsKey(c_dll) Then
                    dlls(c_dll) = Assembly.Load(bytes)
                Else
                    dlls.Add(c_dll, Assembly.Load(bytes))
                End If
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Sub get_dlls()
        If Directory.Exists(assemblydir & "\libs") Then
            Try
                Dim arr As String() = Directory.GetFiles(assemblydir & "\libs")
                For Each c As String In arr
                    If Path.GetExtension(c) = ".dll" Then
                        dll_paths.Add(c)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        End If
        If Directory.Exists(assemblydir & "\lib") Then
            Try
                Dim arr As String() = Directory.GetFiles(assemblydir & "\lib")
                For Each c As String In arr
                    If Path.GetExtension(c) = ".dll" Then
                        dll_paths.Add(c)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        End If
        If Directory.Exists(assemblydir & "\libraries") Then
            Try
                Dim arr As String() = Directory.GetFiles(assemblydir & "\libraries")
                For Each c As String In arr
                    If Path.GetExtension(c) = ".dll" Then
                        dll_paths.Add(c)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        End If
        If Directory.Exists(assemblydir & "\library") Then
            Try
                Dim arr As String() = Directory.GetFiles(assemblydir & "\library")
                For Each c As String In arr
                    If Path.GetExtension(c) = ".dll" Then
                        dll_paths.Add(c)
                    End If
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        End If
        If File.Exists(assemblydir & "\extlibs.lst") Then
            Try
                Dim str As String = File.ReadAllText(assemblydir & "\extlibs.lst")
                Dim obj As Object = convertstringtoobject(str)
                Dim lst As List(Of String) = Nothing
                Try
                    lst = obj
                Catch ex As InvalidCastException
                    lst = New List(Of String)
                End Try
                dll_ext = lst
                For Each c As String In lst
                    dll_paths.Add(c)
                Next
            Catch ex As Threading.ThreadAbortException
                Throw ex
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Function doeslibexist(library As String) As Boolean
        For Each clib As String In dll_paths
            If clib = library Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function islibexternal(library As String) As Boolean
        For Each clib As String In dll_ext
            If clib = library Then
                Return True
            End If
        Next
        Return False
    End Function
End Module

Public Class internal_lib
    Public name As String = ""
    Public instance As Object = Nothing
    Public type As Type = Nothing

    Public Sub New(nom As String, ByRef _instance As Object)
        name = nom
        instance = _instance
        type = instance.GetType()
    End Sub

    Public Sub New(nom As String, ByRef _instance As Object, ByRef _type As Type)
        name = nom
        instance = _instance
        type = _type
    End Sub
End Class
