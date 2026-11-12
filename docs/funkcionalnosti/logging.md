# Logiranje

Carpo.NetGen ne vodi vlastiti log i ne otvara NetGenov `PokaziCBLog`. Sve što je NetGen pisao u `CBLog` ide u postojeći Carpo logger, onaj koji se u Jupiteru otvara s **Ctrl+F11**.

## Kako Carpo logira

`Carpo.Core.Logger` (`Devex_26/Carpo.Core/Log/Logger.cs`) drži `LogList`. Ctrl+F11 na formi (`JupiterShared_KeyDown`, `JupitetShared.cs` 299–321) otvara `Carpo.Core.Log.Log` nad tim istim objektom.

| Poziv | Što Carpo s njim radi | Ide u Ctrl+F11 |
|---|---|---|
| `InfoData(opis, tekst)` | SQL i čitanja (`Logic/Data/Core.cs` `LogData`) | da, `WriteLog` |
| `Info(opis, tekst)` | događaj | da |
| `Error(opis, tekst)` | greška, plus `spCarpoLogInsert` | da |
| `Warn` | upozorenje, plus insert | da |
| `Performance` | JupiterLogger | ne, namjerno mimo liste |

`WriteLog` uz listu zrcali zapis u tekstualni log i u JupiterLogger. To dolazi samo od sebe čim se zove `Logger`.

## Kako je NetGen logirao

`Class1.NapuniCBLog` (`Class1.vb` 76–82) gura jedan red u prsten `CBLog` od 40 mjesta, s vremenom. `PokaziCBLog` (57) to pokaže u vlastitom tekstu. Isti poziv pokriva SQL (`ExecSql` 138), preskok (`SKIPPED`, 106 i 115), grešku (`227–228`) i akcije (assembly, fiskal, stablo). Posebno se ne logiraju tihe greške `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard`.

`CB(400, 1)` nije log. To je međuspremnik parametara (`SETDEFAULTS`). Ostaje podatak.

## Pravilo za svaki sljedeći element

Jedini ulaz je `Carpo.NetGen.Log.NgDnevnik`. Novi kod ne zove `NapuniCBLog`, ne pravi novi prozor za log i ne piše u svoju listu mimo ovog.

| NetGen | NgDnevnik | Carpo metoda | Opis u listi |
|---|---|---|---|
| SQL koji se šalje | `Sql` | `InfoData` | `NetGen SQL` |
| `SKIPPED` | `Preskok` | `Info` | `NetGen preskok` |
| akcija (forma, assembly, stablo) | `Akcija` | `Info` | `NetGen` |
| greška koja nije tiha | `Greska` | `Error` | `NetGen SQL` |

Host (TestShell, kasnije forma) zove `NgDnevnik.Spoji(logger)` s instancom `Carpo.Core.Logger` te granule. `LogLevel` mora biti barem `Info`, inače `InfoData` odmah izađe. Forma na Ctrl+F11 otvara `new Carpo.Core.Log.Log(logger)`, isti poziv kao Carpo.

Grid to već radi: preskok i zabrana pisanja u `MasterGrid.Ucitaj`, poslani SQL u `IzvrsiCitanje`, greška koja se pokaže korisniku u `ZabiljeziGresku`. Tiha SQL greška ostaje bez reda, kao u `ExecSql`.

## Otvoreno

- NetGen forma još ne postoji, pa Ctrl+F11 nema kome stići. Kad se prozor nacrta, `KeyDown` mora otvoriti `Carpo.Core.Log.Log` s loggerom koji je spojen na `NgDnevnik`. TestShell logger spaja, ali nema taj prozor.
- `spNetGenErrorLog` se ne zove. Carpo greška ide u `LogList` i, preko `Logger.Error`, u `spCarpoLogInsert`. To nije isti zapis kao NetGenova tablica.
- `Logger.Error` pri `LogLevelUser` 3 ili 4 još otvori Carpo poruku. NetGen je uz log imao i svoj `MsgBox`. Dok forma ne odluči tko prikazuje poruku, oba se ne smiju slagati u dva dijaloga.
- Prsten od 40 redova se ne preslikava. Ctrl+F11 vidi cijeli `LogList` sesije.
