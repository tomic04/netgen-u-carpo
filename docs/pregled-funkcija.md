# Pregled funkcija NetGen / Carpo

Prvi prolaz su funkcije koje obrađuju podatke: zamjena `@@`, `PokupiParametre`, SQL, validacija i format broja i datuma. Izvor je `netgen/`. U `netgen/*.vb` (bez designer datoteka) ima 476 `Sub`/`Function`. Ovaj dokument pokriva podatkovni sloj. Ostali moduli nisu u ovom prolazu.

Status: `isto` prati NetGen, `razlika` je R-broj koji ostaje, `nedostaje` još nema porta.

## Zamjena parametara

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Form1.SrediSQL` 2470–2503. `@@id` (tri veličine), `@@operateriid`, `@@detid`, `@@node`, `@@table`, `@@xmlname`, `@@jupitersrvpath`, `@@editedcolumn`, `@@lastvalue`, `@@selectedcolor` | `SqlTekst.Sredi` | isto | `sql-zamjena` |
| `Form2.SrediSQL2` 2876–2920. Isto osim `@@Id` i `@@table`. Dodaje `@@tempstring`, `@@dblclickedcolumn`, `@@VERSION`, `@@OPISNIPARAMETRI`, `@@FOCUSEDCONTROL`, `@@selectedfile`, `@@selectedpath` | `SqlTekst.Sredi2` | isto | `sredi2-temp`, `sredi2-id-nije-Id` |
| `@@GRIDROWCOUNT(` u `SrediSQL2` 2899–2905 je komentar. Živa petlja reže token na `(` i nepoznato postavi na `null` | `SrediPoslije.Zamijeni` | isto | `gridrowcount-komentar` |
| `Class1.SrediSQLPosljeNextForme` 740–853. Prvo `SrediSQL2`. Token `@@` staje na razmak, navodnik, `%`, `#`, zarez, `)`, `*`, `/`, `\`, `+`, `-`, `.`, `(`, `=`. `¸¸` postaje `''` | `SrediPoslije.Zamijeni` i `Petlja`. Master šalje `Petlja` nakon `Sredi`. Combo ide istim putem. Dijalog na OK zove `ZaSlanje` | isto | `sredi-poslije-roba`, `sredi-poslije-navodnik`, `intrastat-ostao` |
| `Class1.NadjiVrijednostuPrevParametrima` 856–977. Skine jedan `@`, prvi `@ime=` koji počinje tim prefiksom. `ajdijeva` mijenja `~` u zarez. Prazno je `null`. `@napomena` i `null` je prazan string. `@DAT` i grid-datum pakiraju datum. Inače se navodnici skidaju. Prazno ili `null` gleda clipboard | `SrediPoslije.Nadji` | isto | `sredi-poslije-roba`, `sredi-poslije-ajdijeva`, `sredi-poslije-datum`, `sredi-poslije-napomena`, `sredi-poslije-null`, `sredi-poslije-cb` |
| `Form2.SrediSQLsaKolonamaizGrida` 8440–8456 i `NadjiVrijednostuGridu` 8459–8490. `@(grid..kolona)`. Datum iz `dd.MM.yyyy`, broj mijenja zarez u točku, prazno je `null` | `SrediPoslije.Kolone`. Vrijednost daje pozivatelj | isto | `kolone-grid` |
| Ispis zaglavlja: `Form1` 6001 zove isti `SrediSQLPosljeNextForme` s `DatumjezaIspis`, pa `Rafalna_Priprema` i `Translate` | `IspisZaglavlje.Desno`. Petlja je ista granica tokena, clipboard je rječnik, `ISEMPTY` ide prije prijevoda | isto | `isempty-prazno`, `isempty-vrijednost`, `podnozje-isempty` |

`SETDEFAULTS` na podlozi puni clipboard prije `PokaziUpit` (`Form1.vb` 3862–3882). `SrediPoslije` prima taj rječnik, ali otvaranje mastera još ne izvrši `SETDEFAULTS` u clipboard. Zato `@@tkosmomi` na masteru postane `null`, a ne vrijednost iz `SETDEFAULTS`. To ostaje za sljedeći prolaz.

## Skupljanje parametara

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Form2.PokupiParametre` 3406–3906. Check je `CheckState`, radio 1/0, broj prazan je 0 i zarez je točka, tekst ima `¸¸` i `~~`, datum je `yyyyMMdd` ili `null`, search je id i `@searchbox`, grid je id, nizovi i `@rowcount` | `Dijalog.SviParametri` i `ParametriSvi` | isto | `param-radio`, `param-broj`, `param-search`, `FAK-07` |
| `Form2.Button1_Click` 3377–3388. Prvih 6 slova u `INSERTDELETEUPDATESELECT` ugradi `@@` i obriše niz parametara. Inače se niz lijepi, osim uz `!!` | `SrediPoslije.ZaSlanje`. OK na dijalogu šalje taj tekst | isto | `za-slanje-dml`, `za-slanje-sp`, `za-slanje-uzvicnik`, `dml-ne-lijepi`, `uzvicnik-gasi-lijepljenje` |
| `Form1.PokupiParametreSaGrida` 2544–2612. Stupac 0 na Janusu je ikona pa petlja kreće od 1. Zadnje 4 znamenke koje nisu 0 otpadaju. Razmak je `_`, `!` se briše. Broj mijenja zarez u točku. `DBNull` se preskače | `DetailGrid.ParametriIzReda`. `DataRow` nema ikonu, pa kreće od prvog stupca | isto | `parametri-grida` |
| `Form1.PokupiParametreSaHijerarHijskogGrida` 2615–2643. Ime je caption, bez skidanja 4 znamenke, od stupca 0 | Nema hijerarhijskog grida u portu | nedostaje |  |
| `Form1.GetID` 5566 i `GetDetID` 5605. Prazan grid je `0` | `DetailGrid.IdReda` i `SqlTekst.ZaFormu` | isto | `sql-zamjena` |
| `Form2.PokupiParametreIzTrazilice` 5926 | `ParametriSvi.Search` | isto | `param-search` |

## SQL i validacija

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| `Class1.ExecSql` 85–118. `~~` je zarez. Tekst koji počinje s `@` ili je prazan ili ima `@@` iza prvog znaka se ne šalje | `SqlTekst.Odluka`. Nakon `SrediPoslije` nepoznat `@@` više nije u tekstu, pa se šalje `null` | isto | `sql-prazno`, `sql-at`, `sql-atat-na-nuli-je-at`, `sql-atat-kasnije`, `sql-salji`, `intrastat-ostao` |
| `Class1.ExecSql` Catch 158–205. `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard` se gutaju. `#OK` / `+OK` brišu grešku. `#close` zatvara. `#FOCUS(` daje fokus | `Lanac.Izvrsi` i `MasterGrid.IzvrsiCitanje`. `spNetGenErrorLog` se ne zove; greška ide u Carpo logger | isto | `focusNeGuta` u `GranulaIzvrsenje`, `sql-greska-ne-puni` |
| `Class1.ExecSql2` 252. Upis u `spNetGenErrorLog`, timeout 100 | Nema. Logger je Carpo, ne CBLog i ne `spNetGenErrorLog` | razlika |  |
| `SqlTekst.JeZabranaPisanja`. Prva riječ `insert/update/delete/drop/alter/truncate/merge/create`. `sp...` nije zabrana | isto to | isto | `zastita-prva-rijec`, `zastita-zakljucivanje` |

## Broj i datum

| NetGen | Carpo | Status | Test |
|---|---|---|---|
| Prazan broj je `0`, zarez je točka. `Form2` 3601 | `ParametriSvi.Broj`, `Lanac.PrazanBroj` | isto | `param-broj`, `prazan-broj` |
| `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth` | `Ponasanje.DatumPocetni` | isto | `KON-20` |
| `SHOWCHECKBOX` bez `CHECKED` šalje `null`. Inače `yyyyMMdd` | `Ponasanje.DatumParametar`, `ParametriSvi.Datum` | isto | `DOB-02` |
| Tekst: apostrof je `¸¸`, zarez je `~~`, u SQL-u `¸¸` postaje `''` | `ParametriSvi.Tekst`, `Lanac.USql`, `SrediPoslije` | isto | `sredi-poslije-navodnik` |

## Što je portano u ovom prolazu

`SrediSQL2`, `SrediSQLPosljeNextForme`, `NadjiVrijednostuPrevParametrima` i `SrediSQLsaKolonamaizGrida` nisu imali Carpo tijelo. Sada su u `SqlTekst.Sredi2` i `SrediPoslije`. Master, combo i OK na dijalogu šalju taj tekst. Nepoznat `@@` na masteru više ne ostaje u SQL-u: `Intrastat` šalje `@godina = null`, kao `PokaziUpit` (`Form1.vb` 3882).

Ostaje: hijerarhijski grid (`PokupiParametreSaHijerarHijskogGrida`) i punjenje clipboarda iz `SETDEFAULTS` prije master SQL-a. `ExecSql2` ostaje jer piše u `spNetGenErrorLog`.
