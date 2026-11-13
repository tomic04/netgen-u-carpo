# OdobreniKaloNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/OdobreniKaloNG.xml`. Rang 96, 2 baze, Biovega. Natpis je `Odobreni kalo`. Isti `TREE` i `KlMaster` su u `Agronom granule/OdobreniKaloNG.xml`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). Nema `CMD` ni `BUTTONS`.

`TREE="spTreeKlMasterRoba:KlMaster"`. Lijevo od `:` sadrži `tree`, pa čvorovi dolaze iz storea (`4350–4351`, R01). Stupac 0 je id, stupac 1 roditelj, stupac 2 natpis. Stupac 3 s `:` postane tag, inače je tag `id:KlMaster` (`4471–4477`, `4510–4514`).

`KlMaster` ima `FILLEXPLORER="TRUE"` i `ONSUBMIT="spKaloQueryNG @action=1, @klmasterrobaid=@@node"`. Klik čvora otvori tu formu, a `true` zamijeni master tim upitom (`5445–5452`). `@@node` je id čvora.

`CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"` gasi sumu (`2049–2050`).

## Praznine

- Prazan master na ulazu čeka čvor. Store se na ulazu ipak izvrši da napuni stablo (`4442–4447`).
