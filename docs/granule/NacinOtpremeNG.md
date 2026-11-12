# NacinOtpremeNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/NacinOtpremeNG.xml`. Sedam baza, Biovega. Natpis je `Način otpreme`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spProNacinIsporukeQuery @action = 0, @id=''"`. Master se puni odmah (`3879–3882`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | &Unesi | `NacinOtpremeUnesi#refreshmaster` |
| 2 | &Izmijeni | `NacinOtpremeIzmijeni` |
| 3 | &Obriši | `NacinOtpremeBrisanje` |
| 5 | Is&piši | `PRINTREPXMASTER` |

`MASTERDBLCLICK="NacinOtpremeIzmijeni"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

## Dijalozi

`NacinOtpremeUnesi` `buttonOK` šalje `spProNacinIsporukeAzur @action = 0`, zatim `OKStay`, zatim `select` koji prazni polja. `OKStay` ne zatvara formu (`6884–6887`). `buttonCLOSE` ima natpis Odustani i `ONCLICK="ok"`, što je potvrda. `NEXTFORM="Dodaj"` traži formu koje nema.

Izmjena završava s `#OK` nakon `@action = 1`. Brisanje je `@action = 2 …#OK`.

`spProNacinIsporukeAzur` mijenja podatke. Na Jupiteru se ne šalje.
