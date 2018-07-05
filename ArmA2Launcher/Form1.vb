Imports Microsoft.Win32
Imports QueryMaster
Imports System.Collections.ObjectModel
Imports System.Net

Public Class Form1
    Public A2ConfigPath As String = ""
    Private A2ConfigFile As String = ""
    Private A2File As System.IO.StreamReader
    Private folders As New List(Of String)
    Private folders_doc As New List(Of String)
    Private fileWriter As System.IO.StreamWriter
    Private fileReader As System.IO.StreamReader
    Private profiles As New List(Of String)
    Private proFile As New List(Of String)
    Private modsList As New List(Of String)
    Private startUp As New List(Of String)
    Private checked As New List(Of String)
    Private Arma2RegFound As String = ""
    Public options As Integer = 0


    Private Shared Sub recv(endPoints As ReadOnlyCollection(Of IPEndPoint))
        For Each i As IPEndPoint In endPoints
            '"0.0.0.0:0" is the last address
            If i.ToString <> "0.0.0.0:0" Then
                Dim server As Server = ServerQuery.GetServerInstance(EngineType.Source, i)
            End If
        Next
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Al caricamento del form
        Dim uscita As Boolean = False

        ' Rendi trasparente la PictureBox11 relativa al logo dei TS
        PictureBox11.Parent = PictureBox1
        PictureBox11.Location = New Point(540, 25) 'Y=10

        'If Not System.IO.File.Exists(Application.ExecutablePath() + "\ArmA2OA.exe") Then
        'MsgBox("Could not find the ArmA 2 OA executable. Launcher must be inside the ArmA 2 OA folder!", MsgBoxStyle.Exclamation, "ArmA 2 OA Executable Not Found")
        'uscita = True
        'Else

        ' Visualizza il numero di versione dell'applicazione
        Me.Text = "ArmA 2 OA Launcher v" + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()

        ' Memorizza il percorso in cui dovrebbe trovarsi la cartella delle configurazioni di ArmA 2 OA
        If My.Settings.A2ConfigPath = "" Then
            A2ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\ArmA 2"
        Else
            A2ConfigPath = My.Settings.A2ConfigPath
        End If

        ' Controlla per verificare l'esistenza della cartella del profilo di ArmA 2
        If Not System.IO.Directory.Exists(A2ConfigPath) Then
            MsgBox("The ArmA 2 configurations folder doesn't exists inside the default folder " + vbCrLf + Environment.GetFolderPath(Environment.SpecialFolder.Personal), MsgBoxStyle.Exclamation, "ArmA 2 Config Folder Not Found")
            options = 1                 ' Comunica al form "BrowseFolder" quali label scrivere alla sua apertura
            BrowseFolder.ShowDialog()   ' Apri la finestra per la scelta manuale della cartella contenente i file di profilo di ArmA 2 OA
            If A2ConfigPath = "" Then   ' Se non ho scelto una cartella nel form BrowseFolder
                uscita = True
            End If
        End If

        ' Verifica il percorso di installazione di ArmA 2 OA tramite chiave di registro
        If Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing) Is Nothing Then
            Arma2RegFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing)
        ElseIf Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing) Is Nothing Then
            Arma2RegFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing)
        End If

        'MsgBox(Arma2RegFound)

        ' Verifica la presenza di Steam
        'If Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", Nothing) Is Nothing Then
        '    SteamFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", Nothing)
        '    ToolStripStatusLabel2.Text = "Steam Found!"
        'ElseIf Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", Nothing) Is Nothing Then
        '    SteamFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", Nothing)
        '    ToolStripStatusLabel2.Text = "Steam Found!"
        'End If

        If Not uscita = True Then
            If Not Arma2RegFound = "" Then
                ' Memorizza il percorso assoluto in cui dovrebbe trovarsi il file di 
                ' configurazione di ArmA 2
                A2ConfigFile = A2ConfigPath + "\ArmA2OA.cfg"

                If Not System.IO.File.Exists(A2ConfigFile) Then
                    ' Se il file di configurazione di ArmA 2 OA non viene identificato
                    MsgBox("ArmA 2 OA config file (ArmA2OA.cfg) doesn't exists at location " + A2ConfigFile, MsgBoxStyle.Exclamation, "ArmA 2 OA Config File Not Found")
                    uscita = True   ' Forza l'arresto del programma
                Else
                    ' Se invece viene correttamente identificato il file di configurazione

                    ' Apri e/o crea il file per il salvataggio dei profili
                    Try
                        fileWriter = New IO.StreamWriter(Arma2RegFound + "\A2Launcher.profiles", True)
                        fileWriter.Close()
                    Catch ex As Exception
                        MsgBox("(1) Error while opening the ArmA 2 OA Launcher profiles file:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not open ArmA 2 OA Launcher Profiles File")
                        uscita = True
                    End Try

                    If Not uscita = True Then
                        ' Aggiungi i profili, memorizzati nel file di configurazione del launcher,
                        ' alla ComboBox1
                        AddProfiles()

                        ' Verifica la presenza delle cartelle dei mod di ArmA 2 OA
                        VerifyMod()

                        Timer1.Start()  ' Cancella la scritta della ToolStripStatusLabel1 al suo scadere
                        ToolStripStatusLabel1.Text = folders.Count.ToString + " Mods found!"

                        ' Apri il file di configurazione in lettura
                        Try
                            A2File = New System.IO.StreamReader(A2ConfigFile)

                            Dim riga As String
                            Dim tempMod As String
                            Dim mods As String()
                            Dim checks As String()
                            Dim listaModAttivi As New List(Of String)
                            Dim k As Integer
                            Dim indicatore As Integer = -1
                            Dim listaCompleta As New List(Of String)

                            ' Cerca di identificare un profilo con la stessa lista di mods
                            ' attivi del file di configurazione di ArmA 2 OA (ArmA2OA.cfg)
                            If Not My.Settings.LastProfile = "" And ComboBox1.Items.Contains(My.Settings.LastProfile) Then
                                indicatore = proFile.IndexOf(My.Settings.LastProfile)
                            Else
                                ' Scorri tutti i profili salvati nel file di configurazione del Launcher
                                For i As Integer = 0 To modsList.Count - 1
                                    mods = modsList.Item(i).Split(";")      ' Lista di mod del profilo i-esimo
                                    checks = checked.Item(i).Split(";")     ' Mod attivi del profilo i-esimo

                                    ' Crea una lista (listModAttivi) contenente, in ordine, solo 
                                    ' i mod attivi del profilo considerato
                                    listaModAttivi.Clear()  ' Pulisci la lista dei mod attivi
                                    listaCompleta.Clear()   ' Pulisci la lista di tutti i mods

                                    For j As Integer = 0 To mods.Count - 1
                                        If checks(j) = "true" Then
                                            listaModAttivi.Add(mods(j))
                                        End If
                                    Next j

                                    ' Inverti la lista (nell'ordine da bassa priorità ad alta)
                                    listaModAttivi.Reverse()

                                    ' Resetta l'indice di scorrimento dei mod attivi
                                    k = 0

                                    ' Riporta il flusso di dati del file all'origine
                                    A2File.DiscardBufferedData()
                                    A2File.BaseStream.Seek(0, IO.SeekOrigin.Begin)

                                    If listaModAttivi.Count > 0 Then
                                        While Not A2File.EndOfStream
                                            riga = A2File.ReadLine()
                                            If riga.Contains("dir=") Then
                                                tempMod = riga.Substring(riga.IndexOf("=") + 1)
                                                tempMod = tempMod.Replace("""", "")
                                                tempMod = tempMod.Replace(";", "")

                                                If folders.Contains(tempMod) And Not tempMod = "ACR" Then   ' Se è presente il mod all'interno della cartella di ArmA 2

                                                    listaCompleta.Add(tempMod)

                                                    If Not tempMod = listaModAttivi(k) Then
                                                        indicatore = -1
                                                        Exit While
                                                    Else
                                                        indicatore = i              ' Memorizza la posizione del profilo
                                                        k = k + 1
                                                    End If
                                                    'CheckedListBox1.Items.Insert(0, tempMod)
                                                    'CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(tempMod), True)
                                                End If
                                            End If
                                        End While
                                    End If
                                Next i
                            End If

                            ' Chiudi il file di configurazione di ArmA 2 OA
                            A2File.Close()

                            ' Controlla se ho identificato un profilo esatto
                            If Not indicatore = -1 Then
                                ComboBox1.SelectedItem = proFile(indicatore)
                            Else
                                For Each el As String In listaCompleta
                                    CheckedListBox1.Items.Insert(0, el)
                                    CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(el), True)
                                Next
                                ' Aggiungo alla CheckedListBox1 tutti i mod che non sono
                                ' stati trovati nel file di configurazione di ArmA 2 OA
                                ' ma sono presenti nella cartella di installazione di ArmA 2 OA
                                For Each el As String In folders
                                    If Not CheckedListBox1.Items.Contains(el) Then
                                        CheckedListBox1.Items.Insert(0, el)
                                    End If
                                Next
                            End If

                            ' Compila i textboxes "Multiplayer"
                            If Not My.Settings.IP = "" Then
                                TextBox1.Text = My.Settings.IP
                            End If

                            If Not My.Settings.Port = "" Then
                                TextBox2.Text = My.Settings.Port
                            End If

                            If Not My.Settings.Password = "" Then
                                TextBox3.Text = My.Settings.Password
                            End If

                        Catch ex As Exception
                            MsgBox("Error while opening ArmA 2 OA config file" + vbCrLf + vbCrLf + ex.ToString(), MsgBoxStyle.Critical, "Can not open ArmA 2 OA Config File")
                            uscita = True
                        End Try
                    End If
                End If
            Else
                MsgBox("Can not find ArmA 2 OA installation folder!", MsgBoxStyle.Exclamation, "ArmA 2 OA not found")
                uscita = True
            End If
        End If

        ' Verifica se l'applicazione deve uscire a causa di un errore
        If uscita = True Then
            Application.Exit()
        Else
            ' Inizializza la ComboBox dei Memory Allocators
            If System.IO.Directory.Exists(Arma2RegFound + "\dll") Then
                ' Se esiste la cartella di default degli allocators
                For Each d As String In System.IO.Directory.GetFiles(Arma2RegFound + "\dll")
                    ' Elenca tutti i file in esso presenti
                    If System.IO.Path.GetExtension(d) = ".dll" Then
                        ' Aggiungi i nomi di file alla ComboBox7
                        ComboBox7.Items.Add(System.IO.Path.GetFileNameWithoutExtension(d))
                    End If
                Next
                If ComboBox7.Items.Count > 0 Then
                    ComboBox7.SelectedIndex = 0
                    'Else
                    'ComboBox7.Enabled = False
                    'CheckBox11.Enabled = False
                End If
                'Else
                'ComboBox7.Enabled = False
                'CheckBox11.Enabled = False
            End If
            ComboBox7.Items.Add("system")

            ' Inizializza la ComboBox dei profili
            If System.IO.Directory.Exists(A2ConfigPath) Then
                ' Se esiste la cartella di default dei profili
                For Each d As String In System.IO.Directory.GetFiles(A2ConfigPath)
                    ' Elenca tutti i file in esso presenti
                    If System.IO.Path.GetExtension(d) = ".ArmA2OAProfile" Then
                        ' Aggiungi i nomi di file alla ComboBox7
                        If Not d.Contains(".vars") Then
                            ComboBox8.Items.Add(System.IO.Path.GetFileNameWithoutExtension(d))
                        End If
                    End If
                Next
                If ComboBox8.Items.Count > 0 Then
                    ComboBox8.SelectedIndex = 0
                Else
                    ComboBox8.Enabled = False
                    CheckBox19.Enabled = False
                End If
            Else
                ComboBox8.Enabled = False
                CheckBox19.Enabled = False
            End If

            ' Inizializza i parametri di startup in base a come sono salvati
            ' nel file di configurazione del programma
            CheckParameters()
        End If
    End Sub

    Private Sub VerifyMod()
        ' Popola la lista "folders" con il nome di tutte le cartelle dei mods presenti
        ' nella cartella principale di ArmA 2 OA
        folders.Clear()         ' Resetta la lista "folders"
        folders_doc.Clear()     ' Resetta la lista "folders_doc"
        For Each d In System.IO.Directory.GetDirectories(Arma2RegFound)
            ' Verifica la presenza della sottodirectory "Addons" nella directory "d" presa in esame
            If System.IO.Directory.Exists(d + "\addons") Then
                Dim dirInfo As New System.IO.DirectoryInfo(d)   ' Estrapola solo il nome della cartella dalla directory
                Dim tempFold As String = dirInfo.Name           ' Estrapola solo il nome della cartella dalla directory e salvalo nella variabile "tempFold"
                ' Non considerare le cartelle che non fanno parte dei mods (ACR, BAF, PMC ed Expansion)
                If Not tempFold = "ACR" And Not tempFold = "BAF" And Not tempFold = "PMC" And Not tempFold = "Expansion" Then
                    folders.Add(tempFold)
                    'CheckedListBox1.Items.Add(tempFold)
                End If
            End If
        Next

        ' Controlla per mods presenti anche nella cartella "users\My Documents\ArmA 2"
        For Each d In System.IO.Directory.GetDirectories(A2ConfigPath)
            ' Verifica la presenza della sottodirectory "Addons" nella directory "d" presa in esame
            If System.IO.Directory.Exists(d + "\addons") Then
                Dim dirInfo As New System.IO.DirectoryInfo(d)   ' Estrapola solo il nome della cartella dalla directory
                Dim tempFold As String = dirInfo.Name           ' Estrapola solo il nome della cartella dalla directory e salvalo nella variabile "tempFold"
                ' Non considerare le cartelle che non fanno parte dei mods (ACR, BAF, PMC ed Expansion)
                If Not tempFold = "ACR" And Not tempFold = "BAF" And Not tempFold = "PMC" And Not tempFold = "Expansion" Then
                    folders_doc.Add(tempFold)
                    folders.Add(tempFold)
                    'CheckedListBox1.Items.Add(tempFold)
                End If
            End If
        Next
    End Sub

    Private Sub AddProfiles()
        Try
            ComboBox1.Items.Clear()

            ' Resetta tutte le liste
            proFile.Clear()
            modsList.Clear()
            checked.Clear()
            startUp.Clear()

            fileReader = New IO.StreamReader(Arma2RegFound + "\A2Launcher.profiles")
            Dim temp As String

            While Not fileReader.EndOfStream
                temp = fileReader.ReadLine()
                proFile.Add(temp)                   ' Aggiungi alla lista proFile il nome del profilo
                ComboBox1.Items.Add(temp)           ' Aggiungi il profilo al ComboBox1

                modsList.Add(fileReader.ReadLine()) ' Aggiungi alla lista modsList la lista dei mod del profilo
                checked.Add(fileReader.ReadLine())  ' Aggiungi alla lista checked la lista dei mod selezionati del profilo
                startUp.Add(fileReader.ReadLine())  ' Aggiungi alla lista startup la lista dei parametri di avvio di ArmA 2
            End While

            fileReader.Close()
        Catch ex As Exception
            MsgBox("Error while opening the ArmA 2 OA Launcher profile file:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Critical, "Can not open ArmA 2 OA Launcher Profile File")
            Exit Sub
        End Try
    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        ' Sposta il mod selezionato alla più alta priorità
        If CheckedListBox1.SelectedItems.Count > 0 Then
            ' Se ho selezionato almeno un elemento nella checkedlistbox1
            If CheckedListBox1.SelectedIndex > 0 Then
                Dim indice As Integer = CheckedListBox1.SelectedIndex
                Dim oggetto As String = CheckedListBox1.SelectedItem
                Dim check As Boolean = CheckedListBox1.GetItemChecked(indice)

                ' Riordina la lista delle cartelle dei mod
                If folders.Contains(oggetto) Then
                    folders.RemoveAt(indice)
                    folders.Insert(0, oggetto)
                End If

                ' Ripopola la CheckedListBox1
                CheckedListBox1.Items.RemoveAt(indice)
                CheckedListBox1.Items.Insert(0, oggetto)
                If check = True Then
                    CheckedListBox1.SetItemChecked(0, True)
                End If

                ' Riseleziona l'oggetto selezionato precedentemente
                CheckedListBox1.SelectedItem = oggetto
            End If
        End If

    End Sub

    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        ' Bottone "Sposta a priorità maggiore"
        If CheckedListBox1.SelectedItems.Count > 0 Then
            ' Se ho selezionato almeno un elemento nella checkedlistbox1
            If CheckedListBox1.SelectedIndex > 0 Then
                Dim indice As Integer = CheckedListBox1.SelectedIndex
                Dim oggetto As String = CheckedListBox1.SelectedItem
                Dim check As Boolean = CheckedListBox1.GetItemChecked(indice)
                'MsgBox("Selezionato: " + indice.ToString + " - " + oggetto)

                ' Riordina la lista delle cartelle dei mod
                folders.RemoveAt(indice)
                folders.Insert(indice - 1, oggetto)

                CheckedListBox1.Items.RemoveAt(indice)
                CheckedListBox1.Items.Insert(indice - 1, oggetto)
                If check = True Then
                    CheckedListBox1.SetItemChecked(indice - 1, True)
                End If

                ' Riseleziona l'oggetto selezionato precedentemente
                CheckedListBox1.SelectedItem = oggetto
            End If
        End If
    End Sub

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        ' Bottone "Sposta a priorità inferiore"
        If CheckedListBox1.SelectedItems.Count > 0 Then
            ' Se ho selezionato almeno un elemento nella checkedlistbox1

            Dim indice As Integer = CheckedListBox1.SelectedIndex
            Dim oggetto As String = CheckedListBox1.SelectedItem
            Dim check As Boolean = CheckedListBox1.GetItemChecked(indice)

            'MsgBox("Selezionato: " + indice.ToString + " - " + oggetto)

            If Not CheckedListBox1.SelectedIndex = folders.Count - 1 Then
                ' Riordina la lista delle cartelle dei mod
                folders.RemoveAt(indice)

                'If CheckedListBox1.SelectedIndex = folders.Count Then
                'MsgBox("Primo:" + CheckedListBox1.SelectedIndex.ToString + "/" + (folders.Count).ToString)
                'folders.Add(oggetto)
                'Else
                folders.Insert(indice + 1, oggetto)
                'End If

                CheckedListBox1.Items.RemoveAt(indice)
                CheckedListBox1.Items.Insert(indice + 1, oggetto)
                If check = True Then
                    CheckedListBox1.SetItemChecked(indice + 1, True)
                End If

                ' Riseleziona l'oggetto selezionato precedentemente
                CheckedListBox1.SelectedItem = oggetto
            End If
        End If
    End Sub

    Private Sub PictureBox8_Click(sender As Object, e As EventArgs) Handles PictureBox8.Click
        ' Bottone "Sposta a priorità minima"
        If CheckedListBox1.SelectedItems.Count > 0 Then
            ' Se ho selezionato almeno un elemento nella checkedlistbox1

            Dim indice As Integer = CheckedListBox1.SelectedIndex
            Dim oggetto As String = CheckedListBox1.SelectedItem
            Dim check As Boolean = CheckedListBox1.GetItemChecked(indice)

            'MsgBox("Selezionato: " + indice.ToString + " - " + oggetto)

            If Not CheckedListBox1.SelectedIndex = folders.Count - 1 Then
                ' Riordina la lista delle cartelle dei mod
                folders.RemoveAt(indice)

                'If CheckedListBox1.SelectedIndex = folders.Count Then
                'MsgBox("Primo:" + CheckedListBox1.SelectedIndex.ToString + "/" + (folders.Count).ToString)
                folders.Add(oggetto)
                'Else
                'folders.Insert(indice + 1, oggetto)
                'End If

                CheckedListBox1.Items.RemoveAt(indice)
                CheckedListBox1.Items.Add(oggetto)
                If check = True Then
                    CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
                End If

                ' Riseleziona l'oggetto selezionato precedentemente
                CheckedListBox1.SelectedItem = oggetto
            End If
        End If
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        ' Seleziona il profilo preferito

        If PictureBox2.Tag.ToString = "Grey" Then
            PictureBox2.BackgroundImage = My.Resources.Favorites
            PictureBox2.Tag = "Yellow"
            My.Settings.Profile = ComboBox1.Text.Trim
        Else
            PictureBox2.BackgroundImage = My.Resources.Favorites_Grey
            PictureBox2.Tag = "Grey"
            My.Settings.Profile = ""
        End If

        My.Settings.Save()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' Alla selezione di un profilo dalla ComboBox1
        ' Verifica se è stato impostato un profilo preferito
        ' e nel caso selezionalo nella ComboBox1
        'If My.Settings.Profile = ComboBox1.SelectedItem.ToString Then
        '    PictureBox2.BackgroundImage = My.Resources.Favorites
        '    PictureBox2.Tag = "Yellow"
        'Else
        '    PictureBox2.BackgroundImage = My.Resources.Favorites_Grey
        '    PictureBox2.Tag = "Grey"
        'End If
        Dim nome As String = ComboBox1.SelectedItem
        Dim indice As Integer
        Dim mods As String()
        Dim check As String()


        If proFile.Contains(nome) Then                          ' Ulteriore controllo per verificare che l'utente non abbia cancellato un profilo (raro)
            indice = proFile.IndexOf(nome)
            mods = modsList(indice).Split(";")
            check = checked(indice).Split(";")

            VerifyMod()                                         ' Ricontrolla la presenza delle cartelle dei mod nella directory di ArmA 2 OA
            CheckedListBox1.Items.Clear()                       ' Cancella tutti gli elementi

            For i As Integer = 0 To mods.Count - 1
                If folders.Contains(mods(i)) Then
                    indice = CheckedListBox1.Items.Add(mods(i))
                    If check(i) = "true" Then
                        CheckedListBox1.SetItemChecked(indice, True)
                    End If
                End If
            Next

            ' Aggiungo alla CheckedListBox1 tutti i mod che non sono
            ' stati trovati nel file dei profili del launcher
            ' ma sono presenti nella cartella di installazione di ArmA 2 OA
            For Each el As String In folders
                If Not CheckedListBox1.Items.Contains(el) Then
                    CheckedListBox1.Items.Insert(0, el)
                End If
            Next

            ' Ripopola la variabile "folders" con il corretto ordine dei mod
            folders.Clear()
            For Each el As String In CheckedListBox1.Items
                folders.Add(el)
            Next
        Else
            MsgBox("The profile """ + nome + """ doesn't seem to exists in the ArmA 2 OA Launcher Profile file!", MsgBoxStyle.Critical, "Profile not found")
        End If

    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        ' Salvataggio del profilo
        Dim nome As String = ComboBox1.Text.Trim
        Dim test As String = ""
        Dim line As String = ""
        Dim mods As String = ""
        Dim check As String = ""

        If nome = "" Then
            MsgBox("Can not save with an empty profile name!", MsgBoxStyle.Exclamation, "Profile's name not found")
            Exit Sub
        Else
            ' Se il nome nella ComboBox1.Text è valido, procedi al salvataggio del profilo

            ' Ricostruisci la stringa dei mod presenti nella lista
            For Each el As String In CheckedListBox1.Items
                mods = mods + el + ";"
                If CheckedListBox1.CheckedItems.Contains(el) Then
                    ' Se il mod è anche spuntato
                    check = check + "true;"
                Else
                    check = check + "false;"
                End If
            Next

            ' Verifica se esiste un altro profilo con lo stesso nome
            If proFile.Contains(nome) Then
                ' Sovrascrivi le impostazioni del profilo già presente, con le nuove
                Dim indice As Integer = proFile.IndexOf(nome)   ' Rileva l'indice del profilo già presente

                fileReader = New System.IO.StreamReader(Arma2RegFound + "\A2Launcher.profiles")   ' Apri in lettura il file dei profili

                While Not fileReader.EndOfStream
                    line = fileReader.ReadLine()                ' Leggi e memorizza la corrente linea dal file

                    If line = nome Then
                        ' Se ho individuato la riga del profilo da sovrascrivere
                        If test = "" Then
                            test = line + vbCrLf                     ' Riscrivi il nome del profilo
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + mods + vbCrLf                     ' Scrivi la lista dei mod
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + check + vbCrLf                    ' Scrivi la lista dei mod selezionati
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + "startup parameters"              ' Scrivi la riga dei parametri di startup
                        Else
                            test = test + vbCrLf + line
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + vbCrLf + mods                     ' Scrivi la lista dei mod
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + vbCrLf + check                    ' Scrivi la lista dei mod selezionati
                            fileReader.ReadLine()                           ' Scorri alla riga successiva
                            test = test + vbCrLf + "startup parameters"     ' Scrivi la riga dei parametri di startup
                        End If
                    Else
                        If test = "" Then
                            test = line                                     ' Riscrivi il nome del profilo
                        Else
                            test = test + vbCrLf + line
                        End If
                    End If

                End While

                fileReader.Close()                              ' Chiudi il file in lettura

                fileWriter = New System.IO.StreamWriter(Arma2RegFound + "\A2Launcher.profiles", False)
                fileWriter.Write(test)
                fileWriter.Close()

                AddProfiles()

                ToolStripStatusLabel1.Text = "Profile Saved!"

                Timer1.Start()
            Else
                ' In caso invece di un nuovo profilo

                ' Inserisco il nuovo profilo in cima alla lista dei profili
                test = nome + vbCrLf
                test = test + mods + vbCrLf
                test = test + check + vbCrLf
                test = test + "startup parameters"

                fileReader = New System.IO.StreamReader(Arma2RegFound + "\A2Launcher.profiles")   ' Apri in lettura il file dei profili

                While Not fileReader.EndOfStream
                    test = test + vbCrLf + fileReader.ReadLine()
                End While

                fileReader.Close()

                fileWriter = New System.IO.StreamWriter(Arma2RegFound + "\A2Launcher.profiles", False)
                fileWriter.Write(test)
                fileWriter.Close()

                AddProfiles()

                ToolStripStatusLabel1.Text = "Profile Saved!"

                Timer1.Start()
            End If
        End If
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        ' Bottone di cancellazione del profilo
        Dim nome As String = ComboBox1.Text.Trim
        Dim test As String = ""
        Dim line As String

        If Not nome = "" Then
            If proFile.Contains(nome) Then
                If MsgBox("Do you really want to delete the profile named """ + nome + """?", MsgBoxStyle.OkCancel, "Profile Delete") = MsgBoxResult.Ok Then
                    ' Cancellazione del profilo

                    fileReader = New System.IO.StreamReader(Arma2RegFound + "\A2Launcher.profiles")   ' Apri in lettura il file dei profili

                    While Not fileReader.EndOfStream
                        line = fileReader.ReadLine()
                        If line = nome Then
                            fileReader.ReadLine()
                            fileReader.ReadLine()
                            fileReader.ReadLine()
                        Else
                            test = test + line + vbCrLf
                        End If
                    End While

                    fileReader.Close()

                    fileWriter = New System.IO.StreamWriter(Arma2RegFound + "\A2Launcher.profiles", False)
                    fileWriter.Write(test)
                    fileWriter.Close()

                    AddProfiles()

                    ComboBox1.ResetText()

                    ToolStripStatusLabel1.Text = "Profile Deleted!"

                    Timer1.Start()
                End If
            Else
                ' Se il nome del profilo scritto nella ComboBox1.Text
                ' non compare nell'elenco dei profili salvati
                ' resetta solamente la casella di testo
                ComboBox1.ResetText()
            End If
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Allo scadere del tempo impostato per il Timer1, resetta la ToolStripStatusLabel1
        Timer1.Stop()
        ToolStripStatusLabel1.Text = "ArmA 2 OA Launcher"
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Avvio di ArmA 2
        Dim test As String = ""
        Dim line As String
        Dim indice As Integer = 1
        Dim lista As New List(Of String)
        Dim nome As String = ComboBox1.Text.Trim

        ' Riscrittura del file di configurazione di ArmA 2 OA

        fileReader = New System.IO.StreamReader(A2ConfigFile)

        test = fileReader.ReadLine()    ' Leggi la prima riga del file di configurazione di A2OA

        While Not fileReader.EndOfStream()
            line = fileReader.ReadLine()
            If Not line.Contains("class ModLauncherList") Then
                test = test + vbCrLf + line
            Else
                Exit While
            End If
        End While

        fileReader.Close()

        test = test + vbCrLf + "class ModLauncherList" + vbCrLf + "{" + vbCrLf

        ' Verifica se esiste il mod ACR
        If System.IO.Directory.Exists(Arma2RegFound + "\ACR\addons") Then
            test = test + vbTab + "class Mod" + indice.ToString + vbCrLf + vbTab + "{" + vbCrLf + vbTab + vbTab + "dir=""ACR"";" + vbCrLf + vbTab + vbTab + "name=""Arma 2: Army of The Czech Republic"";" + vbCrLf + vbTab + vbTab + "origin=""REGISTRY"";" + vbCrLf + vbTab + vbTab + "fullPath=""" + Arma2RegFound + "\ACR"";" + vbCrLf + vbTab + "};"
            indice = indice + 1
        End If

        ' Memorizza gli elementi selezionati nella CheckedListBox1 nella variabile "lista"
        For Each el As String In CheckedListBox1.CheckedItems
            lista.Add(el)
        Next

        ' Rovescia la lista
        lista.Reverse()

        ' Ricomponi l'intero testo del file di configurazione di ArmA 2 OA
        For Each el As String In lista
            test = test + vbCrLf + vbTab + "class Mod" + indice.ToString + vbCrLf + vbTab + "{" + vbCrLf

            indice = indice + 1

            test = test + vbTab + vbTab + "dir=""" + el + """;" + vbCrLf
            test = test + vbTab + vbTab + "name="""";" + vbCrLf
            test = test + vbTab + vbTab + "origin=""GAME DIR"";" + vbCrLf
            If folders_doc.Contains(el) Then
                ' Se il mod si trova nella nella cartella dei documenti
                test = test + vbTab + vbTab + "fullPath=""" + A2ConfigPath + "\" + el + """;" + vbCrLf
            Else
                ' Se il mod si trova nella cartella di installazione di ArmA 2 OA
                test = test + vbTab + vbTab + "fullPath=""" + Arma2RegFound + "\" + el + """;" + vbCrLf
            End If
            test = test + vbTab + "};"
        Next

        test = test + vbCrLf + "};"

        ' Salva la lista dei mod nel file di configurazione di ArmA 2 OA
        Try
            fileWriter = New System.IO.StreamWriter(A2ConfigFile, False)
            fileWriter.Write(test)
            fileWriter.Close()
        Catch ex As Exception
            MsgBox("Error writing ArmA 2 OA Config File:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Critical, "Error while saving config file")
            Exit Sub
        End Try

        ' Verifica se si sta facendo partire ArmA 2 OA con un determinato profilo
        ' e che il profilo presente nella textbox del ComboBox1 sia effettivamente
        ' un profilo valido
        If Not nome = "" And ComboBox1.Items.Contains(nome) Then
            My.Settings.LastProfile = nome
            My.Settings.Save()
        Else
            My.Settings.LastProfile = ""
            My.Settings.Save()
        End If

        ' Lancia l'applicazione con i parametri selezionati
        Dim pHelp As New System.Diagnostics.Process()
        Dim process As New System.Diagnostics.Process()

        ' Identifica quale eseguibile è presente nella cartella di installazione di 
        ' ArmA 2 OA
        If System.IO.File.Exists(Arma2RegFound + "\ArmA2OA_BE.exe") Then
            pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA_BE.exe"
            pHelp.StartInfo.Arguments = "0 0 "
        ElseIf System.IO.File.Exists(Arma2RegFound + "\ArmA2OA.exe") Then
            pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA.exe"
            pHelp.StartInfo.Arguments = ""
        Else
            ' Nessun eseguibile valido trovato
            MsgBox("Can not find ArmA 2 OA executables!", MsgBoxStyle.Exclamation, "ArmA 2 OA .exe not found")
            Exit Sub
        End If

        ' Componi la stringa con i parametri di startup
        pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + StartupParameters()

        ' Salva i parametri di startup
        SaveParameters()

        'pHelp.StartInfo.UseShellExecute = True
        'pHelp.StartInfo.WindowStyle = ProcessWindowStyle.Normal

        Try
            ToolStripStatusLabel1.Text = "Launching ArmA 2 OA..."
            pHelp.Start()

            Do
                Application.DoEvents()
            Loop While Not System.Diagnostics.Process.GetProcessesByName("ArmA2OA").Length > 0

            process = System.Diagnostics.Process.GetProcessesByName("ArmA2OA")(0)

            If ComboBox2.SelectedItem = "High" Then
                process.PriorityClass = ProcessPriorityClass.High
            End If

            ' Blocca i controlli finchè ArmA 2 è in funzionamento
            Timer1.Stop()               ' Ferma l'eventuale
            ToolStripStatusLabel1.Text = "ArmA 2 OA is running"
            Button1.Enabled = False
            Button2.Enabled = False
            Button3.Enabled = False
            TextBox1.ReadOnly = True
            TextBox2.ReadOnly = True
            TextBox3.ReadOnly = True

            ' Avvia il Timer2 che controlla la fine dell'esecuzione del processo di ArmA 2
            Timer2.Enabled = True

            'Do
            '    Application.DoEvents()
            'Loop While Not process.HasExited

            '' Riattiva i controlli dopo la chiusura di ArmA 2
            'ToolStripStatusLabel1.Text = "ArmA 2 OA Launcher"
            'Button1.Enabled = True
            'Button2.Enabled = True
            'Button3.Enabled = True
            'TextBox1.ReadOnly = False
            'TextBox2.ReadOnly = False
            'TextBox3.ReadOnly = False
        Catch ex As Exception
            MsgBox("Error while launching ArmA 2 OA:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not launch ArmA 2 OA")
        End Try
    End Sub

    Private Sub PictureBox9_Click(sender As Object, e As EventArgs) Handles PictureBox9.Click
        ' Aggiornamento della CheckedListBox1 con tutte le cartelle dei Mods presenti
        ' nella cartella principale di ArmA 2 OA
        Dim uscita As Boolean = False
        Dim indicatore As Integer = -1

        ' Memorizza il percorso in cui dovrebbe trovarsi la cartella delle configurazioni di ArmA 2 OA
        If My.Settings.A2ConfigPath = "" Then
            A2ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\ArmA 2"
        Else
            A2ConfigPath = My.Settings.A2ConfigPath
        End If

        ' Controlla per verificare l'esistenza della cartella del profilo di ArmA 2
        If Not System.IO.Directory.Exists(A2ConfigPath) Then
            MsgBox("The ArmA 2 configurations folder doesn't exists inside the default folder " + vbCrLf + Environment.GetFolderPath(Environment.SpecialFolder.Personal), MsgBoxStyle.Exclamation, "ArmA 2 Config Folder Not Found")
            options = 1                 ' Comunica al form "BrowseFolder" quali label scrivere alla sua apertura
            BrowseFolder.ShowDialog()   ' Apri la finestra per la scelta manuale della cartella contenente i file di profilo di ArmA 2 OA
            If A2ConfigPath = "" Then   ' Se non ho scelto una cartella nel form BrowseFolder
                Exit Sub
            End If
        End If

        ' Verifica il percorso di installazione di ArmA 2 OA tramite chiave di registro
        If Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing) Is Nothing Then
            Arma2RegFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing)
        ElseIf Not My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing) Is Nothing Then
            Arma2RegFound = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", Nothing)
        End If

        If Not Arma2RegFound = "" Then
            ' Se invece viene identificata la cartella corretta di ArmA 2

            ' Memorizza il percorso assoluto in cui dovrebbe trovarsi il file di 
            ' configurazione di ArmA 2
            A2ConfigFile = A2ConfigPath + "\ArmA2OA.cfg"

            If Not System.IO.File.Exists(A2ConfigFile) Then
                ' Se il file di configurazione di ArmA 2 OA non viene identificato
                MsgBox("ArmA 2 OA config file doesn't exists at location " + A2ConfigFile, MsgBoxStyle.Exclamation, "ArmA 2 OA Config File Not Found")
                Exit Sub
            Else
                ' Se invece viene correttamente identificato il file di configurazione

                ' Apri e/o crea il file per il salvataggio dei profili
                Try
                    fileWriter = New IO.StreamWriter(Arma2RegFound + "\A2Launcher.profiles", True)
                    fileWriter.Close()
                Catch ex As Exception
                    MsgBox("(1) Error while opening the ArmA 2 OA Launcher profiles file:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not open ArmA 2 OA Launcher Profiles File")
                    Exit Sub
                End Try

                ' Memorizza l'eventuale profilo già selezionato nella ComboBox1
                If Not ComboBox1.SelectedItem = Nothing Then
                    indicatore = ComboBox1.SelectedIndex
                End If

                ' Aggiungi i profili, memorizzati nel file di configurazione del launcher,
                ' alla ComboBox1
                AddProfiles()

                ' Verifica la presenza delle cartelle dei mod di ArmA 2 OA
                VerifyMod()

                Timer1.Start()
                ToolStripStatusLabel1.Text = folders.Count.ToString + " Mods found!"

                ' Apri il file di configurazione in lettura

                Try
                    A2File = New System.IO.StreamReader(A2ConfigFile)

                    Dim riga As String
                    Dim tempMod As String
                    Dim mods As String()
                    Dim checks As String()
                    Dim listaModAttivi As New List(Of String)
                    Dim k As Integer
                    Dim listaCompleta As New List(Of String)

                    ' Cerca di identificare un profilo con la stessa lista di mods
                    ' attivi del file di configurazione di ArmA 2 OA

                    If Not My.Settings.LastProfile = "" And ComboBox1.Items.Contains(My.Settings.LastProfile) And indicatore = -1 Then
                        indicatore = proFile.IndexOf(My.Settings.LastProfile)
                    ElseIf indicatore = -1 Then
                        For i As Integer = 0 To modsList.Count - 1
                            mods = modsList.Item(i).Split(";")
                            checks = checked.Item(i).Split(";")

                            ' Crea una lista (listModAttivi) contenente, in ordine, solo 
                            ' i mod attivi del profilo considerato
                            listaModAttivi.Clear()  ' Pulisci la lista dei mod attivi
                            listaCompleta.Clear()   ' Pulisci la lista di tutti i mods

                            For j As Integer = 0 To mods.Count - 1
                                If checks(j) = "true" Then
                                    listaModAttivi.Add(mods(j))
                                End If
                            Next j

                            ' Inverti la lista
                            listaModAttivi.Reverse()

                            ' Resetta l'indice di scorrimento dei mod attivi
                            k = 0

                            ' Riporta il flusso di dati del file all'origine
                            A2File.DiscardBufferedData()
                            A2File.BaseStream.Seek(0, IO.SeekOrigin.Begin)

                            If listaModAttivi.Count > 0 Then
                                While Not A2File.EndOfStream
                                    riga = A2File.ReadLine()
                                    If riga.Contains("dir=") Then
                                        tempMod = riga.Substring(riga.IndexOf("=") + 1)
                                        tempMod = tempMod.Replace("""", "")
                                        tempMod = tempMod.Replace(";", "")

                                        If folders.Contains(tempMod) And Not tempMod = "ACR" Then   ' Se è presente il mod all'interno della cartella di ArmA 2

                                            listaCompleta.Add(tempMod)

                                            If Not tempMod = listaModAttivi(k) Then
                                                indicatore = -1
                                                Exit While
                                            Else
                                                indicatore = i              ' Memorizza la posizione del profilo
                                                k = k + 1
                                            End If
                                            'CheckedListBox1.Items.Insert(0, tempMod)
                                            'CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(tempMod), True)
                                        End If
                                    End If
                                End While
                            End If
                        Next i
                    End If

                    A2File.Close()

                    ' Controlla se ho identificato un profilo esatto
                    If Not indicatore = -1 Then
                        ComboBox1.SelectedItem = proFile(indicatore)
                    Else
                        CheckedListBox1.Items.Clear()           ' Resetta la lista dei mods
                        For Each el As String In listaCompleta
                            CheckedListBox1.Items.Insert(0, el)
                            CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(el), True)
                        Next
                        ' Aggiungo alla CheckedListBox1 tutti i mod che non sono
                        ' stati trovati nel file di configurazione di ArmA 2 OA
                        ' ma sono presenti nella cartella di installazione di ArmA 2 OA
                        For Each el As String In folders
                            If Not CheckedListBox1.Items.Contains(el) Then
                                CheckedListBox1.Items.Insert(0, el)
                            End If
                        Next
                    End If
                Catch ex As Exception
                    MsgBox("(2) Error while opening ArmA 2 OA config file" + vbCrLf + vbCrLf + ex.ToString(), MsgBoxStyle.Critical, "Can not open ArmA 2 OA Config File")
                    Exit Sub
                End Try
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Prova a far partire ArmA 2 OA senza addons
        Dim test As String = ""
        Dim line As String
        Dim lista As New List(Of String)
        Dim nome As String = ComboBox1.Text.Trim

        ' Riscrittura del file di configurazione di ArmA 2 OA

        fileReader = New System.IO.StreamReader(A2ConfigFile)

        test = fileReader.ReadLine()    ' Leggi la prima riga del file di configurazione di A2OA

        While Not fileReader.EndOfStream()
            line = fileReader.ReadLine()
            If Not line.Contains("class ModLauncherList") Then
                test = test + vbCrLf + line
            Else
                Exit While
            End If
        End While

        fileReader.Close()

        test = test + vbCrLf + "class ModLauncherList" + vbCrLf + "{" + vbCrLf

        ' Verifica se esiste il mod ACR e nel caso abilitalo
        If System.IO.Directory.Exists(Arma2RegFound + "\ACR\addons") Then
            test = test + vbTab + "class Mod1" + vbCrLf + vbTab + "{" + vbCrLf + vbTab + vbTab + "dir=""ACR"";" + vbCrLf + vbTab + vbTab + "name=""Arma 2: Army of The Czech Republic"";" + vbCrLf + vbTab + vbTab + "origin=""REGISTRY"";" + vbCrLf + vbTab + vbTab + "fullPath=""" + Arma2RegFound + "\ACR"";" + vbCrLf + vbTab + "};"
        End If

        test = test + vbCrLf + "};"

        ' Salva il file di configurazione
        Try
            fileWriter = New System.IO.StreamWriter(A2ConfigFile, False)
            fileWriter.Write(test)
            fileWriter.Close()
        Catch ex As Exception
            MsgBox("Error writing ArmA 2 OA Config File:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Critical, "Error while saving config file")
        End Try

        ' Lancia l'applicazione con i parametri selezionati
        Dim pHelp As New System.Diagnostics.Process()
        Dim process As New System.Diagnostics.Process()

        ' Identifica quale eseguibile è presente nella cartella di installazione di 
        ' ArmA 2 OA
        If System.IO.File.Exists(Arma2RegFound + "\ArmA2OA_BE.exe") Then
            pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA_BE.exe"
            pHelp.StartInfo.Arguments = "0 0 "
        ElseIf System.IO.File.Exists(Arma2RegFound + "\ArmA2OA.exe") Then
            pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA.exe"
            pHelp.StartInfo.Arguments = ""
        Else
            ' Nessun eseguibile valido trovato
            MsgBox("Can not find ArmA 2 OA executables!", MsgBoxStyle.Exclamation, "ArmA 2 OA not found")
            Exit Sub
        End If

        ' Componi la stringa con i parametri di startup
        pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + StartupParameters()

        ' Salva i parametri di startup
        SaveParameters()

        Try
            ToolStripStatusLabel1.Text = "Launching ArmA 2 OA..."
            pHelp.Start()

            Do
                Application.DoEvents()
            Loop While Not System.Diagnostics.Process.GetProcessesByName("ArmA2OA").Length > 0

            process = System.Diagnostics.Process.GetProcessesByName("ArmA2OA")(0)

            If ComboBox2.SelectedItem = "High" Then
                process.PriorityClass = ProcessPriorityClass.High
            End If

            ' Blocca i controlli finchè ArmA 2 è in funzionamento
            Timer1.Stop()               ' Ferma l'eventuale timer avviato
            ToolStripStatusLabel1.Text = "ArmA 2 OA is running"
            Button1.Enabled = False
            Button2.Enabled = False
            Button3.Enabled = False
            TextBox1.ReadOnly = True
            TextBox2.ReadOnly = True
            TextBox3.ReadOnly = True

            ' Avvia il Timer2 che controlla la fine dell'esecuzione del processo di ArmA 2
            Timer2.Enabled = True

            'Do
            '    Application.DoEvents()
            'Loop While Not process.HasExited

            '' Riattiva i controlli dopo la chiusura di ArmA 2
            'ToolStripStatusLabel1.Text = "ArmA 2 OA Launcher"
            'Button1.Enabled = True
            'Button2.Enabled = True
            'Button3.Enabled = True
            'TextBox1.ReadOnly = False
            'TextBox2.ReadOnly = False
            'TextBox3.ReadOnly = False
        Catch ex As Exception
            MsgBox("Error while launching ArmA 2 OA:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not launch ArmA 2 OA")
        Finally
            If Not pHelp Is Nothing Then
                pHelp.Close()
            End If
            If Not process Is Nothing Then
                process.Close()
            End If
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        ' Abilita il tasto di "Join Game"
        If TextBox1.Text.Trim = "" Then
            Button3.Enabled = False
        Else
            Button3.Enabled = True
        End If
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        ' Permette di accettare solo numeri ed il punto nella TextBox1
        If Asc(e.KeyChar) <> 8 And Not Asc(e.KeyChar) = 46 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        ' Permette di accettare solo numeri nella TextBox2
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Avvia ArmA 2 OA in modalità multiplayer
        If System.Net.IPAddress.TryParse(TextBox1.Text.Trim, Nothing) Then
            ' Se l'indirizzo IP sembra essere corretto allora avvia ArmA 2 OA in modalità multiplayer
            Dim test As String = ""
            Dim line As String
            Dim indice As Integer = 1
            Dim lista As New List(Of String)
            Dim nome As String = ComboBox1.Text.Trim

            ' Riscrittura del file di configurazione di ArmA 2 OA

            fileReader = New System.IO.StreamReader(A2ConfigFile)

            test = fileReader.ReadLine()    ' Leggi la prima riga del file di configurazione di A2OA

            While Not fileReader.EndOfStream()
                line = fileReader.ReadLine()
                If Not line.Contains("class ModLauncherList") Then
                    test = test + vbCrLf + line
                Else
                    Exit While
                End If
            End While

            fileReader.Close()

            test = test + vbCrLf + "class ModLauncherList" + vbCrLf + "{" + vbCrLf

            ' Verifica se esiste il mod ACR
            If System.IO.Directory.Exists(Arma2RegFound + "\ACR\addons") Then
                test = test + vbTab + "class Mod" + indice.ToString + vbCrLf + vbTab + "{" + vbCrLf + vbTab + vbTab + "dir=""ACR"";" + vbCrLf + vbTab + vbTab + "name=""Arma 2: Army of The Czech Republic"";" + vbCrLf + vbTab + vbTab + "origin=""REGISTRY"";" + vbCrLf + vbTab + vbTab + "fullPath=""" + Arma2RegFound + "\ACR"";" + vbCrLf + vbTab + "};"
                indice = indice + 1
            End If

            ' Memorizza gli elementi selezionati nella CheckedListBox1 nella variabile "lista"
            For Each el As String In CheckedListBox1.CheckedItems
                lista.Add(el)
            Next

            ' Rovescia la lista
            lista.Reverse()

            ' Ricomponi l'intero testo del file di configurazione di ArmA 2 OA
            For Each el As String In lista
                test = test + vbCrLf + vbTab + "class Mod" + indice.ToString + vbCrLf + vbTab + "{" + vbCrLf

                indice = indice + 1

                test = test + vbTab + vbTab + "dir=""" + el + """;" + vbCrLf
                test = test + vbTab + vbTab + "name="""";" + vbCrLf
                test = test + vbTab + vbTab + "origin=""GAME DIR"";" + vbCrLf
                If folders_doc.Contains(el) Then
                    ' Se il mod si trova nella nella cartella dei documenti
                    test = test + vbTab + vbTab + "fullPath=""" + A2ConfigPath + "\" + el + """;" + vbCrLf
                Else
                    ' Se il mod si trova nella cartella di installazione di ArmA 2 OA
                    test = test + vbTab + vbTab + "fullPath=""" + Arma2RegFound + "\" + el + """;" + vbCrLf
                End If
                test = test + vbTab + "};"
            Next

            test = test + vbCrLf + "};"

            ' Salva la lista dei mod nel file di configurazione di ArmA 2 OA
            Try
                fileWriter = New System.IO.StreamWriter(A2ConfigFile, False)
                fileWriter.Write(test)
                fileWriter.Close()
            Catch ex As Exception
                MsgBox("Error writing ArmA 2 OA Config File:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Critical, "Error while saving config file")
                Exit Sub
            End Try

            ' Verifica se si sta facendo partire ArmA 2 OA con un determinato profilo
            ' e che il profilo presente nella textbox del ComboBox1 sia effettivamente
            ' un profilo valido
            If Not nome = "" And ComboBox1.Items.Contains(nome) Then
                My.Settings.LastProfile = nome
                My.Settings.Save()
            Else
                My.Settings.LastProfile = ""
                My.Settings.Save()
            End If

            ' Lancia l'applicazione con i parametri selezionati
            Dim pHelp As New System.Diagnostics.Process()
            Dim process As New System.Diagnostics.Process()

            ' Identifica quale eseguibile è presente nella cartella di installazione di 
            ' ArmA 2 OA
            If System.IO.File.Exists(Arma2RegFound + "\ArmA2OA_BE.exe") Then
                pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA_BE.exe"
                pHelp.StartInfo.Arguments = "0 0 "
            ElseIf System.IO.File.Exists(Arma2RegFound + "\ArmA2OA.exe") Then
                pHelp.StartInfo.FileName = Arma2RegFound + "\ArmA2OA.exe"
                pHelp.StartInfo.Arguments = ""
            Else
                ' Nessun eseguibile valido trovato
                MsgBox("Can not find ArmA 2 OA executables!", MsgBoxStyle.Exclamation, "ArmA 2 OA not found")
                Exit Sub
            End If

            ' Aggiungi i parametri di avvio multiplayer
            pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + "-connect=" + TextBox1.Text.Trim + " "

            If Not TextBox2.Text.Trim = "" Then
                pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + "-port=" + TextBox2.Text.Trim + " "
            End If

            If Not TextBox3.Text.Trim = "" Then
                pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + "-password=" + TextBox3.Text.Trim + " "
            End If

            pHelp.StartInfo.Arguments = pHelp.StartInfo.Arguments + StartupParameters()

            ' Salva i parametri di startup
            SaveParameters()

            ' Salva i parametri multiplayer
            My.Settings.IP = TextBox1.Text.Trim
            My.Settings.Port = TextBox2.Text.Trim
            My.Settings.Password = TextBox3.Text.Trim
            My.Settings.Save()

            'pHelp.StartInfo.UseShellExecute = True
            'pHelp.StartInfo.WindowStyle = ProcessWindowStyle.Normal

            Try
                ToolStripStatusLabel1.Text = "Launching ArmA 2 OA..."
                pHelp.Start()

                Do
                    Application.DoEvents()
                Loop While Not System.Diagnostics.Process.GetProcessesByName("ArmA2OA").Length > 0

                process = System.Diagnostics.Process.GetProcessesByName("ArmA2OA")(0)

                If ComboBox2.SelectedItem = "High" Then
                    process.PriorityClass = ProcessPriorityClass.High
                End If

                ' Blocca i controlli finchè ArmA 2 è in funzionamento
                Timer1.Stop()               ' Ferma l'eventuale timer avviato
                ToolStripStatusLabel1.Text = "ArmA 2 OA is running"
                Button1.Enabled = False
                Button2.Enabled = False
                Button3.Enabled = False
                TextBox1.ReadOnly = True
                TextBox2.ReadOnly = True
                TextBox3.ReadOnly = True

                ' Avvia il Timer2 che controlla la fine dell'esecuzione del processo di ArmA 2
                Timer2.Enabled = True

                'Do
                '    Application.DoEvents()
                'Loop While Not process.HasExited

                '' Riattiva i controlli dopo la chiusura di ArmA 2
                'ToolStripStatusLabel1.Text = "ArmA 2 OA Launcher"
                'Button1.Enabled = True
                'Button2.Enabled = True
                'Button3.Enabled = True
                'TextBox1.ReadOnly = False
                'TextBox2.ReadOnly = False
                'TextBox3.ReadOnly = False
            Catch ex As Exception
                MsgBox("Error while launching ArmA 2 OA:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not launch ArmA 2 OA")
            End Try
        Else
            ' Se l'indirizzo IP immesso sembra non essere corretto
            MsgBox("the IP address " + TextBox1.Text.Trim + " you entered seems to be not valid!", MsgBoxStyle.Exclamation, "IP address not valid")
            TextBox1.Select()
            TextBox1.SelectionStart = 0
            TextBox1.SelectionLength = TextBox1.Text.Length
        End If
    End Sub

    Private Function StartupParameters() As String
        ' Restituisce una stringa con tutti i parametri di startup
        Dim startupString As String = ""

        If CheckBox1.Checked = True Then
            startupString = startupString + "-skipIntro "
        End If

        If CheckBox2.Checked = True Then
            startupString = startupString + "-noSplash "
        End If

        If CheckBox3.Checked = True Then
            startupString = startupString + "-world=" + TextBox4.Text.Trim + " "
        End If

        If CheckBox4.Checked = True Then
            startupString = startupString + "-maxMem=" + ComboBox3.SelectedItem + " "
        End If

        If CheckBox5.Checked = True Then
            startupString = startupString + "-maxVRAM=" + ComboBox4.SelectedItem + " "
        End If

        If CheckBox9.Checked = True Then
            startupString = startupString + "-cpuCount=" + ComboBox5.SelectedItem + " "
        End If

        If CheckBox10.Checked = True Then
            startupString = startupString + "-exThreads=" + ComboBox6.SelectedItem + " "
        End If

        If CheckBox11.Checked = True Then
            startupString = startupString + "-malloc=" + ComboBox7.SelectedItem + " "
        End If

        If CheckBox12.Checked = True Then
            startupString = startupString + "-noLogs "
        End If

        If CheckBox8.Checked = True Then
            startupString = startupString + "-noCB "
        End If

        If CheckBox7.Checked = True Then
            startupString = startupString + "-winXP "
        End If

        If CheckBox13.Checked = True Then
            startupString = startupString + "-noPause "
        End If

        If CheckBox14.Checked = True Then
            startupString = startupString + "-showScriptErrors "
        End If

        If CheckBox15.Checked = True Then
            startupString = startupString + "-noFilePatching "
        End If

        If CheckBox16.Checked = True Then
            startupString = startupString + "-window "
        End If

        If CheckBox17.Checked = True Then
            startupString = startupString + "-checkSignatures "
        End If

        If CheckBox18.Checked = True Then
            startupString = startupString + "-init=" + TextBox5.Text.Trim + " "
        End If

        If CheckBox20.Checked = True Then
            startupString = startupString + TextBox6.Text.Trim + " "
        End If

        If CheckBox19.Checked = True Then
            startupString = startupString + "-profile=" + TextBox6.Text.Trim
        End If

        Return startupString
    End Function

    Private Sub SaveParameters()
        ' Salva i parametri di startup
        If CheckBox1.Checked = True Then
            My.Settings.EmptyWorld = True
        Else
            My.Settings.EmptyWorld = False
        End If

        If CheckBox2.Checked = True Then
            My.Settings.SplashScreen = True
        Else
            My.Settings.SplashScreen = False
        End If

        If CheckBox3.Checked = True Then
            My.Settings.World = True
            My.Settings.WorldName = TextBox4.Text.Trim
        Else
            My.Settings.World = False
            My.Settings.WorldName = TextBox4.Text.Trim
        End If

        If CheckBox6.Checked = True Then
            My.Settings.Priority = True
            My.Settings.PriorityName = ComboBox2.SelectedItem
        Else
            My.Settings.Priority = False
            My.Settings.PriorityName = ComboBox2.SelectedItem
        End If

        If CheckBox4.Checked = True Then
            My.Settings.MaxRam = True
            My.Settings.MaxRamName = ComboBox3.SelectedItem
        Else
            My.Settings.MaxRam = False
            My.Settings.MaxRamName = ComboBox3.SelectedItem
        End If

        If CheckBox5.Checked = True Then
            My.Settings.MaxVideoRam = True
            My.Settings.MaxVideoRamName = ComboBox4.SelectedItem
        Else
            My.Settings.MaxVideoRam = False
            My.Settings.MaxVideoRamName = ComboBox4.SelectedItem
        End If

        If CheckBox9.Checked = True Then
            My.Settings.CPUCount = True
            My.Settings.CPUCountName = ComboBox5.SelectedItem
        Else
            My.Settings.CPUCount = False
            My.Settings.CPUCountName = ComboBox5.SelectedItem
        End If

        If CheckBox10.Checked = True Then
            My.Settings.ExThreads = True
            My.Settings.ExThreadsName = ComboBox6.SelectedItem
        Else
            My.Settings.ExThreads = False
            My.Settings.ExThreadsName = ComboBox6.SelectedItem
        End If

        If CheckBox11.Checked = True Then
            My.Settings.Allocator = True
            My.Settings.AllocatorName = ComboBox7.SelectedItem
        Else
            My.Settings.Allocator = False
            My.Settings.AllocatorName = ComboBox7.SelectedItem
        End If

        If CheckBox12.Checked = True Then
            My.Settings.NoLogs = True
        Else
            My.Settings.NoLogs = False
        End If

        If CheckBox8.Checked = True Then
            My.Settings.NoMulticore = True
        Else
            My.Settings.NoMulticore = False
        End If

        If CheckBox7.Checked = True Then
            My.Settings.WinXP = True
        Else
            My.Settings.WinXP = False
        End If

        If CheckBox13.Checked = True Then
            My.Settings.NoPause = True
        Else
            My.Settings.NoPause = False
        End If

        If CheckBox14.Checked = True Then
            My.Settings.ScriptErrors = True
        Else
            My.Settings.ScriptErrors = False
        End If

        If CheckBox15.Checked = True Then
            My.Settings.FilePatch = True
        Else
            My.Settings.FilePatch = False
        End If

        If CheckBox16.Checked = True Then
            My.Settings.Window = True
        Else
            My.Settings.Window = False
        End If

        If CheckBox17.Checked = True Then
            My.Settings.Signatures = True
        Else
            My.Settings.Signatures = False
        End If

        If CheckBox18.Checked = True Then
            My.Settings.Init = True
            My.Settings.InitName = TextBox5.Text.Trim
        Else
            My.Settings.Init = False
            My.Settings.InitName = TextBox5.Text.Trim
        End If

        If CheckBox20.Checked = True Then
            My.Settings.Other = True
            My.Settings.OtherName = TextBox6.Text.Trim
        Else
            My.Settings.Other = False
            My.Settings.OtherName = TextBox6.Text.Trim
        End If

        If CheckBox19.Checked = True Then
            My.Settings.InProfile = True
            My.Settings.InProfileName = ComboBox8.SelectedItem
        Else
            My.Settings.InProfile = False
            My.Settings.InProfileName = ComboBox8.SelectedItem
        End If

        Try
            My.Settings.Save()
            ToolStripStatusLabel1.Text = "Parameters Saved!"
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Error while saving startup parameters:" + vbCrLf + vbCrLf + ex.ToString, MsgBoxStyle.Exclamation, "Can not save startup parameters")
        End Try
    End Sub

    Private Sub CheckParameters()
        ' Controlla i parametri di startup attivi
        If My.Settings.EmptyWorld = True Then
            CheckBox1.Checked = True
        Else
            CheckBox1.Checked = False
        End If

        If My.Settings.SplashScreen = True Then
            CheckBox2.Checked = True
        Else
            CheckBox2.Checked = False
        End If

        If My.Settings.World = True Then
            CheckBox3.Checked = True
            TextBox4.Enabled = True
            TextBox4.Text = My.Settings.WorldName
        Else
            CheckBox3.Checked = False
            TextBox4.Enabled = False
            TextBox4.Text = My.Settings.WorldName
        End If

        If My.Settings.Priority = True Then
            CheckBox6.Checked = True
            ComboBox2.Enabled = True
            If ComboBox2.Items.Contains(My.Settings.PriorityName) Then
                ComboBox2.SelectedItem = My.Settings.PriorityName
            Else
                ComboBox2.SelectedIndex = 0
            End If
        Else
            CheckBox6.Checked = False
            ComboBox2.Enabled = False
            If ComboBox2.Items.Contains(My.Settings.PriorityName) Then
                ComboBox2.SelectedItem = My.Settings.PriorityName
            Else
                ComboBox2.SelectedIndex = 0
            End If
        End If

        If My.Settings.MaxRam = True Then
            CheckBox4.Checked = True
            ComboBox3.Enabled = True
            If ComboBox3.Items.Contains(My.Settings.MaxRamName) Then
                ComboBox3.SelectedItem = My.Settings.MaxRamName
            Else
                ComboBox3.SelectedItem = "2047"
            End If
        Else
            CheckBox4.Checked = False
            ComboBox3.Enabled = False
            If ComboBox3.Items.Contains(My.Settings.MaxRamName) Then
                ComboBox3.SelectedItem = My.Settings.MaxRamName
            Else
                ComboBox3.SelectedItem = "2047"
            End If
        End If

        If My.Settings.MaxVideoRam = True Then
            CheckBox5.Checked = True
            ComboBox4.Enabled = True
            If ComboBox4.Items.Contains(My.Settings.MaxVideoRamName) Then
                ComboBox4.SelectedItem = My.Settings.MaxVideoRamName
            Else
                ComboBox4.SelectedItem = "1024"
            End If
        Else
            CheckBox5.Checked = False
            ComboBox4.Enabled = False
            If ComboBox4.Items.Contains(My.Settings.MaxVideoRamName) Then
                ComboBox4.SelectedItem = My.Settings.MaxVideoRamName
            Else
                ComboBox4.SelectedItem = "1024"
            End If
        End If

        If My.Settings.CPUCount = True Then
            CheckBox9.Checked = True
            ComboBox5.Enabled = True
            If ComboBox5.Items.Contains(My.Settings.CPUCountName) Then
                ComboBox5.SelectedItem = My.Settings.CPUCountName
            Else
                ComboBox5.SelectedItem = "1"
            End If
        Else
            CheckBox9.Checked = False
            ComboBox5.Enabled = False
            If ComboBox5.Items.Contains(My.Settings.CPUCountName) Then
                ComboBox5.SelectedItem = My.Settings.CPUCountName
            Else
                ComboBox5.SelectedItem = "1"
            End If
        End If

        If My.Settings.ExThreads = True Then
            CheckBox10.Checked = True
            ComboBox6.Enabled = True
            If ComboBox6.Items.Contains(My.Settings.ExThreadsName) Then
                ComboBox6.SelectedItem = My.Settings.ExThreadsName
            Else
                ComboBox6.SelectedItem = "1"
            End If
        Else
            CheckBox10.Checked = False
            ComboBox6.Enabled = False
            If ComboBox6.Items.Contains(My.Settings.ExThreadsName) Then
                ComboBox6.SelectedItem = My.Settings.ExThreadsName
            Else
                ComboBox6.SelectedItem = "1"
            End If
        End If

        If My.Settings.Allocator = True Then
            CheckBox11.Checked = True
            ComboBox7.Enabled = True
            If ComboBox7.Items.Contains(My.Settings.AllocatorName) Then
                ComboBox7.SelectedItem = My.Settings.AllocatorName
            ElseIf ComboBox7.Items.Count > 0 Then
                ComboBox7.SelectedIndex = 0
            End If
        Else
            CheckBox11.Checked = False
            ComboBox7.Enabled = False
            If ComboBox7.Items.Contains(My.Settings.AllocatorName) Then
                ComboBox7.SelectedItem = My.Settings.AllocatorName
            ElseIf ComboBox7.Items.Count > 0 Then
                ComboBox7.SelectedIndex = 0
            End If
        End If

        If My.Settings.NoLogs = True Then
            CheckBox12.Checked = True
        Else
            CheckBox12.Checked = False
        End If

        If My.Settings.NoMulticore = True Then
            CheckBox8.Checked = True
        Else
            CheckBox8.Checked = False
        End If

        If My.Settings.WinXP = True Then
            CheckBox7.Checked = True
        Else
            CheckBox7.Checked = False
        End If

        If My.Settings.NoPause = True Then
            CheckBox13.Checked = True
        Else
            CheckBox13.Checked = False
        End If

        If My.Settings.ScriptErrors = True Then
            CheckBox14.Checked = True
        Else
            CheckBox14.Checked = False
        End If

        If My.Settings.FilePatch = True Then
            CheckBox15.Checked = True
        Else
            CheckBox15.Checked = False
        End If

        If My.Settings.Window = True Then
            CheckBox16.Checked = True
        Else
            CheckBox16.Checked = False
        End If

        If My.Settings.Signatures = True Then
            CheckBox17.Checked = True
        Else
            CheckBox17.Checked = False
        End If

        If My.Settings.Init = True Then
            CheckBox18.Checked = True
            TextBox5.Text = My.Settings.InitName
            TextBox5.Enabled = True
        Else
            CheckBox18.Checked = False
            TextBox5.Enabled = False
            TextBox5.Text = My.Settings.InitName
        End If

        If My.Settings.Other = True Then
            CheckBox20.Checked = True
            TextBox6.Enabled = True
            TextBox6.Text = My.Settings.OtherName
        Else
            CheckBox20.Checked = False
            TextBox6.Enabled = False
            TextBox6.Text = My.Settings.OtherName
        End If

        If My.Settings.InProfile = True Then
            CheckBox19.Checked = True
            ComboBox8.Enabled = True
            If ComboBox8.Items.Contains(My.Settings.InProfileName) Then
                ComboBox8.SelectedItem = My.Settings.InProfileName
            ElseIf ComboBox8.Items.Count > 0 Then
                ComboBox8.SelectedIndex = 0
            End If
        Else
            CheckBox19.Checked = False
            ComboBox8.Enabled = False
            If ComboBox8.Items.Contains(My.Settings.InProfileName) Then
                ComboBox8.SelectedItem = My.Settings.InProfileName
            ElseIf ComboBox8.Items.Count > 0 Then
                ComboBox8.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            ComboBox2.Enabled = True
        Else
            ComboBox2.Enabled = False
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = True Then
            ComboBox3.Enabled = True
        Else
            ComboBox3.Enabled = False
        End If
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked = True Then
            ComboBox4.Enabled = True
        Else
            ComboBox4.Enabled = False
        End If
    End Sub

    Private Sub CheckBox9_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked = True Then
            ComboBox5.Enabled = True
        Else
            ComboBox5.Enabled = False
        End If
    End Sub

    Private Sub CheckBox10_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox10.CheckedChanged
        If CheckBox10.Checked = True Then
            ComboBox6.Enabled = True
        Else
            ComboBox6.Enabled = False
        End If
    End Sub

    Private Sub CheckBox11_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox11.CheckedChanged
        If CheckBox11.Checked = True Then
            ComboBox7.Enabled = True
        Else
            ComboBox7.Enabled = False
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            TextBox4.Enabled = True
        Else
            TextBox4.Enabled = False
        End If
    End Sub

    Private Sub CheckBox19_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox19.CheckedChanged
        If CheckBox19.Checked = True Then
            ComboBox8.Enabled = True
        Else
            ComboBox8.Enabled = False
        End If
    End Sub

    Private Sub CheckBox18_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox18.CheckedChanged
        If CheckBox18.Checked = True Then
            TextBox5.Enabled = True
        Else
            TextBox5.Enabled = False
        End If
    End Sub

    Private Sub CheckBox20_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox20.CheckedChanged
        If CheckBox20.Checked = True Then
            TextBox6.Enabled = True
        Else
            TextBox6.Enabled = False
        End If
    End Sub

    Private Sub TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox4.KeyPress
        ' Evita l'immissione del punto e virgola
        If Asc(e.KeyChar) = 35 Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox5_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox5.KeyPress
        ' Evita l'immissione del punto e virgola
        If Asc(e.KeyChar) = 35 Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox6.KeyPress
        ' Evita l'immissione del punto e virgola
        If Asc(e.KeyChar) = 35 Then
            e.Handled = True
        End If
    End Sub

    Private Sub PictureBox10_Click(sender As Object, e As EventArgs) Handles PictureBox10.Click
        ' Salvataggio parametri di startup
        SaveParameters()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ' Apri il form ServerBrowser
        ServerBrowser.Show()
    End Sub

    Private Sub PictureBox11_Click(sender As Object, e As EventArgs) Handles PictureBox11.Click
        ' Al click sul logo dei TS, apre il sito
        System.Diagnostics.Process.Start("http://www.tornadosquad.it")
    End Sub

    Private Sub PictureBox12_Click(sender As Object, e As EventArgs) Handles PictureBox12.Click
        ' Al click sul logo dei TS, apre il sito
        System.Diagnostics.Process.Start("http://www.tornadosquad.it")
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        ' Controlla la fine dell'esecuzione del processo relativo ad ArmA 2 OA
        If Not System.Diagnostics.Process.GetProcessesByName("ArmA2OA").Length > 0 Then
            ' Se non trovo più il processo di ArmA 2 OA riattiva ii bottoni ed i textbox del Form1
            Timer2.Enabled = False
            ToolStripStatusLabel1.Text = "ArmA 2 OA Launcher"
            Button1.Enabled = True
            Button2.Enabled = True
            Button3.Enabled = True
            TextBox1.ReadOnly = False
            TextBox2.ReadOnly = False
            TextBox3.ReadOnly = False
        End If
    End Sub

    ' Al click sul link, apre il sito internet
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("http://querymaster.codeplex.com/")
    End Sub
End Class
