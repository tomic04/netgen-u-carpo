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
- **Zašto ostaje:** DevExpress ima `FormatConditions` i poglede, i to je portano (`TableView`→`GridView`, `CardView`→`CardView`, `SingleCard`→`LayoutView`, `NEGATIVERED`/`NOTZERORED`). Sam `Show()` dizajnera nema ekvivalent koji XML može uključiti.
- **Test:** `F1-114` pokriva poglede. Dizajner se ne otvara.

### R112 Spin.Printing.v23

- **NetGen:** `IspisiMePrekoSpinPrintinga` (`Class1.vb` 297) radi `Assembly.Load("Spin.Printing.v23")` i `CreateInstance("Spin.Printing.Start")`. `IspisiMe7` i `PrintRpt` idu tim putem.
- **Zašto ostaje:** ta biblioteka nije referenca Carpa. Ispis koji jest portan ide kroz `PrintBridge` i `PageHeaderFooter`.
- **Test:** `ispis-zaglavlje` pokriva zaglavlje. Učitavanje `Spin.Printing.v23` se ne izvodi.

### R101 i R102 Spin nema prava tijela

- **NetGen:** zove proceduru iz XML-a. Ako je nema, padne.
- **Zašto ostaje:** na spinu je stub `SELECT … WHERE 1=0` dok ne stignu prava tijela. Jupiter se ne čita. Popis je u `docs/granule/spin-objekti.md`.

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
| R114 `JupiterTranslator10` u `Run` | `F1-114` |
