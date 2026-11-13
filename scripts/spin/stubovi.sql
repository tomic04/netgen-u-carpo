-- Minimalni objekti na spinu da se otvaranje NetGen granula ne zaustavi
-- na nedostajućem stupcu ili proceduri. Ne pišu u tablice.
-- Prava tijela procedura treba dati korisnik; ovo je prazan rezultat ispravnog oblika.
-- Ne pokretati na jupiter2019 ni jupiter2022.

IF OBJECT_ID(N'dbo.NetGenErrorLog', N'U') IS NULL
CREATE TABLE dbo.NetGenErrorLog (
    NetGenErrorLogId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    errorNumber int NULL,
    errorDescription nvarchar(max) NULL,
    ngLog nvarchar(max) NULL,
    Vrijeme datetime NOT NULL CONSTRAINT DF_NetGenErrorLog_Vrijeme DEFAULT (GETDATE())
);
GO

CREATE OR ALTER PROCEDURE dbo.spNetGenErrorLog
    @errorNumber int = NULL,
    @errorDescription nvarchar(max) = NULL,
    @ngLog nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.NetGenErrorLog (errorNumber, errorDescription, ngLog)
    VALUES (@errorNumber, @errorDescription, @ngLog);
END
GO

IF OBJECT_ID(N'dbo.klmasterroba', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.klmasterroba', N'oznaka2') IS NULL
    ALTER TABLE dbo.klmasterroba ADD oznaka2 varchar(50) NULL;
GO

IF OBJECT_ID(N'dbo.spKultureNGQuery', N'P') IS NULL
EXEC(N'
CREATE PROCEDURE dbo.spKultureNGQuery
    @Action int = NULL,
    @KultureId int = NULL,
    @id int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(NULL AS int) AS KultureId,
           CAST(NULL AS varchar(100)) AS Naziv,
           CAST(NULL AS varchar(20)) AS Ozn,
           CAST(NULL AS decimal(18,2)) AS Cijena
    WHERE 1 = 0;
END
');
GO

IF OBJECT_ID(N'dbo.spFakPartneriUvjetiQuery', N'P') IS NULL
EXEC(N'
CREATE PROCEDURE dbo.spFakPartneriUvjetiQuery
    @action int = NULL,
    @FakPartneriUvjetiId int = NULL,
    @id int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(NULL AS int) AS FakPartneriUvjetiId,
           CAST(NULL AS int) AS partneriid,
           CAST(NULL AS varchar(100)) AS Naziv
    WHERE 1 = 0;
END
');
GO
