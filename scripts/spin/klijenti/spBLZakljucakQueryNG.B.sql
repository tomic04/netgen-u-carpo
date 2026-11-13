
  
create  PROCEDURE [dbo].[spBLZakljucakQueryNG]  
@Action int = null,  
@ID int = null,  
@DatumDo datetime = null,  
@Ind tinyint = 0,
@node varchar(20)=''  
  
AS  


  declare @datumod1 datetime, @vrijemeod1 datetime,@datumod datetime  

         if len(@Node)>0 begin
			  set @datumod=left(@Node,8)
			  set @datumdo=right(@Node,8)
			 end
		  else begin
			  set @datumod=(select convert(varchar(8),GETDATE(),112))  
			  --set @datumdo=(select convert(varchar(8),dateadd(d,1,GETDATE()),112))
		  end
       



  set @ind = coalesce(@ind,0)  
  
 if @Action = 1    --zaključak
  begin  
   --datum zadnjeg zaključka  
--   set @datumod1 = (select max(Datum) from BLZakljucci where BlagajneId = @ID)   
         select @datumod1=datum, @vrijemeod1=vrijeme from blzakljucci where vrijeme = (select vrijeme=max(Vrijeme) from BLZakljucci where BlagajneId = @ID)  
  
  
   --ako je zaključak i postoje nezaključeni dokumenti  
   if @ind = 1 and exists(select b.BLPrometGlaId   
       from BLPrometGla b inner join bldokumenti d on b.bldokumentiid = d.bldokumentiid   
         where d.BlagajneId = @ID and b.BLZakljucciId is null and b.Status <> 1  
           and b.Datum <= @DatumDo)  
    begin  
     RAISERROR ('U izabranom razdoblju postoje dokumenti koji nisu zaključeni. Morate ih zaključiti prije zaključka Blagajne!', 16, 1)  
    end  
  
    select  Opis = 'Promet Blagajne',   
       Primitak = coalesce(sum(d.UplataKn), 0),   
       Izdatak = coalesce(sum(d.Isplata), 0)  ,
       PrimitakVal = coalesce(sum(d.UplataVal), 0),   
       IzdatakVal = coalesce(sum(d.IsplataVal), 0)  
   into #temp1  
   from BLPrometGla b left join BLPrometDet d on b.BLPrometGlaId = d.BLPrometGlaId  
          inner join BLDokumenti dk on b.BLDokumentiId = dk.BLDokumentiId  
   where dk.BlagajneId = @ID and b.BLZakljucciId is null  
      and b.Datum <= @DatumDo   
   union all  
   select  Opis = 'Saldo od ' + (case when @datumod1 is null then '' else convert(varchar, Datum, 104)  end),   --+ ' ' + convert(varchar, Vrijeme, 108)   
       Primitak = coalesce(PrSaldoKn, 0) + coalesce(UplataKN, 0) - coalesce(IsplataKn, 0),   
       Izdatak = null  ,
       PrimitakVal = coalesce(PrSaldoVAl, 0) + coalesce(UplataVal, 0) - coalesce(IsplataVal, 0),   
       IzdatakVal = null
   from BLZakljucci  
   where BlagajneId = @ID and Vrijeme = @vrijemeOd1    --Datum = @DatumOd1  
  
   select Opis, Primitak, Izdatak ,PrimitakVal, IzdatakVal 
   from #temp1  
   union all  
   select 'Ukupni primitak', sum(coalesce(Primitak, 0)), null ,sum(coalesce(PrimitakVal,0.00)) as PrimitakDev, sum(coalesce(IzdatakVal,0.00)) as IzdatakDev  
   from #temp1  
   union all  
   select 'Izdatak', sum(coalesce(Izdatak, 0)), null  , sum(coalesce(IzdatakVal ,0.00)), null
   from #temp1  
   union all  
   select 'Saldo na ' + convert(varchar, @DatumDo, 104) , sum(coalesce(Primitak, 0)) - sum(coalesce(Izdatak, 0)), null, sum(coalesce(PrimitakVal, 0)) - sum(coalesce(IzdatakVal, 0)),null    --+ ' ' + convert(varchar, @DatumDo, 108)  
   from #temp1  
  
   return  
  end  


if @Action = 111   --stanje
  begin  
   --datum zadnjeg zaključka  
--   set @datumod1 = (select max(Datum) from BLZakljucci where BlagajneId = @ID)   
         select @datumod1=datum, @vrijemeod1=vrijeme from blzakljucci where vrijeme = (select vrijeme=max(Vrijeme) from BLZakljucci where BlagajneId = @ID)  
  
  
--   --ako je zaključak i postoje nezaključeni dokumenti  
--   if @ind = 1 and exists(select b.BLPrometGlaId   
--       from BLPrometGla b inner join bldokumenti d on b.bldokumentiid = d.bldokumentiid   
--         where d.BlagajneId = @ID and b.BLZakljucciId is null and b.Status <> 1  
--           and b.Datum <= @DatumDo)  
--    begin  
--     RAISERROR ('U izabranom razdoblju postoje dokumenti koji nisu zaključeni. Morate ih zaključiti prije zaključka Blagajne!', 16, 1)  
--    end  
  
  
    select  Opis = 'Promet Blagajne',   
        Primitak = coalesce(sum(d.UplataKn), 0),   
       Izdatak = coalesce(sum(d.Isplata), 0)  ,
       PrimitakVal = coalesce(sum(d.UplataVal), 0),   
       IzdatakVal = coalesce(sum(d.IsplataVal), 0)  
   into #temp111  
   from BLPrometGla b left join BLPrometDet d on b.BLPrometGlaId = d.BLPrometGlaId  
          inner join BLDokumenti dk on b.BLDokumentiId = dk.BLDokumentiId  
   where dk.BlagajneId = @ID and b.BLZakljucciId is null  
      and b.Datum <= @DatumDo   
   union all  
   select  Opis = 'Saldo od ' + (case when @datumod1 is null then '' else convert(varchar, Datum, 104)  end),   --+ ' ' + convert(varchar, Vrijeme, 108)   
       Primitak = coalesce(PrSaldoKn, 0) + coalesce(UplataKN, 0) - coalesce(IsplataKn, 0),   
       Izdatak = null  ,
       PrimitakVal = coalesce(PrSaldoVAl, 0) + coalesce(UplataVal, 0) - coalesce(IsplataVal, 0),   
       IzdatakVal = null 
   from BLZakljucci  
   where BlagajneId = @ID and Vrijeme = @vrijemeOd1    --Datum = @DatumOd1  
  
   select Opis, Primitak, Izdatak ,PrimitakVal, IzdatakVal 
   from #temp111  
   union all  
   select 'Ukupni primitak', sum(coalesce(Primitak, 0)), null ,sum(coalesce(PrimitakVal,0.00)) as PrimitakDev, null as IzdatakDev  
   from #temp111 
   union all  
   select 'Izdatak', sum(coalesce(Izdatak, 0)), null  , sum(coalesce(IzdatakVal ,0.00)), null
   from #temp111  
   union all  
   select 'Saldo na ' + convert(varchar, @DatumDo, 104) , sum(coalesce(Primitak, 0)) - sum(coalesce(Izdatak, 0)), null, sum(coalesce(PrimitakVal, 0)) - sum(coalesce(IzdatakVal, 0)),null    --+ ' ' + convert(varchar, @DatumDo, 108)  
   from #temp111  
  
   return  
  end  
  
  if @Action = 2 begin -- za kreiranje broja  
   select d.BLDokumentiId,d.VrstaDokId,coalesce(d.Prefix,''),coalesce(d.Sufix,''),vd.Opis as OpisVp,  coalesce(d.Template,'') as Template  
   from BLDokumenti d, VrstaDok vd,VrstaProm vp  
   where vd.VrstaPromId = vp.VrstaPromId and  
     d.VrstaDokId = vd.VrstaDokId and  
       d.BlagajneId = @ID and lower(rtrim(vd.Opis)) = 'zaključak'   
  end   
  
  if @Action = 5 begin   
    -- ispis blagajničkog zaključka  
    -- spaja: u Broj konto + broj  
    --      u glavaopis glavaopis + broj računa ...  
   select  g.Datum, g.Vrijeme, g.BrojDok,   
       Broj = coalesce(kn.konto + ' ','') + rtrim(g.Broj),   
       GlavaOpis = coalesce(rtrim(g.Opis) + ' ','') + (case  when d.tablicaveza = 'Fakture' then '(' + rtrim(f.broj) + ')'  
                                  when d.tablicaveza = 'FaktureDob' then '(' + rtrim(fd.broj) + ')'  
                                  when d.tablicaveza = 'KntOstalo' then '(' + rtrim(ko.broj) + ')'  
                                  when d.tablicaveza = 'ProPonGla' then '(' + rtrim(po.broj) + ')'  
                                  else ''  
                               end),   
       o.Naziv as Operater,  
          coalesce(d.UplataVal, 0) as SumUplataVal, coalesce(d.IsplataVal, 0) as SumIsplataVal,  
          coalesce(d.UplataKn, 0) as SumUplataKn, coalesce(d.Isplata, 0) as SumIsplata,   
          v.Opis as NazivDokumenta  
          , d.opis as DetaljiOpis
          
   from BLPrometGla g left join BLPrometDet d on g.BLPrometGlaId = d.BLPrometGlaId  
        inner join ssOperateri o on g.OperateriId = o.OperateriId  
        inner join BLDokumenti vd on g.BLDokumentiId = vd.BLDokumentiId  
        inner join VrstaDok v on vd.VrstaDokId = v.VrstaDokId  
           left join kntplana kn on d.kntplanaid = kn.kntplanaid  
      left join fakture f on d.idveza = f.faktureid  
      left join faktureDob fd on d.idveza = fd.faktureDobid  
      left join kntostalo ko on d.idveza = ko.kntostaloid  
      left join propongla po on d.idveza = po.proponglaid 
	       
        --left join HRKadrovi k on d.HRKadroviId = k.HRKadroviId  
        --left join Partneri p on d.PartneriId = p.PartneriId  
   where g.BLZakljucciId = @ID  
   order by g.Datum, g.Vrijeme, g.BrojDok, g.Broj, g.Opis, o.Naziv, v.Opis  
--   group by  g.Datum, g.Vrijeme, g.BrojDok, g.Broj, g.Opis, o.Naziv, v.Opis  
  
  /*d.Opis as DetaljOpis, d.Tecaj,*/  
  
  end   



 if @Action = 0 begin   
  
   select z.*,   
   SaldoVal = z.prsaldoval + z.uplataval - z.isplataval,  
   SaldoKn = z.prsaldoKn + z.uplataKn - z.isplataKn  
   from BLZakljucci z  
   --where z.BlagajneId = @ID  
  
  return(0)  
  end  

  
  if @Action = 10 begin -- ispis   
  
   select z.BLZakljucciId, z.BlagajneId, z.Datum, z.BrojDok, z.Broj, z.PrSaldoVal, z.PrSaldoKn, z.UplataVal, z.IsplataVal, z.Tecaj, z.UplataKn, z.IsplataKn, z.Prefix, z.Sufix, z.Napomena,   
        min(g.Datum) as DatumOd, count(d.BLPrometDetId) as BrojPriloga,  
    DatumSaldoOd = (select top 1 z1.Datum from BLZakljucci z1 where z1.BlagajneId = z.BlagajneId and z1.BLZakljucciId<>z.BLZakljucciId and convert(int, z1.BrojDok) <  convert(int, z.BrojDok) order by z1.Broj desc),  
    SaldoNa = coalesce(z.PrSaldoKn, 0) + coalesce(z.UplataKn,0) - coalesce(z.IsplataKn, 0),  
    operateriid = (select operateriid from blzakljucci where blzakljucciid = @ID),  
    vrijeme = (select vrijeme from blzakljucci where blzakljucciid = @ID),  
    coalesce(blg.Naziv, '') as NazivBlagajne  
   from BLZakljucci z inner join BLPrometGla g on z.BLZakljucciId = g.BLZakljucciId  
    inner join BLPrometDet d on g.BLPrometGlaId = d.BLPrometGlaId  
    inner join BLDokumenti dk on g.BLDokumentiId = dk.BLDokumentiId  
    inner join Blagajne blg on dk.BlagajneId = blg.BlagajneId  
   where z.BLZakljucciId = @ID  
   group by z.BLZakljucciId, z.BlagajneId, z.Datum, z.BrojDok, z.Broj, z.PrSaldoVal, z.PrSaldoKn, z.UplataVal, z.IsplataVal, z.Tecaj, z.UplataKn, z.IsplataKn, z.Prefix, z.Sufix, z.Napomena, blg.Naziv  
    
  return(0)  
  end  
  
  if @Action = 20 begin -- zakljucci za sve Blagajne   
  
   select z.*,   
   SaldoVal = z.prsaldoval + z.uplataval - z.isplataval,  
   SaldoKn = z.prsaldoKn + z.uplataKn - z.isplataKn  
   from BLZakljucci z  
   where z.BlagajneId = @ID  
  
    
  return(0)  
  end    
  

if @Action = 21 begin -- zakljucci za sve Blagajne,vremenska podjela

			select z.*, 
			       SaldoVal = z.prsaldoval + z.uplataval - z.isplataval,
			       SaldoKn = z.prsaldoKn + z.uplataKn - z.isplataKn,
                   b.Naziv as VrstaBlagajne
			from BLZakljucci z inner join blagajne b on z.blagajneid=b.blagajneid 
		    where z.datum between @DatumOd and @DatumDo

return(0)
end	
 return(0)
