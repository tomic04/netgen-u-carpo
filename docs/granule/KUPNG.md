# KUPNG — lanac naredbi, multi-select, FOREACH

XML: `definitions/netgen/Agronom granule/KUPNG.xml`. Ovdje su naredbe podloge i jedan dijalog s radiom i `FOREACH`. Ostala polja forme nisu predmet ove liste.

## Podloga

`NovaGenerickaForma` prvo zamijeni `~` u `#` (`Form1.vb` 4558), pa cijepa na `#` (`4561`).

`CMD2CAPTION="Unesi:INSERTFORM~REFRESHMASTERGRID"` postane dva segmenta:

1. `INSERTFORM` otvara formu.
2. `REFRESHMASTERGRID` ponovo šalje **zadnji** SQL mastera, ne izvorni predložak (`4570-4577`).

`CMD5` počinje s `MULTISELECT`. `CMD6` s `MULTIBLUE`.

Prije forme (`4932-4936`):

```
delete TotalGenMultiSelectTemp where susersname = suser_sname()
delete TotalGenMultiSelectTemp where operateriid = @@operateriid
```

Zatim insert (`5005`, `5016-5018`):

```
insert into TotalGenMultiSelectTemp (id, hostname, susersname, operateriid)
select <GetID()>, host_name(), suser_sname(), <operateriid>
```

`MULTISELECT` ubacuje **čekirane** retke (`5015-5018`). Jedan ili nula čekiranih ubaci samo trenutni `GetID` (`5003-5006`).

`MULTIBLUE` ubacuje plave, `SelectedItems`, ne čekirane (`5022-5032`).

Na podlozi `FOREACHROW` / `FOREACHSELECTED` / `FOREACHBLUEROW` u `Form1.Rafalno` su komentar. Živ je samo `i = i + 1`, pa se sljedeći segment pojede (`6843-6850`). Gumbi podloge uopće ne ulaze u taj `Rafalno`. Idu u `NovaGenerickaForma` (`5667`), koja tu riječ ne grana.

## Dijalog

`FOREACHSELECTED(GridOsobe)#sljedeci` (`Form2.vb` 7040-7042) zove `ForEachSelected` (`8337`). Traži grid tog imena, uzme `GetCheckedRows`, za svaki red pomakne i izvrši **sljedeći** segment (`8350-8353`). Indeks petlje se poveća da se segment ne izvrši još jednom (`7042`).

`FOREACHROW(gPred)` isto, ali `GetDataRows` — svi redovi (`8310-8327`, `7046-7048`).

`FOREACHBLUEROW` uzme `SelectedItems` od 0 naviše (`8363-8394`).

Radio (`rbLik`, `rbPred`) ide kao `@ime=1` ili `0` (`3432-3435`).

`REFRESHCONTROL(ime)` je `select 'refresh' as ime` (`6911-6915`).

`OK` zatvara formu (`6888-6890`). `OKSTAY` postavi `NeZelimClosatFormu` pa je `Button1` ne zatvori (`6884-6887`). `CANCEL`, `CLOSE` i `CLOSEPARENT` zovu `Button2` (`7031-7037`). `CLOSEPARENT` prije toga stavi `bCloseParent = True`.

## Rupe

- `~` i `#` su isti razdjelnik, ali tek u `NovaGenerickaForma` (`4558`). Parser CMD ne smije ostaviti tildu kao dio imena forme.
- `REFRESHMASTER` nije `REFRESHMASTERGRID` (`4570`). Samo potonji ponovi zadnji SQL.
- `TotalGenMultiSelectTemp` se briše dvaput (suser i operater) i puni s `host_name()` (`4934-5018`). Dva prozora istog operatera dijele retke.
- `MULTISELECT` i `MULTIBLUE` ne pune isti skup redova (`5015` naspram `5022`).
- `FOREACH*` na dijalogu vrti petlju (`Form2.vb` 7040). Na podlozi ne (`Form1.vb` 6843-6850).
- `OKSTAY` ostavlja formu otvorenu (`6884`). `CLOSEPARENT` nije običan `CLOSE` (`7035-7036`).
