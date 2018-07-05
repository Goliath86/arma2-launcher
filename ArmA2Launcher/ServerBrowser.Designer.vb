<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ServerBrowser
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ServerBrowser))
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.PreferredColumn = New System.Windows.Forms.DataGridViewImageColumn()
        Me.KeyColumn = New System.Windows.Forms.DataGridViewImageColumn()
        Me.ServerColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PlayersColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MaxPlayersColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IPColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PortColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PingColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ModsColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TypeColumn = New System.Windows.Forms.DataGridViewImageColumn()
        Me.VersionColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.CheckBox4 = New System.Windows.Forms.CheckBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioButton3 = New System.Windows.Forms.RadioButton()
        Me.RadioButton2 = New System.Windows.Forms.RadioButton()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.CheckBox3 = New System.Windows.Forms.CheckBox()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PreferredColumn, Me.KeyColumn, Me.ServerColumn, Me.PlayersColumn, Me.MaxPlayersColumn, Me.IPColumn, Me.PortColumn, Me.PingColumn, Me.ModsColumn, Me.TypeColumn, Me.VersionColumn})
        Me.DataGridView1.Location = New System.Drawing.Point(6, 6)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(1042, 312)
        Me.DataGridView1.TabIndex = 1
        '
        'PreferredColumn
        '
        Me.PreferredColumn.HeaderText = "Fav."
        Me.PreferredColumn.Name = "PreferredColumn"
        Me.PreferredColumn.ReadOnly = True
        Me.PreferredColumn.Width = 40
        '
        'KeyColumn
        '
        Me.KeyColumn.HeaderText = "Private"
        Me.KeyColumn.Name = "KeyColumn"
        Me.KeyColumn.ReadOnly = True
        Me.KeyColumn.Width = 50
        '
        'ServerColumn
        '
        Me.ServerColumn.HeaderText = "Server Name"
        Me.ServerColumn.Name = "ServerColumn"
        Me.ServerColumn.ReadOnly = True
        Me.ServerColumn.Width = 260
        '
        'PlayersColumn
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.PlayersColumn.DefaultCellStyle = DataGridViewCellStyle2
        Me.PlayersColumn.HeaderText = "Players"
        Me.PlayersColumn.Name = "PlayersColumn"
        Me.PlayersColumn.ReadOnly = True
        Me.PlayersColumn.Width = 50
        '
        'MaxPlayersColumn
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.MaxPlayersColumn.DefaultCellStyle = DataGridViewCellStyle3
        Me.MaxPlayersColumn.HeaderText = "Max Players"
        Me.MaxPlayersColumn.Name = "MaxPlayersColumn"
        Me.MaxPlayersColumn.ReadOnly = True
        Me.MaxPlayersColumn.Width = 80
        '
        'IPColumn
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.IPColumn.DefaultCellStyle = DataGridViewCellStyle4
        Me.IPColumn.HeaderText = "Server IP"
        Me.IPColumn.Name = "IPColumn"
        Me.IPColumn.ReadOnly = True
        Me.IPColumn.Width = 124
        '
        'PortColumn
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.PortColumn.DefaultCellStyle = DataGridViewCellStyle5
        Me.PortColumn.HeaderText = "Server Port"
        Me.PortColumn.Name = "PortColumn"
        Me.PortColumn.ReadOnly = True
        Me.PortColumn.Width = 70
        '
        'PingColumn
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.PingColumn.DefaultCellStyle = DataGridViewCellStyle6
        Me.PingColumn.HeaderText = "Ping"
        Me.PingColumn.Name = "PingColumn"
        Me.PingColumn.ReadOnly = True
        Me.PingColumn.Width = 40
        '
        'ModsColumn
        '
        Me.ModsColumn.HeaderText = "Mods"
        Me.ModsColumn.Name = "ModsColumn"
        Me.ModsColumn.ReadOnly = True
        Me.ModsColumn.Width = 200
        '
        'TypeColumn
        '
        Me.TypeColumn.HeaderText = "Server Type"
        Me.TypeColumn.Name = "TypeColumn"
        Me.TypeColumn.ReadOnly = True
        Me.TypeColumn.Width = 80
        '
        'VersionColumn
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.VersionColumn.DefaultCellStyle = DataGridViewCellStyle7
        Me.VersionColumn.HeaderText = "Game Version"
        Me.VersionColumn.Name = "VersionColumn"
        Me.VersionColumn.ReadOnly = True
        Me.VersionColumn.Width = 80
        '
        'Button1
        '
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button1.Location = New System.Drawing.Point(711, 347)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(86, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.Button1, "Refresh the server list")
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button2.Location = New System.Drawing.Point(711, 376)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(86, 23)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "Stop Refresh"
        Me.ToolTip1.SetToolTip(Me.Button2, "Stop the refresh")
        Me.Button2.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(251, 25)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(75, 17)
        Me.CheckBox1.TabIndex = 6
        Me.CheckBox1.Text = "Dedicated"
        Me.ToolTip1.SetToolTip(Me.CheckBox1, "Show only dedicated servers")
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(353, 24)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(55, 17)
        Me.CheckBox2.TabIndex = 9
        Me.CheckBox2.Text = "Empty"
        Me.ToolTip1.SetToolTip(Me.CheckBox2, "Show empty servers")
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'CheckBox4
        '
        Me.CheckBox4.AutoSize = True
        Me.CheckBox4.Location = New System.Drawing.Point(353, 70)
        Me.CheckBox4.Name = "CheckBox4"
        Me.CheckBox4.Size = New System.Drawing.Size(62, 17)
        Me.CheckBox4.TabIndex = 11
        Me.CheckBox4.Text = "Not Full"
        Me.ToolTip1.SetToolTip(Me.CheckBox4, "Do not show full servers")
        Me.CheckBox4.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 442)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1054, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Image = Global.ArmA2Launcher.My.Resources.Resources.Blinking_Yellow_Dot_2
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(197, 17)
        Me.ToolStripStatusLabel1.Text = "  Querying Steam Master Server..."
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckBox3)
        Me.GroupBox1.Controls.Add(Me.CheckBox4)
        Me.GroupBox1.Controls.Add(Me.CheckBox2)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Controls.Add(Me.RadioButton3)
        Me.GroupBox1.Controls.Add(Me.RadioButton2)
        Me.GroupBox1.Controls.Add(Me.RadioButton1)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 334)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(672, 100)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Filter"
        '
        'RadioButton3
        '
        Me.RadioButton3.AutoSize = True
        Me.RadioButton3.Location = New System.Drawing.Point(9, 70)
        Me.RadioButton3.Name = "RadioButton3"
        Me.RadioButton3.Size = New System.Drawing.Size(142, 17)
        Me.RadioButton3.TabIndex = 5
        Me.RadioButton3.Text = "Show only DayZ Servers"
        Me.RadioButton3.UseVisualStyleBackColor = True
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Location = New System.Drawing.Point(9, 47)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(150, 17)
        Me.RadioButton2.TabIndex = 4
        Me.RadioButton2.Text = "Show only ArmA 2 Servers"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Location = New System.Drawing.Point(9, 24)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(178, 17)
        Me.RadioButton1.TabIndex = 3
        Me.RadioButton1.Text = "Show ArmA 2 and DayZ Servers"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Location = New System.Drawing.Point(353, 47)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(75, 17)
        Me.CheckBox3.TabIndex = 12
        Me.CheckBox3.Text = "Not Empty"
        Me.ToolTip1.SetToolTip(Me.CheckBox3, "Show server with at least one player")
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'ServerBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1054, 464)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.DataGridView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ServerBrowser"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ArmA 2 OA Server Browser"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents PreferredColumn As System.Windows.Forms.DataGridViewImageColumn
    Friend WithEvents KeyColumn As System.Windows.Forms.DataGridViewImageColumn
    Friend WithEvents ServerColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PlayersColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MaxPlayersColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IPColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PortColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PingColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ModsColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TypeColumn As System.Windows.Forms.DataGridViewImageColumn
    Friend WithEvents VersionColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox4 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
End Class
