-- Testni redovi klik-pregleda na spinu. Nije klijentski podatak.
-- Briše samo Naziv koji je klik sam upisao. Tuđi jezici ostaju.
delete from jezici where Naziv like N'klik-proba%';
if object_id(N'dbo.StatusiNext', N'U') is not null and object_id(N'dbo.statusi', N'U') is not null
    delete from dbo.StatusiNext
    where StatusiId in (select statusiid from dbo.statusi where Korak like N'klik-proba%');
if object_id(N'dbo.statusi', N'U') is not null
    delete from dbo.statusi where Korak like N'klik-proba%';
if object_id(N'dbo.VrstaDok', N'U') is not null and col_length(N'dbo.VrstaDok', N'Opis') is not null
    exec(N'delete from dbo.VrstaDok where Opis = N''klik-proba''');
