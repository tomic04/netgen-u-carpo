# Pregled funkcija NetGen / Carpo

Prvi prolaz su funkcije koje obrađuju podatke: zamjena `@@`, `PokupiParametre`, SQL, validacija i format broja i datuma. Izvor je `netgen/`. U `netgen/*.vb` (bez designer datoteka) ima 476 `Sub`/`Function`. Ovaj dokument pokriva podatkovni sloj. Ostali moduli nisu u ovom prolazu.

Status: `isto` prati NetGen, `razlika` je R-broj koji ostaje, `nedostaje` još nema porta. `portano` i `nedostajalo-portano` smiju stajati samo uz test iz `tests/ui/katalog.txt`: kategorija UI, ime granule, otvaranje kroz TestShell. Isto pravilo vrijedi za `docs/pregled-funkcija-form1.md`.

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
| 9 | `SendMessage` 610. Zatvaranje prve forme šalje `&H7D0` (4113) | `frm.Ugradi`: `FormClosed` šalje 0x7D0 na `mainHandle` (ULZ-04) | isto | `ulz04-frm-jupiter` |
| 10 | `SetParent` 614. Prva forma ide u `clientHandle` (994) | `frm.Ugradi` (ULZ-04); podloga kroz `Base.Start` (ULZ-01) | isto | `ulz04-frm-jupiter` |
| 11 | `GetWindowRect` 618. Samo deklaracija, poziva nema | Nema | isto | |
| 12 | `SetWindowPos` 622. Samo deklaracija, poziva nema | Nema | isto | |
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
| 26 | `Form2_Activated` 4025. `Rafalno(ONACTIVATED)`, jedan prijevod, `bCloseParent` | `Dijalog.OnActivated`: `bCloseParent`, pa `ONACTIVATED` kroz lanac | isto | |
| 27 | `Form2_Deactivate` 4073. Tijelo je komentar | Nema | isto | |
| 28 | `Form2_FormClosed` 4079. `Rafalno(ONCLOSED)` | `Dijalog.OnFormClosed` vrti `ONCLOSED` | isto | |
| 29 | `Form2_FormClosing` 4093. `ONCLOSING` je komentar. Živo je DB clipboard | Clipboard se ne piše dok atribut nije `true` | isto | |
| 30 | `Form2_KeyDown` 4135. F1, Escape/`ONESCKEY`, Ctrl+F11/F6, F1-F10 i `*` | `Dijalog.ProcessCmdKey`. Ctrl+F11/F6 pišu u Carpo log (R120) | isto | `hot01-f2` |
| 31 | `NovaGenerickaForma2` 4259 | `DijalogTok` / `OtvoriFormu` | isto | `next-kom` |
| 32 | `DodatnoPostimajKontrolu` 4482 | `Dijalog.DodatnoPostavi`, visible/enable ponovo pitaju ssPermission | isto | `doz02-naknadno` |
| 33 | `RefreshControls` 4556 | `RafalNaredba.RefreshKontrola` i `IzvrsiSql` | isto | `prazan-alert` |
| 34 | `RefreshControlsSaEVALOM` 5217 | `Izraz` | isto | `izraz-ilcalc` |
| 35 | `RefreshGrida` 5284 | `Dijalog.OsvjeziGrid`: povratak na isti red, `move*`, `AFTERREFRESH` | isto | |
| 36 | `RucniRadNaGriduzaCheckiranjeRedova` 5435 | `DijalogGrid.OznaciIzStupca` (`selectedid`) | isto | `chk01` |
| 37 | `RefreshControlSQL` 5460 | `RefreshKontrola` čita SQL kontrole | isto | |
| 38 | `RefreshKontrola` 5470 | `DijalogTok` vrsta `refresh` | isto | `prazan-alert` |
| 39 | `DataGridView1_SelectionChanged` 5623 | Selekcija detaila ide drugim putem (`DetailGrid`) | isto | `sta-detail` |
| 40 | `DataGridView1_RowEnter` 5643. `AddHandler` je komentar (1570, 1782) | Nema | isto | |
| 41 | `ssPermissionProcitaj` 5657. `PERMISIO` na formi, objekt xml.forma | `Dozvole.SqlForme`, `DijalogDozvole.UcitajDozvole` | isto | `doz02-sql`, `doz02-citanje` |
| 42 | `ssPermissionProvjeri` 5674 | `Dozvole.Kontrola`, poslije `ONLOADED`; sve kontrole, ne samo izravna djeca | isto | `doz02-kontrola`, `doz02-dijalog` |
| 43 | `ssPermissionProvjeriKontrolu` 5723 | `Dozvole.KontrolaNaknadno` | isto | `doz02-naknadno` |
| 44 | `ssPermissionProvjeriRafalnuKomandu` 5760 | `Dozvole.SmijeKomandu`: forma prije otvaranja i stalne stavke desnog klika | isto | `doz02-forma`, `doz02-izbornik` |
| 45 | `Form2_KeyUp` 5788. Ctrl (17) gasi `ControlClicked` | Zastavica je `Control.ModifierKeys`: s Ctrl red ne šalje `ONCLICK`, Ctrl + desni klik otvori Carpo izbor stupaca | isto | |
| 46 | `Form2_Load` 5794. `NapuniKontroleZaEdit`, `ONLOADED`, dozvole | `Dijalog.OnLoad`: `ONLOADED`, pa dozvole | isto | `combo-hash`, `doz02-dijalog` |
| 47 | `Button3_Click` 5800. `NovaGenerickaForma2(Tag)` | CMD ide kroz `GumbKlik` | isto | |
| 48 | `Button4_Click` 5805 | Isto | isto | |
| 49 | `Button5_Click` 5810 | Isto | isto | |
| 50 | `Button6_Click` 5815 | Isto | isto | |

U ovom bloku: 47 isto, 3 nedostaje, 0 razlika. `NapuniRSLang` čeka PRV-01 (odluka korisnika); `BF` i `BF2` su HTML obrazac, kojeg Carpo nema (Ctrl+F6 piše parametre u Carpo log). Portano u bloku: `NapuniKontroleZaEdit` (`combo-hash`), uz hijerarhiju, `SETDEFAULTS` i `ExecSql2` iz prethodnog podatkovnog sloja.

## Form2, 51–100

| # | NetGen | Carpo | Status | Test |
|---|---|---|---|---|
| 51 | `Glbl_Validated` 5820. Prazan tekst stavlja tag `0`. Promjena teksta ide u Form3. `ShowDialog` samo kad redova nije 0 ni 1 (5875–5881) | `Dijalog.PotvrdiTrazilicu`, prozor tražilice `TrazilicaProzor` (Form3) kad redova nije 0 ni 1 | isto | `trz01-*` |
| 52 | `Glbl_Validated2` 5912. `ONLOSTFOCUS` samo ako keydown nije već odradio izlaz | `Form2Dogadjaj.IzlazTrazilice`: Enter odradi izlaz, `Leave` ga ne ponavlja | isto | `trazilica-preskok` |
| 53 | `PokupiParametreIzTrazilice` 5926. Od stupca 1, `@SEARCH` + ime, zadnje 4 znamenke | `ParametriSvi.IzTrazilice` | isto | `trazilica-search` |
| 54 | `Glbl_TextChanged` 5958. `ONCHANGE` | `TextChanged` | isto | |
| 55 | `Glbl_ValidatedCombo` 5962. `ONLOSTFOCUS` | `Leave` | isto | `DOS-05` |
| 56 | `Glbl_ValidatedMultiDrop` 5968. Vodi `ErrorText`, ne `ONLOSTFOCUS` | `Form2Dogadjaj.MultiDropNaredba`, `Leave` šalje `ERRORTEXT` | isto | `mdr01-multidrop` |
| 57 | `GlblNumber_Validated` 5977. `ONLOSTFOCUS` | `Leave` | isto | |
| 58 | `Glbl_Gotfocus` 5985. Pamti stari tekst i zove `Glbl_Gotfocus2` | `Enter` na TextEdit: LightGreen i SelectAll | isto |  |
| 59 | `Glbl_popup` 5992. Tijelo je komentar | Nema | isto | |
| 60 | `Glbl_KeyDown` 6016–6100. Enter na tražilici validira pa ide dalje, osim ako SQL ili greška ima `focus`. Ctrl+A i Ctrl+C na gridu | `Dijalog.ProcessCmdKey` zove `Form2Dogadjaj.KeyDown`. Ctrl+A i Ctrl+C na gridu radi DevExpress sam (MultiSelect); u načinu s kvačicama Ctrl+A i označi redove, jer su plavi i označeni redovi u DevExpressu isti | isto | `hot01-f2` |
| 61 | `Glbl_TREE_BeforeSelect` 6103. Boja čvora u bijelo | DevExpress `TreeList` sam crta odabrani čvor | isto |  |
| 62 | `Glbl_Click` 6108. `ONCLICK` kontrole, label i grid | Label `Click`; grid `FocusedRowChanged` i `Click` s preskokom (ZanemariClick), uz Ctrl bez `ONCLICK` (CTL-01) | isto |  |
| 63 | `GlblCHECK_Click` 6182. `Rafalno(Tag)` | Check i radio `Click` šalju `ONCLICK` ili `TAG` | isto |  |
| 64 | `Glbl_DoubleClick` 6188–6208. `Val` različit od 0 mijenja zarez u točku, zatim `ONDBLCLICK` | `Form2Dogadjaj.VrijednostDvoklika`, `GridView.DoubleClick`, `@@dblclickedcolumn` | isto |  |
| 65 | `Glbl_ClickButton` 6221. Prazan tag ili `help` otvara help, inače `Rafalno(tag)` | `Dijalog.GumbKlik` → `PokaziPomoc` (HLP-02), `JupiterPomoc.Argumenti` | isto | `hlp02-argumenti` |
| 66 | `UpaliProgres` 6234. Marquee | `RafalNaredba.Progres` pamti zastavicu, trake nema. `PROGRESSON` je 0 u korpusu; podloga i dijalog imaju Carpo izgled | razlika | `timer-progres` |
| 67 | `UgasiProgres` 6240 | `RafalNaredba.Progres` | razlika | `timer-progres` |
| 68 | `Fiskalizacija` 6247 | `FiskalTok`, samo testni CIS | isto | `fiskal-izvrsi-test` |
| 69 | `PoslovniProstor` 6578. Zove COM inačicu, stari Raverus blok je komentar | `ProstorNaredba` prepoznaje naredbu | isto | |
| 70 | `PoslovniProstor_COM` 6618 | Odluka je `ProstorGrana`. SOAP je **R111** | razlika | `prostor-com` |
| 71 | `Rafalno` 6789 | `DijalogTok` i `GumbKlik` | isto | `dijalog-ostaje` |
| 72 | `FiskalizacijaNapojnica` 7527 | `FiskalTok` bira `spMPXMLGetNapojnicaZahtijev` | isto | |
| 73 | `PosaljiZPorukeEEEEEnaServer` 7727 | `PosaljiServer` vrsta poruka | isto | |
| 74 | `PosaljiPorukunaServer` 7750. POST text/xml, timeout 3000 | `PosaljiServer.Post` | isto | |
| 75 | `PosaljiZakljuckEEEEEnaServer` 7804 | `PosaljiServer` vrsta zakljucak | isto | |
| 76 | `PosaljiZakljucaknaServer` 7845 | Isti POST | isto | |
| 77 | `SpremiResponse` 7900–7906. Piše `PosaljiZakljucaknaServerResponse.txt` | Dio `:FISKALIZACIJA` (M7, R111): čeka odluku | nedostaje |  |
| 78 | `SpremiRacunResponse` 7909–7915. Piše `PosaljiRacunnaServerResponse.txt` | Dio `:FISKALIZACIJA` (M7, R111): čeka odluku | nedostaje |  |
| 79 | `ProvjeraDostupnosti` 7919. `WebClient` na authority | Dio `:FISKALIZACIJA` (M7, R111): čeka odluku | nedostaje |  |
| 80 | `PosaljiRacunEEEEEnaServer` 7936 | `PosaljiServer` vrsta racun | isto | |
| 81 | `PosaljiRacunnaServer` 7974 | `PosaljiServer.UrlRacuna` i POST | isto | |
| 82 | `PosaljiSamoJIRServer` 8097–8167. POST `<doc></doc>`, `a=2`. Prazan JIR i vrijeme su `null`. Produkcijski CIS se ne zove | Dio `:FISKALIZACIJA` (M7, R111): čeka odluku | nedostaje |  |
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
| 93 | `Timer1_Tick` 8510–8514. Nakon tražilice fokus, pa se timer gasi | FOCUS forme u `Dijalog.OnShown` (`Fokusiraj`) | isto | `fokus-zadnji` |
| 94 | `Glbl_DateTime_ValueChanged` 8518. `ONVALUECHANGED` | `DateEdit.EditValueChanged` | isto | |
| 95 | `Glbl_EditingCell` 8532. Pamti staru vrijednost prije editiranja | `ShowingEditor` pamti `VrijednostDvoklika`; `BEFORECELLEDITED` može otkazati | isto | `edt01-uredjiv` |
| 96 | `Glbl_CellEdited` 8616. SQL nakon izmjene ćelije | `CellValueChanged` šalje `ONCELLEDITED`, greška vraća staru | isto | `edt02-oncelledited`, `edt03-greska-vraca` |
| 97 | `Glbl_GridMouseUp` 8704–8731. Desni klik uz Ctrl je stupci, inače `POPUP` kao `natpis:naredba` | `GridIzbornik.Vezi`: `POPUP` kao `natpis:naredba` i stalne stavke; Ctrl + desni klik je Carpo izbor stupaca (CTL-01) | isto |  |
| 98 | `Glbl_TreeMouseUp` 8859. Isti desni klik na stablu | `DijalogStablo.VeziStablo`: `POPUP` stabla (TRE-03) | isto | `tre03-*` |
| 99 | `ToolStripMenuItem1_Click` 8897. Tag stavke je naredba iza dvotočke | `Form2Dogadjaj.Izbornik` vraća `natpis\|naredba`; `~` je lanac, inače `NovaGenerickaForma` | isto |  |
| 100 | `IspisNovi` 9006. Janus `GridExPrinting`, ne PrintLink | Ispis ide kroz `PageHeaderFooter` | razlika | `ispis-zaglavlje` |

Status `portano` i `nedostajalo-portano` smije stajati samo uz test iz `tests/ui/katalog.txt` (kategorija UI, ime granule, TestShell). `dijalog-ucinak` slaže prazan dijalog i zato se ne računa. U ovom bloku (stanje 14. 11. 2026., nakon CTL-01, DOZ-02 i HLP-02): 42 isto, 4 razlike (`IspisNovi`, R111, traka napretka), 4 nedostaje (odgovor, dostupnost i JIR fiskalizacije, čekaju odluku `:FISKALIZACIJA`).

### Nedostajalo, sada portano

Stari popis testova iz ovog odlomka (`keydown-trazilica`, `desni-klik`, ...) maknut je s lažnim portovima (e8fd9e2). Stvarno stanje je u tablici iznad: tražilica `trz01-*`, uređivanje `edt0*`, pomoć `hlp02-*`, desni klik kroz `GridIzbornik` i `DijalogStablo`, Ctrl na gridu CTL-01.

### Ostalo u ovom bloku

`Glbl_Gotfocus2` šalje `GOTFOCUS` na `Enter`, boja je LightGreen i radi se `SelectAll`. `IspisNovi` ostaje razlika: Janus `GridExPrinting` se ne vraća. `PoslovniProstor_COM` slanje ostaje R111 dok `dll-traga` na sql-ai ne nađe `Raverus.FiskalizacijaDEV.COM.dll` ili `SLOFISKAL.dll`. Popis za donijeti ispisuje taj test.

## Form2 od `Glbl_Gotfocus2` do kraja

`Form1` je na drugoj grani i ovdje se ne dira.

| NetGen | Carpo | Status |
|---|---|---|
| `Glbl_Gotfocus2` 9458. Zelena pozadina, `strGotFocus`, `SelectAll`. NFC timer je komentar | `Enter`: LightGreen i SelectAll, `GOTFOCUS` kroz lanac | isto |
| `Glbl_Leave` 9433. Prazna tražilica tag `0` i `ONLOSTFOCUS` | Već u `Leave` tražilice | isto |
| `Glbl_LeaveTrazilica` 9454. Žuta pozadina | `Leave` tražilice stavlja LightYellow i prazan tag `0` | isto |
| `Rafalna_Priprema` 9168. `#ISEMPTY` | `IspisMotora.Zaglavlje` | isto |
| `IspisGRIDREPX` 9041, `ChartGRID` 9062, `PivotGRID` 9085 | `EkstenzijaGrid`. `PrintDocumentREPX` radi. Chart i pivot su OPEN dok nema v12.1 DLL-ova | isto |
| `Ispis` 9108 | `PrintBridge` | isto |
| `SaveGridLayout` 9205, `DeleteGridLayout` 9232, `LoadLayout` 9245, `GetLayoutDirectory` 9260 | `GridRaspored` (GRL-01): ista imena datoteka s `.xml`; Janus `.gxl` se ne čita | isto |
| `AutoCompleteCombo_KeyUp` 9276 i `Leave` 9397 | Carpo `LookUpEdit` (Standard, ImmediatePopup) dopunjuje sam; nepoznat tekst na izlazu ne skače na prvu stavku nego ostaje prošla vrijednost (50-50 → Carpo) | razlika |
| `ContextMenuStrip7_*` 9494–9539 | Ctrl + desni klik otvori Carpo izbor stupaca (`ShowCustomization`, CTL-01) | razlika |
| `DajPokaziHelp` 9539 | `DijalogPomoc.PokaziPomoc` i `JupiterPomoc` (HLP-02); bez koda se ne pokreće | isto |
| `SerialPort1_*` 9577–9611 | Nitko ih ne zove (ni `DajImenaPotova`, `Postimaj`, `Pisi`) | isto |
| `glblGRID_FormattingRow` 9611 | Format stupca je u `MasterGrid` | isto |
| `Timer2_Tick` 9687. `TIMERSQL` | Timer2 se pali samo s `TIMERON`, kojeg u korpusu nema; `TIMERON` je u `RafalNaredba` | isto |
| `Form2_PaddingChanged` 9694. Prazno tijelo | Nema | isto |
| `Timer3_Tick` 9698. Maximize i čekirani multidrop | WINDOWSTATE 2 (FRM-02), označeni multidrop iz taga (MDR-01), FOCUS u `OnShown` | isto |
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
| `FormatirajGrid` 271 | Nitko je ne zove | isto |  |
| `AddConditionalFormatting` 1369 | `GridIzgled.FormatReda` (FMT-01) na masteru, detailu, gridu dijaloga i tražilici | isto | `fmt01-red`, `fmt01-uvjeti` |
| `DajInstall` 1450 | Zovu je samo komentirane metode | isto |  |
| `GlipiMS` 1605 | Zovu je samo komentirane metode | isto |  |
| `GetDXID` 1960 | Carpo master i detail su uvijek DevExpress: `DetailGrid.IdReda`, `Podloga.SpojiId` | isto | `det-id-prvi` |
| `BlagajnaEnableZaMlinar` 1975 | `BlagajnaNaredba` (BLAGAJNAENABLE u lancu podloge i dijaloga) | isto | `open-blagajna-ispis` |
| `UpaliProgres` / `UgasiProgres` 723 | Podloga nema traku napretka (Carpo izgled) | razlika |  |
| `NapuniCBLog` / `PokaziCBLog` | Carpo logger, ne CBLog | razlika | |
| `PrintRpt` 379, `IspisiMe7` 284, `IspisiMePrekoSpinPrintinga` 291 | `PrintBridge` → `Spin.Printing.v26`. `rs:`/`rsgen`/`.clsReport` su stari VB6 PrintDlg COM: ukinuto (ISP-05, R123) | isto | `isp07-printgrid-ime` |
| `NapuniDXGrid` 1699, `NapuniDXGridLATE` 1763 | Isti `GridControl` put kao Janus grid (`NgGrana.DxGrid`) | isto |  |
| `CustomFormatKolonaDX` 1845, `UkljuciSum` 1937 | `GridDefinicija` i `GridIzgled.Ukupno` | isto | `dgr01-*` |
| `New` 1433, `Finalize` 1429 | Nema | isto | |

U Class1 bloku (stanje 14. 11. 2026.): sve žive metode imaju Carpo put ili ih nitko ne zove; 2 razlike (CBLog ide u Carpo logger, traka napretka). `formatid` (FMT-01) je bio jedina prava rupa. `PrintBridge` zove `Spin.Printing.v26`; `rs:`/`rsgen`/`.clsReport` su stari VB6 ispis, ukinut (ISP-05, R123).

## Ostali moduli, prvi blok

`Form1` se ne dira. `Form3.Trazilica` SQL je već `Ponasanje.TrazilicaSql` (`trazilica` test).

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `SpinAppDataFolderDotNet` 48, `JupiterAppDataFolderDotNet` 59, `JupiterGridSettingsFolderDotNet` 70 | `GridRaspored.Mapa` (GRL-01) | isto | `grl01-*` |
| `PutanjaSlike` 136 | Nitko je ne zove izvan modula | isto |  |
| `JupiterLog.Sredi` 122, `SakrijVrijednost` 141 | `NgDnevnik.Sakrij`: `pwd=`, `password=` i polja s PASSWORDCHAR (R120) | isto | `log01-spojni`, `log01-*` |
| `Encrypt` / `Decrypt` / `KeyToBytes` | `XmlIzvor.Dekriptiraj` (Decrypt, KeyToBytes) za XML iz baze `netgenxml` (M5, XML-01); `Encrypt` nitko ne zove | isto | `xml01-dekript`, `xml01-baza` |  |
| `GetLast` 16 | Nitko je ne zove | isto |  |
| `Translate_OLD` 293, `ToCiril2` 474 | Prijevod: PRV-01, čeka odluku | nedostaje |  |
| `Translate` 266 | Tekst ostaje. `JupiterPrevoditelj` se ne učitava kad jezik nije `hrv` | razlika | `prevod` |
| `GetServerPath` 214 | Dio ispisa `rs:` kroz Form6: stari VB6 ispis je ukinut (ISP-05, R123) | razlika | `isp05-ukinut` |

U ovom bloku (stanje 14. 11. 2026.): 5 isto (mape kroz `GridRaspored`, redact kroz `NgDnevnik.Sakrij`, DES kroz `XmlIzvor`), 1 nedostaje (stari prijevod je PRV-01), `GetServerPath` je ukinuti VB6 ispis `rs:` (ISP-05, R123); DES za `netgenxml` je M5 (XML-01), 1 razlika (`Translate`).
