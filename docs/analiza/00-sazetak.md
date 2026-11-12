# 00 — Sažetak analize (faza 1)

Datum: 2026-11-12. Ovo je statička analiza. Ni Carpo ni NetGen nisu građeni: DLL-ovi (DevExpress, Janus, Jupiter SharedAssemblies) nisu u repou, a okolina je Linux. Svaka važna tvrdnja ima putanju u kodu ili u izlazu skripte `tools/netgen_xml_katalog.py`. Što nije provjereno, označeno je kao **neprovjereno**.

Novi projekt `Carpo.NetGen` **nije** započet. Ručno prepisivanje XML-a nije prihvatljivo rješenje, i ručni prepisi u `definitions/Prepisane_granule/` to i pokazuju: mijenjaju procedure, polja i raspored.

## Odluke (iteracija 2)

Korisnik je zatvorio pitanja iz prve verzije ovog sažetka. Ovo vrijedi za sve ostale dokumente.

1. **Dva ulaza.** Jupiter je glavni program i danas pokreće i Carpo i NetGen. `Carpo.NetGen` mora se zvati iz Jupitera **istim ugovorom kao NetGen** (`Loader.Run8`, polja `moduleId`, `strNGParams`, `mainHandle`, `Ikona`, `Koler`, nakon toga `WCaption` i `hWnd`, plus `ShellResize`). Za test Carpa postoji `Carpo.TestShell`. Isti XML mora se moći otvoriti i odatle (roditelj je `pnlShell.Handle`, putanja je `TestXmlPath`). Detalj je u `08-plan-implementacije.md`.
2. **Fiskalizacija, `ASSEMBLYLOAD` i DMS ulaze u opseg.** Nisu kasnija faza koja se smije preskočiti. Ponašanje je port starog koda, ne Carpo `ZKI`/`JIR` i ne Carpo `FileFinder("Carpo")`.
3. **SQL ostaje slobodan tekst.** Nema prelaska na `SqlParameter`. Postupak `!!`, lijepljenje `@ime=` i preskok ako `@@` ostane moraju ostati.
4. **Raspored ne mora biti isti piksel.** Što se može 1:1 (kontrola, natpis, tab order, grid kolone), preslikati. Ostatak (`TOP`/`LEFT` naspram Carpo tablice) je poznata razlika za korisnike, zapisana u dokumentu 05, ne bug.
5. **`TotalGenMultiSelectTemp` ostaje.** Objašnjenje je u dokumentu 02, odjeljak koji je dodan u iteraciji 2. Procedure i dalje čitaju tu tablicu.
6. **Tipfeleri se toleriraju kao danas.** Adapter čita isto pogrešno slovkanje koje kod traži (`CLEARCOLUMS`, `AGREGATEFUNCTION`, `PERMISIO`) i ignorira slovkanja koja kod ne traži (`HIGHT`, `ALOWEMPTY`, `AGGREGATEFUNCTION`, `PERMISSIO`). Potpuni popis je u dokumentu 03, odjeljak 12.
7. **Prva granula nije odabrana.** Predloženi redoslijed smoke testova, od najtanje prema fiskalizaciji, je u dokumentu 08. Ne kreće se od kultura: njihov Carpo prepis zove druge procedure.
8. **DLL-ove korisnik donosi kasnije.** Dovoljan je build TestShella i budućeg `Carpo.NetGen`. Popis je `09-potrebni-dll-ovi.md`. Baza `spin` na stalno upaljenom testnom serveru spaja se u sljedećoj iteraciji; ovdje su samo opcije (self-hosted runner na tom serveru).

## Što je cilj, jednom rečenicom

NetGen XML ostaje kakav jest. Jupiter ga i dalje zove kao danas NetGen, TestShell ga može otvoriti kao danas Carpo XML, a crtanje ide na DevExpress 26 uz punu kompatibilnost SQL teksta, DMS-a, `ASSEMBLYLOAD` i fiskalizacije.

## Najvažniji nalazi

1. **Dva jezika, ne dvije teme.** Carpo deserijalizira strogi model `JRoot` (`root` / `form` / `layout` / `event` / `action` / `datalist`) preko `XmlSerializer`. NetGen nema XSD: `XmlDocument`, atribut po atribut, `On Error Resume Next`. Ulaz Carpa je `Carpo.Gen.Start` koji nasljeđuje `Starter` (`Devex_26/Carpo.Gen/Start.cs` 8–44, `Devex_26/Carpo.UI/Connector/Abstract/Starter.cs` 78–136). Ulaz NetGena je `Loader.Run8` → `Form1` i `frm.Run8` → `Form2` (`netgen/Loader.vb`, `netgen/frm.vb`).

2. **Janus je uži nego što zvuči, ali je i dalje blokada.** U VB-u se stvarno koriste `GridEX`, `GridEXColumn`, `GridEXRow`, `GridEXGroup`, `GridEXFormatCondition`, `GridEXPrintDocument`, `GridEXExporter`, `NumericEditBox` i `Janus.Data.InheritableBoolean`. Combo je obični WinForms `ComboBox`, datum je `DateTimePicker`, tekst je `TextBox`. `UIComboBox`, `CalendarCombo`, `ExplorerBar`, `Schedule` i `TabStrip` u `.vb` izvoru nisu nađeni. Referenca je Janus GridEX **v3.5** (`netgen/NetGenDLL.vbproj` 93–104). Mapa `DevExpress131/` na koju `HintPath` pokazuje **nije u gitu** (0 datoteka). NetGen uz Janus vuče i DevExpress **13.1.7** (grid, checked combo, ispis). Carpo je DevExpress **26.1.3.0**.

3. **Raspored: 1:1 gdje ide, razlika gdje ne ide (odluka).** NetGen dijeli koordinate s `FakPretvorbe = 14` (`netgen/Module1.vb` 65, `netgen/Form2.vb` 1093–1094). Carpo slaže `row`/`col`. Korisnik ne traži isti piksel. Kontrola, natpis i redoslijed tabulatora idu 1:1. Apsolutni `TOP`/`LEFT` i preklapanja ostaju dokumentirana razlika, ne bug.

4. **SQL ostaje slobodan tekst (odluka).** NetGen lijepi `@ime=vrijednost` na kraj `ONSUBMIT`, osim ako SQL počinje s `INSERT`/`DELETE`/`UPDATE`/`SELECT` ili sadrži `!!`. Izvor: `netgen/Form2.vb` 3367–3388 i `netgen/Form1.vb` 5445–5514. Prelazak na prave `SqlParameter` je odbačen jer bi tražio izmjenu procedura kod klijenata.

5. **`@@` i greške iz SQL-a su kontrolni tok.** `ExecSql` preskače prazan SQL, SQL koji počinje s `@`, i SQL u kojem `@@` preživi zamjenu (`netgen/Class1.vb` 105–118). Poruka servera s `#OK` / `+OK` guta grešku, `#close` zatvara formu, `#FOCUS(ime)` vraća fokus (`netgen/Class1.vb` 170–205). Carpo `message` / `confirm` / `validate` to ne rade nad tekstom SQL iznimke.

6. **Korpus je manji nego što izgleda s diska.** Na disku je 1238 XML-ova. Jedinstvenih sadržaja (SHA-256) je **609**. `definitions/netgen` (605) i `NetGenBot/granule` (605) su isti skup bajtova, samo su mape drugačije (`Agronom granule` naspram `Agronom`, `xml-ovi s nevkoša-zubak` naspram `Nevkos`). Unutar jednog stabla 22 datoteke su kopija između Agronoma i Biovege (npr. `MPEurParam.xml`). `Prepisane_granule/Netgen` dodaje 26 novih i 2 koja već postoje u glavnom skupu. Brojač je `tools/netgen_xml_katalog.py`, sirovina `tools/out/netgen-xml-katalog.json`.

7. **Jezik je mali, imena formi su dugačak rep.** Nad 609 datoteka: 54165 `INPUT`, 44 vrijednosti `TYPE`, 2120 različitih imena elemenata od čega je ~2098 **ime forme** (`INSERT`, `FILTERFORM`, `BRISISTAVKU`, …), ne ključna riječ. `TYPE` se ne uspoređuje kao enum nego kao podstring (`InStr`), redom. Zato `checkbox` upadne u `check`, `textarea` u `text`, `currency` u broj. U korpusu su i tipfeleri (`b_utton`, `t1ext`, `buttton`, `datetiem`) koje grana ne pogodi.

8. **Carpo već ima tri NetGen kuke, i one nisu interpreter.** Na gridu postoje atributi `netgenfields`, `netgensizes`, `netgenstyles` (`Devex_26/Carpo.Structure/Models/Core/DataControls.cs` 550–568). `Style.ParseNetGenStyles` parsira `bold,italic,boja` (`Devex_26/Carpo.UI/Logic/Style.cs` 88). Ručni prepisi tražilice stavljaju `netgenfields="true"`. To je kozmetika kolona, ne učitavanje `<PROJECT>`.

9. **Build na čistom runneru danas ne prolazi.** Jedina `ProjectReference` u cijelom solutionu je `Carpo.UI` → `Carpo.Data`. Sve ostalo su `HintPath` na `C:\Program Files (x86)\Jupiter Software\...\SharedAssemblies\Spin\*.dll`, kojih nema u gitu. DevExpress je referenciran po jakom imenu 26.1.3.0, bez `HintPath` i bez `nuget.config`. `OutputPath` Debug ide u taj isti Program Files. Nacrt workflowa je u `docs/analiza/draft-carpo-build.yml` i **nije** uključen.

10. **Preporuka je hibrid, ne čisti prijevod u Carpo XML.** Prijevod u `JRoot` u loadu izgleda uredno i lažno je: ručni prepisi mijenjaju procedure (`spKultureNGQuery` → `spKulture`), izbacuju polja i prelaze na `searchedit`. Čisti NetGen renderer koji ignorira Carpo duplicira konekciju, log i ispis. Hibrid drži NetGen parser i komandni interpreter, a ispod njih zove `Carpo.Data`, DevExpress kontrole i postojeći ispis. Obrazloženje je u `06-prijedlog-arhitekture.md`.

## Gdje je što

| Dokument | Sadržaj |
|---|---|
| `01-carpo-arhitektura.md` | Load, `JRoot`, render, SQL, akcije, točke za novi projekt, graf projekata |
| `02-netgen-arhitektura.md` | Ulaz, XML, Janus popis, kontrole, komande, `@@`, SQL, ispis |
| `03-netgen-xml-katalog.md` | Katalog jezika + brojevi iz skripte, rijetko, kod bez korpusa i obrnuto |
| `04-mapiranje-netgen-carpo.md` | Element/atribut → direktno / adapter / nema u Carpu, uz parove prepisa |
| `05-gap-i-rizici.md` | Što Carpo danas ne umije, razlike u ponašanju, rizici |
| `06-prijedlog-arhitekture.md` | Tri opcije, preporuka, faze, kako se faza provjerava |
| `07-build-ci.md` | Windows build, nacrt workflowa |
| `08-plan-implementacije.md` | Faze, zadaci, kriteriji, smoke redoslijed, harness |
| `09-potrebni-dll-ovi.md` | Što korisnik treba donijeti da se TestShell i `Carpo.NetGen` prevedu |

## Ispravke iz drugog prolaza

- `Loader.Run8` **ne vraća** formu. `Run8 = ob` je zakomentiran (`netgen/Loader.vb` 175). Jupiter čita `WCaption` i `hWnd` s objekta `Loader`. Stari `Run` jest vraćao objekt (`Loader.vb` 95).
- `frm.Run` (ne Run8) deklarira `Form2` bez `New` (`netgen/frm.vb` 39–52, instanciranje je u komentaru). Živi dijalog-ulaz je `frm.Run8` (88), koji radi `New Form2`.
- `IJupiterStart` (`Devex_26/Carpo.Structure/Interfaces/IJupiterStart.cs`) ima dodatni `menuID` i `Starter` ga **ne implementira**. Stvarni Carpo ulaz je `Starter.Run` s 9 argumenata (`Starter.cs` 78). NetGen ulaz je drugi potpis i druga klasa. Ne smiju se pomiješati.
- Carpo nema `ShellResize`. NetGen `Loader.ShellResize` (`Loader.vb` 22–26) Jupiter zove da razvuče dijete. Zamjena NetGena mora tu metodu imati.
- DMS za NetGen je `SpinDMSUtil.NetGenFinder.GetFile` (`Form1.vb` 3172–3182), ne Carpo `FileFinder("Carpo")` (`Starter.cs` 76). Isti DLL, druga klasa i drugi folder.
- `yearmonth` je potvrđen u kodu: `For g = 2005 To 2009` (`Form1.vb` 4527–4534), ne samo u starom markdownu.
- `HEADERALIGNMENT` kod čita na `MASTERCOLUMNS` / `DETAILCOLUMNS` (`Form1.vb` 3296–3309). Nije mrtav atribut.
- `GRIDALLOWEDIT` i `GRIDALLOWEDITCOLUMNS` (npr. `definitions/netgen/BIOVEGA/OznakaKL.xml`) **nisu** u `GetNamedItem` izvan uzorka u `Resource1.Designer.vb`. Engine ih ne primjenjuje. `STARTUPSTORA` bez `ONLOAD` preživi jer linija 3368 padne na `On Error Resume Next` i ne prepiše 3367.

## Pitanja koja su još otvorena

Zatvoreno u iteraciji 2 i više se ne pita: ulaz (Jupiter + TestShell), fiskalizacija/`ASSEMBLYLOAD`/DMS u opsegu, SQL kao tekst, raspored nije piksel-identičan, `TotalGenMultiSelectTemp` ostaje, tipfeleri se toleriraju, DLL-ove donosi korisnik, baza `spin` ide sljedeći put.

1. **Ime assemblyja koji Jupiter traži.** Izvornik Jupitera nije u repou. Vidljivi ugovor je klasa `Loader` s `Run8`. Ako izbornik (`ssMeni` ili ini) tvrdo piše `NetGenDLL`, novi DLL mora ili nositi to ime (zamjena u `SharedAssemblies`) ili se redak izbornika mijenja na `Carpo.NetGen`. Koji od ta dva je dozvoljen?
2. **`yearmonth` 2005–2009** (`Form1.vb` 4530). Kopirati raspon kakav jest, ili ga proširiti? Kopija je kompatibilna i vjerojatno kriva za korisnika.
3. **`FOREACH*` na podlozi je mrtav** (`Form1.vb` 6841–6850). Kopirati rupu (klik ne radi ništa) ili na podlozi ipak izvršiti petlju? Puna kompatibilnost kaže rupu. Treba potvrdu jer je to tihi gubitak.
4. **Deset XML-ova ne prolazi strogi parse** (npr. `definitions/netgen/Agronom granule/MPKaseOperNG.xml`). Ako `XmlDocument.Load` i na disku padne, NetGen ide na DMS/bazu. Treba li parser javiti istu grešku, ili ih pokušati popraviti?
5. **Ispis.** 481 komanda ide u `repx/`. Stari kod zove `Spin.Printing.v23` / `v9.1`, `PrintDlg2010`, `rptDevRepx`, `NetGenExtensions`. Carpo ima `Spin.Printing.v26`. Je li v26 zamjena za te pozive, ili na stroj moraju ostati i stari DLL-ovi?
6. **Ime servera i autentikacija baze `spin`.** `netgen/CLAUDE.md` spominje `jupiter2022`, Windows auth. Je li to taj stalno upaljeni server? Treba li SQL login za runner koji nije u domeni?
7. **`DXGRID`, `DEFAULTCONNECTION`, `CUSTOMCONNECTIONSTRING`** kod čita, u 609 fajlova ih nema. XML samo u `netgenxml` tablici ih može imati. Ulaze u port (da, jer kod postoji) — treba li testni XML iz baze kad se spoji `spin`?
8. **Smije li TestShell dobiti jednu granu** (ako korijen nije `<root>`, zovi `Carpo.NetGen`) u istoj isporuci kad krene kod? Bez tog retka u `Main.cs` `cmdGen_Click` TestShell i dalje zna samo `Gen.Start`.
9. **`refreshmaster` u `JeziciNG.xml`** (`CMD1CAPTION="Unesi:Unesi#refreshmaster"`) nije komanda `REFRESHMASTERGRID`. Klik će tražiti formu tog imena. Je li to poznati kvar granule ili postoji skriveno značenje koje nismo našli?
