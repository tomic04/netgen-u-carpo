# CI

Push i pull request pokreću `build`. To je brzi skup.

- Spin stubovi, prijenos definicija i kratki `Otvori.ps1` (imenovani masteri, ne sve datoteke).
- Build.
- Smoke parsera NetGen XML-a.
- Grid testovi, defaulti i checklista (`Carpo.NetGen.Granule.exe --izvjestaj`: stavke `[Z]` i `Qa.Mora`).
- Uzorak otvaranja, 2–3 granule po tipu forme (`--uzorak`):
  - Manager: `KntKompVerizne`, `Statusi`, `KntBlaZak`
  - obična forma: `OznakaKL`, `JeziciNG`
  - dijalog na ulazu: `RealizacijaStavkeNG`, `KarticaPartnera`
- UI smoke (`Carpo.TestShell.exe --ui-smoke`).

`nocni-otvaranje` otvara svaku NetGen datoteku pod `definitions/netgen` i svaki `asm:NetGenDLL` red iz `ssMeni` (`--ssmeni`, metoda `Repo`). To je puni skup. Ništa od tih testova nije obrisano.

Raspored je `0 21 * * *` (UTC). To je 22:00 Europe/Zagreb zimi (CET). Ljeti je 23:00. Ručno pokretanje je `workflow_dispatch` na istom workflowu.

`build` i dalje ima svoj `workflow_dispatch`, i on pokreće brzi skup.

Na kraju `build` ide čišćenje `C:\NetGenCarpo`. Prvo dry-run ispiše što bi obrisao. Zatim ostavi 5 najnovijih mapa. Mapa iz `zadnji.txt` i mapa iz koje se `Carpo.TestShell` vrti se ne brišu.
