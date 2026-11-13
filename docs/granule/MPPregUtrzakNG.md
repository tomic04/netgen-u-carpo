# MPPregUtrzakNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPPregUtrzakNG.xml`. Rang 74, 3 baze. Natpis je `Pregled utržaka MP`. XML se ne mijenja. `spMPQueryGlaIDetNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FORM1"` otvara dijalog (`3879–3880`, R18). `DETAILGRID` veže `@id=@@id` (R11). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

`TREE` počinje s `Danas:DANAS_FORM#Jučer:JUCER_FORM`, pa `spMPTreeNG` (store, `4350–4351`). Pet vremenskih formi ima `FILLEXPLORER="TRUE"` i `@node` u `ONSUBMIT`.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Načini plaćanja | `NACPLAC2` |
| 4 | Promjena vrste trgovine/pregleda | `FORM1` |
| 6 | Print dok. | `vrstadokid-MaloprodajaGla` (R43) |

`POPUPMASTER="Parametri:FILTERFORM"`. Nema `CMD3` ni `CMD5`.

## Praznine

- Ulaz je `FORM1`, master čeka tu formu ili čvor.
- Ispis je `vrstadokid-` (R43).
