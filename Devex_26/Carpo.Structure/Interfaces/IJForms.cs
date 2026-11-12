using DevExpress.XtraLayout;
using System.Collections.Generic;

namespace Carpo.Structure.Interfaces
{
    public interface IJFormJupiterEditor
    {
        List<LayoutControl> LayoutControlList { get; set; }
        List<IJCustomUC> CustomControls { get; set; }
    }
}