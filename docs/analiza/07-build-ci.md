# 07 — Build i CI

**Iteracija 2.** Korisnik će donijeti DLL-ove. Dovoljan cilj builda je TestShell plus budući `Carpo.NetGen`, ne cijeli Designer/CMS/Lite. Točan popis datoteka je u `09-potrebni-dll-ovi.md`. Spajanje na bazu `spin` nije dio ovog builda; opcije su u dokumentu 08. Nacrt workflowa i dalje nije u `.github/workflows/`.

Čist clone na Linuxu se ne može prevesti: projekti su .NET Framework 4.8, a reference pokazuju na Jupiterov `SharedAssemblies` i na instalirani DevExpress. Ovaj dokument je popis onoga što Windows runner mora imati. Nacrt workflowa je `docs/analiza/draft-carpo-build.yml`. **Nije** u `.github/workflows/` i neće se sam pokrenuti.

Ništa od ključeva nije upisano u repo. Feed i lozinke ostaju secret.

## 1. Što solution očekuje

`Devex_26/Carpo.sln`, Visual Studio 17, konfiguracije Debug/Release × Any CPU, Mixed Platforms, x64, x86 (`Carpo.sln` 85–93).

Svi `*.csproj`: `TargetFrameworkVersion` v4.8. `LangVersion` je mješavina `7.3`, `latest` i `default`, ovisno o projektu. Nema `PackageReference`. `packages.config` postoji samo u:

- `Carpo.UI/packages.config` — `Microsoft.Office.Interop.Outlook` 15.0.4797.1004
- `Carpo.Specific/packages.config` — prazan `<packages>`

`nuget.config` u repou nema.

DevExpress nije NuGet u ovom stablu. Reference su jakog imena, bez `HintPath`, npr. `DevExpress.Data.v26.1, Version=26.1.3.0, PublicKeyToken=b88d1754d700e49a`. Isti obrazac na `XtraEditors`, `XtraGrid`, `XtraLayout`, `XtraBars`, `Utils`, i šire na UI (Charts, Pdf, RichEdit, Spreadsheet, BonusSkins). Analyzer u csproj pokazuje na

`$(ProgramFiles)\DevExpress 26.1\Components\Offline Packages\devexpress.data\26.1.3\analyzers\dotnet\DevExpress.Generator.dll`

(`Carpo.UI.csproj` oko 1050, slično u drugim projektima). To je putanja **instalera**, ne NuGet feeda. Dok se csproj ne prebaci na `PackageReference`, runner mora ili imati instaliran DevExpress 26.1.3, ili dobiti DLL-ove nekim drugim putem. Feed sam po sebi ne zadovoljava današnje `Reference Include` bez `HintPath`.

Službeni NuGet feed DevExpressa je oblika `https://nuget.devexpress.com/<ključ>/api/v3/index.json`. Ključ je licenca. U nacrtu workflowa čita se iz secreta `DEVEXPRESS_NUGET_KEY` i upisuje u **privremeni** `nuget.config` na runneru, koji se ne commita. Imena paketa (`DevExpress.Win` i slično) **nisu** u repou. Dok csproj gleda GAC/installer, `dotnet restore` tih paketa neće spojiti reference. Zato nacrt ne pretvara da `nuget restore` sam sagradi solution. Korak je pripremljen i označen, a build i dalje traži `CARPO_SPIN_DIR` i DX DLL-ove na disku.

## 2. `HintPath` koji nisu u gitu

Svi su ispod `Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\`. U csproj je to relativno (`..\..\..\..\..\Program Files (x86)\…`) ili apsolutno (`adodb`).

| DLL | Tko ga referencira |
|---|---|
| `Carpo.Structure.dll` | Common, Core, UI, Gen, XmlGen, Designer |
| `Carpo.Common.dll` | Data, Core, UI, Gen, XmlGen, Designer, CMS, Specific, TestShell |
| `Carpo.Data.dll` | Core, UI, Gen, XmlGen, Designer, CMS, Specific, TestShell |
| `Carpo.Core.dll` | UI, Gen, XmlGen, Designer, TestShell |
| `Carpo.UI.dll` | Gen, XmlGen, Designer, Lite, TestShell |
| `Carpo.dll` (to je `Carpo.Gen`) | Lite, TestShell |
| `Carpo.XmlGen.dll` | TestShell |
| `RibbonBase.v26.dll` | UI (alias `PresentationCore` u jednom retku — vidi csproj), Gen |
| `Spin.Printing.v26.dll` | UI |
| `SpinDMSUtil.dll` | UI, TestShell |
| `JupiterLogger.dll` | Core |
| `JupiterTranslator10.dll` | Common |
| `Dimenzije.dll` | Specific, HintPath s predubokim `..\` (više razina nego ostali) |
| `Microsoft.Office.Interop.Outlook.dll` | UI, iz Spin mape, ne iz NuGet cachea iako `packages.config` navodi paket |
| `adodb.dll` | UI, apsolutno `SharedAssemblies\Microsoft\adodb.dll` |

Jedina prava projektna referenca: `Carpo.UI` → `Carpo.Data` (`Carpo.UI.csproj` ~1036).

Posljedica: `Carpo.Common` ne može se prevesti dok netko ne stavi **već izgrađeni** `Carpo.Structure.dll` na HintPath, iako je izvor Structure u istom solutionu. To je kružno za CI. Redoslijed koji TFS danas živi je „prvo izgradi Structure u Spin mapu, pa Common, pa …“. `OutputPath` Debug|AnyCPU i jest ta Spin mapa (npr. `Carpo.UI.csproj` Debug). Na runneru pisanje u `C:\Program Files (x86)\…` traži admin i dira tuđu instalaciju. Ne raditi to.

`Carpo.Specific` `Dimenzije` HintPath ima više `..\` nego ostali. Na developerovom disku slučajno pogodi; na runneru gotovo sigurno ne. **Neprovjereno** bez Windowsa, ali broj razina se vidi u csproj.

NetGen (`netgen/NetGenDLL.vbproj`) je zaseban solution. Njegov `HintPath` `DevExpress131\` (Janus 3.5, DX 13.1.7, `GridExPrinting` x86, `ILCalc`, `JupiterHelpClient`, `JupiterTranslator`, Newtonsoft 13.0.1) **nije u gitu** — u toj mapi nema nijedne datoteke. `Carpo.NetGen` te reference ne treba. Stari `NetGenDLL` se na ovom runneru također ne može sagraditi dok se `DevExpress131` ne donese. To nije blokada za Carpo, jest blokada ako netko poželi uspoređivati binarni izlaz.

## 3. Redoslijed koji bi CI morao poštovati

Iz `ProjectDependencies` u `Carpo.sln` i iz smjera HintPatha:

1. `Carpo.Structure` (nema Carpo HintPath)
2. `Carpo.Common` (treba Structure.dll + `JupiterTranslator10`)
3. `Carpo.Data` (treba Common.dll)
4. `Carpo.Core` (Common, Data, Structure, `JupiterLogger`)
5. `Carpo.UI` (Data kao projekt, plus Common, Core, Structure, Ribbon, Printing, SpinDMSUtil, adodb, Outlook, DX)
6. `Carpo.Gen`, `Carpo.XmlGen`, `Carpo.Specific`, `Carpo.Designer`, `Carpo.CMS`
7. `Carpo.TestShell`, `Carpo.Lite`

Vanjske komponente koje integration bilješka (`Devex_26/Devex26_Integration_TestEnv.md`) stavlja **prije** Carpa: JupiterLogger, RibbonBase, Spin.Printing. Njihov izvor nije u ovom repou.

`AssemblyVersion` je `10.5.26.2`. `Devex_26/upute za distribuciju.txt` kaže da se za Jupiter 8 treća znamenka spušta na 8, a za Jupiter 10 ostaje 10. CI ne smije sam mijenjati verziju.

Platforma: `PlatformTarget` u Debug|AnyCPU je AnyCPU, `Prefer32Bit` false. Solution konfiguracija `Mixed Platforms` mapira se na x86 **build red** u sln, ali csproj i dalje često kaže AnyCPU. Nacrt zato zove `Debug|Any CPU` eksplicitno, da se ne naslijedi Mixed.

## 4. Što nacrt workflowa radi, a što ne

Datoteka: `docs/analiza/draft-carpo-build.yml`.

Radi:

- `workflow_dispatch` samo (nema `push`/`pull_request`)
- `windows-latest`
- provjera da secret `DEVEXPRESS_NUGET_KEY` i varijabla okoline `CARPO_SPIN_DIR` postoje; ako ne, job padne s jasnom porukom i ništa ne kopira
- privremeni NuGet feed s ključem u `$RUNNER_TEMP`, ne u workspace koji se može slučajno committati
- MSBuild preko `microsoft/setup-msbuild`, framework 4.8 (runner ga ima kao Windows feature; ako fali targeting pack, korak to ispiše)
- `msbuild` po projektu redom iz odjeljka 3, uz `/p:OutputPath` u `artifacts\carpo\` i `/p:ReferencePath=$(CARPO_SPIN_DIR)` 

Ne radi, i to treba reći iskreno:

- `ReferencePath` **ne prepisuje** `HintPath`. Za HintPath koji je relativan na Program Files, MSBuild i dalje traži tu putanju. Nacrt zato ima korak koji, samo ako je `CARPO_SPIN_DIR` postavljen, napravi directory junction iz očekivane relativne putanje prema toj mapi. To je krhko (broj `..\` ovisi o tome gdje je repo na runneru: `D:\a\<org>\<repo>\Devex_26\Carpo.UI` → pet razina iznad nije korijen diska). Korak je u nacrtu označen `continue-on-error: false` i ispiše stvarnu putanju koju je izračunao, da se na prvom ručnom pokretanju vidi je li junction na pravom mjestu.
- Ne instalira DevExpress. Bez DLL-ova 26.1.3.0 u GAC-u ili u folderu koji reference vide, `Carpo.Structure` padne na prvom `using DevExpress`.
- Ne skida Jupiter DLL-ove. Njih mora dati artifact ili već montirana mapa. Pravni razlog: nisu NuGet, a ključ nije dozvola da se oni committaju.
- Ne uključuje NetGen `vbproj`.
- Ne pokreće TestShell (nema CLI, treba GUI i bazu).

Preporučeni sljedeći korak, izvan ove faze, kad korisnik da ključ i mapu: jednom ručno pokrenuti dispatch, popraviti junction prema ispisu, tek onda kopirati yml u `.github/workflows/`. Dok junction nije viđen na runneru, workflow ne treba uključivati.

## 5. Minimalni skup za prvi zeleni build `Carpo.Structure`

Čak i prvi projekt vuče DevExpress (oko 14 referenci, vidi `Carpo.Structure.csproj`). Dakle „samo Structure, bez Jupitera“ i dalje traži DX 26.1.3. Jupiter DLL-ovi počinju od `Carpo.Common` (`JupiterTranslator10`, `Carpo.Structure.dll` na HintPathu).

Za `Carpo.NetGen` kasnije treba još i izgrađeni `Carpo.Data` / `Carpo.Core`, što znači da cijeli lanac 1–4 mora proći. Nema prečice „novi projekt bez SharedAssemblies“ dok se HintPath ne zamijeni projektnim referencama. Ta zamjena je ispravna, ali mijenja csproj koje TFS danas kopira u Program Files. Nije dio faze 1 i ne treba je raditi usput.

## 6. Secreti (imena, ne vrijednosti)

| Ime | Čemu |
|---|---|
| `DEVEXPRESS_NUGET_KEY` | ključ u URL-u feeda. Ne logirati. Nacrt ga ne ispisuje |
| `CARPO_SPIN_DIR` | lokalna ili artifact mapa s `Carpo.*.dll`, `RibbonBase.v26.dll`, `Spin.Printing.v26.dll`, `SpinDMSUtil.dll`, `JupiterLogger.dll`, `JupiterTranslator10.dll`, `Dimenzije.dll`, `adodb.dll`, Outlook interop. Na runneru je to env, ne nužno GitHub secret, ako je mapa već na self-hosted stroju |

Self-hosted runner s instaliranim Jupiterom i DevExpressom 26.1.3 je manje krhak od `windows-latest` + junction. Nacrt cilja `windows-latest` jer je to ono što je traženo; ako prvi dispatch pokaže da junction ne drži, prebacivanje na self-hosted je manja izmjena od prepisivanja svih HintPatha.
