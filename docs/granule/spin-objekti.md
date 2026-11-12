# Objekti na spinu

Stubovi u `scripts/spin/stubovi.sql` samo otvaraju granulu. Ne pišu. Pravo tijelo treba doći od korisnika. Jupiter se ne dira.

## Već nadomješteno praznim objektom

| Objekt | Zašto | Što stub radi | Što treba pravo |
|---|---|---|---|
| `dbo.klmasterroba.oznaka2` | `OznakaKL` SELECT pada s `Invalid column name 'oznaka2'` | `varchar(50) NULL` ako stupca nema | stupac kakav je u produkcijskoj shemi (tip i duljina) |
| `dbo.spKultureNGQuery` | `Could not find stored procedure` | prazan SELECT `KultureId, Naziv, Ozn, Cijena` | tijelo za `@Action=1` (master) i `@Action=2, @KultureId` |
| `dbo.spFakPartneriUvjetiQuery` | ista poruka na `FakPar` | prazan SELECT `FakPartneriUvjetiId, partneriid, Naziv` | tijelo bez parametara (master) i `@action=2, @FakPartneriUvjetiId` |

## Namjerno ostaje greška

| Granula | SQL koji port šalje | Zašto se ne dira |
|---|---|---|
| `AOPNovi` | `spKntAOPOBRASCIQuery @action='master_grid',@id='null'` | NetGen šalje tekst `null` u navodnicima. Konverzija u int je očekivana. |

## Otvaranje na sql-ai provjerava i ovo

`scripts/spin/Otvori.ps1` šalje master `KUPNG` (`spKUPPrometQueryN @action=4,@operateriid=7,@dostavnaknjigaid=0,@aktivni=1`), `Statusi` (`spStatusiQuery`), `KntBlaZak` (`spBLZakljucakQueryNG`). Ako procedura postoji, u izvještaju je `otvoreno`. Ako ne postoji, poruka ostane u `artifacts/spin/otvaranje.md` i procedura se ne izmišlja drugim tijelom.

`PrijemniList` na projektu nema `ONLOAD`. Master dolazi s klika na stablo, ne s otvaranja.

XML kojih nema u repou (`TecRazOTV`, `MER\RealizacijaFakture`, `MER\RealizacijaStavkeNG`, `Poruke`) se ne dodaje.
