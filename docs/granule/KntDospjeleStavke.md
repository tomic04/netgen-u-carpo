# KntDospjeleStavke — combo, search `#`, multidrop, statusstrip, number

XML: `definitions/netgen/Agronom granule/KntDospjeleStavke.xml`. Forma `MAINFORM`. Ulaz izbornika je `CMD1CAPTION="FORMA:FORM0"`.

## InStr

| Kontrola | TYPE | Grana | Zašto |
|---|---|---|---|
| `s1` | `statusstrip` | 1256 | `TEXT1` i `TEXT2`. `TEXT3` nema, `GetNamedItem` padne i OERN (`706`) ostavi treći tag |
| `FrameNG9` | `frame` | 1354 | `GroupBox`, isto kao `group` |
| `lbldatdok` | `label` | 1296 | ne ulazi u `@ime` |
| `datumod`, `datumdo` | `date` | 2232 | nema `DEFAULT`. Nije `today` |
| `kntplanaid` | `multidrop` | 2478 | |
| `klpid`, `mjestaid`, `zupanijeid` | `combo` | 2370 | |
| `partneriid` | `search` | 2140 | |
| `iznos` | `number` | 2071 | `DECIMALS="2"`, nema `DEFAULT` ni `FORMATMASK` |

## Combo i zavisno punjenje

Nema `DATAFIELD`. `klpid` ima `ALLOWEMPTY="TRUE"`: red `null` / prazan tekst na vrh (`2402`–`2411`). Odabrani taj red šalje riječ `null` bez navodnika (`3472`), što SQL vidi kao null.

`zupanijeid` `ONLOSTFOCUS="select 'refresh' as mjestaid"`. Gubitak fokusa ide u `Rafalno` (`5905`). Segment nije komanda, pa `RefreshControls` (`7510`). Kolona `mjestaid` = `refresh` ponovo izvrši SQL comba (`5158`–`5182`), s `@@zupanijeid` već u parametrima. `ALLOWEMPTY` se i tada ubaci (`5165`–`5176`).

## Search

`partneriid` SQL ne počinje s `sp` ni `select`:

`dbo.viKntPartneriNG#partneriid#Naziv%#!Sifra#!Adresa#!MatBroj#!MjestaNaziv#!PorezniBroj`

`Form3.Trazilica` (`99`–`133`): tablica `dbo.viKntPartneriNG`, id `partneriid`, `Naziv%` je LIKE s obje strane, stupci s `!` idu u SELECT i ORDER BY, ne u WHERE. Stupac 1 grida se sakrije (`165`). Početni `Tag` je `"0"` (`2145`) i šalje `null` (`3626`–`3632`).

## Multidrop, broj, status, datum

`kntplanaid` šalje označene id-eve, zarez postane `~`, razmaci se brišu (`3441`–`3447`). `ALLOWEMPTY` na multidropu se pročita (`2499`–`2500`) i ne ubaci prazan red.

`iznos`: `Text = 0` (`2084`), zatim `DECIMALS` (`2086`). Prazan tekst u parametru je `0` (`3601`), zarez postane točka.

`s1` `TEXT1` ide u tag velikim slovima (`1258`). `@@rowcountgriddosp` postaje `@@ROWCOUNTGRIDDOSP`. Zamjena u `NadjiVrijednostuPrevParametrima` uspoređuje s `ToUpper` (`Class1.vb` 865), a parametar se slaže kao `@rowcount` + ime grida (`Form2.vb` 3818`).

`datumod` / `datumdo` nemaju `DEFAULT`. Nema ključne riječi. Kvačica je upaljena (`2275`) jer nema `SHOWCHECKBOX`, parametar je `'yyyyMMdd'` (`3531`).

## Rupe

- `DATAFIELD` ne postoji u ovom XML-u i engine ga ne čita. Dijete se puni kolonom `refresh`, ne bindanjem polja.
- `TEXT3` na statusstripu fali. OERN ne smije srušiti formu (`1256`–`1260`).
- multidrop ne gleda `ALLOWEMPTY`, iako atribut postoji na drugim granulama (`2500`).
- prazan `iznos` je `0`, ne `null` (`3601`).
