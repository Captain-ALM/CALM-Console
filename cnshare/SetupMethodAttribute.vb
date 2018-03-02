''' <summary>
''' This Attribute is used to Specify The Setup Method of The Library.
''' </summary>
''' <remarks></remarks>
<AttributeUsage(AttributeTargets.Method)>
Public Class SetupMethodAttribute
    Inherits Attribute
    ''' <summary>
    ''' Initalizes a new instance of this Attribute on The Method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
    End Sub
End Class
