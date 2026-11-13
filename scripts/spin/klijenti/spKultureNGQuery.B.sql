-- 01.03.2011 - njakic - stavio trim za char polja u Action 2
-- 20.10.2010 - njakic - promjenio inner u left join
-- 15.07.2010 - njakic - dodao polje Roba u Action 2    
    
CREATE PROCEDURE [dbo].[spKultureNGQuery]    
 @Action int=1,    
 @KultureId int=null    
AS    
 IF @Action=1    
 BEGIN    
  SELECT k.KultureId, k.Naziv AS [Naziv3000], k.Ozn AS [Oznaka0950],    
  k.sifra AS [Šifra0500], k.robaid, k.OrgShemaId,    
  o.naziv as [Organizacijska shema2000], r.naziv AS [Roba2800]    
  FROM Kulture k LEFT JOIN OrgShema o ON k.orgshemaid=o.orgshemaid    
  LEFT JOIN roba r ON k.robaid=r.robaid    
 END    
 IF @Action=2    
 BEGIN    
  SELECT k.KultureId, rtrim(k.Naziv) as Naziv, rtrim(k.Ozn) as Ozn, k.Standard1, k.Standard2, k.Standard3, k.Cijena,    
  rtrim(k.ind) as ind, rtrim(k.sifra) as sifra, rtrim(k.indkor) as indkor, k.robaid, k.OrgShemaId, r.Naziv as Roba    
  FROM Kulture k    
  LEFT JOIN Roba r on k.RobaId=r.RobaId    
  WHERE KultureId=@KultureId    
 END    
  

