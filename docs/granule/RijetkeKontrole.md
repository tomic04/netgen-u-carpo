# Rijetke kontrole i putevi

Broj datoteka je nad `definitions/netgen` (Agronom i BIOVEGA su često ista granula u dva foldera).

## Stablo kao `INPUT` (11)

`Agronom granule/ManagerCjenika.xml`, `AOPNovi.xml`. `BIOVEGA/MPDodKlasaTransportno.xml`, `MPOptimalneZaliheNEWNC0.xml`, `JOPPD.xml`, `MPRobaNarPakovanje.xml`, `MPOptimalneZaliheNEWv2.xml`, `MPOptimalneZaliheNEW.xml`, `AOPNovi.xml`, `RobaWebKlase.xml`, `MPDodKlasaMinKol.xml`.

Grana `Form2.vb` 2558. SQL se reže po `#`, pa po prvoj `:`.

- lijevo je `yearmonth`: tijelo je prazno (`2576–2577`). Na podlozi ista riječ zove `DazovaVremenska` (`Form1.vb` 4348–4349).
- lijevo sadrži `tree`: `DazovaIzStore` (`2578–2579`, `2825–2844`). Tag korijena i djece je stupac 3 kako jest (`2842`, `2864`). Podloga, ako u stupcu 3 nema `:`, doda `:ImeForme` (`Form1.vb` 4474–4478, `4511–4514`).
- inače statički čvor, tag je `:` + desno (`2581–2583`).
- dijete s id `"0"` se ne crta (`2861`).
- `ExpandAll` (`2600`). `TREETRANSLATE` se ovdje ne čita.
- parametar je cijeli `Tag`, ne dio prije `:` (`3667–3674`). Podložni `@@node` je dio prije `:`.

`RobaWebKlase` `TYPE="TREE"` `SQL="spKlRobaWebKlase_Tree:PUNI"`. Lijevo sadrži `tree`, forma je `PUNI`. `ONCLICK` radi `refresh` gridova `klasa_da` i `klasa_ne`.

`AOPNovi` `leftgrid` SQL `spKntAOPOBRASCITreeAzur @action='tree' :LIJEVIGRIDFORMA`. Ista grana, jer lijevo od `:` sadrži `tree`.

## WebBrowser (3)

`Agronom granule/RatArkod.xml`, `STStadoNG.xml`, `BIOVEGA/RatArkod.xml`.

`Form2.vb` 1414. `URL` ide u `Navigate` nakon `ToUpper` (`1431–1433`). Ako `ONLOAD` ima kolonu imena kontrole, prvo se navigira njen tekst (`1419–1420`). `refresh` u toj koloni nije adresa (`4908–4913` samo proslijedi vrijednost).

`RatArkod` forma `WEBBROWSER`: nema `URL`. `ONLOAD` složi kolonu `wb` (`preglednik.arkod.hr` ili `@@Url`). `ONLOADED` je `select 'refresh' as wb`.

## PictureBox (5)

`Agronom granule/HRCKadManagerVanjski.xml`, `BIOVEGA/RobaAtributiUredi.xml`, `xml-ovi s nevkoša-Zubak/BlaBSUnosPN.xml`, `BlaBSUnosFISK.xml`, `BlaBSUnos.xml`.

`Form2.vb` 2707. `LOCATION` postane `ImageLocation` velikim slovima (`2710`). Zatim kolona `ONLOAD` istog imena (`2712–2716`); ako počinje s `http`, `Load`. `SizeMode` je `StretchImage` (`2726`). `SendToBack` (`2732`). `ONCLICK` je u `Tag` (`2729`).

`BlaBSUnos` `pic1` ima `LOCATION` (UNC putanja). `RobaAtributiUredi` `slikalocation` nema `LOCATION`; slika dolazi iz kolone `slikalocation`.

## NEXTFORM

262 datoteke imaju atribut. Prazan string nije sljedeća forma (`4431`).

Nakon OK, dok `strNextForm` nije prazan (`4431–4464`): zatvori trenutnu, nova `Form2` dobije id iz prvog stupca `ONSUBMIT` ako ima redaka (`4435–4438`), zatim `ProcitajXML`. Dozvola se pita za ime **prve** forme (`Form2b.strImeForme`, `4459`), ne za sljedeću.

`DobUgovori` `INSERT_FORM` `NEXTFORM="IZMJENI_FORM"`. `Narudzbe` `UnosStavkeNar` `NEXTFORM="UnosStavkeNar"` (isti naziv, petlja dok je OK).

## FILTERFORM i DB clipboard

`DBCLIPBOARD` se čita (`1099`). Ako ime forme sadrži `filter` i vrijednost nije `false`, postane `true` (`1102–1103`). `true` čita `NetGenDBClipboard` za operatera (`1170–1172`). Greška selecta ugasi clipboard; `CREATE TABLE` je komentar (`1173–1176`).

`MPNGPregDokMAIL` `FILTERFORM` ima `DBCLIPBOARD="false"`, pa ime `filter` ne pali clipboard. `KntDospjeleStavke` `MAINFORM` ima `DBCLIPBOARD="TRUE"` iako ime nema `filter`.

## SETDEFAULTS

280 datoteka. Tekst se reže po `#`, svaki segment je poseban `ExecSql` (`Form1.vb` 3862–3866, `SetDefolte` 3945–3962). Kolona `formcaption` prepiše naslov (`3964–3967`). Jedan batch sa `#` SQL Server odbije. `JeziciNG` nema `#` (jedan select). `PrijemniList` ima više segmenata.

## TREETRANSLATE i ikona

`TREETRANSLATE="TRUE"`: `Agronom granule/MPPredispIz.xml`, `MPNGPregDok.xml`, `MPIntDok.xml`, `Narudzbe.xml`, `MPPredispUl.xml`, `BIOVEGA/Narudzbe.xml`.

Samo podložno stablo (`Form1.vb` 3360, `4461–4464`, `4501–4504`): stupac 2 ide kroz `Translate`. Dijaloško stablo to ne radi (`Form2.vb` 2840, `2863`).

Statički čvor podloge uvijek zove `Translate`, i bez atributa (`4353`).

`TreeView2.ImageList` je `ImageList2`, `ImageIndex` i `SelectedImageIndex` su 0 (`Form1.designer.vb` 575–579`). Kod čvora ne postavlja drugu ikonu. Dijaloško stablo nema `ImageList`. Ikona retka grida (`ikonaid` → `ImageList3`, `Form1.vb` 7902–7905`) nije ikona stabla. `ICON` na projektu je ikona forme (`3392`), ne čvora.

## Grid na dijalogu

384 datoteke imaju `<INPUT TYPE="grid">`. Grana `Form2.vb` 1623, ali samo ako `dxgrid` nije već uzet (`1579–1623`).

`SKIPDEFAULTLOAD="TRUE"` ne puni pri otvaranju (`1643–1646`). Inače `ExecSql` odmah. `SQL2`–`SQL4` se spreme velikim slovima (`1630–1632`). `CHECKBOXVISIBLE="FALSE"` sakrije stupac 1, ne 0 (`1752–1757`). `TOTALROW` nije `FALSE` → suma je uključena (`1745–1748`). `SelectionMode` je višestruki (`1651`).

`RobaWebKlase` `klasa_da` / `klasa_ne`: `CHECKBOXVISIBLE="FALSE"`, `DOCK` 1 i 2, SQL s `@@klrobawebklasaid` (parametar stabla, cijeli tag).

Ispis tog grida je `PRINTGRID(` / `PRINTREPXGRID(` → isti `IspisGRIDREPX` s `"lodani"` (`6950`, `6985`, `9041–9056`).
