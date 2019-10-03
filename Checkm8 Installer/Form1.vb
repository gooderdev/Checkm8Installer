Imports System.IO.Compression

Public Class Form1
    Private Sub BackgroundThread_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles backgroundThread.DoWork
    End Sub

    Public Shared Function logger(ByRef textr)
        If Form1.logbox.Text = "" Then
            Form1.logbox.Text = textr
        Else
            Form1.logbox.Text = Form1.logbox.Text & vbCrLf & textr
        End If
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'backgroundThread.RunWorkerAsync()
        Try
            If My.Computer.FileSystem.DirectoryExists("checkm8-files") Then
                Dim r = MsgBox("We've detected left over checkm8 files from a previous time this program was used, would you like to delete these files? If not, we can't reinstall checkm8 for Windows.", vbYesNo)
                If Not r = vbYes Then
                    logger("Installation cancelled.")
                    Exit Sub
                End If
                My.Computer.FileSystem.DeleteDirectory("checkm8-files", FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If
            If My.Computer.FileSystem.FileExists("checkm8.zip") Then
                My.Computer.FileSystem.DeleteFile("checkm8.zip")
            End If
            If My.Computer.FileSystem.FileExists("libusb.zip") Then
                My.Computer.FileSystem.DeleteFile("libusb.zip")
            End If
            If My.Computer.FileSystem.FileExists("pythoninstaller.exe") Then
                My.Computer.FileSystem.DeleteFile("pythoninstaller.exe")
            End If
            CheckForIllegalCrossThreadCalls = False
            Dim f = MsgBox("Is your device in DFU mode and connected to the computer?", vbYesNo)
            If Not f = vbYes Then
                MsgBox("Put your device into DFU mode and connect it to the computer, once that is done press ok", vbOKOnly)
            End If
            f = MsgBox("Is Python 3.7.x installed?", vbYesNo)
            If Not f = vbYes Then
                logger("Downloading the Python 3.7.4 installer...")
                My.Computer.Network.DownloadFile("http://goodertech.com/python.exe", "pythoninstaller.exe")
                MsgBox("Press OK to start the python installation process, make sure to checkmark ""Add Python 3.7 to path"" once you finish installing python press ""Ok"" on the next prompt.")
                logger("Starting the Python installer...")
                Process.Start("pythoninstaller.exe")
j:
                f = MsgBox("Press Ok once you've installed Python 3.7.4.")
                If Not f = vbOK Then
                    GoTo j
                End If
            End If
            logger("Downloading necessary checkm8 files...")
            If Not My.Computer.FileSystem.FileExists("checkm8.zip") Then
                My.Computer.Network.DownloadFile("http://goodertech.com/checkm8script.zip", "checkm8.zip")
            End If
            If Not My.Computer.FileSystem.FileExists("libusb.zip") Then
                My.Computer.Network.DownloadFile("http://goodertech.com/libusb.zip", "libusb.zip")
            End If
            logger("Successully downloaded checkm8 files.")
            logger("Decompressing checkm8 files...")
            If Not My.Computer.FileSystem.DirectoryExists("checkm8-files") Then
                My.Computer.FileSystem.CreateDirectory("checkm8-files")
                My.Computer.FileSystem.CreateDirectory("checkm8-files/checkm8/")
                My.Computer.FileSystem.CreateDirectory("checkm8-files/libusb/")
            End If
            ZipFile.ExtractToDirectory("checkm8.zip", "checkm8-files/checkm8/")
            ZipFile.ExtractToDirectory("libusb.zip", "checkm8-files/libusb/")
            logger("Successfully decompressed checkm8 files.")
            MsgBox("Another installer is about to pop up after you click ok, once it does, select ""Install a device filter"" and click next. In the list, find your device in DFU mode. It should say ""Apple Mobile Device (DFU Mode)"", click on it and then press install. After it completes, close the pop up.")
            logger("Running necessary scripts...")
            Process.Start("checkm8-files\libusb\libusb\bin\amd64\install-filter-win.exe")
            MsgBox("Press Ok once the pop up is closed.")
            MsgBox("Another pop up is going to open after you click Ok, once it does, click next on the popup. Select ""Apple Mobile Device (DFU Mode)"" and then click next. Check that you've chosen the right device, then click next. On the new window that opens, choose your desktop to save this .inf file.")
            Process.Start("checkm8-files\libusb\libusb\bin\inf-wizard.exe")
            MsgBox("Click Ok once you've completed the task.")
            My.Settings.safemode = True
            My.Settings.Save()
            MsgBox("Now here comes the most tedious part. Due to Windows not allowing unsigned third party drivers to be installed while not in safe mode, we'll have to boot into it. Bring up your power down options, and while holding shift, click restart. Keep holding shift until a blue screen comes up. Click ""Troubleshoot"", then click ""Advanced options"". Click ""Startup Settings"", then click restart. When a list of options comes up, press '7' and let your PC boot. Sign in as normal, and open back up this program.")
        Catch
            logger("An error has occured, please report this in the reddit post: " & ErrorToString())
            Exit Sub
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        logger("Checkm8 Installer 1.0 by Most Gooder")
        If My.Settings.safemode Then
            MsgBox("Welcome back! Now that you're in safe mode we can continue to the next step!")
            MsgBox("Open up Device Manager, and find your Apple device (it's usually down the bottom in one of the USB categories). Right click on it, and choose ""Update Driver"". Choose ""Browse my computer for driver software"". Click ""Let me choose from a list of available drives on my computer"". On the bottom right, click ""Have Disk..."". In the new window, click ""Browse"". Navigate to your desktop, and select the .inf file you made earlier. Click ""Open"", then ""Okay"". Click ""Next"". On the window that pops up, simply confirm your choice. Once it's done, press Ok.")
            Dim psi As New ProcessStartInfo()
            psi.Verb = "runas"
            psi.FileName = "cmd.exe"
            MsgBox("Heres the last step! 2 commands are going to be put into the log, open up a adminstator command prompt and paste those 2 commands into it in order, after that you'll be done!")
            Dim command As String = "cd " & System.AppDomain.CurrentDomain.BaseDirectory() & "\checkm8-files\checkm8\ipwndfu-master\"
            logger("Command 1: " & command)
            command = "python.exe ipwndfu -p"
            logger("Command 2: " & command)
            My.Settings.safemode = False
            My.Settings.Save()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MsgBox("Checkm8, libusb are property of their respectful owners, not Most Gooder. Some of the prompts that you'll see in this program were copied straight from ""u/NeoBassMakesWafflez""
 on his reddit post ""[Tutorial] How to run Checkm8 on Windows 10"", plus this program wouldn't of been made without them! The version of checkm8 used in this program was modified by Geohot to run on Windows. If I missed anybody in the credits please let me know!")
    End Sub
End Class
