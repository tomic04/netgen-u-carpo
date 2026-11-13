
--07.07.2026. --VV svi act- način plaćanja  (HD 107200)
--29.04.2022. --VV   , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi gdje je nedostajala
--15.03.2022. -VV - act 2 - smanjila kolone kako bi se forma Povijest vidjela bez klizača (i promijenjena veličina grida u XML-ovima) (HD 70689)
--19.11.2020. -VV - act 9 - dodala - za čitanje prethodno unesene orgsheme kod odobravanja
--16.11.2020. -VV -act 66 - komentirala coalesce(l.OpisRobe,'') as [Opis robe/usluge1500]
--11.11.2020. - VV - act 66 - dodala orgshemu
--27.06.2019 -VV -act 0 - isnull(p.NapomenaProslijedi, '') as [Napomena3000]
--21.06.2019. -VV - Proknjižio (operaterid iz fakturedob)
--13.07.2018. - napomena kod prosljeđivanja 
--27.02.2018. - VV - action 6- @NapomenaPosaljiOdobri
--18.10.2017. - VV - promijenila čitanje iz KupPrometa,a ne iz KupPrometLog-a
--28.6.2017. VV - indexi na temp - create index i_kk_1 on #tmpKUPlog(kupprometid)
--20.10. 2016. popravljen select za Plaćeno i Opis knjiženja
--30.9.2016. -- VV -  HD slučaj 20558 - kolone Plaćanje i Opis knjiženja
--18.07.2016. VV - OIB kod unosa u KUP
--11.12.2015 PJ - action 66, order by RedOdobravanja, action 4 dodana kolona Povezanih dokumenata, action 5 dodan top(1) iza uniona
--28.11.2014. VV na drvo dodala pregled nezaključenih računa (imaju UFU, ali kosi plavi u knjizi URA)
--23.09.2104 jasminka dodala  @action=1,4 dodala nova polja broj,iban,pozivnabroj
--03.02.14. VV dodani action 7 i 8 za pregled pošte zajedno s povijesti
--03.05. 12. VV - svim actionima zamijenjen inner join sa cross apply na KUPPrometLog i dodana dva indexa radi ubrzanja:
	  
	  --create index IX_KupPrometLog__KupPrometId_KupPrometLogId
	  --on KupPrometLog (KupPrometId, KupPrometLogId desc)

	  --create index IX_FaktureDob_KupPrometId
	  --on FaktureDob (KupPrometId)
--09.12. 2011. Po zahtjevu korisnika Damir Grahovac (Šećerana) omogućena izmjena dokumenta iz koraka Računovodstvo
--20.03.2009 jasminka case 4340,dodala uvjet u action=0 i 3, @DaId is null and (l.Zaprimljeno=0 or l.Zaprimljeno=1)
--02.03.2009 jasminka case 4340,dodala bojanje u plavo,uvjet zaprimljeno=2 
--30.01.2009 jasminka case 4340,dodala u action=5 u formatid za zaprimljeno=2-->boji u plavo-->'G'
--29.01.2009. jasminka case 4340,dodala u action=4 u formatid za zaprimljeno=2-->boji u plavo-->'G'
--28.01.2009. jasminka case 4340,zakomentirala u act=5 l.zaprimljeno=0
--19.12.2007. jasminka case 3021. dodala polje scan u action 0 i 4
--18.07.2007.jasminka,case 2874.,dodala u selecte polja:valuta,Posiljatelj,Rjesenje,UrudzbeniBrojVeza
                  

CREATE     PROCEDURE [dbo].[spKUPPrometQueryN]
@Action int=null,
@ID int =null,
@DatumPrijemaOd datetime=null,
@DatumPrijemaDo datetime=null,
@partneriid int=null,
@KUPStatusId int=null,
@DaId int=null,
@OperateriId int =0,
@dostavnaknjigaid int =null,
@aktivni int=null,
@filter varchar(1000)='',
@ugovori char(1)='0',
@DatDo datetime=null, 
 @Ind int = 0,
@Node varchar(20) =''

with recompile
AS

----------------------------------------------------------------------------------
 declare @KUPStatusIdObrisan int
 set @KUPStatusIdObrisan=(select KUPStatusId from KUPStatus where Opis='Obrisan')
 
 ---stavljeno je u action=4,ako treba još negdje samo dodaš oavj kod u where uvjet
 --and l.KupStatusId<>@KUPStatusIdObrisan 
 ---------------------------------------------------------------------------------

----------------------------------------------------------
--------------------------------------------------------
 if @Action = 66 begin  
--vv isključila da se vide sve grupe
		--declare @grupa int
		--set @grupa = (select max(grupaodobravanja) from KUPPrometOdobrenjeLog where kupprometid = @id) 
   --vv end     
        
		select s.Naziv  
			, l.DatumOdobravanja as [Datum odobravanja]  
			, ks.Opis as Status  
			, l.DatumUnosa as [Datum slanja]  
			, case when l.KupStatusOdobrenjeId is not null then  l.razlogodbijanja else '' end as Napomena  
			, kn.napomena as Napomena2
			-- , l.RazlogOdbijanja as Napomena  
			, l.RedOdobravanja as [Red odobravanja1500]
			--, coalesce(l.OpisRobe,'') as [Opis robe/usluge1500]
			, coalesce(l.MjestoTroska,'') as [Broj mjesta troška1500]
			--, NapomenaPosaljiOdobri as NapomenaUzSlanjeNaOdobr
			, case when l.NapomenaPosaljiOdobri is not null then  l.NapomenaPosaljiOdobri else RazlogOdbijanja end as NapomenaUzSlanjeNaOdobr  
			, l.grupaodobravanja as KrugOdobravanja
			, org.Naziv as [Organizacijska;shema]
			from KUPPrometOdobrenjeLog l  
			inner join ssOperateri s on s.OperateriID = l.operateriidto  
			left outer join KupStatusOdobrenje ks on ks.KupStatusOdobrenjeid = l.KupStatusOdobrenjeid 
			left outer join kupnapomene kn on l.KupNapomeneId = kn.KupNapomeneId 
			 inner join kuppromet kp on kp.KUPPrometid = l.kupprometid
			 left outer join Orgshema org on kp.OrgShemaId=org.OrgShemaId
			where l.KupPrometId = @id
			--  and ((grupaodobravanja = @grupa or @ind = 0) or RedOdobravanja=-1) -- vv isključila
			--group by l.DatumUnosa, s.Naziv, ks.Opis, l.DatumOdobravanja  
		order by l.grupaodobravanja,l.redodobravanja, l.DatumUnosa, 1  
		option (recompile)
		end  
        
  ---------------------------------------------------------------
  ----------------------------------------------------------------      
     /*
     select *   
        into #tmpKUPlog   
        from kupprometlog  
	    where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid)  
	    
	    ---
       create index i_kk_1 on #tmpKUPlog(kupprometid)
		
        */
        
    /*
		select   
		fd.fakturedobid, kup.kupprometid, kp.opis
		, sum(kp.duguje) as [Plaćeno]
		into #FaktureDobPlaćeno
       from fakturedob fd
       inner join kntpromet kp on kp.iddok=fd.faktureDobid
       inner join kntplana a on a.kntplanaid=kp.kntplanaid and a.kntplanaid in (select kntplanaid from kntplana where kntvrstaid in (select kntvrstaid from kntvrsta where tablica='FaktureDob'))
	   inner join kuppromet kup on fd.kupprometid = kup.kupprometid
       where fd.KUPPrometId=kup.kupprometid
		group by fd.fakturedobid, kup.KUPPrometid, kp.opis
		*/
		
		/*
		--select za plaćeno i opis knjiženja
		select   
		fd.fakturedobid, kup.kupprometid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
		, sum(kp.duguje) as [Plaćeno]
		into #FaktureDobPlaceno
		 from fakturedob fd
			inner join kntpromet kp on kp.iddok=fd.faktureDobid
			inner join kntplana a on a.kntplanaid=kp.kntplanaid and a.kntplanaid in (select kntplanaid from kntplana where kntvrstaid in (select kntvrstaid from kntvrsta where tablica='FaktureDob'))
			inner join kuppromet kup on fd.kupprometid = kup.kupprometid
       where fd.KUPPrometId=kup.kupprometid 
		group by fd.fakturedobid, kup.KUPPrometid
		
		
		 create index i_kk_2 on #FaktureDobPlaceno(kupprometid)
		
		*/
		
if @action in (5,7) begin

		select max(grupaodobravanja) as grupaodobravanja
			, kupprometid 
		into #t_max_grupa_kup
		from kupprometodobrenjelog
		group by kupprometid

		select *   
		into #tempodoblog  
		from kupprometodobrenjelog  
		where kupprometodobrenjelogid in (select max(k.kupprometodobrenjelogid)
		  from kupprometodobrenjelog k
		  inner join #t_max_grupa_kup t on t.kupprometid = k.kupprometid
		  where k.operateriidto = @OperateriId 
		  and k.kupstatusodobrenjeid is null
		  and k.grupaodobravanja = t.grupaodobravanja
		  group by k.kupprometid)

	end         

declare @DatumOd varchar(8),@DatumDo varchar(8)  
  
set @DatDo =    getdate()

	if @action=0 --KUP filter forma
	begin
       
			select 
				p.KUPPrometId,formatid=case p.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end,  
				p.KUPStatusId,--'' as [!S0500],
				p.KUPVrstaPosteId,
				pt.Naziv as [Naziv2000] ,
				pt.sifra as [Partner;Šifra1000],
				p.UrudzbeniBroj as [UrudzbeniBroj1200],
				p.Iznos as [Iznos1200],
				vp.naziv as [VrstaPoste2000],
				p.DatumPrijema as [DatumPrijema1200],
				ks.opis as [Korak2000],
				p.Zaprimljeno as [Zaprimljeno1000],
				op.Naziv as [Kome1000],
				p.datumunosa as [Datum promjene1200],
				convert(varchar,p.DatumUnosa,108) as [Vrijeme Promjene1500],
				o.Naziv as [Od koga1000],
				p.PartneriId,
				p.Broj as [Broj2000],
				nar.Broj as [BrojNarudžbe1500],
				-- p.BrojUFE as [BrojUFE2000],
				fd.brojdok as [BrojUFE0900],
				fd.DatumDok as [Datum Knjiženja1200],
				fd.indglk as [Status Knjiženja0800],
				p.DatumValute as [DatumValute1500],
				p.Opis as [Opis1800],
				org.Naziv as [OrgShema2000],
				-- p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,                                      
				(v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
				p.Posiljatelj as [Pošiljatelj1000] ,
				p.Rjesenje as [Rješenje1200],
				p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
				p.Scan as [Scan],    
				p.NabNarGlaId
				, z.IBAN as [IBAN2000]
				,p.model + p.PozivNaBroj as [Poziv na broj2000]
				--       ,p.Dana as [DanaValute0800]  
				, cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]  
				, datumDokumenta as [Datum Dokumenta1500]   
				, cast(p.dana  as int ) as [Broj Dana0700]  
				, p.poreznibroj                  
				, fdp.Placeno as [Plaćeno1500]                 
				, fdp.opis as [Opis knjiženja2000]
				, fdo.Naziv as [Proknjižio]
				  , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi
				  , isnull(p.nacinplacanja, '') as NačinPlaćanja
		 from KUPPromet p --inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
				--  cross apply (select top (1) * from kupprometlog kpl where kpl.kupprometid = p.kupprometid order by kupprometlogid desc) l
				inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
				left outer join Partneri pt on p.PartneriId=pt.PartneriId
				left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId
				inner join ssoperateri o on p.operateriidfrom=o.operateriid
				inner join ssoperateri op on p.operateriidto=op.operateriid
				inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId
				left outer join Valute v on p.ValuteId=v.ValuteId
				left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId
				--inner join ssmeni sm on vp.meniid = sm.meniid
				left outer join fakturedob fd on fd.kupprometid = p.kupprometid
				left join ssoperateri fdo	on fd.operaterid = fdo.operateriid
				left outer join ParZiro z on p.ParZiroId=z.ParZiroId	
				-- left join #fakturedobplaceno fdp on fd.fakturedobid = fdp.fakturedobid
				outer apply (
						select   
						kp.partneriid,kp.iddok as fakturedobid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
						, sum(kp.duguje) as Placeno
						from fakturedob fd with (nolock)
							  inner join kntpromet kp  with (nolock) on kp.iddok=fd.faktureDobid
							  inner join kntplana a  with (nolock) on a.kntplanaid=kp.kntplanaid 
							 inner join kntvrsta kv  with (nolock) on a.kntvrstaid=kv.kntvrstaid and tablica='FaktureDob'
							where fd.kupprometid = p.kupprometid 
						group by kp.partneriid,kp.iddok--fd.fakturedobid
				) fdp
				 
				 
			where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)
				 and (p.partneriid=@partneriid or @partneriid is null)
				 and (p.KUPStatusId=@KUPStatusId or @KUPStatusId is null)
				 and ((@Daid=0 and p.Zaprimljeno=0)or (@Daid=1 and p.Zaprimljeno=1)or (@Daid=2 and p.Zaprimljeno=2)
					   or (@DaId is null and (p.Zaprimljeno=0 or p.Zaprimljeno=1 or p.zaprimljeno=2)))
				 and (convert(varchar(50),p.urudzbenibroj) + '' +convert(varchar(50),p.iznos)/*+ '' + org.Naziv*/) like '%'+@filter+'%'
				 -- and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
			order by p.KUPPrometId
     
		option (recompile)

        return (0)
        end

	if @action=1 --KUP -refresh nakon update-a
	        begin
               
                declare @Zaprimljeno int,@OperateriIdTo int
                set @zaprimljeno=(select zaprimljeno from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id))
                set @OperateriIdTo= (select OperateriIdTo from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id))

           if not exists (
				select * 
				from KUPPrometLog l 
                                inner join KUPStatus s on l.KUPStatusId=s.KUPStatusId
                                
				where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id)
                                and @OperateriIdTo = @OperateriId
			        and s.Korak = 0 or s.korak = 4 and @Zaprimljeno=1) begin
                            
                                EXEC spRaiserror 69001, 'Nemate prava izmjene dokumenta!' 
				return
			end


			select p.KUPPrometId,
					p.KUPStatusId--,'' as [!S0500]
					, formatid=case when p.zaprimljeno=0 then 'red' else ' ' end,
					p.KUPVrstaPosteId,

					pt.Naziv as [Naziv2000] ,
					pt.sifra as [Partner;Šifra1000],
					p.UrudzbeniBroj, --as [UrudzbeniBroj1200],
					p.Iznos, --as [Iznos1200],
					vp.naziv, --as [VrstaPoste2000],
					p.DatumPrijema, --as [DatumPrijema1200],
					--ks.opis, --as [Korak2000],
					p.Zaprimljeno, --as [Zaprimljeno1000],
					op.Naziv , --as [Kome2000],
					p.datumunosa as [Datum promjene1200],
					convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],
					o.Naziv , --as [Od koga2000],
					p.PartneriId,
					pt.sifra as [Partner;Šifra1000],
					pt.Naziv as Partner, 
					p.Broj as Broj,
					nar.Broj as narudzbatra,
					-- p.BrojUFE, --as [BrojUFE2000],
					fd.brojdok as [BrojUFE0900],
					fd.DatumDok as [Datum Knjiženja1200],
					fd.indglk as [Status Knjiženja0800],
					p.DatumValute, --as [DatumValute1500],
					p.Opis, --as [Opis0800],
					p.OrgShemaId,p.KupStatusId,
					org.Naziv , --as [OrgShema2000],
					valuteid=p.valuteid,
					(v.Oznaka+space(1)+v.Naziv) as Valuta,
					p.Posiljatelj ,
					p.Rjesenje,
					p.UrudzbeniBrojVeza,
					p.NabNarGlaId
					,z.IBAN as [IBAN2000]
					,p.model + p.PozivNaBroj as [Poziv na broj2000]
					--  ,p.Dana as [DanaValute0800]  
					-- , cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]  
					, datumDokumenta --as [DatumDokumenta1500]   
					, p.dana --as BrojDana   
					, p.poreznibroj  as poreznibroj--OIB
					, fdp.Placeno as [Plaćeno1500]  
					, fdp.opis as [Opis knjiženja2000]
					, fdo.Naziv as [Proknjižio]
					 , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi
					 , isnull(p.nacinplacanja, '') as NačinPlaćanja
			from KUPPromet p --inner join KUPPrometLog l on p.KUPPrometId=l.KUPPrometId
					left outer join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
					left outer join Partneri pt on p.PartneriId=pt.PartneriId
					left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId
					left outer join ssoperateri o on p.operateriidfrom=o.operateriid
					left outer join ssoperateri op on p.operateriidto=op.operateriid
					left outer join KUPStatus ks on p.KUPStatusId = ks.KupStatusId
					left outer join Valute v on p.ValuteId=v.ValuteId
					left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId
					-- inner join ssmeni sm on vp.meniid = sm.meniid
					left outer join fakturedob fd on fd.kupprometid = p.kupprometid
					left join ssoperateri fdo	on fd.operaterid = fdo.operateriid
					left outer join ParZiro z on p.ParZiroId=z.ParZiroId
					-- left join #fakturedobplaceno fdp on fd.fakturedobid = fdp.fakturedobid
					outer apply (
						select   
						kp.partneriid,kp.iddok as fakturedobid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
						, sum(kp.duguje) as Placeno
						from fakturedob fd with (nolock)
							  inner join kntpromet kp  with (nolock) on kp.iddok=fd.faktureDobid
							  inner join kntplana a  with (nolock) on a.kntplanaid=kp.kntplanaid 
							 inner join kntvrsta kv  with (nolock) on a.kntvrstaid=kv.kntvrstaid and tablica='FaktureDob'
							where fd.kupprometid = p.kupprometid 
						group by kp.partneriid,kp.iddok--fd.fakturedobid
					) fdp

			where 				 
			p.KUPPrometId=@ID
			--and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
			option (recompile)
			end

        if @action=2  --KUP i KUPDostavnaKnjiga -Povijest
	       begin
			 select l.kupprometlogid
					,formatid=case when l.zaprimljeno=0 then 'red' else ' ' end
					,l.kupprometid
					,l.kupstatusid,s.opis as [Status1200]
					,l.Zaprimljeno as [Zaprimljeno1000]
					,o.Naziv as [Od koga1500],
					op.Naziv as [Kome1500]
					,l.DatumUnosa as [DatumPromjene1300]
					, CONVERT(varchar,l.DatumUnosa,108 ) as [VrijemePromjene1300]
					, isnull(l.NapomenaProslijedi, '') as [Napomena3400]
			from KUPPrometLog l 
					inner join KUPStatus s on l.kupstatusid=s.kupstatusid
					inner join ssoperateri o on l.operateriidfrom=o.operateriid
					inner join ssoperateri op on l.operateriidto=op.operateriid
			where 	l.KUPPrometId=@ID
			order by l.KUPPrometLogId
   
            return(0)

        

            end


	if @action=3 --and (select min(ks1.korak) from kupstatus ks1 inner join kupstatusoperateri ko on ks1.kupstatusid=ko.kupstatusid
                         --  where ko.operateriid=@OperateriId)<>0    --KUPDostavnaKnjiga-filter forma,operater nije u urudžbenom
	       begin

		  select 
                  --   p.KUPPrometId,formatid=case l.zaprimljeno when 0 then 'R' when 2 then 'G' else ' ' end,
                      p.KUPPrometId,formatid=case p.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end,  
                     p.KUPStatusId--,'' as [!S0500],
                     ,p.KUPVrstaPosteId,
                    
                     pt.Naziv as [Naziv2000] ,
                      pt.sifra as [Partner;Šifra1000],
                     
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],
                     p.Iznos as [Iznos1200],
                      vp.naziv as [VrstaPoste2000],
                     p.DatumPrijema as [DatumPrijema1200],
                     ks.opis as [Korak2000],
                     p.Zaprimljeno as [Zaprimljeno1000],
                     op.Naziv as [Kome1000],
                     p.datumunosa as [Datum promjene1200],
                     convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],
                     o.Naziv as [Od koga1000],
                     p.PartneriId,
                     -- pt.sifra as [Partner;Šifra1000],
                     --pt.Naziv as [Naziv2000],
                     p.Broj as [Broj2000],
                     nar.Broj as [BrojNarudžbe1500],
                    -- p.BrojUFE as [BrojUFE2000],
                    fd.brojdok as [BrojUFE0900],
                    fd.DatumDok as [Datum Knjiženja1200],
                     fd.indglk as [Status Knjiženja0800],
                     p.DatumValute as [DatumValute1500],
                     p.Opis as [Opis1800],
                     --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
                     org.Naziv as [OrgShema2000],
                     valuteid=p.valuteid,
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
                     p.Posiljatelj as [Pošiljatelj1000],
                     p.Rjesenje as [Rješenje1200],
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
                     p.Scan as [Scan],
                     p.NabNarGlaId 
                    , z.IBAN as [IBAN2000]
                    ,p.model + p.PozivNaBroj as [Poziv na broj2000]
             --       ,p.Dana as [DanaValute0800]  
                    , cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]  
					, datumDokumenta as [DatumDokumenta1500]   
                    , p.dana as BrojDana  
				    , p.poreznibroj 
			        , fdp.Placeno as [Plaćeno1500] 
					, fdp.opis as [Opis knjiženja2000] 
					, fdo.naziv as [Proknjižio]
					 , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi
					 , isnull(p.nacinplacanja, '') as NačinPlaćanja
                  from KUPPromet p --inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
							--  cross apply (select top (1) * from kupprometlog kpl where kpl.kupprometid = p.kupprometid order by kupprometlogid desc) l
							inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
							left outer join Partneri pt on p.PartneriId=pt.PartneriId
							left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId
							inner join ssoperateri o on p.operateriidfrom=o.operateriid
							inner join ssoperateri op on p.operateriidto=op.operateriid
							inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId
							left outer join Valute v on p.ValuteId=v.ValuteId
							left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId
							--inner join ssmeni sm on vp.meniid = sm.meniid
							left outer join ParZiro z on p.ParZiroId=z.ParZiroId	
							left outer join fakturedob fd on fd.kupprometid = p.kupprometid
							left join ssoperateri fdo	on fd.operaterid = fdo.operateriid
							-- left join #fakturedobplaceno fdp on fd.fakturedobid = fdp.fakturedobid
							outer apply (
								select   
								kp.partneriid,kp.iddok as fakturedobid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
								, sum(kp.duguje) as Placeno
								from fakturedob fd with (nolock)
									  inner join kntpromet kp  with (nolock) on kp.iddok=fd.faktureDobid
									  inner join kntplana a  with (nolock) on a.kntplanaid=kp.kntplanaid 
									 inner join kntvrsta kv  with (nolock) on a.kntvrstaid=kv.kntvrstaid and tablica='FaktureDob'
									where fd.kupprometid = p.kupprometid 
								group by kp.partneriid,kp.iddok--fd.fakturedobid
							) fdp

				 where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)
                         and (p.partneriid=@partneriid or @partneriid is null)
                         and ((@Daid=0 and p.Zaprimljeno=0)or (@Daid=1 and p.Zaprimljeno=1)or (@Daid=2 and p.Zaprimljeno=2)
                             or (@DaId is null and (p.Zaprimljeno=0 or p.Zaprimljeno=1)))
                         and p.operateriidto=@OperateriId
						 --and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                                     
                 order by p.KUPPrometId
                 option (recompile)
                return(0)

        

              end

--           
       if @action=4  --KUP-startupstora --sve što ima korak=0-->u urudžbenom
			begin

       
			 select 
					p.KUPPrometId,formatid=case p.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end ,
					p.KUPStatusId--,'' as [!S0500]
					,p.KUPVrstaPosteId,
					pt.Naziv as [Naziv2000] ,
					pt.sifra as [Partner;Šifra1000],
					p.UrudzbeniBroj as [UrudzbeniBroj1200],
					p.Iznos as [Iznos1200],
					vp.naziv as [VrstaPoste2000],
					--   p.UrudzbeniBroj as [UrudzbeniBroj1200],
					ks.opis as [Korak2000],
					p.Zaprimljeno as [Zaprimljeno1000],
					op.Naziv as [Kome1000],
					p.datumunosa as [Datum promjene2000],
					convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],
					o.Naziv as [Od koga1000],
					p.PartneriId,
					-- pt.sifra as [Partner;Šifra1000],
					--pt.Naziv as [Naziv2000],
					p.Broj as [Broj2000],
					nar.Broj as [BrojNarudžbe1500],
					-- p.BrojUFE as [BrojUFE2000],
					fd.brojdok as [BrojUFE0900],
					fd.DatumDok as [Datum Knjiženja1200],
					fd.indglk as [Status Knjiženja0800],

					p.DatumValute as [DatumValute1500],
					p.Opis as [Opis1800],
					--p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
					org.Naziv as [OrgShema2000],
					valuteid=p.valuteid,
					(v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
					p.Posiljatelj as [Pošiljatelj1000],
					p.Rjesenje as [Rješenje1200],
					p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
					p.Scan as [Scan],
					p.NabNarGlaId,
					z.IBAN as [IBAN2000]
					,p.model + p.PozivNaBroj as [Poziv na broj2000]
					--       ,p.Dana as [DanaValute0800]  
					, cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]  
					, datumDokumenta as [DatumDokumenta1500]   
					, cast(p.dana  as int ) as [Broj Dana0700]
					,( select count(LnKUPSktDokId) from LnKUPSktDok where KupPrometId=p.KupPrometId) as [Povezanih dukumenata1500]
					, p.poreznibroj 
					, fdp.Placeno as [Plaćeno1500]  
					, fdp.opis as [Opis knjiženja2000]
					, fdo.Naziv as [Proknjižio]
					 , opk.naziv as [Operater;Knjiženja]
					  , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi
					  , isnull(p.nacinplacanja, '') as NačinPlaćanja
		 from KUPPromet p --inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
					-- cross apply (select top (1) * from kupprometlog kpl where kpl.kupprometid = p.kupprometid order by kupprometlogid desc) l
					inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
					left outer join Partneri pt on p.PartneriId=pt.PartneriId
					left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId
					inner join ssoperateri o on p.operateriidfrom=o.operateriid
					inner join ssoperateri op on p.operateriidto=op.operateriid
					 left join ssoperateri opk on p.operateriidto=opk.operateriid
					inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId
					left outer join Valute v on p.ValuteId=v.ValuteId
					left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId
					--inner join ssmeni sm on vp.meniid = sm.meniid
					left outer join fakturedob fd on fd.kupprometid = p.kupprometid
					left join ssoperateri fdo	on fd.operaterid = fdo.operateriid
					left outer join ParZiro z on p.ParZiroId=z.ParZiroId	
					left join KupStatusOdobrenje kso on p.KupStatusOdobrenjeId = kso.KupStatusOdobrenjeId  
					-- left join #fakturedobplaceno fdp on fd.fakturedobid = fdp.fakturedobid
					outer apply (
						select   
						kp.partneriid,kp.iddok as fakturedobid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
						, sum(kp.duguje) as Placeno
						from fakturedob fd with (nolock)
							  inner join kntpromet kp  with (nolock) on kp.iddok=fd.faktureDobid
							  inner join kntplana a  with (nolock) on a.kntplanaid=kp.kntplanaid 
							 inner join kntvrsta kv  with (nolock) on a.kntvrstaid=kv.kntvrstaid and tablica='FaktureDob'
							where fd.kupprometid = p.kupprometid 
						group by kp.partneriid,kp.iddok--fd.fakturedobid
					) fdp

				  where datumprijema > DATEADD(m, -12, current_timestamp)
				 and ks.korak=0  
				  and   p.Zaprimljeno <> 2 
			--	   and p.KupStatusId<>@KUPStatusIdObrisan 
		          --vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                 order by p.KUPPrometId
		option (recompile)
     

         return (0)
         end


       if @action=5 --KUPDostavnaKnjiga-startupstora  
        begin  

			--drop table #tempBezArhiviranih
			/*
				 select *   
				  into #tempBezArhiviranih
				  from kupprometlog 
				  where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid)
				  and zaprimljeno not in (1,2)
				  order by kupprometid
			*/

			select *   
			 into #tempBezArhiviranih
			 --from #tmpKUPlog 
			 from KupPromet
			 where zaprimljeno not in (1,2)

		  select *
		  into #tempZaKnjizenje
		  from kupprometlog 
		  where operateriidto=@operateriid 
		  and kupprometid in (select kupprometid from #tempBezArhiviranih)
		  --and kupstatusid=(select kupstatusid from kupstatus where korak=2)
		  and kupstatusid=(select kupstatusid from kupstatus where korak=16)
		  and zaprimljeno not in (1)
		  order by kupprometid

		create index #tempZaKnjizenje on #tempZaKnjizenje(kupprometid)




			select   
				 p.KUPPrometId  
				 , formatid = case when p.zaprimljeno = 0 then 'red'   
				when p.zaprimljeno = 2 then 'blue'   
									else ' ' end ,  
				 p.KUPStatusId,'' as [S0500],  
				 p.KUPVrstaPosteId,  
				 vp.naziv as [VrstaPoste2000],  
				 p.UrudzbeniBroj as [UrudzbeniBroj1200],  
				 p.DatumPrijema as [DatumPrijema1200],  
				 p.datumdokumenta as [DatumDokumenta],  
				 ks.opis as [Korak2000],  
				 p.Zaprimljeno as [Zaprimljeno1000],  
				 op.Naziv as [Kome1000],  
				 p.datumunosa as [Datum promjene2000],  
				 convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],  
				 o.Naziv as [Od koga1000],  
				 p.PartneriId,  
				 pt.Naziv as [Naziv2000],  
				 p.Broj as [Broj2000],  
				 nar.Broj as [BrojNarudžbe1500],  
				 --p.BrojUFE as [BrojUFE2000],  
				 fd.brojdok as [BrojUFE2000],  
				 p.DatumValute as [DatumValute1500],  
				 p.Opis as [Opis1800],  
				 p.Iznos as [Iznos1200],  
				 --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,  
				 org.Naziv as [OrgShema2000],  
				 valuteid=p.valuteid,  
				 (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
				 p.Posiljatelj as [Pošiljatelj1000],  
				 p.Rjesenje as [Rješenje1200],  
				 p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
				 p.Scan as [Scan],   
				 p.NabNarGlaId,  
				--  p.Registrator   
				coalesce ((p.UFABroj+' - '+p.registrator), '') as Registrator 
				 ,  kso.Opis as [Status odobravanja]  
				 , p.poreznibroj 
				 , fdp.Placeno as [Plaćeno1500]  
				  , fdp.opis as [Opis knjiženja2000]
				  , fdo.naziv as [Proknjižio]
				   , isnull(p.NapomenaProslijedi, '') as NapomenaProslijedi
				   , isnull(p.nacinplacanja, '') as NačinPlaćanja
			from KUPPromet p --inner join #tmpKUPlog  
                 -- inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))  
                 -- l on p.kupprometid = l.kupprometid  
				inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
				left outer join Partneri pt on p.PartneriId=pt.PartneriId  
				left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
				inner join ssoperateri o on p.operateriidfrom=o.operateriid  
				inner join ssoperateri op on p.operateriidto=op.operateriid  
				inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
				left outer join Valute v on p.ValuteId=v.ValuteId  
				left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId  
				--inner join ssmeni sm on vp.meniid = sm.meniid  
				left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
				left join ssoperateri fdo	on fd.operaterid = fdo.operateriid
				left outer join KupStatusOdobrenje kso on p.KupStatusOdobrenjeId = kso.KupStatusOdobrenjeId  
				 -- left join #fakturedobplaceno fdp on fd.fakturedobid = fdp.fakturedobid
				outer apply (
					select   
					kp.partneriid,kp.iddok as fakturedobid,  max(case when potrazuje <>0.00 then kp.opis  else '' end) as opis
					, sum(kp.duguje) as Placeno
					from fakturedob fd with (nolock)
						  inner join kntpromet kp  with (nolock) on kp.iddok=fd.faktureDobid
						  inner join kntplana a  with (nolock) on a.kntplanaid=kp.kntplanaid 
						 inner join kntvrsta kv  with (nolock) on a.kntvrstaid=kv.kntvrstaid and tablica='FaktureDob'
						where fd.kupprometid = p.kupprometid 
					group by kp.partneriid,kp.iddok--fd.fakturedobid
				) fdp
			 where /*l.zaprimljeno=0   
				 and*/ p.operateriidto=@OperateriId  
				 and   p.Zaprimljeno <> 2  
				 --    and coalesce(p.kupstatusodobrenjeid,0) in (0,1,4)  --u dostavnoj vide samo dokumente koji nisu u procesu odobravanja ili su neodobreni

				--and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')  
				--order by p.KUPPrometId  
	               

	union all

			select   
					p.KUPPrometId  
					 , formatid = case when l.zaprimljeno = 0 and p.KupStatusOdobrenjeId is null then 'red'   
								  when l.zaprimljeno = 2  then 'blue'
								  when l.zaprimljeno = 0 and p.KupStatusOdobrenjeId=3 then 'boldred'  
								  else ' ' end ,  
	
… (skraćeno — puni tekst na stranici usporedbe)