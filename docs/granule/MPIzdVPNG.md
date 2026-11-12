# MPIzdVPNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPIzdVPNG.xml`. Rang 64, 4 baze. Natpis je `Izdatnica iz VP u MP`. XML se ne mijenja. `spMPQueryGlaIDetNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). `DETAILGRID` veže `@id=@@id` (R11).

`TREE` počinje s `Danas:DANAS_FORM#Jučer:JUCER_FORM`, pa `spMPTreeNG` (store, `4350–4351`). Pet formi ima `FILLEXPLORER="TRUE"`.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Filter | `FILTERFORM` |
| 2 | Izmjeni | `UPDATEFORM` |
| 4 | Zaključi | `ZAKLJUCI` |
| 5 | Lista | nema `:`, gumb se ne složi (`3320`, R60) |
| 6 | Ispis dok | `vrstadokid-MaloprodajaGla` (R43) |

`MASTERDBLCLICK="UPDATEFORM"`. `POPUPMASTER` ima Filter, Izmjeni, Isporuči `ZAKLJUCI`, Poništi `PONISTI`. `SETDEFAULTS` je `spMPPermissionNG` s `@@xmlname` i `@sifradok = '104'`. `CHECKBOXVISIBLE="FALSE"`.

## Praznine

- Prazan master na ulazu čeka čvor.
- `Lista` je skrivena. Ispis je `vrstadokid-`.
