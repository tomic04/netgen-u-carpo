-- DOS-11 na localhost/spin. Nije klijentsko tijelo.
-- Combo i multidrop: stupac 0 je vrijednost, stupac 1 je natpis
-- (netgen/Form2.vb 2430-2431 i 2503-2504).
-- Pozivi su iz definitions/netgen/Agronom granule/KntDospjeleStavke.xml.
-- Ne pokretati na jupiter2019 ni jupiter2022.

CREATE OR ALTER PROCEDURE dbo.spKntPopuniKontroleNG
    @Action varchar(30) = NULL,
    @id int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'konto'
        SELECT CAST(1 AS int) AS id, CAST('1000' AS varchar(100)) AS naziv
        UNION ALL
        SELECT 2, '1200';
    ELSE IF @Action = 'klpartneri'
        SELECT CAST(1 AS int) AS id, CAST('Klasa' AS varchar(100)) AS naziv;
    ELSE IF @Action = 'odgosobe'
        SELECT CAST(1 AS int) AS id, CAST('Osoba' AS varchar(100)) AS naziv;
    ELSE IF @Action = 'zupanije'
        SELECT CAST(1 AS int) AS id, CAST('Zupanija' AS varchar(100)) AS naziv;
    ELSE IF @Action = 'mjesta'
        SELECT CAST(10 AS int) AS id, CAST('Mjesto' AS varchar(100)) AS naziv
        WHERE @id IS NULL OR @id = 1;
    ELSE
        SELECT CAST(NULL AS int) AS id, CAST(NULL AS varchar(100)) AS naziv WHERE 1 = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.spOrgShemaQuery
    @action int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(1 AS int) AS orgshemaid, CAST('Test shema' AS varchar(100)) AS naziv;
END
GO

CREATE OR ALTER PROCEDURE dbo.spKntOtvStavkeQueryNG
    @IdOsoba int = NULL,
    @DatumOd varchar(30) = NULL,
    @DatumDo varchar(30) = NULL,
    @idpar int = NULL,
    @nizkonto varchar(400) = NULL,
    @IdMjesto int = NULL,
    @IDzup int = NULL,
    @IDklasa int = NULL,
    @Iznos decimal(18, 2) = NULL,
    @idorg int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        CAST(1 AS int) AS id,
        CAST('Test partner' AS varchar(100)) AS Partner,
        CAST('1000' AS varchar(20)) AS Konto,
        CAST(10.00 AS decimal(18, 2)) AS Iznos;
END
GO
