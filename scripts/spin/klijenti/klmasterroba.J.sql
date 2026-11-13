CREATE TABLE [dbo].[KlMasterRoba] (
    [KlMasterRobaId] int IDENTITY(1,1) NOT NULL,
    [SortKey] varchar(20) COLLATE SQL_Croatian_CP1250_CI_AS NOT NULL,
    [Naziv] varchar(50) COLLATE SQL_Croatian_CP1250_CI_AS NOT NULL,
    [Oznaka] varchar(50) COLLATE SQL_Croatian_CP1250_CI_AS NOT NULL,
    [Marza] float NOT NULL DEFAULT (0),
    [PubInd] char(1) COLLATE SQL_Croatian_CP1250_CI_AS NOT NULL DEFAULT (1),
    [Usluga] char(1) COLLATE SQL_Croatian_CP1250_CI_AS NOT NULL DEFAULT ('0'),
    [KntKontaZaShemuId] int NOT NULL DEFAULT (1),
    [PlanPromet] money NULL DEFAULT (0),
    [PlanRUC] money NULL DEFAULT (0),
    [MarzaMP] float NULL DEFAULT (0),
    [OsobePLMId] int NULL,
    [KlasaKasa] varchar(10) COLLATE SQL_Croatian_CP1250_CI_AS NULL,
    [RabatDefault] decimal(8,4) NULL
);
PRIMARY KEY CLUSTERED ([KlMasterRobaId]) -- PK_KlMasterRoba
CREATE NONCLUSTERED INDEX [IX_KlMasterRoba_SortKey] ON [dbo].[KlMasterRoba] ([SortKey]);
FOREIGN KEY [FK_KlMasterRoba_KntKontaZaShemu] ([KntKontaZaShemuId]) REFERENCES [dbo].[KntKontaZaShemu] ([KntKontaZaShemuId]);