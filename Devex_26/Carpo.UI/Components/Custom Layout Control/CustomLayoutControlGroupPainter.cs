using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.ViewInfo;

namespace Carpo.UI.Components
{
    internal class CustomLayoutControlGroupPainter : LayoutControlGroupPainter
    {
        private CustomLayoutControl _LayoutControl;

        public CustomLayoutControlGroupPainter(CustomLayoutControl lc)
        {
            _LayoutControl = lc;
        }

        protected override void DrawBackground(BaseLayoutItemViewInfo e)
        {
            CustomDrawEventArgs args = new CustomDrawEventArgs(e);
            base.DrawBackground(e);
            _LayoutControl.OnCustomDrawBackground(args);
            if (args.Handled)
                return;
            base.DrawBackground(e);
        }

        protected override void DrawSelection(BaseLayoutItemViewInfo e)
        {
            CustomDrawEventArgs args = new CustomDrawEventArgs(e);
            base.DrawSelection(e);
            _LayoutControl.OnCustomDrawSelection(args);
            if (args.Handled)
                return;
            base.DrawSelection(e);
        }

        protected override void DrawTextArea(BaseLayoutItemViewInfo e)
        {
            CustomDrawEventArgs args = new CustomDrawEventArgs(e);
            base.DrawTextArea(e);
            _LayoutControl.OnCustomDrawTextArea(args);
            if (args.Handled)
                return;
            base.DrawTextArea(e);
        }

        public override void DrawControlsArea(BaseLayoutItemViewInfo e)
        {
            CustomDrawEventArgs args = new CustomDrawEventArgs(e);
            base.DrawControlsArea(e);
            _LayoutControl.OnCustomDrawControlsArea(args);
            if (args.Handled)
                return;
            base.DrawControlsArea(e);
        }
    }
}