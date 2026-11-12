using DevExpress.XtraLayout;

namespace Carpo.UI.Components
{
    internal class CustomLayoutControlItem : LayoutControlItem
    {
        private bool _HighlightFocusedItemEx;

        public bool HighlightFocusedItemEx
        {
            get
            {
                return _HighlightFocusedItemEx;
            }
            set
            {
                if (HighlightFocusedItemEx == value)
                {
                    return;
                }
                _HighlightFocusedItemEx = value;
            }
        }

        public CustomLayoutControlItem(LayoutControlGroup parent)
            : base(parent)
        {
            _HighlightFocusedItemEx = parent.Owner.OptionsView.HighlightFocusedItem;
        }
    }
}