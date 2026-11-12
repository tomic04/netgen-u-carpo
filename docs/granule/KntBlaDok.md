# KntBlaDok — očekivano ponašanje

Izbornik traži `KntBlaDok.xml`. U repou je `definitions/netgen/Agronom granule/KntBlaDok.xml`. Rang 44, 6 baza, Biovega. Natpis je `Blagajnički dokumenti`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD="spBLDokumentiQueryNG @action=0"` puni master (`3879–3882`). Nema dodatnog filtera.

`TREE="Blagajne:#spBlagajneDokTreeNG …"`. Prvi segment `Blagajne` nema `:`. `On Error Resume Next` (`4298`) ga preskoči i nije roditelj (R10). Dva sljedeća segmenta sadrže `tree` i idu u store (`4350–4351`), s `@@operateriid`.

`DETAILGRID="spBLStavkeQueryNG @Action=0,@BlDokumentiId=@@id"` (R11). `MASTERDBLCLICK="IZMJENI_FORM"`.

`BUTTONS`: Unesi `UNESI_FORM`, Ispravi `IZMIJENI_FORM`, Obriši `OBRISI_GLA_FORM`.

`POPUPDETAIL` puni izbornik detalja (R83): `STAVKE_UNESI_FORM`, `STAVKE_IZMIJENI_FORM`, `OBRISI_STAVKU_FORM`. `CHECKBOXVISIBLE="FALSE"`.

## Praznine

- Prvi čvor stabla nema `:` (R10).
- Izbornik detalja nije izbornik mastera (R83).
