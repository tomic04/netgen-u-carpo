Imports System.Windows.Forms
Imports System.Drawing
Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.Runtime.Remoting
Imports DevExpress.Utils


Imports System.Xml
Imports System.IO.IsolatedStorage



Imports ILCalc
Imports System.Net





Public Class Form2
    Public strInclude As String = ""
    Public Ikona As String
    Public cls1 As Class1
    Public strXML2 As String = ""
    Public strXMLfile2 As String = ""
    Public AktivnaKontola As Control
    Public strXML As String = ""
    Public OKClick As Boolean
    Public ID As String = ""
    Public DetID As String = ""
    Public Node As String = ""
    Public strImeForme As String = ""
    Public strStora As String = ""
    Public strONESCKey As String = ""
    Public strHelpKod2 As String = ""
    Public MasterMovePreviuos As Boolean = False
    Public MasterMoveNext As Boolean = False
    Public Version2 As String = ""
    Public strONClosed As String = ""
    Public strONClosing As String = ""

    Public Koler2 As String = ""

    Public strONActivated As String = ""
    Public strONLOADED As String = ""

    Public strStartupStora As String = ""
    Public strStartupStora2 As String = ""
    Public strPrevParametri As String = ""
    Public strOnLostFocusSQL As String = ""
    Dim TrenutnaKolona As Janus.Windows.GridEX.GridEXColumn
    Dim TrenutnaVrijednost As String = ""
    Dim StaraVrijednost As String = ""
    Dim DblClickedColumn As Janus.Windows.GridEX.GridEXColumn
    Dim FakPretvorbe As Long = 14
    Public strParametri As String = ""
    Public strParametriSvi As String = ""
    Public strOpisniParametri As String = ""
    Public EditColumns As String = ""
    Public strNextForm As String = ""
    Public strFillExplorer As String = False
    Public strFindNode As String = False
    Dim OldSearchString As String = ""
    Dim TabIndexCounter As Integer
    Public BrojKontrola As Integer = 0
    Dim Cdt As New DataTable
    Dim FormaJeAktivna As Boolean = False
    Public FormaJeVisibilna As Boolean = True
    Dim Translated As Boolean = False
    Dim NeZelimClosatFormu As Boolean = False
    Dim TempGrid As Grid
    Dim row5 As Janus.Windows.GridEX.GridEXRow
    Event Log5()
    Dim Topp As Long
    Dim Leftt As Long
    Dim CurTabIndex As Integer = 0
    Dim ZadnjiUporabljiviTOP As Long = 0
    Public strHelpKey As String = ""
    Public strRemotedllpath As String = ""
    Public Parent2 As Integer
    Public PrvaForma As Boolean = False

    Public strTimerSQL As String = ""

    Public FreezID As Boolean = False

    Public ControlClicked As Boolean

    Private Const TREE_RELATIONSHIP As String = "TreeParentChild"

    Public strSQLSERVERlogin As String = ""
    Public strSQLSERVERpassword As String = ""

    Dim o2 As New Object
    Dim o2str As String = ""
    Dim dontclose As Boolean = False
    Dim OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown As Boolean = False
    Public KupimnizAjdijeva As Boolean = False
    Dim strRSDBClipboard As String = ""
    Dim formClosingB As Boolean = False

    Public ulazniID As String = ""
    Public ulaznaForma As String = ""

    Public StrWINDOWSTATE As String = ""
    Public strMultiDropSQL As String = ""
    Public strSelectedPath As String = ""
    Public strSelectedFile As String = ""

    Public UsaoUrucniRadZacekiranje As Boolean = False
    Public UsaouforeachROW As Boolean = False



    <StructLayout(LayoutKind.Sequential)> _
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure




    Private Sub MYExnHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject
        ' MsgBox(EX.StackTrace)
    End Sub

    Private Sub MYThreadHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        ' MsgBox(e.Exception.StackTrace)
    End Sub

    Public Sub NapuniRSLang()
        'On Error Resume Next

        'Dim strSql As String
        'If RSDBClipboard Is Nothing Then
        '    RSDBClipboard = cls1.ExecSql("select * from NetGenDBClipboard where operaterid='" & cls1.OperateriID & "'")
        '    If Err.Number <> 0 Then
        '        'strSql = "CREATE TABLE [dbo].[NetGenDBClipboard]([NetGenDBClipboardID] [int] IDENTITY(1,1) NOT NULL,[OperaterID] [int] NOT NULL,[Key] [varchar](100) NOT NULL,[Value]([varchar](100)) CONSTRAINT [PK_NetGenDBClipboard] PRIMARY KEY CLUSTERED ([NetGenDBClipboardID](Asc)))"
        '        'cls1.ExecSql(strSql)
        '        'RSDBClipboard = cls1.ExecSql("select * from NetGenDBClipboard where operaterid='" & cls1.OperateriID & "'")
        '    End If
        'End If
        'Err.Clear()


        'JupiterPrevoditelj.Database = "jupitertranslations"
        'JupiterPrevoditelj.Server = "kkovac-lap"
        'JupiterPrevoditelj.Server = Conn.DataSource
        'JupiterPrevoditelj.Username = ""
        'JupiterPrevoditelj.Password = ""
        'JupiterPrevoditelj.Language = "hun"
        'JupiterPrevoditelj.Init()
        On Error Resume Next



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

    Public Sub Run2(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String)

        Dim Form2b As New Form2
        Dim cf As Form2

        Dim Trs As New DataTable
        cls1 = New Class1
        cls1.strServer = server
        cls1.strBaza = database
        cls1.OperateriID = Operateriid
        strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid
        strRemotedllpath = remotedllpath
        Conn = New SqlClient.SqlConnection
        Conn2 = New OleDb.OleDbConnection

        'frm1 = Me
        'frm1 = New Form1
        frm3 = New Form3
        frm5 = New Form5
        frm3.cls1 = cls1
        frm5.cls1 = cls1

        'On Error Resume Next

        'If cls1.strUser = "" Then
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true"
        'Else
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & cls1.strUser & ";PWD=" & cls1.strPassword & ";"
        'End If

        'If strSQLSERVERlogin <> "" Then
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        'Else
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        'End If

        Dim tstr5 As String = ""


        'If strSQLSERVERlogin = "" Then
        '    tstr5 = "Data Source=" & cls1.strServer & ";Initial Catalog=" & cls1.strBaza & ";Integrated Security=True;Application Name=" & ImeAplikacijeZaProfiler & ";"
        'Else
        '    tstr5 = "Data Source=" & cls1.strServer & ";Initial Catalog=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        'End If

        If strSQLSERVERlogin = "" Then
            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        Else
            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        End If

        strGlobalSQLSERVERlogin = tstr5

        '  MsgBox(strSQLSERVERlogin & vbCrLf & strSQLSERVERpassword & vbCrLf & tstr5)

        Conn.ConnectionString = tstr5
        Err.Clear()
        Conn.Open()

        If Err.Number <> 0 Then
            'MsgBox(Err.Description & vbCrLf & Conn.ConnectionString) kkkk
            Exit Sub
        End If

        If cls1.OperateriID = 0 Then PronadjiOperatera()



        NapuniRSLang()

        ID = ""
        cls1.MasterID = ID

        SetParent(Handle.ToInt32, Parent) ' #######
        'Form2b.SetParent(Handle.ToInt32, Parent)
        Form2b.cls1 = cls1

        Form2b.ID = 0
        Form2b.DetID = 0
        cls1.DetIDIzKlase = Form2b.DetID
        Form2b.Node = Node
        cls1.MasterID = Form2b.ID

        Form2b.strPrevParametri = ""

        cls1.strSviParametriuModulu = ""

        cf = Form2b

        Form2b.OKClick = False
        Form2b.BrojKontrola = 0
        Form2b.strXML = strXML2
        ' Form2b.strXML = strXMLfile2
        Form2b.strXML2 = strXML2
        ' Form2b.strXMLfile2 = strXMLfile2
        Form2b.strImeForme = "MAINFORM"

        Form2b.ProcitajXML()

        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do greške na ONLOAD forme idemo van

        If Form2b.BrojKontrola > 0 Then
            If Form2b.FormaJeVisibilna = False Then
                Form2b.PokupiParametre()
                Form2b.OKClick = True
            Else
                Form2b.ShowDialog()
            End If
        Else
            Form2b.OKClick = True
        End If


        'While cf.OKClick And cf.strNextForm <> ""
        '    Dim PrevParametri As String = IIf(cf.strParametri = "", cf.strPrevParametri, cf.strParametri)
        '    Dim tstrNextForm As String = cf.strNextForm
        '    Tid = cf.ID
        '    If cf.strStora <> "" Then
        '        Trs = cls1.ExecSql(cf.strStora & " " & cf.strParametri)
        '        If Not Trs Is Nothing Then
        '            If Trs.Rows.Count > 0 Then
        '                Tid = Trs.Rows(0)(0)
        '            End If
        '        End If
        '    End If
        '    cf.Close()
        '    Dim Form2c As New Form2
        '    Form2c.cls1 = cls1
        '    cf = Form2c
        '    Form2c.ID = Tid
        '    cls1.MasterID = Tid
        '    Form2c.BrojKontrola = 0
        '    Form2c.strPrevParametri = PrevParametri
        '    Form2c.OKClick = False
        '    Form2c.strXML = strXML2 & "\" & strXMLfile2
        '    Form2c.strXML = strXMLfile2
        '    Form2c.strXML2 = strXML2
        '    Form2c.strXMLfile2 = strXMLfile2
        '    Form2c.strImeForme = tstrNextForm
        '    Form2c.ProcitajXML()
        '    If Form2b.BrojKontrola > 0 Then Form2c.ShowDialog() Else Form2c.OKClick = True
        'End While


        Try
            ' Show()
        Catch ex As Exception
            '  MessageBox.Show(ex.Message)
        End Try

    End Sub


    Public Sub Run(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String, Optional ByVal Koler As String = "")

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


        Dim Trs As New DataTable
        cls1 = New Class1
        cls1.strServer = server
        cls1.strBaza = database
        cls1.OperateriID = Operateriid
        strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid
        strRemotedllpath = remotedllpath
        cls1.strXMLsakojimRadimzaCBlog = strXML2

        ' 08.09.2026. KK - prikljuci se na JupiterLogger (vidi JupiterLog.vb)
        JupiterLog.Inicijaliziraj(Mid)
        cls1.NapuniCBLog(" JupiterLogger : " & JupiterLog.Dijagnostika)
        JupiterLog.Info("Run", "open " & Mid & " | baza: " & database)

        cls1.NapuniCBLog(" ušo u run u frm , xml sa kojim radim : " & strXML2)

        ' MsgBox("Miki xml sa kojim radim " & strXML2)

        Conn = New SqlClient.SqlConnection
        Conn3 = New SqlClient.SqlConnection
        Conn2 = New OleDb.OleDbConnection
        Parent2 = Parent
        Koler2 = Koler
        'frm1 = Me
        'frm1 = New Form1
        frm3 = New Form3
        frm5 = New Form5
        frm3.cls1 = cls1
        frm5.cls1 = cls1




        MOJremotedllpath = remotedllpath

        strSQLSERVERlogin = username
        strSQLSERVERpassword = password

        If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
        If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""


        'If cls1.strUser = "" Then
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true"
        'Else
        '    Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & cls1.strUser & ";PWD=" & cls1.strPassword & ";"
        'End If


        If strSQLSERVERlogin <> "" Then
            Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
            strGlobalSQLSERVERlogin = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        Else
            Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
            strGlobalSQLSERVERlogin = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        End If


        cls1.NapuniCBLog(" prije otvaranja konekcije, cs :  " & Conn.ConnectionString)
        Conn.Open()

        cls1.NapuniCBLog(" nakon otvaranja konekcije :  " & Conn.ConnectionString)

        'MsgBox("tu sam" & Err.Description)

        If cls1.OperateriID = 0 Then PronadjiOperatera()

        cls1.NapuniCBLog(" prije NapuniRSLang() ")
        NapuniRSLang()
        cls1.NapuniCBLog(" nakon NapuniRSLang() ")

        ID = ""

        cls1.MasterID = ID
        'Me.WindowState = FormWindowState.Maximized
        'SetParent(Handle.ToInt32, Parent)

        Me.cls1 = cls1

        ID = 0
        DetID = 0
        cls1.DetIDIzKlase = DetID
        Node = Node
        If ulazniID <> "" Then ID = ulazniID
        cls1.MasterID = ID

        strPrevParametri = ""

        cls1.strSviParametriuModulu = ""

        OKClick = False
        BrojKontrola = 0
        strXML = strXML2
        ' Form2b.strXML = strXMLfile2
        strXML2 = strXML2
        strXMLfile2 = strXML2
        strImeForme = "MAINFORM"
        If ulaznaForma <> "" Then strImeForme = ulaznaForma

        PrvaForma = True
        'MsgBox("1")
        cls1.NapuniCBLog(" prije ProcitajXML() ")


        ProcitajXML()
        cls1.NapuniCBLog(" nakon  ProcitajXML() ")
        'MsgBox("2")
        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do greške na ONLOAD forme idemo van

        If BrojKontrola > 0 Then
            If FormaJeVisibilna = False Then
                PokupiParametre()
                OKClick = True
            Else
                Me.Show()
                'MsgBox("tu sam 3" & BrojKontrola)
            End If
        Else
            OKClick = True
        End If



    End Sub



    Public Sub Run3(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String)

        ' ovo je zbog Vitnerovog poziva fiskalizacije iz veterine

        cls1 = New Class1
        cls1.strServer = server
        cls1.strBaza = database
        cls1.OperateriID = Operateriid
        strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid
        strRemotedllpath = remotedllpath
        cls1.strXMLsakojimRadimzaCBlog = strXML2
        Conn = New SqlClient.SqlConnection
        Conn3 = New SqlClient.SqlConnection
        Conn2 = New OleDb.OleDbConnection
        Parent2 = Parent
        'frm1 = Me
        'frm1 = New Form1
        frm3 = New Form3
        frm5 = New Form5
        frm3.cls1 = cls1
        frm5.cls1 = cls1

        MOJremotedllpath = remotedllpath

        strSQLSERVERlogin = username
        strSQLSERVERpassword = password

        If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
        If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""



        If strSQLSERVERlogin <> "" Then
            Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
        Else
            Conn.ConnectionString = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
        End If


        'MsgBox("tu sam" & Err.Description)


        Conn.Open()



        ID = ""
        cls1.MasterID = ID

        Me.cls1 = cls1

        ID = 0
        DetID = 0
        cls1.DetIDIzKlase = DetID
        Node = Node
        cls1.MasterID = ID

        strPrevParametri = ""

        cls1.strSviParametriuModulu = ""

        OKClick = False
        BrojKontrola = 0
        strXML = strXML2
        ' Form2b.strXML = strXMLfile2
        strXML2 = strXML2
        strXMLfile2 = strXML2
        strImeForme = "MAINFORM"




    End Sub



    <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetParent(ByVal child As Integer, ByVal newParent As Integer) As Integer
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInt32) As Boolean
    End Function

    ' Properties

    Public WriteOnly Property ModuleID() As String
        Set(ByVal value As String)
            Me.Mid = value
        End Set
    End Property

    Public ReadOnly Property hWnd() As Integer
        Get
            'Return MyBase.Handle.ToInt32
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
    Public Mid As String
    Public Argument5 As String



    Sub NapuniKontroleZaEdit()
        ' ovo je pizdarija jer SelectedValue ne prolazi prije lodanja forme
        On Error Resume Next
        For Each ctl As Control In Controls
            If TypeOf ctl Is Combo Then
                Dim Tctl As Combo = ctl

                If Tctl.Tag.ToString.IndexOf("##") > 0 Then
                    Err.Clear()
                    Tctl.SelectedValue = Tctl.Tag.ToString.Substring(Tctl.Tag.ToString.IndexOf("##"), Tctl.Tag.ToString.Length - Tctl.Tag.ToString.IndexOf("##")).Replace("##", "")
                    Tctl.Tag = Tctl.Tag.ToString.Substring(0, Tctl.Tag.ToString.IndexOf("##"))
                    If Err.Number <> 0 Then
                        'Tctl.SelectedValue = 0
                    End If
                End If
            End If

            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                RucniRadNaGriduzaCheckiranjeRedova(Tctl)

                'Tctl.Row = 0
                'Tctl.Col = 12 ' Tctl.CurrentColumn.Index
                'Tctl.SelectCurrentCellText()

            End If

            If TypeOf ctl Is Label Then
                Dim Tctl As Label = ctl
                If Tctl.Tag <> "" Then
                    Tctl.Left = Tctl.Tag - Tctl.Width
                End If
            End If

        Next ctl

    End Sub

    'Private Function GetLastDayInMonth(ByVal dDate As Date) As Date
    '    dDate = DateAdd(DateInterval.Month, 1, dDate)
    '    dDate = Convert.ToDateTime(Month(dDate).ToString() & "/" & "1/" & Year(dDate).ToString())
    '    dDate = DateAdd(DateInterval.Day, -1, dDate)
    '    Return dDate
    'End Function


    Public Sub ProcitajXML()
        On Error Resume Next
        Dim i As Integer
        Dim Tnod As String = ""
        strBF2 = ""

        'MsgBox("Miki ušo u ProcitajXML ")

        Dim Topp As Long = 200
        Dim Leftt As Long = 2600
        Dim TabIndexx As Long = 0

        Dim Ime5 As String
        Dim Nasao5 As Integer
        Dim Vrijednost5 As String
        Dim j As Long
        Dim strIcon As String = ""


        CurTabIndex = -1
        ZadnjiUporabljiviTOP = 0
        strNextForm = ""
        strStartupStora = ""
        strStartupStora2 = ""
        strONClosed = ""
        strONClosing = ""
        strONActivated = ""
        strONLOADED = ""
        strRSDBClipboard = ""

        strStora = ""

        strOnLostFocusSQL = ""
        BrojKontrola = 0
        strFillExplorer = ""
        strFindNode = ""

        Cdt.Columns.Add("control", Type.GetType("System.String"))
        Cdt.Columns.Add("sql", Type.GetType("System.String"))
        Cdt.Columns.Add("tip", Type.GetType("System.String"))

        cls1.UpaliProgres()
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        TabIndexCounter = 0
        ' If Err.Number <> 0 Then MsgBox(Err.Description)

        Button1.Text = ""
        Button2.Text = ""
        Button3.Text = ""
        Button4.Text = ""
        Button5.Text = ""
        Button6.Text = ""
        Version2 = ""
        strStora = ""
        strONESCKey = ""
        strTimerSQL = ""

        strStartupStora = ""
        strStartupStora2 = ""
        strAktivnaKontrolanakonLodanja = ""
        GreskaKodIzvrsavanjaSQLa = ""


        'Dim m_xmld As Xml.XmlDocument
        Dim m_nodelist As Xml.XmlNodeList
        Dim m_node As Xml.XmlNode
        Dim m_node5 As Xml.XmlNode


        Err.Clear()



        '====================      Dodao zbog Mikija 28.11.2023.
        m_xmld = Nothing
        m_xmld.InnerXml = ""


        ' ovo moram još riješiti da ako radi sa xmlom sa diska da ipak svaki puta učitava xml

        'm_xmld = New Xml.XmlDocument()
        'm_xmld.Load(strXML)

        If cls1.strXMLsakojimRadimzaCBlog.StartsWith("JupiterXML Database") Then



            If m_xmld Is Nothing Then
                m_xmld = New Xml.XmlDocument()
                m_xmld.Load(strXML)
            End If

        Else


            '--------------------------------------------------------------------------- XML iz DMS baze 
            Dim a8 As Object
            'MsgBox("Miki  Assembly.Load(SpinDMSUtil) ")
            a8 = Assembly.Load("SpinDMSUtil")
            If Err.Number <> 0 Then
                cls1.NapuniCBLog("Greška kod FORM2 Assembly.Load(SpinDMSUtil) : " & Err.Description)
            End If
            If Err.Number = 0 Then
                Err.Clear()
                Dim o As Object = a8.CreateInstance("SpinDMSUtil.NetGenFinder")
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod FORM2 CreateInstance(SpinDMSUtil.NetGenFinder) : " & Err.Description)
                End If
                ' strXMLsakojimRadim = "C:\Razmjena\NG\jupitersrv\SharedDLLs\xml\akcije.xml"
                m_xmld = o.GetFile(strXML)
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod FORM2 GetFile: " & Mid & " - " & Err.Description)
                End If
                cls1.NapuniCBLog("SpinDMSUtil FORM2 Source : " & o.source)
                cls1.NapuniCBLog("SpinDMSUtil FORM2 exception : " & o.exception)
                cls1.NapuniCBLog("SpinDMSUtil FORM2 connstring : " & o.connstring)
                cls1.NapuniCBLog("SpinDMSUtil FORM2 JupiterClientIniPath : " & o.JupiterClientIniPath)
                'If Err.Number = 0 Then
                cls1.strXMLsakojimRadimzaCBlog = "SpinDMSUtil FORM2 XML PUTANJA : " & strXML
                'End If
        End If

        '------------------------------------------------------------------------------ end of XML iz DMS baze 

            ' MsgBox("Miki  Učitavam XML na stari način sa file sistema ... ")


            If m_xmld.InnerXml = "" Then ' ako nije usplio preko DMS-a idemo direktno na fajsistem
                ' MsgBox("Miki    m_xmld.Load(strXML) " & strXML)
                cls1.NapuniCBLog("Učitavam XML na stari način sa file sistema ...")
                m_xmld = New Xml.XmlDocument()
                m_xmld.Load(strXML)
            End If

            End If



            ' -----------------------------------------------INCLUDE ' $$$$$
            If strInclude.Trim = "" Then
                m_node5 = m_xmld.SelectSingleNode("/PROJECT/" & strImeForme)
                strInclude = m_node5.Attributes.GetNamedItem("INCLUDE").Value.Trim()
                If strInclude.Trim <> "" Then cls1.strIncludeGlobal = strInclude
                If strInclude.Trim <> "" Then cls1.NapuniCBLog(" strIncludeGlobal = " & strInclude)
            End If
            '-----------------------------------------------------------------------------

            ' -----------------------------------------------INCLUDE
            If cls1.strIncludeGlobal <> "" Then
                Dim m_xmldINCLUDE2 As Xml.XmlDocument
                m_xmldINCLUDE2 = New Xml.XmlDocument()
                Dim xnode As Xml.XmlNode
                Dim stINCL As String = ""
                Dim Nadjen As Boolean = False

                Dim a() As String

                a = cls1.strIncludeGlobal.Split("#")

                For j = 0 To a.GetUpperBound(0)

                    strInclude = a(j)

                    stINCL = strXML.Replace(strXML.Substring(InStrRev(strXML, "\"), strXML.Length - InStrRev(strXML, "\")), strInclude)
                    cls1.NapuniCBLog(" stINCL = " & stINCL)
                Err.Clear()


                '--------------------------------------------------------------------------- XML iz DMS baze 
                Dim a9 As Object

                a9 = Assembly.Load("SpinDMSUtil")
                If Err.Number <> 0 Then
                    cls1.NapuniCBLog("Greška kod FORM2 INCLUDE Assembly.Load(SpinDMSUtil) : " & Err.Description)
                End If
                If Err.Number = 0 Then
                    Err.Clear()
                    Dim o As Object = a9.CreateInstance("SpinDMSUtil.NetGenFinder")
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod FORM2 INCLUDE CreateInstance(SpinDMSUtil.NetGenFinder) : " & Err.Description)
                    End If
                    ' strXMLsakojimRadim = "C:\Razmjena\NG\jupitersrv\SharedDLLs\xml\akcije.xml"
                    m_xmldINCLUDE2 = o.GetFile(stINCL)
                    If Err.Number <> 0 Then
                        cls1.NapuniCBLog("Greška kod FORM2 INCLUDE GetFile: " & Mid & " - " & Err.Description)
                    End If
                    cls1.NapuniCBLog("SpinDMSUtil FORM2 INCLUDE Source : " & o.source)
                    cls1.NapuniCBLog("SpinDMSUtil FORM2 INCLUDE exception : " & o.exception)
                    cls1.NapuniCBLog("SpinDMSUtil FORM2 INCLUDE connstring : " & o.connstring)
                    cls1.NapuniCBLog("SpinDMSUtil FORM2 INCLUDE JupiterClientIniPath : " & o.JupiterClientIniPath)
                    'If Err.Number = 0 Then
                    cls1.strXMLsakojimRadimzaCBlog = "SpinDMSUtil FORM2 INCLUDE XML PUTANJA : " & stINCL
                    'End If
                End If

        '------------------------------------------------------------------------------ end of XML iz DMS baze 

        Err.Clear()

        If m_xmldINCLUDE2.InnerXml = "" Then ' ako nije usplio preko DMS-a idemo direktno na fajsistem
            cls1.NapuniCBLog("Učitavam XML na stari način sa file sistema ...")
            m_xmldINCLUDE2 = New Xml.XmlDocument()
            m_xmldINCLUDE2.Load(stINCL)
        End If



        cls1.NapuniCBLog(" m_xmldINCLUDE2.Load  , " & Err.Description)

        ' Dim xnodeListISHODISNI As Xml.XmlNodeList = m_xmld.SelectNodes("/PROJECT")
        Dim xnodeList As Xml.XmlNodeList = m_xmldINCLUDE2.SelectNodes("/PROJECT")
        For Each xnode In xnodeList
            Dim i6 As Integer = 0
            Dim i7 As Integer = 0
            Dim strNN6 As String = ""
            Dim strNN7 As String = ""
            For i6 = 0 To xnode.ChildNodes.Count
                strNN6 = xnode.ChildNodes.ItemOf(i6).Name
                Nadjen = False

                'For i7 = 1 To xnodeListISHODISNI.ChildNodes.Count
                '    strNN7 = m_node.ChildNodes.ItemOf(i7).Name
                '    If strNN6 = strNN7 Then
                '        Nadjen = True ' takav nod već postoji u ishodišnom xml i nema potrebe ga dodavati
                '    End If
                'Next

                If Nadjen = False Then
                    m_xmld.DocumentElement.AppendChild(m_xmld.ImportNode(xnode.ChildNodes.ItemOf(i6), True))
                End If
            Next
        Next

            Next


            End If
            ' -----------------------------------------------END OF INCLUDE


            If strImeForme.ToLower = "mainform" Then
                ' Me.MinimizeBox = True
                ' Me.MaximizeBox = True
            End If


            Err.Clear()

            strBFformname = ""
            strBFformname = strImeForme.Trim.ToLower

            m_nodelist = m_xmld.SelectNodes("/PROJECT/" & strImeForme)
            If Err.Number <> 0 Then
                cls1.UgasiProgres()
                Exit Sub
            End If

            For Each m_node In m_nodelist

                cls1.NapuniCBLog(strImeForme & " : For Each m_node In m_nodelist : " & Err.Description)

                FormaJeVisibilna = True
                FormaJeVisibilna = m_node.Attributes.GetNamedItem("VISIBLE").Value.Trim
                Me.Text = m_node.Attributes.GetNamedItem("CAPTION").Value.Trim

                strIcon = m_node.Attributes.GetNamedItem("ICON").Value.Trim

                If strIcon.Trim = "" Then

                    Dim bmp As Bitmap = New System.Drawing.Bitmap(Ikona)
                    Me.Icon = Icon.FromHandle(bmp.GetHicon)
                    'MsgBox(Err.Description)

                Else
                    Me.Icon = New System.Drawing.Icon(MOJremotedllpath & "\Graphics\Icons\" & strIcon)
                End If



                'Me.Icon = New System.Drawing.Icon(MOJremotedllpath & "\Graphics\Icons\" & strIcon)


                ' Me.WindowState = m_node.Attributes.GetNamedItem("WINDOWSTATE").Value.Trim

                StrWINDOWSTATE = ""
                StrWINDOWSTATE = m_node.Attributes.GetNamedItem("WINDOWSTATE").Value.Trim

            If PrvaForma Then
                SetParent(Handle.ToInt32, Parent2)


                If Koler2 = "Jup10" Then

                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None

                    Dim parentV As Control = Control.FromHandle(Parent2)

                    If parentV IsNot Nothing Then
                        AddHandler parentV.Resize,
                            Sub(sender, e)
                                ' svaki put kad parentV promijeni veličinu
                                '  SetParent(Me.Handle, parentV.Handle)
                                Me.WindowState = FormWindowState.Normal
                                Me.Location = New Point(0, 0)
                                Me.Size = parentV.Size

                            End Sub
                    End If


                    If parentV IsNot Nothing Then
                        AddHandler parentV.FindForm().LocationChanged,
                            Sub(sender, e)
                                ' svaki put kad parentV promijeni veličinu
                                '  SetParent(Me.Handle, parentV.Handle)
                                Me.WindowState = FormWindowState.Normal
                                Me.Location = New Point(0, 0)
                                Me.Size = parentV.Size
                            End Sub
                    End If
                End If

            End If


                Me.MaximizeBox = m_node.Attributes.GetNamedItem("MAXIMIZEBOX").Value.Trim

                strTimerSQL = m_node.Attributes.GetNamedItem("TIMERSQL").Value.Trim

                strCommandTimeout = m_node.Attributes.GetNamedItem("COMMANDTIMEOUT").InnerText.ToUpper
                strStora = m_node.Attributes.GetNamedItem("STORA").Value.Trim
                strONESCKey = m_node.Attributes.GetNamedItem("ONESCKEY").Value.Trim

                strStora = m_node.Attributes.GetNamedItem("ONSUBMIT").Value.Trim
                cls1.strStartupStoraDetalj = m_node.Attributes.GetNamedItem("DETAILGRID").Value.Trim
                strStartupStora = m_node.Attributes.GetNamedItem("STARTUPSTORA").Value.Trim
                strStartupStora = m_node.Attributes.GetNamedItem("ONLOAD").Value.Trim
                strONClosed = m_node.Attributes.GetNamedItem("ONCLOSED").Value.Trim
                strONClosing = m_node.Attributes.GetNamedItem("ONCLOSING").Value.Trim
                strONActivated = m_node.Attributes.GetNamedItem("ONACTIVATED").Value.Trim
                strONLOADED = m_node.Attributes.GetNamedItem("ONLOADED").Value.Trim
                strStartupStora2 = m_node.Attributes.GetNamedItem("ONLOAD2").Value.Trim
                strHelpKod2 = ""
                strHelpKod2 = m_node.Attributes.GetNamedItem("HELPKOD").InnerText.ToUpper
                If strHelpKod2 = "" Then strHelpKod2 = strHelpKod
                strOnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value.Trim
                strNextForm = m_node.Attributes.GetNamedItem("NEXTFORM").Value.ToString.Trim
                strAktivnaKontrolanakonLodanja = m_node.Attributes.GetNamedItem("FOCUS").Value.ToString.Trim

                strAktivnaKontrolanakonLodanja = m_node.Attributes.GetNamedItem("FOCUS").Value.ToString.Trim

                DefaultConnection = 1
                DefaultConnection = m_node.Attributes.GetNamedItem("DEFAULTCONNECTION").InnerText.ToUpper

                strCUSTOMCONNECTIONSTRING = m_node.Attributes.GetNamedItem("CUSTOMCONNECTIONSTRING").Value.ToString.Trim
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD1CAPTION").Value.Trim
                If Tnod <> "" Then Button1.Text = Tnod
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD2CAPTION").Value.Trim
                If Tnod <> "" Then Button2.Text = Tnod
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD3CAPTION").Value.Trim
                If Tnod <> "" Then
                    Button3.Text = Tnod.Substring(0, Tnod.IndexOf(":"))
                    Button3.Tag = Tnod.Trim.Substring(Tnod.Trim.IndexOf(":") + 1, Tnod.Trim.Length - Tnod.Trim.IndexOf(":") - 1)
                End If
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD4CAPTION").Value.Trim
                If Tnod <> "" Then
                    Button4.Text = Tnod.Substring(0, Tnod.IndexOf(":"))
                    Button4.Tag = Tnod.Trim.Substring(Tnod.Trim.IndexOf(":") + 1, Tnod.Trim.Length - Tnod.Trim.IndexOf(":") - 1)
                End If
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD5CAPTION").Value.Trim
                If Tnod <> "" Then
                    Button5.Text = Tnod.Substring(0, Tnod.IndexOf(":"))
                    Button5.Tag = Tnod.Trim.Substring(Tnod.Trim.IndexOf(":") + 1, Tnod.Trim.Length - Tnod.Trim.IndexOf(":") - 1)
                End If
                Tnod = ""
                Tnod = m_node.Attributes.GetNamedItem("CMD6CAPTION").Value.Trim
                If Tnod <> "" Then
                    Button6.Text = Tnod.Substring(0, Tnod.IndexOf(":"))
                    Button6.Tag = Tnod.Trim.Substring(Tnod.Trim.IndexOf(":") + 1, Tnod.Trim.Length - Tnod.Trim.IndexOf(":") - 1)
                End If

                strFillExplorer = m_node.Attributes.GetNamedItem("FILLEXPLORER").Value.Trim
                Me.Width = m_node.Attributes.GetNamedItem("WIDTH").Value / FakPretvorbe
                Me.Height = m_node.Attributes.GetNamedItem("HEIGHT").Value / FakPretvorbe

                Version2 = m_node.Attributes.GetNamedItem("VERSION").Value.Trim
                strPermisio = m_node.Attributes.GetNamedItem("PERMISIO").Value.Trim

                strRSDBClipboard = m_node.Attributes.GetNamedItem("DBCLIPBOARD").Value.Trim


                If InStr(strImeForme.ToLower, "filter") And strRSDBClipboard.ToLower <> "false" Then
                    strRSDBClipboard = "true"
                End If

                Dim sForeColor As String = ""
                Dim sBackColor As String = ""

                sForeColor = m_node.Attributes.GetNamedItem("FORECOLOR").Value.Trim
                sBackColor = m_node.Attributes.GetNamedItem("BACKCOLOR").Value.Trim

                If sForeColor.StartsWith("#") Then
                    Me.ForeColor = System.Drawing.ColorTranslator.FromHtml(sForeColor)
                End If

                If sBackColor.StartsWith("#") Then
                    Me.BackColor = System.Drawing.ColorTranslator.FromHtml(sBackColor)
                End If

                Dim strRezise As String = ""
                strRezise = m_node.Attributes.GetNamedItem("RESIZE").Value.Trim
                If strRezise.ToLower = "false" Then
                    ' Me.ControlBox = False
                    ' Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedDialog
                    Me.MaximizeBox = False
                    Me.MinimizeBox = False
                    Me.MaximumSize = Me.Size
                    Me.MinimumSize = Me.Size
                End If



            Next



            ' za 3th konekciju
            If strCUSTOMCONNECTIONSTRING <> "" Then
                Conn3.ConnectionString = strCUSTOMCONNECTIONSTRING
                Conn3.Open()
            End If
            '--------------------------------



            strStartupStora = cls1.SrediSQLPosljeNextForme(SrediSQL2(strStartupStora))
            strStartupStora2 = cls1.SrediSQLPosljeNextForme(SrediSQL2(strStartupStora2))

            m_nodelist = m_xmld.SelectNodes("/PROJECT/" & strImeForme & "/INPUT")

            'Me.Text = Me.Text & "  [@@ID=" & ID & "]" & "  [@@DETID=" & DetID & "]" & "  [@@NODE=" & Node & "]"

            Dim Trs2 As New DataTable
            Dim Trs3 As New DataTable

            Dim RSDBClipboard As DataTable

            Dim prevParamizStartupStore As String = ""

            'Trs2 = cls1.ExecSql(strStartupStora)
            'Dim Column As DataColumn
            'If Trs2.Rows.Count > 0 Then
            '    For Each Column In Trs2.Columns
            '        prevParamizStartupStore = prevParamizStartupStore & "@" & Column.Caption & "=" & Trs2.Rows(0)(Column.Caption) & ","
            '    Next
            'End If

            Err.Clear()

            If strRSDBClipboard.Trim.ToLower = "true" Then
                'If RSDBClipboard Is Nothing Then
                RSDBClipboard = cls1.ExecSql("select * from NetGenDBClipboard where operaterid='" & cls1.OperateriID & "'")
                If Err.Number <> 0 Then
                    strRSDBClipboard = ""
                    RSDBClipboard = Nothing
                    'strSql = "CREATE TABLE [dbo].[NetGenDBClipboard]([NetGenDBClipboardID] [int] IDENTITY(1,1) NOT NULL,[OperaterID] [int] NOT NULL,[Key] [varchar](100) NOT NULL,[Value]([varchar](100)) CONSTRAINT [PK_NetGenDBClipboard] PRIMARY KEY CLUSTERED ([NetGenDBClipboardID](Asc)))"
                    'cls1.ExecSql(strSql)
                    'RSDBClipboard = cls1.ExecSql("select * from NetGenDBClipboard where operaterid='" & cls1.OperateriID & "'")
                End If
                'End If
            Else
                RSDBClipboard = Nothing
            End If

            Err.Clear()


            If strStartupStora2.Trim <> "" Then
                Trs3 = cls1.ExecSql(strStartupStora2)
                If Err.Number <> 0 Then
                    strStartupStora2 = ""
                End If
            End If

            If strStartupStora.Trim <> "" Then
                Err.Clear()
                Trs2 = cls1.ExecSql(strStartupStora)

                If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do nekontrolirane greške na ONLOAD forme idemo van

                If Err.Number = 0 Then
                    Dim Column As DataColumn
                    If Trs2.Rows.Count > 0 Then
                        For Each Column In Trs2.Columns
                            prevParamizStartupStore = prevParamizStartupStore & "@" & Column.Caption & "=" & Trs2.Rows(0)(Column.Caption) & ","

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
                                ' 27.11.2008 odkomentirao , onload može i dodati vrijednosti u cb
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

            Me.Text = cls1.SrediSQLPosljeNextForme(SrediSQL2(Me.Text), , True)
            StatusStrip1.Visible = False


            Dim xmlname5 As String
            xmlname5 = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))
            xmlname5 = xmlname5.Trim.ToLower

            Err.Clear()




            For Each m_node In m_nodelist


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "statusstrip", CompareMethod.Text) Then
                    StatusStrip1.Visible = True
                    ToolStripStatusLabel1.Tag = m_node.Attributes.GetNamedItem("TEXT1").Value.ToString.ToUpper
                    ToolStripStatusLabel2.Tag = m_node.Attributes.GetNamedItem("TEXT2").Value.ToString.ToUpper
                    ToolStripStatusLabel3.Tag = m_node.Attributes.GetNamedItem("TEXT3").Value.ToString.ToUpper
                Else

                End If



                'If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "statusstrip", CompareMethod.Text) Then
                '    Dim Glbl As New StatusStrip
                '    Glbl.BackColor = Color.Transparent
                '    SrediKontrolu(Glbl, m_node)
                '    If Trs2.Rows.Count > 0 Then
                '        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                '        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                '    End If

                '    If strStartupStora2 <> "" Then
                '        If Trs3.Rows.Count > 0 Then
                '            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                '            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                '        End If
                '    End If

                '    Glbl.Text = Translate(cls1.SrediSQLPosljeNextForme(SrediSQL2(Glbl.Text)))
                '    Glbl.AutoSize = True
                '    Controls.Add(Glbl)
                '    Glbl.Dock = 2
                '    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                '    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                '    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                '    Glbl.BringToFront()
                '    AddHandler Glbl.Click, AddressOf Glbl_Click
                'End If



                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "label", CompareMethod.Text) Then
                    Dim Glbl As New Label

                    Dim sBackC As String = ""
                    sBackC = m_node.Attributes.GetNamedItem("BACKCOLOR").Value.ToString.ToUpper
                    If sBackC <> "" Then
                        Dim c1 As Color = System.Drawing.ColorTranslator.FromHtml(sBackC)
                        Glbl.BackColor = c1
                    Else
                        Glbl.BackColor = Color.Transparent
                    End If



                    SrediKontrolu(Glbl, m_node)
                    Glbl.Tag = ""
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.Text = Translate(cls1.SrediSQLPosljeNextForme(SrediSQL2(Glbl.Text)))
                    If Glbl.Text.Trim.ToLower = "null" Then Glbl.Text = ""


                    Dim lHEIGHT As String = ""
                    Dim lWIDTH As String = ""
                    lWIDTH = m_node.Attributes.GetNamedItem("WIDTH").Value.ToString.ToUpper
                    lHEIGHT = m_node.Attributes.GetNamedItem("HEIGHT").Value.ToString.ToUpper

                    If lHEIGHT <> "" And lWIDTH <> "" Then
                        Glbl.AutoSize = False
                        Glbl.Height = lHEIGHT / FakPretvorbe
                        Glbl.Width = lWIDTH / FakPretvorbe
                    Else
                        Glbl.AutoSize = True
                    End If

                    'Glbl.AutoSize = True
                    Glbl.SendToBack()
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Tag = m_node.Attributes.GetNamedItem("ONCLICK").Value.ToString.ToUpper
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                End If



                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "group", CompareMethod.Text) Or InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "frame", CompareMethod.Text) Then
                    Dim Glbl As New GroupBox
                    Glbl.BackColor = Color.Transparent
                    SrediKontrolu(Glbl, m_node)
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.Text = cls1.SrediSQLPosljeNextForme(SrediSQL2(Glbl.Text))


                    'Glbl.AutoSize = True
                    Glbl.SendToBack()
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper

                    Glbl.SendToBack()
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "panel", CompareMethod.Text) Then
                    Dim Glbl As New Panel
                    Glbl.BackColor = Color.Transparent
                    SrediKontrolu(Glbl, m_node)
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.Text = cls1.SrediSQLPosljeNextForme(SrediSQL2(Glbl.Text))


                    'Glbl.AutoSize = True
                    Glbl.SendToBack()
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper

                    Glbl.SendToBack()
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "webbrowser", CompareMethod.Text) Then
                    Dim Glbl As New WebBrowser
                    Glbl.BackColor = Color.Transparent
                    SrediKontrolu(Glbl, m_node)

                    If Trs2.Rows.Count > 0 Then
                        Glbl.Navigate(Trs2.Rows(0)(Glbl.Name.Trim).ToString)
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Dim strURL As String = ""
                    strURL = m_node.Attributes.GetNamedItem("URL").Value.ToString.ToUpper
                    If strURL <> "" Then
                        Glbl.Navigate(strURL)
                    End If



                    'Glbl.AutoSize = True
                    'Glbl.SendToBack()
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper

                    ' Glbl.SendToBack()
                    'AddHandler Glbl.Click, AddressOf Glbl_Click
                End If



                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "splitcontainer", CompareMethod.Text) Then
                    Dim Glbl As New SplitContainer
                    SrediKontrolu(Glbl, m_node)
                    Glbl.Orientation = m_node.Attributes.GetNamedItem("ORIENTATION").Value.ToString.ToUpper
                    Controls.Add(Glbl)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "check", CompareMethod.Text) Then
                    Dim Glbl As New mojCheck
                    Glbl.Width = 100
                    SrediKontrolu(Glbl, m_node)
                    Glbl.Checked = m_node.Attributes.GetNamedItem("DEFAULT").Value.Trim
                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").Value.Trim




                    If Trs2.Rows.Count > 0 Then
                        Glbl.Checked = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Checked = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.Checked = IIf(r("value").ToString.ToLower.Trim = "checked", True, False)
                                End If
                            Next
                        End If
                    End If


                    Glbl.AutoSize = True
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If






                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "radio", CompareMethod.Text) Then
                    Dim Glbl As New mojRadio
                    Glbl.Width = 200
                    SrediKontrolu(Glbl, m_node)
                    Glbl.Checked = m_node.Attributes.GetNamedItem("DEFAULT").Value.Trim
                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").Value.Trim

                    If Trs2.Rows.Count > 0 Then
                        Glbl.Checked = Trs2.Rows(0)(Glbl.Name.Trim).ToString


                        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                    End If

                    Dim trs76 As String = ""

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Checked = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)


                            trs76 = Trs3.Rows(0)(Glbl.Name.Trim).ToString.ToLower





                        End If
                    End If

                    Controls.Add(Glbl)

                    If trs76 = "1" Or trs76 = "true" Then
                        Glbl.Checked = True
                    End If

                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If



                'If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "grid", CompareMethod.Text) Then
                '    Dim Glbl As New Grid 'DataGridView
                '    Dim Trs As New DataTable
                '    SrediKontrolu(Glbl, m_node)
                '    Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL(m_node.Attributes.GetNamedItem("SQL").InnerText)))
                '    Glbl.DataSource = Trs
                '    Glbl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                '    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").InnerText
                '    Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                '    Glbl.AllowUserToAddRows = False
                '    Glbl.AllowUserToDeleteRows = False
                '    Glbl.EditMode = DataGridViewEditMode.EditProgrammatically
                '    Controls.Add(Glbl)
                '    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                '    'AddHandler Glbl.RowEnter, AddressOf DataGridView1_RowEnter ' cell click nije dobar event
                '    AddHandler Glbl.SelectionChanged, AddressOf DataGridView1_SelectionChanged

                '    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                'End If


                Dim nadjen As Boolean = False

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "dxgrid", CompareMethod.Text) Then
                    nadjen = True
                    Dim Glbl As New GridDX
                    Dim GlblV As New GridDXViewer
                    Dim m_nodeListZaCustomFormat As Xml.XmlNodeList

                    GlblV.GridControl = Glbl

                    Glbl.SQL = cls1.SrediSQLPosljeNextForme(SrediSQL2(m_node.Attributes.GetNamedItem("SQL").InnerText))

                    'Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    Dim Trs As New DataTable
                    Trs = cls1.ExecSql(Glbl.SQL)
                    cls1.NapuniDXGrid(Trs, Glbl, GlblV, "")


                    ' GlblV.FocusedRowHandle = gFocusedRowHandle
                    ' GlblV.TopRowIndex = TopRowIndex

                    m_nodeListZaCustomFormat = m_node.ChildNodes

                    SrediKontrolu(Glbl, m_node)



                    Controls.Add(Glbl)



                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper

                    Glbl.m_nodeList = m_nodeListZaCustomFormat

                    'cls1.NapuniDXGridLATE(Glbl, GlblV, "")
                    'cls1.CustomFormatKolonaDX(GlblV, m_nodeListZaCustomFormat, "", "")

                    Glbl.BringToFront()

                    AddHandler GlblV.FocusedRowChanged, AddressOf DataGridView1_SelectionChanged

                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "grid", CompareMethod.Text) And nadjen = False Then
                    Dim Glbl As New Grid 'DataGridView
                    Dim Trs As New DataTable
                    Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    SrediKontrolu(Glbl, m_node)
                    ' Trs = cls1.ExecSql(SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(m_node.Attributes.GetNamedItem("SQL").InnerText))))
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.strSQL2 = m_node.Attributes.GetNamedItem("SQL2").InnerText.ToUpper
                    Glbl.strSQL3 = m_node.Attributes.GetNamedItem("SQL3").InnerText.ToUpper
                    Glbl.strSQL4 = m_node.Attributes.GetNamedItem("SQL4").InnerText.ToUpper
                    Glbl.strRelacija = m_node.Attributes.GetNamedItem("RELATION").InnerText.ToUpper
                    Dim strOOO As String = ""
                    strOOO = m_node.Attributes.GetNamedItem("SKIPDEFAULTLOAD").InnerText.ToUpper


                    Glbl.zadnjiSQL = cls1.SrediSQLPosljeNextForme(SrediSQL2(m_node.Attributes.GetNamedItem("SQL").InnerText))


                    cls1.NapuniCBLog("ProcitajXML() inicijalno punjenje grida : " & Glbl.Name & " , SQL = " & Glbl.zadnjiSQL)

                    If strOOO = "TRUE" Then

                    Else
                        Trs = cls1.ExecSql(Glbl.zadnjiSQL)
                    End If

                    ' Glbl.DataSource = Trs

                    Glbl.SelectionMode = Janus.Windows.GridEX.SelectionMode.MultipleSelection

                    Glbl.EditColumns = ""
                    Glbl.EditColumns = m_node.Attributes.GetNamedItem("EDITCOLUMNS").InnerText


                    If Glbl.EditColumns.ToLower.StartsWith("sp") Or Glbl.EditColumns.ToLower.StartsWith("select") Then
                        Dim trs88 As DataTable
                        trs88 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(Glbl.EditColumns)))
                        Glbl.EditColumns = trs88(0)(0)
                    End If

                    Glbl.GridovDataTejbl = Trs

                    EditColumns = Glbl.EditColumns
                    Glbl.HideSelection = HideSelection.Highlight

                    Glbl.IncrementalSearchMode = IncrementalSearchMode.AllCharacters


                    'Glbl.RecordNavigator = True
                    Glbl.VisualStyle = VisualStyle.Office2007
                    Glbl.Office2007ColorScheme = Office2007ColorScheme.Black

                    If Glbl.EditColumns.Trim = "" Then
                        Glbl.EnterKeyBehavior = EnterKeyBehavior.None
                    End If

                    Glbl.EnterKeyBehavior = m_node.Attributes.GetNamedItem("ENTERKEYBEHAVIOR").InnerText

                    Glbl.ComboColumns = m_node.Attributes.GetNamedItem("COMBOCOLUMNS").InnerText

                    cls1.CreateGridColumns(Trs, Glbl, EditColumns, Glbl.ComboColumns)
                    cls1.AddConditionalFormatting(Glbl)

                    Glbl.GroupTotals = GroupTotals.Always
                    Glbl.FrozenColumns = m_node.Attributes.GetNamedItem("FROZENCOLUMNS").Value.Trim
                    Glbl.SetDataBinding(Trs, "qwer")

                    Glbl.strAUTOMATICSORT = m_node.Attributes.GetNamedItem("AUTOMATICSORT").InnerText
                    If Glbl.strAUTOMATICSORT.ToLower = "false" Then
                        Glbl.AutomaticSort = False
                    End If

                    Glbl.strMultiGridLayOut = m_node.Attributes.GetNamedItem("MULTIGRIDLAYOUT").Value.Trim

                    ' Glbl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").InnerText
                    Glbl.OnDBLClickSQL = m_node.Attributes.GetNamedItem("ONDBLCLICK").InnerText
                    Glbl.OnEnterKey = m_node.Attributes.GetNamedItem("ONENTERKEY").InnerText
                    Glbl.ONCellEditedstr = m_node.Attributes.GetNamedItem("ONCELLEDITED").InnerText
                    Glbl.BeforeCellEditedstr = m_node.Attributes.GetNamedItem("BEFORECELLEDITED").InnerText
                    Glbl.strCLEARCOLUMS = m_node.Attributes.GetNamedItem("CLEARCOLUMS").Value.Trim()

                    Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    Glbl.GroupByBoxVisible = m_node.Attributes.GetNamedItem("GROUPBYBOXVISIBLE").InnerText

                    Glbl.ColumnHeaders = m_node.Attributes.GetNamedItem("COLUMNHEADERS").InnerText
                    Glbl.strColumnHeaderFormat = m_node.Attributes.GetNamedItem("COLUMNHEADERFORMAT").InnerText



                    'Glbl.ColumnHeaders = InheritableBoolean.False
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.True

                    Glbl.strPopUp = m_node.Attributes.GetNamedItem("POPUP").InnerText
                    Glbl.strPageHeaderRight = m_node.Attributes.GetNamedItem("PAGEHEADERRIGHT").Value.Trim
                    Glbl.strPageHeaderCenter = m_node.Attributes.GetNamedItem("PAGEHEADERCENTER").Value.Trim
                    Glbl.strPageHeaderLeft = m_node.Attributes.GetNamedItem("PAGEHEADERLEFT").Value.Trim
                    Glbl.strPageFooterCenter = m_node.Attributes.GetNamedItem("PAGEFOOTERCENTER").Value.Trim
                    Glbl.strPageFooterLeft = m_node.Attributes.GetNamedItem("PAGEFOOTERLEFT").Value.Trim
                    Glbl.strPageFooterRight = m_node.Attributes.GetNamedItem("PAGEFOOTERRIGHT").Value.Trim
                    Glbl.strPageOrientation = m_node.Attributes.GetNamedItem("PAGEORIENTATION").Value.Trim
                    Glbl.strAfterRefresh = m_node.Attributes.GetNamedItem("AFTERREFRESH").Value.Trim
                    Glbl.FrozenColumns = m_node.Attributes.GetNamedItem("FROZENCOLUMNS").Value.Trim
                    Glbl.strFilterMode = m_node.Attributes.GetNamedItem("FILTERMODE").Value.Trim
                    Glbl.strTranslateColumns = m_node.Attributes.GetNamedItem("TRANSLATECOLUMNS").Value.Trim

                    TempGrid = Glbl
                    LoadLayout()


                    Glbl.EditMode = DataGridViewEditMode.EditProgrammatically
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.FrozenColumns = m_node.Attributes.GetNamedItem("FROZENCOLUMNS").Value.Trim
                    Glbl.BringToFront()

                    Err.Clear()
                    Dim ts2 As String = ""
                    ts2 = m_node.Attributes.GetNamedItem("TOTALROW").InnerText.ToUpper
                    Glbl.TOTALROW2 = ts2
                    If ts2 = "FALSE" Then
                        Glbl.TotalRow = Janus.Data.InheritableBoolean.False
                    Else
                        Glbl.TotalRow = Janus.Data.InheritableBoolean.True
                    End If

                    ts2 = ""
                    ts2 = m_node.Attributes.GetNamedItem("CHECKBOXVISIBLE").InnerText.ToUpper
                    Glbl.CHECKBOXVISIBLE = ts2
                    If ts2 = "FALSE" Then
                        Glbl.RootTable.Columns(1).Visible = False
                    Else
                        Glbl.RootTable.Columns(1).Visible = True
                    End If


                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.BuiltInTexts(Janus.Windows.GridEX.GridEXBuiltInText.GroupByBoxInfo) = ""



                    AddHandler Glbl.MouseUp, AddressOf Glbl_GridMouseUp
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                    AddHandler Glbl.DoubleClick, AddressOf Glbl_DoubleClick
                    'AddHandler Glbl.CellEdited, AddressOf Glbl_CellEdited

                    AddHandler Glbl.CellUpdated, AddressOf Glbl_CellEdited
                    AddHandler Glbl.EditingCell, AddressOf Glbl_EditingCell

                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus
                    AddHandler Glbl.Leave, AddressOf Glbl_Leave
                    'AddHandler Glbl.RowEnter, AddressOf DataGridView1_RowEnter ' cell click nije dobar event
                    AddHandler Glbl.SelectionChanged, AddressOf DataGridView1_SelectionChanged
                    AddHandler Glbl.FormattingRow, AddressOf glblGRID_FormattingRow



                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1

                    Glbl.m_node3 = m_node

                    'Application.DoEvents()

                    ' custom foramat kolona 

                    Dim m_node2 As Xml.XmlNode

                    If Glbl.strCLEARCOLUMS.ToLower = "true" Then
                        For Each Column In Glbl.RootTable.Columns
                            Column.visible = False
                        Next
                    End If

                    Dim iPosition As Integer = 0

                    For Each m_node2 In m_node.ChildNodes
                        Dim tIndex As Integer = 0
                        Dim tName As String = ""
                        Dim tWidth As Integer = 0
                        Dim tCaption As String = ""
                        Dim tmaxLines As Integer = 0
                        Dim tFormat As String = ""
                        Dim tVisible As String = ""
                        Dim tForeColor As String = ""
                        Dim tBackColor As String = ""
                        Dim tAggregateFunction As String = ""
                        Dim tAlign As String = ""
                        Dim tInputMask As String = ""
                        Dim tPosition As String = ""
                        Dim tAutoSize As String = ""

                        iPosition = iPosition + 1

                        tIndex = m_node2.Attributes.GetNamedItem("INDEX").Value.Trim
                        tName = m_node2.Attributes.GetNamedItem("NAME").Value.Trim
                        tWidth = m_node2.Attributes.GetNamedItem("WIDTH").Value.Trim
                        'tCaption = Translate(m_node2.Attributes.GetNamedItem("CAPTION").Value.Trim)
                        tCaption = m_node2.Attributes.GetNamedItem("CAPTION").Value.Trim
                        tFormat = m_node2.Attributes.GetNamedItem("FORMAT").Value.Trim
                        tVisible = m_node2.Attributes.GetNamedItem("VISIBLE").Value.Trim
                        tForeColor = m_node2.Attributes.GetNamedItem("FORECOLOR").Value.Trim
                        tBackColor = m_node2.Attributes.GetNamedItem("BACKCOLOR").Value.Trim
                        tAggregateFunction = m_node2.Attributes.GetNamedItem("AGREGATEFUNCTION").Value.Trim
                        tmaxLines = m_node2.Attributes.GetNamedItem("MAXLINES").Value.Trim

                        tPosition = m_node2.Attributes.GetNamedItem("POSITION").Value.Trim
                        tAutoSize = m_node2.Attributes.GetNamedItem("AUTOSIZE").Value.Trim

                        tAlign = m_node2.Attributes.GetNamedItem("ALIGN").Value.Trim
                        tInputMask = m_node2.Attributes.GetNamedItem("INPUTMASK").Value.Trim

                        If tPosition <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = tPosition

                        If tAutoSize.ToLower = "true" Then
                            Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSizeMode = ColumnAutoSizeMode.AllCellsAndHeader
                            Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSize()
                        End If

                        If tmaxLines > 0 Then
                            Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).MaxLines = tmaxLines
                            Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).wordwrap = True
                        End If

                        If tIndex > 0 Or tName <> "" Then
                            If tWidth > 0 Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).width = tWidth / FakPretvorbe

                            If Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).width < 30 Then
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).HeaderImageKey = ""
                            End If

                            If tCaption <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).caption = tCaption
                            If tFormat <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).FormatString = tFormat
                            If tVisible <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).visible = tVisible

                            If tInputMask <> "" Then Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).InputMask = tInputMask

                            If tAlign <> "" Then
                                If tAlign.ToUpper = "RIGHT" Then
                                    Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 3
                                End If

                                If tAlign.ToUpper = "CENTER" Then
                                    Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 2
                                End If

                                If tAlign.ToUpper = "LEFT" Then
                                    Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 1
                                End If

                            End If




                            If tAggregateFunction <> "" Then
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AggregateFunction = tAggregateFunction
                            End If


                            If Glbl.strCLEARCOLUMS.ToLower = "true" Then
                                iPosition = iPosition + 1
                                Glbl.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = iPosition
                            End If

                            ' Glbl.RootTable.Columns(1).AggregateFunction =        

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
                                End Select
                            End If


                        End If

                    Next
                    '--------------------------------- end custom foramat

                    ' PrevediSadrzajGrida(Glbl) ' to odavde ne radi wtf 




                    '------------------------------------------------ defoltno grupiranje ako ime polja počinje sa !
                    If strKoloneZaGrupiranje.Trim <> "" Then
                        Dim ar4() As String
                        Dim brojkolone As Integer
                        ar4 = strKoloneZaGrupiranje.Split("~")
                        Glbl.RootTable.Groups.Clear()
                        For j = 0 To ar4.GetUpperBound(0)
                            If ar4(j).Trim <> "" Then
                                Dim group As New Janus.Windows.GridEX.GridEXGroup
                                brojkolone = ar4(j) + 2
                                group.Column = Glbl.RootTable.Columns(brojkolone)
                                Glbl.RootTable.Groups.Add(group)
                            End If
                        Next
                    End If
                    '-----------------------------------------------------------------------------------------------


                    Glbl.CurrentColumn = Glbl.RootTable.Columns(m_node.Attributes.GetNamedItem("DEFAULTACTIVECOLUMN").InnerText)
                    Glbl.Row = 1
                    Glbl.Col = Glbl.CurrentColumn.Index
                    Glbl.SelectCurrentCellText()

                    If Glbl.strFilterMode.ToLower.Trim = "true" Then

                        Dim ii As Integer
                        For ii = 1 To Glbl.RootTable.Columns.Count - 1
                            Glbl.RootTable.Columns(ii).EditType = EditType.TextBox
                        Next

                        Glbl.FilterMode = FilterMode.Automatic
                        'Glbl.EditMode = EditMode.EditOn
                        If Glbl.EditColumns = "" Then Glbl.AllowEdit = InheritableBoolean.False
                    End If

                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "text", CompareMethod.Text) Then
                    Dim Glbl As New TeksT
                    Glbl.Width = 300

                    'SrediKontrolu(Glbl, m_node)

                    Dim tParent As String = ""
                    Dim tAnchor As String = ""
                    tParent = m_node.Attributes.GetNamedItem("PARENT").Value.ToString()
                    tAnchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString()
                    SrediKontrolu(Glbl, m_node, tParent, tAnchor)

                    Glbl.Text = ""
                    Glbl.Text = m_node.Attributes.GetNamedItem("DEFAULT").Value.Trim
                    Glbl.EnterASTab = m_node.Attributes.GetNamedItem("ENTERASTAB").Value.Trim





                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        If Glbl.Text.Trim.ToLower = "null" Then Glbl.Text = ""
                        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.Text = r("value")
                                    If Glbl.Text.Trim.ToLower = "null" Then Glbl.Text = ""
                                End If
                            Next
                        End If
                    End If

                    ' Glbl.Text = Translate(Glbl.Text)

                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value
                    Glbl.OnChangeSQL = m_node.Attributes.GetNamedItem("ONCHANGE").Value
                    Glbl.Multiline = m_node.Attributes.GetNamedItem("MULTILINE").Value
                    Glbl.PasswordChar = m_node.Attributes.GetNamedItem("PASSWORDCHAR").Value
                    Glbl.ScrollBars = m_node.Attributes.GetNamedItem("SCROLLBARS").Value
                    Glbl.WordWrap = m_node.Attributes.GetNamedItem("WORDWRAP").Value
                    Glbl.MaxLength = m_node.Attributes.GetNamedItem("MAXLENGTH").Value
                    Glbl.strGotFocus = m_node.Attributes.GetNamedItem("ONGOTFOCUS").Value
                    If Glbl.Text.Trim.ToLower = "null" Then Glbl.Text = ""
                    'Glbl.MaxLength = 10

                    Glbl.strNFCSQL = m_node.Attributes.GetNamedItem("NFCSQL").Value


                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    AddHandler Glbl.Click, AddressOf Glbl_Click
                    AddHandler Glbl.Validated, AddressOf GlblNumber_Validated
                    AddHandler Glbl.TextChanged, AddressOf Glbl_TextChanged
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus2
                    AddHandler Glbl.Leave, AddressOf Glbl_Leave
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "number", CompareMethod.Text) Or InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "cur", CompareMethod.Text) Then
                    Dim Glbl As New NumerBox
                    Glbl.Width = 100
                    'SrediKontrolu(Glbl, m_node)



                    Dim tParent As String = ""
                    Dim tAnchor As String = ""
                    tParent = m_node.Attributes.GetNamedItem("PARENT").Value.ToString()
                    tAnchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString()
                    SrediKontrolu(Glbl, m_node, tParent, tAnchor)

                    Glbl.Text = 0
                    Glbl.FormatMask = m_node.Attributes.GetNamedItem("FORMATMASK").Value
                    Glbl.DecimalDigits = m_node.Attributes.GetNamedItem("DECIMALS").Value
                    Glbl.Text = m_node.Attributes.GetNamedItem("DEFAULT").Value
                    Glbl.Text = m_node.Attributes.GetNamedItem("VALUE").Value
                    Glbl.MaxLength = m_node.Attributes.GetNamedItem("MAXLENGTH").Value
                    ' Glbl.MaximumSize = m_node.Attributes.GetNamedItem("MAXIMUMSIZE").Value
                    ' Glbl.MinimumSize = m_node.Attributes.GetNamedItem("MINIMUMSIZE").Value





                    Glbl.RightToLeft = Windows.Forms.RightToLeft.Yes
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        'DodatnoPostimajKontrolu(Glbl, Trs2.Rows(0)(Glbl.Name).ToString)
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If


                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.Text = r("value")
                                End If
                            Next
                        End If
                    End If

                    Glbl.OnLostFocusSQL = ""
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value


                    Glbl.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper

                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.Validated, AddressOf GlblNumber_Validated
                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus2
                    AddHandler Glbl.Leave, AddressOf Glbl_Leave
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "trazilica", CompareMethod.Text) Or InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "search", CompareMethod.Text) Then
                    Dim Glbl As New SearchBox
                    Glbl.Width = 300
                    SrediKontrolu(Glbl, m_node)
                    Glbl.Text = ""
                    Glbl.Tag = "0"
                    Dim strScaption As String = ""
                    strScaption = m_node.Attributes.GetNamedItem("CAPTION").Value

                    If strScaption.StartsWith("_") Then
                        Glbl.SudjelujeUParametrima = False
                        strScaption = strScaption.Replace("_", "")
                    Else
                        Glbl.SudjelujeUParametrima = True
                    End If

                    ToolTip1.SetToolTip(Glbl, strScaption & "#" & m_node.Attributes.GetNamedItem("SQL").Value)
                    Glbl.OnLostFocusSQL = ""
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value
                    Glbl.strOnLeave = m_node.Attributes.GetNamedItem("ONLEAVE").Value
                    Glbl.strColumnCaption = m_node.Attributes.GetNamedItem("COLUMNCAPTION").Value
                    Glbl.strColumnWidth = m_node.Attributes.GetNamedItem("COLUMNWIDTH").Value
                    Glbl.strColumnAutoResize = m_node.Attributes.GetNamedItem("COLUMNAUTORESIZE").Value




                    If Trs2.Rows.Count > 0 Then
                        Glbl.Tag = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        'Glbl.Text = Trs2.Rows(0)(ToolTip1.GetToolTip(Glbl).Substring(0, ToolTip1.GetToolTip(Glbl).IndexOf("#"))).ToString.Trim
                        Glbl.Text = Trs2.Rows(0)(ToolTip1.GetToolTip(Glbl).Substring(0, ToolTip1.GetToolTip(Glbl).IndexOf("#")).Replace("*", "")).ToString.Trim
                    End If




                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If


                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.Tag = r("value")
                                End If
                            Next

                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower & "caption" Then
                                    Glbl.Text = r("value")
                                End If
                            Next

                        End If
                    End If

                    If Glbl.Text = "null" Then Glbl.Text = ""
                    If Glbl.Tag = "null" Then Glbl.Tag = "0"

                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()

                    'AddHandler Glbl.Validated, AddressOf Glbl_Validated2

                    AddHandler Glbl.Click, AddressOf Glbl_Click
                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus
                    AddHandler Glbl.Leave, AddressOf Glbl_Leave
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                'If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "dtpicker", CompareMethod.Text) Or InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "date", CompareMethod.Text) Then
                '    Dim Glbl As New DateTimePicker
                '    SrediKontrolu(Glbl, m_node)
                '    Glbl.ShowCheckBox = m_node.Attributes.GetNamedItem("SHOWCHECKBOX").Value
                '    If Trs2.Rows.Count > 0 Then
                '        Glbl.Value = Trs2.Rows(0)(Glbl.Name).ToString
                '    End If
                '    Controls.Add(Glbl)
                '    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                '    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                'End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "dtpicker", CompareMethod.Text) Or InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "date", CompareMethod.Text) Then
                    Dim Glbl As New DTPicker
                    Glbl.Width = 100
                    'Glbl.sh()
                    'Displaying week numbers in a datetimepicker control’s dropdown part
                    'http://dotnet.mvps.org/dotnet/faqs/?id=datetimepickerweeknumbers&lang=en
                    'SrediKontrolu(Glbl, m_node)

                    Dim tParent As String = ""
                    Dim tAnchor As String = ""
                    tParent = m_node.Attributes.GetNamedItem("PARENT").Value.ToString()
                    tAnchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString()
                    SrediKontrolu(Glbl, m_node, tParent, tAnchor)

                    Glbl.Format = DateTimePickerFormat.Short

                    Glbl.Format = m_node.Attributes.GetNamedItem("FORMAT").Value
                    Glbl.CustomFormat = m_node.Attributes.GetNamedItem("CUSTOMFORMAT").Value

                    Dim TempDefault As String = ""
                    TempDefault = m_node.Attributes.GetNamedItem("DEFAULT").Value
                    TempDefault = TempDefault.ToLower
                    If TempDefault = "firstdayofyear" Then
                        TempDefault = "01.01." & Year(Today)
                    End If
                    If TempDefault = "firstdayoflastyear" Then
                        TempDefault = "01.01." & (Year(Today) - 1)
                    End If
                    If TempDefault = "firstdayofmonth" Then
                        TempDefault = "01." & (Month(Today)) & "." & Year(Today)
                    End If
                    If TempDefault = "firstdayoflastmonth" Then
                        TempDefault = "01." & (Month(Today) - 1) & "." & Year(Today)
                    End If

                    Glbl.Value = TempDefault

                    'Glbl.Tag = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    Glbl.Tag = m_node.Attributes.GetNamedItem("CAPTION").Value
                    Glbl.strCaption = m_node.Attributes.GetNamedItem("CAPTION").Value
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value
                    Glbl.OnValueChangedSQL = m_node.Attributes.GetNamedItem("ONVALUECHANGED").Value
                    Glbl.Checked = True
                    Glbl.ShowCheckBox = m_node.Attributes.GetNamedItem("SHOWCHECKBOX").Value
                    If Glbl.ShowCheckBox = True Then
                        Glbl.Checked = False
                    End If



                    'If Trs2.Rows.Count > 0 Then
                    '    Err.Clear()
                    '    Glbl.Value = Trs2.Rows(0)(Glbl.Name).ToString
                    '    If Err.Number <> 0 Then
                    '        Glbl.Value = Trs2.Rows(0)(Glbl.Name).ToString.Substring(0, 4) & "-" & Trs2.Rows(0)(Glbl.Name).ToString.Substring(4, 2) & "-" & Trs2.Rows(0)(Glbl.Name).ToString.Substring(6, 2)
                    '    End If
                    'End If

                    Glbl.Checked = m_node.Attributes.GetNamedItem("CHECKED").Value







                    If Trs2.Rows.Count > 0 Then
                        Err.Clear()
                        Glbl.Value = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                        If Err.Number <> 0 Then
                            Err.Clear()
                            Glbl.Value = Trs2.Rows(0)(Glbl.Name.Trim).ToString.Substring(0, 4) & "-" & Trs2.Rows(0)(Glbl.Name.Trim).ToString.Substring(4, 2) & "-" & Trs2.Rows(0)(Glbl.Name.Trim).ToString.Substring(6, 2)
                            If Err.Number = 0 Then
                                If Glbl.ShowCheckBox = True Then
                                    Glbl.Checked = True
                                End If
                            End If
                        Else
                            If Glbl.ShowCheckBox = True Then
                                Glbl.Checked = True
                            End If
                        End If
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            Err.Clear()
                            Glbl.Value = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            If Err.Number <> 0 Then
                                Err.Clear()
                                Glbl.Value = Trs3.Rows(0)(Glbl.Name.Trim).ToString.Substring(0, 4) & "-" & Trs3.Rows(0)(Glbl.Name.Trim).ToString.Substring(4, 2) & "-" & Trs3.Rows(0)(Glbl.Name.Trim).ToString.Substring(6, 2)
                                If Err.Number = 0 Then
                                    If Glbl.ShowCheckBox = True Then
                                        Glbl.Checked = True
                                    End If
                                End If
                            Else
                                If Glbl.ShowCheckBox = True Then
                                    Glbl.Checked = True
                                End If
                            End If
                        End If
                    End If


                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.Value = r("value")
                                End If
                            Next
                        End If
                    End If

                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    'AddHandler Glbl.Click, AddressOf Glbl_ClickButton
                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus2
                    AddHandler Glbl.Leave, AddressOf Glbl_Leave
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.Validated, AddressOf GlblNumber_Validated
                    AddHandler Glbl.ValueChanged, AddressOf Glbl_DateTime_ValueChanged

                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "combo", CompareMethod.Text) Then
                    Dim Glbl As New Combo
                    Dim Trs As New DataTable
                    Dim Trs1 As New DataTable
                    Glbl.Width = 300
                    'SrediKontrolu(Glbl, m_node)

                    Dim tParent As String = ""
                    Dim tAnchor As String = ""
                    tParent = m_node.Attributes.GetNamedItem("PARENT").Value.ToString()
                    tAnchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString()

                    Glbl.OnLostFocusSQL = ""
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value
                    Glbl.OnClickSQL = ""
                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    Glbl.strCaption = m_node.Attributes.GetNamedItem("CAPTION").Value
                    Glbl.SQL = ""
                    Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.ZaPrevodjenje = m_node.Attributes.GetNamedItem("TRANSLATE").InnerText


                    SrediKontrolu(Glbl, m_node, tParent, tAnchor)

                    'Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(m_node.Attributes.GetNamedItem("SQL").InnerText))
                    Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(m_node.Attributes.GetNamedItem("SQL").InnerText, prevParamizStartupStore))
                    Glbl.Text = ""

                    Dim strAllowEmpty As String = ""
                    strAllowEmpty = m_node.Attributes.GetNamedItem("ALLOWEMPTY").InnerText

                    Glbl.AllowEmpty = False
                    If strAllowEmpty.Trim.ToLower = "true" Then
                        Glbl.AllowEmpty = True
                        If Not Trs Is Nothing Then
                            Dim row As DataRow
                            row = Trs.NewRow()
                            row(0) = "null"
                            'row(1) = "*"
                            row(1) = ""
                            Trs.Rows.InsertAt(row, 0)
                            Trs.AcceptChanges()
                            'Trs.Rows.Add(row)
                        End If
                    End If

                    If Trs Is Nothing Then
                        Glbl.DataSource = Nothing 'Trs1
                    Else
                        If Glbl.ZaPrevodjenje.ToLower.Trim = "true" Then
                            Dim r As DataRow
                            For Each r In Trs.Rows
                                r(1) = Translate(r(1).Trim)
                            Next
                        End If

                        Glbl.DataSource = Trs
                    End If

                    Glbl.DisplayMember = Trs.Columns(1).Caption
                    Glbl.ValueMember = Trs.Columns(0).Caption

                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()

                    Dim VridenostZaNaknadnoPunjenje As String = ""




                    Glbl.SelectedValue = Trs2.Rows(0)(Glbl.Name.Trim) ' ovo ne prolazi prije loada forme
                    VridenostZaNaknadnoPunjenje = Trs2.Rows(0)(Glbl.Name.Trim)

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            Glbl.SelectedValue = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            'VridenostZaNaknadnoPunjenje = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    If Not RSDBClipboard Is Nothing Then
                        If RSDBClipboard.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In RSDBClipboard.Rows
                                If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                    Glbl.SelectedValue = r("value")
                                    VridenostZaNaknadnoPunjenje = r("value")
                                End If
                            Next
                        End If
                    End If

                    'Glbl.Tag = Glbl.Tag & "##" & Trs2.Rows(0)(Glbl.Name.Trim) ' zato pamtim vrijednost u tagu , a kasnije ga u "napunikontrolezaedit" ponovo napunim
                    Glbl.Tag = Glbl.Tag & "##" & VridenostZaNaknadnoPunjenje ' zato pamtim vrijednost u tagu , a kasnije ga u "napunikontrolezaedit" ponovo napunim
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus2
                    AddHandler Glbl.Validated, AddressOf Glbl_ValidatedCombo
                    AddHandler Glbl.KeyUp, AddressOf AutoCompleteCombo_KeyUp
                    AddHandler Glbl.Leave, AddressOf AutoCompleteCombo_Leave
                    AddHandler Glbl.SelectedValueChanged, AddressOf Glbl_ComboClick
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "multidrop", CompareMethod.Text) Then
                    Dim Glbl As New DevExpress.XtraEditors.CheckedComboBoxEdit
                    Dim Trs As New DataTable
                    Dim Trs1 As New DataTable
                    Glbl.Width = 300
                    'SrediKontrolu(Glbl, m_node)    

                    Dim tParent As String = ""
                    Dim tAnchor As String = ""
                    tParent = m_node.Attributes.GetNamedItem("PARENT").Value.ToString()
                    tAnchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString()

                    'Glbl.Hide()
                    SrediKontrolu(Glbl, m_node, tParent, tAnchor)
                    'Glbl.Hide()

                    'Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(m_node.Attributes.GetNamedItem("SQL").InnerText))
                    strMultiDropSQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(m_node.Attributes.GetNamedItem("SQL").InnerText, prevParamizStartupStore))
                    Glbl.Text = ""

                    Dim strAllowEmpty As String = ""
                    strAllowEmpty = m_node.Attributes.GetNamedItem("ALLOWEMPTY").InnerText

                    Glbl.Properties.DataSource = Trs
                    Glbl.Properties.DisplayMember = Trs.Columns(1).Caption
                    Glbl.Properties.ValueMember = Trs.Columns(0).Caption

                    Glbl.ErrorText = ""
                    Glbl.ErrorText = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value



                    Controls.Add(Glbl)
                    'Glbl.Hide()
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()

                    Dim tstr6 As String = ""
                    tstr6 = Trs2.Rows(0)(Glbl.Name.Trim)
                    Glbl.Tag = tstr6

                    If tstr6 = "" Then
                        If Not RSDBClipboard Is Nothing Then
                            If RSDBClipboard.Rows.Count > 0 Then
                                Dim r As DataRow
                                For Each r In RSDBClipboard.Rows
                                    If r("key").ToString.Trim.ToLower = xmlname5 & "/" & Glbl.Name.Trim.ToLower Then
                                        Glbl.Tag = r("value")
                                    End If
                                Next
                            End If
                        End If
                    End If

                    'MsgBox("bbb" & Glbl.Tag)

                    'If tstr6.Trim <> "" Then
                    '    tstr6 = "," & tstr6.Replace("~", ",") & ","
                    '    For i8 = 0 To Glbl.Properties.Items.Count
                    '        If tstr6.IndexOf("," & Glbl.Properties.Items(i8).Value.ToString.Trim & ",") > 0 Then
                    '            Glbl.Properties.Items(i8).CheckState = CheckState.Checked
                    '        End If
                    '    Next
                    'End If

                    ' Glbl.Show()

                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1

                    AddHandler Glbl.Validated, AddressOf Glbl_ValidatedMultiDrop
                    AddHandler Glbl.Popup, AddressOf Glbl_popup
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown

                End If




                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "tree", CompareMethod.Text) Then
                    Dim Glbl As New treeViewMoj
                    Dim Trs As New DataTable
                    Dim Trs1 As New DataTable
                    Dim tSql As String
                    Glbl.Width = 300
                    SrediKontrolu(Glbl, m_node)
                    tSql = cls1.SrediSQLPosljeNextForme(m_node.Attributes.GetNamedItem("SQL").InnerText, prevParamizStartupStore)
                    Glbl.SQL = tSql
                    Glbl.Text = ""
                    Dim strAllowEmpty As String = ""
                    strAllowEmpty = m_node.Attributes.GetNamedItem("ALLOWEMPTY").InnerText

                    Dim a() As String

                    a = tSql.Split("#")

                    For j = 0 To a.GetUpperBound(0)
                        If a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower = "yearmonth" Then

                        ElseIf a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower.Contains("tree") Then
                            DazovaIzStore(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")), a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1), Glbl)
                        Else
                            Dim YearNode As TreeNode = New TreeNode(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")))
                            YearNode.Tag = ":" + a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                            Glbl.Nodes.Add(YearNode)
                        End If
                    Next

                    Glbl.strPopUp = m_node.Attributes.GetNamedItem("POPUP").InnerText
                    Glbl.OnLostFocusSQL = ""
                    Glbl.OnLostFocusSQL = m_node.Attributes.GetNamedItem("ONLOSTFOCUS").Value
                    Glbl.OnClickSQL = ""
                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    Glbl.SQL = ""
                    Glbl.SQL = m_node.Attributes.GetNamedItem("SQL").InnerText
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.BringToFront()
                    Glbl.ExpandAll()

                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    AddHandler Glbl.BeforeSelect, AddressOf Glbl_TREE_BeforeSelect

                    'AddHandler Glbl.GotFocus, AddressOf Glbl_Gotfocus2
                    AddHandler Glbl.MouseUp, AddressOf Glbl_TreeMouseUp
                    ' AddHandler Glbl.Validated, AddressOf Glbl_ValidatedCombo
                    AddHandler Glbl.AfterSelect, AddressOf Glbl_Click
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1

                End If


                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "button", CompareMethod.Text) Then
                    Dim Glbl As New Buton
                    Glbl.Width = 100

                    Glbl.OnClickSQL = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    SrediKontrolu(Glbl, m_node)
                    'Glbl.Text = ""
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.ImageList = ImageList1

                    Glbl.Tag = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.HotKey = Trs2.Rows(0)("hotkey" + Glbl.Name.Trim).ToString ' ako se polje zove hotkeyimekontrole onda to definira hotkey

                    Glbl.Text = Translate(Glbl.Text)

                    If Glbl.Text.ToLower.Contains("stavke") Or Glbl.Text.ToLower.Contains("dalje") Or Glbl.Text.ToLower.Contains(">>") Or Glbl.Text.ToLower.Contains("prikaži") Or Glbl.Text.ToLower.Contains("prikaz") Or Glbl.Text.ToLower.Contains("pokaži") Then Glbl.ImageIndex = 19
                    If Glbl.Text.ToLower.Contains("spremi") Then Glbl.ImageIndex = 9
                    If Glbl.Text.ToLower.Contains("ok") Or Glbl.Text.ToLower.Contains("prihvati") Or Glbl.Text.ToLower.Contains("uredu") Or Glbl.Text.ToLower.Contains("u redu") Then Glbl.ImageIndex = 15
                    If Glbl.Text.ToLower.Contains("odustani") Or Glbl.Text.ToLower.Contains("cancel") Or Glbl.Text.ToLower.Contains("gotovo") Or Glbl.Text.ToLower.Contains("izlaz") Then Glbl.ImageIndex = 27
                    If Glbl.Text.ToLower.Contains("ispis") Or Glbl.Text.ToLower.Contains("ispiši") Or Glbl.Text.ToLower.Contains("štamp") Or Glbl.Text.ToLower.Contains("print") Then Glbl.ImageIndex = 20
                    If Glbl.Text.ToLower.Contains("donos") Or Glbl.Text.ToLower.Contains("donesi") Or Glbl.Text.ToLower.Contains("import") Then Glbl.ImageIndex = 31
                    If Glbl.Text.ToLower.Contains("dodaj") Or Glbl.Text.ToLower.Contains("add") Or Glbl.Text.ToLower.Contains("unesi") Then Glbl.ImageIndex = 18
                    If Glbl.Text.ToLower.Contains("obriši") Or Glbl.Text.ToLower.Contains("delete") Or Glbl.Text.ToLower.Contains("brisanje") Then Glbl.ImageIndex = 17
                    If Glbl.Text.ToLower.Contains("izmjeni") Or Glbl.Text.ToLower.Contains("edit") Or Glbl.Text.ToLower.Contains("ispravi") Then Glbl.ImageIndex = 33
                    If Glbl.Text.ToLower.Contains("zaključi") Or Glbl.Text.ToLower.Contains("lock") Then Glbl.ImageIndex = 26
                    If Glbl.Text.ToLower.Contains("gore") Or Glbl.Text.ToLower.Contains("up") Then Glbl.ImageIndex = 2
                    If Glbl.Text.ToLower.Contains("dole") Or Glbl.Text.ToLower.Contains("down") Then Glbl.ImageIndex = 1
                    If Glbl.Text.ToLower.Contains("pomoć") Or Glbl.Text.ToLower.Contains("help") Or Glbl.Text.ToLower.Contains("uputi") Then Glbl.ImageIndex = 14

                    Glbl.ImageIndex = m_node.Attributes.GetNamedItem("IMAGEINDEX").Value

                    If Glbl.ImageIndex > 0 Then
                        Glbl.ImageAlign = ContentAlignment.MiddleLeft
                        Glbl.TextAlign = ContentAlignment.MiddleRight
                    End If

                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper

                    Glbl.BringToFront()
                    AddHandler Glbl.Click, AddressOf Glbl_ClickButton
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "help", CompareMethod.Text) Then
                    Dim Glbl As New Buton
                    Glbl.Width = 100

                    SrediKontrolu(Glbl, m_node)
                    'Glbl.Text = ""
                    If Trs2.Rows.Count > 0 Then
                        Glbl.Text = Trs2.Rows(0)(Glbl.Name.Trim).ToString
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            'Glbl.Text = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                            DodatnoPostimajKontrolu(Glbl, Trs3.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    Glbl.Tag = "help"
                    Glbl.HotKey = m_node.Attributes.GetNamedItem("HOTKEY").Value.ToString.ToUpper
                    Glbl.HotKey = Trs2.Rows(0)("hotkey" + Glbl.Name.Trim).ToString ' ako se polje zove hotkeyimekontrole onda to definira hotkey

                    Glbl.Text = Translate(Glbl.Text)

                    Controls.Add(Glbl)
                    Glbl.Tag = "help"
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper

                    Glbl.BringToFront()
                    AddHandler Glbl.Click, AddressOf Glbl_ClickButton
                    AddHandler Glbl.KeyDown, AddressOf Glbl_KeyDown
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If

                If InStr(m_node.Attributes.GetNamedItem("TYPE").Value.ToString.ToLower, "picturebox", CompareMethod.Text) Then
                    Dim Glbl As New PictureBox
                    SrediKontrolu(Glbl, m_node)
                    Glbl.ImageLocation = m_node.Attributes.GetNamedItem("LOCATION").Value.ToString.ToUpper

                    If Trs2.Rows.Count > 0 Then
                        Glbl.ImageLocation = Trs2.Rows(0)(Glbl.Name.Trim).ToString

                        If Trs2.Rows(0)(Glbl.Name.Trim).ToString.StartsWith("http") Then
                            Glbl.Load(Trs2.Rows(0)(Glbl.Name.Trim).ToString)
                        End If
                    End If

                    If strStartupStora2 <> "" Then
                        If Trs3.Rows.Count > 0 Then
                            Glbl.ImageLocation = Trs3.Rows(0)(Glbl.Name.Trim).ToString
                        End If
                    End If

                    Glbl.SizeMode = PictureBoxSizeMode.StretchImage
                    Glbl.Dock = m_node.Attributes.GetNamedItem("DOCK").Value.ToString.ToUpper
                    Glbl.Anchor = m_node.Attributes.GetNamedItem("ANCHOR").Value.ToString.ToUpper
                    Glbl.Tag = m_node.Attributes.GetNamedItem("ONCLICK").Value
                    Controls.Add(Glbl)
                    Glbl.Parent = Controls(m_node.Attributes.GetNamedItem("PARENT").Value.ToString.ToUpper)
                    Glbl.SendToBack()

                    AddHandler Glbl.Click, AddressOf Glbl_ClickButton
                    If Glbl.Enabled And Glbl.Visible Then TabIndexCounter = TabIndexCounter + 1
                End If


            Next



            'If StatusStrip1.Visiusble Then
            ToolStripStatusLabel1.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel1.Tag.ToString), strPrevParametri))
            ToolStripStatusLabel2.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel2.Tag.ToString), strPrevParametri))
            ToolStripStatusLabel3.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel3.Tag.ToString), strPrevParametri))
            ' End If


            If Trim(Button1.Text) = "" And Trim(Button2.Text) = "" And Trim(Button3.Text) = "" And Trim(Button4.Text) = "" And Trim(Button5.Text) = "" And Trim(Button6.Text) = "" Then
                Panel1.Visible = False
            Else
                Panel1.Visible = True
            End If


            If Trim(Button1.Text) = "" Then Button1.Visible = False Else Button1.Visible = True
            If Trim(Button2.Text) = "" Then Button2.Visible = False Else Button2.Visible = True
            If Trim(Button3.Text) = "" Then Button3.Visible = False Else Button3.Visible = True
            If Trim(Button4.Text) = "" Then Button4.Visible = False Else Button4.Visible = True
            If Trim(Button5.Text) = "" Then Button5.Visible = False Else Button5.Visible = True
            If Trim(Button6.Text) = "" Then Button6.Visible = False Else Button6.Visible = True




            Button2.Left = Button1.Left + Button1.Width + 8
            Button3.Left = Button2.Left + Button2.Width + 8
            Button4.Left = Button3.Left + Button3.Width + 8
            Button5.Left = Button4.Left + Button4.Width + 8
            Button6.Left = Button5.Left + Button5.Width + 8

            cls1.UgasiProgres()
            Windows.Forms.Cursor.Current = Cursors.Default
            Button1.TabIndex = TabIndexCounter + 1
            Button2.TabIndex = TabIndexCounter + 2
            Button3.TabIndex = TabIndexCounter + 3
            Button4.TabIndex = TabIndexCounter + 4
            Button5.TabIndex = TabIndexCounter + 5
            Button6.TabIndex = TabIndexCounter + 6
            TabIndexCounter = 0


        'On Error Resume Next
        If StrWINDOWSTATE <> "2" Then
                If Me.MaximizeBox = True Then
                    Me.Height = GetSetting("Jupiter", "FormHeight", strXML & "-" & strImeForme & "-" & Version1 & "-" & Version2)
                    Me.Width = GetSetting("Jupiter", "FormWidth", strXML & "-" & strImeForme & "-" & Version1 & "-" & Version2)
                End If
            End If

            If strAktivnaKontrolanakonLodanja <> "" Then
                AktivnaKontola = Me.Controls(strAktivnaKontrolanakonLodanja)
                Timer1.Enabled = True
            End If


            If BrojKontrola = 0 Then
                Rafalno(strONLOADED, Nothing, Nothing) '**
            Else

                If strPermisio.ToString.ToLower = "true" Then
                    ssPermissionProcitaj()
                End If

            End If



            ' zbog 125 DPI errora , kada labele prelaze preko kontrola
            For Each ctl As Control In Controls
                If TypeOf ctl Is CheckBox Or TypeOf ctl Is mojCheck Or TypeOf ctl Is mojRadio Or TypeOf ctl Is Combo Or TypeOf ctl Is DTPicker Or TypeOf ctl Is NumerBox Or TypeOf ctl Is SearchBox Or TypeOf ctl Is TextBox Or TypeOf ctl Is Grid Then
                    ctl.BringToFront()
                End If
            Next


            Timer3.Enabled = True


    End Sub



    Sub DazovaIzStore(ByVal Stora As String, ByVal ImeForme As String, ByVal tCtl As treeViewMoj)

        On Error Resume Next

        Dim ds As New System.Data.DataSet()
        Dim adapter As New System.Data.SqlClient.SqlDataAdapter()
        Stora = cls1.SrediSQLPosljeNextForme(SrediSQL2(Stora))
        Err.Clear()
        adapter.SelectCommand = New System.Data.SqlClient.SqlCommand(Stora.ToString, Conn)
        adapter.Fill(ds, "NTGNTEST")
        ds.Relations.Clear()
        ds.Relations.Add(TREE_RELATIONSHIP, ds.Tables(0).Columns(0), ds.Tables(0).Columns(1))

        If Err.Number = 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                Dim rootNode As New TreeNode(ds.Tables(0).Rows(0)(2).ToString)
                rootNode.Tag = "0:" & ImeForme
                rootNode.Tag = ds.Tables(0).Rows(0)(3) ' & ":" & ImeForme
                AddNodes(ds.Tables(0).Rows(0), rootNode, ImeForme)
                tCtl.Nodes.Add(rootNode)
            End If
        Else
            ' frm5.TextBox1.Text = frm5.TextBox1.Text & TimeOfDay & " TREE PROBLEM : " & Err.Description & vbCrLf
            cls1.NapuniCBLog(" TREE PROBLEM : " & Err.Description)
        End If

        'Me.TreeView2.ExpandAll()

    End Sub


    Private Sub AddNodes(ByRef ParentRow As DataRow, ByRef ParentNode As TreeNode, ByVal ImeForme As String)
        Dim children() As DataRow = ParentRow.GetChildRows(TREE_RELATIONSHIP)
        If children.Length <> 0 Then
            For Each dr As DataRow In children
                ' If CType(dr(0), Integer) <> 0 Then
                If dr(0).ToString.Trim <> "0" Then
                    Dim newNode As TreeNode
                    newNode = ParentNode.Nodes.Add(dr(0).ToString, dr(2).ToString)
                    newNode.Tag = dr(3).ToString.Trim ' + ":" + ImeForme
                    'Dim newNode As New TreeNode(dr(2).ToString)
                    'newNode.Tag = dr(3).ToString.Trim + ":" + ImeForme
                    'ParentNode.Nodes.Add(newNode)
                    AddNodes(dr, newNode, ImeForme)
                End If
            Next
        End If
    End Sub



    Function SrediSQL2(ByVal sql As String) As String

        On Error Resume Next

        sql = sql.Replace("@@ID", cls1.MasterID).Replace("@@id", cls1.MasterID).Replace("@@operateriid", cls1.OperateriID).Replace("@@OPERATERIID", cls1.OperateriID).Replace("@@DETID", cls1.DetIDIzKlase).Replace("@@detid", cls1.DetIDIzKlase).Replace("@@DetId", cls1.DetIDIzKlase).Replace("@@node", Node).Replace("@@NODE", Node).Replace("@@tempstring", cls1.strTemp).Replace("@@TEMPSTRING", cls1.strTemp)

        sql = sql.Replace("@@editedcolumn", TrenutnaKolona.Caption).Replace("@@EDITEDCOLUMN", TrenutnaKolona.Caption)
        sql = sql.Replace("@@lastvalue", TrenutnaVrijednost).Replace("@@LASTVALUE", TrenutnaVrijednost)

        sql = sql.Replace("@@dblclickedcolumn", DblClickedColumn.Caption).Replace("@@DBLCLICKEDCOLUMN", DblClickedColumn.Caption)

        sql = sql.Replace("@@VERSION", "NetGen v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision)

        sql = sql.Replace("@@OPISNIPARAMETRI", strOpisniParametri)

        sql = sql.Replace("@@FOCUSEDCONTROL", AktivnaKontola.Name)

        sql = sql.Replace("@@selectedfile", strSelectedFile)

        sql = sql.Replace("@@selectedpath", strSelectedPath)



        'If sql.ToLower.Contains("@@GRIDROWCOUNT(") Then
        '    Dim param As String
        '    param = sql.ToUpper
        '    param = param.Replace("@@GRIDROWCOUNT(", "")
        '    param = param.Replace(")", "")
        '    sql = sql.Replace("@@OPISNIPARAMETRI", strOpisniParametri)
        'End If


        If sql.ToLower.Contains("@@xmlname") Then
            sql = sql.Replace("@@xmlname", strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\")))
            sql = sql.Replace("@@XMLNAME", strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\")))
        End If

        If sql.ToLower.Contains("@@jupitersrvpath") Then
            sql = sql.Replace("@@jupitersrvpath", MOJremotedllpath)
        End If

        sql = sql.Replace("@@selectedcolor", strIzabranaBoja).Replace("@@SELECTEDCOLOR", strIzabranaBoja)


        SrediSQL2 = sql

    End Function

    Sub SrediKontrolu(ByVal Glbl As Control, ByVal m_node As Xml.XmlNode, Optional ByVal sParent As String = "", Optional ByVal sAnchor As String = "")
        On Error Resume Next
        Dim TempTop As Long = 0
        Dim TempLeft As Long = 0
        Dim TempCaption As String = ""
        TempCaption = m_node.Attributes.GetNamedItem("CAPTION").Value.Trim


        Glbl.Text = m_node.Attributes.GetNamedItem("CAPTION").Value.Trim


        If TypeOf Glbl Is RadioButton Then

        Else
            Glbl.Text = m_node.Attributes.GetNamedItem("DEFAULT").Value.Trim
        End If

        Glbl.Text = Glbl.Text.Replace("_", "")

        Glbl.Text = Glbl.Text.Replace("!!", vbCrLf)

        TempTop = m_node.Attributes.GetNamedItem("TOP").Value / FakPretvorbe
        Glbl.Top = TempTop
        TempLeft = m_node.Attributes.GetNamedItem("LEFT").Value / FakPretvorbe
        Glbl.Left = TempLeft
        Glbl.Width = m_node.Attributes.GetNamedItem("WIDTH").Value / FakPretvorbe
        Glbl.Height = m_node.Attributes.GetNamedItem("HEIGHT").Value / FakPretvorbe
        Glbl.Enabled = m_node.Attributes.GetNamedItem("ENABLED").Value
        Glbl.Visible = m_node.Attributes.GetNamedItem("VISIBLE").Value
        Glbl.Name = m_node.Attributes.GetNamedItem("NAME").Value

        Dim strBFlabel As String = ""
        strBFlabel = Glbl.Text

        If Not TypeOf Glbl Is GroupBox And Not TypeOf Glbl Is Label And Not TypeOf Glbl Is Panel Then
            If TempTop = 0 Or TempCaption.EndsWith("*") Then ' kontrolu treba automatski pozicionirati i nalodati labelu za tu kontrolu
                Dim Glbl2 As New Label
                If TempCaption.EndsWith("*") Then
                    ZadnjiUporabljiviTOP = TempTop
                Else
                    ZadnjiUporabljiviTOP = ZadnjiUporabljiviTOP + 30
                    If ZadnjiUporabljiviTOP = 30 Then ZadnjiUporabljiviTOP = 10
                End If
                If TempLeft = 0 Then TempLeft = 140
                Glbl.Top = ZadnjiUporabljiviTOP
                Glbl.Left = TempLeft
                Glbl2.Top = ZadnjiUporabljiviTOP + 4
                Glbl2.Left = TempLeft - 130
                Glbl2.BackColor = Color.Transparent
                Glbl2.AutoSize = True
                Glbl2.TextAlign = ContentAlignment.TopRight
                Glbl2.Tag = ""
                If TempCaption.EndsWith("*") Then
                    Glbl2.Text = TempCaption.Replace("*", "").Trim

                    Glbl.Refresh()
                    Glbl2.Refresh()
                    Glbl2.Left = TempLeft - (Glbl2.Text.Length * 5) - 15
                    Glbl2.Tag = ""
                    Glbl2.Tag = TempLeft
                Else
                    Glbl2.Text = TempCaption.Trim
                End If

                Glbl2.Text = Glbl2.Text.Replace("_", "")

                Glbl2.Text = Translate(Glbl2.Text)
                Glbl2.Name = "lbl" + Glbl.Name

                strBFlabel = Glbl2.Text

                'If Not TypeOf Glbl Is CheckBox And Not TypeOf Glbl Is Button Then
                '    Controls.Add(Glbl2)
                'End If
                Glbl2.AutoSize = True
                ' Glbl2.Width = 50

                If Not TypeOf Glbl Is CheckBox And Not TypeOf Glbl Is Button Then
                    Controls.Add(Glbl2)

                    'If Not Glbl.Parent Is Nothing Then
                    '    Glbl2.Parent = Glbl.Parent
                    'End If

                    If sParent <> "" Then
                        Glbl2.Parent = Controls(sParent)
                    End If

                    If sAnchor <> "" Then
                        Glbl2.Anchor = sAnchor
                    End If

                End If



                Glbl2.BringToFront()
                AddHandler Glbl2.Click, AddressOf Glbl_Click
            Else
                ZadnjiUporabljiviTOP = TempTop
            End If
        End If

        Dim ttabIndex As Integer = -2
        ttabIndex = m_node.Attributes.GetNamedItem("TABINDEX").Value

        Glbl.TabIndex = ttabIndex
        If ttabIndex < 0 Then
            Glbl.TabStop = False
        Else
            Glbl.TabStop = True
        End If


        If ttabIndex = -2 Then ' zanči da nemam definiran tab index i trebam ga automatski dodjeliti
            CurTabIndex = CurTabIndex + 1
            Glbl.TabIndex = CurTabIndex
            Glbl.TabStop = True
        End If



        If Glbl.Enabled = False Then Glbl.TabStop = False

        Glbl.Name = m_node.Attributes.GetNamedItem("NAME").Value
        ToolTip1.SetToolTip(Glbl, m_node.Attributes.GetNamedItem("CAPTION").Value)
        'ToolTip1.SetToolTip(Glbl, ttabIndex)
        Glbl.Tag = m_node.Attributes.GetNamedItem("CAPTION").Value
        If Glbl.Tag.ToString.Trim = "" Then
            Glbl.Tag = m_node.Attributes.GetNamedItem("NAME").Value
        End If
        'Glbl.TabIndex = m_node.Attributes.GetNamedItem("TABINDEX").Value
        Err.Clear()
        Dim tVal As String = ""
        tVal = m_node.Attributes.GetNamedItem("REFRESHAFTERCHILD").Value

        If tVal = "TRUE" Then
            Dim row As DataRow
            row = Cdt.NewRow()
            row("control") = Glbl.Name.Trim
            row("sql") = m_node.Attributes.GetNamedItem("SQL").InnerText
            row("tip") = Glbl.GetType.Name.Trim
            Cdt.Rows.Add(row)
        End If

        BrojKontrola = BrojKontrola + 1




        ' ----------------------------------- ŠMINKA -------------------
        Dim sFont As String = Glbl.Font.Name
        sFont = m_node.Attributes.GetNamedItem("FONT").Value.ToString
        Dim sSize As Single = Glbl.Font.Size
        sSize = m_node.Attributes.GetNamedItem("SIZE").Value
        Dim sBold As FontStyle = Glbl.Font.Style
        sBold = m_node.Attributes.GetNamedItem("STYLE").Value.ToString
        Glbl.Font = New System.Drawing.Font(sFont, sSize, sBold)

        Dim sForeColor As String = ""
        sForeColor = m_node.Attributes.GetNamedItem("FORECOLOR").Value
        If sForeColor.Trim <> "" Then

            If sForeColor.StartsWith("#") Then
                Glbl.ForeColor = System.Drawing.ColorTranslator.FromHtml(sForeColor)
            Else
                Select Case sForeColor.ToLower
                    Case "green"
                        Glbl.ForeColor = Color.Green
                    Case "white"
                        Glbl.ForeColor = Color.White
                    Case "black"
                        Glbl.ForeColor = Color.Black
                    Case "darksalmon"
                        Glbl.ForeColor = Color.DarkSalmon
                    Case "darkseagreen"
                        Glbl.ForeColor = Color.DarkSeaGreen
                    Case "limegreen"
                        Glbl.ForeColor = Color.LimeGreen
                    Case ("greenyellow")
                        Glbl.ForeColor = Color.GreenYellow
                    Case "yellow"
                        Glbl.ForeColor = Color.Yellow
                    Case "lightyellow"
                        Glbl.ForeColor = Color.LightYellow
                    Case "red"
                        Glbl.ForeColor = Color.Red
                End Select
            End If



        End If


        Dim sBackColor As String = ""
        sBackColor = m_node.Attributes.GetNamedItem("BACKCOLOR").Value
        If sBackColor.Trim <> "" Then

            If sBackColor.StartsWith("#") Then
                Glbl.BackColor = System.Drawing.ColorTranslator.FromHtml(sBackColor)
            Else
                Select Case sBackColor.ToLower
                    Case "green"
                        Glbl.BackColor = Color.Green
                    Case "white"
                        Glbl.BackColor = Color.White
                    Case "black"
                        Glbl.BackColor = Color.Black
                    Case "darksalmon"
                        Glbl.BackColor = Color.DarkSalmon
                    Case "darkseagreen"
                        Glbl.BackColor = Color.DarkSeaGreen
                    Case "limegreen"
                        Glbl.BackColor = Color.LimeGreen
                    Case "greenyellow"
                        Glbl.BackColor = Color.GreenYellow
                    Case "yellow"
                        Glbl.BackColor = Color.Yellow
                    Case "lightyellow"
                        Glbl.BackColor = Color.LightYellow
                    Case "red"
                        Glbl.BackColor = Color.Yellow
                    Case "silver"
                        Glbl.BackColor = Color.Silver
                    Case "lightgray"
                        Glbl.BackColor = Color.LightGray
                End Select
            End If
        End If

        ' -------------------------------- END OF ŠMINKA -------------------

        ' ------------- BF
        On Error Resume Next
        Dim strBF As String = ""

        Dim nadjen As Boolean = False

        If TypeOf Glbl Is SearchBox Then
            nadjen = True
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-group"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<label class=""col-sm-3 control-label"" for=""" & Glbl.Name & """>" & strBFlabel & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""col-sm-9"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<input type=""search"" id=""" & Glbl.Name & """ class=""col-sm-10 form-control searchtextbox perzistonblur enterastab "" name=""" & Glbl.Name & """ data-webserviceaction="""" data-form=""" & strBFformname & """ value="""" />" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr & vbCr
            BF2(strBF)
        End If

        If nadjen = False And TypeOf Glbl Is TextBox Then
            nadjen = True
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-group"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<label class=""col-sm-3 control-label"" for=""" & Glbl.Name & """>" & strBFlabel & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""col-sm-9"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<input type=""text"" id=""" & Glbl.Name & """ class=""col-sm-10 form-control perzistonblur enterastab "" name=""" & Glbl.Name & """ data-form=""" & strBFformname & """ value="""" />" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr & vbCr
            BF2(strBF)
        End If

        If nadjen = False And TypeOf Glbl Is NumerBox Then
            nadjen = True
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-group"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<label class=""col-sm-3 control-label"" for=""" & Glbl.Name & """>" & strBFlabel & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""col-sm-9"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<input type=""number"" id=""" & Glbl.Name & """ class=""col-sm-10 form-control perzistonblur enterastab "" name=""" & Glbl.Name & """ data-form=""" & strBFformname & """ value="""" />" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr & vbCr
            BF2(strBF)
        End If

        If nadjen = False And TypeOf Glbl Is Button Then
            nadjen = True
            Dim Tctl As Buton = Glbl
            strBFbutoni = strBFbutoni & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<button type=""submit"" id=""" & Glbl.Name & """ class=""btn btn-primary perzistonclick enterastab"" data-sql=""" & Tctl.OnClickSQL & """ data-form=""" & strBFformname & """  >" & strBFlabel & "</button>" & vbCr
        End If

        If nadjen = False And TypeOf Glbl Is Combo Then
            Dim Tctl As Combo = Glbl
            nadjen = True
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-group"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<label class=""col-sm-3 control-label"" for=""" & Glbl.Name & """>" & strBFlabel & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""col-sm-9"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<select id=""" & Glbl.Name & """ class=""form-control combobox perzistonchange enterastab""  data-webserviceaction="""" data-form=""" & strBFformname & """ >" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<option>1</option>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</select>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr & vbCr
            BF2(strBF)
        End If

        If nadjen = False And TypeOf Glbl Is CheckBox Then
            nadjen = True
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-group"">" & vbCr
            'strBF = strBF & vbTab & vbTab & "<label class=""control-label"" for=""" & Glbl.Name & """>" & strBFlabel & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""controls"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<label class=""checkbox"">" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<input type=""checkbox"" class=""col-sm-2 form-control"" id=""" & Glbl.Name & """ name=""" & Glbl.Name & """ value="""" />" & vbCr
            strBF = strBF & vbTab & vbTab & strBFlabel & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</label>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
            strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr & vbCr
            BF2(strBF)
        End If

        If nadjen = False And TypeOf Glbl Is Grid Then
            Dim Tctl As Grid = Glbl
            nadjen = True
            strBFgridovi = vbTab & vbTab & strBFgridovi & "<div id=""" & Glbl.Name & """ class=""tablegrid""  data-webserviceaction="""" data-format=""table"" data-form=""" & strBFformname & """  /></div>" & vbCr
        End If


    End Sub


    Dim strBF2 As String = ""
    Dim strBFbutoni As String = ""
    Dim strBFgridovi As String = ""
    Dim strBFformname As String = ""

    Function BF2(Optional ByVal param As String = "") As String
        On Error Resume Next
        strBF2 = strBF2 & param
        Return ""
    End Function

    Function BF() As String
        On Error Resume Next
        Dim strBF As String = ""

        strBF = strBF & vbTab & "<!-- " & strBFformname.ToUpper & " : " & Me.Text.ToUpper & " -->" & vbCr
        strBF = strBF & vbTab & "<section class=""sectionng"" id=""section" & strBFformname & """ data-sectiontitle=""" & Me.Text & """ >" & vbCr
        strBF = strBF & vbTab & vbTab & "<div class=""container-fluid backstretch-container""  >" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "<div class=""container"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & "<div class=""row"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""col-sm-6 transparent"">" & vbCr
        ' strBF = strBF & vbTab & "<div class=""panel panel-default"">" & vbCr
        ' strBF = strBF & vbTab & "<div class=""panel-heading""><h3 class=""panel-title"">" & Me.Text & "</h3></div>" & vbCr
        ' strBF = strBF & vbTab & "<div class=""panel-body"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<form class=""form-horizontal"" role=""form"" name=""" & strBFformname & """ id=""" & strBFformname & """ >" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<fieldset>" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & vbTab & "<legend>" & Me.Text & "</legend>" & vbCr & vbCr

        strBF = strBF & strBF2 & vbCr

        'For Each ctl As Control In Controls

        '    If TypeOf ctl Is CheckBox Then
        '        Dim Tctl As CheckBox = ctl

        '        strBF = strBF & "<div class=""form-group"">"
        '        strBF = strBF & "<label class=""control-label"" for=""optionsCheckbox"">Checkbox</label>"
        '        strBF = strBF & "<div class=""controls"">"
        '        strBF = strBF & "<label class=""checkbox"">"
        '        strBF = strBF & "<input type=""checkbox"" id=""optionsCheckbox"" value=""option1"">"
        '        strBF = strBF & "Option one is this and that—be sure to include why it's great"
        '        strBF = strBF & "</label>"
        '        strBF = strBF & "</div>"
        '        strBF = strBF & "</div>"

        '    End If

        '    If TypeOf ctl Is mojRadio Then
        '        Dim Tctl As mojRadio = ctl
        '    End If

        '    If TypeOf ctl Is Combo Then
        '        Dim Tctl As Combo = ctl
        '    End If


        '    If TypeOf ctl Is DTPicker Then
        '        Dim Tctl As DTPicker = ctl

        '        If Tctl.Checked Then

        '        Else

        '        End If

        '    End If



        '    If TypeOf ctl Is NumerBox Then
        '        Dim Tctl As NumerBox = ctl
        '    End If

        '    If TypeOf ctl Is SearchBox Then
        '        Dim Tctl As SearchBox = ctl
        '    End If

        '    If TypeOf ctl Is TextBox Then
        '        Dim Tctl As TextBox = ctl
        '    End If


        '    If TypeOf ctl Is Grid Then
        '        Dim Tctl As Grid = ctl
        '    End If

        'Next

        '"<button type=""submit"" class=""btn btn-primary bsjCallWebServiceOnClick"" bsjDisplay=""""  bsjAction=""advancedsearch"" bsjSql="""" bsjFormat="""" bsjForm="""" bsjWebService="""" >OK</button>"

        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & "<div class=""form-actions"">" & vbCr
        strBF = strBF & strBFbutoni & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & "</fieldset>" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & "</form><br />" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "</div> <!-- end of col --> " & vbCr
        strBF = strBF & vbTab & vbTab & "</div> <!-- end of row -->" & vbCr
        strBF = strBF & vbTab & "</div> <!-- end of container -->" & vbCr
        strBF = strBF & vbTab & "</div> <!-- end of container fluid -->" & vbCr & vbCr

        strBF = strBF & vbTab & "<div class=""container-fluid newitem-container"" >" & vbCr
        strBF = strBF & vbTab & "<div class=""container"" >" & vbCr
        strBF = strBF & vbTab & vbTab & "<div class=""row"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "<div class=""col-sm-12"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & "<div id=""" & strBFformname & "addnewitem""></div>" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & "</div>" & vbCr & vbCr

        strBF = strBF & vbTab & "<div class=""containe-fluid list-container"" >" & vbCr
        strBF = strBF & vbTab & "<div class=""container"" >" & vbCr
        strBF = strBF & vbTab & vbTab & "<div class=""row"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "<div class=""col-sm-12"">" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & "<div id=""" & strBFformname & "list""></div>" & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & vbTab & strBFgridovi & vbCr & vbCr
        strBF = strBF & vbTab & vbTab & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & "</div>" & vbCr
        strBF = strBF & vbTab & "</div>" & vbCr & vbCr

        strBF = strBF & vbTab & "</section> <!-- end of section " & strBFformname & " -->" & vbCr
        Return strBF

    End Function


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        OKClick = True
        PokupiParametre()
        strStora = SrediSQL2(strStora)
        strParametri = SrediSQL2(strParametri)
        ' strStartupStoraDetalj = SrediSQL2(strStartupStoraDetalj)

        cls1.strStartupStoraDetalj = cls1.strStartupStoraDetalj

        If strStora.Trim <> "" Then
            If InStr("INSERTDELETEUPDATESELECT", strStora.ToUpper.Substring(0, 6)) > 0 Then
                strPrevParametri = strParametri
                strStora = cls1.SrediSQLPosljeNextForme(strStora)

                'If cls1.strStartupStoraDetalj.Trim <> "" Then
                '    If InStr("INSERTDELETEUPDATESELECT", cls1.strStartupStoraDetalj.ToUpper.Substring(0, 6)) > 0 Then
                '        cls1.strStartupStoraDetalj = cls1.SrediSQLPosljeNextForme(cls1.strStartupStoraDetalj)
                '    End If
                'End If

                strParametri = ""
            End If
        End If

        If NeZelimClosatFormu Then
            NeZelimClosatFormu = False
        Else
            Me.Close()
        End If

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OKClick = False
        Me.Close()
    End Sub


    Public Sub PokupiParametre()
        'On Error Resume Next
        strParametri = ""
        strParametriSvi = ""
        strOpisniParametri = ""
        cls1.strSviParametriuModulu = ""
        Dim StrParametriSaGridova As String = ""
        Dim ii As Integer
        Dim i3 As Long = 0
        Dim SearchBoxCaptioni As String = ""
        Dim ComboBoxCaptioni As String = ""

        Dim Nadjen As Boolean = False

        Dim hctl As Control = Me.ActiveControl

        For Each ctl As Control In Controls

            If TypeOf ctl Is CheckBox Then
                Dim Tctl As CheckBox = ctl
                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & Tctl.CheckState & ","
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & Tctl.CheckState & ","
                strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "
                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.CheckState.ToString.Trim)
            End If

            If TypeOf ctl Is mojRadio Then
                Dim Tctl As mojRadio = ctl
                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.Checked, 1, 0) & ","
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.Checked, 1, 0) & ","
                strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "

            End If


            If TypeOf ctl Is DevExpress.XtraEditors.CheckedComboBoxEdit Then
                Dim Tctl As DevExpress.XtraEditors.CheckedComboBoxEdit = ctl

                ' Dim ids = (From item In Tctl.Properties.Items Where item.CheckState = CheckState.Checked).ToArray()

                strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Properties.GetCheckedItems().Replace(",", "~").replace(" ", "") & "',"
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Properties.GetCheckedItems().Replace(",", "~").replace(" ", "") & "',"
                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.Properties.GetCheckedItems().Replace(",", "~").replace(" ", ""))

            End If

            If TypeOf ctl Is Combo Then
                Dim Tctl As Combo = ctl
                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then
                    If Tctl.SelectedValue Is Nothing Then
                        If Tctl.AllowEmpty = True Then
                            strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                        Else
                            Try
                                Tctl.SelectedValue = 1
                                If Tctl.SelectedValue Is Nothing Then
                                    strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                                Else
                                    strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.SelectedValue.ToString.Trim & Tctl.SelectedText.Trim = "" Or Tctl.SelectedText.Trim = "*", "null", Tctl.SelectedValue.ToString) & ","
                                End If
                            Catch ex As Exception
                                strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                            End Try
                        End If

                    Else
                        strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.SelectedValue.ToString.Trim & Tctl.SelectedText.Trim = "" Or Tctl.SelectedText.Trim = "*", "null", Tctl.SelectedValue.ToString) & ","
                    End If

                End If

                Dim strSelectedNesto As String = ""

                If Tctl.SelectedValue Is Nothing Then
                    strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                    strSelectedNesto = "0"
                ElseIf Tctl.SelectedValue Is DBNull.Value Then
                    strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                    strSelectedNesto = "0"
                Else

                    'If Tctl.SelectedValue.ToString = "System.Data.DataRowView" Then
                    '    strParametriSvi = strParametriSvi & "@" & Tctl.Name & "=null,"
                    'Else
                    '    strParametriSvi = strParametriSvi & "@" & Tctl.Name & "=" & Tctl.SelectedValue & ","
                    'End If

                    If Tctl.Items.Count > 0 Then
                        ' On Error Resume Next
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & Tctl.SelectedValue & ","
                    End If

                    strSelectedNesto = Tctl.SelectedValue

                End If

                'Tctl.Select()

                ComboBoxCaptioni = ComboBoxCaptioni & "@combobox" & Tctl.Name.Trim & "='" & Tctl.Text.Replace(",", " ") & "',"

                Try
                    If Tctl.Text.ToString.Trim <> "" Then strOpisniParametri = strOpisniParametri & " " & Tctl.strCaption.Trim & "=" & Tctl.Text.ToString.Trim & "; "
                Catch ex2 As Exception
                    ' strOpisniParametri = strOpisniParametri & " " & Tctl.strCaption.Trim & "= ; "
                End Try

                Nadjen = True

                SpremiuDBClipboard(Tctl.Name.Trim, strSelectedNesto)

            End If


            If TypeOf ctl Is DTPicker Then
                Dim Tctl As DTPicker = ctl

                If Tctl.Checked Then
                    If Tctl.Tag.trim = "" Then Tctl.Tag = "*"
                    If cls1.RadimoSaAccessBazom Then
                        If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=#" & Tctl.Value.ToString("yyyy-MM-dd") & "#,"
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=#" & Tctl.Value.ToString("yyyy-MM-dd") & "#,"
                    Else


                        If Tctl.CustomFormat Is Nothing Then
                            If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd") & "',"
                            strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd") & "',"

                        Else
                            If Tctl.CustomFormat.ToUpper = "HH:MM" Then
                                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("HH:mm") & "',"
                                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("HH:mm") & "',"

                            ElseIf Tctl.CustomFormat.ToUpper = "MM.YYYY" Then
                                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMM") & "01',"
                                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMM") & "01',"
                            ElseIf Tctl.CustomFormat.ToUpper = "DD.MM.YYYY HH:MM" Then
                                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd HH:mm") & "',"
                                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd HH:mm") & "',"

                            Else

                                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd") & "',"
                                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Value.ToString("yyyyMMdd") & "',"

                            End If
                        End If





                    End If
                    strOpisniParametri = strOpisniParametri & " " & Tctl.strCaption & "=" & Tctl.Value.ToString("yyyyMMdd") & "; "
                Else
                    If Tctl.Tag.trim = "" Then Tctl.Tag = "*"
                    If cls1.RadimoSaAccessBazom Then
                        If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                    Else
                        If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                    End If
                    ' strOpisniParametri = strOpisniParametri & " " & Tctl.strCaption & "=null; "



                End If


                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.Value)

            End If

            'If TypeOf ctl Is NumerBox And Nadjen = False Then
            '    Dim Tctl As NumerBox = ctl
            '    If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name & "=" & IIf(Tctl.Text.Trim = "", 0, Tctl.Text.Replace(",", ".")) & ","
            '    strParametriSvi = strParametriSvi & "@" & Tctl.Name & "=" & IIf(Tctl.Text.Trim = "", 0, Tctl.Text.Replace(",", ".")) & ","
            '    strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text.Replace(",", ".") & "; "
            '    Nadjen = True
            'End If

            If TypeOf ctl Is NumerBox And Nadjen = False Then
                Dim Tctl As NumerBox = ctl



                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then

                    If strParametri.Trim <> "" Then
                        If Not strParametri.EndsWith(",") Then
                            strParametri = strParametri & ","
                        End If
                    End If

                    strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.Text.Trim = "", 0, Tctl.Value.ToString.Replace(",", ".")) & ","

                End If



                If Not strParametriSvi.EndsWith(",") Then
                    strParametriSvi = strParametriSvi & ","
                End If
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & IIf(Tctl.Text.Trim = "", 0, Tctl.Value.ToString.Replace(",", ".")) & ","
                strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Value.ToString.Replace(",", ".") & "; "
                Nadjen = True
                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.Value.ToString.Trim)
            End If

            If TypeOf ctl Is SearchBox And Nadjen = False Then
                Dim Tctl As SearchBox = ctl

                If Tctl.Tag.ToString.Trim = "" Then
                    Tctl.Tag = "0"
                End If


                ' If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then
                If Tctl.SudjelujeUParametrima Then
                    If Tctl.Tag.ToString.Trim <> "0" And Tctl.Tag.ToString.Trim <> "" Then
                        strParametri = strParametri & "@" & Tctl.Name.Trim & "=" & Tctl.Tag & ","
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & Tctl.Tag & ","
                        strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "
                        SearchBoxCaptioni = SearchBoxCaptioni & "@searchbox" & Tctl.Name.Trim & "='" & Tctl.Text.Replace(",", " ") & "',"
                    Else
                        strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                        strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                        If Tctl.Text.Trim <> "" Then strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "
                        SearchBoxCaptioni = SearchBoxCaptioni & "@searchbox" & Tctl.Name.Trim & "='" & Tctl.Text.Replace(",", " ") & "',"
                    End If

                End If

                Nadjen = True

                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.Tag.ToString.Trim)
                SpremiuDBClipboard(Tctl.Name.Trim & "caption", Tctl.Text.Trim)
            End If





            If TypeOf ctl Is TextBox And Nadjen = False Then
                Dim Tctl As TextBox = ctl
                If Tctl.Tag Is Nothing Then
                    Tctl.Tag = " "
                End If
                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.Text.Replace("'", "¸¸").Replace(",", "~~") & "',"
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.Text.Replace("'", "¸¸").Replace(",", "~~") & "',"
                If Tctl.Text.Trim <> "" Then strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "
                Nadjen = True


                ' kk 30.05.2018. ... .Replace("'", "''")
                SpremiuDBClipboard(Tctl.Name.Trim, Tctl.Text.Trim.Replace("'", "¸¸"))

            End If


            If TypeOf ctl Is treeViewMoj And Nadjen = False Then
                Dim Tctl As treeViewMoj = ctl
                If Not Tctl.SelectedNode Is Nothing Then
                    If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "='" & Tctl.SelectedNode.Tag.ToString.Replace("'", "''").Replace(",", "~~") & "',"
                    strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.SelectedNode.Tag.ToString.Replace("'", "''").Replace(",", "~~") & "',"
                    strOpisniParametri = strOpisniParametri & " " & Tctl.Name & "=" & Tctl.SelectedNode.Text & "; "
                Else
                    If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & "@" & Tctl.Name.Trim & "=null,"
                    strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=null,"
                    strOpisniParametri = strOpisniParametri & " " & Tctl.Name & "=; "
                End If

                Nadjen = True
            End If

            If TypeOf ctl Is GridDX Then
                Dim Tctl As GridDX = ctl
                Dim strGetDXID As String = ""
                'strGetDXID = GetDXID(Tctl.gridv)
                'strParametri = strParametri & "@" & Tctl.Name.Trim & "=" &  & " ,"
                'strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "='" & Tctl.SelectedNode.Tag.ToString.Replace("'", "''").Replace(",", "~~") & "',"
                'strOpisniParametri = strOpisniParametri & " " & Tctl.Name & "=" & Tctl.SelectedNode.Text & "; "
            End If

            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                Dim tGetID As String


                If Tctl.RowCount > 0 Then
                    If Tctl.Row = -3 Then Tctl.Row = 1
                    Try
                        tGetID = Tctl.CurrentRow.Cells(2).Value.ToString
                    Catch ex As Exception
                        Try
                            tGetID = Tctl.CurrentRow.Cells(1).Value.ToString
                        Catch ex2 As Exception
                            tGetID = "0"
                        End Try

                    End Try
                Else
                    tGetID = "0"
                End If




                '--------- niz AJDIJEVA...
                KupimnizAjdijeva = True
                Dim si(0) As Long
                Dim NizAJDIJEVA As String = ""
                Dim NizoznacenihAJDIJEVA As String = ""




                Array.Resize(si, Tctl.SelectedItems.Count)

                For i3 = 0 To Tctl.SelectedItems.Count - 1
                    si(i3) = Tctl.SelectedItems(i3).GetRow().RowIndex
                Next

                Dim TempRowiNdex As Integer = 0

                Try
                    TempRowiNdex = Tctl.CurrentRow.RowIndex
                Catch ex As Exception

                End Try


                For i3 = 0 To si.Count - 1
                    'Tctl.MoveToRowIndex(si(i3))

                    Try
                        'NizAJDIJEVA = NizAJDIJEVA & "~" & Tctl.CurrentRow.Cells(2).Value
                        NizAJDIJEVA = NizAJDIJEVA & "~" & Tctl.SelectedItems(i3).GetRow().Cells(2).Value
                    Catch ex As Exception

                    End Try

                Next
                If NizAJDIJEVA.Trim <> "" Then NizAJDIJEVA = NizAJDIJEVA & "~"


                '--------------------------------------------------------------


                Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                checkedRows = Tctl.GetCheckedRows()
                If checkedRows.Length > 0 Then
                    Dim row As Janus.Windows.GridEX.GridEXRow
                    For Each row In checkedRows
                        'Tctl.MoveToRowIndex(row.RowIndex)
                        'NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~" & Tctl.CurrentRow.Cells(2).Value
                        Try
                            NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~" & row.Cells(2).Value
                        Catch ex As Exception

                        End Try

                    Next
                    If NizoznacenihAJDIJEVA.Trim <> "" Then NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~"
                Else
                    Try
                        'NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~" & Tctl.CurrentRow.Cells(2).Value
                        'If NizoznacenihAJDIJEVA.Trim <> "" Then NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~"


                        'Dim row As Janus.Windows.GridEX.GridEXRow
                        'For Each row In checkedRows
                        '    NizoznacenihAJDIJEVA = NizoznacenihAJDIJEVA & "~" & Tctl.CurrentRow.Cells(2).Value
                        'Next
                        NizoznacenihAJDIJEVA = ""
                    Catch ex As Exception
                        NizoznacenihAJDIJEVA = ""
                    End Try

                End If

                '------------------------------------------------------------



                'Try
                '    Tctl.MoveToRowIndex(TempRowiNdex) ' vrati nazad gdje smo bili
                'Catch ex As Exception

                'End Try


                KupimnizAjdijeva = False



                Dim rowcountgrid As Integer = 0
                If Tctl.TotalRow Then
                    rowcountgrid = Tctl.RowCount - 1
                Else
                    rowcountgrid = Tctl.RowCount
                End If


                If Tctl.Tag.ToString.Substring(0, 1) <> "_" Then strParametri = strParametri & ",@" & Tctl.Name.Trim & "=" & tGetID & "," '& "@nizplavihajdijeva" & Tctl.Name.Trim & "='" & NizAJDIJEVA & "'," & "@nizoznacenihajdijeva" & Tctl.Name.Trim & "='" & NizoznacenihAJDIJEVA & "',"

                If strParametriSvi.Trim.EndsWith(",") Then
                    'strParametriSvi = strParametriSvi
                Else
                    strParametriSvi = strParametriSvi & ","
                End If
                strParametriSvi = strParametriSvi & "@" & Tctl.Name.Trim & "=" & tGetID & "," & "@arrayofblueids" & Tctl.Name.Trim & "='" & NizAJDIJEVA & "'," & "@nizplavihajdijeva" & Tctl.Name.Trim & "='" & NizAJDIJEVA & "'," & "@nizoznacenihajdijeva" & Tctl.Name.Trim & "='" & NizoznacenihAJDIJEVA & "'," & "@rowcount" & Tctl.Name.Trim & "=" & rowcountgrid & ","

                strOpisniParametri = strOpisniParametri & " " & Tctl.Tag & "=" & Tctl.Text & "; "




                'Print(Tctl.Tables(Tctl.CurrentTable.Caption).Columns.Count)

                '---------------- v 1.6.7. 27.01.2009. sve kolone sa grida pokupim tako da ih mogu koristiti u ONLOAD na slijedecoj formi
                Dim ParametriSaGrida As String = ""
                ParametriSaGrida = ""
                If Tctl.RowCount > 0 Then

                    If Not Tctl.CurrentRow Is Nothing Then
                        For ii = 1 To Tctl.RootTable.Columns.Count - 1
                            If Tctl.CurrentRow.Cells(ii).Column.Caption.ToString <> "" Then

                                Dim strImeParametra As String = Tctl.CurrentRow.Cells(ii).Column.DataMember.ToString.Trim
                                Try
                                    If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                                        strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                                    End If
                                Catch ex As Exception

                                End Try

                                If Tctl.CurrentRow.Cells(ii).Value Is Nothing Or Tctl.CurrentRow.Cells(ii).Value Is DBNull.Value Then
                                    ' ovo treba testirati
                                    ' ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & Tctl.CurrentRow.Cells(ii).Column.Caption.ToString.Replace(" ", "_") & "=null, "
                                    ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & strImeParametra.Replace(" ", "_").Replace("!", "") & "=null, "

                                Else
                                    If IsNumeric(Tctl.CurrentRow.Cells(ii).Value.ToString) Then
                                        'ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & Tctl.CurrentRow.Cells(ii).Column.Caption.ToString.Replace(" ", "_") & "='" & Tctl.CurrentRow.Cells(ii).Value.ToString.Replace(",", ".") & "' , "
                                        ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & Tctl.CurrentRow.Cells(ii).Value.ToString.Replace(",", ".") & "' , "

                                    ElseIf Tctl.CurrentRow.Cells(ii).Column.Type.ToString.ToLower.Contains("date") Then
                                        'ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & Tctl.CurrentRow.Cells(ii).Column.Caption.ToString.Replace(" ", "_") & "='" & CDate(Tctl.CurrentRow.Cells(ii).Value).ToString("yyyyMMdd") & "' , "
                                        ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & CDate(Tctl.CurrentRow.Cells(ii).Value).ToString("yyyyMMdd") & "' , "

                                    Else
                                        'ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & Tctl.CurrentRow.Cells(ii).Column.Caption.ToString.Replace(" ", "_") & "='" & Tctl.CurrentRow.Cells(ii).Value.ToString.Replace(",", "") & "' , "
                                        ParametriSaGrida = ParametriSaGrida & "@grid" & Tctl.Name.Trim & strImeParametra.Replace(" ", "_").Replace("!", "") & "='" & Tctl.CurrentRow.Cells(ii).Value.ToString.Replace(",", "") & "' , "

                                    End If
                                End If
                            End If
                        Next
                    End If


                End If
                StrParametriSaGridova = StrParametriSaGridova & ParametriSaGrida
                '------------------------------------------------------------------


            End If


            Nadjen = False
        Next ctl

        If Not hctl Is Nothing Then hctl.Focus()

        If strParametri.EndsWith(",") Then
            strParametri = strParametri.Substring(0, strParametri.Length - 1)
        End If

        If strParametriSvi.EndsWith(",") Then
            strParametriSvi = strParametriSvi.Substring(0, strParametriSvi.Length - 1)
        End If

        '---------------- v 1.6.7.  27.01.2009. sve kolone sa grida pokupim tako da ih mogu koristiti u ONLOAD na slijedecoj formi
        strParametri = strParametri & "," & StrParametriSaGridova

        strParametriSvi = strParametriSvi & "," & StrParametriSaGridova
        '-----------------------------------------------------------------------

        If strParametri.EndsWith(",") Then
            strParametri = strParametri.Substring(0, strParametri.Length - 1)
        End If

        If strParametriSvi.EndsWith(",") Then
            strParametriSvi = strParametriSvi.Substring(0, strParametriSvi.Length - 1)
        End If


        cls1.strSviParametriuModulu = strParametriSvi


        ' "TGClipBoard Story" ... 31.10.2007. ---------------------------------------------
        Dim Ar5() As String
        Dim Ime5 As String
        Dim Nasao5 As Integer
        Dim Vrijednost5 As String
        Dim i As Long
        Dim j As Long

        If SearchBoxCaptioni.EndsWith(",") Then
            SearchBoxCaptioni = SearchBoxCaptioni.Substring(0, SearchBoxCaptioni.Length - 1)
        End If

        If SearchBoxCaptioni.Trim <> "" Then
            SearchBoxCaptioni = SearchBoxCaptioni & ","
        End If

        If ComboBoxCaptioni.EndsWith(",") Then
            ComboBoxCaptioni = ComboBoxCaptioni.Substring(0, ComboBoxCaptioni.Length - 1)
        End If

        If ComboBoxCaptioni.Trim <> "" Then
            ComboBoxCaptioni = ComboBoxCaptioni & ","
        End If


        'Ar5 = Split(SearchBoxCaptioni & ComboBoxCaptioni & strParametri, ",") 

        Ar5 = Split(SearchBoxCaptioni & ComboBoxCaptioni & strParametriSvi, ",")

        Try



            For i = 0 To UBound(Ar5)


                If Ar5(i).Trim <> "" Then
                    Ime5 = Ar5(i).Substring(1, InStr(Ar5(i), "=") - 1)

                    Vrijednost5 = Ar5(i).Substring(InStr(Ar5(i), "="), Ar5(i).Length - InStr(Ar5(i), "="))
                    Vrijednost5 = Vrijednost5.Trim   '----- 27.01.2009.
                    If Vrijednost5.StartsWith("'") Then Vrijednost5 = Vrijednost5.Substring(1, Vrijednost5.Length - 1)
                    If Vrijednost5.EndsWith("'") Then Vrijednost5 = Vrijednost5.Substring(0, Vrijednost5.Length - 1)

                    Vrijednost5 = Trim(Vrijednost5)

                    Nasao5 = -1
                    For j = 0 To UBound(cls1.CB)

                        Ime5 = Ime5.Replace("@", "") ' kk dodao  24.10.2018. zbog čudnog ponašanja na tele2 kojeg nismo mogli izazvati kod nas, a radi se o tome da je duplirao @imegrida parametar

                        If UCase(cls1.CB(j, 0)) = UCase(Ime5) Then

                            Nasao5 = j
                        End If
                    Next

                    If Nasao5 = -1 Then
                        cls1.CB(cls1.PrviPrazniSlot, 0) = Ime5
                        cls1.CB(cls1.PrviPrazniSlot, 1) = Vrijednost5
                        cls1.PrviPrazniSlot = cls1.PrviPrazniSlot + 1
                    Else
                        cls1.CB(Nasao5, 1) = Vrijednost5
                    End If
                End If

            Next

        Catch ex As Exception

        End Try

        ' ------------------------------------------------------------ end of "TGClipBoard Story"

    End Sub


    Function GetDXID(GridView1 As GridDXViewer) As String
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

    Sub SpremiuDBClipboard(ByVal strName As String, ByVal strValue As String)
        On Error Resume Next
        If formClosingB = True Then Exit Sub

        Dim xmlname5 As String
        xmlname5 = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

        If strRSDBClipboard.Trim.ToLower = "true" Then
            Dim sql1 As String
            sql1 = "select count(*) from NetGenDBClipboard where operaterid=" & cls1.OperateriID & " and [key]='" & xmlname5 & "/" & strName.Trim & "'"
            Dim trs97 As DataTable
            trs97 = cls1.ExecSql(sql1)
            If Not trs97 Is Nothing Then
                If trs97(0)(0) = 0 Then
                    sql1 = "inseRt into NetGenDBClipboard (operaterid,[key],[value]) values(" & cls1.OperateriID & ",'" & xmlname5 & "/" & strName.Trim & "','" & strValue.Trim & "')"
                    cls1.ExecSql(sql1)
                Else
                    sql1 = "update NetGenDBClipboard set [value]='" & strValue.Trim & "' where operaterid=" & cls1.OperateriID & " and [key]='" & xmlname5 & "/" & strName.Trim & "'"
                    cls1.ExecSql(sql1)
                End If
            End If
        End If
        Err.Clear()
    End Sub

    Private Sub Form2_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated


        On Error Resume Next

        Rafalno(strONActivated, sender, e)

        FormaJeAktivna = True
        If Translated = False Then
            TranslateForm(Me)
            Button1.Text = Translate(Button1.Text)
            Button2.Text = Translate(Button2.Text)
            Button3.Text = Translate(Button3.Text)
            Button4.Text = Translate(Button4.Text)
            Button5.Text = Translate(Button5.Text)
            Button6.Text = Translate(Button6.Text)
            Translated = True
        End If
        Me.Refresh()

        If cls1.bCloseParent = True Then
            cls1.bCloseParent = False
            Button2_Click(sender, e)
        End If

        '   Windows.Forms.SendKeys.Send("{ALT}")

        'If StrWINDOWSTATE = "2" Then
        '    Me.WindowState = FormWindowState.Maximized
        'End If

        Me.BringToFront()


        For Each ctl As Control In Controls
            If TypeOf ctl Is GridDX Then
                Dim Tctl As GridDX = ctl
                cls1.NapuniDXGridLATE(Tctl, Tctl.GridView1, "")
                cls1.CustomFormatKolonaDX(Tctl.GridView1, Tctl.m_nodeList, "", "")
            End If
        Next

        'Windows.Forms.SendKeys.Send("%")
        'Application.DoEvents()
        'Windows.Forms.SendKeys.Send("%")
        'Application.DoEvents()
    End Sub

    Private Sub Form2_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
        'On Error Resume Next
        'SaveSetting("Jupiter", "FormHeight", strImeForme, Me.Height)
        'SaveSetting("Jupiter", "FormWidth", strImeForme, Me.Width)
    End Sub

    Private Sub Form2_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        JupiterLog.Info("Run", "close " & Mid)

        Rafalno(strONClosed, sender, e)



        ' 27.12.2021. KK dodao zatvaranje/otvaranje 
        ' 20.01.2022. KK zakomentirao zatvaranje/otvaranje zbog ## temp tablica
        ' Conn.Close()
        ' Conn.Open()

    End Sub

    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        On Error Resume Next

        'Rafalno(strONClosing, sender, e)
        'If GreskaKodIzvrsavanjaSQLa <> "" Then
        '    e.Cancel = True
        '    Exit Sub
        'End If

        If strRSDBClipboard.Trim.ToLower = "true" Then
            formClosingB = True
            PokupiParametre()
            formClosingB = False
        End If

        SaveSetting("Jupiter", "FormHeight", strXML & "-" & strImeForme & "-" & Version1 & "-" & sender.Version2, Me.Height)
        SaveSetting("Jupiter", "FormWidth", strXML & "-" & strImeForme & "-" & Version1 & "-" & sender.Version2, Me.Width)
        On Error Resume Next

        If PrvaForma = True Then
            SendMessage(DirectCast(Globalmain_handle, IntPtr), &H7D0, Me.Handle, IntPtr.Zero)

            Conn.Close()


            Conn2.Close()



            cls1.Form2.Close()
            frm3.Close()
            cls1.Form5.Close()

            frm5.Close()

            frm5 = Nothing
            frm3 = Nothing
            frm1 = Nothing
        End If
    End Sub


    Private Sub Form2_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown

        On Error Resume Next

        If e.KeyCode = Keys.F1 Then

            If strHelpKod2.Trim <> "" Then

                'o = CreateObject("JupiterHelpClient.XtraForm1", "")
                'o.run("aaa")


                'Dim a As Assembly = Assembly.Load("JupiterHelpClient")
                'Dim o As Object = a.CreateInstance("JupiterHelpClient.XtraForm1")
                'o.run("aaa")


                DajPokaziHelp()
                Exit Sub
            End If

        End If




        If e.KeyCode = Keys.Escape Then
            If strONESCKey.Trim <> "" Then
                Rafalno(strONESCKey, sender, e)
            Else
                Me.Close()
            End If

        End If

        If e.Control Then
            If e.KeyCode = Keys.F11 Then
                PokupiParametre()
                cls1.PokaziCBLog("PREVPARAMETRI=" & strPrevParametri & vbCrLf & vbCrLf & "PARAMETRI=" & strParametri & vbCrLf & vbCrLf & "PARAMETRISVI=" & strParametriSvi & vbCrLf & vbCrLf & vbCrLf & "PARAMETRIIZTRAZILICE=" & strParametriIzTrazilice & vbCrLf)
                ControlClicked = False
                'frm5.ShowDialog()
                'RaiseEvent Log5()
            End If


            If e.KeyCode = Keys.F6 Then
                PokupiParametre()
                cls1.PokaziCBLog(BF())
                ControlClicked = False
                'frm5.ShowDialog()
                'RaiseEvent Log5()
            End If



        End If

        '----------------- HOTKEY's ----------------------------------
        If (e.KeyCode > 111 And e.KeyCode < 122) Or e.KeyCode = Keys.Multiply Or e.KeyCode = 42 Or e.KeyCode.ToString = "*" Then
            For Each ctl As Control In Controls


                If TypeOf ctl Is CheckBox Then
                    Dim Tctl As CheckBox = ctl

                End If




                If TypeOf ctl Is Button Then
                    Dim Tctl As Buton = ctl

                    If Tctl.HotKey.Trim = "*" Then Tctl.HotKey = "Multiply"

                    If Tctl.HotKey.Trim = e.KeyCode.ToString.Trim Then

                        If Tctl.Enabled Then Glbl_ClickButton(Tctl, e)

                        'Tctl.Focus()
                    End If
                End If

                If TypeOf ctl Is Combo Then
                    Dim Tctl As Combo = ctl
                    If Tctl.HotKey = e.KeyCode.ToString Then
                        Tctl.Focus()
                    End If
                End If

                If TypeOf ctl Is DateTimePicker Then
                    Dim Tctl As DateTimePicker = ctl

                End If

                If TypeOf ctl Is NumerBox Then
                    Dim Tctl As NumerBox = ctl
                    If Tctl.HotKey = e.KeyCode.ToString Then
                        Tctl.Focus()
                    End If
                End If

                If TypeOf ctl Is SearchBox Then
                    Dim Tctl As SearchBox = ctl
                    If Tctl.HotKey = e.KeyCode.ToString Then
                        Tctl.Focus()
                    End If
                End If

                If TypeOf ctl Is TextBox Then

                End If

                If TypeOf ctl Is Grid Then
                    Dim Tctl As Grid = ctl
                    If Tctl.HotKey = e.KeyCode.ToString Then
                        Tctl.Focus()
                    End If
                End If
            Next ctl
        End If

    End Sub

    Sub NovaGenerickaForma2(ByVal ImeForme As String)
        Dim Trs As New DataTable
        Dim Tid As String
        Dim Form2b As New Form2
        Dim cf As Form2
        Dim ImeForme5 As String = ""
        Dim tpP As String = ""
        Form2b.cls1 = cls1
        cls1.NapuniCBLog("GEN FORM CALL : NovaGenerickaForma2(" & ImeForme & ")")
        AktivnaKontola = Me.ActiveControl

        PokupiParametre()

        'If ImeForme.Contains("@@") Then
        '    ImeForme = cls1.SrediSQLPosljeNextForme(SrediSQL2(ImeForme), strParametriSvi)
        'End If

        If ImeForme.ToUpper.StartsWith("VRSTADOKID-") Or ImeForme.Substring(0, 5).ToUpper = "RSGEN" Or ImeForme.Substring(0, 3).ToUpper = "GG/" Or ImeForme.Substring(0, 3).ToUpper = "RS/" Or ImeForme.Substring(0, 3).ToUpper = "RS:" Or ImeForme.ToUpper.StartsWith("REPX:") Or ImeForme.ToUpper.StartsWith("REPX/") Or ImeForme.ToLower.Contains(".clsreport") Then

            arr(0) = ID

            If ImeForme.IndexOf("(") > 0 Then
                ImeForme5 = ImeForme.Trim
                ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
                tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
                tpP = cls1.SrediSQLPosljeNextForme(SrediSQL2(tpP), strParametriSvi)
                'tpP = tpP.Replace("~~", ",")
                ' gogo trazio 18.12.2008.
                arr(0) = Val(tpP)
                If arr(0) = 0 Then arr(0) = ID
            End If


            ImeForme = ImeForme.Replace("rsgen", "rsgen:")
            ImeForme = ImeForme.Replace("RSGEN", "RSGEN:")
            ImeForme = ImeForme.Replace("rs/", "rs:/")
            ImeForme = ImeForme.Replace("RS/", "RS:/")
            ImeForme = ImeForme.Replace("REPX/", "REPX:/")
            ImeForme = ImeForme.Replace("repx/", "repx:/")

            ImeForme = cls1.SrediSQLPosljeNextForme(SrediSQL2(ImeForme), strParametriSvi)

            cls1.NapuniCBLog(" Krećem u ispis  : " & ImeForme & "   " & tpP)

            Dim handle As IntPtr = Me.Handle
            cls1.handle5 = handle


            'cls1.NapuniCBLog(" VALENTINO   Parent.Handle: " & Parent.Handle.ToString())
            'cls1.NapuniCBLog(" VALENTINO   ParentForm.Handle: " & ParentForm.Handle.ToString())


            'If ImeForme.ToLower().StartsWith("multi") Then
            '    cls1.NapuniCBLog(" VALENTINO  multi Owner.Handle: " & Owner.Handle.ToString())
            '    cls1.PrintRpt(ImeForme, tpP, Owner.Handle)
            'Else
            '    cls1.NapuniCBLog(" VALENTINO  single Me.Handle: " & Me.Handle.ToString())
            '    cls1.PrintRpt(ImeForme, tpP, Me.Handle)
            'End If

            cls1.NapuniCBLog(" VALENTINO    Owner.Handle: ")
            'cls1.NapuniCBLog(" VALENTINO    Owner.Handle: " & Owner.Handle.ToString())
            'cls1.NapuniCBLog(" WWWWW    Me.Handle: " & Me.Handle.ToString())
            cls1.PrintRpt(ImeForme, tpP, Me.Handle)

            'cls1.PrintRpt(ImeForme, tpP, Me.Handle)



            Exit Sub
        End If


        'If ImeForme.Contains(".") Then
        '    Dim o As Object
        '    o = CreateObject(ImeForme, "")
        '    o.totalgen(ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, "")
        '    Exit Sub
        'End If


        ' dodao And Not ImeForme.Contains("@") .... dakle , ne želimo kreirati objekt , a imamo točku ... problem je ako šaljemo paramete koji sadrže točku
        If (ImeForme.Contains(".") And Not ImeForme.Contains("@")) Or ImeForme.Contains("..") Then

            On Error Resume Next

            If ImeForme.Contains("..") Then
                ImeForme = ImeForme.Replace("..", ".")
            End If

            'PokupiParametreSaGrida()
            tpP = ""
            If ImeForme.IndexOf("(") > 0 Then
                ImeForme5 = ImeForme.Trim
                ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
                tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
                tpP = cls1.SrediSQLPosljeNextForme(SrediSQL2(tpP), strParametriSvi.ToString)
            End If
            Err.Clear()

            If o2str <> ImeForme Then
                o2str = ImeForme
                o2 = CreateObject(ImeForme, "")
            End If

            o2.totalgen(ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, tpP)

            If Err.Number <> 0 Then
                Err.Clear()

                o2 = CreateObject(ImeForme, "")
                If Err.Number <> 0 Then
                    MsgBox(Err.Description.ToString.Trim + "   " + ImeForme.ToString.Trim)
                    Exit Sub
                End If
                Err.Clear()
                o2.totalgen(ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, tpP)

                If Err.Number <> 0 Then
                    'MsgBox(Err.Description)
                    cls1.NapuniCBLog(ImeForme & " .... PROBLEM : " & Err.Description)
                    Exit Sub
                End If

            End If




            Exit Sub
        End If


        Form2b.ID = ID
        cls1.MasterID = ID
        Form2b.Ikona = Ikona

        cf = Form2b
        Form2b.strPrevParametri = ""

        Form2b.strPrevParametri = strParametriSvi
        Form2b.OKClick = False
        Form2b.strXML = strXML2 & "\" & strXMLfile2
        Form2b.strXML = strXMLfile2
        Form2b.strImeForme = ImeForme
        Form2b.strXML2 = strXML2
        Form2b.strXMLfile2 = strXMLfile2

        If ImeForme.IndexOf("(") > 0 Then ' 1.10.2009. dodao mogucnost da se forma zove sa parametrima npr. IMEFORME(@pero=@@id,@djuro=@@gridUvjetiklasa)
            ImeForme5 = ImeForme.Trim
            ImeForme = ImeForme.Substring(0, ImeForme.IndexOf("("))
            tpP = ImeForme5.Substring(ImeForme5.IndexOf("(") + 1, ImeForme5.Length - ImeForme5.IndexOf("(") - 2)
            tpP = cls1.SrediSQLPosljeNextForme(SrediSQL2(tpP), strParametriSvi)

            If InStr(tpP, "-freezid") Then
                Form2b.FreezID = True
                tpP = tpP.Replace("-freezid", "")
            End If

            strParametriSvi = tpP & "," & strParametriSvi
        End If

        Form2b.strImeForme = cls1.SrediSQLPosljeNextForme(SrediSQL2(ImeForme), strParametriSvi)

        If ssPermissionProvjeriRafalnuKomandu(Form2b.strImeForme) = False Then
            cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED LOADING " + Form2b.strImeForme)
            Exit Sub
        End If

        Form2b.ProcitajXML()
        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub '-- ako je došlo do greške na ONLOAD forme idemo van
        If Form2b.BrojKontrola > 0 Then Form2b.ShowDialog(Me) Else Form2b.OKClick = True ' kkkkkkk valentino dodao (me)
        While cf.OKClick And cf.strNextForm <> ""
            Dim PrevParametri As String = cf.strParametriSvi
            Dim tstrNextForm As String = cf.strNextForm
            Tid = cf.ID
            If cf.strStora <> "" Then
                Trs = cls1.ExecSql(cf.strStora & " " & cf.strParametri)
                If Trs.Rows.Count > 0 Then
                    Tid = Trs.Rows(0)(0)
                End If
            End If
            cf.Close()

            RefreshKontrola()

            Dim Form2c As New Form2
            Form2c.cls1 = cls1
            cf = Form2c
            Form2c.ID = Tid
            Form2c.Ikona = Ikona
            cls1.MasterID = Tid

            Form2c.strPrevParametri = PrevParametri
            Form2c.OKClick = False
            Form2c.strXML = strXML2 & "\" & strXMLfile2
            Form2c.strXML = strXMLfile2
            Form2c.strXML2 = strXML2
            Form2c.strXMLfile2 = strXMLfile2
            Form2c.strImeForme = tstrNextForm
            If ssPermissionProvjeriRafalnuKomandu(Form2b.strImeForme) = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED LOADING " + Form2b.strImeForme)
                Exit Sub
            End If
            Form2c.ProcitajXML()
            If Form2c.BrojKontrola > 0 Then Form2c.ShowDialog(Me) Else Form2c.OKClick = True  ' kkkkkkk valentino dodao (me)
        End While
        If cf.OKClick And cf.strNextForm = "" Then
            'cls1.ExecSql(cf.strStora & " " & cf.strParametri)

            If cf.strParametri.Trim <> "" Then   '----------- potrebno je izbaciti @grid parametre
                If InStr(cf.strParametri, "@grid") Then
                    cf.strParametri = cf.strParametri.Substring(0, InStr(cf.strParametri, ",@grid") - 1)
                End If
            End If

            If cf.strStora.Trim <> "" Then cls1.ExecSql(cls1.SrediSQLPosljeNextForme(cf.strStora & " " & cf.strParametri))

        End If
        RefreshKontrola()
        cf.Close()
    End Sub

    Sub DodatnoPostimajKontrolu(ByVal tctl As Control, ByVal tstr6 As String)
        On Error Resume Next
        If tstr6.Trim = "" Then Exit Sub
        Select Case tstr6.ToLower.Trim
            Case "visible"
                tctl.Visible = True
                Me.Controls("lbl" & tctl.Name).Visible = True
                ssPermissionProvjeriKontrolu(tctl.Name)
                ssPermissionProvjeriKontrolu("lbl" & tctl.Name)
            Case "invisible"
                tctl.Visible = False
                Me.Controls("lbl" & tctl.Name).Visible = False
            Case "tabstoped"
                tctl.TabStop = True
            Case "tabnotstoped"
                tctl.TabStop = False
            Case "enable"
                'tctl.Enabled = True

                If TypeOf tctl Is Grid Then
                    'tctl.Enabled = False

                    Dim Tctl2 As Grid = tctl
                    Tctl2.AllowEdit = InheritableBoolean.True

                ElseIf TypeOf tctl Is DTPicker Then
                    tctl.Enabled = True
                Else
                    tctl.Enabled = True
                End If

                ssPermissionProvjeriKontrolu(tctl.Name)
            Case "disable"

                If TypeOf tctl Is Grid Then
                    'tctl.Enabled = False

                    Dim Tctl2 As Grid = tctl
                    Tctl2.AllowEdit = InheritableBoolean.False

                ElseIf TypeOf tctl Is DTPicker Then
                    tctl.Enabled = False
                Else
                    tctl.Enabled = False
                End If

            Case "refresh"
                If TypeOf tctl Is WebBrowser Then
                    Dim Tctl3 As WebBrowser = tctl
                    Tctl3.Refresh()
                End If
            Case Else
                If TypeOf tctl Is SearchBox Or TypeOf tctl Is Button Or TypeOf tctl Is TeksT Or TypeOf tctl Is ComboBox Or TypeOf tctl Is RadioButton Or TypeOf tctl Is CheckBox Then

                ElseIf TypeOf tctl Is Grid Then
                    Dim Tctl3 As Grid = tctl
                    Dim c4 As GridEXColumn = Tctl3.RootTable.Columns(2)
                    If Tctl3.RowCount > 0 Then
                        If Tctl3.Find(c4, ConditionOperator.Equal, tstr6, -1, 1) = False Then

                        End If
                        Tctl3.EnsureVisible()
                    End If
                ElseIf TypeOf tctl Is WebBrowser Then
                    Dim Tctl3 As WebBrowser = tctl
                    Tctl3.Navigate(tstr6)
                Else
                    tctl.Text = Translate(tstr6)
                    tctl.Refresh() ' $$$$$
                End If
                ' tctl.Text = tstr6
        End Select
    End Sub

    Private Sub RefreshControls(ByVal sql As String) ' za sada samo numberbox-ove
        If sql = "*" Then sql = strOnLostFocusSQL
        If sql.Trim = "" Then Exit Sub
        PokupiParametre()
        Dim tstr6 As String = ""
        Dim Ime5 As String
        Dim Nasao5 As Integer
        Dim strFindNode3 As String
        strPrevParametri = strParametriSvi
        sql = sql.Replace("++", strParametri)
        sql = SrediSQL2(sql)
        sql = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(sql, strPrevParametri))
        Dim Trs2 As New DataTable
        Dim Trs3 As New DataTable
        On Error Resume Next
        Trs2 = cls1.ExecSql(sql)

        If Err.Number <> 0 Or GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
            If strAktivnaKontrolanakonLodanja <> "" Then
                AktivnaKontola = Me.Controls(strAktivnaKontrolanakonLodanja)
                strAktivnaKontrolanakonLodanja = ""
                Timer1.Enabled = True
            End If
            Exit Sub
        End If

        If Trs2 Is Nothing Then Exit Sub
        If Trs2.Rows.Count = 0 Then Exit Sub



        ''---------- ' update CB sa querijem iz rafalnog  .............. TGClipBoard update ... 19.01.2009.
        'Dim Ime5 As String
        'Dim Nasao5 As Integer
        'Dim Vrijednost5 As String

        'If Err.Number = 0 Then
        '    Dim Column As DataColumn
        '    If Trs2.Rows.Count > 0 Then
        '        For Each Column In Trs2.Columns
        '            Vrijednost5 = "null"
        '            Ime5 = Column.Caption & "="
        '            Vrijednost5 = Trs2.Rows(0)(Column.Caption)

        '            If Vrijednost5.StartsWith("'") Then Vrijednost5 = Vrijednost5.Substring(1, Vrijednost5.Length - 1)
        '            If Vrijednost5.EndsWith("'") Then Vrijednost5 = Vrijednost5.Substring(0, Vrijednost5.Length - 1)

        '            Vrijednost5 = Trim(Vrijednost5)

        '            Nasao5 = -1
        '            For j = 0 To UBound(cls1.CB)
        '                If UCase(cls1.CB(j, 0)) = UCase(Ime5) Then
        '                    Nasao5 = j
        '                End If
        '            Next

        '            If Nasao5 = -1 Then
        '                ' 27.11.2008 odkomentirao , onload može i dodati vrijednosti u cb
        '                cls1.CB(cls1.PrviPrazniSlot, 0) = Ime5
        '                cls1.CB(cls1.PrviPrazniSlot, 1) = Vrijednost5
        '                cls1.PrviPrazniSlot = cls1.PrviPrazniSlot + 1
        '            Else
        '                cls1.CB(Nasao5, 1) = Vrijednost5
        '            End If
        '        Next
        '    End If
        'End If
        ''---------------------------------------------------------------------------



        On Error Resume Next
        Dim ts2 As String = ""
        ts2 = Trs2.Rows(0)("focus")
        If ts2 <> "" Then
            AktivnaKontola = Me.Controls(ts2)
            Me.Controls(ts2).Select()
        End If


        Dim ts1212 As String = ""
        ts1212 = Trs2.Rows(0)("fillexplorer")
        If ts1212 <> "" Then
            Me.strFillExplorer = ts1212
        End If


        Dim ts33 As String = ""
        ts33 = Trs2.Rows(0)("formcaption")
        If ts33 <> "" Then
            Me.Text = Translate(ts33)
        End If


        Dim ts4 As String = ""
        ts4 = Trs2.Rows(0)("beep")
        If ts4 <> "" Then
            If Val(ts4) > 30 Then
                ts4 = "30"
            End If
            Dim i6 As Integer
            For i6 = 1 To Val(ts4)
                Microsoft.VisualBasic.Interaction.Beep()
                System.Threading.Thread.Sleep(300)
            Next
        End If


        Dim ts5 As String = ""
        ts5 = Trs2.Rows(0)("form")
        If ts5 <> "" Then
            NovaGenerickaForma2(ts5)
        End If


        Dim ts88 As String = ""
        ts88 = Trs2.Rows(0)("formwidth")
        If ts88 <> "" Then
            Me.Width = ts88
        End If


        Dim ts99 As String = ""
        ts99 = Trs2.Rows(0)("formheight")
        If ts99 <> "" Then
            Me.Height = ts99
        End If


        Dim ts57 As String = ""
        ts57 = Trs2.Rows(0)("click")
        If ts57 <> "" Then
            Glbl_Click(Controls(ts57), Nothing)
        End If


        strFindNode3 = ""
        Dim ts537 As String = ""
        ts537 = Trs2.Rows(0)("findnode")
        If ts537 <> "" Then
            strFindNode = ts537
            strFindNode3 = ts537
        End If


        Dim ts5377 As String = ""
        ts5377 = Trs2.Rows(0)("setcolorforcolorpicker")
        If ts5377 <> "" Then
            ColorDialog1.Color = System.Drawing.ColorTranslator.FromHtml(ts5377)
            strIzabranaBoja = ts5377
        End If




        Dim ts3 As String = "" ' --- @@id je moguće setirati i iz rafalnog i to ako se polje zove "ID"
        If FreezID = False Then
            ts3 = Trs2.Rows(0)("id")
            If ts3 <> "" Then
                ID = ts3
                cls1.MasterID = ts3
                IDzaNoviSlog = ts3
            End If
        Else ' ako lodana forma ima zabranu za setiranje ID-a onda ce taj ID ipak zapamtiti u @@imeforme_id u CB-u

            ts3 = Trs2.Rows(0)("id").ToString.Trim
            If ts3 <> "" Then
                Ime5 = strImeForme & "_ID="
                Nasao5 = -1
                For j = 0 To UBound(cls1.CB)
                    If UCase(cls1.CB(j, 0)) = UCase(Ime5) Then
                        Nasao5 = j
                    End If
                Next

                If Nasao5 = -1 Then
                    ' 21.05.2009 setdefaults iz project taga može i dodati vrijednosti u cb
                    cls1.CB(cls1.PrviPrazniSlot, 0) = Ime5
                    cls1.CB(cls1.PrviPrazniSlot, 1) = ts3
                    cls1.PrviPrazniSlot = cls1.PrviPrazniSlot + 1
                Else
                    cls1.CB(Nasao5, 1) = ts3
                End If

            End If

        End If


        ts3 = ""
        ts3 = Trs2.Rows(0)("temp")
        If ts3 <> "" Then
            ID = ts3
            cls1.strTemp = ts3
        End If


        If StatusStrip1.Visible Then
            ToolStripStatusLabel1.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel1.Tag.ToString), strPrevParametri))
            ToolStripStatusLabel2.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel2.Tag.ToString), strPrevParametri))
            ToolStripStatusLabel3.Text = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(SrediSQL2(ToolStripStatusLabel3.Tag.ToString), strPrevParametri))
        End If


        Dim Nadjen As Boolean = False
        Dim Tstr As String = ""
        On Error Resume Next
        For Each ctl As Control In Controls
            Tstr = ""

            If TypeOf ctl Is PictureBox Then
                Dim Tctl As PictureBox = ctl
                Nadjen = True
                tstr6 = ""

                Err.Clear()
                Dim bBLOBStorage() As Byte = Trs2.Rows(0)(Tctl.Name.Trim)

                If Err.Number = 0 Then
                    Dim ms As New MemoryStream(bBLOBStorage)
                    Tctl.SizeMode = PictureBoxSizeMode.StretchImage
                    Tctl.Image = Image.FromStream(ms)
                    ms.Close()
                    cls1.NapuniCBLog(" PictureBox from DB OK , " & Err.Description)
                Else
                    'Tctl.Image = Nothing
                    cls1.NapuniCBLog(" PictureBox from DB Error , " & Err.Description)
                    'Tctl.Image = Nothing
                End If


                Dim IMGloc As String = ""
                Err.Clear()
                IMGloc = Trs2.Rows(0)(Tctl.Name.Trim & "location")
                If Err.Number = 0 Then
                    If IMGloc.Trim <> "" Then
                        Tctl.ImageLocation = IMGloc
                        cls1.NapuniCBLog(" PictureBox from location OK , " & Err.Description)
                    End If
                End If
                If Err.Number <> 0 Then
                    'cls1.NapuniCBLog(" PictureBox from location error , " & Err.Description)

                End If



            End If


            If TypeOf ctl Is DevExpress.XtraEditors.CheckedComboBoxEdit And Nadjen = False Then
                Dim trs As DataTable
                Dim Tctl As DevExpress.XtraEditors.CheckedComboBoxEdit = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                If tstr6.Trim <> "" Then
                    If InStr(tstr6, "refresh") Then
                        Tctl.Text = ""
                        Tctl.Tag = ""
                        'Tctl.Properties.PopupFormSize = System.Drawing.Size(123)
                        trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(strMultiDropSQL, ""))
                        Tctl.Properties.DataSource = trs
                        Tctl.Properties.DisplayMember = trs.Columns(1).Caption
                        Tctl.Properties.ValueMember = trs.Columns(0).Caption
                        Tctl.ShowPopup()
                        Tctl.ClosePopup()
                    Else
                        Tctl.Tag = tstr6.Trim
                    End If
                End If

                'MsgBox("wwww" & tstr6)

                'If Tctl.Properties.Items.Count = 0 Then

                '    ' Tctl.Visible = False

                '    'Tctl.ShowPopup()
                '    'Tctl.ClosePopup()

                '    Dim nizajdijeva6 As String = ""
                '    nizajdijeva6 = Tctl.Tag.Trim

                '    If nizajdijeva6 <> "" Then
                '        Tctl.Tag = ""
                '        nizajdijeva6 = ",,," & nizajdijeva6.Replace("~", ",") & ","
                '        'MsgBox(nizajdijeva6)
                '        ' Tctl.SetEditValue = 
                '        For i8 = 0 To Tctl.Properties.Items.Count - 1
                '            ' MsgBox(i8)
                '            If nizajdijeva6.IndexOf("," & Tctl.Properties.Items(i8).Value.ToString.Trim & ",", 0) > 0 Then
                '                Tctl.Properties.Items(i8).CheckState = CheckState.Checked
                '            End If
                '        Next
                '    End If
                '    ' Tctl.Visible = True
                'End If

            End If


            If TypeOf ctl Is NumerBox And Nadjen = False Then
                Dim Tctl As NumerBox = ctl
                Dim ImamoGresku As Boolean = False
                Nadjen = True
                tstr6 = ""
                On Error Resume Next
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                If Err.Number <> 0 Then
                    ImamoGresku = True
                End If
                DodatnoPostimajKontrolu(Tctl, tstr6)

                ' Tctl.Enabled = True

                'Tctl.Text = "0"
                'Tctl.Value = 0
                If tstr6.Trim <> "" Then
                    If tstr6.Trim.ToLower <> "disable" Then
                        If tstr6.Trim.ToLower <> "enable" Then
                            If tstr6.Trim.ToLower <> "visible" Then
                                If tstr6.Trim.ToLower <> "invisible" Then
                                    If tstr6.Trim.ToLower <> "refresh" Then
                                        Err.Clear()
                                        Tctl.Text = Trs2.Rows(0)(Tctl.Name.Trim)
                                        Tctl.Value = Trs2.Rows(0)(Tctl.Name.Trim)
                                        If Err.Number <> 0 Then
                                            'cls1.NapuniCBLog("tstr6 =  " & tstr6 & "     Tctl.Name.Trim = " & Tctl.Name.Trim & vbCr & Err.Description)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'Tctl.Enabled = False

            End If


            If TypeOf ctl Is Label Then
                Dim Tctl As Label = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)
                'Tctl.Text = Trs2.Rows(0)(Tctl.Name.Trim)
            End If


            If TypeOf ctl Is WebBrowser Then
                Dim Tctl As WebBrowser = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)

                ' Tctl.Document.AttachEventHandler() ???




            End If


            If TypeOf ctl Is DTPicker Then
                Dim Tctl As DTPicker = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)

                If tstr6.ToLower = "checked" Then
                    Tctl.Checked = True
                End If

                If tstr6.ToLower = "unchecked" Then
                    Tctl.Checked = False
                End If

                'Tctl.Text = Trs2.Rows(0)(Tctl.Name.Trim)
            End If


            If TypeOf ctl Is SearchBox Then
                Dim Tctl As SearchBox = ctl
                Nadjen = True

                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)


                If tstr6.Trim.ToLower = "disable" Or tstr6.Trim.ToLower = "enable" Or tstr6.Trim.ToLower = "visible" Or tstr6.Trim.ToLower = "invisible" Then

                Else
                    Tctl.Tag = Trs2.Rows(0)(Tctl.Name.Trim)
                End If

                If Tctl.Tag = "0" Then
                    If Tctl.Text.Trim <> "*" Then Tctl.Text = ""
                Else
                    If tstr6.Trim.ToLower = "disable" Or tstr6.Trim.ToLower = "enable" Or tstr6.Trim.ToLower = "visible" Or tstr6.Trim.ToLower = "invisible" Then

                    Else
                        Tctl.Text = Trs2.Rows(0)(ToolTip1.GetToolTip(Tctl).Substring(0, ToolTip1.GetToolTip(Tctl).IndexOf("#")).Replace("*", "")).ToString.Trim
                    End If

                End If
            End If


            If TypeOf ctl Is TextBox And Nadjen = False Then
                Dim Tctl As TextBox = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)
                If tstr6.Trim.ToLower = "disable" Or tstr6.Trim.ToLower = "enable" Or tstr6.Trim.ToLower = "visible" Or tstr6.Trim.ToLower = "invisible" Then

                Else
                    Tctl.Text = Trs2.Rows(0)(Tctl.Name.Trim)
                End If

            End If


            If TypeOf ctl Is Button Then
                Dim Tctl As Button = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                If tstr6.Trim <> "" Then
                    If tstr6.Trim.ToLower = "disable" Or tstr6.Trim.ToLower = "enable" Or tstr6.Trim.ToLower = "visible" Or tstr6.Trim.ToLower = "invisible" Then

                    Else
                        Tctl.Text = tstr6
                    End If
                End If

                DodatnoPostimajKontrolu(Tctl, tstr6)

            End If



            If TypeOf ctl Is CheckBox Then
                Dim Tctl As CheckBox = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)
                Tctl.Checked = Trs2.Rows(0)(Tctl.Name.Trim)
            End If

            If TypeOf ctl Is RadioButton Then
                Dim Tctl As RadioButton = ctl
                Nadjen = True
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)
                If tstr6.Trim = "1" Or tstr6.ToLower.Trim = "true" Then
                    Tctl.Checked = True
                    'PokupiParametre()
                    'Tctl.PerformClick()
                End If

            End If


            If TypeOf ctl Is DevExpress.XtraEditors.CheckedComboBoxEdit Then
                If InStr(Tstr, "refresh") Then

                End If
            End If


            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                Nadjen = True
                Tstr = "##"
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)
                Tstr = Trs2.Rows(0)(Tctl.Name.Trim).ToString.ToLower
                If InStr(Tstr, "refresh") Then
                    RefreshGrida(Tctl, Tstr)
                End If

                If InStr(Tstr, "movelast") Then
                    Tctl.MoveLast()
                End If

                If InStr(Tstr, "movefirst") Then
                    Tctl.MoveFirst()
                End If

                If InStr(Tstr, "movetonew") Then
                    Tctl.MoveToNewRecord()
                End If

                If InStr(Tstr, "movenext") Then
                    Tctl.MoveNext()
                End If

                If InStr(Tstr, "moveprevious") Then
                    Tctl.MovePrevious()
                End If

            End If



            If TypeOf ctl Is treeViewMoj Then
                Dim Tctl As treeViewMoj = ctl
                Nadjen = True
                Tstr = "##"
                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                ' DodatnoPostimajKontrolu(Tctl, tstr6)
                Tstr = Trs2.Rows(0)(Tctl.Name.Trim).ToString.ToLower
                If InStr(Tstr, "refresh") Then
                    Tctl.Nodes.Clear()
                    Dim a() As String
                    a = Tctl.SQL.Split("#")
                    For j = 0 To a.GetUpperBound(0)
                        If a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower = "yearmonth" Then

                        ElseIf a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")).ToLower.Contains("tree") Then
                            DazovaIzStore(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")), a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1), Tctl)
                        Else
                            Dim YearNode As TreeNode = New TreeNode(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")))
                            YearNode.Tag = ":" + a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                            Tctl.Nodes.Add(YearNode)
                        End If
                    Next

                    Tctl.ExpandAll()

                End If


                If strFindNode3.Trim <> "" Then
                    strFindNode = strFindNode3.Trim
                    Dim arr As TreeNode() = Tctl.Nodes.Find(strFindNode, True)
                    For i2 = 0 To arr.Length - 1
                        Tctl.SelectedNode = arr(i2)
                        Tctl.SelectedNode.Expand()
                        Tctl.SelectedNode.BackColor = Color.GreenYellow
                        Tctl.SelectedNode.EnsureVisible()
                    Next
                End If


            End If



            If TypeOf ctl Is Combo Then
                Dim Tctl As Combo = ctl
                Nadjen = True
                Tstr = Trs2.Rows(0)(Tctl.Name.Trim).ToString.ToLower

                tstr6 = ""
                tstr6 = Trs2.Rows(0)(Tctl.Name.Trim)
                DodatnoPostimajKontrolu(Tctl, tstr6)

                'Tctl.SelectedIndexMOJ = -1


                If Not tstr6 Is Nothing Then
                    If tstr6 <> "" Then
                        If tstr6.Trim.ToLower = "disable" Or tstr6.Trim.ToLower = "enable" Or tstr6.Trim.ToLower = "visible" Or tstr6.Trim.ToLower = "invisible" Then

                        Else

                            'Tctl.SelectedValue = Nothing
                            'Tctl.SelectedValue = 0
                            'Tctl.SelectedValue = tstr6



                            If Tstr.Trim.ToLower <> "refresh" Then
                                Tctl.SelectedValue = tstr6
                                Tctl.SelectedIndexMOJ = Tctl.SelectedIndex
                                If Tctl.SelectedIndexMOJ = -1 Then
                                    Tctl.SelectedIndex = 0
                                    Tctl.SelectedIndexMOJ = Tctl.SelectedIndex
                                End If
                            End If

                            '  Tctl.SelectedTextMOJ = Trs.Columns(1).Caption

                        End If

                    End If
                End If



                If Tstr = "refresh" Then
                    strPrevParametri = strParametriSvi
                    Dim ts8 As String = ""
                    ts8 = Tctl.SelectedValue
                    Dim Trs As New DataTable
                    Trs = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(Tctl.SQL))

                    If Tctl.AllowEmpty Then
                        If Not Trs Is Nothing Then
                            Dim row As DataRow
                            row = Trs.NewRow()
                            row(0) = "null"
                            'row(1) = "*"
                            row(1) = ""
                            Trs.Rows.InsertAt(row, 0)
                            Trs.AcceptChanges()
                            'Trs.Rows.Add(row)
                        End If
                    End If

                    If Trs Is Nothing Then
                        Tctl.DataSource = Nothing 'Trs1
                    Else
                        Tctl.DataSource = Trs
                    End If

                    Tctl.DisplayMember = Trs.Columns(1).Caption
                    Tctl.ValueMember = Trs.Columns(0).Caption

                    Tctl.SelectedIndex = Tctl.SelectedIndexMOJ

                    If Tctl.SelectedIndexMOJ = -1 Then
                        Tctl.SelectedIndex = 0
                        Tctl.SelectedIndexMOJ = Tctl.SelectedIndex
                    End If

                    If Not ts8 Is Nothing Then
                        Tctl.SelectedValue = ts8
                        If Tctl.SelectedValue Is Nothing Then
                            Tctl.SelectedValue = Trs(0)(0)
                        End If
                    End If




                    ' Tctl.SelectedIndexMOJ = Tctl.SelectedIndex
                    ' Tctl.SelectedTextMOJ = Trs.Columns(1).Caption

                    ' Tctl.DataSource = Trs

                End If
            End If

            Nadjen = False
        Next ctl
    End Sub


    Private Sub RefreshControlsSaEVALOM(ByVal Kontrola As String, ByVal Vrijednost As String) ' za sada samo numberbox-ove

        Dim Nadjen As Boolean = False
        Dim Tstr As String = ""
        On Error Resume Next
        For Each ctl As Control In Controls
            Tstr = ""



            If TypeOf ctl Is NumerBox And Nadjen = False Then
                Dim Tctl As NumerBox = ctl
                If Tctl.Name.ToLower = Kontrola.ToLower Then
                    Nadjen = True
                    DodatnoPostimajKontrolu(Tctl, Vrijednost)
                    Tctl.Text = Vrijednost
                End If

            End If

            If TypeOf ctl Is Label Then
                Dim Tctl As Label = ctl
                If Tctl.Name.ToLower = Kontrola.ToLower Then
                    Nadjen = True
                    DodatnoPostimajKontrolu(Tctl, Vrijednost)
                End If

            End If

            If TypeOf ctl Is DTPicker Then
                Dim Tctl As DTPicker = ctl
                If Tctl.Name.ToLower = Kontrola.ToLower Then
                    Nadjen = True
                    DodatnoPostimajKontrolu(Tctl, Vrijednost)
                End If
            End If


            If TypeOf ctl Is TextBox And Nadjen = False Then
                Dim Tctl As TextBox = ctl
                If Tctl.Name.ToLower = Kontrola.ToLower Then
                    Nadjen = True
                    DodatnoPostimajKontrolu(Tctl, Vrijednost)
                    If Vrijednost.Trim.ToLower = "disable" Or Vrijednost.Trim.ToLower = "enable" Or Vrijednost.Trim.ToLower = "visible" Or Vrijednost.Trim.ToLower = "invisible" Then

                    Else
                        Tctl.Text = Vrijednost
                    End If
                End If
            End If


            If TypeOf ctl Is Button Then
                Dim Tctl As Button = ctl
                If Tctl.Name.ToLower = Kontrola.ToLower Then
                    Nadjen = True
                    DodatnoPostimajKontrolu(Tctl, Vrijednost)
                End If
            End If

            Nadjen = False
        Next ctl
    End Sub




    Sub RefreshGrida(ByVal tCtl As Grid, ByVal Tstr As String)

        Dim Trs3 As New DataTable
        On Error Resume Next
        cls1.NapuniCBLog(" REFRESHGRIDA() : " & tCtl.Name & " , " & Tstr)

        '-------------------- POZICIONIRANJE --------------------
        Dim RA As String = ""
        Dim RA2 As Integer = 0
        RA2 = tCtl.CurrentRow.AbsolutePosition
        If tCtl.RowCount > 0 Then
            row5 = tCtl.CurrentRow
            If tCtl.CurrentRow.Cells(2).Value Is Nothing Then
                RA = 1
            Else
                RA = tCtl.CurrentRow.Cells(2).Value.ToString.Trim
            End If
        End If
        '---------------------------------------------------------

        strPrevParametri = strParametriSvi
        'Tctl.DataSource = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(Tctl.SQL))
        tCtl.zadnjiSQL = SrediSQLsaKolonamaizGrida(cls1.SrediSQLPosljeNextForme(tCtl.SQL, strPrevParametri))
        Trs3 = cls1.ExecSql(tCtl.zadnjiSQL)
        tCtl.GridovDataTejbl = Trs3

        If tCtl.RootTable Is Nothing Then

            cls1.CreateGridColumns(Trs3, tCtl, tCtl.EditColumns)
            cls1.AddConditionalFormatting(tCtl)

        Else

            'If tCtl.RootTable.Columns(1).Caption.Trim = "" Then
            If tCtl.RootTable.Columns.Count < 3 Or tCtl.strMultiGridLayOut = "true" Then
                'tCtl.RootTable = Nothing
                tCtl.ClearItems()
                tCtl.ClearStructure()
                cls1.CreateGridColumns(Trs3, tCtl, tCtl.EditColumns)
                cls1.AddConditionalFormatting(tCtl)
            End If

        End If



        tCtl.SetDataBinding(Trs3, "qwer")


        tCtl.MoveFirst()


        If tCtl.RowCount > 0 Then
            If tCtl.CHECKBOXVISIBLE = "FALSE" Then
                tCtl.RootTable.Columns(1).Visible = False
            Else
                tCtl.RootTable.Columns(1).Visible = True
            End If
        End If


        If tCtl.TOTALROW2 = "FALSE" Then
            tCtl.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False
        Else
            tCtl.TotalRow = Janus.Windows.GridEX.InheritableBoolean.True
        End If

        RucniRadNaGriduzaCheckiranjeRedova(tCtl)

        TempGrid = tCtl

        tCtl.Formatiraj()

        ' LoadLayout() '####

        '-------------------- POZICIONIRANJE --------------------
        If tCtl.RowCount > 0 Then

            Dim c4 As Janus.Windows.GridEX.GridEXColumn = tCtl.RootTable.Columns(2)
            If tCtl.Find(c4, Janus.Data.ConditionOperator.Equal, RA, 0, 1) = False Then
                'tCtl.MoveToRowIndex(row5.RowIndex)
                tCtl.MoveToRowIndex(RA2 - 1)
            End If

            If InStr(Tstr, "movelast") Then
                tCtl.MoveLast()
            End If

            If InStr(Tstr, "movefirst") Then
                tCtl.MoveFirst()
            End If

            If InStr(Tstr, "movetonew") Then
                tCtl.MoveToNewRecord()
            End If

            If InStr(Tstr, "movenext") Then
                tCtl.MoveNext()
            End If

            If InStr(Tstr, "moveprevious") Then
                tCtl.MovePrevious()
            End If

            If InStr(Tstr, "movetoid") Then
                Dim c5 As Janus.Windows.GridEX.GridEXColumn = tCtl.RootTable.Columns("id")
                If tCtl.Find(c5, Janus.Data.ConditionOperator.Equal, Tstr.Replace("refreshmovetoid", ""), 0, 1) = False Then

                End If

                If c5 Is Nothing Then
                    Dim c6 As Janus.Windows.GridEX.GridEXColumn = tCtl.RootTable.Columns(2)
                    If tCtl.Find(c6, Janus.Data.ConditionOperator.Equal, Tstr.Replace("refreshmovetoid", ""), 0, 1) = False Then
                        'tCtl.MoveToNewRecord()
                    End If
                End If

            End If

            tCtl.EnsureVisible()
        End If
        '---------------------------------------------------------


        '-------- pozicioniranje na iducu kolonu
        'tCtl.CurrentColumn = tCtl.RootTable.Columns(TrenutnaKolona.Index)
        'If tCtl.EnterKeyBehavior = EnterKeyBehavior.NextCell Then
        '    tCtl.Focus()
        '    tCtl.CurrentColumn = tCtl.RootTable.Columns(TrenutnaKolona.Index + 1)
        '    tCtl.EditMode = EditMode.EditOn
        '    tCtl.SelectCurrentCellText()

        '    Dim SendKeysArg As String = String.Empty
        '    For r As Integer = 0 To TrenutnaKolona.Index + 1
        '        If tCtl.RootTable.Columns(r).Visible Then SendKeysArg &= "{TAB}"
        '    Next
        '    tCtl.Focus()
        '    Windows.Forms.SendKeys.Send(SendKeysArg & "{F2}")

        'End If
        '----------------------------------------------------------

        If tCtl.strAfterRefresh.Trim <> "" Then
            Rafalno(tCtl.strAfterRefresh.Trim, Nothing, Nothing)
        End If


        PrevediSadrzajGrida(tCtl)

    End Sub

    Sub RucniRadNaGriduzaCheckiranjeRedova(ByVal tCtl As Grid)
        '  ----zbog nemogucnosti da dodam conditionalformat za checked state ... radim to ručno : ----
        On Error Resume Next
        Err.Clear()
        If tCtl.RowCount > 0 Then
            Dim tsss As String = tCtl.CurrentRow.Cells("selectedid").Value
            If Err.Number = 0 Then ' ako postoji formatid kolona ...
                If tCtl.RootTable.Columns(1).Visible Then
                    Dim row As Janus.Windows.GridEX.GridEXRow
                    Dim dataRows() As Janus.Windows.GridEX.GridEXRow
                    dataRows = tCtl.GetDataRows
                    UsaoUrucniRadZacekiranje = True
                    For Each row In dataRows
                        tCtl.MoveToRowIndex(row.RowIndex)
                        If tCtl.CurrentRow.Cells("selectedid").Value.ToString.Trim.ToLower.Contains("1") Then
                            tCtl.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked
                        End If
                    Next
                    UsaoUrucniRadZacekiranje = False
                End If
            End If
        End If
        '----------------------------------------------------------------
    End Sub

    Function RefreshControlSQL(ByVal CtlName As String) As String
        Dim i As Integer
        RefreshControlSQL = ""
        For i = 0 To Cdt.Rows.Count - 1
            If Cdt.Rows(i)("control") = CtlName Then
                RefreshControlSQL = SrediSQL2(Cdt.Rows(i)("sql"))
            End If
        Next
    End Function

    Sub RefreshKontrola()

        Dim Trs3 As New DataTable

        On Error GoTo 0


        PokupiParametre()
        Dim sql As String = ""

        Dim Nadjen As Boolean = False

        For Each ctl As Control In Controls
            sql = ""
            If TypeOf ctl Is CheckBox Then
                Dim Tctl As CheckBox = ctl


            End If
            If TypeOf ctl Is Combo Then
                Dim Tctl As Combo = ctl

                sql = RefreshControlSQL(Tctl.Name.Trim)
                If sql <> "" Then
                    strPrevParametri = strParametriSvi
                    sql = cls1.SrediSQLPosljeNextForme(sql)

                    Dim Trs As New DataTable
                    Trs = cls1.ExecSql(sql)

                    If Tctl.AllowEmpty Then
                        If Not Trs Is Nothing Then
                            Dim row As DataRow
                            row = Trs.NewRow()
                            row(0) = 0
                            'row(1) = "*"
                            row(1) = ""
                            Trs.Rows.InsertAt(row, 0)
                            Trs.AcceptChanges()
                            'Trs.Rows.Add(row)
                        End If
                    End If

                    Tctl.DataSource = Trs




                End If

            End If

            If TypeOf ctl Is DateTimePicker Then
                Dim Tctl As DateTimePicker = ctl

            End If

            If TypeOf ctl Is NumerBox And Nadjen = False Then
                Dim Tctl As NumerBox = ctl

                Nadjen = True
            End If

            If TypeOf ctl Is SearchBox And Nadjen = False Then
                Dim Tctl As SearchBox = ctl
                Nadjen = True
            End If

            If TypeOf ctl Is TextBox And Nadjen = False Then
                Dim Tctl As TextBox = ctl
                Nadjen = True
            End If

            'If TypeOf ctl Is DataGridView Then
            '    Dim Tctl As DataGridView = ctl
            '    sql = RefreshControlSQL(Tctl.Name)
            '    If sql <> "" Then
            '        strPrevParametri = strParametriSvi
            '        sql = cls1.SrediSQLPosljeNextForme(sql)
            '        Tctl.DataSource = cls1.ExecSql(sql)
            '    End If
            'End If


            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                sql = RefreshControlSQL(Tctl.Name.Trim)
                If sql <> "" Then

                    RefreshGrida(Tctl, "")

                    ''-------------------- POZICIONIRANJE --------------------
                    'Dim RA As String = ""
                    'If Tctl.RowCount > 0 Then
                    '    If Tctl.CurrentRow.Cells(2).Value Is Nothing Then
                    '        RA = 1
                    '    Else
                    '        RA = Tctl.CurrentRow.Cells(2).Value.ToString.Trim
                    '    End If
                    'End If
                    ''---------------------------------------------------------

                    'strPrevParametri = strParametriSvi
                    'sql = cls1.SrediSQLPosljeNextForme(sql)
                    ''Tctl.DataSource = cls1.ExecSql(sql)

                    ''Trs3 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(Tctl.SQL))

                    'Trs3 = cls1.ExecSql(sql)


                    'cls1.CreateGridColumns(Trs3, Tctl)
                    'cls1.AddConditionalFormatting(Tctl)
                    'Tctl.SetDataBinding(Trs3, "qwer")
                    '' Tctl.CurrentRow.AbsolutePosition = 2

                    'If Tctl.RowCount > 0 Then
                    '    If Tctl.CHECKBOXVISIBLE = "FALSE" Then
                    '        Tctl.RootTable.Columns(1).Visible = False
                    '    Else
                    '        Tctl.RootTable.Columns(1).Visible = True
                    '    End If
                    'End If


                    'If Tctl.TOTALROW2 = "FALSE" Then
                    '    Tctl.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False
                    'Else
                    '    Tctl.TotalRow = Janus.Windows.GridEX.InheritableBoolean.True
                    'End If

                    ''-------------------- POZICIONIRANJE --------------------
                    'If Tctl.RowCount > 0 Then
                    '    Dim c4 As Janus.Windows.GridEX.GridEXColumn = Tctl.RootTable.Columns(2)
                    '    'c4.Column = GridEX1.RootTable.Columns(2)
                    '    Tctl.Find(c4, Janus.Data.ConditionOperator.Equal, RA, 0, 1)
                    '    Tctl.Focus()
                    '    Tctl.EnsureVisible()
                    'End If
                    ''---------------------------------------------------------

                End If
            End If


            'Me.Refresh()

            Nadjen = False
        Next ctl

    End Sub


    Private Sub DataGridView1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        If UsaoUrucniRadZacekiranje Then Exit Sub
        If UsaouforeachROW Then Exit Sub

        ' RefreshControls(sender.onClickSQL)

        ' cls1.NapuniCBLog(" SelectionChanged() : " & sender.Name & " , onClickSQL=" & sender.onClickSQL)

        If KupimnizAjdijeva = True Then Exit Sub
        'If UsoSamURafal = True Then Exit Sub
        ZanemariClick = True
        If ControlClicked = True Then

        Else
            Rafalno(sender.OnClickSQL, sender, e)
        End If
        'Rafalno(sender.onClickSQL, sender, e) '**
    End Sub

    Private Sub DataGridView1_RowEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        'MsgBox("a")
        'If sender.dataset Is Nothing Then MsgBox("a")

        RefreshControls(sender.onClickSQL)

        'If FormaJeAktivna Then
        '    PokupiParametre()
        '    RefreshKontrola()
        'End If
    End Sub



    Sub ssPermissionProcitaj()
        On Error Resume Next
        Dim sql As String
        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))
        imeXMLa = imeXMLa.Trim

        sql = "select * from sspermission where objectname='" & imeXMLa & "." & strImeForme & "' and operateriid=" & cls1.OperateriID

        ' kk 26.10.2018. izmjenio sort u upitu za permissione tako da bez obzira na deny na grupi ako ipak operater ima prava na nešto onda to ima prednost
        sql = "select *,1 as sort from ssPermission 	where objectname='" & imeXMLa & "." & strImeForme & "' and ((OperateriID = " & cls1.OperateriID & ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " & cls1.OperateriID & ") )) union select *,2 as sort from ssPermission where objectname='" & imeXMLa & "." & strImeForme & "' and OperateriID = 0 and grupeid=0 order by sort desc,isgrupa desc,grupeid,operateriid"

        trsPermisio = cls1.ExecSql(sql)


    End Sub

    Sub ssPermissionProvjeri()
        On Error Resume Next

        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

        If Not trsPermisio Is Nothing Then
            If trsPermisio.Rows.Count > 0 Then
                Dim r As DataRow

                For Each ctl As Control In Controls
                    For Each r In trsPermisio.Rows 'kk 21.02.2022 zamjenio redosljed ove linije i gornje 

                        If r("objectname").ToString.Trim.ToLower = imeXMLa.Trim.ToLower & "." & strImeForme.Trim.ToLower And r("functionname").ToString.Trim.ToLower = ctl.Name.Trim.ToLower Then

                            Err.Clear()
                            If r("Vidljivost") = False Then
                                If Err.Number = 0 Then
                                    ctl.Visible = False
                                    Me.Controls("lbl" & ctl.Name).Visible = False
                                End If
                            End If

                            Err.Clear()
                            If r("Vidljivost") = True Then
                                If Err.Number = 0 Then
                                    ctl.Visible = True
                                    Me.Controls("lbl" & ctl.Name).Visible = True
                                End If
                            End If

                            If r("smije") = False Then
                                ctl.Enabled = False
                            End If

                            If r("smije") = True Then
                                ctl.Enabled = True
                                Me.Controls("lbl" & ctl.Name).Visible = True
                                Exit For 'kk 21.02.2022 ako je bilo koji red true , bilo za osobu ili za grupu izađi van - sve je OK
                            End If

                        End If

                    Next
                Next
            End If
        End If
    End Sub

    Sub ssPermissionProvjeriKontrolu(ByVal strKontrola As String)
        On Error Resume Next
        If Not trsPermisio Is Nothing Then
            If trsPermisio.Rows.Count > 0 Then
                Dim r As DataRow
                For Each r In trsPermisio.Rows

                    If r("functionname").ToString.Trim.ToLower = Me.Controls(strKontrola).Name.Trim.ToLower Then

                        If r("smije") = True Then
                            Me.Controls(strKontrola).Enabled = True
                            Me.Controls(strKontrola).Visible = True
                            Me.Controls("lbl" & Me.Controls(strKontrola).Name).Visible = True
                            Exit For 'kk 21.02.2022 ako je bilo koji red true , bilo za osobu ili za grupu izađi van - sve je OK
                        End If


                        If r("smije") = False Then
                            Me.Controls(strKontrola).Enabled = False
                        End If

                        Err.Clear()
                        If r("Vidljivost") = False Then
                            If Err.Number = 0 Then
                                Me.Controls(strKontrola).Visible = False
                                Me.Controls("lbl" & Me.Controls(strKontrola).Name).Visible = False
                            End If
                        End If

                    End If


                Next
            End If
        End If
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

                        If r("smije") = True Then
                            ssPermissionProvjeriRafalnuKomandu = True
                            Exit For 'kk 21.02.2022 ako je bilo koji red true , bilo za osobu ili za grupu izađi van - sve je OK
                        End If

                    End If


                Next
            End If
        End If
    End Function

    Private Sub Form2_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = 17 Then
            ControlClicked = False
        End If
    End Sub

    Public Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        NapuniKontroleZaEdit()
        Rafalno(sender.stronloaded, sender, e) '**
        ssPermissionProvjeri()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Module1.Sender7 = sender
        NovaGenerickaForma2(sender.tag)
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Module1.Sender7 = sender
        NovaGenerickaForma2(sender.tag)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Module1.Sender7 = sender
        NovaGenerickaForma2(sender.tag)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Module1.Sender7 = sender
        NovaGenerickaForma2(sender.tag)
    End Sub

    Private Sub Glbl_Validated(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        ' izforsirani event za tražilicu ...
        cls1.NapuniCBLog(" EVENT TRAZILICA VALIDATED : " & sender.text.ToString.Trim)

        AktivnaKontola = Me.ActiveControl
        If sender.text.ToString.Trim = "" Then
            sender.tag = "0"
            OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown = True
            RefreshKontrola()
            Rafalno(sender.OnLostFocusSQL, sender, e)
            'RefreshControls(sender.OnLostFocusSQL)

            Exit Sub
        End If

        If OldSearchString = sender.text Then
            If sender.text.trim <> "*" Then
                OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown = True
                RefreshKontrola()
                Rafalno(sender.OnLostFocusSQL, sender, e)
                Exit Sub
            End If
        End If


        PokupiParametre()

        TrazilicaID = "0"
        TrazilicaIDstr = ""

        Form3.OKClick = False

        ' If frm3 Is Nothing Then
        frm3 = New Form3
        frm3.cls1 = cls1
        'End If


        frm3.strXMLfile2 = strXMLfile2
        frm3.strImeForme = strImeForme


        'Form3.strXMLfile2 = strXMLfile2
        'Form3.strImeForme = strImeForme


        frm3.strColumnAutoResize = sender.strColumnAutoResize
        OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown = True

        strParametriIzTrazilice = ""
        frm3.Trazilica(sender.text, ToolTip1.GetToolTip(sender).Substring(ToolTip1.GetToolTip(sender).IndexOf("#") + 1, ToolTip1.GetToolTip(sender).Length - ToolTip1.GetToolTip(sender).IndexOf("#") - 1), sender)

        cls1.NapuniCBLog(" EVENT TRAZILICA VALIDATED - nakon frm3.Trazilica, sql = " & Form3.searchSQL5)
        cls1.NapuniCBLog(" EVENT TRAZILICA VALIDATED - nakon frm3.Trazilica, nađeno: " & Form3.GridRowCount)

        If Form3.GridRowCount = 1 Or Form3.GridRowCount = 0 Then
            'Form3.Odaberi()

        Else
            'Form3.Opacity = 0.9
            frm3.ShowDialog()
        End If

        sender.tag = TrazilicaID
        sender.text = TrazilicaIDstr

        If Form3.OKClick Then
            'sender.tag = Form3.ID.Trim
            'sender.text = Form3.strID.Trim & " (" & Form3.ID & ")"

            sender.tag = TrazilicaID
            sender.text = TrazilicaIDstr

            cls1.NapuniCBLog(" EVENT TRAZILICA VALIDATED - nađen tekst : " & TrazilicaIDstr)

        End If

        Timer1.Enabled = True

        'RefreshControls(sender.OnLostFocusSQL)
        If strParametriIzTrazilice = "" Then PokupiParametreIzTrazilice()


        RefreshKontrola()

        Rafalno(sender.OnLostFocusSQL, sender, e)



    End Sub


    Private Sub Glbl_Validated2(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated

        ' ovaj event bi se trebao odigrati samo u slučaju da to već nije odrađeno na keydown ...

        If OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown Then

        Else
            AktivnaKontola = Me.ActiveControl
            RefreshKontrola()
            Rafalno(sender.OnLostFocusSQL, sender, e)
        End If

    End Sub

    Sub PokupiParametreIzTrazilice()
        On Error Resume Next
        Dim i As Integer
        strParametriIzTrazilice = ""
        If frm3.GridEX1.RowCount > 0 Then
            For i = 1 To frm3.GridEX1.RootTable.Columns.Count - 1
                If frm3.GridEX1.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
                    If frm3.GridEX1.CurrentRow.Cells(i).Value Is Nothing Or frm3.GridEX1.CurrentRow.Cells(i).Value Is DBNull.Value Then

                    Else

                        Dim strImeParametra As String = frm3.GridEX1.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If IsNumeric(frm3.GridEX1.CurrentRow.Cells(i).Value.ToString) Then
                            'strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & frm3.GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                            strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & strImeParametra.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            'strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & frm3.GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString & "' , "
                            strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & strImeParametra.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString & "' , "
                        End If

                    End If
                End If
            Next
        End If
    End Sub



    Private Sub Glbl_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Rafalno(sender.OnChangeSQL, sender, e)
    End Sub

    Private Sub Glbl_ValidatedCombo(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        ' izforsirani event za combo ...
        ' RefreshControls(sender.OnLostFocusSQL)
        Rafalno(sender.OnLostFocusSQL, sender, e)
    End Sub

    Private Sub Glbl_ValidatedMultiDrop(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        ' izforsirani event za combo ...
        ' RefreshControls(sender.OnLostFocusSQL)
        Rafalno(sender.ErrorText, sender, e)
    End Sub




    Private Sub GlblNumber_Validated(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        ' izforsirani event za brojčanu kontrolu ...
        ' RefreshControls(sender.OnLostFocusSQL)

        Rafalno(sender.OnLostFocusSQL, sender, e)

    End Sub

    Private Sub Glbl_Gotfocus(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        OldSearchString = sender.Text
        AktivnaKontola = Me.ActiveControl

        Glbl_Gotfocus2(sender, e)
    End Sub

    Private Sub Glbl_popup(sender As Object, e As EventArgs)

        On Error Resume Next

        'Dim nizajdijeva6 As String = ""
        'nizajdijeva6 = sender.tag.Trim

        'If nizajdijeva6 <> "" Then
        '    sender.tag = ""
        '    nizajdijeva6 = "," & nizajdijeva6.Replace("~", ",") & ","
        '    MsgBox(nizajdijeva6)
        '    ' Tctl.SetEditValue = 
        '    For i8 = 0 To sender.Properties.Items.Count - 1
        '        ' MsgBox(i8)
        '        If nizajdijeva6.IndexOf("," & sender.Properties.Items(i8).Value.ToString.Trim & ",") > 0 Then
        '            sender.Properties.Items(i8).CheckState = CheckState.Checked
        '        End If
        '    Next
        'End If


    End Sub


    Private Sub Glbl_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)

        On Error Resume Next

        OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown = False

        If TypeOf (sender) Is SearchBox And e.KeyCode = Keys.Enter Then
            ' SendKeys.Send("{TAB}")

            Glbl_Validated(sender, e)

            If Not GreskaKodIzvrsavanjaSQLa.Trim.ToLower.Contains("focus(") Then

                If Not sender.OnLostFocusSQL.trim.Tolower.contains("focus") Then
                    Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
                    AktivnaKontola = Me.ActiveControl
                End If

            End If

            Exit Sub

        Else

            If TypeOf (sender) Is SearchBox Then
                OldSearchString = ""
            End If

        End If


        If e.KeyCode = Keys.Enter Then
            If TypeOf (sender) Is Grid Then
                If sender.EditColumns.Trim = "" Then
                    Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
                End If
            ElseIf TypeOf (sender) Is TeksT Then
                If sender.multiline Then
                    ' ništ
                    If sender.enterastab = "true" Then
                        Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
                    End If
                Else
                    Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
                End If
            Else
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
            'SendKeys.Send("{TAB}")
        End If


        Dim iRow As Long = 0

        If TypeOf (sender) Is Grid Then

            If e.KeyCode = Keys.Enter Then
                If sender.OnEnterKey.ToString.Trim <> "" Then
                    Rafalno(sender.OnEnterKey, sender, e)
                End If
            End If

            If e.KeyCode = 17 Then
                ControlClicked = True
            End If

            If e.Control Then
                If e.KeyCode = Keys.A Then
                    'sender.CheckAllRecords()
                    For iRow = 0 To sender.RowCount - 1
                        sender.SelectedItems.Add(iRow)
                    Next
                End If


                If e.KeyCode = Keys.C Then
                    Clipboard.SetText(sender.GetClipString)
                    'Clipboard.SetText(sender.GetClipString)
                End If



            End If
        End If

    End Sub

    Private Sub Glbl_TREE_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs)
        On Error Resume Next
        sender.SelectedNode.BackColor = Color.White
    End Sub

    Private Sub Glbl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' MsgBox("You clicked on : " & CType(sender.tabindex, String))

        On Error Resume Next


        PokupiParametre()

        If TypeOf sender Is Grid Then
            ' sender.SearchColumn = sender.CurrentColumn

            TrenutnaKolona = sender.CurrentColumn

            Err.Clear()
            If sender.strRafalno = "true" Then

            End If
            If sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.ToLower = "true" Or sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.ToLower = "false" Then
                If Err.Number = 0 Or (IsNothing(sender.CurrentColumn)) Then
                    'sender.MovePrevious()
                    'sender.movenext()

                    ' 02.08.2018. kk zakomentirao zbog repetitivnog okidanja sql-a onliko puta koliko ima redova iznad kliknutog reda ?!?!? wtf
                    ' Windows.Forms.SendKeys.Send("+{TAB}")
                    ' Windows.Forms.SendKeys.Send("{TAB}")


                    'cls1.NapuniCBLog("Glbl_Click() : " & sender.OnClickSQL)
                    Rafalno(sender.OnClickSQL, sender, e)
                    Return
                End If
            End If

        End If

        If TypeOf sender Is Button Then

            Rafalno(sender.tag, sender, e)
            Exit Sub
        End If

        If TypeOf sender Is Label Then
            Rafalno(sender.tag, sender, e)
            Exit Sub
        End If


        If TypeOf sender Is SearchBox Then
            sender.selectall()
        End If


        'cls1.NapuniCBLog("Glbl_Click() : " & sender.name & " , " & sender.OnClickSQL)
        If ZanemariClick = True Then
            If TypeOf sender Is Grid Then
                ZanemariClick = False
                Exit Sub
            End If
        End If



        Rafalno(sender.OnClickSQL, sender, e)

        'If sender.OnClickSQL <> "" Then
        '    If sender.OnClickSQL.Substring(0, 1) = ":" Then
        '        NovaGenerickaForma2(sender.OnClickSQL.ToString.Replace(":", ""))
        '    Else
        '        RefreshControls(sender.OnClickSQL)
        '    End If
        'End If

    End Sub

    Private Sub GlblCHECK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        Rafalno(sender.tag.ToString, sender, e)
    End Sub


    Private Sub Glbl_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'MsgBox("You clicked on : " & CType(sender.tabindex, String))
        On Error Resume Next

        DblClickedColumn = sender.CurrentColumn


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


        Rafalno(sender.OnDBLClickSQL, sender, e)

        'If sender.OnDBLClickSQL <> "" Then
        '    If sender.OnDBLClickSQL.Substring(0, 1) = ":" Then
        '        NovaGenerickaForma2(sender.OnDBLClickSQL.ToString.Replace(":", ""))
        '    Else
        '        RefreshControls(sender.OnDBLClickSQL)
        '    End If
        'End If

    End Sub


    Private Sub Glbl_ClickButton(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'On Error Resume Next
        cls1.NapuniCBLog("Glbl_ClickButton event , sender.tag.trim = " & sender.tag.trim)
        If sender.tag.trim = "" Or sender.tag.trim.ToString.ToLower = "help" Then
            DajPokaziHelp()
        Else
            Module1.Sender7 = sender ' xxxxx kkkkkk
            Rafalno(sender.tag, sender, e)
        End If

    End Sub


    Public Sub UpaliProgres()
        ToolStripProgressBar1.Value = 20
        ToolStripProgressBar1.Style = ProgressBarStyle.Marquee
        ToolStripProgressBar1.MarqueeAnimationSpeed = 40
    End Sub

    Public Sub UgasiProgres()
        ToolStripProgressBar1.Style = ProgressBarStyle.Blocks
        ToolStripProgressBar1.MarqueeAnimationSpeed = 0
    End Sub



    Public Sub Fiskalizacija(ByVal mpBlagajnaglaid As String, Optional ByVal TablicaFiskalizacije As Integer = 1)

        ' MsgBox("ušu u fiskalizaciju !!!")

        On Error Resume Next



        Dim trs333 As DataTable
        'trs333 = cls1.ExecSql("select * from mpfiskalizacija")
        If TablicaFiskalizacije = 2 Then ' BOŽO
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 3 Then ' MUT
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 5 Then ' MARTINA
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')")))
        ElseIf TablicaFiskalizacije = 4 Then ' LOGARA
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpponudagla where mpponudaglaid = " & Trim(mpBlagajnaglaid) & " ))")))
        Else ' PEKI
            trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpblagajnagla where mpblagajnaglaid = " & Trim(mpBlagajnaglaid) & " ))")))
        End If



        If trs333 Is Nothing Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!" & vbCr & vbCr)
            MsgBox("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If

        If trs333.Rows.Count = 0 Then
            cls1.NapuniCBLog("Nema ništa u MPFISKALIZACIJA !!!" & vbCr & vbCr)
            MsgBox("Nema ništa u MPFISKALIZACIJA !!!")
            Exit Sub
        End If

        If trs333(0)("datum") > Now Then
            cls1.NapuniCBLog("DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!! NEMA FISKALIZACIJE ! ")
            'MsgBox("DATUM U MPFISKALIZACIJA JE VEĆI OD SADAŠNJEG !!!")
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
            cls1.NapuniCBLog("SLORacun u SLOFISKAL")
            mpBlagajnaglaid = SrediSQL2(mpBlagajnaglaid)
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

        If TablicaFiskalizacije = 2 Then ' BOŽO
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spFaktureXMLGetRacunZahtijev @faktureid=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 3 Then 'MUT
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spBSracuniXMLGetRacunZahtijev @action=2,@bsracuniglaid=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spPonudaGetXMLZahtijev @ProPonGlaId=" & mpBlagajnaglaid)))
        ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spMPXMLGetPDZahtijev @mpponudaglaid=" & mpBlagajnaglaid)))
        Else 'PEKI
            rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spMPXMLGetRacunZahtijev @mpblagajnaglaid=" & mpBlagajnaglaid)))
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
        rsProvjera = cls1.ExecSql(SrediSQL2(sql33))
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
        If TablicaFiskalizacije = 2 Then 'BOŽO
            sql78 = "update fakture set  UUID='" & Uuid & "',KOD='" & zki & "' where faktureid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 3 Then 'MUT
            sql78 = "update BSRacuniGla set  UUID='" & Uuid & "',KOD='" & zki & "' where BSRacuniGlaid=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
            sql78 = "update ProPonGla set  UUID='" & Uuid & "',KOD='" & zki & "' where ProPonGlaId=" & mpBlagajnaglaid
        ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
            sql78 = "update MPPonudagla set  UUID='" & Uuid & "',KOD='" & zki & "' where MPPonudaglaid=" & mpBlagajnaglaid
        Else 'PEKI
            sql78 = "update mpblagajnagla set  UUID='" & Uuid & "',KOD='" & zki & "' where mpblagajnaglaid=" & mpBlagajnaglaid
        End If

        cls1.ExecSql(SrediSQL2(sql78))

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
            ElseIf TablicaFiskalizacije = 4 Then 'LOGARA
                sql78 = "update MPPonudagla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where MPPonudaglaid=" & mpBlagajnaglaid
            ElseIf TablicaFiskalizacije = 5 Then 'MARTINA
                sql78 = "update ProPonGla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where ProPonGlaId=" & mpBlagajnaglaid
            Else ' PEKI
                sql78 = "update mpblagajnagla set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where mpblagajnaglaid=" & mpBlagajnaglaid
            End If

            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql78)))
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

            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql77)))
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
        trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=" & Trim(trgovinaid) & " )")))

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
            trgovinaid = SrediSQL2(trgovinaid)
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

        cls1.NapuniCBLog("NAĐENI PFX : " & strPFX)

        Dim rsFiskal As DataTable
        Err.Clear()

        rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spMPXMLProstorZahtijev @trgovinaid=" & trgovinaid)))

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
        cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql78)))

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
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("GREŠKA KOD POŠTIMAVANJA PARAMETARA COM-a : " & Err.Description)
        End If

        cls1.NapuniCBLog("POZIVAM FISKALIZACIJSKI SERVIS : " & strmojCISURL)

        Err.Clear()

        Odgovor = Fiskal.PosaljiSoapPorukuCertifikatDatoteka(strxml6, MOJremotedllpath & "\Certificates\" & strPFX, strPFXLozinka)

        If Err.Number <> 0 Then
            strGreskaKodSlanjaPoruke = "55555"
            cls1.NapuniCBLog("GREŠKA KOD SLANJA POSLOVNOG PROSTORA : " & Err.Description & vbCr & vbCr & Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
            'MsgBox(Fiskal.DohvatiGresku(Fiskal.OdgovorGreska))
        End If


        If strGreskaKodSlanjaPoruke <> "" Then
            sql78 = "update trgovina set  sifragreske='" & strGreskaKodSlanjaPoruke & "' where trgovinaid=" & trgovinaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql78)))
        End If

        If strGreskaKodSlanjaPoruke <> "" Then
            'MsgBox "Neuspjelo slanje XML-a za fiskalizaciju !" & vbCr & vbCr & Err.Description
        Else
            Err.Clear()
            Dim sql77 As String
            sql77 = "update trgovina set  sifragreske='OK', UUID='" & Uuid & "', datumslanja='" & Date.Now.ToString("yyyy-MM-dd hh:mm:ss") & "' where trgovinaid=" & trgovinaid
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql77)))
            If Err.Number <> 0 Then
                MsgBox(Err.Description)
            End If
        End If

        Err.Clear()

    End Sub

    Sub Rafalno(ByVal stR As String, ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        Dim param As String = ""
        Dim Ar5() As String
        Dim Ar6() As String
        Dim i As Long
        Dim poruka As String = ""
        Dim iducaKontrola As String = ""
        Dim Trs As New DataTable

        If Not IsNothing(sender) Then Module1.Sender7 = sender
        Err.Clear()

        UsoSamURafal = True

        If stR.Trim <> "" Then cls1.NapuniCBLog("Rafalno() : " & stR)

        If stR.ToString.Trim <> "" Then

            Ar5 = Split(stR.ToString, "#")

            For i = 0 To UBound(Ar5)

                If Ar5(i).Trim.Contains("@@") Then ' 30.10.2009. omogucuje programiranje kljucnih rijeci, npr. @@stanjeforme se negdje napuni sa CLOSE ....
                    PokupiParametre()
                    If Ar5(i).Trim.Contains("@SEARCH") Then ' ovo je zbog parametara iz tražilice ... 28.01.2011.
                        Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar5(i)), strParametriSvi & strParametriIzTrazilice)
                    Else
                        Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar5(i)), strParametriSvi)
                    End If
                End If

                If Ar5(i).Trim = "" Then
                    i = i
                ElseIf Ar5(i).Trim.ToUpper = "NULL" Then
                    i = i
                ElseIf Ar5(i).Trim.ToUpper.Contains("BEEP()") Then
                    Microsoft.VisualBasic.Interaction.Beep()
                    System.Threading.Thread.Sleep(300)
                ElseIf Ar5(i).Trim.ToUpper.Contains("DISABLEALLCONTROLS()") Then
                    For Each ctl As Control In Controls
                        ctl.Enabled = False
                    Next
                ElseIf Ar5(i).Trim.ToUpper.Contains("ENABLEALLCONTROLS()") Then
                    For Each ctl As Control In Controls
                        ctl.Enabled = True
                    Next
                ElseIf Ar5(i).Trim.ToUpper.Contains("OPENFORM(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("OPENFORM(", "")
                    param = param.Replace(")", "")
                    NovaGenerickaForma2(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("PROCESSSTART(") Then
                    param = Ar5(i).Trim
                    param = param.Replace("PROCESSSTART(", "")
                    param = param.Replace("processstart(", "")
                    param = param.Replace(")", "")
                    Process.Start(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("SHOWFOLDERBROWSERDIALOG()") Then
                    Dim o As New FolderBrowserDialog
                    o.ShowDialog()
                    strSelectedPath = o.SelectedPath
                ElseIf Ar5(i).Trim.ToUpper.Contains("SHOWOPENFILEDIALOG()") Then
                    Dim o As New OpenFileDialog

                    o.ShowDialog()
                    strSelectedFile = o.FileName
                ElseIf Ar5(i).Trim.ToUpper.Contains("TIMERON(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("TIMERON(", "")
                    param = param.Replace(")", "")
                    Timer2.Interval = param
                    Timer2.Enabled = True
                ElseIf Ar5(i).Trim.ToUpper.Contains("TIMEROFF()") Then
                    Timer2.Enabled = False
                ElseIf Ar5(i).Trim.ToUpper.Contains("PROGRESSON()") Then
                    UpaliProgres()
                ElseIf Ar5(i).Trim.ToUpper.Contains("PROGRESSOFF()") Then
                    UgasiProgres()
                ElseIf Ar5(i).Trim.ToUpper.Contains("BLAGAJNAENABLE(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("BLAGAJNAENABLE(", "")
                    param = param.Replace(")", "")
                    cls1.BlagajnaEnableZaMlinar(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("STARTSEARCH(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("STARTSEARCH(", "")
                    param = param.Replace(")", "")
                    Ar6 = Split(param.ToString, ",")
                    Controls(Ar6(0)).Focus()
                    Controls(Ar6(0)).Select()
                    Controls(Ar6(0)).Text = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar6(1)), strParametriSvi)
                    Glbl_Validated(Controls(Ar6(0)), Nothing)
                ElseIf Ar5(i).Trim.Substring(0, 1) = ":" Then
                    NovaGenerickaForma2(Ar5(i).ToString.Replace(":", ""))
                ElseIf Ar5(i).Trim.ToUpper = "OKSTAY" Then
                    NeZelimClosatFormu = True
                    Button1_Click(sender, e)
                    NeZelimClosatFormu = False
                ElseIf Ar5(i).Trim.ToUpper = "OK" Then
                    NeZelimClosatFormu = False
                    Button1_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper.Contains("POSALJIRACUNENASERVER(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("POSALJIRACUNENASERVER(", "")
                    param = param.Replace(")", "")
                    param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    PosaljiRacunEEEEEnaServer(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("POSALJIZAKLJUCKENASERVER(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("POSALJIZAKLJUCKENASERVER(", "")
                    param = param.Replace(")", "")
                    param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    PosaljiZakljuckEEEEEnaServer(param)

                ElseIf Ar5(i).Trim.ToUpper.Contains("POSALJIPORUKENASERVER(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("POSALJIPORUKENASERVER(", "")
                    param = param.Replace(")", "")
                    param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    PosaljiZPorukeEEEEEnaServer(param)

                ElseIf Ar5(i).Trim.ToUpper.Contains("REFRESHCONTROL(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("REFRESHCONTROL(", "")
                    param = param.Replace(")", "")
                    RefreshControls("select 'refresh' as " & param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("DOCK5(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("DOCK5(", "")
                    param = param.Replace(")", "")
                    Controls(param).Dock = DockStyle.Fill
                ElseIf Ar5(i).Trim.ToUpper.Contains("DOCK3(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("DOCK3(", "")
                    param = param.Replace(")", "")
                    Controls(param).Dock = DockStyle.Left
                ElseIf Ar5(i).Trim.ToUpper.Contains("DOCK2(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("DOCK2(", "")
                    param = param.Replace(")", "")
                    Controls(param).Dock = DockStyle.Bottom
                ElseIf Ar5(i).Trim.ToUpper.Contains("DOCK1(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("DOCK1(", "")
                    param = param.Replace(")", "")
                    Controls(param).Dock = DockStyle.Top
                ElseIf Ar5(i).Trim.ToUpper.Contains("DOCK4(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("DOCK4(", "")
                    param = param.Replace(")", "")
                    Controls(param).Dock = DockStyle.Right
                ElseIf Ar5(i).Trim.ToUpper.Contains("ISEMPTY(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("ISEMPTY(", "")
                    param = param.Replace(")", "")
                    If param.ToLower = "null" Or param.Trim = "" Then
                        i = i + 1
                    Else

                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("PRINTGRID(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("PRINTGRID(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    'Ispis(False, True)
                    ' IspisNovi()
                    IspisGRIDREPX()
                ElseIf Ar5(i).Trim.ToUpper.Contains("RECALCGRID(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("RECALCGRID(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    RecalcGrid()
                ElseIf Ar5(i).Trim.ToUpper.Contains("RECALCGRIDROW(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("RECALCGRIDROW(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    RecalcGridRow()
                ElseIf Ar5(i).Trim.ToUpper.Contains("TOGGLEGRIDGROUP(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("TOGGLEGRIDGROUP(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    'Ispis(False, True)

                    If TempGrid.c = 0 Then
                        TempGrid.ExpandGroups()
                        TempGrid.c = 1
                    Else
                        TempGrid.CollapseGroups()
                        TempGrid.c = 0
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("PRINTREPXGRID(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("PRINTREPXGRID(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    'Ispis(False, True)
                    IspisGRIDREPX()
                ElseIf Ar5(i).Trim.ToUpper.Contains("PIVOTGRID(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("PIVOTGRID(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    'Ispis(False, True)
                    PivotGRID()
                ElseIf Ar5(i).Trim.ToUpper.Contains("CHARTGRID(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CHARTGRID(", "")
                    param = param.Replace(")", "")
                    TempGrid = Controls(param)
                    'Ispis(False, True)
                    ChartGRID()

                ElseIf Ar5(i).Trim.ToUpper.Contains("GRID_COLUMN_VALUE(") Then
                    Dim TempGrid2 As Grid
                    On Error Resume Next
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("GRID_COLUMN_VALUE(", "")
                    param = param.Replace(")", "")
                    PokupiParametre()
                    param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    Dim a() As String
                    a = param.Split(",")
                    TempGrid2 = Controls(a(0))

                    On Error Resume Next
                    If TempGrid2.RowCount > 0 Then
                        Dim c4 As GridEXColumn = TempGrid2.RootTable.Columns(a(1))
                        If Err.Number = 0 Then
                            Err.Clear()
                            TempGrid2.Find(c4, ConditionOperator.Equal, a(2), 0, 1)
                            If Err.Number = 0 Then
                                TempGrid2.EnsureVisible()
                            End If
                        End If
                    End If

                ElseIf Ar5(i).Trim.ToUpper = "CANCEL" Then
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper = "CLOSE" Then
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper = "CLOSEPARENT" Then
                    cls1.bCloseParent = True
                    Button2_Click(sender, e)
                ElseIf Ar5(i).Trim.ToUpper.Contains("PREPAREMULTISELECTEDPRINT(") Then
                    MultiSelectedPrintPriprema(Ar5(i))
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHSELECTED(") Then
                    ForEachSelected(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHBLUEROW(") Then
                    ForEachBlueRow(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHROW(") Then
                    ForEachRow(Ar5(i), Ar5(i + 1), sender, e)
                    i = i + 1
                ElseIf Ar5(i).Trim.ToUpper.Contains("MASTERMOVENEXT") Then
                    MasterMoveNext = True
                ElseIf Ar5(i).Trim.ToUpper.Contains("MASTERMOVEPREVIOUS") Then
                    MasterMovePreviuos = True
                ElseIf Ar5(i).Trim.ToUpper.Contains("ISEMPTYORZEROALERT(") Then
                    PokupiParametre()
                    param = Ar5(i).Trim
                    param = param.Replace("ISEMPTYORZEROALERT(", "")
                    param = param.Replace(")", "")


                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    End If


                    If param.StartsWith("@") Then
                        param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    Else
                        If param.Length > 6 And (param.ToLower.StartsWith("sp") Or param.ToLower.StartsWith("select")) Then
                            Err.Clear()
                            Trs = cls1.ExecSql(param)
                            If Err.Number = 0 Then
                                If Trs.Rows.Count > 0 Then
                                    param = Trs.Rows(0)(0).ToString
                                End If
                            End If
                        End If

                    End If


                    If param.ToLower = "null" Or param.Trim = "" Or param.Trim = "0" Then
                        If MsgBox(poruka, MsgBoxStyle.OkCancel, "Pitanje") = MsgBoxResult.Ok Then

                        Else
                            Exit For
                        End If
                    End If
                ElseIf Ar5(i).Trim.ToUpper.Contains("EVAL(") Then
                    PokupiParametre()
                    param = Ar5(i).Trim
                    param = param.Replace("EVAL(", "")
                    param = param.Replace("eval(", "")
                    'param = param.Replace(")", "")
                    param = param.Trim
                    param = param.Substring(0, param.Length - 1)

                    poruka = ""
                    If param.Contains("=") Then
                        poruka = param.Substring(InStrRev(param, "="), param.Length - InStrRev(param, "="))
                        param = param.Substring(0, InStrRev(param, "=") - 1)
                    End If


                    If poruka.Contains("@") Then
                        poruka = cls1.SrediSQLPosljeNextForme(SrediSQL2(poruka), strParametriSvi)
                    End If

                    cls1.NapuniCBLog("KRECEM U EVALUATE :   " & param & " = " & poruka)

                    RefreshControlsSaEVALOM(param, EvaluateSTR(poruka, param))

                ElseIf Ar5(i).Trim.ToUpper.Contains("ISEMPTYORZEROCANCELBOX(") Then
                    PokupiParametre()
                    param = Ar5(i).Trim

                    param = param.Replace("ISEMPTYORZEROCANCELBOX(", "")
                    param = param.Replace(")", "")


                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    Else
                        Exit For
                    End If

                    If param.StartsWith("@") Then
                        iducaKontrola = param.Replace("@", "")
                        param = cls1.SrediSQLPosljeNextForme(SrediSQL2(param), strParametriSvi)
                    Else
                        Err.Clear()
                        Trs = cls1.ExecSql(param)
                        If Err.Number = 0 Then
                            If Trs.Rows.Count > 0 Then
                                param = Trs.Rows(0)(0).ToString
                            End If
                        End If
                    End If


                    If param.ToLower = "null" Or param.Trim = "" Or param.Trim = "0" Then
                        MsgBox(poruka, MsgBoxStyle.Critical, "Upozorenje")
                        AktivnaKontola = Controls(iducaKontrola)
                        Controls(iducaKontrola).Focus()
                        Exit For
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("CLICK(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CLICK(", "")
                    param = param.Replace(")", "")
                    Glbl_Click(Controls(param), e)
                ElseIf Ar5(i).Trim.ToUpper = "COLORPICKER()" Then

                    'strIzabranaBoja = ""

                    If ColorDialog1.ShowDialog() = 1 Then
                        strIzabranaBoja = ColorDialog1.Color.Name
                        strIzabranaBoja = System.Drawing.ColorTranslator.ToHtml(ColorDialog1.Color)
                    End If

                ElseIf Ar5(i).Trim.ToUpper.Contains("MSGBOX(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("MSGBOX(", "")
                    param = param.Replace(")", "")
                    MsgBox(param, MsgBoxStyle.Information)
                ElseIf Ar5(i).Trim.ToUpper.Contains("CREATETEXTFILE1(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CREATETEXTFILE1(", "")
                    param = param.Replace(")", "")

                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    Else
                        Exit For
                    End If






                    Dim Trs2 As New DataTable
                    Dim Vrijednost5 As String
                    cls1.NapuniCBLog("CREATETEXTFILE1  " & param & "    " & poruka)
                    Trs2 = cls1.ExecSql(poruka)
                    If Trs2 Is Nothing Then
                        My.Computer.FileSystem.DeleteFile(param, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Else

                        Dim fs As New FileStream(param, FileMode.Create, FileAccess.Write)
                        Dim s As New StreamWriter(fs, System.Text.Encoding.ASCII)
                        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub
                        If Trs2.Rows.Count > 0 Then
                            Vrijednost5 = ""
                            Dim r As DataRow

                            For Each r In Trs2.Rows
                                For Each Column In Trs2.Columns
                                    Err.Clear()
                                    Vrijednost5 = Vrijednost5 & r(Column.Caption)
                                    If Err.Number <> 0 Then
                                        cls1.NapuniCBLog("Greška kod prenosa podataka u txt file : " & Err.Description)
                                        Exit Sub
                                    End If
                                Next
                                s.WriteLine(Vrijednost5)
                                Vrijednost5 = ""
                            Next
                        End If
                        s.Write("")
                        s.Close()

                    End If


                ElseIf Ar5(i).Trim.ToUpper.Contains("CREATETEXTFILE2(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CREATETEXTFILE2(", "")
                    param = param.Replace(")", "")

                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    Else
                        Exit For
                    End If


                    Dim Trs2 As New DataTable
                    Dim Vrijednost5 As String
                    cls1.NapuniCBLog("CREATETEXTFILE2  " & param & "    " & poruka)

                    Dim a() As String
                    a = poruka.Split("~")
                    Dim j As Integer

                    My.Computer.FileSystem.DeleteFile(param, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Dim fs As New FileStream(param, FileMode.Create, FileAccess.Write)
                    Dim s As New StreamWriter(fs, System.Text.Encoding.ASCII)

                    For j = 0 To a.GetUpperBound(0)

                        Trs2 = Nothing

                        Trs2 = cls1.ExecSql(a(j))


                        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub
                        If Trs2.Rows.Count > 0 Then
                            Vrijednost5 = ""
                            Dim r As DataRow

                            For Each r In Trs2.Rows
                                For Each Column In Trs2.Columns
                                    Err.Clear()
                                    Vrijednost5 = Vrijednost5 & r(Column.Caption)
                                    If Err.Number <> 0 Then
                                        cls1.NapuniCBLog("Greška kod prenosa podataka u txt file : " & Err.Description)
                                        Exit Sub
                                    End If
                                Next
                                s.WriteLine(Vrijednost5)
                                Vrijednost5 = ""
                            Next
                        End If

                    Next



                    ' s.Write("")
                    s.Close()

                ElseIf Ar5(i).Trim.ToUpper.Contains("CREATETEXTFILE3(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CREATETEXTFILE3(", "")
                    param = param.Replace(")", "")

                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    Else
                        Exit For
                    End If


                    Dim Trs2 As New DataTable
                    Dim Vrijednost5 As String
                    cls1.NapuniCBLog("CREATETEXTFILE3  " & param & "    " & poruka)

                    Dim a() As String
                    a = poruka.Split("~")
                    Dim j As Integer

                    My.Computer.FileSystem.DeleteFile(param, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Dim fs As New FileStream(param, FileMode.Create, FileAccess.Write)
                    Dim s As New StreamWriter(fs, System.Text.Encoding.Default)

                    For j = 0 To a.GetUpperBound(0)

                        Trs2 = Nothing

                        Trs2 = cls1.ExecSql(a(j))


                        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub
                        If Trs2.Rows.Count > 0 Then
                            Vrijednost5 = ""
                            Dim r As DataRow

                            For Each r In Trs2.Rows
                                For Each Column In Trs2.Columns
                                    Err.Clear()
                                    Vrijednost5 = Vrijednost5 & r(Column.Caption)
                                    If Err.Number <> 0 Then
                                        cls1.NapuniCBLog("Greška kod prenosa podataka u txt file : " & Err.Description)
                                        Exit Sub
                                    End If
                                Next
                                s.WriteLine(Vrijednost5)
                                Vrijednost5 = ""
                            Next
                        End If

                    Next



                    ' s.Write("")
                    s.Close()



                ElseIf Ar5(i).Trim.ToUpper.Contains("CREATETEXTFILEUTF8(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CREATETEXTFILEUTF8(", "")
                    param = param.Replace(")", "")

                    poruka = ""
                    If param.Contains(";") Then
                        poruka = param.Substring(InStrRev(param, ";"), param.Length - InStrRev(param, ";"))
                        param = param.Substring(0, InStrRev(param, ";") - 1)
                    Else
                        Exit For
                    End If


                    Dim Trs2 As New DataTable
                    Dim Vrijednost5 As String
                    cls1.NapuniCBLog("CREATETEXTFILEUTF8  " & param & "    " & poruka)

                    Dim a() As String
                    a = poruka.Split("~")
                    Dim j As Integer

                    My.Computer.FileSystem.DeleteFile(param, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    Dim fs As New FileStream(param, FileMode.Create, FileAccess.Write)
                    Dim s As New StreamWriter(fs, System.Text.Encoding.UTF8)

                    For j = 0 To a.GetUpperBound(0)

                        Trs2 = Nothing

                        Trs2 = cls1.ExecSql(a(j))


                        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then Exit Sub
                        If Trs2.Rows.Count > 0 Then
                            Vrijednost5 = ""
                            Dim r As DataRow

                            For Each r In Trs2.Rows
                                For Each Column In Trs2.Columns
                                    Err.Clear()
                                    Vrijednost5 = Vrijednost5 & r(Column.Caption)
                                    If Err.Number <> 0 Then
                                        cls1.NapuniCBLog("Greška kod prenosa podataka u txt file : " & Err.Description)
                                        Exit Sub
                                    End If
                                Next
                                s.WriteLine(Vrijednost5)
                                Vrijednost5 = ""
                            Next
                        End If

                    Next



                    ' s.Write("")
                    s.Close()


                ElseIf Ar5(i).Trim.ToUpper.Contains(".VBS") Then
                    Dim Ar7() As String
                    Dim ii As Integer
                    Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar5(i)), strParametriSvi)
                    Ar7 = Split(Ar5(i).ToString, "~")
                    cls1.NapuniCBLog("poziv VBS : " & Ar5(i).Trim)
                    If UBound(Ar7) = 0 Then System.Diagnostics.Process.Start(Ar7(0))
                    If UBound(Ar7) = 1 Then System.Diagnostics.Process.Start(Ar7(0), Ar7(1))

                ElseIf Ar5(i).Trim.ToUpper.Contains(".BAT") Then
                    Dim Ar7() As String
                    Dim ii As Integer
                    Ar5(i) = cls1.SrediSQLPosljeNextForme(SrediSQL2(Ar5(i)), strParametriSvi)
                    Ar7 = Split(Ar5(i).ToString, "~")
                    cls1.NapuniCBLog("poziv BAT : " & Ar5(i).Trim)
                    If UBound(Ar7) = 0 Then System.Diagnostics.Process.Start(Ar7(0))
                    If UBound(Ar7) = 1 Then System.Diagnostics.Process.Start(Ar7(0), Ar7(1))
                ElseIf Ar5(i).Trim.ToUpper.Contains("CREATEOBJECT(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("CREATEOBJECT(", "")
                    param = param.Replace(")", "")
                    MsgBox(param, MsgBoxStyle.Information)
                ElseIf Ar5(i).Trim.ToUpper.Contains("ASSEMBLYLOAD(") Then

                    param = Ar5(i).ToUpper.Trim
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

                    PokupiParametre()


                    Dim tconstr As String = Conn.ConnectionString.ToString
                    If tconstr.ToLower.Contains("driver") Then

                    Else
                        tconstr = "Driver={SQL Server};" & tconstr
                    End If

                    o.totalgen(ID, cls1.OperateriID.ToString, tconstr, strParametriSvi.ToString, strParametriSvi.ToString)


                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJA(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJA(", "")
                    param = param.Replace(")", "")
                    Fiskalizacija(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAN(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAN(", "")
                    param = param.Replace(")", "")
                    FiskalizacijaNapojnica(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAFAKTURE(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAFAKTURE(", "")
                    param = param.Replace(")", "")
                    Fiskalizacija(param, 2)
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJABSRACUNI(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJABSRACUNI(", "")
                    param = param.Replace(")", "")
                    Fiskalizacija(param, 3)
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAPD(") Then ' LOGARA 02.04.2020.
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAPD(", "")
                    param = param.Replace(")", "")
                    Fiskalizacija(param, 4)
                ElseIf Ar5(i).Trim.ToUpper.Contains("FISKALIZACIJAPONUDE(") Then ' MARTINA 07.04.2020.
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("FISKALIZACIJAPONUDE(", "")
                    param = param.Replace(")", "")
                    Fiskalizacija(param, 5)
                ElseIf Ar5(i).Trim.ToUpper.Contains("POSLOVNIPROSTOR(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("POSLOVNIPROSTOR(", "")
                    param = param.Replace(")", "")
                    PoslovniProstor(param)
                ElseIf Ar5(i).Trim.ToUpper.Contains("SLOPP(") Then
                    param = Ar5(i).ToUpper.Trim
                    param = param.Replace("SLOPP(", "")
                    param = param.Replace(")", "")
                    'PoslovniProstorSLO(param)
                    'ElseIf Ar5(i).Trim.ToUpper.Contains("IDINAWEBSERVICEPOBROJBODOVA(") Then  KKKKKKKKKKKKK
                    '    param = Ar5(i).ToUpper.Trim
                    '    param = param.Replace("IDINAWEBSERVICEPOBROJBODOVA(", "")
                    '    param = param.Replace(")", "")
                    '    cls1.IdiNaWebServicePoBrojBodova(param)
                    'ElseIf Ar5(i).Trim.ToUpper.Contains("IDINAWEBSERVICEIAZURIRAJBODOVE(") Then
                    '    param = Ar5(i).ToUpper.Trim
                    '    param = param.Replace("IDINAWEBSERVICEIAZURIRAJBODOVE(", "")
                    '    param = param.Replace(")", "")
                    '    cls1.AzurirajBodove(param)
                Else
                    RefreshControls(Ar5(i))
                    If GreskaKodIzvrsavanjaSQLa <> "" Then
                        If ZelimIzaciIzForme = True Then
                            Me.Close()
                        End If
                        Exit For
                    End If

                End If
            Next
        End If

        UsoSamURafal = False
    End Sub



    Public Sub FiskalizacijaNapojnica(ByVal mpBlagajnaglaid As String, Optional ByVal TablicaFiskalizacije As Integer = 1)

        On Error Resume Next



        Dim trs333 As DataTable

        trs333 = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=( select trgovinaid from mpblagajnagla where mpblagajnaglaid = " & Trim(mpBlagajnaglaid) & "  ))")))


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

        rsFiskal = cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2("spMPXMLGetNapojnicaZahtijev @mpblagajnaglaid=" & mpBlagajnaglaid)))

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
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql78)))

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
            cls1.ExecSql(cls1.SrediSQLPosljeNextForme(SrediSQL2(sql77)))
        End If

        Err.Clear()

    End Sub



    Public strWebService As String = ""
    Public mjesif As String = ""
    Public kasa As String = ""


    Sub PosaljiZPorukeEEEEEnaServer(sql As String)
        On Error Resume Next
        Dim trs0 As DataTable = cls1.ExecSql("select * from install")
        If IsNothing(trs0) Then Exit Sub
        strWebService = trs0(0)("webservice")
        Dim trs1 As DataTable = cls1.ExecSql("select * from mpkase")
        If IsNothing(trs1) Then Exit Sub
        mjesif = UCase(trs1(0)("oznakatrgovine"))
        kasa = UCase(trs1(0)("kasa"))
        If Trim(strWebService) = "" Then
            Exit Sub
        End If
        Dim trs As DataTable = cls1.ExecSql(sql)
        If IsNothing(trs) Then Exit Sub
        Dim dr As DataRow
        For Each dr In trs.Rows
            PosaljiPorukunaServer(dr("brojdok"), dr("godina"))
        Next
    End Sub




    Sub PosaljiPorukunaServer(brojdokumenta As String, godina As String)
        On Error Resume Next
        If Trim(strWebService) = "" Then
            Exit Sub
        End If
        Dim postData As String = "<?xml version=""1.0"" encoding=""ISO-8859-2""?><xml><broj>5</broj></xml>"
        Me.Refresh()
        Dim trs As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=9, @brojdok='" & brojdokumenta & "' ")
        If IsNothing(trs) Then Exit Sub
        Dim url2 As String = Trim(strWebService) & "xmlrec.asp?a=1&v=3&g=5555555&oznakatrgovine=" & mjesif & "&kasa=" & kasa & "&godina=" & godina & "&vrijemejir=null&jir=null&brojdok=" & brojdokumenta & "&datumdok=" & trs(0)("datumdok1")

        cls1.NapuniCBLog("Šaljem na WS : " & url2)

        Dim writer As New System.IO.StringWriter
        trs.WriteXml(writer, False)
        Console.WriteLine(writer.ToString())
        postData = "<poruka>" & writer.ToString().Replace("NewDataSet", "stavke") & "</poruka>"
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(postData)
        Dim req = DirectCast(WebRequest.Create(url2), HttpWebRequest)
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        req.ContentType = "text/xml"
        req.Method = "POST"
        req.ContentLength = bytes.Length
        req.Timeout = 3000
        Using os As Stream = req.GetRequestStream()
            os.Write(bytes, 0, bytes.Length)
        End Using
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        Dim response As String = ""
        Using resp As System.Net.WebResponse = req.GetResponse()
            Using sr As New StreamReader(resp.GetResponseStream())
                response = sr.ReadToEnd().Trim()
            End Using
        End Using



        If response <> "" Then
            If response <> "0" Then
                cls1.ExecSql("update mpporuke set MP_XML_ID=" & response & " where mpporukejupiterdetid=" & brojdokumenta)
            End If
        End If
    End Sub




    Sub PosaljiZakljuckEEEEEnaServer(sql As String)
        On Error Resume Next

        Dim trs0 As DataTable = cls1.ExecSql("select * from install")

        If IsNothing(trs0) Then Exit Sub

        strWebService = trs0(0)("webservice")


        Dim trs1 As DataTable = cls1.ExecSql("select * from mpkase")

        If IsNothing(trs1) Then Exit Sub

        mjesif = UCase(trs1(0)("oznakatrgovine"))
        kasa = UCase(trs1(0)("kasa"))


        If Trim(strWebService) = "" Then
            Exit Sub
        End If

        Dim trs As DataTable = cls1.ExecSql(sql)
        If IsNothing(trs) Then Exit Sub

        If ProvjeraDostupnosti(strWebService) = True Then
            Dim dr As DataRow
            For Each dr In trs.Rows
                PosaljiZakljucaknaServer(dr("brojdok"), dr("godina"))
            Next
        Else
            cls1.NapuniCBLog("Web service nedostupan : " & strWebService)

        End If


    End Sub




    Sub PosaljiZakljucaknaServer(brojdokumenta As String, godina As String)
        On Error Resume Next
        If Trim(strWebService) = "" Then
            Exit Sub
        End If
        Dim postData As String = "<?xml version=""1.0"" encoding=""ISO-8859-2""?><xml><broj>5</broj></xml>"
        Me.Refresh()
        Dim trs As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=4, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")
        If IsNothing(trs) Then Exit Sub
        Dim url2 As String = Trim(strWebService) & "xmlrec.asp?a=1&v=2&g=5555555&oznakatrgovine=" & mjesif & "&kasa=" & kasa & "&godina=" & godina & "&vrijemejir=null&jir=null&brojdok=" & brojdokumenta & "&datumdok=" & trs(0)("datumdok1")

        cls1.NapuniCBLog("Šaljem na WS : " & url2)

        Dim writer As New System.IO.StringWriter
        trs.WriteXml(writer, False)
        Console.WriteLine(writer.ToString())
        postData = "<zakljucak>" & writer.ToString().Replace("NewDataSet", "stavke") & "</zakljucak>"
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(postData)
        Dim req = DirectCast(WebRequest.Create(url2), HttpWebRequest)
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        req.ContentType = "text/xml"
        req.Method = "POST"
        req.ContentLength = bytes.Length
        req.Timeout = 3000
        Using os As Stream = req.GetRequestStream()
            os.Write(bytes, 0, bytes.Length)
        End Using
        If Err.Number <> 0 Then
        End If
        Err.Clear()
        Dim response As String = ""
        Using resp As System.Net.WebResponse = req.GetResponse()
            Using sr As New StreamReader(resp.GetResponseStream())
                response = sr.ReadToEnd().Trim()
            End Using
        End Using


        If response = "null" Or IsNumeric(response) Then
            If response <> "" Then
                If response <> "0" Then
                    cls1.ExecSql("update zakblag set MP_XML_ID=" & response & " where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")
                End If
            End If
        Else
            SpremiResponse(response, brojdokumenta, godina)
        End If

    End Sub

    Sub SpremiResponse(response As String, brojdokumenta As String, godina As String)
        On Error Resume Next
        Dim FILE_NAME As String = Directory.GetCurrentDirectory() & "\PosaljiZakljucaknaServerResponse.txt"
        Dim objWriter As New System.IO.StreamWriter(FILE_NAME, False)
        objWriter.WriteLine("Response od servera , brojdokumenta: " + brojdokumenta + ", godina" + godina)
        objWriter.WriteLine(response)
        objWriter.Close()
    End Sub

    Sub SpremiRacunResponse(response As String, brojdokumenta As String, godina As String)
        On Error Resume Next
        Dim FILE_NAME As String = Directory.GetCurrentDirectory() & "\PosaljiRacunnaServerResponse.txt"
        Dim objWriter As New System.IO.StreamWriter(FILE_NAME, False)
        objWriter.WriteLine("Response od servera , brojdokumenta: " + brojdokumenta + ", godina" + godina)
        objWriter.WriteLine(response)
        objWriter.Close()
    End Sub


    Function ProvjeraDostupnosti(ByVal strWebServis As String) As Boolean
        Dim uri As New Uri(strWebServis)
        Dim rootUrl As String
        Dim dostupno As Boolean
        dostupno = False
        rootUrl = uri.GetLeftPart(UriPartial.Authority)
        Try
            Dim client As New WebClient()
            client.DownloadData(rootUrl)
            dostupno = True
        Catch ex As Exception
            dostupno = False
        End Try
        Return dostupno
    End Function


    Sub PosaljiRacunEEEEEnaServer(sql As String)
        On Error Resume Next

        Dim trs0 As DataTable = cls1.ExecSql("select * from install")

        If IsNothing(trs0) Then Exit Sub

        strWebService = trs0(0)("webservice")


        Dim trs1 As DataTable = cls1.ExecSql("select * from mpkase")

        If IsNothing(trs1) Then Exit Sub

        mjesif = UCase(trs1(0)("oznakatrgovine"))
        kasa = UCase(trs1(0)("kasa"))


        If Trim(strWebService) = "" Then
            Exit Sub
        End If

        Dim trs As DataTable = cls1.ExecSql(sql)
        If IsNothing(trs) Then Exit Sub

        If ProvjeraDostupnosti(strWebService) = True Then
            Dim dr As DataRow
            For Each dr In trs.Rows
                PosaljiRacunnaServer(dr("brojdok"), dr("godina"))
            Next
        Else

            cls1.NapuniCBLog("Web service nedostupan : " & strWebService)
        End If


    End Sub

    Sub PosaljiRacunnaServer(brojdokumenta As String, godina As String)
        On Error Resume Next

        If Trim(strWebService) = "" Then
            Exit Sub
        End If

        Dim postData As String = "<?xml version=""1.0"" encoding=""ISO-8859-2""?><xml><broj>5</broj></xml>"

        Me.Refresh()

        'http://www.jupiter-software.com/POSBlagajna/Service/XMLrec.asp?OznakaTrgovine=100&kasa=200&brojdok=5&godina=2017

        'strWebService = "http://www.jupiter-software.com/POSBlagajna/Service/"

        'strSpremiLog = strSpremiLog & Now() & " START SLANJA RACUNA " & brojdokumenta & Environment.NewLine


        'Dim trs As DataTable = ExecSql("select * from promet where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")

        Dim trs As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=2, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")


        If IsNothing(trs) Then Exit Sub

        Dim vrijemejir As String = ""
        vrijemejir = trs(0)("vrijemejir1")
        If vrijemejir = "" Then vrijemejir = "null" Else vrijemejir = "'" & vrijemejir & "'"


        Dim jir As String = ""
        jir = trs(0)("jir")
        If jir = "" Then jir = "null" Else jir = "'" & jir & "'"

        Dim url2 As String = Trim(strWebService) & "xmlrec.asp?a=1&v=1&g=5555555&oznakatrgovine=" & mjesif & "&kasa=" & kasa & "&godina=" & godina & "&brojdok=" & brojdokumenta & "&vrijemejir=" & vrijemejir & "&jir=" & jir & "&datumdok=" & trs(0)("datumdok1")


        ' Dim trs2 As DataTable = ExecSql("select * from mpnaplata where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")

        Dim trs2 As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=3, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")

        Dim trs7 As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=7, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")


        'If IsNothing(trs2) Then Exit Sub

        'Dim ds As New DataSet

        'ds.Tables.Add(trs)
        'ds.Tables.Add(trs2)

        'Dim writer3 As New System.IO.StringWriter
        'ds.WriteXml(writer3, XmlWriteMode.WriteSchema)

        Dim writer As New System.IO.StringWriter
        Dim writer2 As New System.IO.StringWriter
        Dim writer7 As New System.IO.StringWriter

        trs.WriteXml(writer, False)
        trs2.WriteXml(writer2, False)
        trs7.WriteXml(writer7, False)

        'ds.WriteXml(writer, False)
        Console.WriteLine(writer.ToString())

        postData = "<doc>" & writer.ToString().Replace("NewDataSet", "stavke") & writer2.ToString().Replace("NewDataSet", "naplata") & writer7.ToString().Replace("NewDataSet", "bon") & "</doc>"

        If Err.Number <> 0 Then

        End If
        Err.Clear()

        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(postData)

        Dim req = DirectCast(WebRequest.Create(url2), HttpWebRequest)

        If Err.Number <> 0 Then

        End If
        Err.Clear()

        req.ContentType = "text/xml"
        req.Method = "POST"
        req.ContentLength = bytes.Length
        req.Timeout = 3000

        Using os As Stream = req.GetRequestStream()
            os.Write(bytes, 0, bytes.Length)
        End Using

        If Err.Number <> 0 Then

        End If

        Err.Clear()

        Dim response As String = ""

        Using resp As System.Net.WebResponse = req.GetResponse()
            Using sr As New StreamReader(resp.GetResponseStream())
                response = sr.ReadToEnd().Trim()
            End Using
        End Using

        If response = "null" Or IsNumeric(response) Then
            If response <> "" Then
                If response <> "0" Then
                    cls1.ExecSql("update promet set MP_XML_ID=" & response & " where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")
                End If
            End If
        Else
            SpremiRacunResponse(response, brojdokumenta, godina)
        End If

        'strSpremiLog = strSpremiLog & Now() & " RESPONSE SA SERVERA : " & response & Environment.NewLine


        'strSpremiLog = strSpremiLog & Now() & " KRAJ SLANJA RACUNA " & brojdokumenta & Environment.NewLine


    End Sub


    Sub PosaljiSamoJIRServer(brojdokumenta As String, godina As String)
        On Error Resume Next

        If Trim(strWebService) = "" Then
            Exit Sub
        End If

        Dim postData As String = "<doc></doc>"
        Me.Refresh()

        'http://www.jupiter-software.com/POSBlagajna/Service/XMLrec.asp?OznakaTrgovine=100&kasa=200&brojdok=5&godina=2017

        'strWebService = "http://www.jupiter-software.com/POSBlagajna/Service/"

        '        strSpremiLog = strSpremiLog & Now() & " START SLANJA RACUNA " & brojdokumenta & Environment.NewLine

        'Dim trs As DataTable = ExecSql("select * from promet where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")

        Dim trs As DataTable = cls1.ExecSql("spmpDajRacuneWS @action=2, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")

        If IsNothing(trs) Then Exit Sub
        Dim vrijemejir As String = ""
        vrijemejir = trs(0)("vrijemejir1")
        If vrijemejir = "" Then vrijemejir = "null" Else vrijemejir = "'" & vrijemejir & "'"
        Dim jir As String = ""
        jir = trs(0)("jir")
        If jir = "" Then jir = "null" Else jir = "'" & jir & "'"
        Dim url2 As String = Trim(strWebService) & "xmlrec.asp?a=2&v=1&g=5555555&oznakatrgovine=" & mjesif & "&kasa=" & kasa & "&godina=" & godina & "&brojdok=" & brojdokumenta & "&vrijemejir=" & vrijemejir & "&jir=" & jir & "&datumdok=" & trs(0)("datumdok1")

        ' Dim trs2 As DataTable = ExecSql("select * from mpnaplata where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")
        ' Dim trs2 As DataTable = ExecSql("spmpDajRacuneWS @action=3, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")
        ' Dim trs7 As DataTable = ExecSql("spmpDajRacuneWS @action=7, @godina='" & godina & "', @brojdok='" & brojdokumenta & "' ")

        'If IsNothing(trs2) Then Exit Sub
        'Dim ds As New DataSet
        'ds.Tables.Add(trs)
        'ds.Tables.Add(trs2)

        'Dim writer3 As New System.IO.StringWriter
        'ds.WriteXml(writer3, XmlWriteMode.WriteSchema)

        Dim writer As New System.IO.StringWriter

        'Dim writer2 As New System.IO.StringWriter

        'Dim writer7 As New System.IO.StringWriter
        'trs.WriteXml(writer, False)
        'trs2.WriteXml(writer2, False)
        'trs7.WriteXml(writer7, False)
        'ds.WriteXml(writer, False)

        Console.WriteLine(writer.ToString())
        ' postData = "<doc>" & writer.ToString().Replace("NewDataSet", "stavke") & writer2.ToString().Replace("NewDataSet", "naplata") & writer7.ToString().Replace("NewDataSet", "bon") & "</doc>"
        If Err.Number <> 0 Then

        End If

        Err.Clear()

        Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(postData)
        Dim req = DirectCast(WebRequest.Create(url2), HttpWebRequest)

        If Err.Number <> 0 Then

        End If

        Err.Clear()
        req.ContentType = "text/xml"
        req.Method = "POST"
        req.ContentLength = bytes.Length
        req.Timeout = 3000

        Using os As Stream = req.GetRequestStream()
            os.Write(bytes, 0, bytes.Length)
        End Using

        If Err.Number <> 0 Then

        End If

        Err.Clear()
        Dim response As String = ""

        Using resp As System.Net.WebResponse = req.GetResponse()
            Using sr As New StreamReader(resp.GetResponseStream())
                response = sr.ReadToEnd().Trim()
            End Using
        End Using



        'If response <> "" Then

        '    If response <> "0" Then

        '        ExecSql("update promet set MP_XML_ID=" & response & " where godina='" & godina & "' and  brojdok='" & brojdokumenta & "' ")

        '    End If

        'End If



        '        strSpremiLog = strSpremiLog & Now() & " RESPONSE SA SERVERA : " & response & Environment.NewLine





        'strSpremiLog = strSpremiLog & Now() & " KRAJ SLANJA RACUNA " & brojdokumenta & Environment.NewLine





    End Sub






    Sub RecalcGrid()
        Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
        checkedRows = TempGrid.GetDataRows
        Dim TempRowiNdex As Integer = 0


        If checkedRows.Length > 0 Then

            Try
                TempRowiNdex = TempGrid.CurrentRow.RowIndex
            Catch ex As Exception

            End Try


            Dim row As Janus.Windows.GridEX.GridEXRow
            For Each row In checkedRows
                TempGrid.MoveToRowIndex(row.RowIndex)
                PokupiParametre()
                For Each Column In TempGrid.RootTable.Columns
                    If Column.caption.ToString.ToLower.StartsWith("eval") Then
                        Dim imekolonekojutrebaizracunati As String = Column.caption.ToString.ToLower.Substring(4, Column.caption.ToString.Length - 4)
                        imekolonekojutrebaizracunati = imekolonekojutrebaizracunati.Substring(0, imekolonekojutrebaizracunati.Length - 2)
                        Dim izrazkojitrebaevaluirati As String = ""
                        izrazkojitrebaevaluirati = TempGrid.CurrentRow.Cells(Column.caption).Value
                        izrazkojitrebaevaluirati = izrazkojitrebaevaluirati.Replace("$$", "@@")
                        izrazkojitrebaevaluirati = cls1.SrediSQLPosljeNextForme(SrediSQL2(izrazkojitrebaevaluirati))

                        TempGrid.CurrentRow.Cells(imekolonekojutrebaizracunati).Value = EvaluateSTR(izrazkojitrebaevaluirati, "grid").Replace(",", ".")

                    End If

                Next
            Next

            Try
                TempGrid.MoveToRowIndex(TempRowiNdex) ' vrati nazad gdje smo bili
            Catch ex As Exception

            End Try

        End If
    End Sub


    Sub RecalcGridRow()

        PokupiParametre()
        For Each Column In TempGrid.RootTable.Columns
            If Column.caption.ToString.ToLower.StartsWith("eval") Then
                Dim imekolonekojutrebaizracunati As String = Column.caption.ToString.ToLower.Substring(4, Column.caption.ToString.Length - 4)
                imekolonekojutrebaizracunati = imekolonekojutrebaizracunati.Substring(0, imekolonekojutrebaizracunati.Length - 2)
                Dim izrazkojitrebaevaluirati As String = ""
                izrazkojitrebaevaluirati = TempGrid.CurrentRow.Cells(Column.caption).Value
                izrazkojitrebaevaluirati = izrazkojitrebaevaluirati.Replace("$$", "@@")
                izrazkojitrebaevaluirati = cls1.SrediSQLPosljeNextForme(SrediSQL2(izrazkojitrebaevaluirati))
                'izrazkojitrebaevaluirati = izrazkojitrebaevaluirati.Replace(".", ",")

                TempGrid.CurrentRow.Cells(imekolonekojutrebaizracunati).Value = EvaluateSTR(izrazkojitrebaevaluirati, "grid").Replace(",", ".")

            End If

        Next


    End Sub

    Function EvaluateSTR(ByVal Sql As String, ByVal Kontrola As String) As String
        On Error Resume Next
        EvaluateSTR = ""
        Dim Calc As New CalcContext(Of Double)("x")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("EVALUATE CREATE OBJECT ERROR : " & Err.Description)
            Exit Function
        End If
        Err.Clear()
        Calc.Constants.ImportBuiltIn()
        Calc.IgnoreCase = False
        Calc.OverflowCheck = True
        cls1.NapuniCBLog("EVALUATE :   " & Kontrola & " = " & Sql)
        Err.Clear()
        Dim result As Double = Calc.Evaluate(Sql, 1.0)  ' = 1.0
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("EVALUATE ERROR : " & Err.Description)
            Exit Function
        End If
        cls1.NapuniCBLog("RESULT :   " & result)
        Return result

    End Function

    Sub ForEachRow(ByVal param As String, ByVal param2 As String, ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        param = param.ToUpper
        param = param.Replace("FOREACHROW(", "")
        param = param.Replace(")", "")
        UsaouforeachROW = True
        For Each ctl As Control In Controls
            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                If Tctl.Name.Trim.ToUpper = param Then ' imam traženi grid
                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Tctl.GetDataRows
                    If checkedRows.Length > 0 Then
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            Tctl.MoveToRowIndex(row.RowIndex)
                            Rafalno(param2, sender, e)
                        Next
                    End If
                Else
                    ' MsgBox("Traženi grid za FOREACHSELECTED nije pronađen na formi !")
                End If
            End If
        Next
        UsaouforeachROW = False
    End Sub

    Sub ForEachSelected(ByVal param As String, ByVal param2 As String, ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        param = param.ToUpper
        param = param.Replace("FOREACHSELECTED(", "")
        param = param.Replace(")", "")
        For Each ctl As Control In Controls
            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                If Tctl.Name.Trim.ToUpper = param Then ' imam traženi grid
                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Tctl.GetCheckedRows()
                    If checkedRows.Length > 0 Then
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            Tctl.MoveToRowIndex(row.RowIndex)
                            'PokupiParametre()
                            Rafalno(param2, sender, e)
                        Next
                    End If
                Else
                    ' MsgBox("Traženi grid za FOREACHSELECTED nije pronađen na formi !")
                End If
            End If
        Next
    End Sub

    Sub ForEachBlueRow(ByVal param As String, ByVal param2 As String, ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        param = param.ToUpper
        param = param.Replace("FOREACHBLUEROW(", "")
        param = param.Replace(")", "")
        Dim i As Long = 0

        For Each ctl As Control In Controls
            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                If Tctl.Name.Trim.ToUpper = param Then ' imam traženi grid
                    Dim si(0) As Long
                    Array.Resize(si, Tctl.SelectedItems.Count)

                    'For Each oItem As Janus.Windows.GridEX.GridEXSelectedItem In Tctl.SelectedItems
                    'Tctl.MoveToRowIndex(oItem.GetRow().RowIndex)
                    'Rafalno(param2, sender, e)
                    'Next

                    For i = 0 To Tctl.SelectedItems.Count - 1
                        si(i) = Tctl.SelectedItems(i).GetRow().RowIndex
                    Next

                    'For i = si.Count - 1 To 0 Step -1
                    '    Tctl.MoveToRowIndex(si(i))
                    '    Rafalno(param2, sender, e)
                    'Next

                    For i = 0 To si.Count - 1
                        Tctl.MoveToRowIndex(si(i))
                        Rafalno(param2, sender, e)
                    Next


                Else
                    ' MsgBox("Traženi grid za FOREACHBLUEROW nije pronađen na formi !")
                End If
            End If
        Next
    End Sub

    Sub MultiSelectedPrintPriprema(ByVal param As String)
        On Error Resume Next
        param = param.ToUpper
        param = param.Replace("PREPAREMULTISELECTEDPRINT(", "")
        param = param.Replace(")", "")
        Array.Resize(cls1.arrMultiSelect, 0)
        Dim i As Integer = 0
        Dim tid5 As String = ""
        For Each ctl As Control In Controls
            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                If Tctl.Name.Trim.ToUpper = param Then ' imam traženi grid
                    Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
                    checkedRows = Tctl.GetCheckedRows()
                    If checkedRows.Length > 0 Then
                        Array.Resize(cls1.arrMultiSelect, checkedRows.Length)
                        Dim row As Janus.Windows.GridEX.GridEXRow
                        For Each row In checkedRows
                            tid5 = "0"
                            Tctl.MoveToRowIndex(row.RowIndex)
                            tid5 = Tctl.CurrentRow.Cells(2).Value.ToString.Trim
                            cls1.arrMultiSelect(i) = tid5
                            If i = 0 Then
                                arr(i) = tid5
                                ID = tid5
                            End If
                            i = i + 1
                        Next
                    End If
                Else
                    'MsgBox("Traženi grid za multiselectprint nije pronađen na formi !")
                End If
            End If
        Next
    End Sub

    Public Function SrediSQLsaKolonamaizGrida(ByVal sql As String)
        If sql = "" Or InStr(sql, "@(") = 0 Then
            SrediSQLsaKolonamaizGrida = sql
            Exit Function
        End If
        Dim Sql2 As String = ""
        Dim Kraj As Long = 0
        While InStr(sql, "@(")
            Sql2 = sql.Substring(sql.IndexOf("@("), sql.Length - sql.IndexOf("@(")) & " "
            If Sql2.IndexOf(")") > -1 Then
                Kraj = Sql2.IndexOf(")")
            End If
            Sql2 = Sql2.Substring(0, Kraj + 1)
            sql = sql.Replace(Sql2, NadjiVrijednostuGridu(Sql2))

        End While
        SrediSQLsaKolonamaizGrida = sql
    End Function

    Public Function NadjiVrijednostuGridu(ByVal Param As String) As String
        Param = Param.Replace("@(", "")
        Param = Param.Replace(")", "")
        Dim ImeGrida As String = ""
        Dim ImeKolone As String = ""
        ImeGrida = Param.Substring(0, InStr(Param, "..") - 1)
        ImeKolone = Param.Replace(ImeGrida + "..", "")
        On Error Resume Next
        ' Dim Tctl As Grid = Form2.Controls(ImeGrida) 'why o  vo ne radi ????
        NadjiVrijednostuGridu = ""
        For Each ctl As Control In Controls
            If TypeOf ctl Is Grid Then
                Dim Tctl As Grid = ctl
                If Tctl.Name.Trim.ToUpper = ImeGrida.ToUpper Then ' imam traženi grid
                    If Tctl.CurrentRow.Cells(ImeKolone).Value Is Nothing Then
                        NadjiVrijednostuGridu = "null"
                    ElseIf Tctl.CurrentRow.Cells(ImeKolone).Value Is DBNull.Value Then
                        NadjiVrijednostuGridu = "null"
                    Else
                        ' #### ovdje bi možda moglo biti problema sa prevodom ...!!!!!
                        'If Tctl.CurrentRow.Cells(ImeKolone).Column.Caption.ToLower.StartsWith("dat") Then
                        If Tctl.CurrentRow.Cells(ImeKolone).Column.Type.ToString.ToLower.Contains("date") Then
                            NadjiVrijednostuGridu = Tctl.CurrentRow.Cells(ImeKolone).Value.ToString.Trim.Substring(6, 4) & Tctl.CurrentRow.Cells(ImeKolone).Value.ToString.Trim.Substring(3, 2) & Tctl.CurrentRow.Cells(ImeKolone).Value.ToString.Trim.Substring(0, 2)
                        ElseIf IsNumeric(Tctl.CurrentRow.Cells(ImeKolone).Value.ToString) Then
                            NadjiVrijednostuGridu = Tctl.CurrentRow.Cells(ImeKolone).Value.ToString.Replace(",", ".")
                        Else
                            NadjiVrijednostuGridu = Tctl.CurrentRow.Cells(ImeKolone).Value.ToString.Trim
                        End If
                    End If
                End If
            End If
        Next
    End Function





    Private Sub Glbl_ComboClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        On Error Resume Next
        'RefreshControls(sender.OnClickSQL)
        NadjeniCOmboiFoundIndex = -1
        NadjeniCOmboiFoundIndex = sender.selectedindex
        '  MsgBox("comboclick " & NadjeniCOmboiFoundIndex)



        Rafalno(sender.OnClickSQL, sender, e)

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        On Error Resume Next
        AktivnaKontola.Select()
        Timer1.Enabled = False
        'Windows.Forms.SendKeys.Send("{ALT}")
    End Sub


    Private Sub Glbl_DateTime_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Rafalno(sender.OnValueChangedSQL, sender, e)

        'If sender.OnValueChangedSQL <> "" Then
        '    If sender.OnValueChangedSQL.Substring(0, 1) = ":" Then
        '        NovaGenerickaForma2(sender.OnValueChangedSQL.ToString.Replace(":", ""))
        '    Else
        '        RefreshControls(sender.OnValueChangedSQL)
        '    End If
        'End If

    End Sub

    Private Sub Glbl_EditingCell(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.EditingCellEventArgs)

        On Error Resume Next
        If sender.RowCount > 0 Then


            If sender.CurrentColumn.Caption.ToString <> "" Then
                If sender.CurrentRow.Cells(sender.CurrentColumn).Value Is Nothing Then

                Else
                    If Val(sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString) <> 0 Then
                        StaraVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString.Replace(",", ".")
                    Else
                        StaraVrijednost = sender.CurrentRow.Cells(sender.CurrentColumn).Value.ToString
                    End If
                End If
            End If


        End If


        If sender.BeforeCellEditedstr.trim = "" Then
            Exit Sub
        End If
        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        PokupiParametre()
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
                If sender.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
                    If sender.CurrentRow.Cells(i).Value Is Nothing Then

                    Else

                        Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
                            'ParametriSaGrida = ParametriSaGrida & "@" & sender.CurrentRow.Cells(i).Column.Caption.ToString & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            'ParametriSaGrida = ParametriSaGrida & "@" & sender.CurrentRow.Cells(i).Column.Caption.ToString & "='" & sender.CurrentRow.Cells(i).Value.ToString & "' , "
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra & "='" & sender.CurrentRow.Cells(i).Value.ToString & "' , "
                        End If
                    End If
                End If
            Next
        End If

        If Not PrijevodjeutijekU Then sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        Dim sql As String = ""
        sql = cls1.SrediSQLPosljeNextForme(SrediSQL2(sender.BeforeCellEditedstr))
        sql = cls1.SrediSQLPosljeNextForme(SrediSQL2(sender.BeforeCellEditedstr), ParametriSaGrida)
        Rafalno(sql, sender, e)
        If GreskaKodIzvrsavanjaSQLa.Trim <> "" Then
            e.Cancel = True
        End If
    End Sub

    Private Sub Glbl_CellEdited(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ParametriSaGrida As String = ""
        On Error Resume Next
        Dim i As Integer
        ParametriSaGrida = ""
        'PokupiParametre()
        TrenutnaKolona = sender.CurrentColumn
        TrenutnaVrijednost = ""

        If Not PrijevodjeutijekU Then sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked
        PokupiParametre()

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
                If sender.CurrentRow.Cells(i).Column.Caption.ToString <> "" Then
                    If sender.CurrentRow.Cells(i).Value Is Nothing Then

                    Else

                        Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If Val(sender.CurrentRow.Cells(i).Value.ToString) <> 0 Then
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        Else
                            ParametriSaGrida = ParametriSaGrida & "@" & strImeParametra & "='" & sender.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                        End If

                    End If
                End If
            Next
        End If

        'sender.CurrentRow.CheckState = Janus.Windows.GridEX.RowCheckState.Checked

        Dim sql As String = ""

        sql = cls1.SrediSQLPosljeNextForme(SrediSQL2(sender.ONCELLEDITEDstr), , , False)

        sql = cls1.SrediSQLPosljeNextForme(SrediSQL2(sender.ONCELLEDITEDstr), ParametriSaGrida, , False)

        GreskaKodIzvrsavanjaSQLa = ""
        Rafalno(sql, sender, e)

        On Error Resume Next
        If GreskaKodIzvrsavanjaSQLa <> "" Then

            'If StaraVrijednost.Trim.ToLower = "null" Then
            '    sender.CurrentRow.Cells(sender.CurrentColumn).Value = DBNull
            'Else
            '    sender.CurrentRow.Cells(sender.CurrentColumn).Value = StaraVrijednost
            'End If

            sender.CurrentRow.Cells(sender.CurrentColumn).Value = StaraVrijednost

        End If

    End Sub


    Private Sub Glbl_GridMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim ContextMenuStrip3 As New ContextMenuStrip

        On Error Resume Next

        Dim i As Integer
        TempGrid = sender
        ContextMenuStrip3.Items.Clear()
        ContextMenuStrip7.Items.Clear()
        If e.Button = Windows.Forms.MouseButtons.Right And ControlClicked = True Then
            For i = 1 To sender.RootTable.Columns.Count - 1
                Dim mnuItem As ToolStripMenuItem = ContextMenuStrip7.Items.Add(sender.CurrentRow.Cells(i).Column.Caption)
                Dim strImeParametra As String = sender.CurrentRow.Cells(i).Column.DataMember.ToString.Trim

                If sender.CurrentRow.Cells(i).Column.Visible = True Then
                    mnuItem.Checked = True
                Else
                    mnuItem.Checked = False
                End If
            Next i
            ControlClicked = False
            ContextMenuStrip7.Show(sender, sender.PointToClient(Windows.Forms.Cursor.Position))

        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            TempGrid = sender
            ContextMenuStrip3.Items.Clear()
            Dim a() As String
            Dim j As Integer
            If sender.strPopUp.ToString.Trim <> "" Then
                a = sender.strPopUp.Split("#")
                For j = 0 To a.GetUpperBound(0)


                    ' ContextMenuStrip3.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
                    ContextMenuStrip3.Items.Add(Translate(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":"))), Nothing, AddressOf ToolStripMenuItem1_Click)
                    ContextMenuStrip3.Items(j).Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)


                    Dim imeXMLa As String = ""
                    imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

                    imeXMLa = imeXMLa.Trim.ToLower



                    On Error Resume Next
                    If Not trsPermisio Is Nothing Then
                        If trsPermisio.Rows.Count > 0 Then
                            Dim r As DataRow
                            For Each r In trsPermisio.Rows

                                If r("objectname").ToString.Trim.ToLower = imeXMLa And r("functionname").ToString.Trim.ToLower = ContextMenuStrip3.Items(j).Text.Trim.ToLower Then

                                    If r("smije") = False Then
                                        ContextMenuStrip3.Items(j).Enabled = False
                                    End If

                                    If r("smije") = True Then
                                        ContextMenuStrip3.Items(j).Enabled = True
                                    End If

                                    Err.Clear()
                                    If r("Vidljivost") = False Then
                                        If Err.Number = 0 Then
                                            ContextMenuStrip3.Items(j).Visible = False
                                        End If
                                    End If

                                    Err.Clear()
                                    If r("Vidljivost") = True Then
                                        If Err.Number = 0 Then
                                            ContextMenuStrip3.Items(j).Visible = True
                                        End If
                                    End If

                                End If


                            Next
                        End If
                    End If


                Next
                ContextMenuStrip3.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
                ContextMenuStrip3.Items(j).Tag = "-"
            Else
                j = -1
            End If

            ContextMenuStrip3.Items.Add(Translate("Spremi postavke grida"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 1).Tag = "spremigrid"
            If ssPermissionProvjeriRafalnuKomandu("Spremi postavke grida") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Spremi postavke grida")
                ContextMenuStrip3.Items(j + 1).Enabled = False
            End If

            ContextMenuStrip3.Items.Add(Translate("Reset grida"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 2).Tag = "resetgrid"
            If ssPermissionProvjeriRafalnuKomandu("Reset grida") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Reset grida")
                ContextMenuStrip3.Items(j + 2).Enabled = False
            End If

            ContextMenuStrip3.Items.Add(Translate("Export grid"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 3).Tag = "exportgrid"
            If ssPermissionProvjeriRafalnuKomandu("Export grid") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Export grid")
                ContextMenuStrip3.Items(j + 3).Enabled = False
            End If

            ContextMenuStrip3.Items.Add(Translate("Grupiraj / razgrupiraj"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 4).Tag = "groupungropu"
            If ssPermissionProvjeriRafalnuKomandu("Grupiraj / razgrupiraj") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Grupiraj / razgrupiraj")
                ContextMenuStrip3.Items(j + 4).Enabled = False
            End If

            ContextMenuStrip3.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 5).Tag = "-"
            ContextMenuStrip3.Items.Add(Translate("Ispis REPX"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 6).Tag = "ispisdx"
            If ssPermissionProvjeriRafalnuKomandu("Ispis REPX") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis REPX")
                ContextMenuStrip3.Items(j + 6).Enabled = False
            End If

            ContextMenuStrip3.Items.Add(Translate("Ispis"), Nothing, AddressOf ToolStripMenuItem1_Click)
            ContextMenuStrip3.Items(j + 7).Tag = "ispisnovi"
            If ssPermissionProvjeriRafalnuKomandu("Ispis") = False Then
                cls1.NapuniCBLog("ZABRANA : PERMISSIO DENIED " + "Ispis")
                ContextMenuStrip3.Items(j + 7).Enabled = False
            End If

            'ContextMenuStrip3.Items.Add("-", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 5).Tag = "-"
            'ContextMenuStrip3.Items.Add("Pregled ispisa", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 6).Tag = "ispis"
            'ContextMenuStrip3.Items.Add("Pregled odabranih", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 7).Tag = "ispisodabranih"

            'ContextMenuStrip3.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 8).Tag = "-"
            'ContextMenuStrip3.Items.Add("Ispis", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 9).Tag = "ispisdirektan"
            'ContextMenuStrip3.Items.Add("Ispis odabranih", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 10).Tag = "ispisodabranihdirektan"

            'ContextMenuStrip3.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 11).Tag = "-"
            'ContextMenuStrip3.Items.Add("Ispis novi", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 12).Tag = "ispisnovi"




            ContextMenuStrip3.Show(sender, sender.PointToClient(Windows.Forms.Cursor.Position))
        End If


    End Sub



    Private Sub Glbl_TreeMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim ContextMenuStrip3 As New ContextMenuStrip
        If e.Button = Windows.Forms.MouseButtons.Right Then
            ' TempGrid = sender

            ContextMenuStrip3.Items.Clear()
            Dim a() As String
            Dim j As Integer
            If sender.strPopUp.ToString.Trim <> "" Then
                a = sender.strPopUp.Split("#")
                For j = 0 To a.GetUpperBound(0)
                    'ContextMenuStrip3.Items.Add(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")), ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
                    ContextMenuStrip3.Items.Add(a(j).ToString.Substring(0, a(j).ToString.IndexOf(":")), Nothing, AddressOf ToolStripMenuItem1_Click)
                    ContextMenuStrip3.Items(j).Tag = a(j).ToString.Substring(a(j).ToString.IndexOf(":") + 1, a(j).ToString.Length - a(j).ToString.IndexOf(":") - 1)
                Next
                ContextMenuStrip3.Items.Add("-", ImageList1.Images(0), AddressOf ToolStripMenuItem1_Click)
                ContextMenuStrip3.Items(j).Tag = "-"
            Else
                j = -1
            End If

            'ContextMenuStrip3.Items.Add("Spremi postavke grida", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 1).Tag = "spremigrid"
            'ContextMenuStrip3.Items.Add("Reset grida", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 2).Tag = "resetgrid"
            'ContextMenuStrip3.Items.Add("Export grid", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 3).Tag = "exportgrid"
            'ContextMenuStrip3.Items.Add("Grupiraj / razgrupiraj", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 4).Tag = "groupungropu"
            'ContextMenuStrip3.Items.Add("-", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 5).Tag = "-"
            'ContextMenuStrip3.Items.Add("Ispis", Nothing, AddressOf ToolStripMenuItem1_Click)
            'ContextMenuStrip3.Items(j + 6).Tag = "ispis"

            ContextMenuStrip3.Show(sender, sender.PointToClient(Windows.Forms.Cursor.Position))
        End If
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Static i As Long = 0
        Dim tStr As String = ""
        tStr = sender.tag
        Select Case tStr
            Case "spremigrid"
                SaveGridLayout()
            Case "resetgrid"
                DeleteGridLayout()
            Case "exportgrid"
                Dim xcFileDialog As New SaveFileDialog
                xcFileDialog.FileName = "exportdetail.xls"
                Dim GridEXExporter2 As New Janus.Windows.GridEX.Export.GridEXExporter
                GridEXExporter2.GridEX = TempGrid
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
                    TempGrid.ExpandGroups()
                    i = 1
                Else
                    TempGrid.CollapseGroups()
                    i = 0
                End If
            Case "ispis"
                Ispis()
            Case "ispisodabranih"
                Ispis(True)
            Case "ispisdirektan"
                Ispis(False, True)
            Case "ispisodabranihdirektan"
                Ispis(True, True)
            Case "ispisnovi"
                IspisNovi()
            Case "ispisdx"
                IspisGRIDREPX()
            Case Else
                If sender.tag.ToString.Contains("~") Then
                    sender.tag = sender.tag.ToString.Replace("~", "#")
                    Rafalno(sender.tag.ToString, sender, e)
                Else
                    NovaGenerickaForma2(sender.tag)
                    'Rafalno(sender.tag.ToString, sender, e)
                End If

        End Select
    End Sub



    'Sub Ispis()

    '    On Error Resume Next

    '    Dim GridEXPrintDocument1 As New Janus.Windows.GridEX.GridEXPrintDocument
    '    Dim PrintPreviewDialog1 As New PrintPreviewDialog
    '    Dim ParametriSaGrida As String = ""

    '    TempGrid.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
    '    TempGrid.RootTable.Columns(0).Visible = False
    '    GridEXPrintDocument1.GridEX = TempGrid

    '    If TempGrid.strPageHeaderCenter.Trim = "" Then
    '        GridEXPrintDocument1.PageHeaderCenter = Me.Text.Replace("##", vbCrLf)
    '    Else
    '        GridEXPrintDocument1.PageHeaderCenter = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), ParametriSaGrida).Replace("##", vbCrLf)
    '    End If

    '    If TempGrid.strPageHeaderLeft.Trim = "" Then
    '        GridEXPrintDocument1.PageHeaderLeft = strOpisniParametri.Replace("##", vbCrLf)
    '    Else
    '        GridEXPrintDocument1.PageHeaderLeft = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), ParametriSaGrida).Replace("##", vbCrLf)
    '    End If

    '    GridEXPrintDocument1.PageHeaderRight = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), ParametriSaGrida).Replace("##", vbCrLf)

    '    GridEXPrintDocument1.PageFooterRight = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterRight), ParametriSaGrida)
    '    GridEXPrintDocument1.PageFooterLeft = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterLeft), ParametriSaGrida)
    '    GridEXPrintDocument1.PageFooterCenter = cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterCenter), ParametriSaGrida)

    '    'GridEXPrintDocument1.FitColumns = FitColumnsMode.SizingColumns
    '    GridEXPrintDocument1.FitColumns = Janus.Windows.GridEX.FitColumnsMode.SizingColumns
    '    If TempGrid.strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
    '        GridEXPrintDocument1.DefaultPageSettings.Landscape = Orientation.Vertical
    '    Else
    '        GridEXPrintDocument1.DefaultPageSettings.Landscape = Orientation.Horizontal
    '    End If

    '    GridEXPrintDocument1.PrepareDocument()
    '    PrintPreviewDialog1.Document = GridEXPrintDocument1
    '    'PrintPreviewDialog1.Parent = Me
    '    PrintPreviewDialog1.ShowDialog()
    'End Sub


    Sub IspisNovi()

        On Error Resume Next
        Dim GridEXPrintDocument1 As New Janus.Windows.GridEX.GridEXPrintDocument
        Dim PrintPreviewDialog1 As New PrintPreviewDialog
        Dim ParametriSaGrida As String = ""
        TempGrid.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        TempGrid.RootTable.Columns(0).Visible = False
        GridEXPrintDocument1.GridEX = TempGrid
        Dim strPageOrientation As String = "PORTRAIT"
        Dim strPageHeaderCenter As String = ""

        Dim GP As New GridExPrinting.GridExPrinting()

        If TempGrid.strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GP.Orientation = GridExPrinting.Orientations.Landscape
        End If

        If TempGrid.strPageHeaderCenter.Trim = "" Then
            GP.PageHeaderCenter = Translate(Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf))
        Else
            GP.PageHeaderCenter = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        End If

        GP.PageHeaderLeft = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        GP.PageHeaderRight = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))

        GridEXPrintDocument1.PrintCollapsedRows = False

        'GP.JanusPrintDocument(GridEXPrintDocument1)
        GP.JanusPrintDocument(GridEXPrintDocument1, Conn.ConnectionString)

    End Sub


    Sub IspisGRIDREPX()
        'On Error Resume Next
        On Error Resume Next
        Dim a As Assembly = Assembly.Load("NetGenExtensions")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(NetGenExtensions) : " & Err.Description)
        End If
        Dim o As Object = a.CreateInstance("NetGenExtensions.frmGridExPrintingREPX")
        o.PageHeaderLeft = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        If TempGrid.strPageHeaderCenter.Trim = "" Then
            o.PageHeaderCenter = Translate(Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf))
        Else
            o.PageHeaderCenter = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        End If
        o.PageHeaderRight = Translate(Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf)))
        o.PrintDocumentREPX("lodani", TempGrid, m_xmld, TempGrid.m_node3, TempGrid.zadnjiSQL, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, "")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod DX ispisa lodanog grida : " & Err.Description)
        End If
    End Sub

    Sub ChartGRID()
        'On Error Resume Next
        On Error Resume Next
        Dim a As Assembly = Assembly.Load("NetGenExtensions")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(NetGenExtensions) : " & Err.Description)
        End If
        Dim o As Object = a.CreateInstance("NetGenExtensions.Form2")

        'o.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'If TempGrid.strPageHeaderCenter.Trim = "" Then
        '    o.PageHeaderCenter = Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf)
        'Else
        '    o.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'End If
        'o.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))

        o.ChartGRID("lodani", TempGrid.GridovDataTejbl, m_xmld, TempGrid.m_node3, TempGrid.zadnjiSQL, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, "")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod DX chartiranja lodanog grida : " & Err.Description)
        End If
    End Sub

    Sub PivotGRID()
        'On Error Resume Next
        On Error Resume Next
        Dim a As Assembly = Assembly.Load("NetGenExtensions")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod Assembly.Load(NetGenExtensions) : " & Err.Description)
        End If
        Dim o As Object = a.CreateInstance("NetGenExtensions.Form1")

        'o.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'If TempGrid.strPageHeaderCenter.Trim = "" Then
        '    o.PageHeaderCenter = Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf)
        'Else
        '    o.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'End If
        'o.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))

        o.PivotGRID("lodani", TempGrid.GridovDataTejbl, m_xmld, TempGrid.m_node3, TempGrid.zadnjiSQL, ID.ToString, cls1.OperateriID.ToString, "Driver={SQL Server};" + Conn.ConnectionString.ToString, strParametriSvi.ToString, "")
        If Err.Number <> 0 Then
            cls1.NapuniCBLog("Greška kod DX pivotiranja lodanog grida : " & Err.Description)
        End If
    End Sub

    Sub Ispis(Optional ByVal SamoOdabrani As Boolean = False, Optional ByVal DirektanIspis As Boolean = False)
        On Error Resume Next
        Dim GridEXPrintDocument1 As New Janus.Windows.GridEX.GridEXPrintDocument
        Dim PrintPreviewDialog1 As New PrintPreviewDialog
        Dim ParametriSaGrida As String = ""
        TempGrid.RowHeaders = Janus.Windows.GridEX.InheritableBoolean.False
        TempGrid.RootTable.Columns(0).Visible = False
        GridEXPrintDocument1.GridEX = TempGrid

        If SamoOdabrani Then
            GridEXPrintDocument1.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.Selection
        Else
            GridEXPrintDocument1.PrinterSettings.PrintRange = 0
        End If

        If TempGrid.strPageHeaderCenter.Trim = "" Then
            GridEXPrintDocument1.PageHeaderCenter = Me.Text.Replace("##", vbCrLf).Replace("!!", vbCrLf)
        Else
            GridEXPrintDocument1.PageHeaderCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        End If

        If TempGrid.strPageHeaderLeft.Trim = "" Then
            ' GridEXPrintDocument1.PageHeaderLeft = strOpisniParametri.Replace("##", vbCrLf)
        Else
            GridEXPrintDocument1.PageHeaderLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderLeft), , True))
            GridEXPrintDocument1.PageHeaderLeft = GridEXPrintDocument1.PageHeaderLeft.Replace("!!", vbCrLf).Replace("!!", vbCrLf)
        End If

        GridEXPrintDocument1.PageHeaderRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageHeaderRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        GridEXPrintDocument1.PageFooterRight = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterRight), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        GridEXPrintDocument1.PageFooterLeft = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterLeft), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        GridEXPrintDocument1.PageFooterCenter = Rafalna_Priprema(cls1.SrediSQLPosljeNextForme(SrediSQL2(TempGrid.strPageFooterCenter), , True).Replace("##", vbCrLf).Replace("!!", vbCrLf))
        'GridEXPrintDocument1.FitColumns = FitColumnsMode.SizingColumns
        GridEXPrintDocument1.FitColumns = Janus.Windows.GridEX.FitColumnsMode.SizingColumns
        If TempGrid.strPageOrientation.ToUpper.Trim = "LANDSCAPE" Then
            GridEXPrintDocument1.DefaultPageSettings.Landscape = Orientation.Vertical
        Else
            GridEXPrintDocument1.DefaultPageSettings.Landscape = Orientation.Horizontal
        End If
        ' GridEXPrintDocument1.
        GridEXPrintDocument1.PrepareDocument()
        PrintPreviewDialog1.Document = GridEXPrintDocument1
        PrintPreviewDialog1.BringToFront()
        PrintPreviewDialog1.AutoSizeMode = Windows.Forms.AutoSizeMode.GrowAndShrink
        'PrintPreviewDialog1.ShowDialog()


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
                If Ar5(i).ToUpper.Contains("ISEMPTY(") Then
                    param = Ar5(i).ToUpper
                    param = param.Replace("ISEMPTY(", "")
                    param = param.Replace(")", "")
                    If param.ToLower = "null" Or param.Trim = "" Then
                        i = i + 1
                        tS = tS & Ar5(i)
                        i = i + 1
                    Else
                        i = i + 2
                        tS = tS & Ar5(i)
                    End If
                End If

                'i = i + 1
            Next
        End If
        Rafalna_Priprema = SrediSQL2(tS)
    End Function


    Sub SaveGridLayout()
        On Error Resume Next
        Dim LayoutDir As String
        Dim LayoutStream As FileStream

        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        LayoutDir = GetLayoutDirectory() + "\" + "GridLodani-" + cls1.strProjectCaption + "-" + ImeFajla + "-" + strImeForme + "-" + TempGrid.Name.Trim + "-" + JupiterTranslatorMPJezik + ".gxl"
        LayoutStream = New FileStream(LayoutDir, FileMode.Create)


        If TempGrid.strFilterMode.ToLower.Trim = "true" Then
            'TempGrid.FilterMode = FilterMode.None
            'TempGrid.RemoveFilters()
        End If

        TempGrid.SaveLayoutFile(LayoutStream)
        LayoutStream.Close()

        If TempGrid.strFilterMode.ToLower.Trim = "true" Then
            'TempGrid.FilterMode = FilterMode.Automatic
        End If


    End Sub

    Sub DeleteGridLayout()
        On Error Resume Next
        Dim LayoutDir As String
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        LayoutDir = GetLayoutDirectory() + "\" + "GridLodani-" + cls1.strProjectCaption + "-" + ImeFajla + "-" + strImeForme + "-" + TempGrid.Name.Trim + "-" + JupiterTranslatorMPJezik + ".gxl"
        My.Computer.FileSystem.DeleteFile(LayoutDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        TempGrid.ClearStructure()
        TempGrid.ClearItems()
        RefreshControls("select 'refresh' as " & TempGrid.Name.Trim)
    End Sub

    Private Sub LoadLayout()
        On Error Resume Next
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        Dim LayoutDir = GetLayoutDirectory() + "\" + "GridLodani-" + cls1.strProjectCaption + "-" + ImeFajla + "-" + strImeForme + "-" + TempGrid.Name.Trim + "-" + JupiterTranslatorMPJezik + ".gxl"
        If My.Computer.FileSystem.FileExists(LayoutDir) Then
            Dim LayoutStream As FileStream
            LayoutStream = New FileStream(LayoutDir, FileMode.Open)
            TempGrid.LoadLayoutFile(LayoutStream)
            LayoutStream.Close()
            If strFilterSave <> "true" Then TempGrid.RemoveFilters()
        End If
    End Sub

    Private Function GetLayoutDirectory() As String

        Dim YourPath As String = ""
        YourPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\SPIN Informatica\Jupiter Software\Grid Settings\"

        If (Not System.IO.Directory.Exists(YourPath)) Then
            System.IO.Directory.CreateDirectory(YourPath)
        End If

        GetLayoutDirectory = YourPath

    End Function


    Public NadjeniCOmboiFoundIndex As Integer

    Public Sub AutoCompleteCombo_KeyUp(ByVal cbo As Combo, ByVal e As KeyEventArgs)
        On Error Resume Next
        Dim sTypedText As String
        Dim iFoundIndex As Integer
        Dim oFoundItem As Object
        Dim sFoundText As String
        Dim sAppendText As String

        'Allow select keys without Autocompleting
        Select Case e.KeyCode
            Case Keys.Back, Keys.Left, Keys.Right, Keys.Up, Keys.Delete, Keys.Down
                Return
        End Select

        'Get the Typed Text and Find it in the list
        sTypedText = cbo.Text

        iFoundIndex = cbo.FindString(sTypedText)
        'iFoundIndex = cbo.FindStringExact(sTypedText)

        cbo.SelectedTextMOJ = sTypedText
        ' iFoundIndex = cbo.FindStringExact(sTypedText)


        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString(" " & sTypedText)
            cbo.SelectedTextMOJ = " " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("  " & sTypedText)
            cbo.SelectedTextMOJ = "  " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("   " & sTypedText)
            cbo.SelectedTextMOJ = "   " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("    " & sTypedText)
            cbo.SelectedTextMOJ = "    " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("     " & sTypedText)
            cbo.SelectedTextMOJ = "     " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("      " & sTypedText)
            cbo.SelectedTextMOJ = "      " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("       " & sTypedText)
            cbo.SelectedTextMOJ = "       " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("        " & sTypedText)
            cbo.SelectedTextMOJ = "        " & sTypedText

        End If

        If iFoundIndex < 0 Then
            iFoundIndex = cbo.FindString("         " & sTypedText)
            cbo.SelectedTextMOJ = "         " & sTypedText

        End If



        'oFoundItem = cbo.Items(iFoundIndex)
        'sFoundText = cbo.GetItemText(oFoundItem)
        'sAppendText = sFoundText.Substring(sTypedText.Length)
        'cbo.SelectedTextMOJ = sAppendText


        NadjeniCOmboiFoundIndex = iFoundIndex
        ' MsgBox("keyup " & NadjeniCOmboiFoundIndex)

        'If we found the Typed Text in the list then Autocomplete
        If iFoundIndex >= 0 Then

            cbo.SelectedIndexMOJ = iFoundIndex

            'Get the Item from the list (Return Type depends if Datasource was bound 
            ' or List Created)
            oFoundItem = cbo.Items(iFoundIndex)

            'Use the ListControl.GetItemText to resolve the Name in case the Combo 
            ' was Data bound
            sFoundText = cbo.GetItemText(oFoundItem)

            'Append then found text to the typed text to preserve case
            sTypedText = cbo.SelectedTextMOJ
            sAppendText = sFoundText.Substring(sTypedText.Length)
            'sAppendText = sFoundText.Substring(cbo.SelectedTextMOJ)

            cbo.Text = sTypedText & sAppendText

            'cbo.Text = sFoundText

            'Select the Appended Text
            If sAppendText.Length > 0 Then
                cbo.SelectionStart = sTypedText.Length
                cbo.SelectionLength = sAppendText.Length
            End If

        End If

    End Sub

    Public Sub AutoCompleteCombo_Leave(ByVal cbo As Combo, ByVal e As System.EventArgs)
        On Error Resume Next
        Dim iFoundIndex As Integer

        'iFoundIndex = cbo.FindStringExact(cbo.Text)
        ' iFoundIndex = NadjeniCOmboiFoundIndex


        'iFoundIndex = cbo.SelectedIndexMOJ

        ' iFoundIndex = cbo.FindStringExact(cbo.SelectedTextMOJ)
        ' cbo.SelectedIndex = iFoundIndex



        iFoundIndex = cbo.FindStringExact(cbo.Text)
        cbo.SelectedIndex = iFoundIndex

        cbo.SelectedIndexMOJ = cbo.SelectedIndex

        If iFoundIndex = -1 Then
            'MsgBox("")
            cbo.SelectedIndex = 0
        End If

        ' ----------------------------------------------- izforsirani click  ' $$$$$
        Glbl_ComboClick(cbo, e)
        '-------------------------------------



        cbo.BackColor = Color.White

    End Sub


    Private Sub Glbl_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs)
        sender.BackColor = Color.White
        If TypeOf (sender) Is SearchBox Then
            If sender.strOnLeave.trim <> "" Then
                cls1.NapuniCBLog(" EVENT LEAVE SEARCHBOX SQL = " & sender.strOnLeave.trim)
                RefreshKontrola()
                Rafalno(sender.strOnLeave, sender, e)
            End If

            If sender.text.ToString.Trim = "" Then
                sender.tag = "0"
                OdradiosamIsforsiraniIzlazakizTrazilicenaKeydown = True
                RefreshKontrola()
                Rafalno(sender.OnLostFocusSQL, sender, e)
                'RefreshControls(sender.OnLostFocusSQL)
                Exit Sub
            End If

        End If
    End Sub

    Private Sub Glbl_LeaveTrazilica(ByVal sender As System.Object, ByVal e As System.EventArgs)
        sender.BackColor = Color.LightYellow
    End Sub

    Private Sub Glbl_Gotfocus2(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles TextBox1.Validated
        sender.BackColor = Color.LightGreen
        On Error Resume Next

        '' kk dodao 25.10.2018. - žele focus na datumu a ne na checkboxu
        'If TypeOf sender Is DTPicker Then
        '    Dim Tctl As DTPicker = sender
        '    If Tctl.ShowCheckBox Then
        '        SendKeys.Send("{Right}")
        '    End If
        'End If
        '------------------------------------

        Dim tssss As String = ""
        tssss = sender.strGotFocus
        'cls1.NapuniCBLog(" Glbl_Gotfocus2() : " & sender.Name & " , strGotFocus=" & tssss)
        Rafalno(sender.strGotFocus, sender, e)


        ' zakomentrao KK 14.10.2021. jer se NFC više ne koristi
        'If sender.strNFCSQL <> "" Then
        '    sender.Tag = sender.strNFCSQL
        '    If Timer4.Enabled = False Then
        '        'cls1.NapuniCBLog("NFC timer on")
        '        Timer4.Enabled = True
        '    End If
        'End If



        sender.selectall()
        'MsgBox(sender.text)
        'Me.Controls("nekibroj").Text = sender.text
        '  NadjeniCOmboiFoundIndex = -1
    End Sub

    Private Sub ContextMenuStrip7_Closing(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosingEventArgs) Handles ContextMenuStrip7.Closing
        If dontclose = True Then
            e.Cancel = True
            dontclose = False
        End If
    End Sub

    Private Sub ContextMenuStrip7_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ContextMenuStrip7.ItemClicked
        'MsgBox(sender.name)

        On Error Resume Next

        dontclose = True

        Dim ti As Integer

        Dim i As Integer

        ' e.ClickedItem.Select()



        For i = 0 To TempGrid.RootTable.Columns.Count - 1

            If TempGrid.RootTable.Columns(i).Caption.ToLower = e.ClickedItem.Text.ToLower Then
                ti = i
            End If

        Next

        If TempGrid.RootTable.Columns(ti).Visible Then
            TempGrid.RootTable.Columns(ti).Visible = False
        Else
            TempGrid.RootTable.Columns(ti).Visible = True
        End If

        Dim mytext As ToolStripMenuItem
        mytext = DirectCast(e.ClickedItem, ToolStripMenuItem)

        mytext.Checked = Not mytext.Checked

    End Sub



    Public Sub DajPokaziHelp()
        On Error Resume Next



        Dim imeXMLa As String = ""
        imeXMLa = strXML2.Substring(InStrRev(strXML2, "\"), strXML2.Length - InStrRev(strXML2, "\"))

        'Dim hf1 As New Help_Client_V8.Class1
        'hf1.Run(strHelpKod2, cls1.strBaza, cls1.strServer, imeXMLa)

        Dim p As New Process()
        p.StartInfo.FileName = "JupiterHelp.exe"
        p.StartInfo.Arguments = "-s " + cls1.strServer + " -d " + cls1.strBaza + " -h " + strHelpKod2 + " -o " + Str(cls1.OperateriID) + " -p " + imeXMLa
        p.StartInfo.CreateNoWindow = True
        p.EnableRaisingEvents = True
        ''p.Exited += New EventHandler(p_Exited)
        ''p.StartInfo.Verb = "runas"
        p.StartInfo.WorkingDirectory = Environment.CurrentDirectory
        p.Start()



        'Dim p As New Process()
        'p.StartInfo.FileName = "JupiterHelpClient.exe"
        'p.StartInfo.Arguments = "-s " + cls1.strServer + " -d " + cls1.strBaza + " -k " + strHelpKod2 + " -o " + Str(cls1.OperateriID)
        'p.StartInfo.CreateNoWindow = True
        'p.EnableRaisingEvents = True
        ''p.Exited += New EventHandler(p_Exited)
        ''p.StartInfo.Verb = "runas"
        'p.StartInfo.WorkingDirectory = Environment.CurrentDirectory
        'p.Start()


    End Sub


    ' serial port .......................................................
    Sub SerialPort1_DajImenaPotova()
        Dim comPorts As Array
        comPorts = IO.Ports.SerialPort.GetPortNames()

        For i = 0 To UBound(comPorts)
            MsgBox(comPorts(i))
        Next
    End Sub
    Sub SerialPort1_Postimaj()

        SerialPort1.PortName = ""
        SerialPort1.BaudRate = 19200
        SerialPort1.Parity = IO.Ports.Parity.None
        SerialPort1.StopBits = IO.Ports.StopBits.One
        SerialPort1.DataBits = 8
    End Sub

    Sub SerialPort1_Pisi()
        Dim rxBuff As String
        rxBuff = ""
        If SerialPort1.IsOpen = False Then SerialPort1.Open()
        SerialPort1.Write("aaaaa" & vbCr)
        System.Threading.Thread.Sleep(800)
        If SerialPort1.IsOpen = True Then SerialPort1.Close()

    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Dim rxBuff As String
        System.Threading.Thread.Sleep(300)
        rxBuff = (SerialPort1.ReadExisting)
    End Sub



    Private Sub glblGRID_FormattingRow(ByVal sender As System.Object, ByVal e As Janus.Windows.GridEX.RowLoadEventArgs)
        On Error Resume Next

        If e.Row.RowType = RowType.Record Then
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

                If e.Row.Cells.Item(ii).Column.Caption.Contains("MaxLinesID") Then
                    imekOlone = e.Row.Cells.Item(ii).Column.Caption.Replace("MaxLinesID", "")
                    e.Row.Cells(imekOlone).Column.WordWrap = True
                    e.Row.Cells(imekOlone).Column.MaxLines = e.Row.Cells(ii).Value.ToString
                End If

                If e.Row.Cells.Item(ii).Column.Caption.Contains("CellForeColorID") Then
                    strBojaFonta5 = e.Row.Cells(ii).Value.ToString
                    Dim c5 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta5)
                    imekOlone = e.Row.Cells.Item(ii).Column.Caption.Replace("CellForeColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.ForeColor = c5
                End If

                If e.Row.Cells.Item(ii).Column.Caption.Contains("CellBackColorID") Then
                    strBojaFonta6 = e.Row.Cells(ii).Value.ToString
                    Dim c6 As Color = System.Drawing.ColorTranslator.FromHtml(strBojaFonta6)

                    imekOlone = e.Row.Cells.Item(ii).Column.Caption.Replace("CellBackColorID", "")
                    'style5 = e.Row.Cells(imekOlone).FormatStyle
                    style5.BackColor = c6
                    'e.Row.Cells(e.Row.Cells.Item(ii).Column.Caption.Replace("CellBackColorID", "")).FormatStyle = style6
                End If

                If imekOlone <> "" Then e.Row.Cells(imekOlone).FormatStyle = style5


            Next

        End If


    End Sub

    Private Sub ContextMenuStrip7_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip7.Opening

    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Rafalno(strTimerSQL, sender, e)
    End Sub




    Private Sub Form2_PaddingChanged(sender As Object, e As EventArgs) Handles Me.PaddingChanged

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick

        Timer3.Enabled = False

        If StrWINDOWSTATE = "2" Then
            Me.WindowState = FormWindowState.Maximized
        End If

        Dim Nadjen As Boolean = False
        Dim Tstr As String = ""
        Dim tstr6 As String = ""
        On Error Resume Next
        For Each ctl As Control In Controls

            If TypeOf ctl Is DevExpress.XtraEditors.CheckedComboBoxEdit Then
                Dim Tctl As DevExpress.XtraEditors.CheckedComboBoxEdit = ctl
                Nadjen = True
                If Tctl.Properties.Items.Count = 0 Then
                    Tctl.ShowPopup()
                    Tctl.ClosePopup()
                    Dim nizajdijeva6 As String = ""
                    nizajdijeva6 = Tctl.Tag.Trim
                    ' MsgBox("aaa" & nizajdijeva6)
                    If nizajdijeva6 <> "" Then
                        Tctl.Tag = ""
                        nizajdijeva6 = ",,," & nizajdijeva6.Replace("~", ",") & ","
                        For i8 = 0 To Tctl.Properties.Items.Count - 1
                            If nizajdijeva6.IndexOf("," & Tctl.Properties.Items(i8).Value.ToString.Trim & ",", 0) > 0 Then
                                Tctl.Properties.Items(i8).CheckState = CheckState.Checked
                            End If
                        Next
                    End If
                End If
            End If

        Next

        If strAktivnaKontrolanakonLodanja <> "" Then
            AktivnaKontola = Me.Controls(strAktivnaKontrolanakonLodanja)
            Timer1.Enabled = True
        End If

    End Sub



    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        On Error Resume Next
        Dim strProcitajKarticu As String = ""

        Timer4.Enabled = False


        ' zakomentirao KK 14.10.2021. jer se NFC više ne koristi u NG-u

        'Dim a2 As Assembly = Assembly.Load("NFCSpin")
        'If Err.Number <> 0 Then
        '    Timer4.Enabled = False
        '    cls1.NapuniCBLog("greška kod Assembly.Load NFCSpin : " & Err.Description)
        '    'MsgBox(Err.Description)
        '    Exit Sub
        'End If

        'Dim NFCSpin As Object = a2.CreateInstance("NFCSpin.NFCSpinClass")
        'strProcitajKarticu = NFCSpin.NFCSpinClass.ProcitajKarticu()

        'If strProcitajKarticu <> "" Then
        '    Timer4.Enabled = False
        '    AktivnaKontola.Text = strProcitajKarticu
        '    cls1.NapuniCBLog("read NFC : " & strProcitajKarticu)
        '    cls1.NapuniCBLog("idem u rafalno od NFC-a : " & AktivnaKontola.Tag)
        '    Rafalno(AktivnaKontola.Tag, Nothing, Nothing)
        'End If
    End Sub
End Class