# MPNarPredis — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPNarPredis.xml`. Sedam baza, Biovega. Natpis je `MP narudžba iz druge trgovine`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master na ulazu ne dolazi iz upita (`3879`).

`TREE` počinje s `Danas:DANAS_FORM#Jučer:JUCER_FORM#spMPTreeNG @action=1:TEKUCIMJESEC_FORM`. Segment s `:` i praznom naredbom nije roditelj sljedećih (R10). `spMPTreeNG` sadrži `tree` i ide u store (`4350–4351`). Čvorovi imaju `FILLEXPLORER="TRUE"`.

`DETAILGRID` veže `@id=@@id` (R11). `MASTERDBLCLICK="UPDATEFORM"`.

`BUTTONS` završava s `Print dok.:vrstadokid-MaloprodajaGla` (R43). `POPUPMASTER` ima Pronađi, Izmjeni, Zaključi i Otključaj.

`SETDEFAULTS` zove `spMPPermissionNG`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Praznine

- Prazan master na ulazu čeka čvor.
- Zaključak i otključaj mijenjaju dokument. Te forme se na Jupiteru ne šalju.
