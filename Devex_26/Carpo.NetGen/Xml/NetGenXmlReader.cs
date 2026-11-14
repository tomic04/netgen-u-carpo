using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Xml
{
    /// <summary>
    /// Ucitava NetGen PROJECT XML onako kako XmlDocument.Load cita BOM
    /// (ukljucujuci UTF-16). Fajl se ne prepisuje i atributi se ne preimenuju.
    /// </summary>
    public static class NetGenXmlReader
    {
        private static readonly HashSet<string> Strukturni = new HashSet<string>(StringComparer.Ordinal)
        {
            "PROJECT", "NAME", "CAPTION", "KOMENTAR", "HISTORY", "INPUT", "COLUMN",
            "MASTERCOLUMNS", "DETAILCOLUMNS", "DETAILCOLUMNS2", "DETAILCOLUMNS3", "DETAILCOLUMNS4",
            "ROW", "ROWS", "MENUITEM", "MENU", "INCLUDE"
        };

        public static bool JeNetGenProjekt(string putanja)
        {
            if (string.IsNullOrWhiteSpace(putanja) || !File.Exists(putanja))
                return false;
            try
            {
                var settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;
                settings.XmlResolver = null;
                using (var reader = XmlReader.Create(putanja, settings))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            return string.Equals(reader.LocalName, "PROJECT", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static NgLoadResult Ucitaj(string putanja)
        {
            try
            {
                var doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(putanja);
                return Parsiraj(doc, putanja);
            }
            catch (Exception ex)
            {
                return NgLoadResult.Neuspjelo(putanja, Poruka(putanja, ex.Message));
            }
        }

        /// <summary>Ista recenica kao Form1.vb 3264-3265 (MsgBox + Err.Description).</summary>
        public static string Poruka(string putanja, string opis)
        {
            return "XML LOAD ERROR : " + putanja + "\r\n\r\n" + (opis ?? "");
        }

        private static NgLoadResult Parsiraj(XmlDocument doc, string putanja)
        {
            var root = doc.DocumentElement;
            if (root == null || !string.Equals(root.LocalName, "PROJECT", StringComparison.OrdinalIgnoreCase))
            {
                string ime = root == null ? "(prazno)" : root.LocalName;
                return NgLoadResult.Neuspjelo(putanja, Poruka(putanja, "Korijen je '" + ime + "', ocekuje se PROJECT."));
            }

            var svi = new List<NgAttributeBag>();
            var elementi = new Dictionary<string, int>(StringComparer.Ordinal);
            var cvor = ProcitajCvor(root, null, svi, elementi);

            var forme = new List<NgForm>();
            var formePoCvoru = new Dictionary<NgCvor, NgForm>();
            var grupe = new List<NgColumnGroup>();

            for (int i = 0; i < cvor.Djeca.Count; i++)
            {
                var dijete = cvor.Djeca[i];
                if (JeGrupaKolona(dijete.Ime))
                    grupe.Add(ProcitajGrupu(dijete));
                else if (!Strukturni.Contains(dijete.Ime))
                {
                    var forma = ProcitajFormu(dijete);
                    forme.Add(forma);
                    formePoCvoru[dijete] = forma;
                }
            }

            var izvan = new List<NgControl>();
            RazmjestiKontrole(cvor, formePoCvoru, izvan);

            var atributi = cvor.Atributi;
            var preskoceni = new List<string>();
            var gumbi = new List<NgButton>();
            for (int n = 1; n <= 10; n++)
                DodajGumbSplit(atributi, n, gumbi, preskoceni);

            string naslov = NaslovIzCaptiona(cvor, atributi);
            var projekt = new NgProject(
                atributi,
                naslov,
                Efektivno(atributi, "STARTUPSTORA", "ONLOAD"),
                TrimAkoIma(atributi, "TREE"),
                TrimAkoIma(atributi, "DETAILGRID"),
                TrimAkoIma(atributi, "BUTTONS"),
                Efektivno(atributi, "POPUP", "POPUPMASTER"),
                Efektivno(atributi, "DBLCLICKFORM", "MASTERDBLCLICK"),
                TrimAkoIma(atributi, "PERMISIO"),
                TrimAkoIma(atributi, "INCLUDE"),
                forme,
                grupe,
                izvan,
                gumbi,
                preskoceni,
                svi,
                elementi);

            return NgLoadResult.URedu(putanja, projekt);
        }

        private static void RazmjestiKontrole(NgCvor cvor, Dictionary<NgCvor, NgForm> forme, List<NgControl> izvan)
        {
            if (cvor.Ime == "INPUT")
            {
                NgForm forma = NadjiFormu(cvor, forme);
                bool izravno = forma != null && cvor.Roditelj != null && forme.ContainsKey(cvor.Roditelj);
                var kontrola = NapraviKontrolu(cvor, izravno);
                if (forma != null)
                    forma.Kontrole.Add(kontrola);
                else
                    izvan.Add(kontrola);
            }
            for (int i = 0; i < cvor.Djeca.Count; i++)
                RazmjestiKontrole(cvor.Djeca[i], forme, izvan);
        }

        private static NgForm NadjiFormu(NgCvor cvor, Dictionary<NgCvor, NgForm> forme)
        {
            var p = cvor.Roditelj;
            while (p != null)
            {
                NgForm f;
                if (forme.TryGetValue(p, out f))
                    return f;
                p = p.Roditelj;
            }
            return null;
        }

        private static NgControl NapraviKontrolu(NgCvor cvor, bool izravno)
        {
            string typeRaw = cvor.Atributi.Ima("TYPE") ? cvor.Atributi.Vrijednost("TYPE") : null;
            string name = cvor.Atributi.Ima("NAME") ? (cvor.Atributi.Vrijednost("NAME") ?? "").Trim() : null;
            var k = new NgControl(typeRaw, NgType.Odredi(typeRaw), name, cvor.Atributi, izravno);
            for (int i = 0; i < cvor.Djeca.Count; i++)
            {
                if (cvor.Djeca[i].Ime == "COLUMN")
                    k.Kolone.Add(Kolona(cvor.Djeca[i].Atributi));
            }
            return k;
        }

        private static NgColumn Kolona(NgAttributeBag a)
        {
            string jednoG = a.Ima("AGREGATEFUNCTION") ? (a.Vrijednost("AGREGATEFUNCTION") ?? "").Trim() : null;
            string name = a.Ima("NAME") ? (a.Vrijednost("NAME") ?? "").Trim() : null;
            return new NgColumn(name, jednoG, a.Ima("AGGREGATEFUNCTION"), a);
        }

        private static NgForm ProcitajFormu(NgCvor cvor)
        {
            var gumbi = new List<NgButton>();
            var preskoceni = new List<string>();
            // Form2: CMD1 i CMD2 su cijeli tekst, CMD3-CMD6 se lome na dvotocki.
            DodajGumbCio(cvor.Atributi, 1, gumbi);
            DodajGumbCio(cvor.Atributi, 2, gumbi);
            for (int n = 3; n <= 6; n++)
                DodajGumbSplit(cvor.Atributi, n, gumbi, preskoceni);

            return new NgForm(
                cvor.Ime,
                cvor.Atributi,
                Efektivno(cvor.Atributi, "STORA", "ONSUBMIT"),
                Efektivno(cvor.Atributi, "STARTUPSTORA", "ONLOAD"),
                new List<NgControl>(),
                gumbi,
                preskoceni);
        }

        private static NgColumnGroup ProcitajGrupu(NgCvor cvor)
        {
            var kolone = new List<NgColumn>();
            for (int i = 0; i < cvor.Djeca.Count; i++)
            {
                if (cvor.Djeca[i].Ime != "COLUMN")
                    continue;
                kolone.Add(Kolona(cvor.Djeca[i].Atributi));
            }
            return new NgColumnGroup(
                cvor.Ime,
                TrimAkoIma(cvor.Atributi, "CLEARCOLUMS"),
                TrimAkoIma(cvor.Atributi, "HEADERALIGNMENT"),
                cvor.Atributi,
                kolone);
        }

        private static bool JeGrupaKolona(string ime)
        {
            return ime == "MASTERCOLUMNS"
                || ime == "DETAILCOLUMNS"
                || ime == "DETAILCOLUMNS2"
                || ime == "DETAILCOLUMNS3"
                || ime == "DETAILCOLUMNS4";
        }

        private static string NaslovIzCaptiona(NgCvor projekt, NgAttributeBag atributi)
        {
            NgCvor caption = null;
            for (int i = 0; i < projekt.Djeca.Count; i++)
            {
                if (projekt.Djeca[i].Ime == "CAPTION")
                {
                    caption = projekt.Djeca[i];
                    break;
                }
            }
            if (caption == null || caption.TekstPrvogDjeteta == null)
                return null;

            string verzija = TrimAkoIma(atributi, "VERSION") ?? "";
            if (verzija.Trim().Length > 0)
                return caption.TekstPrvogDjeteta + "  v[" + verzija.Trim() + "]";
            return caption.TekstPrvogDjeteta;
        }

        /// <summary>
        /// Nedostajuci atribut ne brise prethodnu vrijednost (On Error Resume Next).
        /// Prisutan atribut, cak i prazan, prepisuje.
        /// </summary>
        private static string Efektivno(NgAttributeBag a, string prvo, string prepis)
        {
            string v = "";
            if (a.Ima(prvo))
                v = (a.Vrijednost(prvo) ?? "").Trim();
            if (a.Ima(prepis))
                v = (a.Vrijednost(prepis) ?? "").Trim();
            return v;
        }

        private static string TrimAkoIma(NgAttributeBag a, string ime)
        {
            if (!a.Ima(ime))
                return null;
            return (a.Vrijednost(ime) ?? "").Trim();
        }

        private static void DodajGumbCio(NgAttributeBag a, int n, List<NgButton> gumbi)
        {
            string ime = "CMD" + n.ToString() + "CAPTION";
            if (!a.Ima(ime))
                return;
            string v = (a.Vrijednost(ime) ?? "").Trim();
            if (v.Length == 0)
                return;
            gumbi.Add(new NgButton(n, v, ""));
        }

        private static void DodajGumbSplit(NgAttributeBag a, int n, List<NgButton> gumbi, List<string> preskoceni)
        {
            string ime = "CMD" + n.ToString() + "CAPTION";
            if (!a.Ima(ime))
                return;
            string v = (a.Vrijednost(ime) ?? "").Trim();
            if (v.Length == 0)
                return;
            int i = v.IndexOf(':');
            if (i < 0)
            {
                preskoceni.Add(ime);
                return;
            }
            gumbi.Add(new NgButton(n, v.Substring(0, i), v.Substring(i + 1)));
        }

        private static NgCvor ProcitajCvor(XmlElement el, NgCvor roditelj, List<NgAttributeBag> svi, Dictionary<string, int> elementi)
        {
            int n;
            elementi.TryGetValue(el.LocalName, out n);
            elementi[el.LocalName] = n + 1;

            var bag = ProcitajAtribute(el);
            svi.Add(bag);
            var cvor = new NgCvor();
            cvor.Ime = el.LocalName;
            cvor.Atributi = bag;
            cvor.Roditelj = roditelj;
            cvor.TekstPrvogDjeteta = null;
            if (el.ChildNodes.Count > 0)
                cvor.TekstPrvogDjeteta = el.ChildNodes[0].InnerText;

            for (int i = 0; i < el.ChildNodes.Count; i++)
            {
                var dijete = el.ChildNodes[i] as XmlElement;
                if (dijete == null)
                    continue;
                cvor.Djeca.Add(ProcitajCvor(dijete, cvor, svi, elementi));
            }
            return cvor;
        }

        private static NgAttributeBag ProcitajAtribute(XmlElement el)
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            if (el.Attributes != null)
            {
                foreach (XmlAttribute a in el.Attributes)
                {
                    if (!map.ContainsKey(a.Name))
                        map.Add(a.Name, a.Value ?? "");
                }
            }
            return new NgAttributeBag(map);
        }

        private sealed class NgCvor
        {
            public string Ime;
            public NgAttributeBag Atributi;
            public NgCvor Roditelj;
            public string TekstPrvogDjeteta;
            public readonly List<NgCvor> Djeca = new List<NgCvor>();
        }
    }
}
