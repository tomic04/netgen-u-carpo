# JeziciNG — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/JeziciNG.xml` (windows-1250). Stavka `JEZ-00`: `definitions/netgen/BIOVEGA/JeziciNG.xml` je isti SHA-256. Ponašanje je iz NetGen koda. Implementacija grida u `Carpo.NetGen` nije izvor.

Dijeljenje s 14 i VB zaokruživanje na parni su isti kao u `OznakaKL.md` (`Form2.vb:58`). `2500/14` = 178,571… → širina **179**, ne 178 (cjelobrojno dijeljenje u C# nije isto što i VB operator `/`).

## Podloga

| ID | Očekivano | NetGen |
|---|---|---|
| JEZ-01 | `<PROJECT>` se učita. | `Form1.vb:3263` |
| JEZ-02 | Naslov `Jezici`. Nema `VERSION`, nema ` v[`. Element `NAME` je `Projekt` i nije naslov. | `Form1.vb:3522-3528` |
| JEZ-03 | Nema `STARTUPSTORA`. Master SQL je `ONLOAD`: `spJeziciQuery @action=0, @id=''`. | `Form1.vb:3367-3368,3882` |
| JEZ-04 | `MASTERDBLCLICK=Izmijeni`. | `Form1.vb:3389,6935` |
| JEZ-05 | Nema `TREE` ni `DETAILGRID`. Stablo se sakrije, lijevi splitter je 0 (`JEZ-41`). Detail se ne puni (`JEZ-42`). | `Form1.vb:4310-4316,2111-2113` |
| JEZ-06 | Gumbi: 1 `Unesi` / `Unesi#refreshmaster`, 2 `Izmijeni` / `Izmijeni`, 3 `Obriši` / `Brisanje`, 5 `Ispiši` / `PRINTREPXMASTER`. Nema CMD4. Hash se u parseru ne lomi; lom je u `NovaGenerickaForma` na `#` (`4560`). | `Form1.vb:3314-3335` |
| JEZ-07 | `SETDEFAULTS`, `PAGEHEADERRIGHT=@@tkosmomi`, `HELPKOD=UTPT007`, `ICON=Jezici.ico`. | `Form1.vb:3371,3452,3489,3392` |
| JEZ-08 | `CHECKBOXVISIBLE`, `TOTALROWVISIBLE`, `DETCHECKBOXVISIBLE`, `DETTOTALROWVISIBLE`, `GROUPBYBOXVISIBLE` su `FALSE`, `FROZENCOLUMNS` je `0`. | `Form1.vb:3350,3362,3459-3462` |
| JEZ-09 | `CLEARCOLUMS=false` (nema N). `HEADERALIGNMENT=1`. Kolona `Naziv`, `WIDTH=2500`. Atribut je `AGGREGATEFUNCTION` (dva G), prazan. Kod traži `AGREGATEFUNCTION` (jedno G) i agregat ne veže. | `Form1.vb:3295-3296,7496` |

Redoslijed otvaranja, isti kao OznakaKL, uz ova dva SQL-a:

1. `SETDEFAULTS` se izvrši **prije** mastera (`3862-3866`, `SetDefolte` `3945`). Jedan segment (nema `#`). Rezultat: stupac `tkosmomi` ide u CB kao `tkosmomi=` + vrijednost (`3976-3998`). Nema stupca `formcaption`, naslov ostaje `Jezici`.
2. Zatim `PokaziUpit` s `spJeziciQuery @action=0, @id=''` (`3882`). U tekstu nema `@@`, pa se ne preskače (`Class1.vb:113`).

`JEZ-30` izvrši SETDEFAULTS (čisti `select`). `JEZ-31` pročita `object_definition` procedure i pozove je samo ako tijelo nema `insert/update/delete/merge/exec/into`. `spJeziciAzur` se nikad ne šalje.

`JEZ-40` ključ `sqlTrag` je točno: tekst SETDEFAULTS, ` || `, pa tekst ONLOAD.

`PERMISIO` nije u XML-u, default je `True` (`3153`), pa u trag uđe select nad `ssPermission` (`JEZ-71`, ključ `permisioSql=ssPermission`).

Help je vidljiv, tekst gumba je `?` (`Form1.designer.vb:399`). F1 i klik zovu `JupiterHelp.exe` s `-h UTPT007` (`7781-7799`, `JEZ-43`).

Ikona: `MOJremotedllpath\Graphics\Icons\Jezici.ico` (`3440`, `JEZ-44`). Ako datoteke nema, `On Error Resume Next` ostavi prethodnu.

## Master grid

Isti Janus put kao OznakaKL (nema `DXGRID`): ikona skrivena 22, selector 22 skriven jer je checkbox `FALSE`, zatim stupci iz procedure. `HeaderLines=3`, `EditMode=EditOff`, `EnterKeyBehavior=None`, višestruka selekcija, `EmptyRows`, total na kraju ugašen, group box skriven.

`CustomFormatKolona` nađe stupac `Naziv` po imenu (`INDEX` prazan → `Val` 0). `CLEARCOLUMS` nije `true`, ostali stupci iz rezultata ostaju. Širina `2500/14` = 179. Natpis `Naziv`. `FORMAT` i `VISIBLE` prazni ne diraju vrijednost. `HEADERALIGNMENT` `1` ide u `HeaderAlignment` samo te kolone (`7530-7531`). Dva G se ne čitaju (`7496` traži jedno G).

String stupac koji nije naveden u `COLUMN` dobije širinu **100** u `CreateGridColumns` (`Class1.vb:1203`). Širina 200 je samo hijerarhijski put `DodatnoFormatirajGridnakonRetriveStructure` (`Form1.vb:1799`), a ovdje `RELATION` nije `self` ni `multi`.

| ID | Ključ | Vrijednost |
|---|---|---|
| JEZ-45 | `selectorVidljiv` | `false` |
| JEZ-46 | `totalRow` | `false` |
| JEZ-47 | `groupByBox` | `false` |
| JEZ-48 | `kolona.Naziv.sirina` | `179` |
| JEZ-49 | `kolona.Naziv.headerAlignment` | `1` |
| JEZ-50 | `kolona.Naziv.agregat` | prazno |
| JEZ-51 | `clearSakrivaSve` | `false` |
| JEZ-52 | `stringSirina` | `100` |
| JEZ-53 | `editType` | `NoEdit` |
| JEZ-54 | `headerLines` | `3` |
| JEZ-55 | `gumb.5` | `Ispiši\|PRINTREPXMASTER\|true` |
| JEZ-72 | `enterKey` | `None` |
| JEZ-73 | `cb.tkosmomi` | `ima` |

## Ispiši

`PRINTREPXMASTER` ne otvara formu. `NovaGenerickaForma2` vidi prefiks i zove `IspisGRIDREPX`, pa `Exit Sub` (`4945-4948`). To je isti poziv kao `PRINTMASTER` (`4951-4955`).

`PageHeaderRight` je `@@tkosmomi` propušten kroz `SrediSQL` i CB (`6001`, `Class1.vb:932-966`). Vrijednost je ona iz SETDEFAULTS, bez dodatnih navodnika ako nije datum. `PageHeaderCenter`: nema `PAGEHEADERCENTER` ni `v[` u naslovu `Jezici`, pa `Substring` do `v[` baci, a `On Error Resume Next` ostavi centar prazan (`5996-5997`).

| ID | Ključ | NetGen |
|---|---|---|
| JEZ-56 | `ispis=PRINTREPXMASTER` | `Form1.vb:4945` |
| JEZ-57 | `pageHeaderRight=@@tkosmomi` | `Form1.vb:6001` |

## Dijalozi

Nema `CMD1`–`CMD6` na formama. Gumbi su `INPUT TYPE="button"`. Panel s Prihvati/Odustani je skriven (`2750`). `ONCLICK` ide u `Glbl_ClickButton` → `Rafalno` (`6221-6228`), ne u `Button1` dok segment nije `OK` / `OKStay` / `CLOSE`.

`TEXT=ButtonNG0` se ne čita. Natpis je `CAPTION` (`JEZ-23`). `frame` je `GroupBox`. Nema `CAPTION`, `TEXT=FrmJezici` se ne čita, natpis okvira ostaje prazan (`JEZ-24`, `JEZ-59`).

`BORDERSTYLE=3` nema `GetNamedItem` u učitavanju forme. Ostaje WinForms `Sizable` (`JEZ-20`, `JEZ-69`). `REFRESHAFTERCHILD` na **formi** `SrediKontrolu` ne vidi; taj kod gleda samo `INPUT` (`3058`, `JEZ-21`). Ne radi poseban refresh zbog tog atributa.

Zvjezdica: `SIZE=13` postavi veličinu fonta (`3077-3081`, `JEZ-22`, `JEZ-60`). `BOLD` se ne čita.

### Unesi

Nema `ONSUBMIT`. `NEXTFORM=Dodaj`. Visina `2146/14` = 153, širina `5112/14` = 365.

`Spremi` (`buttonOK`), lanac po `#`:

1. `spJeziciAzur @action=0, @id=@@id, @Naziv='@@Naziv'` — sadrži `@@`, pa `PokupiParametre` i zamjena (`6812-6817`). Zatim `RefreshControls` (`7510`) koji to **izvrši**. Nema lijepljenja drugog niza `@ime=` (to radi samo `Button1` kad `ONSUBMIT` ne počinje s `INSERT|DELETE|UPDATE|SELECT`). Apostrof u nazivu ide `¸¸` pa `''`.
2. `OKStay`: `NeZelimClosatFormu=True`, `Button1_Click`. `ONSUBMIT` je prazan, forma se **ne** zatvori (`6884-6887`).
3. `select '' as naziv, 'naziv' as focus`: stupac `focus` fokusira kontrolu `naziv` (`4627-4632`), stupac `naziv` upiše prazan tekst u `Naziv` (`4974-4979`).

`JEZ-61` `ostajeOtvorena=true`, `JEZ-62` `fokus=naziv`, `JEZ-70` `sqlImaZalijep=false`. Cilj kuke je `Unesi.buttonOK`. Test ne šalje `spJeziciAzur`.

`Odustani` je `ONCLICK=ok`, ne `close` (`JEZ-13`). To je `Button1_Click`: `OKClick=True`, forma se zatvori. Roditelj vidi `NEXTFORM=Dodaj` (`5381`). `Dodaj` ne postoji u XML-u: `BrojKontrola=0`, `OKClick=True` bez dijaloga i bez poruke (`5354-5370`, `JEZ-63`, `JEZ-64`). Zatim, jer je `Zadnji` true, master se ponovo puni zadnjim SQL-om.

Gumb podloge je `Unesi#refreshmaster`. Nakon što se dijalog zatvori, drugi segment **nije** `REFRESHMASTERGRID` (nema te riječi). Pada u `NovaGenerickaForma2("refreshmaster")`: forma ne postoji, 0 kontrola, `OKClick` true, pa `PokaziUpit` zadnjeg SQL-a (`5519`). Master se osvježi iako sinonim nije uveden (`JEZ-65`). To je posljedica prazne forme, ne posebne naredbe.

### Izmijeni

`ONLOAD`: `select * from jezici where jeziciid=@@id`. `@@id` je `MasterID` s mastera (`SrediSQL2` `2880`). Ako ima red, `Naziv` dobije isti stupac (`2016`, `JEZ-14`, `JEZ-66`).

`Spremi`: azur `@action=1` pa `OK`. Forma se zatvori, `OKClick` true, nema `NEXTFORM`, master se osvježi (`JEZ-15`, `JEZ-68`). Cilj `Izmijeni.buttonOK`.

`Odustani` je `CLOSE` → `Button2_Click`, `OKClick` false, nema SQL-a ni refresha (`7033`, `5429`, `JEZ-16`, `JEZ-67`). Cilj `Izmijeni.buttonCLOSE`.

Dvostruki klik mastera otvara istu formu (`JEZ-58`).

### Brisanje

Natpis: `Jeste li sigurni da želite obrisati označeni jezik?` (`JEZ-19`).

`Obriši`: `spJeziciAzur @action=2, @ID=@@ID, @Naziv=''#OK` (`JEZ-17`). SQL se izvrši u `Rafalno`, zatim `OK` zatvori i roditelj osvježi master.

`odustani`: `close` (`JEZ-18`), isto što i `CLOSE`, bez SQL-a.

Visina `1699/14` = 121, širina `4608/14` = 329.

## Što namjerno ne radi

- `AGGREGATEFUNCTION` s dva G ne zbraja.
- `BORDERSTYLE` i `REFRESHAFTERCHILD` na formi ne mijenjaju formu.
- `TEXT` na gumbu i okviru nije natpis.
- `refreshmaster` nije alias za `REFRESHMASTERGRID`, ali prazna forma tog imena ipak ponovo pošalje zadnji master SQL.
- `Dodaj` se traži i ne nađe, bez dijaloške greške.
