# OznakaKL — očekivano ponašanje

Izvor XML-a: `definitions/netgen/BIOVEGA/OznakaKL.xml` (windows-1250). Stavka `OKL-00`: kopija `NetGenBot/granule/Biovega/OznakaKL.xml` mora imati isti SHA-256. Ponašanje je iz NetGen VB-a. `Carpo.NetGen` se ne uzima kao izvor istine.

Jedinica na formi dijeli se s `FakPretvorbe = 14` (`netgen/Form2.vb:58`, `netgen/Module1.vb:65`). Dodjela u `Integer` ide VB zaokruživanjem na najbliži parni. XML u atributu pretvara novi red u razmak (pravilo XML-a), pa `ONSUBMIT` nakon `XmlDocument.Load` nema prijelom retka.

Izgled je Carpo (R59): ribbon, DevExpress kontrole, Carpo skin. Stavke označene **izgled – Carpo** ne ruše QA. Natpis i redoslijed gumba, naredba, širina, skriveni stupac, sort, edit, checkbox, raspored na dijalogu, tab, enable/visible i fokus ostaju.

Stavke `OKL-01`–`OKL-27` parser i čitanje baze mogu proći odmah. `OKL-30` nadalje traže izvedbu. Dok je `Carpo.NetGen.TestHooks.GranulaSesija.Izvrsi` null, test ih broji kao otvorene. Ključ u stupcu „ključ” je ime u `Pogled.Stavke`.

## Podloga (Form1)

Prozor je uvijek isti kostur (stablo lijevo, master, detail), ali prazan `TREE` taj kostur **sakrije**, ne ostavi prazno stablo na ekranu.

| ID | Očekivano | Ključ | NetGen |
|---|---|---|---|
| OKL-01 | Datoteka se učita kao `<PROJECT>`. Greška loada je `XML LOAD ERROR`. | | `Form1.vb:3263-3265` |
| OKL-02 | Naslov je `Pregled i ažuriranje dodatne oznake klase robe` (tekst djeteta `CAPTION`). | | `Form1.vb:3522-3528` |
| OKL-03 | Element `NAME` (`DODKLAZUR`) nije naslov. | | `Form1.vb:3522` |
| OKL-04 | Nema `VERSION`, pa nema sufiksa ` v[…]` ni statusne oznake verzije. | `ikona` je posebno | `Form1.vb:3524-3536` |
| OKL-05 | `ONLOAD` na projektu ne postoji, pa master SQL ostaje `STARTUPSTORA`. Drugi red bi prepisao prvi da atribut postoji (`On Error Resume Next`). | | `Form1.vb:3367-3368` |
| OKL-06 | SQL, doslovno: `select klmasterrobaid, sortkey, Naziv, oznaka, oznaka2 as DodatnaOznaka from klmasterroba order by sortkey` | | `Form1.vb:3882` |
| OKL-07 | Atribut `TREE` ne postoji. | | `Form1.vb:3387` |
| OKL-08 | Atribut `DETAILGRID` ne postoji. | | `Form1.vb:3373` |
| OKL-09 | `MASTERDBLCLICK=FRMEDIT` (prepisuje `DBLCLICKFORM` da postoji). | | `Form1.vb:3388-3389` |
| OKL-10 | `CMD1CAPTION=Izmjena:FRMEDIT` → natpis `Izmjena`, tag `FRMEDIT`. Lom je prva dvotočka. | `gumb.1` = `Izmjena\|FRMEDIT\|true` | `Form1.vb:3314-3325`, klik `5664-5667` |
| OKL-11 | Nema `CMD2`–`CMD10`: tekst prazan, gumb skriven. | | `Form1.vb:3827-3837` |
| OKL-12 | `GROUPBYBOXVISIBLE=FALSE`, `FROZENCOLUMNS=0`, `CHECKBOXVISIBLE=FALSE`, `TOTALROWVISIBLE=FALSE` ostaju takvi. | | `Form1.vb:3350,3362,3459-3460` |
| OKL-13 | `GRIDALLOWEDIT=TRUE` i `GRIDALLOWEDITCOLUMNS=oznaka2,oznaka` se **ne čitaju**. Edit ne uključuju. Engine čita samo `EDITCOLUMNS`. | | `Form1.vb:3466`; uzorak samo u `Resource1.Designer.vb:75` |
| OKL-24 | U modelu master SQL postoji, SQL stabla i detaila ne. | | `Form1.vb:3367,3373,3387` |

### Redoslijed otvaranja

`ProcitajXMLzaPodlogu` (`Form1.vb` od 3065, `On Error Resume Next` na cijeloj proceduri):

1. Sakrije gumbe 1–6 i počisti stanje (`3081-3148`), uključujući `strCHECKBOXVISIBLE=""` (nije više početni `"true"` s polja, red 55).
2. Učitava XML: DMS → `netgenxml` → disk. Test s diska ide na `Load` (`3263`).
3. `MASTERCOLUMNS` / `DETAILCOLUMNS` ne postoje: `CLEARCOLUMS` i `HEADERALIGNMENT` ostanu `""` (`3294-3309`).
4. Gumbi, pa `GROUPBYBOXVISIBLE`, `FROZENCOLUMNS`, `STARTUPSTORA`, pa `ONLOAD` koji ne postoji (`3312-3368`).
5. `COMBO` prazan → `ComboBox1.Visible = False` (`3340-3347`).
6. Naslov iz `CAPTION` (`3522`).
7. Nema `MENU` → `MenuStrip1.Visible = False` (`3552-3553`).
8. Nema `POPUP` → kontekst se očisti, zatim se **ipak** dodaju standardne stavke (`3620-3673`).
9. Nema `BUTTONS`, pa CMD gumbi ostaju. Vidljivost po tekstu (`3827-3848`). Širina 79, razmak 8 px i `AutoSize` su **izgled – Carpo**: gumbi podloge su u ribbonu, ne na dnu. Natpis, redoslijed i naredba ostaju.
10. Detail splitter na pola visine (`3852`), pa kasnije, jer nema detail SQL-a, na `Height - 50` (`2111-2113`).
11. `SETDEFAULTS` nema, petlja se preskače (`3862`).
12. `NapuniDrvoVrijeme`: `TREE` prazan i `COMBO` prazan → `TreeView2.Visible = False`, `SplitContainer1.SplitterDistance = 0`, `ButtonZaDrvo.Enabled = False` (`4310-4316`).
13. `PERMISIO` nije u XML-u. Prije petlje je `strPermisio = True` (`3153`), atribut ga ne prepiše, pa se zove `ssPermissionProcitaj` (`3414-3416`). SQL je select nad `ssPermission` za ime datoteke (`2706`). Poruka greške koja sadrži `sspermission` se guta (`Class1.vb:162-163`).
14. `strStartupStora` ne počinje s `:`, pa `PokaziUpit` (`3879-3882`). `@@` u ovom SQL-u nema, pa `ExecSql` ne preskače (`Class1.vb:113-118`).
15. `DXGRID` nije zadan (reset `3103`). NetGen tada crta Janus i sakrije DevExpress grid (`1906-1908`). **izgled – Carpo:** crta se DevExpress. Grid i dalje puni područje mastera.

| ID | Očekivano | Ključ | NetGen |
|---|---|---|---|
| OKL-30 | Trag SQL-a otvaranja je samo master select. Detail se ne zove. | `sqlTrag` | `Form1.vb:3862-3883,2104-2113` |
| OKL-31 | Stablo nije vidljivo. | `stabloVidljivo=false` | `Form1.vb:4310-4316` |
| OKL-32 | Lijevi splitter je 0. | `lijeviSplitter=0` | `Form1.vb:4314` |
| OKL-33 | Gumb drveta disabled. | `gumbDrvoEnabled=false` | `Form1.vb:4315` |
| OKL-34 | Combo podloge skriven. | `comboVidljiv=false` | `Form1.vb:3346` |
| OKL-35 | Detail SQL prazan (područje se stisne na visinu−50, ne puni se). | `detailSql` prazan | `Form1.vb:2111-2113` |
| OKL-36 | Izbornik skriven. | `menuVidljiv=false` | `Form1.vb:3552-3553` |
| OKL-37 | Help skriven (nema `HELPKOD`). | `helpVidljiv=false` | `Form1.vb:3491-3494` |
| OKL-81 | Nema `TIMER`: statusna oznaka timera je skrivena. | `timerVidljiv=false` | `Form1.vb:3449-3450` |
| OKL-38 | **izgled – Carpo.** NetGen sakrije DX grid. Port crta DevExpress. | | `Form1.vb:1906-1908`, R59 |
| OKL-76 | Select nad `ssPermission` uđe u trag. Vrijednost ključa je oznaka, ne cijeli SQL. | `permisioSql=ssPermission` | `Form1.vb:3153,2706` |
| OKL-77 | Kontekst: Spremi postavke grida, Reset grida, Export grid, Grupiraj / razgrupiraj, Format style, Izgled grida, Ispis REPX, Ispis. | `kontekst` | `Form1.vb:3625-3673` |
| OKL-80 | Nema `ICON`: ikona je ona s hosta (`Ikona`), ne `Graphics\Icons\`. | `ikona=host` | `Form1.vb:3430-3436` |
| OKL-60 | Jedini vidljivi gumb podloge je Izmjena → `FRMEDIT`. | `gumb.1=Izmjena\|FRMEDIT\|true` | `Form1.vb:3827,5667` |

## Master grid

`CreateGridColumns` (`Class1.vb:982`) radi kolone iz rezultata. Zatim `SetDataBinding` (`Form1.vb:1999`). Nema `MASTERCOLUMNS`, pa `CustomFormatKolona` odmah izađe (`7454-7455`).

Stupci, ovim redom:

1. `Icon`, nevezan, širina 22, `Visible=False` (`Class1.vb:1010-1020`). **izgled – Carpo.** Port nema Janus ikonu stupca.
2. `Selector`, checkbox, širina 22, `ActAsSelector=True` (`1022-1027`). `CHECKBOXVISIBLE=FALSE` ga sakrije: `RootTable.Columns(1).Visible = False` (`Form1.vb:2043-2046`). Usporedba je točno `"FALSE"` nakon `ToUpper`. Skriven checkbox ostaje.
3. Podaci redom iz SQL-a. Ime kolone je ime (alias) stupca.

Pravila po tipu (`Class1.vb`, `InStr` na ime .NET tipa):

- `int`: širina 34, format `##0`, `EditType=NoEdit` jer `EDITCOLUMNS` prazan (`1087-1139`). Poravnanje `Far` je **izgled – Carpo**.
- `string`: širina **100**, ne 200. 200 je samo u `DodatnoFormatirajGridnakonRetriveStructure` (`Form1.vb:1799`), a taj put je za `self`/`multi`, ne za ovu granulu (`1201-1239`).
- `decimal`/`double`: širina 44, `#,##0.00`, `Far`, agregat Sum (`1146-1172`).
- `date`: širina 100, `dd.MM.yyyy` (`1247-1270`).
- Ime završava na `ID` (zadnja dva znaka, bez obzira na veličinu) → `Visible=False` (`1124-1126`). `klmasterrobaid` je skriven. `DodatnaOznaka` nije.
- Zadnja 4 znaka natpisa ako su broj: širina = taj broj / 14, natpis se skrati. Ovdje nema takvog imena.
- `EditMode = EditOff` (`1003`). `AllowEdit = False` samo ako je `FILTERMODE=true` (`Form1.vb:2169-2178`). `FILTERMODE` nema, pa ta grana ne dira `AllowEdit`. Edit ipak ne radi jer je svaki `EditType` `NoEdit`.
- `EnterKeyBehavior = None` jer je `EDITCOLUMNS` prazan (`3419-3421`). `ENTERKEYBEHAVIOR` ne postoji, dodjela padne, ostane `None`.
- Zatim `TotalRow = True` i `HeaderLines = 3` (`1363-1364`). Poslije toga `TOTALROWVISIBLE=FALSE` vrati `TotalRow` na False (`2049-2050`). Total ostaje ugašen. `HeaderLines` je **izgled – Carpo**.
- `EmptyRows = True` (`1000`) je **izgled – Carpo**.
- Designer, koji XML ne gazi: `SelectionMode = MultipleSelection` (`Form1.designer.vb:654`) i `IncrementalSearchMode = AllCharacters` (`641`) ostaju. `RowHeaders = True` (`655`) i `HideSelection = Highlight` (`640`) su **izgled – Carpo**.
- `GroupByBoxVisible = False` (`3350`). `FrozenColumns = 0` (`3362`).
- `AUTOMATICSORT` nije `false`, pa se sort klikom na zaglavlje **ne gasi** (`3401-3404`). Početni poredak je poredak SQL-a (`ORDER BY sortkey`), grid sam ne primjenjuje drugi sort (`1999`).
- Group header total: `AggregateFunction.Count`, prefiks `[`, sufiks ` ]` (`2136-2142`).
- `GridEX1.Dock = Fill` (`1906`).
- Ako ima redova: fokus na grid i `EnsureVisible` (`2059-2072`).
- `@@id`: `GetID` traži ćeliju `id`. Nema je, pa uzme `Cells(2)` — prvi podatak, `klmasterrobaid` (`5566-5582`). To je Janus put. DX put (`GetDXID`, prvi field) se ne zove jer `DXGRID` nije `true`.

| ID | Ključ | Vrijednost |
|---|---|---|
| OKL-39 | `selectorVidljiv` | `false`. Icon 22 je **izgled – Carpo** |
| OKL-40 | `kolona.klmasterrobaid.vidljiva` | `false` |
| OKL-41 | `kolona.klmasterrobaid.format` | `##0` |
| OKL-42 | `kolona.Naziv.sirina` | `100` |
| OKL-43 | `kolona.DodatnaOznaka.natpis` | `DodatnaOznaka` |
| OKL-44 | `totalRow` | `false` |
| OKL-45 | | **izgled – Carpo.** NetGen `HeaderLines=3` |
| OKL-46 | `groupByBox` | `false` |
| OKL-47 | `frozenColumns` | `0` |
| OKL-48 | `automaticSort` | `true` |
| OKL-49 | `editType` | `NoEdit` |
| OKL-50 | `editMode` | `EditOff` |
| OKL-51 | `enterKey` | `None` |
| OKL-52 | `selectionMode` | `MultipleSelection` |
| OKL-53 | `incrementalSearch` | `AllCharacters` |
| OKL-54 | | **izgled – Carpo.** NetGen `EmptyRows=true` |
| OKL-55 | | **izgled – Carpo.** NetGen row header |
| OKL-56 | | **izgled – Carpo.** NetGen `HideSelection=Highlight` |
| OKL-57 | `gridDock` | `Fill` |
| OKL-58 | `fokus` | `grid` |
| OKL-59 | `idJe` | `klmasterrobaid` |
| OKL-78 | | **izgled – Carpo.** NetGen prefiks `[` i sufiks ` ]` na group headeru |
| OKL-79 | `pocetniSort` | `sql` |

`OKL-25`–`OKL-27` čitaju isti SELECT na `spin` (samo čitanje) i provjeravaju imena stupaca i da `sortkey` ne opada. Tipovi s poslužitelja određuju koju granu iz `CreateGridColumns` stupac dobije.

## Dijalog FRMEDIT (Form2)

Otvara ga gumb Izmjena (`NovaGenerickaForma(sender.Tag)`, `5667`) i dvostruki klik grida (`6935`). Oba zovu `NovaGenerickaForma1` (`5062`, `Zadnji` default `True`).

Prije `ShowDialog`: `ID = GetID()`, `cls1.MasterID = ID` (`5294-5298`). `BrojKontrola` se broji u `SrediKontrolu` (`3069`), ima kontrola, forma je vidljiva, ide `ShowDialog` (`5354-5362`).

| ID | Očekivano | Ključ | NetGen |
|---|---|---|---|
| OKL-14 | Jedina forma je `FRMEDIT`. | | XML |
| OKL-15 | `ONSUBMIT` je atribut kakav ga `XmlDocument` vrati, triman. Počinje s `update`. | | `Form2.vb:1039` |
| OKL-15b | Prvih 6 slova `update` je u `INSERTDELETEUPDATESELECT`, pa se niz `@ime=` **ne** lijepi. `strParametri` postaje prazan. | `sqlImaZalijep=false` | `Form2.vb:3378-3388` |
| OKL-16 | `ONLOAD` je `select oznaka2 from klmasterroba where klmasterrobaid=@@ID` (krajnji razmak triman). | | `Form2.vb:1041-1042` |
| OKL-17 | `CMD1CAPTION=Prihvati` i `CMD2CAPTION=Odustani` ostaju cijeli natpis. Tag se ne puni. Nema loma na dvotočki (to je tek CMD3–CMD6, `1068-1090`). | | `Form2.vb:1061-1066` |
| OKL-19 | `Label16`: grana `label` (`InStr` „label”). Natpis je `CAPTION` „Dodatna oznaka”. Atribut `TEXT=Label16` kod ne čita. | `kontrola.Label16.tekst` | `Form2.vb:1296,2932` |
| OKL-20 | `oznaka2`: grana `text`. Početni tekst prazan (nema `DEFAULT`). | | `Form2.vb:1996-2009` |
| OKL-21 | `BOLD=FALSE` se ne čita. Font diraju samo `FONT`, `SIZE`, `STYLE` (`3075-3081`). | | `Form2.vb:3079` |
| OKL-22 | Model čuva sirove `5112`, `2189`, `389`, `158`, `2002`, `2650`. | | XML |
| OKL-61 | Dvostruki klik otvara `FRMEDIT`. | `forma=FRMEDIT` | `Form1.vb:6935` |
| OKL-62 | Naslov dijaloga `Izmjena oznake`, pa `SrediSQLPosljeNextForme` (`1240`). Nema `@@`. | `formaNaslov` | `Form2.vb:969,1240` |
| OKL-63 | Širina `5112/14` = 365, prije registra. | `formaSirina=365` | `Form2.vb:1093` |
| OKL-64 | Visina `2189/14` = 156. Ako `MaximizeBox` (default) i `GetSetting` vrati broj, registar prepiše (`2785-2788`). Prvi put ključ ne postoji, ostaje 156. | `formaVisina=156` | `Form2.vb:1094` |
| OKL-65 | Label ne pokazuje `Label16`. | `kontrola.Label16.tekst=Dodatna oznaka` | `Form2.vb:2932` |
| OKL-66 | Label `Top` = 389/14 = 28. | `kontrola.Label16.top=28` | `Form2.vb:2945` |
| OKL-67 | `oznaka2.Left` = 2002/14 = 143. | `kontrola.oznaka2.left=143` | `Form2.vb:2947` |
| OKL-68 | `ONLOAD` se izvrši prije crtanja (`1195`). Ako ima red, `oznaka2.Text` je taj stupac. Vrijednost `null` postaje `""` (`2016-2018`). `@@ID` je `MasterID` (`SrediSQL2`, `2880`). | `kontrola.oznaka2.izOnLoada=true` | `Form2.vb:1146,1195,2016` |

Ostale koordinate, isto zaokruživanje: label `Left` 158/14 = 11, `Height` 288/14 = 21, `Width` 1728/14 = 123. Tekst: `Top` 28, `Height` 21, `Width` 2650/14 = 189.

`VISIBLE`/`ENABLED` = `TRUE` se pretvaraju u bool (`2951-2952`). Nema `TABINDEX` pa je `-2`: automatski tab, `TabStop=True` (`3027-3043`). Redoslijed dodavanja: label pa tekst. Zatim gumbi forme: `Button1.TabIndex = TabIndexCounter+1`, `Button2` +2 (`2775-2776`). `TabIndexCounter` raste samo za text koji je enabled i visible (`2068`), ne za label.

| ID | Ključ | NetGen |
|---|---|---|
| OKL-75 | `tab=Label16,oznaka2,Prihvati,Odustani` | `Form2.vb:3027-3043,2775` |
| OKL-74 | `acceptButton` prazan. Enter u tekstu ne šalje formu. `AcceptButton` se nigdje ne postavlja. | `Form2.vb:3367` |

Prihvati i Odustani su vidljivi (`2750-2757`). Njihov panel na dnu, `AutoSize` i razmak 8 su **izgled – Carpo** (gumbi dijaloga idu u ribbon). Natpis, redoslijed, tab i naredba ostaju.

### Prihvati, Odustani, Escape

`Prihvati` je `Button1_Click` (`3367`):

1. `OKClick = True`.
2. `PokupiParametre`. Text ide kao `@oznaka2='…'`. Apostrof postaje `¸¸`, zarez `~~` (`3655`). Vrijednost ide i u CB.
3. `SrediSQL2` zamijeni `@@ID` s `MasterID` (`2880`). `@@oznaka2` tu ostaje.
4. Prvih 6 slova je `update` → `SrediSQLPosljeNextForme` zamijeni `@@oznaka2` iz parametara/CB (`Class1.vb:740-851`). `¸¸` postane `''` (`850`). `strParametri = ""` (`3388`).
5. Forma se zatvori (`3395`). `FormClosing` ne šalje SQL (`4093`).
6. Roditelj, jer je `OKClick` i nema `NEXTFORM`: `ExecSql` od `strStora` (parametri prazni, nema `!!`) (`Form1.vb:5510-5514`), zatim `PokaziUpit` zadnjeg master SQL-a (`5519`, `Zadnji` je true).

`ExecSql` prije slanja vrati `~~` u zarez (`Class1.vb:87`). Ako u SQL-u ostane `@@`, naredba se ne šalje (`113-118`).

`Odustani` je `Button2_Click`: `OKClick = False`, `Close` (`3400-3402`). Roditelj ne ulazi u blok `5429`, nema updatea ni refresha.

Escape: nema `ONESCKEY`, pa `Close` (`4161-4166`). `OKClick` ostaje false. Isto kao Odustani.

| ID | Akcija | Ključ | NetGen |
|---|---|---|---|
| OKL-69 | `prihvati` | `refreshMaster=true` | `Form2.vb:3367`; `Form1.vb:5514-5519` |
| OKL-70 | `prihvati` | `sqlImaZalijep=false` | `Form2.vb:3378-3388` |
| OKL-71 | `prihvati` | `sqlApostrof=''` (unos s apostrofom u tekstu) | `Form2.vb:3655`; `Class1.vb:87,850` |
| OKL-72 | `odustani` | `refreshMaster=false` | `Form2.vb:3400`; `Form1.vb:5429` |
| OKL-73 | `escape` | `refreshMaster=false` | `Form2.vb:4161-4166` |

Test **ne šalje** `UPDATE`. U `sql` ključu mora stajati tekst koji bi otišao, s zamijenjenim `@@`, bez izvršenja.

## Što se namjerno ne događa

- `GRIDALLOWEDIT` ne uključuje edit ćelije.
- `TEXT` na labeli nije natpis. Font i `BOLD` su **izgled – Carpo**.
- `KOMENTAR` (`CallistoII 1.0.0`) nije naslov.
- Prazno stablo se ne crta kao prazan TreeView na pola forme. Splitter je 0.
- Prevoditelj ne dira natpis dok jezik nije postavljen i nije različit od `hrv` (`Module1.vb:266-272`).
