# Pregled Form1.vb

Izvor je `netgen/Form1.vb`. Designer se ne broji. Status: isto, razlika R-broj, nedostajalo-portano, još nedostaje. Test je [Z] red ili postojeći pokrivač.

QA na `7275629`. Prvi uzorak: 42, pravo 15, vraćeno 27. Preostalih 87: ni jedna nije na putu forme. `Form1Ulaz` i `Form1Srce` zove samo test, i to po povratnoj vrijednosti. Sve 87 vraćeno u još nedostaje. Ostaje portirano 15 iz prvog uzorka.

Baza je `6a77c47`. Blokovi idu po pedeset funkcija.

## Blok 1, funkcije 1–50

Port je `Devex_26/Carpo.NetGen/Ulaz/Form1Ulaz.cs`. Test je `tests/Carpo.NetGen.Granule/Form1Blok.cs`.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 1 | SendMessage | 138 | `Form1Ulaz.PorukaAktivno` 0x7D1, `PorukaNeaktivno` 0x7D2, `PorukaZatvaranje` 0x7D0 | još nedostaje | F1-06 |
| 2 | SetParent | 142 | prazan DllImport, nema tijela | isto | |
| 3 | SetWindowPos | 146 | prazan DllImport. Poziv u `Loader.ShellResize` je `Loader.vb`, ne Form1 | isto | |
| 4 | ShellResize | 211 | `Form1Ulaz.ResizeAkoJeMaksimiziran` | još nedostaje | F1-06 |
| 5 | WndProc | 220 | `Form1Ulaz.TrebaPoravnati` za 0x7E i 0x2E0 | nedostajalo-portano | F1-06 |
| 6 | ClientToolbarGetButtons | 233 | `Form1Ulaz.AlatnaTraka` | nedostajalo-portano | F1-06 |
| 7 | ClientToolbarButtonClick | 251 | `Form1Ulaz.KlikAlatne` | nedostajalo-portano | F1-06 |
| 8 | Form1_Activated | 264 | poruka 0x7D1 | još nedostaje | F1-06 |
| 9 | Form1_Deactivate | 279 | poruka 0x7D2 | još nedostaje | F1-06 |
| 10 | Fiskalizacija | 286 | `FiskalTok` | razlika R109 | FiskalTok |
| 11 | FiskalizacijaNapojnica | 615 | `FiskalTok` tablica −1 | razlika R109 | FiskalTok |
| 12 | PoslovniProstor | 809 | zove `PoslovniProstor_COM` | još nedostaje | F1-13 |
| 13 | PoslovniProstor_COM | 849 | `Form1Ulaz.Prostor`. Slanje samo na testni CIS | nedostajalo-portano, slanje R109 | F1-13 |
| 14 | DajCB | 1016 | `Form1Ulaz.DajCb` | još nedostaje | F1-35 |
| 15 | DajSQL | 1057 | `Form1Ulaz.DajSql` | još nedostaje | F1-25 |
| 16 | DajParametre | 1066 | `Form1Ulaz.DajParametre` | još nedostaje | F1-25 |
| 17 | MYExnHandler | 1077 | `Form1Ulaz.TekstIznimke` | još nedostaje | F1-25 |
| 18 | MYThreadHandler | 1083 | `Form1Ulaz.TekstIznimke` | još nedostaje | F1-25 |
| 19 | Run8 | 1087 | `Loader.Run8` ne učitava JupiterTranslator | razlika R114 | Loader |
| 20 | Run | 1098 | `Loader.Run` ne učitava JupiterTranslator | razlika R114 | Loader |
| 21 | Form1_FormClosed | 1298 | `Form1Ulaz.Zatvoreno` vraća ONCLOSED | još nedostaje | F1-25 |
| 22 | Form1_FormClosing | 1314 | poruka 0x7D0 | još nedostaje | F1-06 |
| 23 | Form1_GotFocus | 1340 | `Form1Ulaz.TimerUpaljen` | još nedostaje | F1-107 |
| 24 | Form1_Invalidated | 1344 | prazno tijelo | isto | |
| 25 | Form1_KeyDown | 1350 | `Form1Ulaz.Tipka` | nedostajalo-portano | F1-25 |
| 26 | NapuniRSLang | 1398 | `Form1Ulaz.JezikIz`. `Return` prije TotalGenLang | još nedostaje | F1-31 |
| 27 | Jezici | 1484 | `Form1Ulaz.SqlMultilingual`, `SqlPismo` | još nedostaje | F1-31 |
| 28 | Form1_Leave | 1497 | `Form1Ulaz.TimerUpaljen` prazan | još nedostaje | F1-107 |
| 29 | Form1_Load | 1500 | `Form1Srce.RedoslijedUcitavanja` | još nedostaje | F1-185 |
| 30 | PronadjiOperatera | 1694 | `Form1Ulaz.SqlOperatera` | još nedostaje | F1-42 |
| 31 | PokupiUlazneparametre | 1707 | `Form1Ulaz.Ulaz` | nedostajalo-portano | F1-31 |
| 32 | DodatnoFormatirajGridnakonRetriveStructure | 1777 | `Form1Ulaz.FormatStupca` | nedostajalo-portano | F1-32 |
| 33 | PokaziUpit | 1825 | `Form1Srce.Pokazi` | još nedostaje | F1-33 |
| 34 | PrevediSadrzajGrida2 | 2189 | `Form1Ulaz.PrevediStupce` | nedostajalo-portano | F1-35 |
| 35 | PokaziBrojStavki | 2232 | `Form1Ulaz.BrojStavki` | nedostajalo-portano | F1-35 |
| 36 | PokaziUpitDetalj | 2258 | `Form1Srce.Detalj` | još nedostaje | F1-33 |
| 37 | DMS_pOkazi_Upit | 2447 | `Form1Ulaz.DmsNaslov`, stupac 1 se skriva | još nedostaje | F1-35 |
| 38 | SrediSQL | 2470 | `Grid/SqlTekst.cs` | isto | SqlTekst |
| 39 | GetLayoutDirectory | 2509 | `Form1Ulaz.MapaRasporeda` | još nedostaje | F1-32 |
| 40 | PokupiParametreSaGrida | 2544 | `Form1Srce.SpojiParametre` | još nedostaje | F1-185 |
| 41 | PokupiParametreSaHijerarHijskogGrida | 2615 | `DetailGrid` | isto | DetailGrid |
| 42 | ssPermissionProvjeriRafalnuKomandu | 2665 | `Form1Ulaz.DozvolaKomande` | nedostajalo-portano | F1-42 |
| 43 | ssPermissionProcitaj | 2694 | `Form1Ulaz.SqlDozvole` | još nedostaje | F1-42 |
| 44 | ssPermissionProvjeri | 2714 | `Form1Srce.Dozvola` | još nedostaje | F1-44 |
| 45 | ProcitajXMLzaPodlogu | 3028 | `NetGenXmlReader`, `Podloga` | isto | postojeći |
| 46 | SetDefolte | 3945 | `ClipboardNg` | isto | ClipboardNg |
| 47 | DodatnoPostimajKontrolu | 4013 | `OnLoad2` | isto | OnLoad2 |
| 48 | NapuniMenuFolder | 4037 | `Form1Ulaz.IzbornikMape` | još nedostaje | F1-32 |
| 49 | NapuniMenuBaza | 4088 | `Form1Srce.TreciNivo` | još nedostaje | F1-185 |
| 50 | NapuniDrvo | 4127 | `Form1Ulaz.PrvoStablo`, `IzbornikMape` | još nedostaje | F1-31 |

Broj u bloku: 50. još nedostaje 28. Portirano ostaje 10.

Zeleni run bloka 1: `3ad0d66`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43366476116

## Blok 2, funkcije 51–100

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 51 | NapuniDrvoIzXMLA | 4217 | `Form1Ulaz.PutMeniXml`, `MeniIzXml` | nedostajalo-portano | F1-51 |
| 52 | AddNode | 4259 | `Form1Ulaz.MeniIzXml` | još nedostaje | F1-51 |
| 53 | NapuniDrvoVrijeme | 4296 | `Stablo.IzTeksta` | isto | sta-stablo |
| 54 | DazovaIzStore | 4435 | `Stablo.IzStore` | isto | Stablo |
| 55 | AddNodes | 4492 | `Stablo.DodajDjecu`, id `0` se preskače | isto | Stablo |
| 56 | DazovaVremenska | 4527 | `Stablo.GodinaMjesec` 2005–2009 | isto | Stablo |
| 57 | NovaGenerickaForma | 4543 | `GumbTok`, `Podloga.IzvrsiLanac` | isto | GumbTok |
| 58 | NovaGenerickaForma2 | 4925 | `Dijalog` | isto | STA-08 |
| 59 | NovaGenerickaForma1 | 5062 | `Podloga`, `Dijalog` | isto | LVO-04 |
| 60 | GetAllChildren | 5544 | `Form1Ulaz.NadjiPoTekstu` | još nedostaje | F1-51 |
| 61 | TraziNoduDrvetupoTagu | 5555 | `Form1Ulaz.NadjiPoTekstu` | još nedostaje | F1-51 |
| 62 | GetID | 5566 | `Form1Ulaz.Id` | isto | F1-51 |
| 63 | GetDXID | 5588 | `Form1Ulaz.DxId` | još nedostaje | F1-51 |
| 64 | GetDetID | 5605 | `Form1Ulaz.Id` | još nedostaje | F1-51 |
| 65 | GetDetDXID | 5625 | `Form1Ulaz.DxId` | još nedostaje | F1-51 |
| 66 | NovaApplikacija | 5638 | `Form1Srce.AplikacijaIzTaga` | još nedostaje | F1-185 |
| 67 | TreeView1_AfterSelect | 5647 | `Form1Srce.AplikacijaIzTaga` | još nedostaje | F1-185 |
| 68 | TreeView2_DoubleClick | 5656 | `Form1Ulaz.RastaviTag` | još nedostaje | F1-51 |
| 69 | Button1_Click | 5664 | `GumbTok` | isto | GumbTok |
| 70 | DataGridView1_DoubleClick | 5670 | `Podloga` `MasterDblClick` | isto | GranulaIzvrsenje |
| 71 | DataGridView1_MouseUp | 5674 | `Form1Srce.DesniKlik` | još nedostaje | F1-185 |
| 72 | ToolStripMenuItem1_Click | 5680 | `Form1Srce.IzvozIme` master | još nedostaje | F1-185 |
| 73 | ToolStripMenuItem2_Click | 5768 | isti izbornik na detailu | još nedostaje | F1-185 |
| 74 | Button2_Click | 5862 | `GumbTok` | isto | GumbTok |
| 75 | Button3_Click | 5868 | `GumbTok` | isto | GumbTok |
| 76 | Button4_Click | 5873 | `GumbTok` | isto | GumbTok |
| 77 | Button5_Click | 5878 | `GumbTok` | isto | GumbTok |
| 78 | Button6_Click | 5883 | `GumbTok` | isto | GumbTok |
| 79 | TreeView1_KeyDown | 5888 | Ctrl+F11, isto kao `Form1Ulaz.Tipka` | isto | F1-25 |
| 80 | TreeView2_KeyDown | 5900 | Ctrl+F11 | isto | F1-25 |
| 81 | IspisNovi | 5910 | `Form1Srce.DxPreskaceJanus` | još nedostaje | F1-185 |
| 82 | IspisNoviDet | 5953 | `Form1Srce.DxPreskaceJanus` | još nedostaje | F1-185 |
| 83 | IspisGRIDREPX | 5984 | `PrintBridge` master | isto | print-master |
| 84 | Button7_Click | 6009 | `IspisZaglavlje` | isto | ispis-zaglavlje |
| 85 | IspisGRIDREPXdet | 6067 | `PrintBridge` detail | isto | print-master |
| 86 | Button77_Click | 6091 | `PrintBridge` detail | isto | print-master |
| 87 | Button10_Click | 6140 | prazan poziv | isto | |
| 88 | ApperianceDialogToolStripMenuItem_Click | 6146 | Janus AppearanceDialog | razlika R113 | |
| 89 | FormatStylesToolStripMenuItem_Click | 6154 | Janus FormatStyles | razlika R113 | |
| 90 | TableViewToolStripMenuItem_Click | 6161 | Janus View.TableView | razlika R113 | |
| 91 | CardViewToolStripMenuItem_Click | 6169 | Janus CardView | razlika R113 | |
| 92 | SingleCardViewToolStripMenuItem_Click | 6177 | Janus SingleCardView | razlika R113 | |
| 93 | SaveGridToolStripMenuItem_Click | 6186 | zove `SaveGridLayout` | još nedostaje | F1-51 |
| 94 | LoadLayout | 6192 | `Form1Srce.Ucitaj` | još nedostaje | F1-140 |
| 95 | LoadLayout2 | 6228 | `Form1Srce.Ucitaj` | još nedostaje | F1-140 |
| 96 | SaveGridLayout | 6246 | `Form1Srce.Spremi` | nedostajalo-portano | F1-140 |
| 97 | LoadLayoutTEMPORARNI | 6312 | `Form1Srce.Ucitaj` | još nedostaje | F1-140 |
| 98 | SaveGridLayoutTEMPORARNI | 6328 | `Form1Srce.Spremi` | još nedostaje | F1-140 |
| 99 | DeleteGridLayout | 6344 | `Form1Srce.Obrisi` | još nedostaje | F1-140 |
| 100 | SaveGridLayoutDet | 6386 | `Form1Srce.Datoteka` detail | još nedostaje | F1-140 |

Broj u bloku: 50. još nedostaje 21. Portirano ostaje 2 (`NapuniDrvoIzXMLA`, `SaveGridLayout`). Janus dizajner ostaje R113.

Zeleni run bloka 2: `fbc3970`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43367408210

## Blok 3, funkcije 101–150

Prazno tijelo je isto. Portana su pravila koja su imala tijelo, a nisu bila u Carpu.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 101 | DeleteGridLayoutDet | 6410 | `Form1Srce.Obrisi` | još nedostaje | F1-140 |
| 102 | Button8_Click | 6432 | `Form1Ulaz.SljedeciRastvoreno` | još nedostaje | F1-107 |
| 103 | ResetGridLayoutToolStripMenuItem_Click | 6444 | `Form1Srce.Ucitaj` uz reset | još nedostaje | F1-140 |
| 104 | GridExportToolStripMenuItem_Click | 6453 | `Form1Srce.IzvozIme` | još nedostaje | F1-185 |
| 105 | GridEX1_CellEdited | 6466 | prazno | isto | |
| 106 | GridEX1_CellUpdated | 6471 | `Form1Ulaz.VrijednostCelije`. Petlja parametara je `PokupiParametreSaGrida` | još nedostaje | F1-107 |
| 107 | Rafalno | 6546 | `Form1Ulaz.SegmentRafala` i `Lanac` | nedostajalo-portano | F1-107 |
| 108 | GridEX1_Click | 6912 | `Form1Srce.SearchJeStupac` | još nedostaje | F1-185 |
| 109 | GridEX1_DoubleClick | 6935 | `Podloga` dvostruki klik | isto | |
| 110 | GridEX1_GroupsChanged | 6941 | `Form1Srce.GrupeSeSklope` | još nedostaje | F1-185 |
| 111 | ContextMenuStrip3_Click | 6952 | prazno | isto | |
| 112 | GridEX1_MouseUp | 6978 | `Form1Srce.DesniKlik` | još nedostaje | F1-185 |
| 113 | GridEX1_KeyDown | 7015 | `Form1Srce.EnterForma` | još nedostaje | F1-185 |
| 114 | GridEX2_CellUpdated | 7063 | ista vrijednost ćelije | još nedostaje | F1-107 |
| 115 | GridEX2_Click | 7130 | `Form1Srce.SearchJeStupac` | još nedostaje | F1-185 |
| 116 | GridEX2_KeyDown | 7145 | `Form1Ulaz.DajParametre` | još nedostaje | F1-25 |
| 117 | GridEX1_RecordUpdated | 7176 | `Form1Srce.KvacicaNakonZapisa` | još nedostaje | F1-185 |
| 118 | TreeView2_AfterSelect | 7182 | `Form1Ulaz.RastaviTag`, zatim `NovaGenerickaForma` | još nedostaje | F1-51 |
| 119 | TreeView2_NodeMouseClick | 7191 | prazno | isto | |
| 120 | GridEX2_DoubleClick | 7201 | `Form1Srce.AplikacijaIzTaga` detail | još nedostaje | F1-185 |
| 121 | GridEX2_MouseUp | 7206 | `Form1Srce.DesniKlik` | još nedostaje | F1-185 |
| 122 | ContextMenuStrip3_Closed | 7229 | prazno | isto | |
| 123 | ContextMenuStrip3_Closing | 7233 | `Form1Srce.ZatvaranjeOtkazano` | još nedostaje | F1-185 |
| 124 | ContextMenuStrip3_ItemClicked | 7241 | `Form1Srce.StupacNakonKlika` | još nedostaje | F1-185 |
| 125 | ContextMenuStrip4_Closing | 7277 | `Form1Srce.ZatvaranjeOtkazano` | još nedostaje | F1-185 |
| 126 | ContextMenuStrip4_ItemClicked | 7284 | `Form1Srce.StupacNakonKlika` | još nedostaje | F1-185 |
| 127 | MenuStrip1_ItemClicked | 7321 | `Form1Srce.AplikacijaIzTaga` | još nedostaje | F1-185 |
| 128 | Timer1_Disposed | 7332 | prazno | isto | |
| 129 | Timer1_Tick | 7336 | `Form1Srce.Pokazi` zadnji SQL | još nedostaje | F1-33 |
| 130 | ToolStripStatusLabel3_Click | 7340 | `Form1Srce.TimerKlik` | još nedostaje | F1-185 |
| 131 | TimerOff | 7348 | `Form1Ulaz.TimerUpaljen` prazan | još nedostaje | F1-107 |
| 132 | TimerOn | 7355 | `Form1Ulaz.TimerUpaljen` | još nedostaje | F1-107 |
| 133 | Button9_Click | 7396 | `Form1Srce.Obrisi` | još nedostaje | F1-140 |
| 134 | TreeView2_BeforeSelect | 7401 | `Form1Srce.BojaPrijeOdabira` | još nedostaje | F1-185 |
| 135 | TreeView2_LostFocus | 7407 | `Form1Srce.BojaBezFokusa` | još nedostaje | F1-185 |
| 136 | GridEX1_SelectionChanged | 7413 | `Form1Srce.Detalj` | još nedostaje | F1-33 |
| 137 | GridEXPrintDocument1_PrintPage | 7420 | `Form1Srce.Podnozje` | još nedostaje | F1-185 |
| 138 | ButtonZaDrvo_Click_1 | 7426 | `Form1Srce.StabloSirina` | još nedostaje | F1-185 |
| 139 | TreeView2_KeyDown1 | 7440 | `Form1Ulaz.DajParametre` | još nedostaje | F1-25 |
| 140 | CustomFormatKolona | 7451 | `Form1Srce.Kolona` | nedostajalo-portano | F1-140 |
| 141 | Button7_Click_1 | 7650 | `GumbTok` | isto | GumbTok |
| 142 | Button8_Click_1 | 7655 | `GumbTok` | isto | GumbTok |
| 143 | Button9_Click_1 | 7660 | `GumbTok` | isto | GumbTok |
| 144 | Button10_Click_1 | 7665 | `GumbTok` | isto | GumbTok |
| 145 | butonzarazgrupiranje_Click | 7670 | `Form1Ulaz.SljedeciRastvoreno` | još nedostaje | F1-107 |
| 146 | Form1_LocationChanged | 7684 | prazno | isto | |
| 147 | Form1_LostFocus | 7688 | `Form1Ulaz.TimerUpaljen` prazan | još nedostaje | F1-107 |
| 148 | Form1_Resize | 7692 | `Form1Srce.ResizePaliTimer` | još nedostaje | F1-185 |
| 149 | Form1_ResizeEnd | 7703 | prazno | isto | |
| 150 | Form1_SizeChanged | 7709 | nema tijela osim On Error | isto |  |

Broj u bloku: 50. još nedostaje 35. Portirano ostaje `Rafalno` i `CustomFormatKolona`.

## Blok 4, funkcije 151–194

Zadnji blok. Ukupno funkcija u Form1.vb je 194.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 151 | ComboBox1_Click | 7714 | prazno | isto | |
| 152 | ComboBox1_SelectedIndexChanged | 7718 | prazno | isto | |
| 153 | TabPage2_Click | 7722 | prazno | isto | |
| 154 | TabPage2_Resize | 7726 | `Form1Srce.ResizePaliTimer` | još nedostaje | F1-185 |
| 155 | ComboBox1_SelectedValueChanged | 7730 | `Stablo` iz odabrane vrijednosti | isto | Stablo |
| 156 | Rafalna_Priprema | 7737 | `Form1Srce.Rafalna` | još nedostaje | F1-33 |
| 157 | ButtonHELP_Click | 7777 | zove `DajPokaziHelp` | još nedostaje | F1-158 |
| 158 | DajPokaziHelp | 7781 | `Form1Ulaz.ArgumentiPomoci`. `Str` ima vodeći razmak | nedostajalo-portano | F1-158 |
| 159 | GridEX1_EditingCell | 7803 | `Form1Ulaz.VrijednostCelije` | još nedostaje | F1-107 |
| 160 | GridEX1_FormattingRow | 7875 | `Form1Srce.Red` | još nedostaje | F1-185 |
| 161 | OriginalToolStripMenuItem_Click | 8006 | `Form1Ulaz.JezikIzbornika` jezik prazan, multilingual 0 | još nedostaje | F1-158 |
| 162 | MKToolStripMenuItem_Click | 8012 | jezik MK, multilingual 2 | još nedostaje | F1-158 |
| 163 | ENToolStripMenuItem_Click | 8019 | jezik EN, multilingual 2 | još nedostaje | F1-158 |
| 164 | GridEX2_FormattingRow | 8026 | `Form1Srce.Red` | još nedostaje | F1-185 |
| 165 | ContextMenuStrip2_Opening | 8098 | prazno | isto | |
| 166 | ContextMenuStrip3_LocationChanged | 8102 | prazno | isto | |
| 167 | ContextMenuStrip3_Opening | 8106 | prazno | isto | |
| 168 | ContextMenuStrip1_Opening | 8110 | prazno | isto | |
| 169 | Button11_Click | 8114 | `Form1Srce.Obrisi` master i detail | još nedostaje | F1-140 |
| 170 | GridEX1_KeyPress | 8120 | prazno | isto | |
| 171 | TreeView2_DragDrop22 | 8126 | `Form1Srce.TekstPovlacenja` | još nedostaje | F1-185 |
| 172 | GridEX1_MouseDown | 8131 | prazno | isto | |
| 173 | TreeView2_ItemDrag | 8135 | prazno | isto | |
| 174 | TreeView2_DragDrop | 8140 | `strTREEDragDrop` ide u SQL | još nedostaje | F1-33 |
| 175 | TreeView2_DragOver | 8180 | `Form1Ulaz.SmijeSpustiti` | još nedostaje | F1-158 |
| 176 | GridView1_CellValueChanged | 8223 | stupac rcb555 se preskače, vrijednost je `VrijednostCelije` | još nedostaje | F1-107 |
| 177 | GridView1_FocusedRowChanged | 8306 | `Form1Srce.Detalj` | još nedostaje | F1-33 |
| 178 | New | 8318 | `Form1Srce.NoviProzor` | još nedostaje | F1-185 |
| 179 | GridView1_KeyDown | 8338 | Enter na stupac s tagom 1 | još nedostaje | F1-185 |
| 180 | GridView1_MouseUp | 8385 | `Form1Srce.DesniKlik` | još nedostaje | F1-185 |
| 181 | GridView1_ShownEditor | 8412 | editor označi sav tekst | još nedostaje | F1-185 |
| 182 | Timer2_Tick | 8424 | jednom timer, pa se gasi | još nedostaje | F1-107 |
| 183 | GridEX3_MouseUp | 8429 | `Form1Ulaz.DmsIzbornik` | još nedostaje | F1-158 |
| 184 | DMS_odustani | 8464 | `Form1Ulaz.SqlDmsOdustani` | još nedostaje | F1-158 |
| 185 | DMS_preuzmizapotpis | 8484 | `Form1Srce.UcitajDms`, `DocumentCheckOut` | još nedostaje | F1-185 |
| 186 | DMS_vratipotpisano | 8528 | `DocumentCheckIn` | još nedostaje | F1-185 |
| 187 | DMS_Obrisi | 8574 | `Form1Srce.SqlDmsBrisi` | još nedostaje | F1-185 |
| 188 | DMS_SpremiNaDisk | 8599 | `SaveDocuments` | još nedostaje | F1-185 |
| 189 | DMS_Pogledaj | 8639 | `OpenDocument` | još nedostaje | F1-185 |
| 190 | TabControl1_DrawItem | 8678 | prazno | isto | |
| 191 | GridEX3_RowDoubleClick | 8702 | zove `DMS_Pogledaj` | još nedostaje | F1-185 |
| 192 | ContextMenuStrip4_Opening | 8706 | prazno | isto | |
| 193 | ParentV_Resize | 8710 | `Form1Srce.Roditelj` | još nedostaje | F1-185 |
| 194 | ParentForm_LocationChanged | 8719 | `Form1Srce.Roditelj` | još nedostaje | F1-185 |

Broj u bloku: 44. još nedostaje 30. Portirano ostaje `DajPokaziHelp`.

Ukupno Form1: 194. još nedostaje 114. nedostajalo-portano 15. isto 56. razlika 9.

Razlike koje se ne mogu izjednačiti: R109 fiskalni SOAP ostaje na testnom CIS-u (10, 11). R114 `Run`/`Run8` ne učitavaju `JupiterTranslator10` jer se Jupiter ne dira (19, 20). R113 Janus `AppearanceDialog`, `FormatStyles`, `TableView`, `CardView` i `SingleCardView` nemaju DevExpress tip (88–92). Poslovni prostor i dalje ne šalje produkcijski CIS (R109, R111).
