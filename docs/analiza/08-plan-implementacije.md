# 08 — Plan implementacije

Ovo je plan. M0 (parser, oba Jupiter ulaza, TestShell grana, build) je otvoren u `Devex_26/Carpo.NetGen`. Faze slijede odluke do iteracije 3 i dopunu iz `ssMeni` (dokument 06): dva ulaza `Loader` i `frm`, TestShell smije se mijenjati, SQL kao tekst, tipfeleri, `TotalGenMultiSelectTemp`, DMS, `ASSEMBLYLOAD`, fiskalizacija, `yearmonth` 2005–2009, `FOREACH*` na podlozi se ne piše, ispis je v26 + `rptDevRepx`. Testna baza je samo `sql-ai` / `spin`. `jupiter2022` se ne koristi. Baza se ne mijenja u M0.

Log ide kroz Carpo logger (Ctrl+F11, `Carpo.Core.Logger`), ne kroz NetGen `CBLog` / `PokaziCBLog`. Pravilo i otvorene stavke su u `docs/funkcionalnosti/logging.md`. Svaki sljedeći element zove `NgDnevnik`.

Svaka razlika NetGena i Carpa ide u `docs/razlike-netgen-carpo.md` u istom PR-u u kojem je otkrivena. Checklist je na vrhu tog registra.

Svaka faza ima kriterij prihvaćanja koji se može pročitati bez „izgleda u redu“. Dok nema DLL-ova (dokument 09), radi se samo ono što ne traži prevodenje: zlatni fajlovi i opisi. Čim TestShell prođe build, kreće M0 u C#.

## Ulazi

### Jupiter

`ssMeni.Parametar` (varchar(80), 830 redova na `spin`, samo čitanje) je `asm:<Assembly>.<Klasa>:<putanja xml>`. Podfolder u putanji ostaje (`MER\ManagerObaveza.xml`). Brojke i rizik jedne točke u prefiksu su u dokumentu 06. Prebacivanje granule je izmjena tog stupca, ne izmjena XML-a. M0 bazu ne dira.

Dvije klase, obje u assemblyju `Carpo.NetGen`:

**`Carpo.NetGen.Loader`** (132 reda `netgendll.loader`). Kao `Loader.vb`:

- polja `moduleId`, `strNGParams`, `mainHandle`, `Ikona`, `Koler`, `WCaption`, `hWnd`
- `Run8(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)`
- `ShellResize(w, h)` kao `Loader.vb` 22–26
- `Run8` vraća `Nothing`. `WCaption` se puni iz `<CAPTION>` (i `v[verzija]` ako `VERSION` postoji, `Form1.vb` 3522–3528). `hWnd` ostaje 0 dok M1 ne otvori prozor

**`Carpo.NetGen.frm`** (9 redova `netgendll.frm`). Kao `frm.vb`, dijalog, ne podloga:

- polja `moduleId`, `mainHandle`, `WCaption`, `hWnd`, `Ikona`, `Koler`, `ulazniID`, `ulaznaForma`
- nema `strNGParams` i nema `ShellResize`
- `Run` i `Run8` s istim potpisom. Živi je `Run8` (`New Form2`). Stari `Run` u VB-u ne instancira formu; potpis ostaje da host ne padne na missing method
- `ulazniID` i `ulaznaForma` se u M0 samo čuvaju (`Form2.vb` 489, 503)

Unutra: forma u istom AppDomainu (AppDomain ostaje ugašen, komentar 22.04.2024). `strNGParams` ide u isti router kao klik gumba (`NovaGenerickaForma`), tek u M1. DMS i tablica `netgenxml` nisu M0: ako XML nije na disku, M0 javi istu `XML LOAD ERROR` poruku i ne ide u bazu.

### TestShell

`Carpo.TestShell/Main.cs` `cmdGen_Click` (167–184) danas uvijek radi `Gen.Start` za obični XML. Predviđena izmjena, jedna grana:

- pročitati prvi element
- `<root>` → postojeći `Gen.Start` (Carpo se ne dira u ponašanju)
- `<PROJECT>` → `Carpo.NetGen` s `TestXmlPath = xml`, `ModuleId = ime fajla`, parent `(int)pnlShell.Handle`, isti server/baza/operater iz dijaloga Postavke

Bez te grane TestShell ne može otvoriti NetGen XML. To je jedina izmjena postojećeg fajla u prvoj isporuci koda, uz dodavanje projekta u `Carpo.sln`.

TestShell i dalje nema CLI. Automatski harness nije klikanje Postavki. Harness je konzola opisana niže. TestShell je ručni smoke nakon što konzola prođe.

## Faze

### M0 — parser

Zadatak: učitati NetGen XML u model (forme, `INPUT`, atributi, `TYPE` grana po `InStr` redu). Nepoznati atribut se čuva. Encoding je onaj koji `XmlDocument` pročita iz BOM-a, uključujući UTF-16. Fajl se ne popravlja. Ako `Load` baci, status je greška s istom porukom kao `Form1.vb` 3264–3265, ne iznimka koja ruši cijeli skup. U ovom korpusu takvih fajlova nema.

Prihvaćanje:

- nad 609 jedinstvenih sadržaja broj `INPUT` i raspodjela `TYPE` jednaki su `tools/out/netgen-xml-katalog.json`
- `OznakaKL.xml`: jedna forma `FRMEDIT`, kontrola `oznaka2` tipa text, `ONSUBMIT` počinje s `update`
- `JeziciNG.xml`: `CLEARCOLUMS` se pročita (kod ga traži), `AGGREGATEFUNCTION` s dva G se **ne** veže na agregat
- `HIGHT` ne postane `HEIGHT`
- `datetiem` ide u granu datuma: `InStr` nađe podstring `date` (riječ počinje s njim). Katalog je to prvo proglasio promašajem; vrijedi kod, ne ta rečenica

### M1 — prvo lijevo stablo i master-detail, pa gumbi

Oko 90% granula ima isti okvir: lijevo stablo, desno master grid, ispod njega detail. To je podloga `Form1` (TreeView + GridEX1 + GridEX2), ne poseban XML predložak. Redoslijed elemenata, jedan po jedan: **grid**, zatim **stablo**, zatim **master-detail veza**, zatim **poziv Carpo subforme** (`docs/funkcionalnosti/poziv-carpo.md`). Prazan `TREE` sakrije `TreeView2` i lijevi splitter stavi na 0 (`Form1.vb` 4310–4316), ne ostavlja prazno stablo na ekranu. `DETAILGRID` puni donji grid. Piksel `TOP`/`LEFT` i dalje nije cilj.

Dijalog dolazi nakon tog okvira. Iz QA OznakaKL i JeziciNG, ovo još nije grid i ostaje otvoreno dok ne dođe taj element:

- `Unesi#refreshmaster` (hash nije `REFRESHMASTERGRID`, ali prazna sljedeća forma ipak osvježi zadnji SQL)
- `Odustani` čiji je `ONCLICK` `ok`, ne `CLOSE`
- `NEXTFORM` koji ne postoji: nema poruke, 0 kontrola i dalje `OKClick`
- `BORDERSTYLE=3` se čuva, kod ga ne čita, ostaje `Sizable`
- `REFRESHAFTERCHILD` na formi je sirov; `SrediKontrolu` ga čita samo na `INPUT`-u
- natpis dijaloga i kontrola je `CAPTION` (ne `TEXT` gumba)

Zadatak, tim redom: okvir lijevo stablo + master + detail; `ONLOAD` koji je SQL puni master (ako `ONLOAD` ne postoji, vrijedi `STARTUPSTORA`); gumbi iz `CMDxCAPTION` / `BUTTONS`; `@@id` i `@@operateriid`; `REFRESHMASTERGRID`; `ONLOAD=":IME"` ne ide u SQL. `Loader.Run8` i TestShell grana otvaraju istu klasu. `frm.Run8` ostaje dijalog, ne drugi crtač podloge.

Prihvaćanje:

- TestShell pokaže okvir lijevo stablo + master + detail prije ijednog dijaloga; `OznakaKL` ima prazno stablo i pun master (`STARTUPSTORA`), detail prazan
- jedinični test substituera: fiksni `@@id` / `@@operateriid`, SQL s preživjelim `@@` se ne šalje (`Class1.vb` 113–118)
- `OznakaKL`: `STARTUPSTORA` se izvrši jer `ONLOAD` ne postoji (linije 3367–3368, `On Error Resume Next`)
- TestShell na toj datoteci pokaže grid (kad ima baze) ili, bez baze, model s točnim SQL tekstom
- `ShellResize` postoji i pomiče prozor; Jupiterov poziv ne pada na missing method

### M2 — dijalog i SQL tekst

Zadatak: text, number, date, combo, check, button. `PokupiParametre` istim redom dodavanja kao XML. `ONSUBMIT` lijepljenje, grana prvih 6 slova `INSERT|DELETE|UPDATE|SELECT`, grana `!!`. `OK`/`CLOSE`/`ok`. Datumski tokeni `firstdayofyear` i srodni. Combo: stupac 0 / stupac 1. Prazan combo bez `ALLOWEMPTY` pokuša vrijednost `1`. Tipfeleri iz odjeljka 12 dokumenta 03 ne mijenjaju kontrolu.

Prihvaćanje:

- `JeziciNG` Unesi: SQL `spJeziciAzur @action=0...` plus `#OKStay` ne zatvara formu; `focus` kolona sljedećeg selecta
- kartica partnera: `ONSUBMIT` s `!!` ne dobije drugi niz `@ime=`
- skup parametara kulture INSERT poklapa imena (`@Naziv`, `@robaid`, …) uz **stari** `spKultureNGAzur`, ne `spKulture`
- `ALOWEMPTY` ne uključi prazan red

### M3 — tražilica, grid u formi, `FOREACH*`, multi-select tablica

Zadatak: oba oblika tražilice (`sp`/`select` i `tablica#id#kolona`). Grid parametri `@gridIme`, nizovi plavih i čekiranih. `FOREACH*` samo na dijalogu (`Form2.vb` 7040–7047, tijela 8310–8363). Na podlozi petlja ne postoji (dokaz u dokumentu 10): `Rafalno` ima poziv u komentaru i živ `i = i + 1` (`Form1.vb` 6843–6850), a `NovaGenerickaForma` tu riječ ne grana. `multiselect`/`multiblue` pune `TotalGenMultiSelectTemp` točno SQL-om iz `Form1.vb` 4934–5032.

Prihvaćanje:

- delete po `suser_sname()` i po `operateriid`, zatim insert s `host_name()`
- `multidrop` šalje `id1~id2~`
- podloga s `FOREACHSELECTED` ne izvrši petlju
- tražilica: očekivani WHERE izvučen iz `Form3` prije nego se test zaključa (opis u `NetGen-Kontrole.md`)

### M4 — stablo, ispis, `INCLUDE`

Zadatak: `TREE` statički čvor i SQL s 4 stupca. `yearmonth` kopira `For g = 2005 To 2009` dok pitanje 2 ne kaže drugačije, i to piše u poznate razlike. `PRINTMASTER` i `::repx/` idu u jedan most. Ako je odluka da v26 zamjenjuje v23, most zove v26; ako ne, zove iste late-bound tipove kao `Class1`. `INCLUDE` spaja dokumente.

Prihvaćanje:

- fixture od dva XML-a: dijete iz `INCLUDE` vidljivo u modelu
- kartica: parametri `rptKarticaKontaSKTele2` izađu s imenima iz captiona (`@DatumDo`, `@DatumOd`, …)
- `PAGEHEADER*` prođu `@@` zamjenu; `##` i `!!` postanu novi red samo u zaglavlju, ne u SQL-u

### M5 — DMS

Zadatak: `IXmlSource` s redom `NetGenFinder.GetFile` → `netgenxml` + decrypt `"tan123ge"` → disk. Dijalog preskače DMS ako je izvor već baza. Prazan assembly: CB log, ne rušenje, pad na sljedeći izvor.

Prihvaćanje:

- tri lažna izvora u testu, redoslijed dokazan bez pravog DMS-a
- na stroju s `SpinDMSUtil.dll`: jedan poznati fajl, `source` upisan u log
- Carpo granule i dalje idu kroz `FileFinder("Carpo")` (regresija TestShella na `<root>`)

### M6 — `ASSEMBLYLOAD`

Zadatak: port tri poziva (`NovaGenerickaForma` 4584, `Rafalno` podloge 6602 i 6681, dijalog 7424). Razlika u zadnja dva argumenta `totalgen` mora ostati (master SQL naspram `strParametriSvi`). Load greška prekida lanac.

Prihvaćanje:

- test s lažnim assemblyjem u `AppDomain`: zabilježeni argumenti `totalgen` za sva tri mjesta
- `Bilanca-GFI.xml` u modelu ima komandu, ne pada u parseru
- bez DLL-a: CB log s imenom assemblyja, forma se ne ruši cijela, ostatak lanca staje (`Exit Sub`)

### M7 — fiskalizacija

Zadatak: `Fiskalizacija` i `FiskalizacijaNapojnica` i `PoslovniProstor` kako su u `Form1.vb` 286, 615, 809 i mapiranje komandi u `Form2.vb` 7459–7493. Više čekiranih redova na podlozi zove fiskalizaciju u petlji (4643–4664). `SLOPP` ostaje prazan. URL s `fu.gov.si` ide na `SLOFISKAL.Program.SLORacun`, inače Raverus `CentralniInformacijskiSustav`.

Prihvaćanje:

- jedinični test bira granu SI/HR iz lažnog retka `mpfiskalizacija` bez mreže
- `FaktureNeFisk.xml` se učita i komanda se vidi
- pravi poziv CIS-a nije dio automatskog testa (nema porezne); ručni smoke je poseban i ne ide u CI
- Carpo `ZKI`/`JIR` na `<root>` granulama i dalje rade (regresija)

## Smoke redoslijed (prijedlog)

Od najmanje pokrivenosti prema složenom. Brojevi kontrola su iz korpusa, ne iz procjene. Svaka sljedeća dodaje jedan sloj koji prethodna nema. Kulture nisu prve: Carpo prepis zove druge procedure, a stari XML je srednje velik.

| Red | Datoteka | Zašto baš ona | Što tek tu počinje |
|---|---|---|---|
| 1 | `definitions/netgen/BIOVEGA/OznakaKL.xml` | 2 kontrole, jedna forma, `update` SQL, `STARTUPSTORA` bez `ONLOAD` | master grid, dijalog, lijepljenje nije ni potrebno (SQL je već pun), `CMD` |
| 2 | `definitions/netgen/Agronom granule/JeziciNG.xml` | 15 kontrola, `CLEARCOLUMS`, `PRINTREPXMASTER`, `OKStay`, `SETDEFAULTS`, `HELPKOD` | unos/izmjena/brisanje, ispis, tipfeler koji se ne smije „popraviti“ (`AGGREGATEFUNCTION` prazan s dva G) |
| 3 | `definitions/netgen/BIOVEGA/MPPorKPNG.xml` ili `sktStariKruh.xml` | combo + datum + broj ili check, bez stabla i fiskala | `firstday*` ako postoji u fajlu, `ALLOWEMPTY`, parametri datuma `yyyyMMdd` |
| 4 | `definitions/netgen/BIOVEGA/nultiKontingenti.xml` | tražilica, 11 kontrola | `@@searchbox…`, SQL oblik tražilice |
| 5 | `definitions/Prepisane_granule/Netgen/Agro/Kulture.xml` | search `roba#…`, combo, `spRaiserror#OK`, stari `spKultureNG*` | dokaz da se ne slijedi Carpo prepis |
| 6 | `definitions/netgen/BIOVEGA/MPPregKasa.xml` ili `KntBrutoBilancaNG.xml` | malo stabla | `TREE`, `@@node` |
| 7 | `definitions/Prepisane_granule/Netgen/Kontor/KarticaPartnera.xml` | `ONLOAD=":FILTERFORM"`, `!!`, `multidrop`, `repx`, `PRINTMASTER`, `PAGEHEADER` | filter prije grida, poznata razlika rasporeda (splitter u prepisu, modal u NetGenu) |
| 8 | granula s `MULTISELECT` u imenu forme (traži se pri M3 iz korpusa; heuristika kaže 94 ovakvih komandi) | temp tablica | `TotalGenMultiSelectTemp` |
| 9 | `definitions/netgen/BIOVEGA/MPNGPregDokMAIL.xml` | najmanji `ASSEMBLYLOAD` (9 kontrola) | `totalgen` |
| 10 | `definitions/netgen/Agronom granule/FaktureNeFisk.xml` | najmanji fiskal (11 kontrola) | Raverus / `mpfiskalizacija`, bez slanja na CIS u automatskom testu |

`yearmonth` nije u ovoj desetorci. Ako pitanje 2 ostane „kopiraj 2005–2009“, dovoljan je jedan ručni XML od tri linije, ne produkcijska granula.

## Harness

Tri razine.

1. **Već postoji.** `python3 tools/netgen_xml_katalog.py`. Svaka izmjena parsera koja bi promijenila broj `TYPE` mora objasniti diff JSON-a. Ovo ne otvara formu i ne treba DLL.

2. **Konzola `Carpo.NetGen.Corpus` (M0, čim se net48 prevede).** Argument: folder. Izlaz: TSV (putanja, status, broj formi, broj kontrola, `TYPE` grane). Očekivani TSV se commita za 10 smoke datoteka plus agregat 609. Nema SQL Servera. Pokreće se na istom Windows runneru koji prevede TestShell.

3. **Baza `spin`, sljedeća iteracija, ne ovaj PR.** Opcije spajanja, od najmanje krhke:

   - **Self-hosted GitHub runner na testnom serveru** (ili na stroju u istoj mreži). Job dobije Windows auth ako je runner u istoj domeni kao SQL, inače SQL login u secretu `SPIN_SQL_USER` / `SPIN_SQL_PASSWORD`. Connection string se ne commita. Ovo je preporuka: cloud runner (`windows-latest`) do `spin` obično nema rutu.
   - **VPN ili privatni endpoint** s cloud runnera. Radi samo ako mreža to već ima. Ne otvarati SQL port prema internetu radi ovog posla.
   - **Ručni smoke** na stroju koji vidi `sql-ai`: TestShell, baza `spin`, deset datoteka iz tablice. `jupiter2022` se ne otvara.

   Self-hosted runner je preporuka za sljedeću iteraciju (spajanje clouda). Može stajati na `sql-ai` ili na drugom Windows stroju u istoj mreži. Cloud `windows-latest` do `sql-ai` nema rutu osim ako se mreža namjerno otvori, a SQL port prema internetu se ne otvara. M0 build ne treba taj runner.

   Automatski test na bazi ne pokreće svih 609. Većina zove procedure i `spRaiserror`. Lista je gornjih 10, i samo čitanje (`select` / query action), bez fiskalnog CIS-a i bez `ASSEMBLYLOAD` koji piše.

## Poznate razlike koje idu korisniku (ne u bug listu)

- piksel raspored i okviri `frame`
- font i tema DevExpress 26
- `AGGREGATEFUNCTION` s dva G i dalje ne zbraja
- `ALOWEMPTY`, `HIGHT`, `WUDTH` i ostalo iz odjeljka 12.3 i dalje ne rade
- `yearmonth` i dalje staje na 2009 (odluka: kopija)
- `FOREACH*` na podlozi i dalje ne vrti petlju (odluka: ne implementirati)
- `SLOPP` i dalje ne radi
- `GRIDALLOWEDIT` na `OznakaKL` i dalje ne uključuje edit (kao ni danas)
