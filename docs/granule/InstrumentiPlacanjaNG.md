# InstrumentiPlacanjaNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/InstrumentiPlacanjaNG.xml`. Rang 31, 8 baza, Biovega. XML se ne mijenja. `spKntNacinPlacNG` se ne šalje.

Izgled je Carpo (R74). Natpis je `Instrumenti plaćanja`. Nema `VERSION`, pa nema `v[]` (`Form1.vb` 3526–3527).

## Podloga

`ONLOAD="spKntNacinPlacNG @action='Grid'"` puni master (`3879–3882`). Nema `TREE`. Stablo se sakrije (`4310–4316`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unesi | `INSTRUMPLACINSERT#refreshmaster` |
| 2 | Izmijeni | `INSTRUMPLACUPDATE` |
| 3 | Obriši | `INSTRUMPLACDELETE` |
| 5 | Ispiši | `PRINTREPXMASTER` |

`refreshmaster` nije `REFRESHMASTERGRID` (R04). `PRINTREPXMASTER` zove `IspisGRIDREPX` (`4945–4948`, R41). Nema `CMD4`.

`MASTERDBLCLICK=":INSTRUMPLACUPDATE"`. Vodeća `:` otvara formu, pa se dvotočka skine (`4862–4867`).

`SETDEFAULTS` je jedan `select naziv as tkosmomi …`. Rezultat ide u context bar (`3958–3963`). `PAGEHEADERRIGHT="@@tkosmomi"` čita taj alias.

`HELPKOD="MER252"` upali `ButtonHELP` i F1 (`3491–3494`, `1356–1367`). Klik pokrene `JupiterHelp.exe` s `-h MER252` (`7781–7799`, R81). `ICON="credit_card.ico"` je datoteka iz `Graphics\Icons` (`3430–3440`). Prozor u portu ostaje ikona Managera (R74, izgled – Carpo).

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. `DETCHECKBOXVISIBLE="FALSE"` i `DETTOTALROWVISIBLE="FALSE"` su za detail (R79); `DETAILGRID` nema. `GROUPBYBOXVISIBLE="FALSE"`.

## Forme

`INSTRUMPLACINSERT`, `INSTRUMPLACUPDATE`, `INSTRUMPLACDELETE`. `ONLOAD` izmjene je `SpKntNacinPlacNG @action='Puni_UPD', @ID=@@ID`. Početak `sp` lijepi parametre (`Form2.vb` 3378–3388, R14).

## Praznine

- F1 i help gumb pale `JupiterHelp.exe` (R81). Manager ponuda taj gumb nema.
- `DETCHECKBOXVISIBLE` nije master kvačica (R79).
