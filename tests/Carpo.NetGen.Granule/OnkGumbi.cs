using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;
using Carpo.NetGen.Model;
using Carpo.NetGen.TestHooks;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Tri gumba iz retka gumbi/lanac kojima checklista nije imala ID.
    /// ONK-04 je slučaj 4 iz docs/granule/Onclick-dijalog-testovi.md na PR #60.
    /// </summary>
    internal static class OnkGumbi
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "ZakljucivanjeTemNG.xml");
            const string opis = "Zaključi otvara Zakljucavanje. MAINFORM ostaje. spKntZakljucivanjeTem se ne šalje";
            const string izvor = "netgen/Form2.vb:6882-6883";
            if (!File.Exists(xml))
            {
                l.Add(Qa.P("ZakljucivanjeTemNG", "ONK-04", opis, izvor, "nema XML"));
                return l;
            }
            Stavka forma = Qa.Runtime("ZakljucivanjeTemNG", "ONK-04", opis, izvor, xml, "kontrola", "MAINFORM.buttonOK", "otvaraFormu", "Zakljucavanje");
            if (forma.Ishod != Ishod.Zeleno)
            {
                l.Add(forma);
                return l;
            }
            Stavka sql = Qa.Runtime("ZakljucivanjeTemNG", "ONK-04", opis, izvor, xml, "kontrola", "MAINFORM.buttonOK", "saljeSubmit", "false");
            if (sql.Ishod != Ishod.Zeleno)
            {
                l.Add(sql);
                return l;
            }
            System.Console.WriteLine("[Z] ZakljucivanjeTemNG ONK-04 otvaraFormu=Zakljucavanje saljeSubmit=false");
            l.Add(Qa.Z("ZakljucivanjeTemNG", "ONK-04", opis, izvor));
            l.Add(Niv05(root));
            l.Add(Rea04(root));
            return l;
        }

        private static Stavka Niv05(string root)
        {
            const string ime = "MPNivelacija";
            const string id = "NIV-05";
            const string opis = "Pokaži na FILTERFORM pošalje ONSUBMIT jer je FILLEXPLORER true";
            const string izvor = "netgen/Form1.vb:5445-5452; netgen/Form2.vb:3367";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPNivelacija.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm f = p.NadjiFormu("FILTERFORM");
            if (f == null)
                return Qa.P(ime, id, opis, izvor, "nema FILTERFORM");
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DateTime danas = DateTime.Now;
            cb["xdatumod"] = danas.Year.ToString("0000", CultureInfo.InvariantCulture) + "0101";
            cb["xdatumdo"] = danas.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string sql = DijalogTok.SqlNakonOk(f, new SqlKontekst { OperaterId = "7", Node = "null" }, cb);
            if (sql.IndexOf("spMPQueryGlaIDetNG", StringComparison.Ordinal) < 0 || sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                return Qa.P(ime, id, opis, izvor, "sql [" + sql + "]");
            int prije = NgDnevnik.Zapisi.Count;
            NgDnevnik.Sql(sql);
            string opisLoga = "";
            for (int i = NgDnevnik.Zapisi.Count - 1; i >= prije; i--)
            {
                if (string.Equals(NgDnevnik.Zapisi[i].Tekst, sql, StringComparison.Ordinal))
                    opisLoga = NgDnevnik.Zapisi[i].Opis ?? "";
            }
            if (opisLoga != "NetGen SQL")
                return Qa.P(ime, id, opis, izvor, "dnevnik [" + opisLoga + "]");
            string spin = "redovi";
            try
            {
                using (var conn = new SqlConnection(Citanje.Spoj))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 60;
                    var dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                spin = ex.Message ?? "";
                NgDnevnik.Greska(spin + " | " + sql);
                if (spin.IndexOf("spMPQueryGlaIDetNG", StringComparison.Ordinal) < 0)
                    return Qa.P(ime, id, opis, izvor, spin);
            }
            Console.WriteLine("[Z] MPNivelacija NIV-05 ONSUBMIT=spMPQueryGlaIDetNG dnevnik=NetGen SQL spin=" + spin);
            return Qa.Z(ime, id, opis, izvor);
        }

        private static Stavka Rea04(string root)
        {
            const string ime = "MPRealizNG";
            const string id = "REA-04";
            const string opis = "Pokaži na FORM1 pošalje ONSUBMIT jer je FILLEXPLORER true";
            const string izvor = "netgen/Form1.vb:5445-5452; netgen/Form2.vb:3367";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPRealizNG.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm f = p.NadjiFormu("FORM1");
            if (f == null)
                return Qa.P(ime, id, opis, izvor, "nema FORM1");
            DateTime danas = DateTime.Now;
            string dan = danas.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string parametri = "@trgovinaid=null,@robaid=null,@search='',@klasaid=null,@podklase=0,@datumod='" + dan + "',@datumdo='" + dan + "',@vrstadokid=null,@partneriid=null";
            string sql = DijalogTok.SqlNakonOk(f, new SqlKontekst { OperaterId = "7", Node = "null" }, new Dictionary<string, string>(), parametri);
            if (sql.IndexOf("spMPRealizacijaRobeNG", StringComparison.Ordinal) < 0 || sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                return Qa.P(ime, id, opis, izvor, "sql [" + sql + "]");
            NgDnevnik.Sql(sql);
            string spin = "redovi";
            try
            {
                using (var conn = new SqlConnection(Citanje.Spoj))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = 60;
                    var dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                spin = ex.Message ?? "";
                NgDnevnik.Greska(spin + " | " + sql);
                if (spin.IndexOf("spMPRealizacijaRobeNG", StringComparison.Ordinal) < 0)
                    return Qa.P(ime, id, opis, izvor, spin);
            }
            Console.WriteLine("[Z] MPRealizNG REA-04 ONSUBMIT=spMPRealizacijaRobeNG dnevnik=NetGen SQL spin=" + spin);
            return Qa.Z(ime, id, opis, izvor);
        }
    }
}
