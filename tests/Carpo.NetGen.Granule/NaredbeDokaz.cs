using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Poziv;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Dokaz naredbi s PR #79 na stvarnom XML-u. Kod glavnog agenta se ne mijenja.
    /// </summary>
    internal static class NaredbeDokaz
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Click(root));
            l.Add(MsgBox(root));
            l.Add(Assembly(root));
            l.Add(Prostor(root));
            l.Add(Blagajna(root));
            l.Add(Roditelj(root));
            return l;
        }

        private static Stavka Click(string root)
        {
            const string granula = "KntBlaPromet";
            const string id = "NRD-CLICK";
            const string opis = "click(Ok) na ISPISDOK_FORM daje kontrolu OK";
            const string izvor = "netgen/Form2.vb:7151-7154";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntBlaPromet.xml");
            string tekst = Procitaj(xml);
            if (tekst == null || tekst.IndexOf("ONLOADED=\"click(Ok)\"", StringComparison.Ordinal) < 0)
                return Qa.P(granula, id, opis, izvor, "XML nema click(Ok)");
            string kontrola = KlikNaredba.Kontrola("click(Ok)");
            if (kontrola != "OK")
                return Qa.P(granula, id, opis, izvor, "kontrola [" + kontrola + "]");
            Console.WriteLine("[Z] KntBlaPromet NRD-CLICK click(Ok)=OK");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static Stavka MsgBox(string root)
        {
            const string granula = "KOD_Diskete";
            const string id = "NRD-MSGBOX";
            const string opis = "msgbox na izvozu diskete ide velikim slovima";
            const string izvor = "netgen/Form2.vb:7165-7169";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KOD_Diskete.xml");
            string tekst = Procitaj(xml);
            string naredba = Izmedu(tekst, "msgbox(", ")");
            if (naredba == null)
                return Qa.P(granula, id, opis, izvor, "XML nema msgbox(");
            string ocekivano = naredba.Trim().ToUpperInvariant().Replace("MSGBOX(", "").Replace(")", "").Trim();
            string dobiveno = PorukaNaredba.Tekst(naredba);
            if (dobiveno != ocekivano || dobiveno.Length == 0)
                return Qa.P(granula, id, opis, izvor, "tekst [" + dobiveno + "]");
            Console.WriteLine("[Z] KOD_Diskete NRD-MSGBOX " + dobiveno);
            return Qa.Z(granula, id, opis, izvor);
        }

        private static Stavka Assembly(string root)
        {
            const string granula = "MPOdjavaNarNG";
            const string id = "NRD-ASSEMBLY";
            const string opis = "Spremi učita ExportToExcelNG i prekine lanac ako sklopa nema";
            const string izvor = "netgen/Form2.vb:7424-7456";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPOdjavaNarNG.xml");
            string tekst = Procitaj(xml);
            string naredba = Izmedu(tekst, "ASSEMBLYLOAD(", ")");
            if (naredba == null)
                return Qa.P(granula, id, opis, izvor, "XML nema ASSEMBLYLOAD(");
            string tip = AssemblyPoziv.TipIz(naredba);
            string sklop = AssemblyPoziv.SklopIz(tip);
            if (tip != "ExportToExcelNG.clsExportToExcelNG" || sklop != "ExportToExcelNG")
                return Qa.P(granula, id, opis, izvor, "tip [" + tip + "] sklop [" + sklop + "]");
            string veza = AssemblyPoziv.Konekcija("dijalog", "Server=localhost;Database=spin");
            if (veza != "Driver={SQL Server};Server=localhost;Database=spin")
                return Qa.P(granula, id, opis, izvor, "veza [" + veza + "]");
            AssemblyPoziv.Ishod ishod = AssemblyPoziv.Pozovi(naredba, "dijalog", "7", "3", "Server=localhost", "zadnji", "grid", "@a=1");
            if (!ishod.Prekinuto || ishod.Pozvan || ishod.Argumenti[3] != "@a=1" || ishod.Argumenti[4] != "@a=1")
                return Qa.P(granula, id, opis, izvor, "prekinuto=" + ishod.Prekinuto + " pozvan=" + ishod.Pozvan);
            Console.WriteLine("[Z] MPOdjavaNarNG NRD-ASSEMBLY tip=ExportToExcelNG.clsExportToExcelNG sklop=ExportToExcelNG prekinuto=da");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static Stavka Prostor(string root)
        {
            const string granula = "MPTrgovinaPortal";
            const string id = "NRD-PROSTOR";
            const string opis = "ODJAVI zove poslovni prostor jednom i lanac ide do OK";
            const string izvor = "netgen/Form2.vb:7489-7493";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPTrgovinaPortal.xml");
            string tekst = Procitaj(xml);
            const string komad = "POSLOVNIPROSTOR(@@gridmasterTrgovinaId)#OK";
            if (tekst == null || tekst.IndexOf(komad, StringComparison.Ordinal) < 0)
                return Qa.P(granula, id, opis, izvor, "XML nema POSLOVNIPROSTOR");
            string naredba = "POSLOVNIPROSTOR(@@gridmasterTrgovinaId)";
            if (!ProstorNaredba.Je(naredba) || ProstorNaredba.Argument(naredba) != "@@GRIDMASTERTRGOVINAID")
                return Qa.P(granula, id, opis, izvor, "argument [" + ProstorNaredba.Argument(naredba) + "]");
            Lanac.Ishod ishod = Lanac.Izvrsi("update trgovina set aktivno = 0#" + komad, true, null);
            if (!ishod.Zatvara || ishod.Prekinuto)
                return Qa.P(granula, id, opis, izvor, "zatvara=" + ishod.Zatvara);
            Console.WriteLine("[Z] MPTrgovinaPortal NRD-PROSTOR argument=@@GRIDMASTERTRGOVINAID zatvara=da");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static Stavka Blagajna(string root)
        {
            const string granula = "MPKasaImport";
            const string id = "NRD-BLAGAJNA";
            const string opis = "Pokreni import šalje spmpBlagajnaEnable i pali ili gasi kasu";
            const string izvor = "netgen/Form2.vb:6868-6872; netgen/Class1.vb:1975-1986";
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPKasaImport.xml");
            string tekst = Procitaj(xml);
            if (tekst == null || tekst.IndexOf("BLAGAJNAENABLE(@@mpkaseid)#OK", StringComparison.Ordinal) < 0)
                return Qa.P(granula, id, opis, izvor, "XML nema BLAGAJNAENABLE");
            string kasa = BlagajnaNaredba.Id("BLAGAJNAENABLE(@@mpkaseid)");
            string upit = BlagajnaNaredba.Upit(kasa);
            string ugasi = BlagajnaNaredba.Update(kasa, false);
            string upali = BlagajnaNaredba.Update(kasa, true);
            if (kasa != "@@MPKASEID"
                || upit != "spmpBlagajnaEnable @mpkaseid = @@MPKASEID"
                || ugasi != "update mpkase set indenable = 0 where mpkaseid = @@MPKASEID"
                || upali != "update mpkase set indenable = 1 where mpkaseid = @@MPKASEID")
                return Qa.P(granula, id, opis, izvor, "upit [" + upit + "]");
            if (!Lanac.Izvrsi("BLAGAJNAENABLE(@@mpkaseid)#OK", true, null).Zatvara)
                return Qa.P(granula, id, opis, izvor, "OK iza naredbe nije zatvorio");
            Console.WriteLine("[Z] MPKasaImport NRD-BLAGAJNA upit=spmpBlagajnaEnable indenable=0/1");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static Stavka Roditelj(string root)
        {
            const string granula = "MPIzdVPNG";
            const string id = "NRD-RODITELJ";
            const string opis = "DA na ODUSTANI1 zatvara roditelja i ne guta SQL ispred";
            const string izvor = "netgen/Form2.vb:7035-7037";
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPIzdVPNG.xml");
            string tekst = Procitaj(xml);
            const string naredba = "spMPIzdatnicaSKLMP @action = 901, @operateriid = @@operateriid , @trgovinaid = @@trgovinaid, @id=@@id#CLOSEPARENT";
            if (tekst == null || tekst.IndexOf(naredba, StringComparison.Ordinal) < 0)
                return Qa.P(granula, id, opis, izvor, "XML nema CLOSEPARENT");
            if (!RoditeljNaredba.Je("CLOSEPARENT"))
                return Qa.P(granula, id, opis, izvor, "Je je false");
            Lanac.Ishod ishod = Lanac.Izvrsi(naredba, true, null);
            bool imaSql = false;
            for (int i = 0; i < ishod.Odigrano.Count; i++)
            {
                if ((ishod.Odigrano[i] ?? "").IndexOf("spMPIzdatnicaSKLMP", StringComparison.Ordinal) >= 0)
                    imaSql = true;
            }
            if (!ishod.CloseParent || !ishod.Button2 || !imaSql)
                return Qa.P(granula, id, opis, izvor, "closeParent=" + ishod.CloseParent + " sql=" + imaSql);
            Console.WriteLine("[Z] MPIzdVPNG NRD-RODITELJ closeParent=da sql=spMPIzdatnicaSKLMP");
            return Qa.Z(granula, id, opis, izvor);
        }

        private static string Izmedu(string tekst, string od, string doZnaka)
        {
            if (tekst == null)
                return null;
            int a = tekst.IndexOf(od, StringComparison.OrdinalIgnoreCase);
            if (a < 0)
                return null;
            int z = tekst.IndexOf(doZnaka, a + od.Length, StringComparison.Ordinal);
            if (z < 0)
                return null;
            return tekst.Substring(a, z - a + doZnaka.Length);
        }

        private static string Procitaj(string put)
        {
            if (!File.Exists(put))
                return null;
            byte[] raw = File.ReadAllBytes(put);
            if (raw.Length >= 2 && raw[0] == 0xFF && raw[1] == 0xFE)
                return System.Text.Encoding.Unicode.GetString(raw);
            return System.Text.Encoding.GetEncoding(1250).GetString(raw);
        }
    }
}
