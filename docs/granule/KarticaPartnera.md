# KarticaPartnera — filter prije grida, `!!`, multidrop, datum

XML: `definitions/netgen/Agronom granule/KarticaPartnera.xml`.

## Ulaz

`ONLOAD=":FILTERFORM"`. Počinje s `:`, pa `NovaGenerickaForma` bez dvotočke. Nije SQL (`Form1.vb` 3879-3880).

`FILTERFORM` ima `FILLEXPLORER=TRUE`. Nakon OK-a `ONSUBMIT` postane master (`5445-5452`). `ONSUBMIT` završava s `!!`. Ta grana makne `!!` i **ne** lijepi `@ime=` (`5449-5450`). Parametri su već u tekstu kao `@@datod`, `@@kntplanaid`, …

Ime forme sadrži `filter`. `DBCLIPBOARD` postaje `true` osim ako je izričito `false` (`Form2.vb` 1102-1104).

## Kontrole

| Kontrola | Pravilo | Izvor |
|---|---|---|
| `datod` date, `DEFAULT="firstdayofyear"` | vrijednost `01.01.` + tekuća godina (`2254-2255`). U parametru `yyyyMMdd` u navodnicima (`3531-3532`) | |
| `datdo` date, caption `do:*` | zvjezdica na captionu forsira labelu (`2959-2962`) | |
| `kntplanaid` multidrop | označeni id-evi, zarez postane `~`, razmaci se brišu (`3441-3447`) | |
| `partneriid` search | SQL počinje sa `sp`, pa ostaje taj tekst (`Form3.vb` 99-100). `ALLOWEMPTY=FALSE` | |
| `DevValuteId` combo | `ALLOWEMPTY=true` se čita bez obzira na velika slova (`2403`) | |
| `AnalitikaPS` check | `CheckState` u parametru (`3424-3427`) | |

Gumb Prikaži: `If @@partneriid is null begin exec spRaiserror ... '+focus(partneriid)' end##OK`.

- `+FOCUS(` upiše ime kontrole (`Class1.vb` 202-204).
- `+focus` nije `+OK`, greška se ne briše (`187-189`).
- `Exit For` zaustavi lanac (`Form2.vb` 7511-7515), pa `OK` ne ode u spremanje.
- `##` je prazan segment. Prazan segment ne skače preko sljedećeg (`6821-6822`). Ako greške nema, `OK` se izvrši.

Izlaz je `CLOSE` (`7033`).

`CMD3CAPTION="Ispis:PRINTMASTER"` ide u `IspisGRIDREPX`, isto kao `PRINTREPXMASTER` (`Form1.vb` 4951-4955).

`CMD4` i `CMD5` su `::repx/... #CLOSE`. Nakon ispisa `CLOSE` zatvori formu (`Form2.vb` 7033). `!!` u captionu je novi red (`2943`), na `ONSUBMIT` gasi lijepljenje. Dva značenja, ovisno o mjestu.

## Rupe

- `ONLOAD=":FILTERFORM"` nije SQL. Grid se ne puni dok filter ne vrati `FILLEXPLORER` (`3879`, `5445`).
- `!!` na kraju `ONSUBMIT` zabranjuje drugi niz parametara (`5449`). Ako se `!!` tretira kao običan tekst, procedure dobiju krivi poziv.
- `+focus(partneriid)` mora vratiti fokus i ostaviti grešku, jer nema `+OK` (`Class1.vb` 187-204).
- multidrop šalje `id1~id2`, ne zarezom (`3446`).
- `firstdayofyear` je 1. siječnja tekuće godine, format parametra `yyyyMMdd` (`2254`, `3531`).
