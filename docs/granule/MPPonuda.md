# MPPonuda — stablo razdoblja, detail, gumbi po natpisu

XML: `definitions/netgen/Agronom granule/MPPonuda.xml`. Izbornik: `asm:NetGenDll.loader:MPPonuda.xml`.

Nema `ONLOAD`. Master se ne puni dok čvor ne vrati `FILLEXPLORER` (`Form1.vb` 3879 ostaje prazan string; prazan `Substring` padne na OERN).

`DETAILGRID = "spMPPonudaQueryGlaIDetNG @action=2, … @sifradok='118'"`. Razmak ispred `=` ne mijenja ime atributa. `@@id` je `GetID`.

`TREE` miješa statičke čvorove (`Danas`, `Jučer`, `Prošli mjesec`) i store `spMPTreeNG` (`4350–4355`). Forme `DANAS_FORM`, `JUCER_FORM`, `TEKUCIMJESEC_FORM`, `PROSLIMJESEC_FORM`, `FILTERODDO_FORM` nemaju `INPUT`, `FILLEXPLORER="TRUE"`. `@@node` je u `FILTERODDO_FORM`. Ostale šalju fiksni `@node='danas'` i slično.

`SETDEFAULTS` zove `spmpPermissionNG` s `@@xmlname` i šifrom `118`. To je jedan segment (`3862–3866`).

`ONLOAD2` vraća stupce `Dodaj`, `Izmjeni`, `Zakljuci`, `Obrisi`, `Storno`, `otkljucaj` s `enable` ili `disable`. Naknadno slaganje gumba traži stupac čije je ime **natpis gumba** `Button.Text` (`Form1.vb` 3914–3921`), ne `NAME`. `BUTTONS` natpisi moraju se poklopiti s tim aliasima. `Izmjeni` u `BUTTONS` i `Izmjeni` u `ONLOAD2` nisu isti zapis ako se slova razlikuju: usporedba stupca je ime kolone.

`BUTTONSoldNEkoristitiFiskal` nije atribut koji engine čita. `BUTTONS` jest (`3391`, `3770`). U živom nizu nema `FISKALIZIRAJ`. Zadnji gumb je `vrstadokid-MPPonudaGla` (`Class1.vb` 443–462`), ne `:repx/`.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="TRUE"` pali sumu mastera. `DETTOTALROWVISIBLE="FALSE"` gasi sumu detaila.

## Rupe

- Bez `ONLOAD` grid čeka klik na stablo.
- `enable`/`disable` gleda natpis gumba, ne `NAME`.
- Stari `BUTTONSold…` se ne čita. Fiskalni gumb iz tog niza ne postoji.
- `vrstadokid-` traži predložak vrste dokumenta, ne formu tog imena.
