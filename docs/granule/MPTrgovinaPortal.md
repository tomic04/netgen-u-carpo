# MPTrgovinaPortal — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPTrgovinaPortal.xml`. Šest baza, Biovega. Natpis je `Trgovina PORTAL`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). Nema stabla ni `BUTTONS`.

`MASTERDBLCLICK=":TRGOVINAFORM"`. Vodeća `:` otvara formu (`4862`). `COMMANDTIMEOUT="300"` (R65). `CHECKBOXVISIBLE="FALSE"`.

Datoteka ima 35 formi. Unos kase `KASAINSERT_FORM` šalje `spMPKase @Action=3`. To mijenja kasu. Na Jupiteru se ne šalje.
