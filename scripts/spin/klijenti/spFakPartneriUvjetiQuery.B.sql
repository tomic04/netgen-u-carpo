
CREATE procedure [dbo].[spFakPartneriUvjetiQuery]
@action int=1,
@FakPartneriUvjetiId int=null

as

if @action=1 begin
	select fp.FakPartneriUvjetiid,p.sifra as [Šifra partnera], p.naziv as [Partner],fp.Ucestalost as [Učestalost],
		case fp.Grupiranje when 1 then 'Po mjestu isporuke' when 2 then 'Po partneru' when 3 then 'Po dokumentu'  when 4 then 'Po roku plaćanja' end as [Grupiranje], fp. Valuta, fp.Rabat, fp.Povrat 
	, fp.parziroid, pz.ziro as [Žiro račun], fp.Ugovor
	from FakPartneriUvjeti fp
	inner join partneri p on p.partneriid=fp.partneriid
	left outer join parziro pz on pz.parziroid=fp.parziroid
	order by p.naziv
end

if @action=2 begin
	select p.naziv as [Partner], fp.* from FakPartneriUvjeti fp
	inner join partneri p on p.partneriid=fp.partneriid
	where @FakPartneriUvjetiId=FakPartneriUvjetiId
end

