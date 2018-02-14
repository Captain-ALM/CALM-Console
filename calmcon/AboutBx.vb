Imports System.Reflection

Public NotInheritable Class AboutBx
    Private description As String = ""
    Private license As String = ""


    Public Sub setupdata(des As String, lic As String)
        description = des
        license = lic
    End Sub

    Private Sub AboutBx_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = "Description: " & vbCrLf & My.Application.Info.Description & vbCrLf & vbCrLf & "Captain ALM Console Shared Library & API Version:" & vbCrLf & GetType(captainalm.calmcon.api.types).Assembly.GetName.Version.ToString & vbCrLf & vbCrLf & description
        Me.TextBox1.Text = "License: " & vbCrLf & license
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub
End Class
