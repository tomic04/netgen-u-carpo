using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.TestShell.Forms
{
    public partial class XmlHistory : DevExpress.XtraEditors.XtraForm
    {
        private List<Models.XmlHistory> datasourceList = new List<Models.XmlHistory>();
        public string ReturnXmlPath = "", ReturnServer = "", ReturnDatabase = "";
        public bool SendConnection = true;

        public XmlHistory(List<Models.XmlHistory> historyModel)
        {
            InitializeComponent();

            CopyModel(datasourceList, historyModel);
            datasourceList.Reverse(); // ovo ispitati

            Populate();
        }

        private void XmlHistory_Load(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void Populate()
        {
            gvHistory.BeginUpdate();
            try
            {
                gcHistory.DataSource = datasourceList;
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                gvHistory.EndUpdate();
            }

            if (gvHistory.RowCount == 0)
            {
                chkbKoristiKonekciju.Enabled = false;
                btnObrisiPovijest.Enabled = false;
                btnPotvrdi.Enabled = false;
            }
        }

        private void SetGrid()
        {
            SetColumns();
            BoldFirstColumn();
            gvHistory.BestFitColumns();
        }

        private void BoldFirstColumn()
        {
            if (gvHistory.VisibleColumns.Count > 0)
            {
                gvHistory.VisibleColumns[0].AppearanceCell.Font = new System.Drawing.Font(gvHistory.VisibleColumns[0].AppearanceCell.Font, System.Drawing.FontStyle.Bold);
                gvHistory.VisibleColumns[0].AppearanceCell.Options.UseFont = true;
            }
        }

        private void SetColumns()
        {
            gvHistory.Columns["Database"].Caption = "Baza";
            gvHistory.Columns["Time"].Caption = "Posljednje korišteno";
            gvHistory.Columns["Time"].DisplayFormat.FormatType = FormatType.DateTime;
            gvHistory.Columns["Time"].DisplayFormat.FormatString = "dd.MM.yyyy. (HH:mm)";

            GridColumn col = gvHistory.Columns["XmlPath"];
            col.Visible = false;
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.SelectedControl == gcHistory)
            {
                GridHitInfo hi = gvHistory.CalcHitInfo(e.ControlMousePosition);

                if (hi.InRowCell && hi.RowHandle >= 0)
                {
                    if (hi.Column.FieldName == "Xml")
                    {
                        int rowHandle = hi.RowHandle;
                        string ovdjeIdeTekst = datasourceList[rowHandle].XmlPath;
                        e.Info = new ToolTipControlInfo("w", ovdjeIdeTekst);
                    }
                }
            }
        }

        private void btnPotvrdi_Click(object sender, EventArgs e)
        {
            DialogResultOK();
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnObrisiPovijest_Click(object sender, EventArgs e)
        {
            DialogResult dr = XtraMessageBox.Show("Želite li obrisati cjelokupnu povijest?", "History", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                gcHistory.DataSource = null;
                Properties.Settings.Default.Xml.Clear();
                datasourceList.Clear();
                Populate();
            }
        }

        private void gvHistory_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            DoRowDoubleClick(view, pt);
        }

        private void DoRowDoubleClick(GridView view, Point pt)
        {
            GridHitInfo info = view.CalcHitInfo(pt);
            if (info.InRow || info.InRowCell)
                DialogResultOK();
        }

        private string SetReturnString(string field)
        {
            string returnS = "";

            try
            {
                var rowHandle = gvHistory.FocusedRowHandle;
                var obj = gvHistory.GetRowCellValue(rowHandle, field);
                returnS = obj.ToString();
            }
            catch { }

            return returnS;
        }

        private void XmlHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void DialogResultOK()
        {
            ReturnXmlPath = SetReturnString("XmlPath");
            ReturnServer = SetReturnString("Server");
            ReturnDatabase = SetReturnString("Database");
            SendConnection = chkbKoristiKonekciju.Checked;

            this.DialogResult = DialogResult.OK;
        }

        private void CopyModel(List<Models.XmlHistory> listTo, List<Models.XmlHistory> listFrom)
        {
            if (listTo.Count > 0)
                listTo.Clear();

            foreach (Models.XmlHistory el in listFrom)
            {
                listTo.Add(el);
            }
        }
    }
}