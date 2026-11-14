# Plan pregleda i dorade `Carpo.NetGen`

Datum: 2026-11-13. Izvor: GitHub `tomic04/netgen-u-carpo` (svih 80 grana i PR-ova) i lokalni folder `netgen u carpo`.

**Cilj projekta:** Carpo otvara postojeće NetGen XML granule bez greške. XML se ne mijenja. Kad je odluka 50-50, uzima se Carpo ponašanje.

**Cilj ovog plana:** provjeriti da li napisani kod zaista radi ono što radi NetGen. Tamo gdje ne radi, dovršiti ga. Dokaz uvijek dolazi iz oba izvora: NetGen VB (`netgen/`) i Carpo (`Devex_26/`).

---

## 0. Polazno stanje

| Što | Broj | Izvor |
|---|---|---|
| NetGen `Form1.vb` / `Form2.vb` / `Class1.vb` | 8 729 / 9 771 / 1 990 linija | `netgen/` |
| NetGen `Sub`/`Function` (bez designera) | 476 | `docs/pregled-funkcija.md` |
| `Carpo.NetGen` (C#) | ~13 000 linija, ~60 datoteka | grana `netgen-form1-pregled-0b55` |
| `Carpo.TestShell/Main.cs` dopisano | +1 041 linija (+1 380 na `treci-otvoreno`) | git diff |
| Jedinstvenih XML granula | 609 (577 cp1250, 10 UTF-16) | `tools/out/netgen-xml-katalog-sazetak.txt` |
| Dokumentirane namjerne razlike (R-brojevi) | 98 | `docs/razlike-netgen-carpo.md` |
| QA granula (CI #31) | 571 provjera: 411 zeleno, 160 otvoreno, 0 pad | `docs/granule/Otvoreno-za-implementaciju.md` |
| Podatkovni sloj | 89 isto, 48 još nedostaje | `docs/pregled-funkcija.md` |
| Form1 | 54 isto, 136 još nedostaje | `docs/pregled-funkcija-form1.md` |
| Javne metode koje zove samo test ili nitko | 62 iznimke | `scripts/ci/mrtve-iznimke.txt` |
| UI smoke kroz pravi TestShell | 6 granula | `scripts/ui/UiSmoke.ps1` |

### Stanje gita

- **Na `main` nije ništa spojeno.** Tamo je samo početni import. Rad je u 80 otvorenih PR-ova od više paralelnih Cursor agenata, s oznakama `3266`, `0b55`, `72f9` (QA), `889d`, `78a6` i `5ee3`.
- Grane se razilaze u **25 vrhova**. Nijedan vrh ne sadrži sve.
- Lokalni folder odgovara grani `netgen-form1-pregled-0b55`, koja ima 198 commitova.
- Grana `netgen-treci-otvoreno-889d` ima **73 commita kojih lokalno nema**: noćni sweep svih granula, `Podloga.cs` s +867 linija, `MultiOdabir.cs` i SQL skripte za spin.
- QA agent je u granama `qa-form1-sve` i `qa-pregled-sve` vratio 155 statusa "portano" u "još nedostaje". Razlog je da je kod postojao, ali ga je zvao samo test, a test je uspoređivao povratnu vrijednost, ne učinak na formi, gridu i bazi.

### Poznati rizici prije pregleda

1. **Kod koji postoji, a nije na putu od ulaza.** Postoje 62 iznimke, a klase `Form1Ulaz` i `Form1Srce` već su jednom maknute.
2. **Testovi dokazuju povrat funkcije, ne učinak.** Zbog toga "isto" u dokumentima nije uvijek dokazano.
3. **`SqlTekst.JeZabranaPisanja`.** Na spinu se nijedan `insert`, `update`, `delete`, `merge` ni `create` ne šalje. Spremanje podataka zato nikad nije stvarno izvedeno.
4. **`scripts/spin/stubovi.sql` stvara prazne procedure.** Zelen CI ne znači da je SQL logika točna.
5. **Dirani su zajednički Carpo projekti.** Promijenjeni su `Manager.cs`, `Manager.Designer.cs`, `JTreeListControl.cs`, `ComponentTree.cs`, `JupitetShared.Designer.cs`, HintPathovi u 10 csproj datoteka te `Directory.Build.props` i `.targets`. To može pokvariti postojeće Carpo granule kod klijenata.
6. ~~**`Placeholders.cs`** još ima `LayoutEngine.Crtaj`, `CommandRunner.Pokreni` i `SqlBridge.Exec`, koji bacaju `NotSupportedException`.~~ Maknuto: nitko ih nije zvao. `PrintBridge` je sada u `Print/PrintBridge.cs`.

---

## 1. Pravila odlučivanja (vrijede za cijeli pregled)

### 1.1 Što znači "otvara bez greške"

Granula je ispravna kad je zadovoljeno sve ovo:

- podloga ili dijalog se otvori, a stablo, master i detail se napune,
- gumbi, dvoklik, popup i dijalozi rade lanac naredbi do kraja,
- nema neuhvaćene iznimke ni MessageBoxa koji NetGen ne bi pokazao,
- **SQL tekst koji ode serveru isti je kao kod NetGena** za isti klik i iste unose.

### 1.2 Pravilo 50-50: kad pobjeđuje NetGen, a kad Carpo (prijedlog, treba tvoja potvrda)

**NetGen se prati 1:1, bez rasprave**, kad ponašanje utječe na:

- tekst SQL-a: zamjenu `@@`, lijepljenje `@ime=`, `!!`, `¸¸`, `~~`, format datuma i broja,
- kontrolni tok: `#OK`, `+OK`, `#close`, `#FOCUS(`, preskok kad `@@` preživi, prekid lanca na grešci, redoslijed naredbi,
- ugovor s Jupiterom: `Loader.Run8` i `frm.Run8`, polja `WCaption` i `hWnd`, `ShellResize`,
- ono što procedure kod klijenta čitaju: `TotalGenMultiSelectTemp`, potpis `totalgen` za `ASSEMBLYLOAD`, fiskalizaciju i DMS (`NetGenFinder`).

**Carpo smije pobijediti** kad je razlika samo u nečem od ovoga:

- izgled i raspored (`TOP`/`LEFT`, fontovi, boje, ikone) te DevExpress kontrola umjesto Janusa,
- UX koji ne mijenja podatke (Find panel, sort, filter, export, izgled ispisa),
- NetGen bug, **ali samo ako nijedna granula u korpusu o njemu ne ovisi**. Dokaz je pretraga po svih 609 XML-ova.

Svaka odluka "pobjeđuje Carpo" dobiva R-broj u `docs/razlike-netgen-carpo.md`, uz liniju NetGena i dokaz iz korpusa.

### 1.3 Statusi u pregledu

| Status | Značenje |
|---|---|
| `isto` | Dokazano testom učinka (poslani SQL, stanje kontrole, otvorena forma), ne samo povratom funkcije |
| `razlika Rxx` | Namjerna razlika, zapisana |
| `nedostaje` | U Carpu nema tog ponašanja |
| `mrtvo` | Kod postoji, ali nije na putu od `Loader.Run8`, `Frm.Run8` ili TestShella |
| `greška` | Na putu je, ali radi drugačije od NetGena, a to nije zapisana razlika |

---

## 2. Faza A: jedna grana (preduvjet)

**Stanje 13. 11.:** napravljeno. Grana `integracija`, detalji i odluke po granama u `docs/pregled/A-grane.md`. Čeka push i CI na `sql-ai`.

Nema smisla pregledavati kod koji postoji u 25 verzija.

| # | Zadatak | Izlaz |
|---|---|---|
| A1 | Za svaki od 25 vrhova: commitovi kojih nema u `form1-pregled` (`git cherry`, patch-id), dirane datoteke, tema | `docs/pregled/A-grane.md` |
| A2 | Prijedlog integracije: `form1-pregled`, pa `treci-otvoreno` (sweep, Podloga), pa QA statusi iz `qa-*-sve`, pa ostali vrhovi samo ako imaju jedinstvene commitove | ista datoteka |
| A3 | Spajanje u granu `integracija` uz riješene konflikte. Build i CI moraju proći kao i prije | grana ili patch |
| A4 | Ostali PR-ovi se označe kao uključeni ili odbačeni, s razlogom | komentar na PR |
| A5 | Pravilo dalje: jedna tema, jedna mala grana iz `integracija`, test učinka uz svaki PR. Bez paralelnih agenata na istim datotekama | `docs/ci.md` |

---

## 3. Faza B: automatska mapa pokrivenosti

Ručno čitanje 20 000 linija VB-a bez mape propušta stvari, pa se mapa izvodi alatom.

| # | Zadatak | Kako | Izlaz |
|---|---|---|---|
| B1 | Svih 476 NetGen funkcija u jednoj tablici: datoteka, linija, što radi, Carpo par | spajanje `pregled-funkcija*.md` + parser VB `Sub`/`Function` | `docs/pregled/B-funkcije.tsv` |
| B2 | Graf poziva u `Carpo.NetGen` od triju ulaza: za svaku metodu "na putu", "samo test" ili "nitko" | proširenje `scripts/ci/mrtve-metode.py` (Roslyn ako ga runner ima) | `B-graf.tsv` |
| B3 | Matrica XML jezika: element, atribut, vrijednost `TYPE`, broj granula, gdje čita NetGen (`GetNamedItem`), gdje čita Carpo.NetGen | proširenje `tools/netgen_xml_katalog.py` + grep po oba izvora | `B-xml-matrica.tsv` |
| B4 | Rang rizika = broj granula koje koriste × utjecaj (SQL i tok > dijalog > izgled) | iz B1 do B3 | `B-rang.md` |

Rezultat B3 odmah pokazuje atribute koje NetGen čita, a Carpo.NetGen ne. To su kandidati za `nedostaje`, poredani po tome koliko granula pogađaju.

---

## 4. Faza C: pregled po modulima (redoslijed po riziku)

### Postupak za svaki modul

1. **NetGen:** popis ponašanja kao ulaz i izlaz (poslani SQL, poruka, stanje forme), s linijama.
2. **Carpo.NetGen:** gdje je to ponašanje i je li na putu od ulaza (iz B2).
3. **Usporedba**, uključujući rubne slučajeve: prazno, `null`, `DBNull`, zarez i točka, cp1250 znakovi, velika i mala slova u imenima atributa, `On Error Resume Next` koji u NetGenu tiho preskače.
4. **Status i nalaz** s `datoteka:linija` u oba izvora.
5. **Za svaku grešku test koji pada prije ispravka.** Ispravka ide tek u fazi E.

### C1: SQL sloj (najveći rizik: podaci)

| NetGen | Carpo.NetGen |
|---|---|
| `Form1.SrediSQL` 2470–2503, `Form2.SrediSQL2` 2876–2920 | `Grid/SqlTekst.cs` (`Sredi`, `Sredi2`) |
| `Class1.SrediSQLPosljeNextForme` 740–853, `NadjiVrijednostuPrevParametrima` 856–977 | `Komande/SrediPoslije.cs` |
| `Form2.PokupiParametre` 3406–3906, `Button1_Click` 3367–3388 | `Komande/ParametriSvi.cs`, `Ui/Dijalog.cs` `SviParametri` |
| `Form1.PokupiParametreSaGrida` 2544–2612, hijerarhija 2615–2643 | `Grid/DetailGrid.cs` |
| `Class1.ExecSql` 85–118 (odluka), 158–205 (`#OK`/`#close`/`#FOCUS`), `ExecSql2` 230–252 | `SqlTekst.Odluka`, `Komande/Lanac.cs`, `Grid/MasterGrid.cs`, `Grid/GreskaSql.cs` |

Posebno se provjerava:

- poredak zamjena i granica tokena `@@`,
- datum `yyyyMMdd` naspram kulture,
- broj `0` kad je polje prazno,
- `~` u `ajdijeva`,
- clipboard (`SETDEFAULTS`).

`JeZabranaPisanja` mora postati **konfiguracija** (spin uključeno, produkcija isključeno), a ne kod koji se mora sjetiti ugasiti.

### C2: komande i lanac (`Rafalno`)

| NetGen | Carpo.NetGen |
|---|---|
| `Form1.vb` 4570–4801, 6579–6850 (podloga) | `Komande/GumbTok.cs`, `Lanac.cs`, `RafalNaredba.cs` i 20 malih `*Naredba.cs` |
| `Form2.vb` 6911–7494 (dijalog) | `Komande/DijalogTok.cs`, `DijalogKlik.cs`, `Form2Dogadjaj.cs` |

Za svaku naredbu iz `docs/naredbe-stanje.md` provjerava se:

- parsiranje segmenata,
- velika i mala slova,
- prekid lanca na grešci,
- što se događa s ostatkom niza.

Poseban slučaj je `FOREACH*` na podlozi, gdje NetGen pojede sljedeći segment. To se mora preslikati 1:1, jer granule mogu računati na takvo ponašanje.

### C3: podloga (Form1)

| NetGen | Carpo.NetGen |
|---|---|
| `ONLOAD` 3879–3882, `SETDEFAULTS` 3862–4009, `ONLOAD2` 3914–4035 | `Ui/Podloga.cs`, `Komande/OnLoad2.cs`, `Kontrole/ClipboardNg.cs` |
| Stablo 4296–4480, prazno stablo i combo 4310 | `Grid/Stablo.cs`, `Shell/ShellLayout.cs` |
| Master i detail, dvoklik, `FILLEXPLORER` 5445–5475, `NEXTFORM` 5381 | `Grid/MasterGrid.cs`, `DetailGrid.cs`, `Komande/DijalogTok.cs` |
| Izbor Manager ili obična forma | `Shell/OdabirForme.cs`, `Ui/ObicnaForma.cs` |

Dio C3 je i usporedba s granom `treci-otvoreno`. Njezinih 867 linija u `Podloga.cs` lokalno nikad nije pregledano.

### C4: dijalog (Form2) i kontrole po `TYPE`

- Svih 46 vrijednosti `TYPE` iz B3, za svaku: kako NetGen crta i čita vrijednost (`Form2`), a kako Carpo (`Kontrole/Raspored.cs`, `Ponasanje.cs`, `Rijetke.cs`, `ComboDijalog.cs`).
- `TYPE` je **podstring**, pa se ponašanje mora poklapati i za tipfelere koje NetGen pogodi.
- Raspored: `/14` (`FakPretvorbe`, `Module1.vb` 65, `Form2.vb` 1093) i tab order se preslikavaju 1:1. Apsolutni piksel je zapisana razlika.
- Ovisni combo (vrijednost u stupcu 0, `Form2.vb` 2396–2431), multidrop, search box, `INCLUDE` (844–934).

### C5: grid i stablo

- Stupci: skriveni ID, int širine 34 s maskom `##0`, string bez WIDTH širine 100, `WIDTH/14` (`Class1.vb` 1087–1244, `Form1.vb` 7522).
- Total red, kvačica, grupiranje, sort, `HEADERALIGNMENT`, uvjetni format.
- Ovdje Carpo smije pobijediti za izgled, ali ne za vrijednosti koje idu u parametre.

### C6: ulaz i ugovor s Jupiterom

- `Loader.vb`: `Run8`, polja, `ShellResize` 22–26, AppDomain zakomentiran 86 i 175.
- `frm.vb` 88 (`Run8` stvara `Form2`).
- U Carpu: `Ulaz/Loader.cs`, `Frm.cs`, `NgUlaz.cs`. Provjera ide potpis po potpis, uključujući `hWnd = 0` dok prozor ne postoji.
- Plan za shim `NetGenDLL` (iz `docs/analiza/06`) i za sync folder `JupiterClient.exe` koji prepisuje DLL.

### C7: čitač XML-a

- `Xml/NetGenXmlReader.cs`, `NgGrana.cs` i `IncludeXml.cs` naspram `Form2.ProcitajXML`.
- Kodiranja: cp1250, UTF-8, UTF-8 s BOM-om i UTF-16.
- Tipfeleri koje NetGen čita, a koje ignorira (`docs/analiza/03`, odjeljak 12).
- Ako `Load` baci iznimku, mora to biti ista greška kao u NetGenu.

### C8: ispis, ASSEMBLYLOAD, fiskalizacija, Word, DMS

| NetGen | Carpo.NetGen | Otvoreno |
|---|---|---|
| Ispis `Form1.vb` 4945–6067, `Form2.vb` 9041–9056, `Class1.vb` 297–353 | `Placeholders.cs` `PrintBridge`, `Print/*` | Kod `lodani` i lijevo/centar/desno zaglavlja nije potvrđeno da imaju 1:1 par u `PrintLink` |
| `ASSEMBLYLOAD` 4584–4623, 6681–6727, `Form2.vb` 7424–7456 | `Poziv/AssemblyPoziv.cs` | `strParametriSvi`: search, grid i radio još nisu u nizu |
| Fiskalizacija 286–434, 4643–4781 | `Poziv/FiskalPoziv.cs`, `Komande/FiskalTok.cs` | SOAP se ne šalje |
| Word `WORDDOT1` 5093–5159 | `Print/WordDot.cs` | Provjeriti je li stvarno na putu |
| DMS `NetGenFinder` 3172–3182 | ? | Provjeriti postoji li uopće |

### C9: regresija zajedničkog Carpo koda

- Diff po datoteci za `Manager.cs`, `JTreeListControl.cs`, `ComponentTree.cs`, `JupitetShared.Designer.cs`, 10 csproj datoteka, `Directory.Build.*` i `Carpo.TestShell`.
- Svaka izmjena mora biti ili neutralna za postojeće Carpo granule, ili opt-in (samo kad je granula NetGen).
- Provjera: `DxAudit26` smoke i 2 do 3 prave Carpo granule iz `definitions/carpo` prije i poslije.

### C10: kvaliteta testova i CI-ja

- Svaki test u `GridTest` (160 KB) i `tests/Carpo.NetGen.Granule` dobiva oznaku: učinak (SQL, kontrola, forma) ili samo povrat.
- Testovi koji provjeravaju samo povrat ne smiju nositi status `isto` bez dodatnog dokaza.
- `tests/ui/katalog.txt` je prazan. Popuniti ga stvarnim UI testovima iz faza D i F.

---

## 5. Faza D: diferencijalni test (najjači dokaz, jer imamo oba izvora)

| # | Što | Kako |
|---|---|---|
| D1 | **SQL trag** | Ista granula i isti klikovi u NetGenu (`Launcher2`) i u `Carpo.NetGen` (TestShell), na `spin`. Extended Events sesija hvata `sql_batch_starting` i `rpc_starting`, filtrirano po aplikaciji ili hostu. Tekst se normalizira (razmaci, GUID, vrijeme) i uspoređuje. Svaka razlika je nalaz za C1 ili C2. |
| D2 | **Čiste funkcije** | `NetGenDLL.dll` se učita refleksijom. Ista ulazna vrijednost ide u VB funkciju (`SrediSQL*`, `NadjiVrijednostuPrevParametrima`, formati) i u C# port. Ulazi se generiraju iz korpusa, iz stvarnih `ONSUBMIT` i `ONLOAD` tekstova. |
| D3 | **Klik scenariji** | Isti `scripts/ui/klik-*.txt` za oba hosta, da D1 uspoređuje iste radnje |
| D4 | **Upisi** | Dok `JeZabranaPisanja` stoji, upis se uspoređuje samo kao tekst koji bi bio poslan. Za pravi upis treba kopija spina ili transakcija s rollbackom (tvoja odluka, točka 9). |

D1 je pravi odgovor na pitanje radi li isto. Ne ovisi o tome koliko je agent dobro pročitao VB.

---

## 6. Faza E: dorada

Backlog nastaje iz nalaza u C i D.

| Prioritet | Što |
|---|---|
| **P0** | Pad pri otvaranju; razlika u poslanom SQL tekstu; razlika u kontrolnom toku; regresija postojećih Carpo granula |
| **P1** | Naredba, gumb ili `TYPE` koji ne radi, a koristi ga više granula (rang iz B4); `mrtvo` koje treba biti na putu |
| **P2** | Razlike u izgledu i UX-u: odluka "pobjeđuje Carpo" s R-brojem ili ispravak |

Pravila za svaki ispravak:

- Jedan PR je jedna tema i grana iz `integracija`.
- Uz svaki ispravak ide test učinka i redak u `tests/ui/katalog.txt`, ako je UI.
- XML se ne dira.
- Zajednički Carpo kod se ne mijenja bez opt-ina za NetGen.
- Placeholderi u `Placeholders.cs` se ili spoje, ili obrišu. Ne ostaju kao zamka koja baca iznimku.

---

## 7. Faza F: pokrivenost korpusa

1. Noćni sweep svih 609 granula (postoji na `treci-otvoreno`). Cilj je **0 padova pri otvaranju**, uz izvještaj po granuli.
2. Najvažnije granule po stvarnoj upotrebi (`ssMeni` i Jupiter log otvaranja, ako postoji) dobivaju puni UI test učinka i D1 trag.
3. 28 prepisanih parova (`definitions/Prepisane_granule`) služe kao referenca kako su Carpo autori sami riješili slučajeve 50-50.
4. Popis 160 otvorenih stavki (`Otvoreno-za-implementaciju.md`) se zatvara kroz fazu E.

---

## 8. Faza G: kontrola prije produkcije

- [x] `JeZabranaPisanja` je konfiguracija i na produkciji je isključena: `DijalogTok.Produkcija` pali samo pravi Jupiter ulaz (B-JUP).
- [x] Zaštita od `jupiter2022` vrijedi samo izvan pravog Jupiter ulaza (B-JUP, odluka 9.3). Jupiter smoke i preskok `Prenesi`/`Retci` u `build.yml` su i dalje kako su bili.
- [x] QA kuka (`TestHooks/GranulaSesija`) nije dostupna iz produkcijskog `Loader`. Nijedna datoteka izvan `TestHooks/` je ne spominje, a `scripts/ci/mrtve-metode.py` ruši build ako se to promijeni (PRD-03).
- [x] Nijedan placeholder koji baca `NotSupportedException` nije na putu. Tri M0 klase su maknute, a `NotImplementedException` u `Carpo.UI` su Carpo kod, ne NetGen.
- [ ] Shim `NetGenDLL` s istim `Loader.Run8` i `ShellResize`, testiran u pravom Jupiteru (`Jupiter.exe` 10.5.26.0+).
- [x] Lozinke se ne upisuju u log (NetGen CBLog ih sadrži). `NgDnevnik` skriva polja s `PASSWORDCHAR` (R120, LOG-01).
- [ ] Fiskalizacija: SOAP uključen tek kad je `Raverus.FiskalizacijaDEV.COM.dll` na stroju i testiran na `cistest`.
- [ ] Postojeće Carpo granule prolaze isti smoke kao prije.
- [ ] Prebacivanje granulu po granulu u `ssMeni`, s povratom na NetGen.

---

## 9. Odluke

Donesene 13. 11. 2026.:

1. **Integracijska grana:** da. `form1-pregled` + `treci-otvoreno`, a ostalo samo ako ima jedinstveni sadržaj. Rezultat je u `docs/pregled/A-grane.md`.
2. **Pravilo 50-50:** da, granica iz točke 1.2.
3. **Upisi u testu:** smiju se slati na `sql-ai`/`spin`, **i na baze na jupiter2019 i jupiter2022**. Zbog toga stara zabrana iz `docs/analiza/00-sazetak.md` i `build.yml` više ne vrijedi. Zaštite u kodu (`SqlTekst`, `SpinVeza`, ugašen Jupiter smoke, preskok `Prenesi`/`Retci`) idu u fazu E kao stavka B-JUP.
4. **NetGen na `sql-ai`:** da, `Launcher2` smije raditi tamo. D1 je moguć.

Još otvoreno:

5. **Najvažnije granule:** koje se granule najviše koriste (top 20 do 30)? Ako postoji log otvaranja, dovoljan je on.
6. **Push:** ja pripremam granu kao paket, a ti pushaš.

---

## 10. Redoslijed rada

```
A (grane)  ──►  B (mapa)  ──►  C1–C2 (SQL, komande)  ──►  C3–C8  ──►  C9–C10
                    │                                     ▲
                    └──►  D (diferencijalni test, paralelno, čim je D1 postavljen) ──┘
                                              │
                                              ▼
                                   E (dorada, P0 → P1 → P2)  ──►  F (korpus)  ──►  G (produkcija)
```

Što radim ja, a što ti:

- **Ja:** A1 i A2, cijeli B, pregled C, D2 (čiste funkcije), backlog i ispravci za E, dokumenti u `docs/pregled/`.
- **Ti ili runner:** odluke iz točke 9, pokretanje D1 i UI testova na Windowsu (`sql-ai`), push i spajanje PR-ova.

Svaka faza završava dokumentom u `docs/pregled/` i kratkim sažetkom: što je provjereno, koliko je nalaza po statusu i što blokira sljedeću fazu.

## Nove granule (500-600 s drugih servera)

1. `python3 tools/nove_granule.py <mapa>`: XML-ovi koji se ne čitaju, novi `TYPE` (i grana koju bi NetGen uzeo), atributi koje Carpo.NetGen ne poznaje (uz oznaku čita li ih NetGen) i naredbe lanca bez literala u kodu.
2. Kopirati u `definitions/netgen/<server>/` (podfolderi ostaju), pa `tools/netgen_xml_katalog.py` i `tools/pregled_b3_xml.py` za matricu.
3. PR: CI (Smoke parsera, `nocni-otvaranje` nad cijelim `definitions/netgen`, Klik pregled) pokaže granule koje padnu.
