# PNObracun — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PNObracun.xml`. Rang 43, 6 baza, Biovega. Natpis je `Putni nalozi`. XML se ne mijenja. `spPNNalozi_Query` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spPNNalozi_Query @action='master', @treenode='otvoreni'"`. Ne počinje s `:`, pa puni master (`Form1.vb` 3879–3882). Filter `otvoreni` je u tekstu SQL-a.

`TREE` ima `Osvježi drvo:REFRESHTREEFORM`, statičke čvorove i `spPNNalozi_Tree` (sadrži `tree`, store, `4350–4351`, R01). `SETDEFAULTS="spPNNalozi_DefaultsNG @action=0"`. `PERMISIO="TRUE"` (`3414`, R06). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

`MASTERDBLCLICK=":UPDATENALOG(@pnnaloziid=@@pnnaloziid)"`. Dvotočka se skine (`4862–4867`). Ime forme je lijevo od `(` (`Form2.vb` 4407–4418, R55). Parametar je `@@pnnaloziid`.

`BUTTONS` počinje s `Unos:INSERTNALOG(@hrkadroviid = 0)`. Isto pravilo imena forme (R55).

## Praznine

- Dvostruki klik nije ime forme `UPDATENALOG(@…)`. Ime je `UPDATENALOG` (R55).
