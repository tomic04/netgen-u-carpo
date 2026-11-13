# PDVpoStjecanju — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PDVpoStjecanju.xml`. Rang 71, 4 baze. Natpis je `PDV po stjecanju`. XML se ne mijenja. `spKntPDVpoStjecanju` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` (`3879–3880`, R18). `CHECKBOXVISIBLE="FALSE"`. `PAGEORIENTATION="LANDSCAPE"` ne dira `PRINTMASTER` (R41).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Prikaz | `FILTERFORM` |
| 2 | Prebaci u knjigu IRA | `PREBACIFORM` |
| 3 | Ispis | `PRINTMASTER` |

## Forme

`FILTERFORM` ima `FILLEXPLORER="TRUE"`. `ONSUBMIT` je `spKntPDVpoStjecanju @action=0,`. `ONLOAD` ima `@@datod` i `@@datdo`, pa se preskoči (`Class1.vb` 113–118).

`PREBACIFORM` je `@action=1`. To prebacuje u knjigu. Na jupiteru se ne šalje.

## Praznine

- Prebaci u knjigu IRA je upis.
- Prazan master na ulazu čeka prikaz.
