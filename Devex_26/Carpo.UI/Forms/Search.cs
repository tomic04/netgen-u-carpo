using Carpo.Common.Helpers;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Carpo.UI.Forms
{
    public partial class Search : Base.JupiterEditor
    {
        private DataTable dataTable;

        private JSearchEdit definition;

        public string ComponentName = "";

        public Search(Forms.Jupiter.Base form, DataTable dataTable, JSearchEdit definition, string searchFormCaption)
        {
            this.definition = definition;

            if (this.DefinitionForm == null)
            {
                this.DefinitionForm = new Structure.Models.Core.JForm();
                this.DefinitionForm.Caption = searchFormCaption;
                this.DefinitionForm.Name = definition.Name;
                ComponentName = definition.Name;
            }

            this.Logger = form.Logger;
            this.ConnectionString = form.ConnectionString;
            this.ModuleId = form.ModuleId;
            this.FormName = form.Name;
            this.DefinitionForm.Name = FormName;

            InitializeComponent();

            pnlButtons.Visible = false;
            statusBar.Visible = false;

            this.dataTable = dataTable;
            Populate();
            TranslateControls();

            try
            {
                if (!(string.IsNullOrEmpty(definition.SaveLayout)) && definition.SaveLayout.ToLower() == "true")
                {
                    GetLayout();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Grid - search: SetControlLayout", ex.Message, ex);
            }
            gridView1.Focus();
        }

        private object GetFormSize(Structure.Models.Vars layout, string control)
        {
            var formSizeStr = layout;
            if (formSizeStr == null)
                return null;

            if (formSizeStr.Value.ToLower() == "maximized")
                return true;

            string[] formSizeArr = formSizeStr.Value.ToString().Split(';');
            if (formSizeArr.Length == 2)
            {
                int? width = formSizeArr[0].CoalesceInt();
                int? height = formSizeArr[1].CoalesceInt();
                if (width != null && height != null)
                    return new System.Drawing.Size((int)width, (int)height);
            }

            return null;
        }

        private void TranslateControls()
        {
            this.Text = Common.Translator.TranslateString("Tražilica");
            cmdGridReset.Text = Common.Translator.TranslateString("Resetiraj grid");
            cmdPotvrdi.Text = Common.Translator.TranslateString("Potvrdi");
            cmdOdustani.Text = Common.Translator.TranslateString("Odustani");
        }

        private void GetLayout()
        {
            try
            {
                VarsLayout varsLayout = new VarsLayout(this);
                varsLayout.SetLayout(); // povuci nove vrijednosti iz baze
            }
            catch (Exception ex)
            {
                Logger.Error("Set layout - search grid", ex.Message, ex);
            }

            var cVar = (this).CarpoVarsLayout.Where(x => x.Name == definition.Name).FirstOrDefault();
            if (cVar != null)
            {
                var formSize = GetFormSize(cVar, definition.Name);
                if (formSize != null)
                {
                    if (formSize is Size)
                        this.Size = (Size)formSize;
                    else if (formSize is Boolean)
                        this.WindowState = FormWindowState.Maximized;
                }
                // može biti null kada je spremljena form size bez spremanja XMLa search forme, a naknadno dodan savelayout atribut
                if (!string.IsNullOrEmpty(cVar.ValueXml.ToString()))
                {
                    MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(cVar.ValueXml.ToString()));
                    gridControl1.DefaultView.RestoreLayoutFromStream(mStream);
                }
            }
            foreach (var col in gridView1.Columns.Where(x => x.FieldName.ToLower().EndsWith("id")))
            {
                col.Visible = false;
            }
        }

        private void Populate()
        {
            gridView1.BeginUpdate();
            try
            {
                gridControl1.DataSource = dataTable;

                if (!(string.IsNullOrEmpty(definition.SaveLayout)) && definition.SaveLayout.ToLower() == "true")
                {
                    if (definition.NetGenFields == true)
                    {
                        foreach (var column in gridView1.Columns.Where(x => x.Visible == true))
                        {
                            if (string.IsNullOrEmpty(column.Caption) && definition.NetGenFields == true)
                                column.Caption = Common.Helpers.Grid.NetGenField(column.FieldName, definition.FieldSmartSpaces);
                        }
                    }
                }
                else
                    SetColumns();
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

        private void Search_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
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

        private void cmdPotvrdi_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void cmdOdustani_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void Search_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(definition.SaveLayout) || (!(string.IsNullOrEmpty(definition.SaveLayout)) && definition.SaveLayout.ToLower() != "true"))
                BoldFirstColumn();

            gridView1.BestFitColumns();
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
                    else if (string.IsNullOrEmpty(column.Caption) && definition.NetGenFields == true)
                    {
                        column.Caption = Common.Helpers.Grid.NetGenField(column.FieldName, definition.FieldSmartSpaces);
                    }

                    Carpo.Common.Helpers.Grid.FormatColumn(column);
                }
            }
            catch { }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void Search_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(string.IsNullOrEmpty(definition.SaveLayout)) && definition.SaveLayout.ToLower() == "true" && gridView1.Columns.Count != 0)
            {
                if (!(string.IsNullOrEmpty(definition.ClearGridFilter)) && definition.ClearGridFilter.ToLower() == "true")
                {
                    try
                    {
                        gridView1.ClearColumnsFilter();
                    }
                    catch (Exception)
                    {
                        Logger.Warn(definition.Name + " - ClearColumnsFilter", "Greška čišćenja filtera grida");
                    }
                }
                Logic.VarsLayout layout = new Logic.VarsLayout(this as Carpo.UI.Forms.Base.JupiterShared);
                layout.SaveXml(gridView1, "grid - search", definition.Name);
            }
        }

        private void cmdGridReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                gridView1.Columns[i].Visible = true;
                gridView1.Columns[i].VisibleIndex = i;
                if (gridView1.Columns[i].FieldName.ToLower().EndsWith("id"))
                    gridView1.Columns[i].Visible = false;
                else if (string.IsNullOrEmpty(gridView1.Columns[i].Caption) && definition.NetGenFields == true)
                    gridView1.Columns[i].Caption = Common.Helpers.Grid.NetGenField(gridView1.Columns[i].FieldName, definition.FieldSmartSpaces);

                gridView1.BestFitColumns();
            }
        }
    }
}