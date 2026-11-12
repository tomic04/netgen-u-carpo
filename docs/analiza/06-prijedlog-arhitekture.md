# 06 — Prijedlog arhitekture `Carpo.NetGen`

Novi projekt je class library u `Carpo.sln`, .NET Framework 4.8, bez reference na Janus. Ne mijenja postojeće Carpo projekte u prvoj isporuci. Ulaz kasnije može biti tanki poziv iz `Carpo.Gen.Start` ili iz Jupitera, ali parser i komande ne žive u `Carpo.UI`.

Tri opcije. Sve učitavaju **izvorni** NetGen XML. Nijedna ne traži od autora da pređe na `<root>`.

## Opcija A — prijevod u `JRoot` pri učitavanju

`NetGenXml` → stablo `JForm` / `JLayout` / `JEventAction` → postojeći `Serializer` + `Starter` + `Manager` / `JupiterEditor`.

Kako bi izgledalo: jedan `INPUT TYPE="text" NAME="Naziv"` postane `<item><textedit name="Naziv"/></item>`, `ONCLICK=":UPDATE#REFRESHMASTERGRID"` postane dvije `action`, `ONSUBMIT` postane `datarun` s `<parameter>`.

Prednosti:

- crtanje, teme, `control populate`, `form open` i ispis REPX već rade
- TestShell može otvoriti rezultat ako se pretvorba spremi u memoriju kao `JRoot` (danas `Starter` čita fajl, pa bi trebao i ulaz iz objekta — mala izmjena `Starter.DeserializeXml`, ili privremeni fajl)
- manje novog UI koda

Mane, i one su presudne:

- apsolutne koordinate nemaju kamo u `JLayout` (dokument 01 i 05). Pretvorba u `row`/`col` je heuristika. Kulture bi izgledale kao ručni prepis, ne kao stara forma
- komandni jezik nije stablo. `if @@robaid is null exec spRaiserror …#OK`, `#FOCUS` u poruci servera, `!!`, `FILLEXPLORER=onlytree` nemaju `JEventAction`
- `@@naziv` traži da se kontrola i dalje zove `Naziv`. To se da sačuvati, ali Carpo evaluator i dalje ne razumije `@@` osim pub-var. Znači i u ovoj opciji treba preprocesor SQL-a — a to je već pola opcije C
- `TotalGenMultiSelectTemp`, DB clipboard, `yearmonth`, `SelectedValue=1` ne postoje u modelu
- `XmlSerializer` je strog. Deset fajlova već pada na običnom XML parseu. Pretvorba koja prvo traži well-formed XML iste fajlove odbija
- ručni prepisi pokazuju da „lijepi“ `JRoot` ljudi pišu tako da **promijene** SQL. Automatski `JRoot` koji SQL ne dira bit će nečitljiv (generirani `dataname`), a onaj koji SQL „sredi“ bit će kriv

Opcija A je dobra kao **debug dump** („pokaži mi što bi Carpo vidio“), ne kao runtime.

## Opcija B — zaseban renderer, Carpo se ne dira

Port `Form1`/`Form2`/`Class1` u C#, kontrole su DevExpress 26, SQL ide kroz vlastiti `SqlConnection`. Carpo projekti se ne referenciraju.

Prednosti:

- semantika se može kopirati funkciju po funkciju (`PokupiParametre`, `Rafalno`, `!!`)
- nema borbe s `JRoot`
- Janus se izbaci točkasto (`GridEX` → `GridControl`, `NumericEditBox` → `TextEdit`/`CalcEdit`)

Mane:

- duplicira se konekcija, log, prijevod, permission, REPX, host handle — sve što `Starter` i `Carpo.Data` već imaju
- dva ispisa (v23 port i v26) će se razići
- Jupiter bi i dalje morao znati koji DLL zvati. Dobijemo drugi NetGen, samo na DX 26
- svaka buduća ispravka Carpo grida (virtual server mode, predlošci) ne vrijedi za stare granule

Opcija B je opravdana samo ako se `Carpo.UI` ne smije ni referencirati. To korisnik nije tražio. Tražio je runtime na Carpo engineu.

## Opcija C — hibrid (preporuka)

Vlastiti model i vlastiti interpreter. Ispod njega Carpo servisi i DevExpress kontrole. **Ne** `JRoot` kao međukorak.

```
NetGen XML (cp1250, tolerantno)
        │
        ▼
Carpo.NetGen.Xml          čita PROJECT / INPUT / COLUMN
        │
        ▼
Carpo.NetGen.Model        forme, kontrole, komande kao string (ne kao JEvent)
        │
        ├─ LayoutEngine     apsolutni panel, koordinate / 14
        ├─ ControlFactory   DX 26: GridControl, TextEdit, DateEdit,
        │                   LookUpEdit, CheckedComboBoxEdit, TreeList, SimpleButton
        ├─ CommandRunner    port NovaGenerickaForma + Rafalno
        ├─ SqlBridge        port SrediSQL + PokupiParametre + ExecSql
        │                   koristi connection string iz Carpo.Data
        └─ PrintBridge      REPX preko Spin.Printing.v26 ako potpis odgovara,
                            inače eksplicitna greška (ne tihi fallback na v23)
```

`Carpo.UI` se referencira tamo gdje je ponašanje već isto (npr. `ParseNetGenStyles`, logger, možda `JGridControl` ako mu se može dati DataTable bez `JGrid` modela). Ako `JGridControl` traži puni `JGrid` XML model, factory u prvoj fazi instancira `GridControl` izravno, da se ne vuče lažni `JRoot`. To je svjesno: ne želimo ovisiti o internim poljima `JGridControl` dok se ne vidi da prima „tuđi“ DataTable. **Neprovjereno** je koliko je `JGridControl` upotrebljiv bez `DefinitionForm`. Čitanje konstruktora nije zatvoreno u ovoj fazi; zato preporuka kaže „izravni DX, Carpo grid tek kad se konstruktor pokaže čist“.

### Zašto C, ukratko

A gubi semantiku na mjestima koja korpus stvarno koristi (`!!` na kartici, `spRaiserror#OK` na kulturama, `/14`, kolona 0/1). B čuva semantiku i baca Carpo. C čuva semantiku i uzima od Carpa ono što nije jezik granule: konekciju, log, DX 26, kasnije ispis.

Cijena C: komandni interpreter se piše iznova, uz VB kao specifikaciju. To je veliki komad, ali je ograničen. `Rafalno` i `PokupiParametre` su već popisani (dokumenti 02 i 03). Ne dira se 9.8k linija `Form2` u komadu; porta se po milestoneu.

### Što se namjerno ne radi

- ne pretvarati XML u `<root>` i spremati ga pored stare granule
- ne „ispravljati“ `HIGHT`, `ALOWEMPTY`, `AGGREGATEFUNCTION`, `PERMISSIO`
- ne mijenjati ime procedure
- ne paliti `FOREACH*` na podlozi
- ne gutati `ASSEMBLYLOAD` i fiskalizaciju u tišini
- ne zvati Janus

### Kuka u hostu, kasnije

Kad C zna otvoriti jednu granulu, `Carpo.Gen.Start.RegisterForm` može na početku pogledati korijen: `PROJECT` → `Carpo.NetGen`, `root` → današnji put. To je jedina izmjena postojećeg projekta, i nije u milestoneu 0–2. Do tada TestShell ili mali harness zove `Carpo.NetGen` izravno.

Druga kuka, `customcontrol`, nije dovoljna: ona očekuje da okolo već postoji Carpo forma.

## Faze

Svaka faza ima provjeru koja ne traži „izgleda dobro“. Dok nema Windows builda (dokument 07), provjera je onoliko koliko ide na Linuxu: parser i stringovi. UI faze čekaju runner.

### M0 — parser i model, bez forme

Ulaz: svih 609 jedinstvenih XML-ova. Izlaz: objekt (forme, `INPUT`, atributi, `TYPE` grana).

Provjera:

- proširiti `tools/netgen_xml_katalog.py` nije dovoljno za C#; M0 dobije NUnit projekt `Carpo.NetGen.Tests` koji na Linuxu **ne može** (net48). Zato M0 prvo ima isti kontrakt u čistom C# kad build postoji, a do tada zlatni fajl ostaje JSON iz ove skripte
- broj `INPUT`, broj formi i `TYPE` grana za svaki fajl mora se poklopiti s JSON-om (dopuštena razlika: 10 strogo neispravnih — oni se broje kao `ParseStatus=Failed`, ne kao uspjeh)
- snapshot za `Kulture.xml` i `KarticaPartnera.xml`: ime svake kontrole, grana `TYPE`, `ONSUBMIT`, prisutnost `!!`
- nepoznati atribut se čuva, ne baca

M0 je gotov kad oba uzorka i agregat 609 daju očekivani broj. Ne crta se ništa.

### M1 — podloga: master grid, gumbi, `@@id`, `@@operateriid`

`ONLOAD` koji je SQL puni `GridControl`. `CMD1`…`BUTTONS` postaju `SimpleButton`. Klik `:IMEFORME` zasad samo javi „forma nije u M1“, osim `REFRESHMASTERGRID`. `ONLOAD=":FILTERFORM"` se prepozna i ne pokuša izvršiti kao SQL.

Provjera:

- fixture bez baze: zamjena `@@id` / `@@operateriid` na poznatom stringu (unit test, čist)
- `ExecSql` pravila: prazno, počinje s `@`, sadrži `@@` → skip, bez slanja (mock)
- s bazom (kasnije, Windows): jedna granula čiji je `ONLOAD` `select` nad malom tablicom, broj redova = broj u gridu
- korpus: svaki `PROJECT/@ONLOAD` koji ne počinje s `:` prolazi kroz substituer i ne baca

### M2 — dijalog: text, number, date, combo, check, button, submit

Apsolutni panel, `/14`. `PokupiParametre` za te tipove. `ONSUBMIT` + lijepljenje, grana `SELECT|INSERT|UPDATE|DELETE`, grana `!!`. `CLOSE` i `OK`. `spRaiserror` se ne parsira iz XML-a; parsira se tekst iznimke (`#OK`, `#close`, `#FOCUS`).

Provjera:

- zlatni stringovi iz `Kulture` INSERT: za zadane vrijednosti kontrola, očekivani SQL je `spKultureNGAzur @Action=0, @Naziv='…', …` (redoslijed `Controls` u WinFormsu nije XML redoslijed — **rizik**). Test neka uspoređuje skup `@ime=`, ne cijeli string, dok se redoslijed ne zaključa očitavanjem `PokupiParametre` (ide redom `Controls`, što je redoslijed dodavanja; adapter mora dodavati istim redom kao XML)
- kartica: `ONSUBMIT` s `!!` ne dobije drugi niz parametara
- `ALLOWEMPTY` false i prazan combo → pokušaj vrijednosti `1`
- combo: value = stupac 0, display = stupac 1, iako se kolone zovu drugačije
- datum `firstdayofyear` na poznati datum (zamrznuti sat u testu)
- 10 neispravnih XML-ova i dalje `Failed`, ne iznimka iz testa

### M3 — tražilica, grid u formi, multidrop, `FOREACH*` samo na dijalogu

`roba#robaid#…` i `spTraziPartneri`. Parametri `@gridIme`, `@nizplavihajdijevaIme`, `@nizoznacenihajdijevaIme`. `FOREACHROW` na `Form2` poziciji. Na podlozi i dalje no-op, s logom.

Provjera:

- fixture tražilice: ulaz `abc def` daje WHERE koji stari `Form3` daje. Očekivani string treba izvući iz `Form3.Trazilica` u zasebnom čitanju prije koda (danas opisano u `NetGen-Kontrole.md`, linija `Form3` ~99). Dok se ta funkcija ne prepiše u test-vektor, M3 nije gotov
- `multidrop` šalje `id1~id2~`
- podloga s `FOREACHSELECTED` u `BUTTONS` ne izvrši petlju (regresija na rupu)

### M4 — stablo, ispis, `INCLUDE`, redoslijed DMS/baza/disk

`TREE` statički čvor i SQL čvor (4 stupca). `yearmonth` tek uz pitanje 6 iz sažetka; do odluke grana postoji i logira „raspon godina je 2005–2009, kao stari kod“. `PRINTMASTER` i `::repx/` zovu jedan `PrintBridge`. Ako v26 potpis ne odgovara, test pada s imenom metode koja fali, ne s praznim ispisom.

Provjera:

- `INCLUDE` na fixtureu od dva XML-a ubaci djecu
- redoslijed izvora je sučelje `IXmlSource` s tri fake izvora; test dokaže DMS pa baza pa disk, uključujući preskok DMS-a na dijalogu ako je log već `JupiterXML Database`
- jedan REPX poziv s parametrima iz kartice (`rptKarticaKontaSKTele2`, imena parametara) — mock, bez pisača

### M5 — multi-select tablica, clipboard, prava, fiskalizacija, `ASSEMBLYLOAD`

Svaka stavka ima poseban prekidač i nije uključena dok korisnik ne odgovori na pitanja 2, 8 i 18. Default: komanda se prepozna, korisnik dobije poruku „nije uključeno“, SQL se ne šalje. To je namjerno glasnije od NetGena.

Provjera: XML koji sadrži `ASSEMBLYLOAD` ne zove `Assembly.Load` u test procesu.

## Harness nad korpusom

Tri razine. Samo prva radi prije Windowsa.

1. **Statika (već postoji).** `python3 tools/netgen_xml_katalog.py` → JSON. Svaki PR koji dira parser usporedi agregate (broj `INPUT`, `TYPE`, komandi) s committed JSON-om. Razlika mora biti objašnjena. Skripta ne otvara formu.
2. **Model (M0, čim se net48 može prevesti).** Konzolni alat `Carpo.NetGen.Corpus` prima folder i ispiše TSV: putanja, status, broj formi, broj kontrola, nepoznate `TYPE` grane. Očekivani TSV se commita. Nema SQL-a, nema UI-a. Na GitHub Actions Windows runneru ovo je prvi zeleni job, prije bilo kakvog TestShella.
3. **S bazom, ručno pa automatski.** Jedna granula, jedan `select` koji ne piše. Usporedba broja redova. Ne 609 granule odjednom: većina zove procedure kojih nema na praznoj bazi, a `spRaiserror` bi bio šum. Lista „smoke“ od 5 imena dolazi od korisnika (pitanje 19). Kulture su dobar kandidat samo ako na bazi još postoje `spKultureNG*`. Ako postoje samo nove `spKulture`, kulture **nisu** dobar smoke za ovaj projekt — pale bi iz krivog razloga.

TestShell se ne proširuje u M0–M2. Nema komandne linije, `_autoMode` se ne čita (`01-carpo-arhitektura.md`). GUI klik na 609 fajlova nije provjera.

## Ovisnosti projekta (kad se doda)

`Carpo.NetGen` referencira:

- `Carpo.Data` (konekcija) — danas samo preko DLL-a u `SharedAssemblies`, vidi dokument 07
- `Carpo.Core` (logger) — isto
- DevExpress 26.1.3: `Data`, `Utils`, `XtraEditors`, `XtraGrid`, `XtraLayout`, `XtraTreeList` po potrebi M3
- ne referencira `Carpo.UI` dok se ne pokaže da se grid/log mogu koristiti bez `JRoot`
- ne referencira Janus, `GridExPrinting`, DX 13

Izlazni DLL ide u isti `SharedAssemblies\Spin` tek kad Jupiter stvarno treba novi ulaz. Do tada build ide u `bin\`, da se ne dira instalacija (isti razlog zbog kojeg `netgen/CLAUDE.md` razdvaja `build.bat` od običnog MSBuilda).
