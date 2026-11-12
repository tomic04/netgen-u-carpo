# Smoke Jupiter

Samo put otvaranja: ONLOAD, master, stablo, detail, combo i SQL na filteru. Nema gumba, nema ONSUBMIT, nema ispisa, nema fiskalizacije.

Prije ovog runnera smoke na jupiter2019 i jupiter2022 nije pokrenut. Nijedna procedura nije izvršena.

`Application Name=CarpoNetGen-smoke`, `CommandTimeout` 30 s, `Pooling=false`, jedna granula pa zatvorena konekcija. Operater je 7, `@@node` je `null`.

Zaštita ne gleda samo prvu riječ. `spKntZakljucivanjeTem @action=1` i `spKntPreknjizavanjeNG @action=2` su poziv procedure. Prije EXEC-a ide `SELECT` iz `sys.sql_modules`, i rekurzivno za procedure koje tijelo zove. INSERT/UPDATE/DELETE/MERGE/TRUNCATE/DROP nad tablicom koja nije `#temp` ili `@varijabla` se ne pokreće (`procedura-piše`). Izravni tekst s DELETE/UPDATE/DROP/TRUNCATE/MERGE/ALTER/GRANT/REVOKE, CREATE osim `CREATE TABLE #temp`, ili INSERT osim u `#temp`, je `preskočeno-zaštita`.

Usporedba s `docs/granule/ssMeni-svi-serveri.md` (inventar, CI 43233316047). Inventar je broj NetGen redova u izborniku.

| Server | Baza | Inventar | Izbornik | s retcima | prazno | pad | preskočeno-zaštita | procedura-piše | nema-xml |
|---|---|---:|---:|---:|---:|---:|---:|---:|---:|
| jupiter2019 | Biovega | 422 |  |  |  |  |  |  |  |
| jupiter2019 | CentarTehnike | 174 |  |  |  |  |  |  |  |
| jupiter2019 | Setcor | 152 |  |  |  |  |  |  |  |
| jupiter2019 | Enna | 167 |  |  |  |  |  |  |  |
| jupiter2019 | Ireks | 172 |  |  |  |  |  |  |  |
| jupiter2022 | Panissimo | 311 |  |  |  |  |  |  |  |
| jupiter2022 | Tusak | 309 |  |  |  |  |  |  |  |
| jupiter2022 | CentarTehnike | 177 |  |  |  |  |  |  |  |
| jupiter2022 | Setcor | 124 |  |  |  |  |  |  |  |

Runner na sql-ai prepisuje ovu datoteku brojkama po bazi. Do tog upisa ništa nije slano.
