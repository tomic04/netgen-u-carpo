# MPKntNacinPlacAzur — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPKntNacinPlacAzur.xml`. U izborniku je na 10 baza, uključujući Biovegu. Agronom kopija nije isti SHA-256. Mjerodavan je BIOVEGA. XML se ne mijenja. `spMPKntNacinPlacAzur` se u testu ne šalje.

Izgled je Carpo (R59). Natpis, redoslijed, naredba, detail, enable i fokus ostaju.

Log ide u `NgDnevnik`.

## Podloga

Naslov je `Ažuriranje načina plaćanja u MP ` s razmakom na kraju (`Form1.vb` 3522–3528). `NAME` ima dva razmaka na kraju i nije naslov.

`ONLOAD=":FILTERFORM"` otvori filter prije mastera (`3879–3880`, R18).

`DETAILGRID="spMPKntNacinPlacAzur @action=10,  @kntnacinplacid = @@id"`. Veza je stupac `id` (R11).

`CMD1`–`CMD6` su prazni. `BUTTONS` ima pet segmenata i prepiše gumbe 1–5 (`3770–3788`, R56). Šesti ostane skriven.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Dodaj | `DODAJFORM` |
| 3 | Izmjeni | `UPDATEFORM` |
| 4 | POS banke | `BANKAPOS` |
| 5 | Provizija | `PROVIZIJA` |

`MASTERDBLCLICK="UPDATEFORM"`. `POPUPMASTER` je prazan: standardne stavke konteksta ostanu (`3620–3673`).

`CHECKBOXVISIBLE="FALSE"` sakrije checkbox (`2043–2046`). `TOTALROW="FALSE"` se ne čita. `TOTALROWVISIBLE` nema, suma ostaje uključena (`2049–2053`, R66). `GROUPBYBOXVISIBLE` nema, group box ostaje uključen.

`SETDEFAULTS="select dbo.fnTkoSmoMi() as tkosmomi"` ide prije mastera (`3862–3866`). Nema `#`. `PAGEHEADERRIGHT="@@tkosmomi"`.

## Filter

`FILLEXPLORER="TRUE"`. `ONSUBMIT="spMPKntNacinPlacAzur @action=0, "` postane master (`5445–5452`, R63). Počinje sa `sp`, parametri se lijepe (`3378–3388`).

Combo `svi` ima `ALLOWEMPTY="FOLSE"`. To nije `true` (`2402–2404`, R31). Prazan red se ne ubaci. Prazan odabir pokuša `SelectedValue=1` (`3455–3468`).

Gumbi forme: `Pokaži` i `Odustani`, cijeli natpis (`1061–1066`).

## Unos

`DODAJFORM` `FOCUS="Naziv"`. Nakon učitavanja `AktivnaKontola` postane ta kontrola i `Timer1` je uključen (`Form2.vb` 1053, `2792–2794`). `ONLOAD="select null as kntnacinplacid"`. `ONSUBMIT` je prazan.

## Praznine

- `TOTALROW="FALSE"` ne gasi sumu (R66).
- `ALLOWEMPTY="FOLSE"` ne znači prazan red (R31).
- `BUTTONS` prepiše pet gumba (R56).
- `FOCUS="Naziv"` stavi fokus na Naziv nakon otvaranja (`1053`).
- Detail je `@kntnacinplacid = @@id` (R11).
