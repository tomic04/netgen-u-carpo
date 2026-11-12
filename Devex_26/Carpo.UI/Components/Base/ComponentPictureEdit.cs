using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Extensions;
using Carpo.UI.Logic;
using Carpo.UI.Properties;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentPictureEdit : ComponentBase
    {
        private PictureEdit PictureEdit;
        private ContextMenuStrip contextMenu;
        Image nullImage = Resources.no_image_64;
        byte[] nullImageHash;

        ToolStripMenuItem saveImageItem;

        private new JPictureEdit definition;

        public ComponentPictureEdit() : base()
        {
        }

        public ComponentPictureEdit(Control control, object def) : base(control, def)
        {
            definition = def as JPictureEdit;
            nullImageHash = nullImage.GenerateHash(); // ne mogu vjerovati da sam ovo morao dodati...

            if (!(control is PictureEdit))
                return;

            PictureEdit = (control as PictureEdit);


            SetEvents();
            Init();

            contextMenu = new ContextMenuStrip();

            PictureEdit.Properties.ContextMenuStrip = contextMenu;

            saveImageItem = new ToolStripMenuItem("Spremi sliku");
            saveImageItem.Click += SaveImageItem_Click; ;
            contextMenu.Items.Add(saveImageItem);
            saveImageItem.Enabled = false;

            PictureEdit.ImageChanged += PictureEdit_ImageChanged;

            if (definition.RightClick == true)
            {
                InitializeContextMenu();  // Inicijalizacija menija
            }

        }

        private void PictureEdit_ImageChanged(object sender, EventArgs e)
        {
            var currentHash = PictureEdit.Image.GenerateHash();
            if (currentHash.IsEqual(nullImageHash)) saveImageItem.Enabled = false;
            else saveImageItem.Enabled = true;
        }

        private void Init()
        {
            //PictureEdit.BackColor = System.Drawing.Color.WhiteSmoke;

            PictureEdit.Properties.ContextMenuStrip = new ContextMenuStrip();

            if (!string.IsNullOrEmpty(definition.dbimg))
            {
                using (Parser parser = new Parser(form, null))
                {
                    var byteArray = parser.GetLogicPartValue(definition.dbimg);
                    if (byteArray is byte[] && byteArray != null)
                        PictureEdit.Image = ConvertToImage(byteArray as byte[]);
                }
            }

            if (!string.IsNullOrEmpty(definition.localimg) && !string.IsNullOrEmpty(definition.localfolder))
                SetControlLocalImage(definition.localimg);

            if (!string.IsNullOrEmpty(definition.sizemode) && definition.sizemode.ToLower() == "stretch")
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            else
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
        }

        private void InitializeContextMenu()
        {
            // Kreiranje novog ContextMenuStrip-a


            // Dodavanje opcije "Učitaj sliku"
            var loadImageItem = new ToolStripMenuItem("Učitaj sliku");
            loadImageItem.Click += LoadImageItem_Click;  // Registracija događaja za klik

            // Dodavanje opcije "Briši sliku"
            var deleteImageItem = new ToolStripMenuItem("Obriši sliku");
            deleteImageItem.Click += DeleteImageItem_Click;  // Registracija događaja za klik


            // Dodavanje opcija u meni
            contextMenu.Items.Add(loadImageItem);

            contextMenu.Items.Add(deleteImageItem);

            // Povezivanje menija sa PictureEdit kontrolom
            //PictureEdit.Properties.ContextMenuStrip = contextMenu;
        }

        private void SaveImageItem_Click(object sender, EventArgs e)
        {
            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Izaberite sliku";
            saveFileDialog.FileName = "Untitled.png";
            saveFileDialog.Filter = "Image File|*.png;";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                PictureEdit.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void LoadImageItem_Click(object sender, EventArgs e)
        {
            // Kreiranje i konfiguracija OpenFileDialog-a
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Izaberite sliku";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Učitavanje slike u PictureEdit kontrolu
                    PictureEdit.Image = Image.FromFile(openFileDialog.FileName);

                    // Opcionalno: Čuvanje lokalne putanje do slike
                    definition.localimg = Path.GetFileName(openFileDialog.FileName);
                    definition.localfolder = Path.GetDirectoryName(openFileDialog.FileName);
                }
            }
        }

        private void DeleteImageItem_Click(object sender, EventArgs e)
        {
            // Brisanje slike iz PictureEdit kontrole
            PictureEdit.Image = null;

            // Takođe, možete resetovati lokalne varijable ako je potrebno
            definition.localimg = null;
            definition.localfolder = null;
        }

        public void SetControlLocalImage(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    PictureEdit.Image = null;
                    return;
                }
                using (Parser parser = new Parser(form, null))
                {
                    var localFolder = parser.GetLogicPartValue(definition.localfolder);

                    if (file.StartsWith("~") || file.StartsWith("#") || file.StartsWith("$"))
                    {
                        var tempImg = parser.GetLogicPartValue(file);
                        if (tempImg == null)
                            return;
                        else
                            definition.localimg = tempImg.ToString();
                    }
                    else
                        definition.localimg = file;

                    if (File.Exists(localFolder.ToString() + @"\\" + definition.localimg))
                        PictureEdit.Image = Image.FromFile(localFolder.ToString() + @"\\" + definition.localimg);
                    else
                        PictureEdit.Image = null;
                }
            }
            catch (Exception ex)
            {
                PictureEdit.Image = null;
                form.Logger.Warn("'" + definition.Name + "' - SetControlLocalImage()", ex.Message);
            }
        }

        public void ChangeControlImage(byte[] byteArray)
        {
            try
            {
                if (byteArray != null)
                    PictureEdit.Image = ConvertToImage(byteArray);
                else
                    PictureEdit.Image = null;
            }
            catch (Exception ex)
            {
                PictureEdit.Image = null;
                form.Logger.Warn("'" + definition.Name + "' - ChangeControlImage()", ex.Message);
            }
        }

        public static Image ConvertToImage(byte[] byteArray)
        {
            Image rImage = null;
            MemoryStream ms = new MemoryStream(byteArray);
            rImage = Image.FromStream(ms);

            return rImage;
        }

        private void SetEvents()
        {
            if (definition.DisplayNoImage == true)
            {
                PictureEdit.Paint += (s, e) =>
                {
                    if (PictureEdit.EditValue == null || PictureEdit.EditValue == DBNull.Value)
                    {
                        PictureEdit.Image = nullImage;
                        PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
                    }

                };
            }
        }

        public void SetEnabled(bool enabled)
        {
            PictureEdit.Enabled = enabled;
        }
    }
}
