# RealizacijaFakture — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/RealizacijaFakture.xml`. Rang 24, 8 baza. `MER\RealizacijaFakture.xml` nije u repou. XML se ne mijenja. Test ne šalje SQL.

Izgled je Carpo (R74).

## Podloga

`VERSION="1.1"`. Naslov dobije `  v[1.1]` (`Form1.vb` 3524–3525). `ONLOAD=":Parametri"` otvori filter (`3879–3880`). `COMMANDTIMEOUT="180"` (R65).

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` gasi sumu. `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`. `MULTIGRIDLAYOUT="true"`. `MASTERGRIDLAYOUTFILE="realfak@@frekappocemu"`: `@@` se zamijeni prije imena datoteke.

Gumbi: `Parametri` → `Parametri`, `Ispiši` → `PRINTREPXMASTER` (`4945–4948`). `CMD2` nije napisan.

`PAGEHEADERRIGHT="@@tkosmomi"`. `PAGEHEADERLEFT` ima `#ISEMPTY` (R72, `7744–7768`). Prazna vrijednost preskoči sljedeći segment. `SETDEFAULTS` je jedan select bez `#` (`3862–3866`).

## Praznine

- `#ISEMPTY` u zaglavlju nije običan tekst (R72).
- Timeout je 180 s (R65).
- `MER\` kopija nije u repou. Provjera je Agronom primjerak.
