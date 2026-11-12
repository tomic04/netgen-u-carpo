-- PL-32. Combo podloge: stupac 0 je vrijednost, stupac 1 natpis.
-- Form1.vb 3342-3344. SelectedValue postane TREE i cijepa se po # i : (4344, 7730-7733).
-- PrijemniList.xml COMBO zove spNg_combo @action='Drvo_NabPri'.
-- Lijevi segment koji sadrzi "tree" je stora. Desno od : su forme iz istog XML-a (Promet, Skladiste).
-- Jupiter se ne dira.

IF OBJECT_ID(N'dbo.NetGenNgCombo', N'U') IS NULL
CREATE TABLE dbo.NetGenNgCombo (
    action varchar(50) NOT NULL,
    vrijednost varchar(400) NOT NULL,
    natpis varchar(100) NOT NULL
);
GO

IF OBJECT_ID(N'dbo.NetGenNgStablo', N'U') IS NULL
CREATE TABLE dbo.NetGenNgStablo (
    izvor varchar(50) NOT NULL,
    id int NOT NULL,
    parent int NULL,
    natpis varchar(100) NOT NULL,
    tag varchar(100) NULL
);
GO

DELETE FROM dbo.NetGenNgCombo WHERE action = 'Drvo_NabPri';
INSERT INTO dbo.NetGenNgCombo (action, vrijednost, natpis)
VALUES ('Drvo_NabPri', 'Danas:Promet#spNgTreeNabPri:Skladiste', 'Nabavni prijem');
GO

DELETE FROM dbo.NetGenNgStablo WHERE izvor = 'NabPri';
INSERT INTO dbo.NetGenNgStablo (izvor, id, parent, natpis, tag)
VALUES ('NabPri', 1, NULL, 'Sva skladista', ''),
       ('NabPri', 2, 1, 'Skladiste 1', '');
GO

IF OBJECT_ID(N'dbo.spNgTreeNabPri', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spNgTreeNabPri;
GO
CREATE PROCEDURE dbo.spNgTreeNabPri
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, parent, natpis, tag
    FROM dbo.NetGenNgStablo
    WHERE izvor = 'NabPri'
    ORDER BY id;
END
GO

IF OBJECT_ID(N'dbo.spNg_combo', N'P') IS NOT NULL
    DROP PROCEDURE dbo.spNg_combo;
GO
CREATE PROCEDURE dbo.spNg_combo
    @action varchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT vrijednost, natpis
    FROM dbo.NetGenNgCombo
    WHERE action = @action
    ORDER BY natpis;
END
GO
