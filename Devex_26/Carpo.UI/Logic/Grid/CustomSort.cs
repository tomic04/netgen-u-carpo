
using DevExpress.XtraGrid.Views.Base;
using System;

namespace Carpo.UI.Logic.Grid
{
    public class CustomSort : IDisposable
    {
        // stringasint
        public int CustomStringColumnSort(object val1, object val2, CustomColumnSortEventArgs e)
        {
            try
            {               
                string val1Str = val1.ToString().Trim();
                string val2Str = val2.ToString().Trim();

                if ((val1 == null && val2 == null) ||
                    (string.IsNullOrEmpty(val1Str) && string.IsNullOrEmpty(val2Str)))
                {
                    return 0;
                }
                else if (val1 == null ||
                    string.IsNullOrEmpty(val1Str))
                {
                    return 1;
                }
                else if (val2 == null ||
                    string.IsNullOrEmpty(val2Str))
                {
                    return -1;
                }
                else
                {
                    object compareVal1 = Convert.ToInt32(val1);
                    object compareVal2 = Convert.ToInt32(val2);

                    return System.Collections.Comparer.Default.Compare(compareVal1, compareVal2);
                }
            }
            catch (Exception ex)
            {
                return System.Collections.Comparer.Default.Compare(val1, val2);
                throw ex;
            }
        }

        // nullaszero
        public int CustomNumericalColumnSort(object val1, object val2, CustomColumnSortEventArgs e)
        {
            try
            {
                if (val1 == DBNull.Value || val2 == DBNull.Value)
                {
                    if (val1 == DBNull.Value && val2 == DBNull.Value)
                    {
                        return 0;
                    }
                    else
                    {
                        if (val1 == DBNull.Value)
                        {
                            if (Convert.ToDecimal(val2) < 0)
                                return 1;
                            else
                                return -1;
                        }
                        else
                        {
                            if (Convert.ToDecimal(val1) < 0)
                                return -1;
                            else
                                return 1;
                        }
                    }
                }
                else
                    return System.Collections.Comparer.Default.Compare(val1, val2);
            }
            catch (Exception ex)
            {
                return System.Collections.Comparer.Default.Compare(val1, val2);
                throw ex;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Parser() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}