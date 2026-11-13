# kntShemaKonta — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/kntShemaKonta.xml`. Rang 66, 4 baze. Natpis je `Shema konta`. XML se ne mijenja. `spKntShema` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="exec spKntShema 2"` puni master (`3879–3882`). `COMMANDTIMEOUT="120"` (R65).

`TREE="spKntShemaTree:TREEROBAFORM"`. Ime storea sadrži `tree`, pa ide u `DazovaIzStore` (`4350–4351`, R01). Klik otvara `TREEROBAFORM`. Ta forma ima `FILLEXPLORER="TRUE"`. `ONSUBMIT` počinje s `if @@node=0`.

`CMD4CAPTION="Ispis:PRINTMASTER"`. Nema `CMD1`–`CMD3`. `PRINTMASTER` zove `IspisGRIDREPX`, ne `IspisNovi` (`4951–4955`). `PAGEORIENTATION="LANDSCAPE"` tu orijentaciju ne mijenja (R41). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Praznine

- Pejzaž ne vrijedi za `PRINTMASTER` (R41).
