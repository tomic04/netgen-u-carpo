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
| 17 | MYExnHandler | 1077 | MYExnHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 18 | MYThreadHandler | 1083 | MYThreadHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 19 | Run8 | 1087 | `Loader.Run8` zove `Prevoditelj.Ucitaj` | nedostajalo-portano | F1-114 |
| 20 | Run | 1098 | `Prevoditelj.Ucitaj` `JupiterTranslator10` | nedostajalo-portano | F1-114 |
| 21 | Form1_FormClosed | 1298 | `Form1Ulaz.Zatvoreno` vraća ONCLOSED | nedostajalo-portano | F1-25 |
| 22 | Form1_FormClosing | 1314 | poruka 0x7D0 | nedostajalo-portano | F1-06 |
| 23 | Form1_GotFocus | 1340 | `Form1Ulaz.TimerUpaljen` | nedostajalo-portano | F1-107 |
| 24 | Form1_Invalidated | 1344 | prazno tijelo | isto | |
| 25 | Form1_KeyDown | 1350 | `Form1Ulaz.Tipka` | nedostajalo-portano | F1-25 |
| 26 | NapuniRSLang | 1398 | `Form1Ulaz.JezikIz`. `Return` prije TotalGenLang | nedostajalo-portano | F1-31 |
| 27 | Jezici | 1484 | `Form1Ulaz.SqlMultilingual`, `SqlPismo` | nedostajalo-portano | F1-31 |
| 28 | Form1_Leave | 1497 | `Form1Ulaz.TimerUpaljen` prazan | nedostajalo-portano | F1-107 |
| 29 | Form1_Load | 1500 | Form1_Load nije izvršni put. Stalni redoslijed je maknut. | još nedostaje | |
| 30 | PronadjiOperatera | 1694 | `Form1Ulaz.SqlOperatera` | nedostajalo-portano | F1-42 |
| 31 | PokupiUlazneparametre | 1707 | `Form1Ulaz.Ulaz` | nedostajalo-portano | F1-31 |
| 32 | DodatnoFormatirajGridnakonRetriveStructure | 1777 | `Form1Ulaz.FormatStupca` | nedostajalo-portano | F1-32 |
| 33 | PokaziUpit | 1825 | MasterGrid.IzvrsiCitanje zna poslati SQL. Timer, novi id, grupe i DX izlaz iz PokaziUpit nisu u tom putu. | još nedostaje | |
| 34 | PrevediSadrzajGrida2 | 2189 | `Form1Ulaz.PrevediStupce` | nedostajalo-portano | F1-35 |
| 35 | PokaziBrojStavki | 2232 | `Form1Ulaz.BrojStavki` | nedostajalo-portano | F1-35 |
| 36 | PokaziUpitDetalj | 2258 | PokaziUpitDetalj ne izlazi dok se puni master i ne puni detail grid. | još nedostaje | |
| 37 | DMS_pOkazi_Upit | 2447 | `Form1Ulaz.DmsNaslov`, stupac 1 se skriva | nedostajalo-portano | F1-35 |
| 38 | SrediSQL | 2470 | `Grid/SqlTekst.cs` | isto | SqlTekst |
| 39 | GetLayoutDirectory | 2509 | `Form1Ulaz.MapaRasporeda` | nedostajalo-portano | F1-32 |
| 40 | PokupiParametreSaGrida | 2544 | PokupiParametreSaGrida ne čita stupce retka. Spajanje stringova je maknuto. | još nedostaje | |
| 41 | PokupiParametreSaHijerarHijskogGrida | 2615 | `DetailGrid` | isto | DetailGrid |
| 42 | ssPermissionProvjeriRafalnuKomandu | 2665 | `Form1Ulaz.DozvolaKomande` | nedostajalo-portano | F1-42 |
| 43 | ssPermissionProcitaj | 2694 | `Form1Ulaz.SqlDozvole` | nedostajalo-portano | F1-42 |
| 44 | ssPermissionProvjeri | 2714 | ssPermissionProvjeri ne čita trsPermisio ni Button.Text. | još nedostaje | |
| 45 | ProcitajXMLzaPodlogu | 3028 | `NetGenXmlReader`, `Podloga` | isto | postojeći |
| 46 | SetDefolte | 3945 | `ClipboardNg` | isto | ClipboardNg |
| 47 | DodatnoPostimajKontrolu | 4013 | `OnLoad2` | isto | OnLoad2 |
| 48 | NapuniMenuFolder | 4037 | `Form1Ulaz.IzbornikMape` | nedostajalo-portano | F1-32 |
| 49 | NapuniMenuBaza | 4088 | `Form1Srce.TreciNivo` | nedostajalo-portano | F1-185 |
| 50 | NapuniDrvo | 4127 | `Form1Ulaz.PrvoStablo`, `IzbornikMape` | nedostajalo-portano | F1-31 |

Broj u bloku 1: lažni portovi su vraćeni u još nedostaje.

Zeleni run bloka 1: `3ad0d66`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43366476116

## Blok 2, funkcije 51–100

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 51 | NapuniDrvoIzXMLA | 4217 | `Form1Ulaz.PutMeniXml`, `MeniIzXml` | nedostajalo-portano | F1-51 |
| 52 | AddNode | 4259 | `Form1Ulaz.MeniIzXml` | nedostajalo-portano | F1-51 |
| 53 | NapuniDrvoVrijeme | 4296 | `Stablo.IzTeksta` | isto | sta-stablo |
| 54 | DazovaIzStore | 4435 | `Stablo.IzStore` | isto | Stablo |
| 55 | AddNodes | 4492 | `Stablo.DodajDjecu`, id `0` se preskače | isto | Stablo |
| 56 | DazovaVremenska | 4527 | `Stablo.GodinaMjesec` 2005–2009 | isto | Stablo |
| 57 | NovaGenerickaForma | 4543 | `GumbTok`, `Podloga.IzvrsiLanac` | isto | GumbTok |
| 58 | NovaGenerickaForma2 | 4925 | `Dijalog` | isto | STA-08 |
| 59 | NovaGenerickaForma1 | 5062 | `Podloga`, `Dijalog` | isto | LVO-04 |
| 60 | GetAllChildren | 5544 | `Form1Ulaz.NadjiPoTekstu` | nedostajalo-portano | F1-51 |
| 61 | TraziNoduDrvetupoTagu | 5555 | `Form1Ulaz.NadjiPoTekstu` | nedostajalo-portano | F1-51 |
| 62 | GetID | 5566 | `Form1Ulaz.Id` | isto | F1-51 |
| 63 | GetDXID | 5588 | `Form1Ulaz.DxId` | nedostajalo-portano | F1-51 |
| 64 | GetDetID | 5605 | `Form1Ulaz.Id` | nedostajalo-portano | F1-51 |
| 65 | GetDetDXID | 5625 | `Form1Ulaz.DxId` | nedostajalo-portano | F1-51 |
| 66 | NovaApplikacija | 5638 | `Form1Srce.AplikacijaIzTaga` | nedostajalo-portano | F1-185 |
| 67 | TreeView1_AfterSelect | 5647 | `Form1Srce.AplikacijaIzTaga` | nedostajalo-portano | F1-185 |
| 68 | TreeView2_DoubleClick | 5656 | `Form1Ulaz.RastaviTag` | nedostajalo-portano | F1-51 |
| 69 | Button1_Click | 5664 | `GumbTok` | isto | GumbTok |
| 70 | DataGridView1_DoubleClick | 5670 | `Podloga` `MasterDblClick` | isto | GranulaIzvrsenje |
| 71 | DataGridView1_MouseUp | 5674 | Desni klik ne otvara izbornik na formi. | još nedostaje | |
| 72 | ToolStripMenuItem1_Click | 5680 | Izbornik mastera ne sprema niti izvozi. | još nedostaje | |
| 73 | ToolStripMenuItem2_Click | 5768 | isti izbornik na detailu | nedostajalo-portano | F1-185 |
| 74 | Button2_Click | 5862 | `GumbTok` | isto | GumbTok |
| 75 | Button3_Click | 5868 | `GumbTok` | isto | GumbTok |
| 76 | Button4_Click | 5873 | `GumbTok` | isto | GumbTok |
| 77 | Button5_Click | 5878 | `GumbTok` | isto | GumbTok |
| 78 | Button6_Click | 5883 | `GumbTok` | isto | GumbTok |
| 79 | TreeView1_KeyDown | 5888 | Ctrl+F11, isto kao `Form1Ulaz.Tipka` | isto | F1-25 |
| 80 | TreeView2_KeyDown | 5900 | Ctrl+F11 | isto | F1-25 |
| 81 | IspisNovi | 5910 | IspisNovi ne ispisuje. return dx je maknuto. | još nedostaje | |
| 82 | IspisNoviDet | 5953 | IspisNoviDet ne ispisuje. | još nedostaje | |
| 83 | IspisGRIDREPX | 5984 | `PrintBridge` master | isto | print-master |
| 84 | Button7_Click | 6009 | `IspisZaglavlje` | isto | ispis-zaglavlje |
| 85 | IspisGRIDREPXdet | 6067 | `PrintBridge` detail | isto | print-master |
| 86 | Button77_Click | 6091 | `PrintBridge` detail | isto | print-master |
| 87 | Button10_Click | 6140 | prazan poziv | isto | |
| 88 | ApperianceDialogToolStripMenuItem_Click | 6146 | Janus AppearanceDialog | razlika R113 | |
| 89 | FormatStylesToolStripMenuItem_Click | 6154 | Janus FormatStyles | razlika R113 | |
| 90 | TableViewToolStripMenuItem_Click | 6161 | `Form1Srce.PogledIz` GridView | nedostajalo-portano | F1-114 |
| 91 | CardViewToolStripMenuItem_Click | 6169 | `Form1Srce.PogledIz` CardView | nedostajalo-portano | F1-114 |
| 92 | SingleCardViewToolStripMenuItem_Click | 6177 | `Form1Srce.PogledIz` LayoutView | nedostajalo-portano | F1-114 |
| 93 | SaveGridToolStripMenuItem_Click | 6186 | zove `SaveGridLayout` | nedostajalo-portano | F1-51 |
| 94 | LoadLayout | 6192 | `Form1Srce.Ucitaj` | nedostajalo-portano | F1-140 |
| 95 | LoadLayout2 | 6228 | `Form1Srce.Ucitaj` | nedostajalo-portano | F1-140 |
| 96 | SaveGridLayout | 6246 | `Form1Srce.Spremi` | nedostajalo-portano | F1-140 |
| 97 | LoadLayoutTEMPORARNI | 6312 | `Form1Srce.Ucitaj` | nedostajalo-portano | F1-140 |
| 98 | SaveGridLayoutTEMPORARNI | 6328 | `Form1Srce.Spremi` | nedostajalo-portano | F1-140 |
| 99 | DeleteGridLayout | 6344 | `Form1Srce.Obrisi` | nedostajalo-portano | F1-140 |
| 100 | SaveGridLayoutDet | 6386 | `Form1Srce.Datoteka` detail | nedostajalo-portano | F1-140 |

Broj u bloku 2: lažni portovi su vraćeni u još nedostaje.

Zeleni run bloka 2: `fbc3970`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43367408210

## Blok 3, funkcije 101–150

Prazno tijelo je isto. Portana su pravila koja su imala tijelo, a nisu bila u Carpu.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 101 | DeleteGridLayoutDet | 6410 | `Form1Srce.Obrisi` | nedostajalo-portano | F1-140 |
| 102 | Button8_Click | 6432 | `Form1Ulaz.SljedeciRastvoreno` | nedostajalo-portano | F1-107 |
| 103 | ResetGridLayoutToolStripMenuItem_Click | 6444 | `Form1Srce.Ucitaj` uz reset | nedostajalo-portano | F1-140 |
| 104 | GridExportToolStripMenuItem_Click | 6453 | GridExport ne piše datoteku. | još nedostaje | |
| 105 | GridEX1_CellEdited | 6466 | prazno | isto | |
| 106 | GridEX1_CellUpdated | 6471 | `Form1Ulaz.VrijednostCelije`. Petlja parametara je `PokupiParametreSaGrida` | nedostajalo-portano | F1-107 |
| 107 | Rafalno | 6546 | `Form1Ulaz.SegmentRafala` i `Lanac` | nedostajalo-portano | F1-107 |
| 108 | GridEX1_Click | 6912 | Klik grida ne postavlja SearchColumn. | još nedostaje | |
| 109 | GridEX1_DoubleClick | 6935 | `Podloga` dvostruki klik | isto | |
| 110 | GridEX1_GroupsChanged | 6941 | GroupsChanged ne sklapa grupe na gridu. | još nedostaje | |
| 111 | ContextMenuStrip3_Click | 6952 | prazno | isto | |
| 112 | GridEX1_MouseUp | 6978 | Desni klik mastera ne otvara izbornik. | još nedostaje | |
| 113 | GridEX1_KeyDown | 7015 | `Form1Srce.EnterForma` | nedostajalo-portano | F1-185 |
| 114 | GridEX2_CellUpdated | 7063 | ista vrijednost ćelije | nedostajalo-portano | F1-107 |
| 115 | GridEX2_Click | 7130 | `Form1Srce.SearchJeStupac` | nedostajalo-portano | F1-185 |
| 116 | GridEX2_KeyDown | 7145 | `Form1Ulaz.DajParametre` | nedostajalo-portano | F1-25 |
| 117 | GridEX1_RecordUpdated | 7176 | `Form1Srce.KvacicaNakonZapisa` | nedostajalo-portano | F1-185 |
| 118 | TreeView2_AfterSelect | 7182 | `Form1Ulaz.RastaviTag`, zatim `NovaGenerickaForma` | nedostajalo-portano | F1-51 |
| 119 | TreeView2_NodeMouseClick | 7191 | prazno | isto | |
| 120 | GridEX2_DoubleClick | 7201 | `Form1Srce.AplikacijaIzTaga` detail | nedostajalo-portano | F1-185 |
| 121 | GridEX2_MouseUp | 7206 | Desni klik detaila ne otvara izbornik. | još nedostaje | |
| 122 | ContextMenuStrip3_Closed | 7229 | prazno | isto | |
| 123 | ContextMenuStrip3_Closing | 7233 | `Form1Srce.ZatvaranjeOtkazano` | nedostajalo-portano | F1-185 |
| 124 | ContextMenuStrip3_ItemClicked | 7241 | `Form1Srce.StupacNakonKlika` | nedostajalo-portano | F1-185 |
| 125 | ContextMenuStrip4_Closing | 7277 | `Form1Srce.ZatvaranjeOtkazano` | nedostajalo-portano | F1-185 |
| 126 | ContextMenuStrip4_ItemClicked | 7284 | `Form1Srce.StupacNakonKlika` | nedostajalo-portano | F1-185 |
| 127 | MenuStrip1_ItemClicked | 7321 | `Form1Srce.AplikacijaIzTaga` | nedostajalo-portano | F1-185 |
| 128 | Timer1_Disposed | 7332 | prazno | isto | |
| 129 | Timer1_Tick | 7336 | Timer1_Tick ne zove punjenje mastera. | još nedostaje | |
| 130 | ToolStripStatusLabel3_Click | 7340 | `Form1Srce.TimerKlik` | nedostajalo-portano | F1-185 |
| 131 | TimerOff | 7348 | `Form1Ulaz.TimerUpaljen` prazan | nedostajalo-portano | F1-107 |
| 132 | TimerOn | 7355 | `Form1Ulaz.TimerUpaljen` | nedostajalo-portano | F1-107 |
| 133 | Button9_Click | 7396 | `Form1Srce.Obrisi` | nedostajalo-portano | F1-140 |
| 134 | TreeView2_BeforeSelect | 7401 | `Form1Srce.BojaPrijeOdabira` | nedostajalo-portano | F1-185 |
| 135 | TreeView2_LostFocus | 7407 | `Form1Srce.BojaBezFokusa` | nedostajalo-portano | F1-185 |
| 136 | GridEX1_SelectionChanged | 7413 | `Form1Srce.Detalj` | nedostajalo-portano | F1-33 |
| 137 | GridEXPrintDocument1_PrintPage | 7420 | `Form1Srce.Podnozje` | nedostajalo-portano | F1-185 |
| 138 | ButtonZaDrvo_Click_1 | 7426 | `Form1Srce.StabloSirina` | nedostajalo-portano | F1-185 |
| 139 | TreeView2_KeyDown1 | 7440 | `Form1Ulaz.DajParametre` | nedostajalo-portano | F1-25 |
| 140 | CustomFormatKolona | 7451 | `Form1Srce.Kolona` | nedostajalo-portano | F1-140 |
| 141 | Button7_Click_1 | 7650 | `GumbTok` | isto | GumbTok |
| 142 | Button8_Click_1 | 7655 | `GumbTok` | isto | GumbTok |
| 143 | Button9_Click_1 | 7660 | `GumbTok` | isto | GumbTok |
| 144 | Button10_Click_1 | 7665 | `GumbTok` | isto | GumbTok |
| 145 | butonzarazgrupiranje_Click | 7670 | `Form1Ulaz.SljedeciRastvoreno` | nedostajalo-portano | F1-107 |
| 146 | Form1_LocationChanged | 7684 | prazno | isto | |
| 147 | Form1_LostFocus | 7688 | `Form1Ulaz.TimerUpaljen` prazan | nedostajalo-portano | F1-107 |
| 148 | Form1_Resize | 7692 | `Form1Srce.ResizePaliTimer` | nedostajalo-portano | F1-185 |
| 149 | Form1_ResizeEnd | 7703 | prazno | isto | |
| 150 | Form1_SizeChanged | 7709 | nema tijela osim On Error | isto |  |

Broj u bloku 3: lažni portovi su vraćeni u još nedostaje.

## Blok 4, funkcije 151–194

Zadnji blok. Ukupno funkcija u Form1.vb je 194.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 151 | ComboBox1_Click | 7714 | prazno | isto | |
| 152 | ComboBox1_SelectedIndexChanged | 7718 | prazno | isto | |
| 153 | TabPage2_Click | 7722 | prazno | isto | |
| 154 | TabPage2_Resize | 7726 | `Form1Srce.ResizePaliTimer` | nedostajalo-portano | F1-185 |
| 155 | ComboBox1_SelectedValueChanged | 7730 | `Stablo` iz odabrane vrijednosti | isto | Stablo |
| 156 | Rafalna_Priprema | 7737 | `Form1Srce.Rafalna` | nedostajalo-portano | F1-33 |
| 157 | ButtonHELP_Click | 7777 | zove `DajPokaziHelp` | nedostajalo-portano | F1-158 |
| 158 | DajPokaziHelp | 7781 | `Form1Ulaz.ArgumentiPomoci`. `Str` ima vodeći razmak | nedostajalo-portano | F1-158 |
| 159 | GridEX1_EditingCell | 7803 | `Form1Ulaz.VrijednostCelije` | nedostajalo-portano | F1-107 |
| 160 | GridEX1_FormattingRow | 7875 | `Form1Srce.Red` | nedostajalo-portano | F1-185 |
| 161 | OriginalToolStripMenuItem_Click | 8006 | `Form1Ulaz.JezikIzbornika` jezik prazan, multilingual 0 | nedostajalo-portano | F1-158 |
| 162 | MKToolStripMenuItem_Click | 8012 | jezik MK, multilingual 2 | nedostajalo-portano | F1-158 |
| 163 | ENToolStripMenuItem_Click | 8019 | jezik EN, multilingual 2 | nedostajalo-portano | F1-158 |
| 164 | GridEX2_FormattingRow | 8026 | `Form1Srce.Red` | nedostajalo-portano | F1-185 |
| 165 | ContextMenuStrip2_Opening | 8098 | prazno | isto | |
| 166 | ContextMenuStrip3_LocationChanged | 8102 | prazno | isto | |
| 167 | ContextMenuStrip3_Opening | 8106 | prazno | isto | |
| 168 | ContextMenuStrip1_Opening | 8110 | prazno | isto | |
| 169 | Button11_Click | 8114 | `Form1Srce.Obrisi` master i detail | nedostajalo-portano | F1-140 |
| 170 | GridEX1_KeyPress | 8120 | prazno | isto | |
| 171 | TreeView2_DragDrop22 | 8126 | `Form1Srce.TekstPovlacenja` | nedostajalo-portano | F1-185 |
| 172 | GridEX1_MouseDown | 8131 | prazno | isto | |
| 173 | TreeView2_ItemDrag | 8135 | prazno | isto | |
| 174 | TreeView2_DragDrop | 8140 | `strTREEDragDrop` ide u SQL | nedostajalo-portano | F1-33 |
| 175 | TreeView2_DragOver | 8180 | `Form1Ulaz.SmijeSpustiti` | nedostajalo-portano | F1-158 |
| 176 | GridView1_CellValueChanged | 8223 | stupac rcb555 se preskače, vrijednost je `VrijednostCelije` | nedostajalo-portano | F1-107 |
| 177 | GridView1_FocusedRowChanged | 8306 | `Form1Srce.Detalj` | nedostajalo-portano | F1-33 |
| 178 | New | 8318 | New ne postavlja StoreAllOptions ni dock. | još nedostaje | |
| 179 | GridView1_KeyDown | 8338 | Enter na stupac s tagom 1 | nedostajalo-portano | F1-185 |
| 180 | GridView1_MouseUp | 8385 | Desni klik DX grida ne otvara izbornik. | još nedostaje | |
| 181 | GridView1_ShownEditor | 8412 | editor označi sav tekst | nedostajalo-portano | F1-185 |
| 182 | Timer2_Tick | 8424 | jednom timer, pa se gasi | nedostajalo-portano | F1-107 |
| 183 | GridEX3_MouseUp | 8429 | `Form1Ulaz.DmsIzbornik` | nedostajalo-portano | F1-158 |
| 184 | DMS_odustani | 8464 | `Form1Ulaz.SqlDmsOdustani` | nedostajalo-portano | F1-158 |
| 185 | DMS_preuzmizapotpis | 8484 | Nema CreateInstance ni DocumentCheckOut. | još nedostaje | |
| 186 | DMS_vratipotpisano | 8528 | Nema DocumentCheckIn. | još nedostaje | |
| 187 | DMS_Obrisi | 8574 | SQL brisanja se ne šalje. | još nedostaje | |
| 188 | DMS_SpremiNaDisk | 8599 | Nema SaveDocuments. | još nedostaje | |
| 189 | DMS_Pogledaj | 8639 | Nema OpenDocument. | još nedostaje | |
| 190 | TabControl1_DrawItem | 8678 | prazno | isto | |
| 191 | GridEX3_RowDoubleClick | 8702 | Dvostruki klik ne zove pogled. | još nedostaje | |
| 192 | ContextMenuStrip4_Opening | 8706 | prazno | isto | |
| 193 | ParentV_Resize | 8710 | Prozor se ne miče na (0,0). | još nedostaje | |
| 194 | ParentForm_LocationChanged | 8719 | Prozor se ne miče na (0,0). | još nedostaje | |

QA uzorak na `7275629` vratio je 27 lažnih portova. Pomoćne funkcije koje samo vrate tekst, `true` ili predani argument su maknute. One nisu port.

Portano znači kod na izvršnom putu `Carpo.NetGen` i test učinka na formi, gridu ili spinu (redovi, red u bazi, vidljivost, položaj). Vraćena vrijednost pomoćne funkcije nije port.

Trenutno je još nedostaje 27 od onih 129. Ostali `nedostajalo-portano` retci su pravila u pomoćnim funkcijama bez testa učinka i idu na istu reviziju. `PokaziUpit` i dalje nije port: `MasterGrid.IzvrsiCitanje` zna poslati SQL, ali timer, novi id, grupe i DX izlaz nisu u tom putu.
