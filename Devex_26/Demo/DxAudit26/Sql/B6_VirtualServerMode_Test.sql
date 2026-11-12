/*
  B6 VirtualServerMode — test schema (50k rows)

  Purpose:
    - Local load / scroll / paging contract for a future opt-in Carpo path
    - Does NOT change existing granules (isolated demo objects)

  Objects:
    dbo.DxAudit26_BigGrid
    dbo.sp_DxAudit26_BigGrid_Page   (@Skip, @Take, @Sort, @Filter, @TotalCount OUT)
    dbo.sp_DxAudit26_BigGrid_Seed   (optional re-seed)

  Run once on a disposable / demo DB. Safe to re-run (IF NOT EXISTS / recreate SP).
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* ---------- table ---------- */
IF OBJECT_ID(N'dbo.DxAudit26_BigGrid', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DxAudit26_BigGrid
    (
        Id          INT             NOT NULL CONSTRAINT PK_DxAudit26_BigGrid PRIMARY KEY,
        Naziv       NVARCHAR(120)   NOT NULL,
        Status      TINYINT         NOT NULL,          -- 0/1
        Kategorija  NVARCHAR(40)    NOT NULL,
        Iznos       DECIMAL(18, 2)  NOT NULL,
        Datum       DATE            NOT NULL,
        PartnerId   INT             NOT NULL,
        Opis        NVARCHAR(200)   NULL
    );

    CREATE INDEX IX_DxAudit26_BigGrid_Datum
        ON dbo.DxAudit26_BigGrid (Datum, Id);

    CREATE INDEX IX_DxAudit26_BigGrid_Status_Naziv
        ON dbo.DxAudit26_BigGrid (Status, Naziv, Id);

    CREATE INDEX IX_DxAudit26_BigGrid_Kategorija
        ON dbo.DxAudit26_BigGrid (Kategorija, Id);
END;
GO

/* ---------- seed 50_000 rows (idempotent) ---------- */
CREATE OR ALTER PROCEDURE dbo.sp_DxAudit26_BigGrid_Seed
    @RowCount INT = 50000
AS
BEGIN
    SET NOCOUNT ON;

    IF @RowCount < 1
        SET @RowCount = 1;

    TRUNCATE TABLE dbo.DxAudit26_BigGrid;

    ;WITH
    n0 AS (SELECT 1 AS n UNION ALL SELECT 1),
    n1 AS (SELECT 1 AS n FROM n0 a CROSS JOIN n0 b),                 -- 4
    n2 AS (SELECT 1 AS n FROM n1 a CROSS JOIN n1 b),                 -- 16
    n3 AS (SELECT 1 AS n FROM n2 a CROSS JOIN n2 b),                 -- 256
    n4 AS (SELECT 1 AS n FROM n3 a CROSS JOIN n3 b),                 -- 65536
    nums AS
    (
        SELECT TOP (@RowCount)
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Id
        FROM n4
    )
    INSERT INTO dbo.DxAudit26_BigGrid
        (Id, Naziv, Status, Kategorija, Iznos, Datum, PartnerId, Opis)
    SELECT
        n.Id,
        N'Stavka ' + RIGHT(N'000000' + CAST(n.Id AS NVARCHAR(10)), 6),
        CAST(n.Id % 2 AS TINYINT),
        CASE n.Id % 5
            WHEN 0 THEN N'Alpha'
            WHEN 1 THEN N'Beta'
            WHEN 2 THEN N'Gamma'
            WHEN 3 THEN N'Delta'
            ELSE N'Omega'
        END,
        CAST((n.Id % 997) * 1.25 AS DECIMAL(18, 2)),
        DATEADD(DAY, -(n.Id % 730), CAST(GETDATE() AS DATE)),
        (n.Id % 8) + 1,
        N'Demo red #' + CAST(n.Id AS NVARCHAR(10))
    FROM nums n;

    SELECT COUNT(*) AS SeededRows FROM dbo.DxAudit26_BigGrid;
END;
GO

/* Seed if empty */
IF NOT EXISTS (SELECT 1 FROM dbo.DxAudit26_BigGrid)
    EXEC dbo.sp_DxAudit26_BigGrid_Seed @RowCount = 50000;
GO

/* ---------- paging SP (B6 contract) ----------
   @Skip / @Take  — OFFSET/FETCH page
   @Sort          — whitelist only (column [ASC|DESC]); default Id ASC
   @Filter        — optional contains on Naziv / Kategorija / Opis (simple demo filter)
   @TotalCount    — OUT total matching filter (for VirtualServerMode Configuration)

   Result set columns match a typical Carpo grid field list.
*/
CREATE OR ALTER PROCEDURE dbo.sp_DxAudit26_BigGrid_Page
    @Skip       INT = 0,
    @Take       INT = 50,
    @Sort       NVARCHAR(200) = NULL,
    @Filter     NVARCHAR(200) = NULL,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Skip IS NULL OR @Skip < 0 SET @Skip = 0;
    IF @Take IS NULL OR @Take < 1 SET @Take = 50;
    IF @Take > 500 SET @Take = 500; -- hard cap for demo safety

    DECLARE @SortNorm NVARCHAR(200) = UPPER(LTRIM(RTRIM(ISNULL(@Sort, N''))));
    DECLARE @FilterNorm NVARCHAR(200) = NULLIF(LTRIM(RTRIM(@Filter)), N'');

    /* Whitelist sort — never concatenate arbitrary @Sort into dynamic SQL blindly */
    DECLARE @OrderBy NVARCHAR(200) =
        CASE @SortNorm
            WHEN N'ID'              THEN N'Id ASC'
            WHEN N'ID ASC'          THEN N'Id ASC'
            WHEN N'ID DESC'         THEN N'Id DESC'
            WHEN N'NAZIV'           THEN N'Naziv ASC'
            WHEN N'NAZIV ASC'       THEN N'Naziv ASC'
            WHEN N'NAZIV DESC'      THEN N'Naziv DESC'
            WHEN N'STATUS'          THEN N'Status ASC'
            WHEN N'STATUS ASC'      THEN N'Status ASC'
            WHEN N'STATUS DESC'     THEN N'Status DESC'
            WHEN N'KATEGORIJA'      THEN N'Kategorija ASC'
            WHEN N'KATEGORIJA ASC'  THEN N'Kategorija ASC'
            WHEN N'KATEGORIJA DESC' THEN N'Kategorija DESC'
            WHEN N'IZNOS'           THEN N'Iznos ASC'
            WHEN N'IZNOS ASC'       THEN N'Iznos ASC'
            WHEN N'IZNOS DESC'      THEN N'Iznos DESC'
            WHEN N'DATUM'           THEN N'Datum ASC'
            WHEN N'DATUM ASC'       THEN N'Datum ASC'
            WHEN N'DATUM DESC'      THEN N'Datum DESC'
            WHEN N'PARTNERID'       THEN N'PartnerId ASC'
            WHEN N'PARTNERID ASC'   THEN N'PartnerId ASC'
            WHEN N'PARTNERID DESC'  THEN N'PartnerId DESC'
            ELSE N'Id ASC'
        END;

    /* Filtered set as CTE for count + page (demo scale OK at 50k) */
    ;WITH filtered AS
    (
        SELECT
            g.Id,
            g.Naziv,
            g.Status,
            g.Kategorija,
            g.Iznos,
            g.Datum,
            g.PartnerId,
            g.Opis
        FROM dbo.DxAudit26_BigGrid AS g
        WHERE @FilterNorm IS NULL
           OR g.Naziv LIKE N'%' + @FilterNorm + N'%'
           OR g.Kategorija LIKE N'%' + @FilterNorm + N'%'
           OR g.Opis LIKE N'%' + @FilterNorm + N'%'
    )
    SELECT @TotalCount = COUNT(*) FROM filtered;

    DECLARE @sql NVARCHAR(MAX) = N'
    ;WITH filtered AS
    (
        SELECT
            g.Id,
            g.Naziv,
            g.Status,
            g.Kategorija,
            g.Iznos,
            g.Datum,
            g.PartnerId,
            g.Opis
        FROM dbo.DxAudit26_BigGrid AS g
        WHERE @pFilter IS NULL
           OR g.Naziv LIKE N''%'' + @pFilter + N''%''
           OR g.Kategorija LIKE N''%'' + @pFilter + N''%''
           OR g.Opis LIKE N''%'' + @pFilter + N''%''
    )
    SELECT
        Id, Naziv, Status, Kategorija, Iznos, Datum, PartnerId, Opis
    FROM filtered
    ORDER BY ' + @OrderBy + N'
    OFFSET @pSkip ROWS FETCH NEXT @pTake ROWS ONLY;';

    EXEC sp_executesql
        @sql,
        N'@pSkip INT, @pTake INT, @pFilter NVARCHAR(200)',
        @pSkip = @Skip,
        @pTake = @Take,
        @pFilter = @FilterNorm;
END;
GO

/* ---------- smoke checks ---------- */
DECLARE @Total INT;

-- page 1
EXEC dbo.sp_DxAudit26_BigGrid_Page
    @Skip = 0, @Take = 50, @Sort = N'Id ASC', @Filter = NULL, @TotalCount = @Total OUTPUT;
SELECT @Total AS TotalCount_Page1;

-- page 2
EXEC dbo.sp_DxAudit26_BigGrid_Page
    @Skip = 50, @Take = 50, @Sort = N'Naziv DESC', @Filter = NULL, @TotalCount = @Total OUTPUT;

-- filtered
EXEC dbo.sp_DxAudit26_BigGrid_Page
    @Skip = 0, @Take = 25, @Sort = N'Iznos DESC', @Filter = N'Alpha', @TotalCount = @Total OUTPUT;
SELECT @Total AS TotalCount_FilterAlpha;

PRINT N'B6 test objects ready: DxAudit26_BigGrid + sp_DxAudit26_BigGrid_Page';
GO
