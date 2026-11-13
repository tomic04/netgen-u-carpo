# Pregled Form1.vb

Izvor je `netgen/Form1.vb`. Designer se ne broji. Status: isto, razlika R-broj, nedostajalo-portano, još nedostaje. Test je [Z] red ili postojeći pokrivač.

Baza je `6a77c47`. Blokovi idu po pedeset funkcija.

QA (`4b691d7`): pregledano 107, pravo 0, vraćeno 107. Od toga 86 je ostatak zadatka od 87 (`GridEX1_Click` je već bio još nedostaje), 15 je prvi uzorak koji na ovom kriteriju pada, 6 su nove tvrdnje (`Run`, `Run8`, `ToolStripMenuItem2_Click`, tri pogleda). `Form1Ulaz` i `Form1Srce` zovu samo testovi. Test ne gleda formu, grid ni bazu.

## Blok 1, funkcije 1–50

Port je `Devex_26/Carpo.NetGen/Ulaz/Form1Ulaz.cs`. Test je `tests/Carpo.NetGen.Granule/Form1Blok.cs`.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 1 | SendMessage | 138 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 2 | SetParent | 142 | prazan DllImport, nema tijela | isto | |
| 3 | SetWindowPos | 146 | prazan DllImport. Poziv u `Loader.ShellResize` je `Loader.vb`, ne Form1 | isto | |
| 4 | ShellResize | 211 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 5 | WndProc | 220 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 6 | ClientToolbarGetButtons | 233 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 7 | ClientToolbarButtonClick | 251 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 8 | Form1_Activated | 264 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 9 | Form1_Deactivate | 279 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 10 | Fiskalizacija | 286 | `FiskalTok` | razlika R109 | FiskalTok |
| 11 | FiskalizacijaNapojnica | 615 | `FiskalTok` tablica −1 | razlika R109 | FiskalTok |
| 12 | PoslovniProstor | 809 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 13 | PoslovniProstor_COM | 849 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 14 | DajCB | 1016 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 15 | DajSQL | 1057 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 16 | DajParametre | 1066 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 17 | MYExnHandler | 1077 | MYExnHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 18 | MYThreadHandler | 1083 | MYThreadHandler samo vraća predani trag. Nije na formi. | još nedostaje | |
| 19 | Run8 | 1087 | Run8 zove Prevoditelj.Ucitaj. F1-114 je zelen i kad JupiterTranslator10.dll nema. Granula se ne otvara. | još nedostaje | |
| 20 | Run | 1098 | Isti F1-114. Assembly.Load se ne provjerava na formi. | još nedostaje | |
| 21 | Form1_FormClosed | 1298 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 22 | Form1_FormClosing | 1314 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 23 | Form1_GotFocus | 1340 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 24 | Form1_Invalidated | 1344 | prazno tijelo | isto | |
| 25 | Form1_KeyDown | 1350 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 26 | NapuniRSLang | 1398 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 27 | Jezici | 1484 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 28 | Form1_Leave | 1497 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 29 | Form1_Load | 1500 | Form1_Load nije izvršni put. Stalni redoslijed je maknut. | još nedostaje | |
| 30 | PronadjiOperatera | 1694 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 31 | PokupiUlazneparametre | 1707 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 32 | DodatnoFormatirajGridnakonRetriveStructure | 1777 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 33 | PokaziUpit | 1825 | MasterGrid.IzvrsiCitanje zna poslati SQL. Timer, novi id, grupe i DX izlaz iz PokaziUpit nisu u tom putu. | još nedostaje | |
| 34 | PrevediSadrzajGrida2 | 2189 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 35 | PokaziBrojStavki | 2232 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 36 | PokaziUpitDetalj | 2258 | PokaziUpitDetalj ne izlazi dok se puni master i ne puni detail grid. | još nedostaje | |
| 37 | DMS_pOkazi_Upit | 2447 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 38 | SrediSQL | 2470 | `Grid/SqlTekst.cs` | isto | SqlTekst |
| 39 | GetLayoutDirectory | 2509 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 40 | PokupiParametreSaGrida | 2544 | PokupiParametreSaGrida ne čita stupce retka. Spajanje stringova je maknuto. | još nedostaje | |
| 41 | PokupiParametreSaHijerarHijskogGrida | 2615 | `DetailGrid` | isto | DetailGrid |
| 42 | ssPermissionProvjeriRafalnuKomandu | 2665 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 43 | ssPermissionProcitaj | 2694 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 44 | ssPermissionProvjeri | 2714 | ssPermissionProvjeri ne čita trsPermisio ni Button.Text. | još nedostaje | |
| 45 | ProcitajXMLzaPodlogu | 3028 | `NetGenXmlReader`, `Podloga` | isto | postojeći |
| 46 | SetDefolte | 3945 | `ClipboardNg` | isto | ClipboardNg |
| 47 | DodatnoPostimajKontrolu | 4013 | `OnLoad2` | isto | OnLoad2 |
| 48 | NapuniMenuFolder | 4037 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 49 | NapuniMenuBaza | 4088 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 50 | NapuniDrvo | 4127 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |

Broj u bloku: isto 8, razlika 2, još nedostaje 40, nedostajalo-portano 0.

Zeleni run bloka 1: `3ad0d66`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43366476116

## Blok 2, funkcije 51–100

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 51 | NapuniDrvoIzXMLA | 4217 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 52 | AddNode | 4259 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 53 | NapuniDrvoVrijeme | 4296 | `Stablo.IzTeksta` | isto | sta-stablo |
| 54 | DazovaIzStore | 4435 | `Stablo.IzStore` | isto | Stablo |
| 55 | AddNodes | 4492 | `Stablo.DodajDjecu`, id `0` se preskače | isto | Stablo |
| 56 | DazovaVremenska | 4527 | `Stablo.GodinaMjesec` 2005–2009 | isto | Stablo |
| 57 | NovaGenerickaForma | 4543 | `GumbTok`, `Podloga.IzvrsiLanac` | isto | GumbTok |
| 58 | NovaGenerickaForma2 | 4925 | `Dijalog` | isto | STA-08 |
| 59 | NovaGenerickaForma1 | 5062 | `Podloga`, `Dijalog` | isto | LVO-04 |
| 60 | GetAllChildren | 5544 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 61 | TraziNoduDrvetupoTagu | 5555 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 62 | GetID | 5566 | `Form1Ulaz.Id` | isto | F1-51 |
| 63 | GetDXID | 5588 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 64 | GetDetID | 5605 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 65 | GetDetDXID | 5625 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 66 | NovaApplikacija | 5638 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 67 | TreeView1_AfterSelect | 5647 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 68 | TreeView2_DoubleClick | 5656 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 69 | Button1_Click | 5664 | `GumbTok` | isto | GumbTok |
| 70 | DataGridView1_DoubleClick | 5670 | `Podloga` `MasterDblClick` | isto | GranulaIzvrsenje |
| 71 | DataGridView1_MouseUp | 5674 | Desni klik ne otvara izbornik na formi. | još nedostaje | |
| 72 | ToolStripMenuItem1_Click | 5680 | Izbornik mastera ne sprema niti izvozi. | još nedostaje | |
| 73 | ToolStripMenuItem2_Click | 5768 | Izbornik detaila ne sprema niti izvozi na formi. | još nedostaje | |
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
| 90 | TableViewToolStripMenuItem_Click | 6161 | PogledIz vraća GridView. Grid se ne prebacuje. | još nedostaje | |
| 91 | CardViewToolStripMenuItem_Click | 6169 | PogledIz vraća CardView. Grid se ne prebacuje. | još nedostaje | |
| 92 | SingleCardViewToolStripMenuItem_Click | 6177 | PogledIz vraća LayoutView. Grid se ne prebacuje. | još nedostaje | |
| 93 | SaveGridToolStripMenuItem_Click | 6186 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 94 | LoadLayout | 6192 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 95 | LoadLayout2 | 6228 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 96 | SaveGridLayout | 6246 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 97 | LoadLayoutTEMPORARNI | 6312 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 98 | SaveGridLayoutTEMPORARNI | 6328 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 99 | DeleteGridLayout | 6344 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 100 | SaveGridLayoutDet | 6386 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |

Broj u bloku: isto 22, razlika 2, još nedostaje 26, nedostajalo-portano 0.

Zeleni run bloka 2: `fbc3970`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43367408210

## Blok 3, funkcije 101–150

Prazno tijelo je isto. Portana su pravila koja su imala tijelo, a nisu bila u Carpu.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 101 | DeleteGridLayoutDet | 6410 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 102 | Button8_Click | 6432 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 103 | ResetGridLayoutToolStripMenuItem_Click | 6444 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 104 | GridExportToolStripMenuItem_Click | 6453 | GridExport ne piše datoteku. | još nedostaje | |
| 105 | GridEX1_CellEdited | 6466 | prazno | isto | |
| 106 | GridEX1_CellUpdated | 6471 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 107 | Rafalno | 6546 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 108 | GridEX1_Click | 6912 | Klik grida ne postavlja SearchColumn. | još nedostaje | |
| 109 | GridEX1_DoubleClick | 6935 | `Podloga` dvostruki klik | isto | |
| 110 | GridEX1_GroupsChanged | 6941 | GroupsChanged ne sklapa grupe na gridu. | još nedostaje | |
| 111 | ContextMenuStrip3_Click | 6952 | prazno | isto | |
| 112 | GridEX1_MouseUp | 6978 | Desni klik mastera ne otvara izbornik. | još nedostaje | |
| 113 | GridEX1_KeyDown | 7015 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 114 | GridEX2_CellUpdated | 7063 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 115 | GridEX2_Click | 7130 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 116 | GridEX2_KeyDown | 7145 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 117 | GridEX1_RecordUpdated | 7176 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 118 | TreeView2_AfterSelect | 7182 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 119 | TreeView2_NodeMouseClick | 7191 | prazno | isto | |
| 120 | GridEX2_DoubleClick | 7201 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 121 | GridEX2_MouseUp | 7206 | Desni klik detaila ne otvara izbornik. | još nedostaje | |
| 122 | ContextMenuStrip3_Closed | 7229 | prazno | isto | |
| 123 | ContextMenuStrip3_Closing | 7233 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 124 | ContextMenuStrip3_ItemClicked | 7241 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 125 | ContextMenuStrip4_Closing | 7277 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 126 | ContextMenuStrip4_ItemClicked | 7284 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 127 | MenuStrip1_ItemClicked | 7321 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 128 | Timer1_Disposed | 7332 | prazno | isto | |
| 129 | Timer1_Tick | 7336 | Timer1_Tick ne zove punjenje mastera. | još nedostaje | |
| 130 | ToolStripStatusLabel3_Click | 7340 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 131 | TimerOff | 7348 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 132 | TimerOn | 7355 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 133 | Button9_Click | 7396 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 134 | TreeView2_BeforeSelect | 7401 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 135 | TreeView2_LostFocus | 7407 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 136 | GridEX1_SelectionChanged | 7413 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 137 | GridEXPrintDocument1_PrintPage | 7420 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 138 | ButtonZaDrvo_Click_1 | 7426 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 139 | TreeView2_KeyDown1 | 7440 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 140 | CustomFormatKolona | 7451 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 141 | Button7_Click_1 | 7650 | `GumbTok` | isto | GumbTok |
| 142 | Button8_Click_1 | 7655 | `GumbTok` | isto | GumbTok |
| 143 | Button9_Click_1 | 7660 | `GumbTok` | isto | GumbTok |
| 144 | Button10_Click_1 | 7665 | `GumbTok` | isto | GumbTok |
| 145 | butonzarazgrupiranje_Click | 7670 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 146 | Form1_LocationChanged | 7684 | prazno | isto | |
| 147 | Form1_LostFocus | 7688 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 148 | Form1_Resize | 7692 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 149 | Form1_ResizeEnd | 7703 | prazno | isto | |
| 150 | Form1_SizeChanged | 7709 | nema tijela osim On Error | isto |  |

Broj u bloku: isto 13, razlika 0, još nedostaje 37, nedostajalo-portano 0.

## Blok 4, funkcije 151–194

Zadnji blok. Ukupno funkcija u Form1.vb je 194.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 151 | ComboBox1_Click | 7714 | prazno | isto | |
| 152 | ComboBox1_SelectedIndexChanged | 7718 | prazno | isto | |
| 153 | TabPage2_Click | 7722 | prazno | isto | |
| 154 | TabPage2_Resize | 7726 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 155 | ComboBox1_SelectedValueChanged | 7730 | `Stablo` iz odabrane vrijednosti | isto | Stablo |
| 156 | Rafalna_Priprema | 7737 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 157 | ButtonHELP_Click | 7777 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 158 | DajPokaziHelp | 7781 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 159 | GridEX1_EditingCell | 7803 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 160 | GridEX1_FormattingRow | 7875 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 161 | OriginalToolStripMenuItem_Click | 8006 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 162 | MKToolStripMenuItem_Click | 8012 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 163 | ENToolStripMenuItem_Click | 8019 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 164 | GridEX2_FormattingRow | 8026 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 165 | ContextMenuStrip2_Opening | 8098 | prazno | isto | |
| 166 | ContextMenuStrip3_LocationChanged | 8102 | prazno | isto | |
| 167 | ContextMenuStrip3_Opening | 8106 | prazno | isto | |
| 168 | ContextMenuStrip1_Opening | 8110 | prazno | isto | |
| 169 | Button11_Click | 8114 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 170 | GridEX1_KeyPress | 8120 | prazno | isto | |
| 171 | TreeView2_DragDrop22 | 8126 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 172 | GridEX1_MouseDown | 8131 | prazno | isto | |
| 173 | TreeView2_ItemDrag | 8135 | prazno | isto | |
| 174 | TreeView2_DragDrop | 8140 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 175 | TreeView2_DragOver | 8180 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 176 | GridView1_CellValueChanged | 8223 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 177 | GridView1_FocusedRowChanged | 8306 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 178 | New | 8318 | New ne postavlja StoreAllOptions ni dock. | još nedostaje | |
| 179 | GridView1_KeyDown | 8338 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 180 | GridView1_MouseUp | 8385 | Desni klik DX grida ne otvara izbornik. | još nedostaje | |
| 181 | GridView1_ShownEditor | 8412 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 182 | Timer2_Tick | 8424 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 183 | GridEX3_MouseUp | 8429 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
| 184 | DMS_odustani | 8464 | Pomoćna funkcija nije na izvršnom putu forme. Test uspoređuje povrat, ne učinak na formi, gridu ni bazi. | još nedostaje | |
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

Broj u bloku: isto 13, razlika 0, još nedostaje 31, nedostajalo-portano 0.

QA na `4b691d7`: svi preostali `nedostajalo-portano` vraćeni su u još nedostaje. `Form1Ulaz` i `Form1Srce` zovu samo testovi (`Form1Blok`, `Form1SrceTest`), ne `Podloga`, `Dijalog` ni ljuska. Test uspoređuje povrat pomoćne funkcije. To nije učinak na formi, gridu ni bazi.

Portano znači kod na izvršnom putu `Carpo.NetGen` i test učinka na formi, gridu ili spinu (redovi, red u bazi, vidljivost, položaj). Vraćena vrijednost pomoćne funkcije nije port.

Isti kriterij vraća i prvih 15 iz uzorka (`WndProc`, alatna traka, `PoslovniProstor_COM`, `Form1_KeyDown`, ulazni parametri, format stupca, prijevod grida, broj stavki, rafalna dozvola, stablo iz XML-a, `SaveGridLayout`, `Rafalno`, `CustomFormatKolona`, `DajPokaziHelp`) i nove tvrdnje `Run`/`Run8` (F1-114 je zelen i bez `JupiterTranslator10.dll`, granula se ne otvara) te poglede `TableView`/`CardView`/`SingleCardView`. `ToolStripMenuItem2_Click` je bio vraćen, pa opet označen portanim. `PokaziUpit` i dalje nije port.