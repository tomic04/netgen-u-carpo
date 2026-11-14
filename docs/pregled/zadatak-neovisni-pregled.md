# Zadatak: neovisni pregled porta NetGen → Carpo (DevExpress)

Ovo je zadatak za novu sesiju. Pročitaj ga cijelog prije prvog koraka.

## 0. Tvoja uloga

Ti si **neovisni recenzent**. Port je radio drugi model (Claude Opus 5.5), većinom 13. i 14. 11. 2026. Tražimo rupe i bugove koje on nije vidio.

- **Ne vjeruj dokumentaciji, komentarima ni porukama commitova.** Pisao ih je isti autor. Svaku tvrdnju provjeri u kodu, u VB izvoru (`netgen/`) i, gdje treba, na bazi ili u CI-ju.
- **Nalaz mora imati dokaz:** datoteku i redak u C#, redak u VB-u, scenarij (koji XML, koji klik) i, gdje je moguće, test koji pada.
- Odvoji **potvrđeno** (imaš dokaz) od **sumnje** (vrijedi provjeriti).
- Korisnik piše hrvatski/srpski. Odgovaraj, piši commitove, PR-ove i dokumentaciju na hrvatskom, kratko i jasno.

## 1. Cilj projekta (riječi korisnika)

- Postojeće NetGen XML granule (VB.NET, Janus grid) moraju se otvarati i raditi u Carpu (C#, DevExpress 26.1), bez grešaka.
- "Kad su stvari 50-50, uzima se funkcionalnost Carpa."
- "Bitno je da se pređe u DevExpress."
- "Ne staješ dok svaka kontrola i funkcionalnost ne radi."

## 2. Okruženje: gdje se vrtiš

### Cloud sesija

- **Sesija:** Linux workspace u oblaku. Repozitorij je `tomic04/netgen-u-carpo`. Dodaj ga alatom `add_repo` s pristupom `push`, kloniraj u `/home/claude/netgen-u-carpo` i registriraj s `register_repo_root`.
- **Nema Windowsa ni DevExpressa:** rješenje se lokalno ne može izgraditi. Lokalne provjere su u `tools/provjera/README.md`: sintaksa preko tree-sittera, ne-UI klase pod Mono i diferencijalni testovi prema VB-u u `tools/diff/`. Pravi build i testovi idu samo u CI.
- **`gh`:** radi preko proxyja, ali samo REST. GraphQL je blokiran. Draft PR se pretvara u gotov s `gh api -X POST repos/tomic04/netgen-u-carpo/pulls/N/ccr/ready_for_review`.

### CI (`.github/workflows/build.yml`)

Self-hosted Windows runner na stroju **sql-ai**, korisnikovom serveru. Jedan run traje 8–10 minuta. Koraci redom:

- zaštita mrtvog koda (`scripts/ci/mrtve-metode.py`)
- Vrsta (čita oznaku sweepa iz naslova HEAD commita)
- spin stubovi i otvaranje
- build
- smoke NetGen XML-a: `PARSED=1041 FAILED=3`, tih 3 je od prije
- Grid testovi: `Carpo.NetGen.GridTest`, ispis `PASS|FAIL ime | detalj` i na kraju `FAIL=0`
- Raspored formi (RAS-01)
- Klik pregled (`scripts/ui/UiSmoke.ps1`)
- Klik sweep (samo s oznakom)
- Stabilni TestShell, koji se postavlja u `C:\NetGenCarpo\<grana>\bin`
- čišćenje

Kako se radi s CI-jem:
- `concurrency` otkazuje stariji run na istoj grani.
- Push na granu s otvorenim PR-om pokrene dva runa, push i pull_request. Push run otkaži: `gh api -X POST repos/tomic04/netgen-u-carpo/actions/runs/<id>/cancel`.
- Log joba: `gh api repos/tomic04/netgen-u-carpo/actions/jobs/<job_id>/logs`. Job id dobiješ iz `.../actions/runs/<id>/jobs`. Artefakti se ne mogu preuzeti, pa sve što treba mora biti u logu.
- **Runner je jedan.** Runovi čekaju u redu, pa ne puštaj više stvari odjednom bez potrebe.
- **CI otvara TestShell na korisnikovoj radnoj površini na sql-ai.** Kad korisnik kaže da testira, ne pushaj.

### Sweep

Sweep (TestShell `--sweep`) otvara granule redom, klikne kartice, stablo i gumbe, a otvorene dijaloge zatvara pozadinska nit. Pokreće se oznakom u naslovu HEAD commita:

| Oznaka | Što radi |
|---|---|
| `[sweep-N]` | šestina N korpusa `definitions/netgen` na spinu |
| `[sweep-0]` | cijeli korpus u jednom procesu, oko 18 min |
| `[ct-sweep-N:Baza]` | Centar tehnike nad jupiter2019 |
| `[kl-sweep:Server:Baza:Mapa]` | sve granule iz `definitions/netgen/<Mapa>` nad tom bazom, oko 5 min za 411 granula; `[kl-sweep-N:...]` je šestina |

Kako se čita rezultat:
- **Ishodi:** CISTO, ROK, PORUKA, GRESKA, HANG, IZNIMKA.
  - ROK znači granicu sweepa: korpus ima 5 upita po 1 s i 4 s po granuli, klijent 20 upita, 5 s po upitu i 12 s po granuli.
  - PORUKA je RAISERROR iz procedure.
- **U logu su:** redovi `.red` s vremenom, formama i GDI/USER objektima; 40 najsporijih upita; pukli upiti; trag prvih pet HANG granula; cijeli stog iznimaka.
- **Čuvar:** ako granula drži UI dulje od 90 s, zapiše stog UI niti i izađe s kodom 3.
- **Na bazi koja nije spin** gumbi šalju samo SQL koji otvara formu (SWP-02).
- Za sweep bez promjene koda napravi prazan commit s oznakom na zasebnoj grani (`git commit --allow-empty`). Takvu granu ne spajaj.

### Baze

| Baza | Što smiješ |
|---|---|
| `spin` na sql-ai (`localhost`) | testna baza; upis je dopušten; tu je i `JupiterLogs` |
| jupiter2019 `CentarTehnike` | backup klijenta, stariji od XML-ova, pa neki objekti nedostaju; samo čitanje i sweep |
| jupiter2019 `biovega` | ista pravila kao CentarTehnike |
| jupiter2022 | testovi ga ne smiju dirati (`SpinVeza.Spoj` baca iznimku izvan produkcije) |

### Korisnikovo računalo

Ako je sesija povezana s desktop aplikacijom, alati `mcp__remote-devices__*` vide sql-ai. Npr. XML-ovi Centra tehnike došli su s radne površine. Ako nisu povezani, ne trebaju ti.

## 3. Pravila (obavezna)

1. **Lozinke ne upisuješ nikad.** Ne koristiš ni kodove za prijavu koje korisnik zalijepi.
2. **Email korisnika je privatan.** Ne šalji ga vanjskim servisima.
3. **ssMeni samo čitaš.** Ne mijenjaj retke `Parametar`.
4. **Ručno ne radiš u mapi runnera** (`C:\actions-runner\_work\...`).
5. **Ne postavljaš DLL-ove u Jupiterov `RemoteDllPath`.**
6. **Upis na jupiter2019/2022 samo uz korisnika.** To vrijedi i za "samo jedan test".
7. **Fiskalizaciju ne diraš.** Korisnik: "ostavljamo za kraj, nju nećemo bez posebne pažnje dirati". Produkcijski URL Porezne ostaje zatvoren (R111).
8. **Spajanje PR-a:**
   - Samo sa zelenim CI-jem, i to s run-om `pull_request` za točno taj SHA: `gh api -X PUT .../pulls/N/merge -f merge_method=merge -f sha=<sha>`.
   - **Za ovaj pregled:** sam smiješ spojiti samo ispravke koje ne mijenjaju ponašanje u produkciji, dakle testove, sweep, alate i dokumentaciju. Sve što mijenja ponašanje granule u Jupiteru predloži u PR-u i čekaj korisnikov "spoji".
9. **Commitovi:** `git -c user.name="Claude" -c user.email="noreply@anthropic.com" commit ...`, s potpisom (trailerima) koji ti zada tvoja sesija. Grana se zove `pregled/<kratko-ime>`. Nikad ne pushaj na `main`.
10. **Otvorene odluke korisnika ne donosiš sam.** Opiši ih i predloži:
    - SQL-01: razmak na kraju vrijednosti s grida (`docs/pregled/C1-sql.md`)
    - FOR-02: FOREACH na podlozi
    - TREE-SQL
    - PRV-01: prijevod
    - fiskalizacija

## 4. Karta koda

### VB izvor (`netgen/`)

- `Form1.vb` je podloga (Manager): grid, stablo, gumbi `CMDxCAPTION`, lanac `Rafalno` (6546).
- `Form2.vb` je dijalog: kontrole `SrediKontrolu` (2924), `PokupiParametre`, `Rafalno` (6789), FOREACH (8309-8400).
- `Class1.vb`:
  - `ExecSql` (85): jedna otvorena veza `Conn`; SQL s preostalim `@@` se ne šalje (SKIPPED, 113).
  - `SrediSQLPosljeNextForme` (740) i `NadjiVrijednostuPrevParametrima` (857): zamjena `@@`, nepoznato postane `null`.
- `Module1.vb`: `Conn` je `ThreadStatic` (128).

### C# (`Devex_26/`)

**`Carpo.NetGen/Ui/`**
- `Podloga.cs` i partiali `PodlogaRafal.cs` (lanac, `OnFormClosed`, `Dispose`), `PodlogaGrid.cs`, `PodlogaDms.cs`, `PodlogaTipke.cs`.
- `Dijalog.cs` je velik: dijalog, kontrole, lanac dijaloga, `IzvrsiForeach`.

**`Carpo.NetGen/Grid/`**
- `MasterGrid.cs`
- `SqlTekst.cs`, u kojem su i `SpinVeza.Spoj`, `Prijava` i `SaAplikacijom`
- `NgVeza.cs`: jedna veza po granuli, SES-01
- `Stablo.cs`, `DetailGrid.cs`, `ZastitaSql.cs`

**Ostale mape**
- `Carpo.NetGen/Komande/`: lanac i naredbe. `DijalogTok.Produkcija` i `DozvoliUpis` su u `DijalogTok.cs`.
- `Carpo.NetGen/Ulaz/`: `Loader.cs` (Jupiterov `Run`/`Run8`), `Frm.cs` i `FrmCitac.cs` (samostalni dijalog), `NgUlaz.cs`.
- `Carpo.NetGen/Print/PrintBridge.cs`: Spin.Printing, ispis grida i predložaka; `MapaIzvoza` je samo za sweep.
- `Carpo.UI/Forms/Manager/Manager.cs`: Carpo bazna forma. Podloga je nasljeđuje bez Carpo JSON postavki (`JSettings == null`), pa Carpo rukovatelji u njoj moraju izaći.
- `Carpo.TestShell/Main.cs` i `Program.cs`: TestShell, klik pregled, sweep.
- `Carpo.NetGen.GridTest/Program.cs`: testovi. Novi test je metoda koja zove `T("ime", uvjet, detalj)`, a poziv dodaš u popis poziva u `Main` ili u `Podaci()`, koju `Main` zove.

### Korpus i dokumentacija

**Korpus** (`definitions/netgen/`): Agronom granule (239), BIOVEGA (331), Centar tehnike (411), xml-ovi s nevkoša-Zubak (35).

**Dokumentacija:**
- `docs/pregled/plan.md`
- `C1-sql.md`, `C2-lanac.md`, `C3-podloga.md`, `C4-kontrole.md` (tablice s ID-ovima poput SES-01)
- `docs/razlike-netgen-carpo.md` (namjerne razlike R1..R124)
- `docs/pregled-funkcija*.md`

Kad nešto ispraviš, dodaj red s novim ID-om u odgovarajući C-dokument. Ako je razlika od NetGena namjerna, dodaj i R-stavku.

## 5. Što je mijenjano 13.–14. 11. 2026.

Diff vidiš s `gh api repos/tomic04/netgen-u-carpo/pulls/N/files` ili `git log --merges`.

| PR | Što |
|---|---|
| #108 | **B-JUP:** `DijalogTok.Produkcija` u pravom Jupiter ulazu pušta upis i jupiter2022; dijalog šalje ONSUBMIT; test i TestShell ostaju zaštićeni |
| #113 | makni M0 placeholdere (PRD-04) |
| #114 | CI: produkcija ne smije dosegnuti QA kuku (PRD-03) |
| #115 | dijalog: TIMERON i TIMERSQL (TMR-01) |
| #116 | Application Name `Carpo.NetGen.<xml>` u spoju (APP-01) |
| #117–#118 | sweep: rok šestine i mjerenje (SWP-01) |
| #119 | ukinut stari VB6 ispis (ISP-05) |
| #120–#121, #123 | z-redoslijed: okvir i panel iza kontrola (LAY-02/03); provjera rasporeda formi (RAS-01); `JupiterLogs` samo gdje postoji (LOG-02) |
| #122 | korpus Centra tehnike |
| #124 | tražilica ne šalje `@searchboxIME` u parametre procedure (PAR-02) |
| #125 | sweep klijenta nad jupiter2019 (SWP-02) |
| #126 | stablo iz procedure O(n) (STB-02) |
| #127 | **CMB-02:** granule s COMBO atributom nisu se otvarale (Carpo rukovatelj bez JSettings); sweep bilježi iznimke (SWP-03), PORUKA (SWP-04) |
| #128 | **SES-01:** jedna SQL veza po granuli (`NgVeza`), bez poola; nakon timeouta se zatvori (R124) |
| #129 | **ULZ-07:** Run/Run8 s korisnikom spaja SQL prijavom; samostalni dijalog u istoj sesiji |
| #130 | sweep klijenta (SWP-06/08); forme se u sweepu uništavaju, `Podloga.Dispose` otpusti vezu, `Manager_Shown` ne dira uništenu formu (SWP-07) |
| #131 | sweep: spori i pukli upiti, trag HANG granula; ispis grida u sweepu ide u PDF (SWP-09/10) |

Rezultat zadnjih sweepova:
- **Centar tehnike nad jupiter2019 (411 granula):** 312 čisto, 44 PORUKA, 41 GRESKA, 13 ROK, 1 HANG, 0 iznimki. Prema autoru greške su razlike starijeg backupa i XML-a ili greške procedura i XML-a. Provjeri barem uzorak.
- **Cijeli korpus na spinu (1016 granula):** 0 iznimki.

## 6. Gdje tražiti (prioritet od najvećeg rizika)

### P1. B-JUP: upis u produkciji

Ovo je najveći rizik: granula u Jupiteru sada piše u bazu klijenta.
- Gdje se postavlja `DijalogTok.Produkcija` i može li ostati `true` u procesu u kojem kasnije radi test ili TestShell? Je li statičan?
- Vrijedi li uvjet u `Loader` i `Frm` (bez `TestXmlPath`, bez `BezProzora`) i za `Run` i za `Run8`?
- Poštuje li ga svaki put koji piše:
  - `SqlTekst.JeZabranaPisanja`
  - `ZastitaSql`
  - `SmijePoslati`
  - `FrmCitac` (`DozvoliUpis`)
  - `Podloga.Citaj`
  - master grid
  - combo
  - CREATETEXTFILE
  - DMS
  - ONSUBMIT
  - REFRESHMASTER
- Je li ijedan put ostao zatvoren, a u NetGenu piše? Je li ijedan otvoren, a ne bi smio biti (Porezna, FOREACH na podlozi)?
- Usporedi s `Form1.vb` i `Form2.vb`: šalje li Carpo ONSUBMIT i osvježava li master istim redom kao NetGen?

### P2. SES-01: jedna veza po granuli (`Grid/NgVeza.cs`)

- **Ključ i brojanje korisnika:**
  - Spoj je ključ i sadrži ime XML-a, pa dvije podloge iste granule dijele vezu. Je li brojanje `Prijavi`/`Otpusti` ispravno kroz `OnFormClosed` + `Dispose`, kroz `Prikazi` pozvan dvaput, kroz ONLOAD grešku i kroz zatvaranje dijaloga u `Frm`?
  - Može li se veza zatvoriti dok je druga podloga još koristi, ili ostati otvorena zauvijek?
- **Niti i ugniježđeni pozivi:**
  - Može li isti thread ući u `Citaj` dok je `Fill` u tijeku (InfoMessage, DoEvents, modalni dijalog)?
  - Što radi grana `Zauzeta` (zasebna veza) za upite koji očekuju istu sesiju?
- **Pooling i timeout:**
  - S `Pooling=false` svaka otvorena granula drži vezu. Kako se ponaša Jupiter s mnogo granula?
  - Nakon timeouta veza se zatvori (R124). Je li to bolje od NetGena ili može nešto slomiti?
- **Upiti izvan `NgVeza`:** master grid, `Podloga.Citaj`, dijalozi i `FrmCitac` idu kroz nju. Što još ide zasebno (`GreskaSql`, Spin.Printing, DMS, `XmlIzvor`, ispis s `konekcija`) i treba li to biti ista sesija kao u NetGenu (`Conn`, `Conn3`, `DefaultConnection = 2`)?
- **Transakcije:** proc koji ostavi otvorenu transakciju sada drži zaključavanja do zatvaranja granule. NetGen radi isto. Ima li u korpusu takvih slučajeva?

### P3. ULZ-07: SQL prijava (`SpinVeza.Prijava`)

- Korisnik i lozinka su statični za cijeli proces. Može li Jupiter u istom procesu otvoriti granule s različitim korisnicima?
- Gradi li ijedan drugi put spoj bez prijave: `DijalogPomoc`, `GreskaSql`, ispis, DMS, Spin.Printing (`konekcija`), `TestHooks`?
- Ide li lozinka igdje u log, u Spin.Printing ili u naslov? Pokriva li `NgDnevnik` sve oblike (`Password=` s navodnicima, `PWD=`)?

### P4. Carpo bazna forma bez JSettings

`Manager.cs`: koji još Carpo rukovatelji mogu okinuti u NetGen podlozi, a diraju JSettings, Definition ili ComponentExt? Kandidati: `bbiGridRefresh`, `bbiOptionSum`, `bbiGridExport`, `bbiGridReset`, F8, tipke, gašenje. Nađi ih i provjeri dosežu li se iz podloge.

### P5. Zamjena parametara

- VB `ExecSql` preskače SQL u kojem je ostao `@@` (SKIPPED, `Class1.vb` 113). Carpo (`Podloga.Citaj`, `FrmCitac`, `SrediPoslije.Petlja`) preostale `@@` pretvara u `null` i šalje. Dokaži je li to ista stvar ili razlika, i na kojim granulama.
- Datumi (`@DAT…`, format `yyyyMMdd`, `DatumjezaIspis`), `@napomena`, `ajdijeva`, `'null'` u navodnicima.

### P6. Neriješeno iz sweepa

- `MPKonUt` i `MPKonUtNG` (`##ttnaplata`), `PregRbTr` i `MPPregRbTrCT` (`##tempstr`): je li to stvarno redoslijed koraka ili Carpo propušta korak koji NetGen radi (npr. detail grid koji puni `##`)?
- `MatTem` (HANG, 11–22 s): zašto?
- Granule koje stanu na ROK: što bi se dogodilo s punim brojem upita?

### P7. Sve ostalo

Što god vidiš: kontrole dijaloga, grid, stablo, ispis, prijevod, popup, tipke. Korisnik traži da sve radi kao u NetGenu.

Ništa od ovoga nije provjereno u pravom Jupiteru, jer shim `NetGenDLL` ne postoji. To ne rješavaš, ali to imaj na umu kad procjenjuješ rizik.

## 7. Kako raditi

1. **Postavi okruženje i pročitaj:** kloniraj repozitorij. Pročitaj `docs/pregled/plan.md`, `C1`–`C4`, `docs/razlike-netgen-carpo.md` i `tools/provjera/README.md`, ali kao tuđe tvrdnje.
2. **Za svako područje iz točke 6:** pročitaj C# i odgovarajući VB. Napiši scenarij koji bi pokazao razliku.
3. **Dokaži:**
   - čistu logiku testom pod Mono
   - ponašanje forme GridTestom u CI-ju, preko PR-a s testom koji pada
   - ponašanje granule sweepom (`[kl-sweep:localhost:spin:<Mapa>]` ili `[sweep-0]`)
   - SQL ponašanje na spinu
4. **Izvještaj:** `docs/pregled/pregled-neovisni-2026-11-<dan>.md`, kroz PR koji smiješ spojiti sam (samo dokumentacija). Tablica:

   | ID | Ozbiljnost | Gdje (C#) | NetGen (VB) | Scenarij | Dokaz | Prijedlog |
   |---|---|---|---|---|---|---|

   - Ozbiljnost: **P0** gubi ili kvari podatke ili piše gdje ne smije; **P1** granula ne radi; **P2** radi drukčije od NetGena; **P3** sitnica.
   - Na kraju dodaj popis **sumnji** koje nisi uspio dokazati i popis mjesta koja si **pregledao i našao ispravnima**, da se vidi pokrivenost.
5. **Ispravci:** svaki u zasebnom PR-u, s testom koji bez ispravka pada. Što mijenja produkciju čeka korisnikov "spoji".
6. **Korisniku** pošalji kratak sažetak: broj nalaza po ozbiljnosti i tri najvažnija. Ne prepisuj cijeli izvještaj.

## 8. Korisne naredbe

```bash
R=repos/tomic04/netgen-u-carpo
gh api "$R/actions/runs?per_page=5" --jq '.workflow_runs[] | "\(.id) \(.event) \(.head_branch) \(.status) \(.conclusion)"'
gh api $R/actions/runs/<id>/jobs --jq '.jobs[0].id'
gh api $R/actions/jobs/<job>/logs > /tmp/log.txt
grep -a "^.\{29\}FAIL \|FAIL=" /tmp/log.txt              # testovi
gh api -X POST $R/actions/runs/<id>/cancel                # otkaži dvostruki run
gh api $R/pulls --method POST --input /tmp/pr.json        # {"title","head","base":"main","body"}
gh api -X PUT $R/pulls/<n>/merge -f merge_method=merge -f sha=<sha>
```

Kraj PR opisa: `🤖 Generated with [Claude Code](https://claude.com/claude-code)` i link na tvoju sesiju.
