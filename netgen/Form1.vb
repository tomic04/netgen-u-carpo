Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Xml
Imports System.IO.IsolatedStorage
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.Runtime.Remoting
Imports GridExPrinting.GridExPrinting


'Imports Microsoft.Reporting.WinForms

'Imports DevExpress.Utils
'Imports DevExpress.Utils.Menu
'Imports DevExpress.XtraGrid.Menu
'Imports DevExpress.XtraGrid.Columns


Public Class Form1
    ' Public CB(400, 1) As String
    Public strXML2 As String = ""
    Public strXMLfile2 As String = ""
    Public cls1 As Class1

    Public panelHwnd As Integer

    Public Ikona As String

    Public PrijevodjeutijekU As Boolean = False

    Private m_parentControl As Control
    Private m_parentForm As Form

    Friend WithEvents txtCustom As System.Windows.Forms.TextBox
    Public ID As String
    Public DetID As String
    Public Node As String = "null"
    Public strTimer As String
    Public strStartupStora As String = ""
    Public strONLoad2 As String = ""
    Public strSETdefaults As String = ""

    Public strTreeNodes As String = ""
    Public strDBlClickForm As String = ""
    Public strDBlClickDetForm As String = ""
    Public strMenu As String = ""
    Dim FakPretvorbe As Long = 14
    Public strPageOrientation As String = "PORTRAIT"
    Public strZadnjiSQLsakojimjepunjenGrid As String = ""
    Public strOpisniParametri As String = ""
    Public ResetGridLayout As Boolean
    Public MultiGridLayout As Boolean = False
    Public strCHECKBOXVISIBLE As String = "true"
    Public strTOTALROWVISIBLE As String = ""
    Public strDetCHECKBOXVISIBLE As String = ""
    Public strFilterModeDet As String = ""
    Public strFilterMode As String = ""
    Public strDetTOTALROWVISIBLE As String = ""
    Public strRelacija As String = ""
    Public strOnEnterKey As String = ""

    Public AktivnaFormaMaster As String = ""
    Public AktivnaFormaDetalj As String = ""

    Public PunimMasterGrid As Boolean = False

    Public strPageHeaderLeft As String = ""
    Public strPageHeaderRight As String = ""
    Public strPageHeaderCenter As String = ""

    Public strPageFooterLeft As String = ""
    Public strPageFooterRight As String = ""
    Public strPageFooterCenter As String = ""
    Public strHelpKey As String = ""

    Public strRemotedllpath As String = ""
    Public ParametriSaGrida As String = ""
    Private Const TREE_RELATIONSHIP As String = "TreeParentChild"
    Private ControlClicked As Boolean
    Public strSQL2 As String = ""
    Public strSQL3 As String = ""
    Public strSQL4 As String = ""
    Dim m_nodeListZaCustomFormat As Xml.XmlNodeList
    Dim m_nodeListZaCustomFormatDET As Xml.XmlNodeList
    Dim strCLEARCOLUMS As String = ""
    Dim strCLEARCOLUMSdet As String = ""
    Dim strHeaderAlignment As String = ""
    Dim strHeaderAlignmentdet As String = ""

    Public strSQLSERVERlogin As String = ""
    Public strSQLSERVERpassword As String = ""

    Public strCombo As String = ""
    Public strComboClick As String = ""


    Public strEditColumns As String = ""
    Public strONCellEditedstr As String = ""
    Public strBeforeCellEditedstr As String = ""


    Public strEditColumnsDET As String = ""
    Public strONCellEditedDETstr As String = ""
    Public strBeforeCellEditedDETstr As String = ""


    Dim dontclose As Boolean = False
    Dim dontclose2 As Boolean = False

    Dim PredhodnaVrijednost As String = ""
    Dim strONClosed As String = ""


    Dim TrenutnaKolona As Janus.Windows.GridEX.GridEXColumn
    Dim TrenutnaVrijednost As String = ""
    Dim TrenutnaKolonaDX As String = ""

    Dim strCUSTOMIMASTERGRIDLAYOUTFILE As String = ""
    Dim strTREEDragDrop As String = ""

    Dim TopRowIndex As Integer
    Dim gFocusedRowHandle As Integer
    Dim strSplitterDistance As String = ""
    Dim strREFRESHAFTERCHILD As String = ""
    Dim strTranslateColumns As String = ""
    Dim strTranslateDETColumns As String = ""

    Dim strTREETranslate As String = ""

    Dim UtjekuJeRucnoCheckiranjeRedova As Boolean = False


    '--------------- Igić

    <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetParent(ByVal child As Integer, ByVal newParent As Integer) As Integer
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, _
     ByVal hWndInsertAfter As IntPtr, _
     ByVal X As Integer, _
     ByVal Y As Integer, _
     ByVal cx As Integer, _
     ByVal cy As Integer, _
    ByVal uFlags As UInteger) As Boolean
    End Function

    Public Const SWP_NOSIZE As Integer = &H1
    Public Const SWP_NOMOVE As Integer = &H2
    Public Const SWP_NOZORDER As Integer = &H4
    Public Const SWP_NOREDRAW As Integer = &H8
    Public Const SWP_NOACTIVATE As Integer = &H10
    'Public Const SWP_DRAWFRAME As Integer = &H20
    Public Const SWP_FRAMECHANGED As Integer = &H20
    Public Const SWP_SHOWWINDOW As Integer = &H40
    Public Const SWP_HIDEWINDOW As Integer = &H80
    Public Const SWP_NOCOPYBITS As Integer = &H100
    Public Const SWP_NOOWNERZORDER As Integer = &H200
    Public Const SWP_NOREPOSITION As Integer = &H200
    Public Const SWP_NOSENDCHANGING As Integer = &H400
    Public Const SWP_DEFERERASE As Integer = &H2000
    Public Const SWP_ASYNCWINDOWPOS As Integer = &H4000

   

    Public ReadOnly Property hWnd() As Integer
        Get
            Return Handle.ToInt32
        End Get
    End Property


    Public WriteOnly Property MainHandle() As Integer
        Set(ByVal value As Integer)
            Me.main_handle = value
            Globalmain_handle = value
        End Set
    End Property

    Public ReadOnly Property WCaption() As String
        Get
            Return Me.Text
        End Get
    End Property

    Private main_handle As IntPtr

    '------------------------------------------------------ za novi client ....

    '<DllImport("user32.dll")> _
    ' Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, _
    '      ByVal hWndInsertAfter As IntPtr, _
    '      ByVal X As Integer, _
    '      ByVal Y As Integer, _
    '      ByVal cx As Integer, _
    '      ByVal cy As Integer, _
    '     ByVal uFlags As UInteger) As Boolean
    'End Function


    'Public Const SWP_NOZORDER As Integer = &H4
    'Public Const SWP_NOACTIVATE As Integer = &H10

    Public Sub ShellResize(ByVal w As Integer, ByVal h As Integer)
        If Me.WindowState = FormWindowState.Maximized Then
            SetWindowPos(Me.hWnd, New IntPtr(-1), 0, 0, w, h, SWP_NOACTIVATE Or SWP_NOZORDER)
        End If
    End Sub



    '***
    Protected Overrides Sub WndProc(ByRef m As Message)
        Const WM_DISPLAYCHANGE As Integer = &H7E
        Const WM_DPICHANGED As Integer = &H2E0

        MyBase.WndProc(m)

        If m.Msg = WM_DISPLAYCHANGE OrElse m.Msg = WM_DPICHANGED Then
            NativeChildHost.AttachAndAlignToPanel(Me.Handle, panelHwnd)
        End If
    End Sub
    '****


    Public Function ClientToolbarGetButtons() As String
        Dim tStr As String = ""
        ' Dim i As Integer
        tStr = "<buttons>"
        'For i = 1 To 10
        '    tStr = tStr & "<button type=""button"" name=""button" & i & """ caption=""" & Controls("Button" & i).Text & """ icon="" "" enaled=""" & Controls("Button" & i).Enabled & """ visible=""" & Controls("Button" & i).Visible & """ />"
        'Nextw

        If Button1.Visible Then tStr = tStr & "<button type=""button"" name=""button1"" caption=""" & Button1.Text & """ icon="" "" enabled=""" & Button1.Enabled & """ visible=""" & Button1.Visible & """ />"
        If Button2.Visible Then tStr = tStr & "<button type=""button"" name=""button2"" caption=""" & Button2.Text & """ icon="" "" enabled=""" & Button2.Enabled & """ visible=""" & Button2.Visible & """ />"
        If Button3.Visible Then tStr = tStr & "<button type=""button"" name=""button3"" caption=""" & Button3.Text & """ icon="" "" enabled=""" & Button3.Enabled & """ visible=""" & Button3.Visible & """ />"
        If Button4.Visible Then tStr = tStr & "<button type=""button"" name=""button4"" caption=""" & Button4.Text & """ icon="" "" enabled=""" & Button4.Enabled & """ visible=""" & Button4.Visible & """ />"
        If Button5.Visible Then tStr = tStr & "<button type=""button"" name=""button5"" caption=""" & Button5.Text & """ icon="" "" enabled=""" & Button5.Enabled & """ visible=""" & Button5.Visible & """ />"

        tStr = tStr & "</buttons>"
        ClientToolbarGetButtons = tStr
    End Function

    Public Sub ClientToolbarButtonClick(ByVal str As String)
        'NovaGenerickaForma(Controls(str).Tag)

        ' Button1.PerformClick()

        If str = "button1" Then Button1.PerformClick()
        If str = "button2" Then Button2.PerformClick()
        If str = "button3" Then Button3.PerformClick()
        If str = "button4" Then Button4.PerformClick()
        If str = "button5" Then Button5.PerformClick()

    End Sub

    Private Sub Form1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        On Error Resume Next
        'TimerOn()
        Timer2.Enabled = True
        Form1_Resize(sender, e)
        SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D1, Me.Handle, IntPtr.Zero)
        'Windows.Forms.SendKeys.Send("{%}")

        'Windows.Forms.SendKeys.Send("%")
        'Application.DoEvents()
        'Windows.Forms.SendKeys.Send("%")
        'Application.DoEvents()

    End Sub

    Private Sub Form1_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        On Error Resume Next
        TimerOff()
        SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D2, Me.Handle, IntPtr.Zero)
    End Sub


    Public Sub Fiskalizacija(ByVal mpBlagajnaglaid As String, Optional ByVal TablicaFiskalizacije As Integer = 1)

        On Error Resume Next

        

        Dim trs333 As DataTable
        'trs333 = cls1.ExecSql("select * from mpfiskalizacija")


        If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 3 Then ' MUT
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 5 Then ' MARTINA
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 4 Then ' LOGARA
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpponudagla where mpponudaglaid = " & Trim(mpBlagajnaglaid) & " ))")))
        Else
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpblagajnagla where mpblagajnaglaid = " & Trim(mpBlagajnaglaid) & "  ))")))
        End If



        If trs333 Is Nothing Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If

        If trs333.Rows.Count = 0 Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If


        If trs333(0)("datum") > Now Then
            cls1.NapuniCBLog("DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!! NEMA FISKALIZACIJE ! ")
            Exit Sub
        End If

        Dim strPFX As String = ""
        Dim strPFXLozinka As String = ""
        Dim strmojCISURL As String = ""
        Dim strmojTEMEout As String = ""
        Dim strPutanjaZaZahtjeveiOdgovoreOdPorezne As String = ""

        strPFX = trs333(0)("pfx")
        strPFXLozinka = trs333(0)("lozinkazapfx")
        strmojCISURL = trs333(0)("url")
        strmojTEMEout = trs333(0)("timeout")
        strPutanjaZaZahtjeveiOdgovoreOdPorezne = trs333(0)("PutanjaZaZahtjeveiOdgovoreOdPorezne")


        Err.Clear()

        If strmojCISURL.ToLower.Contains("fu.gov.si") Then
            Dim a2 As Assembly = Assembly.Load("SLOFISKAL")
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod Assembly.Load(SLOFISKAL) : " & Err.Description)
                MsgBox(Err.Description)
                Exit Sub
            End If

            Dim Fiskal2 As Object = a2.CreateInstance("SLOFISKAL.Program")
            Fiskal2.strCS = Conn.ConnectionString
            mpBlagajnaglaid = SrediSQL(mpBlagajnaglaid)
            cls1.NapuniCBLog("zovem SLOFISKAL.SLORacun(" & mpBlagajnaglaid & ")")
            Fiskal2.SLORacun(mpBlagajnaglaid, TablicaFiskalizacije)
            Exit Sub
        End If


        Err.Clear()

        Dim a As Assembly = Assembly.Load("Raverus.FiskalizacijaDEV.COM")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(Raverus.FiskalizacijaDEV) : " & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If

        Err.Clear()
        Dim Fiskal As Object = a.CreateInstance("Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod CreateInstance (Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav) : " & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If


        If Trim(strmojCISURL) = "" Then
            strmojCISURL = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService"
        End If

        If Trim(strmojTEMEout) = "" Then
            strmojTEMEout = "5000"
        End If

        If Trim(strPFX) = "" Then
            MsgBox("Nije definiran PFX !")
            Exit Sub
        End If

        If Trim(strPFXLozinka) = "" Then
            MsgBox("Nije definirana lozinka za  PFX !")
            Exit Sub
        End If

        Dim rsFiskal As DataTable
        Err.Clear()

        If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spFaktureXMLGetRacunZahtijev @faktureid=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 3 Then 'MUT
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spBSracuniXMLGetRacunZahtijev @action=2,@bsracuniglaid=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spPonudaGetXMLZahtijev @ProPonGlaId=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spMPXMLGetPDZahtijev @mpponudaglaid=" & mpBlagajnaglaid)))
        Else
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spMPXMLGetRacunZahtijev @mpblagajnaglaid=" & mpBlagajnaglaid)))
        End If

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("GREŠKA U STORI :  " & vbCr & Err.Description)
            MsgBox(Err.Description)
            Err.Clear()
            Exit Sub
        End If



        ' ---------------------------- 20.01.2017. provjera
        Err.Clear()
        Dim rsProvjera As DataTable
        Dim sql33 As String
        Dim strProvjera As String = ""

        If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
            sql33 = "select isnull(jir,'') as jir from fakture  where faktureid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 3 Then 'MUT
            sql33 = "select isnull(jir,'') as jir from BSRacuniGla  where BSRacuniGlaid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
            sql33 = "select isnull(jir,'') as jir from ProPonGla  where ProPonGlaId=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
            sql33 = "select isnull(jir,'') as jir from MPPonudagla  where MPPonudaglaid=" & mpBlagajnaglaid
        Else
            sql33 = "select isnull(jir,'') as jir from mpblagajnagla   where mpblagajnaglaid=" & mpBlagajnaglaid
        End If
        rsProvjera = cls1.ExecSql(SrediSQL(sql33))
        strProvjera = rsProvjera(0)(0).ToString.Trim
        If strProvjera <> "" Then
            cls1.NapuniCBLog("RAČUN VEĆ IMA JIR, fiskalizacija prekinuta !! ID=" & mpBlagajnaglaid)
            Exit Sub
        End If
        ' ----------------------------end of provjera



        Dim jir
        jir = ""
        Err.Clear()
        Dim Odgovor As String
        Dim strxml6 As String
        strxml6 = rsFiskal(0)(0)


        Dim Uuid
        Uuid = Fiskal.GenerirajUuid()

        Dim zki
        zki = Fiskal.ZastitniKodIzracunCertifikatDatoteka(MOJremotedllpath & "\Certificates\" & strPFX, Trim(strPFXLozinka), rsFiskal(0)("oib"), rsFiskal(0)("datumivrijeme"), rsFiskal(0)("brojcanaOznakaRacuna"), rsFiskal(0)("oznakaPoslovnogProstora"), rsFiskal(0)("oznakaNaplatnogUredaja"), rsFiskal(0)("ukupniIznosRacuna"))

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("GREŠKA KOD IZRAČUNA ZAŠTITNOG KODA " & vbCr & Err.Description)
            cls1.NapuniCBLog("PUTANJA DO PFX-a : " & vbCr & MOJremotedllpath & "\Certificates\" & strPFX)
            MsgBox("GREŠKA KOD IZRAČUNA ZAŠTITNOG KODA " & vbCr & Err.Description)
            Exit Sub
        End If


        'Err.Clear
        Dim sql78 As String

        If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
            sql78 = "update fakture set  UUID='" & Uuid & "',KOD='" & zki & "' where faktureid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 3 Then 'MUT
            sql78 = "update BSRacuniGla set  UUID='" & Uuid & "',KOD='" & zki & "' where BSRacuniGlaid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
            sql78 = "update ProPonGla set  UUID='" & Uuid & "',KOD='" & zki & "' where ProPonGlaId=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
            sql78 = "update MPPonudagla set  UUID='" & Uuid & "',KOD='" & zki & "' where MPPonudaglaid=" & mpBlagajnaglaid
        Else
            sql78 = "update mpblagajnagla set  UUID='" & Uuid & "',KOD='" & zki & "' where mpblagajnaglaid=" & mpBlagajnaglaid
        End If

        cls1.ExecSql(SrediSQL(sql78))

        strxml6 = Replace(strxml6, "ZZZZZZZZZZ", zki)
        strxml6 = Replace(strxml6, "UUUUUUUUUU", Uuid)

        Dim StatusCisServisa

        Err.Clear()
        Dim strGreskaKodSlanjaPoruke As String
        strGreskaKodSlanjaPoruke = ""

        Fiskal.CisUrl = strmojCISURL
        Fiskal.TimeOut = strmojTEMEout

        If strPutanjaZaZahtjeveiOdgovoreOdPorezne = "" Then
            Fiskal.NazivMapeZahtjev = "C:\FISKALIZACIJA\RACUNI\ZAHTJEVI"
            Fiskal.NazivMapeOdgovor = "C:\FISKALIZACIJA\RACUNI\ODGOVORI"
        ElseIf strPutanjaZaZahtjeveiOdgovoreOdPorezne.Trim.ToUpper = "DB" Then
            Fiskal.NazivMapeZahtjev = ""
            Fiskal.NazivMapeOdgovor = ""
        Else
            Fiskal.NazivMapeZahtjev = strPutanjaZaZahtjeveiOdgovoreOdPorezne & "\FISKALIZACIJA\RACUNI\ZAHTJEVI"
            Fiskal.NazivMapeOdgovor = strPutanjaZaZahtjeveiOdgovoreOdPorezne & "\FISKALIZACIJA\RACUNI\ODGOVORI"
        End If

        Fiskal.NazivAutoGeneriranje = True


        cls1.NapuniCBLog("POZIVAM FISKALIZACIJSKI SERVIS : " & strmojCISURL)
        Err.Clear()

        If strPutanjaZaZahtjeveiOdgovoreOdPorezne.Trim.ToUpper = "DB" Then
            ' spremanje zahtjeva u bazu
            Dim Sql999 As String = "spmpXMLFiskalizacije @action = 1, @brojdok = '" & mpBlagajnaglaid & "', @uuid='" & Uuid & "', @godina = '" & TablicaFiskalizacije & "' , @zahtjev = '" & strxml6 & "' "
            cls1.ExecSql(Sql999)
            Err.Clear()
        End If

        Odgovor = Fiskal.PosaljiSoapPorukuCertifikatDatoteka(strxml6, MOJremotedllpath & "\Certificates\" & strPFX, Trim(strPFXLozinka))

        If Err.Number <> 0 Then
            strGreskaKodSlanjaPoruke = "55555"
            cls1.NapuniCBLog("GREŠKA KOD FISKALIZACIJE RAČUNA : " & Err.Description & vbCr & vbCr & Odgovor & vbCr & vbCr & Fiskal.OdgovorGreska & vbCr & vbCr & Fiskal.OdgovorGreskaStatus & vbCr & vbCr & Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
            MsgBox(Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
        End If

        If strGreskaKodSlanjaPoruke <> "" Then
            If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
                sql78 = "update fakture set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where faktureid=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 3 Then 'MUT
                sql78 = "update BSRacuniGla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where BSRacuniGlaid=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
                sql78 = "update ProPonGla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where ProPonGlaId=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
                sql78 = "update MPPonudagla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where MPPonudaglaid=" & mpBlagajnaglaid
            Else
                sql78 = "update mpblagajnagla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where mpblagajnaglaid=" & mpBlagajnaglaid
            End If
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql78)))
        End If


        If strGreskaKodSlanjaPoruke <> "" Then
            'MsgBox "Neuspjelo slanje XML-a za fiskalizaciju !" & vbCr & vbCr & Err.Description
        Else
            'MsgBox Odgovor
            jir = Fiskal.DohvatiJir(Odgovor)

            'Raverus je inače da sada imao metodu za čitanje jir-a iz XML-a. Očito da sada ne može doći do JIR-a jer je unutar drugačijeg noda PrateciDokumentiOdgovor, a ne RacunOdgovor
            ' pa ako nemamo jir , a 4 ili 5 je onda idem ručno po jir u xml jer mi se čini mijenjati raverus veći problem, who knows
            If jir = "" And (TablicaFiskalizacije = 4 Or TablicaFiskalizacije = 5) Then
                Dim doc As XmlDocument = New XmlDocument()
                doc.LoadXml(Odgovor)
                Dim nsmgr As XmlNamespaceManager

                nsmgr = New XmlNamespaceManager(doc.NameTable)
                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/")
                nsmgr.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73")

                Dim root As XmlElement = doc.DocumentElement
                Dim node As XmlNode = root.SelectSingleNode("soap:Body/tns:PrateciDokumentiOdgovor/tns:Jir", nsmgr)
                If node IsNot Nothing Then jir = node.InnerText

                If jir = "" Then
                    Dim node2 As XmlNode = root.SelectSingleNode("soap:Body/tns:RacunPDOdgovor/tns:Jir", nsmgr)
                    If node2 IsNot Nothing Then jir = node2.InnerText
                End If

            End If

            ' update prometa :
            Err.Clear()
            Dim sql77 As String

            If TablicaFiskalizacije = 2 Then ' za Božine fakture TablicaFiskalizacije = 2
                sql77 = "update fakture set sifragreske='OK',UUID='" & Uuid & "',KOD='" & zki & "',JIR='" & jir & "' where faktureid=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 3 Then 'MUT
                sql77 = "update BSRacuniGla set sifragreske='OK',UUID='" & Uuid & "',KOD='" & zki & "',JIR='" & jir & "' where BSRacuniGlaid=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
                sql77 = "update ProPonGla set sifragreske='OK',UUID='" & Uuid & "',KOD='" & zki & "',JIR='" & jir & "' where ProPonGlaId=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
                sql77 = "update MPPonudagla set sifragreske='OK',UUID='" & Uuid & "',KOD='" & zki & "',JIR='" & jir & "' where MPPonudaglaid=" & mpBlagajnaglaid
            Else
                sql77 = "update mpblagajnagla set sifragreske='OK',UUID='" & Uuid & "',KOD='" & zki & "',JIR='" & jir & "' where mpblagajnaglaid=" & mpBlagajnaglaid
            End If

            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql77)))
            If Err.Number <> 0 Then
                'MsgBox(Err.Description)
            End If


            If strPutanjaZaZahtjeveiOdgovoreOdPorezne.Trim.ToUpper = "DB" Then
                ' spremanje odgovora u bazu
                Dim Sql999 As String = "spmpXMLFiskalizacije @action=2,@uuid='" & Uuid & "', @odgovor='" & Odgovor & "' "
                cls1.ExecSql(Sql999)
                Err.Clear()
            End If



        End If

        If jir = "" Then
            'MsgBox("Neuspjela fiskalizacija , nema JIR-a !")
        End If

        Err.Clear()

    End Sub



    Public Sub FiskalizacijaNapojnica(ByVal mpBlagajnaglaid As String, Optional ByVal TablicaFiskalizacije As Integer = 1)

        On Error Resume Next



        Dim trs333 As DataTable

        trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpblagajnagla where mpblagajnaglaid = " & Trim(mpBlagajnaglaid) & "  ))")))
        

        If trs333 Is Nothing Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If

        If trs333.Rows.Count = 0 Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If


        If trs333(0)("datum") > Now Then
            cls1.NapuniCBLog("DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!! NEMA FISKALIZACIJE ! ")
            Exit Sub
        End If

        Dim strPFX As String = ""
        Dim strPFXLozinka As String = ""
        Dim strmojCISURL As String = ""
        Dim strmojTEMEout As String = ""
        Dim strPutanjaZaZahtjeveiOdgovoreOdPorezne As String = ""

        strPFX = trs333(0)("pfx")
        strPFXLozinka = trs333(0)("lozinkazapfx")
        strmojCISURL = trs333(0)("url")
        strmojTEMEout = trs333(0)("timeout")
        strPutanjaZaZahtjeveiOdgovoreOdPorezne = trs333(0)("PutanjaZaZahtjeveiOdgovoreOdPorezne")


        Err.Clear()
 

        Err.Clear()

        Dim a As Assembly = Assembly.Load("Raverus.FiskalizacijaDEV.COM")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(Raverus.FiskalizacijaDEV) : " & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If

        Err.Clear()
        Dim Fiskal As Object = a.CreateInstance("Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod CreateInstance (Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav) : " & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If


        If Trim(strmojCISURL) = "" Then
            strmojCISURL = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService"
        End If

        If Trim(strmojTEMEout) = "" Then
            strmojTEMEout = "5000"
        End If

        If Trim(strPFX) = "" Then
            MsgBox("Nije definiran PFX !")
            Exit Sub
        End If

        If Trim(strPFXLozinka) = "" Then
            MsgBox("Nije definirana lozinka za  PFX !")
            Exit Sub
        End If

        Dim rsFiskal As DataTable
        Err.Clear()
 
        rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spMPXMLGetNapojnicaZahtijev @mpblagajnaglaid=" & mpBlagajnaglaid)))
       
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("GREŠKA U STORI :  " & vbCr & Err.Description)
            MsgBox(Err.Description)
            Err.Clear()
            Exit Sub
        End If


        Dim jir
        jir = ""
        Err.Clear()
        Dim Odgovor As String
        Dim strxml6 As String = ""
        strxml6 = rsFiskal(0)(0)
        If strxml6 = "" Then
            Err.Clear()
            Exit Sub
        End If

        'Dim Uuid
        'Uuid = Fiskal.GenerirajUuid()

        'Dim sql78 As String

        ' zakomentirao jer Peki koristi uuid od računa
        ' sql78 = "update mpblagajnagla set  UUIDNapojnica='" & Uuid & "' where mpblagajnaglaid=" & mpBlagajnaglaid
        'cls1.ExecSql(SrediSQL(sql78))
        'strxml6 = Replace(strxml6, "UUUUUUUUUU", Uuid)

        Dim StatusCisServisa

        Err.Clear()
        Dim strGreskaKodSlanjaPoruke As String
        strGreskaKodSlanjaPoruke = ""

        Fiskal.CisUrl = strmojCISURL
        Fiskal.TimeOut = strmojTEMEout

        If strPutanjaZaZahtjeveiOdgovoreOdPorezne = "" Then
            Fiskal.NazivMapeZahtjev = "C:\FISKALIZACIJA\RACUNI\ZAHTJEVI"
            Fiskal.NazivMapeOdgovor = "C:\FISKALIZACIJA\RACUNI\ODGOVORI"
        ElseIf strPutanjaZaZahtjeveiOdgovoreOdPorezne.Trim.ToUpper = "DB" Then
            Fiskal.NazivMapeZahtjev = ""
            Fiskal.NazivMapeOdgovor = ""
        Else
            Fiskal.NazivMapeZahtjev = strPutanjaZaZahtjeveiOdgovoreOdPorezne & "\FISKALIZACIJA\RACUNI\ZAHTJEVI"
            Fiskal.NazivMapeOdgovor = strPutanjaZaZahtjeveiOdgovoreOdPorezne & "\FISKALIZACIJA\RACUNI\ODGOVORI"
        End If

        Fiskal.NazivAutoGeneriranje = True


        cls1.NapuniCBLog("POZIVAM FISKALIZACIJSKI SERVIS ZA NAPOJNICU : " & strmojCISURL)
        Err.Clear()


        Odgovor = Fiskal.PosaljiSoapPorukuCertifikatDatoteka(strxml6, MOJremotedllpath & "\Certificates\" & strPFX, Trim(strPFXLozinka))

        If Err.Number <> 0 Then
            strGreskaKodSlanjaPoruke = "55555"
            cls1.NapuniCBLog("GREŠKA KOD FISKALIZACIJE NAPOJNICE : " & Err.Description & vbCr & vbCr & Odgovor & vbCr & vbCr & Fiskal.OdgovorGreska & vbCr & vbCr & Fiskal.OdgovorGreskaStatus & vbCr & vbCr & Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
            MsgBox("GREŠKA KOD FISKALIZACIJE NAPOJNICE")
            ' ovaj dohvat greške sam ipak stavio ovdje 
            Dim doc3 As XmlDocument = New XmlDocument()
            doc3.LoadXml(Fiskal.OdgovorGreska)
            cls1.NapuniCBLog("Učitao odgovoru doc ... idem pronaći Sifru greške")

            cls1.NapuniCBLog("Ovo je odgovor:")

            cls1.NapuniCBLog(Odgovor)

            cls1.NapuniCBLog("Kraj odgovora")

            Dim nsmgr3 As XmlNamespaceManager
            nsmgr3 = New XmlNamespaceManager(doc3.NameTable)
            nsmgr3.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/")
            nsmgr3.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73")
            Dim root3 As XmlElement = doc3.DocumentElement
            Dim node3 As XmlNode = root3.SelectSingleNode("/soap:Envelope/soap:Body/tns:NapojnicaOdgovor/tns:Greske/tns:Greska/tns:SifraGreske", nsmgr3)
            strGreskaKodSlanjaPoruke = "xxx"
            If node3 IsNot Nothing Then strGreskaKodSlanjaPoruke = node3.InnerText

            cls1.NapuniCBLog("Sifra Greške : " & strGreskaKodSlanjaPoruke)
            Dim sql78 As String
            sql78 = "update mpblagajnagla set  JIRNapojnica='" & strGreskaKodSlanjaPoruke & "' where mpblagajnaglaid=" & mpBlagajnaglaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql78)))

        Else
            Dim doc As XmlDocument = New XmlDocument()
            doc.LoadXml(Odgovor)
            cls1.NapuniCBLog("Učitao odgovoru doc ... idem pronaći SifraPoruke")
            Dim nsmgr As XmlNamespaceManager
            nsmgr = New XmlNamespaceManager(doc.NameTable)
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/")
            nsmgr.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73")
            Dim root As XmlElement = doc.DocumentElement
            Dim node As XmlNode = root.SelectSingleNode("/soap:Envelope/soap:Body/tns:NapojnicaOdgovor/tns:PorukaOdgovora/tns:SifraPoruke", nsmgr)
            If node IsNot Nothing Then jir = node.InnerText
            cls1.NapuniCBLog("SifraPoruke : " & jir)
            Err.Clear()
            Dim sql77 As String
            sql77 = "update mpblagajnagla set  JIRNapojnica='" & jir & "' where mpblagajnaglaid=" & mpBlagajnaglaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql77)))
        End If

        Err.Clear()

    End Sub
   

    Sub PoslovniProstor(ByVal trgovinaid As String)

        PoslovniProstor_COM(trgovinaid)



        'Dim poslovniProstor As Raverus.FiskalizacijaDEV.Schema.PoslovniProstorType = New Raverus.FiskalizacijaDEV.Schema.PoslovniProstorType() With { _
        '     .Oib = "12345678901", _
        '     .OznPoslProstora = "01" _
        '    }

        'Dim adresa As Raverus.FiskalizacijaDEV.Schema.AdresaType = New Raverus.FiskalizacijaDEV.Schema.AdresaType() With { _
        ' .Ulica = "jk", _
        ' .KucniBroj = "23", _
        ' .KucniBrojDodatak = "b", _
        ' .BrojPoste = "31000", _
        ' .Naselje = "os", _
        ' .Opcina = "os" _
        '}
        'Dim adresniPodatak As Raverus.FiskalizacijaDEV.Schema.AdresniPodatakType = New Raverus.FiskalizacijaDEV.Schema.AdresniPodatakType() With { _
        ' .Item = adresa _
        '}
        'poslovniProstor.AdresniPodatak = adresniPodatak
        'poslovniProstor.RadnoVrijeme = "od 8 do 9 "
        'poslovniProstor.DatumPocetkaPrimjene = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatum(Date.Now)
        'poslovniProstor.SpecNamj = "12345678901"

        'Dim cis As Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav = New Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav()
        '' cis.SoapMessageSending += cis_SoapMessageSending
        ''cis.SoapMessageSent += cis_SoapMessageSent

        'Dim doc As XmlDocument = cis.PosaljiPoslovniProstor(poslovniProstor, "FISKAL 1")

        'If doc IsNot Nothing Then
        '    MsgBox(doc.InnerXml)
        'Else

        'End If
    End Sub

    Sub PoslovniProstor_COM(ByVal trgovinaid As String)

        On Error Resume Next

      


        Dim trs333 As DataTable
        trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=" & Trim(trgovinaid) & " )")))


        If trs333 Is Nothing Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!" & vbCr & vbCr)
            Exit Sub
        End If

        If trs333.Rows.Count = 0 Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!" & vbCr & vbCr)
            Exit Sub
        End If

        'If trs333(0)("datum") > Now Then
        '    cls1.NapuniCBLog("DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!! NEMA FISKALIZACIJE ! ")
        '    Exit Sub
        'End If



        Dim strPFX As String = ""
        Dim strPFXLozinka As String = ""
        Dim strmojCISURL As String = ""
        Dim strmojTEMEout As String = ""
        strPFX = trs333(0)("pfx")
        strPFXLozinka = trs333(0)("lozinkazapfx")
        strmojCISURL = trs333(0)("url")
        strmojTEMEout = trs333(0)("timeout")




        If strmojCISURL.ToLower.Contains("fu.gov.si") Then
            Dim a2 As Assembly = Assembly.Load("SLOFISKAL")
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod Assembly.Load(SLOFISKAL) : " & Err.Description)
                MsgBox(Err.Description)
                Exit Sub
            End If

            Dim Fiskal2 As Object = a2.CreateInstance("SLOFISKAL.Program")
            Fiskal2.strCS = Conn.ConnectionString
            trgovinaid = SrediSQL(trgovinaid)
            cls1.NapuniCBLog("zovem SLOFISKAL.SLOPP(" & trgovinaid & ")")
            Fiskal2.SLOPP(trgovinaid)
            Exit Sub
        End If


        Dim a As Assembly = Assembly.Load("Raverus.FiskalizacijaDEV.COM")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(Raverus.FiskalizacijaDEV) : " & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If

        Dim Fiskal As Object = a.CreateInstance("Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav")



        If Trim(strmojCISURL) = "" Then
            strmojCISURL = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService"
        End If

        If Trim(strmojTEMEout) = "" Then
            strmojTEMEout = "5000"
        End If

        If Trim(strPFX) = "" Then
            MsgBox("Nije definiran PFX !")
            Exit Sub
        End If

        If Trim(strPFXLozinka) = "" Then
            MsgBox("Nije definirana lozinka za  PFX !")
            Exit Sub
        End If


        Dim rsFiskal As DataTable
        Err.Clear()
        rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("spMPXMLProstorZahtijev @trgovinaid=" & trgovinaid)))

        If Err.Number <> 0 Then
            MsgBox(Err.Description)
            Err.Clear()
        End If

        Dim jir
        jir = ""
        Err.Clear()
        Dim Odgovor As String
        Dim strxml6 As String
        strxml6 = rsFiskal(0)(0)


        Dim Uuid
        Uuid = Fiskal.GenerirajUuid()

        If Err.Number <> 0 Then
            MsgBox(Err.Description)
        End If

        'Err.Clear
        Dim sql78 As String
        sql78 = "update trgovina set  UUID='" & Uuid & "',sifragreske='',DatumSlanja=null where trgovinaid=" & trgovinaid
        cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql78)))

        strxml6 = Replace(strxml6, "UUUUUUUUUU", Uuid)

        Dim StatusCisServisa

        Err.Clear()
        Dim strGreskaKodSlanjaPoruke As String
        strGreskaKodSlanjaPoruke = ""

        Fiskal.CisUrl = strmojCISURL
        Fiskal.TimeOut = strmojTEMEout
        Fiskal.NazivMapeZahtjev = "C:\FISKALIZACIJA\POSLOVNIPROSTOR\ZAHTJEVI"
        Fiskal.NazivMapeOdgovor = "C:\FISKALIZACIJA\POSLOVNIPROSTOR\ODGOVORI"
        Fiskal.NazivAutoGeneriranje = True

        cls1.NapuniCBLog("POZIVAM FISKALIZACIJSKI SERVIS : " & strmojCISURL)

        Err.Clear()

        Odgovor = Fiskal.PosaljiSoapPorukuCertifikatDatoteka(strxml6, MOJremotedllpath & "\Certificates\" & strPFX, strPFXLozinka)

        If Err.Number <> 0 Then
            strGreskaKodSlanjaPoruke = "55555"
            cls1.NapuniCBLog("GREŠKA KOD SLANJA POSLOVNOG PROSTORA : " & Err.Description & vbCr & vbCr & Odgovor & vbCr & vbCr & Fiskal.OdgovorGreska & vbCr & vbCr & Fiskal.OdgovorGreskaStatus & vbCr & vbCr & Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
            ' MsgBox(Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
        End If


        If strGreskaKodSlanjaPoruke <> "" Then
            sql78 = "update trgovina set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where trgovinaid=" & trgovinaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql78)))
        End If

        If strGreskaKodSlanjaPoruke <> "" Then
            'MsgBox "Neuspjelo slanje XML-a za fiskalizaciju !" & vbCr & vbCr & Err.Description
        Else
            Err.Clear()
            Dim sql77 As String
            sql77 = "update trgovina set  sifragreske='OK', UUID='" & Uuid & "', datumslanja='" & Date.Now.ToString("yyyy-MM-dd hh:mm:ss") & "' where trgovinaid=" & trgovinaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(sql77)))
            If Err.Number <> 0 Then
                MsgBox(Err.Description)
            End If
        End If



        Err.Clear()

    End Sub


    Public Function DajCB() As String

        Dim j As Long = 0

        Dim sss As String = ""
        Dim strEditBox1 As String = ""
        On Error Resume Next

        If cls1 Is Nothing Then
            ' cls1 = New Class1
        End If

        For j = 0 To UBound(cls1.CB)
            If cls1.CB(j, 0).ToString.Trim <> "" Then
                sss = sss & j & ".  @@" & cls1.CB(j, 0).ToString & cls1.CB(j, 1).ToString & vbCrLf
            End If
        Next

        strEditBox1 = "---------------------------" & vbCrLf & strEditBox1


        strEditBox1 = "@@id= " & cls1.MasterID & vbCrLf & vbCrLf & strEditBox1
        strEditBox1 = "@@node= " & node77 & vbCrLf & vbCrLf & strEditBox1
        strEditBox1 = "XML = " & cls1.strXMLsakojimRadimzaCBlog & vbCrLf & vbCrLf & strEditBox1
        'strEditBox1 = "NetGen v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision & " b " & vbCrLf & "Server=" & cls1.strServer & vbCrLf & "DB=" & cls1.strBaza & vbCrLf & " @@operateriid=" & cls1.OperateriID.ToString.Trim & vbCrLf & vbCrLf & strEditBox1



        Dim VersionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        Dim verzija As String = VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Build & "." & VersionInfo.Revision


        strEditBox1 = "NetGen DLL v" & VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Revision & "" & vbCrLf & "Server=" & cls1.strServer & vbCrLf & "DB=" & cls1.strBaza & vbCrLf & " @@operateriid=" & cls1.OperateriID.ToString.Trim & vbCrLf & vbCrLf & strEditBox1

        'strEditBox1 = "Verzija utvrdo : 8.3.6." & vbCrLf & "-------------------------" & vbCrLf & strEditBox1

        strEditBox1 = strEditBox1 & sss

        DajCB = strEditBox1
    End Function

    Public Function DajSQL() As String
        Dim i As Integer
        Dim tss As String = ""
        For i = 19 To 0 Step -1
            tss = cls1.CBLog(i) & vbCrLf & vbCrLf & tss
        Next
        DajSQL = tss
    End Function

    Public Function DajParametre() As String
        PokupiParametreSaGrida()
        DajParametre = "PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida
    End Function


    '-------------------------------------------------------- ... end of za new client




    Private Sub MYExnHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject
        MsgBox(EX.StackTrace)
    End Sub

    Private Sub MYThreadHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        MsgBox(e.Exception.StackTrace)
    End Sub

    Public Function Run8(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
      ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object
        ' MsgBox("run8")
        On Error Resume Next

        Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)

        ' MsgBox(Err.Description)

    End Function

    Public Sub Run(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String, Optional ByVal Koler As String = "")
        'MsgBox("run")
        '  Me.Label1.Text = String.Concat(New String() {"server = ", server, ChrW(13) & ChrW(10) & "database = ", database, ChrW(13) & ChrW(10) & "username = ", username, ChrW(13) & ChrW(10) & "password = ", password, ChrW(13) & ChrW(10) & "operaterid = ", Operateriid.ToString})

        On Error Resume Next



        ' ### valentinotranslator4.10.2023.
        Dim assembly1 As Assembly = Assembly.Load("JupiterTranslator10")
        If Err.Number = 0 Then
            JupiterPrevoditeljValentinoBoolean = True
            JupiterPrevoditeljValentinoType = assembly1.GetType("JupiterTranslator.Translator")
            'Dim parameters() As Object = {"jupitertranslations", "jupiter2019", Nothing, Nothing}
            'JupiterPrevoditeljValentinoType.InvokeMember("Init", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters)
            'Dim parameters2() As Object = {"en"}
            'JupiterPrevoditeljValentinoType.InvokeMember("LoadTranslations", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters2)
        End If

        '  Try

        ' Dim currentDomain As AppDomain = AppDomain.CurrentDomain

        ' On Error Resume Next

        ' Dim newDomain As AppDomain = AppDomain.CreateDomain("aaa")
        ' newDomain.CreateInstanceAndUnwrap(Assembly.GetCallingAssembly().GetName().FullName, "Class1")
        ' AddHandler newDomain.UnhandledException, AddressOf MYExnHandler
        ' AddHandler Application.ThreadException, AddressOf MYThreadHandler

        ' newDomain.CreateInstanceAndUnwrap("C:\Users\kkovac.SPIN\Documents\Visual Studio 2008\Projects\NetGenDLL\NetGenDLL\bin\Debug\netgendll.dll", "form1")

        '    MsgBox("run u form1 , server: " & server & ", DB:" & database)

        cls1 = New Class1
        cls1.strServer = server
        cls1.strBaza = database
        cls1.OperateriID = Operateriid

        MOJremotedllpath = remotedllpath

        strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid


        strSQLSERVERlogin = username
        strSQLSERVERpassword = password

        If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
        If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""

        'If database = "kbco" Then ' samo za debug kbco-a
        '    strSQLSERVERlogin = "sa"
        'End If

        strRemotedllpath = remotedllpath

        ' 08.09.2026. KK - prikljuci se na JupiterLogger (JupiterLogs.dbo.LogEvent).
        ' Late-bound; ako DLL-a nema, sve je tihi no-op i NetGen radi kao prije.
        ' Granula = ime XML-a s kojim NetGen radi.
        JupiterLog.Inicijaliziraj(Mid)
        cls1.NapuniCBLog(" JupiterLogger : " & JupiterLog.Dijagnostika)
        JupiterLog.Info("Run", "open " & Mid & " | baza: " & database)

        cls1.NapuniCBLog(" Ušao u form1 RUN : " & Err.Description)
        cls1.NapuniCBLog(" server : " & server)
        cls1.NapuniCBLog(" database : " & database)
        cls1.NapuniCBLog(" username : " & username)
        cls1.NapuniCBLog(" password : " & password)
        cls1.NapuniCBLog(" server : " & server)
        cls1.NapuniCBLog(" OperateriID : " & Operateriid)
        cls1.NapuniCBLog(" remotedllpath : " & remotedllpath)


        Conn = New SqlClient.SqlConnection
        Conn3 = New SqlClient.SqlConnection
        Conn2 = New OleDb.OleDbConnection
        Conn4 = New SqlClient.SqlConnection

        ImeAplikacijeZaProfiler = "NetGenDLL"
        ImeAplikacijeZaProfiler = ImeAplikacijeZaProfiler & "-" & strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

        'frm1 = Me
        frm1 = New Form1
        frm3 = New Form3
        frm5 = New Form5
        frm3.cls1 = cls1
        frm5.cls1 = cls1

        ID = ""
        cls1.MasterID = ID

        SetParent(Handle.ToInt32, Parent) ' #######
        panelHwnd = Parent
        'NativeChildHost.AttachAndAlignToPanel(Me.Handle, Parent)

        If Koler = "Jup10" Then

            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None


            Dim parentV As Control = Control.FromHandle(Parent)

            Me.WindowState = FormWindowState.Normal
            Me.Location = New Point(0, 0)
            Me.Size = parentV.Size


            If parentV IsNot Nothing Then
                m_parentControl = parentV
                AddHandler parentV.Resize, AddressOf ParentV_Resize
            End If


            If parentV IsNot Nothing Then
                m_parentForm = parentV.FindForm()
                If m_parentForm IsNot Nothing Then
                    AddHandler m_parentForm.LocationChanged, AddressOf ParentForm_LocationChanged
                End If
            End If
        End If

        'MsgBox("prije showanja form1 " & Err.Description)

        ' Show()
        Err.Clear()
        Show()
        If Err.Number <> 0 Then
            'MsgBox(Err.Description)
        End If
        'Try
        '    Show()
        'Catch ex As Exception
        '    MessageBox.Show(ex.Message)
        'End Try

        'Catch ex As Exception
        '    'MsgBox(Err.Description)
        '    'MsgBox(ex.StackTrace)
        'End Try

    End Sub

    '<DllImport("user32.dll", CharSet:=CharSet.Auto)> _
    'Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    'End Function

    '<DllImport("user32.dll")> _
    'Private Shared Function SetParent(ByVal child As Integer, ByVal newParent As Integer) As Integer
    'End Function


    ' Properties

    Public WriteOnly Property ModuleID() As String
        Set(ByVal value As String)
            On Error Resume Next
            If value.EndsWith(")") Then
                Argument5 = value.Substring(InStr(value, "("), value.Length - InStr(value, "(") - 1)
                value = value.Substring(0, InStr(value, "(") - 1)
            End If
            Me.Mid = value

        End Set
    End Property

    Public WriteOnly Property ngParams() As String
        Set(ByVal value As String)
            On Error Resume Next
            Me.strNGParams = value
        End Set
    End Property




    'Public ReadOnly Property hWnd() As Integer
    '    Get
    '        'Return MyBase.Handle.ToInt32
    '        Return Handle.ToInt32
    '    End Get
    'End Property

    'Public WriteOnly Property MainHandle() As Integer
    '    Set(ByVal value As Integer)
    '        Me.main_handle = value
    '        Globalmain_handle = value
    '    End Set
    'End Property

    'Public ReadOnly Property WCaption() As String
    '    Get
    '        Return Me.Text
    '    End Get
    'End Property

    'Private main_handle As IntPtr
    Public Mid As String
    Public strNGParams As String
    Public Argument5 As String

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        '' KK ??????
        On Error Resume Next

        JupiterLog.Info("Run", "close " & Mid)
        ' SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
        ' Err.Clear()

        Rafalno(strONClosed, sender, e)
        If GreskaKodIzvrsavanjaSQLa <> "" Then
            MsgBox(GreskaKodIzvrsavanjaSQLa)
        End If
    End Sub



    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        On Error Resume Next
        SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D0, Me.Handle, IntPtr.Zero)
        'SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D0, Globalmain_handle, IntPtr.Zero)
        'SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D0, cls1.Form1.Handle.ToInt32(), IntPtr.Zero)

        ' Close()

        Conn.Close()
        'cls1.Conn = Nothing

        Conn2.Close()
        'cls1.Conn2 = Nothing

        Conn3.Close()
        cls1.Form2.Close()
        frm3.Close()
        cls1.Form5.Close()

        frm5.Close()

        frm5 = Nothing
        frm3 = Nothing
        frm1 = Nothing
    End Sub

    Private Sub Form1_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        TimerOn()
    End Sub

    Private Sub Form1_Invalidated(ByVal sender As Object, ByVal e As System.Windows.Forms.InvalidateEventArgs) Handles Me.Invalidated

    End Sub



    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If

        If e.KeyCode = Keys.F1 Then
            If strHelpKod.Trim <> "" Then

                'o = CreateObject("JupiterHelpClient.XtraForm1", "")
                'o.run("aaa")

                'Dim a As Assembly = Assembly.Load("JupiterHelpClient")
                'Dim o As Object = a.CreateInstance("JupiterHelpClient.XtraForm1")
                'o.run("aaa")
                'strHelpKod = "MER005"

                DajPokaziHelp()
                Exit Sub

            End If
        End If



        If e.Control Then
            If e.KeyCode = Keys.F11 Then
                PokupiParametreSaGrida()
                cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
                'frm5.ShowDialog()
            End If
        End If

        If e.Control Then
            If e.KeyCode = Keys.F9 Then
                Dim frm7 As Form7
                frm7 = New Form7
                frm7.NapuniGrid()
                frm7.ShowDialog()
                NapuniRSLang()

                TranslateForm(Me)
            End If
        End If


    End Sub

    Public Sub NapuniRSLang()
        On Error Resume Next


        'If JupiterTranslatorMP Then
        '    Dim sql = "select isnull(jezik,'') as jezik from jupitersystem..loginimages where [LOGIN] = suser_sname()"
        '    Dim trs As DataTable = cls1.ExecSql2(sql)
        '    If Not trs Is Nothing Then
        '        If trs.Rows.Count > 0 Then
        '            JupiterPrevoditelj.Database = "jupitertranslations"
        '            'JupiterPrevoditelj.Server = "kkovac-lap"
        '            JupiterPrevoditelj.Server = Conn.DataSource
        '            JupiterPrevoditelj.Username = ""
        '            JupiterPrevoditelj.Password = ""
        '            JupiterPrevoditelj.Language = trs(0)(0)
        '            JupiterPrevoditelj.LogFailed = True
        '            JupiterPrevoditelj.Module = "netgen"
        '            JupiterPrevoditelj.Init(Conn.DataSource, "jupitertranslations", "", "", trs(0)(0))
        '        End If
        '    End If
        'End If


        If JupiterTranslatorMP Then

            If JupiterTranslatorMPJezik = "" Then
                Dim sql = "select isnull(jezik,'') as jezik from jupitersystem..loginimages where [LOGIN] = suser_sname()"
                Dim trs As DataTable = cls1.ExecSql2(sql)
                If Not trs Is Nothing Then
                    If trs.Rows.Count > 0 Then
                        JupiterTranslatorMPJezik = trs(0)(0)
                    End If
                End If
            End If

            If JupiterTranslatorMPJezik = "" Then
                Dim sql = "select jezik from MPKase"
                Dim trs As DataTable = cls1.ExecSql2(sql)
                If Not trs Is Nothing Then
                    If trs.Rows.Count > 0 Then
                        JupiterTranslatorMPJezik = trs(0)(0)
                    End If
                End If
            End If


            If JupiterTranslatorMPJezik <> "" Then
                If JupiterTranslatorMPJezik.Trim.ToLower <> "hrv" Then


                    ' ### valentinotranslator4.10.2023. zakomentirano
                    JupiterPrevoditelj.Database = "jupitertranslations"
                    'JupiterPrevoditelj.Server = "kkovac-lap"
                    JupiterPrevoditelj.Server = Conn.DataSource
                    JupiterPrevoditelj.Username = ""
                    JupiterPrevoditelj.Password = ""
                    JupiterPrevoditelj.Language = JupiterTranslatorMPJezik
                    JupiterPrevoditelj.LogFailed = True
                    JupiterPrevoditelj.Module = "netgen"
                    JupiterPrevoditelj.Init(Conn.DataSource, "jupitertranslations", "", "", JupiterTranslatorMPJezik)

                 




                End If
            End If

        End If



        Return


        If Language.Trim = "" Then Exit Sub
        If RSLang Is Nothing Then
            RSLang = cls1.ExecSql("select * from TotalGenLang where lang='" & Language & "'")
            Jezici()
        End If


    End Sub


    Public Sub Jezici()
        On Error Resume Next
        Dim trs1 As DataTable

        trs1 = cls1.ExecSql(" select * from Pubvars where VarIme = 'Multilingual'")
        strMultilingual = trs1(0)("varset")

        Dim trs2 As DataTable
        trs2 = cls1.ExecSql(" select * from jezici where oznaka = '" & Language.Trim & "'")
        strPismo = trs2(0)("pismo").trim

    End Sub

    Private Sub Form1_Leave(sender As Object, e As EventArgs) Handles Me.Leave
        TimerOff()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'cls1.OperateriID = 0

        cls1.NapuniCBLog(" Ušao u form1_load : " & Err.Description)



        ' TabControl1.DrawMode = TabDrawMode.OwnerDrawFixed
        ' Me.TabControl1.Region = New Region(New RectangleF(Me.TabPage1.Left, Me.TabPage1.Top, Me.TabPage1.Width, Me.TabPage1.Height))

        '      TabControl2.DrawMode = TabDrawMode.OwnerDrawFixed
        'Me.TabControl2.Region = New Region(New RectangleF(Me.TabPage2.Left, Me.TabPage2.Top, Me.TabPage2.Width, Me.TabPage2.Height))


        'MsgBox(" Ušao u form1_load : " & Err.Description)

        PokupiUlazneparametre()


        'MsgBox(" pokupio ulazne parametre : " & Err.Description)

        GridEX1.BuiltInTexts(GridEXBuiltInText.GroupByBoxInfo) = ""
        GridEX2.BuiltInTexts(GridEXBuiltInText.GroupByBoxInfo) = ""
        GridEX3.BuiltInTexts(GridEXBuiltInText.GroupByBoxInfo) = ""

        GridEX1.GroupByBoxFormatStyle.BackColor = Color.Aqua


        'If cls1.strUser = "" Then
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"

        'Else
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & cls1.strUser & ";PWD=" & cls1.strPassword & ";"
        'End If




        On Error Resume Next

        ' MsgBox("Poruka 1 : " & vbCr & "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;")

        ' MsgBox("Poruka 1 : " & strSQLSERVERlogin)
        'Err.Clear()

        'If strSQLSERVERlogin <> "" Then
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        'Else
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        'End If


        Dim tstr5 As String = ""
        Dim tstr6 As String = ""

        'If strSQLSERVERlogin = "" Then
        '    tstr5 = "Data Source=" & cls1.strServer & ";Initial Catalog=" & cls1.strBaza & ";Integrated Security=True;"
        'Else
        '    tstr5 = "Data Source=" & cls1.strServer & ";Initial Catalog=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";"
        'End If



        If strSQLSERVERlogin = "" Then
            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
            tstr6 = "Server=" & cls1.strServer & ";Database=JupiterXML;Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        Else
            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
            tstr6 = "Server=" & cls1.strServer & ";Database=JupiterXML;UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        End If

        strGlobalSQLSERVERlogin = tstr5

        '' ------------------- garac case
        'If cls1.strBaza.ToLower.Contains("garac") Then
        '    tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=sa;PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        '    tstr6 = "Server=" & cls1.strServer & ";Database=JupiterXML;UID=sa;PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        'End If



        ' MsgBox(strSQLSERVERlogin & vbCrLf & strSQLSERVERpassword & vbCrLf & tstr5)

        ' cls1.NapuniCBLog(" conn string : " & tstr5)

        Conn.ConnectionString = tstr5
        Conn4.ConnectionString = tstr6

        ' MsgBox(" cs : " & Err.Description)


        ' Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"

        'MsgBox("Poruka 3 : " & vbCr & Err.Description & vbCr & vbCr & Conn.ConnectionString)

        If Err.Number <> 0 Then
            cls1.NapuniCBLog(" Greška  : " & tstr5)
            MsgBox(Err.Description & vbCrLf & Conn.ConnectionString)
            Exit Sub
        End If

        'Try

        Err.Clear()
        'Conn.Open()

        cls1.NapuniCBLog(" prije otvaranja konekcije, cs :  " & Conn.ConnectionString)
        Conn.Open()
        cls1.NapuniCBLog(" nakon otvaranja konekcije :  " & Conn.ConnectionString)



        If Err.Number <> 0 Then
            cls1.NapuniCBLog(" Greška kod otvarana konekcije : " & tstr5 & vbCrLf & vbCrLf & Err.Description)
            MsgBox(Err.Description)
            Exit Sub
        End If



        ' zakomentirao zbog kase
        Err.Clear()
        Conn4.Open()

        If Err.Number <> 0 Then
            cls1.NapuniCBLog(" JupiterXML DB problem : " & Err.Description)
            ' MsgBox(Err.Description)
            'Exit Sub
        End If

        Err.Clear()

        If cls1.OperateriID = 0 Then
            cls1.NapuniCBLog(" prije  PronadjiOperatera()")
            PronadjiOperatera()
            cls1.NapuniCBLog(" nakon  PronadjiOperatera()")
        End If



       

        cls1.NapuniCBLog(" prije  NapuniDrvo()")
        NapuniDrvo()
        cls1.NapuniCBLog(" nakon  NapuniDrvo()")

        cls1.NapuniCBLog(" prije  NapuniRSLang()")
        NapuniRSLang()
        cls1.NapuniCBLog(" nakon  NapuniRSLang()")

        strXMLfile2 = strXML2
        ToolStripStatusLabel2.Text = ""

        ToolStripStatusLabel2.Text = strXML2


        Dim VersionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        Dim verzija As String = VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Build & "." & VersionInfo.Revision


        ToolStripStatusLabel1.Text = "NetGen v" & VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Revision & " .. " & cls1.strServer & " .. " & cls1.strBaza & " .. " & cls1.strAppUser & " [" & cls1.OperateriID.ToString.Trim & "] "
        'ToolStripStatusLabel3.Text = cls1.Conn.ConnectionString
        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        ProcitajXMLzaPodlogu()
        Windows.Forms.Cursor.Current = Cursors.Default
        'NapuniDrvoVrijeme()


        'ClientToolbarButtonClick("button1")

        If TreeView1.Visible = False And TreeView2.Visible = False Then
            TabControl1.Visible = False
        Else
            TabControl1.Visible = True
        End If

        'Catch ex As Exception
        '    MsgBox(ex.Message)
        'End Try

        'If cls1.Conn.State = ConnectionState.Closed Then MsgBox("Database connection closed !")

        If Not IsNothing(strNGParams) Then
            If strNGParams.Trim <> "" Then
                cls1.NapuniCBLog(" strNGParams = " & strNGParams)
                NovaGenerickaForma(strNGParams)
            End If
        End If


    End Sub



    Sub PronadjiOperatera()
        On Error Resume Next
        cls1.OperateriID = 0
        Dim Trs As New DataTable
        Dim sql As String = "select * from ssoperateri where naziv ='" & cls1.strAppUser.Trim & "' and password='" & cls1.strAppPass.Trim & "'"
        Trs = cls1.ExecSql(sql)
        If Err.Number = 0 Then
            If Trs.Rows.Count > 0 Then
                cls1.OperateriID = Trs.Rows(0)(0)
            End If
        End If
    End Sub

    Sub PokupiUlazneparametre()

        Dim strStartupArguments() As String, intCount As Integer

        If Trim(Command()) = "" Then
            ' MsgBox("Use command line arguments : " & vbCr & vbCr & "-xml path to xml file" & vbCr & "-s server" & vbCr & "-d database" & vbCr & "-uid username (if omitted Trusted_Connection=true)" & vbCr & "-pwd password" & vbCr & "-u application user" & vbCr & "-p application password" & vbCr, MsgBoxStyle.Information, "NetGen")
        End If

        strStartupArguments = System.Environment.GetCommandLineArgs

        For intCount = 0 To UBound(strStartupArguments)
            Select Case strStartupArguments(intCount).ToLower
                Case "-xml"
                    strXML2 = strStartupArguments(intCount + 1).Replace("""", "")
                Case "-s"
                    cls1.strServer = strStartupArguments(intCount + 1)
                Case "-d"
                    cls1.strBaza = strStartupArguments(intCount + 1)
                Case "-uid"
                    cls1.strUser = strStartupArguments(intCount + 1)
                Case "-pwd"
                    cls1.strPassword = strStartupArguments(intCount + 1)
                Case "-u"
                    cls1.strAppUser = strStartupArguments(intCount + 1)
                Case "-p"
                    cls1.strAppPass = strStartupArguments(intCount + 1)
                Case "-o"
                    cls1.OperateriID = strStartupArguments(intCount + 1)
                Case "-w"
                    cls1.strWebService = strStartupArguments(intCount + 1)
                Case "-l"
                    Language = strStartupArguments(intCount + 1)
                Case "-y"
                    cls1.strProxy = strStartupArguments(intCount + 1)
            End Select

        Next intCount

        ' samo za potrebe debugiranja exe-a bez parametara:

        'If UBound(strStartupArguments) = 0 Then





        'strXML2 = Application.StartupPath
        'End If
        If strXML2 = "" Then strXML2 = Application.StartupPath ' "\\Spin-sql2005\JupiterSrv\SharedDLLs\xml"

        'MsgBox(Application.StartupPath)

        'If cls1.strServer = "" Then cls1.strServer = "Jupiter" 'LoginForm1.ServerTextBox.Text 
        'If cls1.strBaza = "" Then cls1.strBaza = "pzosatina" 'LoginForm1.DatabaseTextBox.Text 
        'If cls1.strUser = "" Then cls1.strUser = "sa" 'LoginForm1.dbUserNameTextBox.Text '
        'If cls1.strPassword = "" Then cls1.strPassword = "" 'LoginForm1.dbPasswordTextBox.Text 
        'If cls1.strAppUser = "" Then cls1.strAppUser = "Spin" 'LoginForm1.UsernameTextBox.Text 
        'If cls1.strAppPass = "" Then cls1.strAppPass = "s" 'LoginForm1.PasswordTextBox.Text 

        ''If strXML2 = "" Then strXML2 = InputBox("XML ?", "XML", "E:\My Documents\TotalGen2007exe\")
        ''If cls1.strServer = "" Then cls1.strServer = InputBox("Server ?", "Server", "Jupiter")
        ''If cls1.strBaza = "" Then cls1.strBaza = InputBox("Database ?", "Database", "Velekem")
        ' ''If cls1.strUser = "" Then cls1.strUser = InputBox("DB user ?", "DB user", "Sa")
        ' ''If strPassword = "" Then strPassword = InputBox("Password ?", "Password", "")
        ''If cls1.strAppUser = "" Then cls1.strAppUser = InputBox("Application user ?", "Application user", "Spin")
        ''If cls1.strAppPass = "" Then cls1.strAppPass = InputBox(cls1.strAppUser & " , application password ?", cls1.strAppUser & " , application password", "s")

        Me.Text = strXML2 & " .. " & cls1.strServer & " .. " & cls1.strBaza & " .. " & cls1.strAppUser
    End Sub

    Sub DodatnoFormatirajGridnakonRetriveStructure(ByVal GridEX12 As Janus.Windows.GridEX.GridEX)
        On Error Resume Next
        Dim i As Integer
        Dim j As Integer

        If GridEX12.RowCount > 0 Then
            For j = 0 To GridEX12.Tables.Count
                For i = 0 To GridEX12.Tables(j).Columns.Count - 1
                    If InStr(GridEX12.Tables(j).Columns(i).Type.ToString.ToLower, "int") Then
                        GridEX12.Tables(j).Columns(i).Width = 34
                        GridEX12.Tables(j).Columns(i).FormatString = "##0"
                        GridEX12.Tables(j).Columns(i).TextAlignment = TextAlignment.Far
                        GridEX12.Tables(j).Columns(i).HeaderAlignment = TextAlignment.Far
                    End If

                    If InStr(GridEX12.Tables(j).Columns(i).Type.ToString.ToLower, "decimal") Or InStr(GridEX12.Tables(j).Columns(i).Type.ToString.ToLower, "double") Then
                        GridEX12.Tables(j).Columns(i).Width = 44
                        GridEX12.Tables(j).Columns(i).FormatString = "#,##0.00"
                        GridEX12.Tables(j).Columns(i).TextAlignment = TextAlignment.Far
                        GridEX12.Tables(j).Columns(i).HeaderAlignment = TextAlignment.Far
                    End If

                    If InStr(GridEX12.Tables(j).Columns(i).Type.ToString.ToLower, "string") Then
                        GridEX12.Tables(j).Columns(i).Width = 200
                    End If

                    If InStr(GridEX12.Tables(j).Columns(i).Type.ToString.ToLower, "date") Then
                        GridEX12.Tables(j).Columns(i).Width = 100
                        GridEX12.Tables(j).Columns(i).FormatString = "dd.MM.yyyy"
                    End If

                    GridEX12.Tables(j).Columns(i).EditType = EditType.NoEdit
                    If Val(GridEX12.Tables(j).Columns(i).Caption.Trim.Substring(GridEX12.Tables(j).Columns(i).Caption.Trim.Length - 4, 4)) <> 0 Then
                        GridEX12.Tables(j).Columns(i).Width = Val(GridEX12.Tables(j).Columns(i).Caption.Trim.Substring(GridEX12.Tables(j).Columns(i).Caption.Trim.Length - 4, 4)) / FakPretvorbe
                        GridEX12.Tables(j).Columns(i).Caption = GridEX12.Tables(j).Columns(i).Caption.Trim.Substring(0, GridEX12.Tables(j).Columns(i).Caption.Trim.Length - 4)
                    End If
                    'GridEX12.Tables(j).Columns(i).Caption = Translate(GridEX12.Tables(j).Columns(i).Caption).Replace(";", vbCrLf)
                    GridEX12.Tables(j).Columns(i).Caption = GridEX12.Tables(j).Columns(i).Caption.Replace(";", vbCrLf)
                    GridEX12.Tables(j).Columns(i).AllowSize = True
                    If GridEX12.Tables(j).Columns(i).Caption.ToUpper.Substring(GridEX12.Tables(j).Columns(i).Caption.Length - 2, 2) = "ID" Then
                        GridEX12.Tables(j).Columns(i).Visible = False
                    End If

                Next
            Next
        End If
    End Sub

    Sub PokaziUpit(ByVal sql As String)
        Timer1.Enabled = False

        PunimMasterGrid = True

        If Trim(sql) = "" Then Exit Sub
        'GridEX1.Visible = True


        strZadnjiSQLsakojimjepunjenGrid = sql
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Dim tID As String = ""
        tID = GetID()
        ID = tID

        If tID <> IDzaNoviSlog Then ' .... 12.01.2009. ovo je pokušaj pametnog pozicioniranja na masteru na novi slog ako se pojavi
            If IDzaNoviSlog <> "" Then
                If IDzaNoviSlog <> "0" Then
                    tID = IDzaNoviSlog
                    ID = tID
                End If
            End If
        End If

        cls1.MasterID = tID

        'DataGridView1.DataSource = cls1.ExecSql(sql)

        ' 25.11.2008
        sql = cls1.SrediSQLPosljeNextForme(sql)
        '--------------------------------------------

        Dim Trs As New DataTable
        Trs = cls1.ExecSql(sql)

        If DXGrid.Trim.ToLower = "true" Then
            TopRowIndex = GridView1.TopRowIndex
            gFocusedRowHandle = GridView1.FocusedRowHandle

            cls1.NapuniDXGrid(Trs, GridControl1, GridView1, strEditColumns)

            GridView1.FocusedRowHandle = gFocusedRowHandle
            'GridView1.FocusedRowHandle = gFocusedRowHandle + 1
            GridView1.TopRowIndex = TopRowIndex

            cls1.CustomFormatKolonaDX(GridView1, m_nodeListZaCustomFormat, strCLEARCOLUMS, strHeaderAlignment)

            GridControl1.Dock = DockStyle.Fill
            GridControl1.Visible = True
            GridEX1.Visible = False
            'GridView1.OptionsBehavior.Editable = False
            ID = GetID()
            cls1.MasterID = ID
            PunimMasterGrid = False

            ID = GetID()
            cls1.MasterID = ID

            PunimMasterGrid = False

            If cls1.strStartupStoraDetalj.ToString.Trim <> "" Then
                If cls1.strStartupStoraDetalj.ToString.Substring(0, 1) = ":" Then
                    MsgBox("sorkac!") 'NovaGenerickaForma(strStartupStora.Replace(":", ""))
                Else
                    AktivnaFormaDetalj = AktivnaFormaMaster
                    PokaziUpitDetalj(cls1.strStartupStoraDetalj)
                End If
            Else
                On Error Resume Next
                SplitContainer2.SplitterDistance = Me.Height - 50
            End If

            If strCHECKBOXVISIBLE = "FALSE" Then
                GridView1.Columns("rcb555").Visible = False
            Else
                GridView1.Columns("rcb555").Visible = True
            End If

            Exit Sub
        Else
            GridEX1.Dock = 5
            GridControl1.Visible = False
            GridEX1.Visible = True
        End If

        'GridEX1.BoundMode = BoundMode.Unbound

        ToolStripProgressBar1.ForeColor = Color.LightGreen
        cls1.UpaliProgres()

        If strRelacija.Trim.ToLower = "self" Then
            'Dim ds As New System.Data.DataSet()
            'Dim Trs2 As DataTable = Trs.Clone
            'Dim Trs3 As DataTable = Trs.Clone
            'Trs2.Merge(Trs)
            'Trs3.Merge(Trs)
            'ds.Tables.Add(Trs2)
            'ds.Tables.Add(Trs3)
            'ds.Relations.Clear()
            'ds.Relations.Add(TREE_RELATIONSHIP, ds.Tables(0).Columns(0), ds.Tables(0).Columns(1))
            'cls1.CreateGridColumns(ds.Tables(0), GridEX1)
            'cls1.AddConditionalFormatting(GridEX1)
            'GridEX1.Hierarchical = True
            'GridEX1.Hierarchical = True
            'GridEX1.SetDataBinding(ds, "qwer")
            'cls1.CreateGridColumns(Trs, GridEX1)
            'cls1.AddConditionalFormatting(GridEX1)

            GridEX1.SetDataBinding(Trs, "qwer")
            GridEX1.RetrieveStructure()
            DodatnoFormatirajGridnakonRetriveStructure(GridEX1)
            cls1.AddConditionalFormatting(GridEX1)
            GridEX1.EditMode = EditMode.EditOff
            GridEX1.RootTable.HierarchicalMode = HierarchicalMode.SelfReferencing
            GridEX1.RootTable.SelfReferencingSettings.ParentDataMember = "id"
            GridEX1.RootTable.SelfReferencingSettings.ChildDataMember = "parentid"
            GridEX1.RootTable.SelfReferencingSettings.TreatOrphanRowsAsRoot = True
            GridEX1.RootTable.SelfReferencingSettings.AggregationMode = SelfReferencingAggregationMode.AllRows
        ElseIf strRelacija.Trim.ToLower = "multi" Then
            Dim ds As New System.Data.DataSet()
            Dim Trs2 As DataTable = Trs.Clone
            Trs2.Merge(Trs)
            ds.Tables.Add(Trs2)

            If strSQL2.Trim <> "" Then
                Dim Trs3 As DataTable
                Trs3 = cls1.ExecSql(strSQL2, "t2")
                Dim Trs4 As DataTable = Trs3.Clone
                Trs4.Merge(Trs3)
                ds.Tables.Add(Trs4)
            End If

            If strSQL3.Trim <> "" Then
                Dim Trs5 As DataTable
                Trs5 = cls1.ExecSql(strSQL3, "t3")
                Dim Trs6 As DataTable = Trs5.Clone
                Trs6.Merge(Trs5)
                ds.Tables.Add(Trs6)
            End If

            If strSQL4.Trim <> "" Then
                Dim Trs7 As DataTable
                Trs7 = cls1.ExecSql(strSQL4, "t4")
                Dim Trs8 As DataTable = Trs7.Clone
                Trs8.Merge(Trs7)
                ds.Tables.Add(Trs8)
            End If

            ds.Relations.Clear()
            GridEX1.Hierarchical = True
            If strSQL2.Trim <> "" Then ds.Relations.Add(TREE_RELATIONSHIP, ds.Tables(0).Columns(0), ds.Tables(1).Columns(1))
            If strSQL3.Trim <> "" Then ds.Relations.Add("3", ds.Tables(1).Columns(0), ds.Tables(2).Columns(1))
            If strSQL4.Trim <> "" Then ds.Relations.Add("4", ds.Tables(2).Columns(0), ds.Tables(3).Columns(1))
            GridEX1.SetDataBinding(ds, "qwer")
            GridEX1.RetrieveStructure(True)
            DodatnoFormatirajGridnakonRetriveStructure(GridEX1)
            cls1.AddConditionalFormatting(GridEX1)
        Else

            If MultiGridLayout Then
                cls1.CreateGridColumns(Trs, GridEX1, strEditColumns)
                cls1.AddConditionalFormatting(GridEX1)
               

            Else
                '  If GridEX1.RootTable Is Nothing Then ' KK ??????
                cls1.CreateGridColumns(Trs, GridEX1, strEditColumns)
                cls1.AddConditionalFormatting(GridEX1)


                'End If
            End If

            GridEX1.SetDataBinding(Trs, "qwer")
        End If


        CustomFormatKolona(GridEX1, m_nodeListZaCustomFormat, strCLEARCOLUMS, strHeaderAlignment)

        'GridEX1.RetrieveStructure()

        If Not ResetGridLayout Then
            'LoadLayoutTEMPORARNI()
            LoadLayout()
        End If





        PrevediSadrzajGrida(GridEX1)

        PrevediSadrzajGrida2(GridEX1, strTranslateColumns)


        '' uncheck 
        'Dim row As Janus.Windows.GridEX.GridEXRow
        'Dim dataRows() As Janus.Windows.GridEX.GridEXRow
        'dataRows = GridEX1.GetDataRows
        'For Each row In dataRows
        '    GridEX1.MoveToRowIndex(row.RowIndex)
        '    GridEX1.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Unchecked
        'Next



        'End If

        Windows.Forms.Cursor.Current = Cursors.Default

        PokaziBrojStavki()

        'ToolStripStatusLabel2.ToolTipText = ToolStripStatusLabel2.Text
        'cls1.FormatirajGrid(DataGridView1)
        cls1.UgasiProgres()


        If strCHECKBOXVISIBLE = "FALSE" Then
            GridEX1.RootTable.Columns(1).Visible = False
        Else
            GridEX1.RootTable.Columns(1).Visible = True
        End If

        If strTOTALROWVISIBLE = "FALSE" Then
            GridEX1.TotalRow = Janus.Data.InheritableBoolean.False
        Else
            GridEX1.TotalRow = Janus.Data.InheritableBoolean.True
        End If

        Dim c4 As GridEXColumn = GridEX1.RootTable.Columns(2)



        If GridEX1.RowCount > 0 Then
            'c4.Column = GridEX1.RootTable.Columns(2)
            If GridEX1.Find(c4, ConditionOperator.Equal, ID, -1, 1) = False Then
                If Not tID Is Nothing Then
                    If GridEX1.RowCount > 0 Then
                        'c4.Column = GridEX1.RootTable.Columns(2)
                        GridEX1.Find(c4, ConditionOperator.Equal, tID, -1, 1)
                        GridEX1.Focus()
                        GridEX1.EnsureVisible()
                    End If
                End If
            End If
            GridEX1.Focus()
            GridEX1.EnsureVisible()
        End If



        '----------------------------------------------- pozicioniranje u mastergridu
        On Error Resume Next
        If GridEX1.RowCount > 0 Then
            Dim c5 As GridEXColumn = GridEX1.RootTable.Columns("id")
            If Err.Number = 0 Then
                Err.Clear()
                GridEX1.Find(c5, ConditionOperator.Equal, GridEX1.CurrentRow.Cells("movetoid").Value.ToString.Trim, 0, 1)
                If Err.Number = 0 Then
                    GridEX1.EnsureVisible()
                End If
            End If
        End If

        On Error Resume Next
        If cls1.strMasterMoveLast.ToLower = "true" Then
            GridEX1.MoveLast()
            GridEX1.EnsureVisible()
        End If
        '--------------------------------------------------------------------------------------------- 



        ID = GetID()
        cls1.MasterID = ID

        PunimMasterGrid = False

        If cls1.strStartupStoraDetalj.ToString.Trim <> "" Then
            If cls1.strStartupStoraDetalj.ToString.Substring(0, 1) = ":" Then
                MsgBox("sorkac!") 'NovaGenerickaForma(strStartupStora.Replace(":", ""))
            Else
                AktivnaFormaDetalj = AktivnaFormaMaster
                PokaziUpitDetalj(cls1.strStartupStoraDetalj)
            End If
        Else
            On Error Resume Next
            SplitContainer2.SplitterDistance = Me.Height - 50
        End If

        TimerOn()

        '' orignail sa kojim je radio godinama
        ''------------------------------------------------ totali
        'Static gi As Boolean
        'If gi = False Then
        '    gi = True
        '    Dim groupTotal As New GridEXGroupHeaderTotal()
        '    groupTotal.AggregateFunction = AggregateFunction.Count
        '    groupTotal.TotalPrefix = "["
        '    groupTotal.TotalSuffix = " ]"
        '    GridEX1.RootTable.GroupHeaderTotals.Add(groupTotal)

        'End If
        ''------------------------------------------------ totali



        ' 9.3.2022. nakon Logarinog zahtjeva neki pokušaj
        '------------------------------------------------ totali
        On Error Resume Next
        Dim groupTotal As New GridEXGroupHeaderTotal()
        groupTotal.AggregateFunction = AggregateFunction.Count
        groupTotal.TotalPrefix = "["
        groupTotal.TotalSuffix = " ]"
        GridEX1.RootTable.GroupHeaderTotals.Clear()
        GridEX1.RootTable.GroupHeaderTotals.Add(groupTotal)


        '------------------------------------------------ totali





        '------------------------------------------------ defoltno grupiranje ako ime polja počinje sa !
        If strKoloneZaGrupiranje.Trim <> "" Then
            Dim ar4() As String
            Dim brojkolone As Integer
            ar4 = strKoloneZaGrupiranje.Split("~")
            GridEX1.RootTable.Groups.Clear()
            For j = 0 To ar4.GetUpperBound(0)
                If ar4(j).Trim <> "" Then
                    Dim group As New Janus.Windows.GridEX.GridEXGroup
                    brojkolone = ar4(j) + 2
                    group.Column = GridEX1.RootTable.Columns(brojkolone)
                    GridEX1.RootTable.Groups.Add(group)
                End If
            Next
        End If



        If strFilterMode.ToLower.Trim = "true" Then

            Dim ii As Integer
            For ii = 1 To GridEX1.RootTable.Columns.Count - 1
                GridEX1.RootTable.Columns(ii).EditType = EditType.TextBox
            Next

            GridEX1.FilterMode = FilterMode.Automatic
            'Glbl.EditMode = EditMode.EditOn
            If strEditColumns = "" Then GridEX1.AllowEdit = InheritableBoolean.False
        End If





    End Sub



    Sub PrevediSadrzajGrida2(ByVal GridEX1 As Janus.Windows.GridEX.GridEX, ByVal strTranslateColumns As String)
        On Error Resume Next
        If strTranslateColumns.Trim = "" Then Exit Sub
        strTranslateColumns = "," & strTranslateColumns & ","
        PrijevodjeutijekU = True
        Dim RA2 As Integer = GridEX1.CurrentRow.AbsolutePosition

        Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
        checkedRows = GridEX1.GetDataRows
        If checkedRows.Length > 0 Then
            Dim row As Janus.Windows.GridEX.GridEXRow
            For Each row In checkedRows
                GridEX1.MoveToRowIndex(row.RowIndex)
                For i = 1 To GridEX1.RootTable.Columns.Count - 1
                    Dim strImeParametra As String = GridEX1.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                    If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                        strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                    End If

                    If strTranslateColumns.Trim.ToLower.Contains("," & strImeParametra.Trim.ToLower & ",") Then
                        If GridEX1.CurrentRow.Cells(i).Value Is Nothing Or GridEX1.CurrentRow.Cells(i).Value Is DBNull.Value Then

                        Else
                            If IsNumeric(GridEX1.CurrentRow.Cells(i).Value.ToString) Then

                            Else
                                GridEX1.CurrentRow.Cells(i).Value = Translate(GridEX1.CurrentRow.Cells(i).Value.ToString.Trim)
                            End If
                        End If
                    End If

                Next
            Next
        End If

        PrijevodjeutijekU = False

        GridEX1.MoveToRowIndex(RA2)

    End Sub



    Sub PokaziBrojStavki()
        Dim BrojMstavki As Long = 0
        Dim BrojDstavki As Long = 0
        On Error Resume Next
        If GridEX1.TotalRow = InheritableBoolean.True Then
            BrojMstavki = GridEX1.RowCount - 1
        Else
            BrojMstavki = GridEX1.RowCount
        End If



        If GridEX2.TotalRow = InheritableBoolean.True Then
            BrojDstavki = GridEX2.RowCount - 1
        Else
            BrojDstavki = GridEX2.RowCount
        End If

        If BrojMstavki < 0 Then BrojMstavki = 0
        If BrojDstavki < 0 Then BrojDstavki = 0

        ToolStripStatusLabel2.Text = Translate("Pronađeno") & " : " & BrojMstavki & " / " & BrojDstavki & " " & "  " & strOpisniParametri

        TabPage3.Text = "Stavke (" & BrojDstavki & ")"

    End Sub
    Sub PokaziUpitDetalj(ByVal sql As String)

        'Static ZadnjSqlzaPunjenjeDetGrida As String
        'sql = sql.Trim

        'If sql <> "" Then
        '    ZadnjSqlzaPunjenjeDetGrida = sql
        'Else
        '    sql = ZadnjSqlzaPunjenjeDetGrida
        'End If


        If PunimMasterGrid Then Exit Sub

        If UtjekuJeRucnoCheckiranjeRedova Then Exit Sub

        If Trim(sql) = "" Then
            SplitContainer2.SplitterDistance = Me.Height - 50
            Exit Sub
        End If

        'GridEX2.Visible = True
        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        'DataGridView1.DataSource = cls1.ExecSql(sql)


        'If RadimoPrekoHTTPservera = True Then
        '    Dim D As localhost.Service = New localhost.Service
        '    Dim MyData As DataSet = D.HelloWorld(sql, cls1.Conn.ConnectionString)
        '    DataGridView1.DataSource = MyData.Tables("Dataset").DefaultView
        'Else

        DetID = 0
        cls1.DetIDIzKlase = 0

        Dim Trs As New DataTable
        ID = GetID()
        cls1.MasterID = ID

        Dim tID As String = ""
        tID = GetDetID()

        ' dodao 1.12.208.
        PokupiParametreSaGrida()

        ' sql = SrediSQL(sql).Trim
        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(sql).Trim, ParametriSaGrida)

        Trs = cls1.ExecSql(sql)

        If DXGrid.Trim.ToLower = "true" Then
            cls1.NapuniDXGrid(Trs, GridControl2, GridView2, strEditColumnsDET)
            GridControl2.Dock = DockStyle.Fill
            GridControl2.Visible = True
            GridEX2.Visible = False
            DetID = GetDetID()
            cls1.DetIDIzKlase = DetID
            Windows.Forms.Cursor.Current = Cursors.Default
            Exit Sub
        Else
            GridEX2.Dock = 5
            GridControl2.Visible = False
            GridEX2.Visible = True
        End If


        'GridEX1.BoundMode = BoundMode.Unbound

        ToolStripProgressBar1.ForeColor = Color.LightGreen
        cls1.UpaliProgres()


        If MultiGridLayout Then
            cls1.CreateGridColumns(Trs, GridEX2, strEditColumnsDET)
            cls1.AddConditionalFormatting(GridEX2)
        Else
            If GridEX2.RootTable Is Nothing Then
                cls1.CreateGridColumns(Trs, GridEX2, strEditColumnsDET)
                cls1.AddConditionalFormatting(GridEX2)
            End If
        End If




        GridEX2.SetDataBinding(Trs, "qwer")

        'GridEX1.RetrieveStructure()

        CustomFormatKolona(GridEX2, m_nodeListZaCustomFormatDET, strCLEARCOLUMSdet, strHeaderAlignmentdet)

        If Not ResetGridLayout Then LoadLayout2()



        If Not tID Is Nothing Then
            If GridEX2.RowCount > 0 Then
                Dim c4 As GridEXColumn = GridEX2.RootTable.Columns(2)
                'c4.Column = GridEX1.RootTable.Columns(2)
                GridEX2.Find(c4, ConditionOperator.Equal, tID, 0, 1)
                'GridEX2.Focus()
                GridEX2.EnsureVisible()
            End If
        End If



        '----------------------------------------------- pozicioniranje u detgridu
        On Error Resume Next
        If GridEX2.RowCount > 0 Then
            Dim c4 As GridEXColumn = GridEX2.RootTable.Columns("id")
            If Err.Number = 0 Then
                Err.Clear()
                GridEX2.Find(c4, ConditionOperator.Equal, GridEX2.CurrentRow.Cells("movetoid").Value.ToString.Trim, 0, 1)
                If Err.Number = 0 Then
                    GridEX2.EnsureVisible()
                End If
            End If
        End If

        On Error Resume Next
        If cls1.strDetMoveLast.ToLower = "true" Then
            GridEX2.MoveLast()
            GridEX2.EnsureVisible()
        End If
        '--------------------------------------------------------------------------------------------- 



        'End If

        DetID = GetDetID()
        cls1.DetIDIzKlase = DetID

        Windows.Forms.Cursor.Current = Cursors.Default
        'ToolStripStatusLabel2.Text = "Found : " & GridEX1.RowCount & " item(s)." & "  " & strOpisniParametri
        'ToolStripStatusLabel2.ToolTipText = ToolStripStatusLabel2.Text
        'cls1.FormatirajGrid(DataGridView1)
        cls1.UgasiProgres()

        PokaziBrojStavki()

        On Error Resume Next

        If strDetCHECKBOXVISIBLE = "FALSE" Then
            GridEX2.RootTable.Columns(1).Visible = False
        Else
            GridEX2.RootTable.Columns(1).Visible = True
        End If

        If strDetTOTALROWVISIBLE = "FALSE" Then
            GridEX2.TotalRow = Janus.Data.InheritableBoolean.False
        Else
            GridEX2.TotalRow = Janus.Data.InheritableBoolean.True
        End If

        PrevediSadrzajGrida(GridEX2)
        PrevediSadrzajGrida2(GridEX2, strTranslateDETColumns)


        If strFilterModeDet.ToLower.Trim = "true" Then

            Dim ii As Integer
            For ii = 1 To GridEX2.RootTable.Columns.Count - 1
                GridEX2.RootTable.Columns(ii).EditType = EditType.TextBox
            Next

            GridEX2.FilterMode = FilterMode.Automatic
            'Glbl.EditMode = EditMode.EditOn
            If strEditColumns = "" Then GridEX2.AllowEdit = InheritableBoolean.False

        End If

        TabControl2.Visible = True

        If cls1.strDMSDet.Trim <> "" Then
            DMS_pOkazi_Upit()
        Else
            TabControl2.TabPages.Remove(TabPage4)
        End If


        '--- 29.12.2021. KK 
        Conn.Close()
        Conn.Open()


    End Sub

    Sub DMS_pOkazi_Upit()

        TabPage4.Text = "DMS (0)"

        Dim Sql As String = cls1.SrediSQLPosljeNextForme(SrediSQL(cls1.strDMSDet).Trim, ParametriSaGrida)
        Dim Trs As DataTable = cls1.ExecSql(Sql)

        If GridEX3.RootTable Is Nothing Then
            cls1.CreateGridColumns(Trs, GridEX3, "")
            cls1.AddConditionalFormatting(GridEX3)
        End If

        GridEX3.SetDataBinding(Trs, "qwer")
        GridEX3.RootTable.Columns(1).Visible = False
        GridEX3.Visible = True

        TabPage4.Text = "DMS (" & Trs.Rows.Count & ")"

    End Sub




    Function SrediSQL(ByVal sql As String) As String

        On Error Resume Next
        Dim ImeAktivneTabliceuGridu As String = "t1"

        If GridEX1.Hierarchical Then
            If GridEX1.CurrentTable.Caption.ToLower = "qwer" Then ImeAktivneTabliceuGridu = "T1"
            If GridEX1.CurrentTable.Caption.ToLower = "t2" Then ImeAktivneTabliceuGridu = "T2"
            If GridEX1.CurrentTable.Caption.ToLower = "t3" Then ImeAktivneTabliceuGridu = "T3"
            If GridEX1.CurrentTable.Caption.ToLower = "t4" Then ImeAktivneTabliceuGridu = "T4"
        End If

        sql = sql.Replace("@@Id", ID).Replace("@@ID", ID).Replace("@@id", ID).Replace("@@operateriid", cls1.OperateriID).Replace("@@OPERATERIID", cls1.OperateriID).Replace("@@Operateriid", cls1.OperateriID).Replace("@@DETID", DetID).Replace("@@detid", DetID).Replace("@@DetId", DetID).Replace("@@node", Node).Replace("@@NODE", Node).Replace("@@table", ImeAktivneTabliceuGridu).Replace("@@TABLE", ImeAktivneTabliceuGridu)

        If sql.ToLower.Contains("@@xmlname") Then
            sql = sql.Replace("@@xmlname", strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\")))
            sql = sql.Replace("@@XMLNAME", strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\")))
        End If

        If sql.ToLower.Contains("@@jupitersrvpath") Then
            sql = sql.Replace("@@jupitersrvpath", MOJremotedllpath)
        End If


        If TrenutnaKolonaDX <> "" Then
            sql = sql.Replace("@@editedcolumn", TrenutnaKolonaDX).Replace("@@EDITEDCOLUMN", TrenutnaKolonaDX)
        Else
            sql = sql.Replace("@@editedcolumn", TrenutnaKolona.Caption).Replace("@@EDITEDCOLUMN", TrenutnaKolona.Caption)
        End If

        sql = sql.Replace("@@lastvalue", TrenutnaVrijednost).Replace("@@LASTVALUE", TrenutnaVrijednost)
        sql = sql.Replace("@@selectedcolor", strIzabranaBoja).Replace("@@SELECTEDCOLOR", strIzabranaBoja)

        SrediSQL = sql

    End Function



    Private Function GetLayoutDirectory() As String
        'Dim currentDirectory As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(Application.ExecutablePath).Parent
        'While Not (currentDirectory Is Nothing)

        '    Dim childDirectories As System.IO.DirectoryInfo() = currentDirectory.GetDirectories()
        '    Dim childDir As System.IO.DirectoryInfo

        '    For Each childDir In childDirectories

        '        If (childDir.Name = "LayoutData") Then
        '            Return childDir.FullName
        '        End If
        '    Next
        '    currentDirectory = currentDirectory.Parent
        'End While
        'Return ""


        Dim YourPath As String = ""
        YourPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\SPIN Informatica\Jupiter Software\Grid Settings\"

        If (Not System.IO.Directory.Exists(YourPath)) Then
            System.IO.Directory.CreateDirectory(YourPath)
        End If

        GetLayoutDirectory = YourPath


        'GetLayoutDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\SPIN Informatica\Jupiter Software\Grid Settings\"

        'cls1.NapuniCBLog("GetLayoutDirectory : " & GetLayoutDirectory)


    End Function

    Sub PokupiParametreSaGrida()

        ParametriSaGrida = ""

        If GridEX1.Hierarchical Then
            PokupiParametreSaHijerarHijskogGrida()
            Exit Sub
        End If

        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        If GridEX1.RowCount > 0 Then
            For i = 1 To GridEX1.RootTable.Columns.Count - 1
                If GridEX1.CurrentRow.Cells(i).Column.DataMember.ToString <> "" Then
                    If GridEX1.CurrentRow.Cells(i).Value Is Nothing Or GridEX1.CurrentRow.Cells(i).Value Is DBNull.Value Then

                    Else


                        Dim strImeParametra As String = GridEX1.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If


                        If IsNumeric(GridEX1.CurrentRow.Cells(i).Value.ToString) Then
                            'ParametriSaGrida = ParametriSaGrida & "@" & GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            'ParametriSaGrida = ParametriSaGrida & "@" & GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString & "' , "
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString & "' , "
                        End If

                    End If
                End If
            Next
        Else
            ParametriSaGrida = ""
        End If


        If GridEX2.RowCount > 0 And ParametriSaGrida <> "" Then
            For i = 1 To GridEX2.RootTable.Columns.Count - 1
                If GridEX2.CurrentRow.Cells(i).Column.DataMember.ToString <> "" Then
                    If GridEX2.CurrentRow.Cells(i).Value Is Nothing Or GridEX2.CurrentRow.Cells(i).Value Is DBNull.Value Then

                    Else

                        Dim strImeParametra As String = GridEX2.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If IsNumeric(GridEX2.CurrentRow.Cells(i).Value.ToString) Then
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & GridEX2.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & GridEX2.CurrentRow.Cells(i).Value.ToString & "' , "
                        End If

                    End If
                End If
            Next
        Else
            ' ParametriSaGrida = ""
        End If


    End Sub


    Sub PokupiParametreSaHijerarHijskogGrida()
        'Print(Tctl.Tables(Tctl.CurrentTable.Caption).Columns.Count)
        On Error Resume Next
        Dim i As Integer
        Dim IndexTablica As Integer = 1
        ParametriSaGrida = ""
        If GridEX1.RowCount > 0 Then

            If GridEX1.CurrentTable.Caption.ToLower = "qwer" Then IndexTablica = 1
            If GridEX1.CurrentTable.Caption.ToLower = "t2" Then IndexTablica = 2
            If GridEX1.CurrentTable.Caption.ToLower = "t3" Then IndexTablica = 3
            If GridEX1.CurrentTable.Caption.ToLower = "t4" Then IndexTablica = 4

            For i = 0 To GridEX1.Tables(IndexTablica - 1).Columns.Count - 1
                If GridEX1.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
                    If GridEX1.CurrentRow.Cells(i).Value Is Nothing Or GridEX1.CurrentRow.Cells(i).Value Is DBNull.Value Then

                    Else

                        If IsNumeric(GridEX1.CurrentRow.Cells(i).Value.ToString) Then
                            ParametriSaGrida = ParametriSaGrida & "@" & GridEX1.CurrentRow.Cells(i).Column.Caption.ToString & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".").Trim & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & GridEX1.CurrentRow.Cells(i).Column.Caption.ToString & "='" & GridEX1.CurrentRow.Cells(i).Value.ToString.Trim & "' , "
                        End If

                    End If
                End If
            Next
        End If

        'If GridEX2.RowCount > 0 Then
        '    For i = 1 To GridEX2.Tables(GridEX1.CurrentTable.Caption).Columns.Count - 1
        '        If GridEX2.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
        '            If GridEX2.CurrentRow.Cells(i).Value Is Nothing Then

        '            Else
        '                If IsNumeric(GridEX2.CurrentRow.Cells(i).Column.Caption.ToString) Then
        '                    ParametriSaGrida = ParametriSaGrida & "@" & GridEX2.CurrentRow.Cells(i).Column.Caption.ToString & "='" & GridEX2.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
        '                Else
        '                    ParametriSaGrida = ParametriSaGrida & "@" & GridEX2.CurrentRow.Cells(i).Column.Caption.ToString & "='" & GridEX2.CurrentRow.Cells(i).Value.ToString & "' , "
        '                End If

        '            End If
        '        End If
        '    Next
        'End If


    End Sub

    Function ssPermissionProvjeriRafalnuKomandu(ByVal strKontrola As String) As Boolean
        On Error Resume Next
        ssPermissionProvjeriRafalnuKomandu = True
        strKontrola = strKontrola.Replace(":", "")
        If Not trsPermisio Is Nothing Then
            If trsPermisio.Rows.Count > 0 Then
                Dim r As DataRow
                For Each r In trsPermisio.Rows

                    If r("functionname").ToString.Trim.ToLower = strKontrola.Trim.ToLower Then

                        If r("smije") = False Then
                            ssPermissionProvjeriRafalnuKomandu = False
                        End If

                        ' kk 7.11.2018.
                        If r("smije") = True Then
                            ssPermissionProvjeriRafalnuKomandu = True
                            Exit For 'kk 21.02.2022 ako je bilo koji red true , bilo za osobu ili za grupu izađi van - sve je OK
                        End If

                    End If


                Next
            End If
        End If
    End Function

    Sub ssPermissionProcitaj()
        On Error Resume Next
        Dim sql As String
        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))
        imeXMLa = imeXMLa.Trim

        sql = "select * from sspermission where objectname='" & imeXMLa & "' and operateriid=" & cls1.OperateriID

        ' kk 26.10.2018. izmjenio sort u upitu za permissione tako da bez obzira na deny na grupi ako ipak operater ima prava na nešto onda to ima prednost
        ' sql = "select *,1 as sort from ssPermission where objectname='" & imeXMLa & "' and ((OperateriID = " & cls1.OperateriID & ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " & cls1.OperateriID & ") )) union select *,2 as sort from ssPermission where objectname='" & imeXMLa & "' and OperateriID = 0 and grupeid=0 order by sort desc,isgrupa desc,grupeid,operateriid"

        ' 18.10.2021. kk  , Kasalo trazio da izbacim desc
        sql = "select *,1 as sort from ssPermission where objectname='" & imeXMLa & "' and ((OperateriID = " & cls1.OperateriID & ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " & cls1.OperateriID & ") )) union select *,2 as sort from ssPermission where objectname='" & imeXMLa & "' and OperateriID = 0 and grupeid=0 order by sort ,isgrupa desc,grupeid,operateriid"

        trsPermisio = cls1.ExecSql(sql)


    End Sub

    Sub ssPermissionProvjeri()
        On Error Resume Next

        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

        imeXMLa = imeXMLa.Trim.ToLower


        Dim b1 As Boolean = False
        Dim b2 As Boolean = False
        Dim b3 As Boolean = False
        Dim b4 As Boolean = False
        Dim b5 As Boolean = False
        Dim b6 As Boolean = False
        Dim b7 As Boolean = False
        Dim b8 As Boolean = False
        Dim b9 As Boolean = False
        Dim b10 As Boolean = False


        If Not trsPermisio Is Nothing Then
            If trsPermisio.Rows.Count > 0 Then

                Dim r As DataRow

                For Each r In trsPermisio.Rows

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button1.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b1 = False Then Button1.Enabled = False 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        If r("smije") = True Then
                            Button1.Enabled = True
                            b1 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b1 = False Then
                                    Button1.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If

                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button1.Visible = True
                        End If


                    End If


                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button2.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b2 = False Then Button2.Enabled = False
                        End If

                        If r("smije") = True Then
                            Button2.Enabled = True
                            b2 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b2 = False Then
                                    Button2.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button2.Visible = True
                        End If

                    End If


                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button3.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b3 = False Then Button3.Enabled = False
                        End If

                        If r("smije") = True Then
                            Button3.Enabled = True
                            b3 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then Button3.Visible = False
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then
                                If b3 = False Then
                                    Button3.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If


                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button3.Visible = True
                        End If



                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button4.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b4 = False Then Button4.Enabled = False
                        End If

                        If r("smije") = True Then
                            Button4.Enabled = True
                            b4 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b4 = False Then
                                    Button4.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button4.Visible = True
                        End If

                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button5.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b5 = False Then Button5.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button5.Enabled = True
                            b5 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b5 = False Then
                                    Button5.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button5.Visible = True
                        End If
                    End If


                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button6.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b6 = False Then Button6.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button6.Enabled = True
                            b6 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b6 = False Then
                                    Button6.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button6.Visible = True
                        End If

                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button7.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b7 = False Then Button7.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button7.Enabled = True
                            b7 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b7 = False Then
                                    Button7.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button7.Visible = True
                        End If

                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button8.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b8 = False Then Button8.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button8.Enabled = True
                            b8 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then Button8.Visible = False
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then
                                If b8 = False Then
                                    Button8.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button9.Text.Trim.ToLower.Replace("&", "") Then
                        If r("smije") = False Then
                            If b9 = False Then Button9.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button9.Enabled = True
                            b9 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b9 = False Then
                                    Button9.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button9.Visible = True
                        End If

                    End If

                    If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = Button10.Text.Trim.ToLower.Replace("&", "") Then

                        If r("smije") = False Then
                            If b10 = False Then Button10.Enabled = False
                        End If


                        If r("smije") = True Then
                            Button10.Enabled = True
                            b10 = True 'kk 21.02.2022 ako je bilo koji red digao na true , onda nema nazad na false
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                If b10 = False Then
                                    Button10.Visible = False 'kk 21.02.2022 može ići na false samo ako 
                                End If
                            End If
                        End If

                        Err.Clear()
                        If r("Vidljivost") = True Then
                            If Err.Number = 0 Then Button10.Visible = True
                        End If

                    End If


                Next

            End If
        End If
    End Sub

    Sub ProcitajXMLzaPodlogu()
        cls1.NapuniCBLog(" Idem procitati xml za app podlogu ...")
        Dim strPopUp As String = ""
        Dim strPopUpDet As String = ""
        Dim strXMLsakojimRadim As String = ""
        Dim InterniXML As String = ""
        Dim trs2 As DataTable
        Dim Trs4 As DataTable
        cls1.strXMLsakojimRadimzaCBlog = ""
        'DataGridView1.DataSource = Nothing
        TimerOff()

        'GridEX1.ClearItems()
        GridEX1.ClearStructure()
        GridEX2.ClearStructure()
        GridEX1.Visible = False
        GridEX2.Visible = False

        cls1.MasterID = 0

        If InStr(Trim(strXML2.ToLower), ".xml", CompareMethod.Text) > 0 Then
            strXMLsakojimRadim = strXML2
        Else
            If strXMLfile2 <> "" Then

                If strXMLfile2.StartsWith("BuildinXML") Then
                    InterniXML = strXMLfile2
                Else
                    strXMLsakojimRadim = strXML2 & "\" & strXMLfile2
                    strXMLsakojimRadim = strXMLfile2
                End If

            Else
                Exit Sub
            End If
        End If

        On Error Resume Next
        cls1.strXMLsakojimRadimzaCBlog = strXMLsakojimRadim

        Dim jj As Long
        For jj = 0 To UBound(cls1.CB)
            cls1.CB(jj, 0) = ""
            cls1.CB(jj, 1) = ""
        Next


        If Argument5.Trim <> "" Then
            cls1.CB(cls1.PrviPrazniSlot, 0) = "argument="
            cls1.CB(cls1.PrviPrazniSlot, 1) = Argument5
            cls1.PrviPrazniSlot = cls1.PrviPrazniSlot + 1
        End If

        Button1.Visible = False
        Button2.Visible = False
        Button3.Visible = False
        Button4.Visible = False
        Button5.Visible = False
        Button6.Visible = False

        strPageHeaderRight = ""
        strPageHeaderCenter = ""
        strPageHeaderLeft = ""
        strPageFooterCenter = ""
        strPageFooterLeft = ""
        strPageFooterRight = ""
        strCHECKBOXVISIBLE = ""
        strTOTALROWVISIBLE = ""
        strDetCHECKBOXVISIBLE = ""
        strDetTOTALROWVISIBLE = ""
        strHelpKey = ""
        strHelpKod = ""
        strRelacija = ""
        strONClosed = ""
        strXMLVersion = ""
        DXGrid = ""
        strTranslateColumns = ""
        strTranslateDETColumns = ""

        strEditColumns = ""
        strONCellEditedstr = ""
        strBeforeCellEditedstr = ""


        strEditColumnsDET = ""
        strONCellEditedDETstr = ""
        strBeforeCellEditedDETstr = ""


        Button1.Text = ""
        Button2.Text = ""
        Button3.Text = ""
        Button4.Text = ""
        Button5.Text = ""
        Button6.Text = ""
        strOnEnterKey = ""
        strStartupStora = ""
        cls1.strStartupStoraDetalj = ""
        cls1.strDetMoveLast = ""
        cls1.strMasterMoveLast = ""
        cls1.strDMSDet = ""
        strTreeNodes = ""
        strZadnjiSQLsakojimjepunjenGrid = ""
        strDBlClickForm = ""
        strDBlClickDetForm = ""
        strPopUp = ""
        strPopUpDet = ""
        strTimer = ""
        strMenu = ""
        strSQL2 = ""
        strSQL3 = ""
        strSQL4 = ""
        strCombo = ""
        strComboClick = ""
        strFilterModeDet = ""
        strFilterMode = ""

        'strInclude = ""
        Version1 = ""
        strSplitterDistance = ""
        strREFRESHAFTERCHILD = ""

        Dim strAUTOMATICSORT As String = ""
        Dim strAUTOMATICSORTdet As String = ""

        strPermisio = True

        Dim m_nodelist As Xml.XmlNodeList
        Dim m_node As Xml.XmlNode

        m_xmld = New Xml.XmlDocument()


        Dim a8 As Object
       

        'MsgBox(strXMLsakojimRadim)
        m_xmld.RemoveAll()
        Err.Clear()
        If InterniXML = "" Then
            If InStr(Trim(strXMLsakojimRadim.ToLower), ".xml", CompareMethod.Text) > 0 Then


                '--------------------------------------------------------------------------- XML iz DMS baze 
                a8 = Assembly.Load("SpinDMSUtil")
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod Assembly.Load(SpinDMSUtil) : " & Err.Description)
                End If
                Err.Clear()
                Dim o As Object = a8.CreateInstance("SpinDMSUtil.NetGenFinder")
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod CreateInstance(SpinDMSUtil.NetGenFinder) : " & Err.Description)
                End If
                ' strXMLsakojimRadim = "C:\Razmjena\NG\jupitersrv\SharedDLLs\xml\akcije.xml"
                m_xmld = o.GetFile(strXMLsakojimRadim)
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod GetFile: " & Mid & " - " & Err.Description)
                End If
                cls1.NapuniCBLog("SpinDMSUtil Source : " & o.source)
                cls1.NapuniCBLog("SpinDMSUtil exception : " & o.exception)
                cls1.NapuniCBLog("SpinDMSUtil connstring : " & o.connstring)
                cls1.NapuniCBLog("SpinDMSUtil JupiterClientIniPath : " & o.JupiterClientIniPath)
                '  If Err.Number = 0 Then
                cls1.strXMLsakojimRadimzaCBlog = "SpinDMSUtil XML PUTANJA: " & strXMLsakojimRadim
                'End If
                '------------------------------------------------------------------------------ end of XML iz DMS baze 

                If m_xmld.InnerXml = "" Then 'm_xmld Is Nothing Then

                    Dim ImamoGreskuKodLodanjaIzBaze As Boolean = False

                    If Conn4.State = ConnectionState.Open Then

                        Dim ds As DataSet = New DataSet
                        'Dim da As New SqlClient.SqlDataAdapter("select tekst1 from jupiterhelpdx where granula like '%" & Mid & "%'", Conn4)

                        ' čišćenje foldera , prijavljivali su i / i \
                        Mid = Mid.Replace("/", "\")
                        Dim imeXMLa77 = Mid.Substring(InStrRev(Mid, "\"), Mid.Length - InStrRev(Mid, "\"))
                        imeXMLa77 = imeXMLa77.Trim

                        Err.Clear()
                        Dim da As New SqlClient.SqlDataAdapter("select body from netgenxml where xml = '" & imeXMLa77 & "'", Conn4)

                        'kbco
                        'Dim da As New SqlClient.SqlDataAdapter("select tekst1 as body from jupiterhelpdx where granula like '%" & Mid & "%'", Conn4)


                        'Dim da As New SqlClient.SqlDataAdapter("select body from netgenxml where xml = '" & imeXMLa77 & "'", Conn4)
                        da.SelectCommand.CommandTimeout = strCommandTimeout
                        da.Fill(ds, "fff")

                        If ds.Tables.Count > 0 And Err.Number = 0 Then
                            Dim v1 As Byte() = ds.Tables(0).Rows(0)("body")
                            Dim v2 As String = "pero"
                            v2 = KriptirajDekriptiraj.EncryptDecrypt.Decrypt(v1, "tan123ge")
                            If v2 = "pero" Then
                                cls1.NapuniCBLog("XML nije pronađen u bazi JUPITERXML DB .. NetGenXML table : " & imeXMLa77 & vbCr & vbCr & Err.Description)
                                ImamoGreskuKodLodanjaIzBaze = True
                            Else
                                m_xmld.LoadXml(v2)
                            End If

                        Else
                            'cls1.NapuniCBLog("XML nije pronađen u bazi JUPITERXML DB .. NetGenXML table : " & imeXMLa77 & vbCr & vbCr & Err.Description)
                            ImamoGreskuKodLodanjaIzBaze = True
                        End If

                        If ImamoGreskuKodLodanjaIzBaze <> 0 Then
                            'MsgBox("XML LOAD ERROR FROM JUPITERXML DB : " & Mid & vbCr & vbCr & Err.Description)
                            'ImamoGreskuKodLodanjaIzBaze = True
                            'cls1.NapuniCBLog("XML LOAD ERROR FROM JUPITERXML DB .. NetGenXML table : " & imeXMLa77 & vbCr & vbCr & Err.Description)
                            'Exit Sub
                        Else
                            'strXMLsakojimRadim = "JupiterXML Database : " & Mid
                            cls1.strXMLsakojimRadimzaCBlog = "JupiterXML Database .. NetGenXML table : " & imeXMLa77
                        End If


                    Else

                        m_xmld.Load(strXMLsakojimRadim)
                        If Err.Number <> 0 Then
                            'MsgBox("XML LOAD ERROR : " & strXMLsakojimRadim & vbCr & vbCr & Err.Description)
                            'Exit Sub
                        End If
                    End If


                    If ImamoGreskuKodLodanjaIzBaze Then '------- ajd onda probaj sa diska :
                        'm_xmld = Nothing
                        Err.Clear()
                        'strXMLsakojimRadim = strXMLsakojimRadim.Replace("\\", "\")
                        cls1.NapuniCBLog("Učitavam XML na stari način sa file sistema ...")
                        m_xmld = New Xml.XmlDocument()
                        m_xmld.Load(strXMLsakojimRadim)
                        If Err.Number <> 0 Then
                            MsgBox("XML LOAD ERROR : " & strXMLsakojimRadim & vbCr & vbCr & Err.Description)

                            'Exit Sub
                        End If
                    End If


                End If ' end of stari način bez dms-a 


                ' MsgBox("Poruka 1 : " & vbCr & Err.Description & vbCr & vbCr & strXMLsakojimRadim)
            End If

        Else
            m_xmld.LoadXml(My.Resources.Resource1.String1)

            If Err.Number <> 0 Then
                MsgBox("XML LOAD ERROR : " & My.Resources.Resource1.String1 & vbCr & vbCr & Err.Description)
                'Exit Sub
            End If

        End If




        Dim m_node5 As Xml.XmlNode
        Dim m_node6 As Xml.XmlNode

        m_node5 = m_xmld.SelectSingleNode("/PROJECT/MASTERCOLUMNS")
        strCLEARCOLUMS = m_node5.Attributes.GetNamedItem("CLEARCOLUMS").Value.Trim()
        strHeaderAlignment = m_node5.Attributes.GetNamedItem("HEADERALIGNMENT").Value.Trim()


        m_nodelist = m_xmld.SelectNodes("/PROJECT")

        Dim strButtons As String = ""
        Dim strIcon As String = ""

        m_nodeListZaCustomFormat = m_xmld.SelectNodes("/PROJECT/MASTERCOLUMNS/COLUMN")
        m_nodeListZaCustomFormatDET = m_xmld.SelectNodes("/PROJECT/DETAILCOLUMNS/COLUMN")

        m_node6 = m_xmld.SelectSingleNode("/PROJECT/DETAILCOLUMNS")
        strCLEARCOLUMSdet = m_node6.Attributes.GetNamedItem("CLEARCOLUMS").Value.Trim()
        strHeaderAlignmentdet = m_node6.Attributes.GetNamedItem("HEADERALIGNMENT").Value.Trim()

        For Each m_node In m_nodelist
            Button1.Width = 79

            Button1.Text = m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.IndexOf(":"))
            Button2.Text = m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.IndexOf(":"))
            Button3.Text = m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.IndexOf(":"))
            Button4.Text = m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.IndexOf(":"))
            Button5.Text = m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.IndexOf(":"))
            Button6.Text = m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.IndexOf(":"))
            Button7.Text = m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.IndexOf(":"))
            Button8.Text = m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.IndexOf(":"))
            Button9.Text = m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.IndexOf(":"))
            Button10.Text = m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.Substring(0, m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.IndexOf(":"))

            Button1.Tag = m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim.IndexOf(":") - 1)
            Button2.Tag = m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim.IndexOf(":") - 1)
            Button3.Tag = m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim.IndexOf(":") - 1)
            Button4.Tag = m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim.IndexOf(":") - 1)
            Button5.Tag = m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim.IndexOf(":") - 1)
            Button6.Tag = m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim.IndexOf(":") - 1)

            Button7.Tag = m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD7CAPTION").Value.Trim.IndexOf(":") - 1)
            Button8.Tag = m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD8CAPTION").Value.Trim.IndexOf(":") - 1)
            Button9.Tag = m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD9CAPTION").Value.Trim.IndexOf(":") - 1)
            Button10.Tag = m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.Substring(m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.IndexOf(":") + 1, m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.Length - m_node.Attributes.GetNamedItem("CMD10CAPTION").Value.Trim.IndexOf(":") - 1)

            strCombo = m_node.Attributes.GetNamedItem("COMBO").Value.Trim
            strComboClick = m_node.Attributes.GetNamedItem("COMBOCLICK").Value.Trim

            If strCombo.Trim <> "" Then
                Trs4 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(strCombo)))
                ComboBox1.DataSource = Trs4
                ComboBox1.DisplayMember = Trs4.Columns(1).Caption
                ComboBox1.ValueMember = Trs4.Columns(0).Caption
            Else
                ComboBox1.Visible = False
                TreeView2.Top = 3
            End If

            GridEX1.GroupByBoxVisible = m_node.Attributes.GetNamedItem("GROUPBYBOXVISIBLE").InnerText
            GridEX2.GroupByBoxVisible = m_node.Attributes.GetNamedItem("GROUPBYBOXDETVISIBLE").InnerText

            strCUSTOMIMASTERGRIDLAYOUTFILE = ""
            strCUSTOMIMASTERGRIDLAYOUTFILE = m_node.Attributes.GetNamedItem("MASTERGRIDLAYOUTFILE").Value.ToString.Trim

            strTREEDragDrop = ""
            strTREEDragDrop = m_node.Attributes.GetNamedItem("TREEDRAGDROP").Value.ToString.Trim

            strTREETranslate = ""
            strTREETranslate = m_node.Attributes.GetNamedItem("TREETRANSLATE").Value.ToString.Trim

            GridEX1.FrozenColumns = m_node.Attributes.GetNamedItem("FROZENCOLUMNS").Value.Trim

            strOnEnterKey = m_node.Attributes.GetNamedItem("ONENTERKEY").InnerText
            strOnEnterKey = m_node.Attributes.GetNamedItem("ONENTERKEY").InnerText
            strONClosed = m_node.Attributes.GetNamedItem("ONCLOSED").Value.Trim
            strStartupStora = m_node.Attributes.GetNamedItem("STARTUPSTORA").Value.Trim
            strStartupStora = m_node.Attributes.GetNamedItem("ONLOAD").Value.Trim
            strONLoad2 = m_node.Attributes.GetNamedItem("ONLOAD2").Value.Trim
            MultiGridLayout = m_node.Attributes.GetNamedItem("MULTIGRIDLAYOUT").Value.Trim
            strSETdefaults = m_node.Attributes.GetNamedItem("SETDEFAULTS").Value.Trim
            strFilterSave = m_node.Attributes.GetNamedItem("FILTERSAVE").Value.Trim
            cls1.strStartupStoraDetalj = m_node.Attributes.GetNamedItem("DETAILGRID").Value.Trim
            cls1.strDMSDet = m_node.Attributes.GetNamedItem("DMSDET").Value.Trim
            'If cls1.strDMSDet = "" Then
            '    TabControl2.DrawMode = TabDrawMode.OwnerDrawFixed
            'Else
            '    TabControl2.DrawMode = TabDrawMode.Normal
            'End If
            cls1.strDetMoveLast = m_node.Attributes.GetNamedItem("DETMOVELAST").Value.Trim
            cls1.strMasterMoveLast = m_node.Attributes.GetNamedItem("MASTERMOVELAST").Value.Trim
            'strInclude = m_node.Attributes.GetNamedItem("INCLUDE").InnerText.ToUpper
            cls1.strIncludeGlobal = m_node.Attributes.GetNamedItem("INCLUDE").InnerText.ToUpper
            strPopUp = m_node.Attributes.GetNamedItem("POPUP").Value.Trim
            strPopUp = m_node.Attributes.GetNamedItem("POPUPMASTER").Value.Trim
            strPopUpDet = m_node.Attributes.GetNamedItem("POPUPDETAIL").Value.Trim
            strTreeNodes = m_node.Attributes.GetNamedItem("TREE").Value.Trim
            strDBlClickForm = m_node.Attributes.GetNamedItem("DBLCLICKFORM").Value.Trim
            strDBlClickForm = m_node.Attributes.GetNamedItem("MASTERDBLCLICK").Value.Trim
            strDBlClickDetForm = m_node.Attributes.GetNamedItem("DETAILDBLCLICK").Value.Trim
            strButtons = m_node.Attributes.GetNamedItem("BUTTONS").Value.Trim
            strIcon = m_node.Attributes.GetNamedItem("ICON").Value.Trim
            Version1 = m_node.Attributes.GetNamedItem("VERSION").Value.Trim
            strSplitterDistance = m_node.Attributes.GetNamedItem("SPLITTERDISTANCE").Value.Trim
            strPermisio = m_node.Attributes.GetNamedItem("PERMISIO").Value.Trim
            strREFRESHAFTERCHILD = m_node.Attributes.GetNamedItem("REFRESHAFTERCHILD").Value.Trim
            strTranslateColumns = m_node.Attributes.GetNamedItem("TRANSLATECOLUMNS").Value.Trim
            strTranslateDETColumns = m_node.Attributes.GetNamedItem("TRANSLATEDETCOLUMNS").Value.Trim


            strAUTOMATICSORT = m_node.Attributes.GetNamedItem("AUTOMATICSORT").InnerText
            If strAUTOMATICSORT.ToLower = "false" Then
                GridEX1.AutomaticSort = False
            End If

            strAUTOMATICSORTdet = m_node.Attributes.GetNamedItem("AUTOMATICSORTDET").InnerText
            If strAUTOMATICSORTdet.ToLower = "false" Then
                GridEX2.AutomaticSort = False
            End If


            DXGrid = m_node.Attributes.GetNamedItem("DXGRID").Value.Trim

            If strPermisio.ToString.ToLower = "true" Then
                ssPermissionProcitaj()
            End If


            If strEditColumns = "" Then
                GridEX1.EnterKeyBehavior = EnterKeyBehavior.None
            End If

            GridEX1.EnterKeyBehavior = m_node.Attributes.GetNamedItem("ENTERKEYBEHAVIOR").InnerText

            strFilterMode = m_node.Attributes.GetNamedItem("FILTERMODE").Value.Trim
            strFilterModeDet = m_node.Attributes.GetNamedItem("FILTERMODEDET").Value.Trim

            On Error Resume Next
            ' MsgBox(MOJremotedllpath & "\Graphics\Icons\" & strIcon)
            If strIcon.Trim = "" Then
                'strIcon = Ikona
                'Me.Icon = New System.Drawing.Icon(Ikona)
                ' MsgBox(strIcon)

                Dim bmp As Bitmap = New System.Drawing.Bitmap(Ikona)
                Me.Icon = Icon.FromHandle(bmp.GetHicon)
                'MsgBox(Err.Description)

            Else
                Me.Icon = New System.Drawing.Icon(MOJremotedllpath & "\Graphics\Icons\" & strIcon)
            End If



            'Me.Icon = New System.Drawing.Icon(MOJremotedllpath & "\Graphics\Icons\" & strIcon)

            strMenu = m_node.Attributes.GetNamedItem("MENU").Value.Trim
            strTimer = m_node.Attributes.GetNamedItem("TIMER").Value.Trim
            If strTimer.Trim = "" Then
                ToolStripStatusLabel3.Visible = False
            End If
            strPageHeaderRight = m_node.Attributes.GetNamedItem("PAGEHEADERRIGHT").Value.Trim
            strPageHeaderCenter = m_node.Attributes.GetNamedItem("PAGEHEADERCENTER").Value.Trim
            strPageHeaderLeft = m_node.Attributes.GetNamedItem("PAGEHEADERLEFT").Value.Trim
            strPageFooterCenter = m_node.Attributes.GetNamedItem("PAGEFOOTERCENTER").Value.Trim
            strPageFooterLeft = m_node.Attributes.GetNamedItem("PAGEFOOTERLEFT").Value.Trim
            strPageFooterRight = m_node.Attributes.GetNamedItem("PAGEFOOTERRIGHT").Value.Trim
            strPageOrientation = m_node.Attributes.GetNamedItem("PAGEORIENTATION").Value.Trim
            strCHECKBOXVISIBLE = m_node.Attributes.GetNamedItem("CHECKBOXVISIBLE").InnerText.ToUpper
            strTOTALROWVISIBLE = m_node.Attributes.GetNamedItem("TOTALROWVISIBLE").InnerText.ToUpper
            strDetCHECKBOXVISIBLE = m_node.Attributes.GetNamedItem("DETCHECKBOXVISIBLE").InnerText.ToUpper
            strDetTOTALROWVISIBLE = m_node.Attributes.GetNamedItem("DETTOTALROWVISIBLE").InnerText.ToUpper
            strCommandTimeout = m_node.Attributes.GetNamedItem("COMMANDTIMEOUT").InnerText.ToUpper
            strRelacija = m_node.Attributes.GetNamedItem("RELATION").InnerText.ToUpper

            strEditColumns = m_node.Attributes.GetNamedItem("EDITCOLUMNS").InnerText

            If strEditColumns.ToLower.StartsWith("sp") Or strEditColumns.ToLower.StartsWith("select") Then
                Dim trs88 As DataTable
                trs88 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(strEditColumns)))
                strEditColumns = trs88(0)(0)
            End If

            strONCellEditedstr = m_node.Attributes.GetNamedItem("ONCELLEDITED").InnerText
            strBeforeCellEditedstr = m_node.Attributes.GetNamedItem("BEFORECELLEDITED").InnerText

            strEditColumnsDET = m_node.Attributes.GetNamedItem("EDITCOLUMNSDET").InnerText
            strONCellEditedDETstr = m_node.Attributes.GetNamedItem("ONCELLEDITEDDET").InnerText
            strBeforeCellEditedDETstr = m_node.Attributes.GetNamedItem("BEFORECELLEDITEDDET").InnerText


            DefaultConnection = 1
            DefaultConnection = m_node.Attributes.GetNamedItem("DEFAULTCONNECTION").InnerText.ToUpper

            strSQL2 = m_node.Attributes.GetNamedItem("SQL2").InnerText.ToUpper
            strSQL3 = m_node.Attributes.GetNamedItem("SQL3").InnerText.ToUpper
            strSQL4 = m_node.Attributes.GetNamedItem("SQL4").InnerText.ToUpper
            strHelpKey = m_node.Attributes.GetNamedItem("HELPKEY").InnerText.ToUpper
            strHelpKod = m_node.Attributes.GetNamedItem("HELPKOD").InnerText.ToUpper

            If strHelpKod.Trim <> "" Then
                ButtonHELP.Visible = True
            Else
                ButtonHELP.Visible = False
            End If

            Dim strHelpFile As String = ""
            Dim strHelpKeyWord As String = ""

            'If strHelpKey.Trim <> "" Then
            '    ButtonHELP.Visible = True
            '    If strHelpKey.Trim <> "" Then
            '        Trs2 = cls1.ExecSql("select HelpChm, HelpHtml from JupiterSystem..JupiterHelp where Parametar = '" & strHelpKey & "'")
            '        If Not trs2 Is Nothing Then
            '            If trs2.Rows.Count > 0 Then
            '                strHelpFile = strRemotedllpath & "\help\" & trs2.Rows(0)(0)
            '                strHelpKeyWord = trs2.Rows(0)(1)
            '                'System.Diagnostics.Process.Start(strRemotedllpath & "\help\" & trs2.Rows(0)(0) & " " & trs2.Rows(0)(1))
            '            End If
            '        End If
            '    End If
            '    HelpProvider1.HelpNamespace = strHelpFile
            '    HelpProvider1.SetHelpNavigator(Me, HelpNavigator.Topic)
            '    HelpProvider1.SetHelpKeyword(Me, strHelpKeyWord)
            'Else
            '    ButtonHELP.Visible = False
            'End If


        Next

        m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")

        If Version1.Trim <> "" Then
            Me.Text = m_node.ChildNodes.Item(0).InnerText & "  v[" & Trim(Version1) & "]"
        Else
            Me.Text = m_node.ChildNodes.Item(0).InnerText
        End If

        cls1.strProjectCaption = Me.Text

        If Version1.Trim <> "" Then
            ToolStripStatusLabel4.Text = "xml v[" & Trim(Version1) & "]"
        Else
            ToolStripStatusLabel4.Text = ""
        End If


        ' za 3th konekciju
        If strCUSTOMCONNECTIONSTRING <> "" Then
            Conn3.ConnectionString = strCUSTOMCONNECTIONSTRING
            Conn3.Open()
        End If
        '--------------------------------

        If strMenu.Trim <> "" Then
            If strMenu.Trim.ToUpper = "FOLDER" Then
                NapuniMenuFolder()
            Else
                NapuniMenuBaza()
            End If
        Else
            MenuStrip1.Visible = False
        End If



        Dim a() As String
        Dim j As Integer
        If strPopUp.Trim <> "" Then
            a = strPopUp.Split("#")
            ContextMenuStrip1.Items.Clear()
            For j = 0 To a.GetUpperBound(0)
                ' ContextMenuStrip1.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
                ContextMenuStrip1.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), Nothing, AddressOf ToolStripMenuItem1_Click)
                ContextMenuStrip1.Items(j).Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)


                Dim imeXMLa As String = ""
                imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

                imeXMLa = imeXMLa.Trim.ToLower



                On Error Resume Next
                If Not trsPermisio Is Nothing Then
                    If trsPermisio.Rows.Count > 0 Then
                        Dim r As DataRow
                        For Each r In trsPermisio.Rows

                            If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = ContextMenuStrip1.Items(j).Text.Trim.ToLower Then

                                If r("smije") = False Then
                                    ContextMenuStrip1.Items(j).Enabled = False
                                End If

                                If r("smije") = True Then
                                    ContextMenuStrip1.Items(j).Enabled = True
                                    ContextMenuStrip1.Items(j).Visible = True
                                    Exit For 'kk 21.02.2022 ako je bilo koji red true , bilo za osobu ili za grupu izađi van - sve je OK
                                End If

                                Err.Clear()
                                If r("Vidljivost") = False Then
                                    If Err.Number = 0 Then
                                        ContextMenuStrip1.Items(j).Visible = False
                                    End If
                                End If

                                Err.Clear()
                                If r("Vidljivost") = True Then
                                    If Err.Number = 0 Then
                                        ContextMenuStrip1.Items(j).Visible = True
                                    End If
                                End If

                            End If


                        Next
                    End If
                End If



            Next
            ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip1.Items(j).Tag = "-"
        Else
            j = -1
            ContextMenuStrip1.Items.Clear()
        End If

        ContextMenuStrip1.Items.Add(Translate("Spremi postavke grida"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 1).Tag = "spremigrid"
        If ssPermissionProvjeriRafalnuKomandu("Spremi postavke grida") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Spremi postavke grida")
            ContextMenuStrip1.Items(j + 1).Enabled = False
        End If

        ContextMenuStrip1.Items.Add(Translate("Reset grida"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 2).Tag = "resetgrid"
        If ssPermissionProvjeriRafalnuKomandu("Reset grida") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Reset grida")
            ContextMenuStrip1.Items(j + 2).Enabled = False
        End If

        ContextMenuStrip1.Items.Add(Translate("Export grid"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 3).Tag = "exportgrid"
        If ssPermissionProvjeriRafalnuKomandu("Export grid") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Export grid")
            ContextMenuStrip1.Items(j + 3).Enabled = False
        End If

        ContextMenuStrip1.Items.Add(Translate("Grupiraj / razgrupiraj"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 4).Tag = "groupungropu"
        If ssPermissionProvjeriRafalnuKomandu("Grupiraj / razgrupiraj") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Grupiraj / razgrupiraj")
            ContextMenuStrip1.Items(j + 4).Enabled = False
        End If

        ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 5).Tag = "-"

        ContextMenuStrip1.Items.Add(Translate("Format style"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 6).Tag = "format"
        ContextMenuStrip1.Items.Add(Translate("Izgled grida"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 7).Tag = "izgled"


        ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 8).Tag = "-"

        ContextMenuStrip1.Items.Add(Translate("Ispis REPX"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 9).Tag = "ispisdx"
        If ssPermissionProvjeriRafalnuKomandu("Ispis REPX") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis REPX")
            ContextMenuStrip1.Items(j + 9).Enabled = False
        End If

        ContextMenuStrip1.Items.Add(Translate("Ispis"), Nothing, AddressOf ToolStripMenuItem1_Click)
        ContextMenuStrip1.Items(j + 10).Tag = "ispisnovi"
        If ssPermissionProvjeriRafalnuKomandu("Ispis") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis")
            ContextMenuStrip1.Items(j + 10).Enabled = False
        End If


        'ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 8).Tag = "-"
        'ContextMenuStrip1.Items.Add("Pregled ispisa", Nothing, AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 9).Tag = "ispis"
        'ContextMenuStrip1.Items.Add("Pregled odabranih", Nothing, AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 10).Tag = "ispisodabranih"
        'ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 11).Tag = "-"
        'ContextMenuStrip1.Items.Add("Ispis", Nothing, AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 12).Tag = "ispisdirektan"
        'ContextMenuStrip1.Items.Add("Ispis odabranih", Nothing, AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 13).Tag = "ispisodabranihdirektan"
        'ContextMenuStrip1.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 14).Tag = "-"
        'ContextMenuStrip1.Items.Add("Ispis novi", Nothing, AddressOf ToolStripMenuItem1_Click)
        'ContextMenuStrip1.Items(j + 15).Tag = "ispisnovi"
        ''ContextMenuStrip1.Show(sender, sender.PointToClient(Windows.Forms.Cursor.Position))





        If strPopUpDet <> "" Then
            a = strPopUpDet.Split("#")
            ContextMenuStrip2.Items.Clear()
            For j = 0 To a.GetUpperBound(0)
                'ContextMenuStrip2.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), ImageList1.Images(0), AddressOf ToolStripMenuItem2_Click)
                ContextMenuStrip2.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), Nothing, AddressOf ToolStripMenuItem2_Click)
                ContextMenuStrip2.Items(j).Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
            Next
            ContextMenuStrip2.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem2_Click)
            ContextMenuStrip2.Items(j).Tag = "-"
        Else
            j = -1
            ContextMenuStrip2.Items.Clear()
        End If

        ContextMenuStrip2.Items.Add(Translate("Spremi postavke grida"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 1).Tag = "spremigrid"
        ContextMenuStrip2.Items.Add(Translate("Reset grida"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 2).Tag = "resetgrid"
        ContextMenuStrip2.Items.Add(Translate("Export grid"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 3).Tag = "exportgrid"
        ContextMenuStrip2.Items.Add(Translate("Grupiraj / razgrupiraj"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 4).Tag = "groupungropu"



        ContextMenuStrip2.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 5).Tag = "-"



        ContextMenuStrip2.Items.Add(Translate("Format style"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 6).Tag = "format"
        ContextMenuStrip2.Items.Add(Translate("Izgled grida"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 7).Tag = "izgled"



        ContextMenuStrip2.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 8).Tag = "-"
        ContextMenuStrip2.Items.Add(Translate("Ispis REPX"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 9).Tag = "ispisdx"
        If ssPermissionProvjeriRafalnuKomandu("Ispis REPX") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis REPX")
            ContextMenuStrip2.Items(j + 9).Enabled = False
        End If

        ContextMenuStrip2.Items.Add(Translate("Ispis"), Nothing, AddressOf ToolStripMenuItem2_Click)
        ContextMenuStrip2.Items(j + 10).Tag = "ispisnovi"
        If ssPermissionProvjeriRafalnuKomandu("Ispis") = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis")
            ContextMenuStrip2.Items(j + 10).Enabled = False
        End If

        'ContextMenuStrip2.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem2_Click)
        'ContextMenuStrip2.Items(j + 8).Tag = "-"
        'ContextMenuStrip2.Items.Add("Ispis", Nothing, AddressOf ToolStripMenuItem2_Click)
        'ContextMenuStrip2.Items(j + 9).Tag = "ispis"

        'ContextMenuStrip2.Items.Add("Ispis odabranih", Nothing, AddressOf ToolStripMenuItem2_Click)
        'ContextMenuStrip2.Items(j + 10).Tag = "ispisodabranih"

        'ContextMenuStrip1.Show(sender, sender.PointToClient(Windows.Forms.Cursor.Position))





        If strButtons <> "" Then
            a = strButtons.Split("#")
            For j = 0 To a.GetUpperBound(0)
                If j = 0 Then
                    Button1.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button1.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If
                If j = 1 Then
                    Button2.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button2.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If
                If j = 2 Then
                    Button3.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button3.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If
                If j = 3 Then
                    Button4.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button4.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If
                If j = 4 Then
                    Button5.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button5.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If
                If j = 5 Then

                    Button6.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button6.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If

                If j = 6 Then
                    If Button7.Text.Trim = "" Then
                        Button7.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                        Button7.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                    End If
                End If

                If j = 7 Then
                    Button8.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button8.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If

                If j = 8 Then
                    Button9.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button9.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If

                If j = 9 Then
                    Button10.Text = a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))
                    Button10.Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                End If

                If j = 10 Then

                End If
            Next
        End If

        If Button1.Text = "" Then Button1.Visible = False Else Button1.Visible = True
        If Button2.Text = "" Then Button2.Visible = False Else Button2.Visible = True
        If Button3.Text = "" Then Button3.Visible = False Else Button3.Visible = True
        If Button4.Text = "" Then Button4.Visible = False Else Button4.Visible = True
        If Button5.Text = "" Then Button5.Visible = False Else Button5.Visible = True
        If Button6.Text = "" Then Button6.Visible = False Else Button6.Visible = True

        If Button7.Text = "" Then Button7.Visible = False Else Button7.Visible = True
        If Button8.Text = "" Then Button8.Visible = False Else Button8.Visible = True
        If Button9.Text = "" Then Button9.Visible = False Else Button9.Visible = True
        If Button10.Text = "" Then Button10.Visible = False Else Button10.Visible = True

        Button2.Left = Button1.Left + Button1.Width + 8
        Button3.Left = Button2.Left + Button2.Width + 8
        Button4.Left = Button3.Left + Button3.Width + 8
        Button5.Left = Button4.Left + Button4.Width + 8
        Button6.Left = Button5.Left + Button5.Width + 8

        Button7.Left = Button6.Left + Button6.Width + 8
        Button8.Left = Button7.Left + Button7.Width + 8
        Button9.Left = Button8.Left + Button8.Width + 8
        Button10.Left = Button9.Left + Button9.Width + 8

        ' ButtonHELP.Left = Button10.Left + Button10.Width + 8

        SplitContainer2.SplitterDistance = Me.Height - CInt(Me.Height / 2)



        '--------------------------- 21.05.2009.  update CB-a sa setdefaultsima
        On Error Resume Next

        cls1.strSviParametriuModulu = ""
        Dim Ar5() As String '-------- rafalni pristup 27.05.2009.
        Dim i As Long
        If strSETdefaults.ToString.Trim <> "" Then
            Ar5 = Split(strSETdefaults.ToString, "#")
            For i = 0 To UBound(Ar5)
                SetDefolte(Ar5(i).ToString)
            Next
        End If

        '----------------------------------------------------------------------

        NapuniDrvoVrijeme()

        ssPermissionProvjeri()

        'On Error Resume Next
        'Node = ""
        'Node = TreeView2.SelectedNode.Tag.ToString.Substring(0, TreeView2.SelectedNode.Tag.ToString.IndexOf(":"))

        If strStartupStora.Substring(0, 1) = ":" Then
            NovaGenerickaForma(strStartupStora.Replace(":", ""))
        Else
            PokaziUpit(cls1.SrediSQLPosljeNextForme(SrediSQL(strStartupStora)))
        End If


        ' TranslateForm(Me)
        Button1.Text = Translate(Button1.Text)
        Button2.Text = Translate(Button2.Text)
        Button3.Text = Translate(Button3.Text)
        Button4.Text = Translate(Button4.Text)
        Button5.Text = Translate(Button5.Text)
        Button6.Text = Translate(Button6.Text)

        Button7.Text = Translate(Button7.Text)
        Button8.Text = Translate(Button8.Text)
        Button9.Text = Translate(Button9.Text)
        Button10.Text = Translate(Button10.Text)

        Me.Text = Translate(Me.Text)



        '--------------------------- 21.05.2009.  dodatno poštimavanje butona na podlozi
        On Error Resume Next
        Dim Trs3 As New DataTable
        If strONLoad2.Trim <> "" Then
            strONLoad2 = cls1.SrediSQLPosljeNextForme(SrediSQL(strONLoad2))
            Trs3 = cls1.ExecSql(strONLoad2)
            If Err.Number <> 0 Then
                strONLoad2 = ""
            End If
        End If

        If strONLoad2 <> "" Then
            If Trs3.Rows.Count > 0 Then
                DodatnoPostimajKontrolu(Button1, Trs3.Rows(0)(Button1.Text).ToString)
                DodatnoPostimajKontrolu(Button2, Trs3.Rows(0)(Button2.Text).ToString)
                DodatnoPostimajKontrolu(Button3, Trs3.Rows(0)(Button3.Text).ToString)
                DodatnoPostimajKontrolu(Button4, Trs3.Rows(0)(Button4.Text).ToString)
                DodatnoPostimajKontrolu(Button5, Trs3.Rows(0)(Button5.Text).ToString)
                DodatnoPostimajKontrolu(Button6, Trs3.Rows(0)(Button6.Text).ToString)

                DodatnoPostimajKontrolu(Button7, Trs3.Rows(0)(Button7.Text).ToString)
                DodatnoPostimajKontrolu(Button8, Trs3.Rows(0)(Button8.Text).ToString)
                DodatnoPostimajKontrolu(Button9, Trs3.Rows(0)(Button9.Text).ToString)
                DodatnoPostimajKontrolu(Button10, Trs3.Rows(0)(Button10.Text).ToString)

            End If
        End If


        'If strStartupStoraDetalj.Substring(0, 1) = ":" Then
        '    MsgBox("sorkac!") 'NovaGenerickaForma(strStartupStora.Replace(":", ""))
        'Else
        '    PokaziUpitDetalj(strStartupStoraDetalj)
        'End If

        ' ClientToolbarGetButtons()


        ' ssPermissionProvjeri()

    End Sub

    Sub SetDefolte(ByVal strSETdefaults As String)

        Dim Trs2 As New DataTable

        '--------------------------- 21.05.2009.  update CB-a sa setdefaultsima
        On Error Resume Next

        Dim Ime5 As String
        Dim Nasao5 As Integer
        Dim Vrijednost5 As String



        If strSETdefaults.Trim <> "" Then
            Err.Clear()
            cls1.strSviParametriuModulu = ""
            'Trs2 = cls1.ExecSql(SrediSQL(strSETdefaults))
            Trs2 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(strSETdefaults)))

            Dim ts33 As String = ""
            ts33 = Trs2.Rows(0)("formcaption")
            If ts33 <> "" Then
                Me.Text = ts33
            End If

            Err.Clear()
            If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do nekontrolirane greške na ONLOAD forme idemo van

            If Err.Number = 0 Then
                Dim Column As DataColumn
                If Trs2.Rows.Count > 0 Then
                    For Each Column In Trs2.Columns

                        ' update CB sa startupstorom .............. TGClipBoard update ... 13.11.2008.
                        Vrijednost5 = "null"
                        Ime5 = Column.Caption & "="
                        Vrijednost5 = Trs2.Rows(0)(Column.Caption)

                        If Vrijednost5.StartsWith("'") Then Vrijednost5 = Vrijednost5.Substring(1, Vrijednost5.Length - 1)
                        If Vrijednost5.EndsWith("'") Then Vrijednost5 = Vrijednost5.Substring(0, Vrijednost5.Length - 1)

                        Vrijednost5 = Trim(Vrijednost5)

                        Nasao5 = -1
                        For j = 0 To UBound(cls1.CB)
                            If UCase(cls1.CB(j, 0)) = UCase(Ime5) Then
                                Nasao5 = j
                            End If
                        Next

                        If Nasao5 = -1 Then
                            ' 21.05.2009 setdefaults iz project taga može i dodati vrijednosti u cb
                            cls1.CB(cls1.PrviPrazniSlot, 0) = Ime5
                            cls1.CB(cls1.PrviPrazniSlot, 1) = Vrijednost5
                            cls1.PrviPrazniSlot = cls1.PrviPrazniSlot + 1
                        Else
                            cls1.CB(Nasao5, 1) = Vrijednost5
                        End If

                        '---------------------------------------------------------------------------

                    Next
                End If
            End If
        End If


    End Sub
    Sub DodatnoPostimajKontrolu(ByVal tctl As Control, ByVal tstr6 As String)
        On Error Resume Next
        If tstr6.Trim = "" Then Exit Sub
        Select Case tstr6.ToLower
            Case "visible"
                tctl.Visible = True
            Case "invisible"
                tctl.Visible = False
            Case "enable"
                tctl.Enabled = True
            Case "disable"
                tctl.Enabled = False
            Case "refresh"
                '
            Case Else
                If TypeOf tctl Is SearchBox Then

                Else
                    tctl.Text = tstr6
                End If
                ' tctl.Text = tstr6
        End Select
    End Sub

    Sub NapuniMenuFolder()
        On Error Resume Next
        MenuStrip1.Items.Clear()
        MenuStrip1.Visible = True
        Dim path As String = strXML2
        Dim i As Integer
        Dim oFS As New IO.DirectoryInfo(path & "\")
        Dim oFile As IO.FileInfo
        i = 1
        Dim m_xmld As Xml.XmlDocument
        Dim m_node As Xml.XmlNode
        m_xmld = New Xml.XmlDocument()
        Dim item As New ToolStripMenuItem(String.Format("XML-ovi", i.ToString()))
        Dim innerItem As New ToolStripMenuItem(String.Format(("BUILDIN 1 : XMLFile1")))
        item.DropDownItems.Add(innerItem)
        For Each oFile In oFS.GetFiles()
            If InStr(Trim(oFile.Name.ToLower), ".xml", CompareMethod.Text) > 0 Then
                m_xmld.Load(strXML2 & "\" & oFile.Name)
                m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")
                If Not m_node Is Nothing Then
                    Dim innerItem1 As New ToolStripMenuItem(m_node.InnerText)
                    innerItem1.Tag = strXML2 & "\" & oFile.Name
                    item.DropDownItems.Add(innerItem1)
                End If
            End If
        Next
        MenuStrip1.Items.Add(item)
        Dim filePath As String
        For Each folderPath As String In IO.Directory.GetDirectories(path)
            path = folderPath
            Dim oFS1 As New IO.DirectoryInfo(path & "\")
            Dim oFile1 As IO.FileInfo
            Dim innerItem1 As New ToolStripMenuItem(String.Format(IO.Path.GetFileName(folderPath)))
            innerItem1.Tag = ""
            MenuStrip1.Items.Add(innerItem1)
            For Each oFile1 In oFS1.GetFiles()
                If InStr(Trim(oFile1.Name.ToLower), ".xml", CompareMethod.Text) > 0 Then
                    m_xmld.Load(path & "\" & oFile1.Name)
                    m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")
                    Dim innerItem2 As New ToolStripMenuItem(m_node.InnerText)
                    innerItem2.Tag = path & "\" & oFile1.Name
                    innerItem1.DropDownItems.Add(innerItem2)
                End If
            Next
        Next
    End Sub





    Sub NapuniMenuBaza()
        MenuStrip1.Items.Clear()
        MenuStrip1.Visible = True
        Dim Trs As New DataTable
        Dim sql As String = "select * from netgenmenu" ' where parentid is null"
        Trs = cls1.ExecSql(sql)

        For i As Integer = 0 To Trs.Rows.Count - 1
            Dim item As New ToolStripMenuItem(String.Format(Trs.Rows(i)(2)))
            'If Not (Trs.Rows(i)(1)) Is DBNull.Value Then
            Dim trs2 As DataTable
            Dim sql2 As String = "select * from netgenmenu where parentid=" & (Trs.Rows(i)(0))
            trs2 = cls1.ExecSql(sql2)
            If Not trs2 Is Nothing Then 'DBNull.Value Then
                For j As Integer = 0 To trs2.Rows.Count - 1

                    Dim innerItem As New ToolStripMenuItem(String.Format(trs2.Rows(j)(2)))

                    Dim trs3 As DataTable
                    Dim sql3 As String = "select * from netgenmenu where parentid=" & (trs2.Rows(j)(0))
                    trs3 = cls1.ExecSql(sql3)
                    If Not (trs2.Rows(j)(1)) Is DBNull.Value Then
                        For k As Integer = 0 To trs3.Rows.Count - 1
                            Dim sideItem As New ToolStripMenuItem(String.Format(trs3.Rows(k)(2)))
                            innerItem.DropDownItems.Add(sideItem)
                        Next k
                    End If

                    item.DropDownItems.Add(innerItem)

                Next j
            End If
            If (Trs.Rows(i)(1)) Is DBNull.Value Then
                MenuStrip1.Items.Add(item)
            End If
        Next i
    End Sub


    Sub NapuniDrvo()
        Dim path As String = strXML2

        If InStr(Trim(strXML2), ".xml", CompareMethod.Text) > 0 Then
            ' SplitContainer1.SplitterDistance = 0
            'MsgBox(SplitContainer1.SplitterDistance)
            TreeView1.Visible = False
            Exit Sub
        Else

            TreeView1.Visible = True

        End If

        With TreeView1
            .ShowRootLines = True
            .ShowPlusMinus = True
            .ImageList = ImageList1
        End With

        If NapuniDrvoIzXMLA() Then Exit Sub ' ako postoju menu.xml u folderu onda uzimam iz njega meni

        Dim i As Integer
        Dim oNode As New System.Windows.Forms.TreeNode("Aplikacije :")
        Dim oFS As New System.IO.DirectoryInfo(path & "\")
        Dim oFile As System.IO.FileInfo

        i = 1

        Dim m_xmld As Xml.XmlDocument
        Dim m_node As Xml.XmlNode

        m_xmld = New Xml.XmlDocument()

        Dim nodp2 As New Windows.Forms.TreeNode("BuildinExample", 0, 0)
        nodp2.SelectedImageIndex = 0
        nodp2.Tag = "BuildinXML.xml"
        'TreeView1.Nodes.Add(nodp2) 

        On Error Resume Next

        For Each oFile In oFS.GetFiles()

            If InStr(Trim(oFile.Name.ToLower), ".xml", CompareMethod.Text) > 0 Then
                m_xmld.Load(strXML2 & "\" & oFile.Name)
                m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")
                'Dim nodp As New Windows.Forms.TreeNode(" " & Trim(oFile.Name) & " : " & m_node.ChildNodes.Item(0).InnerText, 0, 0)
                Dim nodp As New Windows.Forms.TreeNode(m_node.ChildNodes.Item(0).InnerText, 0, 0)
                nodp.SelectedImageIndex = 0
                nodp.Tag = Trim(oFile.Name)
                nodp.Tag = strXML2 & "\" & oFile.Name
                TreeView1.Nodes.Add(nodp)
            End If
            i = i + 1
        Next

        Dim folderNode As TreeNode
        Dim filePath As String
        Dim NoviNode As TreeNode

        For Each folderPath As String In System.IO.Directory.GetDirectories(path)
            path = folderPath
            folderNode = Me.TreeView1.Nodes.Add(IO.Path.GetFileName(folderPath))

            For Each filePath In System.IO.Directory.GetFiles(folderPath, "*.xml")
                m_xmld.Load(filePath)
                m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")
                Dim nodp As New Windows.Forms.TreeNode(m_node.ChildNodes.Item(0).InnerText, 0, 0)
                nodp.Tag = Trim(System.IO.Path.GetFileName(filePath))
                nodp.Tag = filePath
                folderNode.Nodes.Add(nodp)
            Next

            For Each NoviPath As String In System.IO.Directory.GetDirectories(path)
                'path = folderPath
                NoviNode = folderNode.Nodes.Add(System.IO.Path.GetFileName(NoviPath))
                For Each filePath In System.IO.Directory.GetFiles(NoviPath, "*.xml")
                    m_xmld.Load(filePath)
                    m_node = m_xmld.SelectSingleNode("/PROJECT/CAPTION")
                    Dim nodp As New Windows.Forms.TreeNode(m_node.ChildNodes.Item(0).InnerText, 0, 0)
                    nodp.Tag = Trim(System.IO.Path.GetFileName(filePath))
                    nodp.Tag = filePath
                    NoviNode.Nodes.Add(nodp)
                Next

            Next
        Next

    End Sub

    Function NapuniDrvoIzXMLA() As Boolean
        Try
            ' SECTION 1. Create a DOM Document and load the XML data into it.
            Dim dom As New XmlDocument()


            'Dim proxyObject As New System.Net.WebProxy(cls1.strProxy, 8080)
            'proxyObject.UseDefaultCredentials = True


            If InStr(Trim(strXML2.ToLower), "http", CompareMethod.Text) > 0 Then

                dom.Load(strXML2 & "/menu.xml")
            Else
                dom.Load(strXML2 & "\menu.xml")
            End If


            ' SECTION 2. Initialize the treeview control.
            TreeView1.Nodes.Clear()
            TreeView1.Nodes.Add(New TreeNode(dom.DocumentElement.Attributes.GetNamedItem("caption").Value))
            Dim tNode As New TreeNode()
            tNode = TreeView1.Nodes(0)

            ' SECTION 3. Populate the TreeView with the DOM nodes.
            AddNode(dom.DocumentElement, tNode)
            TreeView1.ExpandAll()

        Catch xmlEx As XmlException
            'MessageBox.Show(xmlEx.Message)
            NapuniDrvoIzXMLA = False
            Exit Function
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
            NapuniDrvoIzXMLA = False
            Exit Function
        End Try

        NapuniDrvoIzXMLA = True

    End Function

    Private Sub AddNode(ByRef inXmlNode As XmlNode, ByRef inTreeNode As TreeNode)
        Dim xNode As XmlNode
        Dim tNode As TreeNode
        Dim nodeList As XmlNodeList
        Dim i As Integer

        ' Loop through the XML nodes until the leaf is reached.
        ' Add the nodes to the TreeView during the looping process.
        If inXmlNode.HasChildNodes() Then
            nodeList = inXmlNode.ChildNodes
            For i = 0 To nodeList.Count - 1
                xNode = inXmlNode.ChildNodes(i)
                inTreeNode.Nodes.Add(New TreeNode(xNode.Attributes.GetNamedItem("caption").Value))
                tNode = inTreeNode.Nodes(i)
                ' tNode.Tag = (inXmlNode.OuterXml).Trim

                'Node.Tag = strXML2 & "\TEST-RobaInfo.xml"
                If InStr(Trim(strXML2.ToLower), "http", CompareMethod.Text) > 0 Then
                    tNode.Tag = strXML2 & "/" & inXmlNode.ChildNodes(i).Attributes.GetNamedItem("xml").Value
                Else
                    tNode.Tag = strXML2 & "\" & inXmlNode.ChildNodes(i).Attributes.GetNamedItem("xml").Value
                End If

                AddNode(xNode, tNode)

            Next
        Else

            ' Here you need to pull the data from the XmlNode based on the
            ' type of node, whether attribute values are required, and so forth.
            ' inTreeNode.Text = (inXmlNode.OuterXml).Trim

        End If
    End Sub



    Sub NapuniDrvoVrijeme()   ' ovo ustvari nije samo vrijeme nego i punjenje drveta sa spTree... storama i link na forme   .... reagira na TREE argument u PROJECT tagu
        cls1.NapuniCBLog(" Idem puniti drvo ..." & CStr(TimeOfDay()))
        On Error Resume Next
        Dim tNode As TreeNode
        tNode = TreeView2.SelectedNode

        TreeView2.Nodes.Clear()

        If TreeView1.Nodes.Count = 0 Then
            TabPage1.Text = ""
            TreeView1.Visible = False ' u dll-u nema granula
        End If


        If strTreeNodes.Trim = "" Then
            If strCombo.Trim = "" Then
                TreeView2.Visible = False
                If TreeView1.Visible = False Then
                    SplitContainer1.SplitterDistance = 0
                    ButtonZaDrvo.Enabled = False
                End If
            End If


            Exit Sub
        Else
            
            If strSplitterDistance.Trim <> "" Then
                SplitContainer1.SplitterDistance = strSplitterDistance
            Else
                SplitContainer1.SplitterDistance = 290
            End If
            ButtonZaDrvo.Enabled = True
            TabControl1.Visible = True
            TreeView2.Visible = True
        End If


        Dim j As Integer

        With TreeView2
            .ShowRootLines = True
            .ShowPlusMinus = True
        End With


        Dim a() As String

        a = strTreeNodes.Split("#")
        'ContextMenuStrip1.Items.Clear()

        For j = 0 To a.GetUpperBound(0)
            If a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower = "yearmonth" Then
                DazovaVremenska(a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1))
            ElseIf a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower.Contains("tree") Then
                DazovaIzStore(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")), a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1))
            Else
                Dim YearNode As TreeNode = New TreeNode(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))))
                YearNode.Tag = ":" + a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                TreeView2.Nodes.Add(YearNode)
            End If
        Next

        Dim trs As DataTable
        trs = cls1.ExecSql("select * from NetGenDBFilter where granula='" + strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\")) + "'")
        If Not trs Is Nothing Then
            If trs.Rows.Count > 0 Then
                Dim r As DataRow
                For Each r In trs.Rows
                    Dim YearNode As TreeNode = New TreeNode(r("naslov"))
                    YearNode.Tag = ":==" + r("sql")
                    TreeView2.Nodes.Add(YearNode)
                Next
            End If
        End If


        If TabControl1.Visible = True Then
            If TreeView2.Nodes.Count > 0 Then
                TreeView2.Visible = True
                TabControl1.SelectTab(1)

                'TreeView2.ExpandAll()
                TreeView2.SelectedNode = tNode ' ... ovo ne radi ?!?!?!?!

                '' ovdje očito još ima posla ...
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(0).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(1).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(2).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(3).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(4).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(5).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(6).Nodes(tNode.Name)
                'If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(7).Nodes(tNode.Name)


                Dim i1 As Integer
                Dim j1 As Integer

                If Not tNode Is Nothing Then
                    ' prvi nivo ...
                    'For i1 = 0 To 10
                    '    If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(i1).Nodes(tNode.Name)
                    'Next

                    '' drugi nivo ....
                    'If TreeView2.SelectedNode Is Nothing Then
                    '    For i1 = 0 To 10
                    '        If TreeView2.SelectedNode Is Nothing Then
                    '            For j1 = 0 To 10
                    '                If TreeView2.SelectedNode Is Nothing Then TreeView2.SelectedNode = TreeView2.Nodes(i1).Nodes(j1).Nodes(tNode.Name)
                    '            Next
                    '        End If
                    '    Next
                    'End If

                End If

                TreeView2.SelectedNode.EnsureVisible()

            Else
                TabControl1.SelectTab(0)
                TreeView2.Visible = False
            End If
        End If


        'If strFindNode2.Trim <> "" Then
        '    Dim arr As TreeNode() = TreeView2.Nodes.Find(strFindNode2, True)
        '    For i2 = 0 To arr.Length - 1
        '        TreeView2.SelectedNode = arr(i2)
        '        TreeView2.SelectedNode.Expand()
        '        TreeView2.SelectedNode.BackColor = Color.Red
        '    Next
        'End If

    End Sub


    Sub DazovaIzStore(ByVal Stora As String, ByVal ImeForme As String)

        On Error Resume Next
        Dim tStr As String = ""

        Dim ds As New System.Data.DataSet()
        Dim adapter As New System.Data.SqlClient.SqlDataAdapter()
        Stora = cls1.SrediSQLPosljeNextForme(SrediSQL(Stora))
        cls1.NapuniCBLog("Punjenje drveta sa storom : " & Stora)
        cls1.NapuniCBLog("Početak izvršavanja store : " & CStr(TimeOfDay()))
        Err.Clear()
        adapter.SelectCommand = New System.Data.SqlClient.SqlCommand(Stora.ToString, Conn)
        adapter.Fill(ds, "NTGNTEST")
        cls1.NapuniCBLog("Kraj izvršavanja store : " & CStr(TimeOfDay()))
        ds.Relations.Clear()
        ds.Relations.Add(TREE_RELATIONSHIP, ds.Tables(0).Columns(0), ds.Tables(0).Columns(1))
        cls1.NapuniCBLog("Kraj relacije  ID -> PARENTID : " & CStr(TimeOfDay()))
        Dim tstr2 As String = ""

        If Err.Number = 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                ' Dim rootNode As New TreeNode(Translate(ds.Tables(0).Rows(0)(2).ToString))




                If strTREETranslate.Trim.ToLower = "true" Then
                    tstr2 = Translate(ds.Tables(0).Rows(0)(2).ToString.Trim)
                Else
                    tstr2 = ds.Tables(0).Rows(0)(2).ToString.Trim
                End If

                Dim rootNode As New TreeNode(tstr2)



                rootNode.Tag = "0:" & ImeForme
                tStr = ""
                tStr = ds.Tables(0).Rows(0)(3)
                If tStr.Contains(":") Then
                    rootNode.Tag = tStr
                Else
                    rootNode.Tag = tStr & ":" & ImeForme
                End If
                AddNodes(ds.Tables(0).Rows(0), rootNode, ImeForme)
                Me.TreeView2.Nodes.Add(rootNode)
            End If
        Else
            ' frm5.TextBox1.Text = frm5.TextBox1.Text & TimeOfDay & " TREE PROBLEM : " & Err.Description & vbCrLf
            cls1.NapuniCBLog(" TREE PROBLEM : " & Err.Description)
        End If
        cls1.NapuniCBLog("Kraj punjenja tree kontrole : " & CStr(TimeOfDay()))
        'Me.TreeView2.ExpandAll()

    End Sub


    Private Sub AddNodes(ByRef ParentRow As DataRow, ByRef ParentNode As TreeNode, ByVal ImeForme As String)
        Dim children() As DataRow = ParentRow.GetChildRows(TREE_RELATIONSHIP)
        Dim tStr As String = ""
        If children.Length <> 0 Then
            For Each dr As DataRow In children
                ' If CType(dr(0), Integer) <> 0 Then
                If dr(0).ToString.Trim <> "0" Then
                    Dim newNode As TreeNode
                    'newNode = ParentNode.Nodes.Add(dr(0).ToString, Translate(dr(2).ToString))
                    If strTREETranslate.Trim.ToLower = "true" Then
                        newNode = ParentNode.Nodes.Add(dr(0).ToString.Trim, Translate(dr(2).ToString.Trim))
                    Else
                        newNode = ParentNode.Nodes.Add(dr(0).ToString.Trim, dr(2).ToString.Trim)
                    End If

                    'newNode.Tag = dr(3).ToString.Trim + ":" + ImeForme

                    tStr = ""
                    tStr = dr(3).ToString.Trim
                    If tStr.Contains(":") Then
                        newNode.Tag = tStr
                    Else
                        newNode.Tag = tStr & ":" & ImeForme
                    End If

                    'Dim newNode As New TreeNode(dr(2).ToString)
                    'newNode.Tag = dr(3).ToString.Trim + ":" + ImeForme
                    'ParentNode.Nodes.Add(newNode)
                    AddNodes(dr, newNode, ImeForme)
                End If
            Next
        End If
    End Sub


    Sub DazovaVremenska(ByVal ImeForme As String)
        Dim g, j As Integer

        For g = 2005 To 2009
            Dim YearNode As TreeNode = New TreeNode(g)
            YearNode.Tag = g.ToString + "0101" + "#" + g.ToString + "1231" + ":" + ImeForme.Trim
            TreeView2.Nodes.Add(YearNode)
            For j = 1 To 12
                Dim MonthNode As TreeNode = New TreeNode(MonthName(j))
                MonthNode.Tag = g.ToString + j.ToString + "01" + "#" + g.ToString + j.ToString + "30" + ":" + ImeForme.Trim
                YearNode.Nodes.Add(MonthNode)
                'MonthNode.Nodes.Add(j)
            Next
        Next
    End Sub

    Sub NovaGenerickaForma(ByVal stR As String)
        On Error Resume Next

        If stR Is Nothing Then Exit Sub

        cls1.NapuniCBLog("CALL NovaGenerickaForma(" & stR & ")")
        Dim Ar5() As String
        Dim i As Long

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("1" + Err.Description)
            Err.Clear()
        End If

        stR = stR.Replace("~", "#")


        If stR.ToString.Trim <> "" Then
            Ar5 = Split(stR.ToString, "#")

            If Err.Number <> 0 Then
                cls1.NapuniCBLog("2" + Err.Description)
                Err.Clear()
            End If

            For i = 0 To UBound(Ar5)

                If Ar5(i).ToUpper.Contains("REFRESHMASTERGRID") Then

                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("3" + Err.Description)
                        Err.Clear()
                    End If

                    PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)

                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("4" + Err.Description)
                        Err.Clear()
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("ASSEMBLYLOAD(") Then

                    Dim param As String = ""

                    PokupiParametreSaGrida()
                    ID = GetID()
                    cls1.MasterID = ID

                    param = Ar5(i).ToUpper.Trim
                    param = Ar5(i).Trim
                    param = param.Replace("ASSEMBLYLOAD(", "")
                    param = param.Replace(")", "")

                    Err.Clear()

                    Dim a As Assembly = Assembly.Load(param.Substring(0, InStrRev(param, ".") - 1))
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod Assembly.Load(" & param.Substring(0, InStrRev(param, ".") - 1) & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If

                    Err.Clear()
                    Dim o As Object = a.CreateInstance(param)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod CreateInstance (" & param & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If

                    PokupiParametreSaGrida()

                    Dim tconstr As String = Conn.ConnectionString.ToString
                    If tconstr.ToLower.Contains("driver") Then

                    Else
                        tconstr = "Driver={SQL Server};" & tconstr
                    End If

                    o.totalgen(ID, cls1.OperateriID.ToString, tconstr, strZadnjiSQLsakojimjepunjenGrid.ToString, ParametriSaGrida)


                ElseIf Ar5(i).Trim.ToUpper = "UNCHECKMASTERGRID" Then
                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    Dim RA2 As Integer = GridEX1.CurrentRow.AbsolutePosition
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    For Each row In checkedRows
                        GridEX1.MoveToRowIndex(row.RowIndex)
                        row.IsChecked = False
                    Next
                    GridEX1.MoveToRowIndex(RA2 - 1)
                ElseIf Ar5(i).Trim.ToUpper.Contains("BLAGAJNAENABLE(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("BLAGAJNAENABLE(", "")
                    param = param.Replace(")", "")
                    cls1.BlagajnaEnableZaMlinar(cls1.SrediSQLPosljeNextForme(SrediSQL(param)))
                ElseIf Ar5(i).Trim.ToUpper = "CLOSE" Then
                    Me.Close()
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJA(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJA(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        Fiskalizacija(param)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            Fiskalizacija(param)
                        Next
                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAN(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAN(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        FiskalizacijaNapojnica(param)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            FiskalizacijaNapojnica(param)
                        Next
                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAFAKTURE(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAFAKTURE(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        Fiskalizacija(param, 2)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            Fiskalizacija(param, 2)
                        Next
                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJABSRACUNI(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJABSRACUNI(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        Fiskalizacija(param, 3)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            Fiskalizacija(param, 3)
                        Next
                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAPD(") Then 'LOGARA
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAPD(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        Fiskalizacija(param, 4)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            Fiskalizacija(param, 4)
                        Next
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAPONUDE(") Then 'MARTINA
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAPONUDE(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        Fiskalizacija(param, 5)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            Fiskalizacija(param, 5)
                        Next
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("POSLOVNIPROSTOR(") Then
                    Dim param As String
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("POSLOVNIPROSTOR(", "")
                    param = param.Replace(")", "")

                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Me.GridEX1.GetCheckedRows()
                    If (checkedRows.Length = 0 Or checkedRows.Length = 1) Then
                        PokupiParametreSaGrida()
                        ID = GetID()
                        cls1.MasterID = ID
                        PoslovniProstor(param)
                    Else
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            GridEX1.MoveToRowIndex(row.RowIndex)
                            PokupiParametreSaGrida()
                            ID = GetID()
                            cls1.MasterID = ID
                            PoslovniProstor(param)
                        Next
                    End If



                ElseIf Ar5(i).ToLower.Contains("findnode") Then
                    Dim arr As TreeNode() = TreeView2.Nodes.Find("", True)
                    For i2 = 0 To arr.Length - 1
                        TreeView1.SelectedNode = arr(i2)
                        TreeView1.SelectedNode.Expand()
                        TreeView1.SelectedNode.BackColor = Color.Red
                    Next


                    'If cf.strFindNode.Trim <> "" Then
                    '    strFindNode2 = cf.strFindNode
                    '    Dim arr As TreeNode() = TreeView2.Nodes.Find(cf.strFindNode, True)
                    '    For i2 = 0 To arr.Length - 1
                    '        TreeView2.SelectedNode = arr(i2)
                    '        TreeView2.SelectedNode.Expand()
                    '        TreeView2.SelectedNode.BackColor = Color.Red
                    '    Next
                    '    ' cf.strFillExplorer = ""
                    '    ' TraziNoduDrvetupoTagu(cf.strFindNode)

                    'End If

                ElseIf Ar5(i).ToLower.Contains("refreshmovetoid") Then
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("5" + Err.Description)
                        Err.Clear()
                    End If
                    Dim tid5 As String = ID
                    PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("6" + Err.Description)
                        Err.Clear()
                    End If
                    ID = IDzaNoviSlog
                    cls1.MasterID = ID
                    Dim c5 As Janus.Windows.GridEX.GridEXColumn = GridEX1.RootTable.Columns(2)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("7" + Err.Description)
                        Err.Clear()
                    End If
                    If GridEX1.Find(c5, Janus.Data.ConditionOperator.Equal, cls1.SrediSQLPosljeNextForme(Ar5(i).ToLower.Replace("refreshmovetoid", "")), 0, 1) = False Then
                        'MsgBox("nisam nasao")
                    End If
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("8" + Err.Description)
                        Err.Clear()
                    End If
                    GridEX1.Focus()
                    GridEX1.EnsureVisible()
                    ID = tid5
                    cls1.MasterID = ID
                    ID = GetID()
                    cls1.MasterID = ID
                ElseIf Ar5(i).StartsWith(":") Then
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("9" + Err.Description)
                        Err.Clear()
                    End If
                    NovaGenerickaForma2(Ar5(i).ToString.Replace(":", ""))
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("10" + Err.Description)
                        Err.Clear()
                    End If
                ElseIf Ar5(i).StartsWith("==") Then

                    PokaziUpit(Ar5(i).Replace("==", ""))
                ElseIf Ar5(i).StartsWith("select") Then


                    On Error Resume Next
                    cls1.NapuniCBLog("Ušao u rafalni sql na podlozi : " + Ar5(i))
                    Dim Trs3 As New DataTable

                    strONLoad2 = cls1.SrediSQLPosljeNextForme(SrediSQL(Ar5(i)))
                    Trs3 = cls1.ExecSql(strONLoad2)

                    If Trs3.Rows.Count > 0 Then
                        DodatnoPostimajKontrolu(Button1, Trs3.Rows(0)(Button1.Text).ToString)
                        DodatnoPostimajKontrolu(Button2, Trs3.Rows(0)(Button2.Text).ToString)
                        DodatnoPostimajKontrolu(Button3, Trs3.Rows(0)(Button3.Text).ToString)
                        DodatnoPostimajKontrolu(Button4, Trs3.Rows(0)(Button4.Text).ToString)
                        DodatnoPostimajKontrolu(Button5, Trs3.Rows(0)(Button5.Text).ToString)
                        DodatnoPostimajKontrolu(Button6, Trs3.Rows(0)(Button6.Text).ToString)

                        DodatnoPostimajKontrolu(Button7, Trs3.Rows(0)(Button7.Text).ToString)
                        DodatnoPostimajKontrolu(Button8, Trs3.Rows(0)(Button8.Text).ToString)
                        DodatnoPostimajKontrolu(Button9, Trs3.Rows(0)(Button9.Text).ToString)
                        DodatnoPostimajKontrolu(Button10, Trs3.Rows(0)(Button10.Text).ToString)

                    End If
                    Err.Clear()

                Else
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("11" + Err.Description)
                        Err.Clear()
                    End If
                    NovaGenerickaForma2(Ar5(i).ToString.Replace(":", ""))
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("12" + Err.Description)
                        Err.Clear()
                    End If
                End If

            Next
        End If

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("13" + Err.Description)
            Err.Clear()
        End If

    End Sub



    Sub NovaGenerickaForma2(ByVal ImeForme As String)
        'cls1.NapuniCBLog("CALL NovaGenerickaForma2(" & ImeForme & ")")
        If ImeForme Is Nothing Then Exit Sub
        If ImeForme.Trim = "" Then Exit Sub

        TimerOff()

        If ImeForme.ToLower.StartsWith("multiselect") Or ImeForme.ToLower.StartsWith("multiblue") Then
            'cls1.ExecSql("if object_id('TotalGenMultiSelectTemp') is not null delete TotalGenMultiSelectTemp where hostname = host_name() and susersname= suser_sname()")
            cls1.ExecSql("if object_id('TotalGenMultiSelectTemp') is not null delete TotalGenMultiSelectTemp where susersname= suser_sname()")
            cls1.ExecSql(SrediSQL("if object_id('TotalGenMultiSelectTemp') is not null delete TotalGenMultiSelectTemp where operateriid= @@operateriid "))
        End If


        If ssPermissionProvjeriRafalnuKomandu(ImeForme) = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + ImeForme)
            Exit Sub
        End If


        If ImeForme.ToLower.StartsWith("printrepxmaster") Then
            'Button7_Click()
            IspisGRIDREPX()
            Exit Sub
        End If

        If ImeForme.ToLower.StartsWith("printmaster") Then
            'Button7_Click()
            'IspisNovi()
            IspisGRIDREPX()
            Exit Sub
        End If

        If ImeForme.ToLower.StartsWith("printdet") Then
            IspisGRIDREPXdet()
            'Button77_Click()
            Exit Sub
        End If

        Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
        checkedRows = Me.GridEX1.GetCheckedRows()

        'On Error Resume Next
        'Dim jj As Long = 0
        'If checkedRows.Length > 0 Then
        '    For Each row In checkedRows
        '        jj = row.RowIndex
        '    Next
        '    'jj = checkedRows(0).RowIndex
        '    If Err.Number <> 0 Then
        '        jj = 0
        '    Else
        '        jj = checkedRows.Length
        '    End If
        'End If
        Dim Tzadnji As Boolean = False
        Dim hh As Integer = 0
        Dim jj As Integer = 0

        'If Me.GridEX1.SelectedItems.Count > 1 Then ' 15.05.2009. automatski foreachbluerow
        '    Dim si(0) As Long
        '    Array.Resize(si, Me.GridEX1.SelectedItems.Count)
        '    For i = 0 To Me.GridEX1.SelectedItems.Count - 1
        '        si(i) = Me.GridEX1.SelectedItems(i).GetRow().RowIndex
        '    Next
        '    For i = 0 To si.Count - 1
        '        On Error Resume Next
        '        jj = jj + 1
        '        If jj = GridEX1.SelectedItems.Count Then Tzadnji = True
        '        Me.GridEX1.MoveToRowIndex(si(i))
        '        If Err.Number <> 0 Then
        '            NovaGenerickaForma1(ImeForme, Tzadnji)
        '            Exit For
        '        End If
        '        NovaGenerickaForma1(ImeForme, Tzadnji)
        '    Next
        'If (checkedRows.Length = 0 Or strEditColumns.Trim <> "") And Not ImeForme.ToLower.StartsWith("multiblue") Then

        If (checkedRows.Length = 0 Or checkedRows.Length = 1) And Not ImeForme.ToLower.StartsWith("multiblue") Then
            If ImeForme.ToLower.StartsWith("multiselect") Or ImeForme.ToLower.StartsWith("multiblue") Then
                cls1.ExecSql("insert into TotalGenMultiSelectTemp (id,hostname , susersname, operateriid )  select " & GetID() & ", host_name() as hostname , suser_sname() as susersname," & cls1.OperateriID)
                NovaGenerickaForma1(ImeForme)
            Else
                NovaGenerickaForma1(ImeForme)
            End If
        Else
            Dim row As Janus.Windows.GridEX.GridEXRow

            If ImeForme.ToLower.StartsWith("multiselect") Or ImeForme.ToLower.StartsWith("multiblue") Then

                If ImeForme.ToLower.StartsWith("multiselect") Then
                    For Each row In checkedRows
                        GridEX1.MoveToRowIndex(row.RowIndex)
                        cls1.ExecSql("insert into TotalGenMultiSelectTemp (id,hostname , susersname, operateriid )  select " & GetID() & ", host_name() as hostname , suser_sname() as susersname," & cls1.OperateriID)
                    Next
                End If

                If ImeForme.ToLower.StartsWith("multiblue") Then

                    Dim si(0) As Long
                    Array.Resize(si, GridEX1.SelectedItems.Count)
                    For i = 0 To GridEX1.SelectedItems.Count - 1
                        si(i) = GridEX1.SelectedItems(i).GetRow().RowIndex
                    Next

                    For i = 0 To si.Count - 1
                        GridEX1.MoveToRowIndex(si(i))
                        cls1.ExecSql("insert into TotalGenMultiSelectTemp (id,hostname , susersname, operateriid )  select " & GetID() & ", host_name() as hostname , suser_sname() as susersname," & cls1.OperateriID)
                    Next

                End If

                NovaGenerickaForma1(ImeForme)

            Else
                hh = checkedRows.Count
                For Each row In checkedRows
                    On Error Resume Next
                    jj = jj + 1

                    If jj = checkedRows.Count Then Tzadnji = True

                    GridEX1.MoveToRowIndex(row.RowIndex)
                    If Err.Number <> 0 Then
                        NovaGenerickaForma1(ImeForme, Tzadnji)
                        Exit For
                    End If
                    NovaGenerickaForma1(ImeForme, Tzadnji)
                Next
            End If

        End If

        TimerOn()

    End Sub

    Sub NovaGenerickaForma1(ByVal ImeForme As String, Optional ByVal Zadnji As Boolean = True)
        Dim Trs As New DataTable
        Dim Tid As String
        Dim Form2b As New Form2
        Dim tpP As String = ""
        Dim ImeForme5 As String
        Dim cf As Form2
        'cls1.NapuniCBLog("CALL NovaGenerickaForma1(" & ImeForme & ")")
        If ImeForme Is Nothing Then Exit Sub
        If ImeForme.Trim = "" Then Exit Sub
        Dim tNode As TreeNode
        Dim tNodeP As TreeNode

        Form2b.cls1 = cls1
        Form2b.Ikona = Ikona

        AktivnaFormaDetalj = ""
        AktivnaFormaMaster = ""
        Dim a() As String
        Dim strPutanja As String = ""
        Dim k As Integer = 0

        ''--------- 13.02.2009.
        'Form2b.strPrevParametri = ""
        'PokupiParametreSaGrida()
        'Form2b.strPrevParametri = ParametriSaGrida
        'cls1.strSviParametriuModulu = ParametriSaGrida
        ''-------------

        '-------------------------------------------------- 26.05.2011. za potrebe lokalnog ispisa preko word dot

        If ImeForme.StartsWith("WORDDOT1") Then
            cls1.NapuniCBLog("Ušao u poziv WORD DOT-a " & strPutanja)
            PokupiParametreSaGrida()
            arr(0) = GetID()

            'ImeForme = ImeForme.Replace("WORDDOT1", "")

            'a = ImeForme.Split("~")

            'For j = 0 To a.GetUpperBound(0)
            '    tpP = a(j)
            '    k = k + 1
            '    If k = 2 Then ' prvo ocekujem putanju
            '        strPutanja = cls1.SrediSQLPosljeNextForme(SrediSQL(tpP), ParametriSaGrida)

            '    End If
            'Next


            strPutanja = ImeForme.Replace("WORDDOT1", "")
            strPutanja = cls1.SrediSQLPosljeNextForme(SrediSQL(strPutanja), ParametriSaGrida)

            'Dim oWord As Word.Application
            'Dim oDoc As Word.Document

            Dim oWord As Object
            Dim oDoc As Object

            On Error Resume Next
            oWord = CreateObject("Word.Application")
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod CreateObject(""Word.Application"")  " & Err.Description)
                Exit Sub
            End If

            oWord.Visible = True

            Dim si(0) As String
            Array.Resize(si, GridEX1.SelectedItems.Count + 1)
            For i = 0 To GridEX1.SelectedItems.Count - 1
                si(i + 1) = GridEX1.SelectedItems(i).GetRow().RowIndex
            Next

            For i = 1 To si.Count - 1
                GridEX1.MoveToRowIndex(si(i))
                si(i) = GetID()
            Next

            cls1.NapuniCBLog("Zovem WORD DOT " & strPutanja)

            Err.Clear()
            oDoc = oWord.Documents.Add(strPutanja)
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod Word.Documents.Add ...  " & Err.Description)
                Exit Sub
            End If

            cls1.NapuniCBLog("ConnectionString " & "Provider=MSDASQL.1;Extended Properties=""DRIVER=SQL Server;" + Conn.ConnectionString + """")

            Err.Clear()
            oWord.Run("Pokreni", si, "Provider=MSDASQL.1;Extended Properties=""DRIVER=SQL Server;" + Conn.ConnectionString + """")
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod Word.run ...  " & Err.Description)
                Exit Sub
            End If

            Exit Sub
        End If


        ''-------------------------------------------------- 26.05.2011. za potrebe lokalnog ispisa RS-a
        'If ImeForme.StartsWith("RDLC") Then
        '    PokupiParametreSaGrida()
        '    arr(0) = GetID()
        '    ImeForme = ImeForme.Replace("RDLC", "")


        '    Dim b() As String

        '    Dim strDataSet As String = ""

        '    Dim Trs5 As New DataTable
        '    Dim sql As String = ""
        '    a = ImeForme.Split("~")

        '    Dim frm4 As New Form4

        '    For j = 0 To a.GetUpperBound(0)
        '        tpP = a(j)
        '        If a(j).Trim <> "" Then
        '            k = k + 1
        '            If k = 1 Then ' prvo ocekujem putanju
        '                strPutanja = cls1.SrediSQLPosljeNextForme(SrediSQL(tpP), ParametriSaGrida)
        '                frm4.ReportViewer1.LocalReport.ReportPath = strPutanja
        '            Else
        '                b = tpP.Split("$")
        '                For i = 0 To b.GetUpperBound(0)
        '                    If i = 0 Then
        '                        strDataSet = cls1.SrediSQLPosljeNextForme(SrediSQL(b(i)), ParametriSaGrida)
        '                    End If
        '                    If i = 1 Then
        '                        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(b(i)), ParametriSaGrida)
        '                        Err.Clear()
        '                        Trs5 = cls1.ExecSql(sql)
        '                        frm4.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource(strDataSet, Trs5))
        '                    End If
        '                Next
        '            End If
        '        End If
        '    Next
        '    frm4.ReportViewer1.LocalReport.EnableExternalImages = True
        '    frm4.ReportViewer1.LocalReport.EnableHyperlinks = True
        '    frm4.ReportViewer1.ZoomMode = 1
        '    frm4.ReportViewer1.SetDisplayMode(DisplayMode.Normal)
        '    frm4.ReportViewer1.Refresh()
        '    frm4.ShowDialog()
        '    Exit Sub
        'End If
        ''-------------------------------------------------------------------------------------------------



        If ImeForme.ToUpper.StartsWith("VRSTADOKID-") Or ImeForme.Substring(0, 3).ToUpper = "RSGEN" Or ImeForme.Substring(0, 3).ToUpper = "GG/" Or ImeForme.Substring(0, 3).ToUpper = "RS/" Or ImeForme.Substring(0, 3).ToUpper = "RS:" Or ImeForme.ToUpper.StartsWith("REPX:") Or ImeForme.ToUpper.StartsWith("REPX/") Or ImeForme.ToUpper.StartsWith("PR10/") Or ImeForme.ToUpper.StartsWith("PR10:") Or ImeForme.ToLower.Contains(".clsreport") Then
            PokupiParametreSaGrida()
            arr(0) = GetID()
            ID = GetID()

            If ImeForme.IndexOf("(") > 0 Then

                ImeForme5 = ImeForme.Trim

                ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
                tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
                tpP = cls1.SrediSQLPosljeNextForme(SrediSQL(tpP), ParametriSaGrida)
                'tpP = tpP.Replace("~~", ",")
                ' gogo trazio 18.12.2008.
                arr(0) = Val(tpP)
                If arr(0) = 0 Then arr(0) = ID
            End If


            ImeForme = ImeForme.Replace("rsgen", "rsgen:")
            ImeForme = ImeForme.Replace("RSGEN", "RSGEN:")
            ImeForme = ImeForme.Replace("rs/", "rs:/")
            ImeForme = ImeForme.Replace("RS/", "RS:/")
            ImeForme = ImeForme.Replace("repx/", "repx:")
            ImeForme = ImeForme.Replace("REPX/", "repx:")
            ImeForme = ImeForme.Replace("pr10/", "pr10:")
            ImeForme = ImeForme.Replace("pr10/", "pr10:")




            ImeForme = cls1.SrediSQLPosljeNextForme(SrediSQL(ImeForme), ParametriSaGrida)


            cls1.PrintRpt(ImeForme, tpP, Me.Handle)
            Exit Sub
        End If


        If ImeForme.Contains(".") Then
            Dim o As Object
            On Error Resume Next

            ID = GetID()
            cls1.MasterID = ID
            PokupiParametreSaGrida()
            tpP = ""
            If ImeForme.IndexOf("(") > 0 Then
                ImeForme5 = ImeForme.Trim
                ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
                tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
                tpP = cls1.SrediSQLPosljeNextForme(SrediSQL(tpP), ParametriSaGrida)
            End If
            Err.Clear()
            o = CreateObject(ImeForme, "")
            If Err.Number <> 0 Then
                MsgBox(Err.Description.ToString.Trim + "   " + ImeForme.ToString.Trim)
                Exit Sub
            End If

            o.id = ID

            o.totalgen(GetID().ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strZadnjiSQLsakojimjepunjenGrid.ToString, tpP)
            ID = o.id
            'MsgBox(ID)
            ' o.totalgen("1", "2", "3", "4", "5")


            If Err.Number <> 0 Then
                'MsgBox(Err.Description)
                cls1.NapuniCBLog(ImeForme & " .... PROBLEM : " & Err.Description)
                Exit Sub
            End If
            Exit Sub
        End If



        Form2b.ID = GetID()
        ID = GetID()
        Form2b.DetID = GetDetID()
        cls1.DetIDIzKlase = Form2b.DetID
        Form2b.Node = Node
        cls1.MasterID = Form2b.ID





        '--------- 25.11.2008.
        Form2b.strPrevParametri = ""
        PokupiParametreSaGrida()
        If ImeForme.IndexOf("(") > 0 Then ' 1.10.2009. dodao mogucnost da se forma zove sa parametrima npr. IMEFORME(@pero=@@id,@djuro=@@gridUvjetiklasa)
            ImeForme5 = ImeForme.Trim
            ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
            tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
            tpP = cls1.SrediSQLPosljeNextForme(SrediSQL(tpP), ParametriSaGrida)
            If tpP.Trim <> "" Then
                ' Dim a() As String
                a = tpP.Trim.Split(",")
                'ContextMenuStrip1.Items.Clear()
                For j = 0 To a.GetUpperBound(0)
                    If a(j).ToLower.Contains("@id") Then
                        ID = a(j).Replace("@id=", "").Replace("@ID=", "")
                        cls1.MasterID = ID
                        Form2b.ID = ID
                    End If
                Next
            End If
            ParametriSaGrida = tpP & "," & ParametriSaGrida
        End If
        Form2b.strPrevParametri = ParametriSaGrida
        cls1.strSviParametriuModulu = ParametriSaGrida
        '-------------


        cf = Form2b

        ImeForme = cls1.SrediSQLPosljeNextForme(SrediSQL(ImeForme))

        'Form2b.strPrevParametri = ""
        'PokupiParametreSaGrida()
        'Form2b.strPrevParametri = ParametriSaGrida

        Form2b.OKClick = False
        Form2b.BrojKontrola = 0
        Form2b.strXML = strXML2 & "\" & strXMLfile2
        Form2b.strXML = strXMLfile2
        Form2b.strXML2 = strXML2
        Form2b.strXMLfile2 = strXMLfile2
        Form2b.strImeForme = ImeForme
        Form2b.Owner = Me

        ' MsgBox(strXMLfile2)
        IDzaNoviSlog = ""
        Form2b.ProcitajXML()

        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do greške na ONLOAD forme idemo van

        If Form2b.BrojKontrola > 0 Then
            If Form2b.FormaJeVisibilna = False Then
                Form2b.PokupiParametre()
                Form2b.OKClick = True
            Else
                IDzaNoviSlog = Form2b.ID
                cls1.NapuniCBLog(" VALENTINO  Me.Handle: ")
                cls1.NapuniCBLog(" VALENTINO  Me.Handle: " & Me.Handle.ToString())
                Form2b.ShowDialog(Me)
                'If Form2b.ID <> IDzaNoviSlog Then
                '    IDzaNoviSlog = Form2b.ID
                'Else
                '    IDzaNoviSlog = ""
                'End If
            End If
        Else
            Form2b.OKClick = True
        End If


        'If Form2b.BrojKontrola > 0 Then
        '    Form2b.ShowDialog()
        'Else
        '    Form2b.OKClick = True
        'End If


        While cf.OKClick And cf.strNextForm <> ""

            Dim PrevParametri As String = IIf(cf.strParametri = "", cf.strPrevParametri, cf.strParametri)
            Dim tstrNextForm As String = cf.strNextForm
            Tid = cf.ID
            If cf.strStora <> "" Then
                Trs = cls1.ExecSql(cf.strStora & " " & cf.strParametri)
                If Not Trs Is Nothing Then
                    If Trs.Rows.Count > 0 Then
                        Tid = Trs.Rows(0)(0)
                    End If
                End If
            End If

            If cf.MasterMoveNext Then
                GridEX1.MoveNext()
                Tid = GetID()
            End If

            If cf.MasterMovePreviuos Then
                GridEX1.MovePrevious()
                Tid = GetID()
            End If

            IDzaNoviSlog = Tid

            cf.Close()

            PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)

            Dim Form2c As New Form2
            Form2c.cls1 = cls1
            cf = Form2c
            Form2c.ID = Tid
            cls1.MasterID = Tid
            Form2c.BrojKontrola = 0
            Form2c.strPrevParametri = PrevParametri
            Form2c.OKClick = False
            Form2c.strXML = strXML2 & "\" & strXMLfile2
            Form2c.strXML = strXMLfile2
            Form2c.strXML2 = strXML2
            Form2c.strXMLfile2 = strXMLfile2
            Form2c.strImeForme = tstrNextForm
            Form2c.ProcitajXML()
            If Form2c.BrojKontrola > 0 Then Form2c.ShowDialog() Else Form2c.OKClick = True
        End While


        If cf.OKClick And cf.strNextForm = "" Then


            If cf.strParametri.Trim <> "" Then   '----------- potrebno je izbaciti @grid parametre
                If InStr(cf.strParametri, "@grid") Then
                    cf.strParametri = cf.strParametri.Substring(0, InStr(cf.strParametri, ",@grid") - 1)
                End If
            End If


            If cf.strImeForme.Trim.ToUpper = "USERFILTER" Then
                cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL("insert into NetGenDBFilter (Naslov,OperaterID,Granula,sql) values('@@filternaslov',@@operateriid,'@@xmlname','" & cf.strStora.Replace("!!", "").Replace("'", "''") & "')")))

            End If


            If cf.strFillExplorer.ToLower = "true" Then
                If cf.strStora.Trim <> "" Then
                    strOpisniParametri = cf.strOpisniParametri
                    AktivnaFormaMaster = ImeForme
                    If cf.strStora.Contains("!!") Then
                        PokaziUpit(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora.Replace("!!", ""))))
                    Else
                        PokaziUpit(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora & " " & cf.strParametri)))
                    End If
                Else
                    ' MsgBox("FILLEXPLOER=TRUE ali STORA je prazan string !!!")
                End If

            ElseIf cf.strFillExplorer.ToLower = "false" Then
                If cf.strStora.Trim <> "" Then
                    Dim tdd As String = ""
                    tdd = cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora & " " & cf.strParametri))
                    If tdd.StartsWith(":") Then
                        NovaGenerickaForma(tdd)
                    Else
                        cls1.ExecSql(tdd)
                    End If


                End If
            ElseIf cf.strFillExplorer.ToLower = "onlydet" Then
                If cls1.strStartupStoraDetalj.ToString.Trim <> "" Then
                    AktivnaFormaDetalj = ImeForme
                    PokaziUpitDetalj(cls1.strStartupStoraDetalj)
                End If
            ElseIf cf.strFillExplorer.ToLower = "onlytree" Then
                NapuniDrvoVrijeme()
                ' TreeView2.ExpandAll()
            ElseIf cf.strFillExplorer.ToLower = "treeandrefreshmaster" Then

                'tNode = TreeView2.SelectedNode

                NapuniDrvoVrijeme()
                'TreeView2.ExpandAll()

                'TreeView2.SelectedNode = TreeView2.Nodes(1).Nodes(tNode.Name)
                'TreeView2.SelectedNode.EnsureVisible()

                If cf.strStora.Trim <> "" Then cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora & " " & cf.strParametri)))
                AktivnaFormaMaster = ImeForme
                If Zadnji Then PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
                TreeView2.Focus()
            ElseIf cf.strFillExplorer.ToLower = "treeandmaster" Then

                NapuniDrvoVrijeme()
                'TreeView2.ExpandAll()
                If cf.strStora.Trim <> "" Then
                    strOpisniParametri = cf.strOpisniParametri
                    AktivnaFormaMaster = ImeForme
                    If cf.strStora.Contains("!!") Then
                        PokaziUpit(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora.Replace("!!", ""))))
                    Else
                        PokaziUpit(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora & " " & cf.strParametri)))
                    End If
                Else
                    ' MsgBox("FILLEXPLOER=TRUE ali STORA je prazan string !!!")
                End If
                TreeView2.Focus()

            Else
                If cf.strStora.Trim <> "" Then
                    If cf.strStora.Contains("!!") Then
                        cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora.Replace("!!", ""))))
                    Else
                        cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(cf.strStora & " " & cf.strParametri)))
                    End If
                End If

                AktivnaFormaMaster = ImeForme
                If Zadnji Then PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
            End If


            strFindNode2 = ""
            If cf.strFindNode.Trim <> "" Then
                strFindNode2 = cf.strFindNode
                Dim arr As TreeNode() = TreeView2.Nodes.Find(cf.strFindNode, True)
                For i2 = 0 To arr.Length - 1
                    TreeView2.SelectedNode = arr(i2)
                    TreeView2.SelectedNode.Expand()
                    TreeView2.SelectedNode.BackColor = Color.Red
                Next
                ' cf.strFillExplorer = ""
                ' TraziNoduDrvetupoTagu(cf.strFindNode)

            End If

        End If


        cf.Close()

    End Sub

    Sub GetAllChildren(ByVal tag As String, ByVal parentNode As TreeNode)
        For Each childNode As TreeNode In parentNode.Nodes
            If tag.Trim.ToLower = childNode.Text.Trim.ToLower Then
                childNode.BackColor = Color.Pink
                TreeView2.SelectedNode = childNode
                TreeView2.SelectedNode.Expand()
            End If
            GetAllChildren(tag, childNode)
        Next
    End Sub

    Private Sub TraziNoduDrvetupoTagu(ByVal tag As String)
        For Each childNode As TreeNode In TreeView2.Nodes
            If tag.Trim.ToLower = childNode.Text.Trim.ToLower Then
                childNode.BackColor = Color.Pink
                TreeView2.SelectedNode = childNode
                TreeView2.SelectedNode.Expand()
            End If
            GetAllChildren(tag, childNode)
        Next
    End Sub

    Function GetID() As String
        GetID = "0"
        If DXGrid.Trim.ToLower = "true" Then
            GetID = GetDXID()
            Exit Function
        End If
        'If DataGridView1.Rows.Count > 0 Then
        If GridEX1.RowCount > 0 Then
            On Error Resume Next
            'GetID = DataGridView1.CurrentRow.Cells("id").Value.ToString.Trim
            GetID = GridEX1.CurrentRow.Cells("id").Value.ToString.Trim
            If Err.Number <> 0 Then
                'GetID = DataGridView1.CurrentRow.Cells(0).Value.ToString.Trim
                GetID = GridEX1.CurrentRow.Cells(2).Value.ToString.Trim
            End If
        Else
            GetID = "0"
        End If

    End Function


    Function GetDXID() As String
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




    Function GetDetID() As String
        GetDetID = "0"
        If DXGrid.Trim.ToLower = "true" Then
            GetDetID = GetDetDXID()
            Exit Function
        End If
        'If DataGridView1.Rows.Count > 0 Then
        If GridEX2.RowCount > 0 Then
            On Error Resume Next
            'GetID = DataGridView1.CurrentRow.Cells("id").Value.ToString.Trim
            GetDetID = GridEX2.CurrentRow.Cells("id").Value.ToString.Trim
            If Err.Number <> 0 Then
                'GetID = DataGridView1.CurrentRow.Cells(0).Value.ToString.Trim
                GetDetID = GridEX2.CurrentRow.Cells(2).Value.ToString.Trim
            End If
        Else
            GetDetID = "0"
        End If
    End Function

    Function GetDetDXID() As String
        GetDetDXID = "0"
        If GridView2.RowCount > 0 Then
            On Error Resume Next
            GetDetDXID = GridView2.GetFocusedRowCellValue("id")
            If Err.Number <> 0 Then
                GetDetDXID = GridView1.GetFocusedRowCellValue(GridView2.Columns(1).FieldName).ToString()
            End If
        Else
            GetDetDXID = "0"
        End If
    End Function

    Sub NovaApplikacija(ByVal Ime As String)
        strXMLfile2 = Ime
        ToolStripStatusLabel2.Text = ""
        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        ProcitajXMLzaPodlogu()
        Windows.Forms.Cursor.Current = Cursors.Default()
        'NapuniDrvoVrijeme()
    End Sub

    Private Sub TreeView1_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView1.AfterSelect
        ' strXMLfile2 = e.Node.Text.Trim.Substring(0, e.Node.Text.Trim.IndexOf(":")).Trim
        NovaApplikacija(e.Node.Tag)
        'strXMLfile2 = e.Node.Tag
        'ToolStripStatusLabel2.Text = ""
        'ProcitajXMLzaPodlogu()
        'NapuniDrvoVrijeme()
    End Sub

    Private Sub TreeView2_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TreeView2.DoubleClick
        On Error Resume Next
        Node = ""
        Node = TreeView2.SelectedNode.Tag.ToString.Substring(0, TreeView2.SelectedNode.Tag.ToString.IndexOf(":"))
        Dim ImeForme As String = TreeView2.SelectedNode.Tag.ToString.Substring(TreeView2.SelectedNode.Tag.ToString.IndexOf(":") + 1, TreeView2.SelectedNode.Tag.ToString.Length - TreeView2.SelectedNode.Tag.ToString.IndexOf(":") - 1)
        NovaGenerickaForma(ImeForme)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        cls1.NapuniCBLog("Button1_Click event")
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub DataGridView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles DataGridView1.DoubleClick
        NovaGenerickaForma(strDBlClickForm)
    End Sub

    Private Sub DataGridView1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles DataGridView1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            ContextMenuStrip1.Show(DataGridView1, DataGridView1.PointToClient(Windows.Forms.Cursor.Position))
        End If
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        Static i As Long = 0
        Dim tStr As String = ""
        tStr = sender.tag.ToString 'sender.ToString.ToLower
        Select Case tStr.ToLower
            Case "spremigrid"
                PokupiParametreSaGrida()
                SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
            Case "resetgrid"
                DeleteGridLayout()
            Case "exportgrid"
                Dim xcFileDialog As New SaveFileDialog
                xcFileDialog.FileName = "exportmaster.xls"
                Dim GridEXExporter2 As New Janus.Windows.GridEX.Export.GridEXExporter
                GridEXExporter2.GridEX = GridEX1
                If xcFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then

                    Dim FileToDelete As String
                    FileToDelete = xcFileDialog.FileName

                    If System.IO.File.Exists(FileToDelete) = True Then
                        System.IO.File.Delete(FileToDelete)
                    End If

                    Dim stream2 As New FileStream(xcFileDialog.FileName, FileMode.CreateNew, FileAccess.ReadWrite)
                    GridEXExporter2.Export(stream2)
                    stream2.Close()
                    System.Diagnostics.Process.Start(xcFileDialog.FileName)
                End If
            Case "groupungropu"
                If i = 0 Then
                    GridEX1.ExpandGroups()
                    i = 1
                Else
                    GridEX1.CollapseGroups()
                    i = 0
                End If
            Case "ispis"
                Button7_Click()
            Case "ispisodabranih"
                Button7_Click(True)
            Case "ispisdirektan"
                Button7_Click(False, True)
            Case "ispisodabranihdirektan"
                Button7_Click(True, True)
            Case "ispisnovi"
                IspisNovi()
            Case "ispisdx"
                IspisGRIDREPX()
            Case "format"
                On Error Resume Next
                Dim dialog As FormatStyles = New FormatStyles()
                dialog.Grid = GridEX1
                dialog.Show()
            Case "izgled"
                On Error Resume Next
                Dim dialog As AppearanceDialog = New AppearanceDialog()
                dialog.Grid = GridEX1
                dialog.Show()
            Case Else


                If tStr.Contains("~") Then
                    tStr = tStr.Replace("~", "#")



                    If tStr.ToLower.Contains("hidecolumns") Or tStr.ToLower.Contains("showcolumns") Then
                        Dim j As Integer
                        For j = 0 To ContextMenuStrip1.Items.Count
                            ContextMenuStrip1.Items(j).Enabled = True
                        Next

                        sender.enabled = False
                    End If

                    Rafalno(":" + tStr, sender, e)

                Else
                    NovaGenerickaForma(tStr)
                End If


        End Select
    End Sub


    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        Static i As Long = 0
        Dim tStr As String = ""
        tStr = sender.tag.ToString 'sender.ToString.ToLower
        Select Case tStr.ToLower
            Case "spremigrid"
                SaveGridLayoutDet()
            Case "resetgrid"
                DeleteGridLayoutDet()
            Case "exportgrid"
                Dim xcFileDialog As New SaveFileDialog
                xcFileDialog.FileName = "exportdetail.xls"
                Dim GridEXExporter2 As New Janus.Windows.GridEX.Export.GridEXExporter
                GridEXExporter2.GridEX = GridEX2
                If xcFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then

                    Dim FileToDelete As String
                    FileToDelete = xcFileDialog.FileName

                    If System.IO.File.Exists(FileToDelete) = True Then
                        System.IO.File.Delete(FileToDelete)
                    End If


                    Dim stream2 As New FileStream(xcFileDialog.FileName, FileMode.CreateNew, FileAccess.ReadWrite)
                    GridEXExporter2.Export(stream2)
                    stream2.Close()
                    System.Diagnostics.Process.Start(xcFileDialog.FileName)
                End If
            Case "groupungropu"
                If i = 0 Then
                    GridEX2.ExpandGroups()
                    i = 1
                Else
                    GridEX2.CollapseGroups()
                    i = 0
                End If
            Case "ispis"
                Button77_Click()
            Case "ispisnovi"
                IspisNoviDet()
            Case "ispisodabranih"
                Button77_Click(True)
            Case "ispisdx"
                IspisGRIDREPXdet()
            Case "format"
                On Error Resume Next
                Dim dialog As FormatStyles = New FormatStyles()
                dialog.Grid = GridEX2
                dialog.Show()
            Case "izgled"
                On Error Resume Next
                Dim dialog As AppearanceDialog = New AppearanceDialog()
                dialog.Grid = GridEX2
                dialog.Show()
            Case Else

                'NovaGenerickaForma1(tStr)


                If tStr.Contains("~") Then
                    tStr = tStr.Replace("~", "#")
                    Rafalno(":" + tStr, sender, e)
                Else
                    NovaGenerickaForma(tStr)
                End If


                'Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                'checkedRows = Me.GridEX2.GetCheckedRows()

                'If checkedRows.Length = 0 Then
                '    NovaGenerickaForma1(tStr)
                'Else

                '    Dim row As Janus.Windows.GridEX.GridEXRow
                '    For Each row In checkedRows
                '        On Error Resume Next
                '        GridEX2.MoveToRowIndex(row.RowIndex)
                '        If Err.Number <> 0 Then
                '            NovaGenerickaForma1(tStr)
                '            Exit For
                '        End If
                '        NovaGenerickaForma1(tStr)
                '    Next
                '    PokaziUpitDetalj("")
                'End If

        End Select
    End Sub



    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        cls1.NapuniCBLog("Button2_Click event")
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub TreeView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TreeView1.KeyDown


        If e.Control Then
            If e.KeyCode = Keys.F11 Then
                PokupiParametreSaGrida()
                cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
                'frm5.ShowDialog()
            End If
        End If
    End Sub

    Private Sub TreeView2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TreeView1.KeyDown
        If e.Control Then
            If e.KeyCode = Keys.F11 Then
                PokupiParametreSaGrida()
                cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
                'frm5.ShowDialog()
            End If
        End If
    End Sub

    Sub IspisNovi()
        PokupiParametreSaGrida()

        On Error Resume Next

        If DXGrid.Trim.ToLower = "true" Then
            ' GridControl1.ShowPrintPreview()
            Dim frm As frmIspis = New frmIspis(GridControl1, "ljevi" & vbCrLf & "cccc", "centrirano", "desno")
            frm.ShowDialog()
            Exit Sub
        End If



        GridEX1.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        GridEX1.RootTable.Columns(0).Visible = False


        Dim GP As New GridExPrinting.GridExPrinting()

        If strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GP.Orientation = GridExPrinting.Orientations.Landscape
        End If

        If strPageHeaderCenter.Trim = "" Then
            GP.PageHeaderCenter = Translate(Rafalna_Priprema(Me.Text.Replace("##", "!!").Substring(0, Me.Text.Length - (Me.Text.Length - Me.Text.IndexOf("v["))).Trim))
        Else
            GP.PageHeaderCenter = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), ParametriSaGrida, True).Replace("##", "!!")))
        End If

        GP.PageHeaderLeft = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), ParametriSaGrida, True).Replace("##", "!!")))
        GP.PageHeaderRight = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), ParametriSaGrida, True).Replace("##", "!!")))


        GridEXPrintDocument1.PrintCollapsedRows = False

        GP.JanusPrintDocument(GridEXPrintDocument1, Conn.ConnectionString)



    End Sub


    Sub IspisNoviDet()
        PokupiParametreSaGrida()

        GridEX2.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        GridEX2.RootTable.Columns(0).Visible = False


        Dim GP As New GridExPrinting.GridExPrinting()

        If strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GP.Orientation = GridExPrinting.Orientations.Landscape
        End If

        'If strPageHeaderCenter.Trim = "" Then
        '    GP.PageHeaderCenter = Rafalna_Priprema(Me.Text.Replace("##", "!!"))
        'Else
        '    GP.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), ParametriSaGrida, True).Replace("##", "!!"))
        'End If

        'GP.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), ParametriSaGrida, True).Replace("##", "!!"))
        'GP.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), ParametriSaGrida, True).Replace("##", "!!"))


        GridEXPrintDocument2.PrintCollapsedRows = False

        GP.JanusPrintDocument(GridEXPrintDocument2, Conn.ConnectionString)



    End Sub

    Sub IspisGRIDREPX()
        PokupiParametreSaGrida()
        'On Error Resume Next
        On Error Resume Next
        Dim a As Assembly = Assembly.Load("NetGenExtensions")

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(NetGenExtensions) : " & Err.Description)
        End If

        Dim o As Object = a.CreateInstance("NetGenExtensions.frmGridExPrintingREPX")
        o.PageHeaderLeft = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        If strPageHeaderCenter.Trim = "" Then
            o.PageHeaderCenter = Translate(Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf).Substring(0, Me.Text.Length - (Me.Text.Length - Me.Text.IndexOf("v["))).Trim)
        Else
            o.PageHeaderCenter = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        End If
        o.PageHeaderRight = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        'o.PrintDocumentREPX("master", GridEX1, m_xmld, Nothing, strZadnjiSQLsakojimjepunjenGrid, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, ParametriSaGrida, "")
        o.PrintDocumentREPX("master", GridEX1, m_xmld, Nothing, strZadnjiSQLsakojimjepunjenGrid, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + strGlobalSQLSERVERlogin, ParametriSaGrida, "")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod DX ispisa master grida : " & Err.Description)
        End If
    End Sub

    Private Sub Button7_Click(Optional ByVal SamoOdabrani As Boolean = False, Optional ByVal DirektanIspis As Boolean = False)


        On Error Resume Next

        PokupiParametreSaGrida()

        GridEX1.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        GridEX1.RootTable.Columns(0).Visible = False

        If SamoOdabrani Then
            GridEXPrintDocument1.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.Selection
        Else
            GridEXPrintDocument1.PrinterSettings.PrintRange = 0
        End If

        If strPageHeaderCenter.Trim = "" Then
            GridEXPrintDocument1.PageHeaderCenter = Rafalna_Priprema(Me.Text.Replace("##", vbCrLf)).Replace("!!", vbCrLf)
        Else
            GridEXPrintDocument1.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)
        End If

        GridEXPrintDocument1.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)
        GridEXPrintDocument1.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)

        GridEXPrintDocument1.PageFooterRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterRight), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)
        GridEXPrintDocument1.PageFooterLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterLeft), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)
        GridEXPrintDocument1.PageFooterCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterCenter), ParametriSaGrida, True).Replace("##", vbCrLf)).Replace("!!", vbCrLf)

        GridEXPrintDocument1.FitColumns = FitColumnsMode.SizingColumns
        If strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GridEXPrintDocument1.DefaultPageSettings.Landscape = System.Windows.Forms.Orientation.Vertical
        Else
            GridEXPrintDocument1.DefaultPageSettings.Landscape = System.Windows.Forms.Orientation.Horizontal
        End If

        GridEXPrintDocument1.PrepareDocument()
        PrintPreviewDialog1.Parent = Me
        PrintPreviewDialog1.BringToFront()
        'PrintPreviewDialog1.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
        PrintPreviewDialog1.Width = Me.Width - 200
        PrintPreviewDialog1.Height = Me.Height - 200


        If DirektanIspis Then
            Dim printDialog As New PrintDialog()
            printDialog.Document = GridEXPrintDocument1
            If printDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                GridEXPrintDocument1.Print()
            End If
        Else
            PrintPreviewDialog1.ShowDialog()
        End If



    End Sub

    Sub IspisGRIDREPXdet()
        PokupiParametreSaGrida()
        'On Error Resume Next
        On Error Resume Next
        Dim a As Assembly = Assembly.Load("NetGenExtensions")

        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(NetGenExtensions) : " & Err.Description)
        End If

        Dim o As Object = a.CreateInstance("NetGenExtensions.frmGridExPrintingREPX")
        'o.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'If strPageHeaderCenter.Trim = "" Then
        '    o.PageHeaderCenter = Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf)
        'Else
        '    o.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'End If
        'o.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        o.PrintDocumentREPX("detail", GridEX2, m_xmld, Nothing, strZadnjiSQLsakojimjepunjenGrid, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, ParametriSaGrida, "")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod DX ispisa detail grida : " & Err.Description)
        End If
    End Sub

    Private Sub Button77_Click(Optional ByVal SamoOdabrani As Boolean = False)

        On Error Resume Next

        PokupiParametreSaGrida()

        GridEX2.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        GridEX2.RootTable.Columns(0).Visible = False

        If SamoOdabrani Then
            GridEXPrintDocument2.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.Selection
        Else
            GridEXPrintDocument2.PrinterSettings.PrintRange = 0
        End If

        If strPageHeaderCenter.Trim = "" Then
            GridEXPrintDocument2.PageHeaderCenter = Rafalna_Priprema(Me.Text.Replace("##", vbCrLf))
        Else
            GridEXPrintDocument2.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderCenter), ParametriSaGrida).Replace("##", vbCrLf))
        End If

        'If strPageHeaderLeft.Trim = "" Then
        '    GridEXPrintDocument2.PageHeaderLeft = strOpisniParametri.Replace("##", vbCrLf)
        'Else
        '    GridEXPrintDocument2.PageHeaderLeft = cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), ParametriSaGrida).Replace("##", vbCrLf)
        'End If

        GridEXPrintDocument2.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderLeft), ParametriSaGrida, True).Replace("##", vbCrLf))

        GridEXPrintDocument2.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageHeaderRight), ParametriSaGrida, True).Replace("##", vbCrLf))

        GridEXPrintDocument2.PageFooterRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterRight), ParametriSaGrida, True).Replace("##", vbCrLf))
        GridEXPrintDocument2.PageFooterLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterLeft), ParametriSaGrida, True).Replace("##", vbCrLf))
        GridEXPrintDocument2.PageFooterCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL(strPageFooterCenter), ParametriSaGrida, True).Replace("##", vbCrLf))

        'GridEXPrintDocument1.FitColumns = FitColumnsMode.SizingColumns
        GridEXPrintDocument2.FitColumns = FitColumnsMode.SizingColumns
        If strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GridEXPrintDocument2.DefaultPageSettings.Landscape = System.Windows.Forms.Orientation.Vertical
        Else
            GridEXPrintDocument2.DefaultPageSettings.Landscape = System.Windows.Forms.Orientation.Horizontal
        End If

        GridEXPrintDocument2.PrepareDocument()
        PrintPreviewDialog2.BringToFront()
        PrintPreviewDialog2.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
        PrintPreviewDialog2.ShowDialog()
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        PokupiParametreSaGrida()
        'strCUSTOMIMASTERGRIDLAYOUTFILE = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
        'SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
    End Sub

    Private Sub ApperianceDialogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ApperianceDialogToolStripMenuItem.Click
        On Error Resume Next
        Dim dialog As AppearanceDialog = New AppearanceDialog()
        dialog.Grid = GridEX1
        dialog.Show()
    End Sub


    Private Sub FormatStylesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormatStylesToolStripMenuItem.Click
        On Error Resume Next
        Dim dialog As FormatStyles = New FormatStyles()
        dialog.Grid = GridEX1
        dialog.Show()
    End Sub

    Private Sub TableViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TableViewToolStripMenuItem.Click
        On Error Resume Next
        GridEX1.View = Janus.Windows.GridEX.View.TableView
        GridEX1.RootTable.Columns("Icon").Visible = True
        GridEX1.RecordNavigator = False
        GridEX1.NewRowPosition = NewRowPosition.TopRow
    End Sub

    Private Sub CardViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CardViewToolStripMenuItem.Click
        On Error Resume Next
        GridEX1.View = Janus.Windows.GridEX.View.CardView
        GridEX1.RootTable.Columns("Icon").Visible = False
        GridEX1.ExpandableCards = True
        GridEX1.RecordNavigator = False
    End Sub

    Private Sub SingleCardViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SingleCardViewToolStripMenuItem.Click
        On Error Resume Next
        GridEX1.View = Janus.Windows.GridEX.View.SingleCard
        GridEX1.RootTable.Columns("Icon").Visible = False
        GridEX1.ExpandableCards = False
        GridEX1.RecordNavigator = True
        GridEX1.NewRowPosition = NewRowPosition.BottomRow
    End Sub

    Private Sub SaveGridToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveGridToolStripMenuItem.Click
        PokupiParametreSaGrida()
        'strCUSTOMIMASTERGRIDLAYOUTFILE = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
        SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
    End Sub

    Private Sub LoadLayout()
        On Error Resume Next
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim

        If strCUSTOMIMASTERGRIDLAYOUTFILE <> "" Then
            PokupiParametreSaGrida()
            'strCUSTOMIMASTERGRIDLAYOUTFILE = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
            ImeFajla = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
        End If


        Dim tempAktivnaFormaMaster As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaMaster = AktivnaFormaMaster
        End If
        Dim LayoutDir As String = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".gxl"
        Dim LayoutDir2 As String = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".xml"

        cls1.NapuniCBLog("LOADGRIDLAYOUT : " & LayoutDir)
        If My.Computer.FileSystem.FileExists(LayoutDir) Then
            If DXGrid.Trim.ToLower = "true" Then
                GridView1.RestoreLayoutFromXml(LayoutDir2)
            Else
                Dim LayoutStream As FileStream
                LayoutStream = New FileStream(LayoutDir, FileMode.Open)
                GridEX1.LoadLayoutFile(LayoutStream)
                LayoutStream.Close()
                If strFilterSave.ToLower <> "true" Then GridEX1.RemoveFilters()
            End If

        End If
    End Sub


    Private Sub LoadLayout2()
        On Error Resume Next
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        Dim tempAktivnaFormaDetalj As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaDetalj = AktivnaFormaDetalj
        End If
        Dim LayoutDETDir As String = GetLayoutDirectory() + "\" + PrefixZaSpremanjeDetaljGrida + ImeFajla + tempAktivnaFormaDetalj + "-" + JupiterTranslatorMPJezik + ".gxl"
        If My.Computer.FileSystem.FileExists(LayoutDETDir) Then
            Dim LayoutStream As FileStream
            LayoutStream = New FileStream(LayoutDETDir, FileMode.Open)
            GridEX2.LoadLayoutFile(LayoutStream)
            LayoutStream.Close()
        End If
    End Sub

    Sub SaveGridLayout(ByVal CustomImeFajla As String)
        On Error Resume Next
        Dim LayoutDir As String
        Dim LayoutDir2 As String
        Dim LayoutStream As FileStream
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        'MsgBox(Err.Description)
        'MsgBox(strXMLfile2)
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        If CustomImeFajla <> "" Then
            ImeFajla = CustomImeFajla
        End If
        cls1.NapuniCBLog(Err.Description)
        cls1.NapuniCBLog("strXMLfile2 : " & strXMLfile2)
        cls1.NapuniCBLog("ImeFajla : " & ImeFajla)
        Dim tempAktivnaFormaMaster As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaMaster = AktivnaFormaMaster
        End If
        LayoutDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".gxl"
        LayoutDir2 = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".xml"
        cls1.NapuniCBLog("SAVEGRIDLAYOUT : " & LayoutDir)



        If DXGrid.Trim.ToLower = "true" Then
            If strFilterMode.ToLower.Trim = "true" Then
                GridEX1.FilterMode = FilterMode.None
                GridEX1.RemoveFilters()
            End If

            GridView1.SaveLayoutToXml(LayoutDir2)

            If strFilterMode.ToLower.Trim = "true" Then
                GridEX1.FilterMode = FilterMode.Automatic
            End If
        Else
            LayoutStream = New FileStream(LayoutDir, FileMode.Create)

            If strFilterMode.ToLower.Trim = "true" Then ' vfratio 27.04.2023. zbog Vlatke
                GridEX1.FilterMode = FilterMode.None
                GridEX1.RemoveFilters()
            End If

            Err.Clear()
            GridEX1.SaveLayoutFile(LayoutStream)
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod spremanja : " & Err.Description)
            End If
            LayoutStream.Close()

            If strFilterMode.ToLower.Trim = "true" Then
                'GridEX1.FilterMode = FilterMode.Automatic
            End If

        End If

        'Dim LayoutDETDir As String
        'Dim LayoutDETStream As FileStream
        'LayoutDETDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeDetaljGrida + ImeFajla + AktivnaFormaDetalj + ".gxl"
        'LayoutDETStream = New FileStream(LayoutDETDir, FileMode.Create)
        '' GridEX2.SaveLayoutFile(LayoutDETStream)
        'LayoutDETStream.Close()
    End Sub

    Private Sub LoadLayoutTEMPORARNI()
        On Error Resume Next
        Dim ImeFajla As String = "TEMPORARNIgridsettingsi"
        Dim tempAktivnaFormaMaster As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaMaster = AktivnaFormaMaster
        End If
        Dim LayoutDir As String = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + ".gxl"
        If My.Computer.FileSystem.FileExists(LayoutDir) Then
            Dim LayoutStream As FileStream
            LayoutStream = New FileStream(LayoutDir, FileMode.Open)
            GridEX1.LoadLayoutFile(LayoutStream)
            LayoutStream.Close()
        End If
    End Sub

    Sub SaveGridLayoutTEMPORARNI()
        On Error Resume Next
        Dim LayoutDir As String
        Dim LayoutStream As FileStream
        Dim ImeFajla As String = "TEMPORARNIgridsettingsi"
        Dim tempAktivnaFormaMaster As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaMaster = AktivnaFormaMaster
        End If
        LayoutDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + ".gxl"
        LayoutStream = New FileStream(LayoutDir, FileMode.Create)
        GridEX1.SaveLayoutFile(LayoutStream)
        LayoutStream.Close()
    End Sub


    Sub DeleteGridLayout()
        On Error Resume Next
        Dim LayoutDir As String
        Dim LayoutDir2 As String

        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        Dim tempAktivnaFormaMaster As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaMaster = AktivnaFormaMaster
        End If

        If strCUSTOMIMASTERGRIDLAYOUTFILE <> "" Then
            PokupiParametreSaGrida()
            'strCUSTOMIMASTERGRIDLAYOUTFILE = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
            ImeFajla = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
        End If

        LayoutDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".gxl"
        LayoutDir2 = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + tempAktivnaFormaMaster + "-" + JupiterTranslatorMPJezik + ".xml"

        cls1.NapuniCBLog("Brišem postavke master grida : " & LayoutDir)
        Err.Clear()

        If DXGrid.Trim.ToLower = "true" Then
            My.Computer.FileSystem.DeleteFile(LayoutDir2, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Else
            My.Computer.FileSystem.DeleteFile(LayoutDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod brisanja : " & Err.Description)
        End If

        'Dim LayoutDETDir As String
        'LayoutDETDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeDetaljGrida + ImeFajla + AktivnaFormaDetalj + ".gxl"
        '' My.Computer.FileSystem.DeleteFile(LayoutDETDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        GridEX1.ClearStructure()
        GridEX1.ClearItems()
        PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
    End Sub

    Sub SaveGridLayoutDet()
        On Error Resume Next
        Dim LayoutDir As String
        Dim LayoutStream As FileStream
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        'LayoutDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + AktivnaFormaMaster + ".gxl"
        'LayoutStream = New FileStream(LayoutDir, FileMode.Create)
        ''GridEX1.SaveLayoutFile(LayoutStream)
        'LayoutStream.Close()
        Dim LayoutDETDir As String
        Dim LayoutDETStream As FileStream
        Dim tempAktivnaFormaDetalj As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaDetalj = AktivnaFormaDetalj
        End If
        LayoutDETDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeDetaljGrida + ImeFajla + tempAktivnaFormaDetalj + "-" + JupiterTranslatorMPJezik + ".gxl"
        LayoutDETStream = New FileStream(LayoutDETDir, FileMode.Create)
        GridEX2.SaveLayoutFile(LayoutDETStream)
        LayoutDETStream.Close()
    End Sub


    Sub DeleteGridLayoutDet()
        On Error Resume Next
        Dim LayoutDir As String
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        'LayoutDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeMasterGrida + ImeFajla + AktivnaFormaMaster + ".gxl"
        ' My.Computer.FileSystem.DeleteFile(LayoutDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Dim LayoutDETDir As String
        Dim tempAktivnaFormaDetalj As String = ""
        If MultiGridLayout Then
            tempAktivnaFormaDetalj = AktivnaFormaDetalj
        End If
        LayoutDETDir = GetLayoutDirectory() + "\" + PrefixZaSpremanjeDetaljGrida + ImeFajla + tempAktivnaFormaDetalj + "-" + JupiterTranslatorMPJezik + ".gxl"
        My.Computer.FileSystem.DeleteFile(LayoutDETDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        GridEX1.ClearStructure()
        GridEX1.ClearItems()
        PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
    End Sub



    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Static i As Long
        If i = 0 Then
            GridEX1.ExpandGroups()
            i = 1
        Else
            GridEX1.CollapseGroups()
            i = 0
        End If

    End Sub

    Private Sub ResetGridLayoutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetGridLayoutToolStripMenuItem.Click
        ResetGridLayout = True
        PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
        ResetGridLayout = False
        PokupiParametreSaGrida()
        'strCUSTOMIMASTERGRIDLAYOUTFILE = cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE))
        SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
    End Sub

    Private Sub GridExportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GridExportToolStripMenuItem.Click
        Dim xcFileDialog As New SaveFileDialog
        xcFileDialog.FileName = "export.xls"

        If xcFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim stream2 As New FileStream(xcFileDialog.FileName, FileMode.CreateNew, FileAccess.ReadWrite)
            GridEXExporter1.Export(stream2)
            stream2.Close()
            System.Diagnostics.Process.Start(xcFileDialog.FileName)
        End If

    End Sub

    Private Sub GridEX1_CellEdited(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.ColumnActionEventArgs) Handles GridEX1.CellEdited
        ' Dim i As Integer
    End Sub


    Private Sub GridEX1_CellUpdated(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.ColumnActionEventArgs) Handles GridEX1.CellUpdated
        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        ID = GetID()
        cls1.MasterID = ID
        PokupiParametreSaGrida()

        TrenutnaKolona = sender.CurrentColumn
        TrenutnaVrijednost = ""

        If sender.RowCount > 0 Then



            If sender.CurrentColumn.Caption.ToString <> "" Then
                If sender.CurrentRow.Cells(sender.CurrentColumn).Value Is Nothing Then

                Else
                    If Val(sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString) <> 0 Then
                        TrenutnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.Replace(",", ".")
                    Else
                        TrenutnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString
                    End If
                End If
            End If



            For i = 1 To sender.RootTable.Columns.Count - 1
                If sender.CurrentRow.Cells(i).Column.DataMember.ToString <> "" Then
                    If sender.CurrentRow.Cells(i).Value Is Nothing Then

                    Else

                        Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        End If
                    End If
                End If
            Next
        End If

        sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        Dim sql As String = ""

        'sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strONCellEditedstr), , , False)

        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strONCellEditedstr), ParametriSaGrida, , False)

        Rafalno(sql, sender, e)

        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
            sender.CurrentRow.Cells(sender.CurrentColumn).Value = PredhodnaVrijednost
        End If

        ' GridEX1.SearchColumn = Nothing

        'GridEX1.RootTable.col()

        'cls1.ExecSql(sql)

    End Sub



    Sub Rafalno(ByVal stR As String, ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        Dim param As String = ""
        Dim Ar5() As String
        Dim Ar6() As String
        Dim i As Long
        Dim strOriginalSQL As String = ""
        If stR.ToString.Trim <> "" Then

            Ar5 = Split(stR.ToString, "#")

            For i = 0 To UBound(Ar5)

                Ar5(i) = Ar5(i).Replace(":HIDECOLUMNS", "HIDECOLUMNS")

                If Ar5(i).Trim.ToLower.StartsWith(":select") Then
                    Ar5(i) = Ar5(i).Replace(":select", "select")
                End If

                If Ar5(i).Trim.Contains("@@") Then ' 30.10.2009. omogucuje programiranje kljucnih rijeci, npr. @@stanjeforme se negdje napuni sa CLOSE ....
                    'PokupiParametre()
                    'Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar5(i)), strParametriSvi)

                    PokupiParametreSaGrida()
                    strOriginalSQL = Ar5(i)
                    Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL(Ar5(i)), ParametriSaGrida)

                End If

                If Ar5(i).Trim = "" Then
                    i = i
                ElseIf Ar5(i).Trim.ToUpper = "NULL" Then
                    i = i
                ElseIf Ar5(i).Trim.ToUpper.Contains("BEEP()") Then
                    Microsoft.VisualBasic.Interaction.Beep()
                    System.Threading.Thread.Sleep(300)
                ElseIf Ar5(i).Trim.ToUpper.Contains("OPENFORM(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("OPENFORM(", "")
                    param = param.Replace(")", "")
                    NovaGenerickaForma2(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("BLAGAJNAENABLE(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("BLAGAJNAENABLE(", "")
                    param = param.Replace(")", "")
                    cls1.BlagajnaEnableZaMlinar(cls1.SrediSQLPosljeNextForme(SrediSQL(param)))
                ElseIf Ar5(i).Trim.ToUpper.Contains("STARTSEARCH(") Then
                    'param = Ar5(i).ToUpper.Trim
                    'param = param.Replace("STARTSEARCH(", "")
                    'param = param.Replace(")", "")
                    'Ar6 = Split(param.ToString, ",")
                    'Controls(Ar6(0)).Focus()
                    'Controls(Ar6(0)).Select()
                    'Controls(Ar6(0)).Text = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar6(1)), strParametriSvi)
                    'Glbl_Validated(Controls(Ar6(0)), Nothing)

                ElseIf Ar5(i).Trim.ToUpper.Contains(":ASSEMBLYLOAD(") Then

                    ''param = Ar5(i).ToUpper.Trim
                    ''param = param.Replace("ASSEMBLYLOAD(", "")
                    ''param = param.Replace(")", "")

                    ''Dim a As Assembly = Assembly.Load("hrkadroviscan")
                    ''If Err.Number <> 0 Then
                    ''    cls1.NapuniCBLog("Greška kod Assembly.Load(hrkadroviscan) : " & Err.Description)
                    ''    MsgBox(Err.Description)
                    ''    Exit Sub
                    ''End If

                    ''Err.Clear()
                    ''Dim o As Object = a.CreateInstance(param)
                    ''If Err.Number <> 0 Then
                    ''    cls1.NapuniCBLog("Greška kod CreateInstance (hrkadroviscan.clsscan) : " & Err.Description)
                    ''    MsgBox(Err.Description)
                    ''    Exit Sub
                    ''End If

                    ''o.totalgen(GetID().ToString, cls1.OperateriID.ToString, Conn.ConnectionString.ToString, strZadnjiSQLsakojimjepunjenGrid.ToString, "")

                    cls1.NapuniCBLog("ušao u  Assembly.Load(" & Ar5(i).Trim)

                    PokupiParametreSaGrida()
                    ID = GetID()
                    cls1.MasterID = ID

                    param = Ar5(i).Trim
                    param = param.Replace(":ASSEMBLYLOAD(", "")
                    param = param.Replace(")", "")

                    Err.Clear()
                    Dim a As Assembly = Assembly.Load(param.Substring(0, InStrRev(param, ".") - 1))
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod Assembly.Load(" & param.Substring(0, InStrRev(param, ".") - 1) & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If

                    Err.Clear()
                    Dim o As Object = a.CreateInstance(param)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod CreateInstance (" & param & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If


                    Dim tconstr As String = Conn.ConnectionString.ToString
                    If tconstr.ToLower.Contains("driver") Then

                    Else
                        tconstr = "Driver={SQL Server};" & tconstr
                    End If

                    cls1.NapuniCBLog("zovem totalgen u asembliju ...")

                    o.totalgen(ID, cls1.OperateriID.ToString, tconstr, strZadnjiSQLsakojimjepunjenGrid.ToString, ParametriSaGrida)

                    cls1.NapuniCBLog("nakon totalgen u asembliju ..." & Err.Description)

                ElseIf Ar5(i).Trim.Substring(0, 1) = ":" Then
                    NovaGenerickaForma2(Ar5(i).ToString.Replace(":", ""))
                ElseIf Ar5(i).Trim.ToUpper = "OKSTAY" Then
                    'NeZelimClosatFormu = True
                    Button1_Click(sender, e)
                    'NeZelimClosatFormu = False
                ElseIf Ar5(i).Trim.ToUpper = "OK" Then
                    'NeZelimClosatFormu = False
                    Button1_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper = "REFRESHMASTERGRID" Then
                    PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)

                ElseIf Ar5(i).Trim.ToUpper.Contains("REFRESHCONTROL(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("REFRESHCONTROL(", "")
                    param = param.Replace(")", "")
                ElseIf Ar5(i).Trim.ToUpper.Contains("ASSEMBLYLOAD(") Then

                    ''param = Ar5(i).ToUpper.Trim
                    ''param = param.Replace("ASSEMBLYLOAD(", "")
                    ''param = param.Replace(")", "")

                    ''Dim a As Assembly = Assembly.Load("hrkadroviscan")
                    ''If Err.Number <> 0 Then
                    ''    cls1.NapuniCBLog("Greška kod Assembly.Load(hrkadroviscan) : " & Err.Description)
                    ''    MsgBox(Err.Description)
                    ''    Exit Sub
                    ''End If

                    ''Err.Clear()
                    ''Dim o As Object = a.CreateInstance(param)
                    ''If Err.Number <> 0 Then
                    ''    cls1.NapuniCBLog("Greška kod CreateInstance (hrkadroviscan.clsscan) : " & Err.Description)
                    ''    MsgBox(Err.Description)
                    ''    Exit Sub
                    ''End If

                    ''o.totalgen(GetID().ToString, cls1.OperateriID.ToString, Conn.ConnectionString.ToString, strZadnjiSQLsakojimjepunjenGrid.ToString, "")

                    PokupiParametreSaGrida()
                    ID = GetID()
                    cls1.MasterID = ID

                    param = Ar5(i).Trim
                    param = param.Replace("ASSEMBLYLOAD(", "")
                    param = param.Replace(")", "")

                    Dim a As Assembly = Assembly.Load(param.Substring(0, InStrRev(param, ".") - 1))
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod Assembly.Load(" & param.Substring(0, InStrRev(param, ".") - 1) & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If

                    Err.Clear()
                    Dim o As Object = a.CreateInstance(param)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod CreateInstance (" & param & ") : " & Err.Description)
                        'MsgBox(Err.Description)
                        Exit Sub
                    End If

                    o.totalgen(ID, cls1.OperateriID.ToString, Conn.ConnectionString.ToString, strZadnjiSQLsakojimjepunjenGrid.ToString, "")


                ElseIf Ar5(i).Trim.ToUpper.Contains("HIDECOLUMNS(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("HIDECOLUMNS(", "")
                    param = param.Replace(")", "")
                    Dim Ar88() As String
                    Dim i2 As Integer
                    Dim imeGrida As String = ""
                    Ar88 = Split(param.ToString, ",")
                    For i2 = 0 To UBound(Ar88)

                        If i2 = 0 Then
                            imeGrida = Ar88(i2)
                        End If

                        If imeGrida.ToLower = "mastergrid" Then
                            GridEX1.RootTable.Columns(Ar88(i2)).Visible = False
                        End If

                    Next
                ElseIf Ar5(i).Trim.ToUpper.Contains("SHOWCOLUMNS(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("SHOWCOLUMNS(", "")
                    param = param.Replace(")", "")
                    Dim Ar88() As String
                    Dim i2 As Integer
                    Dim imeGrida As String = ""
                    Ar88 = Split(param.ToString, ",")
                    For i2 = 0 To UBound(Ar88)

                        If i2 = 0 Then
                            imeGrida = Ar88(i2)
                        End If

                        If imeGrida.ToLower = "mastergrid" Then
                            GridEX1.RootTable.Columns(Ar88(i2)).Visible = True
                        End If

                    Next
                ElseIf Ar5(i).Trim.ToUpper.Contains("ISEMPTY(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("ISEMPTY(", "")
                    param = param.Replace(")", "")
                    If param.ToLower = "null" Or param.Trim = "" Then
                        i = i + 1
                    Else

                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("PRINTGRID(") Then
                    'param = Ar5(i).ToUpper.Trim
                    'param = param.Replace("PRINTGRID(", "")
                    'param = param.Replace(")", "")
                    'TempGrid = Controls(param)
                    ''Ispis(False, True)
                    'IspisNovi()

                ElseIf Ar5(i).Trim.ToUpper.Contains("GRID_COLUMN_VALUE(") Then
                    'Dim TempGrid2 As Grid
                    'On Error Resume Next
                    'param = Ar5(i).ToUpper.Trim
                    'param = param.Replace("GRID_COLUMN_VALUE(", "")
                    'param = param.Replace(")", "")
                    'PokupiParametre()
                    'param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    'Dim a() As String
                    'a = param.Split(",")
                    'TempGrid2 = Controls(a(0))

                    'On Error Resume Next
                    'If TempGrid2.RowCount > 0 Then
                    '    Dim c4 As GridEXColumn = TempGrid2.RootTable.Columns(a(1))
                    '    If Err.Number = 0 Then
                    '        Err.Clear()
                    '        TempGrid2.Find(c4, ConditionOperator.Equal, a(2), 0, 1)
                    '        If Err.Number = 0 Then
                    '            TempGrid2.EnsureVisible()
                    '        End If
                    '    End If
                    'End If

                ElseIf Ar5(i).Trim.ToUpper = "CANCEL" Then
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper = "CLOSE" Then
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.Contains("COLORPICKER(") Then

                    'strIzabranaBoja = ""

                    If ColorDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then

                        strIzabranaBoja = ColorDialog1.Color.Name
                        strIzabranaBoja = System.Drawing.ColorTranslator.ToHtml(ColorDialog1.Color)



                        Ar5(i) = strOriginalSQL
                        param = Ar5(i).Trim
                        param = param.Replace("COLORPICKER(", "")
                        param = param.Replace(")", "")

                        If param.Trim <> "" Then
                            param = cls1.SrediSQLPosljeNextForme(param, ParametriSaGrida)
                            cls1.ExecSql(param)
                        End If


                    End If


                ElseIf Ar5(i).Trim.ToUpper = "CLOSEPARENT" Then
                    cls1.bCloseParent = True
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper.Contains("PREPAREMULTISELECTEDPRINT(") Then
                    'MultiSelectedPrintPriprema(Ar5(i))
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHSELECTED(") Then
                    'ForEachSelected(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHBLUEROW(") Then
                    'ForEachBlueRow(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHROW(") Then
                    'ForEachRow(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("CLICK(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CLICK(", "")
                    param = param.Replace(")", "")
                    'Glbl_Click(Controls(param), e)
                Else
                    'RefreshControls(Ar5(i))

                    Dim Trs2 As New DataTable

                    Trs2 = cls1.ExecSql(Ar5(i))

                    Dim ts5377 As String = ""
                    ts5377 = Trs2.Rows(0)("setcolorforcolorpicker")
                    If ts5377 <> "" Then
                        ColorDialog1.Color = System.Drawing.ColorTranslator.FromHtml(ts5377)
                        strIzabranaBoja = ts5377
                    End If


                    Dim ts53778 As String = ""
                    ts53778 = Trs2.Rows(1)("setcustomcolorsforcolorpicker")
                    If ts53778 <> "" Then

                        Dim r As DataRow
                        Dim boje() As Integer
                        Dim boje2() As String
                        Dim nizboja As String
                        Dim si(0) As Integer
                        Dim i99 As Integer
                        Array.Resize(si, Trs2.Rows.Count)
                        For Each r In Trs2.Rows
                            If r(0).ToString.Trim <> "" Then


                                Dim CurrentColor As Color = System.Drawing.ColorTranslator.FromHtml(r(0).ToString.Trim.ToLower)
                                Dim colourBlue As Integer = CurrentColor.B
                                Dim colourGreen As Integer = CurrentColor.G
                                Dim colourRed As Integer = CurrentColor.R
                                Dim newCustomColour As Integer = colourBlue << 16 Or colourGreen << 8 Or colourRed
                                'ColorDialog1.CustomColors = New Integer() {newCustomColour}
                                'nizboja = nizboja & " " & newCustomColour
                                si(i99) = newCustomColour
                                i99 = i99 + 1
                            End If
                        Next

                        'boje2 = Split(nizboja, " ")

                        ColorDialog1.CustomColors = si

                    End If


                    If GreskaKodIzvrsavanjaSQLa <> "" Then Exit For
                End If
            Next
        End If
    End Sub

    Private Sub GridEX1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX1.Click
        On Error Resume Next

        If sender.CurrentColumn.EditType = EditType.NoEdit Then
            GridEX1.SearchColumn = sender.CurrentColumn
        Else
            GridEX1.SearchColumn = Nothing
        End If

        Err.Clear()
        If sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.ToLower = "true" Or sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.ToLower = "false" Then
            If Err.Number = 0 Then
                'sender.MovePrevious()
                'sender.movenext()
                Windows.Forms.SendKeys.Send("+{TAB}")
                Windows.Forms.SendKeys.Send("{TAB}")
                Return
            End If
        End If

        'GridEX1.SearchColumn = sender.CurrentColumn
    End Sub

    Private Sub GridEX1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX1.DoubleClick
        NovaGenerickaForma(strDBlClickForm)
        'ColorDialog1.ShowDialog()
        'MsgBox(GridEX1.CurrentColumn.Caption)
    End Sub

    Private Sub GridEX1_GroupsChanged(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.GroupsChangedEventArgs) Handles GridEX1.GroupsChanged
        GridEX1.CollapseGroups()
        ' SaveGridLayout(cls1.SrediSQLPosljeNextForme(SrediSQL(strCUSTOMIMASTERGRIDLAYOUTFILE)))
    End Sub

    'Private Sub GridEX1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridEX1.MouseClick
    '    If GridEX1.RowCount > 0 Then
    '        PokaziUpitDetalj(strStartupStoraDetalj)
    '    End If
    'End Sub

    Private Sub ContextMenuStrip3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ContextMenuStrip3.Click
        'Dim i As Integer
        'Dim clickedItem As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)

        'For i = 1 To ContextMenuStrip3.Items.Count - 2
        '    Dim mnuItem As ToolStripMenuItem

        '    mnuItem = ContextMenuStrip3.Items(i) '= ContextMenuStrip3.Items.Add(GridEX1.CurrentRow.Cells(i).Column.Caption)

        '    'If mnuItem.Text = "VrstadokId" Then MsgBox(mnuItem.Checked)

        '    If mnuItem.Checked = True Then
        '        GridEX1.RootTable.Columns(mnuItem.Text).Visible = True
        '    Else
        '        GridEX1.RootTable.Columns(mnuItem.Text).Visible = False
        '    End If


        'Next i


        'GridEX1.RootTable.Columns("vrstadokid").Visible = False
        'GridEX1.Refresh()
    End Sub


    Private Sub GridEX1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridEX1.MouseUp

        On Error Resume Next

        Dim i As Integer
        ContextMenuStrip3.Items.Clear()
        If e.Button = Windows.Forms.MouseButtons.Right And ControlClicked = True Then
            For i = 1 To GridEX1.RootTable.Columns.Count - 1
                ' Dim mnuItem As ToolStripMenuItem = ContextMenuStrip3.Items.Add(GridEX1.CurrentRow.Cells(i).Column.Caption)
                Dim mnuItem As ToolStripMenuItem = ContextMenuStrip3.Items.Add(GridEX1.RootTable.Columns(i).Caption)


                'If GridEX1.CurrentRow.Cells(i).Column.Visible = True Then
                If GridEX1.RootTable.Columns(i).Visible = True Then
                    mnuItem.Checked = True
                Else
                    mnuItem.Checked = False
                End If

            Next i
            ControlClicked = False
            ContextMenuStrip3.Show(GridEX1, GridEX1.PointToClient(Windows.Forms.Cursor.Position))
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then

            'cls1.NapuniCBLog("Podizem masterpopup: " & Err.Description)




            ContextMenuStrip1.Show(GridEX1, GridEX1.PointToClient(Windows.Forms.Cursor.Position))
            'cls1.NapuniCBLog("Err description nakon dizanja masterpopupa : " & Err.Description)

        End If
    End Sub



    Private Sub GridEX1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GridEX1.KeyDown
        'On Error Resume Next

        ' Dim o As Object


        If e.KeyCode = Keys.Enter Then
            If strOnEnterKey.Trim <> "" Then
                NovaGenerickaForma(strOnEnterKey)
            End If
        End If



        'If e.KeyCode = Keys.F11 Then
        '    PokupiParametreSaGrida()
        '    cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
        '    'frm5.ShowDialog()
        'End If

        If e.KeyCode = 17 Then
            ControlClicked = True
        Else
            ControlClicked = False
        End If

        Dim iRow As Long = 0

        If e.Control Then
            If e.KeyCode = Keys.A Then
                'GridEX1.CheckAllRecords()

                UtjekuJeRucnoCheckiranjeRedova = True

                For iRow = 0 To GridEX1.RowCount - 1
                    GridEX1.SelectedItems.Add(iRow)
                Next

                UtjekuJeRucnoCheckiranjeRedova = False

            End If
            If e.KeyCode = Keys.C Then
                Clipboard.SetText(GridEX1.GetClipString)
            End If
        End If

    End Sub

    Private Sub GridEX2_CellUpdated(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.ColumnActionEventArgs) Handles GridEX2.CellUpdated
        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        PokupiParametreSaGrida()

        DetID = GetDetID()

        TrenutnaKolona = sender.CurrentColumn


        If sender.RowCount > 0 Then



            If sender.CurrentColumn.Caption.ToString <> "" Then
                If sender.CurrentRow.Cells(sender.CurrentColumn).Value Is Nothing Then

                Else
                    If Val(sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString) <> 0 Then
                        TrenutnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.Replace(",", ".")
                    Else
                        TrenutnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString
                    End If
                End If
            End If



            For i = 1 To sender.RootTable.Columns.Count - 1
                If sender.CurrentRow.Cells(i).Column.DataMember.ToString <> "" Then
                    If sender.CurrentRow.Cells(i).Value Is Nothing Then

                    Else

                        Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        End If
                    End If
                End If
            Next
        End If

        sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        Dim sql As String = ""

        'sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strONCellEditedstr), , , False)

        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strONCellEditedDETstr), ParametriSaGrida, , False)

        Rafalno(sql, sender, e)

        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
            sender.CurrentRow.Cells(sender.CurrentColumn).Value = PredhodnaVrijednost
        End If

    End Sub

    Private Sub GridEX2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX2.Click
        On Error Resume Next

        'If sender.CurrentColumn.EditType = EditType.NoEdit Then
        '    GridEX2.SearchColumn = sender.CurrentColumn
        'Else
        '    GridEX2.SearchColumn = Nothing
        'End If


        GridEX2.SearchColumn = sender.CurrentColumn


    End Sub

    Private Sub GridEX2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GridEX2.KeyDown
        On Error Resume Next

        If e.KeyCode = Keys.F11 Then
            PokupiParametreSaGrida()
            cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
            'frm5.ShowDialog()
        End If

        If e.KeyCode = 17 Then
            ControlClicked = True
        End If

        Dim iRow As Long = 0

        If e.Control Then
            If e.KeyCode = Keys.A Then
                'GridEX2.CheckAllRecords()

                For iRow = 0 To GridEX1.RowCount - 1
                    GridEX2.SelectedItems.Add(iRow)
                Next

            End If
            If e.KeyCode = Keys.C Then
                Clipboard.SetText(GridEX2.GetClipString)
            End If
        End If

    End Sub

    Private Sub GridEX1_RecordUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX1.RecordUpdated
        'MsgBox("ss")
        'GridEX1.CurrentRow.Cells("update").Value = 1
        If Not PrijevodjeutijekU Then GridEX1.CurrentRow.CheckState = RowCheckState.Checked
    End Sub

    Private Sub TreeView2_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView2.AfterSelect
        On Error Resume Next
        Node = ""
        Node = e.Node.Tag.ToString.Substring(0, e.Node.Tag.ToString.IndexOf(":"))
        Dim ImeForme As String = e.Node.Tag.ToString.Substring(e.Node.Tag.ToString.IndexOf(":") + 1, e.Node.Tag.ToString.Length - e.Node.Tag.ToString.IndexOf(":") - 1)
        NovaGenerickaForma(ImeForme)
        TreeView2.Focus()
    End Sub

    Private Sub TreeView2_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles TreeView2.NodeMouseClick
        'On Error Resume Next
        'Node = ""
        ''Node = e.Node.Tag.ToString
        'Node = e.Node.Tag.ToString.Substring(0, e.Node.Tag.ToString.IndexOf(":"))

        'NovaGenerickaForma(e.Node.Tag.ToString.Substring(e.Node.Tag.ToString.IndexOf(":") + 1, e.Node.Tag.ToString.Length - e.Node.Tag.ToString.IndexOf(":") - 1))
    End Sub


    Private Sub GridEX2_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX2.DoubleClick
        NovaGenerickaForma(strDBlClickDetForm)
    End Sub


    Private Sub GridEX2_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridEX2.MouseUp
        Dim i As Integer
        On Error Resume Next
        ContextMenuStrip4.Items.Clear()
        If e.Button = Windows.Forms.MouseButtons.Right And ControlClicked = True Then
            For i = 1 To GridEX2.RootTable.Columns.Count - 1
                'Dim mnuItem As ToolStripMenuItem = ContextMenuStrip4.Items.Add(GridEX2.CurrentRow.Cells(i).Column.Caption)
                Dim mnuItem As ToolStripMenuItem = ContextMenuStrip4.Items.Add(GridEX2.RootTable.Columns(i).Caption)
                'If GridEX2.CurrentRow.Cells(i).Column.Visible = True Then
                If GridEX2.RootTable.Columns(i).Visible = True Then
                    mnuItem.Checked = True
                Else
                    mnuItem.Checked = False
                End If
            Next i
            ControlClicked = False

            ContextMenuStrip4.Show(GridEX2, GridEX2.PointToClient(Windows.Forms.Cursor.Position))
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            ContextMenuStrip2.Show(GridEX2, GridEX2.PointToClient(Windows.Forms.Cursor.Position))
        End If
    End Sub

    Private Sub ContextMenuStrip3_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs) Handles ContextMenuStrip3.Closed

    End Sub

    Private Sub ContextMenuStrip3_Closing(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosingEventArgs) Handles ContextMenuStrip3.Closing

        If dontclose = True Then
            e.Cancel = True
            dontclose = False
        End If
    End Sub

    Private Sub ContextMenuStrip3_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ContextMenuStrip3.ItemClicked

        On Error Resume Next

        dontclose = True

        Dim ti As Integer
        Dim i As Integer

        For i = 0 To GridEX1.RootTable.Columns.Count - 1
            If GridEX1.RootTable.Columns(i).Caption.ToLower = e.ClickedItem.Text.ToLower Then
                ti = i
            End If
        Next

        Dim mytext As ToolStripMenuItem
        mytext = DirectCast(e.ClickedItem, ToolStripMenuItem)

        mytext.Checked = Not mytext.Checked

        If GridEX1.RootTable.Columns(ti).Visible Then
            GridEX1.RootTable.Columns(ti).Visible = False
            'sender.checked = False
        Else
            GridEX1.RootTable.Columns(ti).Visible = True
            'sender.checked = True
        End If

        'If GridEX1.RootTable.Columns(e.ClickedItem.Text).Visible Then
        '    GridEX1.RootTable.Columns(e.ClickedItem.Text).Visible = False
        'Else
        '    GridEX1.RootTable.Columns(e.ClickedItem.Text).Visible = True
        'End If

    End Sub

    Private Sub ContextMenuStrip4_Closing(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosingEventArgs) Handles ContextMenuStrip4.Closing
        If dontclose2 = True Then
            e.Cancel = True
            dontclose2 = False
        End If
    End Sub

    Private Sub ContextMenuStrip4_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ContextMenuStrip4.ItemClicked

        On Error Resume Next

        dontclose2 = True


        Dim ti As Integer
        Dim i As Integer

        For i = 0 To GridEX2.RootTable.Columns.Count - 1
            If GridEX2.RootTable.Columns(i).Caption.ToLower = e.ClickedItem.Text.ToLower Then
                ti = i
            End If
        Next

        If GridEX2.RootTable.Columns(ti).Visible Then
            GridEX2.RootTable.Columns(ti).Visible = False
        Else
            GridEX2.RootTable.Columns(ti).Visible = True

        End If

        Dim mytext As ToolStripMenuItem
        mytext = DirectCast(e.ClickedItem, ToolStripMenuItem)

        mytext.Checked = Not mytext.Checked


        'If GridEX2.RootTable.Columns(e.ClickedItem.Text).Visible Then
        '    GridEX2.RootTable.Columns(e.ClickedItem.Text).Visible = False
        'Else
        '    GridEX2.RootTable.Columns(e.ClickedItem.Text).Visible = True
        'End If

    End Sub

    Private Sub MenuStrip1_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked
        'NovaGenerickaForma(e.ClickedItem.Tag)

        If Not e.ClickedItem.Tag Is Nothing Then
            If e.ClickedItem.Tag.ToString.Trim <> "" Then
                NovaApplikacija(e.ClickedItem.Tag)
            End If
        End If

    End Sub

    Private Sub Timer1_Disposed(sender As Object, e As EventArgs) Handles Timer1.Disposed

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)
    End Sub

    Private Sub ToolStripStatusLabel3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripStatusLabel3.Click
        If Timer1.Enabled Then
            TimerOff()
        Else
            TimerOn()
        End If
    End Sub

    Sub TimerOff()
        'cls1.NapuniCBLog("Timer OFF")
        Timer1.Enabled = False
        ToolStripStatusLabel3.Text = "Timer OFF"
        ToolStripStatusLabel3.BackColor = Color.Silver
    End Sub

    Sub TimerOn()
        If strTimer <> "" Then
            'cls1.NapuniCBLog("Timer ON")
            Timer1.Interval = CInt(strTimer)
            Timer1.Enabled = True
            ToolStripStatusLabel3.Text = "Timer ON " & Timer1.Interval / 1000
            ToolStripStatusLabel3.BackColor = Color.LightGreen
        End If
    End Sub

    'Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
    '    Dim connectionstring As String
    '    Dim asembly As Reflection.Assembly = Reflection.Assembly.Load("RSGen")
    '    'Dim o As Object = asembly.CreateInstance("RSGen.clsGen")
    '    Dim o2 As Object = CreateObject("RSGen.clsGen")

    '    connectionstring = "Provider=MSDASQL.1;Extended Properties=" + Chr(34) + "DRIVER=SQL Server;SERVER=jupiter\sql2005;UID=GGudlin;APP=SpinClient;WSID=GOGO;DATABASE=Spin;Trusted_Connection=Yes" + Chr(34)
    '    o2.dofaktura3("rptFakturaKNGen", connectionstring, 34742, 0, "HP LaserJet IIIP")
    '    o2 = Nothing

    'End Sub



    'Private Sub LoadAndInvoke()
    '    'Call to show OpenFileDialog to select file 
    '    OpenFileDialog1.ShowDialog()
    '    'Loading assembly data from the file 
    '    Dim myassembly As [Assembly] = [Assembly].LoadFrom(OpenFileDialog1.FileName)
    '    'Used to retrieves types from myassembly 
    '    Dim Atype As Type = myassembly.GetTypes()(0)
    '    'Creating an instance of the assembly value 
    '    Dim o As Object = myassembly.CreateInstance(Atype.FullName)
    '    'Calling InvokeMember from Atype object to execute the method 
    '    'in assembly on runtime. Last member of the call is 
    '    'important because its passes parameters to the call in the form of array. 
    '    Atype.InvokeMember("Test", BindingFlags.Default Or BindingFlags.InvokeMethod Or BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public, Nothing, o, Nothing)
    '    Atype.InvokeMember("Test2", BindingFlags.Default Or BindingFlags.InvokeMethod Or BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public, Nothing, o, Nothing)
    'End Sub


    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        DeleteGridLayout()
    End Sub


    Private Sub TreeView2_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles TreeView2.BeforeSelect
        On Error Resume Next
        TreeView2.SelectedNode.BackColor = Color.White
    End Sub


    Private Sub TreeView2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TreeView2.LostFocus
        On Error Resume Next
        TreeView2.SelectedNode.BackColor = Color.LightGreen
    End Sub


    Private Sub GridEX1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridEX1.SelectionChanged
        If GridEX1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)
        End If
    End Sub


    Private Sub GridEXPrintDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles GridEXPrintDocument1.PrintPage
        If strPageFooterCenter.Trim = "" Then GridEXPrintDocument1.PageFooterCenter = "Stranica " & GridEXPrintDocument1.Page & " / " & GridEXPrintDocument1.TotalPages
    End Sub



    Private Sub ButtonZaDrvo_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonZaDrvo.Click
        Static SD As Integer
        Static VidiSe As Boolean = True
        If VidiSe Then
            SD = SplitContainer1.SplitterDistance
            SplitContainer1.SplitterDistance = 1
            VidiSe = False
        Else
            SplitContainer1.SplitterDistance = SD
            VidiSe = True
        End If
    End Sub


    Private Sub TreeView2_KeyDown1(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TreeView2.KeyDown
        If e.Control Then
            If e.KeyCode = Keys.F11 Then
                PokupiParametreSaGrida()
                cls1.PokaziCBLog("PARAMATERISAMASTERIDETAILGRIDA=" & ParametriSaGrida)
                'frm5.ShowDialog()
            End If
        End If
    End Sub


    Sub CustomFormatKolona(ByVal Glbl As GridEX, ByVal m_nodeList As Xml.XmlNodeList, ByVal strCLEARCOLUMS As String, ByVal strCENTERCOLUMS As String)
        ' custom foramat kolona 
        On Error Resume Next
        If m_nodeList Is Nothing Then Exit Sub
        If m_nodeList.Count = 0 Then Exit Sub

        Dim m_node2 As Xml.XmlNode

        If strCLEARCOLUMS.ToLower = "true" Then
            For Each Column In Glbl.RootTable.Columns
                Column.visible = False
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
                If Not Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)) Is Nothing Then

                    If tAggregateFunction <> "" Then
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AggregateFunction = tAggregateFunction
                    End If

                    If tmaxLines > 0 Then
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).MaxLines = tmaxLines
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).wordwrap = True
                    End If


                    If strCLEARCOLUMS.ToLower = "true" Then
                        iPosition = iPosition + 1
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = iPosition
                    End If


                    If tWidth > 0 Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).width = tWidth / FakPretvorbe
                    If tCaption <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).caption = tCaption
                    If tFormat <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).FormatString = tFormat
                    If tVisible <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).visible = tVisible
                    If tPosition <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = tPosition



                    If strHeaderAlignment.Trim <> "" Then
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).HeaderAlignment = strHeaderAlignment
                    End If

                    'Glbl.RootTable.Columns(1).he
                    Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).caption = Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).caption.replace(";", vbCrLf)

                    If tAutoSize.ToLower = "true" Then
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSizeMode = ColumnAutoSizeMode.AllCellsAndHeader
                        Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSize()
                    End If


                    If tNegativeRed.ToLower.Trim = "true" Then
                        Dim fc As Janus.Windows.GridEX.GridEXFormatCondition
                        fc = New Janus.Windows.GridEX.GridEXFormatCondition(Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)), Janus.Windows.GridEX.ConditionOperator.LessThan, 0)
                        fc.TargetColumn = Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex))
                        fc.FormatStyle.ForeColor = System.Drawing.Color.Red
                        'fc.FormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.Diagonal
                        Glbl.RootTable.FormatConditions.Add(fc)
                    End If



                    If tNotZeroRed.ToLower.Trim = "true" Then
                        Dim fc As Janus.Windows.GridEX.GridEXFormatCondition
                        fc = New Janus.Windows.GridEX.GridEXFormatCondition(Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)), Janus.Windows.GridEX.ConditionOperator.NotEqual, 0)
                        fc.TargetColumn = Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex))
                        fc.FormatStyle.ForeColor = System.Drawing.Color.Red
                        'fc.FormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.Diagonal
                        Glbl.RootTable.FormatConditions.Add(fc)
                    End If




                    ' Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = tColor
                    If tForeColor.Trim <> "" Then
                        Select Case tForeColor.ToLower
                            Case "green"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Green
                            Case "white"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.White
                            Case "black"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Black
                            Case "darksalmon"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.DarkSalmon
                            Case "darkseagreen"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.DarkSeaGreen
                            Case "limegreen"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.LimeGreen
                            Case ("greenyellow")
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.GreenYellow
                            Case "yellow"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Yellow
                            Case "lightyellow"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.LightYellow
                            Case "red"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Red
                            Case "brown"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Brown
                            Case "blue"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Blue
                            Case "ghostwhite"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.GhostWhite
                            Case "gray"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Gray
                            Case "orange"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Orange
                            Case "orangered"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.OrangeRed
                        End Select
                    End If

                    If tBackColor.Trim <> "" Then
                        Select Case tBackColor.ToLower
                            Case "green"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Green
                            Case "white"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.White
                            Case "black"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Black
                            Case "darksalmon"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.DarkSalmon
                            Case "darkseagreen"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.DarkSeaGreen
                            Case "limegreen"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.LimeGreen
                            Case ("greenyellow")
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.GreenYellow
                            Case "yellow"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Yellow
                            Case "lightyellow"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.LightYellow
                            Case "red"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Red
                            Case "brown"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Brown
                            Case "blue"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Blue
                            Case "ghostwhite"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.GhostWhite
                            Case "gray"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Gray
                            Case "orange"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Orange
                            Case "orangered"
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.OrangeRed
                        End Select
                    End If

                End If
            End If

        Next
        '--------------------------------- end custom foramat
    End Sub



    Private Sub Button7_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button8_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button9_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub Button10_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Module1.Sender7 = sender
        NovaGenerickaForma(sender.tag)
    End Sub

    Private Sub butonzarazgrupiranje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butonzarazgrupiranje.Click
        On Error Resume Next
        Static c As Integer = 0
        If c = 0 Then
            GridEX1.ExpandGroups()
            c = 1
        Else
            GridEX1.CollapseGroups()
            c = 0
        End If


    End Sub

    Private Sub Form1_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LocationChanged

    End Sub

    Private Sub Form1_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        TimerOff()
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        On Error Resume Next
        TreeView2.Width = TabPage2.Width - 6
        TreeView2.Height = TabPage2.Height - 30
        If Me.WindowState = FormWindowState.Maximized Then
            ' TimerOn()
            Timer2.Enabled = True
        End If
        ' Me.TabControl1.Region = New Region(New RectangleF(Me.TabPage1.Left, Me.TabPage1.Top, Me.TabPage1.Width, Me.TabPage1.Height))
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        'TimerOn() 
    End Sub



    Private Sub Form1_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
        On Error Resume Next
        'Form1_Resize(sender, e)
    End Sub

    Private Sub ComboBox1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub TabPage2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabPage2.Click

    End Sub

    Private Sub TabPage2_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabPage2.Resize
        Form1_Resize(sender, e)
    End Sub

    Private Sub ComboBox1_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedValueChanged
        On Error Resume Next
        strTreeNodes = ComboBox1.SelectedValue
        NapuniDrvoVrijeme()
    End Sub


    Function Rafalna_Priprema(ByVal stR As String) As String
        On Error Resume Next
        Dim param As String = ""
        Dim Ar5() As String
        Dim i As Long
        Dim tS As String = stR

        If stR.ToUpper.Contains("ISEMPTY(") Then
            tS = ""
        Else
            tS = stR
        End If

        If stR.ToString.Trim <> "" Then
            Ar5 = Split(stR.ToString, "#")
            For i = 0 To UBound(Ar5)
                '
                '
                If Ar5(i).ToUpper.Contains("ISEMPTY(") Then
                    param = Ar5(i).ToUpper
                    param = param.Replace("ISEMPTY(", "")
                    param = param.Replace(")", "")
                    If param.ToLower = "null" Or param.Trim = "" Then
                        i = i + 1
                        tS = tS & Trim(Ar5(i))
                        i = i + 1
                    Else
                        i = i + 2
                        tS = tS & Trim(Ar5(i)).Replace("**", " ")
                        'tS = tS & Ar5(i)
                    End If
                End If
                '
                'i = i + 1
            Next
        End If
        Rafalna_Priprema = SrediSQL(tS)
    End Function


    Private Sub ButtonHELP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonHELP.Click
        DajPokaziHelp()
    End Sub

    Public Sub DajPokaziHelp()
        On Error Resume Next

        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))


        'Dim hf1 As New Help_Client_V8.Class1
        'hf1.Run(strHelpKod, cls1.strBaza, cls1.strServer, imeXMLa)

        Dim p As New Process()
        p.StartInfo.FileName = "JupiterHelp.exe"
        p.StartInfo.Arguments = "-s " + cls1.strServer + " -d " + cls1.strBaza + " -h " + strHelpKod + " -o " + Str(cls1.OperateriID) + " -p " + imeXMLa
        p.StartInfo.CreateNoWindow = True
        p.EnableRaisingEvents = True
        ''p.Exited += New EventHandler(p_Exited)
        ''p.StartInfo.Verb = "runas"
        p.StartInfo.WorkingDirectory = Environment.CurrentDirectory
        p.Start()
    End Sub


    Private Sub GridEX1_EditingCell(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.EditingCellEventArgs) Handles GridEX1.EditingCell

        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        'PokupiParametreSaGrida()
        'TrenutnaKolona = sender.CurrentColumn
        PredhodnaVrijednost = ""
        ' TrenutnaVrijednost = ""

        If sender.RowCount > 0 Then


            If sender.CurrentColumn.Caption.ToString <> "" Then
                If sender.CurrentRow.Cells(sender.CurrentColumn).Value Is Nothing Then

                Else
                    If Val(sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString) <> 0 Then
                        PredhodnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.Replace(",", ".")
                    Else
                        PredhodnaVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString
                    End If
                End If
            End If



            For i = 1 To sender.RootTable.Columns.Count - 1
                If sender.CurrentRow.Cells(i).Column.DataMember.ToString <> "" Then
                    If sender.CurrentRow.Cells(i).Value Is Nothing Then

                    Else

                        Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString & "' , "
                        End If
                    End If
                End If
            Next
        End If




        If strBeforeCellEditedstr.Trim = "" Then
            Exit Sub
        End If

        sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        Dim sql As String = ""
        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strBeforeCellEditedstr))
        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strBeforeCellEditedstr), ParametriSaGrida)

        Rafalno(sql, sender, e)

        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
            e.Cancel = True
        End If

        GridEX1.SearchColumn = Nothing

    End Sub

    Private Sub GridEX1_FormattingRow(ByVal sender As System.Object, ByVal e As Janus.Windows.GridEX.RowLoadEventArgs) Handles GridEX1.FormattingRow
        On Error Resume Next
        'Dim tstr As String = ""
        'On Error Resume Next
        'If e.Row.RowType = RowType.GroupHeader Then
        '    tstr = ""
        '    tstr = e.Row.GroupCaption
        '    e.Row.GroupCaption = tstr + "  ( " + e.Row.Children + ")"
        'End If



        'For group rows, you add a GroupHeaderTotal to the table showing the count. This can be done at design time, but here is a sample of how could you do it in code:

        '    Dim groupTotal As New GridEXGroupHeaderTotal()

        '    groupTotal.AggregateFunction = AggregateFunction.Count

        '    groupTotal.TotalPrefix = "("

        '    groupTotal.TotalSuffix = " items)"

        '    GridEX1.RootTable.GroupHeaderTotals.Add(groupTotal)

        If e.Row.RowType = RowType.Record Then
            Dim strIkona As String = ""
            Err.Clear()
            strIkona = e.Row.Cells("ikonaid").Value.ToString
            If strIkona.Trim <> "" Then
                If Err.Number = 0 Then
                    e.Row.Cells("ikona").Image = ImageList3.Images(strIkona)
                End If
            End If
            Err.Clear()
            Dim strBojaFonta As String = ""
            Dim strBojaPozadine As String = ""
            strBojaFonta = e.Row.Cells("RowForeColorID").Value.ToString
            strBojaPozadine = e.Row.Cells("RowColorID").Value.ToString
            If Err.Number <> 0 Then
                'Exit Sub
            Else
                Dim style1 As New GridEXFormatStyle

                Dim c1 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta)
                Dim c2 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaPozadine)

                style1.ForeColor = c1
                style1.BackColor = c2

                e.Row.RowStyle = style1
            End If


            ' pokušaj bojanja celija ...
            Dim ii As Integer
            Dim strBojaFonta5 As String = ""
            Dim strBojaFonta6 As String = ""

            Dim style6 As New GridEXFormatStyle
            Dim imekOlone As String = ""
            For ii = 0 To e.Row.Cells.Count - 1
                Dim style5 As New GridEXFormatStyle
                imekOlone = ""
                strBojaFonta5 = ""
                strBojaFonta6 = ""

                If e.Row.Cells.Item(ii).Column.DataMember.Contains("MaxLinesID") Then
                    imekOlone = e.Row.Cells.Item(ii).Column.DataMember.Replace("MaxLinesID", "")
                    e.Row.Cells(imekOlone).Column.WordWrap = True
                    e.Row.Cells(imekOlone).Column.MaxLines = e.Row.Cells(ii).Value.ToString
                End If

                If e.Row.Cells.Item(ii).Column.DataMember.Contains("CellForeColorID") Then
                    strBojaFonta5 = e.Row.Cells(ii).Value.ToString
                    Dim c5 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta5)
                    imekOlone = e.Row.Cells.Item(ii).Column.DataMember.Replace("CellForeColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.ForeColor = c5
                End If

                If e.Row.Cells.Item(ii).Column.DataMember.Contains("CellBackColorID") Then
                    strBojaFonta6 = e.Row.Cells(ii).Value.ToString
                    Dim c6 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta6)

                    imekOlone = e.Row.Cells.Item(ii).Column.DataMember.Replace("CellBackColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.BackColor = c6
                    'e.Row.Cells(e.Row.Cells.Item(ii).Column.Caption.Replace("CellBackColorID", "")).FormatStyle = style6
                End If

                If imekOlone <> "" Then e.Row.Cells(imekOlone).FormatStyle = style5


            Next


        End If




        'e.Row.Cells("RowColorID").FormatStyle.ForeColor = 


    End Sub




    'Here is my code:

    '    Private Sub AddGridexNegativeFormat()

    '        Dim fc As Janus.Windows.GridEX.GridEXFormatCondition

    '        'SIZE 1 - NEGATIVES ARE RED

    '        fc = New Janus.Windows.GridEX.GridEXFormatCondition(GridEXOrders.RootTable.ChildTables(0).Columns("S1"), Janus.Windows.GridEX.ConditionOperator.LessThan, 0)

    '        fc.TargetColumn = GridEXOrders.RootTable.ChildTables(0).Columns("S1")

    '        fc.FormatStyle.ForeColor = System.Drawing.Color.Red

    '        'fc.FormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.Diagonal

    '        GridEXOrders.RootTable.FormatConditions.Add(fc)

    '    End Sub



    Private Sub OriginalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OriginalToolStripMenuItem.Click
        Language = ""
        NapuniRSLang()
        strMultilingual = "0"
    End Sub

    Private Sub MKToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MKToolStripMenuItem.Click
        Language = "MK"
        NapuniRSLang()
        strMultilingual = "2"
        TranslateForm(Me)
    End Sub

    Private Sub ENToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ENToolStripMenuItem.Click
        Language = "EN"
        NapuniRSLang()
        strMultilingual = "2"
        TranslateForm(Me)
    End Sub

    Private Sub GridEX2_FormattingRow(ByVal sender As System.Object, ByVal e As Janus.Windows.GridEX.RowLoadEventArgs) Handles GridEX2.FormattingRow
        On Error Resume Next
        If e.Row.RowType = RowType.Record Then
            Dim strIkona As String = ""
            Err.Clear()
            strIkona = e.Row.Cells("ikonaid").Value.ToString
            If strIkona.Trim <> "" Then
                If Err.Number = 0 Then
                    e.Row.Cells("ikona").Image = ImageList3.Images(strIkona)
                End If
            End If
            Err.Clear()
            Dim strBojaFonta As String = ""
            Dim strBojaPozadine As String = ""
            strBojaFonta = e.Row.Cells("RowForeColorID").Value.ToString
            strBojaPozadine = e.Row.Cells("RowColorID").Value.ToString
            If Err.Number <> 0 Then
                ' Exit Sub
            Else
                Dim style1 As New GridEXFormatStyle

                Dim c1 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta)
                Dim c2 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaPozadine)

                style1.ForeColor = c1
                style1.BackColor = c2

                e.Row.RowStyle = style1
            End If



            ' pokušaj bojanja celija ...
            Dim ii As Integer
            Dim strBojaFonta5 As String = ""
            Dim strBojaFonta6 As String = ""

            Dim style6 As New GridEXFormatStyle
            Dim imekOlone As String = ""
            For ii = 0 To e.Row.Cells.Count - 1
                Dim style5 As New GridEXFormatStyle
                imekOlone = ""
                strBojaFonta5 = ""
                strBojaFonta6 = ""

                If e.Row.Cells.Item(ii).Column.DataMember.Contains("CellForeColorID") Then
                    strBojaFonta5 = e.Row.Cells(ii).Value.ToString
                    Dim c5 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta5)
                    imekOlone = e.Row.Cells.Item(ii).Column.DataMember.Replace("CellForeColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.ForeColor = c5
                End If

                If e.Row.Cells.Item(ii).Column.DataMember.Contains("CellBackColorID") Then
                    strBojaFonta6 = e.Row.Cells(ii).Value.ToString
                    Dim c6 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta6)

                    imekOlone = e.Row.Cells.Item(ii).Column.DataMember.Replace("CellBackColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.BackColor = c6
                    'e.Row.Cells(e.Row.Cells.Item(ii).Column.Caption.Replace("CellBackColorID", "")).FormatStyle = style6
                End If

                If imekOlone <> "" Then e.Row.Cells(imekOlone).FormatStyle = style5


            Next


        End If
    End Sub

    Private Sub ContextMenuStrip2_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip2.Opening

    End Sub

    Private Sub ContextMenuStrip3_LocationChanged(sender As Object, e As EventArgs) Handles ContextMenuStrip3.LocationChanged

    End Sub

    Private Sub ContextMenuStrip3_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip3.Opening

    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        On Error Resume Next
        DeleteGridLayout()
        DeleteGridLayoutDet()
    End Sub

    Private Sub GridEX1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles GridEX1.KeyPress
        ' MsgBox("a")
    End Sub



    Private Sub TreeView2_DragDrop22(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TreeView2.DragDrop
        MsgBox(e.Data.GetData(DataFormats.Text).ToString)
    End Sub


    Private Sub GridEX1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GridEX1.MouseDown
        'GridEX1.DoDragDrop(GridEX1, DragDropEffects.Move)
    End Sub

    Private Sub TreeView2_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles TreeView2.ItemDrag
        'MsgBox(1)
    End Sub


    Public Sub TreeView2_DragDrop(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.DragEventArgs) _
        Handles TreeView2.DragDrop

        'MsgBox(e.Data.GetData(DataFormats.Text).ToString)

        'Check that there is a TreeNode being dragged
        'If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", True) = False Then Exit Sub

        'Get the TreeView raising the event (incase multiple on form)
        Dim selectedTreeview As TreeView = CType(sender, TreeView)

        'Get the TreeNode being dragged
        'Dim dropNode As TreeNode = CType(e.Data.GetData("System.Windows.Forms.TreeNode"), TreeNode)

        'The target node should be selected from the DragOver event
        Dim targetNode As TreeNode = selectedTreeview.SelectedNode

        'Remove the drop node from its current location
        'dropNode.Remove()

        'If there is no targetNode add dropNode to the bottom of
        'the TreeView root nodes, otherwise add it to the end of
        'the dropNode child nodes
        ' If targetNode Is Nothing Then
        'selectedTreeview.Nodes.Add(dropNode)
        'Else
        'targetNode.Nodes.Add(dropNode)
        'End If

        'Ensure the newley created node is visible to
        'the user and select it
        'dropNode.EnsureVisible()
        'selectedTreeview.SelectedNode = dropNode

        cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(strTREEDragDrop)))


    End Sub

    Public Sub TreeView2_DragOver(ByVal sender As System.Object, ByVal e As DragEventArgs) Handles TreeView2.DragOver

        'Check that there is a TreeNode being dragged 
        'If e.Data.GetDataPresent("System.Windows.Forms.TreeNode", True) = False Then Exit Sub

        'Get the TreeView raising the event (incase multiple on form)
        Dim selectedTreeview As TreeView = CType(sender, TreeView)

        'As the mouse moves over nodes, provide feedback to 
        'the user by highlighting the node that is the 
        'current drop target
        Dim pt As Point = _
            CType(sender, TreeView).PointToClient(New Point(e.X, e.Y))
        Dim targetNode As TreeNode = selectedTreeview.GetNodeAt(pt)

        'See if the targetNode is currently selected, 
        'if so no need to validate again
        If Not (selectedTreeview.SelectedNode Is targetNode) Then
            'Select the    node currently under the cursor
            selectedTreeview.SelectedNode = targetNode

            'Check that the selected node is not the dropNode and
            'also that it is not a child of the dropNode and 
            'therefore an invalid target
            Dim dropNode As TreeNode = _
                CType(e.Data.GetData("System.Windows.Forms.TreeNode"),  _
                TreeNode)

            Do Until targetNode Is Nothing
                If targetNode Is dropNode Then
                    e.Effect = DragDropEffects.None
                    Exit Sub
                End If
                targetNode = targetNode.Parent
            Loop
        End If

        'Currently selected node is a suitable target
        e.Effect = DragDropEffects.Move


    End Sub

    Private Sub GridView1_CellValueChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs) Handles GridView1.CellValueChanged

        If e.Column.FieldName = "rcb555" Then
            Exit Sub
        End If

        Dim v As String = ""
        v = GridView1.GetDataRow(e.RowHandle)(e.Column.FieldName)

        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        ID = GetID()
        cls1.MasterID = ID
        PokupiParametreSaGrida()

        TrenutnaKolonaDX = ""
        TrenutnaKolonaDX = e.Column.FieldName
        TrenutnaVrijednost = ""


        If TrenutnaKolonaDX <> "" Then
            If v Is Nothing Then

            Else
                If Val(v) <> 0 Then
                    TrenutnaVrijednost = v.Replace(",", ".")
                Else
                    TrenutnaVrijednost = v
                End If
            End If
        End If


        For Each c As DevExpress.XtraGrid.Columns.GridColumn In GridView1.Columns
            Dim valcol = GridView1.GetDataRow(e.RowHandle)(c.FieldName)

            If valcol Is Nothing Then

            Else
                If Val(valcol) <> 0 Then
                    ParametriSaGrida = ParametriSaGrida & "@" & c.FieldName.ToString.Replace(" ", "_") & "='" & valcol.ToString.Replace(",", ".") & "' , "
                Else
                    ParametriSaGrida = ParametriSaGrida & "@" & c.FieldName.ToString.Replace(" ", "_") & "='" & valcol.ToString.Replace(",", ".") & "' , "
                End If
            End If
        Next


        'For i = 1 To sender.RootTable.Columns.Count - 1
        '    If sender.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
        '        If sender.CurrentRow.Cells(i).Value Is Nothing Then

        '        Else
        '            If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
        '                ParametriSaGrida = ParametriSaGrida & "@" & sender.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
        '            Else
        '                ParametriSaGrida = ParametriSaGrida & "@" & sender.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
        '            End If
        '        End If
        '    End If
        'Next

        ' to sutra
        ' sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        GridView1.SetFocusedRowCellValue(GridView1.Columns("rcb555"), True)

        Dim sql As String = ""

        sql = cls1.SrediSQLPosljeNextForme(SrediSQL(strONCellEditedstr), ParametriSaGrida, , False)

        Rafalno(sql, sender, e)

        'If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
        '    sender.CurrentRow.Cells(sender.CurrentColumn).Value = PredhodnaVrijednost
        'End If



    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        ID = GetID()
        cls1.MasterID = ID

        If GridView1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)


        End If
    End Sub


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        On Error Resume Next

        GridView1.OptionsLayout.StoreAllOptions = True
        GridView2.OptionsLayout.StoreAllOptions = True
        ' Add any initialization after the InitializeComponent() call.

        GridEX2.Parent = TabPage3
        GridEX2.Dock = 5
        GridEX3.Dock = 5
        GridEX1.RowHeaders = InheritableBoolean.False
        GridEX2.RowHeaders = InheritableBoolean.False
        'GridEX3.RootTable.Columns(1).Visible = False


    End Sub

    Private Sub GridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles GridView1.KeyDown

        If e.KeyCode = Keys.Return Then
            Dim ai As ArrayList = Nothing
            Dim Fc As Integer = GridView1.FocusedColumn.VisibleIndex

            'Dim cc As Integer = GridView1.Columns.Count
            'For i = 0 To cc - 1
            '    If IsNumeric(GridView1.Columns(i).Tag) AndAlso GridView1.Columns(i).Tag = 1 Then
            '        ai.Add(GridView1.Columns(i).ColumnHandle)
            '    End If
            'Next

            Dim c As Integer = -1

            For i = Fc + 1 To GridView1.VisibleColumns.Count - 1

                If IsNumeric(GridView1.VisibleColumns(i).Tag) AndAlso GridView1.VisibleColumns(i).Tag = 1 Then
                    c = i
                    'GridView1.FocusedColumn = GridView1.VisibleColumns(i)
                    'GridView1.ShowEditor()
                    Exit For
                End If

            Next

            If c = -1 Then
                For i = 0 To Fc
                    If IsNumeric(GridView1.VisibleColumns(i).Tag) AndAlso GridView1.VisibleColumns(i).Tag = 1 Then
                        c = i
                        GridView1.FocusedRowHandle = GridView1.FocusedRowHandle + 1
                        Exit For
                    End If
                Next
            End If

            If c >= 0 Then
                GridView1.FocusedColumn = GridView1.VisibleColumns(c)
                GridView1.ShowEditor()
            End If
            'GridView1.FocusedRowHandle = 6
            'gvMain.FocusedColumn = gvMain.VisibleColumns(4)
            'gvMain.ShowEditor()
        End If

    End Sub

    Private Sub GridView1_MouseUp(sender As Object, e As MouseEventArgs) Handles GridView1.MouseUp
        On Error Resume Next
        'ContextMenuStrip1.Show(GridEX1, GridEX1.PointToClient(Windows.Forms.Cursor.Position))
        If e.Button = MouseButtons.Right Then
            ContextMenuStrip1.Show(GridControl1, GridControl1.PointToClient(Cursor.Position))
        End If

    End Sub

    'Private Sub GridControl1_MouseUp(sender As Object, e As MouseEventArgs) Handles GridControl1.MouseUp
    '    If e.Button = MouseButtons.Right Then
    '        DoShowMenu(GridView1.CalcHitInfo(New Point(e.X, e.Y)))
    '    End If
    'End Sub

    'Sub DoShowMenu(ByVal hi As DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo)
    '    ' Create the menu. 
    '    Dim menu As DevExpress.XtraGrid.Menu.GridViewMenu = Nothing
    '    ' Check whether the header panel button has been clicked. 
    '    If hi.HitTest = DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.ColumnButton Then
    '        menu = New DevExpress.XtraGrid.Menu.GridViewColumnMenu(GridView1)
    '        menu.Init(hi)
    '        ' Display the menu. 
    '        menu.Show(hi.HitPoint)
    '    End If
    'End Sub

    Private Sub GridView1_ShownEditor(sender As Object, e As EventArgs) Handles GridView1.ShownEditor

        BeginInvoke(New Action(Sub()
                                   If GridView1.ActiveEditor IsNot Nothing Then
                                       GridView1.ActiveEditor.SelectAll()
                                   End If
                               End Sub))

    End Sub

     

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        TimerOn()
        Timer2.Enabled = False
    End Sub

    Private Sub GridEX3_MouseUp(sender As Object, e As MouseEventArgs) Handles GridEX3.MouseUp
        On Error Resume Next
        If e.Button = Windows.Forms.MouseButtons.Right Then
            ContextMenuStrip4.Items.Clear()
            If ContextMenuStrip4.Items.Count = 0 Then
                ContextMenuStrip4.Items.Add(Translate("Pogledaj"), Nothing, AddressOf DMS_Pogledaj)
                ContextMenuStrip4.Items.Add(Translate("Spremi na disk"), Nothing, AddressOf DMS_SpremiNaDisk)
                ContextMenuStrip4.Items.Add(Translate("Obriši"), Nothing, AddressOf DMS_Obrisi)
                ContextMenuStrip4.Items.Add("-", Nothing, Nothing)
                ContextMenuStrip4.Items.Add(Translate("Preuzmi za potpis"), Nothing, AddressOf DMS_preuzmizapotpis)
                ContextMenuStrip4.Items.Add(Translate("Vrati potpisano"), Nothing, AddressOf DMS_vratipotpisano)
                ContextMenuStrip4.Items.Add("-", Nothing, Nothing)
                ContextMenuStrip4.Items.Add(Translate("Odustani od potpisivanja"), Nothing, AddressOf DMS_odustani)

                'ContextMenuStrip4.Items.Add(Translate("Predlošci"), Nothing, AddressOf DMS_Pogledaj)

            End If
            If GridEX3.CurrentRow.Cells("CheckOutOperater").Value Is DBNull.Value Then
                'ContextMenuStrip4.Items(3).Enabled = False;
                ContextMenuStrip4.Items(4).Enabled = True
                ContextMenuStrip4.Items(5).Enabled = False
                ' ContextMenuStrip4.Items(6).Enabled = False;
                ContextMenuStrip4.Items(7).Enabled = False
            Else
                'ContextMenuStrip4.Items(3).Enabled = False;
                ContextMenuStrip4.Items(4).Enabled = False
                ContextMenuStrip4.Items(5).Enabled = True
                ' ContextMenuStrip4.Items(6).Enabled = False;
                ContextMenuStrip4.Items(7).Enabled = True
            End If
            ContextMenuStrip4.Show(GridEX3, GridEX3.PointToClient(Windows.Forms.Cursor.Position))
        End If

    End Sub

    Sub DMS_odustani()
        On Error Resume Next
        ContextMenuStrip4.Hide()
        GreskaKodIzvrsavanjaSQLa = ""
        cls1.ExecSql("exec dms.spSetDocumentUndoCheckOutOperater @dokId='" + GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim + "'")
        If Err.Number <> 0 Then
            ' 20.10.2026. KK - SQL grešku ExecSql već sam prikaže, ovdje samo ono što do SQL-a nije ni došlo.
            ' MsgBox mora ići prije NapuniCBLog jer on (On Error) pri izlasku obriše Err.
            If GreskaKodIzvrsavanjaSQLa = "" Then MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("exec dms.spSetDocumentUndoCheckOutOperater @dokId='" & GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim & "' :::" & Err.Description)
            Exit Sub
        End If

        If GridEX1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)
        End If

        DMS_pOkazi_Upit()
    End Sub

    Sub DMS_preuzmizapotpis() ' CHECKOUT
        On Error Resume Next
        ContextMenuStrip4.Hide()
        ' Dim a2 As Assembly = Assembly.Load("DMSScan")
        Dim a2 As Assembly = Assembly.Load("DmsManagerV1")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška kod Assembly.Load DMSScan : " & Err.Description)
            Exit Sub
        End If

        ' Dim o2 As Object = a2.CreateInstance("clsDMSScan.clsDMS")
        Dim o2 As Object = a2.CreateInstance("DmsManager.Db.Documents")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  CreateInstance DmsManager.Db.Documents : " & Err.Description)
            Exit Sub
        End If

        ' Dim o3 As New clsDMSScan.clsDMS
        ' o3.frmVi.ShowDialog()
        cls1.NapuniCBLog("Zovem DocumentCheckOut u DmsManager.Db.Documents dokid=" & GridEX3.CurrentRow.Cells("dokid").Value.ToString)
        cls1.NapuniCBLog("Ime dokumenta: dokument=" & GridEX3.CurrentRow.Cells("dokument").Value.ToString)

        o2.DocumentCheckOut(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim, GridEX3.CurrentRow.Cells("dokument").Value.ToString.Trim)


        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  DocumentCheckOut u DmsManager.Db.Documents : " & Err.Description)
            Exit Sub
        End If

        ' refresh detaljaa
        If GridEX1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)
        End If

        DMS_pOkazi_Upit()

    End Sub

    Sub DMS_vratipotpisano() ' checkin
        On Error Resume Next
        ContextMenuStrip4.Hide()
        ' Dim a2 As Assembly = Assembly.Load("DMSScan")
        Dim a2 As Assembly = Assembly.Load("DmsManagerV1")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška kod Assembly.Load DMSScan : " & Err.Description)
            Exit Sub
        End If

        ' Dim o2 As Object = a2.CreateInstance("clsDMSScan.clsDMS")
        Dim o2 As Object = a2.CreateInstance("DmsManager.Db.Documents")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  CreateInstance DmsManager.Db.Documents : " & Err.Description)
            Exit Sub
        End If

        ' Dim o3 As New clsDMSScan.clsDMS
        ' o3.frmVi.ShowDialog()

        cls1.NapuniCBLog("Zovem DocumentCheckIn u DmsManager.Db.Documents dokid=" & GridEX3.CurrentRow.Cells("dokid").Value.ToString)

        o2.DocumentCheckIn(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim, "")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  DocumentCheckIn u DmsManager.Db.Documents : " & Err.Description)
            Exit Sub
        End If



        ' refresh detaljaa
        If GridEX1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)
        End If

        DMS_pOkazi_Upit()

    End Sub


    Sub DMS_Obrisi()
        On Error Resume Next
        ContextMenuStrip4.Hide()
        cls1.NapuniCBLog("Zovem spNetGenDMSAzur @action='delete',@EntityName='...',@selectedids...")
        GreskaKodIzvrsavanjaSQLa = ""
        cls1.ExecSql("spNetGenDMSAzur @action='delete',@EntityName='" & GridEX3.CurrentRow.Cells("entityid").Value.ToString.Trim & "',@selectedids ='" & GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim & "'")

        ' 20.10.2026. KK - Vlatka: korisnik nije znao da brisanje nije prošlo.
        ' Npr. ako DMS upit ne vraća stupac entityid, ExecSql se uopće ne pozove (On Error Resume Next).
        ' SQL grešku ExecSql već sam prikaže, ovdje samo ono što do SQL-a nije ni došlo.
        ' Opis se odmah sprema jer NapuniCBLog i Translate (On Error) pri izlasku obrišu Err.
        If Err.Number <> 0 Then
            Dim strGreskaDMS As String = Err.Description
            If GreskaKodIzvrsavanjaSQLa = "" Then MsgBox(vbCr & Translate("Dokument nije obrisan.") & vbCr & vbCr & strGreskaDMS & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška kod brisanja DMS dokumenta : " & strGreskaDMS)
        End If

        ' refresh detaljaa
        If GridEX1.RowCount > 0 Then
            PokaziUpitDetalj(cls1.strStartupStoraDetalj)
        End If

        DMS_pOkazi_Upit()
    End Sub

    Sub DMS_SpremiNaDisk()

        ' MsgBox("hej")
        On Error Resume Next
        ContextMenuStrip4.Hide()
        ' Dim a2 As Assembly = Assembly.Load("DMSScan")
        Dim a2 As Assembly = Assembly.Load("DmsManagerV1")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška kod Assembly.Load DMSScan : " & Err.Description)
            Exit Sub
        End If

        ' Dim o2 As Object = a2.CreateInstance("clsDMSScan.clsDMS")
        Dim o2 As Object = a2.CreateInstance("DmsManagerV1.Start")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  CreateInstance DmsManagerV1.Start : " & Err.Description)
            Exit Sub
        End If

        ' Dim o3 As New clsDMSScan.clsDMS
        ' o3.frmVi.ShowDialog()
        cls1.NapuniCBLog("Zovem SaveDocuments u DmsManagerV1.Start dokid=" & GridEX3.CurrentRow.Cells("dokid").Value.ToString)

        o2.SaveDocuments(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim)


        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  SaveDocuments u DmsManagerV1.Start : " & Err.Description)
            Exit Sub
        End If

        ' o3.OpenDocument(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim, "")

    End Sub

    Sub DMS_Pogledaj()
        ' MsgBox("hej")
        On Error Resume Next
        ContextMenuStrip4.Hide()
        ' Dim a2 As Assembly = Assembly.Load("DMSScan")
        Dim a2 As Assembly = Assembly.Load("DmsManagerV1")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška kod Assembly.Load DMSScan : " & Err.Description)
            Exit Sub
        End If

        ' Dim o2 As Object = a2.CreateInstance("clsDMSScan.clsDMS")
        Dim o2 As Object = a2.CreateInstance("DmsManagerV1.Start")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  CreateInstance DmsManagerV1.Start : " & Err.Description)
            Exit Sub
        End If

        ' Dim o3 As New clsDMSScan.clsDMS
        ' o3.frmVi.ShowDialog()
        cls1.NapuniCBLog("Zovem OpenDocument u DmsManagerV1.Start dokid=" & GridEX3.CurrentRow.Cells("dokid").Value.ToString)

        o2.OpenDocument(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim, "")

        If Err.Number <> 0 Then
            MsgBox(vbCr & Err.Description & vbCr & vbCr, MsgBoxStyle.Critical, Translate("Poruka"))
            cls1.NapuniCBLog("Greška u  OpenDocument u DmsManagerV1.Start : " & Err.Description)
            Exit Sub
        End If

        ' o3.OpenDocument(cls1.strServer, cls1.strBaza, "", "", cls1.OperateriID, GridEX3.CurrentRow.Cells("dokid").Value.ToString.Trim, "")

    End Sub

    ' ovo su od lijevog taba 
    Private Sub TabControl1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControl1.DrawItem
        'Dim g As Graphics = e.Graphics
        'Dim pn As Pen = New Pen(Me.BackColor, 10)  '10 can be increased if you see bit of boarders
        'g.DrawRectangle(pn, TabPage2.Bounds)
    End Sub

    'Private Sub TabPage2_Paint(sender As Object, e As PaintEventArgs) Handles TabPage2.Paint
    '    'Me.TabControl1.Region = New Region(New RectangleF(Me.TabPage2.Left, Me.TabPage2.Top, Me.TabPage2.Width, Me.TabPage2.Height))
    'End Sub


    ' ovo su od det taba 
    'Private Sub TabControl2_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControl2.DrawItem
    '    If TabControl2.DrawMode = TabDrawMode.OwnerDrawFixed Then
    '        Dim g As Graphics = e.Graphics
    '        Dim pn As Pen = New Pen(Me.BackColor, 10)  '10 can be increased if you see bit of boarders
    '        g.DrawRectangle(pn, TabPage3.Bounds)
    '    End If
    'End Sub

    'Private Sub TabPage3_Paint(sender As Object, e As PaintEventArgs) Handles TabPage3.Paint
    '    If TabControl2.DrawMode = TabDrawMode.OwnerDrawFixed Then Me.TabControl2.Region = New Region(New RectangleF(Me.TabPage3.Left, Me.TabPage3.Top, Me.TabPage3.Width, Me.TabPage3.Height))
    'End Sub

    Private Sub GridEX3_RowDoubleClick(sender As Object, e As RowActionEventArgs) Handles GridEX3.RowDoubleClick
        DMS_Pogledaj()
    End Sub

    Private Sub ContextMenuStrip4_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip4.Opening

    End Sub

    Private Sub ParentV_Resize(sender As Object, e As EventArgs)
        On Error Resume Next
        Me.WindowState = FormWindowState.Normal
        Me.Location = New Point(0, 0)
        If m_parentControl IsNot Nothing Then
            Me.Size = m_parentControl.Size
        End If
    End Sub

    Private Sub ParentForm_LocationChanged(sender As Object, e As EventArgs)
        On Error Resume Next
        Me.WindowState = FormWindowState.Normal
        Me.Location = New Point(0, 0)
        If m_parentControl IsNot Nothing Then
            Me.Size = m_parentControl.Size
        End If
    End Sub

End Class

