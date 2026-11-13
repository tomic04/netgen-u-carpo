# KOD_RAD1 — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KOD_RAD1.xml`. Šest baza, Biovega. Natpis je `RAD1`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Nema stabla, `BUTTONS` ni dvostrukog klika. Master se na ulazu ne puni (`3879`).

`COMMANDTIMEOUT="600"` (R65). `TOTALROWVISIBLE="false"` gasi sumu (`2049–2050`). `DETCHECKBOXVISIBLE="false"`. `CHECKBOXVISIBLE="FALSE"`.

## Dijalozi

Izmjene zovu `spKOD_RAD1_Azur`. Akcije su tekst, ne broj: `update_stavke_rad1`, `update_stavke_rad1g`, `update_spreme`, `insert_kodjedinicesnimanja`, `update_kodjedinicesnimanja`, `delete_kodjedinicesnimanja`.

Te akcije mijenjaju RAD-1. Na Jupiteru se ne šalju.
