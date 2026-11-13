# MPPSNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPPSNG.xml`. Rang 34, 7 baza, Biovega. Natpis je `Početno stanje MP`. XML se ne mijenja. `spMPQueryGlaIDetNG` i `spMPTreeNG` se ne šalju.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se ne puni na ulazu (`3879`). Puni ga čvor stabla čija forma ima `FILLEXPLORER="TRUE"` (`5445–5452`).

`DETAILGRID="spMPQueryGlaIDetNG @action=2,@operateriid=@@operateriid,@id=@@id, @nenc = @@nenc"`. `@@id` je vrijednost mastera, ne natpis stupca (R11).

`TREE` (segmenti po `#`, `4298`, `4344–4356`):

| Segment | Što NetGen napravi |
|---|---|
| `Granula >>> PS u MP` | nema `:`. `IndexOf` je −1, `On Error Resume Next` preskoči. Čvor se ne doda i nije roditelj (R10). |
| `Danas:DANAS_FORM` | korijen, tag `:DANAS_FORM` |
| `Jučer:JUCER_FORM` | korijen, tag `:JUCER_FORM` |
| `spMPTreeNG @action=1:TEKUCIMJESEC_FORM` | sadrži `tree`, store (`4350–4351`, R01) |
| `Prošli mjesec:PROSLIMJESEC_FORM` | korijen |
| `spMPTreeNG @action=2:FILTERODDO_FORM` | store |
| `spMPTreeNG @action=3:FILTERODDO_FORM` | store |
| `spMPTreeNG @action=4:FILTEROPERATERIFORM` | store |

`SETDEFAULTS="spMPPermissionNG @action = 0 , @operateriid = @@operateriid, @object = '@@xmlname'"`. `COMMANDTIMEOUT="300"` (R65).

`ONLOAD2` vraća `enable`/`disable` u stupcima `Dodaj`, `Izmjeni`, `Zaključi`, `Obrisi`, `Storno`, `otkljucaj`. Stupac se traži po natpisu gumba (`3914–3921`, R57). Pogode `Dodaj`, `Izmjeni` i `Zaključi`. `Obrisi`, `Storno` i `otkljucaj` nemaju gumb tog natpisa.

Gumbi:

| # | Natpis | Naredba |
|---|---|---|
| 1 | Parametri | `FILTERFORM` |
| 2 | Dodaj | `INSERTFORM` |
| 3 | Izmjeni | `UPDATE_FORM` |
| 4 | Zaključi | `ZAKLJUCI` |
| 5 | Otvori | `OTVORI` |
| 6 | Ispis dok | `vrstadokid-MaloprodajaGla` |
| 7 | Lista | nema `:`, gumb se ne složi (`3320`, R60) |
| 8 | Kontrola | `KONTROLAPS` |

`vrstadokid-` nije ime forme (R43). `MASTERDBLCLICK="UPDATE_FORM"`.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="TRUE"` ostavlja sumu (`2049–2052`). `DETTOTALROWVISIBLE="FALSE"` gasi sumu detaila (`2408–2409`). `GROUPBYBOXVISIBLE="FALSE"`. `AUTOFITCOLUMNS="False"` se nigdje ne čita (R80). Širine ostaju iz `Class1`. `REPORT=""` se ne čita.

`POPUPMASTER`: `Izmjeni:UPDATEFORM` traži formu `UPDATEFORM`. U XML-u je `UPDATE_FORM`. Forme nema, dijalog se ne otvori (`5369–5370`, R19). Gumb Izmjeni i dalje otvara `UPDATE_FORM`. `Traži:TRAZI` isto nema formu.

## Forme koje pune master

`DANAS_FORM`, `JUCER_FORM`, `TEKUCIMJESEC_FORM`, `PROSLIMJESEC_FORM`, `FILTERODDO_FORM` imaju `FILLEXPLORER="TRUE"` i `ONSUBMIT` na `spMPQueryGlaIDetNG @action=1`. `FILTERODDO_FORM` nosi `@node='@@node'`.

`UPDATE_FORM0` ima `NEXTFORM=":@@UPDATE_FORM"`. Ime sljedeće forme je vrijednost kontrole (`Form1.vb` 5381–5384, R49).

## Praznine

- Prvi segment stabla nema `:` i ne postane roditelj (R10).
- `AUTOFITCOLUMNS` se ne čita (R80).
- `Lista` ostaje skrivena (R60). Ispis je `vrstadokid-` (R43).
- Popup `UPDATEFORM` nije `UPDATE_FORM` (R19).
