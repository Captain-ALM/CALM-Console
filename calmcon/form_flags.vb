Imports System.Reflection
Imports System.IO
Imports captainalm.calmcon.api

Public Module form_flags
    'form shared
    Public thisassembly As Assembly = Assembly.GetEntryAssembly
    Public assemblyname As String = Path.GetFileNameWithoutExtension(thisassembly.Location)
    Public assemblypath As String = thisassembly.Location
    Public assemblydir As String = Path.GetDirectoryName(thisassembly.Location)
    Public command_stack As New Stack(Of String)
    Public var_dict As New Dictionary(Of String, String)
    Public log As String = ""
    Public loged As Boolean = False
    Public logpath As String = ""
    Public aboutbx_showing As Boolean = False
    Public cancel_action As Boolean = False
    Public disablechkbx As Boolean = False
    Public command_buffer As New List(Of String)
    '1 based
    Public command_buffer_index As Integer = 1
    Public command_buffer_limit As Integer = 40
    Public command_buffer_shortcuts_enabled As Boolean = True
    'form flags
    Public tocleartxt As Boolean = False
    Public movetobottom As Boolean = False
    Public movetotop As Boolean = False
    Public quit As Boolean = False
    Public restart As Boolean = False
    Public showabout As Boolean = False
    Public tochangeenter As Boolean = False
    Public changeenterto As Boolean = False
    Public toappendtext As Boolean = False
    Public appendtext As OutputText = ""
    Public rundelegate As Boolean = False
    Public deltorun As [Delegate] = Nothing
    Public restart_have_args As Boolean = True
    Public restart_custom As Boolean = False
    Public restart_custom_args As String()
    Public restart_admin As Boolean = False
    Public ui_queue As New Queue(Of [Delegate])
    Public form_instance As main
    Public echo_command As Boolean = False
    Public read_key As Boolean = False
    Public cmd_read_ed As Boolean = True
End Module
