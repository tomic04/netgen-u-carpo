-- Izvedeno iz definitions/netgen/BIOVEGA/RobaWebKlase.xml i Form1.vb 4932–5032.
-- Stablo: id, parent, natpis, tag. Master PUNI2 je roba. MULTIBLUE čita TotalGenMultiSelectTemp.
-- Jupiter se ne dira. Postojeća Roba se ne briše.
set nocount on;

if object_id(N'dbo.KlRobaWebKlase', N'U') is null
    create table dbo.KlRobaWebKlase (
        KlRobaWebKlaseId int identity(1, 1) not null constraint PK_KlRobaWebKlase primary key,
        Naziv varchar(100) null,
        Opis varchar(400) null,
        SortKey varchar(20) not null constraint DF_KlRobaWebKlase_Sort default (N'00')
    );

if object_id(N'dbo.Roba', N'U') is null
    create table dbo.Roba (
        RobaId int identity(1, 1) not null constraint PK_RobaKlik primary key,
        Naziv varchar(100) null
    );

if object_id(N'dbo.LnRobaWebKlase', N'U') is null
    create table dbo.LnRobaWebKlase (
        LnRobaWebKlaseId int identity(1, 1) not null constraint PK_LnRobaWebKlase primary key,
        KlRobaWebKlaseId int not null,
        RobaId int not null
    );

if object_id(N'dbo.TotalGenMultiSelectTemp', N'U') is null
    create table dbo.TotalGenMultiSelectTemp (
        id int null,
        hostname varchar(128) null,
        susersname varchar(128) null,
        operateriid int null
    );

if col_length(N'dbo.Roba', N'Naziv') is not null
    exec(N'begin try
        if not exists (select 1 from dbo.Roba where Naziv = N''kp-web-roba-1'')
            insert into dbo.Roba (Naziv) values (N''kp-web-roba-1''), (N''kp-web-roba-2'');
    end try begin catch print ''roba seed: '' + error_message(); end catch');

if not exists (select 1 from dbo.KlRobaWebKlase where Naziv = N'kp-web-seed')
    insert into dbo.KlRobaWebKlase (Naziv, Opis, SortKey) values (N'kp-web-seed', N'seed', N'00');
go

create or alter procedure dbo.spKlRobaWebKlase_Tree
as
begin
    set nocount on;
    select 0 as id, 0 as parentid, N'Web klase' as caption, N'0' as value
    union all
    select KlRobaWebKlaseId, 0, Naziv, cast(KlRobaWebKlaseId as varchar(20))
    from dbo.KlRobaWebKlase;
end
go

create or alter procedure dbo.spKlRobaWebKlase_Query
    @action varchar(20) = null,
    @id int = null
as
begin
    set nocount on;
    if @action = 'UPDATE'
        select Naziv as naziv, Opis as opis, cast(null as int) as nadklasaid
        from dbo.KlRobaWebKlase
        where KlRobaWebKlaseId = @id;
end
go

create or alter procedure dbo.spKlRobaWebKlase_Azur
    @action varchar(20) = null,
    @id int = null,
    @naziv varchar(100) = null,
    @opis varchar(400) = null,
    @nadklasaid int = null
as
begin
    set nocount on;
    if @action = 'INSERT'
    begin
        insert into dbo.KlRobaWebKlase (Naziv, Opis, SortKey)
        values (@naziv, @opis, N'00');
        return;
    end
    if @action = 'UPDATE'
    begin
        update dbo.KlRobaWebKlase set Naziv = @naziv, Opis = @opis where KlRobaWebKlaseId = @id;
        return;
    end
    if @action = 'DELETE'
    begin
        delete from dbo.LnRobaWebKlase where KlRobaWebKlaseId = @id;
        delete from dbo.KlRobaWebKlase where KlRobaWebKlaseId = @id;
    end
end
go

create or alter procedure dbo.spLnRobaWebKlase_Query
    @action varchar(20) = null,
    @klrobawebklaseid int = null
as
begin
    set nocount on;
    if @action = 'SVA'
    begin
        select top 5 r.RobaId as id, r.Naziv
        from dbo.Roba r
        order by r.RobaId;
        return;
    end
    if @action = 'U_KLASI'
    begin
        select r.RobaId as id, r.Naziv
        from dbo.LnRobaWebKlase l
        inner join dbo.Roba r on r.RobaId = l.RobaId
        where l.KlRobaWebKlaseId = @klrobawebklaseid;
        return;
    end
    if @action = 'NE_U_KLASI'
    begin
        select top 5 r.RobaId as id, r.Naziv
        from dbo.Roba r
        where not exists (
              select 1 from dbo.LnRobaWebKlase l
              where l.RobaId = r.RobaId and l.KlRobaWebKlaseId = @klrobawebklaseid);
    end
end
go

create or alter procedure dbo.spLnRobaWebKlase_Azur
    @action varchar(20) = null,
    @klrobawebklaseid int = null,
    @operateriid int = null,
    @robaid int = null,
    @izklase int = null,
    @uklasu int = null
as
begin
    set nocount on;
    if @action = 'MULTI_INSERT'
    begin
        insert into dbo.LnRobaWebKlase (KlRobaWebKlaseId, RobaId)
        select @klrobawebklaseid, t.id
        from dbo.TotalGenMultiSelectTemp t
        where t.operateriid = @operateriid
          and t.id is not null
          and not exists (
              select 1 from dbo.LnRobaWebKlase l
              where l.KlRobaWebKlaseId = @klrobawebklaseid and l.RobaId = t.id);
        return;
    end
    if @action = 'MULTI_DELETE'
    begin
        delete l
        from dbo.LnRobaWebKlase l
        inner join dbo.TotalGenMultiSelectTemp t on t.id = l.RobaId and t.operateriid = @operateriid
        where l.KlRobaWebKlaseId = @klrobawebklaseid;
        return;
    end
    if @action = 'INSERT'
    begin
        if @robaid is not null and not exists (
            select 1 from dbo.LnRobaWebKlase where KlRobaWebKlaseId = @klrobawebklaseid and RobaId = @robaid)
            insert into dbo.LnRobaWebKlase (KlRobaWebKlaseId, RobaId) values (@klrobawebklaseid, @robaid);
        return;
    end
    if @action = 'DELETE'
    begin
        delete from dbo.LnRobaWebKlase where KlRobaWebKlaseId = @klrobawebklaseid and RobaId = @robaid;
        return;
    end
    if @action = 'COPY'
    begin
        insert into dbo.LnRobaWebKlase (KlRobaWebKlaseId, RobaId)
        select @uklasu, l.RobaId
        from dbo.LnRobaWebKlase l
        where l.KlRobaWebKlaseId = @izklase
          and not exists (
              select 1 from dbo.LnRobaWebKlase x
              where x.KlRobaWebKlaseId = @uklasu and x.RobaId = l.RobaId);
        return;
    end
    if @action = 'INSERT_ALL'
    begin
        insert into dbo.LnRobaWebKlase (KlRobaWebKlaseId, RobaId)
        select @klrobawebklaseid, r.RobaId
        from dbo.Roba r
        where r.Naziv like N'kp-web-roba%'
          and not exists (
              select 1 from dbo.LnRobaWebKlase l
              where l.KlRobaWebKlaseId = @klrobawebklaseid and l.RobaId = r.RobaId);
        return;
    end
    if @action = 'DELETE_ALL'
        delete from dbo.LnRobaWebKlase where KlRobaWebKlaseId = @klrobawebklaseid;
end
go

if object_id(N'dbo.spMPRobaZaMPNG', N'P') is null
    exec(N'create procedure dbo.spMPRobaZaMPNG @trgovinaid int = null, @trazi varchar(100) = null as
begin
    set nocount on;
    select top 20 RobaId, Naziv from dbo.Roba where Naziv like N''%'' + isnull(@trazi, N'''') + N''%'';
end');
go
