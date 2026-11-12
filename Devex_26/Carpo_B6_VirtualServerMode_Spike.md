# B6 — VirtualServerMode spike / mini-plan

**Datum:** 2026-08-07 (update 2026-08-10)  
**Status:** Spike → **phased opt-in (Iter 0–3 done, demo only)**  
**DX:** 26.1 (`VirtualServerModeSource` u `DevExpress.Data.v26.1`)

---

## Cilj

Opt-in `virtualservermode="true"` na postojećem `ComponentGrid` **bez** loma default `DataTable` putanje.

---

## Implementirano (2026-08-10)

| Iter | Status | Šta |
|------|--------|-----|
| **0** | Done | SQL 50k + SP — `Demo\DxAudit26\Sql\B6_VirtualServerMode_Test.sql` |
| **1** | Done | `JGrid.virtualservermode`, `VirtualServerModeBinder`, Populate branch, demo gumb **9** |
| **2** | Done | Sort whitelist + Find/filter → `@Sort` / `@Filter` |
| **3** | Done | Dispose/rebind na populate; foreachrow + print guards |
| **4** | Done (partial) | Print/export **hydrate-on-demand** (confirm + wait + temp clone); produkcijska granula i dalje buy-in |

**XML:** `<grid … virtualservermode="true" />` + data `command="sp_DxAudit26_BigGrid_Page"` (ne `type="text"`).

**Binder:** `Carpo.UI\Logic\Grid\VirtualServerModeBinder.cs` — direktan SP call (`@Skip/@Take/@Sort/@Filter/@TotalCount OUT`).

**Demo:** `DxAudit26_BigGrid_Vsm` (paging) + `DxAudit26_BigGrid_Full` (stari full DataTable); gumb **9** u `DxAudit26_Demo.xml`.

### Mjerenje pokretanja (2026-08-10, lokalni TestShell)

| Granula | Način | Rezultat |
|---------|-------|----------|
| `DxAudit26_BigGrid_Vsm` | VSM paging SP | **~2× brži** start forme |
| `DxAudit26_BigGrid_Full` | full `SELECT` 50k → DataTable | baseline |

Iste kolone / ista tablica `DxAudit26_BigGrid`. Subjektivno / stopwatch usporedba otvaranja — potvrđuje opt-in vrijednost za velike setove.

---

## Kratko: SP + kako DX kešira

Za XML autore također: [`Carpo_DX_XmlAutor_Vodic.md`](Carpo_DX_XmlAutor_Vodic.md) §3.7 / §7.  
Puni SQL: `Demo\DxAudit26\Sql\B6_VirtualServerMode_Test.sql`.

### Minimalna paging SP

```sql
CREATE OR ALTER PROCEDURE dbo.sp_MojaLista_Page
    @Skip       INT = 0,
    @Take       INT = 50,
    @Sort       NVARCHAR(200) = NULL,   -- npr. 'Id ASC' (whitelist u SP!)
    @Filter     NVARCHAR(200) = NULL,   -- Find tekst / jednostavan filter
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Skip < 0 SET @Skip = 0;
    IF @Take < 1 SET @Take = 50;

    -- 1) koliko redova ukupno (scrollbar / “ima još”)
    SELECT @TotalCount = COUNT(*)
    FROM dbo.MojaTablica
    WHERE (@Filter IS NULL OR Naziv LIKE N'%' + @Filter + N'%');

    -- 2) samo jedna stranica (ORDER BY: whitelist iz @Sort, ne raw concat)
    SELECT Id, Naziv, Status, Iznos
    FROM dbo.MojaTablica
    WHERE (@Filter IS NULL OR Naziv LIKE N'%' + @Filter + N'%')
    ORDER BY Id ASC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
END
```

XML: `virtualservermode="true"` + `<data command="sp_MojaLista_Page"/>` (**ne** full `type="text"` SELECT).

### Kako radi DevExpress `VirtualServerModeSource`

1. **Configuration** (open / sort / Find) → Carpo zove SP s `@Skip=0`, `@Take≈50`, trenutni sort/filter → prva stranica + `@TotalCount OUT`. DX zna koliki je “virtualni” set.
2. **MoreRows** (scroll) → SP s `@Skip = broj već učitanih` → sljedeća stranica.
3. Carpo (`VirtualServerModeBinder`) samo zove SP i mapira redove. `Grid.DataSource` = `VirtualServerModeSource`, **ne** puni lokalni `DataTable`.

```text
Grid → VirtualServerModeSource
         ├─ ConfigurationChanged → SP page0 + TotalCount
         └─ MoreRows             → SP next page (append)
```

### Šta se kešira

| Šta | Gdje |
|-----|------|
| Stranice već dohvaćene sa SP-a | Memorija DX `VirtualServerModeSource` (append dok skrolaš) |
| `TotalCount` | Binder / DX — za scrollbar, bez vučenja svih redova |
| Trenutni `@Sort` / `@Filter` | Binder; novi sort/Find → Configuration od 0 (keš stranica se resetuje) |
| Cijeli set (npr. 50k) na open | **Ne** |
| Cijeli filter-set na print/export | **Da, on-demand** — confirm → `FetchAllFiltered` → temp clone → dispose |

Zato je start brz: u RAM-u su tipično desetine/stotine redova, ne desetine tisuća.

---

## Feature matrix (važi)

| Feature | VSM |
|---------|-----|
| Scroll / page load | Da |
| Column sort (whitelist SP) | Da |
| Find / simple filter | Da (Find text / simple criteria literal) |
| `foreachrow` full set | **Ne** — warn + Info |
| `#grid.selectedids` | Samo učitani/označeni |
| Full export/print 50k | **Da (on-demand)** — confirm → SP page-all → temp clone → print/export |
| Default grid bez atributa | **Nepromijenjen** |

---

## Šta odjel mora napraviti / prihvatiti (produkcija)

### Mora napraviti (tehnički)

1. **Paging SP** — ugovor: `@Skip`, `@Take`, `@Sort`, `@Filter`, `@TotalCount OUT` (isti filter/sort kao UI).
2. **XML** — na gridu `virtualservermode="true"`; data = ta SP (**ne** `type="text"` full SELECT).
3. **Kolone / tipovi** — SP resultset usklađen s grid kolonama (binder očekuje stabilnu shemu).
4. **Smoke** — otvaranje, scroll, sort, Find, Refresh; bez oslanjanja na `foreachrow` preko cijelog seta.

### Mora prihvatiti — šta se gubi ili mijenja

| Šta | Danas s VSM | Zašto |
|-----|-------------|--------|
| Brzi start / manje memorije | **Dobija** | Učitava se stranica, ne cijeli set |
| `foreachrow` preko **svih** redova | **Gubi** | Nema lokalnog punog `DataTable`; warn + Info |
| `#grid.selectedids` / batch | Samo **označeni / učitani** redovi | Nema “svih Id-eva u memoriji” |
| Print / export **cijelog** seta | **Može** (hydrate) | Confirm + wait; privremeno učita filter/sort set (cap 100000) |
| Sum / agregati nad cijelim setom u UI | Često **netočno** ako DX broji samo cache | Treba SP/server aggregate ili hydrate |
| Stari “napuni sve pa radi” skripti | **Ne rade** na cijelom setu | Moraju batch / selectedids / server SP |

**Buy-in** = odjel eksplicitno kaže: želimo VSM na ovoj granulі, SP + XML su spremni, i prihvaćamo gornju listu (ili čekamo print-hydrate / drugu zamjenu).

---

## Predlog: print → hydrate sve (on-demand)

**Ideja:** UI ostaje VSM (brzi start). Kad korisnik traži print/export **cijelog** seta (ili “sve što filter trenutno pokriva”), Carpo **u tom trenutku** povuče sve stranice (ili jedan “full dump” SP call s istim `@Sort`/`@Filter`) u privremeni `DataTable` / clone grid → print → dispose.

| | |
|--|--|
| **Za** | Zadrži brzi start; print “sve” ostaje moguć kad treba; odjel ne mora birati “ili brzina ili print” |
| **Protiv / rizik** | Spike memorije i vremena baš na print (50k+); timeout; treba wait/cancel UI; filter mora biti **isti** kao na gridu |
| **Preporuka** | **Da, kao opt-in** (npr. ponašanje ili XML flag kasnije) — ne kao tihi default na svaki print. Print selektiranih / vidljivog cachea ostaje jeftin; “print all” = eksplicitan hydrate + poruka “učitavam N redova…” |
| **Bolja alternativa za ogromne setove** | Server-side report / SP koji generira dokument bez slanja 50k na klijent |

**Status (2026-08-10):** implementirano u `PrintControl` / `GridExport` + `VirtualServerModeBinder.FetchAllFiltered`.  
UI: Yes/No confirm → splash wait → temp `PrintClonedGridControl` → print/export → dispose. Live VSM grid ostaje netaknut.

---

## Što Carpo grid danas pretpostavlja (konflikt)

| Pretpostavka | Conflict |
|--------------|----------|
| Pun `DataTable` | Server mode šalje stranice |
| `foreachrow` / `DataRowRef` | Nema lokalnih svih redova |
| Full export clone | Mora pullati sve (hydrate) ili drugačiji ugovor |

---

## DX ugovor

1. **Configuration** — prva stranica + total  
2. **MoreRows** — sljedeće stranice  
3. SP: `@Skip/@Take/@Sort/@Filter` + `@TotalCount OUT`

---

## Odluka (nepromijenjena za produkciju)

| Opcija | Verdict |
|--------|---------|
| Default na svim Manager gridovima | **Ne** |
| Opt-in demo / odabrane granule | **Da** (Iter 1–3) |
| Auto-migracija svih 50k gridova | **Ne** |
| Print hydrate-on-demand | **Done** (Iter 4 partial) — confirm + FetchAllFiltered |

Za velike setove bez VSM i dalje: `async="true"`, SP TOP/OFFSET u granulі, `#grid.selectedids`.

---

## Checklist prije produkcije (Iter 4)

1. SP paging ugovor na ciljnoj granulі  
2. Odjel prihvatio “šta se gubi” tablicu gore  
3. Load test scroll/sort/filter  
4. Ne dirati default path  
5. Print hydrate smoke na `DxAudit26_BigGrid_Vsm` (print all ~50k)  
