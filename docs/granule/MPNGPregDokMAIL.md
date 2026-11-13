# MPNGPregDokMAIL — ASSEMBLYLOAD koji nije Carpo subforma

XML: `definitions/netgen/BIOVEGA/MPNGPregDokMAIL.xml`. Plan ga navodi kao najmanji `ASSEMBLYLOAD` (`docs/analiza/08-plan-implementacije.md`). `docs/funkcionalnosti/poziv-carpo.md` opisuje drugi poziv: `NetGenAdapter.clsCarpoKnjigaUra` i skriveni `INPUT` `CarpoForm`. Ova datoteka to nema.

## Što piše

`POPUPMASTER="Filter:FILTERFORM#Pošalji mail:ASSEMBLYLOAD(RepxToPdfToMail.StartProgram)"`.

`TREE33=…` nije `TREE`. `TrimAkoIma(..., "TREE")` ostane prazan. Stablo iz tog atributa se ne crta (`Form1.vb` 3387 čita `TREE`).

`ONLOAD=":FILTERFORM"` počinje s `:`, pa je poziv forme, ne SQL (`Form1.vb` 3879–3880).

`FILTERFORM` ima `DBCLIPBOARD="false"`. Ime sadrži `filter`, ali `false` spriječi uključivanje (`Form2.vb` 1102–1104).

Nema `INPUT` `CarpoForm`. Nema `NetGenAdapter`.

## Kako se zove

Stavka izbornika nema `~`. `ToolStripMenuItem1_Click` zato ide u `NovaGenerickaForma`, ne u `Rafalno` (`Form1.vb` 5743–5760`).

Grana `ASSEMBLYLOAD(` je `Form1.vb` 4584–4623:

- assembly je sve lijevo od zadnje točke: `RepxToPdfToMail` (`4599`)
- `CreateInstance` cijelog imena: `RepxToPdfToMail.StartProgram` (`4607`)
- `totalgen(id, operater, connection, zadnji SQL mastera, ParametriSaGrida)` (`4623`)
- konekcija bez `driver` dobije prefiks `Driver={SQL Server};` (`4616`–`4620`)
- `Load` ili `CreateInstance` padne: CB log i `Exit Sub` (`4600`–`4611`). Ostatak lanca staje
- povrat `totalgen` se ne čita

To nije dijalog `Form2.vb` 7424–7456 (tamo su oba zadnja argumenta `strParametriSvi`) i nije `:ASSEMBLYLOAD` (`6602`).

`FILTERFORM` kontrole (`datumod`, `datumdo`, combo s `ALLOWEMPTY` i `ONLOSTFOCUS="select 'refresh' as vrstadokid"`, search `vipartneri#…#!adresa`) slijede ista pravila kao `KntDospjeleStavke` i `MPInventura`.

## Rupe

- Ovo nije Carpo subforma. Nema `CarpoForm`, nema `clsCarpoKnjigaUra`. Adapter iz `poziv-carpo.md` ostaje na `KUPNG` / `KUPDostKN`, ne ovdje.
- `TREE33` se ne smije tretirati kao `TREE`.
- Popup bez `~` ide shell putem `4584`, s argumentima master SQL + parametri grida, ne `strParametriSvi`.
- Pad `Assembly.Load` prekida (`4600`), ne tihi nastavak.
