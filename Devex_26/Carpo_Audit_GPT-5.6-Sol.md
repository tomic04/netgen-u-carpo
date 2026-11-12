# Carpo tehnički audit i inkrementalni plan poboljšanja

- **Model:** GPT-5.6 Sol
- **Datum analize:** 31.07.2026.
- **Repository:** `c:\TFS\Core Techonologies\Carpo\Devex_23_4.8_framework`
- **Solution:** `Carpo.sln`
- **Branch/TFVC putanja:** `$/Carpo/Devex_23_4.8_framework`, branched from `$/Carpo/Jupiter10`; potvrđeno u lokalnom `_changeset_details.txt`. TFS server je `http://spin-devops:8080/tfs/core%20technologies`.
- **Analizirani changeset:** najnoviji changeset zabilježen u lokalnom exportu je **5027** od 13.02.2026. (`_changeset_details.txt:3342-3350`). `tf` CLI nije dostupan, pa nije moguće potvrditi da working copy nema novije lokalne/pending promjene. Git commit ne postoji.
- **Obuhvat:** model je imao pristup cijelom lokalnom repositoryju i izvršio read-only pregled solution/csproj konfiguracije, prioritetnih klasa i repository-wide obrazaca. Nije mijenjan postojeći kod.
- **Nije bilo moguće pregledati:** definicije stored procedura i njihove execution planove; produkcijske XML granule i tipične količine podataka; Jupiter host source i binding/deployment logiku; produkcijske logove, dumpove i profile; live TFVC workspace/history iznad lokalnog exporta; stvarno x86 runtime ponašanje COM/DMS/Jupiter integracija.
- **Važna ograda:** nisu izvršeni produkcijski benchmark, SQL trace ni memory profil. Sve brojčane ciljeve ispod treba tretirati kao acceptance kriterije koje tim treba potvrditi baseline mjerenjem, ne kao tvrdnju o trenutnom stanju.

## 1. EXECUTIVE SUMMARY

1. Build nije reproducibilan: većina internih Carpo zavisnosti su reference na prebuilt DLL-ove verzije `10.0.0.24` iz Jupiter `SharedAssemblies`, dok source assemblyji imaju verziju `10.0.8.36`; zato build može uspjeti sa starim binarnim ugovorom i ne mora sadržavati promjene iz solutiona.
2. `Debug|AnyCPU`, `Release|AnyCPU` i dio `Debug|x86` konfiguracija direktno pišu u `Program Files (x86)`; to miješa build i deployment, traži privilegije i omogućava da zaključani ili stari DLL-ovi utiču na rezultat.
3. `Starter.RegisterForm` zapisuje plaintext password u log, a `ComDll.SpinClient` ga stavlja u command-line argument; ovo je potvrđen sigurnosni i operativni rizik, nezavisan od performance profila.
4. Glavni grid “async” put nije stvarno asinhron: argument za `Task.FromResult(...)` prvo sinhrono izvrši SQL na UI threadu, a await završenog taska ne yielda. Zato se cijeli aktivni `GetDataAsync` zapravo završi prije nego `Populate` nastavi; problem je UI blokiranje i obmanjujući API, ne potvrđen race u tom aktivnom putu.
5. `ComponentGrid.Populate` prije svakog dohvata postavlja `DataSource = null`, a `SetData` ponovo radi binding, template/layout obradu i potencijalno višestruki `BestFitColumns`; na velikim tabelama to je vjerovatan izvor UI freezea, ali stvarni udio SQL/binding/layout vremena zahtijeva profilisanje.
6. `Events.RunActions` sinhrono izvršava SQL, Outlook, fiskalizaciju, SOAP i `Thread.Sleep` iz event pipelinea; `Task.Run(...).GetAwaiter().GetResult()` ne oslobađa UI thread. Tree populate je takođe potpuno sinhroni SQL+binding put.
7. `foreachrow` prolazi svaki grid red i za svaki kreira novi `Events` pipeline; ako nested akcije sadrže `datarun`, nastaje potvrđena mogućnost N SQL poziva za N redova. Koje granule to koriste zahtijeva runtime/XML inventar.
8. `Manager_FormClosing` radi po jedan `spCarpoFilterPostavke` poziv za svaki sačuvani node, zatim dodatne layout/value SQL pozive. Zatvaranje forme zato može blokirati UI i držati korisnika u neodzivom stanju.
9. `Carpo.Data.Common` za svaki poziv otvara konekciju, radi dodatni `SET ARITHABORT ON`, puni cijeli `DataSet` i parametre uglavnom tipizira prema CLR vrijednosti bez eksplicitnog SQL tipa/dužine; to povećava round-trip i plan-cache rizik. Stored procedure planovi nisu dostupni.
10. Novi `SqlContext.CreateCommand` vraća `SqlCommand` deklarisan sa `using var`, pa je command disposed pri izlazu iz factory metode; isti sloj koristi `AddWithValue` i nema eksplicitne timeoute. To je potvrđen ownership/API defekt i treba ga karakterizovati prije šire upotrebe.
11. XML startup višestruko skenira isti `XDocument`, rekurzivno proširuje `part`/`form`, može pozivati SQL za `partsql`, te zatim ponovo deserializira cijeli graph. Cache može pomoći samo uz ključ koji uključuje DB/module/path/version i jasnu invalidaciju.
12. `SpecificElementList.DatalistModelList` je statička lista kojoj svaka deserializacija dodaje novi model bez uklanjanja; F5/retry i dugotrajna Jupiter sesija mogu zadržavati XML stringove i objektne grafove.
13. Outlook i generički COM putevi ne oslobađaju RCW objekte; Outlook aplikacija/folder/items/contact i `dynamic comObject` mogu ostati živi do nedeterminističkog GC-a.
14. `Events`, `ComponentGrid`, `Manager`, `Serializer` i `SqlHelper` kombinuju dispatch, UI, I/O, resource ownership i error policy. Izdvajanje treba početi uskim adapterima oko mjerenja, SQL izvršavanja i vanjskih integracija, bez promjene `Starter.Run`/`IJupiterStart`.
15. `Carpo.Specific\UgovoriONajmu` gradi SQL tekst konkatenacijom `searchString`/`varIme` i šalje ga kao `CommandType.Text`; to je potvrđen injection/plan-cache rizik i mora se parametrizovati prije performance mikrooptimizacija.

## 2. MAPA SISTEMA

### 2.1 Projekti i uloge

- **Carpo.Structure** — XML modeli, interfejsi i ugovori (`JRoot`, `JForm`, `JData`, kontrole, eventi, `IJupiterStart`).
- **Carpo.Common** — helperi, translator, grid/layout pomoćne klase i zajednički modeli.
- **Carpo.Data** — ADO.NET pristup, legacy `SqlHelper`, aktivni `Common` wrapper, noviji `SqlContext`/service sloj i specifični Get/Set wrapperi.
- **Carpo.Core** — globalne varijable, logging, ini i print infrastruktura.
- **Carpo.UI** — XML parser/serializer, event/action engine, forme, DevExpress komponente i Jupiter connector.
- **Carpo.Gen** (assembly `Carpo`) — glavni plugin entry point i izbor konkretne forme.
- **Carpo.Specific** — klijentski/modulski WinForms i data kod, uključujući UgovoreONajmu, Virmani, Matpos i COM/serial integracije.
- **Carpo.XmlGen** — XML generisanje i pomoćni parser/data tok.
- **Carpo.Designer**, **Carpo.CMS** — alati za dizajn/održavanje XML definicija.
- **Carpo.Lite** — lakši host/launcher koji referencira prebuilt `Carpo.dll` i `Carpo.UI.dll`.
- **Carpo.TestShell** — primarni ručni integration/smoke harness.

### 2.2 Efektivne zavisnosti i build rizik

Solution deklarira project dependencies, ali one nisu isto što i compile-time `ProjectReference`. Stvarne csproj reference pokazuju:

- `Carpo.UI` ima `ProjectReference` samo na `Carpo.Data`; `Carpo.Common`, `Carpo.Core` i `Carpo.Structure` uzima kao DLL iz Jupiter direktorija.
- `Carpo.Data` uzima `Carpo.Common` kao prebuilt DLL.
- `Carpo.Core` uzima `Carpo.Common`, `Carpo.Data`, `Carpo.Structure` kao prebuilt DLL-ove.
- `Carpo.Gen`, `Carpo.XmlGen`, `Carpo.Designer`, `Carpo.TestShell`, `Carpo.Specific`, `Carpo.CMS` i `Carpo.Lite` dominantno koriste prebuilt Carpo DLL reference.
- Include verzije internih referenci su `10.0.0.24`, dok svih 12 `AssemblyInfo.cs` fajlova trenutno navode `10.0.8.36`.
- `Carpo.UI.csproj` ima developer-specific HintPath putanje `C:\Users\valentino.jagodic\Desktop\...` za dva DevExpress DLL-a.
- DevExpress reference su uglavnom `23.1.5.0`; ADODB, Outlook interop, DMS, Ribbon i Spin.Printing ostaju vanjski deployment preduvjeti.
- Solution sadrži AnyCPU, Mixed Platforms, x86 i x64 mapiranja; `Debug|x86` za više biblioteka efektivno koristi `AnyCPU`. Pure-MSIL AnyCPU biblioteka se normalno izvršava kao x86 unutar x86 hosta, pa sama ta činjenica ne dokazuje `BadImageFormatException`, ali konfiguracija ne garantuje da su svi interop/native dependencyji validirani kao produkcijski x86 set.
- `Carpo.Data` čak i u `Release|x86` piše direktno u `C:\Program Files (x86)\Spin Informatica\...`, dok solution mapira `Carpo.XmlGen Release|x86` na `Release|Any CPU` (`Carpo.sln:284-285`).

Posljedica: solution build order može napraviti source DLL, ali potrošač i dalje kompilirati protiv stare kopije iz `SharedAssemblies`. To je potvrđeno konfiguracijom; da li se dešava u konkretnom developer/TFS buildu treba dokazati binlogom.

### 2.3 Entry pointovi

- `Carpo.Structure\Interfaces\IJupiterStart.cs:3-6` postoji kao javni interfejs, ali repository-wide pretraga nije pronašla nijednu implementaciju. Njegov `Run` prima `menuID`, dok `Starter.Run` nema taj parametar i koristi `Starter.MenuID` property. Zato se ne smije tvrditi da trenutni glavni plugin implementira `IJupiterStart`; stvarni Jupiter binding je vjerovatno reflection/property ugovor i treba ga potvrditi u hostu.
- Glavni generički plugin tip je `Carpo.Gen\Start.cs:9-44`, `Carpo.Gen.Start : Starter`.
- Osnovni runtime ulaz je `Carpo.UI\Connector\Abstract\Starter.cs:78-139`.
- `Carpo.Gen.Start.RegisterForm` bira Manager/Splitter/Info/SideBar/Standard i zatim poziva `Starter.RegisterForm(form)`.
- `Carpo.XmlGen.Start` i pojedini `Carpo.Specific.*.Start` tipovi su dodatni specifični ulazi, ali nisu zamjena za glavni Carpo plugin tok.

### 2.4 Reprezentativni runtime tokovi

**Tok A — startup do prve forme**

`Carpo.Gen.Start` → `Starter.Run` → postavljanje globalnog connection/session stanja → `Common.Translator.Init` → `Serializer.DeserializeXml` → DMS/file dohvat root XML-a → `GetPartsSql`/`GetForms`/`GetParts`/`GetCustomControls` → `XmlSerializer.Deserialize(JRoot)` → `SetInitForm` → `Carpo.Gen.Start.RegisterForm` → konstruktor konkretne forme → permissions/layout/control init → `form.Start` → prikaz.

Dokaz: `Starter.cs:78-139,162-213,265-371`, `Serializer.cs:54-125`, `Carpo.Gen\Start.cs:15-44`.

**Tok B — grid populate do SQL-a i nazad**

XML/UI notification → `JGridControl.Populate` → `ComponentGrid.Populate` → `GetDataAsync` → `DataDefinition.GetDataSet` → `Data.Core.GetDataSet/SetData` → `Carpo.Data.Common.ExecuteQuery` → `SqlDataAdapter.Fill(DataSet)` → `ComponentGrid.SetData` → `DataSource = ds.Tables[0]` → init/template/layout/best-fit/focus/event obrada.

Dokaz: `JGridControl.cs:103-106`, `ComponentGrid.cs:878-969,1241-1345`, `DataDefinition.cs:42-69`, `Data\Core.cs:96-170`, `Carpo.Data\Common.cs:36-114`.

**Tok C — XML event/action do transakcije**

WinForms/DevExpress event → `Events.RunNotification` → `RunActions` → `CheckData` ili `CheckTransaction` → jedna konekcija/transakcija → rekurzivni `RunActions` → `NotifyData` → `Data.Core` → `ExecuteQueryInTran`/`ExecuteAzurInTran` → commit/rollback → UI varijable/kontrole.

Dokaz: `Events.cs:136-170,270-479,1438-1529,1825-1893`, `Data\Core.cs:141-170`, `Data\Common.cs:205-279`.

**Tok D — fiskalizacija/JIR**

Event action → SQL dohvat certifikata → XML data load → certifikat i XML potpis → SOAP poziv → response XML → SQL update JIR/ZKI/UUID. Cijeli tok je u `RunActions`, a SOAP se radi `Task.Run(...).GetAwaiter().GetResult()`.

Dokaz: `Events.cs:297-409`.

**Tok E — Manager zatvaranje**

`FormClosing` → snapshot layouta → petlja kroz `settings` → po-node `spCarpoFilterPostavke` → dispose global custom kontrola → više layout `SaveXml/SaveValue` i eventualni `SetAppControlValues`.

Dokaz: `Manager.cs:1220-1333`, `SettingsService.cs:27-36`.

## 3. TOP NALAZI

### F-01

- **ID:** F-01
- **Naziv:** Build se kompilira protiv starih internih DLL-ova
- **Kategorija:** build, pouzdanost, razvoj
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** svi csproj; posebno `Carpo.UI\Carpo.UI.csproj:87-97,1026`, `Carpo.Gen\Carpo.csproj:88-106`, svi `Properties\AssemblyInfo.cs:34-35`
- **Klasa/metoda:** MSBuild reference resolution
- **Dokaz iz koda:** interne reference traže `10.0.0.24` u Jupiter `SharedAssemblies`, source assemblyji su `10.0.8.36`; samo UI→Data je `ProjectReference`.
- **Problem:** solution source nije jedini build input. Promjena u Common/Core/Structure/UI može biti ignorisana ili se potrošač može povezati sa ABI-em druge verzije.
- **Mogući produkcijski efekat:** runtime `MissingMethodException`, nekonzistentni DLL setovi, nereproducibilan bug i spor development loop.
- **Predložena promjena:** uvesti zasebnu `LocalBuild|x86`/CI konfiguraciju sa ProjectReference između source projekata i centralnim lokalnim outputom; zadržati postojeći deployment profil dok se binarna kompatibilnost ne potvrdi. Vanjske Jupiter/DevExpress DLL-ove usmjeriti kroz dokumentovan `ReferenceRoot`.
- **Rizik promjene:** srednji; source dependency graph može otkriti postojeće cikluse i skrivene verzijske zavisnosti.
- **Procjena rada:** M
- **Način testiranja:** MSBuild binary log, pregled `ResolveAssemblyReference`, clean x86 build na mašini bez lokalnih SharedAssemblies kopija, TestShell smoke, zatim Jupiter smoke.
- **Mjerljivi kriterijum uspjeha:** svaki interni Carpo compile input dolazi iz istog builda; clean build radi bez developer Desktop putanja; deployment paket ima jednu konzistentnu verziju.
- **Zavisnosti od drugih promjena:** baseline build manifest i dogovorena CI/deployment konfiguracija.

### F-02

- **ID:** F-02
- **Naziv:** Credentiali se zapisuju u log i process command line
- **Kategorija:** sigurnost, pouzdanost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Connector\Abstract\Starter.cs:289-299`; `Carpo.UI\Logic\ComDll.cs:31-48`; `Carpo.Data\Log.cs:78-97`; `Carpo.UI\Logic\Data\Core.cs:150,161,170,264-276`; `Carpo.Data\SqlHelper.cs:2535-2542`
- **Klasa/metoda:** `Starter.RegisterForm`, `ComDll.SpinClient`, `Log.CommandAsSql`, `Data.Core.LogData`, legacy app-role setup
- **Dokaz iz koda:** log poruka konkatenira `password`; SpinClient argument string sadrži `Connection.Pwd`; `CommandAsSql` gradi izvršivi SQL sa literalnim vrijednostima parametara koji se šalje u `InfoData`; legacy `sp_setapprole` put sadrži hardkodirani string `"Passw0rd!"`.
- **Problem:** password ostaje u log fajlu i vidljiv je alatima koji čitaju command line procesa. SQL logging može sadržati lične, finansijske i druge osjetljive vrijednosti.
- **Mogući produkcijski efekat:** kompromitacija DB credentiala i širenje tajni u backup/support artefakte.
- **Predložena promjena:** odmah redaktovati password, connection string i literalne SQL vrijednosti; potvrditi koristi li se legacy app-role put i rotirati/izmjestiti tajnu ako je aktivan; za SpinClient zadržati kompatibilni legacy put samo iza eksplicitnog feature flaga dok se ne potvrdi sigurniji host ugovor. Ne mijenjati argument protokol bez Jupiter/VB6 testa.
- **Rizik promjene:** mali za log redakciju; veliki za command-line ugovor.
- **Procjena rada:** S za log, M/L za interprocess ugovor.
- **Način testiranja:** TestShell/Jupiter pokretanje i pregled loga/Process Explorer command line-a sa test credentialom.
- **Mjerljivi kriterijum uspjeha:** plaintext test password se ne pojavljuje u novim Carpo logovima; SpinClient roadmap ima kompatibilan zaštićeni transport ili formalno prihvaćen legacy rizik.
- **Zavisnosti od drugih promjena:** potvrda vlasnika SpinClient protokola.

### F-03

- **ID:** F-03
- **Naziv:** Grid “async” put je sinhron i blokira UI
- **Kategorija:** UI odziv, runtime
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs:878-969,1115-1139,1241-1262`; `ComponentTree.cs:511-639`
- **Klasa/metoda:** `ComponentGrid.Populate`, oba `GetDataAsync`, `ComponentTree.PopulateTreeList`
- **Dokaz iz koda:** aktivni grid put poziva `_ = GetDataAsync()`; taj overload radi `await Task.FromResult(dataDefinition.GetDataSet(...))`. C# prvo evaluira `GetDataSet`, zatim awaita već završeni task, pa se metoda u ovom putu završi sinhrono prije sljedeće linije u `Populate`. Drugi overload sa `Task.Factory.StartNew` postoji, ali aktivni `Populate` ga ne koristi. Tree direktno poziva `GetDataSet` i veže `Tables[0]` na UI threadu.
- **Problem:** UI ostaje blokiran tokom SQL-a, a naziv/signatura stvaraju pogrešan utisak da caller može nastaviti dok I/O traje. Parametar `async` u `Populate` ne bira stvarno asinhroni put.
- **Mogući produkcijski efekat:** UI freeze pri sporom SQL-u; nema potvrđenog sequencing racea u trenutnom aktivnom `Task.FromResult` putu.
- **Predložena promjena:** prvo instrumentisati i serializovati refresh po gridu. Zatim napraviti jedan interni `Task PopulateAsync` uz snapshot parametara na UI threadu, SQL rad van UI threada na vlastitoj konekciji, te binding isključivo nakon povratka na WinForms synchronization context. Javni `Populate` ostaje kompatibilan wrapper.
- **Rizik promjene:** srednji/veliki; parser i forme nisu thread-safe, a Jupiter globalno stanje se ne smije čitati u backgroundu bez snapshota.
- **Procjena rada:** M
- **Način testiranja:** UI heartbeat, rapid double refresh, zatvaranje forme tokom queryja, x86 TestShell, exception/cancellation scenariji.
- **Mjerljivi kriterijum uspjeha:** UI heartbeat gap tokom SQL waita je ispod dogovorenog praga; nakon eventualnog uvođenja stvarnog async puta nema preklopljenih queryja i rezultat zadnjeg refresh zahtjeva je prikazan.
- **Zavisnosti od drugih promjena:** F-06 instrumentacija i characterization testovi.

### F-04

- **ID:** F-04
- **Naziv:** Potpuni grid rebind i ponovljeni layout/best-fit rad na svakom refreshu
- **Kategorija:** UI odziv, CPU, memorija
- **Prioritet:** P1
- **Pouzdanost:** visoka vjerovatnoća
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs:878-969,1264-1345,1508-1570`
- **Klasa/metoda:** `Populate`, `SetData`, `BestFitColumnsInit`, `BestFitColumns`
- **Dokaz iz koda:** `DataSource` se prvo nulira; novi `DataTable` se veže; slijede field init, template, layout restore i do dva best-fit poziva kroz `BestFitColumnsInit` i kasniju granu. Glavni rebind nije obuhvaćen `BeginUpdate/EndUpdate`.
- **Problem:** SQL, DataSet alokacija, DevExpress binding, summary/grouping i layout recalculation se plaćaju zajedno. `BestFitMaxRowCount=50` ograničava skeniranje, ali ne uklanja ponavljanje.
- **Mogući produkcijski efekat:** flicker, CPU spike, LOH/GC pritisak i spor refresh velikih gridova.
- **Predložena promjena:** mjerenjem razdvojiti query/fill/bind/layout/best-fit; dodati `BeginUpdate/EndUpdate` u `finally`; ukloniti dokazano dupli best-fit; za isti schema/layout koristiti in-place refresh samo ako semantics selekcije/groupinga ostanu identične.
- **Rizik promjene:** srednji; DevExpress state restoration i master/detail event redoslijed su osjetljivi.
- **Procjena rada:** M
- **Način testiranja:** grid matrica: 0/1/1k/10k redova, grouping, summary, filter, selection, master/detail, saved layout, export.
- **Mjerljivi kriterijum uspjeha:** manje layout/best-fit invokacija po refreshu i niži bind+layout p50/p95 bez funkcionalne regresije.
- **Zavisnosti od drugih promjena:** F-03 i baseline.

### F-05

- **ID:** F-05
- **Naziv:** `foreachrow` može generisati N+1 SQL pozive
- **Kategorija:** SQL, UI odziv
- **Prioritet:** P1
- **Pouzdanost:** visoka vjerovatnoća
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs:2505-2546,2737-2744`; `Carpo.UI\Logic\Events.cs:1685-1687,1964-1972`
- **Klasa/metoda:** `SetNotifyForEachRow`, `RowForEachRowAction`, `NotifyGridControlForEachRow`
- **Dokaz iz koda:** svaki red poziva novi `Events.RunActions(action.Actions)`; nested akcija može biti `datarun` i pozvati bazu.
- **Problem:** broj SQL poziva raste linearno sa redovima, na UI threadu, uz ponovljeno parsiranje i object creation.
- **Mogući produkcijski efekat:** višeminutne batch akcije, lockovi i djelimično izvršenje bez jasne granice.
- **Predložena promjena:** inventarisati XML `foreachrow` akcije; za potvrđene SQL slučajeve uvesti TVP/batch stored procedure ili jednu transakcijsku komandu sa jasno tipiziranim skupom ID-eva. Ne mijenjati opštu foreach semantiku.
- **Rizik promjene:** srednji/veliki; stored procedure i error semantics moraju ostati kompatibilni.
- **Procjena rada:** M/L po granuli.
- **Način testiranja:** korelacijski SQL counter za 1/10/100 redova; rollback i djelimična greška.
- **Mjerljivi kriterijum uspjeha:** odabrana batch akcija ima O(1) SQL round-tripova umjesto O(n), uz identičan rezultat.
- **Zavisnosti od drugih promjena:** SQL trace i pristup stored procedurama.

### F-06

- **ID:** F-06
- **Naziv:** Nema korelacijskog performance baselinea
- **Kategorija:** observability, pouzdanost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** postoje parcijalni `InfoTest/InfoData` pozivi u `Starter.cs`, `Serializer.cs`, `ComponentGrid.cs`, `Manager.cs`, `Data\Core.cs`
- **Klasa/metoda:** cross-cutting
- **Dokaz iz koda:** vrijeme se često mjeri `DateTime.Now`, opisi nisu uniformni, nema action correlation ID-a, row/table/bytes counta ni agregata SQL broja/trajanja po korisničkoj akciji.
- **Problem:** nije moguće dokazati da je bottleneck SQL, XML, binding ili layout niti sigurno porediti prije/poslije.
- **Mogući produkcijski efekat:** optimizacija pogrešnog sloja i neotkrivena regresija.
- **Predložena promjena:** mali `PerfScope`/Stopwatch adapter i correlation context, feature-flagged sampling, bez promjene javnih ugovora.
- **Rizik promjene:** mali; paziti na log volumen i PII.
- **Procjena rada:** S/M
- **Način testiranja:** TestShell scenario mora proizvesti kompletan startup/action trace.
- **Mjerljivi kriterijum uspjeha:** svaka reprezentativna akcija ima korelirani ukupni elapsed, SQL count/elapsed, row count i UI phase trajanja.
- **Zavisnosti od drugih promjena:** nema.

### F-07

- **ID:** F-07
- **Naziv:** Manager radi sinhroni SQL/disk/layout rad pri otvaranju i zatvaranju
- **Kategorija:** UI odziv, SQL, I/O
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Forms\Manager\Manager.cs:245-326,365-423,1220-1333`; `Carpo.Data\Data\Service\SettingsService.cs:18-45`
- **Klasa/metoda:** konstruktor, `SaveSettings`, `Manager_FormClosing`
- **Dokaz iz koda:** konstruktor sinhrono učitava settings; svaka promjena kolone serializira layout u novi `MemoryStream`; close radi po jedan SQL save po dictionary itemu i dodatne layout/value save pozive.
- **Problem:** chatty persistence i težak FormClosing; event handleri dodani na Load nisu eksplicitno skinuti, iako child→parent reference same po sebi ne dokazuju leak.
- **Mogući produkcijski efekat:** spor prikaz/zatvaranje, previše SQL poziva i alokacija pri drag/resize kolone.
- **Predložena promjena:** dirty flag/debounce za layout snapshot; jedan batch save u SP ako je moguće; na close samo dirty nodeovi; instrumentisati i eksplicitno odjaviti dinamički dodane handlere radi jasnog lifecyclea.
- **Rizik promjene:** mali/srednji; crash prije close može izgubiti posljednju nepospremljenu promjenu, pa debounce mora imati flush.
- **Procjena rada:** S za dirty/debounce; M za batch SP.
- **Način testiranja:** 100 resize događaja, close sa 0/10/100 settings nodeova, forced failure, ponovno otvaranje i restore.
- **Mjerljivi kriterijum uspjeha:** broj SQL save poziva prati broj dirty batch-eva, ne broj svih nodeova; close p95 se mjeri i smanjuje.
- **Zavisnosti od drugih promjena:** F-06; batch zahtijeva SP promjenu.

### F-08

- **ID:** F-08
- **Naziv:** Data wrapper radi dodatni round-trip, puni DataSet i implicitno tipizira parametre
- **Kategorija:** SQL, memorija
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno za C# call-site; SQL uticaj zahtijeva plan
- **Projekat i fajl:** `Carpo.Data\Common.cs:36-133,153-203,205-279`; `Carpo.UI\Logic\Data\Core.cs:141-170`
- **Klasa/metoda:** `ExecuteQuery`, `ExecuteAzur`, `ToConvertSqlParams`
- **Dokaz iz koda:** svaka operacija radi `SET ARITHABORT ON`; reader put uvijek puni `DataSet`; `SqlParameter.Value` se postavlja bez eksplicitnog tipa/size; gotovo svi pozivi su sinhroni.
- **Problem:** najmanje jedan dodatni command po konekciji; DataSet overhead i implicitni `nvarchar`/size mogu izazvati konverzije ili plan fragmentation.
- **Mogući produkcijski efekat:** veća latency, managed heap i lošiji index usage.
- **Predložena promjena:** prvo dokazati potrebu za ARITHABORT i eventualno ga postaviti u connection string/session policy; dodati eksplicitnu metadata podršku u XML modelu za kritične parametre; u uskim hot pathovima koji vraćaju jednu tabelu ponuditi DataTable/reader put bez promjene postojećeg API-ja.
- **Rizik promjene:** srednji; ARITHABORT može promijeniti planove, a tipovi moraju odgovarati SP potpisima.
- **Procjena rada:** M
- **Način testiranja:** Extended Events/Profiler, actual plan, `STATISTICS IO,TIME`, plan cache; C# allocation profile.
- **Mjerljivi kriterijum uspjeha:** manje round-tripova/alokacija i stabilni planovi bez promjene rezultata.
- **Zavisnosti od drugih promjena:** DBA i SP metadata.

### F-09

- **ID:** F-09
- **Naziv:** Transakcija može držati lockove dok se izvršavaju UI/vanjske akcije i može izgubiti originalnu grešku
- **Kategorija:** SQL, pouzdanost
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Events.cs:1465-1529`
- **Klasa/metoda:** `CheckTransaction`
- **Dokaz iz koda:** sve child akcije se dodaju u listu i šalju u rekurzivni `RunActions`; otvorena transakcija traje kroz cijeli pipeline. `catch` bez parametra poziva `transaction.Rollback()` bez null/state provjere i ne rethrowa originalnu grešku.
- **Problem:** transakcija nije ograničena samo na data akcije; rollback može baciti sekundarnu grešku, a caller ne dobija pouzdan failure signal.
- **Mogući produkcijski efekat:** duži lockovi/deadlock rizik, tiho neizvršene akcije i nedijagnostikovan rollback.
- **Predložena promjena:** characterization testovi za trenutnu semantics; validirati da transaction blok sadrži samo DB/condition/variable akcije ili jasno podijeliti pre/DB/post fazu; rollback u zaštićenom bloku, log original+rollback exception, eksplicitno postaviti failure state.
- **Rizik promjene:** veliki ako postoje XML granule koje namjerno kombinuju UI i DB unutar transaction bloka.
- **Procjena rada:** M
- **Način testiranja:** child 2 baca SQL grešku; connection-open failure; commit failure; nested UI action; provjera DB stanja i `LastTransactionSuccess`.
- **Mjerljivi kriterijum uspjeha:** originalni uzrok je sačuvan; nema transakcije otvorene tokom nedozvoljene vanjske/UI akcije; rollback je determinističan.
- **Zavisnosti od drugih promjena:** XML inventar i safety net.

### F-10

- **ID:** F-10
- **Naziv:** XML startup ponavlja skeniranje/deserializaciju i cache nema definisanu invalidaciju
- **Kategorija:** startup, CPU, I/O, memorija
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno za ponavljanje; korist cachea je hipoteza
- **Projekat i fajl:** `Carpo.UI\Logic\Serializer.cs:35-52,54-125,127-188,190-292,294-349`
- **Klasa/metoda:** `DeserializeXml`, `serializerRecursion`, `GetParts`, `GetPartsSql`, `GetForms`
- **Dokaz iz koda:** više `Descendants(...).Count/ToList/Where` prolaza; rekurzija nakon zamjene elemenata; privremena deserializacija `JRoot` u `GetPartsSql` i konačna deserializacija; SQL po distinct dataname-u.
- **Problem:** CPU/I/O raste sa XML graphom; cache bez DB/module/DMS/file-version ključa može prikazati zastarjelu ili tuđu definiciju.
- **Mogući produkcijski efekat:** spor cold/warm open i veliki kratkotrajni graphovi.
- **Predložena promjena:** fazno mjeriti `root fetch`, partsql SQL, file parts/forms, expansion i final deserialize; prvo lokalno smanjiti višestruke enumeracije. Tek potom bounded cache finalnog immutable definition snapshota, ključ `(database,moduleId,canonicalPath,content/version fingerprint,assembly schema version)`, invalidacija na F5/file/DMS version change, weak/LRU limit.
- **Rizik promjene:** veliki za cache zbog mutable `JRoot` objekata koje forme mijenjaju.
- **Procjena rada:** S/M za enumeracije/mjerenje; L za siguran cache.
- **Način testiranja:** XML golden files, missing/recursive/required parts, partsql, F5 reload, dvije baze/modula u istoj Jupiter sesiji.
- **Mjerljivi kriterijum uspjeha:** manje XML phase elapsed/alokacija; cache nikad ne dijeli mutable instance između formi.
- **Zavisnosti od drugih promjena:** immutable snapshot ili deep-clone strategija.

### F-11

- **ID:** F-11
- **Naziv:** Statički datalist model raste sa svakom deserializacijom
- **Kategorija:** memorija, lifecycle
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Common\Helpers\SpecificElementList.cs:8-10`; `Carpo.UI\Logic\Serializer.cs:455-475`; `Carpo.Core\Log\Log.cs:880-889`
- **Klasa/metoda:** `DatalistModelList`, `Serializer.GetDatalistInfo`
- **Dokaz iz koda:** statička `List<List<DatalistInfo>>` dobija `Add` pri svakom deserializeu; nema remove/replace/limit.
- **Problem:** dugotrajni x86 proces zadržava XML metadata/stringove po svakom otvaranju/F5.
- **Mogući produkcijski efekat:** rast private bytes/managed heap i raniji x86 address-space pressure.
- **Predložena promjena:** zamijeniti listu bounded dictionaryjem po `(database,moduleId,path/version)` i replace semantics; očistiti na session/module unload. Prije promjene potvrditi da Log UI ne očekuje historiju svih deserializacija.
- **Rizik promjene:** mali/srednji.
- **Procjena rada:** S
- **Način testiranja:** 100 open/close/F5 ciklusa, heap snapshot i provjera F11 log pregleda.
- **Mjerljivi kriterijum uspjeha:** broj datalist modela ostaje ograničen brojem aktivnih ključeva; heap se stabilizuje nakon GC-a.
- **Zavisnosti od drugih promjena:** definicija lifecycle ključa.

### F-12

- **ID:** F-12
- **Naziv:** COM/Outlook objekti nemaju deterministički cleanup
- **Kategorija:** resursi, UI odziv, pouzdanost
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Helpers\MicrosoftAPI.cs:14-81`; `Carpo.UI\Logic\ComDll.cs:60-129`; `Carpo.UI\Logic\Events.cs:278-294,347-409`
- **Klasa/metoda:** `AddContactToOutlook`, `TotalGen`, Outlook/JIR action
- **Dokaz iz koda:** kreiraju se Outlook Application/Session/MAPIFolder/Items/ContactItem i dynamic COM objekt, bez `Marshal.FinalReleaseComObject`; pozivi su sinhroni. JIR grana kreira `X509Certificate2` bez `using`, za razliku od ZKI grane. `throw ex` resetuje stack.
- **Problem:** RCW lifecycle zavisi od GC-a; COM poziv može blokirati UI ili ostaviti Outlook/COM server živim.
- **Mogući produkcijski efekat:** zombie Outlook proces, COM resource rast, spor event.
- **Predložena promjena:** izdvojiti `IOutlookContactGateway`/`ILegacyComInvoker`; u `finally` osloboditi RCW objekte obrnutim redom i ne zvati GC; deterministički disposeati JIR certifikat. Async se uvodi samo uz provjeru STA zahtjeva i dedicated STA thread/message pumpu.
- **Rizik promjene:** srednji/veliki; Office interop ownership je osjetljiv i ne treba releaseati shared Outlook instance bez testa.
- **Procjena rada:** M
- **Način testiranja:** 50 create/update ciklusa, Outlook već otvoren/zatvoren, exception u Find/Save, Process Explorer COM handle/process provjera.
- **Mjerljivi kriterijum uspjeha:** nema monotonog rasta živih RCW/procesa; UI freeze je izmjeren i kontrolisan.
- **Zavisnosti od drugih promjena:** Outlook test okruženje.

### F-13

- **ID:** F-13
- **Naziv:** `SqlContext` vraća disposed command i koristi `AddWithValue`
- **Kategorija:** SQL, pouzdanost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Data\Data\Context\SqlContext.cs:20-99,128-141`
- **Klasa/metoda:** `CreateCommand` i svi caller overloadi
- **Dokaz iz koda:** `using var command = new SqlCommand(...)` je deklarisan u factory metodi i zatim vraćen; dispose scope završava pri izlazu iz metode. Parametri se dodaju `AddWithValue`; timeout nije postavljen.
- **Problem:** factory vraća objekt čiji ownership je već završen; ponašanje zavisi od implementacijskih detalja `SqlCommand.Dispose`. Tipovi/dužine i timeout su implicitni.
- **Mogući produkcijski efekat:** sporadične greške, nepredvidivi SQL planovi i beskonačno/neusklađeno čekanje u odnosu na XML data timeout.
- **Predložena promjena:** characterization test pa ukloniti `using` iz factoryja; caller zadržava postojeći `using var command`. Dodati eksplicitni timeout i typed parameter descriptor za ovaj uski sloj.
- **Rizik promjene:** mali za ownership, srednji za tipiziranje.
- **Procjena rada:** S
- **Način testiranja:** unit/integration test svakog overload-a, null, binary/string/decimal/date parametri, timeout.
- **Mjerljivi kriterijum uspjeha:** nema disposed-object ponašanja; svi commandi imaju dokumentovan timeout i očekivane `SqlDbType/Size`.
- **Zavisnosti od drugih promjena:** test baza/SP.

### F-14

- **ID:** F-14
- **Naziv:** Parser/evaluator alocira i ponavlja lookup u hot pathu
- **Kategorija:** CPU, arhitektura
- **Prioritet:** P2
- **Pouzdanost:** visoka vjerovatnoća
- **Projekat i fajl:** `Carpo.UI\Logic\Parser.cs:58-193,303-338,383-413`; `Evaluate.cs:38-99,182-193`; `Data\Core.cs:417-457`
- **Klasa/metoda:** `GetLogicPartValue`, `Expression`, `GetDataByName`
- **Dokaz iz koda:** česti `Split(...).ToList`, LINQ, novi Parser/Evaluate i novi Regex; `GetDataByName` radi `Where`, zatim `Count`, pa ponovnu enumeraciju; lookup metoda je linearni scan.
- **Problem:** isti XML izrazi se parsiraju mnogo puta u eventima, parametrima i po-row akcijama.
- **Mogući produkcijski efekat:** CPU/Gen0 pritisak kod velikog broja eventa/redova.
- **Predložena promjena:** prvo sample profiler. Siguran cache smije čuvati samo tokenizirani, nepromjenjivi expression plan, ne rezultat; bounded po exact stringu/parser modeu, bez referenci na Form/Control/DataTable. Indeksirati JData/custom methods po case-insensitive imenu pri form initu.
- **Rizik promjene:** srednji; parser semantics su legacy ugovor.
- **Procjena rada:** M
- **Način testiranja:** golden characterization corpus svih tokena/scopes/formata; allocation benchmark u x86 procesu.
- **Mjerljivi kriterijum uspjeha:** niži parser CPU/alloc po 10k evaluacija uz byte-for-byte/typed identične rezultate.
- **Zavisnosti od drugih promjena:** corpus i profiler dokaz.

### F-15

- **ID:** F-15
- **Naziv:** Globalno mutable stanje i background custom-control init nemaju session ownership
- **Kategorija:** arhitektura, pouzdanost, memorija
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Starter.cs:39-47`; `Carpo.Data\Connection.cs:7-20`; `Carpo.Core\PubVars.cs:7-12`; `Data\Core.cs:27`; `Jupiter\Base.cs:37`; `GlobalCustomControls.cs:7`; `CustomControlInit.cs:10-33,35-60`
- **Klasa/metoda:** `ImeFirme`, `Connection.*`, `PubVars.*`, static connection/control liste, `StartInit`, `DisposeCustomControls`
- **Dokaz iz koda:** connection string, ADO string, DB/server/user/password i operator/menu state su `[ThreadStatic]`; postoje i static liste/stanja. `StartInit` pokreće nečekani `Task` koji dodaje u običnu statičku listu, dok close može iterirati/dispose/clear istu listu. Background thread zato ne nasljeđuje ThreadStatic session vrijednosti.
- **Problem:** ownership je AppDomain/thread, ne forma/Jupiter sesija; background task može završiti nakon closea ili konkurentno mijenjati listu.
- **Mogući produkcijski efekat:** race, cross-granule state bleed, ObjectDisposed i zadržavanje kontrola.
- **Predložena promjena:** prvo korelirati state sa session/form ID-em i ukloniti fire-and-forget za UI/COM kontrolu; init dovršiti na UI threadu prije upotrebe ili vratiti Task koji lifecycle owner čeka. Ne uklanjati ThreadStatic/global API dok Jupiter multi-session/thread model nije poznat.
- **Rizik promjene:** srednji/veliki.
- **Procjena rada:** M
- **Način testiranja:** brzo open/close, dvije granule, exception/slow custom init, isti AppDomain i više threadova.
- **Mjerljivi kriterijum uspjeha:** nema background add nakon dispose; broj global custom kontrola vraća se na baseline poslije closea.
- **Zavisnosti od drugih promjena:** dokumentovan Jupiter lifecycle.

### F-16

- **ID:** F-16
- **Naziv:** Carpo.Specific konkatenira korisničke vrijednosti u SQL tekst
- **Kategorija:** SQL, sigurnost, pouzdanost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Specific\UgovoriONajmu\Ugovori.cs:911-916,929-939`; `UgovoriDetail.cs:1356-1376,1698-1703`; `PartneriPopUp.cs:105-110`
- **Klasa/metoda:** `GetPubVar`, `TraziOrgShemu`, `TraziPartneri`
- **Dokaz iz koda:** `searchString` i `varIme` se konkateniraju u `select ...`/`exec ... @Uvjet='...'` string i izvršavaju kao `Carpo.Data.Common.DataType.Text`. Nema `SqlParameter`.
- **Problem:** vrijednost sa apostrofom mijenja SQL sintaksu, a neprovjerena vrijednost može izmijeniti naredbu. Svaki različit literal takođe proizvodi različit tekst za plan cache.
- **Mogući produkcijski efekat:** SQL injection ili barem query failure na legitimnim imenima/emailovima, plan-cache churn i teško auditabilan SQL.
- **Predložena promjena:** po metodi zamijeniti text concatenation sa stored-procedure `CommandType.StoredProcedure` pozivom i eksplicitno tipiziranim parametrima/dužinama. Ne raditi masovni regex rewrite; prvo search metode sa direktnim korisničkim unosom, zatim `varIme` uz allow-list.
- **Rizik promjene:** srednji; treba potvrditi tačne SP tipove/dužine i postojeću escaping semantics.
- **Procjena rada:** M
- **Način testiranja:** integration test sa apostrofom, Unicodeom, maksimalnom dužinom i injection markerom; actual plan/Query Store poređenje.
- **Mjerljivi kriterijum uspjeha:** nijedan pregledani search/pubvar call-site ne konkatenira input u SQL; rezultat je funkcionalno identičan za validne vrijednosti.
- **Zavisnosti od drugih promjena:** pristup SP potpisima/test bazi.

### 3.1 Razdvajanje nivoa dokaza

- **Potvrđeno kodom:** build/reference mismatch; output u Program Files; plaintext password/literalni SQL u logu/command lineu; lažni async grid overload; sinhroni SQL/SOAP/COM putevi; per-node Manager save; DataSet/ARITHABORT/implicitni parametri; transaction scope/error handling; statički datalist rast; COM bez releasea; `SqlContext` command ownership; custom-control task/static-list race; konkatenirani SQL u UgovoriONajmu.
- **Vjerovatno, ali uticaj nije kvantifikovan:** grid rebind/best-fit kao dominantan freeze; `foreachrow` kao stvarni produkcijski N+1; parser kao CPU hot path; XML cache kao neto dobitak; event subscription kao stvarni retention root. **Potrebno mjerenje/profilisanje.**
- **Stored procedure tvrdnje:** C# call-site potvrđuje broj/vrstu poziva i tip parametra koji šalje; indeks, cardinality estimate, implicit conversion i plan regresija nisu dokazani bez SP teksta/actual plana. **Potrebno mjerenje/profilisanje.**
- **Style bez dokazanog značajnog performance efekta:** `catch { throw; }`, `throw ex`, prazni `IDisposable` templatei, neujednačeni nazivi i komentarisani mrtvi blokovi. To otežava dijagnostiku/održavanje, ali nije predstavljeno kao performance optimizacija.

## 4. QUICK WINS

### Manje od jednog dana

- Redaktovati password u `Starter.RegisterForm` logu; provjeriti logove test credential markerom.
- Dodati `Stopwatch`/correlation ID oko `Starter.Run`, XML faza, `Data.Core.SetData` i grid bind faze, iza config flaga.
- Ispraviti `SqlContext.CreateCommand` ownership i dodati characterization integration test.
- U `Data.Core.GetDataByName` zamijeniti `Where().Count()+foreach` jednom enumeracijom; performance korist je mala, ali promjena je lokalna i testabilna.
- Dodati guard/log za null/failed rollback koji čuva originalni exception, bez promjene transaction semantics.
- Dokumentovati i validirati da je jedini podržani produkcijski build `Release|x86`; ne brisati druge konfiguracije u istom PR-u.

### 1–2 dana

- Uvesti bounded replace semantics za `DatalistModelList` uz F5/open-close heap test.
- Debounce/dirty flag za Manager layout snapshot i eksplicitni flush na close.
- Dodati per-action SQL count/elapsed/rows telemetry u postojeći logger, sa redakcijom parametara.
- U `ComponentGrid` dodati refresh reentrancy guard i `BeginUpdate/EndUpdate` u `finally`, nakon DevExpress regression matrice.
- Napraviti clean-build skriptu/config koja piše u lokalni artifacts folder i generiše manifest DLL verzija; deployment ostaje odvojen.

### Do jedne sedmice

- Uvesti source `ProjectReference` za jedan donji slice, npr. Structure→Common, u novoj LocalBuild/CI konfiguraciji i dokazati binlogom.
- Characterization suite za Parser/Evaluate i XML Serializer na reprezentativnim sanitizovanim XML fajlovima.
- Batch spremanje Manager settingsa ili barem spremanje samo dirty nodeova.
- Parametrizovati `TraziPartneri`, `TraziOrgShemu` i `GetPubVar` u `Carpo.Specific\UgovoriONajmu`, uz integration test apostrofa/Unicodea.
- Izdvojiti Outlook gateway i deterministički COM cleanup uz STA test.
- Napraviti prvi stvarno neblokirajući grid query prototip na jednoj opt-in granuli, sa snapshotom parametara i single-flight pravilom.

## 5. PERFORMANCE BACKLOG

Redoslijed je očekivana korist / rizik / rad:

1. Korelacijski baseline i SQL action counter — velika korist, mali rizik, S/M.
2. Uklanjanje credential loga — velika sigurnosna korist, mali rizik, S.
3. Stabilan lokalni x86 build + DLL manifest — velika pouzdanost/developer korist, mali/srednji rizik, M.
4. `SqlContext` ownership/timeout/typed parametri — srednja/velika pouzdanost, mali rizik, S/M.
5. Manager dirty-only/debounce persistence — srednja UI/SQL korist, mali rizik, S/M.
6. `DatalistModelList` bound/replace — srednja x86 memory korist, mali rizik, S.
7. Grid single-flight i stvarni async SQL na jednoj granuli — velika UI korist, srednji rizik, M.
8. Uklanjanje duplog best-fit/layout rada na izmjerenom gridu — srednja/velika korist, srednji rizik, M.
9. Batch `foreachrow` SQL — potencijalno velika korist, srednji/veliki rizik, M/L.
10. Eksplicitni SQL parametri za top procedure — velika moguća SQL korist, srednji rizik, M.
11. XML scan optimizacija — srednja startup korist, mali/srednji rizik, M.
12. Bounded immutable XML cache — velika moguća warm-start korist, veliki correctness/memory rizik, L.
13. Parser token-plan cache i lookup indeksi — srednja CPU korist, srednji rizik, M.
14. DevExpress Server Mode/virtual/incremental loading — samo za potvrđene ekstremne gridove; velika moguća korist, veliki feature-compatibility rizik, L/XL.

### 5.1 Baseline i profilisanje

Preporučeni log je jedna JSON Lines stavka:

`timestamp, correlationId, sessionIdHash, moduleId, form, control, action, phase, elapsedMs, sqlCount, sqlElapsedMs, rows, tables, managedBytesDelta, threadId, uiThread, success, exceptionType`

Ne logovati password, connection string ni pune parametre. Module/form/control po potrebi hashirati u produkciji.

- **Starter.Run ukupno**
  - Mjesto: `Starter.Run` ulaz/finally.
  - Alat: `Stopwatch`, postojeći Logger; cold process i warm process.
  - Scenario: tri reprezentativne granule, po 10 cold i 30 warm otvaranja.
  - Poređenje: isti PC/DB/user/cache policy; p50/p95 i distribucija faza.
- **XML dohvat i deserializacija**
  - Mjesto: `GetCarpoRootXml`, `GetPartsSql`, `GetForms`, `GetParts`, finalni `XmlSerializer.Deserialize`.
  - Alat: Stopwatch + ETW allocation sampling (PerfView).
  - Scenario: mali/srednji/veliki XML, F5 reload, DMS i disk gdje postoje.
  - Poređenje: elapsed, allocated bytes, parts/files/partsql SQL count.
- **Otvaranje reprezentativne granule**
  - Mjesto: `Starter.Run` → prvi `Shown` i prvi idle.
  - Alat: Stopwatch; WinForms `Application.Idle` marker.
  - Scenario: Manager sa tree/master/detail gridom.
  - Poređenje: cold/warm p50/p95.
- **Inicijalni grid binding**
  - Mjesto: prije `DataSource=table`, poslije template/layout/best-fit.
  - Alat: Stopwatch; row/column/group count.
  - Scenario: 0/1/1k/10k+ redova, grouping on/off.
  - Poređenje: odvojeno query/fill/bind/layout/best-fit.
- **Grid refresh**
  - Mjesto: `Populate` correlation scope do stabilnog UI stanja.
  - Alat: Stopwatch + SQL counter.
  - Scenario: F5/button/timer refresh, rapid double click.
  - Poređenje: elapsed, SQL count, GC, fokus/selekcija.
- **SQL broj i trajanje po akciji**
  - Mjesto: centralno u `Carpo.Data.Common`, `SqlContext`, `SqlHelper`.
  - Alat: C# telemetry + SQL Server Extended Events sa `client_app_name`/correlation tagom gdje je moguće.
  - Scenario: open, save, foreachrow, Manager close, fiskalizacija.
  - Poređenje: count, total, max, rows; ne samo pojedinačni query time.
- **UI freeze intervali**
  - Mjesto: WinForms UI heartbeat timer 50–100 ms, log samo gap iznad npr. 250 ms.
  - Alat: custom heartbeat; PerfView/WPA CPU sampling.
  - Scenario: startup, grid refresh, export, close, SOAP/Outlook.
  - Poređenje: max gap i suma gapova po akciji.
- **Private bytes, managed heap, GC 0/1/2**
  - Mjesto: process counters na početku/kraju ciklusa.
  - Alat: PerfMon (`Process\Private Bytes`, `.NET CLR Memory`), PerfView, dotMemory/ANTS ako licenca postoji.
  - Scenario: 100 open/close/F5 ciklusa u x86 TestShell/Jupiter.
  - Poređenje: steady-state slope nakon warmupa i forced full GC samo u test harnessu.
- **Žive forme i kontrole poslije zatvaranja**
  - Mjesto: test-only weak-reference registry u Base form/control initu.
  - Alat: WeakReference + heap snapshot/gcroot.
  - Scenario: 20 otvaranja/zatvaranja Manager/edit/Info formi.
  - Poređenje: broj preživjelih nakon dvije full GC u testu; production ne treba zvati `GC.Collect`.
- **Otvorene SQL konekcije**
  - Mjesto: Extended Events/login logout i performance counter poola; wrapper active-call gauge.
  - Alat: Extended Events, `NumberOfActiveConnections`, `NumberOfPooledConnections`.
  - Scenario: refresh storm, transaction failure, reader exception.
  - Poređenje: active count se vraća na baseline; nema monotonog rasta.

Za SQL procedure prikupiti actual execution plan, `STATISTICS IO,TIME`, returned row/column count, parameter values/tipove (sanitizovano), wait/deadlock podatke i Query Store historiju. Bez toga nema dokaza da treba indeks, recompile, paging ili druga SP promjena.

## 6. REFACTORING ROADMAP

### Faza 0: baseline i instrumentacija

- **Preduvjeti:** odabrati 3–5 reprezentativnih granula i sanitizovane XML/data scenarije; definisati x86 test VM.
- **Rad:** PerfScope/correlation, SQL counter, UI heartbeat, build binlog/manifest, memory open-close harness.
- **Očekivani rezultat:** dokazani top hot pathovi i ponovljiv before/after protokol.
- **Rollback:** config flag potpuno isključuje telemetry; promjena ne utiče na javne ugovore.

### Faza 1: sigurni lokalni refaktoring

- **Preduvjeti:** faza 0 i minimalni characterization testovi.
- **Rad:** credential redaction; `SqlContext` ownership; rollback error preservation; bounded datalist metadata; Manager dirty/debounce; lokalni build output.
- **Očekivani rezultat:** manje sigurnosnih/resource rizika bez promjene funkcionalne semantics.
- **Rollback:** svaki zahvat zaseban PR/feature flag; zadržati stari persistence put.

### Faza 2: izdvajanje odgovornosti

- **Preduvjeti:** testovi event/parser/XML i poznat Jupiter lifecycle.
- **Rad i predloženi interfejsi:**
  - `IDataCommandExecutor.Execute(JData, DataExecutionContext)` iz `Data.Core.SetData` i `Carpo.Data.Common`; postojeći `Data.Core` ostaje facade.
  - `IGridDataLoader.Load(GridLoadRequest)` iz `ComponentGrid.GetDataAsync`; `Populate` ostaje javni wrapper.
  - `IExternalActionGateway` sa `IOutlookContactGateway`, `IFiscalizationGateway`, `ILegacyComInvoker` iz posebnih `Events` grana.
  - `IXmlDefinitionLoader.Load(DefinitionKey)` iz `Serializer`; postojeći konstruktor/metoda delegiraju.
  - `IManagerSettingsStore.Load/SaveBatch` iza postojećeg `SettingsService`.
- **Očekivani rezultat:** unit-testabilne granice i manji god-method dispatch bez masovnog DI frameworka; constructor injection samo u novim internim tipovima, default adapteri čuvaju kompatibilnost.
- **Rollback:** facade može vratiti stari inline implementation; entry pointovi ostaju netaknuti.

### Faza 3: optimizacija hot pathova

- **Preduvjeti:** profiler dokaz, regression matrica i feature flags po modulu/granuli.
- **Rad:** single-flight async grid load; manje rebind/layout rada; batch foreachrow; eksplicitni SQL tipovi; smanjenje XML enumeracija; parser token-plan cache.
- **Očekivani rezultat:** mjerljivo niži p50/p95 i UI freeze bez promjene host ugovora.
- **Rollback:** per-feature/per-grid XML/config flag vraća legacy put.

### Faza 4: dugoročna arhitektura

- **Preduvjeti:** stabilan CI, test paket i vlasništvo nad deploymentom.
- **Rad:** konzistentan source dependency graph, package/reference manifest, session-scoped state umjesto dijela statika, versioned immutable XML definition cache, odvajanje Carpo.Specific adaptera.
- **Očekivani rezultat:** reproducibilan TFS build i manji blast radius promjena.
- **Rollback:** paralelni compatibility facade i side-by-side deployment set; nema promjene `Starter.Run`, `IJupiterStart` ni Jupiter host ugovora.
- **Platforma:** eventualna moderna .NET procjena je odvojeni budući program i nije rješenje za ovaj backlog.

### 6.1 Minimalni safety net

**Odmah automatizovati bez Jupiter hosta**

- Parser/Evaluate golden testovi: literal, control, data, global/parent, method, format, null, date i invalidni izraz.
- Serializer testovi: root, nested part/form, missing required/optional part, partsql adapter fake, recursion guard, F5/version invalidacija.
- `Data.Core` parameter conversion, table parameter i output parameter testovi.
- `SqlContext`/`Carpo.Data.Common` integration testovi na disposable test bazi: disposal, timeout, transaction, typed params, multi-table result.
- `GridRefresh` helper testovi gdje DevExpress control može raditi u STA test threadu.
- Build test: clean `Release|x86`, reference manifest i assembly version consistency.

**Carpo.TestShell automatizovani/poluautomatizovani smoke**

- open/close/F5 ciklus; Manager tree/master/detail; grid filter/group/sum/export; saved layout; rapid refresh; transaction rollback; memory weak-reference test.
- TestShell mora biti x86 i koristiti isti DevExpress/Jupiter dependency set kao produkcija.

**Zahtijeva Jupiter/vanjsko okruženje**

- `Starter.Run` host binding, SharedAssemblies probing i deployment rollback.
- VB6/ADODB/SpinClient ugovor, COM TotalGen, Outlook installed/running stanja.
- DMS različite verzije i remote file path.
- fiskalizacija certifikat/SOAP/test endpoint.
- prava/translator/Jupiter session/global state.

Ručna regresiona checklista mora obuhvatiti: ulaz u 3–5 granula, edit/save/cancel, permissions, search, tabs, tree drag/drop, master-detail fokus, layout persistence, print/export, DB error `+close/+ok/+focus`, offline/timeout, multi-monitor/DPI, x86 memory i restart/rollback starog DLL seta.

## 7. PRIJEDLOG PRVIH PET PR-OVA

### PR 1 — Performance correlation bez promjene ponašanja

- **Cilj:** izmjeriti startup/XML/SQL/grid/Manager faze.
- **Zahvaćeni fajlovi:** novi mali helper u `Carpo.Core\Log`; `Starter.cs`, `Serializer.cs`, `Data\Core.cs`, `ComponentGrid.cs`, `Manager.cs`.
- **Približan opseg:** 200–350 LOC.
- **Rizik:** mali; log volumen.
- **Test plan:** config on/off, jedan correlation ID kroz open/refresh/close, bez credentiala.
- **Očekivani rezultat:** baseline JSONL trace sa SQL countom i fazama.

### PR 2 — Credential redaction i log policy

- **Cilj:** ukloniti plaintext DB password i redaktovati connection/SQL vrijednosti.
- **Zahvaćeni fajlovi:** `Starter.cs`, `Carpo.Data\Log.cs`/logger helperi, testovi; `ComDll.cs` samo dokumentovanje/feature flag ako host ugovor dopušta.
- **Približan opseg:** 100–200 LOC.
- **Rizik:** mali za log; command line se ne mijenja bez odobrenja.
- **Test plan:** marker credential ne postoji u logovima; dijagnostički naziv procedure ostaje.
- **Očekivani rezultat:** support logovi ne sadrže tajne.

### PR 3 — `SqlContext` ownership i integration characterization

- **Cilj:** ukloniti vraćanje disposed command-a i standardizovati timeout.
- **Zahvaćeni fajlovi:** `SqlContext.cs`, novi test projekt/fajlovi kompatibilni sa .NET Framework 4.8.
- **Približan opseg:** 150–250 LOC.
- **Rizik:** mali.
- **Test plan:** svi overloadi, reader/scalar/nonquery, timeout, null/binary/string/decimal.
- **Očekivani rezultat:** deterministički disposal i dokumentovan command timeout.

### PR 4 — Bounded datalist metadata lifecycle

- **Cilj:** zaustaviti statički rast na F5/open-close.
- **Zahvaćeni fajlovi:** `SpecificElementList.cs`, `Serializer.cs`, `Log.cs`, testovi.
- **Približan opseg:** 150–250 LOC.
- **Rizik:** mali/srednji.
- **Test plan:** 100 deserialize ciklusa za dva modula/baze, F11 log lookup, heap snapshot.
- **Očekivani rezultat:** broj metadata entryja je bounded i replace semantics radi.

### PR 5 — Manager dirty-only persistence

- **Cilj:** smanjiti layout serializaciju i SQL na closeu.
- **Zahvaćeni fajlovi:** `Manager.cs`, `SettingsService.cs`; SP samo u zasebnom follow-upu.
- **Približan opseg:** 200–350 LOC.
- **Rizik:** mali/srednji.
- **Test plan:** resize/filter/move burst, close, reopen, failure i local-file fallback.
- **Očekivani rezultat:** samo dirty nodeovi se serializiraju/spreme; close trace pokazuje manje SQL poziva.

## 8. ŠTA NE TREBA DIRATI

- Ne mijenjati `Starter.Run`, `IJupiterStart`, plugin class names ni argument redoslijed. Oko njih dodati samo interne facade/instrumentaciju.
- Ne uklanjati x86 niti forsirati AnyCPU/x64. ADODB/COM/Jupiter host i address-space ponašanje zahtijevaju produkcijski x86 test.
- Ne mijenjati sve prebuilt reference odjednom. Uvesti LocalBuild source references po dependency sliceu i dokazati paket.
- Ne uvoditi globalni async/await kroz `Events`. Parser, Form, Control i global state nisu thread-safe; background smije dobiti samo immutable snapshot, a UI update mora nazad na UI context.
- Ne uvoditi DevExpress Server Mode kao opšti default. XML-driven custom summaries, grouping, per-row actions, export, master/detail, selection i DataTable filter semantics možda nisu kompatibilni. Razmatrati samo za jednu izmjerenu read-only granulu.
- Ne cacheirati mutable `JRoot`, DataSet/DataTable ni parser rezultat bez bounded ključa i invalidacije. Posebno ne dijeliti `JRoot` između baza/modula/formi.
- Ne uklanjati `SET ARITHABORT ON` bez execution plan/Query Store poređenja; može promijeniti plan izbor.
- Ne prepisivati `SqlHelper` samo zato što je velik. Repository-wide pretraga pokazuje malo aktivnih call-siteova; prvo odvojiti stvarno korišten API od legacy mrtvog dijela.
- Ne “čistiti” široko catch blokove u jednom PR-u. Error semantics (`+close`, `+ok`, `+focus`, transaction stop) su dio XML/DB ugovora.
- Ne mijenjati ThreadStatic/global state dok se ne dokumentuje da li Jupiter može pokretati više granula/sesija u istom AppDomainu i na kojim threadovima.
- Ne refaktorisati designer-generated WinForms fajlove radi stila; korist je mala, merge/regression rizik visok.
- Ne spajati Carpo.Specific u core. Umjesto toga izdvajati samo dokazano duplirane data/COM adaptere sa jasnim vlasnikom.

## 9. OTVORENA PITANJA

1. Koje su tri najsporije produkcijske granule i njihove cold/warm p50/p95 vrijednosti?
2. Koliki su tipični i maksimalni row/column countovi tree/master/detail gridova?
3. Koje XML granule koriste `foreachrow`, `transaction`, `partsql`, `BestFitOnRefresh`, `AutoRefresh` i `Async`?
4. Koji stored procedure pozivi dominiraju ukupnim elapsed/IO i koji su actual planovi za stvarne parametre?
5. Da li Query Store radi i postoje li implicit conversion, spill, deadlock i timeout dokazi?
6. Koja tačno Jupiter host verzija učitava Carpo i kakva je assembly binding/probing politika?
7. Da li više Carpo granula živi istovremeno u istom AppDomainu/threadu?
8. Koji `SqlHelper` overloadi se koriste iz eksternih Jupiter/custom DLL-ova, izvan ovog repositoryja?
9. Da li su `10.0.0.24` reference namjerna compatibility meta ili samo zastarjeli csproj metadata?
10. Koja je službena TFS build konfiguracija i gdje trenutno nastaje deployment paket?
11. Kako izgleda deployment/rollback u SharedAssemblies: atomska zamjena cijelog seta, restart hosta, backup i verifikacija hash/version manifesta?
12. Jesu li developer-specific DevExpress DLL putanje potrebne zbog patchane verzije?
13. Da li SpinClient može primiti credential preko zaštićenog IPC-a/Windows autha ili je trenutni argument ugovor nepromjenjiv?
14. Je li Outlook uvijek STA i smije li Carpo kreirati/zatvarati Application instancu?
15. Koje forme moraju sačuvati settings i ako Jupiter proces padne prije FormClosing?
16. Postoje li produkcijski dumpovi za x86 OOM, GDI/User object rast ili zombie Outlook procese?

## 10. KONAČNA PRIORITIZOVANA LISTA

1. **Uvesti correlation baseline** — korist: velika; rizik: mali; rad: S/M; prethodno profilisanje: ne; redoslijed: 1.
2. **Ukloniti plaintext credential i literal-value SQL logging** — korist: velika; rizik: mali; rad: S/M; profilisanje: ne; redoslijed: 2.
3. **Generisati x86 clean-build/reference/version manifest** — korist: velika; rizik: mali; rad: S; profilisanje: ne; redoslijed: 3.
4. **Ispraviti `SqlContext` disposed command i testirati overload-e** — korist: velika; rizik: mali; rad: S; profilisanje: ne; redoslijed: 4.
5. **Parametrizovati `UgovoriONajmu` search/pubvar SQL konkatenacije** — korist: velika; rizik: srednji; rad: M; profilisanje: ne, ali SP signature test: da; redoslijed: 5.
6. **Definisati reprezentativne TestShell/Jupiter smoke scenarije** — korist: velika; rizik: mali; rad: M; profilisanje: ne; redoslijed: 6.
7. **Stabilizovati LocalBuild/CI `Release|x86` output van Program Files** — korist: velika; rizik: srednji; rad: M; profilisanje: ne; redoslijed: 7.
8. **Bound/replace `DatalistModelList`** — korist: srednja; rizik: mali; rad: S; profilisanje: da, heap baseline; redoslijed: 8.
9. **Manager dirty/debounce i close SQL counter** — korist: srednja/velika; rizik: mali; rad: S/M; profilisanje: da; redoslijed: 9.
10. **Sačuvati originalnu transaction/rollback grešku** — korist: velika; rizik: mali/srednji; rad: S; profilisanje: ne; redoslijed: 10.
11. **Uvesti stvarni async grid SQL sa single-flight pravilom na jednoj opt-in granuli** — korist: velika; rizik: srednji/veliki; rad: M; profilisanje: da; redoslijed: 11.
12. **Ukloniti izmjereni dupli best-fit/layout rad** — korist: srednja/velika; rizik: srednji; rad: M; profilisanje: da; redoslijed: 12.
13. **Batch Manager settings save** — korist: srednja; rizik: srednji; rad: M; profilisanje: da; redoslijed: 13.
14. **Inventarisati i batchovati SQL `foreachrow` hot spotove** — korist: velika; rizik: srednji/veliki; rad: L; profilisanje: da; redoslijed: 14.
15. **Eksplicitno tipizirati parametre top SQL procedura** — korist: velika; rizik: srednji; rad: M; profilisanje/planovi: da; redoslijed: 15.
16. **Izdvojiti Outlook/COM gateway i cleanup** — korist: srednja; rizik: srednji; rad: M; profilisanje: da; redoslijed: 16.
17. **Smanjiti XML višestruke enumeracije** — korist: srednja; rizik: mali/srednji; rad: M; profilisanje: da; redoslijed: 17.
18. **Parser token-plan cache i lookup indeksi** — korist: srednja; rizik: srednji; rad: M; profilisanje: obavezno; redoslijed: 18.
19. **Postepeno zamijeniti interne DLL reference ProjectReference-ima u LocalBuildu** — korist: velika; rizik: srednji; rad: L; profilisanje: ne, ali binlog obavezan; redoslijed: 19.
20. **Procijeniti bounded immutable XML cache samo za dokazani startup hot spot** — korist: potencijalno velika; rizik: veliki; rad: L; profilisanje: obavezno; redoslijed: 20.

---

**Zaključak:** najveći neposredni dobitak dolazi iz dokazivosti i kontrole build/runtime ulaza: korelacijski baseline, konzistentan x86 DLL set, uklanjanje credentiala, zatim lokalne popravke `SqlContext`, Manager persistencea i statičkog XML metadata rasta. Grid async/rebind, foreachrow batch i XML/parser cache treba raditi tek nakon mjerenja i iza reverzibilnih, per-granule flagova.
