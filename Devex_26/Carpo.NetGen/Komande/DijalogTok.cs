using System;
using System.Collections.Generic;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Dijalog kako Form2 radi: ONCLICK, CMD1 = OK, FILLEXPLORER, combo/grid SQL, NEXTFORM.
    /// Na jupiter2019 i jupiter2022 gumb i ONSUBMIT se ne šalju.
    /// </summary>
    public static class DijalogTok
    {
        public sealed class Punjenje
        {
            public string Ime = "";
            public string Sql = "";
            public bool Preskoceno;
        }

        public sealed class Klik
        {
            public string OtvaraFormu = "";
            public bool Zatvara;
            public bool Odustani;
            public bool FillExplorer;
            public string MasterSql = "";
            public bool SaljeSql;
            public string NextForm = "";
            public readonly List<string> Naredbe = new List<string>();
        }

        /// <summary>
        /// Jedan segment Rafalna. Form2.vb 7031–7169 ne izlazi iz petlje na MSGBOX ni na CLOSEPARENT.
        /// </summary>
        public sealed class Korak
        {
            public string Vrsta = "";
            public string Tekst = "";
        }

        public static bool JeJupiter(string server)
        {
            if (string.IsNullOrEmpty(server))
                return false;
            return server.IndexOf("jupiter2019", StringComparison.OrdinalIgnoreCase) >= 0
                || server.IndexOf("jupiter2022", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string NextForm(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("NEXTFORM"))
                return "";
            return (forma.Atributi.Vrijednost("NEXTFORM") ?? "").Trim();
        }

        /// <summary>FILLEXPLORER=onlytree samo NapuniDrvoVrijeme. Form1.vb 5475–5476.</summary>
        public static bool OsvjeziStablo(NgForm forma)
        {
            return string.Equals(FillExplorer(forma), "onlytree", StringComparison.Ordinal);
        }

        /// <summary>FILLEXPLORER=ONLYDET ne dira master. Osvježi detail. Form1.vb 5470–5473.</summary>
        public static bool OsvjeziDetalj(NgForm forma)
        {
            return string.Equals(FillExplorer(forma), "onlydet", StringComparison.Ordinal);
        }

        private static string FillExplorer(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("FILLEXPLORER"))
                return "";
            return (forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim().ToLowerInvariant();
        }

        /// <summary>FILLEXPLORER=TRUE zamijeni master sa ONSUBMIT. Form1.vb 5445–5453.</summary>
        public static string SqlNakonOk(NgForm forma)
        {
            if (!PuniExplorer(forma))
                return "";
            string stora = forma == null ? "" : (forma.OnSubmit ?? "").Trim();
            if (stora.Length == 0)
                return "";
            return Lanac.Lijepi(stora, "");
        }

        public static bool PuniExplorer(NgForm forma)
        {
            if (forma == null || forma.Atributi == null || !forma.Atributi.Ima("FILLEXPLORER"))
                return false;
            return string.Equals((forma.Atributi.Vrijednost("FILLEXPLORER") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Forma bez kontrola i bez gumba ne otvara prozor. OKClick i dalje ima PokaziUpit.
        /// Form1.vb 5369–5370, 5519.
        /// </summary>
        public static bool OtvaraProzor(NgForm forma)
        {
            if (forma == null)
                return false;
            if (forma.Kontrole != null && forma.Kontrole.Count > 0)
                return true;
            if (forma.Gumbi != null && forma.Gumbi.Count > 0)
                return true;
            return false;
        }

        public static List<Punjenje> PriOtvori(NgForm forma)
        {
            var lista = new List<Punjenje>();
            if (forma == null || forma.Kontrole == null)
                return lista;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k.Grana != NgGrana.Combo && k.Grana != NgGrana.Grid && k.Grana != NgGrana.DxGrid)
                    continue;
                var p = new Punjenje();
                p.Ime = k.Name ?? "";
                if (k.Grana != NgGrana.Combo && Skip(k))
                {
                    p.Preskoceno = true;
                    lista.Add(p);
                    continue;
                }
                if (k.Atributi != null && k.Atributi.Ima("SQL"))
                    p.Sql = (k.Atributi.Vrijednost("SQL") ?? "").Trim();
                lista.Add(p);
            }
            return lista;
        }

        public static Klik Gumb(NgForm forma, int redni, string onClick, bool jupiter)
        {
            var k = new Klik();
            k.NextForm = NextForm(forma);
            string naredba = (onClick ?? "").Trim();
            if (naredba.Length == 0 && redni == 2)
            {
                k.Odustani = true;
                k.Zatvara = true;
                k.NextForm = "";
                return k;
            }
            if (naredba.Length == 0 && redni == 1)
            {
                k.Zatvara = true;
                k.FillExplorer = PuniExplorer(forma);
                if (k.FillExplorer && forma != null)
                    k.MasterSql = Lanac.Lijepi((forma.OnSubmit ?? "").Trim(), "");
                k.SaljeSql = !jupiter && k.MasterSql.Length > 0 && !ZastitaSql.Tekst(k.MasterSql).Pise;
                return k;
            }
            string[] seg = Lanac.Segmenti(naredba);
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                if (t.Length == 0)
                    continue;
                if (t.StartsWith(":", StringComparison.Ordinal))
                {
                    k.OtvaraFormu = t.Substring(1).Trim();
                    k.Zatvara = false;
                    k.SaljeSql = false;
                    continue;
                }
                string u = t.ToUpperInvariant();
                if (u == "CLOSE" || u == "CANCEL")
                {
                    k.Odustani = true;
                    k.Zatvara = true;
                    k.NextForm = "";
                    k.SaljeSql = false;
                }
                else if (u == "OK")
                {
                    k.Zatvara = true;
                    if (forma != null)
                        k.MasterSql = Lanac.Lijepi((forma.OnSubmit ?? "").Trim(), "");
                    if (!jupiter && k.MasterSql.Length > 0 && !ZastitaSql.Tekst(k.MasterSql).Pise)
                        k.SaljeSql = true;
                }
                else if (u == "OKSTAY")
                {
                    k.Zatvara = false;
                }
                else
                {
                    string svedeno = Lanac.Lijepi(t, "");
                    k.Naredbe.Add(svedeno);
                    if (!jupiter && !ZastitaSql.Tekst(svedeno).Pise)
                        k.SaljeSql = true;
                }
            }
            if (jupiter)
                k.SaljeSql = false;
            return k;
        }

        /// <summary>
        /// Redoslijed je cijeli ONCLICK. MSGBOX ne prekida (7165–7169).
        /// CLOSEPARENT postavi zastavicu i ide u Button2, petlja ide dalje (7035–7037).
        /// </summary>
        public static List<Korak> Koraci(string onClick)
        {
            var lista = new List<Korak>();
            string[] seg = Lanac.Segmenti(onClick ?? "");
            for (int i = 0; i < seg.Length; i++)
            {
                string t = (seg[i] ?? "").Trim();
                if (t.Length == 0)
                    continue;
                var k = new Korak();
                k.Tekst = t;
                string u = t.ToUpperInvariant();
                if (ForeachNaredba.Vrsta(t).Length > 0)
                {
                    string tijelo = "";
                    if (i + 1 < seg.Length)
                    {
                        tijelo = (seg[i + 1] ?? "").Trim();
                        i++;
                    }
                    k.Vrsta = "foreach";
                    k.Tekst = ForeachNaredba.Vrsta(t) + "|" + ForeachNaredba.Grid(t) + "|" + tijelo;
                }
                else if (u.IndexOf("BEEP()", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "beep";
                else if (u.IndexOf("MSGBOX(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "poruka";
                    k.Tekst = PorukaNaredba.Tekst(t);
                }
                else if (FiskalNaredba.Tablica(t) != 0)
                    k.Vrsta = "fiskal";
                else if (RafalNaredba.Datoteka(t) != 0)
                    k.Vrsta = "datoteka";
                else if (RafalNaredba.RefreshKontrola(t).Length > 0)
                {
                    k.Vrsta = "refresh";
                    k.Tekst = RafalNaredba.RefreshKontrola(t);
                }
                else if (RafalNaredba.Dock(t).Length > 0)
                {
                    k.Vrsta = "dock";
                    k.Tekst = RafalNaredba.Dock(t) + "|" + RafalNaredba.DockKontrola(t);
                }
                else if (u.IndexOf("RECALCGRIDROW(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "recalcred";
                else if (u.IndexOf("RECALCGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "recalc";
                else if (u.IndexOf("DISABLEALLCONTROLS(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "svekontrole";
                    k.Tekst = "0";
                }
                else if (u.IndexOf("ENABLEALLCONTROLS(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "svekontrole";
                    k.Tekst = "1";
                }
                else if (u.IndexOf("PROCESSSTART(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "proces";
                else if (u.IndexOf("SHOWFOLDERBROWSERDIALOG(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "mapa";
                else if (u.IndexOf("SHOWOPENFILEDIALOG(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "datotekadlg";
                else if (u.IndexOf("TIMERON(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "timer";
                else if (u.IndexOf("TIMEROFF(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "timeroff";
                else if (u.IndexOf("PROGRESSON(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "progres";
                    k.Tekst = "1";
                }
                else if (u.IndexOf("PROGRESSOFF(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "progres";
                    k.Tekst = "0";
                }
                else if (u.IndexOf("STARTSEARCH(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "trazi";
                else if (u.IndexOf("GRID_COLUMN_VALUE(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "stupac";
                else if (u.IndexOf("CREATEOBJECT(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "objekt";
                else if (u.IndexOf("TOGGLEGRIDGROUP(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "toggle";
                else if (u.IndexOf("PREPAREMULTISELECTEDPRINT(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "priprema";
                else if (u.IndexOf("PRINTREPXGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "ispisext";
                else if (u.IndexOf("PRINTGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "ispis";
                else if (u.IndexOf("OPENFORM(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "forma";
                    k.Tekst = OtvoriNaredba.Ime(t);
                }
                else if (u.IndexOf("BLAGAJNAENABLE(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "blagajna";
                else if (u.IndexOf("PIVOTGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "pivot";
                else if (u.IndexOf("CHARTGRID(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "chart";
                else if (u.IndexOf("COLORPICKER(", StringComparison.Ordinal) >= 0)
                    k.Vrsta = "boja";
                else if (PosaljiServer.Vrsta(t).Length > 0)
                    k.Vrsta = "posalji";
                else if (Pokretanje.Je(t))
                    k.Vrsta = "pokretanje";
                else if (u.IndexOf("EVAL(", StringComparison.Ordinal) >= 0)
                {
                    k.Vrsta = "eval";
                    k.Tekst = RafalNaredba.EvalLijevo(t) + "=" + RafalNaredba.EvalDesno(t);
                }
                else if (RafalNaredba.MasterSljedeci(t))
                    k.Vrsta = "sljedeci";
                else if (RafalNaredba.MasterPrethodni(t))
                    k.Vrsta = "prethodni";
                else if (RafalNaredba.Slopp(t) || u.IndexOf("CREATEOBJECT(", StringComparison.Ordinal) >= 0
                    || u.IndexOf("ISEMPTYORZERO", StringComparison.Ordinal) >= 0 || PraznoNaredba.Je(t))
                    k.Vrsta = "naredba";
                else if (u == "CLOSE" || u == "CANCEL")
                    k.Vrsta = "odustani";
                else if (u == "CLOSEPARENT")
                    k.Vrsta = "roditelj";
                else if (u == "OK")
                    k.Vrsta = "ok";
                else if (u == "OKSTAY")
                    k.Vrsta = "okstay";
                else if (Carpo.NetGen.Print.PrintBridge.JePredlozak(t))
                    k.Vrsta = "predlozak";
                else if (t.StartsWith(":", StringComparison.Ordinal))
                {
                    k.Vrsta = "forma";
                    k.Tekst = t.Substring(1).Trim();
                }
                else if (KlikNaredba.Kontrola(t).Length > 0)
                {
                    k.Vrsta = "klik";
                    k.Tekst = KlikNaredba.Kontrola(t);
                }
                else
                    k.Vrsta = "sql";
                lista.Add(k);
            }
            return lista;
        }

        public static bool PreskociGrid(NgControl k)
        {
            return Skip(k);
        }

        private static bool Skip(NgControl k)
        {
            if (k.Atributi == null || !k.Atributi.Ima("SKIPDEFAULTLOAD"))
                return false;
            return string.Equals((k.Atributi.Vrijednost("SKIPDEFAULTLOAD") ?? "").Trim(), "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
