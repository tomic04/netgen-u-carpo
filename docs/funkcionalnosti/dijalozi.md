# Dijalozi

Vrijednosti kontrola slaže `Form2.PokupiParametre`.

| Kontrola | Prazno | Napomena |
|---|---|---|
| `text` | `''` | apostrof postaje `¸¸`, u SQL-u `''` (`3655`, `Class1.vb` 850). Zarez postaje `~~` |
| `number` | `0` | zarez u broju postaje točka (`3601`) |
| tražilica `tablica#id#kolona` | `null` ako je tag prazan ili `0` | parametar je `Tag`, ne tekst (`3626–3632`). `!` na komadu je samo sort (`Form3.vb` 99–126) |
| `multidrop` | | id-evi spojeni tildom (`3446`) |
| `radio` | | `1` ili `0` (`3432`) |
| `check` | | `CheckState` |
| datum | | u parametru `yyyyMMdd` (`3531`). `firstdayofyear` je `01.01.` tekuće godine (`2254`) |
| combo `ALLOWEMPTY=true` | `null` | usporedba ne gleda velika slova (`2403`). Bez toga prazan combo pokuša vrijednost `1` (`3455`) |

`ONLOAD=":FILTERFORM"` nije SQL (`Form1.vb` 3879). Ime forme koje sadrži `filter` uključi DB clipboard, osim `DBCLIPBOARD=false` (`Form2.vb` 1102`).

`FILLEXPLORER=TRUE` i forma bez kontrola ne otvara dijalog: `ONSUBMIT` postane master (`5369`, `5445`). `!!` se prije toga skine i parametri se ne lijepe.

`REFRESHCONTROL(ime)` šalje `select 'refresh' as ime` (`6911`).
