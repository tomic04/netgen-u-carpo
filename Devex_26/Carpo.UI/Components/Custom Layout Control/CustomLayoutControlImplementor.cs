using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Registrator;

namespace Carpo.UI.Components
{
    internal class CustomLayoutControlImplementor : LayoutControlImplementor
    {
        public CustomLayoutControlImplementor(ILayoutControlOwner owner)
            : base(owner)
        {
        }

        public override BaseLayoutItem CreateLayoutItem(LayoutGroup parent)
        {
            return new CustomLayoutControlItem((LayoutControlGroup)parent);
        }

        protected override void InitializePaintStyles()
        {
            ISupportLookAndFeel lookAndFeelOwner = owner.GetISupportLookAndFeel();
            if (lookAndFeelOwner != null)
            {
                PaintStyles.Add(new LayoutOffice2003PaintStyle(lookAndFeelOwner));
                PaintStyles.Add(new LayoutWindowsXPPaintStyle(lookAndFeelOwner));
                PaintStyles.Add(new CustomLayoutSkinPaintStyle(lookAndFeelOwner));
                PaintStyles.Add(new Style3DPaintStyle(lookAndFeelOwner));
                PaintStyles.Add(new UltraFlatPaintStyle(lookAndFeelOwner));
                PaintStyles.Add(new FlatPaintStyle(lookAndFeelOwner));
            }
            lookAndFeelOwner = null;
        }
    }
}