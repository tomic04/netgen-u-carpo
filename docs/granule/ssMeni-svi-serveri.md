# ssMeni NetGen na jupiter2019, jupiter2022 i spin

Izvor: CI run [43233316047](https://github.com/tomic04/netgen-u-carpo/actions/runs/43233316047), commit `f6de248`. Samo `SELECT`, `WITH (NOLOCK)`, timeout 8 s, Integrated Security. Nema `EXEC`, nema pokretanja granula.

Točna imena iz `sys.databases`:

| Traženo | Gdje je pročitano | NetGen redova |
|---|---|---:|
| Biovega | `Biovega` na jupiter2019 | 422 |
| Centar tehnike | `CentarTehnike` na jupiter2019 i jupiter2022 | 174 i 177 |
| Panissimo | `Panissimo` na jupiter2022 | 311 |
| Setcor | `Setcor` na jupiter2019 i jupiter2022 | 152 i 124 |
| Tusal | nema tog imena; pročitan je `Tusak` na jupiter2022 | 309 |
| Enna | `Enna` na jupiter2019 | 167 |
| Fidus | `fidus` na jupiter2019 nema tablicu `ssMeni` | 0 |
| Ireks | `Ireks` na jupiter2019 | 172 |
| spin | `spin` na sql-ai | 142 |

`SetcorDMS`, `fidusgrupa`, `IreksCRMCro`, `CentarTehnikeSnapshot` i slična imena su u popisu baza i nisu čitana. Biovega je u tablicama označena **Biovega**. Datoteke kojih nema u `definitions/netgen` su u odjeljku Nedostaje XML, poredane po broju baza.

Samo citanje. `SELECT` iz `ssMeni` s `WITH (NOLOCK)`, `CommandTimeout` 8 s, `Connect Timeout` 8 s, `Pooling=false`, `Application Name=CarpoQA-ssMeni-read`, Integrated Security. Prvi pokusaj veze je `ApplicationIntent=ReadOnly`. Ako veza padne, jedan ponovni pokusaj bez readable secondary, i dalje samo `SELECT`. Nema `EXEC`, `INSERT`, `UPDATE`, `DELETE`, DDL, privremenih tablica, pokretanja granula ni master SQL-a.

Upit po bazi (shema i stupac naziva se uzimaju iz `sys.tables` / `sys.columns`; naziv je prvi od `Opis`, `Naziv`, `Name`, `Caption`, `Tekst`):

```sql
select top (800) [Opis], [Parametar]
from [dbo].[ssMeni] with (nolock)
where [Parametar] like '%netgendll%'
```

## Pristup

| Server | Baza | Kljuc | Stanje | Redova | Nacin | Napomena |
|---|---|---|---|---:|---|---|
| jupiter2019 | Biovega | biovega | ok | 422 | readonly | Opis |
| jupiter2019 | CentarTehnike | centartehnike | ok | 174 | readonly | Opis |
| jupiter2022 | CentarTehnike | centartehnike | ok | 177 | readonly | Opis |
| jupiter2022 | Panissimo | panissimo | ok | 311 | readonly | Opis |
| jupiter2019 | Setcor | setcor | ok | 152 | readonly | Opis |
| jupiter2022 | Setcor | setcor | ok | 124 | readonly | Opis |
| jupiter2022 | Tusak | tusal | ok | 309 | readonly | Opis; ime u sys.databases je Tusak; trazeno tusal |
| jupiter2019 | Enna | enna | ok | 167 | readonly | Opis |
| jupiter2019 | fidus | fidus | nema-ssMeni | 0 | readonly | nema tablice ssMeni |
| jupiter2019 | Ireks | ireks | ok | 172 | readonly | Opis |
| sql-ai | spin | spin | ok | 142 | readonly | Opis |

## Baze na serveru

Imena iz `sys.databases`. `ssMeni` se cita samo kad je normalizirano ime tocno jednako kljucu (`Biovega`, `CentarTehnike`, `Panissimo`, `Setcor`, `Enna`, `Fidus`, `Ireks`) i baza je `ONLINE`, na jupiter2019 ili jupiter2022. `Tusal` nema tog imena; cita se `Tusak`. DMS, DBF, CRM i snapshot se ne citaju.

| Server | Baza | Stanje | Kljuc | ssMeni upitan |
|---|---|---|---|:---:|
| jupiter2019 | master | ONLINE |  |  |
| jupiter2019 | tempdb | ONLINE |  |  |
| jupiter2019 | model | ONLINE |  |  |
| jupiter2019 | msdb | ONLINE |  |  |
| jupiter2019 | Setcor | ONLINE | setcor | da |
| jupiter2019 | JetSetDBFOld1 | ONLINE |  |  |
| jupiter2019 | JupiterSystem | ONLINE |  |  |
| jupiter2019 | JupiterResources | ONLINE |  |  |
| jupiter2019 | JupiterTranslations | ONLINE |  |  |
| jupiter2019 | JupiterXML | ONLINE |  |  |
| jupiter2019 | SetcorDMS | ONLINE |  |  |
| jupiter2019 | HRMDevDMS | ONLINE |  |  |
| jupiter2019 | Prezentacija | ONLINE |  |  |
| jupiter2019 | PrezentacijaDMS | ONLINE |  |  |
| jupiter2019 | test8 | ONLINE |  |  |
| jupiter2019 | HRImovina | ONLINE |  |  |
| jupiter2019 | VGV | ONLINE |  |  |
| jupiter2019 | JupiterMobile | ONLINE |  |  |
| jupiter2019 | DBF2025_1021 | ONLINE |  |  |
| jupiter2019 | HMOD | ONLINE |  |  |
| jupiter2019 | SpinDMS | ONLINE |  |  |
| jupiter2019 | JetSetDBFOld | ONLINE |  |  |
| jupiter2019 | TestValentino | ONLINE |  |  |
| jupiter2019 | ServerModeGridProjects | ONLINE |  |  |
| jupiter2019 | PrijenosBlagajneMlinarTruncated3 | ONLINE |  |  |
| jupiter2019 | MesopreradaDBF | ONLINE |  |  |
| jupiter2019 | Obuka | ONLINE |  |  |
| jupiter2019 | testkk | ONLINE |  |  |
| jupiter2019 | HRMODKK | ONLINE |  |  |
| jupiter2019 | Kongo | ONLINE |  |  |
| jupiter2019 | Demo | ONLINE |  |  |
| jupiter2019 | HRMDev | ONLINE |  |  |
| jupiter2019 | HRMInit | ONLINE |  |  |
| jupiter2019 | HRMInitDMS | ONLINE |  |  |
| jupiter2019 | testMP | ONLINE |  |  |
| jupiter2019 | Insta6 | ONLINE |  |  |
| jupiter2019 | Food | ONLINE |  |  |
| jupiter2019 | HRMTest | ONLINE |  |  |
| jupiter2019 | sqlitetest | ONLINE |  |  |
| jupiter2019 | testVP | ONLINE |  |  |
| jupiter2019 | SpinAgro | ONLINE |  |  |
| jupiter2019 | AgroTovarnikDBF | ONLINE |  |  |
| jupiter2019 | Mauthner | ONLINE |  |  |
| jupiter2019 | Spin2 | ONLINE |  |  |
| jupiter2019 | Trening | ONLINE |  |  |
| jupiter2019 | TreningDMS | ONLINE |  |  |
| jupiter2019 | JupiterWeb | ONLINE |  |  |
| jupiter2019 | fidus | ONLINE | fidus | da |
| jupiter2019 | fidusgrupa | ONLINE |  |  |
| jupiter2019 | SQLShackFinancial | ONLINE |  |  |
| jupiter2019 | JetSetDBF2024Old | ONLINE |  |  |
| jupiter2019 | SpinNotification | ONLINE |  |  |
| jupiter2019 | Tia | ONLINE |  |  |
| jupiter2019 | IreksCRMCro | ONLINE |  |  |
| jupiter2019 | IreksCRMCroDMS | ONLINE |  |  |
| jupiter2019 | IreksCRMSrb | ONLINE |  |  |
| jupiter2019 | WebPPOrahovica | ONLINE |  |  |
| jupiter2019 | HNZ | ONLINE |  |  |
| jupiter2019 | domagoj_sql | ONLINE |  |  |
| jupiter2019 | PrvoHrPivo | ONLINE |  |  |
| jupiter2019 | Joppd | ONLINE |  |  |
| jupiter2019 | JetSetDBF20240201 | ONLINE |  |  |
| jupiter2019 | CentarTehnike | ONLINE | centartehnike | da |
| jupiter2019 | MiholjacPlace | ONLINE |  |  |
| jupiter2019 | MVodovod | ONLINE |  |  |
| jupiter2019 | JupiterSrv | ONLINE |  |  |
| jupiter2019 | Biovega | ONLINE | biovega | da |
| jupiter2019 | TestNemanja | ONLINE |  |  |
| jupiter2019 | mAS2 | ONLINE |  |  |
| jupiter2019 | Multinorm | ONLINE |  |  |
| jupiter2019 | Ireks | ONLINE | ireks | da |
| jupiter2019 | TiaDMS | ONLINE |  |  |
| jupiter2019 | TestFirme | ONLINE |  |  |
| jupiter2019 | Tisup | ONLINE |  |  |
| jupiter2019 | JetSetDBF20240318 | ONLINE |  |  |
| jupiter2019 | JetSetDBF2024 | ONLINE |  |  |
| jupiter2019 | 2023JetSet | ONLINE |  |  |
| jupiter2019 | 2023JetSetJ | ONLINE |  |  |
| jupiter2019 | 2024JetSet | ONLINE |  |  |
| jupiter2019 | BrionkaDDNova | ONLINE |  |  |
| jupiter2019 | Sirrah | ONLINE |  |  |
| jupiter2019 | 4D_Wand_DW | ONLINE |  |  |
| jupiter2019 | JetSetDBF2024a | ONLINE |  |  |
| jupiter2019 | ECS | ONLINE |  |  |
| jupiter2019 | Enna | ONLINE | enna | da |
| jupiter2019 | PoljoprivredaBI | ONLINE |  |  |
| jupiter2019 | AgroCepin | ONLINE |  |  |
| jupiter2019 | AgroCepinDBF | ONLINE |  |  |
| jupiter2019 | BOBIS | ONLINE |  |  |
| jupiter2019 | DemoEuro | ONLINE |  |  |
| jupiter2019 | Mesoprerada | ONLINE |  |  |
| jupiter2019 | MlinarTruncated | ONLINE |  |  |
| jupiter2019 | MlinarTruncated3 | ONLINE |  |  |
| jupiter2019 | Usluga_Brinje | ONLINE |  |  |
| jupiter2019 | Usluga_Vreline | ONLINE |  |  |
| jupiter2019 | TreningOrteo | ONLINE |  |  |
| jupiter2019 | Claims | ONLINE |  |  |
| jupiter2019 | JosicDBF2024 | ONLINE |  |  |
| jupiter2019 | JosicDBF2025 | ONLINE |  |  |
| jupiter2019 | JosicDBF2023os | ONLINE |  |  |
| jupiter2019 | JosicDBF2023pl | ONLINE |  |  |
| jupiter2019 | JosicDBF2023pp | ONLINE |  |  |
| jupiter2019 | JosicDBF2023sa | ONLINE |  |  |
| jupiter2019 | DBF2025_0505 | ONLINE |  |  |
| jupiter2019 | DBF2024_0505 | ONLINE |  |  |
| jupiter2019 | DBF2024_0526 | ONLINE |  |  |
| jupiter2019 | DBF2025_0526 | ONLINE |  |  |
| jupiter2019 | Bijelić | ONLINE |  |  |
| jupiter2019 | hrmdevfs | ONLINE |  |  |
| jupiter2019 | TestValentino1 | ONLINE |  |  |
| jupiter2019 | DBF2025_0916 | ONLINE |  |  |
| jupiter2019 | RazvojTestBaza | ONLINE |  |  |
| jupiter2019 | Spin | ONLINE |  |  |
| jupiter2019 | Anabbela2026 | ONLINE |  |  |
| jupiter2019 | Anabbela2025 | ONLINE |  |  |
| jupiter2019 | TreningOrteo_new | ONLINE |  |  |
| jupiter2019 | EnergijaGradecDBF2026 | ONLINE |  |  |
| jupiter2019 | AppDistribution | ONLINE |  |  |
| jupiter2022 | master | ONLINE |  |  |
| jupiter2022 | tempdb | ONLINE |  |  |
| jupiter2022 | model | ONLINE |  |  |
| jupiter2022 | msdb | ONLINE |  |  |
| jupiter2022 | JupiterResources | ONLINE |  |  |
| jupiter2022 | JupiterSrv | ONLINE |  |  |
| jupiter2022 | JupiterSystem | ONLINE |  |  |
| jupiter2022 | JupiterTranslations | ONLINE |  |  |
| jupiter2022 | JupiterXML | ONLINE |  |  |
| jupiter2022 | Spin | ONLINE |  |  |
| jupiter2022 | Tusak | ONLINE | tusal | da |
| jupiter2022 | PCC | ONLINE |  |  |
| jupiter2022 | PCCDMS | ONLINE |  |  |
| jupiter2022 | JetSet | ONLINE |  |  |
| jupiter2022 | JupiterSrvJetSet | ONLINE |  |  |
| jupiter2022 | JupiterSrv10 | ONLINE |  |  |
| jupiter2022 | Jupiter10 | ONLINE |  |  |
| jupiter2022 | JupiterSrvBackup | ONLINE |  |  |
| jupiter2022 | ObukaJetSet | ONLINE |  |  |
| jupiter2022 | JETSETDBFold1 | ONLINE |  |  |
| jupiter2022 | JupiterChat | ONLINE |  |  |
| jupiter2022 | ZdenoDBF | ONLINE |  |  |
| jupiter2022 | FidusgrupaDBF | ONLINE |  |  |
| jupiter2022 | JetSetDBF | ONLINE |  |  |
| jupiter2022 | Usluga | ONLINE |  |  |
| jupiter2022 | Setcor | ONLINE | setcor | da |
| jupiter2022 | SetcorDMS | ONLINE |  |  |
| jupiter2022 | JupiterSrvSetcor | ONLINE |  |  |
| jupiter2022 | CentarTehnike | ONLINE | centartehnike | da |
| jupiter2022 | Grga | ONLINE |  |  |
| jupiter2022 | JS | ONLINE |  |  |
| jupiter2022 | JS1 | ONLINE |  |  |
| jupiter2022 | CentarTehnikeSnapshot | ONLINE |  |  |
| jupiter2022 | NL_Circ_man | ONLINE |  |  |
| jupiter2022 | NL_ISMarketing | ONLINE |  |  |
| jupiter2022 | NL_ISTisak | ONLINE |  |  |
| jupiter2022 | NL_Evidar | ONLINE |  |  |
| jupiter2022 | PrijenosBlagajneSpin | ONLINE |  |  |
| jupiter2022 | NoviList | ONLINE |  |  |
| jupiter2022 | NoviListDBF | ONLINE |  |  |
| jupiter2022 | HIK | ONLINE |  |  |
| jupiter2022 | Josic | ONLINE |  |  |
| jupiter2022 | JosicDBF | ONLINE |  |  |
| jupiter2022 | JupiterHelpGanymede | ONLINE |  |  |
| jupiter2022 | SpinTomic | ONLINE |  |  |
| jupiter2022 | Studio | ONLINE |  |  |
| jupiter2022 | SpinDMS | ONLINE |  |  |
| jupiter2022 | Compost | ONLINE |  |  |
| jupiter2022 | TestValentino | ONLINE |  |  |
| jupiter2022 | BeljePlus | ONLINE |  |  |
| jupiter2022 | Connex | ONLINE |  |  |
| jupiter2022 | PrijenosBlagajneGrga | ONLINE |  |  |
| jupiter2022 | PrijenosBlagajneCentarTehnike | ONLINE |  |  |
| jupiter2022 | AppDistribution | ONLINE |  |  |
| jupiter2022 | Panissimo | ONLINE | panissimo | da |
| jupiter2022 | SpinTomicRAG | ONLINE |  |  |
| jupiter2022 | JupiterLogs | ONLINE |  |  |
| jupiter2022 | HelpdeskHelper | ONLINE |  |  |
| jupiter2022 | TreningPoljoprivrednik | ONLINE |  |  |

## Srodna imena, nisu citana

Baza sadrzi kljuc, ali ime nije tocno. Nije bilo `SELECT` nad `ssMeni`.

| Server | Baza | Kljuc |
|---|---|---|
| jupiter2019 | SetcorDMS | setcor |
| jupiter2019 | fidusgrupa | fidus |
| jupiter2019 | IreksCRMCro | ireks |
| jupiter2019 | IreksCRMCroDMS | ireks |
| jupiter2019 | IreksCRMSrb | ireks |
| jupiter2022 | FidusgrupaDBF | fidus |
| jupiter2022 | SetcorDMS | setcor |
| jupiter2022 | JupiterSrvSetcor | setcor |
| jupiter2022 | CentarTehnikeSnapshot | centartehnike |
| jupiter2022 | PrijenosBlagajneCentarTehnike | centartehnike |

## XML u izborniku

Jedan red je jedno ime datoteke. Broj baza broji razlicite baze, ne retke izbornika. Biovega je oznacena posebno. Putanja je relativna na korijen repozitorija, samo `definitions/netgen`.

| XML | Baza | Biovega | Ulaz | Putanja u meniju | Naziv stavke | definitions/netgen | Checklist |
|---|---:|:---:|---|---|---|---|---|
| `Bilanca-GFI.xml` | 10 | **Biovega** | loader | Bilanca-GFI.xml | Bilanca GFI; Bilanca-GFI                   ; Bilanca-GFI                             ; Bilanca-GFI | definitions/netgen/Agronom granule/Bilanca-GFI.XML; definitions/netgen/BIOVEGA/Bilanca-GFI.xml | nema |
| `Intrastat.xml` | 10 | **Biovega** | loader | MER\Intrastat.xml; Intrastat.xml | Intrastat; Intrastat                     ; Intrastat | definitions/netgen/BIOVEGA/Intrastat.xml | nema |
| `KntOPZ-STAT-1.xml` | 10 | **Biovega** | loader | KntOPZ-STAT-1.xml | OPZ-STAT-1; OPZ-STAT-1                    ; OPZ-STAT-1 | definitions/netgen/Agronom granule/KntOPZ-STAT-1.xml; definitions/netgen/BIOVEGA/KntOPZ-STAT-1.xml | nema |
| `KntPDVObrasciEU.xml` | 10 | **Biovega** | loader | KntPDVObrasciEU.xml | EU obrasci; EU obrasci                    ; EU obrasci | definitions/netgen/Agronom granule/KntPDVObrasciEU.xml; definitions/netgen/BIOVEGA/KntPDVObrasciEU.xml | nema |
| `KntR2racuni1.xml` | 10 | **Biovega** | loader | KntR2racuni1.xml | R2 Računi; R2 Računi                     ; R2 Računi | definitions/netgen/Agronom granule/KntR2racuni1.xml; definitions/netgen/BIOVEGA/KntR2racuni1.xml | nema |
| `MPFiskalPubVars.xml` | 10 | **Biovega** | loader | MPFiskalPubVars.xml | Parametri fiskalizacje; Parametri fiskalizacije       ; Parametri fiskalizacije                 ; Parametri fiskalizacij | definitions/netgen/BIOVEGA/MPFiskalPubVars.xml | nema |
| `MPKntNacinPlacAzur.xml` | 10 | **Biovega** | loader | MPKntNacinPlacAzur.xml | Načini plaćanja u MP; Načini plačanja u MP          ; Načini plaćanja u MP | definitions/netgen/Agronom granule/MPKntNacinPlacAzur.xml; definitions/netgen/BIOVEGA/MPKntNacinPlacAzur.xml | nema |
| `MPPDV.xml` | 10 | **Biovega** | loader | MPPDV.xml; UTP\MPPDV.xml | Roba – PDV stopa; Roba – PDV stopa              ; Roba – PDV stopa | definitions/netgen/Agronom granule/MPPDV.xml; definitions/netgen/BIOVEGA/MPPDV.xml | nema |
| `MPRACNEFISK.xml` | 10 | **Biovega** | loader | MPRACNEFISK.xml | Pregled nefiskaliziranih računa; Pregled nefisk. računa        ; Pregled nefisk. računa                  ; Preg. nefisk. | definitions/netgen/Agronom granule/MPRACNEFISK.xml; definitions/netgen/BIOVEGA/MPRACNEFISK.xml | nema |
| `Prijemnilist.xml` | 10 | **Biovega** | loader | Prijemnilist.xml; MER\PrijemniList.xml; sklad\PrijemniList.xml | Prijemni list NG; Prijemni list                 ; Prijemni list                           ; Prijemni list; …+1 | definitions/netgen/Agronom granule/PrijemniList.xml | docs/granule/PrijemniList.md |
| `TecRazOTV.xml` | 10 | **Biovega** | loader | TecRazOTV.xml | Teč. razlike na otv. stavke; Teč razlike na Otv. stavke NG ; Teč razlike na Otv. stavke              ; Teč razlike na Ot | NEDOSTAJE | nema |
| `DrzaveNG.xml` | 9 | **Biovega** | loader | UTP\DrzaveNG.xml; DrzaveNG.xml | Države; Države                        ; Države | definitions/netgen/Agronom granule/DrzaveNG.xml; definitions/netgen/BIOVEGA/DrzaveNG.xml | nema |
| `MPKaseOperNG.xml` | 9 | **Biovega** | loader | MPKaseOperNG.xml | KASA - > Operater; KASA - > Operater             ; KASA - > Operater                       ; KASA -  Operater | definitions/netgen/Agronom granule/MPKaseOperNG.xml; definitions/netgen/BIOVEGA/MPKaseOperNG.xml | nema |
| `Poruke.xml` | 9 | **Biovega** | loader | Poruke.xml | Poruke; Standardne napomene           ; Standardne napomene                     ; Standardne napomene | NEDOSTAJE | nema |
| `RealizacijaStavkeNG.xml` | 9 |  | loader | RealizacijaStavkeNG.xml; MER\RealizacijaStavkeNG.xml | Realizacija stavke            ; Realizacija Stavke                      ; Realizacija stavke; Realizacija otpreme - stav | definitions/netgen/Agronom granule/RealizacijaStavkeNG.xml | nema |
| `AOPNovi.xml` | 8 | **Biovega** | loader | AOPNovi.xml | AOP Kreator; AOP Kreator | definitions/netgen/Agronom granule/AOPNovi.xml; definitions/netgen/BIOVEGA/AOPNovi.xml | docs/granule/AOPNovi.md |
| `InstrumentiPlacanjaNG.xml` | 8 | **Biovega** | loader | InstrumentiPlacanjaNG.xml; MER\InstrumentiPlacanjaNG.xml | Instrumenti plaćanja; Instrumenti plaćanja          ; Ažuriranje naćina plaćanja    ; Instrumenti plaćanja | definitions/netgen/BIOVEGA/InstrumentiPlacanjaNG.xml | nema |
| `KOD_RSPostavke.xml` | 8 | **Biovega** | loader | KOD_RSPostavke.xml | Postavke RS; Postavke RS                   ; Postavke statusa doprinosa              ; Postavke statusa doprinosa; …+1 | definitions/netgen/Agronom granule/KOD_RSPostavke.xml; definitions/netgen/BIOVEGA/KOD_RSPostavke.xml | nema |
| `MatTem.xml` | 8 | **Biovega** | loader | MatTem.xml | Materijalna temeljnica; Materijalna temeljnica NG               ; Materijalna temeljnica NG | definitions/netgen/Agronom granule/MatTem.xml; definitions/netgen/BIOVEGA/MatTem.xml | nema |
| `MPKnjigaPopisa.xml` | 8 | **Biovega** | loader+frm | MPKnjigaPopisa.xml | Knjiga popisa u MP; Knjiga popisa; Knjiga popisa                           ; Knjiga popisa u MP                      ; … | definitions/netgen/Agronom granule/MPKnjigaPopisa.xml; definitions/netgen/BIOVEGA/MPKnjigaPopisa.xml | nema |
| `PDVPromjenaNEW.xml` | 8 | **Biovega** | loader | PDVPromjenaNEW.xml | Promjena stope PDV-a; Promjena stope PDV-a          ; Promjena stope PDV-a | definitions/netgen/Agronom granule/PDVPromjenaNEW.xml; definitions/netgen/BIOVEGA/PDVPromjenaNEW.xml | nema |
| `SkladisteNG.xml` | 8 | **Biovega** | loader | SkladisteNG.xml | Skladišta; Skladišta                     ; Skladišta                               ; Skladišta NG | definitions/netgen/Agronom granule/SkladisteNG.xml; definitions/netgen/BIOVEGA/SkladisteNG.xml | nema |
| `vrcjenik.xml` | 8 | **Biovega** | loader | MER\vrcjenik.xml | Vrste cjenika; Vrste cjenika | definitions/netgen/Agronom granule/VrCjenik.xml | nema |
| `ZupanijeNG.xml` | 8 | **Biovega** | loader | UTP\ZupanijeNG.xml; ZupanijeNG.xml | Županije; Županije                      ; Županije | definitions/netgen/Agronom granule/ZupanijeNG.xml; definitions/netgen/BIOVEGA/ZupanijeNG.xml | nema |
| `RealizacijaFakture.xml` | 8 |  | loader | RealizacijaFakture.xml; MER\RealizacijaFakture.xml | Realitacije fakture - stavke  ; Realizacija faktura                     ; Realizacija fakture                     ; Real | definitions/netgen/Agronom granule/RealizacijaFakture.xml | nema |
| `KntKonfShemeMatTem.xml` | 7 | **Biovega** | loader | KntKonfShemeMatTem.xml | Konfigurator Mat Tem; Konfigurator Mat Tem                    ; konfigurator sheme | definitions/netgen/Agronom granule/KntKonfShemeMatTem.xml; definitions/netgen/BIOVEGA/KntKonfShemeMatTem.xml | nema |
| `MPAzurPubVars.xml` | 7 | **Biovega** | loader | MPAzurPubVars.xml | Ažuriranje parametara MP; Ažuriranje parametara MP      ; Ažur. parametara MP                     ; Ažuriranje parametar | definitions/netgen/Agronom granule/MPAzurPubVars.xml; definitions/netgen/BIOVEGA/MPAzurPubVars.xml | nema |
| `MPEurParam.xml` | 7 | **Biovega** | loader | MPEurParam.xml | Parametri EUR-a; Parametri za EUR              ; Parametri za EUR                        ; Parametri za EUR | definitions/netgen/Agronom granule/MPEurParam.xml; definitions/netgen/BIOVEGA/MPEurParam.xml | nema |
| `MPInventura.xml` | 7 | **Biovega** | loader | MPInventura.xml | Maloprodajna inventura; Inventura MP                  ; Inventura MP                            ; Maloprodajna inventura | definitions/netgen/Agronom granule/MPInventura.xml; definitions/netgen/BIOVEGA/MPInventura.xml | docs/granule/MPInventura.md |
| `MPKase.xml` | 7 | **Biovega** | loader | MPKase.xml | Definicija MP kasa; Definicija kasa - NOVO        ; Parametri kasa - centralno              ; Definicija kasa | definitions/netgen/BIOVEGA/MPKase.xml | nema |
| `MPNarPredis.xml` | 7 | **Biovega** | loader | MPNarPredis.xml | Narudžba iz druge trgovine; Narudžba iz druge trgovine              ; Narudžba iz druge trg. | definitions/netgen/BIOVEGA/MPNarPredis.xml | nema |
| `MPPSNG.xml` | 7 | **Biovega** | loader | MPPSNG.xml | Početno stanje MP; Početno stanje u MP           ; Početno stanje MP                       ; PS u MP; …+1 | definitions/netgen/BIOVEGA/MPPSNG.xml | nema |
| `MPStanje.xml` | 7 | **Biovega** | frm | MPStanje.xml | Stanje MP; Stanje robe u MP              ; Stanje MP | definitions/netgen/Agronom granule/MPStanje.xml; definitions/netgen/BIOVEGA/MPStanje.xml | nema |
| `NacinOtpremeNG.xml` | 7 | **Biovega** | loader | UTP\NacinOtpremeNG.xml; MER\NacinOtpremeNG.xml; NacinOtpremeNG.xml | Način otpreme; Način otpreme                 ; Način otpreme | definitions/netgen/BIOVEGA/NacinOtpremeNG.xml | nema |
| `Narudzbe.xml` | 7 | **Biovega** | loader | skt\Narudzbe.xml; MER\Narudzbe.xml; SKLAD\Narudzbe.xml | Narudžbe; Narudžbe                                ; Narudzbe                                ; Narudzbe | definitions/netgen/Agronom granule/Narudzbe.xml; definitions/netgen/BIOVEGA/Narudzbe.xml | docs/granule/Narudzbe.md |
| `PorukeDoc.xml` | 7 | **Biovega** | loader | PorukeDoc.xml | Tekstovi na KomDoc; Tekstovi na KomDoc | NEDOSTAJE | nema |
| `FaktureMinMaxDat.xml` | 7 |  | loader+frm | FaktureMinMaxDat.xml | Fakture min-max datum                   ; Fakture min-max datum | definitions/netgen/Agronom granule/FaktureMinMaxDat.xml; definitions/netgen/BIOVEGA/FaktureMinMaxDat.xml | nema |
| `KntKompVerizne.xml` | 7 |  | loader | KntKompVerizne.xml | Verižne kompenzacije                    ; Verižne kompenzacije | definitions/netgen/Agronom granule/KntKompVerizne.xml | docs/granule/KntKompVerizne.md |
| `MPTrgOprNG.xml` | 7 |  | loader | MPTrgOprNG.xml | Trgovina / Operateri          ; Operateri - trgovina                    ; Operateri - trgovina | definitions/netgen/Agronom granule/MPTrgOprNG.xml; definitions/netgen/BIOVEGA/MPTrgOprNG.xml | nema |
| `SklMinMaxDat.xml` | 7 |  | loader+frm | SklMinMaxDat.xml | Skl dok min - max datum                 ; Skl dok min - max datum; Skladište - min max datum | definitions/netgen/Agronom granule/SklMinMaxDat.xml; definitions/netgen/BIOVEGA/SklMinMaxDat.xml | nema |
| `CarinskaTarifaNG.xml` | 6 | **Biovega** | loader | CarinskaTarifaNG.xml; UTP\CarinskaTarifaNG.xml | Carinska tarifa; Carinska tarifa               ; Carinska tarifa | definitions/netgen/BIOVEGA/CarinskaTarifaNG.xml | nema |
| `JeziciNG.xml` | 6 | **Biovega** | loader | UTP\JeziciNG.xml; JeziciNG.xml | Jezici; Jezici | definitions/netgen/Agronom granule/JeziciNG.xml; definitions/netgen/BIOVEGA/JeziciNG.xml | docs/granule/JeziciNG.md |
| `KntBlaDok.xml` | 6 | **Biovega** | loader | KntBlaDok.xml | Blagajnički dokumenti         ; Blagajnički dokumenti                   ; Blagajnički dokumenti | definitions/netgen/Agronom granule/KntBlaDok.xml | nema |
| `KntBlaPromet.xml` | 6 | **Biovega** | loader | KntBlaPromet.xml | Promet blagajne               ; Promet blagajne                         ; Promet blagajne | definitions/netgen/Agronom granule/KntBlaPromet.xml; definitions/netgen/BIOVEGA/KntBlaPromet.xml | docs/granule/KntBlaPromet.md |
| `KntBlaVrste.xml` | 6 | **Biovega** | loader | KntBlaVrste.xml | Vrste blagajni                ; Vrste blagajni                          ; Vrste blagajni | NEDOSTAJE | nema |
| `KntBlaZak.xml` | 6 | **Biovega** | loader | KntBlaZak.xml | Zaključak blagajne            ; Zaključak blagajne                      ; Zaključak blagajne | definitions/netgen/Agronom granule/KntBlaZak.xml; definitions/netgen/BIOVEGA/KntBlaZak.xml | docs/granule/KntBlaZak.md |
| `KOD_RAD1.xml` | 6 | **Biovega** | frm | KOD_RAD1.xml | RAD-1; RAD1                          ; RAD-1 | definitions/netgen/Agronom granule/KOD_RAD1.xml; definitions/netgen/BIOVEGA/KOD_RAD1.xml | nema |
| `ManagerObaveza.xml` | 6 | **Biovega** | loader | ManagerObaveza.xml; MER\ManagerObaveza.xml | _Manager obveza; Manager obveza                          ; Manager obveza | definitions/netgen/BIOVEGA/ManagerObaveza.xml | nema |
| `MjestaNG.xml` | 6 | **Biovega** | loader | UTP\MjestaNG.xml | Mjesta; Mjesta | definitions/netgen/Agronom granule/MjestaNG.xml; definitions/netgen/BIOVEGA/MjestaNG.xml | nema |
| `MPIzduMPZaprimi.xml` | 6 | **Biovega** | loader | MPIzduMPZaprimi.xml | Zaprimanje robe sa skladišta; Zaprimanje iz Skladišta       ; Zaprimanje robe iz VP u MP    ; Zaprimanje robe iz VP u MP | definitions/netgen/Agronom granule/MPIzduMPZaprimi.xml; definitions/netgen/BIOVEGA/MPIzduMPZaprimi.xml | nema |
| `MPKonUtNG.xml` | 6 | **Biovega** | loader | MPKonUtNG.xml | Analiza prebačenih utržaka; Analiza prenesenih utržaka    ; Analiza prenesenih utržaka | definitions/netgen/Agronom granule/MPKonUtNG.xml; definitions/netgen/BIOVEGA/MPKonUtNG.xml | nema |
| `MPNajnizaMPC.xml` | 6 | **Biovega** | loader | MPNajnizaMPC.xml | Najniža MPC u N dana; Najniža MPC                   ; Najniža MPC                             ; Najniža MPC | definitions/netgen/Agronom granule/MPNajnizaMPC.xml; definitions/netgen/BIOVEGA/MPNajnizaMPC.xml | nema |
| `MPNGPregDok.xml` | 6 | **Biovega** | loader | MPNGPregDok.xml | Pregled dokumenata; Pregled dokumenata MP; Pregled dok. MP - bez MAIL; Pregled dokumenata MP NOVO    ; …+2 | definitions/netgen/Agronom granule/MPNGPregDok.xml; definitions/netgen/BIOVEGA/MPNGPregDok.xml | nema |
| `MPPosMeni.xml` | 6 | **Biovega** | loader | MPPosMeni.xml | POS MENU; Izbornik POS-a                ; Izbornik POS-a | definitions/netgen/Agronom granule/MPPOSmeni.xml; definitions/netgen/BIOVEGA/MPPOSmeni.xml | nema |
| `MPPredispIz.xml` | 6 | **Biovega** | loader | MPPredispIz.xml | MP predispozicija IZLAZ; MP Predispozicija - IZLAZ     ; MP Predispozicija - IZLAZ               ; MP Predispozicija - I | definitions/netgen/Agronom granule/MPPredispIz.xml; definitions/netgen/BIOVEGA/MPPredispIz.xml | nema |
| `MPPredispUl.xml` | 6 | **Biovega** | loader | MPPredispUl.xml | MP predispozicija ULAZ; MP Predispozicija - ULAZ      ; MP Predispozicija - ULAZ                ; MP Predispozicija - UL | definitions/netgen/Agronom granule/MPPredispUl.xml; definitions/netgen/BIOVEGA/MPPredispUl.xml | nema |
| `MPPrInvNG.xml` | 6 | **Biovega** | loader | MPPrInvNG.xml | Pregled inventure MP; Pregled inventure MP          ; Pregled inventure MP | definitions/netgen/Agronom granule/MPPrInvNG.xml; definitions/netgen/BIOVEGA/MPPrInvNG.xml | nema |
| `MPPrMPCNG.xml` | 6 | **Biovega** | loader | MPPrMPCNG.xml | Pregled promjena MPC; Pregled promjena mpc          ; Pregled promjena MPC | definitions/netgen/Agronom granule/MPPrMPCNG.xml; definitions/netgen/BIOVEGA/MPPrMPCNG.xml | nema |
| `MPPrMPRNG.xml` | 6 | **Biovega** | loader | MPPrMPRNG.xml | Pregled MP računa; Pregled MP računa             ; Pregled MP računa | definitions/netgen/BIOVEGA/MPPrMPRNG.xml | nema |
| `MPPrR0NG.xml` | 6 | **Biovega** | loader | MPPrR0NG.xml | Pregled stornirane robe; Preg. stornirane robe na MP ra; Preg. stornirane robe na MP ra | definitions/netgen/Agronom granule/MPPrR0NG.xml; definitions/netgen/BIOVEGA/MPPrR0NG.xml | nema |
| `MPTrgCjenNG.xml` | 6 | **Biovega** | loader | MPTrgCjenNG.xml | Ažuriranje Trgovina/Cjenik; Trgovina - MP Cjenik          ; Veza trgovine s mp cjenikom             ; Ažuriranje Trgovin | definitions/netgen/Agronom granule/MPTrgCjenNG.xml; definitions/netgen/BIOVEGA/MPTrgCjenNG.xml | nema |
| `MPTrgovinaPortal.xml` | 6 | **Biovega** | frm | MPTrgovinaPortal.xml | Trgovina PORTAL; Trgovina PORTAL               ; Trgovina PORTAL | definitions/netgen/Agronom granule/MPTrgovinaPortal.xml; definitions/netgen/BIOVEGA/MPTrgovinaPortal.xml | nema |
| `MPUpdateKase.xml` | 6 | **Biovega** | loader | MPUpdateKase.xml | Update kasa; Update blagajni               ; Update blagajni | definitions/netgen/Agronom granule/MPUpdateKase.xml; definitions/netgen/BIOVEGA/MPUpdateKase.xml | nema |
| `MPUtrNacPlcNG.xml` | 6 | **Biovega** | loader | MPUtrNacPlcNG.xml | Pregled utržaka po načinu plaćanja; Pregled utržaka po načinu plać; Rekap. po nač.plaćanja        ; Pregled utržaka po n | definitions/netgen/Agronom granule/MPUtrNacPlcNG.xml; definitions/netgen/BIOVEGA/MPUtrNacPlcNG.xml | nema |
| `MPUvjetiKase.xml` | 6 | **Biovega** | loader | MPUvjetiKase.xml | Uvjeti plaćanja na kasi (stara); Definiranje uvjeta na blagajni; Uvjeti plaćanja na kasi | definitions/netgen/Agronom granule/MPUvjetiKase.xml; definitions/netgen/BIOVEGA/MPUvjetiKase.xml | nema |
| `MPVrstaDokAzur.xml` | 6 | **Biovega** | loader | MPVrstaDokAzur.xml | Ažuriranje vrsta dok.; Ažuriranje vrsta dokumenata   ; Ažuriranje vrsta dok.         ; Ažuriranje vrsta dokumenta | definitions/netgen/BIOVEGA/MPVrstaDokAzur.xml | nema |
| `MPZakljPerioda.xml` | 6 | **Biovega** | loader+frm | MPZakljPerioda.xml | Zaključak perioda; Zaključak perioda NG          ; Zaključak perioda MP                    ; Zaključak perioda MP | definitions/netgen/Agronom granule/MPZakljPerioda.xml; definitions/netgen/BIOVEGA/MPZakljPerioda.xml | nema |
| `Obrazac.xml` | 6 | **Biovega** | loader | Obrazac.xml | Obrazac; Obrazac                       ; Obrazac IP1; Obrazac IP1 | definitions/netgen/Agronom granule/Obrazac.xml; definitions/netgen/BIOVEGA/Obrazac.xml | nema |
| `OdjeliNG.xml` | 6 | **Biovega** | loader | UTP\OdjeliNG.xml; OdjeliNG.xml | Odjeli; Odjeli | definitions/netgen/BIOVEGA/OdjeliNG.xml | nema |
| `PartneriAuditNG.xml` | 6 | **Biovega** | loader | PartneriAuditNG.xml | Povijest izmjene partnera; Povijest izmjene partnera               ; Povijest izmjene partnera n             ; Povijest | definitions/netgen/Agronom granule/PartneriAuditNG.xml; definitions/netgen/BIOVEGA/PartneriAuditNG.xml | nema |
| `PNKatalozi.xml` | 6 | **Biovega** | loader | PNKatalozi.xml | Katalozi                      ; Katalozi                                ; Katalozi | definitions/netgen/Agronom granule/PNKatalozi.xml; definitions/netgen/BIOVEGA/PNKatalozi.xml | nema |
| `PNObracun.xml` | 6 | **Biovega** | loader | PNObracun.xml | Obračun putnih naloga         ; Obračun putnih naloga                   ; Putni nalozi | definitions/netgen/Agronom granule/PNObracun.xml; definitions/netgen/BIOVEGA/PNObracun.xml | nema |
| `PostTituleNG.xml` | 6 | **Biovega** | loader | UTP\PostTituleNG.xml; PostTituleNG.xml | Post-Titule; Post-Titule | definitions/netgen/BIOVEGA/PostTituleNG.xml | nema |
| `PredTituleNG.xml` | 6 | **Biovega** | loader | UTP\PredTituleNG.xml; PredTituleNG.xml | Pred-Titule; Pred-Titule | definitions/netgen/BIOVEGA/PredTituleNG.xml | nema |
| `ValuteNG.xml` | 6 | **Biovega** | loader | UTP\ValuteNG.xml | Valute; Valute | definitions/netgen/BIOVEGA/ValuteNG.xml | nema |
| `Akcije.xml` | 6 |  | loader | Akcije.xml | Dodjela prava                           ; Akcije                                  ; Akcije | definitions/netgen/Agronom granule/Akcije.xml; definitions/netgen/BIOVEGA/Akcije.xml | nema |
| `MPRealizNG.xml` | 6 |  | loader | MPRealizNG.xml | Realizacija robe u MP         ; Realizacija robe u MP NG                ; Realizacija robe u MP; Realizacija robe u MP | definitions/netgen/Agronom granule/MPRealizNG.xml | nema |
| `MPTrgovine.xml` | 6 |  | loader | MPTrgovine.xml | Trgovine                      ; Trgovine                                ; Trgovine | NEDOSTAJE | nema |
| `ZakljucivanjeTemNG.xml` | 6 |  | loader+frm | ZakljucivanjeTemNG.xml | Zaključivanje temeljnica                ; Zaključivanje temeljnica | definitions/netgen/Agronom granule/ZakljucivanjeTemNG.xml | nema |
| `DobUgovori.xml` | 5 | **Biovega** | loader | DobUgovori.xml; MER\DobUgovori.xml | Ugovori s dobavljačima; Ugovori s dobavljačima                  ; Dobavljači ugovori | definitions/netgen/Agronom granule/DobUgovori.xml; definitions/netgen/BIOVEGA/DobUgovori.xml | docs/granule/DobUgovori.md |
| `HROsobeUcinak.xml` | 5 | **Biovega** | loader | HROsobeUcinak.xml | Povezivanje kadrova; Povezivanje kadrova | definitions/netgen/Agronom granule/HROsobeUcinak.xml; definitions/netgen/BIOVEGA/HROsobeUcinak.xml | nema |
| `KntBrutoBilancaNG.xml` | 5 | **Biovega** | loader | KntBrutoBilancaNG.xml | Bruto bilanca - period; Bruto bilanca                 ; Bruto bilanca | definitions/netgen/Agronom granule/KntBrutoBilancaNG.xml; definitions/netgen/BIOVEGA/KntBrutoBilancaNG.xml | nema |
| `KntPreknjizavanjeNG.xml` | 5 | **Biovega** | loader | KntPreknjizavanjeNG.xml | Knjiženje s konta na konto; Knjiženje s konta na konto NG; Knjiženje s konta na konto | definitions/netgen/Agronom granule/KntPreknjizavanjeNG.xml; definitions/netgen/BIOVEGA/KntPreknjizavanjeNG.xml | nema |
| `KOD_Relacije.xml` | 5 | **Biovega** | frm | KOD_Relacije.xml | Prijevoz - relacije; Prijevoz - relacije           ; Prijevoz - relacije | definitions/netgen/Agronom granule/KOD_Relacije.xml; definitions/netgen/BIOVEGA/KOD_Relacije.xml | nema |
| `KUPPregOdob.xml` | 5 | **Biovega** | loader | KUPPregOdob.xml | KUP pregled odobravanja | definitions/netgen/BIOVEGA/KUPPregOdob.xml | nema |
| `MPDnevniProm.xml` | 5 | **Biovega** | loader | MPDnevniProm.xml | Dnevni promet blagajni; Pregled dnevnog prometa       ; Dnevni promet trgovina | definitions/netgen/BIOVEGA/MPDnevniProm.xml | nema |
| `MPKasaPolog.xml` | 5 | **Biovega** | loader+frm | MPKasaPolog.xml | Polog MP blagajne; Polog MP blagajne             ; Polog POS blagajne | definitions/netgen/Agronom granule/MPKasaPolog.xml; definitions/netgen/BIOVEGA/MPKasaPolog.xml | nema |
| `MPNivelacija.xml` | 5 | **Biovega** | loader | MPNivelacija.xml | Nivelacije; Nivelacije u MP; Nivelacija u MP; Nivelacija u MP               ; …+2 | definitions/netgen/BIOVEGA/MPNivelacija.xml | nema |
| `MPnivlCKNG.xml` | 5 | **Biovega** | loader | MPnivlCKNG.xml | Promjena MPC po klasi; Promjena mpc u % po klasi     ; Promjena MP cijena po klasi | definitions/netgen/Agronom granule/MPnivlCKNG.xml; definitions/netgen/BIOVEGA/MPnivlCKNG.xml | nema |
| `MPPrZakPerNG.xml` | 5 | **Biovega** | loader | MPPrZakPerNG.xml | Pregled zaključaka perioda; Pregled zaključaka perioda MP ; Pregled zaključaka perioda | definitions/netgen/BIOVEGA/MPPrZakPerNG.xml | nema |
| `ZahtjevZaIzdavanje.xml` | 5 | **Biovega** | loader | skt\ZahtjevZaIzdavanje.xml; MER\ZahtjevZaIzdavanje.xml | Zahtjev za izdavanje; Zahtjev za izdavanje          ; Zahtjev za izdavanje | NEDOSTAJE | nema |
| `FaktureNeFisk.xml` | 5 |  | loader | FaktureNeFisk.xml; MER\FaktureNeFisk.xml | Fiskalizacija VP faktura      ; Fiskalizacija VP Faktura                ; Fiskalizacija VP faktura | definitions/netgen/Agronom granule/FaktureNeFisk.xml; definitions/netgen/BIOVEGA/FaktureNeFisk.xml | nema |
| `KntIzvodTijek.xml` | 5 |  | loader | KntIzvodTijek.xml | Tijek novca-direktna metoda; Tijek novca direktna metoda; Tijek novca-direktna metoda | NEDOSTAJE | nema |
| `KntPriGen.xml` | 5 |  | loader | KntPriGen.xml | Prijeboji generator                     ; Prijeboji generator | NEDOSTAJE | nema |
| `KntZatvaranjeNG.xml` | 5 |  | loader+frm | KntZatvaranjeNG.xml | Zatvaranje klase 4 i 7                  ; Zatvaranje klase 4 i 7 | NEDOSTAJE | nema |
| `KUPVrstPN.xml` | 5 |  | loader | KUPVrstPN.xml | KUP Vrsta pošte; Vrsta pošte; KUP Vrsta dok | definitions/netgen/Agronom granule/KUPVrstPN.xml | nema |
| `ManagerCjenika.xml` | 5 |  | frm | MER\ManagerCjenika.xml | Manager cjenika                         ; Manager cjenika | definitions/netgen/Agronom granule/ManagerCjenika.xml | nema |
| `ManPokazateljiGrupe.xml` | 5 |  | loader | ManPokazateljiGrupe.xml | Grupe pokazatelja                       ; Grupe pokazatelja | NEDOSTAJE | nema |
| `MPInvDonNG.xml` | 5 |  | loader | MPInvDonNG.xml | MP inventura - DONOS          ; MP Inventura - POPIS                    ; MP Inventura - DONOS | definitions/netgen/Agronom granule/MPInvDonNG.xml | nema |
| `MPnivelCNG.xml` | 5 |  | loader | MPnivelCNG.xml | MP Cjenik - Izmjena           ; MP Cjenik - Izmjena                     ; MP Cjenik - Izmjena | definitions/netgen/Agronom granule/MPnivelCNG.xml; definitions/netgen/BIOVEGA/MPnivelCNG.xml | nema |
| `MPOtpremnica.xml` | 5 |  | loader | MPOtpremnica.xml | MP Otpremnica                 ; MP Otpremnica                           ; MP Otpremnica | definitions/netgen/BIOVEGA/MPOtpremnica.xml | nema |
| `MPStanjeZaKasu.xml` | 5 |  | loader | MPStanjeZaKasu.xml | Stanje za kasu                ; Stanje za kasu                          ; Stanje za kasu | definitions/netgen/Agronom granule/MPStanjeZaKasu.xml | nema |
| `AnalitikaNabave.xml` | 4 | **Biovega** | loader | AnalitikaNabave.xml | Realizacija nabave; Realizacija nabave                      ; Analitika nabave | definitions/netgen/Agronom granule/AnalitikaNabave.xml; definitions/netgen/BIOVEGA/AnalitikaNabave.xml | nema |
| `JobMail.xml` | 4 | **Biovega** | loader | JobMail.xml | Definiranje maila-a; Izvještaji na mail                      ; Izvještaji na mail | definitions/netgen/BIOVEGA/JobMail.xml | nema |
| `kntDobDospRac.xml` | 4 | **Biovega** | loader | kntDobDospRac.xml | Prijedlog za plaćanje; Dobavljači - prijedlog za plaćanje; Dobavljači dospjelo                     ; Dobavljači - prijed | definitions/netgen/BIOVEGA/kntDobDospRac.xml | nema |
| `KntKljuceviTroska2.xml` | 4 | **Biovega** | frm | KntKljuceviTroska2.xml | Ključevi rasporeda troškova; Ključevi rasporeda troška; Ključevi rasporeda troška | definitions/netgen/BIOVEGA/KntKljuceviTroska2.xml | nema |
| `KntOdobrenje.xml` | 4 | **Biovega** | loader | KntOdobrenje.xml | PDV Odobrenja; Odobrenja PDV                           ; Odobrenja PDV | definitions/netgen/Agronom granule/KntOdobrenje.xml; definitions/netgen/BIOVEGA/KntOdobrenje.xml | nema |
| `KUPStatNLinije.xml` | 4 | **Biovega** | loader | KUPStatNLinije.xml | Status pošte; KUP Status; KUP Status Pošte; Status pošte linije | definitions/netgen/BIOVEGA/KUPStatNLinije.xml | nema |
| `MPAsortimanTip.xml` | 4 | **Biovega** | loader | MPAsortimanTip.xml | Vrste asortimana; Tip prodajnog objekta                   ; Tip prodajnog objekta | definitions/netgen/Agronom granule/MPAsortimanTip.xml; definitions/netgen/BIOVEGA/MPAsortimanTip.xml | nema |
| `MPDirZaduzenjeNG2.xml` | 4 | **Biovega** | loader | MPDirZaduzenjeNG2.xml | Direktno zaduženje MP; Direktno zaduženje; Direktno zaduženje MP         ; Dir. zaduženje MP - NOVO | definitions/netgen/BIOVEGA/MPDirZaduzenjeNG2.xml | nema |
| `MPDirZaduzenjeNG3.xml` | 4 | **Biovega** | loader | MPDirZaduzenjeNG3.xml | Kalkulacija MP; Kalkulacija u MP                        ; MP Kalkulacija | definitions/netgen/BIOVEGA/MPDirZaduzenjeNG3.xml | nema |
| `MPKonMpSkl.xml` | 4 | **Biovega** | loader | MPKonMpSkl.xml | Kontrola MP SKL; Kontrola MP -> SKL                      ; Kontrola MP->SKL | NEDOSTAJE | nema |
| `MPNeZakNG.xml` | 4 | **Biovega** | loader | MPNeZakNG.xml | Pregled nezak. dokumenata; Pregled nezaklj. dokumenata             ; Pregled nezaključenih dokumena | definitions/netgen/Agronom granule/MPNeZakNG.xml; definitions/netgen/BIOVEGA/MPNeZakNG.XML | nema |
| `MPObracunKalaNG.xml` | 4 | **Biovega** | loader | MPObracunKalaNG.xml | Obračun dozvoljenog kala u MP; Pregled dozvoljenog kala; Obračun dozvoljenog kala u MP | definitions/netgen/Agronom granule/MPObracunKalaNG.xml; definitions/netgen/BIOVEGA/MPObracunKalaNG.xml | nema |
| `MPPovUVP.xml` | 4 | **Biovega** | loader | MPPovUVP.xml | _Povratnica iz MP u VP; Povratnica u VP  - NG         ; Povratnica iz MP u VP | definitions/netgen/BIOVEGA/MPPovuVP.xml | nema |
| `OOPDef.xml` | 4 | **Biovega** | loader | OOPDef.xml | Postavke plaće; Postavke plaće | definitions/netgen/BIOVEGA/OOPDef.xml | nema |
| `PNNalozi.xml` | 4 | **Biovega** | loader | PNNalozi.xml | Putni nalozi                  ; Obrada; Putni nalozi; Putni nalozi | definitions/netgen/BIOVEGA/PNNalozi.xml | nema |
| `RobaDeklaracijeNG.xml` | 4 | **Biovega** | loader | RobaDeklaracijeNG.xml | Deklaracija Robe; Deklaracija robe | definitions/netgen/BIOVEGA/RobaDeklaracijeNG.xml | nema |
| `kntKontrolaPDV.xml` | 4 |  | loader | kntKontrolaPDV.xml | Kontrola PDV-a                          ; Kontrola PDV-a | NEDOSTAJE | nema |
| `kntPreknjizavanjePartnera.xml` | 4 |  | loader | kntPreknjizavanjePartnera.xml | Preknjižavanje partnera; Preknjižavanje partnera | definitions/netgen/Agronom granule/KntPreknjizavanjePartnera.xml | nema |
| `kntShemaKonta.xml` | 4 |  | loader | kntShemaKonta.xml | Shema konta                             ; Shema konta | definitions/netgen/Agronom granule/kntShemaKonta.xml | nema |
| `MPBlagajna.xml` | 4 |  | loader | MPBlagajna.xml | MP Blagajna                   ; Fiskalna blagajna MP iz VP; MP računi - Blagajna | definitions/netgen/BIOVEGA/MPBlagajna.xml | docs/granule/MPBlagajna.md |
| `MPIzdVPNG.xml` | 4 |  | loader | MPIzdVPNG.xml | Izdatnica u MP - NG           ; Izdatnica u MP; Izdatnica u MP | definitions/netgen/BIOVEGA/MPIzdVPNG.xml | nema |
| `MPPregARTNG.xml` | 4 |  | loader | MPPregARTNG.xml | Pregled prometa artikala u MP ; Pregled prometa rtikala                 ; Pregled prometa artikala | definitions/netgen/Agronom granule/MPPregArtNG.xml | nema |
| `MPVezCijng.xml` | 4 |  | loader | MPVezCijng.xml | Vezanje mp cjenika            ; Uređivanje cjenika MP | NEDOSTAJE | nema |
| `PDVpoStjecanju.xml` | 4 |  | loader | PDVpoStjecanju.xml | PDV po stjecanju              ; PDV po stjecanju; PDV po stjecanju | definitions/netgen/Agronom granule/PDVpoStjecanju.xml; definitions/netgen/BIOVEGA/PDVpoStjecanju.xml | nema |
| `PDVPromjena.xml` | 4 |  | loader | PDVPromjena.xml | Promjena stope PDV-a | NEDOSTAJE | nema |
| `Predujmovi.xml` | 4 |  | loader | Predujmovi.xml | Predujmovi NG; Predujmovi NG | definitions/netgen/Agronom granule/Predujmovi.xml | nema |
| `FakPar.xml` | 3 | **Biovega** | loader | FakPar.xml | Uvjeti fakturiranja; Fakturiranje uvjeti | definitions/netgen/BIOVEGA/FakPar.xml | docs/granule/FakPar.md |
| `HREvi.xml` | 3 | **Biovega** | loader | HREvi.xml | Dnevna evidencija; Dnevna evidencija | definitions/netgen/BIOVEGA/HREvi.xml | nema |
| `KOD_DopStope.xml` | 3 | **Biovega** | loader | KOD_DopStope.xml | Stope doprinosa; Stope doprinosa | definitions/netgen/Agronom granule/KOD_DopStope.xml; definitions/netgen/BIOVEGA/KOD_DopStope.xml | nema |
| `KomStanjeSkl.xml` | 3 | **Biovega** | frm | MER\KomStanjeSkl.xml | Komercijalno stanje skladišta ; Komercijalno stanje skladišta; Kom. stanje skl. NG | definitions/netgen/BIOVEGA/KomStanjeSkl.xml | nema |
| `MP4RedaRac.xml` | 3 | **Biovega** | loader | MP4RedaRac.xml | Poruke na računima; Poruke na računu | NEDOSTAJE | nema |
| `MPAnalKNG.xml` | 3 | **Biovega** | loader | MPAnalKNG.xml | Analiza kartica roba MP; Analiza kartica MP | definitions/netgen/BIOVEGA/MPAnalKNG.xml | nema |
| `MPAsortimanStruAzur.xml` | 3 | **Biovega** | loader | MPAsortimanStruAzur.xml | Asortiman; Asortiman trgovina | definitions/netgen/BIOVEGA/MPAsortimanStruAzur.xml | nema |
| `MPBankaPOSKod.xml` | 3 | **Biovega** | loader | MPBankaPOSKod.xml | MP Banka POS Kod; Kod banke za POS | NEDOSTAJE | nema |
| `MPEtiketeNG.xml` | 3 | **Biovega** | loader | MPEtiketeNG.xml | MP Etikete; MP Etikete sa dokumenta | definitions/netgen/Agronom granule/MPEtiketeNG.xml; definitions/netgen/BIOVEGA/MPEtiketeNG.xml | nema |
| `MPEtiketeTerminal.xml` | 3 | **Biovega** | loader | MPEtiketeTerminal.xml | Etikete s terminala; MP Etikete za pojedinu robu             ; Etikete s terminala | definitions/netgen/BIOVEGA/MPEtiketeTerminal.xml | nema |
| `MPIntDokZamjena.xml` | 3 | **Biovega** | loader | MPIntDokZamjena.xml | Zamjena šifri u MP; Zamjena šifri; Sa šifre na šifru | definitions/netgen/BIOVEGA/MPIntDokZamjena.xml | nema |
| `MPJMzaMPC.xml` | 3 | **Biovega** | loader | MPJMzaMPC.xml | Unos dodatne JM za MPC; Unos dodatne JM za MPC | definitions/netgen/Agronom granule/MPJMzaMPC.xml; definitions/netgen/BIOVEGA/MPJMzaMPC.xml | nema |
| `MPKOntrolaObrProizvodnje.xml` | 3 | **Biovega** | loader | MPKOntrolaObrProizvodnje.xml | Kontrola obračuna proizvodnje; Kontrola obračuna proizvodnje | definitions/netgen/BIOVEGA/MPKOntrolaObrProizvodnje.xml | nema |
| `MPKOntrolaPrijenosaKasa.xml` | 3 | **Biovega** | loader | MPKOntrolaPrijenosaKasa.xml | Kontrola prijenosa kasa; Analiza prijenoa blagajni | NEDOSTAJE | nema |
| `MPKontrolaProizvodnje.xml` | 3 | **Biovega** | loader | MPKontrolaProizvodnje.xml | Kontrola razduženja sirovina; Kontrola razduženja sirovina | definitions/netgen/BIOVEGA/MPKontrolaProizvodnje.xml | nema |
| `MPKontrolaProizvodnje2.xml` | 3 | **Biovega** | loader | MPKontrolaProizvodnje2.xml | Kontrola zaduženja proizvoda; Kontrola zaduženja proizvoda | definitions/netgen/BIOVEGA/MPKontrolaProizvodnje2.xml | nema |
| `MPKupciR1.xml` | 3 | **Biovega** | loader | MPKupciR1.xml | Kupci R1; Kupci R1 | definitions/netgen/Agronom granule/MPKupciR1.xml; definitions/netgen/BIOVEGA/MPKupciR1.xml | nema |
| `MPNarIzdSkl.xml` | 3 | **Biovega** | loader | MPNarIzdSkl.xml | Narudžba za skladište; Narudžba MP za skladište      ; Narudžba MP - SKL | definitions/netgen/BIOVEGA/MPNarIzdSkl.xml | docs/granule/MPNarIzdSkl.md |
| `MPOperTrg.xml` | 3 | **Biovega** | loader | MPOperTrg.xml | Operateri u trgovinama; Operateri u trgovinama | definitions/netgen/BIOVEGA/MPOperTrg.xml | nema |
| `MPParStru.xml` | 3 | **Biovega** | loader | MPParStru.xml | Ažuriranje GLN lokacija | NEDOSTAJE | nema |
| `MPPonudaWeb.xml` | 3 | **Biovega** | loader | MPPonudaWeb.xml | Ponuda s weba; MP ponuda WEB; WEB ponuda | definitions/netgen/BIOVEGA/MPPonudaWeb.xml | nema |
| `MPPoruke.xml` | 3 | **Biovega** | loader | MPPoruke.xml | Poruke za MP; Poruke za MP | NEDOSTAJE | nema |
| `MPPovijestPrijava.xml` | 3 | **Biovega** | loader | MPPovijestPrijava.xml | _Trgovina radno vrijeme; Radno vremena trgovina | definitions/netgen/Agronom granule/MPPovijestPrijava.xml | nema |
| `MPPregANivNG.xml` | 3 | **Biovega** | loader | MPPregANivNG.xml | Pregled A-nivelacija; Pregled Autom.nivelacija | definitions/netgen/BIOVEGA/MPPregANivNG.xml | nema |
| `MPPregkasa.xml` | 3 | **Biovega** | loader | MPPregkasa.xml | Pregled blagajni; Pregled blagajni | definitions/netgen/BIOVEGA/MPPregKasa.xml | nema |
| `MPPregNekur.xml` | 3 | **Biovega** | loader | MPPregNekur.xml | Pregled nekurentnih zaliha MP; Pregled nekurentnih zaliha MP | definitions/netgen/BIOVEGA/MPPregNekur.XML | nema |
| `MPPregProizNorm.xml` | 3 | **Biovega** | loader | MPPregProizNorm.xml | Ažuriranje normativa; Ažur. proizvoda i normativa; Pregled proizvoda i normativa | definitions/netgen/BIOVEGA/MPPregProizNorm.xml | nema |
| `MPPregPromProiz.xml` | 3 | **Biovega** | loader | MPPregPromProiz.xml | Pregled prometa proizvoda; Pregled prometa proizvoda | definitions/netgen/BIOVEGA/MPPregPromProiz.xml | nema |
| `MPPregPromSirovina.xml` | 3 | **Biovega** | loader | MPPregPromSirovina.xml | Pregled prometa sirovina; Pregled prometa sirovina | definitions/netgen/BIOVEGA/MPPregPromSirovina.xml | nema |
| `MPRazlogStorna.xml` | 3 | **Biovega** | loader | MPRazlogStorna.xml | Razlog storna MP računa; Razlozi storniranja MP raćuna | definitions/netgen/BIOVEGA/MPRazlogStorna.xml | nema |
| `MPRealiKNG.xml` | 3 | **Biovega** | loader | MPRealiKNG.xml | Realizacija MP kumulativno; Realizacija MP - kumul. NG | definitions/netgen/Agronom granule/MPRealiKNG.xml; definitions/netgen/BIOVEGA/MPRealiKNG.xml | nema |
| `MPRekDokNG.xml` | 3 | **Biovega** | loader | MPRekDokNG.xml | Rekap. prometa po vrsti dok.; Rekap. prometa po vrsti dok. | definitions/netgen/BIOVEGA/MPRekDokNG.XML | nema |
| `MPRobaPortal.xml` | 3 | **Biovega** | loader | MPRobaPortal.xml | MP roba portal; MP Roba portal | definitions/netgen/Agronom granule/MPRobaPortal.xml; definitions/netgen/BIOVEGA/MPRobaPortal.xml | nema |
| `MPRobaRNNG.xml` | 3 | **Biovega** | loader | MPRobaRNNG.xml | Pregled robe po MP računu; Preg. prod. robe po računu | definitions/netgen/Agronom granule/MPRobaRNNG.xml; definitions/netgen/BIOVEGA/MPRobaRNNG.xml | nema |
| `MPSirovine.xml` | 3 | **Biovega** | loader | MPSirovine.xml | Sirovine u MP; Sirovine MP | definitions/netgen/BIOVEGA/MPSirovine.xml | nema |
| `MPsmjene.xml` | 3 | **Biovega** | loader | MPsmjene.xml | Smjene radnog vremena u MP; Smjene radnog vremena | definitions/netgen/BIOVEGA/MPSmjene.xml | nema |
| `MPStanjeSirovina.xml` | 3 | **Biovega** | frm | MPStanjeSirovina.xml | Pregled stanja sirovina; Pregled stanja sirovina | definitions/netgen/BIOVEGA/MPStanjeSirovina.xml | nema |
| `MPUgoPDV.xml` | 3 | **Biovega** | loader | MPUgoPDV.xml | Ugostiteljski PDV; Ugostiteljski PDV | NEDOSTAJE | nema |
| `MPUgoRobaNG.xml` | 3 | **Biovega** | loader | MPUgoRobaNG.xml | Ugostiteljska roba; Ugostiteljska roba | definitions/netgen/BIOVEGA/MPUgoRobaNG.xml | nema |
| `MPUmirovNG.xml` | 3 | **Biovega** | loader | MPUmirovNG.xml | _Umirovljenje robe u MP; Umirovljenje roba u MP                  ; Umirovljenje robe u mP | definitions/netgen/Agronom granule/MPUmirovNG.xml; definitions/netgen/BIOVEGA/MPUmirovNG.xml | nema |
| `MPUslugeKasa.xml` | 3 | **Biovega** | loader | MPUslugeKasa.xml | Usluge ka kasu; Usluge za kasu | definitions/netgen/BIOVEGA/MPUslugeKasa.xml | nema |
| `MPVaganaRobaDod.xml` | 3 | **Biovega** | loader | MPVaganaRobaDod.xml | Vagana roba; Vagana roba dodatno | definitions/netgen/BIOVEGA/MPVaganaRobaDoD.xml | nema |
| `MPVaganaRobaTrg.xml` | 3 | **Biovega** | loader | MPVaganaRobaTrg.xml | Vagana roba u trgovini; Vagana roba trgovina | definitions/netgen/BIOVEGA/MPVaganaRobaTrg.xml | nema |
| `MPVaucher.xml` | 3 | **Biovega** | loader | MPVaucher.xml | Izdavanje vaucher-a; Izdavanje VAUCHER-a | definitions/netgen/BIOVEGA/MPVaucher.xml | nema |
| `PrnRNNG.xml` | 3 | **Biovega** | loader | PrnRNNG.xml; MFG/PrnRNNG.xml | Radni nalozi LT; Radni nalozi LT | definitions/netgen/BIOVEGA/PrnRNNG.xml | nema |
| `sktStariKruh.xml` | 3 | **Biovega** | loader | SKT/sktStariKruh.xml | Knjiženje starog kruha; Knjiženje starog kruha | definitions/netgen/BIOVEGA/sktStariKruh.xml | nema |
| `HRCKadManagerVanjski.xml` | 3 |  | loader | HRCKadManagerVanjski.xml | Vanjski suradnici                       ; Vanjski suradnici | definitions/netgen/Agronom granule/HRCKadManagerVanjski.xml | nema |
| `Inventura.xml` | 3 |  | loader | Inventura.xml | Inventura unos                          ; Inventura NG | NEDOSTAJE | nema |
| `IzvKlasePartneriParstru.xml` | 3 |  | loader | IzvKlasePartneriParstru.xml | Pregled klasa za partnera               ; Pregled klasa za partnera | NEDOSTAJE | nema |
| `jesteliznali.xml` | 3 |  | loader | jesteliznali.xml | Manager Zanimljivosti | NEDOSTAJE | nema |
| `JMjereNG.xml` | 3 |  | loader | UTP\JMjereNG.xml | Jedinice mjere                          ; Jedinice mjere | NEDOSTAJE | nema |
| `JOPPD.xml` | 3 |  | loader | JOPPD.xml | JOPPD                                   ; JOPPD; JOPPD | definitions/netgen/BIOVEGA/JOPPD.xml | docs/granule/JOPPD.md |
| `KarticaPartnera.xml` | 3 |  | loader | KarticaPartnera.xml | Kartica Partnera                        ; Kartica Partnera | definitions/netgen/Agronom granule/KarticaPartnera.xml | docs/granule/KarticaPartnera.md |
| `KntImportTem.xml` | 3 |  | loader | KntImportTem.xml | Excel Import Tem                        ; Excel Import Tem | definitions/netgen/Agronom granule/KntImportTem.xml | nema |
| `KntOpomene.xml` | 3 |  | frm | KntOpomene.xml | Opomene NG; Opomene NG | NEDOSTAJE | nema |
| `KntRasporedTros.xml` | 3 |  | loader | KntRasporedTros.xml | Raspored troškova; Raspored troškova | NEDOSTAJE | nema |
| `KntZakljucniListBB.xml` | 3 |  | loader | KntZakljucniListBB.xml | Bruto Bilanca | NEDOSTAJE | nema |
| `KonfigSrvRN.xml` | 3 |  | loader | KonfigSrvRN.xml | Konfigurator uslužnih RN | NEDOSTAJE | nema |
| `KUPPregNG.xml` | 3 |  | loader | KUPPregNG.xml | Pregled pošte; KUP Pregled pošte | NEDOSTAJE | nema |
| `ManGadTip.xml` | 3 |  | loader | ManGadTip.xml | Manager indikatora; Manager indikatora | NEDOSTAJE | nema |
| `MatposMrtvaTrka.xml` | 3 |  | loader | MatposMrtvaTrka.xml | Obrada robnog Međuskladišnice           ; Obrada robnog međuskladišnice | definitions/netgen/Agronom granule/MatposMrtvaTrka.xml; definitions/netgen/BIOVEGA/MatposMrtvaTrka.xml | nema |
| `MPAsortimanAzurN.xml` | 3 |  | loader | MPAsortimanAzurN.xml | Asortiman dobavljača                    ; Asortiman dobavljača | NEDOSTAJE | nema |
| `MPIntDok.xml` | 3 |  | loader | MPIntDok.xml | Interni dokumenti MP                    ; Interni dokumenti MP | definitions/netgen/Agronom granule/MPIntDok.xml | nema |
| `MPInvPregArt.xml` | 3 |  | loader | MPInvPregArt.xml | Pregled robe na inventurama             ; Pregled robe na inventurama | definitions/netgen/Agronom granule/MPInvPregArt.xml | nema |
| `MPKaloNG.xml` | 3 |  | loader | MPKaloNG.xml | Unos postotka dozvoljenog kala          ; Unos postotka kala | definitions/netgen/Agronom granule/MPKaloNG.xml | nema |
| `MPNapuniKase.xml` | 3 |  | frm | MPNapuniKase.xml | Priprema podataka za trgovinu           ; Priprema podataka za trgovine | definitions/netgen/Agronom granule/MPNapuniKase.xml | nema |
| `MPOtvCij.xml` | 3 |  | loader | MPOtvCij.xml | Otvaranje MP cjenika                    ; Otvaranje MP cjenika | definitions/netgen/Agronom granule/MPOtvCij.xml | nema |
| `MPPorKPNG.xml` | 3 |  | loader | MPPorKPNG.xml | Poravnavanje KP                         ; Poravnavanje KP | definitions/netgen/BIOVEGA/MPPorKPNG.xml | nema |
| `MPPovratDobNG.xml` | 3 |  | loader | MPPovratDobNG.xml | Povrat dobavljaču iz MP                 ; Povrat dobavljaču iz MP | NEDOSTAJE | nema |
| `MPPovuVPSKL.xml` | 3 |  | loader | MPPovuVPSKL.xml | Povrat iz MP                            ; Povrat iz MP | definitions/netgen/Agronom granule/MPPovuVPSKL.xml | nema |
| `MPPregUtrzakNG.xml` | 3 |  | loader | MPPregUtrzakNG.xml | Utržak iz maloprodaje | definitions/netgen/Agronom granule/MPPregUtrzakNG.xml; definitions/netgen/BIOVEGA/MPPregUtrzakNG.xml | nema |
| `MPRekapPorez.xml` | 3 |  | loader | MPRekapPorez.xml | Rekapitulacija poreza po dok.           ; Rekapitulacija poreza po dok. | NEDOSTAJE | nema |
| `NabNarPregledNarucenog.xml` | 3 |  | loader | NabNarPregledNarucenog.xml | Pregled naručene robe                   ; Pregled naručene robe | NEDOSTAJE | nema |
| `newswebsite.xml` | 3 |  | loader | newswebsite.xml | Manager web stranica | NEDOSTAJE | nema |
| `ObnovaNBCijena.xml` | 3 |  | frm | ObnovaNBCijena.xml | Obnova nab cijena iz robnog   ; Obnova nb cijena iz robnog | definitions/netgen/Agronom granule/ObnovaNBCijena.xml | nema |
| `OOPIzv.xml` | 3 |  | loader | OOPIzv.xml | Izvještaji NG                 ; Izvještaji NG | NEDOSTAJE | nema |
| `OrgShemaKonf.xml` | 3 |  | loader | OrgShemaKonf.xml | Konfigurator Org sheme                  ; Konfigurator Org sheme | NEDOSTAJE | nema |
| `OsobeOperateri.xml` | 3 |  | loader | OsobeOperateri.xml | Veza osobe - operateri; Osobe i operateri; Osobe i operateri | NEDOSTAJE | nema |
| `proRajoni.xml` | 3 |  | loader | skt\proRajoni.xml | Ažuriranje linija                       ; Ažuriranje linija | NEDOSTAJE | nema |
| `PubVarsNG.xml` | 3 |  | loader | PubVarsNG.xml | Proizvodnja Konfigurator | NEDOSTAJE | nema |
| `RekapitulacijaUI.xml` | 3 |  | loader | SKLAD\RekapitulacijaUI.xml | Rekapitulacija ulaza i izlaza; Rekapitulacija ulaza i izlaza ; Rekapitulacija ulaza i izlaza | definitions/netgen/Agronom granule/RekapitulacijaUI.xml | nema |
| `ServisJamstvo3.xml` | 3 |  | loader | ServisJamstvo3.xml | Obračun jamstva trećih | NEDOSTAJE | nema |
| `ServisPrava.xml` | 3 |  | frm | ServisPrava.xml | Prava Pristupa - Servis | definitions/netgen/Agronom granule/ServisPrava.xml | nema |
| `Spojka.xml` | 3 |  | loader | Spojka.xml | Spajanje lokacija                       ; Spajanje lokacija | NEDOSTAJE | nema |
| `TarifaNG.xml` | 3 |  | loader | UTP\TarifaNG.xml | Tarifa                                  ; Tarifa | NEDOSTAJE | nema |
| `Upitnici.xml` | 3 |  | frm | MAR\Upitnici.xml | Upitnici | NEDOSTAJE | nema |
| `VoziloFinRekapNG.xml` | 3 |  | loader | VoziloFinRekapNG.xml | Rekapitulacija troškova vozila | NEDOSTAJE | nema |
| `bTobProcessPurchaseOrder.xml` | 2 | **Biovega** | loader | bTobProcessPurchaseOrder.xml | E-narudžbe WS; E-narudžbe NOVE | definitions/netgen/BIOVEGA/bTobProcessPurchaseOrder.xml | nema |
| `HRSkole.xml` | 2 | **Biovega** | frm | HRSkole.xml | Škole; Škole HR | definitions/netgen/BIOVEGA/HRSkole.xml | nema |
| `KOD_Diskete.xml` | 2 | **Biovega** | loader | KOD_Diskete.xml | Datoteke - specifikacije; Datoteke-specifikacije | definitions/netgen/Agronom granule/KOD_Diskete.xml; definitions/netgen/BIOVEGA/KOD_Diskete.xml | nema |
| `KOD_Doznake.xml` | 2 | **Biovega** | loader | KOD_Doznake.xml | Doznake; Doznake | definitions/netgen/Agronom granule/KOD_Doznake.xml; definitions/netgen/BIOVEGA/KOD_Doznake.xml | nema |
| `KUPSktDokumenti.xml` | 2 | **Biovega** | loader | KUPSktDokumenti.xml | VrsteDok za KUP povezivanje | definitions/netgen/Agronom granule/KUPSktDokumenti.xml; definitions/netgen/BIOVEGA/KUPSktDokumenti.xml | nema |
| `LVKatalozi.xml` | 2 | **Biovega** | loader | LVKatalozi.xml | Katalozi                      ; Katalozi | definitions/netgen/BIOVEGA/LVKatalozi.xml | nema |
| `LVObracun.xml` | 2 | **Biovega** | loader | LVObracun.xml | Obračun loko vožnje           ; Obračun loko vožnje | definitions/netgen/BIOVEGA/LVObracun.xml | docs/granule/LVObracun.md |
| `MPKalkKnjizenje.xml` | 2 | **Biovega** | loader | MPKalkKnjizenje.xml | Povezivanje MP dir.zad. UFA; Povezivanje MP Kalk i MP Fakt | definitions/netgen/Agronom granule/MPKalkKnjizenje.xml; definitions/netgen/BIOVEGA/MPKalkKnjizenje.xml | nema |
| `MPNarFFirma.xml` | 2 | **Biovega** | loader | MPNarFFirma.xml | Nar. franšizera Biovegi; Narudžbenica franšizera | definitions/netgen/BIOVEGA/MPNarFFirma.xml | nema |
| `MPPregNapojnica.xml` | 2 | **Biovega** | loader | MPPregNapojnica.xml | Pregled napojnica; Pregled napojnica | NEDOSTAJE | nema |
| `OdobreniKaloNG.xml` | 2 | **Biovega** | loader+frm | UTP\OdobreniKaloNG.xml; OdobreniKaloNG.xml | Kalo; Odobreni Kalo NG | definitions/netgen/Agronom granule/OdobreniKaloNG.xml; definitions/netgen/BIOVEGA/OdobreniKaloNG.xml | nema |
| `SktSerBrojAzur.xml` | 2 | **Biovega** | loader | SktSerBrojAzur.xml | Izmjena serijskog broja; Ažuriranje broja kontigenta | NEDOSTAJE | nema |
| `wmsinfo.xml` | 2 | **Biovega** | loader | skt\wmsinfo.xml; wmsinfo.xml | WMS info | definitions/netgen/BIOVEGA/wmsinfo.xml | nema |
| `C2620_test.xml` | 2 |  | loader | C2620_test.xml | Stanje VPMP | NEDOSTAJE | nema |
| `CekBlagajnaNG.xml` | 2 |  | loader | CekBlagajnaNG.xml | Unos čekova | NEDOSTAJE | nema |
| `CekObradaNG.xml` | 2 |  | loader | CekObradaNG.xml | Obrada čekova | NEDOSTAJE | nema |
| `DD_Obrade.xml` | 2 |  | loader | DD_Obrade.xml | Obrada | NEDOSTAJE | nema |
| `DD_PredVirmani.xml` | 2 |  | loader | DD_PredVirmani.xml | Predlošci virmana | NEDOSTAJE | nema |
| `DjelatnostiNG.xml` | 2 |  | loader | UTP\DjelatnostiNG.xml | Djelatnosti                             ; Djelatnosti | NEDOSTAJE | nema |
| `dmsstatus.xml` | 2 |  | loader | dmsstatus.xml | DMS Status; DMS Status | NEDOSTAJE | nema |
| `dmsVrstadokumenta.xml` | 2 |  | loader | dmsVrstadokumenta.xml | DMS Vrsta dokumenta; DMS Vrsta dokumenta | NEDOSTAJE | nema |
| `Dne_Det_ECS.xml` | 2 |  | frm | Dne_Det_ECS.xml | Dnevna evidencija                       ; Dnevna evidencija | NEDOSTAJE | nema |
| `Dne_Det_MK.xml` | 2 |  | frm | Dne_Det_MK.xml | Dnevna evidencija | NEDOSTAJE | nema |
| `Dne_Smjene.xml` | 2 |  | frm | Dne_Smjene.xml | Smjene                                  ; Smjene | definitions/netgen/Agronom granule/Dne_Smjene.xml; definitions/netgen/BIOVEGA/Dne_Smjene.xml | nema |
| `fakgen.xml` | 2 |  | loader | fakgen.xml | Generiranje faktura | definitions/netgen/BIOVEGA/FakGen.xml | nema |
| `HRCKadManager.xml` | 2 |  | loader | HRCKadManager.xml | Manager ljudskih resursa                ; Manager kadrova NG; Manager kadrova | NEDOSTAJE | nema |
| `HRCRMManager.xml` | 2 |  | loader | HRCRMManager.xml | Manager radnih mjesta                   ; Manager radnih mjesta NG | NEDOSTAJE | nema |
| `HRIspraveNG.xml` | 2 |  | frm | HRIspraveNG.xml | HR Isprave                              ; HR Isprave | definitions/netgen/Agronom granule/HRIspraveNG.xml | nema |
| `HRPartneri.xml` | 2 |  | loader | HRPartneri.xml | Osobe - partneri | definitions/netgen/BIOVEGA/HRPartneri.xml | nema |
| `IspisProDokSI.xml` | 2 |  | frm | mfg/IspisProDokSI.xml | Ispis proizvodnje | NEDOSTAJE | nema |
| `IzvIskoristeniKalo.xml` | 2 |  | loader | IzvIskoristeniKalo.xml | Pregled Kala | NEDOSTAJE | nema |
| `JupiterTran.xml` | 2 |  | loader | JupiterTran.xml | Translator                              ; Translator | NEDOSTAJE | nema |
| `kategorija.xml` | 2 |  | loader | kategorija.xml | Kategorije zadataka; Kategorije zadataka | NEDOSTAJE | nema |
| `KntIOSObrazac.xml` | 2 |  | frm | KntIOSObrazac.xml | IOS Obrazac NG | NEDOSTAJE | nema |
| `kntIzv306090DEFKOP.xml` | 2 |  | loader | kntIzv306090DEFKOP.xml | 30 60 90 Definicija | definitions/netgen/Agronom granule/kntIzv306090DEFKOP.xml | nema |
| `KntKnjizenjeProizvodnje.xml` | 2 |  | frm | KntKnjizenjeProizvodnje.xml | Knjiženje proizvodnje | NEDOSTAJE | nema |
| `KntKontoUzOrgshemu.xml` | 2 |  | loader | KntKontoUzOrgshemu.xml | Konto uz Orgshemu | definitions/netgen/Agronom granule/KntKontoUzOrgshemu.xml; definitions/netgen/BIOVEGA/KntKontoUzOrgshemu.xml | nema |
| `KntPDVObrazacProjekcija.xml` | 2 |  | frm | KntPDVObrazacProjekcija.xml | PDV projekcija | NEDOSTAJE | nema |
| `KntRobneKorekcije.xml` | 2 |  | frm | KntRobneKorekcije.xml | Robne korekcije ukupno | definitions/netgen/Agronom granule/KntRobneKorekcije.xml | nema |
| `KOD_Donos.xml` | 2 |  | loader | KOD_Donos.xml | Stalni dodaci                           ; Stalni dodaci | NEDOSTAJE | nema |
| `KOD_Doprinosi.xml` | 2 |  | frm | KOD_Doprinosi.xml | Doprinosi                               ; Doprinosi | NEDOSTAJE | nema |
| `KOD_GodPorez.xml` | 2 |  | frm | KOD_GodPorez.xml | Konačni obračun poreza | NEDOSTAJE | nema |
| `KOD_IzmjenaVirmana.xml` | 2 |  | frm | KOD_IzmjenaVirmana.xml | Izmjena naloga za plaćanje              ; Izmjena naloga za plaćanje | NEDOSTAJE | nema |
| `KOD_Obustave.xml` | 2 |  | loader | KOD_Obustave.xml | Obustave                                ; Obustave | definitions/netgen/Agronom granule/KOD_Obustave.xml | nema |
| `KOD_Opcine2.xml` | 2 |  | frm | KOD_Opcine2.xml | Porezne općine                          ; Porezna općina                          ; Porezne općine; Porezna općina | definitions/netgen/Agronom granule/KOD_Opcine2.xml | nema |
| `KOD_PorezIzDohotka.xml` | 2 |  | frm | KOD_PorezIzDohotka.xml | Porez iz dohotka                        ; Porez iz dohotka | NEDOSTAJE | nema |
| `KOD_Predlozak.xml` | 2 |  | frm | KOD_Predlozak.xml | Predlošci naloga                        ; Predlošci naloga | definitions/netgen/Agronom granule/KOD_Predlozak.xml | nema |
| `KUPDostKNECS.xml` | 2 |  | loader | kup\KUPDostKNECS.xml; KUPDostKNECS.xml | KUP Dostavna knjiga; KUP Dostavna knjiga STARA | NEDOSTAJE | nema |
| `KUPDostKNOdobravanjeTusak.xml` | 2 |  | loader | KUPDostKNOdobravanjeTusak.xml | Dostavna knjiga | NEDOSTAJE | nema |
| `KUPNGMlinar.xml` | 2 |  | loader | KUPNGMlinar.xml | Knjiga pošte | NEDOSTAJE | nema |
| `KUPNGTusak_KUP_URA.xml` | 2 |  | loader | KUPNGTusak_KUP_URA.xml | Knjiga pošte | NEDOSTAJE | nema |
| `KUPPregOdobMlinar.xml` | 2 |  | loader | KUPPregOdobMlinar.xml | KUP Pregled odobravanja | NEDOSTAJE | nema |
| `KUPStatN.xml` | 2 |  | loader | KUPStatN.xml | Status pošte; KUP Status | definitions/netgen/Agronom granule/KUPStatN.xml | nema |
| `MercurNaloziOtp.xml` | 2 |  | loader | MercurNaloziOtp.xml | Otprema naloga | NEDOSTAJE | nema |
| `ModeliNaplateNG.xml` | 2 |  | loader | MER\ModeliNaplateNG.xml | Modeli naplate | NEDOSTAJE | nema |
| `MPAnalPrNG.xml` | 2 |  | loader | MPAnalPrNG.xml | Analiza računa po vremenu | NEDOSTAJE | nema |
| `MPAzurTrazilicaRobe.xml` | 2 |  | loader | MPAzurTrazilicaRobe.xml | Pravo na tražilici robe | NEDOSTAJE | nema |
| `MPBlagajnaWEB.xml` | 2 |  | loader | MPBlagajnaWEB.xml | Web Blagajna | NEDOSTAJE | nema |
| `MPBonovi.xml` | 2 |  | loader | MPBonovi.xml | Bonovi | NEDOSTAJE | nema |
| `MPBonoviPregled.xml` | 2 |  | loader | MPBonoviPregled.xml | Pregled bonova | NEDOSTAJE | nema |
| `MPCopyDok.xml` | 2 |  | loader | MPCopyDok.xml | Kopiranje dokumenata | NEDOSTAJE | nema |
| `MPDeAktTrgovina.xml` | 2 |  | frm | MPDeAktTrgovina.xml | Trgovina DE(aktivacija) | NEDOSTAJE | nema |
| `MPDirZaduzenjeKomNG.xml` | 2 |  | loader | MPDirZaduzenjeKomNG.xml | Dir. zaduženje - KOMISIJA | NEDOSTAJE | nema |
| `MPDirZaduzenjeNG1.xml` | 2 |  | loader | MPDirZaduzenjeNG1.xml | MP Primka od dobavljača | NEDOSTAJE | nema |
| `MPDirZaduzenjeNG1BobisGRATIS.xml` | 2 |  | loader | MPDirZaduzenjeNG1BobisGRATIS.xml | MP Primka - Gratis | NEDOSTAJE | nema |
| `MPDodKlsRbaKasa.xml` | 2 |  | loader | MPDodKlsRbaKasa.xml | Dodatne jednice mjere za kasu ; Dodatne klase za kasu | NEDOSTAJE | nema |
| `MPEizvod.xml` | 2 |  | loader | MPEizvod.xml | E-Izvod MP | NEDOSTAJE | nema |
| `MPEizvodPregled.xml` | 2 |  | loader | MPEizvodPregled.xml | Pregled knjiženja E-izvoda | NEDOSTAJE | nema |
| `MPEtiketeNGCentarTehnike.xml` | 2 |  | loader | MPEtiketeNGCentarTehnike.xml | MP Etikete sa dokumenta | NEDOSTAJE | nema |
| `MPEtiketeTerminalCT.xml` | 2 |  | loader | MPEtiketeTerminalCT.xml | Pojedinačne etikete | NEDOSTAJE | nema |
| `MPFakOtpGRUP.xml` | 2 |  | loader | MPFakOtpGRUP.xml | Fakturiranje obustava u mp | NEDOSTAJE | nema |
| `MPGrupeRobe2.xml` | 2 |  | frm | MPGrupeRobe2.xml | Predložak tipkovnice | NEDOSTAJE | nema |
| `MPIntDOkKalo.xml` | 2 |  | loader | MPIntDOkKalo.xml | Kalo u MP - dokument | NEDOSTAJE | nema |
| `MPIntDokZamjenaINVENTURA.xml` | 2 |  | loader | MPIntDokZamjenaINVENTURA.xml | MP Zamjena šifre po inventuri | definitions/netgen/Agronom granule/MPIntDokZamjenaINVENTURA.xml | nema |
| `MPInventuraVrsteRoba.xml` | 2 |  | loader | MPInventuraVrsteRoba.xml | MP inventura - NOVA | NEDOSTAJE | nema |
| `MPInvFaktura.xml` | 2 |  | loader | MPInvFaktura.xml | Faktura inventurne razlike | definitions/netgen/Agronom granule/MPInvFaktura.xml | nema |
| `MPInvRazlike.xml` | 2 |  | loader | MPInvRazlike.xml | Inventurne razlike  MP; Inventurne razlike  MP | definitions/netgen/BIOVEGA/MPInvRazlike.xml | nema |
| `MPIzdVPMP.xml` | 2 |  | loader | MPIzdVPMP.xml | Zaduženje MP iz VP – sa skladišta | definitions/netgen/BIOVEGA/MPIzdVPMP.xml | nema |
| `MPIzdVPMPMlinar.xml` | 2 |  | loader | MPIzdVPMPMlinar.xml | Izdatnica iz VP u MP                    ; Izdatnica u MP | NEDOSTAJE | nema |
| `MPizdVPMPPVC.xml` | 2 |  | loader | MPizdVPMPPVC.xml | Izdatnica PVC vrećice | NEDOSTAJE | nema |
| `MPKalkKnjizenjeMULTI.xml` | 2 |  | frm | MPKalkKnjizenjeMULTI.xml | Povezivanje MP kalk i UFA | NEDOSTAJE | nema |
| `MPKasaImport.xml` | 2 |  | loader | MPKasaImport.xml | Kasa - Import | definitions/netgen/Agronom granule/MPKasaImport.xml | nema |
| `MPKnjigaSanka.xml` | 2 |  | loader | MPKnjigaSanka.xml | Knjiga šanka | NEDOSTAJE | nema |
| `MPKonFinNG.xml` | 2 |  | loader | MPKonFinNG.xml | Kontrola MP - Fin | NEDOSTAJE | nema |
| `MPKontrolaKalk.xml` | 2 |  | loader | MPKontrolaKalk.xml | Kontrola knjiženja kalk. | NEDOSTAJE | nema |
| `MPKOntrolaPredispozicija.xml` | 2 |  | loader | MPKOntrolaPredispozicija.xml | Kontrola predispozicija | NEDOSTAJE | nema |
| `MPKontrolaPrometaStanja.xml` | 2 |  | loader | MPKontrolaPrometaStanja.xml | Kontrola Promet-Stanje | NEDOSTAJE | nema |
| `MPKontrolaSK.xml` | 2 |  | loader | MPKontrolaSK.xml | Kontrola sindikalnih kredita | NEDOSTAJE | nema |
| `MPKOntrolaUGOPDV.xml` | 2 |  | loader | MPKOntrolaUGOPDV.xml | Kontrola UGO PDV-a | NEDOSTAJE | nema |
| `MPKorekcijaNVMJ.xml` | 2 |  | loader | MPKorekcijaNVMJ.xml | Mjesečna korekcija stanja | NEDOSTAJE | nema |
| `MPKrivoStanjeKomisije.xml` | 2 |  | loader | MPKrivoStanjeKomisije.xml | Neispravno stanje komis. robe | NEDOSTAJE | nema |
| `MPListaAmb.xml` | 2 |  | loader | MPListaAmb.xml | Lista ambalaže | NEDOSTAJE | nema |
| `MPlnKlasaKasa.xml` | 2 |  | loader | MPlnKlasaKasa.xml | Ažur. robe za klase na kasi | NEDOSTAJE | nema |
| `MPMinMaxCentarTehnike.xml` | 2 |  | loader | MPMinMaxCentarTehnike.xml | Min i max zalihe CT | NEDOSTAJE | nema |
| `MPMinMaxKonikom.xml` | 2 |  | loader | MPMinMaxKonikom.xml | Min i max zalihe | NEDOSTAJE | nema |
| `MPMinMaxPr.xml` | 2 |  | loader | MPMinMaxPr.xml | Što naručiti - MinMax zalihe | NEDOSTAJE | nema |
| `MPMOBAnaliza.xml` | 2 |  | loader | MPMOBAnaliza.xml | Analiza MP MOB aplikacije | NEDOSTAJE | nema |
| `MPNajArtikli.xml` | 2 |  | loader | MPNajArtikli.xml | TOP artikli u MP | NEDOSTAJE | nema |
| `MPNajavaObnoveNC.xml` | 2 |  | loader | MPNajavaObnoveNC.xml | Najava obnove NC | NEDOSTAJE | nema |
| `MPNar.xml` | 2 |  | loader | MPNar.xml | MP Narudžba dobavljaču | definitions/netgen/Agronom granule/MPNar.xml | nema |
| `MPNarDneProiz.xml` | 2 |  | loader | MPNarDneProiz.xml | Narudžba redovne robe                   ; Narudžba vlastite robe | NEDOSTAJE | nema |
| `MPNarDneProizSMRZ.xml` | 2 |  | loader | MPNarDneProizSMRZ.xml | Narudžba SMRZNUTE robe | NEDOSTAJE | nema |
| `MPNarDneProizTorte.xml` | 2 |  | loader | MPNarDneProizTorte.xml | Extra narudžba | NEDOSTAJE | nema |
| `MPNarPredlosci.xml` | 2 |  | frm | MPNarPredlosci.xml | Definiranje predložaka | NEDOSTAJE | nema |
| `MPNarucenoIsporuceno.xml` | 2 |  | loader | MPNarucenoIsporuceno.xml | Naručeno-isporučeno u MP | NEDOSTAJE | nema |
| `MPNeObracunProi.xml` | 2 |  | loader | MPNeObracunProi.xml | Proizvodi - NE obračun | NEDOSTAJE | nema |
| `MPnivelNCNG.xml` | 2 |  | loader | MPnivelNCNG.xml | Izmjena NC u MP | NEDOSTAJE | nema |
| `MPObracunKalaPeriod.xml` | 2 |  | loader | MPObracunKalaPeriod.xml | Obračun kala u periodu | NEDOSTAJE | nema |
| `MPOdjavaNarNG.xml` | 2 |  | loader | MPOdjavaNarNG.xml | Odjava komisijske robe | definitions/netgen/BIOVEGA/MPOdjavaNarNG.xml | nema |
| `MPOdjavaNG.xml` | 2 |  | loader | MPOdjavaNG.xml | Pregled MP kom.robe za odjavu | definitions/netgen/BIOVEGA/MPOdjavaNG.xml | nema |
| `MPOptimoKategorije.xml` | 2 |  | loader | MPOptimoKategorije.xml | MP dostave prioritet kat. | NEDOSTAJE | nema |
| `MPOptimoVozac.xml` | 2 |  | loader | MPOptimoVozac.xml | OptimoRoute Vozači | NEDOSTAJE | nema |
| `MPOtprMPNG.xml` | 2 |  | loader | MPOtprMPNG.xml | Dostava iz MP | NEDOSTAJE | nema |
| `MPPonudaCentarTehnike.xml` | 2 |  | loader | MPPonudaCentarTehnike.xml | MP Ponuda                     ; MP Ponuda NG | NEDOSTAJE | nema |
| `MPPONUDANEFISK.xml` | 2 |  | loader | MPPONUDANEFISK.xml | Pregled nefiskl. PD | definitions/netgen/Agronom granule/MPPONUDANEFISK.xml | nema |
| `MPPonudaTusak.xml` | 2 |  | loader | MPPonudaTusak.xml | MP Ponuda | NEDOSTAJE | nema |
| `MPPonudaWebP.xml` | 2 |  | loader | MPPonudaWebP.xml | WEB Ponuda Fiskalizacija | definitions/netgen/BIOVEGA/MPPonudaWEBP.xml | nema |
| `MPPovNaknada.xml` | 2 |  | loader | MPPovNaknada.xml | Ukidanje POV naknad | NEDOSTAJE | nema |
| `MPPovOdKup.xml` | 2 |  | loader | MPPovOdKup.xml | Povrat robe od kupca | NEDOSTAJE | nema |
| `MPPovuVPTusakRedov.xml` | 2 |  | loader | MPPovuVPTusakRedov.xml | Povratnica iz MP u VP-redovna | NEDOSTAJE | nema |
| `MPPovUVPTusakTNR.xml` | 2 |  | loader | MPPovUVPTusakTNR.xml | Povratnica iz MP u VP-TNR | NEDOSTAJE | nema |
| `MPPregAkcijaMC.xml` | 2 |  | loader | MPPregAkcijaMC.xml | Pregled akcije MC | NEDOSTAJE | nema |
| `MPPregDirZaduzenjaKum.xml` | 2 |  | loader | MPPregDirZaduzenjaKum.xml | Pregled mp primki - kumulativ | NEDOSTAJE | nema |
| `MPPregDneNar.xml` | 2 |  | loader | MPPregDneNar.xml | Pregled dnevnih narudžbi | NEDOSTAJE | nema |
| `MPPregDnevP.xml` | 2 |  | loader | MPPregDnevP.xml | Pregled stanja dnev.proiz. | NEDOSTAJE | nema |
| `MPPregExtraNar.xml` | 2 |  | loader | MPPregExtraNar.xml | Pregled extra narudžbi | NEDOSTAJE | nema |
| `MPPregInvRazlika.xml` | 2 |  | loader | MPPregInvRazlika.xml | Pregled inv. razlika | NEDOSTAJE | nema |
| `MPPregMPCjenik.xml` | 2 |  | loader | MPPregMPCjenik.xml | Pregled MP cjenika | NEDOSTAJE | nema |
| `MPPregMPCnaKasi.xml` | 2 |  | loader | MPPregMPCnaKasi.xml | Pregled promjene MP cijene | NEDOSTAJE | nema |
| `MPPregNarPotrosniSkl.xml` | 2 |  | loader | MPPregNarPotrosniSkl.xml | Interna otpremnica - Potrošni | NEDOSTAJE | nema |
| `MPPregNCUgovorDob.xml` | 2 |  | loader | MPPregNCUgovorDob.xml | Pregled NC iz ugov.dobavljača | NEDOSTAJE | nema |
| `MPPregNeprUtr.xml` | 2 |  | loader | MPPregNeprUtr.xml | Pregled prijenosa utržaka po kasama | definitions/netgen/Agronom granule/MPPregNeprUtr.xml | nema |
| `MPPregPovNak.xml` | 2 |  | loader | MPPregPovNak.xml | Pregled povratne naknade | definitions/netgen/Agronom granule/MPPregPovNak.xml | nema |
| `MPPregProizCijene.xml` | 2 |  | loader | MPPregProizCijene.xml | Pregled proizvoda - Cijene | NEDOSTAJE | nema |
| `MPPregProvizije.xml` | 2 |  | loader | MPPregProvizije.xml | Pregled provizija | NEDOSTAJE | nema |
| `MPPregPS.xml` | 2 |  | loader | MPPregPS.xml | Pregled PS u MP | definitions/netgen/Agronom granule/MPPregPS.xml | nema |
| `MPPregRACVANRV.xml` | 2 |  | loader | MPPregRACVANRV.xml | Pregled rač. van rad. vremena | NEDOSTAJE | nema |
| `MPPregRbTrCT.xml` | 2 |  | loader | MPPregRbTrCT.xml | Preg. robe po trgovinama NG | NEDOSTAJE | nema |
| `MPPregRbTrNG.xml` | 2 |  | loader | MPPregRbTrNG.xml | Pregled robe po trgovinama | definitions/netgen/Agronom granule/MPPregRbTrNG.xml; definitions/netgen/BIOVEGA/MPPregRbTrNG.xml | nema |
| `MPPregStanjeProizvodjac.xml` | 2 |  | loader | MPPregStanjeProizvodjac.xml | Stanje robe po proizvođaču | NEDOSTAJE | nema |
| `MPPregZaduzenjaMPMlinar.xml` | 2 |  | loader | MPPregZaduzenjaMPMlinar.xml | Pregled zaduženja MP iz VP | NEDOSTAJE | nema |
| `MPPrInvKumulativNG.xml` | 2 |  | loader | MPPrInvKumulativNG.xml | Pregled inventure MP-kumulativno | NEDOSTAJE | nema |
| `MPPrmArtNGBobis.xml` | 2 |  | loader | MPPrmArtNGBobis.xml | Pregled prometa artikala MP | NEDOSTAJE | nema |
| `MPPromDatum.xml` | 2 |  | loader | MPPromDatum.xml | Promjena datuma MP | NEDOSTAJE | nema |
| `MPProvizije.xml` | 2 |  | loader | MPProvizije.xml | Provizije djelatnika | NEDOSTAJE | nema |
| `MPProvizijePregled.xml` | 2 |  | loader | MPProvizijePregled.xml | Pregled stimulacija djelatnika | NEDOSTAJE | nema |
| `MPPrPovNG.xml` | 2 |  | loader | MPPrPovNG.xml | Analiza povrata iz MP | definitions/netgen/Agronom granule/MPPrPovNG.xml | nema |
| `MPPrPromOp.xml` | 2 |  | loader | MPPrPromOp.xml | Pregled prom.po oper. (NOVO) | NEDOSTAJE | nema |
| `MPRabUtNG.xml` | 2 |  | loader | MPRabUtNG.xml | Pregled popusta u utršku | NEDOSTAJE | nema |
| `MPRACAF.xml` | 2 |  | loader | MPRACAF.xml | Preg. nak. fiskaliziranih | NEDOSTAJE | nema |
| `MPRealizacijaKons.xml` | 2 |  | loader | MPRealizacijaKons.xml | Realizacija komis. robe | definitions/netgen/BIOVEGA/MPRealizacijaKons.xml | nema |
| `MPRegije.xml` | 2 |  | loader | MPRegije.xml | Regije trgovina | NEDOSTAJE | nema |
| `MPRekapDobav.xml` | 2 |  | loader | MPRekapDobav.xml | Rekapitulacija po dobavljaču | NEDOSTAJE | nema |
| `MPRekapPorezPP.xml` | 2 |  | loader | MPRekapPorezPP.xml | Rekap. poreza na potrošnju | definitions/netgen/Agronom granule/MPRekapPorezPP.xml | nema |
| `MPRekapPromArt.xml` | 2 |  | frm | MPRekapPromArt.xml | Rekap. prometa po artiklu | NEDOSTAJE | nema |
| `MPRobaBezMPC.xml` | 2 |  | loader | MPRobaBezMPC.xml | Roba bez MPC | definitions/netgen/Agronom granule/MPRobabezMPC.xml | nema |
| `MPRObaBezNC.xml` | 2 |  | loader | MPRObaBezNC.xml | Roba bez NC | definitions/netgen/Agronom granule/MPRobabezNC.xml | nema |
| `MPRobaSmrznuto.xml` | 2 |  | loader | MPRobaSmrznuto.xml | Roba - dodatni atributi | NEDOSTAJE | nema |
| `MPRobaZabrana2.xml` | 2 |  | loader | MPRobaZabrana2.xml | Zabrana robe na blagajni | definitions/netgen/BIOVEGA/MPRobaZabrana2.xml | nema |
| `MPSkladisteVeza.xml` | 2 |  | loader | MPSkladisteVeza.xml | Veza skladišta | NEDOSTAJE | nema |
| `MPStanjeBezNC.xml` | 2 |  | frm | MPStanjeBezNC.xml | Stanje MP | NEDOSTAJE | nema |
| `MPStanjeKons.xml` | 2 |  | loader | MPStanjeKons.xml | Pregled stanja komis. mp robe | definitions/netgen/BIOVEGA/MPStanjeKons.xml | nema |
| `MPStanjeKumulativno.xml` | 2 |  | loader | MPStanjeKumulativno.xml | Stanje trgovina - KUMULATIV | NEDOSTAJE | nema |
| `MPStanjeKumulativnoVL.xml` | 2 |  | loader | MPStanjeKumulativnoVL.xml | Stanje vlastite robe | NEDOSTAJE | nema |
| `MPStanjeMVMJ.xml` | 2 |  | loader | MPStanjeMVMJ.xml | Stanje trgovina po NV | NEDOSTAJE | nema |
| `MPStanjeSirivinaANALIT.xml` | 2 |  | loader | MPStanjeSirivinaANALIT.xml | Stanje vl. sirovina - Analit. | NEDOSTAJE | nema |
| `MPStanjeUgo.xml` | 2 |  | frm | MPStanjeUgo.xml | Stanje robe ugostiteljstvo | NEDOSTAJE | nema |
| `MPTestBlagajni.xml` | 2 |  | loader | MPTestBlagajni.xml | Test Blagajni | definitions/netgen/Agronom granule/MPTestBlagajni.xml | nema |
| `MPTestKomisRobe.xml` | 2 |  | loader | MPTestKomisRobe.xml | Kontrola komisijske robe | definitions/netgen/BIOVEGA/MPTestKomisRobe.xml | nema |
| `MPTovarnilist.xml` | 2 |  | loader | MPTovarnilist.xml | MP Tovarni list | NEDOSTAJE | nema |
| `MPTrgFiskPD.xml` | 2 |  | loader | MPTrgFiskPD.xml | Trg. za fisk. dokumenata | definitions/netgen/Agronom granule/MPTrgFiskPD.xml | nema |
| `MPTrosarine.xml` | 2 |  | loader | MPTrosarine.xml | MP Trošarine | NEDOSTAJE | nema |
| `MPUplKredCKNG.xml` | 2 |  | loader | MPUplKredCKNG.xml | Uplata CK Kredita | NEDOSTAJE | nema |
| `MPUplKredNG.xml` | 2 |  | loader | MPUplKredNG.xml | Uplata Konikom kredita - NOVO | NEDOSTAJE | nema |
| `MPUplKredTJNG.xml` | 2 |  | loader | MPUplKredTJNG.xml | Uplata kredita                ; Uplata TJ kredita | NEDOSTAJE | nema |
| `MPVaucherPregled.xml` | 2 |  | loader | MPVaucherPregled.xml | Pregled vaucher-a | NEDOSTAJE | nema |
| `MPZadnjiBroj.xml` | 2 |  | frm | MPZadnjiBroj.xml | Zadnji brojevi dok. za kasu | NEDOSTAJE | nema |
| `MsklDistrib.xml` | 2 |  | loader | SKT\MsklDistrib.xml | Skladišta za naravnavanje | NEDOSTAJE | nema |
| `nabispanaliza.xml` | 2 |  | loader | nabispanaliza.xml | Analiza NAR-ISP | NEDOSTAJE | nema |
| `NacinObracunaZadatka.xml` | 2 |  | loader | NacinObracunaZadatka.xml | Način obračuna zadatka; Način obračuna zadatka | NEDOSTAJE | nema |
| `NarQryNG.xml` | 2 |  | loader | NarQryNG.xml | Generiranje naloga | NEDOSTAJE | nema |
| `NOtpIsporuceno.xml` | 2 |  | loader | skt/NOtpIsporuceno.xml | Povrat | NEDOSTAJE | nema |
| `NotpStatus.xml` | 2 |  | loader | NotpStatus.xml | Praćenje statusa otpreme | NEDOSTAJE | nema |
| `OOPKnjigaR.xml` | 2 |  | loader | OOPKnjigaR.xml | Knjiga radnika | NEDOSTAJE | nema |
| `OperaterPogon.xml` | 2 |  | frm | mfg/OperaterPogon.xml | Izbor pogona | NEDOSTAJE | nema |
| `ParStruPrava.xml` | 2 |  | frm | ParStruPrava.xml | Operater - lokacija prava | NEDOSTAJE | nema |
| `PKDuznNG.xml` | 2 |  | loader | PKDuznNG.xml | Pregled dužnika po kreditima | NEDOSTAJE | nema |
| `PKKrZatNG.xml` | 2 |  | loader | PKKrZatNG.xml | Zatvaranje krediita | NEDOSTAJE | nema |
| `PKTuzbe.xml` | 2 |  | loader | PKTuzbe.xml | Evidencija tužbi | NEDOSTAJE | nema |
| `PKUvjetiNG.xml` | 2 |  | loader | PKUvjetiNG.xml | Uvjeti kredita | NEDOSTAJE | nema |
| `PKZaduzNG.xml` | 2 |  | loader | PKZaduzNG.xml | Stanje osoba po kreditima | NEDOSTAJE | nema |
| `PovijestCjenika.xml` | 2 |  | loader | PovijestCjenika.xml | Pregled izmjene cjenika; Pregled izmjene cjenika | NEDOSTAJE | nema |
| `povispanaliza.xml` | 2 |  | loader | povispanaliza.xml | Analiza PRE-ISP | NEDOSTAJE | nema |
| `PregledCjenikaNG.xml` | 2 |  | loader | PregledCjenikaNG.xml | Pregled cjenika; Pregled cjenika - ispis | NEDOSTAJE | nema |
| `PregledKnj.xml` | 2 |  | loader | PregledKnj.xml | Pregled prometa NG; Pregled prometa NG | NEDOSTAJE | nema |
| `PrijemniListNG.xml` | 2 |  | loader | PrijemniListNG.xml | Prijemni list                           ; Prijemni list | NEDOSTAJE | nema |
| `prioritet.xml` | 2 |  | loader | prioritet.xml | Prioriteti zadataka; Prioriteti zadataka | NEDOSTAJE | docs/granule/Prioritet.md |
| `prnPredajaM.xml` | 2 |  | loader | MFG/prnPredajaM.xml | Dnevni Nalog | definitions/netgen/BIOVEGA/prnPredajaM.xml | nema |
| `prnPredajaSKTM.xml` | 2 |  | loader | MFG\prnPredajaSKTM.xml | Predaja iz proizvodnje SKT | NEDOSTAJE | nema |
| `prnProizvodMulti.xml` | 2 |  | loader | MFG/prnProizvodMulti.xml | Ažuriranje receptura | NEDOSTAJE | nema |
| `prnSastavniceQuery.xml` | 2 |  | drugo | prnSastavniceQuery.xml | Pregled sastavnica | NEDOSTAJE | nema |
| `prnUtrosakAzurMlinar.xml` | 2 |  | loader | MFG\prnUtrosakAzurMlinar.xml | Ažuriranje utroška | NEDOSTAJE | nema |
| `proces.xml` | 2 |  | loader | proces.xml | Proces; Proces | NEDOSTAJE | nema |
| `ProENarudzbe.xml` | 2 |  | loader | ProENarudzbe.xml | E-Narudžbe | NEDOSTAJE | nema |
| `PrometAmbalazeNG.xml` | 2 |  | loader | PrometAmbalazeNG.xml | Promet Ambalaže NG | NEDOSTAJE | nema |
| `ProNarPredlozakNG.XML` | 2 |  | loader | ProNarPredlozakNG.XML | Predlošci | NEDOSTAJE | nema |
| `ProNarudzbeNG.xml` | 2 |  | loader | ProNarudzbeNG.xml | Pekarske narudžbe | NEDOSTAJE | nema |
| `PrOrgSh.xml` | 2 |  | loader | PrOrgSh.xml | Pregled po MT-Kartica | NEDOSTAJE | nema |
| `ProTovarka2.xml` | 2 |  | frm | ProTovarka2.xml | Lista za komisioniranje | NEDOSTAJE | nema |
| `PTOsobeCjenik.xml` | 2 |  | loader | PTOsobeCjenik.xml | Osobe Cjenik; Osobe Cjenik | NEDOSTAJE | nema |
| `ptutrvrstarada.xml` | 2 |  | loader | ptutrvrstarada.xml | Vrsta rada; Vrsta rada | NEDOSTAJE | nema |
| `PTVrstaDok.xml` | 2 |  | loader | PTVrstaDok.xml | Vrste dokumenata; Vrste dokumenata | NEDOSTAJE | nema |
| `RbaUDNG.xml` | 2 |  | loader | RbaUDNG.xml | Isporučeno-nezaprimljeno MP | NEDOSTAJE | nema |
| `rutelnNG.xml` | 2 |  | loader | rutelnNG.xml | Dostavna mjesta - Linije | NEDOSTAJE | nema |
| `sktKomoraJob.xml` | 2 |  | frm | SKT/sktKomoraJob.xml | Generiranje međuskladišnica | NEDOSTAJE | nema |
| `SktMrvice.xml` | 2 |  | loader | SKT\SktMrvice.xml | Mrvice | NEDOSTAJE | nema |
| `SktPrometSMSK.xml` | 2 |  | loader | SktPrometSMSK.xml | Pregled stanja na dan | NEDOSTAJE | nema |
| `sktStariKruh_Query.xml` | 2 |  | loader | skt/sktStariKruh_Query.xml | Stari kruh pregled | NEDOSTAJE | nema |
| `Statusi.xml` | 2 |  | loader | Statusi.xml | Statusi dokumenata | definitions/netgen/Agronom granule/Statusi.xml | docs/granule/Statusi.md |
| `StatusProjekta.xml` | 2 |  | loader | StatusProjekta.xml | Status projekta; Status projekta | NEDOSTAJE | nema |
| `statusZadatka.xml` | 2 |  | loader | statusZadatka.xml | Status zadatka; Status zadatka | NEDOSTAJE | nema |
| `STFunkcijeOsobaNG.xml` | 2 |  | loader | STFunkcijeOsobaNG.xml | Funkcije osoba | NEDOSTAJE | nema |
| `TecajnaListaNG.xml` | 2 |  | loader | TecajnaListaNG.xml | Tečajna lista; Tečajna lista | NEDOSTAJE | nema |
| `TovarniListNG.xml` | 2 |  | loader | TovarniListNG.xml | Tovarni list | NEDOSTAJE | nema |
| `UDjeParNG.xml` | 2 |  | loader | UDjeParNG.xml | Urednik djelatnosti partnera            ; Urednik djelatnosti partnera | NEDOSTAJE | nema |
| `VrstaProjekt.xml` | 2 |  | loader | VrstaProjekt.xml | Vrsta projekta; Vrste projekta | NEDOSTAJE | nema |
| `WSOsobe.xml` | 2 |  | loader | WSOsobe.xml | Korisnički računi za Web Shop | NEDOSTAJE | nema |
| `biovegafakgen.xml` | 1 | **Biovega** | loader | biovegafakgen.xml | Generiranje faktura | definitions/netgen/BIOVEGA/BiovegaFakGen.xml | nema |
| `GastroPartner.xml` | 1 | **Biovega** | loader | GastroPartner.xml | Gastro partneri | definitions/netgen/BIOVEGA/GastroPartner.xml | nema |
| `GastroPregledRacuna.xml` | 1 | **Biovega** | loader | GastroPregledRacuna.xml | Pregled računa | definitions/netgen/BIOVEGA/GastroPregledRacuna.xml | nema |
| `inventurace.xml` | 1 | **Biovega** | loader | skt/inventurace.xml | Donos s terminala | definitions/netgen/BIOVEGA/inventuraCE.xml | nema |
| `InvRaz.xml` | 1 | **Biovega** | loader | InvRaz.xml | Inv. razlike po klasama robe; Inv. razlike po klasama robe B | definitions/netgen/BIOVEGA/InvRaz.xml | nema |
| `IspisNaljepnica.xml` | 1 | **Biovega** | frm | IspisNaljepnica.xml | Ispis etiketa | definitions/netgen/BIOVEGA/IspisNaljepnica.xml | nema |
| `izmjenapostotka.xml` | 1 | **Biovega** | loader | izmjenapostotka.xml | Klase robe - % NC | NEDOSTAJE | nema |
| `KKvalIzv.xml` | 1 | **Biovega** | loader | KKvalIzv.xml | Slijedivost | definitions/netgen/BIOVEGA/KKvalIZV.xml | nema |
| `KKvalUvjetiPartnera.xml` | 1 | **Biovega** | loader | KKvalUvjetiPartnera.xml | Rok trajanja - Partneri | NEDOSTAJE | nema |
| `KUPDostKnNGBvg.xml` | 1 | **Biovega** | loader | KUPDostKnNGBvg.xml | Dostavna knjiga | definitions/netgen/BIOVEGA/KUPDostKnNGBvg.xml | docs/granule/KUPDostKnNGBvg.md |
| `KUPNGNGBvg.xml` | 1 | **Biovega** | loader | KUPNGNGBvg.xml | Knjiga pošte | definitions/netgen/BIOVEGA/KUPNGNGBvg.xml | nema |
| `KUPVrsPN.xml` | 1 | **Biovega** | loader | KUPVrsPN.xml | Vrsta pošte | definitions/netgen/BIOVEGA/KUPVrsPN.xml | nema |
| `MP-RabUtNG.xml` | 1 | **Biovega** | loader | MP-RabUtNG.xml | Pregled odobrenih popusta | definitions/netgen/BIOVEGA/MP-RabUtNG.xml | nema |
| `MPAnalBrStavaka.xml` | 1 | **Biovega** | loader | MPAnalBrStavaka.xml | Analiza broja stavaka računa | definitions/netgen/BIOVEGA/MPAnalBrStavaka.xml | nema |
| `MPAnalizaVezana.xml` | 1 | **Biovega** | loader | MPAnalizaVezana.xml | Analiza vezane kupnje | definitions/netgen/BIOVEGA/MPAnalizaVezana.xml | nema |
| `MPAsortimanAzurNBiovega.xml` | 1 | **Biovega** | loader | MPAsortimanAzurNBiovega.xml | _Asortiman dobavljača | definitions/netgen/BIOVEGA/MPAsortimanAzurNBiovega.xml | nema |
| `MPBlagajnaBiovega.xml` | 1 | **Biovega** | loader | MPBlagajnaBiovega.xml | Računi; MP računi - Blagajna | definitions/netgen/BIOVEGA/MPBlagajnaBiovega.xml | nema |
| `MPBodNG.xml` | 1 | **Biovega** | loader | MPBodNG.xml | Bodovi | definitions/netgen/BIOVEGA/MPBodNG.xml | nema |
| `MPClanK.xml` | 1 | **Biovega** | loader | MPClanK.xml | Pregled prometa članova | definitions/netgen/BIOVEGA/MPClanK.xml | nema |
| `MPClanNG.xml` | 1 | **Biovega** | loader | MPClanNG.xml | Članovi kluba | definitions/netgen/BIOVEGA/MPClanNG.xml | nema |
| `MPClanPBNG.xml` | 1 | **Biovega** | loader | MPClanPBNG.xml | Pregled stanja članova | definitions/netgen/BIOVEGA/MPClanPBng.xml | nema |
| `MPClanPoklon.xml` | 1 | **Biovega** | loader | MPClanPoklon.xml | Poklon bodovi | NEDOSTAJE | nema |
| `MPDanDostaveSkt.xml` | 1 | **Biovega** | frm | MPDanDostaveSkt.xml | Dani kreiranja aut. narudžbi | definitions/netgen/BIOVEGA/MPDanDostaveSkt.xml | nema |
| `MPDobavljaciAmbulatni.xml` | 1 | **Biovega** | loader | MPDobavljaciAmbulatni.xml | Ambulatni dobavljači | definitions/netgen/BIOVEGA/MPDobavljaciAmbulatni.xml | nema |
| `MPDodatniBodovi.xml` | 1 | **Biovega** | loader | MPDodatniBodovi.xml | Dodatni bodovi | definitions/netgen/BIOVEGA/MPDodatniBodovi.xml | nema |
| `MPDodKlasaMinKol.xml` | 1 | **Biovega** | frm | MPDodKlasaMinKol.xml | Min. kol. po dod. klasi | definitions/netgen/BIOVEGA/MPDodKlasaMinKol.xml | nema |
| `MPDodKlasaTransportno.xml` | 1 | **Biovega** | frm | MPDodKlasaTransportno.xml | _Ažuriranje % tran. pakiranja | definitions/netgen/BIOVEGA/MPDodKlasaTransportno.xml | nema |
| `MPDonTerNG.xml` | 1 | **Biovega** | loader | MPDonTerNG.xml | Donos sa terminala | definitions/netgen/BIOVEGA/MPDonTerNG.xml | nema |
| `MPEFaktureOtpremnice_test.xml` | 1 | **Biovega** | loader | MPEFaktureOtpremnice_test.xml | _efak eotp test | NEDOSTAJE | nema |
| `MPEtiketeBiovegaRepx.xml` | 1 | **Biovega** | frm | MPEtiketeBiovegaRepx.xml | Ispis etikete-NOVO; Ispis etikete-Biovega repx | definitions/netgen/BIOVEGA/MPEtiketeBiovegaRepx.xml | nema |
| `MPEtiketeTerminalNEW.xml` | 1 | **Biovega** | loader | MPEtiketeTerminalNEW.xml | Etikete mob | definitions/netgen/BIOVEGA/MPEtiketeTerminalNEW.xml | nema |
| `MPGrupeRobeTest.xml` | 1 | **Biovega** | frm | MPGrupeRobeTest.xml | Definiranje Tipkovnice | definitions/netgen/BIOVEGA/MPGrupeRobeTest.xml | nema |
| `MPIntDokIntPrim.xml` | 1 | **Biovega** | loader | MPIntDokIntPrim.xml | Interna primka | definitions/netgen/BIOVEGA/MPIntDokIntPrim.xml | nema |
| `MPIntDokZamjenaUGO.xml` | 1 | **Biovega** | loader | MPIntDokZamjenaUGO.xml | Zamjena šifri UGO | definitions/netgen/BIOVEGA/MPIntDokZamjenaUGO.xml | nema |
| `MPInvDonNGNew.xml` | 1 | **Biovega** | loader | MPInvDonNGNew.xml | MP Inventura - DONOS | definitions/netgen/BIOVEGA/MPInvDonNGNew.xml | nema |
| `MPIzdVPMPKont.xml` | 1 | **Biovega** | loader | MPIzdVPMPKont.xml | Izdatnica u MP - Kontigenti | definitions/netgen/Agronom granule/MPIzdVPMPKont.xml; definitions/netgen/BIOVEGA/MPIzdVPMPKont.xml | nema |
| `MPKaseKeksPay.xml` | 1 | **Biovega** | loader | MPKaseKeksPay.xml | KeksPay | NEDOSTAJE | nema |
| `MPKasetemplate.xml` | 1 | **Biovega** | loader | MPKasetemplate.xml | Ažuriranje template kase | definitions/netgen/BIOVEGA/MPKaseTemplate.xml | nema |
| `MpKeksPayKontrola.xml` | 1 | **Biovega** | loader | MpKeksPayKontrola.xml | Kontrola KeksPay-a | NEDOSTAJE | nema |
| `MPKonIzBiovega.xml` | 1 | **Biovega** | loader | MPKonIzBiovega.xml | Kontrola izdatnica iz VP | definitions/netgen/BIOVEGA/MPKonIzBiovega.xml | nema |
| `MPKontrolaMPRobno.xml` | 1 | **Biovega** | loader | MPKontrolaMPRobno.xml | Kontrola MP Robno | NEDOSTAJE | nema |
| `MPKontrolaUtrzaka.xml` | 1 | **Biovega** | loader | MPKontrolaUtrzaka.xml | Kontrola prijenosa utržaka | definitions/netgen/BIOVEGA/MPKontrolaUtrzaka.xml | nema |
| `MPKontrolaWMS.xml` | 1 | **Biovega** | loader | MPKontrolaWMS.xml | Kontrola WMS - MP | NEDOSTAJE | nema |
| `MPKorekcijaStanja.xml` | 1 | **Biovega** | loader | MPKorekcijaStanja.xml | Pregled korekcije stanja | definitions/netgen/BIOVEGA/MPKorekcijaStanja.xml | nema |
| `MPLoyParametri.xml` | 1 | **Biovega** | loader | MPLoyParametri.xml | Loyalty parametri | NEDOSTAJE | nema |
| `MPMInPeriodRoba.xml` | 1 | **Biovega** | loader | MPMInPeriodRoba.xml | Min. zalihe za period | definitions/netgen/BIOVEGA/MPMinPeriodRoba.xml | nema |
| `MPNagradniBodovi.xml` | 1 | **Biovega** | loader | MPNagradniBodovi.xml | Nagradni bodovi | definitions/netgen/BIOVEGA/MPNagradniBodovi.xml | nema |
| `MPNajavaOptimalne.xml` | 1 | **Biovega** | loader | MPNajavaOptimalne.xml | Najava optimalnih zaliha | definitions/netgen/BIOVEGA/MPNajavaOptimalne.xml | nema |
| `MPNarFDob.xml` | 1 | **Biovega** | loader | MPNarFDob.xml | Narudž. franšizera dobavljaču (old) | NEDOSTAJE | nema |
| `MPNarFDobA.xml` | 1 | **Biovega** | loader | MPNarFDobA.xml | Nar. franšizera dobavljaču | NEDOSTAJE | nema |
| `MPNarFDobMail.xml` | 1 | **Biovega** | loader | MPNarFDobMail.xml | Nar. franšizera dobavljaču | definitions/netgen/BIOVEGA/MPNarFDobMail.xml | nema |
| `MPNarFFirma1.xml` | 1 | **Biovega** | loader | MPNarFFirma1.xml | Nar. franšizera prema vlas | NEDOSTAJE | nema |
| `MPNarIzdSklAgiliris.xml` | 1 | **Biovega** | loader | MPNarIzdSklAgiliris.xml | Nar. za skladište iz MP (Agiliris); _Nar. za skladište iz MP (Agiliris) | definitions/netgen/BIOVEGA/MPNarIzdSklAgiliris.xml | nema |
| `MPNarIzdSklTab.xml` | 1 | **Biovega** | loader | MPNarIzdSklTab.xml | _Nar. za skladište ( Tabela ) | NEDOSTAJE | nema |
| `MPNarZaprimi.xml` | 1 | **Biovega** | loader | MPNarZaprimi.xml | _Narudžbenica dobavljaču (old); Narudžbenica dobavljaču (old) | definitions/netgen/BIOVEGA/MPNarZaprimi.xml | nema |
| `MPNarZaprimiMail.xml` | 1 | **Biovega** | loader | MPNarZaprimiMail.xml | Narudžba dobavljaču; Narudžbenica dobavljaču | definitions/netgen/BIOVEGA/MPNarZaprimiMail.xml | nema |
| `MPNGPregDokMAIL.xml` | 1 | **Biovega** | loader | MPNGPregDokMAIL.xml | Pregled dokumenata MP | definitions/netgen/BIOVEGA/MPNGPregDokMAIL.xml | docs/granule/MPNGPregDokMAIL.md |
| `MPnivelCNGBiovega.xml` | 1 | **Biovega** | loader | MPnivelCNGBiovega.xml | MP cjenik izmjena | definitions/netgen/BIOVEGA/MPnivelCNGBiovega.xml | nema |
| `MPObiteljska.xml` | 1 | **Biovega** | loader | MPObiteljska.xml | Obiteljske kartice | definitions/netgen/BIOVEGA/MPObiteljska.xml | nema |
| `MPObracunKalaNGBiovegaWEB.xml` | 1 | **Biovega** | loader | MPObracunKalaNGBiovegaWEB.xml | Pregled dozvoljenog kala webshop | NEDOSTAJE | nema |
| `MPObracunKalaNG_OLD.xml` | 1 | **Biovega** | loader | MPObracunKalaNG_OLD.xml | Obračun dozvoljenog kala u MP | definitions/netgen/BIOVEGA/MPObracunKalaNG_old.xml | nema |
| `MPObracunSanka.xml` | 1 | **Biovega** | loader | MPObracunSanka.xml | Obračun šanka | definitions/netgen/BIOVEGA/MPObracunSanka.xml | nema |
| `MPObrpNG.xml` | 1 | **Biovega** | loader | MPObrpNG.xml | Obračun proizvodnje MP | definitions/netgen/BIOVEGA/MPObrpNG.XML | nema |
| `MPOptimalneZaliheNEW.xml` | 1 | **Biovega** | frm | MPOptimalneZaliheNEW.xml | Optimalne zalihe s NC | definitions/netgen/BIOVEGA/MPOptimalneZaliheNEW.xml | nema |
| `MPOptimalneZaliheNEWNC0.xml` | 1 | **Biovega** | frm | MPOptimalneZaliheNEWNC0.xml | Optimalne zalihe; _Optimalne zalihe | definitions/netgen/BIOVEGA/MPOptimalneZaliheNEWNC0.xml | nema |
| `MPOptimalneZaliheNEWv2.xml` | 1 | **Biovega** | frm | MPOptimalneZaliheNEWv2.xml | Min. zalihe po trgovinama; Minimalne zalihe | definitions/netgen/BIOVEGA/MPOptimalneZaliheNEWv2.xml | nema |
| `MPOtpis.xml` | 1 | **Biovega** | loader | MPOtpis.xml | Otpis robe u MP; Otpis robe | definitions/netgen/BIOVEGA/MPOtpis.xml | nema |
| `MPOtpisanaRoba.xml` | 1 | **Biovega** | loader | MPOtpisanaRoba.xml | Otpisana roba u MP | definitions/netgen/BIOVEGA/MPOtpisanaRoba.xml | nema |
| `MPOtpremnicaBB.xml` | 1 | **Biovega** | loader | MPOtpremnicaBB.xml | MP otpremnica | definitions/netgen/BIOVEGA/MPOtpremnicaBB.xml | nema |
| `MPOtpremnicaF.xml` | 1 | **Biovega** | loader | MPOtpremnicaF.xml | Faktura MP franšize | definitions/netgen/BIOVEGA/MPOtpremnicaF.xml | nema |
| `MPPakiranjeDodKlasa.xml` | 1 | **Biovega** | loader | MPPakiranjeDodKlasa.xml | Parametari naručivanja | NEDOSTAJE | nema |
| `MPPakiranjeRoba.xml` | 1 | **Biovega** | loader | MPPakiranjeRoba.xml | Parametri naručivanja - roba | definitions/netgen/BIOVEGA/MPPakiranjeRoba.xml | nema |
| `MPParAsortIsporuka.xml` | 1 | **Biovega** | loader | MPParAsortIsporuka.xml | Rokovi isporuke partnera; _Rokovi isporuke partnera | definitions/netgen/BIOVEGA/MPParAsortIsporuka.xml | nema |
| `MPPoklonKartice.xml` | 1 | **Biovega** | loader | MPPoklonKartice.xml | Darovne kartice | definitions/netgen/BIOVEGA/MPPoklonKartice.xml | nema |
| `MPPonudaNaruciRazliku.xml` | 1 | **Biovega** | loader | MPPonudaNaruciRazliku.xml | MP narudžba na SKL za WEB | definitions/netgen/BIOVEGA/MPPonudaNaruciRazliku.xml | nema |
| `MPPonudaNEW.xml` | 1 | **Biovega** | loader | MPPonudaNEW.xml | Ponuda ručni unos; MP ponuda 0800 | definitions/netgen/BIOVEGA/MPPonudaNEW.xml | nema |
| `MPPonudaPotrebe.xml` | 1 | **Biovega** | loader | MPPonudaPotrebe.xml | Potrebe za isporuku webshop | definitions/netgen/BIOVEGA/MPPonudaPotrebe.xml | nema |
| `MPPorukeloy.xml` | 1 | **Biovega** | loader | MPPorukeloy.xml | _Poruke za članove | definitions/netgen/BIOVEGA/MPPorukeLoy.xml | nema |
| `MPPovDobFran2.xml` | 1 | **Biovega** | loader | MPPovDobFran2.xml | Povrat robe Biovegi; Povrat robe franšizera | definitions/netgen/BIOVEGA/MPPovDobFran2.xml | nema |
| `MPPovratDobNGMail.xml` | 1 | **Biovega** | loader | MPPovratDobNGMail.xml | Povratnica dobavljaču; Povratnica dobavljaču iz MP | definitions/netgen/BIOVEGA/MPPovratDobNGMail.xml | nema |
| `MPPovUVPKONT.xml` | 1 | **Biovega** | loader | MPPovUVPKONT.xml | Povratnica iz MP u VP; Povratnica u VP | definitions/netgen/BIOVEGA/MPPovuVPKONT.xml | nema |
| `MPPovuVPSKLKont.XML` | 1 | **Biovega** | loader | MPPovuVPSKLKont.XML | Povratnica iz MP - Kontigenti | definitions/netgen/Agronom granule/MPPovuVPSKLKont.xml; definitions/netgen/BIOVEGA/MPPovuVPSKLKont.xml | nema |
| `MPPregANar.xml` | 1 | **Biovega** | loader | MPPregANar.xml | Pregled robe za narudžbu | definitions/netgen/BIOVEGA/MPPregANar.xml | nema |
| `MPPregANarDob.xml` | 1 | **Biovega** | loader | MPPregANarDob.xml | Pregled robe za nar. dobavljaču | definitions/netgen/BIOVEGA/MPPregANarDob.xml | nema |
| `MPPregArtNG2.xml` | 1 | **Biovega** | loader | MPPregArtNG2.xml | Pregled prometa artikala | definitions/netgen/BIOVEGA/MPPregArtNG2.xml | nema |
| `MPPregArtPonWeb.xml` | 1 | **Biovega** | loader | MPPregArtPonWeb.xml | _Pregled nepromoviranih ponuda; Pregled nepromoviranih ponuda | definitions/netgen/BIOVEGA/MPPregArtPonWeb.xml | nema |
| `MPPregAutoNar.xml` | 1 | **Biovega** | frm | MPPregAutoNar.xml | Kontrola auto. narudžbenica | definitions/netgen/BIOVEGA/MPPregAutoNar.xml | nema |
| `MPPregFakFran.xml` | 1 | **Biovega** | loader | MPPregFakFran.xml | Preg. faktura vlasnika franšize | definitions/netgen/BIOVEGA/MPPregFakFran.xml | nema |
| `MPPregKlaseStanja.xml` | 1 | **Biovega** | loader | MPPregKlaseStanja.xml | Pregled stanja po klasama | definitions/netgen/BIOVEGA/MPPregKlaseStanja.xml | nema |
| `MPPreglClanPOTrg.xml` | 1 | **Biovega** | loader | MPPreglClanPOTrg.xml | Pregled prometa po trgovini | definitions/netgen/BIOVEGA/MPPreglClanPOTrg.xml | nema |
| `MPPregledPrometaCC.xml` | 1 | **Biovega** | loader | MPPregledPrometaCC.xml | Pregled prometa za trg. centre; Pregled prometa CC | definitions/netgen/BIOVEGA/MPPregledPrometaCC.xml | nema |
| `MPPregOSS.xml` | 1 | **Biovega** | loader | MPPregOSS.xml | Pregled za OSS | definitions/netgen/BIOVEGA/MPPregOSS.xml | nema |
| `MPPregProizNorm2.xml` | 1 | **Biovega** | loader | MPPregProizNorm2.xml | _Ažur. proizvoda i normativa | NEDOSTAJE | nema |
| `MPPregPromPDV.xml` | 1 | **Biovega** | loader | MPPregPromPDV.xml | Inv. stanje promjena PDV | NEDOSTAJE | nema |
| `MPPregPromRabat.xml` | 1 | **Biovega** | loader | MPPregPromRabat.xml | Pregled prodaje po rabatima | definitions/netgen/BIOVEGA/MPPregPromRabat.xml | nema |
| `MPPregRbTrNaDatNG.xml` | 1 | **Biovega** | loader | MPPregRbTrNaDatNG.xml | Pregled robe u trg. na dan | definitions/netgen/BIOVEGA/MPPregRbTrNaDatNG.xml | nema |
| `MPPregRbTrNG2.xml` | 1 | **Biovega** | loader | MPPregRbTrNG2.xml | Pregled robe po trgovinama | definitions/netgen/BIOVEGA/MPPregRbTrNG2.xml | nema |
| `MPPregRbTrNGBezPrava.xml` | 1 | **Biovega** | loader | MPPregRbTrNGBezPrava.xml | Pregled robe po svim trgovinama | definitions/netgen/BIOVEGA/MPPregRbTrNGBezPrava.xml | nema |
| `MPPregRbTrNGBezPravaV2.xml` | 1 | **Biovega** | loader | MPPregRbTrNGBezPravaV2.xml | Pregled robe po SVIM trgovinama | definitions/netgen/BIOVEGA/MPPregRbTrNGBezPravaV2.xml | nema |
| `MPPregRobeZaNarSkl.xml` | 1 | **Biovega** | loader | MPPregRobeZaNarSkl.xml | _Pregled robe za narudžbu | NEDOSTAJE | nema |
| `MPPregVaucher.xml` | 1 | **Biovega** | loader | MPPregVaucher.xml | Pregled realizacije vaucher-a | definitions/netgen/BIOVEGA/MPPregVaucher.xml | nema |
| `MPPregWoltTrg.xml` | 1 | **Biovega** | loader | MPPregWoltTrg.xml | Wolt stanje | definitions/netgen/BIOVEGA/MPPregWoltTrg.xml | nema |
| `MPRabatRaspored.xml` | 1 | **Biovega** | loader | MPRabatRaspored.xml | Odobreni popusti po vrstama | definitions/netgen/BIOVEGA/MPRabatRaspored.xml | nema |
| `MPRazlogOtpisa.xml` | 1 | **Biovega** | loader | MPRazlogOtpisa.xml | Razlog otpisa u MP | definitions/netgen/BIOVEGA/MPRazlogOtpisa.xml | nema |
| `MPRazlogPovrata.xml` | 1 | **Biovega** | loader | MPRazlogPovrata.xml | Razlog povrata robe u skl. | definitions/netgen/BIOVEGA/MPRazlogPovrata.xml | nema |
| `MPRekPorDokNG.xml` | 1 | **Biovega** | loader | MPRekPorDokNG.xml | Rekap. poreza po dokumentima | definitions/netgen/BIOVEGA/MPRekPorDokNG.xml | nema |
| `MPRobaNarPakovanje.xml` | 1 | **Biovega** | frm | MPRobaNarPakovanje.xml | Definiranje pakovanja nar.; _Definiranje pakovanja nar. | definitions/netgen/BIOVEGA/MPRobaNarPakovanje.xml | nema |
| `MPRobaXmlWolt.xml` | 1 | **Biovega** | loader | MPRobaXmlWolt.xml | Roba XML Wolt | definitions/netgen/BIOVEGA/MPRobaXmlWolt.xml | nema |
| `MPSezonskaRoba.xml` | 1 | **Biovega** | loader | MPSezonskaRoba.xml | Sezonska roba | NEDOSTAJE | nema |
| `MPstanjeTrg.xml` | 1 | **Biovega** | frm | MPstanjeTrg.xml | Stanja trgovina | definitions/netgen/BIOVEGA/MPStanjeTrg.xml | nema |
| `MPTrgovineDanaProm.xml` | 1 | **Biovega** | loader | MPTrgovineDanaProm.xml | Dani prometa trgovina | NEDOSTAJE | nema |
| `MPTrgovineWoltXml.xml` | 1 | **Biovega** | loader | MPTrgovineWoltXml.xml | Trgovina XML Wolt | definitions/netgen/BIOVEGA/MPTrgovineWoltXml.xml | nema |
| `MPUvjetikasenew.xml` | 1 | **Biovega** | loader | MPUvjetikasenew.xml | Uvjeti prodaje na kasi | definitions/netgen/BIOVEGA/MPUvjetiKaseNEW.xml | nema |
| `MPUvjetiKaseNEW2.xml` | 1 | **Biovega** | loader | MPUvjetiKaseNEW2.xml | Uvjeti plaćanja na kasi | definitions/netgen/BIOVEGA/MPUvjetiKaseNEW2.xml | nema |
| `MPVaganaRoba.xml` | 1 | **Biovega** | loader | MPVaganaRoba.xml | Vagana roba (staro) | NEDOSTAJE | nema |
| `MPVracenaRoba.xml` | 1 | **Biovega** | loader | MPVracenaRoba.xml | Vraćena roba u MP | definitions/netgen/BIOVEGA/MPVracenaRoba.xml | nema |
| `MPwebDostavaDef.xml` | 1 | **Biovega** | loader | MPwebDostavaDef.xml | Definiranje web dostave | definitions/netgen/BIOVEGA/MPwebDostavaDef.xml | nema |
| `MPZakljucakPeriodaBiovega.xml` | 1 | **Biovega** | loader | MPZakljucakPeriodaBiovega.xml | Zaključak perioda MP sve | definitions/netgen/BIOVEGA/MPZakljucakPeriodaBiovega.xml | nema |
| `NabStoNarucitiBiovegaNG.xml` | 1 | **Biovega** | loader | MER\NabStoNarucitiBiovegaNG.xml | Što naručiti | definitions/netgen/BIOVEGA/NabStoNarucitiBiovegaNG.xml | nema |
| `nultiKontingenti.xml` | 1 | **Biovega** | loader | nultiKontingenti.xml | Kontingenti | definitions/netgen/BIOVEGA/nultiKontingenti.xml | nema |
| `nultimultikontingenti.xml` | 1 | **Biovega** | loader | nultimultikontingenti.xml | Nulti kontingenti | definitions/netgen/BIOVEGA/nultiMultiKontingenti.xml | nema |
| `OOPGodOlaksice.xml` | 1 | **Biovega** | loader | OOPGodOlaksice.xml | Godišnje porezne stope; Godišnje porezne olakšice | NEDOSTAJE | nema |
| `Otpremnine.xml` | 1 | **Biovega** | loader | Otpremnine.xml | Otpremnine | definitions/netgen/BIOVEGA/Otpremnine.xml | nema |
| `OznakaKl.xml` | 1 | **Biovega** | drugo | OznakaKl.xml | Dodatna oznaka klase | definitions/netgen/BIOVEGA/OznakaKL.xml | docs/granule/OznakaKL.md |
| `PovKompost.xml` | 1 | **Biovega** | loader | skt\PovKompost.xml | Povezivanje komposta | NEDOSTAJE | nema |
| `PrnTroskovi.xml` | 1 | **Biovega** | loader | PrnTroskovi.xml | Troškovi uz klase/proizvode | definitions/netgen/BIOVEGA/PrnTroskovi.xml | nema |
| `PrnVrstaTroska.xml` | 1 | **Biovega** | loader | PrnVrstaTroska.xml | Vrste Troškova | definitions/netgen/BIOVEGA/PrnVrstaTroska.xml | nema |
| `RobaAtributiUredi.xml` | 1 | **Biovega** | loader | RobaAtributiUredi.xml | Web roba atributi; Roba- WEB atributi | definitions/netgen/BIOVEGA/RobaAtributiUredi.xml | docs/granule/RobaAtributiUredi.md |
| `RobaNutritivneVrijednosti.xml` | 1 | **Biovega** | loader | RobaNutritivneVrijednosti.xml | Web nutritivne vrijednosti | definitions/netgen/BIOVEGA/RobaNutritivneVrijednosti.xml | nema |
| `RobaPosebniAtributi2.xml` | 1 | **Biovega** | loader | RobaPosebniAtributi2.xml | Web posebni atributi | definitions/netgen/BIOVEGA/RobaPosebniAtributi2.xml | nema |
| `RobaUputeCuvanja.xml` | 1 | **Biovega** | loader | RobaUputeCuvanja.xml | Web upute čuvanja | definitions/netgen/BIOVEGA/RobaUputeCuvanja.xml | nema |
| `RobaWebKlase.xml` | 1 | **Biovega** | loader | RobaWebKlase.xml | Web roba klase | definitions/netgen/BIOVEGA/RobaWebKlase.xml | nema |
| `SessionAudit.xml` | 1 | **Biovega** | loader | SessionAudit.xml | Praćenje logiranja korisnika | NEDOSTAJE | nema |
| `sktIntOtMP.xml` | 1 | **Biovega** | loader | sktIntOtMP.xml | Interna Otpremnica - MPC BVG | definitions/netgen/BIOVEGA/sktIntOtMP.xml | nema |
| `sktkont.xml` | 1 | **Biovega** | drugo | sktkont.xml | Usklađivanje kontingenata | definitions/netgen/BIOVEGA/sktkont.xml | nema |
| `wmsinventura.xml` | 1 | **Biovega** | loader | skt\wmsinventura.xml | WMS inventura | definitions/netgen/BIOVEGA/wmsInventura.xml | nema |
| `admini.xml` | 1 |  | loader | admini.xml | Jupiter administratori | NEDOSTAJE | nema |
| `Ambalaza.xml` | 1 |  | loader | Ambalaza.xml | Evidencija težine ambalaže | NEDOSTAJE | nema |
| `BisnodeExport.xml` | 1 |  | loader | BisnodeExport.xml | Bisnode | definitions/netgen/Agronom granule/BisnodeExport.xml | nema |
| `bTobIreksNabNarHrSrb.xml` | 1 |  | loader | bTobIreksNabNarHrSrb.xml | Uvoz narudžbi (Ireks doo) | NEDOSTAJE | nema |
| `budzetiranje.xml` | 1 |  | drugo | budzetiranje.xml | Plan Proizvodnje Mjesečni | NEDOSTAJE | nema |
| `budzo.xml` | 1 |  | drugo | budzo.xml | Parametri Plana | NEDOSTAJE | nema |
| `Connexobustave.xml` | 1 |  | loader | Connexobustave.xml | Obustave | NEDOSTAJE | nema |
| `CoreKompLog.xml` | 1 |  | loader | CoreKompLog.xml | Core Components Trace | NEDOSTAJE | nema |
| `DD_arhiva.xml` | 1 |  | loader | DD_arhiva.xml | Arhiva obrada | NEDOSTAJE | nema |
| `EviIzlaza.xml` | 1 |  | loader | EviIzlaza.xml | Evidencija izlaza | NEDOSTAJE | nema |
| `EviUlaza.xml` | 1 |  | loader | EviUlaza.xml | Zabrana ulaza | NEDOSTAJE | nema |
| `HRDokumenti.xml` | 1 |  | frm | HRDokumenti.xml | Dokumenti | NEDOSTAJE | nema |
| `HRDopust.xml` | 1 |  | loader | HRDopust.xml | Manager plaćenih dopusta | NEDOSTAJE | nema |
| `HRDrzavljanstvo.xml` | 1 |  | frm | HRDrzavljanstvo.xml | Državljanstvo | NEDOSTAJE | nema |
| `HRKontakti.xml` | 1 |  | frm | HRKontakti.xml | Kontakti | NEDOSTAJE | nema |
| `HRManagerGO.xml` | 1 |  | loader | HRManagerGO.xml | Manager godišnjih odmora | definitions/netgen/Agronom granule/HRManagerGO.xml; definitions/netgen/BIOVEGA/HRManagerGO.xml | nema |
| `HrmIzvjestaji.xml` | 1 |  | loader | HrmIzvjestaji.xml | Izvještaji | NEDOSTAJE | nema |
| `HRNacionalnost.xml` | 1 |  | frm | HRNacionalnost.xml | Nacionalnost | NEDOSTAJE | nema |
| `HRRazlogDolaska.xml` | 1 |  | frm | HRRazlogDolaska.xml | Razlog zasnivanja RO | NEDOSTAJE | nema |
| `HRRazlogOdlaska.xml` | 1 |  | frm | HRRazlogOdlaska.xml | Razlog prestanka RO | NEDOSTAJE | nema |
| `HRSpreme.xml` | 1 |  | frm | HRSpreme.xml | Spreme | NEDOSTAJE | nema |
| `HRSrodstvo.xml` | 1 |  | frm | HRSrodstvo.xml | Srodstvo | NEDOSTAJE | nema |
| `HRStavkeGO.xml` | 1 |  | frm | HRStavkeGO.xml | Stavke godišnjeg odmora | NEDOSTAJE | nema |
| `HRVjestine.xml` | 1 |  | frm | HRVjestine.xml | Vještine | NEDOSTAJE | nema |
| `HRVrsteGO.xml` | 1 |  | frm | HRVrsteGO.xml | Vrste godišnjeg odmora | NEDOSTAJE | nema |
| `HRVrsteRO.xml` | 1 |  | frm | HRVrsteRO.xml | Vrste radnog odnosa | NEDOSTAJE | nema |
| `HRVrsteRV.xml` | 1 |  | frm | HRVrsteRV.xml | Vrste radnog vremena | definitions/netgen/Agronom granule/HRVrsteRV.xml | nema |
| `HRZvanja.xml` | 1 |  | frm | HRZvanja.xml | Zvanja | NEDOSTAJE | nema |
| `IreksAnalitikaTrosarina.xml` | 1 |  | loader | IreksAnalitikaTrosarina.xml | Trošarine - pregled analitika | NEDOSTAJE | nema |
| `IreksPregledTrosarina.xml` | 1 |  | loader | MER\IreksPregledTrosarina.xml | Trošarine - pregled kumulativ | NEDOSTAJE | nema |
| `IreksPrometTrosArtikala.xml` | 1 |  | loader | IreksPrometTrosArtikala.xml | Promet trošarinskih artikala | NEDOSTAJE | nema |
| `IreksPrometTrosArtikalaNe.xml` | 1 |  | loader | IreksPrometTrosArtikalaNe.xml | Promet troš. artikala NOVO | NEDOSTAJE | nema |
| `IreksTrosarina.xml` | 1 |  | loader | IreksTrosarina.xml | Pregled trošarina | NEDOSTAJE | nema |
| `IreksTrosarinaAna.xml` | 1 |  | loader | IreksTrosarinaAna.xml | Pregled trošarina analitika | NEDOSTAJE | nema |
| `IreksTrosarinaAnaNew.xml` | 1 |  | loader | IreksTrosarinaAnaNew.xml | Pregled troš. analitika NOVO | NEDOSTAJE | nema |
| `IreksTrosarinaNew.xml` | 1 |  | loader | IreksTrosarinaNew.xml | Pregled trošarina NOVO | NEDOSTAJE | nema |
| `IreksZaliheTrosArtikala.xml` | 1 |  | loader | IreksZaliheTrosArtikala.xml | Zalihe trošarinskih artikala | NEDOSTAJE | nema |
| `isokategorija.xml` | 1 |  | loader | isokategorija.xml | ISO kategorije zadataka | NEDOSTAJE | nema |
| `IspisEtiketa.xml` | 1 |  | loader | IspisEtiketa.xml | Ispis GS1 etiketa | NEDOSTAJE | nema |
| `IspisiFirme.xml` | 1 |  | loader | IspisiFirme.xml | Ispisi kod korisnika | NEDOSTAJE | nema |
| `klasetecajevi.xml` | 1 |  | loader | klasetecajevi.xml | Klase tečajeva                          ; Klase JA tečajeva | NEDOSTAJE | nema |
| `KntIOSObrazacDev.xml` | 1 |  | frm | KntIOSObrazacDev.xml | IOS Devizno | NEDOSTAJE | nema |
| `KntIspravakVrijednosti.xml` | 1 |  | loader | KntIspravakVrijednosti.xml | Ispravak vrijednosti | NEDOSTAJE | nema |
| `KntOcekivaniPriljevOdljev.xml` | 1 |  | loader | KntOcekivaniPriljevOdljev.xml | Dospijeće po periodima | NEDOSTAJE | nema |
| `kntRekapOtvDob.xml` | 1 |  | loader | kntRekapOtvDob.xml | Obaveze prema dobavljačima | NEDOSTAJE | nema |
| `KntVirmaniKoop.xml` | 1 |  | loader | KntVirmaniKoop.xml | Generator Virmana | NEDOSTAJE | nema |
| `KOD_Arhiva.xml` | 1 |  | loader | KOD_Arhiva.xml | Arhiva obrada | NEDOSTAJE | nema |
| `KOD_Korekcija.xml` | 1 |  | loader | KOD_Korekcija.xml | Korekcija plaće | definitions/netgen/Agronom granule/KOD_Korekcija.xml | nema |
| `KOD_Place.xml` | 1 |  | loader | KOD_Place.xml | Plaće | NEDOSTAJE | nema |
| `KOD_Stavke.xml` | 1 |  | loader | KOD_Stavke.xml | Dohodovne stavke | definitions/netgen/BIOVEGA/KOD_Stavke.xml | nema |
| `KontniPlanHR-ENG.xml` | 1 |  | loader | KontniPlanHR-ENG.xml | Kontni plan - višejezično | definitions/netgen/Agronom granule/KontniPlanHR-ENG.xml | nema |
| `KTEukpNG.xml` | 1 |  | loader | KTEukpNG.xml | Kamate UKP NOVO | definitions/netgen/Agronom granule/KTEukpNG.xml | nema |
| `KUPDostKNAgroFructus.xml` | 1 |  | loader | KUPDostKNAgroFructus.xml | Dostavna knjiga | NEDOSTAJE | nema |
| `KUPDostKNECS_URA.xml` | 1 |  | loader | KUPDostKNECS_URA.xml | Dostavna knjiga URA; KUP Dostavna knjiga | NEDOSTAJE | nema |
| `KUPDostKNIreks.xml` | 1 |  | loader | KUPDostKNIreks.xml | Dostavna knjiga | NEDOSTAJE | nema |
| `KUPDostKnjigaTEST.xml` | 1 |  | loader | KUPDostKnjigaTEST.xml | KUP Dostavna knjiga TEST | NEDOSTAJE | nema |
| `KUPDostKNOdobravanje.xml` | 1 |  | loader | KUPDostKNOdobravanje.xml | KUP Dostavna knjiga | NEDOSTAJE | nema |
| `KUPNGAgroFructus.xml` | 1 |  | loader | KUPNGAgroFructus.xml | Knjiga ulazne pošte | NEDOSTAJE | nema |
| `KUPNGAgroFructusDMSScan.xml` | 1 |  | loader | KUPNGAgroFructusDMSScan.xml | Knjiga pošte SCAN | NEDOSTAJE | nema |
| `KUPNGECS.xml` | 1 |  | loader | kup\KUPNGECS.xml | Knjiga pošte | NEDOSTAJE | nema |
| `KUPNGECS_BackOffice_URA.xml` | 1 |  | loader | KUPNGECS_BackOffice_URA.xml | Knjiga pošte BackOffice URA | NEDOSTAJE | nema |
| `KUPNGECS_URA.xml` | 1 |  | loader | KUPNGECS_URA.xml | Knjiga pošte URA | NEDOSTAJE | nema |
| `KUPNGIreks.xml` | 1 |  | loader | KUPNGIreks.xml | Knjiga pošte | NEDOSTAJE | nema |
| `KUPNGSpin.xml` | 1 |  | loader | KUPNGSpin.xml | Knjiga pošte | NEDOSTAJE | nema |
| `KUPNGSpin_KUP_URA.xml` | 1 |  | loader | KUPNGSpin_KUP_URA.xml | Knjiga pošte KUP_URA | NEDOSTAJE | nema |
| `KUPNGTest.xml` | 1 |  | loader | KUPNGTest.xml | Knjiga pošte TEST | NEDOSTAJE | nema |
| `KUPNG_ISOOdobravanje.xml` | 1 |  | loader | KUPNG_ISOOdobravanje.xml | ISO odobravanje | NEDOSTAJE | nema |
| `KUPStatNLinijeOdo.xml` | 1 |  | loader | KUPStatNLinijeOdo.xml | Status pošte | NEDOSTAJE | nema |
| `KUPStatNRedOdo.xml` | 1 |  | loader | KUPStatNRedOdo.xml | Status ulazne pošte | NEDOSTAJE | nema |
| `kupura.xml` | 1 |  | loader | kupura.xml | Povezivanje KUP_URA | NEDOSTAJE | nema |
| `KUPVrsPTip.xml` | 1 |  | loader | KUPVrsPTip.xml | Vrsta pošte | NEDOSTAJE | nema |
| `MENIPubVars.xml` | 1 |  | loader | MENIPubVars.xml | PubVarsi kod korisnika | NEDOSTAJE | nema |
| `MPAgronomEtiketeCjenikNG.xml` | 1 |  | loader | MPAgronomEtiketeCjenikNG.xml | MP Etikete za robu iz cjenika | definitions/netgen/Agronom granule/MPAgronomEtiketeCjenikNG.xml | nema |
| `MPAppManager.xml` | 1 |  | loader | MPAppManager.xml | Upravljanje mob. app za MP | NEDOSTAJE | nema |
| `MPBlagajnaFIS.xml` | 1 |  | loader | MPBlagajnaFIS.xml | Blagajna FIS | NEDOSTAJE | nema |
| `MPBlagajnaPOS.xml` | 1 |  | loader | MPBlagajnaPOS.xml | MP Blagajna - online | NEDOSTAJE | nema |
| `MPBlagajnaPOSNaturala.xml` | 1 |  | loader | MPBlagajnaPOSNaturala.xml | MP blagajna POS | NEDOSTAJE | nema |
| `MPBlagajnaSVEIreks.xml` | 1 |  | loader | MPBlagajnaSVEIreks.xml | Blagajna MP iz VP-NOVO | NEDOSTAJE | nema |
| `MPBrojIskaznicaZastitaNEW.xml` | 1 |  | loader | MPBrojIskaznicaZastitaNEW.xml | Iskaznica zastita | NEDOSTAJE | nema |
| `MPBrojIskazniceZastita.xml` | 1 |  | loader | MPBrojIskazniceZastita.xml | Ažur.broja iskaznice-Zaštita | definitions/netgen/Agronom granule/MPBrojIskazniceZastita.xml | nema |
| `MPDirZaduzenjeNG1Mlinar.xml` | 1 |  | loader | MPDirZaduzenjeNG1Mlinar.xml | MP primka | NEDOSTAJE | nema |
| `MPEtiketeCjenikNG.xml` | 1 |  | loader | MPEtiketeCjenikNG.xml | Ispis etiketa MP iz cjenika | NEDOSTAJE | nema |
| `MPFisRoba.xml` | 1 |  | loader | MPFisRoba.xml | FIS - Zaštitna sredstva | definitions/netgen/Agronom granule/MPFISRoba.xml | nema |
| `MPKlasaIsklCijena.xml` | 1 |  | loader | MPKlasaIsklCijena.xml | Isključenje klasa za sidrenje cijene | definitions/netgen/BIOVEGA/MPKlasaIsklCijena.xml | nema |
| `MPNarA.xml` | 1 |  | loader | MPNarA.xml | Narudžbenica dobavljaču iz MP | NEDOSTAJE | nema |
| `MPPonuda.xml` | 1 |  | loader | MPPonuda.xml | MP Ponuda NG | definitions/netgen/Agronom granule/MPPonuda.xml; definitions/netgen/BIOVEGA/MPPonuda.xml | docs/granule/MPPonuda.md |
| `MPPonudaIreks.xml` | 1 |  | loader | MPPonudaIreks.xml | MP Ponuda | NEDOSTAJE | nema |
| `MPPovratDobNGMlinar.xml` | 1 |  | loader | MPPovratDobNGMlinar.xml | Povrat dobavljaču iz MP | NEDOSTAJE | nema |
| `MPPredujam.xml` | 1 |  | loader | MPPredujam.xml | Predujmovi u MP | NEDOSTAJE | nema |
| `MPPregRbTrD.xml` | 1 |  | loader | MPPregRbTrD.xml | Pregled robe po trgovinama | NEDOSTAJE | nema |
| `MPPrmArtNG.xml` | 1 |  | loader | MPPrmArtNG.xml | Pregled prometa artikala u MP | definitions/netgen/Agronom granule/MPPrmArtNG.xml | nema |
| `MPPrMPRNGIreks.xml` | 1 |  | loader | MPPrMPRNGIreks.xml | Pregled MP računa | NEDOSTAJE | nema |
| `MPRobaUslugaRNNG.xml` | 1 |  | loader | MPRobaUslugaRNNG.xml | Pregled robe+usluga po MP rač. | NEDOSTAJE | nema |
| `MPRobaZabrana.xml` | 1 |  | loader | MPRobaZabrana.xml | Zabrana robe na blagajni | NEDOSTAJE | nema |
| `MPRobnoDOk.xml` | 1 |  | loader | MPRobnoDOk.xml | Status MP dokumenata | NEDOSTAJE | nema |
| `MPUtrzakMPVP.xml` | 1 |  | loader | MPUtrzakMPVP.xml | Utržak MP iz VP | NEDOSTAJE | nema |
| `NabPriLAA.xml` | 1 |  | loader | MER\NabPriLAA.xml | LAA po primkama | NEDOSTAJE | nema |
| `NabStoNarucitiNG.xml` | 1 |  | loader | NabStoNarucitiNG.xml | Što naručiti? | definitions/netgen/Agronom granule/NabStoNarucitiNG.xml | nema |
| `Narudzbe_new.xml` | 1 |  | loader | Narudzbe_new.xml | Narudžbe | NEDOSTAJE | nema |
| `NOTPNGBROJ.xml` | 1 |  | loader | NOTPNGBROJ.xml | Manager otpreme - ponude | NEDOSTAJE | nema |
| `OpasneKemikalijeNG.xml` | 1 |  | loader | OpasneKemikalijeNG.xml | Opasne Kemikalije | NEDOSTAJE | nema |
| `OpasneNabavaNG.xml` | 1 |  | loader | OpasneNabavaNG.xml | Nabava opasnih tvari | NEDOSTAJE | nema |
| `OpasneProizvodnjaNG.xml` | 1 |  | loader | OpasneProizvodnjaNG.xml | Proizvodnja opasnih tvari | NEDOSTAJE | nema |
| `OpasnePrometNG.xml` | 1 |  | loader | OpasnePrometNG.xml | Promet opasnih tvari | NEDOSTAJE | nema |
| `OpasneStanjeNG.xml` | 1 |  | loader | OpasneStanjeNG.xml | Stanje opasnih tvari | NEDOSTAJE | nema |
| `OpasneTvariPregledNG.xml` | 1 |  | loader | OpasneTvariPregledNG.xml | Opasne Tvari - Pregled | NEDOSTAJE | nema |
| `OpasneTvariUrednikNG.xml` | 1 |  | loader | OpasneTvariUrednikNG.xml | Opasne Tvari - Urednik | NEDOSTAJE | nema |
| `OtkupniListAF.xml` | 1 |  | loader | SKLAD\OtkupniListAF.xml | Otkupni list | NEDOSTAJE | nema |
| `otprema.xml` | 1 |  | loader | mer/otprema.xml | Manager utovara | NEDOSTAJE | nema |
| `ParJm.xml` | 1 |  | loader | ParJm.xml | Partneri JM | NEDOSTAJE | nema |
| `PNKatalozi_ENNA.xml` | 1 |  | loader | PNKatalozi_ENNA.xml | PN Katalozi | NEDOSTAJE | nema |
| `PNNaloziMoji.xml` | 1 |  | loader | PNNaloziMoji.xml | Putni nalozi-moji | NEDOSTAJE | nema |
| `PredatnicaLAA.xml` | 1 |  | loader | SKLAD\PredatnicaLAA.xml | LAA po predatnicama | NEDOSTAJE | nema |
| `PregledOperateraNaDok.xml` | 1 |  | loader | MER\PregledOperateraNaDok.xml | Pregled operatera na dok | NEDOSTAJE | nema |
| `PrnEV.xml` | 1 |  | loader | PrnEV.xml | Energ. vrijed. sir. | NEDOSTAJE | nema |
| `prnKeub.xml` | 1 |  | loader | MFG\prnKeub.xml | KEUB Knjige | NEDOSTAJE | nema |
| `prnkeubr.xml` | 1 |  | loader | prnkeubr.xml | Robe u KEUB-u | NEDOSTAJE | nema |
| `prnMSKLizPredatnice.xml` | 1 |  | loader | MFG\prnMSKLizPredatnice.xml | Kreiranje MSKL ambalaže | NEDOSTAJE | nema |
| `ProPonKolPotvrdi.xml` | 1 |  | loader | ProPonKolPotvrdi.xml | Potvrđivanje narudžbi | NEDOSTAJE | nema |
| `ProPotvrdiNarudzbu.xml` | 1 |  | loader | ProPotvrdiNarudzbu.xml | Potvrđivanje e-narudžbi | NEDOSTAJE | nema |
| `Protokoli.xml` | 1 |  | loader | Protokoli.xml | Protokoli | NEDOSTAJE | nema |
| `ProtokolKatalog.xml` | 1 |  | frm | ProtokolKatalog.xml | Katalozi protokola | NEDOSTAJE | nema |
| `Racuni_za_carinuIreks.xml` | 1 |  | loader | Racuni_za_carinuIreks.xml | PDV po uvozu | NEDOSTAJE | nema |
| `RASCjenik.xml` | 1 |  | loader | RASCjenik.xml | Cjenik | NEDOSTAJE | nema |
| `RASGrupe.xml` | 1 |  | loader | RASGrupe.xml | Grupe radnika | NEDOSTAJE | nema |
| `RASRaspored.xml` | 1 |  | loader | RASRaspored.xml | Raspored | NEDOSTAJE | nema |
| `RazlikeUvjetaDokUgovor.xml` | 1 |  | loader | RazlikeUvjetaDokUgovor.xml | Razlike uvjeta ugovor/dok | NEDOSTAJE | nema |
| `RBProdaja.xml` | 1 |  | loader | RBProdaja.xml | Prodaja ribe | NEDOSTAJE | nema |
| `RealizacijaOtpNG.xml` | 1 |  | loader | MER\RealizacijaOtpNG.xml | Realizacija otp - stavke | NEDOSTAJE | nema |
| `RealizacijaPonuda.xml` | 1 |  | loader | MER\RealizacijaPonuda.xml | Realizacija ponuda | definitions/netgen/Agronom granule/RealizacijaPonuda.xml | nema |
| `RealizacijaPonudaStanje.xml` | 1 |  | loader | MER\RealizacijaPonudaStanje.xml | Realizacija ponuda sa stanjem | NEDOSTAJE | nema |
| `RobaDodatniAtr.xml` | 1 |  | loader | UTP\RobaDodatniAtr.xml | Dodatni atributi robe | NEDOSTAJE | nema |
| `RobaNaknade.xml` | 1 |  | loader | RobaNaknade.xml | Evidencija naknada i trošarina | NEDOSTAJE | nema |
| `robapakpovng.xml` | 1 |  | loader | SKLAD/robapakpovng.xml | Pretvorba Stari kruh | definitions/netgen/BIOVEGA/RobaPakPovNG.xml | nema |
| `RobaZemljaPorijekla.xml` | 1 |  | loader | UTP\RobaZemljaPorijekla.xml | Zemlja preferen. porijekla | NEDOSTAJE | nema |
| `SiraOtp.xml` | 1 |  | loader | SiraOtp.xml | WMS Provedi | NEDOSTAJE | nema |
| `SKLTrosarinskiIspisIreks.xml` | 1 |  | loader | SKLTrosarinskiIspisIreks.xml | Prateći trošarinski dok. | NEDOSTAJE | nema |
| `sktRegalUlaz.xml` | 1 |  | loader | sktRegalUlaz.xml | Ulaz paleta | NEDOSTAJE | nema |
| `SktUtovarEurovoceNG.xml` | 1 |  | loader | SktUtovarEurovoceNG.xml | Manager Utovara | NEDOSTAJE | nema |
| `StanjeAmbalazeNG.xml` | 1 |  | loader | StanjeAmbalazeNG.xml | Stanje Ambalaže                         ; Stanje Ambalaže | NEDOSTAJE | nema |
| `statusadmin.xml` | 1 |  | loader | statusadmin.xml | Status Jupiter administratora | NEDOSTAJE | nema |
| `TDObracun.xml` | 1 |  | loader | TDObracun.xml | Obračun terenskog dodatka | NEDOSTAJE | nema |
| `Tecajevi.xml` | 1 |  | loader | Tecajevi.xml | Tečajevi | NEDOSTAJE | nema |
| `UgovorSKupcima.xml` | 1 |  | loader | UgovorSKupcima.xml | Ugovori s kupcima | definitions/netgen/Agronom granule/UgovorSKupcima.xml; definitions/netgen/BIOVEGA/UgovorSKupcima.xml | nema |
| `voziloex.xml` | 1 |  | loader | mer/voziloex.xml | Kapacitet vozila | NEDOSTAJE | nema |
| `wmsstanje.xml` | 1 |  | loader | wmsstanje.xml | Zahtjev za WMS stanjem | NEDOSTAJE | nema |
| `WSLogin.xml` | 1 |  | loader | WSLogin.xml | Web Shop Login | NEDOSTAJE | nema |
| `wwwkomp.xml` | 1 |  | loader | wwwkomp.xml | WWWkompenzacije | NEDOSTAJE | nema |
| `ZahtjevZaNabavkuNG.xml` | 1 |  | loader | MER\ZahtjevZaNabavkuNG.xml | Zahtjevi za nabavku | definitions/netgen/Agronom granule/ZahtjevZaNabavkuNG.xml | nema |
| `zapornjak.xml` | 1 |  | loader | zapornjak.xml | Zamjena lozinke | NEDOSTAJE | nema |

## Nedostaje XML

Datoteke kojih nema u `definitions/netgen`, poredane po broju baza. **Biovega** znaci da je datoteka u izborniku te baze.

| XML | Baza | Biovega | Baze | Naziv stavke | Putanja u meniju |
|---|---:|:---:|---|---|---|
| `TecRazOTV.xml` | 10 | **Biovega** | Biovega (jupiter2019); CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019); Ireks (jupiter2019); spin (sql-ai) | Teč. razlike na otv. stavke; Teč razlike na Otv. stavke NG ; Teč razlike na Otv. stavke              ; Teč razlike na Otv. stavke NG; Teč razlike na Otv. stavke NG | TecRazOTV.xml |
| `Poruke.xml` | 9 | **Biovega** | Biovega (jupiter2019); CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019); spin (sql-ai) | Poruke; Standardne napomene           ; Standardne napomene                     ; Standardne napomene | Poruke.xml |
| `PorukeDoc.xml` | 7 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019); spin (sql-ai) | Tekstovi na KomDoc; Tekstovi na KomDoc | PorukeDoc.xml |
| `KntBlaVrste.xml` | 6 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); spin (sql-ai) | Vrste blagajni                ; Vrste blagajni                          ; Vrste blagajni | KntBlaVrste.xml |
| `MPTrgovine.xml` | 6 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Ireks (jupiter2019); spin (sql-ai) | Trgovine                      ; Trgovine                                ; Trgovine | MPTrgovine.xml |
| `ZahtjevZaIzdavanje.xml` | 5 | **Biovega** | Biovega (jupiter2019); CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Panissimo (jupiter2022); Tusak (jupiter2022) | Zahtjev za izdavanje; Zahtjev za izdavanje          ; Zahtjev za izdavanje | skt\ZahtjevZaIzdavanje.xml; MER\ZahtjevZaIzdavanje.xml |
| `KntIzvodTijek.xml` | 5 |  | Setcor (jupiter2019); Setcor (jupiter2022); Enna (jupiter2019); Ireks (jupiter2019); spin (sql-ai) | Tijek novca-direktna metoda; Tijek novca direktna metoda; Tijek novca-direktna metoda | KntIzvodTijek.xml |
| `KntPriGen.xml` | 5 |  | Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); spin (sql-ai) | Prijeboji generator                     ; Prijeboji generator | KntPriGen.xml |
| `KntZatvaranjeNG.xml` | 5 |  | Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); spin (sql-ai) | Zatvaranje klase 4 i 7                  ; Zatvaranje klase 4 i 7 | KntZatvaranjeNG.xml |
| `ManPokazateljiGrupe.xml` | 5 |  | Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022); spin (sql-ai) | Grupe pokazatelja                       ; Grupe pokazatelja | ManPokazateljiGrupe.xml |
| `MPKonMpSkl.xml` | 4 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019) | Kontrola MP SKL; Kontrola MP -> SKL                      ; Kontrola MP->SKL | MPKonMpSkl.xml |
| `kntKontrolaPDV.xml` | 4 |  | Panissimo (jupiter2022); Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022) | Kontrola PDV-a                          ; Kontrola PDV-a | kntKontrolaPDV.xml |
| `MPVezCijng.xml` | 4 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Panissimo (jupiter2022); Tusak (jupiter2022) | Vezanje mp cjenika            ; Uređivanje cjenika MP | MPVezCijng.xml |
| `PDVPromjena.xml` | 4 |  | Panissimo (jupiter2022); Setcor (jupiter2019); Tusak (jupiter2022); spin (sql-ai) | Promjena stope PDV-a | PDVPromjena.xml |
| `MP4RedaRac.xml` | 3 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Tusak (jupiter2022) | Poruke na računima; Poruke na računu | MP4RedaRac.xml |
| `MPBankaPOSKod.xml` | 3 | **Biovega** | Biovega (jupiter2019); CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP Banka POS Kod; Kod banke za POS | MPBankaPOSKod.xml |
| `MPKOntrolaPrijenosaKasa.xml` | 3 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Tusak (jupiter2022) | Kontrola prijenosa kasa; Analiza prijenoa blagajni | MPKOntrolaPrijenosaKasa.xml |
| `MPParStru.xml` | 3 | **Biovega** | Biovega (jupiter2019); Enna (jupiter2019); Ireks (jupiter2019) | Ažuriranje GLN lokacija | MPParStru.xml |
| `MPPoruke.xml` | 3 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Tusak (jupiter2022) | Poruke za MP; Poruke za MP | MPPoruke.xml |
| `MPUgoPDV.xml` | 3 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022); Tusak (jupiter2022) | Ugostiteljski PDV; Ugostiteljski PDV | MPUgoPDV.xml |
| `Inventura.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Ireks (jupiter2019) | Inventura unos                          ; Inventura NG | Inventura.xml |
| `IzvKlasePartneriParstru.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Pregled klasa za partnera               ; Pregled klasa za partnera | IzvKlasePartneriParstru.xml |
| `jesteliznali.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Manager Zanimljivosti | jesteliznali.xml |
| `JMjereNG.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022) | Jedinice mjere                          ; Jedinice mjere | UTP\JMjereNG.xml |
| `KntOpomene.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Opomene NG; Opomene NG | KntOpomene.xml |
| `KntRasporedTros.xml` | 3 |  | Setcor (jupiter2022); Ireks (jupiter2019); spin (sql-ai) | Raspored troškova; Raspored troškova | KntRasporedTros.xml |
| `KntZakljucniListBB.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); Ireks (jupiter2019) | Bruto Bilanca | KntZakljucniListBB.xml |
| `KonfigSrvRN.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Konfigurator uslužnih RN | KonfigSrvRN.xml |
| `KUPPregNG.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); Ireks (jupiter2019) | Pregled pošte; KUP Pregled pošte | KUPPregNG.xml |
| `ManGadTip.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Manager indikatora; Manager indikatora | ManGadTip.xml |
| `MPAsortimanAzurN.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019) | Asortiman dobavljača                    ; Asortiman dobavljača | MPAsortimanAzurN.xml |
| `MPPovratDobNG.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019) | Povrat dobavljaču iz MP                 ; Povrat dobavljaču iz MP | MPPovratDobNG.xml |
| `MPRekapPorez.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019) | Rekapitulacija poreza po dok.           ; Rekapitulacija poreza po dok. | MPRekapPorez.xml |
| `NabNarPregledNarucenog.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Enna (jupiter2019) | Pregled naručene robe                   ; Pregled naručene robe | NabNarPregledNarucenog.xml |
| `newswebsite.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Manager web stranica | newswebsite.xml |
| `OOPIzv.xml` | 3 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022); Setcor (jupiter2019) | Izvještaji NG                 ; Izvještaji NG | OOPIzv.xml |
| `OrgShemaKonf.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Konfigurator Org sheme                  ; Konfigurator Org sheme | OrgShemaKonf.xml |
| `OsobeOperateri.xml` | 3 |  | Enna (jupiter2019); Ireks (jupiter2019); spin (sql-ai) | Veza osobe - operateri; Osobe i operateri; Osobe i operateri | OsobeOperateri.xml |
| `proRajoni.xml` | 3 |  | Panissimo (jupiter2022); Tusak (jupiter2022); Ireks (jupiter2019) | Ažuriranje linija                       ; Ažuriranje linija | skt\proRajoni.xml |
| `PubVarsNG.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Proizvodnja Konfigurator | PubVarsNG.xml |
| `ServisJamstvo3.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Obračun jamstva trećih | ServisJamstvo3.xml |
| `Spojka.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Spajanje lokacija                       ; Spajanje lokacija | Spojka.xml |
| `TarifaNG.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); Tusak (jupiter2022) | Tarifa                                  ; Tarifa | UTP\TarifaNG.xml |
| `Upitnici.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Upitnici | MAR\Upitnici.xml |
| `VoziloFinRekapNG.xml` | 3 |  | Setcor (jupiter2019); Setcor (jupiter2022); spin (sql-ai) | Rekapitulacija troškova vozila | VoziloFinRekapNG.xml |
| `MPPregNapojnica.xml` | 2 | **Biovega** | Biovega (jupiter2019); Panissimo (jupiter2022) | Pregled napojnica; Pregled napojnica | MPPregNapojnica.xml |
| `SktSerBrojAzur.xml` | 2 | **Biovega** | Biovega (jupiter2019); spin (sql-ai) | Izmjena serijskog broja; Ažuriranje broja kontigenta | SktSerBrojAzur.xml |
| `C2620_test.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Stanje VPMP | C2620_test.xml |
| `CekBlagajnaNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Unos čekova | CekBlagajnaNG.xml |
| `CekObradaNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Obrada čekova | CekObradaNG.xml |
| `DD_Obrade.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Obrada | DD_Obrade.xml |
| `DD_PredVirmani.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Predlošci virmana | DD_PredVirmani.xml |
| `DjelatnostiNG.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Djelatnosti                             ; Djelatnosti | UTP\DjelatnostiNG.xml |
| `dmsstatus.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | DMS Status; DMS Status | dmsstatus.xml |
| `dmsVrstadokumenta.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | DMS Vrsta dokumenta; DMS Vrsta dokumenta | dmsVrstadokumenta.xml |
| `Dne_Det_ECS.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Dnevna evidencija                       ; Dnevna evidencija | Dne_Det_ECS.xml |
| `Dne_Det_MK.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Dnevna evidencija | Dne_Det_MK.xml |
| `HRCKadManager.xml` | 2 |  | Setcor (jupiter2019); Ireks (jupiter2019) | Manager ljudskih resursa                ; Manager kadrova NG; Manager kadrova | HRCKadManager.xml |
| `HRCRMManager.xml` | 2 |  | Setcor (jupiter2019); Ireks (jupiter2019) | Manager radnih mjesta                   ; Manager radnih mjesta NG | HRCRMManager.xml |
| `IspisProDokSI.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Ispis proizvodnje | mfg/IspisProDokSI.xml |
| `IzvIskoristeniKalo.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled Kala | IzvIskoristeniKalo.xml |
| `JupiterTran.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Translator                              ; Translator | JupiterTran.xml |
| `kategorija.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Kategorije zadataka; Kategorije zadataka | kategorija.xml |
| `KntIOSObrazac.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | IOS Obrazac NG | KntIOSObrazac.xml |
| `KntKnjizenjeProizvodnje.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Knjiženje proizvodnje | KntKnjizenjeProizvodnje.xml |
| `KntPDVObrazacProjekcija.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | PDV projekcija | KntPDVObrazacProjekcija.xml |
| `KOD_Donos.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Stalni dodaci                           ; Stalni dodaci | KOD_Donos.xml |
| `KOD_Doprinosi.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Doprinosi                               ; Doprinosi | KOD_Doprinosi.xml |
| `KOD_GodPorez.xml` | 2 |  | Setcor (jupiter2019); Ireks (jupiter2019) | Konačni obračun poreza | KOD_GodPorez.xml |
| `KOD_IzmjenaVirmana.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Izmjena naloga za plaćanje              ; Izmjena naloga za plaćanje | KOD_IzmjenaVirmana.xml |
| `KOD_PorezIzDohotka.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Porez iz dohotka                        ; Porez iz dohotka | KOD_PorezIzDohotka.xml |
| `KUPDostKNECS.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | KUP Dostavna knjiga; KUP Dostavna knjiga STARA | kup\KUPDostKNECS.xml; KUPDostKNECS.xml |
| `KUPDostKNOdobravanjeTusak.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Dostavna knjiga | KUPDostKNOdobravanjeTusak.xml |
| `KUPNGMlinar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Knjiga pošte | KUPNGMlinar.xml |
| `KUPNGTusak_KUP_URA.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Knjiga pošte | KUPNGTusak_KUP_URA.xml |
| `KUPPregOdobMlinar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | KUP Pregled odobravanja | KUPPregOdobMlinar.xml |
| `MercurNaloziOtp.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Otprema naloga | MercurNaloziOtp.xml |
| `ModeliNaplateNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Modeli naplate | MER\ModeliNaplateNG.xml |
| `MPAnalPrNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Analiza računa po vremenu | MPAnalPrNG.xml |
| `MPAzurTrazilicaRobe.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pravo na tražilici robe | MPAzurTrazilicaRobe.xml |
| `MPBlagajnaWEB.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Web Blagajna | MPBlagajnaWEB.xml |
| `MPBonovi.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Bonovi | MPBonovi.xml |
| `MPBonoviPregled.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled bonova | MPBonoviPregled.xml |
| `MPCopyDok.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Kopiranje dokumenata | MPCopyDok.xml |
| `MPDeAktTrgovina.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Trgovina DE(aktivacija) | MPDeAktTrgovina.xml |
| `MPDirZaduzenjeKomNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Dir. zaduženje - KOMISIJA | MPDirZaduzenjeKomNG.xml |
| `MPDirZaduzenjeNG1.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | MP Primka od dobavljača | MPDirZaduzenjeNG1.xml |
| `MPDirZaduzenjeNG1BobisGRATIS.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | MP Primka - Gratis | MPDirZaduzenjeNG1BobisGRATIS.xml |
| `MPDodKlsRbaKasa.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Dodatne jednice mjere za kasu ; Dodatne klase za kasu | MPDodKlsRbaKasa.xml |
| `MPEizvod.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | E-Izvod MP | MPEizvod.xml |
| `MPEizvodPregled.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled knjiženja E-izvoda | MPEizvodPregled.xml |
| `MPEtiketeNGCentarTehnike.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP Etikete sa dokumenta | MPEtiketeNGCentarTehnike.xml |
| `MPEtiketeTerminalCT.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pojedinačne etikete | MPEtiketeTerminalCT.xml |
| `MPFakOtpGRUP.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Fakturiranje obustava u mp | MPFakOtpGRUP.xml |
| `MPGrupeRobe2.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Predložak tipkovnice | MPGrupeRobe2.xml |
| `MPIntDOkKalo.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Kalo u MP - dokument | MPIntDOkKalo.xml |
| `MPInventuraVrsteRoba.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP inventura - NOVA | MPInventuraVrsteRoba.xml |
| `MPIzdVPMPMlinar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Izdatnica iz VP u MP                    ; Izdatnica u MP | MPIzdVPMPMlinar.xml |
| `MPizdVPMPPVC.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Izdatnica PVC vrećice | MPizdVPMPPVC.xml |
| `MPKalkKnjizenjeMULTI.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Povezivanje MP kalk i UFA | MPKalkKnjizenjeMULTI.xml |
| `MPKnjigaSanka.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Knjiga šanka | MPKnjigaSanka.xml |
| `MPKonFinNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Kontrola MP - Fin | MPKonFinNG.xml |
| `MPKontrolaKalk.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Kontrola knjiženja kalk. | MPKontrolaKalk.xml |
| `MPKOntrolaPredispozicija.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Kontrola predispozicija | MPKOntrolaPredispozicija.xml |
| `MPKontrolaPrometaStanja.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Kontrola Promet-Stanje | MPKontrolaPrometaStanja.xml |
| `MPKontrolaSK.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Kontrola sindikalnih kredita | MPKontrolaSK.xml |
| `MPKOntrolaUGOPDV.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Kontrola UGO PDV-a | MPKOntrolaUGOPDV.xml |
| `MPKorekcijaNVMJ.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Mjesečna korekcija stanja | MPKorekcijaNVMJ.xml |
| `MPKrivoStanjeKomisije.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Neispravno stanje komis. robe | MPKrivoStanjeKomisije.xml |
| `MPListaAmb.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Lista ambalaže | MPListaAmb.xml |
| `MPlnKlasaKasa.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Ažur. robe za klase na kasi | MPlnKlasaKasa.xml |
| `MPMinMaxCentarTehnike.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Min i max zalihe CT | MPMinMaxCentarTehnike.xml |
| `MPMinMaxKonikom.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Min i max zalihe | MPMinMaxKonikom.xml |
| `MPMinMaxPr.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Što naručiti - MinMax zalihe | MPMinMaxPr.xml |
| `MPMOBAnaliza.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Analiza MP MOB aplikacije | MPMOBAnaliza.xml |
| `MPNajArtikli.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | TOP artikli u MP | MPNajArtikli.xml |
| `MPNajavaObnoveNC.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Najava obnove NC | MPNajavaObnoveNC.xml |
| `MPNarDneProiz.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Narudžba redovne robe                   ; Narudžba vlastite robe | MPNarDneProiz.xml |
| `MPNarDneProizSMRZ.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Narudžba SMRZNUTE robe | MPNarDneProizSMRZ.xml |
| `MPNarDneProizTorte.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Extra narudžba | MPNarDneProizTorte.xml |
| `MPNarPredlosci.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Definiranje predložaka | MPNarPredlosci.xml |
| `MPNarucenoIsporuceno.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Naručeno-isporučeno u MP | MPNarucenoIsporuceno.xml |
| `MPNeObracunProi.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Proizvodi - NE obračun | MPNeObracunProi.xml |
| `MPnivelNCNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Izmjena NC u MP | MPnivelNCNG.xml |
| `MPObracunKalaPeriod.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Obračun kala u periodu | MPObracunKalaPeriod.xml |
| `MPOptimoKategorije.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP dostave prioritet kat. | MPOptimoKategorije.xml |
| `MPOptimoVozac.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | OptimoRoute Vozači | MPOptimoVozac.xml |
| `MPOtprMPNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Dostava iz MP | MPOtprMPNG.xml |
| `MPPonudaCentarTehnike.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP Ponuda                     ; MP Ponuda NG | MPPonudaCentarTehnike.xml |
| `MPPonudaTusak.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | MP Ponuda | MPPonudaTusak.xml |
| `MPPovNaknada.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Ukidanje POV naknad | MPPovNaknada.xml |
| `MPPovOdKup.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Povrat robe od kupca | MPPovOdKup.xml |
| `MPPovuVPTusakRedov.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Povratnica iz MP u VP-redovna | MPPovuVPTusakRedov.xml |
| `MPPovUVPTusakTNR.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Povratnica iz MP u VP-TNR | MPPovUVPTusakTNR.xml |
| `MPPregAkcijaMC.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled akcije MC | MPPregAkcijaMC.xml |
| `MPPregDirZaduzenjaKum.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled mp primki - kumulativ | MPPregDirZaduzenjaKum.xml |
| `MPPregDneNar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled dnevnih narudžbi | MPPregDneNar.xml |
| `MPPregDnevP.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled stanja dnev.proiz. | MPPregDnevP.xml |
| `MPPregExtraNar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled extra narudžbi | MPPregExtraNar.xml |
| `MPPregInvRazlika.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled inv. razlika | MPPregInvRazlika.xml |
| `MPPregMPCjenik.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled MP cjenika | MPPregMPCjenik.xml |
| `MPPregMPCnaKasi.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled promjene MP cijene | MPPregMPCnaKasi.xml |
| `MPPregNarPotrosniSkl.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Interna otpremnica - Potrošni | MPPregNarPotrosniSkl.xml |
| `MPPregNCUgovorDob.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled NC iz ugov.dobavljača | MPPregNCUgovorDob.xml |
| `MPPregProizCijene.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled proizvoda - Cijene | MPPregProizCijene.xml |
| `MPPregProvizije.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled provizija | MPPregProvizije.xml |
| `MPPregRACVANRV.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled rač. van rad. vremena | MPPregRACVANRV.xml |
| `MPPregRbTrCT.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Preg. robe po trgovinama NG | MPPregRbTrCT.xml |
| `MPPregStanjeProizvodjac.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje robe po proizvođaču | MPPregStanjeProizvodjac.xml |
| `MPPregZaduzenjaMPMlinar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled zaduženja MP iz VP | MPPregZaduzenjaMPMlinar.xml |
| `MPPrInvKumulativNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled inventure MP-kumulativno | MPPrInvKumulativNG.xml |
| `MPPrmArtNGBobis.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled prometa artikala MP | MPPrmArtNGBobis.xml |
| `MPPromDatum.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Promjena datuma MP | MPPromDatum.xml |
| `MPProvizije.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Provizije djelatnika | MPProvizije.xml |
| `MPProvizijePregled.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled stimulacija djelatnika | MPProvizijePregled.xml |
| `MPPrPromOp.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled prom.po oper. (NOVO) | MPPrPromOp.xml |
| `MPRabUtNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled popusta u utršku | MPRabUtNG.xml |
| `MPRACAF.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Preg. nak. fiskaliziranih | MPRACAF.xml |
| `MPRegije.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Regije trgovina | MPRegije.xml |
| `MPRekapDobav.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Rekapitulacija po dobavljaču | MPRekapDobav.xml |
| `MPRekapPromArt.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Rekap. prometa po artiklu | MPRekapPromArt.xml |
| `MPRobaSmrznuto.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Roba - dodatni atributi | MPRobaSmrznuto.xml |
| `MPSkladisteVeza.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Veza skladišta | MPSkladisteVeza.xml |
| `MPStanjeBezNC.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje MP | MPStanjeBezNC.xml |
| `MPStanjeKumulativno.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje trgovina - KUMULATIV | MPStanjeKumulativno.xml |
| `MPStanjeKumulativnoVL.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje vlastite robe | MPStanjeKumulativnoVL.xml |
| `MPStanjeMVMJ.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje trgovina po NV | MPStanjeMVMJ.xml |
| `MPStanjeSirivinaANALIT.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje vl. sirovina - Analit. | MPStanjeSirivinaANALIT.xml |
| `MPStanjeUgo.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stanje robe ugostiteljstvo | MPStanjeUgo.xml |
| `MPTovarnilist.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | MP Tovarni list | MPTovarnilist.xml |
| `MPTrosarine.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | MP Trošarine | MPTrosarine.xml |
| `MPUplKredCKNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Uplata CK Kredita | MPUplKredCKNG.xml |
| `MPUplKredNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Uplata Konikom kredita - NOVO | MPUplKredNG.xml |
| `MPUplKredTJNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Uplata kredita                ; Uplata TJ kredita | MPUplKredTJNG.xml |
| `MPVaucherPregled.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled vaucher-a | MPVaucherPregled.xml |
| `MPZadnjiBroj.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Zadnji brojevi dok. za kasu | MPZadnjiBroj.xml |
| `MsklDistrib.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Skladišta za naravnavanje | SKT\MsklDistrib.xml |
| `nabispanaliza.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Analiza NAR-ISP | nabispanaliza.xml |
| `NacinObracunaZadatka.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Način obračuna zadatka; Način obračuna zadatka | NacinObracunaZadatka.xml |
| `NarQryNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Generiranje naloga | NarQryNG.xml |
| `NOtpIsporuceno.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Povrat | skt/NOtpIsporuceno.xml |
| `NotpStatus.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Praćenje statusa otpreme | NotpStatus.xml |
| `OOPKnjigaR.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Knjiga radnika | OOPKnjigaR.xml |
| `OperaterPogon.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Izbor pogona | mfg/OperaterPogon.xml |
| `ParStruPrava.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Operater - lokacija prava | ParStruPrava.xml |
| `PKDuznNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled dužnika po kreditima | PKDuznNG.xml |
| `PKKrZatNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Zatvaranje krediita | PKKrZatNG.xml |
| `PKTuzbe.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Evidencija tužbi | PKTuzbe.xml |
| `PKUvjetiNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Uvjeti kredita | PKUvjetiNG.xml |
| `PKZaduzNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Stanje osoba po kreditima | PKZaduzNG.xml |
| `PovijestCjenika.xml` | 2 |  | Enna (jupiter2019); spin (sql-ai) | Pregled izmjene cjenika; Pregled izmjene cjenika | PovijestCjenika.xml |
| `povispanaliza.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Analiza PRE-ISP | povispanaliza.xml |
| `PregledCjenikaNG.xml` | 2 |  | Setcor (jupiter2019); Ireks (jupiter2019) | Pregled cjenika; Pregled cjenika - ispis | PregledCjenikaNG.xml |
| `PregledKnj.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Pregled prometa NG; Pregled prometa NG | PregledKnj.xml |
| `PrijemniListNG.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Prijemni list                           ; Prijemni list | PrijemniListNG.xml |
| `prioritet.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Prioriteti zadataka; Prioriteti zadataka | prioritet.xml |
| `prnPredajaSKTM.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Predaja iz proizvodnje SKT | MFG\prnPredajaSKTM.xml |
| `prnProizvodMulti.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Ažuriranje receptura | MFG/prnProizvodMulti.xml |
| `prnSastavniceQuery.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled sastavnica | prnSastavniceQuery.xml |
| `prnUtrosakAzurMlinar.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Ažuriranje utroška | MFG\prnUtrosakAzurMlinar.xml |
| `proces.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Proces; Proces | proces.xml |
| `ProENarudzbe.xml` | 2 |  | Enna (jupiter2019); Ireks (jupiter2019) | E-Narudžbe | ProENarudzbe.xml |
| `PrometAmbalazeNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Promet Ambalaže NG | PrometAmbalazeNG.xml |
| `ProNarPredlozakNG.XML` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Predlošci | ProNarPredlozakNG.XML |
| `ProNarudzbeNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pekarske narudžbe | ProNarudzbeNG.xml |
| `PrOrgSh.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Pregled po MT-Kartica | PrOrgSh.xml |
| `ProTovarka2.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Lista za komisioniranje | ProTovarka2.xml |
| `PTOsobeCjenik.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Osobe Cjenik; Osobe Cjenik | PTOsobeCjenik.xml |
| `ptutrvrstarada.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Vrsta rada; Vrsta rada | ptutrvrstarada.xml |
| `PTVrstaDok.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Vrste dokumenata; Vrste dokumenata | PTVrstaDok.xml |
| `RbaUDNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Isporučeno-nezaprimljeno MP | RbaUDNG.xml |
| `rutelnNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Dostavna mjesta - Linije | rutelnNG.xml |
| `sktKomoraJob.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Generiranje međuskladišnica | SKT/sktKomoraJob.xml |
| `SktMrvice.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Mrvice | SKT\SktMrvice.xml |
| `SktPrometSMSK.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Pregled stanja na dan | SktPrometSMSK.xml |
| `sktStariKruh_Query.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Stari kruh pregled | skt/sktStariKruh_Query.xml |
| `StatusProjekta.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Status projekta; Status projekta | StatusProjekta.xml |
| `statusZadatka.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Status zadatka; Status zadatka | statusZadatka.xml |
| `STFunkcijeOsobaNG.xml` | 2 |  | Panissimo (jupiter2022); Tusak (jupiter2022) | Funkcije osoba | STFunkcijeOsobaNG.xml |
| `TecajnaListaNG.xml` | 2 |  | Setcor (jupiter2022); spin (sql-ai) | Tečajna lista; Tečajna lista | TecajnaListaNG.xml |
| `TovarniListNG.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Tovarni list | TovarniListNG.xml |
| `UDjeParNG.xml` | 2 |  | Setcor (jupiter2019); Setcor (jupiter2022) | Urednik djelatnosti partnera            ; Urednik djelatnosti partnera | UDjeParNG.xml |
| `VrstaProjekt.xml` | 2 |  | Ireks (jupiter2019); spin (sql-ai) | Vrsta projekta; Vrste projekta | VrstaProjekt.xml |
| `WSOsobe.xml` | 2 |  | CentarTehnike (jupiter2019); CentarTehnike (jupiter2022) | Korisnički računi za Web Shop | WSOsobe.xml |
| `izmjenapostotka.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Klase robe - % NC | izmjenapostotka.xml |
| `KKvalUvjetiPartnera.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Rok trajanja - Partneri | KKvalUvjetiPartnera.xml |
| `MPClanPoklon.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Poklon bodovi | MPClanPoklon.xml |
| `MPEFaktureOtpremnice_test.xml` | 1 | **Biovega** | Biovega (jupiter2019) | _efak eotp test | MPEFaktureOtpremnice_test.xml |
| `MPKaseKeksPay.xml` | 1 | **Biovega** | Biovega (jupiter2019) | KeksPay | MPKaseKeksPay.xml |
| `MpKeksPayKontrola.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Kontrola KeksPay-a | MpKeksPayKontrola.xml |
| `MPKontrolaMPRobno.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Kontrola MP Robno | MPKontrolaMPRobno.xml |
| `MPKontrolaWMS.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Kontrola WMS - MP | MPKontrolaWMS.xml |
| `MPLoyParametri.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Loyalty parametri | MPLoyParametri.xml |
| `MPNarFDob.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Narudž. franšizera dobavljaču (old) | MPNarFDob.xml |
| `MPNarFDobA.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Nar. franšizera dobavljaču | MPNarFDobA.xml |
| `MPNarFFirma1.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Nar. franšizera prema vlas | MPNarFFirma1.xml |
| `MPNarIzdSklTab.xml` | 1 | **Biovega** | Biovega (jupiter2019) | _Nar. za skladište ( Tabela ) | MPNarIzdSklTab.xml |
| `MPObracunKalaNGBiovegaWEB.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Pregled dozvoljenog kala webshop | MPObracunKalaNGBiovegaWEB.xml |
| `MPPakiranjeDodKlasa.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Parametari naručivanja | MPPakiranjeDodKlasa.xml |
| `MPPregProizNorm2.xml` | 1 | **Biovega** | Biovega (jupiter2019) | _Ažur. proizvoda i normativa | MPPregProizNorm2.xml |
| `MPPregPromPDV.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Inv. stanje promjena PDV | MPPregPromPDV.xml |
| `MPPregRobeZaNarSkl.xml` | 1 | **Biovega** | Biovega (jupiter2019) | _Pregled robe za narudžbu | MPPregRobeZaNarSkl.xml |
| `MPSezonskaRoba.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Sezonska roba | MPSezonskaRoba.xml |
| `MPTrgovineDanaProm.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Dani prometa trgovina | MPTrgovineDanaProm.xml |
| `MPVaganaRoba.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Vagana roba (staro) | MPVaganaRoba.xml |
| `OOPGodOlaksice.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Godišnje porezne stope; Godišnje porezne olakšice | OOPGodOlaksice.xml |
| `PovKompost.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Povezivanje komposta | skt\PovKompost.xml |
| `SessionAudit.xml` | 1 | **Biovega** | Biovega (jupiter2019) | Praćenje logiranja korisnika | SessionAudit.xml |
| `admini.xml` | 1 |  | spin (sql-ai) | Jupiter administratori | admini.xml |
| `Ambalaza.xml` | 1 |  | Ireks (jupiter2019) | Evidencija težine ambalaže | Ambalaza.xml |
| `bTobIreksNabNarHrSrb.xml` | 1 |  | Ireks (jupiter2019) | Uvoz narudžbi (Ireks doo) | bTobIreksNabNarHrSrb.xml |
| `budzetiranje.xml` | 1 |  | Enna (jupiter2019) | Plan Proizvodnje Mjesečni | budzetiranje.xml |
| `budzo.xml` | 1 |  | Enna (jupiter2019) | Parametri Plana | budzo.xml |
| `Connexobustave.xml` | 1 |  | spin (sql-ai) | Obustave | Connexobustave.xml |
| `CoreKompLog.xml` | 1 |  | spin (sql-ai) | Core Components Trace | CoreKompLog.xml |
| `DD_arhiva.xml` | 1 |  | Setcor (jupiter2022) | Arhiva obrada | DD_arhiva.xml |
| `EviIzlaza.xml` | 1 |  | Enna (jupiter2019) | Evidencija izlaza | EviIzlaza.xml |
| `EviUlaza.xml` | 1 |  | Enna (jupiter2019) | Zabrana ulaza | EviUlaza.xml |
| `HRDokumenti.xml` | 1 |  | Setcor (jupiter2019) | Dokumenti | HRDokumenti.xml |
| `HRDopust.xml` | 1 |  | Setcor (jupiter2019) | Manager plaćenih dopusta | HRDopust.xml |
| `HRDrzavljanstvo.xml` | 1 |  | Setcor (jupiter2019) | Državljanstvo | HRDrzavljanstvo.xml |
| `HRKontakti.xml` | 1 |  | Setcor (jupiter2019) | Kontakti | HRKontakti.xml |
| `HrmIzvjestaji.xml` | 1 |  | Setcor (jupiter2019) | Izvještaji | HrmIzvjestaji.xml |
| `HRNacionalnost.xml` | 1 |  | Setcor (jupiter2019) | Nacionalnost | HRNacionalnost.xml |
| `HRRazlogDolaska.xml` | 1 |  | Setcor (jupiter2019) | Razlog zasnivanja RO | HRRazlogDolaska.xml |
| `HRRazlogOdlaska.xml` | 1 |  | Setcor (jupiter2019) | Razlog prestanka RO | HRRazlogOdlaska.xml |
| `HRSpreme.xml` | 1 |  | Setcor (jupiter2019) | Spreme | HRSpreme.xml |
| `HRSrodstvo.xml` | 1 |  | Setcor (jupiter2019) | Srodstvo | HRSrodstvo.xml |
| `HRStavkeGO.xml` | 1 |  | Setcor (jupiter2019) | Stavke godišnjeg odmora | HRStavkeGO.xml |
| `HRVjestine.xml` | 1 |  | Setcor (jupiter2019) | Vještine | HRVjestine.xml |
| `HRVrsteGO.xml` | 1 |  | Setcor (jupiter2019) | Vrste godišnjeg odmora | HRVrsteGO.xml |
| `HRVrsteRO.xml` | 1 |  | Setcor (jupiter2019) | Vrste radnog odnosa | HRVrsteRO.xml |
| `HRZvanja.xml` | 1 |  | Setcor (jupiter2019) | Zvanja | HRZvanja.xml |
| `IreksAnalitikaTrosarina.xml` | 1 |  | Ireks (jupiter2019) | Trošarine - pregled analitika | IreksAnalitikaTrosarina.xml |
| `IreksPregledTrosarina.xml` | 1 |  | Ireks (jupiter2019) | Trošarine - pregled kumulativ | MER\IreksPregledTrosarina.xml |
| `IreksPrometTrosArtikala.xml` | 1 |  | Ireks (jupiter2019) | Promet trošarinskih artikala | IreksPrometTrosArtikala.xml |
| `IreksPrometTrosArtikalaNe.xml` | 1 |  | Ireks (jupiter2019) | Promet troš. artikala NOVO | IreksPrometTrosArtikalaNe.xml |
| `IreksTrosarina.xml` | 1 |  | Ireks (jupiter2019) | Pregled trošarina | IreksTrosarina.xml |
| `IreksTrosarinaAna.xml` | 1 |  | Ireks (jupiter2019) | Pregled trošarina analitika | IreksTrosarinaAna.xml |
| `IreksTrosarinaAnaNew.xml` | 1 |  | Ireks (jupiter2019) | Pregled troš. analitika NOVO | IreksTrosarinaAnaNew.xml |
| `IreksTrosarinaNew.xml` | 1 |  | Ireks (jupiter2019) | Pregled trošarina NOVO | IreksTrosarinaNew.xml |
| `IreksZaliheTrosArtikala.xml` | 1 |  | Ireks (jupiter2019) | Zalihe trošarinskih artikala | IreksZaliheTrosArtikala.xml |
| `isokategorija.xml` | 1 |  | spin (sql-ai) | ISO kategorije zadataka | isokategorija.xml |
| `IspisEtiketa.xml` | 1 |  | Ireks (jupiter2019) | Ispis GS1 etiketa | IspisEtiketa.xml |
| `IspisiFirme.xml` | 1 |  | spin (sql-ai) | Ispisi kod korisnika | IspisiFirme.xml |
| `klasetecajevi.xml` | 1 |  | spin (sql-ai) | Klase tečajeva                          ; Klase JA tečajeva | klasetecajevi.xml |
| `KntIOSObrazacDev.xml` | 1 |  | spin (sql-ai) | IOS Devizno | KntIOSObrazacDev.xml |
| `KntIspravakVrijednosti.xml` | 1 |  | Ireks (jupiter2019) | Ispravak vrijednosti | KntIspravakVrijednosti.xml |
| `KntOcekivaniPriljevOdljev.xml` | 1 |  | Enna (jupiter2019) | Dospijeće po periodima | KntOcekivaniPriljevOdljev.xml |
| `kntRekapOtvDob.xml` | 1 |  | Ireks (jupiter2019) | Obaveze prema dobavljačima | kntRekapOtvDob.xml |
| `KntVirmaniKoop.xml` | 1 |  | Enna (jupiter2019) | Generator Virmana | KntVirmaniKoop.xml |
| `KOD_Arhiva.xml` | 1 |  | Setcor (jupiter2019) | Arhiva obrada | KOD_Arhiva.xml |
| `KOD_Place.xml` | 1 |  | Setcor (jupiter2019) | Plaće | KOD_Place.xml |
| `KUPDostKNAgroFructus.xml` | 1 |  | Enna (jupiter2019) | Dostavna knjiga | KUPDostKNAgroFructus.xml |
| `KUPDostKNECS_URA.xml` | 1 |  | Setcor (jupiter2022) | Dostavna knjiga URA; KUP Dostavna knjiga | KUPDostKNECS_URA.xml |
| `KUPDostKNIreks.xml` | 1 |  | Ireks (jupiter2019) | Dostavna knjiga | KUPDostKNIreks.xml |
| `KUPDostKnjigaTEST.xml` | 1 |  | spin (sql-ai) | KUP Dostavna knjiga TEST | KUPDostKnjigaTEST.xml |
| `KUPDostKNOdobravanje.xml` | 1 |  | spin (sql-ai) | KUP Dostavna knjiga | KUPDostKNOdobravanje.xml |
| `KUPNGAgroFructus.xml` | 1 |  | Enna (jupiter2019) | Knjiga ulazne pošte | KUPNGAgroFructus.xml |
| `KUPNGAgroFructusDMSScan.xml` | 1 |  | Enna (jupiter2019) | Knjiga pošte SCAN | KUPNGAgroFructusDMSScan.xml |
| `KUPNGECS.xml` | 1 |  | Setcor (jupiter2019) | Knjiga pošte | kup\KUPNGECS.xml |
| `KUPNGECS_BackOffice_URA.xml` | 1 |  | Setcor (jupiter2022) | Knjiga pošte BackOffice URA | KUPNGECS_BackOffice_URA.xml |
| `KUPNGECS_URA.xml` | 1 |  | Setcor (jupiter2022) | Knjiga pošte URA | KUPNGECS_URA.xml |
| `KUPNGIreks.xml` | 1 |  | Ireks (jupiter2019) | Knjiga pošte | KUPNGIreks.xml |
| `KUPNGSpin.xml` | 1 |  | spin (sql-ai) | Knjiga pošte | KUPNGSpin.xml |
| `KUPNGSpin_KUP_URA.xml` | 1 |  | spin (sql-ai) | Knjiga pošte KUP_URA | KUPNGSpin_KUP_URA.xml |
| `KUPNGTest.xml` | 1 |  | spin (sql-ai) | Knjiga pošte TEST | KUPNGTest.xml |
| `KUPNG_ISOOdobravanje.xml` | 1 |  | spin (sql-ai) | ISO odobravanje | KUPNG_ISOOdobravanje.xml |
| `KUPStatNLinijeOdo.xml` | 1 |  | Tusak (jupiter2022) | Status pošte | KUPStatNLinijeOdo.xml |
| `KUPStatNRedOdo.xml` | 1 |  | Enna (jupiter2019) | Status ulazne pošte | KUPStatNRedOdo.xml |
| `kupura.xml` | 1 |  | Setcor (jupiter2022) | Povezivanje KUP_URA | kupura.xml |
| `KUPVrsPTip.xml` | 1 |  | Tusak (jupiter2022) | Vrsta pošte | KUPVrsPTip.xml |
| `MENIPubVars.xml` | 1 |  | spin (sql-ai) | PubVarsi kod korisnika | MENIPubVars.xml |
| `MPAppManager.xml` | 1 |  | spin (sql-ai) | Upravljanje mob. app za MP | MPAppManager.xml |
| `MPBlagajnaFIS.xml` | 1 |  | Enna (jupiter2019) | Blagajna FIS | MPBlagajnaFIS.xml |
| `MPBlagajnaPOS.xml` | 1 |  | Panissimo (jupiter2022) | MP Blagajna - online | MPBlagajnaPOS.xml |
| `MPBlagajnaPOSNaturala.xml` | 1 |  | Enna (jupiter2019) | MP blagajna POS | MPBlagajnaPOSNaturala.xml |
| `MPBlagajnaSVEIreks.xml` | 1 |  | Ireks (jupiter2019) | Blagajna MP iz VP-NOVO | MPBlagajnaSVEIreks.xml |
| `MPBrojIskaznicaZastitaNEW.xml` | 1 |  | Enna (jupiter2019) | Iskaznica zastita | MPBrojIskaznicaZastitaNEW.xml |
| `MPDirZaduzenjeNG1Mlinar.xml` | 1 |  | Enna (jupiter2019) | MP primka | MPDirZaduzenjeNG1Mlinar.xml |
| `MPEtiketeCjenikNG.xml` | 1 |  | Enna (jupiter2019) | Ispis etiketa MP iz cjenika | MPEtiketeCjenikNG.xml |
| `MPNarA.xml` | 1 |  | Enna (jupiter2019) | Narudžbenica dobavljaču iz MP | MPNarA.xml |
| `MPPonudaIreks.xml` | 1 |  | Ireks (jupiter2019) | MP Ponuda | MPPonudaIreks.xml |
| `MPPovratDobNGMlinar.xml` | 1 |  | Tusak (jupiter2022) | Povrat dobavljaču iz MP | MPPovratDobNGMlinar.xml |
| `MPPredujam.xml` | 1 |  | CentarTehnike (jupiter2022) | Predujmovi u MP | MPPredujam.xml |
| `MPPregRbTrD.xml` | 1 |  | Enna (jupiter2019) | Pregled robe po trgovinama | MPPregRbTrD.xml |
| `MPPrMPRNGIreks.xml` | 1 |  | Ireks (jupiter2019) | Pregled MP računa | MPPrMPRNGIreks.xml |
| `MPRobaUslugaRNNG.xml` | 1 |  | Ireks (jupiter2019) | Pregled robe+usluga po MP rač. | MPRobaUslugaRNNG.xml |
| `MPRobaZabrana.xml` | 1 |  | Enna (jupiter2019) | Zabrana robe na blagajni | MPRobaZabrana.xml |
| `MPRobnoDOk.xml` | 1 |  | spin (sql-ai) | Status MP dokumenata | MPRobnoDOk.xml |
| `MPUtrzakMPVP.xml` | 1 |  | Enna (jupiter2019) | Utržak MP iz VP | MPUtrzakMPVP.xml |
| `NabPriLAA.xml` | 1 |  | Ireks (jupiter2019) | LAA po primkama | MER\NabPriLAA.xml |
| `Narudzbe_new.xml` | 1 |  | Enna (jupiter2019) | Narudžbe | Narudzbe_new.xml |
| `NOTPNGBROJ.xml` | 1 |  | Enna (jupiter2019) | Manager otpreme - ponude | NOTPNGBROJ.xml |
| `OpasneKemikalijeNG.xml` | 1 |  | Ireks (jupiter2019) | Opasne Kemikalije | OpasneKemikalijeNG.xml |
| `OpasneNabavaNG.xml` | 1 |  | Ireks (jupiter2019) | Nabava opasnih tvari | OpasneNabavaNG.xml |
| `OpasneProizvodnjaNG.xml` | 1 |  | Ireks (jupiter2019) | Proizvodnja opasnih tvari | OpasneProizvodnjaNG.xml |
| `OpasnePrometNG.xml` | 1 |  | Ireks (jupiter2019) | Promet opasnih tvari | OpasnePrometNG.xml |
| `OpasneStanjeNG.xml` | 1 |  | Ireks (jupiter2019) | Stanje opasnih tvari | OpasneStanjeNG.xml |
| `OpasneTvariPregledNG.xml` | 1 |  | Ireks (jupiter2019) | Opasne Tvari - Pregled | OpasneTvariPregledNG.xml |
| `OpasneTvariUrednikNG.xml` | 1 |  | Ireks (jupiter2019) | Opasne Tvari - Urednik | OpasneTvariUrednikNG.xml |
| `OtkupniListAF.xml` | 1 |  | Enna (jupiter2019) | Otkupni list | SKLAD\OtkupniListAF.xml |
| `otprema.xml` | 1 |  | Ireks (jupiter2019) | Manager utovara | mer/otprema.xml |
| `ParJm.xml` | 1 |  | Ireks (jupiter2019) | Partneri JM | ParJm.xml |
| `PNKatalozi_ENNA.xml` | 1 |  | Enna (jupiter2019) | PN Katalozi | PNKatalozi_ENNA.xml |
| `PNNaloziMoji.xml` | 1 |  | spin (sql-ai) | Putni nalozi-moji | PNNaloziMoji.xml |
| `PredatnicaLAA.xml` | 1 |  | Ireks (jupiter2019) | LAA po predatnicama | SKLAD\PredatnicaLAA.xml |
| `PregledOperateraNaDok.xml` | 1 |  | spin (sql-ai) | Pregled operatera na dok | MER\PregledOperateraNaDok.xml |
| `PrnEV.xml` | 1 |  | Ireks (jupiter2019) | Energ. vrijed. sir. | PrnEV.xml |
| `prnKeub.xml` | 1 |  | Tusak (jupiter2022) | KEUB Knjige | MFG\prnKeub.xml |
| `prnkeubr.xml` | 1 |  | Tusak (jupiter2022) | Robe u KEUB-u | prnkeubr.xml |
| `prnMSKLizPredatnice.xml` | 1 |  | Enna (jupiter2019) | Kreiranje MSKL ambalaže | MFG\prnMSKLizPredatnice.xml |
| `ProPonKolPotvrdi.xml` | 1 |  | Enna (jupiter2019) | Potvrđivanje narudžbi | ProPonKolPotvrdi.xml |
| `ProPotvrdiNarudzbu.xml` | 1 |  | Enna (jupiter2019) | Potvrđivanje e-narudžbi | ProPotvrdiNarudzbu.xml |
| `Protokoli.xml` | 1 |  | spin (sql-ai) | Protokoli | Protokoli.xml |
| `ProtokolKatalog.xml` | 1 |  | spin (sql-ai) | Katalozi protokola | ProtokolKatalog.xml |
| `Racuni_za_carinuIreks.xml` | 1 |  | Ireks (jupiter2019) | PDV po uvozu | Racuni_za_carinuIreks.xml |
| `RASCjenik.xml` | 1 |  | Enna (jupiter2019) | Cjenik | RASCjenik.xml |
| `RASGrupe.xml` | 1 |  | Enna (jupiter2019) | Grupe radnika | RASGrupe.xml |
| `RASRaspored.xml` | 1 |  | Enna (jupiter2019) | Raspored | RASRaspored.xml |
| `RazlikeUvjetaDokUgovor.xml` | 1 |  | spin (sql-ai) | Razlike uvjeta ugovor/dok | RazlikeUvjetaDokUgovor.xml |
| `RBProdaja.xml` | 1 |  | Setcor (jupiter2019) | Prodaja ribe | RBProdaja.xml |
| `RealizacijaOtpNG.xml` | 1 |  | Enna (jupiter2019) | Realizacija otp - stavke | MER\RealizacijaOtpNG.xml |
| `RealizacijaPonudaStanje.xml` | 1 |  | Enna (jupiter2019) | Realizacija ponuda sa stanjem | MER\RealizacijaPonudaStanje.xml |
| `RobaDodatniAtr.xml` | 1 |  | Ireks (jupiter2019) | Dodatni atributi robe | UTP\RobaDodatniAtr.xml |
| `RobaNaknade.xml` | 1 |  | Ireks (jupiter2019) | Evidencija naknada i trošarina | RobaNaknade.xml |
| `RobaZemljaPorijekla.xml` | 1 |  | Ireks (jupiter2019) | Zemlja preferen. porijekla | UTP\RobaZemljaPorijekla.xml |
| `SiraOtp.xml` | 1 |  | Ireks (jupiter2019) | WMS Provedi | SiraOtp.xml |
| `SKLTrosarinskiIspisIreks.xml` | 1 |  | Ireks (jupiter2019) | Prateći trošarinski dok. | SKLTrosarinskiIspisIreks.xml |
| `sktRegalUlaz.xml` | 1 |  | Enna (jupiter2019) | Ulaz paleta | sktRegalUlaz.xml |
| `SktUtovarEurovoceNG.xml` | 1 |  | Enna (jupiter2019) | Manager Utovara | SktUtovarEurovoceNG.xml |
| `StanjeAmbalazeNG.xml` | 1 |  | Enna (jupiter2019) | Stanje Ambalaže                         ; Stanje Ambalaže | StanjeAmbalazeNG.xml |
| `statusadmin.xml` | 1 |  | spin (sql-ai) | Status Jupiter administratora | statusadmin.xml |
| `TDObracun.xml` | 1 |  | spin (sql-ai) | Obračun terenskog dodatka | TDObracun.xml |
| `Tecajevi.xml` | 1 |  | spin (sql-ai) | Tečajevi | Tecajevi.xml |
| `voziloex.xml` | 1 |  | Ireks (jupiter2019) | Kapacitet vozila | mer/voziloex.xml |
| `wmsstanje.xml` | 1 |  | Ireks (jupiter2019) | Zahtjev za WMS stanjem | wmsstanje.xml |
| `WSLogin.xml` | 1 |  | Ireks (jupiter2019) | Web Shop Login | WSLogin.xml |
| `wwwkomp.xml` | 1 |  | spin (sql-ai) | WWWkompenzacije | wwwkomp.xml |
| `zapornjak.xml` | 1 |  | Enna (jupiter2019) | Zamjena lozinke | zapornjak.xml |

