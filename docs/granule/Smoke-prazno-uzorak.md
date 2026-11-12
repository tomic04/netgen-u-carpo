# Smoke prazno — uzorak 20

Izvor brojeva: CI #35, `docs/granule/smoke-spin.md` na `cursor/netgen-smoke-broj-3266` (`1c25e51`). XML u repou: **605**. S retcima **82**. Prazno **413**. Pad **110**, od toga **91** `nema-procedure`.

Prazno znači da je prozor otvoren i master ima 0 redova. Uzorak je 20 od tih 413. Usporedba je s `Form1.SrediSQL` (`2470–2502`) i `Class1.ExecSql` (`105–118`). Port radi istu zamjenu u `SqlTekst.Sredi`. Operater u smokeu je `7`. `@@id` na podlozi kreće kao prazan string (`Form1.vb` 1186). `@@node` kreće kao `null` (`Form1.vb` 39). Na formi je `@@id` `0` (`GetID`, `5567–5582`).

Ni jedan od ovih 20 nije prazan zato što je port dodao tuđi filter. `nema-procedure` (91) nije u uzorku: to su padovi, ne prazni gridovi.

## Nema SQL-a na ulazu (10)

`ONLOAD` je prazan. `ExecSql` dobije prazan string i vrati `Nothing` (`Class1.vb` 113–116). Master ostane prazan dok čvor ili combo ne pošalje svoj SQL. To nije „upit je vratio 0 redova“.

| XML | Što NetGen čeka |
|---|---|
| `Akcije` | čvor `spAkcije_Tree` / `REFRESHTREEFORM` |
| `JOPPD` | čvor stabla, `onlytree` ne šalje master (R54) |
| `MPBlagajna` | čvor stabla |
| `MPPSNG` | čvor s `FILLEXPLORER=TRUE` |
| `MPPonuda` | čvor stabla |
| `PrijemniList` | `COMBO` upiše `TREE` i tek onda puni stablo (`7730–7733`, R03). `ONLOAD` nema |
| `MPKnjigaPopisa` | nema ni `ONLOAD` ni `TREE`. Grid ostane prazan |
| `PNKatalozi` | klik na store `spPNKatalozi_Tree` |
| `KOD_RSPostavke` | klik na store |
| `MPInventura` | čvor stabla |

## Forma, `@@` ostane (8)

`ONLOAD` projekta je `:ime`. Master SQL se ne šalje (`3879–3880`). `ONLOAD` forme i dalje ima `@@datod`, `@@datumdo`, `@@trgovinaid`, `@@izvjestaj` i slično. To nisu tokeni iz `SrediSQL`. `ExecSql` vidi `@@` iza prvog znaka i preskoči (`113–118`). Port radi isto (`SqlTekst.Odluka` = `ostao-@@`). Master ostane prazan dok `FILLEXPLORER` ne pošalje `ONSUBMIT`. To nije prazan rezultat upita.

| XML | Forma | Što ostane |
|---|---|---|
| `kntDobDospRac` | `FILTERFORM` | `@@datod`, `@@datdo` |
| `KntOPZ-STAT-1` | `UPIT` | `@@datumDoParam`, `@@datdospparam` |
| `KntPDVObrasciEU` | `FILTER_FORM` | `@@checkdatod`, `@@datod` |
| `KntR2racuni1` | `FILTERFORM` | `@@datod`, `@@datdo` |
| `PDVpoStjecanju` | `FILTERFORM` | `@@datod`, `@@datdo` |
| `Predujmovi` | `FILTERFORM` | `@@datumod`, `@@datumdo` |
| `MPRACNEFISK` | `FORM1` | `@@datumod`, `@@datumdo`, `@@trgovinaid` |
| `Bilanca-GFI` | `FILTERFORM` | `@@datod`, `@@datdo`. Lanac iza `#` ima `@@izvjestaj`, pa bi i NetGen preskočio taj segment (R71, `113–118`) |

## SQL je poslan i vratio 0 (2)

Zamjena ne doda filter kojeg nema u XML-u. Smoke je taj SQL izvršio i dobio 0 redova. Prazno je rezultat upita.

| XML | SQL koji i NetGen šalje |
|---|---|
| `KntBlaDok` | `spBLDokumentiQueryNG @action=0` |
| `LVObracun` | `spLVGla_Query @action='master', @treenode='0'` |

`@treenode='0'` je literal u XML-u, ne zamjena `@@node`.
