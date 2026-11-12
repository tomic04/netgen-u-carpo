Imports System.IO
Imports System.Reflection

Public Class Form6

    Public EasyPrintWorkaround As Boolean
    Public cls1 As Class1

    Dim Preview As String = ""
    Dim Pisac As String = ""
    Dim Kopije As String = ""

    Public OKClicked As Boolean = False

    Dim Translated As Boolean = False

    Private Sub FormInit()

        cls1 = New Class1

        Dim dtPisaci As New DataTable
        Dim Red As DataRow

       
        Try

            Me.Text = "Parametri ispisa [ver. " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." _
                     & My.Application.Info.Version.Build & "." & My.Application.Info.Version.Revision & "]"



            Preview = GetSetting("Jupiter", "PrintDlg2010", "Preview")

            If Len(Trim(Preview)) > 0 Then
                If Preview = "1" Then
                    rbPreview.Checked = True
                Else
                    rbPisaci.Checked = True
                End If
            End If

            ' Kopije = My.Settings.Kopije

            Kopije = GetSetting("Jupiter", "PrintDlg2010", "Copies")

            If Len(Trim(Kopije)) > 0 Then
                numCopies.Value = Kopije
            Else
                numCopies.Value = 1
            End If

            dtPisaci.Columns.Add("Naziv", Type.GetType("System.String"))

        

            For Each pkInstalledPrinters In Drawing.Printing.PrinterSettings.InstalledPrinters

                Red = dtPisaci.NewRow()
                Red(0) = pkInstalledPrinters
                If dtPisaci.Rows.Count > 0 Then
                    dtPisaci.Rows.InsertAt(Red, dtPisaci.Rows.Count - 1)
                Else
                    dtPisaci.Rows.InsertAt(Red, 0)
                End If
                dtPisaci.AcceptChanges()

            Next pkInstalledPrinters


            Pisac = GetSetting("Jupiter", "PrintDlg2010", "Printer")


            cbPisaci.DataSource = dtPisaci
            cbPisaci.DisplayMember = dtPisaci.Columns(0).Caption
            cbPisaci.ValueMember = dtPisaci.Columns(0).Caption

            ' My.Settings.Kopije = numCopies.Value

            If Len(Trim(Pisac)) > 0 Then
                cbPisaci.SelectedValue = Pisac
            Else
                cbPisaci.SelectedValue = dtPisaci.Rows(0)(0)
            End If



        Catch ex As Exception
            MsgBox("Greška inicijalizaciji ugrađenog printdialoga FormInit() :" & vbCrLf & ex.Message)

        End Try

    End Sub

    Public Sub PrintDocument(ByVal pConnectionString As String, ByVal ID() As Integer, ByVal Report As String, ByVal ParametriStora1 As String, Optional ByVal IndikatorDirektPrintId As Integer = 0)

        On Error Resume Next

        Dim strServerPath As String = ""



        'MsgBox(UBound(ID))

        FormInit()

        strServerPath = GetServerPath()

        strServerPath = MOJremotedllpath

        If strServerPath.Trim = "" Then
            strServerPath = "C:\JupiterClient"
        End If

        If UBound(ID) > 1 Then
            rbPreview.Enabled = False
        Else
            rbPreview.Enabled = True
        End If

        'numCopies.Value = 1

        OKClicked = False
        Me.ShowDialog()

        'Try

        If OKClicked = True Then

            Dim i As Integer = 0


            If rbPreview.Checked Then

                Dim a As Assembly = Assembly.Load("rptDevRepx")
                Dim o As Object = a.CreateInstance("rptDevRepx.clsReport")
                o.isnetgen = 1
                o.parent = Handle

                'MsgBox(GetServerPath())
                'MsgBox(Report)
                ' MsgBox(ParametriStora1)

                If Report.ToUpper.Contains("FROMDB-") Then
                    Report = Report.Replace("FROMDB-", "").Replace("fromdb-", "")
                    Report = Report.Replace("REPX:/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "")

                    If Report.IndexOf("@") > 0 Then
                        ParametriStora1 = Report.Substring(Report.IndexOf("@"), Report.Length - Report.IndexOf("@"))
                        Report = Report.Substring(0, Report.IndexOf("@"))
                    End If
                    Report = Report.Replace("/", "")

                    o.DoPrintDB(pConnectionString, ID(0), Report.Replace("/", ""), Report.Replace("/", ""), 0, cbPisaci.Text, ParametriStora1)
                Else

                    cls1.NapuniCBLog(" pConnectionString : " & pConnectionString)
                    cls1.NapuniCBLog(" ID(0) : " & ID(0))
                    cls1.NapuniCBLog(" Putanja : " & strServerPath & "\SharedDLLs\repx")
                    cls1.NapuniCBLog(" Report : " & Report.Replace("REPX:/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", ""))

                    o.DoPrint2(pConnectionString, ID(0), strServerPath & "\SharedDLLs\repx", Report.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", ""), 0, cbPisaci.Text, ParametriStora1)

                End If


            Else

                For i = LBound(ID) To UBound(ID)
                    Dim a As Assembly = Assembly.Load("rptDevRepx")
                    Dim o As Object = a.CreateInstance("rptDevRepx.clsReport")
                    o.isnetgen = 1
                    o.parent = Handle

                    If Report.ToUpper.Contains("FROMDB-") Then
                        Report = Report.Replace("FROMDB-", "").Replace("fromdb-", "")
                        Report = Report.Replace("REPX:/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "")

                        If Report.IndexOf("@") > 0 Then
                            ParametriStora1 = Report.Substring(Report.IndexOf("@"), Report.Length - Report.IndexOf("@"))
                            Report = Report.Substring(0, Report.IndexOf("@"))
                        End If
                        Report = Report.Replace("/", "")

                        o.DoPrintDB(pConnectionString, ID(i), Report.Replace("/", ""), Report.Replace("/", ""), numCopies.Value, cbPisaci.Text, ParametriStora1)
                    Else
                        o.DoPrint2(pConnectionString, ID(i), strServerPath & "\SharedDLLs\repx", Report.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", ""), numCopies.Value, cbPisaci.Text, ParametriStora1)
                    End If


                    o = Nothing
                    a = Nothing
                Next


            End If
        End If


        'Pisac = GetSetting("Jupiter", "PrintDlg2010", "Printer")
        'Preview = GetSetting("Jupiter", "PrintDlg2010", "Preview")
        'Kopije = GetSetting("Jupiter", "PrintDlg2010", "Copies")

        SaveSetting("Jupiter", "PrintDlg2010", "Printer", cbPisaci.Text)
        SaveSetting("Jupiter", "PrintDlg2010", "Preview", IIf(rbPreview.Checked, "1", "0"))
        SaveSetting("Jupiter", "PrintDlg2010", "Copies", numCopies.Value)
        'My.Settings.Kopije = numCopies.Value

        'Catch ex As Exception
        '    MsgBox("Greška u ugrađenom printdialogu PrintDocument()  :" & vbCrLf & ex.Message)

        'End Try
    End Sub

    Public Function GetServerPath() As String
        'Dim pString As String, tString As String

        On Error Resume Next

        Dim getFileInfo As FileInfo = Nothing
        Dim JupiterPutanja As String = ""
        GetServerPath = ""
        'Try
        EasyPrintWorkaround = False

        getFileInfo = My.Computer.FileSystem.GetFileInfo(JupiterAppDataFolderDotNet() & "JupiterClient.ini")

        If getFileInfo.Exists = True Then
            JupiterPutanja = JupiterAppDataFolderDotNet() & "JupiterClient.ini"
        Else
            getFileInfo = Nothing
            getFileInfo = My.Computer.FileSystem.GetFileInfo(JupiterAppDataFolderDotNet() & "JupiterClient.ini")
            If getFileInfo.Exists = True Then
                JupiterPutanja = JupiterAppDataFolderDotNet() & "JupiterClient.ini"
            End If
        End If

        'MsgBox(JupiterPutanja)

        If JupiterPutanja.Trim = "" Then
            JupiterPutanja = "C:\JupiterClient\JupiterClient.ini"
        End If

        If Len(Trim(JupiterPutanja)) > 0 Then
            Dim citac As TextReader
            citac = File.OpenText(JupiterPutanja)
            Dim linija As String = "a"

            If Not (citac Is Nothing) Then
                Do While linija <> ""
                    linija = citac.ReadLine()

                    If Not (linija Is Nothing) Then
                        If linija.Contains("RemoteDllPath") Then
                            Return linija.Substring(14)
                        End If

                        If Trim(LCase(linija)) = "easyprintworkaround=true" Then
                            EasyPrintWorkaround = True
                        End If

                    End If
                Loop
            End If

            citac.Close()
        End If


        'Catch ex As Exception
        'MsgBox("Greška u ugrađenom printdialogu GetServerPath() :" & vbCrLf & ex.Message)
        ' Return ""
        ' End Try

    End Function

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        My.Settings.Kopije = numCopies.Value
        SaveSetting("Jupiter", "PrintDlg2010", "Copies", numCopies.Value)
        OKClicked = True
    End Sub

    Private Sub btnPrintOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrintOptions.Click

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        OKClicked = False
        Me.Hide()
    End Sub

    Private Sub cbPisaci_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPisaci.SelectedIndexChanged

    End Sub

    Private Sub rbPisaci_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbPisaci.CheckedChanged

    End Sub

    Private Sub Form6_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Translated = False Then
            'TranslateForm(Me)

            btnOK.Text = Translate(btnOK.Text)
            btnCancel.Text = Translate(btnCancel.Text)
            btnPrintOptions.Text = Translate(btnPrintOptions.Text)
            label1.Text = Translate(label1.Text)

            rbLandscape.Text = Translate(rbLandscape.Text)
            rbPortrait.Text = Translate(rbPortrait.Text)

            rbPreview.Text = Translate(rbPreview.Text)
            rbPisaci.Text = Translate(rbPisaci.Text)
            gbOrientation.Text = Translate(gbOrientation.Text)

            groupBox1.Text = Translate(groupBox1.Text)

            Me.Text = Translate(Me.Text)

            Translated = True
        End If

        Me.Refresh()

    End Sub
End Class