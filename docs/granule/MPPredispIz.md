# MPPredispIz — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPPredispIz.xml`. Šest baza, Biovega. Natpis je `MP Predispozicija izlaz`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master čeka čvor (`3879`).

`TREE` počinje s `Danas:DANAS_FORM`, zatim čvor `JUCER_FORM`, pa `spMPTreeNG`. Store je ondje gdje SQL sadrži `tree` (`4350–4351`). Čvorovi imaju `FILLEXPLORER="TRUE"` i `spMPQueryGlaIDetNG @action=1`.

Datoteka javlja `windows-1250`, a natpisi jučer i prošli mjesec u stablu spremljeni su kao UTF-8 bajtovi. Naredba `JUCER_FORM` ostaje ista.

`DETAILGRID` veže `@id=@@id` (R11). `MASTERDBLCLICK="UPDATE_FORM"`. `BUTTONS` završava s `vrstadokid-MaloprodajaGla` (R43). `SETDEFAULTS` zove `spMPPermissionNG`.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Praznine

- Zaključak mijenja dokument. Na Jupiteru se ne šalje.
