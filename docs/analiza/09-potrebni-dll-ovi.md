# 09 — Potrebni DLL-ovi

Cilj koji je korisnik zadao: prevoditi **TestShell i budući `Carpo.NetGen`**, ne Designer, CMS, Lite ni stari `NetGenDLL`. Janus i `DevExpress131\` zato **nisu** na listi. Stari NetGen se ne gradi.

Sve putanje u csproj pokazuju na

`C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\`

ili na instalirani DevExpress 26.1.3 (GAC / `Program Files\DevExpress 26.1`, bez `HintPath`). `SpecificVersion` je `False` na Jupiter DLL-ovima. Broj verzije u `Include` (npr. `10.0.0.24` na TestShellu) nije obavezan: `AssemblyInfo` u izvoru kaže `10.5.26.2`. Donijeti DLL koji se danas stvarno vrti uz Jupiter 10.

Dovoljno je jedna mapa `Spin\` i jedna `Microsoft\` s ovim datotekama, plus DevExpress 26.1.3. Ne treba cijeli Jupiter installer ako su DLL-ovi skupljeni ručno, ali DX installer je jednostavniji od kopiranja desetaka jakih imena.

## 1. Minimalni skup za build TestShella

TestShell (`Devex_26/Carpo.TestShell/Carpo.TestShell.csproj`) vuče ove `HintPath` datoteke:

| DLL | Mapa |
|---|---|
| `Carpo.dll` | `Spin\` (izlaz projekta `Carpo.Gen`) |
| `Carpo.Common.dll` | `Spin\` |
| `Carpo.Core.dll` | `Spin\` |
| `Carpo.Data.dll` | `Spin\` |
| `Carpo.UI.dll` | `Spin\` |
| `Carpo.XmlGen.dll` | `Spin\` |
| `SpinDMSUtil.dll` | `Spin\` (u csproj piše Version 2.0.0.12, `SpecificVersion` false) |

Te `Carpo.*.dll` se **grade iz ovog repoa** čim njihove tuđe reference postoje. Za prvi build ih još nema, pa HintPath gleda prazno. Redoslijed:

1. `Carpo.Structure` — nema Jupiter HintPath, treba samo DevExpress
2. `Carpo.Common` — treba upravo izgrađeni `Carpo.Structure.dll` **i** `JupiterTranslator10.dll`
3. `Carpo.Data` — treba `Carpo.Common.dll`
4. `Carpo.Core` — treba Common, Data, Structure i `JupiterLogger.dll`
5. `Carpo.UI` — treba sve prethodno plus tablica u odjeljku 2
6. `Carpo.Gen` — treba Common, Core, Data, Structure, UI i `RibbonBase.v26.dll`
7. `Carpo.XmlGen` — treba Common, Core, Data, Structure, UI
8. `Carpo.TestShell` — treba izlaze 6 i 7 plus `SpinDMSUtil.dll`

Zato „samo TestShell exe“ i dalje traži cijeli taj lanac. Designer, Specific (`Dimenzije.dll`), CMS i Lite ne trebaju.

### Jupiter DLL-ovi koje lanac traži, a repo ih ne proizvodi

| DLL | Tko | HintPath |
|---|---|---|
| `JupiterTranslator10.dll` | `Carpo.Common.csproj` | `Spin\` |
| `JupiterLogger.dll` | `Carpo.Core.csproj` | `Spin\` |
| `RibbonBase.v26.dll` | `Carpo.UI.csproj`, `Carpo.Gen\Carpo.csproj` | `Spin\` |
| `Spin.Printing.v26.dll` | `Carpo.UI.csproj` | `Spin\` |
| `SpinDMSUtil.dll` | `Carpo.UI.csproj`, `Carpo.TestShell.csproj` | `Spin\` |
| `Microsoft.Office.Interop.Outlook.dll` | `Carpo.UI.csproj` | `Spin\` (ne NuGet cache, iako `packages.config` navodi paket 15.0.4797.1004) |
| `adodb.dll` | `Carpo.UI.csproj` | `SharedAssemblies\Microsoft\adodb.dll` (apsolutna putanja) |

`Dimenzije.dll` treba samo `Carpo.Specific`. Za TestShell + `Carpo.NetGen` se može izostaviti.

## 2. DevExpress 26.1.3.0

`PublicKeyToken=b88d1754d700e49a`. Nema ih u gitu. Najčišće: instalirati DevExpress 26.1.3 na build stroj (isti build kao na developerovim mašinama). NuGet feed `https://nuget.devexpress.com/<ključ>/api` **ne spaja** današnje reference, jer csproj nema `HintPath` ni `PackageReference` na te pakete. Dok se csproj ne prepiše, feed sam ne gradi.

TestShell izravno referencira (`Carpo.TestShell.csproj` 127–158):

`DevExpress.Drawing.v26.1`, `Data.v26.1`, `Pdf.v26.1.Core`, `Office.v26.1.Core`, `RichEdit.v26.1.Core`, `RichEdit.v26.1.Export`, `Printing.v26.1.Core`, `Sparkline.v26.1.Core`, `Data.Desktop.v26.1`, `Utils.v26.1`, `XtraBars.v26.1`, `XtraEditors.v26.1`, `XtraGrid.v26.1`, `XtraLayout.v26.1`, `XtraNavBar.v26.1`, `XtraPrinting.v26.1`.

`Carpo.UI` dodatno (jer TestShell vuče `Carpo.UI.dll`, a UI projekt se mora prvo prevesti):

`BonusSkins`, `Charts.v26.1.Core`, `XtraCharts`, `XtraCharts.Extensions`, `XtraCharts.UI`, `XtraCharts.Wizard`, `Pdf.v26.1.Drawing`, `XtraPdfViewer`, `XtraRichEdit`, `Images`, `XtraSpreadsheet`, `XtraTreeList`.

`Carpo.Structure` i `Carpo.Core` imaju još uži skup istog 26.1.3 (Bars, Editors, Layout, Grid, …). Jedan installer pokrije sve. Kopirati DLL ručno isplati se samo ako je popis iz `HintPath` prazan, a DX reference nemaju HintPath — ručna kopija bez instalera znači staviti ih u GAC ili dodati HintPath, što ovaj repo danas ne radi.

Analyzer `DevExpress.Generator.dll` je u `$(ProgramFiles)\DevExpress 26.1\Components\Offline Packages\...`. Build prođe i bez njega ako je `Analyzer` uvjetno; nije provjereno gašenjem. Instalacija 26.1.3 ga donese.

## 3. Što `Carpo.NetGen` doda na taj skup

Compile (kad se projekt doda):

- iste `Carpo.Data`, `Carpo.Core`, po potrebi `Carpo.UI` i `Carpo.Common`
- DX `XtraGrid`, `XtraEditors`, `XtraLayout`, `XtraTreeList`, `Data`, `Utils` — već u skupu iz odjeljka 2
- **ne** Janus, **ne** DX 13.1

Runtime, late-bound, nisu reference u csproj. Bez njih se TestShell prevede, a granula koja dođe do komande padne u CB log kao i danas kad DLL fali:

| DLL | Komanda | Klasa |
|---|---|---|
| `SpinDMSUtil.dll` | učitavanje XML-a | `SpinDMSUtil.NetGenFinder` (već u compile skupu TestShella) |
| `Raverus.FiskalizacijaDEV.COM` | `FISKALIZACIJA*` | `Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav` |
| `SLOFISKAL` | isti, ako URL sadrži `fu.gov.si` | `SLOFISKAL.Program` |
| assembly iz `ASSEMBLYLOAD(Ime.Klasa)` | po granuli | javna metoda `totalgen(id, operater, connection, a, b)` |
| `rptDevRepx.dll` | `::repx/` i dijalog ispisa | `rptDevRepx.clsReport`, `isnetgen = 1` (`Form6.vb` 134–136). Već je DevExpress 26 |
| `Spin.Printing.v26.dll` | ispis preko `Spin.Printing.Start` | zamjena za v23/v9.1 (`Class1.vb` 297–304). v23 i v9.1 se ne isporučuju |
| `JupiterTranslator10.dll` | prijevod | već u compile skupu Commona; NetGen ga još i `Assembly.Load`a u `Form1.Run` |
| `JupiterLogger.dll` | log | već u compile skupu Corea |

`Raverus` i `SLOFISKAL` ne trebaju za build TestShella. Trebaju na stroju na kojem se smoke-a `FaktureNeFisk.xml` (faza M7). Certifikat PFX i URL nisu DLL; dolaze iz tablice `mpfiskalizacija`.

## 4. Što ne treba

- `Janus.Windows.GridEX.v3`, `Janus.Data.v3`, `Janus.Windows.Common.v3`
- `DevExpress.*.v13.1`
- `GridExPrinting.dll`, `Spin.Printing.v23`, `Spin.Printing.v9.1` — ispis ide na v26 i `rptDevRepx`
- `ILCalc`, `JupiterHelpClient` (stari NetGen vbproj)
- `Dimenzije.dll` dok se ne gradi `Carpo.Specific`

## 5. Kako ih predati

Dovoljna je zip mapa s ovim stablom:

```text
SharedAssemblies\
  Spin\
    JupiterTranslator10.dll
    JupiterLogger.dll
    RibbonBase.v26.dll
    Spin.Printing.v26.dll
    rptDevRepx.dll
    SpinDMSUtil.dll
    Microsoft.Office.Interop.Outlook.dll
    Raverus.FiskalizacijaDEV.COM.dll
    SLOFISKAL.dll
  Microsoft\
    adodb.dll
```

`Carpo.*.dll` ne moraju biti u zipu ako se grade iz izvora. Ako su u zipu, prvi build ih prepiše.

DevExpress ostaje instalacija na stroju (ili poseban dogovor o HintPathu). Ne stavljati ga u git.

Baza nije DLL. Spajanje na `spin` je sljedeća iteracija: self-hosted runner na testnom serveru, opis u dokumentu 08.
