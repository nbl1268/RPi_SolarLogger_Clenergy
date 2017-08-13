<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.sp = New System.IO.Ports.SerialPort(Me.components)
        Me.btnClearLog = New System.Windows.Forms.Button()
        Me.lvwLog = New System.Windows.Forms.ListView()
        Me.chdLog = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.gpbCommSettings = New System.Windows.Forms.GroupBox()
        Me.btnCommPortControl = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnGetPorts = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cboDataParityStopbits = New System.Windows.Forms.ComboBox()
        Me.cboBaudRate = New System.Windows.Forms.ComboBox()
        Me.cboCommPorts = New System.Windows.Forms.ComboBox()
        Me.fbd = New System.Windows.Forms.FolderBrowserDialog()
        Me.ss = New System.Windows.Forms.StatusStrip()
        Me.ssCommSettings = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ssCommStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ssLogFile = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ssBufLen = New System.Windows.Forms.ToolStripStatusLabel()
        Me.txtGetSerial = New System.Windows.Forms.TextBox()
        Me.btnGetSerial = New System.Windows.Forms.Button()
        Me.btnSendLogIn = New System.Windows.Forms.Button()
        Me.txtSendLogIn = New System.Windows.Forms.TextBox()
        Me.btnGetPVOutput = New System.Windows.Forms.Button()
        Me.txtGetPVOutput = New System.Windows.Forms.TextBox()
        Me.txtInverterSerial = New System.Windows.Forms.TextBox()
        Me.lblInverterSerial = New System.Windows.Forms.Label()
        Me.tmrSP = New System.Windows.Forms.Timer(Me.components)
        Me.txtGetSerialCRC = New System.Windows.Forms.TextBox()
        Me.txtSendLogInCRC = New System.Windows.Forms.TextBox()
        Me.txtGetPVOutputCRC = New System.Windows.Forms.TextBox()
        Me.gpbCommSettings.SuspendLayout()
        Me.ss.SuspendLayout()
        Me.SuspendLayout()
        '
        'sp
        '
        '
        'btnClearLog
        '
        Me.btnClearLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearLog.Location = New System.Drawing.Point(816, 114)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New System.Drawing.Size(81, 24)
        Me.btnClearLog.TabIndex = 8
        Me.btnClearLog.Text = "Clear Log"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'lvwLog
        '
        Me.lvwLog.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwLog.AutoArrange = False
        Me.lvwLog.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chdLog})
        Me.lvwLog.GridLines = True
        Me.lvwLog.Location = New System.Drawing.Point(12, 144)
        Me.lvwLog.MultiSelect = False
        Me.lvwLog.Name = "lvwLog"
        Me.lvwLog.Size = New System.Drawing.Size(885, 357)
        Me.lvwLog.TabIndex = 3
        Me.lvwLog.UseCompatibleStateImageBehavior = False
        Me.lvwLog.View = System.Windows.Forms.View.Details
        '
        'chdLog
        '
        Me.chdLog.Text = "Log"
        Me.chdLog.Width = 666
        '
        'gpbCommSettings
        '
        Me.gpbCommSettings.Controls.Add(Me.btnCommPortControl)
        Me.gpbCommSettings.Controls.Add(Me.Label6)
        Me.gpbCommSettings.Controls.Add(Me.btnGetPorts)
        Me.gpbCommSettings.Controls.Add(Me.Label5)
        Me.gpbCommSettings.Controls.Add(Me.cboDataParityStopbits)
        Me.gpbCommSettings.Controls.Add(Me.cboBaudRate)
        Me.gpbCommSettings.Controls.Add(Me.cboCommPorts)
        Me.gpbCommSettings.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpbCommSettings.Location = New System.Drawing.Point(12, 12)
        Me.gpbCommSettings.Name = "gpbCommSettings"
        Me.gpbCommSettings.Size = New System.Drawing.Size(177, 126)
        Me.gpbCommSettings.TabIndex = 1
        Me.gpbCommSettings.TabStop = False
        Me.gpbCommSettings.Text = "Comm Settings"
        '
        'btnCommPortControl
        '
        Me.btnCommPortControl.Location = New System.Drawing.Point(6, 97)
        Me.btnCommPortControl.Name = "btnCommPortControl"
        Me.btnCommPortControl.Size = New System.Drawing.Size(75, 23)
        Me.btnCommPortControl.TabIndex = 9
        Me.btnCommPortControl.Text = "Open Port"
        Me.btnCommPortControl.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(8, 67)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(86, 20)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "Data Parity Stop"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnGetPorts
        '
        Me.btnGetPorts.Location = New System.Drawing.Point(6, 14)
        Me.btnGetPorts.Name = "btnGetPorts"
        Me.btnGetPorts.Size = New System.Drawing.Size(75, 23)
        Me.btnGetPorts.TabIndex = 8
        Me.btnGetPorts.Text = "Get Ports"
        Me.btnGetPorts.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(8, 41)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(86, 20)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Baud Rate"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboDataParityStopbits
        '
        Me.cboDataParityStopbits.FormattingEnabled = True
        Me.cboDataParityStopbits.Location = New System.Drawing.Point(100, 67)
        Me.cboDataParityStopbits.Name = "cboDataParityStopbits"
        Me.cboDataParityStopbits.Size = New System.Drawing.Size(67, 21)
        Me.cboDataParityStopbits.TabIndex = 5
        '
        'cboBaudRate
        '
        Me.cboBaudRate.FormattingEnabled = True
        Me.cboBaudRate.Location = New System.Drawing.Point(100, 41)
        Me.cboBaudRate.Name = "cboBaudRate"
        Me.cboBaudRate.Size = New System.Drawing.Size(67, 21)
        Me.cboBaudRate.TabIndex = 3
        '
        'cboCommPorts
        '
        Me.cboCommPorts.FormattingEnabled = True
        Me.cboCommPorts.Location = New System.Drawing.Point(100, 15)
        Me.cboCommPorts.Name = "cboCommPorts"
        Me.cboCommPorts.Size = New System.Drawing.Size(67, 21)
        Me.cboCommPorts.Sorted = True
        Me.cboCommPorts.TabIndex = 1
        '
        'ss
        '
        Me.ss.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ssCommSettings, Me.ssCommStatus, Me.ssLogFile, Me.ssBufLen})
        Me.ss.Location = New System.Drawing.Point(0, 520)
        Me.ss.Name = "ss"
        Me.ss.Size = New System.Drawing.Size(909, 24)
        Me.ss.SizingGrip = False
        Me.ss.TabIndex = 1
        Me.ss.Text = "ss"
        '
        'ssCommSettings
        '
        Me.ssCommSettings.AutoSize = False
        Me.ssCommSettings.Name = "ssCommSettings"
        Me.ssCommSettings.Size = New System.Drawing.Size(93, 19)
        Me.ssCommSettings.Text = "Com1:9600N81"
        Me.ssCommSettings.ToolTipText = "Port settings"
        '
        'ssCommStatus
        '
        Me.ssCommStatus.AutoSize = False
        Me.ssCommStatus.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ssCommStatus.Name = "ssCommStatus"
        Me.ssCommStatus.Size = New System.Drawing.Size(43, 19)
        Me.ssCommStatus.Text = "Closed"
        Me.ssCommStatus.ToolTipText = "Port Status"
        '
        'ssLogFile
        '
        Me.ssLogFile.AutoSize = False
        Me.ssLogFile.Name = "ssLogFile"
        Me.ssLogFile.Size = New System.Drawing.Size(45, 19)
        Me.ssLogFile.Text = "LogFile"
        '
        'ssBufLen
        '
        Me.ssBufLen.Name = "ssBufLen"
        Me.ssBufLen.Size = New System.Drawing.Size(54, 19)
        Me.ssBufLen.Text = "SBuffLen"
        '
        'txtGetSerial
        '
        Me.txtGetSerial.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGetSerial.Location = New System.Drawing.Point(288, 28)
        Me.txtGetSerial.Name = "txtGetSerial"
        Me.txtGetSerial.Size = New System.Drawing.Size(340, 20)
        Me.txtGetSerial.TabIndex = 12
        Me.txtGetSerial.Text = "BB BB 00 00 00 00 00 00 00"
        '
        'btnGetSerial
        '
        Me.btnGetSerial.Location = New System.Drawing.Point(195, 26)
        Me.btnGetSerial.Name = "btnGetSerial"
        Me.btnGetSerial.Size = New System.Drawing.Size(87, 23)
        Me.btnGetSerial.TabIndex = 13
        Me.btnGetSerial.Text = "Get Serial"
        Me.btnGetSerial.UseVisualStyleBackColor = True
        '
        'btnSendLogIn
        '
        Me.btnSendLogIn.Location = New System.Drawing.Point(195, 78)
        Me.btnSendLogIn.Name = "btnSendLogIn"
        Me.btnSendLogIn.Size = New System.Drawing.Size(87, 23)
        Me.btnSendLogIn.TabIndex = 15
        Me.btnSendLogIn.Text = "Send Log In"
        Me.btnSendLogIn.UseVisualStyleBackColor = True
        '
        'txtSendLogIn
        '
        Me.txtSendLogIn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSendLogIn.Location = New System.Drawing.Point(288, 79)
        Me.txtSendLogIn.Name = "txtSendLogIn"
        Me.txtSendLogIn.Size = New System.Drawing.Size(340, 20)
        Me.txtSendLogIn.TabIndex = 14
        Me.txtSendLogIn.Text = "BB BB 00 00 00 00 00 01 0C 31 30 30 33 33 37 36 31 31 30 32"
        '
        'btnGetPVOutput
        '
        Me.btnGetPVOutput.Location = New System.Drawing.Point(195, 109)
        Me.btnGetPVOutput.Name = "btnGetPVOutput"
        Me.btnGetPVOutput.Size = New System.Drawing.Size(87, 23)
        Me.btnGetPVOutput.TabIndex = 17
        Me.btnGetPVOutput.Text = "Get PV Output"
        Me.btnGetPVOutput.UseVisualStyleBackColor = True
        '
        'txtGetPVOutput
        '
        Me.txtGetPVOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGetPVOutput.Location = New System.Drawing.Point(288, 111)
        Me.txtGetPVOutput.Name = "txtGetPVOutput"
        Me.txtGetPVOutput.Size = New System.Drawing.Size(340, 20)
        Me.txtGetPVOutput.TabIndex = 16
        Me.txtGetPVOutput.Text = "BB BB 01 00 00 01 01 02 00"
        '
        'txtInverterSerial
        '
        Me.txtInverterSerial.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtInverterSerial.Location = New System.Drawing.Point(288, 53)
        Me.txtInverterSerial.Name = "txtInverterSerial"
        Me.txtInverterSerial.Size = New System.Drawing.Size(386, 20)
        Me.txtInverterSerial.TabIndex = 18
        '
        'lblInverterSerial
        '
        Me.lblInverterSerial.Location = New System.Drawing.Point(196, 53)
        Me.lblInverterSerial.Name = "lblInverterSerial"
        Me.lblInverterSerial.Size = New System.Drawing.Size(86, 20)
        Me.lblInverterSerial.TabIndex = 19
        Me.lblInverterSerial.Text = "Inverter Serial"
        Me.lblInverterSerial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tmrSP
        '
        '
        'txtGetSerialCRC
        '
        Me.txtGetSerialCRC.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGetSerialCRC.Location = New System.Drawing.Point(634, 28)
        Me.txtGetSerialCRC.Name = "txtGetSerialCRC"
        Me.txtGetSerialCRC.Size = New System.Drawing.Size(40, 20)
        Me.txtGetSerialCRC.TabIndex = 20
        Me.txtGetSerialCRC.Text = "01 76"
        '
        'txtSendLogInCRC
        '
        Me.txtSendLogInCRC.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSendLogInCRC.Location = New System.Drawing.Point(634, 79)
        Me.txtSendLogInCRC.Name = "txtSendLogInCRC"
        Me.txtSendLogInCRC.Size = New System.Drawing.Size(40, 20)
        Me.txtSendLogInCRC.TabIndex = 21
        Me.txtSendLogInCRC.Text = "03 AB"
        '
        'txtGetPVOutputCRC
        '
        Me.txtGetPVOutputCRC.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGetPVOutputCRC.Location = New System.Drawing.Point(634, 111)
        Me.txtGetPVOutputCRC.Name = "txtGetPVOutputCRC"
        Me.txtGetPVOutputCRC.Size = New System.Drawing.Size(40, 20)
        Me.txtGetPVOutputCRC.TabIndex = 21
        Me.txtGetPVOutputCRC.Text = "01 7B"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(909, 544)
        Me.Controls.Add(Me.txtGetPVOutputCRC)
        Me.Controls.Add(Me.txtSendLogInCRC)
        Me.Controls.Add(Me.txtGetSerialCRC)
        Me.Controls.Add(Me.lblInverterSerial)
        Me.Controls.Add(Me.txtInverterSerial)
        Me.Controls.Add(Me.btnGetPVOutput)
        Me.Controls.Add(Me.txtGetPVOutput)
        Me.Controls.Add(Me.btnSendLogIn)
        Me.Controls.Add(Me.txtSendLogIn)
        Me.Controls.Add(Me.btnGetSerial)
        Me.Controls.Add(Me.txtGetSerial)
        Me.Controls.Add(Me.btnClearLog)
        Me.Controls.Add(Me.ss)
        Me.Controls.Add(Me.lvwLog)
        Me.Controls.Add(Me.gpbCommSettings)
        Me.DoubleBuffered = True
        Me.Name = "frmMain"
        Me.Text = "vbPowerCom"
        Me.gpbCommSettings.ResumeLayout(False)
        Me.ss.ResumeLayout(False)
        Me.ss.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents sp As IO.Ports.SerialPort
    Friend WithEvents lvwLog As ListView
    Friend WithEvents btnGetPorts As Button
    Friend WithEvents gpbCommSettings As GroupBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents cboDataParityStopbits As ComboBox
    Friend WithEvents cboBaudRate As ComboBox
    Friend WithEvents cboCommPorts As ComboBox
    Friend WithEvents fbd As FolderBrowserDialog
    Friend WithEvents btnClearLog As Button
    Friend WithEvents chdLog As ColumnHeader
    Friend WithEvents ss As StatusStrip
    Friend WithEvents ssCommSettings As ToolStripStatusLabel
    Friend WithEvents ssCommStatus As ToolStripStatusLabel
    Friend WithEvents btnCommPortControl As Button
    Friend WithEvents ssLogFile As ToolStripStatusLabel
    Friend WithEvents ssBufLen As ToolStripStatusLabel
    Friend WithEvents txtGetSerial As TextBox
    Friend WithEvents btnGetSerial As Button
    Friend WithEvents btnSendLogIn As Button
    Friend WithEvents txtSendLogIn As TextBox
    Friend WithEvents btnGetPVOutput As Button
    Friend WithEvents txtGetPVOutput As TextBox
    Friend WithEvents txtInverterSerial As TextBox
    Friend WithEvents lblInverterSerial As Label
    Friend WithEvents tmrSP As Timer
    Friend WithEvents txtGetSerialCRC As TextBox
    Friend WithEvents txtSendLogInCRC As TextBox
    Friend WithEvents txtGetPVOutputCRC As TextBox
End Class
