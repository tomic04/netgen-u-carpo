-- Izvedeno iz definitions/netgen/BIOVEGA/PrnVrstaTroska.xml.
-- Query @Action=1 puni master (id, Naziv, Sifra, PrnTipObr).
-- Query @Action=2, @Id puni UPDATE ONLOAD u kontrole Naziv, Sifra, PrnTipObr.
-- Azur @Action 0/1/2 su Unesi, Izmjena i Brisanje. Jupiter se ne dira.
set nocount on;

if object_id(N'dbo.PrnTipObr', N'U') is null
    create table dbo.PrnTipObr (
        PrnTipObr varchar(20) not null constraint PK_PrnTipObr primary key,
        Naziv varchar(50) null
    );

if not exists (select 1 from dbo.PrnTipObr where PrnTipObr = N'T1')
    insert into dbo.PrnTipObr (PrnTipObr, Naziv) values (N'T1', N'Test obracun');

if object_id(N'dbo.PrnVrstaTroska', N'U') is null
    create table dbo.PrnVrstaTroska (
        PrnVrstaTroskaId int identity(1, 1) not null constraint PK_PrnVrstaTroska primary key,
        Naziv varchar(50) null,
        Sifra varchar(20) null,
        PrnTipObr varchar(20) null
    );

go

create or alter procedure dbo.spPrnVrstaTroskaQuery
    @Action int = null,
    @Id int = null
as
begin
    set nocount on;
    if @Action = 1
    begin
        select PrnVrstaTroskaId as id, Naziv, Sifra, PrnTipObr
        from dbo.PrnVrstaTroska
        order by Naziv, PrnVrstaTroskaId;
        return;
    end
    if @Action = 2
    begin
        select Naziv, Sifra, PrnTipObr
        from dbo.PrnVrstaTroska
        where PrnVrstaTroskaId = @Id;
        return;
    end
end
go

create or alter procedure dbo.spPrnVrstaTroskaAzur
    @Action int = null,
    @Id int = null,
    @Naziv varchar(50) = null,
    @Sifra varchar(20) = null,
    @PrnTipObr varchar(20) = null
as
begin
    set nocount on;
    if @Action = 0
    begin
        insert into dbo.PrnVrstaTroska (Naziv, Sifra, PrnTipObr)
        values (@Naziv, @Sifra, @PrnTipObr);
        return;
    end
    if @Action = 1
    begin
        update dbo.PrnVrstaTroska
        set Naziv = @Naziv, Sifra = @Sifra, PrnTipObr = @PrnTipObr
        where PrnVrstaTroskaId = @Id;
        return;
    end
    if @Action = 2
    begin
        delete from dbo.PrnVrstaTroska where PrnVrstaTroskaId = @Id;
        return;
    end
end
go
