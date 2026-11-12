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

## Četvrti agent — preuzeto

Grana `cursor/netgen-cetvrti-dokaz-0b55`. Ne diram ručni Start, UI smoke, `zadnji.txt`, `JEZ-31`, `DOS-11` ni 11 redova `ssMeni` iz tablice iznad. `JEZ-57` je preuzet: `PAGEHEADERRIGHT` ide kroz CB (`Form1.vb` 6001, `Class1.vb` 966).

| Stavka | Što radim |
|---|---|
| Statusi `STA-12` | Red s Nivo djeteta, Parent na postojeći MeniID, `fk_meni` ostaje uključen. Loader otvara `Statusi.xml`. |
| PrijemniList `PL-32` | Stupac 0 comboa je TREE tekst (`#` i `:`). `Form1.vb` 3342–3344, 4344, 7730–7733. |
| MPPorKPNG `MPK-30` | `FILLEXPLORER=true` zamijeni master s `ONSUBMIT`. `Form1.vb` 5445–5452. |
| MPPorKPNG `MPK-31` | Prazan `FILLEXPLORER` izvrši `ONSUBMIT` pa ponovi zadnji master. `Form1.vb` 5509–5519. |
| Statusi `STA-34` | Forma bez kontrola ne otvara dijalog. `Form1.vb` 5369–5370, 5445–5452. |
| MPPonuda `MPP-20` | `enable`/`disable` traži `Button.Text`. `Form1.vb` 3914–3921. Nije zatvoreno na `cursor/netgen-rucni-start-3266`. |
| LVObracun `LVO-20` | `FILLEXPLORER=onlytree` zove samo stablo, master se ne šalje. `INSERTLVGLA(@hrkadroviid = 0)` je forma `INSERTLVGLA`. `Form1.vb` 5475, `Form2.vb` 4407–4418. |
| KntBlaPromet `KBP-20` | `CLICK(Ok)` izvrši `ONCLICK` gumba `ok`. `close` ne guta `:REPX/@@template`. `Form2.vb` 7151–7155, 7033, 6882. |
| MPPorKPNG `MPK-43` | SQL filtera `spMPPoravnajKP` ide u `NgDnevnik` kao `NetGen SQL`, ne u CBLog. `Class1.vb` 138. |
| ZakljucivanjeTemNG `ONK-04` | `Zaključi` otvara `Zakljucavanje`. `spKntZakljucivanjeTem` se ne šalje. `Form2.vb` 6882. |
| MPNivelacija `NIV-05` | `Pokaži` šalje `ONSUBMIT` jer je `FILLEXPLORER=TRUE`. `Form1.vb` 5445. |
| MPRealizNG `REA-04` | `Pokaži` na `FORM1` šalje `ONSUBMIT` jer je `FILLEXPLORER=TRUE`. `Form1.vb` 5445. |
