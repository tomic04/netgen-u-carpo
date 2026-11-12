# Carpo Framework — Tehnički Audit i Plan Poboljšanja

| | |
|---|---|
| **Model** | Claude Fable 5 (thinking, high effort) — Anthropic, pokrenut kroz Cursor |
| **Datum analize** | 31.07.2026. |
| **Repository** | `c:\TFS\Core Techonologies\Carpo\Devex_23_4.8_framework` (TFS: `http://spin-devops:8080/tfs/core technologies`) |
| **Branch / commit** | TFVC workspace — nije git repozitorij, commit hash nije dostupan. Analizirano stanje lokalnog workspacea na datum analize. |
| **Pristup repozitoriju** | Model je imao pristup **cijelom** lokalnom repozitoriju (488 .cs fajlova, ~79.000 linija, 13 projekata + solution). |
| **Šta nije pregledano** | Jupiter host (nije u repou), definicije stored procedura (nisu u repou), produkcijski granule XML-ovi, interni kod `SpinDMSUtil`/`CarpoFinder`, `JupiterTranslator10.dll`, `RibbonBase.v23.dll`, `Spin.Printing.v23.dll`, `Dimenzije.dll` (samo binarne reference), VB6/COM strana `TotalGen`-a. `Carpo.CMS`, `Carpo.Designer` i `Carpo.XmlGen` pregledani ciljano (pattern-search), ne liniju po liniju. |

**Metodologija:** pregled `Carpo.sln` + svih csproj-eva; potpuno čitanje `SqlHelper.cs`, `Common.cs`, `Connection.cs`, `Log.cs`, `Constructor.cs`, `SqlContext.cs`, `Events.cs`, `Parser.cs`, `ParserDirect.cs`, `Evaluate.cs`, `Statemants.cs`, `Logic\Data\Core.cs`, `Starter.cs`, `Connector\Xml.cs`, dijelova `JupitetShared.cs`; dubinski pregledi `ComponentGrid.cs`/`ComponentBase.cs`/`ComponentTree.cs`, `Manager.cs`/`JupiterEditor.cs`/`ControlInit.cs`, `Serializer.cs`/`Carpo.Structure\Models`, `Carpo.Specific` i repo-wide pattern pretrage (15 kategorija). U kod **nije** dirano — jedina izmjena je ovaj dokument.

Oznake pouzdanosti: **potvrđeno** = vidljivo direktno iz koda s navedenim linijama; **visoka vjerovatnoća** = obrazac potvrđen, stvarni uticaj zavisi od XML definicija/podataka; **hipoteza** = potrebno mjerenje/profilisanje.

---

## 1. EXECUTIVE SUMMARY

1. **Transakcioni engine tiho guta greške.** `Events.CheckTransaction` nakon `Rollback()` ne loguje, ne obavještava korisnika i ne prosljeđuje exception — neuspjela transakcija izgleda kao uspjeh, osim ako XML eksplicitno provjerava `transaction` varijablu (`Carpo.UI\Logic\Events.cs`, 1511–1519). Potvrđeno. Ovo je najozbiljniji pojedinačni nalaz u repou.
2. **SQL transakcija ostaje otvorena preko modalnih dijaloga.** `CheckTransaction` omotava cijeli `RunActions`, koji može prikazati `confirm`/`message` dijaloge — SQL lockovi se drže dok korisnik razmišlja. Potvrđeno.
3. **Lozinka se loguje u čistom tekstu** (`Starter.RegisterForm`, 295–299), app-role lozinka `Passw0rd!` je hardkodirana u kodu (`SqlHelper.cs`, 2538–2539), a `Connection.Pwd` drži kredencijale u ThreadStatic stringu. Potvrđeno.
4. **Svaki SQL poziv plaća dva skrivena poreza:** poseban roundtrip `SET ARITHABORT ON` prije svake komande i bezuslovno generisanje punog SQL teksta za logging (`Log.CommandAsSql` s inline regexom) — i kad se log ne koristi (`Carpo.Data\Common.cs`, 51–54/90/166–183). Potvrđeno.
5. **Grid je klijentski, sinhron i bez suspenda:** `Populate` nula `DataSource`, puni cijeli `DataTable` na UI threadu (lažni async — `Task.FromResult` NAKON sinhronog `GetDataSet`), `BeginUpdate/EndUpdate` je zakomentarisan, `BestFitColumns` se može izvršiti dvaput, a `FormatRules` se **akumuliraju pri svakom Populate** (`ComponentGrid.cs`, 897–923, 1241–1252, 1309–1333, 4139–4168). Potvrđeno.
6. **N+1 obrasci postoje na tri nivoa:** `foreachrow` akcije izvršavaju puni event pipeline (potencijalno SQL) po svakom redu grida; `Carpo.XmlGen\Data\GetDataSet.GoThroughParent` zove `ExecuteQuery` po parent redu; `Manager_FormClosing` snima filter layoute jedan SQL poziv po ključu. Potvrđeno (obrazac), uticaj zavisi od XML-a — potrebno mjerenje.
7. **Event handler leak obrazac je sistemski:** `ComponentGrid` 33× `+=` / 3 stvarna `-=`; `ComponentBase` ~25× `+=` / 0 `-=`; tab-lambde u `ControlInit` i `Manager` se nikad ne otkače. Zatvorene forme se „liječe“ sa `GC.Collect()` u `FormClosing` — blokirajući full GC na UI threadu pri svakom zatvaranju forme (`JupitetShared.cs`, 604–608). Potvrđeno.
8. **Fiskalizacija (ZKI/JIR) i Outlook su hardkodirani u generičku `Events.RunActions` petlju**, izvršavaju se sinhrono na UI threadu (JIR: `Task.Run(...).GetAwaiter().GetResult()` — sync-over-async preko mreže), bez retry logike i bez `Marshal.ReleaseComObject` za Outlook COM objekte (`Events.cs`, 278–410; `MicrosoftAPI.cs`, 18–74). Potvrđeno.
9. **Build nije reproducibilan:** projekti se međusobno referenciraju preko **prebuilt DLL-ova iz `Program Files`** (HintPath) umjesto ProjectReference (jedini izuzetak: Carpo.UI→Carpo.Data), output ide direktno u `Program Files\...\SharedAssemblies\Spin`, postoji hardkodiran put na desktop developera (`C:\Users\valentino.jagodic\Desktop\DevExpress.Sparkline.v23.1.Core.dll` u `Carpo.UI.csproj`, linija 111), a verzije se mijenjaju ručno po `upute za distribuciju.txt`. Rizik buildanja protiv stare verzije DLL-a je realan. Potvrđeno.
10. **SQL injection površina u `Carpo.Specific`:** ~25 mjesta gradi SQL tekst konkatenacijom, uključujući korisnički unos (`PartneriPopUp.cs`, 109–110: `@Uvjet='" + searchString + "'"`). Potvrđeno.
11. **Parser/Evaluate/Statemants nemaju nikakav cache** — isti XML izrazi se iznova parsiraju pri svakom eventu, uslovu i renderu ćelije; `Evaluate.GetSubStrings` kreira `new Regex` po pozivu; paint-path (`RowCellStyle`/`CustomDrawCell`) po ćeliji izvršava evaluaciju uslova. Potvrđeno (obrazac); CPU uticaj — potrebno profilisanje.
12. **Kontrola toka preko exception poruka:** SQL RAISERROR tekstovi sa sufiksima `+close`/`+ok`/`+focus` parsiraju se iz `SqlException.Message` i upravljaju UI ponašanjem (`Logic\Data\Core.SetData`, 183–226). Uz to ~41 prazan `catch {}` i desetine no-op `catch { throw; }` blokova. Potvrđeno.
13. **`SqlContext.CreateCommand` vraća već disposan `SqlCommand`** (`using var` unutar metode koja ga vraća — latentan bug), 118× `AddWithValue` bez tipova (rizik implicitnih konverzija i plan cache pollution), reader bez `using` u `SqlContext.ExecuteReader`. Potvrđeno.
14. **XML definicija granule se deserializuje jednom po startu** (dobro), ali: `GetPartsSql` radi **duplu** deserializaciju `JRoot`-a kad postoji `partsql`, statička `SpecificElementList.DatalistModelList` raste bez čišćenja pri svakom Reload/Retry, a `JRoot` graf je **dijeljen i mutabilan** među svim formama (Manager radi `DefinitionRoot.Form.AddRange`). Potvrđeno.
15. **`Manager` i `JupiterEditor` rade sinhroni SQL + disk I/O u konstruktoru, `Load` i `FormClosing`** (filter postavke iz `spCarpoFilterPostavke` ili `%AppData%`, CarpoVars layouti, veličina forme) — sve na UI threadu; `FormClosing` u filter modu ima **early return koji preskače** snimanje ostalih layouta i splittera. Potvrđeno.

---

## 2. MAPA SISTEMA

### 2.1 Projekti i zavisnosti

Stvarne zavisnosti (kombinacija ProjectReference + DLL HintPath referenci — vidi nalaz B-01):

```
Carpo.Structure   (XML modeli: JRoot/JForm/JLayout/JData; interfejsi IJControl, IJupiterStart)
   ↑
Carpo.Common      (Translator, Conversion, JMessages/JConfirm, IniFile, Export)
   ↑
Carpo.Data        (SqlHelper [MS DAAB], Common.ExecuteQuery*, Connection [ThreadStatic],
   ↑               SqlContext, Get/Set wrapperi za CarpoVars, GridTemplates, Vars)
Carpo.Core        (Logger, PubVars, DependencyContainer, Log forma, PrintDlg)
   ↑
Carpo.UI          (Connector\Starter, Serializer, Events/Parser/Evaluate/Statemants,
   ↑               Components\ComponentGrid/Tree/Base, Forms\Manager/JupiterEditor/JupitetShared,
   ↑               Helpers\Fiscalization/MicrosoftAPI/PdfMacFixer; referencira adodb + Outlook interop)
Carpo.Gen         (= "Carpo.dll": Start : Starter — entry point koji Jupiter učitava)
   ↑
Carpo.Lite        (samostalni EXE launcher za dev)      Carpo.TestShell (ručni test harness)
Carpo.XmlGen      (generisanje dokumenata iz XML+SQL)   Carpo.Designer / Carpo.CMS (alati)
Carpo.Specific    (klijentski kod: UgovoriONajmu, Kontor\Virmani, Cjenik, Matpos, Grafko...;
                   učitava se ISKLJUČIVO refleksijom preko XML atributa dll/class — nema compile-time reference iz Carpo.UI)
```

### 2.2 Entry pointovi

| Entry | Mehanizam |
|---|---|
| **Produkcija** | Jupiter host → `Carpo.Gen.Start` (naslijeđuje `Carpo.UI.Connector.Abstract.Starter`), postavlja propertyje (`ModuleId`, `MenuID`, ...) pa zove `Run(server, database, username, password, operateriid, parent, pTop, pLeft, path)`. Konvencija iz komentara: `asm:Carpo.Granula.Start:ModuleId` (`Forms\Jupiter\Base.cs`, 43). `IJupiterStart` interfejs postoji u `Carpo.Structure`, ali ga niko u repou ne implementira (ugovor prema hostu). |
| **Dev** | `Carpo.Lite\Program.cs` (23–27) i `Carpo.TestShell\Main.cs` (181–184): `new Gen.Start() { TestXmlPath = ... }.Run(...)`. |
| **Klijentski specifično** | `Carpo.Specific.Kontor.Virmani.Start.VirmaniWithId(...)` i sl., pozvani refleksijom (`JCustomMethod.Run`: `Assembly.Load` + `Invoke`). |

### 2.3 Startup tok (potvrđen kodom)

```
Starter.Run (Starter.cs 78–138)
 ├─ new Logger + DependencyContainer.Add (globalni, "last wins")
 ├─ ThreadStatic globali: Connection.ConnectionString/ADO/Uid/Pwd, PubVars.OperateriId
 ├─ Translator.Init(IniFile["Server"])
 ├─ DeserializeXml (162–213):
 │    put = TestXmlPath ili RemoteDllPath\Carpo\{ModuleId}
 │    → SpinDMSUtil.FileFinder.CheckFileExist → Serializer.DeserializeXml
 │       → CarpoFinder.GetCarpoRootXml (disk/DMS) → XDocument merge (partsql→forms→parts→custom)
 │       → new XmlSerializer(typeof(JRoot)).Deserialize  [ako ima partsql: JRoot se deserializuje DVAPUT]
 ├─ SetInitForm: bira JForm type="master"; ManagerLayout ⇒ Manager, Layout ⇒ Standard
 └─ RegisterForm (265–371): varijable, Permissions.SetPermissions (SQL), VarsLayout.SetLayout (SQL),
      dodjela DIJELJENOG DefinitionRoot forme → form.Start(...) → Win32 SetParent + Show
```

### 2.4 Tri reprezentativna toka (ulaz → baza → UI)

**Tok A — otvaranje Manager granule i punjenje grida:**
`Starter.Run` → `Manager` ctor (`Manager.cs` 245–332: **SQL `spCarpoFilterPostavke` + AppData disk I/O u konstruktoru**) → `TreeWithGrids_Load` → `PopulateControls` (600–607) → `JGridControl.Populate` (`ComponentGrid.cs` 878–979: `DataSource=null` → `GetDataAsync` → **sinhroni** `DataDefinition.GetDataSet`) → `Logic\Data\Core.SetData` (96–250: `Evaluate` komande, `GetParameters` kroz `Parser`) → `Carpo.Data.Common.ExecuteQuery` (36–133: `SET ARITHABORT ON` roundtrip + `CommandAsSql` + `SqlDataAdapter.Fill`) → `SetData` bind punog `DataTable` (1296–1345) → `BestFitColumnsInit` (+ mogući drugi BestFit) → `TranslateGridColumns` → `SetConditionalFormatting` (**FormatRules.Add bez Clear**) → `FocusedRowChanged(null)` → XML notifikacije (mogući detail reload).

**Tok B — korisnička akcija s transakcijom (npr. spremi dokument):**
klik dugmeta → `ComponentBase.SetEvents` handler → `new Events(...).RunNotification` → `RunActions` (270–479) → za svaku akciju do 17 sekvencijalnih `Check*` provjera (`Checking`, 533–576) → `CheckTransaction` (1465–1530): `BeginTransaction` → rekurzivni `RunActions(dataActionsList)` (uklj. `CheckConfirm` **modalne dijaloge unutar transakcije**) → `NotifyData` → `Core.SetData` → `Common.ExecuteAzurInTran` → `Commit` / **`catch { Rollback; /* bez loga i poruke */ }`** → UI čita `form.LastTransactionSuccess`.

**Tok C — fiskalizacija računa (JIR):**
XML akcija `type="JIR"` → hardkodirana grana u `RunActions` (347–410) → `SqlContext.ExecuteReader<CertificateModel>("select * from mpfiskalizacija ...", CommandType.Text)` → `X509Certificate2` s diska (bez `using` u JIR grani) → `Core.GetDataSet("JIRLoad")` → `Regex.Replace` + `XmlDocument` → `Fiscalization.SignXmlDoc` → `SoapWebContext.SendSoapRequest` preko `Task.Run(...).GetAwaiter().GetResult()` (**UI thread blokiran mrežnim pozivom**) → parsiranje `tns:Jir` → `Core.ExecuteData("UpdateAll")`. Stanje između ZKI i JIR akcija prenosi se kroz **instance field `zki`** (linija 269).

### 2.5 Runtime state model

- **Po formi:** `DataList` (Dictionary<string, DataTable> — keš rezultata), `VariablesForm`, `ControlList`, `DefinitionForm`.
- **Globalno/ThreadStatic:** `Connection.*` (7 polja, uklj. lozinku), `PubVars` (OperateriId, MenuID, DmsExists), `Starter.ImeFirme`, `DependencyContainer` (Logger), `IniFile.ini`, `GlobalCustomControls.CustomControls`, `SpecificElementList.DatalistModelList` (**raste bez čišćenja**), `Common.OutParams` (**static, dijeljen između poziva — race**), `SqlHelper.ParamCache`.

---

## 3. TOP NALAZI

> Prioriteti: **P0** = rizik gubitka podataka / sigurnost / tiha greška; **P1** = visok, mjerljiv uticaj na performanse ili stabilnost; **P2** = srednji uticaj / arhitekturna higijena; **P3** = nisko.

---

### N-01 — Tihi rollback transakcije bez loga i poruke

- **ID:** N-01
- **Naziv:** `CheckTransaction` guta exception nakon rollbacka
- **Kategorija:** Pouzdanost / integritet podataka
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Carpo.UI\Logic\Events.cs`
- **Klasa/metoda:** `Events.CheckTransaction`
- **Dokaz iz koda:** linije 1511–1519 — `catch { transaction.Rollback(); connection.Close(); foreach (...) FormsVar.SetVariable(...); }` — nema `form.Logger.Error`, nema `JMessages`, nema rethrow. `form.LastTransactionSuccess = "success"` postavlja se samo u sretnom putu (1509).
- **Problem:** Ako bilo koja akcija unutar transakcije pukne, korisnik ne dobija nikakvu povratnu informaciju, log ne sadrži uzrok, a nastavak akcija poslije transakcije se izvršava normalno. Ponašanje zavisi od toga da li konkretni granule XML provjerava `transaction != success` — što je opt-in, ne default.
- **Mogući produkcijski efekat:** korisnik misli da je dokument spremljen, a nije; nemoguće dijagnosticirati sporadične „nestale“ zapise.
- **Predložena promjena:** u `catch`: (1) `form.Logger.Error("CheckTransaction", ex.Message, ex)`, (2) postaviti `form.LastTransactionSuccess = "failed"`, (3) prikazati poruku korisniku (postojeći `JMessages.Info` mehanizam s parsiranjem `+close` sufiksa iz `Core.SetData` se već izvršava za SqlException — ali samo ako `connTranModel != null` propagira; provjeriti oba puta), (4) razmotriti rethrow iza cleanup-a uz postojeći outer handler u `RunNotification`.
- **Rizik promjene:** nizak-srednji — postojeći XML-ovi koji se oslanjaju na tiho ponašanje mogli bi početi prikazivati poruke; zato poruku uvesti iza feature flaga ili prvo samo logging + `LastTransactionSuccess="failed"` (čisto aditivno).
- **Procjena rada:** 0,5–1 dan + regresija.
- **Način testiranja:** TestShell granula s transakcijom koja namjerno puca (RAISERROR u SP); provjeriti log, `transaction` varijablu u XML `if`-u i da rollback ostaje ispravan.
- **Mjerljivi kriterijum uspjeha:** 100% neuspjelih transakcija završava u logu s exception detaljima; `LastTransactionSuccess` pouzdano razlikuje uspjeh/neuspjeh.
- **Zavisnosti od drugih promjena:** nema.

---

### N-02 — Transakcija otvorena preko modalnih dijaloga

- **ID:** N-02
- **Naziv:** `SqlTransaction` obuhvata cijeli `RunActions` uključujući UI
- **Kategorija:** SQL / pouzdanost / konkurentnost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno (struktura koda); trajanje lockova — potrebno mjerenje
- **Projekat i fajl:** Carpo.UI — `Carpo.UI\Logic\Events.cs`
- **Klasa/metoda:** `Events.CheckTransaction` → `RunActions`
- **Dokaz iz koda:** 1496–1508: `BeginTransaction` → `RunActions(dataActionsList)`; `RunActions` (270–479) unutar iste petlje poziva `CheckConfirm` (648–699, `JConfirm.Confirm` = modalni dijalog), `CheckMsgBox`, `WaitFormRun` sa `Thread.Sleep` (201).
- **Problem:** XML može (i po komentarima u kodu, već koristi) kombinovati `confirm`/`message` akcije s `datarun` akcijama unutar `transaction` bloka. SQL lockovi i otvorena konekcija drže se neograničeno dugo dok korisnik ne klikne.
- **Mogući produkcijski efekat:** blokade drugih korisnika na istim tabelama, lock escalation, timeouti kod drugih; „aplikacija se smrzla svima kad je X otvorio dijalog“.
- **Predložena promjena:** kratkoročno: u `CheckTransaction` prije `BeginTransaction` provjeriti listu akcija i **logovati upozorenje** ako sadrži confirm/message tipove (dijagnostika bez promjene ponašanja). Srednjoročno: pravilo da se `confirm` akcije izvršavaju PRIJE otvaranja transakcije (validirati/izvući ne-data akcije ispred transakcionog bloka), dokumentovati za XML autore.
- **Rizik promjene:** srednji — promjena redoslijeda akcija je semantička promjena za postojeće XML-ove; zahtijeva inventuru granula (grep po `<action type="transaction"` u XML skladištu, koje nije u repou).
- **Procjena rada:** dijagnostika 0,5 dana; preuređenje semantike 3–5 dana + regresija po granulama.
- **Način testiranja:** SQL Server: `sys.dm_tran_locks`/`sp_whoisactive` dok je confirm dijalog otvoren unutar transakcije u TestShell scenariju.
- **Mjerljivi kriterijum uspjeha:** nijedna transakcija ne traje > X sekundi (baseline mjerenjem); broj blocking-chain incidenata.
- **Zavisnosti:** N-01 (prvo logging da se vidi stvarna upotreba).

---

### N-03 — Kredencijali u logu i kodu

- **ID:** N-03
- **Naziv:** plaintext lozinka u logu; hardkodirana app-role lozinka
- **Kategorija:** Sigurnost
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Connector\Abstract\Starter.cs`; Carpo.Data — `SqlHelper.cs`, `Connection.cs`
- **Klasa/metoda:** `Starter.RegisterForm`; `AppRole.ExecuteEnableAppRole`; `Connection`
- **Dokaz iz koda:** `Starter.cs` 295–299: `form.Logger.Info("Info RunParameters", "server: ... ; password: " + password + ...)`. `SqlHelper.cs` 2538–2539: `AddWithValue("@password", "Passw0rd!")`. `Connection.cs` 17–20: `[ThreadStatic] pwd` + javni property `Pwd`.
- **Problem:** SQL lozinka operatera završava u log fajlu na disku; app-role kredencijal je u source controlu (metoda trenutno nije pozvana iz živog koda — pozivi su zakomentarisani, ali kod je javan i kompajlira se).
- **Mogući produkcijski efekat:** kompromitacija pristupa bazi kroz log fajlove / repo.
- **Predložena promjena:** ukloniti `password` iz log poruke (ostaviti server/db/username); obrisati `AppRole` klasu ili prebaciti kredencijal u konfiguraciju; dodatno — `Log.CommandAsSql` serializuje **vrijednosti svih parametara** u log (`Common.cs` 90), pa parametri tipa lozinka/OIB završavaju u logu: uvesti maskiranje po imenu parametra (npr. `*pass*`, `*pwd*`).
- **Rizik promjene:** zanemariv (logging only).
- **Procjena rada:** < 0,5 dana.
- **Način testiranja:** pregled log outputa nakon starta granule i par akcija; grep loga na poznatu test lozinku.
- **Mjerljivi kriterijum uspjeha:** 0 pojavljivanja kredencijala u log fajlovima.
- **Zavisnosti:** nema.

---

### N-04 — Sinhroni SQL na UI threadu + lažni async u gridu

- **ID:** N-04
- **Naziv:** `GetDataAsync` ne offloaduje SQL; sav data pristup blokira UI
- **Kategorija:** UI responsiveness
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno (blokada); trajanje po granuli — potrebno mjerenje
- **Projekat i fajl:** Carpo.UI — `Components\Base\ComponentGrid.cs` (i analogno `ComponentTree.cs`, `Manager.cs`, `JupiterEditor.cs`)
- **Klasa/metoda:** `ComponentGrid.GetDataAsync`, `Populate`, `SetData`
- **Dokaz iz koda:** 1241–1252: `DataSet ds = await Task.FromResult<DataSet>(dataDefinition.GetDataSet(...))` — `GetDataSet` se izvršava sinhrono PRIJE `Task.FromResult`; `await` ne radi ništa. Poziv je `_ = GetDataAsync();` (fire-and-forget, 923) pa se i exceptioni gube. `PopulateTreeList` (Tree 511–639) i Manager ctor/Load/FormClosing rade isti sinhroni I/O.
- **Problem:** svako punjenje grida/stabla/forme zamrzava UI za puno trajanje SQL-a + bind; discarded task guta greške punjenja.
- **Mogući produkcijski efekat:** „bijela forma“ / not responding na sporijim upitima; izgubljene greške punjenja.
- **Predložena promjena:** korak 1 (siguran, bez threadinga): vratiti pravu sinhronost — ukloniti fake-async omotač i `_ =` discard, čime se greške ponovo propagiraju, a `WaitForm` mehanizam ostaje. Korak 2 (uz pažljivu analizu WinForms konteksta): stvarni `Task.Run` oko čistog SQL dijela uz marshaling `SetData` na UI thread — **ali** tek nakon rješavanja ThreadStatic `Connection` (N-13), jer background thread ne vidi connection string. Ne uvoditi paralelni SQL preko iste konekcije.
- **Rizik promjene:** korak 1 nizak; korak 2 srednji-visok (sync context, ThreadStatic, reentrancy grida).
- **Procjena rada:** korak 1: 1 dan; korak 2: 1–2 sedmice pilot na jednoj formi.
- **Način testiranja:** mjerenje UI freeze intervala (vidi Prilog A) prije/poslije na 3 reprezentativne granule; provjera da greška SQL-a pri punjenju sada prikazuje poruku.
- **Mjerljivi kriterijum uspjeha:** nijedan izgubljeni exception pri punjenju; (korak 2) UI freeze pri punjenju < 200 ms uz isti ukupni elapsed.
- **Zavisnosti:** N-13 za korak 2.

---

### N-05 — `SET ARITHABORT ON` roundtrip + bezuslovni `CommandAsSql` na svakom SQL pozivu

- **ID:** N-05
- **Naziv:** dva fiksna poreza po svakom SQL pozivu
- **Kategorija:** SQL performanse / CPU
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno (kod); ušteda — potrebno mjerenje
- **Projekat i fajl:** Carpo.Data — `Common.cs`, `Log.cs`
- **Klasa/metoda:** `Common.ExecuteQuery` / `ExecuteAzur`; `Log.CommandAsSql`
- **Dokaz iz koda:** `Common.cs` 51–54 i 166–169: `new SqlCommand("SET ARITHABORT ON", connection).ExecuteNonQuery()` prije svake komande (89 call siteova `ExecuteQuery` + 19 `ExecuteNonQuery` repo-wide). `Common.cs` 90/183/229/264: `sqlText = Log.CommandAsSql(command)` uvijek. `Log.cs` 82–96: `Regex.Replace(sql, @"\s+", " ")` inline (novi regex po pozivu) + `sql +=` konkatenacija u petlji po parametrima + serializacija svih vrijednosti.
- **Problem:** +1 mrežni roundtrip po svakom SQL pozivu; CPU + alokacije za generisanje log stringa i kad se ne loguje; osjetljive vrijednosti parametara u stringu (veza s N-03).
- **Mogući produkcijski efekat:** na akciji s 20 SQL poziva — 20 nepotrebnih roundtripova; primjetno na WAN/VPN linkovima.
- **Predložena promjena:** (a) `CommandAsSql` učiniti lijenim — generisati samo kad je odgovarajući log level uključen (potpis promijeniti u `Func<string>` ili provjeru levela prije generisanja); regex zamijeniti statičkim `static readonly Regex` s `RegexOptions.Compiled`, konkatenaciju `StringBuilder`-om. (b) Za ARITHABORT: **ne** uklanjati naslijepo (SP-ovi s indeksiranim view/computed kolonama ga zahtijevaju); opcije: postaviti na nivou servera (`sp_configure 'user options'`) ili baze i ukloniti klijentski poziv, ili za `CommandType.Text` prependati `SET ARITHABORT ON;` u isti batch. Odluku donijeti s DBA — do tada barem izbjeći poziv kad se ista pooled konekcija ponovo koristi nije pouzdano izvodivo, pa je serversko rješenje ispravno.
- **Rizik promjene:** (a) zanemariv; (b) srednji — zahtijeva DBA koordinaciju i regresiju upita.
- **Procjena rada:** (a) 0,5–1 dan; (b) 1–2 dana s testiranjem.
- **Način testiranja:** SQL Profiler/Extended Events: broj batchova po korisničkoj akciji prije/poslije; PerfView alokacije `Log.CommandAsSql`.
- **Mjerljivi kriterijum uspjeha:** broj SQL batchova po akciji smanjen za ~50% (svaki poziv gubi ARITHABORT batch); 0 alokacija CommandAsSql kad je log isključen.
- **Zavisnosti:** nema.

---

### N-06 — `FormatRules` se akumuliraju pri svakom Populate

- **ID:** N-06
- **Naziv:** curenje conditional-formatting pravila u gridu
- **Kategorija:** Performanse / memorija / degradacija tokom sesije
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Components\Base\ComponentGrid.cs`
- **Klasa/metoda:** `ComponentGrid.SetConditionalFormatting` (poziv iz `Populate`)
- **Dokaz iz koda:** 958 (poziv pri svakom Populate) i 4139–4168: `gridView1.FormatRules.Add(gridFormatRule)` bez `FormatRules.Clear()` ili guard flaga (za razliku od `CreateGroupSummary`, koji ima `groupSummaryCreated` guard na 3789).
- **Problem:** svaki refresh grida dodaje novi set pravila; poslije N refresh-eva grid evaluira N× više pravila po ćeliji pri renderu.
- **Mogući produkcijski efekat:** grid postaje progresivno sporiji što korisnik duže radi (klasičan „restartuj aplikaciju pa opet radi brzo“ simptom).
- **Predložena promjena:** `gridView1.FormatRules.Clear()` na početku metode ili guard flag kao kod group summaryja.
- **Rizik promjene:** vrlo nizak (pravila se ionako iznova grade iz iste definicije).
- **Procjena rada:** < 1 sat + test.
- **Način testiranja:** TestShell: granula s conditional formattingom, 20× refresh, pratiti `gridView1.FormatRules.Count` i vrijeme rendera.
- **Mjerljivi kriterijum uspjeha:** `FormatRules.Count` konstantan kroz refreshe.
- **Zavisnosti:** nema.

---

### N-07 — Populate pipeline: null DataSource, bez BeginUpdate, dupli BestFit, redundantni poslovi

- **ID:** N-07
- **Naziv:** neefikasan grid bind ciklus
- **Kategorija:** Performanse / UI
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno (kod); dobit — potrebno mjerenje
- **Projekat i fajl:** Carpo.UI — `Components\Base\ComponentGrid.cs`
- **Klasa/metoda:** `Populate` (878–979), `GetDataAsync` (1241–1252), `SetData` (1264–1345), `BestFitColumnsInit` (1508–1563), `TranslateGridColumns` (974–986)
- **Dokaz iz koda:** 897 `gridControl.DataSource = null;` prije svakog punjenja; `BeginUpdate/EndUpdate` zakomentarisani (1117/1136, 1245/1257); dupli `BestFitColumns` kad je `BestFitOnRefresh` + prvi init (1309–1333 i 1510–1516); `TranslateGridColumns` prevodi sve captione pri svakom Populate; `FocusedRowChanged(null)` nakon binda (939–940) okida XML notifikacije još jednom; `GetTemplateVisibleColumns` SQL pri svakom SetData (1318).
- **Problem:** višak layout/paint prolaza i fokus-event kaskada pri svakom refreshu; template SQL po bindu.
- **Mogući produkcijski efekat:** sporiji refresh grida, treperenje, dupli detail reload kod master-detail formi.
- **Predložena promjena:** (1) `gridView1.BeginDataUpdate()`/`EndDataUpdate()` oko dodjele DataSource i column build; (2) jedan BestFit po logičkom refreshu (objediniti uslove); (3) translate captiona samo pri (re)kreiranju kolona, ne pri svakom bindu; (4) keširati template visible-columns po (template, korisnik) uz invalidaciju na promjenu templatea.
- **Rizik promjene:** srednji — grid ima puno spregnutih ponašanja (fokus, selekcija, grupisanje); mijenjati u malim PR-ovima uz vizuelnu regresiju.
- **Procjena rada:** 2–4 dana kroz 2–3 PR-a.
- **Način testiranja:** mjerenje `Populate`→prvi paint (Stopwatch + `Logger.InfoTest` postoji), vizuelna regresija na 5 najkorištenijih granula (fokus, selekcija, grupisanje, sume, layout restore).
- **Mjerljivi kriterijum uspjeha:** vrijeme refresh operacije −20% na baseline granulama; bez regresije fokusa/selekcije.
- **Zavisnosti:** izmjeriti baseline prije (Faza 0).

---

### N-08 — N+1 obrasci: foreachrow po redu grida i XmlGen po parent redu

- **ID:** N-08
- **Naziv:** SQL/akcije u petlji po redovima
- **Kategorija:** SQL performanse
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno (obrazac); frekvencija — visoka vjerovatnoća, zavisi od XML-a
- **Projekat i fajl:** Carpo.UI — `ComponentGrid.cs`; Carpo.XmlGen — `Data\GetDataSet.cs`; Carpo.Core — `Log.cs`
- **Klasa/metoda:** `ComponentGrid.SetNotifyForEachRow` (2536–2540) → `RowForEachRowAction` (2737–2743: `ev.RunActions(action.Actions)` po redu); `GetDataSet.GoThroughParent` (102–105: `AddDataTable`→`ExecuteQuery` po parent redu); `Log` skript petlja (837–856: `ExecuteDatasetTypedParams` po redu).
- **Problem:** za grid od 1.000 redova `foreachrow` s `datarun` akcijom = 1.000 SQL poziva × (ARITHABORT + CommandAsSql + konekcija iz poola). XmlGen dokument s M parent redova = M upita.
- **Mogući produkcijski efekat:** masovne akcije (npr. „obradi označene“) traju minutama; generisanje dokumenata sporo linearno s brojem stavki.
- **Predložena promjena:** kratkoročno mjerenje: logovati broj SQL poziva po `foreachrow` akciji (brojač u `Events`). Srednjoročno: set-based varijanta — postojeći `tablepar` mehanizam (`Core.ParseTableParameters`, 365–390) već podržava slanje DataTable parametra; dokumentovati i preferirati „jedan SP s TVP“ umjesto foreachrow za nove granule; za XmlGen razmotriti jedan upit s JOIN-om po definiciji.
- **Rizik promjene:** promjena XML konvencije — nizak za nove granule, visok za retrofit starih; zato retrofit samo za dokazano spore granule.
- **Procjena rada:** brojač 0,5 dana; pilot set-based konverzija jedne spore granule 2–3 dana.
- **Način testiranja:** Extended Events broj RPC-eva po akciji prije/poslije.
- **Mjerljivi kriterijum uspjeha:** broj SQL poziva po masovnoj akciji sa O(n) na O(1)–O(k).
- **Zavisnosti:** Faza 0 instrumentacija.

---

### N-09 — Event handler leakovi + `GC.Collect()` pri svakom zatvaranju forme

- **ID:** N-09
- **Naziv:** lifecycle kontrola i formi
- **Kategorija:** Memorija / resursi
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno (obrazac); stvarno zadržavanje formi — hipoteza, potrebno mjerenje heap dumpom
- **Projekat i fajl:** Carpo.UI — `ComponentGrid.cs`, `ComponentBase.cs`, `ComponentTree.cs`, `ControlInit.cs`, `Manager.cs`, `Forms\Base\JupitetShared.cs`
- **Klasa/metoda:** `ComponentGrid.SetEvents` (285–552: 33× `+=`, 3 stvarna `-=`); `ComponentBase` (~25× `+=`, 0 `-=`); `ControlInit.SetTabEvents` (448–458, lambda nikad otkačena); `JupiterShared_FormClosing` (604–608): `Dispose(); GC.Collect();`
- **Dokaz iz koda:** navedene linije; dodatno Ctrl+F12 ručni `GC.Collect()` (324).
- **Problem:** handleri su lambde koje hvataju `this`; dokle god publisher živi, drži pretplatnike. `GC.Collect()` u FormClosing je simptomatska terapija koja uvodi blokirajući full GC (gen2 + LOH) na UI threadu pri svakom zatvaranju forme — na x86 procesu s velikim DataTable-ovima to je mjerljiv freeze.
- **Mogući produkcijski efekat:** kratko smrzavanje pri svakom zatvaranju forme; potencijalno zadržane forme/kontrole (treba potvrditi dumpom).
- **Predložena promjena:** (1) izmjeriti: broj živih `JupiterShared`/`GridControl` instanci nakon 20 open/close ciklusa (sos `!dumpheap -type`); (2) ako nema leaka — ukloniti `GC.Collect()` iz FormClosing (zadržati Ctrl+F12 debug prečicu); (3) ako ima — dodati `Dispose` cleanup u `ComponentGrid`/`ComponentBase` (otkačiti view handlere u postojećem `GridControl_Disposed`, 196–207, koji već postoji za timer).
- **Rizik promjene:** uklanjanje GC.Collect nizak uz prethodno mjerenje; dodavanje unsubscribe logike srednji (redoslijed dispose-a).
- **Procjena rada:** mjerenje 1 dan; cleanup 2–3 dana.
- **Način testiranja:** heap dump prije/poslije 20 ciklusa; mjerenje vremena zatvaranja forme.
- **Mjerljivi kriterijum uspjeha:** 0 zadržanih formi nakon ciklusa; vrijeme zatvaranja forme bez GC spike-a.
- **Zavisnosti:** Faza 0 (brojanje živih formi).

---

### N-10 — Fiskalizacija i Outlook hardkodirani u `RunActions`, sinhroni, bez oslobađanja COM-a

- **ID:** N-10
- **Naziv:** vanjske integracije u generičkom event pipelineu
- **Kategorija:** Arhitektura / UI responsiveness / pouzdanost
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Logic\Events.cs`, `Helpers\MicrosoftAPI.cs`, `Helpers\Fiscalization.cs`, `Logic\ComDll.cs`
- **Klasa/metoda:** `Events.RunActions` grane `"outlook"` (278–295), `"ZKI"` (297–344), `"JIR"` (347–410); `MicrosoftAPI.AddContactToOutlook` (18–74); `ComDll.TotalGen` (105–119)
- **Dokaz iz koda:** inline SQL `select * from mpfiskalizacija where partneriid=(select varset from pubvars ...)` (303, 353); `Task.Run(() => web.SendSoapRequest(...)).GetAwaiter().GetResult()` (388); stanje između akcija kroz instance field `zki` (269); JIR grana: `X509Certificate2` bez `using` (371); Outlook: `new Outlook.Application()` + folder + items bez ijednog `Marshal.ReleaseComObject`; `ComDll.TotalGen` COM instanca bez release-a.
- **Problem:** (a) UI thread blokiran mrežnim SOAP pozivom prema poreznoj upravi (timeout 30 s) — deadlock rizika nema jer se koristi `Task.Run`, ali freeze je pun; (b) nema retry/queue za fiskalizaciju — pad mreže = exception kroz generički handler; (c) COM objekti Outlook-a ostaju do GC-a (drži `OUTLOOK.EXE` proces); (d) domain logika (fiskalizacija!) živi u generičkom framework fajlu i izvršava se kroz `if (action.Type == "ZKI")` prije svih standardnih provjera.
- **Mogući produkcijski efekat:** kasa blokirana 30 s pri problemu s mrežom; „duh“ OUTLOOK.EXE procesi; nemogućnost testiranja fiskalizacije izolovano.
- **Predložena promjena:** izdvojiti u `IFiscalizationService` (`CalcZki`, `RequestJir`) i `IOutlookContactService` u zasebne klase (isti assembly, bez novih slojeva): `Events` grane postaju 3-linijski dispatch. U Outlook helperu dodati `finally { Marshal.ReleaseComObject(...) }` lanac. U JIR grani `using var cert`. Retry/backoff za SOAP tek nakon dogovora s poslovnom stranom (fiskalni rokovi).
- **Rizik promjene:** nizak za ekstrakciju (mehanički move + delegacija); srednji za retry semantiku.
- **Procjena rada:** ekstrakcija + COM cleanup 2–3 dana; retry politika posebno.
- **Način testiranja:** TestShell scenarij sa ZKI/JIR protiv test servisa porezne; provjera da OUTLOOK.EXE nestaje nakon akcije; postojeći tok bajt-za-bajt isti XML ugovor.
- **Mjerljivi kriterijum uspjeha:** identičan fiskalni rezultat na test servisu; 0 zaostalih OUTLOOK procesa.
- **Zavisnosti:** nema (čista ekstrakcija).

---

### N-11 — SQL injection u Carpo.Specific

- **ID:** N-11
- **Naziv:** konkatenacija korisničkog unosa u SQL tekst
- **Kategorija:** Sigurnost / SQL
- **Prioritet:** P0 (sigurnost), P2 (performanse — plan cache)
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.Specific — `UgovoriONajmu\PartneriPopUp.cs`, `UgovoriDetail.cs`, `Ugovori.cs`
- **Klasa/metoda:** `PartneriPopUp` (109–110, 119–124), `UgovoriDetail` (1315–1317, 1360, 1375, 1666–1667, 1702, 1739), analogno u `Ugovori.cs`
- **Dokaz iz koda:** `"exec spTraziPartneri @Action='Partneri', @broj_lokacija=1, @Uvjet='" + searchString + "'"`; ~25 mjesta `string sql = "..." + ...` (repo-wide pretraga), koncentrisano u UgovoriONajmu.
- **Problem:** `searchString` dolazi iz UI unosa; jedan apostrof ruši upit, zlonamjeran unos izvršava proizvoljan SQL pod pravima operatera. Usput: svaka varijanta stringa = novi plan u cacheu.
- **Mogući produkcijski efekat:** sigurnosni incident; sporadične greške na unosima s apostrofom (npr. prezime „O'Brien“) — vjerovatno već viđeno u produkciji.
- **Predložena promjena:** mehanička parametrizacija: `Common.ExecuteQuery` već prima `Dictionary<string, object> par` — prepisati call siteove na SP tip s parametrima (uzor: `Carpo.Specific\Data\Virmani.cs`, koji to već radi ispravno).
- **Rizik promjene:** nizak po pozivu (ista procedura, isti parametri), ali treba pokriti sve ~25+ lokacija.
- **Procjena rada:** 2–3 dana + ručna regresija UgovoriONajmu ekrana.
- **Način testiranja:** unosi s `'`, `--`, `%`; uporediti rezultate pretrage prije/poslije na istim podacima.
- **Mjerljivi kriterijum uspjeha:** 0 konkateniranih SQL stringova s korisničkim unosom (grep provjera); pretraga radi s apostrofima.
- **Zavisnosti:** nema.

---

### N-12 — Parser/Evaluate bez cachea + regex u hot pathu + evaluacija uslova u paint eventima

- **ID:** N-12
- **Naziv:** CPU trošak expression enginea
- **Kategorija:** Performanse / CPU
- **Prioritet:** P2 (dok mjerenje ne pokaže drugačije)
- **Pouzdanost:** potvrđeno (obrazac); udio u CPU — hipoteza, potrebno profilisanje
- **Projekat i fajl:** Carpo.UI — `Logic\Evaluate.cs`, `Logic\Parser.cs`, `Logic\Statemants.cs`, `Components\Base\ComponentGrid.cs`
- **Klasa/metoda:** `Evaluate.GetSubStrings` (185: `new Regex(start + "(.*?)" + end)` po pozivu, neescapovan i nekompajliran); `Parser.GetLogicPartValue` (63–193: split/alokacije po pozivu, bez cachea rezultata parsiranja strukture izraza); `Parser.GetDirectByPart` (398: inline `Regex.IsMatch(str, "^(@|#)")` — zamjenjivo sa `str.Length>0 && (str[0]=='@'||str[0]=='#')`); `ComponentGrid` `RowCellStyle`/`CustomDrawCell`/`SetStylesHelper` (593–670) pozivaju `Conditions.CheckCondition`→`Statemants.CheckIf`→2× `new Parser` po uslovu **po iscrtanoj ćeliji**; `Statemants.GetValueByObject` (162) pokušava decimal parse svake vrijednosti; `Evaluate` decimal konverzija kroz try/catch (71–78 — exception u normalnom toku).
- **Problem:** isti stringovi iz XML-a parsiraju se hiljadama puta; paint path množi trošak s brojem vidljivih ćelija × repaint frekvencijom (scroll!).
- **Mogući produkcijski efekat:** trom scroll na gridovima sa stilovima/uslovima; visok CPU pri renderu.
- **Predložena promjena:** (1) `static readonly Regex` s `Compiled` u `Evaluate` (pattern je uvijek `{`/`}` — konstantan); (2) zamijeniti inline `Regex.IsMatch` provjerom prvog znaka; (3) **siguran cache parsiranih izraza**: keš strukture (tokenizacije) izraza po stringu — ne keš vrijednosti (vrijednosti zavise od stanja forme). Ključ: string izraza; vrijednost: immutable token lista; `ConcurrentDictionary` s gornjom granicom (npr. 10k unosa; izrazi dolaze iz konačnog skupa XML definicija pa rast nije problem, ali granicu ipak postaviti). Invalidacija nije potrebna jer je mapiranje string→tokeni čisto.
- **Rizik promjene:** (1)(2) zanemariv; (3) srednji — zahtijeva razdvajanje „parsiranje strukture“ od „evaluacija vrijednosti“ u `Parser`, raditi tek nakon profilisanja koje potvrdi da je hot.
- **Procjena rada:** (1)(2) 0,5 dana; (3) 3–5 dana.
- **Način testiranja:** PerfView/VS CPU profiler: udio `Parser.GetLogicPartValue` + `Regex` tokom scrolla grida sa stilovima; prije/poslije.
- **Mjerljivi kriterijum uspjeha:** CPU udio expression enginea u scroll scenariju mjerljivo smanjen (cilj definisati nakon baseline-a).
- **Zavisnosti:** Faza 0 profilisanje prije koraka (3).

---

### N-13 — ThreadStatic globalno stanje (konekcija, sesija) i statički mutable objekti

- **ID:** N-13
- **Naziv:** globalno stanje blokira svaki pokušaj offloadinga
- **Kategorija:** Arhitektura / konkurentnost
- **Prioritet:** P2 (postaje P1 čim se krene u async)
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.Data — `Connection.cs`, `Common.cs`; Carpo.Core — `PubVars`; Carpo.UI — `Starter.cs`; Carpo.Structure — `SpecificElementList`
- **Klasa/metoda:** `Connection` (7 ThreadStatic polja); `Common.OutParams` (linija 15: `private static List<OutParModel> OutParams` — **dijeljen između svih poziva**, overload bez out parametra piše u njega); `SpecificElementList.DatalistModelList` (append-only pri svakom deserialize/Retry — `Serializer.cs` 462–465); `PubVars`, `Starter.ImeFirme`.
- **Dokaz iz koda:** navedene linije.
- **Problem:** (a) svaki kod na background threadu vidi `Connection.ConnectionString == null` — skriveni razlog zašto je „sve sinhrono“; (b) `Common.OutParams` je race i logička greška: dva uzastopna poziva dijele isti statički objekt; (c) `DatalistModelList` je memory rast kroz F5/Retry cikluse.
- **Mogući produkcijski efekat:** (b) pogrešne vrijednosti output parametara pri preklapanju poziva (npr. timer refresh + korisnička akcija); (c) spor rast memorije u dugim sesijama.
- **Predložena promjena:** (b) učiniti lokalnim: overloadi 17–34 neka proslijede lokalnu listu (out discard), obrisati statičko polje — mala, sigurna promjena; (c) `DatalistModelList.Clear()` na početku deserializacije (kao što `CustomControlsElement` već radi — `Serializer.cs` 513–517); (a) dugoročno: `ConnectionString` prosljeđivati eksplicitno (forma ga već ima — `form.ConnectionString`); ThreadStatic zadržati kao fallback radi Jupiter kompatibilnosti, NE mijenjati semantiku sesije bez analize Jupiter lifecycle-a (više granula u istom procesu/threadu).
- **Rizik promjene:** (b)(c) nizak; (a) srednji — dokumentovan oprez oko Jupiter sesija.
- **Procjena rada:** (b)(c) 0,5 dana; (a) inkrementalno kroz Fazu 2.
- **Način testiranja:** (b) unit test dva uzastopna poziva s različitim out parametrima; (c) memorija nakon 50× F5 na granuli s datalistom.
- **Mjerljivi kriterijum uspjeha:** stabilna memorija kroz Retry cikluse; deterministički out parametri.
- **Zavisnosti:** (a) je preduslov za N-04 korak 2.

---

### N-14 — Build sistem: DLL reference iz Program Files, output u Program Files, ručno verzionisanje

- **ID:** N-14
- **Naziv:** nereproducibilan build i stale-DLL rizik
- **Kategorija:** Build / DevEx
- **Prioritet:** P1 (za razvojni tim)
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** svi csproj-evi; `Carpo.sln`; `upute za distribuciju.txt`
- **Klasa/metoda:** n/a
- **Dokaz iz koda:** `Carpo.UI.csproj` 89–97: `<HintPath>..\..\..\..\..\Program Files (x86)\Jupiter Software\...\Carpo.Common.dll` (reference na **binarne** artefakte prethodnog builda umjesto ProjectReference; sln koristi `ProjectDependencies` samo za redoslijed); OutputPath direktno u `Program Files ...\SharedAssemblies\Spin` (više varijanti relativnih puteva, 41/51/63); linija 111: `C:\Users\valentino.jagodic\Desktop\DevExpress.Sparkline.v23.1.Core.dll` (i linija 140 `...\Desktop\DevExpress.XtraSpreadsheet.v23.1.dll`); `Carpo.Common.csproj` 129: referenca na `Microsoft.QualityTools.Testing.Fakes.dll`; `upute za distribuciju.txt`: ručna izmjena „treće cifre“ verzije i odkomentarisanje koda pri distribuciji.
- **Problem:** build nije reproducibilan i **niko ne garantuje redoslijed**: ako se Carpo.UI kompajlira prije nego što je novi Carpo.Common iskopiran u `Program Files`, linkuje se protiv **stare** verzije. Hardkodiran desktop put pada na svakoj drugoj mašini. Output u `Program Files` zahtijeva elevated build i miješa build artefakte s deploymentom. Ručno verzionisanje je izvor grešaka.
- **Mogući produkcijski efekat:** „radi na mojoj mašini“, teško reproducibilni bugovi zbog mješavine verzija DLL-ova, nemoguć čist CI build bez ručnih koraka.
- **Predložena promjena:** (1) zamijeniti inter-Carpo `<Reference><HintPath>` sa `<ProjectReference>` (počevši od jednog para, npr. Carpo.UI→Carpo.Common — Carpo.UI→Carpo.Data već je ProjectReference, dokaz da radi); (2) prebaciti DevExpress/DMS reference na `$(DevExpressBinPath)` MSBuild property ili NuGet, ukloniti desktop put; (3) OutputPath u lokalni `bin\$(Platform)\$(Configuration)`, a kopiranje u SharedAssemblies izdvojiti u zaseban deploy korak (postoji `bin\x86\Release` varijanta u nekim konfiguracijama — standardizovati); (4) verziju centralizovati (`Directory.Build.props` ili shared `AssemblyInfo` s MSBuild varijablom). Sve inkrementalno, projekt po projekt.
- **Rizik promjene:** srednji — TFVC/Jupiter deployment očekuje DLL-ove na tačnoj lokaciji; svaki korak validirati punim rebuildom i smoke testom u Jupiteru. Ne dirati redoslijed platformi (x86 ostaje).
- **Procjena rada:** ProjectReference migracija 2–3 dana; DevExpress path + verzija 1–2 dana; deploy korak 1–2 dana.
- **Način testiranja:** clean checkout na drugoj mašini → `msbuild Carpo.sln /p:Configuration=Release;Platform=x86` bez ručnih koraka → deploy → smoke test 3 granule u Jupiteru.
- **Mjerljivi kriterijum uspjeha:** clean build s praznog workspacea prolazi bez ručne intervencije; nula HintPath referenci na `Program Files`/desktop među Carpo projektima.
- **Zavisnosti od drugih promjena:** nema; preduslov za pouzdan CI (Faza 0).

---

### N-15 — Sinhroni SQL/disk I/O u Manager/JupiterEditor lifecycle + FormClosing early-return

- **ID:** N-15
- **Naziv:** blokirajući I/O u ctor/Load/FormClosing i preskočeno snimanje layouta
- **Kategorija:** UI responsiveness / pouzdanost (gubitak postavki)
- **Prioritet:** P1 (responsiveness), P2 (early-return bug)
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Forms\Manager\Manager.cs`, `Forms\Base\JupiterEditor.cs`
- **Klasa/metoda:** `Manager` ctor (258–280), `Manager_FormClosing` (1220–1334), `JupiterEditor.BeforeInit`/`SetControlAppValues`/`OnFormClosing`
- **Dokaz iz koda:** ctor: `SettingsService.LoadSettings` (`spCarpoFilterPostavke`) + `MergeFilterLayoutsFromAppDataFileIntoSettings` (disk) — sve prije prikaza forme; `Manager_FormClosing` 1241–1244: u filter modu `return` nakon snimanja non-filter stream-a → **preskače** master/detail tab layoute (1247–1314), splittere (1316–1320) i `SetAppControlValues` (1325–1326); 1225–1228: `SaveSettings` — jedan SQL poziv po ključu (N round-tripova).
- **Problem:** otvaranje/zatvaranje Manager forme blokira UI za trajanje više SQL poziva + disk I/O; u filter modu se dio korisnikovih layout postavki nikad ne snimi.
- **Mogući produkcijski efekat:** „štuca“ pri otvaranju/zatvaranju granula; korisnik gubi raspored kolona/splittera ako je zatvorio u filter modu.
- **Predložena promjena:** (a) early-return bug: restrukturirati `Manager_FormClosing` da se zajednički save izvrši u `finally`/prije return-a bez obzira na mod (mala, sigurna izmjena); (b) batch: `SaveSettings` sve ključeve u jednom SQL pozivu (TVP ili jedan multi-statement) umjesto N; (c) responsiveness: nakon N-13, razmotriti pozadinsko čitanje filter postavki dok se forma inicijalizuje (uz `WaitForm`).
- **Rizik promjene:** (a) nizak; (b) nizak-srednji (promjena SP potpisa/poziva); (c) srednji.
- **Procjena rada:** (a) 0,5 dana; (b) 1 dan; (c) uz N-13.
- **Način testiranja:** otvoriti/zatvoriti Manager u filter i non-filter modu; provjeriti da su svi layouti (master, detail, splitter, filter combo) snimljeni u oba slučaja; mjeriti vrijeme otvaranja/zatvaranja.
- **Mjerljivi kriterijum uspjeha:** 0 izgubljenih layout postavki neovisno o modu; broj SQL poziva na close smanjen s N na 1–2.
- **Zavisnosti od drugih promjena:** N-13 za (c).

---

### N-16 — `SqlContext.CreateCommand` vraća disposan objekat; in-tran komande i reader bez Dispose; AddWithValue bez tipova

- **ID:** N-16
- **Naziv:** resource i tip-safety problemi u data sloju
- **Kategorija:** Pouzdanost / SQL
- **Prioritet:** P2
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.Data — `SqlContext.cs`, `Common.cs`, `Set\Vars.cs` i ostali AddWithValue call-siteovi
- **Klasa/metoda:** `SqlContext.CreateCommand` (128–141), `ExecuteReader` (33–51), `Common.ExecuteQueryInTran`/`ExecuteAzurInTran` (212/249)
- **Dokaz iz koda:** `SqlContext.CreateCommand` — `using var command = new SqlCommand(...); ... return command;` (objekat se disposuje na kraju metode PA se vraća — pozivalac dobija disposan command; radi „slučajno“ jer ga pozivalac odmah koristi u `using` prije nego GC finalizuje, ali je bug); `ExecuteReader` dict overload (41): reader bez `using`; `ExecuteQueryInTran`/`ExecuteAzurInTran`: `SqlCommand` bez `using`; 118× `AddWithValue` repo-wide (najviše `CMS\Data\Elements.cs`, `Data\Set\Vars.cs`).
- **Problem:** disposan command je latentna bomba (radi dok se ponašanje SqlClient-a ne promijeni); reader/command bez `using` u transakcijama drži resurse; `AddWithValue` prepušta tip inference-u (varchar↔nvarchar → implicitne konverzije, index scan umjesto seek; različiti tipovi → plan cache pollution).
- **Mogući produkcijski efekat:** sporadični „command disposed“ ili resource curenje pod greškom; suboptimalni planovi za parametrizovane upite.
- **Predložena promjena:** `CreateCommand` — ukloniti `using` (pozivalac već koristi `using`); dodati `using` na reader i in-tran komande; postupno zamijeniti `AddWithValue` sa `Add(name, SqlDbType).Value = ...` na najfrekventnijim upitima (ne sve odjednom — prioritet po pozivima/danu).
- **Rizik promjene:** `CreateCommand`/`using` fixevi nizak; AddWithValue srednji (tip mora tačno odgovarati koloni — inače regresija).
- **Procjena rada:** Dispose fixevi 0,5 dana; AddWithValue postupno (per-modul).
- **Način testiranja:** postojeći tokovi kroz `SqlContext` (ZKI/JIR/Outlook/translator) i transakcije; SQL Profiler za tip parametara na top upitima.
- **Mjerljivi kriterijum uspjeha:** 0 disposanih vraćenih komandi; korektni parametarski tipovi na ciljanim upitima.
- **Zavisnosti od drugih promjena:** nema.

---

### N-17 — Kontrola toka kroz exception poruke i ~41 prazan catch

- **ID:** N-17
- **Naziv:** greške kao control flow + gušenje izuzetaka
- **Kategorija:** Pouzdanost / održavanje
- **Prioritet:** P2
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Carpo.UI — `Logic\Data\Core.cs`, `Forms\Jupiter\Base.cs`, i repo-wide
- **Klasa/metoda:** `Core.SetData` (176–239), `Jupiter\Base` (181–191), razni `catch { throw; }`
- **Dokaz iz koda:** `Core.SetData` parsira `SqlException.Message` na sufikse `+close`/`+ok`/`+focus` (183–226) i time upravlja UI-jem — normalan poslovni tok se vozi kroz RAISERROR poruke i `ex.Number >= 50000`; `Jupiter\Base` 181–191: `catch (Exception es) { }` pa `p.Kill()` u `catch {}`; ~41 praznih `catch {}` (najviše TestShell 7, Core\Log 6, JupiterEditor 3); mnogo `catch { throw; }` (no-op koji samo gubi originalni stack u nekim varijantama).
- **Problem:** poslovna pravila kodirana u tekstu poruka su krhka (promjena teksta SP-a mijenja UI ponašanje), skupa (exception throw/catch u normalnom toku) i teško testabilna; prazni catch-evi skrivaju greške.
- **Mogući produkcijski efekat:** tiho progutane greške; nepredvidivo UI ponašanje na promjenu SP poruka; teška dijagnostika.
- **Predložena promjena:** kratkoročno — sve prazne `catch {}` barem logovati (`Logger.Warn/Error`); `catch { throw; }` ukloniti (nepotrebni) ili zamijeniti stvarnim handlingom. Dugoročno — standardizovati signalizaciju iz SP-a kroz strukturirani output parametar / return code umjesto teksta poruke (zahtijeva koordinaciju s bazom, dokumentovati kao poseban track). Ne mijenjati `+close/+ok/+focus` semantiku dok se ne inventarišu SP-ovi koji je koriste.
- **Rizik promjene:** logging u catch-evima nizak; promjena SP protokola visok (širok, van repoa).
- **Procjena rada:** logging pass 1–2 dana; SP protokol — poseban dugoročni projekat.
- **Način testiranja:** namjerno izazvati greške na ključnim tokovima i provjeriti da su u logu.
- **Mjerljivi kriterijum uspjeha:** 0 praznih catch blokova bez loga; dijagnostika greške moguća iz loga.
- **Zavisnosti od drugih promjena:** N-01 (isti duh — ne gutati greške).

---

## 4. QUICK WINS

Male, niskorizične, mjerljive promjene.

### < 1 dan
- **QW-1 (N-03):** ukloniti lozinku iz `Starter.RegisterForm` log poruke + maskirati parametre s imenom `*pass*/*pwd*` u `Log.CommandAsSql`. Rezultat: 0 kredencijala u logu.
- **QW-2 (N-06):** `gridView1.FormatRules.Clear()` na početku `SetConditionalFormatting`. Rezultat: konstantan `FormatRules.Count` kroz refreshe.
- **QW-3 (N-16):** ukloniti `using` iz `SqlContext.CreateCommand`; dodati `using` na reader i in-tran komande. Rezultat: bez disposanih/procurjelih komandi.
- **QW-4 (N-12 dio):** `static readonly Regex ... RegexOptions.Compiled` u `Evaluate.GetSubStrings`; zamijeniti `Regex.IsMatch(str,"^(@|#)")` u `Parser.GetDirectByPart` provjerom prvog znaka. Rezultat: eliminisana per-poziv regex alokacija.
- **QW-5 (N-13 dio):** `SpecificElementList.DatalistModelList.Clear()` na početku deserializacije; učiniti `Common.OutParams` lokalnom varijablom. Rezultat: stabilna memorija kroz Retry, deterministički out parametri.
- **QW-6 (N-04 dio):** ukloniti fake-async omotač i `_ = GetDataAsync()` discard u gridu → vratiti propagaciju grešaka pri punjenju. Rezultat: nema izgubljenih exceptiona.

### 1–2 dana
- **QW-7 (N-01):** logging + `LastTransactionSuccess="failed"` u `CheckTransaction` catch (bez mijenjanja UI ponašanja). Rezultat: 100% neuspjelih transakcija u logu.
- **QW-8 (N-15a):** popraviti `Manager_FormClosing` early-return da uvijek snimi sve layoute. Rezultat: 0 izgubljenih postavki.
- **QW-9 (N-05a):** lijeno generisanje `CommandAsSql` samo kad je log level uključen. Rezultat: 0 alokacija kad je log off.
- **QW-10 (N-17 dio):** logovati sve prazne `catch {}` u Carpo.UI/Carpo.Data. Rezultat: vidljivost grešaka.

### Do 1 sedmice
- **QW-11 (N-10 dio):** dodati `Marshal.ReleaseComObject` lanac + `using var cert` u Outlook/JIR putanjama. Rezultat: 0 zaostalih OUTLOOK.EXE procesa.
- **QW-12 (N-14 dio):** migrirati jedan par inter-Carpo referenci na `ProjectReference` + ukloniti desktop DevExpress put. Rezultat: korak ka reproducibilnom buildu.
- **QW-13 (N-11 dio):** parametrizovati `PartneriPopUp` SQL (najizloženiji korisničkom unosu). Rezultat: zatvorena najveća injection rupa.

---

## 5. PERFORMANCE BACKLOG

Sortirano po odnosu **korist / (rizik × rad)** — najbolji odnos prvo.

| # | Stavka | Nalaz | Korist | Rizik | Rad | Napomena |
|---|--------|-------|--------|-------|-----|----------|
| 1 | FormatRules.Clear | N-06 | Visoka (uklanja progresivnu degradaciju) | Vrlo nizak | XS | Odmah |
| 2 | Lijeni CommandAsSql + ARITHABORT serverski | N-05 | Visoka (−~50% batcheva/akciji) | Nizak / srednji (b) | S | (b) uz DBA |
| 3 | Regex/parse micro-optim | N-12 (1,2) | Srednja (scroll) | Vrlo nizak | XS | Odmah |
| 4 | Grid Populate: BeginDataUpdate + 1 BestFit + translate on-create | N-07 | Visoka (refresh −20%) | Srednji | M | Mjeriti prije |
| 5 | Ukloniti GC.Collect iz FormClosing | N-09 | Srednja (freeze na close) | Nizak (uz mjerenje) | S | Prvo heap dump |
| 6 | foreachrow → set-based (pilot) | N-08 | Visoka za spore akcije | Srednji | M–L | Po granuli |
| 7 | Definition cache po granuli / izbjeći dvostruki deserialize | N-14(XML)/B-nalaz | Srednja (startup) | Srednji | M | Mjeriti deserialize ms |
| 8 | Expression tokenizacijski cache | N-12 (3) | Srednja–visoka (CPU) | Srednji | L | Tek nakon profilisanja |
| 9 | Pozadinski SQL za punjenje (pilot 1 forma) | N-04 (korak 2) | Visoka (UI freeze) | Visok | L | Nakon N-13 |

---

## 6. REFACTORING ROADMAP

### Faza 0 — Baseline i instrumentacija (preduslov za sve ostalo)
- **Preduslovi:** dogovoren set od 3–5 reprezentativnih granula (jedna Manager master-detail, jedna teška edit forma, jedna s foreachrow masovnom akcijom, jedna fiskalna); pristup test bazi.
- **Radnje:** iskoristiti postojeći `Logger.InfoTest(opis, startTime)` (već instrumentiran u `Starter`, `SetCaption`, `CreateEvents`) i proširiti ga na `Populate`/`SetData`/`RunActions`; SQL Profiler/Extended Events sesija za broj i trajanje batcheva po akciji; PerfView za alokacije i GC; brojač živih formi (heap dump). Definisati format loga: `KATEGORIJA | granula | operacija | ms | brojSQL`.
- **Očekivani rezultat:** brojčani baseline za svaku metriku iz Priloga A.
- **Rollback:** instrumentacija je aditivna (logging) — trivijalan.

### Faza 1 — Siguran lokalni refaktoring (bez promjene ponašanja)
- **Preduslovi:** Faza 0.
- **Sadržaj:** svi Quick Winovi QW-1..QW-10 (logging, Clear, Dispose fixevi, regex, static state čišćenje, early-return fix, lijeni log).
- **Očekivani rezultat:** uklonjena progresivna degradacija (FormatRules), sigurnosne rupe u logu, resource leakovi; bez vidljive promjene za korisnika osim stabilnijeg ponašanja.
- **Rollback:** svaka promjena je mali izolovani PR — revert pojedinačno.

### Faza 2 — Izdvajanje odgovornosti (bez novih slojeva bez koristi)
- **Preduslovi:** Faza 1, karakterizacioni testovi (Prilog B).
- **Sadržaj:** izdvojiti fiskalizaciju (`IFiscalizationService`) i Outlook (`IOutlookContactService`) iz `Events.RunActions` (N-10); parametrizovati `Carpo.Specific` SQL (N-11); učiniti `Connection` string eksplicitnim gdje je moguće (N-13a); build higijena (N-14).
- **Očekivani rezultat:** `Events` grane postaju tanke; zatvorena injection površina; reproducibilan build.
- **Rollback:** ekstrakcije su mehaničke uz očuvan javni ugovor — revert po PR-u.

### Faza 3 — Optimizacija hot pathova
- **Preduslovi:** Faza 0 baseline + profil koji potvrđuje hotspot.
- **Sadržaj:** grid Populate ciklus (N-07), ARITHABORT/CommandAsSql (N-05), expression tokenizacijski cache (N-12.3), foreachrow set-based pilot (N-08), uklanjanje GC.Collect (N-09).
- **Očekivani rezultat:** mjerljivo brži refresh/scroll, manje SQL batcheva, manje freeze-a.
- **Rollback:** feature-flag ili po-granuli uvođenje; revert PR-a; baseline za poređenje.

### Faza 4 — Dugoročna arhitektura
- **Preduslovi:** Faze 0–3, stabilan test net.
- **Sadržaj:** pozadinski data-access uz rješen sync context (N-04 korak 2); razmotriti DevExpress `ServerMode`/`AsyncServerMode` **samo** za dokazano velike gridove i tek nakon provjere feature kompatibilnosti (grupisanje, sume, custom sort, conditional formatting — vidi §8); SP signalizacija bez tekstualnih sufiksa (N-17); jedinstveni layout persistence servis (Manager ima 3 sistema).
- **Očekivani rezultat:** UI ne blokira na SQL-u; skalabilni gridovi; testabilan data protokol.
- **Rollback:** svaka stavka pilot na jednoj granuli prije šireg uvođenja.

> **Napomena o platformi (odvojeni dugoročni roadmap, NE zamjena za gornje):** eventualna migracija na noviji .NET / moderni UI stack može se razmatrati isključivo kao zaseban, dugoročan projekat nakon što je sistem instrumentiran i pokriven testovima. Ništa u ovom auditu ne zavisi od takve migracije i ona se ne preporučuje kao odgovor na trenutne probleme.

---

## 7. PRIJEDLOG PRVIH PET PR-OVA

Svaki PR je mali i nezavisno reviewable.

### PR-1 — Sigurnost logova i kredencijali (N-03)
- **Cilj:** ukloniti kredencijale iz logova i koda.
- **Zahvaćeni fajlovi:** `Carpo.UI\Connector\Abstract\Starter.cs` (295–299), `Carpo.Data\Log.cs` (maskiranje parametara), `Carpo.Data\SqlHelper.cs` (ukloniti/izolovati `AppRole` hardkodiranu lozinku).
- **Opseg:** ~20 linija.
- **Rizik:** zanemariv (samo logging/dead code).
- **Test plan:** pokrenuti granulu, pregledati log fajl, grep na test lozinku i OIB.
- **Očekivani rezultat:** 0 kredencijala/osjetljivih parametara u logu.

### PR-2 — Grid FormatRules leak (N-06)
- **Cilj:** zaustaviti akumulaciju conditional-formatting pravila.
- **Zahvaćeni fajlovi:** `Carpo.UI\Components\Base\ComponentGrid.cs` (`SetConditionalFormatting`, ~4139).
- **Opseg:** 1–3 linije + test.
- **Rizik:** vrlo nizak.
- **Test plan:** TestShell granula sa conditional formattingom; 20× refresh; pratiti `FormatRules.Count` i vizuelnu ispravnost.
- **Očekivani rezultat:** konstantan broj pravila; identičan izgled.

### PR-3 — Data-sloj Dispose i static state (N-16, N-13 dio)
- **Cilj:** ispraviti disposan command, dodati `using`, ukloniti dijeljeni static `OutParams`, očistiti `DatalistModelList`.
- **Zahvaćeni fajlovi:** `Carpo.Data\Data\Context\SqlContext.cs`, `Carpo.Data\Common.cs`, `Carpo.UI\Logic\Serializer.cs`.
- **Opseg:** ~30 linija.
- **Rizik:** nizak.
- **Test plan:** tokovi kroz `SqlContext` (ZKI/JIR/Outlook/translator) + transakcija; dva uzastopna poziva s različitim out parametrima.
- **Očekivani rezultat:** bez resource leaka; deterministički out parametri; stabilna memorija kroz Retry.

### PR-4 — Tihi rollback transakcije: logging (N-01, aditivno)
- **Cilj:** učiniti neuspjele transakcije vidljivim bez mijenjanja UI ponašanja.
- **Zahvaćeni fajlovi:** `Carpo.UI\Logic\Events.cs` (`CheckTransaction` catch, 1511–1519).
- **Opseg:** ~5 linija.
- **Rizik:** nizak (samo logging + `LastTransactionSuccess="failed"`).
- **Test plan:** TestShell transakcija koja puca (RAISERROR); provjeriti log i `transaction` varijablu u XML `if`-u; rollback i dalje ispravan.
- **Očekivani rezultat:** 100% neuspjelih transakcija u logu s exception detaljima.

### PR-5 — Regex/parse micro-optimizacije + fake-async cleanup (N-12 dio, N-04 dio)
- **Cilj:** eliminisati per-poziv regex alokacije i vratiti propagaciju grešaka pri punjenju grida.
- **Zahvaćeni fajlovi:** `Carpo.UI\Logic\Evaluate.cs` (185), `Carpo.UI\Logic\Parser.cs` (398), `Carpo.UI\Components\Base\ComponentGrid.cs` (`GetDataAsync`, `Populate` discard).
- **Opseg:** ~20 linija.
- **Rizik:** nizak (ponašanje isto; greške sada vidljive).
- **Test plan:** punjenje grida s namjernom SQL greškom (sada prikazuje poruku); PerfView provjera nestanka regex alokacija tokom scrolla.
- **Očekivani rezultat:** bez izgubljenih exceptiona; manje alokacija.

---

## 8. ŠTA NE TREBA DIRATI (bez jakog razloga / mjerenja)

- **`SqlHelper.cs` (MS Data Access Application Block).** Standardni, stabilan, dobro poznat kod; `ParamCache` je sinhronizovan. Ne prepisivati; eventualne izmjene samo ciljano (npr. timeout parametar). Rizik/korist nepovoljan.
- **DevExpress `ServerMode` kao globalni default.** Ne uvoditi paušalno — `ComponentGrid` oslanja se na client-side grupisanje, weighted custom summary, custom sort i conditional formatting; `ServerMode` mijenja semantiku svih tih feature-a i traži da SP podrži paging/sort na serveru. Razmotriti isključivo per-grid za dokazano velike setove, uz feature-compat provjeru (Faza 4).
- **ThreadStatic `Connection` semantika sesije.** Ne mijenjati model sesije bez analize Jupiter lifecycle-a (više granula u istom procesu/threadu). Izmjene ograničiti na eksplicitno prosljeđivanje connection stringa tamo gdje forma već ima referencu (N-13a) — bez ukidanja ThreadStatic fallbacka.
- **Paralelni SQL preko iste konekcije.** `SqlConnection`/`SqlTransaction` nisu thread-safe; ne uvoditi paralelizam nad dijeljenom konekcijom. Offload (N-04 korak 2) samo s konekcijom po background operaciji i marshalingom rezultata na UI.
- **`+close/+ok/+focus` protokol poruka.** Krhko, ali široko korišteno kroz produkcijske SP-ove (van repoa). Ne dirati dok se ne napravi inventar SP-ova; promjena je poseban dugoročni track (N-17).
- **`Ugovori`/`UgovoriDetail` strukturni fork.** Iako dupliraju logiku, to su stabilne klijentske forme; objedinjavanje u zajedničku baznu klasu nosi visok rizik regresije za nisku korist. Prioritet je samo parametrizacija SQL-a (sigurnost), ne restrukturiranje.
- **Potpuni rewrite bilo kojeg god-fajla (`ComponentGrid`, `Events`).** Previše implicitnih ponašanja bez testova; raditi isključivo inkrementalnim, mjerljivim izmjenama.

---

## 9. OTVORENA PITANJA (nedostajuće informacije)

Da bi preporuke bile pouzdanije, potrebno je:

1. **Produkcijske metrike** — trenutna vremena otvaranja granula, refresh grida, UI freeze intervali; nemamo baseline (Faza 0 ih proizvodi).
2. **Spore granule** — koje konkretne granule korisnici prijavljuju kao spore? To usmjerava N-04/N-07/N-08 na prave mete.
3. **SQL execution planovi i IO** — za najfrekventnije SP-ove; definicije SP-ova **nisu u repou**. Bez njih ne možemo tvrditi da je usko grlo klijent ili baza.
4. **Tipične veličine DataTable/Grid** — koliko redova vraćaju glavni upiti? Odlučuje da li je `ServerMode`/paging opravdan (N-07/Faza 4).
5. **Verzija Jupiter hosta** — da li host drži `Carpo.dll` u toplom AppDomain-u (amortizuje XmlSerializer/JIT) ili ga reloaduje po granuli? Utiče na vrijednost definition cachea. `upute za distribuciju.txt` spominju Jupiter 8 i 10 — koje su u produkciji?
6. **Deployment i rollback procedura** — kako se DLL-ovi trenutno distribuiraju u `SharedAssemblies` i kako se radi rollback? Preduslov za bezbjednu build reorganizaciju (N-14).
7. **Broj granula i njihovi XML-ovi** — koliko ih ima i koliko su veliki (za procjenu deserialize troška i N-08 opsega).
8. **Da li se `foreachrow` masovne akcije koriste na velikim gridovima** — i koje; ključno za prioritet N-08.
9. **Fiskalni SLA** — dozvoljeno vrijeme/retry politika za JIR (N-10); poslovno ograničenje.
10. **Postoji li ikakav CI danas** (TFS build definicija)? Određuje polaznu tačku za N-14.

---

## 10. KONAČNA PRIORITIZOVANA LISTA (rangirano)

| # | Aktivnost | Nalaz | Korist | Rizik | Rad | Treba profilisanje? | Preporučeni redoslijed |
|---|-----------|-------|--------|-------|-----|:---:|---|
| 1 | Ukloniti kredencijale iz loga + maskirati parametre | N-03 | Velika | Mali | S | Ne | 1 (odmah, PR-1) |
| 2 | `FormatRules.Clear()` u gridu | N-06 | Velika | Mali | S | Ne | 2 (PR-2) |
| 3 | Tihi rollback → logging + `LastTransactionSuccess` | N-01 | Velika | Mali | S | Ne | 3 (PR-4) |
| 4 | SqlContext/InTran Dispose + static state čišćenje | N-16, N-13 | Srednja | Mali | S | Ne | 4 (PR-3) |
| 5 | Regex/parse micro-optim + fake-async cleanup | N-12, N-04 | Srednja | Mali | S | Ne | 5 (PR-5) |
| 6 | Manager FormClosing early-return fix | N-15 | Srednja | Mali | S | Ne | 6 |
| 7 | Lijeni CommandAsSql (log-gated) | N-05 | Srednja | Mali | S | Ne | 7 |
| 8 | Prazni catch → logging | N-17 | Srednja | Mali | M | Ne | 8 |
| 9 | Outlook/JIR COM release + `using cert` | N-10 | Srednja | Mali | M | Ne | 9 |
| 10 | Parametrizovati Carpo.Specific SQL (injection) | N-11 | Velika (sigurnost) | Srednji | M | Ne | 10 |
| 11 | **Faza 0 baseline instrumentacija** | — | Velika (omogućuje sve dalje) | Mali | M | n/a | 11 (paralelno od početka) |
| 12 | ProjectReference migracija + build higijena | N-14 | Velika (DevEx/CI) | Srednji | L | Ne | 12 |
| 13 | Ukloniti `GC.Collect()` iz FormClosing | N-09 | Srednja | Mali | S | **Da** (heap dump prvo) | 13 |
| 14 | Grid Populate ciklus (BeginDataUpdate/1×BestFit/translate) | N-07 | Velika | Srednji | M | **Da** | 14 |
| 15 | ARITHABORT serverski (uz DBA) | N-05b | Srednja | Srednji | S | **Da** | 15 |
| 16 | Izdvojiti fiskalizaciju/Outlook iz Events | N-10 | Srednja (arh./test) | Nizak | M | Ne | 16 |
| 17 | foreachrow → set-based (pilot spore granule) | N-08 | Velika (za te akcije) | Srednji | L | **Da** | 17 |
| 18 | Definition cache / izbjeći dvostruki deserialize | N-14(XML) | Srednja | Srednji | M | **Da** | 18 |
| 19 | Expression tokenizacijski cache | N-12.3 | Srednja | Srednji | L | **Da** | 19 |
| 20 | Pozadinski data-access (pilot 1 forma) | N-04 korak 2, N-13a | Velika | Veliki | XL | **Da** | 20 (posljednje, Faza 4) |

---

## PRILOG A — Plan mjerenja i profilisanja (Faza 0, prije refaktoringa)

Svi alati kompatibilni s .NET Framework 4.8 / WinForms / x86. Zajednički format loga (koristiti postojeći `Logger`): `PERF | <granula> | <operacija> | <ms> | <brojSQL> | <dodatno>`. Svako mjerenje ponoviti 3× (uzeti medijan), cold i warm start odvojeno.

| Metrika | Gdje postaviti mjerenje | Alat / instrumentacija | Test scenario | Poređenje prije/poslije |
|---|---|---|---|---|
| `Starter.Run` ukupno | Početak/kraj `Run` (`Starter.cs` 78/138) — `Logger.InfoTest` već postoji za dijelove | `Stopwatch` + postojeći `InfoTest` | Otvaranje svake od 3–5 referentnih granula iz Jupitera i iz TestShell-a | Isti XML, ista baza, medijan od 3 pokretanja |
| XML učitavanje + deserializacija | `Serializer.DeserializeXml` (oko `XmlSerializer.Deserialize` poziva) | `Stopwatch`; PerfView za alokacije | Iste granule; zabilježiti veličinu XML-a (KB) uz vrijeme | ms/KB po granuli |
| Otvaranje granule (do prvog painta) | `Starter.Run` start → `form.Shown` event | `Stopwatch` u `RegisterForm`/`Shown` | Referentne granule, cold (prvi put u sesiji) i warm | Medijan cold i warm odvojeno |
| Inicijalni grid binding | `ComponentGrid.Populate` start → kraj `SetData` | `Stopwatch` + `InfoTest` | Master grid najveće granule | ms + broj redova (normalizovati ms/1000 redova) |
| Grid refresh | Isto mjesto, okidano refresh akcijom | `Stopwatch` | 20 uzastopnih refresh-eva (otkriva i N-06 degradaciju) | Prvi vs. dvadeseti refresh + medijan |
| Broj SQL poziva po akciji | SQL Server strana | Extended Events (`rpc_completed`, `sql_batch_completed`, filter po host/login) | Snimiti tipičnu akciju: otvori granulu, refresh, spremi dokument, masovna foreachrow akcija | Broj eventova i suma `duration` po akciji |
| Ukupno SQL trajanje po akciji | Isto | Isto (suma `duration`) | Isto | Suma ms po akciji |
| UI freeze intervali | Message pump | Timer koji mjeri kašnjenje `Application.Idle` / jednostavan watchdog thread koji `Invoke`-uje marker i mjeri latenciju (>200 ms = freeze); alternativno PerfView `Thread Time` | Otvaranje granule, refresh, spremanje s transakcijom, JIR | Broj i trajanje freeze-ova > 200 ms |
| Private bytes / managed heap | Proces | Performance Monitor (`Private Bytes`, `# Bytes in all Heaps` za proces), PerfView snapshot | 2h simulirane sesije: 30× otvori/zatvori granulu + refreshi | Kriva rasta; ravna = OK |
| Gen 0/1/2 GC broj | Proces | PerfMon `.NET CLR Memory` counteri | Ista sesija; posebno pratiti Gen2 skokove pri zatvaranju forme (N-09) | Gen2/sat prije/poslije uklanjanja `GC.Collect()` |
| Žive forme/kontrole poslije zatvaranja | Heap | PerfView heap snapshot ili WinDbg+SOS `!dumpheap -type JupiterShared` / `-type GridControl` | 20× otvori/zatvori istu granulu, pa full GC (Ctrl+F12 postoji), pa snapshot | Broj instanci = 0 ili 1 očekivano |
| Otvorene SQL konekcije | SQL Server | `sys.dm_exec_sessions` po host/login; PerfMon `NumberOfPooledConnections` | Tokom sesije i s otvorenim confirm dijalogom u transakciji (N-02) | Broj i trajanje otvorenih sesija |
| Cold vs. warm start | Kao „otvaranje granule“ | Isto | Prvi start nakon reboot/iisreset Jupitera vs. ponovno otvaranje | Odvojene kolone u logu |

**CPU profilisanje (za N-12):** PerfView (CPU sampling, radi na 4.8/x86) ili Visual Studio Performance Profiler; scenario: scroll grida sa stilovima/uslovima 30 s; gledati inkluzivni udio `Parser.GetLogicPartValue`, `Evaluate.*`, `System.Text.RegularExpressions.*`.

---

## PRILOG B — Plan testiranja (minimalni safety net)

**Odmah automatizovano (bez Jupiter okruženja):**

1. **Unit testovi čiste logike** — `Parser`, `Evaluate`, `Statemants`, `Conversion`, `Log.CommandAsSql` (maskiranje): čisti string-in/string-out, bez baze. Ovo je i characterization: snimiti sadašnje ponašanje (uključujući rubne slučajeve poput `&&`+`||` miješanja u `Statemants`) prije bilo kakvog diranja parsera.
2. **Testovi XML deserializacije** — komplet postojećih granula XML-ova (koliko ih je dostupno) kroz `Serializer.DeserializeXml`: ne smije baciti exception, ključna polja `JRoot` grafa stabilna (snapshot poređenje). Štiti N-14(XML)/PR-ove koji diraju Serializer.
3. **Integration testovi SQL wrappera** — protiv test baze (LocalDB/test server): `Common.ExecuteQuery/ExecuteAzur/ExecuteAzurInTran` happy path, RAISERROR putanja (`+close` sufiks!), out parametri (štiti N-13 `OutParams` fix), timeout. Pokreće se iz MSTest/NUnit na 4.8, x86 test runner.
4. **Characterization test za `CheckTransaction`** — kroz TestShell-sličan harness: transakcija koja uspijeva / puca; assert na `LastTransactionSuccess` i (poslije PR-4) log zapis.

**Zahtijeva Jupiter okruženje / ručno:**

5. **Smoke scenariji kroz `Carpo.TestShell`** — skripta od ~10 koraka po referentnoj granuli: otvori, popuni grid, refresh, filtriraj, otvori detail, spremi (transakcija), zatvori. Izvršiti prije/poslije svakog PR-a iz §7.
6. **Ručna regresiona checklista za Jupiter host** — granula se otvara u Jupiteru (SetParent!), permisije rade, layout persistencija (posebno filter mod — N-15), prijevodi, print. x86 build obavezan.
7. **DevExpress grid/tree ponašanje** — fokus/selekcija/grupisanje/sume/conditional formatting/BestFit/export u Excel nakon svakog PR-a koji dira `ComponentGrid` (PR-2, N-07).
8. **COM/Outlook i fiskalizacija** — samo uz N-10 promjene: test Outlook profil (kontakt se kreira, OUTLOOK.EXE se gasi), fiskalni test servis porezne uprave (ZKI/JIR identičan rezultat prije/poslije ekstrakcije).

---

*Kraj izvještaja. Izvorni kod nije mijenjan; jedina izmjena u repozitoriju je kreiranje ovog dokumenta.*