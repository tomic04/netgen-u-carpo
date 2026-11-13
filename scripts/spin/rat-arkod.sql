-- Izvedeno iz definitions/netgen/BIOVEGA/RatArkod.xml.
-- main puni master, id puni UPDATE, katcestice je detail i grid stavke.
-- Unos ubaci i jednu česticu da detail ima red. Jupiter se ne dira.
set nocount on;

if object_id(N'dbo.Numbers', N'U') is null
begin
    create table dbo.Numbers (n int not null constraint PK_Numbers primary key);
    declare @n int = 2018;
    while @n <= 2032
    begin
        insert into dbo.Numbers (n) values (@n);
        set @n = @n + 1;
    end
end

if object_id(N'dbo.Tabla', N'U') is null
    create table dbo.Tabla (
        TablaId int identity(1, 1) not null constraint PK_TablaKlik primary key,
        Sifra varchar(20) null,
        Naziv varchar(100) null
    );
if object_id(N'dbo.Tabla', N'U') is not null and col_length(N'dbo.Tabla', N'Naziv') is not null
    exec(N'begin try if not exists (select 1 from dbo.Tabla where Naziv = N''kp-tabla'') insert into dbo.Tabla (Sifra, Naziv) values (N''1'', N''kp-tabla''); end try begin catch print ''tabla: '' + error_message(); end catch');

if object_id(N'dbo.RatVrstaUporabe', N'U') is null
    create table dbo.RatVrstaUporabe (
        RatVrstaUporabeId int identity(1, 1) not null primary key,
        Naziv varchar(100) null
    );
exec(N'begin try if not exists (select 1 from dbo.RatVrstaUporabe where Naziv = N''kp-uporaba'') insert into dbo.RatVrstaUporabe (Naziv) values (N''kp-uporaba''); end try begin catch print ''uporaba: '' + error_message(); end catch');

if object_id(N'dbo.RatVrstaTla', N'U') is null
    create table dbo.RatVrstaTla (
        RatVrstaTlaId int identity(1, 1) not null primary key,
        Naziv varchar(100) null
    );
exec(N'begin try if not exists (select 1 from dbo.RatVrstaTla where Naziv = N''kp-tlo'') insert into dbo.RatVrstaTla (Naziv) values (N''kp-tlo''); end try begin catch print ''tlo: '' + error_message(); end catch');

if object_id(N'dbo.RatVrstaProizvodnje', N'U') is null
    create table dbo.RatVrstaProizvodnje (
        RatVrstaProizvodnjeId int identity(1, 1) not null primary key,
        Naziv varchar(100) null
    );
exec(N'begin try if not exists (select 1 from dbo.RatVrstaProizvodnje where Naziv = N''kp-proizvodnja'') insert into dbo.RatVrstaProizvodnje (Naziv) values (N''kp-proizvodnja''); end try begin catch print ''proizvodnja: '' + error_message(); end catch');

if object_id(N'dbo.RatArkod', N'U') is not null and col_length(N'dbo.RatArkod', N'Url') is null
    alter table dbo.RatArkod add Url varchar(200) null;

if object_id(N'dbo.RatArkod', N'U') is null
    create table dbo.RatArkod (
        RatArkodId int identity(1, 1) not null constraint PK_RatArkod primary key,
        Oznaka varchar(30) null,
        Naziv varchar(100) null,
        Povrsina decimal(12, 2) null,
        OrgShemaId int null,
        TablaId int null,
        Godina int null,
        MjestaId int null,
        RatVrstaUporabeId int null,
        RatVrstaTlaId int null,
        RatVrstaProizvodnjeId int null,
        IsTUG bit null,
        IsNavodnjavanje bit null,
        IsRazminirano bit null,
        Aktivan bit null,
        Napomena varchar(200) null,
        Url varchar(200) null
    );

if object_id(N'dbo.RatArkodKat', N'U') is null
    create table dbo.RatArkodKat (
        RatArkodKatId int identity(1, 1) not null primary key,
        RatArkodId int not null,
        Broj varchar(20) null,
        Povrsina decimal(12, 2) null,
        PovrsinaArkod decimal(12, 2) null
    );
go

if object_id(N'dbo.fnRatArkod_Url', N'FN') is null
    exec(N'create function dbo.fnRatArkod_Url(@oznaka varchar(30)) returns varchar(200) as
begin
    return N''preglednik.arkod.hr/'' + isnull(@oznaka, N'''');
end');
go

create or alter procedure dbo.spRatVrstaUporabe_Query
    @action varchar(20) = null
as
begin
    set nocount on;
    if @action = 'combo'
        select RatVrstaUporabeId, Naziv from dbo.RatVrstaUporabe order by Naziv;
end
go

create or alter procedure dbo.spRatVrstaTla_Query
    @action varchar(20) = null
as
begin
    set nocount on;
    if @action = 'combo'
        select RatVrstaTlaId, Naziv from dbo.RatVrstaTla order by Naziv;
end
go

if object_id(N'dbo.spOrgShemaQuery', N'P') is null
    exec(N'create procedure dbo.spOrgShemaQuery @action int = null, @id int = null, @naziv varchar(100) = null as
begin
    set nocount on;
    if object_id(N''dbo.OrgShema'', N''U'') is null return;
    select top 20 OrgShemaId, Naziv from dbo.OrgShema where (@naziv is null or Naziv like N''%'' + @naziv + N''%'');
end');
go

create or alter procedure dbo.spRatArkod_Query
    @action varchar(20) = null,
    @id int = null,
    @godina int = null
as
begin
    set nocount on;
    if @action = 'main'
    begin
        select a.RatArkodId as id, a.Naziv, a.Povrsina as [Površina], a.Godina, a.Napomena, a.Url as URL
        from dbo.RatArkod a
        where @godina is null or a.Godina = @godina
        order by a.RatArkodId;
        return;
    end
    if @action = 'id'
    begin
        select Oznaka, Naziv, Povrsina, OrgShemaId, TablaId, Godina, MjestaId,
            RatVrstaUporabeId, RatVrstaTlaId, RatVrstaProizvodnjeId,
            IsTUG, IsNavodnjavanje, IsRazminirano, Aktivan, Napomena, Url
        from dbo.RatArkod where RatArkodId = @id;
        return;
    end
    if @action = 'katcestice'
    begin
        select Broj, Povrsina as [Površina], PovrsinaArkod as [Površina arkod]
        from dbo.RatArkodKat where RatArkodId = @id;
    end
end
go

create or alter procedure dbo.spRatArkod_Azur
    @action varchar(20) = null,
    @id int = null,
    @oznaka varchar(30) = null,
    @naziv varchar(100) = null,
    @povrsina decimal(12, 2) = null,
    @orgshemaid int = null,
    @tablaid int = null,
    @godina int = null,
    @mjestaid int = null,
    @ratvrstauporabeid int = null,
    @ratvrstatlaid int = null,
    @istug int = null,
    @ratvrstaproizvodnjeid int = null,
    @isnavodnjavanje int = null,
    @israzminirano int = null,
    @aktivan int = null,
    @napomena varchar(200) = null
as
begin
    set nocount on;
    if @action = 'insert'
    begin
        insert into dbo.RatArkod (
            Oznaka, Naziv, Povrsina, OrgShemaId, TablaId, Godina, MjestaId,
            RatVrstaUporabeId, RatVrstaTlaId, RatVrstaProizvodnjeId,
            IsTUG, IsNavodnjavanje, IsRazminirano, Aktivan, Napomena, Url)
        values (
            @oznaka, @naziv, @povrsina, @orgshemaid, @tablaid, @godina, @mjestaid,
            @ratvrstauporabeid, @ratvrstatlaid, @ratvrstaproizvodnjeid,
            @istug, @isnavodnjavanje, @israzminirano, @aktivan, @napomena,
            N'preglednik.arkod.hr/' + isnull(@oznaka, N''));
        declare @novi int = scope_identity();
        insert into dbo.RatArkodKat (RatArkodId, Broj, Povrsina, PovrsinaArkod)
        values (@novi, N'1', 0.10, 0.10);
        return;
    end
    if @action = 'update'
    begin
        update dbo.RatArkod
        set Oznaka = @oznaka, Naziv = @naziv, Povrsina = @povrsina, Godina = @godina, Napomena = @napomena
        where RatArkodId = @id;
        return;
    end
    if @action = 'delete'
    begin
        delete from dbo.RatArkodKat where RatArkodId = @id;
        delete from dbo.RatArkod where RatArkodId = @id;
    end
end
go
