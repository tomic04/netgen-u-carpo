# RekapitulacijaUI — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/RekapitulacijaUI.xml`. Izbornik traži `SKLAD\RekapitulacijaUI.xml`. Te putanje nema. Rang 75, 3 baze, tri ista retka. Natpis je `Promet na skladištu`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":Parametri#refreshmaster"`. Prvi segment otvara `Parametri` (`3879–3880`). Drugi je `refreshmaster`, ne `REFRESHMASTERGRID` (R04). `COMMANDTIMEOUT="180"` (R65). `MASTERDBLCLICK="KARTICA"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="TRUE"` ostavlja sumu.

| # | Natpis | Naredba |
|---|---|---|
| 1 | P&arametri | `Parametri` |
| 2 | &Kartica | `KARTICA` |
| 4 | Is&piši | `PRINTREPXMASTER` |

`PRINTREPXMASTER` zove `IspisGRIDREPX` (`4945–4948`, R41). `SETDEFAULTS` puni `tkosmomi`.

## Forme

`Parametri` ima `FILLEXPLORER="true"`. `ONLOAD` ima `@@fskladisteid`. To `ExecSql` preskoči (`Class1.vb` 113–118). `KARTICA` ima `FILLEXPLORER="false"` i `ONLOAD` s `raiserror` ako nema id.

## Praznine

- `SKLAD\RekapitulacijaUI.xml` nije u repou.
- `refreshmaster` nije `REFRESHMASTERGRID` (R04).
