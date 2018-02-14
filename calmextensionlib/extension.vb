Imports captainalm.calmcon.api

Public Class extension
    Public Function setup() As LibrarySetup
        'reg syntaxes
        Dim syntaxes(4) As ISyntax
        syntaxes(0) = New calm_type
        syntaxes(1) = New spaced_type
        syntaxes(2) = New commad_type
        syntaxes(3) = New objective_type
        syntaxes(4) = New cbrak_objective_type
        'reg commands
        Dim commands As Command() = Nothing
        'reg hooks
        Dim hooki As New HookInfo("")
        'reg lib
        Return New LibrarySetup("calm_extension_library", 0, hooki, commands, syntaxes)
    End Function
End Class


