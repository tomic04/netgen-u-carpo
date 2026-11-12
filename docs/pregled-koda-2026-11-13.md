# Pregled koda 2026-11-13

Uspoređen je `Devex_26/Carpo.NetGen` s `netgen/Form1.vb`, `Form2.vb`, `Class1.vb` i `Loader.vb`. Devet modula. Razlika DA znači da Carpo ne radi isto što NetGen na tom mjestu. Popravljeno je ono što je stvarni propust. Namjerno ostaje u `docs/razlike-netgen-carpo.md`.

## Sažetak

Pregledano je 9 modula. Nađene su dvije greške i obje su popravljene: `FILLEXPLORER` nakon OK i `ONLOAD2` po natpisu gumba. Ostaje otvoreno: `Translate` na ispisu, `Rafalna_Priprema` (`#ISEMPTY`) izvan zaglavlja koje je već riješeno za `@@tkosmomi`, i puni `Form2` klik na običnoj formi (gumb se samo zabilježi).

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

## Što ostaje otvoreno

- `Translate` prije ispisa (`Form1.vb` 6001) nema rječnik.
- `Rafalna_Priprema` (`#ISEMPTY`) nije u svakom tekstu, samo zamjena `@@` u zaglavlju.
- Gumb na `ObicnaForma` se zabilježi, lanac se ne izvrši.
- `FILLEXPLORER=ONLYDET` nije posebna grana.
- 12 redova `ssMeni` i XML-ovi kojih nema u repou. Shema se ne dira.
