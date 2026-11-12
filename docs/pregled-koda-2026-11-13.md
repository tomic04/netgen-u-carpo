# Pregled koda 2026-11-13

Uspoređen je `Devex_26/Carpo.NetGen` s `netgen/Form1.vb`, `Form2.vb`, `Class1.vb` i `Loader.vb`. Devet modula. Razlika DA znači da Carpo ne radi isto što NetGen na tom mjestu. Popravljeno je ono što je stvarni propust. Namjerno ostaje u `docs/razlike-netgen-carpo.md`.

## Sažetak

Pregledano je 9 modula. Popravljeno: `FILLEXPLORER` nakon OK, `ONLOAD2` po natpisu gumba, `Translate` na ispisu, `Rafalna_Priprema` i na podnožju, lanac gumba na običnoj formi, `FILLEXPLORER=ONLYDET`.

## 1. Loader i XML

| | |
|---|---|
| NetGen | `Loader.vb` `Run8`: ista polja, `hWnd` 0 dok nema prozora. `Form2.ProcitajXML` čita atribute kako jesu. |
| Carpo | `Ulaz/Loader.cs` 60–96 i `Xml/NetGenXmlReader.cs` 47–127. |
| Razlika | NE za ulaz. `INCLUDE` uvozi djecu `/PROJECT` (`Xml/IncludeXml.cs`, `Form2.vb` 844–934). Nema datoteke: forma ostaje. |

## 2. ExecSql i @@

| | |
|---|---|
| NetGen | `Class1.vb` 105–118: prazno, počinje s `@`, ili `IndexOf("@@") > 0` se ne šalje. `Form1.vb` 2482 zamjenjuje samo poznate tokene, ne `@@godina` i ne `@@tkosmomi`. |
| Carpo | `Grid/SqlTekst.cs` 25–72. `MasterGrid.IzvrsiCitanje` šalje samo odluku `salji`. |
| Razlika | NE. `@@` na početku hvata grana „počinje s @”. Upisi (`insert`/`update`/…) se na spinu ne šalju. To je namjerno, nije NetGen. |

## 3. ONLOAD, ONSUBMIT, FILLEXPLORER

| | |
|---|---|
| NetGen | `Form1.vb` 3879–3882: `:ime` otvara formu, inače `PokaziUpit`. `5445–5453`: `FILLEXPLORER=true` i OK zamijene master sa `ONSUBMIT`. |
| Carpo | `Grid/MasterGrid.cs` 99–114. `Ui/Podloga.cs` nakon OK zove `DijalogTok.SqlNakonOk` (`Komande/DijalogTok.cs`). |
| Razlika | Bila je DA: OK je osvježavao stari master. Popravljeno. `FILLEXPLORER=ONLYDET` i dalje nije grananje iz `5458`. |

## 4. Gumbi enable/disable

| | |
|---|---|
| NetGen | `Form1.vb` 3914–3921 traži stupac čije je ime `Button.Text`. `4013–4035`: `enable`/`disable`/`visible`/`invisible`/`refresh`, inače novi natpis. Prazno se preskače. |
| Carpo | `Komande/OnLoad2.cs` i `Ui/Podloga.cs` `PrimijeniOnLoad2`. |
| Razlika | Bila je DA (nije se gledao redak). Popravljeno. Traženje riječi u tekstu SQL-a i dalje nije to. |

## 5. Combo i TREE

| | |
|---|---|
| NetGen | Combo: vrijednost je stupac 0 (`Form2.vb` 2396–2431). Stablo: stupci 0 id, 1 parent, 2 natpis, 3 tag (`Form1.vb` 4296–4480). Prazan `TREE` bez `COMBO` sakrije lijevo (`4310`). |
| Carpo | `Kontrole/ComboDijalog.cs` 27–36. `Grid/Stablo.cs`. `Shell/ShellLayout.cs` 47–74. |
| Razlika | NE za stupac 0 i prazno stablo. Prazan red u int stupcu više nije tekst `null` (DBNull), inače dijalog padne prije prikaza. |

## 6. Dijalozi

| | |
|---|---|
| NetGen | `Form2`: `TOP/LEFT/WIDTH/HEIGHT / 14`. `ONCLICK` ide u `Rafalno`. `NEXTFORM` se otvara dok nije prazan (`Form1.vb` 5381). |
| Carpo | `Ui/Dijalog.cs` 51–164. `Kontrole/Raspored.cs`. `Komande/DijalogTok.cs`. |
| Razlika | NE za mjere i `NEXTFORM`. Ulaz `:Forma` sada otvara taj dijalog, ne Manager (`Ui/Prozor.cs`). |

## 7. Grid

| | |
|---|---|
| NetGen | `Class1.vb` 1087–1244: ID skriven, int 34 i `##0`, string bez WIDTH je 100. `Form1.vb` 7522: WIDTH/14 prepisuje. Fokus ako ima redova (`2059–2072`). |
| Carpo | `Grid/MasterGrid.cs` `IzPodatka` i `Fokus`. |
| Razlika | NE. `@@id` bez stupca `id` je prvi podatak. |

## 8. Podloga i Manager

| | |
|---|---|
| NetGen | `Form1` je ljuska. Dijalog je `Form2`. |
| Carpo | `Shell/OdabirForme.cs`: Manager samo uz stablo, detail koji nije `:forma`, i gumbe. Inače `Ui/ObicnaForma.cs`. |
| Razlika | NE prema dogovoru (R104). Prije ovoga je svaka granula bila Manager. To je bio propust, popravljen. |

## 9. Zatvaranje

| | |
|---|---|
| NetGen | Forma se zatvori bez Carpo loggera. |
| Carpo | `Manager.cs` `Manager_FormClosing`: izlaz odmah ako nema `JSettings` ili `Logger`. |
| Razlika | NE. Raniji `NullReference` na liniji 1337 bio je `Logger.Error` dok logger nije postojao. |

## 10. Translate, Rafalna, obična forma, ONLYDET

| | |
|---|---|
| NetGen | `Module1.vb` 266–284: prazan jezik i `hrv` ostaju, inače vanjski prevoditelj, greška ostavlja izvor. `Form1.vb` 6001 i 6034: `Translate(Rafalna_Priprema(...))` na zaglavlju i podnožju. `Rafalna_Priprema` 7737–7773 bira granu `ISEMPTY`. `Button1_Click` 5664–5667 zove `NovaGenerickaForma`. `REFRESHMASTERGRID` je 4570. `PRINT` izlazi prije forme (4945). Forma bez kontrola ima `OKClick` i `PokaziUpit` (5369, 5519). `ONLYDET` osvježi detail, ne master (5470–5473). |
| Carpo | `Print/Prevod.cs`, `Print/IspisZaglavlje.cs` `Desno` i `Podnozje`, `Komande/GumbTok.cs`, `Ui/ObicnaForma.cs` klik, `Ui/Podloga.cs` nakon OK. |
| Razlika | Bila je DA. Popravljeno. `onlytree` osvježi samo stablo (`Form1.vb` 5475). Fiskalizacija, Word i `ASSEMBLYLOAD` i dalje se samo zabilježe. |

## 11. ASSEMBLYLOAD

| | |
|---|---|
| NetGen | `Form1.vb` 4584–4623: sklop je lijevo od zadnje točke, `CreateInstance` cijelog imena, `totalgen(id, operater, konekcija, zadnji SQL, ParametriSaGrida)`. Konekcija bez `driver` dobije `Driver={SQL Server};`. Pad `Load`/`CreateInstance` prekida lanac. Povrat se ne čita. `Form1.vb` 6681–6727 peti argument je `""` i nema Driver prefiksa. `Form2.vb` 7424–7456 zadnja dva su `strParametriSvi`. |
| Carpo | `Poziv/AssemblyPoziv.cs`. Gumb na podlozi zove shell potpis. Dijalog zove dijalog potpis. |
| Razlika | Bila je DA (samo zapis u dnevnik). Sada se sklop stvarno učita i zove `totalgen`. Tuđi DLL nije u repou; ako ga na stroju nema, lanac staje, kao kod NetGena. |

## 12. Fiskalizacija je vanjski DLL

| | |
|---|---|
| NetGen | `Form1.vb` 360–368 učita `Raverus.FiskalizacijaDEV.COM` i klasu `CentralniInformacijskiSustav`. `GenerirajUuid` (454), `ZastitniKodIzracunCertifikatDatoteka` s 8 argumenata (457), `PosaljiSoapPorukuCertifikatDatoteka` (520) vraća SOAP, `DohvatiJir` (548). URL s `fu.gov.si` ide u `SLOFISKAL.Program.SLORacun` (341–354). |
| Carpo | `Poziv/FiskalPoziv.cs` učita isti sklop i istu klasu. `PosaljiSoapPorukuCertifikatDatoteka` se ne zove. |
| Razlika | DA za slanje. Izvorni kod Raverusa nije potreban. Treba DLL na stroju. |

## 13. Ispis grida

| | |
|---|---|
| NetGen | `PRINTREPXMASTER` i `PRINTMASTER` zovu ispis master grida (`Form1.vb` 4945–4955). Detail je drugi grid (`6067`). Dijalog šalje `"lodani"` u NetGenExtensions (`Form2.vb` 9056). Zaglavlje je lijevo, centar i desno (`5995–6001`). |
| Carpo | `PrintBridge` zove `Spin.Printing.v26` / `Spin.Printing.Start.PrintLink`, isto kao `ComponentGrid.PrintControl`. `:repx/` zove `Print`, isto kao `PrintDialog`. |
| Razlika | `PrintLink` nema riječi master/detail/lodani ni `PageHeaderLeft/Center/Right`. Grid se predaje kao kontrola. Centar ide u `caption`, lijevo u `header`, desno u `subHeader`. `PAGEORIENTATION=LANDSCAPE` ide u `isLandscape`. Nema tablice predložaka `printDialogComboTemplates`. |

## 14. Placeholdere i što ugasiti prije produkcije

Živi ulaz (TestShell, `Prozor`, `Podloga`) ne zove ove tri metode. One i dalje bace `NotSupportedException` ako ih netko pozove:

| Klasa | Datoteka | Što radi |
|---|---|---|
| `LayoutEngine.Crtaj` | `Placeholders.cs` 11–14 | M0 ne crta. Crta `Podloga` / `ObicnaForma`. |
| `CommandRunner.Pokreni` | `Placeholders.cs` 22–25 | M0 ne izvršava komandu. Radi `GumbTok` i `Lanac`. |
| `SqlBridge.Exec` | `Placeholders.cs` 37–40 | M0 ne šalje SQL. Radi `MasterGrid` i `Citaj`. |

Ostalo što nije NetGen tijelo:

| Što | Gdje | NetGen | Prije produkcije |
|---|---|---|---|
| Upis `insert`/`update`/`delete`/`drop`/`alter`/`truncate`/`merge`/`create` se ne šalje | `SqlTekst.JeZabranaPisanja` 104–120 | `Class1.ExecSql` šalje | Na spinu i jupiteru ovo mora ostati. Na produkcijskoj bazi se gasi, inače spremanje ne stiže. Test `zabrana-insert`. |
| `jupiter2022` u `SpinVeza.Spoj` baca | `SqlTekst.cs` 135–136 | NetGen se spaja | Ostaje ugašeno. Nema čitanja ni upisa na taj server. |
| SOAP fiskalizacije | `FiskalPoziv.Ucitaj` | `Form1.vb` 520 `PosaljiSoapPorukuCertifikatDatoteka` | Ne uključivati dok `Raverus.FiskalizacijaDEV.COM.dll` nije na stroju. Ovaj kod metodu ne zove. |
| Word | nema poziva | `Form1.vb` 5093 `WORDDOT1` i `CreateObject("Word.Application")` | Nije implementirano. |
| `:repx/` izvještaj | `PrintBridge.Predlozak` zove `Spin.Printing.Start.Print` | `Class1.vb` 348–353, `Carpo.UI` `PrintDialog` | Poziv postoji. v23 i v9.1 se ne učitavaju, samo v26. |
| QA kuka | `GranulaSesija` | nema je | Produkcijski `Loader` je ne zove. Ostaje isključena. |

## Naredbe

Stanje je `radi` kad se naredba izvrši, `nije` kad se samo preskoči ili zabilježi. R59 (izgled Carpo) nije u tablici.

| Naredba | NetGen | Carpo | Stanje |
|---|---|---|---|
| REFRESHMASTERGRID | `Form1.vb` 4570, 6674 | `GumbTok`, `Lanac` | radi |
| refreshmaster bez forme | `Form1.vb` 5369, 5519 | `GumbTok` | radi |
| ASSEMBLYLOAD | `Form1.vb` 4584–4623, `Form2.vb` 7424 | `AssemblyPoziv` | radi |
| WORDDOT1 | `Form1.vb` 5093–5159 | `WordDot` | radi |
| PRINTMASTER / PRINTREPXMASTER / PRINTGRID / PRINTREPXGRID | `Form1.vb` 4945, 5984, `Form2.vb` 9056 | `PrintBridge.PrintLink` | radi, R110 |
| :repx/ | `Class1.vb` 594–632, `Form6.vb` 134–161 | `Print` pa `DoPrint2` | radi |
| OK / OKSTAY / CLOSE / CANCEL | `Form1.vb` 6671, 6809 | `Lanac`, `DijalogTok` | radi |
| FILLEXPLORER true / ONLYDET / onlytree | `Form1.vb` 5445, 5470, 5475 | `DijalogTok` | radi |
| ONLOAD2 | `Form1.vb` 3914, 4013 | `OnLoad2` | radi |
| INCLUDE | `Form2.vb` 844–934 | `IncludeXml` | radi |
| select na podlozi | `Form1.vb` 4875–4898 | `ObicnaForma.PrimijeniSelect` | radi |
| HIDECOLUMNS / SHOWCOLUMNS | `Form1.vb` 6730–6766 | `StupciNaredba` | radi |
| FISKALIZACIJA* | `Form1.vb` 4643–4781 | `FiskalNaredba` bira tablicu i broj poziva, SOAP je `FiskalPoziv.Posalji` | radi, bez porezne |
| CLICK( | `Form2.vb` 7151; podloga `Form1.vb` 6856 komentirano | `KlikNaredba` | radi na dijalogu, podloga ne |
| MSGBOX( | `Form2.vb` 7165–7169 | `PorukaNaredba` | radi |
| OPENFORM( | `Form1.vb` 6582–6585 | `OtvoriNaredba` | radi |
| POSLOVNIPROSTOR( | `Form1.vb` 4783–4801 | `ProstorNaredba` | radi, broj poziva |
| BLAGAJNAENABLE( | `Form1.vb` 4635, `Class1.vb` 1975 | `BlagajnaNaredba` | radi, upis i dalje stoji na spinu |
| UNCHECKMASTERGRID | `Form1.vb` 4626–4633 | `OdznaciNaredba` | radi |
| CLOSEPARENT | `Form1.vb` 6838–6840 | `RoditeljNaredba` | radi |
| ISEMPTY u lancu | `Form1.vb` 6768–6776 | `PraznoNaredba` | radi |
| GRID_COLUMN_VALUE( | `Form1.vb` 6785–6807 komentirano | `StupacVrijednost` ne izvršava | radi, kao NetGen |
| FOREACHSELECTED / FOREACHROW / FOREACHBLUEROW | `Form1.vb` 6843–6850 tijelo komentirano; `Form2` izvršava | `Lanac` samo zabilježi | nije |
| BEEP() | `Form1.vb` 6579 | nema | nije |
| STARTSEARCH( | `Form1.vb` 6592 | komentirano i u NetGenu | nije |
| CLICK na podlozi | `Form1.vb` 6852 | komentirano i u NetGenu | nije |

## Što ostaje otvoreno

- Word (`Form1.vb` 5093) otvara `.dot` preko `Word.Application` kad je Word instaliran. `WORDDOT1` plus putanja.
- Fiskalizacija ne šalje SOAP.
- `PrintLink` nema lijevo/centar/desno ni riječ `lodani`. To su ulazi koje Carpo složi u `caption`/`header`/`subHeader` i u odabir grida.
- `JeZabranaPisanja` na spinu ostaje uključena.
- `strParametriSvi` na dijalogu za `ASSEMBLYLOAD` skuplja check, lookup i tekst. Search, grid i radio iz `Form2.vb` 3432 još nisu u tom nizu.
- 12 redova `ssMeni` i XML-ovi kojih nema u repou. Shema se ne dira.
