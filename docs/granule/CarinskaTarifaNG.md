# CarinskaTarifaNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/CarinskaTarifaNG.xml`. Šest baza, Biovega. Natpis je `Carinska tarifa`. XML se ne mijenja.

Obrazac je u `Sifranik-obrazac.md`.

`ONLOAD="spCarinskaTarifaQuery @action=0, @id='', @carinskatarifaid='', @tarifa=''"`.

Forme su `CarinskaTarifaUnesi`, `CarinskaTarifaIzmijeni` i `CarinskaTarifaBrisanje`. Dvostruki klik je izmjena. Ispis je `PRINTREPXMASTER`.

Unos: Spremi šalje `spCarinskaTarifaAzurNG @action=0` i ostaje. Odustani je `ok`. `NEXTFORM="Dodaj"` nema formu.

Izmjena završava s `#OK` nakon `@action=1`. Brisanje je `@action=2 …#OK`. Na Jupiteru se azur ne šalje.
