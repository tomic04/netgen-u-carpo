Public Class Form7

    Public cls1 As Class1

    Public Sub NapuniGrid()
        On Error Resume Next
        Dim Trs As DataTable

        Dim sql As String
        sql = "select j.Naziv, j.Oznaka, isnull(j.Pismo, 'lat') as Pismo from Jezici j where j.Oznaka is not null and exists (     select *   from TotalGenLang l    where l.lang = j.Oznaka) "
        cls1 = New Class1
        Trs = cls1.ExecSql(sql)

        If Err.Number <> 0 Then

            Err.Clear()
            Exit Sub
        End If


        cls1.CreateGridColumns(Trs, GridEX1)
        cls1.AddConditionalFormatting(GridEX1)
        GridEX1.SetDataBinding(Trs, "qwer")
        GridEX1.RootTable.Columns(1).Visible = False
        GridEX1.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Language = ""
        strPismo = ""
        GetID2()

        If CheckBox1.Checked Then
            strMultilingual = "3"
        End If

        If CheckBox2.Checked Then
            strPismo = "cir"
        End If

        Me.Close()

    End Sub


    Function GetID2() As String ' za janus
        GetID2 = ""
        strPismo = ""
        Language = ""
        If GridEX1.RowCount > 0 Then
            On Error Resume Next
            Language = GridEX1.CurrentRow.Cells("oznaka").Value.ToString.Trim
            strPismo = GridEX1.CurrentRow.Cells("pismo").Value.ToString.Trim

        End If
        If Language <> "" Then
            strMultilingual = "2"
        End If
    End Function

End Class