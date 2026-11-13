# RealizacijaStavkeNG — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/RealizacijaStavkeNG.xml`. Rang 19, 9 baza, nije na Biovegi. `MER\RealizacijaStavkeNG.xml` iz izbornika nije u repou. XML se ne mijenja. Test ne šalje SQL.

Izgled je Carpo (R74).

## Podloga

`VERSION="1.5"`. Naslov dobije `  v[1.5]` (`Form1.vb` 3524–3525). `ONLOAD=":Parametri"` otvori formu (`3879–3880`). `COMMANDTIMEOUT="180"` (R65). `ICON="SQL Script Search.ico"` traži `Graphics\Icons\` (`3440`).

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` gasi sumu (`2049–2050`). `GROUPBYBOXVISIBLE="TRUE"`. `FROZENCOLUMNS="0"`. `MULTIGRIDLAYOUT="true"`. `MASTERGRIDLAYOUTFILE="REAL1@@frekappocemu"`: `@@` se zamijeni prije imena datoteke layouta.

Gumbi: `Parametri` → `Parametri`, `Ispiši` → `PRINTREPXMASTER`. `PRINTREPXMASTER` zove `IspisGRIDREPX` (`4945–4948`). `CMD2` nije napisan.

`PAGEHEADERRIGHT="@@tkosmomi"`. `PAGEHEADERLEFT` ima `#ISEMPTY(@@datumod)#`. `Rafalna_Priprema` reže na `#` (`7744–7768`, R72). Ako je vrijednost u zagradi prazna ili `null`, preskoči sljedeći segment. Inače preskoči dva i uzme treći. `**` u uzetom tekstu postane razmak (`7764`).

`SETDEFAULTS` je jedan select bez `#` (`3862–3866`).

## Forme

`Parametri` i `xParametri`. Filter koji se otvara je `Parametri`.

## Praznine

- `#ISEMPTY` u zaglavlju nije običan tekst (R72).
- Timeout je 180 s (R65).
- Datoteka iz `MER\` nije u repou. Provjera je na Agronom primjerku.
