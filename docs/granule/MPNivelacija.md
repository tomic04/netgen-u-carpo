# MPNivelacija — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPNivelacija.xml`. Rang 54, 5 baza, Biovega. Natpis je `MP Nivelacija`. XML se ne mijenja. `spMPQueryGlaIDetNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). `DETAILGRID` veže `@@id` (R11).

`TREE` počinje s `Danas:DANAS_FORM#Jučer:JUCER_FORM`, pa `spMPTreeNG` (store, `4350–4351`, R01). Forme `DANAS_FORM`, `JUCER_FORM`, `TEKUCIMJESEC_FORM`, `PROSLIMJESEC_FORM` i `FILTERODDO_FORM` imaju `FILLEXPLORER="TRUE"`.

`BUTTONS="Pronađi:FILTERFORM#Print dok.:vrstadokid-MaloprodajaGla"`. `vrstadokid-` nije ime forme (R43). `POPUPMASTER="Pronađi:FILTERFORM"`.

`ONLOAD2` vraća `enable`/`disable` u stupcima `Dodaj`, `Izmjeni`, `Zaključi`. Stupac se traži po natpisu gumba (`3914–3921`, R57). Ovi natpisi nisu na `BUTTONS`.

`SETDEFAULTS` je `spMPPermissionNG`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. `DETCHECKBOXVISIBLE="FALSE"`.

## Praznine

- Prazan master na ulazu čeka klik na stablo.
- Ispis je `vrstadokid-` (R43).
