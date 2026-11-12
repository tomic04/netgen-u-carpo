using DevExpress.XtraLayout.ViewInfo;

namespace Carpo.UI.Components
{
    public class CustomDrawEventArgs
    {
        public CustomDrawEventArgs(BaseLayoutItemViewInfo args)
        {
            _Args = args;
        }

        private BaseLayoutItemViewInfo _Args;

        public BaseLayoutItemViewInfo Args
        {
            get { return _Args; }
        }

        private bool _Handled;

        public bool Handled
        {
            get { return _Handled; }
            set { _Handled = value; }
        }
    }

    public delegate void CustomDrawEventHandler(object sender, CustomDrawEventArgs e);
}