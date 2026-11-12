Imports System.Windows.Forms
Imports System.IO

Public Class Form3
    Public cls1 As Class1
    Public Shared ID As String
    Public Shared strID As String = ""
    Public Shared OKClick As Boolean
    Public Shared GridRowCount As Long
    Public strColumnAutoResize As String = ""
    Public Shared strXMLfile2 As String = ""
    Public Shared strImeForme As String = ""
    Public Shared searchSQL5 As String = ""
    ' Public Shared strParametriIzTrazilice = ""
    Dim Translated As Boolean = False


    Public Shared Sub Odaberi()
        OKClick = True
        ID = frm3.GetID2()

        TrazilicaID = ID.Trim
        ' TrazilicaIDstr = strID.Trim & " (" & ID & ")"

        TrazilicaIDstr = strID.Trim

        strParametriIzTrazilice = ""


        

    End Sub


    


    Sub spremipostavke()
        On Error Resume Next
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        Dim LayoutDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\" + "GridTrazilica-" + ImeFajla + "-" + strImeForme + ".gxl"
        Dim LayoutStream As FileStream
        LayoutStream = New FileStream(LayoutDir, FileMode.Create)
        frm3.GridEX1.SaveLayoutFile(LayoutStream)
        LayoutStream.Close()
    End Sub

    Sub DeleteGridLayout()
        On Error Resume Next
        Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
        ImeFajla = ImeFajla.Replace("\", "").Trim
        ImeFajla = ImeFajla.Replace(".xml", "").Trim
        Dim LayoutDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\" + "GridTrazilica-" + ImeFajla + "-" + strImeForme + ".gxl"
        My.Computer.FileSystem.DeleteFile(LayoutDir, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Odaberi()
        Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OKClick = False
        Me.Close()
    End Sub


    Public Sub Trazilica(ByVal txtTrazi As String, ByVal Parametri As String, ByVal Sender As Object)
        Dim StrS As String = ""
        Dim Polja As String = ""
        Dim SortPolja As String = ""
        Dim PomPolja As String = ""
        Dim Tablica As String
        Dim Lstr As String = ""
        Dim Dstr As String = ""
        Dim tar
        Dim tar2() As String
        Dim i, j As Long
        Dim wstr As String = ""
        Dim sstr As String = ""
        Dim separator As String = ""
        Dim sql As String = ""
        searchSQL5 = ""
        If Parametri.Trim = "" Then Exit Sub
        Parametri = cls1.SrediSQLPosljeNextForme(Parametri).Trim

        cls1.NapuniCBLog("U FORM3.TRAZILICA, sql = " & Parametri)
        cls1.NapuniCBLog("U FORM3.TRAZILICA, tražim = " & txtTrazi)

        GridRowCount = 0
        txtTrazi.Trim.Replace("'", "")
        StrS = txtTrazi
        If StrS = "" Then Exit Sub


        If Parametri.ToLower.StartsWith("sp") Or Parametri.ToLower.StartsWith("select") Then
            sql = Parametri
        Else
            tar2 = Parametri.Split("#")
            Tablica = tar2(0)
            frm3.Text = txtTrazi
            For i = 1 To tar2.GetUpperBound(0)
                If i > 1 Then SortPolja = SortPolja & tar2(i).Replace("!", "").Replace("%", "") & ", "
                Polja = Polja & tar2(i).Replace("!", "").Replace("%", "").Replace("desc", "").Replace("asc", "") & ", "
            Next
            Polja = Polja.Substring(0, Polja.Length - 2)
            If InStr(Polja, "/", CompareMethod.Text) Then Polja = Polja.Substring(0, InStr(Polja, "/", CompareMethod.Text) - 3)
            SortPolja = SortPolja.Substring(0, SortPolja.Length - 2)
            If InStr(SortPolja, "/", CompareMethod.Text) Then SortPolja = SortPolja.Substring(0, InStr(SortPolja, "/", CompareMethod.Text) - 3)
            tar = StrS.Split(" ")

            For i = 0 To tar.GetUpperBound(0)
                PomPolja = "("
                For j = 2 To tar2.GetUpperBound(0)
                    If InStr(tar2(j), "!", CompareMethod.Text) Then

                    ElseIf InStr(tar2(j), "/") Then
                        wstr = tar2(j).Replace("/", "")
                    Else
                        If tar2(j).Trim.Replace("desc", "").Replace("asc", "").Substring(0, 1) = "%" Then Lstr = "%" Else Lstr = ""
                        If tar2(j).Trim.Replace("desc", "").Replace("asc", "").EndsWith("%") Then Dstr = "%" Else Dstr = ""
                        PomPolja = PomPolja & tar2(j).Replace("desc", "").Replace("asc", "").Replace("%", "") & " like '" & Lstr & tar(i) & Dstr & "' or "
                    End If
                Next
                PomPolja = PomPolja.Substring(0, PomPolja.Length - 3) & " ) "
                sstr = sstr & separator & PomPolja
                separator = " and "
            Next

            sql = "select distinct " & Polja & " from " & Tablica & " where " & sstr & " " & wstr & " order by " & SortPolja
        End If


        'frm3.DataGridView1.DataSource = Class1.ExecSql(sql)
        'GridRowCount = frm3.DataGridView1.Rows.Count
        'Class1.FormatirajGrid(frm3.DataGridView1)

        Err.Clear()
        searchSQL5 = sql

        cls1.NapuniCBLog(" SEARCH SQL :   " & sql)

        Dim Trs As New DataTable
        Trs = cls1.ExecSql(sql)

        If Err.Number <> 0 Then
            GridRowCount = 0
            OKClick = True
            ID = "0"
            TrazilicaID = "0"
            TrazilicaIDstr = ""
            Err.Clear()
            Exit Sub
        End If

        GridRowCount = Trs.Rows.Count

        If GridRowCount > 0 Then
            cls1.CreateGridColumns(Trs, GridEX1)
            cls1.AddConditionalFormatting(GridEX1)
            GridEX1.SetDataBinding(Trs, "qwer")
            GridEX1.RootTable.Columns(1).Visible = False

            If strColumnAutoResize.ToLower = "false" Then
                GridEX1.ColumnAutoResize = False
            Else
                GridEX1.AutoSizeColumns()
                GridEX1.ColumnAutoResize = True
            End If




            






            On Error Resume Next
            Dim ImeFajla As String = strXMLfile2.Substring(strXMLfile2.LastIndexOf("\"), strXMLfile2.Length - strXMLfile2.LastIndexOf("\"))
            ImeFajla = ImeFajla.Replace("\", "").Trim
            ImeFajla = ImeFajla.Replace(".xml", "").Trim

            Dim LayoutDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\" + "GridTrazilica-" + ImeFajla + "-" + strImeForme + ".gxl"
            If My.Computer.FileSystem.FileExists(LayoutDir) Then
                Dim LayoutStream As FileStream
                LayoutStream = New FileStream(LayoutDir, FileMode.Open)
                GridEX1.LoadLayoutFile(LayoutStream)
                LayoutStream.Close()
            End If


            On Error Resume Next


            If Sender.strcolumncaption.trim <> "" Then
                tar2 = Sender.strcolumncaption.Split(",")
                i = 0
                For Each Column In GridEX1.RootTable.Columns
                    If i > 2 Then
                        If i - 3 <= tar2.GetUpperBound(0) Then
                            Column.caption = tar2(i - 3)
                        End If
                    End If
                    i = i + 1
                Next
            End If

            If Sender.strcolumnwidth.trim <> "" Then
                tar2 = Sender.strcolumnwidth.Split(",")
                i = 0
                For Each Column In GridEX1.RootTable.Columns
                    If i > 2 Then
                        If i - 3 <= tar2.GetUpperBound(0) Then
                            Column.width = tar2(i - 3)
                            If Val(tar2(i - 3)) = 0 Then
                                Column.visible = False
                            End If
                        End If
                    End If
                    i = i + 1
                Next
            End If

            PrevediSadrzajGrida(GridEX1)

        End If

        Dim tNaziv As String = ""

        If GridRowCount = 1 Then
            Odaberi()

            On Error Resume Next

            strParametriIzTrazilice = ""

             
            Dim r As DataRow
            For Each r In Trs.Rows
                For Each Column In Trs.Columns
                    Err.Clear()

                    If r(Column.Caption) Is Nothing Or r(Column.Caption) Is DBNull.Value Then

                    Else

                        Dim strImeParametra As String = Column.Caption
                        If Val(strImeParametra.Substring(strImeParametra.Length - 4, 4)) <> 0 Then
                            strImeParametra = strImeParametra.Substring(0, strImeParametra.Length - 4)
                        End If

                        If IsNumeric(r(Column.Caption)) Then
                            'strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & frm3.GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString.Replace(",", ".") & "' , "
                            strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & strImeParametra.Replace(" ", "_") & "='" & r(Column.Caption).ToString.Replace(",", ".") & "' , "
                        Else
                            'strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & frm3.GridEX1.CurrentRow.Cells(i).Column.Caption.ToString.Replace(" ", "_") & "='" & frm3.GridEX1.CurrentRow.Cells(i).Value.ToString & "' , "
                            strParametriIzTrazilice = strParametriIzTrazilice & "@SEARCH" & strImeParametra.Replace(" ", "_") & "='" & r(Column.Caption).ToString.Trim & "' , "
                        End If

                    End If

                Next
                Exit For
            Next


            If ID = "0" Then

                ID = Trs(0)(0)
                TrazilicaID = ID.Trim
                TrazilicaIDstr = Trs(0)(1) ' & " (" & ID & ")"



                'TrazilicaIDstr = Trs(0)(2)
                Err.Clear()
                tNaziv = Trs(0)("naziv")

                ' 7.1.2010 zbog Pekija ...
                For i4 = 1 To Trs.Columns.Count - 1
                    If Trs.Columns(i4).Caption.ToLower.StartsWith("naziv") Then
                        tNaziv = Trs(0)(i4)
                    End If
                Next i4

                If tNaziv.Trim <> "" Then
                    TrazilicaIDstr = tNaziv
                End If





            End If
        End If

        If GridRowCount = 0 Then
            OKClick = True
            ID = "0"
            TrazilicaID = "0"
            TrazilicaIDstr = ""
        End If

    End Sub


    Private Sub Form3_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

        If Translated = False Then
            TranslateForm(Me)
            Button1.Text = Translate(Button1.Text)
            Button2.Text = Translate(Button2.Text)
            Translated = True
        End If
        Me.Refresh()

        GridEX1.Focus()
    End Sub


    Private Sub Form3_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    'Function GetID2() As String
    '    GetID2 = "0"
    '    strID = ""
    '    If Me.DataGridView1.Rows.Count > 0 Then
    '        On Error Resume Next
    '        GetID2 = Me.DataGridView1.CurrentRow.Cells(0).Value.ToString
    '        strID = Me.DataGridView1.CurrentRow.Cells(1).Value.ToString
    '        strID = strID + "  " + Me.DataGridView1.CurrentRow.Cells(2).Value.ToString
    '    Else
    '        GetID2 = "0"
    '    End If
    'End Function

    Function GetID2() As String ' za janus
        GetID2 = "0"
        strID = ""
        Dim tNaziv As String = ""
        'If DataGridView1.Rows.Count > 0 Then
        If GridEX1.RowCount > 0 Then
            On Error Resume Next
            'GetID = DataGridView1.CurrentRow.Cells("id").Value.ToString.Trim
            GetID2 = GridEX1.CurrentRow.Cells("id").Value.ToString.Trim
            If Err.Number <> 0 Then
                'GetID = DataGridView1.CurrentRow.Cells(0).Value.ToString.Trim
                GetID2 = GridEX1.CurrentRow.Cells(2).Value.ToString.Trim
            End If

            strID = GridEX1.CurrentRow.Cells(3).Value.ToString.Trim
            Err.Clear()
            tNaziv = GridEX1.CurrentRow.Cells("naziv").Value.ToString.Trim

            ' 7.1.2010 zbog Pekija ...
            For i4 = 1 To GridEX1.RootTable.Columns.Count - 1
                If GridEX1.CurrentRow.Cells(i4).Column.Caption.ToLower.StartsWith("naziv") Then
                    tNaziv = GridEX1.CurrentRow.Cells(i4).Value.ToString.Trim
                End If
            Next i4

            If tNaziv.Trim <> "" Then
                strID = tNaziv
            End If

           



            'If tNaziv <> strID Then
            '    'strID = strID & "  " & GridEX1.CurrentRow.Cells("naziv").Value.ToString.Trim
            '    strID = GridEX1.CurrentRow.Cells("naziv").Value.ToString.Trim
            'End If

            'If Err.Number <> 0 Then
            '    'strID = strID & "  " & GridEX1.CurrentRow.Cells(4).Value.ToString.Trim
            '    strID = GridEX1.CurrentRow.Cells(4).Value.ToString.Trim
            '    tNaziv = GridEX1.CurrentRow.Cells(4).Value.ToString.Trim
            'End If

        Else
            GetID2 = "0"
        End If
    End Function


    Private Sub DataGridView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
        If e.KeyCode = Keys.Enter Then
            Odaberi()
            Me.Close()
        End If
    End Sub


    Private Sub GridEX1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GridEX1.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
        If e.KeyCode = Keys.Enter Then
            Odaberi()
            Me.Close()
        End If
    End Sub


    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        DeleteGridLayout()
    End Sub

    Private Sub butonzarazgrupiranje_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butonzarazgrupiranje.Click
        spremipostavke()
    End Sub


    Private Sub GridEX1_FormattingRow(sender As Object, e As Janus.Windows.GridEX.RowLoadEventArgs) Handles GridEX1.FormattingRow

    End Sub
End Class