using Carpo.Designer.Forms;
using System.Windows.Forms;

namespace Carpo.Designer.Logic
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string value)
        {
            using (frmCustomAttribute frm = new frmCustomAttribute(value))
            {
                return frm.ShowDialog() == DialogResult.OK ? frm.Value : value;
            }
        }
    }
}