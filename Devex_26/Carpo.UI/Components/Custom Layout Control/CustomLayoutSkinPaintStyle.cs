using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Registrator;

namespace Carpo.UI.Components
{
    internal class CustomLayoutSkinPaintStyle : LayoutSkinPaintStyle
    {
        private CustomLayoutControl _LayoutControl;

        public CustomLayoutSkinPaintStyle(DevExpress.LookAndFeel.ISupportLookAndFeel owner)
            : base(owner)
        {
            _LayoutControl = owner as CustomLayoutControl;
        }

        public override DevExpress.XtraLayout.Painting.LayoutControlItemSkinPainter GetControlItemSkinPainter()
        {
            if (layoutControlItemSkinPainter == null)
            {
                layoutControlItemSkinPainter = new CustomLayoutControlItemSkinPainter();
            }
            return layoutControlItemSkinPainter;
        }

        public override LayoutControlGroupPainter GetControlGroupPainter()
        {
            return new CustomLayoutControlGroupPainter(_LayoutControl);
        }

        public override GroupObjectPainter CreateGroupPainter(IPanelControlOwner owner)
        {
            return new CustomSkinGroupObjectPainter(owner, LookAndFeel);
        }
    }
}