# MPNarFFirma — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPNarFFirma.xml`. Rang 94, 2 baze, Biovega. Natpis je `Narudžba franšizera prema Centrali`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. `DETAILGRID` veže `@id=@@id` (R11). `COMMANDTIMEOUT="300"` (R65). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

`TREE` počinje s `Granula >>> Narudžbe:#Danas:DANAS_FORM#…`. Prvi segment ima `:`, natpis je `Granula >>> Narudžbe`, naredba je prazna. Nije roditelj sljedećih segmenata (R10). `spMPTree` sadrži `tree` i ide u store (`4350–4351`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Dodaj | `INSERTFORM` |
| 3 | Izmjeni | `UPDATE_FORM` |
| 4 | Zaključi | `ZAKLJUCI` |
| 6 | Ispis dok | `vrstadokid-propongla` (R43) |
| 7 | Lista | nema `:`, skriven (`3320`, R60) |

`MASTERDBLCLICK="UPDATE_FORM"`. `POPUPMASTER` ima Filter, Izmjeni i Zaključi.

## Praznine

- Prazan master na ulazu čeka čvor.
- Ispis je `vrstadokid-propongla` (R43).
