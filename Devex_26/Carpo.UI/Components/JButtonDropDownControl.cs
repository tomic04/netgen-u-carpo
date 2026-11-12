using Carpo.Structure.Models.Core;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    [Browsable(true)]
    public partial class JButtonDropDownControl : JButtonControl
    {
        //public Models.Core.JButtonDropDown Definition { get; set; }

        public JButtonDropDownControl()
        {
            InitializeComponent();
            //InitMaster();
        }

        public JButtonDropDownControl(JButton definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        public JButtonDropDownControl(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        //public void InitMaster()
        //{
        //    DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
        //    Font = new System.Drawing.Font(Font.FontFamily, 8F, System.Drawing.FontStyle.Regular);
        //    Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //    Width = 90;
        //}
    }
}