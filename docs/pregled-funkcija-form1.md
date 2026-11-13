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

Broj u bloku: 50. nedostajalo-portano 28. isto 7. razlika 5. još nedostaje 10.

Zeleni run bloka 1: `3ad0d66`, https://github.com/tomic04/netgen-u-carpo/actions/runs/43366476116

## Blok 2, funkcije 51–100

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 51 | NapuniDrvoIzXMLA | 4217 | `Form1Ulaz.PutMeniXml`, `MeniIzXml` | nedostajalo-portano | F1-51 |
| 52 | AddNode | 4259 | `Form1Ulaz.MeniIzXml` | nedostajalo-portano | F1-51 |
| 53 | NapuniDrvoVrijeme | 4296 | `Stablo.IzTeksta` | razlika R01, R10, R100 | Stablo |
| 54 | DazovaIzStore | 4435 | `Stablo.IzStore` | isto | Stablo |
| 55 | AddNodes | 4492 | `Stablo.DodajDjecu`, id `0` se preskače | isto | Stablo |
| 56 | DazovaVremenska | 4527 | `Stablo.GodinaMjesec` 2005–2009 | isto | Stablo |
| 57 | NovaGenerickaForma | 4543 | `GumbTok`, `Podloga.IzvrsiLanac` | razlika R20, R22, R35 | GumbTok |
| 58 | NovaGenerickaForma2 | 4925 | `Dijalog` | razlika R19 | Dijalog |
| 59 | NovaGenerickaForma1 | 5062 | `Podloga`, `Dijalog` | razlika R19, R54 | DijalogTok |
| 60 | GetAllChildren | 5544 | `Form1Ulaz.NadjiPoTekstu` | nedostajalo-portano | F1-51 |
| 61 | TraziNoduDrvetupoTagu | 5555 | `Form1Ulaz.NadjiPoTekstu` | nedostajalo-portano | F1-51 |
| 62 | GetID | 5566 | `Form1Ulaz.Id`. `MasterGrid.Fokus` i dalje uzima stupac 0 | razlika R07 | F1-51 |
| 63 | GetDXID | 5588 | `Form1Ulaz.DxId` | nedostajalo-portano | F1-51 |
| 64 | GetDetID | 5605 | `Form1Ulaz.Id` | nedostajalo-portano | F1-51 |
| 65 | GetDetDXID | 5625 | `Form1Ulaz.DxId` | nedostajalo-portano | F1-51 |
| 66 | NovaApplikacija | 5638 | ponovno čitanje XML-a | još nedostaje | |
| 67 | TreeView1_AfterSelect | 5647 | zove `NovaApplikacija(Tag)` | još nedostaje | |
| 68 | TreeView2_DoubleClick | 5656 | `Form1Ulaz.RastaviTag` | nedostajalo-portano | F1-51 |
| 69 | Button1_Click | 5664 | `GumbTok` | isto | GumbTok |
| 70 | DataGridView1_DoubleClick | 5670 | `Podloga` `MasterDblClick` | isto | GranulaIzvrsenje |
| 71 | DataGridView1_MouseUp | 5674 | desni klik otvara izbornik | razlika R59 | |
| 72 | ToolStripMenuItem1_Click | 5680 | izbornik mastera | još nedostaje | |
| 73 | ToolStripMenuItem2_Click | 5768 | izbornik detaila | još nedostaje | |
| 74 | Button2_Click | 5862 | `GumbTok` | isto | GumbTok |
| 75 | Button3_Click | 5868 | `GumbTok` | isto | GumbTok |
| 76 | Button4_Click | 5873 | `GumbTok` | isto | GumbTok |
| 77 | Button5_Click | 5878 | `GumbTok` | isto | GumbTok |
| 78 | Button6_Click | 5883 | `GumbTok` | isto | GumbTok |
| 79 | TreeView1_KeyDown | 5888 | Ctrl+F11, isto kao `Form1Ulaz.Tipka` | isto | F1-25 |
| 80 | TreeView2_KeyDown | 5900 | Ctrl+F11 | isto | F1-25 |
| 81 | IspisNovi | 5910 | stari Janus ispis, u kodu iza DX grane | razlika R41 | |
| 82 | IspisNoviDet | 5953 | isto, detail | razlika R41 | |
| 83 | IspisGRIDREPX | 5984 | `PrintBridge` master | razlika R41, R42 | F1 ispis |
| 84 | Button7_Click | 6009 | zaglavlje ispisa | razlika R41 | IspisZaglavlje |
| 85 | IspisGRIDREPXdet | 6067 | `PrintBridge` detail | razlika R41 | |
| 86 | Button77_Click | 6091 | detail ispis | razlika R41 | |
| 87 | Button10_Click | 6140 | prazan poziv | isto | |
| 88 | ApperianceDialogToolStripMenuItem_Click | 6146 | Janus AppearanceDialog | razlika R07 | |
| 89 | FormatStylesToolStripMenuItem_Click | 6154 | Janus FormatStyles | razlika R07 | |
| 90 | TableViewToolStripMenuItem_Click | 6161 | Janus View.TableView | razlika R07 | |
| 91 | CardViewToolStripMenuItem_Click | 6169 | Janus CardView | razlika R07 | |
| 92 | SingleCardViewToolStripMenuItem_Click | 6177 | Janus SingleCardView | razlika R07 | |
| 93 | SaveGridToolStripMenuItem_Click | 6186 | zove `SaveGridLayout` | nedostajalo-portano | F1-51 |
| 94 | LoadLayout | 6192 | čitanje .gxl | još nedostaje | |
| 95 | LoadLayout2 | 6228 | čitanje .xml rasporeda | još nedostaje | |
| 96 | SaveGridLayout | 6246 | `Form1Ulaz.DatotekaRasporeda`. Upis Janus/DX datoteke | još nedostaje | F1-51 |
| 97 | LoadLayoutTEMPORARNI | 6312 | privremeni .gxl | još nedostaje | |
| 98 | SaveGridLayoutTEMPORARNI | 6328 | privremeni .gxl | još nedostaje | |
| 99 | DeleteGridLayout | 6344 | brisanje rasporeda | još nedostaje | |
| 100 | SaveGridLayoutDet | 6386 | detail raspored | još nedostaje | |

Broj u bloku: 50. nedostajalo-portano 9. isto 13. razlika 17. još nedostaje 11. Zbroj 50.

## Blok 3, funkcije 101–150

Prazno tijelo je isto. Portana su pravila koja su imala tijelo, a nisu bila u Carpu.

| # | Funkcija | Linija | Carpo | Status | Test |
| --- | --- | --- | --- | --- | --- |
| 101 | DeleteGridLayoutDet | 6410 | | još nedostaje | |
| 102 | Button8_Click | 6432 | `Form1Ulaz.SljedeciRastvoreno` | nedostajalo-portano | F1-107 |
| 103 | ResetGridLayoutToolStripMenuItem_Click | 6444 | | još nedostaje | |
| 104 | GridExportToolStripMenuItem_Click | 6453 | | još nedostaje | |
| 105 | GridEX1_CellEdited | 6466 | prazno | isto | |
| 106 | GridEX1_CellUpdated | 6471 | `Form1Ulaz.VrijednostCelije`. Petlja parametara je `PokupiParametreSaGrida` | nedostajalo-portano | F1-107 |
| 107 | Rafalno | 6546 | `Form1Ulaz.SegmentRafala` i `Lanac` | nedostajalo-portano | F1-107 |
| 108 | GridEX1_Click | 6912 | | još nedostaje | |
| 109 | GridEX1_DoubleClick | 6935 | `Podloga` dvostruki klik | isto | |
| 110 | GridEX1_GroupsChanged | 6941 | | još nedostaje | |
| 111 | ContextMenuStrip3_Click | 6952 | prazno | isto | |
| 112 | GridEX1_MouseUp | 6978 | | još nedostaje | |
| 113 | GridEX1_KeyDown | 7015 | | još nedostaje | |
| 114 | GridEX2_CellUpdated | 7063 | ista vrijednost ćelije | nedostajalo-portano | F1-107 |
| 115 | GridEX2_Click | 7130 | | još nedostaje | |
| 116 | GridEX2_KeyDown | 7145 | | još nedostaje | |
| 117 | GridEX1_RecordUpdated | 7176 | | još nedostaje | |
| 118 | TreeView2_AfterSelect | 7182 | `Form1Ulaz.RastaviTag`, zatim `NovaGenerickaForma` | nedostajalo-portano | F1-51 |
| 119 | TreeView2_NodeMouseClick | 7191 | prazno | isto | |
| 120 | GridEX2_DoubleClick | 7201 | | još nedostaje | |
| 121 | GridEX2_MouseUp | 7206 | | još nedostaje | |
| 122 | ContextMenuStrip3_Closed | 7229 | prazno | isto | |
| 123 | ContextMenuStrip3_Closing | 7233 | | još nedostaje | |
| 124 | ContextMenuStrip3_ItemClicked | 7241 | | još nedostaje | |
| 125 | ContextMenuStrip4_Closing | 7277 | | još nedostaje | |
| 126 | ContextMenuStrip4_ItemClicked | 7284 | | još nedostaje | |
| 127 | MenuStrip1_ItemClicked | 7321 | | još nedostaje | |
| 128 | Timer1_Disposed | 7332 | prazno | isto | |
| 129 | Timer1_Tick | 7336 | | još nedostaje | |
| 130 | ToolStripStatusLabel3_Click | 7340 | | još nedostaje | |
| 131 | TimerOff | 7348 | `Form1Ulaz.TimerUpaljen` prazan | nedostajalo-portano | F1-107 |
| 132 | TimerOn | 7355 | `Form1Ulaz.TimerUpaljen` | nedostajalo-portano | F1-107 |
| 133 | Button9_Click | 7396 | zove `DeleteGridLayout` | još nedostaje | |
| 134 | TreeView2_BeforeSelect | 7401 | | još nedostaje | |
| 135 | TreeView2_LostFocus | 7407 | | još nedostaje | |
| 136 | GridEX1_SelectionChanged | 7413 | zove `PokaziUpitDetalj` | još nedostaje | |
| 137 | GridEXPrintDocument1_PrintPage | 7420 | | još nedostaje | |
| 138 | ButtonZaDrvo_Click_1 | 7426 | | još nedostaje | |
| 139 | TreeView2_KeyDown1 | 7440 | | još nedostaje | |
| 140 | CustomFormatKolona | 7451 | | još nedostaje | |
| 141 | Button7_Click_1 | 7650 | | još nedostaje | |
| 142 | Button8_Click_1 | 7655 | | još nedostaje | |
| 143 | Button9_Click_1 | 7660 | | još nedostaje | |
| 144 | Button10_Click_1 | 7665 | | još nedostaje | |
| 145 | butonzarazgrupiranje_Click | 7670 | | još nedostaje | |
| 146 | Form1_LocationChanged | 7684 | prazno | isto | |
| 147 | Form1_LostFocus | 7688 | | još nedostaje | |
| 148 | Form1_Resize | 7692 | | još nedostaje | |
| 149 | Form1_ResizeEnd | 7703 | prazno | isto | |
| 150 | Form1_SizeChanged | 7709 | | još nedostaje | |

Broj u bloku: 50. nedostajalo-portano 7. isto 8. razlika 0. još nedostaje 35. Zbroj 50. Zbroj 50.
