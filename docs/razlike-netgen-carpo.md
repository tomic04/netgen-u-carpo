# Razlike NetGen / Carpo

Registar za implementaciju. QA dopisuje stavku čim vidi da isto ime ne znači isto ponašanje. Izvor istine za NetGen je `netgen/*.vb` i XML. Carpo citat je kod kontrole ili `docs/analiza/04-mapiranje-netgen-carpo.md` kad port tu kontrolu još ne crta. Port koji već prati NetGen nije ovdje ponovljen kao rupa.

Prije novog broja povuci `docs/razlike-netgen-carpo.md` s najviše QA grane i nastavi od zadnjeg `R`. R01–R44 su QA (#16 i #17). Carpo.NetGen od R45 dopisuje odluku, test i PR u istom PR-u.

## Odluka u Carpo.NetGen

| Broj | Odluka | Test | PR |
|---|---|---|---|
| R01 | NetGen logika. Stupci po poziciji, prvi red, id `0`, tag | `stablo-pozicija-ne-ime`, `stablo-fill-ne-preskace-atat` | #13 |
| R02 | NetGen logika. Prazan `TREE` sakrije stablo, splitter 0 | `stablo-jezici-skriveno` | #8 |
| R04 | `REFRESHMASTERGRID` ponovi zadnji SQL. Nuspojava prazne forme je otvorena | `refresh-koristi-zadnji-tekst`, `refreshmaster-nije-grid` | #4, #14 |
| R14 | `sp` dobije drugi niz. `!!` gasi lijepljenje | `sp-lijepi-parametre`, `uzvicnik-gasi-lijepljenje` | #14 |
| R16 | Tražilica: `sp`/`select` ostaju, `dbo.sp` ide u `tablica#`, `!` nije u WHERE | `trazilica-dbo-nije-sp`, `trazilica-like` | #18 |
| R24 | Četiri `firstday*`. `today` ostaje literal. Siječanj da mjesec `0` | `datum-mjesec-0`, `datum-today-literal` | #18 |
| R25 | `SHOWCHECKBOX` bez `CHECKED` šalje `null`. `AllowNull` ostaje false | `datum-checkbox-null`, `datum-nije-allownull` | #18 |
| R31 | `TREE33`, `WUDTH`, `DEFALUT`, `GRUOPBYBOXVISIBLE` se ne čitaju | `tipfel-ne-popravlja` | #18 |
| R35 | `MPNGPregDokMAIL` je shell `totalgen`, nije Carpo subforma | `assembly-shell-pet` | #14, #18 |
| R37 | `ONLOADED` s kontrolom ide u `Form2_Load`, ne u grani nula kontrola | `onloaded-u-loadu` | poziv |
| R38 | Tri potpisa. Dijalog šalje `strParametriSvi` dvaput. Shell šalje zadnji SQL | `totalgen-dijalog`, `totalgen-shell`, `totalgen-rafalno` | poziv |
| R39 | Tilda u routeru otvara lanac. Bez tilde je jedan poziv | `tilda-router` | poziv |
| R40 | `ASSEMBLYLOAD` ne čita povrat. Greška prekida lanac | `assembly-ne-cita-povrat`, `assembly-prekine` | poziv |
| R41 | `PRINTMASTER` i `PRINTREPXMASTER` su isti ispis. Orijentacija se ne gleda | `print-master-isti`, `print-ne-gleda-orijentaciju` | ispis |
| R42 | Dijalog `repx:/`, shell `repx:`. Port zove v26, fallback `rptDevRepx` | `repx-prefiks`, `spin-v26` | ispis |
| R43 | `vrstadokid-` ide u ispis, nije ime forme | `vrstadok-nije-forma` | ispis |
| R44 | Ime forme koje počinje s `MULTISELECT` briše temp i prije ispisa | `multiselect-prije-ispisa` | ispis |

## R01 Stablo iz SQL-a nije Carpo `tree`

- **Element:** `TREE` / `TreeView2` naspram `JTree`
- **NetGen:** `SqlDataAdapter.Fill`, ne `ExecSql`, pa `@@` koji preživi zamjenu ipak ode na server (`Form1.vb` 4442–4447, `Class1.vb` 113–118). Relacija je stupac 0 → stupac 1, natpis stupac 2, tag stupac 3 (`4450`, `4461–4478`). Imena kolona se ne gledaju. Korijen je samo prvi red (`4455`, `4480`). Dijete s id `"0"` se ne crta (`4498`). Tag bez dvotočke postane `vrijednost:ImeForme` (`4474–4477`).
- **Carpo:** `JTree` traži `key` / `parentfield` / `valuefield` po imenu kolone (`Devex_26/Carpo.Structure/Models/Core/DataControls.cs` 8–16, `ComponentTree.cs` 770–793). Nema pravila „samo prvi red“ ni „preskoči id 0“.
- **Posljedica:** isti SELECT nacrtan kao Carpo tree dobije druge čvorove, ili upozorenje da key/parent ne postoje, iako u NetGenu radi.
- **Test:** `STA-30`, `STA-31`, `STA-32`, `STA-35`

## R02 Prazno stablo se skloni, ne ostane prazan panel

- **Element:** lijevi splitter
- **NetGen:** prazan `TREE` i prazan combo sakriju `TreeView2` i lijevi splitter stave na 0 (`Form1.vb` 4310–4316).
- **Carpo:** `tree` je kontrola u layoutu. Port to prati u `ShellLayout.Iz` (`Devex_26/Carpo.NetGen/Shell/ShellLayout.cs` 56–61). Carpo manager i dalje drži drvo kao stupac, ne kao WinForms splitter na 0.
- **Posljedica:** granule bez stabla (OznakaKL) ne smiju pokazati prazno drvo.
- **Test:** `OKL-31`, `ShellLayout`

## R03 Combo puni stablo, atribut `TREE` ne mora postojati

- **Element:** `COMBO` na projektu (PrijemniList)
- **NetGen:** prvi prolaz vidi prazan `TREE`, ali combo nije prazan pa stablo ne gasi (`4310–4320`). Kasnije `SelectedValue` prepiše `strTreeNodes` i zove `NapuniDrvoVrijeme` (`7730–7733`).
- **Carpo:** `ShellLayout` vidi samo tekst atributa `TREE` i činjenicu da `COMBO` nije prazan (`ShellLayout.cs` 51–57). Ne čita odabranu vrijednost comboa kao SQL stabla.
- **Posljedica:** stablo je „vidljivo“, a čvorova nema. Kostur koji gleda samo `TREE` granulu ostavi bez drveta.
- **Test:** `PRI` checklista, `docs/granule/PrijemniList.md`

## R04 `REFRESHMASTER` nije `REFRESHMASTERGRID`

- **Element:** komanda na gumbu
- **NetGen:** samo `REFRESHMASTERGRID` ponovi zadnji SQL mastera (`Form1.vb` 4570–4577). `REFRESHMASTER` traži formu tog imena. Ako forma ne postoji, 0 kontrola i `OKClick`, pa `PokaziUpit` zadnjeg SQL-a (`5519`). Osvježenje je nuspojava prazne forme (JeziciNG `Unesi#refreshmaster`).
- **Carpo:** `control populate` nema ova dva imena. Port u `MasterGrid.cs` 243 bilježi samo `REFRESHMASTERGRID`.
- **Posljedica:** preimenovanje u `REFRESHMASTERGRID` promijeni JeziciNG (radi i bez te riječi). Izostanak oba ostavi PrijemniList bez osvježenja.
- **Test:** `JEZ-65`, `KUP-02`, `PRI` checklista

## R05 Naslov `v[verzija]` nije `root/@version`

- **Element:** `VERSION` + `CAPTION`
- **NetGen:** uz verziju naslov dobije dva razmaka i ` v[…]` (`Form1.vb` 3524–3525). Element `NAME` nije naslov.
- **Carpo:** `root/@version` je verzija definicije, ne sufiks prozora (`docs/analiza/04-mapiranje-netgen-carpo.md`, tablica komandi).
- **Posljedica:** Statusi se zovu `Statusi  v[1.1]`, ne `Statusi`.
- **Test:** `STA-02`

## R06 `PERMISIO` nije Carpo permissions

- **Element:** `ssPermission`
- **NetGen:** atribut koji fali ostane `True` (`Form1.vb` 3153) i šalje select nad `ssPermission` (`3414`, `2706`). Greška s `sspermission` se guta (`Class1.vb` 162–163).
- **Carpo:** `Logic/Permissions.cs` je drugi ugovor (dokument 04).
- **Posljedica:** uključiti Carpo permissions na NetGen XML promijeni tko vidi gumb.
- **Test:** `JEZ-71`, `OKL` permisio

## R07 Janus grid nije DevExpress grid

- **Element:** master grid
- **NetGen:** `DXGRID` nije true → Janus (`Form1.vb` 1906–1908). Više redova, row header, incremental search (`Form1.designer.vb` 640–641, 655, 657). Stupac čije ime završava na `ID` je skriven (`Class1.vb` 1125–1127). `GetID` bez stupca `id` uzme ćeliju 2 (`Form1.vb` 5566–5580). Širina je `jedinica/14` uz VB zaokruživanje na parni (`Form2.vb` 58). `AUTOMATICSORT` nije `false` → sort klikom ostaje (`3401–3404`).
- **Carpo:** `GridControl` nema te Janus početke. Port širinu računa u `MasterGrid.VbJedinica` (`MasterGrid.cs` 88–90). Ostalo (selection, `GetID` → ćelija 2) nije taj grid.
- **Posljedica:** OznakaKL `@@id` je `klmasterrobaid`, ne prvi vidljivi stupac. Širina 2500 je 179, ne 178.
- **Test:** `OKL-31` nadalje, `JEZ` širina

## R08 `AGGREGATEFUNCTION` s dva G se ne čita; `"0"` s jednim G se čita

- **Element:** `COLUMN`
- **NetGen:** traži se jedno G (`Form1.vb` 7496). Dva G (JeziciNG) ne zbrajaju. `"0"` nije prazan string, agregat se dodijeli (`7506`).
- **Carpo:** grid sum po imenu polja. Port u `MasterGrid.cs` 447–448 upisuje agregat ako atribut nije prazan, uključujući `"0"`.
- **Posljedica:** „popravak“ dvostrukog G uključi sumu koje u NetGenu nema. `"0"` se ne smije tretirati kao „nema“.
- **Test:** `JEZ` checklista, `BLA` `AGREGATEFUNCTION`

## R09 Detail kolone samo dok je `RootTable` prazan

- **Element:** detail grid
- **NetGen:** `CreateGridColumns` se preskoči ako `RootTable` već postoji (`Form1.vb` 2334–2336). Drugi red mastera ne složi kolone iznova.
- **Carpo:** `control populate` obično ponovo veže kolone.
- **Posljedica:** drugi detail upit u Statusima i KntBlaZak ne smije prepisati format.
- **Test:** `STA-39`

## R10 Statički čvor stabla nije roditelj storea

- **Element:** `TREE` segment bez `tree` u lijevom dijelu (KntBlaZak `Blagajne:`)
- **NetGen:** susjed, ne roditelj (`4347–4356`). Klik na njega ne radi ništa (`4546`). Svaki store je svoj korijen (`4480`).
- **Carpo:** `parentfield` bi te čvorove ugnijezdio ako im SQL vrati parent.
- **Posljedica:** tri storea ostanu tri korijena. Klik na natpis „Blagajne“ ne mijenja master.
- **Test:** `BLA` checklista

## R11 Detail parametar je ime stupca, ne natpis

- **Element:** `@@blagajneid` u `DETAILGRID`
- **NetGen:** `PokupiParametreSaGrida` lijepi `@` + `DataMember` (`Form1.vb` 2564–2572, `2301–2304`).
- **Carpo:** parametar grida često ide po `caption` / `name` kolone u XML-u, ne po imenu polja rezultata.
- **Posljedica:** detail prazan ako se veže na natpis umjesto na `blagajneid`.
- **Test:** `BLA` checklista

## R12 `SETDEFAULTS` i `#` nisu jedan SQL batch

- **Element:** `SETDEFAULTS` (PrijemniList)
- **NetGen:** svaki segment između `#` je poseban `ExecSql` (`Form1.vb` 3862–3866).
- **Carpo:** jedan `data` command s `#` u tekstu SQL Server odbije (`Incorrect syntax near '#select'`).
- **Posljedica:** otvaranje granule padne prije stabla.
- **Test:** `PRI` checklista

## R13 `@robaid` u `declare` nije `@@robaid`

- **Element:** `DETAILGRID` Prijemnog lista
- **NetGen:** `@robaid` je lokalna SQL varijabla koju `declare` stvori. NetGen je ne puni iz grida. `@@id` jest `GetID` (`Form1.vb` 2294).
- **Carpo:** zamjena svakog `@ime` iz kontrole ili grida upiše vrijednost u tu varijablu.
- **Posljedica:** detail dobije krivi `@robaid`.
- **Test:** `PRI` checklista

## R14 `ONSUBMIT` koji počinje sa `sp` dobije drugi niz parametara

- **Element:** `ONSUBMIT` / `!!`
- **NetGen:** prvih 6 znakova u `INSERTDELETEUPDATESELECT` briše `strParametri` (`Form2.vb` 3378–3388). Inače se `@ime=` lijepi na kraj (`Form1.vb` 5514). `!!` na `ONSUBMIT` makne se i zabrani lijepljenje (`5449–5450`). `!!` u captionu je novi red (`Form2.vb` 2943).
- **Carpo:** `<parameter>` se ne lijepi drugi put, a `!!` nije token.
- **Posljedica:** Kulture dobiju parametre dvaput. Kartica partnera s `!!` ne smije dobiti treći niz. Caption s `!!` ostane u jednom redu ako se `!!` briše i tamo.
- **Test:** `KUL-04`, `KAR` `!!`

## R15 Prazan tekst i prazan broj nisu isto

- **Element:** `text` / `number` / `cur`
- **NetGen:** prazan text šalje `''` (`Form2.vb` 3655). Prazan `NumerBox` šalje `0` (`3601`). Početni tekst broja je `0`, pa `DEFAULT`, pa `VALUE` (`2084–2088`). `cur` bilo gdje u `TYPE` je isti `NumerBox` (`2071`), bez posebnog novčanog formata.
- **Carpo:** `textedit` šalje prazan string ili izostavi parametar. Nema pravila „prazno = 0“ (`docs/analiza/04` red `number`/`cur`). `CMEditVB6.CurMEdit` po točnom imenu ne postoji.
- **Posljedica:** Kulture `Cijena` na unosu (text) i izmjeni (number) daju `''` odnosno `0`. FakPar rabat prazan mora otići kao `0`.
- **Test:** `KUL-06`, `FAK-03`, `FAK-20`, `DOS-08`, `DOS-22`

## R16 Tražilica nije `searchedit`

- **Element:** `search` / `trazilica`
- **NetGen:** SQL koji počinje s `sp` ili `select` ostaje taj tekst (`Form3.vb` 99–100). `dbo.sp…` ne počinje s `sp`, pa ide u `tablica#id#kolone` (`102`). `%` je LIKE s te strane, `!` izbaci polje iz WHERE (`118–126`). Stupac 1 grida tražilice je skriven (`165`). Šalje se `Tag` (id); `0` ili prazan tag je `null` (`Form2.vb` 2145, `3626–3632`). `@@searchboxime` je upisani tekst. `ALLOWEMPTY` se na searchu ne čita (`2140–2216`). Caption na `_` ne ulazi u parametre (`2149–2150`).
- **Carpo:** `searchedit` + `netgenfields` ne gradi `roba#robaid#…` (`docs/analiza/04` red search). `valuefield` je po imenu.
- **Posljedica:** `vipartneri#…#!adresa` i `partneri#…#sifra%` ne rade kao Carpo search. `ALLOWEMPTY=""` na FakPar searchu ne smije uključiti prazan red.
- **Test:** `KUL-07`, `DOS-06`, `INV-08`, `INV-12`, `FAK-09`, `KON-25`

## R17 `spRaiserror` i `+focus` bez `+OK`

- **Element:** poruka servera
- **NetGen:** `#OK` / `+OK` u tekstu greške briše poruku. `+focus(ime)` upiše fokus (`Class1.vb` 187–204). `+focus` nije `+OK`, greška ostane, `Exit For` prekine ostatak `#` (`Form2.vb` 7511–7515).
- **Carpo:** `raiserror` je iznimka ili toast. Nema parsiranja `+focus` iz teksta.
- **Posljedica:** Kartica ostane na partneru uz poruku. Kulture `#OK` iza `spRaiserror` se ne izvrši ako je greška.
- **Test:** `KUL-09`, `KAR` focus

## R18 `ONLOAD=":FILTERFORM"` nije SQL

- **Element:** ulaz granule
- **NetGen:** vodeća `:` otvara formu (`Form1.vb` 3879–3880). Master se puni tek kad filter vrati `FILLEXPLORER` (`5445–5452`).
- **Carpo:** `onload` je `data` ili event. Nema „prvo modal, pa njegov ONSUBMIT postane master“.
- **Posljedica:** Kartica i MPNGPregDokMAIL pokažu grid prije uvjeta, ili prazan grid bez dijaloga.
- **Test:** `KAR`, `MAIL-04`

## R19 Prazna forma s `FILLEXPLORER` ne otvara prozor

- **Element:** čvor stabla `POPUNI_FORM`
- **NetGen:** 0 kontrola → nema dijaloga (`Form1.vb` 5369–5370), ali `FILLEXPLORER` prepiše master (`5445–5452`).
- **Carpo:** `form open` otvori prozor i kad je prazan.
- **Posljedica:** klik na čvor Statusa otvori prazan dijalog umjesto da samo zamijeni master.
- **Test:** `STA-08`, `STA-34`

## R20 `~` postaje `#` tek u routeru

- **Element:** `CMD` / `BUTTONS` / popup
- **NetGen:** `NovaGenerickaForma` zamijeni `~` s `#` prije cijepanja (`Form1.vb` 4558, `4561`). Popup bez `~` ide u `NovaGenerickaForma`, ne u `Rafalno` (`5743–5760`).
- **Carpo:** nema tildu kao razdjelnik akcija.
- **Posljedica:** parser koji tildu ostavi u imenu forme ne nađe formu. Mail popup (`MPNGPregDokMAIL`) nije lanac dijaloga.
- **Test:** `KUP-02`, `MAIL-02`

## R21 `MULTISELECT` i `MULTIBLUE` ne pune isti skup

- **Element:** `TotalGenMultiSelectTemp`
- **NetGen:** briše dvaput, po `suser_sname()` i po `operateriid`, pa insert id, `host_name()`, suser, operater (`Form1.vb` 4934–5018). `MULTISELECT` su čekirani (ili trenutni red). `MULTIBLUE` su `SelectedItems` (`5015` naspram `5022`).
- **Carpo:** nema te tablice. Selekcija grida je drugi skup.
- **Posljedica:** dva prozora istog operatera dijele retke. Plavi i čekirani nisu zamjena.
- **Test:** `KUP-03`, `KUP-04`, `KUP-10`

## R22 `FOREACH*` na podlozi ne vrti petlju

- **Element:** `FOREACHROW` / `FOREACHSELECTED` / `FOREACHBLUEROW`
- **NetGen:** na dijalogu petlja i pojede sljedeći `#` segment (`Form2.vb` 7040–7048, `8310–8394`). Na podlozi je tijelo komentar, ali `i = i + 1` i dalje pojede sljedeći segment (`Form1.vb` 6843–6850).
- **Carpo:** `control foreachrow` vrti. Nema „prepoznaj i preskoči“.
- **Posljedica:** ista naredba na gumbu podloge ne smije izvršiti petlju, a ne smije ni ostaviti sljedeći segment.
- **Test:** `KUP-11`, `KUP-12`, `KUP-13`

## R23 `OKSTAY` i `CLOSEPARENT`

- **Element:** gumbi dijaloga
- **NetGen:** `OK` zatvara (`Form2.vb` 6888–6890). `OKSTAY` ne (`6884–6887`). `CANCEL` i `CLOSE` su `Button2` (`7031–7034`). `CLOSEPARENT` još stavi `bCloseParent` (`7035–7037`).
- **Carpo:** `form close` je jedno. Nema `OKSTAY` ni zatvaranja roditelja iz djeteta.
- **Posljedica:** Stay ostane otvoren. Close parent zatvori i granulu, ne samo dijalog.
- **Test:** `KUP-16`, `KUP-17`

## R24 Datum `firstday*` nije `dateedit`, a `today` nije ključna riječ

- **Element:** `date` / `dtpicker` / `datetime` / `datetiem`
- **NetGen:** nakon `ToLower` samo `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth` (`Form2.vb` 2253–2265`). Mjesec bez vodeće nule. U siječnju `firstdayoflastmonth` da `01.0.godina` (`2263–2264`). `TODAY` postane literal `today` i ide u `Value` (`2267`). `DEFALUT` se ne čita (`2252`). `datetiem` i `datetime` sadrže `date` pa nastanu (`2232`). Parametar je `'yyyyMMdd'` ako je kvačica upaljena (`3531`), inače `null` (`3566`). `CUSTOMFORMAT` `HH:MM`, `MM.YYYY`, `DD.MM.YYYY HH:MM` mijenjaju format (`3535–3544`).
- **Carpo:** `JDateEdit` nema te tokene (`ComponentDateEdit.cs` 34–96). `allownull` je gumb i readonly izgled (`156–171`), ne kvačica `DateTimePicker`. Dokument 04, red tipfelera, kaže da se `datetiem` ne crta.
- **Posljedica:** MPInventura `datumod` mora biti 1. ovog mjeseca. `datumdo` s `DEFALUT="today"` ostaje običan datum. `datumact` `DEFAULT="TODAY"` ne smije dobiti novu ključnu riječ. `KOPIRAJ.datum` (`datetiem`) se crta. Siječanjski prošli mjesec ostaje `01.0.…` dok NetGen tako radi.
- **Test:** `KON-20`–`KON-23`, `INV-02`, `INV-03`, `INV-04`, `INV-20`, `NAR-02`, `NAR-03`, `KAR` `firstdayofyear`

## R25 `SHOWCHECKBOX` nije `allownull`

- **Element:** datum s kvačicom (DobUgovori)
- **NetGen:** krene checked, `SHOWCHECKBOX=true` ugasi (`2275–2278`), zatim `CHECKED` ako postoji (`2295`). Bez `CHECKED` ostane ugašeno i šalje `null` (`3560–3567`).
- **Carpo:** `allownull=true` doda gumb i `SetCheck(false)` (`ComponentDateEdit.cs` 89–93, `170`). Nema atributa `SHOWCHECKBOX` / `CHECKED` iz NetGen XML-a.
- **Posljedica:** `datugovorvrijedido` bez kvačice šalje `null`, ne današnji datum.
- **Test:** `DOB-02`, `DOB-20`

## R26 Combo nije `comboedit` (`valuefield` / `DATAFIELD`)

- **Element:** `combo`
- **NetGen:** stupac 0 je vrijednost, stupac 1 prikaz (`Form2.vb` 2430–2431). Imena se ne gledaju. `DATAFIELD` se ne čita. `ALLOWEMPTY` je točno `true` (`2403`); prazan string nije. Tada se ubaci red `null` / prazan tekst (`2402–2411`). Bez toga, ako ništa nije odabrano, pokušaj `SelectedValue=1` (`3455–3468`). `TRANSLATE=true` prevodi samo stupac 1 (`2420–2424`). `@@comboboxime` je prikaz (`3504`).
- **Carpo:** `valuefield` i `displayfield` po imenu (`ComponentCheckedComboBoxEdit.cs` 156–175, isti obrazac kao combo). Nema `SelectedValue=1`. `allowempty` nije NetGen `ALLOWEMPTY`.
- **Posljedica:** procedura bez tih imena da prazan Carpo combo. FakPar `grupiranje` s `ALLOWEMPTY=""` i id `1` u NetGenu odabere prvi red. `KntDospjeleStavke.klpid` s `TRUE` ima prazan red. Prevod ne smije dirati id.
- **Test:** `DOS-04`, `FAK-05`, `FAK-06`, `INV-06`, `DOB-03`, `KON-24`

## R27 Zavisni combo je kolona `refresh`, ne `datafield`

- **Element:** `ONCLICK` / `ONLOSTFOCUS` comba
- **NetGen:** klik ide u `Rafalno` (`8497–8506`). SQL koji nije komanda ide u `RefreshControls` (`7510`). Vrijednost `refresh` u koloni čije je ime kontrola ponovo izvrši njen SQL (`5158–5182`), uz `ALLOWEMPTY` red (`5165–5176`). `REFRESHCONTROL(ime)` je kratica za `select 'refresh' as ime` (`6911–6915`).
- **Carpo:** `datavaluechanged` + `control populate` veže se po imenu polja. Nema vrijednosti `refresh` kao naredbe.
- **Posljedica:** `zupanijeid` → `mjestaid` (`@@zupanijeid`), `trgovinaid` → `osobeid`, `mpregijeId` → multidrop `str`.
- **Test:** `DOS-05`, `DOS-21`, `INV-05`, `INV-09`, `MAIL-06`

## R28 Multidrop nije `combomultiedit`

- **Element:** `multidrop`
- **NetGen:** šalje `'id1~id2'` bez razmaka (`Form2.vb` 3446`). `ALLOWEMPTY` se pročita (`2500`) i ne ubaci red. `strMultiDropSQL` je SQL zadnjeg multidropa na formi; refresh svih dijeli taj tekst (`2495`, `4817`).
- **Carpo:** `combomultiedit` šalje svoj format liste i traži `valuefield` (`ComponentCheckedComboBoxEdit.cs` 156–175).
- **Posljedica:** procedure koje cijepaju `~` ostanu prazne ako stigne zarez ili Carpo lista. `ALLOWEMPTY="FALSE"` na `dodklasa` ne smije dobiti null red „kao combo“.
- **Test:** `DOS-07`, `DOS-23`, `INV-09`, `NAR-08`, `KAR` multidrop

## R29 Check i radio nisu ista Carpo kontrola

- **Element:** `check` / `checkbox` / `vb.checkbox` / `radio`
- **NetGen:** sve što sadrži `check` je `mojCheck` (`1460`), uključujući `checkbox` i `vb.checkbox`. Parametar je `CheckState` (`3424–3427`), dakle 0, 1 ili 2. Radio je `1` ili `0` (`3432–3435`), ne vrijednost grupe.
- **Carpo:** `checkedit` je boolean. `radioedit` je grupa s jednom vrijednošću (dokument 04).
- **Posljedica:** `PoSkladistu` i `knjustv` ne šalju `true`/`false` kao tekst. `rbtnKumulativ` šalje `1` ili `0`.
- **Test:** `FAK-07`, `FAK-21`, `INV-07`, `INV-10`, `INV-22`, `DOB-05`, `KON-01`, `KON-02`

## R30 `TYPE` je `InStr`, ne enum, i grane nisu `ElseIf`

- **Element:** `INPUT TYPE`
- **NetGen:** neovisni `If` (`Form2.vb` 1256–2707), `On Error Resume Next` (`706`). `dxgrid` stavi `nadjen` pa `grid` ne nastane (`1579–1623`). Tip s dva ključa složi dvije kontrole. `b_utton`, `picture`, `int` ne pogode ništa. `NgType.Odredi` (`NgGrana.cs` 42–63) vraća samo prvu granu.
- **Carpo:** element se bira točnim imenom (`textedit`, `dateedit`, …). Dokument 04, red tipfelera, za `datetiem` kaže „ništa“.
- **Posljedica:** `datetiem`, `datetime`, `CMEditVB6.CurMEdit`, `VB.textbox`, `SEARCH` moraju upasti u granu. `b_utton` se ne crta. Tip s dva ključa u portu danas da jednu kontrolu; VB bi dao dvije. U ovim granulama takvog tipa nema.
- **Test:** `KON-01`–`KON-17`, `NAR-02`, `FAK-03`, `FAK-08`

## R31 Tipfeler atributa se ne ispravlja

- **Element:** `DEFALUT`, `WUDTH`, `GRUOPBYBOXVISIBLE`, `TREE33`, `ALOWEMPTY`, `DATAFIELD`
- **NetGen:** čita se točno ime. `DEFAULT` (`2252`), `WIDTH` (`2949`), `GROUPBYBOXVISIBLE` (`Form1.vb` 3350), `TREE` (`3387`), `ALLOWEMPTY` (`2403`). Ostalo OERN preskoči.
- **Carpo:** nema tih aliasa. Adapter koji „ispravi“ slovnu grešku promijeni ekran.
- **Posljedica:** MPInventura `datumdo` nema početni `today`. `pakovanje` ne dobije širinu iz `WUDTH`. FakPar grupiranje kutije ostane default. `TREE33` na MPNGPregDokMAIL ne crta stablo. `ALOWEMPTY` ne ubaci prazan red.
- **Test:** `INV-03`, `INV-21`, `NAR-07`, `NAR-21`, `FAK-02`, `MAIL-03`

## R32 Statusna traka nije footer akcija

- **Element:** `statusstrip`
- **NetGen:** ne nastaje nova kontrola. Uključi se postojeći `StatusStrip1`. `TEXT1`–`TEXT3` idu u tag velikim slovima (`Form2.vb` 1256–1260`). Atribut koji fali OERN preskoči. `@@` se i dalje nađe jer je usporedba s `ToUpper` (`Class1.vb` 865). Tekst je SQL (`4754–4756`).
- **Carpo:** `footer` su akcije, ne tri SQL etikete (dokument 04).
- **Posljedica:** `KntDospjeleStavke` `TEXT1` s `@@rowcountgriddosp` mora preživjeti velika slova. `TEXT3` koji fali ne ruši formu. DobUgovori `TEXT2`/`TEXT3` fale.
- **Test:** `DOS-02`, `DOS-20`, `DOB-07`, `KON-13`

## R33 `frame` i `group` su isti `GroupBox`; `panel` je `DOCK` broj

- **Element:** okvir
- **NetGen:** `group` ili `frame` → `GroupBox` (`1354`). `panel` → `Panel` (`1384`), i bez `NAME`. `DOCK` je broj `DockStyle` (`1376`, `1407`): `1` vrh, `2` dno.
- **Carpo:** `group` je red layouta, ne apsolutni okvir (dokument 04).
- **Posljedica:** dva panela u `DONOSAUTOMATSKI` i dalje nastanu. `bottomgroupbox` `DOCK=2` ide na dno, ne u novi red „ispod“.
- **Test:** `DOS-03`, `NAR-04`, `NAR-05`, `NAR-06`, `DOB-06`

## R34 Label ne šalje parametar

- **Element:** `label`
- **NetGen:** nije u `PokupiParametre`. `ONCLICK` ide u `Tag` (`1345`). `TEXT` na labeli nije natpis (OznakaKL). `BOLD` se ne čita.
- **Carpo:** `labeledit` nema komandu na klik osim ako se zamijeni gumbom (dokument 04).
- **Posljedica:** label s `ONCLICK` mora ostati klikabilan. Njen `NAME` ne smije u `@ime`.
- **Test:** `DOS-03` okolina, `OKL` „što se ne događa“

## R35 `ASSEMBLYLOAD` na shellu nije Carpo subforma

- **Element:** `ASSEMBLYLOAD` / `CarpoForm`
- **NetGen:** popup bez `~` ide u `NovaGenerickaForma` (`Form1.vb` 5759–5760`). `ASSEMBLYLOAD(` (`4584–4623`): assembly lijevo od zadnje točke, `CreateInstance` cijelog imena, `totalgen(id, operater, konekcija, zadnji SQL mastera, ParametriSaGrida)`. Pad `Load`/`CreateInstance`: CB log i `Exit Sub` (`4600–4611`). Povrat se ne čita. Dijalog (`Form2.vb` 7424–7456`) zadnja dva argumenta su `strParametriSvi`. `:ASSEMBLYLOAD` je treći potpis (`6602`).
- **Carpo:** `docs/funkcionalnosti/poziv-carpo.md` opisuje `NetGenAdapter.clsCarpoKnjigaUra` i skriveni `INPUT CarpoForm`. To nije `comdll`. `MPNGPregDokMAIL` zove `RepxToPdfToMail.StartProgram` i nema `CarpoForm`.
- **Posljedica:** mail ne otvara Knjigu URA. Argumenti su master SQL i parametri grida, ne parametri filter forme. Greška učitavanja prekida lanac.
- **Test:** `MAIL-02`, `MAIL-05`, `MAIL-20`, `MAIL-21`

## R36 `DBCLIPBOARD=false` gasi clipboard iako se forma zove filter

- **Element:** clipboard baze
- **NetGen:** ime koje sadrži `filter` uključi clipboard, osim ako je `DBCLIPBOARD` `false` (`Form2.vb` 1102–1104`).
- **Carpo:** nema NetGen DB clipboard.
- **Posljedica:** filter MPNGPregDokMAIL ne pamti zadnji unos. Kartica, čije ime sadrži `filter` i nema `false`, pamti.
- **Test:** `MAIL-05`, `KAR` clipboard

## R37 `ONLOADED` s kontrolom ipak se izvrši u `Load`

- **Element:** `ONLOADED` na `ADAPTER_FORM`
- **NetGen:** `ProcitajXML` zove `strONLOADED` samo kad je `BrojKontrola = 0` (`Form2.vb` 2798–2799). Svaki `INPUT` poveća brojač (`3069`), pa jedan skriveni `CarpoForm` tu granu preskoči i forma se prikaže (`4430`). `Form2_Load` zatim zove isti string (`5796`; VB ne razlikuje `stronloaded` i `strONLOADED`). Forma bez kontrola (ispis) ide samo kroz `2798` i `ShowDialog` se ne zove.
- **Carpo:** `docs/funkcionalnosti/poziv-carpo.md` je prvo opisao samo granu `2798`. `form open` nema poseban `Load` koji kasnije pojede `ONLOADED`.
- **Posljedica:** preskok `ONLOADED` čim forma ima kontrolu ne otvori Knjigu URA. Pokretanje `ONLOADED` prije prikaza, na grani nula kontrola, na ovoj formi se ne događa.
- **Test:** `KUC-04`, `BVG-02`, `MPB-04`

## R38 Tri potpisa `totalgen`

- **Element:** `ASSEMBLYLOAD`
- **NetGen:** shell `NovaGenerickaForma` (`Form1.vb` 4584–4623): id, operater, konekcija, zadnji SQL mastera, `ParametriSaGrida`. `:ASSEMBLYLOAD` u `Rafalno` podloge (`6602–6661`) isto, uz CB log. `Rafalno` podloge bez dvotočke (`6681–6727`) zadnji argument je `""`. Dijalog (`Form2.vb` 7424–7456): oba zadnja su `strParametriSvi` (tu je `@CarpoForm=`).
- **Carpo:** `poziv-carpo.md` traži poziv u procesu (`Carpo.Gen`), ne novi `Assembly.Load`. `comdll` ima drugi potpis (dokument 04).
- **Posljedica:** DMS `ScanPreview` s podloge ne dobije `CarpoForm`. Adapter dijaloga ga dobije. Zadnji prazan argument na `Rafalno` podloge nije `strParametriSvi`.
- **Test:** `KUC-03`, `KUC-05`, `KND-02`, `KND-04`

## R39 Tilda bira router

- **Element:** `CMD` / popup / `MASTERDBLCLICK`
- **NetGen:** `~` postane `#` tek u `NovaGenerickaForma` (`Form1.vb` 4558). Popup s tildom ide u `Rafalno(":" + …)` (`5743–5757`). Bez tildu ide u `NovaGenerickaForma` (`5759–5760`). `MASTERDBLCLICK` bez tildu je jedan poziv, ne lanac.
- **Carpo:** akcija je jedno stablo. Nema tildu kao razdjelnik prije routera.
- **Posljedica:** `ADAPTER_FORM~REFRESHMASTERGRID` otvori dijalog pa osvježi master. `ASSEMBLYLOAD(...)` bez tildu ne otvara `ADAPTER_FORM`.
- **Test:** `KUC-02`, `KND-03`, `KND-05`

## R40 Povrat `totalgen` se ne čita

- **Element:** `ASSEMBLYLOAD` naspram `CreateObject`
- **NetGen:** `ASSEMBLYLOAD` ne sprema povrat i ne čita `o.id` (`Form1.vb` 4607–4623). `CreateObject` upiše `o.id = ID` i poslije pročita `ID = o.id` (`5275–5278`). Greška `Load`/`CreateInstance` je CB log i `Exit Sub` (`4600–4611`).
- **Carpo:** `form open` vraća vrijednosti u pub vars. To nije ovaj ugovor.
- **Posljedica:** id koji Carpo forma vrati ne smije prepisati `@@id` na `ASSEMBLYLOAD` putu. Na `CreateObject` putu smije.
- **Test:** `KUC-06`, `MAIL-21`

## R41 `PRINTMASTER` nije Janus preview i ne gleda orijentaciju

- **Element:** `PRINTMASTER` / `PRINTREPXMASTER` / `PRINTGRID`
- **NetGen:** oba prefiksa na podlozi zovu `IspisGRIDREPX` i izađu (`Form1.vb` 4945–4955). `Button7_Click` (Janus, tu je `PAGEORIENTATION`, `6039–6043`) se ne zove. REPX master (`5984–6003`) puni zaglavlje, `##` i `!!` su novi red, prazan centar reže naslov do `v[`. Konekcija je `strGlobalSQLSERVERlogin`. Detail (`6067–6085`) koristi `Conn` i prefiks `Driver={SQL Server};`. Na dijalogu `PRINTGRID(` i `PRINTREPXGRID(` zovu istu `IspisGRIDREPX` (`6950–6957`, `6985–6991`) s argumentom `"lodani"` i zaglavljem grida (`9041–9056`). Prazan centar dijaloga je cijeli `Me.Text`, ne rez do `v[`.
- **Carpo:** `print` čije ime počinje s `repx` ide u REPX (`docs/analiza/01-carpo-arhitektura.md`). Nema dva potpisa master/detail ni `lodani`. `PrintBridge` još ne zove pisač (`Placeholders.cs` 46–52).
- **Posljedica:** `PAGEORIENTATION="LANDSCAPE"` na Kartici ne okrene ispis liste. `PRINTGRID` i `PRINTREPXGRID` nisu dva ispisa.
- **Test:** `KAR-30`, `KAR-31`, `JEP-02`, `MPB-02`

## R42 `:repx/` ide u Spin.Printing, pa u `rptDevRepx`

- **Element:** `repx/` / `repx:`
- **NetGen:** shell zamijeni `repx/` i `REPX/` s `repx:` (`Form1.vb` 5238–5239). Dijalog zamijeni s `repx:/` odnosno `REPX:/` (`Form2.vb` 4296–4297). `PrintRpt` (`Class1.vb` 594–632) prvo `IspisiMePrekoSpinPrintinga`: `Assembly.Load("Spin.Printing.v23")`, u catch `v9.1`, klasa `Spin.Printing.Start` (`297–304`). `Print` ili `PrintMulti`. Ako parametar već ima `@id=`, ne dodaje se drugi (`320–327`). Pad s hwnd pokuša bez hwnd (`362–369`). Ako Spin padne, `Form6` učita `rptDevRepx.clsReport`, `isnetgen = 1`, `DoPrint2` ili `DoPrintDB` za `FROMDB-` (`Form6.vb` 134–161). Putanja predloška je `SharedDLLs\repx`.
- **Carpo:** odluka je `Spin.Printing` v26 i `rptDevRepx`, v23 i v9.1 se ne zovu (`docs/analiza/00-sazetak.md`). `PrintBridge` to još ne radi (`Placeholders.cs` 46–52). Nije provjereno da je klasa i dalje `Spin.Printing.Start`.
- **Posljedica:** učitati v23 „da bude kao VB“ krši odluku. Učitati samo v26 s drugim imenom metode ne ispiše. `repx:/` i `repx:` moraju oba skinuti prefiks prije imena izvještaja.
- **Test:** `KAR-32`, `KAR-33`, `ISP-01`

## R43 `vrstadokid-` nije ime forme

- **Element:** `:vrstadokid-mpblagajnagla`
- **NetGen:** vodeća `:` u `Rafalno` skine se i zove `NovaGenerickaForma2` (`Form2.vb` 6882–6883). Prefiks `vrstadokid-` uđe u `PrintRpt` (`Class1.vb` 443–462`), ne u traženje XML forme. Predložak je iz `vrstadok.template` (`471–486`). `#` na početku templatea diže `Form9` i izbornik.
- **Carpo:** nema `vrstadokid-`. `print` traži ime repx-a, ne red u `vrstadok`.
- **Posljedica:** `MULTISELECTISPIS_FORM` bez kontrola ne smije ostati prazan dijalog. Mora okinuti predložak vrste dokumenta.
- **Test:** `MPB-03`, `MPB-04`

## R44 `MULTISELECT` u imenu forme briše temp tablicu i prije ispisa

- **Element:** `MULTISELECTISPIS_FORM`, `MULTISELECT_ADAPTER_FORM_…`
- **NetGen:** `NovaGenerickaForma2` ako ime počinje s `multiselect` ili `multiblue` briše `TotalGenMultiSelectTemp` po `suser_sname()` i po `@@operateriid` (`Form1.vb` 4932–4936) prije nego što vidi da je posao ispis ili Carpo forma.
- **Carpo:** nema te tablice. Selekcija grida ne briše tuđe retke.
- **Posljedica:** ispis dokumenata i Carpo izmjena kupca obrišu temp retke operatera iako gumb nije `MULTISELECT` punjenje.
- **Test:** `MPB-05`, `BVG-04`

## Pokrivenost checklisti

| Checklista | Što je tamo različito od Carpa | Stavke |
|---|---|---|
| `OznakaKL.md` | prazno stablo, Janus, `GetID`, `PERMISIO`, širina `/14`, label `TEXT`/`BOLD` | R02, R06, R07, R34 |
| `JeziciNG.md` | dva G, `refreshmaster`, `TEXT` na gumbu nije natpis, širina 179, `PRINTREPXMASTER` | R04, R08, R07, R34, R41 |
| `Statusi.md` | adapter stabla, samo prvi korijen, id 0, prazna forma, detail kolone jednom, `v[` | R01, R05, R09, R19 |
| `KntBlaZak.md` | susjed nije roditelj, tri korijena, ime stupca, `CLEARCOLUMS` jednom, agregat `"0"` | R10, R11, R09, R08 |
| `PrijemniList.md` | combo umjesto `TREE`, `REFRESHMASTER`, `@robaid`, `SETDEFAULTS` po `#` | R03, R04, R13, R12 |
| `Kulture.md` | lijepljenje `sp`, `Cijena` text/number, tražilica, `spRaiserror` | R14, R15, R16, R17 |
| `KarticaPartnera.md` | `:FILTERFORM`, `!!`, `+focus`, multidrop, `firstdayofyear` | R18, R14, R17, R28, R24 |
| `KUPNG.md` | tilda, `REFRESHMASTER`, temp tablica, `FOREACH`, `OKSTAY`/`CLOSEPARENT`, Carpo `ONLOADED` | R20, R04, R21, R22, R23, R37, R38, R39 |
| `KUPDostKN.md` | shell DMS, popup s tildom, dijalog `CarpoForm` | R38, R39, R40 |
| `KUPDostKnNGBvg.md` | `clsCarpoKupUra`, gumb umjesto `ONLOADED`, `MULTISELECT_` ime | R37, R38, R44 |
| `MPBlagajna.md` | `PRINTMASTER`, `vrstadokid-`, prazna forma | R41, R42, R43, R44 |
| `KarticaPartnera.md` | `:repx/`, `PRINTMASTER`, orijentacija, v23 naspram v26 | R14, R17, R18, R24, R28, R41, R42 |
| `KntDospjeleStavke.md` | nema `DATAFIELD`, refresh kolona, hash search, multidrop, broj `0`, statusstrip | R26, R27, R16, R28, R15, R32 |
| `MPInventura.md` | `firstdayofmonth`, `DEFALUT`, `TODAY`, `SelectedValue=1`, radio, hash i `sp` search | R24, R31, R26, R29, R16 |
| `FakPar.md` | `cur` u tipu, prazan `ALLOWEMPTY`, `checkbox`, `VB.textbox`, `GRUOPBYBOXVISIBLE` | R15, R26, R29, R30, R31 |
| `MPNarIzdSkl.md` | `datetiem`, panel, `WUDTH`, multidrop `ALLOWEMPTY` | R24, R30, R33, R31, R28 |
| `DobUgovori.md` | `SHOWCHECKBOX`, `TRANSLATE`, `sp` search, group, status bez `TEXT2` | R25, R26, R16, R33, R32 |
| `MPNGPregDokMAIL.md` | nije Carpo subforma, `TREE33`, clipboard `false` | R35, R31, R18, R36 |
| `Kontrole-matrica.md` | InStr red i atributi iz tablice | R24–R34 |

## R45 Auto-filter red je uključen iako Janus šuti

- **Element:** master grid
- **NetGen:** `FilterMode` ostaje `None` dok XML nema `FILTERMODE=true` (`Form1.vb` 2169).
- **Carpo:** `ShowAutoFilterRow` je uključen osim eksplicitnog false (`ComponentGrid.cs` 772–773).
- **Odluka:** namjerno odstupanje kad `FILTERMODE` nema. Ako atribut postoji, vrijedi NetGen.
- **Test:** `oznaka-filter-carpo`, `filtermode-false-gasi`
- **PR:** #4

## R46 Log nije prsten `CBLog`

- **Element:** zapis događaja
- **NetGen:** `NapuniCBLog` drži 40 redova (`Class1.vb` 76–82). Greška zna ići u `spNetGenErrorLog`.
- **Carpo:** `NgDnevnik` piše u `Carpo.Core.Logger`. Ctrl+F11 vidi cijeli `LogList`. `Logger.Error` zove `spCarpoLogInsert`.
- **Odluka:** Carpo mehanika.
- **Test:** `log-preskok`, `log-greska`
- **PR:** #4, #9

## R47 Upis i `jupiter2022` se ne šalju

- **Element:** veza na bazu
- **NetGen:** `insert`/`update`/`delete` idu na server. Produkcija zna biti `jupiter2022`.
- **Carpo.NetGen:** `SqlTekst.JeZabranaPisanja` zaustavi upis. `SpinVeza` odbije ime koje sadrži `jupiter2022`.
- **Odluka:** namjerno odstupanje. Čitanje ide na `spin`.
- **Test:** `zabrana-insert`, `ne-jupiter2022`
- **PR:** #4

## R48 TestShell crta `DataGridView`, ugovor je DevExpress niz

- **Element:** prozor granule
- **NetGen:** Janus `GridEX`.
- **Carpo:** TestShell u `Podloga.cs` 79–111 crta `DataGridView` s NetGen defaultima. `MasterGrid.DxPostavke` je niz za DevExpress host.
- **Odluka:** Carpo mehanika za crtež. Ponašanje ostaje NetGen.
- **Test:** `dx-header-empty-select`
- **PR:** #4, #9

## R49 Ikona stabla nije `OrdersDetails16.ico`

- **Element:** `TreeView2.ImageList`
- **NetGen:** indeks 0, ključ `OrdersDetails16.ico` (`Form1.designer.vb` 575–579).
- **Carpo.NetGen:** datoteke nema. `Podloga` stavi zamjensku sličicu 16×16 i drži indeks 0.
- **Odluka:** Carpo mehanika za sliku.
- **Test:** `stablo-prevod`
- **PR:** #9

## R50 `GroupTotals=Always` nije mapiran

- **Element:** master i detail grid
- **NetGen:** dizajner stavi `GroupTotals=Always`. Svježi GridEX je `Never`.
- **Carpo:** to svojstvo nije preneseno na DX footer.
- **Odluka:** otvoreno.
- **Test:** nema
- **PR:** #4
