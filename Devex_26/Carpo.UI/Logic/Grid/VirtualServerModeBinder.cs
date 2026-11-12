using Carpo.UI.Forms.Base;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Carpo.UI.Logic.Grid
{
    /// <summary>
    /// B6: opt-in VirtualServerModeSource binder. Calls paging SP directly;
    /// does not alter default ComponentGrid DataTable GetDataSet path.
    /// Contract: dbo.sp_* (@Skip, @Take, @Sort, @Filter, @TotalCount OUTPUT).
    /// </summary>
    internal sealed class VirtualServerModeBinder : IDisposable
    {
        private readonly JupiterShared form;
        private readonly GridControl gridControl;
        private readonly GridView gridView;
        private readonly string connectionString;
        private readonly string procedureName;
        private readonly int pageSize;

        private VirtualServerModeSource source;
        private int totalCount;
        private string currentSort = "Id ASC";
        private string currentFilter;
        private int bindGeneration;
        private bool disposed;

        public VirtualServerModeBinder(
            JupiterShared form,
            GridControl gridControl,
            GridView gridView,
            string connectionString,
            string procedureName,
            int pageSize = 50)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
            this.gridControl = gridControl ?? throw new ArgumentNullException(nameof(gridControl));
            this.gridView = gridView ?? throw new ArgumentNullException(nameof(gridView));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.procedureName = procedureName?.Trim() ?? throw new ArgumentNullException(nameof(procedureName));
            this.pageSize = pageSize < 1 ? 50 : Math.Min(pageSize, 500);
        }

        public int TotalCount => totalCount;

        public string CurrentSort => currentSort;

        public string CurrentFilter => currentFilter;

        /// <summary>
        /// B6 Iter 4: pull all pages for current sort/filter into one DataTable (print/export hydrate).
        /// Caps at <paramref name="maxRows"/>; sets <paramref name="truncated"/> if more rows exist on server.
        /// </summary>
        public DataTable FetchAllFiltered(out bool truncated, int maxRows = 100000, int pageTake = 500, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            truncated = false;
            if (pageTake < 1)
                pageTake = 500;
            if (pageTake > 500)
                pageTake = 500;
            if (maxRows < 1)
                maxRows = 100000;

            var result = new DataTable();
            int skip = 0;
            string sort = string.IsNullOrWhiteSpace(currentSort) ? "Id ASC" : currentSort;
            string filter = currentFilter;

            while (skip < maxRows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int take = Math.Min(pageTake, maxRows - skip);
                DataTable page = FetchPage(skip, take, sort, filter, out totalCount, cancellationToken);

                if (result.Columns.Count == 0)
                {
                    result = page.Clone();
                }

                foreach (DataRow row in page.Rows)
                    result.ImportRow(row);

                int got = page.Rows.Count;
                skip += got;

                if (got == 0)
                    break;
                if (skip >= totalCount)
                    break;
                if (skip >= maxRows && totalCount > maxRows)
                {
                    truncated = true;
                    break;
                }
            }

            form.Logger.Info("B6 VirtualServerMode",
                "Hydrate FetchAll rows=" + result.Rows.Count + ", total=" + totalCount +
                ", truncated=" + truncated + ", sort=" + sort +
                ", filter=" + (filter ?? "(null)"));

            return result;
        }

        public DataTable FetchPage(int skip, int take, string sort, string filter, out int total, CancellationToken cancellationToken = default)
        {
            total = 0;
            FormsVar.SetVariable(form, "DxVsmSkip", skip);
            FormsVar.SetVariable(form, "DxVsmTake", take);
            FormsVar.SetVariable(form, "DxVsmSort", sort ?? (object)DBNull.Value);
            FormsVar.SetVariable(form, "DxVsmFilter", string.IsNullOrWhiteSpace(filter) ? (object)DBNull.Value : filter);

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add("@Skip", SqlDbType.Int).Value = skip;
                cmd.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                cmd.Parameters.Add("@Sort", SqlDbType.NVarChar, 200).Value = (object)sort ?? DBNull.Value;
                cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrWhiteSpace(filter) ? (object)DBNull.Value : filter.Trim();
                var pTotal = cmd.Parameters.Add("@TotalCount", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;

                conn.Open();
                cancellationToken.ThrowIfCancellationRequested();

                var table = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }

                total = pTotal.Value == DBNull.Value || pTotal.Value == null
                    ? table.Rows.Count
                    : Convert.ToInt32(pTotal.Value);

                return table;
            }
        }

        public void Bind(int initialTotalCount)
        {
            ThrowIfDisposed();
            DisposeSourceOnly();
            bindGeneration++;

            totalCount = initialTotalCount;
            currentSort = "Id ASC";
            currentFilter = null;

            source = new VirtualServerModeSource();
            // Public CLR properties required for DX cell painting (Expando does not work).
            source.RowType = typeof(VsmPageRow);
            source.ConfigurationChanged += OnConfigurationChanged;
            source.MoreRows += OnMoreRows;

            gridControl.DataSource = source;
        }

        private void OnConfigurationChanged(object sender, VirtualServerModeRowsEventArgs e)
        {
            int generation = bindGeneration;
            ApplySortFilterFromConfiguration(e.ConfigurationInfo);

            e.RowsTask = Task.Run(() =>
            {
                if (generation != bindGeneration || e.CancellationToken.IsCancellationRequested)
                    return new VirtualServerModeRowsTaskResult(Array.Empty<VsmPageRow>(), false, null);

                try
                {
                    DataTable table = FetchPage(0, pageSize, currentSort, currentFilter, out totalCount, e.CancellationToken);
                    IList rows = ToRowList(table);
                    bool more = rows.Count > 0 && rows.Count < totalCount;
                    form.Logger.Info("B6 VirtualServerMode",
                        "Configuration page0 rows=" + rows.Count + ", total=" + totalCount);
                    return new VirtualServerModeRowsTaskResult(rows, more, null);
                }
                catch (Exception ex)
                {
                    form.Logger.Error("B6 VirtualServerMode ConfigurationChanged", ex.Message, ex);
                    throw;
                }
            }, e.CancellationToken);
        }

        private void OnMoreRows(object sender, VirtualServerModeRowsEventArgs e)
        {
            int generation = bindGeneration;
            int skip = e.CurrentRowCount;
            string sort = currentSort;
            string filter = currentFilter;

            e.RowsTask = Task.Run(() =>
            {
                if (generation != bindGeneration || e.CancellationToken.IsCancellationRequested)
                    return new VirtualServerModeRowsTaskResult(Array.Empty<VsmPageRow>(), false, null);

                try
                {
                    DataTable table = FetchPage(skip, pageSize, sort, filter, out totalCount, e.CancellationToken);
                    IList rows = ToRowList(table);
                    bool more = skip + rows.Count < totalCount;
                    return new VirtualServerModeRowsTaskResult(rows, more, e.UserData);
                }
                catch (Exception ex)
                {
                    form.Logger.Error("B6 VirtualServerMode MoreRows", ex.Message, ex);
                    throw;
                }
            }, e.CancellationToken);
        }

        private void ApplySortFilterFromConfiguration(VirtualServerModeConfigurationInfo info)
        {
            currentSort = MapSort(info?.SortInfo) ?? "Id ASC";

            string findText = null;
            try
            {
                if (gridView != null)
                    findText = gridView.FindFilterText;
            }
            catch
            {
                findText = null;
            }

            if (!string.IsNullOrWhiteSpace(findText))
                currentFilter = findText.Trim();
            else
                currentFilter = MapFilterToContains(info?.Filter);

            FormsVar.SetVariable(form, "DxVsmSort", currentSort);
            FormsVar.SetVariable(form, "DxVsmFilter",
                string.IsNullOrWhiteSpace(currentFilter) ? (object)DBNull.Value : currentFilter);
        }

        private static string MapSort(ServerModeOrderDescriptor[] sortInfo)
        {
            if (sortInfo == null || sortInfo.Length == 0)
                return "Id ASC";

            var first = sortInfo[0];
            string col = first.SortPropertyName;
            if (string.IsNullOrWhiteSpace(col))
                return "Id ASC";

            col = col.Trim().Trim('[', ']');
            int dot = col.LastIndexOf('.');
            if (dot >= 0 && dot < col.Length - 1)
                col = col.Substring(dot + 1);

            return col + (first.IsDesc ? " DESC" : " ASC");
        }

        private static string MapFilterToContains(DevExpress.Data.Filtering.CriteriaOperator filter)
        {
            if (ReferenceEquals(filter, null))
                return null;

            string s = filter.ToString();
            if (string.IsNullOrWhiteSpace(s) || s == "True" || s == "False")
                return null;

            int q1 = s.IndexOf('\'');
            if (q1 < 0)
                return null;
            int q2 = s.IndexOf('\'', q1 + 1);
            if (q2 <= q1)
                return null;

            string literal = s.Substring(q1 + 1, q2 - q1 - 1).Replace("''", "'").Trim('%');
            return string.IsNullOrWhiteSpace(literal) ? null : literal;
        }

        private static IList ToRowList(DataTable table)
        {
            var list = new List<VsmPageRow>();
            if (table == null)
                return list;

            foreach (DataRow row in table.Rows)
                list.Add(VsmPageRow.FromDataRow(row));

            return list;
        }

        private void DisposeSourceOnly()
        {
            if (source == null)
                return;

            try
            {
                source.ConfigurationChanged -= OnConfigurationChanged;
                source.MoreRows -= OnMoreRows;
                source.Dispose();
            }
            catch { }
            finally
            {
                source = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(VirtualServerModeBinder));
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            bindGeneration++;
            DisposeSourceOnly();
        }

        /// <summary>
        /// CLR row for DX VirtualServerModeSource.RowType (matches sp_DxAudit26_BigGrid_Page).
        /// </summary>
        public sealed class VsmPageRow
        {
            public int Id { get; set; }
            public string Naziv { get; set; }
            public int Status { get; set; }
            public string Kategorija { get; set; }
            public decimal Iznos { get; set; }
            public DateTime Datum { get; set; }
            public int PartnerId { get; set; }
            public string Opis { get; set; }

            public static VsmPageRow FromDataRow(DataRow row)
            {
                var r = new VsmPageRow();
                if (row == null || row.Table == null)
                    return r;

                r.Id = GetInt(row, "Id");
                r.Naziv = GetString(row, "Naziv");
                r.Status = GetInt(row, "Status");
                r.Kategorija = GetString(row, "Kategorija");
                r.Iznos = GetDecimal(row, "Iznos");
                r.Datum = GetDate(row, "Datum");
                r.PartnerId = GetInt(row, "PartnerId");
                r.Opis = GetString(row, "Opis");
                return r;
            }

            private static bool Has(DataRow row, string col) =>
                row.Table.Columns.Contains(col) && row[col] != DBNull.Value && row[col] != null;

            private static int GetInt(DataRow row, string col) =>
                Has(row, col) ? Convert.ToInt32(row[col]) : 0;

            private static decimal GetDecimal(DataRow row, string col) =>
                Has(row, col) ? Convert.ToDecimal(row[col]) : 0m;

            private static DateTime GetDate(DataRow row, string col) =>
                Has(row, col) ? Convert.ToDateTime(row[col]) : DateTime.MinValue;

            private static string GetString(DataRow row, string col) =>
                Has(row, col) ? Convert.ToString(row[col]) : null;
        }
    }
}
