# ONCLICK na dijalogu — deset slučajeva

Port na gumbu čita natpis i stavi `DialogResult` (`Dijalog.cs` 116, 234–238, 316–319). `ONCLICK` se ne čita (`199`). Natpis `odustani`, `cancel` ili `close` postane Cancel. Svaki drugi natpis postane OK.

NetGen na `TYPE` koji sadrži `button` stavi `ONCLICK` u `Tag` (`Form2.vb` 2634) i klik vodi u `Glbl_ClickButton` (`2667`, `6221`). Prazan tag ili `help` otvori pomoć (`6224`). Inače ide `Rafalno`. Lanac se lomi na `#` (`6808`). Segment koji sadrži `@@` prvo prolazi zamjenu (`6812–6818`).

`OK` zove `Button1_Click` (`6888–6890`): `OKClick` postane true, `ONSUBMIT` se sredi, forma se zatvori (`3367–3396`). `OKSTAY` ne zatvori formu (`6884–6887`). `CANCEL`, `CLOSE` i `CLOSEPARENT` zovu `Button2_Click` (`7031–7037`): `OKClick` ostane false i `ONSUBMIT` se ne šalje (`3400–3402`). Usporedba je nakon `Trim` i `ToUpper`, pa `close`, `Ok` i `cancel` vrijede.

Segment koji počinje s `:` otvori tu formu (`6882–6883`, `4407`). Ime lijevo od `(` je forma, a zagrada ide u parametre (R55).

Ostali segment ide u `RefreshControls`, a odatle u `ExecSql` (`7509–7510`, `4556–4571`). Greška, uključujući `@@` koji ostane (`Class1.vb` 113–118), prekine ostatak lanca (`7511–7515`). Prazan segment se preskoči (`6821`).

Nakon što se dijalog zatvori s `OKClick` i praznim `NEXTFORM`, `FILLEXPLORER` odluči što roditelj radi s `ONSUBMIT` (`5429`):

| Vrijednost | Roditelj |
|---|---|
| `true` | `PokaziUpit` zamijeni master. `!!` se makne i parametri se ne lijepe (`5445–5452`). |
| `false` | `ExecSql`. Rezultat ne postane master (`5458–5466`). |
| `onlytree` | osvježi se samo stablo (`5475–5476`, R54). |
| prazno | `ExecSql`, zatim zadnji master (`5509–5519`). |

Ako prvih šest znakova `ONSUBMIT` nije u `INSERTDELETEUPDATESELECT`, parametri kontrola se lijepe na kraj (R14, `3378–3388`). `sp` i `exec` se lijepe. `insert`, `update`, `delete` i `select` ne.

| # | Gumb | Ulaz | Očekivano |
|---|---|---|---|
| 1 | `ServisPrava` `MAINFORM.ButtonNG27` | natpis `Završi`, `ONCLICK="CLOSE"` | `Button2`. `OKClick` ostane false. `ONSUBMIT` se ne šalje. Natpis `Završi` nije naredba. |
| 2 | `MPePotvrdeNarudzbe` `FILTERFORM.ButtonNG26` | natpis `Odustani`, `ONCLICK="cancel"`, `FILLEXPLORER="TRUE"` | `cancel` je isti `Button2` kao `CLOSE` (`7031`). `FILLEXPLORER` se ne gleda jer dijalog nije potvrđen. |
| 3 | `ProNotpL` `MAINFORM.button1` | natpis `Zatvori (F8)`, `ONCLICK="OK"`, nema `ONSUBMIT` ni `FILLEXPLORER` | Ovo je potvrda, ne zatvaranje. `ONSUBMIT` je prazan. Roditelj ponovo pošalje zadnji master (`5518–5519`). |
| 4 | `ZakljucivanjeTemNG` `MAINFORM.buttonOK` | natpis `Zaključi`, `ONCLICK=":Zakljucavanje"` | Otvori se forma `Zakljucavanje` (`6882`). `MAINFORM` ostane otvoren. Njegov `ONSUBMIT` se ne šalje. |
| 5 | `KntOPZ-STAT-1` `UPIT.buttonOK` | `ONCLICK="OK"`, `FILLEXPLORER="TRUE"`, `ONSUBMIT` počinje s `spKntOPZSTAT @action=1` | Potvrda zamijeni master tim upitom (`5445–5452`). `@@` se zamijeni, a parametri kontrola se lijepe još jednom (R14). |
| 6 | `KntBrutoBilancaNG` `FILTER_FORM.ok` | `ONCLICK` je `dbo.spKntBrutoBilanca_Pregled_Tree …#OK`, `FILLEXPLORER="ONLYTREE"`, `ONSUBMIT` prazan | Prvo se procedura izvrši dok je dijalog otvoren (`7509`). Zatim `OK`. Roditelj osvježi samo stablo (`5475`). Rezultat procedure ne postane master. |
| 7 | `BisnodeExport` `PARAM_FORM.buttonOK` | `ONCLICK="#OK"`, `FILLEXPLORER="true"`, `ONSUBMIT` je `spBisnodeExportQuery @action=@@ComboIzv, …` | Prazan segment se preskoči (`6821`). `OK` zamijeni master. `@@ComboIzv` i `@@Razdoblje` se zamijene, pa se parametri kontrola lijepe (R14). |
| 8 | `SklMinMaxDat` `MAINFORM.cmdDodaj` | `ONCLICK="spSktMinMaxDatum @action='promijeni_datume',@DatOd=@@datod,@DatDo=@@datdo #OK"` | `@@` se zamijeni, procedura se izvrši, zatim `OK`. `ONSUBMIT` i `FILLEXPLORER` su prazni, pa nema drugog upita iz submita. Greška procedure prekine lanac i `OK` se ne dogodi (`7511–7515`). |
| 9 | `KntPDVObrasciEU` `FILTER_FORM.ok` | šest segmenata, `FILLEXPLORER="TRUE"` | Redom: `spKntObrazacEU`, `spKntObrazacPPO`, `update dbo.KntPDVObrasciEU`, `spKntPdvObrazacDod`, `select case …`, `OK`. Svaki segment prije `OK` ide u `ExecSql`. Zatim submit zamijeni master. Greška bilo kojeg segmenta prekine ostatak, uključujući `OK`. |
| 10 | `MPKasaImport` `FILTERFORM.ok` | `ONCLICK="BLAGAJNAENABLE(@@mpkaseid)#OK"`, `FILLEXPLORER="TRUE"`, `ONSUBMIT` počinje s `spMPKasaCopy` | `@@mpkaseid` se zamijeni prije ključne riječi (`6812`). `BLAGAJNAENABLE` nije SQL (`6868`). Zatim `OK` pošalje `spMPKasaCopy` i `true` zamijeni master. |

Slučajevi 8, 9 i 10 pišu. NetGen ih šalje u `ExecSql`. `update` u slučaju 9 počinje riječju koju zaštita pisanja vidi. `spSktMinMaxDatum`, `spKntObrazacEU`, `spKntObrazacPPO`, `spKntPdvObrazacDod` i `spMPKasaCopy` počinju s `sp`, pa ta zaštita njih ne vidi. Na Jupiteru se nijedna od tih naredbi ne šalje.

Port koji gumb zatvori prema natpisu pada na svih deset. `Završi` bi potvrdio dijalog. `Zatvori (F8)` bi, ako se natpis pročita kao zatvaranje, odbio potvrdu. `Zaključi` bi zatvorio `MAINFORM` i ne bi otvorio `Zakljucavanje`.
