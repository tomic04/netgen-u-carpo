using System;
using System.Data;
using System.Windows.Forms;

namespace Carpo.Specific.Forms
{
    public partial class Search : DevExpress.XtraEditors.XtraForm
    {
        public int? SearchId = null;
        private string connectionString = "";
        private DataTable dataTable;
        private string searchfield = "";

        public Search(DataTable DataTable, string ConnectionString, string SearchField)
        {
            InitializeComponent();

            this.connectionString = ConnectionString;
            this.dataTable = DataTable;
            this.searchfield = SearchField;

            Populate();
            gvSearch.Focus();
        }

        private void Search_Load(object sender, EventArgs e)
        {
            BoldFirstColumn();
            gvSearch.BestFitColumns();
        }

        private void Populate()
        {
            gvSearch.BeginUpdate();
            try
            {
                gcSearch.DataSource = dataTable;
                SetColumns();
                //Grid.FormatGrid(gridView1, false);
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                gvSearch.EndUpdate();
            }
        }

        private void SetColumns()
        {
            try
            {
                for (int i = 0; i < gvSearch.Columns.Count; i++)
                {
                    var column = gvSearch.Columns[i];
                    if (column.FieldName.ToLower().EndsWith("id"))
                        column.Visible = false;
                    //else if (string.IsNullOrEmpty(column.Caption) && definition.NetGenFields == true)
                    //    column.Caption = Common.Helpers.Grid.NetGenField(column.FieldName, definition.FieldSmartSpaces);
                }
            }
            catch { }
        }

        private void BoldFirstColumn()
        {
            if (gvSearch.VisibleColumns.Count > 0)
            {
                gvSearch.VisibleColumns[0].AppearanceCell.Font = new System.Drawing.Font(gvSearch.VisibleColumns[0].AppearanceCell.Font, System.Drawing.FontStyle.Bold);
                gvSearch.VisibleColumns[0].AppearanceCell.Options.UseFont = true;
            }
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }

        private void btnPotvrdi_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void gvSearch_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                SearchId = Convert.ToInt32(gvSearch.GetRowCellValue(e.FocusedRowHandle, searchfield));
            }
            catch { }
        }
    }
}