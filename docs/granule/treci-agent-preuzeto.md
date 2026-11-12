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
| `FAK-10`, `DOS-10`, `KUL-12`, `KND-06`, `BVG-07`, `KUC-07`, `KUP-05`, `NAR-09`, `MAIL-07`, `RIJ-31`, `RIJ-33` | Red u `ssMeni.Parametar` nije u spinu. Red se ne izmišlja i `ssMeni` se ne ispravlja. |
| `DOS-11` | `spKntPopuniKontroleNG`, `spOrgShemaQuery` i `spKntOtvStavkeQueryNG` nisu na spinu. Tijelo postoji samo kod klijenata. Ne izmišlja se. |
