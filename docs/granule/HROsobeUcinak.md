# HROsobeUcinak — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/HROsobeUcinak.xml`. Rang 56, 5 baza, Biovega. Natpis je `Prikaz djelatnika`. XML se ne mijenja. `spHROsobeUcinakQuery` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spHROsobeUcinakQuery @Action=0"` puni master (`Form1.vb` 3879–3882). Nema `TREE`. Stablo se sakrije (`4310–4316`).

`CMD1CAPTION="Izmjena:UPDATE_FORM"`. `MASTERDBLCLICK=":UPDATE_FORM"`. Vodeća `:` otvara istu formu (`4862–4867`). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

Forme su `AUTOR` i `UPDATE_FORM`. `AUTOR` nitko s podloge ne zove.

## Praznine

- Dvostruki klik i Izmjena otvaraju `UPDATE_FORM`.
