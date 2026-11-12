# NetGen tok granule i forme

Izvor: `Form1.vb` učitavanje i podloga, `Form2.vb` dijalog. Commit `b0d5d41`. Ovo je redoslijed, ne popis atributa. Atributi su u `NetGen-XML-Pravila.md`, komande u `NetGen-Komande.md`, kontrole u `NetGen-Kontrole.md`.

## 1. Odakle se XML učita

Putanja je `remotedllpath\SharedDLLs\xml\` + ime granule (`Form1.vb` 1139).

`ProcitajXMLzaPodlogu` (`Form1.vb` 3167–3268), istim redom:

1. `SpinDMSUtil.NetGenFinder.GetFile` (3172–3182). Ako assembly ne postoji, greška ide samo u Ctrl+F11.
2. Ako je dokument prazan i konekcija `Conn4` je otvorena: `select body from netgenxml where xml = 'ime.xml'` (3210). `body` je kriptiran (`tan123ge`).
3. Ako ni baza ne da XML: `m_xmld.Load` s diska (3262).
4. Ako ni to: ugrađeni test resurs (3278).

Ime za bazu je fajl bez putanje. Kosa crta `/` se prije toga pretvori u `\`.

Dijalog čita **isti** XML, čvor `/PROJECT/ImeForme`, ne poseban fajl. `INCLUDE` na tom čvoru (`Form2.vb` 844–943) ubaci `/PROJECT` iz drugog XML-a: prvo DMS, pa fajl.

## 2. Podloga, redoslijed

`Form1_Load` otvori konekcije, napuni meni granula, pa `ProcitajXMLzaPodlogu`.

Unutar toga, nakon čitanja atributa:

1. Naslov iz `/PROJECT/CAPTION`.
2. `BUTTONS` prepiše `CMDxCAPTION` ako nije prazan.
3. `TREE` napuni drvo (`NapuniDrvoVrijeme`, poziv 3871).
4. `ONLOAD`: počinje li s `:`, otvori formu; inače jedan SQL puni master (`3879–3883`).
5. `ONLOAD2`: jedan red, podešava gumbe (`3906`).
6. `DETAILGRID` puni donji grid. Ako počinje s `:`, poruka `sorkac!` i nema forme.
7. Ako je Jupiter proslijedio `strNGParams`, i to ide u `NovaGenerickaForma`.

Svaka promjena reda na masteru ponovo zove detail (`GridEX1.SelectionChanged`).

`@@id` je kolona `id` tekućeg reda, inače treća ćelija, inače `0`. Na DX gridu prva kolona ako nema `id`.

## 3. Dijalog, redoslijed

`NovaGenerickaForma2` / `NovaGenerickaForma1` postavi ime forme i pozove `ProcitajXML`.

1. Atributi forme (`ONSUBMIT` prepiše `STORA`, 1036–1039).
2. Ako ime sadrži `filter` i `DBCLIPBOARD` nije `false`, clipboard se uključuje (1102).
3. `ONLOAD` se izvrši **prije** crtanja kontrola (1195). Prvi red: svaka kolona ide u CB kao `imeKolone=vrijednost` i u `prevParamizStartupStore`. Greška na `ONLOAD` prekida formu (`GreskaKodIzvrsavanjaSQLa`).
4. `ONLOAD2` u `Trs3`.
5. Petlja `INPUT`. Za svaku kontrolu, ako `Trs2` ima kolonu = `NAME`, upiše se vrijednost. Zatim `ONLOAD2` zove `DodatnoPostimajKontrolu` s tom kolonom.
6. Ako forma nema nijednu kontrolu, odmah `Rafalno(ONLOADED)` i ne prikazuje se dijalog (`OKClick=true`, 4430). Inače `ShowDialog`, pa `ONLOADED` u `Form2_Load`.

`DodatnoPostimajKontrolu` (`4482`) razumije riječi, ne samo tekst:

| Vrijednost kolone | Efekt |
|---|---|
| `visible` / `invisible` | Kontrola i njen `lbl` |
| `enable` / `disable` | Enabled. Na gridu to je `AllowEdit`, ne Enabled |
| `tabstoped` / `tabnotstoped` | Tab stop |
| `refresh` | Osvježi WebBrowser |
| bilo koji drugi tekst | `Text` kontrole. Na gridu traži taj id u koloni 2 i skroluje do reda. Na WebBrowseru `Navigate` |

Search, button, text, combo, radio i check u grani `Else` **ne** dobiju taj proizvoljni tekst (4534–4536). Njih `ONLOAD` puni ranije, direktno iz kolone.

## 4. Submit

Gumb 1 (`CMD1CAPTION`) pokupi parametre i izvrši `ONSUBMIT` + niz `@ime=vrijednost`.

Gumb 2 zatvara bez toga.

`OK` u lancu je isto što i gumb 1. `OKSTAY` spremi i ostane na formi.

`NEXTFORM` (`4431`): dok je OK i `NEXTFORM` nije prazan, izvrši `ONSUBMIT`, uzme prvu kolonu prvog reda kao novi id i otvori sljedeću formu. Parametri prethodne forme idu dalje.

Forma se smije zvati `IME(@p=@@id,-freezid)`. `-freezid` (`4413`) znači: kolona `id` iz `RefreshControls` ne prepisuje `@@id`, nego se pamti u CB kao `ImeForme_ID`. U XML-u se čita kao `@@imeforme_id`.

`FILLEXPLORER="TRUE"` na formi: nakon OK-a podloga ponovo učita master SQL-om te forme (`Form1` oko 5445). Istu riječ može vratiti i kolona `fillexplorer` iz SQL-a.

## 5. SQL koji puni kontrole usred forme

`RefreshControls` (`4556`) je ono što se desi kad `ONLOSTFOCUS`, `ONCLICK` ili neprepoznati segment lanca nisu posebna komanda.

Redoslijed: pokupi parametre, zamijeni `@@`, izvrši SQL. Posebne kolone prvog reda su u `NetGen-Komande.md` (tablica na kraju): `focus`, `form`, `id`, `temp`, `beep`, `click`, `formcaption`, `formwidth`, `formheight`, `fillexplorer`, `findnode`.

`++` u tom SQL-u zamijeni se trenutnim nizom parametara (4564).

`*` kao SQL znači `ONLOSTFOCUS` same forme (4557).

Greška u tom SQL-u prekida lanac. `#close` u poruci zatvara formu (`ZelimIzaciIzForme`, 7512).

## 6. Parametri i clipboard

`PokupiParametre` složi `@ime=vrijednost` za svaku kontrolu koja smije. To je `strParametri` (za `ONSUBMIT`) i `strParametriSvi` (za `@@` u sljedećem SQL-u).

CB je niz u memoriji (`cls1.CB`), ključ `ime=`, vrijednost bez navodnika. `ONLOAD` ga puni. `SrediSQLPosljeNextForme` ga čita ako parametar nije nađen u nizu forme. Traje dok je proces živ. Nije tablica u bazi.

`DBCLIPBOARD` / filter: vrijednosti se spremaju po ključu `imeXml/imeKontrole` i vrate se sljedeći put. Na formi čije ime sadrži `filter` to je uključeno osim `DBCLIPBOARD="false"`.

`SETDEFAULTS` na PROJECT-u upisuje početne vrijednosti u CB (`Form1.SetDefolte`, 3945). `FILTERSAVE` pamti filter granule.

Na masteru parametri tekućeg reda nisu `@grid…`. `PokupiParametreSaGrida` (`Form1.vb` 2544) radi `@ImeKolone='vrijednost'` iz DataMember-a. Zato u `ONLOAD` mastera pišeš `@@broj` ako SQL vrati kolonu `broj`. Numerička vrijednost i tu ide u navodnicima, sa točkom umjesto zareza.

## 7. Prava

`PERMISIO="true"` na PROJECT-u učita prava (`ssPermissionProcitaj`, `Form1.vb` 3414).

Prije otvaranja forme i prije stavke izbornika engine zove `ssPermissionProvjeriRafalnuKomandu`. Ako ne smije: u log `ZABRANA : PERMISSIO DENIED` i izlaz, bez poruke korisniku (`Form1.vb` 4939, `Form2.vb` 4423).

Isti mehanizam gasi stavke desnog klika (spremi layout, export, ispis) ako pravo nedostaje.

## 8. Šta agent treba zaključiti bez koda

- Prazan grid i `SKIPPED` u Ctrl+F11: u SQL-u je ostao `@@` ili SQL počinje s `@`.
- Kontrola prazna iako `ONLOAD` vraća podatak: ime kolone nije `NAME` kontrole.
- Gumb na podlozi s `FOREACHROW` ne radi: ta komanda je živa samo na dijalogu.
- `REFRESHCONTROL` na podlozi ne osvježi ništa.
- Combo pokazuje krivi par: kolona 0 nije id.
- Filter pamti staru vrijednost: ime forme sadrži `filter`.
- `@@id` na dijalogu je id mastera, osim `-freezid`, kad je novi id u `@@imeforme_id`.
