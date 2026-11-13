# KntOPZ-STAT-1 — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KntOPZ-STAT-1.xml`. Isti SHA-256 kao `Agronom granule/KntOPZ-STAT-1.xml`. U izborniku je na 10 baza, uključujući Biovegu. XML se ne mijenja. `spKntOPZSTAT` se u testu ne šalje.

Izgled je Carpo (R59). `HEADERALIGNMENT="2"` je **izgled – Carpo**. Širina, skriveni stupac, `POSITION`, natpis i naredba ostaju.

Log ide u `NgDnevnik`.

## Podloga

Naslov je `Obrazac OPZ-STAT-1 ` s razmakom na kraju (`3522–3528`).

`ONLOAD=":UPIT"` otvori parametre prije mastera (`3879–3880`, R18). Nema `TREE` ni `DETAILGRID`. Stablo se sakrije (`4310–4316`).

`FILLEXPLORER="TRUE"` na projektu se ne čita (R70). Master puni forma `UPIT`, koja ima svoj `FILLEXPLORER="TRUE"` (`5445–5452`).

`COMMANDTIMEOUT="240"` (`3463`, `Class1.vb` 99, R65).

`CHECKBOXVISIBLE="FALSE"`, `TOTALROWVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`. Sort ostaje.

Gumbi (`3314–3329`):

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `UPIT` |
| 2 | Ispravak | `ISPRAVI` |
| 3 | Ispis | `repx/rptOpzStat1(@DatumDo=@@DatumDoParam, @DatDosp=@@DatDospParam)` |
| 4 | Eporezna | `OpzStatExport.Start(@@datumDoParam ; @@datdospparam)` |

`MASTERDBLCLICK="ISPRAVI"`. Popup je `Poništi stavku: PONISTI_STAVKU` (razmak iza `:`).

Ispis je `repx/` s parametrima (`5215–5239`). Eporezna nema `ASSEMBLYLOAD(`. Ime forme je `OpzStatExport.Start`, forma ne postoji, dijalog se ne otvori (`4407–4418`, `5369–5370`, R67). Točka-zarez nije poziv DLL-a.

## Stupci

`MASTERCOLUMNS CLEARCOLUMS="TRUE"`. Svi se sakriju, pa se navedeni vrate (`7459–7462`). `AGREGATEFUNCTION="0"` ima jedno G i nije prazan, agregat se dodijeli (`7496`, `7506`, R08).

`FormatId`: `VISIBLE="FALSE"`, `POSITION="-1"`. `POSITION` prepiše red (`7526`, R69). Širina 400/14. `DatumDok` `FORMAT="dd.MM.yyyy"` (`7524`). `HEADERALIGNMENT="2"` je **izgled – Carpo**.

## Forme

`UPIT` `ONSUBMIT` počinje sa `spKntOPZSTAT`. `multidrop` `nizKonto` šalje `id1~id2` (R28). `ISPRAVI` ima `FILLEXPLORER="treeandrefreshmaster"`: osvježi stablo, izvrši storu, pa zadnji master (`5478–5490`). `PONISTI_STAVKU` gumbi su `INPUT` `ONCLICK="OK"` i `CLOSE`, ne `CMD`.

## Praznine

- `FILLEXPLORER` na projektu ne puni master (R70).
- Eporezna ne učitava DLL (R67).
- `POSITION="-1"` i `CLEARCOLUMS` rade oba, `POSITION` pobijedi (R69).
- `"0"` na agregatu nije „nema sume“ (R08).
- Timeout je 240 s (R65).
