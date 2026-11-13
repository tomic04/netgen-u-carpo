# MPKaseOperNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPKaseOperNG.xml` (UTF-16). Rang 20, 9 baza, Biovega. XML se ne mijenja. `spMPKaseOper` se ne šalje.

Izgled je Carpo (R74).

## Podloga

`ONLOAD = "spMPKaseOper @action=1"`. Razmak oko `=` ne mijenja ime. Ne počinje s `:`, pa ide u master (`Form1.vb` 3879–3882).

`DETAILGRID = "spMPKaseOper @action=2, @trgovinaid=@@id"`. Veza je stupac `id` (R11).

`DETAILDBLCLICK="UPDATE_FORM"`. Dvostruki klik detaila zove tu formu (`Form1.vb` 3390, `7201–7202`, R75). `POPUPDETAIL="Izmjeni:UPDATEFORM"`.

`TREE=""`. Prazno stablo se sakrije (`4310–4316`).

`CMD2` je `&Izmjeni` → `UPDATE_FORM`, `CMD3` `&Brisanje` → `DELETEFORM`. `CMD7CAPTION="Lista"` nema `:`, gumb ostaje skriven (R60). Ostali `CMD` su prazni.

`DETCHECKBOXVISIBLE="FALSE"` sakrije checkbox na detailu (`2402`).

## Praznine

- Dvostruki klik detaila nije isti kao master (R75).
- `Lista` se ne vidi (R60).
- Detail SQL veže `@@id` kao `@trgovinaid` (R11).
