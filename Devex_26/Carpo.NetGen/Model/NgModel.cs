using System.Collections.Generic;
using Carpo.NetGen.Shell;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Model
{
    /// <summary>
    /// Atributi kako su napisani. Ime se ne preimenuje (HIGHT ostaje HIGHT).
    /// </summary>
    public sealed class NgAttributeBag
    {
        private readonly Dictionary<string, string> _map;

        public NgAttributeBag(Dictionary<string, string> map)
        {
            _map = map ?? new Dictionary<string, string>();
        }

        public int Broj
        {
            get { return _map.Count; }
        }

        public bool Ima(string ime)
        {
            return ime != null && _map.ContainsKey(ime);
        }

        public string Vrijednost(string ime)
        {
            if (ime == null)
                return null;
            string v;
            if (_map.TryGetValue(ime, out v))
                return v;
            return null;
        }

        public IEnumerable<KeyValuePair<string, string>> Stavke
        {
            get { return _map; }
        }
    }

    public sealed class NgButton
    {
        public NgButton(int redni, string natpis, string naredba)
        {
            Redni = redni;
            Natpis = natpis ?? "";
            Naredba = naredba ?? "";
        }

        public int Redni { get; private set; }
        public string Natpis { get; private set; }
        public string Naredba { get; private set; }
    }

    public sealed class NgControl
    {
        public NgControl(string typeRaw, NgGrana grana, string name, NgAttributeBag atributi, bool izravnoDijeteForme)
        {
            TypeRaw = typeRaw;
            Grana = grana;
            Name = name;
            Atributi = atributi;
            IzravnoDijeteForme = izravnoDijeteForme;
            Kolone = new List<NgColumn>();
        }

        /// <summary>TYPE kako je napisan, ukljucujuci tipfelere. Null ako atributa nema.</summary>
        public string TypeRaw { get; private set; }
        public NgGrana Grana { get; private set; }
        public string Name { get; private set; }
        public NgAttributeBag Atributi { get; private set; }
        public bool IzravnoDijeteForme { get; private set; }

        /// <summary>COLUMN djeca grida dijaloga (Form2.vb 1804-1957), redom kako su napisana. DGR-01.</summary>
        public List<NgColumn> Kolone { get; private set; }
    }

    public sealed class NgColumn
    {
        public NgColumn(string name, string agregateFunction, bool imaDvostrukoG, NgAttributeBag atributi)
        {
            Name = name;
            AgregateFunction = agregateFunction;
            ImaDvostrukoG = imaDvostrukoG;
            Atributi = atributi;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Vezano samo ako postoji AGREGATEFUNCTION (jedno G), kao Grid.vb.
        /// AGGREGATEFUNCTION (dva G) ovo polje ne puni.
        /// </summary>
        public string AgregateFunction { get; private set; }

        public bool ImaDvostrukoG { get; private set; }
        public NgAttributeBag Atributi { get; private set; }
    }

    public sealed class NgColumnGroup
    {
        public NgColumnGroup(string ime, string clearColums, string headerAlignment, NgAttributeBag atributi, List<NgColumn> kolone)
        {
            Ime = ime;
            ClearColums = clearColums;
            HeaderAlignment = headerAlignment;
            Atributi = atributi;
            Kolone = kolone ?? new List<NgColumn>();
        }

        public string Ime { get; private set; }

        /// <summary>CLEARCOLUMS (nema N). Null ako atributa nema. CLEARCOLUMNS se ne cita.</summary>
        public string ClearColums { get; private set; }

        public string HeaderAlignment { get; private set; }
        public NgAttributeBag Atributi { get; private set; }
        public List<NgColumn> Kolone { get; private set; }
    }

    public sealed class NgForm
    {
        public NgForm(string ime, NgAttributeBag atributi, string onSubmit, string onLoad, List<NgControl> kontrole, List<NgButton> gumbi, List<string> preskoceniGumbi)
        {
            Ime = ime;
            Atributi = atributi;
            OnSubmit = onSubmit ?? "";
            OnLoad = onLoad ?? "";
            Kontrole = kontrole ?? new List<NgControl>();
            Gumbi = gumbi ?? new List<NgButton>();
            PreskoceniGumbi = preskoceniGumbi ?? new List<string>();
        }

        public string Ime { get; private set; }
        public NgAttributeBag Atributi { get; private set; }

        /// <summary>STORA, pa ONSUBMIT ako atribut postoji (Form2.vb 1036-1039).</summary>
        public string OnSubmit { get; private set; }

        /// <summary>STARTUPSTORA, pa ONLOAD ako atribut postoji.</summary>
        public string OnLoad { get; private set; }

        public List<NgControl> Kontrole { get; private set; }
        public List<NgButton> Gumbi { get; private set; }
        public List<string> PreskoceniGumbi { get; private set; }
    }

    public sealed class NgProject
    {
        private readonly List<NgAttributeBag> _sviAtributi;
        private readonly Dictionary<string, int> _elementi;

        public NgProject(
            NgAttributeBag atributi,
            string naslov,
            string onLoad,
            string tree,
            string detailGrid,
            string buttons,
            string popup,
            string masterDblClick,
            string permisio,
            string include,
            List<NgForm> forme,
            List<NgColumnGroup> grupeKolona,
            List<NgControl> kontroleIzvanForme,
            List<NgButton> gumbi,
            List<string> preskoceniGumbi,
            List<NgAttributeBag> sviAtributi,
            Dictionary<string, int> elementi)
        {
            Atributi = atributi;
            Naslov = naslov;
            OnLoad = onLoad ?? "";
            Tree = tree;
            DetailGrid = detailGrid;
            Buttons = buttons;
            Popup = popup ?? "";
            MasterDblClick = masterDblClick ?? "";
            Permisio = permisio;
            Include = include;
            Forme = forme ?? new List<NgForm>();
            GrupeKolona = grupeKolona ?? new List<NgColumnGroup>();
            KontroleIzvanForme = kontroleIzvanForme ?? new List<NgControl>();
            Gumbi = gumbi ?? new List<NgButton>();
            PreskoceniGumbi = preskoceniGumbi ?? new List<string>();
            _sviAtributi = sviAtributi ?? new List<NgAttributeBag>();
            _elementi = elementi ?? new Dictionary<string, int>();
            Raspored = ShellLayout.Iz(this);
        }

        public NgAttributeBag Atributi { get; private set; }

        /// <summary>Naslov prozora kao Form1.vb 3522-3528. Null ako nema elementa CAPTION.</summary>
        public string Naslov { get; private set; }

        public string OnLoad { get; private set; }
        public string Tree { get; private set; }
        public string DetailGrid { get; private set; }
        public string Buttons { get; private set; }
        public string Popup { get; private set; }
        public string MasterDblClick { get; private set; }

        /// <summary>PERMISIO (jedno S). PERMISSIO se ne cita.</summary>
        public string Permisio { get; private set; }

        public string Include { get; private set; }
        public List<NgForm> Forme { get; private set; }
        public List<NgColumnGroup> GrupeKolona { get; private set; }
        public List<NgControl> KontroleIzvanForme { get; private set; }
        public List<NgButton> Gumbi { get; private set; }
        public List<string> PreskoceniGumbi { get; private set; }
        public ShellLayout Raspored { get; private set; }

        public int BrojKontrola()
        {
            int n = KontroleIzvanForme.Count;
            for (int i = 0; i < Forme.Count; i++)
                n += Forme[i].Kontrole.Count;
            return n;
        }

        public int BrojElemenata(string ime)
        {
            int n;
            if (ime != null && _elementi.TryGetValue(ime, out n))
                return n;
            return 0;
        }

        public Dictionary<string, int> HistogramAtributa()
        {
            var d = new Dictionary<string, int>();
            for (int i = 0; i < _sviAtributi.Count; i++)
            {
                foreach (var kv in _sviAtributi[i].Stavke)
                {
                    int n;
                    d.TryGetValue(kv.Key, out n);
                    d[kv.Key] = n + 1;
                }
            }
            return d;
        }

        public NgForm NadjiFormu(string ime)
        {
            for (int i = 0; i < Forme.Count; i++)
            {
                if (Forme[i].Ime == ime)
                    return Forme[i];
            }
            return null;
        }

        public NgControl NadjiKontrolu(string name)
        {
            for (int i = 0; i < Forme.Count; i++)
            {
                var lista = Forme[i].Kontrole;
                for (int j = 0; j < lista.Count; j++)
                {
                    if (lista[j].Name == name)
                        return lista[j];
                }
            }
            return null;
        }

        public NgColumnGroup NadjiGrupu(string ime)
        {
            for (int i = 0; i < GrupeKolona.Count; i++)
            {
                if (GrupeKolona[i].Ime == ime)
                    return GrupeKolona[i];
            }
            return null;
        }
    }

    public sealed class NgLoadResult
    {
        private NgLoadResult(bool uspjeh, string putanja, string greska, NgProject projekt)
        {
            Uspjeh = uspjeh;
            Putanja = putanja;
            Greska = greska;
            Projekt = projekt;
        }

        public bool Uspjeh { get; private set; }
        public string Putanja { get; private set; }
        public string Greska { get; private set; }
        public NgProject Projekt { get; private set; }

        public static NgLoadResult URedu(string putanja, NgProject projekt)
        {
            return new NgLoadResult(true, putanja, null, projekt);
        }

        public static NgLoadResult Neuspjelo(string putanja, string poruka)
        {
            return new NgLoadResult(false, putanja, poruka, null);
        }
    }
}
