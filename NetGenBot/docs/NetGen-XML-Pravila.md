# NetGen XML — pravila za autore

Izvor ponašanja: `netgen`, commit `b0d5d41`. Komande: `NetGen-Komande.md`. Kontrole: `NetGen-Kontrole.md`. Tok granule: `NetGen-Tok.md`. Linije: `NetGen-Engine.md`.

Jedna granula je jedan XML. Root je `<PROJECT>`. Forme (`FILTERFORM`, `INSERTFORM`, `UPDATEFORM`, bilo koje ime) su elementi u istom fajlu. Jupiter učitava fajl iz `SharedDLLs\xml\` pod imenom granule.

Engine gotovo svuda ima `On Error Resume Next`. Nedostajući atribut često ne sruši granulu, nego ostane prazan. To nije dozvola da se obavezni dijelovi izostave: `CMDxCAPTION` i `BUTTONS` rade `IndexOf(":")` i pucaju ako nema dvotočke.

## 1. PROJECT — podloga

```xml
<PROJECT NAME="Primjer"
         ONLOAD="spPrimjer_Query @action=1,@operateriid=@@operateriid"
         DETAILGRID="spPrimjer_Query @action=2,@id=@@id"
         MASTERDBLCLICK="UPDATEFORM"
         BUTTONS="Unos:INSERTFORM#Izmjena:UPDATEFORM"
         CHECKBOXVISIBLE="FALSE"
         TOTALROWVISIBLE="TRUE"
         GROUPBYBOXVISIBLE="FALSE"
         FROZENCOLUMNS="1">
  <CAPTION>Naslov granule</CAPTION>
  <MASTERCOLUMNS>
    <COLUMN NAME="naziv" CAPTION="Naziv" WIDTH="220"/>
  </MASTERCOLUMNS>
</PROJECT>
```

### ONLOAD

- Ako **ne** počinje s `:`, cijeli string je jedan SQL koji puni master grid. `@@` se zamijene prije izvršavanja.
- Ako počinje s `:`, ostatak je ime forme (ili lanac komandi). Tipično `ONLOAD=":FILTERFORM"` otvara filter prije grida.
- `ONLOAD2` je poseban upit: prvi red rezultata dodatno podešava gumbe (vidljivost, enable), ne puni grid.

`DETAILGRID` je SQL za donji grid. Ako počinje s `:`, engine pokaže poruku `sorkac!` i ne otvori formu. Detail mora biti upit, najčešće s `@@id`.

### Gumbi

Dva zapisa, isti oblik stavke `Natpis:Akcija`:

- `BUTTONS="Unos:INSERTFORM#Izmjena:UPDATEFORM"` — stavke odvojene `#`, redom pune Button1, Button2, …
- ili `CMD1CAPTION="Unos:INSERTFORM"` … `CMD10CAPTION`.

`BUTTONS` prepisuje gumbe koje je `CMDxCAPTION` već postavio.

Jedan gumb može imati više komandi. `#` je već zauzet kao separator gumba, zato se unutar akcije koristi `~`. Engine `~` pretvara u `#` i izvršava redom:

```xml
BUTTONS="Obriši:DELETEFORM~REFRESHMASTERGRID"
```

Akcija nakon dvotočke:

| Zapis | Efekt |
|---|---|
| `IMEFORME` ili `:IMEFORME` | Otvori formu tog imena iz istog XML-a |
| `select ...` / SQL | Izvrši upit |
| `==sql` | Prikaži rezultat upita (nakon što se skinu `==`) |
| `REFRESHMASTERGRID` | Ponovo učitaj master zadnjim SQL-om |
| `PRINTMASTER` / `PRINTREPXMASTER` | Ispis master grida (REPX) |
| `PRINTDET` | Ispis detail grida |
| `REFRESHCONTROL(ime)` | Ponovo napuni kontrolu |

`FOREACHROW`, `FOREACHBLUEROW` i `FOREACHSELECTED` rade **samo u dijalogu** (`Form2`). Na podlozi (`Form1`) te tri komande su zakomentirane i klik ne uradi ništa (`Form1.vb` 6843–6850). Na dijalogu (`Form2.vb` 7040–7047):

| Komanda | Šta obilazi |
|---|---|
| `FOREACHROW(grid)` | Sve redove tog grida |
| `FOREACHBLUEROW(grid)` | Plavo označene redove (`SelectedItems`) |
| `FOREACHSELECTED(grid)` | Redove s checkboxom |

Više označenih redova s mastera ide preko forme čije ime počinje s `MULTISELECT` ili `MULTIBLUE`, ne preko `FOREACH*` na podlozi.

`MASTERDBLCLICK="UPDATEFORM"` otvara tu formu na dvoklik reda. `DETAILDBLCLICK` isto za donji grid.

`POPUPMASTER="Izmjena:UPDATEFORM#Obriši:DELETEFORM"` je desni klik na masteru. `POPUPDETAIL` na detailu. Stavke opet `Natpis:Akcija`, odvojene `#`.

### Tree

`TREE` je lista odvojena `#`. Svaki komad je `lijevo:desno`.

| Lijevo od `:` | Što se desi |
|---|---|
| `yearmonth` | Ugrađeno stablo godina 2005–2009 i mjeseci. Desno je ime forme. |
| sadrži `tree` (npr. `spMPTreeNG @action=2`) | Lijevo je SQL/procedura koja puni čvorove, desno ime forme |
| bilo koji naslov (`Danas`, `Filter`) | Statički čvor. Desno je ime forme koja se otvara klikom |

Procedura za stablo mora vratiti najmanje 4 kolone, ovim redom:

1. id čvora
2. parent id (veza dijete → roditelj)
3. natpis
4. vrijednost koja ide u `@@node` (ako sama sadrži `:`, engine je ne dira; inače dopiše `:ImeForme`)

Klik na čvor postavlja `@@node` na dio taga **prije** dvotočke i otvara formu iz dijela iza dvotočke. Statički čvor `Danas:FILTERDANAS` ima prazan `@@node` i otvara `FILTERDANAS`.

`TREETRANSLATE="true"` prevodi natpise čvorova. `SPLITTERDISTANCE` je širina panela stabla (inače 290). `TREEDRAGDROP` se čita; prazan `TREE` sakrije stablo.

### Master grid

Atributi na `<PROJECT>`:

| Atribut | Značenje |
|---|---|
| `GROUPBYBOXVISIBLE` | Kutija za grupiranje mastera |
| `GROUPBYBOXDETVISIBLE` | Isto za detail |
| `FROZENCOLUMNS` | Broj zamrznutih kolona |
| `CHECKBOXVISIBLE` | `FALSE` gasi checkboxe na masteru (default u kodu je `true`) |
| `TOTALROWVISIBLE` | Suma / total red |
| `DETCHECKBOXVISIBLE`, `DETTOTALROWVISIBLE` | Isto za detail |
| `ENTERKEYBEHAVIOR` | Janus ponašanje Entera |
| `AUTOMATICSORT` / `AUTOMATICSORTDET` | `false` gasi automatski sort |
| `FILTERMODE`, `FILTERMODEDET` | Način filtera grida |
| `DXGRID` | `true` — master ide na DevExpress grid, ne na Janus |
| `EDITCOLUMNS` | Kolone koje se smiju editirati; može biti i `select`/`sp` koji vrati listu |
| `ONCELLEDITED`, `BEFORECELLEDITED` | SQL nakon / prije izmjene ćelije (`EDITCOLUMNSDET` i `...DET` za detail) |
| `MULTIGRIDLAYOUT`, `MASTERGRIDLAYOUTFILE` | Više snimljenih layouta / ime fajla |
| `COMMANDTIMEOUT` | Timeout upita |
| `PERMISIO` | `true` uključuje provjeru prava |
| `INCLUDE` | Uključivanje drugog XML-a (čita se kao `INCLUDE`) |
| `SETDEFAULTS`, `FILTERSAVE` | Pamćenje filtera |
| `REFRESHAFTERCHILD` | Osvježi podlogu nakon zatvaranja child forme |
| `ICON` | Ikona iz `Graphics\Icons\` |
| `VERSION` | Doda se u naslov: `v[…]` |
| `PAGEHEADERLEFT/CENTER/RIGHT`, `PAGEFOOTER*`, `PAGEORIENTATION` | Zaglavlje ispisa stranice |
| `TIMER` | Ako je prazan, treći status label se sakrije |
| `MENU` | `FOLDER` ili iz baze; prazan sakrije menu |
| `COMBO` | SQL za combo iznad stabla; prazan combo se sakrije |
| `RELATION` | Relacija hijerarhijskog grida |
| `HELPKOD` | Prikazuje HELP gumb |

`<CAPTION>` je tekst prozora (prvi child tekst).

## 2. Kolone

```xml
<MASTERCOLUMNS CLEARCOLUMS="false" HEADERALIGNMENT="">
  <COLUMN NAME="naziv" CAPTION="Naziv" WIDTH="200" FORMAT="" VISIBLE="true"/>
  <COLUMN NAME="iznos" CAPTION="Iznos" WIDTH="90" FORMAT="n2" AGREGATEFUNCTION="Sum" NEGATIVERED="true"/>
</MASTERCOLUMNS>
```

`DETAILCOLUMNS` je isti obrazac za donji grid.

`CLEARCOLUMS="true"` prvo sakrije sve kolone koje je vratio SQL, pa pokaže samo navedene `<COLUMN>`.

Kolona se gađa s `NAME` (ime kolone iz SQL-a) ili s `INDEX` (1-based). Jedno od toga mora pogoditi rezultat.

| Atribut | Efekt |
|---|---|
| `WIDTH` | Širina (dijeli se internim faktorom) |
| `CAPTION` | Naslov. Na DX gridu `;` u captionu postaje novi red |
| `FORMAT` | Janus `FormatString` |
| `VISIBLE` | `true`/`false` |
| `ALIGN` | `LEFT`, `CENTER`, `RIGHT` — samo grid **unutar forme** (`Grid.Formatiraj`). Master i detail kolone (`Form1.CustomFormatKolona`) ovaj atribut ne čitaju |
| `AGREGATEFUNCTION` | Janus agregat (`Sum`, `Count`, …) |
| `NEGATIVERED`, `NOTZERORED` | `true` — crveni tekst ako je vrijednost &lt; 0 odnosno ≠ 0. Vrijedi za master/detail kolone |
| `POSITION` | Redoslijed |
| `AUTOSIZE` | `true` — širina po sadržaju |
| `MAXLINES` | Više redova + word wrap |
| `FORECOLOR`, `BACKCOLOR` | Imena boja: green, white, black, darksalmon, darkseagreen, limegreen, greenyellow, yellow, lightyellow, red |

DX grid dodatno čita `NEGATIVERED` i `NOTZERORED`.

`@@id` je kolona `id` trenutnog reda. Ako je nema, uzima se treća ćelija (indeks 2) na Janus gridu, odnosno prva kolona na DX gridu. Nema reda → `@@id` je `0`. `@@detid` je `id` donjeg grida.

## 3. Forme

Forma je element čije ime engine traži (`INSERTFORM`, `FILTERFORM`, …). Bitni atributi:

| Atribut | Efekt |
|---|---|
| `ONLOAD` | SQL koji puni kontrole (kolone = `NAME` kontrola) ili, ako počinje s `:`, druga forma |
| `ONLOAD2` | Drugi upit, dodatno podešavanje kontrola |
| `ONSUBMIT` | SQL na potvrdu (stari alias `STORA` se odmah prepiše s `ONSUBMIT`) |
| `ONCLOSED`, `ONCLOSING`, `ONACTIVATED`, `ONLOADED` | SQL na te događaje |
| `ONLOSTFOCUS` | SQL kad forma izgubi fokus |
| `NEXTFORM` | Forma koja slijedi |
| `FOCUS` | Ime kontrole koja dobije fokus nakon učitavanja |
| `WIDTH`, `HEIGHT` | Veličina |
| `RESIZE` | `false` — fiksna veličina, bez max/min |
| `MAXIMIZEBOX` | |
| `WINDOWSTATE` | |
| `CMD1CAPTION` | Natpis gumba Spremi. Klik pokupi parametre i izvrši `ONSUBMIT`, zatim zatvori formu |
| `CMD2CAPTION` | Natpis Odustani. Zatvara formu bez `ONSUBMIT` |
| `CMD3CAPTION`…`CMD6CAPTION` | `Natpis:ImeForme` — otvara tu formu |
| `FILLEXPLORER` | Nakon submita osvježi stablo / podlogu |
| `DBCLIPBOARD` | Pamćenje vrijednosti. Na formi čije ime sadrži `filter` default je `true`, osim ako je izričito `false` |
| `FORECOLOR`, `BACKCOLOR` | HTML boja koja počinje s `#` |
| `DETAILGRID` | Grid unutar forme |
| `PERMISIO`, `VERSION`, `HELPKOD`, `COMMANDTIMEOUT`, `TIMERSQL`, `CUSTOMCONNECTIONSTRING` | Kao na PROJECT-u |

Ime forme koje počinje s `MULTISELECT` ili `MULTIBLUE` prije otvaranja briše `TotalGenMultiSelectTemp` za tog operatera (`suser_sname()` i `@@operateriid`), pa ubacuje id označenih redova. Procedura čita tu tablicu, ne parametar `@id`.

## 4. INPUT

Puni popis atributa i parametara je u `NetGen-Kontrole.md`. Ovdje je sažetak.

`TYPE` se prepoznaje kao podstring (mala slova), ne kao tačan naziv. Zato:

- `textarea` upadne u `text`. Više redova je `MULTILINE="true"` na `TYPE="text"`.
- `checkbox` upadne u `check` (traži se `check`).
- `currency` upadne u broj jer sadrži `cur`.
- `dxgrid` mora ostati prije običnog `grid` (engine prvo traži `dxgrid`).

Zajednički atributi (gdje ih `SrediKontrolu` čita): `CAPTION`, `DEFAULT`, `TOP`, `LEFT`, `WIDTH`, `HEIGHT`, `ENABLED`, `VISIBLE`, `NAME`, `TABINDEX`, `PARENT`, `ANCHOR`, `SQL`, `REFRESHAFTERCHILD`, `HOTKEY`.

Ako je `TOP="0"` ili caption završava s `*`, engine sam slaže label lijevo od kontrole. `_` u captionu se briše, `!!` postaje novi red.

`REFRESHAFTERCHILD="TRUE"` pamti `SQL` te kontrole i ponovo je puni nakon child forme.

### Tipovi

| TYPE sadrži | Kontrola | Napomena |
|---|---|---|
| `label` | Label | |
| `group` ili `frame` | GroupBox | |
| `panel` | Panel | |
| `webbrowser` | WebBrowser | |
| `splitcontainer` | Split | |
| `check` | CheckBox | |
| `radio` | Radio | |
| `dxgrid` | DevExpress grid | mora biti prije običnog grida |
| `grid` | Janus grid | |
| `text` | TextBox | `null` postaje prazan string |
| `number` ili `cur` | Numerički unos | |
| `trazilica` ili `search` | Tražilica | |
| `dtpicker` ili `date` | Datum | |
| `combo` | Combo | |
| `multidrop` | Višestruki odabir | |
| `tree` | Stablo na formi | isti `yearmonth` / `tree` zapis kao PROJECT `TREE` |
| `button` | Gumb | `ONCLICK` |
| `help` | Help | |
| `picturebox` | Slika | |
| `statusstrip` | Status traka | |

### Combo

`SQL` mora vratiti **kolonu 0 = vrijednost, kolonu 1 = prikaz**. Engine ne gleda imena kolona, nego redoslijed: `ValueMember = Columns(0)`, `DisplayMember = Columns(1)`.

`ALLOWEMPTY="true"` ubaci prvi red `null` / prazan prikaz.

`ONCLICK` i `ONLOSTFOCUS` su SQL koji se izvrši na te događaje. `TRANSLATE="true"` prevodi tekst prikaza.

### Search

Dva oblika `SQL`:

1. Počinje s `sp` ili `select` — izvršava se taj upit (nakon zamjene `@@`).
2. Inače format s `#`:

```text
tablica#idKolona#kolonaPrikaza#%kolonaZaPretragu%
```

- dio 0 = tablica ili view
- dio 1 = id (ide u SELECT, ne u LIKE i ne u ORDER BY)
- od dijela 2 = kolone. `%` na početku ili kraju znači LIKE wildcard s te strane
- `!` u komadu = to polje ne ulazi u WHERE
- `/ostatak` = dodatni WHERE

`CAPTION` koji počinje s `_` ne šalje vrijednost u parametre. `ONLOSTFOCUS` je SQL nakon izlaska iz polja. `COLUMNCAPTION`, `COLUMNWIDTH`, `COLUMNAUTORESIZE` oblikuju grid dijaloga tražilice (`Form3`).

Vrijednost tražilice (id) je u `Tag`, tekst u `Text`. U SQL id ide kao `@@ime`. Tekst prikaza ide kao `@@searchboxime`. Combo uz id (`@@ime`) šalje i prikaz kao `@@comboboxime`.

`CAPTION` koji počinje s `_` izbacuje kontrolu iz parametara (ne samo search).

Grid na formi u parametre stavlja `@gridIme` (id reda), `@gridImeKolona` za ćelije, `@nizplavihajdijevaIme` (plavi redovi), `@nizoznacenihajdijevaIme` (checkbox) i `@rowcountIme`. Na master podlozi kolone tekućeg reda idu kao `@ImeKolone` (DataMember), bez prefiksa `grid`.

U SQL-u na dijalogu `@(ImeGrida..ImeKolone)` čita ćeliju trenutnog reda tog grida (`SrediSQLsaKolonamaizGrida`, `Form2.vb` 8440).

### Datum

Parametar čije ime počinje s `@DAT` (ili sadrži `GRID` i `DAT`) formatira se u `yyyyMMdd` unutar navodnika, osim kad je ispis (`DatumjezaIspis`). Prazna vrijednost postaje `null`.

## 5. Varijable `@@`

Zamjena je tekstualna, prije slanja na SQL Server. Imena su case-osjetljiva samo onoliko koliko ih `Replace` nabraja — piši oblik iz tablice.

### Uvijek (master `SrediSQL` i dijalog `SrediSQL2`)

| Zapis | Odakle |
|---|---|
| `@@id`, `@@ID`, `@@Id` | Id master reda. Na dijalogu to je `cls1.MasterID` (id s podloge), ne id kontrole na formi. Nema reda → `0` |
| `@@detid`, `@@DETID`, `@@DetId` | Id detail reda |
| `@@operateriid`, `@@OPERATERIID` | Ulogirani operater |
| `@@node`, `@@NODE` | Dio taga čvora prije `:` |
| `@@editedcolumn`, `@@EDITEDCOLUMN` | Naslov kolone koja se editira |
| `@@lastvalue`, `@@LASTVALUE` | Stara vrijednost ćelije |
| `@@selectedcolor`, `@@SELECTEDCOLOR` | Odabrana boja |
| `@@xmlname`, `@@XMLNAME` | Ime XML fajla bez putanje |
| `@@jupitersrvpath` | `remotedllpath` |

Samo na masteru: `@@table`, `@@TABLE` — `T1`…`T4` ako je grid hijerarhijski.

Samo na dijalogu: `@@tempstring`, `@@dblclickedcolumn`, `@@VERSION`, `@@OPISNIPARAMETRI`, `@@FOCUSEDCONTROL`, `@@selectedfile`, `@@selectedpath`.

### Kontrole i kolone grida

Svako polje koje sudjeluje u parametrima postaje `@@ime`. `ime` je `NAME` kontrole. Engine traži `@ime=` u nizu parametara prethodne forme i u clipboardu (CB).

- Nema vrijednosti → u SQL uđe `null`.
- Numerička vrijednost iz clipboarda: zarez postaje točka.
- `@napomena` koji bi bio `null` ide kao prazan string.
- Ime koje sadrži `ajdijeva` (niz označenih id-eva): `~` se pretvara u zarez.

### Ako `@@` ostane

`ExecSql` **ne šalje** upit ako je prazan, ako počinje s `@`, ili ako negdje iza prvog znaka još stoji `@@`. U log (Ctrl+F11) piše `SKIPPED`. Grid ostane prazan, bez SQL greške. Najčešći uzrok: ime kontrole se ne poklapa, čvor nije odabran pa je `@@node` prazan ali token je ostao, ili je upit krenuo s `@@` na poziciji 0 (taj jedan slučaj se ne preskače — `IndexOf("@@") > 0`).

## 6. Poruke iz SQL-a

`RAISERROR` / poruka koja sadrži `#` ili `+` prikazuje se kao informacija, ne kao kritična greška.

| U tekstu greške | Efekt |
|---|---|
| `#OK` ili `+OK` | Poruka se prikaže, greška se poništi, forma nastavlja |
| `#close` ili `+close` | Forma se zatvara |
| `#FOCUS(ime)` ili `+FOCUS(ime)` | Fokus na tu kontrolu |
| inače `#` / `+` | Informativni MsgBox, dio prije znaka |

`~~` u SQL-u postaje zarez prije izvršavanja (kad u tekstu ne smije stajati zarez).

## 7. Ispis

Na podlozi, akcija gumba:

- `PRINTMASTER` i `PRINTREPXMASTER` zovu isti ispis master grida (REPX).
- `PRINTDET` ispisuje detail.

REPX predlošci su u `SharedDLLs\repx`. `Form6` je dijalog pisača. `Form9` bira template ako ih ima više.

Desni klik na grid uvijek nudi i ugrađene stavke: spremi postavke, reset, export, grupiranje, format, izgled, ispis REPX, ispis. `PERMISIO` ih može ugasiti.

## 8. Česte greške

| Simptom | Uzrok |
|---|---|
| Grid prazan, u logu `SKIPPED` | U SQL-u je ostao `@@` |
| Combo prazan ili krivi prikaz | SQL ne vraća vrijednost u koloni 0 i tekst u koloni 1 |
| Gumb ne radi, ili puca na otvaranju | Nema `:` u `Natpis:Akcija` |
| Drugi gumb se pojavi umjesto druge komande | U `BUTTONS` je korišten `#` unutar jedne akcije; treba `~` |
| `@@id` je `0` ili krivi broj | Rezultat nema kolonu `id` |
| Stablo bez teksta ili bez djece | Procedura nema 4 kolone (id, parent, natpis, vrijednost) ili parent ne pokazuje na id |
| Filter se ne otvori na startu | `ONLOAD` ne počinje s `:` |
| Detail prazan i MsgBox `sorkac!` | `DETAILGRID` počinje s `:` |
| Tražilica ne filtrira | `SQL` nije ni `sp`/`select` ni `tablica#id#kolone` |
| Multiselect forma ne vidi redove | Ime forme ne počinje s `MULTISELECT` ili `MULTIBLUE`, pa se temp tablica ne puni |
