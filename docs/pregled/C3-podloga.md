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

## Otvoreno

| ID | Što | Zašto nije sada |
|---|---|---|
| FOR-02 | FOREACH* na podlozi: NetGen ne vrti petlju (Rafalno preskoči tijelo, NovaGenerickaForma ga otvori kao formu); Carpo `IzvrsiLanac` vrti čitanja | treba odluka: VB ponašanje je praktički "ne radi ništa" |
| TREE-SQL | čvor stabla s `select`/`sp`/`exec`: Carpo puni grid, NetGen `NovaGenerickaForma` (forma ili gumbi) | u korpusu čvorovi nose imena formi; provjeriti s novih 500-600 granula |
| MULTIGRIDLAYOUT | 42 granule: Janus layout iz datoteke | Carpo uvijek gradi stupce ispočetka (50-50 → Carpo) |
| FILTERSAVE, MASTERGRIDLAYOUTFILE | 2 / 3 | dio Janus layout datoteka (.gxl), kao MULTIGRIDLAYOUT. AUTOFITCOLUMNS (84) NetGen uopće ne čita |
| TRANSLATECOLUMNS | 2 | `Translate` vrijednosti ćelija; bez jezika (hrv) ne mijenja ništa, Carpo nema prevoditelja za podlogu |
| SQL-01, B-JUP | upis na jupiter2019/2022 | čeka korisnika |
