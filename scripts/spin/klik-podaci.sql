-- Podaci i procedure za klik Unesi/Izmijeni/Obriši. Nije klijentski katalog.
-- Jupiter se ne dira. Ako je procedura prava (ima insert u jezici / statusi), tijelo ostaje.

set nocount on;

declare @d nvarchar(max);
declare @i int;

set @d = object_definition(object_id(N'dbo.spJeziciAzur'));
print 'DEF-JEZICI-START';
if @d is null
    print 'NEMA';
else
begin
    set @i = 1;
    while @i <= len(@d)
    begin
        print substring(@d, @i, 3000);
        set @i = @i + 3000;
    end
end
print 'DEF-JEZICI-END';

if @d is null or @d not like '%insert%jezici%'
begin
    print 'spJeziciAzur je stub ili nema insert u jezici. Pise se iz JeziciNG.xml: action 0 insert, 1 update, 2 delete.';
    exec(N'
create or alter procedure dbo.spJeziciAzur
    @action int = null,
    @id int = null,
    @Naziv varchar(100) = null
as
begin
    set nocount on;
    if @action = 0
    begin
        if exists (select 1 from sys.columns where object_id = object_id(N''dbo.jezici'') and name = N''JeziciId'' and is_identity = 1)
            insert into dbo.jezici (Naziv) values (@Naziv);
        else
            insert into dbo.jezici (JeziciId, Naziv)
            select isnull(max(JeziciId), 0) + 1, @Naziv from dbo.jezici;
        return;
    end
    if @action = 1
    begin
        update dbo.jezici set Naziv = @Naziv where JeziciId = @id;
        return;
    end
    if @action = 2
    begin
        delete from dbo.jezici where JeziciId = @id;
        return;
    end
end');
end
else
    print 'spJeziciAzur je prava. Tijelo se ne mijenja.';

set @d = object_definition(object_id(N'dbo.spStatusiAzur'));
print 'DEF-STATUSI-AZUR-START';
if @d is null print 'NEMA';
else
begin
    set @i = 1;
    while @i <= len(@d)
    begin
        print substring(@d, @i, 3000);
        set @i = @i + 3000;
    end
end
print 'DEF-STATUSI-AZUR-END';

set @d = object_definition(object_id(N'dbo.spStatusiQuery'));
print 'DEF-STATUSI-QUERY-START';
if @d is null print 'NEMA';
else
begin
    set @i = 1;
    while @i <= len(@d) and @i < 6001
    begin
        print substring(@d, @i, 3000);
        set @i = @i + 3000;
    end
end
print 'DEF-STATUSI-QUERY-END';

-- Testni VrstaDok i jedan Statusi. Stupci koji ne postoje se ne spominju.
declare @vd int = null;
if object_id(N'dbo.VrstaDok', N'U') is not null
begin
    declare @ima int = 0;
    declare @traz nvarchar(max) = N'select @n = count(*) from dbo.VrstaDok where 1=0';
    if col_length(N'dbo.VrstaDok', N'Opis') is not null
        set @traz = N'select @n = count(*) from dbo.VrstaDok where Opis = N''klik-proba''';
    else if col_length(N'dbo.VrstaDok', N'Naziv') is not null
        set @traz = N'select @n = count(*) from dbo.VrstaDok where Naziv = N''klik-proba''';
    exec sp_executesql @traz, N'@n int output', @n = @ima output;
    if @ima = 0
    begin
        declare @cols nvarchar(max) = N'';
        declare @vals nvarchar(max) = N'';
        select
            @cols = @cols + N',' + quotename(c.name),
            @vals = @vals + N',' + case
                when c.name in (N'Opis', N'Naziv', N'Sifra') then N'N''klik-proba'''
                when ty.name in (N'int', N'smallint', N'tinyint', N'bigint') then N'1'
                when ty.name = N'bit' then N'1'
                when ty.name in (N'date', N'datetime', N'datetime2', N'smalldatetime') then N'getdate()'
                when ty.name in (N'varchar', N'nvarchar', N'char', N'nchar') then N'N''klik-proba'''
                else N'0'
            end
        from sys.columns c
        join sys.types ty on c.user_type_id = ty.user_type_id
        where c.object_id = object_id(N'dbo.VrstaDok')
          and c.is_identity = 0
          and c.is_computed = 0
          and (c.is_nullable = 0 or c.name in (N'Opis', N'Naziv'));
        if len(@cols) > 1
        begin
            declare @sql nvarchar(max) = N'insert into dbo.VrstaDok (' + substring(@cols, 2, 4000) + N') values (' + substring(@vals, 2, 4000) + N')';
            print @sql;
            exec(@sql);
        end
    end
    if col_length(N'dbo.VrstaDok', N'VrstaDokId') is not null
    begin
        if col_length(N'dbo.VrstaDok', N'Opis') is not null
            exec sp_executesql N'select @x = (select top 1 VrstaDokId from dbo.VrstaDok where Opis = N''klik-proba'')', N'@x int output', @x = @vd output;
        else if col_length(N'dbo.VrstaDok', N'Naziv') is not null
            exec sp_executesql N'select @x = (select top 1 VrstaDokId from dbo.VrstaDok where Naziv = N''klik-proba'')', N'@x int output', @x = @vd output;
    end
end
print 'TEST-VRSTADOK ' + isnull(cast(@vd as varchar(20)), 'nema');

if @vd is not null and object_id(N'dbo.statusi', N'U') is not null and col_length(N'dbo.statusi', N'Korak') is not null
begin
    declare @imaS int = 0;
    exec sp_executesql N'select @n = count(*) from dbo.statusi where Korak = N''klik-proba-seed''', N'@n int output', @n = @imaS output;
    if @imaS = 0
    begin
        declare @sc nvarchar(max) = N'';
        declare @sv nvarchar(max) = N'';
        select
            @sc = @sc + N',' + quotename(c.name),
            @sv = @sv + N',' + case
                when c.name = N'VrstaDokId' then cast(@vd as nvarchar(20))
                when c.name = N'Korak' then N'N''klik-proba-seed'''
                when c.name in (N'Redoslijed', N'redoslijed') then N'1'
                when ty.name = N'bit' or c.name in (N'smije', N'zakljuceno', N'aktivan', N'obavezan') then N'1'
                when ty.name in (N'int', N'smallint', N'tinyint', N'bigint') then N'1'
                when ty.name in (N'varchar', N'nvarchar', N'char', N'nchar') then N'N''klik-proba-seed'''
                when ty.name in (N'date', N'datetime', N'datetime2', N'smalldatetime') then N'getdate()'
                else N'0'
            end
        from sys.columns c
        join sys.types ty on c.user_type_id = ty.user_type_id
        where c.object_id = object_id(N'dbo.statusi')
          and c.is_identity = 0
          and c.is_computed = 0
          and (c.is_nullable = 0 or c.name in (N'Korak', N'VrstaDokId', N'Redoslijed', N'redoslijed'));
        if len(@sc) > 1
        begin
            declare @ins nvarchar(max) = N'insert into dbo.statusi (' + substring(@sc, 2, 4000) + N') values (' + substring(@sv, 2, 4000) + N')';
            print @ins;
            exec(@ins);
        end
    end
end
