Public Class Password

    ' Al caricamento del form
    Private Sub Password_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Select()   ' Focus sulla textbox
    End Sub

    ' Controllo della password
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then
            MsgBox("You have to enter a valid password to connect to the server!", MsgBoxStyle.Exclamation, "Blank Password Error")
            TextBox1.Select()
        Else
            ServerBrowser.passwordNeed = TextBox1.Text()
            Me.Close()
        End If
    End Sub

    ' Bottone "Cancel". Chiude il form ed annulla la connessione al server
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ServerBrowser.passwordNeed = ""
        Me.Close()
    End Sub
End Class