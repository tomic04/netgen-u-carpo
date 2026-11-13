using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;

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
            bool ima = sql.IndexOf("@@statusiid", System.StringComparison.OrdinalIgnoreCase) < 0
                && sql.IndexOf("@ID='5'", System.StringComparison.Ordinal) >= 0
                && parametri.IndexOf("@statusiid='5'", System.StringComparison.Ordinal) >= 0;
            l.Add(ima
                ? Qa.Z("GridRed", "GRD-01", "@@statusiid je stupac retka, vrijednost je '5'", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849")
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
                && !DijalogTok.OnLoadJeUpit(":FILTERFORM");
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
            bool redOk = izReda.IndexOf("@vrstadokid='249'", StringComparison.Ordinal) >= 0
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
            return l;
        }
    }
}
