




/*************
VGV
--28.04.2026. Odjel

*****************/


/*************
NOVI LIST
--11.06.2025. VV - act 0 i 3 - zaprimljeno da se nakon Pregleda pokažu arhivirani


*****************/


/**************************
--ISTARSKA PIVOVARA
--27.11.2024. VV - act 5 - prvi union komentirala kupstatusodobrenje
--25.11.2024. VV - act 5 odobreni i neodobreni prema statusu iz vrste pošte


***************************/

--07.09.2022. VV - act 5 - dohvat statusa za pregled odobrenih dokumenta

--BOBIS kreće od Mlinar verzije
-------------------------------
--------------------------------
--27.09.2017 jasminka promijenila čitanje iz KupPrometa,a ne iz KupPrometLog-a
--21. 02. 2016. VV vratila and (grupaodobravanja = @grupa or @ind = 0)
--19.01.2016.VV privremeno isključila  --and (grupaodobravanja = @grupa or @ind = 0) -- kako bi se vidjeli svi detalji, a ne samo zadnji krug odobravanja
--16.07.2015. ZK promijenio where uvjet kod inserta u #tempodoblog i action 6 - prikaži u detalj gridu samo zadnji krug odobravanja (@ind = 1) (zahtjev g. Sršen)
--23.06.2015. VV action 4 dodano where l.Zaprimljeno <> 2  - da se u Knjizi pošte ne prikazuju arhivirani 
 --19.06.2015. VV--27.05.2015. JC & VV - action 5 - podešeno da se kod onih koji su za knjiženje ne vide dokumenti koje je netko drugi arhivirao
  --27.02.2015. JC & VV - knjiženje i prikaz dokumenata za knjiženje (action 5)
--02.12.2014 jasminka dodala red odobravanja u act=6  
--24.03.2014 jasminka dodala polje opis u action=2 i action=88  
--04.02.2014. VV - @action = 111 - refresh nakon update projekta  
--08.11.2013. VV - napomena kod odobravanja  
--rujan 2013. -  select * into #tmpKUPlog  
--10.07.2013 Josip zakomentirao !S zbog pamćenja grida  
--21.06.2013. VV @PtProcesId  
--28.11.2012. VV datum dokumenta  
--23.11.2012. ZK dodao action 6, dodao union all u action 5  
--31.07.12. ZK & VV pretraživanje po registratoru  
--06.06. 12. VV registrator  
--20.03.2009 jasminka case 4340,dodala uvjet u action=0 i 3, @DaId is null and (l.Zaprimljeno=0 or l.Zaprimljeno=1)  
--02.03.2009 jasminka case 4340,dodala bojanje u plavo,uvjet zaprimljeno=2   
--30.01.2009 jasminka case 4340,dodala u action=5 u formatid za zaprimljeno=2-->boji u plavo-->'G'  
--29.01.2009. jasminka case 4340,dodala u action=4 u formatid za zaprimljeno=2-->boji u plavo-->'G'  
--28.01.2009. jasminka case 4340,zakomentirala u act=5 l.zaprimljeno=0  
--19.12.2007. jasminka case 3021. dodala polje scan u action 0 i 4  
--18.07.2007.jasminka,case 2874.,dodala u selecte polja:valuta,Posiljatelj,Rjesenje,UrudzbeniBrojVeza  
                    
  
CREATE      PROCEDURE [dbo].[spKUPPrometQueryN]  
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
@registrator varchar(50)='',  
@Godina varchar (7) = '',  
@TipPoste varchar (20) = '',  
@UFABroj varchar (20) = '',  
@Node varchar(20) ='',  
@DatDo datetime=null,
@Ind int = 0
  

with recompile  
  
AS  
  
   if @Action = 6 begin  

		declare @grupa int
		set @grupa = (select max(grupaodobravanja) from KUPPrometOdobrenjeLog where kupprometid = @id) 
        
		  select s.Naziv  
			, l.DatumOdobravanja as [Datum odobravanja]  
			, ks.Opis as Status  
			, l.DatumUnosa as [Datum slanja]  
			, l.razlogodbijanja as Napomena  
			, l.RedOdobravanja as [Red odobravanja0700]
			, l.GrupaOdobravanja as [Grupa odobravanja0700]
			, coalesce(l.OpisRobe,'') as [Opis robe/usluge1500]
			, coalesce(l.MjestoTroska,'') as [Broj mjesta troška1500]  
			 , opo.Naziv as Odobrio
			 , kop.naziv as [Predložak1000]
		    
		  from KUPPrometOdobrenjeLog l  
		  inner join ssOperateri s on s.OperateriID = l.operateriidto  
		  left outer join KupStatusOdobrenje ks on ks.KupStatusOdobrenjeid = l.KupStatusOdobrenjeid  
		  left join ssOperateri opo on opo.OperateriID = l.odobrio  
		   left outer join kupodobripredlozak kop on l.kupodobripredlozakid = kop.kupodobripredlozakid
		  where l.KupPrometId = @id
		--  and (grupaodobravanja = @grupa or @ind = 0)   
		  order by l.GrupaOdobravanja, l.RedOdobravanja--1
		  
	  option(recompile)
	  return
	end  

  --      select *   
  --      into #tmpKUPlog   
  --      from kupprometlog  
	 --   where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid)  
		
		--create index i_kk_1 on #tmpKUPlog(kupprometid, operateriidto,zaprimljeno)
        
		
if @action in (5,7) begin

 	select max(grupaodobravanja) as grupaodobravanja
		, kupprometid 
	into #t_max_grupa_kup
	from kupprometodobrenjelog
	group by kupprometid
	
	create index #t_max_grupa_kup on #t_max_grupa_kup(kupprometid)

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

									  
	
	create index #tempodoblog on #tempodoblog(kupprometid)

end         

declare @DatumOd varchar(8),@DatumDo varchar(8)  
  
 if @action=0 --KUP filter forma  
 begin  

                     select   
                     p.KUPPrometId,formatid=case p.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end,    
                     p.KUPStatusId 
                     ,p.KUPVrstaPosteId,  
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
                     pt.Naziv as [Naziv2000] ,  
                     p.Broj as [Broj2000],  
                    
                  coalesce(p.narudzbe,'') as [Broj narudžbe1500],  
                      
                    fd.brojdok as [BrojUFE1500],  
                     p.DatumValute as [DatumValute1500],  
                     p.Opis as [Opis1800],  
                     p.Iznos as [Iznos1200],  
                     org.Naziv as [OrgShema2000],  
                                                        
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
                     p.Posiljatelj as [Posiljatelj1000] ,  
                     p.Rjesenje as [Rješenje1200],  
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
                     p.Scan as [Scan],      
                     p.NabNarGlaId 
                  -- ,pr.naziv as VrstaTroška  
                     
          
         from dbo.KUPPromet p --inner join #tmpKUPlog  
  -- --(select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))  
  -- l on p.kupprometid = l.kupprometid  
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId  
                                  left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
                                  inner join ssoperateri o on p.operateriidfrom=o.operateriid  
                                  inner join ssoperateri op on p.operateriidto=op.operateriid  
                                  inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
                                  left outer join Valute v on p.ValuteId=v.ValuteId  
                                  left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId  
        
        left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
     --   left join PtProces pr on pr.PtProcesId = p.PtProcesId  
     where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)  
                                 and (p.partneriid=@partneriid or @partneriid is null)  
                                 and (p.KUPStatusId=@KUPStatusId or @KUPStatusId is null)  
                                      and ((@Daid=0 and p.Zaprimljeno=0)or (@Daid=1 and p.Zaprimljeno=1)or (@Daid=2 and p.Zaprimljeno=2)
                                       or (@DaId is null and (p.Zaprimljeno=0 or p.Zaprimljeno=1 or p.zaprimljeno=2)))
                                 and (convert(varchar(50),p.urudzbenibroj) + '' +convert(varchar(50),p.iznos)/*+ '' + org.Naziv*/) like '%'+@filter+'%'  
       
                  
                 order by p.KUPPrometId  
 
  
        return (0)  
 end  
  
  
        if @action=1 --KUP -refresh nakon update-a  
         begin  
                 
                declare @Zaprimljeno int,@OperateriIdTo int  
                set @zaprimljeno=(select zaprimljeno from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id))  
                set @OperateriIdTo= (select OperateriIdTo from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id))  
  
           if not exists (  select * 
                            from KUPPrometLog l inner join KUPStatus s on l.KUPStatusId=s.KUPStatusId  
                            where kupprometlogid in (select max(kupprometlogid) from kupprometlog where kupprometid=@id)  
                            and @OperateriIdTo = @OperateriId and s.Korak = 0 and @Zaprimljeno=1) begin  
                              
                                EXEC spRaiserror 69001, 'Nemate prava izmjene dokumenta!'  
           return  
          end  
  
        
  
    select p.KUPPrometId,  
       p.KUPStatusId,
       formatid=case when p.zaprimljeno=0 then 'red' else ' ' end,  
       p.KUPVrstaPosteId,  
       vp.naziv,  
       p.UrudzbeniBroj, 
       p.DatumPrijema,  
       p.datumdokumenta as [DatumDokumenta],  
    
       p.Zaprimljeno,  
       op.Naziv , 
       p.datumunosa, 
       convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],  
       o.Naziv , 
       p.PartneriId,  
       pt.Naziv as Partner,   
       p.Broj,   
                          coalesce(p.narudzbe,'') as [Narudzba1500],  
      
      fd.brojdok as [BrojUFE1500],  
       p.DatumValute, 
       p.Opis, 
       p.Iznos, 
       p.OrgShemaId--,p.KUPPrometLogid  --jasminka 27.09.
       ,p.KupStatusId,  
       org.Naziv , 
       valuteid=p.valuteid,  
       (v.Oznaka+space(1)+v.Naziv) as Valuta,  
        p.Posiljatelj ,         p.Rjesenje,  
       p.UrudzbeniBrojVeza,  
                         p.NabNarGlaId     
                           
     -- ,pr.PtProcesId  
      , p.Dana    
    --   , p.godinaurbr           
    from KUPPromet p --inner join KUPPrometLog l on p.KUPPrometId=l.KUPPrometId  
    --inner join #tmpKUPlog l on p.kupprometid = l.kupprometid  
            left outer join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
            left outer join Partneri pt on p.PartneriId=pt.PartneriId  
            left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
            left outer join ssoperateri o on p.operateriidfrom=o.operateriid  
            left outer join ssoperateri op on p.operateriidto=op.operateriid  
            left outer join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
            left outer join Valute v on p.ValuteId=v.ValuteId  
                                           
           left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
     --      left join PtProces pr on pr.PtProcesId = p.PtProcesId  
    where        
    p.KUPPrometId=@ID  
   
     
     return
   end  
  
  
if @action=111 --KUP -refresh nakon update-a  
 begin  
      
  
      
    select p.KUPPrometId,  
       p.KUPStatusId, 
       formatid=case when p.zaprimljeno=0 then 'red' else ' ' end,  
       p.KUPVrstaPosteId,  
       vp.naziv,  
       p.UrudzbeniBroj,   
       p.DatumPrijema, 
       p.datumdokumenta as [DatumDokumenta],  
   
       p.Zaprimljeno, 
       op.Naziv , 
       p.datumunosa, 
       convert(varchar,p.DatumUnosa,108) as [VrijemePromjene1500],  
       o.Naziv , 
       p.PartneriId,  
       pt.Naziv as Partner,   
       p.Broj,
                       
                          coalesce(p.narudzbe,'') as [Narudzba1500],  
       
      fd.brojdok as [BrojUFE1500],  
       p.DatumValute,  
       p.Opis, 
       p.Iznos, 
       p.OrgShemaId
       ,p.KupStatusId,  
       org.Naziv , 
       valuteid=p.valuteid,  
       (v.Oznaka+space(1)+v.Naziv) as Valuta,  
        p.Posiljatelj ,  
       p.Rjesenje,  
       p.UrudzbeniBrojVeza,  
                         p.NabNarGlaId     
                          
     -- ,pr.PtProcesId  
                 , cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]    
  
    from KUPPromet p --inner join KUPPrometLog l on p.KUPPrometId=l.KUPPrometId  
   -- inner join #tmpKUPlog l on p.kupprometid = l.kupprometid  
            left outer join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
            left outer join Partneri pt on p.PartneriId=pt.PartneriId  
            left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
            left outer join ssoperateri o on p.operateriidfrom=o.operateriid  
            left outer join ssoperateri op on p.operateriidto=op.operateriid  
            left outer join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
            left outer join Valute v on p.ValuteId=v.ValuteId  
                                              
                                             left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
     --      left join PtProces pr on pr.PtProcesId = p.PtProcesId  
    where        
    p.KUPPrometId=@ID  
   
     return
        end  
  
  
        if @action=2  --KUP i KUPDostavnaKnjiga -Povijest  
        begin  
   select l.kupprometlogid,formatid=case when l.zaprimljeno=0 then 'red' else ' ' end,l.kupprometid,l.kupstatusid,s.opis as [Status2000],l.Zaprimljeno as [Zaprimljeno1000],o.Naziv as [Od koga1800],  
                        op.Naziv as [Kome1800],l.DatumUnosa as [DatumPromjene1700], CONVERT(varchar,l.DatumUnosa,108 ) as [VrijemePromjene1700],l.Opis as [Opis5000]  
                 from KUPPrometLog l inner join KUPStatus s on l.kupstatusid=s.kupstatusid  
                                     inner join ssoperateri o on l.operateriidfrom=o.operateriid  
                                     inner join ssoperateri op on l.operateriidto=op.operateriid  
                 where  l.KUPPrometId=@ID  
                 order by l.KUPPrometLogId  
     
                return(0)  

       end  
  
         if @action=3  --KUPDostavnaKnjiga-filter forma,operater nije u urudžbenom  
            begin  
  
            select   
                     p.KUPPrometId, formatid=case p.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end,  
                     p.KUPStatusId,
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
                    
                     coalesce(p.narudzbe,'') as [Narudzba1500],  
                   
                    fd.brojdok as [BrojUFE1500],  
                     p.DatumValute as [DatumValute1500],  
                     p.Opis as [Opis1800],  
                     p.Iznos as [Iznos1200],  
                     
                     org.Naziv as [OrgShema2000],  
                     valuteid=p.valuteid,  
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
                     p.Posiljatelj as [Pošiljatelj1000],  
                     p.Rjesenje as [Rješenje1200],  
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
                     p.Scan as [Scan],  
                     p.NabNarGlaId 
                   
              --      ,pr.naziv as VrstaTroška  
                        , cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]    
      
                    from KUPPromet p --inner join #tmpKUPlog  
                   --(select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))  
                  --l on p.kupprometid = l.kupprometid  
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId  
                                  left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
                                  inner join ssoperateri o on p.operateriidfrom=o.operateriid  
                                  inner join ssoperateri op on p.operateriidto=op.operateriid  
                                  inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
                                  left outer join Valute v on p.ValuteId=v.ValuteId  
                                  left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId  
                                  
                                   left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
     --    left join PtProces pr on pr.PtProcesId = p.PtProcesId  
  
     where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)  
                                 and (p.partneriid=@partneriid or @partneriid is null)  
                                    and ((@Daid=0 and p.Zaprimljeno=0)or (@Daid=1 and p.Zaprimljeno=1)or (@Daid=2 and p.Zaprimljeno=2)
                                       or (@DaId is null and (p.Zaprimljeno=0 or p.Zaprimljeno=1 or p.zaprimljeno=2)))
                                 and p.operateriidto=@OperateriId  
                                          
                           
                 order by p.KUPPrometId  
    return(0)  

  end  
  
--              
  
       if @action=4  --KUP-startupstora --sve što ima korak=0-->u urudžbenom  
       begin  
  
         
            select   
                     p.KUPPrometId  
                     
                      , formatid = case when p.zaprimljeno = 0 then 'red'   
						   when p.zaprimljeno = 2 then 'blue'   
						   when p.KupStatusOdobrenjeId ! = 1 then 'green'  
						   else 'black' end ,  
                     p.KUPStatusId,
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
                  
                     coalesce(p.narudzbe,'') as [Narudzba1500],  
                    
                    fd.brojdok as [BrojUFE1500],  
                     p.DatumValute as [DatumValute1500],  
                     p.Opis as [Opis1800],  
                     p.Iznos as [Iznos1200],  
                      
                     org.Naziv as [OrgShema2000],  
                     valuteid=p.valuteid,  
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
                     p.Posiljatelj as [Pošiljatelj1000],  
                     p.Rjesenje as [Rješenje1200],  
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
                     p.Scan as [Scan],  
                     p.NabNarGlaId 
                    , kso.Opis as [Status odobravanja]  
                 --   ,pr.naziv as VrstaTroška  
                    , cast(datediff(d,p.datumPrijema,@DatDo) as int) as [Dana0700]    
				--	, odj.Naziv as Odjel
     from KUPPromet p --inner join #tmpKUPlog l  
          --(select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))  
       --on p.kupprometid = l.kupprometid  
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId  
                                  left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
                                  inner join ssoperateri o on p.operateriidfrom=o.operateriid  
                                  inner join ssoperateri op on p.operateriidto=op.operateriid  
                                  inner join KUPStatus ks on p.KUPStatusId = ks.KupStatusId  
                                  left outer join Valute v on p.ValuteId=v.ValuteId  
                                  left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId  
                                  
                                  left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
                                  left join KupStatusOdobrenje kso on p.KupStatusOdobrenjeId = kso.KupStatusOdobrenjeId  
						--		  left join Odjel odj on p.odjelid = odj.OdjelId
     --       left join PtProces pr on pr.PtProcesId = p.PtProcesId  
  
  
      where     p.Zaprimljeno <> 2 
	 -- and datumprijema > DATEADD(m, -3, current_timestamp)
	and datumprijema between cast(year(getdate()) as varchar(4))+'0101' and cast(year(getdate())as varchar(4))+'1231'
                 order by p.KUPPrometId desc  

				 option (recompile)
  
     return (0)  
 end  
 
 
       if @action=5 --KUPDostavnaKnjiga-startupstora  
        begin  


			 select *   
			 into #tempBezArhiviranih
			 --from #tmpKUPlog 
			 from KupPromet
			 where zaprimljeno not in (1,2)
      
/*
		--select * from kupstatus
		
			  select *
			  into #tempZaKnjizenje
			  from kupprometlog 
			  where operateriidto=@operateriid 
			  and kupprometid in (select kupprometid from #tempBezArhiviranih)
			  and kupstatusid=(select KUPStatusId from KUPStatus where Opis like 'Računovodstvo')
			  and zaprimljeno not in (1)
			  order by kupprometid
			  */
			
			   select *
		  into #tempZaKnjizenje --odobreni
		  from kupprometlog 
		  where operateriidto=@operateriid 
		  and kupprometid in (select kupprometid from #tempBezArhiviranih)
		  and kupstatusid in (
	  		  --select kupstatusid from kupstatus where Opis = 'ODJEL RAČUNOVODSTVA'
	  			 select distinct kso.KUPStatusId from KUPVrstaPoste kv
					inner join KupStatus kso on kv.KUPStatusOdobrenoId = kso.KUPStatusId
					where KUPStatusOdobrenoId is not null 

					union 

				select distinct kson.KUPStatusId from KUPVrstaPoste kvn
					inner join KupStatus kson on kvn.KUPStatusNijeOdobId = kson.KUPStatusId
					where KUPStatusNijeOdobId is not null

				 )
		  and zaprimljeno not in (1)
		  order by kupprometid
		 
	  
				create index #tempZaKnjizenje on #tempZaKnjizenje(kupprometid)


                   select   
                     p.KUPPrometId  
                     ,formatid = case when p.zaprimljeno = 0 then 'red'   
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
                     
                     fd.brojdok as [BrojUFE2000],  
                     p.DatumValute as [DatumValute1500],  
                     p.Opis as [Opis1800],  
                     p.Iznos as [Iznos1200],  
                     
                     org.Naziv as [OrgShema2000],  
                     valuteid=p.valuteid,  
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
                     p.Posiljatelj as [Pošiljatelj1000],  
                     p.Rjesenje as [Rješenje1200],  
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
                     p.Scan as [Scan],   
                     p.NabNarGlaId--,  
                     
				--	coalesce ((p.UFABroj+' - '+p.registrator), '') as Registrator 
                    ,kso.Opis as [Status odobravanja]  
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
                                 
                                   left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
                                   left outer join KupStatusOdobrenje kso on p.KupStatusOdobrenjeId = kso.KupStatusOdobrenjeId  
  
                  where    
                                
                                 p.operateriidto=@OperateriId  
                                 and   p.Zaprimljeno <> 2  
							--27.11.2024. VV komentirala	     and coalesce(p.kupstatusodobrenjeid,0) in (0,1,4)  --u dostavnoj vide samo dokumente koji nisu u procesu odobravanja ili su neodobreni

     
               

union all



			 select   
                     p.KUPPrometId  
                         , formatid = case when l.zaprimljeno = 0 and p.KupStatusOdobrenjeId is null then 'red'   
												   when l.zaprimljeno = 2  then 'blue'
												   when l.zaprimljeno = 0 and p.KupStatusOdobrenjeId=3 then 'boldred'  
													 else ' ' end ,  
                     l.KUPStatusId,'' as [S0500],  
                     p.KUPVrstaPosteId,  
                     vp.naziv as [VrstaPoste2000],  
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],  
                     p.DatumPrijema as [DatumPrijema1200],  
                     p.datumdokumenta as [DatumDokumenta],  
                     ks.opis as [Korak2000],  
                     l.Zaprimljeno as [Zaprimljeno1000],  
                     op.Naziv as [Kome1000],  
                     l.datumunosa as [Datum promjene2000],  
                     convert(varchar,l.DatumUnosa,108) as [VrijemePromjene1500],  
                     o.Naziv as [Od koga1000],  
                     p.PartneriId,  
                     pt.Naziv as [Naziv2000],  
                     p.Broj as [Broj2000],  
                     nar.Broj as [BrojNarudžbe1500],  
                       
                     fd.brojdok as [BrojUFE2000],  
                     p.DatumValute as [DatumValute1500],  
                     p.Opis as [Opis1800],  
                     p.Iznos as [Iznos1200],  
                     
                     org.Naziv as [OrgShema2000],  
                     valuteid=p.valuteid,  
                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],  
                     p.Posiljatelj as [Pošiljatelj1000],  
                     p.Rjesenje as [Rješenje1200],  
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],  
                     p.Scan as [Scan],   
                     p.NabNarGlaId--,  
                    
				--	coalesce ((p.UFABroj+' - '+p.registrator), '') as Registrator 
     ,  '' as [Status odobravanja]    
       
							   from KUPPromet p inner join  #tempZaKnjizenje l  on p.kupprometid = l.kupprometid
--											(select * from kupprometlog 
--                                             where operateriidto=@operateriid 
--                                             and kupstatusid=(select kupstatusid from kupstatus where korak=1)
--                                             )l on p.kupprometid = l.kupprometid 

											  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId  
											  left outer join Partneri pt on p.PartneriId=pt.PartneriId  
											  left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId  
											  inner join ssoperateri o on l.operateriidfrom=o.operateriid  
											  inner join ssoperateri op on l.operateriidto=op.operateriid  
											  inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId  
											  left outer join Valute v on p.ValuteId=v.ValuteId  
											  left outer join NabNarGla nar on p.NabNarGlaId=nar.NabNarGlaId  
											 
											   left outer join fakturedob fd on fd.kupprometid = p.kupprometid  
											

  
				 where l.zaprimljeno =0   
				 and p.kupstatusodobrenjeid=3 --u dostavnoj samo tete iz računovodstva(u joinu where korak=1) vide one koji su odobreni i spremni za knjiženje,a nisu zaprimljeni kao bold red
				--and fd.indglk = 0

 union all  
               
             select p.KUPPrometId  
     , 'green' as formatid  
                    , null as KUPStatusId  
                    , '' as [S0500]  
                    , p.KUPVrstaPosteId,  
                     vp.naziv as [VrstaPoste2000],  
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],  
                     p.DatumPrijema as [DatumPrijema1200],  
                     p.datumdokumenta as [DatumDokumenta],  
                     '' as [Korak2000],  
                     '' as [Zaprimljeno1000],  
                    '' as [Kome1000], 
				
                     odl
… (skraćeno — puni tekst na stranici usporedbe)