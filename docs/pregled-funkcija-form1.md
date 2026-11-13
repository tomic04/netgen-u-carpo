# Pregled Form1.vb

Izvor je `netgen/Form1.vb`. Designer se ne broji. Status: isto, razlika, još nedostaje. Portano je samo kod na izvršnom putu forme s testom učinka.

Baza je `6a77c47`. Blokovi idu po pedeset funkcija.

## Blok 1, funkcije 1–50

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 1 | SendMessage | 138 | nije na izvršnom putu | još nedostaje | |
| 2 | SetParent | 142 | prazan DllImport, nema tijela | isto | |
| 3 | SetWindowPos | 146 | prazan DllImport. Poziv u `Loader.ShellResize` je `Loader.vb`, ne Form1 | isto | |
| 4 | ShellResize | 211 | nije na izvršnom putu | još nedostaje | |
| 5 | WndProc | 220 | nije na izvršnom putu | još nedostaje | |
| 6 | ClientToolbarGetButtons | 233 | nije na izvršnom putu | još nedostaje | |
| 7 | ClientToolbarButtonClick | 251 | nije na izvršnom putu | još nedostaje | |
| 8 | Form1_Activated | 264 | nije na izvršnom putu | još nedostaje | |
| 9 | Form1_Deactivate | 279 | nije na izvršnom putu | još nedostaje | |
| 10 | Fiskalizacija | 286 | `FiskalTok` | razlika R109 | FiskalTok |
| 11 | FiskalizacijaNapojnica | 615 | `FiskalTok` tablica −1 | razlika R109 | FiskalTok |
| 12 | PoslovniProstor | 809 | nije na izvršnom putu | još nedostaje | |
| 13 | PoslovniProstor_COM | 849 | nije na izvršnom putu | još nedostaje | |
| 14 | DajCB | 1016 | nije na izvršnom putu | još nedostaje | |
| 15 | DajSQL | 1057 | nije na izvršnom putu | još nedostaje | |
| 16 | DajParametre | 1066 | nije na izvršnom putu | još nedostaje | |
| 17 | MYExnHandler | 1077 | MYExnHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 18 | MYThreadHandler | 1083 | MYThreadHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 19 | Run8 | 1087 | nije na izvršnom putu | još nedostaje | |
| 20 | Run | 1098 | nije na izvršnom putu | još nedostaje | |
| 21 | Form1_FormClosed | 1298 | nije na izvršnom putu | još nedostaje | |
| 22 | Form1_FormClosing | 1314 | nije na izvršnom putu | još nedostaje | |
| 23 | Form1_GotFocus | 1340 | nije na izvršnom putu | još nedostaje | |
| 24 | Form1_Invalidated | 1344 | prazno tijelo | isto | |
| 25 | Form1_KeyDown | 1350 | nije na izvršnom putu | još nedostaje | |
| 26 | NapuniRSLang | 1398 | nije na izvršnom putu | još nedostaje | |
| 27 | Jezici | 1484 | nije na izvršnom putu | još nedostaje | |
| 28 | Form1_Leave | 1497 | nije na izvršnom putu | još nedostaje | |
| 29 | Form1_Load | 1500 | Form1_Load nije izvršni put. Stalni redoslijed je maknut. | još nedostaje | |
| 30 | PronadjiOperatera | 1694 | nije na izvršnom putu | još nedostaje | |
| 31 | PokupiUlazneparametre | 1707 | nije na izvršnom putu | još nedostaje | |
| 32 | DodatnoFormatirajGridnakonRetriveStructure | 1777 | nije na izvršnom putu | još nedostaje | |
| 33 | PokaziUpit | 1825 | `Podloga.NapuniGrid` šalje ONLOAD i veže redove na grid (`Form1.vb` 1855–1859). Timer, novi id, grupe, total i detail iz istog Suba nisu taj put. | isto | pokazi-upit |
| 34 | PrevediSadrzajGrida2 | 2189 | nije na izvršnom putu | još nedostaje | |
| 35 | PokaziBrojStavki | 2232 | nije na izvršnom putu | još nedostaje | |
| 36 | PokaziUpitDetalj | 2258 | PokaziUpitDetalj ne izlazi dok se puni master i ne puni detail grid. | još nedostaje | |
| 37 | DMS_pOkazi_Upit | 2447 | nije na izvršnom putu | još nedostaje | |
| 38 | SrediSQL | 2470 | `Grid/SqlTekst.cs` | isto | SqlTekst |
| 39 | GetLayoutDirectory | 2509 | nije na izvršnom putu | još nedostaje | |
| 40 | PokupiParametreSaGrida | 2544 | PokupiParametreSaGrida ne čita stupce retka. Spajanje stringova je maknuto. | još nedostaje | |
| 41 | PokupiParametreSaHijerarHijskogGrida | 2615 | `DetailGrid` | isto | DetailGrid |
| 42 | ssPermissionProvjeriRafalnuKomandu | 2665 | nije na izvršnom putu | još nedostaje | |
| 43 | ssPermissionProcitaj | 2694 | nije na izvršnom putu | još nedostaje | |
| 44 | ssPermissionProvjeri | 2714 | ssPermissionProvjeri ne čita trsPermisio ni Button.Text. | još nedostaje | |
| 45 | ProcitajXMLzaPodlogu | 3028 | `NetGenXmlReader`, `Podloga` | isto | postojeći |
| 46 | SetDefolte | 3945 | `ClipboardNg` | isto | ClipboardNg |
| 47 | DodatnoPostimajKontrolu | 4013 | `OnLoad2` | isto | OnLoad2 |
| 48 | NapuniMenuFolder | 4037 | nije na izvršnom putu | još nedostaje | |
| 49 | NapuniMenuBaza | 4088 | nije na izvršnom putu | još nedostaje | |
| 50 | NapuniDrvo | 4127 | nije na izvršnom putu | još nedostaje | |

Broj u bloku 1: lažni portovi su vraćeni u još nedostaje.

Zeleni run bloka 1: `3ad0d66`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43366476116

## Blok 2, funkcije 51–100

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 51 | NapuniDrvoIzXMLA | 4217 | nije na izvršnom putu | još nedostaje | |
| 52 | AddNode | 4259 | nije na izvršnom putu | još nedostaje | |
| 53 | NapuniDrvoVrijeme | 4296 | `Stablo.IzTeksta` | isto | sta-stablo |
| 54 | DazovaIzStore | 4435 | `Stablo.IzStore` | isto | Stablo |
| 55 | AddNodes | 4492 | `Stablo.DodajDjecu`, id `0` se preskače | isto | Stablo |
| 56 | DazovaVremenska | 4527 | `Stablo.GodinaMjesec` 2005–2009 | isto | Stablo |
| 57 | NovaGenerickaForma | 4543 | `GumbTok`, `Podloga.IzvrsiLanac` | isto | GumbTok |
| 58 | NovaGenerickaForma2 | 4925 | `Dijalog` | isto | STA-08 |
| 59 | NovaGenerickaForma1 | 5062 | `Podloga`, `Dijalog` | isto | LVO-04 |
| 60 | GetAllChildren | 5544 | nije na izvršnom putu | još nedostaje | |
| 61 | TraziNoduDrvetupoTagu | 5555 | nije na izvršnom putu | još nedostaje | |
| 62 | GetID | 5566 | nije na izvršnom putu | još nedostaje | |
| 63 | GetDXID | 5588 | nije na izvršnom putu | još nedostaje | |
| 64 | GetDetID | 5605 | nije na izvršnom putu | još nedostaje | |
| 65 | GetDetDXID | 5625 | nije na izvršnom putu | još nedostaje | |
| 66 | NovaApplikacija | 5638 | nije na izvršnom putu | još nedostaje | |
| 67 | TreeView1_AfterSelect | 5647 | nije na izvršnom putu | još nedostaje | |
| 68 | TreeView2_DoubleClick | 5656 | nije na izvršnom putu | još nedostaje | |
| 69 | Button1_Click | 5664 | `GumbTok` | isto | GumbTok |
| 70 | DataGridView1_DoubleClick | 5670 | `Podloga` `MasterDblClick` | isto | GranulaIzvrsenje |
| 71 | DataGridView1_MouseUp | 5674 | Desni klik ne otvara izbornik na formi. | još nedostaje | |
| 72 | ToolStripMenuItem1_Click | 5680 | Izbornik mastera ne sprema niti izvozi. | još nedostaje | |
| 73 | ToolStripMenuItem2_Click | 5768 | nije na izvršnom putu | još nedostaje | |
| 74 | Button2_Click | 5862 | `GumbTok` | isto | GumbTok |
| 75 | Button3_Click | 5868 | `GumbTok` | isto | GumbTok |
| 76 | Button4_Click | 5873 | `GumbTok` | isto | GumbTok |
| 77 | Button5_Click | 5878 | `GumbTok` | isto | GumbTok |
| 78 | Button6_Click | 5883 | `GumbTok` | isto | GumbTok |
| 79 | TreeView1_KeyDown | 5888 | nije na izvršnom putu | još nedostaje | |
| 80 | TreeView2_KeyDown | 5900 | nije na izvršnom putu | još nedostaje | |
| 81 | IspisNovi | 5910 | IspisNovi ne ispisuje. return dx je maknuto. | još nedostaje | |
| 82 | IspisNoviDet | 5953 | IspisNoviDet ne ispisuje. | još nedostaje | |
| 83 | IspisGRIDREPX | 5984 | `PrintBridge` master | isto | print-master |
| 84 | Button7_Click | 6009 | `IspisZaglavlje` | isto | ispis-zaglavlje |
| 85 | IspisGRIDREPXdet | 6067 | `PrintBridge` detail | isto | print-master |
| 86 | Button77_Click | 6091 | `PrintBridge` detail | isto | print-master |
| 87 | Button10_Click | 6140 | prazan poziv | isto | |
| 88 | ApperianceDialogToolStripMenuItem_Click | 6146 | Janus AppearanceDialog | razlika R113 | |
| 89 | FormatStylesToolStripMenuItem_Click | 6154 | Janus FormatStyles | razlika R113 | |
| 90 | TableViewToolStripMenuItem_Click | 6161 | nije na izvršnom putu | još nedostaje | |
| 91 | CardViewToolStripMenuItem_Click | 6169 | nije na izvršnom putu | još nedostaje | |
| 92 | SingleCardViewToolStripMenuItem_Click | 6177 | nije na izvršnom putu | još nedostaje | |
| 93 | SaveGridToolStripMenuItem_Click | 6186 | nije na izvršnom putu | još nedostaje | |
| 94 | LoadLayout | 6192 | nije na izvršnom putu | još nedostaje | |
| 95 | LoadLayout2 | 6228 | nije na izvršnom putu | još nedostaje | |
| 96 | SaveGridLayout | 6246 | nije na izvršnom putu | još nedostaje | |
| 97 | LoadLayoutTEMPORARNI | 6312 | nije na izvršnom putu | još nedostaje | |
| 98 | SaveGridLayoutTEMPORARNI | 6328 | nije na izvršnom putu | još nedostaje | |
| 99 | DeleteGridLayout | 6344 | nije na izvršnom putu | još nedostaje | |
| 100 | SaveGridLayoutDet | 6386 | nije na izvršnom putu | još nedostaje | |

Broj u bloku 2: lažni portovi su vraćeni u još nedostaje.

Zeleni run bloka 2: `fbc3970`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43367408210

## Blok 3, funkcije 101–150

Prazno tijelo je isto. Tijelo koje forma ne izvršava je još nedostaje.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 101 | DeleteGridLayoutDet | 6410 | nije na izvršnom putu | još nedostaje | |
| 102 | Button8_Click | 6432 | nije na izvršnom putu | još nedostaje | |
| 103 | ResetGridLayoutToolStripMenuItem_Click | 6444 | nije na izvršnom putu | još nedostaje | |
| 104 | GridExportToolStripMenuItem_Click | 6453 | GridExport ne piše datoteku. | još nedostaje | |
| 105 | GridEX1_CellEdited | 6466 | prazno | isto | |
| 106 | GridEX1_CellUpdated | 6471 | nije na izvršnom putu | još nedostaje | |
| 107 | Rafalno | 6546 | nije na izvršnom putu | još nedostaje | |
| 108 | GridEX1_Click | 6912 | Klik grida ne postavlja SearchColumn. | još nedostaje | |
| 109 | GridEX1_DoubleClick | 6935 | `Podloga` dvostruki klik | isto | |
| 110 | GridEX1_GroupsChanged | 6941 | GroupsChanged ne sklapa grupe na gridu. | još nedostaje | |
| 111 | ContextMenuStrip3_Click | 6952 | prazno | isto | |
| 112 | GridEX1_MouseUp | 6978 | Desni klik mastera ne otvara izbornik. | još nedostaje | |
| 113 | GridEX1_KeyDown | 7015 | nije na izvršnom putu | još nedostaje | |
| 114 | GridEX2_CellUpdated | 7063 | nije na izvršnom putu | još nedostaje | |
| 115 | GridEX2_Click | 7130 | nije na izvršnom putu | još nedostaje | |
| 116 | GridEX2_KeyDown | 7145 | nije na izvršnom putu | još nedostaje | |
| 117 | GridEX1_RecordUpdated | 7176 | nije na izvršnom putu | još nedostaje | |
| 118 | TreeView2_AfterSelect | 7182 | nije na izvršnom putu | još nedostaje | |
| 119 | TreeView2_NodeMouseClick | 7191 | prazno | isto | |
| 120 | GridEX2_DoubleClick | 7201 | nije na izvršnom putu | još nedostaje | |
| 121 | GridEX2_MouseUp | 7206 | Desni klik detaila ne otvara izbornik. | još nedostaje | |
| 122 | ContextMenuStrip3_Closed | 7229 | prazno | isto | |
| 123 | ContextMenuStrip3_Closing | 7233 | nije na izvršnom putu | još nedostaje | |
| 124 | ContextMenuStrip3_ItemClicked | 7241 | nije na izvršnom putu | još nedostaje | |
| 125 | ContextMenuStrip4_Closing | 7277 | nije na izvršnom putu | još nedostaje | |
| 126 | ContextMenuStrip4_ItemClicked | 7284 | nije na izvršnom putu | još nedostaje | |
| 127 | MenuStrip1_ItemClicked | 7321 | nije na izvršnom putu | još nedostaje | |
| 128 | Timer1_Disposed | 7332 | prazno | isto | |
| 129 | Timer1_Tick | 7336 | Timer1_Tick ne zove punjenje mastera. | još nedostaje | |
| 130 | ToolStripStatusLabel3_Click | 7340 | nije na izvršnom putu | još nedostaje | |
| 131 | TimerOff | 7348 | nije na izvršnom putu | još nedostaje | |
| 132 | TimerOn | 7355 | nije na izvršnom putu | još nedostaje | |
| 133 | Button9_Click | 7396 | nije na izvršnom putu | još nedostaje | |
| 134 | TreeView2_BeforeSelect | 7401 | nije na izvršnom putu | još nedostaje | |
| 135 | TreeView2_LostFocus | 7407 | nije na izvršnom putu | još nedostaje | |
| 136 | GridEX1_SelectionChanged | 7413 | nije na izvršnom putu | još nedostaje | |
| 137 | GridEXPrintDocument1_PrintPage | 7420 | nije na izvršnom putu | još nedostaje | |
| 138 | ButtonZaDrvo_Click_1 | 7426 | nije na izvršnom putu | još nedostaje | |
| 139 | TreeView2_KeyDown1 | 7440 | nije na izvršnom putu | još nedostaje | |
| 140 | CustomFormatKolona | 7451 | nije na izvršnom putu | još nedostaje | |
| 141 | Button7_Click_1 | 7650 | `GumbTok` | isto | GumbTok |
| 142 | Button8_Click_1 | 7655 | `GumbTok` | isto | GumbTok |
| 143 | Button9_Click_1 | 7660 | `GumbTok` | isto | GumbTok |
| 144 | Button10_Click_1 | 7665 | `GumbTok` | isto | GumbTok |
| 145 | butonzarazgrupiranje_Click | 7670 | nije na izvršnom putu | još nedostaje | |
| 146 | Form1_LocationChanged | 7684 | prazno | isto | |
| 147 | Form1_LostFocus | 7688 | nije na izvršnom putu | još nedostaje | |
| 148 | Form1_Resize | 7692 | nije na izvršnom putu | još nedostaje | |
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
| 154 | TabPage2_Resize | 7726 | nije na izvršnom putu | još nedostaje | |
| 155 | ComboBox1_SelectedValueChanged | 7730 | `Stablo` iz odabrane vrijednosti | isto | Stablo |
| 156 | Rafalna_Priprema | 7737 | nije na izvršnom putu | još nedostaje | |
| 157 | ButtonHELP_Click | 7777 | nije na izvršnom putu | još nedostaje | |
| 158 | DajPokaziHelp | 7781 | nije na izvršnom putu | još nedostaje | |
| 159 | GridEX1_EditingCell | 7803 | nije na izvršnom putu | još nedostaje | |
| 160 | GridEX1_FormattingRow | 7875 | nije na izvršnom putu | još nedostaje | |
| 161 | OriginalToolStripMenuItem_Click | 8006 | nije na izvršnom putu | još nedostaje | |
| 162 | MKToolStripMenuItem_Click | 8012 | nije na izvršnom putu | još nedostaje | |
| 163 | ENToolStripMenuItem_Click | 8019 | nije na izvršnom putu | još nedostaje | |
| 164 | GridEX2_FormattingRow | 8026 | nije na izvršnom putu | još nedostaje | |
| 165 | ContextMenuStrip2_Opening | 8098 | prazno | isto | |
| 166 | ContextMenuStrip3_LocationChanged | 8102 | prazno | isto | |
| 167 | ContextMenuStrip3_Opening | 8106 | prazno | isto | |
| 168 | ContextMenuStrip1_Opening | 8110 | prazno | isto | |
| 169 | Button11_Click | 8114 | nije na izvršnom putu | još nedostaje | |
| 170 | GridEX1_KeyPress | 8120 | prazno | isto | |
| 171 | TreeView2_DragDrop22 | 8126 | nije na izvršnom putu | još nedostaje | |
| 172 | GridEX1_MouseDown | 8131 | prazno | isto | |
| 173 | TreeView2_ItemDrag | 8135 | prazno | isto | |
| 174 | TreeView2_DragDrop | 8140 | nije na izvršnom putu | još nedostaje | |
| 175 | TreeView2_DragOver | 8180 | nije na izvršnom putu | još nedostaje | |
| 176 | GridView1_CellValueChanged | 8223 | nije na izvršnom putu | još nedostaje | |
| 177 | GridView1_FocusedRowChanged | 8306 | nije na izvršnom putu | još nedostaje | |
| 178 | New | 8318 | New ne postavlja StoreAllOptions ni dock. | još nedostaje | |
| 179 | GridView1_KeyDown | 8338 | nije na izvršnom putu | još nedostaje | |
| 180 | GridView1_MouseUp | 8385 | Desni klik DX grida ne otvara izbornik. | još nedostaje | |
| 181 | GridView1_ShownEditor | 8412 | nije na izvršnom putu | još nedostaje | |
| 182 | Timer2_Tick | 8424 | nije na izvršnom putu | još nedostaje | |
| 183 | GridEX3_MouseUp | 8429 | nije na izvršnom putu | još nedostaje | |
| 184 | DMS_odustani | 8464 | nije na izvršnom putu | još nedostaje | |
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

Form1Ulaz i Form1Srce su maknuti. Portano je samo ono što forma radi i test izmjeri. `PokaziUpit` slanje i bind su `isto` (`pokazi-upit` na JeziciNG, redovi u gridu). Ostatak tog Suba je još nedostaje.
