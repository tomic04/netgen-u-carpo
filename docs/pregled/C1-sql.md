# Faza C1: SQL sloj (zamjena `@@`, parametri, međuspremnik)

Datum: 2026-11-13. Grana: `pregled/sql-c1`.

## Metoda: diferencijalni test (D2)

Umjesto čitanja i usporedbe "na oko", VB funkcije su prenesene **doslovno** u `tools/diff/vb_oracle.py`, s VB zamkama:

- `String.Replace` mijenja sve pojave i razlikuje velika i mala slova;
- `Substring` baca iznimku kad je string prekratak;
- `On Error Resume Next` nastavlja s naredbom iza one koja je pala, i kad je to naredba unutar `If` bloka čija je glava pala.

C# kod iz `Carpo.NetGen` izvršava se istim ulazima pod Mono (`tools/diff/CsVoz.cs`), bez Windowsa i DevExpressa.

Ulaz su **svi SQL tekstovi s `@@` iz korpusa**: 13 746 jedinstvenih tekstova iz 609 granula, cijeli atributi i segmenti lanca `#`. Svaki tekst se izvodi u 7 scenarija, ukupno 96 222 slučaja:

| Scenarij | Parametri |
|---|---|
| dijalog | `@Ime=vrijednost,` kao `Form2.PokupiParametre` (tekst u navodnicima, datum `yyyyMMdd`) |
| grid | `@Ime='vrijednost' , ` kao `Form1.PokupiParametreSaGrida` (datum u hr-HR: `13.11.2026. 0:00:00`) |
| grid-ispis | isto, `DatumjezaIspis = True` |
| clipboard / clipboard-ispis | prazni parametri, vrijednosti samo u CB (`IME=`), datum `13.11.2026`, broj `1,5` |
| prazno | ništa: svaki token postaje `null` |
| prefiks | `@ImeX` stoji ispred `@Ime` (VB uzima prvi koji **počinje** imenom) |

Drugi test (`tools/diff/detail_diff.py`) uspoređuje **stvarni put detail grida**:

- VB: `Form1.vb` 2304: `SrediSQLPosljeNextForme(SrediSQL(sql), ParametriSaGrida)`;
- C#: `DetailGrid.Slozi`.

Pokretanje je opisano u zaglavlju `tools/diff/sql_diff.py`.

## Rezultat

| Scenarij | Razlika prije | Razlika poslije |
|---|---:|---:|
| dijalog | 57 | **0** |
| clipboard | 2 464 | **0** |
| clipboard-ispis | 70 | **0** |
| prazno | 6 | **0** |
| grid / grid-ispis / prefiks | ~11 000 | ~11 000, **sve SQL-01** (razmak, vidi dolje) |
| detail put (stvarni) | 11 122 (navodnici, datumi, progutani SQL) | 10 863, **sve SQL-01** osim 5 tokena dijaloga |

## Nalazi

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **SQL-04** | P0 | `@@` iz retka grida (detail, gumbi, dijalog na masteru) rješavala je `DetailGrid.ZamijeniOstatak`, druga implementacija s drugim pravilima | `Form1.vb` 2304, `Class1.vb` 740–977 | `'@@x'` → `''abc''` (neispravan SQL), datum ostaje `13.11.2026. 0:00:00`, token pred prijelomom reda proguta SQL do sljedećeg razmaka (`end`/`else` nestanu). `IzReda` je datum pisao invarijantno (`11/13/2026`) | **ispravljeno**: `SqlTekst.Sredi`, `Sredi2`, `ZaGumb`, `Dijalog`, `Podloga` koriste `SrediPoslije.Petlja`; `IzReda` piše u kulturi računala kao VB `Value.ToString`. `ZamijeniOstatak` je obrisan |
| **SQL-03** | P0 | Datum `@DAT…` iz međuspremnika (npr. SETDEFAULTS `select getdate() as datumdo`) | `Class1.vb` 945: `'` + `Substring(6,4)` + `(3,2)` + `(0,2)` + `'` → `'20261113'` | pomaci 7/4/1 (za vrijednost s navodnikom): `'026.1.3.'` ili `13.11.2026` bez navodnika → SQL greška | **ispravljeno** |
| **SQL-05** | P0 | Petlja `@@` stala nakon 80 tokena | `Class1.vb` 764: bez ograničenja | `spMPKase` (postavke blagajne) ima više od 80 tokena: `@@` ostane i `ExecSql` **ne pošalje upis** | **ispravljeno** (zaštita je provjera da vrijednost ne sadrži token) |
| **SQL-02** | P1 | Token s imenom kraćim od 3 slova (`@@kg`, `@@r1`, `@@c1`, `@@p1`…, 23 granule) | `Class1.vb` 876/888: `Param.Substring(0,4)` baci iznimku i `Resume Next` uđe u granu za datum, pa je vrijednost u navodnicima | bez navodnika: tekst postane ime stupca, datum `20261113` postane broj | **ispravljeno** (isto kao NetGen) |
| SQL-03b | P2 | Usporedba ključa u CB | `Class1.vb` 938: točan VB uvjet, vrijedi zadnji pogodak; `@@@ime` se ne nađe | prvi pogodak, `@@@ime` se nađe | **ispravljeno** |
| **SQL-01** | odluka | Razmak na kraju vrijednosti s grida | `Class1.vb` 867 uzima vrijednost iz neobrezanog dijela `"@x='abc' , "`: `'@@x'` → `'abc '`, `@@x` → `abc ` | bez razmaka | **čeka odluku**, prijedlog R: Carpo (vidi dolje) |
| SQL-06 | P2 | `@@selectedfile`, `@@selectedpath`, `@@FOCUSEDCONTROL` na putu detail grida | `SrediSQL2` ih zamijeni globalnim vrijednostima zadnjeg dijaloga | `null` | otvoreno; u korpusu se pojavljuju samo u lancima dijaloga, gdje Carpo koristi `Sredi2` |
| NUM-01 | P0 | Brojčano polje dijaloga šalje prikazani tekst, `DECIMALS` se ne čita | `Form2.vb` 2086, 3601 | `1.234,5` → `1.234.5` u SQL-u, decimale se ne mogu unijeti | **ispravljeno** (grana `pregled/num-01`) |
| MULTI-01 | P1 | MULTISELECT i MULTIBLUE isto | `Form1.vb` 5015–5032, 4934 | oba uzimaju označene retke; temp se ne briše po `suser_sname()` | otvoreno |

### SQL-01: prijedlog odluke

NetGen u vrijednosti s grida ostavlja završni razmak, jer je niz parametara `@x='abc' , @y='5' , `. Posljedice u NetGenu:

- tekst u navodnicima ode kao `'abc '` i tako se upiše u tablicu;
- `LIKE '@@x%'` traži `'abc %'`;
- `CREATETEXTFILEUTF8(@@putanja.txt)` napravi datoteku `10 .txt`.

U korpusu nijedan `DETAILGRID` ne stavlja token s grida u navodnike. Takvi tokeni u gumbima i dijalozima postoje (npr. `@aopodkojegkrecemo='@@AOP'`).

**Prijedlog:** razlika R u korist Carpa (bez razmaka). To je NetGen artefakt koji puni podatke razmacima, a nijedna procedura se ne bi trebala oslanjati na njega. Ako se želi 1:1, ispravak je jedna linija u `SrediPoslije.Nadji` (vrijednost iz neobrezanog dijela).

## Testovi

| Test | Gdje | Što dokazuje |
|---|---|---|
| `sql02-kratko-ime`, `sql03-cb-datum`, `sql03-cb-datum-ispis`, `sql03-cb-tri-at`, `sql05-vise-od-80`, `sql04-grid-pravila`, `sql04-redak-hr` | `Carpo.NetGen.GridTest` | ispravke na Windowsu (CI) |
| `broj-vrijednost-hr`, `broj-decimals` | `Carpo.NetGen.GridTest` | NUM-01 |
| `tools/diff/sql_diff.py`, `tools/diff/detail_diff.py` | Linux, Mono | cijeli korpus, ponovljivo nakon svake izmjene SQL sloja |

## Što je još u C1

- Lijepljenje parametara nakon dijaloga (`Form1.vb` 5445–5514): `!!` u grani `FILLEXPLORER=false`. NetGen tamo `!!` ne skida, a Carpo ga skida u `ZaSlanje`. Treba odluka po korpusu.
- `ExecSql` odluka i obrada poruka servera (`#OK`, `+OK`, `#close`, `#FOCUS(`), `Class1.vb` 105–205.
- `PokupiParametre` za ostale tipove kontrola (radio, check, multidrop, grid u formi) protiv `ParametriSvi`.
