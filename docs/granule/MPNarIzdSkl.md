# MPNarIzdSkl — panel, group, datetime, checkbox, WUDTH

XML: `definitions/netgen/BIOVEGA/MPNarIzdSkl.xml`.

## Datum koji slučajno nastane

`KOPIRAJ.datum` je `TYPE="datetiem"`. Sadrži `date` (`2232`), pa nastane `DTPicker`. `KOPIRAJ.datumdok` je `TYPE="datetime"`, ista grana. Nema `DEFAULT`, nema `firstday*`, nema `today`. Port koji odbaci `datetiem` jer nije točno `date` promijeni formu.

## Panel i group

`DONOSAUTOMATSKI` ima dva `TYPE="panel"` bez `NAME`, `DOCK="1"` i `DOCK="2"` (`476`–`477`). Grana `1384` složi `Panel`. `Dock` se dodijeli iz atributa (`1407`): `1` je `DockStyle.Top`, `2` je `Bottom`.

`bottomgroupbox` je `TYPE="group"` (`511`). Ista grana kao `frame` (`1354`), `GroupBox`. `DOCK="2"`.

`frame` na drugim formama ove granule (npr. zaglavlja) je isti `GroupBox`.

## Check, multidrop, širina

`podklase` i `pakovanje` su `checkbox` → `check` (`1460`). `pakovanje` ima `WUDTH="1500"`, ne `WIDTH`. `SrediKontrolu` čita `WIDTH` (`2949`). `WUDTH` ostane u atributima i ne mijenja širinu.

`dodklasa` i `vrsta` su `multidrop`. `ALLOWEMPTY="FALSE"` se pročita (`2500`) i ne ubaci red. Vrijednost je `id1~id2` (`3446`). `vrsta` SQL vraća string id-eve `'1'`, `'2'`, `'3'`.

`danaisporuke` je `number` s `DECIMALS="0"`.

## Rupe

- `datetiem` je datum (`2232`). Ne crtati ništa, i ne preimenovati u XML-u.
- Panel bez `NAME` i dalje nastane (`1384`).
- `WUDTH` se ne pretvara u `WIDTH` (`2949`).
- `ALLOWEMPTY` na multidropu ne radi ono što radi na combu (`2402` naspram `2500`).
