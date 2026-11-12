using DevExpress.XtraNavBar;
using System.Windows.Forms;

namespace Carpo.Designer.Modules
{
    public partial class ucToolbox : UserControl
    {
        public ucToolbox()
        {
            InitializeComponent();
            NavBarGroup standardGroup = navBar.Groups.Add();
            standardGroup.Caption = "Standard";
            NavBarGroup devexpressGroup = navBar.Groups.Add();
            devexpressGroup.Caption = "DevExpress";
            devexpressGroup.Expanded = true;
            foreach (string key in toolboxImages.Images.Keys)
            {
                NavBarItem item = new NavBarItem();
                item.Caption = GetCaption(key);
                item.SmallImageIndex = toolboxImages.Images.IndexOfKey(key);
                navBar.Items.Add(item);
                if (!key.Contains("DX"))
                    standardGroup.ItemLinks.Add(item);
                else
                    devexpressGroup.ItemLinks.Add(item);
            }
        }

        private string GetCaption(string key)
        {
            return key.Substring(0, key.IndexOf('_'));
        }
    }
}