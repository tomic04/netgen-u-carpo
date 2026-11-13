using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Kontrole
{
    /// <summary>
    /// Combo na dijalogu, Form2.vb 2396–2431 i 3455–3464 i 5158–5176.
    /// Stupac 0 je vrijednost, stupac 1 je prikaz. ALLOWEMPTY je samo točno true.
    /// </summary>
    public static class ComboDijalog
    {
        public static bool AllowEmpty(string vrijednost)
        {
            return string.Equals((vrijednost ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool AllowEmpty(NgControl kontrola)
        {
            if (kontrola == null || kontrola.Atributi == null || !kontrola.Atributi.Ima("ALLOWEMPTY"))
                return false;
            return AllowEmpty(kontrola.Atributi.Vrijednost("ALLOWEMPTY"));
        }

        public static void UbaciPrazan(DataTable tablica)
        {
            if (tablica == null || tablica.Columns.Count == 0)
                return;
            DataRow red = tablica.NewRow();
            // Form2.vb 2408 je pod On Error Resume Next. String primi "null".
            // Int stupac iz procedure to ne primi i ostane prazan (DBNull), ne 0.
            if (tablica.Columns[0].DataType == typeof(string))
                red[0] = "null";
            else
                red[0] = DBNull.Value;
            if (tablica.Columns.Count > 1)
            {
                if (tablica.Columns[1].DataType == typeof(string))
                    red[1] = "";
                else
                    red[1] = DBNull.Value;
            }
            tablica.Rows.InsertAt(red, 0);
        }

        /// <summary>Ništa nije odabrano. AllowEmpty šalje null. Inače SelectedValue=1 ako taj red postoji.</summary>
        public static string OdabirAkoNista(DataTable tablica, bool allowEmpty)
        {
            if (allowEmpty)
                return "null";
            if (tablica == null)
                return "null";
            for (int i = 0; i < tablica.Rows.Count; i++)
            {
                if (string.Equals(Convert.ToString(tablica.Rows[i][0]), "1", StringComparison.Ordinal))
                    return "1";
            }
            return "null";
        }

        public static string SqlZaSlanje(string sql, SqlKontekst kontekst)
        {
            return SrediPoslije.Petlja(SqlTekst.Sredi(sql ?? "", kontekst), "", null, false);
        }

        /// <summary>select 'refresh' as ime ponovo puni ime, ne kontrolu koja je kliknuta.</summary>
        public static List<string> RefreshImena(string onclick)
        {
            var lista = new List<string>();
            string s = onclick ?? "";
            int i = 0;
            while (i < s.Length)
            {
                int p = s.IndexOf("refresh", i, StringComparison.OrdinalIgnoreCase);
                if (p < 0)
                    break;
                int kao = s.IndexOf(" as ", p, StringComparison.OrdinalIgnoreCase);
                if (kao < 0 || kao > p + 12)
                    break;
                int a = kao + 4;
                int b = a;
                while (b < s.Length && (char.IsLetterOrDigit(s[b]) || s[b] == '_'))
                    b++;
                if (b > a)
                    lista.Add(s.Substring(a, b - a));
                i = b + 1;
            }
            return lista;
        }

        public static bool OnClickSeSalje(bool jupiter)
        {
            return !jupiter;
        }

        /// <summary>
        /// Form2.vb 2396–2431 i 2503–2504. Procedura ide kao i ostali SQL.
        /// Upis ne ide. Stupac 0 je vrijednost, stupac 1 je natpis.
        /// ALLOWEMPTY ubaci red 0 = null i red 1 = prazan tekst.
        /// </summary>
        public static DataTable Ucitaj(string sql, bool allowEmpty, Func<string, DataTable> citaj)
        {
            if (citaj == null || string.IsNullOrWhiteSpace(sql))
                return null;
            if (ZastitaSql.Tekst(sql).Pise)
                return null;
            DataTable dt = citaj(SqlZaSlanje(sql, new SqlKontekst()));
            if (dt == null || dt.Columns.Count == 0)
                return dt;
            if (allowEmpty)
                UbaciPrazan(dt);
            return dt;
        }
    }
}
