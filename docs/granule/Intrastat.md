# Intrastat — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/Intrastat.xml` (windows-1250). U izborniku je na 10 baza, uključujući Biovegu. Ponašanje je iz NetGen VB-a. XML se ne mijenja. `spIntrastat_Query` se u testu ne šalje.

Izgled je Carpo (R59). **izgled – Carpo** ne ruši QA. Natpis, redoslijed, naredba, širina, skriveni stupac, sort, checkbox, detail i fokus ostaju.

Log ide u `NgDnevnik`, ne u `CBLog`.

## Podloga

`VERSION="2.4"`. Naslov je `Intrastat  v[2.4]` (dva razmaka, `Form1.vb` 3524–3525). `NAME` nije naslov.

`ONLOAD` je `spIntrastat_Query @action = 0, @godina = @@godina, @mjesec = @@mjesec, @tokrobeid = @@tokrobeid, @vrstaobrascaid=@@vrstaobrascaid`. Ne počinje s `:`, pa ide u `PokaziUpit` (`3879–3882`). `@@` koji preživi zamjenu ne smije se poslati (`Class1.vb` 113–118).

`DETAILGRID` je `spIntrastat_Query @action = 4, @id = @@id`. Veza je stupac `id`, ne natpis (R11). Poziv je poslije mastera i na promjenu retka (`2109`, `7413–7416`).

`SETDEFAULTS` ima `#if exists`. Reže se na `#` (`3862–3866`, R51). Prvi komad je `select naziv as tkosmomi ...`. Drugi počinje s `if exists` i vraća `IndPromjenaNazivaRobe`. Nisu jedan batch.

`COMMANDTIMEOUT="300"` prepiše default `"30"` (`Module1.vb` 177, `Form1.vb` 3463, `Class1.vb` 99, R65).

`CHECKBOXVISIBLE="FALSE"` sakrije checkbox stupac (`2043–2046`). `TOTALROWVISIBLE="FALSE"` gasi sumu (`2049–2050`). `GROUPBYBOXVISIBLE="TRUE"` ostavlja group box. `FROZENCOLUMNS="0"`. Sort klikom ostaje (`3401–3404`). Nema `DXGRID`. **izgled – Carpo:** DevExpress, bez row headera, praznih redaka i `HeaderLines=3` kao uvjeta.

Gumbi, lom na prvoj `:` (`3314–3334`). `&` ostaje u natpisu (mnemonik):

| # | Natpis | Naredba |
|---|---|---|
| 1 | `&Unesi` | `Unos` |
| 2 | `&Izmijeni` | `Izmjena` |
| 3 | `&Obriši` | `Brisanje` |
| 5 | `&Export XML` | `EXPORT` |
| 6 | `&Kopiraj` | `Kopiranje` |
| 7 | `&Ispiši` | `repx/rptIntrastat(@id=@@id)` |
| 9 | `&Filter` | `Filter` |

`CMD4` i `CMD8` nisu napisani, gumbi su skriveni (`3833–3836`). `repx/` na podlozi postane `repx:` i ime se reže na `(` (`5215–5239`, `4407–4418`). `@@id` ide u parametar ispisa.

`MASTERDBLCLICK="Izmjena"`.

## Stupci

`DETAILCOLUMNS` `CLEARCOLUMS="false"`, `HEADERALIGNMENT="1"`. `false` ne sakrije ostale stupce (`7459–7462`). Poravnanje zaglavlja je **izgled – Carpo**. Širina je `WIDTH/14` (`7522`). `SifraRobe` je 900/14, `OpisRobe` 3000/14. Prazan `AGREGATEFUNCTION` ne veže sumu (`7506`).

## Forme

`UNOSSTAVKE`, `IZMJENASTAVKE`, `DELETEITEM`, `CAROBNJAK`, `CAROBNJAK2`, `RAZDIJELI`, `DONOS`, `EXPORT`, `NMASA`, `TECAJ`, `PROVJERE`. Test ne šalje njihov SQL. Greška na `ONLOAD` ne otvara formu (`Form2.vb` 1199, `Form1.vb` 4429).

## Praznine

- `SETDEFAULTS` s `#if` su dva `ExecSql`, ne jedan tekst s `#` (R51).
- Timeout mastera je 300 s, ne 30 (R65).
- Detail veže `@id = @@id` iz stupca `id` (R11).
- Ispis je `repx/rptIntrastat` s parametrom u zagradi, ne forma tog imena (R55).
- `&` u natpisu ostaje.
