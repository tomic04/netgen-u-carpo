# ZakljucivanjeTemNG — očekivano ponašanje

Izvor: `definitions/netgen/Agronom granule/ZakljucivanjeTemNG.xml`. Rang 51, 6 baza. Natpis je `Zaključivanje temeljnica`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD=":MAINFORM"` otvara dijalog (`3879–3880`, R18). Nema gumba na projektu. `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="FALSE"`. `DETCHECKBOXVISIBLE="FALSE"`. Nema `TREE`.

## MAINFORM

Natpis je `Odredite parametre za zaključivanje ili poništavanje zaključka temeljnica`. `MAXIMIZEBOX="TRUE"` upali povećanje (`Form2.vb` 1031).

Gumb `Zaključi` ima `ONCLICK=":Zakljucavanje"`. Gumb `Poništi zaključke` ima `ONCLICK=":Ponistavanje"`. `Odustani` je `CLOSE`.

`Zakljucavanje` `ONSUBMIT` počinje sa `spKntZakljucivanjeTem @action=1`. `Ponistavanje` je `@action=2`. To su naredbe koje mijenjaju podatke. Na jupiter2019 i jupiter2022 se ne smiju izvršiti. Zaštita koja gleda samo prvu riječ `insert`/`update`/`delete` ovaj `sp` propušta.

## Praznine

- Otvaranje smije pokazati `MAINFORM`. Klik na Zaključi ili Poništi na klijentskom serveru ne smije poslati `spKntZakljucivanjeTem`.
