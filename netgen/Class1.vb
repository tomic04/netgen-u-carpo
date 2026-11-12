Imports System.Web.Services
Imports System.Deployment
Imports System.Drawing
Imports System.Windows.Forms
Imports Janus.Windows.GridEX
Imports System.Reflection
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Web
Imports System.Net
Imports System.Text
Imports DevExpress.XtraEditors.Repository
Imports System.Drawing.Printing

 
Public Class Class1


    Public strProxy555 As String = ""
    Public strProxy As String = ""
    Public strProjectCaption As String = ""
    'Public Shared strXML As String = ""
    'Public Shared strXMLfile As String = ""
    Public MasterID As String = ""
    Public strTemp As String = ""
    Public strXMLsakojimRadimzaCBlog As String = ""
    Public bCloseParent As Boolean = False
    Public strSviParametriuModulu As String
    Public strStartupStoraDetalj As String = ""
    Public strDMSDet As String = ""
    Public strDetMoveLast As String = ""
    Public strMasterMoveLast As String = ""

    Public strServer As String = ""
    Public strBaza As String = ""
    Public strUser As String = ""
    Public strAppUser As String = ""
    Public strAppPass As String = ""
    Public strPassword As String = ""
    Public DetIDIzKlase As String = ""
    Public strWebService As String = ""
    Public OperateriID As Long = 0
    Public CB(400, 1) As String
    Public PrviPrazniSlot As Integer
    Public RadimoPrekoHTTPservera As Boolean
    Public RadimoSaAccessBazom As Boolean
    Public arrMultiSelect() As String
    ' Public Shared Form3 As New Form3
    Public Form2 As New Form2
    Public Frm6 As New Form6
    'Public Shared Form1 As Form1
    Public WithEvents Form5 As New Form5
    Public CBLog(40) As String
    Public strIncludeGlobal As String = ""
    Public handle5 As IntPtr = 0

    Public Sub PokaziCBLog(Optional ByVal ss As String = "")
        On Error Resume Next
        Dim frm55 As New Form5

        frm55.cls1 = Me
        frm55.TextBox1.Text = ""

        For i = 39 To 0 Step -1
            frm55.TextBox1.Text = CBLog(i) & vbCrLf & vbCrLf & frm55.TextBox1.Text
        Next

        frm55.TextBox2.Text = ss

        frm55.ShowDialog()

        frm55.Close()
        frm55 = Nothing
    End Sub

    Public Sub NapuniCBLog(ByVal sql As String)
        On Error Resume Next
        Dim i As Long = 0
        For i = 39 To 0 Step -1
            CBLog(i + 1) = CBLog(i)
        Next
        CBLog(0) = TimeOfDay & " : " & sql
    End Sub

    Public Function ExecSql(ByVal Sql As String, Optional ByVal strTableName As String = "qwer") As DataTable

        Sql = Sql.Replace("~~", ",")



        Dim da As New SqlClient.SqlDataAdapter(Sql, Conn)
        Dim da3 As New SqlClient.SqlDataAdapter(Sql, Conn3)

        Dim oc As New OleDb.OleDbCommand(Sql, Conn2)
        Dim oc2 As New OleDb.OleDbDataAdapter(Sql, Conn2)

        Dim ds As DataSet = New DataSet

        da.SelectCommand.CommandTimeout = strCommandTimeout

        'NapuniCBLog(" CommandTimeout :   " & da.SelectCommand.CommandTimeout)



        If Sql.Trim.StartsWith("@") Then
            NapuniCBLog(" SKIPPED :   " & Sql)
            Return Nothing
            Exit Function
        End If

        GreskaKodIzvrsavanjaSQLa = ""

        If Sql = "" Or Sql.IndexOf("@@") > 0 Then
            'frm5.TextBox1.Text = frm5.TextBox1.Text & vbCrLf & TimeOfDay & " SKIPPED :   " & Sql & vbCrLf & vbCrLf
            NapuniCBLog(" SKIPPED :   " & Sql)
            GreskaKodIzvrsavanjaSQLa = "SKIPPED"
            Return Nothing
            Exit Function
        End If

        If frm1 Is Nothing Then frm1 = New Form1

        frm1.ToolStripProgressBar1.ForeColor = Color.Green
        UpaliProgres()


        Try



            If DefaultConnection = 2 Then
                NapuniCBLog("CUSTOMCONNECTION :  " & Sql)
                da3.Fill(ds, strTableName)
            Else
                If Sql.Contains("NetGenDBClipboard") Then

                Else
                    NapuniCBLog(Sql)
                End If

                da.Fill(ds, strTableName)
            End If



            'da.Fill(ds)
            'frm5.TextBox1.Text = TimeOfDay & " " & Sql & vbCrLf & frm5.TextBox1.Text


            UgasiProgres()
            If ds.Tables.Count > 0 Then
                Return ds.Tables(0)
            Else
                Return Nothing
            End If

        Catch ex As Exception
            If InStr(ex.Message.ToLower, "ssoperateri") Then

            ElseIf InStr(ex.Message.ToLower, "totalgenlang ") Then

            ElseIf InStr(ex.Message.ToLower, "sspermission") Then

            ElseIf InStr(ex.Message.ToLower, "netgendbfilter") Then

            ElseIf InStr(ex.Message.ToLower, "netgendbclipboard") Then

            Else

                If Err.Number = 5 Or ex.Message.Contains("#") Or ex.Message.Contains("+") Then
                    If ex.Message.Contains("#") Or ex.Message.Contains("+") Then
                        GreskaKodIzvrsavanjaSQLa = ex.Message

                        'MsgBox(vbCr & Translate(ex.Message.Replace("#OK", "").Replace("#", "")) & vbCr & vbCr, MsgBoxStyle.Information, Translate("Poruka"))
                        Dim ff As String = ""

                        If ex.Message.Contains("#") Then
                            ff = ex.Message.Substring(0, InStr(ex.Message, "#") - 2)
                        End If

                        If ex.Message.Contains("+") Then
                            ff = ex.Message.Substring(0, InStr(ex.Message, "+") - 2)
                        End If

                        MsgBox(vbCr & Translate(ff) & vbCr & vbCr, MsgBoxStyle.Information, Translate("Poruka"))

                        If ex.Message.Contains("#OK") Or ex.Message.Contains("+OK") Then
                            GreskaKodIzvrsavanjaSQLa = ""
                        End If

                        ZelimIzaciIzForme = False
                        If ex.Message.ToLower.Contains("#close") Or ex.Message.ToLower.Contains("+close") Then
                            ZelimIzaciIzForme = True
                        End If


                        If ex.Message.ToUpper.Contains("#FOCUS(") Then
                            strAktivnaKontrolanakonLodanja = ex.Message.Substring(InStr(ex.Message.ToUpper, "#FOCUS("), InStr(ex.Message, ")") - InStr(ex.Message.ToUpper, "#FOCUS("))
                            strAktivnaKontrolanakonLodanja = strAktivnaKontrolanakonLodanja.ToLower.Replace("focus(", "").Replace(")", "")
                        End If

                        If ex.Message.ToUpper.Contains("+FOCUS(") Then
                            strAktivnaKontrolanakonLodanja = ex.Message.Substring(InStr(ex.Message.ToUpper, "+FOCUS("), InStr(ex.Message, ")") - InStr(ex.Message.ToUpper, "+FOCUS("))
                            strAktivnaKontrolanakonLodanja = strAktivnaKontrolanakonLodanja.ToLower.Replace("focus(", "").Replace(")", "")
                        End If

                        '--------------------------- KK dodao dodao 19.5.2021. zbog Vlatke i KUP-a
                        '--------------------------- ovo ustvari uopće nije loše , tako je i trebalo biti prije
                        If ds.Tables.Count > 0 Then
                            Return ds.Tables(0)
                        Else
                            Return Nothing
                        End If
                        '--------------------------------------------------------------------------------------

                    Else
                        GreskaKodIzvrsavanjaSQLa = ex.Message
                        MsgBox(vbCr & Translate(ex.Message.ToString.Replace("The transaction ended in the trigger. The batch has been aborted.", "")) & vbCr & vbCr, MsgBoxStyle.Information, Translate("Poruka"))
                    End If
                Else
                    GreskaKodIzvrsavanjaSQLa = ex.Message
                    MsgBox(vbCr & Translate(ex.Message.ToString.Replace("The transaction ended in the trigger. The batch has been aborted.", "")) & vbCr & vbCr & Sql, MsgBoxStyle.Critical, Translate("Poruka"))
                End If



                NapuniCBLog(Sql)
                NapuniCBLog(ex.Message)

                ExecSql2("dbo.spNetGenErrorLog @errorNumber=" & Err.Number & ", @errorDescription='" & ex.Message.Replace("'", "''") & "',@ngLog='" & Sql.Replace("'", "''") & "'")

                ' 08.09.2026. KK - dodatno biljezenje u JupiterLogs (Component = NetGen).
                ' Tiho ne radi nista ako JupiterLogger.dll nije dostupan.
                JupiterLog.Greska("ExecSql", ex.Message & " | SQL: " & Sql, ex)



            End If

            ' 27.12.2021. KK dodao zatvaranje/otvaranje u slučaju greške
            Conn.Close()
            Conn.Open()

            'frm5.TextBox1.Text = vbCrLf & TimeOfDay & " " & Sql & vbCrLf & ex.Message & vbCrLf & frm5.TextBox1.Text

            UgasiProgres()
            Return Nothing
        End Try

    End Function

    Function ExecSql2(ByVal Sql As String, Optional ByVal strTableName As String = "qwer") As DataTable

        Dim da As New SqlClient.SqlDataAdapter(Sql, Conn)
        Dim ds As DataSet = New DataSet
        da.SelectCommand.CommandTimeout = 100
        Try
            da.Fill(ds, strTableName)
            If ds.Tables.Count > 0 Then
                Return ds.Tables(0)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            'MsgBox(ex.Message)
            Return Nothing
        End Try
    End Function


    Public Sub FormatirajGrid(ByRef DataGridView1 As DataGridView)
        Dim i As Integer
        For i = 1 To DataGridView1.ColumnCount - 1
            If DataGridView1.Columns(i).Name.Trim.ToLower.EndsWith("id") And DataGridView1.Columns(i).Name.Trim.ToLower <> "id" Then DataGridView1.Columns(i).Visible = False
            If IsNumeric(DataGridView1.Columns(i).Name.Trim.Substring(DataGridView1.Columns(i).Name.Trim.Length - 4, 4)) Then
                DataGridView1.Columns(i).Width = DataGridView1.Columns(i).Name.Trim.Substring(DataGridView1.Columns(i).Name.Trim.Length - 4, 4) / 14
                DataGridView1.Columns(i).Name = DataGridView1.Columns(i).Name.Trim.Substring(0, DataGridView1.Columns(i).Name.Trim.Length - 4)
            End If
        Next

    End Sub


    Private Sub IspisiMe7(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'MsgBox(sender.tag)
        'Frm6.PrintDocument(strGlobalSQLSERVERlogin, Module1.arrI7, sender.tag.ToString.Replace(":", "/"), Module1.prevParametri7.ToString, 0)
        IspisiMePrekoSpinPrintinga(sender.tag.ToString.Replace(":", "/"), Module1.prevParametri7.ToString)
    End Sub


    Function IspisiMePrekoSpinPrintinga(report As String, param As String, Optional ByVal Vhandle As IntPtr = Nothing) As Boolean

        Dim a As Assembly ' 19.03.2024. kk
        Dim o As Object

        Try
            a = Assembly.Load("Spin.Printing.v23")
            o = a.CreateInstance("Spin.Printing.Start")
            NapuniCBLog("Load Spin.Printing.v23 ")

        Catch ex As Exception
            a = Assembly.Load("Spin.Printing.v9.1")
            o = a.CreateInstance("Spin.Printing.Start")
            NapuniCBLog("Load Spin.Printing.v9.1 ")
        End Try

        

        Dim paramList As New List(Of String)
        '   paramList.AddRange(Module1.arrI7)


        ' ovako je bilo do 18.4.2024.
        'For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
        '    paramList.Add("@id=" & Module1.arrI7(i).ToString & "," & param)
        'Next

        ' ovako nekaon 18.4.2024.
        ' situacija kada netko želi overrajdat @id
        If param.ToLower.Contains("@id=") Or param.ToLower.Contains("@id =") Then
            For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
                paramList.Add(param)
            Next
        Else
            For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
                paramList.Add("@id=" & Module1.arrI7(i).ToString & "," & param)
            Next
        End If

        report = report.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "")


        report = report.Replace("/", "")


        'Try
        '    If UBound(Module1.arrI7) > 1 Then
        '        o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList, "")
        '    Else
        '        o.Print(strGlobalSQLSERVERlogin, report, paramList(0), "")
        '    End If
        '    IspisiMePrekoSpinPrintinga = True
        'Catch ex As Exception
        '    IspisiMePrekoSpinPrintinga = False
        '    NapuniCBLog(" Ispis preko Spin Printinga Error A : " & ex.Message)
        'End Try

        Try
            If UBound(Module1.arrI7) > 1 Then
                o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList, "", Vhandle)
            Else
                o.Print(strGlobalSQLSERVERlogin, report, paramList(0), "", Vhandle)
            End If
            IspisiMePrekoSpinPrintinga = True
        Catch ex As Exception
            IspisiMePrekoSpinPrintinga = False
            NapuniCBLog(" Ispis preko Spin Printinga Error A : " & ex.Message)
        End Try


        ' ajmo probati još jednom , vjerojatno je problem u petom parametru handle5, 27.03.2025 kk
        If IspisiMePrekoSpinPrintinga = False Then
            Try
                If UBound(Module1.arrI7) > 1 Then
                    o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList)
                Else
                    o.Print(strGlobalSQLSERVERlogin, report, paramList(0))
                End If
                IspisiMePrekoSpinPrintinga = True
            Catch ex As Exception
                IspisiMePrekoSpinPrintinga = False
                NapuniCBLog(" Ispis preko Spin Printinga Error B : " & ex.Message)
            End Try
        End If


    End Function

    Public Sub PrintRpt(ByVal ImeForme As String, Optional ByVal prevParametri As String = "", Optional ByVal Vhandle As IntPtr = Nothing)
        'Form4.PrintRpt()

        Dim cPrint As Object
        Dim arrI(0) As Integer

        On Error Resume Next
        Err.Clear()
        ImeForme = Trim(ImeForme)
        If ImeForme.ToLower.Contains("repx:") Or ImeForme.ToLower.Contains("repx/") Then

        Else

        End If

        If Err.Number <> 0 Then
            MsgBox(Err.Description.ToString.Trim + "   " + ImeForme.ToString.Trim)
            Exit Sub
        End If

        'Sql =  SrediSQLPosljeNextForme(Form2.SrediSQL(Sql).Trim)

        ' cPrint.PrintDocument2(arr, ImeForme)
        Err.Clear()


        If arrMultiSelect Is Nothing Then
            Array.Resize(arrMultiSelect, UBound(arr))
            arrMultiSelect(0) = arr(0)
        End If

        If ImeForme.ToLower.Contains(".clsreport") Then



            If InStr(prevParametri, "@") Then ' 25.01.2010. ovo mi nije jasno cemu sluzi ?!!?!?!?!??!?!?
                If prevParametri.LastIndexOf("@") + 1 = InStr(prevParametri, "@") Then
                    prevParametri = ""
                End If
            End If




            ' 12.05.2009. gogo trazio da u 22 metodu dodam i prevparametre
            cPrint = CreateObject("PrintDlg.clsPrintDlg")
            cPrint.ConnectionString = "Driver={SQL Server};" + Conn.ConnectionString
            If UBound(arrMultiSelect) > 0 Then
                NapuniCBLog(" Zovem  PrintDocument22 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                Err.Clear()

                cPrint.PrintDocument22(arrMultiSelect, ImeForme, Conn.ConnectionString, prevParametri.ToString)
                If Err.Number <> 0 Then
                    NapuniCBLog(" Greška ko poziva  PrintDocument22 : " & Err.Description)
                End If
            Else
                NapuniCBLog(" Zovem  PrintDocument22 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                Err.Clear()
                cPrint.PrintDocument22(arr, ImeForme, Conn.ConnectionString, prevParametri.ToString)
                If Err.Number <> 0 Then
                    NapuniCBLog(" Greška ko poziva  PrintDocument22 : " & Err.Description)
                End If
            End If

        ElseIf ImeForme.ToLower.Contains("vrstadokid-") Then

            NapuniCBLog("VALENTINO TEST vrstadokid- " & ImeForme)
            Dim a As Assembly ' 19.03.2024. kk
            Dim o As Object

            a = Assembly.Load("Spin.Printing.v23")
            o = a.CreateInstance("Spin.Printing.Start")
            If Err.Number = 0 Then
                If o.SpinPrintingDizePopUp Then

                End If
                If Err.Number = 0 Then
                    Array.Resize(arrI, UBound(arr) + 1)
                    For i = 0 To UBound(arr)
                        arrI(i) = CInt(arr(i))
                    Next
                    Module1.arrI7 = arrI
                    Module1.prevParametri7 = prevParametri.ToString
                    IspisiMePrekoSpinPrintinga(ImeForme, prevParametri.ToString, Vhandle)
                    Exit Sub
                End If
            End If

            ' ako ovo gore nije prošlo idemo po starom ovo dole ....

            NapuniCBLog("XXXX vrstadokid- " & ImeForme)

            '################################################################### KK 6.6.2022. pokušaj izbjegavanja printdialoga zbog ANYCPU priče
            ImeForme = ImeForme.ToLower.Replace("vrstadokid-", "")
            Dim ID7 As String = ""
            If UBound(arrMultiSelect) > 0 Then
                ID7 = arrMultiSelect(0)
            Else
                ID7 = arr(0)
            End If
            Dim SQL7 As String = "select coalesce(template,'') as Template from vrstadok where vrstadokid = (select vrstadokid from " & ImeForme & " where identitycol = " & ID7 & ")"
            NapuniCBLog("XXXX SQL7 = " & SQL7)
            Dim Template As String
            Dim trs7 As DataTable = ExecSql2(SQL7)
            If Err.Number <> 0 Then
                NapuniCBLog(" Greška ko poziva  sql : " & Err.Description)
            End If
            Template = trs7(0)(0)
            ' Template = "#radninalog"
            Array.Resize(arrI, UBound(arr) + 1)
            For i = 0 To UBound(arr)
                arrI(i) = CInt(arr(i))
            Next
            Module1.arrI7 = arrI
            Module1.prevParametri7 = prevParametri.ToString
            If Template.Substring(0, 1) = "#" Then ' dižem popup
                Dim SQL8 As String = "select * from templates where oznaka = '" & Template & "' and opis != '-'"
                NapuniCBLog("XXXX dižem popup = " & SQL8)
                Dim trs8 As DataTable = ExecSql2(SQL8)
                Dim ContextMenuStrip7 As New ContextMenuStrip
                ContextMenuStrip7.Items.Clear()
                Dim r As DataRow
                For Each r In trs8.Rows
                    Dim mnuItem As ToolStripMenuItem = ContextMenuStrip7.Items.Add(r("opis"), Nothing, AddressOf IspisiMe7)
                    mnuItem.Tag = r("template")
                Next

                ' ==============   kk 02.07.2025. idem na formu umjesto popupa
                Dim frm9 As Form9
                frm9 = New Form9
                frm9.handle5 = handle5
                CreateGridColumns(trs8, frm9.GridEX1)
                AddConditionalFormatting(frm9.GridEX1)
                frm9.GridEX1.SetDataBinding(trs8, "qwer")
                frm9.GridEX1.RootTable.Columns(1).Visible = False
                frm9.GridEX1.RootTable.Columns(2).Visible = False
                frm9.GridEX1.RootTable.Columns(3).Visible = False
                frm9.GridEX1.RootTable.Columns(4).Visible = True
                frm9.GridEX1.RootTable.Columns(4).Width = 400
                frm9.GridEX1.RootTable.Columns(5).Visible = False
                frm9.GridEX1.RootTable.Columns(6).Visible = False
                frm9.Template = ""
                'frm9.ShowDialog()
                'MessageBox.Show(frm9.Template)
                If frm9.Template <> "" Then
                    ' IspisiMePrekoSpinPrintinga(frm9.Template.ToString.Replace(":", "/"), Module1.prevParametri7.ToString)
                End If

                ' ============== 

                ContextMenuStrip7.Show(Sender7, Sender7.PointToClient(Windows.Forms.Cursor.Position))
                If Err.Number <> 0 Then
                    NapuniCBLog("XXXX ContextMenuStrip7.Show: " & Err.Description)
                End If
            Else
                NapuniCBLog("XXXX zovem direktno Template = " & Template.ToString.Replace(":", "/"))
                ' zovem direktno ispis
                'Frm6.PrintDocument(strGlobalSQLSERVERlogin, arrI, Template.ToString.Replace(":", "/"), prevParametri.ToString, 0)
                IspisiMePrekoSpinPrintinga(Template.ToString.Replace(":", "/"), prevParametri.ToString, Vhandle)

            End If
            '################################################################################################################

            ' ovo je bilo prije  6.6.2022
            'cPrint = CreateObject("PrintDlg.clsPrintDlg")
            'If Err.Number <> 0 Then
            '    MsgBox("Greška kod kreiranja CreateObject(PrintDlg.clsPrintDlg) " & vbCr & vbCr & Err.Description)
            'End If

            'cPrint.ConnectionString = "Driver={SQL Server};" + Conn.ConnectionString
            'ImeForme = ImeForme.ToLower.Replace("vrstadokid-", "")
            ''ImeForme = ImeForme.ToLower.Replace(")", "")
            'If UBound(arrMultiSelect) > 0 Then
            '    cPrint.PrintDocumentNG(arrMultiSelect, ImeForme)
            'Else
            '    cPrint.PrintDocumentNG(arr, ImeForme)
            'End If



        ElseIf ImeForme.ToLower.Contains("pr10:") Then ''''''''''''''''''''''''''''''''' gogo new print dlg
            ImeForme = ImeForme.ToLower.Replace("pr10:", "")


            Err.Clear()
            NapuniCBLog("Zovem  Assembly.Load(""PrintDlg2010"") ")
            Dim a As Assembly = Assembly.Load("PrintDlg2010")
            If Err.Number <> 0 Then NapuniCBLog(" Greška : " & Err.Description)

            Err.Clear()
            NapuniCBLog("Kreiram  a.CreateInstance(""PrintDlg2010.frmPrintDlg"") ")
            Dim o As Object = a.CreateInstance("PrintDlg2010.frmPrintDlg")
            If Err.Number <> 0 Then NapuniCBLog(" Greška : " & Err.Description)

            If UBound(arrMultiSelect) > 0 Then
                NapuniCBLog("Zovem  o.PrintDocumentPrazan " & Conn.ConnectionString & "   multiselectarr  " & ImeForme)
                o.PrintDocumentPrazan(Conn.ConnectionString, arrMultiSelect, ImeForme, "")
            Else
                NapuniCBLog("Zovem  o.PrintDocumentPrazan " & Conn.ConnectionString & "   arr  " & ImeForme)
                o.PrintDocumentPrazan(Conn.ConnectionString, arr, "rptPonuda", "")
            End If
        Else
            'cPrint.PrintDocument33(arr(0).ToString, ImeForme.ToString, prevParametri.ToString)
            'cPrint.PrintDocument22(arr(0).ToString, ImeForme.ToString, Conn.ConnectionString)

            If InStr(prevParametri, "@") Then ' 25.01.2010. ovo mi nije jasno cemu sluzi ?!!?!?!?!??!?!?
                If prevParametri.LastIndexOf("@") + 1 = InStr(prevParametri, "@") Then
                    If Not InStr(prevParametri, "=") > 1 Then
                        prevParametri = "" ' 27.01.2010. zakomentirao
                    End If

                End If
            End If


            If ImeForme.ToLower.Contains("repx:") Or ImeForme.ToLower.Contains("repx/") Then
                'cPrint.PrintDocument22(arr(0).ToString, ImeForme.ToString, Conn.ConnectionString)
                'cPrint.PrintDocument33("0", ImeForme.ToString, prevParametri.ToString)

                Array.Resize(arrI, UBound(arr) + 1)
                For i = 0 To UBound(arr)
                    arrI(i) = CInt(arr(i))
                Next
                Module1.arrI7 = arrI

                NapuniCBLog(" WWWW XXXXX Zovem repx i šalejm Vhandle : ")

                If IspisiMePrekoSpinPrintinga(ImeForme.Replace(":", "/"), prevParametri.ToString, Vhandle) = False Then

                    '########################################   ' ovo je bilo prije  6.6.2022
                    If UBound(arrMultiSelect) > 0 Then
                        'NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                        ImeForme = ImeForme.Replace(":", "/")
                        NapuniCBLog(" Zovem  ugrađeni printdialog za REPX multiselect : " & ImeForme & "   " & prevParametri.ToString)
                        Err.Clear()
                        'Frm6.PrintDocument(Conn.ConnectionString, arrI, ImeForme.ToString, prevParametri.ToString, 0)
                        Frm6.PrintDocument(strGlobalSQLSERVERlogin, arrI, ImeForme.ToString, prevParametri.ToString, 0)
                        'cPrint.PrintDocument33(arrMultiSelect, ImeForme.ToString, prevParametri.ToString)
                        If Err.Number <> 0 Then
                            NapuniCBLog(" Greška ko poziva  ugrađenog printdialoga za REPX multiselect  : " & Err.Description)
                        End If
                    Else
                        'NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                        'cPrint.PrintDocument33(arr, ImeForme.ToString, prevParametri.ToString)
                        ImeForme = ImeForme.Replace(":", "/")

                        Err.Clear()

                        NapuniCBLog(" Zovem  ugrađeni printdialog za REPX : " & ImeForme & "   " & prevParametri.ToString)

                        NapuniCBLog("strGlobalSQLSERVERlogin : " & strGlobalSQLSERVERlogin)

                        ' Frm6.PrintDocument(Conn.ConnectionString, arrI, ImeForme.ToString, prevParametri.ToString, 0)
                        Frm6.PrintDocument(strGlobalSQLSERVERlogin, arrI, ImeForme.ToString, prevParametri.ToString, 0)

                        If Err.Number <> 0 Then
                            NapuniCBLog(" Greška ko poziva  ugrađenog printdialoga za REPX   : " & Err.Description)
                        End If
                    End If
                    '#######################################################################

                End If


            ElseIf ImeForme.ToLower.Contains("opr1") Then
                Dim a As Assembly = Assembly.Load("rptDevRepx")
                Dim o As Object = a.CreateInstance("rptDevRepx.clsReport")
                Dim settings As New PrinterSettings()
                ' o.DoPrint2(Conn.ConnectionString, 0, Environment.CurrentDirectory & "\SharedDlls\REPX\", Template, 1, settings.PrinterName, "")
            ElseIf ImeForme.ToLower.Contains("rs:/") Then
                cPrint = CreateObject("PrintDlg.clsPrintDlg")
                cPrint.ConnectionString = "Driver={SQL Server};" + Conn.ConnectionString
                If UBound(arrMultiSelect) > 0 Then
                    NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    cPrint.PrintDocument33(arrMultiSelect, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument33 multiselect  : " & Err.Description)
                    End If
                Else
                    NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    cPrint.PrintDocument33(arr, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument33  : " & Err.Description)
                    End If
                End If


            ElseIf ImeForme.ToLower.Contains("gg/") Then
                cPrint = CreateObject("PrintDlg.clsPrintDlg")
                cPrint.ConnectionString = "Driver={SQL Server};" + Conn.ConnectionString
                ImeForme = ImeForme.Replace("gg/", "") + ".clsreport"
                If UBound(arrMultiSelect) > 0 Then
                    NapuniCBLog(" Zovem  PrintDocument3 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    cPrint.PrintDocument3(arrMultiSelect, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument3 multiselect  : " & Err.Description)
                    End If
                Else
                    NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    cPrint.PrintDocument3(arr, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument3  : " & Err.Description)
                    End If
                End If


            Else
                cPrint = CreateObject("PrintDlg.clsPrintDlg")
                cPrint.ConnectionString = "Driver={SQL Server};" + Conn.ConnectionString
                'cPrint.PrintDocument22(arr(0).ToString, ImeForme.ToString, Conn.ConnectionString)
                If UBound(arrMultiSelect) > 0 Then
                    NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    'cPrint.PrintDocument22(arrMultiSelect, ImeForme.ToString, Conn.ConnectionString)
                    ' 18.12.2008. gogo je trazio da za rsgen zovem 33 , ne vise 22
                    cPrint.PrintDocument33(arrMultiSelect, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument33 : " & Err.Description)
                    End If
                Else
                    NapuniCBLog(" Zovem  PrintDocument33 u printdialogu : " & ImeForme & "   " & prevParametri.ToString)
                    Err.Clear()
                    cPrint.PrintDocument33(arr, ImeForme.ToString, prevParametri.ToString)
                    If Err.Number <> 0 Then
                        NapuniCBLog(" Greška ko poziva  PrintDocument33 : " & Err.Description)
                    End If
                End If
            End If
        End If

        'If Err.Number <> 0 Then
        '    MsgBox(Err.Description.ToString.Trim + vbCrLf + ImeForme.ToString.Trim + vbCrLf + prevParametri.ToString.Trim)
        '    Exit Sub
        'End If


        cPrint = Nothing

    End Sub

    Public Sub UpaliProgres()
        frm1.ToolStripProgressBar1.Visible = True
        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        frm1.ToolStripProgressBar1.Value = 20
        frm1.ToolStripProgressBar1.Style = ProgressBarStyle.Marquee
        frm1.ToolStripProgressBar1.MarqueeAnimationSpeed = 40
    End Sub

    Public Sub UgasiProgres()
        frm1.ToolStripProgressBar1.Visible = False

        frm1.ToolStripProgressBar1.Value = 0
        frm1.ToolStripProgressBar1.Style = ProgressBarStyle.Blocks
        frm1.ToolStripProgressBar1.MarqueeAnimationSpeed = 0
        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub

    Public Function SrediSQLPosljeNextForme(ByVal sql As String, Optional ByVal prevParametri As String = "", Optional ByVal DatumjezaIspis As Boolean = False, Optional ByVal IdiUclipbord As Boolean = True) As String

        'Form2.cls1 = Me

        If prevParametri <> "" Then
            Form2.strPrevParametri = prevParametri
            strSviParametriuModulu = prevParametri
        Else
            ' Form2.strPrevParametri = frm1.ParametriSaGrida
            ' strSviParametriuModulu = frm1.ParametriSaGrida
        End If


        If Form2.strPrevParametri Is Nothing Then Form2.strPrevParametri = ""

        sql = Form2.SrediSQL2(sql).Trim
        If Form2.strPrevParametri.Trim = "" And InStr(sql, "@@") = 0 Then
            SrediSQLPosljeNextForme = sql
            Exit Function
        End If

        Dim Ar() As String
        Dim Sql2 As String
        Dim Kraj As Long
        Ar = sql.Split(" ")
        While InStr(sql, "@@")
            Sql2 = sql.Substring(sql.IndexOf("@@"), sql.Length - sql.IndexOf("@@")) & " "
            Kraj = Sql2.IndexOf(" ")
            If Sql2.IndexOf("'") > -1 Then
                If Sql2.IndexOf("'") < Kraj Then
                    Kraj = Sql2.IndexOf("'")
                End If
            End If

            If Sql2.IndexOf("%") > -1 Then
                If Sql2.IndexOf("%") < Kraj Then
                    Kraj = Sql2.IndexOf("%")
                End If
            End If

            If Sql2.IndexOf("#") > -1 Then
                If Sql2.IndexOf("#") < Kraj Then
                    Kraj = Sql2.IndexOf("#")
                End If
            End If


            If Sql2.IndexOf(",") > -1 Then
                If Sql2.IndexOf(",") < Kraj Then
                    Kraj = Sql2.IndexOf(",")
                End If
            End If

            If Sql2.IndexOf(")") > -1 Then
                If Sql2.IndexOf(")") < Kraj Then
                    Kraj = Sql2.IndexOf(")")
                End If
            End If

            If Sql2.IndexOf("*") > -1 Then
                If Sql2.IndexOf("*") < Kraj Then
                    Kraj = Sql2.IndexOf("*")
                End If
            End If


            If Sql2.IndexOf("/") > -1 Then
                If Sql2.IndexOf("/") < Kraj Then
                    Kraj = Sql2.IndexOf("/")
                End If
            End If

            If Sql2.IndexOf("\") > -1 Then
                If Sql2.IndexOf("\") < Kraj Then
                    Kraj = Sql2.IndexOf("\")
                End If
            End If

            If Sql2.IndexOf("+") > -1 Then
                If Sql2.IndexOf("+") < Kraj Then
                    Kraj = Sql2.IndexOf("+")
                End If
            End If

            If Sql2.IndexOf("-") > -1 Then
                If Sql2.IndexOf("-") < Kraj Then
                    Kraj = Sql2.IndexOf("-")
                End If
            End If

            If Sql2.IndexOf(".") > -1 Then
                If Sql2.IndexOf(".") < Kraj Then
                    Kraj = Sql2.IndexOf(".")
                End If
            End If

            If Sql2.IndexOf("(") > -1 Then
                If Sql2.IndexOf("(") < Kraj Then
                    Kraj = Sql2.IndexOf("(")
                End If
            End If

            If Sql2.IndexOf("=") > -1 Then
                If Sql2.IndexOf("=") < Kraj Then
                    Kraj = Sql2.IndexOf("=")
                End If
            End If

            Sql2 = Sql2.Substring(0, Kraj)
            sql = sql.Replace(Sql2, NadjiVrijednostuPrevParametrima(Sql2, DatumjezaIspis, IdiUclipbord))
            sql = sql.Replace("¸¸", "''")
        End While
        SrediSQLPosljeNextForme = sql
    End Function


    Public Function NadjiVrijednostuPrevParametrima(ByVal Param As String, Optional ByVal DatumjezaIspis As Boolean = False, Optional ByVal IdiUclipbord As Boolean = True) As String
        Dim Ar2() As String
        Dim i As Long
        NadjiVrijednostuPrevParametrima = ""
        Param = Trim(Replace(Param.Trim, "@@", "@"))
        'Ar2 = Split(Form2.strPrevParametri.Trim, ",")
        Ar2 = Split(strSviParametriuModulu.Trim, ",")
        For i = 0 To UBound(Ar2)
            On Error Resume Next
            If Ar2(i).Trim.Substring(0, Param.Length).ToUpper = Param.ToUpper Then
                If Err.Number = 0 Then

                    NadjiVrijednostuPrevParametrima = Ar2(i).Substring(Ar2(i).IndexOf("=") + 1, Ar2(i).Length - Ar2(i).IndexOf("=") - 1)

                    If Param.ToLower.Contains("ajdijeva") Then

                        NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace("~", ",")

                    End If


                    If Param.Substring(0, 4).ToUpper.StartsWith("@DAT") Then
                        If NadjiVrijednostuPrevParametrima.Contains(".") Then
                            If Not DatumjezaIspis Then
                                NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Substring(7, 4) & NadjiVrijednostuPrevParametrima.Substring(4, 2) & NadjiVrijednostuPrevParametrima.Substring(1, 2)
                            End If
                        End If
                    End If

                    Exit For
                End If
            End If
        Next
        If Param.Substring(0, 4).ToUpper = "@DAT" Or (Param.ToUpper.Contains("GRID") And Param.ToUpper.Contains("DAT")) Then
            ' v 1.4.5. dodao 19.12.2008. zbog pogrešnog formata datuma kod ispisa
            If DatumjezaIspis Then
                If NadjiVrijednostuPrevParametrima.Contains(".") = False Then
                    NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Substring(7, 2) & "." & NadjiVrijednostuPrevParametrima.Substring(5, 2) & "." & NadjiVrijednostuPrevParametrima.Substring(1, 4)
                Else
                    NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace(" 0:00:00", "")
                End If
                NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace("'", "")
            Else
                NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace("'", "")
                If NadjiVrijednostuPrevParametrima.Trim <> "" Then
                    If NadjiVrijednostuPrevParametrima = "null" Then

                    Else
                        NadjiVrijednostuPrevParametrima = "'" & NadjiVrijednostuPrevParametrima & "'"
                    End If

                End If
            End If
        Else
            NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace("'", "")
            Print(NadjiVrijednostuPrevParametrima)
        End If


        ' testiram
        If NadjiVrijednostuPrevParametrima.Trim = "" Then NadjiVrijednostuPrevParametrima = "null"


        If IdiUclipbord = False And Param.ToUpper.Contains("@GRID") Then
            IdiUclipbord = False
        Else
            IdiUclipbord = True
        End If

        IdiUclipbord = True

        If Param.Trim.ToLower = "@napomena" And NadjiVrijednostuPrevParametrima.Trim.ToLower = "null" Then
            NadjiVrijednostuPrevParametrima = ""
            IdiUclipbord = False
        End If

        If IdiUclipbord Then
            ' "TGClipBoard Story" ... 31.10.2007. --------------------------------------
            ' .... probaj naci u clipboardu ... ------------------------------------
            Dim j As Integer
            If NadjiVrijednostuPrevParametrima = "null" Or NadjiVrijednostuPrevParametrima = "" Then
                For j = 0 To UBound(CB)
                    If "@" + UCase(CB(j, 0)) = IIf(UCase(CB(j, 0)).StartsWith("@"), "@", "") + UCase(Param) + "=" Then

                        If Param.Trim.ToString.ToUpper.StartsWith("@DAT") Or (Param.ToUpper.Contains("GRID") And Param.ToUpper.Contains("DAT")) Then
                            '--- 1.12.2008 problem ako u CB uleti neformatirani datum ...
                            If CB(j, 1).Contains(".") Then
                                If DatumjezaIspis Then
                                    NadjiVrijednostuPrevParametrima = "" & CB(j, 1) & ""
                                    NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Replace(" 0:00:00", "")
                                Else
                                    NadjiVrijednostuPrevParametrima = "'" & CB(j, 1).Substring(6, 4) & CB(j, 1).Substring(3, 2) & CB(j, 1).Substring(0, 2) & "'"
                                End If
                            Else
                                If DatumjezaIspis Then
                                    NadjiVrijednostuPrevParametrima = "" & CB(j, 1) & ""  ' odkomentirao 27.02.2009. , a ne znam zašto je uopce bilo zakomentirano
                                    NadjiVrijednostuPrevParametrima = NadjiVrijednostuPrevParametrima.Substring(6, 2) & "." & NadjiVrijednostuPrevParametrima.Substring(4, 2) & "." & NadjiVrijednostuPrevParametrima.Substring(0, 4)
                                Else
                                    If CB(j, 1) = "null" Then
                                        NadjiVrijednostuPrevParametrima = "" & CB(j, 1) & ""
                                    Else
                                        NadjiVrijednostuPrevParametrima = "'" & CB(j, 1) & "'"
                                    End If

                                End If
                            End If
                        Else
                            If IsNumeric(CB(j, 1)) Then  '--v 1.6.9. ... , kod preuzimanja numeric vrijednosti iz CB zarez se replace sa točkom 
                                NadjiVrijednostuPrevParametrima = CB(j, 1).Replace(",", ".")
                            Else
                                NadjiVrijednostuPrevParametrima = CB(j, 1)
                            End If
                        End If

                    End If
                Next
            End If
            '------------------------------------------------------------------------
        End If



    End Function



    Public Sub CreateGridColumns(ByVal trs As DataTable, ByVal GridEX11 As Janus.Windows.GridEX.GridEX, Optional ByVal EditColumns As String = "", Optional ByVal ComboColumns As String = "")

        If trs Is Nothing Then Exit Sub

        If trs.Rows.Count = 0 Then
            ' Exit Sub
        End If

        On Error Resume Next

        Dim Column As DataColumn
        Dim fs As New GridEXFormatStyle
        fs.BackColor = Color.LightYellow

        GridEX11.RootTable.Columns.Clear()

        GridEX11.ImageList = frm1.ImageList2
        '  GridEX11.RootTable.HeaderLines = 3
        GridEX11.EmptyRows = True
        'GridEX11.ColumnAutoResize = True

        GridEX11.EditMode = EditMode.EditOff

        'Create a new GridEXTable
        Dim table As New Janus.Windows.GridEX.GridEXTable()
        'add columns to the table

        'adding an unbound icon column
        Dim iconColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add("Icon", Janus.Windows.GridEX.ColumnType.Image, Janus.Windows.GridEX.EditType.NoEdit)
        'set other properties
        iconColumn.Width = 22
        iconColumn.Caption = ""
        iconColumn.AllowSort = False
        iconColumn.AllowGroup = False
        iconColumn.AllowSize = False
        iconColumn.Selectable = False
        iconColumn.BoundMode = Janus.Windows.GridEX.ColumnBoundMode.UnboundFetch
        iconColumn.ImageKey = "OrdersDetails16.ico"
        iconColumn.Visible = False

        Dim CheckColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add("Selector", ColumnType.CheckBox, EditType.CheckBox)
        'set other properties
        CheckColumn.Width = 22
        CheckColumn.Caption = ""
        CheckColumn.AllowSort = True
        CheckColumn.ActAsSelector = True


        Dim tCaptionPrijePrevoda As String = ""

        On Error Resume Next


        strKoloneZaGrupiranje = ""
        Dim brojKolone4 As Integer = -1

        For Each Column In trs.Columns
            brojKolone4 = brojKolone4 + 1
            If InStr(Column.DataType.ToString.ToLower, "boolean") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.CheckBox, EditType.CheckBox)
                completeColumn.Width = 34
                completeColumn.AllowSize = True


                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If



                tCaptionPrijePrevoda = completeColumn.Caption.Trim


                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")

                'completeColumn.FormatString = "##0"
                'completeColumn.EditType = EditType.NoEdit
                completeColumn.TextAlignment = TextAlignment.Far
                completeColumn.HeaderAlignment = TextAlignment.Far
                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If

                If EditColumns.ToLower.Contains(tCaptionPrijePrevoda.ToLower) Then
                    completeColumn.EditType = EditType.CheckBox

                    If completeColumn.Width > 29 Then
                        completeColumn.HeaderImageKey = "OrdersDetails16.ico"
                    End If
                    Dim fs2 As New GridEXFormatStyle
                    fs2.BackColor = Color.LightYellow
                    completeColumn.CellStyle = fs2
                Else
                    completeColumn.EditType = EditType.NoEdit
                End If



            End If

            If InStr(Column.DataType.ToString.ToLower, "int") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.Text, EditType.TextBox)
                completeColumn.Width = 34
                completeColumn.AllowSize = True


                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If

                tCaptionPrijePrevoda = completeColumn.Caption.Trim


                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")

                completeColumn.FormatString = "##0"
                'completeColumn.EditType = EditType.NoEdit
                completeColumn.TextAlignment = TextAlignment.Far
                completeColumn.HeaderAlignment = TextAlignment.Far



                If completeColumn.Caption.ToLower.StartsWith("sum!") Then
                    completeColumn.AggregateFunction = AggregateFunction.Sum
                    completeColumn.TotalFormatString = "#,##0"
                    completeColumn.Caption = completeColumn.Caption.Replace("sum!", "").Replace("SUM!", "")
                End If




                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If

                If EditColumns.ToLower.Contains(tCaptionPrijePrevoda.ToLower) Then
                    completeColumn.EditType = EditType.TextBox
                    If completeColumn.Width > 29 Then
                        completeColumn.HeaderImageKey = "OrdersDetails16.ico"
                    End If
                    Dim fs2 As New GridEXFormatStyle
                    fs2.BackColor = Color.LightYellow
                    completeColumn.CellStyle = fs2
                Else
                    completeColumn.EditType = EditType.TextBox
                    completeColumn.EditType = EditType.NoEdit
                End If



            End If

            If InStr(Column.DataType.ToString.ToLower, "decimal") Or InStr(Column.DataType.ToString.ToLower, "double") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.Text, EditType.TextBox)
                completeColumn.Width = 44
                completeColumn.AllowSize = True

                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If

                tCaptionPrijePrevoda = completeColumn.Caption.Trim




                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")

                completeColumn.FormatString = "#,##0.00"
                completeColumn.TextAlignment = TextAlignment.Far
                completeColumn.HeaderAlignment = TextAlignment.Far
                completeColumn.AggregateFunction = AggregateFunction.Sum
                completeColumn.TotalFormatString = "#,##0.00"


                If EditColumns.ToLower.Contains(tCaptionPrijePrevoda.ToLower) Then
                    completeColumn.EditType = EditType.TextBox

                    If completeColumn.Width > 29 Then
                        completeColumn.HeaderImageKey = "OrdersDetails16.ico"
                    End If

                    Dim fs2 As New GridEXFormatStyle
                    fs2.BackColor = Color.LightYellow
                    completeColumn.CellStyle = fs2
                Else

                    completeColumn.EditType = EditType.TextBox
                    completeColumn.EditType = EditType.NoEdit
                End If


                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If




            End If

            If InStr(Column.DataType.ToString.ToLower, "string") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, Janus.Windows.GridEX.ColumnType.Text, Janus.Windows.GridEX.EditType.NoEdit)
                completeColumn.Width = 100
                completeColumn.AllowSize = True

                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If


                tCaptionPrijePrevoda = completeColumn.Caption.Trim

                'completeColumn.EditType = EditType.NoEdit

                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")



                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If

                If EditColumns.ToLower.Contains(tCaptionPrijePrevoda.ToLower) Then
                    completeColumn.EditType = EditType.TextBox
                    If completeColumn.Width > 29 Then
                        completeColumn.HeaderImageKey = "OrdersDetails16.ico"
                    End If
                    Dim fs2 As New GridEXFormatStyle
                    fs2.BackColor = Color.LightYellow
                    completeColumn.CellStyle = fs2
                Else
                    completeColumn.EditType = EditType.TextBox
                    completeColumn.EditType = EditType.NoEdit
                End If




            End If

            If InStr(Column.DataType.ToString.ToLower, "date") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.Text, EditType.CalendarDropDown)
                completeColumn.Width = 100
                completeColumn.AllowSize = True


                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If

                tCaptionPrijePrevoda = completeColumn.Caption.Trim




                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")
                'completeColumn.FormatString = "dd.MM.yyyy hh:mm:ss"
                completeColumn.FormatString = "dd.MM.yyyy"
                'completeColumn.EditType = EditType.NoEdit

                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If

                If EditColumns.ToLower.Contains(tCaptionPrijePrevoda.ToLower) Then
                    completeColumn.EditType = EditType.CalendarDropDown
                    If completeColumn.Width > 29 Then
                        completeColumn.HeaderImageKey = "OrdersDetails16.ico"
                    End If
                    Dim fs2 As New GridEXFormatStyle
                    fs2.BackColor = Color.LightYellow
                    completeColumn.CellStyle = fs2
                Else
                    completeColumn.EditType = EditType.NoEdit
                End If



            End If


            If InStr(Column.Caption.ToString.ToLower, "picture") Then
                Dim completeColumn As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.BoundImage, EditType.NoEdit)
                completeColumn.Width = 100
                completeColumn.AllowSize = True

                If Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) <> 0 Then
                    completeColumn.Width = Val(Column.Caption.Trim.Substring(Column.Caption.Trim.Length - 4, 4)) / FakPretvorbe
                    completeColumn.Caption = Column.Caption.Trim.Substring(0, Column.Caption.Trim.Length - 4)
                End If

                tCaptionPrijePrevoda = completeColumn.Caption.Trim

                If completeColumn.Caption.StartsWith("!") Then
                    strKoloneZaGrupiranje = strKoloneZaGrupiranje & "~" & brojKolone4
                End If

                'completeColumn.Caption = Translate(completeColumn.Caption).Replace(";", vbCrLf).Replace("!", "")
                completeColumn.Caption = completeColumn.Caption.Replace(";", vbCrLf).Replace("!", "")

                If Column.Caption.ToUpper.Substring(Column.Caption.Length - 2, 2) = "ID" Then
                    completeColumn.Visible = False
                End If

            End If


            If InStr(Column.Caption.ToString.ToLower, "combo") Then
                Dim column2 As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.Text, EditType.Combo)
                column2.HasValueList = True
                'Set EditType to Combo or DropDownList.
                'In a MultipleValues Column, the dropdown will appear with a checkbox 
                'at the left of each item to let the user select multiple items
                column2.EditType = EditType.DropDownList
                column2.ValueList.PopulateValueList(ExecSql("select 1 as id, 'aloalo' as naziv union all select 2 , 'bbbb'  ").DefaultView, "id", "naziv")
                column2.CompareTarget = ColumnCompareTarget.Value
                column2.DefaultGroupInterval = GroupInterval.Text
                column2.Caption = Translate(column2.Caption)
            End If

            If InStr(Column.Caption.ToString.ToLower, "combo") Then
                Dim column2 As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add(Column.Caption, ColumnType.Text, EditType.Combo)
                column2.HasValueList = True
                'Set EditType to Combo or DropDownList.
                'In a MultipleValues Column, the dropdown will appear with a checkbox 
                'at the left of each item to let the user select multiple items
                column2.EditType = EditType.DropDownList
                column2.ValueList.PopulateValueList(ExecSql("select 1 as id, 'aloalo' as naziv union all select 2 , 'bbbb'  ").DefaultView, "id", "naziv")
                column2.CompareTarget = ColumnCompareTarget.Value
                column2.DefaultGroupInterval = GroupInterval.Text
                column2.Caption = Translate(column2.Caption)
            End If


        Next Column



        'Dim completeColumn5 As Janus.Windows.GridEX.GridEXColumn = table.Columns.Add("Update", ColumnType.Text, EditType.NoEdit)
        'completeColumn5.Width = 34
        'completeColumn5.AllowSize = False
        'completeColumn5.Caption = "Update"
        'completeColumn5.FormatString = "##0"
        'completeColumn5.EditType = EditType.NoEdit


        'once all columns have been added to the GridEXTable, set the table as the RootTable of the grid
        On Error Resume Next
        GridEX11.RootTable = Nothing
        GridEX11.RootTable = table
        GridEX11.TotalRow = InheritableBoolean.True
        GridEX11.RootTable.HeaderLines = 3

    End Sub


    Public Sub AddConditionalFormatting(ByVal GridEX11 As Janus.Windows.GridEX.GridEX)

        On Error Resume Next

        If GridEX11.RowCount = 0 Then
            'Exit Sub 
        End If

        Dim fc As GridEXFormatCondition
        For j = 0 To GridEX11.Tables.Count

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "bold")
            fc.FormatStyle.FontBold = Janus.Windows.GridEX.TriState.True
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "gray")
            fc.FormatStyle.ForeColor = SystemColors.GrayText
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "red")
            fc.FormatStyle.ForeColor = Color.Red
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "green")
            fc.FormatStyle.ForeColor = Color.Green
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "blue")
            fc.FormatStyle.ForeColor = Color.Blue
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "brown")
            fc.FormatStyle.ForeColor = Color.Brown
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "yellow")
            fc.FormatStyle.ForeColor = Color.Yellow
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "orange")
            fc.FormatStyle.ForeColor = Color.Orange
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "strike")
            fc.FormatStyle.FontStrikeout = TriState.True
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "italic")
            fc.FormatStyle.FontItalic = TriState.True
            GridEX11.Tables(j).FormatConditions.Add(fc)

            fc = New GridEXFormatCondition(GridEX11.Tables(j).Columns("formatid"), ConditionOperator.Contains, "underline")
            fc.FormatStyle.FontUnderline = TriState.True
            GridEX11.Tables(j).FormatConditions.Add(fc)
        Next



    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Sub New()
        Form2.cls1 = Me
    End Sub



    'Public Sub DeleteGridLayoutT(ByVal ImeFajla As String, ByVal GridEX1 As GridEX)
    '    On Error Resume Next
    '    Dim LayoutDir As String
    '    LayoutDir = GetLayoutDirectory() + "\" + "GridTrazilica-" + ImeFajla + ".gxl"
    '    My.Computer.FileSystem.DeleteFile(LayoutDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
    'End Sub


    Dim strWebServiceMP As String = ""
    Dim strWebServiceBaza As String = ""

    Function DajInstall() As Boolean

        On Error Resume Next

        If strWebServiceMP <> "" And strWebServiceBaza <> "" Then
            DajInstall = True
            Exit Function
        End If

        DajInstall = False
        Dim sql As String = ""

        sql = "select * from Install "

        Dim trs As DataTable = ExecSql(sql)
        If Err.Number <> 0 Then
            MsgBox(Err.Description)

            Exit Function
        End If

        If trs Is Nothing Then
            MsgBox(Err.Description)
            Exit Function
        End If

        If trs.Rows.Count = 0 Then
            MsgBox("Nema ništa u Install!")
            Exit Function
        End If

        strWebServiceMP = trs(0)("webservice")
        strWebServiceBaza = trs(0)("baza")
        DajInstall = True
    End Function


    'Sub IdiNaWebServicePoBrojBodova(str As String)
    '    ' ovdje ide sinhron poziv sevrvce-a , 
    '    On Error Resume Next

    '    If DajInstall() = False Then Exit Sub

    '    If Trim(strWebServiceMP) = "" Then
    '        Exit Sub
    '    End If

    '    Dim sifraclana As String = ""
    '    Dim strGuid As String = "2341231234512541234wefwqefqwef"
    '    Dim brojBodova As Decimal = 0

    '    Dim sql As String = ""
    '    sql = "spmpazurBodove @action=2 , @sifraclana='" & str & "' , @brojbodova=" & brojBodova
    '    ExecSql(sql)

    '    Dim request As WebRequest = WebRequest.Create(Trim(strWebServiceMP) & "?action=dajbodove&d=" & strWebServiceBaza & "&s=" & str & "&g=" & strGuid)
    '    request.Method = "POST"
    '    request.Credentials = CredentialCache.DefaultCredentials
    '    request.Timeout = 5000
    '    Dim postData As String
    '    postData = "?action=dajbodove&d=" & strWebServiceBaza & "&s=" & sifraclana & "&g=" & strGuid
    '    postData = ""
    '    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
    '    request.ContentType = "application/x-www-form-urlencoded"
    '    request.ContentLength = byteArray.Length
    '    Dim dataStream As Stream = request.GetRequestStream()
    '    dataStream.Write(byteArray, 0, byteArray.Length)
    '    dataStream.Close()
    '    Dim response As WebResponse = request.GetResponse()
    '    Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
    '    dataStream = response.GetResponseStream()
    '    Dim reader As New StreamReader(dataStream)
    '    Dim responseFromServer As String = reader.ReadToEnd()
    '    If responseFromServer = "0" Then
    '        MsgBox("Login Failed")
    '    Else
    '        'MsgBox("json data")
    '        Dim json As String = responseFromServer
    '        Dim ser As JObject = JObject.Parse(json)
    '        Dim data As List(Of JToken) = ser.Children().ToList
    '        Dim output As String = ""

    '        For Each item As JProperty In data
    '            item.CreateReader()

    '            If item.Name = "brojbodova" Then
    '                brojBodova = item.Value
    '            End If

    '            If item.Name = "sifraclana" Then
    '                sifraclana = item.Value
    '            End If


    '            'Select Case item.Name
    '            '    Case "comments"
    '            '        output += "Comments:" + vbCrLf
    '            '        For Each comment As JObject In item.Values
    '            '            Dim u As String = comment("user")
    '            '            Dim d As String = comment("date")
    '            '            Dim c As String = comment("comment")
    '            '            output += u + vbTab + d + vbTab + c + vbCrLf
    '            '        Next

    '            '    Case "messages"
    '            '        output += "Messages:" + vbCrLf
    '            '        For Each msg As JObject In item.Values
    '            '            Dim f As String = msg("from")
    '            '            Dim t As String = msg("to")
    '            '            Dim d As String = msg("date")
    '            '            Dim m As String = msg("message")
    '            '            Dim s As String = msg("status")
    '            '            output += f + vbTab + t + vbTab + d + vbTab + m + vbTab + s + vbCrLf
    '            '        Next

    '            'End Select
    '        Next
    '    End If
    '    reader.Close()
    '    dataStream.Close()
    '    response.Close()


    '    If sifraclana <> "" Then
    '        sql = "spmpazurBodove @action=1 , @sifraclana='" & sifraclana & "' , @brojbodova=" & brojBodova
    '        ExecSql(sql)
    '    End If



    'End Sub

    'Sub AzurirajBodove(MPRacunBodoviId As String)
    '    Dim sql As String
    '    Dim trs As DataTable
    '    Dim txtClanSifra As String = ""
    '    Dim txtBroj As String = ""
    '    Dim txtGodina As String = ""

    '    If DajInstall() = False Then Exit Sub


    '    sql = "select * from MPRacunBodovi where MPRacunBodoviId=" & MPRacunBodoviId
    '    trs = ExecSql(sql)
    '    txtClanSifra = trs(0)("sifraclana")
    '    txtBroj = trs(0)("brojdok")
    '    txtGodina = trs(0)("godina")

    '    sql = "spmpazurBodove @action = 10 , @brojdok='" & txtBroj & "', @godina=" & txtGodina
    '    trs = ExecSql(sql)
    '    IdiNaWebServiceIAzurirajBodove(txtClanSifra, strWebServiceBaza, trs(0)("prezime"), trs(0)("ime"), trs(0)("adresa"), trs(0)("pocetnibodovi"), trs(0)("spol"), trs(0)("bodoviporacunu"), trs(0)("bodovirealizacija"), trs(0)("brojdok"), trs(0)("godina"), trs(0)("mpracunbodoviid"))

    'End Sub


    Function GlipiMS(str As String)
        str = str.Replace("Ć", "CCC").Replace("ć", "ccc")
        str = str.Replace("Ž", "ZZZ").Replace("ž", "zzz")
        str = str.Replace("Č", "YYY").Replace("č", "yyy")
        str = str.Replace("Š", "SSS").Replace("š", "sss")
        str = str.Replace("Đ", "DDD").Replace("đ", "ddd")
        GlipiMS = str
    End Function


    'Sub IdiNaWebServiceIAzurirajBodove(str As String, strWebServiceBaza As String, prezime As String, ime As String, adresa As String, pocetnibodovi As String, spol As String, bodoviporacunu As String, realiziranibodovi As String, brojdok As String, godina As String, mpracunbodoviid As String)


    '    ' ovdje ide Async poziv sevrvce-a  
    '    On Error Resume Next

    '    If Trim(strWebServiceMP) = "" Then
    '        Exit Sub
    '    End If

    '    Dim sifraclana As String = ""
    '    Dim strGuid As String = "2341231234512541234wefwqefqwef"
    '    Dim strOK As String = ""
    '    ' Dim mpracunBodoviid As String = ""

    '    Dim urlString As String = ""

    '    urlString = Trim(strWebServiceMP) & "?action=azurbodova&d=" & strWebServiceBaza & "&sifraclana=" & str & "&g=" & strGuid & "&prezime=" & HttpUtility.UrlEncode(GlipiMS(prezime)) & "&ime=" & HttpUtility.UrlEncode(GlipiMS(ime)) & "&adresa=" & HttpUtility.UrlEncode(GlipiMS(adresa)) & "&pocetnibodovi=" & HttpUtility.UrlEncode(pocetnibodovi) & "&spol=" & HttpUtility.UrlEncode(GlipiMS(spol)) & "&bodoviporacunu=" & bodoviporacunu & "&realiziranibodovi=" & realiziranibodovi & "&brojdok=" & brojdok & "&godina=" & godina

    '    ' urlString = "http://www.spin.hr/ng/blagajnadxservice/"
    '    ' Dim myUri As New Uri(Uri.EscapeUriString(urlString))
    '    ' urlString = HttpUtility.UrlDecode(urlString)

    '    Dim request As WebRequest = WebRequest.Create(urlString)
    '    request.Credentials = CredentialCache.DefaultCredentials
    '    request.Timeout = 5000
    '    request.Method = "POST"
    '    Dim postData As String
    '    postData = ""
    '    Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
    '    request.ContentType = "application/x-www-form-urlencoded"
    '    request.ContentLength = byteArray.Length
    '    Dim dataStream As Stream = request.GetRequestStream()
    '    dataStream.Write(byteArray, 0, byteArray.Length)
    '    dataStream.Close()
    '    Dim response As WebResponse = request.GetResponse()
    '    Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
    '    dataStream = response.GetResponseStream()
    '    Dim reader As New StreamReader(dataStream)
    '    Dim responseFromServer As String = reader.ReadToEnd()
    '    If responseFromServer = "0" Then
    '        MsgBox("Login Failed")
    '    Else
    '        'MsgBox("json data")
    '        Dim json As String = responseFromServer
    '        Dim ser As JObject = JObject.Parse(json)
    '        Dim data As List(Of JToken) = ser.Children().ToList
    '        Dim output As String = ""

    '        For Each item As JProperty In data
    '            item.CreateReader()

    '            If item.Name = "sifraclana" Then
    '                sifraclana = item.Value
    '            End If

    '            If item.Name = "mpracunbodoviid" Then
    '                ' mpracunBodoviid = item.Value
    '            End If

    '            If item.Name = "ok" Then
    '                strOK = item.Value
    '            End If

    '        Next
    '    End If
    '    reader.Close()
    '    dataStream.Close()
    '    response.Close()

    '    Dim sql As String = ""
    '    If strOK = "1" Then
    '        sql = "delete from MPRacunBodovi where mpracunbodoviid=" & mpracunbodoviid
    '        ExecSql(sql)
    '    End If


    'End Sub


    '-------------------------------------- devexizacija
    Dim WithEvents riEdit As RepositoryItemTextEdit = New RepositoryItemTextEdit
    Dim WithEvents riEditChk As RepositoryItemCheckEdit = New RepositoryItemCheckEdit

    Public Sub NapuniDXGrid(trs As DataTable, gcArtikli As DevExpress.XtraGrid.GridControl, gvArtikli As DevExpress.XtraGrid.Views.Grid.GridView, strEditColumns As String)
        On Error Resume Next


        Dim dcID As DataColumn = New DataColumn("rcb555", GetType(Boolean))
        dcID.DefaultValue = False
        trs.Columns.Add(dcID)
        gcArtikli.DataSource = trs
        gvArtikli.Columns("rcb555").VisibleIndex = 1
        gvArtikli.Columns("rcb555").OptionsColumn.AllowEdit = True
        gvArtikli.Columns("rcb555").Caption = "Odaberi"

        gvArtikli.OptionsView.ColumnAutoWidth = False


        For i = 0 To gvArtikli.Columns.Count - 1

            If gvArtikli.Columns(i).GetCaption().ToLower.EndsWith("id") Then
                gvArtikli.Columns(i).Visible = False
            End If

            If IsNumeric(gvArtikli.Columns(i).GetCaption().Trim.Substring(gvArtikli.Columns(i).GetCaption().Trim.Length - 4, 4)) Then
                gvArtikli.Columns(i).Width = gvArtikli.Columns(i).GetCaption().Trim.Substring(gvArtikli.Columns(i).GetCaption().Trim.Length - 4, 4) / 14
                gvArtikli.Columns(i).Caption = gvArtikli.Columns(i).GetCaption().Trim.Substring(0, gvArtikli.Columns(i).GetCaption().Trim.Length - 4)
                If Err.Number <> 0 Then
                    ' MsgBox(Err.Description, MsgBoxStyle.Critical)
                End If
            End If

            'i = i + 1


            riEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
            riEdit.Mask.EditMask = "N2"
            riEdit.Mask.UseMaskAsDisplayFormat = True

            If strEditColumns.IndexOf(gvArtikli.Columns(i).GetCaption()) > 0 Then

                If gvArtikli.Columns(i).ColumnType = GetType(Boolean) Then
                    gvArtikli.Columns(i).ColumnEdit = riEditChk
                Else
                    gvArtikli.Columns(i).ColumnEdit = riEdit
                End If

                gvArtikli.Columns(i).OptionsColumn.AllowEdit = True
                gvArtikli.Columns(i).Tag = "1"
                gvArtikli.Columns(i).AppearanceCell.BackColor = Color.Azure
            Else
                gvArtikli.Columns(i).OptionsColumn.AllowEdit = False
            End If


        Next
        gvArtikli.Columns("rcb555").OptionsColumn.AllowEdit = True
        gvArtikli.Columns("rcb555").Visible = False
        If Err.Number <> 0 Then
            'MsgBox(Err.Description, MsgBoxStyle.Critical)
        End If
        'gvArtikli.MoveLast()
        'gvArtikli.MakeRowVisible(gvArtikli.GetFocusedRow)
    End Sub



    Public Sub NapuniDXGridLATE(gcArtikli As DevExpress.XtraGrid.GridControl, gvArtikli As DevExpress.XtraGrid.Views.Grid.GridView, strEditColumns As String)
        On Error Resume Next


        gvArtikli.OptionsView.ColumnAutoWidth = False


        For i = 0 To gvArtikli.Columns.Count - 1

            If gvArtikli.Columns(i).GetCaption().ToLower.EndsWith("id") Then
                gvArtikli.Columns(i).Visible = False
            End If

            If IsNumeric(gvArtikli.Columns(i).GetCaption().Trim.Substring(gvArtikli.Columns(i).GetCaption().Trim.Length - 4, 4)) Then
                gvArtikli.Columns(i).Width = gvArtikli.Columns(i).GetCaption().Trim.Substring(gvArtikli.Columns(i).GetCaption().Trim.Length - 4, 4) / 14
                gvArtikli.Columns(i).Caption = gvArtikli.Columns(i).GetCaption().Trim.Substring(0, gvArtikli.Columns(i).GetCaption().Trim.Length - 4)
                If Err.Number <> 0 Then
                    ' MsgBox(Err.Description, MsgBoxStyle.Critical)
                End If
            End If

            'i = i + 1


            riEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
            riEdit.Mask.EditMask = "N2"
            riEdit.Mask.UseMaskAsDisplayFormat = True

            If strEditColumns.IndexOf(gvArtikli.Columns(i).GetCaption()) > 0 Then

                If gvArtikli.Columns(i).ColumnType = GetType(Boolean) Then
                    gvArtikli.Columns(i).ColumnEdit = riEditChk
                Else
                    gvArtikli.Columns(i).ColumnEdit = riEdit
                End If

                gvArtikli.Columns(i).OptionsColumn.AllowEdit = True
                gvArtikli.Columns(i).Tag = "1"
                gvArtikli.Columns(i).AppearanceCell.BackColor = Color.Azure
            Else
                gvArtikli.Columns(i).OptionsColumn.AllowEdit = False
            End If


        Next
        gvArtikli.Columns("rcb555").OptionsColumn.AllowEdit = True
        gvArtikli.Columns("rcb555").Visible = False
        If Err.Number <> 0 Then
            'MsgBox(Err.Description, MsgBoxStyle.Critical)
        End If
        'gvArtikli.MoveLast()
        'gvArtikli.MakeRowVisible(gvArtikli.GetFocusedRow)
    End Sub




    'Private Sub riEdit_Leave(sender As Object, e As EventArgs) Handles riEdit.Leave
    '    On Error Resume Next

    '    Dim val = sender.EditValue
    '    If Not IsNumeric(val) Then
    '        sender.EditValue = 0
    '    End If

    '    Dim trs As DataTable



    '    'trs = ExecSql(sql)
    '    'If Err.Number <> 0 Then
    '    '    MsgBox(Err.Description)
    '    'Else

    '    'End If



    'End Sub



    Sub CustomFormatKolonaDX(ByVal Glbl As DevExpress.XtraGrid.Views.Grid.GridView, ByVal m_nodeList As Xml.XmlNodeList, ByVal strCLEARCOLUMS As String, ByVal strCENTERCOLUMS As String)
        ' custom foramat kolona 
        'Exit Sub
        On Error Resume Next
        If m_nodeList Is Nothing Then Exit Sub
        If m_nodeList.Count = 0 Then Exit Sub

        'Glbl.OptionsView.ColumnAutoWidth = False

        ' Glbl.Columns("rcb555").Visible = False


        Dim m_node2 As Xml.XmlNode

        If strCLEARCOLUMS.ToLower = "true" Then
            For i = 0 To Glbl.Columns.Count - 1
                Glbl.Columns(i).Visible = False
            Next
        End If

        Dim iPosition As Integer = 0

        For Each m_node2 In m_nodeList
            Dim tIndex As Integer = 0
            Dim tName As String = ""
            Dim tWidth As Integer = 0
            Dim tmaxLines As Integer = 0
            Dim tCaption As String = ""
            Dim tFormat As String = ""
            Dim tVisible As String = ""
            Dim tForeColor As String = ""
            Dim tBackColor As String = ""
            Dim tAggregateFunction As String = ""
            Dim tNegativeRed As String = ""
            Dim tNotZeroRed As String = ""
            Dim tPosition As String = ""
            Dim tAutoSize As String = ""

            iPosition = iPosition + 1


            tIndex = Val(m_node2.Attributes.GetNamedItem("INDEX").Value.Trim)
            tName = m_node2.Attributes.GetNamedItem("NAME").Value.Trim
            tWidth = Val(m_node2.Attributes.GetNamedItem("WIDTH").Value.Trim)
            'tCaption = Translate(m_node2.Attributes.GetNamedItem("CAPTION").Value.Trim)
            tCaption = m_node2.Attributes.GetNamedItem("CAPTION").Value.Trim
            tFormat = m_node2.Attributes.GetNamedItem("FORMAT").Value.Trim
            tVisible = m_node2.Attributes.GetNamedItem("VISIBLE").Value.Trim
            tForeColor = m_node2.Attributes.GetNamedItem("FORECOLOR").Value.Trim
            tBackColor = m_node2.Attributes.GetNamedItem("BACKCOLOR").Value.Trim
            tmaxLines = m_node2.Attributes.GetNamedItem("MAXLINES").Value.Trim
            tAggregateFunction = m_node2.Attributes.GetNamedItem("AGREGATEFUNCTION").Value.Trim
            tNegativeRed = m_node2.Attributes.GetNamedItem("NEGATIVERED").Value.Trim
            tNotZeroRed = m_node2.Attributes.GetNamedItem("NOTZERORED").Value.Trim
            tPosition = m_node2.Attributes.GetNamedItem("POSITION").Value.Trim
            tAutoSize = m_node2.Attributes.GetNamedItem("AUTOSIZE").Value.Trim


            If tIndex > 0 Or tName <> "" Then
                If Not Glbl.Columns(IIf(tIndex = 0, tName, tIndex)) Is Nothing Then


                    Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).DisplayFormat.FormatString = "{0:N2}"
                    CType(Glbl.Columns(IIf(tIndex = 0, tName, tIndex)), DevExpress.XtraGrid.Columns.GridColumn).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    If tWidth > 0 Then Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).width = tWidth / FakPretvorbe
                    If tCaption <> "" Then Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).caption = tCaption

                    If tVisible <> "" Then Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).visible = tVisible
                    If tPosition <> "" Then Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).VisibleIndex = tPosition

                    If strCLEARCOLUMS.ToLower = "true" Then
                        iPosition = iPosition + 1
                        Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).VisibleIndex = iPosition
                    End If

                    Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).caption = Glbl.Columns(IIf(tIndex = 0, tName, tIndex)).caption.replace(";", vbCrLf)


                End If
            End If

        Next
        '--------------------------------- end custom foramat

        UkljuciSum(True, Glbl)

        ' Glbl.OptionsView.ColumnAutoWidth = False


    End Sub


    Private Sub UkljuciSum(ukljuci As Boolean, ByVal Glbl As DevExpress.XtraGrid.Views.Grid.GridView)
        If ukljuci = True Then
            For Each col As DevExpress.XtraGrid.Columns.GridColumn In Glbl.Columns

                If col.ColumnType = GetType(Decimal) Then
                    Dim GroupSum As New DevExpress.XtraGrid.GridGroupSummaryItem
                    GroupSum.DisplayFormat = "{0:N2}"
                    GroupSum.FieldName = col.FieldName
                    GroupSum.SummaryType = DevExpress.Data.SummaryItemType.Sum
                    GroupSum.ShowInGroupColumnFooter = Glbl.Columns(col.FieldName)
                    Glbl.GroupSummary.Add(GroupSum)
                    col.DisplayFormat.FormatString = "{0:N2}"
                End If
            Next
        Else
            Glbl.GroupSummary.Clear()
        End If


    End Sub



    Function GetDXID(GridView1) As String
        GetDXID = "0"
        If GridView1.RowCount > 0 Then
            On Error Resume Next
            GetDXID = GridView1.GetFocusedRowCellValue("id")
            If Err.Number <> 0 Or GridView1.GetFocusedRowCellValue("id") Is Nothing Then
                GetDXID = GridView1.GetFocusedRowCellValue(GridView1.Columns(0).FieldName).ToString()
            End If
            If GetDXID = "" Then GetDXID = "0"
        Else
            GetDXID = "0"
        End If
    End Function


    Sub BlagajnaEnableZaMlinar(strId As String)
        On Error Resume Next
        Dim Sql As String = "spmpBlagajnaEnable @mpkaseid = " & strId
        Dim trs As DataTable = ExecSql2(Sql)
        Dim ind As String = ""
        ind = trs(0)(0)
        If Err.Number <> 0 Or ind Is Nothing Then
            Sql = "update mpkase set indenable = 0 where mpkaseid = " & strId
        Else
            Sql = "update mpkase set indenable = 1 where mpkaseid = " & strId
        End If
        ExecSql(Sql)
    End Sub


End Class
