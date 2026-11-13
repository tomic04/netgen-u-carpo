# Otvoreno za implementaciju

Brojke su prema CI implementacije [43269944426](https://github.com/tomic04/netgen-u-carpo/actions/runs/43269944426) (`ef710b9`) i [#74](https://github.com/tomic04/netgen-u-carpo/pull/74). Jupiter QA ne zove. Testovi nisu popušteni.

Od 136 zatvorenih, **107 je pravilo** i **29 je spin podatak**. Devet je `OznakaKL` (`OKL-25`–`27`, `OKL-40`–`43`, `OKL-58`, `OKL-59`): 947 redova. Deseti je `JEZ-31`. Jedanaesti je `FAK-11`. Dvanaesti je `PL-32`. Trinaesti je `MPP-20`. Četrnaesti je `STA-12`: `Parent` `0701` vodi na `MeniID` 1611, `Loader.Run8` otvori `Statusi.xml`. Petnaesti je `JEZ-57`: desno zaglavlje je `naziv` iz `SETDEFAULTS`, ne `@@tkosmomi`. `Form1.vb` 6001, `Class1.vb` 966.

| Element | Ukupno | Zatvoreno | Pravilo | Spin podaci | Otvoreno |
|---|---:|---:|---:|---:|---:|
| grid | 34 | 33 | 24 | 9 | 1 |
| stablo | 20 | 20 | 19 | 1 | 0 |
| combo | 5 | 1 | 1 | 0 | 4 |
| gumbi/lanac | 38 | 29 | 27 | 2 | 9 |
| dijalog | 28 | 20 | 20 | 0 | 8 |
| ispis | 4 | 2 | 1 | 1 | 2 |
| ostalo | 35 | 31 | 15 | 16 | 4 |
| **sve** | **164** | **136** | **107** | **29** | **28** |

[#66](https://github.com/tomic04/netgen-u-carpo/pull/66) zatvara pravilo, ne redove iz baze. `STA-33` je forma iza dvotočke (`Form1.vb` 7186–7187). `STA-35` puni stablo adapterom (`4441–4447`). `BLA-30` prazan tag `:` ide u `NovaGenerickaForma` (`4353–4355`, `7187`). `BLA-31` `SrediSQL` prije `Fill` (`4442`). `BLA-32` četiri korijena: natpis `Blagajne` plus tri store (`4344–4356`). `PL-30` i `PL-31` slijede `7730–7733` i izlaz dok je `TREE` prazan (`4310`). `PL-35` je `REFRESHMASTER` koji nije `REFRESHMASTERGRID` (`4570`).

Po stavci, [#74](https://github.com/tomic04/netgen-u-carpo/pull/74) `af3816f`:

- `JEZ-31` potvrđen. `spJeziciQuery @action=0` vratio je `JeziciId, Naziv` i 15 redova. Tijelo nema upis. `Class1.vb` 113–118 šalje poziv jer nema `@@`. `Form1.vb` 3882. Sažetak 164 / 138 / 26.
- `DOS-11` odbijen. Test broji ime u `sys.objects`. `dospjele.sql` sam kaže da tri procedure nisu klijentsko tijelo. `Form2.vb` 2430–2431 (stupac 0 vrijednost, stupac 1 natpis) se ne izvršava. Ostaje otvoren. Sažetak i dalje 164 / 138 / 26.
- `DOS-11` potvrđen. Run [43284522194](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284522194) (`261149b`) je zelen. `[Z] konto 1/1000, prazan red na klasi, shema 1/Test shema, grid ima red`. `ComboDijalog.Ucitaj` pozove `spKntPopuniKontroleNG`, `spOrgShemaQuery` i `spKntOtvStavkeQueryNG`. Stupac 0 je vrijednost, stupac 1 natpis (`Form2.vb` 2430–2431, 2503–2504). `ALLOWEMPTY` ubaci prazan prvi red. Tijela na spinu i dalje vraćaju te vrijednosti, ali test ih izvrši i veže stupce. Sažetak 164 / 146 / 18.
- `FAK-10` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:FakPar.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `FAK-10` potvrđen. Run [43286278068](https://github.com/tomic04/netgen-u-carpo/actions/runs/43286278068) (`cad37f0`) je zelen. `[Z] asm:NetGenDLL.loader:FakPar.xml -> FakPar.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 147 / 17.
- `DOS-10` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:KntDospjeleStavke.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `DOS-10` potvrđen. Run [43287717644](https://github.com/tomic04/netgen-u-carpo/actions/runs/43287717644) (`62a622e`) je zelen. `[Z] asm:NetGenDLL.loader:KntDospjeleStavke.xml -> KntDospjeleStavke.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 148 / 16.
- `KUL-12` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:Kulture.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `KUL-12` potvrđen. Run [43288406517](https://github.com/tomic04/netgen-u-carpo/actions/runs/43288406517) (`e8c9bf9`) je zelen. `[Z] asm:NetGenDLL.loader:Kulture.xml -> Kulture.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 149 / 15.
- `KND-06` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:KUPDostKN.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `KND-06` potvrđen. Run [43289399962](https://github.com/tomic04/netgen-u-carpo/actions/runs/43289399962) (`e9c2ded`) je zelen. `[Z] asm:NetGenDLL.loader:KUPDostKN.xml -> KUPDostKN.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 150 / 14.
- `BVG-07` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:KUPDostKnNGBvg.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `BVG-07` potvrđen. Run [43290097974](https://github.com/tomic04/netgen-u-carpo/actions/runs/43290097974) (`3c4a84e`) je zelen. `[Z] asm:NetGenDLL.loader:KUPDostKnNGBvg.xml -> KUPDostKnNGBvg.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 151 / 13.
- `KUC-07` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:KUPNG.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `KUC-07` potvrđen. Run [43294928466](https://github.com/tomic04/netgen-u-carpo/actions/runs/43294928466) (`91ec915`) je zelen. `[Z] asm:NetGenDLL.loader:KUPNG.xml -> KUPNG.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 152 / 12.
- `KUP-05` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:KUPNG.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `KUP-05` potvrđen. Run [43297471533](https://github.com/tomic04/netgen-u-carpo/actions/runs/43297471533) (`aeec3bc`) je zelen. `[Z] asm:NetGenDLL.loader:KUPNG.xml -> KUPNG.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 153 / 11.
- `NAR-09` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:MPNarIzdSkl.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `NAR-09` potvrđen. Run [43299854455](https://github.com/tomic04/netgen-u-carpo/actions/runs/43299854455) (`3b92974`) je zelen. `[Z] asm:NetGenDLL.loader:MPNarIzdSkl.xml -> MPNarIzdSkl.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 154 / 10.
- `MAIL-07` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:MPNGPregDokMAIL.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `MAIL-07` potvrđen. Run [43301537550](https://github.com/tomic04/netgen-u-carpo/actions/runs/43301537550) (`a0956c1`) je zelen. `[Z] asm:NetGenDLL.loader:MPNGPregDokMAIL.xml -> MPNGPregDokMAIL.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 155 / 9.
- `RIJ-31` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:RatArkod.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `RIJ-31` potvrđen. Run [43303230660](https://github.com/tomic04/netgen-u-carpo/actions/runs/43303230660) (`512d0e6`) je zelen. `[Z] asm:NetGenDLL.loader:RatArkod.xml -> RatArkod.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 156 / 8.
- `RIJ-33` odbijen. Test gleda `Parametar like`. `Meni.ps1` upise `asm:NetGenDLL.loader:RobaWebKlase.xml`. `fk_meni` je `nocheck` jer izracunati Parent ne prolazi. Nema NetGen linije koja ovo potvrduje. Ostaje otvoren. Sazetak 164 / 138 / 26.
- `RIJ-33` potvrđen. Run [43305196144](https://github.com/tomic04/netgen-u-carpo/actions/runs/43305196144) (`1f1a837`) je zelen. `[Z] asm:NetGenDLL.loader:RobaWebKlase.xml -> RobaWebKlase.xml otvoren`. `Loader.vb` 183 stavi `ModuleID`, `Form1.vb` 1251–1258 spremi `Mid`, `1139` otvori `SharedDLLs\xml\` plus taj modul. `ONLOAD` učitanog XML-a je isti kao u repou. `fk_meni` se ne gasi. Sažetak 164 / 157 / 7.
- `ONK-04` potvrđen. Tri gumba su `ONK-04`, `NIV-05`, `REA-04`. Run [43304310432](https://github.com/tomic04/netgen-u-carpo/actions/runs/43304310432) (`2138faf`) je zelen. `[Z] otvaraFormu=Zakljucavanje saljeSubmit=false`. Gumb `Zaključi` ima `ONCLICK=:Zakljucavanje`. `Form2.vb` 6882 otvara tu formu i ne šalje `spKntZakljucivanjeTem`. Sažetak 164 / 158 / 6.
- `NIV-05` potvrđen. Run [43307347232](https://github.com/tomic04/netgen-u-carpo/actions/runs/43307347232) (`99e36b4`) je zelen. `[Z] ONSUBMIT=spMPQueryGlaIDetNG dnevnik=NetGen SQL redovi=1`. `Pokaži` je `Button1` (`Form2.vb` 3367, `OKClick=True`). `FILTERFORM` ima `FILLEXPLORER=TRUE`, pa `Form1.vb` 5445–5452 pošalje `ONSUBMIT`. `@@` je zamijenjen i upit vrati red. Sažetak 164 / 159 / 5.
- `REA-04` potvrđen. Isti run. `[Z] ONSUBMIT=spMPRealizacijaRobeNG dnevnik=NetGen SQL redovi=1`. `Pokaži` na `FORM1` je `Button1` (`Form2.vb` 3367). `FILLEXPLORER=TRUE` pošalje `ONSUBMIT` (`Form1.vb` 5445–5452). `@@` je zamijenjen i upit vrati red. Ovo nije `REA-00`. Sažetak 164 / 160 / 4.
- `JEZ-69` vraćen u otvoreno. Kuka uvijek stavi `border=Sizable`, ne čita `BORDERSTYLE`. `Form2.vb` 999–1124.
- `KAR-24` vraćen u otvoreno. `repxClose` je uvijek `CLOSE`, ne gleda `#CLOSE` u naredbi. `Form2.vb` 7033.
- `DOS-23` vraćen u otvoreno. `multidrop.kntplanaid=tilde` čim kontrola postoji, ne slaže `id1~id2`. `Form2.vb` 3446.
- `INV-22` vraćen u otvoreno. `radio.rbtnKumulativ=1-0` čim radio postoji, ne šalje 1 ili 0. `Form2.vb` 3432–3435.
- `FAK-21` vraćen u otvoreno. `check.PoSkladistu=checkstate` čim checkbox postoji, ne šalje `CheckState`. `Form2.vb` 3424–3427.
- `INV-21` vraćen u otvoreno. `date.datumdo=nema-default` čim postoji `DEFALUT`, ne provjeri da se `today` ne primijeni. `Form2.vb` 2252.
- `NAR-20` vraćen u otvoreno. `date.datum=date` čim tip sadrži `datetiem`. `Form2.vb` 2232.
- `NAR-21` vraćen u otvoreno. `width.pakovanje=nema` čim postoji `WUDTH`. `Form2.vb` 2949.
- `DOB-21` vraćen u otvoreno. `combo.translate=stupac1` čim je `TRANSLATE` true, stupac se ne prevodi. `Form2.vb` 2420–2424.
- `RIJ-40` vraćen u otvoreno. `tree.dijalog.tag=stupac3` čim dijalog ima stablo. `Form2.vb` 2576–2577.
- `RIJ-41` vraćen u otvoreno. `web.navigate=onload-kolona` čim webbrowser nema `URL`. `Form2.vb` 1419–1433.
- `RIJ-42` vraćen u otvoreno. `pic.stretch` je konstanta `stretch`. `Form2.vb` 2710–2726.
- `OKL-77` vraćen u otvoreno. `kontekst` je upisan niz stavki, ne izbornik iz `Form1`. `Form1.vb` 3620–3673.
- `MPB-20` vraćen u otvoreno. `spin.printing` je uvijek `v26`. `Class1.vb` 297–304.
- `KAR-40` vraćen u otvoreno. `repx.fallback` je uvijek `rptDevRepx`. `Form6.vb` 134–161. Sažetak 164 / 145 / 19.
- `NRD-MSGBOX` potvrđen. Run [43314372499](https://github.com/tomic04/netgen-u-carpo/actions/runs/43314372499) (`7a2a744`) je zelen. `[Z] KOD_Diskete`. `PorukaNaredba.Tekst` digne `MSGBOX(...)` na velika slova (`Form2.vb` 7165–7169). Nije u 164. Samo dijalog `KOD_Diskete`, nema druge forme. Sažetak i dalje 164 / 145 / 19.
- `NRD-ASSEMBLY` potvrđen. Isti run. `[Z] MPOdjavaNarNG tip=ExportToExcelNG.clsExportToExcelNG prekinuto=da`. Gumb `Spremi` je na dijalogu `EXPORTEXCEL`. `Assembly.Load` koji padne prekine lanac, a argument 3 i 4 su `strParametriSvi` (`Form2.vb` 7431–7435, 7456). Shell (`Form1.vb` 4623) nije pokrenut. Nije u 164. Sažetak i dalje 164 / 145 / 19.
- `NRD-RODITELJ` potvrđen. Isti run. `[Z] MPIzdVPNG closeParent=da sql=spMPIzdatnicaSKLMP`. SQL ispred ostane, `CLOSEPARENT` stavi `bCloseParent` i zove `Button2` (`Form2.vb` 7035–7037). Ista dva koraka su i na podlozi (`Form1.vb` 6838–6840). `KUP-16` je već u zatvorenima. Nije nova stavka od 164. Sažetak i dalje 164 / 145 / 19.
- `NRD-DOPRINT` potvrđen kao poziv. Isti run. `[Z] KntBlaPromet DoPrint2 isnetgen=1 put=SharedDLLs\repx kopije=0`. `DevRepx` zove `DoPrint2` (`Form6.vb` 134–161). Test zove `DevRepx` izravno, ne nakon pada `Spin.Printing` (`Class1.vb` 606). Predložak nema vodeći `/` koji `Class1.vb` 623 i `Form6.vb` 161 ostave. `KAR-40` ostaje otvoren. Sažetak i dalje 164 / 145 / 19.
- `NRD-CLICK` odbijen. Isti run je zelen, ali test samo skine ime iz `click(Ok)`. `Glbl_Click` se ne zove (`Form2.vb` 7155). Lanac `ONCLICK` već drži `KBP-20`. Nije nova stavka. Sažetak i dalje 164 / 145 / 19.
- `NRD-PROSTOR` odbijen. Test gleda da lanac dođe do `OK`. `PoslovniProstor` se ne zove, ni jednom ni po označenom redu (`Form2.vb` 7489–7493, `Form1.vb` 4791–4804). Sažetak i dalje 164 / 145 / 19.
- `NRD-BLAGAJNA` odbijen. Tekst `spmpBlagajnaEnable` i `update mpkase` odgovara `Class1.vb` 1975–1986, ali test sam pošalje `true` i `false`. Rezultat procedure se ne čita. Sažetak i dalje 164 / 145 / 19.
- `NRD-ISPIS` odbijen. `PRINTREPXMASTER` se samo svrsta u `ispis` / `master`. `IspisGRIDREPX` i `PrintDocumentREPX("master")` se ne zovu (`Form1.vb` 4945–4947, 6003). Sažetak i dalje 164 / 145 / 19.
- Klik podloge, run [43316376641](https://github.com/tomic04/netgen-u-carpo/actions/runs/43316376641) (`a666024`), tri tipa: `OznakaKL` (gumb), `JeziciNG` (master i gumbi), `Statusi` (stablo, master, detail). `Prikazi` napuni prije `Show`, pa klik ide i kad prozora nema u `OpenForms`. `GRESKA` ne ruši korak. `Statusi` master i detail: `Invalid object name 'NetGenDBFilter'` (`Form1.vb` 3882, 2258). `Izmjeni`: `@@statusiid` ostane (`Class1.vb` 765–849). Nije stavka od 164. Sažetak i dalje 164 / 145 / 19.
- Filter `NetGenDBFilter` potvrđen na `Statusi`. Run [43325937538](https://github.com/tomic04/netgen-u-carpo/actions/runs/43325937538) (`a1926ed`) je zelen. Master više nije `GRESKA`, nego `PRAZAN`. `Form1.vb` 4360–4361 preskoči čvorove kad upit vrati `Nothing`. U kratkom kliku je samo ova granula sa stablom. Nije stavka od 164. Sažetak i dalje 164 / 145 / 19.
- `GRD-01` odbijen kao zatvaranje gumba. Isti run. Funkcija na izmišljenom retku stavi `@statusiid='5'` i preskoči stupac 0 (`Form1.vb` 2556–2576). `Statusi` `Izmjeni` i dalje pošalje `@@statusiid` (`Must declare the scalar variable`). Dijalog ne uzme `ParametriSaGrida` (`Class1.vb` 765–849). Sažetak i dalje 164 / 145 / 19.
- `GRD-02` odbijen kao zatvaranje unosa. Isti run. Funkcija na rječniku stavi `@Naziv='klik-proba'` (`Form2.vb` 3655). `JeziciNG` `Unesi` je `GRESKA`: nema reda `Naziv=klik-proba-20261113000735`. `Statusi` `Unesi` pošalje INSERT, ali `VrstaDokId` i `StatusiId` ostanu NULL (`Form2.vb` 3377, 3406). `Izmijeni` na `JeziciNG` pošalje `select * from jezici where jeziciid=13`, ne `spJeziciAzur`. `Obriši` ne obriše testni red. Sažetak i dalje 164 / 145 / 19.
- Zadnji run grane `cursor/netgen-treci-otvoreno-889d` je zelen, ali `GRESKA` na `Unesi` i `Izmjeni` ne ruši korak. Kratki klik ima dvije granule, ne treći tip forme. Sažetak i dalje 164 / 145 / 19.
- `BUTTONS` potvrđen na podlozi. Zadnji zeleni run `cursor/netgen-brzi-ci-3266` je [43329105454](https://github.com/tomic04/netgen-u-carpo/actions/runs/43329105454) (`208645a`). `GumbiNiz` prepiše samo segmente s dvotočkom, a `Button7` ostane ako `CMD7` nije prazan (`Form1.vb` 3770–3824, 3799–3803). Na `KntKompVerizne` ostanu `OBRISI_FORM` i `repx/`. Druga i treća granula nisu učitane. Nije stavka od 164. Sažetak i dalje 164 / 145 / 19.
- `Odustani` kao `Button2` odbijen. Isti zeleni run klikne `Odustani` na `OznakaKL` `Izmjena` i na ulaznom dijalogu `RealizacijaStavkeNG`, ali ne gleda `OKClick=false` ni izostanak SQL-a (`Form2.vb` 3400–3402). Noviji [43334848017](https://github.com/tomic04/netgen-u-carpo/actions/runs/43334848017) (`e20881f`) padne: `KntKompVerizne` `Obriši` ostavi dijalog `Brisanje`, tekst i dalje ima `@@broj`, a `KeyNotFound` otvori `ThreadExceptionDialog`. Timeout.
- `KUP-02` vraćen u otvoreno. Test gleda tildu i riječ `REFRESHMASTERGRID` u `CMD2`. Ne pretvara tildu u `#` i ne osvježi zadnji SQL. `Form1.vb` 4558, 4570–4577.
- `KUP-03` vraćen u otvoreno. Test gleda da naredba počinje s `MULTISELECT`. Temp tablica se ne puni. `Form1.vb` 4932–5018.
- `KUP-04` vraćen u otvoreno. Test gleda da naredba počinje s `MULTIBLUE`. `SelectedItems` se ne čita. `Form1.vb` 5022–5032.
- `KUP-10` vraćen u otvoreno. `MultiTemp()` uvijek vrati `delete-dva-pa-insert`. Nema dva brisanja ni inserta. `Form1.vb` 4934–5018.
- `KUP-11` vraćen u otvoreno. Kuka stavi `cekirani` čim tekst sadrži `FOREACHSELECTED`. Retci se ne vrte. `Form2.vb` 7040–7042, 8337–8354.
- `KUP-12` vraćen u otvoreno. Kuka stavi `zivo` čim tekst sadrži `FOREACHROW`. Petlja po retku se ne izvrši, ni komentar na podlozi. `Form2.vb` 7046–7048, `Form1.vb` 6843–6846.
- `KUP-13` vraćen u otvoreno. Kuka stavi `plavi` čim tekst sadrži `FOREACHBLUEROW`. `SelectedItems` od 0 se ne vrti. `Form2.vb` 7043–7045, 8363–8394.
- `KUP-15` vraćen u otvoreno. Očekivanje je konstanta `select refresh`. NetGen radi `select 'refresh' as ime`. `Form2.vb` 6911–6915.
- `DOS-20` vraćen u otvoreno. `TEXT1-upper` prođe čim `StatusTekst` vrati velika slova. `@@rowcount` se ne traži. `Form2.vb` 1258, `Class1.vb` 865. Sažetak 164 / 136 / 28.
- Brzi CI je crven. [43358008327](https://github.com/tomic04/netgen-u-carpo/actions/runs/43358008327) (`7fc8a8b`) i [43357710086](https://github.com/tomic04/netgen-u-carpo/actions/runs/43357710086) (`38ed45e`) stanu u buildu: `Placeholders.cs` `PageArea.Content` je samo za čitanje (`CS0200`), a `StringCollection` se ne pretvara u `string[]` (`CS0029`). Grid test i TestShell se ne pokrenu. `93af8ae` je još u redu. `main` nema novi commit od početnog uvoza.
- `STA-01` do `STA-05`, `WEB-01`, `BTN-01`, `TRO-01` i `ARK-01` nisu u 164 i nisu potvrđeni. Testovi slažu tekst u memoriji, ne šalju SQL na spin. `STA-01` ne zove `ExecSql` ni stupce iz `ONLOAD` (`Form2.vb` 1195–1205). `ARK-01` za prazan search stavi `null`, što jest grana `Form2.vb` 3632, ali `Spremi` se ne izvrši na `RatArkod`. `WEB-01` ne puni `TotalGenMultiSelectTemp` na spinu (`Form1.vb` 4932–5032). Sažetak i dalje 164 / 136 / 28.
- `MPK-30` potvrđen. `FILTERFORM` ima `FILLEXPLORER=TRUE` i `ONSUBMIT` `spMPPoravnajKP`. `Form1.vb` 5445–5452 zamijeni master tim SQL-om. Test nije konstanta: kriva klasa ili procedura bez tog imena pada. Sažetak 164 / 139 / 25.
- `MPK-31` potvrđen. `DODAJIZNOS` ima prazan `FILLEXPLORER`. To nije grana `true`. `Form1.vb` 5509–5519 izvrši `ONSUBMIT` pa ponovi zadnji master. Gumb je `Dodaj:DODAJIZNOS`. Sažetak 164 / 140 / 24.
- `STA-34` potvrđen. `POPUNI_FORM` nema kontrola pa se dijalog ne otvara (`Form1.vb` 5369–5370, `OKClick=True`). `FILLEXPLORER=TRUE` stavi master na `spStatusiQuery @action=0, @node=@@node` (`5445–5452`). Test traži taj tekst, nije konstanta. Sažetak 164 / 141 / 23.
- `PL-32` odbijen. Run [43284226475](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284226475) (`d4d658a`) stao je u `sta12.ps1` prije testova. `pl32.sql` je prošao, `Cetvrti.Pl32` se nije izvršio, nema `[Z]`. Stupac 0 iz `spNg_combo` nije prošao kroz `IzTeksta` (`Form1.vb` 3342–3344, 4344). Ostaje otvoren. Sažetak 164 / 141 / 23.
- `MPP-20` odbijen. Isti run nije došao do `Cetvrti.Mpp20`. `mpp20.sql` je prošao, nema `[Z]` da je `@@dodaj` zamijenjen prije `ExecSql` i da `Button.Text` `Dodaj` dobije enable ili disable (`Form1.vb` 3914–3921, `Class1.vb` 765–849, 932–964). Ostaje otvoren. Sažetak 164 / 141 / 23.
- `STA-12` odbijen. `sta12.ps1` je ubacio `Nivo` `070101` uz izračunati `Parent` i roditelja `MeniID` 55, pa stao: Parent ne pokazuje na postojeći `MeniID`. `fk_meni` nije ugašen. `Loader.Run8` (`Form1.vb` 1139, 1251–1258) se nije izvršio. Ostaje otvoren. Sažetak 164 / 141 / 23.
- `JEZ-57` odbijen. Run [43285371955](https://github.com/tomic04/netgen-u-carpo/actions/runs/43285371955) ispisuje `PASS jez-57-cb | Spin d.o.o.` i `PASS jez-57-prazan | null`. `GridTest.Jez57` sam stavi `tkosmomi = "Spin d.o.o."` i traži da `Desno` vrati taj tekst; prazan rječnik vrati riječ `null`. Nema `SETDEFAULTS` ni `IspisGRIDREPX`. U `c473084` očekivanje je promijenjeno s `@@tkosmomi` na `VrijednostTkosmomi()`, koja na iznimku vrati `null`, isto što prazan CB vrati (`Class1.vb` 916). `Form1.vb` 6001 i `Class1.vb` 966 nisu izvršeni nad retkom iz baze. Ponovo otvoren. Sažetak 164 / 140 / 24.
- `FAK-11` potvrđen. Run [43284832210](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284832210) i [43284846734](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284846734) (`08a871c`) su zeleni. `FakPar` se otvorio s 3 reda i 11 stupaca, bez `Invalid object name 'FakPartneriUvjeti'`. Tablica je iz `scripts/spin/FakPartneriUvjeti.sql`. `ONLOAD` je `spFakPartneriUvjetiQuery` bez `@@`, pa `Class1.vb` 113–118 šalje poziv, a `Form1.vb` 3882 ga prikaže. Stupci su aliasi action 1: `Šifra partnera`, `Partner`, `Učestalost`, `Grupiranje`, `Valuta`, `Rabat`, `Povrat`, `Žiro račun`, `Ugovor`; `FakPartneriUvjetiid` i `parziroid` su skriveni. `PoSkladistu` je samo checkbox forme. Test pada ako tijelo ima `WHERE 1=0` ili ako stupac ne odgovara. Sažetak 164 / 141 / 23.
- `PL-32` potvrđen. Run [43287798889](https://github.com/tomic04/netgen-u-carpo/actions/runs/43287798889) (`d221ac6`) ima `[Z] Cetvrti.Pl32 IzTeksta Danas:Promet#spNgTreeNabPri:Skladiste`. Stupac 0 iz `spNg_combo` nije natpis. `IzTeksta` cijepa po `#` i `:` (`Form1.vb` 3342–3344, 4344): `Danas` ide na `Promet`, a `spNgTreeNabPri` napuni `Sva skladista` s jednim djetetom. Krivi stupac ili izostanak store pada. Sažetak 164 / 142 / 22.
- `MPP-20` potvrđen. Isti run ima `[Z] Cetvrti.Mpp20 @@dodaj=1 Button.Text=Dodaj stanje=enable`. `spmpPermissionNG` vrati stupac `dodaj`, ne `INSERTFORM1`. `@@dodaj` se zamijeni prije `ExecSql` (`Class1.vb` 765–849, 932–964). `Form1.vb` 3914–3921 čita `Button.Text` `Dodaj`. Kuka vrati `gumb.stanje=enable`. Sažetak 164 / 143 / 21.
- `STA-12` odbijen na `d221ac6`. `fk_meni` je na `AplikacijeID` i `disabled=False`; nije gašen. `Parent` je izračunat `char(6)`, `left(nivo, len(nivo)-2)`, ne `MeniID`. Priprema je pala: Statusi.xml nema Parent na postojeći `MeniID`. Test: `Parent 0701   nije MeniID`. `Loader.Run8` (`Form1.vb` 1139, 1251–1258) se nije izvršio. Ostaje otvoren. Sažetak 164 / 143 / 21.
- `STA-12` potvrđen. Run [43289652736](https://github.com/tomic04/netgen-u-carpo/actions/runs/43289652736) (`6086a32`) je zelen, 359/360, 0 padova. Red je `MeniID` 3342, `Nivo` `070101`, `Parent` `char(6)` `0701`, `ParentMeniID` 1611. `[Z] Loader.Run8 Statusi.xml`. `fk_meni` na `AplikacijeID` je `disabled=False`. `Form1.vb` 1251–1258 spremi `Statusi.xml`, `1139` otvori `SharedDLLs\xml\` plus taj modul. Sažetak 164 / 144 / 20.
- `JEZ-57` odbijen na istom runu. `[Z]` traži `pageHeaderRight=@@tkosmomi`. Kuka na `6086a32` vraća sirovi atribut `PAGEHEADERRIGHT`, ne vrijednost iz CB. `Form1.vb` 6001 i `Class1.vb` 966 se ne izvrše. Ostaje otvoren. Sažetak i dalje 164 / 144 / 20.
- `JEZ-57` potvrđen. Run [43297138106](https://github.com/tomic04/netgen-u-carpo/actions/runs/43297138106) (`57af751`) je zelen. Test uzme `naziv` iz `SETDEFAULTS` (`select naziv as tkosmomi from partneri ... nasid`). Upit koji padne je `[P]`, ne prazan prolaz. Kuka `PageHeaderRight` izvrši isti `SETDEFAULTS`, stavi stupac u CB i zamijeni `@@tkosmomi` prije zaglavlja (`Form1.vb` 6001, `Class1.vb` 966). `[Z]` znači da se desno zaglavlje poklopilo s tim `nazivom`, ne sa sirovim tokenom. Sažetak 164 / 145 / 19.
- `LVO-20` potvrđen. Run [43300179394](https://github.com/tomic04/netgen-u-carpo/actions/runs/43300179394) (`7412885`) je zelen. `[Z] forma=INSERTLVGLA param=@hrkadroviid = 0 onlytree=bez-mastera stablo=spLVGla_Tree`. Gumb `Unos` je `INSERTLVGLA(@hrkadroviid = 0)`: ime staje prije zagrade (`Form2.vb` 4407–4411). `Osvježi drvo` otvara `REFRESHTREEFORM` s `FILLEXPLORER=onlytree`, pa `Form1.vb` 5475 puni samo stablo (`spLVGla_Tree`) i ne šalje master `spLVGla_Query`. Stari pečat `onlytree.param=samo-stablo` je maknut. Stavka je već u zatvorenom stablu. Sažetak i dalje 164 / 145 / 19.
- `KBP-20` potvrđen. Run [43301799321](https://github.com/tomic04/netgen-u-carpo/actions/runs/43301799321) (`0413400`) je zelen. `[Z] click=OK lanac=close|REPX/@@template (@id=@@BLPrometGlaId)`. `Ispiši` otvara `ISPISDOK_FORM`, `ONLOADED` je `click(Ok)`. `Form2.vb` 7151–7154 nađe gumb `ok` i pusti `ONCLICK` `close#:REPX/@@template (@id=@@BLPrometGlaId)`. `CLOSE` zove `Button2_Click` i ne izlazi iz petlje (`7033`), pa sljedeći `:REPX` i dalje ide (`6882`). Test pada ako lanac ostane samo `close`. Stari pečat `click.ok=repx-template` je maknut. Stavka je već u zatvorenim gumbima. Sažetak i dalje 164 / 145 / 19.
- `MPK-43` potvrđen. Run [43303144856](https://github.com/tomic04/netgen-u-carpo/actions/runs/43303144856) (`50c6c5b`) je zelen. `[Z] dnevnik=NetGen SQL sql=spMPPoravnajKP`. Gumb `Parametri` otvara `FILTERFORM`. `ONSUBMIT` je `spMPPoravnajKP @action=1`. `@@operateriid` se zamijeni, pa u tekstu nema `@@`. `Class1.vb` 138 je `NapuniCBLog(Sql)` za poslani SQL. Port to piše kao `NgDnevnik.Sql`: razina `InfoData`, opis `NetGen SQL`, ne red `CBLog`. Stari pečat `dnevnik=NetGen SQL` bez SQL-a je maknut. Stavka nije bila među 19 otvorenih. Sažetak i dalje 164 / 145 / 19.
- `FAK-11` na tom runu nije dokaz. `[Z]` kaže `objekti postoje: spFakPartneriUvjetiQuery`. To je ime u `sys.objects`, ne `Form1.vb` 3882 ni `Class1.vb` 113–118. Potvrda ostaje run [43284832210](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284832210) (`08a871c`): 3 reda, 11 stupaca, bez `Invalid object name 'FakPartneriUvjeti'`. Sažetak i dalje 164 / 144 / 20.
- `FILLEXPLORER` nakon OK odbijen. Run [43288194563](https://github.com/tomic04/netgen-u-carpo/actions/runs/43288194563) (`fbc9f4c`) ima `PASS fillexplorer-statusi | spStatusiQuery @action=0, @node=@@node`. To je `SqlNakonOk` na jednoj formi `POPUNI_FORM`. `Form1.vb` 5445–5452 nakon OK zove `PokaziUpit`, a uz `!!` ne lijepi parametre, inače lijepi `strParametri`. Test ne otvara dijalog i ne puni master. `POPUNI_FORM` nema kontrola, pa NetGen uopće ne zove `ShowDialog` (`5369–5370`). `MPPorKPNG` `FILTERFORM` ima kontrole, a `SqlNakonOk` uvijek zove `Lijepi` s praznim parametrima. Nije nova zatvorena stavka. Sažetak i dalje 164 / 143 / 21.
- `ONLOAD2` po imenu stupca odbijen. Isti run ima `PASS onload2-disable`. `OnLoad2.Akcija` samo razvrsta tekst ćelije: `enable`, `disable`, prazno, inače `natpis`. `Form1.vb` 3914–3921 čita `Rows(0)(Button.Text)`, a `4013–4035` prazno preskače, `refresh` ne dira kontrolu, ostalo postaje natpis osim na `SearchBox`. Test ne daje red ni natpis gumba. `MPPonuda` ima stupac `Dodaj`, ne naredbu `INSERTFORM1`. Dijaloški `ONLOAD2` (`btdodaj`) nije na gumbu ljuske. Nije nova zatvorena stavka. Sažetak i dalje 164 / 143 / 21.

[#72](https://github.com/tomic04/netgen-u-carpo/pull/72) stavlja klijentski XML izvan `definitions/netgen`. `TRZ-01`, `POR-01`, `RFA-00` i `REA-00` ostaju otvoreni. [#71](https://github.com/tomic04/netgen-u-carpo/pull/71) ispravno ne šalje `@@godina` (`Class1.vb` 113). To nije zatvorena stavka.

Ostaje otvoreno 28: XML `TRZ-01`, `POR-01`, `RFA-00`, `REA-00`; pečati `JEZ-69`, `KAR-24`, `DOS-23`, `INV-22`, `FAK-21`, `INV-21`, `NAR-20`, `NAR-21`, `DOB-21`, `RIJ-40`, `RIJ-41`, `RIJ-42`, `OKL-77`, `MPB-20`, `KAR-40`, `DOS-20`; gumbi bez izvršenja `KUP-02`, `KUP-03`, `KUP-04`, `KUP-10`, `KUP-11`, `KUP-12`, `KUP-13`, `KUP-15`.

Uzorak po tipu: samo-grid `OznakaKL` (spin). Šifrarnik `JeziciNG`: `JEZ-56` pravilo, `JEZ-31` spin. Manager `KntBlaZak` i `PrijemniList`: čvor je pravilo, `PL-32` je spin. Stablo-master `Statusi`: `STA-33` i `STA-35` su pravilo. `RIJ-40`–`42` su otvoreni pečati.

Klik kroz granule, podforme i gumbe, za implementaciju: upis (`insert`/`update`/`delete`) smije samo baza `spin` na `sql-ai`. Fiskalizacija smije samo testni URL, ne `cis.porezna-uprava.hr`. `GRESKA` mora srušiti korak. Trenutno: `JeziciNG` `Unesi` ne ostavi red; `Statusi` `Unesi` šalje NULL `VrstaDokId`; `Izmjeni` ostavi `@@statusiid`; `KntKompVerizne` `Obriši` ostavi `@@broj` i `ThreadExceptionDialog` (`KeyNotFound`) pa `ShowDialog` ne izađe. `Odustani` nije `Button2` (`Form2.vb` 3400–3402). Zadnji zeleni brzi run je [43343354817](https://github.com/tomic04/netgen-u-carpo/actions/runs/43343354817) (`efb27aa`), to nije ovaj klik. CI ove QA grane nije zelen: zadnji izvršeni run [43303275147](https://github.com/tomic04/netgen-u-carpo/actions/runs/43303275147) je crven, kasniji zapisi ne pokreću CI da ne zauzmu jedini `sql-ai`.

`PL-25` ostaje zatvoren kao pravilo. Bez `ONLOAD` master ostaje prazan: `strStartupStora` krene kao `""` (`Form1.vb` 3124), `ONLOAD` koji ne postoji ne prepiše ga (`3368`, `On Error Resume Next` od `3065`), a `PokaziUpit` na prazan SQL izađe (`1830`). `SETDEFAULTS` se ipak izvrši prije toga (`3862–3866`, `SetDefolte` šalje SQL na `3958–3962`). Zato trag na ovoj grani ima tekst `SETDEFAULTS`, pa ` || `, pa prazan master. [#66](https://github.com/tomic04/netgen-u-carpo/pull/66) prazni `sqlTrag` kad nema `ONLOAD`. To gasi master, ali briše dokaz da je `SetDefolte` poslan. Test i dalje traži oba dijela.

Grid je detail, kvačica, suma, sort i stupci mastera. Stablo je čvor, lijevi panel i `onlytree`. Combo je odjeljak 13. Gumbi/lanac su naredba, `OK`/`CLOSE`, `NEXTFORM`, `FILLEXPLORER` i `FOREACH`. Dijalog je datum, raspored, prazan broj, tražilica i kontrole na formi. Ispis je `PRINTREPX`. Ostalo je izbornik, `totalgen`, XML kojeg nema i `ssMeni`.

Što korisnik donosi s klijenata (SQL koji fali na spinu i 63 XML-a) je u [fali-od-korisnika.md](fali-od-korisnika.md). Jupiter nije pitan, pa nijedan objekt nije zapisan kao odsutan na oba Jupitera. Stavke se tim popisom ne zatvaraju.

Od 19 otvorenih, 4 su XML (`TRZ-01`, `POR-01`, `RFA-00`, `REA-00`), a 15 je pečat: `JEZ-69`, `KAR-24`, `DOS-23`, `INV-22`, `FAK-21`, `INV-21`, `NAR-20`, `NAR-21`, `DOB-21`, `RIJ-40`, `RIJ-41`, `RIJ-42`, `OKL-77`, `MPB-20`, `KAR-40`. Naredbe `NRD-*` nisu u 164 i ne mijenjaju broj. `KAR-12`, `INV-13`, `DOB-08`, `MPB-07`, `RIJ-30` i `RIJ-32` su izvan checkliste.

Stari CI `b7d8989` ima 571 / 411 / 160. Novi `ef710b9` ima 354 / 341 / 13. Od 160, 117 je još u katalogu (13 i dalje otvoreno, 104 zeleno). 42 prave stavke nema u novom katalogu. 36 ima NetGen dokaz i ostaje zatvoreno. Šest je izbačeno bez dokaza i ostaje otvoreno: `STA-12`, `MPP-20`, `PL-32`, `MPK-30`, `MPK-31`, `STA-34`.

[#75](https://github.com/tomic04/netgen-u-carpo/pull/75) bira Manager samo uz stablo, detail koji nije `:forma` i gumbe (`Form1.vb` 4310–4316). To nije stavka iz 164.

Izvor polaznog izvještaja je CI [#34](https://github.com/tomic04/netgen-u-carpo/pull/34), commit `30236ff`, `SVE ukupno=624 zeleno=460 otvoreno=164 pad=0`.

Svih 164 stavki je otvoreno u izvještaju [#34](https://github.com/tomic04/netgen-u-carpo/pull/34). Nijedna nije pad. Grupe su po elementu NetGena. Tablica ispod i dalje opisuje to stanje. Novije stanje, prema implementaciji [#50](https://github.com/tomic04/netgen-u-carpo/pull/50), [#52](https://github.com/tomic04/netgen-u-carpo/pull/52), [#54](https://github.com/tomic04/netgen-u-carpo/pull/54), [#55](https://github.com/tomic04/netgen-u-carpo/pull/55) i [#56](https://github.com/tomic04/netgen-u-carpo/pull/56), vrh je `cursor/netgen-nextform-3266` (`5015fe1`), stoji u sljedećem odjeljku.

## Stanje prema #50, #52, #54, #55 i #56

Vrh je `cursor/netgen-nextform-3266` (`5015fe1`). Zeleno je samo ono što kuka `GranulaIzvrsenje` vrati testu, bez upita. Provjera deset Biovega granula na Jupiteru je otkazana. `jupiter2019` i `jupiter2022` se ne diraju, ni `SELECT`.

| Stavka | Test | Zašto je zeleno |
|---|---|---|
| OznakaKL `OKL-61` | `dblclick` → `forma=FRMEDIT` | `MASTERDBLCLICK` je `FRMEDIT`. Kuka ne šalje SQL. |
| JeziciNG `JEZ-58` | `dblclick` → `forma=Izmijeni` | `MASTERDBLCLICK` je `Izmijeni`. Kuka ne šalje SQL. |
| JeziciNG `JEZ-67` | `kontrola` `Izmijeni.buttonCLOSE` → `refreshMaster=false` | `ONCLICK` je `CLOSE`. `DijalogKlik` ne šalje SQL. |
| OznakaKL `OKL-74` | `gumb` `Izmjena` → `acceptButton` prazan | `Raspored.KljuceviForme` stavi prazan `acceptButton`. Nema upita. |
| KarticaPartnera `KAR-21` | `otvori` → `uzvicnik=dva-znacenja` | `ONLOAD` je `:FILTERFORM`, master se ne šalje. `Lanac.Caption` i `Lanac.Lijepi` daju oba značenja `!!`. |
| KarticaPartnera `KAR-23` | `otvori` → `prazanSegment=nastavi` | Isti otvor bez master SQL-a. `##OK` i dalje zatvara. |

To je 6 stavki. `OKL-72` ostaje otvoren: akcija testa je `odustani`, a kuka je ne prima (`Izvedeno=false`). `KUP-17` ima ključ `okStay` tek nakon uspjelog otvaranja. `KUPNG` ulaz je `spKUPPrometQueryN`. Ako taj upit padne, test ostane otvoren. Upit nije ponovno slan.

Ostale stavke ostaju otvorene. Kuka na `otvori` ih ne zatvara, ili ljuska ih ne izvrši:

- `refreshmaster` klasifikator vidi (`Lanac.OsvjezavaZadnjiSql`). Ljuska osvježi samo na `REFRESHMASTERGRID`. Otvoreno: `JEZ-65`, `NEF-20`, `PL-35`, `OKL-69`, `JEZ-68`.
- `NEXTFORM` se pročita i ne otvori se nakon `OK`. Forma koje nema samo zabilježi da je nema. Otvoreno: `JEZ-63`, `JEZ-64`.
- SQL s gumba ide u `Klik.Naredbe` i `GumbKlik` ga ne pošalje. Otvoreno: `JEZ-61`, `JEZ-70`, `OKL-70`, `OKL-71`.
- Combo i grid u dijalogu pošalju samo `select` koji zaštita ne označi kao proceduru ili upis. `sp` se preskoči. Širina iz zadnje četvorke i skriven `ID` nisu u tom punjenju.
- `FILLEXPLORER=true` složi `MasterSql`. Ljuska ga nakon dijaloga ne stavi u master. Otvoreno: `MPK-30`, `MPK-31`, `STA-34`.

Grid test na vrhu (#56) posebno drži `rupa-zakljuci` (`:Zakljucavanje` otvara formu i ne šalje SQL) i `rupa-pokazi` (`FILLEXPLORER` složi SQL, a na Jupiteru ga ne šalje). To nisu stavke iz ovih 164. Master ljuske se i dalje ne zamijeni.

`smoke-jupiter.md` se ne čita kao dokaz. Brojači su prazni, a novi upit prema Jupiteru nije dopušten.

| # | Element | Granula | Stavki |
|---|---|---:|---:|
| 1 | XML nije u spin ssMeni | 10 | 12 |
| 2 | Naredba dijaloga, dvostruki klik i refresh mastera | 5 | 18 |
| 3 | Datum: format, ključne riječi, krivi atribut | 5 | 6 |
| 4 | Detail grid: veza, jednokratne kolone, selector | 4 | 9 |
| 5 | totalgen i ASSEMBLYLOAD | 4 | 6 |
| 6 | XML izbornika nije u repou | 4 | 4 |
| 7 | onlytree, CLICK, enable po natpisu, dnevnik | 4 | 4 |
| 8 | Kvačica, suma, sort, edit, Enter i selekcija | 3 | 14 |
| 9 | Raspored dijaloga: /14, tab, label, ONLOAD polja | 3 | 12 |
| 10 | Stupac mastera: tip, širina, skrivanje, poredak | 3 | 11 |
| 11 | Stablo iz SQL-a i klik na čvor | 3 | 10 |
| 12 | Radio, check, OKSTAY, CLOSE i REFRESHCONTROL | 3 | 6 |
| 13 | Ovisni combo, multidrop, status, WUDTH, TRANSLATE | 3 | 5 |
| 14 | Prazan broj, apostrof i spRaiserror | 3 | 5 |
| 15 | FILLEXPLORER i prazan početni master | 3 | 4 |
| 16 | Ispis mastera, zaglavlje i Spin.Printing | 3 | 4 |
| 17 | Procedura ili tijelo nisu dostupni za čitanje | 3 | 3 |
| 18 | Lijevi panel: prazno stablo, combo, splitter | 2 | 9 |
| 19 | Izbornik, help, timer, PERMISIO, popup, ikona | 1 | 7 |
| 20 | !!, +FOCUS, prazan ## i repx #CLOSE | 1 | 4 |
| 21 | Temp tablica i FOREACH | 1 | 4 |
| 22 | Dijaloško stablo, web i slika | 1 | 3 |
| 23 | Tražilica | 1 | 2 |
| 24 | Redoslijed punjenja podloge | 1 | 1 |
| 25 | fidus nema tablicu ssMeni | 1 | 1 |

Zbroj stavki je 164.

## 1. XML nije u spin ssMeni

**10 granula, 12 stavki:** FakPar, KUPDostKN, KUPDostKnNGBvg, KUPNG, KntDospjeleStavke, Kulture, MPNGPregDokMAIL, MPNarIzdSkl, Rijetke, Statusi.

**NetGen:** Test traži red u `dbo.ssMeni` čiji `Parametar` sadrži `asm:NetGenDLL.loader` ili `asm:NetGenDLL.frm` i točno ime datoteke. Prazan rezultat je 0 redova.

**Nedostatak:** Port ne ovisi o ovom retku da bi XML radio. Stavka ostaje otvorena dok izbornik na spinu nema tu granulu. Ponašanje granule ide iz XML-a u repou.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Statusi `STA-12` | Statusi.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| Kulture `KUL-12` | Kulture.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| KUPNG `KUP-05` | KUPNG.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| KntDospjeleStavke `DOS-10` | KntDospjeleStavke.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| FakPar `FAK-10` | FakPar.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| MPNarIzdSkl `NAR-09` | MPNarIzdSkl.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| MPNGPregDokMAIL `MAIL-07` | MPNGPregDokMAIL.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| KUPNG `KUC-07` | KUPNG.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| KUPDostKN `KND-06` | KUPDostKN.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| KUPDostKnNGBvg `BVG-07` | KUPDostKnNGBvg.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| Rijetke `RIJ-31` | RatArkod.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |
| Rijetke `RIJ-33` | RobaWebKlase.xml nije u ssMeni.Parametar | `dbo.ssMeni` | spin ssMeni nema red |

## 2. Naredba dijaloga, dvostruki klik i refresh mastera

**5 granula, 18 stavki:** JeziciNG, KntPDVObrasciEU, MPRACNEFISK, OznakaKL, PrijemniList.

**NetGen:** `NovaGenerickaForma` cijepa po `#` (`Form1.vb` 4557–4561). `REFRESHMASTERGRID` jedini ponovi zadnji SQL (`4570–4577`). `refreshmaster` traži formu; prazna forma i dalje ima `OKClick`, pa `PokaziUpit` (`5354–5370`, `5519`). `OK` zatvara, `OKSTAY` ostaje (`Form2.vb` 6884–6890). `CLOSE` ne šalje SQL (`7033`, `Form1.vb` 5429). Enter na gridu koji ima `ONENTERKEY` otvara tu formu (`Form1.vb` 7021–7023). Dvostruki klik zove `MASTERDBLCLICK` (`6935–6936`).

**Nedostatak:** Kuka još ne izvršava lanac dijaloga. Port mora otvoriti imenovanu formu, ostati na `OKSTAY`, osvježiti master i kad riječ nije `REFRESHMASTERGRID`, i ne otvoriti formu koje nema.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-61` | Dvostruki klik otvara FRMEDIT | `netgen/Form1.vb:6935-6936` | dijalog još nije u kuki |
| OznakaKL `OKL-69` | Prihvati izvrsi update tek nakon zatvaranja i onda osvjezi master | `netgen/Form2.vb:3367-3396; netgen/Form1.vb:5510-5519` | dijalog još nije u kuki |
| OznakaKL `OKL-70` | Update ne dobije zalijepljene parametre; @@oznaka2 i @@ID su zamijenjeni | `netgen/Form2.vb:3378-3388; netgen/Class1.vb:740-851` | dijalog još nije u kuki |
| OznakaKL `OKL-71` | Apostrof u tekstu ide kao '' (preko ¸¸), zarez se vrati iz ~~ | `netgen/Form2.vb:3655; netgen/Class1.vb:87,850` | dijalog još nije u kuki |
| OznakaKL `OKL-72` | Odustani ne salje SQL i ne osvjezava master | `netgen/Form2.vb:3400-3402; netgen/Form1.vb:5429` | dijalog još nije u kuki |
| OznakaKL `OKL-73` | Escape zatvara bez OKClick i bez SQL-a | `netgen/Form2.vb:4161-4166` | dijalog još nije u kuki |
| OznakaKL `OKL-74` | Nema AcceptButton: Enter u tekstu ne radi Prihvati | `netgen/Form2.vb:3367` | dijalog još nije u kuki |
| JeziciNG `JEZ-58` | Dvostruki klik otvara Izmijeni | `netgen/Form1.vb:6935` | dijalog još nije u kuki |
| JeziciNG `JEZ-61` | Spremi na Unesi ostaje na formi (OKStay), Naziv se isprazni | `netgen/Form2.vb:6884-6887,4627-4633,4970-4979` | dijalog još nije u kuki |
| JeziciNG `JEZ-63` | Odustani na Unesi je naredba ok: OKClick true, pa NEXTFORM Dodaj | `netgen/Form2.vb:6888; netgen/Form1.vb:5381` | dijalog još nije u kuki |
| JeziciNG `JEZ-64` | Forma Dodaj ne postoji: nema poruke, 0 kontrola i dalje OKClick | `netgen/Form1.vb:5354-5370` | dijalog još nije u kuki |
| JeziciNG `JEZ-65` | Segment refreshmaster nije REFRESHMASTERGRID, ali prazna forma ipak osvježi zadnji SQL | `netgen/Form1.vb:4570,4926-5370,5519` | dijalog još nije u kuki |
| JeziciNG `JEZ-67` | Izmijeni Odustani je CLOSE: nema SQL-a i nema refresha | `netgen/Form2.vb:7033; netgen/Form1.vb:5429` | dijalog još nije u kuki |
| JeziciNG `JEZ-68` | Izmijeni Spremi zatvori formu i osvježi master (Zadnji=true) | `netgen/Form2.vb:6888; netgen/Form1.vb:5519` | dijalog još nije u kuki |
| JeziciNG `JEZ-70` | Azur na Unesi ide kroz Rafalno, ne kroz lijepljenje ONSUBMIT; @@ se zamijeni, drugi niz @ime= se ne dodaje, upis se ne šalje u testu | `netgen/Form2.vb:7510,3655` | dijalog još nije u kuki |
| MPRACNEFISK `NEF-20` | Treci segment osvjezi zadnji master iako rijec nije REFRESHMASTERGRID | `netgen/Form1.vb:4570,5519` | dijalog još nije u kuki |
| PrijemniList `PL-35` | REFRESHMASTER na dvostrukom kliku ne osvjezava preko rijeci REFRESHMASTERGRID | `netgen/Form1.vb:4570` | SELECT na spinu nije prošao |
| KntPDVObrasciEU `PDV-20` | Enter otvara IZMIJENI_FORM | `netgen/Form1.vb:7021-7023` | kuka nema ključ |

## 3. Datum: format, ključne riječi, krivi atribut

**5 granula, 6 stavki:** DobUgovori, KarticaPartnera, MPInventura, MPNarIzdSkl, MPPorKPNG.

**NetGen:** Parametar datuma je `'yyyyMMdd'` (`Form2.vb` 3531–3532). `firstdayofmonth` složi `01.` + mjesec + `.` + godina (`2260–2261`). `DEFALUT` nije `DEFAULT` (`2252`). `datetiem` i dalje crta datum (`2232`). `SHOWCHECKBOX` bez `CHECKED` šalje null (`2277–2278`, `3566`).

**Nedostatak:** Kuka nema ključ datuma, ili SELECT na spinu padne prije nego što se vrijednost provjeri. Port ne smije uvesti `today` kao ključnu riječ i ne smije čitati `DEFALUT`.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| KarticaPartnera `KAR-20` | Datum ide kao yyyyMMdd u navodnicima | `netgen/Form2.vb:3531-3532` | kuka nema ključ |
| MPPorKPNG `MPK-41` | Datum nije DATE1. U parametru je danas kao yyyyMMdd, broj je 0 | `netgen/Form2.vb:2251-2267,3530-3532,2084` | dijalog još nije u kuki |
| MPInventura `INV-20` | datumod je prvi dan ovog mjeseca, format 01.m.gggg | `netgen/Form2.vb:2260-2261` | SELECT na spinu nije prošao |
| MPInventura `INV-21` | DEFALUT=today se ignorira | `netgen/Form2.vb:2252` | SELECT na spinu nije prošao |
| MPNarIzdSkl `NAR-20` | datetiem se crta kao datum | `netgen/Form2.vb:2232` | SELECT na spinu nije prošao |
| DobUgovori `DOB-20` | SHOWCHECKBOX bez CHECKED salje null | `netgen/Form2.vb:2277-2278,3566` | kuka nema ključ |

## 4. Detail grid: veza, jednokratne kolone, selector

**4 granula, 9 stavki:** Intrastat, KntBlaZak, PrijemniList, Statusi.

**NetGen:** Promjena reda mastera ponovo zove `DETAILGRID` (`Form1.vb` 7413–7416). Za vrijeme punjenja mastera detail se preskoči (`2270`, `2099–2109`). `DETCHECKBOXVISIBLE=FALSE` sakrije stupac 1 detaila (`2402–2406`). Drugi upit ne radi `CreateGridColumns` ako `RootTable` već postoji (`2334–2336`). `CLEARCOLUMS` na detailu sakrije stupce izvan `DETAILCOLUMNS` (`2347`, `7459`). `@@id` ide u detail SQL (`Class1.vb` 99).

**Nedostatak:** Kuka nema ključeve detail toka, ili SQL na spinu nije prošao. Jedan grid ne smije dijeliti master kvačicu ni ponovo stvarati stupce na svakom detalju.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Statusi `STA-36` | Promjena reda mastera ponovo zove DETAILGRID | `netgen/Form1.vb:7413-7416` | kuka nema ključ |
| Statusi `STA-37` | Tijekom punjenja mastera detail se preskace (PunimMasterGrid), pa jednom na kraju | `netgen/Form1.vb:2270,2099-2109` | kuka nema ključ |
| Statusi `STA-38` | DETCHECKBOXVISIBLE FALSE sakrije selector detaila (stupac 1) | `netgen/Form1.vb:2402-2406` | kuka nema ključ |
| Statusi `STA-39` | Drugi detail upit ne radi CreateGridColumns ako RootTable vec postoji | `netgen/Form1.vb:2334-2336` | kuka nema ključ |
| KntBlaZak `BLA-33` | Detail se ponovi na SelectionChanged mastera | `netgen/Form1.vb:7413-7416` | kuka nema ključ |
| KntBlaZak `BLA-34` | CLEARCOLUMS detaila sakrije stupce koji nisu u DETAILCOLUMNS | `netgen/Form1.vb:2347,7459` | kuka nema ključ |
| PrijemniList `PL-23` | Detail SQL je declare/exec | `netgen/Form1.vb:3373` | SELECT na spinu nije prošao |
| PrijemniList `PL-33` | Detail se veze na trenutni @@id nakon master reda | `netgen/Form1.vb:2294-2304,7413` | SELECT na spinu nije prošao |
| Intrastat `INT-20` | Master timeout je 300 i detail salje @@id | `netgen/Class1.vb:99` | SELECT na spinu nije prošao |

## 5. totalgen i ASSEMBLYLOAD

**4 granula, 6 stavki:** KUPDostKnNGBvg, KUPNG, KntOPZ-STAT-1, MPNGPregDokMAIL.

**NetGen:** Shell bez tilde zove `totalgen(id, operater, konekcija, zadnji SQL mastera, parametri grida)` i povrat ne čita (`Form1.vb` 4599–4623, `5275–5278`). Dijalog šalje `strParametriSvi` dvaput (`Form2.vb` 7424–7456). `Load` ili `CreateInstance` koji padne upiše log i izađe (`4600–4611`). `Ime.Start` nije `ASSEMBLYLOAD`: Eporezna ne otvara dijalog i ne učitava DLL (`5369–5370`).

**Nedostatak:** Kuka nema ovaj poziv, ili SQL padne prije njega. Port ne smije čitati povrat `totalgen` i ne smije `Ime.Start` pretvoriti u učitavanje DLL-a.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| MPNGPregDokMAIL `MAIL-20` | totalgen(id, operater, konekcija, zadnji SQL mastera, parametri grida). Povrat se ne cita | `netgen/Form1.vb:4599-4623` | SELECT na spinu nije prošao |
| MPNGPregDokMAIL `MAIL-21` | Load ili CreateInstance padne: CB log i Exit Sub | `netgen/Form1.vb:4600-4611` | SELECT na spinu nije prošao |
| KUPNG `KUC-20` | Dijalog totalgen: id, operater, konekcija, strParametriSvi, strParametriSvi | `netgen/Form2.vb:7424-7456` | SELECT na spinu nije prošao |
| KUPNG `KUC-21` | Shell bez tildu: zadnji SQL mastera i ParametriSaGrida. Povrat se ne cita | `netgen/Form1.vb:4623,5275-5278` | SELECT na spinu nije prošao |
| KUPDostKnNGBvg `BVG-20` | clsCarpoKupUra i clsCarpoKnjigaUra dijele totalgen, razlikuje se CarpoForm | `netgen/Form2.vb:7456` | SELECT na spinu nije prošao |
| KntOPZ-STAT-1 `OPZ-20` | Eporezna ne otvara dijalog i ne ucitava DLL | `netgen/Form1.vb:5369-5370` | dijalog još nije u kuki |

## 6. XML izbornika nije u repou

**4 granula, 4 stavki:** Poruke, RealizacijaFakture, RealizacijaStavkeNG, TecRazOTV.

**NetGen:** Stavka ostaje otvorena dok datoteka iz `ssMeni.Parametar` ne stigne u `definitions/netgen`. Agronom primjerak, kad postoji, nije menijska putanja `MER\`.

**Nedostatak:** Port ne može provjeriti ponašanje granule bez tog XML-a. Menijska putanja i primjerak u drugom folderu nisu ista datoteka.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| TecRazOTV `TRZ-01` | TecRazOTV.xml nije u definitions/netgen | `docs/granule/ssMeni-svi-serveri.md` | nema datoteke, checklista ponašanja čeka XML |
| RealizacijaStavkeNG `REA-00` | MER\RealizacijaStavkeNG.xml nije u repou. Provjera je Agronom primjerak | `docs/granule/Prioritet.md` | C:\actions-runner\_work\netgen-u-carpo\netgen-u-carpo\definitions\netgen\MER\RealizacijaStavkeNG.xml |
| RealizacijaFakture `RFA-00` | MER\RealizacijaFakture.xml nije u repou | `docs/granule/Prioritet.md` | C:\actions-runner\_work\netgen-u-carpo\netgen-u-carpo\definitions\netgen\MER\RealizacijaFakture.xml |
| Poruke `POR-01` | Poruke.xml nije u definitions/netgen | `docs/granule/ssMeni-svi-serveri.md` | otvoreno dok XML ne stigne |

## 7. onlytree, CLICK, enable po natpisu, dnevnik

**4 granula, 4 stavki:** KntBlaPromet, LVObracun, MPPonuda, MPPorKPNG.

**NetGen:** `FILLEXPLORER=onlytree` zove samo `NapuniDrvoVrijeme` (`Form1.vb` 5475). `Ime(@a=)` nije ime forme (`Form2.vb` 4407). `CLICK(Ok)` izvrši `ONCLICK` te kontrole (`Form2.vb` 7151–7155). `enable`/`disable` traži `Button.Text` (`Form1.vb` 3914–3921). SQL filtera ide u NgDnevnik, ne u CB log.

**Nedostatak:** Četiri odvojene rupe. Kuka nema `onlytree` ni `CLICK`. `MPPonuda` stoji na SELECT-u koji traži `@@operateriid`. Dnevnik filtera nije CB log.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| LVObracun `LVO-20` | onlytree ne salje master. Ime(@a=) nije ime forme | `netgen/Form1.vb:5475; netgen/Form2.vb:4407` | kuka nema ključ |
| KntBlaPromet `KBP-20` | CLICK(Ok) izvrsi ONCLICK, ne samo zatvaranje | `netgen/Form2.vb:7151-7155` | kuka nema ključ |
| MPPonuda `MPP-20` | enable/disable trazi Button.Text, ne NAME | `netgen/Form1.vb:3914-3921` | SELECT na spinu nije prošao |
| MPPorKPNG `MPK-43` | SQL filtera ide u NgDnevnik, ne u CBLog | `docs/funkcionalnosti/logging.md` | dijalog još nije u kuki |

## 8. Kvačica, suma, sort, edit, Enter i selekcija

**3 granula, 14 stavki:** MPPorKPNG, OznakaKL, PrijemniList.

**NetGen:** `CHECKBOXVISIBLE=FALSE` sakrije stupac 1 (`Form1.vb` 2043–2046). Suma na kraju slijedi `TOTALROWVISIBLE`, iako stupci prvo uključe total (`Class1.vb` 1363, `Form1.vb` 2049–2050). `GROUPBYBOXVISIBLE` i `FROZENCOLUMNS` ostaju kako su napisani (`3350`, `3362`). `AUTOMATICSORT` koji nije `false` ostavi sort klikom (`3401–3404`). Prazan `EDITCOLUMNS` i nepročitan `GRIDALLOWEDIT` gase edit (`3466`, `Class1.vb` 1137–1139). `EditMode` je `EditOff` (`Class1.vb` 1003). Nema `ONENTERKEY`: Enter ne otvara formu (`3419–3423`). Selekcija je višestruka (`Form1.designer.vb` 654). `IncrementalSearch` je `AllCharacters` (`641`). Grid je `Dock Fill` (`1906`). Ako ima redova, fokus je na gridu (`2059–2072`).

**Nedostatak:** SELECT OznakaKL i Prijemnog lista na spinu nije prošao, pa se ova stanja nisu potvrdila na napunjenom gridu. Janus ikona i poravnanje Far su izgled – Carpo. Kvačica, suma, sort i edit ostaju NetGen.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-39` | CHECKBOXVISIBLE FALSE sakrije checkbox stupac. Janus Icon 22 je izgled – Carpo | `netgen/Form1.vb:2043-2046` | SELECT na spinu nije prošao |
| OznakaKL `OKL-44` | TotalRow na kraju FALSE iako CreateGridColumns prvo stavi True | `netgen/Class1.vb:1363; netgen/Form1.vb:2049-2050` | SELECT na spinu nije prošao |
| OznakaKL `OKL-46` | GroupByBox je skriven, FrozenColumns je 0 | `netgen/Form1.vb:3350,3362` | SELECT na spinu nije prošao |
| OznakaKL `OKL-47` | FrozenColumns je 0 | `netgen/Form1.vb:3362` | SELECT na spinu nije prošao |
| OznakaKL `OKL-48` | AUTOMATICSORT nije false pa sort klikom na zaglavlje ostaje ukljucen | `netgen/Form1.vb:3401-3404` | SELECT na spinu nije prošao |
| OznakaKL `OKL-49` | EDITCOLUMNS prazan i GRIDALLOWEDIT se ne cita: nema editiranja celije | `netgen/Form1.vb:3466; netgen/Class1.vb:1137-1139; netgen/Resource1.Designer.vb:75` | SELECT na spinu nije prošao |
| OznakaKL `OKL-50` | EditMode je EditOff | `netgen/Class1.vb:1003` | SELECT na spinu nije prošao |
| OznakaKL `OKL-51` | Enter na gridu ne otvara formu (EnterKeyBehavior None, nema ONENTERKEY) | `netgen/Form1.vb:3419-3423` | SELECT na spinu nije prošao |
| OznakaKL `OKL-52` | Selekcija je visestruka (designer), ne DX default jednog reda | `netgen/Form1.designer.vb:654` | SELECT na spinu nije prošao |
| OznakaKL `OKL-53` | IncrementalSearch je AllCharacters | `netgen/Form1.designer.vb:641` | SELECT na spinu nije prošao |
| OznakaKL `OKL-57` | Grid je Dock Fill | `netgen/Form1.vb:1906` | SELECT na spinu nije prošao |
| OznakaKL `OKL-58` | Ako ima redova, fokus je na gridu | `netgen/Form1.vb:2059-2072` | SELECT na spinu nije prošao |
| MPPorKPNG `MPK-42` | Checkbox stupac je vidljiv, total je ukljucen, sort klikom ostaje | `netgen/Form1.vb:2043-2053,3401-3404` | kuka nema ključ |
| PrijemniList `PL-24` | CHECKBOXVISIBLE FALSE | `netgen/Form1.vb:2043` | SELECT na spinu nije prošao |

## 9. Raspored dijaloga: /14, tab, label, ONLOAD polja

**3 granula, 12 stavki:** JeziciNG, MPPorKPNG, OznakaKL.

**NetGen:** Širina i visina forme i kontrole dijele se s 14, VB zaokruži na parni (`Form2.vb` 1093–1094, `2945–2950`). Label prikazuje `CAPTION`, ne `TEXT` (`2932–2938`). `ONLOAD` upiše vrijednost stupca (`2016–2018`). Tab ide redom iz XML-a (`3027–3043`). Dvije kontrole mogu dijeliti isti indeks; red je redoslijed u XML-u (`3027–3030`). Fokus nakon `OKSTAY` je imenovana kontrola (`4627–4632`).

**Nedostatak:** Dijalog još nije u kuki, ili SQL na spinu padne prije crtanja. Port mora primijeniti `/14` i natpis iz `CAPTION`.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-62` | Naslov dijaloga je Izmjena oznake | `netgen/Form2.vb:969` | dijalog još nije u kuki |
| OznakaKL `OKL-63` | Sirina dijaloga je 5112/14, visina 2189/14, prije GetSetting registra | `netgen/Form2.vb:1093-1094,2785-2788` | dijalog još nije u kuki |
| OznakaKL `OKL-64` | Visina dijaloga je 2189/14 | `netgen/Form2.vb:1094` | dijalog još nije u kuki |
| OznakaKL `OKL-65` | Label prikazuje CAPTION, ne TEXT=Label16 | `netgen/Form2.vb:2932-2938` | dijalog još nije u kuki |
| OznakaKL `OKL-66` | Label TOP/LEFT/W/H su jedinice/14, VB zaokruzivanje na parni | `netgen/Form2.vb:2945-2950` | dijalog još nije u kuki |
| OznakaKL `OKL-67` | oznaka2 LEFT je 2002/14 | `netgen/Form2.vb:2947` | dijalog još nije u kuki |
| OznakaKL `OKL-68` | ONLOAD upise oznaka2 iz reda; tekst null postaje prazan | `netgen/Form2.vb:2016-2018` | dijalog još nije u kuki |
| OznakaKL `OKL-75` | Tab: label (auto), pa oznaka2, pa Prihvati, pa Odustani | `netgen/Form2.vb:3027-3043,2775-2776` | dijalog još nije u kuki |
| JeziciNG `JEZ-59` | Unesi: okvir nema natpis FrmJezici | `netgen/Form2.vb:2932-2938` | dijalog još nije u kuki |
| JeziciNG `JEZ-62` | Nakon OKStay fokus je kontrola naziv | `netgen/Form2.vb:4627-4632` | dijalog još nije u kuki |
| JeziciNG `JEZ-66` | Izmijeni ONLOAD napuni Naziv iz stupca | `netgen/Form2.vb:2016` | dijalog još nije u kuki |
| MPPorKPNG `MPK-40` | datumdok i napomena dijele indeks 1. Red je datum pa napomena | `netgen/Form2.vb:3027-3030` | dijalog još nije u kuki |

## 10. Stupac mastera: tip, širina, skrivanje, poredak

**3 granula, 11 stavki:** KntBlaZak, OznakaKL, PrijemniList.

**NetGen:** Tipovi iz `CreateGridColumns` (`Class1.vb` 1087–1244). Ime koje završava na `id` je skriveno (`1124–1126`). String bez `WIDTH` ima širinu 100 (`1201–1204`). Natpis je alias iz SQL-a (`1037`). Poredak redova je poredak SQL-a (`Form1.vb` 1999, `Class1.vb` 1360). `@@id` bez kolone `id` je `Cells(2)` (`Form1.vb` 5566–5582). Širina iz XML-a dijeli se s 14 (`Form1.vb` 7522–7524).

**Nedostatak:** Na OznakaKL SELECT padne jer spin nema stupac `oznaka2`, pa se širina i poredak nisu mogli potvrditi. Port računa širinu iz tipa i `WIDTH/14`, skriva `*id` i ne sortira sam.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-25` | SELECT mastera vraca stupce redom | `netgen/Form1.vb:3882` | SELECT na spinu nije prošao |
| OznakaKL `OKL-26` | ORDER BY sortkey je poredak koji grid mora pokazati | `netgen/Class1.vb:1360` | SELECT na spinu nije prošao |
| OznakaKL `OKL-27` | Tipovi stupaca odreduju sirinu, format i skrivanje *id | `netgen/Class1.vb:1087-1244` | SELECT na spinu nije prošao |
| OznakaKL `OKL-40` | klmasterrobaid je skriven jer ime zavrsava na ID | `netgen/Class1.vb:1124-1126` | SELECT na spinu nije prošao |
| OznakaKL `OKL-41` | Int stupac: sirina 34, format ##0, EditType NoEdit. Poravnanje Far je izgled – Carpo | `netgen/Class1.vb:1087-1139` | SELECT na spinu nije prošao |
| OznakaKL `OKL-42` | String stupac bez WIDTH u MASTERCOLUMNS ima sirinu 100 (nije 200; to je samo hijerarhijski put) | `netgen/Class1.vb:1201-1204` | SELECT na spinu nije prošao |
| OznakaKL `OKL-43` | Natpis stupca DodatnaOznaka je alias iz SQL-a, ne oznaka2 | `netgen/Class1.vb:1037` | SELECT na spinu nije prošao |
| OznakaKL `OKL-59` | Nema kolone id pa je @@id vrijednost Cells(2), prvi podatak (klmasterrobaid) | `netgen/Form1.vb:5566-5582` | SELECT na spinu nije prošao |
| OznakaKL `OKL-79` | Pocetni poredak redova je poredak SQL-a, grid sam ne sortira | `netgen/Form1.vb:1999` | SELECT na spinu nije prošao |
| KntBlaZak `BLA-35` | Sirina VrstaBlagajne je 2500/14 = 179 | `netgen/Form1.vb:7522` | kuka nema ključ |
| PrijemniList `PL-34` | Kolicina na detailu: sirina 1000/14 = 71, format #,####0.0000 | `netgen/Form1.vb:7522-7524` | SELECT na spinu nije prošao |

## 11. Stablo iz SQL-a i klik na čvor

**3 granula, 10 stavki:** KntBlaZak, PrijemniList, Statusi.

**NetGen:** Punjenje ide `SqlDataAdapter.Fill`, ne `ExecSql` (`Form1.vb` 4442–4447). Stupci su 0 id, 1 parent, 2 natpis, 3 tag (`4450`, `4461–4478`). Korijen je samo prvi red. Dijete s id `0` se preskoči (`4455`, `4498`). Tag bez dvotočke postane `vrijednost:ImeForme` (`4474–4477`). `AfterSelect`: `@@node` je lijevo od `:`, forma desno (`7182–7188`). Tag `:` ima prazno ime forme i `NovaGenerickaForma` izađe (`7185–7187`, `4546`). Combo koji vrati TREE tekst upiše `SelectedValue` i ponovo zove `NapuniDrvoVrijeme` (`7730–7733`, `3342–3344`).

**Nedostatak:** Kuka nema ključeve stabla, ili SQL na spinu nije prošao. Carpo `tree` ne smije tražiti imena kolona `key` i `parent`.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Statusi `STA-30` | Stablo iz store: stupci 0 id, 1 parent, 2 natpis, 3 tag; relacija 0->1 | `netgen/Form1.vb:4450,4461-4478` | kuka nema ključ |
| Statusi `STA-31` | Korijen je samo prvi red. Dijete kojem je id 0 se preskace | `netgen/Form1.vb:4455,4498` | kuka nema ključ |
| Statusi `STA-32` | Tag cvora bez dvotocke u stupcu 3 je vrijednost:POPUNI_FORM | `netgen/Form1.vb:4474-4477` | kuka nema ključ |
| Statusi `STA-33` | AfterSelect: @@node je dio taga prije dvotocke, forma iza | `netgen/Form1.vb:7182-7188` | kuka nema ključ |
| Statusi `STA-35` | Punjenje stabla ide SqlDataAdapterom, ne ExecSql: prezivjeli @@ se ipak salje | `netgen/Form1.vb:4446-4447` | kuka nema ključ |
| KntBlaZak `BLA-30` | Klik na Blagajne: tag je ':' pa je ime forme prazno i NovaGenerickaForma izadje | `netgen/Form1.vb:4354,7185-7187,4546` | kuka nema ključ |
| KntBlaZak `BLA-31` | @@operateriid u TREE store se zamijeni prije SqlDataAdapter.Fill | `netgen/Form1.vb:4442-4446` | kuka nema ključ |
| KntBlaZak `BLA-32` | Tri storea su tri korijena, ne podgrane cvora Blagajne | `netgen/Form1.vb:4347-4480` | kuka nema ključ |
| PrijemniList `PL-30` | SelectedValueChanged upise SelectedValue u strTreeNodes i ponovo zove NapuniDrvoVrijeme | `netgen/Form1.vb:7730-7733` | SELECT na spinu nije prošao |
| PrijemniList `PL-32` | Vrijednost combo stupca 0 mora biti TREE tekst (segmenti s # i :) | `netgen/Form1.vb:3342-3344,4344` | SELECT na spinu nije prošao |

## 12. Radio, check, OKSTAY, CLOSE i REFRESHCONTROL

**3 granula, 6 stavki:** FakPar, KUPNG, MPInventura.

**NetGen:** Radio šalje `1` ili `0` (`Form2.vb` 3432–3435). Check šalje `CheckState` (`3424–3427`). `REFRESHCONTROL(ime)` radi `select 'refresh' as ime` (`6911–6915`). `CANCEL` i `CLOSE` su `Button2`; `CLOSEPARENT` još postavi `bCloseParent` (`7031–7037`). `OKSTAY` ne zatvara formu (`6884–6890`).

**Nedostatak:** Na KUPNG i FakPar SQL padne prije nego što se parametar pročita. Radio i check ne smiju ići kao isti editor.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| KUPNG `KUP-14` | Radio šalje 1 ili 0 | `netgen/Form2.vb:3432-3435` | SELECT na spinu nije prošao |
| KUPNG `KUP-15` | REFRESHCONTROL(ime) radi select 'refresh' as ime | `netgen/Form2.vb:6911-6915` | SELECT na spinu nije prošao |
| KUPNG `KUP-16` | CANCEL i CLOSE su Button2. CLOSEPARENT jos postavi bCloseParent | `netgen/Form2.vb:7031-7037` | SELECT na spinu nije prošao |
| KUPNG `KUP-17` | OKSTAY ne zatvara formu, OK zatvara | `netgen/Form2.vb:6884-6890` | SELECT na spinu nije prošao |
| MPInventura `INV-22` | radio rbtnKumulativ je 1 ili 0 | `netgen/Form2.vb:3432-3435` | SELECT na spinu nije prošao |
| FakPar `FAK-21` | checkbox PoSkladistu salje CheckState | `netgen/Form2.vb:3424-3427` | SELECT na spinu nije prošao |

## 13. Ovisni combo, multidrop, status, WUDTH, TRANSLATE

**3 granula, 5 stavki:** DobUgovori, KntDospjeleStavke, MPNarIzdSkl.

**NetGen:** Refresh comboa ponovo puni imenovani combo i vrati `ALLOWEMPTY` red (`Form2.vb` 5158–5176). Multidrop je `id1~id2` bez razmaka (`3446`). Statusstrip drži tekst u tagu velikim slovima (`1258`). `WUDTH` se ne čita (`2949`). `TRANSLATE` prevodi samo stupac prikaza (`2420–2424`).

**Nedostatak:** Kuka nema ove ključeve, ili SQL traži `@@operateriid`. Port ne smije čitati `WUDTH` ni prevoditi stupac vrijednosti.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| KntDospjeleStavke `DOS-20` | statusstrip TEXT1 u tag velikim slovima, @@rowcount i dalje se nadje | `netgen/Form2.vb:1258; netgen/Class1.vb:865; netgen/Form2.vb:3818` | kuka nema ključ |
| KntDospjeleStavke `DOS-21` | zupanijeid refresh ponovo puni mjestaid i vrati ALLOWEMPTY red | `netgen/Form2.vb:5158-5176` | kuka nema ključ |
| KntDospjeleStavke `DOS-23` | multidrop kntplanaid je id1~id2 bez razmaka | `netgen/Form2.vb:3446` | kuka nema ključ |
| MPNarIzdSkl `NAR-21` | WUDTH se ignorira | `netgen/Form2.vb:2949` | SELECT na spinu nije prošao |
| DobUgovori `DOB-21` | TRANSLATE prevodi samo prikaz | `netgen/Form2.vb:2420-2424` | kuka nema ključ |

## 14. Prazan broj, apostrof i spRaiserror

**3 granula, 5 stavki:** FakPar, KntDospjeleStavke, Kulture.

**NetGen:** Prazan number ide kao `0`, prazan text kao navodnici (`Form2.vb` 3601, `3655`). Apostrof postane `¸¸`, pa `''` (`3655`, `Class1.vb` 850). `spRaiserror` bez `#OK` u tekstu poruke ne guta grešku i ne izvrši sljedeći `#OK` (`Class1.vb` 187–189, `Form2.vb` 7511–7515). `CurMEdit` prazan ide kao `0`, ne kao tekst (`2071`, `3601`).

**Nedostatak:** SELECT pada jer procedura nije na spinu, ili kuka nema ključ broja. Pravilo parametra i dalje je NetGen.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Kulture `KUL-20` | Prazan number ide kao 0, prazan text kao navodnici | `netgen/Form2.vb:3601,3655` | SELECT na spinu nije prošao |
| Kulture `KUL-23` | Apostrof u tekstu postaje ¸¸ pa '' u SQL | `netgen/Form2.vb:3655; netgen/Class1.vb:850` | SELECT na spinu nije prošao |
| Kulture `KUL-24` | spRaiserror bez #OK u tekstu poruke ne guta grešku i ne izvrši sljedeći #OK | `netgen/Class1.vb:187-189; netgen/Form2.vb:7511-7515` | SELECT na spinu nije prošao |
| KntDospjeleStavke `DOS-22` | prazan iznos ide kao 0 | `netgen/Form2.vb:3601` | kuka nema ključ |
| FakPar `FAK-20` | CurMEdit prazan ide kao 0, ne kao tekst | `netgen/Form2.vb:2071,3601` | SELECT na spinu nije prošao |

## 15. FILLEXPLORER i prazan početni master

**3 granula, 4 stavki:** MPPorKPNG, PrijemniList, Statusi.

**NetGen:** `true` zamijeni master SQL-om `ONSUBMIT` i parametara (`Form1.vb` 5445–5452). Prazan `FILLEXPLORER` izvrši `ONSUBMIT`, pa ponovi zadnji master (`5509–5519`). Forma bez kontrola ne otvara dijalog (`5369–5370`). Nema `ONLOAD`: početni master SQL je prazan (`3367–3368`, `1830`).

**Nedostatak:** SQL na Prijemnom listu padne na `#select`. Na MPPorKPNG dijalog još nije u kuki. Prazan `FILLEXPLORER` nije `true`.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| MPPorKPNG `MPK-30` | FILTERFORM true zamijeni master SQL-om ONSUBMIT i parametara | `netgen/Form1.vb:5445-5452` | dijalog još nije u kuki |
| MPPorKPNG `MPK-31` | Prazan FILLEXPLORER izvrsi ONSUBMIT pa ponovi zadnji master | `netgen/Form1.vb:5509-5519` | dijalog još nije u kuki |
| Statusi `STA-34` | Forma bez kontrola ne otvara dijalog; FILLEXPLORER zamijeni master ONSUBMIT-om | `netgen/Form1.vb:5369-5370,5445-5452` | kuka nema ključ |
| PrijemniList `PL-25` | Nema ONLOAD: pocetni master SQL je prazan | `netgen/Form1.vb:3367-3368,1830` | SELECT na spinu nije prošao |

## 16. Ispis mastera, zaglavlje i Spin.Printing

**3 granula, 4 stavki:** JeziciNG, KarticaPartnera, MPBlagajna.

**NetGen:** `PRINTREPXMASTER` zove ispis mastera, ne formu tog imena (`Form1.vb` 4945–4948, `5984`). Desno zaglavlje je `@@tkosmomi` iz `SETDEFAULTS` (`6001`, `Class1.vb` 932–966). NetGen ide na Spin.Printing v23 pa v9.1 (`Class1.vb` 297–304). Port zove v26 (`Placeholders.cs` 46–52). Ako ispis padne, `Form6` zove `rptDevRepx.DoPrint2` (`Form6.vb` 134–161, `Class1.vb` 606–632).

**Nedostatak:** Kuka nema ključ ispisa. Verzija DLL-a i fallback na `rptDevRepx` nisu izvedeni.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| JeziciNG `JEZ-56` | Ispiši zove ispis mastera, ne formu tog imena | `netgen/Form1.vb:4945-4948,5984` | dijalog još nije u kuki |
| JeziciNG `JEZ-57` | Desno zaglavlje ispisa dolazi iz @@tkosmomi (SETDEFAULTS u CB) | `netgen/Form1.vb:6001; netgen/Class1.vb:932-966` | dijalog još nije u kuki |
| MPBlagajna `MPB-20` | Spin.Printing.v23 pa v9.1. Port zove v26, ne v23 | `netgen/Class1.vb:297-304; Devex_26/Carpo.NetGen/Placeholders.cs:46-52` | kuka nema ključ |
| KarticaPartnera `KAR-40` | Ako Spin.Printing padne, Form6 zove rptDevRepx.DoPrint2 | `netgen/Form6.vb:134-161; netgen/Class1.vb:606-632` | kuka nema ključ |

## 17. Procedura ili tijelo nisu dostupni za čitanje

**3 granula, 3 stavki:** FakPar, JeziciNG, KntDospjeleStavke.

**NetGen:** `Class1.vb` 85 ne šalje SQL čije tijelo nije dokazano kao samo čitanje. `sys.objects` na spinu nema `spKntPopuniKontroleNG` i `spOrgShemaQuery`. FakPar nema sve objekte.

**Nedostatak:** Ovo nije rupa u parseru. Stavka ostaje otvorena dok se tijelo ne dokaže kao čitanje ili dok objekt ne postoji na spinu. SQL se ne šalje ako nije čitanje.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| JeziciNG `JEZ-31` | spJeziciQuery se ne izvrsava jer tijelo nije dokazano samo citanje | `netgen/Class1.vb:85` | tijelo nije dokazano kao samo čitanje |
| KntDospjeleStavke `DOS-11` | nisu svi objekti na spin | `sys.objects` | spKntPopuniKontroleNG,spOrgShemaQuery |
| FakPar `FAK-11` | nisu svi objekti na spin | `sys.objects` | otvoreno |

## 18. Lijevi panel: prazno stablo, combo, splitter

**2 granula, 9 stavki:** OznakaKL, PrijemniList.

**NetGen:** Prazan `TREE` i prazan combo sakriju `TreeView2`, splitter na 0, gumb drveta disabled (`Form1.vb` 4310–4316). Combo koji nije prazan drži stablo vidljivim iako `TREE` atribut ne postoji (`4310–4320`, `3340–3347`). Splitter 180 tek kad TREE tekst postoji (`4323–4324`). Dok je TREE još prazan, `NapuniDrvo` izađe prije splittera (`4310–4320`). Nema detail SQL: donji splitter ide na visinu minus 50 (`2111–2113`).

**Nedostatak:** Na obje granule SELECT mastera padne prije nego što se raspored potvrdi. Pravilo panela ostaje ovo.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-31` | Prazan TREE i prazan COMBO skrivaju TreeView2 | `netgen/Form1.vb:4310-4316,3345-3347` | SELECT na spinu nije prošao |
| OznakaKL `OKL-32` | Lijevi splitter je 0 i gumb drveta je ugasen | `netgen/Form1.vb:4313-4315` | SELECT na spinu nije prošao |
| OznakaKL `OKL-33` | Gumb za drvo je disabled | `netgen/Form1.vb:4315` | SELECT na spinu nije prošao |
| OznakaKL `OKL-34` | Combo na podlozi je skriven | `netgen/Form1.vb:3345-3347` | SELECT na spinu nije prošao |
| OznakaKL `OKL-35` | Nema detail SQL pa se donji splitter spusti na visinu-50 | `netgen/Form1.vb:2111-2113` | SELECT na spinu nije prošao |
| PrijemniList `PL-20` | COMBO drzi stablo vidljivim iako TREE atribut ne postoji | `netgen/Form1.vb:4310-4320,7730-7733` | SELECT na spinu nije prošao |
| PrijemniList `PL-21` | Combo je vidljiv | `netgen/Form1.vb:3340-3347` | SELECT na spinu nije prošao |
| PrijemniList `PL-22` | Splitter 180 tek kad TREE tekst postoji (nakon SelectedValue) | `netgen/Form1.vb:4323-4324` | SELECT na spinu nije prošao |
| PrijemniList `PL-31` | Dok je TREE jos prazan, NapuniDrvo izadje prije splitter-a i prije Enable gumba | `netgen/Form1.vb:4310-4320` | SELECT na spinu nije prošao |

## 19. Izbornik, help, timer, PERMISIO, popup, ikona

**1 granula, 7 stavki:** OznakaKL.

**NetGen:** Nema `MENU`: izbornik je skriven (`3552–3553`). Nema `HELPKOD`: help gumb je skriven (`3491–3494`). Nema `TIMER`: oznaka timera je skrivena (`3449–3450`). `PERMISIO` nije u XML-u pa default ostaje `true` i šalje se `select ssPermission` (`3153`, `3395`, `3414–3416`). Kontekstni izbornik ima standardne stavke iako `POPUP` ne postoji (`3620–3673`). Nema `ICON`: ikona dolazi iz hosta (`3430–3440`). Vidi se samo gumb koji ima dvotočku (`3827–3837`).

**Nedostatak:** Sve stoji na tome što master SQL OznakaKL na spinu nema stupac `oznaka2`. Pravila okvira su NetGen. Ikona prozora u portu je Manager (izgled – Carpo).

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-36` | Izbornik je skriven (nema MENU) | `netgen/Form1.vb:3552-3553,3449-3450,3491-3494` | SELECT na spinu nije prošao |
| OznakaKL `OKL-37` | Help gumb nije vidljiv (nema HELPKOD) | `netgen/Form1.vb:3491-3494` | SELECT na spinu nije prošao |
| OznakaKL `OKL-76` | PERMISIO nije u XML-u pa default ostaje true i salje se select ssPermission | `netgen/Form1.vb:3153,3395,3414-3416,2706` | SELECT na spinu nije prošao |
| OznakaKL `OKL-77` | Kontekstni izbornik ima standardne stavke iako POPUP ne postoji | `netgen/Form1.vb:3620-3673` | SELECT na spinu nije prošao |
| OznakaKL `OKL-80` | Nema ICON pa ikona dolazi iz hosta (Ikona), ne iz Graphics\Icons | `netgen/Form1.vb:3430-3440` | SELECT na spinu nije prošao |
| OznakaKL `OKL-81` | Nema TIMER: statusna oznaka timera je skrivena | `netgen/Form1.vb:3449-3450` | SELECT na spinu nije prošao |
| OznakaKL `OKL-60` | Vidljiv je samo gumb Izmjena, naredba FRMEDIT | `netgen/Form1.vb:3827-3837,5664-5667` | SELECT na spinu nije prošao |

## 20. !!, +FOCUS, prazan ## i repx #CLOSE

**1 granula, 4 stavki:** KarticaPartnera.

**NetGen:** `!!` u natpisu je novi red, na `ONSUBMIT` gasi lijepljenje (`Form2.vb` 2943, `Form1.vb` 5449). `+FOCUS(ime)` vrati fokus i ne gasi grešku ako nema `+OK` (`Class1.vb` 187–204). Prazan segment `##` ne preskače sljedeći OK (`Form2.vb` 6821–6822). `repx` s `#CLOSE` zatvara formu nakon ispisa (`7033`, `Form1.vb` 4276).

**Nedostatak:** Kuka nema ove ključeve. Četiri pravila Kartice partnera ostaju neizvedena.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| KarticaPartnera `KAR-21` | !! u captionu je novi red, na ONSUBMIT gasi lijepljenje | `netgen/Form2.vb:2943; netgen/Form1.vb:5449` | kuka nema ključ |
| KarticaPartnera `KAR-22` | +FOCUS(ime) vraca fokus i ne gasi grešku ako nema +OK | `netgen/Class1.vb:187-204` | kuka nema ključ |
| KarticaPartnera `KAR-23` | Prazan segment ## ne preskače sljedeći OK | `netgen/Form2.vb:6821-6822` | kuka nema ključ |
| KarticaPartnera `KAR-24` | repx naredba s #CLOSE zatvara formu nakon ispisa | `netgen/Form2.vb:7033; netgen/Form1.vb:4276` | kuka nema ključ |

## 21. Temp tablica i FOREACH

**1 granula, 4 stavki:** KUPNG.

**NetGen:** `MULTISELECT` prvo briše `TotalGenMultiSelectTemp` po `suser_sname` i po `operateriid`, pa ubaci id (`Form1.vb` 4934–5018). `FOREACHSELECTED` vrti označene retke (`Form2.vb` 7040–7042, `8337–8354`). `FOREACHROW` vrti sve retke dijaloga; na podlozi je petlja komentar (`7046–7048`, `Form1.vb` 6843–6846). `FOREACHBLUEROW` vrti `SelectedItems` od 0 (`7043–7045`, `8363–8394`).

**Nedostatak:** SQL KUPNG padne na `Incorrect syntax near ','` prije petlje. Petlja na podlozi i dalje ne smije raditi.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| KUPNG `KUP-10` | Temp tablica: delete po suser_sname i po operateriid, pa insert id, host, suser, operater | `netgen/Form1.vb:4934-5018` | SELECT na spinu nije prošao |
| KUPNG `KUP-11` | FOREACHSELECTED na dijalogu vrti cekirane retke grida i sljedeci segment | `netgen/Form2.vb:7040-7042,8337-8354` | SELECT na spinu nije prošao |
| KUPNG `KUP-12` | FOREACHROW vrti sve retke grida. Na podlozi Form1 petlja je komentar | `netgen/Form2.vb:7046-7048,8310-8327; netgen/Form1.vb:6843-6846` | SELECT na spinu nije prošao |
| KUPNG `KUP-13` | FOREACHBLUEROW vrti SelectedItems redom od 0 | `netgen/Form2.vb:7043-7045,8363-8394` | SELECT na spinu nije prošao |

## 22. Dijaloško stablo, web i slika

**1 granula, 3 stavki:** Rijetke.

**NetGen:** Tag dijalog stabla je stupac 3, bez `:forma`. `yearmonth` na dijalogu ne crta godine (`Form2.vb` 2576–2577, `2842`, `2864`). `webbrowser` zove `Navigate`. Atribut `URL` je `ToUpper`. `refresh` nije URL (`1419–1433`). `picturebox` je `StretchImage`. `http` zove `Load`. `LOCATION` je `ToUpper` (`2710–2726`).

**Nedostatak:** Web i slika nemaju ključ u kuki. Stablo dijaloga padne na pretvorbi varchar u int.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Rijetke `RIJ-40` | Dijalog stablo: tag je stupac 3, bez :forma. yearmonth na dijalogu ne crta | `netgen/Form2.vb:2576-2577,2842,2864` | SELECT na spinu nije prošao |
| Rijetke `RIJ-41` | webbrowser Navigate. URL atribut je ToUpper. refresh nije URL | `netgen/Form2.vb:1419-1433` | kuka nema ključ |
| Rijetke `RIJ-42` | picturebox StretchImage. http zove Load. LOCATION je ToUpper | `netgen/Form2.vb:2710-2726` | kuka nema ključ |

## 23. Tražilica

**1 granula, 2 stavki:** Kulture.

**NetGen:** Tražilica šalje `Tag` (id), ne tekst. Prazan tag je null (`Form2.vb` 3626–3632). Search SQL: id je `robaid`, `LIKE` na `userid` i `naziv`, stupac s `!` samo sortira (`Form3.vb` 102–126).

**Nedostatak:** `spKultureNGQuery` nije na spinu, pa se lanac do tražilice nije izvršio. Port šalje id, ne natpis.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| Kulture `KUL-21` | Tražilica šalje Tag (id), ne tekst. Prazan tag je null | `netgen/Form2.vb:3626-3632` | SELECT na spinu nije prošao |
| Kulture `KUL-22` | Search SQL roba: id=robaid, LIKE na userid i naziv, ! stupac samo sort | `netgen/Form3.vb:102-126` | SELECT na spinu nije prošao |

## 24. Redoslijed punjenja podloge

**1 granula, 1 stavki:** OznakaKL.

**NetGen:** Nema `SETDEFAULTS`, pa ide drvo, pa master SQL, detail se ne zove (`Form1.vb` 3862–3883, `2104–2113`).

**Nedostatak:** Master SQL padne na `oznaka2`, pa se redoslijed nije potvrdio na napunjenom gridu.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| OznakaKL `OKL-30` | Redoslijed: nema SETDEFAULTS, drvo, pa master SQL, detail se ne zove | `netgen/Form1.vb:3862-3883,2104-2113` | SELECT na spinu nije prošao |

## 25. fidus nema tablicu ssMeni

**1 granula, 1 stavki:** ssMeni-serveri.

**NetGen:** Upit na `jupiter2019.fidus` gleda `sys.tables`. Tablice `ssMeni` nema.

**Nedostatak:** Nema izbornika za čitati. Granule s tog servera ostaju nepopisane. Na toj bazi se ništa ne piše.

| Stavka | Očekivano | Izvor | Zašto je otvoreno |
|---|---|---|---|
| ssMeni-serveri `SSMENI-jupiter2019fidus` | fidus nema ssMeni | `jupiter2019.fidus` | objekt nije na serveru |

## Dijalozi: MPRealizNG, ZakljucivanjeTemNG, MPNivelacija

Port je `Dijalog` na `cursor/netgen-filter-forma-3266` (`14d3208`, `Devex_26/Carpo.NetGen/Ui/Dijalog.cs`). Ove tri forme nemaju `NEXTFORM`. Port taj atribut nigdje ne čita, pa ga ni ne bi slijedio.

| Što | NetGen | Port | Posljedica |
|---|---|---|---|
| Klik gumba | `ONCLICK` ide u `Tag` i `Glbl_Click` (`Form2.vb` 1345–1349). `:Zakljucavanje` otvara tu formu. `CLOSE` zatvara. `OK` šalje `ONSUBMIT`. | `SimpleButton` dobije samo `DialogResult` prema natpisu (`Dijalog.cs` 199, 234–238, 103–120). `Podloga.OtvoriFormu` rezultat ne gleda. | `ZakljucivanjeTemNG` Zaključi zatvori dijalog i ne otvori `Zakljucavanje`. `MPNivelacija` Pokaži ne pošalje `ONSUBMIT` iako je `FILLEXPLORER=TRUE`. |
| Combo pri otvaranju | SQL comba ide u `ExecSql` čim se kontrola složi (`Form2.vb` 2396–2427). `ALLOWEMPTY=true` ubaci prazan red. | `LookUpEdit` se nacrta bez SQL-a (`Dijalog.cs` 174–175). | `MPRealizNG` `trgovinaid`, `vrstadokid`, `partneriid`, `ZakljucivanjeTemNG` `vrstadokid` i `MPNivelacija` tri comba na `FILTERFORM` ostanu prazni. |
| Grid pri otvaranju | Bez `SKIPDEFAULTLOAD` grid zove `ExecSql` (`Form2.vb` 1635–1646). `@@` kontrole zamijeni `SrediSQLPosljeNextForme` (`Class1.vb` 755–760). | `GridControl` ostane bez izvora (`Dijalog.cs` 208–215). | `MPRealizNG` `PROMET.stavke` i `KARTICA.gridkartica` se ne pune. |

Ovo nije novi R. Zakoni su već R26, R53 i R49. Port ih na dijalogu još ne izvršava.


