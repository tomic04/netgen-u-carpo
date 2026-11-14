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
| **TXT-02** | `MULTILINE` (226 polja, 48 granula), `SCROLLBARS` 0/3, `PASSWORDCHAR` `*` (12, 4 granule), `MAXLENGTH` (~330 polja, 20 granula), `ENTERASTAB` (0) | `TeksT` (2045-2050); Enter u višelinijskom polju je novi red, osim uz ENTERASTAB "true" (Glbl_KeyDown) | (bilo otvoreno u ovom popisu) | `MemoEdit` s visinom iz XML-a, ScrollBars, WordWrap; PasswordChar i MaxLength na `TextEdit`; Enter kao VB |
| **DAT-02** | `VALUE="DATE"`/`DATE1`/`DATE2` na datumu (197 polja, 41 granula) | Form2 čita `VALUE` samo za broj (2088), `DTPicker` ga ne gleda | ignorirano | isto (nema čega prenijeti). `TYPE="tab"` nema u korpusu |
| **TRE-03** | `TYPE="tree"` na dijalogu (11 granula: ManagerCjenika, AOPNovi, JOPPD, MPOptimalneZalihe…) | `treeViewMoj` (2558-2601): SQL `sp…tree…:forma#tekst:naredba`, `DazovaIzStore` (id, roditelj, natpis, oznaka), `ExpandAll`; AfterSelect → ONCLICK; parametar `@ime='oznaka'` ili null (3667); POPUP na desni klik; "refresh" puni iznova (5071) | prazan `TreeList`, bez parametra i događaja | `Dijalog.NapuniStablo` preko istog `Stablo` (NaDijalogu) kao podloga; `ParametriSvi.Stablo`; ONCLICK na promjenu čvora; POPUP; refresh |
| **HLP-02** | HELPKOD projekta (16 granula) ili forme (2); gumb bez ONCLICK; TYPE help | `Form2_KeyDown` F1 i `Glbl_ClickButton` → `DajPokaziHelp` (JupiterHelp.exe -s -d -h -o -p) | dijalog nije imao pomoć | F1 i takvi gumbi pokreću JupiterHelp preko zajedničkog `JupiterPomoc` (kao podloga); bez koda ništa (VB bi pokrenuo s praznim `-h`) |
| **DEV-01** | WinForms ostaci u UI-u | — | `ContextMenuStrip` na stablu dijaloga, `MessageBox` za ISEMPTYORZEROALERT/CANCELBOX | DevExpress `DXPopupMenu` i `XtraMessageBox` (bez korisnika na CI-u pitanje je Cancel). Datoteke/mape/boja ostaju WinForms dijalozi, kao u Carpu; `WebBrowser` (3 granule) nema DevExpress zamjenu |

## Grid dijaloga

| ID | NetGen | Sada |
|---|---|---|
| GRD-01 | `CreateGridColumns` s EDITCOLUMNS (može biti sp/select) | `GridIzgled.Primijeni` (natpisi bez širine, skriveni `*ID`, formati, uređivanje) |
| GRD-02 | `TOTALROW` (zbroj decimalnih stupaca) i `CHECKBOXVISIBLE` (kvačica reda) | footer sa zbrojem; `CheckBoxRowSelect` osim kad je `CHECKBOXVISIBLE="FALSE"`; uređen red postane označen (8624) |
| FOR-01 | `FOREACH*` pomakne grid i izvrši lanac po retku | isto; `@(grid..stupac)` iz trenutnog reda (`SrediSQLsaKolonamaizGrida`) |
| **DGR-01** | `COLUMN` djeca grida (1 349 stupaca, 47 granula: Narudzbe, ManagerCjenika, ZahtjevZaNabavku, KomStanjeSkl…) i `CLEARCOLUMS` (8): Form2.vb 1798-1957 — POSITION, MAXLINES, WIDTH, CAPTION, FORMAT, VISIBLE, ALIGN, AGREGATEFUNCTION, pa CLEARCOLUMS pozicija (nadjača POSITION), boje | XML reader ih je bacao, svi stupci su se vidjeli pod imenom iz SQL-a | `NgControl.Kolone` → `GridDefinicija` (isti kod kao podloga, red Form2) → `GridIzgled.PrimijeniKolone`. CLEARCOLUMS skrije sve, i kvačicu (Form2 ju postavi prije, 1748), pa opisani stupci idu redom definicije. Osvježavanje ne slaže stupce iznova (RefreshGrida 5305-5315), pa korisnikova širina i redoslijed ostaju |
| **DGR-02** | `GROUPBYBOXVISIBLE` (1 281 grid, 236 granula; FALSE 1 102), `FROZENCOLUMNS` (10), `FILTERMODE` (6), `AUTOMATICSORT`, `ENTERKEYBEHAVIOR` 1 (22) | ploča za grupiranje uvijek vidljiva, ostalo ignorirano | `ShowGroupPanel` po CBool (prazno ostavi Janus True; tekst ploče Carpov), `Fixed` (ikona i kvačica se broje), red za filtriranje, `AllowSort`, Enter na sljedeći stupac dok se uređuje. ENTERKEYBEHAVIOR 2 (2 grida) nije vezan: Janus enum nije potvrđen |
| **DGR-03** | `@@editedcolumn`/`@@lastvalue` u ONCELLEDITED grida (Narudzbe `g_ugo_stavke`: `@kolona='@@editedcolumn'`), `@@dblclickedcolumn`; `BEFORECELLEDITED` (0 u korpusu) | `Glbl_CellEdited`/`Glbl_EditingCell`/`Glbl_DoubleClick` postave TrenutnaKolona (natpis), TrenutnaVrijednost, DblClickedColumn; SrediSQL2 2882 ih zamijeni. EditingCell s greškom otkaže uređivanje | dijalog ih nije postavljao, pa je `@kolona=''` išao u proceduru; BEFORECELLEDITED ignoriran | kontekst dobije natpis stupca i vrijednost (točka umjesto zareza za broj) prije SQL-a; BEFORECELLEDITED na `ShowingEditor`, greška ga otkaže |
| **CHK-01** | stupac upita `selectedid` u gridu dijaloga | `RucniRadNaGriduzaCheckiranjeRedova` (5435, nakon punjenja i RefreshGrida 5351): uz vidljivu kvačicu red s "1" u `selectedid` je označen | nije postojalo | isto, nakon svakog punjenja |

## NetGenDBClipboard (DBC-01)

| Što | NetGen | Carpo |
|---|---|---|
| Kada | `DBCLIPBOARD="true"` (32 forme), ili svaka forma s `filter` u imenu osim `DBCLIPBOARD="false"` (Form2 1099-1104) | isto (`Komande/DbClipboard`) |
| Čitanje | `select * from NetGenDBClipboard where operaterid=…`; greška (nema tablice) gasi pamćenje; vrijednost nadjača ONLOAD i ONLOAD2; ključ `ime.xml/kontrola` bez obzira na velika slova | isto, nakon `PrimijeniOnLoad` (`Ui/DijalogDbClipboard`) |
| Po tipu | kvačica `Checked`, tekst (`null` → prazno), broj, datum, combo (id), tražilica (id + `…caption`), multidrop samo bez ONLOAD vrijednosti | isto; `¸¸` se vrati u `'` (VB ga ostavlja u polju) |
| Pisanje | `PokupiParametre` na svaki događaj: `select count(*)` pa `insert` ili `update` za svaku kontrolu | piše se samo vrijednost koja se razlikuje od one u tablici (isti krajnji sadržaj, bez upita na svaki događaj); upis prolazi `DijalogTok.SmijePoslati` |

Zatvaranje (DBC-02): `Form2_FormClosing` 4095-4099 uz RSDBCLIPBOARD još jednom zove `PokupiParametre` (polja u CB), a `formClosingB` preskoči upis u tablicu. Isto je u `Dijalog.OnFormClosing`.

## STATUSSTRIP (STS-01)

`TEXT1..3` (23 kontrole, 9 granula: `@@version`, `Broj stavki:`, `@@gridstavke`, `Gornji prikaz sadrži @@rowcountgridrob redova`) se u NetGenu spreme velikim slovima i na svakom `RefreshControls` (4753) prođu kroz `SrediSQL2`, `SrediSQLPosljeNextForme` i stupce grida. Carpo je pokazivao samo `TEXT1` doslovno (`@@VERSION`). Sada `Ui/DijalogStatus` radi isto, poslije ONLOAD-a i poslije svakog rezultata SQL-a, a `@@VERSION` je `NetGen v` + verzija sklopa.

## Broj s FORMATMASK (NUM-02)

`FORMATMASK="#0"` (godina proizvodnje/baždarenja, 4 kontrole) nema grupiranje tisućica; Carpo je s maskom `n0` pokazivao `2.024`. Maska bez zareza sada je `f{DECIMALS}`, sa zarezom ostaje `n{DECIMALS}`.

## Slika (PIC-01)

`TYPE="picturebox"`: `LOCATION` (3 granule, UNC put do .jpg), vrijednost iz ONLOAD/ONLOAD2 (put ili http) i u rezultatu SQL-a `byte[]` u stupcu s imenom ili put u stupcu `<ime>location` (Form2 2710-2724, 4766-4795). Carpo je čitao samo `byte[]`; sada i put/URL (`PictureEdit.LoadAsync`).

## Log i Ctrl+F11 (LOG-01)

`Form2_KeyDown` 4167-4176: Ctrl+F11 i Ctrl+F6 pokažu `PREVPARAMETRI`, `PARAMETRISVI` i `PARAMETRIIZTRAZILICE` u CBLog prozoru (Form5); Ctrl+F6 umjesto toga daje HTML obrasca (`BF`), kojeg Carpo nema. Dijalog nije reagirao ni na jedno. Sada obje tipke upišu parametre u Carpo log i otvore njegov prozor, kao Ctrl+F11 na podlozi, koja sad upiše i `PARAMATERISAMASTERIDETAILGRIDA` (Form1 1376-1378). Polja s `PASSWORDCHAR` (MPKase) u logu imaju `'***'` (R120).

## Dozvole na dijalogu (DOZ-02)

`PERMISIO="true"` na FORM (Form2.vb 1097) čita `ssPermission` za objekt `xml.forma`, s `order by sort desc` (Form1 je od 2021. bez `desc`). Poslije `ONLOADED` redovi s imenom kontrole skriju ili pokažu kontrolu i natpis `lbl`+ime (`Vidljivost`) i zabrane ili dopuste je (`smije`); dopušten red pobjeđuje. `visible`/`enable` iz rezultata ponovo pitaju dozvolu (4489, 4513). Forma s `smije=0` se ne otvara (4423, 4459), a zabranjene stalne stavke desnog klika grida su sive (8787-8826). Stavke POPUP-a VB uspoređuje s objektom bez forme, pa na dijalogu ne djeluju; Carpo isto. VB gleda samo izravnu djecu forme (`Controls`), Carpo sve kontrole (i one u okviru). U korpusu `PERMISIO` je samo na PROJECT (29), na formi 0, pa je ovo za nove granule.

## Ctrl na gridu (CTL-01)

`ControlClicked` (Form2 6079, 5635, 5790, 8704; Form1 6984, 7036): dok je Ctrl pritisnut, promjena reda na gridu dijaloga ne šalje `ONCLICK` (ni klik koji ju je napravio). Ctrl + desni klik na gridu (dijalog i podloga) u NetGenu je popis stupaca s kvačicom vidljivosti; Carpo otvori svoj izbor stupaca (`ShowCustomization`, kao `ComponentGrid.SetAttributeColumnChooser`), 50-50 → Carpo.

## Naslov forme s @@ (CAP-01)

`Form2.ProcitajXML` 1240: `Me.Text = SrediSQLPosljeNextForme(SrediSQL2(Me.Text), , True)`. 752 naslova formi u korpusu imaju `@@` (`Izmjena dokumenta : @@broj [ @@id ]`, `Arkodovi za tablu @@Šifra`). Carpo je naslov pokazivao doslovno. Sada se poslije ONLOAD-a i NetGenDBClipboard-a naslov sredi kao status (STS-01): parametri modula, pa CB, datumi za ispis. Test `cap01-naslov`.

Isto vrijedi za tekst labele, okvira i panela (1324, 1369, 1399; LBL-01): 64 labele i 2 okvira u korpusu imaju `@@` (`Želite li obrisati tablu @@Šifra ?`). Labela koja postane `null` je prazna. Test `lbl01-natpis`.

## Veličina dijaloga (VEL-01)

`Form2_FormClosing` 4108-4109 sprema visinu i širinu kroz VB `SaveSetting("Jupiter", "FormHeight"/"FormWidth", ključ)`, a `ProcitajXML` 2785-2790 ih vraća kad forma nije WINDOWSTATE 2 i ima MaximizeBox. Ključ je `strXML-strImeForme-Version1-Version2`: put XML-a (`remotedllpath\SharedDLLs\xml\ime.xml`), ime forme, VERSION projekta i VERSION forme. Carpo nije pamtio veličinu. Sada `VelicinaDijaloga` čita i piše isti ključ u `HKCU\Software\VB and VBA Program Settings\Jupiter`, pa vrati veličinu koju je korisnik namjestio još u NetGenu. Bez korisnika (CI, klik pregled) se ne čita ni ne piše. Maksimiziran dijalog sprema normalnu veličinu (R121). Test `vel01-*`.

## Otvoreno

- `TIMERSQL` (4 forme, 1 granula): Timer2 je u NetGenu ugašen dok ga `TIMERON(n)` ne upali, a `TIMERON` nema nigdje u korpusu, pa se ni u NetGenu ne izvršava.
- `IMAGEINDEX` (30 gumba, 9 granula) i automatske ikone po natpisu (dodaj, obriši, izmjeni…): Carpo gumbi imaju svoj izgled (50-50 → Carpo).
- Grid dijaloga: layout datoteke (`MULTIGRIDLAYOUT` 1), `INPUTMASK` stupca (0 u korpusu). `COLUMNCAPTION`/`COLUMNWIDTH`/`DEFAULTACTIVECOLUMN` na gridu nema u korpusu. `PAGEFOOTER*` (57, 30 granula): Spin.Printing PrintLink nema podnožje (Carpo ComponentGrid ga isto ne šalje), pa ostaje bez njega (50-50 → Carpo).

## Testovi

`log01-*` (`LozinkaULogu`), `doz02-*` (`DozvoleDijaloga`), `lay01-raspored`, `hot01-f2`, `dat01-datumi` (`RasporedDijaloga`), `dgr01-*`, `dgr02-grid-atributi`, `dgr03-editedcolumn` (`GridDijalogaStupci`), `isp07-printgrid-ime`, `grb01-*`, `tre03-*`, `mdr01-multidrop`, `for01-foreachrow` (`DogadjajiDijaloga`), `Form2Izgled` pod Mono (dock, anchor, stil, boja, hotkey, mjesta gumba).
