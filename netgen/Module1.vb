Imports System.Windows.Forms
Imports System.Reflection


Module Module1



    Public Sender7 As Object = Nothing
    Public arrI7(0) As Integer
    Public Template7 As String
    Public prevParametri7 As String

    Public strMultilingual As String = "0"
    Public strPismo As String = ""

    Public m_xmld As Xml.XmlDocument ' KKKK

    Public Language As String = ""
    Public node77 As String = ""

    Public MOJremotedllpath As String = ""
    Public strHelpKod As String = ""
    Public strFilterSave As String = ""


    Public RSLang As DataTable

    Public arr(0) As String
    '<ThreadStatic> Private _arr(0) As String
    'Public Property arr(0) As String
    '    Get
    '        Return _arr(0)
    '    End Get
    '    Set(ByVal value As String)
    '        _arr(0) = value
    '    End Set
    'End Property


    'Public Globalmain_handle As IntPtr
    <ThreadStatic> Private _Globalmain_handle As IntPtr
    Public Property Globalmain_handle As IntPtr
        Get
            Return _Globalmain_handle
        End Get
        Set(ByVal value As IntPtr)
            _Globalmain_handle = value
        End Set
    End Property


    'Public GlobalhWnd As Integer
    <ThreadStatic> Private _GlobalhWnd As Integer
    Public Property GlobalhWnd As Integer
        Get
            Return _GlobalhWnd
        End Get
        Set(ByVal value As Integer)
            _GlobalhWnd = value
        End Set
    End Property


    Public FakPretvorbe As Long = 14

    Public TrazilicaID As String
    Public TrazilicaIDstr As String
    Public GreskaKodIzvrsavanjaSQLa As String = ""

    'Public strSviParametriuModulu As String
    'Public strStartupStoraDetalj As String = ""

    Public PrefixZaSpremanjeMasterGrida As String = "GridMaster"
    Public PrefixZaSpremanjeDetaljGrida As String = "GridDet"

    ' Public strProjectCaption As String = ""
    Public strAktivnaKontrolanakonLodanja As String = ""
    Public ZelimIzaciIzForme As Boolean = False


    'Public frm1 As Form1
    <ThreadStatic> Private _frm1 As Form1
    Public Property frm1 As Form1
        Get
            Return _frm1
        End Get
        Set(ByVal value As Form1)
            _frm1 = value
        End Set
    End Property


    Public frm3 As Form3
    Public frm5 As Form5



    ' Public IDzaNoviSlog As String = ""
    <ThreadStatic> Private _IDzaNoviSlog As String
    Public Property IDzaNoviSlog As String
        Get
            Return _IDzaNoviSlog
        End Get
        Set(ByVal value As String)
            _IDzaNoviSlog = value
        End Set
    End Property


    'Public ImeAplikacijeZaProfiler As String = ""
    <ThreadStatic> Private _ImeAplikacijeZaProfiler As String
    Public Property ImeAplikacijeZaProfiler As String
        Get
            Return _ImeAplikacijeZaProfiler
        End Get
        Set(ByVal value As String)
            _ImeAplikacijeZaProfiler = value
        End Set
    End Property

    ' Public MasterID As String = ""
    ' Public TrazenJeMultiSelectIspis As Boolean = False



    ' Public Conn As SqlClient.SqlConnection
    <ThreadStatic> Private _Conn As SqlClient.SqlConnection
    Public Property Conn As SqlClient.SqlConnection
        Get
            Return _Conn
        End Get
        Set(ByVal value As SqlClient.SqlConnection)
            _Conn = value
        End Set
    End Property


    'Public Conn2 As OleDb.OleDbConnection
    <ThreadStatic> Private _Conn2 As OleDb.OleDbConnection
    Public Property Conn2 As OleDb.OleDbConnection
        Get
            Return _Conn2
        End Get
        Set(ByVal value As OleDb.OleDbConnection)
            _Conn2 = value
        End Set
    End Property


    'Public Conn3 As SqlClient.SqlConnection
    <ThreadStatic> Private _Conn3 As SqlClient.SqlConnection
    Public Property Conn3 As SqlClient.SqlConnection
        Get
            Return _Conn3
        End Get
        Set(ByVal value As SqlClient.SqlConnection)
            _Conn3 = value
        End Set
    End Property


    'Public Conn4 As SqlClient.SqlConnection
    <ThreadStatic> Private _Conn4 As SqlClient.SqlConnection
    Public Property Conn4 As SqlClient.SqlConnection
        Get
            Return _Conn4
        End Get
        Set(ByVal value As SqlClient.SqlConnection)
            _Conn4 = value
        End Set
    End Property




    Public strCommandTimeout As String = "30"
    'Public strInclude As String = ""
    ' Public strIncludeGlobal As String = ""
    Public DefaultConnection As Integer = 1
    Public strCUSTOMCONNECTIONSTRING As Integer = 1
    Public strParametriIzTrazilice As String = ""

    Public strIzabranaBoja As String = ""
    Public strFindNode2 As String = ""
    Public strKoloneZaGrupiranje As String = ""
    Public Version1 As String = ""

    Public strPermisio As Boolean = True
    Public trsPermisio As DataTable
    Public strXMLVersion As String = ""
    Public DXGrid As String = ""


    Public JupiterTranslatorMP As Boolean = True
    Public JupiterTranslatorMPJezik As String = ""


    'Public JupiterPrevoditelj As New JupiterTranslatorMP.Translator

    ' ### valentinotranslator4.10.2023. zakomentirano
    Public JupiterPrevoditelj As New JupiterTranslator.Translator
    'C:\Users\kkovac.SPIN\Documents\Visual Studio 2012\Projects\NetGenDLL core\Main\NetGenDLL\bin\Debug\JupiterTranslator.dll

    'Public JupiterPrevoditelj As JupiterTranslator.Translator

    Public JupiterPrevoditeljValentinoType As Type
    Public JupiterPrevoditeljValentinoBoolean As Boolean = False

    Public UsoSamURafal As Boolean = False
    Public ZanemariClick As Boolean = False

    Public PrijevodjeutijekU As Boolean = False

    Public strGlobalSQLSERVERlogin As String = ""



    Public Function TranslateForm(ByVal frmMe As Form) As Boolean
        On Error Resume Next

        ''NapuniRSLang
        'If Language.Trim = "" Then Exit Function
        'If RSLang Is Nothing Then Exit Function
        'If RSLang.Rows.Count = 0 Then Exit Function
        'If strMultilingual = "0" Or strMultilingual = "1" Then Exit Function

        For Each ctl As Control In frmMe.Controls

            If TypeOf ctl Is Label Then
                ctl.Text = Translate(ctl.Text)
                'ctl.Text = Translate("Spremi")
            End If

            If TypeOf ctl Is CheckBox Then
                ctl.Text = Translate(ctl.Text)
                'ctl.Text = Translate("Spremi")
            End If

            If TypeOf ctl Is RadioButton Then
                ctl.Text = Translate(ctl.Text)
                'ctl.Text = Translate("Spremi")
            End If

            If TypeOf ctl Is Button Then
                ctl.Text = Translate(ctl.Text)
            End If

            If TypeOf ctl Is Grid Then
                PrevediSadrzajGrida(ctl)
                PrevediSadrzajGrida2(ctl)
            End If

            If TypeOf ctl Is GroupBox Or TypeOf ctl Is Panel Then
                ctl.Text = Translate(ctl.Text)
            End If

        Next ctl


        frmMe.Text = Translate(frmMe.Text)

        'frmMe.Translated = True
    End Function

    Public Function Translate(ByVal str As String) As String
        On Error Resume Next
        Translate = str
        If JupiterTranslatorMPJezik.Trim.ToLower <> "" Then
            If JupiterTranslatorMPJezik.Trim.ToLower <> "hrv" Then

                ' ### valentinotranslator4.10.2023. zakomentirano
                Translate = JupiterPrevoditelj.TranslateString(str)

                If JupiterPrevoditeljValentinoBoolean = True Then
                    Dim parameters() As Object = {str, "NetGen"}
                    ' Pozivanje statičke metode
                    Dim result As String = JupiterPrevoditeljValentinoType.InvokeMember("TranslateString", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static,
                        Nothing, Nothing, parameters)
                    Translate = result
                End If

            End If
        End If





        'Translate = str
    End Function

    Public Function Translate_OLD(ByVal str As String) As String

        Translate_OLD = str
        If strMultilingual = "0" Or strMultilingual = "1" Then Exit Function
        If Language.Trim = "" Then Exit Function
        If RSLang Is Nothing Then Exit Function

        If RSLang.Rows.Count = 0 Then
            Exit Function
        End If

        ' RSLang.Find("original='" & str & "'")
        ' Dim dro() As DataRow = RSLang.Select("original='" & str & "'")
        ' On Error Resume Next
        ' Translate = dro(0).Table.Rows(0)(2)
        'Translate = dro.("translated")

        Dim r As DataRow

        For Each r In RSLang.Rows
            If r("original").ToString.Trim.ToLower = str.Trim.ToLower Then
                Translate_OLD = r("translated")
                If strPismo = "cir" Then Translate_OLD = ToCiril2(Translate_OLD)
                Exit Function
            End If
        Next

        If str.Contains(" ") Then ' nije pronašao i idemo prevesti rijec po rijec
            Dim tstr As String = ""
            Dim tstr2 As String = ""
            Dim a() As String
            a = Split(str.ToString, " ")
            For j = 0 To a.GetUpperBound(0)
                tstr2 = a(j)
                For Each r In RSLang.Rows
                    If r("original").ToString.Trim.ToLower = a(j).Trim.ToLower Then
                        tstr2 = r("translated")
                    End If
                Next
                tstr = tstr & " " & tstr2
            Next

            If tstr.Trim <> "" Then
                Translate_OLD = tstr.Trim
            End If

        End If


        If strPismo = "cir" Then Translate_OLD = ToCiril2(Translate_OLD)

    End Function

    Public Function Translate2(ByVal destLang As String, ByVal src As String) As String ' ovo je Vitnerov algoritam
        Translate2 = src
        If strMultilingual = "0" Or strMultilingual = "1" Then Exit Function
        If Language.Trim = "" Then Exit Function
        If RSLang Is Nothing Then Exit Function

        Dim srcLower As String = src.ToLower()

        Dim i As Integer
        For Each dr As DataRow In RSLang.Rows
            i = src.IndexOf(dr("original").ToString(), StringComparison.OrdinalIgnoreCase)
            While i >= 0
                src = String.Concat(src.Substring(0, i), dr("translated").ToString, src.Substring(i + dr("original").ToString.Length))
                i = src.IndexOf(dr("original").ToString(), i + dr("translated").ToString.Length, StringComparison.OrdinalIgnoreCase)
            End While
        Next

        Translate2 = src
    End Function

    Sub PrevediHedere(ByVal GridEX1 As Janus.Windows.GridEX.GridEX)


        '  Exit Sub ' zasad


        On Error Resume Next
        Dim i As Integer

        ' If GridEX1.RowCount > 0 Then
        For i = 2 To GridEX1.RootTable.Columns.Count - 1
            If GridEX1.RootTable.Columns(i).Visible = True Then
                GridEX1.RootTable.Columns(i).Caption = Translate(GridEX1.RootTable.Columns(i).Caption).Replace("*", "")
            End If
            'If strMultilingual = "3" Then
            '    GridEX1.RootTable.Columns(i).Caption = ToCiril2(GridEX1.RootTable.Columns(i).Caption)
            'End If

        Next
        ' End If

    End Sub

    Sub PrevediSadrzajGrida(ByVal GridEX1 As Janus.Windows.GridEX.GridEX)

        If JupiterTranslatorMPJezik.Trim.ToLower <> "" Then
            If JupiterTranslatorMPJezik.Trim.ToLower <> "hrv" Then
                PrevediHedere(GridEX1)
            End If
        End If

        Exit Sub
        'On Error Resume Next
        'If strMultilingual <> "3" Then Exit Sub ' samo 3ojka čirilizira sadržaj
        'Dim checkedRows() As Janus.Windows.GridEX.GridEXRow
        'checkedRows = GridEX1.GetDataRows
        'If checkedRows.Length > 0 Then
        '    Dim row As Janus.Windows.GridEX.GridEXRow
        '    For Each row In checkedRows
        '        GridEX1.MoveToRowIndex(row.RowIndex)
        '        For i = 1 To GridEX1.RootTable.Columns.Count - 1


        '            If GridEX1.CurrentRow.Cells(i).Value Is Nothing Or GridEX1.CurrentRow.Cells(i).Value Is DBNull.Value Then

        '            Else

        '                If IsNumeric(GridEX1.CurrentRow.Cells(i).Value.ToString) Then

        '                Else
        '                    If strPismo = "cir" Then
        '                        'GridEX1.CurrentRow.Cells(i).Value = Translate(GridEX1.CurrentRow.Cells(i).Value.ToString.Trim)
        '                        ' samo čiriliziraj ....
        '                        GridEX1.CurrentRow.Cells(i).Value = ToCiril2(GridEX1.CurrentRow.Cells(i).Value.ToString.Trim)
        '                    End If

        '                End If

        '            End If


        '        Next
        '    Next
        'End If

    End Sub

    Sub PrevediSadrzajGrida2(ByVal GridEX1 As Grid)
        On Error Resume Next
        If GridEX1.strTranslateColumns.Trim = "" Then Exit Sub
        GridEX1.strTranslateColumns = "," & GridEX1.strTranslateColumns & ","

        Dim RA2 As Integer = GridEX1.CurrentRow.AbsolutePosition
        PrijevodjeutijekU = True

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

                    If GridEX1.strTranslateColumns.Trim.ToLower.Contains("," & strImeParametra.Trim.ToLower & ",") Then
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

    Function ToCiril2(ByVal tekst As String) As String

        Dim Latinica As String
        Dim Cirilica As String
        Dim i As Integer
        Dim k As Integer
        Dim cTekst As String
        Dim slovo As String


        Latinica = "abcdefghijklmnoprstuvzABCDEFGHIJKLMNOPRSTUVZ" & "š" & ChrW(263) & ChrW(273) & ChrW(382) & ChrW(269) & "Š" & ChrW(262) & ChrW(272) & ChrW(381) & ChrW(268)

        Cirilica = ChrW(1072) & ChrW(1073) & ChrW(1094) & ChrW(1076) & ChrW(1077) & ChrW(1092) & ChrW(1075) & ChrW(1093) & ChrW(1080) & ChrW(1112) & ChrW(1082) & ChrW(1083) & ChrW(1084) & ChrW(1085) & ChrW(1086) & ChrW(1087) & ChrW(1088) & ChrW(1089) & ChrW(1090) & ChrW(1091) & ChrW(1074) & ChrW(1079) & ChrW(1040) & ChrW(1041) & ChrW(1062) & ChrW(1044) & ChrW(1045) & ChrW(1060) & ChrW(1043) & ChrW(1061) & ChrW(1048) & ChrW(1032) & ChrW(1050) & ChrW(1051) & ChrW(1052) & ChrW(1053) & ChrW(1054) & ChrW(1055) & ChrW(1056) & ChrW(1057) & ChrW(1058) & ChrW(1059) & ChrW(1042) & ChrW(1047) & ChrW(1096) & ChrW(1115) & ChrW(1106) & ChrW(1078) & ChrW(1095) & ChrW(1064) & ChrW(1035) & ChrW(1026) & ChrW(1046) & ChrW(1063)

        Dim LatLjNjDz() As String = {"LJ", "Lj", "lj", "NJ", "Nj", "nj", "D" & ChrW(381), "D" & ChrW(382), "d" & ChrW(382)}

        Dim CirLjNjDz() As String = {ChrW(1033), ChrW(1033), ChrW(1113), ChrW(1034), ChrW(1034), ChrW(1114), ChrW(1039), ChrW(1039), ChrW(1119)}


        cTekst = tekst ' Posle zamenje LJ, NJ
        For i = 0 To UBound(LatLjNjDz)
            cTekst = Replace(cTekst, LatLjNjDz(i), CirLjNjDz(i))
        Next i

        For i = 1 To Len(cTekst)
            slovo = Mid(cTekst, i, 1)
            k = InStr(1, Latinica, slovo)
            If k > 0 Then
                Mid(cTekst, i, 1) = Mid(Cirilica, k, 1)
            End If
        Next i
        ToCiril2 = cTekst


    End Function






End Module
