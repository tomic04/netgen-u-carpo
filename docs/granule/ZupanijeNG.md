# ZupanijeNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/ZupanijeNG.xml`. Osam baza, Biovega. Natpis je `Županije`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spZupanijeQuery @action=0, @id='', @Naziv=''"`. Master se puni odmah (`3879–3882`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unesi | `ZupanijeUnesi#refreshmaster` |
| 2 | Izmjeni | `ZupanijeIzmijeni` |
| 3 | Obriši | `ZupanijeBrisanje` |
| 5 | Ispiši | `PRINTREPXMASTER` |

`MASTERDBLCLICK="ZupanijeIzmijeni"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Dijalozi

`ZupanijeUnesi` `buttonOK` šalje `spZupanijeAzur @action=0` i osvježi comboe. Nema `#OK`, forma ostaje. `buttonCLOSE` ima natpis Odustani i `ONCLICK="OK"`. `NEXTFORM="Dodaj"` traži formu koje nema (`4430`).

`ZupanijeBrisanje` šalje `spZupanijeAzur @action=2 …#OK`.

`spZupanijeAzur` mijenja podatke. Na Jupiteru se ne šalje.
