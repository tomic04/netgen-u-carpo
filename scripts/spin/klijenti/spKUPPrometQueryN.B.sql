

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
@ugovori char(1)='0'


AS

	if @action=0 --KUP filter forma
	begin
       
                     select 
                     p.KUPPrometId,formatid=case l.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end,  
                     l.KUPStatusId,'' as [!S0500],
                     p.KUPVrstaPosteId,
                     vp.naziv as [VrstaPoste2000],
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],
                     p.DatumPrijema as [DatumPrijema1200],
                     ks.opis as [Korak2000],
                     l.Zaprimljeno as [Zaprimljeno1000],
                     op.Naziv as [Kome1000],
                     l.datumunosa as [Datum promjene2000],
                     o.Naziv as [Od koga1000],
                     p.PartneriId,
                     pt.Naziv as [Naziv2000] ,
--                     p.Broj as [Broj2000],
--                     p.BrojUFE as [BrojUFE2000],
--                     p.DatumValute as [DatumValute1500],
                     p.Opis as [Opis1800],
--                     p.Iznos as [Iznos1200],
                     org.Naziv as [OrgShema2000],
                    -- p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,                                      
--                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
                     p.Posiljatelj as [Posiljatelj/Primatelj1000] ,
--                     p.Rjesenje as [Rješenje1200],
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
                     p.Scan as [Scan]    
                                
                    
		 from KUPPromet p inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId
                                  inner join Orgshema org on p.OrgShemaId=org.OrgShemaId
                                  inner join ssoperateri o on l.operateriidfrom=o.operateriid
                                  inner join ssoperateri op on l.operateriidto=op.operateriid
                                  inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
                                  left outer join Valute v on p.ValuteId=v.ValuteId
								inner join ssmeni sm on vp.meniid = sm.meniid

				 where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)
                                 and (p.partneriid=@partneriid or @partneriid is null)
                                 and (l.KUPStatusId=@KUPStatusId or @KUPStatusId is null)
                                 and ((@Daid=0 and l.Zaprimljeno=0)or (@Daid=1 and l.Zaprimljeno=1)or (@Daid=2 and l.Zaprimljeno=2)
                                       or (@DaId is null and (l.Zaprimljeno=0 or l.Zaprimljeno=1)))
                                 and (convert(varchar(50),p.urudzbenibroj) + '' +convert(varchar(50),p.iznos)+ '' + org.Naziv) like '%'+@filter+'%'
								  and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                 order by p.KUPPrometId
     


         

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
			        and s.Korak = 0 and @Zaprimljeno=1) begin
                            
                                EXEC spRaiserror 69001, 'Nemate prava izmjene dokumenta!' 
				return
			end

      

			 select p.KUPPrometId,
						 l.KUPStatusId,'' as [!S0500], formatid=case when l.zaprimljeno=0 then 'red' else ' ' end,
						 p.KUPVrstaPosteId,
						 vp.naziv, --as [VrstaPoste2000],
						 p.UrudzbeniBroj, --as [UrudzbeniBroj1200],
						 p.DatumPrijema, --as [DatumPrijema1200],
						 --ks.opis, --as [Korak2000],
						 l.Zaprimljeno, --as [Zaprimljeno1000],
						 op.Naziv , --as [Kome2000],
						 l.datumunosa, --as [Datum promjene2000],
						 o.Naziv , --as [Od koga2000],
						 p.PartneriId,
						 pt.Naziv , 
--						 p.Broj, --as [Broj2000],
--						 p.BrojUFE, --as [BrojUFE2000],
--						 p.DatumValute, --as [DatumValute1500],
						 p.Opis, --as [Opis0800],
--						 p.Iznos, --as [Iznos1200],
						 p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
						 org.Naziv , --as [OrgShema2000],
						 valuteid=p.valuteid,
--						 (v.Oznaka+space(1)+v.Naziv) as Valuta,
						  p.Posiljatelj ,
--						 p.Rjesenje,
						 p.UrudzbeniBrojVeza    
	               
				from KUPPromet p inner join KUPPrometLog l on p.KUPPrometId=l.KUPPrometId
											 left outer join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
											 left outer join Partneri pt on p.PartneriId=pt.PartneriId
											 left outer join Orgshema org on p.OrgShemaId=org.OrgShemaId
											 left outer join ssoperateri o on l.operateriidfrom=o.operateriid
											 left outer join ssoperateri op on l.operateriidto=op.operateriid
											 left outer join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
											 left outer join Valute v on p.ValuteId=v.ValuteId
                                             inner join ssmeni sm on vp.meniid = sm.meniid

				where 				 
				p.KUPPrometId=@ID
				and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
   
        end

        if @action=2  --KUP i KUPDostavnaKnjiga -Povijest
	       begin
		 select l.kupprometlogid,formatid=case when l.zaprimljeno=0 then 'red' else ' ' end,l.kupprometid,l.kupstatusid,s.opis as [Status2000],l.Zaprimljeno as [Zaprimljeno1000],o.Naziv as [Od koga1800],
                        op.Naziv as [Kome1800],l.DatumUnosa as [DatumPromjene1700]
                 from KUPPrometLog l inner join KUPStatus s on l.kupstatusid=s.kupstatusid
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
                     p.KUPPrometId,formatid=case l.zaprimljeno when 0 then 'R' when 2 then 'G' else ' ' end,
                     l.KUPStatusId,'' as [!S0500],
                     p.KUPVrstaPosteId,
                     vp.naziv as [VrstaPoste2000],
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],
                     p.DatumPrijema as [DatumPrijema1200],
                     ks.opis as [Korak2000],
                     l.Zaprimljeno as [Zaprimljeno1000],
                     op.Naziv as [Kome1000],
                     l.datumunosa as [Datum promjene2000],
                     o.Naziv as [Od koga1000],
                     p.PartneriId,
                     pt.Naziv as [Naziv2000],
--                     p.Broj as [Broj2000],
--                     p.BrojUFE as [BrojUFE2000],
--                     p.DatumValute as [DatumValute1500],
                     p.Opis as [Opis1800],
--                     p.Iznos as [Iznos1200],
                     --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
                     org.Naziv as [OrgShema2000],
                     valuteid=p.valuteid,
--                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
                     p.Posiljatelj as [Pošiljatelj1000],
--                     p.Rjesenje as [Rješenje1200],
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
                     p.Scan as [Scan]
                     

                  from KUPPromet p inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId
                                  inner join Orgshema org on p.OrgShemaId=org.OrgShemaId
                                  inner join ssoperateri o on l.operateriidfrom=o.operateriid
                                  inner join ssoperateri op on l.operateriidto=op.operateriid
                                  inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
                                  left outer join Valute v on p.ValuteId=v.ValuteId
                                  inner join ssmeni sm on vp.meniid = sm.meniid


				 where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)
                                 and (p.partneriid=@partneriid or @partneriid is null)
                                 and ((@Daid=0 and l.Zaprimljeno=0)or (@Daid=1 and l.Zaprimljeno=1)or (@Daid=2 and l.Zaprimljeno=2)
                                     or (@DaId is null and (l.Zaprimljeno=0 or l.Zaprimljeno=1)))
                                 and l.operateriidto=@OperateriId
								 and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                                 
                                      
                                       
                         
                 order by p.KUPPrometId
                return(0)

        

              end

--             if @action=3 and (select min(ks1.korak) from kupstatus ks1 inner join kupstatusoperateri ko on ks1.kupstatusid=ko.kupstatusid
--                            where ko.operateriid=@OperateriId)=0  --KUPDostavnaKnjiga-filter forma,operater je u urudžbenom
-- 	       begin
-- 
-- 		  select 
--                      p.KUPPrometId,formatid=case when l.zaprimljeno=0 then 'R' else ' ' end,
--                      l.KUPStatusId,'' as [!S0500],
--                      p.KUPVrstaPosteId,
--                      vp.naziv as [VrstaPoste2000],
--                      p.UrudzbeniBroj as [UrudzbeniBroj1200],
--                      p.DatumPrijema as [DatumPrijema1200],
--                      ks.opis as [Korak2000],
--                      l.Zaprimljeno as [Zaprimljeno1000],
--                      op.Naziv as [Kome1000],
--                      l.datumunosa as [Datum promjene2000],
--                      o.Naziv as [Od koga1000],
--                      p.PartneriId,
--                      pt.Naziv as [Naziv2000],
--                      p.Broj as [Broj2000],
--                      p.BrojUFE as [BrojUFE2000],
--                      p.DatumValute as [DatumValute1500],
--                      p.Opis as [Opis1800],
--                      p.Iznos as [Iznos1200],
--                      --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
--                      org.Naziv as [OrgShema2000],
--                      valuteid=p.valuteid,
--                      (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
--                      p.Posiljatelj as [Pošiljatelj1000],
--                      p.Rjesenje as [Rješenje1200],
--                      p.UrudzbeniBrojVeza as [VezaNaUrBroj1200]
--                      
-- 
--                   from KUPPromet p inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
--                                   inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
--                                   left outer join Partneri pt on p.PartneriId=pt.PartneriId
--                                   inner join Orgshema org on p.OrgShemaId=org.OrgShemaId
--                                   inner join ssoperateri o on l.operateriidfrom=o.operateriid
--                                   inner join ssoperateri op on l.operateriidto=op.operateriid
--                                   inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
--                                   left outer join Valute v on p.ValuteId=v.ValuteId
-- 
-- 				 where ((p.datumprijema between @datumprijemaod and @datumprijemado) or @datumprijemaod is null or @datumprijemado is null)
--                                  and (p.partneriid=@partneriid or @partneriid is null)
--                                  and ((@Daid=0 and l.Zaprimljeno=0)or (@Daid=1 and l.Zaprimljeno=1)
--                                      or @DaId is null)
--                                  and l.operateriidto=@OperateriId 
--                                  or l.Zaprimljeno=0
--                                       
--                                        
--                          
--                  order by p.KUPPrometId
--                 return(0)
-- 
--         
-- 
--               end


       if @action=4  --KUP-startupstora --sve što ima korak=0-->u urudžbenom
	begin

       
		 select 
                     p.KUPPrometId,formatid=case l.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end ,
                     l.KUPStatusId,'' as [!S0500],
                     p.KUPVrstaPosteId,
                     vp.naziv as [VrstaPoste2000],
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],
                     p.DatumPrijema as [DatumPrijema1200],
                     ks.opis as [Korak2000],
                     l.Zaprimljeno as [Zaprimljeno1000],
                     op.Naziv as [Kome1000],
                     l.datumunosa as [Datum promjene2000],
                     o.Naziv as [Od koga1000],
                     p.PartneriId,
                     pt.Naziv as [Naziv2000],
--                     p.Broj as [Broj2000],
--                     p.BrojUFE as [BrojUFE2000],
--                     p.DatumValute as [DatumValute1500],
                     p.Opis as [Opis1800],
--                     p.Iznos as [Iznos1200],
                     --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
                     org.Naziv as [OrgShema2000],
                     valuteid=p.valuteid,
--                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
                     p.Posiljatelj as [Pošiljatelj/;Primatelj1000],
--                     p.Rjesenje as [Rješenje1200],
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
                     p.Scan as [Scan]                 
                    
		 from KUPPromet p inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId
                                  inner join Orgshema org on p.OrgShemaId=org.OrgShemaId
                                  inner join ssoperateri o on l.operateriidfrom=o.operateriid
                                  inner join ssoperateri op on l.operateriidto=op.operateriid
                                  inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
                                  left outer join Valute v on p.ValuteId=v.ValuteId
                                 inner join ssmeni sm on vp.meniid = sm.meniid


				  where --ks.korak=0  
		          vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                 order by p.KUPPrometId


        

                return (0)
         end

       if @action=5 --KUPDostavnaKnjiga-startupstora
	       begin
		  select 
                     p.KUPPrometId,formatid=case l.zaprimljeno when 0 then 'red' when 2 then 'blue' else ' ' end ,
                     l.KUPStatusId,'' as [!S0500],
                     p.KUPVrstaPosteId,
                     vp.naziv as [VrstaPoste2000],
                     p.UrudzbeniBroj as [UrudzbeniBroj1200],
                     p.DatumPrijema as [DatumPrijema1200],
                     ks.opis as [Korak2000],
                     l.Zaprimljeno as [Zaprimljeno1000],
                     op.Naziv as [Kome1000],
                     l.datumunosa as [Datum promjene2000],
                     o.Naziv as [Od koga1000],
                     p.PartneriId,
                     pt.Naziv as [Naziv2000],
--                     p.Broj as [Broj2000],
--                     p.BrojUFE as [BrojUFE2000],
--                     p.DatumValute as [DatumValute1500],
                     p.Opis as [Opis1800],
--                     p.Iznos as [Iznos1200],
                     --p.OrgShemaId,l.KUPPrometLogid,l.KupStatusId,
                     org.Naziv as [OrgShema2000],
                     valuteid=p.valuteid,
--                     (v.Oznaka+space(1)+v.Naziv) as [Valuta1200],
                     p.Posiljatelj as [Pošiljatelj1000],
--                 p.Rjesenje as [Rješenje1200],
                     p.UrudzbeniBrojVeza as [VezaNaUrBroj1200],
                     p.Scan as [Scan] 

                  from KUPPromet p inner join (select * from kupprometlog where kupprometlogid in (select max(kupprometlogid) from kupprometlog group by kupprometid))l on p.kupprometid = l.kupprometid
                                  inner join KUPVrstaPoste vp on p.KUPVrstaPosteId=vp.KUPVrstaPosteId
                                  left outer join Partneri pt on p.PartneriId=pt.PartneriId
                                  inner join Orgshema org on p.OrgShemaId=org.OrgShemaId
                                  inner join ssoperateri o on l.operateriidfrom=o.operateriid
                                  inner join ssoperateri op on l.operateriidto=op.operateriid
                                  inner join KUPStatus ks on l.KUPStatusId = ks.KupStatusId
                                  left outer join Valute v on p.ValuteId=v.ValuteId
                                  inner join ssmeni sm on vp.meniid = sm.meniid

				 where /*l.zaprimljeno=0 
                                 and*/ l.operateriidto=@OperateriId
						and vp.MeniId=(select MeniId from ssmeni where opis like 'Knjiga ulazne%')
                 order by p.KUPPrometId
                return(0)
        
              end

      
return(0)









