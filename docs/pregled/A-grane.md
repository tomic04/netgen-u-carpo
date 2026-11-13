# Faza A: grane i integracija

Datum: 2026-11-13. Grana: `integracija`, nastala iz `cursor/netgen-form1-pregled-0b55`.

## Polazno stanje

- Na `main` je samo početni import.
- 80 otvorenih PR-ova od više paralelnih Cursor agenata (`3266`, `0b55`, `889d`, `78a6`, `72f9` = QA).
- Grane se razilaze u **25 vrhova**. Ni jedan vrh ne sadrži sve ostale.

## Pravilo spajanja

1. **Spaja se** sve što je dodatno: novi testovi, podaci, skripte, dokumentacija i kod koji ne postoji u glavnoj liniji.
2. **Ne spaja se kod koji je druga implementacija iste stvari.** Dva puta za isti NetGen ponašanje znače da jedan nije na izvršnom putu, a to CI zaštita (`mrtve-metode.py`) i zabranjuje. Scenariji takvih grana idu u fazu C kao obavezne provjere nad integracijom (tablica C ispod).
3. Kad se obje glavne linije razilaze u ponašanju, vrijedi **novija promjena s dokazom** (QA potvrda, CI izvještaj, NetGen linija).

## Što je u integraciji

| Commit | Što |
|---|---|
| `89adac2` | Spajanje `netgen-treci-otvoreno-889d` (73 commita: noćni sweep, Podloga +867, MultiOdabir, spin skripte). Konflikti: `build.yml`, `Ui/Dijalog.cs` (vidi dolje) |
| `ceb852e` | **Ispravak CI zaštite** `scripts/ci/mrtve-metode.py`: `"https://"` u stringu je progutao ~700 redova `Podloga.cs`, pa je zaštita javljala lažno mrtve metode i nije vidjela dvije deklaracije |
| `87be7e0` | `netgen-klijenti-3266`: klijentski XML-ovi (`xml/klijenti`) i procedure (`scripts/spin/klijenti`), `PrimijeniKlijente.ps1` u CI |
| `a61aa36` | `netgen-naredbe-dokaz-0b55`: `NaredbeDokaz.cs` (NRD-DOPRINT, ISPIS, RODITELJ, BLAGAJNA, PROSTOR, ASSEMBLY, MSGBOX, CLICK) |
| `e1d1824` | Dokumentacija: 118 QA checklista po granuli (`qa-obrasci-otvoreno-72f9`), QA verzije 11 dokumenata u `docs/qa-dnevnik/`, `docs/granule/tipovi-formi.md` |
| `c5a51db` | `netgen-peti-otvoreno-78a6`: FAK-11 (otvaranje FakPar, klijentsko tijelo `spFakPartneriUvjetiQuery`) |

### Konflikt u `Ui/Dijalog.cs`

Obje glavne linije su prepravile otvaranje dijaloga:

- `form1-pregled` je razdvojio `Prikazi` u `Slozi` + prikaz i zadržao skriveni `XtraForm` kao vlasnika.
- `treci-otvoreno` je **kasnije** (`df927a0`, 13. 11. 01:29) maknuo skrivenog vlasnika uz dokaz: dijalog iz klika na podlozi ne vrati kontrolu i JeziciNG ostane u timeoutu. ONLOAD je prebacio u dva koraka (`UcitajOnLoad` prije kontrola, `PrimijeniOnLoad` poslije) i dodao `Trag`.

Rješenje: struktura `Prikazi`/`Slozi` (testovi zovu `Slozi`), ponašanje iz `treci-otvoreno`. `NapuniIzOnLoad` više nema poziva i maknut je. `IzvrsiSqlGreska` zadržava povrat stringa iz `form1-pregled` i `DozvoliUpis` iz `treci-otvoreno`.

### Provjere nakon spajanja (bez Windowsa i DevExpressa)

| Provjera | Rezultat |
|---|---|
| Sintaksa C# (tree-sitter) nad `Carpo.NetGen*`, `Carpo.TestShell`, `tests` | 0 grešaka, 0 dvostrukih metoda |
| Pozivi `Klasa.Metoda(` na metodu koja ne postoji | 0 |
| `scripts/ci/mrtve-metode.py` (ispravljen) | 0 mrtvih; `MultiOdabir.Plavi` u iznimkama uz nalaz MULTI-01 |
| `scripts/ci/portano-ui.py` | prolazi (0 redaka `portano`, katalog prazan) |
| `build.yml` | ispravan YAML |

**Build i testovi moraju proći na runneru `sql-ai`.** Ovo gore ne zamjenjuje kompajliranje.

## Svih 25 vrhova

| Grana | Jedinstvenih commitova | Odluka |
|---|---:|---|
| `netgen-form1-pregled-0b55` | baza | Polazište integracije |
| `netgen-treci-otvoreno-889d` | 73 | **Spojeno** |
| `netgen-klijenti-3266` | 1 | **Spojeno** |
| `netgen-naredbe-dokaz-0b55` | 8 | **Spojeno** |
| `netgen-peti-otvoreno-78a6` | 4 | **Spojeno** |
| `qa-obrasci-otvoreno-72f9` | 100 | **Dokumenti preuzeti**, testovi ne (stariji API, nisu provjereni). C-QA |
| `netgen-tipovi-formi-3266` | 2 | **Dokument preuzet**; `Shell/TipForme.cs` ne, jer isto radi `Shell/OdabirForme.cs` (R104). C-TIP |
| `netgen-cetvrti-dokaz-0b55` | 16 | **Nije spojeno.** Testovi ovise o `PodlogaPravila.cs` (336 redova), paralelnoj implementaciji pravila podloge. C-4 |
| `netgen-onclick-lanac-3266` | 4 | **Nije spojeno.** `DijalogTok.Razradi` je druga implementacija ONCLICK-a uz `Koraci`. C-ONK |
| `netgen-combo-prazan-3266` | 2 | Podskup `onclick-lanac` (combo testovi `3cd920a`). C-ONK |
| `netgen-cvor-stabla-3266` | 1 | Nije spojeno: samo QA kuka (ključevi opisa stabla), ne ponašanje. C-STB |
| `netgen-grid-stupac-3266` | 1 | Nije spojeno: test OznakaKL stupaca u starom `GridTest`. C-OKL |
| `netgen-ispis-zaglavlje-3266` | 2 | Pokriveno u integraciji (`c473084` JEZ-57 `@@tkosmomi`) |
| `netgen-gumb-natpis-3266` | 2 | Commit i njegov revert, bez učinka |
| `netgen-jupiter-smoke-3266` | 1 | Isključuje Jupiter smoke; integracija ga već ima isključenog. B-JUP |
| `netgen-dijalog-gumb-3266` | 1 | Isto (samo isključenje Jupiter smokea) |
| `netgen-dijalog-sql-3266` | 1 | Isto |
| `netgen-dijalog-rupe-3266` | 1 | Isto |
| `netgen-grid-dijalog-3266` | 1 | Isto |
| `netgen-nextform-3266` | 1 | Isto |
| `qa-jezicing-72f9` | 3 | Zastarjelo: prva QA JeziciNG/OznakaKL, kasnije ugrađeno u glavnu liniju |
| `qa-pregled-funkcija-72f9` | 1 | Zastarjelo: integracija ima novije stanje `pregled-funkcija.md` (90 isto / 48 nedostaje) |
| `qa-pregled-sve-72f9` | 1 | Isto stanje kao integracija |
| `qa-form1-uzorak-72f9` | 2 | Zastarjelo |
| `qa-form1-sve-72f9` | 1 | Integracija je novija (`7aa667e`, 05:37 naspram 05:32). Razlika u 4 retka (PokaziUpit, GetID, TreeView1/2_KeyDown) je svjesna |

Ostalih 55 grana su preci ovih vrhova i nemaju ništa svoje.

## Stavke za fazu C iz nespojenih grana

| Oznaka | Izvor | Što provjeriti nad integracijom |
|---|---|---|
| **C-4** | `netgen-cetvrti-dokaz-0b55` | Scenariji NIV-05, REA-04, ONK-04 (**QA potvrdio**), KBP-20, LVO-20, MPK-43, JEZ-57, STA-12, PL-32, MPP-20. Za svaki: radi li integracija isto. Ako ne, prenijeti ponašanje u postojeći put, ne `PodlogaPravila.cs`. Testovi `Cetvrti.cs`, `OnkGumbi.cs`, `KntBlaPromet.cs`, `LVObracun.cs`, `MpPorKp.cs` i spin skripte `mpp20.sql`, `nivrea.sql`, `pl32.sql`, `sta12.ps1` prepisati na API integracije |
| **C-ONK** | `netgen-onclick-lanac-3266` (`4def845`, `3cd920a`, `4cf7c43`) | ONCLICK ONK-01 do ONK-10 i deset combo slučajeva (R90) nad `DijalogTok.Koraci` / `Dijalog` |
| **C-QA** | `qa-obrasci-otvoreno-72f9` | QA brojač 164 / 136 otvoreno / 28 potvrđeno. Uskladiti `docs/qa-dnevnik/*` s glavnom linijom; QA testove (29 datoteka) prepisati gdje pokrivaju nešto čega nema |
| **C-TIP** | `netgen-tipovi-formi-3266` | `docs/granule/tipovi-formi.md` naspram `Shell/OdabirForme.cs`: daju li isti tip forme za sve granule iz korpusa |
| **C-STB** | `netgen-cvor-stabla-3266` | Combo koji puni stablo, prazan klik i korijeni `TREE` (`#`, `:`) u stvarnom `Grid/Stablo.cs` |
| **C-OKL** | `netgen-grid-stupac-3266` | OznakaKL stupci iz reda: ID skriven, string širine 100 |
| **MULTI-01** | spajanje, `Podloga.NapuniOdabir` | `multiselect` uzima retke s kvačicom (`Form1.vb` 5015), `multiblue` označene (5022). Carpo za oba uzima označene. NetGen briše temp i po `suser_sname()` (4934), Carpo samo po operateru |
| **B-JUP** | 7 grana + odluka korisnika 13. 11. | Jupiter smoke je isključen jer je bio zabranjen upit na jupiter2019/2022. Korisnik je to dopustio: ponovno uključiti, maknuti preskok `Prenesi`/`Retci` po imenu grane u `build.yml` i zaštitu `jupiter2022` u `SqlTekst`/`SpinVeza` |

## Što treba napraviti na GitHubu

1. Push grane `integracija` (paket `integracija.bundle` u folderu, upute u odgovoru).
2. Otvoriti PR `integracija → main` i pustiti CI na `sql-ai`.
3. Kad je CI zelen: spojiti u `main`, zatvoriti ostalih 80 PR-ova s komentarom koji upućuje na ovaj dokument.
4. Dalje: jedna tema, jedna mala grana iz `main`, test učinka uz svaki PR.
