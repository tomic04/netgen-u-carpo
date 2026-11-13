-- Testni redovi klik-pregleda. Samo VrstaDok čiji je Opis klik-proba, i jezici klik-proba.
set nocount on;
delete from dbo.jezici where Naziv like N'klik-proba%' or Naziv like N'kp[0-9]%';

if object_id(N'dbo.Zupanije', N'U') is not null and col_length(N'dbo.Zupanije', N'Naziv') is not null
    exec(N'begin try delete from dbo.Zupanije where Naziv like N''kp[0-9]%'' or Naziv like N''klik-proba%''; end try begin catch print ''ciscenje zupanije: '' + error_message(); end catch');
if object_id(N'dbo.PrnVrstaTroska', N'U') is not null and col_length(N'dbo.PrnVrstaTroska', N'Naziv') is not null
    exec(N'begin try delete from dbo.PrnVrstaTroska where Naziv like N''kp[0-9]%'' or Naziv like N''klik-proba%''; end try begin catch print ''ciscenje troska: '' + error_message(); end catch');

if object_id(N'dbo.VrstaDok', N'U') is null or col_length(N'dbo.VrstaDok', N'Opis') is null
    return;

declare @sql nvarchar(max) = N'
declare @vd table (id int);
insert into @vd select VrstaDokId from dbo.VrstaDok where Opis = N''klik-proba'';
';
if object_id(N'dbo.StatusiNext', N'U') is not null
    set @sql = @sql + N'
delete from dbo.StatusiNext where StatusiId in (select statusiid from dbo.statusi where VrstaDokId in (select id from @vd) or Korak like N''klik-proba%'' or Korak like N''kp[0-9]%'');
';
if object_id(N'dbo.StatusiAudit', N'U') is not null
    set @sql = @sql + N'
delete from dbo.StatusiAudit where VrstaDokId in (select id from @vd);
';
if object_id(N'dbo.statusi', N'U') is not null
    set @sql = @sql + N'
delete from dbo.statusi where VrstaDokId in (select id from @vd) or Korak like N''klik-proba%'' or Korak like N''kp[0-9]%'';
';
set @sql = @sql + N'
delete from dbo.VrstaDok where VrstaDokId in (select id from @vd);
';
exec(@sql);
