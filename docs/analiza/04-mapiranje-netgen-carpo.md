# 04 — Mapiranje NetGen → Carpo

**Iteracija 2.** Raspored: preslikati 1:1 ono što Carpo već zna (kontrola, natpis, tab order, kolone grida, `value`/`display` kad su stupci 0 i 1). `TOP`/`LEFT` se ne tjeraju u piksel. Razlika se zapisuje, ne popravlja. SQL se ne mapira na `<parameter>`. `TotalGenMultiSelectTemp`, DMS `NetGenFinder`, `ASSEMBLYLOAD`→`totalgen` i `FISKALIZACIJA`→Raverus/`SLOFISKAL` nemaju Carpo ekvivalent i ostaju vlastiti kod. Tipfeleri iz dokumenta 03 odjeljak 12 se ne „popravljaju“ u Carpo atribute.

Tri oznake:

- **direktno** — Carpo već ima isti smisao i može se napuniti bez novog koda, ili uz tanak adapter koji ne mijenja semantiku
- **adapter** — DevExpress kontrola postoji, ali NetGen pravilo (redoslijed kolona, `/14`, podstring `TYPE`, lijepljenje SQL-a, komandni string) mora ostati u novom kodu
- **nema** — u Carpo modelu ili u `Events.Checking` nema ekvivalenta; treba novi runtime, ne novi XML atribut

Ručni parovi u `definitions/Prepisane_granule/` su dokaz što su ljudi radili kad su granulе prepisivali. **Nisu** specifikacija `Carpo.NetGen`. Na kulturama se vidi da prepis mijenja procedure i polja. Runtime koji učitava stari XML ne smije to ponoviti.

## 1. Parovi prepisa

NetGen strana: `definitions/Prepisane_granule/Netgen/` (28 fajlova). Carpo strana: `definitions/Prepisane_granule/Carpo/`. Imena se ne poklapaju 1:1. Jedan NetGen fajl često postane glavni XML plus `sub/forms` i `sub/parts`.

| NetGen | Carpo prepis | Odnos |
|---|---|---|
| `Agro/Kulture.xml` | `Carpo/Agro/Kulture.xml` | isti posao, drugi SQL |
| `Agro/OtkupnoMjesto.xml` | `Carpo/Agro/OtkupnoMjesto.xml` | par |
| `Agro/Parametri obračuna/KultureDet.xml` | `Carpo/Agro/Parametri obračuna/RatOtkupKultureDetalji.xml` + `sub/forms/*.frmUnos.xml`, `frmIzmjena.xml` | preimenovano i rascijepano |
| `Kontor/KarticaPartnera.xml` | `Carpo/Kontor/KarticaPartnera.xml` | par, drugačiji raspored |
| `Kontor/KntIzv306090.xml` | `Carpo/Kontor/Izvjestaj306090.xml` | preimenovano |
| `Kontor/KntPrijebojiDev.xml` | `Carpo/Kontor/CKntPrijebojiDev.xml` | par |
| `Kontor/KntPrijebojiKn.xml` | `Carpo/Kontor/CKntPrijebojiKn.xml` | par |
| `Kontor/KontniPlanHR-ENG.xml` | `Carpo/Kontor/KontniPlan.xml` | par |
| `Kontor/MPRacuni.xml` | `Carpo/Kontor/MPRacuni.xml` | par |
| `Kontor/rptOpzStat1.xml` | `Carpo/Kontor/KntOpzStat1.xml` | par |
| `Kontor/KntNaplReal.xml`, `Predujmovi.xml`, `PregledKnj.xml` | istoimeno ili blisko pod `Carpo/Kontor/` | par, nije red po red uspoređivan u ovoj fazi |
| `MP/*.xml` (12) | `Carpo/MP/*Carpo.xml` + `SUB/forms` + `SUB/parts` | jedan fajl → mnogo fajlova |
| `Skt/ZahtjevZaNabavkuNG.xml` | `Carpo/Skt/ZahtjevZaNabavu.xml` + `ZahtjevZaNabavu.frm*.xml` | rascijepano |
| `Skt/StanjeAmbalazeNG.xml` | `Carpo/Skt/StanjeAmbalaze.xml` + `.frmKartica` + `.frmPregled` | rascijepano |
| `Skt/DobUgovori.xml` | `Carpo/Skt/UgovoriSDobavljacima.xml` | preimenovano |

Uzorci koji su pročitani red po red: `Kulture` (oba) i `KarticaPartnera` (oba). Ostali parovi su mapirani po imenu i po strukturi direktorija, ne po svakom atributu. Tvrdnje o „svim prepisima“ koje nisu iz ta dva fajla označene su kao uzorak.

## 2. Što prepisi stvarno rade (nije 1:1)

### Kulture

NetGen (`definitions/Prepisane_granule/Netgen/Agro/Kulture.xml`):

- korijen `PROJECT`, `ONLOAD="spKultureNGQuery @Action=1"`, gumbi `CMD1CAPTION="Unesi:INSERT"` …
- `INSERT` je apsolutni dijalog 7085×4104, kontrole `text`, `search` (`roba#robaid#userid%#…`), `combo` nad `select orgshemaid, rtrim(oznaka)+…`
- `ONSUBMIT="spKultureNGAzur @Action=0,"` — zarez na kraju, parametri se lijepe
- gumb ne zove `OK` slijepo: `ONCLICK="if (@@robaid is null or '@@naziv'='') exec spRaiserror …#OK"`
- `PARAMETRI` ima grid `stavke` i `@@gridstavkeKultureFormaId`

Carpo (`definitions/Prepisane_granule/Carpo/Agro/Kulture.xml`):

- korijen `root`, forma `frmKulture` `type="master"`, `managerlayout` / `grid` `dataname="~dtKulture"`
- procedura je **`spKulture`**, ne `spKultureNGQuery`. Akcije su drugi brojevi (`@Action=1` lista, `@Action=4` brisanje, `@Action=2` insert)
- polja Standard1–3, cijena, oznaka, `ind`, `indkor`, šifra kao zasebni `text` **nisu** prenesena. Dodan je `Tip` (`comboedit`)
- combo orgsheme postao je `searchedit` nad `spOrgShemaQuery`
- tražilica robe: `searchedit` + `netgenfields="true"` + `spTraziRoba`, ne `roba#robaid#…`
- brisanje više nije forma `DELETE` s `ONSUBMIT`, nego `confirm` + `datarun nonquery` + `control populate`
- validacija je `required="true"` i `validate`, ne `spRaiserror` u `ONCLICK`

Zaključak: čovjek je granulu **dizajnirao iznova** na Carpo jeziku i usput promijenio ugovor s bazom. `Carpo.NetGen` koji učita stari XML mora zvati `spKultureNGQuery` / `spKultureNGAzur` i pokazati Standard1–3. Inače stara granula nije ista granula.

### Kartica partnera

NetGen (`…/Netgen/Kontor/KarticaPartnera.xml`), cijeli fajl je kratak i gust:

- `ONLOAD=":FILTERFORM"` — prije grida se otvori dijalog
- `CMD3CAPTION="Ispis:PRINTMASTER"`
- `CMD4CAPTION` i `CMD5` su `::repx/rptKarticaKonta…(@DatumDo=@@DatDo, …)#CLOSE` — REPX s imenovanim parametrima, dva jezika
- `FILTERFORM` `ONSUBMIT` završava s `!!` i `FILLEXPLORER="TRUE"` — master se puni tim SQL-om **bez** drugog lijepljenja parametara (`netgen/Form1.vb` 5449)
- `ONLOAD` filtera puni datume iz `@@datod` / `@@datdo` i firme iz `PubVars`
- `multidrop` za konto, `search` preko `spTraziPartneri`, `combo` valuta, `check` Analitika PS
- `date` s `DEFAULT="firstdayofyear"` i captionom `Razdoblje:*` (zvjezdica = automatski label)
- gumb: `If @@partneriid is null begin exec spRaiserror 66001, '… +focus(partneriid)' end##OK` — `#` je postao `##` jer je unutar jednog gumba? U fajlu je `end##OK`. `##` NetGen u zaglavlju ispisa tretira kao novi red, a u lancu komandi `#` je separator. `##` su dva separatora s praznim segmentom između. Prazan segment se preskače (`NetGen-Komande.md`). Znači `##OK` je i dalje `OK`. To je lako pogrešno „popraviti“
- `PAGEHEADERLEFT="@@searchboxpartneriid"`, `PAGEORIENTATION="LANDSCAPE"`

Carpo prepis (`…/Carpo/Kontor/KarticaPartnera.xml`):

- `type="master" template="splitter" maximized="true"`
- filter nije modalna forma nego lijevi panel (`layout name="panel1" type="collapsable"`)
- konto je `combomultiedit`, partner `searchedit netgenfields="true"`, datumi `dateedit` s `savevalue="app"` i `required="true"`
- `DEFAULT="firstdayofyear"` nema. Početni datum je drugačiji problem (varijabla / `oninit`), nije isti token
- `!!` nema. Parametri su `<parameter name="@…" value="#kontrola"/>`
- fokus se eksplicitno dira (`control focus` na `load` i `shown`) jer Carpo inače šalje `SendKeys` — komentar u fajlu 23.01.2025
- `minsize="200"` zbog novog DevExpresa — komentar 20.5.2021. Apsolutni NetGen layout taj problem nema, DX layout ima

Neočito što prepisi otkrivaju, a iz samog Carpo XML-a se ne vidi:

1. `!!` na `ONSUBMIT` je dio ugovora „ne lijepi parametre“, ne tipfeler. Kartica bez toga dobije dvostruke parametre ili ih izgubi.
2. `##` u akciji gumba je prazan segment, ne escape.
3. `:*` na captionu stvara label. U Carpu caption je na `<item>`, ne na kontroli, i zvjezdica nema tu ulogu.
4. `FILLEXPLORER` ima vrijednosti `true`, `false`, `onlydet`, `onlytree`, `treeandrefreshmaster`, `treeandmaster` (`Form1.vb` 5445–5507). Carpo „osvježi grid“ je `control populate`. Nema `onlytree`.
5. `netgenfields="true"` na `searchedit` je svjestan kompromis autora prepisa: hoće DX tražilicu, ali NetGen raspored kolona. To je jedina službena kuka. Ne pokriva `roba#id#kolona%`.
6. Rascijep jednog XML-a na `path=` forme (`Kartica` vuče `IOSObrazac.frmFakturaRacun.xml`, `Temeljnice.frmTemEdit.xml`, `KnjigaUra.frmDMS.xml`) je Carpo navika. NetGen `INCLUDE` ubacuje drugi `<PROJECT>` u isti dokument, ne referencira formu po putanji na isti način. Adapter koji „za ljepotu“ rascijepi fajl promijeni `@@xmlname` i putanju DMS-a.
7. Prepis kultura mijenja SP. Svaki automatski diff „NetGen XML vs Carpo XML“ koji traži isti `command` pasti će i kad je prepis ispravan kao proizvod, i kad je kriv kao prevoditelj. Harness ne smije koristiti Carpo prepis kao expected output.

## 3. Korijen i forma

| NetGen | Carpo | Oznaka |
|---|---|---|
| `<PROJECT>` | `<root>` + jedna master `<form>` | adapter. Ne smije se pisati na disk kao zamjena |
| dijete-forma `<INSERT>` | `<form name="INSERT">` | adapter. Ime mora ostati isto jer ga komande zovu |
| `<CAPTION>` tekst | `form/@caption` | direktno |
| `WIDTH` `HEIGHT` `/14` | `form/@width` (pixeli, drugi smisao) | adapter. Carpo width nije NetGen width |
| `RESIZE=false` | fiksna forma — Carpo ima `maximized`, nema isti `RESIZE` | adapter / djelomično nema |
| `MAXIMIZEBOX` | nema 1:1 atributa na `JForm` u isječku koji je čitan | **neprovjereno** da `JForm` ima `maximizebox`. Ako nema: adapter na WinForms formi |
| `ONLOAD` SQL | `event type="load"` + `datarun` | adapter. U NetGenu isti atribut može biti `:IMEFORME` |
| `ONLOAD` `:IME` | `form open` | adapter, i to prije grida |
| `ONLOAD2` | nema. Najbliže je drugi `datarun` koji postavlja `enable`/`visible` | adapter. Semantika je „kolona = ime kontrole, vrijednost = visible/enable/refresh“ |
| `ONSUBMIT` / `STORA` | `datarun` na klik Spremi | adapter zbog lijepljenja i `!!` |
| `ONCLOSED` `ONCLOSING` `ONLOADED` `ONACTIVATED` `ONLOSTFOCUS` | `event type="close"`, `shown`, `leave` | djelomično direktno. `ONACTIVATED` i `ONCLOSING` u korpusu 0, u kodu da |
| `NEXTFORM` | ručni `form open` s parametrom | adapter. Carpo nema lanac formi koji prenosi cijeli prethodni parametarski string |
| `FOCUS` | `control focus` | direktno ako se izvede nakon load |
| `FILLEXPLORER` | `control populate` parent grida | adapter, više vrijednosti |
| `CMD1` = spremi, `CMD2` = odustani bez submita | `panelbuttons` / `button` | adapter. Uloga je po **poziciji** gumba, ne po imenu |
| `CMD3`…`Natpis:Forma` | `form open` | adapter |
| `BUTTONS` | isti gumbi, `#` ih dijeli | adapter. `BUTTONS` prepisuje `CMDx` |
| `VERSION` u naslovu | `root/@version` je verzija definicije, ne sufiks naslova | nije isto |
| `PERMISIO` | Carpo permissions (`Logic/Permissions.cs`) | **nije isto.** NetGen zove `ssPermission*`. Spajanje je neprovjereno i ne treba ga pretpostaviti |
| `INCLUDE` | `<part path>` / `<form path>` | slično, drugi format. Adapter čita NetGen `INCLUDE` |
| `ICON` | ribbon / forma | neprovjereno 1:1 |
| `HELPKOD` | nema Janus/Jupiter help klijenta u Carpu | nema, osim ako se zove `JupiterHelpClient` (NetGen referenca). Carpo help **nije** čitan u ovoj fazi |

## 4. Kontrole

| NetGen `TYPE` | Carpo element | Oznaka | Napomena |
|---|---|---|---|
| `text` | `textedit` | adapter | `MULTILINE` → `memoedit`. `PASSWORDCHAR` postoji na NetGen textu. Carpo `textedit` lozinka: **neprovjereno** da postoji password char na `JTextEdit` |
| `number` / `cur` | `textedit` s maskom ili spin | adapter | NetGen je Janus `NumericEditBox`, prazno = `0`, šalje broj bez navodnika. Carpo nema „prazno postaje nula“ kao default jezika |
| `date` / `dtpicker` / `datetime` | `dateedit` | adapter | `DEFAULT=firstdayofyear\|firstdayoflastyear\|firstdayofmonth\|firstdayoflastmonth` Carpo nema. `SHOWCHECKBOX` + null datum Carpo `allownull` nije isti (kvačica na pickeru) |
| `combo` | `comboedit` | adapter | NetGen: kolona 0 = value, kolona 1 = display, **imena se ne gledaju**. Carpo traži `valuefield` i `displayfield`. Ako procedura vrati kolone bez tih imena, Carpo combo je prazan osim ako adapter sam postavi prvu i drugu |
| `multidrop` | `combomultiedit` | adapter | NetGen šalje `'id1~id2~'`, ne Carpo liste. Procedure to već parsiraju |
| `search` / `trazilica` | `searchedit` | adapter | Dva SQL oblika: `sp`/`select`, ili `tablica#id#prikaz#%kol%`. Drugi oblik Carpo ne razumije. `@@searchboxime` je tekst, ne id. `netgenfields` pokriva samo izgled |
| `check` / `checkbox` / `vb.checkbox` | `checkedit` | adapter | NetGen šalje `CheckState` (0/1/2), ne boolean. `Indeterminate` je moguć |
| `radio` | `radioedit` | adapter | šalje 1/0, ne grupu Carpo radio vrijednosti. Grupa je implicitna po WinForms parentu |
| `label` | `labeledit` ili caption na `item` | adapter | `ONCLICK` na labeli je komanda. Carpo label to nema osim ako se stavi `button` |
| `button` / `help` | `button` | adapter | `ONCLICK` je cijeli jezik komandi, ne jedna `action` |
| `frame` / `group` | `group` | adapter | apsolutni okvir naspram layout grupe. Caption okvira da, pozicija ne |
| `panel` | `container` ili group | adapter | |
| `grid` | `grid` | adapter | vidi odjeljak 5. Janus → `GridControl` |
| `dxgrid` | `grid` | direktnije | korpus 0. I dalje DX 13 API u starom kodu, ne 26 |
| `tree` | `tree` | adapter | `yearmonth`, SQL s 4 kolone, tag `id:forma`. Carpo tree je drugi ugovor (nije čitan red-po-red u ovoj fazi; model je `JTree`) |
| `webbrowser` | nema `webbrowser` elementa u `JLayoutItem` | nema | 3 kontrole u korpusu. DX nema 1:1 u popisu editora. WinForms `WebBrowser` u adapteru |
| `picturebox` | `pictureedit` | adapter | blob u koloni `NAME`, putanja u `NAMElocation` |
| `splitcontainer` | splitter predložak forme, ne kontrola u layoutu | djelomično | 0 u korpusu kao TYPE? Nije u top 44 osim ako je unutar rijetkih. Nije izašao. Kod ga ima |
| `statusstrip` | footer akcije (`footer`) | adapter | `TEXT1`–`TEXT3` su SQL |
| tipfeleri `b_utton`, `g_rid`, `datetiem` | ništa | direktno „ništa“ | ne crtati |

Zajednički atributi:

| NetGen | Carpo | Oznaka |
|---|---|---|
| `NAME` | `name` | direktno, ali Carpo ime mora biti jedinstveno i bez razmaka; NetGen ime je i SQL parametar `@ime`. Preimenovanje u `txtNaziv` lomi `@@naziv` |
| `CAPTION` | `caption` | direktno, uz pravila `_`, `!!`, `*` |
| `ENABLED` `VISIBLE` | `enabled` `visible` | direktno za literal `true`/`false`. NetGen prima i `0`/`1` i šuti na smeće |
| `TABINDEX` | `tabindex` | direktno. `-2` i prazno u NetGenu dobiju automatski indeks |
| `ANCHOR` `DOCK` `PARENT` | dock djelomično | adapter. Carpo layout ionako rješava parent preko grupe |
| `HOTKEY` | **nema** viđeno na `JButton` u ovoj fazi | nema ili neprovjereno. Ako `JButton` nema hotkey atribut, adapter ga veže na `KeyDown` |
| `DEFAULT` | `default` / početni `value` | adapter. Za datum posebni tokeni |
| `SQL` | `datalist/data` | adapter |
| `REFRESHAFTERCHILD` | `control populate` nakon `form close` | adapter. NetGen pamti SQL kontrole |
| `TOP LEFT WIDTH HEIGHT` | `row col` samo kao gruba složenost, ne kao koordinata | **poznata razlika** (odluka 4). Ne crtati apsolutni panel kao obvezu. Zapisati u log granulu koja ima preklop (dva kontrole isti `TOP`/`LEFT`) |
| `TEXT` | ništa | ignorirati |
| `BOLD` `BORDERSTYLE` | `fontbold` postoji na layout itemu | ne kopirati `BOLD` jer ga NetGen ne čita. Inače prepis „poboljša“ ekran |
| `ALOweMPTY` | ne mapirati na `allownull` | NetGen ga ne čita |

## 5. Grid

| NetGen | Carpo | Oznaka |
|---|---|---|
| master iz `ONLOAD` SQL-a, kolone iz rezultata | `grid` + `dataname`, kolone iz `field` ili auto | adapter. NetGen crta što SQL vrati, `MASTERCOLUMNS` samo prekriju |
| `CLEARCOLUMS=true` (tipfeler u imenu atributa, tako ga kod čita) | `control datasourceempty` / sakrivanje | adapter. Ispravno slovkanje `CLEARCOLUMNS` kod **ne** traži — provjera imena: u `NetGen-XML-Pravila.md` i u kodu je `CLEARCOLUMS`. Carpo neka ne „ispravlja“ |
| `COLUMN/@NAME` ili `@INDEX` (1-based) | `field/@name` | adapter. Indeks pobjeđuje ime |
| `FORMAT` | `field/@format` | djelomično. Janus format string nije uvijek DX format |
| `AGREGATEFUNCTION` | `groupsummary/summary` | adapter, i samo za tipfeler s jednim G |
| `AGGREGATEFUNCTION` | ništa | namjerno |
| `CHECKBOXVISIBLE` | `multiselect` / checkbox kolona | **nije isto.** NetGen checkbox je stupac 1 za `FOREACHSELECTED` i `@nizoznacenihajdijeva`. Carpo `multiselect` je plavi odabir, bliži `FOREACHBLUEROW` |
| `TOTALROWVISIBLE` | summary red | adapter |
| `GROUPBYBOXVISIBLE` | group panel | direktno kao mogućnost DX grida |
| `FROZENCOLUMNS` | fixed columns | direktno ako se postavi na DX |
| `EDITCOLUMNS` | editable `field` | adapter. Vrijednost smije biti `select`/`sp` koji vrati listu |
| `ONCELLEDITED` / `BEFORECELLEDITED` | `cellvaluechanged` | adapter. BEFORE je otkazivanje, Carpo event to nema u istom trenutku |
| `POPUP` / `POPUPMASTER` | `<menu><menuitem>` | adapter. Stavke su `Natpis:Akcija` |
| `MASTERDBLCLICK` | `event type="doubleclick"` | adapter |
| `DXGRID=true` | uvijek DX | u novom runtimeu master je uvijek DX 26. Atribut gubi smisao, ponašanje `GetDXID` (fallback prva kolona) treba sačuvati ako ikad naiđe XML iz baze |
| `ENTERKEYBEHAVIOR` | `entermovenexteditor` na Carpo gridu (`DataControls.cs` 571) | djelomično. Janus enum nije taj bool |
| `FILTERMODE` | DX filter mode | adapter, drugi enum |
| `MULTIGRIDLAYOUT` / `MASTERGRIDLAYOUTFILE` | Carpo `setlayout` / predlošci | adapter. Spremanje je u NetGenu lokalni fajl + baza. Carpo ima vlastite predloške (`Carpo.Data` GridTemplates). Ne miješati spremnike u prvoj verziji |
| širina kolone `/14`, ili zadnja 4 znaka captiona | `netgensizes` | adapter. `Class1.vb` ~1047 čita širinu iz captiona ako je tako posloženo |
| `NEGATIVERED` `NOTZERORED` | `formatrule` / `style` | adapter. Korpus 0, kod da (`Form1.vb` 7544) |

`@@id` nije `key` Carpo grida. Ako nema kolone `id`, Janus uzme treću ćeliju, DX prvu. Carpo `key="KultureId"` u prepisu je nova odluka autora. Adapter mora kopirati NetGen fallback, ne Carpo `key`.

## 6. Stablo, komande, SQL, ispis

| NetGen | Carpo | Oznaka |
|---|---|---|
| `TREE` segmenti `#`, `lijevo:desno` | `tree` + `dataname` | adapter |
| `yearmonth` | nema | nema. Hardkod 2005–2009 |
| SQL stabla, 4 kolone redom | Carpo tree polja po imenu | adapter |
| `@@node` | nema ugrađenog | adapter, varijabla |
| `SPLITTERDISTANCE` | splitter `minsize` nije isto | adapter |
| `#` lanac, `~` unutar gumba | ugniježđene `action` | **nema** kao string. Interpreter mora ostati |
| `CLOSE` `OK` `OKSTAY` `CANCEL` `CLOSEPARENT` | `form close`, `dialogresult` | adapter. `OK` još i izvrši submit |
| `REFRESHMASTERGRID` | `control populate` parent | adapter |
| `REFRESHCONTROL(ime)` | `control populate` jedne kontrole | adapter, i samo na dijalogu |
| `FOREACHROW/SELECTED/BLUEROW` | `control foreachrow` / `foreachrowselected` | djelomično. Carpo `foreachrow` postoji (`Notifications.cs`). NetGen verzija je drugi segment lanca i na podlozi je mrtva. Ne zvati Carpo foreach na podlozi ako se želi ista rupa |
| `MULTISELECT` / `MULTIBLUE` forme | `foreachrow` + parametri | **nema** zamjene za `TotalGenMultiSelectTemp`. Procedure čitaju tablicu, ne parametar |
| `PRINTMASTER` `PRINTGRID` | `print` / `control print` | adapter. Oba NetGen prefiksa zovu REPX grid ispis, ne stari ispis |
| `::repx/ime(@p=@@x)` | `action type="print" value="repx/…"` | adapter za parsiranje parametara iz zagrada |
| `PAGEHEADER*` `##` `!!` `@@` | Carpo `print` header **nije** čitan u ovoj fazi kao isti jezik | adapter ili nema. Zaglavlje je NetGen string |
| `ASSEMBLYLOAD` | `comdll` / `method` | **nema.** Potpis `totalgen(id, operater, connection, …)` je drugi ugovor od Carpo `comdll` |
| `FISKALIZACIJA*` | `ZKI` / `JIR` akcije | **nije isto.** Carpo fiscal je drugi ulaz (`FiscalOutlookActions`). Stari je `Raverus` COM. Spajanje je odluka, ne mapiranje |
| `spRaiserror` + `#OK` `+focus` `#close` | `message` / `confirm` | **nema** ako se čita samo XML. Tekst dolazi iz SQL iznimke u runtimeu |
| `!!` na `ONSUBMIT` | nema | nema. Mora ostati u interpreteru |
| `@@` bilo koje ime | `#kontrola` / `$param` / `~dt.@kol` | adapter koji radi zamjenu **prije** nego SQL vidi Carpo. Ne prevoditi `@@robaid` u `#srcRoba` jer se kontrola tako ne zove |
| DB clipboard | Carpo `savevalue="app"` | **nije isto.** Clipboard je tablica `netgendbclipboard`, ključ operater + forma. `savevalue` je Carpo pohrana. Ne zamjenjivati u prvoj verziji |
| `SETDEFAULTS` `FILTERSAVE` | `savevalue` | djelomično, drugi spremnik |

## 7. Što se ne smije mapirati iz prepisa

Ovo su zamjene koje čovjek smije, a loader ne smije:

- novo ime procedure (`spKultureNGQuery` → `spKulture`)
- novo ime kontrole (`Naziv` → `txtNaziv`) jer `@@naziv` i `@Naziv` ovise o starom
- izbacivanje polja (Standard1–3)
- combo → searchedit kad SQL vraća dva stupca bez tražilice
- modalni `FILTERFORM` → ugrađeni panel, osim ako se ponašanje (blokada dok se ne klikne Prikaži, `FILLEXPLORER`) sačuva
- `confirm` umjesto forme `DELETE` — vizualno je bolje, ali `ONLOAD` te forme i `spRaiserror` više se ne izvrše istim redom
- `key="KultureId"` umjesto fallbacka na treću ćeliju

Ako produkt jednog dana želi „ljepši Carpo“ za pojedinu granulu, to ostaje ručni prepis. `Carpo.NetGen` je drugi proizvod: stari XML, novo crtanje.
