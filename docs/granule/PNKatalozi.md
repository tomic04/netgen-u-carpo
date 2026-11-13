# PNKatalozi — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PNKatalozi.xml`. Rang 47, 6 baza, Biovega. Natpis je `Putni nalozi - katalozi`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`).

`TREE="spPNKatalozi_Tree @action='tree':STATUS"`. SQL sadrži `tree`, pa čvorovi dolaze iz storea (`4350–4351`, R01). Klik otvara `STATUS`.

`COMMANDTIMEOUT="600"` (R65). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` gasi sumu (`ToUpper`, `2049–2050`). Nema `BUTTONS` ni `CMD`.

## Praznine

- Prazan master na ulazu je zato što nema `ONLOAD`, ne zato što store nema redova.
