# KntPDVObrasciEU — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KntPDVObrasciEU.xml`. Isti SHA-256 kao `Agronom granule/KntPDVObrasciEU.xml`. U izborniku je na 10 baza, uključujući Biovegu. XML se ne mijenja. SQL obrazaca se u testu ne šalje.

Izgled je Carpo (R59). Boja stupca (`FORECOLOR` / `BACKCOLOR` prazni), font i položaj gumba su **izgled – Carpo**. Natpis, redoslijed, naredba, širina, Enter i fokus ostaju.

Log ide u `NgDnevnik`.

## Podloga

Naslov je `PDV Obrasci EU 2`. `NAME` je `PDV Obrasci EU` i nije naslov (`Form1.vb` 3522–3528). Nema `VERSION`.

`ONLOAD=":FILTER_FORM"`. Prvo se otvori filter, master se ne šalje (`3879–3880`, R18).

Nema `TREE` ni `DETAILGRID`. Stablo se sakrije, splitter je 0 (`4310–4316`).

`CHECKBOXVISIBLE="FALSE"` i `TOTALROWVISIBLE="FALSE"` gase checkbox stupac i sumu (`2043–2053`). `GROUPBYBOXVISIBLE="FALSE"` gasi group box (`3350`).

`ONENTERKEY="IZMIJENI_FORM"`. Prazan `EDITCOLUMNS` stavi `EnterKeyBehavior=None` (`3419–3421`), ali Enter i dalje otvori formu (`7021–7023`, R68).

`PAGEORIENTATION="LANDSCAPE"` čita se (`3458`) i ide u Janus `IspisNovi` (`5930`). Gumb ispisa je `repx/`, a taj put zove `PrintRpt` i orijentaciju ne gleda (`5215–5248`, R41).

`PAGEHEADERRIGHT="@@DatumVrijeme"`, `PAGEFOOTERLEFT="@@TKOSMOMI"`, `PAGEFOOTERRIGHT` je fiksni tekst. `@@` se zamijeni prije ispisa.

## `BUTTONS`

Sedam segmenata, rez na `#` (`3770–3788`, R56). Prepišu gumbe 1–7. `CMD` na projektu nema, pa 8–10 ostanu skriveni.

| # | Natpis | Naredba |
|---|---|---|
| 1 | Izmijeni | `IZMIJENI_FORM` |
| 2 | Parametri | `FILTER_FORM` |
| 3 | Dodatni podaci PDV | `PDV_FORM` |
| 4 | Preknjiženja PDV | `PREKNJIZ_FORM` |
| 5 | Predispis PDV | `PREDISPIS_FORM` |
| 6 | Ispis | `repx/@@Template(@id = @@id, @datum = @@datod, ...)` |
| 7 | Eporezna | `ASSEMBLYLOAD(EPoreznaObrasci.Program)` |

Ime izvještaja je vrijednost `@@Template`, ne fiksni tekst (R55). `repx/` postane `repx:` (`5238–5239`).

`ASSEMBLYLOAD(` na podlozi učita `EPoreznaObrasci`, klasa `Program` (`4584`, R35). To nije `Ime.Start` (R67).

## Stupci

`MASTERCOLUMNS` nema `CLEARCOLUMS`. Širine `/14`. `Naziv` 1500, `PDVIdentBr` 2000. Prazan `FORECOLOR` ne boji ćeliju (`7567`). Boja je **izgled – Carpo**.

## Forme

`FILTER_FORM`, `IZMIJENI_FORM`, `PDV_FORM`, `PREKNJIZ_FORM`, `PREKNJIZ_UNOS_FORM`, `PREKNJIZ_IZMJENA_FORM`, `PREDISPIS_FORM`. `ONSUBMIT` koji počinje sa `sp` lijepi parametre (`Form2.vb` 3378–3388). Greška `ONLOAD` ne otvara formu (`1199`).

## Praznine

- Enter otvara `IZMIJENI_FORM` iako je kretanje Enterom ugašeno (R68).
- Ispis uzima `@@Template`. `PAGEORIENTATION` taj ispis ne okrene (R41).
- Eporezna je `ASSEMBLYLOAD`, ne `Ime.Start` (R35, R67).
- `BUTTONS` prepiše sedam gumba i ne dira osmi (R56).
