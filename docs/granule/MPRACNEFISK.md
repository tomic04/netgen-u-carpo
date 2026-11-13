# MPRACNEFISK — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/MPRACNEFISK.xml`. U izborniku je na 10 baza, uključujući Biovegu. Agronom kopija nije isti SHA-256. Mjerodavan je BIOVEGA. XML se ne mijenja. `spmpPregRacNG` se u testu ne šalje.

Izgled je Carpo (R59). Natpis, redoslijed, naredba, detail i tab ostaju.

Log ide u `NgDnevnik`.

## Podloga

Naslov je `Pregled nefiskaliziranih računa MP ` s razmakom na kraju (`3522–3528`).

`ONLOAD=":FORM1"` otvori parametre prije mastera (`3879–3880`, R18). Nema `TREE`.

`DETAILGRID = "spmpPregRacNG @action=2,@iD= @@id"`. Razmak ispred `=` ne mijenja ime atributa. Veza je `@@id` (R11).

`SETDEFAULTS="select dbo.fnTkoSmoMi() as tkosmomi"` prije mastera (`3862–3866`). `PAGEHEADERRIGHT="@@tkosmomi"`.

`REPORT="rptGotRNMMP.clsReport(@@ID)"`. `.clsreport` ide u `PrintRpt` (`5215`), nije XML forma (R43).

`CMD1` je `Parametri:FORM1`. `CMD2` i `CMD4`–`CMD6` su prazni, skriveni. `CMD3`:

`FISKALIZACIJA : FISKALIZACIJA(@@id)#:VRIJEMEJIR#REFRESHMASTER`

Lom je prva `:`. Natpis je `FISKALIZACIJA ` s razmakom. Naredba ima vodeći razmak. Segmenti (`4558–4570`):

1. `FISKALIZACIJA(@@id)` — ime forme lijevo od `(`, parametar unutra (R55).
2. `:VRIJEMEJIR` — vodeća `:` otvori formu (`4862`).
3. `REFRESHMASTER` — nije `REFRESHMASTERGRID`. Traži se forma tog imena. Nema je, 0 kontrola, `OKClick`, pa se zadnji master ipak ponovi (`4570`, `5519`, R04).

`MASTERDBLCLICK="PROMET"`. Forme `PROMET` u XML-u nema. 0 kontrola, dijalog se ne otvori (`5369–5370`).

`CHECKBOXVISIBLE` i `TOTALROWVISIBLE` nisu napisani. Checkbox stupac i suma ostaju uključeni (`2043–2053`). Group box ostaje uključen.

## `FORM1`

`FILLEXPLORER="TRUE"`. `ONSUBMIT="spmpPregRacNG @action=4, @operateriid = @@operateriid,"` postane master (R63). Počinje sa `sp`, parametri se lijepe.

`ONLOAD` čita `@@datumod`, `@@datumdo`, `@@trgovinaid`. Kontrole se zovu `datum`, `dadatum`, `danas`, `trgovinaid`. `@@datumod` nije ime kontrole. Ako `@@` ostane, upit se preskoči (`Class1.vb` 113–118).

`datum` je `date`. `ALLOWEMPTY` se na datumu ne čita. Kvačica je `SHOWCHECKBOX` (`Form2.vb` 2276, R62). `dadatum` i `danas` su `check`. Šalju 1 ili 0 (R29). `ALLOWEMPTY` na checku se ne čita. `trgovinaid` `ALLOWEMPTY="true"` ubaci prazan red (`2402–2411`).

Sva četiri `TABINDEX="1"`. Među jednakima vrijedi red dodavanja (`3028–3030`).

Gumbi: `Pokaži`, `Odustani`.

## Ostale forme

`VRIJEMEJIR` ima prazan `FILLEXPLORER`: `ONSUBMIT` se izvrši, pa se zadnji master ponovi (R63). `STAVKE` i `PLACANJE` su grid na dijalogu (`TYPE="grid"`), ne master (R53).

## Praznine

- `REFRESHMASTER` nije `REFRESHMASTERGRID`, ali prazna forma tog imena ipak osvježi master (R04).
- `PROMET` ne postoji. Dvostruki klik ne otvara dijalog.
- `ALLOWEMPTY` na datumu i checku ne gasi vrijednost (R62, R29).
- `ONLOAD` traži `@@datumod`, a kontrola je `datum`.
- `.clsReport` na projektu je ispis, ne forma (R43).
