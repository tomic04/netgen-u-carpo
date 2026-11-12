# KntPreknjizavanjePartnera — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/KntPreknjizavanjePartnera.xml`. Rang 69, 4 baze. Natpis je `Knjiženje s partnera na partnera`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` (`3879–3880`, R18). `COMMANDTIMEOUT="300"` (R65). `CHECKBOXVISIBLE="TRUE"` ostavlja stupac 1 (`2043–2046`). `PAGEORIENTATION="LANDSCAPE"` ne dira `PRINTMASTER` (R41).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Proknjiži | `MULTISELECTPROKNJIZI` |
| 3 | Ispis | `PRINTMASTER` |

## Forme

`FILTERFORM` ima `FILLEXPLORER="TRUE"`. `ONSUBMIT` počinje s `exec`, ne sa `sp`, pa se niz `@ime=` ne lijepi na tekst (R14). Parametri su već u tekstu (`@DatumKnjizenja=@@DatumKnjizenja`).

`MULTISELECTPROKNJIZI` je `@Action=2` i mijenja knjiženje. Na jupiteru se taj `exec` ne šalje. Ime nema prefiks `MULTISELECT_`, pa temp tablica iz R44 ovdje ne vrijedi.

## Praznine

- Proknjiži je upis.
- Combo na filteru port ne puni pri otvaranju (`Dijalozi-pregled.md`).
