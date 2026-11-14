# Razlike NetGen / Carpo

Pregled stavku po stavku. Port koji prati NetGen nije rupa. Ostale stavke su uklonjene jer ih `Carpo.NetGen` već radi, uz test u `Carpo.NetGen.GridTest` ili `tests/Carpo.NetGen.Granule`.

## Neizbježno

### R109 Prazan CIS URL ne postaje produkcija

- **NetGen:** prazan URL postane `https://cis.porezna-uprava.hr:8449/FiskalizacijaService` (`Form1.vb` 377).
- **Zašto ostaje:** Carpo taj URL ne šalje. SOAP ide samo na testni CIS. Ostali Raverus koraci su isti (`GenerirajUuid`, zaštitni kod, `PosaljiSoapPorukuCertifikatDatoteka`, `DohvatiJir`).
- **Test:** `fiskal-izvrsi-test`, `fiskal-uuid-spin`

### R85 `spMPTrgovineNG` nema `@operateriid`

- **NetGen:** šalje `@operateriid` (`Form1` parametar operatera).
- **Zašto ostaje:** XML se ne mijenja. Procedura na spinu nema taj parametar. Isti tekst bi pao i u NetGenu.

### R86 `MPKasaPolog` ima dvostruke stupce

- **NetGen:** SQL ima `odsat`, `odmin`, `dosat`, `domin` dvaput.
- **Zašto ostaje:** XML se ne mijenja. Spin javi `Ambiguous column name`. Isti tekst.

### R87 `oznaka2` nije stupac

- **NetGen:** `OznakaKL` čita `klmasterroba.oznaka2`.
- **Zašto ostaje:** XML se ne mijenja. Ako stupca nema, `ExecSql` padne i grid ostane prazan (`Form1` punjenje mastera).

### R88 Prazan master je podatak

- **NetGen:** prazan rezultat ne otvara drugi upit.
- **Zašto ostaje:** Carpo šalje isti predložak. Prazno je podatak ili čekanje na čvor, ne drugačiji SQL.

### R89 Jupiter ne izvršava proceduru koja piše

- **NetGen:** `JeZabranaPisanja` gleda prvu riječ (`Class1`). `spKntZakljucivanjeTem @action=1` nije `insert`, pa bi se poziv poslao. Tijelo piše u trajnu tablicu.
- **Zašto ostaje:** smoke na `jupiter2019` i `jupiter2022` ne pokreće proceduru čije tijelo piše. XML i Jupiter se ne diraju.
- **Test:** `zastita-zakljucivanje`

### R111 COM fiskalizacija prostora

- **NetGen:** `PoslovniProstor_COM` (`Form2.vb` 6671–6759) učitava `Raverus.FiskalizacijaDEV.COM` ili `SLOFISKAL`. Prazan URL postane produkcijski CIS (6689).
- **Zašto ostaje:** te dvije biblioteke nisu na stroju. Prazan URL i `cis.porezna-uprava.hr` se ne šalju. Grana odluke (nema red, Slovenija, nema PFX, test URL) jest portana.
- **Test:** `prostor-com`

### R113 Janus dizajner bez XML-a

- **NetGen:** `ApperianceDialog` i `FormatStyles` (`Form1.vb` 6146–6159) samo otvore Janus dijalog i vežu ga na grid. Nema atributa u XML-u koji to traži.
- **Zašto ostaje:** XML ne traži te dijaloge. `Show()` nema put u Carpo formi.

### R101 i R102 Spin nema prava tijela

- **NetGen:** zove proceduru iz XML-a. Ako je nema, padne.
- **Zašto ostaje:** na spinu je stub `SELECT … WHERE 1=0` dok ne stignu prava tijela. Jupiter se ne čita. Popis je u `docs/granule/spin-objekti.md`.

### R114 Ispis iz izbornika grida ide kroz Spin.Printing, bez podnožja

- **NetGen:** „Ispis” (`IspisNovi`, Janus GridExPrinting) i „Ispis REPX” (`IspisGRIDREPX`, NetGenExtensions) s PAGEHEADER*, PAGEFOOTER* i PAGEORIENTATION (Form1, Form2 9015-9056).
- **Zašto ostaje:** Carpo grid ispisuje Spin.Printing `PrintLink`, koji ima zaglavlje (naslov, lijevo, desno) i orijentaciju, ali ne podnožje (Carpo `ComponentGrid` ga isto ne šalje). Jedna stavka „Ispis” umjesto dvije, jer bi obje radile isto (ISP-07).
- **Test:** `isp07-printgrid-ime`

### R115 Izgled grida u .xml, Janus .gxl se ne čita

- **NetGen:** „Spremi postavke grida” piše Janus `.gxl` (za DXGRID `.xml`) u `Grid Settings` (Form1 6192-6420, Form2 9205-9243).
- **Zašto ostaje:** DevExpress ne čita Janus format. Ime datoteke je isto, nastavak `.xml` kao NetGen DX grid (GRL-01).
- **Test:** `grl01-spremi-ucitaj`, `grl01-reset`

### R116 Pomoć bez HELPKOD-a se ne pokreće

- **NetGen:** gumb bez ONCLICK (ili „help”) i F1 zovu `DajPokaziHelp` i kad nema HELPKOD-a (`-h` prazan).
- **Zašto ostaje:** JupiterHelp bez koda nema što pokazati; Carpo ga ne pokreće (HLP-02).
- **Test:** `hlp02-argumenti`

### R117 Obje boje ćelije iz upita vrijede

- **NetGen:** svaki `*CellForeColorID`/`*CellBackColorID` stupac zamijeni cijeli FormatStyle ćelije, pa od dviju boja ostane zadnja po redu stupaca (`GridEX1_FormattingRow` 7875).
- **Zašto ostaje:** očita namjera procedure je obje boje; DevExpress `RowCellStyle` postavi obje (GRB-01).
- **Test:** `grb01-boje`

### R118 Pitanje lanca bez korisnika je Cancel

- **NetGen:** ISEMPTYORZEROALERT/CANCELBOX uvijek pokaže MsgBox.
- **Zašto ostaje:** na CI-u i u klik-pregledu nema korisnika, a modalni prozor zaustavi test; pitanje vrati Cancel (lanac stane), upozorenje ide u dnevnik. S korisnikom je `XtraMessageBox` (DEV-01).

### R119 Tekst ploče za grupiranje je Carpov

- **NetGen:** `BuiltInTexts(GroupByBoxInfo) = ""` (prazna ploča).
- **Zašto ostaje:** izgled je Carpov (pravilo 50-50); vidljivost ploče prati GROUPBYBOXVISIBLE (DGR-02).

### R120 Lozinka nije u logu, log ima granicu

- **NetGen:** `NapuniCBLog` piše SQL kakav ide u bazu, s vrijednošću polja s `PASSWORDCHAR` (MPKase: `ServerPass`, `PassAdmin`). Ctrl+F11 na dijalogu pokaže te parametre u Form5.
- **Zašto ostaje:** plan, faza G. `NgDnevnik` umjesto `@ime='…'` polja s lozinkom piše `'***'`. Upisana vrijednost (3 znaka i više) skriva se i bez imena. Lista zapisa drži zadnjih 1000; cijeli log je u Carpo Loggeru. Ctrl+F11 na dijalogu i podlozi upiše parametre u Carpo log i otvori njegov prozor (LOG-01).
- **Test:** `log01-ime`, `log01-vrijednost`, `log01-kratka`, `log01-granica`, `log01-dijalog`

### R121 Maksimiziran dijalog pamti normalnu veličinu

- **NetGen:** `SaveSetting` sprema `Me.Height`/`Me.Width` i kad je dijalog maksimiziran, pa se idući put otvori golem, ali ne maksimiziran.
- **Zašto ostaje:** Carpo sprema `RestoreBounds` (normalnu veličinu) kad dijalog nije Normal (VEL-01). Ključ i mjesto u registru su isti kao u NetGenu.
- **Test:** `vel01-registar`

### R122 Combo na izlazu ne ponavlja ONCLICK i ne skače na prvu stavku

- **NetGen:** `AutoCompleteCombo_Leave` (Form2 9397-9409) na svakom izlasku iz comboa traži točan tekst. Ako ga ne nađe, postavi prvu stavku (`SelectedIndex = 0`), a zatim zove `Glbl_ComboClick`, pa se ONCLICK combo (223 u korpusu, 164 su `select` koji puni druga polja) izvrši još jednom, i bez promjene vrijednosti.
- **Zašto ostaje:** Carpo `LookUpEdit` sam dopunjuje upisani tekst, a nepoznat tekst vrati na prošlu vrijednost. ONCLICK ide samo na promjenu vrijednosti (`EditValueChanged`), pa povratak u combo i izlazak bez promjene ne prepiše ručno ispravljena polja (50-50 → Carpo).

### R123 Stari VB6 ispis je ukinut

- **NetGen:** `.clsReport` ide u VB6 COM `PrintDlg.clsPrintDlg` (`Class1.PrintRpt` 379-420), a `rs:`/`rs/`, `rsgen` i `gg/` u Form6/PrintDlg (92 poziva u ~26 granula).
- **Zašto ostaje:** odluka korisnika 14. 11. 2026.: stari VB6 ispis se ukida. Carpo ga ne zove; `PrintBridge.JeStariVb6` zapiše grešku u Carpo log s imenom ispisa, umjesto da klik tiho ne napravi ništa. `vrstadokid-` i `repx` idu u Spin.Printing kao i dosad (ISP-05).
- **Test:** `isp05-prepoznaje`, `isp05-ukinut`

### R124 Nakon SQL greške granula dobije novu sesiju (kao NetGen Close/Open)

- **NetGen:** svi upiti granule idu kroz jednu otvorenu vezu (`Conn`, Form1 1607). Nakon svake greške koja nije RAISERROR s `#`/`+` `Class1.ExecSql` 241-242 radi `Conn.Close()` i `Conn.Open()`; pool pri tome pozove `sp_reset_connection`, pa se otvorena transakcija poništi i `#temp` tablice nestanu. Timeout ne stigne do toga ako je na razini veze.
- **Carpo:** `NgVeza` nakon takve greške i nakon timeouta zatvori vezu, pa server napravi rollback; sljedeći upit otvori novu sesiju. Isti ishod kao NetGen (PN-04, neovisni pregled 14. 11. 2026.; prije je zatvarala samo nakon timeouta).
- **Test:** `ses01-ista-sesija`, `ses01-otpusti-zatvori`, `ses02-veza-nakon-greske`

## Uklonjeno

Svaka uklonjena stavka ima test koji provjerava NetGen linije. R59 i R110 su portani: redni stupac grida, gumbi 79/8 na dnu, slika čvora 0, `PageHeaderFooter` Left/Center/Right.

| Stavka | Test |
|---|---|
| R01 stablo stupci 0–3, id 0 | `sta-stablo` |
| R02 prazno stablo | `OKL-31` |
| R03 combo puni stablo | `odabir-verizne-manager` |
| R04 `REFRESHMASTER` | `jez-refreshmaster` |
| R05 naslov ` v[verzija]` | `JEZ-02` |
| R06 `PERMISIO` | `JEZ-71` |
| R07 širina `/14`, skriveni id, redni stupac | `bla-sirina`, `redni-stupac` |
| R08 agregat jedno G, `"0"` | `JEZ-09` |
| R09 detail kolone jednom | `sta-detail` |
| R10 statički čvor je susjed | `sta-stablo` |
| R11 detail parametar je ime stupca | `bla-sirina` |
| R12 `SETDEFAULTS` na `#` | `RIJ-26` |
| R13 `@robaid` nije `@@robaid` | `PRI` |
| R14 `ONSUBMIT` koji počinje sa `sp` | `KUL-04` |
| R15 prazan broj je `0` | `FAK-03` |
| R16 tražilica | `KUL-07` |
| R17 `spRaiserror` i `+focus` | `KUL-09` |
| R18 `:FILTERFORM` | `MAIL-04` |
| R19 prazna forma ne otvara prozor | `STA-08` |
| R20 `~` postaje `#` | `KUP-02` |
| R21 `MULTISELECT` / `MULTIBLUE` | `KUP-03` |
| R22 `FOREACH` na podlozi | `foreach-podloga` |
| R23 `OKSTAY` / `CLOSEPARENT` | `dijalog-ostaje` |
| R24 `firstday*` | `KON-20` |
| R25 `SHOWCHECKBOX` | `DOB-02` |
| R26 combo stupac 0/1 | `combo-1-prazan` |
| R27 `refresh` kolona | `DOS-05` |
| R28 multidrop `~` | `DOS-07` |
| R29 check je 0/1/2, radio 1/0 | `FAK-07` |
| R30 `TYPE` je `InStr` | `KON-01` |
| R31 tipfeler se ne ispravlja | `INV-03` |
| R32 statusna traka | `DOS-02` |
| R33 `DOCK` broj | `NAR-04` |
| R34 label ne šalje parametar | `DOS-03` |
| R35 `ASSEMBLYLOAD` | `MAIL-20` |
| R36 `DBCLIPBOARD=false` | `MAIL-05` |
| R37 `ONLOADED` | `KUC-04` |
| R38 tri potpisa `totalgen` | `KUC-03` |
| R39 tilda bira router | `KUC-02` |
| R40 povrat `totalgen` se ne čita | `KUC-06` |
| R41 `PRINTMASTER` | `print-master` |
| R42 `:repx/` | `KAR-32` |
| R43 `vrstadokid-` | `MPB-03` |
| R44 `MULTISELECT` briše temp | `MPB-05` |
| R45 dijalog stablo | `RIJ-11` |
| R46 `yearmonth` samo podloga | `RIJ-40` |
| R47 WebBrowser `URL` | `RIJ-16` |
| R48 PictureBox | `RIJ-18` |
| R49 `NEXTFORM` | `next-kom` |
| R50 DB clipboard | `RIJ-28` |
| R51 `SETDEFAULTS` | `RIJ-26` |
| R52 ikona 0 | `redni-stupac` |
| R53 grid na dijalogu | `RIJ-14` |
| R54 `onlytree` | `LVO-04` |
| R55 `Ime(@a=)` | `LVO-05` |
| R56 `BUTTONS` | `buttons-ostaju-cmd` |
| R57 enable po natpisu | `MPP-04` |
| R58 `CLICK` | `klik-podloga` |
| R59 redni stupac, gumbi 79/8, slika čvora | `redni-stupac` |
| R60 `CMD` bez dvotočke | `MPK-06` |
| R61 `TABINDEX` s razmakom | `MPK-21` |
| R62 `VALUE` nije datum | `MPK-22` |
| R63 prazan `FILLEXPLORER` | `MPK-30` |
| R65 `COMMANDTIMEOUT` | `INT-08` |
| R66 `TOTALROW` | `NPL-04` |
| R67 `Ime.Start` | `opz-nije-dll` |
| R68 `ONENTERKEY` | `PDV-05` |
| R69 `POSITION` | `OPZ-07` |
| R70 `FILLEXPLORER` na projektu | `OPZ-04` |
| R71 `ONLOAD` lanac | `GFI-03` |
| R72 `#ISEMPTY` u zaglavlju | `REA-05` |
| R73 `.clsGFIPOD` | `GFI-05` |
| R74 podloga nije Office ribbon | `odabir-jezici-nije` |
| R75 `DETAILDBLCLICK` | `KAS-04` |
| R76 `PORTAL` nije landscape | `KNP-03` |
| R77 `REFRESHAFTERCHILD` projekt | `KOD-05` |
| R78 `INCLUDE` | `include-dijete` |
| R79 `DETCHECKBOXVISIBLE` | `STA-38` |
| R80 `AUTOFITCOLUMNS` | `MPSNG-05` |
| R81 `HELPKOD` | `INS-04` |
| R82 `REFRESHAFTERCHILD` forma | `VRC-05` |
| R83 checklista rasporeda | `odabir-verizne-manager` |
| R84 `FILTER` je forma | `filter-je-forma` |
| R90 lanac dijaloga | `dijalog-ostaje` |
| R91 combo i grid SELECT | `dijalog-combo-sql` |
| R92 `NEXTFORM` i `FILLEXPLORER` | `rupa-next` |
| R93 prazan red comboa | `combo-prazan-red` |
| R94 `exec` se ne lijepi | `onclick-exec-ne-lijepi` |
| R95 `SKIPDEFAULTLOAD` | `grid-skip` |
| R96 `NEXTFORM` tekst | `next-kom` |
| R97 master nakon OK | `okl-prihvati` |
| R98 širina `/14` | `bla-sirina` |
| R99 `Ime.Start` nije DLL | `opz-nije-dll` |
| R100 stablo 0–3 | `sta-stablo` |
| R103 `WIDTH` prepisuje | `bla-width-prepisuje` |
| R104 Manager samo kad treba | `odabir-verizne-manager` |
| R108 parametri dijaloga | `param-radio` |
| R110 Left/Center/Right i `lodani` | `ispis-zaglavlje` |
