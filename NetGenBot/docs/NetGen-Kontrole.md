# NetGen kontrole (INPUT)

Izvor: `Form2.vb`, grane po `TYPE` od 1256 do 2707, parametri u `PokupiParametre` od 3406. Commit `b0d5d41`.

`TYPE` nije tačan naziv. Engine radi `InStr` nad malim slovima, redom. Prva grana koja pogodi uzima kontrolu.

| Pišeš | Upadne u | Zašto |
|---|---|---|
| `textarea` | `text` (1996) | sadrži `text`. Više redova: `MULTILINE="true"` |
| `checkbox` | `check` (1460) | sadrži `check` |
| `currency` | broj (2071) | sadrži `cur` |
| `dxgrid` | DX grid (1579) | mora biti prije `grid`, i jest |
| `grid` | Janus grid (1623) | samo ako `dxgrid` nije već uzet |

Zajedničko, `SrediKontrolu` (`Form2.vb` 2924): `CAPTION`, `DEFAULT`, `TOP`, `LEFT`, `WIDTH`, `HEIGHT`, `ENABLED`, `VISIBLE`, `NAME`, `TABINDEX`, `PARENT`, `ANCHOR`, `SQL`, `REFRESHAFTERCHILD`, `HOTKEY`.

`TOP="0"` ili caption na `*` složi label `lbl`+ime. `_` u tekstu se briše, `!!` je novi red. `TABINDEX="-2"` ili prazan dobije automatski red. `REFRESHAFTERCHILD="TRUE"` pamti SQL i ponovo puni kontrolu nakon child forme.

`CAPTION` koji počinje s `_` ne ulazi u parametre (search: `SudjelujeUParametrima=False`, 2149).

`ONLOAD` prvog reda: kolona čije je ime = `NAME` upisuje se u kontrolu. `null` na tekstu postaje prazan string.

## text — `TeksT` (1996)

Atributi: `DEFAULT`, `ENTERASTAB`, `MULTILINE`, `PASSWORDCHAR`, `SCROLLBARS`, `WORDWRAP`, `MAXLENGTH`, `ONLOSTFOCUS`, `ONCHANGE`, `ONGOTFOCUS`, `NFCSQL`, `HOTKEY`.

Šalje `@ime='tekst'`. Apostrof u tekstu postaje `¸¸`, zarez `~~` (engine ih kasnije vrati). Prazno može otići kao prazan string, ne kao `null`.

## number / cur — `NumerBox` (2071)

Atributi: `FORMATMASK`, `DECIMALS`, `DEFAULT`, `VALUE` (prepiše default), `MAXLENGTH`, `ONLOSTFOCUS`, `HOTKEY`.

Početni tekst je `0`. Šalje `@ime=` broj bez navodnika. Prazno postaje `0`.

## date / dtpicker — `DTPicker` (2232)

Atributi: `FORMAT`, `CUSTOMFORMAT`, `DEFAULT`, `SHOWCHECKBOX`, `CHECKED`, `ONLOSTFOCUS`, `ONVALUECHANGED`, `HOTKEY`, `CAPTION`.

`DEFAULT`:

| Vrijednost | Datum |
|---|---|
| `firstdayofyear` | 1. 1. tekuće godine |
| `firstdayoflastyear` | 1. 1. prošle |
| `firstdayofmonth` | 1. tekući mjesec |
| `firstdayoflastmonth` | 1. prošli mjesec |
| ostalo | literal koji picker razumije |

`SHOWCHECKBOX=true` na početku gasi kvačicu (2277), zatim `CHECKED` može vratiti. Ako `ONLOAD` uspije parsirati datum i checkbox je uključen, kvačica se pali (2306).

Šalje `@ime='yyyyMMdd'` ako je kvačica upaljena, inače `null`. Format ovisi o `FORMAT` / `CUSTOMFORMAT` (sat, mjesec). U parametrima se ime koje počinje s `@DAT` posebno pakuje — vidi `NetGen-XML-Pravila.md`, odjeljak varijabli.

## combo — `Combo` (2370)

Atributi: `SQL`, `ONCLICK`, `ONLOSTFOCUS`, `ALLOWEMPTY`, `TRANSLATE`, `HOTKEY`, `CAPTION`.

Kolona 0 = vrijednost, kolona 1 = prikaz. Imena kolona se ne gledaju. `ALLOWEMPTY="true"` ubaci red `null` / prazan tekst na vrh. `TRANSLATE="true"` prevodi prikaz.

Šalje `@ime=<SelectedValue>` ili `null`. Prikaz ide i kao `@comboboxime='tekst'`, u XML-u `@@comboboxime`.

Ako ništa nije odabrano i `ALLOWEMPTY` nije true, engine pokuša `SelectedValue=1`. Ne uspije li, šalje `null`.

## multidrop (2478)

Isti SQL: kolona 0 i 1. `ONLOSTFOCUS` se veže na gubitak fokusa.

Šalje `@ime='id1~id2~'` (zarez iz odabira postaje `~`, razmaci se brišu, 3446).

## search / trazilica — `SearchBox` (2140)

Atributi: `SQL`, `CAPTION`, `ONLOSTFOCUS`, `ONLEAVE`, `COLUMNCAPTION`, `COLUMNWIDTH`, `COLUMNAUTORESIZE`, `HOTKEY`.

Dva oblika `SQL` (`Form3.Trazilica`, 99):

- počinje s `sp` ili `select` — taj upit, nakon `@@`
- inače `tablica#idKolona#kolonaPrikaza#%kolona%`

`%` je LIKE s te strane. `!` izbaci polje iz WHERE. `/ostatak` je dodatni WHERE. Razmaci u upisu su AND grupa.

Id je u `Tag`, tekst u `Text`. Šalje `@ime=<id>` ili `null`, plus `@searchboxime='tekst napisan u polju'` (`@@searchboxime`).

`ONLOAD` puni `Tag` iz kolone `NAME`, tekst iz kolone čije je ime caption prije `#`.

## check / radio (1460, 1509)

`DEFAULT` je početno `Checked`. `ONCLICK` je SQL ili lanac komandi.

Check šalje `@ime=` stanje kvačice. Radio šalje `1` ili `0`.

## button / help (2614, 2672)

`ONCLICK` ide u `Tag` i izvršava se kao `Rafalno` na klik. `HOTKEY`, `IMAGEINDEX`, `DOCK`. `help` otvara help, ne šalje parametar. Dugme ne ulazi u `@ime`.

## label, group, frame, panel (1296, 1354, 1384)

Kontejner ili natpis. Label: `ONCLICK` u `Tag`, `BACKCOLOR`, `DOCK`, `PARENT`, `ANCHOR`. `ONLOAD` može promijeniti `Text` ako postoji kolona imena labele.

`group` i `frame` su isti `GroupBox`.

## grid na formi (1623) i dxgrid (1579)

Atributi koje grana čita:

`SQL`, `SQL2`, `SQL3`, `SQL4`, `RELATION`, `SKIPDEFAULTLOAD`, `EDITCOLUMNS`, `AUTOMATICSORT`, `MULTIGRIDLAYOUT`, `ONCLICK`, `ONDBLCLICK`, `ONENTERKEY`, `ONCELLEDITED`, `BEFORECELLEDITED`, `CLEARCOLUMS`, `GROUPBYBOXVISIBLE`, `COLUMNHEADERS`, `COLUMNHEADERFORMAT`, `POPUP`, `PAGEHEADERLEFT/CENTER/RIGHT`, `PAGEFOOTER*`, `PAGEORIENTATION`, `AFTERREFRESH`, `FROZENCOLUMNS`, `FILTERMODE`, `TRANSLATECOLUMNS`, `TOTALROW`, `CHECKBOXVISIBLE`, `DOCK`, `HOTKEY`, `PARENT`, `ANCHOR`.

`SKIPDEFAULTLOAD="TRUE"` ne puni grid na otvaranju. `EDITCOLUMNS` smije biti `select`/`sp` koji vrati listu. `CHECKBOXVISIBLE="FALSE"` sakrije kolonu 1. `TOTALROW="FALSE"` gasi sumu; inače je suma uključena.

Djeca `<COLUMN>` formatira `Grid.Formatiraj` (tu postoji `ALIGN`). Vidi `NetGen-Engine.md`, odjeljak grida.

Šalje, za fokusirani red i cijeli grid:

| Parametar | Sadržaj |
|---|---|
| `@gridIme` | id trenutnog reda |
| `@gridImeKolona` | ćelija te kolone |
| `@nizplavihajdijevaIme` | plavi redovi, id-evi odvojeni `~` |
| `@nizoznacenihajdijevaIme` | checkbox redovi |
| `@rowcountIme` | broj redova |

U XML-u isto s `@@`. U samom SQL-u grida `@(ImeGrida..ImeKolone)` čita ćeliju bez prolaska kroz parametar (`SrediSQLsaKolonamaizGrida`, 8440).

`ONCLICK`, `ONDBLCLICK`, `ONCELLEDITED`, `BEFORECELLEDITED`, `ONENTERKEY`, `AFTERREFRESH` su lanci komandi ili SQL.

## tree na formi (2558)

`SQL` je isti zapis kao `TREE` na PROJECT-u: segmenti `#`, `yearmonth:`, nešto što sadrži `tree` puni iz procedure, inače statički čvor. `ONCLICK`.

Šalje `@ime='cijeli Tag čvora'`, ne samo dio prije `:`. To nije isto što `@@node` na podlozi.

## ostalo

| TYPE | Linija | Napomena |
|---|---|---|
| `webbrowser` | 1414 | `URL`. `DodatnoPostimajKontrolu` s tekstom radi `Navigate` |
| `picturebox` | 2707 | `ONCLICK`. `ONLOAD` blob u koloni `NAME`, putanja u `NAMElocation` |
| `splitcontainer` | 1451 | `ORIENTATION`, `DOCK` |
| `statusstrip` | 1256 | `TEXT1`–`TEXT3` su SQL u tagu labela |

## Šta kontrola ne radi

Nema odvojenog `listbox`, `tab`, `password` tipa. Lozinka je `TYPE="text"` i `PASSWORDCHAR`. Nema `displayfield` na combu: samo redoslijed kolona.
