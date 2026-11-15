# Treći neovisni pregled Carpo.NetGen — Fable 5.2, 15. 11. 2026.

Pregledavač: Claude Fable 5.2. Port je napisao Claude Opus 5.5, prvi neovisni pregled Claude Fable 5.1
(`pregled-neovisni-2026-11-14.md`, PN-01..PN-15, PR #134–#149). Ništa od toga nije uzeto na riječ: svaki nalaz ima
C# mjesto, VB redak, scenarij i test koji pada bez ispravka.

**Dokaz za sve potvrđene nalaze:** grana `pregled52/dokaz` (main + samo testovi), run **43757943205**, `FAIL=15`.
Isti testovi na vrhu složenih PR-ova (#154, run 43757718623): `FAIL=0`.
Brojevi redaka C# koda su za `main` 6f1c295.

## Potvrđeno

| ID | Ozbiljnost | Gdje (C#) | NetGen (VB) | Scenarij | Dokaz | Prijedlog |
|---|---|---|---|---|---|---|
| **PF-03** | **P0** (gubitak podatka) | `Ui/Dijalog.cs` 3123 `UcitajOnLoad`: datum iz ONLOAD-a → tekst `InvariantCulture` (`11/05/2026 00:00:00`); `PostaviVrijednost` (3277) ga upiše u `DateEdit.Text` | `Form2.vb` 2301 `Glbl.Value = Trs2.Rows(0)(ime)`; `PokupiParametre` 3531 šalje `'yyyyMMdd'` | Windows s hrvatskim postavkama (i runner sql-ai je hr-HR). Forma izmjene učita datum iz ONLOAD-a. DevExpress taj tekst ne pretvori u datum (`EditValue` ostane tekst), pa OK pošalje `@datum=null`: procedura upiše NULL ili javi grešku. `@@ime` ONLOAD datuma bez kontrole ide kao `'11/05/2026 00:00:00'` umjesto `'20261105'`. | `FAIL pf03-onload-datum-hr | … -> nema (11/05/2026 00:00:00)`, `FAIL pf03-onload-datum-parametar | @datumtekst=null,@datumdo=null,@datumdok=null`, `FAIL pf03-onload-datum-token` | PR **#150**: `DateEdit` dobije `DateTime`; tekstualni datum kroz `ProcitajDatum`; `@@ime` kao `'yyyyMMdd'`. |
| **PF-04** | **P0/P1** | `Grid/NgVeza.cs` 89, 153: `da.Fill(dt)` (DataTable) čita samo prvi rezultat; ostatak i greška u njemu se odbace | `Class1.vb` 141 `da.Fill(ds, …)` (svi rezultati) → Catch 156: MsgBox, ErrorLog 230, `Conn.Close/Open` 241-242; uz `#`/`+` vrati `ds.Tables(0)` (164-195) | Procedura vrati redove pa padne ili digne RAISERROR. Carpo: nema greške, veza se ne resetira, transakcija koju je procedura otvorila ostaje otvorena na vezi granule (sve što korisnik nakon toga "spremi" ide u nju i nestane pri zatvaranju). Poruka `… #OK` nakon tablice se ne pokaže. | `FAIL pf04-greska-nakon-tablice | iznimka='' @@trancount=1`, `FAIL pf04-tablica-uz-poruku`, `FAIL pf04-poruka-s-tablicom | master redova=1 … poruka=[]` | PR **#152**: `NgVeza.Napuni` puni DataSet; tablica uz poruku ide pozivatelju (`SqlException.Data`). |
| **PF-02** | **P1** | `Ui/Podloga.cs` 728 `IzvrsiOnSubmit` → `Citaj`: greška samo u statusnoj traci (`PostaviStatus`), koju `OsvjeziMaster` odmah prebriše | `Form1.vb` 5385-5507 → `cls1.ExecSql` → `Class1.vb` 156-205 MsgBox | Korisnik na dijalogu klikne OK, procedura digne `RAISERROR('Dokument je zaključen #')` ili padne. Dijalog se zatvori, poruke nema, korisnik misli da je spremljeno. **Regresija iz #138** (PN-01): dok je ONSUBMIT slao dijalog, poruku je pokazivao `Dijalog.Omotaj`. | `FAIL pf02-onsubmit-poruka | poruka korisniku: 0 []`, `FAIL pf02-onsubmit-greska` | PR **#151**: `CitajSPorukom` (`SqlPoruka.IzIznimke` + `NgPoruka.Info`). |
| **PF-01** | **P1** | `Ui/Podloga.cs` 1693 `ZapamtiNoviTrosak` (zove se iz `NakonDijaloga` 635, 706) i 1313 `_poslano.Add` (prazni se samo u klik pregledu) | `Form1.vb` 5385-5390 (`Tid = cf.ID` ili prva ćelija rezultata), 5509-5519 (ExecSql, pa PokaziUpit) | Kod klik pregleda za spin radi u produkciji. Čim je u podlozi poslan `spPrnTroskoviGlaAzur … @action=0`, nakon SVAKOG sljedećeg ONSUBMIT-a zadane grane podloga čita `select top 1 … from PrnTroskoviGla … order by PrnTroskoviGlaId desc` i taj id (zadnji u tablici, možda tuđi) upiše u `_master.Stanje.Id`, `Kontekst.Id` i `@PrnTroskoviGlaId`. NEXTFORM otvori sljedeću formu s tim id-em. `_poslano` raste dok je podloga otvorena. | `FAIL pf01-trosak-hack | upita 'from PrnTroskoviGla' izvan klik pregleda: 1`, `FAIL pf01-poslano-ne-raste | … 2 SQL-ova` | PR **#153**: `_klikPregled`; izvan klik pregleda ništa od toga ne radi. |
| **PF-05** | **P2** | `Ui/Dijalog.cs` `PrimeniRed` (1600-1650) ne čita stupac `findnode` | `Form2.vb` 4694 (`strFindNode`), `Form1.vb` 5523-5531 (`TreeView2.Nodes.Find`, odabir, širenje) | 20 ONCLICK-a u 9 XML-ova (DobUgovori, MER_DobUgovori, UgovorSKupcima, AOPNovi, KOD_Stavke): `…#OK#select @@partneriid as findnode`. Stablo nakon spremanja ostane na starom čvoru. | `FAIL pf05-findnode | čvor prije='1' poslije='1'` | PR **#154**: `Dijalog.FindNode`, `Podloga.OdaberiCvor`. |

PR-ovi su složeni jedan na drugi (#150 ← #151 ← #152 ← #153 ← #154) jer dijele test datoteku
`Carpo.NetGen.GridTest/PregledFable52.cs`. Svi mijenjaju ponašanje u Jupiteru i čekaju "spoji".

## Sumnje (nisu dokazane testom)

- **PF-06 — ONLOAD vrijednost comba i tip ključa.** `Dijalog.PostaviVrijednost` (3255-3264) u `LookUpEdit.EditValue` stavi
  `int`; ako je ključ u listi `numeric`/`smallint`/`bigint`, DevExpress red ne nađe i combo je prazan (parametar se
  ipak pošalje točno). VB `SelectedValue` (DataView.Find) pretvara tip. Test `pf06-combo-*` u run-u 43757943205 pada i
  za `int`, jer dijalog u testu nije prikazan — test ne dokazuje ništa. Treba provjeriti ručno na formi s combom čiji
  ključ nije `int`, ili testom s prikazanim dijalogom.
- **Ostale greške SQL-a na podlozi bez MsgBoxa.** Nakon #151 poruku ima ONSUBMIT i `Rafalno`. Detalj grid, stablo,
  SETDEFAULTS, dozvole i `==` segment i dalje pišu grešku samo u statusnu traku ili u grid; NetGen za svaki `ExecSql`
  pokaže MsgBox (`Class1.vb` 186/203). P3.
- **Kontrole s `PARENT`** (group/frame/panel, `Form2.vb` 2058-2059): `For Each ctl In Controls` (3422) ih ne vidi, pa u
  NetGenu nisu u `strParametri`/`strParametriSvi`/DBClipboardu; Carpo (`SveKontrole`) ih skuplja. 9 XML-ova, forme
  bez lijepljenog ONSUBMIT-a. P3.
- **NEXTFORM petlja šalje sirovi tekst** (`Form1.vb` 5387: `ExecSql(strStora & " " & strParametri)` bez
  `SrediSQLPosljeNextForme`); `ExecSql` preskoči SQL u kojem ostane `@@` (`Class1.vb` 113). Carpo `@@` zamijeni i
  izvrši. U korpusu jedna forma (`PDVPromjena.xml` DONESIXX, `@@datumpromjene`), a na nju nitko ne pokazuje. P3.
- **`!!` u granama `false`/`treeandrefreshmaster` i u dijalogu iz dijaloga** VB ne skida (5461, 5488, `Form2.vb` 4475),
  pa bi tamo SQL pao; `Lanac.Lijepi` ga skida svugdje. U korpusu svih 68 ONSUBMIT-a s `!!` je u granama `true` i
  zadanoj. Bez učinka.
- **Sweep nad jupiterom i ugniježđene procedure.** `Dijalog.OnLoadPise` gleda tijelo procedure ONLOAD-a forme, ali ne
  procedure koje ona zove; ONLOAD podloge, SQL comba, grida i stabla idu bez provjere tijela. Na jupiter2019 backupu
  sweep time može nešto upisati. Sigurnije: sweep nad kopijom baze ili prijavom bez prava upisa.
- **Kod klik pregleda u `Podloga.cs`** (≈700 redaka, uključujući `delete from jezici/statusi/Zupanije/PrnVrstaTroska`
  u `OcistiTest`) živi u produkcijskoj klasi. Nakon #153 ne radi izvan `ProvediKlikove`, ali mu je mjesto u TestShellu.
- **`@errorNumber` u ErrorLogu** (#148): VB šalje `Err.Number`, Carpo `SqlException.Number`. P3.

## Pregledano i ispravno

- `Lanac.Lijepi` (PN-03) prema `Form2.vb` 3376-3383 i `Form1.vb` 5449/5499/5511: DML po prvih 6 znakova, `!!`.
- `Podloga.NakonDijaloga` (PN-01/05/07/12): redoslijed grana `true`/`false`/`onlydet`/`onlytree`/`treeandrefreshmaster`/
  `treeandmaster`/zadana odgovara `Form1.vb` 5445-5519; NEXTFORM uzima prvu ćeliju rezultata (5390).
- Greška ONLOAD-a forme ne otvara formu (`Form1.vb` 5352, `Form2.vb` 4429): `Dijalog.GreskaOnLoad`.
- `NgVeza` nakon greške (PN-04) prema `Class1.vb` 164-242: reset veze osim za poruke s `#`/`+`.
- `ZastitaSql` (PN-15): tokenizer napreduje; u produkciji (`DijalogTok.Produkcija`) ne blokira ništa, sva mjesta sa
  zabranom pisanja (`SmijePoslati`, `JeZabranaPisanja`, `.Pise`) imaju izuzeće za produkciju.
- `Manager` bez `JSettings` (PN-06): `BezDefinicije` pokriva grupe, sume, izvoz, reset; ostali rukovatelji imaju
  provjeru ili su u Podlozi isključeni.
- `Form1.Rafalno` 6546-6910 prema `Form1Rafal.Vrsta` i `Podloga.IzvrsiRafalno`: redoslijed grana, `:forma` bez svih
  dvotočki, FOREACH preskače tijelo, greška prekida lanac.
- `Form2.RefreshControls` 4556-4750 prema `Dijalog.PrimeniRed`: focus, formcaption, beep, form, formwidth,
  formheight, click, setcolorforcolorpicker, id, temp (fali samo `findnode`, PF-05; stupac `fillexplorer` u korpusu
  nitko ne vraća).
- DBClipboard (`SpremiuDBClipboard` 4000-4023), zadani datum (`PripremiDatum`, DAT-01), parametri reda grida u kulturi
  računala (SQL-04).
- **#147 (PAR-03, Opus):** odgovara `Form2.vb` 3422-3901; komentar na PR-u. **#148 (PN-10):** odgovara `Class1.vb` 230;
  komentar na PR-u. Oba čekaju "spoji"; #148 i #152 diraju isti `catch` u `Podloga.Citaj` (ručni merge).
- Autorovi diferencijalni alati (`sql_diff`, `lanac_diff`) i `KorpusParseri` nisu ponovno puštani; Mono build ne-UI
  klasa prolazi i nakon #152.

## Ocjena prethodna dva pregleda

- **Zajednički propust: testovi ne pokrivaju ono što korisnik vidi.** Sweep i klik pregled broje iznimke i SQL greške,
  a ne provjeravaju je li poruka pokazana (PF-02), je li učitani datum poslan (PF-03) ni je li greška uopće stigla
  (PF-04). Runner je hr-HR, ali nijedan test nije učitavao datum iz ONLOAD-a u `DateEdit`.
- **Opus (port):** `Fill(DataTable)` umjesto `Fill(DataSet)` je tiha, a temeljna razlika prema `ExecSql` (PF-04). Kod
  klik pregleda pisan je u produkcijsku klasu i ostao aktivan (PF-01). ONLOAD datum kroz tekst (PF-03).
- **Fable 5.1 (pregled):** PN-01 je točan, ali je #138 prebacio ONSUBMIT na podlogu bez poruke o grešci (PF-02), a u
  NEXTFORM grani je dodao još jedan poziv `ZapamtiNoviTrosak` umjesto da ga makne (PF-01). Procjena "0 od 986" za `_`
  kontrole bila je kriva (već ispravljeno, PAR-03). U ovom pregledu sam i sam u komentaru na #147 prvo naveo krive
  brojeve redaka (ispravljeno) — brojeve treba provjeriti `grep -n`-om prije pisanja.

## Što dalje

1. "Spoji" za #150–#154 (redom), #147, #148. PF-03 i PF-04 su najvažniji.
2. PF-06 provjeriti ručno (combo s ključem koji nije `int`, forma izmjene).
3. Nakon #152 pustiti sweep na spinu: greške nakon prve tablice sada će se vidjeti i broj GRESKA može narasti — to su
   greške koje bi vidio i NetGen.
