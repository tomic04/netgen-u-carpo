# Prioritet granula iz ssMeni

Izvor izbornika na spinu: CI run [43229676282](https://github.com/tomic04/netgen-u-carpo/actions/runs/43229676282). Broj baza: CI run [43233316047](https://github.com/tomic04/netgen-u-carpo/actions/runs/43233316047), presjek u `ssMeni-svi-serveri.md`. Uvjet na spinu: `Parametar` počinje s `asm:NetGenDLL.loader:` ili `asm:NetGenDLL.frm:`.

n=141 loader=132 frm=9. Jedna datoteka može imati više redova izbornika. Broj baza broji različite baze u tom presjeku (Biovega, CentarTehnike na oba servera, Panissimo, Setcor na oba, Tusak, Enna, Ireks, spin). `fidus` nema `ssMeni`.

Rang: prvo lijevo stablo i `DETAILGRID`, zatim veći broj baza, zatim samo stablo, zatim detail, zatim bod. Bod je broj elemenata plus 10 ako ima i stablo i detail. Granula u više baza ima veći prioritet unutar istog rasporeda.

Checklista znači `docs/granule/<Ime>.md`. U ovom izborniku pokrivene su DobUgovori, Narudzbe, PrijemniList, AOPNovi, MPInventura, MPBlagajna, KntBlaZak, LVObracun, JOPPD, KntKompVerizne, KntBlaPromet, MPPonuda.

Scorer traži `detailgrid="` bez razmaka. `KntBlaPromet` i `MPPonuda` imaju `DETAILGRID =` s razmakom, pa su u tablici bez detaila, a XML detail ima.

Sljedeće nepokriveno po ovom rangu, sve na 10 baza i na Biovegi: `Intrastat`, `KntPDVObrasciEU`, `MPKntNacinPlacAzur`, `KntOPZ-STAT-1`, `MPRACNEFISK`.

| # | Pokrivena | Stablo | Detail | Baze | Biovega | Bod | Datoteka | Izbornik | Elementi |
|---:|:---:|:---:|:---:|---:|:---:|---:|---|---|---|
| 1 | da | stablo | detail | 10 | **Biovega** | 22 | `PrijemniList.xml` | `asm:NetGenDLL.loader:PrijemniList.xml` | stablo,detail,setdefaults,repx,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 2 | da | stablo | detail | 8 | **Biovega** | 20 | `AOPNovi.xml` | `asm:NetGenDLL.loader:AOPNovi.xml` | stablo,detail,setdefaults,buttons,foreach,number,check,grid-dijalog,tree-dijalog,nextform |
| 3 | da | stablo | detail | 7 | **Biovega** | 22 | `Narudzbe.xml` | `asm:NetGenDLL.loader:SKLAD\Narudzbe.xml` | stablo,detail,setdefaults,treetranslate,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 4 | da | stablo | detail | 7 |  | 20 | `KntKompVerizne.xml` | `asm:NetGenDll.loader:KntKompVerizne.xml` | stablo,detail,setdefaults,buttons,repx,search,date,number,grid-dijalog,nextform |
| 5 | da | stablo | detail | 5 | **Biovega** | 23 | `DobUgovori.xml` | `asm:NetGenDLL.loader:DobUgovori.xml` | stablo,detail,setdefaults,buttons,repx,search,combo,date,number,check,radio,grid-dijalog,nextform |
| 6 | da | stablo | detail | 3 |  | 20 | `JOPPD.xml` | `asm:NetGenDll.Loader:JOPPD.xml` | stablo,detail,buttons,search,combo,date,number,check,grid-dijalog,tree-dijalog |
| 7 | da | stablo | detail | 3 |  | 20 | `JOPPD.xml` | `asm:NetGenDll.Loader:JOPPD.xml` | stablo,detail,buttons,search,combo,date,number,check,grid-dijalog,tree-dijalog |
| 8 | da | stablo | detail | 2 | **Biovega** | 21 | `LVObracun.xml` | `asm:NetGenDll.Loader:LVObracun.xml` | stablo,detail,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog |
| 9 | ne | - | detail | 10 | **Biovega** | 10 | `Intrastat.xml` | `asm:NetGenDll.loader:MER\Intrastat.xml` | detail,setdefaults,repx,search,combo,number,check,radio,grid-dijalog,nextform |
| 10 | ne | - | - | 10 | **Biovega** | 10 | `KntPDVObrasciEU.xml` | `asm:NetGenDLL.Loader:KntPDVObrasciEU.xml` | buttons,onload-forma,print,repx,assemblyload,combo,date,number,check,grid-dijalog |
| 11 | ne | - | - | 10 | **Biovega** | 9 | `MPKntNacinPlacAzur.xml` | `asm:NetGendll.loader:MPKntNacinPlacAzur.xml` | setdefaults,buttons,onload-forma,search,combo,date,number,grid-dijalog,nextform |
| 12 | ne | - | - | 10 | **Biovega** | 8 | `KntOPZ-STAT-1.xml` | `asm:NetGenDll.Loader:KntOPZ-STAT-1.xml` | onload-forma,repx,combo,date,number,check,multidrop,clipboard |
| 13 | ne | - | - | 10 | **Biovega** | 7 | `MPRACNEFISK.xml` | `asm:NetGendll.loader:MPRACNEFISK.xml` | setdefaults,onload-forma,combo,date,check,grid-dijalog,nextform |
| 14 | ne | - | - | 10 | **Biovega** | 6 | `MPFiskalPubVars.xml` | `asm:NetGenDll.loader:MPFiskalPubVars.xml` | setdefaults,buttons,onload-forma,combo,date,number |
| 15 | ne | - | - | 10 | **Biovega** | 5 | `Bilanca-GFI.xml` | `asm:NetGenDll.loader:Bilanca-GFI.xml` | onload-forma,print,assemblyload,combo,date |
| 16 | ne | - | - | 10 | **Biovega** | 5 | `MPPDV.xml` | `asm:NetGenDLL.Loader:UTP\MPPDV.xml` | onload-forma,search,combo,date,grid-dijalog |
| 17 | ne | - | - | 10 | **Biovega** | 4 | `KntR2racuni1.xml` | `asm:NetGenDll.loader:KntR2racuni1.xml` | onload-forma,print,date,check |
| 18 | ne | - | - | 10 | **Biovega** | 0 | `TecRazOTV.xml` | `asm:NetGenDll.loader:TecRazOTV.xml` |  |
| 19 | ne | - | - | 9 |  | 9 | `RealizacijaStavkeNG.xml` | `asm:NetGenDLL.Loader:MER\RealizacijaStavkeNG.xml` | setdefaults,onload-forma,print,search,combo,date,check,multidrop,clipboard |
| 20 | ne | - | - | 9 | **Biovega** | 2 | `MPKaseOperNG.xml` | `asm:NetGenDll.loader:MPKaseOperNG.xml` | setdefaults,combo |
| 21 | ne | - | - | 9 | **Biovega** | 0 | `Poruke.xml` | `asm:NetGenDLL.loader:Poruke.xml` |  |
| 22 | ne | - | - | 9 | **Biovega** | 0 | `Poruke.xml` | `asm:NetGenDLL.loader:Poruke.xml` |  |
| 23 | ne | stablo | - | 8 | **Biovega** | 6 | `KOD_RSPostavke.xml` | `asm:NetGenDLL.Loader:KOD_RSPostavke.xml` | stablo,buttons,combo,number,check,grid-dijalog |
| 24 | ne | - | - | 8 |  | 8 | `RealizacijaFakture.xml` | `asm:NetGenDLL.Loader:MER\RealizacijaFakture.xml` | setdefaults,onload-forma,print,search,combo,date,check,clipboard |
| 25 | ne | - | - | 8 | **Biovega** | 5 | `MPKnjigaPopisa.xml` | `asm:NetGenDll.loader:MPKnjigaPopisa.xml` | setdefaults,buttons,combo,date,grid-dijalog |
| 26 | ne | - | - | 8 | **Biovega** | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 27 | ne | - | - | 8 | **Biovega** | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 28 | ne | - | - | 8 | **Biovega** | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 29 | ne | - | - | 8 | **Biovega** | 5 | `SkladisteNG.xml` | `asm:NetGenDLL.Loader:SkladisteNG.xml` | buttons,print,search,combo,check |
| 30 | ne | - | - | 8 | **Biovega** | 4 | `vrcjenik.xml` | `asm:NetGenDLL.loader:MER\vrcjenik.xml` | combo,date,check,multidrop |
| 31 | ne | - | - | 8 | **Biovega** | 3 | `InstrumentiPlacanjaNG.xml` | `asm:NetGenDLL.Loader:MER\InstrumentiPlacanjaNG.xml` | setdefaults,print,check |
| 32 | ne | - | - | 8 | **Biovega** | 3 | `MatTem.xml` | `asm:NetGenDll.loader:MatTem.xml` | onload-forma,print,combo |
| 33 | da | stablo | - | 7 | **Biovega** | 13 | `MPInventura.xml` | `asm:NetGenDll.loader:MPInventura.xml` | stablo,setdefaults,multiselect,foreach,search,combo,date,number,check,radio,multidrop,grid-dijalog,nextform |
| 34 | ne | stablo | - | 7 | **Biovega** | 10 | `MPPSNG.xml` | `asm:NetGenDll.loader:MPPSNG.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 35 | ne | - | - | 7 | **Biovega** | 8 | `MPstanje.xml` | `asm:NetGenDll.frm:MPstanje.xml` | setdefaults,buttons,search,combo,date,number,check,grid-dijalog |
| 36 | ne | - | - | 7 |  | 8 | `MPTrgOprNG.xml` | `asm:NetGenDll.loader:MPTrgOprNG.xml` | setdefaults,buttons,onload-forma,combo,number,check,grid-dijalog,nextform |
| 37 | ne | - | - | 7 | **Biovega** | 6 | `MPKase.xml` | `asm:NetGenDll.loader:MPKase.xml` | setdefaults,foreach,combo,number,check,grid-dijalog |
| 38 | ne | - | - | 7 |  | 2 | `FaktureMinMaxDat.xml` | `asm:NetGenDLL.frm:FaktureMinMaxDat.xml` | onload-forma,date |
| 39 | ne | - | - | 7 | **Biovega** | 2 | `KntKonfShemeMatTem.xml` | `asm:NetGenDll.loader:KntKonfShemeMatTem.xml` | search,combo |
| 40 | ne | - | - | 7 |  | 2 | `SklMinMaxDat.xml` | `asm:NetGenDLL.frm:SklMinMaxDat.xml` | onload-forma,date |
| 41 | ne | - | - | 7 | **Biovega** | 0 | `PorukeDoc.xml` | `asm:NetGenDLL.loader:PorukeDoc.xml` |  |
| 42 | da | stablo | - | 6 | **Biovega** | 11 | `KntBlaPromet.xml` | `asm:NetGenDLL.loader:KntBlaPromet.xml` | stablo,setdefaults,buttons,repx,search,combo,date,number,check,grid-dijalog,nextform |
| 43 | ne | stablo | - | 6 | **Biovega** | 10 | `PNObracun.xml` | `asm:NetGenDll.Loader:PNObracun.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog |
| 44 | ne | stablo | - | 6 | **Biovega** | 8 | `KntBlaDok.xml` | `asm:NetGenDLL.loader:KntBlaDok.xml` | stablo,setdefaults,buttons,search,combo,date,check,nextform |
| 45 | ne | stablo | - | 6 |  | 7 | `Akcije.xml` | `asm:NetGenDll.Loader:Akcije.xml` | stablo,buttons,search,combo,number,check,grid-dijalog |
| 46 | ne | stablo | - | 6 | **Biovega** | 7 | `ManagerObaveza.xml` | `asm:NetGenDll.loader:MER\ManagerObaveza.xml` | stablo,print,search,combo,date,number,radio |
| 47 | ne | stablo | - | 6 | **Biovega** | 7 | `PNKatalozi.xml` | `asm:NetGenDll.Loader:PNKatalozi.xml` | stablo,search,combo,date,number,check,grid-dijalog |
| 48 | da | stablo | - | 6 | **Biovega** | 5 | `KntBlaZak.xml` | `asm:NetGenDLL.loader:KntBlaZak.xml` | stablo,setdefaults,buttons,repx,nextform |
| 49 | ne | - | - | 6 |  | 8 | `MPRealizNG.xml` | `asm:NetGenDLL.Loader:MPRealizNG.xml` | setdefaults,onload-forma,search,combo,date,check,grid-dijalog,nextform |
| 50 | ne | - | - | 6 | **Biovega** | 5 | `PartneriAuditNG.xml` | `asm:NetGendll.loader:PartneriAuditNG.xml` | onload-forma,search,combo,check,grid-dijalog |
| 51 | ne | - | - | 6 |  | 3 | `ZakljucivanjeTemNG.xml` | `asm:NetGenDll.Loader:ZakljucivanjeTemNG.xml` | onload-forma,combo,date |
| 52 | ne | - | - | 6 | **Biovega** | 0 | `KntBlaVrste.xml` | `asm:NetGenDLL.loader:KntBlaVrste.xml` |  |
| 53 | ne | - | - | 6 |  | 0 | `MPTrgovine.xml` | `asm:NetGenDll.loader:MPTrgovine.xml` |  |
| 54 | ne | stablo | - | 5 | **Biovega** | 5 | `MPNivelacija.xml` | `asm:NetGenDll.loader:MPNivelacija.xml` | stablo,setdefaults,buttons,combo,date |
| 55 | ne | - | - | 5 | **Biovega** | 7 | `KntPreknjizavanjeNG.xml` | `asm:NetGenDll.Loader:KntPreknjizavanjeNG.xml` | buttons,onload-forma,print,multiselect,search,combo,date |
| 56 | ne | - | - | 5 | **Biovega** | 3 | `HROsobeUcinak.xml` | `asm:NetGenDLL.Loader:HROsobeUcinak.xml` | setdefaults,combo,nextform |
| 57 | ne | - | - | 5 |  | 2 | `KUPVrstPN.xml` | `asm:NetGenDLL.loader:KUPVrstPN.xml` | combo,nextform |
| 58 | ne | - | - | 5 |  | 0 | `KntIzvodTijek.xml` | `asm:NetGenDll.loader:KntIzvodTijek.xml` |  |
| 59 | ne | - | - | 5 |  | 0 | `KntPriGen.xml` | `asm:NetGenDll.loader:KntPriGen.xml` |  |
| 60 | ne | - | - | 5 |  | 0 | `KntZatvaranjeNG.xml` | `asm:NetGenDLL.Loader:KntZatvaranjeNG.xml` |  |
| 61 | ne | - | - | 5 |  | 0 | `ManPokazateljiGrupe.xml` | `asm:NetGenDLL.Loader:ManPokazateljiGrupe.xml` |  |
| 62 | da | stablo | - | 4 |  | 12 | `MPBlagajna.xml` | `asm:NetGenDll.loader:MPBlagajna.xml` | stablo,setdefaults,buttons,print,repx,assemblyload,multiselect,combo,date,number,grid-dijalog,nextform |
| 63 | da | stablo | - | 4 |  | 12 | `MPBlagajna.xml` | `asm:NetGenDll.loader:MPBlagajna.xml` | stablo,setdefaults,buttons,print,repx,assemblyload,multiselect,combo,date,number,grid-dijalog,nextform |
| 64 | ne | stablo | - | 4 |  | 10 | `MPIzdVPNG.xml` | `asm:NetGenDll.loader:MPIzdVPNG.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 65 | ne | stablo | - | 4 | **Biovega** | 9 | `PNNalozi.xml` | `asm:NetGenDll.Loader:PNNalozi.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,grid-dijalog |
| 66 | ne | stablo | - | 4 |  | 3 | `kntShemaKonta.xml` | `asm:NetGenDll.loader:kntShemaKonta.xml` | stablo,buttons,print |
| 67 | ne | - | - | 4 | **Biovega** | 6 | `kntDobDospRac.xml` | `asm:NetGenDll.loader:kntDobDospRac.xml` | setdefaults,onload-forma,print,search,date,nextform |
| 68 | ne | - | - | 4 | **Biovega** | 6 | `kntDobDospRac.xml` | `asm:NetGenDLL.loader:kntDobDospRac.xml` | setdefaults,onload-forma,print,search,date,nextform |
| 69 | ne | - | - | 4 |  | 6 | `kntPreknjizavanjePartnera.xml` | `asm:NetGenDll.loader:kntPreknjizavanjePartnera.xml` | onload-forma,print,multiselect,search,combo,date |
| 70 | ne | - | - | 4 |  | 6 | `Predujmovi.xml` | `asm:NetGenDll.loader:Predujmovi.xml` | buttons,onload-forma,print,multiselect,date,nextform |
| 71 | ne | - | - | 4 |  | 4 | `PDVpoStjecanju.xml` | `asm:NetGenDLL.Loader:PDVpoStjecanju.xml` | onload-forma,print,date,nextform |
| 72 | ne | - | - | 4 |  | 0 | `PDVPromjena.xml` | `asm:NetGenDll.loader:PDVPromjena.xml` |  |
| 73 | ne | - | - | 4 |  | 0 | `PDVPromjena.xml` | `asm:NetGenDll.loader:PDVPromjena.xml` |  |
| 74 | ne | stablo | - | 3 |  | 7 | `MPPregUtrzakNG.xml` | `asm:NetGenDll.loader:MPPregUtrzakNG.xml` | stablo,setdefaults,onload-forma,combo,date,grid-dijalog,nextform |
| 75 | ne | - | - | 3 |  | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 76 | ne | - | - | 3 |  | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 77 | ne | - | - | 3 |  | 9 | `RekapitulacijaUI.xml` | `asm:NetGenDLL.Loader:SKLAD\RekapitulacijaUI.xml` | setdefaults,onload-forma,print,search,combo,date,check,radio,grid-dijalog |
| 78 | ne | - | - | 3 |  | 3 | `ServisPrava.xml` | `asm:NetGendll.frm:ServisPrava.xml` | onload-forma,combo,grid-dijalog |
| 79 | ne | - | - | 3 |  | 0 | `IzvKlasePartneriParstru.xml` | `asm:NetGenDLL.loader:IzvKlasePartneriParstru.xml` |  |
| 80 | ne | - | - | 3 |  | 0 | `jesteliznali.xml` | `asm:NetGenDll.Loader:jesteliznali.xml` |  |
| 81 | ne | - | - | 3 |  | 0 | `KntOpomene.xml` | `asm:NetGenDLL.frm:KntOpomene.xml` |  |
| 82 | ne | - | - | 3 |  | 0 | `KntRasporedTros.xml` | `asm:NetGenDll.loader:KntRasporedTros.xml` |  |
| 83 | ne | - | - | 3 |  | 0 | `KonfigSrvRN.xml` | `asm:NetGendll.loader:KonfigSrvRN.xml` |  |
| 84 | ne | - | - | 3 |  | 0 | `ManGadTip.xml` | `asm:NetGenDLL.loader:ManGadTip.xml` |  |
| 85 | ne | - | - | 3 |  | 0 | `newswebsite.xml` | `asm:NetgenDLL.loader:newswebsite.xml` |  |
| 86 | ne | - | - | 3 |  | 0 | `OrgShemaKonf.xml` | `asm:NetGendll.loader:OrgShemaKonf.xml` |  |
| 87 | ne | - | - | 3 |  | 0 | `OsobeOperateri.xml` | `asm:NetgenDLL.loader:OsobeOperateri.xml` |  |
| 88 | ne | - | - | 3 |  | 0 | `PubVarsNG.xml` | `asm:NetGenDll.loader:PubVarsNG.xml` |  |
| 89 | ne | - | - | 3 |  | 0 | `ServisJamstvo3.xml` | `asm:NetGenDll.loader:ServisJamstvo3.xml` |  |
| 90 | ne | - | - | 3 |  | 0 | `Spojka.xml` | `asm:NetGenDLL.Loader:Spojka.xml` |  |
| 91 | ne | - | - | 3 |  | 0 | `Upitnici.xml` | `asm:NetGenDll.frm:MAR\Upitnici.xml` |  |
| 92 | ne | - | - | 3 |  | 0 | `VoziloFinRekapNG.xml` | `asm:netgendll.loader:VoziloFinRekapNG.xml` |  |
| 93 | ne | stablo | - | 2 |  | 10 | `MPInvRazlike.xml` | `asm:NetGenDll.loader:MPInvRazlike.xml` | stablo,setdefaults,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 94 | ne | stablo | - | 2 | **Biovega** | 10 | `MPNarFFirma.xml` | `asm:NetGenDll.loader:MPNarFFirma.xml` | stablo,foreach,search,combo,date,number,check,multidrop,grid-dijalog,nextform |
| 95 | ne | stablo | - | 2 | **Biovega** | 4 | `LVKatalozi.xml` | `asm:NetGenDll.Loader:LVKatalozi.xml` | stablo,date,number,grid-dijalog |
| 96 | ne | stablo | - | 2 | **Biovega** | 1 | `OdobreniKaloNG.xml` | `asm:NetGenDLL.frm:OdobreniKaloNG.xml` | stablo |
| 97 | ne | - | - | 2 |  | 3 | `KUPStatN.xml` | `asm:NetGenDLL.loader:KUPStatN.xml` | foreach,combo,grid-dijalog |
| 98 | ne | - | - | 2 |  | 0 | `kategorija.xml` | `asm:NetgenDLL.Loader:kategorija.xml` |  |
| 99 | ne | - | - | 2 |  | 0 | `NacinObracunaZadatka.xml` | `asm:NetgenDll.loader:NacinObracunaZadatka.xml` |  |
| 100 | ne | - | - | 2 |  | 0 | `PovijestCjenika.xml` | `asm:NetGenDLL.loader:PovijestCjenika.xml` |  |
| 101 | ne | - | - | 2 |  | 0 | `PregledKnj.xml` | `asm:NetGenDll.loader:PregledKnj.xml` |  |
| 102 | da | - | - | 2 |  | 0 | `prioritet.xml` | `asm:NetgenDll.loader:prioritet.xml` |  |
| 103 | ne | - | - | 2 |  | 0 | `proces.xml` | `asm:NetgenDll.loader:proces.xml` |  |
| 104 | ne | - | - | 2 |  | 0 | `PTOsobeCjenik.xml` | `asm:NetgenDll.loader:PTOsobeCjenik.xml` |  |
| 105 | ne | - | - | 2 |  | 0 | `ptutrvrstarada.xml` | `asm:Netgendll.loader:ptutrvrstarada.xml` |  |
| 106 | ne | - | - | 2 |  | 0 | `PTVrstaDok.xml` | `asm:NetgenDll.loader:PTVrstaDok.xml` |  |
| 107 | ne | - | - | 2 | **Biovega** | 0 | `SktSerBrojAzur.xml` | `asm:NetGendll.loader:SktSerBrojAzur.xml` |  |
| 108 | ne | - | - | 2 |  | 0 | `StatusProjekta.xml` | `asm:NetgenDll.loader:StatusProjekta.xml` |  |
| 109 | ne | - | - | 2 |  | 0 | `statusZadatka.xml` | `asm:NetgenDLL.loader:statusZadatka.xml` |  |
| 110 | ne | - | - | 2 |  | 0 | `TecajnaListaNG.xml` | `asm:NetGenDll.loader:TecajnaListaNG.xml` |  |
| 111 | ne | - | - | 2 |  | 0 | `VrstaProjekt.xml` | `asm:NetgenDll.loader:VrstaProjekt.xml` |  |
| 112 | da | stablo | - | 1 |  | 11 | `MPPonuda.xml` | `asm:NetGenDll.loader:MPPonuda.xml` | stablo,setdefaults,buttons,foreach,search,combo,date,number,check,grid-dijalog,nextform |
| 113 | ne | - | - | 1 |  | 3 | `BisnodeExport.xml` | `asm:NetGenDll.loader:BisnodeExport.xml` | onload-forma,combo,date |
| 114 | ne | - | - | 1 |  | 0 | `admini.xml` | `asm:NetgenDll.loader:admini.xml` |  |
| 115 | ne | - | - | 1 |  | 0 | `admini.xml` | `asm:NetgenDll.loader:admini.xml` |  |
| 116 | ne | - | - | 1 |  | 0 | `Connexobustave.xml` | `asm:netgendll.loader:Connexobustave.xml` |  |
| 117 | ne | - | - | 1 |  | 0 | `CoreKompLog.xml` | `asm:NetGenDLL.loader:CoreKompLog.xml` |  |
| 118 | ne | - | - | 1 |  | 0 | `isokategorija.xml` | `asm:NetgenDLL.Loader:isokategorija.xml` |  |
| 119 | ne | - | - | 1 |  | 0 | `IspisiFirme.xml` | `asm:NetGenDll.loader:IspisiFirme.xml` |  |
| 120 | ne | - | - | 1 |  | 0 | `klasetecajevi.xml` | `asm:NetgenDll.Loader:klasetecajevi.xml` |  |
| 121 | ne | - | - | 1 |  | 0 | `klasetecajevi.xml` | `asm:NetgenDll.Loader:klasetecajevi.xml` |  |
| 122 | ne | - | - | 1 |  | 0 | `KntIOSObrazacDev.xml` | `asm:NetGenDLL.frm:KntIOSObrazacDev.xml` |  |
| 123 | ne | - | - | 1 |  | 0 | `KUPDostKnjigaTEST.xml` | `asm:NetGenDLL.loader:KUPDostKnjigaTEST.xml` |  |
| 124 | ne | - | - | 1 |  | 0 | `KUPDostKNOdobravanje.xml` | `asm:NetGenDLL.loader:KUPDostKNOdobravanje.xml` |  |
| 125 | ne | - | - | 1 |  | 0 | `KUPNG_ISOOdobravanje.xml` | `asm:NetGenDLL.loader:KUPNG_ISOOdobravanje.xml` |  |
| 126 | ne | - | - | 1 |  | 0 | `KUPNGSpin.xml` | `asm:NetGenDLL.loader:KUPNGSpin.xml` |  |
| 127 | ne | - | - | 1 |  | 0 | `KUPNGSpin_KUP_URA.xml` | `asm:NetGenDLL.loader:KUPNGSpin_KUP_URA.xml` |  |
| 128 | ne | - | - | 1 |  | 0 | `KUPNGTest.xml` | `asm:NetGenDLL.loader:KUPNGTest.xml` |  |
| 129 | ne | - | - | 1 |  | 0 | `MENIPubVars.xml` | `asm:NetGenDll.loader:MENIPubVars.xml` |  |
| 130 | ne | - | - | 1 |  | 0 | `MPAppManager.xml` | `asm:netgendll.loader:MPAppManager.xml` |  |
| 131 | ne | - | - | 1 |  | 0 | `MPRobnoDOk.xml` | `asm:NetGenDll.loader:MPRobnoDOk.xml` |  |
| 132 | ne | - | - | 1 |  | 0 | `PNNaloziMoji.xml` | `asm:NetGenDll.Loader:PNNaloziMoji.xml` |  |
| 133 | ne | - | - | 1 |  | 0 | `PregledOperateraNaDok.xml` | `asm:NetGenDLL.Loader:MER\PregledOperateraNaDok.xml` |  |
| 134 | ne | - | - | 1 |  | 0 | `Protokoli.xml` | `asm:NetGenDLL.loader:Protokoli.xml` |  |
| 135 | ne | - | - | 1 |  | 0 | `ProtokolKatalog.xml` | `asm:NetgenDLL.frm:ProtokolKatalog.xml` |  |
| 136 | ne | - | - | 1 |  | 0 | `RazlikeUvjetaDokUgovor.xml` | `asm:NetGenDll.loader:RazlikeUvjetaDokUgovor.xml` |  |
| 137 | ne | - | - | 1 |  | 0 | `statusadmin.xml` | `asm:NetgenDll.loader:statusadmin.xml` |  |
| 138 | ne | - | - | 1 |  | 0 | `statusadmin.xml` | `asm:NetgenDll.loader:statusadmin.xml` |  |
| 139 | ne | - | - | 1 |  | 0 | `TDObracun.xml` | `asm:NetGenDll.Loader:TDObracun.xml` |  |
| 140 | ne | - | - | 1 |  | 0 | `Tecajevi.xml` | `asm:NetGenDll.loader:Tecajevi.xml` |  |
| 141 | ne | - | - | 1 |  | 0 | `wwwkomp.xml` | `asm:NetGenDll.loader:wwwkomp.xml` |  |
