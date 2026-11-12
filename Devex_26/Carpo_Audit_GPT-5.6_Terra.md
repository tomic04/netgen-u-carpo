# Carpo tehnički audit i inkrementalni plan poboljšanja

## Metapodaci

- Model: **GPT-5.6 Terra**
- Datum analize: 31.07.2026.
- Analizirani repozitorij: `c:\TFS\Core Techonologies\Carpo\Devex_23_4.8_framework`
- Branch/commit: Git metapodaci nisu dostupni u radnoj kopiji (`git` je vratio “not a git repository”); TFS je konfiguriran u `Carpo.sln`.
- Pristup repozitoriju: pregledan je cijeli source tree statički, svih 12 `.csproj` datoteka, solution konfiguracija te ciljani runtime/SQL/UI tokovi. Nije izvođen build, Jupiter host, SQL Server niti produkcijska granula.
- Nije bilo moguće pregledati: Jupiter host loader, definicije stored procedura, produkcijske XML granule/DMS sadržaj, execution planove, CI build definicije, telemetry/logove i stvarno ponašanje COM/Outlook/fiskalnih servisa.
- Metod: statička analiza. “Potvrđeno” znači da je obrazac vidljiv u C# / MSBuild kodu; ne predstavlja izmjeren produkcijski učinak.

---

# 1. EXECUTIVE SUMMARY

1. Primarni tok `ComponentGrid.Populate → GetDataAsync()` **sinkrono** izvršava SQL na WinForms UI niti: `Task.FromResult(dataDefinition.GetDataSet(...))` ne prebacuje rad u pozadinu. Ovo je potvrđen izvor UI zastoja.
2. Druga putanja grida započinje pozadinski SQL, ali poziva `_ = GetDataAsync()` bez čekanja i odmah šalje focus događaje. To je potvrđena utrka koja može proizvesti duple `focusedrowchanged` kaskade i prerano punjenje detail grida.
3. `Events.RunActions` u istoj UI/action putanji radi SQL, Outlook COM, kriptografiju i SOAP fiskalizaciju. Posebno je blokirajuće `Task.Run(...).GetAwaiter().GetResult()` za JIR.
4. `Carpo.Data.Common.ExecuteQueryInTran` i `ExecuteAzurInTran` kreiraju `SqlCommand` bez `using`/`Dispose`. To je potvrđen ownership propust za svaku transakcijsku akciju.
5. `SqlHelper` kreira komande bez `Dispose` u više overloadova; connection-string overloadovi zatvaraju konekciju, ali command resurs nije deterministički oslobođen.
6. Sve SQL putanje u `Core.SetData` vraćaju `DataSet` za reader scenarije, čak kada pozivatelj troši samo prvu tablicu. To je potvrđen memorijski i CPU overhead, ali veličinu učinka treba profilisati.
7. `Serializer` više puta obilazi i mutira isti `XDocument`, rekurzivno učitava XML dijelove i za `partsql` radi dodatnu `XmlSerializer` deserializaciju i SQL. Startup hot path postoji; njegov trošak zahtijeva mjerenje po granuli.
8. `ComponentGrid.SetData` pri svakom punjenju može inicijalizirati kolone, snimiti/vratiti layout, promijeniti template i raditi BestFit. `BeginUpdate/EndUpdate` je komentiran baš oko punjenja; kod potvrđuje potencijal za višestruke layout recalculations.
9. `Manager` sprema kompletan layout u memoriju na svaku promjenu širine, pozicije ili filtera kolone, a pri zatvaranju sinkrono sprema više layouta i settings zapise. Učinak je vjerovatan; potrebno je mjerenje na velikim layoutima/sporoj bazi.
10. `Carpo.Specific\UgovoriONajmu` ima mnogo sinkronih inline-text SQL poziva iz UI klasa, uključujući konkatenaciju korisničkog `searchString`; to je potvrđen SQL-injection i plan-cache rizik, ne samo performance problem.
11. Build graf je dominantno zasnovan na već instaliranim DLL-ovima i `HintPath`, ne na `ProjectReference`. Samo `Carpo.UI → Carpo.Data` koristi `ProjectReference`; clean build iz klona je zbog toga visokorizičan.
12. Postoje hardkodirane/machine-specific reference i dva različita Jupiter install root-a, uz stale `Version=10.0.0.24` reference metadata prema assembly verziji `10.0.8.36`.
13. `ThreadStatic` globalni kontekst (`Connection`, `PubVars`, `Starter.ImeFirme`) je dio host ugovora. Ne mijenjati ga kao performance “optimizaciju”; prvo treba mapirati Jupiter sesiju i sve background putanje.
14. Najveća classes po veličini — `ComponentGrid` (4174), `SqlHelper` (2556), `Events` (2024), `ComponentBase` (1921) i `Manager` (1443 linije) — nose više odgovornosti. Velicina sama nije performance dokaz, ali kod potvrđuje problematične hot pathove.
15. Prvi cilj nije rewrite niti masovni async/DI: instrumentirati tri reprezentativne granule, ukloniti sinkrono blokiranje i resource propuste, a zatim mjerljivo smanjiti duple refresh/SQL kaskade.

---

# 2. MAPA SISTEMA

## Projekti i zavisnosti

Solution sadrži 12 projekata: `Carpo.Structure`, `Common`, `Data`, `Core`, `UI`, `Gen`, `Specific`, `XmlGen`, `Designer`, `CMS`, `Lite` i `TestShell`.

- `Structure` definira XML model (`JRoot`, `JForm`, `JData`, kontrole).
- `Common`, `Data` i `Core` daju pomoćne, SQL i shared runtime funkcije.
- `UI` implementira Starter, form factory, XML parser/serializer, WinForms/DevExpress kontrole i event engine.
- `Gen` je primarni framework/plugin entry assembly.
- `Specific` sadrži domenske Jupiter ekrane i data wrapper-e; nema compile-time potrošača u solutionu pa je runtime/plugin učitavanje izvan repozitorija potrebno potvrditi.
- `TestShell` je WinExe i primarni lokalni integration harness; `Lite`, `CMS` i `Designer` su sekundarni alati/hostovi.

Logički slojevi su `Structure → Common → Data/Core → UI → Gen`, ali fizički MSBuild graf nije takav: gotovo sve međuprojektne veze su DLL reference prema `SharedAssemblies`; jedini potvrđeni `ProjectReference` je `Carpo.UI\Carpo.UI.csproj:1026–1029` prema `Carpo.Data`.

## Entry pointovi i ugovori

1. Produkcijski framework entry: `Carpo.Gen\Start.cs:9–44`, `Carpo.Gen.Start : Starter`.
2. Host ugovor: `Carpo.Structure\Interfaces\IJupiterStart.cs:3–6` deklarira `Run(server, database, username, password, operateriid, menuID, parent, pTop, pLeft, path)`.
3. Stvarna bazna implementacija u repozitoriju: `Carpo.UI\Connector\Abstract\Starter.cs:78–139`, s kompatibilnim dev/host parametrima i svojstvima (`ModuleId`, `MenuID`, `MainHandle`, `isModal`, `procID`).
4. Potvrđeni lokalni pozivatelji: `Carpo.TestShell\Main.cs` i `Carpo.Lite\Program.cs`. Jupiter production loader nije u repozitoriju — **hipoteza**, ne dokaz.

## Tri reprezentativna runtime toka

### A. Cold start Manager granule

`Carpo.Gen.Start.RegisterForm` (`Carpo.Gen\Start.cs:15–44`) bira `Forms.Manager`; `Starter.Run` postavlja globalni/session kontekst, deserializira XML i registrira formu (`Starter.cs:78–139`, `162–205`, `265–367`). Manager konstruktor učitava settings i postavlja dozvole; `TreeWithGrids_Load` puni kontrole, tree i master grid (`Manager.cs:365–391`). Grid zatim ide preko `ComponentGrid.Populate → DataDefinition → Core.SetData → Common.ExecuteQuery → gridControl.DataSource`.

Potvrđeni paralelni/sekvencijalni SQL kandidati prije prve korisničke akcije: Manager settings, permissions, vrijednost tree filtera, tree data i master grid data. Broj i međusobni redoslijed XML-driven detaljnih upita treba izmjeriti.

### B. XML event/action → data → UI

`JupiterShared`/`ComponentBase` event šalje `Events.RunNotification`; `Events.RunActions` obrađuje XML akcije; `CheckData/NotifyData` poziva `Logic.Data.Core.ExecuteData`, `GetDataSet` ili `ExecuteScalar`; `Core.SetData` zove `Carpo.Data.Common`; reader rezultat ide u `CurrentForm.DataList`, nakon čega sljedeće control akcije osvježavaju UI. Dokaz: `Carpo.UI\Logic\Events.cs:270–472`, `1825–1893`; `Core.cs:96–172`, `289–314`.

### C. Tree/filter → master → detail kaskada

`Manager` puni tree, `ComponentTree.PopulateTreeList` sinkrono veže `DataTable`; promjena focused node pokreće XML `focusedrowchanged`, koji može pozvati `control populate` za master, a master focus zatim može puniti detail grid. Framework wiring je potvrđen; konkretni detail stored procedure pozivi su u XML-u koji nije bio dostupan.

## Data ownership

- XML se učitava kao `XDocument`, mijenja u `Serializer`, zatim `XmlSerializer` deserializira u `JRoot`.
- SQL reader putanje većinom materializiraju `DataSet`; `Core.LoadingFormData` kopira tablice u `CurrentForm.DataList`.
- Grid se primarno veže na `DataSet.Tables[0]`.
- `DataSet`, `DataTable` i layout `MemoryStream` instance imaju ownership u formi/komponenti, ali dio lifecyclea je implicitno vezan za Dispose kontrole.

---

# 3. TOP NALAZI

## F-01
- ID: F-01
- Naziv: Sinkroni SQL u podrazumijevanom grid populate toku
- Kategorija: UI odziv / runtime / SQL
- Prioritet: P0
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.UI\Components\Base\ComponentGrid.cs`
- Klasa/metoda: `ComponentGrid.GetDataAsync()` bez `FocusType`, `1241–1262`
- Dokaz iz koda: `await Task.FromResult<DataSet>(dataDefinition.GetDataSet(...))` na liniji 1251 evaluira `GetDataSet` prije kreiranja dovršenog taska. `Core.SetData` potom poziva sinkroni ADO.NET (`Core.cs:141–169`; `Common.cs:36–114`).
- Problem: naziv metode sugerira async, ali SQL, `DataSet` materializacija i priprema podataka rade na UI niti.
- Mogući produkcijski efekat: freeze pri otvaranju/refreshu velikih gridova, Windows “Not responding”, odgođen repaint i input.
- Predložena promjena: nakon baselinea uvesti jedan kontrolirani `LoadDataAsync` put: SQL/materializaciju raditi izvan UI niti s lokalnim, eksplicitno proslijeđenim connection stringom; `SetData`, DevExpress i svi form state pristupi ostaju na UI niti. Ne koristiti zajednički `SqlConnection` ni `ThreadStatic` state u workeru. U početku uključiti samo za opt-in XML action/granule.
- Rizik promjene: srednji — `Parser`, `ThreadStatic Connection/PubVars` i form state nisu dokazano thread-safe.
- Procjena rada: M
- Način testiranja: Characterization TestShell granula; provjeriti master/detail focus redoslijed, cancel/close tijekom loada, x86, više modalnih formi.
- Mjerljivi kriterijum uspjeha: isti SQL rezultat i isti broj focus događaja; UI watchdog ne bilježi blok >200 ms tijekom SQL čekanja; prije/poslije P50/P95 trajanje grid loada.
- Zavisnosti od drugih promjena: F-03 instrumentacija, F-04 race zaštita, analiza ThreadStatic konteksta.

## F-02
- ID: F-02
- Naziv: Fire-and-forget grid load pokreće focus prije bindinga
- Kategorija: Pouzdanost / UI / SQL kaskade
- Prioritet: P0
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.UI\Components\Base\ComponentGrid.cs`
- Klasa/metoda: `ComponentGrid.Populate`, oko `919–940`; `GetDataAsync(FocusType)`, `1115–1139`
- Dokaz iz koda: poziva se `_ = GetDataAsync()` bez awaita, dok se `SetState(focusType)` i `FocusedRowChanged(null)` izvršavaju odmah. Binding se radi kasnije u `SetData(ds)` na 1126.
- Problem: master/detail event kaskada može čitati stari ili prazan datasource; komentar u kodu već spominje problem dvostrukog focusa.
- Mogući produkcijski efekat: dupli SQL pozivi, detail puni pogrešnim ključem, nedeterminističan prvi red, intermitentni bugovi.
- Predložena promjena: zamijeniti dva puta jednim awaitable state machineom: generacijski token/sequence id po gridu, fokus samo nakon uspješnog bindinga aktualne generacije, eksplicitna politika “initial focus once”. Ne dozvoliti paralelne populate zahtjeve za istim gridom bez coalescinga.
- Rizik promjene: srednji — XML akcije mogu se oslanjati na postojeći, iako nekonzistentan, redoslijed.
- Procjena rada: M
- Način testiranja: logirati `populateId`, datasource rows, selected key i svaki `focusedrowchanged`; testirati brzi filter, promjenu tree noda i zatvaranje forme tijekom loada.
- Mjerljivi kriterijum uspjeha: jedan initial-focus signal po populate generaciji; nema detail upita s null/stalim ključem; SQL count po akciji ne raste.
- Zavisnosti od drugih promjena: F-01 ili zasebni synchronisation-only PR; F-03.

## F-03
- ID: F-03
- Naziv: Nema pouzdanog baselinea za startup, grid, SQL i memoriju
- Kategorija: Mjerenje / pouzdanost promjena
- Prioritet: P0
- Pouzdanost: potvrđeno
- Projekat i fajl: `Starter.cs`, `ComponentGrid.cs`, `Core.cs`, `Common.cs`, `Manager.cs`
- Klasa/metoda: postojeći markeri `Starter.Run:87,138`, `ComponentGrid.SetData:1344`, `Serializer.DeserializeXml:56–123`
- Dokaz iz koda: postoje `Logger.InfoTest/InfoData` pozivi, ali nema jedinstvenog correlation ID-a, SQL duration/count metrike ni UI stall/memory mjerenja.
- Problem: bez baselinea nije moguće razlikovati SQL, XML, DevExpress layout i action pipeline trošak niti sigurno dokazati poboljšanje.
- Mogući produkcijski efekat: optimizacije mogu promijeniti ponašanje bez stvarnog učinka ili pogoršati sporu granulu.
- Predložena promjena: minimalna, opt-in `CarpoPerformanceScope` instrumentacija s `Stopwatch`, granule/action correlation ID-om i sanitiziranim command name/hashom. Ne logirati lozinku ni literalne osjetljive parametre.
- Rizik promjene: mali ako je isključena po defaultu i log format append-only.
- Procjena rada: S
- Način testiranja: TestShell cold/warm run; validirati da logging ne mijenja rezultat i ne sadrži password/certifikat/PII.
- Mjerljivi kriterijum uspjeha: za svaku akciju vidljivi su count i zbroj SQL vremena, XML faze, grid bind, UI lag i private bytes snapshot.
- Zavisnosti od drugih promjena: nema.

## F-04
- ID: F-04
- Naziv: Event/action pipeline blokira UI i miješa vanjske integracije sa SQL akcijama
- Kategorija: UI odziv / arhitektura / pouzdanost
- Prioritet: P0
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.UI\Logic\Events.cs`
- Klasa/metoda: `RunActions`, `270–472`; JIR akcija `347–407`
- Dokaz iz koda: u jednoj metodi se stvaraju `SqlContext`, Outlook kontakt, certifikat, XML, fiskalni SOAP i `Core` data pozivi. Linija 388 radi `Task.Run(() => web.SendSoapRequest(...)).GetAwaiter().GetResult()`.
- Problem: sync-over-async i COM/mrežni pozivi blokiraju UI; odgovornost je preširoka, retry/error/cancel ponašanje nije izolirano.
- Mogući produkcijski efekat: zamrznuta forma za vrijeme SOAP/Outlook poziva, nejasan rollback i teško reproducibilni integration bugovi.
- Predložena promjena: prvo izdvojiti bez promjene public XML action ugovora: `IExternalActionExecutor` sa implementacijama `OutlookActionExecutor` i `FiscalizationActionExecutor`, te `IDataActionExecutor`. `Events` ostaje kompatibilni dispatcher. Vanjski rad se kasnije uvodi kao modal/progress workflow s cancellation granicom; SQL update tek nakon potvrđenog odgovora.
- Rizik promjene: veliki za fiskalizaciju/COM; ne uvoditi paralelizam preko postojeće transakcije.
- Procjena rada: L
- Način testiranja: fake executor unit testovi za dispatcher; manual TestShell/Jupiter smoke za Outlook i fiskalizaciju u certificiranom okruženju; simulirati timeout i odbijeni SOAP.
- Mjerljivi kriterijum uspjeha: UI je repaintable tijekom čekanja; jedan SOAP request i jedan save po akciji; postoje korelirani error i outcome logovi.
- Zavisnosti od drugih promjena: F-03, ugovor s poslovnim vlasnikom fiskalizacije.

## F-05
- ID: F-05
- Naziv: Transakcijski SQL wrapper ne disposea SqlCommand
- Kategorija: Memorija / SQL resursi
- Prioritet: P1
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Data\Common.cs`
- Klasa/metoda: `ExecuteQueryInTran:205–242`, `ExecuteAzurInTran:244–280`
- Dokaz iz koda: `new SqlCommand(commandText, connection, tranImported)` na 212 odnosno 249 nema `using`, `Dispose` ni `finally`.
- Problem: deterministički ownership `SqlCommand` i parametara nije definiran. Kod je posebno osjetljiv u serijama data actions unutar transakcije.
- Mogući produkcijski efekat: odgođeno oslobađanje managed/native resursa, više GC pritiska i potencijalno zadržani parametri tijekom velikih batch actiona.
- Predložena promjena: omotati command u `using`, bez zatvaranja externally-owned connection/transaction; zadržati postojeće API-je i ponašanje output parametara.
- Rizik promjene: mali, ali provjeriti čita li caller command nakon povratka (nema dokaza da čita).
- Procjena rada: S
- Način testiranja: integration test s lokalnim SQL Serverom/stub SP-om: commit, rollback, reader/nonquery/scalar, output parametri; proces/handle i GC snapshot prije/poslije 1.000 poziva.
- Mjerljivi kriterijum uspjeha: identičan rezultat i transaction semantics; nema rastućih `SqlCommand` instanci u profileru.
- Zavisnosti od drugih promjena: F-03 preporučen, nije obavezan.

## F-06
- ID: F-06
- Naziv: SQL helper overloadovi ne disposeaju SqlCommand i nemaju centralnu observabilnost
- Kategorija: SQL resursi / održavanje
- Prioritet: P1
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Data\SqlHelper.cs`
- Klasa/metoda: `ExecuteNonQuery(SqlConnection...):294–310`, transakcijski overload `379–394`, `ExecuteDataset:558–584`, reader helper `747–782`
- Dokaz iz koda: `var cmd = new SqlCommand()` bez `using`; dataset adapter se disposea, command ne. `PrepareCommand` (`140–177`) otvara konekciju, ali nema standardnu command timeout politiku.
- Problem: stari helper duplicira `Carpo.Data.Common` odgovornosti i ima neujednačen resource lifecycle/timeout/logging.
- Mogući produkcijski efekat: resource churn, neujednačeno ponašanje po call-siteu i teško određivanje sporog SP-a.
- Predložena promjena: prvo testirati `SqlHelper`, zatim lokalno dodati `using` za command gdje reader ownership ne zahtijeva njegov opstanak. Za reader overload command mora ostati živ do close readera — ne mijenjati bez wrappera koji posjeduje i reader i command.
- Rizik promjene: srednji za `ExecuteReader`; mali za nonquery/dataset/scalar.
- Procjena rada: M
- Način testiranja: posebni testovi za connection-string/external-connection/transaction/reader overloadove; provjeriti `CommandBehavior.CloseConnection`.
- Mjerljivi kriterijum uspjeha: svi overloadovi zadržavaju rezultate i connection ownership; profiler ne pokazuje rast komandi.
- Zavisnosti od drugih promjena: F-03; ne spajati s velikim prepisivanjem Data layera.

## F-07
- ID: F-07
- Naziv: DataSet-first model i višestruka materializacija podataka
- Kategorija: SQL performanse / memorija
- Prioritet: P1
- Pouzdanost: potvrđeno za obrazac; potrebno mjerenje/profilisanje za učinak
- Projekat i fajl: `Carpo.UI\Logic\Data\Core.cs`, `Carpo.Data\Common.cs`, `ComponentGrid.cs`
- Klasa/metoda: `Core.SetData:141–152, 289–314`; `Common.ExecuteQuery:36–114`; `ComponentGrid.SetData:1264–1344`
- Dokaz iz koda: reader putanja prima `DataSet`; rezultat se stavlja u `CurrentForm.DataList` ili tablice, a grid koristi samo `Tables[0]`.
- Problem: DataSet nosi schema, constraints i kolekcijski overhead i često se zadržava kroz formu. Bez row-count/column-count telemetrije nije opravdano mijenjati sve pozive.
- Mogući produkcijski efekat: LOH alokacije, Gen2 GC, x86 address-space pressure i sporiji binding na velikim granula gridovima.
- Predložena promjena: dodati metrike `tables/rows/columns/estimated bytes`; zatim za odabrane single-result, read-only grid upite uvesti `DataTable` overload ili `SqlDataReader → DataTable`, bez mijenjanja XML ugovora. Paging/server mode razmotriti samo za potvrđene velike setove i nakon provjere group/sort/export featurea.
- Rizik promjene: srednji — XML može očekivati više result setova i `DataList` tablice.
- Procjena rada: M po ciljanom flowu
- Način testiranja: usporediti schema, DBNull, binding, export, group summaries i XML akcije za odabranu granulu.
- Mjerljivi kriterijum uspjeha: ista funkcionalnost; smanjeni private bytes/managed allocation po loadu bez dodatnih SQL poziva.
- Zavisnosti od drugih promjena: F-03, tipične veličine podataka, SQL planovi.

## F-08
- ID: F-08
- Naziv: Grid binding pokreće skupe layout operacije bez batching granice
- Kategorija: DevExpress UI performanse
- Prioritet: P1
- Pouzdanost: potvrđeno za obrazac; potrebno mjerenje/profilisanje za trošak
- Projekat i fajl: `Carpo.UI\Components\Base\ComponentGrid.cs`
- Klasa/metoda: `SetData:1264–1344`, `RestoreLayout:1166–1207`, `GetDataAsync:1115–1139,1241–1262`
- Dokaz iz koda: po bindu se pozivaju `SetDataColumnsProp`, eventualni `InitGrid`, `DataSource=`, `BestFitColumnsInit`, template vidljivost, restore layout i opcionalni `BestFitColumns`; `BeginUpdate/EndUpdate` oko load-a su komentirani (`1117,1136,1245,1257`).
- Problem: DevExpress može re-kalkulirati view/layout više puta za isti datasource; BestFit, group summary i expand groups rastu s brojem redova/kolona.
- Mogući produkcijski efekat: spor refresh, flicker, dug UI freeze na širokim ili grupiranim gridovima.
- Predložena promjena: uvesti mali `GridUpdateScope` s `gridControl.BeginUpdate/EndUpdate` i/ili `gridView.BeginDataUpdate/EndDataUpdate` prema DevExpress API-ju, u `try/finally`. Scope mora obuhvatiti datasource/kolone/layout, ali ne SQL. Preskočiti BestFit za refresh osim kada definicija izričito traži. Ne uvoditi Server Mode bez feature matrixa.
- Rizik promjene: srednji — redoslijed layout/focus/group eventa.
- Procjena rada: M
- Način testiranja: TestShell s velikim master gridom, sorting/grouping/summaries/export/printed selected rows i spremljenim layoutom.
- Mjerljivi kriterijum uspjeha: manje view layout/paint vremena, nema flickera, identičan layout/focus/sum rezultat.
- Zavisnosti od drugih promjena: F-03, DevExpress feature smoke checklist.

## F-09
- ID: F-09
- Naziv: foreach-row akcije izvršavaju kompletan action pipeline po retku
- Kategorija: SQL / runtime
- Prioritet: P1
- Pouzdanost: potvrđeno za petlju; potrebno mjerenje/profilisanje za SQL N+1
- Projekat i fajl: `Carpo.UI\Components\Base\ComponentGrid.cs`, `Carpo.UI\Logic\Events.cs`
- Klasa/metoda: `SetNotifyForEachRow:2505–2547`; `Events` foreachrow grane `1685–1712`, `1966+`
- Dokaz iz koda: za svaki selektirani ili svaki `DataRowCount` red postavlja se `DataRowRef` i poziva `RowForEachRowAction`.
- Problem: ako XML akcija sadrži data action, broj SQL/COM/print operacija raste linearno po retku na UI niti.
- Mogući produkcijski efekat: ekstremno spora batch akcija, partial failure, dugi lockovi i neodzivan UI.
- Predložena promjena: instrumentirati rows/action/SQL count. Za potvrđene SQL foreach scenarije dodati bulk SP/TVP action tip koji eksplicitno čuva semantiku. Ne paralelizirati pozive preko iste konekcije/transakcije.
- Rizik promjene: srednji do veliki — poslovna semantika, redoslijed i partial failure.
- Procjena rada: M–L po akciji
- Način testiranja: 1/10/100 selektiranih redova; rollback/partial failure; usporediti rezultat i SQL count.
- Mjerljivi kriterijum uspjeha: broj round-tripova se ne povećava s brojem redova za batch SP; identični poslovni rezultat.
- Zavisnosti od drugih promjena: F-03; SP izmjena i planovi.

## F-10
- ID: F-10
- Naziv: Serializer radi rekurzivna XML proširenja i SQL `partsql` pri startupu
- Kategorija: Startup / CPU / I/O / memorija
- Prioritet: P1
- Pouzdanost: potvrđeno za obrazac; potrebno mjerenje/profilisanje za dominantni trošak
- Projekat i fajl: `Carpo.UI\Logic\Serializer.cs`
- Klasa/metoda: `DeserializeXml:54–125`, `GetParts:127–188`, `GetPartsSql:190–291`, `GetForms:294–349`
- Dokaz iz koda: `DeserializeXml` radi `GetPartsSql`, `GetForms`, `GetParts`, `GetCustomControls`, `GetDatalistInfo`, potom `XmlSerializer.Deserialize`; `GetParts/GetForms` opet pozivaju `serializerRecursion`; `GetPartsSql` prvo deserializira `JRoot` radi data definicije i može zvati SQL.
- Problem: višestruki LINQ `Descendants`, XML dokumenti, zamjene elemenata, disk/DMS i SQL miješaju se u startup hot pathu. Rekurzija može višestruko posjetiti dijelove.
- Mogući produkcijski efekat: spor cold start, visoke alokacije i nejasno porijeklo startup SQL-a.
- Predložena promjena: fazno instrumentirati svaki part/form/partsql load s cache keyem i depthom; dodati cycle/depth guard. Tek nakon mjerenja cacheirati samo immutable, versioned XML definicije s bounded LRU i invalidacijom po path+last-write/DMS version+database. Ne cacheirati `JRoot` koji se poslije mutira po formi.
- Rizik promjene: srednji — include override prioriteti i DMS kompatibilnost.
- Procjena rada: M za mjerenje/guard, L za cache
- Način testiranja: test fixture XML s nested forms/parts, missing optional part, DMS mismatch i dva database overridea; usporediti canonical output XML/JRoot.
- Mjerljivi kriterijum uspjeha: isti deserializirani JRoot; log pokazuje broj loaded files i ukupno vrijeme po fazi; cache hit nema stale definiciju.
- Zavisnosti od drugih promjena: F-03, DMS invalidation ugovor.

## F-11
- ID: F-11
- Naziv: Parser stvara alokacije i regex u često pozivanoj evaluaciji
- Kategorija: CPU / GC / održavanje
- Prioritet: P2
- Pouzdanost: potvrđeno za obrazac; potrebno mjerenje/profilisanje za hotness
- Projekat i fajl: `Carpo.UI\Logic\Parser.cs`, `ParserDirect.cs`, `Evaluate.cs`
- Klasa/metoda: `Parser.GetLogicPartValue:58–192`; `GetDirectByPart:383–403`; `Evaluate` regex konstrukcija
- Dokaz iz koda: svaka evaluacija radi `ToLower`, `Split('.').ToList`, više LINQ pretraga; `Regex.IsMatch(str, "^(@|#)")` se poziva u static helperu. Parser može instancirati `Data.Core` i evaluirati izraze `{...}`.
- Problem: parametri i actioni mogu zvati parser mnogo puta po eventu/retku; cache nije automatski siguran jer rezultat ovisi o formi, kontroli, current rowu i globalnom stanju.
- Mogući produkcijski efekat: dodatni Gen0 GC i CPU u grid eventovima, ali nije dokazano da je dominantan.
- Predložena promjena: prvo sample profiler. Sigurne mikro-promjene: `StringComparison.OrdinalIgnoreCase`, `StartsWith('@')/StartsWith('#')` umjesto regexa, nealokirajući split gdje je opravdano. Eventualni cache samo za tokenizirani, immutable plan; evaluacija ostaje per context i cache se bounded/invalidira na XML definition version.
- Rizik promjene: mali za mikro-promjene, srednji za cache.
- Procjena rada: S–M
- Način testiranja: characterization matrica za `this`, parent/global, data table, method, parameter i `{expression}`; benchmark nakon funkcionalne jednakosti.
- Mjerljivi kriterijum uspjeha: identični parser output za corpus XML-a; manje allocations/call u profileru.
- Zavisnosti od drugih promjena: F-03, XML corpus.

## F-12
- ID: F-12
- Naziv: Manager layout persistence radi sinkrono i nede-bounced
- Kategorija: UI / disk/SQL I/O / memorija
- Prioritet: P2
- Pouzdanost: potvrđeno za obrazac; potrebno mjerenje/profilisanje za učinak
- Projekat i fajl: `Carpo.UI\Forms\Manager\Manager.cs`
- Klasa/metoda: `SaveSettings:407–423`, `TrMain_FocusedNodeChanged:335–348`, `Manager_FormClosing:1220–1334`
- Dokaz iz koda: svaka `ColumnWidthChanged`, `ColumnFilterChanged`, `ColumnPositionChanged` radi `SaveLayoutToStream` i `stream.ToArray`; FormClosing iterira settings i sinkrono zove `service.SaveSettings`.
- Problem: drag kolone često emitira mnogo eventa; zatvaranje forme može raditi više SQL/disk/layout operacija na UI niti.
- Mogući produkcijski efekat: lag pri pomicanju kolone i sporo zatvaranje; memorija raste s brojem spremljenih node layouta.
- Predložena promjena: 250–500 ms debounce samo za in-memory snapshot; persistence zadržati pri explicit save/form close. Dodati limit max node layouta/bytes i telemetry. `MemoryStream` koristiti u `using` gdje nije field (`SaveSettings`).
- Rizik promjene: mali do srednji — neočekivano zatvaranje prije debouncea mora se pokriti snapshotom na FormClosing.
- Procjena rada: S–M
- Način testiranja: brzo resize/reorder/filter, promjena tree nodea, normal close i forced process termination.
- Mjerljivi kriterijum uspjeha: manje serializacija po drag akciji; restauriran isti layout nakon restarta; p95 close time ne raste.
- Zavisnosti od drugih promjena: F-03.

## F-13
- ID: F-13
- Naziv: UgovoriONajmu koristi konkatenirani text SQL iz UI sloja
- Kategorija: SQL performanse / sigurnost / održavanje
- Prioritet: P1
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Specific\UgovoriONajmu\Ugovori.cs`, `UgovoriDetail.cs`
- Klasa/metoda: `GetDateSource:800–812`, `TraziOrgShemu:929–941`, `GetPubVar:911–926` i srodni data source helperi
- Dokaz iz koda: primjeri `exec ... " + Id`, `"@searchstring='" + searchString + "'"` i `"select ... where varime = " + varIme`; svi se šalju kao `DataType.Text`.
- Problem: SQL literal konkatenacija iz UI-a nosi injection/escaping rizik, razbija plan reuse i povezuje formu, SQL i user feedback.
- Mogući produkcijski efekat: sigurnosni propust, query parse/plan overhead, greške na apostrofima i spor autocomplete/pretraga.
- Predložena promjena: prvo izdvojiti `IUgovoriDataGateway` iza postojećih private metoda, s eksplicitnim `SqlParameter` tipovima i stored procedure pozivima. Zadržati postojeće UI metode kao adaptere. Za `searchString` obavezno parametrizirati prvi.
- Rizik promjene: srednji — SP ugovor i implicitne SQL konverzije treba provjeriti.
- Procjena rada: M po grupi upita
- Način testiranja: SQL integration test za apostrof, `%`, null, large id i schema; TestShell smoke forme; prikupiti execution plan prije/poslije.
- Mjerljivi kriterijum uspjeha: nema konkateniranog user inputa; isti result set; query store/Extended Events pokazuje parametriziran shape i plan reuse.
- Zavisnosti od drugih promjena: pristup SP definicijama i DB test okruženju.

## F-14
- ID: F-14
- Naziv: Build ne reproducira pouzdano source dependency graf
- Kategorija: Build/development time / deployment
- Prioritet: P1
- Pouzdanost: potvrđeno za konfiguraciju; clean-build kvar je hipoteza dok se ne proba izolirani agent
- Projekat i fajl: svih 12 `.csproj`, posebno `Carpo.UI\Carpo.UI.csproj:82–158`; `Carpo.sln:85–285`
- Klasa/metoda: MSBuild konfiguracija
- Dokaz iz koda: gotovo sve Carpo reference su `HintPath` prema instaliranom `Program Files (x86)\...\SharedAssemblies\Spin`; samo UI→Data je `ProjectReference`. UI ima `C:\Users\valentino.jagodic\Desktop\...` DevExpress paths. Output putanje variraju između “Jupiter Software” i “Spin Informatica”; stale reference metadata je `10.0.0.24`, dok `AssemblyInfo.cs:34–35` navodi `10.0.8.36`.
- Problem: build može protivno očekivanju koristiti staru DLL verziju; developer/CI zahtijeva lokalnu instalaciju; x64 solution mapiranja često vode na AnyCPU, a Debug|x86 nije svugdje stvarni x86.
- Mogući produkcijski efekat: nereproducibilan artefakt, “works on my machine”, pogrešan DLL u SharedAssemblies.
- Predložena promjena: prvo read-only clean build probe u izoliranom worktree/agent VM-u. Zatim dodati MSBuild property rootove (`JupiterSharedAssemblies`, `DevExpressLibPath`) bez mijenjanja host deploya; migrirati `ProjectReference` leaf-up (Structure→Common→Data→Core→UI) uz privremeni compatibility copy target; standardizirati x86 build artefact staging izvan Program Files.
- Rizik promjene: srednji do veliki — host binding i deployment redoslijed.
- Procjena rada: L
- Način testiranja: clean checkout build x86, AssemblyLoad trace, TestShell x86 smoke, hash/manifests svih deploy DLL-ova i rollback kopija.
- Mjerljivi kriterijum uspjeha: reproducibilan x86 build bez machine-specific patha; manifest pokazuje očekivane verzije; Jupiter učitava identične assembly verzije.
- Zavisnosti od drugih promjena: deployment owner, DevExpress licensing/package izvor, host assembly-binding provjera.

## F-15
- ID: F-15
- Naziv: Globalni ThreadStatic context i Specific static state ograničavaju sigurnu asinkronizaciju
- Kategorija: Arhitektura / pouzdanost
- Prioritet: P2
- Pouzdanost: potvrđeno za stanje; potreba za promjenom je hipoteza
- Projekat i fajl: `Carpo.Data\Connection.cs:7–55`, `Carpo.Core\PubVars.cs:7–27`, `Starter.cs:39–47`, `Carpo.Specific\Grafko\Graf.cs:12–29`
- Klasa/metoda: statički session/context state; `Graf.CrtajGraf`
- Dokaz iz koda: connection string, ADO string, DB, user/password i PubVars su `[ThreadStatic]`; Graf drži statičke liste i ulazne argumente za generiranje dijagrama.
- Problem: worker thread ne nasljeđuje ThreadStatic state; Graf nije siguran za paralelne instance. Ovo objašnjava zašto “samo dodati Task.Run” nije sigurna korekcija.
- Mogući produkcijski efekat: null/krivi connection na workeru, cross-instance korupcija grafa, nedeterministični async bugovi.
- Predložena promjena: ne dirati framework globals globalno. Za svaku novu async putanju eksplicitno prenijeti immutable `OperationContext` (connection string, operater, module, cancellation). `Graf` lokalizirati state u kratkoživući context objekt, uz zadržan static facade API.
- Rizik promjene: srednji; Jupiter može zavisiti od thread-affinity semantike.
- Procjena rada: M po izoliranom području
- Način testiranja: dvije istodobne TestShell forme/granule, x86; background load bez pristupa globalu; Graf deterministični output test.
- Mjerljivi kriterijum uspjeha: background rad ne čita ThreadStatic state; dvije instance ne miješaju rezultat.
- Zavisnosti od drugih promjena: F-01/F-04, host session analiza.

---

# 4. QUICK WINS

## Manje od jednog dana

1. **Performance correlation logging (F-03).** Dodati opt-in `Stopwatch` markere u `Starter.Run`, `Serializer.DeserializeXml`, `Core.SetData`, `ComponentGrid.SetData` i `ComponentTree.PopulateTreeList`. Rezultat: baseline po granuli bez promjene poslovne logike.
2. **Dispose transakcijskih SqlCommandova (F-05).** Lokalni `using` u dvije metode `Common.cs`, uz SQL integration test. Rezultat: jasniji resource ownership.
3. **Sanitizirati logove.** `Starter.RegisterForm` trenutno logira `username` i `password` (`Starter.cs:295–299`). Ukloniti password iz log poruke. Rezultat: izravno smanjen sigurnosni rizik; bez funkcionalne promjene.
4. **Ukloniti prazne `catch { throw; }` blokove u ciljanim SQL wrapperima.** Ne daje performance dobit, ali čuva originalni stack bez lažne “obrade”; izvesti samo uz test. Rezultat: bolja dijagnostika.

## 1–2 dana

1. **Grid update scope prototip na jednoj reprezentativnoj granuli (F-08).** Batching oko datasource/layout operacija s `try/finally`; mjeriti refresh prije/poslije.
2. **Manager debounce layout snapshot (F-12).** Debounce in-memory `SaveSettings`; FormClosing ostaje final flush. Rezultat: manje layout serializacija na resize.
3. **Ugovori parametrizacija za `TraziOrgShemu` (F-13).** Najveći risk user inputa, mali zahvat ako SP podržava parametre. Rezultat: pravilno apostrofi i plan reuse.

## Do jedne sedmice

1. **Grid populate sequencing (F-02).** Jedan cancellable/load-generation flow za master grid, s characterization logovima.
2. **SQL wrapper test harness.** Minimalni integration testovi za `Common` i `SqlHelper` connection/transaction/reader ownership.
3. **Build path centralizacija bez ProjectReference migracije (F-14).** Shared props + CI preflight koji validira paths i manifest artefakata.
4. **XML parser test corpus i serializer phase measurement (F-10/F-11).**

---

# 5. PERFORMANCE BACKLOG

Sortirano prema očekivana korist / rizik / rad, bez izmišljanja numeričkih dobitaka:

1. Baseline instrumentacija i action correlation — korist velika, rizik mali, S.
2. `SqlCommand` dispose u `Common` transakcijskim metodama — korist srednja, rizik mali, S.
3. Uklanjanje lozinke iz loga — korist velika za sigurnost, rizik mali, S.
4. Manager layout debounce — korist srednja, rizik mali-srednji, S–M.
5. Grid update batching — korist srednja-velika za široke gridove, rizik srednji, M.
6. Grid load sequencing / dupli focus guard — korist velika za pouzdanost i SQL count, rizik srednji, M.
7. Opt-in background fetch za jedan grid flow — korist velika ako SQL je spor, rizik srednji, M.
8. Parametrizacija Specific Ugovori call-siteova — korist velika (sigurnost i SQL), rizik srednji, M.
9. XML serializer cycle guard i phase telemetry — korist srednja, rizik mali-srednji, M.
10. Ciljani DataTable/paging za potvrđeno velike gride — korist velika, rizik srednji-veliki, M–L.
11. Bulk/TVP zamjena potvrđenih foreach-row SQL akcija — korist velika, rizik veliki, L.
12. Izdvajanje Outlook/Fiscal executora — korist srednja-velika, rizik veliki, L.
13. Parser micro-optimizations nakon profiler samplea — korist mala-srednja, rizik mali, S–M.
14. Bounded XML definition cache nakon invalidation ugovora — korist srednja, rizik srednji, L.
15. Leaf-up ProjectReference/build staging modernizacija — korist velika za development, rizik srednji-veliki, L.
16. `Graf` static-state izolacija — korist srednja za concurrency, rizik srednji, M.

---

# 6. REFACTORING ROADMAP

## Faza 0: baseline i instrumentacija

- Preduvjeti: TestShell granule, bez izvornog functional refaktora.
- Aktivnosti: F-03, sanitizacija logova, x86 cold/warm test scenario, Extended Events/Query Store pristup za odabranu DB.
- Očekivani rezultat: correlation ID i vrijeme za `startup/xml/SQL/grid-bind/layout/action`; broj SQL poziva po akciji; private bytes/GC snapshot.
- Rollback: config flag gasi dodatne markere; log format append-only.

## Faza 1: sigurni lokalni refaktoring

- Preduvjeti: faza 0 baseline, minimalni SQL wrapper tests.
- Aktivnosti: F-05, sigurni `SqlHelper` non-reader dispose, Manager debounce, password log redakcija, Specific parametrizacija jednog search poziva.
- Očekivani rezultat: definiran ownership resursa i manje lokalnog UI rada bez mijenjanja host/API ugovora.
- Rollback: svaki PR zaseban; DLL versioned deploy i kopija prethodnog SharedAssemblies paketa.

## Faza 2: izdvajanje odgovornosti

- Preduvjeti: characterization scenariji za event pipeline i relevantne XML granule.
- Aktivnosti:
  - `IGridDataLoader`: `Load(JGridDefinition, GridLoadContext)`; preuzima DataDefinition/SQL fetch dio, ne DevExpress.
  - `IExternalActionExecutor`: `CanHandle(JEventAction)` i `Execute(...)`; preuzima Outlook/JIR/ZKI grane iz `Events.RunActions`.
  - `IUgovoriDataGateway`: preuzima private SQL helper metode UI forme.
  - `SqlParameterValue`/helper u `Data` ili `Common`: zamjenjuje cross-module `Virmani.SendIntParamToDatabase`.
- Backward compatibility: `Events`, `ComponentGrid` i XML action stringovi ostaju facade/adapter; ne mijenjati `Starter.Run`, `IJupiterStart` ni plugin class name.
- Očekivani rezultat: testabilni rubovi oko SQL/COM/UI, ne nova apstraktna arhitektura cijelog sustava.
- Rollback: executor/gateway se aktivira samo za jedan action/forma path; stari adapter ostaje.

## Faza 3: optimizacija potvrđenih hot pathova

- Preduvjeti: p95 baseline, SQL planovi i realne veličine gridova.
- Aktivnosti: F-01/F-02 load sequencing, grid update scope, confirmed N+1 bulk SP/TVP, confirmed large-data paging ili DevExpress Server Mode.
- Očekivani rezultat: smanjen UI stall i SQL count na tri ciljana scenarija.
- Rollback: feature flag po granuli/actionu; cache/server mode i async fetch moraju imati kill switch.

## Faza 4: dugoročna arhitektura

- Preduvjeti: stabilan x86 CI build, safety net i deployment manifest.
- Aktivnosti: leaf-up `ProjectReference` migracija, shared MSBuild props/versioning, definition repository/cache, postupno smanjenje ThreadStatic implicitnog statea.
- Očekivani rezultat: reproducibilan build i jasnija granica host/session/ui/data slojeva.
- Rollback: ne mijenjati platformu ni .NET Framework 4.8; zadržati DLL binding/deploy manifest i paralelni build lane dok Jupiter host ne potvrdi.

---

# 7. PRIJEDLOG PRVIH PET PR-OVA

## PR 1 — Dodaj opt-in performance baseline
- Cilj: korelirano mjeriti startup, XML, SQL, binding i actione.
- Zahvaćeni fajlovi: `Starter.cs`, `Serializer.cs`, `Core.cs`, `ComponentGrid.cs`, `ComponentTree.cs`, logger/config.
- Približan opseg: 150–250 LOC, bez promjene public ugovora.
- Rizik: mali.
- Test plan: TestShell cold/warm Manager i Standard granula; provjera sanitizacije loga.
- Očekivani rezultat: usporediv prije/poslije log format s granule/action ID-om.

## PR 2 — Popravi ownership transakcijskih SqlCommandova
- Cilj: deterministički dispose komandi u `Common`.
- Zahvaćeni fajlovi: `Carpo.Data\Common.cs`; novi/postojeći SQL wrapper tests.
- Približan opseg: 30–80 LOC.
- Rizik: mali.
- Test plan: commit/rollback, scalar/nonquery/dataset, output parameter i x86 TestShell smoke.
- Očekivani rezultat: nema promjene SQL rezultata; profiler potvrđuje da command ne ostaje živ.

## PR 3 — Zaštiti grid load redoslijed
- Cilj: spriječiti focus/event prije dovršenog bindanja.
- Zahvaćeni fajlovi: `ComponentGrid.cs`, eventualno `ComponentBase.cs`; characterization log assertions.
- Približan opseg: 150–300 LOC.
- Rizik: srednji.
- Test plan: tree→master→detail, filter, refresh, AutoRefresh, close during load, saved layout.
- Očekivani rezultat: jedna fokus kaskada po aktualnom loadu; nema stale detail keya.

## PR 4 — Batch grid datasource/layout update
- Cilj: smanjiti DevExpress recalculation tijekom `SetData`.
- Zahvaćeni fajlovi: `ComponentGrid.cs`, TestShell scenarij/script.
- Približan opseg: 80–150 LOC.
- Rizik: srednji.
- Test plan: grouping, summary, sorting, export, print selected rows, restored layout, 10k-row representative data.
- Očekivani rezultat: mjerljivo kraći bind/refresh na odabranom gridu bez vizualne regresije.

## PR 5 — Parametriziraj Specific Ugovori pretragu
- Cilj: ukloniti user-input SQL konkatenaciju i izdvojiti mali data gateway.
- Zahvaćeni fajlovi: `Carpo.Specific\UgovoriONajmu\Ugovori.cs`, jedan data helper/gateway, testovi.
- Približan opseg: 100–200 LOC.
- Rizik: srednji.
- Test plan: search s apostrofom/wildcardom/null, existing result equivalence, SQL plan capture.
- Očekivani rezultat: parametrizirani call-site, isti rezultat i stabilan query shape.

---

# 8. ŠTA NE TREBA DIRATI

1. **`Starter.Run`, `IJupiterStart` i `Carpo.Gen.Start` public/plugin signature.** To je potvrđeni host ugovor; mijenjati samo internu implementaciju iza identičnog API-ja.
2. **Globalni `[ThreadStatic]` context kao cjelinu.** `Connection` i `PubVars` su opsežno korišteni. Globalna zamjena DI-jem ili `AsyncLocal` bez Jupiter session analize bi promijenila semantiku.
3. **Masovna async/await konverzija.** WinForms synchronization context, DevExpress kontrole i ThreadStatic context zahtijevaju jedan opt-in flow s explicit contextom, ne bulk zamjenu.
4. **DevExpress Server Mode/Virtual Mode po defaultu.** Grouping, custom summaries, export, selected-row printing, templates i DataTable-specifični actioni trebaju feature matrix; koristiti samo ako baseline pokaže velike skupove.
5. **Fiskalizacija i Outlook COM rewrite u performance PR-u.** To su eksterni poslovni integracijski ugovori; izdvojiti adaptere i testirati u certificiranom okruženju prije optimizacije.
6. **`SqlHelper.ExecuteReader` ownership bez dedicated testova.** Reader može implicitno posjedovati command/connection lifetime; “dodati using” bi mogao prerano zatvoriti command.
7. **Uvođenje cachea za XML/Parser bez invalidacije i limita.** XML se overridea po DB/pathu, a parser rezultat ovisi o runtime contextu; neograničeni cache bi povećao x86 memorijski rizik.
8. **Platform migration na .NET 6/8/9 kao rješenje ovog audita.** Ne rješava postojeće action/SQL/UI design probleme i krši produkcijska ograničenja.

---

# 9. OTVORENA PITANJA

1. Koje su tri najsporije granule, korisničke akcije i ciljane p95/p99 vrijednosti?
2. Koji su tipični i maksimalni redovi/kolone za master/detail/tree/grid, te veličine XML granula?
3. Jesu li XML granule/DMS definicije immutable tijekom procesa, i postoji li version/last-write token za invalidaciju cachea?
4. Koje stored procedure dominiraju po duration/reads/CPU i koji su njihovi actual execution planovi, `SET STATISTICS IO/TIME` i Query Store trendovi?
5. Postoje li connection pool exhaustion, timeout, deadlock ili memory incidenti u produkciji?
6. Koja verzija Jupiter hosta učitava `Carpo.Gen`, kako radi assembly resolution i može li učitati side-by-side DLL paket?
7. Koji je formalni deploy/rollback proces za SharedAssemblies, uključujući zaključane DLL-ove i restart hosta?
8. Postoji li CI/TFS build definition, DevExpress package source/licenca i clean build agent bez lokalnog Jupiter installa?
9. Podržava li svaka kritična granula x86 samo, i postoji li stvarna x64 produkcijska potreba?
10. Koji XML actioni koriste `foreachrow`, `populateasync`, fiskalizaciju, Outlook i external COM?
11. Koja je točna transakcijska semantika multi-action `Events` toka; smiju li vanjske SOAP/COM operacije biti u istoj transakcijskoj odluci?
12. Jesu li postojeći SQL logovi prihvatljivi s aspekta PII/credentials i retention politike?

---

# 10. KONAČNA PRIORITIZOVANA LISTA

1. Uvesti opt-in baseline/correlation instrumentaciju — korist: velika; rizik: mali; rad: S; profilisanje prije: ne; redoslijed: 1.
2. Redaktirati password i osjetljive parametre iz logova — korist: velika; rizik: mali; rad: S; profilisanje prije: ne; redoslijed: 2.
3. Dodati SQL wrapper characterization/integration testove — korist: velika; rizik: mali; rad: M; profilisanje prije: ne; redoslijed: 3.
4. Dispose `SqlCommand` u transakcijskim `Common` putanjama — korist: srednja; rizik: mali; rad: S; profilisanje prije: ne; redoslijed: 4.
5. Izmjeriti tri Manager cold/warm scenarija u TestShell x86 — korist: velika; rizik: mali; rad: S; profilisanje prije: da, to je aktivnost; redoslijed: 5.
6. Dodati grid load generation/focus guard — korist: velika; rizik: srednji; rad: M; profilisanje prije: da; redoslijed: 6.
7. Uvesti DevExpress update scope na jednom gridu — korist: srednja-velika; rizik: srednji; rad: M; profilisanje prije: da; redoslijed: 7.
8. Debounce Manager layout snapshot — korist: srednja; rizik: mali-srednji; rad: S; profilisanje prije: da; redoslijed: 8.
9. Parametrizirati Ugovori `searchString` i inline SQL — korist: velika; rizik: srednji; rad: M; profilisanje prije: SQL planovi; redoslijed: 9.
10. Dodati serializer phase/depth telemetry i cycle guard — korist: srednja; rizik: mali-srednji; rad: M; profilisanje prije: da; redoslijed: 10.
11. Izmjeriti i ukloniti potvrđene duple startup populate pozive — korist: velika; rizik: srednji; rad: M; profilisanje prije: da; redoslijed: 11.
12. Izdvojiti SQL parameter helper iz Specific UI klase — korist: srednja; rizik: mali; rad: S; profilisanje prije: ne; redoslijed: 12.
13. Lokalizirati `Graf` static state iza instance/contexta — korist: srednja; rizik: srednji; rad: M; profilisanje prije: concurrent usage; redoslijed: 13.
14. Izmjeriti DataSet row/column/allocation i odabrati jedan DataTable optimizacijski kandidat — korist: velika; rizik: srednji; rad: M; profilisanje prije: da; redoslijed: 14.
15. Zamijeniti potvrđeni foreach-row SQL batch TVP/stored procedure akcijom — korist: velika; rizik: veliki; rad: L; profilisanje prije: da; redoslijed: 15.
16. Izdvojiti Outlook/fiskalizaciju u compatibility-preserving executore — korist: srednja-velika; rizik: veliki; rad: L; profilisanje prije: ne; redoslijed: 16.
17. Centralizirati MSBuild pathove/verziju i ukloniti personal DevExpress paths — korist: velika; rizik: srednji; rad: M; profilisanje prije: isolated clean build; redoslijed: 17.
18. Migrirati source dependency graf na ProjectReference leaf-up — korist: velika; rizik: srednji-veliki; rad: L; profilisanje prije: clean build/deploy audit; redoslijed: 18.
19. Razmotriti bounded immutable XML cache — korist: srednja; rizik: srednji; rad: L; profilisanje prije: da; redoslijed: 19.
20. Razmotriti DevExpress Server/Virtual Mode samo za dokazano velike gridove — korist: velika; rizik: veliki; rad: L; profilisanje prije: da; redoslijed: 20.

---

# Dodatak A — konkretan baseline plan

## Metrike, mjesto i alat

- **`Starter.Run` cold/warm startup:** `Stopwatch` od `Starter.Run` ulaza do `form.Start`/first shown; postojeći `InfoTest` markeri prošireni correlation ID-om. Scenario: isti TestShell XML, svježi proces (cold) i drugi run (warm).
- **XML load/deserializacija:** oko `FileFinder`, `GetPartsSql`, `GetForms`, `GetParts`, `GetCustomControls`, `GetDatalistInfo`, `XmlSerializer.Deserialize`. Log: JSON-lines ili key=value: `ts,runId,module,phase,elapsedMs,xmlFiles,depth`.
- **Otvaranje granule i inicialni grid bind:** `Manager.TreeWithGrids_Load`, `ComponentGrid.Populate`, SQL fetch, `SetData`, first paint. Alat: internal log + Visual Studio Performance Profiler / JetBrains dotTrace na .NET Framework.
- **Grid refresh:** mjeriti fetch, `SetData`, BestFit, layout restore zasebno; scenario bez/sa groupingom i saved layoutom.
- **SQL count/trajanje po akciji:** instrumentirati `Common.ExecuteQuery`, `ExecuteScalar`, `ExecuteNonQuery`, `Core.SetData`; logirati procedure/command hash, type, timeout, elapsed, rows, correlation ID. SQL Server Extended Events ili Query Store potvrđuje duration/reads/plan.
- **UI freeze interval:** WinForms `System.Windows.Forms.Timer` heartbeat na 100 ms koji logira delta >200 ms samo dok je perf flag uključen; dopuniti ETW/PerfView ili Visual Studio UI responsiveness traceom.
- **Private bytes, managed heap, Gen0/1/2:** `Process.PrivateMemorySize64`, `GC.GetTotalMemory(false)`, `GC.CollectionCount(0..2)` na run start/end i nakon zatvaranja forme; potvrditi s PerfMon (`Process\Private Bytes`, `.NET CLR Memory`).
- **Žive forme/kontrole:** weak-reference probe registriran pri create/dispose za odabranu formu/grid; nakon close + prisilnog test GC provjeriti `IsAlive`. Ne uvoditi GC.Collect u produkciju.
- **Otvorene SQL konekcije:** SqlClient perf counters gdje su dostupni i SQL Server DMV/Extended Events; ne zaključivati samo po managed objektima.

## Usporedba

Svaka promjena koristi istu x86 verziju, istu DB snapshot/test podatke, isti XML i najmanje 10 ponavljanja cold/warm scenarija. Izvještavati medijan i P95, ne samo najbolji rezultat. Ako SQL execution plan nije pod kontrolom, rezultat označiti “Potrebno mjerenje/profilisanje”.

# Dodatak B — minimalni safety net

- **Odmah automatizabilno:** parser characterization corpus; serializer XML fixture testovi; `Common`/`SqlHelper` SQL wrapper integration tests prema test SQL Serveru; parameter conversion; Ugovori gateway testovi; static Graf determinism test.
- **Carpo.TestShell smoke:** Manager start, Standard start, grid populate/refresh, tree→master→detail, grouping/sorting/summaries, export/print, layout save/restore, modal form, error path.
- **Jupiter-only manual checklist:** plugin discovery/entry signature, SharedAssemblies deploy/rollback, x86 host load, parent window/modal behavior, DMS XML resolution, permissions, COM Outlook, fiscalization/ZKI/JIR, legacy ADODB paths.
- **Test discipline:** svaki refaktor prvo characterization test/log koji opisuje sadašnje ponašanje; tek zatim lokalna promjena. Ne testirati fiskalizaciju ili Outlook sa stvarnim produkcijskim dokumentima/kontaktima bez poslovnog odobrenja.

---

# Dodatak C — dopunski potvrđeni nalazi iz SQL/parser auditiranja

## F-16
- ID: F-16
- Naziv: Statički `OutParams` je race condition pri paralelnim SQL učitavanjima
- Kategorija: Pouzdanost / concurrency / SQL
- Prioritet: P1
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Data\Common.cs`
- Klasa/metoda: polje `OutParams:15`; overloadovi `ExecuteQuery:17–34`
- Dokaz iz koda: `private static List<...> OutParams` prosljeđuje se kao `out` iz javnih overloadova. U isto vrijeme `ComponentGrid.GetDataAsync(FocusType)` na `ComponentGrid.cs:1124` koristi `Task.Factory.StartNew`, pa dva data loada mogu prepisati istu listu.
- Problem: output parametri pripadaju pojedinačnom pozivu, ali pohranjuju se u shared mutable state.
- Mogući produkcijski efekat: pogrešna output vrijednost ili intermitentni rezultat kada su dva paralelna loada aktivna.
- Predložena promjena: ukinuti static field interno; lokalnu varijablu proslijediti u postojeći overload i vratiti je samo kroz eksplicitni `out` API gdje je potreban. Ne uvoditi lock kao trajno rješenje jer bi serijalizirao grid loade.
- Rizik promjene: mali-srednji; prvo pretražiti call-siteove koji čitaju `OutParams`.
- Procjena rada: S
- Način testiranja: paralelni TestShell/integration test s dvije procedure koje vraćaju različite output parametre; ponoviti stotine puta.
- Mjerljivi kriterijum uspjeha: output parametar je uvijek vezan uz ispravan correlation ID/proceduru.
- Zavisnosti od drugih promjena: F-03; ne zahtijeva host promjenu.

## F-17
- ID: F-17
- Naziv: `LoadingFormData` zadržava parent `DataSet` graph preko DataTable reference
- Kategorija: Memorija
- Prioritet: P1
- Pouzdanost: potvrđeno za ownership; potrebno mjerenje/profilisanje za veličinu
- Projekat i fajl: `Carpo.UI\Logic\Data\Core.cs`
- Klasa/metoda: `LoadingFormData:289–314`
- Dokaz iz koda: `CurrentForm.DataList.Add(data.Name, ds.Tables[0])`; `DataTable.DataSet` referenca zadržava cijeli `DataSet`, uključujući ostale result setove i metadata.
- Problem: kod nominalno sprema tablicu, ali lifetime tablice produžuje lifetime kompletnog dataset graph-a.
- Mogući produkcijski efekat: više privatne memorije i Gen2 pritiska kod formi s mnogo reader akcija/multiple result setova, posebno u x86 procesu.
- Predložena promjena: ne kopirati tablice globalno. Nakon mjerenja, za DataList putanju koja treba samo jednu tablicu vratiti single `DataTable` iz data layera ili eksplicitno `Copy`/odvojiti samo kada se dokaže manji retained size. Za multi-result XML akcije zadržati postojeći `DataSet`.
- Rizik promjene: srednji — `DataList` i XML actioni možda očekuju tablice po indeksu.
- Procjena rada: M po ciljanom data actionu.
- Način testiranja: profiler retained-size snapshot nakon otvaranja/zatvaranja granule s multi-result procedurom; schema/DBNull/regresija.
- Mjerljivi kriterijum uspjeha: smanjen retained private/managed heap bez promjene dostupnih DataList vrijednosti.
- Zavisnosti od drugih promjena: F-03, stvarne veličine result setova.

## F-18
- ID: F-18
- Naziv: `SET ARITHABORT ON` uvodi dodatni SQL round-trip po pozivu
- Kategorija: SQL latency
- Prioritet: P2
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Data\Common.cs`
- Klasa/metoda: `ExecuteQuery:51–54`, `ExecuteAzur:166–169`
- Dokaz iz koda: svaki novo-otvoreni `SqlConnection` izvršava zasebni `SqlCommand("SET ARITHABORT ON")` prije glavnog command-a.
- Problem: jedan dodatni server round-trip po query/scalar/nonquery pozivu; budući da ADO.NET pooling vraća sesije, potrebna je provjera je li session-option potreban na svakom checkoutu i kakav je plan-cache razlog.
- Mogući produkcijski efekat: povećana latency kod mnoštva malih upita; moguć plan mismatch ako se ukloni bez provjere.
- Predložena promjena: prvo Extended Events izmjeriti broj i trajanje tog command-a te usporediti planove s Jupiter/VB6 sesijama. Ako je potreban, preferirati connection-string/session politiku koju DBA potvrdi; ne uklanjati “na slijepo”.
- Rizik promjene: veliki za SQL planove usprkos malom kodskom zahvatu.
- Procjena rada: S za mjerenje, M za promjenu.
- Način testiranja: Query Store/actual plan i regression test reprezentativnih SP-ova s i bez opcije.
- Mjerljivi kriterijum uspjeha: nema regresije planova/rezultata, a broj round-tripova po akciji je smanjen ako se promjena odobri.
- Zavisnosti od drugih promjena: DBA i SQL execution planovi.

## F-19
- ID: F-19
- Naziv: `SqlHelper` parametar-discovery SP putanja koristi dvije konekcije
- Kategorija: SQL latency / build-up overhead
- Prioritet: P2
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.Data\SqlHelper.cs`
- Klasa/metoda: `ExecuteDataset(string, string, params object[]):507–522`; `SqlHelperParameterCache.DiscoverSpParameterSet:2321–2358`
- Dokaz iz koda: `GetSpParameterSet(connectionString, spName)` otvara konekciju za `SqlCommandBuilder.DeriveParameters`, a zatim `ExecuteDataset(connectionString, ...)` otvara novu konekciju za izvršenje. Cache hit i dalje klonira cijeli `SqlParameter[]`.
- Problem: legacy convenience overload ima dodatni connection/setup trošak i clone alokacije.
- Mogući produkcijski efekat: sporiji first-call i manje izražen overhead na warm calls; dominantnost treba profilisati jer primarni UI tok koristi `Common`, ne `SqlHelper`.
- Predložena promjena: ne optimizirati prije call-site inventara. Za frekventni potvrđeni poziv preći na explicit typed parameters/command na postojećoj konekciji; ne dijeliti mutable `SqlParameter` instance između poziva.
- Rizik promjene: srednji — output/direction/size parametara.
- Procjena rada: M.
- Način testiranja: cold/warm SP benchmark, output parameter i `SqlDbType` provjere.
- Mjerljivi kriterijum uspjeha: jedna konekcija po izvršenju gdje je to dopušteno; identični parametri i rezultat.
- Zavisnosti od drugih promjena: F-03, call-site inventar.

## F-20
- ID: F-20
- Naziv: Dvostruka JRoot deserializacija i pogrešan parser control-name strip
- Kategorija: Startup CPU / pouzdanost
- Prioritet: P2
- Pouzdanost: potvrđeno
- Projekat i fajl: `Carpo.UI\Logic\Serializer.cs`, `Carpo.UI\Logic\Parser.cs`
- Klasa/metoda: `Serializer.GetPartsSql:200–203` i `DeserializeXml:101–104`; `Parser.GetDirectByPart:394–396`
- Dokaz iz koda: `GetPartsSql` deserializira `JRoot` prije mergea dijelova, nakon čega `DeserializeXml` deserializira cijeli root još jednom. U `GetDirectByPart` za `#control` vraća `str.Replace("@", "")`, pa `#` ostaje.
- Problem: prva deserializacija je startup overhead koji je opravdan samo ako je stvarno potreban za `partsql`; prefix greška može proizvesti neispravno razrješenje control imena.
- Mogući produkcijski efekat: dodatne startup alokacije; parser bug u call-siteovima koji koriste `DirectType.ControlName`.
- Predložena promjena: za parser napraviti izolirani bugfix characterization test i promijeniti u `Replace("#", "")`. Za serializer prvo izmjeriti koliko granula sadrži `partsql`; ako je potrebno samo mapiranje data definicije, razmotriti selektivno čitanje bez pune deserializacije.
- Rizik promjene: mali za parser uz test; srednji za serializer.
- Procjena rada: S za parser, M–L za serializer.
- Način testiranja: corpus parser tokena te XML fixture s/bez `partsql`, nested parts i database overrideom.
- Mjerljivi kriterijum uspjeha: `#control` daje naziv bez prefixa; serializer daje identičan JRoot uz manje deserializacija za ciljane granule.
- Zavisnosti od drugih promjena: F-10 instrumentacija.

## Dopuna prioritetima

Između aktivnosti 4 i 5 u konačnoj listi treba uvrstiti F-16 (`OutParams` lokalizacija) kao P1/S nakon concurrency testa. F-17 ide prije svakog općeg cache/paging rada, ali tek nakon retained-memory baselinea. F-18 i F-19 ostaju iza SQL plan/profiling prikupljanja; njihova naizgled jednostavna optimizacija ne smije promijeniti session options ili parameter semantiku bez DBA provjere.
