''' <summary>
''' This class is used to output text to the rich textbox output.
''' </summary>
''' <remarks></remarks>
Public Class OutputText
    Private _blocks As New List(Of TextBlock)
    ''' <summary>
    ''' Creates a new output text instance
    ''' </summary>
    ''' <param name="txt">The intal text.</param>
    ''' <param name="forecol">The intal forecolor.</param>
    ''' <param name="bld">Is the intal text bold.</param>
    ''' <param name="itl">Is the intal text italic.</param>
    ''' <param name="ul">Is the intal text underlined.</param>
    ''' <param name="so">Is the intal text strikeout.</param>
    ''' <remarks></remarks>
    Public Sub New(Optional txt As String = "", Optional forecol As Drawing.Color = Nothing, Optional bld As Boolean = False, Optional itl As Boolean = False, Optional ul As Boolean = False, Optional so As Boolean = False)
        If Not txt Is Nothing And txt <> "" Then
            Dim cblock As New TextBlock(txt, forecol, bld, itl, ul, so)
            _blocks.Add(cblock)
        End If
    End Sub
    'Only accessible in the Shared & API Library.
    Friend Sub New(otba As OutputTextBlock())
        For Each otb As OutputTextBlock In otba
            _blocks.Add(New TextBlock(otb))
        Next
    End Sub
    ''' <summary>
    ''' Writes text to the current block with the current block's attributes.
    ''' </summary>
    ''' <param name="txt">The text to write.</param>
    ''' <exception cref="InvalidOperationException">Thrown when no blocks to write to.</exception>
    ''' <remarks></remarks>
    Public Sub write(txt As String)
        If Not _blocks.Count = 0 Then
            _blocks(_blocks.Count - 1).write(txt)
        Else
            Throw New InvalidOperationException("There are no blocks to write to.")
        End If
    End Sub
    ''' <summary>
    ''' Writes text and a new line to the current block with the current block's attributes.
    ''' </summary>
    ''' <param name="txt">The text to write.</param>
    ''' <exception cref="InvalidOperationException">Thrown when no blocks to write to.</exception>
    ''' <remarks></remarks>
    Public Sub writeline(txt As String)
        If Not _blocks.Count = 0 Then
            _blocks(_blocks.Count - 1).writeline(txt)
        Else
            Throw New InvalidOperationException("There are no blocks to write to.")
        End If
    End Sub
    ''' <summary>
    ''' Writes text to a new or the current block depending on the current block's attributes or if there are any blocks at all.
    ''' </summary>
    ''' <param name="txt">The text.</param>
    ''' <param name="forecol">The forecolor.</param>
    ''' <param name="bld">Is the text bold.</param>
    ''' <param name="itl">Is the text italic.</param>
    ''' <param name="ul">Is the text underlined.</param>
    ''' <param name="so">Is the text strikeout.</param>
    ''' <remarks></remarks>
    Public Sub write(txt As String, Optional forecol As Drawing.Color = Nothing, Optional bld As Boolean = False, Optional itl As Boolean = False, Optional ul As Boolean = False, Optional so As Boolean = False)
        If Not _blocks.Count = 0 Then
            If _blocks(_blocks.Count - 1).forecolor = forecol And _blocks(_blocks.Count - 1).bold = bld And _blocks(_blocks.Count - 1).italic = itl And _blocks(_blocks.Count - 1).underline = ul And _blocks(_blocks.Count - 1).strikeout = so Then
                _blocks(_blocks.Count - 1).write(txt)
            Else
                Dim cblock As New TextBlock(txt, forecol, bld, itl, ul, so)
                _blocks.Add(cblock)
            End If
        Else
            Dim cblock As New TextBlock(txt, forecol, bld, itl, ul, so)
            _blocks.Add(cblock)
        End If
    End Sub
    ''' <summary>
    ''' Writes text and a new line to a new or the current block depending on the current block's attributes or if there are any blocks at all.
    ''' </summary>
    ''' <param name="txt">The text.</param>
    ''' <param name="forecol">The forecolor.</param>
    ''' <param name="bld">Is the text bold.</param>
    ''' <param name="itl">Is the text italic.</param>
    ''' <param name="ul">Is the text underlined.</param>
    ''' <param name="so">Is the text strikeout.</param>
    ''' <remarks></remarks>
    Public Sub writeline(txt As String, Optional forecol As Drawing.Color = Nothing, Optional bld As Boolean = False, Optional itl As Boolean = False, Optional ul As Boolean = False, Optional so As Boolean = False)
        If Not _blocks.Count = 0 Then
            If _blocks(_blocks.Count - 1).forecolor = forecol And _blocks(_blocks.Count - 1).bold = bld And _blocks(_blocks.Count - 1).italic = itl And _blocks(_blocks.Count - 1).underline = ul And _blocks(_blocks.Count - 1).strikeout = so Then
                _blocks(_blocks.Count - 1).writeline(txt)
            Else
                Dim cblock As New TextBlock(txt & ControlChars.CrLf, forecol, bld, itl, ul, so)
                _blocks.Add(cblock)
            End If
        Else
            Dim cblock As New TextBlock(txt & ControlChars.CrLf, forecol, bld, itl, ul, so)
            _blocks.Add(cblock)
        End If
    End Sub
    ''' <summary>
    ''' Converts the contents of the OutputText to an array of OutputTextBlock Structures.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ToOutputTextBlocks() As OutputTextBlock()
        Dim lst As New List(Of OutputTextBlock)
        For Each tb As TextBlock In _blocks
            lst.Add(tb.ToOutputTextBlock)
        Next
        Return lst.ToArray
    End Function
    ''' <summary>
    ''' Returns the number of blocks in the OutputText object.
    ''' </summary>
    ''' <value>Integer.</value>
    ''' <returns>The number of blocks in the OutputText object.</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property BlockCount As Integer
        Get
            Return _blocks.Count
        End Get
    End Property
    ''' <summary>
    ''' The default property, gets a block form an index.
    ''' </summary>
    ''' <param name="index">The index number.</param>
    ''' <value>OutputTextBlock</value>
    ''' <returns>The OutputTextBlock as that index.</returns>
    ''' <remarks></remarks>
    Default ReadOnly Property Block(index As Integer) As OutputTextBlock
        Get
            Return _blocks(index).ToOutputTextBlock()
        End Get
    End Property
    ''' <summary>
    ''' Converts a string to outputtext.
    ''' </summary>
    ''' <param name="str">The string to convert.</param>
    ''' <returns>The output text instance.</returns>
    ''' <remarks></remarks>
    Public Shared Widening Operator CType(ByVal str As String) As OutputText
        Return New OutputText(str)
    End Operator
    ''' <summary>
    ''' Converts an array of outputtextblock to outputtext.
    ''' </summary>
    ''' <param name="otba">The outputtextblock array to convert.</param>
    ''' <returns>The output text instance.</returns>
    ''' <remarks></remarks>
    Public Shared Widening Operator CType(ByVal otba As OutputTextBlock()) As OutputText
        Return New OutputText(otba)
    End Operator
    ''' <summary>
    ''' Converts an outputtext instance to an string.
    ''' </summary>
    ''' <param name="optxt">The outputtext instance to convert.</param>
    ''' <returns>The string held by the outputtext instance.</returns>
    ''' <remarks></remarks>
    Public Shared Narrowing Operator CType(ByVal optxt As OutputText) As String
        Dim txt As String = ""
        For Each bk As TextBlock In optxt._blocks
            txt &= bk.text
        Next
        Return txt
    End Operator
    ''' <summary>
    ''' Converts an outputtext instance to an output text block array.
    ''' </summary>
    ''' <param name="optxt">The outputtext instance to convert.</param>
    ''' <returns>A outputtextblock array form the outputtext instance.</returns>
    ''' <remarks></remarks>
    Public Shared Narrowing Operator CType(ByVal optxt As OutputText) As OutputTextBlock()
        Dim lst As New List(Of OutputTextBlock)
        For Each tb As TextBlock In optxt._blocks
            lst.Add(tb.ToOutputTextBlock)
        Next
        Return lst.ToArray
    End Operator
    ''' <summary>
    ''' Concats two Output Texts Together.
    ''' </summary>
    ''' <param name="optxt1">The first output text.</param>
    ''' <param name="optxt2">The second output text.</param>
    ''' <returns>The concated output text object.</returns>
    ''' <remarks></remarks>
    Public Shared Operator &(ByVal optxt1 As OutputText, ByVal optxt2 As OutputText) As OutputText
        Dim noptxt As New OutputText(optxt1.ToOutputTextBlocks)
        For Each cb As TextBlock In optxt2._blocks
            noptxt.write(cb.text, cb.forecolor, cb.bold, cb.italic, cb.underline, cb.strikeout)
        Next
        'noptxt._blocks.AddRange(optxt1._blocks)
        'noptxt._blocks.AddRange(optxt2._blocks)
        Return noptxt
    End Operator
    ''' <summary>
    ''' Checks if two output texts are not equal.
    ''' </summary>
    ''' <param name="optxt1">The first output text.</param>
    ''' <param name="optxt2">The second output text.</param>
    ''' <returns>The boolean of if they are not equal.</returns>
    ''' <remarks></remarks>
    Public Shared Operator <>(ByVal optxt1 As OutputText, ByVal optxt2 As OutputText) As Boolean
        Return (Not optxt1._blocks.Equals(optxt2._blocks)) And (Not optxt2._blocks.Equals(optxt1._blocks))
    End Operator
    ''' <summary>
    ''' Checks if two output texts are equal.
    ''' </summary>
    ''' <param name="optxt1">The first output text.</param>
    ''' <param name="optxt2">The second output text.</param>
    ''' <returns>The boolean of if they are equal.</returns>
    ''' <remarks></remarks>
    Public Shared Operator =(ByVal optxt1 As OutputText, ByVal optxt2 As OutputText) As Boolean
        Return (optxt1._blocks.Equals(optxt2._blocks)) And (optxt2._blocks.Equals(optxt1._blocks))
    End Operator
    'This is the internal textblock instance.
    Private Class TextBlock
        Private _text As String = ""
        Private _forecolor As Drawing.Color = Drawing.Color.Black
        Private _bold As Boolean = False
        Private _italic As Boolean = False
        Private _underline As Boolean = False
        Private _strike As Boolean = False

        Public Sub New(otb As OutputTextBlock)
            _text = otb.text
            _forecolor = otb.forecolor
            _bold = otb.bold
            _italic = otb.italic
            _underline = otb.underline
            _strike = otb.strikeout
        End Sub

        Public Sub New(Optional txt As String = "", Optional forecol As Drawing.Color = Nothing, Optional bld As Boolean = False, Optional itl As Boolean = False, Optional ul As Boolean = False, Optional so As Boolean = False)
            _text = txt
            If forecol.IsEmpty Then
                _forecolor = Drawing.Color.Black
            Else
                _forecolor = forecol
            End If
            _bold = bld
            _italic = itl
            _underline = ul
            _strike = so
        End Sub

        Public Property text As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
            End Set
        End Property

        Public Property forecolor As Drawing.Color
            Get
                Return _forecolor
            End Get
            Set(value As Drawing.Color)
                _forecolor = value
            End Set
        End Property

        Public Property bold As Boolean
            Get
                Return _bold
            End Get
            Set(value As Boolean)
                _bold = value
            End Set
        End Property

        Public Property italic As Boolean
            Get
                Return _italic
            End Get
            Set(value As Boolean)
                _italic = value
            End Set
        End Property

        Public Property underline As Boolean
            Get
                Return _underline
            End Get
            Set(value As Boolean)
                _underline = value
            End Set
        End Property

        Public Property strikeout As Boolean
            Get
                Return _strike
            End Get
            Set(value As Boolean)
                _strike = value
            End Set
        End Property

        Public Sub write(txt As String)
            _text &= txt
        End Sub

        Public Sub writeline(txt As String)
            _text &= txt & ControlChars.CrLf
        End Sub

        Public Function ToOutputTextBlock() As OutputTextBlock
            Return New OutputTextBlock(_text, _forecolor, _bold, _italic, _underline, _strike)
        End Function
    End Class
End Class
''' <summary>
''' This is what is the split up parts of OutputText.
''' </summary>
''' <remarks></remarks>
Public Structure OutputTextBlock
    ''' <summary>
    ''' The held text.
    ''' </summary>
    ''' <remarks></remarks>
    Public text As String
    ''' <summary>
    ''' The foreground colour of the text.
    ''' </summary>
    ''' <remarks></remarks>
    Public forecolor As Drawing.Color
    ''' <summary>
    ''' If the text is bold.
    ''' </summary>
    ''' <remarks></remarks>
    Public bold As Boolean
    ''' <summary>
    ''' If the text is italic.
    ''' </summary>
    ''' <remarks></remarks>
    Public italic As Boolean
    ''' <summary>
    ''' If the text is underlined.
    ''' </summary>
    ''' <remarks></remarks>
    Public underline As Boolean
    ''' <summary>
    ''' If the text is striked out.
    ''' </summary>
    ''' <remarks></remarks>
    Public strikeout As Boolean
    'Only accessible in the Shared & API Library.
    Friend Sub New(txt As String, forecol As Drawing.Color, bld As Boolean, itl As Boolean, ul As Boolean, so As Boolean)
        text = txt
        forecolor = forecol
        bold = bld
        italic = itl
        underline = ul
        strikeout = so
    End Sub
End Structure