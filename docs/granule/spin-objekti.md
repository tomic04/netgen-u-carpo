# Objekti na spinu

## Testni retci, nisu s klijenta

`scripts/spin/Meni.ps1` u `dbo.ssMeni.Parametar` stavlja `asm:NetGenDLL.loader:<datoteka>.xml`. To je oblik koji spin već ima (`PrijemniList.xml` u dokumentu 06). Jupiter iza druge dvotočke preda `ModuleID`. NetGen ga spremi u `Mid` (`Form1.vb` 1251–1258) i otvori `remotedllpath\SharedDLLs\xml\` + `Mid` (`1139`). Datoteke: `FakPar.xml` (`FAK-10`), `KntDospjeleStavke.xml` (`DOS-10`), `Kulture.xml` (`KUL-12`), `KUPDostKN.xml` (`KND-06`), `KUPDostKnNGBvg.xml` (`BVG-07`), `KUPNG.xml` (`KUP-05` i `KUC-07`), `MPNarIzdSkl.xml` (`NAR-09`), `MPNGPregDokMAIL.xml` (`MAIL-07`), `RatArkod.xml` (`RIJ-31`), `RobaWebKlase.xml` (`RIJ-33`). Nije klijentski red izbornika.

`scripts/spin/dospjele.sql` radi `DOS-11`. Combo i multidrop dobiju stupac 0 (vrijednost) i stupac 1 (natpis), `Form2.vb` 2430–2431 i 2503–2504. `spKntPopuniKontroleNG` prima `@Action` i `@id` kako piše u `KntDospjeleStavke.xml` (`konto`, `klpartneri`, `odgosobe`, `zupanije`, `mjesta`). `spOrgShemaQuery` prima `@action`. `spKntOtvStavkeQueryNG` prima parametre grida iz istog XML-a. Tijela su testni SELECT, ne klijentska procedura.

`scripts/spin/Prenesi.ps1` na sql-ai čita s jupiter2019 i jupiter2022 samo `sys.columns` i `sys.sql_modules`. Procedure se tamo ne pokreću i retci tablica se ne čitaju. Tijelo se kreira na spinu. Ako tekst ima INSERT, UPDATE, DELETE ili MERGE, izvor je označen s `pise`. Redoslijed: Biovega, pa ostale baze na jupiter2019, zatim jupiter2022. Izvor nakon runa je u `artifacts/spin/izvori.md`.

`scripts/spin/stubovi.sql` i `scripts/spin/Otvori.ps1` rade samo na `localhost` / baza `spin`. Ako otvaranje javi da procedura ne postoji, skripta kreira prazan `SELECT ... WHERE 1 = 0` s parametrima iz poziva. Ako javi nepoznat stupac u jednostavnom `FROM`, doda `varchar(50) NULL`. Stub nastaje samo kad `Prenesi.ps1` nije našao tijelo. Popis kreiranog ide u `artifacts/spin/stubovi-kreirani.md`, ishod u `artifacts/spin/otvaranje.md`.

`scripts/spin/Retci.ps1` na spinu složi `dbo.klmasterroba` iz `sys.columns` (isti serveri, bez čitanja redaka). Ako definicije nema, stub su stupci iz `OznakaKL` SELECT-a: `klmasterrobaid`, `sortkey`, `Naziv`, `oznaka`, `oznaka2`. Ako je tablica prazna, ubaci jedan red (`Naziv = NetGen test`, `sortkey = 01`). Postojeći retci se ne diraju. NetGen stavlja fokus na grid kad `RowCount > 0` (`Form1.vb` 2059–2072). Izvještaj je `artifacts/spin/retci.md`. `OKL-58` time dobiva uvjet za `fokus=grid`; stavka nije zatvorena dok test na sql-ai ne vrati taj ključ.

To nisu prava poslovna tijela. Da test bude vjerodostojan, korisnik treba dati:

| Objekt | Poziv iz XML-a | Što stub ne zna |
|---|---|---|
| `klmasterroba.oznaka2` | `OznakaKL` SELECT | pravi tip i duljina; stub je `varchar(50) NULL` |
| `spKultureNGQuery` | `@Action=1` i `@Action=2, @KultureId` | stupci i filteri mastera |
| `spFakPartneriUvjetiQuery` | bez parametara, i `@action=2` | stupci uvjeta partnera |
| `spKUPPrometQueryN` | `@action=4,@operateriid=7,@dostavnaknjigaid=0,@aktivni=1` | ako na spinu fali, stub je prazan SELECT |
| `spStatusiQuery` | `@action=0, @node=0` | isto |
| `spBLZakljucakQueryNG` | `@action=0` | isto |

`AOPNovi` šalje `@id='null'`. Konverzija u int ostaje, kao u NetGenu. Stub se za to ne pravi.

`Intrastat` ONLOAD je `spIntrastat_Query ... @godina = @@godina`. `Form1.SrediSQL` ne dira `@@godina`. `Class1.ExecSql` ne šalje tekst u kojem `IndexOf("@@") > 0` (`Class1.vb` 113–118). Otvaranje na spinu taj slučaj bilježi kao `preskok`, ne kao pad konverzije. Procedura se ne izmišlja.

`PrijemniList` nema projektni `ONLOAD`. `Poruke`, `TecRazOTV`, `RealizacijaFakture` i `RealizacijaStavkeNG` nemaju XML u repou.

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
