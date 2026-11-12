# NetGen komande (Rafalno)

Izvor: `Form2.Rafalno` (`Form2.vb` 6789) i `Form1.Rafalno` (`Form1.vb` 6546), plus `NovaGenerickaForma` (`Form1.vb` 4543). Commit `b0d5d41`.

Komanda je jedan segment u lancu. Lanac se dijeli po `#`. Na podlozi, u akciji gumba, `#` je već separator gumba u `BUTTONS`, pa se unutar jedne akcije piše `~` (`NovaGenerickaForma` pretvara `~` u `#`, `Form1.vb` 4557).

Ako segment sadrži `@@`, engine prvo zamijeni tokene, pa tek onda gleda komandu. Zato `@@stanjeforme` može postati `CLOSE` (`Form2.vb` 6812, `Form1.vb` 6565).

Prazan segment i `NULL` se preskoče.

Ako nijedna komanda ne pogodi segment, na dijalogu se segment šalje u `RefreshControls` (SQL koji puni kontrole). Na podlozi se šalje u `ExecSql`.

## Gdje komanda živi

| Oznaka | Značenje |
|---|---|
| Dijalog | `Form2.Rafalno`, živa |
| Podloga | `Form1.Rafalno` ili `NovaGenerickaForma`, živa |
| Oba | ista komanda radi na oba, ponekad drugačije |
| Mrtvo | grana postoji, tijelo je komentar — klik ne uradi ništa |

## Tok i forme

| Komanda | Gdje | Efekt | Linija |
|---|---|---|---|
| `:IMEFORME` | Oba | Otvori formu. Na podlozi `Replace(":", "")` skine sve dvotočke | D 6882, P 6665; ulaz u formu `NovaGenerickaForma2` |
| `OPENFORM(IME)` | Oba | Isto, ime u zagradi | D 6836, P 6582 |
| `IMEFORME` bez `:` | Podloga, `NovaGenerickaForma` | Ako nije SQL ni poznata komanda, otvara formu | `Form1.vb` 4906 |
| `IMEFORME(@p=@@id)` | Dijalog | Ime forme smije imati parametre u zagradi. `-freezid` u tim parametrima zabrani da rezultat prepiše `@@id` | `Form2.vb` 4407–4416 |
| `OK` | Dijalog | Kao Spremi: `ONSUBMIT`, pa zatvori. Na podlozi zove `Button1_Click` | D 6888, P 6671 |
| `OKSTAY` | Dijalog | Spremi, formu ne zatvara (`NeZelimClosatFormu`) | D 6884. Na podlozi zastavica je komentar (6668) |
| `CANCEL`, `CLOSE` | Oba | Odustani / zatvori bez submita | D 7031–7034, P 6809–6812 |
| `CLOSEPARENT` | Oba | Zatvori i roditelja (`bCloseParent`) | D 7035, P 6838 |
| `REFRESHMASTERGRID` | Podloga | Ponovo učitaj master zadnjim SQL-om | `Form1.Rafalno` 6674 i `NovaGenerickaForma` 4570 |
| `==sql` | Podloga, `NovaGenerickaForma` | `PokaziUpit` bez prefiksa `==` | 4872 |
| `select …` | Podloga, `NovaGenerickaForma` | Izvrši SQL; prvi red podešava gumbe | 4875 |
| SQL koji nije komanda | Dijalog | `RefreshControls` | 7510 |
| SQL koji nije komanda | Podloga | `ExecSql`. Ako rezultat ima `setcolorforcolorpicker`, postavi boju | 6857–6868 |

## Uvjeti

`ISEMPTY` i srodne gledaju vrijednost **nakon** zamjene `@@`.

| Komanda | Gdje | Efekt | Linija |
|---|---|---|---|
| `ISEMPTY(vrijednost)` | Oba | Ako je `null` ili prazno, preskoči **sljedeći** segment | D 6941, P 6768 |
| `ISEMPTYORZEROALERT(vrijednost;poruka)` | Dijalog | Ako je prazno, `null` ili `0`, pitaj Ok/Cancel. Cancel prekida lanac. Vrijednost može biti `@kontrola` ili `select`/`sp` | 7053 |
| `ISEMPTYORZEROCANCELBOX(vrijednost;poruka)` | Dijalog | Ako je prazno/`null`/`0`, kritični MsgBox, fokus na kontrolu iz imena parametra, prekid lanca. Bez `;` izađe odmah | 7114 |

## Petlje po gridu — samo dijalog

Na podlozi su zakomentirane (`Form1.vb` 6841–6850): engine prepozna ime i preskoči segment (i sljedeći, jer radi `i = i + 1`), ali petlju ne izvrši.

Na dijalogu drugi segment (iza `#`) je akcija za svaki red:

| Komanda | Šta obilazi | Linija |
|---|---|---|
| `FOREACHROW(imeGrida)` | Sve data redove | 7046, tijelo 8310 |
| `FOREACHBLUEROW(imeGrida)` | Plavo označene (`SelectedItems`) | 7043, tijelo 8363 |
| `FOREACHSELECTED(imeGrida)` | Checkbox redove | 7040, tijelo 8337 |
| `PREPAREMULTISELECTEDPRINT(...)` | Napuni niz id-eva iz checkbox redova za ispis | D 7038 živo. P 6841 mrtvo |

Više redova s **master** grida ide formom `MULTISELECT…` / `MULTIBLUE…` (`Form1.vb` 4932), ne ovim komandama.

## Grid, kolone, ispis

| Komanda | Gdje | Efekt | Linija |
|---|---|---|---|
| `PRINTGRID(ime)` | Dijalog | REPX ispis tog grida (`IspisGRIDREPX`) | 6950. Na podlozi mrtvo (6777) |
| `PRINTREPXGRID(ime)` | Dijalog | Isto | 6985 |
| `PIVOTGRID(ime)` | Dijalog | Pivot | 6992 |
| `CHARTGRID(ime)` | Dijalog | Chart | 6999 |
| `RECALCGRID(ime)` | Dijalog | Preračun cijelog grida | 6958 |
| `RECALCGRIDROW(ime)` | Dijalog | Preračun reda | 6964 |
| `TOGGLEGRIDGROUP(ime)` | Dijalog | Proširi / sklopi grupe | 6970 |
| `GRID_COLUMN_VALUE(grid,kolona,vrijednost)` | Dijalog | Nađi red i učini ga vidljivim | 7007. Na podlozi mrtvo (6785) |
| `HIDECOLUMNS(mastergrid,kolona,…)` | Podloga | Sakrij kolonu. Radi samo ako je prvi argument `mastergrid` | 6730 |
| `SHOWCOLUMNS(mastergrid,kolona,…)` | Podloga | Prikaži kolonu, isti uvjet | 6749 |
| `PRINTMASTER`, `PRINTREPXMASTER` | Podloga, `NovaGenerickaForma2` | Ispis master grida | 4945–4955 |
| `PRINTDET…` | Podloga | Ispis detail grida | 4958 |
| `MASTERMOVENEXT` | Dijalog | Poslije zatvaranja pomakni master na sljedeći red | 7049 |
| `MASTERMOVEPREVIOUS` | Dijalog | Pomakni na prethodni | 7051 |
| `refreshmovetoid…` | Podloga, `NovaGenerickaForma` | Osvježi master i pozicioniraj red | 4831 |

## Kontrole i izgled

| Komanda | Gdje | Efekt | Linija |
|---|---|---|---|
| `REFRESHCONTROL(ime)` | Dijalog | `RefreshControls` s lažnim `select 'refresh' as ime` — kolona se zove kao kontrola, vrijednost `refresh` ide u `DodatnoPostimajKontrolu` | 6911 |
| `REFRESHCONTROL(ime)` | Podloga | Samo skine zagrade. **Ne** osvježi kontrolu | 6677 |
| `CLICK(ime)` | Dijalog | Klik te kontrole | 7151. Na podlozi mrtvo (6852) |
| `STARTSEARCH(kontrola,tekst)` | Dijalog | Fokus, upiši tekst, pokreni tražilicu | 6873. Na podlozi mrtvo (6592) |
| `DISABLEALLCONTROLS()` | Dijalog | Sve kontrole `Enabled=false` | 6828 |
| `ENABLEALLCONTROLS()` | Dijalog | Sve `Enabled=true` | 6832 |
| `DOCK5(ime)` | Dijalog | Fill | 6916 |
| `DOCK1` / `DOCK2` / `DOCK3` / `DOCK4` | Dijalog | Top / Bottom / Left / Right | 6921–6936 |
| `EVAL(ime=izraz)` | Dijalog | Izračuna izraz i upiše u kontrolu | 7090, `EvaluateSTR` 8286 |
| `COLORPICKER()` | Dijalog | Dijalog boje → `@@selectedcolor` (HTML) | 7156 |
| `COLORPICKER(sql)` | Podloga | Isto, pa ako u zagradi ima SQL, izvrši ga | 6813 |

## Poruke, fajlovi, vanjski poziv

| Komanda | Gdje | Efekt | Linija |
|---|---|---|---|
| `MSGBOX(tekst)` | Dijalog | Informacija. Tekst se prije toga digne u velika slova | 7165 |
| `BEEP()` | Oba | Zvuk i pauza 300 ms | D 6825, P 6579 |
| `SHOWOPENFILEDIALOG()` | Dijalog | Putanja u `@@selectedfile` | 6851 |
| `SHOWFOLDERBROWSERDIALOG()` | Dijalog | Folder u `@@selectedpath` | 6847 |
| `CREATETEXTFILE1(putanja;sql)` | Dijalog | Jedan SQL, ASCII, na kraj znak 0x1A. Bez `;` prekid | 7170 |
| `CREATETEXTFILE2(putanja;sql1~sql2)` | Dijalog | Više upita odvojenih `~`, ASCII, bez 0x1A | 7222 |
| `CREATETEXTFILE3(putanja;sql~…)` | Dijalog | Isto, encoding sistema | 7281 |
| `CREATETEXTFILEUTF8(putanja;sql~…)` | Dijalog | Isto, UTF-8 | 7342 |
| `nešto.vbs` / `nešto.bat` | Dijalog | `Process.Start`. Argument iza `~` | 7402, 7411 |
| `PROCESSSTART(putanja)` | Dijalog | `Process.Start` | 6841 |
| `TIMERON(ms)` / `TIMEROFF()` | Dijalog | Timer forme | 6856, 6862 |
| `PROGRESSON()` / `PROGRESSOFF()` | Dijalog | Progress bar | 6864, 6866 |
| `CREATEOBJECT(ime)` | Dijalog | Samo MsgBox s imenom. Ne stvara objekt | 7419 |

## Assembly i fiskalizacija

Ovo nisu obrasci za novu granulu. Imena su ovdje da agent ne izmisli drugačiju sintaksu.

| Komanda | Gdje | Šta prima | Linija |
|---|---|---|---|
| `ASSEMBLYLOAD(Ime.Klasa)` | Dijalog | Učita assembly do zadnje točke, `CreateInstance` cijelog imena, zove `totalgen(ID, operater, connection, parametri, parametri)` | 7424 |
| `ASSEMBLYLOAD(Ime.Klasa)` | Podloga | Isto, ali `totalgen` dobije zadnji SQL mastera, ne parametre forme | 6681 |
| `:ASSEMBLYLOAD(Ime.Klasa)` | Podloga | Isto, četvrti argument je zadnji SQL, peti su parametri grida | 6602 |
| `FISKALIZACIJA(id)` | Dijalog | Id računa, tablica 1 | 7459 |
| `FISKALIZACIJAN(id)` | Dijalog | Napojnica | 7464 |
| `FISKALIZACIJAFAKTURE(id)` | Dijalog | Tablica 2 | 7469 |
| `FISKALIZACIJABSRACUNI(id)` | Dijalog | Tablica 3 | 7474 |
| `FISKALIZACIJAPD(id)` | Dijalog | Tablica 4 | 7479 |
| `FISKALIZACIJAPONUDE(id)` | Dijalog | Tablica 5 | 7484 |
| `POSLOVNIPROSTOR(id)` | Oba | Prijava prostora | D 7489, P `NovaGenerickaForma` 4783 |
| `BLAGAJNAENABLE(id)` | Oba | Enable blagajne (Mlinar) | D 6868, P 6587 |
| `POSALJIRACUNENASERVER(id)` | Dijalog | Slanje računa | 6891 |
| `POSALJIZAKLJUCKENASERVER(id)` | Dijalog | Zaključak | 6897 |
| `POSALJIPORUKENASERVER(id)` | Dijalog | Poruke | 6904 |
| `SLOPP(id)` | Dijalog | Tijelo je komentar. Ne radi | 7494 |

Zakomentirano i na dijalogu: `IDINAWEBSERVICEPOBROJBODOVA`, `IDINAWEBSERVICEIAZURIRAJBODOVE` (`Form2.vb` 7499–7508).

## Posebne kolone SQL-a koji nije komanda

Kad dijalog izvrši običan SQL (`RefreshControls`, `Form2.vb` 4628–4749), ime kolone prvog reda ima značenje:

| Kolona | Efekt |
|---|---|
| `focus` | Fokus na tu kontrolu |
| `form` | Otvori tu formu |
| `formcaption` | Naslov forme |
| `formwidth`, `formheight` | Veličina |
| `fillexplorer` | Postavi `FILLEXPLORER` |
| `beep` | Toliko beepova, najviše 30 |
| `click` | Klik kontrole tog imena |
| `findnode` | Traži čvor |
| `id` | Novi `@@id`, osim ako je forma otvorena s `-freezid` — tada ide u CB kao `ImeForme_ID` |
| `temp` | `@@tempstring` |
| `setcolorforcolorpicker` | Boja dijaloga |
| ostale kolone | Ime = `NAME` kontrole. Vrijednost ide u kontrolu, ili u `DodatnoPostimajKontrolu` ako je `visible` / `invisible` / `enable` / `disable` / `refresh` / `tabstoped` / `tabnotstoped` |

`ONLOAD2` puni te iste riječi preko `DodatnoPostimajKontrolu` (`Form2.vb` 4482).
