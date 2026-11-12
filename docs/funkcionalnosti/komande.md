# Komande

Lanac je `Carpo.NetGen.Komande.Lanac`. Tilda u tekstu gumba ostaje tilda u parseru. `~` postaje `#` tek kad `NovaGenerickaForma` krene (`Form1.vb` 4558).

| Segment | Dijalog `Form2.Rafalno` | Podloga `Form1.Rafalno` |
|---|---|---|
| prazan (`##`) | ne radi ništa i ne preskače sljedeći (`6821`) | isto |
| `OK` | zatvara formu (`6888`) | |
| `OKSTAY` | ne zatvara (`6884`) | |
| `CLOSE` / `CANCEL` | `Button2` (`7033`) | |
| `CLOSEPARENT` | `bCloseParent` pa `Button2` (`7035`) | |
| `REFRESHMASTERGRID` | ponovi zadnji master SQL (`Form1.vb` 4570) | isto |
| `REFRESHMASTER` | nije ta riječ, grid se ne osvježava | isto |
| `FOREACHSELECTED` | vrti čekirane i pojede sljedeći segment (`7040`) | poziv je komentar, `i = i + 1` je živ (`6843`) |
| `FOREACHBLUEROW` | plavi redovi (`7043`) | isto, petlja se ne vrti |
| `FOREACHROW` | svi redovi (`7046`) | isto |
| SQL | `ExecSql`. Poruka bez `#OK` i `+OK` prekida `Exit For` (`7511`, `Class1.vb` 187) | |
| `+focus(ime)` | upiše fokus. Samo to nije `+OK`, greška ostaje (`202`) | |

Gumbi podloge zovu `NovaGenerickaForma`, ne `Rafalno` (`5667`).

| `ONSUBMIT` koji ne počinje s `INSERT`/`DELETE`/`UPDATE`/`SELECT` (prvih 6 slova) dobije razmak i niz `@ime=` iako već ima parametre (`Form2.vb` 3378, `Form1.vb` 5514). `spKultureNGAzur @Action=0,` je taj slučaj. `!!` na kraju store makne lijepljenje (`5449`). U captionu `!!` je novi red (`Form2.vb` 2943).

`MULTISELECT` prije forme briše `TotalGenMultiSelectTemp` dvaput (`suser_sname()` i `operateriid`) i inserta čekirane id-eve s `host_name()` (`4934–5018`). `MULTIBLUE` uzima plave retke (`5022`). Test ne šalje te naredbe u bazu.

Log ide kroz `NgDnevnik`.
