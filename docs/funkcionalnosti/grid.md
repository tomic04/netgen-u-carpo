# Grid (master podloge)

Prvi element. Stablo i veza master-detail nisu ovdje: `DETAILGRID` se čita u parseru, ali se donji grid ne puni. Dijalog `INPUT TYPE="grid"` (2077 pojava) dijeli isti jezik kolona; host dijaloga nije ovaj korak.

Korpus: 633 XML-a (`definitions/netgen` + `definitions/Prepisane_granule/Netgen`, uključujući `*.XML`). Jedan nije granula (`rptOpzStat1.xml`, korijen `report`). `DXGRID` se u korpusu ne pojavljuje, pa živi put je Janus `GridEX` (`Form1.PokaziUpit` grana bez DX-a), ne `NapuniDXGrid`. Carpo crta DevExpress 26, a ponašanje kopira Janus put. DevExpress defaulti se ne prepuštaju: vidi odjeljak Defaulti.

## Atributi projekta koje grid čita

Izvor čitanja: `netgen/Form1.vb` 3350–3478 i 2036–2178. Broj je pojava atributa, ne datoteka, osim gdje piše files.

| Atribut | Pojave | Datoteka | Primjer | Što kod radi |
|---|---|---|---|---|
| `ONLOAD` | 502 | 502 | `MPUvjetiKase.xml` `:FILTERFORM` | Ako postoji, prepiše `STARTUPSTORA` (`3367–3368`). Počinje li s `:`, nije SQL (`3879`) |
| `STARTUPSTORA` | 2 | 2 | `OznakaKL.xml` `select klmasterrobaid...` | Vrijedi samo dok nema `ONLOAD` |
| `CHECKBOXVISIBLE` | 574 | 574 | `TRUE` / `FALSE` | Točno `FALSE` gasi stupac 1 (selector). Inače ostaje (`2043`, `3094`, `3459`) |
| `TOTALROWVISIBLE` | 450 | 450 | `FALSE` | Točno `FALSE` gasi total. Inače uključen (`2049`) |
| `GROUPBYBOXVISIBLE` | 399 | 399 | `FALSE` | Postavlja `GroupByBoxVisible`. Ako atributa nema, ostaje Janus default (`3350`) |
| `FROZENCOLUMNS` | 297 | 297 | `0` | `FrozenColumns` (`3362`) |
| `AUTOMATICSORT` | 7 | 7 | `MPDanDostaveSkt.xml` `FALSE` | Samo `false` gasi sort klikom. Inače se ne dira (`3401`) |
| `EDITCOLUMNS` | 44 | 44 | `[Zaštitno_sredstvo1600]` | `Contains` natpisa prije brisanja `!` (`Class1.vb` 1129). Ako počinje s `sp`/`select`, prvo se izvrši i uzme se ćelija (0,0) (`3468`) |
| `ONCELLEDITED` | 41 | 41 | SQL | Čuva se. Izvršenje na izmjeni ćelije nije dio ovog koraka (nema još editora retka) |
| `BEFORECELLEDITED` | 1 | 1 | prazan | Isto, samo se pročita |
| `FILTERMODE` | 26 | 26 | `TRUE` | `FilterMode.Automatic`. Ako nema `EDITCOLUMNS`, `AllowEdit=false` (`2169`) |
| `ENTERKEYBEHAVIOR` | 12 | 12 | `1` | Ako nema edit stupaca, prvo `None`, zatim atribut prepiše ako postoji (`3419–3423`) |
| `COMMANDTIMEOUT` | 222 | 222 | `600` | Inače `30` (`Module1.vb` 177) |
| `MULTIGRIDLAYOUT` | 42 | 42 | `TRUE` | Oba kraka i dalje zovu `CreateGridColumns` (`1985`) |
| `TRANSLATECOLUMNS` | 2 | 2 | `StatusKorak` | `PrevediSadrzajGrida2` (`2188`). Prijevodni DLL nije u M0/M1; tekst ostaje |
| `MASTERGRIDLAYOUTFILE` | 3 | 3 | `RekapitulacijaUI.xml` | Layout s diska se ne učitava (nema korisničkog profila u CI) |
| `GRIDALLOWEDIT` | 1 | 1 | `OznakaKL.xml` `TRUE` | Kod ga ne čita. Edit se ne pali |
| `GRIDALLOWEDITCOLUMNS` | 1 | 1 | `oznaka2,oznaka` | Isto, ne čita se |
| `DXGRID` | 0 | 0 | | Grana postoji (`1861`) i nije u korpusu. Ne koristimo je |
| `GROUPBYBOXDETVISIBLE` | 0 | 0 | | Čita se za detail. Detail grid je sljedeći element |
| `AUTOMATICSORTDET` | 0 | 0 | | Isto |
| `MASTERMOVELAST` | 0 | 0 | | Kod: `true` ide na zadnji red (`2091`). U korpusu nema, test ga i dalje pokriva |
| `RELATION` | 0 | 0 | | `self` / `multi` u `PokaziUpit`. Nema u korpusu |
| `DETAILGRID` | 304 | 304 | `spMPKaseOper @action=2, @trgovinaid=@@id` | Nije ovaj element. Veza master-detail je sljedeći PR |

`DETCHECKBOXVISIBLE` 408, `DETTOTALROWVISIBLE` 431, `EDITCOLUMNSDET` 17, `ONCELLEDITEDDET` 17, `FILTERMODEDET` 3, `BEFORECELLEDITEDDET` 0, `DETMOVELAST` 0, `TRANSLATEDETCOLUMNS` 0: čitaju se, vrijede za detail grid.

## Stupci `MASTERCOLUMNS` / `COLUMN`

Grupe: `MASTERCOLUMNS` 117, `DETAILCOLUMNS` 52. Isti formatter (`CustomFormatKolona`, `Form1.vb` 7451). Nedostajući atribut ostaje prazan (`On Error Resume Next`).

| Atribut grupe | Pojave | Primjer |
|---|---|---|
| `CLEARCOLUMS` | 74 master + 37 detail | `false` u `MER_ZahtjevZaNabavkuNG.xml`. `true` sakrije sve, pa navedene vrati |
| `HEADERALIGNMENT` | 74 + 37 | `1` |
| `CLEARCOLUMNS` (ispravno N) | ne čita se | |
| `FILLEXPLORER` na `MASTERCOLUMNS` | 2 | ne čita ga formatter kolona |
| `NAME` na grupi | 2 | ne čita ga formatter |

| Atribut `COLUMN` | Pojave | Datoteka | Primjer | Kod |
|---|---|---|---|---|
| `NAME` | 1661 | 140 | `KOLIČINA0900` | traži se ako je `INDEX` 0 |
| `INDEX` | 1660 | 139 | `9` | `> 0` bira stupac po indeksu |
| `WIDTH` | 1648 | 133 | `1000` | `/ 14` ako je `> 0` |
| `FORMAT` | 1628 | 133 | `#,###0.###0` | `FormatString` |
| `CAPTION` | 1620 | 132 | | prazan ne dira natpis |
| `VISIBLE` | 1598 | 123 | | prazan ne dira |
| `AGREGATEFUNCTION` (jedno G) | 1106 | 80 | `0` | veže se samo ako nije prazan (`7506`) |
| `AGGREGATEFUNCTION` (dva G) | 291 | 29 | `""` u `JeziciNG.xml` | ne čita se |
| `FORECOLOR` / `BACKCOLOR` | 174 / 174 | 17 | | samo imena boja iz `7567–7638` |
| `POSITION` | 115 | 5 | `-1` | |
| `STYLE` | 11 | 2 | `1` | formatter ga ne čita |
| `ALIGN`, `AUTOSIZE`, `MAXLINES`, `NEGATIVERED`, `NOTZERORED` | 0 | 0 | | kod ih čita; korpus ih nema. Test i dalje prolazi kroz granu |

## Ponašanje (redoslijed)

`Form1` nakon XML-a (`3862–3883`), zatim `PokaziUpit` (`1825`):

1. Efektivni SQL: `STARTUPSTORA`, pa `ONLOAD` ako atribut postoji.
2. Ako počinje s `:`, `NovaGenerickaForma` bez dvotočke. Grid se ne puni.
3. Inače `SrediSQL`, to se zapamti kao zadnji SQL, pa još jedan `SrediSQL`, pa `ExecSql`.
4. `~~` postaje zarez prije preskoka (`Class1.vb` 87).
5. Preskok: trim počinje s `@`, ili je točno prazan, ili `IndexOf("@@") > 0` (`104–118`). `@@` na poziciji 0 ne hvata drugi uvjet, ali ga hvata `@`.
6. Zabrana u ovom koraku: `insert/update/delete/drop/alter/truncate/merge/create` se ne šalje (`spin` je samo čitanje).
7. Stupci iz podataka: ikona (skrivena, širina 22), selector (širina 22), zatim podaci. `int` 34 i `##0`, `decimal`/`double` 44 i `#,##0.00`, `string` 200, `date` 100 i `dd.MM.yyyy`. Zadnja 4 znamenke natpisa su širina `/ 14`. `;` je novi red. Ime koje završava na `ID` se skriva. `!` se briše prije provjere `sum!`, pa `sum!` u praksi ne veže agregat (`Class1.vb` 1107 pa 1116).
8. `CustomFormatKolona` preko toga. `CLEARCOLUMS=true` prvo sve sakrije.
9. Fokus: nema redova → id `0`. Inače prvi red, id iz stupca `id` ili iz prvog podatka (Janus ćelija indeks 2, iza ikone i selectora, `GetID` 5566). `MASTERMOVELAST=true` ide na zadnji.
10. `REFRESHMASTERGRID` ponovo šalje zapamćeni tekst, ne izvorni predložak s `@@` (`4570`).

`@@id` / `@@operateriid` / `@@node` (zadano tekst `null`, `Form1.vb` 39) zamjenjuju se u `SrediSQL` (`2482`).

## Defaulti

Svježi `new GridEX()` i `new GridView()` mjeri `Carpo.NetGen.Defaults` na runneru. Izlaz je artifact `grid-defaults.tsv`. Ova tablica je ono što znamo iz koda prije tog mjerenja; brojevi iz alata dopunjuju je čim run prođe. Gdje XML šuti, Carpo ne ostavlja DevExpress default ako se razlikuje od Janusa.

NetGen designer (`Form1.designer.vb` 624–664) već mijenja svježi GridEX prije XML-a. To je polazna točka granule, ne tvornički `new GridEX()`.

| Svojstvo | Janus / WinForms kako ga NetGen ostavi | DevExpress 26 (tvornički, Carpo ga ne kopira) | Što Carpo.NetGen postavlja |
|---|---|---|---|
| Sort klikom na header | `AutomaticSort` se gasi samo na `AUTOMATICSORT=false`. Inače ostaje uključen (`3401`) | Carpo `ComponentGrid` zna ugasiti `AllowSort` po definiciji | `OptionsCustomization.AllowSort=true`, osim `false` u XML-u |
| Auto-filter red | `FilterMode` nije Automatic dok XML ne kaže `FILTERMODE=true` (`2169`) | Carpo pali `ShowAutoFilterRow` osim ako definicija kaže false (`ComponentGrid.cs` 772) | `ShowAutoFilterRow=false`, osim `FILTERMODE=true` |
| Group panel | Atribut `GROUPBYBOXVISIBLE` ako postoji. Inače designer ga ne dira | Carpo gasi panel samo ako XML kaže false (`719`) | `ShowGroupPanel` samo ako XML ima atribut. Inače `janus-default` dok tsv ne kaže tvorničku vrijednost |
| Total / footer | Uključen osim `TOTALROWVISIBLE=FALSE` (`2049`) | DX footer je ugašen dok se ne traži | `ShowFooter` prati to pravilo |
| Širina kolona | Fiksne (`CreateGridColumns`), ne auto na cijeli grid | DX `ColumnAutoWidth` zna biti uključen | `ColumnAutoWidth=false` |
| Selekcija | Designer: `MultipleSelection` (`657`) | DX često jedan red | `MultiSelect=true` |
| Selekcija bez fokusa | Designer: `HideSelection.Highlight` (`640`) | DX zna posiviti red | `EnableAppearanceHideSelection=false` |
| Inkrementalna pretraga | Designer: `AllCharacters` (`641`) | Carpo je eksplicitno pali (`798`) | `AllowIncrementalSearch=true` |
| Uređivanje | `EditMode.EditOff`, stupac `NoEdit` dok nije u `EDITCOLUMNS` | DX često editable | `Editable=false`, osim filter+edit stupaca |
| Enter | Bez edit stupaca `EnterKeyBehavior.None`, pa atribut ako postoji (`3419`) | DX Enter zna otvoriti editor | `Enter=None` ili vrijednost iz XML-a |
| Obrub | Designer: `BorderStyle.None` | DX ima obrub | `BorderStyle=NoBorder` |
| Font | Designer: MS Sans Serif 8.25 | DX tema | isti font |
| Row header | Designer: `RowHeaders=True` | DX indicator | `ShowIndicator=true` |
| Checkbox stupac | Selector je vidljiv dok XML nije `FALSE` | DX nema ActAsSelector | poseban stupac `Selector`, vidljivost po `CHECKBOXVISIBLE` |
| `GRIDALLOWEDIT` | ne postoji u kodu | | ne postavlja se |

Ponašanja koja nisu jedno svojstvo:

| Ponašanje | NetGen | Test |
|---|---|---|
| Enter/Tab | vidi `ENTERKEYBEHAVIOR` | `oznaka-edit-ignorira-gridallowedit` (`None`) |
| Sort klikom | uključen osim `AUTOMATICSORT=false` | `oznaka-sort`, `oznaka-dx-sort-on` |
| Auto-filter | ugašen osim `FILTERMODE=true` | `oznaka-nema-filter`, `oznaka-dx-nema-autofilter` |
| Dvostruki klik | `MASTERDBLCLICK` otvara formu, nije svojstvo grida. Veza na formu nije ovaj element | nije u grid testu |
| Selekcija više redova | designer `MultipleSelection` | DX postavka `MultiSelect=true` |
| Scroll pri refreshu | `REFRESHMASTERGRID` ponovi zadnji SQL (`4577`), fokus po id / `MASTERMOVELAST` | `refresh-koristi-zadnji-tekst`, `fokus-zadnji` |

`docs/granule/` i `tests/` piše drugi agent. Ovaj dokument ih ne dira.
