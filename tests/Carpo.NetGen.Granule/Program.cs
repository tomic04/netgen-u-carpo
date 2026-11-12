using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Carpo.NetGen.Granule
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string root = null;
            string izvjestaj = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--izvjestaj" && i + 1 < args.Length)
                {
                    izvjestaj = args[++i];
                    continue;
                }
                if (root == null && !args[i].StartsWith("--", StringComparison.Ordinal))
                    root = args[i];
            }
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                Console.Error.WriteLine("Upotreba: Carpo.NetGen.Granule.exe <korijen-repa> --izvjestaj <datoteka>");
                return 2;
            }

            var sve = new List<Stavka>();
            sve.AddRange(OznakaKL.Provjeri(root));
            sve.AddRange(JeziciNG.Provjeri(root));
            sve.AddRange(MPPorKPNG.Provjeri(root));
            sve.AddRange(Intrastat.Provjeri(root));
            sve.AddRange(KntPdvObrasciEu.Provjeri(root));
            sve.AddRange(MpKntNacinPlac.Provjeri(root));
            sve.AddRange(KntOpzStat.Provjeri(root));
            sve.AddRange(MpRacNeFisk.Provjeri(root));
            sve.AddRange(MpFiskalPubVars.Provjeri(root));
            sve.AddRange(BilancaGfi.Provjeri(root));
            sve.AddRange(MpPdv.Provjeri(root));
            sve.AddRange(KntR2Racuni.Provjeri(root));
            sve.AddRange(TecRazOtv.Provjeri(root));
            sve.AddRange(RealizacijaStavke.Provjeri(root));
            sve.AddRange(MpKaseOper.Provjeri(root));
            sve.AddRange(KodRsPostavke.Provjeri(root));
            sve.AddRange(RealizacijaFakture.Provjeri(root));
            sve.AddRange(MpKnjigaPopisa.Provjeri(root));
            sve.AddRange(SkladisteNg.Provjeri(root));
            sve.AddRange(PorukeXml.Provjeri(root));
            sve.AddRange(VrCjenik.Provjeri(root));
            sve.AddRange(InstrumentiPlacanja.Provjeri(root));
            sve.AddRange(MatTem.Provjeri(root));
            sve.AddRange(MpPsNg.Provjeri(root));
            sve.AddRange(MpStanje.Provjeri(root));
            sve.AddRange(MpTrgOpr.Provjeri(root));
            sve.AddRange(MpKase.Provjeri(root));
            sve.AddRange(FaktureMinMax.Provjeri(root));
            sve.AddRange(KntKonfSheme.Provjeri(root));
            sve.AddRange(SklMinMax.Provjeri(root));
            sve.AddRange(PnObracun.Provjeri(root));
            sve.AddRange(KntBlaDok.Provjeri(root));
            sve.AddRange(Akcije.Provjeri(root));
            sve.AddRange(ManagerObaveza.Provjeri(root));
            sve.AddRange(PnKatalozi.Provjeri(root));
            sve.AddRange(PorukeDocXml.Provjeri(root));
            sve.AddRange(SmokePrazno.Provjeri(root));
            sve.AddRange(MpRealizNg.Provjeri(root));
            sve.AddRange(PartneriAudit.Provjeri(root));
            sve.AddRange(ZakljucivanjeTem.Provjeri(root));
            sve.AddRange(MpNivelacija.Provjeri(root));
            sve.AddRange(KntPreknjizavanje.Provjeri(root));
            sve.AddRange(KntBlaVrsteXml.Provjeri(root));
            sve.AddRange(MpTrgovineXml.Provjeri(root));
            sve.AddRange(HrOsobeUcinak.Provjeri(root));
            sve.AddRange(KupVrstPn.Provjeri(root));
            sve.AddRange(MpIzdVp.Provjeri(root));
            sve.AddRange(PnNalozi.Provjeri(root));
            sve.AddRange(KntShemaKonta.Provjeri(root));
            sve.AddRange(NedostajeXml.Provjeri(root));
            sve.AddRange(Statusi.Provjeri(root));
            sve.AddRange(KntBlaZak.Provjeri(root));
            sve.AddRange(PrijemniList.Provjeri(root));
            sve.AddRange(Kulture.Provjeri(root));
            sve.AddRange(KarticaPartnera.Provjeri(root));
            sve.AddRange(KupNg.Provjeri(root));
            sve.AddRange(KontrolePravila.Provjeri());
            sve.AddRange(KntDospjeleStavke.Provjeri(root));
            sve.AddRange(MPInventura.Provjeri(root));
            sve.AddRange(FakPar.Provjeri(root));
            sve.AddRange(MPNarIzdSkl.Provjeri(root));
            sve.AddRange(DobUgovori.Provjeri(root));
            sve.AddRange(MpNgPregDokMail.Provjeri(root));
            sve.AddRange(PozivPravila.Provjeri());
            sve.AddRange(KupCarpo.Provjeri(root));
            sve.AddRange(KupDostKn.Provjeri(root));
            sve.AddRange(KupDostBvg.Provjeri(root));
            sve.AddRange(MpBlagajnaIspis.Provjeri(root));
            sve.AddRange(KarticaIspis.Provjeri(root));
            sve.AddRange(JeziciIspis.Provjeri(root));
            sve.AddRange(RijetkeKontrole.Provjeri(root));
            sve.AddRange(PrioritetMeni.Provjeri(root));
            sve.AddRange(LVObracun.Provjeri(root));
            sve.AddRange(Joppd.Provjeri(root));
            sve.AddRange(KntKompVerizne.Provjeri(root));
            sve.AddRange(KntBlaPromet.Provjeri(root));
            sve.AddRange(MPPonuda.Provjeri(root));
            sve.AddRange(ServeriMeni.Provjeri(root));

            string tekst = Qa.Tekst(sve);
            if (!string.IsNullOrEmpty(PrioritetMeni.Tekst))
            {
                tekst += Environment.NewLine;
                tekst += PrioritetMeni.Tekst;
            }
            if (!string.IsNullOrEmpty(ServeriMeni.Tekst))
            {
                tekst += Environment.NewLine;
                tekst += ServeriMeni.Tekst;
            }
            Console.WriteLine(tekst);
            if (!string.IsNullOrWhiteSpace(izvjestaj))
            {
                string dir = Path.GetDirectoryName(izvjestaj);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(izvjestaj, tekst, new UTF8Encoding(false));
            }

            int pad = 0;
            for (int i = 0; i < sve.Count; i++)
            {
                if (sve[i].Ishod == Ishod.Pad)
                    pad++;
            }
            return pad == 0 ? 0 : 1;
        }
    }
}
