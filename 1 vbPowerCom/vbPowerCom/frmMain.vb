' Project Name:     vbPowerCom
' Author/s:         Neil Blanchard
' Start Date:       19/10/2016
' Last Changed:     19/10/2016
' Version:          0.1.1.*
' Description:      Communicate with Clenergy Inverter and store as CSV

'19Oct - Working - send and recv from inverter - details of commands and responses in XLS file
'cmd args Com4 9600 N81

#Region " ToDo: "

#End Region

#Region " Updates "

#End Region

Option Strict On
Option Explicit On
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Public Class frmMain

#Region " Declarations "
    Dim sMsgText As String  ' app to gui messages

    Const iFormHeightMin As Integer = 400
    Const iFormWidthMin As Integer = 700
    Dim bFrmMainLoaded As Boolean

    Dim fs As FileStream
    Dim sw As StreamWriter
    Dim sAppPath As String = My.Application.Info.DirectoryPath
    Dim sLogFileName As String = My.Application.Info.AssemblyName & ".txt"
    Dim sRawFileName As String = My.Application.Info.AssemblyName & ".raw"

    Dim iCommDataBits As Integer
    Dim iCommPortParity As Integer
    Dim iCommPortStopBits As Integer
    Dim bCommPort As Boolean
    Dim bCommPortOpen As Boolean
    Dim bCommPortSettingsChanged As Boolean
    Dim bCommError As Boolean

    Dim bArgs As Boolean        ' flag is command line args available

    Dim sRcvBuff As String      ' fifo for serial port

    'Dim sBuff As String ' buffer for serial receive
    'Const sMTC As String = vbLf & vbCr   ' message terminator characters '0x0A 0x0D' eg vbLF vbCR
    'Const iLenMTC As Integer = 2         ' number of terminator characters, must match MTC

    ' command data
    Dim sCmdGetSerial() As Byte
    Dim sCmdLogIn() As Byte
    Dim sCmdGetPVOutput() As Byte


#End Region

    'Const bDEBUG As Boolean = False    ' for testing
    'Const bDEBUG As Boolean = True     ' for testing

#Region " frmMain "

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' App Started; update form Title
        Dim sAppVer As String
        sAppVer = My.Application.Info.Version.Major.ToString + "." +
        My.Application.Info.Version.Minor.ToString + "." +
        My.Application.Info.Version.Build.ToString
        Me.Text = "vbPowerCom - Ver " + sAppVer
        UpdateLog("App: " & Me.Text.ToString & " - Started...")
        bFrmMainLoaded = True

        ' setup CommPort combo box
        GetSerialPortNames()

        ' setup CommPort Baudrate combo box
        cboBaudRate.Items.Add("9600")       ' Default - index = 0
        cboBaudRate.Items.Add("19200")
        cboBaudRate.SelectedIndex = 0

        ' setup CommPort ParityDataStopBits combo box
        cboDataParityStopbits.Items.Add("7N1")
        cboDataParityStopbits.Items.Add("7O1")
        cboDataParityStopbits.Items.Add("7E1")
        cboDataParityStopbits.Items.Add("8N1")    ' Default - index = 3
        cboDataParityStopbits.Items.Add("8O1")
        cboDataParityStopbits.Items.Add("8E1")
        cboDataParityStopbits.SelectedIndex = 3

        ' set status strip items
        ssCommStatus.Text = "Closed"
        ssCommStatus.BackColor = Color.Red

        ' check cmd args
        CmdArgs()

        ' setup comms
        If bCommPort Then
            SetSerialComms(bCommPortOpen)
        End If

        tmrSP.Interval = 100
        tmrSP.Enabled = True

        ' wait here for events... :)

    End Sub

    Private Sub CmdArgs()
        ' check cmd args
        ' - {ComPort}{Baudrate}{BitsParityStop}
        Dim args() As String = {}   'default empty
        Try
            args = Environment.GetCommandLineArgs
            Select Case args.Length
                Case 1
                    ' just exe path - dont care
                    bArgs = False

                Case 4
                    ' - {ComPort}{Baudrate}{BitsParityStop}
                    bArgs = True

                Case Else
                    bArgs = False
                    MsgBox("App: Command Line Args not valid", MsgBoxStyle.Exclamation, "Command Line Arguments")
                    UpdateLog("App: Command Line Args not valid")
            End Select
        Catch
        End Try

        If bArgs Then
            ' valid number of args to use
            ' - {ComPort}{Baudrate}{BitsParityStop}{LogPath}
            UpdateLog("App: Command Line Args valid")

            If args(1).ToUpper Like "COM*" Then
                ' check if com port exists (in cboCommPorts)
                Try
                    Select Case cboCommPorts.FindStringExact(args(1).ToUpper)
                        Case -1
                            ' not found
                            cboCommPorts.SelectedIndex = -1 '0 eg default
                            bCommPort = False
                            MessageBox.Show("Command Line ComPort value not valid" & vbCr _
                                            & "No Comms Port selected...", Me.Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)

                        Case Else
                            ' found
                            cboCommPorts.SelectedIndex = cboCommPorts.FindStringExact(args(1).ToUpper)
                            bCommPort = True
                            bCommPortOpen = True
                    End Select
                Catch ex As Exception
                End Try
            End If

            If IsNumeric(args(2)) Then
                ' check for matching Baudrate in cboBaudRate
                Try
                    Select Case cboBaudRate.FindStringExact(args(2).ToUpper)
                        Case -1
                            ' not found
                            cboBaudRate.SelectedIndex = 1
                            MessageBox.Show("Command Line Baudrate value not valid" & vbCr _
                                            & "Default Baudrate selected...", Me.Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)

                        Case Else
                            ' found
                            cboBaudRate.SelectedIndex = cboBaudRate.FindStringExact(args(2).ToUpper)
                    End Select
                Catch ex As Exception
                End Try
            End If

            If IsNumeric(args(3)) Then
                ' check for matching DataParityStop in cboDataParityStopbits
                Try
                    Select Case cboDataParityStopbits.FindStringExact(args(3).ToUpper)
                        Case -1
                            ' not found
                            cboDataParityStopbits.SelectedIndex = 3
                            MessageBox.Show("Command Line Data Parity Stop value not valid" & vbCr _
                                            & "Default Data pairty Stop selected...", Me.Text,
                                            MessageBoxButtons.OK, MessageBoxIcon.Error)

                        Case Else
                            ' found
                            cboDataParityStopbits.SelectedIndex = cboDataParityStopbits.FindStringExact(args(3).ToUpper)
                    End Select
                Catch ex As Exception
                End Try
            End If
        End If

    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        ' form resize finished
        ' check minimum size
        If WindowState = FormWindowState.Normal Then
            ' enforce minimum form size

            Me.SuspendLayout()

            If Me.Height < iFormHeightMin Then
                Me.Height = iFormHeightMin
            End If

            If Me.Width < iFormWidthMin Then
                Me.Width = iFormWidthMin
            End If

            Me.ResumeLayout()

        End If

        ' resize the column in lvwLog
        lvwLog.Columns(0).Width = -2

    End Sub

    Private Sub frmMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ' check / stop timer
        If tmrSP.Enabled Then
            tmrSP.Enabled = False
        End If

        ' check / close serial port
        If sp.IsOpen Then
            sp.Close()
        End If

        ' App stopping
        UpdateLog("App: " & Me.Text.ToString & " - Stopping...")
        UpdateLog("")
        UpdateLog("")
    End Sub
#End Region

#Region " Serial Port "

    'http://blogs.msmvps.com/coad/2005/03/23/serialport-rs-232-serial-com-port-in-c-net/
    'DB9 Male(Pin Side)                   DB9 Female (Pin Side)
    'DB9 Female(Solder Side)              DB9 Male (Solder Side)
    '    -------------                          -------------
    '    \ 1 2 3 4 5 /                          \ 5 4 3 2 1 /
    '     \ 6 7 8 9 /                            \ 9 8 7 6 /
    '      ---------                              ---------

    'DB9 Female To DB9 Female Null-Modem Wiring
    ' 2 |  3 |  7 |  8 | 6&1|  5 |  4
    '---- ---- ---- ---- ---- ---- ---- 
    ' 3 |  2 |  8 |  7 |  4 |  5 | 6&1

    '9-pin   25-pin  Assignment                 From PC
    '------  ------  -------------------------  ------------
    'Shield  1       Case Ground                Gnd
    '1       8       DCD (Data Carrier Detect)  Input
    '2       3       RX  (Receive Data)         Input
    '3       2       TX  (Transmit Data)        Output
    '4       20      DTR (Data Terminal Ready)  Output
    '5       7       GND (Signal Ground)        Gnd
    '6       6       DSR (Data Set Ready)       Input
    '7       4       RTS (Request To Send)      Output
    '8       5       CTS (Clear To Send)        Input
    '9       22      RI  (Ring Indicator)       Input

    '- RTS & DTR are binary outputs that can be manually set And held
    '- DCD, DSR, CTS, And RI are binary inputs that can be read
    '- RX & TX can Not be set manually And are controlled by the UART
    '- maximum voltages are between -15 volts And +15 volts
    '- binary outputs are between +5 to +15 volts And -5 to -15 volts
    '- binary inputs are between +3 to +15 volts And -3 to -15 volts
    '- input voltages between -3 to +3 are undefined while output voltages between -5 And +5 are undefined
    '- positive voltages indicate ON Or SPACE, negative voltages indicate OFF Or MARK

    Private Sub btnGetPorts_Click(sender As Object, e As EventArgs) Handles btnGetPorts.Click
        GetSerialPortNames()
    End Sub

    Private Sub GetSerialPortNames()
        ' get all available COM ports.
        ' load array and com box

        Dim i As Integer

        Try
            ' setup CommPort combo box
            cboCommPorts.Items.Clear()
            cboCommPorts.Items.Add("--")

            For i = 1 To My.Computer.Ports.SerialPortNames.Count
                cboCommPorts.Items.Add(My.Computer.Ports.SerialPortNames.Item(i - 1))
            Next

            cboCommPorts.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub

    Private Function CheckCommPortExists(ByVal cCommPort As Integer) As Boolean
        ' Check commport array to see if the selected commport exists 
        Dim i As Integer
        CheckCommPortExists = False

        Try
            If cCommPort = 0 Then Exit Function
            For i = 1 To cboCommPorts.Items.Count - 1
                If cCommPort = CInt(Mid(cboCommPorts.Items(i).ToString, 4, 1)) Then
                    Return True
                End If
            Next
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub cboCommPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCommPorts.SelectedIndexChanged
        ' read commport name and set commport integer
        ' recheck if comm port exists??

        Try
            If bFrmMainLoaded Then

                Dim i As Integer
                i = cboCommPorts.SelectedIndex

                If cboCommPorts.SelectedIndex > 0 Then
                    bCommPort = True
                    bCommPortSettingsChanged = True
                    'UpdateLog("App: Com settings " & cboCommPorts.Items(cboCommPorts.SelectedIndex).ToString)
                End If

            End If

            If bCommPort Then
                CommPortClose()
                SetSerialComms(False)
            End If

        Catch ex As Exception
        End Try

    End Sub

    Private Sub cboBaudRate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBaudRate.SelectedIndexChanged
        Try
            With sp
                .BaudRate = CInt(cboBaudRate.SelectedItem)
            End With
            bCommPortSettingsChanged = True
            'UpdateLog("App: Com settings " & cboBaudRate.Items(cboBaudRate.SelectedIndex).ToString)

            If bCommPort Then
                CommPortClose()
                SetSerialComms(False)
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub cboParityStopBits_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDataParityStopbits.SelectedIndexChanged
        ' SerialPort
        ' Parity
        '   None = 0
        '   Odd = 1
        '   Even = 2
        ' Stop
        '   None = 0
        '   One = 1
        '   Two = 2

        Try
            Select Case cboDataParityStopbits.SelectedItem.ToString
                Case "7N1"
                    iCommDataBits = 7
                    iCommPortParity = 0
                    iCommPortStopBits = 1

                Case "7O1"
                    iCommDataBits = 7
                    iCommPortParity = 1
                    iCommPortStopBits = 1

                Case "7E1"
                    iCommDataBits = 7
                    iCommPortParity = 2
                    iCommPortStopBits = 1

                Case "8N1"
                    iCommDataBits = 8
                    iCommPortParity = 0
                    iCommPortStopBits = 1

                Case "8O1"
                    iCommDataBits = 8
                    iCommPortParity = 1
                    iCommPortStopBits = 1

                Case "8E1"
                    iCommDataBits = 8
                    iCommPortParity = 2
                    iCommPortStopBits = 1

                Case Else
                    iCommDataBits = 8
                    iCommPortParity = 0
                    iCommPortStopBits = 1
            End Select

            With sp
                .DataBits = iCommDataBits
                .Parity = CType(iCommPortParity, Ports.Parity)
                .StopBits = CType(iCommPortStopBits, Ports.StopBits)
            End With
            bCommPortSettingsChanged = True
            'UpdateLog("App: Com settings " & cboDataParityStopbits.Items(cboDataParityStopbits.SelectedIndex).ToString)

            If bCommPort Then
                CommPortClose()
                SetSerialComms(False)
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub SetSerialComms(ByVal bOpen As Boolean)
        ' active the serial port function
        With sp
            If .IsOpen Then
                CommPortClose()
            End If

            Do While .IsOpen
                ' just wait til port is closed
            Loop

            ' brief pause for hardware
            Thread.Sleep(100)
            If bOpen Then OpenCommPort()
        End With

        sMsgText = cboCommPorts.Items(cboCommPorts.SelectedIndex).ToString & " " &
                   cboBaudRate.Items(cboBaudRate.SelectedIndex).ToString & " " &
                   cboDataParityStopbits.Items(cboDataParityStopbits.SelectedIndex).ToString
        UpdateLog("App: Com settings " & sMsgText)
        ssCommSettings.Text = sMsgText
        sMsgText = ""

        bCommPortSettingsChanged = False
    End Sub

    Private Sub btnCommPortControl_Click(sender As Object, e As EventArgs) Handles btnCommPortControl.Click
        ' toggle comm port status

        If Me.btnCommPortControl.Text = "Open Port" Then
            ' open the open
            OpenCommPort()

        Else
            ' close the port
            CommPortClose()
        End If
    End Sub

    Private Sub OpenCommPort()
        ' open the open
        Try
            With sp
                .PortName = cboCommPorts.SelectedItem.ToString
                .BaudRate = CInt(cboBaudRate.SelectedItem)
                .DataBits = iCommDataBits
                .Parity = CType(iCommPortParity, Ports.Parity)
                .StopBits = CType(iCommPortStopBits, Ports.StopBits)
                .DtrEnable = True
                .RtsEnable = True
                '.Encoding = System.Text.Encoding.GetEncoding(28605)     ' support char above 128
                .Encoding = System.Text.Encoding.GetEncoding("Windows-1252")
                .Open()
            End With
            ssCommStatus.Text = "Open"
            ssCommStatus.BackColor = Color.LimeGreen
            Me.btnCommPortControl.Text = "Close Port"
            bCommPort = True
            UpdateLog("App: Com Port Open")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CommPortClose()
        ' close the port
        Try
            With sp
                .DtrEnable = False
                '.RtsEnable = False
                .Close()
            End With
            ssCommStatus.Text = "Closed"
            ssCommStatus.BackColor = Color.Red
            Me.btnCommPortControl.Text = "Open Port"
            bCommPort = False
            UpdateLog("App: Com Port Closed")
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region " Serial Port Events "
    Private Sub sp_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles sp.DataReceived
        ' new data to process - review if this is better as a timer driven function
        Call spDataReceived()
    End Sub

    Private Sub spDataReceived()
        ' ## NOT USED - tmr driver event used in place of this
        ' ## TODO review if this is better as a timer driven function
        'Try
        '    ' If there is data in the commport then read it
        '    While (sp.BytesToRead <> 0)
        '        ssBufLen.Text = sp.BytesToRead.ToString
        '        sBuff = sBuff & Chr(sp.ReadChar) ' get char-by-char (asc)
        '    End While
        '    If bDEBUG Then UpdateLog("sBuff: " & sBuff)
        'Catch ex As Exception
        '    ' An exception is raised when there is no information to read.
        '    '   Don't do anything here, just let the exception go.
        'End Try

        'Try
        '    If sBuff.Length > 0 Then
        '        Call ProcessReceivedData(sBuff)
        '    End If

        'Catch ex As Exception
        'End Try
    End Sub

    Private Sub sp_ErrorReceived(sender As Object, e As SerialErrorReceivedEventArgs) Handles sp.ErrorReceived
        ' handle error
        Try
            ' maybe ingore ??
        Catch ex As Exception
        End Try
    End Sub

    Private Sub sp_PinChanged(sender As Object, e As SerialPinChangedEventArgs) Handles sp.PinChanged
        ' could be useful for hardware handshake lines; BUT other than setting the DTR now other handshaking used...
        ' eg BREAK, CD, CTS, CSR, RING
        ' Note: event for enter BREAK but not exit

        Select Case e.EventType
            Case SerialPinChange.Break
            Case SerialPinChange.CDChanged
            Case SerialPinChange.CtsChanged
            Case SerialPinChange.DsrChanged
            Case SerialPinChange.Ring
        End Select

    End Sub

#End Region

#Region " Test Data "
    ' Known Commands

    ' Request Serial Number
    ' send  BB BB 00 00 00 00 00 00 00 01 76
    ' send $BB$BB$00$00$00$00$00$00$00$01$76
    ' recv  BB BB 00 00 00 00 00 80 {lgh} {Serial Number} {CRC} {CRC}
    ' recv  BB BB 00 00 00 00 00 80 0B 31 30 30 33 33 37 36 31 31 30 32 04 29    eg Serial No. 10033761102
    ' recv $BB$BB$00$00$00$00$00$80$0B$31$30$30$33$33$37$36$31$31$30$32$04$29


    ' Login to Inverter
    ' send  BB BB 00 00 00 00 00 01 {lgh} {Serial Number} {CRC} {CRC}
    ' send  BB BB 00 00 00 00 00 01 0C 31 30 30 33 33 37 36 31 31 30 32 03 AB
    ' send $BB$BB$00$00$00$00$00$01$0C$31$30$30$33$33$37$36$31$31$30$32$03$AB
    ' recv  BB BB 00 01 00 00 00 81 01 06 01 FF
    ' recv $BB$BB$00$01$00$00$00$81$01$06$01$FF


    ' Request Single PV String output
    ' send  BB BB 01 00 00 01 01 02 00 01 7B
    ' send $BB$BB$01$00$00$01$01$02$00$01$7B
    ' recv  BB BB 00 01 01 00 01 82 {lgh} {data words} {CRC} {CRC}
    ' recv  BB BB 00 01 01 00 01 82 2A 00 D9 08 C8 00 10 00 00 13 47 00 01 00 00 00 00 00 00 00 00 00 00 00 0E 09 6C 13 86 01 5E 00 00 00 00 59 CC 00 00 00 00 00 00 06 D9
    ' recv $BB$BB$00$01$01$00$01$82$2A$00$D9$08$C8$00$10$00$00$13$47$00$01$00$00$00$00$00$00$00$00$00$00$00$0E$09$6C$13$86$01$5E$00$00$00$00$59$CC$00$00$00$00$00$00$06$D9

    'Dim sCmdGetSerial() As Byte
    'Dim sCmdLogIn() As Byte
    'Dim sCmdGetPVOutput() As Byte

#End Region

#Region " Send Serial Commands "

    Private Sub SendSerial(ByVal sSend() As Byte)
        ' send byte commands to serial port
        Try
            If sp.IsOpen Then
                sp.Write(sSend, 0, sSend.Count)
                UpdateLog("App: Sent  " & BytesToString(sSend, True))
            End If
        Catch ex As Exception
            ' do nothin...
        End Try
    End Sub

    Private Sub btnGetSerial_Click(sender As Object, e As EventArgs) Handles btnGetSerial.Click
        ' send the' Get inverter serial number command via comport in hex format
        Dim sSendMsg As Byte()
        sSendMsg = txtGetSerial.Text.TrimEnd.Split(" "c).Select(Function(n) Convert.ToByte(Convert.ToInt32(n, 16))).ToArray()

        ' get CRC for msg
        Dim sCRC() As Byte
        sCRC = CalculateCRC(sSendMsg)
        Array.Resize(Of Byte)(sSendMsg, sSendMsg.Length + 2)
        sSendMsg(UBound(sSendMsg) - 1) = sCRC(0)
        sSendMsg(UBound(sSendMsg) - 0) = sCRC(1)

        ' display CRC
        txtGetSerialCRC.Text = BytesToString(sCRC, True)

        ' send to comport
        SendSerial(sSendMsg)

    End Sub

    Private Sub btnLogIn_Click(sender As Object, e As EventArgs) Handles btnSendLogIn.Click
        ' using serial number and send the 'log in' command

        Dim sSendMsg As Byte()
        sSendMsg = txtSendLogIn.Text.TrimEnd.Split(" "c).Select(Function(n) Convert.ToByte(Convert.ToInt32(n, 16))).ToArray()

        ' get CRC for msg
        Dim sCRC() As Byte
        sCRC = CalculateCRC(sSendMsg)
        Array.Resize(Of Byte)(sSendMsg, sSendMsg.Length + 2)
        sSendMsg(UBound(sSendMsg) - 1) = sCRC(0)
        sSendMsg(UBound(sSendMsg) - 0) = sCRC(1)

        ' display CRC
        txtSendLogInCRC.Text = BytesToString(sCRC, True)

        ' send to comport
        SendSerial(sSendMsg)

    End Sub

    Private Sub btnGetPVOutput_Click(sender As Object, e As EventArgs) Handles btnGetPVOutput.Click
        ' send the 'get Single PV Output command

        Dim sSendMsg As Byte()
        sSendMsg = txtGetPVOutput.Text.TrimEnd.Split(" "c).Select(Function(n) Convert.ToByte(Convert.ToInt32(n, 16))).ToArray()

        ' get CRC for msg
        Dim sCRC() As Byte
        sCRC = CalculateCRC(sSendMsg)
        Array.Resize(Of Byte)(sSendMsg, sSendMsg.Length + 2)
        sSendMsg(UBound(sSendMsg) - 1) = sCRC(0)
        sSendMsg(UBound(sSendMsg) - 0) = sCRC(1)

        ' display CRC
        txtGetPVOutputCRC.Text = BytesToString(sCRC, True)

        ' send to comport
        SendSerial(sSendMsg)
    End Sub

#End Region

    Private Function CalculateCRC(ByVal Packet() As Byte) As Byte()
        'Calculate simple 2 bytes CRC on the packet 
        Dim CheckSum As Integer, CRC(1) As Byte

        Try
            CheckSum = 0
            For i = 0 To CByte((UBound(Packet)))
                CheckSum += Convert.ToInt32(Packet(i))
            Next i

            'return the CRC as a byte array in high byte/low byte order
            CRC(0) = CByte((CheckSum \ 256))
            CRC(1) = CByte((CheckSum - ((CheckSum \ 256) * 256)))

            Return CRC
        Catch ex As Exception
            Return CRC
        End Try

    End Function

    Private Sub tmrSP_Tick(sender As Object, e As EventArgs) Handles tmrSP.Tick
        ' check and get any data from the SerialPort
        Try
            ' read all in buffer and add to FIFO
            sRcvBuff += sp.ReadExisting()

            'Try
            '    ' If there is data in the commport then read it
            '    While (sp.BytesToRead <> 0)
            '        ssBufLen.Text = sp.BytesToRead.ToString
            '        sBuff = sBuff & Chr(sp.ReadChar) ' get char-by-char (asc)
            '    End While
            '    If bDEBUG Then UpdateLog("sBuff: " & sBuff)
            'Catch ex As Exception
            '    ' An exception is raised when there is no information to read.
            '    '   Don't do anything here, just let the exception go.
            'End Try

        Catch ex As Exception
            ' do nothin
        End Try

        ' Serial FIFO
        ' look for and pass through messages
        '   sRcvBuff holds message/s

        If sRcvBuff <> "" Then
            ProcessReceivedData(sRcvBuff)
        End If

    End Sub

    Private Sub ProcessReceivedData(ByRef sBuff As String)
        Dim iPtr As Integer                 ' pointer to the message terminating characters
        Dim bMTC As Boolean                 ' MTC found
        Dim sRcvd As String                 ' Received data block
        Dim sData() As String               ' Array of bytes received

        Try

            ' process the serial data receive
            If sBuff.Length = 0 Then
                'just in case there is nothing in the buffer we can quit here
                Exit Sub
            End If

            ssBufLen.Text = "sBuff len = " & sBuff.Length.ToString
            UpdateLog("App: Rcvd " & StringToHex(sBuff, True))

            ' find start of message
            ' {BB}{BB}{da}{sa}{nn}{nn}{cmd}{subcmd}{lgh}{data}{crc}{crc}
            ' BB BB nn nn nn nn nn nn xx cc cc






            ''Find Start Delim
            'iRcvdMsgStartPtr = sRcvdMsg.IndexOf(sStartDelim)
            'If iRcvdMsgStartPtr >= 0 Then
            '    ' remove any leading chars from FIFO
            '    sRcvdMsg.Remove(0, iRcvdMsgStartPtr)

            '    'Find End Delim
            '    iRcvdMsgEndPtr = sRcvdMsg.IndexOf(sEndDelim)
            '    If iRcvdMsgEndPtr > iRcvdMsgStartPtr Then
            '        sMsg = sRcvdMsg.Substring(iRcvdMsgStartPtr, (iRcvdMsgEndPtr - iRcvdMsgStartPtr + 1))

            '        ' remove msg from FIFO
            '        sRcvdMsg = sRcvdMsg.Remove(0, (iRcvdMsgEndPtr - iRcvdMsgStartPtr + 1))

            '        ' send msg for processing
            '        If Not chkLvwFilter.Checked Then
            '            UpdateLog(sMsg)
            '        End If
            '        ReviewRecvd(sMsg)
            '    Else
            '        ' remove 
            '        sRcvdMsg = sRcvdMsg.Remove(0, iRcvdMsgStartPtr)

            '    End If
            'End If





            '' look for message terminating characters '0x12 0x0A 0x0D'
            'iPtr = InStr(sBuff, sMTC)
            'Do While iPtr > 0
            '    ' sMTC found
            '    bMTC = True
            '    sRcvd = Mid$(sBuff, 1, iPtr - 1)

            '    ' save raw data, including MTC, for reprocessing
            '    SaveRaw(Mid$(sBuff, 1, iPtr + iLenMTC), True)

            '    ' remove from the message queue
            '    sBuff = Mid$(sBuff, iPtr + iLenMTC)

            '    ' look for another message
            '    iPtr = InStr(sBuff, sMTC)

            '    ' remove unused characters eg '0x0E' '0x0F' and '0x12' from rcvd buffer
            '    Try
            '        If InStr(sRcvd, Chr(&HE)) >= 0 Then sRcvd = sRcvd.Remove(InStr(sRcvd, Chr(&HE)) - 1, 1)
            '    Catch ex As Exception
            '    End Try

            '    Try
            '        If InStr(sRcvd, Chr(&HF)) >= 0 Then sRcvd = sRcvd.Remove(InStr(sRcvd, Chr(&HF)) - 1, 1)
            '    Catch ex As Exception
            '    End Try

            '    Try
            '        If InStr(sRcvd, Chr(&H12)) >= 0 Then sRcvd = sRcvd.Remove(InStr(sRcvd, Chr(&H12)) - 1, 1)
            '    Catch ex As Exception
            '    End Try

            '    ' check for and remove first char if is a space
            '    If Mid(sRcvd, 1, 1) = " " Then
            '        sRcvd = sRcvd.Remove(0, 1)
            '    End If

            '    If bMTC Then
            '        ' send to log
            '        UpdateLog("Rcvd: " & sRcvd)

            '        ' Process raw data
            '        sData = Split(sRcvd, " ")   ' use spaces as delimiters

            '        ' split string and collect key data items
            '        ' example
            '        '   3573 17:35:46 11/07/16 Access Granted at Door 4 to User 105 Neil Blanchard
            '        ' iMsgId
            '        ' dtTime
            '        ' dtDate
            '        ' iDoor
            '        ' iUser
            '        ' sUserName - maybe in parts
            '        If sData.Length >= 12 Then
            '            Dim nEntry As New AccessMessage
            '            If IsNumeric(sData(0)) Then nEntry.iMsgId = CInt(sData(0).ToString)
            '            If IsDate(sData(2)) Then nEntry.dtDate = CDate(sData(2))
            '            If IsDate(sData(1)) Then nEntry.dtTime = CDate(sData(1))
            '            nEntry.sAccess = sData(4)  ' need to check for denied access events
            '            If IsNumeric(sData(7)) Then nEntry.iDoor = CInt(sData(7).ToString)
            '            If IsNumeric(sData(10)) Then nEntry.iUser = CInt(sData(10).ToString)
            '            ' check for number of parts beyond iUser
            '            For i = 11 To sData.Length - 1
            '                nEntry.sUserName &= sData(i)
            '                If i < sData.Length - 1 Then nEntry.sUserName &= " "
            '            Next

            '            If bDEBUG Then
            '                UpdateLog("iMsgId   : " & nEntry.iMsgId.ToString)
            '                UpdateLog("dtDate   : " & nEntry.dtDate.ToString("yyyy-MM-dd"))
            '                UpdateLog("dtTime   : " & nEntry.dtTime.ToString("HH:mm:ss"))
            '                UpdateLog("sAccess  : " & nEntry.sAccess.ToString)
            '                UpdateLog("iDoor    : " & nEntry.iDoor.ToString)
            '                UpdateLog("iUser    : " & nEntry.iUser.ToString)
            '                UpdateLog("sUserName: " & nEntry.sUserName.ToString)
            '            End If

            '            ' send to CSV
            '            SaveCSV(nEntry.iMsgId.ToString & "," &
            '                    nEntry.dtDate.ToString("yyyy-MM-dd") & "," &
            '                    nEntry.dtTime.ToString("HH:mm:ss") & "," &
            '                    nEntry.sAccess.ToString & "," &
            '                    nEntry.iDoor.ToString & "," &
            '                    nEntry.iUser.ToString & "," &
            '                    PrepForCSV(nEntry.sUserName.ToString))
            '        End If
            '            bMTC = False
            '    Else
            '        sRcvd = ""
            '    End If
            'Loop
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SaveRaw(ByVal sRaw As String, ByVal bHex As Boolean)
        ' save raw data from serial port (via sBuff)

        Try
            ' check directoy exists; if not create it
            If Not Directory.Exists(sAppPath) Then
                Directory.CreateDirectory(sAppPath)
            End If

            If Not File.Exists(sAppPath & "\" & sRawFileName) Then
                ' files doesnt exist
                fs = New FileStream(sAppPath & "\" & sRawFileName, FileMode.OpenOrCreate)
                sw = New StreamWriter(fs)
                ' write data in to logFile
                If Not bHex Then
                    sw.WriteLine(sRaw)
                Else
                    sw.WriteLine(StringToHex(sRaw, True))
                End If
                sw.Close()
                fs.Close()
            Else
                ' file exists
                fs = New FileStream(sAppPath & "\" & sRawFileName, FileMode.Append)
                sw = New StreamWriter(fs)
                ' write data in to logFile
                If Not bHex Then
                    sw.WriteLine(sRaw)
                Else
                    sw.WriteLine(StringToHex(sRaw, True))
                End If
                sw.Close()
                fs.Close()
            End If

        Catch ex As Exception
            ' could be due folder not existing
            ' file not available for open/write operation
            Console.WriteLine("The process failed: {0}", ex.ToString())
        End Try
    End Sub

#Region " Update Log and Log File "
    Private Delegate Sub UpdateLogCallback(ByVal text As String)
    Private Sub UpdateLog(ByVal text As String)
        ' Provides UI Log function with timestamp
        ' Appends text to the log field, followed by a line break.
        If (InvokeRequired) Then
            Invoke(New UpdateLogCallback(AddressOf UpdateLog), text)
        Else

            Dim sLogText As String = Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " " & text

            ' Last entry at top of list
            lvwLog.Items.Insert(0, sLogText & ControlChars.NewLine)
            'lvwLogchdLog.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            chdLog.Width = -2   ' resize to fill

            Try
                ' check directoy exists; if not create it
                If Not Directory.Exists(sAppPath) Then
                    Directory.CreateDirectory(sAppPath)
                End If
                If Not File.Exists(sAppPath & "\" & sLogFileName) Then
                    ' files doesnt exist
                    fs = New FileStream(sAppPath & "\" & sLogFileName, FileMode.OpenOrCreate)
                    sw = New StreamWriter(fs)
                    ' write data in to logFile
                    sw.WriteLine(sLogText)
                    sw.Close()
                    fs.Close()
                Else
                    ' file exists
                    fs = New FileStream(sAppPath & "\" & sLogFileName, FileMode.Append)
                    sw = New StreamWriter(fs)
                    ' write data in to logFile
                    sw.WriteLine(sLogText)
                    sw.Close()
                    fs.Close()
                End If

            Catch ex As Exception
                ' could be due folder not existing
                ' file not available for open/write operation
                Console.WriteLine("The process failed: {0}", ex.ToString())
            End Try
        End If
    End Sub

    Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click
        lvwLog.Clear()
    End Sub

#End Region

#Region " Misc Functions "

    Private Function CheckFolderExists(ByVal sPath As String) As Boolean
        ' check if the folder exists; return result
        CheckFolderExists = False
        Try
            If Directory.Exists(sPath) Then
                ' This path is a directory.
                Return True
            Else
                Console.WriteLine("{0} is not a valid directory.", sPath)
                Return False
            End If
        Catch ex As Exception
            Console.WriteLine("{0} is not a valid directory.", sPath)
            Return False
        End Try
    End Function

    Function HexToString(ByVal sHex As String) As String
        ' convert sHex to readable string
        Try
            Dim sText As New System.Text.StringBuilder(sHex.Length \ 2)

            For i As Integer = 0 To sHex.Length - 2 Step 2
                sText.Append(Chr(Convert.ToByte(sHex.Substring(i, 2), 16)))
            Next

            Return sText.ToString
        Catch ex As Exception
            ' do nothin
            Return ""
        End Try
    End Function

    Public Function StringToHex(ByRef sText As String, ByVal bSpace As Boolean) As String
        Try
            Dim sVal As String
            Dim sHex As String = ""
            While sText.Length > 0
                sVal = Conversion.Hex(Strings.Asc(sText.Substring(0, 1).ToString())).PadLeft(2, "0"c)
                sText = sText.Substring(1, sText.Length - 1)
                sHex = sHex & sVal
                If bSpace Then If sText.Length > 0 Then sHex = sHex & " "
            End While
            Return sHex
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function BytesToString(ByVal Input As Byte(), ByVal bSpace As Boolean) As String
        Try
            Dim Result As New System.Text.StringBuilder(Input.Length * 2)
            Dim Part As String
            For Each b As Byte In Input
                Part = Conversion.Hex(b)
                If Part.Length = 1 Then Part = "0" & Part
                Result.Append(Part)
                If bSpace Then If Result.Length > 0 Then Result.Append(" ")
            Next
            Return Result.ToString()
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Shared Function ProcessStringCtrlChars(ByVal strEdit As String) As String
        Dim strBuilder As New StringBuilder(strEdit)
        Dim sNewString As String = ""

        ' Array for ASCII Control Chars
        Dim sASCIIcontrolchars() As String = {"NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "TAB",
                                    "LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3", "DC4",
                                    "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS", "RS", "US"}

        'if length  of strBuilder is zero then no iterations are performed in the
        'while loop below.
        ' expect last two chars to be CRLF eg Chrs 13 and 10
        Dim count As Integer = 0
        Dim len As Integer = strBuilder.Length ' - 2
        Dim ch As Char
        Dim chPrev As Char

        While (count < len)
            ch = strBuilder.Chars(count)
            Select Case Asc(ch)
                Case Is <= 0
                    'strBuilder.Replace(ch, "")
                Case Is < 32
                    'strBuilder.Replace(ch, "{" + sASCIIcontrolchars(Asc(ch)) + "}")
                    sNewString = sNewString + "{" + sASCIIcontrolchars(Asc(ch)) + "}"

                Case Is >= 32
                    sNewString = sNewString + ch

                Case Is > 127
                    'strBuilder.Replace(ch, "")
            End Select
            count = count + 1
            chPrev = ch
            If Asc(chPrev) = 10 And Asc(ch) = 13 Then
                sNewString = sNewString + vbCrLf
            ElseIf Asc(chPrev) = 13 And Asc(ch) = 10 Then
                sNewString = sNewString + vbCrLf
            End If
        End While
        Return sNewString + vbCrLf
    End Function

#End Region

End Class
