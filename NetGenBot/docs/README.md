# NetGen Bot

Znanje za ljude koji i dalje pišu i dorađuju **NetGen XML**. Nije vodič za konverziju u Carpo i nije uputa za izmjenu `NetGenDLL`.

## Izvor

| | |
|---|---|
| Kod | `netgen/` (klon `NetGen2026`) |
| Commit | `b0d5d41` (9. 9. 2026., grana `master`) |
| Održavanje DLL-a | `netgen/CLAUDE.md` — ne dirati VB radi ovog bota |

Brojevi linija vrijede za taj commit. Ako se klon osvježi, linije treba ponovo provjeriti u kodu.

## Redoslijed čitanja

1. **`NetGen-XML-Pravila.md`** — što autor smije napisati.
2. Po pitanju, jedan od kataloga:
   - **`NetGen-Komande.md`** — `Rafalno`: što radi, na podlozi ili na dijalogu, i što je mrtvo.
   - **`NetGen-Kontrole.md`** — `TYPE`, atributi i što ide u `@ime`.
   - **`NetGen-Tok.md`** — učitavanje XML-a, `ONLOAD`, submit, `NEXTFORM`, prava, clipboard.
3. **`NetGen-Engine.md`** — samo kad treba `fajl:linija` za nešto što katalog već tvrdi. Ne čitaj ga umjesto kataloga.
4. **`NetGen-Cheatsheet.md`** — kratki obrasci.

VB (`netgen/*.vb`) otvori tek kad pojam **nije** u katalogu. Tada traži `GetNamedItem("IME")` ili ime komande u `Rafalno`. Stari popis od 527 pravila nije u ovom paketu. Nije specifikacija. Specifikacija je katalog u `docs/`.

## Mapa: XML pojam → VB

| Pojam | Fajl | Što autor od toga ima |
|---|---|---|
| Ulaz granule, putanja XML-a | `Form1.vb` ~1139, `Form2.vb` ~247, `Run.vb` ~26 | `remotedllpath\SharedDLLs\xml\` + ime XML-a |
| `<PROJECT>`, ONLOAD, gumbi, tree, master grid | `Form1.vb` | Podloga granule |
| Dijalog (FILTER, INSERT, UPDATE, …) | `Form2.vb` | Forme unutar istog XML-a |
| `@@` na masteru | `Form1.SrediSQL` ~2470 | Zamjena prije SQL-a na podlozi |
| `@@` na dijalogu + ostale kontrole | `Form2.SrediSQL2` ~2876 | Zamjena na formi |
| Parametri `@@imeKontrole` i clipboard | `Class1.SrediSQLPosljeNextForme` ~740, `NadjiVrijednostuPrevParametrima` ~856 | Vrijednosti polja |
| Izvršavanje SQL-a, preskok `@@`, poruke `#` / `+` | `Class1.ExecSql` ~85 | Svaki upit |
| Janus grid kolone | `Grid.vb` `Formatiraj` ~57 | `<COLUMN>` na masteru i u formi |
| DevExpress grid (`DXGRID="true"`) | `Class1.CustomFormatKolonaDX` ~1845 | Isti `<COLUMN>`, plus par DX atributa |
| Tražilica | `Form3.Trazilica` ~71, `SearchBox.vb` | `TYPE="search"` |
| Ispis | `Form1.NovaGenerickaForma2` ~4945, `Form6.vb`, `Form9.vb` | `PRINTMASTER`, REPX |
| Ctrl+F11 log | `Form5.vb`, `Class1.NapuniCBLog` | Dijagnostika, nije XML |
| Form4 | cijeli fajl je komentar | Autor ga ne koristi |
| Form7 | Ctrl+F9, izbor jezika | Nije XML tip |
| Form8 | prazna klasa | Ništa |

Kontrole koje engine instancira iz `TYPE`: `Combo.vb`, `SearchBox.vb`, `teksT.vb`, `DTPicker.vb`, `NumerBox.vb`, `Buton.vb`, `mojCheck.vb`, `mojRadio.vb`, `Grid.vb`, `GridDX.vb`, `treeViewMoj.vb`.

## Gdje ide nova granula

Jedan XML, ime granule: `granule/Nove/ImeGranule.xml`. Primjeri tuđih granula su u `granule/Biovega`, `granule/Agronom` i `granule/Nevkos`. Ne prepisuj primjer osim ako korisnik to traži. Gotov fajl ide u Jupiter `SharedDLLs\xml\`.

## Što bot ne radi

- Ne predlaže Carpo XML, osim ako korisnik to izričito traži.
- Ne mijenja `netgen/*.vb`.
- Ne tretira primjere u `granule/` kao specifikaciju. Specifikacija je katalog u `docs/`, a ponašanje je kod u `netgen/`.
