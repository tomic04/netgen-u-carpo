using Carpo.Common.Helpers;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.CMS.Forms
{
    public partial class ElementsNew : Form
    {
        public int Id { get; set; }

        private byte[] imageByte;
        private byte[] imageThumbByte;
        private byte[] fileData;
        private string fileName;
        private string description;
        private string title;
        private string name;

        public ElementsNew()
        {
            InitializeComponent();
        }

        private void btnDodajFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "XML files (*.xml)|*.xml|SQL files (*.sql)|*.sql";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in dialog.FileNames)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                    fileData = System.IO.File.ReadAllBytes(file);
                    fileName = fileInfo.Name;
                    lbl.Text = fileInfo.FullName;
                    btnDelDatoteke.Visible = true;
                }
            }
        }

        private void PopulateFiles(string name)
        {
            //napuni labelu

            lbl.Text = name;
        }

        private void PopulateImages(byte[] imageByte)
        {
            Image image;
            if (imageByte != null)
                image = Common.Helpers.Images.ByteArrayToImage(imageByte);
            else
                image = null;

            pic.Image = image;
        }

        private void cmdSpremi_Click(object sender, EventArgs e)
        {
            var typeid = cmdElementTypes.EditValue;

            if (txtNaziv.Text.Trim() != "")
            {
                if (Id != 0)
                    Data.Elements.UpdateElem(txtNaziv.Text.Trim(), (int)Id, txtDescription.Text.Trim() == "" ? null : txtDescription.Text.Trim(), txtTitle.Text.Trim() == "" ? null : txtTitle.Text.Trim(), imageByte, fileData, fileName == "" ? null : fileName, typeid.CoalesceInt());
                else
                    Data.Elements.SetElem(txtNaziv.Text.Trim(), txtDescription.Text.Trim() == "" ? null : txtDescription.Text.Trim(), txtTitle.Text.Trim() == "" ? null : txtTitle.Text.Trim(), imageByte, fileData, fileName == "" ? null : fileName, typeid.CoalesceInt());

                DialogResult = DialogResult.OK;
            }
            else
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Obavezan unos");
                txtNaziv.Select();
            }
        }

        private void ElementsNew_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        public void InitForm()
        {
            initTypesCombo();
            DataTable dt = Data.Elements.GetElement(Id);
            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["Name"].ToString();
                description = dt.Rows[0]["Description"].ToString();
                title = dt.Rows[0]["Title"].ToString();
                imageByte = dt.Rows[0]["ContentImg"] != DBNull.Value ? (byte[])dt.Rows[0]["ContentImg"] : null;
                fileData = dt.Rows[0]["ContentFile"] != DBNull.Value ? (byte[])dt.Rows[0]["ContentFile"] : null;
                fileName = dt.Rows[0]["ContentFileName"].ToString();
                var elementtypeid = dt.Rows[0]["CarpoDsElementTypeId"];
                //Image image = Helpers.Images.ByteArrayToImage((byte[])dt.Rows[0]["Content"]);
                //pic.Image = image;
                if (name != null)
                    txtNaziv.Text = name.ToString();
                if (title != null)
                    txtTitle.Text = title.ToString();
                if (description != null)
                    txtDescription.Text = description.ToString();
                if (imageByte != null)
                    PopulateImages(imageByte);
                if (fileData != null)
                {
                    PopulateFiles(fileName);
                    btnDelDatoteke.Visible = true;
                }

                if (elementtypeid != DBNull.Value)
                    cmdElementTypes.EditValue = elementtypeid;
            }

            txtNaziv.Select();
        }

        private void initTypesCombo()
        {
            DataTable types = Data.Elements.GetElementTypes();

            DataRow _ravi = types.NewRow();
            _ravi["Id"] = DBNull.Value;
            _ravi["Name"] = " ";
            types.Rows.InsertAt(_ravi, 0);

            cmdElementTypes.Properties.DataSource = types;
            cmdElementTypes.Properties.DisplayMember = "Name";
            cmdElementTypes.Properties.ValueMember = "Id";
            cmdElementTypes.Properties.PopulateColumns();
            cmdElementTypes.Properties.Columns[0].Visible = false;
        }

        private void btnDodajSliku_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in dialog.FileNames)
                {
                    using (Image image = Bitmap.FromFile(file))
                    {
                        imageByte = Images.ImageToByteArray(image);
                        //Image imageThumb = image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
                        using (Bitmap imageThumb = Images.ResizeImage(image))
                        {
                            //create thumb
                            imageThumbByte = Images.ImageToByteArray(imageThumb);
                        }
                    }
                }
                PopulateImages(imageByte);
            }
        }

        private void pic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (imageByte != null)
                {
                    DialogResult result = MessageBox.Show("Želite obrisati sliku?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                    if (result == DialogResult.Yes)
                    {
                        imageByte = null;
                        PopulateImages(imageByte);
                    }
                }
            }
        }

        private void btnDelDatoteke_Click(object sender, EventArgs e)
        {
            if (fileData != null)
            {
                fileName = "Naziv filea...";
                fileData = null;
                PopulateFiles(fileName);
                btnDelDatoteke.Visible = false;
            }
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ElementsNew_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
            else if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}