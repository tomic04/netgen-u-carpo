# Kontrole — tip, atribut, granula

Razlike prema Carpo kontrolama su u `docs/razlike-netgen-carpo.md` (R30–R44).

`TYPE` nije enum. `ProcitajXML` (`Form2.vb` 705, `On Error Resume Next` na 706) za svaki `INPUT` radi neovisne `If InStr` nad malim slovima (`1256`–`2707`). Nije `ElseIf`. Prva pogodba u portu (`NgType.Odredi`) vraća jednu granu. U VB-u `dxgrid` postavi `nadjen` pa `grid` ne nastane (`1577`–`1623`). Tip koji sadrži dva ključa (npr. `label` i `text`) u VB-u složi dvije kontrole. U ovim granulama takvog `TYPE` nema.

`DATAFIELD` se nigdje ne čita. Combo veže stupac 0 kao vrijednost i stupac 1 kao prikaz (`2430`–`2431`). Zavisno punjenje je SQL koji vrati vrijednost `refresh` u koloni čije je ime kontrola (`5116`–`5182`), ne Carpo `datafield`.

## InStr red

| Red | Podstring | Linija | Klasa |
|---|---|---|---|
| 1 | `statusstrip` | 1256 | postojeći `StatusStrip1`, `TEXT1`–`TEXT3` u Tag, `.ToUpper` |
| 2 | `label` | 1296 | `Label` |
| 3 | `group` ili `frame` | 1354 | isti `GroupBox` |
| 4 | `panel` | 1384 | `Panel` |
| 5 | `check` | 1460 | `mojCheck` (`checkbox`, `vb.checkbox`) |
| 6 | `radio` | 1509 | `mojRadio` |
| 7 | `dxgrid` pa `grid` ako `nadjen=false` | 1579, 1623 | |
| 8 | `text` | 1996 | `textarea`, `vb.textbox` |
| 9 | `number` ili `cur` | 2071 | `currency`, `cmeditvb6.curmedit` |
| 10 | `trazilica` ili `search` | 2140 | |
| 11 | `dtpicker` ili `date` | 2232 | `datetime`, `datetiem` |
| 12 | `combo` | 2370 | |
| 13 | `multidrop` | 2478 | |
| 14 | `button` | 2614 | |

`b_utton`, `picture` (nema `picturebox`), `int`, `tekst` ne pogode nijednu granu. `ALOWEMPTY` nije `ALLOWEMPTY` (`2400`–`2403`).

## Tip → atribut → gdje

| Tip | Atribut / pravilo | Granula |
|---|---|---|
| combo | `ALLOWEMPTY` točno `true` (bilo koja slova) ubaci red `null` / prazan tekst (`2402`–`2411`). Prazan string nije true | `KntDospjeleStavke` `klpid`; `FakPar` `grupiranje` (`ALLOWEMPTY=""`) |
| combo | bez praznog reda, ništa odabrano: pokušaj `SelectedValue=1` (`3455`–`3468`) | `MPInventura` `sortpoid` (id-evi 1–4, pa 1 pogodi) |
| combo | nema `DATAFIELD`. Zavisno: `ONCLICK`/`ONLOSTFOCUS` → `Rafalno` → `RefreshControls` (`8497`–`8506`, `7510`, `5158`–`5182`) | `KntDospjeleStavke` `zupanijeid`→`mjestaid`; `MPInventura` `trgovinaid`→`osobeid`, `mpregijeId`→`str` |
| combo | `TRANSLATE=true` prevodi stupac 1 (`2420`–`2424`). `@@comboboxime` je prikaz (`3504`) | `DobUgovori` `DobUgovoriVrsteId` |
| search | počinje s `sp` ili `select`: taj SQL (`Form3.vb` 99–100). `dbo.sp…` ne počinje s `sp` | `MPInventura` `robaid` = `spMPRobaZaMPNG` |
| search | inače `tablica#id#kolone`. `%` je LIKE s te strane, `!` izbaci iz WHERE (`Form3.vb` 102–133). Stupac 1 grida je skriven (`165`) | `KntDospjeleStavke` `partneriid`; `MPInventura` `vipartneri#…#!adresa` |
| search | `ALLOWEMPTY` se ne čita. Tag `0` ili prazan šalje `null` (`2145`, `3626`–`3632`). Caption na `_` ne ulazi u parametre (`2149`–`2150`) | `FakPar` `partneriid` ima `ALLOWEMPTY=""` |
| date | ključne riječi samo `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth` (`2254`–`2265`). `today` nije među njima. `DEFAULT` se spusti na mala slova prije usporedbe | `MPInventura` `datumod`=`firstdayofmonth`, `datumact`=`TODAY` |
| date | `firstdayoflastmonth` u siječnju da mjesec 0 (`2263`–`2264`) | pravilo, nema granule u ovom bloku |
| date | `DEFALUT` se ne čita (traži se `DEFAULT`) | `MPInventura` `datumdo` |
| date | `SHOWCHECKBOX=true` ugasi kvačicu (`2275`–`2278`). Ugašeno šalje `null` (`3560`–`3567`). Inače `yyyyMMdd` (`3531`) | `DobUgovori` `datugovorvrijedido` |
| date | `datetime` i `datetiem` sadrže `date` | `MPNarIzdSkl` `datumdok`, `datum` |
| number / cur | prazan tekst šalje `0` (`3601`). Početak je `Text=0`, pa `DEFAULT`, pa `VALUE` (`2084`–`2088`). `cur` bilo gdje u `TYPE` je `NumerBox` | `KntDospjeleStavke` `iznos`; `FakPar` `CMEditVB6.CurMEdit` |
| number | `DECIMALS`, `FORMATMASK` samo ako atribut postoji (`2085`–`2086`, OERN) | `MPInventura` `kolicinastvarna` `DECIMALS=4`; `cijena` bez `DECIMALS` |
| check | `checkbox` i `vb.checkbox` sadrže `check`. Parametar je `CheckState` (`3424`–`3427`), ne 1/0 | `FakPar` `PoSkladistu`; `MPInventura` `knjustv` |
| radio | `1` ili `0` (`3432`–`3435`) | `MPInventura` `rbtnKumulativ` |
| multidrop | `id1~id2`, razmaci se brišu (`3446`). `ALLOWEMPTY` se pročita (`2500`) i ne ubaci red. `strMultiDropSQL` je zadnji multidrop forme (`2495`, `4817`) | `KntDospjeleStavke` `kntplanaid`; `MPInventura` `str` |
| label | ne šalje `@ime`. `ONCLICK` ide u `Tag` (`1345`). `!!` u tekstu je novi red (`2943`) | sve tri |
| frame / group | oba `GroupBox` (`1354`) | `KntDospjeleStavke` `FrameNG9`; `MPNarIzdSkl` `bottomgroupbox` |
| panel | `Panel`. `DOCK` je broj `DockStyle` (`1407`) | `MPNarIzdSkl` `DOCK=1` i `DOCK=2`, bez `NAME` |
| statusstrip | ne nastaje nova kontrola. `TEXT1`–`TEXT3` u Tag, velika slova (`1256`–`1260`). `@@` se traži bez obzira na slova (`Class1.vb` 865) | `KntDospjeleStavke` `s1` |
| tipfeler širine | `WUDTH` nije `WIDTH` (`2949`) | `MPNarIzdSkl` `pakovanje` |
| tipfeler projekta | `GRUOPBYBOXVISIBLE` nije `GROUPBYBOXVISIBLE` (`Form1.vb` 3350). `TREE33` nije `TREE` (`3387`) | `FakPar`; `MPNGPregDokMAIL` |
