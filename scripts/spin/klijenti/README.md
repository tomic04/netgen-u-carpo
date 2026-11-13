# Klijentske definicije za spin

Izvorna matrica (koja verzija na kojem klijentu) je `IZVOR.md`. Tijela se ne mijenjaju. `PrimijeniKlijente.ps1` ih stavlja na `localhost/spin` nakon prijenosa s Jupitera, tako da ova verzija ostane.

| Objekt | Verzija na spinu | Zašto |
|---|---|---|
| `spBLZakljucakQueryNG` | A | 4 baze, više od B i C |
| `spFakPartneriUvjetiQuery` | A | 3 baze, B je samo Panissimo |
| `spKultureNGQuery` | A | 5 baza |
| `spKUPPrometQueryN` | A | 2 baze (Žito); ostale su po jedna |
| `spStatusiQuery` | A | 6 baza |
| `klmasterroba` | postojeća tablica na spinu, plus `oznaka2 varchar(30) NULL` | A je najbliža referenca strukture, ali nema `oznaka2`. Taj stupac traži `OznakaKL`, a imaju ga samo C, H i K, sve `varchar(30)`. Tablica se ne ruši i retci se ne brišu. |

Ako procedura zove tablicu koje na spinu nema, skripta je složi iz `sys.columns` na jupiter2019/jupiter2022 (samo definicija, bez redaka). Ako je nema ni tamo, ostane stub `id int NULL` i to se zapiše u `artifacts/spin/klijenti.md`.
