# QA dnevnik (grana `cursor/qa-obrasci-otvoreno-72f9`)

QA agent je od 12. 11. 12:30 do 13. 11. 04:24 provjeravao stavke drugih agenata (100 commitova).
Grana nikad nije spojena. Kod u `Devex_26` nije mijenjala.

U integraciju su preuzeti:

- **`docs/granule/*.md` kojih nije bilo** (118 datoteka): QA checkliste po granuli, `ssMeni-svi-serveri.md`, `Prioritet*.md`, `Nedostaje-*.md`.
- **Ova mapa:** QA verzije 11 dokumenata koji postoje i u glavnoj liniji, a razlikuju se.
  Ime je putanja pod `docs/` s `__` umjesto `/`. Glavna linija i dalje vrijedi.
  Razlike se usklađuju u fazi C pregleda.

Nisu preuzeti testovi iz `tests/Carpo.NetGen.Granule` te grane (29 datoteka, ~8 000 redova).
Pisani su nad API-jem od 12. 11. i nisu provjereni nad integracijom. Ostaju na grani
kao izvor slučajeva za fazu C.

QA tijek po commitovima: `git log origin/cursor/qa-obrasci-otvoreno-72f9`.
Zadnje stanje QA brojača: 164 stavke, 136 otvoreno, 28 potvrđeno (commit `dcc4fac`).
