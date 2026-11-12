# Treći agent — preuzeto

Grana `cursor/netgen-treci-otvoreno-889d`. Ne diram posao implementacije (`cursor/netgen-rucni-start-3266`: ručni Start, `Manager_FormClosing`, UI smoke, čišćenje build mapa, `zadnji.txt`, uvoz klijentskog XML/SQL, `MPP-20`, `JEZ-57`) ni QA (`cursor/qa-obrasci-otvoreno-72f9`, PR #60: crveni `Qa.Mora` i popis što korisnik donosi).

Polazni broj je iz zelenog CI-ja [43269944426](https://github.com/tomic04/netgen-u-carpo/actions/runs/43269944426), commit `ef710b9`: `SVE ukupno=354 zeleno=341 otvoreno=13 pad=0`. Zaglavlje u `Otvoreno-za-implementaciju.md` koje kaže 160 je stari snimak CI #31.

## Preuzimam

| Stavka | Što radim |
|---|---|
| JeziciNG `JEZ-31` | Na spinu pročitati `object_definition` od `dbo.spJeziciQuery`. Komentar, zaglavlje `CREATE/ALTER PROC` i `WITH EXECUTE AS` nisu upis. Ako izvršni tekst nema `insert/update/delete/merge/drop/alter/truncate/exec/execute/into`, postojeći test smije poslati `@action=0`. NetGen taj poziv šalje (`Form1.vb` 3882, `Class1.vb` 113–118) jer u tekstu nema `@@`. Ako ključna riječ ostane, stavka ostaje otvorena. Test se ne popušta. |

## Zatvoreno na spinu, testni podaci

| Stavka | Dokaz |
|---|---|
| `DOS-11` | `scripts/spin/dospjele.sql`. Carpo zove procedure i čita stupac 0/1 (`Form2.vb` 2430–2431, 2503–2504). CI [43284522194](https://github.com/tomic04/netgen-u-carpo/actions/runs/43284522194). |
| `FAK-10` … `RIJ-33` | `scripts/spin/Meni.ps1` upiše testni `asm:NetGenDLL.loader:<xml>`. `Parent` je izračunat, `fk_meni` ostaje na check. Loader otvori `xml\` + tekst iza druge dvotočke (`Loader.vb` 183, `Form1.vb` 1251–1258 i 1139). Zadnji, `RIJ-33`: CI [43305196144](https://github.com/tomic04/netgen-u-carpo/actions/runs/43305196144). |

## I dalje otvoreno

`KAR-12`, `INV-13`, `DOB-08`, `MPB-07`, `RIJ-30`, `RIJ-32`. Red u `ssMeni` nije otvaranje granule.
