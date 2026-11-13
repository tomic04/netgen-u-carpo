# KntKonfShemeMatTem — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KntKonfShemeMatTem.xml`. Rang 39, 7 baza, Biovega. Natpis je `Konfigurator Mat Tem`. XML se ne mijenja. `spKntKonfShemeMatTem` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spKntKonfShemeMatTem 1"` ne počinje s `:`, pa puni master (`3879–3882`). Nema zareza ni `@action`. To je poziv s jednim pozicijskim argumentom. Nema `TREE`. Stablo se sakrije (`4310–4316`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unos | `INSERTFORM` |
| 2 | Izmjeni | `UPDATEFORM` |
| 3 | Kopiraj | `COPYFORM` |
| 4 | Obriši | `DELETEFORM` |

`MASTERDBLCLICK="UPDATEFORM"`. `CHECKBOXVISIBLE="FALSE"`. `PAGEORIENTATION="LANDSCAPE"` mijenja samo `IspisNovi` (`5930`). Nema gumba ispisa, pa se orijentacija na ovoj podlozi ne primijeni (R41).

## Forme

`INSERTFORM`, `UPDATEFORM`, `COPYFORM`, `DELETEFORM`. `ONSUBMIT` počinje sa `spKntKonfShemeMatTem`, pa se niz `@ime=` doda (`Form2.vb` 3378–3388, R14). `UPDATEFORM` `ONLOAD` puni kontrolu iz `select km.vrstapromid…`.

## Praznine

- `ONLOAD` je `spKntKonfShemeMatTem 1`, ne `@action=1`.
- `LANDSCAPE` nema kome promijeniti ispis (R41).
