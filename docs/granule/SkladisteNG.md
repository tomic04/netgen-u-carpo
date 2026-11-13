# SkladisteNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/SkladisteNG.xml`. Rang 26, 8 baza, Biovega, četiri ista retka izbornika. XML se ne mijenja. `spSkladisteQueryNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spSkladisteQueryNG @action=8"`. Ne počinje s `:`, pa puni master (`Form1.vb` 3879–3882). Nema `TREE` ni `DETAILGRID`. Stablo se sakrije (`4310–4316`).

`BUTTONS` (R56):

| # | Natpis | Naredba |
|---|---|---|
| 1 | Dodaj | `INSERT` |
| 2 | Izmjeni | `UPDATE` |
| 3 | Obriši | `DELETE` |
| 4 | Ispis | `PRINTMASTER` |

`PRINTMASTER` zove `IspisGRIDREPX` (`4951–4955`, R41). `MASTERDBLCLICK="UPDATE"`.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"` gasi sumu (`2049–2050`). `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`.

## Forme

`INSERT`, `UPDATE`, `DELETE`. `ONSUBMIT` koji počinje sa `sp` lijepi parametre (`Form2.vb` 3378–3388). Greška `ONLOAD` ne otvara formu (`1199`).

## Praznine

- Ispis je `PRINTMASTER`, ne REPX predložak (R41).
- Četiri retka izbornika dijele istu datoteku.
