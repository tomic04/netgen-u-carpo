# Carpo × DevExpress — changelog (živi dokument)

**Za XML autore:** [Carpo_DX_XmlAutor_Vodic.md](Carpo_DX_XmlAutor_Vodic.md) — opt-in atributi + XML snippeti (Devex_26).

**Ažurirati:** nakon svake iteracije / merge-a.  
**Demo granula:** [`Demo\DxAudit26\DxAudit26_Demo.xml`](Demo/DxAudit26/DxAudit26_Demo.xml)  
**Carpo DX opt-in wave (v1–v12):** **zatvoren** (2026-08-10). Next = **integration test env** — vidi [`Devex26_Integration_TestEnv.md`](Devex26_Integration_TestEnv.md).

---

## Status

| Wave | Scope | Status |
|------|--------|--------|
| **v1 / Iter 0–6** | Auditi + inventory + gap + P0 audit + B1–B5 DX opt-in | **Done** (2026-08-07) |
| **v2** | B11 FilterEditor + B10 LookUp + A8 SQL tax | **Done** (2026-08-07) |
| **v3** | B7 EditForm + A10 Parser paint | **Done** (2026-08-07) |
| **v4** | B12 Column Chooser + A9 foreachrow guidance | **Done** (2026-08-07) |
| **v5** | B6 VirtualServerMode spike + B10 AutoSuggest | **Done** (2026-08-07) |
| **v6** | B9 TokenEdit (novi XML element) | **Done** (2026-08-07) |
| **v7** | B8 Tree async populate (partial) | **Done** (2026-08-07) |
| **v8** | Fiscal/Outlook extract iz Events | **Done** (2026-08-10) |
| **v9** | HintPath / build ops (minimal) | **Done** (2026-08-10) |
| **v10** | B8 Tree lazy node load | **Done** (2026-08-10) |
| **v11 / B6** | VirtualServerMode opt-in Iter 0–3 (demo) | **Done** (2026-08-10) |
| **v12** | Sitni UX polish (Find allow/always, emptymessage, asynccaption) | **Done** (2026-08-10) |
| **CLOSED** | Carpo DX opt-in feature-wave (B1–B12 + B6 demo + UX) | **Done** — next: integration test env |

---

## Šta je novo (isporučeno) + primjer



### Wave 12 (2026-08-10)

| ID | Sta | Primjer | Demo |
|----|-----|---------|------|
| **UX** | Find `allow`/`always` + highlight/Clear; `emptymessage`; `asynccaption` | `findpanel="allow"` `emptymessage="..."` `asynccaption="..."` | DxAudit26 gumbi **2/2a/2b**; master async caption |
### Wave 11 / B6 Iter 0–3 (2026-08-10)

| ID | Sta | Primjer | Demo |
|----|-----|---------|------|
| **B6** | Opt-in `virtualservermode="true"` + `VirtualServerModeBinder` (paging SP); default DataTable path netaknut | `<grid virtualservermode="true" …/>` + `sp_DxAudit26_BigGrid_Page` | VSM vs FULL; SQL u `Demo\DxAudit26\Sql\`; **~2x brzi start** VSM; Iter4 print/export hydrate-on-demand |
### Wave 10 (2026-08-10)

| ID | Sta | Primjer | Demo |
|----|-----|---------|------|
| **B8** | Tree lazy="true" — children on expand via $TreeParentId + haschildren | <tree lazy="true" …/> + @ParentId= | Gumb **1c** / frmLazyTree; **1b** async nepromijenjen |
### Wave 9 (2026-08-10)

| ID | Sta | Primjer | Demo |
|----|-----|---------|------|
| **Ops** | HintPath cleanup: uklonjeni Desktop DX pathovi u Carpo.UI; Outlook → SharedAssemblies; TestShell SpinDMSUtil → Spin | Bez `C:\Users\...\Desktop\DevExpress.*` | N/A (build ops) |
### Wave 8 (2026-08-10)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **Audit** | Outlook / ZKI / JIR izvučeni iz `Events.RunActions` u `FiscalOutlookActions` | XML actioni `outlook` / `ZKI` / `JIR` — ista semantika; opt-in `demo="true"` dry-run | Gumbi **8** / **8b** (Info, bez cert/Outlook/SOAP) |
### Wave 7 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B8** | Tree `async="true"` — background SQL + loading panel (full populate) | `<tree … async="true" />` | Gumb **1b**; lazy: gumb **1c** |

### Wave 6 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B9** | DX `TokenEdit` — novi `<tokenedit>` | `<tokenedit … separator="," allowcustom="true" />` → EditValue CSV | Subforma **Tagovi** + gumb “Prikaži tagove”; review: `Carpo_B9_TokenEdit_Review.md` |

### Wave 5 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B6** | VirtualServerMode spike → tada **NO-GO** (historija); runtime kasnije u **v11** | vidi [`Carpo_B6_VirtualServerMode_Spike.md`](Carpo_B6_VirtualServerMode_Spike.md) | Wave 5: samo plan; v11 = demo opt-in |
| **B10** | LookUp `dxsearchmode="autosuggest"` + `$DxLookupSearch` re-query | `<comboedit dxsearchmode="autosuggest" …/>` + param `@q=$DxLookupSearch` | Subforma **Partner AutoSuggest** |

### Wave 4 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B12** | Column Chooser opt-in (search + quick hide) | `<grid … columnchooser="true" />` / runtime attribute | Gumb **6** otvara chooser |
| **A9** | Guidance + demo: prefer `#grid.selectedids` (1 SQL); warn log na `foreachrow`+`datarun` | `#grdMaster.selectedids` → jedan `datarun` | Gumb **7** vs **7b** (anti N+1). Semantika foreachrow **ne dirana**. |

### Wave 3 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B7** | Grid EditForm opt-in (F2; double-click ostaje Carpo) | `<grid … editform="true" />` (+ `editable` na field) | Master — gumb 3 / F2; modes: `editform`, `editforminplace`, `inplace` |
| **A10** | `CheckIf`: jedan `Parser` za L+R strane | (nema XML) | RowCellStyle / conditional styles — manje alloc |

### Wave 2 (2026-08-07)

| ID | Šta | Primjer | Demo |
|----|-----|---------|------|
| **B11** | Grid Filter Editor opt-in | `<grid … filtereditor="true" />` | Master — Filter Editor u filter UI/meniju |
| **B10** | LookUp `immediatepopup` + `nulltext` + AutoComplete usporedba | `dxsearchmode="autocomplete" immediatepopup="false"` | Subforma drugi Partner |
| **A8** | Text SQL: `SET ARITHABORT` u istom batchu; `CommandAsSql` samo kad LogLevel ≥ Info | (nema XML) | Niže loglevel → manje CPU na log SQL |

### DX / XML opt-in (Track B)

| ID | Šta | Primjer XML / akcija | Demo |
|----|-----|----------------------|------|
| **B1** | Grid Find Panel | `<grid … findpanel="true" />` | Master + detail u demo granuli |
| **B2** | Tree Find Panel | `<tree … findpanel="true" />` | Filter tree |
| **B3** | Async populate (pravi background) | `<grid … async="true" />` | Master + gumb “Refresh async” (~1s WAITFOR) |
| **B4** | Runtime Find Panel | `<action type="control attribute" value="#grdMaster"><parameter name="findpanel" value="true"/></action>` | Gumbi 2 / 2b; fix: `JTreeListControl.SetAttribute` → `ComponentExt` |
| **B5a** | LookUp AutoSearch | `<comboedit … dxsearchmode="autosearch" />` (aliases: `autofilter`, `autocomplete`, `autosuggest`, `none`) | Subforma Partner — tipkaj `om` → Omega |
| **B5b** | Tree FilterMode.Smart | `<tree … filtermode="smart" />` | Filter tree |

**Napomena:** default granules bez ovih atributa ponašaju se kao prije.

### Audit / stabilnost (Track A)

| ID | Šta | Primjer / kako provjeriti |
|----|-----|---------------------------|
| **A1** | `SqlContext.CreateCommand` ne dispose-a prije returna | Data sloj — nema XML |
| **A2** | `async="true"` → `Task.Factory.StartNew` + BeginUpdate | Demo Refresh async |
| **A3** | Transaction fail → log + `{transaction}` = `fail` | Demo gumbi Transaction OK / FAIL |
| **A4** | Password u logu → `***` | `Starter.RegisterForm` |
| **A5** | AppRole hardkod password uklonjen | `SqlHelper` |
| **A6** | `FormatRules.Clear` + Begin/EndUpdate | Demo master databar; refresh ne množi pravila |
| **A7** | Bez `GC.Collect` na FormClosing | `JupiterShared` |

---

## Demo granula — brzi smoke

1. TestShell → `Demo\DxAudit26\DxAudit26_Demo.xml` → forma `DxAudit26_Demo`
2. Find traka na tree/grid (ili gumb 2)
3. Refresh async (~1s loading)
4. Gumb **3) EditForm** → F2 na master redu (Naziv/Iznos editable); double-click i dalje otvara Lookup
5. “Lookup AutoSearch” → Partner tipkaj `om` / `al`
6. Transaction FAIL → poruka `fail` + Carpo log
7. Gumb **6** Column Chooser; označi redove → **7** batch `selectedids` (1 SQL) vs **7b** foreachrow N× SQL

---

## Backlog — još ima na spisku (nije potreban novi plan za start)

Nastavljamo kao **wave 3+** u ovom docu + dopuna `Carpo_DevExpress_Gap.md`. Novi formalni plan treba samo ako ide veliki komad (npr. VirtualServerMode kao default arhitektura, Events rewrite).

### Audit P1 (Track A) — pending

| ID | Tema | Priority | Napomena |
|----|------|----------|----------|
| **A8** | `SET ARITHABORT` + `CommandAsSql` tax po pozivu | P1 | **Done** (text inline + log gate) |
| **A9** | `foreachrow` N× Events / N+1 | P1 | **Partial** — guidance + warn + demo (`selectedids`); bez rewrite semantike |
| **A10** | Paint-path Parser alloc (RowCellStyle) | P1 | **Done** (jedan Parser L+R u `CheckIf`) |
| — | Fiscal/Outlook sync iz Events | P1 later | **Done** (extract → `FiscalOutlookActions`; no rewrite) |
| — | HintPath / build ops | P2 | **Partial** — Desktop DX + Outlook + SpinDMSUtil fixed; ProjectReference rewrite later |

### DX gap (Track B) — pending shortlist kandidati

| ID | Feature | Effort | Risk | Predloženi red |
|----|---------|--------|------|----------------|
| **B6** | VirtualServerMode (opt-in) | High | High | **Partial** — demo Iter 0–4 (paging + print hydrate); produkcija buy-in only; vidi spike |
| **B7** | Grid EditForm (opt-in) | Med | Med | **Done** (F2; `editform` attr) |
| **B8** | Tree async node load | High | High | **Done** — full populate async + lazy expand (lazy / $TreeParentId) |
| **B9** | TokenEdit (novi XML element) | Med | Med | **Done** (`<tokenedit>`; vidi review) |
| **B10** | LookUp async server / SearchMode širenje | Med | Med | **Done** (autosuggest + `$DxLookupSearch`); dalje samo po potrebi |
| **B11** | FilterEditor na gridu (opt-in) | Low | Low | **Done** |
| **B12** | Column chooser / layout parity polish | Low | Low | **Done** (`columnchooser` opt-in) |

### Explicit van scope (i dalje)

- Full Events/ComponentGrid split  
- .NET Core migracija  
- Masovna asyncifikacija cijelog pipelinea  
- Breaking XML semantika  

---

## Parking lot (ne zaboraviti)

| ID | Tema | Trigger za pokretanje |
|----|------|------------------------|
| **B6** | VirtualServerMode — **poseban projekt** | Pojavi se granulа/SP s 50k+ redova + paging ugovor (`@Skip/@Take/@Sort`); mini-plan: `Carpo_B6_VirtualServerMode_Spike.md` |
| **REPX** | Report corpus / bindings / Expressions — **zaseban track** (ne Carpo DX wave) | Trigger: print/REPX smoke ili Phase0 triage; vidi `Devex26_Integration_TestEnv.md` + Repx `docs/` |

Agent rule: `.cursor/rules/b6-virtualservermode-parking.mdc` — podsjetnik na kraju waveova / “što dalje”.

## Predloženi sljedeći korak

**Carpo DX opt-in wave CLOSED.** Next: **integration / test env** — [Devex26_Integration_TestEnv.md](Devex26_Integration_TestEnv.md) (shell + SharedAssemblies Spin + DxAudit26 smoke + print).  
B13 DX AI = parked. **B6 produkcija** samo uz odjel buy-in. **REPX** = zaseban track.

---

## Povijest dopuna

| Datum | Šta |
|-------|-----|
| 2026-08-07 | v1 zatvoren: A1–A7, B1–B5, demo granula, changelog kreiran |
| 2026-08-07 | wave 2: B11 FilterEditor, B10 LookUp dorada, A8 ARITHABORT/CommandAsSql |
| 2026-08-07 | wave 3: B7 EditForm (F2), A10 CheckIf Parser reuse |
| 2026-08-07 | wave 4: B12 columnchooser, A9 selectedids guidance + warn |
| 2026-08-07 | wave 5: B6 spike NO-GO, B10 AutoSuggest + `$DxLookupSearch` |
| 2026-08-07 | wave 6: B9 TokenEdit (`<tokenedit>`) |
| 2026-08-07 | wave 7: B8 tree async populate (partial) |
| 2026-08-10 | wave 8: Fiscal/Outlook extract → FiscalOutlookActions |
| 2026-08-10 | wave 9: HintPath cleanup (Desktop DX, Outlook, SpinDMSUtil) |
| 2026-08-10 | wave 10: B8 tree lazy=true + $TreeParentId |
| 2026-08-10 | wave 11 / B6: VirtualServerMode opt-in Iter 0–3 (demo 50k) |
| 2026-08-10 | wave 12: Find allow/always, emptymessage, asynccaption |
| 2026-08-10 | Carpo DX opt-in wave CLOSED; Devex26_Integration_TestEnv.md; REPX zaseban track |
