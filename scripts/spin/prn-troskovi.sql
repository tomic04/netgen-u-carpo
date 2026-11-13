-- Izvedeno iz definitions/netgen/BIOVEGA/PrnTroskovi.xml.
-- Gla je zaglavlje, ADDITEMS je podforma, ADDITEM je stavka. Jupiter se ne dira.
set nocount on;

if object_id(N'dbo.PrnTroskoviGla', N'U') is null
    create table dbo.PrnTroskoviGla (
        PrnTroskoviGlaId int identity(1, 1) not null constraint PK_PrnTroskoviGla primary key,
        KlMasterRobaId int null,
        RobaId int null
    );

if object_id(N'dbo.PrnTroskoviDet', N'U') is null
    create table dbo.PrnTroskoviDet (
        PrnTroskoviDetId int identity(1, 1) not null constraint PK_PrnTroskoviDet primary key,
        PrnTroskoviGlaId int not null,
        PrnVrstaTroskaId int null,
        OrgShemaId int null,
        Formula varchar(100) null,
        Vrijednost decimal(18, 4) null
    );
go

create or alter procedure dbo.spPrnTroskoviGlaQuery
    @Action int = null,
    @Id int = null
as
begin
    set nocount on;
    if @Action = 1
    begin
        select PrnTroskoviGlaId as id, KlMasterRobaId, RobaId
        from dbo.PrnTroskoviGla
        order by PrnTroskoviGlaId;
        return;
    end
    if @Action = 2
    begin
        if @Id is null or @Id = 0
            select top 1 PrnTroskoviGlaId, KlMasterRobaId, RobaId
            from dbo.PrnTroskoviGla
            where KlMasterRobaId is null and RobaId is null
            order by PrnTroskoviGlaId desc;
        else
            select PrnTroskoviGlaId, KlMasterRobaId, RobaId
            from dbo.PrnTroskoviGla
            where PrnTroskoviGlaId = @Id;
    end
end
go

create or alter procedure dbo.spPrnTroskoviGlaAzur
    @Action int = null,
    @Id int = null,
    @KlMasterRobaId int = null,
    @RobaId int = null,
    @searchboxklmasterrobaid varchar(100) = null,
    @searchboxrobaid varchar(100) = null
as
begin
    set nocount on;
    if @Action = 0
    begin
        insert into dbo.PrnTroskoviGla (KlMasterRobaId, RobaId) values (@KlMasterRobaId, @RobaId);
        return;
    end
    if @Action = 1
    begin
        update dbo.PrnTroskoviGla
        set KlMasterRobaId = @KlMasterRobaId, RobaId = @RobaId
        where PrnTroskoviGlaId = @Id;
        return;
    end
    if @Action = 2
    begin
        delete from dbo.PrnTroskoviDet where PrnTroskoviGlaId = @Id;
        delete from dbo.PrnTroskoviGla where PrnTroskoviGlaId = @Id;
    end
end
go

create or alter procedure dbo.spPrnTroskoviDetQuery
    @Action int = null,
    @Id int = null,
    @PrnTroskoviGlaId int = null
as
begin
    set nocount on;
    if @Action = 1
    begin
        select PrnTroskoviDetId, PrnTroskoviGlaId, PrnVrstaTroskaId, Formula, Vrijednost
        from dbo.PrnTroskoviDet
        where PrnTroskoviGlaId = @PrnTroskoviGlaId;
        return;
    end
    if @Action = 2
        select PrnVrstaTroskaId, Formula, Vrijednost
        from dbo.PrnTroskoviDet
        where PrnTroskoviDetId = @Id;
end
go

create or alter procedure dbo.spPrnTroskoviDetAzur
    @Action int = null,
    @Id int = null,
    @PrnTroskoviGlaId int = null,
    @PrnVrstaTroskaId int = null,
    @OrgShemaId int = null,
    @Formula varchar(100) = null,
    @Vrijednost decimal(18, 4) = null,
    @searchboxOrgShemaid varchar(100) = null
as
begin
    set nocount on;
    if @Action = 0
    begin
        if @PrnTroskoviGlaId is null
            select @PrnTroskoviGlaId = (
                select top 1 PrnTroskoviGlaId
                from dbo.PrnTroskoviGla
                where KlMasterRobaId is null and RobaId is null
                order by PrnTroskoviGlaId desc);
        insert into dbo.PrnTroskoviDet (PrnTroskoviGlaId, PrnVrstaTroskaId, OrgShemaId, Formula, Vrijednost)
        values (@PrnTroskoviGlaId, @PrnVrstaTroskaId, @OrgShemaId, @Formula, @Vrijednost);
        return;
    end
    if @Action = 1
    begin
        update dbo.PrnTroskoviDet
        set PrnVrstaTroskaId = @PrnVrstaTroskaId, Formula = @Formula, Vrijednost = @Vrijednost
        where PrnTroskoviDetId = @Id;
        return;
    end
    if @Action = 2
        delete from dbo.PrnTroskoviDet where PrnTroskoviDetId = @Id;
end
go
