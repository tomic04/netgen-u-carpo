# RobaAtributiUredi — picturebox

XML: `definitions/netgen/BIOVEGA/RobaAtributiUredi.xml`.

`INPUT TYPE="picturebox" NAME="slikalocation"`. Nema `LOCATION` ni `ONCLICK`.

`Form2.vb` 2707–2734:

- `LOCATION`, ako postoji, ide u `ImageLocation` nakon `ToUpper` (`2710`). `BlaBSUnos.xml` `pic1` ima UNC `LOCATION`; putanja postane velika slova.
- Kolona `ONLOAD` imena `slikalocation` prepiše `ImageLocation` (`2712–2713`). Ako tekst počinje s `http`, zove se `Load` (`2715–2716`).
- `StretchImage` (`2726`), `SendToBack` (`2732`).
- Klik ide kroz `Tag` = `ONCLICK` (`2729`, `2734`).

Ostale datoteke: `Agronom granule/HRCKadManagerVanjski.xml`, `BlaBSUnosPN.xml`, `BlaBSUnosFISK.xml`.

## Rupe

- Carpo `pictureedit` ne radi `ToUpper` na putanji i ne gleda `http` posebno (`docs/analiza/04`).
- Ime kontrole je `slikalocation`, ne `slika`. Kolona se zove tako. Preimenovanje ugasi sliku.
- Nema `LOCATION`: prazan `GetNamedItem` se preskoči (OERN na `706`).
