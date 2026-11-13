# Što korisnik donosi s klijenata

Jupiter2019 i jupiter2022 se ne pitaju. Zato **nijedan SQL objekt nije dokazan kao odsutan na oba ta servera**. Stavke se ovim popisom ne zatvaraju.

SQL ispod fali na spinu (`sql-ai`). Tip i tijelo treba doći s klijenta. Stub koji port sam doda nije ta definicija.

## SQL

| Objekt | Stavka | Granula | Što treba |
|---|---|---|---|
| `dbo.klmasterroba.oznaka2` | `OKL-25`, `OKL-26`, `OKL-27`, `OKL-40`, `OKL-41`, `OKL-42`, `OKL-43`, `OKL-58`, `OKL-59` | OznakaKL | Stupac. Na spinu tablica ima 947 redova i stupac `Oznaka`, a `oznaka2` nema (`Invalid column name`). Master je `select klmasterrobaid, sortkey, Naziv, oznaka, oznaka2 as DodatnaOznaka from klmasterroba order by sortkey`. Treba stvarni tip, duljina i NULL. `varchar(50)` iz stuba nije klijentski tip. |
| `dbo.spKultureNGQuery` | nema zasebnog otvorenog ID-a | Kulture | Procedura. Na spinu je nema. Master zove `@Action=1`. Izmjena zove `@Action=2, @KultureId`. Treba tijelo, ne prazan SELECT. |
| `dbo.spFakPartneriUvjetiQuery` | `FAK-11` | FakPar | Procedura. Na spinu je nema. Master je poziv bez parametara. Izmjena je `@action=2, @FakPartneriUvjetiId`. Treba tijelo. |
| `dbo.spKntPopuniKontroleNG` | `DOS-11` | KntDospjeleStavke | Procedura. Na spinu je nema. Pozivi: `@Action='konto'`, `'klpartneri'`, `'odgosobe'`, `'mjesta', @id`, `'zupanije'`. Treba tijelo. |
| `dbo.spOrgShemaQuery` | `DOS-11` | KntDospjeleStavke | Procedura. Na spinu je nema. Combo zove `@action=0`. Treba tijelo. |
| `dbo.spKntOtvStavkeQueryNG` | `DOS-11` | KntDospjeleStavke | Procedura. Na spinu je nema. Grid zove `@IdOsoba, @DatumOd, @DatumDo, @idpar, @nizkonto, @IdMjesto, @IDzup, @IDklasa, @Iznos, @idorg`. Treba tijelo. |

`spJeziciQuery` nije na ovom popisu. Na spinu vraća 15 redova. `JEZ-31` traži dokaz da tijelo ne piše, ne novu proceduru.

## XML kojeg nema u repou

63 datoteke iz `Prioritet.md` imaju bod 0 i nisu na disku. Broj baza je iz `ssMeni-svi-serveri.md`. Dva ID-a postoje u otvorenim stavkama. Ostale nemaju checklistu.

| Datoteka | Izbornik | Baze | Stavka | Što treba |
|---|---|---:|---|---|
| `TecRazOTV.xml` | `asm:NetGenDll.loader:TecRazOTV.xml` | 10 | TRZ-01 | datoteka izbornika, cijeli XML |
| `Poruke.xml` | `asm:NetGenDLL.loader:Poruke.xml` | 9 | POR-01 | datoteka izbornika, cijeli XML |
| `PorukeDoc.xml` | `asm:NetGenDLL.loader:PorukeDoc.xml` | 7 | nema | datoteka izbornika, cijeli XML |
| `KntBlaVrste.xml` | `asm:NetGenDLL.loader:KntBlaVrste.xml` | 6 | nema | datoteka izbornika, cijeli XML |
| `MPTrgovine.xml` | `asm:NetGenDll.loader:MPTrgovine.xml` | 6 | nema | datoteka izbornika, cijeli XML |
| `KntIzvodTijek.xml` | `asm:NetGenDll.loader:KntIzvodTijek.xml` | 5 | nema | datoteka izbornika, cijeli XML |
| `KntPriGen.xml` | `asm:NetGenDll.loader:KntPriGen.xml` | 5 | nema | datoteka izbornika, cijeli XML |
| `KntZatvaranjeNG.xml` | `asm:NetGenDLL.Loader:KntZatvaranjeNG.xml` | 5 | nema | datoteka izbornika, cijeli XML |
| `ManPokazateljiGrupe.xml` | `asm:NetGenDLL.Loader:ManPokazateljiGrupe.xml` | 5 | nema | datoteka izbornika, cijeli XML |
| `PDVPromjena.xml` | `asm:NetGenDll.loader:PDVPromjena.xml` | 4 | nema | datoteka izbornika, cijeli XML |
| `IzvKlasePartneriParstru.xml` | `asm:NetGenDLL.loader:IzvKlasePartneriParstru.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `jesteliznali.xml` | `asm:NetGenDll.Loader:jesteliznali.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `KntOpomene.xml` | `asm:NetGenDLL.frm:KntOpomene.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `KntRasporedTros.xml` | `asm:NetGenDll.loader:KntRasporedTros.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `KonfigSrvRN.xml` | `asm:NetGendll.loader:KonfigSrvRN.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `ManGadTip.xml` | `asm:NetGenDLL.loader:ManGadTip.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `newswebsite.xml` | `asm:NetgenDLL.loader:newswebsite.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `OrgShemaKonf.xml` | `asm:NetGendll.loader:OrgShemaKonf.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `OsobeOperateri.xml` | `asm:NetgenDLL.loader:OsobeOperateri.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `PubVarsNG.xml` | `asm:NetGenDll.loader:PubVarsNG.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `ServisJamstvo3.xml` | `asm:NetGenDll.loader:ServisJamstvo3.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `Spojka.xml` | `asm:NetGenDLL.Loader:Spojka.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `Upitnici.xml` | `asm:NetGenDll.frm:MAR\Upitnici.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `VoziloFinRekapNG.xml` | `asm:netgendll.loader:VoziloFinRekapNG.xml` | 3 | nema | datoteka izbornika, cijeli XML |
| `kategorija.xml` | `asm:NetgenDLL.Loader:kategorija.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `NacinObracunaZadatka.xml` | `asm:NetgenDll.loader:NacinObracunaZadatka.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `PovijestCjenika.xml` | `asm:NetGenDLL.loader:PovijestCjenika.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `PregledKnj.xml` | `asm:NetGenDll.loader:PregledKnj.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `prioritet.xml` | `asm:NetgenDll.loader:prioritet.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `proces.xml` | `asm:NetgenDll.loader:proces.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `PTOsobeCjenik.xml` | `asm:NetgenDll.loader:PTOsobeCjenik.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `ptutrvrstarada.xml` | `asm:Netgendll.loader:ptutrvrstarada.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `PTVrstaDok.xml` | `asm:NetgenDll.loader:PTVrstaDok.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `SktSerBrojAzur.xml` | `asm:NetGendll.loader:SktSerBrojAzur.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `StatusProjekta.xml` | `asm:NetgenDll.loader:StatusProjekta.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `statusZadatka.xml` | `asm:NetgenDLL.loader:statusZadatka.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `TecajnaListaNG.xml` | `asm:NetGenDll.loader:TecajnaListaNG.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `VrstaProjekt.xml` | `asm:NetgenDll.loader:VrstaProjekt.xml` | 2 | nema | datoteka izbornika, cijeli XML |
| `admini.xml` | `asm:NetgenDll.loader:admini.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `Connexobustave.xml` | `asm:netgendll.loader:Connexobustave.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `CoreKompLog.xml` | `asm:NetGenDLL.loader:CoreKompLog.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `isokategorija.xml` | `asm:NetgenDLL.Loader:isokategorija.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `IspisiFirme.xml` | `asm:NetGenDll.loader:IspisiFirme.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `klasetecajevi.xml` | `asm:NetgenDll.Loader:klasetecajevi.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KntIOSObrazacDev.xml` | `asm:NetGenDLL.frm:KntIOSObrazacDev.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPDostKnjigaTEST.xml` | `asm:NetGenDLL.loader:KUPDostKnjigaTEST.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPDostKNOdobravanje.xml` | `asm:NetGenDLL.loader:KUPDostKNOdobravanje.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPNG_ISOOdobravanje.xml` | `asm:NetGenDLL.loader:KUPNG_ISOOdobravanje.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPNGSpin.xml` | `asm:NetGenDLL.loader:KUPNGSpin.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPNGSpin_KUP_URA.xml` | `asm:NetGenDLL.loader:KUPNGSpin_KUP_URA.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `KUPNGTest.xml` | `asm:NetGenDLL.loader:KUPNGTest.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `MENIPubVars.xml` | `asm:NetGenDll.loader:MENIPubVars.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `MPAppManager.xml` | `asm:netgendll.loader:MPAppManager.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `MPRobnoDOk.xml` | `asm:NetGenDll.loader:MPRobnoDOk.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `PNNaloziMoji.xml` | `asm:NetGenDll.Loader:PNNaloziMoji.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `PregledOperateraNaDok.xml` | `asm:NetGenDLL.Loader:MER\PregledOperateraNaDok.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `Protokoli.xml` | `asm:NetGenDLL.loader:Protokoli.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `ProtokolKatalog.xml` | `asm:NetgenDLL.frm:ProtokolKatalog.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `RazlikeUvjetaDokUgovor.xml` | `asm:NetGenDll.loader:RazlikeUvjetaDokUgovor.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `statusadmin.xml` | `asm:NetgenDll.loader:statusadmin.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `TDObracun.xml` | `asm:NetGenDll.Loader:TDObracun.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `Tecajevi.xml` | `asm:NetGenDll.loader:Tecajevi.xml` | 1 | nema | datoteka izbornika, cijeli XML |
| `wwwkomp.xml` | `asm:NetGenDll.loader:wwwkomp.xml` | 1 | nema | datoteka izbornika, cijeli XML |

## XML na krivoj putanji

Izbornik traži `MER\`, a u repou je primjerak pod `Agronom granule`. To nisu ista datoteka dok korisnik ne potvrdi. Stavke ostaju otvorene.

| Datoteka | Stavka | Što treba |
|---|---|---|
| `MER\RealizacijaFakture.xml` | `RFA-00` | Datoteka s putanje izbornika, ili potvrda da je `definitions/netgen/Agronom granule/RealizacijaFakture.xml` ta ista. |
| `MER\RealizacijaStavkeNG.xml` | `REA-00` | Isto za `definitions/netgen/Agronom granule/RealizacijaStavkeNG.xml`. |
