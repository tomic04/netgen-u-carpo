# RatArkod — webbrowser

XML: `definitions/netgen/Agronom granule/RatArkod.xml`. Isti tip je i u `BIOVEGA/RatArkod.xml` i `Agronom granule/STStadoNG.xml`. U korpusu su tri datoteke.

Forma `WEBBROWSER`:

`ONLOAD` vraća kolonu `wb`: `preglednik.arkod.hr` ako je `@@Url` prazan ili `null`, inače `@@Url`.

`INPUT TYPE="webbrowser" NAME="wb" DOCK="5"`. Nema `URL`.

Pri stvaranju, ako `ONLOAD` ima red, `Navigate` na tekst kolone `wb` (`Form2.vb` 1419–1420`). `URL` bi se zatim navigirao velikim slovima (`1431–1433`); ovdje ga nema, OERN preskoči.

`ONLOADED="select 'refresh' as wb"`. Vrijednost `refresh` nije adresa (`4908–4913`).

`DOCK="5"` je `DockStyle.Fill`.

## Rupe

- Carpo layout nema `webbrowser` (`docs/analiza/04`). `Navigate` nije `document`.
- `URL` velikim slovima promijeni host ako se atribut doda.
- `refresh` ne smije postati adresa.
