# MPKnjigaPopisa — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPKnjigaPopisa.xml`. Rang 25, 8 baza, Biovega. XML se ne mijenja. `spMPKnjigaPopisaSveTrgNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879` vidi prazan string, ne ulazi u `:`). `BUTTONS` je prazan. Nema `CMD`. Jedina forma je `MAINFORM`, a ništa je ne zove.

`MASTERDBLCLICK="KARTICA"`. Forme `KARTICA` nema. Dvostruki klik ne otvara dijalog (`5369–5370`).

`PAGEORIENTATION="PORTAL"`. Samo `LANDSCAPE` mijenja orijentaciju (`5930`, R76). `PORTAL` ostane uspravno. `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="FALSE"`. `FROZENCOLUMNS="0"`. `TOTALROWVISIBLE` nema, suma ostaje.

## `MAINFORM`

`NAME=" MAINFORM "` ima razmake. Tag forme je `MAINFORM`. `COMMANDTIMEOUT="600"` je na formi (`Form2.vb` 1035). `WINDOWSTATE="0"`. `MAXIMIZEBOX="TRUE"`.

`ONLOAD` vraća `datumod`, `tkosmomi`, `DatumVrijeme`, `NoviRed`. `group` `DOCK="1"` je vrh, `DOCK="2"` dno (R33). Grid `mastergrid` `DOCK="5"` puni sredinu. Njegov SQL, ako godine `@@datumod` i `@@datumdo` nisu iste, zove `spRaiserror` i ne nastavlja pregled. Test to ne šalje.

Gumb `b0` `ONCLICK="select 'refresh' as mastergrid"` ponovo puni grid čije je ime stupac (R27). `b1` je `PRINTGRID(mastergrid)`. `b3` je `close`. `trgovinaidid` `ALLOWEMPTY="TRUE"`. `CAPTION` sa `*` je obavezan natpis (`2959–2976`).

## Praznine

- Podloga nema gumb koji otvara `MAINFORM`.
- `KARTICA` ne postoji.
- `PORTAL` nije `LANDSCAPE` (R76).
- Grid na dijalogu nije master podloge (R53).
