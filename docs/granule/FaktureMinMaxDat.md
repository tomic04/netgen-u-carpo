# FaktureMinMaxDat — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/FaktureMinMaxDat.xml`. Rang 38, 7 baza. XML se ne mijenja. `spFaktureMinMaxDatum` se ne šalje.

Izgled je Carpo (R74).

## Podloga

Element `CAPTION` je `Klase tečajeva`. To je naslov podloge (`Form1.vb` 3522–3528). Nema `VERSION`.

`ONLOAD=":MAINFORM"` otvara dijalog (`3879–3880`, R18). Jedan gumb: `Parametri` → `MAINFORM`. `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="FALSE"`. Nema `TREE`.

## MAINFORM

Natpis dijaloga je `Fakture-min i max datum`. `ONLOAD="spFaktureMinMaxDatum @action='popuni_datume'"`. `MAXIMIZEBOX="FALSE"` gasi povećanje (`Form2.vb` 1031, R84). `WINDOWSTATE` je prazan.

Datumi `datod` i `datdo`. Gumb `U redu` ima `ONCLICK` koji zove `spFaktureMinMaxDatum @action='promijeni_datume',@DatOd=@@datod,@DatDo=@@datdo` pa `#OK`. Datum u parametru je `'yyyyMMdd'` (`Form2.vb` 3531–3532, R24). `Odustani` je `close`.

## Praznine

- Naslov prozora i naslov dijaloga nisu isti tekst.
- Dijalog se ne smije povećati (R84).
