# 00 — Sažetak analize (faza 1)

Datum: 2026-11-12. Ovo je statička analiza. Ni Carpo ni NetGen nisu građeni: DLL-ovi (DevExpress, Janus, Jupiter SharedAssemblies) nisu u repou, a okolina je Linux. Svaka važna tvrdnja ima putanju u kodu ili u izlazu skripte `tools/netgen_xml_katalog.py`. Što nije provjereno, označeno je kao **neprovjereno**.

Novi projekt `Carpo.NetGen` **nije** započet. Ručno prepisivanje XML-a nije prihvatljivo rješenje, i ručni prepisi u `definitions/Prepisane_granule/` to i pokazuju: mijenjaju procedure, polja i raspored.

## Što je cilj, jednom rečenicom

NetGen XML ostaje kakav jest (`<PROJECT>`, apsolutne koordinate, `@@`, lanac komandi odvojen s `#`). U runtimeu ga izvršava novi sloj na DevExpressu i na servisima Carpa (konekcija, log, ispis), a ne Janus i ne stari NetGenDLL.

## Najvažniji nalazi

1. **Dva jezika, ne dvije teme.** Carpo deserijalizira strogi model `JRoot` (`root` / `form` / `layout` / `event` / `action` / `datalist`) preko `XmlSerializer`. NetGen nema XSD: `XmlDocument`, atribut po atribut, `On Error Resume Next`. Ulaz Carpa je `Carpo.Gen.Start` koji nasljeđuje `Starter` (`Devex_26/Carpo.Gen/Start.cs` 8–44, `Devex_26/Carpo.UI/Connector/Abstract/Starter.cs` 78–136). Ulaz NetGena je `Loader.Run8` → `Form1` i `frm.Run8` → `Form2` (`netgen/Loader.vb`, `netgen/frm.vb`).

2. **Janus je uži nego što zvuči, ali je i dalje blokada.** U VB-u se stvarno koriste `GridEX`, `GridEXColumn`, `GridEXRow`, `GridEXGroup`, `GridEXFormatCondition`, `GridEXPrintDocument`, `GridEXExporter`, `NumericEditBox` i `Janus.Data.InheritableBoolean`. Combo je obični WinForms `ComboBox`, datum je `DateTimePicker`, tekst je `TextBox`. `UIComboBox`, `CalendarCombo`, `ExplorerBar`, `Schedule` i `TabStrip` u `.vb` izvoru nisu nađeni. Referenca je Janus GridEX **v3.5** (`netgen/NetGenDLL.vbproj` 93–104). Mapa `DevExpress131/` na koju `HintPath` pokazuje **nije u gitu** (0 datoteka). NetGen uz Janus vuče i DevExpress **13.1.7** (grid, checked combo, ispis). Carpo je DevExpress **26.1.3.0**.

3. **Raspored se ne može prevesti u Carpo layout bez gubitka.** NetGen dijeli `TOP`/`LEFT`/`WIDTH`/`HEIGHT` s `FakPretvorbe = 14` (`netgen/Module1.vb` 65, `netgen/Form2.vb` 1093–1094 i 2945–2950). To su apsolutni pikseli. Carpo slaže kontrole u tablicu `row`/`col` unutar DevExpress `LayoutControl` (`Devex_26/Carpo.Structure/Models/Core/Layouts.cs` 170–194, `Devex_26/Carpo.UI/Logic/Layout/ControlInit.cs`). Ručni prepisi (npr. kulture) ne čuvaju koordinate: NetGen forma 7085×4104 twip-jedinica postaje Carpo `width="450"` i retci.

4. **SQL nije parametarski.** NetGen lijepi tekst `@ime=vrijednost` na kraj `ONSUBMIT`, osim ako SQL počinje s `INSERT`/`DELETE`/`UPDATE`/`SELECT` (tada se `@@` ugrade u sam tekst i niz se briše) ili ako SQL sadrži `!!` (tada se `!!` obriše i parametri se **ne** dodaju). To je u `netgen/Form2.vb` 3367–3388 i `netgen/Form1.vb` 5445–5514. Carpo izvršava `JData.Command` s tipiziranim `<parameter>` (`Devex_26/Carpo.UI/Logic/Data/Core.cs`). Postojeće procedure očekuju NetGen oblik. Adapter mora zadržati slaganje stringa, ne „popraviti“ ga u prave `SqlParameter` dok se ponašanje ne dokaže.

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
| `07-build-ci.md` | Kako složiti Windows build, što fali, nacrt workflowa |

## Pitanja za korisnika

1. Tko hosta `Carpo.NetGen` u produkciji: postojeći Jupiter i dalje zove `Loader.Run8`, novi ulaz u `Carpo.Gen.Start`, ili oba dok traje prijelaz? Jupiterov `SetParent` / `mainHandle` je dio ugovora (`netgen/CLAUDE.md`).
2. Mora li prva verzija pokriti fiskalizaciju (`FISKALIZACIJA*`, `Raverus.FiskalizacijaDEV.COM`, `SLOFISKAL`), `ASSEMBLYLOAD` / `totalgen(...)` i DMS (`SpinDMSUtil.NetGenFinder`)? U korpusu je `FISKALIZACIJA` 22 puta, `ASSEMBLYLOAD` 55. To su late-bound DLL-ovi kojih nema u repou.
3. Je li prihvatljivo da SQL i dalje ide kao slobodan tekst (isti rizik i isti ugovor prema procedurama), ili se smije prijeći na prave parametre čim se string poklopi?
4. `!!` na kraju `ONSUBMIT` (npr. kartica partnera) gasi dodavanje `@ime=...`. Treba li to sačuvati bajt-po-bajt? Vidi `netgen/Form1.vb` 5449–5514.
5. Raspored: traži li se vizualno isti ekran (pikseli iz `/14`), ili je dovoljan čitljiv DevExpress layout koji ne čuva `TOP`/`LEFT`?
6. Stablo `yearmonth` u kodu puni godine **2005–2009** (`NetGenBot/docs/NetGen-Engine.md`, `Form1.vb` grana `DazovaVremenska`). Je li to živo u produkciji ili mrtav ostatak koji adapter smije ne kopirati?
7. `FOREACHROW` / `FOREACHSELECTED` / `FOREACHBLUEROW` na **podlozi** (`Form1`) su zakomentirani i ne rade. Na dijalogu rade. Adapter treba li kopirati i tu rupu?
8. `TotalGenMultiSelectTemp` (brisanje po `suser_sname()` i `operateriid`, pa insert id-eva) je ugovor s procedurama. Smije li ostati ta tablica, ili se multi-select prebacuje na Carpo `foreachrow`?
9. Atributi koje autor piše, a kod ne čita: `AGGREGATEFUNCTION` (518, ispravno slovkanje; kod čita tipfeler `AGREGATEFUNCTION`, 1951), `ALOweMPTY`/`ALOWEMPTY` (127), `TEXT` (17971, Callisto šum), `REPORT` (176 na `PROJECT`), `BOLD`, `BORDERSTYLE`. Treba li adapter namjerno ignorirati isto što i NetGen, uključujući tipfelere?
10. Deset datoteka ne prolazi strogi XML parse (neescapani znakovi). Primjer: `definitions/netgen/Agronom granule/MPKaseOperNG.xml`. Jesu li u produkciji? Tolerantni parser mora li ih progutati?
11. `definitions/netgen` i `NetGenBot/granule` su isti sadržaj. Je li jedan od njih višak koji smijemo tretirati kao duplikat u testovima?
12. Ručni prepisi mijenjaju ime procedure i skup polja (kulture: `spKultureNG*` → `spKulture`, izbačeni Standard1–3, cijena, `ind`). Potvrđujete li da `Carpo.NetGen` **ne smije** slijediti te prepise, nego stari SQL?
13. Ispis: produkcija ide preko `Spin.Printing.v23` / `v9.1`, `PrintDlg2010`, `rptDevRepx` i `NetGenExtensions` (sve late-bound). Carpo zove `print` i REPX preko `Spin.Printing.v26` (`Carpo.UI.csproj`). Koji ispis je izvor istine za granule koje u captionu imaju `::repx/...` (u korpusu 481 takvih komandi)?
14. DevExpress NuGet ključ i mapa `SharedAssemblies` (Carpo DLL-ovi, `RibbonBase.v26`, `Spin.Printing.v26`, `SpinDMSUtil`, `JupiterLogger`, `JupiterTranslator10`, `Dimenzije`, `adodb`) — hoće li doći kao repo secret i privatni artifact, ili se CI oslanja na već instaliran Jupiter na runneru?
15. Smije li `Carpo.NetGen` biti novi projekt u `Carpo.sln` koji **ne mijenja** postojeće Carpo projekte u prvoj fazi (samo ga oni kasnije referenciraju)?
16. `DXGRID="true"` na `PROJECT` prebacuje master s Janusa na DevExpress 13. U ovom korpusu taj atribut **nema nijedne** pojave (kod ga čita: `Form1.vb`). Je li na terenu ipak uključen iz baze/DMS XML-a kojeg nema u gitu?
17. Veza na drugu bazu (`DEFAULTCONNECTION`, `CUSTOMCONNECTIONSTRING`, `Conn3`) u korpusu nema nijedan pogodak, a kod je čita. Treba li je u opsegu?
18. Prijevod i `PERMISIO` (kod čita `PERMISIO`, ne `PERMISSIO`; u korpusu je `PERMISSIO` na 2 `PROJECT` čvora i zato ne radi). Je li provjera prava uvjet za prvu granulu?
19. Koji je prvi vertikalni rez za dokaz: jedna mala granula (npr. kulture, ali sa **starim** `spKultureNG*`) ili cijeli master+filter obrazac (kartica partnera: `ONLOAD=":FILTERFORM"`, `multidrop`, tražilica, `!!`, `PRINTMASTER`, `repx`)?
20. Test bez SQL Servera: je li dovoljan harness koji učita svih 609 XML-ova, izgradi stablo kontrola i snapshot komandi, a pravi SQL ostaje za kasniji Windows posao s bazom `spin`?
