using DevExpress.Drawing.Printing;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace Carpo.Common.Helpers
{
    public class Printing : IDisposable
    {
        private Dictionary<string, string> reportOptions = new Dictionary<string, string>();
        public string Caption { get; set; } = "";
        public string CaptionAlign { get; set; }
        public string Header { get; set; } = "";
        public string HeaderAlign { get; set; }
        public string SubHeader { get; set; } = "";
        public string SubHeaderAlign { get; set; }

        public string operaterNaziv = "";
        public string tvrtkaPodaci = "";
        private int rowNum = 1;

        public void PrintControl(IPrintable iPrintableControl)
        {
            using (PrintingSystem ps = new PrintingSystem())
            {
                ps.ShowPrintStatusDialog = false;
                using (PrintableComponentLink link = new PrintableComponentLink(ps))
                {
                    link.PaperKind = DXPaperKind.A4;
                    link.Landscape = true;
                    //link.EnablePageDialog = true;

                    //konvertiraj inch u mm
                    int margin = Convert.ToInt32((1 / 2.54) * 100);
                    link.Margins = new Margins(margin, margin, margin, margin);
                    link.Component = iPrintableControl;
                    link.CreateDocument();
                    Settings(ps);
                    link.CreateDocument();
                    link.ShowPreviewDialog();
                }
            }
        }

        public object CustomPrinting(GridControl gridcontrol, string tvrtka, string operater, bool isLandscape = false)
        {
            operaterNaziv = operater;
            tvrtkaPodaci = tvrtka;
            SubHeaderNewRowSettings();

            PrintingSystem ps = new PrintingSystem();
            ps.ShowPrintStatusDialog = false;

            PrintableComponentLink link = new PrintableComponentLink(ps);
            link.PaperKind = DXPaperKind.A4;
            link.Component = gridcontrol;
            link.Landscape = isLandscape;
            link.Margins = new System.Drawing.Printing.Margins(ReturnMm(100), ReturnMm(100), ReturnMm(100), ReturnMm(130));

            // Header - unutar ReportHeaderArea
            link.CreateReportHeaderArea += new CreateAreaEventHandler(pl_CreateReportHeaderArea);
            // Footer - unutar MarginalFooterArea | na njega ne utječe scale
            link.CreateMarginalFooterArea += new CreateAreaEventHandler(pl_CreateMarginalFooterArea);

            // defaultni TextExportMode postavljen u Text jer nije datumske vrijednosti ispravno exportao u XLSX - Tomislav
            ps.ExportOptions.Xlsx.TextExportMode = TextExportMode.Text;
            ps.ExportOptions.Xls.TextExportMode = TextExportMode.Text;

            link.CreateDocument();
            Settings(ps);
            link.CreateDocument();

            return link;
        }

        private void Settings(PrintingSystem ps)
        {
            ps.Document.AutoFitToPagesWidth = 1;
            float scaleFactor = ps.Document.ScaleFactor;
            if (scaleFactor > 1)
            {
                ps.Document.AutoFitToPagesWidth = 0;
                ps.Document.ScaleFactor = 1;
            }
        }

        public void SetPrintPageOptions(GridView gc)
        {
            //gc.OptionsPrint.PrintVertLines = reportOptions.ContainsKey("printvertlines");
            gc.OptionsPrint.PrintVertLines = true;
            gc.OptionsPrint.PrintHorzLines = !reportOptions.ContainsKey("printhorzlines");
            gc.OptionsPrint.PrintFooter = reportOptions.ContainsKey("printfooter");
            gc.OptionsPrint.UsePrintStyles = !reportOptions.ContainsKey("useprintstyles");
            gc.OptionsPrint.PrintHeader = !reportOptions.ContainsKey("printheader");
        }

        private void pl_CreateReportHeaderArea(object sender, CreateAreaEventArgs e)
        {
            int headerSize = 0; // u slučaju da se pošalju samo SubHeaderi
            //HEADER
            if (Caption != "")
            {
                TextBrick brickHeader = e.Graph.DrawString(Caption, Color.Black,
               new RectangleF(0, 0, e.Graph.ClientPageSize.Width, 20), BorderSide.None);
                SetBrickAlignment(brickHeader, CaptionAlign);
                brickHeader.Font = new Font("Arial", 12, FontStyle.Bold);
                headerSize = 21;
            }
            //SUBHEADER(S)
            if (Header != "" && SubHeader != "")
            {
                TextBrick brickSubHeader = e.Graph.DrawString(Header, Color.Gray,
                    new RectangleF(0, headerSize, HeaderHelper(e, "length"), 17 * rowNum), BorderSide.None);
                SetBrickAlignment(brickSubHeader, HeaderAlign);
                brickSubHeader.Font = new Font("Arial", 11, FontStyle.Italic);

                TextBrick brickSubHeader2 = e.Graph.DrawString(SubHeader, Color.Gray,
               new RectangleF(HeaderHelper(e, "position"), headerSize, HeaderHelper(e, "length"), 17 * rowNum), BorderSide.None);
                SetBrickAlignment(brickSubHeader2, SubHeaderAlign);
                brickSubHeader2.Font = new Font("Arial", 11, FontStyle.Italic);
            }
            else if (Header != "")
            {
                TextBrick brickSubHeader = e.Graph.DrawString(Header, Color.Gray,
               new RectangleF(0, headerSize, Convert.ToInt32(e.Graph.ClientPageSize.Width), 17 * rowNum), BorderSide.None);
                SetBrickAlignment(brickSubHeader, HeaderAlign);
                brickSubHeader.Font = new Font("Arial", 11, FontStyle.Italic);
            }
        }

        private void pl_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            string vrijeme = DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss");
            string pageText = "Stranica {0}/{1}";
            string operater = "Operater: " + operaterNaziv;

            //footer lijevo: Tvrtka | Operater
            TextBrick brickTvrtka = e.Graph.DrawString(tvrtkaPodaci, Color.DarkGray,
               new RectangleF(0, 1, FooterHelper(e, "footer_sidebricks_width"), 10), BorderSide.None);
            brickTvrtka.HorzAlignment = HorzAlignment.Near;
            brickTvrtka.Font = new Font("Arial", 7, FontStyle.Italic);

            TextBrick brickOperater = e.Graph.DrawString(operater, Color.DarkGray,
               new RectangleF(0, 11, FooterHelper(e, "footer_sidebricks_width"), 10), BorderSide.None);
            brickOperater.HorzAlignment = HorzAlignment.Near;
            brickOperater.Font = new Font("Arial", 7, FontStyle.Italic);

            //footer sredina: Jupiter logo
            ImageBrick brickImage = e.Graph.DrawImage(new Bitmap(Common.Properties.Resources.JupiterLogo),
                new RectangleF(FooterHelper(e, "center_widthlocation"), 1, 41, 24));
            brickImage.Sides = BorderSide.None;

            //footer desno: Broj stranice | Vrijeme
            PageInfoBrick pageInfoBrick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, pageText, Color.DimGray,
                new RectangleF(FooterHelper(e, "right_widthlocation"), 1, FooterHelper(e, "footer_sidebricks_width"), 14), BorderSide.None);
            pageInfoBrick.Font = new Font("Arial", 7);
            pageInfoBrick.Alignment = BrickAlignment.Far;
            pageInfoBrick.AutoWidth = true;
            pageInfoBrick.Padding = DevExpress.XtraPrinting.PaddingInfo.Empty;

            TextBrick brickVrijeme = e.Graph.DrawString(vrijeme, Color.DarkGray,
               new RectangleF(FooterHelper(e, "right_widthlocation"), 15, FooterHelper(e, "footer_sidebricks_width") - 4, 10), BorderSide.None);
            brickVrijeme.HorzAlignment = HorzAlignment.Far;
            brickVrijeme.Font = new Font("Arial", 7, FontStyle.Italic);
        }

        #region Helpers

        private int ReturnMm(int i)
        {
            if (!(i > 100))
                i = 100;
            int cm = Convert.ToInt32((1 / 2.54) * i);
            return cm;
        }

        private int HeaderHelper(EventArgs ev, string helperStr)
        {
            int i = 0;
            CreateAreaEventArgs e = ev as CreateAreaEventArgs;
            int width = Convert.ToInt32(e.Graph.ClientPageSize.Width);
            switch (helperStr)
            {
                case "length":
                    if (width % 2 == 1) // širina je neparna
                        i = width / 2;
                    else
                        i = (width / 2) - 1;
                    break;

                case "position":
                    i = (width / 2) + 1;
                    break;

                default:
                    break;
            }

            return i;
        }

        private int FooterHelper(EventArgs ev, string helperStr)
        {
            int i = 0;
            CreateAreaEventArgs e = ev as CreateAreaEventArgs;
            int width = Convert.ToInt32(e.Graph.ClientPageSize.Width); // 1047 | 719
            int height = Convert.ToInt32(e.Graph.ClientPageSize.Height);
            switch (helperStr)
            {
                case "footer_heightlocation":
                    i = height - 40;
                    break;

                case "footer_sidebricks_width":
                    i = (width - 53 - 2) / 2;
                    break;

                case "center_widthlocation":
                    i = ((width - 53 - 2) / 2) + 2;
                    break;

                case "right_widthlocation":
                    i = width - ((width - 53 - 2) / 2) + 1;
                    break;

                default:
                    break;
            }

            return i;
        }

        private void SubHeaderNewRowSettings()
        {
            int subNR = Header.Replace("\\n", "\r\n").Count(x => x == '\n');
            int subNR2 = SubHeader.Replace("\\n", "\r\n").Count(x => x == '\n');

            Header = Header.Replace("\\n", "\r\n");
            SubHeader = SubHeader.Replace("\\n", "\r\n");

            int numberOfRows = subNR > subNR2 ? subNR : subNR2;
            SetHeaderSize(numberOfRows + 1);
        }

        private void SetHeaderSize(int numberOfRows)
        {
            if (numberOfRows > 1)
                rowNum = numberOfRows;
        }

        private void SetBrickAlignment(TextBrick brick, string alignment)
        {
            switch (alignment)
            {
                case "left":
                    brick.HorzAlignment = HorzAlignment.Near;
                    break;

                case "right":
                    brick.HorzAlignment = HorzAlignment.Far;
                    break;

                case "center":
                    brick.HorzAlignment = HorzAlignment.Center;
                    break;
            }
        }

        #endregion Helpers

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            else
            {
                disposedValue = true;

                if (this != null)
                    this.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}