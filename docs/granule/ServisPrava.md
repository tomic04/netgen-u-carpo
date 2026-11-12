# ServisPrava — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/ServisPrava.xml`. Rang 78, 3 baze. Natpis projekta je `Novi projekt`. XML se ne mijenja. `spServisPravaQueryNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":MAINFORM"` otvara dijalog (`3879–3880`). `DETAILDBLCLICK=":UPDATE"` otvara `UPDATE` s detalja (R75). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. Nema `TREE` ni gumba na projektu.

## Forme

`MAINFORM` postoji. Natpis je `Dodavanje operatera servisu`. `MAXIMIZEBOX="TRUE"`.

`INSERT` combo `servisid` ima `ALLOWEMPTY="FALSE"` i `ONLOSTFOCUS="select 'refresh' as operateriid"`. Combo `operateriid` SQL ovisi o `@@servisid`. To su slučajevi iz `Combo-dijalog-testovi.md`.

Element `<FILTER>` je forma (`Form2.vb` 957, implementacijski R84). `FILLEXPLORER="TRUE"`, `ONSUBMIT` veže `@servisid=@@node`.

## Praznine

- Klik `ONCLICK` na dijalogu port još ne izvrši.
- Combo se pri otvaranju ne puni.
