using System;

namespace Carpo.TestShell.Forms
{
    public partial class Help : DevExpress.XtraEditors.XtraForm
    {
        private int Id = 0;

        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            PopulateMasterElement();
        }

        private void PopulateMasterElement()
        {
            gcElem.DataSource = Carpo.Data.Get.HelpGetElements.GetElements();

            if (!(gvElem.RowCount > 0))
                return;

            var objId = gvElem.GetRowCellValue(gvElem.FocusedRowHandle, "CarpoDsElementId");
            Id = Convert.ToInt32(objId);
        }

        private void gvElem_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var objId = gvElem.GetRowCellValue(gvElem.FocusedRowHandle, "CarpoDsElementId");
            Id = Convert.ToInt32(objId);

            // Child elementi
            gcElementsChild.DataSource = Carpo.Data.Get.HelpGetElements.GetOnlyChildElements(Id);

            // Atributi
            gcOther.DataSource = Carpo.Data.Get.HelpGetElements.GetElementAttributes(Id);
            SetDescription();
        }

        private void gvOther_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            SetDescription();
        }

        private void SetDescription()
        {
            object objDesc = gvOther.GetRowCellValue(gvOther.FocusedRowHandle, "Description");
            if (objDesc != null)
                memoEdit1.Text = objDesc.ToString();
            else
                memoEdit1.Text = "";
        }
    }
}