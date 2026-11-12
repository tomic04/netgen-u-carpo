
'Imports Microsoft.Reporting.WinForms

'Public Class Form4

'    Public Sub PrintRpt() 'frmReport_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

'        Dim siteUri As New Uri("http://jupiter/reportserver$sql2005")


'        Dim par1 As New ReportParameter("broj1", _
'                "2006", False)
'        Dim par2 As New ReportParameter("hrkadroviid", _
'                "0", False)
'        Dim par3 As New ReportParameter("ID", _
'            "0", False)
'        Dim par4 As New ReportParameter("combo1", _
'            "0", False)
'        Dim Parametri(3) As ReportParameter


'        Parametri(0) = par1
'        Parametri(1) = par2
'        Parametri(2) = par3
'        Parametri(3) = par4

'        Me.ReportViewer1.ProcessingMode = ProcessingMode.Remote
'        Me.ReportViewer1.ServerReport.ReportServerUrl = siteUri
'        Me.ReportViewer1.ServerReport.ReportPath = "/ProbniReporti/rptOOPPlace/rptOOPPK1_2005"

'        Me.ReportViewer1.ServerReport.SetParameters(Parametri)
'        Me.ReportViewer1.RefreshReport()
'        Me.ShowDialog()
'    End Sub

'    Public Sub LocalPrintRpt(ByVal Putanja As String, ByVal table1 As DataTable, ByVal table2 As DataTable)
'        Me.ReportViewer1.LocalReport.ReportPath = Putanja
'        Me.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("DataSet1", table1))
'        'Me.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource("table2", table2))
'        Me.ReportViewer1.ZoomMode = 1
'        Me.ReportViewer1.SetDisplayMode(DisplayMode.PrintLayout)
'        Me.ReportViewer1.Refresh()
'        Me.ShowDialog()
'    End Sub

'End Class