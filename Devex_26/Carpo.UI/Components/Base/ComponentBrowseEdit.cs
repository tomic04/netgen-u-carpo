using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentBrowseEdit : ComponentBase
    {
        private ButtonEdit browseEdit;
        //private bool isDark = false;

        private new JBrowseEdit definition;

        public string SearchText { get; set; }

        public ComponentBrowseEdit() : base()
        {
        }

        public ComponentBrowseEdit(Control control, object def) : base(control, def)
        {
            definition = def as JBrowseEdit;
            this.browseEdit = (JBrowseEditControl)control;
            Init();
            SetValue(browseEdit as IJEditor);
        }

        private void Init()
        {
            if (!string.IsNullOrEmpty(definition.TextEditStyle))
            {
                if (definition.TextEditStyle.ToLower() == "disabletexteditor")
                    browseEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                else if (definition.TextEditStyle.ToLower() == "hidetexteditor")
                    browseEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            }

            SetEvents();
        }

        private void SetEvents()
        {
            browseEdit.ButtonClick += (s, e) =>
            {
                Browse();
            };

            browseEdit.DoubleClick += (s, e) =>
            {
                Browse();
            };
        }

        private void Browse()
        {
            if (!string.IsNullOrEmpty(definition.Type) && definition.Type.ToLower() == "folder")
                GetFolder();
            else
                GetFile();
        }

        private void GetFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                browseEdit.EditValue = dlg.FileName;
            }
        }

        private void GetFolder()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                browseEdit.EditValue = dlg.SelectedPath;
            }
        }
    }
}