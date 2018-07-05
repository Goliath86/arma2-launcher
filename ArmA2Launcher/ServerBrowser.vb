Imports QueryMaster
Imports System.Collections.ObjectModel
Imports System.Net

Public Class ServerBrowser

    Private NumServer As Integer = 0       ' Variabile che tiene conto del numero totale di Game Servers
    Private stopIter As Integer = 0
    Private MasterServer As MasterServer
    Private avvio As Integer = 1
    Public passwordNeed As String = ""

    Private Delegate Sub UpdateDGV(info As ServerInfo, rules As ReadOnlyCollection(Of Rule))

    Private syncObject As System.ComponentModel.ISynchronizeInvoke = Nothing

    Private Sub recv(endPoints As ReadOnlyCollection(Of IPEndPoint))
        ' Per ogni indirizzo IP ritornato dal thread, esegui una query per ricavarne
        ' le informazioni
        If stopIter = 0 Then
            For Each i As IPEndPoint In endPoints
                If stopIter = 0 Then
                    '"0.0.0.0:0" è l'ultimo indirizzo ricevuto di tutta la lista
                    If i.ToString <> "0.0.0.0:0" Then
                        Dim server As Server = ServerQuery.GetServerInstance(EngineType.Source, i)
                        Try
                            Dim info As ServerInfo = server.GetInfo()
                            Dim rules As ReadOnlyCollection(Of Rule) = server.GetRules()
                            QueryServer(info, rules)
                        Catch
                            ' Evito l'eccezione dovuta ad un timeout di collegamento al server
                        End Try
                        server.Dispose()
                    Else
                        ' Se raggiunta la fine della lista
                        ToolStripStatusLabel1.Image = My.Resources.Button_Blank_Green_icon
                    End If
                End If
            Next
            'Else
            'ToolStripStatusLabel1.Image = My.Resources.Button_Blank_Green_icon
            'MsgBox("Iter fermo")
        End If
    End Sub

    Private Sub ServerBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        syncObject = Me
        NumServer = 0
        stopIter = 0

        DataGridView1.Columns("PlayersColumn").ValueType = GetType(Integer)           ' Rendi la colonna "PlayersColumn" una colonna di Integers
        DataGridView1.Columns("MaxPlayersColumn").ValueType = GetType(Integer)        ' Rendi la colonna "MaxPlayersColumn" una colonna di Integers
        DataGridView1.Columns("PingColumn").ValueType = GetType(Integer)              ' Rendi la colonna "Ping" una colonna di Integers

        ' Setta il RadioButton in base alle impostazioni
        Dim ser As Integer = My.Settings.ShowServers
        Select Case ser
            Case 1
                RadioButton1.Checked = True
            Case 2
                RadioButton2.Checked = True
            Case 3
                RadioButton3.Checked = True
        End Select

        MasterServer = MasterQuery.GetMasterServerInstance(EngineType.Source)

        ' Controlla e setta le impostazioni salvate del programma
        CheckSettings()

        ' Costruisci il filtro per i server
        Dim a As New IpFilter
        MakeFilter(a)

        avvio = 0

        ' Interroga il Master Server di Steam per la lista IP di tutti i Game Servers
        MasterServer.GetAddresses(QueryMaster.Region.Rest_of_the_world, AddressOf recv, a)
    End Sub

    Private Sub QueryServer(info As ServerInfo, rules As ReadOnlyCollection(Of Rule))
        If Not Me.IsDisposed Then
            If Me.syncObject Is Nothing OrElse Not Me.syncObject.InvokeRequired Then
                ' Aggiungi il server come elemento al DGV
                'MsgBox(info.Name())
                Try
                    ' Controlla se visualizzare o meno i server di DayZ
                    If RadioButton1.Checked Then

                        ' Mostra tutti i servers (ArmA 2 e DayZ)
                        Dim address As String = info.Address.Split(":")(0)
                        Dim port As String = ""
                        Dim serverType As String = info.Environment
                        Dim image As Bitmap
                        Dim privato As Bitmap = My.Resources.Blank

                        If serverType = "Windows" Then
                            image = My.Resources.Windows_icon
                        Else
                            image = My.Resources.linux_icon
                        End If

                        If info.IsPrivate Then
                            privato = My.Resources.key_icon
                        End If

                        ' Ricava la porta del server, se passata come informazione
                        If info.Extra.Contains("ServerPort") Then
                            port = (info.Extra.Split(";")(0)).Split("=")(1)
                        End If

                        Dim item As Object()
                        item = New Object() {My.Resources.Blank, privato, info.Name, info.Players, CType(info.MaxPlayers, Integer), address, port, info.Ping, info.Description, image, info.GameVersion}
                        DataGridView1.Rows.Insert(DataGridView1.Rows.Count, item)
                        NumServer += 1
                        ToolStripStatusLabel1.Text = "  " + NumServer.ToString + " servers found"

                    ElseIf RadioButton2.Checked Then

                        ' Se non voglio visualizzare i server di DayZ
                        Dim jump As Integer = 0             ' Variabile che identifica la presenza di un qualcosa che ha a che fare con DayZ

                        For Each regola As Rule In rules
                            'MsgBox(regola.Name + "   " + regola.Value)
                            If jump = 0 And regola.Value.ToString().IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And regola.Value.ToString().IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And regola.Value.ToString().IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                                jump = 1
                            End If
                        Next

                        If jump = 0 And Not info.Description.IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And Not info.Name.IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And Not info.Name.IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And Not info.Name.IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And Not info.Description.IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And Not info.Description.IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                            ' Se nè il nome, nè la descrizione contengono la parola dayz allora visualizza il server nel DGV
                            Dim address As String = info.Address.Split(":")(0)
                            'Dim port As String = info.Address.Split(":")(1)        ' Porta del Game Server per il dialogo con lo Steam Master Server
                            Dim port As String = ""
                            Dim serverType As String = info.Environment
                            Dim image As Bitmap
                            Dim privato As Bitmap = My.Resources.Blank

                            If serverType = "Windows" Then
                                image = My.Resources.Windows_icon
                            Else
                                image = My.Resources.linux_icon
                            End If

                            If info.IsPrivate Then
                                privato = My.Resources.key_icon
                            End If

                            ' Ricava la porta del server, se passata come informazione
                            If info.Extra.Contains("ServerPort") Then
                                port = (info.Extra.Split(";")(0)).Split("=")(1)
                            End If

                            Dim item As Object()
                            item = New Object() {My.Resources.Blank, privato, info.Name, info.Players, CType(info.MaxPlayers, Integer), address, port, info.Ping, info.Description, image, info.GameVersion}
                            DataGridView1.Rows.Insert(DataGridView1.Rows.Count, item)
                            NumServer += 1
                            ToolStripStatusLabel1.Text = "  " + NumServer.ToString + " servers found"
                        End If
                    Else
                        ' RadioButton3.Checked = True

                        ' Se non voglio visualizzare i server di ArmA 2
                        Dim jump As Integer = 0             ' Variabile che identifica la presenza di un qualcosa che ha a che fare con ArmA 2

                        For Each regola As Rule In rules
                            'MsgBox(regola.Name + "   " + regola.Value)
                            If jump = 0 And regola.Value.ToString().IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And regola.Value.ToString().IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 And regola.Value.ToString().IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                                jump = 1
                            End If
                        Next

                        If jump = 1 Or info.Description.IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Or info.Name.IndexOf("overpoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Or info.Name.IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Or info.Name.IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Or info.Description.IndexOf("dayz", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Or info.Description.IndexOf("epoch", 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                            ' Se nè il nome, nè la descrizione contengono la parola dayz allora visualizza il server nel DGV
                            Dim address As String = info.Address.Split(":")(0)
                            'Dim port As String = info.Address.Split(":")(1)        ' Porta del Game Server per il dialogo con lo Steam Master Server
                            Dim port As String = ""
                            Dim serverType As String = info.Environment
                            Dim image As Bitmap
                            Dim privato As Bitmap = My.Resources.Blank

                            If serverType = "Windows" Then
                                image = My.Resources.Windows_icon
                            Else
                                image = My.Resources.linux_icon
                            End If

                            If info.IsPrivate Then
                                privato = My.Resources.key_icon
                            End If

                            ' Ricava la porta del server, se passata come informazione
                            If info.Extra.Contains("ServerPort") Then
                                port = (info.Extra.Split(";")(0)).Split("=")(1)
                            End If

                            Dim item As Object()
                            item = New Object() {My.Resources.Blank, privato, info.Name, info.Players, CType(info.MaxPlayers, Integer), address, port, info.Ping, info.Description, image, info.GameVersion}
                            DataGridView1.Rows.Insert(DataGridView1.Rows.Count, item)
                            NumServer += 1
                            ToolStripStatusLabel1.Text = "  " + NumServer.ToString + " servers found"
                        End If
                    End If
                Catch ex As Exception
                    'MsgBox("Error querying game server:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Error querying game server")
                End Try
            Else
                ' Invoca la funzione QueryServer() dal thread che contiene il DGV (ovvero il form stesso)
                ' in modo da poter aggiungere elementi al DGV
                syncObject.Invoke(New UpdateDGV(AddressOf QueryServer), New Object() {info, rules})
            End If
        End If
    End Sub

    Private Sub ServerBrowser_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Alla chiusura del form interrompi la ricerca dei server browser
        stopIter = 1
        MasterServer.Dispose()
        Me.Dispose()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Refresh
        stopIter = 1

        Button2.Enabled = False

        NumServer = 0

        MasterServer.Dispose()

        DataGridView1.Rows.Clear()

        ToolStripStatusLabel1.Text = "  Trying to reconnect to Steam Master Server..."
        ToolStripStatusLabel1.Image = My.Resources.Blinking_Yellow_Dot_2

        ' Pausa di sei secondi
        Dim tempo As DateTime
        tempo = Now.AddMilliseconds(6000)
        Do
            Application.DoEvents()
        Loop Until Now > tempo

        Button2.Enabled = True

        ' Costruisci il filtro per i server
        Dim a As New IpFilter
        MakeFilter(a)
        stopIter = 0

        MasterServer = MasterQuery.GetMasterServerInstance(EngineType.Source)
        MasterServer.GetAddresses(QueryMaster.Region.Rest_of_the_world, AddressOf recv, a)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Stop il refresh
        stopIter = 1

        If NumServer = 0 Then
            ToolStripStatusLabel1.Text = "Stopped"
            ToolStripStatusLabel1.Image = My.Resources.Button_Blank_Green_icon
        Else
            ToolStripStatusLabel1.Image = My.Resources.Button_Blank_Green_icon
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        ' Al click sul RadioButton1 (Mostra ArmA 2 e DayZ servers)
        If RadioButton1.Checked Then
            My.Settings.ShowServers = 1
            My.Settings.Save()
            If avvio = 0 Then
                Button1.PerformClick()
            End If
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        ' Al click sul RadioButton2 (Mostra solo ArmA 2 servers)
        If RadioButton2.Checked Then
            My.Settings.ShowServers = 2
            My.Settings.Save()
            If avvio = 0 Then
                Button1.PerformClick()
            End If
        End If
    End Sub


    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        ' Al click sul RadioButton3 (Mostra solo DayZ servers)
        If RadioButton3.Checked Then
            My.Settings.ShowServers = 3
            My.Settings.Save()
            If avvio = 0 Then
                Button1.PerformClick()
            End If
        End If
    End Sub

    Private Sub MakeFilter(filtro As IpFilter)
        ' Generazione di un filtro

        ' Scelta del gioco di cui cercare i servers
        filtro.GameDirectory = "arma2arrowpc"

        ' Server dedicato
        If CheckBox1.Checked Then
            filtro.IsDedicated = True
        Else
            filtro.IsDedicated = False
        End If

        ' Server vuoto
        If CheckBox2.Checked Then
            filtro.IsNoPlayers = True
        Else
            filtro.IsNoPlayers = False
        End If

        ' Server non vuoti
        If CheckBox3.Checked Then
            filtro.IsNotEmpty = True
        Else
            filtro.IsNotEmpty = False
        End If

        ' Server non pieno
        If CheckBox4.Checked Then
            filtro.IsNotFull = True
        Else
            filtro.IsNotFull = False
        End If

    End Sub

    ' Se clicco sul checkbox per filtrare solo i server vuoti
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            CheckBox3.Checked = False   ' Disabilita il checkbox per filtrare i server con minimo un giocatore
        End If

        If avvio = 0 Then
            If CheckBox2.Checked Then
                My.Settings.Empty = True
            Else
                My.Settings.Empty = False
            End If
            My.Settings.Save()
        End If

    End Sub
    ' Se clicco sul checkbox per filtrare i server con minimo un giocatore
    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked Then
            CheckBox2.Checked = False   ' Disabilita il checkbox per filtrare i server vuoti
        End If

        If avvio = 0 Then
            If CheckBox3.Checked Then
                My.Settings.NotEmpty = True
            Else
                My.Settings.NotEmpty = False
            End If
            My.Settings.Save()
        End If
    End Sub

    ' Dedicated checkbox
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If avvio = 0 Then
            If CheckBox1.Checked Then
                My.Settings.Dedi = True
            Else
                My.Settings.Dedi = False
            End If
            My.Settings.Save()
        End If
    End Sub

    ' NotFull checkbox
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If avvio = 0 Then
            If CheckBox4.Checked Then
                My.Settings.NotFull = True
            Else
                My.Settings.NotFull = False
            End If
            My.Settings.Save()
        End If
    End Sub

    'Controllo e settaggio delle impostazioni del programma
    Private Sub CheckSettings()
        If My.Settings.Dedi = True Then
            CheckBox1.Checked = True
        End If

        If My.Settings.Empty = True Then
            CheckBox2.Checked = True
        End If

        If My.Settings.NotEmpty = True Then
            CheckBox3.Checked = True
        End If

        If My.Settings.NotFull = True Then
            CheckBox4.Checked = True
        End If
    End Sub

    ' Al doppio click su di una riga del DGV
    Private Sub DataGridView1_DoubleClick(sender As Object, e As EventArgs) Handles DataGridView1.DoubleClick
        ' Se non ho cliccato due volte su di una riga valida del DGV
        If DataGridView1.SelectedRows.Count < 1 Then
            Exit Sub
        End If

        ' Verificare se è richiesta una password per connettersi
        ' e se sì, aprire un form per l'immissione della stessa
        Dim riga As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim colonna As DataGridViewImageCell = riga.Cells(1)
        Dim bitMap As Bitmap = colonna.Value
        If CompareImages(bitMap, My.Resources.key_icon) Then
            ' Mostra il form per l'immissione di una password
            Password.ShowDialog()

            If Not passwordNeed = "" Then
                ' Se la password è stata inserita nel form precedentemente aperto
                Form1.TextBox3.Text = passwordNeed

                ' Scrivi gli altri parametri sui textbox del Form1
                Dim ip As String = riga.Cells(5).Value.ToString
                Dim port As String = riga.Cells(6).Value.ToString

                Form1.TextBox1.Text = ip
                Form1.TextBox2.Text = port

                ' Chiudi il form del Server Browser
                Me.Close()
            Else
                ' Se la password non è stata inserita nel form precedentemente aperto
                Exit Sub
            End If
        Else
            ' Se il server selezionato non è privato

            ' Scrivi gli altri parametri sui textbox del Form1
            Dim ip As String = riga.Cells(5).Value.ToString
            Dim port As String = riga.Cells(6).Value.ToString

            Form1.TextBox1.Text = ip
            Form1.TextBox2.Text = port

            ' Chiudi il form del Server Browser
            Me.Close()
        End If
    End Sub

    ' Funzione per la comparazione di due immagini
    Private Function CompareImages(image1 As Bitmap, image2 As Bitmap) As Boolean
        If image1.Width = image2.Width AndAlso image1.Height = image2.Height Then
            For i As Integer = 0 To image1.Width - 1
                For j As Integer = 0 To image1.Height - 1
                    If image1.GetPixel(i, j) <> image2.GetPixel(i, j) Then
                        Return False
                    End If
                Next
            Next
            Return True
        Else
            Return False
        End If
    End Function
End Class