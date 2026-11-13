# MPIzduMPZaprimi — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPIzduMPZaprimi.xml`. Šest baza, Biovega. Natpis je `Zaprimanje robe u MP sa skladišta`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan (`3879`).

`TREE` počinje s `Granula >>> Narudžbe:#Danas:DANAS_FORM`. Prvi segment ima `:`, natpis je `Granula >>> Narudžbe`, naredba je prazna. Nije roditelj sljedećih (R10). Store je `spMPTree`, ne `spMPTreeNG` (`4350–4351`).

`DETAILGRID` veže `@id=@@id` (R11). `MASTERDBLCLICK="UPDATE_FORM"`. `POPUPMASTER` zove `UPDATEFORM`, bez crtice. To su dva imena.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 4 | Zaključi | `ZAKLJUCI` |
| 6 | Print dok. | `ISPIS_FORM` |
| 7 | Lista | nema `:`, skriven (`3320`, R60) |

`SETDEFAULTS` zove `spMPPermissionNG`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Unos

`INSERTFORM` ima `NEXTFORM="UPDATE_FORM"` i `ONSUBMIT` koji počinje s `spMPInsertGlaIdet @action=1`. Prije sljedeće forme taj upis ide (`4435–4439`). Na Jupiteru se ne šalje. Zaključak isto.
