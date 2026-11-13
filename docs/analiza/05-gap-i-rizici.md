# 05 — Rupe i rizici

**Iteracija 2.** Rupe koje su bile „pitaj korisnika“ sada su odluke: SQL tekst ostaje, tipfeleri se šute, multi-select tablica ostaje, fiskalizacija i DMS i `ASSEMBLYLOAD` se portaju, raspored smije odstupati. Donja tablica i dalje opisuje što Carpo model sam ne napravi. To više nije razlog da se izbace iz opsega.

„Carpo to ne umije“ ovdje znači: postojeći `JRoot` + `Events.Checking` + `ControlInit` nemaju to ponašanje. Ne znači da se na DevExpressu ne može nacrtati. Većina rupa je semantička, ne grafička.

## 1. Što NetGen radi, a Carpo model nema

| Ponašanje | Dokaz | Zašto Carpo nije dovoljan |
|---|---|---|
| Apsolutni `TOP/LEFT/WIDTH/HEIGHT` / 14 | `Form2.vb` 1093–1094, 2945–2950, `FakPretvorbe=14` | Carpo je tablica `row`/`col` u `LayoutControl` |
| `TYPE` kao podstring, redoslijed grana | `Form2.vb` ~1256–2737 | Carpo element je točno ime (`textedit`, ne `text`) |
| Komandni string `#` / `~` / `:forma` / `==sql` | `Form1.vb` 4543+, `Form2.Rafalno` | Carpo akcija je stablo XML-a, ne interpreter stringa |
| `@@biloKojeIme` | `Class1.SrediSQLPosljeNextForme` 740–853 | Carpo `@@` je uski popis pub-var (`Variables.cs`) |
| Lijepljenje `@ime=vrijednost` i `!!` | `Form2.vb` 3367–3388, `Form1.vb` 5449–5514 | Carpo parametri su tipizirani `<parameter>` |
| `#OK` `+close` `#FOCUS(` u tekstu SQL greške | `Class1.vb` 170–205 | Carpo ne parsira `SqlException.Message` kao komandu |
| Preskok SQL-a koji još sadrži `@@` ili počinje s `@` | `Class1.vb` 105–118 | Carpo bi bacio ili poslao tekst |
| `TotalGenMultiSelectTemp` | `Form1.vb` 4932–4935 | Carpo multi-select ne puni tu tablicu |
| `yearmonth` stablo 2005–2009 | `netgen/Form1.vb` 4527–4534 (`For g = 2005 To 2009`) | nema ugrađenog kalendara |
| `FILLEXPLORER` s 6 vrijednosti | `Form1.vb` 5445–5507 | `control populate` je jedna operacija |
| DB clipboard + prisila na formama s `filter` u imenu | `Form2.vb` ~1102 | `savevalue="app"` je drugi spremnik |
| `ONLOAD2` riječi `enable`/`disable`/`visible`/`refresh` | `DodatnoPostimajKontrolu` | Carpo nema „vrijednost kolone je glagol“ |
| `COMBO` kolona 0 / kolona 1 bez imena | `NetGen-Kontrole.md`, grana combo | Carpo traži `valuefield`/`displayfield` |
| Tražilica `tablica#id#prikaz%` | `Form3` | `searchedit` očekuje `datalist` |
| `FOREACH*` mrtav na podlozi | `Form1.vb` 6841–6850 | Carpo foreach radi ako se napiše. Kopirati rupu ili ne — odluka |
| Late-bound `ASSEMBLYLOAD` / `totalgen` | `Form2.vb` ~7424, `Form1.vb` ~6681 | `comdll` ima drugi potpis |
| Fiskalizacija `Raverus` / `SLOFISKAL` | `Form1.vb` ~342–360 | Carpo `ZKI`/`JIR` je drugi kod |
| DMS → `netgenxml` → disk | `Form1.ProcitajXMLzaPodlogu` | Carpo `FileFinder("Carpo")` traži drugi folder (`RemoteDllPath\Carpo`) |
| Janus format condition, card view | `Form1.vb` 6163–6179, 7544 | DX grid to umije, ali nije povezano na te atribute |
| Greška se guta (`On Error Resume Next`, ~269 mjesta prema `CLAUDE.md`) | cijeli VB | Carpo `bool.Parse` i `XmlSerializer` pucaju |

## 2. Razlike u ponašanju koje će korisnik vidjeti

### Raspored (prihvaćena razlika)

NetGen forma kultura je 7085×4104 jedinica (`/14` ≈ 506×293 px), kontrole na fiksnim koordinatama. Carpo prepis iste granule je `width="450"` i retci. Korisnik je prihvatio da ekran ne bude isti piksel.

Što ide 1:1: vrsta kontrole, `CAPTION` (uz `_`, `!!`, završni `*`), `ENABLED`/`VISIBLE` kad su `true`/`false`, `TABINDEX`, kolone grida (`NAME`/`INDEX`, `FORMAT`, `VISIBLE`, `AGREGATEFUNCTION` s jednim G), combo stupac 0/1.

Što se dokumentira korisniku, a ne lovi se kao bug:

- drugi razmak i font DevExpress 26 naspram WinFormsa iz 2005.
- `frame`/`group` ne drže djecu apsolutno unutar okvira
- dvije kontrole na istom `TOP` mogu se u DX layoutu razdvojiti
- `FakPretvorbe` se **ne** koristi za crtanje. Tko ga ipak upotrijebi, dijeli s 14, ne s 15

Poznata razlika ide u release bilješku granule, ne u iznimku.

### Redoslijed događaja

NetGen dijalog (`NetGen-Tok.md`, potvrđeno čitanjem `Button1_Click` i opisa `ProcitajXML`):

1. `ONLOAD` SQL **prije** crtanja kontrola. Prvi red ide u clipboard i u kontrole po imenu kolone.
2. `ONLOAD2`.
3. Petlja `INPUT`.
4. Ako nema nijedne kontrole, forma se ne prikaže, odmah `ONLOADED`.
5. Inače `ShowDialog`, pa `ONLOADED`.
6. Klik CMD1: `PokupiParametre`, pa `ONSUBMIT`, pa (ako nema `!!` i nije `SELECT|INSERT|…`) parametri se nakače, forma se zatvori, podloga gleda `FILLEXPLORER`.

Carpo: `init` → `load` → `shown`, kontrole postoje prije `load`, `datarun` ne puni kontrole „po imenu kolone“ nego po `datavalue`. Validacija je prije snimanja. NetGen validacija je često `if @@x is null exec spRaiserror` **unutar** `ONCLICK`, pa tek `#OK`. Ako adapter prvo odradi `validate`, pa SQL, `spRaiserror` se ne dogodi istim redom i `#FOCUS` u poruci ne vrati fokus.

### Validacija

NetGen nema `required`. Obaveznost je SQL ili `ALLOWEMPTY=false` koji na praznom combu pokuša `SelectedValue=1` (`Form2.vb` 3455–3468). To je iznenađenje: prazan combo bez `ALLOWEMPTY` pošalje `1`, ne `null`, ako vrijednost 1 postoji. Carpo `required` blokira klik. Adapter koji doda `required` gdje ga XML nema zaustavit će granule koje su se oslanjale na `SelectedValue=1`.

### Grid

- Checkbox NetGena nije DX `multiselect`. `FOREACHSELECTED` gleda kvačice, `FOREACHBLUEROW` gleda `SelectedItems`.
- `@@id` bez kolone `id`: treća ćelija (Janus) ili prva (DX). Carpo `key` to ne radi.
- Enter, filter red, group box, frozen columns, total red: DX 26 ih ima, ali defaulti nisu Janus defaulti. `CHECKBOXVISIBLE` default u kodu je uključen dok XML ne kaže `FALSE` (518 korijena ga gasi — znači autorima smeta default).
- Uređivanje ćelije: `BEFORECELLEDITED` može odbiti. DX `ShowingEditor` je blizu, nije spojeno.
- `AGGREGATEFUNCTION` (518 kolona, ispravno slovkanje) NetGen ne zbraja. Adapter koji zbroji „popravi“ izvještaj i razići se od produkcije.
- Širina: `/14`, plus poseban slučaj zadnja 4 znaka captiona (`Class1.vb` ~1047). `netgensizes` u Carpu je ručni atribut, ne čitač NetGen XML-a.

### Ispis

`PRINTMASTER` i `PRINTREPXMASTER` su ista funkcija `IspisGRIDREPX` (`Form1.vb` 4945–4956). Stari ispis je komentar. Zaglavlje je string s `@@`, `##` i `!!`. Carpo `print` zna `repx/…`, ne zna NetGen zaglavlje ni `PAGEORIENTATION` na `PROJECT` bez novog koda. Late-bound `Spin.Printing.v23` naspram Carpo `Spin.Printing.v26` — **neprovjereno** jesu li potpisi isti. To je rizik prije nego što se prvi `::repx/` pusti.

### Varijable i praznine

`ExecSql` ne pošalje SQL u kojem je ostao `@@`. Korisnik vidi tihi preskok (CB log), ne grešku. Carpo bi vjerojatno bacio. Adapter koji „popravi“ tišinu u dijalog promijeni rad na lošem XML-u.

Datum s ugašenom kvačicom šalje `null`. Broj prazan šalje `0`. Tekst prazan šalje `''` (ne `null`), apostrof kao `¸¸`, zarez kao `~~`. `~~` se u `ExecSql` vraća u zarez, `¸¸` u `''` (`Class1` 87 i 850). Ako adapter koristi prave parametre, `¸¸` više nije potrebno i procedure koje **same** traže `¸¸` (ako ih ima — **neprovjereno**) puknu. Ako adapter lijepi string, SQL injection ostaje kakav jest. To je svjesno: cilj je isti ugovor, ne čišći SQL.

## 3. Rizici

1. **Lažni prijevod.** Najveći rizik ove faze nije Janus nego odluka da se NetGen XML pretvori u Carpo XML. Prepisi kultura i kartice pokazuju da ljudi pritom mijenjaju SP, polja i tok. Korisnici koji „i dalje pišu NetGen“ dobili bi drugi program.

2. **Dvije DevExpress verzije.** NetGen 13.1.7 (i to samo za `multidrop`, `dxgrid`, preview) i Carpo 26.1.3. `CheckedComboBoxEdit` API između 13 i 26 nije čitan red-po-red. `GetCheckedItems()` u `Form2.vb` 3446 može se zvati drugačije. To treba provjeriti na Windows buildu, ne pretpostaviti.

3. **Janus DLL-ova nema u gitu**, mapa `DevExpress131/` je prazna (0 datoteka). Usporedba starog grida s DX 26 ostaje neprovjerena dok DLL-ovi ne dođu. Testna baza je `sql-ai` / `spin`. `jupiter2022` je produkcija i ne koristi se.

4. **Host.** `SetParent` na Jupiter handle, zabranjen vlastiti message loop (`CLAUDE.md`). Novi projekt koji se testira kao običan `Application.Run` može pasti na isti `StackOverflow`. TestShell to već rješava tako da granula sjedi u panelu (`pnlShell.Handle`). Taj put je bliži od novog exe-a.

5. **DMS i baza `netgenxml`.** 609 fajlova u gitu nisu sve granule na terenu. `DXGRID`, `CUSTOMCONNECTIONSTRING`, `NEGATIVERED` imaju kod i nula pogodaka ovdje. XML iz baze može biti jedini korisnik. Katalog nije produkcijski inventar.

6. **UTF-16 nije bio kvar.** Deset fajlova (`MPKase.xml` i srodni) ima BOM `FF FE`. `XmlDocument.Load` ih učita. Prva skripta ih je krivo čitala kao cp1250. Port ne popravlja XML. Ako `Load` baci, ista `MsgBox` poruka kao `Form1.vb` 3264–3265.

7. **`On Error Resume Next` je značajka.** Atribut koji fali, krivi `TYPE`, `HIGHT`, `ALOWEMPTY` — produkcija šuti. Adapter na C# s iznimkama bit će glasniji i „pokvariti“ granule koje su godinama šutjele. Prva verzija treba logirati i ići dalje, ne bacati.

8. **Prava i prijevod.** `PERMISIO` (jedno S) uključuje `ssPermission`. U korpusu je `PERMISSIO` (dva S) na 2 korijena i zato ne radi. `JupiterTranslator10` je vanjski. Carpo ima vlastiti translator. Dva sustava prijevoda na istoj riječi nisu uspoređena.

9. **Lozinke u logu.** `Form1.Run` piše password u CB log (`CLAUDE.md`). Novi logger ne smije to prepisati u `JupiterLogs` bez redakta koji `JupiterLog.Sredi` već radi. Lako se previde pri portu `ExecSql` Catch-a.

10. **Kružne reference Carpa.** Čak i kad `Carpo.NetGen` bude tanak, ne može se testirati na CI dok se ne riješi `HintPath` na `SharedAssemblies` (dokument 07). Rizik da se „faza 2“ krene pisati bez ijednog zelenog builda.

11. **Multi-select temp tablica** je dijeljeni globalni spremnik po `suser_sname()` i `operateriid`. Dva klika odjednom (dva Jupiter prozora, isti SQL login) brišu jedno drugome id-eve. To je današnji bug. Adapter koji ga sačuva sačuva i bug. Adapter koji ga „popravi“ lomi procedure. Treba odluku (pitanje 8 u sažetku).

12. **Fiskalizacija i `ASSEMBLYLOAD`** (22 + 55 pogodaka) jesu u opsegu (odluka 2). Diraju novac i tuđi DLL kojeg nema u gitu. Ne ulaze u prvi smoke (`OznakaKL`, `JeziciNG`). Kad komanda dođe, a DLL fali, port radi kao `Form1.vb` 360–365 i 4600–4611: CB log i prekid lanca, ne tihi nastavak i ne Carpo `ZKI`. Pravi poziv porezne uprave nije automatski test.

## 4. Što nije rupa

- Grid, combo, date, text, check, button, tree, tabs, print REPX, `foreachrow` kao **ideja** — Carpo i DX 26 to imaju.
- Host `Run(server, database, …, parentHandle)` — `Starter` već ima taj potpis.
- `netgenfields` / `netgensizes` / `netgenstyles` — kozmetika kolona, može se zvati iz adaptera tražilice, nije loader.
- Logiranje preko `JupiterLogger` — Carpo.Core već referencira `JupiterLogger.dll`.

## 5. Otvoreno (neprovjereno u ovoj fazi)

- Točan sadržaj `SpinDMSUtil.NetGenFinder` naspram `CarpoFinder` (nema izvora).
- Je li `Spin.Printing.v26` binarni kompatibilan s pozivima koje NetGen radi na v23.
- `CHILDFORM` (156 pojava, 65 datoteka): lookup ga ne zove; nije rađen puni string-search svih parcijalnih usporedbi.
- `STYLE` na `COLUMN` (11): `Grid.Formatiraj` ga u linijama 89–101 ne čita. Može ga čitati `CustomFormatKolona`. Nije zatvoreno.
- Ponašanje `yearmonth` na produkcijskim bazama (raspon godina).
- Koliko XML-ova živi samo u `JupiterXML.dbo.netgenxml` i nema ih u gitu.
