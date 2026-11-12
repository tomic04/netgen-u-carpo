using System;
using System.Data;
using System.Windows.Forms;

namespace Carpo.UI.Forms
{
    public partial class SearchDoc : Base.JupiterEditor
    {
        public DataRow FocusedRow
        {
            get
            {
                if (gridView1.FocusedRowHandle >= 0)
                    return gridView1.GetDataRow(gridView1.FocusedRowHandle);
                return null;
            }
        }

        private DataTable dataTable;

        public SearchDoc(Forms.Jupiter.Base form, DataTable dataTable)
        {
            if (this.DefinitionForm == null)
            {
                this.DefinitionForm = new Structure.Models.Core.JForm();
                this.DefinitionForm.Width = 850;
                this.DefinitionForm.Height = 550;
                this.DefinitionForm.Caption = Common.Translator.TranslateString("Dokumenti");
            }
            this.MinimumSize = new System.Drawing.Size(600, 500);

            this.Logger = form.Logger;
            this.ConnectionString = form.ConnectionString;

            InitializeComponent();
            pnlButtons.Visible = false;
            statusBar.Visible = false;

            this.dataTable = dataTable;

            TranslateControls();
            Populate();
            gridView1.Focus();
        }

        private void SearchDoc_Load(object sender, EventArgs e)
        {
            BoldFirstColumn();
            gridView1.BestFitColumns();
        }

        private void SearchDoc_KeyDown(object sender, KeyEventArgs e)
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

        private void Populate()
        {
            gridView1.BeginUpdate();
            try
            {
                gridControl1.DataSource = dataTable;
                SetColumns();
                //Grid.FormatGrid(gridView1, false);
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                gridView1.EndUpdate();
            }
        }

        private void TranslateControls()
        {
            this.Text = Common.Translator.TranslateString("Dokumenti");
            cmdPotvrdi.Text = Common.Translator.TranslateString("Potvrdi");
            cmdOdustani.Text = Common.Translator.TranslateString("Odustani");
        }

        private void cmdPotvrdi_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void cmdOdustani_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void BoldFirstColumn()
        {
            if (gridView1.VisibleColumns.Count > 0)
            {
                gridView1.VisibleColumns[0].AppearanceCell.Font = new System.Drawing.Font(gridView1.VisibleColumns[0].AppearanceCell.Font, System.Drawing.FontStyle.Bold);
                gridView1.VisibleColumns[0].AppearanceCell.Options.UseFont = true;
            }
        }

        private void SetColumns()
        {
            try
            {
                for (int i = 0; i < gridView1.Columns.Count; i++)
                {
                    var column = gridView1.Columns[i];
                    if (column.FieldName.ToLower().EndsWith("id"))
                    {
                        column.Visible = false;
                        continue;
                    }

                    Carpo.Common.Helpers.Grid.FormatColumn(column);
                }
            }
            catch { }
        }
    }
}