# 10 — Nedoumice za tim koji poznaje NetGen granule

Ovo nije popis bugova u planu. Ovo su mjesta gdje `Carpo.NetGen` namjerno radi isto što i današnji VB, iako ponašanje izgleda krivo ili nerazumljivo, te mjesta gdje smo odstupili jer je tako odlučeno u iteraciji 3. Svaka stavka ima trenutnu odluku. Ako je odluka kriva, treba je promijeniti prije faze koja to kodira, ne tijekom M0.

M0 (prazan projekt, parser, build TestShella) ove stavke ne čeka.

## FOREACH na podlozi — dokaz, ne sumnja

Petlja se na podlozi (`Form1`) ne izvršava. U `Form1.vb` nema procedura `ForEachRow`, `ForEachSelected` ni `ForEachBlueRow` (pretraga po imenu je prazna). Jedino pojavljivanje je u `Form1.Rafalno`:

```6841:6850:netgen/Form1.vb
ElseIf Ar5(i).Trim.ToUpper.Contains("PREPAREMULTISELECTEDPRINT(") Then
    'MultiSelectedPrintPriprema(Ar5(i))
ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHSELECTED(") Then
    'ForEachSelected(Ar5(i), Ar5(i + 1), sender, e)
    i = i + 1
ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHBLUEROW(") Then
    'ForEachBlueRow(Ar5(i), Ar5(i + 1), sender, e)
    i = i + 1
ElseIf Ar5(i).Trim.ToUpper.Contains("FOREACHROW(") Then
    'ForEachRow(Ar5(i), Ar5(i + 1), sender, e)
    i = i + 1
```

Poziv je komentar. `i = i + 1` nije. Ako komanda dođe u `Rafalno`, petlja se ne vrti, a sljedeći segment iza `#` se preskoči.

Gumbi podloge ne zovu `Rafalno`. `Button1_Click` zove `NovaGenerickaForma(sender.tag)` (`Form1.vb` 5664–5667). Ta procedura (`4543`) nema granu `FOREACH`. Nepoznata riječ padne u `NovaGenerickaForma2` (`4906`) i traži formu tog imena.

Na dijalogu (`Form2.vb` 7040–7047) iste tri komande zovu žive `ForEachSelected` / `ForEachBlueRow` / `ForEachRow` (8310, 8337, 8363).

**Odluka:** petlju na podlozi ne pisati. Na dijalogu je pisati. Na putu koji odgovara `Rafalno` prepoznati ime i preskočiti sljedeći segment, bez petlje. Na putu gumba podloge ne uvoditi posebnu granu (ostaje „otvori formu tog imena“).

Primjer u korpusu: `FOREACHROW` 162, `FOREACHSELECTED` 143, `FOREACHBLUEROW` 109 (brojač, uglavnom na dijalogu). Ako netko zna granulu koja `FOREACH` stavlja na gumb **podloge** i očekuje petlju, ta granula ni danas ne radi petlju.

## Ostale stavke

| # | Što | Primjer | Trenutna odluka | Pitanje timu |
|---|---|---|---|---|
| 1 | `yearmonth` puni godine 2005–2009, mjesec nije zero-padded (`Form1.vb` 4527–4534) | granule s `TREE` koji sadrži `yearmonth:` | kopija 1:1 | Je li itko još otvara to stablo, ili je mrtvo i smije ostati krivo? |
| 2 | Kod traži `CLEARCOLUMS` (nema N), `AGREGATEFUNCTION` (jedno G), `PERMISIO` (jedno S) | `JeziciNG.xml` ima `CLEARCOLUMS` i prazan `AGGREGATEFUNCTION` (dva G, zato se ne veže) | čitati isto pogrešno slovkanje | Je li dvo-G namjerno ostavljeno da suma ne radi? |
| 3 | `HIGHT`, `WUDTH`, `ALOWEMPTY`, `MAXLENGHT`, `PERMISSIO`, `TEXT`, `BOLD`, `BORDERSTYLE`, `REPORT` — korpus ih ima, `GetNamedItem` ne. Popis: dokument 03, odjeljak 12 | `ALOWEMPTY` 127 puta | ignorirati, kao NetGen | Ima li granule gdje autor vjeruje da `ALOWEMPTY` ubaci prazan red? |
| 4 | `GRIDALLOWEDIT` i `GRIDALLOWEDITCOLUMNS` nisu u `GetNamedItem` (samo uzorak u `Resource1.Designer.vb`) | `definitions/netgen/BIOVEGA/OznakaKL.xml` | ne uključuju edit | Je li edit na toj granuli ikad radio, ili samo `EDITCOLUMNS`? |
| 5 | Prazan combo bez `ALLOWEMPTY` pokuša `SelectedValue = 1` (`Form2.vb` 3455–3468) | bilo koji combo bez `ALLOWEMPTY` | kopirati | Je li „pošalji 1“ namjera ili slučaj? |
| 6 | Prazan broj ide kao `0`, prazan tekst kao `''`, datum bez kvačice kao `null` | number/text/date u `Jezici` nije broj; vidi `NetGen-Kontrole.md` | kopirati | |
| 7 | Apostrof u tekstu postaje `¸¸`, zarez `~~`, pa se u `ExecSql` vraćaju (`Class1.vb` 87 i ~850) | bilo koji text u `ONSUBMIT` | kopirati, SQL ostaje tekst | Postoji li procedura koja očekuje sirove `¸¸`? |
| 8 | `!!` na `ONSUBMIT` gasi lijepljenje parametara (`Form1.vb` 5449). U captionu `!!` je novi red (`Form2.vb` 2943) | `KarticaPartnera.xml` završava `ONSUBMIT` s `!!` | oba značenja, ovisno o mjestu | |
| 9 | SQL u kojem ostane `@@` se ne šalje (`Class1.vb` 113–118), samo CB log | kriv `@@` u `ONLOAD` | isto, tihi skip | Je li korisnik ikad vidio tu tišinu kao „grid je prazan“? |
| 10 | Poruka SQL greške s `#OK` / `+OK` guta grešku, `#close` zatvara, `#FOCUS(` vraća fokus (`Class1.vb` 170–205) | `Kulture.xml` `spRaiserror` u `ONCLICK` | kopirati | |
| 11 | `PRINTMASTER` i `PRINTREPXMASTER` zovu istu `IspisGRIDREPX` (`Form1.vb` 4945–4956). Stari ispis je komentar | `JeziciNG.xml` `PRINTREPXMASTER` | jedna funkcija | |
| 12 | Ispis ide na `Spin.Printing` v26 i `rptDevRepx`, ne na v23/v9.1 koje VB još traži (`Class1.vb` 297–304, `Form6.vb` 134) | 481 `repx/` komandi, npr. kartica partnera | odluka iteracije 3 | Je li klasa u v26 i dalje `Spin.Printing.Start`, a u rpt `clsReport` s `isnetgen`? |
| 13 | `CMD1CAPTION="Unesi:Unesi#refreshmaster"` — `refreshmaster` nije `REFRESHMASTERGRID` | `definitions/netgen/Agronom granule/JeziciNG.xml` red 2 | ne izmišljati sinonim; tražit će se forma imena `refreshmaster` | Je li to tipfeler u granuli koji produkcija „nekako“ osvježi? |
| 14 | `##` u lancu je prazan segment (`OK` i dalje dođe) | kartica, `end##OK` | preskočiti prazan segment | |
| 15 | `DETAILGRID` koji počinje s `:` pokaže `sorkac!` i ne otvori formu | ako takav XML postoji samo u bazi | kopirati poruku | Ima li takvih granula na `sql-ai`? |
| 16 | `SLOPP(` je komentirano (`Form2.vb` 7494–7498) | nema pogodaka u korpusu | ne radi, kao danas | |
| 17 | `CREATEOBJECT` samo pokaže `MsgBox` (`Form2.vb` 7419–7423) | rijetko | isto | |
| 18 | `IDINAWEBSERVICE…` je komentar (`Form2.vb` 7499–7508) | | ne dirati | |
| 19 | `TotalGenMultiSelectTemp`: delete po `suser_sname()` i po `operateriid`, insert id-eva (`Form1.vb` 4932–5032). Dva prozora istog operatera brišu jedan drugome redove | forme `MULTISELECT*` / `MULTIBLUE*` | ostaje, odluka iteracije 2 | Je li tablica na `sql-ai` još ta shema (`id, hostname, susersname, operateriid`)? |
| 20 | `@@id` bez kolone `id`: Janus uzme ćeliju indeks 2, DX prvu (`GetID` ~5566, `GetDXID` ~5588) | master bez kolone `id` | kopirati oba fallbacka; novi grid je uvijek DX pa vrijedi DX fallback | Koji fallback korisnici stvarno vide na Janus masteru? |
| 21 | Forma čije ime sadrži `filter` forsira DB clipboard osim ako je `DBCLIPBOARD=false` (`Form2.vb` ~1102) | `FILTERFORM` | kopirati | |
| 22 | Combo: stupac 0 = vrijednost, stupac 1 = prikaz, imena se ne gledaju | svaki `TYPE="combo"` | kopirati | |
| 23 | `STARTUPSTORA` vrijedi samo ako nema `ONLOAD` (3367 pa 3368; drugi red baci i `On Error Resume Next` ostavi prvi) | `OznakaKL.xml` ima `STARTUPSTORA`, nema `ONLOAD` | isto | |
| 24 | `CMDxCAPTION` bez dvotočke: `IndexOf(":")` baci, gumb ostane prazan, granula ne padne (petlja je pod `On Error Resume Next`) | ako negdje fali `:` | isto: preskočiti gumb, upisati u log | |
| 25 | `cmeditvb6.curmedit` upadne u broj jer sadrži `cur` (8 kontrola, 1 fajl) | traži se fajl po tom `TYPE` u JSON-u | ne „popravljati“ u text | |
| 26 | `b_utton`, `t1ext`, `g_rid`, `datetiem` ne sadrže podstring i ne crtaju se | vidi dokument 03 | ne crtati | |
| 27 | `CHECKBOXVISIBLE` default je uključen dok XML ne kaže `FALSE` (518 korijena ga gasi) | većina `PROJECT` | default uključen | |
| 28 | `DXGRID`, `DEFAULTCONNECTION`, `CUSTOMCONNECTIONSTRING`, `SQL3`, `SQL4` kod čita, u 609 fajlova ih nema | samo baza `netgenxml` može imati | čitati ako se pojave | Ima li ih na `sql-ai`? |
| 29 | `CHILDFORM` (156 puta, 65 datoteka) lookup ne zove | | ignorirati dok netko ne pokaže granu u kodu | Zna li tko da je to ikad nešto radilo? |
| 30 | Dekripcija tijela iz `netgenxml` ključem koji je u izvoru (`Form1.vb` 3223) | DMS prazan, baza puna | isti ključ, isti redoslijed DMS → baza → disk | |
| 31 | `frm.Run` (ne `Run8`) ne radi `New Form2` (`frm.vb` 39–52). Živi je `Run8` | | portirati `Run8` | Zove li Jupiter još stari `Run`? |
| 32 | `Loader.Run8` vraća `Nothing` (`Loader.vb` 175, dodjela je komentar). Jupiter čita `hWnd` i `WCaption` | | isto | |
| 33 | `spNetGenErrorLog` se zove, a na klijentima je prazna procedura (`netgen/CLAUDE.md`) | svaki SQL catch | zvati i dalje, ne oslanjati se da nešto zapiše | |
| 34 | Raspored nije piksel (`/14` se ne crta). Kontrola, natpis i tab order jesu | svaka forma s `TOP`/`LEFT` | odluka iteracije 2, poznata razlika | |
| 35 | `jupiter2022` je u `netgen/CLAUDE.md` kao stari test. Produkcija je, ne dirati | | test je samo `sql-ai` / `spin` | |

Ako stavka nema odgovor, odluka u stupcu ostaje. M0 je ne čeka.
