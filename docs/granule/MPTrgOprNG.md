# MPTrgOprNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPTrgOprNG.xml`. Rang 36, 7 baza. Natpis je `Ažuriranje operatera po trgovinama`. XML se ne mijenja. `spMPTrgOpr` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` otvara dijalog (`Form1.vb` 3879–3880, R18). `DETAILGRID="spMPTrgOpr @action = 7, @operateriid=@@operateriid, @id=@@id"`. `@@id` je vrijednost mastera (R11).

`BUTTONS` i `CMD1`–`CMD5` nose iste naredbe. `BUTTONS` prepiše prvih pet gumba (R56):

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Unos | `UNOS` |
| 3 | Izmjena | `IZMJENA` |
| 4 | Brisanje | `BRISANJE` |
| 5 | Pregled svih | `SVI` |

`MASTERDBLCLICK="IZMJENA"`. `POPUPMASTER="Izmjena:IZMJENA"`. `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="TRUE"`. `AUTOFITCOLUMNS="False"` se ne čita (R80).

## Forme

`FILTERFORM` ima `FILLEXPLORER="TRUE"`: OK zamijeni master `ONSUBMIT`-om (`5445–5452`). `IZMJENA`, `UNOS`, `BRISANJE` zovu `spMPTrgOpr`. `SVI` nema `ONSUBMIT`.

## Praznine

- Ulaz je dijalog, ne master SQL (R18).
- `AUTOFITCOLUMNS` se ne čita (R80).
