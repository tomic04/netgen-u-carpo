# MPFiskalPubVars — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPFiskalPubVars.xml`. Rang 14 u `Prioritet.md`, 10 baza, Biovega. XML se ne mijenja. Test ne šalje SQL.

Izgled je Carpo (R74). Natpis i naredba ostaju NetGen.

## Podloga

`ONLOAD=":UPDATEFORM"` otvori tu formu prije mastera (`Form1.vb` 3879–3880). `CMD1`–`CMD6` su prazni. `BUTTONS="Izmjeni:UPDATEFORM"` prepiše prvi gumb (`3770–3788`). Natpis `Izmjeni`, naredba `UPDATEFORM`.

`MASTERDBLCLICK` i `POPUPMASTER` su prazni. Dvostruki klik ne otvara formu. `CHECKBOXVISIBLE="FALSE"` sakrije checkbox stupac (`2043–2046`). `TOTALROW="FALSE"` se ne čita, `TOTALROWVISIBLE` nema, suma ostaje (`2049–2053`, R66). `SETDEFAULTS` je prazan.

Nema `TREE` ni `DETAILGRID`. Stablo se sakrije (`4310–4316`).

## `UPDATEFORM`

Jedina forma. `ONLOAD` puni kontrole prije prikaza (`Form2.vb` 1042, 1195). Greška na tom SQL-u ne otvara formu (`1199`). Gumbi `CMD1`/`CMD2` su cijeli natpis (`1061–1066`).

## Praznine

- Ulaz je forma, ne master SQL.
- `TOTALROW` ne gasi sumu (R66).
- Jedini gumb dolazi iz `BUTTONS`, ne iz praznih `CMD`.
