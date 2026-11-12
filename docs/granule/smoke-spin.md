# Smoke spin

Izvor: CI PR #27, commit `28a39f2`, run 43236761185. Baza `spin`, samo čitanje. Operater 7, timeout 12 s.

## Što se otvara

- ssMeni: 128 stavki, otvoreno 52, greške 4.
- XML u repou: 605 projekata, otvoreno 493, greške 112.

`Narudzbe.xml` (BIOVEGA i Agronom) je `nema-sql` s 1 korijenom: combo je napunio stablo, master predloška nema dok se ne klikne čvor.

## Zašto greške nisu popravak porta

- `pretvorba` na AOP: NetGen šalje `@id='null'` (`Node` kreće kao `"null"`, odabir čvora je komentar).
- `nema-procedure` (91 u repou): procedura nije u spinu. XML i baza se ne mijenjaju.
- `nema-forme`: `ONLOAD` traži element `/PROJECT/ime`. `FILTER` ili `FORM1` tog imena nema (NetGen `SelectSingleNode` radi isto). `NAME` atribut se ne čita kao ime forme.
- `sintaksa` na MPTrgOprNG: u mjerenju #27 `@@id` je otišao prazan. NetGen za formu zove `GetID()`, a prazan grid vrati `0` (`Form1.vb` 5293–5298, 5566–5582). To se ispravlja u sljedećem koraku (`SqlTekst.ZaFormu`). Master upit i dalje vidi prazan `ID` dok grid ne postoji.
- `krivi-stupac` OznakaKL: stupac `oznaka2` ne postoji u `klmasterroba`.
- `ostalo`: procedura odbije parametar ili SQL u XML-u ima dvostruki stupac. Tekst upita se ne mijenja.

# Smoke spin: ssMeni NetGenDLL

Samo citanje. Upit je `asm:NetGenDLL.loader` i `asm:NetGenDLL.frm`. Operater je 7. Timeout naredbe je 12 s. AOP i dalje šalje `@id='null'` jer NetGen `Node` kreće kao `"null"` i odabir čvora je komentar.

Ukupno 128, otvoreno 52, greske 4.

## Uzroci

| Uzrok | Broj |
|---|---|
| nema-xml | 72 |
| forma | 19 |
| nema-sql | 18 |
| otvoreno | 15 |
| nema-forme | 2 |
| pretvorba | 1 |
| sintaksa | 1 |

## Stavke

| XML | Redova | Korijena | Uzrok | Poruka |
|---|---:|---:|---|---|
| FaktureMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| KntIOSObrazacDev.xml |  |  | nema-xml | nema datoteke za KntIOSObrazacDev.xml |
| KntOpomene.xml |  |  | nema-xml | nema datoteke za KntOpomene.xml |
| MAR\Upitnici.xml |  |  | nema-xml | nema datoteke za MAR\Upitnici.xml |
| MPStanje.xml | 0 |  | nema-sql | prazno |
| OdobreniKaloNG.xml | 0 | 1 | nema-sql | prazno |
| ProtokolKatalog.xml |  |  | nema-xml | nema datoteke za ProtokolKatalog.xml |
| ServisPrava.xml | 0 |  | forma | otvorena MAINFORM |
| SklMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| admini.xml |  |  | nema-xml | nema datoteke za admini.xml |
| Akcije.xml | 0 | 5 | nema-sql | prazno |
| AOPNovi.xml |  | 1 | pretvorba | Error converting data type varchar to int.    spKntAOPOBRASCIQuery @action='master_grid',@id='null' |
| Bilanca-GFI.XML | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| BisnodeExport.xml | 1 |  | forma | otvorena PARAM_FORM |
| Connexobustave.xml |  |  | nema-xml | nema datoteke za Connexobustave.xml |
| CoreKompLog.xml |  |  | nema-xml | nema datoteke za CoreKompLog.xml |
| DobUgovori.xml | 0 | 7 | nema-sql | prazno |
| HROsobeUcinak.xml | 154 |  | otvoreno |  |
| isokategorija.xml |  |  | nema-xml | nema datoteke za isokategorija.xml |
| IspisiFirme.xml |  |  | nema-xml | nema datoteke za IspisiFirme.xml |
| IzvKlasePartneriParstru.xml |  |  | nema-xml | nema datoteke za IzvKlasePartneriParstru.xml |
| jesteliznali.xml |  |  | nema-xml | nema datoteke za jesteliznali.xml |
| JOPPD.xml | 0 | 2 | nema-sql | prazno |
| kategorija.xml |  |  | nema-xml | nema datoteke za kategorija.xml |
| klasetecajevi.xml |  |  | nema-xml | nema datoteke za klasetecajevi.xml |
| KntBlaDok.xml | 0 | 3 | otvoreno |  |
| KntBlaPromet.xml | 0 | 2 | otvoreno |  |
| KntBlaVrste.xml |  |  | nema-xml | nema datoteke za KntBlaVrste.xml |
| KntBlaZak.xml | 1012 | 4 | otvoreno |  |
| kntDobDospRac.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntIzvodTijek.xml |  |  | nema-xml | nema datoteke za KntIzvodTijek.xml |
| KntKompVerizne.xml | 10 | 5 | otvoreno |  |
| KntKonfShemeMatTem.xml | 25 |  | otvoreno |  |
| KntOPZ-STAT-1.xml | 0 |  | forma | otvorena UPIT, sql ostao-@@ |
| KntPDVObrasciEU.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| KntPreknjizavanjeNG.xml |  |  | nema-forme | nema forme FILTER |
| KntPreknjizavanjePartnera.xml | 1 |  | forma | otvorena FILTERFORM |
| KntPriGen.xml |  |  | nema-xml | nema datoteke za KntPriGen.xml |
| KntR2racuni1.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntRasporedTros.xml |  |  | nema-xml | nema datoteke za KntRasporedTros.xml |
| kntShemaKonta.xml | 0 | 1 | otvoreno |  |
| KntZatvaranjeNG.xml |  |  | nema-xml | nema datoteke za KntZatvaranjeNG.xml |
| KOD_RSPostavke.xml | 0 | 2 | nema-sql | prazno |
| KonfigSrvRN.xml |  |  | nema-xml | nema datoteke za KonfigSrvRN.xml |
| KUPDostKNOdobravanje.xml |  |  | nema-xml | nema datoteke za KUPDostKNOdobravanje.xml |
| KUPDostKnjigaTEST.xml |  |  | nema-xml | nema datoteke za KUPDostKnjigaTEST.xml |
| KUPNG_ISOOdobravanje.xml |  |  | nema-xml | nema datoteke za KUPNG_ISOOdobravanje.xml |
| KUPNGSpin.xml |  |  | nema-xml | nema datoteke za KUPNGSpin.xml |
| KUPNGSpin_KUP_URA.xml |  |  | nema-xml | nema datoteke za KUPNGSpin_KUP_URA.xml |
| KUPNGTest.xml |  |  | nema-xml | nema datoteke za KUPNGTest.xml |
| KUPStatN.xml | 7 |  | otvoreno |  |
| KUPVrstPN.xml | 14 |  | otvoreno |  |
| LVKatalozi.xml | 0 | 1 | nema-sql | prazno |
| LVObracun.xml | 0 | 3 | otvoreno |  |
| ManGadTip.xml |  |  | nema-xml | nema datoteke za ManGadTip.xml |
| ManPokazateljiGrupe.xml |  |  | nema-xml | nema datoteke za ManPokazateljiGrupe.xml |
| MatTem.xml | 1 |  | forma | otvorena FILTERFORM |
| MENIPubVars.xml |  |  | nema-xml | nema datoteke za MENIPubVars.xml |
| MER\InstrumentiPlacanjaNG.xml |  |  | nema-xml | nema datoteke za MER\InstrumentiPlacanjaNG.xml |
| MER\Intrastat.xml |  |  | nema-xml | nema datoteke za MER\Intrastat.xml |
| MER\ManagerObaveza.xml |  |  | nema-xml | nema datoteke za MER\ManagerObaveza.xml |
| MER\PregledOperateraNaDok.xml |  |  | nema-xml | nema datoteke za MER\PregledOperateraNaDok.xml |
| MER\RealizacijaFakture.xml |  |  | nema-xml | nema datoteke za MER\RealizacijaFakture.xml |
| MER\RealizacijaStavkeNG.xml |  |  | nema-xml | nema datoteke za MER\RealizacijaStavkeNG.xml |
| MER\vrcjenik.xml |  |  | nema-xml | nema datoteke za MER\vrcjenik.xml |
| MPAppManager.xml |  |  | nema-xml | nema datoteke za MPAppManager.xml |
| MPBlagajna.xml | 0 | 7 | nema-sql | prazno |
| MPFiskalPubVars.xml | 0 |  | forma | otvorena UPDATEFORM |
| MPInventura.xml | 0 | 7 | nema-sql | prazno |
| MPInvRazlike.xml | 0 | 7 | nema-sql | prazno |
| MPIzdVPNG.xml | 0 | 7 | nema-sql | prazno |
| MPKase.xml | 1 |  | otvoreno |  |
| MPKaseOperNG.xml | 1 |  | otvoreno |  |
| MPKntNacinPlacAzur.xml | 0 |  | forma | otvorena FILTERFORM |
| MPKnjigaPopisa.xml | 0 |  | nema-sql | prazno |
| MPNarFFirma.xml | 0 | 4 | nema-sql | prazno |
| MPNivelacija.xml | 0 | 7 | nema-sql | prazno |
| MPPonuda.xml | 0 | 7 | nema-sql | prazno |
| MPPregUtrzakNG.xml | 0 | 7 | forma | otvorena FORM1 |
| MPPSNG.xml | 0 | 8 | nema-sql | prazno |
| MPRACNEFISK.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRealizNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRobnoDOk.xml |  |  | nema-xml | nema datoteke za MPRobnoDOk.xml |
| MPTrgOprNG.xml |  |  | sintaksa | forma FILTERFORM: Incorrect syntax near '='. |
| MPTrgovine.xml |  |  | nema-xml | nema datoteke za MPTrgovine.xml |
| NacinObracunaZadatka.xml |  |  | nema-xml | nema datoteke za NacinObracunaZadatka.xml |
| newswebsite.xml |  |  | nema-xml | nema datoteke za newswebsite.xml |
| OrgShemaKonf.xml |  |  | nema-xml | nema datoteke za OrgShemaKonf.xml |
| OsobeOperateri.xml |  |  | nema-xml | nema datoteke za OsobeOperateri.xml |
| PartneriAuditNG.xml |  |  | nema-forme | nema forme FILTER |
| PDVpoStjecanju.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| PDVPromjena.xml |  |  | nema-xml | nema datoteke za PDVPromjena.xml |
| PNKatalozi.xml | 0 | 1 | nema-sql | prazno |
| PNNalozi.xml | 0 | 3 | otvoreno |  |
| PNNaloziMoji.xml |  |  | nema-xml | nema datoteke za PNNaloziMoji.xml |
| PNObracun.xml | 0 | 9 | otvoreno |  |
| Poruke.xml |  |  | nema-xml | nema datoteke za Poruke.xml |
| PorukeDoc.xml |  |  | nema-xml | nema datoteke za PorukeDoc.xml |
| PovijestCjenika.xml |  |  | nema-xml | nema datoteke za PovijestCjenika.xml |
| Predujmovi.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| PregledKnj.xml |  |  | nema-xml | nema datoteke za PregledKnj.xml |
| PrijemniList.xml | 0 | 1 | nema-sql | prazno |
| prioritet.xml |  |  | nema-xml | nema datoteke za prioritet.xml |
| proces.xml |  |  | nema-xml | nema datoteke za proces.xml |
| Protokoli.xml |  |  | nema-xml | nema datoteke za Protokoli.xml |
| PTOsobeCjenik.xml |  |  | nema-xml | nema datoteke za PTOsobeCjenik.xml |
| ptutrvrstarada.xml |  |  | nema-xml | nema datoteke za ptutrvrstarada.xml |
| PTVrstaDok.xml |  |  | nema-xml | nema datoteke za PTVrstaDok.xml |
| PubVarsNG.xml |  |  | nema-xml | nema datoteke za PubVarsNG.xml |
| RazlikeUvjetaDokUgovor.xml |  |  | nema-xml | nema datoteke za RazlikeUvjetaDokUgovor.xml |
| ServisJamstvo3.xml |  |  | nema-xml | nema datoteke za ServisJamstvo3.xml |
| SKLAD\Narudzbe.xml |  |  | nema-xml | nema datoteke za SKLAD\Narudzbe.xml |
| SKLAD\RekapitulacijaUI.xml |  |  | nema-xml | nema datoteke za SKLAD\RekapitulacijaUI.xml |
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
| UTP\MPPDV.xml |  |  | nema-xml | nema datoteke za UTP\MPPDV.xml |
| VoziloFinRekapNG.xml |  |  | nema-xml | nema datoteke za VoziloFinRekapNG.xml |
| VrstaProjekt.xml |  |  | nema-xml | nema datoteke za VrstaProjekt.xml |
| wwwkomp.xml |  |  | nema-xml | nema datoteke za wwwkomp.xml |
| ZakljucivanjeTemNG.xml | 0 |  | forma | otvorena MAINFORM |

# Smoke spin: XML u repou

Svaka NetGen datoteka pod `definitions/netgen`. Isti uvjeti. `nema-xml` ovdje nema.

Ukupno 605, otvoreno 493, greske 112.

## Uzroci

| Uzrok | Broj |
|---|---|
| forma | 234 |
| nema-sql | 183 |
| nema-procedure | 91 |
| otvoreno | 76 |
| nema-forme | 14 |
| pretvorba | 2 |
| ostalo | 2 |
| sintaksa | 2 |
| krivi-stupac | 1 |

## Stavke

| XML | Redova | Korijena | Uzrok | Poruka |
|---|---:|---:|---|---|
| Akcije.xml | 0 | 5 | nema-sql | prazno |
| AnalitikaNabave.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| AOPNovi.xml |  | 1 | pretvorba | Error converting data type varchar to int.    spKntAOPOBRASCIQuery @action='master_grid',@id='null' |
| Bilanca-GFI.XML | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| BisnodeExport.xml | 1 |  | forma | otvorena PARAM_FORM |
| Dne_Det_Agronom.xml | 0 | 1 | nema-sql | prazno |
| Dne_Smjene.xml | 0 |  | nema-sql | prazno |
| DobUgovori.xml | 0 | 7 | nema-sql | prazno |
| DrzaveNG.xml | 30 |  | otvoreno |  |
| FaktureMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| FaktureNeFisk.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| HRCKadManagerVanjski.xml | 0 | 1 | nema-sql | prazno |
| HRIspraveNG.xml | 0 |  | nema-sql | prazno |
| HRManagerGO.xml | 0 | 1 | nema-sql | prazno |
| HROsobeUcinak.xml | 154 |  | otvoreno |  |
| HRVrsteRV.xml | 0 |  | nema-sql | prazno |
| IzdatnicaIzPonude.xml | 0 |  | forma | otvorena MAINFORM |
| JeziciNG.xml | 15 |  | otvoreno |  |
| KarticaPartnera.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntBlaDok.xml | 0 | 3 | otvoreno |  |
| KntBlaPromet.xml | 0 | 2 | otvoreno |  |
| KntBlaZak.xml | 1012 | 4 | otvoreno |  |
| KntBrutoBilancaNG.xml | 1 | 1 | forma | otvorena FILTER_FORM |
| KntDospjeleStavke.xml | 0 |  | nema-sql | prazno |
| KntImportTem.xml | 0 |  | otvoreno |  |
| KntIzv306090.xml | 0 | 0 | nema-sql | prazno |
| kntIzv306090DEFKOP.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntKompVerizne.xml | 10 | 5 | otvoreno |  |
| KntKonfShemeMatTem.xml | 25 |  | otvoreno |  |
| KntKontoUzOrgshemu.xml | 201 |  | otvoreno |  |
| KntKontoUzRobu.xml |  |  | nema-procedure | Could not find stored procedure 'spKntRobaKonta'.    spKntRobaKonta @action=1 |
| KntOdobrenje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntOPZ-STAT-1.xml | 0 |  | forma | otvorena UPIT, sql ostao-@@ |
| KntPDVObrasciEU.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| KntPreknjizavanjeNG.xml |  |  | nema-forme | nema forme FILTER |
| KntPreknjizavanjePartnera.xml | 1 |  | forma | otvorena FILTERFORM |
| KntR2racuni1.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntRobneKorekcije.xml | 0 |  | nema-sql | prazno |
| kntShemaKonta.xml | 0 | 1 | otvoreno |  |
| KODIzv.xml | 0 | 1 | nema-sql | prazno |
| KODIzv2.xml | 0 | 1 | nema-sql | prazno |
| KODPotvrde.xml | 0 |  | nema-sql | prazno |
| KOD_Diskete.xml |  |  | nema-procedure | Could not find stored procedure 'spKOD_DiskPrijenosGlaQuery'.    spKOD_DiskPrijenosGlaQuery @action='master' |
| KOD_DopStope.xml |  | 0 | nema-procedure | Could not find stored procedure 'spKOD_DopStopeQuery'.    spKOD_DopStopeQuery |
| KOD_Doznake.xml | 0 | 1 | nema-sql | prazno |
| KOD_Korekcija.xml | 0 | 1 | nema-sql | prazno |
| KOD_Obustave.xml |  |  | nema-procedure | Could not find stored procedure 'spKOD_ObustaveQuery'.    spKOD_ObustaveQuery @operateriid=7 |
| KOD_Opcine2.xml | 0 |  | nema-sql | prazno |
| KOD_Predlozak.xml | 0 |  | nema-sql | prazno |
| KOD_RAD1.xml | 0 |  | nema-sql | prazno |
| KOD_Relacije.xml | 0 |  | nema-sql | prazno |
| KOD_RSPostavke.xml | 0 | 2 | nema-sql | prazno |
| KontniPlanHR-ENG.xml |  |  | nema-procedure | Could not find stored procedure 'spKntPlanHrEngQuery'.    spKntPlanHrEngQuery @action=1 |
| KoopObrSklad.xml |  |  | nema-forme | nema forme FILTER |
| KoopUgoFinLimit.xml |  |  | nema-procedure | Could not find stored procedure 'spOtkupUgovorFin_Query'.    spOtkupUgovorFin_Query @action='grid'; |
| KTEukpNG.xml | 0 |  | forma | otvorena Lista |
| Kulture.xml |  |  | nema-procedure | Could not find stored procedure 'spKultureNGQuery'.    spKultureNGQuery @Action=1 |
| KultureDet.xml |  |  | nema-forme | nema forme FILTER |
| KUPDostKN.xml | 0 | 4 | otvoreno |  |
| KUPNG.xml | 0 | 6 | otvoreno |  |
| KUPSktDokumenti.xml |  |  | nema-procedure | Could not find stored procedure 'spKUPSktDokumenti'.    spKUPSktDokumenti @action=0 |
| KUPStatN.xml | 7 |  | otvoreno |  |
| KUPVrstPN.xml | 14 |  | otvoreno |  |
| ManagerCjenika.xml |  |  | nema-procedure | forma MAINFORM: Could not find stored procedure 'spCjenik_Query'. |
| MatposMrtvaTrka.xml | 0 |  | forma | otvorena FILTERFORM |
| MatTem.xml | 1 |  | forma | otvorena FILTERFORM |
| MER_DobUgovori.xml | 0 | 7 | nema-sql | prazno |
| MER_ZahtjevZaNabavkuNG.xml | 0 | 1 | nema-sql | prazno |
| MjestaNG.xml | 625 |  | otvoreno |  |
| MjestoOtkupaPrava.xml |  |  | nema-procedure | Could not find stored procedure 'spMjestoOtkupaPravaQuery'.    spMjestoOtkupaPravaQuery @Action=0, @Id=null |
| MPAgronomEtiketeCjenikNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPAsortimanAzur.xml | 0 | 0 | nema-sql | prazno |
| MPAsortimanTip.xml |  |  | nema-procedure | Could not find stored procedure 'spMPAsortimanTipNG'.    spMPAsortimanTipNG @action = 1 |
| MPAzurPubVars.xml | 73 |  | otvoreno |  |
| MPBlagajnaZ.xml | 0 | 7 | nema-sql | prazno |
| MPBrojIskazniceZastita.xml |  |  | nema-procedure | Could not find stored procedure 'spMPBrojIskazniceZastita'.    spMPBrojIskazniceZastita @action = 1 |
| MPDirZaduzenjeNG1DMSAgronom.xml | 0 | 8 | nema-sql | prazno |
| MPDirZaduzenjeNG2DMS.xml | 0 | 8 | nema-sql | prazno |
| MPDirZaduzenjeNG3DMS.xml | 0 | 8 | nema-sql | prazno |
| MPDonesiPrometKase.xml | 1 |  | otvoreno |  |
| MPEtiketeNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPEtiketeTerminalAgronom.xml | 0 |  | forma | otvorena FILTERFORM |
| MPEurParam.xml | 1 |  | forma | otvorena UPDATEFORM |
| MPFISRoba.xml | 0 | 1 | nema-sql | prazno |
| MPIntDok.xml | 0 | 7 | nema-sql | prazno |
| MPIntDokZamjenaINVENTURA.xml | 0 | 7 | forma | otvorena DANAS_FORM |
| MPInvDonNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPInventura.xml | 0 | 7 | nema-sql | prazno |
| MPInvFaktura.xml | 0 |  | forma | otvorena FILTERFORM |
| MPInvPregArt.xml | 0 |  | forma | otvorena FILTERFORM |
| MPIzduMPZaprimi.xml | 0 | 8 | nema-sql | prazno |
| MPIzdVPMPKont.xml | 1 | 7 | otvoreno |  |
| MPJMzaMPC.xml | 0 | 1 | nema-sql | prazno |
| MPKalkKnjizenje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKaloNG.xml | 0 | 1 | nema-sql | prazno |
| MPKasaImport.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKasaPolog.xml | 0 |  | nema-sql | prazno |
| MPKaseOperNG.xml | 1 |  | otvoreno |  |
| MPKnjigaPopisa.xml | 0 |  | nema-sql | prazno |
| MPKnjigaPopisaSveTrg.xml | 0 |  | nema-sql | prazno |
| MPKntNacinPlacAzur.xml | 0 |  | forma | otvorena FILTERFORM |
| MPKonUtNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPKupciR1.xml | 0 |  | forma | otvorena FILTERFORM |
| MPKupciZastIsk.xml | 0 |  | forma | otvorena FILTERFORM |
| MPNajArtikliAgronom.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPNajnizaMPC.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPNapuniKase.xml | 1 |  | otvoreno |  |
| MPNar.xml | 0 | 6 | otvoreno |  |
| MPNarDobKor.xml | 0 | 6 | otvoreno |  |
| MPNarIzdSklAgronom.xml | 0 | 7 | nema-sql | prazno |
| MPNarIzdSklKomercAgronom.xml | 0 | 7 | otvoreno |  |
| MPNeZakNG.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPNGPregDok.xml | 0 | 6 | forma | otvorena FILTERFORM |
| MPnivelCNG.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNIvelacijaNG'.    spMPNIvelacijaNG @action=3, @operateriid=7 |
| MPnivlCKNG.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNIvelacijaNG'.    spMPNIvelacijaNG @action=3,@operateriid=7 |
| MPObracunKalaNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPOtpremnicaDMS.xml | 0 | 7 | nema-sql | prazno |
| MPOtpremnicaDonacijaDMS.xml | 0 | 7 | nema-sql | prazno |
| MPOtpremnicaKooperacijaDMS.xml | 0 | 7 | nema-sql | prazno |
| MPOtvCij.xml |  |  | nema-procedure | Could not find stored procedure 'spMPVezaniCjeniciNG'.    spMPVezaniCjeniciNG @action=111,@operateriid=7 |
| MPPDV.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| MPPonuda.xml | 0 | 7 | nema-sql | prazno |
| MPPONUDANEFISK.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPOSmeni.xml |  |  | nema-procedure | Could not find stored procedure 'spmpPOSmeni'.    spmpPOSmeni @action = 1 |
| MPPovijestPrijava.xml |  |  | ostalo | @operateriid is not a parameter for procedure spMPTrgovineNG.    spMPTrgovineNG @action=1, @operateriid = 7 |
| MPPovratDobDMSNG.xml | 0 | 7 | forma | otvorena DANAS_FORM |
| MPPovratPDV.xml | 0 | 7 | forma | otvorena DANAS_FORM |
| MPPovuVPAgronomRedovPov.xml | 0 | 7 | nema-sql | prazno |
| MPPovuVPAgronomVlastPotr.xml | 0 | 7 | nema-sql | prazno |
| MPPovuVPSKL.xml | 0 | 7 | nema-sql | prazno |
| MPPovuVPSKLKont.xml | 2 | 7 | otvoreno |  |
| MPPredispIz.xml | 0 | 7 | nema-sql | prazno |
| MPPredispUl.xml | 0 | 7 | nema-sql | prazno |
| MPPregArtNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregNekurAgronom.XML | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregNeprUtr.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregPovNak.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPPregPromVaucher.xml | 0 |  | forma | otvorena FORM1 |
| MPPregPS.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregRbTrNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregUtrzakNG.xml | 0 | 7 | forma | otvorena FORM1 |
| MPPrInvNG.xml | 0 |  | forma | otvorena FORM1 |
| MPPrmArtNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPrMPCNG.xml | 0 |  | forma | otvorena FORM1 |
| MPPrMPRNGKupacZastita.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPrMPRNGKupacZastitaNacPlac.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPrPovNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPrR0NG.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPRACNEFISK.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRacuni.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRealiKNG.xml | 0 |  | forma | otvorena FORM1 |
| MPRealizNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRealizNGBackOffice.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRekapPorezPP.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRekDokNGAgronom.XML | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRobabezMPC.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRobabezNC.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRobaKontigenti.xml | 0 | 2 | nema-sql | prazno |
| MPRobaPortal.xml | 0 | 1 | nema-sql | prazno |
| MPRobaRNNG.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPStanje.xml | 0 |  | nema-sql | prazno |
| MPStanjeRobeTrg.xml | 1 |  | otvoreno |  |
| MPStanjeRobeTrgBackOffice.xml | 1 |  | otvoreno |  |
| MPStanjeRobeTrgBezNab.xml | 1 |  | otvoreno |  |
| MPStanjeZaKasu.xml | 0 |  | forma | otvorena FILTERFORM |
| MPTestBlagajni.xml |  |  | nema-procedure | Could not find stored procedure 'spMPTestBlagajne'.    spMPTestBlagajne @action = 1 |
| MPTrgCjenNG.xml | 3 |  | otvoreno |  |
| MPTrgFiskPD.xml |  |  | nema-procedure | Could not find stored procedure 'spMPTrgFiskPD'.    spMPTrgFiskPD @action=1, @operateriid=7 |
| MPTrgOprNG.xml |  |  | sintaksa | forma FILTERFORM: Incorrect syntax near '='. |
| MPTrgovinaPortal.xml | 0 |  | nema-sql | prazno |
| MPUmirovNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPUpdateKase.xml | 0 |  | forma | otvorena FILTER_FORM |
| MPUtrNacPlcNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPUvjetiKase.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVaucherIspis.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVaucherNEW.xml | 0 |  | forma | otvorena FILTERFORM |
| MPZakljPerioda.xml |  |  | nema-procedure | Could not find stored procedure 'spMPZakljucakPeriodaMPNG'.    spMPZakljucakPeriodaMPNG @action = 8,  @operateriid = 7 |
| MPZastitnaSredstva.xml | 0 | 1 | nema-sql | prazno |
| NabStoNarucitiNG.xml | 0 | 3 | nema-sql | prazno |
| Narudzbe.xml | 0 | 1 | nema-sql | prazno |
| ObnovaNBCijena.xml | 0 |  | nema-sql | prazno |
| Obrazac.xml | 0 | 1 | nema-sql | prazno |
| OdobreniKaloNG.xml | 0 | 1 | nema-sql | prazno |
| OOPManIsplata.xml | 0 | 2 | nema-sql | prazno |
| osobe.xml |  |  | nema-procedure | Could not find stored procedure 'spOsobe_query'.    spOsobe_query @action='grid' |
| OtkupnoMjesto.xml | 2 |  | otvoreno |  |
| PartneriAuditNG.xml |  |  | nema-forme | nema forme FILTER |
| PDVpoStjecanju.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| PDVPromjenaNEW.xml | 1 | 1 | forma | otvorena FILTERFORM |
| PNKatalozi.xml | 0 | 1 | nema-sql | prazno |
| PNObracun.xml | 0 | 9 | otvoreno |  |
| Predujmovi.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| PrijemniList.xml | 0 | 1 | nema-sql | prazno |
| PSTudjaRoba.xml |  |  | nema-forme | nema forme FILTER |
| RatArkod.xml |  |  | nema-forme | nema forme FILTER |
| RatKatCestice.xml |  |  | nema-procedure | Could not find stored procedure 'spRatKatCestice_query'.    spRatKatCestice_query @action='all' |
| RatLnKultureRoba.xml |  |  | nema-procedure | Could not find stored procedure 'spLnKultureRoba_Query'.    spLnKultureRoba_Query @action='MAIN' |
| RatOtkupCijena.xml |  |  | nema-forme | nema forme FILTER |
| RatReproUtrosak.xml |  |  | nema-procedure | Could not find stored procedure 'spRatReproUtrosakGla_Query'.    spRatReproUtrosakGla_Query @action='grid' |
| RatVrstaProizvodnje.xml |  |  | nema-procedure | Could not find stored procedure 'spRatVrstaProizvodnje_query'.    spRatVrstaProizvodnje_query @action='all' |
| RatVrstaTla.xml |  |  | nema-procedure | Could not find stored procedure 'spRatVrstaTla_Query'.    spRatVrstaTla_Query @action='all' |
| RatVrstaUporabe.xml |  |  | nema-procedure | Could not find stored procedure 'spRatVrstaUporabe_Query'.    spRatVrstaUporabe_Query @action='all' |
| RatVrstaVlasnistva.xml |  |  | nema-procedure | Could not find stored procedure 'spRatVrstaVlasnistva_Query'.    spRatVrstaVlasnistva_Query @action='all' |
| RealizacijaFakture.xml | 0 |  | forma | otvorena Parametri, sql ostao-@@ |
| RealizacijaPonuda.xml | 0 |  | forma | otvorena PARAMETRI, sql ostao-@@ |
| RealizacijaStavkeNG.xml | 0 |  | forma | otvorena Parametri, sql ostao-@@ |
| RekapitulacijaUI.xml | 0 |  | forma | otvorena Parametri, sql ostao-@@ |
| ServisPrava.xml | 0 |  | forma | otvorena MAINFORM |
| SkladisteNG.xml | 9 |  | otvoreno |  |
| SklMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| sorte.xml |  |  | nema-procedure | Could not find stored procedure 'spSorte_Query'.    spSorte_Query |
| Statusi.xml | 0 | 1 | otvoreno |  |
| STFarmeNG.xml |  |  | nema-procedure | Could not find stored procedure 'spSTFarme'.    spSTFarme @action = 1, @operateriid= 7 |
| STGrloSkupinaNG.xml |  |  | nema-procedure | Could not find stored procedure 'spSTGrloSkupina'.    spSTGrloSkupina @action = 1, @operateriid= 7,  @vrsta = 'G' |
| STHranidbeniDani.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| STIzdatnicaFarme.xml | 0 | 3 | nema-sql | prazno |
| STIzlucenjeGrla.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STKategorijaNG.xml |  |  | nema-procedure | Could not find stored procedure 'spSTKategorija'.    spSTKategorija @action = 1, @operateriid= 7, @vrsta='G' |
| STKupovina.xml | 0 | 3 | nema-sql | prazno |
| STLista.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STPocetnoStanje.xml | 0 | 3 | nema-sql | prazno |
| STPregDok.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STPregNeTov.xml | 1 |  | forma | otvorena FILTERFORM |
| STPregPromGrla.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STPregTov.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STRazING.xml |  |  | nema-procedure | Could not find stored procedure 'spSTNacinIzlucenja'.    spSTNacinIzlucenja @action = 6, @operateriid = 7 |
| STStadoNG.xml | 0 | 0 | forma | otvorena FILTERFORM |
| STStadoPregledNG.xml |  | 0 | nema-procedure | forma FILTERFORM: Could not find stored procedure 'spstStadoQuery'. |
| STSTanjeGrlaNaDan.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| STTovUlaz.xml | 0 | 3 | forma | otvorena FILTERFORM, sql ostao-@@ |
| STVertikalnaKontrola.xml |  |  | nema-procedure | Could not find stored procedure 'spSTVertikalnaKontrola'.    spSTVertikalnaKontrola @action = 1 |
| TablaTip.xml |  |  | nema-procedure | Could not find stored procedure 'spTablaTip_Query'.    spTablaTip_Query @action='MAIN' |
| table.xml |  |  | nema-procedure | Could not find stored procedure 'spRatTabla_Query'.    spRatTabla_Query @action='MAIN' |
| UcinakOper.xml | 0 | 1 | nema-sql | prazno |
| UgovorSKupcima.xml | 0 | 3 | nema-sql | prazno |
| VrCjenik.xml | 1 |  | otvoreno |  |
| ZahtjevZaNabavkuNG.xml | 0 | 1 | nema-sql | prazno |
| ZakljucivanjeTemNG.xml | 0 |  | forma | otvorena MAINFORM |
| ZupanijeNG.xml | 209 |  | otvoreno |  |
| Akcije.xml | 0 | 5 | nema-sql | prazno |
| AnalitikaNabave.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| AOPNovi.xml |  | 1 | pretvorba | Error converting data type varchar to int.    spKntAOPOBRASCIQuery @action='master_grid',@id='null' |
| Bilanca-GFI.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| BiovegaFakGen.xml | 0 |  | nema-sql | prazno |
| bTobProcessPurchaseOrder.xml |  |  | nema-forme | nema forme FILTER |
| CarinskaTarifaNG.xml | 1 |  | otvoreno |  |
| Dne_Det_BioZrno.xml | 0 | 1 | nema-sql | prazno |
| Dne_Smjene.xml | 0 |  | nema-sql | prazno |
| Dne_SmjeneOrg.xml | 0 | 1 | nema-sql | prazno |
| DobUgovori.xml | 2 | 1 | otvoreno |  |
| DrzaveNG.xml | 30 |  | otvoreno |  |
| FakGen.xml | 0 |  | nema-sql | prazno |
| FakPar.xml |  |  | nema-procedure | Could not find stored procedure 'spFakPartneriUvjetiQuery'.    spFakPartneriUvjetiQuery |
| FaktureMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| FaktureNeFisk.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| GastroPartner.xml |  |  | nema-procedure | Could not find stored procedure 'spGastroPartneri_Query'.    spGastroPartneri_Query @action=0 |
| GastroPregledRacuna.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| HREvi.xml | 0 | 1 | nema-sql | prazno |
| HRManagerGO.xml | 0 | 1 | nema-sql | prazno |
| HROsobeUcinak.xml | 154 |  | otvoreno |  |
| HRPartneri.xml | 0 | 1 | nema-sql | prazno |
| HRSkole.xml | 0 |  | nema-sql | prazno |
| InstrumentiPlacanjaNG.xml | 15 |  | otvoreno |  |
| Intrastat.xml | 0 |  | nema-sql | ostao-@@ |
| inventuraCE.xml | 10 |  | otvoreno |  |
| InvRaz.xml | 1 |  | forma | otvorena FILTERFORM |
| IspisNaljepnica.xml | 1 |  | forma | otvorena MAINFORM |
| JeziciNG.xml | 15 |  | otvoreno |  |
| JobMail.xml | 0 |  | forma | otvorena FILTERFORM |
| JOPPD.xml | 0 | 2 | nema-sql | prazno |
| KKvalIZV.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntBlaPromet.xml | 0 | 2 | otvoreno |  |
| KntBlaZak.xml | 1012 | 4 | otvoreno |  |
| KntBrutoBilancaNG.xml | 1 | 1 | forma | otvorena FILTER_FORM |
| kntDobDospRac.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntKljuceviTroska2.xml | 0 |  | nema-sql | prazno |
| KntKonfShemeMatTem.xml | 25 |  | otvoreno |  |
| KntKontoUzOrgshemu.xml | 201 |  | otvoreno |  |
| KntOdobrenje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KntOPZ-STAT-1.xml | 0 |  | forma | otvorena UPIT, sql ostao-@@ |
| KntOrgshemaUzRobu.xml |  |  | nema-procedure | Could not find stored procedure 'spKntRobaOrgShema'.    spKntRobaOrgShema @action=1 |
| KntPDVObrasciEU.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| KntPreknjizavanjeNG.xml |  |  | nema-forme | nema forme FILTER |
| KntR2racuni1.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| KOD_Diskete.xml |  |  | nema-procedure | Could not find stored procedure 'spKOD_DiskPrijenosGlaQuery'.    spKOD_DiskPrijenosGlaQuery @action='master' |
| KOD_DopStope.xml |  | 0 | nema-procedure | Could not find stored procedure 'spKOD_DopStopeQuery'.    spKOD_DopStopeQuery |
| KOD_Doznake.xml | 0 | 1 | nema-sql | prazno |
| KOD_RAD1.xml | 0 |  | nema-sql | prazno |
| KOD_Relacije.xml | 0 |  | nema-sql | prazno |
| KOD_RSPostavke.xml | 0 | 2 | nema-sql | prazno |
| KOD_Stavke.xml | 0 | 1 | nema-sql | prazno |
| KomStanjeSkl.xml | 1 |  | forma | otvorena MAINFORM |
| Kulture.xml |  |  | nema-procedure | Could not find stored procedure 'spKultureNGQuery'.    spKultureNGQuery @Action=1 |
| KUPDostKnNGBvg.xml | 0 | 3 | otvoreno |  |
| KUPNGNGBvg.xml | 0 | 5 | otvoreno |  |
| KUPPregOdob.xml |  | 2 | nema-procedure | Could not find stored procedure 'spKUPPrometPregledPoste'.    spKUPPrometPregledPoste @action=23,@node='0' |
| KUPSktDokumenti.xml |  |  | nema-procedure | Could not find stored procedure 'spKUPSktDokumenti'.    spKUPSktDokumenti @action=0 |
| KUPStatNLinije.xml | 7 |  | otvoreno |  |
| KUPVrsPN.xml | 14 |  | otvoreno |  |
| LVKatalozi.xml | 0 | 1 | nema-sql | prazno |
| LVObracun.xml | 0 | 3 | otvoreno |  |
| ManagerObaveza.xml | 0 | 1 | nema-sql | prazno |
| MatposMrtvaTrka.xml | 0 |  | forma | otvorena FILTERFORM |
| MatTem.xml | 1 |  | forma | otvorena FILTERFORM |
| MjestaNG.xml | 625 |  | otvoreno |  |
| MP-RabUtNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPAnalBrStavaka.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPAnalizaVezana.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPAnalKNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPAsortimanAzurNBiovega.xml | 0 | 1 | nema-sql | prazno |
| MPAsortimanSIfreDOb.xml | 0 | 1 | nema-sql | prazno |
| MPAsortimanStruAzur.xml |  |  | nema-procedure | Could not find stored procedure 'spMPParAsortimanPregledNG'.    spMPParAsortimanPregledNG @action=1111,@partneriid=6382 |
| MPAsortimanTip.xml |  |  | nema-procedure | Could not find stored procedure 'spMPAsortimanTipNG'.    spMPAsortimanTipNG @action = 1 |
| MPAzurPubVars.xml | 73 |  | otvoreno |  |
| MPBlagajna.xml | 0 | 7 | nema-sql | prazno |
| MPBlagajnaBiovega.xml | 0 | 7 | nema-sql | prazno |
| MPBodNG.xml | 0 |  | otvoreno |  |
| MPClanK.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPClanKPlan.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPClanNG.xml | 0 |  | forma | otvorena FORM1 |
| MPClanPBng.xml | 0 |  | forma | otvorena FORM1 |
| MPDanDostaveSkt.xml | 0 |  | nema-sql | prazno |
| MPDirZaduzenjeKomNGTrosak.xml | 0 | 7 | nema-sql | prazno |
| MPDirZaduzenjeNG2.xml | 0 | 7 | nema-sql | prazno |
| MPDirZaduzenjeNG2Trosak.xml | 0 | 7 | nema-sql | prazno |
| MPDirZaduzenjeNG3.xml | 0 | 7 | nema-sql | prazno |
| MPDnevniProm.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPDobavljaciAmbulatni.xml | 0 | 1 | nema-sql | prazno |
| MPDodatniBodovi.xml | 0 |  | forma | otvorena FILTERFORM |
| MPDodKlasaMinKol.xml | 1 | 0 | forma | otvorena FILTERFORM |
| MPDodKlasaTransportno.xml | 1 | 0 | forma | otvorena FILTERFORM |
| MPDonTerNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPEFaktureOtpremnice.xml | 0 |  | forma | otvorena FILTERFORM |
| MPePotvrdeNarudzbe.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPePotvrdeNarudzbe_XML.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPEtiketeBiovegaRepx.xml | 0 |  | nema-sql | prazno |
| MPEtiketeNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPEtiketeTerminal.xml | 0 |  | forma | otvorena FILTERFORM |
| MPEtiketeTerminalNEW.xml | 0 |  | forma | otvorena FILTERFORM |
| MPEurParam.xml | 1 |  | forma | otvorena UPDATEFORM |
| MPFiskalPubVars.xml | 0 |  | forma | otvorena UPDATEFORM |
| MPGrupeRobeTest.xml | 0 |  | forma | otvorena FILTER_FORM |
| MPIntDokIntPrim.xml | 0 | 7 | nema-sql | prazno |
| MPIntDokZamjena.xml | 0 | 7 | nema-sql | prazno |
| MPIntDokZamjenaUGO.xml | 0 | 7 | nema-sql | prazno |
| MPInvDonNGNew.xml | 0 | 8 | nema-sql | prazno |
| MPInventura.xml | 0 | 7 | nema-sql | prazno |
| MPInvRazlike.xml | 0 | 7 | nema-sql | prazno |
| MPIzduMPZaprimi.xml | 0 | 4 | nema-sql | prazno |
| MPIzdVPMP.xml | 1 | 7 | otvoreno |  |
| MPIzdVPMPKont.xml | 1 | 7 | otvoreno |  |
| MPIzdVPNG.xml | 0 | 7 | nema-sql | prazno |
| MPJMzaMPC.xml | 0 | 1 | nema-sql | prazno |
| MPKalkKnjizenje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKasaPolog.xml |  |  | ostalo | Ambiguous column name 'odsat'.  Ambiguous column name 'odmin'.  Ambiguous column name 'dosat'.  Ambiguous column name 'domin'.  Ambiguous column name 'odsat'.  Ambiguous column nam |
| MPKase.xml | 1 |  | otvoreno |  |
| MPKaseOperNG.xml | 1 |  | otvoreno |  |
| MPKaseTemplate.xml | 0 |  | otvoreno |  |
| MPKlasaIsklCijena.xml |  |  | nema-procedure | Could not find stored procedure 'spmpMPKlaseIskljuciCijenaDan'.    spmpMPKlaseIskljuciCijenaDan @action = 1 |
| MPKnjigaPopisa.xml | 0 |  | nema-sql | prazno |
| MPKntNacinPlacAzur.xml | 0 |  | forma | otvorena FILTERFORM |
| MPKonIzBiovega.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKonOtpB.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKOntrolaObrProizvodnje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKontrolaPrimkiBB.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKontrolaProizvodnje.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKontrolaProizvodnje2.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPKontrolaUtrzaka.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPKonUtNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPKorekcijaStanja.xml |  |  | nema-procedure | Could not find stored procedure 'spmpKorekcijaStanja'.    spmpKorekcijaStanja @action=1,@Operateriid=7 |
| MPKupciR1.xml | 0 |  | forma | otvorena FILTERFORM |
| MPMinPeriodRoba.xml |  |  | nema-procedure | Could not find stored procedure 'spMPMinKolPeriod'.    spMPMinKolPeriod @action = 2 |
| MPNagradniBodovi.xml | 0 |  | forma | otvorena FILTERFORM |
| MPNajavaOptimalne.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNajavaOptimalne'.    spMPNajavaOptimalne @action = 3 |
| MPNajnizaMPC.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPNarFDobMail.xml |  | 7 | nema-procedure | Could not find stored procedure 'spMPNarFDobavljacu'.    spMPNarFDobavljacu @action=1,@operateriid=7,@node='danas' |
| MPNarFFirma.xml | 0 | 4 | nema-sql | prazno |
| MPNarIzdSkl.xml | 0 | 8 | nema-sql | prazno |
| MPNarIzdSklAgiliris.xml | 0 | 8 | nema-sql | prazno |
| MPNarPredis.xml | 0 | 7 | nema-sql | prazno |
| MPNarZaprimi.xml | 0 | 7 | otvoreno |  |
| MPNarZaprimiMail.xml | 0 | 7 | otvoreno |  |
| MPNarZaprimiPLT.xml | 0 | 7 | otvoreno |  |
| MPNarZaprimiPLTNeKom.xml | 0 | 7 | otvoreno |  |
| MPNeZakNG.XML | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPNGPregDok.xml | 0 |  | forma | otvorena FILTERFORM |
| MPNGPregDokMAIL.xml | 0 |  | forma | otvorena FILTERFORM |
| MPNivelacija.xml | 0 | 7 | nema-sql | prazno |
| MPnivelCNG.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNIvelacijaNG'.    spMPNIvelacijaNG @action=3, @operateriid=7 |
| MPnivelCNGBiovega.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNivelacijaNG'.    spMPNivelacijaNG @action=3, @operateriid=7 |
| MPnivelCNGPlanetopija.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNIvelacijaNG'.    spMPNIvelacijaNG @action=3, @operateriid=7 |
| MPnivelNCNGPlanetopija.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNivelacijaNCNG'.    spMPNivelacijaNCNG @action=3, @operateriid=7 |
| MPnivlCKNG.xml |  |  | nema-procedure | Could not find stored procedure 'spMPNIvelacijaNG'.    spMPNIvelacijaNG @action=3,@operateriid=7 |
| MPObiteljska.xml | 1 |  | forma | otvorena FILTERFORM |
| MPObracunKalaNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPObracunKalaNG_old.xml | 0 |  | forma | otvorena FILTERFORM |
| MPObracunSanka.xml | 0 |  | forma | otvorena FILTERFORM |
| MPObrpNG.XML | 1 |  | forma | otvorena FILTERFORM |
| MPOdjavaNarNG.xml |  | 6 | nema-procedure | Could not find stored procedure 'spMPOdjavaNG'.    spMPOdjavaNG @action=1,@operateriid=7,@node='danas' |
| MPOdjavaNG.xml | 0 | 0 | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPOperTrg.xml | 0 |  | forma | otvorena FILTERFORM |
| MPOptimalneZaliheNEW.xml | 1 | 1 | forma | otvorena FILTERFORM |
| MPOptimalneZaliheNEWNC0.xml | 1 | 1 | forma | otvorena FILTERFORM |
| MPOptimalneZaliheNEWv2.xml |  |  | nema-forme | nema forme FILTERFORM |
| MPOtpis.xml | 0 | 7 | nema-sql | prazno |
| MPOtpisanaRoba.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPOtpremnica.xml | 0 | 7 | nema-sql | prazno |
| MPOtpremnicaBB.xml | 0 | 7 | nema-sql | prazno |
| MPOtpremnicaF.xml | 0 | 7 | nema-sql | prazno |
| MPPakiranjeRoba.xml |  |  | nema-procedure | Could not find stored procedure 'spMPRobaPakiranje'.    spMPRobaPakiranje @action = 1 |
| MPParAsortIsporuka.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPDV.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| MPPoklonKartice.xml | 0 |  | forma | otvorena FORM1 |
| MPPonuda.xml | 0 | 8 | nema-sql | prazno |
| MPPonudaNaruciRazliku.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPonudaNEW.xml | 0 | 8 | nema-sql | prazno |
| MPPonudaPotrebe.xml | 1 |  | forma | otvorena FILTERFORM |
| MPPonudaWeb.xml | 0 | 8 | nema-sql | prazno |
| MPPonudaWEBP.xml | 0 | 8 | nema-sql | prazno |
| MPPorKPNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPorukeLoy.xml | 0 | 7 | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPOSmeni.xml |  |  | nema-procedure | Could not find stored procedure 'spmpPOSmeni'.    spmpPOSmeni @action = 1 |
| MPPovDobFran2.xml | 0 | 7 | nema-sql | ostao-@@ |
| MPPovratDobNGMail.xml | 0 | 7 | forma | otvorena DANAS_FORM |
| MPPovuVP.xml | 0 | 7 | nema-sql | prazno |
| MPPovuVPKONT.xml | 0 | 7 | nema-sql | prazno |
| MPPovuVPSKLKont.xml | 2 | 7 | otvoreno |  |
| MPPredispIz.xml | 0 | 7 | nema-sql | prazno |
| MPPredispUl.xml | 0 | 7 | nema-sql | prazno |
| MPPregANar.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregANarDob.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregANivNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregArtNG2.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregArtNGPlanetopija.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPregArtPonWeb.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregAutoNar.xml | 0 |  | nema-sql | prazno |
| MPPregDokMPVP.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregFakFran.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregKasa.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregKlaseStanja.xml | 0 |  | forma | otvorena FORM1 |
| MPPreglClanPOTrg.xml | 0 |  | forma | otvorena FORM1 |
| MPPregledPrometaCC.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPregNekur.XML | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregOSS.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPregProdajeAkcija.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregProizNorm.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregPromProiz.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregPromRabat.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregPromSirovina.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPregRbTrNaDatNG.xml | 0 |  | forma | otvorena FORM1 |
| MPPregRbTrNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregRbTrNG2.xml |  |  | nema-forme | nema forme FORM1 |
| MPPregRbTrNGBezPrava.xml | 1 |  | forma | otvorena FORM1 |
| MPPregRbTrNGBezPravaV2.xml | 1 |  | forma | otvorena FORM1 |
| MPPregStanjaPPkartica.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPregUtrzakNG.xml | 0 | 7 | forma | otvorena FORM1 |
| MPPregUtrzakRazdvojeno.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPPregVaucher.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPregWoltTrg.xml | 0 |  | forma | otvorena FILTERFORM |
| MPPrInvNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPrMPCNG.xml | 0 |  | forma | otvorena FORM1 |
| MPPrMPCNGPlanetopija.xml | 0 |  | forma | otvorena FORM1 |
| MPPrMPRNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPPrNCNG.xml | 0 |  | forma | otvorena FORM1 |
| MPPrR0NG.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPPrZakPerNG.xml | 1 |  | forma | otvorena FILTERFORM |
| MPPSNG.xml | 0 | 8 | nema-sql | prazno |
| MPRabatRaspored.xml | 0 |  | forma | otvorena FILTERFORM |
| MPRACNEFISK.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRazlogOtpisa.xml |  |  | nema-procedure | Could not find stored procedure 'spmpRazlogOtpisa'.    spmpRazlogOtpisa @action = 0 |
| MPRazlogPovrata.xml |  |  | nema-procedure | Could not find stored procedure 'spmpRazlogPovrata'.    spmpRazlogPovrata @action = 0 |
| MPRazlogStorna.xml |  |  | nema-procedure | Could not find stored procedure 'spmpRazlogStorna'.    spmpRazlogStorna @action = 0 |
| MPRealiKNG.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPRealizacijaKons.xml | 0 | 0 | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRekDokNG.XML | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRekPorDokNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPRobaBiovega.xml |  |  | nema-procedure | Could not find stored procedure 'spMPRobaBiovega'.    spMPRobaBiovega @action=1 |
| MPRobaNarPakovanje.xml | 1 | 0 | forma | otvorena FILTERFORM |
| MPRobaPortal.xml | 0 | 1 | nema-sql | prazno |
| MPRobaPredprodaja.xml |  |  | nema-procedure | Could not find stored procedure 'spmpDefPredProdaja'.    spmpDefPredProdaja @action=1 |
| MPRobaRNNG.xml | 0 |  | forma | otvorena FILTER_FORM, sql ostao-@@ |
| MPRobaXmlWolt.xml | 0 | 1 | nema-sql | prazno |
| MPRobaZabrana2.xml | 1 |  | forma | otvorena FILTERFORM |
| MPSirovine.xml | 0 | 2 | nema-sql | prazno |
| MPSmjene.xml |  |  | nema-procedure | Could not find stored procedure 'spMPSmjene'.    spMPSmjene @action = 6, @operateriid = 7 |
| MPStanje.xml | 0 |  | nema-sql | prazno |
| MPStanjeKons.xml | 0 | 0 | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPStanjePlan.xml | 0 |  | nema-sql | prazno |
| MPStanjeRobeBio.xml | 0 |  | forma | otvorena FILTERFORM |
| MPStanjeSirovina.xml | 0 |  | nema-sql | prazno |
| MPStanjeTrg.xml | 0 |  | nema-sql | prazno |
| MPTestKomisRobe.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPTrgCjenNG.xml | 3 |  | otvoreno |  |
| MPTrgOprNG.xml |  |  | sintaksa | forma FILTERFORM: Incorrect syntax near '='. |
| MPTrgovinaPortal.xml | 0 |  | nema-sql | prazno |
| MPTrgovineOstalo.xml | 0 |  | forma | otvorena FILTERFORM |
| MPTrgovineWoltXml.xml |  |  | nema-procedure | Could not find stored procedure 'spmpWoltXml'.    spmpWoltXml  @action = 1 |
| MPUgoRobaNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPUmirovNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| MPUpdateKase.xml | 0 |  | forma | otvorena FILTER_FORM |
| MPUslugeKasa.xml | 0 |  | forma | otvorena FILTERFORM |
| MPUtrNacPlcNG.xml | 0 |  | forma | otvorena FILTERFORM |
| MPUvjetiKase.xml | 0 |  | forma | otvorena FILTERFORM |
| MPUvjetiKaseNEW.xml |  |  | nema-forme | nema forme FILTERFORM |
| MPUvjetiKaseNEW2.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVaganaRobaDoD.xml | 0 | 1 | nema-sql | prazno |
| MPVaganaRobaTrg.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVaucher.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVaucherRokLom.xml | 0 |  | forma | otvorena FILTERFORM |
| MPVracenaRoba.xml | 0 |  | forma | otvorena FORM1, sql ostao-@@ |
| MPVrstaDokAzur.xml | 0 |  | forma | otvorena FILTERFORM |
| MPwebDostavaDef.xml |  |  | nema-procedure | Could not find stored procedure 'smmpwebdostave'.    smmpwebdostave @action=1 |
| MPZakljPerioda.xml | 1 |  | otvoreno |  |
| MPZakljucakPeriodaBiovega.xml |  |  | nema-procedure | Could not find stored procedure 'spMPZakljucakPeriodaMPNGBiovega'.    spMPZakljucakPeriodaMPNGBiovega @action = 8,  @operateriid = 7 |
| NabStoNarucitiBiovegaNG.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| NacinOtpremeNG.xml | 4 |  | otvoreno |  |
| Narudzbe.xml | 0 | 1 | nema-sql | prazno |
| nultiKontingenti.xml |  |  | nema-procedure | Could not find stored procedure 'spKkvalNulti_Query'.    spKkvalNulti_Query @action='grid' |
| nultiMultiKontingenti.xml |  | 1 | nema-procedure | Could not find stored procedure 'spRobaKontingenti'.    spRobaKontingenti @action='grid', @parametar=null |
| Obrazac.xml | 0 | 1 | nema-sql | prazno |
| OdjeliNG.xml | 57 |  | otvoreno |  |
| OdobreniKaloNG.xml | 0 | 1 | nema-sql | prazno |
| OOPDef.xml | 0 | 2 | nema-sql | prazno |
| OOPUcinakSezonci_Partneri.xml | 0 | 1 | nema-sql | prazno |
| OsStrojeviSve.xml |  |  | nema-procedure | Could not find stored procedure 'spSviStrojeviQuery'.    spSviStrojeviQuery @action=0 |
| Otpremnine.xml | 0 | 1 | nema-sql | prazno |
| OznakaKL.xml |  |  | krivi-stupac | Invalid column name 'oznaka2'.    select klmasterrobaid, sortkey, Naziv, oznaka, oznaka2 as DodatnaOznaka from klmasterroba order by sortkey |
| PartneriAuditNG.xml |  |  | nema-forme | nema forme FILTER |
| PDVpoStjecanju.xml | 0 |  | forma | otvorena FILTERFORM, sql ostao-@@ |
| PDVPromjenaNEW.xml | 1 | 1 | forma | otvorena FILTERFORM |
| PNKatalozi.xml | 0 | 1 | nema-sql | prazno |
| PNNalozi.xml | 0 | 3 | otvoreno |  |
| PNObracun.xml | 0 | 5 | otvoreno |  |
| PostTituleNG.xml | 25 |  | otvoreno |  |
| PredTituleNG.xml | 3 |  | otvoreno |  |
| PregledRacunaSBVG.xml | 0 |  | forma | otvorena Filter, sql ostao-@@ |
| prnPredajaM.xml |  | 0 | nema-procedure | Could not find stored procedure 'spprnPredajaGla_Query'.    spprnPredajaGla_Query @action = 'MAIN' |
| PrnRNNG.xml |  |  | nema-forme | nema forme FILTER |
| PrnTroskovi.xml |  |  | nema-procedure | Could not find stored procedure 'spPrnTroskoviGlaQuery'.    spPrnTroskoviGlaQuery @Action=1 |
| PrnVrstaTroska.xml |  |  | nema-procedure | Could not find stored procedure 'spPrnVrstaTroskaQuery'.    spPrnVrstaTroskaQuery @Action=1 |
| ProNotpL.xml | 0 |  | forma | otvorena MAINFORM, sql ostao-@@ |
| RatArkod.xml |  |  | nema-procedure | Could not find stored procedure 'spRatArkod_Query'.    spRatArkod_Query @action='main' |
| RatPlanSjetve2.xml |  |  | nema-procedure | Could not find stored procedure 'spRatPlanSjetveGla_Query'.    spRatPlanSjetveGla_Query @action='MAIN' |
| RatPlanTip.xml |  |  | nema-procedure | Could not find stored procedure 'spRatPlanTip_Query'.    spRatPlanTip_Query @action='MAIN' |
| RobaAtributiUredi.xml | 0 | 1 | nema-sql | prazno |
| RobaDeklaracijeNG.xml | 0 | 0 | nema-sql | prazno |
| RobaNutritivneVrijednosti.xml |  |  | nema-procedure | Could not find stored procedure 'spRobaNutritivneAzur'.    spRobaNutritivneAzur @action='Grid' |
| RobaPakPovNG.xml |  |  | nema-procedure | Could not find stored procedure 'spRobaPakiranjaPovratAzur'.    spRobaPakiranjaPovratAzur @action=0 |
| RobaPosebniAtributi.xml |  |  | nema-procedure | Could not find stored procedure 'spRobaPosebniAtributiAzur'.    spRobaPosebniAtributiAzur @action='Grid' |
| RobaPosebniAtributi2.xml |  |  | nema-procedure | Could not find stored procedure 'spRobaPosebniAtributiAzur'.    spRobaPosebniAtributiAzur @action='Grid' |
| RobaUputeCuvanja.xml |  |  | nema-procedure | Could not find stored procedure 'spRobaUputeCuvanjaAzur'.    spRobaUputeCuvanjaAzur @action='Grid' |
| RobaWebKlase.xml | 0 | 0 | nema-sql | prazno |
| RobnaMNG.XML | 0 |  | forma | otvorena FORM1 |
| RobneMarkeAzur.xml | 0 | 1 | nema-sql | prazno |
| SkladisteNG.xml | 9 |  | otvoreno |  |
| SklMinMaxDat.xml | 1 |  | forma | otvorena MAINFORM |
| sktIntOtMP.xml | 0 | 0 | nema-sql | prazno |
| sktkont.xml | 0 |  | otvoreno |  |
| sktStariKruh.xml | 0 |  | nema-sql | prazno |
| sorte.xml |  |  | nema-procedure | Could not find stored procedure 'spSorte_Query'.    spSorte_Query |
| table.xml |  |  | nema-procedure | Could not find stored procedure 'spRatTabla_Query'.    spRatTabla_Query @action='MAIN' |
| UcinakDet_BioZrno.xml | 0 | 2 | nema-sql | prazno |
| UcinakDet_R.xml | 0 | 2 | nema-sql | prazno |
| UcinakObr_Agrar.xml | 0 | 2 | nema-sql | prazno |
| UcinakOper.xml | 0 | 1 | nema-sql | prazno |
| UgovorSKupcima.xml | 0 | 3 | nema-sql | prazno |
| ValuteNG.xml | 32 |  | otvoreno |  |
| wmsinfo.xml | 0 |  | forma | otvorena FILTERFORM |
| wmsInventura.xml |  |  | nema-procedure | Could not find stored procedure 'spWMS_Inventura_Query'.    exec spWMS_Inventura_Query @ind = 'ZAG' |
| ZupanijeNG.xml | 209 |  | otvoreno |  |
| BBAzurPPD.xml |  |  | nema-procedure | Could not find stored procedure 'dbo.spBBQuery'.    dbo.spBBQuery @action=0 |
| BBZak.xml | 0 | 1 | nema-sql | prazno |
| BlaBSUnos.xml |  |  | nema-procedure | Could not find stored procedure 'spBSBlagajnaVGV'.    spBSBlagajnaVGV @action='master', @bbmjestaid=1 |
| BlaBSUnosFISK.xml |  |  | nema-procedure | Could not find stored procedure 'spBSBlagajnaVGV'.    spBSBlagajnaVGV @action='master', @bbmjestaid=3 |
| BlaBSUnosPN.xml |  |  | nema-procedure | Could not find stored procedure 'spBSBlagajnaVGV'.    spBSBlagajnaVGV @action='master', @bbmjestaid=4 |
| BSCjen.xml | 0 | 1 | nema-sql | prazno |
| BSDok.xml | 0 | 1 | nema-sql | prazno |
| BSEFina.xml | 0 | 3 | nema-sql | prazno |
| BSEizvod.xml | 0 | 4 | nema-sql | prazno |
| BSGlOtpad.xml | 0 | 2 | nema-sql | prazno |
| BSInk.xml | 0 | 1 | nema-sql | prazno |
| BSIzv.xml | 0 | 31 | nema-sql | prazno |
| BSKamate.xml | 0 | 1 | nema-sql | prazno |
| BSKnjizUKontor.xml | 0 |  | nema-sql | prazno |
| BSKor.xml | 0 | 2 | nema-sql | prazno |
| BSMMSl.xml | 0 | 0 | nema-sql | prazno |
| BSOcitanjaNarukvica.xml | 0 |  | nema-sql | prazno |
| BSOcitanja_WEB.xml |  | 3 | nema-procedure | Could not find stored procedure 'spBSOcitanjaWeb_Query'.    spBSOcitanjaWeb_Query @action='master' |
| BSOdvozKom.xml | 0 | 1 | nema-sql | prazno |
| BSOpomene1.xml | 0 | 1 | nema-sql | prazno |
| BSOvrhe.xml | 0 | 1 | nema-sql | prazno |
| BSOvrheOpomene.xml | 0 | 1 | nema-sql | prazno |
| BSPDA.xml | 0 | 1 | nema-sql | prazno |
| BSPosude.xml | 0 | 3 | nema-sql | prazno |
| BSRacPr.xml | 0 |  | nema-sql | prazno |
| BSRek.xml | 0 | 2 | nema-sql | prazno |
| BSReobracun.xml | 0 | 1 | nema-sql | prazno |
| BSReobracunN.xml | 0 | 1 | nema-sql | prazno |
| BSRute.xml |  |  | nema-procedure | Could not find stored procedure 'spBSRute_Query'.    spBSRute_Query @action='zone' |
| BSSporazumi.xml | 0 | 1 | nema-sql | prazno |
| BSTemeljniPodaci.xml | 0 | 10 | nema-sql | prazno |
| BSUgovori_INC.xml | 0 |  | nema-sql | prazno |
| BSUplatnicaRucno.xml | 0 | 2 | nema-sql | prazno |
| BSUsl.xml | 0 | 1 | nema-sql | prazno |
| BSVGV_RacuniUsluge.xml | 0 | 2 | nema-sql | prazno |
