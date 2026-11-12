# NetGenDLL

VB.NET 4.8 **class library** (`OutputType=Library`, namespace `NetGenDLL`). Nije samostalna
aplikacija — DLL se hostira unutar **Jupiter Client** shell-a, koji forme iz ovog projekta
prikazuje kao child prozore preko Win32 API-ja (`SetParent`, `SetWindowPos`).

Ulazne točke koje Jupiter zove: **`Loader.vb`** (stvara `Form1`) i **`frm.vb`** (stvara `Form2`).
Obje imaju `Run` i `Run8`; `Run8` su novije i danas se koriste. Primaju `moduleId`,
`strNGParams`, `mainHandle`.

`ModuleID` → `Mid` = **ime XML-a granule** (npr. `KUP_Preglednik.xml`). Property usput
odsijeca parametre u zagradama. Puna putanja je `remotedllpath & "\SharedDLLs\xml\" & Mid`.

**AppDomain — ne dirati.** `Loader.Run` ([Loader.vb:86](Loader.vb#L86)) stvara zaseban AppDomain;
u `Loader.Run8` i `frm.Run8` je zakomentiran od 22.04.2024. Komentari u kodu objašnjavaju zašto
se to više puta uključivalo i isključivalo (sharing varijabli vs. fokus na blagajni vs. J10).
Radi — ne diraj.

## Status: održavanje — čitaj prije bilo kakve izmjene

NetGen je u produkciji **~20 godina** i radi na **velikom broju računala kod korisnika**.
Projekt je u **statusu održavanja — nema aktivnog razvoja**.

Izmjene se rade **minimalno, ciljano i oprezno**, tako da se ne pokida postojeća funkcionalnost.
Regresija ne pogađa jedan sustav nego sve instalacije na terenu, gdje je povrat skup.
Kod star 20 godina koji radi vrijedniji je od "čistijeg" koda koji možda ne radi.

Konkretno:

- Diraj **samo ono što je izričito traženo**. Bez usputnog čišćenja, refaktoriranja,
  preimenovanja ili "modernizacije".
- Stil koji izgleda zastarjelo je namjeran ili barem dokazano radi — ne ispravljaj ga.
  To uključuje `On Error`, `Option Strict Off` i zatečenih 9 build warninga.
- Puno logike je late-bound (`Assembly.Load`), pa kompajler **ne hvata greške** — izlaze
  tek u runtimeu kod korisnika. Uspješan build nije dokaz da je promjena sigurna.
- Kod dvojbe pitaj prije nego promijeniš, umjesto da pretpostaviš.

## Build i deploy

```
build.bat
```

Radi `MSBuild /t:Rebuild /p:Configuration=Debug`, pa kopira `bin\Debug\NetGenDLL.dll` na dvije lokacije:

- `C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\Spin`
- `..\Lanucher\Launcher\bin\Debug`

**`build.bat` deploya** — nije samo build. Za provjeru da se kod kompilira bez diranja instalacije:

```
& "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" NetGenDLL.vbproj /t:Build /p:Configuration=Debug /v:minimal
```

Skripta završava s `pause`; neinteraktivno pokretanje treba `cmd /c ".\build.bat < NUL"`.
Ispis u konzoli izgleda pokvareno (`USPJEĹ NO`) jer je `build.bat` UTF-8, a cmd ispisuje u OEM
code page — kozmetički, nije greška.

### Verzija — podigni kod svakog builda

Prije svakog builda podigni **zadnji broj** u `My Project/AssemblyInfo.vb`:

```
<Assembly: AssemblyVersion("10.1.0.17")>
<Assembly: AssemblyFileVersion("10.1.0.17")>
```

Oba atributa uvijek isto. Mijenja se **samo zadnja komponenta** (`10.1.0.X` → `10.1.0.X+1`);
prva tri broja ostaju `10.1.0`. Datoteka je UTF-8 s BOM-om — sačuvaj BOM pri uređivanju.

Očekivano stanje: **0 errors, 9 warnings**. Tih 9 su predegzistirajuća (`BC42349` DirectCast,
`BC42104`/`BC42105` u `frm.vb`, `MSB3270` arhitektura `GridExPrinting`). Ne "popravljaj" ih usput.

## Kodiranje datoteka — pazi

Sve `.vb` datoteke su **UTF-8 s BOM-om**. BOM je obavezan: bez njega VB kompajler i Visual Studio
padaju natrag na sistemsku ANSI code page (CP1250) i hrvatski znakovi se raspadnu.

Projekt je povijesno bio mješavina CP1250 i UTF-8. Ako naiđeš na `š` prikazan kao `?` ili `Å¡`,
datoteka je pročitana krivim kodiranjem — **ponovno je otvori ispravno prije uređivanja**, jer
spremanje u tom stanju trajno gubi znakove.

## Logiranje i greške

Tri sloja, svaki s različitom svrhom:

**1. `CBLog` — dijagnostika na licu mjesta.** Ring buffer od 40 stringova u instanci `Class1`,
puni se kroz `Class1.NapuniCBLog(msg)` (~355 poziva). Prikaz: **Ctrl+F11** → `Form5`.
Samo u memoriji — zatvoriš granulu, nestalo je. Nema levela, stacka ni trajanja.

**Pazi: CBLog sadrži lozinke.** `Form1.Run` upisuje password u čistom
tekstu, a `Conn.ConnectionString` (koji na SQL-auth grani sadrži `PWD=`) ide u log na 6+ mjesta.
Lokalno je to prihvatljivo, ali **ništa od toga ne smije u centralnu bazu bez redakta**.

**2. `spNetGenErrorLog` — mrtvo.** `ExecSql` je zove u Catch bloku, ali procedura je na
**svim** klijentskim bazama prazna (`create procedure ... as return`) i ne prima parametre
koje joj poziv šalje. Poziv ide kroz `ExecSql2`, čiji Catch je prazan — greška se tiho guta.
Logiranje je tako neaktivno godinama. Ostavljeno netaknuto radi backward compatibilityja.

Srodne napuštene tablice: `JupiterSystem.dbo.GreskeLog` i `dbo.ErrorLOG` na klijentskim
bazama (puni je `sp_SetErrorInfo` iz SQL procedura, ne NetGen). Ne slati novo tamo.

**3. `JupiterLogger` → `JupiterLogs.dbo.LogEvent` — aktivni sustav.** Vidi [JupiterLog.vb](JupiterLog.vb).

- Late-bound (`Assembly.Load("JupiterLogger")`), **bez reference u `.vbproj`** — namjerno:
  prava referenca bi značila da granula ne otvara ako DLL fali na klijentu.
- Ako DLL-a nema, `Dostupan` ostaje `False` i sve je tihi no-op.
- `Component = "NetGen"`, granula = `Mid`. `SetGranule` je ThreadStatic, pogađa samo thread granule.
- **Nikad ne zvati** `ConfigureSql`, `SetContext`, `Shutdown` — to radi Jupiter kao host.
  NetGen se samo priključi; zato lozinka logina `JupiterLogger` nije potrebna.
- Logira se **samo greške** (`ExecSql` Catch) + `Run` open/close. Ne dual-write svih 355
  CBLog poziva: postojeći log je 95 % INFO i 0,1 % ERROR, a veliki korisnici bi to osjetili.
- **`JupiterLog.Sredi()` radi redact prije svakog upisa** — `JupiterLogger.dll` sam NE radi
  nikakav redact (provjereno u assemblyju).

Cross-database put (klijentska baza → `JupiterLogs`) **ne radi**: klijenti se spajaju Windows
autentikacijom kao `db_owner`, a `JupiterLogs` ima samo SQL login `JupiterLogger` s `EXECUTE`
na procedure. Bez certifikata ili `TRUSTWORTHY` upis pada. Zato ide kroz DLL, ne kroz SQL.

### Provjera i isporuka

Radi od **10.1.0.20**, potvrđeno u pravom Jupiteru. Traži **`Jupiter.exe` 10.5.26.0 ili noviji** —
starija 10.5.0.37 nije radila `ConfigureSql`, pa `SqlEnabled` ostaje `False` i ništa se ne upisuje
(ni Jupiterovi vlastiti redovi).

Stanje se vidi u **Ctrl+F11**, lijevi stupac, red `JupiterLogger : ...` — pokazuje iz koje je
putanje DLL učitan i je li `SqlEnabled` `True`. Ta je dijagnostika ondje jer `CBLog` drži samo
40 linija i poruku iz `Run` istisne prije nego se prozor otvori.

**`JupiterClient.exe` sync prepisuje `NetGenDLL.dll`** verzijom sa servera
(`\spin-sql\JupiterSrv\Sync_8\`, ondje je još 8.9.0.357 iz 2022.). Za test pokreni `Jupiter.exe`
izravno; za isporuku korisnicima nova verzija mora u sync folder. `JupiterLogger.dll` sync sam
donosi u `SharedAssemblies\Spin`.

## Konvencije koda

- **`Option Strict Off`** (`Option Explicit On`). Late binding i implicitne konverzije su posvuda
  i kod na tome ovisi — ne uključuj Strict.
- **`On Error Resume Next` / `On Error GoTo` je dominantni stil** (~269 mjesta naspram ~43 `Try`).
  Klasični VB6-style error handling. Ne konvertiraj postojeći `On Error` u `Try/Catch` osim ako se
  to izričito traži — ranije je već bio slučaj da se `On Error Resume Next` morao vraćati natrag.
- Logiranje ide kroz `Class1.NapuniCBLog(msg)`. Instanca se obično zove `cls1`.
- **`NapuniCBLog` i `Translate` brišu `Err`.** Imaju `On Error`, a VB pri izlasku iz takve procedure
  očisti `Err`. `Err.Description` zato pročitaj (ili spremi u varijablu) **prije** njih, i to vrijedi
  i unutar istog izraza: `MsgBox(Translate("x") & Err.Description)` prikaže prazan opis.
  Isto vrijedi za `ExecSql`: nakon njegovog Catch bloka `Err.Number` je 0.
- Poruke i komentari su na hrvatskom. Piši ih na hrvatskom.
- Dosta funkcionalnosti se učitava **late-bound** preko `Assembly.Load` (`Spin.Printing.v23`,
  `PrintDlg2010`, `rptDevRepx`, fiskalizacija). Te reference ne postoje u projektu i ne mogu se
  provjeriti u build-u — greške izlaze tek u runtime-u, kroz `NapuniCBLog`.

## Struktura

`Form1.vb` (~8.7k linija) i `Form2.vb` (~9.8k) su glavni moduli i nose većinu logike.
`Class1.vb` (~2k) je zajednički helper: logiranje, SQL, ispis. Ostale forme su manje i specifične.

UI koristi **Janus GridEX v3** i **DevExpress v13.1** (stare verzije, u `DevExpress131\`).

## Pokretanje granule izvan Jupitera

### Launcher2 — preferirani način testiranja

**`..\Launcher2\bin\Debug\Launcher2.exe`** (od 15.10.2026.) je host kao `Jupiter.exe`: firme iz
`JupiterSystem.dbo.Firme`, granule iz `ssMeni`, tab XML folder, history, granula u svom tabu.
NetGenDLL učitava iz `SharedAssemblies\Spin`, kamo ga stavlja `build.bat`. Status bar pokazuje
učitanu verziju. Sve o njemu je u **`..\Launcher2\CLAUDE.md`**, uključujući odjeljak
„Test izmjene u NetGenu kroz Launcher2” (UI test skriptom, `_test\ui.ps1`).

Ukratko, za test izmjene:
- **Zatvori Launcher2 prije `build.bat`**, jer drži DLL zaključanim.
- `build.bat` pokreći iz **PowerShella**. Iz Git basha drugo kopiranje pukne na `š` u putanji.
- Za test bez diranja originala napravi kopiju XML-a pod novim imenom (`_TEST_...xml`) u
  `C:\Razmjena\NG\SharedDLLs\xml\`.

### Stari Launcher

Koristi `Launcher.exe` **iz `..\Lanucher\Launcher\bin\Debug\`** — ondje su sve ovisnosti
(DevExpress, Janus) koje `build.bat` deploya. Kopija istog exe-a u `C:\Razmjena\NG` je
bajt-identična, ali ondje se granula ne otvori.

| Polje | Vrijednost |
|---|---|
| Server / Baza | `jupiter2022` / `spin` |
| Putanja do XML | `C:\Razmjena\NG` — **bez** `\SharedDlls`; NetGen sam dodaje `\SharedDLLs\xml\` |
| XML | npr. `test.xml` (ima `ONLOAD="select * from ssoperateri"`) |
| radio | `loader` = `Loader.Run8` |

Za logiranje u JupiterLogs treba `JupiterLogger.dll` u istom folderu. Launcher ima
`PripremiJupiterLogger()` koji radi `Init` + `ConfigureSql` + `SetContext` umjesto Jupitera;
naslov prozora tada pokazuje `JupiterLogs SqlEnabled: True`. Debug: attach iz VS-a na `Launcher.exe`.

**Ne pisati vlastite PowerShell test-hostove** — vlastita `Form` plus `Application.Run()`
nakon `Run8` obara proces sa `StackOverflowException` (NetGen vodi svoj message loop).

## Testni SQL server

`jupiter2022` (SQL Server 2022), Windows auth, defaultna baza `Spin`:

```
sqlcmd -S jupiter2022 -E -d Spin -Q "SELECT ..." -h -1 -W
```

Korisne baze: `JupiterLogs` (LogEvent + `spLogInsert`/`spLogQuery`/`spLogErrors`),
`JupiterSystem`, `Jupiter10`, i ~50 klijentskih.

Na produkciji NetGen radi preko **Windows autentikacije**; korisnici su članovi AD grupa
i obično `db_owner` na klijentskoj bazi.

## Git

Grana je `master`. `.gitattributes` fiksira CRLF za izvorne datoteke.
`NetGenDLL.vbproj.user` i `.vs/` su lokalni — ne commitaj ih.
