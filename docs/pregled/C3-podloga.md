# C3: podloga (Form1) → `Carpo.NetGen.Ui.Podloga`

Usporedba `Form1.vb` (NovaGenerickaForma 4543, NovaGenerickaForma2 4925, NovaGenerickaForma1 5062, Rafalno 6546, PokaziUpit 1825, CellUpdated 6470/7063, EditingCell 7803, ssPermission 2665-2790) s podlogom u Carpu (DevExpress 26.1). Brojevi su iz korpusa `definitions/netgen` (605 PROJECT datoteka).

Pravilo 50-50: kad NetGen i Carpo rade drukčije, a nijedno nije očito ispravno, ostaje Carpo. Kad je NetGen ponašanje ono što korisnik vidi i na što se XML oslanja, Carpo ga prati.

## Dva lanca na podlozi

NetGen podloga ima dva različita izvršitelja lanca `a#b#c`:

| | `NovaGenerickaForma` (4543) | `Rafalno` (6546) |
|---|---|---|
| Tko zove | gumbi CMDn, MASTERDBLCLICK, DETAILDBLCLICK, desni klik bez `~`, čvor stabla | ONCELLEDITED, ONCELLEDITEDDET, BEFORECELLEDITED, ONCLOSED, desni klik s `~` |
| `~` | postaje `#` | (izbornik ga pretvori u `#` i doda `:` sprijeda) |
| Obični segment | `NovaGenerickaForma2` (otvori formu) | `cls1.ExecSql` (izvrši SQL) |
| `select …` | izvrši, prvi red postavlja Button1..10 po natpisu | izvrši kao SQL |
| `@@` u segmentu | ne dira (rade forme) | `PokupiParametreSaGrida` + `SrediSQLPosljeNextForme` |
| FOREACH* | nema grane (ide kao forma) | preskoči sljedeći segment, bez petlje |
| OK, CANCEL, CLOSE, CLOSEPARENT | — | `Button1_Click`/`Button2_Click(grid)` → `NovaGenerickaForma(Nothing)` → ništa |

Carpo je imao samo prvi (`IzvrsiLanac`), pa su desni klik s `~` i događaji ćelija išli kao otvaranje forme.

## Ulaz granule (RUT-01)

NetGen ima jednu podlogu (Form1) za svaku granulu. Bez stabla i detalja ti se paneli sakriju; ONLOAD `:Forma` (Form1.vb 3879) otvori dijalog modalno nad podlogom, a FILLEXPLORER dijaloga puni master grid (5445-5453).

Carpo je imao tri ulaza: Podloga samo za granule sa stablom, detaljem i gumbima (132), skraćenu `ObicnaForma` (235 granula: bez izbornika, lanaca, uređivanja, dozvola, zbrojeva, DMS-a…) i samostalan dijalog za `:Forma` (230 granula; 228 ulaznih formi ima FILLEXPLORER, pa rezultat filtra nije imao gdje). Klik-pregled je već sve otvarao u Podlozi.

Sada `Prozor.Otvori` svaku granulu otvara u Podlozi; `ObicnaForma` i `OdabirForme` su uklonjeni. Ako se želi Carpo izgled bez Manager okvira za jednostavne granule, to je odluka izgleda, ne funkcionalnosti.

## Nalazi

| ID | Korpus | NetGen | Carpo prije | Sada |
|---|---:|---|---|---|
| **RAF-01** | desni klik s `~`, ONCLOSED 4, ONCELLEDITED 22 | `Rafalno`: SQL segment se izvrši, greška (`GreskaKodIzvrsavanjaSQLa`) prekida lanac | `IzvrsiLanac`: SQL segment je išao u `OtvoriFormu` ("nema forme" + osvježi) | `IzvrsiRafalno` (`Komande/Form1Rafal`, `Ui/PodlogaRafal`) s istim granama i redom. Upis prolazi kroz `DijalogTok.SmijePoslati`, kao događaji dijaloga |
| **EDT-01** | EDITCOLUMNS 30 granula, EDITCOLUMNSDET 1 | `CreateGridColumns(..., strEditColumns)`: stupci iz popisa su TextBox/CheckBox, ostali NoEdit | grid podloge uvijek `ReadOnly`, EDITCOLUMNSDET se nije čitao | grid je uređiv kad ima takvih stupaca, ostali stupci `AllowEdit=false`. EDITCOLUMNS sa `sp`/`select` se izvrši jednom (3468) |
| **EDT-02** | ONCELLEDITED 22, ONCELLEDITEDDET 1 | `CellUpdated`: @@EDITEDCOLUMN (natpis), @@LASTVALUE, parametri ćelije `@ime_s_podvlakom='v'` (zarez → točka), red dobije kvačicu, `SrediSQLPosljeNextForme(..., IdiUclipbord=False)`, `Rafalno`; greška vrati staru vrijednost | ništa se nije slalo | isto. Kvačica (bool) šalje odmah kao Janus (`PostEditor`) |
| **EDT-03** | BEFORECELLEDITED 0 | `EditingCell`: lanac prije uređivanja, greška otkazuje | — | isto (master). BEFORECELLEDITEDDET NetGen čita, ali nigdje ne koristi |
| **BTN-03** | `select` u lancu gumba | izvrši SQL, `DodatnoPostimajKontrolu(ButtonN, red(ButtonN.Text))` | `Lanac.GumbIzCase` je procjenjivao CASE iz teksta bez baze | izvrši SQL, isto pravilo kao ONLOAD2 (`PrimijeniStanjeGumba`). Bez veze ostaje stara procjena |
| **REF-01** | REFRESHMASTERGRID, osvježavanje nakon OK | `PokaziUpit(strZadnjiSQLsakojimjepunjenGrid)`: zadnji SQL, s filtrom forme | `_master.Ucitaj`: ONLOAD ispočetka, filter se gubio | zadnji SQL (`Stanje.SqlZaSlanje`), ONLOAD samo ako ga nema |
| **FOK-01** | MASTERMOVELAST 0 u podlozi, `movetoid` | nakon punjenja red s istim prvim stupcem kao prije, pa `movetoid`, pa MASTERMOVELAST | fokus uvijek na prvi red | kao NetGen |
| **MUL-01** | CHECKBOXVISIBLE=TRUE ~70 | `NovaGenerickaForma2`: više kvačica → forma za svaki red, osvježava samo zadnji (`Zadnji`) | forma jednom, za fokusirani red | kao NetGen (CheckBoxRowSelect). MULTISELECT/MULTIBLUE i dalje jednom |
| **MSL-01** | MULTISELECT | bez kvačica ubaci `GetID()` trenutnog reda u TotalGenMultiSelectTemp | bez kvačica nije ubacio ništa | trenutni red |
| **DOZ-01** | PERMISIO=true 29 | `ssPermission` za granulu i operatera: gumbi (enable/vidljivost), stavke desnog klika, forme (`ssPermissionProvjeriRafalnuKomandu`), stalne stavke (Export grid, Grupiraj, Ispis). Red s dopuštenjem pobjeđuje | ignorirano | `Komande/Dozvole` s istim pravilima (uklj. DBNull pod On Error Resume Next). PERMISSIO (2 granule, dva S) se ne čita ni u NetGenu |
| **MSG-01** | svaki MSGBOX, svaka SQL greška | `MsgBox` (ExecSql 186/200/203, Form2 7165) | `PorukaNaredba.Prikazi` je bio null izvan testa: MSGBOX se nije vidio | `Ui/NgPoruka` (XtraMessageBox); poruka greške kao `ExecSql` (`#`/`+` skraćuje, `#OK` nije greška, ssPermission/NetGenDBFilter… tiho). Na CI-u i u klik-pregledu samo dnevnik |
| **DBL-02** | DETAILDBLCLICK 19 | `NovaGenerickaForma(strDBlClickDetForm)` (lanac) | `OtvoriFormu` (samo forma) | `IzvrsiLanac` |
| **ONC-01** | ONCLOSED 4 | `Form1_FormClosed`: `Rafalno(strONClosed)` | ignorirano | isto |
| **DMS-01** | DMSDET 9 | kartica "DMS (n)" uz stavke (GridEX3), puni se nakon detalja; desni klik Pogledaj / Spremi na disk / Obriši / Preuzmi za potpis / Vrati potpisano / Odustani; dvoklik Pogledaj (DmsManagerV1 kasnim vezanjem) | nije postojalo | `Ui/PodlogaDms` + `Poziv/DmsPoziv`, iste metode i SQL; argumenti se pretvore u tipove parametara kao VB late binding |
| **ENT-02** | ONENTERKEY 2 | `GridEX1_KeyDown` Enter → `NovaGenerickaForma(ONENTERKEY)` | ignorirano (čitao ga je samo dijalog) | isto, osim dok se ćelija uređuje |
| **KEY-01** | sve granule | `Form1_KeyDown`: Escape zatvara podlogu | Carpo `JupiterShared_KeyDown` čita `DefinitionForm` koji podloga nema: uz KeyPreview svaka tipka baca NullReferenceException | podloga sama obrađuje tipke (`OnKeyDown`), Escape zatvara osim dok se uređuje ćelija ili filter red |
| **HLP-01** | HELPKOD 16 | F1 i gumb "?" pokreću `JupiterHelp.exe -s -d -h -o -p` | ignorirano | isto (bez exe-a samo dnevnik, kao On Error Resume Next) |
| **MSG-02** | svaki SQL u dijalogu | `ExecSql` pokaže grešku, `GreskaKodIzvrsavanjaSQLa` prekida lanac (Form2 7511), "#OK" ne | `Podloga.Citaj` je gutao grešku: lanac dijaloga je išao dalje (i OK zatvarao formu), poruka se nije vidjela (EVT-03 je radio samo s čitačem koji baca) | `Dijalog.Omotaj`: poruka kao ExecSql, "#OK"/tiha greška vraća null, ostale se bace pa lanac stane. Podloga i ObicnaForma daju dijalogu čitač koji baca |
| **ONL-02** | ONLOAD forme | `ProcitajXML` 1199: greška na ONLOAD → `Exit Sub`, Form1 5352 ne otvara formu | forma se otvarala s praznim poljima | forma se ne otvara (`Dijalog.GreskaOnLoad`) |
| **TOT-01** | TOTALROWVISIBLE/DETTOTALROWVISIBLE (ukupni red u ~236 granula), AGREGATEFUNCTION 642 stupca | ukupni red; decimalni stupac zbraja (`#,##0.00`), COLUMN AGREGATEFUNCTION 0 None, 1 Count, 2 Sum, 3 Average, 4 Min, 5 Max | MasterGrid je računao `TotalRed`, ali podloga nije imala footer ni zbrojeve | footer i sažeci (`Grid/GridAgregat`, `Ui/PodlogaGrid`); značenje brojeva potvrđeno na korpusu (2 su količine i vrijednosti, 3 cijene i "pros.") |
| **SRT-01** | AUTOMATICSORT=false 7 | `GridEX1.AutomaticSort = False` | izračunato, nije primijenjeno | `AllowSort=false` |
| **FRZ-01** | FROZENCOLUMNS 5/8/13 u 12 granula | `GridEX1.FrozenColumns` (broji ikonu i kvačicu) | izračunato, nije primijenjeno | toliko podatkovnih stupaca lijevo `Fixed` |
| **MSG-03** | poruka s `#close`/`+close` | `ZelimIzaciIzForme` zatvara formu (Form2 7512) | nije postojalo | greška SQL-a u lancu dijaloga s `#close` zatvori dijalog |
| **COL-01** | MASTERCOLUMNS/DETAILCOLUMNS (1 634 stupca, 107 granula), CLEARCOLUMS TRUE u 22 grupe | `CustomFormatKolona` (Form1 7451): CLEARCOLUMS skrije sve, pa pozicija po redu definicije; VISIBLE odlučuje | master je nakon VISIBLE prisilno palio stupac (8 stupaca s VISIBLE=FALSE se vidjelo); detail nije primjenjivao FORMAT, AGREGATEFUNCTION, INDEX ni redoslijed; nitko nije primjenjivao Position | jedan `GridDefinicija` za master, detail i grid dijaloga; Position kao Janus (stupac se premjesti, ostali se pomaknu) → `VisibleIndex` |
| **COL-02** | HEADERALIGNMENT 1/2 (110 grupa, 70 granula); brojčani stupci | Janus TextAlignment 1 Near, 2 Center, 3 Far, samo na opisanim stupcima; detail dobije poravnanje **mastera** (`CustomFormatKolona` čita polje `strHeaderAlignment`, parametar je neiskorišten); Class1 1066/1112/1170: zaglavlje boolean/int/decimal je Far | "1"/"2" se nisu prepoznavali; master je poravnanje stavljao na sve stupce | kao VB |
| **COL-03** | MAXLINES, AUTOSIZE, FORECOLOR/BACKCOLOR (lightyellow, 2 granule), NEGATIVERED, NOTZERORED, `!` u natpisu (grupiranje) | WordWrap/MaxLines, AutoSize, CellStyle, FormatCondition crveno, `RootTable.Groups` | MasterGrid je računao, podloga nije primjenjivala | `RowAutoHeight` + prelamanje, `BestFit`, boja ćelije, `RowCellStyle` crveno, `GroupIndex` |
| **GRB-01** | stupci upita `RowForeColorID`+`RowColorID`, `<ime>CellForeColorID`, `<ime>CellBackColorID`, `<ime>MaxLinesID` (vraćaju ih procedure, u XML-u ih nema) | `GridEX1/2_FormattingRow` (Form1 7875, 8026) i `glblGRID_FormattingRow` (Form2): HTML boja reda i ćelije, prelamanje stupca | nije postojalo | `RowCellStyle` na podlozi i gridu dijaloga; ako su zadane obje boje ćelije, vrijede obje (VB zadrži zadnju po redu stupaca). `ikonaid` (ikona reda) ne: Carpo izgled |
| **TRE-02** | dvoklik na čvor stabla | `TreeView2_DoubleClick` 5656: `NovaGenerickaForma` za odabrani čvor, kao AfterSelect | samo promjena čvora je radila nešto, ponovni klik na isti čvor ništa | dvoklik ponovi radnju čvora (`IzvrsiCvor`) |
| **GRL-01** | „Spremi postavke grida” / „Reset grida” u izborniku svakog grida (podloga i dijalog), FILTERSAVE (2), MULTIGRIDLAYOUT (39) | `SaveGridLayout`/`LoadLayout`/`DeleteGridLayout` (Form1 6192-6420, Form2 9205-9243): datoteka u `%APPDATA%\SPIN Informatica\Jupiter Software\Grid Settings`, `GridMaster`/`GridDet`/`GridLodani-…` + ime XML-a (+ forma uz MULTIGRIDLAYOUT) + jezik; za DevExpress grid NetGen piše `.xml` (`SaveLayoutToXml`) | stavki nije bilo, izgled se nije pamtio | `GridRaspored`: ista imena s `.xml`, učitavanje nakon slaganja stupaca, bez FILTERSAVE se filter briše; Reset obriše datoteku i složi stupce iznova; MASTERGRIDLAYOUTFILE (3) daje ime datoteke mastera. Janus `.gxl` se ne čita. AUTOFITCOLUMNS (84) NetGen ne čita |
| **ULZ-01** | Jupiter ulaz `asm:Carpo.NetGen.Loader:<xml>` (132 retka ssMeni danas su `NetGenDLL.loader`) | `Loader.Run8` → `Form1.Run`: učita XML, `SetParent(clientHandle)`, `Show`, `hWnd` | Run8 je bio M0: učitao model i vratio, prozor se nije otvarao, pa prebacivanje retka na Carpo ne bi ništa prikazalo | Run8 otvori istu Podlogu kao TestShell: Logger u DependencyContainer, `InitLogger`, `MainHandle`/`ParentHandle`, `Base.Start()` (SetParent + Show), `hWnd`. TestShell i Granule test postave `BezProzora`. `asm:…frm` (9 redaka, ULZ-02): Run8 otvori samostalni dijalog `ulaznaForma` (inače MAINFORM) s ID-om `ulazniID`, kao `Form2.Run`; greška ONLOAD-a ga ne otvori; čitanje ima ista pravila kao podloga (`FrmCitac`). `strNGParams` (ULZ-03) ide u lanac kad se Podloga pokaže, kao `Form1_Load` 1682-1686 |

## Otvoreno

| ID | Što | Zašto nije sada |
|---|---|---|
| FOR-02 | FOREACH* na podlozi: NetGen ne vrti petlju (Rafalno preskoči tijelo, NovaGenerickaForma ga otvori kao formu); Carpo `IzvrsiLanac` vrti čitanja | treba odluka: VB ponašanje je praktički "ne radi ništa" |
| TREE-SQL | čvor stabla s `select`/`sp`/`exec`: Carpo puni grid, NetGen `NovaGenerickaForma` (forma ili gumbi) | u korpusu čvorovi nose imena formi; provjeriti s novih 500-600 granula |
| PRV-01 (TRANSLATECOLUMNS, TREETRANSLATE, `Translate` natpisa) | jezik iz `jupitersystem..loginimages` / `MPKase` (Form1 1418-1440), pa `JupiterTranslator.Translator.TranslateString(tekst, "NetGen")` za natpise forme, gumbe, izbornike, stablo i ćelije iz TRANSLATECOLUMNS (2) | `Prevod`/`Prevoditelj` postoje, ali jezik i prevoditelj nisu spojeni, pa je sve na izvornom jeziku (za hrv i prazan jezik isto kao NetGen). Carpo ima `Carpo.Common.Translator` (isti JupiterTranslator, Init sa serverom iz Jupiter ini-ja); spojiti kad se zna da neki klijent radi na drugom jeziku |
| SQL-01, B-JUP | upis na jupiter2019/2022 | čeka korisnika |
