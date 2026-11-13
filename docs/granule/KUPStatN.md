# KUPStatN — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/KUPStatN.xml`. Rang 97, 2 baze. Natpis je `Status`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` počinje s `select kupstatusid`. To nije forma. Master se puni tim upitom (`3879–3882`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unesi | `INSERTFORM` |
| 2 | Izmjeni | `UPDATEFORM` |
| 3 | Obriši | `DELETEFORM` |

`MASTERDBLCLICK="UPDATEFORM"`. `POPUPMASTER` ima četiri stavke i svaka ima `:`, pa su stavke izbornika.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Dijalozi

`INSERTFORM` šalje `spKUPStatusAzurN @Action=0,`. `UPDATEFORM` šalje `@action=1,@ID=@@ID,`. `DELETEFORM` šalje `@action=2,@ID=@@ID`. Nema `FILLEXPLORER`, pa nakon OK-a procedura ide u `ExecSql`, a zatim se ponovo pošalje zadnji master (`5509–5519`).

Prvih šest znakova `spKUPS` nisu u `INSERTDELETEUPDATESELECT`, pa se parametri kontrola lijepe na kraj (`3378–3388`, R14). `@Action=0` i `@action=1` već imaju zarez na kraju.

`Proknjizi` je combo. SQL počinje s `select 0`. `ALLOWEMPTY` nema, pa prazan red ostaje isključen (`2402`).

## Praznine

- `@action=2` briše status. `spKUPStatusAzurN` nije riječ `delete`, pa je zaštita prvog slova ne vidi. Na Jupiteru se ta procedura ne šalje.
