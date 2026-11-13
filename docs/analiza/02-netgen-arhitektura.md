# 02 — Arhitektura NetGena

Izvor je `netgen/` (VB.NET, .NET Framework 4.8, `Option Strict Off`, izlaz `Library`, namespace `NetGenDLL`). Glavnina logike je u `Form1.vb` (podloga) i `Form2.vb` (dijalog). Zajednički SQL i ispis su u `Class1.vb`. Postojeći opis ponašanja koji se poklopio s kodom u ovoj provjeri: `NetGenBot/docs/NetGen-Engine.md`, `NetGen-Komande.md`, `NetGen-Kontrole.md`, `NetGen-Tok.md`. Ovdje su samo tvrdnje koje su u ovoj fazi ponovo viđene u kodu, s linijama.

## 1. Ulaz

Jupiter ne pokreće exe. Zove:

- `Loader.Run` / `Loader.Run8` — stvara `Form1` (`netgen/Loader.vb`). `Run` još diže zaseban AppDomain (oko 86). U `Run8` je AppDomain zakomentiran (komentar 22.04.2024).
- `frm.Run` / `frm.Run8` — stvara `Form2` (`netgen/frm.vb`).

Oba primaju `moduleId`, parametre i `mainHandle`. `ModuleID` odsiječe argumente u zagradi i ostavi `Mid` = ime XML-a. Putanja:

```1137:1139:netgen/Form1.vb
MOJremotedllpath = remotedllpath
strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid
```

Isto u `Form2.vb` (oko 411). `Run.vb` slaže istu putanju, ali **nije** u `NetGenDLL.vbproj` — ne ulazi u DLL.

`Form1.Run` (oko 1098) otvara konekcije, zove `Show()`, pa Win32 `SetParent` na Jupiterov handle. Vlastiti `Application.Run` oko toga ruši se u `StackOverflowException` (`netgen/CLAUDE.md`). Harness ne smije biti prazan WinForms host.

### Redoslijed učitavanja XML-a

`Form1.ProcitajXMLzaPodlogu` (oko 3167–3269), potvrđeno u `NetGen-Tok.md` i u čitanju iste funkcije:

1. `Assembly.Load("SpinDMSUtil")` → `SpinDMSUtil.NetGenFinder.GetFile`. Ako DLL-a nema, greška ide u CB log, ne u korisnika.
2. Ako je dokument prazan i `Conn4` živi: `select body from netgenxml` (baza `JupiterXML`), tijelo se dekriptira (`tan123ge` — detalj ključa **nije** ponovo čitan u ovoj fazi, ostaje prema `NetGen-Tok.md`).
3. Inače `m_xmld.Load` s diska.
4. Ako baza baci, opet disk.
5. Ako je `BuildinXML`, ugrađeni resurs `Resource1`.

`Form2.ProcitajXML` (oko 789–838): ako log već kaže da je XML došao iz baze, DMS se preskače; inače DMS, pa disk. `INCLUDE` (oko 844–943) dijeli putanje s `#`, svaku vuče iz DMS-a ili fajla i ubacuje djecu u glavni dokument.

Nema XSD. Nema `GetElementsByTagName` kao glavnog puta. Koristi se `SelectNodes("/PROJECT")` i `Attributes.GetNamedItem("IME")`. Nedostajući atribut pod `On Error Resume Next` ne ruši granulu. Izuzetak koji dokumentacija i kod ističu: `IndexOf(":")` na `CMDxCAPTION` / `BUTTONS` pukne ako nema dvotočke.

## 2. Podloga i dijalog

`Form1` čita atribute **jednog** čvora `/PROJECT` (petlja oko 3299–3520). Naslov je tekst prvog djeteta `/PROJECT/CAPTION` (oko 3522). `VERSION` se doda u naslov kao `v[…]`.

Atributi koje ta petlja stvarno čita (linije prema `NetGen-Engine.md`, spot-provjera `ONLOAD` 3368, `TREE` 3387, `BUTTONS` 3391, `PERMISIO` 3395):

`CMD1CAPTION`…`CMD10CAPTION`, `COMBO`, `COMBOCLICK`, `GROUPBYBOXVISIBLE`, `GROUPBYBOXDETVISIBLE`, `MASTERGRIDLAYOUTFILE`, `TREEDRAGDROP`, `TREETRANSLATE`, `FROZENCOLUMNS`, `ONENTERKEY`, `ONCLOSED`, `STARTUPSTORA` (odmah prepisano s `ONLOAD`), `ONLOAD2`, `MULTIGRIDLAYOUT`, `SETDEFAULTS`, `FILTERSAVE`, `DETAILGRID`, `DMSDET`, `DETMOVELAST`, `MASTERMOVELAST`, `INCLUDE`, `POPUP` (prepis `POPUPMASTER`), `POPUPDETAIL`, `TREE`, `DBLCLICKFORM` (prepis `MASTERDBLCLICK`), `DETAILDBLCLICK`, `BUTTONS`, `ICON`, `VERSION`, `SPLITTERDISTANCE`, `PERMISIO`, `REFRESHAFTERCHILD`, `TRANSLATECOLUMNS`, `TRANSLATEDETCOLUMNS`, `AUTOMATICSORT`, `AUTOMATICSORTDET`, `DXGRID`, `ENTERKEYBEHAVIOR`, `FILTERMODE`, `FILTERMODEDET`, `MENU`, `TIMER`, `PAGEHEADER*`, `PAGEFOOTER*`, `PAGEORIENTATION`, `CHECKBOXVISIBLE`, `TOTALROWVISIBLE`, `DETCHECKBOXVISIBLE`, `DETTOTALROWVISIBLE`, `COMMANDTIMEOUT`, `RELATION`, `EDITCOLUMNS`, `ONCELLEDITED`, `BEFORECELLEDITED`, `EDITCOLUMNSDET`, `ONCELLEDITEDDET`, `BEFORECELLEDITEDDET`, `DEFAULTCONNECTION`, `SQL2`, `SQL3`, `SQL4`, `HELPKEY`, `HELPKOD`.

`BUTTONS` (`Form1.vb` oko 3770–3813) radi `Split("#")` i redom prepisuje Button1… Zato `#` unutar jedne akcije ne smije biti separator komandi. Za to služi `~`, koje `NovaGenerickaForma` pretvara u `#` (`Form1.vb` 4557).

`Form2` traži `/PROJECT/{imeForme}` i djecu `INPUT` (oko 957 i 1149). Atributi forme (1031–1122, viđeno 1031–1058): `MAXIMIZEBOX`, `TIMERSQL`, `COMMANDTIMEOUT`, `STORA` pa odmah prepis `ONSUBMIT` (1036–1039), `DETAILGRID`, `ONLOAD`, `ONCLOSED`, `ONCLOSING`, `ONACTIVATED`, `ONLOADED`, `ONLOAD2`, `HELPKOD`, `ONLOSTFOCUS`, `NEXTFORM`, `FOCUS`, `DEFAULTCONNECTION`, `CUSTOMCONNECTIONSTRING`, `CMD1CAPTION`…`CMD6CAPTION`, `FILLEXPLORER`, `WIDTH`, `HEIGHT`, `VERSION`, `PERMISIO`, `DBCLIPBOARD`, `RESIZE`, `ONESCKEY`.

`ONSUBMIT` prepisuje `STORA`. Stariji atribut je mrtav čim postoji `ONSUBMIT`.

`ONLOAD` (`strStartupStora`) izvršava se prije petlje `INPUT`: `Form2.vb` 1195–1199 radi `ExecSql`, i ako `GreskaKodIzvrsavanjaSQLa` nije prazan, izlazi iz forme. Petlja kontrola počinje tek oko 1253. `yearmonth` na stablu podloge je `For g = 2005 To 2009` (`Form1.vb` 4527–4534).

`DBCLIPBOARD`: ako ime forme sadrži `filter` i atribut nije `false`, forsira se `true` (oko 1102–1104).

`WIDTH`/`HEIGHT` forme dijele se s 14:

```1093:1094:netgen/Form2.vb
Me.Width = m_node.Attributes.GetNamedItem("WIDTH").Value / FakPretvorbe
Me.Height = m_node.Attributes.GetNamedItem("HEIGHT").Value / FakPretvorbe
```

`FakPretvorbe` je 14 na tri mjesta (`Module1.vb` 65, `Form1.vb` 49, `Form2.vb` 58). Nije 15 (twip/pixel na 96 DPI). 7085/14 ≈ 506 px. To je empirijski faktor, ne W3C.

## 3. Janus — svaki tip koji se u kodu koristi

Referenca (`NetGenDLL.vbproj` 93–104): `Janus.Data.v3` 3.5.0.0, `Janus.Windows.Common.v3` 3.5.0.0, `Janus.Windows.GridEX.v3` 3.5.0.0, `PublicKeyToken=21d5517571b185bf`. `HintPath` je `DevExpress131\…`. Te mape u gitu nema.

| Tip | Gdje | Čemu |
|---|---|---|
| `Janus.Windows.GridEX.GridEX` | `Grid.vb` 8 (baza klase `Grid`), `Form1.designer.vb` (master i detail), `Form3`/`Form7`/`Form9` designer | grid na podlozi, grid u dijalogu, picker tražilice, jezici, predlošci ispisa |
| `GridEXColumn` | `Form1.vb` (npr. 116, 4844), `Form2.vb` 54 | tekuća i dvokliknuta kolona, `Find` |
| `GridEXRow` | `Form1.vb` 4964+, `Module1.vb` 441 | čekirani redovi, prijevod |
| `GridEXGroup` | `Form1.vb` 2159, `Form2.vb` ~1966 | grupiranje |
| `GridEXFormatCondition` | `Form1.vb` 7544–7556 | `NEGATIVERED` / `NOTZERORED` |
| `GridEXPrintDocument` | `Form1.designer.vb`, runtime `New` u `Form2.vb` ~9009 i ~9110 | ispis grida |
| `Export.GridEXExporter` | `Form1.vb` 5694, `Form2.vb` ~8909 | Excel |
| `EditControls.NumericEditBox` | `NumerBox.vb` 4 | `TYPE` koji sadrži `number` ili `cur` |
| `EditControls.EditBox` | `Form5.designer.vb` | samo dijagnostički prozor, ne XML |
| `Janus.Data.InheritableBoolean` | `Form1.vb` 2050, `Form2.vb` ~1746 | total red |
| enumi `SelectionMode`, `View`, `RowCheckState`, `ConditionOperator`, `ColumnAutoSizeMode` | `Form1.vb`, `Form2.vb`, `Grid.vb` | ponašanje grida |
| `AppearanceDialog`, `FormatStyles` | posebne forme | editor izgleda grida, nije XML jezik |

`GridExPrinting.GridExPrinting` (`Form1.vb` ~10) je zaseban DLL (`NetGenDLL.vbproj` 85–87), x86, nije Janus namespace.

**Nije nađeno** u `.vb`: `UIComboBox`, `CalendarCombo`, `ExplorerBar`, `Schedule`, `TabStrip`. Combo XML-a je `System.Windows.Forms.ComboBox` (`Combo.vb`). Datum je `DateTimePicker` (`DTPicker.vb`). Tekst je vlastiti `TeksT` : `TextBox`. Check je `mojCheck`, radio `mojRadio`.

DevExpress **13.1.7** unutar NetGena (isti `vbproj` 59–84):

| Tip | Čemu |
|---|---|
| `XtraGrid.GridControl` / `GridView` | `GridDX` / `GridDXViewer`, `TYPE` sadrži `dxgrid` |
| `XtraEditors.CheckedComboBoxEdit` | `TYPE` sadrži `multidrop` (`Form2.vb` 2478) |
| `XtraEditors.Repository.*` | `Class1.vb` |
| kontrole u `frmIspis` | preview pivot/chart ispisa |

Late-bound, nema reference u projektu (provjereno `Assembly.Load` / `CreateObject` u `.vb`):

| Ime | Uloga |
|---|---|
| `SpinDMSUtil` | XML iz DMS-a |
| `JupiterLogger` | `JupiterLog.vb`, greške |
| `JupiterTranslator10` | prijevod |
| `Spin.Printing.v23` i `v9.1` | ispis iz `Class1` |
| `PrintDlg2010`, COM `PrintDlg.clsPrintDlg` | dijalog ispisa |
| `rptDevRepx` | `Form6.vb`, REPX |
| `NetGenExtensions` | REPX nad gridom |
| `Raverus.FiskalizacijaDEV.COM` | HR fiskalizacija |
| `SLOFISKAL` | SI fiskalizacija |
| `DmsManagerV1` | DMS UI |
| dinamički `ASSEMBLYLOAD(Ime.Klasa)` | `totalgen(...)` |

## 4. Stvaranje kontrola

`TYPE` nije enum. `Form2` radi `InStr` nad malim slovima, prva grana pobjeđuje (`Form2.vb` od ~1256 do ~2737).

| Podstring | Linija (približno, viđeno u kodu i u `NetGen-Kontrole.md`) | Klasa |
|---|---|---|
| `statusstrip` | 1256 | postojeći `StatusStrip1` |
| `label` | 1296 | `Label` |
| `group` ili `frame` | 1354 | `GroupBox` |
| `panel` | 1384 | `Panel` |
| `webbrowser` | 1414 | `WebBrowser` |
| `splitcontainer` | 1451 | `SplitContainer` |
| `check` | 1460 | `mojCheck` |
| `radio` | 1509 | `mojRadio` |
| `dxgrid` | 1579 | `GridDX` (DX 13) |
| `grid` | 1623 | `Grid` : Janus `GridEX` |
| `text` | 1996 | `TeksT` |
| `number` ili `cur` | 2071 | `NumerBox` : Janus `NumericEditBox` |
| `search` ili `trazilica` | 2140 | `SearchBox` |
| `date` ili `dtpicker` | 2232 | `DTPicker` |
| `combo` | 2370 | `Combo` |
| `multidrop` | 2478 | `CheckedComboBoxEdit` |
| `tree` | 2558 | `treeViewMoj` |
| `button` | 2614 | `Buton` |
| `help` | 2672 | `Buton` s `Tag="help"` |
| `picturebox` | 2707 | `PictureBox` |

Posljedica koja se vidi u korpusu (dokument 03): `checkbox` (404) ipak radi jer sadrži `check`. `b_utton` (15) i `buttton` (2) ne sadrže `button` i ne nastanu. `textarea` bi upao u `text` (u ovom korpusu ga nema). `t1ext` (14) ne upadne.

Zajedničko sređivanje je `SrediKontrolu` (`Form2.vb` ~2924): `CAPTION`, `DEFAULT`, `TOP`, `LEFT`, `WIDTH`, `HEIGHT`, `ENABLED`, `VISIBLE`, `NAME`, `TABINDEX`, `PARENT`, `ANCHOR`, `SQL`, `REFRESHAFTERCHILD`, `HOTKEY`. `!!` u tekstu kontrole postaje novi red (2943). `_` se briše. `TOP="0"` ili caption koji završava s `*` složi label `lbl`+ime.

## 5. Komande

Lanac se dijeli po `#`. Na podlozi, unutar `BUTTONS`, `#` je već separator gumba, pa akcija koristi `~`.

`NovaGenerickaForma` (`Form1.vb` 4543) je router podloge. Viđene grane: `REFRESHMASTERGRID` (4570), `ASSEMBLYLOAD(` (4584), `UNCHECKMASTERGRID`, `BLAGAJNAENABLE(`, `CLOSE`, fiskalni prefiksi, `findnode` (4809), `refreshmovetoid` (4831), vodeće `:` → forma (4862), `==` → `PokaziUpit` (4872), `select` (4875), inače ime forme (4906).

`NovaGenerickaForma2` (`Form1.vb` 4925):

- ime počinje s `multiselect` ili `multiblue` (4932): briše `TotalGenMultiSelectTemp` po `suser_sname()` i po `@@operateriid`, zatim insert id-eva
- `ssPermissionProvjeriRafalnuKomandu` (4939): zabrana se samo upiše u log i **izlazi**
- `printrepxmaster` i `printmaster` oba zovu `IspisGRIDREPX` (4945–4956)
- `printdet` zove `IspisGRIDREPXdet` (4958)

`Rafalno` na dijalogu (`Form2.vb` ~6789) ima pun skup: `FOREACHSELECTED`, `FOREACHBLUEROW`, `FOREACHROW`, `PRINTGRID`, `PRINTREPXGRID`, `PIVOTGRID`, `CHARTGRID`, `REFRESHCONTROL`, `OK`, `OKSTAY`, `CLOSE`, `CLICK`, `EVAL`, `MSGBOX`, `CREATETEXTFILE*`, `SHOWOPENFILEDIALOG`, `COLORPICKER`, fiskalizacija, `ASSEMBLYLOAD`. Na podlozi (`Form1.vb` ~6546) iste `FOREACH*` grane postoje i tijelo im je komentar (6841–6850): prepoznaju se, petlja se ne izvrši, i indeks se pomakne pa se pojede i sljedeći segment.

Ako nijedna komanda ne pogodi, dijalog segment šalje u `RefreshControls` (SQL koji puni kontrole). Podloga ga šalje u `ExecSql`.

Posebne kolone prvog reda takvog SQL-a (`NetGen-Komande.md`, `Form2` `DodatnoPostimajKontrolu` ~4482): `focus`, `form`, `formcaption`, `formwidth`, `formheight`, `fillexplorer`, `beep`, `click`, `findnode`, `id`, `temp`, `setcolorforcolorpicker`, te `visible`/`invisible`/`enable`/`disable`/`refresh`. Ostale kolone se zovu kao `NAME` kontrole.

## 6. `@@`

Master, `Form1.SrediSQL` (oko 2470–2504), zamjenjuje fiksni skup: `@@id` (tri veličine slova), `@@operateriid`, `@@detid`, `@@node`, `@@table` (T1–T4 po captionu kartice hijerarhijskog grida), `@@xmlname`, `@@jupitersrvpath`, `@@editedcolumn`, `@@lastvalue`, `@@selectedcolor`.

Dijalog, `Form2.SrediSQL2` (oko 2876–2922), dodaje: `@@tempstring`, `@@dblclickedcolumn`, `@@VERSION`, `@@OPISNIPARAMETRI`, `@@FOCUSEDCONTROL`, `@@selectedfile`, `@@selectedpath`.

Sve ostalo (`@@robaid`, `@@gridstavkeKolona`, `@@searchboxrobaid`, `@@comboboxime`) ide kroz `Class1.SrediSQLPosljeNextForme` (740–853): dok ima `@@`, token se reže do razdjelnika (razmak, `'`, `%`, `#`, `)`, `*`, `/`, `\`, `+`, `-`, `.`, `(`, `=`). Token ide u `NadjiVrijednostuPrevParametrima` (856): skine jedan `@`, traži `@ime=` u `strSviParametriuModulu`. Ime s `ajdijeva` pretvara `~` u zarez. Ime koje počinje s `@DAT` ili sadrži `GRID` i `DAT` pakira datum. Prazno postaje `null`. `@napomena` + `null` postaje prazan string. Ako i dalje nema vrijednosti, gleda se DB clipboard.

`@@id` na masteru: kolona `id`, inače ćelija indeks 2 na Janusu, inače `"0"`. Na DX gridu fallback je prva kolona (`GetID` ~5566, `GetDXID` ~5588). `@@node` se puni tek na klik stabla, ne prije.

`@@GRIDROWCOUNT(` je u `SrediSQL2` zakomentiran (2899–2905). Ne radi. U korpusu se zato koristi `@rowcountIme` koje složi grid parametar, ne taj token.

## 7. SQL

`Class1.ExecSql` (85):

- `~~` → zarez (87), jer zarez u tekstu kontrole ne smije razbiti listu parametara
- `Trim.StartsWith("@")` → `SKIPPED`, `Nothing` (105–108)
- prazan **ili** `IndexOf("@@") > 0` → `SKIPPED` (113–118). Token na poziciji 0 ovaj uvjet ne hvata; hvata ga grana s `@`
- inače `SqlDataAdapter.Fill`, timeout `strCommandTimeout` (default `"30"` u `Module1.vb`)
- `DefaultConnection = 2` ide na `Conn3` (131–141)
- Catch guta poruke koje sadrže `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard` (158–166)
- poruka s `#` ili `+`, ili `Err.Number = 5`: info dijalog. `#OK`/`+OK` brišu grešku. `#close`/`+close` postavljaju `ZelimIzaciIzForme`. `#FOCUS(` / `+FOCUS(` čitaju ime kontrole (170–205)
- inače kritični dijalog s tekstom i SQL-om, pa `spNetGenErrorLog` (koji je na klijentskim bazama prazan, `netgen/CLAUDE.md`) i `JupiterLog`

Konekcije u `Form1.Run`: `Conn` (SQL), `Conn2` (OleDb), `Conn3` (custom), `Conn4` (JupiterXML). `SQL2`/`SQL3`/`SQL4` na `PROJECT` nisu četvrta konekcija; to su dodatni upiti za hijerarhijski grid.

`ONSUBMIT` + parametri (`Form2.Button1_Click` 3367–3388):

```3377:3388:netgen/Form2.vb
If strStora.Trim <> "" Then
    If InStr("INSERTDELETEUPDATESELECT", strStora.ToUpper.Substring(0, 6)) > 0 Then
        strPrevParametri = strParametri
        strStora = cls1.SrediSQLPosljeNextForme(strStora)
        strParametri = ""
    End If
End If
```

Ako prvih 6 slova upadne u `INSERTDELETEUPDATESELECT` (`SELECT`, `INSERT`, `UPDATE`, `DELETE` upadnu; `spKart` ne), `@@` se ugrade u tekst i sakupljeni parametri se **ne** dodaju. Inače se na podlozi, nakon zatvaranja dijaloga, radi `strStora & " " & strParametri`, osim ako stora sadrži `!!`:

```5449:5453:netgen/Form1.vb
If cf.strStora.Contains("!!") Then
    PokaziUpit(... cf.strStora.Replace("!!", ""))
Else
    PokaziUpit(... cf.strStora & " " & cf.strParametri)
```

Isto za `treeandmaster` (5499) i za obični `ExecSql` (5511–5514). `!!` je prekidač „SQL je već potpun“. U captionu kontrole `!!` je novi red. Ista dva znaka, dva značenja, ovisno o mjestu.

`PokupiParametre` (`Form2.vb` 3406) hoda `Controls` i lijepi:

- check: `@ime=CheckState`
- radio: `@ime=1|0`
- multidrop: `@ime='id1~id2~'`
- combo: `@ime=SelectedValue` ili `null`; uz to `@comboboxime='tekst'`
- search: `@ime=id` (Tag) i `@searchboxime='tekst'`
- grid: `@gridIme`, `@gridImeKolona`, `@nizplavihajdijevaIme`, `@nizoznacenihajdijevaIme`, `@rowcountIme`
- text: `@ime='...'`, apostrof postaje `¸¸`, zarez `~~`
- number: broj bez navodnika, prazno je `0`
- date: `@ime='yyyyMMdd'` ako je kvačica, inače `null`

Caption koji počinje s `_` ne ulazi u parametre (search: `SudjelujeUParametrima=False`).

## 8. Ispis

| Put | Stanje |
|---|---|
| `PRINTMASTER` / `PRINTREPXMASTER` | oba zovu `IspisGRIDREPX` (`Form1.vb` 4945–4956). Stari `IspisNovi` je komentar |
| `PRINTDET` | `IspisGRIDREPXdet` |
| `PRINTGRID` / `PRINTREPXGRID` na dijalogu | živo (`Form2.vb` ~6950 i ~6985) |
| Janus `GridEXPrintDocument` | živo, zaglavlje iz `PAGEHEADER*` |
| `GridExPrinting.dll` | referenca, x86 |
| `NetGenExtensions.frmGridExPrintingREPX` | late-bound |
| `Spin.Printing.v23` / `v9.1` | late-bound, `Class1` |
| `rptDevRepx` | late-bound, `Form6` |
| `LokalniIspisRDLC.vb` | cijela klasa komentirana, mrtvo |
| `Form4` ReportViewer | komentiran, mrtvo |

`::repx/ime(@p=@@x)` u captionu gumba nije Janus. To je REPX s parametrima. U korpusu 481 takvih komandi (dokument 03).

## 9. Ostale forme

| Forma | Uloga |
|---|---|
| `Form1` | podloga, master/detail Janus, stablo, router |
| `Form2` | generator dijaloga |
| `Form3` | prozor tražilice |
| `Form4` | mrtav SSRS |
| `Form5` | Ctrl+F11, CB log (zadnjih ~40 linija, zna sadržavati lozinku) |
| `Form6` | parametri ispisa + `rptDevRepx` |
| `Form7` | izbor jezika |
| `Form8` | prazna |
| `Form9` | izbor predloška Spin.Printing |
| `AppearanceDialog`, `FormatStyles` | izgled Janusa |
| `frmIspis` | DX 13 preview |

## 10. Što ovo znači za novi projekt

NetGen nije „grid koji treba zamijeniti DevExpressom“. On je interpreter. Zamjena samo `GridEX` i `NumericEditBox` ostavlja komande, SQL, DMS, fiskalizaciju i `ASSEMBLYLOAD` neportirane. Odluka iteracije 2: sve to ide u port.

## 11. Ugovor koji Jupiter stvarno zove

Izvor Jupitera nije u repou. Ugovor je ono što je `public` na `Loader` i `frm`.

`Loader` (`netgen/Loader.vb` 9–18) drži polja koja Jupiter postavi **prije** poziva: `moduleId`, `strNGParams`, `mainHandle`, `Ikona`, `Koler`. Nakon poziva čita `WCaption` i `hWnd`. `ShellResize(w, h)` (22–26) radi `SetWindowPos` na `hWnd`.

`Run8` (122–190): `On Error Resume Next`, `New Form1` u **istom** AppDomainu (AppDomain je zakomentiran 22.04.2024, linije 169–175), prepiše polja, zove `ob.Run(...)`. `Run8 = ob` je u komentaru, pa funkcija vraća `Nothing`. Stari `Run` (79–119) još radi `Run = ob` i diže AppDomain.

`Form1.Run8` (1087–1096) samo proslijedi u `Form1.Run` (1098). Taj `Run` složi `strXML2`, konekcije, `JupiterLog.Inicijaliziraj`, `Show` i `SetParent`. Lozinka ide u lokalne varijable `strSQLSERVERlogin` / `strSQLSERVERpassword` (1142–1146). CB log je u starom kodu znao ispisati password; novi port to ne smije u trajni log.

`frm.Run8` (72–119) isto, ali `New Form2`, plus `ulazniID` i `ulaznaForma`. `frm.Run` (35–69) **nema** `New` — instanca je zakomentirana. Ako Jupiter negdje još zove `frm.Run`, to je null. Živi put je `Run8`.

Potpis oba `Run8`:

```text
Run8(server, database, userId, password, operater As Int32,
     clientHandle As Int32, top As Short, left As Short, remoteDllPath As String) As Object
```

`Carpo.NetGen` koji Jupiter treba prepoznati mora imati klasu s tim poljima i tom metodom. Ime assemblyja (`NetGenDLL` naspram novog) je otvoreno pitanje 1 u sažetku.

## 12. DMS, baza, disk

`Form1.ProcitajXMLzaPodlogu` (3167–3286), red koji se mora kopirati:

1. `Assembly.Load("SpinDMSUtil")`, `CreateInstance("SpinDMSUtil.NetGenFinder")`, `GetFile(strXMLsakojimRadim)` (3172–3182). Greška ide u CB log, ne u korisnika. Na objektu se čitaju `source`, `exception`, `connstring`, `JupiterClientIniPath`.
2. Ako je `InnerXml` prazan i `Conn4` otvoren: ime fajla bez putanje, `select body from netgenxml where xml = '...'` (3210). `body` je `byte()`, dekriptira se `KriptirajDekriptiraj.EncryptDecrypt.Decrypt(v1, "tan123ge")` (3221–3228). Ključ je u izvoru, nije nova tajna.
3. Ako baza ne vrati XML: `m_xmld.Load` s diska (3262–3263). Greška tek tada ide u `MsgBox`.
4. `InterniXML` prazan nije: ugrađeni `Resource1` (3278).

`Form2` (oko 789–838) preskače DMS ako je XML već došao iz baze. `INCLUDE` (844–943) ponavlja DMS pa disk po svakoj putanji odvojenoj `#`.

Carpo `FileFinder("Carpo")` gleda drugi skup dokumenata. Poziv te klase za NetGen XML vratio bi krivi fajl ili ništa. Port zove `NetGenFinder`.

## 13. `TotalGenMultiSelectTemp`

Kad ime forme počinje s `multiselect` ili `multiblue` (`Form1.vb` 4932–5037):

1. `delete from TotalGenMultiSelectTemp where susersname = suser_sname()`.
2. `delete ... where operateriid = @@operateriid`.
3. Za svaki čekirani red (`multiselect`) ili svaki plavi red (`multiblue`) `insert into TotalGenMultiSelectTemp (id, hostname, susersname, operateriid) select <@@id>, host_name(), suser_sname(), <operater>`.
4. Onda se forma otvori. Procedura unutar forme čita tu tablicu. Nema parametra `@id` s listom.

Ako je čekiran nula ili jedan red i ime ipak počinje s `multiselect`, ubaci se samo trenutni `GetID()` (5003–5006).

Tablica je privremeni spremnik na SQL Serveru, ne kontrole. Dva Jupiter prozora istog operatera brišu jedan drugome redove. To je današnje ponašanje i ostaje (odluka 5). Carpo `foreachrow` to ne zamjenjuje.

U korpusu je 94 otvaranja forme čije ime počinje s `MULTISELECT` i 52 s `MULTIBLUE` (heuristika brojača, dokument 03).

## 14. `ASSEMBLYLOAD`

Oblik: `ASSEMBLYLOAD(ImeAssembly.ImeKlase)`.

Podloga, `NovaGenerickaForma` (`Form1.vb` 4584–4623):

- assembly je sve lijevo od zadnje točke (`Substring(0, InStrRev-1)`)
- `CreateInstance` cijelog imena
- `o.totalgen(id, operateriid, connection, zadnjiSqlMastera, parametriGrida)`
- connection dobije prefiks `Driver={SQL Server};` ako ga već nema

Ista komanda u `Form1.Rafalno` (6681 i `:ASSEMBLYLOAD` 6602) razlikuje se u zadnja dva argumenta (zadnji SQL naspram praznog, ili parametri grida). Dijalog (`Form2.vb` 7424–7456) zove `totalgen(id, operater, connection, strParametriSvi, strParametriSvi)` — parametri forme, ne SQL mastera.

Ako `Load` ili `CreateInstance` padne, ide CB log i `Exit Sub` (prekida ostatak lanca). Nema sučelja u repou: klasa je tuđi DLL koji mora imati javnu `totalgen` s pet argumenata. Port ne smije to pretvoriti u Carpo `comdll` (drugi potpis). U korpusu 55 pogodaka. Najmanji primjeri: `definitions/netgen/BIOVEGA/MPNGPregDokMAIL.xml`, `Bilanca-GFI.xml`.

## 15. Fiskalizacija

Jedna metoda `Form1.Fiskalizacija(id, tablica)` (`Form1.vb` 286). Dijalog je prosljeđuje (`Form2.vb` 7459–7493).

| Komanda | Tablica | Napomena |
|---|---|---|
| `FISKALIZACIJA(id)` | 1 | račun, `mpblagajnagla` |
| `FISKALIZACIJAN(id)` | | `FiskalizacijaNapojnica` (615), isti CIS, drugi XML čvor |
| `FISKALIZACIJAFAKTURE(id)` | 2 | |
| `FISKALIZACIJABSRACUNI(id)` | 3 | |
| `FISKALIZACIJAPD(id)` | 4 | logara, id je `mpponudaglaid` |
| `FISKALIZACIJAPONUDE(id)` | 5 | |
| `POSLOVNIPROSTOR(id)` | | `PoslovniProstor` (809), ne tablica |
| `SLOPP(id)` | | tijelo komentirano (`Form2.vb` 7494–7498), ne radi |

Tok `Fiskalizacija`:

1. `select * from mpfiskalizacija` s `where` koji ovisi o tablici (296–306). Prazno ili datum u budućnosti: izlaz, samo CB log.
2. Kolone `pfx`, `lozinkazapfx`, `url`, `timeout`, `PutanjaZaZahtjeveiOdgovoreOdPorezne`.
3. Ako URL sadrži `fu.gov.si`: `Assembly.Load("SLOFISKAL")`, klasa `SLOFISKAL.Program`, metoda `SLORacun(id, tablica)` (341–354).
4. Inače `Assembly.Load("Raverus.FiskalizacijaDEV.COM")`, klasa `Raverus.FiskalizacijaDEV.COM.CentralniInformacijskiSustav` (360–368). Mape zahtjeva/odgovora pod `C:\FISKALIZACIJA\RACUNI\...` ili pod putanjom iz tablice (497–504).

Na podlozi, ako je čekirano više redova, `FISKALIZACIJA` se zove za svaki (`Form1.vb` 4643–4664). To nije `FOREACH*`.

Carpo `FiscalOutlookActions` (`ZKI`/`JIR`) je drugi kod i drugi XML. Ne zamjenjuje ovaj put. DLL-ovi `Raverus.FiskalizacijaDEV.COM` i `SLOFISKAL` nisu u repou; trebaju na stroju u runtimeu (dokument 09). U korpusu najmanji pogodak je `FaktureNeFisk.xml` (11 kontrola, ima `FISKAL`).

`POSALJIRACUNENASERVER`, `POSALJIZAKLJUCKENASERVER`, `POSALJIPORUKENASERVER` i `BLAGAJNAENABLE` su susjedne komande (blagajna/Mlinar), ne Raverus. Ostaju u portu komandi jer su u istom `Rafalno`, ali nisu fiskalni CIS. U korpusu su rijetke (`BLAGAJNAENABLE` 3). Njihov vanjski DLL u ovoj fazi nije identificiran izvan `Class1.BlagajnaEnableZaMlinar` — **neprovjereno** što ta metoda učitava.
