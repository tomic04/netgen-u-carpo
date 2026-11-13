# MPInvRazlike — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPInvRazlike.xml`. Rang 93, 2 baze. Natpis je `Inventurne razlike u MP`. XML se ne mijenja. `spMPQueryGlaIDetNG` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master čeka čvor (`3879`). `DETAILGRID` veže `@id=@@id` i `@nenc = 1` (R11). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`.

`TREE` počinje s `Danas:DANAS_FORM`, pa `spMPTreeNG` (store, `4350–4351`).

| # | Natpis | Naredba |
|---|---|---|
| 1 | &Parametri | `FILTERFORM` |
| 2 | &Dodaj | `INSERTFORM@@indvrdok` |
| 3 | &Izmjeni | `UPDATE_FORM` |
| 4 | &Zaključi | `ZAKLJUCI` |
| 6 | &Print dok. | `vrstadokid-MaloprodajaGla` (R43) |
| 7 | Lista | nema `:`, gumb se ne složi (`3320`, R60) |

Iza dvotočke na Dodaj stoji `INSERTFORM@@indvrdok`. To je ime koje se traži. Forma `INSERTFORM` nije taj string.

`POPUPMASTER` završava s `Stornoraj#STORNO`. Niti jedan segment nema `:`. Stavka se ne složi (R89, `3564–3566`).

`MASTERDBLCLICK="UPDATE_FORM"`. `SETDEFAULTS` je `spmpPermissionNG`.

## Praznine

- Dodaj ne otvara `INSERTFORM`.
- `Stornoraj` i `STORNO` nisu stavke izbornika (R89).
