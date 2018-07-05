Public Class BrowseFolder

    Private Sub BrowseFolder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Form1.options = 1 Then
            Me.Text = "ArmA 2 OA Config Folder"
            Label1.Text = "Location of ArmA 2 OA Config Folder:"
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Alla pressione del bottone "OK"
        If Not System.IO.File.Exists(TextBox1.Text.Trim + "\ArmA2OA.cfg") Then
            MsgBox("ArmA 2 OA config file doesn't exists at location " + TextBox1.Text.Trim, MsgBoxStyle.Exclamation, "ArmA 2 OA Config File Not Found")
            Exit Sub
        Else
            My.Settings.A2ConfigPath = TextBox1.Text.Trim
            Form1.A2ConfigPath = TextBox1.Text.Trim
            Me.Close()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Alla pressione del tasto "Cancel"
        Form1.A2ConfigPath = ""
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Apri il FolderBrowserDialog
        If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
End Class