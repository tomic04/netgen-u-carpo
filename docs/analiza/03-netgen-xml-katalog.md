# 03 — Katalog NetGen XML jezika

Katalog je spoj onoga što interpreter čita (`netgen/*.vb`, vidi dokument 02) i onoga što je u stvarnim granulama. Brojevi dolaze iz `tools/netgen_xml_katalog.py`, pokrenutog nad repoom 2026-11-12. Sirovina: `tools/out/netgen-xml-katalog.json` i kraći `tools/out/netgen-xml-katalog-sazetak.txt`.

## 1. Kako je brojano

Tri mape:

| Mapa | Datoteka na disku | Strogi XML parse pada |
|---|---|---|
| `definitions/netgen` | 605 | 10 |
| `NetGenBot/granule` | 605 | 10 (isti fajlovi) |
| `definitions/Prepisane_granule/Netgen` | 28 | 0 |
| **na disku** | **1238** | |
| **jedinstven SHA-256** | **609** | **10** |

`definitions/netgen` i `NetGenBot/granule` su isti skup bajtova. Razlikuju se imena mapa: `Agronom granule` naspram `Agronom`, `xml-ovi s nevkoša-zubak` naspram `Nevkos`, `BIOVEGA` naspram `Biovega`. Unutar jednog stabla još 22 datoteke su kopija Agronom↔Biovega (npr. `MPEurParam.xml`, `KntOPZ-STAT-1.xml`, `KOD_RAD1.xml`). `Prepisane_granule/Netgen` dodaje 26 novih sadržaja; 2 se poklapaju s glavnim skupom.

Svi brojevi ispod su nad **609 jedinstvenih** sadržaja. Sirovi zbroj po mapama u JSON-u pod `corpora` duplikate još ima, namjerno, da se vidi koliko ih je na disku.

Encoding: 587× windows-1250, 17× UTF-8, 5× UTF-8 s BOM. Callisto granule su cp1250. Skripta prvo pokuša UTF-8, pa cp1250.

Parser je tolerantni regex (komentari se izbace, atributi `ime="..."` / `ime='...'`). Strogi `xml.etree` pada na 10 datoteka. Uzorak: `definitions/netgen/Agronom granule/MPKaseOperNG.xml`, `MPPOSmeni.xml`, `MPPovijestPrijava.xml`, `MPTestBlagajni.xml`, `definitions/netgen/BIOVEGA/MPEFaktureOtpremnice.xml`, `MPKase.xml`, `MPKaseOperNG.xml`, `MPKaseTemplate.xml`, `MPPOSmeni.xml`, `MPTrgovineOstalo.xml`. Uzrok nije ručno otvaran za svaku; tipično su neescapani `&` ili `<` unutar atributa. **Neprovjereno** je li svaka od tih deset u produkciji. NetGenov `XmlDocument.Load` na njima može pasti i otići na DMS/bazu — to nije pokrenuto.

Kod-atributi: skripta vadi `GetNamedItem("X")` i `.Attributes("X")` iz `netgen/*.vb`. Pristup koji slaže ime atributa u runtimeu skripta ne vidi. Zato „nema u kodu“ znači „nema u tom obrascu“, ne apsolutnu nemogućnost.

Komande: heuristika. Vrijednost se dijeli po `#` i `~`. Ako je oblik `Natpis:Akcija` i lijevo nije SQL ni poznata komanda, broji se desno. Poznati prefiksi (`CLOSE`, `OK`, `PRINTMASTER`, `REPX/`, …) su pouzdani. Imena formi s `š/č/ž/đ` regex tokena reže na ASCII (`Pokaži` → `POKA`). Zato kanta `<FORMA:…>` nije popis formi. Popis formi je popis **elemenata** (ime taga je Unicode).

## 2. Jezik, ne imena formi

NetGen nema fiksni skup formi. Sve što je izravno dijete `<PROJECT>`, a nije strukturni tag, jest forma. Engine je traži kao `/PROJECT/{ime}`.

Strukturni elementi koji jesu jezik (broj pojavljivanja / broj datoteka):

| Element | Broj | Datoteka | Značenje |
|---|---|---|---|
| `PROJECT` | 598 | 598 | korijen. 11 jedinstvenih fajlova nema `PROJECT` (pokvareni ili fragment). |
| `INPUT` | 54165 | 589 | kontrola. `TYPE` odlučuje klasu |
| `COLUMN` | 2918 | 170 | format kolone grida |
| `NAME` | 598 | 597 | dijete `PROJECT`, staro ime; engine naslov vuče iz `CAPTION` |
| `CAPTION` | 598 | 597 | naslov prozora, prvi tekstualni child |
| `KOMENTAR` | 336 | 336 | Callisto metapodatak, engine ga ne čita kao ponašanje |
| `HISTORY` | 225 | 225 | isto |
| `MASTERCOLUMNS` | 111 | 111 | omot `<COLUMN>` za master |
| `DETAILCOLUMNS` | 52 | 52 | omot za detail |
| `AUTOR` | 85 | 85 | metapodatak, nije viđen kao `GetNamedItem` |
| `INPUT2` | 36 | 3 | **nije** drugi prolaz. Engine traži `INPUT`. Ovo su 3 datoteke s krivim imenom taga |

`FILTER` kao element (35) u skripti je izbačen iz liste imena formi jer je bio u strukturnom skupu skripte. To je vjerojatno forma koja se zove `FILTER`, ne ključna riječ. **Oprez:** broj imena formi 2098 uključuje i takve granične slučajeve. Pravi jezik kontrola je `INPUT` + `COLUMN` + atributi na `PROJECT` i na formi.

Najčešća **imena formi** (element = ime, svaka pojava je jedna definicija):

| Ime | Broj | Datoteka |
|---|---|---|
| `FILTERFORM` | 247 | 247 |
| `DANAS_FORM` | 97 | 97 |
| `JUCER_FORM` | 89 | 89 |
| `PROSLIMJESEC_FORM` | 89 | 89 |
| `FILTERODDO_FORM` | 88 | 88 |
| `UPDATEFORM` | 86 | 86 |
| `TEKUCIMJESEC_FORM` | 85 | 85 |
| `BRISISTAVKU` | 77 | 77 |
| `ZAKLJUCI` | 76 | 76 |
| `DONOSIZROBE` | 75 | 75 |
| `DONESI` | 72 | 72 |
| `IZMJENISTAVKU` | 70 | 70 |
| `UPDATE_FORM` | 68 | 68 |
| `OTKLJUCAJ` | 65 | 65 |
| `MAINFORM` | 64 | 64 |
| `INSERTFORM` | 57 | 57 |
| `DELETEFORM` | 45 | 45 |
| `INSERT` / `UPDATE` / `DELETE` | 33 / 32 / 30 | isto toliko datoteka |

Uzorak `*_FORM` (`DANAS_FORM`, `JUCER_FORM`, `PROSLIMJESEC_FORM`, `TEKUCIMJESEC_FORM`, `FILTERODDO_FORM`) je gotovo isti na ~85–97 datoteka. To je Callisto predložak filtera razdoblja, ne 97 različitih jezika. Adapter koji prođe jedan takav predložak pokriva cijeli taj skup.

Ukupno različitih imena elemenata: 2120. Od toga je dugi rep ime forme. Puni popis je u JSON-u, ključ `elements`.

## 3. `TYPE` na `INPUT`

Engine: prvi podstring u `Form2.vb` (dokument 02). Broj je nad 54165 kontrola.

| TYPE kako je napisan | Broj | Datoteka | Grana koja ga uzme |
|---|---|---|---|
| `label` | 12964 | 421 | `label` |
| `button` | 11731 | 491 | `button` |
| `text` | 7506 | 420 | `text` |
| `number` | 6491 | 321 | `number` |
| `combo` | 4609 | 485 | `combo` |
| `date` | 2618 | 403 | `date` |
| `grid` | 2029 | 387 | `grid` (Janus) |
| `search` | 1728 | 339 | `search` |
| `frame` | 1603 | 281 | `frame` → GroupBox |
| `check` | 1355 | 230 | `check` |
| `group` | 474 | 74 | `group` → isti GroupBox |
| `checkbox` | 404 | 138 | sadrži `check`, radi |
| `radio` | 299 | 45 | `radio` |
| `vb.checkbox` | 56 | 29 | sadrži `check`, radi; VB6 ostatak |
| `trazilica` | 49 | 29 | `trazilica` |
| `multidrop` | 43 | 21 | DX checked combo |
| `datetime` | 35 | 16 | sadrži `date` |
| `statusstrip` | 25 | 9 | statusna traka |
| `panel` | 23 | 11 | `panel` |
| `tree` | 11 | 11 | `tree` |
| `picturebox` | 5 | 5 | `picturebox` |
| `webbrowser` | 3 | 3 | `webbrowser` |
| `picture` | 2 | 2 | **ne** sadrži `picturebox` — ne nastane |
| `int` | 3 | 3 | ne pogodi nijednu granu |
| prazan `TYPE` | 1 | 1 | ne nastane |

Tipfeleri koje grana ne uzme (rijetko, ali postoje u produkcijskim fajlovima):

| TYPE | Broj | Datoteka | Zašto ne radi |
|---|---|---|---|
| `b_utton` | 15 | 7 | nema podstring `button` |
| `t1ext` | 14 | 5 | nema `text` |
| `bxutton` | 11 | 4 | nema `button` |
| `cmeditvb6.curmedit` | 8 | 1 | nema `cur` na početku grane; sadrži `cur` u `curmedit` — **upadne u number** jer grana traži `cur` bilo gdje. Jedan fajl, 8 kontrola. Ponašanje je slučajno |
| `vb.textbox` | 8 | 2 | nema `text` (`textbox` bi imao; `vb.textbox` ima `text`? `vb.textbox` sadrži `text`. **Upadne u text.** Ispravka: `InStr("vb.textbox","text")` je istina. Ovi rade kao text |
| `g_rid` | 4 | 4 | nema `grid` |
| `b1utton` | 3 | 1 | nema `button` |
| `buttton` | 2 | 2 | nema `button` (`butto` + `n` višak t) |
| `datetiem` | 5 | 5 | nema `date` |
| `buttonnevrijedi` | 2 | 1 | sadrži `button` — **nastane gumb**, ime je komentar autora |
| `cbex3.comboboxex3` | 1 | 1 | sadrži `combo` — nastane combo |
| `tmedit2vb6.textmedit2` | 2 | 2 | sadrži `text` — nastane text |
| `tekst` | 2 | 1 | nema `text` |

Pravilo za adapter: ne uvoditi enum. Kopirati redoslijed `InStr`, uključujući slučajne pogotke (`vb.checkbox`, `cmeditvb6.curmedit`, `datetime`). Tipfelere koji ne pogode (`b_utton`, `g_rid`, `datetiem`) NetGen također ne crta. Adapter koji ih „popravi“ mijenja ponašanje.

`dxgrid` kao `TYPE` u ovom korpusu **nije** izašao u top listi (nema ga među 44 vrijednosti). `DXGRID` kao atribut `PROJECT` kod čita (`Form1.vb`), korpus ga nema. Master grid u ovim 609 fajlova je Janus put, ne DX put. **Neprovjereno** za XML koji živi samo u bazi `netgenxml`.

## 4. Atributi `PROJECT`

111 različitih imena atributa na `<PROJECT>`. 69 ih kod traži `GetNamedItem`. Najčešći (broj datoteka koje ga imaju na korijenu):

| Atribut | Datoteka | Kod čita |
|---|---|---|
| `CHECKBOXVISIBLE` | 518 | da |
| `ONLOAD` | 465 | da |
| `MASTERDBLCLICK` | 438 | da |
| `TOTALROWVISIBLE` | 403 | da |
| `POPUPMASTER` | 389 | da |
| `DETTOTALROWVISIBLE` | 386 | da |
| `DETCHECKBOXVISIBLE` | 374 | da |
| `GROUPBYBOXVISIBLE` | 361 | da |
| `CMD1CAPTION` | 353 | da |
| `CMD2CAPTION` | 315 | da |
| `CMD3CAPTION` | 311 | da |
| `DETAILGRID` | 281 | da |
| `FROZENCOLUMNS` | 262 | da |
| `SETDEFAULTS` | 255 | da |
| `CMD5CAPTION` | 252 | da |
| `BUTTONS` | 250 | da, prepisuje `CMDx` |
| `CMD4CAPTION` | 240 | da |
| `CMD6CAPTION` | 231 | da |
| `TREE` | 227 | da |
| `COMMANDTIMEOUT` | 203 | da |
| `REPORT` | 176 | **ne** |
| `PAGEHEADERRIGHT` | 158 | da |
| `AUTOFITCOLUMNS` | 59 | **ne** |
| `CMDnCAPTION1` (1…8) | 16 svaki | **ne** — ime s viškom `1` |
| `GROUPBOXVISIBLE` | 6 | **ne** (kod traži `GROUPBYBOXVISIBLE`) |
| `B_UTTONS` | 6 | **ne** |
| `PERMISSIO` | 2 | **ne** (kod traži `PERMISIO`) |
| `FILTERSAVE` | 2 | da |
| `ONSUBMIT` na PROJECT | 3 | da na formi; na korijenu se čita ako je u istoj petlji — **neprovjereno** da master `ONSUBMIT` igdje radi. Master submit je `ONLOAD` |

`REPORT` na 176 korijena je najveći atribut koji lookup ne dira. U `netgen/*.vb` string `REPORT` kao ime atributa nije nađen (pojavljuje se samo u uzorku unutar `Resource1.Designer.vb`). Za adapter: ignorirati, kao NetGen. Ako neki vanjski alat čita `REPORT`, to nije ovaj DLL.

Puni popis 111 je u JSON-u, ključ `project_attributes`.

## 5. Atributi `INPUT`

Najčešći (broj kontrola, ne datoteka):

| Atribut | Broj | Kod u `SrediKontrolu` / grani |
|---|---|---|
| `TYPE` | 54165 | da |
| `NAME` | 54136 | da. 29 kontrola bez imena — parametar se ne može složiti |
| `TOP` | 51696 | da, `/14` |
| `WIDTH` | 51466 | da, `/14` |
| `LEFT` | 51363 | da, `/14` |
| `HEIGHT` | 48108 | da, `/14` |
| `CAPTION` | 41536 | da |
| `TABINDEX` | 21160 | da |
| `TEXT` | 17796 | **ne.** Callisto duplikat captiona. Engine crta `CAPTION` |
| `ONCLICK` | 12527 | da, ide u `Tag` i `Rafalno` |
| `SQL` | 11012 | da |
| `ENABLED` | 9082 | da |
| `VISIBLE` | 7015 | da |
| `DECIMALS` | 5789 | da, number |
| `ALLOWEMPTY` | 4522 | da, combo |
| `ONLOSTFOCUS` | 3728 | da |
| `CHECKBOXVISIBLE` | 1898 | da, grid |
| `ANCHOR` | 1781 | da |
| `HOTKEY` | 1644 | da |
| `TOTALROW` | 1477 | da, grid u formi |
| `REFRESHAFTERCHILD` | 1473 | da |
| `GROUPBYBOXVISIBLE` | 1244 | da |
| `STYLE` | 1177 | djelomično (kolone / grid) |
| `DOCK` | 991 | da |
| `SIZE` | 828 | **neprovjereno** u `SrediKontrolu`; nije u lookup setu skripte za sve fajlove — provjera: nije u listi „in_code“ za ovaj izvještaj skripte na INPUT ako skripta kaže `code=True` samo za GetNamedItem. U sažetku `SIZE` je `code=True` (linija sažetka). Znači neki `.vb` radi `GetNamedItem("SIZE")` |
| `BOLD` | 763 | **ne** |
| `BACKCOLOR` | 712 | da, uz ograničen skup imena boja |
| `EDITCOLUMNS` | 628 | da |
| `PARENT` | 530 | da |
| `ONDBLCLICK` | 489 | da |
| `POPUP` | 459 | da |
| `VALUE` | 455 | da, number prepisuje default |
| `ONCELLEDITED` | 454 | da |
| `PAGEHEADER*` / `PAGEFOOTER*` / `PAGEORIENTATION` | 300–450 | da, ispis grida u formi |
| `SHOWCHECKBOX` / `CHECKED` | 361 / 94 | da, date |
| `MAXLENGTH` | 299 | da |
| `MULTILINE` | 263 | da, text |
| `SKIPDEFAULTLOAD` | 175 | da |
| `ALOweMPTY` (`ALOWEMPTY`) | 127 | **ne** — tipfeler od `ALLOWEMPTY`. Tih 127 combo/search **nema** prazan red, iako je autor to mislio |
| `ENTERKEYBEHAVIOR` | 112 | da |
| `COLUMNCAPTION` / `COLUMNWIDTH` | 92 / 79 | da, tražilica |
| `FORMAT` | 85 | da, date |
| `HIGHT` | 43 | **ne** — visina se ignorira |
| `WUDTH` | 23 | **ne** |
| `MAXLENGHT` | 21 | **ne** |
| `DECIMAL` | 20 | **ne** (kod traži `DECIMALS`) |
| `SKIPDEFAULTLOAD2` | 19 | **ne** |
| `STILE` | 17 | **ne** |
| `ONCLICKKK` | 16 | **ne** |

Adapter koji „ispravi“ `HIGHT`→`HEIGHT` promijeni ekran u odnosu na NetGen. Za kompatibilnost prva verzija treba ignorirati isto što i DLL. Poseban izvještaj tipfelera može ići autorima, izvan runtimea.

## 6. Atributi `COLUMN`

`Grid.Formatiraj` (`netgen/Grid.vb` 89–101) čita, redom, i pod `On Error Resume Next` ako atribut fali: `INDEX`, `NAME`, `WIDTH`, `CAPTION`, `FORMAT`, `VISIBLE`, `FORECOLOR`, `BACKCOLOR`, `AGREGATEFUNCTION`, `ALIGN`, `POSITION`, `AUTOSIZE`, `MAXLINES`.

U korpusu:

| Atribut | Broj | Kod |
|---|---|---|
| `NAME` | 2918 | da |
| `INDEX` | 2904 | da. Gotovo svaka kolona ima i ime i indeks. Engine uzme indeks ako je > 0 |
| `WIDTH` | 2876 | da, dijeli se s 14 |
| `FORMAT` | 2850 | da, Janus `FormatString` |
| `CAPTION` | 2840 | da |
| `VISIBLE` | 2821 | da |
| `AGREGATEFUNCTION` | 1951 | da (jedno G — tako je u kodu, linija 97) |
| `AGGREGATEFUNCTION` | 518 | **ne.** Ispravno englesko slovkanje se ignorira. Suma na tih 518 kolona u NetGenu ne radi |
| `FORECOLOR` / `BACKCOLOR` | 222 / 222 | da, samo imena: green, white, black, darksalmon, darkseagreen, limegreen, greenyellow, yellow, lightyellow, red (`Grid.vb` 147–168) |
| `POSITION` | 96 | da |
| `STYLE` | 11 | da ako ga grana formata čita; na `Grid.Formatiraj` iz ovog isječka **nije** u listi 89–101. Može ga čitati druga petlja (`Form1.CustomFormatKolona`). **Djelomično neprovjereno** za `STYLE` na koloni |
| `ALIGN` | 2 | da u `Grid.Formatiraj` (grid **unutar forme**). `Form1.CustomFormatKolona` za master/detail `ALIGN` ne čita (`NetGen-XML-Pravila.md`). U korpusu je ionako 2 |

`AUTOSIZE`, `MAXLINES`, `NEGATIVERED`, `NOTZERORED` kod čita, u korpusu **0**. Ili se ne koriste, ili su samo u XML-u iz baze.

Boje izvan tog `Select Case` ne rade ni u NetGenu (tiho, `On Error Resume Next` oko cijelog bloka ako atribut fali; ako boja nije u listi, grana se samo ne uđe).

## 7. Komande u korpusu

Heuristika iz odjeljka 1. Pouzdani prefiksi:

| Komanda | Broj | Gdje uopće radi |
|---|---|---|
| SQL koji nije posebna komanda | 20669 | dijalog: `RefreshControls`; podloga: `ExecSql` |
| `CLOSE` | 4135 | oba |
| `OK` | 3555 | dijalog = spremi; podloga zove klik Button1 |
| `REFRESHCONTROL` | 1526 | dijalog puni kontrolu; na podlozi samo skine zagrade |
| `REPX` (`repx/...` ili `:repx/`) | 481 | ispis predloška, nije Janus |
| `FOREACHROW` | 158 | **samo dijalog** |
| `FOREACHSELECTED` | 143 | samo dijalog |
| `FOREACHBLUEROW` | 109 | samo dijalog |
| otvaranje forme čije ime počinje s `MULTISELECT` | 94 | podloga, puni `TotalGenMultiSelectTemp` |
| `PRINTGRID` | 93 | dijalog živo; podloga mrtvo |
| `PRINTMASTER` | 64 | podloga, `NovaGenerickaForma2` 4951 |
| `ASSEMBLYLOAD` | 55 | oba, late-bound |
| `OKSTAY` | 55 | dijalog |
| `CLICK` | 54 | dijalog; podloga mrtvo |
| `MULTIBLUE` (ime forme) | 52 | podloga |
| `REFRESHMASTERGRID` | 47 | podloga |
| `CANCEL` | 36 | oba |
| `FISKALIZACIJA` | 22 | late-bound |
| `MSGBOX` | 20 | dijalog |
| `PRINTREPXMASTER` | 19 | isto što i `PRINTMASTER` |
| `CREATETEXTFILEUTF8` | 13 | dijalog |
| `COLORPICKER` | 10 | oba, različito |
| `FISKALIZACIJABSRACUNI` | 7 | dijalog |
| `POSLOVNIPROSTOR` | 6 | oba |
| `PRINTREPXGRID` | 5 | dijalog |
| `ISEMPTYORZEROALERT` | 5 | dijalog |
| `BLAGAJNAENABLE` | 3 | oba |
| `DISABLEALLCONTROLS` | 3 | dijalog |
| `FISKALIZACIJAFAKTURE` | 2 | dijalog |
| `CLOSEPARENT` | 2 | oba |
| `CREATETEXTFILE3` | 1 | dijalog |
| `FISKALIZACIJAPD` | 1 | dijalog |
| `SHOWFOLDERBROWSERDIALOG` | 1 | dijalog |
| `SHOWOPENFILEDIALOG` | 1 | dijalog |
| `MASTERMOVEPREVIOUS` | 1 | dijalog |

U korpusu **nema** (kanta komandi = 0), a kod ih ima: `ISEMPTY(`, `ISEMPTYORZEROCANCELBOX`, `EVAL`, `RECALCGRID`, `RECALCGRIDROW`, `TOGGLEGRIDGROUP`, `PIVOTGRID`, `CHARTGRID`, `GRID_COLUMN_VALUE`, `HIDECOLUMNS`, `SHOWCOLUMNS`, `UNCHECKMASTERGRID`, `TIMERON`, `PROGRESSON`, `PROCESSSTART`, `CREATETEXTFILE1`, `CREATETEXTFILE2`, `BEEP`, `ENABLEALLCONTROLS`, `DOCK1`–`DOCK5`, `FINDNODE` kao komanda (kao kolona SQL-a i dalje može doći), `PRINTDET`, `MASTERMOVENEXT`, `SLOPP`, `POSALJIRACUNENASERVER` i srodne, `FISKALIZACIJAN`, `FISKALIZACIJAPONUDE`. Nula u korpusu ne znači nulu u bazi `netgenxml`. Za prvi rez nisu blokada. Za potpuni interpreter jesu, jer jedan XML iz baze može biti jedini korisnik.

`FOREACH*` × (158+143+109) su sve na dijalogu ako autori poštuju engine. Ako netko istu komandu stavi na `BUTTONS` podloge, NetGen je pojede u tišini. Adapter koji ih na podlozi **provede** bio bi ispravniji i nekompatibilan.

## 8. `@@` tokeni

Najčešći (samo top; punih 80 je u JSON-u `at_tokens`):

| Token | Broj | Odakle vrijednost |
|---|---|---|
| `@@id` | 5294 | kolona `id` tekućeg reda, inače 0 |
| `@@operateriid` | 4467 | host, `cls1.OperateriID` |
| `@@trgovinaid` | 1546 | kontrola ili kolona, ne ugrađeni token |
| `@@robaid` | 1535 | isto |
| `@@partneriid` | 1065 | isto |
| `@@datumod` | 954 | kontrola, često date |
| `@@datumdok` | 913 | |
| `@@datum` | 901 | |
| `@@datumdo` | 772 | |
| `@@vrstadokid` | 694 | |
| `@@node` | 681 | stablo, dio taga prije `:` |
| `@@kolicina` | 672 | |
| `@@hrkadroviid` | 664 | |
| `@@stavke` | 601 | grid u formi, `@gridstavke` se vidi i kao `@@stavke` ovisno o zamjeni — ovdje je sirovi token u XML-u |
| `@@searchboxrobaid` | 273 | tekst tražilice, ne id |
| `@@searchboxpartneriid` | 181 | isto |
| `@@searchboxorgshemaid` | 177 | isto |
| `@@xmlname` | 130 | ime granule |
| `@@gridmastertrgovinaid` | 117 | ćelija master grida |

Ugrađeni tokeni koje kod zamjenjuje, a u top 40 nisu ili su rijetki: `@@detid`, `@@editedcolumn`, `@@lastvalue`, `@@selectedcolor`, `@@tempstring`, `@@focusedcontrol`, `@@selectedfile`, `@@selectedpath`, `@@dblclickedcolumn`, `@@version`, `@@table`, `@@jupitersrvpath`, `@@opisniparametri`. Oni su i dalje dio jezika. Korpus ih samo rijetko piše eksplicitno jer ih engine ubaci sam (`@@selectedfile` nastaje iz `SHOWOPENFILEDIALOG`, kojeg ima 1).

Imena `@@trgovinaid` i slična **nisu** ključne riječi. To su `NAME` kontrola. Katalog ključnih riječi su pravila iz dokumenta 02 (`SrediSQL`, `SrediSQL2`, `NadjiVrijednostuPrevParametrima`), ne top lista.

## 9. Kod ima, korpus nema

37 atributa koje `.vb` traži, a u 609 fajlova ih nema nijednom. Najvažniji:

`DXGRID`, `AUTOMATICSORTDET`, `BEFORECELLEDITEDDET`, `COLUMNAUTORESIZE`, `COLUMNHEADERFORMAT`, `COMBOCLICK`, `COMBOCOLUMNS`, `CUSTOMCONNECTIONSTRING`, `DEFAULTCONNECTION`, `DEFAULTACTIVECOLUMN`, `DETMOVELAST`, `MASTERMOVELAST`, `ENTERASTAB`, `FONT`, `GROUPBYBOXDETVISIBLE`, `HELPKEY`, `INPUTMASK`, `MAXIMUMSIZE`, `MINIMUMSIZE`, `MAXLINES`, `AUTOSIZE`, `MENU`, `NEGATIVERED`, `NOTZERORED`, `NFCSQL`, `ONACTIVATED`, `ONCLOSING`, `ORIENTATION`, `RELATION`, `RESIZE`, `SQL3`, `SQL4`, `TIMER`, `TRANSLATEDETCOLUMNS`, `TREEDRAGDROP`, `WORDWRAP`.

`ONACTIVATED` i `ONCLOSING` kod čita na formi (`Form2.vb` 1044–1045). U korpusu ih nema. Grana je živa, samo je se ovi autori nisu sjetili. `RESIZE`, `CUSTOMCONNECTIONSTRING`, `RELATION`, `SQL3`/`SQL4` isto.

`NEGATIVERED` / `NOTZERORED` dokumentacija opisuje kao podržane (`NetGen-XML-Pravila.md`). U ovom korpusu se ne koriste. Adapter ih ipak mora imati ako je cilj „što kod umije“, ne „što ovih 609 fajlova slučajno dira“.

## 10. Korpus ima, kod ne traži

156 imena. Većina su tipfeleri (`HIGHT`, `WUDTH`, `CAPITON`, `O1NLOAD`, `ONLOADEDD`, `ONLOSFOCUS`, `STYILE`, `GROUPBOXVISIBLE`, `CMD1CAPTION1`…). Najveći koji nisu očiti tipfeleri:

| Atribut | Broj | Datoteka | Čitanje |
|---|---|---|---|
| `TEXT` | 17971 | 331 | Callisto. Ne crta se |
| `BORDERSTYLE` | 898 | 274 | ne |
| `BOLD` | 828 | 157 | ne. Debeli font na labeli u NetGenu ne dolazi odavde (osim ako je u `STYLE`, što nije provjereno za svih 828) |
| `AGGREGATEFUNCTION` | 518 | 34 | ne; vidi `AGREGATEFUNCTION` |
| `CHECKBOX` | 391 | 90 | ne (postoji `CHECKBOXVISIBLE`) |
| `REPORT` | 299 | 191 | ne |
| `CHILDFORM` | 156 | 65 | ne u lookupu. **Neprovjereno** da li neki string-compare ipak gleda `CHILDFORM` |
| `CANCELONSUBMIT` | 114 | 28 | ne |
| `AUTOFITCOLUMNS` | 68 | 62 | ne |
| `KOMENTAR` kao atribut | 11 | 9 | element `KOMENTAR` postoji; atribut se ne čita |

`CHILDFORM` na 65 datoteka je kandidat za ručnu provjeru prije nego se proglasi mrtvim. U ovoj fazi: lookup ga ne zove.

## 12. Potpuni popis tolerancija (iteracija 2)

Odluka: adapter radi ono što radi današnji `On Error Resume Next` i `InStr`, ne „ispravlja“ autora. Tri sloja.

### 12.1 Kod sam traži pogrešno slovkanje

Ako autor napiše ispravno, ništa se ne dogodi. Adapter mora tražiti **lijevi** stupac.

| Kod traži | Ispravno bi bilo | Dokaz |
|---|---|---|
| `CLEARCOLUMS` | CLEARCOLUMNS | `Form1.vb` 3295, 3308; `Form2.vb` 1703; `Grid.vb` 32 |
| `AGREGATEFUNCTION` | AGGREGATEFUNCTION | `Grid.vb` 97 |
| `PERMISIO` | PERMISSION / u korpusu `PERMISSIO` | `Form1.vb` 3395, `Form2.vb` 1097 |

U korpusu (609 jedinstvenih): `AGREGATEFUNCTION` (jedno G, kod čita) 1951 puta; `AGGREGATEFUNCTION` (dva G, kod ne čita) 518 puta na 34 datoteke. `PERMISSIO` je na 2 `PROJECT` čvora i zato prava ne uključi.

### 12.2 `TYPE` nije enum

Prvi podstring u `Form2.vb` ~1256–2737. Posljedice koje korpus stvarno dira:

| Napisano | Broj | Što nastane |
|---|---|---|
| `checkbox`, `vb.checkbox` | 404 + 56 | `check` (sadrži `check`) |
| `datetime` | 35 | `date` |
| `trazilica` | 49 | tražilica (grana traži i tu riječ) |
| `cmeditvb6.curmedit` | 8 | broj, jer sadrži `cur` |
| `vb.textbox`, `tmedit2vb6.textmedit2` | 8 + 2 | text, jer sadrži `text` |
| `cbex3.comboboxex3` | 1 | combo |
| `buttonnevrijedi` | 2 | gumb (sadrži `button`) |
| `b_utton`, `bxutton`, `b1utton`, `buttton` | 15+11+3+2 | **ništa** (nema podstring `button`) |
| `t1ext`, `tekst` | 14+2 | ništa |
| `g_rid` | 4 | ništa |
| `datetiem` | 5 | ništa |
| `picture` | 2 | ništa (`picturebox` se traži cijeli) |
| `int` | 3 | ništa |
| prazan `TYPE` | 1 | ništa |

Adapter koji „popravi“ `b_utton` u gumb crta kontrolu koje u NetGenu nema.

### 12.3 Atributi u korpusu koje `GetNamedItem`/`Attributes` ne traže

Skripta je našla 156 takvih imena. Ovo su sva s najmanje 2 pojave. Adapter ih ne čita i ne mapira na Carpo svojstvo. `TEXT` je Callisto šum (17971).

| Atribut | Broj | Datoteka | Vjerojatni tipfeler od |
|---|---|---|---|
| `TEXT` | 17971 | 331 | nije tipfeler, duplikat captiona |
| `BORDERSTYLE` | 898 | 274 | WinForms svojstvo koje ova petlja ne čita |
| `BOLD` | 828 | 157 | |
| `AGGREGATEFUNCTION` | 518 | 34 | `AGREGATEFUNCTION` |
| `CHECKBOX` | 391 | 90 | `CHECKBOXVISIBLE` |
| `REPORT` | 299 | 191 | |
| `CHILDFORM` | 156 | 65 | nije tipfeler; lookup ga nema. Port ga i dalje ignorira dok se ne nađe drugi pristup |
| `CANCELONSUBMIT` | 114 | 28 | |
| `AUTOFITCOLUMNS` | 68 | 62 | |
| `ALOWEMPTY` | 127 | 52 | `ALLOWEMPTY` |
| `HIGHT` | 43 | 22 | `HEIGHT` |
| `WUDTH` | 23 | 12 | `WIDTH` |
| `MAXLENGHT` | 21 | 9 | `MAXLENGTH` |
| `DECIMAL` | 20 | 10 | `DECIMALS` |
| `SKIPDEFAULTLOAD2` | 19 | 10 | `SKIPDEFAULTLOAD` |
| `STILE` | 17 | 8 | `STYLE` |
| `CMD1CAPTION1` … `CMD8CAPTION1` | 16 svaki | 16 | `CMDnCAPTION` |
| `ONCLICKKK` | 16 | 15 | `ONCLICK` |
| `SQLOLD` | 12 | 4 | stara kopija, ne čita se |
| `STYILE` | 12 | 4 | `STYLE` |
| `ANCHOR2` | 11 | 3 | |
| `KOMENTAR` (kao atribut) | 11 | 9 | |
| `CAPITON` | 9 | 5 | `CAPTION` |
| `ONLOADEDD` | 9 | 8 | `ONLOADED` |
| `O1NLOAD` | 8 | 6 | `ONLOAD` |
| `ONLOSFOCUS` | 8 | 1 | `ONLOSTFOCUS` |
| `EDITCOLUMNS22` | 7 | 6 | `EDITCOLUMNS` |
| `GROUPBOXVISIBLE` | 7 | 6 | `GROUPBYBOXVISIBLE` |
| `ONCELLEDITED22` | 7 | 6 | `ONCELLEDITED` |
| `B_UTTONS` | 6 | 6 | `BUTTONS` |
| `ENABLE` | 6 | 1 | `ENABLED` |
| `ONCHANGED` | 6 | 3 | `ONCHANGE` |
| `ONCLOSE` | 6 | 2 | `ONCLOSED` |
| `ONCLOSED2` | 6 | 2 | |
| `HEIGTH` | 5 | 5 | `HEIGHT` |
| `ONLOAD5` | 5 | 5 | |
| `ONLOSRFOCUS` | 5 | 4 | `ONLOSTFOCUS` |
| `SETDEFAULTSKIP` | 5 | 5 | `SETDEFAULTS` |
| `VISIBE` | 4 | 4 | `VISIBLE` |
| `DESIMALS` | 4 | 1 | `DECIMALS` |
| `INCLUDEE` | 4 | 4 | `INCLUDE` |
| `ONCLICKSTARO` | 4 | 3 | |
| `GRUOPBYBOXVISIBLE` | 3 | 3 | `GROUPBYBOXVISIBLE` |
| `ONCLICKKKKK`, `ONCLIOCK` | 3+3 | 3 | `ONCLICK` |
| `ONLOADED33`, `ONLOADSTARO` | 3+3 | 3 | |
| `MAXLENGH` | 3 | 1 | `MAXLENGTH` |
| `DEFALUT` | 2 | 2 | `DEFAULT` |
| `PERMISSIO` | 2 | 2 | `PERMISIO` |
| `SKIPDEFAUILTLOAD` | 2 | 2 | `SKIPDEFAULTLOAD` |
| `VISBLE`, `VIZIBLE` | 2+1 | | `VISIBLE` |
| `O_NLOAD`, `NOLOAD2`, `ONLOAD666`, `ONLOADDD` | 2 | | `ONLOAD` |
| `P_OPUP` | 2 | 1 | `POPUP` |
| `SCROLLBAR` | 2 | 2 | `SCROLLBARS` |
| `GROUPBYBOX` | 2 | 2 | `GROUPBYBOXVISIBLE` |

Imena s jednom pojavom (ostatak do 156) su u `tools/out/netgen-xml-katalog.json`, ključ `corpus_attributes_not_in_code_lookup`. Ne nabrajaju se ovdje jer su jednokratni šum (`KASA`, `V.PDV`, `SQLL`, …).

Posebno, viđeno u fajlu a nije u `GetNamedItem` izvan komentara u `Resource1`: `GRIDALLOWEDIT` i `GRIDALLOWEDITCOLUMNS` na `definitions/netgen/BIOVEGA/OznakaKL.xml`. Ne uključuju uređivanje ćelija. Uređivanje ide preko `EDITCOLUMNS`.

### 12.4 Atributi koje kod traži, a ovih 609 fajlova nema

37 imena. Port ih i dalje čita, jer XML iz baze `netgenxml` može biti jedini korisnik. Popis: `AUTOMATICSORTDET`, `AUTOSIZE`, `BEFORECELLEDITEDDET`, `COLUMNAUTORESIZE`, `COLUMNHEADERFORMAT`, `COMBOCLICK`, `COMBOCOLUMNS`, `CUSTOMCONNECTIONSTRING`, `DEFAULTACTIVECOLUMN`, `DEFAULTCONNECTION`, `DETMOVELAST`, `DXGRID`, `ENTERASTAB`, `FONT`, `GROUPBYBOXDETVISIBLE`, `HELPKEY`, `INPUTMASK`, `MASTERMOVELAST`, `MAXIMUMSIZE`, `MAXLINES`, `MENU`, `MINIMUMSIZE`, `NEGATIVERED`, `NFCSQL`, `NOTZERORED`, `ONACTIVATED`, `ONCLOSING`, `ORIENTATION`, `RELATION`, `RESIZE`, `SQL3`, `SQL4`, `TIMER`, `TRANSLATEDETCOLUMNS`, `TREEDRAGDROP`, `WORDWRAP`, `XML`.

### 12.5 Ostale tihe tolerancije u kodu

- Nedostajući atribut: `GetNamedItem(...).Value` baci, `On Error Resume Next` nastavi, vrijednost ostane prazna ili prethodna. Zato `STARTUPSTORA` preživi ako `ONLOAD` ne postoji (`Form1.vb` 3367–3368).
- `CMDxCAPTION` bez dvotočke: `IndexOf(":")` baci i taj gumb ostane prazan. To **nije** mekana tolerancija. Adapter neka isto preskoči gumb i upiše u log, ne ruši granulu (današnji `On Error Resume Next` oko petlje).
- Komande se uspoređuju s `ToUpper`. `ok` i `OK` su isto (`JeziciNG.xml` ima `ONCLICK="ok"`).
- `##` u lancu je prazan segment između dva `#` i preskače se.
- SQL koji još sadrži `@@` se ne šalje (`Class1.vb` 113–118).

## 13. Što katalog govori o redoslijedu izrade

Pokriće po broju kontrola, ne po egzotici:

1. `label`, `button`, `text`, `number`, `combo`, `date`, `grid`, `search`, `frame` — to je >95% `INPUT`.
2. Komande `CLOSE`, `OK`, SQL-segment, `REFRESHCONTROL`, otvaranje forme po imenu, `REPX`, `PRINTMASTER`.
3. `@@id`, `@@operateriid`, parametri po `NAME`, `!!`, lijepljenje `@ime=`.
4. Tek onda `check`/`checkbox`, `radio`, `multidrop`, `tree`, `FOREACH*`, `ASSEMBLYLOAD`, fiskalizacija.

Egzotika s jednom pojavom (`SHOWOPENFILEDIALOG`, `CREATETEXTFILE3`) ne smije ući u prvi milestone, ali parser je ne smije ni odbaciti: nepoznati segment na dijalogu je SQL.
