using System;
using System.Collections.Generic;
using Carpo.NetGen.Grid;

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Desno zaglavlje ispisa. Form1.vb 6001: SrediSQL, pa SrediSQLPosljeNextForme
    /// (Class1.vb 740–852) s DatumjezaIspis, pa Translate. @@tkosmomi nije poznati
    /// token SrediSQL-a (Form1.vb 2482). CB ga zamijeni (Class1.vb 932–966).
    /// Nije datum, pa nema navodnika. Prazno ostaje riječ null (916).
    /// </summary>
    public static class IspisZaglavlje
    {
        public static string Desno(string sirovo, IDictionary<string, string> cb, string jezik)
        {
            string sql = SqlTekst.Sredi(sirovo ?? "", new SqlKontekst());
            sql = sql.Replace("##", "\r\n").Replace("!!", "\r\n");
            int zastita = 0;
            while (sql.IndexOf("@@", StringComparison.Ordinal) >= 0 && zastita < 40)
            {
                zastita++;
                int p = sql.IndexOf("@@", StringComparison.Ordinal);
                string rep = sql.Substring(p) + " ";
                int kraj = rep.IndexOf(' ');
                char[] granice = new char[] { '\'', '%', '#', ',', ')', '*', '/', '\\', '+', '-', '.', '(', '=' };
                for (int i = 0; i < granice.Length; i++)
                {
                    int k = rep.IndexOf(granice[i]);
                    if (k > -1 && k < kraj)
                        kraj = k;
                }
                if (kraj <= 0)
                    break;
                string token = rep.Substring(0, kraj);
                string vrijednost = IzCb(token, cb);
                if (vrijednost.IndexOf(token, StringComparison.Ordinal) >= 0)
                    break;
                sql = sql.Replace(token, vrijednost);
            }
            sql = SqlTekst.Sredi(IspisMotora.Zaglavlje(sql), new SqlKontekst());
            return Prevod.Tekst(sql, jezik);
        }

        public static string Desno(string sirovo, IDictionary<string, string> cb)
        {
            return Desno(sirovo, cb, "");
        }

        /// <summary>Form1.vb 6034. Podnožje ide istim Rafalna_Priprema i Translate kao zaglavlje.</summary>
        public static string Podnozje(string sirovo, IDictionary<string, string> cb)
        {
            return Desno(sirovo, cb, "");
        }

        private static string IzCb(string token, IDictionary<string, string> cb)
        {
            string param = (token ?? "").Trim().Replace("@@", "@");
            string ime = param.TrimStart('@');
            string vrijednost = "";
            if (cb != null)
            {
                foreach (var par in cb)
                {
                    if (string.Equals(par.Key, ime, StringComparison.OrdinalIgnoreCase))
                    {
                        vrijednost = par.Value ?? "";
                        break;
                    }
                }
            }
            if (vrijednost.Trim().Length == 0)
                return "null";
            return vrijednost;
        }
    }
}
