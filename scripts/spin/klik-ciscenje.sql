-- Testni redovi klik-pregleda na spinu. Nije klijentski podatak.
-- Briše samo Naziv koji je klik sam upisao. Tuđi jezici ostaju.
delete from jezici where Naziv like N'klik-proba%';
if object_id(N'dbo.statusi', N'U') is not null
    delete from statusi where Korak like N'klik-proba%';
