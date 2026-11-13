# KntBlaPromet — stablo blagajne, detail, ispis preko `@@template`

XML: `definitions/netgen/Agronom granule/KntBlaPromet.xml`. Izbornik: `asm:NetGenDLL.loader:KntBlaPromet.xml`.

Atribut je napisan `DETAILGRID = "…"`, s razmakom. Ime je i dalje `DETAILGRID`. SQL je `spBLPrometDetQueryNG @Action = 0,@ID =@@id`.

`ONLOAD` je master `spBLPrometGlaQueryNG @action=0`. `TREE` je statički `Blagajne:` plus `spBlagajnePrometTreeNG @operateriid=@@operateriid:MJESEC_FORM`. Lijevo sadrži `tree`, store grana (`Form1.vb` 4350–4351).

`MJESEC_FORM` je prazna, `FILLEXPLORER="TRUE"`, `ONSUBMIT` šalje `@id='@@node'` u navodnicima. `@@node` je tekst čvora prije `:`.

`SETDEFAULTS="select 1 as blagajneid"` jedan je segment.

`BUTTONS` prepiše prvih šest gumba. `CMD6CAPTION=""` ne doda sedmi.

`MASTERCOLUMNS CLEARCOLUMS="TRUE"` sakrije sve kolone pa vrati navedene (`Form1.vb` 7459–7462). `AGREGATEFUNCTION="2"` na iznosima jest jedno G i nije prazno, pa se dodijeli (`7506`). `"0"` također nije prazno.

## Ispis

`ISPISDOK_FORM` ima kontrole, visinu 1. `ONLOAD` vrati `template` i `@@BLPrometGlaId`. `ONLOADED="click(Ok)"` ne ide u granu nula kontrola (`Form2.vb` 2798), ali `Form2_Load` ga zove (`5796`).

`CLICK(Ok)` traži kontrolu tog imena i klikne je (`7151–7155`). Gumb `ok` ima `ONCLICK="close#:REPX/@@template (@id=@@BLPrometGlaId)"`. `@@template` se zamijeni prije `PrintRpt`. `repx/` na dijalogu postane `repx:/` (`4296–4297`). Ime izvještaja je iz baze, ne iz XML-a.

## Rupe

- `@@node` u masteru blagajne je u navodnicima. Broj bez navodnika promijeni SQL.
- `CLICK(Ok)` mora naći gumb `ok`. Samo `form close` ne pokrene REPX.
- `CLEARCOLUMS` i agregat `"2"` ostaju. Dva G se i dalje ne čitaju.
