using Carpo.Structure.Interfaces;
using System.ComponentModel;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public partial class JStandardButtons : UserControl
    {
        public JStandardButtons()
        {
            InitializeComponent();
        }

        private void bbiPrintGridMaster_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Print("master");
        }

        private void bbiPrintGridDetalji_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Print("detail");
        }

        private void bbiPrintGridTree_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Print("tree");
        }

        private void Print(string controlStr)
        {
            Form form = FindForm();
            if (form is Forms.Master.Manager)
            {
                if (controlStr == "master")
                    (((Forms.Master.Manager)form).gcGla as INotifyPrint).PrintControl(false);
                if (controlStr == "detail")
                    (((Forms.Master.Manager)form).gcDet as INotifyPrint).PrintControl(false);
                if (controlStr == "tree")
                    (((Forms.Master.Manager)form).trMain as INotifyPrint).PrintControl(false);
            }
        }

        private void popupIspisKontrola_BeforePopup(object sender, CancelEventArgs e)
        {
            Form form = this.FindForm();
            if (form is Forms.Master.Manager)
            {
                Forms.Master.Manager frmManager = form as Forms.Master.Manager;

                bool isReordering = false;

                if (frmManager.trMain != null && frmManager.trMain.ComponentExt != null)
                    isReordering = frmManager.trMain.ComponentExt.Action == "reordering";

                bbiPrintGridDetalji.Visibility = GetBarItemVisibility(frmManager.DetailGridVisible && isReordering == false);
                bbiPrintGridMaster.Visibility = GetBarItemVisibility(frmManager.MasterGridVisible && isReordering == false);
                bbiPrintGridTree.Visibility = GetBarItemVisibility(frmManager.TreeControlVisible);
            }
        }

        private DevExpress.XtraBars.BarItemVisibility GetBarItemVisibility(bool isVisible)
        {
            if (isVisible == true)
                return DevExpress.XtraBars.BarItemVisibility.Always;
            else
                return DevExpress.XtraBars.BarItemVisibility.Never;
        }
    }
}