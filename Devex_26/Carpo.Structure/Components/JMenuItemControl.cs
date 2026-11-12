using System;
using System.Windows.Forms;

namespace Carpo.Structure.Components
{
    [Serializable()]
    public class JMenuItemControl : ToolStripMenuItem
    {
        public string Type { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        public JMenuItemControl()
        {
        }
    }
}