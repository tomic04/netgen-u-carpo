-- FakPartneriUvjeti za otvaranje FakPar na spinu.
-- Stupci iz spFakPartneriUvjetiQuery.A.sql (action 1 i fp.*) i iz FakPar.xml.
-- partneri i parziro procedura spaja. Ako ih nema, samo stupci koje SELECT cita.
-- Postojeca partneri se ne mijenja. Jupiter se ne cita.

IF OBJECT_ID(N'dbo.partneri', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.partneri (
        partneriid int NOT NULL CONSTRAINT PK_partneri_ngfak PRIMARY KEY,
        sifra varchar(30) NULL,
        naziv varchar(100) NULL,
        matbroj varchar(30) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.parziro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.parziro (
        parziroid int NOT NULL CONSTRAINT PK_parziro_ngfak PRIMARY KEY,
        partneriid int NULL,
        ziro varchar(40) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.FakPartneriUvjeti', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FakPartneriUvjeti (
        FakPartneriUvjetiid int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FakPartneriUvjeti PRIMARY KEY,
        partneriid int NOT NULL,
        Ucestalost int NULL,
        Grupiranje int NULL,
        PoSkladistu int NULL,
        Valuta int NULL,
        Rabat decimal(18,2) NULL,
        Povrat decimal(18,2) NULL,
        parziroid int NULL,
        Ugovor varchar(100) NULL
    );
END
GO

IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'partneriid') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD partneriid int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Ucestalost') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Ucestalost int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Grupiranje') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Grupiranje int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'PoSkladistu') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD PoSkladistu int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Valuta') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Valuta int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Rabat') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Rabat decimal(18,2) NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Povrat') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Povrat decimal(18,2) NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'parziroid') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD parziroid int NULL;
IF COL_LENGTH(N'dbo.FakPartneriUvjeti', N'Ugovor') IS NULL ALTER TABLE dbo.FakPartneriUvjeti ADD Ugovor varchar(100) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.partneri)
   AND COL_LENGTH(N'dbo.partneri', N'sifra') IS NOT NULL
   AND COL_LENGTH(N'dbo.partneri', N'naziv') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM sys.columns c
        WHERE c.object_id = OBJECT_ID(N'dbo.partneri')
          AND c.is_nullable = 0
          AND c.is_identity = 0
          AND c.default_object_id = 0
          AND c.name NOT IN (N'partneriid', N'sifra', N'naziv')
   )
BEGIN
    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.partneri'), N'partneriid', 'IsIdentity') = 1
        SET IDENTITY_INSERT dbo.partneri ON;
    INSERT INTO dbo.partneri (partneriid, sifra, naziv) VALUES
        (900001, N'NG-FAK-1', N'NetGen partner 1'),
        (900002, N'NG-FAK-2', N'NetGen partner 2'),
        (900003, N'NG-FAK-3', N'NetGen partner 3');
    IF COLUMNPROPERTY(OBJECT_ID(N'dbo.partneri'), N'partneriid', 'IsIdentity') = 1
        SET IDENTITY_INSERT dbo.partneri OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.FakPartneriUvjeti WHERE Ugovor LIKE N'NG-FAK-%')
BEGIN
    ;WITH p AS (
        SELECT TOP 3 partneriid, ROW_NUMBER() OVER (ORDER BY partneriid) AS n
        FROM dbo.partneri
    )
    INSERT INTO dbo.FakPartneriUvjeti
        (partneriid, Ucestalost, Grupiranje, PoSkladistu, Valuta, Rabat, Povrat, Ugovor)
    SELECT partneriid,
           CASE n WHEN 1 THEN 7 WHEN 2 THEN 14 ELSE 30 END,
           CASE n WHEN 2 THEN 2 ELSE 1 END,
           CASE n WHEN 3 THEN 1 ELSE 0 END,
           CASE n WHEN 1 THEN 8 WHEN 2 THEN 15 ELSE 30 END,
           CASE n WHEN 1 THEN CAST(0 AS decimal(18,2)) ELSE CAST(2.50 AS decimal(18,2)) END,
           CASE n WHEN 1 THEN CAST(0 AS decimal(18,2)) ELSE CAST(1.25 AS decimal(18,2)) END,
           N'NG-FAK-' + CAST(n AS nvarchar(10))
    FROM p;
END
GO
