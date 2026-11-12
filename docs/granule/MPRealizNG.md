# MPRealizNG — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/MPRealizNG.xml`. Rang 49, 6 baza. Natpis je `Realizacija robe u MP`. XML se ne mijenja. `spMPRealizacijaRobeNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FORM1"` otvara dijalog, ne puni master (`Form1.vb` 3879–3880, R18). `CMD1CAPTION="Parametri:FORM1"`. `CMD4CAPTION="Kartica:KARTICA"`. `MASTERDBLCLICK="PROMET"`.

`SETDEFAULTS` zove `spMPPermissionNG` s `@@operateriid` i `@@xmlname`. `COMMANDTIMEOUT="600"` (R65). `PAGEORIENTATION="LANDSCAPE"` mijenja samo `IspisNovi` (`5930`, R41). Na podlozi nema `PRINTMASTER`.

## FORM1

`FILLEXPLORER="TRUE"`. `ONLOAD` je `select @@datumod as datumod , @@datumdo as datumdo , @@trgovinaid as trgovinaid`. Te `@@` nisu u `SrediSQL` (`2470–2502`). `ExecSql` ih preskoči (`Class1.vb` 113–118). Master se napuni tek iz `ONSUBMIT` nakon OK-a (`5445–5452`).

`PROMET` i `KARTICA` imaju `FILLEXPLORER="TRUE"`.

## Praznine

- Prazan master na ulazu nije rezultat upita. Čeka se OK na `FORM1`.
