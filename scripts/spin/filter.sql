-- NetGen čita spremljene filtere. Prazna tablica je 0 redova, ne greška.
-- Form1.vb 4360-4370: ako upit padne, čvorovi se ne dodaju. Stupci su iz inserta na 5440.
if object_id(N'dbo.NetGenDBFilter', N'U') is null
begin
    create table dbo.NetGenDBFilter (
        Naslov varchar(200) null,
        OperaterID int null,
        Granula varchar(200) null,
        [sql] varchar(max) null
    );
end
