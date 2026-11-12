using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Carpo.NetGen;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// STA-12, PL-32, MPK-30, MPK-31, STA-34, MPP-20.
    /// Očekivanje je iz Form1.vb. Test se ne popušta da prođe.
    /// </summary>
    public static class Cetvrti
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Sta12(root));
            l.Add(Mpk(root, "MPK-30", "FILTERFORM", "true", true,
                "FILTERFORM true zamijeni master SQL-om ONSUBMIT i parametara",
                "netgen/Form1.vb:5445-5452", "Parametri"));
            l.Add(Mpk(root, "MPK-31", "DODAJIZNOS", "prazan", false,
                "Prazan FILLEXPLORER izvrsi ONSUBMIT pa ponovi zadnji master",
                "netgen/Form1.vb:5509-5519", "Dodaj"));
            l.Add(Sta34(root));
            l.Add(Pl32(root));
            l.Add(Mpp20(root));
            return l;
        }

        private static Stavka Sta12(string root)
        {
            const string ime = "Statusi";
            const string id = "STA-12";
            const string opis = "ssMeni.Parametar otvara Statusi kroz Loader, Parent prolazi fk_meni";
            const string izvor = "netgen/Form1.vb:1139,1251-1258";
            const string param = "asm:NetGenDLL.loader:Statusi.xml";
            try
            {
                string ugasen = Citanje.PrviTekst(
                    "select cast(fk.is_disabled as varchar(1)) from sys.foreign_keys fk join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id where fk.parent_object_id = object_id(N'dbo.ssMeni') and fkc.referenced_object_id = object_id(N'dbo.ssMeni')",
                    out string g1);
                if (ugasen == null)
                    return Qa.P(ime, id, opis, izvor, g1 ?? "fk");
                if (ugasen != "0")
                    return Qa.P(ime, id, opis, izvor, "fk_meni je ugasen");
                DataTable red = Tablica("select Parametar, Parent from ssMeni where Parametar = N'" + param + "'");
                if (red.Rows.Count != 1)
                    return Qa.P(ime, id, opis, izvor, "redova=" + red.Rows.Count);
                if (red.Rows[0]["Parent"] == DBNull.Value)
                    return Qa.P(ime, id, opis, izvor, "Parent je null");
                string parent = Convert.ToString(red.Rows[0]["Parent"]);
                string postoji = Citanje.PrviTekst(
                    "select cast(count(*) as varchar(10)) from ssMeni where MeniID = " + parent,
                    out string g2);
                if (postoji != "1")
                    return Qa.P(ime, id, opis, izvor, "Parent " + parent + " nije MeniID. " + (g2 ?? ""));
                string module = MeniParametar.ModuleId(Convert.ToString(red.Rows[0]["Parametar"]));
                if (module != "Statusi.xml")
                    return Qa.P(ime, id, opis, izvor, "moduleId [" + module + "]");
                string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "Statusi.xml");
                string daljina = Path.Combine(root, "artifacts", "spin", "meni-xml");
                string cilj = Path.Combine(daljina, "SharedDLLs", "xml", module);
                Directory.CreateDirectory(Path.GetDirectoryName(cilj));
                File.Copy(xml, cilj, true);
                var loader = new Loader();
                loader.moduleId = module;
                loader.TestXmlPath = null;
                loader.Run8("localhost", "spin", "", "", 7, 0, 0, 0, daljina);
                if (loader.Projekt == null)
                    return Qa.P(ime, id, opis, izvor, loader.ZadnjaGreska ?? "Loader nije ucitao XML");
                if ((loader.Projekt.Tree ?? "").IndexOf("POPUNI_FORM", StringComparison.Ordinal) < 0)
                    return Qa.P(ime, id, opis, izvor, loader.Projekt.Tree ?? "");
                Console.WriteLine("[Z] Cetvrti.STA-12 Loader.Run8 moduleId=" + module + " Parent=" + parent);
                return Qa.Z(ime, id, "Loader.Run8 " + module + " Parent=" + parent, izvor);
            }
            catch (Exception ex)
            {
                return Qa.P(ime, id, opis, izvor, ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static Stavka Mpk(string root, string id, string forma, string klasa, bool zamjenjuje, string opis, string izvor, string gumb)
        {
            const string ime = "MPPorKPNG";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPPorKPNG.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm f = p.NadjiFormu(forma);
            if (f == null)
                return Qa.P(ime, id, opis, izvor, "nema forme " + forma);
            if (FillExplorer.Klasa(f) != klasa)
                return Qa.P(ime, id, opis, izvor, "klasa [" + FillExplorer.Klasa(f) + "]");
            if (FillExplorer.ZamjenjujeMaster(klasa) != zamjenjuje || FillExplorer.PonavljaZadnji(klasa) == zamjenjuje)
                return Qa.P(ime, id, opis, izvor, "grana mastera nije " + klasa);
            if (zamjenjuje && FillExplorer.NoviMaster(f, "").IndexOf("spMPPoravnajKP", StringComparison.Ordinal) < 0)
                return Qa.P(ime, id, opis, izvor, FillExplorer.NoviMaster(f, ""));
            Stavka s = Qa.Kljuc(ime, id, opis, izvor, xml, "gumb", gumb, "fillExplorer", klasa);
            if (s.Ishod == Ishod.Otvoreno)
                return Qa.P(ime, id, opis, izvor, "kuka nije vratila fillExplorer. " + (s.Detalj ?? ""));
            return s;
        }

        private static Stavka Sta34(string root)
        {
            const string ime = "Statusi";
            const string id = "STA-34";
            const string opis = "Forma bez kontrola ne otvara dijalog; FILLEXPLORER zamijeni master ONSUBMIT-om";
            const string izvor = "netgen/Form1.vb:5369-5370,5445-5452";
            const string ocek = "spStatusiQuery @action=0, @node=@@node";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "Statusi.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            NgForm pop = p.NadjiFormu("POPUNI_FORM");
            if (pop == null)
                return Qa.P(ime, id, opis, izvor, "nema POPUNI_FORM");
            if (FillExplorer.OtvaraDijalog(pop))
                return Qa.P(ime, id, opis, izvor, "kontrola=" + pop.Kontrole.Count);
            if (FillExplorer.Klasa(pop) != "true")
                return Qa.P(ime, id, opis, izvor, "klasa " + FillExplorer.Klasa(pop));
            if (FillExplorer.CvorSql(p) != ocek)
                return Qa.P(ime, id, opis, izvor, "cvorSql [" + FillExplorer.CvorSql(p) + "]");
            return Qa.Mora(ime, id, opis, izvor, xml, "otvori", "", "cvorSql", ocek);
        }

        private static Stavka Pl32(string root)
        {
            const string ime = "PrijemniList";
            const string id = "PL-32";
            const string opis = "Vrijednost combo stupca 0 mora biti TREE tekst (segmenti s # i :)";
            const string izvor = "netgen/Form1.vb:3342-3344,4344";
            try
            {
                if (ComboStablo.IndeksVrijednosti != 0 || ComboStablo.IndeksPrikaza != 1)
                    return Qa.P(ime, id, opis, izvor, "ValueMember nije stupac 0");
                DataTable combo = Tablica("exec spNg_combo @action='Drvo_NabPri'");
                if (combo.Rows.Count == 0 || combo.Columns.Count < 2)
                    return Qa.P(ime, id, opis, izvor, "spNg_combo nije vratio stupac 0 i 1");
                string vrijednost = Convert.ToString(combo.Rows[0][ComboStablo.IndeksVrijednosti]);
                string natpis = Convert.ToString(combo.Rows[0][ComboStablo.IndeksPrikaza]);
                if (ComboStablo.Klasa(vrijednost) != "tree")
                    return Qa.P(ime, id, opis, izvor, "stupac 0 [" + vrijednost + "]");
                if (string.Equals(vrijednost, natpis, StringComparison.Ordinal))
                    return Qa.P(ime, id, opis, izvor, "stupac 0 je natpis, ne TREE");
                var stablo = new Carpo.NetGen.Grid.Stablo();
                stablo.Kontekst.OperaterId = "7";
                stablo.IzTeksta(vrijednost, null, delegate (string sql) { return Tablica(sql); });
                bool danas = false;
                bool skladiste = false;
                for (int i = 0; i < stablo.Stanje.Korijen.Count; i++)
                {
                    Cvor c = stablo.Stanje.Korijen[i];
                    if (c.Natpis == "Danas" && c.Naredba == "Promet")
                        danas = true;
                    if (c.Natpis == "Sva skladista" && c.Naredba == "Skladiste" && c.Djeca.Count == 1)
                        skladiste = true;
                }
                if (!danas || !skladiste)
                    return Qa.P(ime, id, opis, izvor, "stablo nije rasclanilo " + vrijednost);
                Console.WriteLine("[Z] Cetvrti.Pl32 IzTeksta " + vrijednost);
                return Qa.Z(ime, id, "IzTeksta " + vrijednost, izvor);
            }
            catch (Exception ex)
            {
                return Qa.P(ime, id, opis, izvor, ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static Stavka Mpp20(string root)
        {
            const string ime = "MPPonuda";
            const string id = "MPP-20";
            const string opis = "enable/disable trazi Button.Text, ne NAME";
            const string izvor = "netgen/Form1.vb:3914-3921";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPPonuda.xml");
            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
                return Qa.P(ime, id, opis, izvor, greska ?? "XML");
            var gumbi = GumbNatpis.Efektivni(p);
            if (GumbNatpis.PoStupcu(gumbi, "INSERTFORM1") != "")
                return Qa.P(ime, id, opis, izvor, "stupac INSERTFORM1 je naredba, ne Button.Text");
            if (GumbNatpis.PoStupcu(gumbi, "Dodaj") != "Dodaj")
                return Qa.P(ime, id, opis, izvor, "Button.Text Dodaj nije nađen");
            try
            {
                string set = p.Atributi.Vrijednost("SETDEFAULTS") ?? "";
                string poziv = "";
                string[] dijelovi = set.Split('#');
                for (int i = 0; i < dijelovi.Length; i++)
                {
                    if ((dijelovi[i] ?? "").IndexOf("spmpPermissionNG", StringComparison.OrdinalIgnoreCase) >= 0)
                        poziv = dijelovi[i].Trim();
                }
                if (poziv.Length == 0)
                    return Qa.P(ime, id, opis, izvor, "nema spmpPermissionNG");
                var k = new SqlKontekst { OperaterId = "7", XmlName = "MPPonuda.xml", Node = "null" };
                poziv = SqlTekst.Sredi(poziv, k);
                DataTable dozvola = Tablica(poziv);
                if (dozvola.Rows.Count != 1 || !dozvola.Columns.Contains("dodaj"))
                    return Qa.P(ime, id, opis, izvor, "spmpPermissionNG nije vratio dodaj");
                if (dozvola.Columns.Contains("INSERTFORM1"))
                    return Qa.P(ime, id, opis, izvor, "procedura vraca naredbu umjesto natpisa");
                int dodaj = Convert.ToInt32(dozvola.Rows[0]["dodaj"]);
                string ocek = dodaj == 1 ? "enable" : "disable";
                string on2 = p.Atributi.Vrijednost("ONLOAD2") ?? "";
                string sql = Dozvola.Poslije(SqlTekst.Sredi(on2, k), Dozvola.IzReda(dozvola.Rows[0]));
                if (sql.IndexOf("@@dodaj", StringComparison.OrdinalIgnoreCase) >= 0)
                    return Qa.P(ime, id, opis, izvor, "@@dodaj nije zamijenjen prije ExecSql");
                if (sql.IndexOf(dodaj.ToString(System.Globalization.CultureInfo.InvariantCulture) + "=1", StringComparison.Ordinal) < 0)
                    return Qa.P(ime, id, opis, izvor, sql);
                DataTable stanje = Tablica(sql);
                string dobiveno = Convert.ToString(stanje.Rows[0]["Dodaj"]).Trim().ToLowerInvariant();
                if (dobiveno != ocek)
                    return Qa.P(ime, id, opis, izvor, "Dodaj ocekivano [" + ocek + "] dobiveno [" + dobiveno + "]");
                if (GranulaSesija.Izvrsi == null)
                    return Qa.P(ime, id, opis, izvor, "Kuka nije spojena");
                Pogled pogled = GranulaSesija.Izvrsi(new Zahtjev { XmlPutanja = xml, Akcija = "otvori", Cilj = "", MasterId = "0", Vrijednosti = "" });
                if (pogled == null || !pogled.Izvedeno || pogled.Stavke == null || !pogled.Stavke.ContainsKey("gumb.stanje"))
                    return Qa.P(ime, id, opis, izvor, "kuka nema gumb.stanje");
                if (pogled.Stavke["gumb.stanje"] != ocek || pogled.Stavke["gumb.natpis"] != "Dodaj")
                    return Qa.P(ime, id, opis, izvor, "kuka [" + pogled.Stavke["gumb.natpis"] + " " + pogled.Stavke["gumb.stanje"] + "]");
                Console.WriteLine("[Z] Cetvrti.Mpp20 @@dodaj=" + dodaj.ToString(System.Globalization.CultureInfo.InvariantCulture) + " Button.Text=Dodaj stanje=" + dobiveno);
                return Qa.Z(ime, id, "@@dodaj Button.Text Dodaj=" + ocek, izvor);
            }
            catch (Exception ex)
            {
                return Qa.P(ime, id, opis, izvor, ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static DataTable Tablica(string sql)
        {
            string t = (sql ?? "").TrimStart();
            string rijec = "";
            int i = 0;
            while (i < t.Length && char.IsLetter(t[i]))
            {
                rijec += char.ToLowerInvariant(t[i]);
                i++;
            }
            if (rijec == "insert" || rijec == "update" || rijec == "delete" || rijec == "drop"
                || rijec == "alter" || rijec == "truncate" || rijec == "merge" || rijec == "create")
                throw new InvalidOperationException("nije citanje: " + rijec);
            var dt = new DataTable();
            using (var conn = new SqlConnection(Citanje.Spoj))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = 30;
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }
    }
}
