# Faza C2: lanac naredbi dijaloga (`Form2.Rafalno`)

Datum: 2026-11-13. Grana: `pregled/lanac-c2` (na `pregled/sql-c1`).

## Metoda

`tools/diff/lanac_vb.py` doslovno prenosi redoslijed `ElseIf` grana iz `Form2.vb` 6821–7509. Grana se bira po `Trim.ToUpper.Contains`, pa redoslijed određuje ishod kad segment sadrži više ključnih riječi.

`tools/diff/LanacVoz.cs` izvršava C# `DijalogTok.Koraci` pod Mono.

Usporedba je nad **5 885 jedinstvenih segmenata**: svi `ON*` događaji kontrola u formama iz 609 granula, podijeljeni na `#`. Za pokretanje treba `tools/diff/stubs/` (nadomjestak `PrintBridge` bez DevExpressa).

## Rezultat

| | Prije | Poslije |
|---|---:|---:|
| Segmenti koje VB i Carpo prepoznaju drugačije | 159 | **2** |

Preostala 2 su greške autora XML-a, gdje Carpo radi ono što je autor htio: `:printgrid(grid)` i `:FISKALIZACIJA(...)`. VB oba otvara kao formu tog imena, koja ne postoji. Prijedlog: razlika R.

## Nalazi

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **LAN-01** | P0 | `ASSEMBLYLOAD(...)` u lancu s više segmenata | `Form2.vb` 7424: učita sklop i zove `totalgen` | prepoznat kao SQL i poslan serveru, a DLL se ne zove. Pogađa spremanje DMS-a (`select 'savedms' as akcija #ASSEMBLYLOAD(DmsNetGen.clsDms) #OK`, 10+ granula), `MPBlagajna` (slanje računa), `MPOdjavaNarNG` (Excel), `Narudzbe` | **ispravljeno**: korak `assembly`, `AssemblyPoziv` s potpisom dijaloga, pad prekida lanac |
| **LAN-02** | P1 | `POSLOVNIPROSTOR(...)` u lancu | `Form2.vb` 7489 | poslan kao SQL | **ispravljeno**: bilježi se kao na podlozi (fiskalni dio je R109/R111) |
| **LAN-03** | P0 | Segment s `@@` se zamijeni **prije** prepoznavanja | `Form2.vb` 6812–6819 | prepoznavanje na sirovom tekstu: `:@@imereporta1(@id=…)` otvara formu doslovno nazvanu `@@imereporta1`, a `@@izvjestaj(...)` ide serveru kao SQL. Pogađa ispise u PNNalozi, PNObracun, LVObracun, HRManagerGO i JOPPD | **ispravljeno**: izvršitelj lanca zamijeni pa ponovno prepozna segment |
| **CB-01** | P0 | Međuspremnik podloge (SETDEFAULTS) u dijalogu i na gumbu | `cls1.CB` je globalan, pa ga `NadjiVrijednostuPrevParametrima` vidi svugdje | dijalog i gumb nisu vidjeli CB, pa je `@@imereporta1` iz SETDEFAULTS postao `null` | **ispravljeno**: `SqlKontekst.Clipboard`, koji `ZaFormu` prenosi u dijalog |
| LAN-04 | P2 | SQL koji u tekstu vraća `':REPX/…'` | `Form2.vb` 7509: SQL | prepoznat kao predložak ispisa | **ispravljeno**: predložak samo kad segment počinje s `:` |
| **ISP-02** | P1 | `:RS/…`, `:GG/…`, `:rsgen:…`, `:vrstadokid-…`, `:….clsReport` (14 segmenata u dijalozima, više na podlozi) | `NovaGenerickaForma2` → ispis (`Form2.vb` 4276) | `OtvoriFormu` ih ne poznaje ("nema forme"), a `PrintBridge.Ispisi` zna samo za `repx` i ispis grida | **djelomično**: `vrstadokid-` (117 puta, 83 granule) ide u Spin.Printing kao `IspisiMePrekoSpinPrintinga` (PrintRpt 443-461), broj u zagradi je id, na podlozi za svaki označeni red (ISP-06). Otvoreno: `rs:/` (6 granula), `rsgen` (4), `.clsReport` (16) preko PrintDlg COM-a |
| ISP-03 | P2 | `IspisMotora.JeIspisImena` | `Substring(0, 5) = "RSGEN"` | `tri == "RSGEN"` uspoređuje 3 znaka s 5, pa nikad nije istina | **otvoreno → C8** (bez ISP-02 ispravak ne mijenja ishod) |

## Testovi

| Test | Što |
|---|---|
| `lan01-assembly-nije-sql` | `select 1 as x#ASSEMBLYLOAD(...)#OK`: ASSEMBLYLOAD ne ide serveru |
| `lan03-token-u-ispis` | `:@@imereporta1(@id=5)#CLOSE` uz CB `imereporta1=repx/rptX` ide u ispis `rptX`, a ne u SQL |
| `tools/diff/lanac_diff.py` | prepoznavanje svih 5 885 segmenata korpusa protiv VB modela |

## Događaji kontrola dijaloga (EVT)

`Form2` sve događaje kontrola šalje u `Rafalno`, isto kao gumb (`Form2.vb` 5638–6228, 8506–8677, 9439–9474). Carpo je cijeli tekst događaja slao kao **jedan SQL**. Naredbe (`:FORMA`, `MSGBOX`, `REFRESHCONTROL`, `OK`, `#`…) u događaju zato nisu radile, a niz od više SQL-ova išao je kao jedan.

Korpus (584 granule): ONLOSTFOCUS 2 700+ (NUMBER 1 253, SEARCH 555, COMBO 515…), ONDBLCLICK grida 358, ONCELLEDITED 221, ONCLICK comboa/kvačice/radija/grida/labele 530, AFTERREFRESH 104, ONENTERKEY 12, ONLOADED 907 (217 granula), ONCLOSED 68, ONESCKEY 52.

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **EVT-01** | P0 | Događaj kontrole ide kroz lanac | `Rafalno(sender.On…SQL)` | `IzvrsiSql(tekst)`, jedan SQL | **ispravljeno**: `IzvrsiDogadjaj`, isti izvršitelj kao gumb (`IzvrsiKorake`). Jedan obični SQL ide kao i prije |
| **EVT-02** | P1 | Enter u tražilici | `Glbl_Validated`: traži, `RefreshKontrola`, `ONLOSTFOCUS` | ONLOSTFOCUS se na Enter nije slao | **ispravljeno**. Izlaz Tabom s upisanim tekstom ne šalje ništa (`Validated2` je u VB-u isključen, 2210); prazna tražilica na izlazu šalje |
| **EVT-03** | P1 | Greška SQL-a u lancu | `GreskaKodIzvrsavanjaSQLa` → `Exit For` (7510–7516) | lanac je išao dalje (ispis, OK, nova forma nakon greške) | **ispravljeno**: lanac staje i vraća grešku |
| **EVT-04** | P2 | Atribut fokusa | `ONGOTFOCUS` (2051) | čitao se `GOTFOCUS`, kojeg u korpusu nema | **ispravljeno** (`GOTFOCUS` ostaje kao zamjena) |
| **FRM-01** | P0 | `ONLOADED` forme | `Form2_Load` → `Rafalno` (5794) | nije se izvršavao | **ispravljeno**; uz to `ONACTIVATED` (4030) i `ONCLOSED` (4082) |
| **ESC-01** | P1 | Escape | `ONESCKEY` kroz Rafalno, inače zatvori (4158–4166) | ništa | **ispravljeno**; otvoren popis comboa/datuma prvo zatvori editor |
| **ENT-01** | P2 | `ONENTERKEY` grida | 6071–6076 | nije spojen | **ispravljeno**; grid bez `EDITCOLUMNS` ide na sljedeću kontrolu (6040) |
| **GCL-01** | P1 | `ONCLICK` grida | `SelectionChanged` + `Click` uz `ZanemariClick` (5623, 6107) | nije spojen | **ispravljeno**: promjena reda, a klik koji ju je napravio se preskoči. Nakon osvježavanja jednom |
| **CEL-01** | P1 | `ONCELLEDITED` | stupci reda postanu parametri (8640–8674), greška vrati staru vrijednost | grid je bio `Editable=false`, pa nikad; bez parametara reda | **ispravljeno** s GRD-01 |
| **AFT-01** | P1 | `AFTERREFRESH` grida | `RefreshGrida` → `Rafalno` (5426) | nije postojao | **ispravljeno** |

## Rezultat SQL-a događaja (RES)

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **RES-01** | P0 | Prvi red rezultata postavlja kontrole | `RefreshControls` (4556–5190) i `DodatnoPostimajKontrolu` (4497–4552): `enable/disable/visible/invisible/tabstoped`, vrijednost broja, datuma (`checked`/`unchecked`), kvačice (CBool), radija, comboa (SelectedValue, inače prvi), tražilice (Tag i tekst iz stupca CAPTION), gumba, labele; `refresh`/`move*` grida; posebni stupci `focus`, `formcaption`, `beep`, `form`, `formwidth/height`, `click`, `setcolorforcolorpicker`, `id`, `temp` | upisivao je samo tekst u polja | **ispravljeno**. U korpusu `'disable'` ~820, `'enable'` ~770, `focus` ~960, `'refresh'` ~1 300 puta |
| RES-02 | P2 | `tabstoped`/`tabnotstoped` u tekstnom polju | VB upiše i tu riječ u tekst (nije u popisu od četiri) | — | Carpo je ne upisuje (50-50 → Carpo) |
| RES-03 | P2 | DBNull u rezultatu | dodjela u String baci iznimku, kontrola ostaje | Carpo je upisivao "" | **ispravljeno** kao NetGen |
| REF-01 | P1 | `REFRESHCONTROL(ime)` | `RefreshControls("select 'refresh' as ime")` | osvježavao je samo combo, bez parametara dijaloga | **ispravljeno**: isti put kao stupac `refresh` |

## Tražilica (TRZ), osvježavanje nakon podforme (RAC), parametri (CMB)

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **TRZ-01** | P0 | Tražilica (1 739 polja u 338 od 584 granule) | `Form3.Trazilica`: `sp…`/`select…` s `@@searchboxIME`, ili `tablica#polje#…` → `select … like`; jedan red bez prozora, više → prozor; ništa ili odustajanje brišu | nije tražila: polje je ostajalo bez ID-a | **ispravljeno**: `TrazilicaProzor` (DevExpress XtraForm + GridControl, COLUMNCAPTION/COLUMNWIDTH/COLUMNAUTORESIZE), `TrazilicaTok` (id iz `id` ili prvog stupca, tekst iz `naziv*` ili drugog stupca) |
| **TRZ-02** | P1 | `@SEARCH` parametri | Janus stupac 1 je kvačica, pa je i prvi stupac podatka (`@SEARCHid`) parametar | `IzTrazilice` je preskakao prvi stupac; `@@SEARCH…` u lancu nije bio zamijenjen (1 497 puta u 277 granula) | **ispravljeno** |
| **RAC-01** | P0 | `REFRESHAFTERCHILD="TRUE"` (836 gridova, 49 comboa) | `RefreshKontrola` nakon podforme (4443, 4478) i tražilice | nije postojalo: dodana stavka se ne vidi | **ispravljeno**: `OsvjeziNakonDjeteta`, SQL s parametrima dijaloga, grid zadrži red |
| **CMB-01** | P1 | `PokupiParametre` puni CB | i `@searchboxIME`, `@comboboxIME` (3904–3981) | CB se iz dijaloga nije punio; `@@comboboxIME` (216 puta) je bio `null` | **ispravljeno**: `ClipboardNg.UpisiParametre` |
| **GRD-01** | P1 | Stupci grida dijaloga | `CreateGridColumns` s `EDITCOLUMNS` (1643–1673), `MultipleSelection` | sirova imena, vidljivi `*ID`, nije se moglo uređivati | **ispravljeno**: `GridIzgled.Primijeni` (isti `MasterGrid.IzPodatka` kao podloga) |
| **BTN-02** | P1 | CMD3–CMD6 dijaloga | `Button3..6_Click` → `NovaGenerickaForma2(Tag)` (5800–5818), bez Rafalna | tag je išao serveru kao SQL (35 gumba) | **ispravljeno**: forma, ispis ili objekt; nakon forme RAC-01 |
| ISP-04 | P1 | `ispis(parametri)` | `NovaGenerickaForma2` rastavi ime i `tpP` (4277–4288) | `rptX(@id=5)` kao ime predloška | **ispravljeno**: `PrintBridge.BezZagrade` |
| **ISP-07** | P1 | `PRINTGRID(ime)` u lancu/gumbu dijaloga (karticagrid, 3 granule) i „Ispis” u izborniku grida (podloga i dijalog) | `TempGrid = Controls(ime)`; `IspisGRIDREPX`/`IspisNovi` (Form2 9015-9056, Form1 isto): PAGEHEADERLEFT/CENTER/RIGHT grida kroz SrediSQL2 i SrediSQLPosljeNextForme s datumom za ispis, prazan centar je naslov, PAGEORIENTATION grida (LANDSCAPE u 344 grida) | lanac je zvao ispis bez grida, gumb prvi grid bez zaglavlja i s orijentacijom forme; „Ispis” u izborniku je bio goli pregled grida | **ispravljeno**: `Dijalog.IspisiGrid` i `Podloga.IspisiGrid` preko Spin.Printing PrintLink s tim zaglavljima i orijentacijom; jedna stavka „Ispis” (VB ima i „Ispis REPX”, isti ispis drugim motorom) |

Otvoreno iz ovog pregleda (stanje nakon C4):

- ~~**FOR-01**~~: ispravljeno u C4 (`FOREACH*` vrti cijeli lanac po retku, test `for01-foreachrow`).
- **SQL-07** (P2): lanac dijaloga sređuje SQL kroz `Pripremi` (`ZamijeniTokene` + `ZaGumb`), a jedan SQL događaja kroz `SrediPoslije.Zamijeni` s `strParametriSvi`, kao `RefreshControls`. Usporediti s `vb_oracle` na korpusu.
- ~~**MDR-01**~~: ispravljeno u C4 (stavke, `@ime='id~id'`, test `mdr01-multidrop`).
- **ISP-05** (P2): `rs:/…`, `rsgen` i `.clsReport` traže stari PrintDlg COM; čeka odluku ostaje li on na računalima.
- ~~`TIMERSQL`, `MULTILINE`/`ENTERASTAB`, `STYLE`, `HOTKEY`~~: obrađeno u C4 (TXT-02, HOT-01, LAY-01; TIMERSQL se ni u NetGenu ne izvršava).

## Testovi (događaji)

| Test | Što |
|---|---|
| `dijalog-ucinak` | prazna tražilica na izlazu šalje ONLOSTFOCUS jednom; Enter šalje jednom, izlaz nakon toga ne |
| `frm01-onloaded` | ONLOADED postavi labelu kroz rezultat |
| `evt01-lanac-rezultat` | ONLOSTFOCUS s dva SQL-a: `disable` i tekst, pa `refresh` grida i AFTERREFRESH |
| `trz01-jedan-red`, `trz01-prozor` | `spTrazi '@@searchboxpartner'`, jedan red bez prozora, više redova kroz prozor; REFRESHAFTERCHILD s novim id-em |
| `rac01-nakon-djeteta` | `:DIJETE` otvori podformu i osvježi grid |
| `cel01-grid-red` | EDITCOLUMNS, skriveni `*ID`, ONCELLEDITED s parametrima reda |
| `ent01-grid-enter`, `esc01-onesckey`, `cmb01-cb-dijalog` | ONENTERKEY, ONESCKEY, CB iz dijaloga |
| `res01-*`, `trz01-*`, `cel01-parametri`, `rac01-*`, `cmb01-cb` | čista pravila bez prozora |

## Što je još u C2

- Lanac podloge (`Form1.Rafalno`, `Form1.vb` 6546–6850) i `GumbTok`/`Podloga`: ista usporedba prepoznavanja.
- Učinak pojedinih grana (MSGBOX, ISEMPTY*, EVAL, CREATETEXTFILE*, REFRESHCONTROL, DOCK*) protiv `Form2.vb`, grana po grana.
- `FOREACH*` na dijalogu (`ForEachSelected` / `BlueRow` / `Row`).
