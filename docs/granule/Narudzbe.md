# Narudzbe — TREETRANSLATE, NEXTFORM, grid na dijalogu

XML: `definitions/netgen/Agronom granule/Narudzbe.xml`. `TREETRANSLATE="TRUE"` je i na `MPPredispIz`, `MPNGPregDok`, `MPIntDok`, `MPPredispUl` i `BIOVEGA/Narudzbe.xml`.

## Prijevod i ikona stabla

`TREETRANSLATE` čita samo podloga (`Form1.vb` 3360). Stupac 2 SQL stabla ide kroz `Translate` (`4461–4464`, `4501–4504`). Dijaloški `INPUT TYPE="tree"` isti atribut ne gleda.

Statički čvor (`Narudžbe:…` u `TREE`) uvijek se prevodi (`4353`), i kad je atribut `false`.

Svaki čvor `TreeView2` nosi sliku 0 iz `ImageList2` (`Form1.designer.vb` 575–579). Ta slika je **izgled – Carpo**. Nema stupca za ikonu čvora i `ikonaid` grida nije ikona stabla (`7902–7905`). To ostaje.

## NEXTFORM

`UnosNar` `NEXTFORM="IzmjenaNar"`. `UnosStavkeNar` `NEXTFORM="UnosStavkeNar"`.

Nakon OK lanac (`Form2.vb` 4431–4464) otvara sljedeću formu i prenese id iz prvog stupca submita. Dozvola sljedeće forme pita ime prve (`4459`).

## Grid

`stavke`, `neobradjeni`, `g_ugo_stavke` su `TYPE="grid"` na dijalogu (`1623`). `CHECKBOXVISIBLE="FALSE"` sakrije stupac 1 (`1754–1755`). `g_ugo_stavke` ima `CHECKBOXVISIBLE="true"` i `EDITCOLUMNS`. `SKIPDEFAULTLOAD` ovdje nema: grid se puni čim se forma čita (`1643–1646`).

`TOTALROW="FALSE"` gasi sumu (`1745–1746`). Bez tog atributa suma ostaje.

## Rupe

- `TREETRANSLATE` na dijalog stablu ne radi ništa.
- Slika čvora (indeks 0) je **izgled – Carpo**. Čvor i dalje ne čita ikonu iz podatka, `ICON` ni `ikonaid`.
- `NEXTFORM` sam na sebe (`UnosStavkeNar`) vrti formu dok je OK.
- Grid dijaloga nije master. Sakriveni stupac je indeks 1.
