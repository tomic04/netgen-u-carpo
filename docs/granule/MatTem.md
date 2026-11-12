# MatTem — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MatTem.xml`. Rang 32, 8 baza, Biovega. XML se ne mijenja. `spKntMatKnjizenje` se ne šalje.

Izgled je Carpo (R74). Natpis je `Materijalno knjiženje`.

## Podloga

`ONLOAD=":FILTERFORM"` otvara dijalog, ne puni master SQL-om (`3879–3880`, R18). Nema `TREE`. Stablo se sakrije (`4310–4316`). `MASTERDBLCLICK` je prazan.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Proknjiži | `PROKNJIZIFORM` |
| 3 | Ispis | `PRINTMASTER` |

`CMD2CAPTION` ima razmak iza imena. `Trim` prije reza na `:` ga skine (`3326`). `PRINTMASTER` zove `IspisGRIDREPX` (`4951–4955`), ne `IspisNovi`. `PAGEORIENTATION="LANDSCAPE"` mijenja samo `IspisNovi` (`5930`, R41). Ispis gumba ostaje kako `IspisGRIDREPX` složi.

`COMMANDTIMEOUT="300"` (`3463`, R65). `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="TRUE"`. `PAGEHEADERLEFT="Materijalno knjiženje za: @@comboboxdatum"`. `PAGEFOOTERLEFT="@@TKOSMOMI"`.

## Forme

`FILTERFORM` `FILLEXPLORER="TRUE"`: nakon OK-a `ONSUBMIT` zamijeni master (`5445–5452`). Tekst završava s `!!`, pa se parametri ne lijepe (`5449`, R14). `ONSUBMIT` je `spKntMatKnjizenje @Action=1,@Datum=@@Datum,@operaterid=0!!`.

`PROKNJIZIFORM` `ONSUBMIT="spkntMatknjizenjequeryNG"` počinje sa `sp`, pa se niz `@ime=` doda (`Form2.vb` 3378–3388`). `FILLEXPLORER="TRUE"`.

## Praznine

- Pejzaž ne vrijedi za `PRINTMASTER` (R41).
- `!!` na filteru gasi lijepljenje (R14).
