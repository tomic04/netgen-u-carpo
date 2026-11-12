using Carpo.Structure.Models.Core;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic.Layout
{
    public class SetLayoutGroup
    {
        private const SizeType defaultSizeType = SizeType.AutoSize;
        private BaseLayoutItem layoutControlGroup;
        private Forms.Base.JupiterEditor form;

        public SetLayoutGroup(LayoutControlGroup layoutControlGroup, Forms.Base.JupiterEditor form)
        {
            this.layoutControlGroup = layoutControlGroup;
            this.form = form;
        }

        public void SetLayoutGroupDefinition(JLayoutGroup group)
        {
            DateTime startTime = DateTime.Now;
            List<JRow> rowList = null;
            List<JColumn> colList = null;

            if (group.Col != null && group.Row != null)
            {
                using (Parser parser = new Parser(form, null))
                {
                    object objCol = parser.GetLogicPartValue(group.Col, null);
                    if (objCol != null)
                        layoutControlGroup.OptionsTableLayoutItem.ColumnIndex = Convert.ToInt32(objCol) - 1;

                    object objRow = parser.GetLogicPartValue(group.Row, null);
                    if (objRow != null)
                        layoutControlGroup.OptionsTableLayoutItem.RowIndex = Convert.ToInt32(objRow) - 1;
                }
            }

            if (group.RowSpan != null && group.RowSpan > 1)
                layoutControlGroup.OptionsTableLayoutItem.RowSpan = (int)group.RowSpan;
            if (group.ColSpan != null && group.ColSpan > 1)
                layoutControlGroup.OptionsTableLayoutItem.ColumnSpan = (int)group.ColSpan;

            if (layoutControlGroup.IsGroup == true)
            {
                Tuple<double, SizeType> defSize = Tuple.Create(20D, SizeType.AutoSize);

                if (group.RowsDefinition != null)
                {
                    if (group.RowsDefinition.Size != null)
                    {
                        defSize = GetSize(group.RowsDefinition.Size);
                    }
                    rowList = group.RowsDefinition.RowList;
                }
                else
                {
                    List<JRow> rows = GetRowsDefinition(group);
                    if (rows != null)
                        rowList = rows;
                }

                Tuple<double, SizeType> sizeT;

                int i = 1;

                ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.RowDefinitions.BeginUpdate();

                try
                {
                    int? groupRows = GetCellsCount(group.Rows);
                    if (groupRows == null)
                        return;

                    while (i <= groupRows)
                    {
                        RowDefinition def = new RowDefinition();
                        object size = null;
                        if (rowList != null)
                        {
                            size = rowList.Where(x => x.Position == i)
                                .Select(x => x.Size)
                                .DefaultIfEmpty(null).First();
                            if (size == null && rowList.Count == groupRows)
                            {
                                size = rowList.ElementAt(i - 1).Size;
                            }
                        }

                        if (size != null)
                            sizeT = GetSize(size.ToString());
                        else
                            sizeT = defSize;

                        def.SizeType = sizeT.Item2;
                        def.Height = sizeT.Item1;

                        ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.RowDefinitions.Add(def);
                        i += 1;
                    }
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, ex.Message);
                }
                finally
                {
                    ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.RowDefinitions.EndUpdate();
                }

                defSize = Tuple.Create(20D, SizeType.AutoSize);

                var lstWidth = group.ItemList.Where(x => !string.IsNullOrEmpty(x.Width)).OrderBy(x => x.Col);

                if (group.ColumnsDefinition != null)
                {
                    if (group.ColumnsDefinition.Size != null)
                    {
                        defSize = GetSize(group.ColumnsDefinition.Size);
                    }
                    colList = group.ColumnsDefinition.ColumnList;
                }
                else
                {
                    List<JColumn> cols = GetColsDefinition(group);
                    if (cols != null)
                        colList = cols;
                }

                i = 1;

                ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.ColumnDefinitions.BeginUpdate();
                try
                {
                    int? groupCols = GetCellsCount(group.Cols);
                    if (groupCols == null)
                        return;

                    while (i <= groupCols)
                    {
                        ColumnDefinition def = new ColumnDefinition();
                        object size = null;
                        if (colList != null)
                        {
                            size = colList.Where(x => x.Position == i)
                                .Select(x => x.Size)
                                .DefaultIfEmpty(null).First();
                            if (size == null && colList.Count == groupCols)
                            {
                                size = colList.ElementAt(i - 1).Size;
                            }
                        }

                        if (size != null)
                            sizeT = GetSize(size.ToString());
                        else
                            sizeT = defSize;

                        def.SizeType = sizeT.Item2;
                        //if (sizeT.Item1 != null)
                        //{
                        def.Width = (Convert.ToDouble(sizeT.Item1));
                        //}

                        ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.ColumnDefinitions.Add(def);
                        i += 1;
                    }
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, ex.Message);
                }
                finally
                {
                    ((LayoutControlGroup)layoutControlGroup).OptionsTableLayoutGroup.ColumnDefinitions.EndUpdate();
                }
            }
            //form.Logger.InfoData("SetLayoutGroupDefinition", "", startTime);
        }

        private int? GetCellsCount(string attributValue)
        {
            int val;
            bool isNumeric = int.TryParse(attributValue, out val);

            if (isNumeric == false)
            {
                Parser parser = new Parser(form, null);
                object parserValue = parser.GetLogicPartValue(attributValue);
                if (parserValue != null)
                {
                    isNumeric = int.TryParse(parserValue.ToString(), out val);
                }
            }

            if (isNumeric == true)
                return val;

            form.Logger.Error("SetLayoutGroupDefinition", "nemoguće parsirati 'group.Rows' atribut");
            return null;
        }

        private List<JRow> GetRowsDefinition(JLayoutGroup group)
        {
            var lstItems = group.ItemList.Where(x => !string.IsNullOrEmpty(x.Height)).OrderBy(x => x.Row);
            var lstGroups = group.GroupList.Where(x => !string.IsNullOrEmpty(x.Height)).OrderBy(x => x.Row);
            var lstTabs = group.TabsList.Where(x => !string.IsNullOrEmpty(x.Height)).OrderBy(x => x.Row);

            List<JLayoutShared> lstJoined = new List<JLayoutShared>();
            foreach (JLayoutItem item in lstItems)
                lstJoined.Add(item as JLayoutShared);
            foreach (JLayoutGroup item in lstGroups)
                lstJoined.Add(item as JLayoutShared);
            foreach (JLayoutTabs item in lstTabs)
                lstJoined.Add(item as JLayoutShared);

            if (lstJoined.Count > 0)
            {
                List<JRow> rows = new List<JRow>();
                var lst = lstJoined.OrderBy(x => x.Row);
                foreach (JLayoutShared item in lst)
                {
                    if (item.Row != null)
                    {
                        using (Parser parser = new Parser(form, null))
                        {
                            object objRow = parser.GetLogicPartValue(item.Row, null);
                            if (objRow != null)
                                rows.Add(new JRow() { PositionStr = objRow.ToString(), Size = item.Height });
                        }
                    }
                }
                return rows;
            }

            return null;
        }

        private List<JColumn> GetColsDefinition(JLayoutGroup group)
        {
            var lstItems = group.ItemList.Where(x => !string.IsNullOrEmpty(x.Width));
            var lstGroups = group.GroupList.Where(x => !string.IsNullOrEmpty(x.Width));
            var lstTabs = group.TabsList.Where(x => !string.IsNullOrEmpty(x.Width));

            List<JLayoutShared> lstJoined = new List<JLayoutShared>();
            foreach (JLayoutItem item in lstItems)
                lstJoined.Add(item as JLayoutShared);
            foreach (JLayoutGroup item in lstGroups)
                lstJoined.Add(item as JLayoutShared);
            foreach (JLayoutTabs item in lstTabs)
                lstJoined.Add(item as JLayoutShared);

            if (lstJoined.Count > 0)
            {
                List<JColumn> cols = new List<JColumn>();
                var lst = lstJoined.OrderBy(x => x.Col);
                foreach (JLayoutShared item in lst)
                {
                    if (item.Col != null)
                    {
                        using (Parser parser = new Parser(form, null))
                        {
                            object objCol = parser.GetLogicPartValue(item.Col, null);
                            if (objCol != null)
                                cols.Add(new JColumn() { PositionStr = objCol.ToString(), Size = item.Width });
                        }
                    }
                }
                return cols;
            }

            return null;
        }

        private static Object GetSizeValue()
        {
            return null;
        }

        private static Tuple<double, SizeType> GetSize(string size)
        {
            SizeType sizeType = SizeType.AutoSize;
            double sizeValue = 20;
            if (size.Contains("%"))
            {
                var value = Common.Helpers.Conversion.Parse(typeof(int), size.Replace("%", ""));
                if (value != null && Convert.ToInt32(value) <= 100)
                {
                    sizeType = SizeType.Percent;
                    sizeValue = Convert.ToDouble(value);
                }
            }
            else
            {
                var value = Common.Helpers.Conversion.Parse(typeof(int), size);
                if (value != null)
                {
                    sizeType = SizeType.Absolute;
                    sizeValue = Convert.ToDouble(value);
                }
            }

            return Tuple.Create(sizeValue, sizeType);
        }
    }
}