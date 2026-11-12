# MPInventura — datum ključne riječi, radio, check, search, multidrop

XML: `definitions/netgen/Agronom granule/MPInventura.xml`.

## Datum

`FILTERFORM.datumod` `DEFAULT="firstdayofmonth"`. Nakon `ToLower` (`2253`) grana `2260`–`2261` složi `01.` + mjesec + `.` + godina, bez vodeće nule na mjesecu.

`FILTERFORM.datumdo` ima `DEFALUT="today"`, ne `DEFAULT`. `GetNamedItem("DEFAULT")` ne nađe atribut (`2252`). Riječ `today` se ne primijeni.

`INSERTROBANE_FORM.datumact` `DEFAULT="TODAY"`. To nije ključna riječ (`2254`–`2265` poznaju samo `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth`). Tekst se spusti na `today` i ide u `Value` (`2267`). `DTPicker` je `DateTimePicker` (`DTPicker.vb` 3–4). Uspjeh parsiranja riječi `today` nije zajamčen; OERN (`706`) ostavi staru vrijednost. Port ne smije uvesti `today` kao novu ključnu riječ.

`firstdayoflastmonth` u siječnju da `01.0.godina` (`2263`–`2264`). Ova granula tu riječ nema.

`SHOWCHECKBOX` na ovim datumima nema. Kvačica ostaje upaljena (`2275`), parametar je `'yyyyMMdd'` (`3531`).

## Combo, radio, check

`INSERTFORM.trgovinaid` `ALLOWEMPTY="FALSE"`. Nije `true` (`2403`), prazan red se ne ubaci. `ONCLICK="select 'refresh' as skladisteid, 'refresh' as osobeid"` ide u `Glbl_ComboClick` → `Rafalno` (`8497`–`8506`). Oba stupca vrijednosti `refresh` ponovo pune istoimene comboe (`5158`).

`sortpoid` SQL ima `id` 1, 2, 3, 4 i `ALLOWEMPTY="FALSE"`. Ako ništa nije odabrano, `PokupiParametre` pokuša `SelectedValue = 1` (`3459`–`3464`) i to pogodi prvi red.

`INSERTGROUP_FORM.mpregijeId` `ALLOWEMPTY="TRUE"`, `ONCLICK="select 'refresh' as str"`. `str` je multidrop. Refresh multidropa koristi `strMultiDropSQL` (`4813`–`4821`), a ta varijabla je SQL zadnjeg multidropa učitanog na formu (`2495`). Ovdje je jedan, `spMPDajTrgovinaNG … @mpregijeId = @@mpregijeId`.

`MPKARTICA` `rbtnKumulativ` / `rbtnAnalitika` su `radio` (`1509`). Parametar je `1` ili `0` (`3432`–`3435`). `ONCLICK` radi `refresh` / `visible` / `invisible` grida.

`knjustv` i `samokomisija` su `check` (`1460`), ne `checkbox`. Šalju `CheckState` (`3424`–`3427`).

## Search i broj

`INSERTFORM.partneriid` SQL je `vipartneri#partneriid#%naziv%#%mjestanaziv%#sifra#!adresa`. Nije `sp`/`select` (`Form3.vb` 99). `%naziv%` i `%mjestanaziv%` su LIKE s obje strane. `sifra` nema `%`, pa je LIKE bez postotka. `!adresa` nije u WHERE (`118`–`119`). `TRAZI.robaid` počinje s `spMPRobaZaMPNG`, pa ostaje taj tekst, uz `@@searchboxrobaid`.

`ALLOWEMPTY="FALSE"` na searchu se ne čita (`2140`–`2216`).

`TRAZI.kolicinastvarna` `DECIMALS="4"`. `cijena` nema `DECIMALS` ni `FORMATMASK`: dodjela se preskoči (`2085`–`2086`, OERN). Prazan broj šalje `0` (`3601`).

## Rupe

- `DEFALUT` nije `DEFAULT`. `datumdo` ne dobije `today`.
- `TODAY` nije ključna riječ. Samo četiri `firstday*` (`2254`–`2265`).
- `ALLOWEMPTY=FALSE` na combu čiji SQL ima red `1` slučajno odabere taj red (`3459`).
- `ALLOWEMPTY` na searchu se ignorira.
- Refresh multidropa dijeli jedan `strMultiDropSQL` (`2495`).
