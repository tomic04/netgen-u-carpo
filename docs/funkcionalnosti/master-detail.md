# Master-detail

Donji grid je `GridEX2`. SQL je atribut `DETAILGRID`. Nakon što se master napuni, `PokaziUpit` zove `PokaziUpitDetalj` (`Form1.vb` 2104). Svaka promjena odabira mastera ponovo zove isti predložak (`GridEX1_SelectionChanged`, 7413), ako master ima redova.

U korpusu `definitions/netgen` + prepisane granule, `DETAILGRID` je na oko 300 XML-ova. `TREE` i `DETAILGRID` zajedno ima 119 datoteka samo u `definitions/netgen`. Drugi donji grid ne postoji. `SQL2`, `SQL3` i `SQL4` na `<PROJECT>` u korpusu nisu nađeni; u kodu (`1950–1978`) to su dodatne tablice hijerarhijskog mastera kad je relacija `multi`, ne drugi `GridEX2`. Ako atribut ipak postoji, isti `@@` prolaz složi i njihov SQL.

## Punjenje

1. Prazan `DETAILGRID`: SQL se ne šalje, donji splitter ide na visinu forme minus 50 (`2274–2276`, `2111–2113`).
2. Vodeće `:`: SQL se ne šalje. NetGen tu kaže `sorkac` (`2105`). Zapis ide u log.
3. Inače `GetID` mastera (kolona `id`, inače prvi podatak, Janus ćelija 2), pa `PokupiParametreSaGrida` (`2544`): svaki stupac tekućeg reda postaje `@ime='vrijednost'`. Zadnja 4 znamenke imena, ako nisu 0, otpadaju. Razmak je `_`, `!` se briše. Broj s zarezom ostaje s točkom.
4. `SrediSQL` mijenja `@@id`, `@@node` (zadano `null` dok čvor nije odabran), `@@operateriid`, `@@detid`.
5. Preostali `@@ime` ide kroz `SrediSQLPosljeNextForme`: vrijednost je desna strana `@ime=...`, uključujući navodnike.
6. Upis (`insert`/`update`/…) se ne šalje.
7. Greška SQL-a: poruka, detail ostaje prazan, isto kao `ExecSql` Catch. Log je `NgDnevnik`.

`DETCHECKBOXVISIBLE=FALSE` skriva selector. `DETTOTALROWVISIBLE=FALSE` gasi total. `DETMOVELAST=true` fokusira zadnji red detaila. `DETAILCOLUMNS` nose `WIDTH` / `CAPTION` / `VISIBLE` / `CLEARCOLUMS` istim pravilima kao master, uključujući dijeljenje s 14.

## Defaulti GridEX2

Dizajner `Form1.designer.vb` 683–716. Isto što i master, postavljeno eksplicitno na donjem gridu prozora: višestruki odabir cijelog reda, `HideSelection` Highlight, obrub None, font MS Sans Serif 8.25, zaglavlje visine tri retka, bez dodavanja reda, ćelije se ne uređuju.

## Stablo, dopuna

Klik: `Node` je tag do prvog `:`, naredba je ostatak. Ako master SQL sadrži `@@node`, šalje se ponovo. Ako naredba nije SQL, traži se forma tog imena i otvara dijalog (naslov `CAPTION`, `ONLOAD` koji je `select` puni polja). Puni `Form2` (gumbi, `ONSUBMIT`, lijepljenje) još nije tu.

`TREETRANSLATE`: statični natpis uvijek prolazi `Translate` (`4353`). Natpis iz store samo kad je atribut `true` (`4461`). `Translate` (`Module1.vb` 266) vrati isti tekst kad jezik nije postavljen ili je `hrv`. Drugi jezik koristi rječnik ako je dan; inače tekst ostaje i u log ide da riječi nema. Jupiterov `JupiterPrevoditelj` nije spojen.

Ikona: `TreeView2.ImageList` je `ImageList2`, indeks 0 i odabrani indeks 0 (`designer` 575–579, ključ `OrdersDetails16.ico`). Prozor stavlja listu od jedne slike 16×16 i indeks 0. Izvorni `.ico` nije u repou.

## Log

SQL detaila, greška, otvaranje forme i prijevod idu kroz `NgDnevnik` u Carpo logger. Ctrl+F11 na podlozi otvara taj log.
