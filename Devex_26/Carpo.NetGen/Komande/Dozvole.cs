using System;
using System.Data;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// PERMISIO="true" (Form1.vb 3414): ssPermissionProcitaj (2694), ssPermissionProvjeri (2714, gumbi),
    /// desni klik (3577-3610) i ssPermissionProvjeriRafalnuKomandu (2665, forme i stalne stavke izbornika).
    /// Red s dopuštenjem pobjeđuje (kk 21.02.2022). PERMISSIO s dva S se ne čita (DOZ-01).
    /// </summary>
    public static class Dozvole
    {
        public sealed class Stanje
        {
            public bool? Omogucen;
            public bool? Vidljiv;
        }

        public static bool Ukljucene(string permisio)
        {
            return (permisio ?? "").Trim().ToLowerInvariant() == "true";
        }

        /// <summary>2697-2707, verzija od 18.10.2021.</summary>
        public static string Sql(string imeXml, string operaterId)
        {
            string x = (imeXml ?? "").Trim();
            string op = operaterId ?? "";
            return "select *,1 as sort from ssPermission where objectname='" + x + "' and ((OperateriID = " + op
                + ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " + op
                + ") )) union select *,2 as sort from ssPermission where objectname='" + x
                + "' and OperateriID = 0 and grupeid=0 order by sort ,isgrupa desc,grupeid,operateriid";
        }

        /// <summary>
        /// 2665-2692: ime bez ':'; red za funkciju sa smije=False zabrani, smije=True dopusti i izađe.
        /// DBNull u If uvjetu pod On Error Resume Next uđe u obje grane, pa je dopušteno.
        /// </summary>
        public static bool SmijeKomandu(DataTable t, string kontrola)
        {
            bool smije = true;
            if (t == null || t.Rows.Count == 0 || !t.Columns.Contains("functionname"))
                return true;
            string k = (kontrola ?? "").Replace(":", "").Trim().ToLowerInvariant();
            foreach (DataRow r in t.Rows)
            {
                if (Tekst(r, "functionname").Trim().ToLowerInvariant() != k)
                    continue;
                bool? s = Bool(r, "smije");
                if (s != true)
                    smije = false;
                if (s != false)
                    return true;
            }
            return smije;
        }

        /// <summary>
        /// 2740-2775 za Button1..10: objectname i natpis bez '&amp;'. Zabrana i skrivanje samo dok
        /// nijedan red nije dopustio; Vidljivost bez stupca ili s DBNull ne mijenja ništa.
        /// </summary>
        public static Stanje Gumb(DataTable t, string imeXml, string natpis)
        {
            var st = new Stanje();
            if (t == null || t.Rows.Count == 0 || !t.Columns.Contains("functionname"))
                return st;
            string x = (imeXml ?? "").Trim().ToLowerInvariant();
            string n = (natpis ?? "").Trim().ToLowerInvariant().Replace("&", "");
            bool dopusteno = false;
            foreach (DataRow r in t.Rows)
            {
                if (Tekst(r, "objectname").Trim().ToLowerInvariant() != x || Tekst(r, "functionname").Trim().ToLowerInvariant() != n)
                    continue;
                bool? s = Bool(r, "smije");
                if (s != true && !dopusteno)
                    st.Omogucen = false;
                if (s != false)
                {
                    st.Omogucen = true;
                    dopusteno = true;
                }
                bool? v = t.Columns.Contains("Vidljivost") ? Bool(r, "Vidljivost") : null;
                if (v == false && !dopusteno)
                    st.Vidljiv = false;
                if (v == true)
                    st.Vidljiv = true;
            }
            return st;
        }

        /// <summary>3577-3610: stavka desnog klika po natpisu (bez skidanja '&amp;'). Dopušteno izlazi iz petlje.</summary>
        public static Stanje Stavka(DataTable t, string imeXml, string natpis)
        {
            var st = new Stanje();
            if (t == null || t.Rows.Count == 0 || !t.Columns.Contains("functionname"))
                return st;
            string x = (imeXml ?? "").Trim().ToLowerInvariant();
            string n = (natpis ?? "").Trim().ToLowerInvariant();
            foreach (DataRow r in t.Rows)
            {
                if (Tekst(r, "objectname").Trim().ToLowerInvariant() != x || Tekst(r, "functionname").Trim().ToLowerInvariant() != n)
                    continue;
                bool? s = Bool(r, "smije");
                if (s != true)
                    st.Omogucen = false;
                if (s != false)
                {
                    st.Omogucen = true;
                    st.Vidljiv = true;
                    break;
                }
                bool? v = t.Columns.Contains("Vidljivost") ? Bool(r, "Vidljivost") : null;
                if (v == false)
                    st.Vidljiv = false;
                if (v == true)
                    st.Vidljiv = true;
            }
            return st;
        }

        private static string Tekst(DataRow r, string stupac)
        {
            if (!r.Table.Columns.Contains(stupac) || r[stupac] == DBNull.Value)
                return "";
            return Convert.ToString(r[stupac]) ?? "";
        }

        private static bool? Bool(DataRow r, string stupac)
        {
            if (!r.Table.Columns.Contains(stupac))
                return null;
            object v = r[stupac];
            if (v == null || v == DBNull.Value)
                return null;
            if (v is bool)
                return (bool)v;
            string s = Convert.ToString(v).Trim().ToLowerInvariant();
            if (s == "true" || s == "1")
                return true;
            if (s == "false" || s == "0")
                return false;
            return null;
        }
    }
}
