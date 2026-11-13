# MPEurParam — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPEurParam.xml`. Sedam baza, Biovega. Natpis je `Ažuriranje parametara za Eur`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":UPDATEFORM"`. Ulaz je dijalog (`3879`, R18). `BUTTONS="Izmjeni:UPDATEFORM"`. `CHECKBOXVISIBLE="FALSE"`.

## UPDATEFORM

`ONLOAD` forme je `spmpEurParam @action = 1`. To puni kontrole.

Gumb Spremi šalje `spmpEurParam @action=2` i završava s `#Ok`. `Ok` je potvrda (`6888`). Odustani je `close` (`7033`).

`@action=2` mijenja parametre eura. Na Jupiteru se ne šalje.
