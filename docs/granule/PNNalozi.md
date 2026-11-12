# PNNalozi — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PNNalozi.xml`. Rang 65, 4 baze, Biovega. Natpis je `Putni nalozi`. XML se ne mijenja. `spPNNalozi_Query` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spPNNalozi_Query @action='master_radnik', @treenode='otvoreni', @operateriid = @@operateriid"`. `@@operateriid` se zamijeni (`Form1.vb` 2482). Literal `otvoreni` ostane. SQL se šalje (`3879–3882`).

`TREE` ima `Osvježi drvo:REFRESHTREEFORM`. Ta forma ima `FILLEXPLORER="onlytree"` i ne šalje master (R54). Zatim `Otvoreni nalozi:OTVORENI` i store `spPNNalozi_Tree` (R01). `OTVORENI` i `STATUS` imaju `FILLEXPLORER="TRUE"`.

`MASTERDBLCLICK=":UPDATENALOG(@pnnaloziid=@@pnnaloziid)"`. Ime forme je `UPDATENALOG`, zagrada je parametar (R55). `BUTTONS` počinje isto tako.

`SETDEFAULTS="spPNNalozi_DefaultsNG @action=0"`. `PERMISIO="TRUE"` (R06). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Praznine

- Osvježi drvo ne smije ponovo poslati master (R54).
