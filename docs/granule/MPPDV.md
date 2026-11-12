# MPPDV — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPPDV.xml`. Rang 16, 10 baza, Biovega. XML se ne mijenja. Test ne šalje SQL.

Izgled je Carpo (R74). `&` u natpisu ostaje. Redoslijed i naredba ostaju.

## Podloga

`VERSION="1.0"`. Naslov dobije `  v[1.0]` (`Form1.vb` 3524–3525). `PERMISIO="TRUE"` šalje select nad `ssPermission` (`3414–3416`).

`ONLOAD=":Filter"` otvori filter (`3879–3880`). Nema `TREE` ni `DETAILGRID`. Stablo se sakrije (`4310–4316`).

`CHECKBOXVISIBLE="FALSE"`, `TOTALROWVISIBLE="FALSE"`, isto za detail. `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`.

Gumbi: `1 &Izmjeni` → `MP_PDV`, `3 &Filter` → `Filter`, `4` Pronađi → `Find`, `6` Izvrši promjene odmah → `IzvrsiJob`. `CMD2` i `CMD5` nisu napisani.

`MASTERDBLCLICK=":MP_PDV"`. Dvostruki klik zove `NovaGenerickaForma` (`6935`). Vodeća `:` i dalje otvori formu (`4862`).

## Forme

`Filter`, `MP_PDV`, `Find`, `Unesi`, `Izmjena`, `Brisanje`, `IzvrsiJob`. `ONSUBMIT` koji počinje sa `sp` lijepi parametre (`Form2.vb` 3378–3388). Greška `ONLOAD` ne otvara formu (`1199`).

## Praznine

- Dvostruki klik ima vodeću `:`. Mora otvoriti `MP_PDV`, ne tražiti formu čije ime počinje dvotočkom.
- `Find` i `IzvrsiJob` su forme iz `CMD`, ne ugrađene naredbe.
- `PERMISIO` je uključen iako Carpo permissions nisu isti ugovor (R06).
