Imports System.Windows.Forms

Imports DevExpress.XtraPivotGrid
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrintingLinks
Imports System.Drawing
Imports DevExpress.Utils
Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.XtraReports.UI
Imports System.Data.SqlClient


Public Class frmIspis
    Private v_CrossGenLayoutId As Integer
    Private v_control As Object
    Private controlTip As String
    Private naslov As String = ""
    Private filterGlavni As String = ""
    Private filter As String = ""
    Private verzija As String = ""
    Private printableComponentLink1 As PrintableComponentLink = New PrintableComponentLink()

    Private composLink As New CompositeLink(New PrintingSystem())
    Private filterGlavniReport As New Link()
    Private filteriReport As New Link()
    Private HeaderArea As New Link()

    Private FooterArea As New Link()

    Private pageWidthInPixels As Integer
    Private pageHeightInPixels As Integer

    Private vSettings As String = ""
    Private v_Template As Integer = 1

    Public Property CrossGenLayoutId() As Integer
        Get
            Return v_CrossGenLayoutId
        End Get
        Set(ByVal value As Integer)
            v_CrossGenLayoutId = value
        End Set
    End Property

    Public Property Template() As Integer
        Get
            Return v_Template
        End Get
        Set(ByVal value As Integer)
            v_Template = value
        End Set
    End Property


    Public Sub New(ByVal control As Object, ByVal naslov As String, ByVal naslov2 As String, ByVal naslov3 As String)
        If controlTip = "grid" Then
            Me.v_control = TryCast(v_control, DevExpress.XtraGrid.GridControl)
        End If
        Me.controlTip = controlTip
        AddHandler composLink.CreateMarginalHeaderArea, AddressOf CreateMarginalHeaderArea


        InitializeComponent()

        Me.v_control = control
        If Template = 2 Then
            naslov = "Izvještaj: " & naslov
        End If
        Me.naslov = naslov
        Me.filterGlavni = filterGlavni
        Me.filter = filter
        Me.verzija = verzija

    End Sub

   

    Private Sub frmIspis_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        Dim pcLink1 As New PrintableComponentLink()

        pcLink1.Component = v_control
        PrintingSystem1.Links.AddRange(New Object() {composLink})


        Dim vMarginL As Integer = 60, vMarginR As Integer = 60, vMarginT As Integer = 60, vMarginB As Integer = 70
        Dim vLandscape As Boolean = True, vFitToPage As Byte = 0, vScaleFactor As Single = 1, vPaperKind As Integer = 1
        If controlTip = "chart" Then
            vFitToPage = 1
        Else
            vFitToPage = 0
        End If

      


        composLink.Links.Add(HeaderArea)
        composLink.Links.Add(filterGlavniReport)
        composLink.Links.Add(filteriReport)
        composLink.Links.Add(pcLink1)
        composLink.Landscape = vLandscape
        composLink.PaperKind = vPaperKind
        composLink.Margins.Left = vMarginL
        composLink.Margins.Right = vMarginR
        composLink.Margins.Top = vMarginT
        composLink.Margins.Bottom = vMarginB
        composLink.MinMargins.Bottom = 0
        composLink.CreateDocument()

        If vFitToPage <> 1 Then
            PrintingSystem1.Document.ScaleFactor = vScaleFactor
        Else
            PrintingSystem1.Document.AutoFitToPagesWidth = vFitToPage
        End If
    End Sub


    Private Sub frmIspis_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            DialogResult = Windows.Forms.DialogResult.Cancel
        End If
        Dim originalWidths As New List(Of Single)()

    End Sub

    Private Sub CreateMarginalHeaderArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
        e.Graph.Font = New Font("Tahoma", 9.0F, FontStyle.Bold)
        Dim sizeTmp As SizeF = e.Graph.MeasureString(Me.naslov)
        Dim size1 As SizeF = New SizeF(e.Graph.ClientPageSize.Width, sizeTmp.Height)
        e.Graph.BorderWidth = 0.0F
        e.Graph.DefaultBrickStyle.Padding = 0
        If Template = 2 Then
            e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
        Else
            e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Center).ChangeAlignment(StringAlignment.Center)
        End If
        Dim d As TextBrick = e.Graph.DrawString(naslov, New RectangleF(New PointF(0, 15), size1))

    End Sub

    ''Private Sub CreateReportHeaderArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    ''    Dim gr As BrickGraphics = PrintingSystem1.Graph
    ''    gr.Modifier = BrickModifier.ReportHeader
    ''    Dim sizePage As SizeF = e.Graph.MeasureString("testiram")
    ''    Dim pageNumberBrick As PageInfoBrick = e.Graph.DrawPageInfo(PageInfo.None, "testiram", Color.FromArgb(255, 150, 150, 150), New RectangleF(New SizeF(0, 4), sizePage), BorderSide.All)
    ''    pageNumberBrick.Alignment = BrickAlignment.Center
    ''    pageNumberBrick.VertAlignment = DevExpress.Utils.VertAlignment.Bottom
    ''    pageNumberBrick.AutoWidth = True
    ''End Sub


    ''Private Sub CreateMarginalFooterArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    ''    e.Graph.DrawImage(New Bitmap(My.Resources.logo_jupiter), New RectangleF(4, 4, 70, 38))
    ''    Dim v_verzija As String = "CrossGen v" & verzija
    ''    e.Graph.Font = New Font("Tahoma", 7.0F)
    ''    e.Graph.ForeColor = Color.FromArgb(255, 200, 0, 0)
    ''    Dim size1 As SizeF = e.Graph.MeasureString(v_verzija)
    ''    e.Graph.DrawString(v_verzija, New RectangleF(New SizeF(80, 4), size1))

    ''    Dim sizePage As SizeF = e.Graph.MeasureString("Stranica {0} / {1}")
    ''    Const PageNumberFormat As String = "Stranica {0} / {1}"
    ''    Dim pageNumberBrick As PageInfoBrick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, PageNumberFormat, Color.FromArgb(255, 150, 150, 150), New RectangleF(New SizeF(0, 4), sizePage), BorderSide.None)
    ''    pageNumberBrick.Alignment = BrickAlignment.Far
    ''    pageNumberBrick.AutoWidth = True

    ''    Dim sizeVrijeme As SizeF = e.Graph.MeasureString(Format(Now, "dd.MM.yyyy HH:mm"))
    ''    Dim pageVrijeme As PageInfoBrick = e.Graph.DrawPageInfo(PageInfo.DateTime, "{0:dd.MM.yyyy HH:mm}", Color.FromArgb(255, 150, 150, 150), New RectangleF(New SizeF(0, 4), sizeVrijeme), BorderSide.None)
    ''    pageVrijeme.Alignment = BrickAlignment.Center
    ''    pageVrijeme.AutoWidth = True
    ''End Sub

    'Private Sub CreateMarginalFooterArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    '    'left
    '    Dim sizeVrijeme As SizeF = e.Graph.MeasureString(Date.Now)
    '    Dim pageVrijeme As PageInfoBrick = e.Graph.DrawPageInfo(PageInfo.DateTime, "{0:dd.MM.yyyy HH:mm}", Color.FromArgb(255, 150, 150, 150), New RectangleF(New SizeF(0, 0), sizeVrijeme), BorderSide.None)
    '    pageVrijeme.Font = New Font("Tahoma", 7.0F)
    '    pageVrijeme.Alignment = BrickAlignment.Near
    '    pageVrijeme.HorzAlignment = DevExpress.Utils.HorzAlignment.Near
    '    pageVrijeme.AutoWidth = True



    '    Dim v_Firma As String = ""
    '    Try
    '        Dim dt As DataTable = dBase.GetRecordSetDataAdapterBezErr("exec spRptDocPartner")
    '        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
    '            Dim FNaziv As String = dt.Rows(0)("Naziv").ToString.Trim
    '            If FNaziv.Substring(Len(FNaziv) - 1, 1) = "." Then
    '                FNaziv = FNaziv.Substring(0, Len(FNaziv) - 1)
    '            End If
    '            Dim FAdresa As String = dt.Rows(0)("Adresa").ToString.Trim
    '            Dim FHPTBroj As String = dt.Rows(0)("HPTBroj").ToString.Trim
    '            Dim FMjesto As String = dt.Rows(0)("Mjesto").ToString.Trim

    '            v_Firma = FNaziv
    '            v_Firma += IIf(FNaziv <> "" And FAdresa <> "", ", ", "") & FAdresa
    '            v_Firma += IIf(v_Firma <> "" And (FHPTBroj & FMjesto) <> "", ", ", "") & Trim(FHPTBroj & " " & FMjesto)
    '        End If
    '    Catch ex As Exception

    '    End Try





    '    'center
    '    Dim oImageBrick As ImageBrick = e.Graph.DrawImage(New Bitmap(My.Resources.logo_jupiter), New RectangleF(e.Graph.ClientPageSize.Width / 2 - 30, 4, 60, 32))
    '    oImageBrick.SizeMode = ImageSizeMode.ZoomImage

    '    Dim v_verzija As String = "CrossGen v" & verzija
    '    e.Graph.Font = New Font("Tahoma", 6.0F)
    '    e.Graph.ForeColor = Color.FromArgb(255, 220, 0, 0)
    '    Dim sizeTmp As SizeF = e.Graph.MeasureString(v_verzija)
    '    Dim size1 As SizeF = New SizeF(e.Graph.ClientPageSize.Width, sizeTmp.Height + 10)
    '    e.Graph.BorderWidth = 0.0F
    '    e.Graph.DefaultBrickStyle.Padding = 0
    '    e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '    e.Graph.DrawString(v_verzija, New RectangleF(New PointF((size1.Width / 2) - (sizeTmp.Width / 2), 37), New Size(200, 11)))



    '        'Dim v_Firma As String = "SPIN Informatica d.o.o, 31000 Osijek"
    '        e.Graph.Font = New Font("Tahoma", 7.0F, FontStyle.Bold)
    '    e.Graph.ForeColor = Color.FromArgb(0, 0, 0)
    '    Dim sizeFirmaTmp As SizeF = e.Graph.MeasureString(v_Firma)
    '        e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '    e.Graph.DrawString(v_Firma, New Rectangle(0, 16, sizeFirmaTmp.Width + 100, sizeFirmaTmp.Height))


    '    'Dim d As TextBrick = e.Graph.DrawString(naslov, New RectangleF(New PointF(0, 15), size1))

    '    'Dim brick As DevExpress.XtraPrinting.TextBrick
    '    'brick = e.Graph.DrawString("My Footer", Color.Navy, New RectangleF(0, 0, e.Graph.ClientPageSize.Width, 40), DevExpress.XtraPrinting.BorderSide.None)
    '    'brick.Font = New Font("Tahoma", 16)
    '    'brick.StringFormat = New DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center)
    '    'e.Graph.DrawBrick(brick)


    '    If Template = 2 Then
    '        Try
    '            Dim SSB As SqlConnectionStringBuilder = New SqlConnectionStringBuilder(ConnStr)
    '            Dim dbName As String = SSB.InitialCatalog
    '            Dim imgLogo As Image = helpers.clsImage.Byte2Image(dBase.GetRecordValue("SELECT Logo From JupiterSystem.dbo.Firme WHERE DboNaziv='" & dbName & "'", ConnStr))
    '            'Dim imgLogo As Image = New Bitmap(My.Resources.logo_jupiter)
    '            Dim ratio As Decimal = Math.Min(150 / imgLogo.Width, 20 / imgLogo.Height)
    '            Dim wLogo = ratio * imgLogo.Width
    '            Dim hLogo = ratio * imgLogo.Height

    '            Dim oLogoBrick As ImageBrick = e.Graph.DrawImage(imgLogo, New RectangleF(0, 29, wLogo, hLogo))
    '            oLogoBrick.SizeMode = ImageSizeMode.ZoomImage
    '        Catch ex As Exception
    '        End Try
    '    End If





    '    'Dim sizeFirmaTmp As SizeF = e.Graph.MeasureString(v_Firma)
    '    'Dim sizeFirma1 As SizeF = New SizeF(e.Graph.ClientPageSize.Width, sizeFirmaTmp.Height)
    '    'e.Graph.BorderWidth = 0.0F
    '    'e.Graph.DefaultBrickStyle.Padding = 0
    '    'e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '    'e.Graph.DrawString(v_Firma, New RectangleF(New PointF(0, 37), sizeFirma1))


    '    'Dim graph As BrickGraphics = e.Graph

    '    'graph.DrawImage(New Bitmap(My.Resources.logo_jupiter), New RectangleF(e.Graph.ClientPageSize.Width / 2 - 30, 4, 60, 32))

    '    'graph.Modifier = BrickModifier.ReportHeader
    '    'graph.DrawString("test", New RectangleF(0, 0, 200, 20))

    '    'graph.StringFormat = New DevExpress.XtraPrinting.BrickStringFormat(StringAlignment.Center)

    '    'graph.Modifier = BrickModifier.MarginalFooter
    '    'graph.DrawString("marginalfooter", New RectangleF(0, 0, 150, 20))

    '    '' Set the modifier - specify the page area.
    '    'graph.Modifier = BrickModifier.MarginalHeader

    '    'Dim format As String = "Page {0} of {1}"
    '    'graph.Font = graph.DefaultFont
    '    'graph.BackColor = Color.Transparent

    '    'Dim r As New RectangleF(0, 0, 0, graph.Font.Height)

    '    '' Create a brick.
    '    'Dim brick As PageInfoBrick = graph.DrawPageInfo(PageInfo.NumberOfTotal, format, Color.Black, r, BorderSide.None)

    '    'brick.Alignment = BrickAlignment.Far
    '    'brick.AutoWidth = True






    '    'right
    '    Dim OperaterStr As String = ""
    '    Try
    '        Dim Operater = dBase.GetRecord("SELECT Naziv From ssOperateri WHERE OperateriId=" & OperaterId)
    '        If Not Operater Is Nothing Then
    '            OperaterStr = vbCrLf & "Operater: " & Operater.ToString()
    '        End If
    '    Catch ex As Exception
    '    End Try
    '    Dim sizePage As SizeF = e.Graph.MeasureString("Stranica {0} / {1}" & OperaterStr)

    '    Dim PageNumberFormat As String = "Stranica {0} / {1}" & OperaterStr
    '    Dim pageNumberBrick As PageInfoBrick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, PageNumberFormat, Color.FromArgb(255, 150, 150, 150), New RectangleF(New SizeF(0, 4), sizePage), BorderSide.None)
    '    pageNumberBrick.Font = New Font("Tahoma", 7.0F)
    '    pageNumberBrick.Alignment = BrickAlignment.Far
    '    pageNumberBrick.HorzAlignment = DevExpress.Utils.HorzAlignment.Far
    '    pageNumberBrick.LineAlignment = BrickAlignment.Near
    '    pageNumberBrick.AutoWidth = True
    'End Sub

    'Private Sub filteriReport_CreateDetailArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    '    If filter <> "" Then
    '        e.Graph.Font = New Font("Tahoma", 8.0F, FontStyle.Regular)
    '        e.Graph.ForeColor = Color.FromArgb(255, 150, 150, 150)
    '        e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '        Dim sizeNaslov As SizeF = e.Graph.MeasureString("Filteri: ")
    '        'sizeNaslov.Width.
    '        e.Graph.DrawString("Filteri: ", New RectangleF(Point.Empty, sizeNaslov))

    '        e.Graph.Font = New Font("Tahoma", 8.0F, FontStyle.Italic)
    '        Dim fnt As Font = New Font("Tahoma", 8.0F, FontStyle.Italic)
    '        e.Graph.ForeColor = Color.FromArgb(255, 150, 150, 150)
    '        e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '        Dim size2 As SizeF = e.Graph.MeasureString(filter, e.Graph.ClientPageSize.Width - sizeNaslov.Width - 10)
    '        e.Graph.DrawString(filter, New RectangleF(New PointF(sizeNaslov.Width + 10, 0), size2))
    '    End If
    'End Sub
    'Private Sub filterGlavniReport_BeforeCreateAreas(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    '    RecalcSize()
    'End Sub


    'Private Sub filterGlavniReport_CreateDetailArea(ByVal sender As Object, ByVal e As DevExpress.XtraPrinting.CreateAreaEventArgs)
    '    RecalcSize()

    '    'Dim size2 As SizeF = e.Graph.MeasureString(filterGlavni, e.Graph.ClientPageSize.Width)
    '    Dim sizeTmp As SizeF = e.Graph.MeasureString(filterGlavni)
    '    Dim size1 As SizeF = New SizeF(pageWidthInPixels, sizeTmp.Height)
    '    e.Graph.Font = New Font("Tahoma", 8.0F, FontStyle.Bold)
    '    e.Graph.ForeColor = Color.FromArgb(255, 150, 150, 150)
    '    If Template = 2 Then
    '        e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Near).ChangeAlignment(StringAlignment.Near)
    '    Else
    '        e.Graph.StringFormat = e.Graph.StringFormat.ChangeLineAlignment(StringAlignment.Center).ChangeAlignment(StringAlignment.Center)
    '    End If

    '    e.Graph.DrawString(filterGlavni, New RectangleF(Point.Empty, size1))
    'End Sub

    'Private Sub printingSystem1_AfterMarginsChange(ByVal sender As Object, ByVal e As MarginsChangeEventArgs) Handles PrintingSystem1.AfterMarginsChange
    '    RecalcSize()
    '    filterGlavniReport.CreateDocument()
    'End Sub

    'Private Sub printingSystem1_PageSettingsChanged(ByVal sender As Object, ByVal e As EventArgs) Handles PrintingSystem1.PageSettingsChanged
    '    RecalcSize()
    '    filterGlavniReport.CreateDocument()
    'End Sub

    'Private Sub RecalcSize()
    '    PrintingSystem1.Graph.PageUnit = GraphicsUnit.Pixel

    '    pageWidthInPixels = Convert.ToInt32(Math.Floor(PrintingSystem1.Graph.ClientPageSize.Width))
    '    pageHeightInPixels = Convert.ToInt32(Math.Floor(PrintingSystem1.Graph.ClientPageSize.Height))
    'End Sub

End Class