# Razlike NetGen / Carpo

Registar za implementaciju. QA dopisuje stavku čim vidi da isto ime ne znači isto ponašanje. Izvor istine za NetGen je `netgen/*.vb` i XML. Carpo citat je kod kontrole ili `docs/analiza/04-mapiranje-netgen-carpo.md` kad port tu kontrolu još ne crta. Port koji već prati NetGen nije ovdje ponovljen kao rupa.

Izgled je namjerno Carpo (R59). Checklist odsad ne traži Janus ni WinForms krom (boja, stil gumba, gumbi na dnu, prazni retci, row header, visina zaglavlja, font, obrub). I dalje se strogo gleda natpis i redoslijed gumba, što gumb radi, širina i skriveni stupac, sort, edit, checkbox stupac, raspored kontrola na dijalogu, tab, enable/visible i fokus. XML se ne mijenja.

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
- **Carpo:** `GridControl` nema te Janus početke. Port širinu računa u `MasterGrid.VbJedinica` (`MasterGrid.cs` 88–90). Ostalo (selection, `GetID` → ćelija 2) nije taj grid. Krom (row header, prazni retci, `HeaderLines`, highlight, font, obrub) je R59, ne rupa.
- **Posljedica:** OznakaKL `@@id` je `klmasterrobaid`, ne prvi vidljivi stupac. Širina 2500 je 179, ne 178. Izgled kontrole je DevExpress.
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

## R45 Dijaloško stablo nije podložno stablo

- **Element:** `INPUT TYPE="tree"` naspram `TREE` na projektu
- **NetGen:** dijalog (`Form2.vb` 2558–2608, `2825–2864`) reže SQL po `#` i `:`. `yearmonth` ima prazno tijelo (`2576–2577`). Tag je stupac 3 kako jest, bez `:ImeForme`. Parametar je cijeli tag (`3667–3671`). `ExpandAll`. Nema `TREETRANSLATE`. Podloga (`Form1.vb` 4474–4478, `4511–4514`) doda `:ImeForme` ako stupac 3 nema dvotočku, a `@@node` je dio prije `:`.
- **Carpo:** jedan `JTree` s `key`/`parentfield` po imenu (`ComponentTree.cs` 770–793). Nema dva ugovora taga.
- **Posljedica:** `RobaWebKlase` `@@klrobawebklasaid` dobije sirovi stupac 3. Isti SQL na podlozi dobije sufiks `:PUNI` i pokvari `int`.
- **Test:** `RIJ-11`, `RIJ-13`, `RIJ-40`

## R46 `yearmonth` na dijalog stablu ne crta godine

- **Element:** segment `yearmonth:` u `INPUT TYPE="tree"`
- **NetGen:** na dijalogu grana je prazna (`Form2.vb` 2576–2577). Na podlozi zove `DazovaVremenska` (`Form1.vb` 4348–4349), godine 2005–2009.
- **Carpo:** nema te riječi. Prazan segment i godine 2005–2009 nisu isto.
- **Posljedica:** prepis podložnog `yearmonth` na kontrolu dijaloga nacrta godine kojih u NetGenu nema.
- **Test:** `RIJ-40`

## R47 WebBrowser `URL` je velika slova

- **Element:** `TYPE="webbrowser"`
- **NetGen:** `Navigate` na `URL` nakon `ToUpper` (`Form2.vb` 1431–1433). Prije toga, ako `ONLOAD` ima kolonu imena kontrole, navigira njen tekst (`1419–1420`). `refresh` nije adresa (`4908–4913`).
- **Carpo:** `JLayout` nema `webbrowser` (`docs/analiza/04`).
- **Posljedica:** `RatArkod` otvara `preglednik.arkod.hr` iz `ONLOAD`, ne iz `URL`. Atribut `URL` bi host pretvorio u velika slova.
- **Test:** `RIJ-16`, `RIJ-41`

## R48 PictureBox: `LOCATION` velikim slovima, `http` zove `Load`

- **Element:** `TYPE="picturebox"`
- **NetGen:** `ImageLocation = LOCATION.ToUpper` (`Form2.vb` 2710). Kolona istog imena prepiše (`2712–2713`). `http` zove `Load` (`2715–2716`). `StretchImage` (`2726`), `SendToBack` (`2732`).
- **Carpo:** `pictureedit` nema ovo (`docs/analiza/04`).
- **Posljedica:** `slikalocation` traži kolonu tog imena. UNC iz `BlaBSUnos` postane velika slova. Preimenovanje kontrole ugasi sliku.
- **Test:** `RIJ-18`, `RIJ-19`, `RIJ-42`

## R49 `NEXTFORM` i dozvola prve forme

- **Element:** `NEXTFORM`
- **NetGen:** nakon OK, dok atribut nije prazan, zatvori formu i otvori sljedeću (`Form2.vb` 4431–4464). Id je prvi stupac submita (`4435–4438`). Provjera dozvole zove `Form2b.strImeForme` (`4459`), ime prve forme, ne sljedeće.
- **Carpo:** `form open` ne veže se na uspješan OK prethodne i ne nasljeđuje taj id sam.
- **Posljedica:** `DobUgovori` unos otvara izmjenu. `Narudzbe` `UnosStavkeNar` zove samu sebe dok je OK. Dozvola se ne pita za ime sljedeće forme.
- **Test:** `RIJ-21`, `RIJ-24`

## R50 DB clipboard nije Carpo stanje forme

- **Element:** `DBCLIPBOARD` / ime `filter`
- **NetGen:** ime koje sadrži `filter` upali clipboard osim ako je `DBCLIPBOARD` `false` (`Form2.vb` 1102–1103). `true` čita `NetGenDBClipboard` (`1170–1172`). Pad selecta ugasi clipboard. `CREATE TABLE` je komentar (`1173–1176`).
- **Carpo:** nema tu tablicu. `savevalue` na kontroli je drugi mehanizam.
- **Posljedica:** filter bez izričitog `false` pamti zadnji unos. `FILTERFORM` s `DBCLIPBOARD="false"` ne pamti. Ako tablice nema, NetGen ne stvara je.
- **Test:** `RIJ-28`, `MAIL-05`

## R51 `SETDEFAULTS` nije jedan SQL

- **Element:** `SETDEFAULTS`
- **NetGen:** rez po `#`, svaki komad `ExecSql` (`Form1.vb` 3862–3866, `3945–3962`). Kolona `formcaption` prepiše naslov (`3964–3967`).
- **Carpo:** jedan `data` s `#` u tekstu SQL Server odbije.
- **Posljedica:** `JeziciNG` je jedan select. `PrijemniList` ima više segmenata i padne ako se spoje.
- **Test:** `RIJ-26`

## R52 Ikona stabla je slika 0, `TREETRANSLATE` je samo podloga

- **Element:** `TREETRANSLATE`, `TreeView2`
- **NetGen:** prijevod stupca 2 samo ako je atribut `true` (`Form1.vb` 4461–4464, `4501–4504`). Statički čvor se prevodi uvijek (`4353`). Dijaloško stablo se ne prevodi. `TreeView2` ima `ImageList2` i indeks 0 za sve čvorove (`Form1.designer.vb` 575–579). Kod ne čita ikonu po čvoru. `ikonaid` je ikona retka grida (`7902–7905`). `ICON` je ikona forme (`3392`).
- **Carpo:** `tree` nema `ImageList2` ni prisilan indeks 0. Prijevod natpisa nije `TREETRANSLATE`.
- **Posljedica:** čvor ne dobije ikonu iz podatka ni iz `ikonaid`. `ImageList` indeks 0 je izgled – Carpo (R59). Uključiti `TREETRANSLATE` na dijalog stablu ne prevede ništa.
- **Test:** `RIJ-23`

## R53 Grid na dijalogu nije master

- **Element:** `INPUT TYPE="grid"`
- **NetGen:** Janus `Grid` (`Form2.vb` 1623), osim ako je `dxgrid` (`1579`). `SKIPDEFAULTLOAD="TRUE"` ne puni odmah (`1643–1646`). `CHECKBOXVISIBLE="FALSE"` sakrije stupac 1 (`1754–1755`). `SQL2`–`SQL4` se spreme velikim slovima (`1630–1632`). Suma je uključena osim `TOTALROW="FALSE"` (`1745–1748`). `PRINTGRID` i `PRINTREPXGRID` zovu isti ispis `"lodani"` (`6950`, `6985`, `9041`).
- **Carpo:** master grid i `grid` na formi dijele jedan DevExpress `GridControl` (R59). Nema `"lodani"` ni sakrivanja baš stupca 1.
- **Posljedica:** `klasa_da` na `RobaWebKlase` nema kvačice i puni se čim se forma čita. Sakriti stupac 0 sakrije id, a NetGen sakrije stupac 1.
- **Test:** `RIJ-14`

## R54 `FILLEXPLORER=onlytree` puni samo stablo

- **Element:** `onlytree`
- **NetGen:** prazna forma se ne prikaže (`Form2.vb` 4430). Vrijednost `onlytree` zove samo `NapuniDrvoVrijeme` (`Form1.vb` 5475–5476). Master SQL se ne šalje. `true` zamijeni master (`5445–5452`).
- **Carpo:** `control populate` nema ovu treću vrijednost. `CommandRunner` ispis još ne izvršava (`Placeholders.cs` 19–24).
- **Posljedica:** `LVObracun` i `JOPPD` gumb „Osvježi drvo“ ne smije ponovo poslati master. `STATUS` s `true` smije.
- **Test:** `LVO-04`, `JOP-03`

## R55 `ImeForme(@a=@@x)` nije ime forme

- **Element:** poziv forme s zagradom
- **NetGen:** `NovaGenerickaForma2` reže na `(` (`Form2.vb` 4407–4418). Lijevo je ime forme, unutra ide na početak `strParametriSvi`. `-freezid` u tom nizu zabrani novi id (`4413–4415`). Vodeća `:` i dalje otvara formu (`Form1.vb` 4862–4867).
- **Carpo:** `form open` ne parsira `@a=@@x` iz istog tokena.
- **Posljedica:** `LVObracun` `INSERTLVGLA(@hrkadroviid = 0)` traži formu `INSERTLVGLA`, a `0` je parametar. Cijeli string kao ime forme ne postoji.
- **Test:** `LVO-05`, `LVO-20`

## R56 `BUTTONS` prepiše samo prvih N gumba

- **Element:** `BUTTONS` i `CMDxCAPTION`
- **NetGen:** `CMD` se složi prvi (`Form1.vb` 3319–3330). Ako `BUTTONS` nije prazan, segment `j` prepiše gumb `j+1` (`3770–3796`). Kasniji `CMD` ostaju. `Button6` iz `BUTTONS` je tek šesti segment (`j = 5`).
- **Carpo:** jedan niz gumba. Nema ostatka `CMD` iza kraćeg `BUTTONS`.
- **Posljedica:** `KntKompVerizne` ima pet `BUTTONS` i još `CMD6`–`CMD8` `repx/`. Repx gumbi se vide. Brisati ih jer „BUTTONS zamjenjuje CMD“ makne ispis.
- **Test:** `KOV-04`, `KOV-05`

## R57 `enable` gleda natpis gumba

- **Element:** `ONLOAD2` i rafalni `select` na podlozi
- **NetGen:** stupac se traži po `Button.Text` (`Form1.vb` 3914–3921 i 4886–4896). To je natpis, ne `NAME`.
- **Carpo:** `control enable` veže se na `name`.
- **Posljedica:** `MPPonuda` `ONLOAD2` stupac `Dodaj` gasi gumb čiji je natpis Dodaj. Veza po `NAME` ne nađe stupac.
- **Test:** `MPP-04`, `MPP-20`

## R58 `CLICK(ime)` klikne tu kontrolu

- **Element:** `CLICK(`
- **NetGen:** `Rafalno` skine `CLICK(`, ostatak je ime, `Glbl_Click` na toj kontroli (`Form2.vb` 7151–7155). `KntBlaPromet` `ONLOADED="click(Ok)"` tako stisne gumb `ok`, čiji je `ONCLICK` `close#:REPX/@@template (@id=@@BLPrometGlaId)`.
- **Carpo:** nema `CLICK(ime)`. Zatvaranje forme ne pokrene `ONCLICK` tog gumba.
- **Posljedica:** ispis blagajne ostane na praznoj formi ako se `click(Ok)` preskoči. Ime izvještaja je vrijednost `@@template` iz `ONLOAD`.
- **Test:** `KBP-04`, `KBP-20`

## R59 Izgled je Carpo, ponašanje je NetGen

- **Element:** podloga, gumbi, kontrole, skin. Namjerno odstupanje.
- **NetGen:** podloga je WinForms `Form1`. Gumbi `Button1`–`Button8` stoje na dnu, širina 79, razmak 8 (`Form1.vb` 3312, 3827–3848). Dijalog drži Prihvati/Odustani u panelu (`Form2.vb` 2750–2757). Master je Janus `GridEX` kad `DXGRID` nije true (`Form1.vb` 1906–1908): `HeaderLines=3`, `EmptyRows`, `RowHeaders`, `HideSelection=Highlight` (`Class1.vb` 1000, 1364, `Form1.designer.vb` 640, 655). `BORDERSTYLE` se na formi ne čita, ostaje WinForms `Sizable`. `SIZE` mijenja font (`Form2.vb` 3077–3081). Čvor stabla crta `ImageList2` indeks 0 (`Form1.designer.vb` 575–579).
- **Carpo:** podloga nasljeđuje Carpo `RibbonBase` (`Devex_26/Carpo.UI/Forms/Jupiter/Base.cs`). Gumbi podloge idu u ribbon. Sve kontrole, uključujući dijalog i editore, su DevExpress. Skin i komponente su Carpo gdje postoje. XML se ne mijenja i ne prevodi se u Carpo XML.
- **Posljedica:** QA ne traži boju, stil gumba, položaj gumba dolje, prazne retke, row header, broj redova zaglavlja, highlight bez fokusa, font, obrub ni sliku čvora. Pad nije ako to ne izgleda kao Janus. I dalje se gleda natpis i redoslijed gumba, naredba gumba, širina i skriveni stupac, sort, editabilnost, checkbox stupac, raspored kontrola na dijalogu, tab, enable/visible i fokus.
- **Test:** `OKL-38`, `OKL-45`, `OKL-54`, `OKL-55`, `OKL-56`, `OKL-78`, `JEZ-49`, `JEZ-54`, `JEZ-60`, `JEZ-69` su oznaka, ne pad.

## R60 `CMDxCAPTION` bez dvotočke nije gumb

- **Element:** `CMD7CAPTION` / `CMD8CAPTION` na podlozi (`MPPorKPNG` `Lista`, `Print dok.`)
- **NetGen:** `IndexOf(":")` je −1, `Substring` baci, `On Error Resume Next` ostavi tekst prazan (`Form1.vb` 3320–3333). `Button7` i `Button8` se prije toga ne prazne (`3118–3122` čisti samo 1–6), a dizajner im ne da tekst. Prazan tekst sakrije gumb (`3834–3835`).
- **Carpo:** parser takav zapis stavi u `PreskoceniGumbi` (`NetGenXmlReader.cs` 276–280) i ne napravi gumb. Ljuska koja od cijelog teksta napravi natpis bez naredbe pokaže `Lista` i `Print dok.`
- **Posljedica:** ta dva gumba se ne vide i ništa ne pokreću. Popup ih nema.
- **Test:** `MPK-06`, `MPK-07`

## R61 `TABINDEX` s razmakom i dalje je taj broj

- **Element:** `TABINDEX` na `INPUT`
- **NetGen:** u fajlu je `1` i tab. `XmlDocument` tab u atributu pretvori u razmak, vrijednost je `"1 "`. Dodjela u `Integer` (`Form2.vb` 3027–3030) razmak proguta, indeks je 1, `TabStop=True`. Grana za nedostajući indeks (`3038–3041`) se ne pali. `Enabled=False` gasi `TabStop` (`3046`).
- **Carpo:** `int.Parse` bez trima na `"1 "` baci. Forma padne ili kontrola ostane bez taba.
- **Posljedica:** `datumdok` i `napomena` dijele indeks 1. Redoslijed među jednakim indeksima je red dodavanja: prvo datum, zatim napomena, pa 2, 3 i 4.
- **Test:** `MPK-21`, `MPK-40`

## R62 `VALUE` na datumu nije početna vrijednost

- **Element:** `INPUT TYPE="date"`
- **NetGen:** čita se `DEFAULT`, ne `VALUE` (`Form2.vb` 2251–2267). Prazan `DEFAULT` u `Value` baci, ostane danas. `CHECKBOX` se ne čita; kvačica je `SHOWCHECKBOX` (`2275–2278`). Bez nje datum je uključen. U parametru je `'yyyyMMdd'` (`3530–3532`). `DECIMALS` na broju je `DecimalDigits` (`2086`); početni tekst je 0 (`2084`).
- **Carpo:** `value` na kontroli je početna vrijednost. `DATE1` / `DATE2` / `DATE` bi postali tekst, ne danas.
- **Posljedica:** `MPPorKPNG` filter i unos šalju današnji datum, ne riječ `DATE1`. Broj krene od 0 s četiri decimale.
- **Test:** `MPK-21`, `MPK-22`, `MPK-41`

## R63 Prazan `FILLEXPLORER` nije `true`

- **Element:** `FILLEXPLORER` na dijalogu
- **NetGen:** `true` zamijeni master SQL-om `ONSUBMIT` plus parametri (`Form1.vb` 5445–5452). Prazan string padne u `Else`: `ExecSql` toga SQL-a, zatim ponovi zadnji master (`5509–5519`). `ONSUBMIT` koji ne počinje s `INSERT`/`DELETE`/`UPDATE`/`SELECT` ne briše zalijepljene parametre (`3378–3388`).
- **Carpo:** nema tu granu. Oba oblika lako postanu isti refresh.
- **Posljedica:** `FILTERFORM` (`TRUE`) napuni grid. `DODAJIZNOS` (prazan) izvrši `spMPPoravnajKP` i osvježi stari master, ne postavi novi upit.
- **Test:** `MPK-30`, `MPK-31`

Implementacijski registar na `cursor/netgen-otvaranje-3266` završava s R64. Sljedeće stavke su R65.

## R65 `COMMANDTIMEOUT` na projektu je timeout SQL-a

- **Element:** `COMMANDTIMEOUT`
- **NetGen:** default je `"30"` (`Module1.vb` 177). Atribut na projektu prepiše `strCommandTimeout` (`Form1.vb` 3463). `ExecSql` ga stavi na `SqlCommand` (`Class1.vb` 99).
- **Carpo:** grid i dalje može ostati na 30 s.
- **Posljedica:** `Intrastat` čeka 300 s, `KntOPZ-STAT-1` 240 s. Kraći timeout prekine obrazac.
- **Test:** `INT-08`, `OPZ-08`

## R66 `TOTALROW` nije `TOTALROWVISIBLE`

- **Element:** suma na masteru
- **NetGen:** gasi se samo `TOTALROWVISIBLE="FALSE"` (`Form1.vb` 2049–2053). `TOTALROW` se na podlozi ne čita.
- **Carpo:** lako pročita `TOTALROW` kao total.
- **Posljedica:** `MPKntNacinPlacAzur` ima `TOTALROW="FALSE"`, a `TOTALROWVISIBLE` nema. Suma ostaje uključena.
- **Test:** `NPL-04`

## R67 `Ime.Start(a ; b)` nije `ASSEMBLYLOAD`

- **Element:** naredba gumba
- **NetGen:** `ASSEMBLYLOAD(` je posebna grana (`Form1.vb` 4584). `OpzStatExport.Start(@@datumDoParam ; @@datdospparam)` nema taj prefiks. Ime se reže na `(` (`Form2.vb` 4407–4418), forma `OpzStatExport.Start` ne postoji, 0 kontrola, dijalog se ne otvori (`Form1.vb` 5369–5370). Točka-zarez ostane u parametrima koji se ne izvrše.
- **Carpo:** `Ime.Start` lako postane učitavanje DLL-a, kao pravi `ASSEMBLYLOAD` na `KntPDVObrasciEU`.
- **Posljedica:** Eporezna na OPZ-STAT-1 ne diže DLL. EPorezna na PDV obrascima diže `EPoreznaObrasci.Program`.
- **Test:** `OPZ-06`, `PDV-06`

## R68 `ONENTERKEY` nije `EnterKeyBehavior`

- **Element:** Enter na master gridu
- **NetGen:** prazan `EDITCOLUMNS` stavi `EnterKeyBehavior=None` (`Form1.vb` 3419–3421). `ONENTERKEY` se i dalje čita (`3364`). `GridEX1_KeyDown` na Enter zove `NovaGenerickaForma` (`7021–7023`).
- **Carpo:** `Enter=None` proguta tipku i ne otvori formu.
- **Posljedica:** `KntPDVObrasciEU` na Enter otvara `IZMIJENI_FORM`.
- **Test:** `PDV-05`

## R69 `POSITION` prepiše red stupca

- **Element:** `COLUMN POSITION`
- **NetGen:** ako je `CLEARCOLUMS` true, stupac prvo dobije tekući red (`Form1.vb` 7516–7518). Ako `POSITION` nije prazan, taj broj prepiše red (`7526`).
- **Carpo:** red stupaca ostane red iz rezultata ili iz XML-a, bez drugog upisa.
- **Posljedica:** `KntOPZ-STAT-1` `FormatId` ide na poziciju −1 i `VISIBLE=FALSE`.
- **Test:** `OPZ-07`

## R70 `FILLEXPLORER` na projektu se ne čita

- **Element:** `FILLEXPLORER` na `<PROJECT>`
- **NetGen:** `Form1` taj atribut ne čita. Vrijednost vrijedi na formi koja se zatvori (`Form2.vb` 1092, `Form1.vb` 5445).
- **Carpo:** atribut na projektu lako se protumači kao da master odmah puni sebe.
- **Posljedica:** `KntOPZ-STAT-1` ima `FILLEXPLORER="TRUE"` na projektu. Master se puni tek kad `UPIT` (koji isto ima `TRUE`) vrati SQL.
- **Test:** `OPZ-04`

## R71 `ONLOAD` s `#` je lanac, ne jedan SQL

- **Element:** `ONLOAD` na projektu
- **NetGen:** cijeli tekst ide u `NovaGenerickaForma` ako počinje s `:` (`Form1.vb` 3879–3880). Tamo se reže na `#` (`4558–4561`). Prvi segment `:FILTERFORM` otvori formu (`4862`). Sljedeći segmenti su posebne naredbe.
- **Carpo:** `onload` lako ostane jedan string i ne odradi `enable`/`disable` iza filtera.
- **Posljedica:** `Bilanca-GFI` nakon filtera gasi `TSI` ili `GFI` ovisno o `@@izvjestaj`.
- **Test:** `GFI-03`

## R72 `#ISEMPTY` u zaglavlju ispisa

- **Element:** `PAGEHEADERLEFT`
- **NetGen:** `Rafalna_Priprema` reže na `#` (`Form1.vb` 7744–7768). Segment `ISEMPTY(...)` koji je prazan ili `null` preskoči sljedeći komad. Inače preskoči dva i uzme treći. `**` postane razmak (`7764`).
- **Carpo:** zaglavlje nema tu granu. `#` ostane u tekstu ili sruši ispis.
- **Posljedica:** `RealizacijaStavkeNG` lijevo zaglavlje ispiše period, skladište i partnera samo kad vrijednost postoji.
- **Test:** `REA-05`

## R73 `.clsGFIPOD` nije `.clsreport`

- **Element:** drugi segment gumba GFI/TSI
- **NetGen:** ispis ulazi samo ako ime počinje s `repx/` ili sadrži `.clsreport` (`Form1.vb` 5215). `AOPuExcel.clsGFIPOD(...)` ne zadovoljava. Ime lijevo od `(` traži se kao forma (`4407–4418`). Forme nema, dijalog se ne otvori (`5369–5370`).
- **Carpo:** nastavak `.cls` lako ode u isti ispis kao `.clsReport`.
- **Posljedica:** `Bilanca-GFI` GFI/TSI otvore `GFIPODFORM` / `TSIPODFORM`, a drugi segment ne ispisuje Excel. DLL je samo `ASSEMBLYLOAD(AOPuExcel64.Class1)`.
- **Test:** `GFI-05`

## R74 Podloga je Manager ponuda, ne Office ribbon

- **Element:** izgled prozora granule. Namjerno odstupanje, pojašnjenje R59.
- **NetGen:** `Form1` je WinForms. Gumbi su na dnu, stablo lijevo, master desno, detail ispod.
- **Carpo:** klasa je `Carpo.UI.Forms.Master.Manager`, baza `JupiterManager` → `JupiterShared` → `Jupiter.Base` → `RibbonBase.RibbonBase` (`Manager.cs` 30, `Base.cs` 16). Uzorak je `definitions/carpo/AIOPonuda.xml`, forma `frmMaster`, natpis `SPIN - Manager ponuda`. Raspored: `splitMain` lijevo (`cmbComboEditFilter`, `trMain`), desno `gcGla` s `bar2` (ikone 16 px), `splitGrid` detail ispod, `pnlButtons` na dnu, status `bbiBrojSlogova` = `Broj slogova:` (`Manager.Designer.cs` 138–158, 381–400, 641–719, `JupitetShared.Designer.cs` 122–126).
- **Posljedica:** port ne stavlja gumbe u Office ribbon s velikim ikonama. Donji red ostaje. Lijevi combo i stablo ostaju. Status pokazuje broj slogova. Naredbe, SQL i redoslijed gumba ostaju NetGen.
- **Test:** `docs/granule/Izgled-Carpo-checklista.md`

Implementacija `cursor/netgen-gfi-ispis-3266` (`be99aa7`) završava s R74. QA od R75 ide dalje, da se brojevi ne sudare.

## R75 `DETAILDBLCLICK` nije dvostruki klik mastera

- **Element:** `DETAILDBLCLICK`
- **NetGen:** čita se u `strDBlClickDetForm` (`Form1.vb` 3390). `GridEX2_DoubleClick` zove `NovaGenerickaForma` s tim imenom (`7201–7202`). Master i dalje gleda `MASTERDBLCLICK` (`6935`).
- **Carpo:** jedan događaj dvostrukog klika lako otvori master formu i na detailu.
- **Posljedica:** `MPKaseOperNG` na detailu otvara `UPDATE_FORM`. Klik na master to ne radi.
- **Test:** `KAS-04`

## R76 `PAGEORIENTATION=PORTAL` nije landscape

- **Element:** `PAGEORIENTATION`
- **NetGen:** default je `PORTRAIT` (`Form1.vb` 50). Atribut se učita (`3458`). Orijentaciju promijeni samo `LANDSCAPE` (`5930`). `PORTAL` ne ulazi u tu granu.
- **Carpo:** nepoznata riječ lako se protumači kao pejzaž ili se baci.
- **Posljedica:** `MPKnjigaPopisa` ostaje uspravno. `LANDSCAPE` i dalje vrijedi samo za Janus `IspisNovi`, ne za `PRINTMASTER` (R41).
- **Test:** `KNP-03`

## R77 `REFRESHAFTERCHILD` na projektu se samo upiše

- **Element:** `REFRESHAFTERCHILD` na `<PROJECT>`
- **NetGen:** vrijednost ide u `strREFRESHAFTERCHILD` (`Form1.vb` 3396). Na podlozi se taj string dalje ne čita. Na kontroli ga čita `SrediKontrolu` (`Form2.vb` 3058).
- **Carpo:** atribut na projektu lako osvježi master nakon svakog dijaloga.
- **Posljedica:** `KOD_RSPostavke` ima `REFRESHAFTERCHILD="TRUE"` na projektu. Sam taj atribut master ne osvježava.
- **Test:** `KOD-05`

## R78 `INCLUDE` spoji tuđi PROJECT u isti dokument

- **Element:** `INCLUDE` na formi
- **NetGen:** ako je globalni include prazan, čita se s forme (`Form2.vb` 844–849). Vrijednost se cijepa po `#` (`863`). Ime datoteke zamijeni zadnji segment putanje (`869`). Prvo `SpinDMSUtil.NetGenFinder.GetFile`, pa `XmlDocument.Load` (`905–908`). Djeca `/PROJECT` se uvezu (`916–934`). `On Error Resume Next` je na ulazu u `ProcitajXML` (`706`). Datoteka koja ne postoji ostavi formu kakva je napisana (`913`).
- **Carpo:** atribut se spremi (`NetGenXmlReader.cs` 118). Drugi XML se ne učitava i kontrole se ne dodaju.
- **Posljedica:** `MPStanje` `MAINFORM` traži `MPKarticaRobe.xml` u istom folderu. Te datoteke nema. Kad stigne, njeni čvorovi moraju ući u dokument prije crtanja `MAINFORM`.
- **Test:** `MPS-04`

## R79 `DETCHECKBOXVISIBLE` nije kvačica mastera

- **Element:** `DETCHECKBOXVISIBLE` / `DETTOTALROWVISIBLE`
- **NetGen:** oba se učitaju velikim slovima (`Form1.vb` 3461–3462). Nakon detaila, `FALSE` sakrije `GridEX2` stupac 1 i ugasi sumu (`2402–2412`). Prazan string ostavi stupac vidljiv i sumu uključenu. Master gleda `CHECKBOXVISIBLE` (`2043–2052`).
- **Carpo:** jedan grid lako dijeli jednu zastavicu. Sakriti stupac 0 sakrije id.
- **Posljedica:** `Statusi` gasi selector detaila, a master ima svoju zastavicu. `VrCjenik` i `InstrumentiPlacanjaNG` imaju `FALSE`, ali nemaju `DETAILGRID`, pa grana pod `On Error Resume Next` (`2400`) ne crta detail.
- **Test:** `STA-38`, `VRC-04`

## R80 `AUTOFITCOLUMNS` se ne čita

- **Element:** `AUTOFITCOLUMNS`
- **NetGen:** `GetNamedItem("AUTOFITCOLUMNS")` nema u `netgen/*.vb`. Širina ostaje ona iz `Class1.vb` (`1087–1244`). `False` i `True` ne mijenjaju grid.
- **Carpo:** `BestFitColumns` suzi stupce na sadržaj.
- **Posljedica:** `MPPSNG` ima `AUTOFITCOLUMNS="False"`. To ne gasi autofit, jer autofita nema. Širine ostaju izračunate.
- **Test:** `MPSNG-05`

## R81 `HELPKOD` pali JupiterHelp

- **Element:** `HELPKOD`
- **NetGen:** neprazan kod pokaže `ButtonHELP` (`Form1.vb` 3491–3494). F1 (`1356–1367`) i klik (`7777–7778`) zovu `DajPokaziHelp`. Proces je `JupiterHelp.exe` s `-h` i kodom (`7792–7799`). Prazan kod sakrije gumb.
- **Carpo:** Manager ponuda nema taj gumb (R74). Kod se spremi u test kuku, proces se ne pokreće.
- **Posljedica:** `InstrumentiPlacanjaNG` `HELPKOD="MER252"` na F1 otvara help. Ikona `credit_card.ico` ostaje izgled – Carpo (R74).
- **Test:** `INS-04`

## R82 `REFRESHAFTERCHILD` na formi se ne čita

- **Element:** `REFRESHAFTERCHILD` na elementu forme
- **NetGen:** projekt ga samo upiše (`Form1.vb` 3396, R77). Kontrola ga čita u `SrediKontrolu` i, ako je točno `TRUE`, doda red u `Cdt` (`Form2.vb` 3058–3066). Atribut na `<UNOS_VRC>` nije ni projekt ni kontrola.
- **Carpo:** `TRUE` na formi lako osvježi master nakon zatvaranja dijaloga.
- **Posljedica:** `VrCjenik` unos i izmjena ne osvježavaju master samo zato što forma nosi taj atribut.
- **Test:** `VRC-05`

## R83 Provjera checklistе na Manager rasporedu

- **Element:** `docs/granule/Izgled-Carpo-checklista.md`. Build `gfi-ispis` (`b46b32c`) sadrži `Podloga : Carpo.UI.Forms.Master.Manager` (`4267122`).
- **Drži:** nema ribbon stranice. Combo je `cmbComboEditFilter`, stablo `trMain`. Lijeva strana se sakrije kad nema `TREE` ni `COMBO`. Master je `gcGla`. `bar2` je iznad. Detail je ispod, kartica Stavke. Gumbi su u `pnlButtons`. Status počinje s `Broj slogova:`.
- **Odstupanje:** nema kartica Dimenzije, Zaliha i DMS iz `AIOPonuda.xml`. NetGen ima jedan `DETAILGRID`. Ikona čvora je indeks 0, sivi krug iz `JTreeListControl.OnCreateControl`, ne ikone danas/tjedan.
- **Odluka:** namjerno odstupanje, izgled.
- **Test:** nema slike.
- **PR:** dblclick

## R84 `FILTER` na projektu je forma

- **Element:** 14 redova `nema-forme` u smokeu
- **NetGen:** `SelectNodes("/PROJECT/" & strImeForme)` (`Form2.vb` 957). Ime je element, ne atribut `NAME`.
- **Carpo:** parser je `FILTER` držao strukturnim, pa `/PROJECT/FILTER` nije bio forma. To je naš propust. `MPUvjetiKaseNEW` ima element `FILTERFORM1` i `NAME=FILTERFORM`, a `ONLOAD` traži `FILTERFORM`. `MPPregRbTrNG2` traži `FORM1`, element je `FILTERFORM`. `MPOptimalneZaliheNEWv2` traži `FILTERFORM`, element je `MAINFORM`. Te tri NetGen isto ne nađe.
- **Odluka:** NetGen logika. `FILTER` se čita kao forma. Tri datoteke bez tog elementa ostaju `nema-forme`.
- **Test:** `filter-je-forma`
- **PR:** filter-forma

## R85 `spMPTrgovineNG` nema parametar `@operateriid`

- **Element:** smoke `ostalo`, `MPPovijestPrijava.xml`
- **NetGen:** `SrediSQL` zamijeni `@@operateriid` i pošalje isti tekst (`Form1.vb` 2482).
- **Carpo:** upit je `spMPTrgovineNG @action=1, @operateriid = 7`. Spin javi da procedura nema taj parametar. Port tekst nije prepisao.
- **Odluka:** NetGen logika. XML se ne mijenja.
- **Test:** nema
- **PR:** filter-forma

## R86 `MPKasaPolog` ima dvostruke stupce u SQL-u

- **Element:** drugi smoke `ostalo`
- **NetGen:** šalje `ONLOAD` kako je napisan.
- **Carpo:** spin javi `Ambiguous column name 'odsat'`, `odmin`, `dosat`, `domin`. Isti tekst.
- **Odluka:** NetGen logika. XML se ne mijenja.
- **Test:** nema
- **PR:** filter-forma

## R87 `oznaka2` nije stupac u `klmasterroba`

- **Element:** smoke `krivi-stupac`, `OznakaKL.xml`
- **NetGen:** master je `select ... oznaka2 as DodatnaOznaka from klmasterroba`.
- **Carpo:** spin javi `Invalid column name 'oznaka2'`. Zato QA stavke OznakaKL koje traže napunjen grid ostaju otvorene.
- **Odluka:** NetGen logika. Upit se ne prepravlja.
- **Test:** nema
- **PR:** filter-forma

## R88 Prazan master na uzorku od 20

- **Element:** 413 praznih u smokeu repoa
- **Uzorak:** Akcije, OdobreniKaloNG, MPPonudaWeb, UcinakObr_Agrar, MER_ZahtjevZaNabavkuNG (nema master `ONLOAD`, ima `TREE` ili `COMBO`). KntOPZ-STAT-1, MPKasaImport, MPPDV, MPPrMPCNG, RealizacijaPonuda, ZakljucivanjeTemNG, kntDobDospRac, MPAnalKNG, MPePotvrdeNarudzbe, MPKntNacinPlacAzur, MPNeZakNG, MPPregKlaseStanja, MPPrMPRNG, MPTestKomisRobe, MPTrgovinaPortal (`ONLOAD` je `:forma` ili prazan).
- **NetGen:** prazan `ONLOAD` ne šalje master. `:forma` otvori formu i master čeka `FILLEXPLORER`. Stablo se ne klikne samo.
- **Carpo:** isti predložak. Nismo poslali drugačiji parametar ni filter. Prazno je podatak ili čekanje na čvor ili formu, ne kriva zamjena.
- **Odluka:** NetGen logika.
- **Test:** nema
- **PR:** filter-forma

## R89 Jupiter smoke ne pokreće proceduru koja piše

- **Element:** EXEC na `jupiter2019` i `jupiter2022`
- **NetGen:** `JeZabranaPisanja` gleda prvu riječ. `spKntZakljucivanjeTem @action=1` i `spKntPreknjizavanjeNG @action=2` nisu `insert`/`update`/`delete`, pa bi se poziv poslao. Tijelo piše u trajnu tablicu. To su naredbe gumba (`ONSUBMIT`), ne load.
- **Carpo:** smoke prije svakog EXEC-a čita `sys.sql_modules` i rekurzivno tijela koje procedura zove. INSERT/UPDATE/DELETE/MERGE/TRUNCATE/DROP nad tablicom koja nije `#temp` ili `@varijabla` se ne pokreće. Oznaka je `procedura-piše`.
- **Posljedica:** `ZakljucivanjeTemNG` i `KntPreknjizavanjeNG` na otvaranju te procedure ne zovu. Ako se pojave u load putu, ostanu neizvršene. Smoke prije ove zaštite nije pokrenut. Nijedna procedura nije izvršena.
- **Odluka:** zaštita smokea. XML, tablice i procedure se ne diraju.
- **Test:** `zastita-zakljucivanje`, `zastita-preknjizi`, `otvaranje-nema-zakljucivanje`, `otvaranje-nema-preknjizi`
- **PR:** jupiter-smoke

## R90 Klik na dijalogu ide kroz lanac, upis ne

- **Element:** `ONCLICK` gumba na formi
- **NetGen:** `Rafalno` (`Form2.vb` 6884–6890). `OKStay` ostaje i fokusira. `OK` zatvara i osvježi master. `ok` je OKClick pa `NEXTFORM`. `CLOSE` ne šalje SQL.
- **Carpo:** isti lanac za ostanak, zatvaranje, fokus i `refreshMaster`. `sqlImaZalijep` je false: ONCLICK je Rafalno, ne drugi niz `@ime=`. SELECT se pošalje. Poziv procedure koji piše se ne šalje (R89).
- **Odluka:** NetGen logika za tok. Upis ostaje zaštićen.
- **Test:** `dijalog-ostaje`, `dijalog-ok`, `dijalog-close`, `dijalog-refresh`
- **PR:** dijalog-gumb

## R91 Combo i grid na dijalogu šalju svoj SELECT

- **Element:** `SQL` na combo i grid kontrole forme
- **NetGen:** pri otvaranju forme pokrene SQL kontrole. `ONSUBMIT` čeka gumb.
- **Carpo:** combo (`LookUpEdit`) i grid dobiju rezultat ako je tekst `SELECT` i ne zove proceduru. Poziv procedure i dalje čeka provjeru tijela (R89), pa se ne šalje iz dijaloga.
- **Odluka:** NetGen logika za SELECT. Upis i neprovjerena procedura ostaju zaštićeni.
- **Test:** `dijalog-combo-sql`
- **PR:** dijalog-sql

## R92 Dijalog: ONCLICK, FILLEXPLORER, combo, grid, NEXTFORM

- **Element:** gumb, combo, grid i `NEXTFORM` na formi
- **NetGen:** `ONCLICK` ide u `Tag` i `Glbl_Click` (`Form2.vb` 1345–1349, 6221–6228). `:Zakljucavanje` otvara tu formu, ne zatvara roditelja. `CMD1` bez dvotočke je OK (`3367–3396`). `FILLEXPLORER=TRUE` pošalje `ONSUBMIT` u master (`Form1.vb` 5445–5452). Combo puni `SQL` (`Form2.vb` 2396–2431). Grid puni `SQL` osim `SKIPDEFAULTLOAD=TRUE` (`1635–1646`). `NEXTFORM` se čita (`1052`) i nakon OK otvara tu formu (`4431`).
- **Carpo:** isto za tok. Na `jupiter2019` i `jupiter2022` gumb i `ONSUBMIT` se ne šalju. Combo i grid koji su čisti `SELECT` smiju se poslati pri otvaranju. Procedura u njihovom SQL-u čeka provjeru tijela i s Jupitera se ne šalje.
- **Posljedica:** `ZakljucivanjeTemNG` Zaključi otvara `Zakljucavanje`. `MPRealizNG` Pokaži drži `spMPRealizacijaRobeNG`, ali ga Jupiter ne izvrši. `MPNivelacija` combo je `spMPDajTrgovinaNG`. `JeziciNG` `NEXTFORM` je `Dodaj`. `PROMET` `NEXTFORM` je prazan.
- **Odluka:** NetGen logika. Upis i Jupiter gumb ostaju zaštićeni.
- **Test:** `rupa-zakljuci`, `rupa-pokazi`, `rupa-combo`, `rupa-grid`, `rupa-next`, `rupa-niv-combo`, `rupa-niv-pokazi`, `rupa-jupiter-nema-onsubmit`
- **PR:** dijalog-rupe

## R93 Combo na dijalogu: prazan red, stupac 0/1, refresh

- **Element:** combo `SQL`, `ALLOWEMPTY`, `ONCLICK` refresh
- **NetGen:** čim se kontrola složi, `ExecSql` puni combo (`Form2.vb` 2396–2431). Stupac 0 je vrijednost, stupac 1 prikaz. `ALLOWEMPTY` je samo `true`. Inače se na prazan odabir pokuša `SelectedValue=1` (`3455–3464`). `select 'refresh' as ime` ponovo puni tu kontrolu, ne onu koja je kliknuta (`5158–5176`).
- **Carpo:** isto pravilo. `FOLSE` i nedostajući atribut ne ubace prazan red. `@@operateriid` se zamijeni. `@@trgovinaid` koji ostane ne šalje se. Refresh je `ONCLICK`: na jupiter2019 i jupiter2022 se ne šalje. Multidrop nije combo.
- **Posljedica:** `MPRealizNG` trgovina ima prazan red. `MPInventura` sort nema. `MPKntNacinPlacAzur` `FOLSE` nije prazan red, vrijednost 1 je «Samo za MP». `MPNivelacija` `TRUE` jest prazan red.
- **Odluka:** NetGen logika. Jupiter ne šalje ONCLICK.
- **Test:** `combo-1-prazan` do `combo-10-nula`, `combo-prazan-red`
- **PR:** combo-prazan

## R94 `ONCLICK` je lanac, `exec` se ne lijepi

- **Element:** `ONCLICK` i `ONSUBMIT` koji počinje s `exec`
- **NetGen:** `ONCLICK` ide u `Tag` i `Rafalno` (`Form2.vb` 6221–6228, 6806). `!!` skine lijepljenje (`Form1.vb` 5511–5512). `exec sp...` nije `sp` na početku, pa se drugi niz `@ime=` ne dodaje.
- **Carpo:** segmenti se kupe. `:forma` otvara formu. `OK` zatvara. Na jupiter2019 i jupiter2022 nijedan `ONCLICK` ni `ONSUBMIT` se ne šalje. `exec` i `!!` ostanu bez drugog niza.
- **Posljedica:** `MPInventura` Stavke šalje `spMPInventuraNG` pa `OK`, ali ne s Jupitera. `KntPreknjizavanjePartnera` `exec spKntPreknjizavanjePartnera @Action=1` ne dobije `@PartnerSTARIid=9` na kraj.
- **Odluka:** NetGen logika. Jupiter gumb se ne izvršava.
- **Test:** `onclick-jupiter`, `onclick-exec-ne-lijepi`
- **PR:** onclick-lanac

## R95 Grid na dijalogu puni SQL osim SKIPDEFAULTLOAD

- **Element:** grid `SQL` na formi
- **NetGen:** bez `SKIPDEFAULTLOAD=TRUE` grid zove `ExecSql` čim se složi (`Form2.vb` 1635–1646). `TRUE` ostavi grid prazan.
- **Carpo:** isti uvjet. `MPRealizNG` `PROMET.stavke` šalje `@action=2`. `BSDok` grid s `SKIPDEFAULTLOAD` ne šalje SQL. To nije `ONCLICK` ni `ONSUBMIT`, pa Jupiter i dalje ne izvršava gumbe.
- **Odluka:** NetGen logika.
- **Test:** `grid-puni`, `grid-skip`
- **PR:** grid-dijalog

## R96 `NEXTFORM` se čita i nakon OK ostaje taj tekst

- **Element:** `NEXTFORM`
- **NetGen:** atribut se upiše u `strNextForm` (`Form2.vb` 1052). Nakon OK-a, dok nije prazan, otvara se ta forma (`4431–4458`).
- **Carpo:** čita se cijeli tekst, uključujući `ButtonKartica:KARTICA`. Prazan atribut ostaje prazan. Na jupiter2019 i jupiter2022 se pritom ne šalje `ONSUBMIT`.
- **Posljedica:** jedini ulazni `NEXTFORM` je `BIOVEGA/KomStanjeSkl.xml`. `MPRealizNG` `PROMET` je prazan.
- **Odluka:** NetGen logika. Jupiter gumb se ne izvršava.
- **Test:** `next-kom`, `next-prazan`
- **PR:** nextform

## R97 Master se osvježi i na `refreshmaster`, i nakon OK

- **Element:** lanac na ljusci i `NEXTFORM` nakon potvrde
- **NetGen:** `REFRESHMASTERGRID` zove `PokaziUpit` (`Form1.vb` 4570). `refreshmaster` i `REFRESHMASTER` nisu ta riječ, ali prazna forma ima `OKClick` pa se zadnji master ipak ponovi (`5354–5370`, `5519`, `Zadnji` je true). `NEXTFORM` se otvara dok nije prazan (`5381`). `Odustani` i Escape bez `ONESCKEY` ne šalju SQL (`3400`, `4161`).
- **Carpo:** ljuska osvježi master na obje riječi i nakon OK. Forma koje nema ne baca poruku. `prihvati`, `odustani` i `escape` stižu do kuke. Na jupiter2019 i jupiter2022 se taj master ne šalje.
- **Posljedica:** `JeziciNG` Unesi ima `#refreshmaster` i `NEXTFORM=Dodaj`. `OznakaKL` Prihvati osvježi master, Odustani ne. `update` ne lijepi drugi niz, apostrof ide kao `''`.
- **Odluka:** NetGen logika. Jupiter se ne dira.
- **Test:** `jez-refreshmaster`, `jez-next`, `okl-prihvati`, `okl-odustani`, `okl-escape`
- **PR:** refresh-ok

## R98 Detail se veže na red, širina je WIDTH/14

- **Element:** `DETAILGRID`, `DETCHECKBOXVISIBLE`, `CLEARCOLUMS`, `COMMANDTIMEOUT`, `WIDTH`
- **NetGen:** promjena reda ponovo zove detail (`Form1.vb` 7413). Za vrijeme punjenja mastera detail se preskoči, pa jednom na kraju (`2270`, `2099`). Drugi upit ne radi `CreateGridColumns` ako stupci već postoje (`2334`). `DETCHECKBOXVISIBLE=FALSE` sakrije selector (`2402`). `CLEARCOLUMS` na detailu sakrije ostale stupce (`2347`). Širina iz XML-a dijeli se s 14 (`7522`).
- **Carpo:** isti ključevi na otvaranju. `VrstaBlagajne` 2500/14 je 179. `Kolicina` 1000/14 je 71, format `#,####0.0000`. Intrastat i prijemni list imaju timeout 300. `OznakaKL` i dalje treba stupac `oznaka2` na spinu da master prođe.
- **Odluka:** NetGen logika. Jupiter se ne dira.
- **Test:** `sta-detail`, `bla-sirina`, `pl-kolicina`, `mpk-checkbox`
- **PR:** grid-kljuc

## R99 `Ime.Start` nije `ASSEMBLYLOAD`, totalgen ne čita povrat

- **Element:** `ASSEMBLYLOAD` i `totalgen`
- **NetGen:** `OpzStatExport.Start` nije `ASSEMBLYLOAD`, dijalog se ne otvara i DLL se ne učitava (`Form1.vb` 5369). Shell `totalgen` dobije id, operatera, konekciju, zadnji SQL mastera i parametre grida. Povrat se ne čita (`4599–4623`). Pad `Load`/`CreateInstance` upiše log i izađe (`4600`). Dijalog šalje `strParametriSvi` dvaput (`Form2.vb` 7424).
- **Carpo:** gumb bez `ASSEMBLYLOAD(` ima `dll=ne`. Pravila totalgena su `shell-pet`, `prekini`, `parametri-dva`, `master-sql`. `CarpoForm` s `KupEdit` je `KnjigaUra.frmKnjigaUraKupEdit.xml`.
- **Odluka:** NetGen logika. Jupiter se ne dira. XML kojeg nema u repou se ne dodaje.
- **Test:** `opz-nije-dll`, `bvg-carpoform`
- **PR:** ostalo-kljuc

## R100 Stablo iz store: stupci 0–3, id 0 se preskače

- **Element:** `TREE` koji zove proceduru
- **NetGen:** stupac 0 je id, 1 parent, 2 natpis, 3 tag (`Form1.vb` 4450, 4461). Korijen je prvi red. Dijete kojem je id 0 se preskače (`4498`). Tag bez dvotočke postane `vrijednost:imeForme` (`4474`).
- **Carpo:** isti raspored. `Statusi` daje `vrijednost:POPUNI_FORM`.
- **Odluka:** NetGen logika. Jupiter se ne dira.
- **Test:** `sta-stablo`
- **PR:** stablo-kljuc

## R102 Na spinu prazan stupac ili procedura, Jupiter se ne čita

- **Element:** otvaranje granula kojima na spinu fali objekt
- **NetGen:** `ExecSql` padne i grid ostane prazan. XML se ne mijenja.
- **Carpo:** `scripts/spin/Prenesi.ps1` čita samo `sys.columns` i `sys.sql_modules` na jupiter2019 i jupiter2022, pa tijelo kreira na spinu. Ako definicije nema, `Otvori.ps1` doda prazan `SELECT ... WHERE 1 = 0` ili `varchar(50) NULL`. Procedure se na Jupiteru ne pokreću.
- **Posljedica:** `AOPNovi` i dalje šalje `@id='null'`. Popis pravih tijela je u `docs/granule/spin-objekti.md`.
- **Odluka:** spin je testni. Jupiter čeka novo odobrenje za bilo što osim otvaranja granula.
- **Test:** korak `Spin stubovi i otvaranje`
- **PR:** spin-otvaranje

## R101 Na spinu prazan stupac i dvije prazne procedure

- **Element:** objekti koje spin nema, a XML ih zove pri otvaranju
- **NetGen:** `OznakaKL` čita `klmasterroba.oznaka2`. `Kulture` zove `spKultureNGQuery`. `FakPar` zove `spFakPartneriUvjetiQuery`.
- **Carpo:** `scripts/spin/stubovi.sql` doda stupac ako ga nema i proceduru samo ako je nema. Tijelo je `SELECT ... WHERE 1=0`, bez upisa. `scripts/spin/Otvori.ps1` na sql-ai otvori master tih granula i `KUPNG`, `Statusi`, `KntBlaZak`. `AOPNovi` i dalje šalje `@id='null'` i smije pasti na konverziji. Jupiter se ne dira.
- **Posljedica:** stub nije prava poslovna procedura. Popis tijela koja treba korisnik je u `docs/granule/spin-objekti.md`.
- **Odluka:** privremeno na spinu, dok ne stignu prava tijela.
- **Test:** korak `Spin stubovi i otvaranje` na sql-ai
- **PR:** spin-otvaranje

## R103 `WIDTH` prepisuje širinu koju je SQL već složio

- **Element:** `MASTERCOLUMNS` `WIDTH`
- **NetGen:** `CreateGridColumns` prvo stavi širinu po tipu, zatim `CustomFormatKolona` prepiše je s `WIDTH/14` (`Form1.vb` 7522).
- **Carpo:** isto. `VrstaBlagajne` kao string nije 100 nego 179. Combo koji drži stablo, a `TREE` je prazan, uzme `SPLITTERDISTANCE` (prijemni list 180).
- **Odluka:** NetGen logika.
- **Test:** `bla-width-prepisuje`
- **PR:** spin-otvaranje

## R107 Desno zaglavlje ispisa ostaje `@@tkosmomi`

- **Element:** `PAGEHEADERRIGHT`
- **NetGen:** ispis mastera čita desno zaglavlje iz atributa. `SETDEFAULTS` kasnije nađe `@@tkosmomi` u međuspremniku (`Form1.vb` 6001, `Class1.vb` 932).
- **Carpo:** gumb `Ispiši` vraća `pageHeaderRight=@@tkosmomi`. Vrijednost se ne zamjenjuje prije ispisa u testu i ne šalje se upit.
- **Odluka:** NetGen logika.
- **Test:** `jez-57`
- **PR:** ispis-zaglavlje

## Pokrivenost checklisti

| Checklista | Što je tamo različito od Carpa | Stavke |
|---|---|---|
| `OznakaKL.md` | prazno stablo, `GetID`, `PERMISIO`, širina `/14`, label `TEXT`. Janus krom je R59 | R02, R06, R07, R34, R59 |
| `JeziciNG.md` | dva G, `refreshmaster`, `TEXT` na gumbu nije natpis, širina 179, `PRINTREPXMASTER`. Font, obrub i HeaderLines su R59 | R04, R08, R07, R34, R41, R59 |
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
| `RijetkeKontrole.md` | katalog 11/3/5 i pravila | R45–R53 |
| `AOPNovi.md` / `RobaWebKlase` | dijalog stablo, tag, yearmonth | R45, R46 |
| `RatArkod.md` | webbrowser | R47 |
| `RobaAtributiUredi.md` | picturebox | R48 |
| `Narudzbe.md` | TREETRANSLATE, ikona, NEXTFORM, grid | R49, R52, R53 |
| `Prioritet.md` | 141 red `ssMeni`, rang stablo+detail | |
| `LVObracun.md` | `onlytree`, `@@lvglaid`, `Ime(@a=)` | R54, R55 |
| `JOPPD.md` | `onlytree`, `@@id` naspram `@@kodjoppdaid`, tilda | R54, R20, R04 |
| `KntKompVerizne.md` | `sptree`, `BUTTONS` ne briše kasniji CMD, `repx/` | R56, R42 |
| `KntBlaPromet.md` | `CLICK(Ok)`, `@@template`, `CLEARCOLUMS` | R58, R42, R08 |
| `MPPonuda.md` | nema `ONLOAD`, natpis gumba, `vrstadokid-` | R57, R43 |
| `MPPorKPNG.md` | `CMD` bez `:`, `TABINDEX` s razmakom, `VALUE` na datumu, prazan `FILLEXPLORER` | R18, R60, R61, R62, R63 |
| `Intrastat.md` | `SETDEFAULTS` rezan na `#`, `COMMANDTIMEOUT` 300, detail `@@id`, repx s parametrom | R51, R11, R55, R65 |
| `KntPDVObrasciEU.md` | `BUTTONS`, `ONENTERKEY`, `ASSEMBLYLOAD`, `repx/@@Template`, `PAGEORIENTATION` ne dira REPX | R41, R56, R35, R68 |
| `MPKntNacinPlacAzur.md` | `TOTALROW` se ne čita, `ALLOWEMPTY="FOLSE"`, `BUTTONS`, `FOCUS` | R31, R56, R66 |
| `KntOPZ-STAT-1.md` | `Ime.Start`, `POSITION`, `FILLEXPLORER` na projektu, `CLEARCOLUMS` | R67, R69, R70, R08 |
| `MPRACNEFISK.md` | `DETAILGRID` s razmakom, `REFRESHMASTER`, `FISKALIZACIJA(@@id)`, `.clsReport` | R04, R43, R55, R63 |
| `MPFiskalPubVars.md` | ulaz je `:UPDATEFORM`, `TOTALROW` se ne čita | R18, R66 |
| `Bilanca-GFI.md` | `ONLOAD` lanac, `.clsGFIPOD` nije ispis, timeout 120 | R71, R73, R65, R41 |
| `MPPDV.md` | dvostruki klik `:MP_PDV`, `PERMISIO` | R18, R06 |
| `KntR2racuni1.md` | `PRINTMASTER` ne gleda `LANDSCAPE` | R41 |
| `TecRazOTV.md` | XML nije u repou | |
| `RealizacijaStavkeNG.md` | `#ISEMPTY`, timeout 180, `PRINTREPXMASTER` | R72, R65, R41 |
| `Izgled-Carpo-checklista.md` | Manager ponuda, ne Office ribbon | R74 |
| `MPKaseOperNG.md` | `DETAILDBLCLICK`, `Lista` bez `:`, detail `@@id` | R75, R60, R11 |
| `KOD_RSPostavke.md` | stablo store, `IZMJENA(@…)`, `REFRESHAFTERCHILD` | R55, R54, R77, R65 |
| `RealizacijaFakture.md` | `#ISEMPTY`, timeout 180 | R72, R65 |
| `MPKnjigaPopisa.md` | `PORTAL`, nema gumba za `MAINFORM`, `KARTICA` ne postoji | R76, R53 |
| `SkladisteNG.md` | `ONLOAD` je SQL, `PRINTMASTER` | R41 |
| `Poruke.md` | XML nije u repou | |
| `VrCjenik.md` | `REFRESHAFTERCHILD` na formi, `DETCHECKBOXVISIBLE` | R82, R79, R05 |
| `InstrumentiPlacanjaNG.md` | `HELPKOD`, `refreshmaster`, `PRINTREPXMASTER` | R81, R04, R41, R79 |
| `MatTem.md` | `:FILTERFORM`, `!!`, `LANDSCAPE` ne dira `PRINTMASTER` | R18, R14, R41, R65 |
| `MPPSNG.md` | stablo bez `:`, `AUTOFITCOLUMNS`, `Lista`, `vrstadokid-` | R10, R80, R60, R43, R57 |
| `MPStanje.md` | `INCLUDE`, `SKIPDEFAULTLOAD` | R78, R53, R65 |
