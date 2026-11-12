# Treći agent — preuzeto

Grana `cursor/netgen-treci-otvoreno-889d`. Ne diram posao implementacije (`cursor/netgen-rucni-start-3266`: ručni Start, `Manager_FormClosing`, UI smoke, čišćenje build mapa, `zadnji.txt`, uvoz klijentskog XML/SQL, `MPP-20`, `JEZ-57`) ni QA (`cursor/qa-obrasci-otvoreno-72f9`, PR #60: crveni `Qa.Mora` i popis što korisnik donosi).

Polazni broj je iz zelenog CI-ja [43269944426](https://github.com/tomic04/netgen-u-carpo/actions/runs/43269944426), commit `ef710b9`: `SVE ukupno=354 zeleno=341 otvoreno=13 pad=0`. Zaglavlje u `Otvoreno-za-implementaciju.md` koje kaže 160 je stari snimak CI #31.

## Preuzimam

| Stavka | Što radim |
|---|---|
| JeziciNG `JEZ-31` | Na spinu pročitati `object_definition` od `dbo.spJeziciQuery`. Komentar, zaglavlje `CREATE/ALTER PROC` i `WITH EXECUTE AS` nisu upis. Ako izvršni tekst nema `insert/update/delete/merge/drop/alter/truncate/exec/execute/into`, postojeći test smije poslati `@action=0`. NetGen taj poziv šalje (`Form1.vb` 3882, `Class1.vb` 113–118) jer u tekstu nema `@@`. Ako ključna riječ ostane, stavka ostaje otvorena. Test se ne popušta. |

## Ne preuzimam

| Stavka | Zašto ostaje |
|---|---|
| `FAK-10`, `DOS-10`, `KUL-12`, `KND-06`, `BVG-07`, `KUC-07`, `KUP-05`, `NAR-09`, `MAIL-07`, `RIJ-31`, `RIJ-33` | Testni red `asm:NetGenDLL.loader:<xml>` u `scripts/spin/Meni.ps1`. Nije klijentski red. |
| `DOS-11` | `scripts/spin/dospjele.sql`: tri procedure, stupac 0/1 kao `Form2.vb` 2430–2431 i 2503–2504. Testni SELECT, ne klijentsko tijelo. |

# Peti agent — preuzeto

Grana `cursor/netgen-peti-otvoreno-78a6`. Sažetak QA je 164 / 137 zatvoreno / 27 otvoreno (PR #60).

Ne diram tuđe: `JEZ-57`, `MPP-20` (bc-c17744ce); `JEZ-31`, `DOS-11`, `FAK-10`, `DOS-10`, `KUL-12`, `KND-06`, `BVG-07`, `KUC-07`, `KUP-05`, `NAR-09`, `MAIL-07`, `RIJ-31`, `RIJ-33` (bc-5ddb95b3); `STA-12`, `PL-32`, `MPK-30`, `MPK-31`, `STA-34` (četvrti agent).

## Preuzimam

| Stavka | Što radim |
|---|---|
| FakPar `FAK-11` | Na spinu zamijeniti stub `WHERE 1=0` klijentskim tijelom verzije A (`scripts/spin/klijenti/spFakPartneriUvjetiQuery.A.sql`). Test čita `object_definition`. NetGen šalje `ONLOAD` (`Form1.vb` 3882, `Class1.vb` 113–118) jer u tekstu nema `@@`. Ako tijelo ostane stub, stavka ostaje otvorena. Test se ne popušta. |
| TecRazOTV `TRZ-01` | Datoteka testa je `definitions/netgen/TecRazOTV.xml`. Klijentske kopije su `xml/klijenti/TecRazOTV.v1–v3`. Putanja testa se ne mijenja. |
| Poruke `POR-01` | Datoteka testa je `definitions/netgen/Poruke.xml`. Kopija je `xml/klijenti/Poruke.v1.xml`. Putanja testa se ne mijenja. |
| RealizacijaFakture `RFA-00` | Test traži `definitions/netgen/MER/RealizacijaFakture.xml`. Agronom primjerak nije isti md5 kao klijentski `v1`. |
| RealizacijaStavkeNG `REA-00` | Test traži `definitions/netgen/MER/RealizacijaStavkeNG.xml`. Agronom primjerak nije isti md5 kao `v1` ni `v4`. |
