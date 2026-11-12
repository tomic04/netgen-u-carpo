Imports System
Imports System.Environment

Module GetSpecialFolderLocationDotnet

    Public Function GetSpecialFolderLocationDotNet(ByVal KojiFolder As System.Environment.SpecialFolder) As String

        Dim strPath As String = ""


        Try
            strPath = Environment.GetFolderPath(KojiFolder)



        Finally
            If Len(Trim(strPath)) > 0 Then
                GetSpecialFolderLocationDotNet = strPath
            Else
                GetSpecialFolderLocationDotNet = ""
            End If

        End Try

    End Function

    Public Function UserApplicationDataFolderDotNet() As String

        Dim strPath As String = ""


        Try

            strPath = GetSpecialFolderLocationDotNet(SpecialFolder.ApplicationData)



        Finally
            If Len(Trim(strPath)) > 0 Then
                UserApplicationDataFolderDotNet = strPath
            Else
                UserApplicationDataFolderDotNet = ""
            End If
        End Try

    End Function

    Public Function SpinAppDataFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        SpinAppDataFolderDotNet = UserApplicationDataFolderDotNet() & "\SPIN Informatica\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(SpinAppDataFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(SpinAppDataFolderDotNet)
        End If
    End Function

    Public Function JupiterAppDataFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        JupiterAppDataFolderDotNet = SpinAppDataFolderDotNet() & "Jupiter Software\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(JupiterAppDataFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(JupiterAppDataFolderDotNet)
        End If
    End Function

    Public Function JupiterGridSettingsFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        JupiterGridSettingsFolderDotNet = JupiterAppDataFolderDotNet() & "Grid Settings\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(JupiterGridSettingsFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(JupiterAppDataFolderDotNet)
        End If
    End Function

    Public Function AllUsersApplicationDataFolderDotNet() As String

        Dim strPath As String = ""


        Try

            strPath = GetSpecialFolderLocationDotNet(SpecialFolder.CommonApplicationData)



        Finally
            If Len(Trim(strPath)) > 0 Then
                AllUsersApplicationDataFolderDotNet = strPath
            Else
                AllUsersApplicationDataFolderDotNet = ""
            End If
        End Try

    End Function

    Public Function AllUsersSpinAppDataFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        AllUsersSpinAppDataFolderDotNet = AllUsersApplicationDataFolderDotNet() & "\SPIN Informatica\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(AllUsersSpinAppDataFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(AllUsersSpinAppDataFolderDotNet)
        End If
    End Function

    Public Function AllUsersJupiterAppDataFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        AllUsersJupiterAppDataFolderDotNet = AllUsersSpinAppDataFolderDotNet() & "Jupiter Software\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(AllUsersJupiterAppDataFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(AllUsersJupiterAppDataFolderDotNet)
        End If
    End Function

    Public Function AllUsersJupiterGridSettingsFolderDotNet() As String

        Dim getInfo As System.IO.DirectoryInfo

        AllUsersJupiterGridSettingsFolderDotNet = AllUsersJupiterAppDataFolderDotNet() & "Grid Settings\"
        getInfo = My.Computer.FileSystem.GetDirectoryInfo(AllUsersJupiterGridSettingsFolderDotNet)
        If getInfo.Exists = False Then
            My.Computer.FileSystem.CreateDirectory(AllUsersJupiterGridSettingsFolderDotNet)
        End If
    End Function


    Public Function PutanjaSlike() As String
        Dim strTvrtka As String = ""
        Dim Putanja As String = ""
        Dim udaljenost As Long = 0

        Try
            'strTvrtka = GetSetting("Jupiter", "SpinClient", "MRUFirma", "")
            strTvrtka = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\VB and VBA Program Settings\Jupiter\SpinClient", "MRUFirma", "")
            'strTvrtka = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Environment\", "TEMP", "")


            If Left(My.Application.Info.DirectoryPath, 1) <> "\" Then
                Putanja = Left(My.Application.Info.DirectoryPath, 1) & ":\JupiterClient\" & strTvrtka & "\logos\"
            Else

                udaljenost = InStr(3, My.Application.Info.DirectoryPath, "\")
                Putanja = Mid(My.Application.Info.DirectoryPath, 1, udaljenost) & "JupiterClient\" & strTvrtka & "\logos\"
            End If

        Finally
            If Len(Trim(Putanja)) > 0 Then
                PutanjaSlike = Putanja
            Else
                PutanjaSlike = ""
            End If
        End Try
    End Function

End Module
