using DevExpress.XtraGrid.Columns;

namespace Carpo.Common.Helpers
{
    public class ColumnPropertiesModel
    {
        public GridColumn Column { get; set; }
        public FixedStyle ColumnFixedSide { get; set; }
        public int ColumnVisibleIndex { get; set; }
        public int ColumnMinWidth { get; set; }
        public int ColumnMaxWidth { get; set; }
    }
}