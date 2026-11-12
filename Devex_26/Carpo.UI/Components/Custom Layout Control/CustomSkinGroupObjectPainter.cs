using DevExpress.Skins;
using DevExpress.Utils.Drawing;

namespace Carpo.UI.Components
{
    internal class CustomSkinGroupObjectPainter : SkinGroupObjectPainter
    {
        public CustomSkinGroupObjectPainter(IPanelControlOwner owner, ISkinProvider provider)
            : base(owner, provider)
        {
        }

        public override void DrawObject(ObjectInfoArgs e)
        {
            base.DrawObject(e);
        }
    }
}