# MPUvjetiKase — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPUvjetiKase.xml`. Šest baza, Biovega. Natpis je `Uvjeti plaćanja na kasi`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":FILTERFORM"`. Ulaz je dijalog (`3879`, R18). `MASTERDBLCLICK="UPDATEFORM"`. `CHECKBOXVISIBLE="TRUE"` ostavlja stupac za označavanje (`2043–2046`).

`BUTTONS` ima Pronađi, Dodaj, Izmjeni, Obriši, Kopiraj, (de)Aktiviraj, Akcije i Tipovi.

## Dijalozi

`FILTERFORM` ima `FILLEXPLORER="TRUE"` i `ONSUBMIT="spMPUvjetiKase @action=1,"`. Potvrda zamijeni master (`5445–5452`).

`DELETEFORM` gumb Briši šalje `@action=5 … #Ok`. `AKTIVIRAJ` šalje `@action=6 … #Ok`. Obje mijenjaju uvjet. Na Jupiteru se ne šalju. `@action=1` i `@action=2` na `ONLOAD` dijaloga su prikaz.
