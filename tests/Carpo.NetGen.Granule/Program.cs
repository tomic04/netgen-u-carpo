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

            string tekst = Qa.Tekst(sve);
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
