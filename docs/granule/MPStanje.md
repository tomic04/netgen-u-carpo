# MPStanje — očekivano ponašanje

Izbornik je `MPstanje.xml`. Datoteka u repou je `definitions/netgen/BIOVEGA/MPStanje.xml`. Rang 35, 7 baza, Biovega. Natpis je `Stanje MP`. XML se ne mijenja.

Izgled je Carpo (R74).

## Podloga

`BUTTONS=""`. `POPUPMASTER=""`. `ONLOAD` je prazan, master se ne puni na ulazu (`3879`). `SETDEFAULTS` je prazan. `MASTERDBLCLICK="KARTICA"`. Forma `KARTICA` postoji.

`COMMANDTIMEOUT="300"` (R65). `CHECKBOXVISIBLE="FALSE"`. `GROUPBYBOXVISIBLE="TRUE"`. Nema `TREE`. Stablo se sakrije (`4310–4316`).

## MAINFORM i INCLUDE

`MAINFORM` ima `INCLUDE="MPKarticaRobe.xml"`. Form2, prije čitanja forme, cijepa `INCLUDE` po `#`, traži datoteku u istom folderu i doda djecu tuđeg `/PROJECT` u trenutni dokument (`844–934`, R78). Prvo pokuša `SpinDMSUtil`, pa `XmlDocument.Load`. Datoteke nema u repou. `Load` padne pod `On Error Resume Next` (`706`, `913`). Forma ostane onakva kakva je napisana: vlastite kontrole (`group`, `grid`, `combo`, `search`, `date`, `number`).

`ONLOAD` forme složi `datumod` kao `yyyy0101`. `ONLOAD2` zove `spMPPermissionNG`.

## KARTICA

`SKIPDEFAULTLOAD="TRUE"` na gridu preskoči početni `ExecSql` (`Form2.vb` 1635–1646, R53). SQL ostane u `zadnjiSQL`. `CHECKBOXVISIBLE="TRUE"` na toj formi. `ONLOAD` čita `##temp50`. Natpis je `Kartica: '@@opis'`.

`OBNOVIKOL` i `OBNOVISTANJE` su posebne forme. `ONLOAD` uzima `year(@@datumdo)`.

## Praznine

- `MPKarticaRobe.xml` nije u repou. Spajanje `INCLUDE` se ne može provjeriti na sadržaju (R78).
- Grid kartice se ne puni čim se forma čita (R53).
