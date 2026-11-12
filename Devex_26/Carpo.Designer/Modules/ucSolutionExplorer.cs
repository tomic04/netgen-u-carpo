using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.Designer.Modules
{
    public partial class ucSolutionExplorer : System.Windows.Forms.UserControl
    {
        public ucSolutionExplorer()
        {
            InitializeComponent();
            InitTreeView(treeView);
            treeView.CustomDrawNodeCell += treeView_CustomDrawNodeCell;
            treeView.AfterCollapse += treeView_AfterCollapse;
            treeView.AfterExpand += treeView_AfterExpand;

            treeView.KeyDown += new KeyEventHandler(treeView_KeyDown);
            treeView.HiddenEditor += new EventHandler(treeView_HiddenEditor);

            //AddAllNodes(iShow.Down);
        }

        public static void InitTreeView(DevExpress.XtraTreeList.TreeList treeView)
        {
            //TreeListColumn column = treeView.Columns.Add();
            //column.Visible = true;

            treeView.KeyFieldName = "Id";
            treeView.ParentFieldName = "ParentId";

            treeView.Columns.Add(new TreeListColumn() { FieldName = "Name", Visible = true });

            treeView.OptionsView.ShowColumns = false;
            treeView.OptionsView.ShowIndicator = false;
            treeView.OptionsView.ShowVertLines = false;
            treeView.OptionsView.ShowHorzLines = false;
            treeView.OptionsBehavior.Editable = false;
            treeView.OptionsSelection.EnableAppearanceFocusedCell = false;
        }

        private void treeView_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            if (e.Node.Selected)
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            else
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
        }

        private void SetIndex(TreeListNode node, int index, bool expand)
        {
            //int newIndex = expand ? index - 1 : index + 1;
            //if(node.StateImageIndex == index)
            //    node.StateImageIndex = newIndex;
        }

        private void treeView_AfterExpand(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetIndex(e.Node, 7, true);
            SetIndex(e.Node, 9, true);
        }

        private void treeView_AfterCollapse(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetIndex(e.Node, 6, false);
            SetIndex(e.Node, 8, false);
        }

        public void PopulateNodes(List<Models.ProjectTreeNode> source)
        {
            treeView.Nodes.Clear();
            treeView.DataSource = source;
            treeView.ExpandAll();
        }

        public event EventHandler TreeViewItemClick;

        private void treeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (TreeViewItemClick != null)
                TreeViewItemClick(sender, EventArgs.Empty);
        }

        private void treeView_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            if (e.Node.Level == 0)
                e.NodeImageIndex = 4;
            else
                e.NodeImageIndex = 11;
        }

        private void mnuTreeViewOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //otvori dokument koji je kliknut
            if (TreeViewItemClick != null)
                TreeViewItemClick(treeView, EventArgs.Empty);
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            TreeList tree = sender as TreeList;
            TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Location);
            if (e.Button == MouseButtons.Right && hitInfo.HitTest.Node != null)
            {
                hitInfo.HitTest.Node.Selected = !hitInfo.HitTest.Node.Selected;
                mnuEditiranje.ShowPopup(MousePosition);

                //ako je child node
                if (hitInfo.HitTest.Node.Level > 0)
                {
                    mnuTreeViewAdd.Enabled = false;
                    mnuTreeViewAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    mnuTreeViewClose.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    mnuTreeViewOpen.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    mnuTreeViewOpen.Enabled = true;
                    mnuTreeViewClose.Enabled = false;
                }
                else
                {
                    mnuTreeViewAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    mnuTreeViewOpen.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    mnuTreeViewClose.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    mnuTreeViewOpen.Enabled = false;
                    mnuTreeViewAdd.Enabled = true;
                    mnuTreeViewClose.Enabled = true;
                }
            }
        }

        private void mnuTreeViewRename_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode focusedNode = treeView.FocusedNode;
            var record = (Models.ProjectTreeNode)treeView.GetDataRecordByNode(focusedNode);

            if (focusedNode.Level > 0)
            {
                treeView.OptionsBehavior.Editable = !treeView.OptionsBehavior.Editable;
                treeView.ShowEditor();
                treeView.EditingValue = Path.GetFileNameWithoutExtension(record.Name);
            }
        }

        private XmlNode GetNodeElement()
        {
            TreeListNode focusedNode = treeView.FocusedNode;
            XmlNode node = (treeView.GetDataRecordByNode(focusedNode) as Models.ProjectTreeNode).Node;
            return node;
        }

        private string GetElementName()
        {
            XmlNode node = GetNodeElement();
            return node.Name;
        }

        private void treeView_HiddenEditor(object sender, EventArgs e)
        {
            treeView.OptionsBehavior.Editable = false;
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                treeView.OptionsBehavior.Editable = true;
                treeView.ShowEditor();
                e.Handled = true;
            }
        }

        private void mnuTreeViewDelete_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode focusedNode = treeView.FocusedNode;
            var record = (Models.ProjectTreeNode)treeView.GetDataRecordByNode(focusedNode);

            if ((MessageBox.Show("Želite obrisati " + record.ItemType + ": " + record.Name, "Brisanje odabranog noda", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {
                treeView.DeleteNode(treeView.FocusedNode);

                List<Models.ProjectTreeNode> treeList = (List<Models.ProjectTreeNode>)treeView.DataSource;

                PopulateNodes(treeList);

                //delete from xml
            }
        }

        private void mnuTreeViewClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode focusedNode = treeView.FocusedNode;
            var record = (Models.ProjectTreeNode)treeView.GetDataRecordByNode(focusedNode);

            treeView.DeleteNode(treeView.FocusedNode);

            List<Models.ProjectTreeNode> treeList = (List<Models.ProjectTreeNode>)treeView.DataSource;

            PopulateNodes(treeList);
            //TODO:
            //zatvori sve dokumente u dokument manageru kojji su otvoreni a unutar su odabranog xmla
        }

        private void mnuTreeViewAddNewFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //dodaj formu sa templateom
        }

        private void treeView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //load xml
            //update xml sa novim nazivom forme / parta
            TreeListNode focusedNode = treeView.FocusedNode;
            var record = (Models.ProjectTreeNode)treeView.GetDataRecordByNode(focusedNode);
            var filepath = record.ItemFile.xDocFile;

            Models.FileDef itemFile = record.ItemFile;

            XmlNode node = GetNodeElement();
            string attr = "name";
            string value = e.Value.ToString();

            if (node != null && node.Attributes[attr] != null)
            {
                if (!String.IsNullOrEmpty(value))
                    node.Attributes[attr].Value = value.ToString();
            }
            StreamWriter outStream = System.IO.File.CreateText(itemFile.xDocFile);
            itemFile.xDoc.Save(outStream);
            outStream.Close();
        }
    }
}