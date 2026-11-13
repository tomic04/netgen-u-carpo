# kntDobDospRac — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/kntDobDospRac.xml`. Rang 67, 4 baze, Biovega, dva retka izbornika. Natpis je `Dobavljači - prijedlog za plaćanje`. XML se ne mijenja. `spkntDobDospRacNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` otvara dijalog (`Form1.vb` 3879–3880, R18). `COMMANDTIMEOUT="120"` (R65). `MASTERDBLCLICK=":UPDATEFORM"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="TRUE"` ostavlja sumu (`2049–2052`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Prikaz | `FILTERFORM` |
| 2 | Virman | `VIRMANFORM` |
| 3 | Ispis | `PRINTMASTER` |
| 4 | Označi | `MULTIBLUEEDIT` |

`PRINTMASTER` zove `IspisGRIDREPX` (`4951–4955`, R41). `POPUPMASTER="Označi:MULTIBLUEEDIT"`. `MULTIBLUE` puni plave retke, ne kvačice (R21).

## FILTERFORM

`FILLEXPLORER="TRUE"`. `ONLOAD` i dalje ima `@@datod` i `@@datdo`. `ExecSql` to preskoči (`Class1.vb` 113–118). Master dođe iz `ONSUBMIT` nakon OK-a.

`VIRMANFORM` je `@action=4`, `UPDATEFORM` `@action=2`, `MULTIBLUEEDIT` `@action=6`. To mijenja podatke. Na jupiteru se ti `sp` ne izvršavaju.

## Praznine

- Prazan master na ulazu čeka OK na filteru.
- Klik gumba na dijalogu port još ne izvrši (`Dijalozi-pregled.md`).
