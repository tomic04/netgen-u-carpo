# Zito

XML granule klijenta Žito, kopirane 17. 11. 2026. iz mape `žito xml\xml - original` (888 datoteka, s podmapama
`MER`, `old`, `Rat`, `SKLAD`, `test`, `ucinak`, `UTP`). Datoteke nisu mijenjane.

- 885 su NetGen granule (`<PROJECT>`). `ExcelImportTem.xml`, `VagarskiList.GridOptions.xml` i `VagarskiListDef.xml`
  nisu granule, pa ih smoke parsera vodi u `FAILED_LIST`.
- 487 granula po imenu nema u ostalim mapama korpusa, 278 su druge verzije postojećih, 120 je isto.
- `old` su starije verzije istog klijenta.

Baza ovog klijenta nije na serverima na kojima se testira (`sql-ai`, `jupiter2019`, `jupiter2022`), pa granule
pokrivaju samo provjere bez baze: smoke parsera, raspored formi i sweep.
