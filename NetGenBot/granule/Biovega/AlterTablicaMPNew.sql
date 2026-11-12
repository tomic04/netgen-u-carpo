
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[trMaloprodajaDet]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop trigger [dbo].[trMaloprodajaDet]
GO


if not exists (select * from syscolumns where name='nepovratrobaid' and id=object_id('jmroba')) 
alter table jmroba add nepovratrobaid int null 
GO

if not exists (select * from syscolumns where name='defaultind' and id=object_id('barcode')) 
alter table barcode add defaultind int null 
GO

if not exists (select * from syscolumns where name='Storno' and id=object_id('mpblagajnagla')) 
alter table mpblagajnagla add Storno char(1) null
GO

if not exists (select * from syscolumns where name='StornoId' and id=object_id('mpblagajnagla')) 
alter table mpblagajnagla add StornoId int null
GO

if not exists (select * from syscolumns where name='KolicinaNaruceno' and id=object_id('maloprodajadet')) 
ALter table maloprodajadet add KolicinaNaruceno money default (0)
GO

if not exists (select * from syscolumns where name='brojrata' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add brojrata int 
GO


if not exists (select * from syscolumns where name='datumvalute' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add datumvalute datetime null
GO


if not exists (select * from syscolumns where name='ufaiznos' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add ufaiznos money null
GO


if not exists (select * from syscolumns where name='fakturedobid' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add fakturedobid int null
GO

if not exists (select * from syscolumns where name='kolicinakasa' and id=object_id('mpstanje')) 
alter table mpstanje add kolicinakasa money default(0)
GO

if not exists (select * from syscolumns where name='MPGlaOldId' and id=object_id('mpstanje')) 
alter table mpstanje add MPGlaOldId int null
GO

if not exists (select * from syscolumns where name='brracuna' and id=object_id('mputrzaknaplata')) 
alter table mputrzaknaplata add brracuna int null
GO


if not exists (select * from syscolumns where name='vrijeme' and id=object_id('mputrzaknaplata')) 
alter table mputrzaknaplata add vrijeme datetime null
GO

if not exists (select * from syscolumns where name='odracuna' and id=object_id('mputrzaknaplata')) 
alter table mputrzaknaplata add odracuna varchar(25) null
GO


if not exists (select * from syscolumns where name='doracuna' and id=object_id('mputrzaknaplata')) 
alter table mputrzaknaplata add doracuna varchar(25) null
GO


if not exists (select * from syscolumns where name='Nabcijena' and id=object_id('ptprijenos')) 
alter table ptprijenos add Nabcijena money default(0)
GO


if not exists (select * from syscolumns where name='rabat' and id=object_id('ptprijenos')) 
alter table ptprijenos add rabat money default(0)
GO


if not exists (select * from syscolumns where name='skladisteid' and id=object_id('ptprijenos')) 
alter table ptprijenos add skladisteid int null 
GO

if not exists (select * from syscolumns where name='dokument' and id=object_id('ptprijenos')) 
alter table ptprijenos add dokument varchar(50) null
GO


if not exists (select * from syscolumns where name='datum' and id=object_id('ptprijenos')) 
alter table ptprijenos add datum datetime null
GO

if not exists (select * from syscolumns where name='parasortimantipsifra' and id=object_id('trgovina')) 
alter table trgovina add parasortimantipsifra char(2) null
GO


if not exists (select * from syscolumns where name='povratnanaknada' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add povratnanaknada money default(0)
GO

if not exists (select * from syscolumns where name='mpvrijednostknjiga' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add mpvrijednostknjiga money default(0)
GO

if not exists (select * from syscolumns where name='povratnanaknada' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add povratnanaknada money default(0)
GO 

if not exists (select * from syscolumns where name='mpvrijednostknjiga' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add mpvrijednostknjiga money default(0)
GO

if not exists (select * from syscolumns where name='MPIznosKnjiga' and id=object_id('mpblagajnagla')) 
	alter table mpblagajnagla add MPIznosKnjiga money default (0)
GO

if not exists (select * from syscolumns where name='POvIznos' and id=object_id('mpblagajnagla')) 
	alter table mpblagajnagla add POvIznos money default (0)
GO


if exists (select * from syscolumns where name='maloprodajaglaid' and id=object_id('mpstanje')) 
	alter table mpstanje alter column maloprodajaglaid int null 
GO
if exists (select * from syscolumns where name='MPglaoldId' and id=object_id('mpstanje')) 
	alter table mpstanje alter column MPglaoldId int null 
GO
if exists (select * from syscolumns where name='Datumpromjene' and id=object_id('mpstanje')) 
	alter table mpstanje alter column Datumpromjene datetime null 
GO

if not exists (select * from syscolumns where name='maloprodajaglaid' and id=object_id('TempNivelacija')) 
	alter table TempNivelacija add maloprodajaglaid int null
GO

if not exists (select * from syscolumns where name='Ind' and id=object_id('TempNivelacija')) 
	alter table TempNivelacija add ind  int  null
GO

if not exists (select * from syscolumns where name='Vrstadokid' and id=object_id('kntnacinplac')) --ksenija
	alter table kntnacinplac add Vrstadokid  int  null
GO

if not exists (select * from syscolumns where name='brojrata' and id=object_id('mpblagajnagla')) 
	alter table mpblagajnagla add brojrata  int  null
GO

if not exists (select * from syscolumns where name='partija' and id=object_id('mpblagajnagla')) 
	alter table mpblagajnagla add partija int  null
GO

if not exists (select * from syscolumns where name='brojrata' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add brojrata  int  null
GO

if not exists (select * from syscolumns where name='partija' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add partija int  null
GO

if not exists (select * from syscolumns where name='projekt' and id=object_id('pubvars')) 
	alter table pubvars add projekt varchar(2) null
GO

if not exists (select * from syscolumns where name='projekt' and id=object_id('vrstaprom')) 
	alter table vrstaprom add projekt varchar(2) null
GO


if not exists (select * from syscolumns where name='maloprodajadetid' and id=object_id('mpstanje')) 
	alter table mpstanje add maloprodajadetid int null
GO

if not exists (select * from syscolumns where name='konto' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac add konto varchar(10) null
GO

if not exists (select * from syscolumns where name='oznaka' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac add oznaka char(3) null
GO

if not exists (select * from syscolumns where name='VrstaPlacanja' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac add VrstaPlacanja varchar(20) null
GO

if not exists (select * from syscolumns where name='VrstaDokId' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac add VrstaDokId int null
GO

if not exists (select * from syscolumns where name='IznosUsluge' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add IznosUsluge money default (0)
GO

if not exists (select * from syscolumns where name='PDVUsluge' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add PDVUsluge money default (0)
GO

if not exists (select * from syscolumns where name='napomena2' and id=object_id('mpponudagla')) 
	alter table mpponudagla add napomena2 varchar(300)null
GO

if exists (select * from syscolumns where name='napomena' and id=object_id('mpponudagla')) 
	alter table mpponudagla alter column napomena varchar(300)null
GO


if not exists (select * from syscolumns where name='napomena' and id=object_id('mpponudadet')) 
	alter table mpponudadet  add napomena varchar(300)null
GO

if not exists (select * from syscolumns where name='VRKredit' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac  add VRKredit varchar(1)null
GO

if not exists (select * from syscolumns where name='knjizenje' and id=object_id('vrstadok')) 
	alter table vrstadok  add knjizenje char (1)null
GO

if not exists (select * from syscolumns where name='MPCijena' and id=object_id('Cjenik')) 
	alter table Cjenik  add MPCijena money default(0)
GO

if not exists (select * from syscolumns where name='IndMP' and id=object_id('VRCjenik')) 
	alter table VrCjenik  add IndMP int null
GO

if not exists (select * from syscolumns where name='nabcijena' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add nabcijena money not null default (0)
GO

if not exists (select * from syscolumns where name='nabcijena' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add nabcijena money not null default (0)
GO

if not exists (select * from syscolumns where name='RobnaMarkaID' and id=object_id('Roba')) 
	alter table Roba add RobnaMarkaID int null 
GO

if not exists (select * from syscolumns where name='mpcijena' and id=object_id('Cjeniklog')) 
	alter table Cjeniklog add mpcijena money not null default (0)
GO

if not exists (select * from syscolumns where name='StaraNC' and id=object_id('mpnabcijene')) 
	alter table mpnabcijene add StaraNC money not null default (0)
GO


if not exists (select * from syscolumns where name='Partneriid' and id=object_id('TempPromNC')) 
	alter table TempPromNC add Partneriid int  null 
GO

if not exists (select * from syscolumns where name='Parstruid' and id=object_id('trgovina')) 
	alter table trgovina add Parstruid int  null 
GO

if not exists (select * from syscolumns where name='DatumOtvaranja' and id=object_id('Maloprodajagla')) 
	alter table Maloprodajagla add DatumOtvaranja datetime null
GO

if not exists (select * from syscolumns where name='InsCjenik ' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add InsCjenik int null 
GO

if not exists (select * from syscolumns where name='MPClanoviId ' and id=object_id('MPPonudagla')) 
	alter table MPPonudagla add MPClanoviId int null 
GO
if not exists (select * from syscolumns where name='MPClanoviId ' and id=object_id('MPBlagajnagla')) 
	alter table MPBlagajnagla add MPClanoviId int null 
GO

if not exists (select * from syscolumns where name='SifraClana ' and id=object_id('MPBlagajnagla')) 
	alter table MPBlagajnagla add SifraClana varchar(13) null 
GO

if not exists (select * from syscolumns where name='BrojBodova ' and id=object_id('MPBlagajnagla')) 
	alter table MPBlagajnagla add BrojBodova money null 
GO

if not exists (select * from syscolumns where name='Ind_add ' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add Ind_add int default(0)
GO

if not exists (select * from syscolumns where name='DanaValute' and id=object_id('Vrstadok')) 
	alter table Vrstadok add DanaValute int default(0)
GO

if not exists (select * from syscolumns where name='DodatniDaniValute' and id=object_id('Vrstadok')) 
	alter table Vrstadok add DodatniDaniValute int default(0)
GO

if not exists (select * from syscolumns where name='UkupnoBodova ' and id=object_id('MPClanovi')) 
	alter table MPClanovi add UkupnoBodova money null 
GO

if not exists (select * from syscolumns where name='trgovinaid ' and id=object_id('UplateKredita')) 
	alter table UplateKredita add trgovinaid int null
GO

if not exists (select * from syscolumns where name='operateriid ' and id=object_id('UplateKredita')) 
	alter table UplateKredita add operateriid int null
GO

if not exists (select * from syscolumns where name='OznKredit ' and id=object_id('KntNacinPlac')) 
	alter table KntNacinPlac add OznKredit char(1) null
GO

if not exists (select * from syscolumns where name='NabIznosProsjecni ' and id=object_id('Maloprodajagla')) 
	alter table Maloprodajagla add NabIznosProsjecni money not null default (0)
GO

if not exists (select * from syscolumns where name='NabCijenaProsjecna ' and id=object_id('Maloprodajadet')) 
	alter table Maloprodajadet add NabCijenaProsjecna money not null default (0)
GO

if not exists (select * from syscolumns where name='KntTemId ' and id=object_id('Maloprodajagla')) 
	alter table Maloprodajagla add KntTemId int null
GO

if not exists (select * from syscolumns where name='Opis1' and id=object_id('Fakture')) 
	alter table Fakture add Opis1 varchar(50) null
GO

if not exists (select * from syscolumns where name='PronOtpDetId' and id=object_id('FakDetalj')) 
	alter table FakDetalj add PronOtpDetId int null
GO

if not exists (select * from syscolumns where name='Opis' and id=object_id('Orgshema')) 
	alter table Orgshema add Opis varchar(50) null
GO


if not exists (select * from syscolumns where name='NabVrijednostProsjecna ' and id=object_id('Maloprodajadet')) 
        alter table Maloprodajadet add [NabVrijednostProsjecna] AS (round(([nabcijenaprosjecna] * ([kolicinaulaz] + [kolicinaizlaz])),2)) 
GO

if not exists (select * from syscolumns where name='DatumOtvaranja' and id=object_id('MPNargla')) 
        alter table MPNargla add [DatumOtvaranja] datetime null
GO

if not exists (select * from syscolumns where name='GodinaRodjenja' and id=object_id('MPClanovi')) 
        alter table MPClanovi add [GodinaRodjenja] char(2)
GO
if not exists (select * from syscolumns where name='MjesecRodjenja' and id=object_id('MPClanovi')) 
        alter table MPClanovi add [MjesecRodjenja] char(2)
GO
if not exists (select * from syscolumns where name='DanRodjenja' and id=object_id('MPClanovi')) 
        alter table MPClanovi add [DanRodjenja] char(2)
GO

if not exists (select * from syscolumns where name='PocetniBodovi' and id=object_id('MPClanovi')) 
        alter table MPClanovi add [PocetniBodovi] int not null default(0)
GO

if not exists (select * from syscolumns where name='Trgovinaid' and id=object_id('MPClanovi')) 
        alter table MPClanovi add Trgovinaid int null 
GO

if not exists (select * from syscolumns where name='Operateriid' and id=object_id('MPclanoviPromet')) 
        alter table MPClanovipromet add Operateriid int null 
GO
if not exists (select * from syscolumns where name='Vrijeme' and id=object_id('MPclanoviPromet')) 
        alter table MPClanovipromet  add Vrijeme datetime null
GO

if not exists (select * from syscolumns where name='Operateriid' and id=object_id('MPClanovi')) 
        alter table  MPClanovi add Operateriid int null 
GO
if not exists (select * from syscolumns where name='Vrijeme' and id=object_id('MPClanovi')) 
        alter table MPClanovi  add Vrijeme datetime null
GO



if not exists (select * from syscolumns where name='DatumOtvaranja' and id=object_id('MPClanovi')) 
        alter table MPClanovi add DatumOtvaranja datetime null
GO

if not exists (select * from syscolumns where name='Trgovinaid' and id=object_id('MPClanoviPromet')) 
        alter table MPClanoviPromet add Trgovinaid int  null
GO
if not exists (select * from syscolumns where name='Operateriid' and id=object_id('MPClanoviPromet')) 
        alter table MPClanoviPromet add Operateriid int  null
GO
if not exists (select * from syscolumns where name='Vrijeme' and id=object_id('MPClanoviPromet')) 
        alter table MPClanoviPromet add Vrijeme  datetime null 
GO

if not exists (select * from syscolumns where name='Datum' and id=object_id('izracunbodova')) 
        alter table izracunbodova add datum datetime null
GO

if not exists (select * from syscolumns where name='Vrijeme' and id=object_id('MPBlagajnagla')) 
        alter table MPBlagajnagla add Vrijeme datetime null
GO


if not exists (select * from syscolumns where name='IndMinus' and id=object_id('Trgovina')) 
        alter table Trgovina add IndMinus char(1) not null default(1)
GO

if not exists (select * from syscolumns where name='Lokacijeid' and id=object_id('Trgovina')) 
        alter table Trgovina add Lokacijeid int null 
GO







if not exists (select * from syscolumns where name='Napomena' and id=object_id('MPClanovi')) 
        alter table MPClanovi add Napomena varchar(400) null
GO

if not exists (select * from syscolumns where name='BrojBodova' and id=object_id('MPClanovi')) 
        alter table MPClanovi add BrojBodova int not null default (0)
GO

if not exists (select * from syscolumns where name='BrojBodovaPrije' and id=object_id('mpblagajnagla')) 
        alter table mpblagajnagla add BrojBodovaPrije money not null default (0)
GO

if not exists (select * from syscolumns where name='KP' and id=object_id('vrstadok')) 
        alter table vrstadok add KP int	null
GO


if not exists (select * from syscolumns where name='Robaid' and id=object_id('VrstaDok')) 
        alter table VrstaDok add Robaid int null 
GO

if not exists (select * from syscolumns where name='TrosakPostotak' and id=object_id('VrstaDok')) 
        alter table VrstaDok add TrosakPostotak money not null default (0)
GO

if not exists (select * from syscolumns where name='Rabatnom' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add Rabatnom money not null default (0)
GO

if not exists (select * from syscolumns where name='MPKaseid' and id=object_id('Maloprodajagla')) 
        alter table Maloprodajagla add MPKaseid int null 
GO


if exists (select * from syscolumns where name='ind' and id=object_id('mpcijenapromet')) 
	alter table mpcijenapromet alter column ind int null
GO

if not exists (select * from syscolumns where name='MPPredznak' and id=object_id('Vrstadok')) 
        alter table Vrstadok add MPPredznak int not null default (1)
GO


if not exists (select * from syscolumns where name='nabcijena' and id=object_id('mpinventuradet')) 
	alter table mpinventuradet add nabcijena money not null default (0)
GO

if not exists (select * from syscolumns where name='postotak' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add postotak money not null default (0)
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add operateriid int null 
GO

if not exists (select * from syscolumns where name='postotak' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add postotak money not null default (0)
GO

if not exists (select * from syscolumns where name='napomena' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add napomena varchar(100) null 
GO

if not exists (select * from syscolumns where name='zakljucakid' and id=object_id('prijenoszakljucka')) 
	alter table prijenoszakljucka add zakljucakid int null 
GO

if not exists (select * from syscolumns where name='Brojracuna' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Brojracuna varchar(15) null 
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add operateriid  int null 
GO

if not exists (select * from syscolumns where name='Vrstadokid' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Vrstadokid  int null 
GO

if not exists (select * from syscolumns where name='komisija' and id=object_id('roba')) 
	alter table roba add komisija int not null default (0)
GO

if not exists (select * from syscolumns where name='provizija' and id=object_id('kntnacinplac')) 
	alter table kntnacinplac add provizija money not null default (0)
GO

if not exists (select * from syscolumns where name='Sufix' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Sufix varchar(15) not null default (' ')
GO



if not exists (select * from syscolumns where name='Prefix' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Prefix varchar(15) not null default (' ')
GO

if not exists (select * from syscolumns where name='Broj' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Broj varchar(15) not null default (' ')
GO

if not exists (select * from syscolumns where name='vrstadokid' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add vrstadokid int null 
GO

if not exists (select * from syscolumns where name='Nacinisporuke' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add Nacinisporuke int null 
GO

if not exists (select * from syscolumns where name='NPIndZak' and id=object_id('vrstadok')) 
	alter table vrstadok add NPIndZak int null 
GO

if not exists (select * from syscolumns where name='Opis' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add OPis  varchar(50) null 
GO

if not exists (select * from syscolumns where name='Datumdok' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add Datumdok  datetime null 
GO
if not exists (select * from syscolumns where name='Trgovinaid' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add Trgovinaid  int null 
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add operateriid  int null 
GO

if not exists (select * from syscolumns where name='mpponudaglaid' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add mpponudaglaid  int null 
GO

if not exists (select * from syscolumns where name='vrijeme' and id=object_id('MPAmbalaza')) 
	alter table MPAmbalaza add vrijeme  datetime null 
GO

if not exists (select * from syscolumns where name='DatumKnjizenja' and id=object_id('MPNivelGla')) 
	alter table MPNivelGla add DatumKnjizenja  datetime null 
GO

if not exists (select * from syscolumns where name='KntTemId' and id=object_id('MPNivelGla')) 
	alter table MPNivelGla add KntTemId  int null 
GO

if not exists (select * from syscolumns where name='maloprodajadetid' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add maloprodajadetid  int null 
GO

if not exists (select * from syscolumns where name='napomena' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add napomena varchar(200) null 
GO
if not exists (select * from syscolumns where name='datumotpreme' and id=object_id('MPOtpremaGla')) 
	alter table MPOtpremaGla add datumotpreme datetime null 
GO

if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('PTPrijenos')) 
	alter table PTPrijenos add trgovinaid int null 
GO

if not exists (select * from syscolumns where name='vozacid' and id=object_id('Maloprodajagla')) 
	alter table Maloprodajagla add vozacid int null 
GO

if not exists (select * from syscolumns where name='predznak' and id=object_id('TempNivelacija')) 
	alter table TempNivelacija add predznak int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('rajoni')) 
	alter table rajoni add partneriid int null 
GO

if not exists (select * from syscolumns where name='osobeid' and id=object_id('rajoni')) 
	alter table rajoni add osobeid int null 
GO

if not exists (select * from syscolumns where name='MPGrupeBodovaId' and id=object_id('IzracunBodova')) 
	alter table IzracunBodova add MPGrupeBodovaId int null 
GO
if not exists (select * from syscolumns where name='PocetniBodovi' and id=object_id('MPClanovi'))
	Alter table MPClanovi add PocetniBodovi money not null default(0)
GO

if not exists (select * from syscolumns where name='Datum' and id=object_id('MPClanovi'))
	Alter table MPClanovi add Datum datetime null
GO

if not exists (select * from syscolumns where name='PocetniBodovi' and id=object_id('PrijenosClanovi'))
	Alter table PrijenosClanovi add PocetniBodovi money  not null default(0)
GO

if not exists (select * from syscolumns where name='Datum' and id=object_id('PrijenosClanovi'))
	Alter table PrijenosClanovi add Datum datetime null
GO

if not exists (select * from syscolumns where name='BodoviRealizacija' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add BodoviRealizacija money not null default (0)
GO

if not exists (select * from syscolumns where name='BodoviRealizacija' and id=object_id('mpblagajnagla')) 
        alter table mpblagajnagla add BodoviRealizacija money not null default (0)
GO

if not exists (select * from syscolumns where name='MPIndZak' and id=object_id('VrstaDok')) 
        alter table VrstaDok add MPIndZak int null
GO

if not exists (select * from syscolumns where name='KratkiNaziv' and id=object_id('Roba')) 
        alter table Roba add KratkiNaziv varchar(25) not null default ('')
GO

if not exists (select * from syscolumns where name='Povratnanaknada' and id=object_id('MPNivelDet')) 
        alter table MPNivelDet add Povratnanaknada money not null default (0)
GO

if not exists (select * from syscolumns where name='Aktivno' and id=object_id('Trgovina')) 
        alter table Trgovina add Aktivno int not null default (1)
GO

if not exists (select * from syscolumns where name='PovNaknadaRoba' and id=object_id('maloprodajadet')) 
        alter table maloprodajadet add PovNaknadaRoba money not null default (0)
GO

if not exists (select * from syscolumns where name='OrgDok' and id=object_id('Pronotpgla')) 
        alter table Pronotpgla add OrgDok varchar(30) null
GO

if not exists (select * from syscolumns where name='NabIznosPros' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add NabIznosPros money not null default (0)
GO

if not exists (select * from syscolumns where name='Status' and id=object_id('MPClanovi')) 
        alter table MPClanovi add Status int not null default (1)
GO

if not exists (select * from syscolumns where name='IznosPov' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add IznosPov money not null default (0)
GO

if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('TempPromNC')) 
        alter table TempPromNC add trgovinaid int null 
GO

if not exists (select * from syscolumns where name='Status' and id=object_id('PrijenosClanovi')) 
        alter table PrijenosClanovi add Status int not null default (1)
GO

if not exists (select * from syscolumns where name='Poreziid' and id=object_id('MPInventuradet')) 
        alter table MPInventuradet add Poreziid int null 
GO


if not exists (select * from syscolumns where name='Porezpp' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add Porezpp money not null default(0)
GO

if not exists (select * from syscolumns where name='Porezippid' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add Porezippid int null 
GO

if not exists (select * from syscolumns where name='poreziid' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add poreziid int null 
GO
 
if not exists (select * from syscolumns where name='Iznospp' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add Iznospp money not null default(0)
GO


if not exists (select * from syscolumns where name='Porezpp' and id=object_id('Maloprodajadet')) 
        alter table Maloprodajadet add Porezpp money not null default(0)
GO

if not exists (select * from syscolumns where name='Porezippid' and id=object_id('Maloprodajadet')) 
        alter table Maloprodajadet add Porezippid int null 
GO

if not exists (select * from syscolumns where name='Vrijednostpp' and id=object_id('Maloprodajadet')) 
        alter table Maloprodajadet add Vrijednostpp money not null default(0)
GO

if not exists (select * from syscolumns where name='Iznospp' and id=object_id('Maloprodajagla')) 
        alter table Maloprodajagla add Iznospp money not null default(0)
GO

if not exists (select * from syscolumns where name='povratnanaknada' and id=object_id('mpponudadet')) 
	alter table mpponudadet add povratnanaknada money default(0)
GO

if not exists (select * from syscolumns where name='PovNaknadaRoba' and id=object_id('mpponudadet')) 
        alter table mpponudadet add PovNaknadaRoba money not null default (0)
GO


if not exists (select * from syscolumns where name='IznosPov' and id=object_id('MPPonudagla')) 
        alter table MPPonudagla add IznosPov money not null default (0)
GO

if not exists (select * from syscolumns where name='Mpblagajnaglaid' and id=object_id('MPPonudagla')) 
        alter table MPPonudagla add Mpblagajnaglaid int null 
GO

--if not exists (select * from syscolumns where name='MPBlagajnaglaId' and id=object_id('MPDonosWeb')) 
--        alter table MPDonosWeb add MPBlagajnaglaId int null 
--GO

if not exists (select * from syscolumns where name='Storno' and id=object_id('Prijenosnaplate')) 
        alter table Prijenosnaplate add Storno char(1) null
GO

if not exists (select * from syscolumns where name='Umirovljen' and id=object_id('MPStanje')) 
        alter table MPStanje add Umirovljen int null
GO

if not exists (select * from syscolumns where name='MPVrDokid' and id=object_id('KNTPromet')) 
        alter table KNTPromet add MPVrDokid int null
GO

if not exists (select * from syscolumns where name='Vezanicjenikid' and id=object_id('TempPromMPC')) 
        alter table TempPromMPC add Vezanicjenikid int null
GO

if not exists (select * from syscolumns where name='MPVrDokid' and id=object_id('tmpMatKnjizenjeMP')) 
        alter table tmpMatKnjizenjeMP add MPVrDokid int null
GO

if not exists (select * from syscolumns where name='Ind_UplKred' and id=object_id('vrstadok')) 
        alter table vrstadok add Ind_UplKred int null
GO

if not exists (select * from syscolumns where name='OrgshemaId' and id=object_id('FakDetalj')) 
        alter table FakDetalj add OrgshemaId int null
GO

if not exists (select * from syscolumns where name='OsobeId' and id=object_id('Trgovina')) 
        alter table Trgovina add OsobeId int null
GO

if not exists (select * from syscolumns where name='MPCbezPOV' and id=object_id('mpinventuradet')) begin 
        alter table mpinventuradet add MPCbezPOV money default(0)
end 
GO 
if not exists (select * from syscolumns where name='povnaknadaroba' and id=object_id('mpinventuradet')) 
        alter table mpinventuradet add povnaknadaroba money default(0)
GO

if not exists (select * from syscolumns where name='sifra' and id=object_id('LNTrgovinaOperateri')) 
        alter table LNTrgovinaOperateri add sifra varchar(20) not null 
GO

if not exists (select * from syscolumns where name='ssoperateriid' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add ssoperateriid int null 
GO

if not exists (select * from syscolumns where name='ssoperateriid' and id=object_id('mpblagajnadet')) 
        alter table mpblagajnadet add ssoperateriid int null 
GO

if not exists (select * from syscolumns where name='minzaliha' and id=object_id('mpstanje')) 
        alter table mpstanje add minzaliha money not null default(0)
GO

if not exists (select * from syscolumns where name='maxzaliha' and id=object_id('mpstanje')) 
        alter table mpstanje add maxzaliha money not null default(0)
GO

if not exists (select * from syscolumns where name='MPkaseid' and id=object_id('mpponudagla')) 
        alter table mpponudagla add MPkaseid int null 
GO

if not exists (select * from syscolumns where name='MPIznosKnjiga' and id=object_id('mpponudagla')) 
        alter table mpponudagla add MPIznosKnjiga money not null default (0)
GO

if not exists (select * from syscolumns where name='MPVrijednostKnjiga' and id=object_id('mpponudadet')) 
        alter table mpponudadet add MPVrijednostKnjiga money not null default (0)
GO

if not exists (select * from syscolumns where name='vrijediod' and id=object_id('vrcjenik')) 
        alter table vrcjenik add vrijediod datetime null 
GO



/*

if not exists (select * from syscolumns where name='MPVrijednostorg1' and id=object_id('mpponudadet'))  begin 
	
        alter table mpponudadet add MPVrijednostorg1 money not null default (0)
	update  mpponudadet set MPVrijednostorg1 = MPVrijednostorg 
        alter table mpponudadet drop column MPVrijednostOrg 
        alter table mpponudadet add MPVrijednostorg money not null default (0)
	update  mpponudadet set MPVrijednostorg = MPVrijednostorg1 
--        alter table mpponudadet drop column MPVrijednostOrg1 
end 
GO
if not exists (select * from syscolumns where name='MPVrijednostorg' and id=object_id('mpponudadet')) begin 
	
        alter table mpponudadet add MPVrijednostorg1 money not null default (0)
	update  mpponudadet set MPVrijednostorg1 = MPVrijednostorg 
        alter table mpponudadet drop column MPVrijednostOrg 
        alter table mpponudadet add MPVrijednostorg money not null default (0)
	update  mpponudadet set MPVrijednostorg = MPVrijednostorg1 
--        alter table mpponudadet drop column MPVrijednostOrg1 
end 
GO

*/
if not exists (select * from syscolumns where name='ucesce' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add ucesce money not null default (0)
GO

if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('zaglavljeracuna')) 
        alter table zaglavljeracuna add trgovinaid int null
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('ptprijenos')) 
        alter table ptprijenos add operateriid int null 
GO

if not exists (select * from syscolumns where name='povnaknadaroba' and id=object_id('mpniveldet')) 
        alter table mpniveldet add povnaknadaroba money not null default (0)

GO

if not exists (select * from syscolumns where name='maloprodajadetid' and id=object_id('TempNivelacija')) 
	alter table TempNivelacija add maloprodajadetid int null
GO

if not exists (select * from syscolumns where name='povnaknadaroba' and id=object_id('TempNivelacija')) 
	alter table TempNivelacija add povnaknadaroba money not null default(0)
GO

if not exists (select * from syscolumns where name='vrijeme' and id=object_id('lntrgovinaCjenik')) 
	alter table lntrgovinaCjenik add vrijeme datetime null 
GO

if not exists (select * from syscolumns where name='vrijeme' and id=object_id('mpvezanicjenik')) 
	alter table mpvezanicjenik add vrijeme datetime null 
GO

if not exists (select * from syscolumns where name='datumaktiviranja' and id=object_id('vrcjenik')) 
	alter table vrcjenik add datumaktiviranja datetime null 
GO

if not exists (select * from syscolumns where name='trosak' and id=object_id('maloprodajadet')) 
alter  table maloprodajadet add trosak money not null default (0)
GO 


if not exists (select * from syscolumns where name='ptprijenosID' and id=object_id('ptprijenos')) 
alter table ptprijenos add ptprijenosID int IDENTITY(1,1) NOT NULL primary key
GO

if not exists (select * from syscolumns where name='putanja' and id=object_id('trgovina')) 
alter table trgovina add putanja varchar(50) NULL 
GO

if not exists (select * from syscolumns where name='vrstadokid' and id=object_id('ptprijenos')) 
alter table ptprijenos add vrstadokid int NULL 
GO

if not exists (select * from syscolumns where name='rbr' and id=object_id('ptprijenos')) 
alter table ptprijenos add rbr int NULL 
GO

if not exists (select * from syscolumns where name='povnaknadaroba ' and id=object_id('ptprijenos')) 
alter table ptprijenos add povnaknadaroba int not NULL default(0)
GO


if not exists (select * from syscolumns where name='putanja' and id=object_id('trgovina')) 
alter table trgovina add putanja varchar(50) NULL 
GO

if not exists (select * from syscolumns where name='vrstadokid' and id=object_id('ptprijenos')) 
alter table ptprijenos add vrstadokid int NULL 
GO

if not exists (select * from syscolumns where name='rbr' and id=object_id('ptprijenos')) 
alter table ptprijenos add rbr int NULL 
GO


if not exists (select * from syscolumns where name='povnaknadaroba ' and id=object_id('ptprijenos')) 
alter table ptprijenos add povnaknadaroba money  not NULL default(0)
GO

if not exists (select * from syscolumns where name='rbr' and id=object_id('mpinventuradet')) 
alter table mpinventuradet add rbr int not NULL default(0)
GO

if not exists (select * from syscolumns where name='pomocnakasa' and id=object_id('mpkase')) 
alter table mpkase add pomocnakasa int null 
GO

if not exists (select * from syscolumns where name='Oznaka' and id=object_id('Porezi')) 
alter table Porezi add Oznaka Char(1)
GO

if not exists (select * from syscolumns where name='KlasaKasa' and id=object_id('KLMasterRoba')) 
alter table KLMasterRoba add KlasaKasa VarChar(10) null 
GO

if not exists (select * from syscolumns where name='Storno' and id=object_id('MAloprodajagla')) 
alter table MAloprodajagla add Storno VarChar(1) null 
GO

if not exists (select * from syscolumns where name='StornoDokid' and id=object_id('MAloprodajagla')) 
alter table MAloprodajagla add StornoDokid int  null 
GO

if not exists (select * from syscolumns where name='datcijene' and id=object_id('MAloprodajadet')) 
alter table MAloprodajadet add datcijene datetime null 
GO

if not exists (select * from syscolumns where name='sort' and id=object_id('MAloprodajadet')) 
alter table MAloprodajadet add sort int not null default (0)
GO

if not exists (select * from syscolumns where name='MPPonudaId' and id=object_id('MAloprodajagla')) 
alter table MAloprodajagla add MPPonudaId int  null 
GO

if not exists (select * from syscolumns where name='Rokisporuke' and id=object_id('MPPonudagla')) 
alter table MPPonudagla add Rokisporuke datetime null 
GO

if not exists (select * from syscolumns where name='CMPCijena' and id=object_id('Maloprodajadet')) 
alter table Maloprodajadet add CMPCijena  AS (mpcijena+povnaknadaroba)
GO

if not exists (select * from syscolumns where name='CMPCijenaorg' and id=object_id('Maloprodajadet')) 
alter table Maloprodajadet add CMPCijenaorg  AS (mpcijenaorg+povnaknadaroba)
GO


if not exists (select * from syscolumns where name='CMPIZNOS' and id=object_id('MaloprodajaGla')) 
alter table MaloprodajaGla add CMPIZNOS  AS (mpiznos+iznospov)
GO

if not exists (select * from syscolumns where name='tekst' and id=object_id('mpetikete')) 
alter table mpetikete add tekst  varchar(500)
GO


if not exists (select * from syscolumns where name='CMPIZNOSORG' and id=object_id('MaloprodajaGla')) 
alter table MaloprodajaGla add CMPIZNOSORG  AS (mpiznosORG+iznospov)
GO

if not exists (select * from syscolumns where name='Vezanicjenikid' and id=object_id('TempPromNC')) 
alter table TempPromNC add Vezanicjenikid  int null 
GO

if not exists (select * from syscolumns where name='InsCjenik' and id=object_id('TempPromNC')) 
alter table TempPromNC add InsCjenik int null 
GO

if not exists (select * from syscolumns where name='InsCjenik' and id=object_id('TempPromNC')) 
alter table TempPromNC add InsCjenik int null 
GO

if not exists (select * from syscolumns where name='postotak' and id=object_id('TempPromNC')) 
alter table TempPromNC add postotak money default(0)
GO

if not exists (select * from syscolumns where name='nabcijena' and id=object_id('TempPromNC')) 
alter table TempPromNC add nabcijena money default(0)
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('Maloprodajadet')) 
alter table Maloprodajadet  add operateriid int null 
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('MPPonudaDet')) 
alter table MPPonudaDet  add operateriid int null 
GO

if not exists (select * from syscolumns where name='nenc' and id=object_id('Vrstadok')) 
alter table Vrstadok  add nenc int default(0)
GO

if not exists (select * from syscolumns where name='ind_cijene' and id=object_id('JmRoba')) 
alter table JmRoba  add ind_cijene int null
GO

if not exists (select * from syscolumns where name='datumaktiviranja' and id=object_id('MPVezanicjenikUvjeti')) 
alter table MPVezanicjenikUvjeti add datumaktiviranja datetime null 
GO


if not exists (select * from syscolumns where name='odjelid' and id=object_id('LNTrgovinaOperateri')) 
alter table LNTrgovinaOperateri add odjelid int null
GO

if not exists (select * from syscolumns where name='trosak' and id=object_id('mpponudadet')) 
alter  table mpponudadet add trosak money not null default (0)
GO 


if not exists (select * from syscolumns where name='status' and id=object_id('lntrgovinacjenik')) 
alter  table lntrgovinacjenik add status int null 
GO 

if not exists (select * from syscolumns where name='posto' and id=object_id('lntrgovinacjenik')) 
alter  table lntrgovinacjenik add posto money not null default (0)
GO 


if (select count(*)from lntrgovinacjenik where Status is null) >0 
	update lntrgovinacjenik set status = 2 
GO


if not exists (select * from syscolumns where name='parametritrazilice' and id=object_id('mpkase')) 
alter  table mpkase add parametritrazilice varchar(200) null
GO 


if not exists (select * from syscolumns where name='danaodgode' and id=object_id('KntNacinPlac')) 
alter  table KntNacinPlac add danaodgode int not null default(0)
GO 

if not exists (select * from syscolumns where name='akcija' and id=object_id('cjenik')) 
alter  table cjenik add akcija int null 
GO 

if not exists (select * from syscolumns where name='MPPonudaglaid' and id=object_id('mpblagajnagla')) 
alter  table mpblagajnagla add MPPonudaglaid int null 
GO 

if not exists (select * from syscolumns where name='MPPonudaglaid' and id=object_id('prijenosprometa')) 
alter  table prijenosprometa add MPPonudaglaid int null 
GO 

if not exists (select * from syscolumns where name='MPPonudadetid' and id=object_id('prijenosprometa')) 
alter  table prijenosprometa add MPPonudadetid int null 
GO 



if not exists (select * from syscolumns where name='JMCijena' and id=object_id('mpetikete')) 
alter table mpetikete add JMCijena money NOT NULL DEFAULT (0)
GO

if not exists (select * from syscolumns where name='JMCijenaNaziv' and id=object_id('mpetikete')) 
alter table mpetikete add JMCijenaNaziv varchar(50) NULL
GO

if not exists (select * from syscolumns where name='VaganaRoba' and id=object_id('roba')) 
alter table roba add VaganaRoba int  NULL
GO

if not exists (select * from syscolumns where name='robaid' and id=object_id('mpetikete')) 
alter table mpetikete add robaid int null 
GO


if not exists (select * from syscolumns where name='SifraClana' and id=object_id('MPPonudaGla')) 
alter table MPPonudaGla add SifraClana varchar(20) null 
GO

if not exists (select * from syscolumns where name='kntnacinplacid' and id=object_id('MPPonudaGla')) 
alter table MPPonudaGla add kntnacinplacid int null 
GO

if not exists (select * from syscolumns where name='BodoviRealizacija' and id=object_id('MPPonudaGla')) 
alter table MPPonudaGla add BodoviRealizacija int null 
GO

if not exists (select * from syscolumns where name='Rabat' and id=object_id('MPPonudaGla')) 
alter table MPPonudaGla add rabat money not null default(0)
GO

if not exists (select * from syscolumns where name='Vrsta' and id=object_id('ParAsortimanTip')) 
alter table ParAsortimanTip add Vrsta varchar(1) null 
GO

if not exists (select * from syscolumns where name='ParAsortimanTipid' and id=object_id('ParAsortimanTip')) 
alter table ParAsortimanTip add ParAsortimanTipid  int not null identity
GO

if not exists (select * from syscolumns where name='Aktivno' and id=object_id('ParAsortiman')) 
alter table ParAsortiman add Aktivno  int  null
GO

if not exists (select * from syscolumns where name='ParAsortimanid' and id=object_id('ParAsortiman')) 
alter table ParAsortiman add ParAsortimanid int not null identity

GO

if not exists (select * from syscolumns where name='NaKasu' and id=object_id('MPKase')) 
alter table MPKase add NaKasu int null 

GO

if not exists (select * from syscolumns where name='RabatUsluge' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add RabatUsluge money not null default(0)

GO
if not exists (select * from syscolumns where name='WebUserID' and id=object_id('mpclanovi')) 
alter table  mpclanovi add WebUserID int null 

GO


if not exists (select * from syscolumns where name='RokIsporukeD' and id=object_id('MPPonudadet')) 
alter table  MPPonudadet add RokIsporukeD  datetime null 

GO

if not exists (select * from syscolumns where name='VrijediDana' and id=object_id('MPPonudagla')) 
alter table  MPPonudagla add VrijediDana  int not null default (0)

GO

if not exists (select * from syscolumns where name='rabat' and id=object_id('maloprodajagla')) 
alter table  maloprodajagla add rabat money not null default (0)

GO

if not exists (select * from syscolumns where name='Obavijest' and id=object_id('mpclanovi')) 
alter table  mpclanovi add Obavijest int null 

GO
if not exists (select * from syscolumns where name='userid' and id=object_id('mpetikete')) 
alter table mpetikete add userid int null 
GO

if not exists (select * from syscolumns where name='sifra' and id=object_id('mpetikete')) 
alter table mpetikete add sifra varchar(25) null 
GO

if not exists (select * from syscolumns where name='rabatdp' and id=object_id('MPInsertDet')) 
alter table MPInsertDet add rabatdp money not null default(0)
GO

if not exists (select * from syscolumns where name='indkredit' and id=object_id('Kntnacinplac')) 
alter table Kntnacinplac add indkredit char(1) null 
GO


if not exists (select * from syscolumns where name='MPIzdatnicaZakljuci' and id=object_id('vrstadok')) 
alter table vrstadok add MPIzdatnicaZakljuci int not null default(0)
GO

if not exists (select * from syscolumns where name='TrosakDostave' and id=object_id('MPPonudaGla')) 
alter table MPPonudaGla add TrosakDostave int not null default(1)
GO

if not exists (select * from syscolumns where name='indstanje' and id=object_id('MPInventuragla')) 
alter table MPInventuragla add indstanje int null 
GO

if not exists (select * from syscolumns where name='Datumod' and id=object_id('MPBodoviRealizacija')) 
alter table MPBodoviRealizacija add Datumod datetime null 
GO

if not exists (select * from syscolumns where name='DatumDo' and id=object_id('MPBodoviRealizacija')) 
alter table MPBodoviRealizacija add DatumDo datetime null 
GO

if not exists (select * from syscolumns where name='faktor' and id=object_id('MPBodoviRealizacija')) 
alter table MPBodoviRealizacija add faktor money default (1) 
GO

if not exists (select * from syscolumns where name='IdSKase' and id=object_id('MPClanoviPromet')) 
alter table MPClanoviPromet add IdSKase int null
GO

if not exists (select * from syscolumns where name='MPKaseid' and id=object_id('MPClanoviPromet')) 
alter table MPClanoviPromet add MPKaseid int null
GO

if not exists (select * from syscolumns where name='Napomena' and id=object_id('MPInventuragla')) 
alter table MPInventuragla add Napomena varchar(300) null 
GO

if not exists (select * from syscolumns where name='Barcodempc' and id=object_id('MPKase')) 
alter table MPKase add Barcodempc int null 
GO

if not exists (select * from syscolumns where name='kolicinaDonos' and id=object_id('MPInventuraDet')) 
alter table MPInventuraDet add kolicinaDonos money not null default(0)
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('VrstaDok')) 
alter table VrstaDok add partneriid int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('KntNacinplac')) 
alter table KntNacinplac add partneriid int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('MPClanovi')) 
alter table MPClanovi add partneriid int null 
GO

if not exists (select * from syscolumns where name='sifraclana' and id=object_id('Maloprodajagla')) 
alter table Maloprodajagla add sifraclana varchar(30)
GO

if not exists (select * from syscolumns where name='bodovirealizacija' and id=object_id('Maloprodajagla')) 
alter table Maloprodajagla add bodovirealizacija money not null default(0)
GO

if not exists (select * from syscolumns where name='robaid' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add robaid int null 
GO

if not exists (select * from syscolumns where name='klmasterrobaid' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add klmasterrobaid int null 
GO

if not exists (select * from syscolumns where name='robaid' and id=object_id('mpUvjetiKasePrijenos')) 
alter table mpUvjetiKasePrijenos add robaid int null 
GO

if not exists (select * from syscolumns where name='sortkey' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add sortkey varchar(20) null 
GO

if not exists (select * from syscolumns where name='klmasterrobaid' and id=object_id('mpUvjetiKasePrijenos')) 
alter table mpUvjetiKasePrijenos add klmasterrobaid int null 
GO

if not exists (select * from syscolumns where name='sortkey' and id=object_id('mpUvjetiKasePrijenos')) 
alter table mpUvjetiKasePrijenos add sortkey varchar(20) null 
GO

if exists (select * from syscolumns where name='mpkaseid' and id=object_id('mpUvjetiKasePrijenos')) 
alter table mpUvjetiKasePrijenos alter column mpkaseid int null 
GO

if exists (select * from syscolumns where name='napomena' and id=object_id('Maloprodajadet')) 
	alter table Maloprodajadet alter column napomena varchar(100)null
GO

if exists (select * from syscolumns where name='napomena' and id=object_id('mpnargla')) 
alter table mpnargla alter column napomena varchar(100) null  
GO

if exists (select * from syscolumns where name='opis' and id=object_id('mpnardet')) 
alter table mpnardet alter column opis varchar(100) null  
GO


if not exists (select * from syscolumns where name='MPizVP' and id=object_id('vrstadok')) 
alter table vrstadok add  MPizVP int null 
GO

if not exists (select * from syscolumns where name='vrijemezak' and id=object_id('MPObrada')) 
alter table MPObrada add  vrijemezak datetime null 
GO

if not exists (select * from syscolumns where name='Rate' and id=object_id('Kntnacinplac')) 
alter table Kntnacinplac add  rate int null 
GO

if not exists (select * from syscolumns where name='BrojRata' and id=object_id('MPNaplata')) 
alter table MPNaplata add  BrojRata int not null default(0)
GO

if not exists (select * from syscolumns where name='BrojRata' and id=object_id('PrijenosNaplate')) 
alter table PrijenosNaplate add  BrojRata int not null default(0)
GO

if not exists (select * from syscolumns where name='DatumRodjenja' and id=object_id('PKOsobe')) 
alter table PKOsobe add  DatumRodjenja datetime null 
GO

if not exists (select * from syscolumns where name='BrojBodova' and id=object_id('MaloprodajaGla')) 
        alter table MaloprodajaGla add BrojBodova int not null default (0)
GO

if not exists (select * from syscolumns where name='rezervacije' and id=object_id('mpponudadet')) 
	alter table mpponudadet add rezervacije int null
GO

if exists (select * from syscolumns where name='datumdo' and id=object_id('MPProizvodiGla')) 
	alter table MPProizvodiGla alter column datumdo datetime null 
GO

if exists (select * from syscolumns where name='datumdo' and id=object_id('MPProizvodiGla')) 
	alter table MPProizvodiGla alter column datumod datetime null 
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('MPProizvodiGla')) 
	alter table MPProizvodiGla add operateriid int null 
GO

if not exists (select * from syscolumns where name='MPSirovina' and id=object_id('Roba')) 
	alter table Roba add MPSirovina int not null default(0)
GO

if not exists (select * from syscolumns where name='Satod' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add satod char(2) null
GO

if not exists (select * from syscolumns where name='Satdo' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add satdo char(2) null
GO

if not exists (select * from syscolumns where name='MinOd' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add MinOd char(2) null
GO

if not exists (select * from syscolumns where name='Mindo' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add Mindo char(2) null
GO

if not exists (select * from syscolumns where name='Satod' and id=object_id('mpuvjetikasePrijenos')) 
alter table mpuvjetikasePrijenos add satod char(2) null
GO

if not exists (select * from syscolumns where name='Satdo' and id=object_id('mpuvjetikasePrijenos')) 
alter table mpuvjetikasePrijenos add satdo char(2) null
GO

if not exists (select * from syscolumns where name='MinOd' and id=object_id('mpuvjetikasePrijenos')) 
alter table mpuvjetikasePrijenos add MinOd char(2) null
GO

if not exists (select * from syscolumns where name='Mindo' and id=object_id('mpuvjetikasePrijenos')) 
alter table mpuvjetikasePrijenos add Mindo char(2) null
GO

if not exists (select * from syscolumns where name='porezpp' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add porezpp money not null default(0)
GO

if not exists (select * from syscolumns where name='porezppid' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add porezppid int null 
GO

if not exists (select * from syscolumns where name='mpponudadetid' and id=object_id('rezervacije')) 
alter table rezervacije add mpponudadetid int null 
GO

if not exists (select * from syscolumns where name='odobreno' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add odobreno int null 
GO

if not exists (select * from syscolumns where name='iznos' and id=object_id('MPVezaniCjenik')) 
alter table MPVezaniCjenik add iznos money not null default(0)
GO

if not exists (select * from syscolumns where name='iznos' and id=object_id('MPVezanicjenikUvjeti')) 
alter table MPVezanicjenikUvjeti add iznos money not null default(0)
GO

if not exists (select * from syscolumns where name='rezervni' and id=object_id('mpproizvodidet')) 
alter table mpproizvodidet add rezervni int null
GO

if not exists (select * from syscolumns where name='ZbirnaNarId' and id=object_id('MPNarGla')) 
alter table MPNarGla add ZbirnaNarId int null
GO

if not exists (select * from syscolumns where name='Iznospp' and id=object_id('MPBlagajnagla')) 
alter table MPBlagajnagla add Iznospp money not null default(0)
GO

if not exists (select * from syscolumns where name='Iznospp' and id=object_id('Maloprodajagla')) 
alter table Maloprodajagla add Iznospp money not null default(0)
GO

if not exists (select * from syscolumns where name='Iznospp' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add Iznospp money not null default(0)
GO

if not exists (select * from syscolumns where name='porezpp' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add porezpp money not null default(0)
GO
if not exists (select * from syscolumns where name='porezippid' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add porezippid int null 
GO

if not exists (select * from syscolumns where name='Iznospp' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add Iznospp money not null default(0)
GO

if not exists (select * from syscolumns where name='porezpp' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add porezpp money not null default(0)
GO
if not exists (select * from syscolumns where name='porezippid' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add porezippid int null 
GO

if not exists (select * from syscolumns where name='ObracunPro1id' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add ObracunPro1id int null 
GO

if not exists (select * from syscolumns where name='ObracunPro2id' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add ObracunPro2id int null 
GO

if not exists (select * from syscolumns where name='MPSirovina' and id=object_id('mpinventuradet')) 
alter table mpinventuradet add mpsirovina int null 
GO

if not exists (select * from syscolumns where name='mpvrijednost' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add mpvrijednost money not null default (0)
GO

if not exists (select * from syscolumns where name='netocijena' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add netocijena money not null default (0)
GO
if not exists (select * from syscolumns where name='pdvvrijednost' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add pdvvrijednost money not null default (0)
GO
if not exists (select * from syscolumns where name='Netovrijednost' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add Netovrijednost money not null default (0)
GO

if exists (select * from syscolumns where name='Poreziid' and id=object_id('MPUgoRoba')) 
alter table MPUgoRoba alter column poreziid int null 
GO

if not exists (select * from syscolumns where name='popust' and id=object_id('izracunbodova')) 
alter table izracunbodova add popust money not null default(0)
GO


if not exists (select * from syscolumns where name='UgoInd' and id=object_id('Trgovina')) 
alter table Trgovina add UgoInd int null 
GO

if not exists (select * from syscolumns where name='PrijenosMPO' and id=object_id('MPKase')) 
alter table MPKase add PrijenosMPO int null 
GO

if not exists (select * from syscolumns where name='UpisClana' and id=object_id('MPKase')) 
alter table MPKase add UpisClana int null 
GO

if not exists (select * from syscolumns where name='Dan' and id=object_id('MPUVJETikase')) 
alter table MPUVJETikase add Dan Varchar(20) null 
GO

if not exists (select * from syscolumns where name='indProi' and id=object_id('Trgovina')) 
alter table Trgovina add indProi int null 
GO

if not exists (select * from syscolumns where name='parstruid' and id=object_id('MPPrometBlag')) 
alter table MPPrometBlag add parstruid int null 
GO

if not exists (select * from syscolumns where name='mjestaID' and id=object_id('MPKupciR1')) 
alter table MPKupciR1 add mjestaid int null 
GO

if not exists (select * from syscolumns where name='Oib' and id=object_id('MPKupciR1')) 
alter table MPKupciR1 add Oib varchar(20) not null default ('')
GO

if not exists (select * from syscolumns where name='skladisteid' and id=object_id('MPNarGla')) 
alter table MPNarGla add skladisteid int null 
GO

if exists (select * from syscolumns where name='status' and id=object_id('mpnargla')) 
	alter table mpnargla alter column status int null 
GO
if not exists (select * from syscolumns where name='kolicinaizlaznalozena' and id=object_id('sktprometdet')) 
alter table sktprometdet add kolicinaizlaznalozena money not null default(0)
GO

if not exists (select * from syscolumns where name='zabranipopust' and id=object_id('mpUvjetiKase')) 
alter table mpUvjetiKase add zabranipopust int not null default(0)
GO

if not exists (select * from syscolumns where name='zabranipopust' and id=object_id('mpUvjetiKasePrijenos')) 
alter table mpUvjetiKasePrijenos add zabranipopust int not null default(0)
GO

if not exists (select * from syscolumns where name='rabatinekp' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add rabatinekp money not null default(0)
GO

if not exists (select * from syscolumns where name='indmoze' and id=object_id('mpkase')) 
alter table mpkase add indmoze int not null default(0)
GO

if not exists (select * from syscolumns where name='telefon' and id=object_id('PrijenosClanovi')) 
        alter table PrijenosClanovi add telefon varchar(50) null 
GO

if not exists (select * from syscolumns where name='mobitel' and id=object_id('PrijenosClanovi')) 
        alter table PrijenosClanovi add mobitel varchar(50) null 
GO
if not exists (select * from syscolumns where name='email' and id=object_id('PrijenosClanovi')) 
        alter table PrijenosClanovi add email varchar(50) null 
GO

if not exists (select * from syscolumns where name='vrijemeprijenosa' and id=object_id('mpkase')) 
        alter table mpkase add vrijemeprijenosa datetime  null 
GO
if not exists (select * from syscolumns where name='enablejob' and id=object_id('mpkase')) 
        alter table mpkase add enablejob int  not null default(1)
GO

if not exists (select * from syscolumns where name='rabatnekp' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add rabatnekp money not null default(0)
GO

if not exists (select * from syscolumns where name='vrijemeotvaranja' and id=object_id('MPInventuragla')) 
        alter table MPInventuragla add vrijemeotvaranja datetime null 
GO

if not exists (select * from syscolumns where name='vrijemepromjene' and id=object_id('MPInventuragla')) 
        alter table MPInventuragla add vrijemepromjene datetime null 
GO

if not exists (select * from syscolumns where name='datumdo' and id=object_id('MPVezanaRobaUvjeti')) 
        alter table MPVezanaRobaUvjeti add datumdo datetime null 
GO

if not exists (select * from syscolumns where name='datumdo' and id=object_id('MPVezanaRoba')) 
        alter table MPVezanaRoba add datumdo datetime null 
GO


if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('MPVezanaRoba')) 
        alter table MPVezanaRoba add trgovinaid int null 
GO


if not exists (select * from syscolumns where name='vrijeme' and id=object_id('MPVezanaRobaUvjeti')) 
        alter table MPVezanaRobaUvjeti add vrijeme datetime null 
GO

if not exists (select * from syscolumns where name='indprimanje' and id=object_id('mpkase')) 
        alter table mpkase add indprimanje int not null default (1) 
GO

if not exists (select * from syscolumns where name='RadniSati' and id=object_id('Trgovina')) 
        alter table Trgovina add RadniSati int not null default (0) 
GO

if not exists (select * from syscolumns where name='EnterKraj' and id=object_id('MPkase')) 
        alter table MPkase add EnterKraj int not null default (1) 
GO


if not exists (select * from syscolumns where name='VrijemePrijenosa' and id=object_id('MPkase')) 
        alter table MPkase add VrijemePrijenosa datetime null 
GO

update vrstadok set mpizvp = 1 
--select v.*
from vrstadok v 
inner join vrstaprom vp on v.vrstapromid = vp.vrstapromid and vp.projekt = 'M' and mpizvp is null 



if not exists (select * from syscolumns where name='vrijemedeaktivacije' and id=object_id('MPVezanaRobaUvjeti')) 
alter table MPVezanaRobaUvjeti add vrijemedeaktivacije datetime null 
GO

if not exists (select * from syscolumns where name='operaterideaktivacijeid' and id=object_id('MPVezanaRobaUvjeti')) 
alter table MPVezanaRobaUvjeti add operaterideaktivacijeid int null 
GO


if not exists (select * from syscolumns where name='MPTipUvjetaPopustaid' and id=object_id('mpUvjetiKase')) 
        alter table mpUvjetiKase add MPTipUvjetaPopustaid int not null   default (0)
GO

if not exists (select * from syscolumns where name='kolicinaminus' and id=object_id('vrstadok')) 
        alter table vrstadok add kolicinaminus int not null default(1)  
GO

if not exists (select * from syscolumns where name='prioritet' and id=object_id('mpUvjetiKasePrijenos')) 
        alter table mpUvjetiKasePrijenos add prioritet int not null default(0)  
GO

if not exists (select * from syscolumns where name='odracuna' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add odracuna varchar(10) null 
GO

if not exists (select * from syscolumns where name='doracuna' and id=object_id('maloprodajagla')) 
        alter table maloprodajagla add doracuna varchar(10) null 
GO
if not exists (select * from syscolumns where name='komisija' and id=object_id('mpstanje')) 
        alter table mpstanje add komisija int not null default(0)
GO
if not exists (select * from syscolumns where name='komisija' and id=object_id('vrstadok')) 
        alter table vrstadok add komisija int not null default(0)
GO
if not exists (select * from syscolumns where name='rata' and id=object_id('vrstadok')) 
        alter table vrstadok add rata int not null default(0)
GO

if not exists (select * from syscolumns where name='napomena2' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add napomena2 varchar(300)null
GO

if not exists (select * from syscolumns where name='maloprodajaglaid' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add maloprodajaglaid int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('mpstanje')) 
alter table mpstanje add partneriid int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('MPUvjetikase')) 
alter table MPUvjetikase add partneriid int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('MPUvjetikasePrijenos')) 
alter table MPUvjetikasePrijenos add partneriid int null 
GO

if not exists (select * from syscolumns where name='parasortimanstruid' and id=object_id('parasortimanstru')) 
alter table parasortimanstru add parasortimanstruid int IDENTITY(1,1) NOT NULL primary key
GO 

if not exists (select * from syscolumns where name='sort' and id=object_id('vrstadok')) 
alter table vrstadok add sort int NULL 
GO 

if not exists (select * from syscolumns where name='datumknjizenja' and id=object_id('mpobrada')) 
alter table mpobrada add datumknjizenja datetime NULL 
GO 
if not exists (select * from syscolumns where name='DatumObnoveNC' and id=object_id('mpobrada')) 
alter table mpobrada add DatumObnoveNC datetime NULL 
GO 
if not exists (select * from syscolumns where name='OperateriKnjizenjaid' and id=object_id('mpobrada')) 
alter table mpobrada add OperateriKnjizenjaid int NULL 
GO 
if not exists (select * from syscolumns where name='OperateriObnoveNCid' and id=object_id('mpobrada')) 
alter table mpobrada add OperateriObnoveNCid int NULL 
GO 

if not exists (select * from syscolumns where name='OperateriObradeid' and id=object_id('mpobrada')) 
alter table mpobrada add OperateriObradeid int NULL 
GO 


if not exists (select * from syscolumns where name='indfin' and id=object_id('trgovina')) 
alter table trgovina add indfin int not NULL default(0)
GO 

if not exists (select * from syscolumns where name='email' and id=object_id('mpwebshopisporuka')) 
alter table mpwebshopisporuka add email varchar(50) null 
GO 

if not exists (select * from syscolumns where name='telefon' and id=object_id('mpwebshopisporuka')) 
alter table mpwebshopisporuka add telefon varchar(50) null 
GO 

if not exists (select * from syscolumns where name='mobitel' and id=object_id('mpwebshopisporuka')) 
alter table mpwebshopisporuka add mobitel varchar(50) null 
GO 

if not exists (select * from syscolumns where name='mprazlogpovrataid' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add mprazlogpovrataid int  null 
GO 

if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('MPVezanaRobaUvjeti')) 
alter table MPVezanaRobaUvjeti add trgovinaid int null 
GO

if not exists (select * from syscolumns where name='vrcjenikid' and id=object_id('MPVezanaRobaUvjeti')) 
alter table MPVezanaRobaUvjeti add vrcjenikid int null 
GO

if not exists (select * from syscolumns where name='kratkinaziv' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add kratkinaziv varchar(25)
GO

if not exists (select * from syscolumns where name='indusluga' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add indusluga int null 
GO

if not exists (select * from syscolumns where name='poreziid' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add poreziid int null 
GO

if not exists (select * from syscolumns where name='klasakasa' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add klasakasa varchar(10) null 
GO


if not exists (select * from syscolumns where name='telefon' and id=object_id('prijenosclanoving')) 
alter table prijenosclanoving add telefon varchar(50) null 
GO
if not exists (select * from syscolumns where name='mobitel' and id=object_id('prijenosclanoving')) 
alter table prijenosclanoving add mobitel varchar(50) null 
GO
if not exists (select * from syscolumns where name='email' and id=object_id('prijenosclanoving')) 
alter table prijenosclanoving add email varchar(50) null 
GO


if not exists (select * from syscolumns where name='kolicinaizlazorg' and id=object_id('mpponudadet')) 
	alter table mpponudadet add kolicinaizlazorg  money not null default (0)
GO

if not exists (select * from syscolumns where name='kolicinaizlazpromovirano' and id=object_id('mpponudadet')) 
	alter table mpponudadet add kolicinaizlazpromovirano  money not null default (0)
GO

if not exists (select * from syscolumns where name='kolicinaizlazstorno' and id=object_id('mpponudadet')) 
	alter table mpponudadet add kolicinaizlazstorno  money not null default (0)
GO

if not exists (select * from syscolumns where name='MPPonudaDetID' and id=object_id('mpblagajnadet')) 
  alter table mpblagajnadet add MPPonudaDetID int null 
GO


if not exists (select * from syscolumns where name='vrstadokid' and id=object_id('mpinsertdet')) 
  alter table mpinsertdet add vrstadokid int null 
GO




if not exists (select * from syscolumns where name='SifraPrikaz' and id=object_id('mpkase')) 
alter table mpkase add SifraPrikaz varchar(40) NULL
GO

if not exists (select * from syscolumns where name='DefaultPlac' and id=object_id('mpkase')) 
alter table mpkase add DefaultPlac char(10) NULL
GO

if not exists (select * from syscolumns where name='OsnSifra' and id=object_id('mpkase')) 
alter table mpkase add OsnSifra varchar(50) NULL
GO

if not exists (select * from syscolumns where name='kasaizbor' and id=object_id('mpkase')) 
alter table mpkase add kasaizbor int null 
GO




if not exists (select * from syscolumns where name='rate' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add rate int null 
GO

if not exists (select * from syscolumns where name='vrcjenikid' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add vrcjenikid int null 
GO

if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add trgovinaid int null 
GO

if not exists (select * from syscolumns where name='brojrata' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add brojrata money NOT NULL DEFAULT (0)
GO

if not exists (select * from syscolumns where name='popust' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add popust money NOT NULL DEFAULT (0) 
GO

if not exists (select * from syscolumns where name='Datumod' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add Datumod datetime NULL
GO

if not exists (select * from syscolumns where name='Datumdo' and id=object_id('PKKreditiUvjeti')) 
alter table PKKreditiUvjeti add Datumdo datetime NULL
GO



if not exists (select * from syscolumns where name='staro' and id=object_id('PKKreditiPrometDet')) 
alter table PKKreditiPrometDet add staro int null 
GO

if not exists (select * from syscolumns where name='Napomena' and id=object_id('PKKreditiPrometDet')) 
alter table PKKreditiPrometDet add Napomena varchar(50) NULL 
GO

if not exists (select * from syscolumns where name='staro1' and id=object_id('PKKreditiPrometDet')) 
alter table PKKreditiPrometDet add staro1 int null 
GO

if not exists (select * from syscolumns where name='uplatekreditaid' and id=object_id('PKKreditiPrometDet')) 
alter table PKKreditiPrometDet add uplatekreditaid int null 
GO

if not exists (select * from syscolumns where name='datumdokumenta' and id=object_id('PKKreditiPrometDet')) 
alter table PKKreditiPrometDet add datumdokumenta datetime NULL
GO


if not exists (select * from syscolumns where name='CNetoVrijednost' and id=object_id('Maloprodajadet')) 
alter table Maloprodajadet add CNetoVrijednost AS ((mpvrijednost-pdvvrijednost)-iznospp)
GO


if not exists (select * from syscolumns where name='MPPonudaGlaId' and id=object_id('sktprometgla'))
alter table sktprometgla add MPPonudaGlaId int null --references MPPonudagla (MPPonudaGlaId)
GO


if not exists (select * from syscolumns where name='osobnibroj' and id=object_id('PKOsobe')) 
alter table PKOsobe add osobnibroj varchar(50) NULL
GO

if not exists (select * from syscolumns where name='telefon' and id=object_id('PKOsobe')) 
alter table PKOsobe add telefon varchar(50) NULL
GO


if not exists (select * from syscolumns where name='Privremeniracun' and id=object_id('MPKase')) 
alter table MPKase add Privremeniracun int not NULL default(0)
GO

if not exists (select * from syscolumns where name='operateriid' and id=object_id('mpnaplataBlag')) 
alter table mpnaplataBlag add operateriid int not NULL 
GO

if not exists (select * from syscolumns where name='IndKupUgovori' and id=object_id('Trgovina')) 
alter table Trgovina add IndKupUgovori int NULL 
GO

if not exists (select * from syscolumns where name='povnaknadaroba' and id=object_id('MPReversTemp')) 
alter table MPReversTemp add povnaknadaroba money not NULL default('0')
GO

if not exists (select * from syscolumns where name='nextvrdid' and id=object_id('MPReversTemp')) 
alter table MPReversTemp add nextvrdid int NULL 
GO
if not exists (select * from syscolumns where name='AmbalazaPovratna' and id=object_id('MPReversTemp')) 
alter table MPReversTemp add AmbalazaPovratna int not NULL default('0')
GO


if not exists (select * from syscolumns where name='fakcijena' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add fakcijena money not NULL default('0')
GO
if not exists (select * from syscolumns where name='samokomisija' and id=object_id('MPInventuragla')) 
alter table MPInventuragla add samokomisija int not NULL default('0')
GO
if not exists (select * from syscolumns where name='partneriid' and id=object_id('MPInventuragla')) 
alter table MPInventuragla add partneriid int NULL 
GO

if exists (select * from syscolumns where name='trgovinaid' and id=object_id('TempPromNC')) 
alter table TempPromNC alter column trgovinaid int null 
GO

if not exists (select * from syscolumns where name='VrijemeKreiranja' and id=object_id('mpponudagla')) 
	alter table mpponudagla add VrijemeKreiranja datetime null
GO

if not exists (select * from syscolumns where name='KKvalMP' and id=object_id('Roba')) 
	alter table Roba add KKvalMP int not null default(0)
GO

if not exists (select * from syscolumns where name='KKvalMP' and id=object_id('MPStanjeZaKasu')) 
	alter table MPStanjeZaKasu add KKvalMP int not null default(0)
GO

if not exists (select * from syscolumns where name='ProNacinIsporukeId' and id=object_id('Maloprodajagla')) 
	alter table Maloprodajagla add ProNacinIsporukeId int null
GO

if not exists (select * from syscolumns where name='kontigent' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add kontigent varchar(50) null 
GO
if not exists (select * from syscolumns where name='naziv' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add naziv varchar(50) null 
GO
if not exists (select * from syscolumns where name='adresa' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add adresa varchar(100) null 
GO
if not exists (select * from syscolumns where name='mjesto' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add mjesto varchar(50) null 
GO
if not exists (select * from syscolumns where name='maticnibroj' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add maticnibroj varchar(50) null 
GO
if not exists (select * from syscolumns where name='KupciKontigentID' and id=object_id('prijenosprometa')) 
	alter table prijenosprometa add KupciKontigentID int  null 
GO

if not exists (select * from syscolumns where name='kontigent' and id=object_id('mpblagajnadet')) 
	alter table mpblagajnadet add kontigent varchar(50) null 
GO
if not exists (select * from syscolumns where name='KupciKontigentID' and id=object_id('mpblagajnaDET')) 
	alter table mpblagajnaDET add KupciKontigentID int null 
GO

if not exists (select * from syscolumns where name='kkvalmp' and id=object_id('MPPrometBlag')) 
	alter table MPPrometBlag add kkvalmp int null 
GO
if not exists (select * from syscolumns where name='VaganaRoba' and id=object_id('MPStanjeZaKasu')) 
	alter table MPStanjeZaKasu add VaganaRoba int null 
GO
if not exists (select * from syscolumns where name='Trgovina2id' and id=object_id('MPPonudagla')) 
	alter table MPPonudagla add Trgovina2id int null 
GO
if not exists (select * from syscolumns where name='Partneriid' and id=object_id('MPPonudadet')) 
	alter table MPPonudadet add Partneriid int null 
GO
if not exists (select * from syscolumns where name='AmbalazaPovratna' and id=object_id('MPPonudadet')) 
alter table MPPonudadet add AmbalazaPovratna int not NULL default('0')
GO

if not exists (select * from syscolumns where name='nosibodove' and id=object_id('mpblagajnadet')) 
alter table mpblagajnadet add nosibodove char(1) NULL 
GO

if not exists (select * from syscolumns where name='NacinIsporukeId' and id=object_id('MPPonudagla')) 
	alter table MPPonudagla add NacinIsporukeId int null 
GO

if not exists (select * from syscolumns where name='IndOrgShemaVP' and id=object_id('trgovina')) 
	alter table trgovina add IndOrgShemaVP int not null default(0)
GO

if not exists (select * from syscolumns where name='MPDatumDok' and id=object_id('Vrstadok')) 
	alter table Vrstadok add MPDatumDok int not null default(0)
GO

if not exists (select * from syscolumns where name='sortkey' and id=object_id('mprobalojaliti')) 
	alter table mprobalojaliti add sortkey varchar(30) null 
GO



if not exists (select * from syscolumns where name='OIB' and id=object_id('lntrgovinaoperateri')) 
	alter table lntrgovinaoperateri add OIB varchar(30) null 
GO

if not exists (select * from syscolumns where name='hrkadroviid' and id=object_id('lntrgovinaoperateri')) 
	alter table lntrgovinaoperateri add hrkadroviid int  null 
GO

if not exists (select * from syscolumns where name='NazivOpr' and id=object_id('lntrgovinaoperateri')) 
	alter table lntrgovinaoperateri add NazivOpr varchar(50) null 
GO

if not exists (select * from syscolumns where name='OIB' and id=object_id('ssoperateri')) 
	alter table ssoperateri add OIB varchar(15) null 
GO

if not exists (select * from syscolumns where name='fakcijena' and id=object_id('cjenik')) 
alter table cjenik add fakcijena money not NULL default('0')
GO

if not exists (select * from syscolumns where name='brojfisk' and id=object_id('mpblagajnagla')) 
alter table mpblagajnagla add brojfisk varchar(20) NULL 
GO

if not exists (select * from syscolumns where name='KOD' and id=object_id('mpblagajnagla'))
	alter table mpblagajnagla add kod varchar(100) null 
GO  

if not exists (select * from syscolumns where name='JIR' and id=object_id('mpblagajnagla'))
	alter table mpblagajnagla add JIR varchar(100) null 
GO  

if not exists (select * from syscolumns where name='IDPoruke' and id=object_id('mpblagajnagla'))
	alter table mpblagajnagla add IDPoruke varchar(100) null 
GO  


if not exists (select * from syscolumns where name='UUID' and id=object_id('mpblagajnagla'))
                alter table mpblagajnagla add UUID varchar(36) null 
GO  


if not exists (select * from syscolumns where name='kucnibroj' and id=object_id('trgovina'))
                alter table trgovina add kucnibroj varchar(100) null 
GO  
if not exists (select * from syscolumns where name='KucniBrojDodatak' and id=object_id('trgovina'))
                alter table trgovina add KucniBrojDodatak varchar(100) null 
GO  


if not exists (select * from syscolumns where name='DatumSlanja' and id=object_id('Trgovina'))
                alter table Trgovina add DatumSlanja datetime null 
GO  
if not exists (select * from syscolumns where name='UUID' and id=object_id('Trgovina'))
                alter table Trgovina add UUID varchar(36) null
GO  

if not exists (select * from syscolumns where name='sifragreske' and id=object_id('Trgovina'))
                alter table Trgovina add sifragreske varchar(5) null
GO  

if not exists (select * from syscolumns where name='KOD' and id=object_id('mpblagajnagla'))
	alter table mpblagajnagla add kod varchar(100) null 
GO  

if not exists (select * from syscolumns where name='UGO' and id=object_id('mpblagajnagla'))
	alter table mpblagajnagla add ugo varchar(100) null 
GO  
if not exists (select * from syscolumns where name='UGO' and id=object_id('prijenosprometaNG'))
	alter table prijenosprometaNG add ugo int not null default (0) 
GO  
if not exists (select * from syscolumns where name='UGO' and id=object_id('prijenosprometa'))
	alter table prijenosprometa add ugo int not null default (0) 
GO  

if not exists (select * from syscolumns where name='JIR' and id=object_id('prijenosprometaNG'))
	alter table prijenosprometaNG add JIR varchar(100) null 
GO  

if not exists (select * from syscolumns where name='sifragreske' and id=object_id('prijenosprometaNG'))
	alter table prijenosprometaNG add sifragreske varchar(100) null 
GO  


if not exists (select * from syscolumns where name='UUID' and id=object_id('prijenosprometaNG'))
                alter table prijenosprometaNG add UUID varchar(36) null 
GO  






if not exists (select * from syscolumns where name='Partneriid' and id=object_id('MPFiskalizacija'))
	alter table MPFiskalizacija add partneriid int null 
GO  

if not exists (select * from syscolumns where name='pfx' and id=object_id('mpfiskalizacija'))
	alter table mpfiskalizacija add pfx varchar(200) null 
GO  

if not exists (select * from syscolumns where name='lozinkazapfx' and id=object_id('mpfiskalizacija'))
                alter table mpfiskalizacija add lozinkazapfx varchar(100) null 
GO  

if not exists (select * from syscolumns where name='putanjapfx' and id=object_id('mpfiskalizacija'))
                alter table mpfiskalizacija add putanjapfx varchar(100) null 
GO  

if not exists (select * from syscolumns where name='kucnibroj' and id=object_id('trgovina'))
                alter table trgovina add kucnibroj varchar(100) null 
GO  
if not exists (select * from syscolumns where name='KucniBrojDodatak' and id=object_id('trgovina'))
                alter table trgovina add KucniBrojDodatak varchar(100) null 
GO  


if not exists (select * from syscolumns where name='Datumpocetka' and id=object_id('Trgovina'))
                alter table Trgovina add Datumpocetka datetime null
GO  

if not exists (select * from syscolumns where name='Datumkraja' and id=object_id('Trgovina'))
                alter table Trgovina add Datumkraja datetime null
GO  
if not exists (select * from syscolumns where name='xmllog' and id=object_id('Trgovina'))
                alter table Trgovina add xmllog varchar(8000) null
GO  

if not exists (select * from syscolumns where name='KupciKontigentID' and id=object_id('mpblagajnaGLA')) 
	alter table mpblagajnaGLA add KupciKontigentID int null 
GO

if not exists (select * from syscolumns where name='sifragreske' and id=object_id('mpblagajnaGLA')) 
	alter table mpblagajnaGLA add sifragreske varchar(10) null 
GO

if not exists (select * from syscolumns where name='paragon' and id=object_id('mpblagajnaGLA')) 
	alter table mpblagajnaGLA add paragon varchar(20) null 
GO
if not exists (select * from syscolumns where name='paragon' and id=object_id('MPPrometBlag')) 
	alter table MPPrometBlag add paragon varchar(20) null 
GO

if not exists (select * from syscolumns where name='TimeOut' and id=object_id('mpfiskalizacija')) 
	alter table mpfiskalizacija add TimeOut int not null default (5000)
GO

if not exists (select * from syscolumns where name='url' and id=object_id('mpfiskalizacija')) 
	alter table mpfiskalizacija add URL varchar(1000) not null default ('https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest')
GO

if not exists (select * from syscolumns where name='paragon' and id=object_id('prijenosprometa'))
                alter table prijenosprometa add paragon varchar(30) null 
GO  

if not exists (select * from syscolumns where name='paragon' and id=object_id('prijenosprometang'))
                alter table prijenosprometang add paragon varchar(30) null 
GO  


if not exists (select * from syscolumns where name='ProNOtpGlaId ' and id=object_id('MPPonudagla')) 
	alter table MPPonudagla add ProNOtpGlaId int null 
GO

if not exists (select * from syscolumns where name='ProPonGlaId ' and id=object_id('MPPonudagla')) 
	alter table MPPonudagla add ProPonGlaId int null 
GO


if not exists (select * from syscolumns where name='skladisteid' and id=object_id('Vrstadok')) 
	alter table Vrstadok add skladisteid int  null 
GO


if not exists (select * from syscolumns where name='vrijemejir' and id=object_id('mpblagajnagla')) 
	alter table mpblagajnagla add vrijemejir datetime null 
GO

if not exists (select * from syscolumns where name='skladisteid' and id=object_id('mpinsertdet')) 
alter table mpinsertdet add skladisteid int NULL 
GO


if not exists (select * from syscolumns where name='vrijemejir' and id=object_id('PrijenosPrometa')) 
        alter table PrijenosPrometa add vrijemejir datetime null 
GO

if not exists (select * from syscolumns where name='vrijemejir' and id=object_id('PrijenosPrometaNG')) 
        alter table PrijenosPrometang add vrijemejir datetime null 
GO
if not exists (select * from syscolumns where name='poreziid' and id=object_id('PrijenosPrometaNG')) 
        alter table PrijenosPrometang add poreziid int null 
GO

if not exists (select * from syscolumns where name='pronotpglaid' and id=object_id('maloprodajagla')) 
	alter table maloprodajagla add pronotpglaid int null 
GO 

if not exists (select * from syscolumns where name='partneriid' and id=object_id('MPBodoviRealizacija')) 
	alter table MPBodoviRealizacija add partneriid int null 
GO 

if not exists (select * from syscolumns where name='indoperoznaka' and id=object_id('MPkase')) 
	alter table MPkase add indoperoznaka int not null default(0)
GO 

if not exists (select * from syscolumns where name='spol' and id=object_id('PrijenosClanovi'))
	Alter table PrijenosClanovi add spol char(1) null 
GO
if not exists (select * from syscolumns where name='infomail' and id=object_id('PrijenosClanovi'))
	Alter table PrijenosClanovi add infomail int null 
GO

if not exists (select * from syscolumns where name='spol' and id=object_id('PrijenosClanoviNG'))
	Alter table PrijenosClanoviNG add spol char(1) null 
GO
if not exists (select * from syscolumns where name='infomail' and id=object_id('PrijenosClanoviNG'))
	Alter table PrijenosClanoviNG add infomail int null 
GO

if not exists (select * from syscolumns where name='KolicinaNaruceno' and id=object_id('mpinsertdet')) 
ALter table mpinsertdet add KolicinaNaruceno money default (0)
GO

if not exists (select * from syscolumns where name='operaterlock' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add operaterlock int null 
GO

if not exists (select * from syscolumns where name='propondetid' and id=object_id('MPPrometBlag')) 
alter table MPPrometBlag add propondetid int null 
GO

if not exists (select * from syscolumns where name='propondetid' and id=object_id('MPBlagajnadet')) 
alter table MPBlagajnadet add propondetid int null 
GO
if not exists (select * from syscolumns where name='MPBlagajnaDetid' and id=object_id('lnproponnalog')) 
alter table lnproponnalog add MPBlagajnaDetid int null 
GO

if not exists (select * from syscolumns where name='kkvalglaid' and id=object_id('MPPrometBlag')) 
alter table MPPrometBlag add kkvalglaid int null 
GO

if not exists (select * from syscolumns where name='ServisRNDetid' and id=object_id('MPPrometBlag')) 
alter table MPPrometBlag add ServisRNDetid int null 
GO

if not exists (select * from syscolumns where name='ServisRNDetid' and id=object_id('MPBlagDonos')) 
alter table MPBlagDonos add ServisRNDetid int null 
GO
if not exists (select * from syscolumns where name='ServisRNGlaid' and id=object_id('MPBlagDonos')) 
alter table MPBlagDonos add ServisRNGlaid int null 
GO
if not exists (select * from syscolumns where name='ServisRNDetid' and id=object_id('MPBlagajnadet')) 
alter table MPBlagajnadet add ServisRNDetid int null 
GO

if not exists (select * from syscolumns where name='datumotpreme' and id=object_id('lnproponnalog')) 
alter table lnproponnalog add datumotpreme datetime null 
GO


if not exists (select * from syscolumns where name='mpblagajnaglaid' and id=object_id('ServisRNDet')) 
alter table ServisRNDet add mpblagajnaglaid int null 
GO

if not exists (select * from syscolumns where name='mpnarglaid' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add mpnarglaid int null 
GO

if not exists (select * from syscolumns where name='mpnardetid' and id=object_id('maloprodajadet')) 
alter table maloprodajadet add mpnardetid int null 
GO


if not exists (select * from syscolumns where name='maloprodajaglaid' and id=object_id('mpnargla')) 
alter table mpnargla add maloprodajaglaid int null 
GO

if not exists (select * from syscolumns where name='maloprodajadetid' and id=object_id('mpnardet')) 
alter table mpnardet add maloprodajadetid  int null 
GO

if not exists (select * from syscolumns where name='mpkupcir1Id' and id=object_id('mpblagajnagla')) 
alter table mpblagajnagla add mpkupcir1Id  int null 
GO

if not exists (select * from syscolumns where name='partneriid' and id=object_id('mpkupcir1')) 
alter table mpkupcir1 add partneriid  int null 
GO

if not exists (select * from syscolumns where name='ind_kalk' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add ind_kalk int not null default(1)
GO

if not exists (select * from syscolumns where name='ind_prim' and id=object_id('maloprodajagla')) 
alter table maloprodajagla add ind_prim int not null default(0)
GO


if not exists (select * from syscolumns where name='radnovrijeme' and id=object_id('trgovina')) 
alter table trgovina add radnovrijeme varchar(1000) null 
GO

if not exists (select * from syscolumns where name='kupcir1' and id=object_id('mpkupcir1')) 
alter table mpkupcir1 add kupcir1  int null 
GO

if not exists (select * from syscolumns where name='ind_pokretnaMP' and id=object_id('trgovina')) 
alter table trgovina add ind_pokretnaMP  int not null default(0) 
GO

if not exists (select * from syscolumns where name='PortPOS2' and id=object_id('mpkase')) 
alter table mpkase add PortPOS2  varchar(50) null 
GO

if not exists (select * from syscolumns where name='ind_kuhinja' and id=object_id('mpstanjezakasu')) 
alter table mpstanjezakasu add ind_kuhinja  varchar(50) null 
GO

if not exists (select * from syscolumns where name='pos2zvono' and id=object_id('mpkase')) 
alter table mpkase add pos2zvono  varchar(50) null 
GO
if not exists (select * from syscolumns where name='NAPOMENASTAVKa' and id=object_id('MPPrometblag')) 
alter table MPPrometblag add NAPOMENASTAVKa  varchar(200) null 
GO
if not exists (select * from syscolumns where name='mattemid' and id=object_id('maloprodajagla')) 
alter table maloprodajagla  add mattemid  int  null 
GO
if not exists (select * from syscolumns where name='rabatgla' and id=object_id('maloprodajagla')) 
alter table maloprodajagla  add rabatgla  money not null default(0) 
GO

if not exists (select * from syscolumns where name='indpokretnamp' and id=object_id('trgovina')) 
alter table trgovina  add indpokretnamp  int not null default(0)
GO
if not exists (select * from syscolumns where name='napomenastavka' and id=object_id('MPKase')) 
alter table MPkase add napomenastavka  varchar(10) null 
GO
if not exists (select * from syscolumns where name='vrijeme' and id=object_id('MProbatrgovina')) 
alter table MProbatrgovina add  vrijeme datetime null 
GO

if not exists (select * from syscolumns where name='NaplacenoInd ' and id=object_id('mpblagajnagla')) 
alter table mpblagajnagla add  NaplacenoInd  int null 
GO


if not exists (select * from syscolumns where name='ServerUSer ' and id=object_id('MPKasaInstall')) 
alter table MPKasaInstall add  ServerUSer  varchar(50) not null default('sa') 
GO

if not exists (select * from syscolumns where name='ind_update' and id=object_id('MpKase')) 
alter table MpKase add  ind_update  int not null default('0') 
GO

if not exists (select * from syscolumns where name='Datum_update' and id=object_id('MpKase')) 
alter table MpKase add  Datum_update  datetime   null 
GO

if not exists (select * from syscolumns where name='akcija' and id=object_id('mpcijenapromet')) 
	alter table mpcijenapromet add akcija int not null default (0)
GO 

if not exists (select * from syscolumns where name='akcija' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add akcija int not null default (0)
GO 

if not exists (select * from syscolumns where name='DatumDo' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add DatumDo datetime null
GO

if not exists (select * from syscolumns where name='DatumDo' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add DatumDo datetime null 
GO 

if not exists (select * from syscolumns where name='MPCijenaPrometIzvorId' and id=object_id('mpcijenapromet')) 
	alter table mpcijenapromet add MPCijenaPrometIzvorId int  null 
GO 

if not exists (select * from syscolumns where name='ind_akcija' and id=object_id('TempPromMPC')) 
	alter table TempPromMPC add ind_akcija int null 
GO 

if not exists (select * from syscolumns where name='ind_akcija' and id=object_id('MPCijenaPromet')) 
	alter table MPCijenaPromet add ind_akcija int null
GO


if not exists (select * from syscolumns where name='rabat1' and id=object_id('MPPonudadet')) 
	alter table MPPonudadet add rabat1 money not null default(0)
GO 
if not exists (select * from syscolumns where name='rabat2' and id=object_id('MPPonudadet')) 
	alter table MPPonudadet add rabat2 money not null default(0)
GO 
if not exists (select * from syscolumns where name='rabat3' and id=object_id('MPPonudadet')) 
	alter table MPPonudadet add rabat3 money not null default(0)
GO 

if not exists (select * from syscolumns where name='OtkupUgovorId' and id=object_id('MaloprodajaGla')) 
alter table MaloprodajaGla add OtkupUgovorId int null
GO

if not exists (select * from syscolumns where name='MPC' and id=object_id('MPVezanaRobaUvjeti')) 
alter table MPVezanaRobaUvjeti add MPC money not null default(0)
GO

if not exists (select * from syscolumns where name='narudzba' and id=object_id('MPKASE'))
	alter table mpkase add narudzba varchar(10) null 
GO 

if not exists (select * from syscolumns where name='DonosPomocna' and id=object_id('MPKASE'))
	alter table mpkase add DonosPomocna varchar(10)
GO 
if not exists (select * from syscolumns where name='GrupeArt' and id=object_id('MPKASE'))
	alter table mpkase add GrupeArt varchar(10)
GO 
if not exists (select * from syscolumns where name='NapomenaRac' and id=object_id('MPKASE'))
	alter table mpkase add NapomenaRac varchar(10)
GO 
if not exists (select * from syscolumns where name='Napomenadet' and id=object_id('MPKASE'))
	alter table mpkase add Napomenadet varchar(10)
GO 

if not exists (select * from syscolumns where name='maloprodajaglaid' and id=object_id('sktprometgla'))
	alter table sktprometgla add maloprodajaglaid int null 
GO 
if not exists (select * from syscolumns where name='ind_kontrole' and id=object_id('mpnargla'))
	alter table mpnargla add ind_kontrole int null 
GO 

if not exists (select * from syscolumns where name='Operatercitanjaid' and id=object_id('MPPOrukedet'))
	alter table MPPOrukedet add Operatercitanjaid int null 
GO 

if not exists (select * from syscolumns where name='Aktivno' and id=object_id('MPPosMenu'))
	alter table MPPosMenu add aktivno int not null default(1)
GO 

if not exists (select * from syscolumns where name='mpc' and id=object_id('MPVezanaroba'))
	alter table MPVezanaroba add mpc money not null default(0)
GO 
if not exists (select * from syscolumns where name='mpkasaloginid' and id=object_id('MPPosMenuperater'))
	alter table MPPosMenuperater add mpkasaloginid int null 
GO 

if not exists (select * from syscolumns where name='Sifra' and id=object_id('MPPosMenu'))
	alter table MPPosMenu add sifra varchar(10) null 
GO 

if not exists (select * from syscolumns where name='Granula' and id=object_id('MPPosMenu'))
	alter table MPPosMenu add Granula varchar(100) null 
GO 

if not exists (select * from syscolumns where name='Tipka' and id=object_id('MPPosMenu'))
	alter table MPPosMenu add tipka varchar(10) null 
GO 

if not exists (select * from syscolumns where name='brojkopijaotp' and id=object_id('mpkase'))
	alter table mpkase add brojkopijaotp int not null default(1)
GO 

if not exists (select * from syscolumns where name='brojkopijaracuna' and id=object_id('mpkase'))
	alter table mpkase add brojkopijaracuna int not null default(1)
GO 

if not exists (select * from syscolumns where name='IPKase' and id=object_id('mpkase'))
	alter table mpkase add IPKase varchar(20) null 
GO 
if not exists (select * from syscolumns where name='DatumZaSINC' and id=object_id('mpkase'))
	alter table mpkase add DatumZaSINC datetime null 
GO 
if not exists (select * from syscolumns where name='DatumSINC' and id=object_id('mpkase'))
	alter table mpkase add DatumSINC datetime null 
GO 

if not exists (select * from syscolumns where name='VPN' and id=object_id('mpkasainstall'))
	alter table mpkasainstall add VPN varchar(30) null
GO 

if not exists (select * from syscolumns where name='VPNUser' and id=object_id('mpkasainstall'))
	alter table mpkasainstall add VPNUser varchar(30) null
GO 
if not exists (select * from syscolumns where name='VPNPasw' and id=object_id('mpkasainstall'))
	alter table mpkasainstall add VPNPasw varchar(30) null
GO 

if not exists (select * from syscolumns where name='ponisti' and id=object_id('mpkase'))
	alter table mpkase add ponisti varchar(10) null 
GO 
if not exists (select * from syscolumns where name='Nefiskalizacija' and id=object_id('mpkase'))
	alter table mpkase add Nefiskalizacija int not null default(0)
GO 
if not exists (select * from syscolumns where name='IDnaKasi' and id=object_id('mpkasalogin'))
	alter table mpkasalogin add IDnaKasi int null 
GO 


if exists (select * from syscolumns where name='sort' and id=object_id('mpposmenu'))
	alter table mpposmenu alter column sort varchar(10) null 
GO 

if not exists (select * from syscolumns where name='RokTrajanjadani' and id=object_id('LNSklSKL'))
	alter table LNSklSKL add RokTrajanjadani int not null default(0)
GO 

if not exists (select * from syscolumns where name='lcd' and id=object_id('ZaglavljeRacuna'))
	alter table ZaglavljeRacuna add lcd char(40) not null default('')
GO 

if not exists (select * from syscolumns where name='WebService' and id=object_id('MPkasainstall'))
	alter table MPkasainstall add WebService varchar(500) null 
GO 
if not exists (select * from syscolumns where name='Datumdok' and id=object_id('mpracunbodovi'))
	alter table mpracunbodovi add Datumdok datetime  null 
GO 

if not exists (select * from syscolumns where name='fiskbrkase' and id=object_id('mpkase'))
	alter table mpkase add fiskbrkase varchar(5) 
GO 

if not exists (select * from syscolumns where name='DirUpdate' and id=object_id('mpkasainstall'))
	alter table mpkasainstall add DirUpdate varchar(50) 
GO 

if not exists (select * from syscolumns where name='broj' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add broj varchar(50) null 
GO 
if not exists (select * from syscolumns where name='brojdok' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add brojdok varchar(50) null 
GO 
if not exists (select * from syscolumns where name='prefix' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add prefix varchar(50) null 
GO 
if not exists (select * from syscolumns where name='sufix' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add sufix varchar(50) null
GO 
if not exists (select * from syscolumns where name='status' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add status int null 
GO 
if not exists (select * from syscolumns where name='operateriid' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add operateriid int null
GO 
if not exists (select * from syscolumns where name='vrstadokid' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add vrstadokid  int null 
GO 
if not exists (select * from syscolumns where name='MPPredlozakTipid' and id=object_id('ProNarudzbeGla'))
	alter table ProNarudzbeGla add MPPredlozakTipid  int null 
GO 

if not exists (select * from syscolumns where name='sklid' and id=object_id('maloprodajadet'))
	alter table maloprodajadet add sklid  int null 
GO 

if not exists (select * from syscolumns where name='bodovi' and id=object_id('mpkase'))
	alter table mpkase add bodovi  varchar(10) null 
GO 

if not exists (select * from syscolumns where name='sktprometglaid' and id=object_id('maloprodajagla'))
	alter table maloprodajagla add sktprometglaid  int null 
GO 

if not exists (select * from syscolumns where name='zabrana1' and id=object_id('mpnarpredlozak'))
	alter table mpnarpredlozak add zabrana1  int not null default(0) 
GO 
if not exists (select * from syscolumns where name='zabrana2' and id=object_id('mpnarpredlozak'))
	alter table mpnarpredlozak add zabrana2  int not null default(0) 
GO 
if not exists (select * from syscolumns where name='zabrana3' and id=object_id('mpnarpredlozak'))
	alter table mpnarpredlozak add zabrana3  int not null default(0) 
GO 

if not exists (select * from syscolumns where name='zabrana4' and id=object_id('mpnarpredlozak'))
	alter table mpnarpredlozak add zabrana4  int not null default(0) 
GO 

if not exists (select * from syscolumns where name='napomena2' and id=object_id('ProNarudzbeDet'))
	alter table ProNarudzbeDet add napomena2  varchar(100) null 
GO 

if not exists (select * from syscolumns where name='faktorprodaje' and id=object_id('roba'))
	alter table roba add faktorprodaje  money not null default(0)
GO 

if not exists (select * from syscolumns where name='maxkolicinaprodaje' and id=object_id('trgovina'))
	alter table trgovina add maxkolicinaprodaje  money not null default(0)
GO 



if not exists (select * from syscolumns where name='datumizmjeneRV' and id=object_id('trgovina'))
	alter table trgovina add datumizmjeneRV  datetime null 
GO 

if not exists (select * from syscolumns where name='mpcijena' and id=object_id('mpuvjetikasePrijenos'))
	alter table mpuvjetikasePrijenos add mpcijena  money not null default(0)
GO 
if not exists (select * from syscolumns where name='mpcijena' and id=object_id('mpuvjetikasePrijenos'))
	alter table mpuvjetikasePrijenos add mpcijena  money not null default(0)
GO 
if not exists (select * from syscolumns where name='mpcijena' and id=object_id('mpuvjetikase'))
	alter table mpuvjetikase add mpcijena  money not null default(0)
GO 

if not exists (select * from syscolumns where name='faktorprodaje' and id=object_id('mpstanjezakasu'))
	alter table mpstanjezakasu add faktorprodaje  money not null default(0)
GO 
if not exists (select * from syscolumns where name='indevidencije' and id=object_id('KntNacinPlac'))
	alter table KntNacinPlac add indevidencije  int  not null default(0)
GO 
if not exists (select * from syscolumns where name='ugo' and id=object_id('maloprodajagla'))
	alter table maloprodajagla add ugo  int  not null default(0)
GO 

if not exists (select * from syscolumns where name='storno' and id=object_id('mpkase'))
	alter table mpkase add storno  varchar(10) not null default('')
GO 

if not exists (select * from syscolumns where name='mpkaseid' and id=object_id('mpracunbodovi'))
	alter table mpracunbodovi add mpkaseid  int  null 
GO 
if not exists (select * from syscolumns where name='fiskalizacija' and id=object_id('mpkase'))
	alter table mpkase add fiskalizacija  varchar(10) not null default('')
GO 
if not exists (select * from syscolumns where name='printaj' and id=object_id('mpkase'))
	alter table mpkase add printaj  varchar(10) not null default('')
GO 
if not exists (select * from syscolumns where name='DjelatnikSifra' and id=object_id('Partneri'))
	alter table Partneri add DjelatnikSifra  varchar(15) not null default('')
GO 
if not exists (select * from syscolumns where name='Odsat' and id=object_id('MPRadnoVrijeme'))
	alter table MPRadnoVrijeme add Odsat  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='DoSat' and id=object_id('MPRadnoVrijeme'))
	alter table MPRadnoVrijeme add DoSat  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='OdMin' and id=object_id('MPRadnoVrijeme'))
	alter table MPRadnoVrijeme add OdMin  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='DoMin' and id=object_id('MPRadnoVrijeme'))
	alter table MPRadnoVrijeme add DoMin  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='Odsat' and id=object_id('lntregovinasmjene'))
	alter table lntregovinasmjene add Odsat  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='DoSat' and id=object_id('lntregovinasmjene'))
	alter table lntregovinasmjene add DoSat  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='OdMin' and id=object_id('lntregovinasmjene'))
	alter table lntregovinasmjene add OdMin  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='DoMin' and id=object_id('lntregovinasmjene'))
	alter table lntregovinasmjene add DoMin  varchar(4) not null default('')
GO 

if not exists (select * from syscolumns where name='mpbonoviid' and id=object_id('MPBonoviPromet'))
	alter table MPBonoviPromet add mpbonoviid  int null 
GO 
if not exists (select * from syscolumns where name='godina' and id=object_id('MPBonoviPromet'))
	alter table MPBonoviPromet add godina  int not null default(0) 
GO 
if not exists (select * from syscolumns where name='trgovinaid' and id=object_id('MPStolovi'))
	alter table MPStolovi add trgovinaid  int null  
GO 

if not exists (select * from syscolumns where name='PartnerObavezan' and id=object_id('MPKase'))
	alter table MPKase add PartnerObavezan  int not null default(0)  
GO 
if  exists (select * from syscolumns where name='operateriid' and id=object_id('MPPosMenuperater'))
	alter table MPPosMenuperater alter column operateriid  int null
GO 


if not exists (select * from syscolumns where name='dobavljac' and id=object_id('parasortiman'))
	alter table parasortiman add dobavljac  int null
GO 
if not exists (select * from syscolumns where name='inddobavljac' and id=object_id('parasortiman'))
	alter table parasortiman add inddobavljac varchar(10) null
GO 

if not exists (select * from syscolumns where name='partneriid' and id=object_id('NabIzddet'))
	alter table NabIzddet add partneriid  int null
GO 

if not exists (select * from syscolumns where name='skladisteid' and id=object_id('NabIzddet'))
	alter table NabIzddet add skladisteid  int null
GO 

if not exists (select * from syscolumns where name='mpkaseid' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add mpkaseid  int null
GO 
if not exists (select * from syscolumns where name='odsat' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add odsat  varchar(2) null 
GO 
if not exists (select * from syscolumns where name='dosat' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add dosat  varchar(2) null 
GO 
if not exists (select * from syscolumns where name='odmin' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add odmin  varchar(2) null 
GO 
if not exists (select * from syscolumns where name='domin' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add domin  varchar(2) null		
GO 
if not exists (select * from syscolumns where name='vrijemeprijenosa' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add vrijemeprijenosa  datetime null
GO 
if not exists (select * from syscolumns where name='indprijenos' and id=object_id('MPEvidencijaRada'))
	alter table MPEvidencijaRada add indprijenos  int not null default(0)
GO 

if not exists (select * from syscolumns where name='otpremnicasaMPC' and id=object_id('MPKase'))
	alter table MPKase add otpremnicasaMPC  int not null default(0)
GO 

if not exists (select * from syscolumns where name='tipro' and id=object_id('roba')) 
	alter table roba add tipro varchar(50) null 
GO
if not exists (select * from syscolumns where name='tipro' and id=object_id('mpstanjezakasu')) 
	alter table mpstanjezakasu add tipro varchar(50) null 
GO
if not exists (select * from syscolumns where name='bojaroba' and id=object_id('MPArtikliGrupe'))
	alter table MPArtikliGrupe add bojaroba  varchar(50) 
GO 
if not exists (select * from syscolumns where name='MPclanoviPrometid' and id=object_id('MPRacunBodovi'))
	alter table MPRacunBodovi add MPclanoviPrometid  int null 
GO 


if not exists (select * from syscolumns where name='defaultnaplata' and id=object_id('mpkase'))
	alter table mpkase add defaultnaplata int not null default (0) 
GO 

if not exists (select * from syscolumns where name='job2008' and id=object_id('mpkase'))
	alter table mpkase add job2008 int not null default (0) 
GO 
if not exists (select * from syscolumns where name='ind_stampe' and id=object_id('mpkase'))
	alter table mpkase add ind_stampe int not null default (0) 
GO 

if not exists (select * from syscolumns where name='indact' and id=object_id('mpkase'))
	alter table mpkase add indact int not null default (1) 
GO 
if not exists (select * from syscolumns where name='duzinaPPKartice' and id=object_id('mpkase'))
	alter table mpkase add duzinaPPKartice int not null default (0) 
GO 

-- update Install set webservice = 'http://www.spin.hr/ng/blagajnadxservice/'


--- AŽURIRANJE POS MENU-A




if (select mpposmenuid  from mpposmenu where oznaka = 'mnuProdaja') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuProdaja', 'Prodaja', '', 1, '0101', 'F1')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuZakljucak') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuZakljucak', 'Zakljuèak blagajne', 'asm:NetGenDll.frm:MPZakBlag.xml', 1, '0102', 'F2')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuRoba') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuRoba', 'Roba', 'asm:NetGenDll.loader:MPRoba.xml', 1, '0201', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPartneri') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPartneri', 'Partneri', 'asm:NetGenDll.loader:MPPartneri.xml', 1, '0202', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPregledracuna') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPregledracuna', 'Pregled raèuna', 'asm:NetGenDll.frm:MPPregracuna.xml', 1, '0301', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPregledzak') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPregledzak', 'Pregled zakljuèaka blagajne', 'asm:NetGenDll.frm:MPPregZakljucaka.xml', 1, '0302', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPregledotpremnica') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPregledotpremnica', 'Pregled otpremnica ', 'asm:NetGenDll.frm:MPPregOtpremnica.xml', 1, '0303', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuParametrikase') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuParametrikase', 'Parametri kase', 'asm:NetGenDll.frm:MPKasa.xml', 1, '0501', '')
end 

if (select mpposmenuid  from mpposmenu where oznaka = 'mnuKasanapocetak') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuKasanapocetak', 'Kasa na poèetak', 'asm:NetGenDll.frm:MPNaPocetak.xml', 1, '0502', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPripremapodataka') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPripremapodataka', 'Priprema podataka za centralu ', 'asm:NetGenDll.frm:MPPrijenosPodataka.xml', 1, '0402', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuDonosPodatakaizJupitera') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuDonosPodatakaizJupitera', 'Donos podataka iz Jupitera ', 'asm:NetGenDll.frm:MPDonosPodataka.xml', 1, '0401', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuBackup') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuBackup', 'Arhiviranje podataka', 'asm:NetGenDll.frm:MPBackup.xml', 1, '0403', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuBrojeviDokumenata') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuBrojeviDokumenata', 'Brojevi dokumenata', 'asm:NetGenDll.frm:MPAzurBrojDok.xml', 1, '0503', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuKasaVeza') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuKasaVeza', 'Kasa Veza', 'asm:NetGenDll.frm:MPKasaVeza.xml', 1, '0504', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuMPPregDok') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuMPPregDok', 'Pregled dokumenata MP', 'asm:NetGenDll.frm:MPPregDokMP.xml', 1, '0304', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuBodovi') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuBodovi', 'Vaucher', 'asm:NetGenDll.frm:MPVaucher.xml', 1, '', '0104')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPologBlagajne') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPologBlagajne', 'Polog blagajne', 'asm:NetGenDll.frm:MPPolog.xml', 1, '0103', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuKupciR1Izmjena') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuKupciR1Izmjena', 'Kupci R1', 'asm:NetGenDll.loader:MPKupciR1.xml', 1, '0203', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuSlanjeVaga') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuSlanjeVaga', 'Slanje podataka na vagu', 'asm:NetGenDll.frm:MPNapuniVagu.xml', 1, '0405', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPoruke') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPoruke', 'Obavijesti', 'asm:NetGenDll.loader:MPPoruke.xml', 1, '0104', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnublagajna') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnublagajna', 'BLAGAJNA', '', 1, '01', '')
end 

if (select mpposmenuid  from mpposmenu where oznaka = 'mnuOsnovno') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuOsnovno', 'OSNOVNI PODACI ', '', 1, '02', '')
end 

if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPregledi') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPregledi', 'PREGLEDI', '', 1, '03', '')
end 

if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPrijenos') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPrijenos', 'PRIJENOS PODATAKA', '', 1, '04', '')
end 

if (select mpposmenuid  from mpposmenu where oznaka = 'mnuodrzavanje') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuodrzavanje', 'ODRŽAVANJE', '', 1, '05', '')
end 
if (select mpposmenuid  from mpposmenu where oznaka = 'mnuPregProm') is null  begin 
	insert into mpposmenu (oznaka , opis, granula, aktivno , sifra, tipka )
	values('mnuPregProm', 'Pregled prodaje artikala', 'asm:NetGendll.loader:MPPregProm.xml', 1, '05', '')
end 


update MPKase set fiskbrkase = MPKaseId where fiskbrkase is null 





/*

-- kreiranje PRIMARY KEY -eva na tablicama 



IF EXISTS(SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#tableNoPK')) 
	drop table #tableNoPK

GO 

select distinct t.name 
into #tableNoPK
from sys.tables t
left outer join sys.key_constraints c on t.object_id = c.parent_object_id and t.schema_id = c.schema_id and c.type = 'PK'
where c.object_id is null
and t.name like 'mp%'
order by t.name

declare @name varchar(30), @sqlAddPK varchar(1000), @columnExists int, @sqlSetNull varchar(1000)
set @columnExists = 0

while (select COUNT(*) from #tableNoPK) > 0 begin
	
	set @name = (select top 1 name from #tableNoPK)
	
	IF EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
				WHERE TABLE_NAME = @name  
				AND  COLUMN_NAME = @name + 'id') 
					set @columnExists = 1 
				else  
					set @columnExists = 0 
	
	if @columnExists = 1 begin
		
		--if column is nullable change it in not null
		if (SELECT COLUMNPROPERTY(OBJECT_ID(@name, 'U'), @name +'Id', 'AllowsNull'))= 1 begin
			set @sqlSetNull = 'alter table ' + @name + ' alter column ' + @name + 'id int not null'
			exec (@sqlSetNull)
		end
		
		set @sqlAddPK = 'alter table ' + @name + ' add constraint PK_' + @name + ' primary key(' + @name + 'id)'
		exec (@sqlAddPK)
	end
	
	delete from #tableNoPK where name = @name
end



--select * from #tableNoPK
*/