using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraPdfViewer;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Carpo.UI.Helpers;

namespace Carpo.UI.Components.Base
{
    public class ComponentPdfViewer : ComponentBase
    {
        private PdfViewer pdfViewer;
        private new JPdfViewer definition;

        public ComponentPdfViewer() : base()
        {
        }

        public ComponentPdfViewer(Control control, object def) : base(control, def)
        {
            definition = def as JPdfViewer;

            if (!(control is PdfViewer))
                return;

            pdfViewer = (control as PdfViewer);
            pdfViewer.NavigationPaneInitialVisibility = PdfNavigationPaneVisibility.Hidden;

            // SetEvents();
            Init();
        }

        private static Stream GetDocumentStream()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Stream stream = assembly.GetManifestResourceStream("Demo.pdf");
            if (stream == null)
                return assembly.GetManifestResourceStream("Carpo.UI.Data.Demo.pdf");
            return stream;
        }

        private void Init()
        {

            if (!string.IsNullOrEmpty(definition.dbpdf))
            {
                using (Parser parser = new Parser(form, null))
                {
                    var byteArray = parser.GetLogicPartValue(definition.dbpdf);
                    if (byteArray is byte[] && byteArray != null)
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(((byte[])byteArray));

                        if (PdfMacRepair.RequiresRepair(stream)) stream = PdfMacRepair.Repair(stream);

                        pdfViewer.LoadDocument(stream);
                        pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                    }
                }
            }

            if (!string.IsNullOrEmpty(definition.localpdf) && !string.IsNullOrEmpty(definition.localfolder))
                SetControlLocalPdf(definition.localpdf);
        }

        public void SetControlLocalPdf(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    pdfViewer = null;
                    return;
                }
                using (Parser parser = new Parser(form, null))
                {
                    var localFolder = parser.GetLogicPartValue(definition.localfolder);

                    if (file.StartsWith("~") || file.StartsWith("#") || file.StartsWith("$"))
                    {
                        var tempImg = parser.GetLogicPartValue(file);
                        if (tempImg == null)
                            return;
                        else
                        {
                            if (file.ToString().Contains(".pdf")) { definition.localpdf = file; }
                            else { definition.localpdf = file + ".pdf"; }
                        }
                    }
                    else
                    {
                        if (file.ToString().Contains(".pdf")) { definition.localpdf = file; }
                        else { definition.localpdf = file + ".pdf"; }

                        // definition.localpdf = file;
                    }

                    if (File.Exists(localFolder.ToString() + @"\\" + definition.localpdf))

                        pdfViewer.LoadDocument(localFolder.ToString() + @"\\" + definition.localpdf);
                    else
                        pdfViewer = null;
                }
            }
            catch (Exception ex)
            {
                pdfViewer = null;
                form.Logger.Warn("'" + definition.Name + "' - SetControlLocalImage()", ex.Message);
            }
        }

        internal void ChangeControlPdf(byte[] byteArray)
        {
            MemoryStream ms = new MemoryStream(byteArray);
            //pdfViewer. = ms;
            try
            {
                if (byteArray != null)
                    pdfViewer.LoadDocument(ms);
                //        PictureEdit.Image = ConvertToImage(byteArray);
                //    else
                //        PictureEdit.Image = null;
            }
            catch (Exception ex)
            {
                pdfViewer = null;
                form.Logger.Warn("'" + definition.Name + "' - ChangeControlImage()", ex.Message);
            }
        }
    }
}