# Predujmovi — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/Predujmovi.xml`. Rang 70, 4 baze. Natpis je `Predujmovi`. XML se ne mijenja. `spPredujmoviQueryNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` (`3879–3880`, R18). `COMMANDTIMEOUT="300"` (R65). `CHECKBOXVISIBLE="TRUE"`. `PAGEORIENTATION="LANDSCAPE"` ne dira `PRINTMASTER` (R41).

`BUTTONS`:

| # | Natpis | Naredba |
|---|---|---|
| 1 | Pregled | `FILTERFORM` |
| 2 | Proknjiži | `MULTISELECTPROKNJIZI` |
| 3 | Filter | `SAMOPREDFORM` |
| 4 | Ispiši | `PRINTMASTER` |

## Forme

`FILTERFORM` i `SAMOPREDFORM` imaju `FILLEXPLORER="TRUE"`. `ONLOAD` filtera ima `@@datumod` i `@@datumdo`, pa ga `ExecSql` preskoči (`Class1.vb` 113–118).

`MULTISELECTPROKNJIZI` je `@action=2` i knjiži. Na jupiteru se ne šalje. Ime nema `MULTISELECT_`.

## Praznine

- Proknjiži je upis.
- Prazan master na ulazu čeka OK.
