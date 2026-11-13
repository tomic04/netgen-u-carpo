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
    -- Postojeća RatArkod ima stupce kojih XML ne nabraja. Piše se samo ono što postoji,
    -- a obavezni stupac bez defaulta dobije praznu vrijednost da Unesi prođe.
    declare @c nvarchar(max) = N'';
    declare @v nvarchar(max) = N'';
    declare @sql nvarchar(max);
    declare @novi int;
    if @action = 'insert'
    begin
        if col_length(N'dbo.RatArkod', N'Oznaka') is not null select @c += N',[Oznaka]', @v += N',@oznaka';
        if col_length(N'dbo.RatArkod', N'Naziv') is not null select @c += N',[Naziv]', @v += N',@naziv';
        if col_length(N'dbo.RatArkod', N'Povrsina') is not null select @c += N',[Povrsina]', @v += N',@povrsina';
        if col_length(N'dbo.RatArkod', N'OrgShemaId') is not null select @c += N',[OrgShemaId]', @v += N',@orgshemaid';
        if col_length(N'dbo.RatArkod', N'TablaId') is not null select @c += N',[TablaId]', @v += N',@tablaid';
        if col_length(N'dbo.RatArkod', N'Godina') is not null select @c += N',[Godina]', @v += N',@godina';
        if col_length(N'dbo.RatArkod', N'MjestaId') is not null select @c += N',[MjestaId]', @v += N',@mjestaid';
        if col_length(N'dbo.RatArkod', N'RatVrstaUporabeId') is not null select @c += N',[RatVrstaUporabeId]', @v += N',@ratvrstauporabeid';
        if col_length(N'dbo.RatArkod', N'RatVrstaTlaId') is not null select @c += N',[RatVrstaTlaId]', @v += N',@ratvrstatlaid';
        if col_length(N'dbo.RatArkod', N'RatVrstaProizvodnjeId') is not null select @c += N',[RatVrstaProizvodnjeId]', @v += N',@ratvrstaproizvodnjeid';
        if col_length(N'dbo.RatArkod', N'IsTUG') is not null select @c += N',[IsTUG]', @v += N',@istug';
        if col_length(N'dbo.RatArkod', N'IsNavodnjavanje') is not null select @c += N',[IsNavodnjavanje]', @v += N',@isnavodnjavanje';
        if col_length(N'dbo.RatArkod', N'IsRazminirano') is not null select @c += N',[IsRazminirano]', @v += N',@israzminirano';
        if col_length(N'dbo.RatArkod', N'Aktivan') is not null select @c += N',[Aktivan]', @v += N',@aktivan';
        if col_length(N'dbo.RatArkod', N'Napomena') is not null select @c += N',[Napomena]', @v += N',@napomena';
        if col_length(N'dbo.RatArkod', N'Url') is not null select @c += N',[Url]', @v += N',N''preglednik.arkod.hr/'' + isnull(@oznaka, N'''')';
        select @c += N',[' + c.name + N']',
            @v += N',' + case
                when ty.name in (N'bit', N'int', N'smallint', N'tinyint', N'bigint', N'decimal', N'numeric', N'money', N'smallmoney', N'float', N'real') then N'0'
                when ty.name in (N'datetime', N'datetime2', N'smalldatetime', N'date', N'datetimeoffset') then N'getdate()'
                when ty.name = N'uniqueidentifier' then N'newid()'
                when ty.name in (N'binary', N'varbinary', N'image') then N'0x'
                else N'N'''''
            end
        from sys.columns c
        inner join sys.types ty on ty.user_type_id = c.user_type_id
        where c.object_id = object_id(N'dbo.RatArkod')
          and c.is_identity = 0 and c.is_computed = 0 and c.is_nullable = 0
          and c.default_object_id = 0
          and ty.name not in (N'timestamp', N'rowversion')
          and charindex(N'[' + c.name + N']', @c) = 0;
        if len(@c) > 1
        begin
            set @sql = N'insert into dbo.RatArkod (' + substring(@c, 2, len(@c)) + N') values (' + substring(@v, 2, len(@v)) + N'); set @out = scope_identity();';
            exec sp_executesql @sql,
                N'@oznaka varchar(30), @naziv varchar(100), @povrsina decimal(12,2), @orgshemaid int, @tablaid int, @godina int, @mjestaid int, @ratvrstauporabeid int, @ratvrstatlaid int, @ratvrstaproizvodnjeid int, @istug int, @isnavodnjavanje int, @israzminirano int, @aktivan int, @napomena varchar(200), @out int output',
                @oznaka, @naziv, @povrsina, @orgshemaid, @tablaid, @godina, @mjestaid, @ratvrstauporabeid, @ratvrstatlaid, @ratvrstaproizvodnjeid, @istug, @isnavodnjavanje, @israzminirano, @aktivan, @napomena, @novi output;
        end
        if @novi is not null and object_id(N'dbo.RatArkodKat', N'U') is not null and col_length(N'dbo.RatArkodKat', N'RatArkodId') is not null
        begin
            declare @kc nvarchar(max) = N',[RatArkodId]';
            declare @kv nvarchar(max) = N',@novi';
            if col_length(N'dbo.RatArkodKat', N'Broj') is not null select @kc += N',[Broj]', @kv += N',N''1''';
            if col_length(N'dbo.RatArkodKat', N'Povrsina') is not null select @kc += N',[Povrsina]', @kv += N',0.10';
            if col_length(N'dbo.RatArkodKat', N'PovrsinaArkod') is not null select @kc += N',[PovrsinaArkod]', @kv += N',0.10';
            select @kc += N',[' + c.name + N']',
                @kv += N',' + case
                    when ty.name in (N'bit', N'int', N'smallint', N'tinyint', N'bigint', N'decimal', N'numeric', N'money', N'smallmoney', N'float', N'real') then N'0'
                    when ty.name in (N'datetime', N'datetime2', N'smalldatetime', N'date', N'datetimeoffset') then N'getdate()'
                    else N'N'''''
                end
            from sys.columns c
            inner join sys.types ty on ty.user_type_id = c.user_type_id
            where c.object_id = object_id(N'dbo.RatArkodKat')
              and c.is_identity = 0 and c.is_computed = 0 and c.is_nullable = 0
              and c.default_object_id = 0
              and ty.name not in (N'timestamp', N'rowversion')
              and charindex(N'[' + c.name + N']', @kc) = 0;
            set @sql = N'insert into dbo.RatArkodKat (' + substring(@kc, 2, len(@kc)) + N') values (' + substring(@kv, 2, len(@kv)) + N')';
            exec sp_executesql @sql, N'@novi int', @novi;
        end
        return;
    end
    if @action = 'update'
    begin
        set @c = N'';
        if col_length(N'dbo.RatArkod', N'Oznaka') is not null set @c += N',Oznaka=@oznaka';
        if col_length(N'dbo.RatArkod', N'Naziv') is not null set @c += N',Naziv=@naziv';
        if col_length(N'dbo.RatArkod', N'Povrsina') is not null set @c += N',Povrsina=@povrsina';
        if col_length(N'dbo.RatArkod', N'Godina') is not null set @c += N',Godina=@godina';
        if col_length(N'dbo.RatArkod', N'Napomena') is not null set @c += N',Napomena=@napomena';
        if len(@c) > 1
        begin
            set @sql = N'update dbo.RatArkod set ' + substring(@c, 2, len(@c)) + N' where RatArkodId=@id';
            exec sp_executesql @sql,
                N'@id int, @oznaka varchar(30), @naziv varchar(100), @povrsina decimal(12,2), @godina int, @napomena varchar(200)',
                @id, @oznaka, @naziv, @povrsina, @godina, @napomena;
        end
        return;
    end
    if @action = 'delete'
    begin
        select @sql = coalesce(@sql, N'') + N'delete from ' + quotename(isnull(object_schema_name(fk.parent_object_id), N'dbo')) + N'.' + quotename(object_name(fk.parent_object_id))
            + N' where ' + quotename(pc.name) + N' = @id; '
        from sys.foreign_keys fk
        inner join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
        inner join sys.columns pc on pc.object_id = fkc.parent_object_id and pc.column_id = fkc.parent_column_id
        where fk.referenced_object_id = object_id(N'dbo.RatArkod');
        if @sql is not null
            exec sp_executesql @sql, N'@id int', @id;
        if object_id(N'dbo.RatArkodKat', N'U') is not null and col_length(N'dbo.RatArkodKat', N'RatArkodId') is not null
            delete from dbo.RatArkodKat where RatArkodId = @id;
        delete from dbo.RatArkod where RatArkodId = @id;
    end
end
go
