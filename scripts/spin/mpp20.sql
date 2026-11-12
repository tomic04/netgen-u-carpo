-- MPP-20. SETDEFAULTS zove spmpPermissionNG. Stupci su @@ imena iz ONLOAD2,
-- ne naredbe gumba (INSERTFORM1 je Tag, Dodaj je Button.Text).
-- Form1.vb 3862-3866, 3906-3921, 3976-3998. Class1.vb 113-118 i 765-849.
-- Jupiter se ne dira.

IF OBJECT_ID(N'dbo.NetGenMpDozvola', N'U') IS NULL
CREATE TABLE dbo.NetGenMpDozvola (
    operateriid int NOT NULL,
    object varchar(100) NOT NULL,
    sifradok varchar(10) NOT NULL,
    dodaj int NOT NULL,
    izmjeni int NOT NULL,
    Zakljuci int NOT NULL,
    Obrisi int NOT NULL,
    storno int NOT NULL,
    otkljucaj int NOT NULL
);
GO

DELETE FROM dbo.NetGenMpDozvola
WHERE object = 'MPPonuda.xml' AND sifradok = '118' AND operateriid = 7;
INSERT INTO dbo.NetGenMpDozvola (operateriid, object, sifradok, dodaj, izmjeni, Zakljuci, Obrisi, storno, otkljucaj)
VALUES (7, 'MPPonuda.xml', '118', 1, 0, 0, 0, 0, 0);
GO

IF OBJECT_ID(N'dbo.spmpPermissionNG', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spmpPermissionNG;
GO
CREATE PROCEDURE dbo.spmpPermissionNG
    @action int = NULL,
    @operateriid int = NULL,
    @object varchar(100) = NULL,
    @sifradok varchar(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT dodaj, izmjeni, Zakljuci, Obrisi, storno, otkljucaj
    FROM dbo.NetGenMpDozvola
    WHERE @action = 0
      AND operateriid = @operateriid
      AND object = @object
      AND sifradok = @sifradok;
END
GO
