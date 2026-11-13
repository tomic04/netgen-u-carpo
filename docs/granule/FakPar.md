# FakPar — `cur` u tipu, `checkbox`, `vb.textbox`, prazan ALLOWEMPTY

XML: `definitions/netgen/BIOVEGA/FakPar.xml`. `ONLOAD="spFakPartneriUvjetiQuery"`. Na projektu je `GRUOPBYBOXVISIBLE=""`. Engine čita `GROUPBYBOXVISIBLE` (`Form1.vb` 3350). Tipfeler se ne ispravlja.

## TYPE koji slučajno pogodi

| Kako je napisano | Grana | Zašto |
|---|---|---|
| `SEARCH` | search `2140` | `InStr` je bez obzira na velika slova |
| `CMEditVB6.CurMEdit` | number `2071` | sadrži `cur`, ne sadrži raniji ključ |
| `checkbox` | check `1460` | sadrži `check`. Nema posebne grane `checkbox` |
| `VB.textbox` | text `1996` | sadrži `text` |
| `combo` | combo `2370` | |
| `label` | label `1296` | |

`Ucestalost`, `Valuta`, `Rabat`, `Povrat` su `CMEditVB6.CurMEdit`. `Rabat` i `Povrat` imaju `DECIMALS="2"`, `Ucestalost` i `Valuta` `DECIMALS="0"`. Nema `FORMATMASK` ni `DEFAULT`. Početni tekst je `0` (`2084`). Prazno u parametru ostaje `0` (`3601`). `cur` ne uključuje poseban novčani format: to je isti `NumerBox` kao `number`.

## Combo i search

`grupiranje` `ALLOWEMPTY=""`. Usporedba je `trim.lower == "true"` (`2403`). Prazno nije true, prazan red se ne ubaci. SQL ima `id` 1 i 2. Ako ništa nije odabrano, pokušaj `SelectedValue=1` (`3459`) odabere „Po mjestu isporuke“.

`parziroid` `ALLOWEMPTY="TRUE"` ubaci null red (`2402`–`2411`).

`partneriid` `SQL="partneri#partneriid#sifra%#naziv%#matbroj%"`. Hash oblik (`Form3.vb` 102). `sifra%` je LIKE samo zdesna, `naziv%` isto, `matbroj%` isto. `ALLOWEMPTY=""` na searchu se ne čita (`2140`).

`PoSkladistu` je `checkbox` → `mojCheck`. Parametar je `CheckState` (`3424`), ne `1`/`0` radio pravilo.

`Ugovor` je `VB.textbox` → `TeksT`. Prazan tekst ide kao `''` (`3655`), ne kao `null`.

## Rupe

- `CMEditVB6.CurMEdit` mora ostati broj. Ispravak u `number` nije potreban, ali odbacivanje tipa jer „nije number“ jest krivo (`2071`).
- `GRUOPBYBOXVISIBLE` se ne smije čitati kao `GROUPBYBOXVISIBLE`.
- Prazan `ALLOWEMPTY` nije `true` (`2403`).
- `checkbox` šalje `CheckState`, ne boolean caption.
