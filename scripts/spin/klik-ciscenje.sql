-- Testni redovi klik-pregleda. Samo VrstaDok čiji je Opis klik-proba, i jezici klik-proba.
set nocount on;
delete from dbo.jezici where Naziv like N'klik-proba%' or Naziv like N'kp[0-9]%';

if object_id(N'dbo.Zupanije', N'U') is not null and col_length(N'dbo.Zupanije', N'Naziv') is not null
    exec(N'begin try delete from dbo.Zupanije where Naziv like N''kp[0-9]%'' or Naziv like N''klik-proba%''; end try begin catch print ''ciscenje zupanije: '' + error_message(); end catch');
if object_id(N'dbo.PrnVrstaTroska', N'U') is not null and col_length(N'dbo.PrnVrstaTroska', N'Naziv') is not null
    exec(N'begin try delete from dbo.PrnVrstaTroska where Naziv like N''kp[0-9]%'' or Naziv like N''klik-proba%''; end try begin catch print ''ciscenje troska: '' + error_message(); end catch');
if object_id(N'dbo.LnRobaWebKlase', N'U') is not null
    exec(N'begin try delete from dbo.LnRobaWebKlase where KlRobaWebKlaseId in (select KlRobaWebKlaseId from dbo.KlRobaWebKlase where Naziv like N''kp%''); end try begin catch print ''ciscenje veza: '' + error_message(); end catch');
if object_id(N'dbo.KlRobaWebKlase', N'U') is not null and col_length(N'dbo.KlRobaWebKlase', N'Naziv') is not null
    exec(N'begin try delete from dbo.KlRobaWebKlase where Naziv like N''kp%''; end try begin catch print ''ciscenje webklase: '' + error_message(); end catch');
if object_id(N'dbo.RatArkodKat', N'U') is not null and object_id(N'dbo.RatArkod', N'U') is not null
    exec(N'begin try delete from dbo.RatArkodKat where RatArkodId in (select RatArkodId from dbo.RatArkod where Naziv like N''kp%''); end try begin catch print ''ciscenje cestica: '' + error_message(); end catch');
if object_id(N'dbo.RatArkod', N'U') is not null and col_length(N'dbo.RatArkod', N'Naziv') is not null
    exec(N'begin try delete from dbo.RatArkod where Naziv like N''kp%''; end try begin catch print ''ciscenje arkod: '' + error_message(); end catch');
if object_id(N'dbo.PrnTroskoviDet', N'U') is not null and object_id(N'dbo.PrnTroskoviGla', N'U') is not null
    exec(N'begin try delete from dbo.PrnTroskoviDet where PrnTroskoviGlaId in (select PrnTroskoviGlaId from dbo.PrnTroskoviGla where KlMasterRobaId is null and RobaId is null); end try begin catch print ''ciscenje stavki: '' + error_message(); end catch');
if object_id(N'dbo.PrnTroskoviGla', N'U') is not null
    exec(N'begin try delete from dbo.PrnTroskoviGla where KlMasterRobaId is null and RobaId is null; end try begin catch print ''ciscenje troskova: '' + error_message(); end catch');

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
