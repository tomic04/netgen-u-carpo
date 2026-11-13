# KUPVrstPN — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/KUPVrstPN.xml`. Rang 57, 5 baza. Natpis je `Vrsta pošte`. XML se ne mijenja. `spKUPVrstaPosteAzur` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je `select` nad `kupvrstaposte`. Puni master (`3879–3882`). Nema `TREE`.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Unesi | `INSERTFORM` |
| 2 | Izmjeni | `UPDATEFORM` |
| 3 | Obriši | `DELETEFORM` |

`MASTERDBLCLICK="UPDATEFORM"`. `CHECKBOXVISIBLE="FALSE"`.

`ONSUBMIT` svih triju formi počinje sa `spKUPVrstaPosteAzur`, pa se niz `@ime=` doda (`Form2.vb` 3378–3388, R14). `@action=2` na brisanju mijenja podatke i na jupiteru se ne šalje.

## Praznine

- Brisanje je `sp` koji piše. Zaštita po riječi `delete` ga ne vidi.
