# C4: kontrole dijaloga po tipu i atributu

Usporedba `Form2.ProcitajXML` / `SrediKontrolu` / `PokupiParametre` / `RefreshControls` (NetGen) s `Carpo.NetGen.Ui.Dijalog` (DevExpress 26.1). Brojevi su iz korpusa od 584 jedinstvene granule (`definitions/netgen`).

Pravilo 50-50: kad NetGen i Carpo rade drukčije, a nijedno nije očito ispravno, ostaje Carpo. Kad je NetGen ponašanje ono što korisnik vidi i na što se XML oslanja, Carpo ga prati.

## Raspored (LAY-01)

| Atribut | Korpus | NetGen | Carpo prije | Sada |
|---|---:|---|---|---|
| `PARENT` | 500 kontrola, 64 granule (424 gumba u okvirima) | `Glbl.Parent = Controls(PARENT)`, LEFT/TOP su relativni prema okviru | kontrola na formi s koordinatama okvira, pa na krivom mjestu | kontrola (i njen natpis `lbl…`) ide u `GroupControl`/`PanelControl`. Okvir definiran kasnije: VB ostavi kontrolu bez roditelja; Carpo je smjesti (nakon svih kontrola) |
| `DOCK` | 965, 125 granula (grid `5` = Fill 240, okviri `1`/`2`) | broj → `DockStyle` (riječ baci iznimku pa se preskoči) | ignoriran | isto; Fill ide zadnji pa popuni ostatak |
| `ANCHOR` | 1 752, 74 granule (`6`, `13`, `9`, `15`) | broj → `AnchorStyles`; postavljen nakon Docka ga poništi (dxgrid obrnuto) | ignoriran | isto, s istim redoslijedom |
| Gumbi CMD1–CMD6 | | `Panel1` na dnu (33), Button1..6 na x = 12 + 81·(n−1), prazan natpis skriven | gumbi na dnu forme, preklapali su se s Fill gridom | ploča `pnlGumbi` na dnu, ista mjesta |
| `STYLE` / `SIZE` | 1 089 (`1` = bold) | `New Font(FONT, SIZE, STYLE)` | ignorirano | isto (`BOLD` kao riječ ne prolazi, kao u VB-u) |
| `FORECOLOR` / `BACKCOLOR` | 590 | `#rrggbb` ili ime s popisa; `BACKCOLOR="red"` je žuta (3141) | ignorirano | isto, uključujući žutu |

Kontrole u okvirima: VB `PokupiParametre`, `RefreshControls` i HOTKEY idu samo po `Me.Controls`, pa kontrola u okviru nije parametar. U korpusu su u okvirima uglavnom gumbi; za 25 tekstova, brojeva i radija Carpo ih vidi (50-50 → Carpo).

## Ulaz i vrijednosti

| ID | Korpus | NetGen | Carpo prije | Sada |
|---|---:|---|---|---|
| **TXT-01** | svako tekstualno polje bez DEFAULT | `SrediKontrolu` stavi CAPTION, ali TeksT/NumerBox/SearchBox ga odmah obrišu (2008, 2084, 2144) | CAPTION je ostajao u polju i išao u parametar | prazno ili DEFAULT; broj 0 → DEFAULT → VALUE; `null` je prazno |
| **DAT-01** | 352 SHOWCHECKBOX, 49 granula; svi datumi | `DTPicker.Value = Now`, DEFAULT, Checked; SHOWCHECKBOX gasi, CHECKED pali; neoznačen je `null` | datum bez DEFAULT bio je prazan, pa `null` u SQL-u | kao VB; CUSTOMFORMAT je i prikaz i oblik parametra (HH:mm, yyyyMM01, yyyyMMdd HH:mm). `@@datum` u gumbu/lancu je `'yyyyMMdd'`, ne prikazani tekst |
| **MDR-01** | 42 MULTIDROP, 20 granula | CheckedComboBoxEdit iz SQL-a, parametar `@ime='id~id'`, ONLOAD/rezultat označe id-eve | prazan, parametar je bio tekst | kao VB |
| **HOT-01** | 1 036 HOTKEY, 139 granula (F2, F5, F8) | F1–F10 i `*`: gumb klik, combo/broj/tražilica/grid fokus | ignorirano | kao VB |
| **BTN-03** | | gumb s jednim SQL-om → `RefreshControls` | rezultat se odbacivao | rezultat se primijeni, greška prekida |

## Grid dijaloga

| ID | NetGen | Sada |
|---|---|---|
| GRD-01 | `CreateGridColumns` s EDITCOLUMNS (može biti sp/select) | `GridIzgled.Primijeni` (natpisi bez širine, skriveni `*ID`, formati, uređivanje) |
| GRD-02 | `TOTALROW` (zbroj decimalnih stupaca) i `CHECKBOXVISIBLE` (kvačica reda) | footer sa zbrojem; `CheckBoxRowSelect` osim kad je `CHECKBOXVISIBLE="FALSE"`; uređen red postane označen (8624) |
| FOR-01 | `FOREACH*` pomakne grid i izvrši lanac po retku | isto; `@(grid..stupac)` iz trenutnog reda (`SrediSQLsaKolonamaizGrida`) |

## NetGenDBClipboard (DBC-01)

| Što | NetGen | Carpo |
|---|---|---|
| Kada | `DBCLIPBOARD="true"` (32 forme), ili svaka forma s `filter` u imenu osim `DBCLIPBOARD="false"` (Form2 1099-1104) | isto (`Komande/DbClipboard`) |
| Čitanje | `select * from NetGenDBClipboard where operaterid=…`; greška (nema tablice) gasi pamćenje; vrijednost nadjača ONLOAD i ONLOAD2; ključ `ime.xml/kontrola` bez obzira na velika slova | isto, nakon `PrimijeniOnLoad` (`Ui/DijalogDbClipboard`) |
| Po tipu | kvačica `Checked`, tekst (`null` → prazno), broj, datum, combo (id), tražilica (id + `…caption`), multidrop samo bez ONLOAD vrijednosti | isto; `¸¸` se vrati u `'` (VB ga ostavlja u polju) |
| Pisanje | `PokupiParametre` na svaki događaj: `select count(*)` pa `insert` ili `update` za svaku kontrolu | piše se samo vrijednost koja se razlikuje od one u tablici (isti krajnji sadržaj, bez upita na svaki događaj); upis prolazi `DijalogTok.SmijePoslati` |

## STATUSSTRIP (STS-01)

`TEXT1..3` (23 kontrole, 9 granula: `@@version`, `Broj stavki:`, `@@gridstavke`, `Gornji prikaz sadrži @@rowcountgridrob redova`) se u NetGenu spreme velikim slovima i na svakom `RefreshControls` (4753) prođu kroz `SrediSQL2`, `SrediSQLPosljeNextForme` i stupce grida. Carpo je pokazivao samo `TEXT1` doslovno (`@@VERSION`). Sada `Ui/DijalogStatus` radi isto, poslije ONLOAD-a i poslije svakog rezultata SQL-a, a `@@VERSION` je `NetGen v` + verzija sklopa.

## Otvoreno

- `MULTILINE` (243 / 60 granula), `ENTERASTAB`, `PASSWORDCHAR` (4), `MAXLENGTH` (80 / 14): tekstualno polje je uvijek `TextEdit`.
- `VALUE="DATE"` na datumu (133): u VB kodu za `DTPicker` nije pronađeno čitanje; provjeriti gdje se koristi.
- Tab stranice (`TYPE="tab"`), `TIMERSQL` (1 granula).
- Grid dijaloga: `COLUMNCAPTION`/`COLUMNWIDTH` za grid (nije tražilica), `FROZENCOLUMNS`, `AUTOMATICSORT`, layout datoteke.

## Testovi

`lay01-raspored`, `hot01-f2`, `dat01-datumi` (`RasporedDijaloga`), `mdr01-multidrop`, `for01-foreachrow` (`DogadjajiDijaloga`), `Form2Izgled` pod Mono (dock, anchor, stil, boja, hotkey, mjesta gumba).
