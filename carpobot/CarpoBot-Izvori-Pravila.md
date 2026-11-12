# CarpoBot – izvori pravila i dokumentacije

Pregled gdje se šta nalazi i kada šta koristiti.

---

## Rules (.cursor/rules/)

| Rule | Svrha | Kada se primjenjuje |
|------|-------|---------------------|
| **carpo-standards.mdc** | Hijerarhija izvora (XSD, FINAL-v2, Engine Rules), pravila za atribute, escapiranje, imenovanje | Uvijek (alwaysApply) |
| **carpo-nove-granule.mdc** | Kreiranje novih Carpo granula od nule | Kad korisnik traži novu formu/granulu (bez konverzije) |
| **konvertirane-granule.mdc** | Konverzija NetGen→Carpo, post-konverzijski review, sva pravila | Kad korisnik traži konverziju |
| **kompleksne-granule.mdc** | SP analiza u planu za Score > 70 | Kad radiš s granulama Score > 70 |
| **netgen-sql.mdc** | Referenca stored procedura (sql/{Client}/) | Kad treba pregledati SP strukturu |
| **run-testshell.mdc** | Pokretanje TestShell-a, logovi | Kad korisnik traži pokretanje ili provjeru logova |
| **carpo-novosti.mdc** | „Što ima novo u Carpu“ – sažetak iz **docs/Novosti/** | Kad korisnik traži novosti, promjene za korisnike/XML (ne C#) |

---

## Dokumentacija (docs/)

| Dokument | Svrha |
|----------|-------|
| **Carpo-XML-Rules-FINAL-v2.md** | Glavni Carpo XML standard – struktura, hijerarhija, imenovanje |
| **Carpo-XML-SuperCheatsheet.md** | Brza referenca |
| **Carpo-Engine-Master-Rules.md** | Ponašanje engine-a (runtime) |
| **docs/netgen/NetGen-to-Carpo-Conversion-Guide.md** | Vodič konverzije – mapiranja, pattern-i 1–18 |
| **docs/netgen/Conversion-Workflow-Checklist.md** | High-level workflow konverzije (Koraci 1–7) |
| **docs/netgen/Plan-Konverzije-Template.md** | Template za plan konverzije |
| **docs/netgen/NetGen-XML-Patterns-Examples.md** | Stvarni primjeri iz NetGen XML-ova |
| **docs/netgen/{Client}-Tree-SP-Inventory.md** | Inventar Tree SP za NetGen granule (spMPTreeNG, spKUPTreeQueryNG, itd.) |
| **docs/netgen/Carpo-Tree-SP-Inventory.md** | Inventar Tree SP za Carpo granule (iz definitions/carpo) |

### Novosti (prečica)

| Lokacija | Svrha |
|----------|-------|
| **docs/Novosti/README.md** | Uvod: kada agent koristi folder; popis materijala |
| **docs/Novosti/*.md** | Pregled promjena za korisnike, Info predložak, DevExpress grid (korisnički) — **ne** zamjenjuje FINAL-v2; trajna pravila idu u **Carpo-XML-Rules-FINAL-v2.md** |

---

## Folderi – uloga svakog

| Folder | Uloga |
|--------|-------|
| **definitions/carpo/** | Postojeće Carpo granule – **primjeri i pattern-i** za CarpoBot. Mogu biti zastarjeli. NE nadjačavaju XSD + FINAL-v2. **Nije izlaz** za konverzije. |
| **definitions/Prepisane_granule/** | NetGen + Carpo **parovi** (28 granula) – ground truth za konverziju. NetGen izvor + Carpo ekvivalent. Koristi se kao referenca pri konverziji. |
| **definitions/netgen/** | NetGen XML granule – **izvor** za konverziju. |
| **GeneriraneGranule/CarpoGranule/** | **Izlaz** za nove Carpo granule (kreirane od nule, bez konverzije). |
| **GeneriraneGranule/KonvertiraneGranule/{ImeGranule}/** | **Izlaz** za konvertirane NetGen→Carpo granule (vol1). Jedan subfolder po granuli. |
| **GeneriraneGranule/KonvertiraneGranule/Agronom/{ImeGranule}_Carpo/** | **Izlaz** za konvertirane Agronom granule (245). SP: sql/Agronom/. Tracking: Agronom-Konverzija-Status.md |
| **GeneriraneGranule/KonvertiraneGranule/vol2/{ImeGranule}_Carpo/** | **Izlaz** za konvertirane granule iz „biovega dodatne granule“ (52 granule). Jedan subfolder po granuli, ime sa sufiksom _Carpo. |
| **docs/Novosti/** | **Novosti za korisnike** (XML, ponašanje, bugfixevi) — pregled za iskusne korisnike; sinkronizirati s FINAL-v2 kad postane pravilo |

---

## Hijerarhija izvora (prioritet)

1. **Carpo.xsd** + **FINAL-v2** – šta smije postojati u XML-u
2. **Engine-Master-Rules** – kako atributi rade u runtime-u
3. **SuperCheatsheet** – brza referenca (ne nadjačava FINAL-v2)
4. **definitions/carpo** i **Prepisane_granule/Carpo** – primjeri (ne nadjačavaju standard)

---

## Kada šta koristiti

| Situacija | Šta koristiti |
|-----------|---------------|
| Kreiranje nove granule | carpo-nove-granule, carpo-standards, FINAL-v2, definitions/carpo |
| Konverzija NetGen→Carpo | konvertirane-granule, Conversion-Workflow-Checklist, NetGen-to-Carpo-Conversion-Guide |
| Agronom konverzija | konvertirane-granule (Agronom sekcija), Agronom-ExactMatch-Analiza.md, Agronom-Konverzija-Status.md |
| Score > 70 | kompleksne-granule, netgen-sql, Plan-Konverzije-Template (SP analiza) |
| Granula s tree + SP | netgen-sql, {Client}-Tree-SP-Inventory, Carpo-Tree-SP-Inventory |
| Pregled SP strukture | netgen-sql, sql/{Client}/StoredProcedures/ |
| TestShell, logovi | run-testshell |
| „Što ima novo u Carpu“, novosti za korisnike/XML | carpo-novosti, **docs/Novosti/**, zatim po potrebi FINAL-v2 |
