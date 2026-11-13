# DrzaveNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/DrzaveNG.xml`. Devet baza, Biovega. Natpis je `Države`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spdrzaveQuery @action=0, @id=''"`. To je upit, master se puni odmah (`3879–3882`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | &Unesi | `DrzaveUnesi#refreshmaster` |
| 2 | &Izmijeni | `DrzaveIzmijeni` |
| 3 | &Obriši | `DrzaveBrisanje` |
| 5 | Is&piši | `PRINTREPXMASTER` |

`MASTERDBLCLICK="DrzaveIzmijeni"`. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

`SETDEFAULTS` puni `tkosmomi` iz `partneri` / `pubvars`.

## Dijalozi

`DrzaveUnesi` nema `ONSUBMIT`. `buttonOK` šalje `spDrzaveAzur @action=0` i na kraju radi `select 'refresh'`. Nema `#OK`, forma ostaje otvorena (`7509`). `buttonCLOSE` ima natpis Odustani i `ONCLICK="OK"`. To je potvrda (`6888`), ne `CLOSE`. `NEXTFORM="Dodaj"`. Forma `Dodaj` ne postoji, pa se dijalog ne prikaže (`4430`).

`DrzaveIzmijeni` `buttonOK` završava s `#OK` nakon `@action=1`. `buttonCLOSE` je `CLOSE`.

`DrzaveBrisanje` šalje `spDrzaveAzur @action=2 …#OK`.

`spDrzaveAzur` mijenja podatke. Na Jupiteru se ne šalje.
