# KOD_RSPostavke — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KOD_RSPostavke.xml`. Rang 23, 8 baza, Biovega. XML se ne mijenja. `spKOD_RSPostavke_tree` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`VERSION="1.1"`. Naslov dobije `  v[1.1]` (`Form1.vb` 3524–3525). `PERMISIO="TRUE"` (`3414`). `COMMANDTIMEOUT="600"` (R65).

`TREE` ima tri dijela:

1. `Osvježi drvo:REFRESHTREEFORM` — lijevo nema riječ `tree`, pa je statički čvor (`4347–4356`).
2. `spKOD_RSPostavke_tree @action = 'postavke':TREEFORM` — lijevo sadrži `tree`, store čvor.
3. `spKOD_RSPostavke_tree @action = 'doprinosi':TREEFORM` — isti store obrazac.

`REFRESHTREEFORM` na čvoru ima `FILLEXPLORER` koji puni samo stablo ako je `onlytree` (R54). Ovdje se forma zove `REFRESHTREEFORM`.

`BUTTONS` reže se na `#` (R56):

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unesi | `DODAJ` |
| 2 | Izmijeni | `IZMJENA(@tablica='@@tablicaid',@id=@@id)` |
| 3 | Obriši | `BRISANJE(@tablica='@@tablicaid',@id=@@id)` |
| 4 | RS - statusi | `RS_STATUSI` |

Ime forme je lijevo od `(` (R55). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` gasi sumu (`2049`). `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`.

`FILLEXPLORER="true"` na projektu se ne čita (R70). `REFRESHAFTERCHILD="TRUE"` se upiše (`3396`) i dalje se na podlozi ne koristi (R77).

## Forme

`REFRESHTREEFORM`, `TREEFORM`, `DODAJ`, `IZMJENA`, `BRISANJE`, `RS_STATUSI`, `NOVI_RS_STATUS`, `IZMJENA_RS_STATUS`, `BRISANJE_RS_STATUS`.

## Praznine

- Dva store čvora dijele formu `TREEFORM`. Akcija je u SQL-u, ne u imenu forme.
- `IZMJENA(...)` i `BRISANJE(...)` nisu ime forme s zagradom (R55).
- `REFRESHAFTERCHILD` na projektu ne osvježava sam (R77).
- Timeout je 600 s (R65).
