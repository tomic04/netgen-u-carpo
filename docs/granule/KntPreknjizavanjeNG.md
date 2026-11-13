# KntPreknjizavanjeNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KntPreknjizavanjeNG.xml`. Rang 55, 5 baza, Biovega. Natpis je `Preknjižavanje s konta na konto`. XML se ne mijenja. `spKntPreknjizavanjeNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTER"`. Element forme je `<FILTER>` (`Form2.vb` 957). Parser na ovoj grani ime `FILTER` drži strukturnim, pa forma nije u modelu. To je implementacijski R84. Dok taj kod nije na testiranoj grani, stavka je otvorena.

`BUTTONS="Pokaži:FILTER#Proknjiži:MULTISELECT_Proknjiži"`. `CMD5CAPTION="Ispis:PRINTMASTER#REFRESHMASTER"`. `BUTTONS` prepiše prva dva gumba, `CMD5` ostane (R56). `PRINTMASTER` zove `IspisGRIDREPX` (`4951–4955`, R41). `REFRESHMASTER` nije `REFRESHMASTERGRID` (R04).

`CHECKBOXVISIBLE="TRUE"` ostavlja stupac 1 vidljiv (`2043–2046`). `TOTALROWVISIBLE="FALSE"`. `DETCHECKBOXVISIBLE="FALSE"`.

## FILTER i proknjiženje

`FILTER` `FILLEXPLORER="TRUE"`, `ONSUBMIT="spKntPreknjizavanjeNG @action=1,"`. To je čitanje izbornika. `MULTISELECT_Proknjiži` `ONSUBMIT` je `@action=2` i mijenja knjiženje. Na jupiteru se taj `sp` ne smije izvršiti. Ime `MULTISELECT_` briše temp tablicu prije forme (R44).

## Praznine

- Element `FILTER` mora biti forma.
- `@action=2` je upis i ne ide na klijentski server.
