# Devex_26 — integration / testno okruženje

**Datum:** 2026-08-10  
**Status:** Carpo DX opt-in wave **CLOSED** → ovaj doc = sljedeći korak  
**Deploy root:** `C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\`  
**Carpo / TestShell output:** `...\SharedAssemblies\Spin\`

---

## Cilj

Jedan **integracijski smoke** na Devex_26 artefaktima (shell + Carpo + Ribbon + Printing + Logger), ne novi Carpo feature-wave.

Dokaz koji već postoji: `Demo\DxAudit26\` u TestShellu.  
Dokaz koji treba: **host + SharedAssemblies + print + tipična granula** drže zajedno.

---

## Stack (šta spada u test env)

| Komponenta | Repo / grana | Artefakt |
|------------|--------------|----------|
| Shell | `JupiterSoftware\JupiterSoftware_devex26` | Jupiter client |
| Carpo | `Carpo\Devex_26` | `Spin\Carpo.*.dll`, `Carpo.TestShell.exe` |
| Ribbon | `RibbonBase\Devex_26` | `RibbonBase.v26.dll` |
| Print | `Spin.Printing\Devex_26` | `Spin.Printing.v26.dll` |
| Logger | `JupiterLogger` | `JupiterLogger.dll` |
| Chat / Notif | SpinChat / NotificationWidget | SharedAssemblies (periferija) |
| REPX | `Repx\devex26` | **Zaseban track** — vidi § REPX |

---

## Priprema (jednom / po buildu)

1. Build (Debug) redoslijedom: **JupiterLogger → RibbonBase → Spin.Printing → Carpo** (pa shell po potrebi).
2. Potvrdi da `...\SharedAssemblies\Spin\` ima svježe `Carpo.UI.dll`, `Carpo.Structure.dll`, `Carpo.TestShell.exe`, `RibbonBase.v26.dll`, `Spin.Printing.v26.dll`.
3. (Opcionalno) version stamp: zabilježi datum/changeset (npr. Carpo #5130+) u bilješku testa.
4. TestShell DB connection (ista baza gdje je B6 seed ako testiraš VSM).
5. Logovi: `...\SharedAssemblies\Spin\LogoviTestShella\` (i `...\Spin\Log\` po potrebi).

B6 SQL seed (jednom na demo DB):  
`Carpo\Devex_26\Demo\DxAudit26\Sql\B6_VirtualServerMode_Test.sql`

---

## Smoke checklist (minimalno)

Označi pri prolazu. Fail → zabilježi log + changeset.

### 1) Shell + Logger

- [ ] JupiterSoftware (Devex_26) startuje bez crasha
- [ ] Logovi se pišu (JupiterLogger / AppLogger)
- [ ] Nema očitog Ribbon StyleChanged flicker-a / cross-form leaka (fix #5108)

### 2) Carpo TestShell — DxAudit26

| # | Forma / XML | Provjera | OK |
|---|-------------|----------|----|
| 2a | `DxAudit26_Demo` | Open; Find; Refresh async (~1s); F2 EditForm | [ ] |
| 2b | `DxAudit26_UxPolish` | asynccaption splash; Find allow/always; emptymessage (gumb 3) | [ ] |
| 2c | `DxAudit26_BigGrid_Vsm` | Open/scroll (seed); Find; Refresh | [ ] |
| 2d | (opcionalno) `DxAudit26_BigGrid_Full` | Open sporiji od VSM — usporedba | [ ] |

XML path: `...\Carpo\Devex_26\Demo\DxAudit26\*.xml`

### 3) Print / Spin.Printing

- [ ] Print preview / print dialog s Carpo grida (npr. Demo `displaycontrolsprint` ili poznata granula)
- [ ] Nema load fail na `Spin.Printing.v26`
- [ ] (Ako ima REPX report u scenariju) preview bez crasha — detaljniji REPX = § ispod

### 4) Periferija (smoke, ne deep)

- [ ] SpinChat UI otvara se iz shell-a
- [ ] Notification widget otvara se / ne ruši shell

### 5) Realna granula (1–2 “known good”)

- [ ] _______________ (upiši ime) — open + osnovni refresh
- [ ] _______________ — print ili detail navigate

---

## Šta nije u ovom env-u

| Stavka | Gdje |
|--------|------|
| Novi Carpo DX featurei (AI, Events rewrite) | Parked / van scope |
| B6 produkcijska granula odjela | Buy-in + SP — kasnije |
| REPX corpus migracija | **Zaseban track** |

---

## REPX — zaseban track

**Ne miješati** s Carpo DX opt-in waveom.

- Repo: `Repx\devex26` (DX 26.1 već na kodu; report corpus / bindings / Expressions = open)
- Ulaz: `REPX\docs\Phase0-Inventory.md`, `REPX\ReportSmokeTest\`, `REPX\docs\Devex26_Track.md`
- U Carpo changelog parking lot: red **REPX**
- Integration smoke smije dirnuti **jedan** print/REPX happy-path; dubinska migracija reporta ≠ Carpo checklist

---

## Pass / fail bilješka

| Polje | Vrijednost |
|-------|------------|
| Datum | |
| Tester | |
| Carpo changeset | |
| SharedAssemblies stamp | |
| Rezultat | PASS / FAIL |
| Fail napomena / log | |

---

## Povezano

- [`Carpo_DX_Changelog.md`](Carpo_DX_Changelog.md) — wave CLOSED
- [`Carpo_DX_XmlAutor_Vodic.md`](Carpo_DX_XmlAutor_Vodic.md) — XML opt-in za odjele
- [`Carpo_B6_VirtualServerMode_Spike.md`](Carpo_B6_VirtualServerMode_Spike.md) — VSM SP / keš
- [`Demo\DxAudit26\README.md`](Demo/DxAudit26/README.md)
