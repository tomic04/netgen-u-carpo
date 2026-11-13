# Bilanca-GFI — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/Bilanca-GFI.xml`. Rang 15, 10 baza, Biovega. XML se ne mijenja. `AOPuExcel` se ne pokreće. Test ne šalje SQL.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` počinje s `:FILTERFORM#`. To nije jedan SQL. `NovaGenerickaForma` reže na `#` (`Form1.vb` 4558–4561). Prvi segment `:FILTERFORM` otvori filter (`4862`). Sljedeći segmenti su `select case ... enable/disable` i pale ili gase kontrole po `@@izvjestaj` (R71).

`COMMANDTIMEOUT="120"` (`3463`, R65). `CHECKBOXVISIBLE="FALSE"`. `PAGEORIENTATION="LANDSCAPE"` vrijedi za Janus `IspisNovi` (`5930`). `CMD4` je `PRINTMASTER`, a taj put zove `IspisGRIDREPX` i orijentaciju ne gleda (`4951–4955`, R41).

Zaglavlje ispisa: desno `u periodu: @@datumod do @@datumdo`, podnožje lijevo `@@tkosmomi##@@mjesto` (`##` je novi red u pripremi zaglavlja).

Gumbi:

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` plus isti `#select case` lanac kao `ONLOAD` |
| 2 | GFI | `GFIPODFORM#AOPuExcel.clsGFIPOD(GFI-POD601.xls)` |
| 3 | TSI | `TSIPODFORM#AOPuExcel.clsGFIPOD(TI-POD_4.0.0.xls)` |
| 4 | Ispis | `PRINTMASTER` |
| 7 | AOPuExcel | `ASSEMBLYLOAD(AOPuExcel64.Class1)` |

Drugi segment GFI/TSI sadrži `.clsGFIPOD`, ne `.clsreport`. Uvjet ispisa traži `.clsreport` (`5215`). Ime lijevo od `(` je `AOPuExcel.clsGFIPOD`. Forma ne postoji, dijalog se ne otvori (`5369–5370`, R73). Pravi DLL je samo `CMD7`.

`CMD5`, `CMD6`, `CMD8`–`CMD10` nisu napisani, skriveni.

## Forme

`FILTERFORM`, `GFIPODFORM`, `TSIPODFORM`. `MASTERCOLUMNS` drži stupce ispisa. Širina je `WIDTH/14` (`7522`).

## Praznine

- `ONLOAD` je lanac, ne jedan upit (R71).
- `.clsGFIPOD(...)` nije ispis ni `ASSEMBLYLOAD` (R73).
- `PRINTMASTER` ne čita `LANDSCAPE` (R41).
- Timeout je 120 s (R65).
