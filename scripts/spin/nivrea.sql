-- NIV-05 i REA-04. ONSUBMIT mora proći na spinu bez SQL greške.
-- MPNivelacija FILTERFORM šalje spMPQueryGlaIDetNG s @lokacijeid (XML).
-- Ostali pozivi istog XML-a nose @node i @id. Jupiter se ne dira.

IF OBJECT_ID(N'dbo.spMPQueryGlaIDetNG', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spMPQueryGlaIDetNG;
GO
CREATE PROCEDURE dbo.spMPQueryGlaIDetNG
    @action int = NULL,
    @sifradok varchar(20) = NULL,
    @operateriid int = NULL,
    @trgovinaid int = NULL,
    @datumod varchar(20) = NULL,
    @datumdo varchar(20) = NULL,
    @lokacijeid int = NULL,
    @trazibroj varchar(50) = NULL,
    @node varchar(50) = NULL,
    @id int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS id, N'nivelacija' AS naziv;
END
GO

-- MPRealizNG FORM1: ONSUBMIT ima @action i @operateriid, pa NetGen lijepi parametre kontrola.
IF OBJECT_ID(N'dbo.spMPRealizacijaRobeNG', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spMPRealizacijaRobeNG;
GO
CREATE PROCEDURE dbo.spMPRealizacijaRobeNG
    @action int = NULL,
    @operateriid int = NULL,
    @trgovinaid int = NULL,
    @robaid int = NULL,
    @search varchar(100) = NULL,
    @klasaid int = NULL,
    @podklase int = NULL,
    @datumod varchar(20) = NULL,
    @datumdo varchar(20) = NULL,
    @vrstadokid int = NULL,
    @partneriid int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 1 AS id, N'realizacija' AS naziv;
END
GO
