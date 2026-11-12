# Carpo Technical Audit — Performance, Architecture, Maintainability

| Meta | Value |
|------|--------|
| **Model** | Cursor Grok 4.5 (language model jointly trained/owned by SpaceXAI and Cursor) |
| **Document** | `Carpo_Audit_Cursor-Grok-4.5.md` |
| **Analysis date** | 2026-07-31 |
| **Repository** | `c:\TFS\Core Techonologies\Carpo\Devex_23_4.8_framework` |
| **Solution** | `Carpo.sln` |
| **Branch / commit** | Not available — workspace is TFS-bound (`Carpo.vssscc`); no usable git HEAD in this checkout |
| **Repository access** | Full local source tree (~532 `.cs` files, 12 projects) |
| **Not reviewed** | Stored procedure definitions (not in repo); Jupiter host source; production telemetry; SQL execution plans; runtime profiling dumps; live SharedAssemblies binary drift vs source |

**Scope constraint:** No application source was modified. This document is the only artifact created.

**Legend for findings:**
- **potvrđeno** — visible in source with cited path/method
- **visoka vjerovatnoća** — call graph + pattern strongly imply runtime cost; needs measurement
- **hipoteza** — plausible; must profile before investing

---

## 1. EXECUTIVE SUMMARY

1. **Runtime SQL hot path is `Carpo.Data.Common` + `Carpo.UI.Logic.Data.Core`, not `SqlHelper`.** UI granules almost always go through `Common.ExecuteQuery` / `ExecuteAzur*`. `SqlHelper` remains a large secondary surface (Specific, CMS, Designer, AppRole remnant).

2. **Every non-transactional query opens a connection and runs an extra `SET ARITHABORT ON` round-trip** before the real command (`Common.ExecuteQuery` ~L43–54). Under event storms this doubles network/SQL chatter.

3. **Grid “async” load is fake.** `ComponentGrid.GetDataAsync` uses `await Task.FromResult(GetDataSet(...))` so SQL still runs on the UI thread; the real `Task.Factory.StartNew` path is commented out at the call site.

4. **Manager open paints as a sequential cascade on the UI thread:** tree populate → `FocusedNodeChanged` → master grid SQL → focus → detail grid SQL. Expected 3+ round-trips before interactivity for typical Manager granules.

5. **`foreachrow` is a confirmed N× action interpreter:** each row calls nested `Events.RunActions`. Nested `datarun*` produces classic N+1 SQL on the UI thread.

6. **FormClosing filter persistence is N SQL calls:** `Manager_FormClosing` loops `settings` and calls `SaveSettings` per key.

7. **Row/cell style evaluation is a paint-time CPU bomb when styles/`if` exist:** `RowCellStyle` → `SetStylesHelper` → `Conditions.CheckCondition` → new `Statemants` + `Parser` per style per paint.

8. **Expression engine allocates heavily:** `Evaluate.GetSubStrings` constructs a new `Regex` on every call; `Parser`/`Evaluate` are created per operand across Events, Core, Conditions.

9. **XML deserialize rebuilds `XmlSerializer(typeof(JRoot))` and can recurse into DMS parts + `partsql` SQL during startup** — cold start cost is I/O + SQL + serializer codegen, not just file read.

10. **Security hygiene issues are confirmed in code:** password logged in `Starter.RegisterForm`; AppRole password `"Passw0rd!"` hardcoded in `SqlHelper`; SQL text with inlined parameter values logged via `Log.CommandAsSql`.

11. **Build graph is fragile:** most sibling Carpo projects use HintPath into Jupiter `SharedAssemblies` instead of `ProjectReference` (UI only ProjectReferences `Carpo.Data`). Debug/Release often output into Program Files; some DevExpress HintPaths point to a developer Desktop path.

12. **`Events` (~2k LOC) and `ComponentGrid` (~3.5k LOC) are God surfaces** mixing data, UI mutation, fiscalization, Outlook/COM, export, and transactions — primary maintainability and regression risk.

13. **`SqlContext.CreateCommand` disposes the `SqlCommand` before return** (`using var command` then `return command`) — confirmed correctness defect on Outlook/fiscal paths.

14. **Transaction rollback catch can NRE** if `BeginTransaction` fails (`transaction.Rollback()` without null check); non-tran SQL errors in `NotifyData` are often swallowed as soft failure.

15. **No rewrite / no .NET 6+ migration is required to improve the system.** Highest ROI is: baseline instrumentation (already partially via `Logger.InfoTest`/`InfoData`), real off-UI data load for grids, batch FormClosing saves, typed SQL parameters + drop per-call ARITHABORT, extract fiscal/Outlook from Events, fix build ProjectReferences for Debug.

---

## 2. MAPA SISTEMA

### 2.1 Projects and roles

| Project | Role | Typical consumers |
|---------|------|-------------------|
| **Carpo.Structure** | XML models, interfaces (`IJupiterStart`), shared DTOs | All |
| **Carpo.Common** | Helpers, translator, localization, shared UI helpers | UI, Core, Data |
| **Carpo.Data** | ADO.NET: `Common`, `SqlHelper`, `Connection`, `SqlContext`, Get/Set wrappers | UI, Core, Specific |
| **Carpo.Core** | Logging, print dialogs, pub vars | UI, Gen |
| **Carpo.UI** | WinForms/DevExpress framework: Starter, Forms, Components, Events, Parser, Serializer | Gen, Lite, host |
| **Carpo.Gen** (`Carpo.dll`) | `Start : Starter` — form factory entry for Jupiter | Host / Lite / TestShell |
| **Carpo.Specific** | Client-specific UC plugins loaded by reflection | Runtime via XML `customcontrol` |
| **Carpo.XmlGen** | XML generation / authoring helpers | Designer tooling |
| **Carpo.Designer** | Designer tooling | Dev |
| **Carpo.CMS** | CMS tooling | Dev |
| **Carpo.Lite** | Thin host for local run | Dev / smoke |
| **Carpo.TestShell** | Primary manual integration harness | QA / Dev |

**Dependency shape (intended):**

```
Structure ← Common ← Data ← Core ← UI ← Gen (Carpo.dll)
                              ↑
                         Specific (plugin, reflection)
Lite / TestShell → Gen + UI (+ XmlGen for TestShell)
```

**Actual build coupling:** most projects reference **prebuilt DLLs** under Jupiter SharedAssemblies via HintPath, not ProjectReference. Only notable ProjectReference found: `Carpo.UI` → `Carpo.Data`.

### 2.2 Entry points

| Entry | Path | Notes |
|-------|------|-------|
| Production host | Jupiter loads `Carpo.dll` (`Carpo.Gen.Start`) from SharedAssemblies | Host contract: `Starter.Run(...)` |
| `Starter.Run` | `Carpo.UI\Connector\Abstract\Starter.cs` L78–139 | Connection, XML, `RegisterForm` |
| Form factory | `Carpo.Gen\Start.cs` `RegisterForm` L15–44 | Manager / Standard / Splitter / Info / SideBar |
| Lite | `Carpo.Lite\Program.cs` → `Gen.Start.Run` | Dev smoke |
| TestShell | `Carpo.TestShell\Main.cs` → `Gen.Start.Run` | Manual harness |
| `IJupiterStart` | `Carpo.Structure\Interfaces\IJupiterStart.cs` | Declares `menuID` parameter; **no implementer in repo**; live API uses `MenuID` property + `Run` without menuID arg |

### 2.3 Primary data / control flow

```
Host / Lite / TestShell
  → Carpo.Gen.Start.Run (Starter.Run)
      → Constructor.SetConnectionString + ThreadStatic Connection.*
      → Serializer.DeserializeXml (DMS / file / parts / partsql)
      → SetInitForm → RegisterForm
          → Manager|Standard|... ctor
          → permissions / VarsLayout / form.Start / Show
  → Manager Load/Shown
      → Populate tree/grid
      → Events.CreateEvents (control wiring on Shown for Manager)
  → User action / focus
      → Events.RunNotification → RunActions → Checking
          → Core.SetData → Common.ExecuteQuery|Azur*
          → NotifyControl / SetDataSource / dialogs
```

### 2.4 Three representative end-to-end paths

**Flow A — Cold Manager open (tree + master + detail)**  
`Starter.Run` → XML deserialize → `Forms.Manager` ctor (settings SQL, permissions, `init` event) → `TreeWithGrids_Load` / `PopulateControls` → `ComponentTree.PopulateTreeList` → SQL → focus cascade → `ComponentGrid.Populate` → SQL → detail populate → SQL. All synchronous on UI thread.

**Flow B — Button / menu `datarun` + refresh**  
Control event (`ComponentBase`) → `Events.RunNotification` → `RunActions` → `CheckData` / `NotifyData` → `Core.SetData` → `Common` → nested `control populate` → grid rebind.

**Flow C — Fiscalization JIR / Outlook**  
Special-cased at top of `Events.RunActions` (before normal `Checking`): ad-hoc SQL via `SqlContext`, certificate load, SOAP via `Task.Run(...).GetAwaiter().GetResult()`, or Outlook COM via `MicrosoftAPI.AddContactToOutlook`. Blocks UI for the whole chain; Outlook path `return`s and skips remaining actions.

### 2.5 Largest / most complex surfaces (size alone ≠ bug)

| File | ~Size | Complexity driver |
|------|------:|-------------------|
| `Carpo.UI\Components\Base\ComponentGrid.cs` | 165 KB | Binding, layout, styles, export, foreachrow |
| `Carpo.Data\SqlHelper.cs` | 145 KB | Legacy DAAB; secondary path |
| `Carpo.UI\Logic\Events.cs` | 82 KB | Action interpreter God class |
| `Carpo.UI\Components\Base\ComponentBase.cs` | 76 KB | Control event wiring / focus |
| `Carpo.UI\Forms\Manager\Manager.cs` | 57 KB | Manager lifecycle + filter settings |
| `Carpo.Specific\UgovoriONajmu\UgovoriDetail.cs` | 97 KB | Client-specific UC |

---

## 3. TOP NALAZI

### FIND-001

- **ID:** FIND-001
- **Naziv:** Fake-async grid populate blocks UI thread
- **Kategorija:** UI odziv / runtime performanse
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs`
- **Klasa/metoda:** `Populate` (~L919–923), `GetDataAsync` (L1241–1261), `SetData` (L1264+)
- **Dokaz iz koda:** Call site uses `_ = GetDataAsync();` while true background overload is commented. `GetDataAsync` does `await Task.FromResult(dataDefinition.GetDataSet(...))` — SQL executes on caller (UI) thread. `BeginUpdate`/`EndUpdate` around load are commented out.
- **Problem:** Long SP + large DataSet freezes WinForms message pump during every populate/refresh/timer refresh.
- **Mogući produkcijski efekat:** “Aplikacija se smrzne” pri otvaranju granule / promjeni fokuseda / auto-refresh.
- **Predložena promjena:** Restore background load carefully: `Task.Run`/`Task.Factory.StartNew` for `GetDataSet` only; marshal `SetData` via `Control.BeginInvoke`; copy ThreadStatic connection string into local before leaving UI thread; keep synchronous path as XML/feature flag fallback. Re-enable `BeginUpdate`/`EndUpdate` around bind.
- **Rizik promjene:** Srednji — ThreadStatic `Connection.*` / `PubVars` not visible on pool threads; focus/event ordering may change; must not re-enter Events from background.
- **Procjena rada:** M (2–4 dana) + regression matrix
- **Način testiranja:** TestShell Manager granule with large grid; verify UI remains paintable during SP delay (inject WAITFOR in test SP); focus/master-detail still fire once.
- **Mjerljivi kriterijum uspjeha:** UI freeze during populate drops toward SQL wait only on background; `InfoData` timestamps show gap between SQL complete and bind; no lost focus events.
- **Zavisnosti od drugih promjena:** FIND-015 (instrumentation); understand ThreadStatic before enabling.

### FIND-002

- **ID:** FIND-002
- **Naziv:** Per-query `SET ARITHABORT ON` + new connection outside transactions
- **Kategorija:** SQL performanse
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Data\Common.cs`
- **Klasa/metoda:** `ExecuteQuery` L43–54; `ExecuteAzur` L155–168
- **Dokaz iz koda:** After `Open()`, separate `SqlCommand("SET ARITHABORT ON")` always executes before the real command. Non-tran path creates new `SqlConnection` per call.
- **Problem:** Extra round-trip on every logical SQL; pool churn when Events fire many data actions without transaction.
- **Mogući produkcijski efekat:** Higher latency on chatty granules; amplified under VPN/WAN.
- **Predložena promjena:** Prefer connection-string / server default / open-once session setting; or execute ARITHABORT only when connection freshly opened and not already set. Optionally short-lived shared connection for sequential multi-data actions in one event (without changing transaction semantics).
- **Rizik promjene:** Mali–srednji — verify ARITHABORT-dependent plans still correct (some legacy SPs assume it).
- **Procjena rada:** S–M
- **Način testiranja:** Extended Events: ratio of `SET ARITHABORT` batches to RPC calls before/after; smoke critical granules.
- **Mjerljivi kriterijum uspjeha:** ARITHABORT batches ≈ 0 (or 1 per physical connection), not 1 per SP; wall time of multi-action events improves.
- **Zavisnosti od drugih promjena:** None for ARITHABORT removal; batching couples with Events transaction design.

### FIND-003

- **ID:** FIND-003
- **Naziv:** Full `DataSet` materialization for every reader
- **Kategorija:** Memorija / SQL
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Data\Common.cs`; `Carpo.UI\Logic\Data\Core.cs`
- **Klasa/metoda:** `ExecuteQuery` L98–103 (`SqlDataAdapter.Fill`); `Core.SetData` reader branch L141–152; grid binds `ds.Tables[0]` (`ComponentGrid.SetData` L1302)
- **Dokaz iz koda:** Default path always allocates `DataSet` even when UI uses one table; scalar/nonquery exist but grids use reader→DataSet.
- **Problem:** Schema + all rows + unused columns inflate managed heap; GC pauses on large lists.
- **Mogući produkcijski efekat:** High private bytes on heavy Manager screens; Gen2 GC after repeated refresh.
- **Predložena promjena:** Keep DataSet for multi-table SPs; add DataTable-only / reader path for single-table populate; push column pruning to SP/XML where possible. **Do not** enable DevExpress Server Mode without feature audit (grouping, foreachrow, styles, layout, export).
- **Rizik promjene:** Srednji for API shape; Server Mode = visoki rizik (incompatible with many Carpo features).
- **Procjena rada:** M for DataTable path; XL for Server Mode (defer)
- **Način testiranja:** Compare row counts, column counts, heap after populate; functional parity on templates/layout/styles.
- **Mjerljivi kriterijum uspjeha:** Lower allocated bytes per populate for same row count; no functional regression.
- **Zavisnosti od drugih promjena:** FIND-001 bind path; SP contracts for paging if pursued later.

### FIND-004

- **ID:** FIND-004
- **Naziv:** Untyped SQL parameters (`ToConvertSqlParams` / `AddWithValue`)
- **Kategorija:** SQL performanse
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Data\Common.cs` L282–288; `Carpo.Data\Data\Context\SqlContext.cs` L137; `Carpo.Data\Set\Vars.cs` AddWithValue sites
- **Klasa/metoda:** `Common.ToConvertSqlParams`; `SqlContext.CreateCommand`
- **Dokaz iz koda:** Parameters set name+value only — no `SqlDbType`/`Size`. Output params partially typed; inputs not.
- **Problem:** Plan-cache pollution; implicit conversions; possible index seek failures (nvarchar vs varchar).
- **Mogući produkcijski efekat:** Inconsistent SP duration; high CPU on SQL Server for hot procedures.
- **Predložena promjena:** Map XML parameter types (already parsed in `Core.ParseParameter` / Conversion) to `SqlDbType` at bind time; fix SqlContext similarly.
- **Rizik promjene:** Srednji — wrong type mapping can break SPs that relied on AddWithValue inference.
- **Procjena rada:** M
- **Način testiranja:** Characterization tests for type mapping; compare plans for top 10 SPs; TestShell smoke.
- **Mjerljivi kriterijum uspjeha:** Fewer duplicate plans; stable/improved SP duration for typed hot paths.
- **Zavisnosti od drugih promjena:** Need inventory of XML `type` attributes on parameters.

### FIND-005

- **ID:** FIND-005
- **Naziv:** `foreachrow` nested Events = N+1 SQL/actions on UI thread
- **Kategorija:** SQL / UI odziv
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno (pattern); produkcijski uticaj ovisi o granule XML
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs`; `Carpo.UI\Logic\Events.cs`
- **Klasa/metoda:** `SetNotifyForEachRow` L2505–2547; `RowForEachRowAction`; `NotifyGridControlForEachRow` (~Events L1964+)
- **Dokaz iz koda:** Loop over all/selected rows; per row sets `DataRowRef` and runs nested action list via Events.
- **Problem:** Linear growth of SQL/COM/UI work with row count; freezes UI.
- **Mogući produkcijski efekat:** Multi-second freezes on “obradi označene” actions; deadlocks under long transactions wrapping foreachrow.
- **Predložena promjena:** (1) Document XML guidance: prefer single TVP/`tablepar` datarun over foreachrow+datarun. (2) Optional framework batch helper that collects keys then one SP. (3) Keep foreachrow for pure UI-only nested actions.
- **Rizik promjene:** Mali for documentation/new action type; srednji if changing existing foreachrow semantics.
- **Procjena rada:** S (docs + measurement) / M (batch helper)
- **Način testiranja:** Count `InfoData` SQL lines vs selected row count; synthetic 10/100/1000 rows.
- **Mjerljivi kriterijum uspjeha:** SQL calls per action ≈ O(1) for batchable ops.
- **Zavisnosti od drugih promjena:** SP support for TVP/table-valued params (already partially supported via DataTable params in Common).

### FIND-006

- **ID:** FIND-006
- **Naziv:** Manager FormClosing N× `SaveSettings` SQL
- **Kategorija:** SQL / UI odziv
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Forms\Manager\Manager.cs`
- **Klasa/metoda:** `Manager_FormClosing` L1220–1228
- **Dokaz iz koda:** `foreach (var item in settings) service.SaveSettings(...)`.
- **Problem:** Closing a Manager with many tree nodes/layouts triggers N stored-procedure round-trips on UI thread.
- **Mogući produkcijski efekat:** Slow form close; user perceives hang when exiting granule.
- **Predložena promjena:** Batch save (single SP with TVP/XML, or one transactional multi-row write); keep in-memory debounce for column resize (already snapshots frequently — also candidate to throttle).
- **Rizik promjene:** Srednji — must preserve per-node layout correctness and AppData fallback path (L1230–1231).
- **Procjena rada:** M (needs SP change or carefully batched calls)
- **Način testiranja:** Count `spCarpoFilterPostavke` (or equivalent) on close before/after; verify layouts restore.
- **Mjerljivi kriterijum uspjeha:** 1 (or small constant) save round-trip on close; layouts identical.
- **Zavisnosti od drugih promjena:** SettingsService / SP contract; FIND-015 baseline.

### FIND-007

- **ID:** FIND-007
- **Naziv:** Paint-time style/`if` evaluation via Parser
- **Kategorija:** UI odziv / CPU
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno (cost); magnitude needs profiling
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs`; `Carpo.UI\Logic\Conditions.cs`; `Carpo.UI\Logic\Statemants.cs`
- **Klasa/metoda:** `RowCellStyle` / `SetStylesHelper` L652–662; `Conditions.CheckCondition` L31–51; Statemants creates `new Parser` per operand (~L76–94)
- **Dokaz iz koda:** For each style in `definition.Styles.StyleList`, condition check runs during style events; Parser allocated inside statements evaluation.
- **Problem:** O(visible cells × styles × expression complexity) on scroll/paint.
- **Mogući produkcijski efekat:** Janky scrolling on styled grids; high CPU even after data loaded.
- **Predložena promjena:** Precompute style predicates per row on data bind / row change into a cache dictionary; invalidate on DataSource change; avoid Parser in paint path. Fallback: FormatConditions native DevExpress where expressions are simple.
- **Rizik promjene:** Srednji–visoki — visual parity with XML `if` language must be preserved.
- **Procjena rada:** L
- **Način testiranja:** PerfView/CPU sampling while scrolling styled vs unstyled grid; characterization tests for style expressions.
- **Mjerljivi kriterijum uspjeha:** Paint handlers no longer dominate CPU; scroll FPS/subjective smoothness improved without style change.
- **Zavisnosti od drugih promjena:** FIND-008 (Parser/Regex cache helps but is not enough alone).

### FIND-008

- **ID:** FIND-008
- **Naziv:** Uncached Regex and heavy Evaluate/Parser allocations
- **Kategorija:** Runtime CPU / alokacije
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Evaluate.cs` L182–188; `Carpo.UI\Logic\Parser.cs` (~Regex.IsMatch); widespread `new Parser`/`new Evaluate` in Events/Core/Statemants
- **Klasa/metoda:** `Evaluate.GetSubStrings`; `Parser.GetLogicPartValue`; `Core.SetData` L135–136; `Core` ParseParameter `using (Parser ...)`
- **Dokaz iz koda:** `new Regex(start + "(.*?)" + end)` every `GetSubStrings` call; many call sites allocate Parser per parameter/action.
- **Problem:** Hot-path allocation and JIT/regex compile cost during every data action and many UI evaluations.
- **Mogući produkcijski efekat:** Extra ms per action; Gen0 churn under chatty events.
- **Predložena promjena:** Static compiled Regex for brace extraction and `^(@|#)`; reuse Parser instance per `SetData` / per `RunActions` batch; optional expression result cache keyed by (expression, form-instance generation, dependency stamp) with hard size cap + clear on form close.
- **Rizik promjene:** Mali for static Regex; srednji for caching (stale values if invalidation wrong).
- **Procjena rada:** S–M
- **Način testiranja:** Unit/characterization on Evaluate/Parser; allocate counters via ETW before/after.
- **Mjerljivi kriterijum uspjeha:** Fewer Regex ctor / Parser allocations per action; identical evaluation results on fixture expressions.
- **Zavisnosti od drugih promjena:** None for Regex; cache needs form lifecycle hooks.

### FIND-009

- **ID:** FIND-009
- **Naziv:** XML serializer + recursive parts/`partsql` cost at startup
- **Kategorija:** Startup / I/O / SQL
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Serializer.cs`; `Starter.DeserializeXml`
- **Klasa/metoda:** `DeserializeXml` L99–107 (`new XmlSerializer(typeof(JRoot))`); `GetParts` / `GetPartsSql` recursive merge
- **Dokaz iz koda:** XmlSerializer created on deserialize; parts recursion loads nested files; partsql can execute SQL and parse XML fragments during startup.
- **Problem:** Cold start pays serializer generation + disk + optional SQL before UI shows.
- **Mogući produkcijski efekat:** Slow first open of complex modules with many parts.
- **Predložena promjena:** Cache static `XmlSerializer` for `JRoot` (and other fixed types); cache merged document by (path, lastWriteTimeUtc, content hash) with memory cap; avoid double work in GetPartsSql; clear cache on module reload.
- **Rizik promjene:** Mali for XmlSerializer static; srednji for document cache (stale XML if file changes undetected).
- **Procjena rada:** S–M
- **Način testiranja:** `Logger.InfoTest` Init→DeserializeXml→RegisterForm timings cold/warm; mutate XML mtime to prove invalidation.
- **Mjerljivi kriterijum uspjeha:** Warm deserialize significantly faster; cold not worse; correct XML after edit.
- **Zavisnosti od drugih promjena:** FIND-015.

### FIND-010

- **ID:** FIND-010
- **Naziv:** Credentials and SQL parameter values logged / retained
- **Kategorija:** Pouzdanost / security hygiene
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Connector\Abstract\Starter.cs` L295–299; `Carpo.Data\Common.cs` L90; `Carpo.Data\Connection.cs` ThreadStatic Pwd; `Carpo.Data\SqlHelper.cs` AppRole L2525–2542 (`"Passw0rd!"`); parameter cache keys include connection string
- **Klasa/metoda:** `Starter.RegisterForm` logging; `Log.CommandAsSql`; `SqlHelper.AppRole.ExecuteEnableAppRole`
- **Dokaz iz koda:** Password written to Info log; CommandAsSql inlines values; hardcoded AppRole password in source (call sites commented but method remains).
- **Problem:** Secrets in logs, binaries, process memory, possibly CLI (`ComDll` pattern reported for Pwd).
- **Mogući produkcijski efekat:** Compliance risk; credential leak via log files / support bundles.
- **Predložena promjena:** Redact password from logs immediately; log SP name + param names/types by default; full SQL behind explicit debug flag; remove or externalize AppRole secret; cache keys without password; never pass Pwd on process command lines.
- **Rizik promjene:** Mali for redaction; verify support workflows that relied on full SQL logs.
- **Procjena rada:** S
- **Način testiranja:** Grep logs for password after Run; secret scan on binaries.
- **Mjerljivi kriterijum uspjeha:** No plaintext password in default logs; AppRole secret not in source.
- **Zavisnosti od drugih promjena:** None.

### FIND-011

- **ID:** FIND-011
- **Naziv:** `SqlContext.CreateCommand` returns disposed command
- **Kategorija:** Pouzdanost / correctness
- **Prioritet:** P0
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.Data\Data\Context\SqlContext.cs`
- **Klasa/metoda:** `CreateCommand` L128–141
- **Dokaz iz koda:** `using var command = new SqlCommand(...); ... return command;` — disposed at method exit.
- **Problem:** Outlook/fiscal/`SettingsService` paths may use disposed command — undefined/flaky behavior.
- **Mogući produkcijski efekat:** Intermittent `ObjectDisposedException` or silent failures on Settings/Outlook/fiscal.
- **Predložena promjena:** Remove `using` from factory; dispose at caller; ensure readers disposed.
- **Rizik promjene:** Mali if callers already dispose; verify all call sites.
- **Procjena rada:** S (&lt;1 day)
- **Način testiranja:** Unit/integration around SettingsService + Outlook path with TestShell.
- **Mjerljivi kriterijum uspjeha:** No dispose-related failures; settings load/save stable.
- **Zavisnosti od drugih promjena:** None — do first.

### FIND-012

- **ID:** FIND-012
- **Naziv:** Transaction catch rollback NRE + swallowed failures
- **Kategorija:** Pouzdanost
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Events.cs`; `Carpo.UI\Logic\Data\Core.cs`
- **Klasa/metoda:** `CheckTransaction` L1496–1518; `NotifyData` / `Core.SetData` error paths
- **Dokaz iz koda:** `catch { transaction.Rollback(); ...}` without null check; sets variables null; no rethrow. Non-tran path often returns null/false after log.
- **Problem:** Failed BeginTransaction → secondary exception; UI may continue after silent rollback.
- **Mogući produkcijski efekat:** Partial state, confusing UX, hard-to-diagnose “nothing happened”.
- **Predložena promjena:** `transaction?.Rollback()`; set `LastTransactionSuccess` failure; rethrow or surface error consistently; don’t clear variables without signaling failure.
- **Rizik promjene:** Mali–srednji — some granules may rely on soft-fail behavior.
- **Procjena rada:** S
- **Način testiranja:** Force connection failure / SP error inside transaction action; assert UX + flag.
- **Mjerljivi kriterijum uspjeha:** No NRE; user always sees failure; success flag accurate.
- **Zavisnosti od drugih promjena:** None.

### FIND-013

- **ID:** FIND-013
- **Naziv:** Fiscalization / Outlook embedded in Events dispatcher (sync-over-async + COM)
- **Kategorija:** Arhitektura / UI odziv
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Events.cs`
- **Klasa/metoda:** `RunActions` L278–410 (outlook/ZKI/JIR)
- **Dokaz iz koda:** Hard-coded SQL text to `mpfiskalizacija`; cert load; JIR uses `Task.Run(...).GetAwaiter().GetResult()`; Outlook COM then `return` (skips remaining actions); JIR cert not in `using` (ZKI is).
- **Problem:** Blocks UI; mixes cross-cutting infra into action interpreter; COM lifetime unclear (`Marshal.ReleaseComObject` not found in repo); Outlook early return is surprising semantics.
- **Mogući produkcijski efekat:** Long freezes during fiscal ops; Outlook action silently skips subsequent XML actions.
- **Predložena promjena:** Extract `IFiscalizationService` / `IOutlookContactService`; keep thin action adapters; dispose certs; replace sync-over-async with background + UI continuation (no deadlock-prone `.Result` on UI if sync context captured — prefer explicit thread + `BeginInvoke`). Cache certificate metadata with invalidation.
- **Rizik promjene:** Visoki for behavior changes; medium for extract-without-behavior-change.
- **Procjena rada:** L
- **Način testiranja:** Dedicated fiscal/Outlook checklist on machines with certs/Outlook; verify action ordering after Outlook.
- **Mjerljivi kriterijum uspjeha:** UI remains responsive during SOAP wait; certs disposed; documented Outlook `return` behavior preserved or consciously changed with XML migration notes.
- **Zavisnosti od drugih promjena:** FIND-011 SqlContext; host COM apartment rules.

### FIND-014

- **ID:** FIND-014
- **Naziv:** HintPath DLL graph + Program Files OutputPath + machine-specific DevExpress paths
- **Kategorija:** Build / razvoj
- **Prioritet:** P1
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** Multiple `*.csproj` (e.g. `Carpo.UI.csproj` HintPaths L89–97, DevExpress Desktop paths L111/L140, OutputPath to SharedAssemblies; Release x86 often `bin\x86\Release`)
- **Klasa/metoda:** n/a (project system)
- **Dokaz iz koda:** UI ProjectReferences only `Carpo.Data`; Common/Core/Structure consumed as installed DLLs; OutputPath targets Jupiter SharedAssemblies / absolute Program Files paths.
- **Problem:** Build against stale SharedAssemblies while editing source; non-reproducible builds across machines; Desktop HintPaths break CI/other developers.
- **Mogući produkcijski efekat:** “Fixed in source but not in running Jupiter”; long debug loops; broken builds on clean machines.
- **Predložena promjena:** Debug configurations: ProjectReference among Carpo.* + local `bin\`; keep deploy/copy step to SharedAssemblies as explicit post-build or distribution script; fix DevExpress refs to packages/`Lib`; document x86 Release as production config.
- **Rizik promjene:** Srednji — assembly binding / copy-local / Jupiter load order.
- **Procjena rada:** M
- **Način testiranja:** Clean machine build; verify versions in output match AssemblyInfo; TestShell against local bin.
- **Mjerljivi kriterijum uspjeha:** One-click Debug build without Program Files write; no Desktop HintPaths; source edits visible without manual DLL copy gymnastics.
- **Zavisnosti od drugih promjena:** Distribution docs (`upute za distribuciju.txt`); versioning policy (Jupiter 8 vs 10 third version digit).

### FIND-015

- **ID:** FIND-015
- **Naziv:** Incomplete baseline instrumentation for performance decisions
- **Kategorija:** Mjerenje
- **Prioritet:** P0 (process)
- **Pouzdanost:** potvrđeno (hooks exist); gaps confirmed
- **Projekat i fajl:** `Starter.Run` InfoTest; `Core.SetData` InfoData; Manager/Grid partial timings
- **Klasa/metoda:** `Logger.InfoTest` / `InfoData`
- **Dokaz iz koda:** Good span hooks exist around Run/RegisterForm/SQL, but missing standardized metrics: SQL count per user action, UI freeze duration, param-parse vs execute split, form/control counts after close, connection pool counters.
- **Problem:** Without baseline, refactors cannot prove benefit; risk of optimizing wrong layer.
- **Mogući produkcijski efekat:** Wasted engineering effort; regressions unnoticed.
- **Predložena promjena:** Add lightweight `PerfScope` (Stopwatch + counters) behind existing Logger levels; correlation id per RunNotification; document TestShell scenarios. See section “Mjerenje” below.
- **Rizik promjene:** Mali if logging level gated.
- **Procjena rada:** S–M
- **Način testiranja:** Run 5 scenarios; export CSV/log; compare before/after any change.
- **Mjerljivi kriterijum uspjeha:** Repeatable baseline pack checked into docs (this file’s companion logs).
- **Zavisnosti od drugih promjena:** Prerequisite for all performance PRs.

### FIND-016

- **ID:** FIND-016
- **Naziv:** Events God class + Manager dual ownership of lifecycle
- **Kategorija:** Arhitektura / održavanje
- **Prioritet:** P2 (start extract after safety nets)
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Events.cs`; `Carpo.UI\Forms\Manager\Manager.cs`
- **Klasa/metoda:** `RunActions` / `Checking`; Manager ctor/Load/FormClosing
- **Dokaz iz koda:** Single class handles data, forms, controls, fiscal, Outlook, export, print, panels, tabs, transactions, layout; Manager mixes settings persistence, filters, tabs, tree/grid wiring (~1440 LOC).
- **Problem:** High change risk; untestable branches; every fix can break unrelated granules.
- **Mogući produkcijski efekat:** Long regression cycles; fear-driven freezes on improvements.
- **Predložena promjena:** Incremental extract behind facades without changing XML:
  1. `DataActionExecutor` (from CheckData/NotifyData/CheckTransaction)
  2. `ControlActionExecutor`
  3. `FormActionExecutor`
  4. `ExternalIntegrationActions` (fiscal/Outlook)
  Keep `Events` as thin router preserving public methods used by ComponentBase.
- **Rizik promjene:** Visoki if big-bang; mali per vertical extract with characterization tests.
- **Procjena rada:** XL overall; S–M per extract PR
- **Način testiranja:** Characterization tests per action type; TestShell smoke matrix.
- **Mjerljivi kriterijum uspjeha:** Events.cs LOC decreases; action types covered by focused tests; no XML changes required.
- **Zavisnosti od drugih promjena:** FIND-015; FIND-013 extract is first vertical.

### FIND-017

- **ID:** FIND-017
- **Naziv:** Filter-mode FormClosing early `return` skips layout persistence
- **Kategorija:** Pouzdanost / correctness
- **Prioritet:** P2
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Forms\Manager\Manager.cs`
- **Klasa/metoda:** `Manager_FormClosing` L1241–1244
- **Dokaz iz koda:** When filter edit value is `"Filter"`, saves one layout then `return` — skips master/detail/splitter persistence below.
- **Problem:** Incomplete save path depending on UI mode.
- **Mogući produkcijski efekat:** Lost splitter/grid layouts for users working in Filter mode.
- **Predložena promjena:** Save filter-specific layout then continue with shared cleanup/splitter saves as applicable.
- **Rizik promjene:** Srednji — need product confirmation of intended behavior.
- **Procjena rada:** S
- **Način testiranja:** Manual: Filter mode → rearrange → close → reopen.
- **Mjerljivi kriterijum uspjeha:** Layouts persist in Filter and non-Filter modes per product rules.
- **Zavisnosti od drugih promjena:** Product owner clarification.

### FIND-018

- **ID:** FIND-018
- **Naziv:** `Thread.Sleep` on UI thread in wait progress
- **Kategorija:** UI odziv
- **Prioritet:** P2
- **Pouzdanost:** potvrđeno
- **Projekat i fajl:** `Carpo.UI\Logic\Events.cs`
- **Klasa/metoda:** `WaitFormRun` L200–201
- **Dokaz iz koda:** `System.Threading.Thread.Sleep((int)sleep)` while splash wait form shown.
- **Problem:** Artificial UI freeze; message pump starved (splash may not paint smoothly).
- **Mogući produkcijski efekat:** Pointless freezes if XML configures sleep.
- **Predložena promjena:** Remove sleep or replace with async delay that pumps messages only if product requires minimum display time.
- **Rizik promjene:** Mali if sleep unused in prod XML; verify granules using wait sleep attribute.
- **Procjena rada:** S
- **Način testiranja:** Grep XML for wait sleep usage; UI freeze measurement.
- **Mjerljivi kriterijum uspjeha:** No Sleep on UI thread in default paths.
- **Zavisnosti od drugih promjena:** XML inventory.

### FIND-019

- **ID:** FIND-019
- **Naziv:** Grid event handlers mostly never unsubscribed; Dispose gaps
- **Kategorija:** Memorija
- **Prioritet:** P2
- **Pouzdanost:** potvrđeno (handlers); leak impact = hipoteza
- **Projekat i fajl:** `Carpo.UI\Components\Base\ComponentGrid.cs` `SetEvents` (~L289–548); `Events.Dispose` stub
- **Klasa/metoda:** anonymous `+=` handlers; limited `-=`
- **Dokaz iz koda:** Many anonymous subscriptions; cleanup mainly timer/Disposed; `Events.Dispose` is effectively empty.
- **Problem:** If controls/forms are recreated without full Dispose chain, handlers keep graphs alive.
- **Mogući produkcijski efekat:** Growing private bytes after open/close cycles in long Jupiter sessions.
- **Predložena promjena:** Named handlers + unsubscribe in Dispose; verify form close releases grids; add weak test: open/close N times, assert handle/control counts.
- **Rizik promjene:** Srednji — easy to break events if unsubscribe wrong.
- **Procjena rada:** M
- **Način testiranja:** WinForms handle counts / `!form.IsDisposed` tracking after close; SOS/`!gcroot` on suspect.
- **Mjerljivi kriterijum uspjeha:** Stable handle count after 50 open/close cycles.
- **Zavisnosti od drugih promjena:** FIND-015 memory metrics.

### FIND-020

- **ID:** FIND-020
- **Naziv:** Carpo.Specific duplication and large UCs on reflection load path
- **Kategorija:** Arhitektura / održavanje
- **Prioritet:** P3
- **Pouzdanost:** potvrđeno (structure); perf impact = hipoteza
- **Projekat i fajl:** `Carpo.Specific\**`; `Serializer.GetCustomControls`; `JCustomUC` Assembly.Load
- **Klasa/metoda:** Parallel Search/Jupiter shells; large UCs (Ugovori, Virmani, Kalkulacija)
- **Dokaz iz koda:** Specific not ProjectReferenced by UI; loaded via XML customcontrol reflection; duplicate Search form patterns vs UI.
- **Problem:** Framework fixes don’t flow to Specific copies; large UCs can dominate module cost when loaded.
- **Mogući produkcijski efekat:** Inconsistent UX; heavy modules slow only when custom controls present.
- **Predložena promjena:** Stop duplicating Search/grid helpers; extract shared services; keep client logic in Specific; lazy-load assemblies only when control needed (verify current behavior).
- **Rizik promjene:** Srednji per UC
- **Procjena rada:** L overall
- **Način testiranja:** Module-specific checklists; ensure Assembly.Load count.
- **Mjerljivi kriterijum uspjeha:** Fewer duplicated forms; Specific UCs call shared helpers.
- **Zavisnosti od drugih promjena:** Framework stability first.

---

## 4. QUICK WINS

### &lt; 1 day

| Win | Action | Risk | Measurable result |
|-----|--------|------|-------------------|
| QW-1 | Fix `SqlContext.CreateCommand` dispose bug (FIND-011) | Mali | Settings/Outlook/fiscal stop using disposed command |
| QW-2 | Redact password from `Starter` Info log (FIND-010) | Mali | Logs without plaintext password |
| QW-3 | `transaction?.Rollback()` + failure flag (FIND-012 partial) | Mali | No NRE on failed BeginTransaction |
| QW-4 | Static compiled Regex in `Evaluate.GetSubStrings` + Parser `^(@|#)` (FIND-008 partial) | Mali | Fewer Regex allocations (ETW/alloc profiler) |
| QW-5 | Static cache `XmlSerializer(typeof(JRoot))` (FIND-009 partial) | Mali | Faster warm deserialize |

### 1–2 days

| Win | Action | Risk | Measurable result |
|-----|--------|------|-------------------|
| QW-6 | Stop per-call `SET ARITHABORT ON` or gate it (FIND-002) | Mali–srednji | Fewer batches in XEvents |
| QW-7 | Log SP+param names by default; full SQL opt-in (FIND-010) | Mali | Smaller/safer logs |
| QW-8 | Remove/hard-disable AppRole hardcoded password method (FIND-010) | Mali | Secret gone from binary |
| QW-9 | Baseline PerfScope + TestShell scenario script (FIND-015) | Mali | Repeatable before/after CSV |
| QW-10 | Re-enable BeginUpdate/EndUpdate around grid bind (part of FIND-001) | Mali–srednji | Fewer intermediate paints |

### Do jedne sedmice

| Win | Action | Risk | Measurable result |
|-----|--------|------|-------------------|
| QW-11 | Real off-UI GetData for grid/tree with ThreadStatic-safe locals (FIND-001) | Srednji | Reduced UI freeze on large populate |
| QW-12 | Batch Manager settings save (FIND-006) | Srednji | O(1) SQL on FormClosing |
| QW-13 | Typed parameters on `Common.ToConvertSqlParams` (FIND-004) | Srednji | Cleaner plans for hot SPs |
| QW-14 | Debug ProjectReferences + local bin output (FIND-014) | Srednji | Reproducible Debug builds |
| QW-15 | foreachrow guidance + optional key-batch helper (FIND-005) | Mali–srednji | SQL count vs rows flattens for adopted granules |

---

## 5. PERFORMANCE BACKLOG

Sorted by **expected benefit / risk / effort** (best first). Benefit is engineering judgment pending FIND-015 baselines — not fabricated percentages.

| Rank | Item | Benefit | Risk | Effort | Notes |
|-----:|------|---------|------|--------|-------|
| 1 | FIND-015 Baseline instrumentation | Enabler | Mali | S–M | Do first |
| 2 | FIND-011 SqlContext dispose | Correctness | Mali | S | |
| 3 | FIND-010 Log/secret hygiene | Security | Mali | S | |
| 4 | FIND-002 ARITHABORT / connection chatter | Latency | Mali–srednji | S–M | Measure XEvents |
| 5 | FIND-006 Batch FormClosing saves | Close UX | Srednji | M | May need SP |
| 6 | FIND-001 Real async grid/tree load | UI freeze | Srednji | M | ThreadStatic care |
| 7 | FIND-008 Regex/Parser reuse | CPU/alloc | Mali | S–M | |
| 8 | FIND-005 foreachrow batching | N+1 | Srednji | M | XML+SP |
| 9 | FIND-004 Typed SQL params | SQL CPU | Srednji | M | Plan validation |
| 10 | FIND-009 XmlSerializer + XML cache | Startup | Mali–srednji | S–M | Cap+invalidate |
| 11 | FIND-007 Style precompute | Scroll CPU | Srednji–visoki | L | Profile first |
| 12 | FIND-003 DataTable-only path | Memory | Srednji | M | Not Server Mode yet |
| 13 | FIND-012 Transaction error semantics | Reliability | Mali–srednji | S | |
| 14 | FIND-014 Build ProjectReferences (Debug) | Dev speed | Srednji | M | |
| 15 | FIND-013 Extract fiscal/Outlook | Maintain + UX | Visoki | L | After tests |
| 16 | FIND-018 Remove UI Sleep | UX | Mali | S | XML inventory |
| 17 | FIND-019 Event unsubscribe / leak hunt | Memory | Srednji | M | Profile first |
| 18 | FIND-017 Filter FormClosing return | Correctness | Srednji | S | Product confirm |
| 19 | FIND-016 Events split (incremental) | Maintain | Visoki if rushed | XL | Many PRs |
| 20 | Server/InstantFeedback mode | Large grids | Visoki | XL | Only after feature matrix |

**Trade-off callouts:**
- **Off-UI SQL vs ThreadStatic:** Background threads won’t see `Connection.*` / `PubVars` — must pass explicit connection string and operater id. Opposite of “just sprinkle async”.
- **Server Mode vs Carpo grid features:** foreachrow, XML styles/`if`, layout templates, selection web style, group summaries may be incompatible — treat as separate product project, not a quick win.
- **Full SQL logging vs supportability:** Redaction improves security but support may want opt-in verbose SQL; keep explicit debug switch.

---

## 6. REFACTORING ROADMAP

### Faza 0 — Baseline i instrumentacija

- **Preduvjeti:** TestShell + one slow Manager granule + one Standard editor; SQL login for Extended Events; permission to write logs.
- **Posao:** Standardize PerfScope; correlation id per notification; document 8–10 scenarios; capture cold/warm; SQL counts; private bytes; open connections.
- **Očekivani rezultat:** Baseline spreadsheet/log pack; no behavior change.
- **Rollback:** Remove/disable PerfScope flag.

### Faza 1 — Sigurni lokalni refaktoring

- **Preduvjeti:** Faza 0 baselines.
- **Posao:** FIND-011, FIND-010, FIND-012, static Regex, XmlSerializer static, ARITHABORT gate, BeginUpdate on bind, AppRole secret removal.
- **Očekivani rezultat:** Safer correctness/security; small latency/alloc wins; green smoke checklist.
- **Rollback:** Per-PR revert; DLL rollback via SharedAssemblies previous version.

### Faza 2 — Izdvajanje odgovornosti

- **Preduvjeti:** Characterization tests for Data/Control/Form actions; fiscal/Outlook checklist machines.
- **Posao:** Extract DataActionExecutor; Fiscal/Outlook services; Settings batch API; thin Events router. **Do not** introduce full DI container.
- **Predloženi interfejs (primjer, minimalan):**
  ```csharp
  interface IDataActionExecutor {
      bool Execute(JEventAction action, EventsContext ctx);
  }
  interface IFiscalizationService {
      string ComputeZki(...);
      string SubmitJir(...);
  }
  ```
  Methods move from Events `CheckData`/`NotifyData`/`CheckTransaction` and ZKI/JIR blocks; Events keeps `RunActions` signature for ComponentBase.
- **Očekivani rezultat:** Smaller blast radius for changes; testable units for pure logic (Parser/Evaluate/param mapping).
- **Rollback:** Facade forwarding to old private methods initially; feature flag if needed.

### Faza 3 — Optimizacija hot pathova

- **Preduvjeti:** Measured proof of hot spots from Faza 0.
- **Posao:** Real off-UI grid/tree load; typed params; foreachrow batch patterns; style precompute; optional DataTable path; XML document cache with caps.
- **Očekivani rezultat:** Lower UI freeze and SQL chatter on top granules.
- **Rollback:** Feature flags per optimize (e.g. `AsyncGrid=false` XML/default).

### Faza 4 — Dugoročna arhitektura (ne zamjena za Faze 0–3)

- **Preduvjeti:** Stable test harness; product buy-in.
- **Posao:** Full Events modularization; ProjectReference graph for all configs; evaluate .NET / WinForms modernization **only as separate roadmap**; Server Mode product project if large-list modules demand it; reduce Specific duplication.
- **Očekivani rezultat:** Sustainable evolution without host break.
- **Rollback:** N/A (strategic); keep Jupiter contracts (`Starter.Run`, SharedAssemblies, x86) frozen.

---

## 7. PRIJEDLOG PRVIH PET PR-OVA

### PR-1 — Correctness: SqlContext command lifetime

- **Cilj:** Fix disposed `SqlCommand` return (FIND-011); dispose at caller; dispose readers.
- **Zahvaćeni fajlovi:** `Carpo.Data\Data\Context\SqlContext.cs`; callers (`SettingsService`, Outlook/fiscal usage).
- **Opseg:** &lt; ~50 LOC behavior fix
- **Rizik:** Mali
- **Test plan:** Settings load/save; Outlook action if available; no ObjectDisposedException.
- **Očekivani rezultat:** Stable SqlContext paths.

### PR-2 — Security hygiene: secrets out of default logs + AppRole hardcode

- **Cilj:** Redact password logging; default SQL log without values; remove/disable AppRole password constant (FIND-010).
- **Zahvaćeni fajlovi:** `Starter.cs`; `Carpo.Data\Log.cs` / CommandAsSql usage; `SqlHelper.cs` AppRole region.
- **Opseg:** Small, reviewable
- **Rizik:** Mali (support may need verbose flag docs)
- **Test plan:** Run Lite/TestShell; confirm logs; ensure verbose flag still works for support.
- **Očekivani rezultat:** No plaintext password in default logs; secret not in source.

### PR-3 — Instrumentation pack

- **Cilj:** PerfScope + correlation id + TestShell scenario list (FIND-015).
- **Zahvaćeni fajlovi:** Logger helpers; thin hooks in `Events.RunNotification`, `Core.SetData`, `ComponentGrid.Populate`, `Starter.Run`; doc section or `PerfBaseline.md` optional — prefer keep metrics in existing Logger to avoid doc sprawl unless team wants it.
- **Opseg:** Small–medium, no functional change when log level off
- **Rizik:** Mali
- **Test plan:** Capture baseline for 3 granules × cold/warm.
- **Očekivani rezultat:** Repeatable metrics for later PRs.

### PR-4 — SQL chatter: ARITHABORT + transaction null-safe rollback

- **Cilj:** FIND-002 + FIND-012 partial
- **Zahvaćeni fajlovi:** `Carpo.Data\Common.cs`; `Events.CheckTransaction`
- **Opseg:** Small
- **Rizik:** Mali–srednji (validate ARITHABORT-sensitive SPs)
- **Test plan:** XEvents ratio; transaction failure UX; smoke top granules.
- **Očekivani rezultat:** Fewer session SET batches; no rollback NRE.

### PR-5 — Grid bind hygiene: BeginUpdate + prepare real async behind flag

- **Cilj:** Re-enable BeginUpdate/EndUpdate; introduce explicit `definition.Async` path that uses true background load with marshaling; default remains sync until validated (FIND-001 staged).
- **Zahvaćeni fajlovi:** `ComponentGrid.cs` (possibly `ComponentTree.cs`)
- **Opseg:** Medium, isolated to populate pipeline
- **Rizik:** Srednji — ship flag default off in production until TestShell + Jupiter sign-off
- **Test plan:** Large grid populate; master-detail focus once; timer refresh; x86.
- **Očekivani rezultat:** Measurable freeze reduction when flag on; no regression when off.

---

## 8. ŠTA NE TREBA DIRATI

| Area | Why leave alone (for now) |
|------|---------------------------|
| **Jupiter host contracts** (`Starter.Run` signature, SharedAssemblies load, native parent HWND) | Breaks production host; out of Carpo control |
| **x86 production targeting** | COM/Outlook/VB6/ADODB interop reality |
| **Mass migration to .NET 6/8/9** | Explicitly out of scope; doesn’t fix XML/Events/SQL patterns by itself |
| **Big-bang rewrite of Events/ComponentGrid** | Untestable blast radius without characterization suite |
| **DevExpress Server Mode as default** | Conflicts with foreachrow, XML styles, layouts, selection modes — needs product design |
| **SqlHelper full deletion** | Still used by Specific/CMS/Designer paths; shrink later after caller migration |
| **XML schema / action vocabulary breaking changes** | Thousands of granules depend on it; only additive actions |
| **ThreadStatic Connection/PubVars redesign without session analysis** | Jupiter multi-form/session semantics unknown; easy to break |
| **Wide DI/IoC introduction** | High churn, low immediate perf gain; prefer targeted facades |
| **Carpo.Specific large UCs during framework hot-path work** | Different release/risk profile; isolate |

---

## 9. OTVORENA PITANJA

1. **Produkcijske metrike:** Koje granule generišu najviše ticketova za sporost / freeze?
2. **Tipične veličine gridova:** P50/P95 row counts and column counts for Manager master/detail?
3. **SQL execution plans** for top SPs called from Carpo (with actual parameter types/sizes)?
4. **Jupiter host verzija** (8 vs 10) and exact assembly load/binding redirects in SharedAssemblies?
5. **Deployment/rollback:** How are DLLs staged to SharedAssemblies today; can Debug build to local bin without overwriting prod install?
6. **Is `IJupiterStart` used by host via reflection** or dead? Confirm menuID passing convention.
7. **Which granules use foreachrow + datarun**, fiscal ZKI/JIR, Outlook, wait sleep?
8. **Are ARITHABORT-dependent plans** documented anywhere on SQL Server?
9. **Log retention / support** reliance on full `CommandAsSql` text?
10. **Memory complaints** after long Jupiter sessions (leak hypothesis FIND-019)?
11. **Filter-mode FormClosing early return** — intentional product behavior?
12. **Connection auth mode** in prod (SQL auth vs integrated) given Starter sets both password fields and an IntegratedSecurity-style ADO string?

---

## 10. KONAČNA PRIORITIZOVANA LISTA (max 20)

| # | Aktivnost | Korist | Rizik | Rad | Profilisanje prije? | Redoslijed |
|--:|-----------|--------|-------|-----|---------------------|------------|
| 1 | Baseline PerfScope + scenario pack (FIND-015) | velika (enabler) | mali | M | Ne | 1 |
| 2 | Fix SqlContext disposed command (FIND-011) | srednja | mali | S | Ne | 2 |
| 3 | Secret/log redaction + AppRole cleanup (FIND-010) | velika | mali | S | Ne | 3 |
| 4 | Transaction null-safe rollback + clearer failure (FIND-012) | srednja | mali | S | Ne | 4 |
| 5 | Gate/remove per-call ARITHABORT (FIND-002) | srednja–velika | mali–srednji | S | Da (XEvents) | 5 |
| 6 | Static Regex + Parser reuse per batch (FIND-008) | srednja | mali | S | Poželjno | 6 |
| 7 | Cache XmlSerializer (+ optional XML mtime cache) (FIND-009) | srednja | mali | S | Poželjno | 7 |
| 8 | Batch Manager FormClosing settings save (FIND-006) | velika | srednji | M | Da | 8 |
| 9 | Grid BeginUpdate + flagged async flag (FIND-001) | velika | srednji | M | Da | 9 |
| 10 | Typed SQL parameters on Common path (FIND-004) | srednja–velika | srednji | M | Da (plans) | 10 |
| 11 | foreachrow batch pattern + helper (FIND-005) | velika | srednji | M | Da | 11 |
| 12 | Debug ProjectReferences / local bin (FIND-014) | srednja (dev) | srednji | M | Ne | 12 |
| 13 | Style precompute cache (FIND-007) | velika | srednji–veliki | L | **Da** | 13 |
| 14 | DataTable-only populate path (FIND-003) | srednja | srednji | M | Da | 14 |
| 15 | Extract fiscal/Outlook services (FIND-013) | srednja | veliki | L | Djelomično | 15 |
| 16 | Remove UI Thread.Sleep wait (FIND-018) | mala–srednja | mali | S | XML inventura | 16 |
| 17 | Fix Filter-mode FormClosing early return (FIND-017) | srednja | srednji | S | Product confirm | 17 |
| 18 | Event unsubscribe / leak verification (FIND-019) | srednja | srednji | M | **Da** | 18 |
| 19 | Incremental Events split — Data executor first (FIND-016) | velika (maintain) | srednji per PR | L | Ne (tests) | 19 |
| 20 | Server/InstantFeedback mode feasibility study only | nepoznata | veliki | L | **Da** + feature matrix | 20 (study, not implement) |

---

## APPENDIX A — Baseline measurement plan (.NET Framework 4.8 / WinForms)

| Metric | Where to measure | Tool / instrumentation | Log format | Scenario | Compare |
|--------|------------------|------------------------|------------|----------|---------|
| Starter.Run total | Start/end `Starter.Run` | Existing `Logger.InfoTest` | `ts;metric;ms;module` | Cold start Module X | cold vs warm |
| XML deserialize | `Serializer.DeserializeXml` | InfoData / Stopwatch | `ts;xml_ms;path;parts` | Complex parts module | before/after cache |
| Granule interactive | First paint after Shown + first populate complete | InfoTest + manual stopwatch | `ts;tti_ms` | Manager open | median of 5 |
| Initial grid bind | `Populate` / `SetData` | Stopwatch split: SQL vs bind | `sql_ms;bind_ms;rows` | Large grid | before/after async |
| Grid refresh | Timer or explicit populate | same | same | Refresh ×10 | p95 |
| SQL calls / action | Count InfoData reader/scalar/nonquery under correlation id | Logger + optional XEvents | `actionId;count` | Button save; node focus; foreachrow | before/after |
| SQL duration sum | Sum execute times | Logger / XEvents `rpc_completed` | `actionId;sql_ms_sum` | same | before/after |
| UI freeze | UI thread samples | VS Diagnostic Session / PerfView / `Application.Idle` gaps | `freeze_ms` | During populate | before/after |
| Private bytes / heap | After open; after close+GC | PerfMon / VS .NET Memory | `priv_bytes;heap` | 50 open/close | leak slope |
| Gen 0/1/2 | Same | PerfMon .NET CLR Memory | counts | Heavy scroll styled grid | before/after style work |
| Live forms/controls | After close | custom counter in Dispose | counts | 50 cycles | must return to baseline |
| Open SQL connections | During/after actions | PerfMon `SqlClient` pooled | count | Chatty event | before/after ARITHABORT/batch |
| Cold vs warm | Process restart vs second open | all above | tag `cold|warm` | Module X | document both |

**Tools compatible with this stack:** Visual Studio Diagnostic Tools, PerfView, SQL Server Extended Events / Profiler, PerfMon counters, existing Carpo `Logger` (`InfoTest`/`InfoData`), dotMemory (optional), DebugDiag for hangs.

**Do not** invent expected percentage gains; record absolute ms/counts and compare after each PR.

---

## APPENDIX B — Minimal safety net (testing)

### Automatable now (no Jupiter host)

| Test type | Target | Notes |
|-----------|--------|-------|
| Unit | `Evaluate` / Parser brace substitution / Regex | Pure string fixtures |
| Unit | Parameter type mapping (once introduced) | XML type → SqlDbType |
| Unit | SqlContext command not disposed before use | After FIND-011 |
| Characterization | Serializer deserialize sample XML from TestShell fixtures | Golden `JRoot` shape |
| Integration | `Common.ExecuteQuery` against local SQL with stub SP | Connection lifecycle, timeout, output params |

### Requires Jupiter / TestShell / COM

| Test type | Target |
|-----------|--------|
| Smoke | Carpo.TestShell: Manager + Standard + Splitter open/close |
| Regression | Master-detail focus cascade; layout save/restore; filter mode |
| x86 | Explicit x86 build + run (production parity) |
| DevExpress | Grouping, BestFitOnRefresh, export, selection web style |
| Fiscal / Outlook | Only on equipped machines; not in CI by default |
| Host | Jupiter SharedAssemblies load of Carpo.dll; menu launch; modal editors |

### Manual checklist (Jupiter)

1. Open top 5 business granules cold/warm  
2. Tree navigation + detail refresh  
3. Save/close/reopen layouts  
4. Transactional multi-datarun success/fail  
5. foreachrow on small selection  
6. Print/export smoke  
7. Permissions-restricted controls  
8. x86 process bitness confirmed  

---

## APPENDIX C — Style vs performance (explicit separation)

| Item | Category |
|------|----------|
| Renaming locals / whitespace / expression-bodied members | Style only — not a performance backlog item |
| Splitting Events for readability without behavior change | Maintainability (Faza 2) |
| Removing dead commented blocks in ComponentGrid | Maintainability; do in small PRs after baselines |
| Fake-async, ARITHABORT, foreachrow N+1, paint-time Parser | Performance — prioritize |

---

## APPENDIX D — Long-term platform note (not a current fix)

A future migration off .NET Framework 4.8 / toward newer WinForms hosting may eventually be warranted for supportability, but it **does not** replace Faze 0–3. Any such program must preserve: Jupiter plugin contracts, x86/COM/ADODB interop strategy, XML granule compatibility, and SharedAssemblies deployment. Treat as a separate multi-year roadmap item.

---

*End of audit. Source code was not modified; only this document was added.*
