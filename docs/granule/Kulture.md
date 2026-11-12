# Kulture — unos, izmjena, brisanje

XML: `definitions/netgen/BIOVEGA/Kulture.xml`. Master je `spKultureNGQuery`, ne `spKulture` iz Carpo prepisa.

## Gumbi podloge

`CMD1CAPTION="Unesi:INSERT"`, `CMD2CAPTION="Izmjeni:UPDATE"`, `CMD3CAPTION="Obriši:DELETE"`, `CMD5CAPTION="Parametri:PARAMETRI"`.

Klik je `NovaGenerickaForma(sender.Tag)` (`Form1.vb` 5667). Tag je dio iza prve `:` (`3314-3325`).

## Dijalog i lijepljenje

`INSERT` `ONSUBMIT="spKultureNGAzur @Action=0,"`. Prvih 6 slova je `spKult`. To nije u `INSERTDELETEUPDATESELECT` (`Form2.vb` 3378). `strParametri` se ne briše. Roditelj šalje `stora + " " + parametri` (`Form1.vb` 5514).

`UPDATE` isto počinje sa `sp`, a u tekstu već stoji `@KultureId=@@ID`. `@@ID` zamijeni `SrediSQL2` (`Form2.vb` 2880). Parametri kontrola se i dalje dodaju na kraj.

`DELETE` `ONSUBMIT="spKultureNGAzur @Action=2,@KultureId=@@ID"`. Nema `INPUT` osim okvira, labele i dva gumba. Gumb `ONCLICK="OK"` zove `Button1` (`6888`).

## Kontrole

| Kontrola | Pravilo | Izvor |
|---|---|---|
| `Cijena` na INSERT je `text` | prazan tekst je `''`, apostrof ide `¸¸` pa `''` | `Form2.vb` 1996, 3655; `Class1.vb` 850 |
| `Cijena` na UPDATE je `number` | prazan broj je `0`, zarez u točku | `Form2.vb` 2071, 3601 |
| `robaid` search | SQL `roba#robaid#userid%#!cast(userid as varchar)#%naziv%`. Nije `sp`/`select`, pa `Form3` cijepa na `#`: tablica `roba`, id `robaid`, LIKE od trećeg komada. `!` je samo sort (`Form3.vb` 99-126). Parametar je `Tag` (id). Prazan tag je `null` (`Form2.vb` 3626-3632) | |
| `orgshemaid` combo | stupac 0 vrijednost, stupac 1 natpis. `ALLOWEMPTY=TRUE` ubaci prvi red `null` (`Form2.vb` 2402-2411). Bez `ALLOWEMPTY` prazan combo pokuša `SelectedValue=1` (`3455-3468`) | |

Gumb Dodaj na unosu: `if ... exec spRaiserror ...#OK`. `RefreshControls` izvrši SQL (`7510`). Ako ostane greška, `Exit For` i `#OK` se ne izvrši (`7511-7515`). Poruka nema `#OK`/`+OK`, pa se greška ne guta (`Class1.vb` 187-189).

Odustani je `CLOSE` = `Button2`, `OKClick` false (`7033`, `3400`).

`PARAMETRI`: gumb Dodaj u `ONCLICK` ima `select 'Naziv' as focus` i `'refresh' as Naziv`, `'refresh' as stavke`. Stupac `focus` fokusira kontrolu (`4627-4632`). Vrijednost `refresh` na tekstu i gumbu ide kroz `DodatnoPostimajKontrolu` (`4974-4998`). `:PARAMETARUPDATE` otvara formu (`6882-6883`).

## Rupe

- `ONSUBMIT` koji počinje sa `sp` dobije drugi niz `@ime=` iako već ima svoje parametre (`Form2.vb` 3378, `Form1.vb` 5514).
- `Cijena` nije isti tip na unosu i izmjeni. Prazan tekst i prazan broj nisu ista vrijednost (`3655`, `3601`).
- Tražilica `roba#...` nije Carpo `searchedit`. WHERE se gradi u `Form3.vb` 102-126.
- `spRaiserror` bez `#OK` u tekstu poruke prekida ostatak lanca (`7515`).
