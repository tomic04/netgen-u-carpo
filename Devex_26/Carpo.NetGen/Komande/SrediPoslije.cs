using System;
using System.Collections.Generic;
using System.Globalization;
using Carpo.NetGen.Grid;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Class1.SrediSQLPosljeNextForme (740–853) i NadjiVrijednostuPrevParametrima (856–977).
    /// Form2.SrediSQLsaKolonamaizGrida (8440–8456).
    /// </summary>
    public static class SrediPoslije
    {
        /// <summary>
        /// Button1 (3371–3388), pa Form1 5450–5514. DML ugradi @@ i ne lijepi parametre.
        /// !! gasi lijepljenje. Ostalo se lijepi, pa ide SrediSQL i ova zamjena.
        /// </summary>
        public static string ZaSlanje(string stora, string parametri, SqlKontekst k, IDictionary<string, string> clipboard)
        {
            string s = stora ?? "";
            bool uzvicnik = s.IndexOf("!!", StringComparison.Ordinal) >= 0;
            string bez = s.Replace("!!", "");
            if (Lanac.Prvih6JeDml(bez.TrimStart()))
                return Zamijeni(bez, parametri, k, clipboard, false);
            if (uzvicnik)
                return SqlTekst.Sredi2(bez, k).Trim();
            string spojeno = Lanac.Lijepi(bez, parametri ?? "");
            return Zamijeni(SqlTekst.Sredi(spojeno, k), parametri, k, clipboard, false);
        }

        /// <summary>SrediSQL2, pa petlja @@. Prazan ulaz bez @@ ostaje. Class1.vb 755–852.</summary>
        public static string Zamijeni(string sql, string parametri, SqlKontekst k, IDictionary<string, string> clipboard, bool datumZaIspis)
        {
            string s = SqlTekst.Sredi2(sql, k).Trim();
            if ((parametri ?? "").Trim().Length == 0 && s.IndexOf("@@", StringComparison.Ordinal) < 0)
                return s;
            return Petlja(s, parametri, clipboard, datumZaIspis);
        }

        public static string Petlja(string sql, string parametri, IDictionary<string, string> clipboard, bool datumZaIspis)
        {
            string s = sql ?? "";
            int zastita = 0;
            while (s.IndexOf("@@", StringComparison.Ordinal) >= 0 && zastita < 80)
            {
                zastita++;
                int poc = s.IndexOf("@@", StringComparison.Ordinal);
                string rep = s.Substring(poc) + " ";
                int kraj = rep.IndexOf(' ');
                char[] granice = { '\'', '%', '#', ',', ')', '*', '/', '\\', '+', '-', '.', '(', '=' };
                for (int i = 0; i < granice.Length; i++)
                {
                    int p = rep.IndexOf(granice[i]);
                    if (p > -1 && p < kraj)
                        kraj = p;
                }
                if (kraj <= 0)
                    break;
                string token = rep.Substring(0, kraj);
                string vrijednost = Nadji(token, parametri, clipboard, datumZaIspis);
                if (vrijednost.IndexOf(token, StringComparison.Ordinal) >= 0)
                    break;
                s = s.Replace(token, vrijednost);
                s = s.Replace("¸¸", "''");
            }
            return s;
        }

        /// <summary>@(grid..kolona). Form2.vb 8440–8485. Datum dd.MM.yyyy postaje yyyyMMdd.</summary>
        public static string Kolone(string sql, Func<string, string, string> vrijednost)
        {
            string s = sql ?? "";
            if (s.Length == 0 || s.IndexOf("@(", StringComparison.Ordinal) < 0)
                return s;
            int zastita = 0;
            while (s.IndexOf("@(", StringComparison.Ordinal) >= 0 && zastita < 40)
            {
                zastita++;
                int poc = s.IndexOf("@(", StringComparison.Ordinal);
                int z = s.IndexOf(')', poc);
                if (z < 0)
                    break;
                string token = s.Substring(poc, z - poc + 1);
                string unutra = token.Substring(2, token.Length - 3);
                int tocke = unutra.IndexOf("..", StringComparison.Ordinal);
                string grid = tocke < 0 ? "" : unutra.Substring(0, tocke);
                string kolona = tocke < 0 ? "" : unutra.Substring(tocke + 2);
                string v = vrijednost == null ? "" : (vrijednost(grid, kolona) ?? "");
                s = s.Substring(0, poc) + v + s.Substring(z + 1);
            }
            return s;
        }

        public static string Nadji(string token, string parametri, IDictionary<string, string> clipboard, bool datumZaIspis)
        {
            string param = (token ?? "").Trim().Replace("@@", "@");
            string vrijednost = "";
            string[] dijelovi = (parametri ?? "").Trim().Split(',');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string dio = dijelovi[i].Trim();
                if (param.Length == 0 || dio.Length < param.Length)
                    continue;
                if (!dio.Substring(0, param.Length).Equals(param, StringComparison.OrdinalIgnoreCase))
                    continue;
                int eq = dio.IndexOf('=');
                if (eq < 0)
                    continue;
                vrijednost = dio.Substring(eq + 1);
                if (param.ToLowerInvariant().IndexOf("ajdijeva", StringComparison.Ordinal) >= 0)
                    vrijednost = vrijednost.Replace("~", ",");
                if (param.Length >= 4 && param.Substring(0, 4).ToUpperInvariant().StartsWith("@DAT", StringComparison.Ordinal) && vrijednost.IndexOf('.') >= 0 && !datumZaIspis)
                    vrijednost = SpakujDatum(vrijednost);
                break;
            }
            bool datum = (param.Length >= 4 && param.Substring(0, 4).ToUpperInvariant() == "@DAT")
                || (param.ToUpperInvariant().IndexOf("GRID", StringComparison.Ordinal) >= 0 && param.ToUpperInvariant().IndexOf("DAT", StringComparison.Ordinal) >= 0);
            if (datum)
            {
                if (datumZaIspis)
                {
                    if (vrijednost.IndexOf('.') < 0)
                        vrijednost = DatumZaIspis(vrijednost);
                    else
                        vrijednost = vrijednost.Replace(" 0:00:00", "");
                    vrijednost = vrijednost.Replace("'", "");
                }
                else
                {
                    vrijednost = vrijednost.Replace("'", "");
                    if (vrijednost.Trim().Length > 0 && vrijednost != "null")
                        vrijednost = "'" + vrijednost + "'";
                }
            }
            else
                vrijednost = vrijednost.Replace("'", "");

            if (vrijednost.Trim().Length == 0)
                vrijednost = "null";
            if (param.Trim().ToLowerInvariant() == "@napomena" && vrijednost.Trim().ToLowerInvariant() == "null")
                return "";

            if (vrijednost == "null" || vrijednost.Length == 0)
                vrijednost = IzClipboarda(param, clipboard, datum, datumZaIspis, vrijednost);
            return vrijednost;
        }

        private static string IzClipboarda(string param, IDictionary<string, string> clipboard, bool datum, bool datumZaIspis, string trenutno)
        {
            if (clipboard == null)
                return trenutno;
            string ime = param.TrimStart('@');
            string cb = null;
            foreach (var par in clipboard)
            {
                if (string.Equals(par.Key, ime, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(par.Key, param, StringComparison.OrdinalIgnoreCase))
                {
                    cb = par.Value ?? "";
                    break;
                }
            }
            if (cb == null)
                return trenutno;
            if (datum)
            {
                if (cb.IndexOf('.') >= 0)
                {
                    if (datumZaIspis)
                        return cb.Replace(" 0:00:00", "");
                    string spak = SpakujDatum(cb);
                    return spak == cb ? cb : "'" + spak.Replace("'", "") + "'";
                }
                if (datumZaIspis)
                    return DatumZaIspis(cb);
                if (cb == "null")
                    return cb;
                return "'" + cb + "'";
            }
            string broj = cb.Trim().Replace(",", ".");
            double n;
            if (double.TryParse(broj, NumberStyles.Any, CultureInfo.InvariantCulture, out n))
                return broj;
            return cb;
        }

        private static string SpakujDatum(string s)
        {
            if (s == null || s.Length < 11)
                return s ?? "";
            return s.Substring(7, 4) + s.Substring(4, 2) + s.Substring(1, 2);
        }

        private static string DatumZaIspis(string s)
        {
            if (s == null || s.Length < 9)
                return s ?? "";
            return s.Substring(7, 2) + "." + s.Substring(5, 2) + "." + s.Substring(1, 4);
        }
    }
}
