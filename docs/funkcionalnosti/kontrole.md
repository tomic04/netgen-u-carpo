# Kontrole dijaloga

`TYPE` se spušta na mala slova i traži se podstring, redom iz `Form2.vb` 1256–2707. Prva grana koja nađe podstring je `NgType.Odredi`. `datetiem` je datum jer sadrži `date` (`2232`). `cur` bilo gdje je isti broj kao `number` (`2071`). `picture` nije `picturebox`. `b_utton` nije gumb.

`DATAFIELD` se ne čita. Combo vrijednost je stupac 0, prikaz stupac 1 (`2430–2431`). `ALLOWEMPTY` je točno `true`, bez obzira na velika slova. Prazan string nije. Bez praznog reda prazan combo pokuša vrijednost `1` (`3459–3464`). Na multidropu se `ALLOWEMPTY` pročita i ne ubaci red (`2499–2504`).

Zavisni combo: SQL koji u koloni imena kontrole vrati `refresh` ponovo izvrši SQL tog comboa i, ako je `ALLOWEMPTY`, vrati prazan red (`5158–5176`).

Datum čita samo `DEFAULT`. `DEFALUT` ostaje nepročitan. Ključne riječi su `firstdayofyear`, `firstdayoflastyear`, `firstdayofmonth`, `firstdayoflastmonth` (`2254–2265`). `today` se samo spusti na mala slova. U siječnju `firstdayoflastmonth` da mjesec `0`. `SHOWCHECKBOX` bez `CHECKED` šalje `null` (`2277–2278`, `3566`). Carpo `allownull` (`ComponentDateEdit.cs` 89–93) se time ne uključuje.

Prazan broj je `0` (`3601`). Tražilica: `sp` i `select` ostaju SQL. `dbo.sp…` ide u `tablica#` (`Form3.vb` 99–102`). `%` je LIKE, `!` ispadne iz WHERE.

`TREE33`, `WUDTH` i `GRUOPBYBOXVISIBLE` se ne prepisuju u `TREE`, `WIDTH` i `GROUPBYBOXVISIBLE` (`Form1.vb` 3387, `3350`, `Form2.vb` 2949).

`ASSEMBLYLOAD` na podlozi (npr. `MPNGPregDokMAIL` `RepxToPdfToMail.StartProgram`) nije Carpo subforma. `totalgen` dobije id, operatera, konekciju, zadnji SQL mastera i parametre grida (`Form1.vb` 4599–4623`). Pad `Load` ili `CreateInstance` prekine lanac.

DevExpress svojstva koja NetGen postavlja eksplicitno, jer tvornički DX nije isti:

Stablo na dijalogu ne dodaje `:imeForme` na tag. `yearmonth` na dijalogu ne crta godine. `TREETRANSLATE` vrijedi samo na podlozi. Svaki čvor nosi sliku 0.

`WebBrowser` šalje `URL` velikim slovima u `Navigate`. Ako URL-a nema, adresa je kolona imena kontrole iz `ONLOAD`. Vrijednost `refresh` nije adresa.

`PictureBox` je `StretchImage`. `LOCATION` ide velikim slovima. Vrijednost koja počinje s `http` zove `Load`.

`NEXTFORM` nakon OK otvara tu formu. Dozvola pita ime prve forme, ne sljedeće.

Ime forme s `filter` pali DB clipboard, osim `DBCLIPBOARD=false`. Pad tog selecta ne stvara tablicu.

`CHECKBOXVISIBLE=FALSE` na gridu dijaloga sakrije stupac 1. `SKIPDEFAULTLOAD=TRUE` ne puni grid odmah.

| Grana | Širina | Ostalo |
|---|---|---|
| text | 300 | `TextEdit` |
| number i cur | 100 | `RightToLeft=Yes`, poravnanje `Near`, bez maske valute |
| search | 300 | tag `0` |
| date | 100 | format Short, `AllowNull=false` |
| combo | 300 | stupac 0 / stupac 1 |
| check | 100 | `AutoSize` |
| radio | 200 | |
| multidrop | 300 | `CheckedComboBoxEdit` |
| label | | `AutoSize`, prozirna pozadina, ne šalje parametar |
| frame i group | | `GroupControl`, prozirna pozadina |
| panel | | `PanelControl`, `DOCK` ostaje broj |
| statusstrip | | vidljiv, `TEXT1` velikim slovima |
