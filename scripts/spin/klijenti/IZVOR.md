# SQL objekti s klijenata (usporedba #11, 12.11.2026.)

## Sažetak

| Usporedba SQL objekata #11 |  |  |  |  |
| Kreirano | 12.11.2026. 17:08 |  |  |  |
| Kreirao | SPIN\ntomic |  |  |  |
| Opis | provera nekih stora |  |  |  |
| Status | U tijeku |  |  |  |
| Baze | 14 ukupno, 11 klijenata, 2 čeka agenta |  |  |  |
| klmasterroba (tablica) |  |  |  |  |
| 11 različite verzije (A: 2, B: 1, C: 1, D: 1, E: 1, F: 1, G: 1, H: 1, I: 1, J: 1, K: 1); čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 2 | 1 | Osatina |  |
| B | 1 | 1 | Žito | Dodatna kolona [RabatDefault] decimal(8,4)<br>Kolona [Naziv]: tip varchar(300) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(50) (referenca: varchar(6))<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| C | 1 | 1 | Žito | Dodatna kolona [PlanTrosak] money<br>Dodatna kolona [PlanTrosakP] float<br>Dodatna kolona [Expand] char(1)<br>Dodatna kolona [KntTroPriRob] varchar(10)<br>Dodatna kolona [oznaka2] varchar(30)<br>Kolona [Naziv]: tip varchar(50) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(50) (referenca: varchar(6))<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| D | 1 | 1 | Vodoprivreda | Nedostaje kolona [KlasaKasa] varchar(10)<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| E | 1 | 1 | Tisup | Dodatna kolona [smsind] int<br>Nedostaje kolona [MarzaMP] float<br>Nedostaje kolona [KlasaKasa] varchar(10)<br>Nedostaje kolona [OsobePLMId] int<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Dodatni indeks [idx_SortKey]<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka]<br>Nedostaje FK [FK_KlMasterRoba_KntKontaZaShemu] |
| F | 1 | 1 | Sokol Vinkovci | Dodatna kolona [RabatDefault] decimal(8,4)<br>Kolona [Naziv]: tip varchar(100) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(20) (referenca: varchar(6))<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| G | 1 | 1 | REEM Electronic | Kolona [Naziv]: tip varchar(50) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(50) (referenca: varchar(6))<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| H | 1 | 1 | Panissimo | Dodatna kolona [KRUNAklmasterrobaid] int<br>Dodatna kolona [grupa] varchar(10)<br>Dodatna kolona [podgrupa] varchar(10)<br>Dodatna kolona [KntOrgshemaId] int<br>Dodatna kolona [oznaka2] varchar(30)<br>Kolona [Naziv]: tip varchar(300) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(20) (referenca: varchar(6))<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| I | 1 | 1 | Miholjački vodovod | Kolona [Naziv]: tip varchar(300) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(20) (referenca: varchar(6))<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| J | 1 | 1 | Mauthner | Dodatna kolona [RabatDefault] decimal(8,4)<br>Kolona [Naziv]: tip varchar(50) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(50) (referenca: varchar(6))<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| K | 1 | 1 | Koestlin | Dodatna kolona [RDA] int<br>Dodatna kolona [RTI] int<br>Dodatna kolona [oznaka2] varchar(30)<br>Kolona [Naziv]: tip varchar(50) (referenca: varchar(30))<br>Kolona [Oznaka]: tip varchar(50) (referenca: varchar(6))<br>Kolona [KntKontaZaShemuId]: NOT NULL (referenca: NULL)<br>Nedostaje indeks [IX_KlMasterRoba_Oznaka] |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |
| spBLZakljucakQueryNG (procedura) |  |  |  |  |
| 3 različite verzije (A: 4, B: 2, C: 2); ne postoji na 4; čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 4 | 4 | Mauthner, Panissimo, Sokol Vinkovci, Žito |  |
| B | 2 | 2 | Miholjački vodovod, Vodoprivreda |  |
| C | 2 | 1 | Osatina |  |
| ne postoji | 4 |  | Koestlin/Trening, REEM Electronic/Trening, Tisup/Tisup, Žito/zito |  |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |
| spFakPartneriUvjetiQuery (procedura) |  |  |  |  |
| 2 različite verzije (A: 3, B: 1); ne postoji na 8; čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 3 | 3 | Mauthner, Sokol Vinkovci, Žito |  |
| B | 1 | 1 | Panissimo |  |
| ne postoji | 8 |  | Koestlin/Trening, Miholjački vodovod/Trening, Osatina/PZOsatina, Osatina/Trening, REEM Electronic/Trening, Tisup/Tisup, Vodoprivreda/Trening, Žito/zito |  |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |
| spKultureNGQuery (procedura) |  |  |  |  |
| 3 različite verzije (A: 5, B: 2, C: 1); ne postoji na 4; čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 5 | 5 | Mauthner, Miholjački vodovod, Panissimo, Sokol Vinkovci, Žito |  |
| B | 2 | 1 | Osatina |  |
| C | 1 | 1 | Žito |  |
| ne postoji | 4 |  | Koestlin/Trening, REEM Electronic/Trening, Tisup/Tisup, Vodoprivreda/Trening |  |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |
| spKUPPrometQueryN (procedura) |  |  |  |  |
| 9 različite verzije (A: 2, B: 1, C: 1, D: 1, E: 1, F: 1, G: 1, H: 1, I: 1); ne postoji na 2; čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 2 | 1 | Žito |  |
| B | 1 | 1 | Vodoprivreda |  |
| C | 1 | 1 | Sokol Vinkovci |  |
| D | 1 | 1 | Panissimo |  |
| E | 1 | 1 | Osatina |  |
| F | 1 | 1 | Osatina |  |
| G | 1 | 1 | Miholjački vodovod |  |
| H | 1 | 1 | Mauthner |  |
| I | 1 | 1 | Koestlin |  |
| ne postoji | 2 |  | REEM Electronic/Trening, Tisup/Tisup |  |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |
| spStatusiQuery (procedura) |  |  |  |  |
| 3 različite verzije (A: 6, B: 1, C: 1); ne postoji na 4; čeka 2 |  |  |  |  |
| Verzija | Baza | Klijenata | Klijenti | Razlike u strukturi prema A |
| A | 6 | 5 | Mauthner, Osatina, Panissimo, Sokol Vinkovci, Žito |  |
| B | 1 | 1 | Žito |  |
| C | 1 | 1 | Miholjački vodovod |  |
| ne postoji | 4 |  | Koestlin/Trening, REEM Electronic/Trening, Tisup/Tisup, Vodoprivreda/Trening |  |
| čeka agenta | 2 |  | Tušak/Trening, Tušak/Tusak |  |

## Po bazi

| Klijent | Baza | Status baze | klmasterroba | spBLZakljucakQueryNG | spFakPartneriUvjetiQuery | spKultureNGQuery | spKUPPrometQueryN | spStatusiQuery |
| Koestlin | Trening | OK | K | ne postoji | ne postoji | ne postoji | I | ne postoji |
| Mauthner | Mauthner | OK | J | A | A | A | H | A |
| Miholjački vodovod | Trening | OK | I | B | ne postoji | A | G | C |
| Osatina | PZOsatina | OK | A | C | ne postoji | B | F | A |
| Osatina | Trening | OK | A | C | ne postoji | B | E | A |
| Panissimo | Panissimo | OK | H | A | B | A | D | A |
| REEM Electronic | Trening | OK | G | ne postoji | ne postoji | ne postoji | ne postoji | ne postoji |
| Sokol Vinkovci | Trening | OK | F | A | A | A | C | A |
| Tisup | Tisup | OK | E | ne postoji | ne postoji | ne postoji | ne postoji | ne postoji |
| Tušak | Trening | čeka agenta | … | … | … | … | … | … |
| Tušak | Tusak | čeka agenta | … | … | … | … | … | … |
| Vodoprivreda | Trening | OK | D | B | ne postoji | ne postoji | B | ne postoji |
| Žito | AgroTovarnik | OK | B | A | A | A | A | A |
| Žito | zito | OK | C | ne postoji | ne postoji | C | A | B |

## Gdje je koja verzija

| Objekt | Verzija | Klijent | Baza |
| klmasterroba | A | Osatina | PZOsatina |
| klmasterroba | A | Osatina | Trening |
| klmasterroba | B | Žito | AgroTovarnik |
| klmasterroba | C | Žito | zito |
| klmasterroba | D | Vodoprivreda | Trening |
| klmasterroba | E | Tisup | Tisup |
| klmasterroba | F | Sokol Vinkovci | Trening |
| klmasterroba | G | REEM Electronic | Trening |
| klmasterroba | H | Panissimo | Panissimo |
| klmasterroba | I | Miholjački vodovod | Trening |
| klmasterroba | J | Mauthner | Mauthner |
| klmasterroba | K | Koestlin | Trening |
| spBLZakljucakQueryNG | A | Mauthner | Mauthner |
| spBLZakljucakQueryNG | A | Panissimo | Panissimo |
| spBLZakljucakQueryNG | A | Sokol Vinkovci | Trening |
| spBLZakljucakQueryNG | A | Žito | AgroTovarnik |
| spBLZakljucakQueryNG | B | Miholjački vodovod | Trening |
| spBLZakljucakQueryNG | B | Vodoprivreda | Trening |
| spBLZakljucakQueryNG | C | Osatina | PZOsatina |
| spBLZakljucakQueryNG | C | Osatina | Trening |
| spFakPartneriUvjetiQuery | A | Mauthner | Mauthner |
| spFakPartneriUvjetiQuery | A | Sokol Vinkovci | Trening |
| spFakPartneriUvjetiQuery | A | Žito | AgroTovarnik |
| spFakPartneriUvjetiQuery | B | Panissimo | Panissimo |
| spKultureNGQuery | A | Mauthner | Mauthner |
| spKultureNGQuery | A | Miholjački vodovod | Trening |
| spKultureNGQuery | A | Panissimo | Panissimo |
| spKultureNGQuery | A | Sokol Vinkovci | Trening |
| spKultureNGQuery | A | Žito | AgroTovarnik |
| spKultureNGQuery | B | Osatina | PZOsatina |
| spKultureNGQuery | B | Osatina | Trening |
| spKultureNGQuery | C | Žito | zito |
| spKUPPrometQueryN | A | Žito | AgroTovarnik |
| spKUPPrometQueryN | A | Žito | zito |
| spKUPPrometQueryN | B | Vodoprivreda | Trening |
| spKUPPrometQueryN | C | Sokol Vinkovci | Trening |
| spKUPPrometQueryN | D | Panissimo | Panissimo |
| spKUPPrometQueryN | E | Osatina | Trening |
| spKUPPrometQueryN | F | Osatina | PZOsatina |
| spKUPPrometQueryN | G | Miholjački vodovod | Trening |
| spKUPPrometQueryN | H | Mauthner | Mauthner |
| spKUPPrometQueryN | I | Koestlin | Trening |
| spStatusiQuery | A | Mauthner | Mauthner |
| spStatusiQuery | A | Osatina | PZOsatina |
| spStatusiQuery | A | Osatina | Trening |
| spStatusiQuery | A | Panissimo | Panissimo |
| spStatusiQuery | A | Sokol Vinkovci | Trening |
| spStatusiQuery | A | Žito | AgroTovarnik |
| spStatusiQuery | B | Žito | zito |
| spStatusiQuery | C | Miholjački vodovod | Trening |
