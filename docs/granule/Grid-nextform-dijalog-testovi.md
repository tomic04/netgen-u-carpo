# Grid i NEXTFORM na dijalogu

Port nacrta prazan `GridControl` i ne čita `SQL` (`Dijalog.cs` 208–215). `NEXTFORM` se ne čita. NetGen oba radi čim se forma složi.

## Grid s vlastitim SQL-om

Deset ulaznih datoteka. Ulaz je `ONLOAD` koji počinje s `:`, ili element `FILTER`.

`TYPE` koji sadrži `grid`, a nije već uzet kao `dxgrid`, slika `SQL` u `zadnjiSQL` kroz `SrediSQLPosljeNextForme` (`Form2.vb` 1623–1638). `SKIPDEFAULTLOAD` nakon `ToUpper` mora biti točno `TRUE` da se upit preskoči (`1635–1646`). `FALSE` i prazan atribut pune grid. `dxgrid` puni uvijek, bez tog atributa (`1587–1591`).

`@@id`, `@@operateriid`, `@@node` i `@@detid` zamijeni `SrediSQL2` (`2876–2880`). Ostali `@@` traže se u parametrima i, ako ih nema, postanu prazan tekst (`Class1.vb` 765–849). Upit se ipak pošalje. `ExecSql` preskoči samo ono što i dalje sadrži `@@` (`113–118`).

Stupci dolaze iz rezultata (`CreateGridColumns`, `982`). Zadnja četiri znaka natpisa, ako su broj, postanu širina dijeljena s 14 (`1206–1208`, `Module1.vb` 65). Natpis koji završava s `ID` je skriven (`1225–1226`). To gleda izvorni natpis, prije rezanja širine. String bez te četvorke ima širinu 100 (`1203`).

| # | Grid | Ulaz | Očekivano |
|---|---|---|---|
| 1 | `ServisPrava` `MAINFORM.master` | `spServisPravaQueryNG @action=2`, nema `SKIPDEFAULTLOAD` | Upit ide čim se kontrola složi (`1646`). |
| 2 | `ServisPrava` `MAINFORM.detail` | `@servisid=@@master` | `@@master` nije ključ iz `SrediSQL2`. Na otvaranju postane prazan tekst i upit se pošalje. |
| 3 | `RekapitulacijaUI` `Parametri.strskl` | `select skladisteid … '@@nizoznacenih…'` | `skladisteid` završava s `id` i stupac je skriven. `@@niz` kojeg nema u parametrima postane prazan, upit ide. |
| 4 | `ManagerCjenika` `MAINFORM.g_master` | `declare … if @@find = 0 … exec spCjenik_Query` | `@@find` na otvaranju postane prazan. Tekst `if  = 0` je greška izvršenja, grid ostane bez stupaca (`984`). |
| 5 | `KomStanjeSkl` `MAINFORM.skladisteid` | `select skladisteid, … naziv, … oznaka, 1 as selectedid from viSkladistaPravaIzvjestaji` | Nema `@@`. `skladisteid` i `selectedid` su skriveni. Naziv i oznaka ostaju, širina 100. |
| 6 | `MPPonudaNaruciRazliku` `FILTERFORM.povezinar` | `SKIPDEFAULTLOAD="TRUE"`, `spmpPonudaNaruciRazliku @action = 2` | `TRUE` preskoči `ExecSql` (`1643`). Grid je prazan iako SQL postoji. Susjedni `povezinar1` ima isti `TRUE` i `@id = @@povezinar`. |
| 7 | `IspisNaljepnica` `MAINFORM.detalji` | `spIspisNaljepnica … @ind = 'D', @id = @@glava` | `@@glava` na otvaranju postane prazan. Upit ide s praznim `@id`. `glava` iznad njega nema `SKIPDEFAULTLOAD` i puni se odmah. |
| 8 | `MPAnalBrStavaka` `FILTERFORM.stavke` | `select … [Rbr0400] … from ##tmpanalstavke` | Upit ide, uključujući `##` tablicu. `Rbr0400` ima širinu `400/14`. Ako tablice nema, greška ostavi grid prazan. |
| 9 | `MPPregArtNG2` `FILTERFORM.grdrobaOdabir` | `SKIPDEFAULTLOAD="FALSE"`, `exec spMPPregArtng @action = 6` | `FALSE` nije `TRUE` (`1643`). Upit ide. Isti je `FALSE` na `MPPregRbTrNGBezPravaV2` `FORM1.grdrobaOdabir`. |
| 10 | `ProNotpL` `MAINFORM.stavke` | `select pd.pronotpdetid, r.robaid, r.userid as Šifra1000, …` | `pronotpdetid` i `robaid` su skriveni. `Šifra1000` ima širinu `1000/14`, a zadnja dva znaka izvornog natpisa nisu `ID`. |

## NEXTFORM

Jedina ulazna forma s `NEXTFORM` je `BIOVEGA/KomStanjeSkl.xml` `MAINFORM`. Ostalih je 103 forme u 93 datoteke. Uzorak ispod pokriva ime, dvotočku, `@@`, upis, prazan submit, formu koje nema i lanac na samu sebe.

Atribut se čita cijeli (`Form2.vb` 1052). Nakon `ShowDialog`, dok je `OKClick` i `NEXTFORM` nije prazan, roditelj s ljuske (`Form1.vb` 5381–5425) prvo pošalje `ONSUBMIT`, uzme id iz prvog stupca prvog reda, osvježi zadnji master i otvori sljedeću formu pod tim id-om. Dijalog otvoren iz dijaloga radi isto u `Form2.vb` 4431–4464, a između formi zove `RefreshKontrola`.

Forma s 0 kontrola ne dobije `ShowDialog`. `OKClick` postane true (`4430`, `5425`). `CLOSE` ostavi `OKClick` false, pa se lanac ne nastavi (`3400–3402`).

Kad lanac stane, `FILLEXPLORER` vrijedi za zadnju formu, ne za onu koja je imala `NEXTFORM` (`5429`).

| # | Forma | `NEXTFORM` | Očekivano |
|---|---|---|---|
| 1 | `KomStanjeSkl` `MAINFORM` | `ButtonKartica:KARTICA` | Ulaz je `:MAINFORM`. Cijeli tekst je ime forme. `KARTICA` postoji kao drugi element i ne otvara se ovim imenom. `SelectNodes` s dvotočkom ne nađe čvor, kontrola je 0, dijalog se ne vidi. `FILLEXPLORER="true"` na `MAINFORM` se ne primijeni jer ta forma nije zadnja u lancu. `ONSUBMIT` je prazan. |
| 2 | `MPPSNG` `UPDATE_FORM0` | `:@@UPDATE_FORM` | Ime ostaje doslovno, s dvotočkom i `@@`. Zamjena `@@` iz `SrediSQL2` ovdje se ne zove. |
| 3 | `KntKompVerizne` `DODAJ_FORM` | `IZMJENI_FORM` | `ONSUBMIT` je `spKntKompVerizneAzur @action=0`. Prije otvaranja id postane prvi stupac prvog reda (`4435–4439`). Forma `IZMJENI_FORM` postoji. |
| 4 | `MPNarIzdSkl` `INSERTFORM` | `UPDATE_FORM` | `spMPInsertGlaIdetNG @action=1` ide prije sljedeće forme. To je upis. Na Jupiteru se ne šalje. Id nove forme je prvi stupac. |
| 5 | `DrzaveNG` `DrzaveUnesi` | `Dodaj` | Forma `Dodaj` ne postoji. Nakon potvrde nema dijaloga (`4430`). Gumb Odustani na toj formi ima `ONCLICK="OK"`, pa potvrda ipak krene u ovaj lanac. |
| 6 | `RobaAtributiUredi` `AZURFORM` | `AZURFORM` | Sljedeća forma je ista. Svaka potvrda otvori je opet, dok korisnik ne zatvori bez `OKClick`. |
| 7 | `NabStoNarucitiBiovegaNG` `NARUDZBA` | `Naruceno` | `Naruceno` ima vlastiti `NEXTFORM="FILTERFORM"`. Lanac ide dalje samo ako je i `Naruceno` zatvoren s `OKClick`. |
| 8 | `MER_ZahtjevZaNabavkuNG` `INSERT` | `UPDATE` | `ONSUBMIT` je prazan. `UPDATE` se otvori s istim id-om koji je forma imala pri ulasku (`4434`). |
