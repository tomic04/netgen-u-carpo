



create PROCEDURE [dbo].[spStatusiQuery]
@Action int = null,
@ID int = null,
@node int = null,
@sqlstring nvarchar(max) = '',
@str varchar(200)=''



AS

	if @Action = 0  --master grid sa stabla
		begin
			select statusiid,  redoslijed as [Redoslijed1500],Korak as [Status2000],
			  case when obavezan=0 then 'NE' else 'DA' end as [Obavezan1500],
			  case when smije=0 then 'NE' else 'DA' end as [Dokument se može mijenjati1800],
			  case when zakljuceno=0 then 'NE' else 'DA' end as [Dokument je zaključen1800]
			  ,dbo.[fnStatusiString] (statusiid) as [Slijedeći statusi4000]
			  ,VrstaDokId    
			  , case when aktivan = 0 then 'NE' else 'DA' end as [Aktivan1500]
			  , case when aktivan = 0 then 'gray' else 'black' end as [FormatID]
			  from statusi s
			  where vrstadokid=@node
			  order by Redoslijed,statusiid
			  
			return(0) 
		end


	if @Action = 1  --detail grid iz mastera
		begin

			select so.statusioperateriid , so.operateriid as opid,so.statusiid,o.naziv as [Operater3000],
			  case when vracanjestatusa=0 then 'NE' else 'DA' end as [Može vratiti status2500]
				  from statusioperateri so 
				  inner join ssoperateri o on so.operateriid=o.operateriid
				  inner join statusi s on so.statusiid=s.statusiid
				  where so.statusiid=@id ---and s.vrstadokid=@node
				  ORDER BY o.Naziv
			return(0) 
		end


	if @Action = 2  --za edit statusa
		begin
			select statusiid,  redoslijed ,Korak , obavezan, smije,zakljuceno
			  ,dbo.[fnStatusiString] (statusiid) as Next
			  ,VrstaDokId 
			  , aktivan   
			  from statusi s
			  where statusiid=@id
			  order by Redoslijed,statusiid
			  
			return(0) 
		end

	if @Action = 3 begin --za grid 
	
		select s.statusiid
				, s.Korak as [Korak4100]
				, case when sn.StatusiId=@id then 1 else 0 end as selectedid
		from statusi s
		left outer join StatusiNext sn on s.StatusiId = sn.NextId and sn.StatusiId=@id
		where s.StatusiId != @id
		and s.aktivan != 0
		order by s.Redoslijed
	
	end

	if @Action = 4 begin --za grid 
		
		set @str = SUBSTRING(@str,2,len(@str)-2) 
			
		set @sqlstring=N'declare @out varchar(1000)
						 select @out=coalesce(@out,'''')+korak +''; ''from statusi where statusiid in ('+@str+')
						 select @out as next	
						'
		--print @sqlstring
		exec sp_executesql @sqlstring
	
	end

	return(0)



	

