using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using Carpo.NetGen.Shell;

namespace Carpo.NetGen.Granule
{
    public static class GridRed
    {
        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            var t = new DataTable();
            t.Columns.Add("id", typeof(int));
            t.Columns.Add("statusiid", typeof(int));
            t.Columns.Add("Korak", typeof(string));
            t.Rows.Add(7, 5, "Radni");
            string parametri = SqlTekst.IzReda(t.Rows[0]);
            var k = new SqlKontekst();
            k.ParametriSaGrida = parametri;
            string sql = SqlTekst.Sredi("spStatusiQuery @action=2, @ID=@@statusiid", k);
            // Class1.vb 862-915 (NadjiVrijednostuPrevParametrima): vrijednost koja nije datum gubi
            // navodnike, pa je @ID=5, ne @ID='5' (C1, tools/diff/vb_oracle.py).
            bool ima = sql.IndexOf("@@statusiid", System.StringComparison.OrdinalIgnoreCase) < 0
                && sql.IndexOf("@ID=5", System.StringComparison.Ordinal) >= 0
                && sql.IndexOf("@ID='5'", System.StringComparison.Ordinal) < 0
                && parametri.IndexOf("@statusiid='5'", System.StringComparison.Ordinal) >= 0;
            l.Add(ima
                ? Qa.Z("GridRed", "GRD-01", "@@statusiid je stupac retka, vrijednost je 5 (bez navodnika)", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849")
                : Qa.P("GridRed", "GRD-01", "@@statusiid iz grida", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849", sql + " | " + parametri));

            var kontrole = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            kontrole["Naziv"] = "klik-proba";
            string unos = DijalogTok.ZamijeniTokene("spJeziciAzur @action=0, @id=@@id, @Naziv='@@Naziv'", "0", kontrole);
            bool token = unos.IndexOf("@@", StringComparison.Ordinal) < 0
                && unos.IndexOf("@Naziv='klik-proba'", StringComparison.Ordinal) >= 0
                && unos.IndexOf("@id=0", StringComparison.Ordinal) >= 0;
            l.Add(token
                ? Qa.Z("GridRed", "GRD-02", "@@Naziv iz kontrole uđe u storu kao 'klik-proba'", "netgen/Form2.vb:3367,3406,3655")
                : Qa.P("GridRed", "GRD-02", "@@Naziv iz kontrole", "netgen/Form2.vb:3367,3406,3655", unos));

            bool onload = DijalogTok.OnLoadJeUpit("spStatusiQuery @action=2, @ID=@@statusiid")
                && DijalogTok.OnLoadJeUpit("select @@node as vrstadokid")
                && DijalogTok.OnLoadJeUpit("if @@id=0 raiserror ('n', 16, 1) else exec spRatArkod_Query @action='id', @id=@@id")
                && !DijalogTok.OnLoadJeUpit(":FILTERFORM")
                && !DijalogTok.OnLoadJeUpit("INSERT_KLASA");
            l.Add(onload
                ? Qa.Z("GridRed", "STA-01", "ONLOAD procedure se izvršava, ne samo select", "netgen/Form2.vb:1195-1205")
                : Qa.P("GridRed", "STA-01", "ONLOAD procedure", "netgen/Form2.vb:1195-1205", "sp nije upit"));

            var status = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            status["vrstadokid"] = "249";
            status["korak"] = "kp-izmjena";
            status["redoslijed"] = "1";
            status["smije"] = "0";
            status["zakljuceno"] = "0";
            status["aktivan"] = "1";
            status["obavezan"] = "0";
            string predlozak = "spStatusiAzur @Action=1,@ID=@@id,@vrstadokid=@@vrstadokid, @korak='@@korak', @redoslijed=@@redoslijed, @smije=@@smije, @zakljuceno=@@zakljuceno, @str='@@nizoznacenihajdijevaslijedeci', @aktivan=@@aktivan, @obavezan=@@obavezan";
            var ctx = new SqlKontekst();
            ctx.ParametriSaGrida = "@Next='a, b' , @statusiid='71' , ";
            string izmjena = SqlTekst.Sredi(DijalogTok.ZamijeniTokene(predlozak, "71", status), ctx);
            bool izmjenaOk = izmjena.IndexOf("@@", StringComparison.Ordinal) < 0
                && izmjena.IndexOf("=,", StringComparison.Ordinal) < 0
                && izmjena.IndexOf("@ID=71", StringComparison.Ordinal) >= 0
                && izmjena.IndexOf("@vrstadokid='249'", StringComparison.Ordinal) >= 0
                && izmjena.IndexOf("@korak='kp-izmjena'", StringComparison.Ordinal) >= 0
                && izmjena.IndexOf("@str=''", StringComparison.Ordinal) >= 0
                && izmjena.IndexOf("@smije='0'", StringComparison.Ordinal) >= 0;
            l.Add(izmjenaOk
                ? Qa.Z("GridRed", "STA-02", "Izmjeni složi @vrstadokid i ne ostavi prazan zarez", "netgen/Form2.vb:3377,3406; netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849,915")
                : Qa.P("GridRed", "STA-02", "Izmjeni SQL", "netgen/Form2.vb:3377; netgen/Class1.vb:915", izmjena));

            var prazan = new SqlKontekst();
            prazan.ParametriSaGrida = "@statusiid='71' , @Next='a, b' , ";
            string bez = SqlTekst.Sredi("spStatusiAzur @Action=1,@vrstadokid=@@vrstadokid,", prazan);
            bool bezOk = bez.IndexOf("=,", StringComparison.Ordinal) < 0
                && bez.IndexOf("@vrstadokid=null", StringComparison.Ordinal) >= 0;
            l.Add(bezOk
                ? Qa.Z("GridRed", "STA-03", "nedostajući @@vrstadokid postane null, ne prazan zarez", "netgen/Class1.vb:915")
                : Qa.P("GridRed", "STA-03", "null umjesto praznog zareza", "netgen/Class1.vb:915", bez));

            var ctxRed = new SqlKontekst();
            ctxRed.ParametriSaGrida = "@Next='a, b' , @vrstadokid='249' , ";
            string izReda = SqlTekst.Sredi("@vrstadokid=@@vrstadokid,", ctxRed);
            bool redOk = izReda.IndexOf("@vrstadokid=249,", StringComparison.Ordinal) >= 0
                && izReda.IndexOf("=,", StringComparison.Ordinal) < 0;
            l.Add(redOk
                ? Qa.Z("GridRed", "STA-04", "zarez unutar vrijednosti ne guta sljedeći parametar", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:862")
                : Qa.P("GridRed", "STA-04", "zarez u vrijednosti", "netgen/Class1.vb:862", izReda));

            var forma = new NgForm(
                "DELETEFORM",
                new NgAttributeBag(new Dictionary<string, string>()),
                "spStatusiAzur @action=2,@ID=@@id",
                "",
                new List<NgControl>(),
                new List<NgButton> { new NgButton(1, "Obriši", ""), new NgButton(2, "Odustani", "") },
                new List<string>());
            DijalogTok.Klik brisi = DijalogTok.Gumb(forma, 1, "", false);
            string obrisano = DijalogTok.ZamijeniTokene(brisi.MasterSql, "71", null);
            bool brisiOk = brisi.SaljeSql
                && obrisano.IndexOf("spStatusiAzur", StringComparison.OrdinalIgnoreCase) >= 0
                && obrisano.IndexOf("@action=2", StringComparison.OrdinalIgnoreCase) >= 0
                && obrisano.IndexOf("@ID=71", StringComparison.Ordinal) >= 0
                && obrisano.IndexOf("@@", StringComparison.Ordinal) < 0
                && obrisano.IndexOf("spStatusiQuery", StringComparison.OrdinalIgnoreCase) < 0;
            l.Add(brisiOk
                ? Qa.Z("GridRed", "STA-05", "Obriši šalje ONSUBMIT spStatusiAzur @action=2", "netgen/Form2.vb:3377-3388")
                : Qa.P("GridRed", "STA-05", "Obriši ONSUBMIT", "netgen/Form2.vb:3377-3388", (brisi.SaljeSql ? "salje " : "ne salje ") + obrisano));

            bool plavo = MultiOdabir.Je("MULTIBLUE_ADD")
                && !MultiOdabir.Je("INSERT_KLASA")
                && MultiOdabir.Ubaci("12", "7").IndexOf("TotalGenMultiSelectTemp", StringComparison.Ordinal) >= 0
                && MultiOdabir.Ubaci("12", "7").IndexOf("select 12,", StringComparison.Ordinal) >= 0
                && MultiOdabir.Obrisi("7").IndexOf("operateriid=7", StringComparison.Ordinal) >= 0
                && MultiOdabir.ObrisiKorisnika().IndexOf("susersname= suser_sname()", StringComparison.Ordinal) >= 0;
            l.Add(plavo
                ? Qa.Z("GridRed", "WEB-01", "MULTIBLUE puni TotalGenMultiSelectTemp s GetID", "netgen/Form1.vb:4931-5032")
                : Qa.P("GridRed", "WEB-01", "MULTIBLUE temp", "netgen/Form1.vb:4932-5032", MultiOdabir.Ubaci("12", "7")));

            var gumbi = GumbiNiz.Slozi(null, "Dodaj:INSERT#Izmjeni:UPDATE#Obriši:DELETE");
            bool buttons = gumbi.Count == 3
                && gumbi[0].Natpis == "Dodaj" && gumbi[0].Naredba == "INSERT"
                && gumbi[2].Naredba == "DELETE";
            l.Add(buttons
                ? Qa.Z("GridRed", "BTN-01", "BUTTONS prepiše natpis i naredbu gumba", "netgen/Form1.vb:3770-3824")
                : Qa.P("GridRed", "BTN-01", "BUTTONS", "netgen/Form1.vb:3770-3824", gumbi.Count.ToString()));

            string spojeno = ParametriModula.Dodaj("@Id='0' , ", "@PrnTroskoviGlaId='8' , ");
            var dijete = new SqlKontekst();
            dijete.Id = "0";
            dijete.ParametriSaGrida = spojeno;
            string stavka = SqlTekst.Sredi("spPrnTroskoviDetAzur @Action=0, @PrnTroskoviGlaId=@@PrnTroskoviGlaId,", dijete);
            bool tro = stavka.IndexOf("@@", StringComparison.Ordinal) < 0
                && stavka.IndexOf("@PrnTroskoviGlaId=8,", StringComparison.Ordinal) >= 0
                && spojeno.StartsWith("@PrnTroskoviGlaId='8'", StringComparison.Ordinal);
            l.Add(tro
                ? Qa.Z("GridRed", "TRO-01", "Podforma stavke vidi @@PrnTroskoviGlaId iz ONLOAD-a roditelja", "netgen/Form2.vb:3906; netgen/Class1.vb:765-849")
                : Qa.P("GridRed", "TRO-01", "@@PrnTroskoviGlaId iz roditelja", "netgen/Form2.vb:3906; netgen/Class1.vb:765-849", stavka));

            var prazanKontekst = new SqlKontekst();
            string bezMedjuspremnika = SqlTekst.ZaGumb("spPrnTroskoviDetAzur @Action=0, @PrnTroskoviGlaId=@@PrnTroskoviGlaId,", prazanKontekst);
            bool troPrazno = bezMedjuspremnika.IndexOf("@@", StringComparison.Ordinal) < 0
                && bezMedjuspremnika.IndexOf("@PrnTroskoviGlaId=null", StringComparison.Ordinal) >= 0;
            l.Add(troPrazno
                ? Qa.Z("GridRed", "TRO-02", "prazan međuspremnik pretvori @@PrnTroskoviGlaId u null", "netgen/Class1.vb:915")
                : Qa.P("GridRed", "TRO-02", "prazan međuspremnik", "netgen/Class1.vb:915", bezMedjuspremnika));

            string bezZareza = Lanac.BezKrajnjegZareza("spPrnTroskoviDetAzur @Action=0, @PrnTroskoviGlaId=null, @Vrijednost=0,");
            bool zarez = bezZareza.EndsWith("0", StringComparison.Ordinal) && !bezZareza.EndsWith(",", StringComparison.Ordinal);
            l.Add(zarez
                ? Qa.Z("GridRed", "TRO-03", "ONSUBMIT koji završava zarezom ne šalje zadnji zarez", "netgen/Form2.vb:3884,3898; netgen/Form1.vb:5452")
                : Qa.P("GridRed", "TRO-03", "zadnji zarez", "netgen/Form2.vb:3884,3898", bezZareza));

            var bojaK = new SqlKontekst();
            bojaK.SelectedColor = "Black";
            string bojaSql = SqlTekst.Sredi("select '@@selectedcolor' as boja", bojaK);
            bool boja = bojaSql.IndexOf("@@", StringComparison.Ordinal) < 0
                && bojaSql.IndexOf("Black", StringComparison.Ordinal) >= 0;
            l.Add(boja
                ? Qa.Z("GridRed", "BOJ-01", "COLORPICKER stavi HTML u @@selectedcolor i select as boja", "netgen/Form2.vb:7156-7163; netgen/Form1.vb:6813-6831")
                : Qa.P("GridRed", "BOJ-01", "@@selectedcolor", "netgen/Form2.vb:7160", bojaSql));

            string akcija = SrediPoslije.Petlja("select @@akcijeid as akcijeid", "", null, false);
            bool akc = akcija.IndexOf("@@", StringComparison.Ordinal) < 0
                && akcija.IndexOf("null", StringComparison.Ordinal) >= 0;
            l.Add(akc
                ? Qa.Z("GridRed", "AKC-01", "@@akcijeid bez međuspremnika postane null prije slanja", "netgen/Class1.vb:915")
                : Qa.P("GridRed", "AKC-01", "@@akcijeid", "netgen/Class1.vb:915", akcija));

            bool trazilica = ParametriSvi.SearchId(null) == "null"
                && ParametriSvi.SearchId("0") == "null"
                && ParametriSvi.SearchId("5") == "5";
            l.Add(trazilica
                ? Qa.Z("GridRed", "TRO-04", "prazan search šalje null, ne natpis kontrole", "netgen/Form2.vb:3627-3632")
                : Qa.P("GridRed", "TRO-04", "search id", "netgen/Form2.vb:3627-3632", ParametriSvi.SearchId("0")));

            int spremi = PotvrdaGumba.Indeks(
                new string[] { "ButtonNG142", "Spremi", "Odustani" },
                new string[] { ":WEBBROWSER", "spRatArkod_Azur @action='insert'", "CLOSE" });
            var prazno = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            prazno["RobaId"] = "";
            prazno["Naziv"] = "";
            string prazanSearch = DijalogTok.ZamijeniTokene("exec x @robaid=@@RobaId, @naziv='@@Naziv'", "0", prazno);
            bool arkod = spremi == 1
                && prazanSearch.IndexOf("@robaid=null", StringComparison.Ordinal) >= 0
                && prazanSearch.IndexOf("@naziv=''", StringComparison.Ordinal) >= 0;
            l.Add(arkod
                ? Qa.Z("GridRed", "ARK-01", "Spremi ide prije :WEBBROWSER, prazan search id je null", "netgen/Form2.vb:3367,3616")
                : Qa.P("GridRed", "ARK-01", "Spremi i prazan search", "netgen/Form2.vb:3367,3616", spremi.ToString() + " " + prazanSearch));
            return l;
        }
    }
}
