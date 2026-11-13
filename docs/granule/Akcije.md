# Akcije — očekivano ponašanje

Izvor: `definitions/netgen/BIOVEGA/Akcije.xml`. Rang 45, 6 baza. Natpis je `Akcije`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`ONLOAD` je prazan. Master se na ulazu ne puni (`3879`). Puni ga čvor čija forma ima `FILLEXPLORER`.

`TREE` počinje s `Osvježi drvo:REFRESHTREEFORM`, pa `spAkcije_Tree` (store, R01). `COMMANDTIMEOUT="600"` (R65). `CHECKBOXVISIBLE="FALSE"`. `TOTALROWVISIBLE="false"` je isto što i `FALSE` (`ToUpper`, `2049–2050`).

`BUTTONS`: `Akcije:AKCIJE#Operateri:OPERATERI`.

`POPUPMASTER="Promjene:PRIHVATI(@akcijeid=@@akcijeid)#-:#Obriši operatera:OPER_BRISI(@akcijeopid=@@akcijeopid)"`. Segment `-:` ima natpis `-` i prazan tag (`3564–3566`, R85). Imena s zagradom režu se na `(` (R55).

## Praznine

- Nema početnog master SQL-a. Prazan grid nije „nema podataka“.
- Crtica u popup-u ostaje stavka (R85).
