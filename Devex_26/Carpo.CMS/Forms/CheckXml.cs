using System;
using System.Drawing;

namespace Carpo.CMS.Forms
{
    public partial class CheckXml : DevExpress.XtraEditors.XtraForm
    {
        public int SetCnt
        {
            set { lblGreske.Text = "Broj grešaka: <b>" + value.ToString() + "</b>"; }
        }

        public CheckXml()
        {
            InitializeComponent();
        }

        private void SetGrid()
        {
            //gvCheck.OptionsBehavior.Editable = false;
            gvCheck.OptionsSelection.EnableAppearanceFocusedCell = false;
        }

        private void gvCheck_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            Models.CheckXml item = (gvCheck.GetRow(e.RowHandle) as Models.CheckXml);
            var element = item.Element;
            var atribute = item.Attribute;

            if (e.Column.FieldName == "Element" && atribute == null)
            {
                SetCellStyle(e);
            }
            else if (e.Column.FieldName == "Attribute" && atribute != null)
            {
                SetCellStyle(e);
            }
        }

        private void SetCellStyle(DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            e.Appearance.ForeColor = Color.FromArgb(200, 0, 0);
            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        }

        private void CheckXml_Load(object sender, EventArgs e)
        {
            SetGrid();
        }

        private void CheckXml_Shown(object sender, EventArgs e)
        {
            gvCheck.Columns[0].BestFit();
            gvCheck.Columns[1].BestFit();
            gvCheck.Columns[2].OptionsColumn.AllowEdit = false;
        }
    }
}