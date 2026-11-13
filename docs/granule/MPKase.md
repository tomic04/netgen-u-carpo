# MPKase — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPKase.xml` (UTF-16). Rang 37, 7 baza, Biovega. Natpis je `MPKase`. XML se ne mijenja. `spMPKase` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spMPKase @action=1"` puni master (`3879–3882`). `DETAILGRID="spMPKase @action=2, @trgovinaid=@@id"`. `DETAILDBLCLICK="UPDATE_FORM"` otvara formu na dvostrukom kliku detalja, ne mastera (R75). `MASTERDBLCLICK` nema.

| # | Natpis | Naredba |
|---|---|---|
| 1 | &Dodaj | `INSERTFORM` |
| 2 | &Izmjeni | `UPDATE_FORM` |
| 3 | &Kopiraj | `KOPIRAJFORM` |
| 4 | &Sve kase | `DAJSVE` |
| 5 | &Zaglavlje računa | `ZAGLAVLJE` |
| 7 | Lista | nema `:`, gumb se ne složi (`3320`, R60) |

`POPUPDETAIL="Dodaj:INSERTFORM#Izmjeni:UPDATEFORM"` puni izbornik detalja `ContextMenuStrip2` (`3386`, `3702–3750`, `7225`, R83). `Izmjeni` traži `UPDATEFORM`. Forma je `UPDATE_FORM` (R19). `POPUPMASTER` nema.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. `DETCHECKBOXVISIBLE="FALSE"` i `DETTOTALROWVISIBLE="FALSE"` su detail (R79). `GROUPBYBOXVISIBLE="FALSE"`. `AUTOFITCOLUMNS="False"` se ne čita (R80). `SETDEFAULTS="select dbo.fnTkoSmoMi() as tkosmomi"`. `PAGEHEADERRIGHT="@@tkosmomi"`. `REPORT` se ne čita. Nema `TREE`.

## Forme

`UPDATE_FORM` `ONLOAD` je `exec spMPKase @action=4 ,@MPkaseID=@@detid`. To je id detalja, ne mastera. `INSERTFORM` i `KOPIRAJFORM` vežu `@@id` trgovine.

## Praznine

- Izbornik detalja nije izbornik mastera (R83).
- `Lista` ostaje skrivena (R60).
- `UPDATEFORM` u popup-u nije `UPDATE_FORM` (R19).
