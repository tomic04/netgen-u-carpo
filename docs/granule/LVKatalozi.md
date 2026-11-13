# LVKatalozi — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/LVKatalozi.xml`. Rang 95, 2 baze, Biovega. Natpis je `Loko vožnja - katalozi`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). Nema gumba na projektu.

`TREE="spLVKatalozi_Tree @action='tree':STATUS"`. Store (`4350–4351`, R01). Klik otvara `STATUS`.

`REFRESHTREEFORM` ima `FILLEXPLORER="onlytree"` i ne šalje master (R54). `STATUS` ima `FILLEXPLORER="TRUE"` i `ONLOAD="select @@treenode as '@@node'"`.

`COMMANDTIMEOUT="600"` (R65). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` gasi sumu jer se čita velikim slovima (`2049–2050`).

## Praznine

- Prazan master na ulazu nije rezultat upita.
- Osvježi drvo ne smije ponovo poslati master (R54).
