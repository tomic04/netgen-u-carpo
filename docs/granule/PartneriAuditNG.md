# PartneriAuditNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/PartneriAuditNG.xml`. Rang 50, 6 baza, Biovega. Natpis je `Pregled povijesti partnera`. XML se ne mijenja. `spPartneriAuditQueryNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTER"`. `CMD1CAPTION="Filter:FILTER"`. `CMD3CAPTION="Detalji:DETALJI"`. `MASTERDBLCLICK="DETALJI"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. Nema `TREE`.

## Element `FILTER`

Forma se traži kao `/PROJECT/FILTER` (`Form2.vb` 957). U XML-u je element `<FILTER>`, ne atribut `NAME`. `FILLEXPLORER="TRUE"`. `ONSUBMIT` je `spPartneriAuditQueryNG @Action=0,…`. `ONLOAD` forme i dalje ima `@@fklpartneriid`. To `ExecSql` preskoči (`Class1.vb` 113–118`).

Parser na ovoj grani drži ime `FILTER` strukturnim (`NetGenXmlReader.cs` 19), pa element nije forma. Implementacija to vodi kao svoj R84. Dok taj kod nije na grani koja se testira, stavka je otvorena.

## Praznine

- `FILTER` mora biti forma. Smoke je zato javio `nema-forme`.
