# NetGen XML — cheatsheet

Kratki obrasci. Ponašanje je ono iz `NetGen-XML-Pravila.md` / `NetGen-Engine.md` (kod `b0d5d41`). Ako se ovo i engine raziđu, vrijedi engine.

## Master + detail

```xml
<PROJECT NAME="Primjer"
         ONLOAD="spPrimjer_Query @action=1,@operateriid=@@operateriid"
         DETAILGRID="spPrimjer_Query @action=2,@id=@@id"
         MASTERDBLCLICK="UPDATEFORM"
         BUTTONS="Unos:INSERTFORM#Izmjena:UPDATEFORM#Obriši:DELETEFORM~REFRESHMASTERGRID"
         CHECKBOXVISIBLE="FALSE"
         GROUPBYBOXVISIBLE="FALSE"
         FROZENCOLUMNS="1">
  <CAPTION>Primjer</CAPTION>
  <MASTERCOLUMNS>
    <COLUMN NAME="broj" CAPTION="Broj" WIDTH="80"/>
    <COLUMN NAME="naziv" CAPTION="Naziv" WIDTH="220"/>
    <COLUMN NAME="iznos" CAPTION="Iznos" WIDTH="90" FORMAT="n2" AGREGATEFUNCTION="Sum" NEGATIVERED="true"/>
  </MASTERCOLUMNS>
</PROJECT>
```

`DETAILGRID` je SQL, ne `:FORMA`. `@@id` traži kolonu `id`.

## Filter na startu

```xml
ONLOAD=":FILTERFORM"
```

Na formi:

```xml
<FILTERFORM ONLOAD="select convert(varchar(8),getdate(),112) as datod"
            ONSUBMIT="spPrimjer_Query @action=1,@datod=@@datod,@operateriid=@@operateriid"
            FILLEXPLORER="TRUE" WIDTH="420" HEIGHT="220">
  <INPUT TYPE="date" NAME="datod" CAPTION="Datum od:"/>
</FILTERFORM>
```

Ime forme sadrži `filter` → vrijednosti se pamte (`DBCLIPBOARD`), osim `DBCLIPBOARD="false"`.

## Gumbi i lanac

```text
BUTTONS="Natpis:AKCIJA#Natpis2:AKCIJA2"
```

Više koraka u jednom gumbu: `~`, ne `#`.

```text
Obriši:DELETEFORM~REFRESHMASTERGRID
```

Desni klik: `POPUPMASTER="Izmjena:UPDATEFORM#Obriši:DELETEFORM"`.

## Tree

```xml
TREE="Danas:FILTERDANAS#spMPTreeNG @action=2:MAINFORM"
```

Procedura stabla, redom: id, parent id, natpis, vrijednost za `@@node`.

```sql
select id, parentid, naziv, value from ...
```

Klik otvara formu iza `:` i stavlja `@@node` na dio prije `:`.

## Combo i search

Combo: kolona 0 = id, kolona 1 = tekst.

```xml
<INPUT TYPE="combo" NAME="vrstadokid" CAPTION="Vrsta:"
       SQL="select vrstadokid, opis from vrstadok"
       ALLOWEMPTY="true"
       ONLOSTFOCUS="select 'x' as lblOpis"/>
```

Search preko viewa:

```xml
<INPUT TYPE="search" NAME="partneriid" CAPTION="Partner:"
       SQL="viPartneri#partneriid#Naziv#%Naziv%"/>
```

Search preko procedure (upit se izvršava kakav jest, nakon `@@`):

```xml
<INPUT TYPE="search" NAME="robaid" CAPTION="Roba:"
       SQL="spTraziRoba @action='Roba',@uvjet='@@searchboxrobaid'"/>
```

## Forma unosa

```xml
<INSERTFORM ONSUBMIT="spPrimjer_Azur @action=0,@naziv='@@naziv',@datum=@@datum"
            ONLOAD="select '' as naziv"
            WIDTH="480" HEIGHT="260">
  <INPUT TYPE="text" NAME="naziv" CAPTION="Naziv*"/>
  <INPUT TYPE="date" NAME="datum" CAPTION="Datum*"/>
</INSERTFORM>
```

`TOP="0"` ili `*` na captionu: engine sam doda label.

## Multiselect

Ime forme mora početi s `MULTISELECT` ili `MULTIBLUE`. Engine napuni `TotalGenMultiSelectTemp` za `@@operateriid`. Procedura čita tu tablicu.

## Ispis

```text
CMD2CAPTION="Ispis:PRINTMASTER"
```

`PRINTDET` za donji grid. Oba master ispisa (`PRINTMASTER`, `PRINTREPXMASTER`) idu na REPX.

## @@ koje treba pamtiti

| Token | Smisao |
|---|---|
| `@@id` | Red mastera, kolona `id`, inače `0` |
| `@@detid` | Red detaila |
| `@@operateriid` | Operater |
| `@@node` | Čvor, dio prije `:` |
| `@@imeKontrole` | Vrijednost polja `NAME="imeKontrole"` |
| `@@xmlname` | Ime XML fajla |

Ostane li `@@` u upitu, upit se ne izvrši (`SKIPPED` u Ctrl+F11).

## Poruka iz procedure

| Tekst | Efekt |
|---|---|
| `...#OK` | Info, nastavi |
| `...#close` | Zatvori formu |
| `...#FOCUS(naziv)` | Fokus na kontrolu |

## Prije nego što se atribut proglasi ispravnim

Ako ga nema u pravilima, u `netgen` potraži `GetNamedItem("ATRIBUT")`. Stari fajl `docs/netgen/netgen_master_global_rules_detaljno_updated.md` je popis primjera, ne dozvola. `@@GRIDROWCOUNT` ne radi.

Cijeli rječnik komandi (što je živo na dijalogu, a mrtvo na podlozi): `NetGen-Komande.md`. Atributi po `TYPE`: `NetGen-Kontrole.md`. Redoslijed učitavanja i submita: `NetGen-Tok.md`.
