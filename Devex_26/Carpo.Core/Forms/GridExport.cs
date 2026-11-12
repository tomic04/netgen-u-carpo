using System;
using System.Windows.Forms;

namespace Carpo.Core.Forms
{
    public partial class GridExport : Form
    {
        public Export ExportTip { get; set; }

        public enum Export { FullGrid, SelectedRows }

        public GridExport()
        {
            InitializeComponent();
        }

        private void GridExport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void cmdFullGrid_Click(object sender, EventArgs e)
        {
            ExportTip = Export.FullGrid;
            FormClose();
        }

        private void cmdSelectedRows_Click(object sender, EventArgs e)
        {
            ExportTip = Export.SelectedRows;
            FormClose();
        }

        private void FormClose()
        {
            DialogResult = DialogResult.OK;
        }
    }
}