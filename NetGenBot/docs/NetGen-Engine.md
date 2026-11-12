# NetGen engine — ponašanje iz koda

Commit `b0d5d41`. Linije su iz `netgen/`. Ovo je za provjeru „zašto“, ne za prepisivanje VB-a.

Gotovo sve procedure počinju s `On Error Resume Next`. Atribut koji ne postoji ne baca se do korisnika; vrijednost ostane prazna ili se preskoči sljedeći red. Iznimka su `Substring`/`IndexOf(":")` na gumbima: ako nema dvotočke, greška se pojavi.

## 1. Ulaz i XML

`Form1` je podloga (master). `Form2` je dijalog. Putanja:

- `Form1.vb` 1139: `strXML2 = remotedllpath & "\SharedDLLs\xml\" & Mid`
- `Form2.vb` 247 i 411: isto
- `Run.vb` 26: `Class1.strXML = remotedllpath & "\SharedDLLs\xml\" & Mid`

`Mid` je ime XML-a. Redoslijed DMS → baza → disk je u `NetGen-Tok.md`. Disk je `Form1.vb` 3262–3263 `m_xmld.Load`.

## 2. Čitanje PROJECT-a

Petlja je `Form1.vb` 3299–3520, `SelectNodes("/PROJECT")`.

Naslov: `Form1.vb` 3522–3528, čvor `/PROJECT/CAPTION`, prvi child text. `VERSION` se doda kao `v[…]`.

Atributi koji se stvarno čitaju (redoslijed u petlji):

| Atribut | Linija |
|---|---|
| `CMD1CAPTION`…`CMD10CAPTION` | 3314–3335. Tekst prije `:`, akcija iza `:` ide u `Button.Tag` |
| `COMBO`, `COMBOCLICK` | 3337–3348. SQL puni combo iznad stabla; kolona 1 = prikaz, kolona 0 = vrijednost |
| `GROUPBYBOXVISIBLE`, `GROUPBYBOXDETVISIBLE` | 3350–3351 |
| `MASTERGRIDLAYOUTFILE` | 3354 |
| `TREEDRAGDROP`, `TREETRANSLATE` | 3357–3360 |
| `FROZENCOLUMNS` | 3362 |
| `ONENTERKEY` | 3364 |
| `ONCLOSED` | 3366 |
| `STARTUPSTORA`, odmah prepisano s `ONLOAD` | 3367–3368 |
| `ONLOAD2` | 3369 |
| `MULTIGRIDLAYOUT` | 3370 |
| `SETDEFAULTS`, `FILTERSAVE` | 3371–3372 |
| `DETAILGRID` | 3373 → `cls1.strStartupStoraDetalj` |
| `DMSDET`, `DETMOVELAST`, `MASTERMOVELAST` | 3374–3381 |
| `INCLUDE` | 3383 |
| `POPUP`, pa prepis `POPUPMASTER` | 3384–3385 |
| `POPUPDETAIL` | 3386 |
| `TREE` | 3387 |
| `DBLCLICKFORM`, pa prepis `MASTERDBLCLICK` | 3388–3389 |
| `DETAILDBLCLICK` | 3390 |
| `BUTTONS` | 3391 |
| `ICON`, `VERSION`, `SPLITTERDISTANCE`, `PERMISIO` | 3392–3396 |
| `REFRESHAFTERCHILD` | 3397 |
| `TRANSLATECOLUMNS`, `TRANSLATEDETCOLUMNS` | 3398–3399 |
| `AUTOMATICSORT`, `AUTOMATICSORTDET` | 3401–3408. Samo `false` gasi sort |
| `DXGRID` | 3412 |
| `ENTERKEYBEHAVIOR` | 3423 |
| `FILTERMODE`, `FILTERMODEDET` | 3425–3426 |
| `MENU`, `TIMER` | 3447–3450 |
| `PAGEHEADER*`, `PAGEFOOTER*`, `PAGEORIENTATION` | 3452–3458 |
| `CHECKBOXVISIBLE`, `TOTALROWVISIBLE`, `DETCHECKBOXVISIBLE`, `DETTOTALROWVISIBLE` | 3459–3462, `.ToUpper` |
| `COMMANDTIMEOUT`, `RELATION` | 3463–3464 |
| `EDITCOLUMNS` | 3466–3472. Ako počinje s `sp` ili `select`, izvrši se i uzme se `trs(0)(0)` |
| `ONCELLEDITED`, `BEFORECELLEDITED` | 3474–3475 |
| `EDITCOLUMNSDET`, `ONCELLEDITEDDET`, `BEFORECELLEDITEDDET` | 3477–3479 |
| `DEFAULTCONNECTION`, `SQL2`, `SQL3`, `SQL4` | 3483–3486 |
| `HELPKEY`, `HELPKOD` | 3488–3495. Prazan `HELPKOD` sakrije HELP gumb |

`BUTTONS` (`Form1.vb` 3770–3813): `Split("#")`, svaki komad `tekst:akcija` prepisuje Button1… redom. Zato `#` unutar jedne akcije postaje novi gumb.

Klik gumba: `Form1.vb` 5664–5666 `NovaGenerickaForma(sender.tag)`.

Dvoklik stabla i `AfterSelect`: `Form1.vb` 5656–5661 i 7182–7187. `@@node` = tag do prve `:`. Ime forme = tag iza `:`. Onda `NovaGenerickaForma`.

## 3. ONLOAD i detail

`Form1.vb` 3871–3883:

1. `NapuniDrvoVrijeme()`
2. Ako `ONLOAD` počinje s `:`, `NovaGenerickaForma` na stringu s izbačenim svim dvotočkama (`Replace(":", "")`).
3. Inače `PokaziUpit` nakon `SrediSQL` + `SrediSQLPosljeNextForme`.

`ONLOAD2` (`Form1.vb` 3906–3928): jedan red rezultata, `DodatnoPostimajKontrolu` po gumbima. Ne puni grid.

`DETAILGRID` koji počinje s `:` (`Form1.vb` 1887–1888 i 2105–2106): `MsgBox("sorkac!")`, forma se ne otvara. Inače `PokaziUpitDetalj`.

## 4. Lanac komandi

Cijeli rječnik, uključujući mrtve grane na podlozi, je u `NetGen-Komande.md`. Ovdje ostaju linije ulaza.

`NovaGenerickaForma` (`Form1.vb` 4543): `~` postane `#` (4557), zatim `Split("#")`.

Bitne grane:

| Uvjet | Linija | Efekt |
|---|---|---|
| `REFRESHMASTERGRID` | 4570 | Ponovi zadnji SQL mastera |
| `findnode` | 4809 | |
| `refreshmovetoid` | 4831 | Osvježi i pozicioniraj red |
| počinje s `:` | 4862 | `NovaGenerickaForma2` bez vodeće `:` |
| počinje s `==` | 4872 | `PokaziUpit` bez `==` |
| počinje s `select` | 4875 | Izvrši SQL, prvi red podešava gumbe |
| inače | 4906 | `NovaGenerickaForma2` (ime forme) |

`NovaGenerickaForma2` (`Form1.vb` 4925):

- Ime počinje s `multiselect` ili `multiblue` (4932): briše `TotalGenMultiSelectTemp` po `suser_sname()` i po `operateriid`, zatim insert id-eva (5005, 5018, 5032): `insert into TotalGenMultiSelectTemp (id, hostname, susersname, operateriid)`.
- `printrepxmaster` i `printmaster` (4945–4955): oba zovu `IspisGRIDREPX`.
- `printdet` (4958): `IspisGRIDREPXdet`.
- `ssPermissionProvjeriRafalnuKomandu` (4939): zabrana se samo upiše u log i izađe.

Na dijalogu (`Form2.vb` 7038–7047) rade `PREPAREMULTISELECTEDPRINT`, `FOREACHSELECTED` (checkbox, 8337), `FOREACHBLUEROW` (plavi redovi, 8363), `FOREACHROW` (svi redovi, 8310). Drugi argument je akcija za svaki red.

Na podlozi (`Form1.vb` 6841–6850) iste četiri komande su zakomentirane: prepozna ih, preskoči i ne izvrši petlju.

## 5. Tree

`NapuniDrvoVrijeme` (`Form1.vb` 4296). Prazan `TREE` sakrije `TreeView2` i skupi splitter (4310–4320). Inače splitter 290 ili `SPLITTERDISTANCE` (4323–4326).

`Split("#")` (4344). Za svaki komad, lijevo od `:`:

- `yearmonth` → `DazovaVremenska` (4348, procedura 4527). Godine 2005–2009, mjeseci 1–12. Tag godine: `yyyy0101#yyyy1231:ImeForme`. Tag mjeseca je grublji (`g & j & "01"`), nije zero-padded.
- lijevo sadrži `tree` → `DazovaIzStore(lijevo, desno)` (4350, procedura 4435).
- inače statički čvor, caption = lijevo, tag = `:` + desno (4353–4355).

`DazovaIzStore`:

- SQL ide kroz `SrediSQLPosljeNextForme(SrediSQL(...))` (4442).
- Relacija (4450): `Columns(0)` = id, `Columns(1)` = parent.
- Natpis = `Columns(2)` (4461–4464). `TREETRANSLATE=true` prevodi.
- Tag = `Columns(3)`. Ako nema `:`, doda se `:ImeForme` (4473–4478). Root tag inače `0:ImeForme`.
- Djeca: `AddNodes` (4492). Preskače id `"0"`.

Nakon toga se iz `NetGenDBFilter` dodaju korisnički filter-čvorovi, tag `:==` + sql (4359–4368).

## 6. Varijable

### Master — `Form1.SrediSQL` (2470–2504)

Jedna linija zamjena (2482): `@@Id`/`@@ID`/`@@id` = polje `ID`, `@@operateriid` (tri veličine slova) = `cls1.OperateriID`, `@@DETID`/`@@detid`/`@@DetId` = `DetID`, `@@node`/`@@NODE` = `Node`, `@@table`/`@@TABLE` = `T1`–`T4` po captionu tekuće tablice hijerarhijskog grida (2473–2479).

Zatim `@@xmlname`, `@@jupitersrvpath`, `@@editedcolumn` (DX ime kolone ako postoji, inače Janus caption), `@@lastvalue`, `@@selectedcolor`.

`ID` dolazi iz `GetID` (5566): kolona `id`, fallback ćelija indeks 2, inače `"0"`. DX: `GetDXID` (5588), kolona `id`, fallback prva kolona.

`Node` se puni tek na klik/dvoklik stabla (5658–5659, 7184–7185), ne prije prvog odabira.

### Dijalog — `Form2.SrediSQL2` (2876–2922)

`On Error Resume Next` preko cijele funkcije.

Zamjene (2880–2895, 2917): `@@ID`/`@@id`, `@@operateriid`/`@@OPERATERIID`, `@@DETID`/`@@detid`/`@@DetId`, `@@node`/`@@NODE`, `@@tempstring`/`@@TEMPSTRING`, `@@editedcolumn`, `@@lastvalue`, `@@dblclickedcolumn`, `@@VERSION` (`NetGen vMajor.Minor.Revision`), `@@OPISNIPARAMETRI`, `@@FOCUSEDCONTROL` (`AktivnaKontola.Name`), `@@selectedfile`, `@@selectedpath`, `@@xmlname`, `@@jupitersrvpath`, `@@selectedcolor`.

`@@GRIDROWCOUNT(` je zakomentiran (2899–2905). Ne radi.

### Parametri kontrola — `Class1.SrediSQLPosljeNextForme` (740–853)

Prvo zove `Form2.SrediSQL2`. Ako nema prethodnih parametara i nema `@@`, vrati SQL kakav jest (756–758).

Inače, dok god ima `@@`, izreže token do prvog razdjelnika: razmak, `'`, `%`, `#`, zarez, `)`, `*`, `/`, `\`, `+`, `-`, `.`, `(`, `=` (766–847). Token ide u `NadjiVrijednostuPrevParametrima`. `¸¸` postaje `''`.

`NadjiVrijednostuPrevParametrima` (856):

- skine jedan `@` (`@@id` → `@id`) i traži u `strSviParametriuModulu` po prefiksu `@ime=` (860–865).
- `ajdijeva` u imenu: `~` → zarez (870–874).
- ime počinje s `@DAT`, ili sadrži `GRID` i `DAT`: prepakira datum. Za SQL stavlja navodnike, osim ako je vrijednost `null` (877–908).
- prazno → `"null"` (916).
- `@napomena` + `null` → prazan string, bez clipboarda (927–930).
- Clipboard `CB`: ako je i dalje prazno/`null`, traži `@IME=`. Broj: zarez → točka (963–964). Datumi iz CB idu u `'yyyyMMdd'` osim za ispis.

## 7. ExecSql

`Class1.ExecSql` (85):

- `~~` → zarez (87).
- SQL trim počinje s `@` → log `SKIPPED`, `Nothing` (104–108).
- SQL prazan **ili** `IndexOf("@@") > 0` → `SKIPPED`, `GreskaKodIzvrsavanjaSQLa = "SKIPPED"`, `Nothing` (113–118). Token na poziciji 0 se ovim uvjetom ne hvata.
- Inače `SqlDataAdapter.Fill`. Timeout = `strCommandTimeout` (99).
- Catch (157): poruke koje sadrže `ssoperateri`, `totalgenlang`, `sspermission`, `netgendbfilter`, `netgendbclipboard` se gutaju.
- Poruka s `#` ili `+`, ili `Err.Number = 5` (170): info MsgBox. `#OK`/`+OK` briše grešku (187–189). `#close`/`+close` postavlja `ZelimIzaciIzForme` (192–194). `#FOCUS(` / `+FOCUS(` čita ime kontrole (197–205).
- Inače kritični MsgBox s tekstom i SQL-om (222).
- Log: `spNetGenErrorLog` preko `ExecSql2` (230) i `JupiterLog.Greska` (235). `ExecSql2` (252) guta iznimku.

## 8. Form2 i INPUT

Atributi po tipu i što ide u `@ime`: `NetGen-Kontrole.md`. Redoslijed `ONLOAD` / submit / `NEXTFORM`: `NetGen-Tok.md`.

Atributi forme, `Form2.vb` 1031–1122: `MAXIMIZEBOX`, `TIMERSQL`, `COMMANDTIMEOUT`, `STORA` pa prepis `ONSUBMIT` (1036–1039), `DETAILGRID`, `ONLOAD`, `ONCLOSED`, `ONCLOSING`, `ONACTIVATED`, `ONLOADED`, `ONLOAD2`, `HELPKOD`, `ONLOSTFOCUS`, `NEXTFORM`, `FOCUS`, `DEFAULTCONNECTION`, `CUSTOMCONNECTIONSTRING`, `CMD1CAPTION`…`CMD6CAPTION`, `FILLEXPLORER`, `WIDTH`, `HEIGHT`, `VERSION`, `PERMISIO`, `DBCLIPBOARD`.

`DBCLIPBOARD`: ako ime forme sadrži `filter` i atribut nije `false`, forsira se `true` (1102–1104).

`RESIZE=false` (1120–1128): bez max/min, fiksna veličina.

`TYPE` je `InStr` nad malim slovima, ne enum. Redoslijed grana u `Form2.vb`:

| Linija | Podstring |
|---|---|
| 1256 | `statusstrip` |
| 1296 | `label` |
| 1354 | `group` ili `frame` |
| 1384 | `panel` |
| 1414 | `webbrowser` |
| 1451 | `splitcontainer` |
| 1460 | `check` |
| 1509 | `radio` |
| 1579 | `dxgrid` |
| 1623 | `grid` (samo ako `dxgrid` nije već uzet) |
| 1996 | `text` |
| 2071 | `number` ili `cur` |
| 2140 | `trazilica` ili `search` |
| 2232 | `dtpicker` ili `date` |
| 2370 | `combo` |
| 2478 | `multidrop` |
| 2558 | `tree` |
| 2614 | `button` |
| 2672 | `help` |
| 2707 | `picturebox` |

`SrediKontrolu` (`Form2.vb` 2924): `CAPTION`, pa `DEFAULT` prepiše `Text` (osim radio), skine `_`, `!!` → novi red. `TOP`/`LEFT`/`WIDTH`/`HEIGHT` dijele se s `FakPretvorbe`. `ENABLED`, `VISIBLE`, `NAME`. `TOP=0` ili caption na `*` pravi label `lbl`+ime (2959–3021). `TABINDEX` &lt; 0 gasi tab stop; `-2` ili prazno dobije automatski indeks. `REFRESHAFTERCHILD=TRUE` sprema ime, SQL i tip (3057–3066).

Combo (`2370–2431`): `ONLOSTFOCUS`, `ONCLICK`, `SQL`, `HOTKEY`, `TRANSLATE`, `ALLOWEMPTY`. `ValueMember` = kolona 0, `DisplayMember` = kolona 1. `ALLOWEMPTY` ubaci red `null` / `""` na vrh.

Search (`2140–2207`): caption na `_` → `SudjelujeUParametrima = False`. Tooltip = `CAPTION#SQL`. `ONLOSTFOCUS`, `ONLEAVE`, `COLUMNCAPTION`, `COLUMNWIDTH`, `COLUMNAUTORESIZE`. Id ostaje u `Tag`.

Tree na formi (`2558`) koristi isti `yearmonth` test kao podloga (`2576`).

## 9. Tražilica — Form3

`Form3.Trazilica` (71). Parametri prvo prolaze `SrediSQLPosljeNextForme`.

- Ako SQL počinje s `sp` ili `select` (99–100), izvršava se takav.
- Inače `Split("#")` (102): `[0]` tablica, `[1]` id kolona (u SELECT, nije u LIKE), od `[2]` kolone pretrage i sorta. `%` je wildcard LIKE s te strane (123–125). `!` izbaci polje iz WHERE (118). `/` je dodatni WHERE (120–121). Složi `select distinct … from … where … order by …` (133).

Odabir: `Form3.Odaberi` (18) puni `TrazilicaID` i `TrazilicaIDstr`.

## 10. Grid kolone

Master i detail kolone formatira `Form1.CustomFormatKolona` (7451), ne `Grid.Formatiraj`. Čita `INDEX`, `NAME`, `WIDTH`, `CAPTION`, `FORMAT`, `VISIBLE`, `FORECOLOR`, `BACKCOLOR`, `AGREGATEFUNCTION`, `POSITION`, `AUTOSIZE`, `MAXLINES`, `NEGATIVERED`, `NOTZERORED` (7486–7500). **Ne čita `ALIGN`.** `NEGATIVERED=true` crveni tekst za vrijednost &lt; 0 (7543–7549); `NOTZERORED` za vrijednost ≠ 0 (7554–7560). Caption: `;` → novi red (7535). `CHECKBOXVISIBLE=FALSE` sakrije kolonu indeks 1 (`Form1.vb` 2043–2052); na DX gridu kolonu `rcb555` (1898).

Grid **unutar forme** ide kroz `Grid.Formatiraj` (`Grid.vb` 57) i tu `ALIGN` postoji: RIGHT=3, CENTER=2, LEFT=1 (122–130). Isti ostali atributi kao gore, bez `NEGATIVERED` u toj proceduri. `CLEARCOLUMS=true` prvo sakrije sve kolone (63–66). Gađa kolonu po `INDEX` ako je &gt; 0, inače po `NAME`.

DX `Class1.CustomFormatKolonaDX` (1845): isti skup, plus `NEGATIVERED`, `NOTZERORED` (1878–1879). `POSITION` ide u `VisibleIndex`. Caption: `;` → `vbCrLf` (1922). Prije toga postavi numerički format `{0:N2}` na kolonu (1907) — DX default, `FORMAT` iz XML-a na toj grani ne prepisuje FormatString na isti način kao Janus.

`@@id` i checkbox: `CHECKBOXVISIBLE=FALSE` gasi checkboxe (`Form1.vb` 1898, 2043). Edit ćelije čekira red (`GridEX1_RecordUpdated`, 7176–7179), osim dok traje prijevod.

Layout grida se snima u `%AppData%\SPIN Informatica\Jupiter Software\Grid Settings\` (`GetLayoutDirectory`, 2509–2534).

## 11. Ispis i pomoćne forme

| Klasa | Uloga za XML |
|---|---|
| `Form5` | Ctrl+F11, ring buffer `NapuniCBLog`. Nije atribut. |
| `Form6` | Dijalog ispisa, `PrintDocument` (94). REPX ide u `SharedDLLs\repx` (158–161). |
| `Form9` | Odabir REPX templatea, kolona `Template` (12–20), `IspisiMePrekoSpinPrintinga` (72). |
| `Form3` | Dijalog tražilice. |
| `Form7` | Ctrl+F9, izbor jezika (`Form1.vb` 1384, `Form7.NapuniGrid`). Nije XML tip. |
| `Form4` | Cijeli fajl je komentar. |
| `Form8` | Prazna klasa. |
| `frmIspis` | Stari DevExpress ispis kontrole, ne XML jezik. |
| `LokalniIspisRDLC` | Zakomentiran. |
| `FormatStyles`, `AppearanceDialog` | UI za „Format style“ / „Izgled grida“ iz desnog klika, ne atributi. |

Desni klik mastera (`Form1.vb` 3560–3676) doda stavke iz `POPUPMASTER`, zatim fiksne: spremi postavke (`spremigrid`), reset, export, grupiranje, format, izgled, `ispisdx`, `ispisnovi`.

## 12. Starih 527 pravila

`docs/netgen/netgen_master_global_rules_detaljno_updated.md` (20. 1. 2026.) skupljen je iz primjera XML-a, ne iz ovog izvora.

Potvrđeno u kodu ovog commita: `ONLOAD`, `ONLOAD2`, `DETAILGRID`, `MASTERDBLCLICK`, `DETAILDBLCLICK`, `BUTTONS`, `CMDxCAPTION`, `POPUPMASTER`, `POPUPDETAIL`, `TREE`, `TREETRANSLATE`, `SPLITTERDISTANCE`, `GROUPBYBOXVISIBLE`, `FROZENCOLUMNS`, `CHECKBOXVISIBLE`, `TOTALROWVISIBLE`, `DETCHECKBOXVISIBLE`, `DETTOTALROWVISIBLE`, `ENTERKEYBEHAVIOR`, `MULTIGRIDLAYOUT`, `DXGRID`, `EDITCOLUMNS`, `ONCELLEDITED`, `MASTERCOLUMNS`/`COLUMN` atributi iz odjeljka 10, `INPUT` tipovi iz odjeljka 8, `@@` iz odjeljka 6, `FOREACHROW`, `FOREACHBLUEROW`, `REFRESHCONTROL`, `PRINTMASTER`, `TotalGenMultiSelectTemp` za `MULTISELECT*` / `MULTIBLUE*`.

Ne prenositi u odgovor autoru atribut koji postoji samo u tom fajlu, a nema `GetNamedItem` u `netgen/*.vb`. `@@GRIDROWCOUNT` je u starom tekstu i u komentaru koda — ne radi. `FOREACHROW` / `FOREACHBLUEROW` / `FOREACHSELECTED` rade na dijalogu; na podlozi su zakomentirani. `ALIGN` na `<COLUMN>` vrijedi za grid u formi, ne za `MASTERCOLUMNS`.
