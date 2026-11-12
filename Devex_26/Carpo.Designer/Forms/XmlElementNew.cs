using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.Designer.Forms
{
    public partial class XmlElementNew : DevExpress.XtraEditors.XtraForm
    {
        public List<Models.Element> returnList = new List<Models.Element>();

        private XmlDocument xdoc = new XmlDocument();

        private List<Models.ElementCheck> newElementsList = new List<Models.ElementCheck>();
        private List<Models.Element> recievedElement = new List<Models.Element>();
        private List<Models.Element> elementList = new List<Models.Element>();
        private List<Models.Element> tempChildElementsList = new List<Models.Element>();
        private List<Models.Element> sortTempList = new List<Models.Element>();

        private int rowCount = 0, rowHandle = 0;
        private string parent = "", objectId = "";
        private List<string> newIdList = new List<string>();
        private List<string> defaultIdOrder = new List<string>();

        public XmlElementNew(List<Models.ElementCheck> listOfAvailableNewElements, List<Models.Element> element, XmlDocument doc)
        {
            InitializeComponent();

            Helpers.ModelDataTransfer.CopyElementCheckModel(newElementsList, listOfAvailableNewElements);
            Helpers.ModelDataTransfer.CopyElementModel(recievedElement, element);

            parent = newElementsList[0].ParentName;
            xdoc = doc;

            this.Text = "Element <" + parent + ">";
        }

        private void XmlElementNew_Load(object sender, EventArgs e)
        {
            SetNewElementsGrid();
            SetExistingElementsGrid();
        }

        private void SetNewElementsGrid()
        {
            gcNewElements.DataSource = newElementsList;

            gvNewElements.Columns["Name"].Caption = "Naziv";
            GridColumn col = gvNewElements.Columns["ParentName"];
            col.Visible = false;
        }

        private void SetExistingElementsGrid()
        {
            // punim listu bez parent elementa
            foreach (Models.Element el in recievedElement)
            {
                if (el.ParentId == recievedElement[0].Id)
                {
                    tempChildElementsList.Add(el);
                    elementList.Add(el);
                    defaultIdOrder.Add(el.Id);
                    rowCount++;
                }
            }

            gcExistingElements.DataSource = tempChildElementsList;

            gvExistingElements.Columns["Name"].Caption = "Naziv";
            foreach (GridColumn gc in gvExistingElements.Columns)
            {
                if (!(gc.Name == "colName" || gc.Name == "colInfo")) // ovo provjerit s Igorom
                    gc.Visible = false;
            }
        }

        private void SetEditButtons(int rowhandle)
        {
            if (gvExistingElements.RowCount == 0) // ako nema zapisa u gridu sve disejblam
            {
                btnMoveNodeDown.Enabled = btnMoveNodeUp.Enabled = btnRemoveNewNode.Enabled = false;
                return;
            }
            else // u suprotnom sve enejblam
                btnMoveNodeDown.Enabled = btnMoveNodeUp.Enabled = btnRemoveNewNode.Enabled = true;

            //zasebno disejblanje svakog buttona
            if (gvExistingElements.FocusedRowHandle == 0)
            {
                btnMoveNodeUp.Enabled = false;
                btnMoveNodeDown.Select();
            }
            if (gvExistingElements.FocusedRowHandle == gvExistingElements.RowCount - 1)
            {
                btnMoveNodeDown.Enabled = false;
                btnMoveNodeUp.Select();
            }
            if (!newIdList.Contains(gvExistingElements.GetRowCellValue(rowhandle, "Id").ToString()))
                btnRemoveNewNode.Enabled = false;
        }

        private void SetSaveButton(bool enabled)
        {
            btnSave.Enabled = enabled;
            FontStyle style = new FontStyle();
            Color color = new Color();

            if (enabled)
            {
                style = FontStyle.Bold;
                color = Color.Red;
            }
            else
            {
                style = FontStyle.Regular;
                color = Color.LightGray;
            }

            btnSave.Font = new Font(btnSave.Font.Name, btnSave.Font.Size, style);
            btnSave.Appearance.ForeColor = color;
        }

        private void gvExistingElements_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if ((newIdList.Count > 0) && (e.RowHandle >= 0))
            {
                if (newIdList.Contains(gvExistingElements.GetRowCellValue(e.RowHandle, "Id").ToString()))
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void gvExistingElements_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            CheckIfSortChanged();
            SetEditButtons(e.FocusedRowHandle);
        }

        private void gvNewElements_DoubleClick(object sender, EventArgs e)
        {
            string id = Guid.NewGuid().ToString();

            var rowHandle = gvNewElements.FocusedRowHandle;
            var obj = gvNewElements.GetRowCellValue(rowHandle, "Name");
            string name = obj.ToString();

            string parentId = recievedElement[0].Id;

            XmlDocument xdocTemp = new XmlDocument();
            System.Xml.XmlNode newNode = xdocTemp.CreateNode(XmlNodeType.Element, name, "");

            // u privremenu listu dodajem nove elemente
            tempChildElementsList.Add(new Models.Element() { Id = id, Name = name, ParentId = parentId, Node = null, Info = "" });
            // punim listu novih Id-eva
            newIdList.Add(id);

            // čistim i osvježavam datasource gridu
            gcExistingElements.DataSource = null;
            gcExistingElements.DataSource = tempChildElementsList;

            if (gvExistingElements.RowCount == rowCount + 1)
                SetSaveButton(true);
        }

        private void btnMoveNodeUp_Click(object sender, EventArgs e)
        {
            GetSortData();

            for (int i = 0; i < tempChildElementsList.Count; i++)
            {
                if (i == rowHandle - 1)
                    sortTempList.Add(tempChildElementsList[rowHandle]);
                else if (i == rowHandle)
                    sortTempList.Add(tempChildElementsList[rowHandle - 1]);
                else if (i != rowHandle - 1)
                    sortTempList.Add(tempChildElementsList[i]);
            }

            RefreshSortedGrid();
            CheckIfSortChanged();
        }

        private void btnMoveNodeDown_Click(object sender, EventArgs e)
        {
            GetSortData();

            for (int i = 0; i < tempChildElementsList.Count; i++)
            {
                if (i == rowHandle)
                    sortTempList.Add(tempChildElementsList[rowHandle + 1]);
                else if (i == rowHandle + 1)
                    sortTempList.Add(tempChildElementsList[rowHandle]);
                else if (i != rowHandle)
                    sortTempList.Add(tempChildElementsList[i]);
            }

            RefreshSortedGrid();
            CheckIfSortChanged();
        }

        private void btnRemoveNewNode_Click(object sender, EventArgs e)
        {
            GetSortData();

            int index = tempChildElementsList.FindIndex(x => x.Id == objectId);
            int index1 = newIdList.FindIndex(x => x == objectId);
            tempChildElementsList.RemoveAt(index);
            newIdList.RemoveAt(index1);
            gcExistingElements.DataSource = null;
            gcExistingElements.DataSource = tempChildElementsList;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AddNodesIntoXdoc();
            SetModel();
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetModel();
            this.DialogResult = DialogResult.Cancel;
        }

        private void AddNodesIntoXdoc()
        {
            System.Xml.XmlNode parentNode = recievedElement[0].Node;
            string parentId = recievedElement[0].Id;

            // čistim listu koju ću slati nazad
            if (elementList.Count > 0)
                elementList.Clear();

            System.Xml.XmlNode previousNode = null;
            foreach (Models.Element e in tempChildElementsList)
            {
                // stare (već postojeće) node
                if (!(e.Node == null))
                {
                    previousNode = null;
                    elementList.Add(e);
                    previousNode = e.Node;
                    //previousNode = xdoc.ChildNodes[0];
                }
                else // nove node
                {
                    System.Xml.XmlNode newNode = xdoc.CreateNode(XmlNodeType.Element, e.Name, "");
                    elementList.Add(new Models.Element() { Id = e.Id, Name = e.Name, ParentId = parentId, Node = newNode, Info = "" });
                    //parentNode.AppendChild(newNode);
                    parentNode.InsertAfter(newNode, previousNode);
                }
            }
        }

        private void SetModel()
        {
            // returnList - lista koju ćemo vraćati na parent formu
            returnList.Add(recievedElement[0]); // na listu dodajem parent element
            foreach (Models.Element el in elementList)
            {
                returnList.Add(el); // potom na listu dodajem svaki element koji smo editirali

                // ako je dodani element imao child elemente u prvom modelu, dodajem njih (+ brišem ih s liste) pa tek potom idem dalje
                for (int i = 0; i < recievedElement.Count; i++)
                {
                    Models.Element element = recievedElement[i];
                    if (element.ParentId == el.Id)
                    {
                        returnList.Add(element);
                        recievedElement.RemoveAt(recievedElement.IndexOf(element));
                        i--;
                    }
                }
            }
        }

        private void GetSortData()
        {
            rowHandle = gvExistingElements.FocusedRowHandle;
            objectId = gvExistingElements.GetRowCellValue(rowHandle, "Id").ToString();
            tempChildElementsList = gcExistingElements.DataSource as List<Models.Element>;
            if (sortTempList.Count > 0)
                sortTempList.Clear();
        }

        private void CheckIfSortChanged()
        {
            if (gcExistingElements.DataSource != null)
            {
                List<Models.Element> datasourceModel = gcExistingElements.DataSource as List<Models.Element>;
                List<string> tempIdOrder = new List<string>();
                bool isExact = false;

                foreach (Models.Element el in datasourceModel)
                {
                    tempIdOrder.Add(el.Id);
                }

                if (tempIdOrder.Count == defaultIdOrder.Count)
                {
                    for (int i = 0; i < tempIdOrder.Count; i++)
                    {
                        if (tempIdOrder[i] != defaultIdOrder[i])
                        {
                            SetSaveButton(true);
                            return;
                        }
                    }
                    isExact = true;
                }
                else
                    SetSaveButton(true);

                if (isExact)
                    SetSaveButton(false);
            }
        }

        private void RefreshSortedGrid()
        {
            Helpers.ModelDataTransfer.CopyElementModel(tempChildElementsList, sortTempList);
            gcExistingElements.DataSource = tempChildElementsList;
            gvExistingElements.FocusedRowHandle = tempChildElementsList.FindIndex(x => x.Id == objectId);
        }
    }
}