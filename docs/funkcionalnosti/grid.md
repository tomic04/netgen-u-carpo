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

## Rezultat na sql-ai

Run koji je prošao: https://github.com/tomic04/netgen-u-carpo/actions/runs/43213260590
Prethodni, 43212527893 i 43212561971, pali su na istoj SQL iznimci prije nego je Catch uhvaćen. Run 43212771219 je prvi zeleni, još bez group-panel defaulta.

Automatski testovi grida: **59 PASS, FAIL=0**.

| Granula | SQL koji bi NetGen poslao | Redovi | Kolone | Napomena |
|---|---|---|---|---|
| `OznakaKL` | `select klmasterrobaid, sortkey, Naziv, oznaka, oznaka2 as DodatnaOznaka from klmasterroba order by sortkey` | 0 | nije vezan | `Invalid column name 'oznaka2'`. `ExecSql` bi ušao u Catch, pokazao poruku i vratio `Nothing`. Tablica ima **947** redova. Od stupaca s „oznak” postoji samo `Oznaka`. XML i baza nisu dirani. |
| `JeziciNG` | `spJeziciQuery @action=0, @id=''` | **15** | **2** | Fokus na prvom redu, id=13. `Naziv` širina 178 (2500/14). Agregat s dva G nije vezan. |

## Defaulti

Izvor mjerenja: `Carpo.NetGen.Defaults` na runneru, artifact `grid-defaults.tsv` (isti run). To su svježi `new GridEX()` i `new GridView()`, bez XML-a i bez NetGen designera.

Kad XML šuti, Carpo prati **kontrolu koju NetGen drži na formi**: designer (`Form1.designer.vb` 624–664) plus kod u `PokaziUpit`. Svojstva koja designer ne dira ostaju na izmjerenoј tvorničkoj vrijednosti.

| Svojstvo | Svježi Janus GridEX | NetGen kad XML šuti | Svježi DevExpress 26 | Što Carpo.NetGen postavlja |
|---|---|---|---|---|
| Sort klikom na header | `AutomaticSort=True` | ostaje True (kod gasi samo na `false`, `3401`) | sort kolone je uključen | `AllowSort=true` |
| Group panel | `GroupByBoxVisible=True` | ostaje True (designer ne dira) | panel DX-a nije isti objekt | `ShowGroupPanel=true` |
| Auto-filter | `FilterMode=None` | ostaje None dok XML nema `FILTERMODE` | `ActiveFilterEnabled=True` | **namjerno odstupanje:** `ShowAutoFilterRow=true` kao Carpo, osim ako XML ima `FILTERMODE` |
| Total red | `TotalRow=False` | kod ga upali osim na `FALSE` (`2049`) | footer nije uključen | `ShowFooter=true` (pratimo kod, ne svježi GridEX) |
| Uređivanje | `AllowEdit=True`, `EditMode=EditOff` | stupci `NoEdit` | `Editable=True` | `Editable=false` |
| Enter | `EnterKeyBehavior=NextRow` | kod stavi `None` ako nema `EDITCOLUMNS` (`3419`) | Enter zna otvoriti editor | `Enter=None` |
| Selekcija | `SingleSelection`, cijeli red | designer: `MultipleSelection` | `IsMultiSelect=False` | `MultiSelect=true` |
| Selekcija bez fokusa | `HideSelection=HideSelection` | designer: `Highlight` | `FocusRectStyle=CellFocus` | `EnableAppearanceHideSelection=false` |
| Row header | `RowHeaders=False` | designer: `True` | indicator | `ShowIndicator=true` |
| Obrub | `SunkenLight3D` | designer: `None` | `BorderStyle=Default` | `NoBorder` |
| Širina kolona | fiksne | isto | auto širina je čest DX default | `ColumnAutoWidth=false` |
| Scroll | `ScrollBars=Automatic` | isto | `HorzScrollVisibility=Auto` | ne diramo |
| Grupni total | `GroupTotals=Never` | designer: `Always` | `GroupFooterShowMode=VisibleIfExpanded` | otvoreno, nije mapirano |
| Font | (designer) MS Sans Serif 8.25 | isto | tema | `Font=Microsoft Sans Serif, 8.25pt` |
| Checkbox stupac | nema ga dok `CreateGridColumns` ne doda Selector | vidljiv osim `CHECKBOXVISIBLE=FALSE` | nema ActAsSelector | stupac `Selector` |
| `GRIDALLOWEDIT` | kod ga ne čita | ne pali edit | | ne postavlja se |

Najvažnije razlike koje su morale biti eksplicitne, jer DX default nije Janus:

- auto-filter: vidi odjeljak Namjerno odstupanje. `FILTERMODE` u XML-u i dalje odlučuje.
- uređivanje: DX `Editable=True`, Janus `EditMode=EditOff` → `Editable=false`
- Enter: Janus tvornički `NextRow`, NetGen kod `None` kad nema edit stupaca → `Enter=None`
- selekcija: svježi GridEX i DX su jedan red, designer NetGena je `MultipleSelection` → `MultiSelect=true`
- obrub i row header: svježi GridEX je `SunkenLight3D` i bez row headera; forma ima `None` i `RowHeaders=True`

Ponašanja koja nisu jedno svojstvo:

| Ponašanje | NetGen | Test |
|---|---|---|
| Enter | `None` bez `EDITCOLUMNS` | `oznaka-edit-ignorira-gridallowedit` |
| Sort klikom | uključen osim `AUTOMATICSORT=false` | `oznaka-sort` |
| Auto-filter | ugašen u Janus-u (`FilterMode=None`) osim `FILTERMODE=true` | namjerno odstupanje, vidi ispod |
| Dvostruki klik | `MASTERDBLCLICK` otvara formu | otvoreno, nije ovaj element |
| Više redova | designer `MultipleSelection` | `MultiSelect=true` |
| Scroll pri refreshu | ponovi zadnji SQL, fokus po id / `MASTERMOVELAST` | `refresh-koristi-zadnji-tekst`, `fokus-zadnji` |

## Namjerno odstupanje

Filter red. Janus `FilterMode` je `None` dok XML ne kaže `FILTERMODE=true` (`Form1.vb` 2169). Carpo u `ComponentGrid.SetProperties` (`Carpo.UI/Components/Base/ComponentGrid.cs` 772–773) radi:

```csharp
if (!(definition.ShowAutoFilterRow == false))
    gridView1.OptionsView.ShowAutoFilterRow = true;
```

`ShowAutoFilterRow` je `bool?` na gridu (`Carpo.Structure/Models/Core/DataControls.cs` 42 i 196). Atribut u Carpo XML-u je `showautofilterrow`. Ako ga nema (`null`) ili je `true`, red je uključen. Samo eksplicitni `false` ga gasi. Uz to Carpo zaključa promjenu operatora: `OptionsFilter.AllowAutoFilterConditionChange = False` (isti fajl, 797) i `ShowFilterPanelMode = Default` (794).

Carpo.NetGen radi isto kad NetGen XML **nema** `FILTERMODE`: auto-filter red je uključen. Razlog: red samo sužava prikaz, ne mijenja SQL, fokus ni spremanje. Ako `FILTERMODE` postoji, vrijedi NetGen: `true` uključi, sve ostalo (uključujući `false`) isključi. `AllowEdit` se i dalje dira samo kad je `FILTERMODE=true`, kao u `Form1.vb` 2178, da dodatni red ne ugasi uređivanje.

## Otvoreno

- `OznakaKL` na `spin`: 0 redova, jer nema `oznaka2` (ima `Oznaka`, tablica ima 947 redova).
- `EDITCOLUMNS` koji je SQL (44 datoteke) se ne izvršava da bi se dobio popis stupaca.
- `ONCELLEDITED` / `BEFORECELLEDITED` se čitaju, izmjena ćelije se ne vrti.
- `TRANSLATECOLUMNS` i `MASTERGRIDLAYOUTFILE`.
- `GroupTotals=Always` s designera nije mapiran na DX.
- Sonda `asm:` (UTF-16) nije našla `Jupiter.exe`. 16 pogodaka su tuđi DLL-ovi. Ulaz ostaje `Carpo.NetGen.Loader`. Nije dokazano cijepa li host na prvoj ili zadnjoj točki.

`docs/granule/` i `tests/` piše drugi agent. Ovaj dokument ih ne dira.
