# Smoke spin: ssMeni NetGenDLL

Mjerenje na sql-ai, baza `spin`, samo citanje. Upit: `Parametar like 'asm:NetGenDLL.loader%'` ili `asm:NetGenDLL.frm%`. Operater 7, timeout 12 s. Izvor: CI run 43230508391, prije comboa na podlozi.

AOP: NetGen šalje `@id='null'` i prikaže grešku. To nije preskok. Narudžbe nemaju `ONLOAD`, master ostaje prazan dok se ne odabere čvor. Stablo dolazi iz prve vrijednosti `COMBO`. 21 stavka `nema-sql` nema master predložak, nije pad upita. 63 `nema-xml` nema datoteku u repou.

Ukupno 128, otvoreno 38.

## Uzroci

| Uzrok | Broj |
|---|---|
| nema-xml | 63 |
| forma | 26 |
| nema-sql | 21 |
| otvoreno | 17 |
| pretvorba | 1 |

## Stavke

| XML | Redova | Korijena | Uzrok | Poruka |
|---|---:|---:|---|---|
| FaktureMinMaxDat.xml |  |  | forma | ONLOAD je forma MAINFORM |
| KntIOSObrazacDev.xml |  |  | nema-xml | nema datoteke za KntIOSObrazacDev.xml |
| KntOpomene.xml |  |  | nema-xml | nema datoteke za KntOpomene.xml |
| MAR\Upitnici.xml |  |  | nema-xml | nema datoteke za MAR\Upitnici.xml |
| MPStanje.xml | 0 |  | nema-sql | prazno |
| OdobreniKaloNG.xml | 0 |  | nema-sql | prazno |
| ProtokolKatalog.xml |  |  | nema-xml | nema datoteke za ProtokolKatalog.xml |
| ServisPrava.xml |  |  | forma | ONLOAD je forma MAINFORM |
| SklMinMaxDat.xml |  |  | forma | ONLOAD je forma MAINFORM |
| admini.xml |  |  | nema-xml | nema datoteke za admini.xml |
| Akcije.xml | 0 |  | nema-sql | prazno |
| AOPNovi.xml |  |  | pretvorba | Error converting data type varchar to int.    spKntAOPOBRASCIQuery @action='master_grid',@id='null' |
| Bilanca-GFI.XML |  |  | forma | ONLOAD je forma FILTERFORM#select case when '@@izvjestaj'='GFI-POD' then 'disable' else 'enable' end as TSI#select case when '@@izvjestaj'='TSI-POD' then 'disable' else 'enable' end as GFI |
| BisnodeExport.xml |  |  | forma | ONLOAD je forma PARAM_FORM |
| Connexobustave.xml |  |  | nema-xml | nema datoteke za Connexobustave.xml |
| CoreKompLog.xml |  |  | nema-xml | nema datoteke za CoreKompLog.xml |
| DobUgovori.xml | 0 |  | nema-sql | prazno |
| HROsobeUcinak.xml | 154 |  | otvoreno |  |
| isokategorija.xml |  |  | nema-xml | nema datoteke za isokategorija.xml |
| IspisiFirme.xml |  |  | nema-xml | nema datoteke za IspisiFirme.xml |
| IzvKlasePartneriParstru.xml |  |  | nema-xml | nema datoteke za IzvKlasePartneriParstru.xml |
| jesteliznali.xml |  |  | nema-xml | nema datoteke za jesteliznali.xml |
| JOPPD.xml | 0 |  | nema-sql | prazno |
| kategorija.xml |  |  | nema-xml | nema datoteke za kategorija.xml |
| klasetecajevi.xml |  |  | nema-xml | nema datoteke za klasetecajevi.xml |
| KntBlaDok.xml | 0 | 3 | otvoreno |  |
| KntBlaPromet.xml | 0 | 2 | otvoreno |  |
| KntBlaVrste.xml |  |  | nema-xml | nema datoteke za KntBlaVrste.xml |
| KntBlaZak.xml | 1012 | 4 | otvoreno |  |
| kntDobDospRac.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| KntIzvodTijek.xml |  |  | nema-xml | nema datoteke za KntIzvodTijek.xml |
| KntKompVerizne.xml | 10 | 5 | otvoreno |  |
| KntKonfShemeMatTem.xml | 25 |  | otvoreno |  |
| KntOPZ-STAT-1.xml |  |  | forma | ONLOAD je forma UPIT |
| KntPDVObrasciEU.xml |  |  | forma | ONLOAD je forma FILTER_FORM |
| KntPreknjizavanjeNG.xml |  |  | forma | ONLOAD je forma FILTER |
| KntPreknjizavanjePartnera.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| KntPriGen.xml |  |  | nema-xml | nema datoteke za KntPriGen.xml |
| KntR2racuni1.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| KntRasporedTros.xml |  |  | nema-xml | nema datoteke za KntRasporedTros.xml |
| kntShemaKonta.xml | 0 | 1 | otvoreno |  |
| KntZatvaranjeNG.xml |  |  | nema-xml | nema datoteke za KntZatvaranjeNG.xml |
| KOD_RSPostavke.xml | 0 |  | nema-sql | prazno |
| KonfigSrvRN.xml |  |  | nema-xml | nema datoteke za KonfigSrvRN.xml |
| KUPDostKNOdobravanje.xml |  |  | nema-xml | nema datoteke za KUPDostKNOdobravanje.xml |
| KUPDostKnjigaTEST.xml |  |  | nema-xml | nema datoteke za KUPDostKnjigaTEST.xml |
| KUPNG_ISOOdobravanje.xml |  |  | nema-xml | nema datoteke za KUPNG_ISOOdobravanje.xml |
| KUPNGSpin.xml |  |  | nema-xml | nema datoteke za KUPNGSpin.xml |
| KUPNGSpin_KUP_URA.xml |  |  | nema-xml | nema datoteke za KUPNGSpin_KUP_URA.xml |
| KUPNGTest.xml |  |  | nema-xml | nema datoteke za KUPNGTest.xml |
| KUPStatN.xml | 7 |  | otvoreno |  |
| KUPVrstPN.xml | 14 |  | otvoreno |  |
| LVKatalozi.xml | 0 |  | nema-sql | prazno |
| LVObracun.xml | 0 | 3 | otvoreno |  |
| ManGadTip.xml |  |  | nema-xml | nema datoteke za ManGadTip.xml |
| ManPokazateljiGrupe.xml |  |  | nema-xml | nema datoteke za ManPokazateljiGrupe.xml |
| MatTem.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| MENIPubVars.xml |  |  | nema-xml | nema datoteke za MENIPubVars.xml |
| InstrumentiPlacanjaNG.xml | 15 |  | otvoreno |  |
| Intrastat.xml | 0 |  | nema-sql | ostao-@@ |
| ManagerObaveza.xml | 0 |  | nema-sql | prazno |
| MER\PregledOperateraNaDok.xml |  |  | nema-xml | nema datoteke za MER\PregledOperateraNaDok.xml |
| RealizacijaFakture.xml |  |  | forma | ONLOAD je forma Parametri |
| RealizacijaStavkeNG.xml |  |  | forma | ONLOAD je forma Parametri |
| VrCjenik.xml | 1 |  | otvoreno |  |
| MPAppManager.xml |  |  | nema-xml | nema datoteke za MPAppManager.xml |
| MPBlagajna.xml | 0 |  | nema-sql | prazno |
| MPFiskalPubVars.xml |  |  | forma | ONLOAD je forma UPDATEFORM |
| MPInventura.xml | 0 |  | nema-sql | prazno |
| MPInvRazlike.xml | 0 |  | nema-sql | prazno |
| MPIzdVPNG.xml | 0 |  | nema-sql | prazno |
| MPKase.xml | 1 |  | otvoreno |  |
| MPKaseOperNG.xml | 1 |  | otvoreno |  |
| MPKntNacinPlacAzur.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| MPKnjigaPopisa.xml | 0 |  | nema-sql | prazno |
| MPNarFFirma.xml | 0 |  | nema-sql | prazno |
| MPNivelacija.xml | 0 |  | nema-sql | prazno |
| MPPonuda.xml | 0 |  | nema-sql | prazno |
| MPPregUtrzakNG.xml |  |  | forma | ONLOAD je forma FORM1 |
| MPPSNG.xml | 0 |  | nema-sql | prazno |
| MPRACNEFISK.xml |  |  | forma | ONLOAD je forma FORM1 |
| MPRealizNG.xml |  |  | forma | ONLOAD je forma FORM1 |
| MPRobnoDOk.xml |  |  | nema-xml | nema datoteke za MPRobnoDOk.xml |
| MPTrgOprNG.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| MPTrgovine.xml |  |  | nema-xml | nema datoteke za MPTrgovine.xml |
| NacinObracunaZadatka.xml |  |  | nema-xml | nema datoteke za NacinObracunaZadatka.xml |
| newswebsite.xml |  |  | nema-xml | nema datoteke za newswebsite.xml |
| OrgShemaKonf.xml |  |  | nema-xml | nema datoteke za OrgShemaKonf.xml |
| OsobeOperateri.xml |  |  | nema-xml | nema datoteke za OsobeOperateri.xml |
| PartneriAuditNG.xml |  |  | forma | ONLOAD je forma FILTER |
| PDVpoStjecanju.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| PDVPromjena.xml |  |  | nema-xml | nema datoteke za PDVPromjena.xml |
| PNKatalozi.xml | 0 |  | nema-sql | prazno |
| PNNalozi.xml | 0 | 3 | otvoreno |  |
| PNNaloziMoji.xml |  |  | nema-xml | nema datoteke za PNNaloziMoji.xml |
| PNObracun.xml | 0 | 9 | otvoreno |  |
| Poruke.xml |  |  | nema-xml | nema datoteke za Poruke.xml |
| PorukeDoc.xml |  |  | nema-xml | nema datoteke za PorukeDoc.xml |
| PovijestCjenika.xml |  |  | nema-xml | nema datoteke za PovijestCjenika.xml |
| Predujmovi.xml |  |  | forma | ONLOAD je forma FILTERFORM |
| PregledKnj.xml |  |  | nema-xml | nema datoteke za PregledKnj.xml |
| PrijemniList.xml | 0 |  | nema-sql | prazno |
| prioritet.xml |  |  | nema-xml | nema datoteke za prioritet.xml |
| proces.xml |  |  | nema-xml | nema datoteke za proces.xml |
| Protokoli.xml |  |  | nema-xml | nema datoteke za Protokoli.xml |
| PTOsobeCjenik.xml |  |  | nema-xml | nema datoteke za PTOsobeCjenik.xml |
| ptutrvrstarada.xml |  |  | nema-xml | nema datoteke za ptutrvrstarada.xml |
| PTVrstaDok.xml |  |  | nema-xml | nema datoteke za PTVrstaDok.xml |
| PubVarsNG.xml |  |  | nema-xml | nema datoteke za PubVarsNG.xml |
| RazlikeUvjetaDokUgovor.xml |  |  | nema-xml | nema datoteke za RazlikeUvjetaDokUgovor.xml |
| ServisJamstvo3.xml |  |  | nema-xml | nema datoteke za ServisJamstvo3.xml |
| Narudzbe.xml | 0 |  | nema-sql | prazno |
| RekapitulacijaUI.xml |  |  | forma | ONLOAD je forma Parametri#refreshmaster |
| SkladisteNG.xml | 9 |  | otvoreno |  |
| SktSerBrojAzur.xml |  |  | nema-xml | nema datoteke za SktSerBrojAzur.xml |
| Spojka.xml |  |  | nema-xml | nema datoteke za Spojka.xml |
| statusadmin.xml |  |  | nema-xml | nema datoteke za statusadmin.xml |
| StatusProjekta.xml |  |  | nema-xml | nema datoteke za StatusProjekta.xml |
| statusZadatka.xml |  |  | nema-xml | nema datoteke za statusZadatka.xml |
| TDObracun.xml |  |  | nema-xml | nema datoteke za TDObracun.xml |
| Tecajevi.xml |  |  | nema-xml | nema datoteke za Tecajevi.xml |
| TecajnaListaNG.xml |  |  | nema-xml | nema datoteke za TecajnaListaNG.xml |
| TecRazOTV.xml |  |  | nema-xml | nema datoteke za TecRazOTV.xml |
| MPPDV.xml |  |  | forma | ONLOAD je forma Filter |
| VoziloFinRekapNG.xml |  |  | nema-xml | nema datoteke za VoziloFinRekapNG.xml |
| VrstaProjekt.xml |  |  | nema-xml | nema datoteke za VrstaProjekt.xml |
| wwwkomp.xml |  |  | nema-xml | nema datoteke za wwwkomp.xml |
| ZakljucivanjeTemNG.xml |  |  | forma | ONLOAD je forma MAINFORM |

## Kako čitati uzroke

| Uzrok | Značenje |
|---|---|
| nema-xml | 63 stavke. Putanja je u ssMeni, datoteke nema u `definitions/netgen`. XML se ne dodaje. |
| forma | 26 stavki. `ONLOAD` počinje s `:`. NetGen otvara tu formu. Master se ne puni dok forma ne vrati upit. |
| nema-sql | 21 stavka. Nema master `ONLOAD` (prazan predložak ili `@@` koji preživi). Prozor se i dalje otvori. |
| otvoreno | 17 stavki. Master SQL je prošao. Broj redova je u tablici. |
| pretvorba | 1 stavka, `AOPNovi.xml`. `@id='null'` jer čvor još nije odabran. |

