# ManagerObaveza — očekivano ponašanje

Izbornik traži `MER\ManagerObaveza.xml`. Te datoteke nema. Provjera ide na `definitions/netgen/BIOVEGA/ManagerObaveza.xml`. Rang 46, 6 baza, Biovega. Natpis je `Manager obveza`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni.

`TREE="spTreeKlMasterRoba :FILTEROPARTNERIFORM"`. Lijevo od `:` je `spTreeKlMasterRoba ` (s razmakom). Sadrži `tree`, pa ide u store (`4350–4351`, R01). Desno je forma `FILTEROPARTNERIFORM`.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Zatvori | `MULTIBLUEZATVORI` |
| 2 | Rezerviraj | `REZERVIRAJ` |
| 3 | Razdoblje | nema `:`, gumb se ne složi (`3320`, R60) |
| 4 | Filter | `FILTER` |
| 8 | Ispis | `PRINTREPXMASTER` |

`PRINTREPXMASTER` zove `IspisGRIDREPX` (`4945–4948`, R41). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Praznine

- `MER\ManagerObaveza.xml` nije u repou.
- `Razdoblje` ostaje skriveno (R60).
