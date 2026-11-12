using System;
using System.Reflection;
using Carpo.NetGen.Log;
using Carpo.NetGen.Shell;

namespace Carpo.NetGen.Layout
{
    /// <summary>M1: prvo lijevo stablo + master grid + detail grid. M0 ne crta.</summary>
    public static class LayoutEngine
    {
        public static void Crtaj(ShellLayout raspored)
        {
            throw new NotSupportedException(
                "M1 crta obrazac " + ShellLayout.Obrazac + ". M0 ne otvara formu.");
        }
    }
}

namespace Carpo.NetGen.Commands
{
    /// <summary>M1: ONLOAD koji ne pocinje s ':' ide u grid; ':' otvara formu. Gumbi CMDxCAPTION / BUTTONS.</summary>
    public static class CommandRunner
    {
        public static void Pokreni(string naredba)
        {
            throw new NotSupportedException("M1: komande. M0 samo cuva tekst naredbe.");
        }
    }
}

namespace Carpo.NetGen.Sql
{
    /// <summary>
    /// M1: @@id i @@operateriid, preskok ako @@ preživi. SQL ostaje slobodan tekst.
    /// M0 ne otvara konekciju i ne dira bazu.
    /// </summary>
    public static class SqlBridge
    {
        public static void Exec(string sql)
        {
            throw new NotSupportedException("M1: SQL tekst. M0 ne šalje naredbu serveru.");
        }
    }
}

namespace Carpo.NetGen.Print
{
    /// <summary>
    /// Ispis grida. Form1.vb 4945–4959 izađe u IspisGRIDREPX.
    /// Master 5984–6003: PrintDocumentREPX("master", ..., Driver + strGlobalSQLSERVERlogin, parametri grida).
    /// Detail 6067–6085: "detail" i Driver + Conn. Dijalog 9041–9056: "lodani" i strParametriSvi.
    /// PAGEORIENTATION gleda samo Janus Button7 (6039–6043), ne REPX. LANDSCAPE postane Vertical.
    /// </summary>
    public static class PrintBridge
    {
        public static object Stroj;

        public static string Vrsta(string naredba, bool dijalog)
        {
            string u = (naredba ?? "").Trim();
            if (u.StartsWith(":", StringComparison.Ordinal))
                u = u.Substring(1).Trim();
            if (dijalog && (u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase)))
                return "lodani";
            if (u.StartsWith("PRINTDET", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXDET", StringComparison.OrdinalIgnoreCase))
                return "detail";
            if (u.StartsWith("PRINTMASTER", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXMASTER", StringComparison.OrdinalIgnoreCase)
                || u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase))
                return "master";
            return "";
        }

        /// <summary>Form1.vb 6039. REPX ovu granu ne zove.</summary>
        public static string JanusOrijentacija(string pageOrientation)
        {
            if (string.Equals((pageOrientation ?? "").Trim(), "LANDSCAPE", StringComparison.OrdinalIgnoreCase))
                return "vertical";
            return "horizontal";
        }

        public static bool RepxGledaOrijentaciju()
        {
            return false;
        }

        public static string Centar(string centar, string naslov, bool dijalog)
        {
            if (!string.IsNullOrWhiteSpace(centar))
                return IspisZaglavlje.Desno(centar, null);
            string t = naslov ?? "";
            if (!dijalog)
            {
                int z = t.IndexOf("v[", StringComparison.Ordinal);
                if (z >= 0)
                    return t.Substring(0, z).Trim();
            }
            return t;
        }

        public static string Konekcija(string vrsta, string konekcija, string login)
        {
            if (vrsta == "master")
                return "Driver={SQL Server};" + (login ?? "");
            return "Driver={SQL Server};" + (konekcija ?? "");
        }

        public static bool Ispisi(string naredba, bool dijalog, string id, string operater, string konekcija, string login, string zadnjiSql, string parametriGrida, string parametriSvi, string lijevo, string centar, string desno, string naslov)
        {
            string vrsta = Vrsta(naredba, dijalog);
            if (vrsta.Length == 0)
                return false;
            object o = Stroj;
            if (o == null)
            {
                try
                {
                    Assembly a = Assembly.Load("NetGenExtensions");
                    o = a.CreateInstance("NetGenExtensions.frmGridExPrintingREPX");
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod Assembly.Load(NetGenExtensions) : " + ex.Message);
                    return false;
                }
            }
            if (o == null)
            {
                NgDnevnik.Greska("Greška kod Assembly.Load(NetGenExtensions) : CreateInstance");
                return false;
            }
            if (vrsta != "detail")
            {
                Postavi(o, "PageHeaderLeft", IspisZaglavlje.Desno(lijevo, null));
                Postavi(o, "PageHeaderCenter", Centar(centar, naslov, dijalog || vrsta == "lodani"));
                Postavi(o, "PageHeaderRight", IspisZaglavlje.Desno(desno, null));
            }
            string parametri = vrsta == "lodani" ? (parametriSvi ?? "") : (parametriGrida ?? "");
            string cs = Konekcija(vrsta == "lodani" ? "detail" : vrsta, konekcija, login);
            try
            {
                o.GetType().InvokeMember(
                    "PrintDocumentREPX",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    o,
                    new object[] { vrsta, null, null, null, zadnjiSql ?? "", id ?? "", operater ?? "", cs, parametri, "" });
                return true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod DX ispisa : " + ex.Message);
                return false;
            }
        }

        private static void Postavi(object o, string ime, string vrijednost)
        {
            try
            {
                PropertyInfo p = o.GetType().GetProperty(ime, BindingFlags.Instance | BindingFlags.Public);
                if (p != null && p.CanWrite)
                    p.SetValue(o, vrijednost ?? "", null);
            }
            catch
            {
            }
        }
    }
}
