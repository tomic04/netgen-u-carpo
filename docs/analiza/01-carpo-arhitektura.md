# 01 — Arhitektura Carpa

**Iteracija 2.** Jupiter i TestShell su oba ciljna ulaza. `IJupiterStart` nije ugovor koji `Starter` implementira; vidi kraj ovog dokumenta. Raspored ne mora biti piksel-identičan.

Izvor je `Devex_26/` (C#, .NET Framework 4.8, DevExpress 26.1.3.0). Solution `Devex_26/Carpo.sln` ima 12 projekata. DLL-ovi nisu u gitu, pa graf ovisnosti dolazi iz `.csproj` i `.sln`, ne iz pokrenutog builda.

## 1. Graf projekata

Svi projekti: `TargetFrameworkVersion` v4.8. Jedina MSBuild `ProjectReference` u cijelom stablu je `Carpo.UI` → `Carpo.Data` (`Devex_26/Carpo.UI/Carpo.UI.csproj`, oko linije 1036). Sve ostale veze su `HintPath` na već izgrađene DLL-ove u

`C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\Spin\`.

Zato solution `ProjectDependencies` (npr. `Carpo.sln` 7–77) opisuje redoslijed koji TFS očekuje, ali čist clone sam sebe ne može složiti.

```
Carpo.Structure
    └─ Carpo.Common          (+ JupiterTranslator10.dll)
         └─ Carpo.Data
              └─ Carpo.Core  (+ JupiterLogger.dll)
                   └─ Carpo.UI   (+ RibbonBase.v26, Spin.Printing.v26,
                                  SpinDMSUtil, adodb, Outlook interop)
                        ├─ Carpo.Gen          (assembly ime: Carpo)
                        ├─ Carpo.XmlGen
                        ├─ Carpo.Designer
                        ├─ Carpo.Lite         (assembly ime: ComponentTracker)
                        └─ Carpo.TestShell    (WinExe)
Carpo.Specific  (+ Dimenzije.dll, Diagram)
Carpo.CMS       (WinExe, Common + Data)
```

| Projekt | Izlaz | DevExpress | Napomena |
|---|---|---|---|
| `Carpo.Structure` | Library | v26.1, oko 14 referenci | XML model, bez UI |
| `Carpo.Common` | Library | v26.1 | `HintPath` na `Carpo.Structure.dll`, `JupiterTranslator10.dll` |
| `Carpo.Data` | Library | v26.1, tanko | SQL helperi; `HintPath` na `Carpo.Common.dll` |
| `Carpo.Core` | Library | v26.1 | logger; `JupiterLogger.dll` |
| `Carpo.UI` | Library | 27 referenci (Charts, Pdf, RichEdit, Spreadsheet, …) | jedini projekt koji referencira drugi **projekt** |
| `Carpo.Gen` | Library `Carpo.dll` | 8 referenci | host `Start : Starter` |
| `Carpo.XmlGen` | Library | samo `XtraEditors.v26.1` | generator XML-a iz sheme |
| `Carpo.Specific` | Library | + Diagram, VerticalGrid | `Dimenzije.dll`, HintPath s predubokim `..\` |
| `Carpo.TestShell` | WinExe | v26.1 | ručni pokretač granule |
| `Carpo.Designer` | WinExe | širi skup | dizajner |
| `Carpo.CMS` | WinExe | v26.1 | |
| `Carpo.Lite` | WinExe `ComponentTracker` | nema izravnog DX | samo `Carpo.dll` + `Carpo.UI.dll` |

Verzija asemblera na projektima je `10.5.26.2` (npr. `Devex_26/Carpo.Gen/Properties/AssemblyInfo.cs`). `packages.config` postoji samo za Outlook interop (`Microsoft.Office.Interop.Outlook` 15.0.4797.1004 u `Carpo.UI/packages.config`). `PackageReference` nema. `nuget.config` nema.

Debug `OutputPath` pokazuje na `SharedAssemblies\Spin\` (npr. `Carpo.UI.csproj` Debug|AnyCPU). Post-build koraci su zakomentirani.

## 2. Kako se XML učita

Tok za granulu:

1. Host zove `Starter.Run(server, database, username, password, operateriid, parent, pTop, pLeft, path)` (`Devex_26/Carpo.UI/Connector/Abstract/Starter.cs` 78–136).
2. Složi se connection string (`Carpo.Data.Constructor.SetConnectionString`, linija 110) i ADO string za stari VB6 (`Connection.ConnectionStringADO`, 113–115).
3. `DeserializeXml` (162–213):
   - ako je postavljen `TestXmlPath`, to je putanja (TestShell);
   - inače ako `ModuleId` sadrži `\`, to je puna putanja;
   - inače `Path.Combine(IniFile.Ini["RemoteDllPath"], "Carpo", ModuleId)` (167–172).
4. Postojanje datoteke provjerava `FileFinder("Carpo").CheckFileExist` (183). Implementacija je u vanjskom `SpinDMSUtil`, **nije u ovom repou**.
5. `Serializer.DeserializeXml` (`Devex_26/Carpo.UI/Logic/Serializer.cs`) spoji dijelove i deserijalizira `JRoot`.
6. `SetInitForm` (215–263) bira formu `type="master"`. Ako ima `managerlayout` → manager, inače `layout` → standard. Bez oba, greška.
7. `Carpo.Gen.Start.RegisterForm` (`Devex_26/Carpo.Gen/Start.cs` 15–44) stvara konkretnu formu.

`Carpo.UI/Connector/Xml.cs` (`ReadXml`) učitava `{assemblyFolder}\{assemblyName}.xml`. To **nije** putanja granule. Granula ide kroz `Serializer`.

### Spajanje prije deserijalizacije

`Serializer` radi nad `XDocument`, pa tek onda `XmlSerializer(typeof(JRoot))`:

- `<part path name required>` — ubaci fragment s diska (`GetParts`)
- `<partsql dataname name>` — SQL vrati XML fragment (`GetPartsSql`)
- `<form path name>` — forma u drugom fajlu (`GetForms`)
- `<customcontrol dll class name initonmaster>` — registar vanjskih kontrola

Redoslijed fallbacka za fajl (isti obrazac u `Serializer` i `Carpo.Core/Logic/XmlPath.cs`): `sub{Database}/forms|parts/...`, zatim `sub/...`, zatim ravni folder. DMS sloj (`CarpoFinder.GetCarpoRootXml`) je u `SpinDMSUtil`. Ponašanje iza poziva je **neprovjereno** u ovom repou.

`Carpo.XmlGen` je drugi cjevovod: shema → zamjena `@` i `@@operateriid` → `Deserialize` → punjenje `datalist` → `Parse` s `foreach="~dt"` (`Devex_26/Carpo.XmlGen/Start.cs`, `XmlParse/Parse.cs`). To generira Carpo XML. Nije NetGen interpreter.

## 3. Objektni model

Korijen:

```6:26:Devex_26/Carpo.Structure/Models/Core/Root.cs
[XmlRoot("root")]
public class JRoot
{
    [XmlElement("form")]
    public List<JForm> Form { get; set; }
    [XmlElement("datalist")]
    public JDataList DataList { get; set; }
    // loglevel, version, translatortruncate, translatoruselog
}
```

`JForm` (`Models/Core/Forms.cs`) nosi `managerlayout` **ili** `layout`, plus `event`, `datalist`, `panelbuttons`, `actionpanel`, `infocontrols`, `method`, `parameter`.

`JManagerLayout` (`Models/Core/Controls.cs` 6–19): `grid`, `tree`, `lookup`, `tabs`. To je master/detail granula.

`JLayout` / `JLayoutItem` (`Models/Core/Layouts.cs`): ugniježđeni `group`, `item`, `tabs`, definicije redaka i stupaca. Na `item` sjedi točno jedan editor. Popis elemenata (308–392):

`textedit`, `hyperlinkedit`, `memoedit`, `richedit`, `comboedit`, `combomultiedit`, `tokenedit`, `combotreeedit`, `browseedit`, `dateedit`, `buttontextedit`, `button`, `searchedit`, `labeledit`, `checkedit`, `radioedit`, `daysedit`, `timeedit`, `pictureedit`, `pdfviewer`, `documentviewer`, `grid`, `tree`, `tabs`, `separator`, `customcontrol`, `chart`, `container`.

Uvjeti `if` / `elseif` / `else` su atributi na `JCondition` (`Models/Core/Base/Base.cs`). Vrijednost se evaluira u `createelement` ili `deleteelement` (pravila u `carpobot/Carpo-Engine-Master-Rules.md`, izvor `Carpo.XmlGen` `Parse.cs` i `Statemants.cs`).

Bool svojstva često imaju par: tipizirano polje s `[XmlIgnore]` i string `[XmlAttribute]` (npr. `enabled` / `enabledStr`). Ako string nije `true`/`false`, `bool.Parse` baci. To je suprotno NetGenu, koji šuti.

Na gridu već postoje NetGen atributi, ali samo kao stil kolona, ne kao jezik:

```550:568:Devex_26/Carpo.Structure/Models/Core/DataControls.cs
[XmlAttribute("netgenfields")]
public string NetGenFieldsStr { get; set; }
[XmlAttribute("netgensizes")]
public string NetGenSizesStr { get; set; }
[XmlAttribute("netgenstyles")]
public string NetGenStylesStr { get; set; }
```

`ParseNetGenStyles` (`Devex_26/Carpo.UI/Logic/Style.cs` 88) razbija zarezima `bold`, `italic`, `strikeout`, `underline` i ime boje.

## 4. Crtanje

| Predložak | Klasa | Gdje se bira |
|---|---|---|
| manager | `Forms.Master.Manager` : `JupiterManager` | `Start.cs` 24–25 |
| splitter | `Forms.Splitter` : `JupiterEditor` | `Start.cs` 28–29, `template` sadrži `splitter` |
| info | `Forms.Info` | `Start.cs` 30–31 |
| sidebar | `Forms.SideBar` | `Start.cs` 32–33 |
| ostalo | `Forms.Standard` | `Start.cs` 35 |

`JupiterEditor.SetLayout` gradi `CustomLayoutControl` (potklasa DevExpress `LayoutControl`). Grupa je tablica: `row`, `col`, `rowspan`, `colspan`. Nema apsolutnog `TOP`/`LEFT`.

`ControlInit.CreateControl` (`Devex_26/Carpo.UI/Logic/Layout/ControlInit.cs`, grana od ~108) mapira model na omot:

| Model | Omot | DevExpress ispod |
|---|---|---|
| `JGrid` | `JGridControl` | `GridControl` / `GridView` |
| `JTree` | `JTreeListControl` | `TreeList` |
| `JComboEdit` | `JLookUpControl` | lookup |
| `JComboMultiEdit` | `JComboMultiEditControl` | checked combo |
| `JTextEdit` | `JTextEditControl` | `TextEdit` |
| `JDateEdit` | `JDateEditControl` | `DateEdit` |
| `JSearchEdit` | tražilica | |
| `JButton` | `JButtonControl` | |
| `JCustomUCDef` | `JCustomUC` | refleksija, `dll` + `class` |

Točan DX tip za svaki omot je u pripadajućem `J*Control.cs`. Ovdje je bitno da je tvornica **zatvorena grana if-ova** nad svojstvima `JLayoutItem`, ne registar imena. Novi XML element bez novog `[XmlElement]` i nove grane u `CreateControl` ne postoji.

Chrome forme je `Forms.Jupiter.Base` : `RibbonBase.RibbonBase` (`Devex_26/Carpo.UI/Forms/Jupiter/Base.cs`). `RibbonBase.v26.dll` nije u repou.

## 5. Podaci

`Carpo.Data`:

- `Constructor.SetConnectionString` — `SqlConnectionStringBuilder`, integrated ili uid/pwd
- `Connection` — thread-static string, server, baza
- `SqlHelper` i `Get/*`, `Set/*` za varijable, pretragu, predloške grida

`Carpo.UI/Logic/Data/Core.cs`:

- `FindJData` bira `<data>` po imenu i po `if`
- `SetData` izvršava stored procedure ili tekstualni SQL
- rezultat sjeda u `Dictionary<string, DataTable>` na formi, ključ je `data/@name`
- `~ime` u izrazima skida se na to ime (`GetDataByName`)
- izlazni parametri se čitaju nakon izvršenja (`CheckOutputParameters`)

Veza kontrole na podatke je deklarativna: `dataname="~dtX"`, `datavalue="~dtX.@Kolona"`, `fieldvalue` / `fieldtext` na tražilici. To nije NetGenovo „kolona 0 = vrijednost, kolona 1 = prikaz“ i nije lijepljenje `@ime=` na kraj SQL-a.

## 6. Događaji i akcije

Forma (`JupiterShared.SetEvents`) veže XML `event/@type` koji sadrži: `init`, `load`, `shown`, `close`, `keydown`.

Kontrola (`ComponentBase.SetEvents`) veže, među ostalim: `cellvaluechanged`, `valuechanged`, `focusedrowchanged`, `selectionchanged`, `doubleclick`, `click`, `leave`, `tabchanging`, `tabchanged`, `keydown`, `rowclick`.

Dispečer je `Events.Checking` (`Devex_26/Carpo.UI/Logic/Events.cs` 426–468). Prva grana koja pogodi `action/@type` vraća se. Redoslijed: `exit`, `method`, `process.run`, `print`, `control …`, `menu`, `variable value`, `datarun`, `form`, `message`, `comdll`, `footer`, `tab`, `search`, `panel`, `transaction`, `*layout`, `generate`, `infocontrols`.

Prije `Checking`, `RunActions` posebno hvata `textexport`, `cancel` na tabu, `outlook` / `ZKI` / `JIR` (`FiscalOutlookActions.cs`), `confirm`, `validate`, `drop`/`drag`/`editor`.

Porodice koje engine stvarno gleda (prefiks, case-insensitive):

| Porodica | Primjeri type |
|---|---|
| podaci | `datarun reader`, `datarun scalar`, `datarun nonquery` |
| transakcija | `transaction` s ugniježđenim datarun |
| kontrola | `control populate`, `control value`, `control focus`, `control enable`, `control foreachrow`, … puni `switch` u `Logic/Controls/Notifications.cs` (~104–135) |
| forma | `form open`, `form close`, `form caption`, `form dialogresult`, `form clear` |
| ispis | `print` (vrijednost koja počinje s `repx` ide u REPX) |
| poruka | `message`, `confirm`, `validate` |
| varijabla | `variable value` |
| ostalo | `menu`, `tab`, `panel`, `footer`, `search`, `comdll`, `process.run`, `method`, `generate string\|number\|datastring`, `setlayout` |

`method` zove XML `<method>` po imenu (`Logic/Methods.cs`). Nije NetGen `ASSEMBLYLOAD`.

Uvjeti idu kroz `Statemants.CheckIf` i `Conditions` (datoteka se zove `Statemants.cs`, s tim slovkanjem).

## 7. Izrazi

Carpo **nema** NetGen `@@ime` kao opći mehanizam. Ima:

| Zapis | Značenje | Izvor |
|---|---|---|
| `{izraz}` i `{izraz::format}` | zamjena u stringu | `Logic/Evaluate.cs` |
| `#kontrola` | vrijednost kontrole | `Logic/Parser.cs` |
| `~dt` i `~dt.@kolona` | dataset | `Parser.cs`, `Data/Core.cs` |
| `$param` | parametar forme / globalni | `Parser.cs` |
| `global.` / `parent.` | opseg | `Parser.CheckScope` |
| `@@operateriid`, `@@database`, `@@connection`, … | uski skup pub-var | `Logic/Variables.cs` |
| `##metoda` | ugrađena ili XML metoda | `Parser.cs` |
| `parent.#grid` | kontrola roditelja | akcije `control populate` |

`@@` u Carpu nije „svaki naziv kontrole“. U NetGenu jest. To je središnji razlog zašto se NetGen SQL ne može proslijediti Carpo evaluatoru bez vlastitog preprocesora.

## 8. Kamo se `Carpo.NetGen` može uključiti

Postojeće točke, s posljedicom:

| Točka | Što dopušta | Što ne dopušta |
|---|---|---|
| Podklasa `Starter`, override `RegisterForm` | novi host uz isti `Run` potpis (`IJupiterStart`) | `DeserializeXml` i dalje traži `JRoot` |
| `Serializer` prije `XmlSerializer` | ubaciti pretvorbu NetGen→`JRoot` | gubitak svega što model nema (apsolutni layout, komandni jezik, `!!`) |
| `ControlInit.CreateControl` + novi `[XmlElement]` | nova kontrola u Carpo jeziku | ne čita `<INPUT TYPE>` |
| `<customcontrol dll class>` | tuđi UserControl | i dalje Carpo XML okolo |
| nova grana u `Events.Checking` | nova akcija | akcija je i dalje deklarativni XML, ne string `CLOSE#OK` |
| `netgenfields` / `ParseNetGenStyles` | izgled kolona tražilice | nije loader |

Zaključak za fazu 2: `Carpo.NetGen` ne treba naslijediti `Starter.DeserializeXml` ako želi učitati NetGen bajtove nepromijenjene. Treba vlastiti parser, a od Carpa uzeti konekciju, log, DX kontrole i ispis. Obrazloženje opcija je u dokumentu 06.

## 9. TestShell

`Carpo.TestShell/Program.cs` radi `Application.Run(new Main())`. Nema parsiranja komandne linije. `Main` ima konstruktore `(server, database, xml, operateriId)` i još jedan s `autoMode`, ali `autoMode` se u repou nigdje ne čita (**neprovjereno** da negdje izvan repa ipak služi). Klik Start (`cmdGen_Click`) radi:

```csharp
start.TestXmlPath = xml;
start.ModuleId = Path.GetFileName(xml);
start.Run(server, database, username, password, operateriId, (int)pnlShell.Handle, 0, 0, "");
```

Harness za korpus zato ne može biti „pusti TestShell nad 609 fajlova“ bez novog ulaza. Vidi dokument 08.

## 10. Ispravka: dva Jupiter ugovora, ne jedan

`IJupiterStart` (`Devex_26/Carpo.Structure/Interfaces/IJupiterStart.cs`) glasi:

```csharp
void Run(string server, string database, string username, string password,
         int operateriid, string menuID, int parent, short pTop, short pLeft, string path);
```

`Starter` tu sučelje **ne** nasljeđuje. Njegov `Run` (`Starter.cs` 78) nema `menuID`. `MenuID` je svojstvo koje Jupiter može postaviti na objekt prije `Run` (`Starter.cs` 48, čita se u `RegisterForm` 346–352). TestShell zove točno tih 9 argumenata i prije toga postavi `TestXmlPath` i `ModuleId` (`Carpo.TestShell/Main.cs` 181–184). Roditeljski handle je `(int)pnlShell.Handle`.

Carpo forma se veže na Jupitera u `Forms.Jupiter.Base.Start` (`Base.cs` 103–122): `SetParent(this.Handle, ParentHandle)`, zatim `Show` ili `ShowDialog` ako je `isModal`. Nema metode `ShellResize`. NetGen je ima na `Loader` (`netgen/Loader.vb` 22–26). Zato `Carpo.NetGen` koji zamjenjuje NetGen ne smije samo naslijediti `Starter`: Jupiterov poziv NetGen granule ne prolazi kroz `Carpo.Gen.Start`.

TestShell danas ne gleda korijen XML-a. `cmdGen_Click` uvijek radi `new Gen.Start()` osim ako je uključen XmlGen. Da bi se NetGen XML testirao iz istog prozora, ta grana mora prepoznati `<PROJECT>` i zvati drugi `Run`. To je jedina predviđena izmjena TestShella; opis je u dokumentu 08.
