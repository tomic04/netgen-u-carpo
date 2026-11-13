# PDVPromjenaNEW — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PDVPromjenaNEW.xml`. Osam baza, Biovega. Natpis je `Promjena stope PDV-a`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"`. Ulaz je dijalog (`3879`, R18).

`TREE="spTreeKlMasterRoba:KLASIFIKACIJA"`. Store (`4350–4351`, R01). Klik otvara `KLASIFIKACIJA`, a `FILLEXPLORER="TRUE"` zamijeni master s `spPdvPromjenaNew @action=1, @klmasterrobaid=@@node` (`5445–5452`).

`BUTTONS` počinje s `Parametri:FILTERFORM#Pripremi promjenu stope…`. `POPUPMASTER` ima `INSERTFORM`, `DELETEFORM` i `OBRISISVE`, svaki s `:`.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Dijalozi

`FILTERFORM` ima `ONLOAD` koji stavi `getdate()+1` u `datumpromjene`, `ONSUBMIT="spPdvPromjenaNew @action = 1,"` i `FILLEXPLORER="TRUE"`.

`DELETEFORM` šalje `@action = 55` i na submitu ima `!!`, pa se drugi niz parametara ne lijepi (`5449–5450`, R14). `OBRISISVE` šalje `@action = 555`.

Te akcije mijenjaju najave stope. Na Jupiteru se `spPdvPromjenaNew` s tim akcijama ne šalje. `@action = 1` je prikaz.
