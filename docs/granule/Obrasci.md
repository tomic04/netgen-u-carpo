# Obrasci, ne svih 431

`Prioritet-2.md` i dalje drži redoslijed. Checklistu ne dobiva svaka datoteka. Ostale granule pokriva smoke: otvaranje, master, stablo, detail i SQL na filteru, bez gumba i bez `ONSUBMIT`.

Izvor tipova je `tipovi-formi.md` na [#49](https://github.com/tomic04/netgen-u-carpo/pull/49) (`cursor/netgen-tipovi-formi-3266`). Manager ide samo kad XML ima lijevi panel, detail i gumbe. Šifrarnik nije zaseban tip. To je `samo-grid` s obrascem iz `Sifranik-obrazac.md`.

| Obrazac | U tipovima | Već obrađeno | Rijetko, još nije zatvoreno |
|---|---:|---|---|
| šifrarnik | unutar samo-grid | `DrzaveNG`, `ZupanijeNG`, `NacinOtpremeNG`, `CarinskaTarifaNG` | `OKStay` prazni polja, `NEXTFORM` nakon `OK` |
| manager | 137 | `MPNarPredis`, `MPPredispIz`, `MPIzduMPZaprimi` | `UPDATE_FORM` i `UPDATEFORM` u istoj granuli, `vrstadokid-`, prvi čvor s praznom naredbom |
| stablo-master | 74 | `Obrazac`, `OdobreniKaloNG`, `PDVPromjenaNEW` | store puni master preko `@@node`; `onlytree` na ljusci ne smije ponoviti master |
| samo-detail | 82 | `Intrastat`, `MPKase` | detail `@@id`, checkbox na detailu |
| samo-grid | 143 | `JeziciNG`, `KUPStatN` | širina stupca iz zadnje četvorke, skriven `ID` |
| dijalog | 168 | `MPEurParam`, `MPUvjetiKase`, `KarticaPartnera` | `NEXTFORM` nakon `OK`, `REFRESHCONTROL`, radio `1/0`, multidrop, `TRANSLATE` |

Sljedeća checklista ide samo ako mehanizam nije u ovom stupcu „već obrađeno“ i nije u `Otvoreno-za-implementaciju.md` označen kao zatvoren.
