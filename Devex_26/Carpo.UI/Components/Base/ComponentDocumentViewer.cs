using Carpo.Structure.Models.Core;
using Carpo.UI.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPdfViewer;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    internal class ComponentDocumentViewer : ComponentBase
    {
        SpreadsheetControl sheet;
        RichEditControl rich;
        PdfViewer pdf;
        JDocumentViewer definitions;
        PictureEdit picture;
        CancellationTokenSource cancellationTokenSource;
        public ComponentDocumentViewer(Control control, object def) : base(control, def)
        {
            sheet = new SpreadsheetControl();
            sheet.ReadOnly = true;

            pdf = new PdfViewer();
            pdf.NavigationPaneInitialVisibility = PdfNavigationPaneVisibility.Hidden;
            pdf.ReadOnly = true; ;

            rich = new RichEditControl();
            rich.ReadOnly = true;

            picture = new PictureEdit();

            picture.BackColor = Color.Transparent;
            picture.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            picture.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;

            definitions = def as JDocumentViewer;
        }
        public override void RefreshValue()
        {
            LoadDocument();
        }
        static void ShowTables(DataSet ds)//samo debug
        {
            var form = new XtraForm { Text = "docV dump", Size = new Size(900, 500) };
            if (ds == null || ds.Tables.Count == 0)
            {
                form.Controls.Add(new LabelControl { Text = "DataSet je prazan", Dock = DockStyle.Fill });
                form.ShowDialog();
                return;
            }

            var tabs = new DevExpress.XtraTab.XtraTabControl { Dock = DockStyle.Fill };
            foreach (DataTable t in ds.Tables)
            {
                var page = tabs.TabPages.Add($"{t.TableName} ({t.Rows.Count})");
                var grid = new DevExpress.XtraGrid.GridControl { Dock = DockStyle.Fill };
                var view = new DevExpress.XtraGrid.Views.Grid.GridView(grid);
                grid.MainView = view;
                view.OptionsBehavior.Editable = false;
                grid.DataSource = t;
                page.Controls.Add(grid);
            }
            form.Controls.Add(tabs);
            form.ShowDialog();
        }
        public void LoadDocument()
        {

            var core = new Logic.Data.Core()
            {
                ConnectionString = form.ConnectionString,
                ParameterControlValue = "",
                CurrentForm = form
            };

            if (definitions == null) return;

            var data = core.GetDataByName(definitions.DataName);

            DataSet dataSet = core.GetDataSet(data, "docV", out bool stop);
            
            //ShowTables(dataSet);
            var row = dataSet.Tables[0].Rows[0];

            var name = row.Field<string>(definitions.ColumnName);
            var file = row.Field<byte[]>(definitions.ColumnData);


            var repixID = row.Field<int?>(definitions.ColumnRepixID);
            var repixName = row.Field<string>(definitions.ColumnRepixName);

            control.Controls.Clear();

            if (!string.IsNullOrEmpty(repixName))
            {
                byte[] repxBytes;
                using (var repxClass = new Carpo.Common.Helpers.AssemblyLoad())
                {
                    repxBytes = (byte[])repxClass.Load("rptDevRepx", "rptDevRepx.clsReport", "GetBytes", new List<object>() { connectionString, repixID, "", repixName, "", "" });
                }

                using var memory = new MemoryStream(repxBytes);
                pdf.LoadDocument(memory);
                pdf.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                control.Controls.Add(pdf);
                pdf.Dock = DockStyle.Fill;

                return;
            }

            switch (name)
            {
                case var _ when name.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase) > 0:
                    {
                        sheet.LoadDocument(file);
                        control.Controls.Add(sheet);
                        sheet.Dock = DockStyle.Fill;
                        break;
                    }

                case var _ when name.IndexOf(".docx", StringComparison.OrdinalIgnoreCase) > 0:
                    {
                        rich.LoadDocument(file);
                        control.Controls.Add(rich);
                        rich.Dock = DockStyle.Fill;
                        break;

                    }
                case var _ when name.IndexOf(".pdf", StringComparison.OrdinalIgnoreCase) > 0:
                    {
                        var memory = new MemoryStream(file);

                        if (PdfMacRepair.RequiresRepair(memory)) memory = PdfMacRepair.Repair(memory);

                        pdf.LoadDocument(memory);
                        pdf.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
                        control.Controls.Add(pdf);
                        pdf.Dock = DockStyle.Fill;
                        break;
                    }
                case var _ when name.IndexOf(".jpg", StringComparison.OrdinalIgnoreCase) > 0 || name.IndexOf(".jepg", StringComparison.OrdinalIgnoreCase) > 0 || name.IndexOf(".bmp", StringComparison.OrdinalIgnoreCase) > 0 || name.IndexOf(".png", StringComparison.OrdinalIgnoreCase) > 0 || name.IndexOf(".gif", StringComparison.OrdinalIgnoreCase) > 0:
                    {
                        var memory = new MemoryStream(file);

                        picture.Image = Image.FromStream(memory);
                        control.Controls.Add(picture);
                        picture.Dock = DockStyle.Fill;
                        break;
                    }
            }


        }
    }
}
