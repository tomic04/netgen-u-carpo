# Dijalozi — koliko granula pogađaju četiri rupe

Izvor je svaki `PROJECT` pod `definitions/netgen`. Učitano je **601** datoteka, **507** različitih imena. Četiri datoteke se nisu dale pročitati: `KTEukpNG.xml`, `MPStanjeRobeTrg.xml`, `MPStanjeRobeTrgBackOffice.xml`, `MPStanjeRobeTrgBezNab.xml`.

Port je `Dijalog` na `cursor/netgen-filter-forma-3266` (`14d3208`). Četiri rupe iz [#43](https://github.com/tomic04/netgen-u-carpo/pull/43):

| Rupa | NetGen | Port |
|---|---|---|
| Klik gumba | `ONCLICK` ide u `Tag` i `Glbl_Click` (`Form2.vb` 1345–1349) | `DialogResult` prema natpisu, naredba se ne izvrši (`Dijalog.cs` 199, 234–238) |
| Combo pri otvaranju | SQL ide u `ExecSql` čim se kontrola složi (`Form2.vb` 2396–2427) | `LookUpEdit` bez SQL-a (`Dijalog.cs` 174–175) |
| Grid pri otvaranju | Bez `SKIPDEFAULTLOAD` grid zove `ExecSql` (`Form2.vb` 1635–1646) | `GridControl` bez izvora (`Dijalog.cs` 208–215) |
| `NEXTFORM` | Nakon OK-a otvara sljedeću formu (`Form2.vb` 1052, 4431–4458) | Atribut se ne čita |

Jedan popravak mehanizma pokriva sve retke te rupe. Redoslijed ispod je po tome što korisnik vidi čim se dijalog otvori.

## Ulazni dijalog

Ulaz je `ONLOAD` koji počinje s `:`, ili element `<FILTER>`. Takvih datoteka je **251** s dvotočkom i **34** s elementom `FILTER`.

| Rupa na ulaznoj formi | Datoteka | Imena |
|---|---:|---:|
| Combo ima vlastiti `SQL` | 198 | 169 |
| Gumb ima `ONCLICK` | 74 | 59 |
| Grid ima vlastiti `SQL` | 10 | 10 |
| `NEXTFORM` nije prazan | 1 | 1 |

Jedina ulazna forma s `NEXTFORM` je `BIOVEGA/KomStanjeSkl.xml`.

Redoslijed za prvi dodir: combo SQL, zatim `ONCLICK`, zatim grid SQL, zatim `NEXTFORM`.

Kriteriji za combo su u `Combo-dijalog-testovi.md`. Kriteriji za `ONCLICK` su u `Onclick-dijalog-testovi.md`. Kriteriji za grid i `NEXTFORM` su u `Grid-nextform-dijalog-testovi.md`.

## Sve forme u datoteci

Ista rupa na bilo kojoj formi, ne samo na ulazu. Korisnik je vidi tek kad tu formu otvori.

| Rupa | Datoteka | Imena | Kontrola ili forma |
|---|---:|---:|---:|
| Gumb s `ONCLICK` | 490 | 414 | 11443 gumba |
| Combo s vlastitim `SQL` | 487 | 412 | 4523 comba |
| Grid s vlastitim `SQL` | 381 | 327 | 1957 gridova |
| `NEXTFORM` | 93 | 85 | 103 forme |

Sve četiri rupe u istoj datoteci ima **78** datoteka. Barem jednu ima **574**.

Imena su manja od datoteka jer Biovega i Agronom često nose istu granulu. Popravak ide po mehanizmu, a proba po imenu.
