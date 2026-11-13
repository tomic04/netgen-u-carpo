# SklMinMaxDat — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/SklMinMaxDat.xml`. Rang 40, 7 baza. XML se ne mijenja. `spSktMinMaxDatum` se ne šalje.

Izgled je Carpo (R74).

## Podloga

Element `CAPTION` je `Klase tečajeva`. To je naslov podloge (`Form1.vb` 3522–3528).

`ONLOAD=":MAINFORM"` otvara dijalog (`3879–3880`, R18). Gumb `Parametri` → `MAINFORM`. `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="FALSE"`. Nema `TREE`.

## MAINFORM

Natpis dijaloga je `Skladište-min i max datum`. `ONLOAD="spSktMinMaxDatum @action='popuni_datume'"`. `MAXIMIZEBOX="FALSE"` gasi povećanje (`Form2.vb` 1031, R84).

Datumi `datod` i `datdo`. `U redu` šalje `spSktMinMaxDatum @action='promijeni_datume',@DatOd=@@datod,@DatDo=@@datdo` pa `#OK`. Datum je `'yyyyMMdd'` (`Form2.vb` 3531–3532, R24). `Odustani` je `close`.

## Praznine

- Naslov prozora i naslov dijaloga nisu isti tekst.
- Dijalog se ne smije povećati (R84).
