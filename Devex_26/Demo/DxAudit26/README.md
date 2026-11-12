# DxAudit26 — demo granule

**Vodič za XML autore:** [../../Carpo_DX_XmlAutor_Vodic.md](../../Carpo_DX_XmlAutor_Vodic.md)

**Integration test env:** [../../Devex26_Integration_TestEnv.md](../../Devex26_Integration_TestEnv.md)

## XML granule

| XML | Forma | Sta |
|-----|-------|-----|
| [`DxAudit26_Demo.xml`](DxAudit26_Demo.xml) | `DxAudit26_Demo` | DX/audit gumbi 1-8 (+ **9** dialog VSM) |
| [`DxAudit26_BigGrid_Vsm.xml`](DxAudit26_BigGrid_Vsm.xml) | `DxAudit26_BigGrid_Vsm` | **B6 VSM** — 50k paging (brzi start) |
| [`DxAudit26_BigGrid_Full.xml`](DxAudit26_BigGrid_Full.xml) | `DxAudit26_BigGrid_Full` | **Stari nacin** — 50k full DataTable (usporedba) |
| [DxAudit26_UxPolish.xml](DxAudit26_UxPolish.xml) | DxAudit26_UxPolish | **Wave12 UX** — Find allow/always, emptymessage, asynccaption |

## Wave 12 — UX polish (2026-08-10)

| Attr | Gdje | Sta |
|------|------|-----|
| `findpanel="true"|"always"` | grid/tree | Pinned Find + highlight + Clear |
| `findpanel="allow"` | grid/tree | Allow Find (Ctrl+F), nije pinned |
| `emptymessage="..."` | grid | Poruka kad nema redova |
| `asynccaption="..."` | grid/tree | Splash tekst uz `async="true"` |

Demo: master `asynccaption` + `emptymessage`; detail `findpanel="allow"`; gumbi **2 / 2a / 2b**.
LookUp `nulltext` vec koristi NullValuePrompt (watermark) — bez nove izmjene.

## B6 BigGrid 50k — usporedba VSM vs FULL

Preduvjet (jednom): [`Sql\B6_VirtualServerMode_Test.sql`](Sql/B6_VirtualServerMode_Test.sql)

| | VSM | FULL (stari) |
|--|-----|----------------|
| XML / forma | `DxAudit26_BigGrid_Vsm` | `DxAudit26_BigGrid_Full` |
| Data | `sp_DxAudit26_BigGrid_Page` (stranice) | `SELECT ... FROM DxAudit26_BigGrid` (sve) |
| Memorija | ~stranica | cijeli set (~50k) |
| Sto mjeriti | vrijeme do prvog prikaza / scroll | vrijeme do populate gotov |

Iste kolone + styles — usporedi otvaranje forme i gumb Refresh.

## DxAudit26_Demo — gumbi

**Vodič za XML autore:** [../../Carpo_DX_XmlAutor_Vodic.md](../../Carpo_DX_XmlAutor_Vodic.md)

**Integration test env:** [../../Devex26_Integration_TestEnv.md](../../Devex26_Integration_TestEnv.md)

| Gumb / feature | Sta radi | Kako testirati |
|----------------|----------|----------------|
| **1) Refresh grid async** | Master se puni u backgroundu (~1s) | Klik — loading panel |
| **1b) Refresh tree async** | Tree `async="true"` (~0.5s) | Klik — TreeList loading panel |
| **1c) Lazy tree** | B8: `lazy="true"` + `$TreeParentId` na expand | Forma → expand Root A/B |
| **9) Big grid 50k (dialog)** | Isti VSM u dialogu (ili otvori posebnu granulи gore) | Izmedu **1c** i **2** na traci |
| **2) Prikaži Find traku** | Runtime ukljuci search bar | Tipkaj npr. `Aktiv` / `Alpha` |
| **2b) Sakrij Find traku** | Sakrije Find bar | Klik |
| **3) EditForm (F2)** | DX EditForm (`editform="true"`) | Selektiraj red → **F2** |
| **4) Lookup + TokenEdit** | AutoSearch / AutoComplete / AutoSuggest / Tagovi | Subforma |
| **5 / 5b Transaction** | success vs fail + log | FAIL → log |
| **6) Column Chooser** | B12 | Klik |
| **7) Batch selectedids** | A9 preferred | Checkbox → klik |
| **7b) foreachrow N× SQL** | A9 anti-pattern | Oznaci redove |
| **8 / 8b** | Outlook / ZKI+JIR dry-run | Info poruke |

## Scope

v1–v11 + B6 demo (opt-in VirtualServerMode + FULL usporedba). Produkcija VSM samo uz buy-in.
