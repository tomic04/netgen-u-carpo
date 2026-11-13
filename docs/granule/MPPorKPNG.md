# MPPorKPNG — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPPorKPNG.xml` (windows-1250). Stavka `MPK-00`: `NetGenBot/granule/Biovega/MPPorKPNG.xml` ima isti SHA-256. Ponašanje je iz NetGen VB-a. `Carpo.NetGen` nije izvor istine.

Treća granula smoke reda iz `docs/analiza/08-plan-implementacije.md` (poslije OznakaKL i JeziciNG): datum, combo, broj, filter prije grida. Test ne šalje `spMPPoravnajKP` (procedura piše). Baza ostaje samo čitanje. `jupiter2022` se ne otvara.

Izgled je Carpo (R59). Stavke **izgled – Carpo** ne ruše QA. Natpis i redoslijed gumba, naredba, širina, skriveni stupac, sort, edit, checkbox stupac, raspored na dijalogu, tab, enable/visible i fokus ostaju.

Log SQL-a ide u `NgDnevnik` / Carpo logger (Ctrl+F11), ne u `CBLog` (`docs/funkcionalnosti/logging.md`).

Jedinica `/14` i VB zaokruživanje na parni su kao u `OznakaKL.md` (`Form2.vb` 58).

## Podloga

Nema `TREE`, `DETAILGRID`, `VERSION`, `CHECKBOXVISIBLE`, `TOTALROWVISIBLE`, `GROUPBYBOXVISIBLE`. `ONLOAD=":FILTERFORM"`.

| ID | Očekivano | NetGen |
|---|---|---|
| MPK-01 | Naslov je točno `>Poravnavanje KP u maloprodaji ` (vodeći `>`, razmak na kraju). `NAME` nije naslov. Nema ` v[`. | `Form1.vb` 3522–3528 |
| MPK-02 | `ONLOAD` otvara formu `FILTERFORM` prije mastera. Master SQL se ne šalje na ulazu. | `Form1.vb` 3879–3880 |
| MPK-03 | Nema stabla ni detaila. Prazan `TREE` i prazan combo sakriju drvo, splitter je 0. | `Form1.vb` 4310–4316, 2111–2113 |
| MPK-04 | `MASTERDBLCLICK=IZMJENIIZNOS`. | `Form1.vb` 3388–3389, 6935 |
| MPK-05 | Popup je `Filter:FILTERFORM#Izmjeni:IZMJENIIZNOS#Zaključi:ZAKLJUCI#Otključaj:OTKLJUCAJ`. Nema Liste ni ispisa. | `Form1.vb` 3620–3673, 5743 |
| MPK-06 | Gumbi 1–5: Parametri→`FILTERFORM`, Dodaj→`DODAJIZNOS`, Izmjeni→`IZMJENIIZNOS`, Zaključi→`ZAKLJUCI`, Otključaj→`OTKLJUCAJ`. `CMD6` je prazan, gumb skriven. | `Form1.vb` 3314–3330, 3827–3833 |
| MPK-07 | `CMD7CAPTION=Lista` i `CMD8CAPTION=Print dok.` nemaju `:`. `Substring` baci, tekst ostane prazan, gumb je skriven. Ne pokreću ništa. | `Form1.vb` 3320–3333, 3834–3835 |

`PERMISIO` nije u XML-u. Prije petlje je `True` (`3153`), pa select nad `ssPermission` ipak ide (`3414`, `2706`). Greška s `sspermission` se guta (`Class1.vb` 162–163).

### Janus / WinForms kad XML šuti

Atributa nema, `On Error Resume Next` ne prepiše dizajner. Ovo ostaje obavezno:

- `CHECKBOXVISIBLE` nije `"FALSE"` (reset na `""`, `3094`). Stupac 1, checkbox, je vidljiv (`2043–2047`).
- `TOTALROWVISIBLE` nije `"FALSE"`. Total red ostaje uključen (`2049–2053`). `CreateGridColumns` ga prvo upali (`Class1.vb` 1363).
- `GROUPBYBOXVISIBLE` se ne dodijeli. Dizajner ga ne dira, Janus početak je vidljiv group box (`Form1.designer.vb` 622–665, tvornički `True`).
- `FROZENCOLUMNS` se ne dodijeli. Ostaje 0.
- `AUTOMATICSORT` nije `false`, sort klikom ostaje (`3401–3404`).
- `EDITCOLUMNS` prazan: `EditType=NoEdit`, `EnterKeyBehavior=None` (`3419–3421`). `GRIDALLOWEDIT` se ne čita.
- Nema `DXGRID`: NetGen crta Janus. **izgled – Carpo:** kontrola je DevExpress. Row header, prazni retci, `HeaderLines=3`, highlight, font MS Sans Serif 8.25, obrub `None` su krom, ne pad.
- Gumbi na dnu, širina 79, razmak 8. **izgled – Carpo:** gumbi su u ribbonu. Natpis, redoslijed i naredba ostaju.

## Dijalozi

`CMD1` i `CMD2` na formi su cijeli natpis, bez loma na `:` (`Form2.vb` 1061–1066). `CMD3` prazan se ne vidi.

| Forma | `FILLEXPLORER` | `ONSUBMIT` počinje | Gumbi |
|---|---|---|---|
| `FILTERFORM` | `TRUE` | `spMPPoravnajKP @action = 1,` | Pokaži, Odustani |
| `DODAJIZNOS` | prazan | `spMPPoravnajKP @action=2, @id = @@id,` | Spremi, Odustani |
| `IZMJENIIZNOS` | prazan | `spMPPoravnajKP @action=4, @id = @@id,` | Spremi, Odustani |
| `ZAKLJUCI` | nema | `spMPPoravnajKP 5,@id=@@id,` | Zaključi, Odustani |
| `OTKLJUCAJ` | nema | `spMPPoravnajKP 6,@id=@@id,` | Otključaj, Odustani |

`sp` nije među `INSERTDELETEUPDATESELECT`. Zalijepljeni parametri ostanu (`3378–3388`). `@@id` i `@@operateriid` se zamijene prije slanja.

`FILLEXPLORER=true` taj SQL postane master (`5445–5452`). Prazan `FILLEXPLORER` izvrši SQL pa ponovi zadnji master (`5509–5519`). To nije isti ishod.

`ONLOAD` na `ZAKLJUCI`, `OTKLJUCAJ` i `IZMJENIIZNOS` ima `exec spRaiserror`. Greška na `ONLOAD` izađe iz `ProcitajXML` i forma se ne pokaže (`1199`, `4429`). Test tu naredbu ne šalje.

`BORDERSTYLE=3` se na formi ne čita. **izgled – Carpo.**

### Datum

`VALUE="DATE1"` / `DATE2` / `DATE` se ne čita. Čita se samo `DEFAULT` (`2251–2267`). `DEFAULT` nema, dodjela praznog stringa baci, ostane danas. `CHECKBOX="FALSE"` nije `SHOWCHECKBOX` (`2276`). Kvačica ostane uključena (`2275`), parametar je `'yyyyMMdd'` (`3530–3532`).

Širina 1500/14. Nema `TOP`/`LEFT`: automatski red, lijevo 140, label iz `CAPTION` (`2959–2986`).

### Combo

`ALLOWEMPTY="true"` i `"TRUE"` ubace prvi red `null` i prazan natpis (`2402–2411`). `TrgovinaId` na unosu nema `ALLOWEMPTY`: ostane `False`, prazan odabir pokuša `SelectedValue=1` (`3455–3468`). Stupac 0 je vrijednost, stupac 1 natpis. `@@operateriid` ide u SQL prije slanja.

`IZMJENIIZNOS.TrgovinaId` ima `ENABLED="False"`. Kontrola je ugašena, `TabStop` je false (`3046`).

### Broj

`mpiznos` krene od 0 (`2084`). `DECIMALS="4"` je `DecimalDigits` (`2086`). `VALUE` i `DEFAULT` nema. Prazan broj u parametru je `0`, ne `null`.

### Tab

`datumdok` u fajlu ima `TABINDEX` `1` i tab. `XmlDocument` tab u atributu pretvori u razmak (`"1 "`). VB dodjela u `Integer` razmak proguta, indeks je 1 (`3027–3030`). Grana −2 se ne pali. `napomena` ima čisti `1`. Oba su indeks 1; među jednakima vrijedi red dodavanja, datum pa napomena. Zatim trgovina 2, osoba 3, iznos 4. Spremi i Odustani dobiju `TabIndexCounter+1` i `+2` (`2775–2776`).

## Praznine za implementaciju

- `CMD7`/`CMD8` bez `:` ostaju skriveni. Ne smiju postati gumb `Lista` ili `Print dok.` (R60).
- `TABINDEX` `"1 "` (tab iz fajla, razmak nakon učitavanja) i dalje je indeks 1. Ne smije srušiti formu (R61).
- `VALUE=DATE1` na datumu nije početna vrijednost. Početak je danas. `CHECKBOX` na datumu ne gasi kvačicu (R62).
- `DECIMALS=4` je broj decimala, vrijednost i dalje kreće od 0.
- `FILLEXPLORER` prazan izvrši `ONSUBMIT` i osvježi stari master. `TRUE` zamijeni master tim SQL-om (R63).
- `ONSUBMIT` koji počinje sa `sp` i dalje lijepi `@ime=` kontrola.
- Datum u parametru je `'yyyyMMdd'`, u navodnicima, samo dok je datum uključen.
- Greška `spRaiserror` na `ONLOAD` ne otvara dijalog.
- Naslov zadržava vodeći `>`.
- Checkbox stupac je vidljiv, total je uključen, group box ostaje uključen, jer ti atributi nisu `"FALSE"` i nisu uopće napisani.
- SQL ide u Carpo logger, ne u `CBLog`.
