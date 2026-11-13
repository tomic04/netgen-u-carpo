# VrCjenik — očekivano ponašanje

Izbornik traži `MER\vrcjenik.xml`. Te datoteke nema. Provjera ide na `definitions/netgen/Agronom granule/VrCjenik.xml` (rang 30, 8 baza, Biovega). XML se ne mijenja. `spVrCjenikQuery_NG` se ne šalje.

Izgled je Carpo (R74). Naslov je `Cjenici  v[1.0]` (`Form1.vb` 3524–3525, R05).

## Podloga

`ONLOAD="spVrCjenikQuery_NG @action = 0"` ne počinje s `:`, pa puni master (`3879–3882`). Nema `TREE`. Stablo se sakrije (`4310–4316`).

`CMD` (nema `BUTTONS`):

| # | Natpis | Naredba |
|---|---|---|
| 1 | &Unesi | `UNOS_VRC` |
| 2 | &Izmjena | `IZMJENA_VRC` |
| 3 | O&briši | `BRISANJE_VRC` |
| 4 | — | nema `CMD4CAPTION` |
| 5 | &Kopiraj | `KOPIRAJ_VRC` |

`MASTERDBLCLICK="IZMJENA_VRC"`.

`CHECKBOXVISIBLE="FALSE"` sakrije stupac 1 mastera (`2043–2046`). `TOTALROWVISIBLE="FALSE"` gasi sumu (`2049–2050`). `DETCHECKBOXVISIBLE="FALSE"` i `DETTOTALROWVISIBLE="FALSE"` vrijede za detail (`2402–2412`, R79). Ova granula nema `DETAILGRID`, pa `GridEX2.RootTable` pod `On Error Resume Next` (`2400`) ne promijeni prikaz. `GROUPBYBOXVISIBLE="TRUE"`. `MASTERCOLUMNS CLEARCOLUMS="false"` ne skida stupce (`7459`).

## Forme

`UNOS_VRC`, `IZMJENA_VRC`, `BRISANJE_VRC`, `KOPIRAJ_VRC`. Na `UNOS_VRC` i `IZMJENA_VRC` piše `REFRESHAFTERCHILD="TRUE"`. Form1 to čita samo s projekta (`3396`). Form2 to čita samo na kontroli (`3058`). Na elementu forme se ne izvrši (R82).

`IZMJENA_VRC` `ONLOAD` je `spVrCjenikQuery_NG @action = 1, @id = @@id`. `ONLOADED` je lanac s `#` (`Form2.vb` 1036 i dalje kroz rafal).

## Praznine

- `MER\vrcjenik.xml` nije u repou. U repou je Agronom kopija.
- `REFRESHAFTERCHILD` na formi se ne čita (R82).
- `DETCHECKBOXVISIBLE` je odvojen od mastera (R79).
