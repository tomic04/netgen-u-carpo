-- 15.07.2010 - njakic - dodao polje Roba u Action 2

create PROCEDURE [dbo].[spKultureNGQuery]
	@Action int=1,
	@KultureId int=null
AS
	IF @Action=1
	BEGIN
		SELECT k.KultureId, k.Naziv AS [Naziv3000], k.Ozn AS [Oznaka0950],
		k.Standard1 AS [Standard 11150], k.Standard2  AS [Standard 21150],
		k.Standard3 AS [Standard 31150], k.Cijena AS [Cijena0850], k.ind AS [ind0500],
		k.sifra AS [Šifra0500], k.indkor AS [inkor0500], k.robaid, k.OrgShemaId,
		coalesce(o.naziv,'') as [Organizacijska shema2000], coalesce(r.naziv,'') AS [Roba2800]
		FROM Kulture k left outer JOIN OrgShema o ON k.orgshemaid=o.orgshemaid
		left outer JOIN roba r ON k.robaid=r.robaid
	END
	IF @Action=2
	BEGIN
		SELECT k.KultureId, k.Naziv, k.Ozn, k.Standard1, k.Standard2, k.Standard3, k.Cijena,
		k.ind, k.sifra, k.indkor, k.robaid, k.OrgShemaId, r.Naziv as Roba
		FROM Kulture k
		left outer JOIN Roba r on k.RobaId=r.RobaId
		WHERE KultureId=@KultureId
	END