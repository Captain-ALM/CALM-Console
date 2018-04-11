''' <summary>
''' This provides API switching for the program.
''' </summary>
''' <remarks></remarks>
Public Module Switches
    ''' <summary>
    ''' Enable, Disable or Program Control the Multiline Entry Checkbox.
    ''' </summary>
    ''' <remarks></remarks>
    Public MultilineCheckboxEnablement As Boolean? = Nothing
    ''' <summary>
    ''' Read keys from the output boxes.
    ''' </summary>
    ''' <remarks></remarks>
    Public OutputBoxReadKey As Boolean = False
    ''' <summary>
    ''' Enable Or Disable Command Execution.
    ''' </summary>
    ''' <remarks></remarks>
    Public CommandExecution As Boolean = True
    ''' <summary>
    ''' Allow multipule Lines to be Entered into the command box.
    ''' </summary>
    ''' <remarks></remarks>
    Public AllowMultiCommandEntry As Boolean = False
End Module
''' <summary>
''' This provides API manipulation for the program.
''' </summary>
''' <remarks></remarks>
Public Module Manipulator
    ''' <summary>
    ''' This provides the command stack for access and modification.
    ''' </summary>
    ''' <remarks></remarks>
    Public CommandStack As New Stack(Of String)
    ''' <summary>
    ''' This provides the variable dictionary for access and modification.
    ''' </summary>
    ''' <remarks></remarks>
    Public VariableDictionary As New Dictionary(Of String, String)
    ''' <summary>
    ''' Provides the syntax mode for access and manipulation, be careful!
    ''' </summary>
    ''' <remarks></remarks>
    Public SyntaxMode As String = ""
End Module
