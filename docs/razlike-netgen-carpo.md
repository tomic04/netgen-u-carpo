# Razlike NetGen i Carpo

Jedini registar. Razlika se upisuje čim se vidi, u istom PR-u, ne na kraju elementa.

## Checklist svakog PR-a

- Nova ili promijenjena razlika dobije red u tablici ispod.
- Red ima: što radi NetGen (datoteka i linija), što radi Carpo (datoteka i linija), odluku, test koji je zaključava, broj PR-a.
- Odluka je jedna od: **NetGen logika**, **Carpo mehanika**, **namjerno odstupanje**, **otvoreno**.
- Ako testa još nema, u stupcu Test piše `nema`, a odluka ostaje **otvoreno** dok ga PR ne doda.

NetGen izvor je `netgen/`. Carpo izvor je `Devex_26/`. Carpo.NetGen je sloj koji drži odluku.

## Stablo

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| Punjenje čvorova | Stupci po poziciji 0 id, 1 roditelj, 2 natpis, 3 tag. `Form1.vb` 4446–4498, `SqlDataAdapter.Fill` | `JTree` veže `key` / `parentfield` / `valuefield` po imenu. `ComponentTree.cs` 49, 770–791. Upozorenje ako imena nema | **NetGen logika.** `Stablo.IzReda` čita samo ordinale. Carpo drvo je crtež već složenih čvorova | `stablo-pozicija-ne-ime`; na spinu usporedba čvor po čvor (`SPIN-STABLO`) | #13 |
| Samo podstablo prvog reda | Korijen je `Rows(0)`. `Form1.vb` 4492–4524 | Carpo nacrta sve retke koji imaju parent | **NetGen logika** | `stablo-store`, `stablo-pozicija-ne-ime` | #8, #13 |
| Id `"0"` se ne crta | Dijete čiji je id tekst `"0"` i njegovi potomci ispadaju | Carpo nema taj preskok | **NetGen logika.** `Stablo.cs` 237 | `stablo-store` (dijete `0` nije u stablu), `stablo-pozicija-ne-ime` | #8, #13 |
| Tag bez `:` | Na stupac 3 dodaje `:imeForme` | Carpo `value` ostaje kako ga upit vrati | **NetGen logika** | `stablo-store-tag` (`1:KLAS`) | #8 |
| `@@` koji preživi | `ExecSql` preskoči ako `IndexOf("@@")>0` (`Class1.vb` 113–118). Fill stabla taj preskok nema | Carpo šalje upit koji je već mapiran | **NetGen logika.** Stablo i dalje zove čitanje | `sql-atat-kasnije` (grid preskače), `stablo-fill-ne-preskace-atat` (stablo šalje) | #4, #13 |
| Prazan `TREE` | Sakrije `TreeView2`, splitter 0, gumb ugašen. `Form1.vb` 4310–4316 | Carpo forma nema taj splitter | **NetGen logika** | `stablo-jezici-skriveno`, `oznaka-stablo-0` | #8 |
| Prazan `TREE`, `COMBO` postoji | Ne ulazi u sakrivanje. Splitter ostaje dizajnerskih 243 dok combo ne napiše čvorove. `4310–4320`, `7730` | | **NetGen logika** za vidljivost. Combo koji mijenja SQL drveta je **otvoreno** (`tree.md`) | `nema` za prvi prolaz comboa | #8 |
| `yearmonth` | Godine 2005–2009 i 12 mjeseci, iako u korpusu nema nijednog. Siječanj 2005 je `2005101#2005130:FORM`. `4527–4540` | | **NetGen logika** | `stablo-yearmonth`, `stablo-yearmonth-tag` | #8 |
| Statični čvor | Lijevo je natpis, tag je `:` + desno, `@@node` je prazan. `4353–4355` | | **NetGen logika** | `stablo-static`, `stablo-static-tag`, `stablo-afterselect` | #8 |
| Dizajner TreeView | `AllowDrop` true, obrub None, `ControlLightLight`, `ShowRootLines`/`ShowPlusMinus` true. `Form1.designer.vb` 572–581. Svježi TreeView je Fixed3D i `AllowDrop` false | | **NetGen logika** (forma, ne svježa kontrola) | `stablo-obrub-none` | #8 |
| Ikona | `ImageList2`, indeks 0, ključ `OrdersDetails16.ico`. `designer` 575–579 | `.ico` nije u repou | **Carpo mehanika.** `Podloga` crta zamjensku sličicu 16×16 i drži indeks 0. `Podloga.cs` 74–76 | `stablo-prevod` (ikona 0) | #9 |
| Prijevod | `Translate` (`Module1.vb` 266) vrati izvornik za prazan jezik i `hrv`. Inače `JupiterPrevoditelj` | DLL nije u repou | **NetGen logika** za hrv/prazno. Tuđi DLL je **otvoreno**: ako je rječnik dan, natpis se zamijeni | `stablo-hrv-ne-prevodi`, `stablo-prevod` | #8 |
| `TREEDRAGDROP` | Šalje SQL. `8175` | | **NetGen logika** za tekst. Upis se ne šalje (vidi sigurnost) | `stablo-drag-ne-pise` | #8 |

## Grid

Janus `GridEX` je živi put. `DXGRID` u korpusu ima 0 pogodaka (`grid.md`). Svježi DevExpress nije isti kao forma NetGena, pa Carpo.NetGen svojstva upisuje eksplicitno.

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| Auto-filter | `FilterMode=None` dok XML nema `FILTERMODE=true`. `Form1.vb` 2169 | `ShowAutoFilterRow=true` osim eksplicitnog false. `ComponentGrid.cs` 772–773 | **Namjerno odstupanje** kad `FILTERMODE` nema. Kad atribut postoji, vrijedi NetGen (`true` uključi, sve ostalo ugasi) | `oznaka-filter-carpo`, `oznaka-dx-autofilter`, `filtermode-nema-carpo-ukljucen`, `filtermode-false-gasi` | #4 |
| Uređivanje | Designer `EditMode=EditOff`, stupac `NoEdit`. `GRIDALLOWEDIT` se ne čita | Svježi DX `Editable=True` | **NetGen logika.** DX niz `Editable=false`. TestShell: `ReadOnly` i `EditProgrammatically`. `Podloga.cs` 81, 94 | `oznaka-gridallowedit-nije-edit`, `jezici-edit-noedit` | #4, #9 |
| Enter | Kod stavi `None` ako nema `EDITCOLUMNS`. `3419`. Svježi GridEX je `NextRow` | DX Enter otvara editor | **NetGen logika** | `oznaka-edit-ignorira-gridallowedit` | #4 |
| Selekcija | Designer `MultipleSelection`, `HideSelection=Highlight`. Svježi GridEX je jedan red | DX `IsMultiSelect=False` | **NetGen logika.** `MultiSelect=true`, `EnableAppearanceHideSelection=false`. TestShell `MultiSelect` i cijeli red. `Podloga.cs` 85–86 | `jezici-header-prazni-redovi`, `dx-header-empty-select` | #4, #9 |
| Row header i obrub | Designer `RowHeaders=True`, `BorderStyle=None`. Svježi GridEX je obrnuto | DX indicator i `BorderStyle=Default` | **NetGen logika.** `ShowIndicator=true`, `NoBorder`. TestShell `RowHeadersVisible`, `BorderStyle.None` | `dx-header-empty-select` | #4, #9 |
| Prazni redovi i tri retka zaglavlja | `EmptyRows=True` (`Class1.vb` 1000), `HeaderLines=3` (`1364`) | DX ima jedan red zaglavlja i nema praznih redova | **NetGen logika.** TestShell visina zaglavlja 48 (`3×16`). `Podloga.cs` 92 | `jezici-header-prazni-redovi` | #4, #9 |
| Font | MS Sans Serif 8.25 na formi | tema | **NetGen logika** | `nema` zasebnog; vidi se na podlozi | #9 |
| Širina stringa | 100 u `CreateGridColumns` (`1203`). 200 je samo hijerarhijski put (`Form1.vb` 1799) | DX često širi kolonu sam | **NetGen logika.** `ColumnAutoWidth=false` | `kol-string-100` | #4 |
| Širina iz XML-a | VB `/` pa bankarsko zaokruženje. `2500/14` je 179 | cjelobrojno dijeljenje dalo bi 178 | **NetGen logika.** `MasterGrid.VbJedinica` | `jezici-vb-179`, `jezici-naziv` | #4 |
| Total red | Kod ga upali osim `TOTALROWVISIBLE=FALSE` (`2049`). Svježi GridEX je false | DX footer nije uključen | **NetGen logika** (kod, ne svježi GridEX) | `total-default-ukljucen`, `oznaka-total-iskljucen` | #4 |
| `AGREGATEFUNCTION` s dva G | Ne veže se (`7496`) | | **NetGen logika** | `jezici-dva-g-nema-agregat` | #4 |
| `CLEARCOLUMS` | Jedno N. `false` ne skriva ostale stupce. `7459–7462` | | **NetGen logika.** `CLEARCOLUMNS` se ne čita | `jezici-clear-false-ne-skriva` | #4 |
| Stupac koji završava na ID | Skriven, format `##0` | | **NetGen logika.** `@@id` bez stupca `id` je prvi podatak (Janus ćelija 2) | `kol-id-skriven`, `det-id-prvi` | #4, #13 |
| `GroupTotals` | Svježi GridEX `Never`, designer `Always` | `GroupFooterShowMode=VisibleIfExpanded` | **Otvoreno.** Nije mapirano | `nema` | #4 |
| Grupni panel | `GroupByBoxVisible` ostaje true dok XML ne kaže false | DX panel nije isti objekt | **NetGen logika.** `ShowGroupPanel=true` | `groupby-janus-default-true`, `oznaka-groupby-false` | #4 |
| Greška SQL-a | `ExecSql` Catch pokaže poruku i vrati prazno. `spNetGenErrorLog` se ne zove iz ovog puta | | **NetGen logika** za prikaz. Zapis greške ide u Carpo logger (vidi log) | `sql-greska-ne-puni`, `log-greska` | #4 |
| `ONLOAD` počinje s `:` | Nije SQL, ime forme. `Form1.vb` 3879 | | **NetGen logika** | `dvotocka-nije-sql` | #4 |
| `REFRESHMASTERGRID` | Ponovi zadnji već zamijenjeni SQL, ne izvorni predložak. `4570` | | **NetGen logika** | `refresh-koristi-zadnji-tekst`, `refresh-redoslijed` | #4 |
| Crtanje u TestShellu | Janus GridEX | DevExpress GridView je cilj u Jupiteru | **Carpo mehanika** za crtež prozora: `DataGridView` s NetGen defaultima upisanim u kod (`Podloga.cs` 79–111). Niz `DxPostavke` u `MasterGrid` je ugovor za DevExpress host | `dx-header-empty-select` (niz), podloga drži ista ponašanja | #4, #9 |

## Log

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| Kanal | `NapuniCBLog` puni prsten `CBLog` od 40 redova. `PokaziCBLog` (`Class1.vb` 57, 76–82) | `Carpo.Core.Logger` / `LogList`. Ctrl+F11 otvara `Carpo.Core.Log.Log` (`JupitetShared.cs` 299) | **Carpo mehanika.** Jedini ulaz je `NgDnevnik`. Prsten od 40 se ne pravi; Ctrl+F11 vidi cijeli `LogList` | `log-preskok`, `log-greska` | #4, #9 |
| Tiha greška | `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard` se ne logiraju | | **NetGen logika** | `nema` zasebnog | #4 |
| Tablica grešaka | `spNetGenErrorLog` | `Logger.Error` zove `spCarpoLogInsert` | **Carpo mehanika.** NetGen tablica se ne puni | `log-greska` (razina Error) | #4 |
| `CB(400,1)` | Međuspremnik `SETDEFAULTS`, nije log | | **NetGen logika.** Ostaje podatak | `nema` zasebnog; Jezici `tkosmomi` ide kroz kuku | #4 |
| Dva dijaloga na grešci | NetGen `MsgBox` uz log | `Logger.Error` pri `LogLevelUser` 3 ili 4 otvori Carpo poruku | **Otvoreno.** Zadani `LogLevelUser` je 0, poruka se ne otvara. Tko prikazuje dijalog kad oba postoje nije zaključano | `nema` | #4 |

## Master-detail

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| Donji grid | `GridEX2` / `PokaziUpitDetalj` (`2258`), jednom nakon mastera (`2104`) i na `SelectionChanged` (`7413`) | Carpo detail je zaseban grid po imenima polja | **NetGen logika.** Isti predložak, `@@id` iz tekućeg reda | `det-zamjena`, `det-refresh-predlozak` | #13 |
| Prazan i vodeće `:` | Nema SQL-a. `:` NetGen javi `sorkac` (`2105`) | | **NetGen logika.** Poruka ide u log, SQL se ne šalje | `det-prazan`, `det-dvotocka` | #13 |
| Parametri reda | `PokupiParametreSaGrida` (`2544`): `@ime='vrijednost'`, zadnje 4 znamenke otpadaju ako `Val` nije 0, razmak u `_`, `!` se briše, zarez u broju postaje točka | | **NetGen logika** | `det-zamjena` | #13 |
| `SQL2`/`SQL3`/`SQL4` | Dodatne tablice hijerarhijskog mastera (`1950–1978`), ne drugi `GridEX2`. Na `PROJECT` u korpusu nisu nađeni | | **NetGen logika** | `det-sql2` | #13 |
| Stupci detaila | Stvaraju se samo dok je `RootTable` null (`2334–2336`). Kasniji refresh zadrži stare | | **Otvoreno.** `DetailGrid.PrimijeniTablicu` složi stupce iz rezultata svaki put | `nema` | #13 |
| `DETCHECKBOXVISIBLE` / total / `DETMOVELAST` | Ista pravila kao master, širina `/14` | | **NetGen logika** za zastavice. `HEADERALIGNMENT` i `AGREGATEFUNCTION="0"` na detailu nisu zaključani testom | `det-checkbox-skriven` | #13 |
| Defaulti `GridEX2` | Isti skup kao master (`designer` 683–716), `GroupTotals=Always` i dalje otvoren | | **NetGen logika** na donjem `DataGridView`. `Podloga.cs` 97–111 | `nema` zasebnog osim crteža podloge | #13 |

## Komande i dijalozi

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| `ONSUBMIT` koji počinje sa `sp` | Prvih 6 slova nisu u `INSERTDELETEUPDATESELECT`, pa se niz `@ime=` lijepi iako SQL već ima parametre. `Form2.vb` 3378, `Form1.vb` 5514 | Carpo prepis Kultura zove druge procedure | **NetGen logika.** Stari `spKultureNGAzur` ostaje | `sp-lijepi-parametre`, `dml-ne-lijepi` | #14 |
| `!!` | Na kraju store gasi lijepljenje (`5449`). U captionu je novi red (`Form2.vb` 2943) | | **NetGen logika.** Dva značenja ostaju | `uzvicnik-gasi-lijepljenje`, `caption-novi-red` | #14 |
| Prazan text i prazan number | Text `''` (`3655`), number `0` (`3601`). Isti naziv polja može biti text na unosu i number na izmjeni | | **NetGen logika** | `prazan-tekst`, `prazan-broj` | #14 |
| Apostrof | U kontroli `¸¸`, u SQL-u `''`. `Class1.vb` 850 | | **NetGen logika** | `apostrof` | #14 |
| Tražilica | `Form3.Trazilica` (`99–126`): `sp`/`select` ide kako jest; inače `tablica#id#LIKE`. `!` je samo sort. Parametar je `Tag`, prazan ili `0` je null (`Form2.vb` 3626–3632) | Carpo `searchedit` veže polja po imenu | **NetGen logika** za tag i tablicu. Cijeli `WHERE` iz `Form3` još nije u kodu | `search-tag`, `search-tablica`. Puni `WHERE`: `nema` | #14 |
| Datum | `firstdayofyear` je `01.01.` tekuće godine (`2254`). Parametar `yyyyMMdd` (`3531`) | Carpo datum je tip datuma | **NetGen logika** | `datum` | #14 |
| Multidrop | Označeni id-evi spojeni tildom (`3446`) | Checked combo često šalje zarez | **NetGen logika** | `multidrop` | #14 |
| Radio i check | Radio `1`/`0` (`3432`). Check je `CheckState` (`3424`) | | **NetGen logika** za radio. Check kao `CheckState` nema zaseban test | `nema` zasebnog za check; radio je kuka `radio` | #14 |
| Combo `ALLOWEMPTY` | `true` bez obzira na velika slova ubaci red `null` (`2402–2411`). Bez toga prazan combo pokuša vrijednost `1` (`3455–3468`) | | **NetGen logika** za zastavicu. Pokušaj vrijednosti `1` nema zaseban test | `allowempty` | #14 |
| `REFRESHCONTROL` | `select 'refresh' as ime` (`6915`), pa combo s vrijednošću `refresh` ponovo izvrši svoj SQL (`5158–5163`) | | **NetGen logika.** Kuka QA je token `select refresh` | `refresh-sql`; QA `KUP-15` | #14 |
| `REFRESHMASTER` | Nije `REFRESHMASTERGRID`. Grid se ne osvježava tom riječju. `4570` | | **NetGen logika** | `refreshmaster-nije-grid` | #14 |
| Tilda | U parseru ostaje. `~` postaje `#` tek u `NovaGenerickaForma` (`4558`) | | **NetGen logika** | `tilde-u-lanacu` | #14 |
| `##` | Prazan segment. `i = i`, sljedeći `OK` se ne preskače (`6821–6822`) | | **NetGen logika** | `prazan-segment` | #14 |
| `spRaiserror` | Poruka bez `#OK` i `+OK` ne guta grešku. `Exit For` stane (`Class1.vb` 187–189, `Form2.vb` 7511) | | **NetGen logika** | `raiserror-prekida`, `hash-ok-guta` | #14 |
| `+focus(x)` | Upiše fokus. Samo to nije `+OK` (`202–204`) | | **NetGen logika** | `focus-nije-ok` | #14 |
| `OKSTAY` / `CLOSE` | `OKSTAY` ne zatvara (`6884`). `CANCEL` i `CLOSE` su `Button2`. `CLOSEPARENT` prvo postavi `bCloseParent` (`7031–7037`) | | **NetGen logika** | `okstay`, `close-button2` | #14 |
| `FOREACHSELECTED` | Na dijalogu vrti **čekirane** i pojede sljedeći segment (`7040`, `8337`). Na podlozi je poziv komentar, `i = i + 1` je živ (`Form1.vb` 6843–6850) | Carpo multiselect je plava selekcija, ne checkbox stupac | **NetGen logika.** Čekirani i plavi su dvije naredbe. Podloga petlju ne vrti | `foreach-dijalog`, `foreach-podloga` | #14 |
| `FOREACHBLUEROW` / `FOREACHROW` | Plavi su `SelectedItems` (`7043`). Svi redovi su `FOREACHROW` (`7046`). Na podlozi isto: poziv komentar | | **NetGen logika** | `foreach-plavi` | #14 |
| Gumbi podloge | `5667` zove `NovaGenerickaForma`, ne `Rafalno` | | **NetGen logika** | `foreach-podloga` | #14 |
| `TotalGenMultiSelectTemp` | Prije forme dva `delete` (`suser_sname()` i `operateriid`), zatim insert čekiranih s `host_name()`. `4934–5018`. `MULTIBLUE` uzima plave (`5022`) | | **NetGen logika** za oblik naredbe. **Namjerno odstupanje:** test i kuka ne šalju te naredbe u bazu | kuka `multiTemp` = `delete-dva-pa-insert` | #14 |
| `SETDEFAULTS` | Reže se po `#` i svaki komad ide zasebno (`3862–3866`) | | **NetGen logika.** `GranulaIzvrsenje` reže i guta grešku jednog komada | `nema` zasebnog imena; Jezici `tkosmomi` prođe na spinu | #14 |
| Filter i clipboard | Ime forme koje sadrži `filter` uključi DB clipboard osim `DBCLIPBOARD=false` (`Form2.vb` 1102–1104) | | **Otvoreno** u kodu dijaloga. Pravilo je zapisano u `dijalozi.md` | `nema` | #14 |
| `ONLOADED` | Zove se samo kad je `BrojKontrola = 0` (`Form2.vb` 2798). Forma s `INPUT` se prikaže | | **Otvoreno.** `Dijalog` još ne provjerava broj kontrola | `nema` | |

## Poziv Carpo subforme

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| `ASSEMBLYLOAD` | `Assembly.Load`, `CreateInstance`, late-bound `totalgen` s pet argumenata. Dijalog šalje `strParametriSvi` dvaput (`Form2.vb` 7424–7456). Povrat se ne čita. Greška prekida lanac | Isti proces, `Carpo.Gen` / postojeći `Run`. `docs/funkcionalnosti/poziv-carpo.md` | **Carpo mehanika** (nema novog `Assembly.Load`, nema drugog Jupitera). **Otvoreno:** kod još nije spojen | `nema` | nema |

## Parser i sigurnost

| Stavka | NetGen | Carpo | Odluka | Test | PR |
|---|---|---|---|---|---|
| Tipfeleri | `HIGHT` ostaje. `datetiem` upadne u datum jer `InStr` nađe `date`. `CLEARCOLUMS` se čita, `CLEARCOLUMNS` ne | | **NetGen logika** za imena atributa. Grananje `datetiem` je **otvoreno** do elementa kontrola | `nema` za `datetiem` | #2 (parser) |
| `<root>` i `<PROJECT>` | NetGen čita `<PROJECT>` | `<root>` ostaje `Gen.Start`. `<PROJECT>` ide u Carpo.NetGen | **Carpo mehanika** za granu ulaza | smoke korpusa | #2 |
| Upis | NetGen šalje `insert`/`update`/`delete` | | **Namjerno odstupanje.** `SqlTekst.JeZabranaPisanja` zaustavi naredbu prije slanja. Čitanje (`select`, `sp`, `exec`) ide | `zabrana-insert`, `select-nije-zabrana`, `stablo-drag-ne-pise` | #4 |
| Poslužitelj | Produkcija zna ići na `jupiter2022` | | **Namjerno odstupanje.** `SpinVeza` odbije ime koje sadrži `jupiter2022`. Čitanje je `localhost` / `spin` | `ne-jupiter2022` | #4 |
