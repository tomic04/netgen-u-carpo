using DevExpress.XtraEditors;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public class Export : IDisposable
    {
        public void ExportTxt(string filePath, string textContent, bool showOpenFileDialogOnSuccess)
        {
            ExportTxt(filePath, textContent, showOpenFileDialogOnSuccess, null);
        }

        /// <summary>
        /// encodingName: vrijednost XML parametra 'encoding' akcije 'text export' (vidi ResolveEncoding).
        /// null/prazno → UTF-8 bez BOM-a (identično dosadašnjem File.WriteAllText(path, text)).
        /// Nepoznat naziv → poruka korisniku, datoteka se ne kreira.
        /// </summary>
        public void ExportTxt(string filePath, string textContent, bool showOpenFileDialogOnSuccess, string encodingName)
        {
            try
            {
                Encoding encoding = ResolveEncoding(encodingName) ?? new UTF8Encoding(false);

                if (!File.Exists(filePath))
                {
                    System.IO.File.WriteAllText(filePath, textContent, encoding);
                    ShowMessageSuccess(filePath, showOpenFileDialogOnSuccess);
                }
                else
                {
                    DialogResult dialogResult = XtraMessageBox.Show("Datoteka sa istim nazivom već postoji. Želite li je prebrisati?", "Upozorenje", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        System.IO.File.WriteAllText(filePath, textContent, encoding);
                        ShowMessageSuccess(filePath, showOpenFileDialogOnSuccess);
                    }
                }
            }
            catch (Exception e)
            {
                Type exceptionType = e.GetType();

                // defaultna poruka
                string errorMsg = "Dogodila se greška prilikom kreiranja datoteke.";
                errorMsg = LocalizeException(exceptionType, errorMsg);

                Carpo.Common.Helpers.JMessages.Error(e, errorMsg);
            }
        }

        /// <summary>
        /// Pretvara vrijednost XML parametra 'encoding' (akcija text export) u Encoding.
        /// null/prazno → null (poziva se default). Prihvaća: ansi, utf-8, utf-8-bom, ascii, unicode/utf-16,
        /// broj code page-a (1250) ili IANA/.NET ime (windows-1250, cp1250, iso-8859-2 ...).
        /// Nepoznato ime → ArgumentException.
        /// </summary>
        public static Encoding ResolveEncoding(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            string key = name.Trim().Trim('\'').Replace(" ", string.Empty).ToLowerInvariant();
            if (key.Length == 0)
                return null;

            switch (key)
            {
                case "ansi":
                case "default":
                    return Encoding.Default;
                case "utf8":
                case "utf-8":
                    return new UTF8Encoding(false);
                case "utf8bom":
                case "utf-8-bom":
                case "utf8-bom":
                    return new UTF8Encoding(true);
                case "ascii":
                    return Encoding.ASCII;
                case "unicode":
                case "utf16":
                case "utf-16":
                    return Encoding.Unicode;
            }

            if (key.StartsWith("cp"))
                key = key.Substring(2);

            try
            {
                int codePage;
                if (int.TryParse(key, out codePage))
                    return Encoding.GetEncoding(codePage);
                return Encoding.GetEncoding(key);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Nepoznat encoding '" + name + "' u parametru 'encoding' akcije 'text export'. Primjeri: windows-1250, ansi, utf-8, utf-8-bom.", ex);
            }
        }

        private string LocalizeException(Type exceptionType, string errorMsg)
        {
            if (exceptionType == typeof(ArgumentException))
            {
                errorMsg = "Neispravan parametar akcije (provjerite 'encoding' i putanju).";
            }
            else if (exceptionType == typeof(ArgumentNullException))
            {
            }
            else if (exceptionType == typeof(PathTooLongException))
            {
                errorMsg = "Putanja do datoteke je predugačka.";
            }
            else if (exceptionType == typeof(DirectoryNotFoundException))
            {
                errorMsg = "Odabrani direktorij ne postoji.";
            }
            else if (exceptionType == typeof(IOException))
            {
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                errorMsg = "Nemate prava kreirati datoteku na odabranoj putanji.";
            }
            else if (exceptionType == typeof(NotSupportedException))
            {
            }
            else if (exceptionType == typeof(System.Security.SecurityException))
            {
                errorMsg = "Nemate prava kreirati datoteku na odabranoj putanji.";
            }

            return errorMsg;
        }

        private void ShowMessageSuccess(string filePath, bool showOpenFileDialogOnSuccess)
        {
            if (!showOpenFileDialogOnSuccess)
                Carpo.Common.Helpers.JMessages.Info("Datoteka uspješno spremljena na lokaciju '" + Path.GetFullPath(filePath) + "'.");
            else
            {
                DialogResult dialogResult = XtraMessageBox.Show("Datoteka uspješno spremljena na lokaciju '" + Path.GetFullPath(filePath) + "'." + Environment.NewLine + "Želite li otvoriti datoteku? ", "Info", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    OpenFile(filePath);
                }
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                Process.Start(filePath);
            }
            catch (Exception e)
            {
                Carpo.Common.Helpers.JMessages.Error(e, e.Message);
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