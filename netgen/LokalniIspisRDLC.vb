'Imports System
'Imports Microsoft.Reporting.WinForms
'Imports System.Runtime.InteropServices
'Imports System.IO
'Imports System.Data
'Imports System.Text
'Imports System.Drawing.Imaging
'Imports System.Drawing.Printing
'Imports System.Collections.Generic
'Imports System.Data.SqlClient
'Imports System.AppDomain

'Public Class LokalniIspisRDLC
'    Public Conn As SqlClient.SqlConnection
'    Public cls1 As Class1
'    Dim a() As String
'    Dim strPutanja As String = ""
'    Dim k As Integer = 0
'    Dim tpP As String = ""
'    Public strSQLSERVERlogin As String = ""
'    Public strSQLSERVERpassword As String = ""

'    Private deviceInfo As String = _
'      "<DeviceInfo>" + _
'      "  <OutputFormat>EMF</OutputFormat>" + _
'      "  <PageWidth>21cm</PageWidth>" + _
'      "  <PageHeight>29.7cm</PageHeight>" + _
'      "  <MarginTop>0cm</MarginTop>" + _
'      "  <MarginLeft>0.2cm</MarginLeft>" + _
'      "  <MarginRight>0.2cm</MarginRight>" + _
'      "  <MarginBottom>0cm</MarginBottom>" + _
'      "</DeviceInfo>"

'    Private m_currentPageIndex As Integer
'    Private m_streams As IList(Of Stream)
'    Private m_Landscape As Boolean = False

'    Dim report1 As LocalReport = New LocalReport()


'    Public Sub Run(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String, ByVal ImeForme As String)


'        On Error Resume Next

'        'MsgBox(server)

'        Dim ds As DataSet = New DataSet


'        cls1 = New Class1
'        cls1.strServer = server
'        cls1.strBaza = database
'        cls1.OperateriID = Operateriid

'        Dim tstr5 As String = ""

'        strSQLSERVERlogin = username
'        strSQLSERVERpassword = password

'        If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
'        If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""

'        Conn = New SqlClient.SqlConnection

'        ImeAplikacijeZaProfiler = "IspisRDLC"

'        If strSQLSERVERlogin = "" Then
'            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";Trusted_Connection=true;Application Name=" & ImeAplikacijeZaProfiler & ";"
'        Else
'            tstr5 = "Server=" & cls1.strServer & ";Database=" & cls1.strBaza & ";UID=" & strSQLSERVERlogin & ";PWD=" & strSQLSERVERpassword & ";Application Name=" & ImeAplikacijeZaProfiler & ";"
'        End If


'        'MsgBox(tstr5)

'        Conn.ConnectionString = tstr5

'        Err.Clear()
'        Conn.Open()

'        If Err.Number <> 0 Then
'            MsgBox(Err.Description)
'            Exit Sub
'        End If


'        '------------------------------------------------------------------------------------------------

'        ImeForme = ImeForme.Replace("RDLC", "")


'        Dim b() As String

'        Dim strDataSet As String = ""

'        Dim Trs5 As New DataTable
'        Dim sql As String = ""
'        a = ImeForme.Split("~")

'        Dim frm4 As New Form4

'        For j = 0 To a.GetUpperBound(0)
'            tpP = a(j)
'            If a(j).Trim <> "" Then
'                k = k + 1
'                If k = 1 Then
'                    strPutanja = tpP
'                    frm4.ReportViewer1.LocalReport.ReportPath = strPutanja
'                    report1.ReportPath = My.Application.Info.DirectoryPath & "\MPRacun.rdlc"
'                    report1.ReportPath = strPutanja
'                    ' MsgBox("putanja : " & strPutanja)
'                Else
'                    b = tpP.Split("$")
'                    For i = 0 To b.GetUpperBound(0)
'                        If i = 0 Then
'                            strDataSet = b(i)
'                        End If
'                        If i = 1 Then
'                            sql = b(i)
'                            Err.Clear()
'                            Dim da As New SqlClient.SqlDataAdapter(sql, Conn)
'                            da.Fill(ds, "qwer")
'                            Trs5 = ds.Tables(0)

'                            ' MsgBox("rc  : " & Trs5.Rows.Count)
'                            frm4.ReportViewer1.LocalReport.DataSources.Add(New ReportDataSource(strDataSet, Trs5))

'                            report1.DataSources.Add(New ReportDataSource(strDataSet, Trs5))

'                            'MsgBox("dataset  : " & strDataSet)
'                        End If
'                    Next
'                End If
'            End If
'        Next

'        Export(report1)

'        m_currentPageIndex = 0
'        Print(1, "POS")





'        'frm4.ReportViewer1.LocalReport.EnableExternalImages = True
'        'frm4.ReportViewer1.LocalReport.EnableHyperlinks = True
'        'frm4.ReportViewer1.ZoomMode = 1
'        'frm4.ReportViewer1.SetDisplayMode(DisplayMode.Normal)
'        'frm4.ReportViewer1.Refresh()
'        'frm4.ShowDialog()








'        'report1.EnableExternalImages = True

'        'ako ima subreport
'        'report1.LoadSubreportDefinition(ImeSubreporta, Me.GetType.Assembly.GetManifestResourceStream("PrimjerSubreporta.Ispis.rdlc"))
'        'AddHandler report1.SubreportProcessing, AddressOf DemoSubreportProcessingEventHandler




'    End Sub



'    Private Function CreateStream(ByVal name As String, _
'       ByVal fileNameExtension As String, _
'       ByVal encoding As Encoding, ByVal mimeType As String, _
'       ByVal willSeek As Boolean) As Stream

'        Dim stream As Stream = _
'                New MemoryStream()

'        m_streams.Add(stream)
'        Return stream
'    End Function

'    Private Sub Export(ByVal report As LocalReport)

'        Dim warnings() As Warning = Nothing
'        m_streams = New List(Of Stream)()

'        report.Render("Image", deviceInfo, AddressOf CreateStream, _
'           warnings)

'        Dim stream As Stream
'        For Each stream In m_streams
'            stream.Position = 0
'        Next
'    End Sub

'    Private Sub PrintPage(ByVal sender As Object, _
'    ByVal ev As PrintPageEventArgs)
'        Dim pageImage As New Metafile(m_streams(m_currentPageIndex))
'        ev.Graphics.DrawImage(pageImage, ev.PageBounds)

'        m_currentPageIndex += 1
'        ev.HasMorePages = (m_currentPageIndex < m_streams.Count)
'    End Sub

'    Private Sub Print(ByVal Copies As Integer, ByVal Printer As String)
'        Dim printerName As String

'        printerName = Printer

'        If m_streams Is Nothing Or m_streams.Count = 0 Then
'            Return
'        End If

'        Dim printDoc As New PrintDocument()
'        printDoc.PrinterSettings.PrinterName = printerName
'        If Not printDoc.PrinterSettings.IsValid Then
'            MsgBox("Direct print greška.")
'        End If
'        AddHandler printDoc.PrintPage, AddressOf PrintPage
'        printDoc.PrinterSettings.Copies = Copies
'        printDoc.DefaultPageSettings.Landscape = m_Landscape

'        printDoc.PrintController = New StandardPrintController()

'        printDoc.Print()
'    End Sub


'End Class
