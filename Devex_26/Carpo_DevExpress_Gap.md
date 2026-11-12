# Carpo × DevExpress Gap (22→26) — Manager-first

**Status:** Done (Iter 4)  
**Rule:** opt-in XML / attribute, **default off**, no host/`Starter.Run` contract break  
**DX baseline:** already on **26.1** — gap = feature adoption, not version upgrade

---

## Explicitly do not touch

- Breaking XML semantics for existing granules  
- Mass async rewrite of Events pipeline  
- ThreadStatic / DI rewrite  
- VirtualServerMode as default for all grids (needs data-contract redesign)  
- New Designer controls until shortlist passes review (unless clear hole)

---

## Surface scan (priority order)

### 1. GridView / GridControl — highest ROI

| Feature | DX era (approx) | Carpo fit | Effort | Risk | Value | Notes |
|---------|-----------------|-----------|--------|------|-------|-------|
| Find Panel | long-standing; UX polished 22+ | XML `findpanel` | Low | Low | High | Unused today |
| Real async populate | — | existing `async` | Low | Med | High | Code exists, commented |
| FormatRules hygiene | — | always-on fix | Low | Low | High | Audit A6 |
| Export / Clipboard | used | — | — | — | — | Already wired |
| EditForm | 22+ | possible later | Med | Med | Med | Needs granule model |
| VirtualServerMode | 22+ | opt-in later | High | High | High | Defer past Iter6 |
| Column chooser persist | via layout | partial | Low | Low | Med | Layout already stores much |
| HTML templates | 23+ | careful | Med | High | Low | Skip for now |

### 2. TreeList

| Feature | Fit | Effort | Risk | Value |
|---------|-----|--------|------|-------|
| Find Panel | XML `findpanel` | Low | Low | Med-High |
| Filter modes | attr later | Low | Low | Med |
| Async node load | later | High | High | Med |

### 3. LayoutControl

Carpo already does manual VarsLayout — DX restore parity is incremental; **not** Iter6.

### 4. Editors / LookUp / TreeLookUp / AutoSuggest

| Feature | Fit | Notes |
|---------|-----|-------|
| SearchMode / PopupFilterMode | XML opt-in | Good follow-up |
| Async server lookup | Partial pattern in Info | Defer |
| TokenEdit | new XML element | Out until review |

### 5. Bars / Ribbon

Shared RibbonBase — SVG/search polish only if Carpo-specific; skip Iter6.

### 6. Chart / Pdf / DocumentViewer

Secondary — no Iter6 candidates.

---

## Shortlist B1…B5 (Iter 6 prototypes)

| ID | Candidate | Opt-in | Where | Demo |
|----|-----------|--------|-------|------|
| **B1** | Grid Find Panel | `findpanel="true"` | `ComponentGrid` + `DataControls` | Any Manager grid XML |
| **B2** | Tree Find Panel | `findpanel="true"` | `ComponentTree` + same attr | Manager filter tree / tree controls |
| **B3** | Grid async populate | existing `async="true"` | Re-enable `GetDataAsync(FocusType)` path | One heavy Manager grid |
| **B4** | Runtime `setattribute` findpanel | action attribute | `ComponentBase.SetAttribute` | Events without XML redeploy |
| **B5** | LookUp DX search + Tree smart filter | `dxsearchmode="autosearch"` / tree `filtermode="smart"` | `ComponentLookUpEdit` + `ComponentTree` | Lookup form / Manager filter tree |

### Deferred (good, not this round)

- B6 VirtualServerMode — **spike 2026-08-07: NO-GO** (vidi `Carpo_B6_VirtualServerMode_Spike.md`); samo poseban projekt uz SP paging  
- B7 EditForm — **Done** (wave 3)  
- B8 Tree async load  
- B9 TokenEdit control — **Done** (wave 6; `Carpo_B9_TokenEdit_Review.md`)

---

## XML author notes (after Iter 6)

```xml
<!-- Grid / Tree: show DevExpress Find Panel (default off) -->
<grid name="Master" findpanel="true" ... />
<tree name="Filter" findpanel="true" ... />

<!-- Grid: background SQL fetch (existing attribute; now wired again) -->
<grid name="Master" async="true" ... />

<!-- LookUp: DX GridLookUp SearchMode (optional; none|autocomplete|autosuggest|autosearch|autofilter) -->
<comboedit name="Partner" dxsearchmode="autosearch" ... />

<!-- Tree: DX FilterMode.Smart (optional) -->
<tree name="Filter" filtermode="smart" ... />
```

Runtime (events):

```xml
<action type="setattribute" value="#Master" name="findpanel">true</action>
```

---

## Iter 6 acceptance

- Default granules unchanged (no find panel, sync populate as today unless `async` already set).  
- One smoke path: Manager grid with `findpanel="true"`; optional `async="true"` on a non-critical granule.  
- Document stays in this file (no Designer control additions).

## Demo granula

`Demo\DxAudit26\DxAudit26_Demo.xml` (+ `README.md`) — Manager tree/grid/detail + editor lookup + transaction FAIL gumb.  
Pokrenuti u TestShell/Lite.
