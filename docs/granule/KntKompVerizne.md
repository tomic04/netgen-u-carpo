# KntKompVerizne — stablo, detail, BUTTONS i preostali CMD

XML: `definitions/netgen/Agronom granule/KntKompVerizne.xml`. Izbornik: `asm:NetGenDll.loader:KntKompVerizne.xml`.

`ONLOAD="spKntKompVerizneQuery @action=0"`. `DETAILGRID` je `@id=@@id`.

`TREE="Verižne kompenzacije:#Danas:DANAS_FORM#sptreeMjesec:MJESEC_FORM#sptreeGodina:GODINA_FORM#sptreeVrijemeVerizne:FILTERODDO_FORM"`.

- `Verižne kompenzacije:` i `Danas:` su statički čvorovi (`Form1.vb` 4352–4355). Natpis statičkog čvora uvijek ide kroz `Translate` (`4353`).
- `sptreeMjesec`, `sptreeGodina`, `sptreeVrijemeVerizne` sadrže `tree`, pa su store (`4350–4351`), ne statički čvorovi.

Prazne forme `DANAS_FORM`, `MJESEC_FORM`, `GODINA_FORM`, `FILTERODDO_FORM` imaju `FILLEXPLORER="TRUE"`. Ne otvaraju prozor (`Form2.vb` 4430). `ONSUBMIT` postane master (`5445–5452`). `MJESEC_FORM` nema `@@node` u SQL-u; ostale ga imaju.

`SETDEFAULTS` je jedan select `template` iz `vrstadok` (`3862–3866`).

`BUTTONS` ima pet stavki i prepiše gumbe 1–5 (`Form1.vb` 3770–3788). `CMD6`–`CMD8` su `repx/rptKompVerizne…` i ostanu na gumbima 6–8, jer `BUTTONS` nema šesti segment (`3793–3796` dira `Button6` tek kao `j = 5`). `repx/` na shellu postane `repx:` (`5238–5239`).

`MASTERDBLCLICK="IZMJENI_FORM"` nema `:`. Ipak otvara formu (`4901–4906` skida `:` ako je ima).

`VERSION="1.21"` ide u naslov s dva razmaka.

## Rupe

- `sptree…` nije statički mjesec. Lijevo od `:` sadrži `tree`.
- Pet `BUTTONS` ne briše `CMD6`–`CMD8`. Repx gumbi ostaju.
- `MJESEC_FORM` ne šalje `@@node`.
