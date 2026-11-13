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

## Form2, 51–100

| # | NetGen | Carpo | Status | Test |
|---|---|---|---|---|
| 51 | `Glbl_Validated` 5820. Prazan tekst stavlja tag `0`. Promjena teksta ide u Form3. `ShowDialog` samo kad redova nije 0 ni 1 (5875–5881) | `Form2Dogadjaj.TrazilicaKorak` i `PokaziTrazilicu` | nedostajalo-portano | `trazilica-dijalog` |
| 52 | `Glbl_Validated2` 5912. `ONLOSTFOCUS` samo ako keydown nije već odradio izlaz | `Form2Dogadjaj.IzlazTrazilice`, `Dijalog` Leave i `ProcessCmdKey` | nedostajalo-portano | `keydown-izlaz` |
| 53 | `PokupiParametreIzTrazilice` 5926. Od stupca 1, `@SEARCH` + ime, zadnje 4 znamenke | `ParametriSvi.IzTrazilice` | isto | `trazilica-search` |
| 54 | `Glbl_TextChanged` 5958. `ONCHANGE` | `TextChanged` | isto | |
| 55 | `Glbl_ValidatedCombo` 5962. `ONLOSTFOCUS` | `Leave` | isto | `DOS-05` |
| 56 | `Glbl_ValidatedMultiDrop` 5968. Vodi `ErrorText`, ne `ONLOSTFOCUS` | `Form2Dogadjaj.MultiDropNaredba`, `Leave` šalje `ERRORTEXT` | nedostajalo-portano | `multidrop-error` |
| 57 | `GlblNumber_Validated` 5977. `ONLOSTFOCUS` | `Leave` | isto | |
| 58 | `Glbl_Gotfocus` 5985. Pamti stari tekst i zove `Glbl_Gotfocus2` | `Enter` boja LightGreen i `SelectAll` | nedostajalo-portano | `fokus-boja` |
| 59 | `Glbl_popup` 5992. Tijelo je komentar | Nema | isto | |
| 60 | `Glbl_KeyDown` 6016–6100. Enter na tražilici validira pa ide dalje, osim ako SQL ili greška ima `focus`. Ctrl+A i Ctrl+C na gridu | `Form2Dogadjaj.KeyDown`, `Dijalog.ProcessCmdKey` | nedostajalo-portano | `keydown-trazilica` |
| 61 | `Glbl_TREE_BeforeSelect` 6103. Boja čvora u bijelo | `Form2Dogadjaj.BojaCvoraPrije` vraća `White` | nedostajalo-portano | `boja-cvora` |
| 62 | `Glbl_Click` 6108. `ONCLICK` kontrole, label i grid | Label `Click` šalje `ONCLICK` ili `TAG` | nedostajalo-portano | `klik-labela` |
| 63 | `GlblCHECK_Click` 6182. `Rafalno(Tag)` | Check i radio `Click` šalju `ONCLICK` ili `TAG` | nedostajalo-portano | `klik-labela` |
| 64 | `Glbl_DoubleClick` 6188–6208. `Val` različit od 0 mijenja zarez u točku, zatim `ONDBLCLICK` | `Form2Dogadjaj.VrijednostDvoklika`, `GridView.DoubleClick` | nedostajalo-portano | `dvoklik-vrijednost` |
| 65 | `Glbl_ClickButton` 6221. Prazan tag ili `help` otvara help, inače `Rafalno(tag)` | `Form2Dogadjaj.HelpArgumenti`. Proces se ne pali u testu | nedostajalo-portano | `help-arg` |
| 66 | `UpaliProgres` 6234. Marquee | `Form2Dogadjaj.Progres.Upali` | nedostajalo-portano | `progres` |
| 67 | `UgasiProgres` 6240 | `Progres.Ugasi` | nedostajalo-portano | `progres` |
| 68 | `Fiskalizacija` 6247 | `FiskalTok`, samo testni CIS | isto | `fiskal-izvrsi-test` |
| 69 | `PoslovniProstor` 6578. Zove COM inačicu, stari Raverus blok je komentar | `ProstorNaredba` prepoznaje naredbu | isto | |
| 70 | `PoslovniProstor_COM` 6618 | Odluka je `ProstorGrana`. SOAP je **R111** | razlika | `prostor-com` |
| 71 | `Rafalno` 6789 | `DijalogTok` i `GumbKlik` | isto | `dijalog-ostaje` |
| 72 | `FiskalizacijaNapojnica` 7527 | `FiskalTok` bira `spMPXMLGetNapojnicaZahtijev` | isto | |
| 73 | `PosaljiZPorukeEEEEEnaServer` 7727 | `PosaljiServer` vrsta poruka | isto | |
| 74 | `PosaljiPorukunaServer` 7750. POST text/xml, timeout 3000 | `PosaljiServer.Post` | isto | |
| 75 | `PosaljiZakljuckEEEEEnaServer` 7804 | `PosaljiServer` vrsta zakljucak | isto | |
| 76 | `PosaljiZakljucaknaServer` 7845 | Isti POST | isto | |
| 77 | `SpremiResponse` 7900–7906. Piše `PosaljiZakljucaknaServerResponse.txt` | `Form2Dogadjaj.SpremiOdgovor` | nedostajalo-portano | `odgovor-datoteka` |
| 78 | `SpremiRacunResponse` 7909–7915. Piše `PosaljiRacunnaServerResponse.txt` | Isti zapis, druga datoteka | nedostajalo-portano | `odgovor-datoteka` |
| 79 | `ProvjeraDostupnosti` 7919. `WebClient` na authority | `Form2Dogadjaj.Dostupan`, test daje `Probe` | nedostajalo-portano | `dostupnost` |
| 80 | `PosaljiRacunEEEEEnaServer` 7936 | `PosaljiServer` vrsta racun | isto | |
| 81 | `PosaljiRacunnaServer` 7974 | `PosaljiServer.UrlRacuna` i POST | isto | |
| 82 | `PosaljiSamoJIRServer` 8097–8167. POST `<doc></doc>`, `a=2`. Prazan JIR i vrijeme su `null`. Produkcijski CIS se ne zove | `Form2Dogadjaj.UrlJir` | nedostajalo-portano | `jir-test` |
| 83 | `RecalcGrid` 8219 | `RecalcGrid.Izracunaj` | isto | `recalc-red` |
| 84 | `RecalcGridRow` 8264 | `RecalcGrid.IzracunajRed` | isto | `recalc-red` |
| 85 | `EvaluateSTR` 8286 | `Izraz` | isto | `izraz-ilcalc` |
| 86 | `ForEachRow` 8310 | `ForeachNaredba` | isto | `foreach-podloga` |
| 87 | `ForEachSelected` 8337 | Isto | isto | |
| 88 | `ForEachBlueRow` 8363 | Isto | isto | |
| 89 | `MultiSelectedPrintPriprema` 8404 | `DijalogTok` vrsta `priprema` | isto | |
| 90 | `SrediSQLsaKolonamaizGrida` 8440 | `SrediPoslije.Kolone` | isto | `kolone-grid` |
| 91 | `NadjiVrijednostuGridu` 8459 | Isti poziv, vrijednost daje pozivatelj | isto | `kolone-grid` |
| 92 | `Glbl_ComboClick` 8497. `ONCLICK` | `LookUpEdit.EditValueChanged` | isto | |
| 93 | `Timer1_Tick` 8510–8514. Nakon tražilice fokus, pa se timer gasi | `Form2Dogadjaj.TimerFokusa` | nedostajalo-portano | `timer-fokus` |
| 94 | `Glbl_DateTime_ValueChanged` 8518. `ONVALUECHANGED` | `DateEdit.EditValueChanged` | isto | |
| 95 | `Glbl_EditingCell` 8532. Pamti staru vrijednost prije editiranja | `ShowingEditor` pamti `VrijednostDvoklika` | nedostajalo-portano | `izmjena-celije` |
| 96 | `Glbl_CellEdited` 8616. SQL nakon izmjene ćelije | `CellValueChanged` šalje `ONCELLEDITED`, greška vraća staru | nedostajalo-portano | `izmjena-celije` |
| 97 | `Glbl_GridMouseUp` 8704–8731. Desni klik uz Ctrl je stupci, inače `POPUP` kao `natpis:naredba` | `Form2Dogadjaj.DesniKlik` i `Izbornik` | nedostajalo-portano | `desni-klik` |
| 98 | `Glbl_TreeMouseUp` 8859. Isti desni klik na stablu | Isti razrez `POPUP` | nedostajalo-portano | `desni-klik` |
| 99 | `ToolStripMenuItem1_Click` 8897. Tag stavke je naredba iza dvotočke | `Izbornik` vraća `natpis\|naredba` | nedostajalo-portano | `desni-klik` |
| 100 | `IspisNovi` 9006. Janus `GridExPrinting`, ne PrintLink | Ispis ide kroz `PageHeaderFooter` | razlika | `ispis-zaglavlje` |

U ovom bloku nakon dopune: 26 isto, 0 još nedostaje, 22 nedostajalo-portano, 2 razlike (`IspisNovi`, `PoslovniProstor_COM` / R111).

### Nedostajalo, sada portano

Form3 pri promjeni teksta (`trazilica-dijalog`, `Form2.vb` 5820–5881), `KeyDown` (`keydown-trazilica`, 6016–6100), dvoklik (`dvoklik-vrijednost`, 6198–6204), desni klik (`desni-klik`, 8704–8731 i 8897), timer fokusa (`timer-fokus`, 8510–8514), datoteke odgovora (`odgovor-datoteka`, 7900–7915), `PosaljiSamoJIRServer` samo ako URL nije produkcijski CIS (`jir-test`, 8097–8167).

### Ostalo u ovom bloku

`Glbl_Gotfocus2` šalje `GOTFOCUS` na `Enter`, boja je LightGreen i radi se `SelectAll`. `IspisNovi` ostaje razlika: Janus `GridExPrinting` se ne vraća. `PoslovniProstor_COM` slanje ostaje R111 dok `dll-traga` na sql-ai ne nađe `Raverus.FiskalizacijaDEV.COM.dll` ili `SLOFISKAL.dll`. Popis za donijeti ispisuje taj test.

## Form2 od `Glbl_Gotfocus2` do kraja

`Form1` je na drugoj grani i ovdje se ne dira.

| NetGen | Carpo | Status |
|---|---|---|
| `Glbl_Gotfocus2` 9458. Zelena pozadina, `strGotFocus`, `SelectAll`. NFC timer je komentar | `Enter` šalje `GOTFOCUS`, LightGreen i `SelectAll` | nedostajalo-portano |
| `Glbl_Leave` 9433. Prazna tražilica tag `0` i `ONLOSTFOCUS` | Već u `Leave` tražilice | isto |
| `Glbl_LeaveTrazilica` 9454. Žuta pozadina | `Leave` tražilice stavlja LightYellow | nedostajalo-portano |
| `Rafalna_Priprema` 9168. `#ISEMPTY` | `IspisMotora.Zaglavlje` | isto |
| `IspisGRIDREPX` 9041, `ChartGRID` 9062, `PivotGRID` 9085 | `EkstenzijaGrid`. `PrintDocumentREPX` radi. Chart i pivot su OPEN dok nema v12.1 DLL-ova | isto |
| `Ispis` 9108 | `PrintBridge` | isto |
| `SaveGridLayout` 9205, `DeleteGridLayout` 9232, `LoadLayout` 9245, `GetLayoutDirectory` 9260 | `ImeRasporeda`, `MapaRasporeda`, `SpremiRaspored`. Janus `.gxl` binarni format nije isti | nedostajalo-portano |
| `AutoCompleteCombo_KeyUp` 9276 i `Leave` 9397 | `PreskociTipku`, `NadjiStavku`, `Dopuna` | nedostajalo-portano |
| `ContextMenuStrip7_*` 9494–9539 | `StupacPoNatpisu`, `Okreni`, `OtkaziZatvaranje` | nedostajalo-portano |
| `DajPokaziHelp` 9539 | `HelpArgumenti`. `JupiterHelp.exe` se ne pokreće u testu | nedostajalo-portano |
| `SerialPort1_*` 9577–9611 | `LazniPort`, `PostimajPort`, `PisiPort`. Test ne otvara hardver | nedostajalo-portano |
| `glblGRID_FormattingRow` 9611 | Format stupca je u `MasterGrid` | isto |
| `Timer2_Tick` 9687. `TIMERSQL` | `Timer2ImaSql` | nedostajalo-portano |
| `Form2_PaddingChanged` 9694. Prazno tijelo | Nema | isto |
| `Timer3_Tick` 9698. Maximize i čekirani multidrop | `Maksimiziraj`, `CekiranId` | nedostajalo-portano |
| `Timer4_Tick` 9744. NFC, komentirano da se ne koristi | Ne radi se | isto |

## Što je portano

`SrediSQL2`, `SrediSQLPosljeNextForme`, `NadjiVrijednostuPrevParametrima`, `SrediSQLsaKolonamaizGrida`, hijerarhijski parametri, `SETDEFAULTS` clipboard prije master SQL-a i `ExecSql2` / `spNetGenErrorLog`.

## Class1

`Form1` se ne dira. 24 metode u `Class1.vb`. Komentirane (`IdiNaWebServicePoBrojBodova`, `AzurirajBodove`, `IdiNaWebServiceIAzurirajBodove`, `DeleteGridLayoutT`) nisu u broju.

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `ExecSql` 85 | `SqlTekst.Odluka`, `MasterGrid.IzvrsiCitanje` | isto | `sql-salji` |
| `ExecSql2` 252 | `GreskaSql` | isto | `execsql2-poziv` |
| `SrediSQLPosljeNextForme` 740 | `SrediPoslije.Zamijeni` | isto | `sredi-poslije-roba` |
| `NadjiVrijednostuPrevParametrima` 856 | `SrediPoslije.Nadji` | isto | `sredi-poslije-cb` |
| `CreateGridColumns` 982 | `MasterGrid.IzPodatka` | isto | `bla-sirina` |
| `FormatirajGrid` 271 | `Class1Pravila.SakrijId`, `Sirina` | nedostajalo-portano | `format-grid` |
| `AddConditionalFormatting` 1369 | `Class1Pravila.FormatUvjeta` | nedostajalo-portano | `format-uvjet` |
| `DajInstall` 1450 | `Class1Pravila.InstallVecIma` | nedostajalo-portano | `install-vec` |
| `GlipiMS` 1605 | `Class1Pravila.Glipi` | nedostajalo-portano | `glipi` |
| `GetDXID` 1960 | `Class1Pravila.DxId` | nedostajalo-portano | `dx-id` |
| `BlagajnaEnableZaMlinar` 1975 | `Class1Pravila.BlagajnaUpit` | nedostajalo-portano | `blagajna-enable` |
| `UpaliProgres` / `UgasiProgres` 723 | `Form2Dogadjaj.Progres` | nedostajalo-portano | `progres` |
| `NapuniCBLog` / `PokaziCBLog` | Carpo logger, ne CBLog | razlika | |
| `PrintRpt` 379, `IspisiMe7` 284, `IspisiMePrekoSpinPrintinga` 291 | `SpinIspis` zove `Spin.Printing.v26` `Print`/`PrintMulti` kao `Carpo.UI.PrintDialog`. Test piše PDF, ne zove dijalog | nedostajalo-portano | `spin-ispis` |
| `NapuniDXGrid` 1699, `NapuniDXGridLATE` 1763 | `SakrijCaptionId`, `SirinaNatpisa`, `SmijeEdit`. `IndexOf` &gt; 0. Stupac `rcb555` | nedostajalo-portano | `dx-grid` |
| `CustomFormatKolonaDX` 1845, `UkljuciSum` 1937 | `KljucKolone`, `SirinaXml`, `Natpis`, `Zbroj` | nedostajalo-portano | `dx-format` |
| `New` 1433, `Finalize` 1429 | Nema | isto | |

U Class1 bloku, 24 metode: 7 isto (`ExecSql`, `ExecSql2`, `SrediSQLPosljeNextForme`, `NadjiVrijednostuPrevParametrima`, `CreateGridColumns`, `New`, `Finalize`), 15 nedostajalo-portano, 2 razlike (`NapuniCBLog` i `PokaziCBLog` idu u Carpo logger), 0 još nedostaje. Ispis je `Spin.Printing.v26`.

## Ostali moduli, prvi blok

`Form1` se ne dira. `Form3.Trazilica` SQL je već `Ponasanje.TrazilicaSql` (`trazilica` test).

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `SpinAppDataFolderDotNet` 48, `JupiterAppDataFolderDotNet` 59, `JupiterGridSettingsFolderDotNet` 70 | `OstaliModuli.SpinMapa`, `JupiterMapa`, `GridSettingsMapa`. Stvara se roditelj, ne mapa Grid Settings | nedostajalo-portano | `mape-spin` |
| `PutanjaSlike` 136 | `OstaliModuli.PutanjaSlike` | nedostajalo-portano | `mape-spin` |
| `JupiterLog.Sredi` 122, `SakrijVrijednost` 141 | `OstaliModuli.SrediLog`. `Assembly.Load("JupiterLogger")` se ne zove | nedostajalo-portano | `log-sredi` |
| `Encrypt` / `Decrypt` / `KeyToBytes` | `OstaliModuli.Kriptiraj`, `Dekriptiraj`, `Kljuc` | nedostajalo-portano | `kript` |
| `GetLast` 16 | `OstaliModuli.ZadnjiNamespace`. `MessageBox` se ne pali | nedostajalo-portano | `zadnji-form` |
| `Translate_OLD` 293, `ToCiril2` 474 | `PrevediStaro`, `UCirilicu` | nedostajalo-portano | `prevod` |
| `Translate` 266 | Tekst ostaje. `JupiterPrevoditelj` se ne učitava kad jezik nije `hrv` | razlika | `prevod` |
| `GetServerPath` 214 | `IniPutanja`, `RemoteDll` | nedostajalo-portano | `server-put` |

U ovom bloku: 7 nedostajalo-portano, 1 razlika (`Translate` bez `JupiterPrevoditelj`). `JupiterLogger.dll` ostaje van: redact ide u Carpo, učitavanje DLL-a nema.
