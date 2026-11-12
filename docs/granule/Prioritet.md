# Prioritet granula iz ssMeni

Izvor: `spin.dbo.ssMeni`, samo čitanje, CI run [43229676282](https://github.com/tomic04/netgen-u-carpo/actions/runs/43229676282). Uvjet: `Parametar` počinje s `asm:NetGenDLL.loader:` ili `asm:NetGenDLL.frm:` (velika/mala slova se ne gledaju).

n=141 loader=132 frm=9. Jedna datoteka može imati više redova izbornika.

Rang: prvo lijevo stablo (`TREE` ili `COMBO` s vrijednošću) **i** `DETAILGRID`, zatim samo stablo, zatim broj različitih elemenata. Bod je broj elemenata plus 10 ako ima i stablo i detail.

Checklista znači vlastiti `docs/granule/<Ime>.md`. Pokrivene u ovom izborniku: DobUgovori, Narudzbe, PrijemniList, AOPNovi, MPInventura, MPBlagajna, KntBlaZak. Ostale postojeće checkliste (OznakaKL, JeziciNG, Kulture, KUPNG, …) nisu u ovom `ssMeni`.

Scorer traži `detailgrid="` bez razmaka. `KntBlaPromet` i `MPPonuda` imaju `DETAILGRID =` s razmakom, pa su u tablici bez detaila, a XML detail ima. Pune checkliste to uzimaju iz XML-a.

Top 5 još nepokrivenih: `LVObracun`, `JOPPD`, `KntKompVerizne`, `KntBlaPromet`, `MPPonuda`.

| # | Pokrivena | Stablo | Detail | Bod | Datoteka | Izbornik | Elementi |
|---:|:---:|:---:|:---:|---:|---|---|---|
| 1 | da | stablo | detail | 23 | `DobUgovori.xml` | `asm:NetGenDLL.loader:DobUgovori.xml` | stablo,detail,setdefaults,buttons,repx,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 2 | da | stablo | detail | 22 | `Narudzbe.xml` | `asm:NetGenDLL.loader:SKLAD\Narudzbe.xml` | stablo,detail,setdefaults,treetranslate,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 3 | da | stablo | detail | 22 | `PrijemniList.xml` | `asm:NetGenDLL.loader:PrijemniList.xml` | stablo,detail,setdefaults,repx,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 4 | ne | stablo | detail | 21 | `LVObracun.xml` | `asm:NetGenDll.Loader:LVObracun.xml` | stablo,detail,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog |
| 5 | da | stablo | detail | 20 | `AOPNovi.xml` | `asm:NetGenDLL.loader:AOPNovi.xml` | stablo,detail,setdefaults,buttons,foreach,number,check,grid-dijalog,tree-dijalog,nextform |
| 6 | ne | stablo | detail | 20 | `JOPPD.xml` | `asm:NetGenDll.Loader:JOPPD.xml` | stablo,detail,buttons,search,combo,date,number,check,grid-dijalog,tree-dijalog |
| 7 | ne | stablo | detail | 20 | `JOPPD.xml` | `asm:NetGenDll.Loader:JOPPD.xml` | stablo,detail,buttons,search,combo,date,number,check,grid-dijalog,tree-dijalog |
| 8 | ne | stablo | detail | 20 | `KntKompVerizne.xml` | `asm:NetGenDll.loader:KntKompVerizne.xml` | stablo,detail,setdefaults,buttons,repx,search,date,number,grid-dijalog,nextform |
| 9 | da | stablo | - | 13 | `MPInventura.xml` | `asm:NetGenDll.loader:MPInventura.xml` | stablo,setdefaults,multiselect,foreach,search,combo,date,number,check,radio,multidrop,grid-dijalog,nextform |
| 10 | da | stablo | - | 12 | `MPBlagajna.xml` | `asm:NetGenDll.loader:MPBlagajna.xml` | stablo,setdefaults,buttons,print,repx,assemblyload,multiselect,combo,date,number,grid-dijalog,nextform |
| 11 | da | stablo | - | 12 | `MPBlagajna.xml` | `asm:NetGenDll.loader:MPBlagajna.xml` | stablo,setdefaults,buttons,print,repx,assemblyload,multiselect,combo,date,number,grid-dijalog,nextform |
| 12 | ne | stablo | - | 11 | `KntBlaPromet.xml` | `asm:NetGenDLL.loader:KntBlaPromet.xml` | stablo,setdefaults,buttons,repx,search,combo,date,number,check,grid-dijalog,nextform |
| 13 | ne | stablo | - | 11 | `MPPonuda.xml` | `asm:NetGenDll.loader:MPPonuda.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 14 | ne | stablo | - | 10 | `MPInvRazlike.xml` | `asm:NetGenDll.loader:MPInvRazlike.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 15 | ne | stablo | - | 10 | `MPIzdVPNG.xml` | `asm:NetGenDll.loader:MPIzdVPNG.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 16 | ne | stablo | - | 10 | `MPNarFFirma.xml` | `asm:NetGenDll.loader:MPNarFFirma.xml` | stablo,foreach,search,combo,date,number,check,multidrop,grid-dijalog,nextform |
| 17 | ne | stablo | - | 10 | `MPPSNG.xml` | `asm:NetGenDll.loader:MPPSNG.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 18 | ne | stablo | - | 10 | `PNObracun.xml` | `asm:NetGenDll.Loader:PNObracun.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog |
| 19 | ne | stablo | - | 9 | `PNNalozi.xml` | `asm:NetGenDll.Loader:PNNalozi.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,grid-dijalog |
| 20 | ne | stablo | - | 8 | `KntBlaDok.xml` | `asm:NetGenDLL.loader:KntBlaDok.xml` | stablo,setdefaults,buttons,search,combo,date,check,nextform |
| 21 | ne | stablo | - | 7 | `Akcije.xml` | `asm:NetGenDll.Loader:Akcije.xml` | stablo,buttons,search,combo,number,check,grid-dijalog |
| 22 | ne | stablo | - | 7 | `ManagerObaveza.xml` | `asm:NetGenDll.loader:MER\ManagerObaveza.xml` | stablo,print,search,combo,date,number,radio |
| 23 | ne | stablo | - | 7 | `MPPregUtrzakNG.xml` | `asm:NetGenDll.loader:MPPregUtrzakNG.xml` | stablo,setdefaults,onload-forma,combo,date,grid-dijalog,nextform |
| 24 | ne | stablo | - | 7 | `PNKatalozi.xml` | `asm:NetGenDll.Loader:PNKatalozi.xml` | stablo,search,combo,date,number,check,grid-dijalog |
| 25 | ne | stablo | - | 6 | `KOD_RSPostavke.xml` | `asm:NetGenDLL.Loader:KOD_RSPostavke.xml` | stablo,buttons,combo,number,check,grid-dijalog |
| 26 | da | stablo | - | 5 | `KntBlaZak.xml` | `asm:NetGenDLL.loader:KntBlaZak.xml` | stablo,setdefaults,buttons,repx,nextform |
| 27 | ne | stablo | - | 5 | `MPNivelacija.xml` | `asm:NetGenDll.loader:MPNivelacija.xml` | stablo,setdefaults,buttons,combo,date |
| 28 | ne | stablo | - | 4 | `LVKatalozi.xml` | `asm:NetGenDll.Loader:LVKatalozi.xml` | stablo,date,number,grid-dijalog |
| 29 | ne | stablo | - | 3 | `kntShemaKonta.xml` | `asm:NetGenDll.loader:kntShemaKonta.xml` | stablo,buttons,print |
| 30 | ne | stablo | - | 1 | `OdobreniKaloNG.xml` | `asm:NetGenDLL.frm:OdobreniKaloNG.xml` | stablo |
| 31 | ne | - | detail | 10 | `Intrastat.xml` | `asm:NetGenDll.loader:MER\Intrastat.xml` | detail,setdefaults,repx,search,combo,number,check,radio,grid-dijalog,nextform |
| 32 | ne | - | - | 10 | `KntPDVObrasciEU.xml` | `asm:NetGenDLL.Loader:KntPDVObrasciEU.xml` | buttons,onload-forma,print,repx,assemblyload,combo,date,number,check,grid-dijalog |
| 33 | ne | - | - | 9 | `MPKntNacinPlacAzur.xml` | `asm:NetGendll.loader:MPKntNacinPlacAzur.xml` | setdefaults,buttons,onload-forma,search,combo,date,number,grid-dijalog,nextform |
| 34 | ne | - | - | 9 | `RealizacijaStavkeNG.xml` | `asm:NetGenDLL.Loader:MER\RealizacijaStavkeNG.xml` | setdefaults,onload-forma,print,search,combo,date,check,multidrop,clipboard |
| 35 | ne | - | - | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 36 | ne | - | - | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 37 | ne | - | - | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 38 | ne | - | - | 8 | `KntOPZ-STAT-1.xml` | `asm:NetGenDll.Loader:KntOPZ-STAT-1.xml` | onload-forma,repx,combo,date,number,check,multidrop,clipboard |
| 39 | ne | - | - | 8 | `MPRealizNG.xml` | `asm:NetGenDLL.Loader:MPRealizNG.xml` | setdefaults,onload-forma,search,combo,date,check,grid-dijalog,nextform |
| 40 | ne | - | - | 8 | `MPstanje.xml` | `asm:NetGenDll.frm:MPstanje.xml` | setdefaults,buttons,search,combo,date,number,check,grid-dijalog |
| 41 | ne | - | - | 8 | `MPTrgOprNG.xml` | `asm:NetGenDll.loader:MPTrgOprNG.xml` | setdefaults,buttons,onload-forma,combo,number,check,grid-dijalog,nextform |
| 42 | ne | - | - | 8 | `RealizacijaFakture.xml` | `asm:NetGenDLL.Loader:MER\RealizacijaFakture.xml` | setdefaults,onload-forma,print,search,combo,date,check,clipboard |
| 43 | ne | - | - | 7 | `KntPreknjizavanjeNG.xml` | `asm:NetGenDll.Loader:KntPreknjizavanjeNG.xml` | buttons,onload-forma,print,multiselect,search,combo,date |
| 44 | ne | - | - | 7 | `MPRACNEFISK.xml` | `asm:NetGendll.loader:MPRACNEFISK.xml` | setdefaults,onload-forma,combo,date,check,grid-dijalog,nextform |
| 45 | ne | - | - | 6 | `kntDobDospRac.xml` | `asm:NetGenDll.loader:kntDobDospRac.xml` | setdefaults,onload-forma,print,search,date,nextform |
| 46 | ne | - | - | 6 | `kntDobDospRac.xml` | `asm:NetGenDLL.loader:kntDobDospRac.xml` | setdefaults,onload-forma,print,search,date,nextform |
| 47 | ne | - | - | 6 | `kntPreknjizavanjePartnera.xml` | `asm:NetGenDll.loader:kntPreknjizavanjePartnera.xml` | onload-forma,print,multiselect,search,combo,date |
| 48 | ne | - | - | 6 | `MPFiskalPubVars.xml` | `asm:NetGenDll.loader:MPFiskalPubVars.xml` | setdefaults,buttons,onload-forma,combo,date,number |
| 49 | ne | - | - | 6 | `MPKase.xml` | `asm:NetGenDll.loader:MPKase.xml` | setdefaults,foreach,combo,number,check,grid-dijalog |
| 50 | ne | - | - | 6 | `Predujmovi.xml` | `asm:NetGenDll.loader:Predujmovi.xml` | buttons,onload-forma,print,multiselect,date,nextform |
| 51 | ne | - | - | 5 | `Bilanca-GFI.xml` | `asm:NetGenDll.loader:Bilanca-GFI.xml` | onload-forma,print,assemblyload,combo,date |
| 52 | ne | - | - | 5 | `MPKnjigaPopisa.xml` | `asm:NetGenDll.loader:MPKnjigaPopisa.xml` | setdefaults,buttons,combo,date,grid-dijalog |
| 53 | ne | - | - | 5 | `MPPDV.xml` | `asm:NetGenDLL.Loader:UTP\MPPDV.xml` | onload-forma,search,combo,date,grid-dijalog |
| 54 | ne | - | - | 5 | `PartneriAuditNG.xml` | `asm:NetGendll.loader:PartneriAuditNG.xml` | onload-forma,search,combo,check,grid-dijalog |
| 55 | ne | - | - | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 56 | ne | - | - | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 57 | ne | - | - | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 58 | ne | - | - | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 59 | ne | - | - | 4 | `KntR2racuni1.xml` | `asm:NetGenDll.loader:KntR2racuni1.xml` | onload-forma,print,date,check |
| 60 | ne | - | - | 4 | `PDVpoStjecanju.xml` | `asm:NetGenDLL.Loader:PDVpoStjecanju.xml` | onload-forma,print,date,nextform |
| 61 | ne | - | - | 4 | `vrcjenik.xml` | `asm:NetGenDLL.loader:MER\vrcjenik.xml` | combo,date,check,multidrop |
| 62 | ne | - | - | 3 | `BisnodeExport.xml` | `asm:NetGenDll.loader:BisnodeExport.xml` | onload-forma,combo,date |
| 63 | ne | - | - | 3 | `HROsobeUcinak.xml` | `asm:NetGenDLL.Loader:HROsobeUcinak.xml` | setdefaults,combo,nextform |
| 64 | ne | - | - | 3 | `InstrumentiPlacanjaNG.xml` | `asm:NetGenDLL.Loader:MER\InstrumentiPlacanjaNG.xml` | setdefaults,print,check |
| 65 | ne | - | - | 3 | `KUPStatN.xml` | `asm:NetGenDLL.loader:KUPStatN.xml` | foreach,combo,grid-dijalog |
| 66 | ne | - | - | 3 | `MatTem.xml` | `asm:NetGenDll.loader:MatTem.xml` | onload-forma,print,combo |
| 67 | ne | - | - | 3 | `ServisPrava.xml` | `asm:NetGendll.frm:ServisPrava.xml` | onload-forma,combo,grid-dijalog |
| 68 | ne | - | - | 3 | `ZakljucivanjeTemNG.xml` | `asm:NetGenDll.Loader:ZakljucivanjeTemNG.xml` | onload-forma,combo,date |
| 69 | ne | - | - | 2 | `FaktureMinMaxDat.xml` | `asm:NetGenDLL.frm:FaktureMinMaxDat.xml` | onload-forma,date |
| 70 | ne | - | - | 2 | `KntKonfShemeMatTem.xml` | `asm:NetGenDll.loader:KntKonfShemeMatTem.xml` | search,combo |
| 71 | ne | - | - | 2 | `KUPVrstPN.xml` | `asm:NetGenDLL.loader:KUPVrstPN.xml` | combo,nextform |
| 72 | ne | - | - | 2 | `MPKaseOperNG.xml` | `asm:NetGenDll.loader:MPKaseOperNG.xml` | setdefaults,combo |
| 73 | ne | - | - | 2 | `SklMinMaxDat.xml` | `asm:NetGenDLL.frm:SklMinMaxDat.xml` | onload-forma,date |
| 74 | ne | - | - | 0 | `admini.xml` | `asm:NetgenDll.loader:admini.xml` |  |
| 75 | ne | - | - | 0 | `admini.xml` | `asm:NetgenDll.loader:admini.xml` |  |
| 76 | ne | - | - | 0 | `Connexobustave.xml` | `asm:netgendll.loader:Connexobustave.xml` |  |
| 77 | ne | - | - | 0 | `CoreKompLog.xml` | `asm:NetGenDLL.loader:CoreKompLog.xml` |  |
| 78 | ne | - | - | 0 | `isokategorija.xml` | `asm:NetgenDLL.Loader:isokategorija.xml` |  |
| 79 | ne | - | - | 0 | `IspisiFirme.xml` | `asm:NetGenDll.loader:IspisiFirme.xml` |  |
| 80 | ne | - | - | 0 | `IzvKlasePartneriParstru.xml` | `asm:NetGenDLL.loader:IzvKlasePartneriParstru.xml` |  |
| 81 | ne | - | - | 0 | `jesteliznali.xml` | `asm:NetGenDll.Loader:jesteliznali.xml` |  |
| 82 | ne | - | - | 0 | `kategorija.xml` | `asm:NetgenDLL.Loader:kategorija.xml` |  |
| 83 | ne | - | - | 0 | `klasetecajevi.xml` | `asm:NetgenDll.Loader:klasetecajevi.xml` |  |
| 84 | ne | - | - | 0 | `klasetecajevi.xml` | `asm:NetgenDll.Loader:klasetecajevi.xml` |  |
| 85 | ne | - | - | 0 | `KntBlaVrste.xml` | `asm:NetGenDLL.loader:KntBlaVrste.xml` |  |
| 86 | ne | - | - | 0 | `KntIOSObrazacDev.xml` | `asm:NetGenDLL.frm:KntIOSObrazacDev.xml` |  |
| 87 | ne | - | - | 0 | `KntIzvodTijek.xml` | `asm:NetGenDll.loader:KntIzvodTijek.xml` |  |
| 88 | ne | - | - | 0 | `KntOpomene.xml` | `asm:NetGenDLL.frm:KntOpomene.xml` |  |
| 89 | ne | - | - | 0 | `KntPriGen.xml` | `asm:NetGenDll.loader:KntPriGen.xml` |  |
| 90 | ne | - | - | 0 | `KntRasporedTros.xml` | `asm:NetGenDll.loader:KntRasporedTros.xml` |  |
| 91 | ne | - | - | 0 | `KntZatvaranjeNG.xml` | `asm:NetGenDLL.Loader:KntZatvaranjeNG.xml` |  |
| 92 | ne | - | - | 0 | `KonfigSrvRN.xml` | `asm:NetGendll.loader:KonfigSrvRN.xml` |  |
| 93 | ne | - | - | 0 | `KUPDostKnjigaTEST.xml` | `asm:NetGenDLL.loader:KUPDostKnjigaTEST.xml` |  |
| 94 | ne | - | - | 0 | `KUPDostKNOdobravanje.xml` | `asm:NetGenDLL.loader:KUPDostKNOdobravanje.xml` |  |
| 95 | ne | - | - | 0 | `KUPNGSpin.xml` | `asm:NetGenDLL.loader:KUPNGSpin.xml` |  |
| 96 | ne | - | - | 0 | `KUPNGSpin_KUP_URA.xml` | `asm:NetGenDLL.loader:KUPNGSpin_KUP_URA.xml` |  |
| 97 | ne | - | - | 0 | `KUPNGTest.xml` | `asm:NetGenDLL.loader:KUPNGTest.xml` |  |
| 98 | ne | - | - | 0 | `KUPNG_ISOOdobravanje.xml` | `asm:NetGenDLL.loader:KUPNG_ISOOdobravanje.xml` |  |
| 99 | ne | - | - | 0 | `ManGadTip.xml` | `asm:NetGenDLL.loader:ManGadTip.xml` |  |
| 100 | ne | - | - | 0 | `ManPokazateljiGrupe.xml` | `asm:NetGenDLL.Loader:ManPokazateljiGrupe.xml` |  |
| 101 | ne | - | - | 0 | `MENIPubVars.xml` | `asm:NetGenDll.loader:MENIPubVars.xml` |  |
| 102 | ne | - | - | 0 | `MPAppManager.xml` | `asm:netgendll.loader:MPAppManager.xml` |  |
| 103 | ne | - | - | 0 | `MPRobnoDOk.xml` | `asm:NetGenDll.loader:MPRobnoDOk.xml` |  |
| 104 | ne | - | - | 0 | `MPTrgovine.xml` | `asm:NetGenDll.loader:MPTrgovine.xml` |  |
| 105 | ne | - | - | 0 | `NacinObracunaZadatka.xml` | `asm:NetgenDll.loader:NacinObracunaZadatka.xml` |  |
| 106 | ne | - | - | 0 | `newswebsite.xml` | `asm:NetgenDLL.loader:newswebsite.xml` |  |
| 107 | ne | - | - | 0 | `OrgShemaKonf.xml` | `asm:NetGendll.loader:OrgShemaKonf.xml` |  |
| 108 | ne | - | - | 0 | `OsobeOperateri.xml` | `asm:NetgenDLL.loader:OsobeOperateri.xml` |  |
| 109 | ne | - | - | 0 | `PDVPromjena.xml` | `asm:NetGenDll.loader:PDVPromjena.xml` |  |
| 110 | ne | - | - | 0 | `PDVPromjena.xml` | `asm:NetGenDll.loader:PDVPromjena.xml` |  |
| 111 | ne | - | - | 0 | `PNNaloziMoji.xml` | `asm:NetGenDll.Loader:PNNaloziMoji.xml` |  |
| 112 | ne | - | - | 0 | `Poruke.xml` | `asm:NetGenDLL.loader:Poruke.xml` |  |
| 113 | ne | - | - | 0 | `Poruke.xml` | `asm:NetGenDLL.loader:Poruke.xml` |  |
| 114 | ne | - | - | 0 | `PorukeDoc.xml` | `asm:NetGenDLL.loader:PorukeDoc.xml` |  |
| 115 | ne | - | - | 0 | `PovijestCjenika.xml` | `asm:NetGenDLL.loader:PovijestCjenika.xml` |  |
| 116 | ne | - | - | 0 | `PregledKnj.xml` | `asm:NetGenDll.loader:PregledKnj.xml` |  |
| 117 | ne | - | - | 0 | `PregledOperateraNaDok.xml` | `asm:NetGenDLL.Loader:MER\PregledOperateraNaDok.xml` |  |
| 118 | ne | - | - | 0 | `prioritet.xml` | `asm:NetgenDll.loader:prioritet.xml` |  |
| 119 | ne | - | - | 0 | `proces.xml` | `asm:NetgenDll.loader:proces.xml` |  |
| 120 | ne | - | - | 0 | `Protokoli.xml` | `asm:NetGenDLL.loader:Protokoli.xml` |  |
| 121 | ne | - | - | 0 | `ProtokolKatalog.xml` | `asm:NetgenDLL.frm:ProtokolKatalog.xml` |  |
| 122 | ne | - | - | 0 | `PTOsobeCjenik.xml` | `asm:NetgenDll.loader:PTOsobeCjenik.xml` |  |
| 123 | ne | - | - | 0 | `ptutrvrstarada.xml` | `asm:Netgendll.loader:ptutrvrstarada.xml` |  |
| 124 | ne | - | - | 0 | `PTVrstaDok.xml` | `asm:NetgenDll.loader:PTVrstaDok.xml` |  |
| 125 | ne | - | - | 0 | `PubVarsNG.xml` | `asm:NetGenDll.loader:PubVarsNG.xml` |  |
| 126 | ne | - | - | 0 | `RazlikeUvjetaDokUgovor.xml` | `asm:NetGenDll.loader:RazlikeUvjetaDokUgovor.xml` |  |
| 127 | ne | - | - | 0 | `ServisJamstvo3.xml` | `asm:NetGenDll.loader:ServisJamstvo3.xml` |  |
| 128 | ne | - | - | 0 | `SktSerBrojAzur.xml` | `asm:NetGendll.loader:SktSerBrojAzur.xml` |  |
| 129 | ne | - | - | 0 | `Spojka.xml` | `asm:NetGenDLL.Loader:Spojka.xml` |  |
| 130 | ne | - | - | 0 | `statusadmin.xml` | `asm:NetgenDll.loader:statusadmin.xml` |  |
| 131 | ne | - | - | 0 | `statusadmin.xml` | `asm:NetgenDll.loader:statusadmin.xml` |  |
| 132 | ne | - | - | 0 | `StatusProjekta.xml` | `asm:NetgenDll.loader:StatusProjekta.xml` |  |
| 133 | ne | - | - | 0 | `statusZadatka.xml` | `asm:NetgenDLL.loader:statusZadatka.xml` |  |
| 134 | ne | - | - | 0 | `TDObracun.xml` | `asm:NetGenDll.Loader:TDObracun.xml` |  |
| 135 | ne | - | - | 0 | `Tecajevi.xml` | `asm:NetGenDll.loader:Tecajevi.xml` |  |
| 136 | ne | - | - | 0 | `TecajnaListaNG.xml` | `asm:NetGenDll.loader:TecajnaListaNG.xml` |  |
| 137 | ne | - | - | 0 | `TecRazOTV.xml` | `asm:NetGenDll.loader:TecRazOTV.xml` |  |
| 138 | ne | - | - | 0 | `Upitnici.xml` | `asm:NetGenDll.frm:MAR\Upitnici.xml` |  |
| 139 | ne | - | - | 0 | `VoziloFinRekapNG.xml` | `asm:netgendll.loader:VoziloFinRekapNG.xml` |  |
| 140 | ne | - | - | 0 | `VrstaProjekt.xml` | `asm:NetgenDll.loader:VrstaProjekt.xml` |  |
| 141 | ne | - | - | 0 | `wwwkomp.xml` | `asm:NetGenDll.loader:wwwkomp.xml` |  |
