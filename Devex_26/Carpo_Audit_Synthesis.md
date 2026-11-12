# Carpo Audit Synthesis — Devex_26

**Status:** Done (Iter 1 + Iter 3 re-verify)  
**Sources:** `Carpo_Audit_Claude-Fable-5.md`, `Carpo_Audit_Cursor-Grok-4.5.md`, `Carpo_Audit_GPT-5.6_Terra.md`, `Carpo_Audit_GPT-5.6-Sol.md`  
**Target tree:** `Carpo\Devex_26`  
**Date:** 2026-08-07

---

## Status table

| Iter | Deliverable | Status |
|------|-------------|--------|
| 0 | Copy 4 audit MD | Done |
| 1 | This synthesis | Done |
| 2 | `Carpo_DevExpress_Inventory.md` | Done |
| 3 | Re-verify P0 on Devex_26 | Done — **0 FIXED / all STILL_PRESENT** |
| 4 | `Carpo_DevExpress_Gap.md` | Done |
| 5 | Audit P0 implementation | Done (A1–A7) |
| 6 | DX opt-in prototypes | Done (B1–B5) |

---

## Consensus map (≥3/4 audits)

| ID | Tema | Sev | Confidence (post-Iter3) | Devex_26 path | Minimal fix | XML/host risk |
|----|------|-----|-------------------------|---------------|-------------|----------------|
| A1 | `SqlContext.CreateCommand` dispose-before-return | P0 | High — STILL | `Carpo.Data\Data\Context\SqlContext.cs` | Drop `using` on returned command | Low |
| A2 | Fake grid async (`Task.FromResult` SQL on UI) | P0 | High — STILL | `ComponentGrid.GetDataAsync()` | BeginUpdate + restore real `StartNew` when `async=true` | Low if default stays sync |
| A3 | Transaction swallow + lock across dialogs | P0 | High — STILL | `Events.CheckTransaction` | Log + `LastTransactionSuccess=fail`; no Events rewrite | Medium if rethrow |
| A4 | Password in log (`RegisterForm`) | P0 sec | High — STILL | `Starter.RegisterForm` | Redact password from log | None |
| A5 | Hardcoded AppRole `Passw0rd!` | P0 sec | High — STILL (calls commented) | `SqlHelper.ExecuteEnableAppRole` | Remove literal / disable path | Low |
| A6 | FormatRules accumulate; BeginUpdate commented | P0/P1 | High — STILL | `ComponentGrid.SetConditionalFormatting` / populate | `FormatRules.Clear` + Begin/EndUpdate | Low |
| A7 | FormClosing `GC.Collect` | P1 | High — STILL | `JupitetShared.JupiterShared_FormClosing` | Remove forced GC | Low |
| A8 | Per-call `SET ARITHABORT` + `CommandAsSql` | P1 | High — STILL | `Carpo.Data\Common.ExecuteQuery` | Defer — session-level ARITHABORT later | Medium |
| A9 | `foreachrow` N× Events / N+1 | P1 | High — STILL | `ComponentGrid.SetNotifyForEachRow` | Defer — not Iter5 | High |
| A10 | Paint-path `Parser` alloc (RowCellStyle→Statemants) | P1 | High — STILL | `Conditions` / `Statemants` | Defer — cache Parser later | Medium |

### 2/4 or unique (not Iter5)

- HintPath / Program Files build fragility — **ops P2**
- Fiscal / Outlook sync inside Events — extract later P1
- ThreadStatic / DI rewrite — **out of scope**

---

## Ranked implementable backlog (A1…A8)

Iter 5 implemented:

1. **A1** SqlContext.CreateCommand ownership — **Done**  
2. **A4** Stop logging plaintext password — **Done**  
3. **A5** Remove AppRole hardcoded password — **Done**  
4. **A3** Transaction: log + fail signal (minimal) — **Done**  
5. **A6** FormatRules.Clear + BeginUpdate on populate/SetData — **Done**  
6. **A2** Grid: BeginUpdate; re-wire `async` attribute to real background fetch — **Done**  
7. **A7** Remove FormClosing `GC.Collect` — **Done**  
8. **A8** *Deferred* — ARITHABORT / CommandAsSql (needs broader data-layer review)

---

## Ops vs runtime

| Bucket | Items |
|--------|--------|
| Runtime bugs | A1–A3, A6–A7, A9–A10 |
| Security | A4, A5 |
| Ops / build | HintPath to Desktop / Program Files DX packages |
| Perf tax (later) | A8, A9, A10 |

---

## Iter 3 verification notes

- Live populate always calls parameterless `GetDataAsync()` → `Task.FromResult(GetDataSet(...))`.  
- Background overload `GetDataAsync(FocusType)` with `Task.Factory.StartNew` exists but call site is commented.  
- AppRole credential still in source; enable call sites commented.  
- RowCellStyle path allocates `Parser` via `Statemants`, not `Evaluate` (wording nuance only).
