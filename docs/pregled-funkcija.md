# Pregled funkcija NetGen / Carpo

Prvi prolaz su funkcije koje obrađuju podatke: zamjena `@@`, `PokupiParametre`, SQL, validacija i format broja i datuma. Izvor je `netgen/`. U `netgen/*.vb` (bez designer datoteka) ima 476 `Sub`/`Function`. Ovaj dokument pokriva podatkovni sloj. Ostali moduli nisu u ovom prolazu.

Status: `isto` prati NetGen, `razlika` je R-broj koji ostaje, `nedostaje` još nema porta.

## Zamjena parametara

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Form1.SrediSQL` 2470–2503. `@@id` (tri veličine), `@@operateriid`, `@@detid`, `@@node`, `@@table`, `@@xmlname`, `@@jupitersrvpath`, `@@editedcolumn`, `@@lastvalue`, `@@selectedcolor` | `SqlTekst.Sredi` | isto | `sql-zamjena` |
| `Form2.SrediSQL2` 2876–2920. Isto osim `@@Id` i `@@table`. Dodaje `@@tempstring`, `@@dblclickedcolumn`, `@@VERSION`, `@@OPISNIPARAMETRI`, `@@FOCUSEDCONTROL`, `@@selectedfile`, `@@selectedpath` | `SqlTekst.Sredi2` | isto | `sredi2-temp`, `sredi2-id-nije-Id` |
| `@@GRIDROWCOUNT(` u `SrediSQL2` 2899–2905 je komentar. Živa petlja reže token na `(` i nepoznato postavi na `null` | `SrediPoslije.Zamijeni` | isto | `gridrowcount-komentar` |
| `Class1.SrediSQLPosljeNextForme` 740–853. Prvo `SrediSQL2`. Token `@@` staje na razmak, navodnik, `%`, `#`, zarez, `)`, `*`, `/`, `\`, `+`, `-`, `.`, `(`, `=`. `¸¸` postaje `''` | `SrediPoslije.Zamijeni` i `Petlja`. Master šalje `Petlja` nakon `Sredi`. Combo ide istim putem. Dijalog na OK zove `ZaSlanje` | isto | `sredi-poslije-roba`, `sredi-poslije-navodnik`, `intrastat-ostao` |
| `Class1.NadjiVrijednostuPrevParametrima` 856–977. Skine jedan `@`, prvi `@ime=` koji počinje tim prefiksom. `ajdijeva` mijenja `~` u zarez. Prazno je `null`. `@napomena` i `null` je prazan string. `@DAT` i grid-datum pakiraju datum. Inače se navodnici skidaju. Prazno ili `null` gleda clipboard | `SrediPoslije.Nadji` | isto | `sredi-poslije-roba`, `sredi-poslije-ajdijeva`, `sredi-poslije-datum`, `sredi-poslije-napomena`, `sredi-poslije-null`, `sredi-poslije-cb` |
| `Form2.SrediSQLsaKolonamaizGrida` 8440–8456 i `NadjiVrijednostuGridu` 8459–8490. `@(grid..kolona)`. Datum iz `dd.MM.yyyy`, broj mijenja zarez u točku, prazno je `null` | `SrediPoslije.Kolone`. Vrijednost daje pozivatelj | isto | `kolone-grid` |
| Ispis zaglavlja: `Form1` 6001 zove isti `SrediSQLPosljeNextForme` s `DatumjezaIspis`, pa `Rafalna_Priprema` i `Translate` | `IspisZaglavlje.Desno`. Petlja je ista granica tokena, clipboard je rječnik, `ISEMPTY` ide prije prijevoda | isto | `isempty-prazno`, `isempty-vrijednost`, `podnozje-isempty` |

`SETDEFAULTS` na podlozi puni clipboard prije master SQL-a (`Form1.vb` 3862–3882 i `SetDefolte` 3945–4009). `Podloga` zove `ClipboardNg.IzSetDefaults`, a `MasterGrid` taj rječnik daje `SrediPoslije`. Ključ je `ime=`. Greška na segmentu prekida ostatak.

## Skupljanje parametara

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Form2.PokupiParametre` 3406–3906. Check je `CheckState`, radio 1/0, broj prazan je 0 i zarez je točka, tekst ima `¸¸` i `~~`, datum je `yyyyMMdd` ili `null`, search je id i `@searchbox`, grid je id, nizovi i `@rowcount` | `Dijalog.SviParametri` i `ParametriSvi` | isto | `param-radio`, `param-broj`, `param-search`, `FAK-07` |
| `Form2.Button1_Click` 3377–3388. Prvih 6 slova u `INSERTDELETEUPDATESELECT` ugradi `@@` i obriše niz parametara. Inače se niz lijepi, osim uz `!!` | `SrediPoslije.ZaSlanje`. OK na dijalogu šalje taj tekst | isto | `za-slanje-dml`, `za-slanje-sp`, `za-slanje-uzvicnik`, `dml-ne-lijepi`, `uzvicnik-gasi-lijepljenje` |
| `Form1.PokupiParametreSaGrida` 2544–2612. Stupac 0 na Janusu je ikona pa petlja kreće od 1. Zadnje 4 znamenke koje nisu 0 otpadaju. Razmak je `_`, `!` se briše. Broj mijenja zarez u točku. `DBNull` se preskače | `DetailGrid.ParametriIzReda`. `DataRow` nema ikonu, pa kreće od prvog stupca | isto | `parametri-grida` |
| `Form1.PokupiParametreSaHijerarHijskogGrida` 2615–2643. `qwer`/`t2`/`t3`/`t4` bira karticu. Ime je caption, od stupca 0, bez skidanja 4 znamenke | `DetailGrid.ParametriHijerarhije`, `IndeksKartice` | isto | `hijerarhija-grid` |
| `Form1.GetID` 5566 i `GetDetID` 5605. Prazan grid je `0` | `DetailGrid.IdReda` i `SqlTekst.ZaFormu` | isto | `sql-zamjena` |
| `Form2.PokupiParametreIzTrazilice` 5926 | `ParametriSvi.Search` | isto | `param-search` |

## SQL i validacija

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Class1.ExecSql` 85–118. `~~` je zarez. Tekst koji počinje s `@` ili je prazan ili ima `@@` iza prvog znaka se ne šalje | `SqlTekst.Odluka`. Nakon `SrediPoslije` nepoznat `@@` više nije u tekstu, pa se šalje `null` | isto | `sql-prazno`, `sql-at`, `sql-atat-na-nuli-je-at`, `sql-atat-kasnije`, `sql-salji`, `intrastat-ostao` |
| `Class1.ExecSql` Catch 158–205. `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard` se gutaju. `#OK` / `+OK` brišu grešku. `#close` zatvara. `#FOCUS(` daje fokus | `Lanac.Izvrsi` i `MasterGrid.IzvrsiCitanje`. `spNetGenErrorLog` se ne zove; greška ide u Carpo logger | isto | `focusNeGuta` u `GranulaIzvrsenje`, `sql-greska-ne-puni` |
| `Class1.ExecSql2` 252 i poziv 230. Timeout 100, Catch guta. `dbo.spNetGenErrorLog @errorNumber, @errorDescription, @ngLog` | `GreskaSql`. Na spinu tablica `NetGenErrorLog` i procedura koja ubacuje red (`scripts/spin/stubovi.sql`). Carpo logger i dalje prima istu grešku | isto | `execsql2-poziv`, `execsql2-spin` |
| `SqlTekst.JeZabranaPisanja`. Prva riječ `insert/update/delete/drop/alter/truncate/merge/create`. `sp...` nije zabrana | isto to | isto | `zastita-prva-rijec`, `zastita-zakljucivanje` |

## Broj i datum

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| Prazan broj je `0`, zarez je točka. `Form2` 3601 | `ParametriSvi.Broj`, `Lanac.PrazanBroj` | isto | `param-broj`, `prazan-broj` |
| `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth` | `Ponasanje.DatumPocetni` | isto | `KON-20` |
| `SHOWCHECKBOX` bez `CHECKED` šalje `null`. Inače `yyyyMMdd` | `Ponasanje.DatumParametar`, `ParametriSvi.Datum` | isto | `DOB-02` |
| Tekst: apostrof je `¸¸`, zarez je `~~`, u SQL-u `¸¸` postaje `''` | `ParametriSvi.Tekst`, `Lanac.USql`, `SrediPoslije` | isto | `sredi-poslije-navodnik` |

## Form2, prvih 50

Redom iz `Form2.vb`. `nedostaje` znači da živo tijelo nema Carpo par. Komentirano tijelo nije rupa.

| # | NetGen | Carpo | Status | Test |
|---|---|---|---|---|
| 1 | `MYExnHandler` 129. Tijelo je komentirani `MsgBox` | Nema handlera koji guta iznimku | isto | |
| 2 | `MYThreadHandler` 135. Isto, komentirani `MsgBox` | Smoke i dalje padne na `ThreadExceptionDialog` | isto | |
| 3 | `NapuniRSLang` 139. Prije `Return` čita jezik iz `loginimages` / `MPKase` i zove `JupiterPrevoditelj` | `Prevod` prevodi samo kad je jezik već dan. Upit jezika nema | nedostaje | |
| 4 | `Jezici` 213. Zove se tek iza `Return` u `NapuniRSLang` | Mrtav poziv | isto | |
| 5 | `PronadjiOperatera` 224. Samo ako je `OperateriID` 0 (`Run2` 301) | Host daje operatera. SQL po lozinki se ne šalje | isto | |
| 6 | `Run2` 237 | `frm.Run` / `NgUlaz`, Windows veza | isto | |
| 7 | `Run` 390 | Isti ulaz | isto | |
| 8 | `Run3` 533 | Isti ulaz | isto | |
| 9 | `SendMessage` 610 | Nema Jupiter `hwnd` roditelja | nedostaje | |
| 10 | `SetParent` 614 | Isto | nedostaje | |
| 11 | `GetWindowRect` 618 | Isto | nedostaje | |
| 12 | `SetWindowPos` 622 | Isto | nedostaje | |
| 13 | `NapuniKontroleZaEdit` 659. Combo `Tag` iza `##` postaje `SelectedValue` | `ComboDijalog.OdvojiHash`, `Dijalog.NapuniKontrolu` | isto | `combo-hash` |
| 14 | `ProcitajXML` 705 | `NetGenXmlReader` | isto | |
| 15 | `DazovaIzStore` 2825. Stupci 0–3, id 0 se preskače | `Stablo` | isto | `sta-stablo` |
| 16 | `AddNodes` 2856 | `Stablo.DodajDjecu` | isto | `sta-stablo` |
| 17 | `SrediSQL2` 2876 | `SqlTekst.Sredi2` | isto | `sredi2-temp` |
| 18 | `SrediKontrolu` 2924 | `Raspored` | isto | `MPK-21` |
| 19 | `BF2` 3245. Lijepi HTML | Nema HTML obrasca | nedostaje | |
| 20 | `BF` 3251. HTML forme, zove se iz loga 4182 | Nema | nedostaje | |
| 21 | `Button1_Click` 3367 | `SrediPoslije.ZaSlanje` | isto | `za-slanje-dml` |
| 22 | `Button2_Click` 3400. `OKClick = False`, `Close` | `Dijalog` odustani | isto | `dijalog-ostaje` |
| 23 | `PokupiParametre` 3406 | `Dijalog.SviParametri` | isto | `param-broj` |
| 24 | `GetDXID` 3986. Prazan grid je `0`, inače stupac `id` ili prvi | `DetailGrid.IdReda` | isto | `det-id-prvi` |
| 25 | `SpremiuDBClipboard` 4000. Piše samo ako je `DBCLIPBOARD` `true` | `Rijetke.ClipboardPadStvaraTablicu` je false, pa se ne piše | isto | `MAIL-05` |
| 26 | `Form2_Activated` 4025. `Rafalno(ONACTIVATED)`, jedan prijevod, `bCloseParent` | `Dijalog.OnActivated` radi samo `bCloseParent`. `ONACTIVATED` se ne vrti | nedostaje | |
| 27 | `Form2_Deactivate` 4073. Tijelo je komentar | Nema | isto | |
| 28 | `Form2_FormClosed` 4079. `Rafalno(ONCLOSED)` | Zatvaranje ne vrti `ONCLOSED` | nedostaje | |
| 29 | `Form2_FormClosing` 4093. `ONCLOSING` je komentar. Živo je DB clipboard | Clipboard se ne piše dok atribut nije `true` | isto | |
| 30 | `Form2_KeyDown` 4135 | Nije provučeno kroz Carpo tipke dijaloga | nedostaje | |
| 31 | `NovaGenerickaForma2` 4259 | `DijalogTok` / `OtvoriFormu` | isto | `next-kom` |
| 32 | `DodatnoPostimajKontrolu` 4482 | Djelomično kroz `OnLoad2` i focus | nedostaje | |
| 33 | `RefreshControls` 4556 | `RafalNaredba.RefreshKontrola` i `IzvrsiSql` | isto | `prazan-alert` |
| 34 | `RefreshControlsSaEVALOM` 5217 | `Izraz` | isto | `izraz-ilcalc` |
| 35 | `RefreshGrida` 5284 | `RECALC` / osvježavanje grida na dijalogu nije isti poziv | nedostaje | |
| 36 | `RucniRadNaGriduzaCheckiranjeRedova` 5435 | Nema ručnog checka po loadu | nedostaje | |
| 37 | `RefreshControlSQL` 5460 | `RefreshKontrola` čita SQL kontrole | isto | |
| 38 | `RefreshKontrola` 5470 | `DijalogTok` vrsta `refresh` | isto | `prazan-alert` |
| 39 | `DataGridView1_SelectionChanged` 5623 | Selekcija detaila ide drugim putem (`DetailGrid`) | isto | `sta-detail` |
| 40 | `DataGridView1_RowEnter` 5643 | Nema posebnog `RowEnter` na dijalog gridu | nedostaje | |
| 41 | `ssPermissionProcitaj` 5657 | Upit se ne šalje | nedostaje | |
| 42 | `ssPermissionProvjeri` 5674 | Vidljivost kontrole se ne gasi iz `ssPermission` | nedostaje | |
| 43 | `ssPermissionProvjeriKontrolu` 5723 | Isto | nedostaje | |
| 44 | `ssPermissionProvjeriRafalnuKomandu` 5760 | Komanda se ne gasi po dozvoli | nedostaje | |
| 45 | `Form2_KeyUp` 5788. Ctrl (17) gasi `ControlClicked` | Nema te zastavice | nedostaje | |
| 46 | `Form2_Load` 5794. `NapuniKontroleZaEdit`, `ONLOADED`, dozvole | `ONLOADED` lanac i `##` jesu. Dozvole nisu | nedostaje | `combo-hash` |
| 47 | `Button3_Click` 5800. `NovaGenerickaForma2(Tag)` | CMD ide kroz `GumbKlik` | isto | |
| 48 | `Button4_Click` 5805 | Isto | isto | |
| 49 | `Button5_Click` 5810 | Isto | isto | |
| 50 | `Button6_Click` 5815 | Isto | isto | |

U ovom bloku: 32 isto, 18 nedostaje, 0 razlika. Portano u bloku: `NapuniKontroleZaEdit` (`combo-hash`), uz hijerarhiju, `SETDEFAULTS` i `ExecSql2` iz prethodnog podatkovnog sloja.

## Što je portano

`SrediSQL2`, `SrediSQLPosljeNextForme`, `NadjiVrijednostuPrevParametrima`, `SrediSQLsaKolonamaizGrida`, hijerarhijski parametri, `SETDEFAULTS` clipboard prije master SQL-a i `ExecSql2` / `spNetGenErrorLog`.
