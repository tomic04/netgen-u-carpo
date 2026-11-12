# AOPNovi — stablo na dijalogu

XML: `definitions/netgen/BIOVEGA/AOPNovi.xml` (kopija u `Agronom granule/AOPNovi.xml`).

`INPUT TYPE="tree" NAME="leftgrid" SQL="spKntAOPOBRASCITreeAzur @action='tree' :LIJEVIGRIDFORMA"`.

Jedan segment (nema `#`). Prva `:` dijeli na lijevo `spKntAOPOBRASCITreeAzur @action='tree' ` i desno `LIJEVIGRIDFORMA`. Lijevo sadrži `tree`, pa `DazovaIzStore` (`Form2.vb` 2575–2579, `2825`). Nije `yearmonth` (to bi na dijalogu bilo prazno tijelo, `2576–2577`).

Tag čvora je stupac 3 bez dodavanja `:LIJEVIGRIDFORMA` (`2842`, `2864`). Podloga bi, ako stupac 3 nema `:`, dodala `:ime` (`Form1.vb` 4474–4478`).

Parametar je `@leftgrid='cijeli tag'` (`3667–3671`). `@@node` podloge je samo dio prije `:`.

`POPUP` ima `~` pa `refresh` grida. `ExpandAll` (`2600`). `ALLOWEMPTY` se na ovoj kontroli ne čita kao kod comba; atribut na stablu se uzme (`2569`) i ne ubaci čvor.

## Rupe

- Dijaloško stablo nije `TreeView2`. Nema `TREETRANSLATE` ni `ImageList2`.
- `yearmonth` na `INPUT TYPE="tree"` ne crta godine.
- Tag nije `@@node`.
