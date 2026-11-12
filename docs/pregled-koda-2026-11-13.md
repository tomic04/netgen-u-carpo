# Pregled koda 2026-11-13

Uspoređen je `Devex_26/Carpo.NetGen` s `netgen/Form1.vb`, `Form2.vb`, `Class1.vb` i `Loader.vb`. Devet modula. Razlika DA znači da Carpo ne radi isto što NetGen na tom mjestu. Popravljeno je ono što je stvarni propust. Namjerno ostaje u `docs/razlike-netgen-carpo.md`.

## Sažetak

Pregledano je 9 modula. Popravljeno: `FILLEXPLORER` nakon OK, `ONLOAD2` po natpisu gumba, `Translate` na ispisu, `Rafalna_Priprema` i na podnožju, lanac gumba na običnoj formi, `FILLEXPLORER=ONLYDET`.

## 1. Loader i XML

| | |
|---|---|
| NetGen | `Loader.vb` `Run8`: ista polja, `hWnd` 0 dok nema prozora. `Form2.ProcitajXML` čita atribute kako jesu. |
| Carpo | `Ulaz/Loader.cs` 60–96 i `Xml/NetGenXmlReader.cs` 47–127. |
| Razlika | NE za ulaz i čitanje atributa. `INCLUDE` se spremi, a tuđi XML se ne uvozi (R78, namjerno dok datoteka ne postoji). |

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

## Što ostaje otvoreno

- Word još nema tijelo.
- Fiskalizacija ne šalje SOAP dok DLL nije na stroju, i ovaj kod ga ne zove.
- `strParametriSvi` na dijalogu za `ASSEMBLYLOAD` skuplja check, lookup i tekst. Search, grid i radio iz `Form2.vb` 3432 još nisu u tom nizu.
- 12 redova `ssMeni` i XML-ovi kojih nema u repou. Shema se ne dira.
