using Carpo.Structure.Models.Core;
using DevExpress.XtraLayout;

namespace Carpo.UI.Components.Base
{
    public class ComponentSimpleLabel : ComponentBase
    {
        private SimpleLabelItem simpleLabel;
        private new JLabelEdit definition;

        public ComponentSimpleLabel() : base()
        {
        }

        public ComponentSimpleLabel(LayoutControl layoutControl, SimpleLabelItem simpleLabel, object def) : base(layoutControl, simpleLabel, def)
        {
            definition = def as JLabelEdit;
            this.simpleLabel = simpleLabel as SimpleLabelItem;
            Init();
        }

        private void Init()
        {
            simpleLabel.AllowHtmlStringInCaption = true;
            string labelText = "", prefixText = "", sufixText = "";

            if (!string.IsNullOrEmpty(definition.Caption))
                labelText = definition.Caption;
            if (!string.IsNullOrEmpty(definition.Prefix))
                prefixText = definition.Prefix;
            if (!string.IsNullOrEmpty(definition.Sufix))
                sufixText = definition.Sufix;

            simpleLabel.Text = prefixText + labelText + sufixText;
        }
    }
}