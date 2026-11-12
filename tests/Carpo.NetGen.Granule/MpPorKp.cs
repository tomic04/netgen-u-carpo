using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Log;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// MPK-43: SQL filtera (ONSUBMIT FILTERFORM) ide u NgDnevnik kao NetGen SQL, ne u CBLog.
    /// Class1.vb 138, docs/funkcionalnosti/logging.md.
    /// </summary>
    internal static class MpPorKp
    {
        public const string Ime = "MPPorKPNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPPorKPNG.xml");
            const string opis = "SQL filtera ide u NgDnevnik, ne u CBLog";
            const string izvor = "netgen/Class1.vb:138; docs/funkcionalnosti/logging.md";
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "MPK-43", opis, izvor, "nema XML"));
                return l;
            }
            if (GranulaSesija.Izvrsi == null)
            {
                l.Add(Qa.P(Ime, "MPK-43", opis, izvor, "kuka nije spojena"));
                return l;
            }
            int prije = NgDnevnik.Zapisi.Count;
            Pogled p = GranulaSesija.Izvrsi(new Zahtjev
            {
                XmlPutanja = xml,
                Akcija = "gumb",
                Cilj = "Parametri",
                MasterId = "0",
                Vrijednosti = ""
            });
            string sql = "";
            string opisLoga = "";
            string razina = "";
            bool cb = false;
            for (int i = prije; i < NgDnevnik.Zapisi.Count; i++)
            {
                NgZapis z = NgDnevnik.Zapisi[i];
                if (z == null)
                    continue;
                if ((z.Opis ?? "").IndexOf("CBLog", StringComparison.OrdinalIgnoreCase) >= 0
                    || (z.Tekst ?? "").IndexOf("CBLog", StringComparison.OrdinalIgnoreCase) >= 0)
                    cb = true;
                if ((z.Tekst ?? "").IndexOf("spMPPoravnajKP", StringComparison.Ordinal) >= 0)
                {
                    sql = z.Tekst ?? "";
                    opisLoga = z.Opis ?? "";
                    razina = z.Razina ?? "";
                }
            }
            bool ok = !cb
                && opisLoga == "NetGen SQL"
                && razina == "InfoData"
                && sql.IndexOf("spMPPoravnajKP", StringComparison.Ordinal) >= 0
                && sql.IndexOf("@@", StringComparison.Ordinal) < 0
                && Kljuc(p, "dnevnik") == "NetGen SQL";
            if (ok)
            {
                Console.WriteLine("[Z] MPPorKPNG MPK-43 dnevnik=NetGen SQL sql=spMPPoravnajKP");
                l.Add(Qa.Z(Ime, "MPK-43", opis, izvor));
            }
            else
            {
                l.Add(Qa.P(Ime, "MPK-43", opis, izvor,
                    "opis=[" + opisLoga + "] razina=[" + razina + "] cb=" + cb + " sql=[" + sql + "] dnevnik=[" + Kljuc(p, "dnevnik") + "] " + (p == null ? "null" : (p.Napomena ?? ""))));
            }
            return l;
        }

        private static string Kljuc(Pogled p, string kljuc)
        {
            if (p == null || !p.Izvedeno || p.Stavke == null || !p.Stavke.ContainsKey(kljuc))
                return "";
            return p.Stavke[kljuc] ?? "";
        }
    }
}
