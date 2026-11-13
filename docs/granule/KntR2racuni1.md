# KntR2racuni1 — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/KntR2racuni1.xml`. Rang 17, 10 baza, Biovega. XML se ne mijenja. Test ne šalje SQL.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"` otvori prikaz prije mastera (`Form1.vb` 3879–3880). Nema stabla ni detaila. Stablo se sakrije (`4310–4316`).

Gumbi: `Prikaz` → `FILTERFORM`, `Proknjiži` → `PROKNJIZIFORM`, `Ispis` → `PRINTMASTER`. `PRINTMASTER` zove `IspisGRIDREPX` i izađe (`4951–4955`). Ne gleda `PAGEORIENTATION` (R41).

`PAGEORIENTATION="LANDSCAPE"` ipak se učita (`3458`) i vrijedi samo za Janus `IspisNovi` (`5930`). Zaglavlje desno: `u periodu: @@datod do @@datdo`. Podnožje lijevo `@@tkosmomi##@@mjesto`, desno `@@datdanas##Jupiter Software by SPIN`. `##` u pripremi zaglavlja postaje novi red.

`CHECKBOXVISIBLE="FALSE"` sakrije checkbox stupac (`2043–2046`). `TOTALROWVISIBLE` nema, suma ostaje (`2049–2053`). `POPUPMASTER` i `MASTERDBLCLICK` su prazni.

## Forme

`FILTERFORM` puni master ako ima `FILLEXPLORER="TRUE"` (`5445–5452`). `PROKNJIZIFORM` je dijalog proknjiženja. `ONSUBMIT` koji počinje sa `sp` lijepi parametre (`Form2.vb` 3378–3388).

## Praznine

- Ispis je `PRINTMASTER`, ne predložak iz `PAGEHEADER*` (R41).
- `LANDSCAPE` ne okrene taj ispis.
- Suma ostaje jer `TOTALROWVISIBLE` nije napisan.
