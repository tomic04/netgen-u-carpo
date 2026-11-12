# MPZakljPerioda — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPZakljPerioda.xml`. Šest baza, Biovega. Natpis je `Zaključak perioda maloprodaje`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je `select` koji vraća stupac `prazno`. Master dobije taj jedan red (`3879–3882`), ne ostaje prazan zato što upita nema.

`MASTERDBLCLICK="MPKARTICA"`. `BUTTONS` i `POPUPMASTER` su prazni. `COMMANDTIMEOUT="600"` (R65). `CHECKBOXVISIBLE="FALSE"`.

## Dijalozi

`IZMJENIFORM` šalje `spMPKontrolaNivelacijaNG @action=8`. `IZMJENIKOL` šalje `@action=15`, `IZMJENIKOLA` `@action=13`. To mijenja nivelaciju. Na Jupiteru se ne šalje.
