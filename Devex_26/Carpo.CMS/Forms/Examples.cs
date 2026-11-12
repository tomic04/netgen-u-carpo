using Carpo.Common.Helpers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class Examples : Form
    {
        private string action = "";
        private Color checkDefaultForeColor;
        private TreeState treeState;

        public Examples()
        {
            InitializeComponent();
            treeState = new TreeState(tree);
            TreeInit();
        }

        private void Examples_Load(object sender, EventArgs e)
        {
            gvFiles.UserCellPadding = new Padding(0, 0, 10, 0);
        }

        private void Examples_Shown(object sender, EventArgs e)
        {
            PopulateTree(false);
            tree.ExpandAll();
        }

        private void TreeInit()
        {
            tree.ParentFieldName = "ParentId";
            tree.KeyFieldName = "Id";
            tree.OptionsSelection.EnableAppearanceFocusedCell = false;
            tree.OptionsBehavior.Editable = false;

            tree.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "Name" });
        }

        private void PopulateTree(bool keepFocus)
        {
            try
            {
                tree.BeginUpdate();
                if (keepFocus == true)
                    treeState.SaveState();

                tree.DataSource = Data.Examples.Tree();

                if (keepFocus == true)
                    treeState.LoadState();
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                tree.EndUpdate();
            }
        }

        private void PopulateFiles(bool keepFocus)
        {
            int focusedId = -1;
            if (gvFiles.FocusedRowHandle >= 0 && keepFocus == true)
                focusedId = (int)gvFiles.GetDataRow(gvFiles.FocusedRowHandle)["CarpoCmsExampleFileId"];
            try
            {
                gcFiles.BeginUpdate();
                gcFiles.DataSource = null;
                int id = Convert.ToInt32(tree.FocusedNode["Id"]);
                gcFiles.DataSource = Data.Examples.Files(id);

                gvFiles.Columns[0].BestFit();
                gvFiles.Columns[1].BestFit();
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                if (focusedId >= 0)
                {
                    int idRowHandle = gvFiles.LocateByValue("CarpoCmsExampleFileId", focusedId);
                    gvFiles.FocusedRowHandle = idRowHandle;
                }

                gcFiles.EndUpdate();
            }
        }

        private void PopulateImages()
        {
            galleryImages.Gallery.Groups[0].Items.Clear();

            int id = GetExampleId();
            DataTable dt = Data.Examples.Images(id);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    byte[] imageThumb = (byte[])dr["ContentThumb"];

                    GalleryItem item = new GalleryItem();
                    item.Tag = new Models.ExamplesImage() { Id = Convert.ToInt32(dr["CarpoCmsExampleImageId"]), Index = i };
                    item.Image = Images.ByteArrayToImage(imageThumb);

                    item.ItemClick += (s, e) =>
                    {
                        GalleryItem_Click(s, e);
                    };
                    galleryImages.Gallery.Groups[0].Items.Add(item);
                    i++;
                }
            }
        }

        protected void GalleryItem_Click(object s, EventArgs e)
        {
            using (ExamplesImage frm = new ExamplesImage())
            {
                int imageId = ((s as GalleryItem).Tag as Models.ExamplesImage).Id;
                frm.Id = imageId;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Data.Examples.EditImageName(imageId, frm.TextName);
                }
            }
        }

        private void bbiFIleAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "XML/SQL Files (*.xml; *.sql)|*.xml; *.sql";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int id = Convert.ToInt32(tree.FocusedNode["Id"]);
                foreach (var file in dialog.FileNames)
                {
                    SaveFile(file, id);
                }
            }
        }

        private void tree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (tree.FocusedNode != null)
            {
                try
                {
                    action = "focusednodechanged";
                    bbiTreeAdd.Enabled = e.Node.Level == 0;
                    bbiFIleAdd.Enabled = e.Node.Level == 1;

                    SetDetails();
                }
                finally
                {
                    action = "";
                }
            }
        }

        private void SetDetails()
        {
            TreeListNode node = tree.FocusedNode;

            if (node.Level == 0)
            {
                pnlEmpty.BringToFront();

                lblCntExamples.Text = "<b>" + node["Name"].ToString() + "</b><size=14><color=100,100,100>" + Environment.NewLine + "broj primjera: " + node.Nodes.Count().ToString() + "</color></size>";
            }
            else
            {
                DataRowView dr = (tree.GetDataRecordByNode(tree.FocusedNode) as DataRowView);

                bool visible = (bool)dr["Visible"];
                var description = dr["Description"];

                chkVisible.Checked = visible;

                if (checkDefaultForeColor == null)
                    checkDefaultForeColor = chkVisible.ForeColor;

                if (visible == true)
                    chkVisible.ForeColor = checkDefaultForeColor;
                else
                    chkVisible.ForeColor = Color.FromArgb(200, 0, 0);

                if (description != null)
                    txtDescription.Text = description.ToString();

                PopulateFiles(false);
                PopulateImages();

                splitContainerControl2.BringToFront();
            }
        }

        private void repTxt_Leave(object sender, EventArgs e)
        {
            string name = (sender as DevExpress.XtraEditors.TextEdit).Text;
            int id = GetFileId();
            Data.Examples.EditFileName(id, name);
        }

        private void bbiImageAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            //dialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif";

            dialog.Filter = "Image Files (*.jpg; *.png; *.gif)|*.jpg; *.png; *.gif";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int id = GetExampleId();
                foreach (var file in dialog.FileNames)
                {
                    using (Image image = Bitmap.FromFile(file))
                    {
                        byte[] imageByte = Images.ImageToByteArray(image);
                        //Image imageThumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                        using (Bitmap imageThumb = Images.ResizeImage(image))
                        {
                            //create thumb
                            byte[] imageThumbByte = Images.ImageToByteArray(imageThumb);
                            SaveImage(file, id, imageByte, imageThumbByte);
                        }
                    }
                }
                PopulateImages();
            }
        }

        private void gvFiles_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Point);
            if (hitInfo.InRow)
            {
                view.FocusedRowHandle = hitInfo.RowHandle;
                SetFilePopup();
                popupFile.ShowPopup(barManager1, Control.MousePosition);
            }
        }

        private void tree_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            TreeList tree = sender as TreeList;
            TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
            if (hitInfo.Node != null)
            {
                SetTreePopup(hitInfo.Node);
                tree.FocusedNode = hitInfo.Node;
                popupTree.ShowPopup(barManager1, Control.MousePosition);
            }
        }

        private void galleryImages_Gallery_ItemRightClick(object sender, GalleryItemClickEventArgs e)
        {
            mnuImgDelete.Tag = e.Item.Tag;
            mnuImgDown.Tag = e.Item.Tag;
            mnuImgUp.Tag = e.Item.Tag;

            SetImagePopup(e.Item);
            popupImg.ShowPopup(barManager1, Control.MousePosition);
        }

        private void SetTreePopup(TreeListNode node)
        {
            //reset
            PopupEnableAllItems(popupTree);

            //dopustio kreiranje unutar samo root nodovima
            if (node.Level > 0)
                mnuTreeAddInside.Enabled = false;

            //nemoj dopustiti brisanje ako root node ima djecu
            if (node.Level == 0 && node.HasChildren == true)
                mnuTreeDelete.Enabled = false;
        }

        private void SetImagePopup(GalleryItem galleryItem)
        {
            Models.ExamplesImage tag = (galleryItem.Tag as Models.ExamplesImage);

            //reset
            PopupEnableAllItems(popupImg);
            //dopustio kreiranje unutar samo root nodovima
            if (tag.Index == galleryImages.Gallery.GetAllItems().Count - 1)
                mnuImgDown.Enabled = false;
            else if (tag.Index == 0)
                mnuImgUp.Enabled = false;
        }

        private void SetFilePopup()
        {
            PopupEnableAllItems(popupFile);

            if (gvFiles.FocusedRowHandle == gvFiles.RowCount - 1)
                mnuFileDown.Enabled = false;
            else if (gvFiles.FocusedRowHandle == 0)
                mnuFileUp.Enabled = false;
        }

        private void PopupEnableAllItems(PopupMenu popup)
        {
            foreach (var item in popup.ItemLinks)
            {
                if (item is BarButtonItemLink)
                {
                    (item as BarButtonItemLink).Item.Enabled = true;
                }
            }
        }

        #region Commands

        //*****save*****//
        private void SaveFile(string file, int id)
        {
            if (!System.IO.File.Exists(file))
                return;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            byte[] fileData = System.IO.File.ReadAllBytes(file);

            Data.Examples.AddFile(id, fileData, fileInfo.Extension, fileInfo.Name);

            PopulateFiles(false);
        }

        private void SaveImage(string file, int id, byte[] imageByte, byte[] imageThumbByte)
        {
            if (!System.IO.File.Exists(file))
                return;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            Data.Examples.AddImage(id, imageByte, imageThumbByte, fileInfo.Extension, fileInfo.Name);
        }

        //*****add*****//
        private void AddExample(int? carpoCmsExampleId)
        {
            using (ExamplesNew frm = new ExamplesNew())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Data.Examples.AddExample(carpoCmsExampleId, frm.TextName);
                    PopulateTree(true);
                }
            }
        }

        //*****edit name*****//
        //private void EditExampleName()

        //*****delete*****//
        private void DeleteFile()
        {
            if (Common.Helpers.JConfirm.Confirm("Sigurno želite obrisati datoteku?", "Brisanje", "", "") != DialogResult.Yes)
                return;

            Data.Examples.DeleteFile(GetFileId());
            PopulateFiles(false);
        }

        private void DeleteImage(int id)
        {
            if (Common.Helpers.JConfirm.Confirm("Sigurno želite obrisati sliku?", "Brisanje", "", "") != DialogResult.Yes)
                return;

            Data.Examples.DeleteImage(id);
            PopulateImages();
        }

        #endregion Commands

        #region IdHelpers

        private int GetExampleId()
        {
            if (tree.FocusedNode != null)
                return Convert.ToInt32(tree.FocusedNode["Id"]);

            return -1;
        }

        private int? GetExampleIdParent()
        {
            if (tree.FocusedNode.ParentNode == null)
                return null;

            return (int?)tree.FocusedNode.ParentNode["Id"];
        }

        private int GetFileId()
        {
            if (gvFiles.FocusedRowHandle >= 0)
                return Convert.ToInt32(gvFiles.GetDataRow(gvFiles.FocusedRowHandle)["CarpoCmsExampleFileId"]);
            return -1;
        }

        #endregion IdHelpers



        #region MenuCommands

        private void mnuGridDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DeleteFile();
        }

        private void mnuImgDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DeleteImage((e.Item.Tag as Models.ExamplesImage).Id);
        }

        //*****tree*****//
        private void mnuTreeAddInside_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddExample(GetExampleId());
        }

        private void mnuTreeAddInSameLevel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AddExample(GetExampleIdParent());
        }

        #endregion MenuCommands

        private void mnuTreeDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            int id = GetExampleId();
            if (id > 0)
            {
                try
                {
                    Data.Examples.DeleteExample(id);
                    PopulateTree(true);
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, ex.Message);
                }
            }
        }

        private void tree_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Appearance.ForeColor = Color.FromArgb(60, 60, 60);
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
            }
            else if (e.Node.Level == 1 && (bool)e.Node["Visible"] == false)
                e.Appearance.ForeColor = Color.FromArgb(200, 0, 0);
        }

        private void cmdSpremiOpis_Click(object sender, EventArgs e)
        {
            Data.Examples.EditExampleDescription(GetExampleId(), txtDescription.Text.Trim());
            PopulateTree(true);
        }

        private void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (action == "")
            {
                Data.Examples.EditExampleVisible(GetExampleId(), chkVisible.Checked);
                PopulateTree(true);
            }
        }

        private void mnuFileUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            FilesSort("up");
        }

        private void mnuFileDown_ItemClick(object sender, ItemClickEventArgs e)
        {
            FilesSort("down");
        }

        private void FilesSort(string direction)
        {
            Data.Examples.SortFile(GetFileId(), direction);
            PopulateFiles(true);
        }

        private void mnuImgUp_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImageSort((e.Item.Tag as Models.ExamplesImage).Id, "up");
        }

        private void mnuImgDown_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImageSort((e.Item.Tag as Models.ExamplesImage).Id, "down");
        }

        private void ImageSort(int id, string direction)
        {
            Data.Examples.SortImage(id, direction);
            PopulateImages();
        }

        private void tree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hitInfo = tree.CalcHitInfo(e.Location);

            if (hitInfo.HitInfoType != HitInfoType.Cell)
                return;

            var id = GetExampleId();

            var name = (tree.FocusedNode.GetValue("Name") != null) ? tree.FocusedNode.GetValue("Name").ToString() : "";

            using (ExamplesNew frm = new ExamplesNew(name, "Izmjeni naslov"))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Data.Examples.EditExampleName(id, frm.TextName);
                        PopulateTree(true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Greška kod izmjene naziva noda.{0}{1}", Environment.NewLine, ex.InnerException), "Greška");
                        System.Diagnostics.Debug.WriteLine("tree_MouseDoubleClick " + ex.InnerException);
                    }
                }
            }
        }
    }
}