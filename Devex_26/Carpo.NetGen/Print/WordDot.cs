using System;
using System.Reflection;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// WORDDOT1. Form1.vb 5093–5159. Putanja je ostatak imena.
    /// Word.Application, Visible, Documents.Add, Run("Pokreni", idjevi, ODBC niz).
    /// Ako Worda nema, lanac staje. Test može dati Aplikacija umjesto CreateObject.
    /// </summary>
    public static class WordDot
    {
        public static object Aplikacija;

        public static string Putanja(string naredba)
        {
            string s = (naredba ?? "").Trim();
            if (s.StartsWith(":", StringComparison.Ordinal))
                s = s.Substring(1).Trim();
            if (s.StartsWith("WORDDOT1", StringComparison.OrdinalIgnoreCase))
                s = s.Substring("WORDDOT1".Length);
            return s.Trim();
        }

        public static string Veza(string konekcija)
        {
            return "Provider=MSDASQL.1;Extended Properties=\"DRIVER=SQL Server;" + (konekcija ?? "") + "\"";
        }

        public static bool Otvori(string naredba, string konekcija, string[] idjevi)
        {
            string put = Putanja(naredba);
            object word = Aplikacija;
            if (word == null)
            {
                Type tip = Type.GetTypeFromProgID("Word.Application");
                if (tip == null)
                {
                    NgDnevnik.Greska("Greška kod CreateObject(\"Word.Application\")");
                    return false;
                }
                try
                {
                    word = Activator.CreateInstance(tip);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod CreateObject(\"Word.Application\")  " + ex.Message);
                    return false;
                }
            }
            try
            {
                word.GetType().InvokeMember("Visible", BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null, word, new object[] { true });
                object docs = word.GetType().InvokeMember("Documents", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, word, null);
                docs.GetType().InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, docs, new object[] { put });
                word.GetType().InvokeMember("Run", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, word, new object[] { "Pokreni", idjevi ?? new string[0], Veza(konekcija) });
                return true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod Word.Documents.Add ...  " + ex.Message);
                return false;
            }
        }
    }
}
