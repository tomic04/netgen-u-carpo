# Pregled Form1.vb

Izvor je `netgen/Form1.vb`. Designer se ne broji. Status: isto, razlika R-broj, nedostajalo-portano, još nedostaje. Test je [Z] red ili postojeći pokrivač.

Baza je `6a77c47`. Blokovi idu po pedeset funkcija.

## Blok 1, funkcije 1–50

Port je `Devex_26/Carpo.NetGen/Ulaz/Form1Ulaz.cs`. Test je `tests/Carpo.NetGen.Granule/Form1Blok.cs`.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 1 | SendMessage | 138 | `Form1Ulaz.PorukaAktivno` 0x7D1, `PorukaNeaktivno` 0x7D2, `PorukaZatvaranje` 0x7D0 | nedostajalo-portano | F1-06 |
| 2 | SetParent | 142 | prazan DllImport, nema tijela | isto | |
| 3 | SetWindowPos | 146 | prazan DllImport. Poziv u `Loader.ShellResize` je `Loader.vb`, ne Form1 | isto | |
| 4 | ShellResize | 211 | `Form1Ulaz.ResizeAkoJeMaksimiziran` | nedostajalo-portano | F1-06 |
| 5 | WndProc | 220 | `Form1Ulaz.TrebaPoravnati` za 0x7E i 0x2E0 | nedostajalo-portano | F1-06 |
| 6 | ClientToolbarGetButtons | 233 | `Form1Ulaz.AlatnaTraka` | nedostajalo-portano | F1-06 |
| 7 | ClientToolbarButtonClick | 251 | `Form1Ulaz.KlikAlatne` | nedostajalo-portano | F1-06 |
| 8 | Form1_Activated | 264 | poruka 0x7D1 | nedostajalo-portano | F1-06 |
| 9 | Form1_Deactivate | 279 | poruka 0x7D2 | nedostajalo-portano | F1-06 |
| 10 | Fiskalizacija | 286 | `FiskalTok` | razlika R109 | FiskalTok |
| 11 | FiskalizacijaNapojnica | 615 | `FiskalTok` tablica −1 | razlika R109 | FiskalTok |
| 12 | PoslovniProstor | 809 | zove `PoslovniProstor_COM` | nedostajalo-portano | F1-13 |
| 13 | PoslovniProstor_COM | 849 | `Form1Ulaz.Prostor`. Slanje samo na testni CIS | nedostajalo-portano, slanje R109 | F1-13 |
| 14 | DajCB | 1016 | `Form1Ulaz.DajCb` | nedostajalo-portano | F1-35 |
| 15 | DajSQL | 1057 | `Form1Ulaz.DajSql` | nedostajalo-portano | F1-25 |
| 16 | DajParametre | 1066 | `Form1Ulaz.DajParametre` | nedostajalo-portano | F1-25 |
| 17 | MYExnHandler | 1077 | `Form1Ulaz.TekstIznimke` | nedostajalo-portano | F1-25 |
| 18 | MYThreadHandler | 1083 | `Form1Ulaz.TekstIznimke` | nedostajalo-portano | F1-25 |
| 19 | Run8 | 1087 | `Loader.Run8` vraća null i ne učitava JupiterTranslator | razlika | Loader |
| 20 | Run | 1098 | `Loader.Run` | razlika | Loader |
| 21 | Form1_FormClosed | 1298 | `Form1Ulaz.Zatvoreno` vraća ONCLOSED | nedostajalo-portano | F1-25 |
| 22 | Form1_FormClosing | 1314 | poruka 0x7D0 | nedostajalo-portano | F1-06 |
| 23 | Form1_GotFocus | 1340 | zove `TimerOn` | još nedostaje | |
| 24 | Form1_Invalidated | 1344 | prazno tijelo | isto | |
| 25 | Form1_KeyDown | 1350 | `Form1Ulaz.Tipka` | nedostajalo-portano | F1-25 |
| 26 | NapuniRSLang | 1398 | `Form1Ulaz.JezikIz`. `Return` prije TotalGenLang | nedostajalo-portano | F1-31 |
| 27 | Jezici | 1484 | `Form1Ulaz.SqlMultilingual`, `SqlPismo` | nedostajalo-portano | F1-31 |
| 28 | Form1_Leave | 1497 | zove `TimerOff` | još nedostaje | |
| 29 | Form1_Load | 1500 | `Spoj`, `TabVidljiv`, `SqlOperatera`. Boja grida, kursor i cijeli redoslijed | još nedostaje | F1-31 |
| 30 | PronadjiOperatera | 1694 | `Form1Ulaz.SqlOperatera` | nedostajalo-portano | F1-42 |
| 31 | PokupiUlazneparametre | 1707 | `Form1Ulaz.Ulaz` | nedostajalo-portano | F1-31 |
| 32 | DodatnoFormatirajGridnakonRetriveStructure | 1777 | `Form1Ulaz.FormatStupca` | nedostajalo-portano | F1-32 |
| 33 | PokaziUpit | 1825 | `MasterGrid` pokriva punjenje, ne cijelo tijelo | još nedostaje | |
| 34 | PrevediSadrzajGrida2 | 2189 | `Form1Ulaz.PrevediStupce` | nedostajalo-portano | F1-35 |
| 35 | PokaziBrojStavki | 2232 | `Form1Ulaz.BrojStavki` | nedostajalo-portano | F1-35 |
| 36 | PokaziUpitDetalj | 2258 | `DetailGrid`. Izlaz dok se puni master nije portan | još nedostaje | |
| 37 | DMS_pOkazi_Upit | 2447 | `Form1Ulaz.DmsNaslov`. Vezanje grida | još nedostaje | F1-35 |
| 38 | SrediSQL | 2470 | `Grid/SqlTekst.cs` | isto | SqlTekst |
| 39 | GetLayoutDirectory | 2509 | `Form1Ulaz.MapaRasporeda` | nedostajalo-portano | F1-32 |
| 40 | PokupiParametreSaGrida | 2544 | `AssemblyPoziv.ParametriReda` za master. Dodatak s detail grida | još nedostaje | |
| 41 | PokupiParametreSaHijerarHijskogGrida | 2615 | `DetailGrid` | isto | DetailGrid |
| 42 | ssPermissionProvjeriRafalnuKomandu | 2665 | `Form1Ulaz.DozvolaKomande` | nedostajalo-portano | F1-42 |
| 43 | ssPermissionProcitaj | 2694 | `Form1Ulaz.SqlDozvole` | nedostajalo-portano | F1-42 |
| 44 | ssPermissionProvjeri | 2714 | | još nedostaje | |
| 45 | ProcitajXMLzaPodlogu | 3028 | `NetGenXmlReader`, `Podloga` | razlika R30, R56, R59, R74, R78 | postojeći |
| 46 | SetDefolte | 3945 | `ClipboardNg` | isto | ClipboardNg |
| 47 | DodatnoPostimajKontrolu | 4013 | `OnLoad2` | isto | OnLoad2 |
| 48 | NapuniMenuFolder | 4037 | `Form1Ulaz.IzbornikMape` | nedostajalo-portano | F1-32 |
| 49 | NapuniMenuBaza | 4088 | `Form1Ulaz.IzbornikBaza`, `SqlIzbornika`. Treća razina | još nedostaje | F1-32 |
| 50 | NapuniDrvo | 4127 | `Form1Ulaz.PrvoStablo`. Hod po folderu | još nedostaje | F1-31 |

Broj u bloku: 50. nedostajalo-portano 28. isto 7. razlika 5. još nedostaje 10. Zbroj 50.
