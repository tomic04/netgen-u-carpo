# Obrazac — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/Obrazac.xml`. Šest baza, Biovega. Natpis je `Obrazac`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master čeka čvor (`3879`).

`TREE="Osvježi drvo:REFRESHTREEFORM#spObrazacTree @action='obrazac':STATUS"`. `spObrazacTree` sadrži `tree` i ide u store (`4350–4351`, R01). `REFRESHTREEFORM` ima `FILLEXPLORER="onlytree"` i ne šalje master (R54). `STATUS` ima `FILLEXPLORER="TRUE"`.

`BUTTONS` počinje s `Obrazac - Unos:OBRAZAC(@obrazacid=0)`. Ime forme je `OBRAZAC`, a `@obrazacid=0` je parametar (`4407–4418`, R55). Ispis je `ISPIS_@@ispisoznakaid`.

`COMMANDTIMEOUT="600"` (R65). `TOTALROWVISIBLE="false"` gasi sumu (`2049–2050`). `POPUPMASTER="Brisanje obrasca:OBRAZACDEL"`.

## Praznine

- Brisanje obrasca je dijalog `OBRAZACDEL`. Na Jupiteru se njegov upis ne šalje.
