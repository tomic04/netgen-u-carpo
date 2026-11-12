using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class ElementsStructure : Form
    {
        public static bool state = false;

        public ElementsStructure()
        {
            InitializeComponent();
        }

        private void ElementsStructure_Load(object sender, EventArgs e)
        {
            PopulateParentElements(null);
        }

        private void PopulateElements(int? rowhandle = null, int? toprowindex = null)
        {
            gcElem.DataSource = null;
            gcElem.DataSource = Data.Elements.GetElementsForParent(GetParentId());
            changeButtonState();

            if (toprowindex != null)
                gvElem.TopRowIndex = (int)toprowindex;
            if (rowhandle != null)
                gvElem.FocusedRowHandle = (int)rowhandle;
        }

        private void PopulateParentElements(int? id, int? rowhandle = null, int? toprowindex = null)
        {
            gcElemParent.DataSource = null;
            gcElemParent.DataSource = Data.Elements.GetParentsForElement(id);
            changeButtonState();

            if (toprowindex != null)
                gvElemParent.TopRowIndex = (int)toprowindex;
            if (rowhandle != null)
                gvElemParent.FocusedRowHandle = (int)rowhandle;
        }

        private void gvElemParent_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridElementParentFocusedRowChanged();
        }

        private void GridElementParentFocusedRowChanged()
        {
            if (pnlParentElements.Parent == splitContainerControl1.Panel1 && gvElemParent.FocusedRowHandle >= 0)
            {
                int toprowindex = gvElem.TopRowIndex;
                int rowhandle = gvElem.FocusedRowHandle;
                PopulateElements(rowhandle, toprowindex);
            }
        }

        private int GetParentId()
        {
            return Convert.ToInt32(gvElemParent.GetDataRow(gvElemParent.FocusedRowHandle)["CarpoDsElementId"]);
        }

        private int GetElementId()
        {
            return Convert.ToInt32(gvElem.GetDataRow(gvElem.FocusedRowHandle)["CarpoDsElementId"]);
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (pnlParentElements.Parent == splitContainerControl1.Panel1)
            {
                pnlParentElements.Parent = splitContainerControl1.Panel2;
                pnlElements.Parent = splitContainerControl1.Panel1;
                panel3.Visible = false;
                gvElemParent.Columns["Chk"].Visible = true;
                gvElemParent.Columns["Chk"].VisibleIndex = 0;
                gvElem.Columns["Chk"].Visible = false;
                btnSpremiParentElement.Visible = true;
                GridElementFocusedRowChanged();
            }
            else
            {
                pnlParentElements.Parent = splitContainerControl1.Panel1;
                pnlElements.Parent = splitContainerControl1.Panel2;
                panel3.Visible = true;
                gvElemParent.Columns["Chk"].Visible = false;
                gvElem.Columns["Chk"].Visible = true;
                gvElem.Columns["Chk"].VisibleIndex = 0;
                btnSpremiParentElement.Visible = false;
                GridElementParentFocusedRowChanged();
            }
        }

        private void gvElem_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
                return;
            GridElementFocusedRowChanged();
        }

        private void GridElementFocusedRowChanged()
        {
            if (pnlElements.Parent == splitContainerControl1.Panel1)
            {
                int toprowindex = gvElemParent.TopRowIndex;
                int rowhandle = gvElemParent.FocusedRowHandle;

                PopulateParentElements(GetElementId(), rowhandle, toprowindex);
            }
        }

        private void gvElem_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (pnlParentElements.Parent == splitContainerControl1.Panel1)
            {
                if (e.RowHandle < 0)
                    return;

                bool chk = (bool)gvElem.GetDataRow(e.RowHandle)["Chk"];
                if (chk == true)
                {
                    e.Appearance.ForeColor = Color.DarkBlue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
        }

        private void gvElemParent_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (pnlElements.Parent == splitContainerControl1.Panel1)
            {
                if (e.RowHandle < 0)
                    return;

                bool chk = (bool)gvElemParent.GetDataRow(e.RowHandle)["Chk"];
                if (chk == true)
                {
                    e.Appearance.ForeColor = Color.DarkBlue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
        }

        private void btnSpremiElement_Click(object sender, EventArgs e)
        {
            int rowHandle = Convert.ToInt32(gvElem.FocusedRowHandle);

            //protrčati kroz sve redove grida i updateat tablicu CarpoDsAttribute i dodati vezu atribut/element gdje je changed = true

            int carpoDsElementIdParent, carpoDsElementId;

            carpoDsElementIdParent = GetParentId();

            bool chk = false;

            try
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Spremanje");

                for (int i = 0; i < gvElem.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElem.GetRowHandle(i));

                    chk = (bool)gvElem.GetDataRow(handle)["chk"];

                    carpoDsElementId = Convert.ToInt32(gvElem.GetRowCellValue(handle, "CarpoDsElementId"));

                    try
                    {
                        if ((bool)gvElem.GetDataRow(handle)["chk"] != (bool)gvElem.GetDataRow(handle)["chk2"])
                            Data.Elements.SetElemParent(carpoDsElementIdParent, carpoDsElementId, chk);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    //MessageBox.Show(email);
                }

                //PopulateGridNePosl();
            }
            catch
            {
                splashScreenManager1.CloseWaitForm();
            }
            finally
            {
                if (splashScreenManager1.IsSplashFormVisible == true)
                {
                    System.Threading.Thread.Sleep(500);
                    splashScreenManager1.CloseWaitForm();
                    PopulateElements(rowHandle);
                }
            }
        }

        private void changeButtonState()
        {
            DataTable dt;
            DataRow[] dr;
            if (pnlParentElements.Parent == splitContainerControl1.Panel1)
            {
                dt = (DataTable)gcElem.DataSource;

                dr = dt.Select("chk <> chk2");
            }
            else
            {
                dt = (DataTable)gcElemParent.DataSource;

                dr = dt.Select("chk <> chk2");
            }

            if (dr.Count() > 0)
            {
                btnSpremiElement.LookAndFeel.SkinMaskColor = Color.LightCoral;
                btnSpremiParentElement.LookAndFeel.SkinMaskColor = Color.LightCoral;
            }
            else
            {
                btnSpremiElement.LookAndFeel.SkinMaskColor = Color.Empty;
                btnSpremiParentElement.LookAndFeel.SkinMaskColor = Color.Empty;
            }
        }

        private void repChk_CheckedChanged(object sender, EventArgs e)
        {
            changeButtonState();
        }

        private void repCheckElement_CheckedChanged(object sender, EventArgs e)
        {
            changeButtonState();
        }

        private void btnSpremiParentElement_Click(object sender, EventArgs e)
        {
            int rowHandle = Convert.ToInt32(gvElemParent.FocusedRowHandle);

            //protrčati kroz sve redove grida i updateat tablicu CarpoDsAttribute i dodati vezu atribut/element gdje je changed = true

            int carpoDsElementIdParent, carpoDsElementId;

            carpoDsElementId = GetElementId();

            bool chk = false;

            try
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Spremanje");

                for (int i = 0; i < gvElemParent.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElemParent.GetRowHandle(i));

                    chk = (bool)gvElemParent.GetDataRow(handle)["chk"];

                    carpoDsElementIdParent = Convert.ToInt32(gvElemParent.GetRowCellValue(handle, "CarpoDsElementId"));

                    try
                    {
                        if ((bool)gvElemParent.GetDataRow(handle)["chk"] != (bool)gvElemParent.GetDataRow(handle)["chk2"])
                            Data.Elements.SetElemParent(carpoDsElementIdParent, carpoDsElementId, chk);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    //MessageBox.Show(email);
                }

                //PopulateGridNePosl();
            }
            catch
            {
                splashScreenManager1.CloseWaitForm();
            }
            finally
            {
                if (splashScreenManager1.IsSplashFormVisible == true)
                {
                    System.Threading.Thread.Sleep(500);
                    splashScreenManager1.CloseWaitForm();
                    PopulateParentElements(GetElementId(), rowHandle);
                }
            }
        }

        private void gvElem_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (pnlParentElements.Parent == splitContainerControl1.Panel1)
                popupMenu1.ShowPopup(barManager2, Control.MousePosition);
        }

        private void gvElemParent_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (pnlElements.Parent == splitContainerControl1.Panel1)
                popupMenu1.ShowPopup(barManager2, Control.MousePosition);
        }

        private void btnSelSve_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            state = true;
            bool parentgridselected = false;

            if (pnlElements.Parent == splitContainerControl1.Panel1)
            {
                //ako je grid elementi lijevo onda selektiramo sve parente
                parentgridselected = true;
            }

            changeGridState(parentgridselected);
        }

        private void changeGridState(bool parentgridselected)
        {
            if (parentgridselected == true)
            {
                //update atribute elementa
                //carpodselementid = GetElementId();

                for (int i = 0; i < gvElemParent.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElemParent.GetRowHandle(i));
                    //carpodsattributeid = Convert.ToInt32(gvAttr.GetRowCellValue(handle, "CarpoDsAttributeId"));

                    //if ((bool)gvAttr.GetDataRow(handle)["Chk"] != chk)
                    //    Data.Cms.Elements.SetElemAttr(carpodselementid, carpodsattributeid, chk);

                    if ((bool)gvElemParent.GetDataRow(handle)["Chk"] != state)
                    {
                        gvElemParent.GetDataRow(handle)["chk"] = state;
                    }
                }

                changeButtonState();
            }
            else
            {
                for (int i = 0; i < gvElem.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElem.GetRowHandle(i));

                    if ((bool)gvElem.GetDataRow(handle)["Chk"] != state)
                    {
                        gvElem.GetDataRow(handle)["Chk"] = state;
                    }
                }

                changeButtonState();
            }
        }

        private void btnDesSve_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            state = false;
            bool parentgridselected = false;

            if (pnlElements.Parent == splitContainerControl1.Panel1)
            {
                //ako je grid elementi lijevo onda selektiramo sve parente
                parentgridselected = true;
            }

            changeGridState(parentgridselected);
        }
    }
}