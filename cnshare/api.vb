﻿Public Module Types
    ''' <summary>
    ''' Any commands set in libraries need to use this as the delegate type.
    ''' </summary>
    ''' <param name="arr">The array of parameters for the command.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Delegate Function Cmd(ByVal arr As String()) As String
    ''' <summary>
    ''' This is the hook delegate type required to hook to the command stack.
    ''' </summary>
    ''' <param name="stack">The passed command stack instance.</param>
    ''' <remarks></remarks>
    Public Delegate Sub CommandStackHook(ByRef stack As Stack(Of String))
    ''' <summary>
    ''' This is the hook delegate type required to hook to the variable Dictionary.
    ''' </summary>
    ''' <param name="dictionary">The passed variable Dictionary instance.</param>
    ''' <remarks></remarks>
    Public Delegate Sub variableDictionaryHook(ByRef dictionary As Dictionary(Of String, String))
    ''' <summary>
    ''' This is the hook delegate used for the start and stop program hook events.
    ''' </summary>
    ''' <remarks></remarks>
    Public Delegate Sub ProgramEventHook()
    ''' <summary>
    ''' This is the hook used for the preexecute command hook event.
    ''' </summary>
    ''' <param name="commandstr">The command string being executed.</param>
    ''' <remarks></remarks>
    Public Delegate Sub PreCommandExecuteHook(ByVal commandstr As String)
    ''' <summary>
    ''' This is the hook used for the postexecute command hook event.
    ''' </summary>
    ''' <param name="commandstr">The command string being executed.</param>
    ''' <param name="returnval">The return string from the command.</param>
    ''' <remarks></remarks>
    Public Delegate Sub PostCommandExecuteHook(ByVal commandstr As String, ByVal returnval As String)
    ''' <summary>
    ''' This hook is used to access and modify the main form.
    ''' </summary>
    ''' <param name="form">The passed main form.</param>
    ''' <remarks></remarks>
    Public Delegate Sub FormHook(ByRef form As Windows.Forms.Form)
    ''' <summary>
    ''' This hook is used to access the output box on the main form.
    ''' </summary>
    ''' <param name="txtbx">The output textbox on the form.</param>
    ''' <remarks></remarks>
    Public Delegate Sub OutputTextBoxHook(ByRef txtbx As Windows.Forms.TextBoxBase)
    ''' <summary>
    ''' This hook is used to access the reference to the syntax name string.
    ''' </summary>
    ''' <param name="syntax_name"></param>
    ''' <remarks></remarks>
    Public Delegate Sub SyntaxNameHook(ByRef syntax_name As String)
    ''' <summary>
    ''' This hook is used to run a command,
    ''' and can be invoked by the library once an instance is retrieved via the GetRunCommandHook.
    ''' </summary>
    ''' <param name="command">The command to pass.</param>
    ''' <param name="args">The arguments to pass.</param>
    ''' <returns>The return value of the command.</returns>
    ''' <remarks></remarks>
    Public Delegate Function RunCommandHook(ByVal command As String, ByVal args As String()) As String
    ''' <summary>
    ''' Gets the hook instance for RunCommandHook so it can be invoked by the library.
    ''' </summary>
    ''' <param name="hook">The hook instance.</param>
    ''' <remarks></remarks>
    Public Delegate Sub GetRunCommandHook(ByRef hook As RunCommandHook)
    ''' <summary>
    ''' This hook is used to write to the output textbox,
    ''' and can be invoked by the library once an instance is retrieved via the GetWriteOutputHook.
    ''' </summary>
    ''' <param name="text">The string to write to the output textbox.</param>
    ''' <remarks></remarks>
    Public Delegate Sub WriteOutputHook(ByVal text As String)
    ''' <summary>
    ''' Gets the hook instance for WriteOutputHook so it can be invoked by the library.
    ''' </summary>
    ''' <param name="hook">The hook instance.</param>
    ''' <remarks></remarks>
    Public Delegate Sub GetWriteOutputHook(ByRef hook As WriteOutputHook)
    ''' <summary>
    ''' This hook is used to read from the output textbox,
    ''' and can be invoked by the library once an instance is retrieved via the GetWriteOutputHook.
    ''' </summary>
    ''' <returns>The contents of the output textbox.</returns>
    ''' <remarks></remarks>
    Public Delegate Function ReadOutputHook() As String
    ''' <summary>
    ''' Gets the hook instance for ReadOutputHook so it can be invoked by the library.
    ''' </summary>
    ''' <param name="hook">The hook instance.</param>
    ''' <remarks></remarks>
    Public Delegate Sub GetReadOutputHook(ByRef hook As ReadOutputHook)
End Module
''' <summary>
''' API interface for adding other syntax language interpreters.
''' </summary>
''' <remarks></remarks>
Public Interface ISyntax
    ''' <summary>
    ''' Returns the Syntax Name.
    ''' </summary>
    ''' <returns>Returns the Syntax Name.</returns>
    ''' <remarks></remarks>
    Function name() As String
    ''' <summary>
    ''' Defines the required command decryptor, the returned list has the command as the first item and the arguments as the other items.
    ''' </summary>
    ''' <param name="strcmd">This is where the command string is passed.</param>
    ''' <param name="commands">This is where the list of commands on the console is passed.</param>
    ''' <returns>The returned list has the command as the first item and the arguments as the other items.</returns>
    ''' <remarks></remarks>
    Function decrypt(ByVal strcmd As String, ByVal commands As List(Of String)) As List(Of String)
    REM ''' <summary>
    REM ''' Defines the conversion of literals like numbers and strings to the right commands str dec int.
    REM ''' </summary>
    REM ''' <param name="strcmd">This is where the command string is passed.</param>
    REM ''' <param name="isConverted">This is where you should put if the literal was already converted.</param>
    REM ''' <returns>The literal converted to a command with an argument.</returns>
    REM ''' <remarks></remarks>
    REM Function literalconvert(ByVal strcmd As String, ByVal isConverted As Boolean) As String
End Interface
''' <summary>
''' Use this structure to register hooks against a delegate, use nothing to specify no hook.
''' </summary>
''' <remarks></remarks>
Public Structure HookInfo
    ''' <summary>
    ''' The hook set name.
    ''' </summary>
    ''' <remarks></remarks>
    Public name As String
    ''' <summary>
    ''' The hook command stack delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_commandstack As CommandStackHook
    ''' <summary>
    ''' The hook variable dictionary delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_variabledictionary As variableDictionaryHook
    ''' <summary>
    ''' The hook program start delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_programstart As ProgramEventHook
    ''' <summary>
    ''' The hook program stop delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_programstop As ProgramEventHook
    ''' <summary>
    ''' The hook command pre-execute delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_command_preexecute As PreCommandExecuteHook
    ''' <summary>
    ''' The hook command post-execute delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_command_postexecute As PostCommandExecuteHook
    ''' <summary>
    ''' The hook form delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_form As FormHook
    ''' <summary>
    ''' The hook out textbox delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_out_txtbx As OutputTextBoxHook
    ''' <summary>
    ''' The hook get runcommand delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_runcommand As GetRunCommandHook
    ''' <summary>
    ''' The hook get writeoutput delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_writeoutput As GetWriteOutputHook
    ''' <summary>
    ''' The hook get readoutput delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_readoutput As GetReadOutputHook
    ''' <summary>
    ''' The hook syntax name delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_syntaxname As SyntaxNameHook
    ''' <summary>
    ''' Constructs a new set of hook info, use nothing as a parameter if you do not want to register a certain hook.
    ''' </summary>
    ''' <param name="hook_set_name">The name of the hook set.</param>
    ''' <param name="hcmdstk">The command stack hook delegate.</param>
    ''' <param name="hvardic">The variable dictionary hook delegate</param>
    ''' <param name="hcompreex">The Pre-Command Execute hook delegate.</param>
    ''' <param name="hcompstex">The Post-Command Execute hook delegate.</param>
    ''' <param name="hprostr">The Program Start hook delegate.</param>
    ''' <param name="hprostp">The Program stop hook delegate.</param>
    ''' <param name="hform">The form hook delegate.</param>
    ''' <param name="hout">The output textbox hook delegate.</param>
    ''' <param name="getrcom">The Get RunCommand Hook delegate.</param>
    ''' <param name="getwout">The Get WriteOutput Hook delegate.</param>
    ''' <param name="getrout">The Get ReadOutput Hook delegate.</param>
    ''' <param name="synxnom">The Syntax Name Hook delegate.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal hook_set_name As String, Optional ByVal hcmdstk As CommandStackHook = Nothing, Optional ByVal hvardic As variableDictionaryHook = Nothing, Optional ByVal hprostr As ProgramEventHook = Nothing, Optional ByVal hprostp As ProgramEventHook = Nothing, Optional ByVal hcompreex As PreCommandExecuteHook = Nothing, Optional ByVal hcompstex As PostCommandExecuteHook = Nothing, Optional ByVal hform As FormHook = Nothing, Optional ByVal hout As OutputTextBoxHook = Nothing, Optional ByVal getrcom As GetRunCommandHook = Nothing, Optional ByVal getwout As GetWriteOutputHook = Nothing, Optional ByVal getrout As GetReadOutputHook = Nothing, Optional ByVal synxnom As SyntaxNameHook = Nothing)
        name = hook_set_name
        hook_commandstack = hcmdstk
        hook_variabledictionary = hvardic
        hook_programstart = hprostr
        hook_programstop = hprostp
        hook_command_preexecute = hcompreex
        hook_command_postexecute = hcompstex
        hook_form = hform
        hook_out_txtbx = hout
        hook_runcommand = getrcom
        hook_writeoutput = getwout
        hook_readoutput = getrout
        hook_syntaxname = synxnom
    End Sub
End Structure
''' <summary>
''' Pass this structure from your setup() function, you need one of these functions per class, structure or module.
''' </summary>
''' <remarks></remarks>
Public Structure LibrarySetup
    ''' <summary>
    ''' Returns the name of the library.
    ''' </summary>
    ''' <remarks></remarks>
    Public name As String
    ''' <summary>
    ''' Returns the version number of the library.
    ''' </summary>
    ''' <remarks></remarks>
    Public version As Integer
    ''' <summary>
    ''' Returns the hook set information of the library.
    ''' </summary>
    ''' <remarks></remarks>
    Public hook_info As HookInfo
    ''' <summary>
    ''' Returns the commands of the library.
    ''' </summary>
    ''' <remarks></remarks>
    Public commands As command()
    ''' <summary>
    ''' Retruns the syntaxes of the library.
    ''' </summary>
    ''' <remarks></remarks>
    Public syntaxes As ISyntax()
    ''' <summary>
    ''' Constructs a new LibrarySetup Structure.
    ''' </summary>
    ''' <param name="_name">The library name.</param>
    ''' <param name="ver">The library Version.</param>
    ''' <param name="_hookinfo">The library Hook Information.</param>
    ''' <param name="_commands">The library Commands Array.</param>
    ''' <param name="_syntaxes">The library Syntax Array.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _name As String, ByVal ver As Integer, ByVal _hookinfo As HookInfo, Optional ByVal _commands As command() = Nothing, Optional ByVal _syntaxes As ISyntax() = Nothing)
        name = _name
        version = ver
        hook_info = _hookinfo
        commands = _commands
        syntaxes = _syntaxes
    End Sub
End Structure
''' <summary>
''' Use this structure to register commands from a library.
''' </summary>
''' <remarks></remarks>
Public Structure Command
    ''' <summary>
    ''' The held command delegate.
    ''' </summary>
    ''' <remarks></remarks>
    Public command As [Delegate]
    ''' <summary>
    ''' The name of the command.
    ''' </summary>
    ''' <remarks></remarks>
    Public name As String
    ''' <summary>
    ''' The help string for the command.
    ''' </summary>
    ''' <remarks></remarks>
    Public help As String
    ''' <summary>
    ''' Constructs a new command.
    ''' </summary>
    ''' <param name="_name">The name of the command.</param>
    ''' <param name="del">The delegate for the command using <seealso cref=" captainalm.calmcon.api.types.Cmd">the Cmd delegate type</seealso>.</param>
    ''' <param name="_help">The help string for the command.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal _name As String, ByVal del As Cmd, Optional ByVal _help As String = "")
        name = _name
        command = del
        help = _help
    End Sub
End Structure