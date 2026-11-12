using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class Elements : Form
    {
        public static bool allchecked = false;
        public static int changesCounter = 0;

        public Elements()
        {
            InitializeComponent();
        }

        private void gcElem_Load(object sender, EventArgs e)
        {
            PopulateElements(null);
        }

        private void PopulateElements(int? id, int? rowHandle = null, int? topRowIndex = null)
        {
            gcElem.DataSource = null;
            gcElem.DataSource = Data.Elements.GetElements(id);
            changesCounter = 0;
            if (topRowIndex != null)
                gvElem.TopRowIndex = (int)topRowIndex;

            if (rowHandle != null)
            {
                gvElem.FocusedRowHandle = (int)rowHandle;
                gvElem.SelectRow((int)rowHandle);
            }

            changeButton2State();
        }

        private void PopulateAttrtibutes(int? rowHandle = null, int? topRowIndex = null)
        {
            gcAttr.DataSource = null;
            gcAttr.DataSource = Data.Elements.GetAttributes(GetElementId());
            changesCounter = 0;

            populateRepositoryLookupEdit();
            if (topRowIndex != null)
                gvAttr.TopRowIndex = (int)topRowIndex;
            if (rowHandle != null)
            {
                gvAttr.FocusedRowHandle = (int)rowHandle;
                gvAttr.SelectRow((int)rowHandle);
            }

            changeButtonState();
        }

        public void populateRepositoryLookupEdit()
        {
            DataTable type = new DataTable();
            type = Data.Elements.GetTypes();

            RepositoryItemLookUpEdit riLookup = new RepositoryItemLookUpEdit();
            riLookup.DataSource = type;
            riLookup.ValueMember = "Id";
            riLookup.DisplayMember = "Name";

            riLookup.DropDownRows = type.Rows.Count;

            riLookup.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            riLookup.AutoSearchColumnIndex = 1;
            riLookup.NullText = "...";
            riLookup.PopulateColumns();
            riLookup.Columns["Id"].Visible = false;
            gvAttr.Columns["Types"].ColumnEdit = riLookup;
            //gvAttr.BestFitColumns();
        }

        private void gvElem_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridElementsFocusedRowChanged();
        }

        private void GridElementsFocusedRowChanged()
        {
            if (pnlElemets.Parent == splitContainerControl1.Panel1 && gvElem.FocusedRowHandle >= 0)
            {
                int rowHandle = gvAttr.FocusedRowHandle;
                int toprowindex = gvAttr.TopRowIndex;
                PopulateAttrtibutes(rowHandle, toprowindex);
            }
        }

        private void repChk_EditValueChanged(object sender, EventArgs e)
        {
            bool value = (sender as DevExpress.XtraEditors.CheckEdit).Checked;

            //int carpoDsElementid = GetElementId();
            //int carpoDsAttributeId = GetAttributeId();

            //Data.Cms.Elements.SetElemAttr(carpoDsElementid, carpoDsAttributeId, value);
        }

        private int GetElementId()
        {
            return Convert.ToInt32(gvElem.GetDataRow(gvElem.FocusedRowHandle)["CarpoDsElementId"]);
        }

        private int GetAttributeId()
        {
            return Convert.ToInt32(gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["CarpoDsAttributeId"]);
        }

        private void gvAttr_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (pnlElemets.Parent == splitContainerControl1.Panel1)
            {
                if (e.RowHandle < 0)
                    return;

                bool chk = (bool)gvAttr.GetDataRow(e.RowHandle)["Chk"];
                if (chk == true)
                {
                    e.Appearance.ForeColor = Color.DarkBlue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
        }

        private void txtDescription_Leave(object sender, EventArgs e)
        {
            if (gvAttr.FocusedRowHandle < 0)
                return;

            //int rowHandle = Convert.ToInt32(gvAttr.FocusedRowHandle);
            gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description"] = txtDescription.Text;
            //string description = txtDescription.Text;

            //Data.Cms.Elements.SetAttrDescription(GetAttributeId(), description);

            //PopulateAttrtibutes(rowHandle);
            //string desc = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description"].ToString();
            //string desc2 = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description2"].ToString();

            //if (gvAttr.FocusedRowHandle < 0)
            //    return;

            //if ( desc != desc2)
            //{
            //    changesCounter++;
            //}
            //else
            //{
            //    changesCounter--;
            //}

            changeButtonState();
        }

        private void gvAttr_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridAttrFocusedRowChanged();
        }

        private void GridAttrFocusedRowChanged()
        {
            if (gvAttr.FocusedRowHandle < 0)
                return;

            var description = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description"];
            if (description != null)
                txtDescription.Text = description.ToString();

            if (pnlAttr.Parent == splitContainerControl1.Panel1)
            {
                int toprowindex = gvElem.TopRowIndex;
                int rowhandle = gvElem.FocusedRowHandle;
                PopulateElements(GetAttributeId(), rowhandle, toprowindex);
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (pnlElemets.Parent == splitContainerControl1.Panel1)
            {
                pnlElemets.Parent = splitContainerControl1.Panel2;
                pnlAttr.Parent = splitContainerControl1.Panel1;
                pnlDescription.Hide();
                panel3.Hide();
                panel4.Show();
                gvAttr.Columns["Value"].Visible = false;
                gvAttr.Columns["Types"].Visible = false;
                gvElem.Columns["Chk"].Visible = true;
                gvElem.Columns["Chk"].VisibleIndex = 0;
                gvAttr.Columns["Chk"].Visible = false;

                GridAttrFocusedRowChanged();
            }
            else
            {
                pnlElemets.Parent = splitContainerControl1.Panel1;
                pnlAttr.Parent = splitContainerControl1.Panel2;
                gvAttr.Columns["Chk"].Visible = true;
                gvAttr.Columns["Chk"].VisibleIndex = 0;
                pnlDescription.Show();
                panel3.Show();
                panel4.Hide();
                gvAttr.Columns["Value"].Visible = true;
                gvAttr.Columns["Types"].Visible = true;
                gvElem.Columns["Chk"].Visible = false;
                gvAttr.Columns["Chk"].Visible = true;
                GridElementsFocusedRowChanged();
            }
        }

        private void btnSpremiAtribut_Click(object sender, EventArgs e)
        {
            int rowHandle = Convert.ToInt32(gvAttr.FocusedRowHandle);

            //protrčati kroz sve redove grida i updateat tablicu CarpoDsAttribute i dodati vezu atribut/element gdje je changed = true

            int attributeID, attributetypeId, elementId;
            string description, value;

            bool chk = true;

            if (allchecked == true)
                chk = false;

            elementId = GetElementId();
            try
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Spremanje");

                for (int i = 0; i < gvAttr.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvAttr.GetRowHandle(i));

                    if ((bool)gvAttr.GetDataRow(handle)["changed"] == true)
                    {
                        chk = (bool)gvAttr.GetDataRow(handle)["chk"];
                        attributeID = Convert.ToInt32(gvAttr.GetRowCellValue(handle, "CarpoDsAttributeId"));
                        attributetypeId = gvAttr.GetRowCellValue(handle, "Types") != DBNull.Value ? Convert.ToInt32(gvAttr.GetRowCellValue(handle, "Types")) : 0;
                        description = gvAttr.GetRowCellValue(handle, "Description").ToString() == "" ? null : Convert.ToString(gvAttr.GetRowCellValue(handle, "Description"));
                        value = gvAttr.GetRowCellValue(handle, "Value").ToString() == "" ? null : Convert.ToString(gvAttr.GetRowCellValue(handle, "Value"));

                        try
                        {
                            //daodaj link atribut / element ako je mijenjan state
                            if ((bool)gvAttr.GetDataRow(handle)["chk"] != (bool)gvAttr.GetDataRow(handle)["chk2"])
                                Data.Elements.SetElemAttr(elementId, attributeID, chk);
                            //spremi - update attribute
                            Data.Elements.setAttrDescValue(attributeID, description, value, attributetypeId);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
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
                    PopulateAttrtibutes(rowHandle);
                    //btnSpremiAtribut.Enabled = false;
                    changeButtonState();
                }
            }
        }

        private void gvAttr_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (pnlElemets.Parent == splitContainerControl1.Panel1)
                popupMenu1.ShowPopup(barManager1, Control.MousePosition);
        }

        private void btnSelectSve_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool selectAttributes = false;
            allchecked = false;
            if (pnlElemets.Parent == splitContainerControl1.Panel1)
            {
                selectAttributes = true;
            }

            ChangeState(selectAttributes);

            //DialogResult dialogResult = MessageBox.Show("Želite dodati sve zapise?","", MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);
            //if (dialogResult == DialogResult.Yes)
            //{
            //    if (gvAttr.RowCount > 0)
            //        ChangeState(selectAttributes);
            //}
        }

        private void ChangeState(bool selectAttributes)
        {
            bool chk = true;

            if (allchecked == true)
                chk = false;

            if (selectAttributes == true)
            {
                //update atribute elementa
                //carpodselementid = GetElementId();

                for (int i = 0; i < gvAttr.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvAttr.GetRowHandle(i));
                    //carpodsattributeid = Convert.ToInt32(gvAttr.GetRowCellValue(handle, "CarpoDsAttributeId"));

                    //if ((bool)gvAttr.GetDataRow(handle)["Chk"] != chk)
                    //    Data.Cms.Elements.SetElemAttr(carpodselementid, carpodsattributeid, chk);

                    if ((bool)gvAttr.GetDataRow(handle)["Chk"] != chk)
                    {
                        gvAttr.GetDataRow(handle)["changed"] = true;
                        changesCounter++;
                    }
                    else
                    {
                        gvAttr.GetDataRow(handle)["changed"] = false;
                        changesCounter--;
                    }

                    gvAttr.GetDataRow(handle)["Chk"] = chk;
                }

                changeButtonState();
            }
            else
            {
                //update elemente atributa
                //carpodsattributeid = GetAttributeId();

                for (int i = 0; i < gvElem.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElem.GetRowHandle(i));
                    //carpodselementid = Convert.ToInt32(gvElem.GetRowCellValue(handle, "CarpoDsElementId"));

                    //if ((bool)gvElem.GetDataRow(handle)["Chk"] != chk)
                    //    Data.Cms.Elements.SetElemAttr(carpodselementid, carpodsattributeid, chk);

                    if ((bool)gvElem.GetDataRow(handle)["Chk"] != chk)
                    {
                        gvElem.GetDataRow(handle)["changed"] = true;
                        changesCounter++;
                    }
                    else
                    {
                        gvElem.GetDataRow(handle)["changed"] = false;
                        changesCounter--;
                    }

                    gvElem.GetDataRow(handle)["Chk"] = chk;
                }

                changeButton2State();
            }
        }

        private void gvElem_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (pnlAttr.Parent == splitContainerControl1.Panel1)
                popupMenu1.ShowPopup(barManager1, Control.MousePosition);
        }

        private void repCheckElement_EditValueChanged(object sender, EventArgs e)
        {
            bool value = (sender as DevExpress.XtraEditors.CheckEdit).Checked;

            //int carpoDsElementid = GetElementId();
            //int carpoDsAttributeId = GetAttributeId();

            //Data.Cms.Elements.SetElemAttr(carpoDsElementid, carpoDsAttributeId, value);
        }

        private void gvElem_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (pnlAttr.Parent == splitContainerControl1.Panel1)
            {
                if (e.RowHandle < 0)
                    return;

                bool chk = (bool)gvElem.GetDataRow(e.RowHandle)["Chk"];
                if (chk == true)
                {
                    e.Appearance.ForeColor = Color.DarkBlue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }

                //changeButtonState();
            }
        }

        private void btnUnselectSve_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool selectAttributes = false;
            allchecked = true;

            if (pnlElemets.Parent == splitContainerControl1.Panel1)
            {
                selectAttributes = true;
            }

            ChangeState(selectAttributes);

            //DialogResult dialogResult = MessageBox.Show("Želite maknuti sve zapise?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            //if (dialogResult == DialogResult.Yes)
            //{
            //    if (gvAttr.RowCount > 0)
            //        ChangeState(selectAttributes);
            //}
        }

        private void btnDodajAatribut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddAttribute(null, null);
        }

        private void AddAttribute(string name, int? id)
        {
            int rowHandle = gvAttr.FocusedRowHandle;
            using (ExamplesNew frm = new ExamplesNew(name, "Atribut"))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    //ako je update
                    if (id != null)
                        Data.Elements.UpdateAttr(frm.TextName, (int)id);
                    else
                        Data.Elements.SetAttr(frm.TextName);
                    PopulateAttrtibutes(rowHandle);
                }
            }
        }

        private void AddElement(int? id)
        {
            int rowHandle = gvElem.FocusedRowHandle;
            using (ElementsNew frm = new ElementsNew())
            {
                frm.Id = id == null ? 0 : (int)id;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    PopulateElements(null);
                    gvElem.FocusedRowHandle = (int)rowHandle;

                    gvElem.SelectRow((int)rowHandle);
                }
            }

            //using (ExamplesNew frm = new ExamplesNew(name, "Element"))
            //{
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //        //ako je update
            //        if (id != null)
            //            Data.Cms.Elements.UpdateElem(frm.TextName, (int)id);
            //        else
            //            Data.Cms.Elements.SetElem(frm.TextName);
            //        PopulateElements(null);
            //    }
            //}
        }

        private void btnDodajElement_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddElement(null);
        }

        private void gvAttr_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (gvAttr.FocusedRowHandle < 0)
            //    return;
            //if ((bool)gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] != true)
            //    gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] = true;
            //else
            //    gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] = false;
            if (e.RowHandle < 0)
                return;

            changeButtonState();
        }

        private void changeButtonState()
        {
            //DataTable dt = (DataTable)gcAttr.DataSource;
            //DataRow[] dr = dt.Select("Types <> Types2 OR Description <> Description2 OR Value <> Value2");

            //btnSpremiAtribut.Enabled = dr.Count() > 0;
            if (gvAttr.FocusedRowHandle < 0)
                return;

            string desc = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description"].ToString();
            string desc2 = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Description2"].ToString();

            string value = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Value"].ToString();
            string value2 = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Value2"].ToString();

            int type = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Types"] != DBNull.Value ? Convert.ToInt32(gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Types"]) : 0;
            int type2 = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Types2"] != DBNull.Value ? Convert.ToInt32(gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Types2"]) : 0;

            bool chk = (bool)gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Chk"];
            bool chk2 = (bool)gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Chk2"];

            if (value != value2 || type != type2 || desc != desc2 || chk != chk2)
            {
                if ((bool)gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] != true)
                    changesCounter++;
                gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] = true;
            }
            else
            {
                if ((bool)gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] != false)
                    changesCounter--;
                gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["changed"] = false;
            }

            if (changesCounter > 0)
            {
                btnSpremiAtribut.LookAndFeel.SkinMaskColor = Color.LightCoral;
            }
            else
            {
                btnSpremiAtribut.LookAndFeel.SkinMaskColor = Color.Empty;
            }
        }

        private void changeButton2State()
        {
            //DataTable dt = (DataTable)gcAttr.DataSource;
            //DataRow[] dr = dt.Select("Types <> Types2 OR Description <> Description2 OR Value <> Value2");

            //btnSpremiAtribut.Enabled = dr.Count() > 0;
            if (gvElem.FocusedRowHandle < 0)
                return;

            bool chk = (bool)gvElem.GetDataRow(gvElem.FocusedRowHandle)["Chk"];
            bool chk2 = (bool)gvElem.GetDataRow(gvElem.FocusedRowHandle)["Chk2"];

            if (chk != chk2)
            {
                if ((bool)gvElem.GetDataRow(gvElem.FocusedRowHandle)["changed"] != true)
                    changesCounter++;
                gvElem.GetDataRow(gvElem.FocusedRowHandle)["changed"] = true;
            }
            else
            {
                if ((bool)gvElem.GetDataRow(gvElem.FocusedRowHandle)["changed"] != false)
                    changesCounter--;
                gvElem.GetDataRow(gvElem.FocusedRowHandle)["changed"] = false;
            }

            if (changesCounter > 0)
            {
                btnSpremiElement.LookAndFeel.SkinMaskColor = Color.LightCoral;
            }
            else
            {
                btnSpremiElement.LookAndFeel.SkinMaskColor = Color.Empty;
            }
        }

        private void repChk_CheckedChanged(object sender, EventArgs e)
        {
            changeButtonState();
        }

        private void btnSpremiElement_Click(object sender, EventArgs e)
        {
            int rowHandle = Convert.ToInt32(gvElem.FocusedRowHandle);
            try
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Spremanje");

                //update elemente atributa
                int carpodsattributeid, carpodselementid;
                carpodsattributeid = GetAttributeId();
                bool chk;
                for (int i = 0; i < gvElem.RowCount; i++)
                {
                    int handle = Convert.ToInt32(gvElem.GetRowHandle(i));
                    carpodselementid = Convert.ToInt32(gvElem.GetRowCellValue(handle, "CarpoDsElementId"));
                    chk = (bool)gvElem.GetDataRow(handle)["chk"];

                    if ((bool)gvElem.GetDataRow(handle)["changed"] == true)
                        Data.Elements.SetElemAttr(carpodselementid, carpodsattributeid, chk);
                }
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
                    PopulateElements(GetAttributeId(), rowHandle);
                    //btnSpremiAtribut.Enabled = false;
                }
            }
        }

        private void repCheckElement_CheckedChanged(object sender, EventArgs e)
        {
            changeButton2State();
        }

        private void gvElem_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.RowHandle < 0)
                return;
            changeButton2State();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool selectAttributes = false;

            if (pnlElemets.Parent == splitContainerControl1.Panel1)
            {
                selectAttributes = true;
            }

            DialogResult dialogResult = MessageBox.Show("Želite obrisati odabrani redak?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dialogResult == DialogResult.Yes)
            {
                Obrisi(selectAttributes);
            }
        }

        private void Obrisi(bool atribut)
        {
            int carpoDsAttributeId = GetAttributeId();
            int carpoDsElementId = GetElementId();

            if (atribut == true)
            {
                //delete atribut
                Data.Elements.DelAttr(carpoDsAttributeId, carpoDsElementId);
                PopulateAttrtibutes();
            }
            else
            {
                //delete element
                Data.Elements.DelElem(carpoDsElementId, carpoDsAttributeId);
                PopulateElements(null);
            }
        }

        private void gvAttr_DoubleClick(object sender, EventArgs e)
        {
            if (gvAttr.FocusedRowHandle < 0)
                return;
            string name = gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["Name"].ToString();
            int id = Convert.ToInt32(gvAttr.GetDataRow(gvAttr.FocusedRowHandle)["CarpoDsAttributeId"]);
            AddAttribute(name, id);
        }

        private void gvElem_DoubleClick(object sender, EventArgs e)
        {
            if (gvElem.FocusedRowHandle < 0)
                return;

            int id = Convert.ToInt32(gvElem.GetDataRow(gvElem.FocusedRowHandle)["CarpoDsElementId"]);
            AddElement(id);
        }
    }
}