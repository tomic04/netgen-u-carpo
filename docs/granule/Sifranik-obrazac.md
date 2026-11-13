# Šifrarnik NG — zajednički obrazac

Isti raspored imaju `DrzaveNG`, `ZupanijeNG`, `NacinOtpremeNG` i `CarinskaTarifaNG`. Puna checklista prve tri ostaje u njihovim datotekama. Nova granula istog obrasca dobije samo razlike.

Izgled je Carpo (R74). XML se ne mijenja.

## Podloga

`ONLOAD` je `sp…Query @action=0`. To je upit, master se puni odmah (`Form1.vb` 3879–3882).

| # | Naredba |
|---|---|
| 1 | `ImeUnesi#refreshmaster` |
| 2 | `ImeIzmijeni` |
| 3 | `ImeBrisanje` |
| 5 | `PRINTREPXMASTER` |

`MASTERDBLCLICK` je forma izmjene. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. `SETDEFAULTS` puni `tkosmomi` iz `partneri` / `pubvars`.

## Dijalozi

Unos nema `ONSUBMIT`. Spremi šalje `sp…Azur @action=0` i ne završava s `#OK`, pa forma ostane (`7509`). Izuzetak je `NacinOtpremeNG`: iza procedure ide `OKStay`, forma i dalje ostane (`6884–6887`).

Gumb s natpisom Odustani na unosu ima `ONCLICK` `OK` ili `ok`. To je potvrda (`6888`), ne `CLOSE`. `NEXTFORM="Dodaj"`. Forma `Dodaj` ne postoji, dijalog se ne prikaže (`4430`).

Izmjena završava s `#OK` nakon `@action=1`. Njen Odustani je `CLOSE`.

Brisanje šalje `@action=2 …#OK`. Ta procedura mijenja šifrarnik. Na Jupiteru se ne šalje.

## Razlike

| Granula | Upit | Azur | Odustani na unosu |
|---|---|---|---|
| `DrzaveNG` | `spdrzaveQuery` | `spDrzaveAzur` | `OK` |
| `ZupanijeNG` | `spZupanijeQuery` | `spZupanijeAzur` | `OK` |
| `NacinOtpremeNG` | `spProNacinIsporukeQuery` | `spProNacinIsporukeAzur` | `ok`, a Spremi ima `OKStay` |
| `CarinskaTarifaNG` | `spCarinskaTarifaQuery` | `spCarinskaTarifaAzurNG` | `ok` |
