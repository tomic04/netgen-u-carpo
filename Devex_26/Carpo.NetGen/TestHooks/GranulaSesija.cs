using System;
using System.Collections.Generic;

namespace Carpo.NetGen.TestHooks
{
    /// <summary>
    /// Kuka za QA granule. Produkcijski ulaz (Loader, frm, TestShell) je ne zove.
    /// Dok je <see cref="Izvrsi"/> null, test u tests/Carpo.NetGen.Granule broji
    /// stavku kao otvorenu (funkcionalnost jos nije izvedena) i build ostaje zelen.
    /// Kad granula stvarno radi, postavi Izvrsi da vrati <see cref="Pogled"/> s
    /// kljucevima iz docs/granule. Tada test usporeduje vrijednosti i pad rusi build.
    /// Ne otvaraj prozor i ne salji UPDATE/INSERT/DELETE. SQL koji bi NetGen poslao
    /// upisi u kljuc sql, a citanja smiju ici na spin (samo citanje).
    /// </summary>
    public static class GranulaSesija
    {
        public static Func<Zahtjev, Pogled> Izvrsi;

        static GranulaSesija()
        {
            GranulaIzvrsenje.Spoji();
        }
    }

    public sealed class Zahtjev
    {
        public string XmlPutanja { get; set; }

        /// <summary>otvori | gumb | dblclick | prihvati | odustani | escape | kontrola. Za kontrolu Cilj je Forma.ImeKontrole.</summary>
        public string Akcija { get; set; }

        /// <summary>Natpis gumba podloge, ili ime kontrole na dijalogu.</summary>
        public string Cilj { get; set; }

        /// <summary>Vrijednost trenutnog master reda (Janus GetID: kolona id, inace stupac indeks 2).</summary>
        public string MasterId { get; set; }

        /// <summary>ime=vrijednost odvojeno tockom-zarezom. Apostrof i zarez ostaju sirovi.</summary>
        public string Vrijednosti { get; set; }
    }

    public sealed class Pogled
    {
        /// <summary>False: ova akcija jos nije izvedena, test ostaje otvoren.</summary>
        public bool Izvedeno { get; set; }

        public string Napomena { get; set; }

        /// <summary>Kljucevi su u docs/granule uz stavku. Vrijednost je tekst (true/false, broj, SQL).</summary>
        public Dictionary<string, string> Stavke { get; set; }
    }
}
