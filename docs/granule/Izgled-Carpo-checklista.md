# Izgled podloge kao Manager ponuda

Namjerno odstupanje (R74). Prozor granule nasljeđuje `RibbonBase` iz DLL-a, a crta se kao tipična Carpo manager forma. Uzorak je **SPIN - Manager ponuda**. Nema Office ribbona s velikim ikonama. Ponašanje i XML ostaju NetGen.

## Lanac predloška

| Sloj | Gdje |
|---|---|
| XML uzorak | `definitions/carpo/AIOPonuda.xml`, forma `frmMaster`, `type="master"`, `caption="SPIN - Manager ponuda"` |
| Raspored u XML-u | `<managerlayout>` pa `<panelbuttons>` (`AIOPonuda.xml` 4–374 i 375–570) |
| Klasa | `Carpo.UI.Forms.Master.Manager` (`Devex_26/Carpo.UI/Forms/Manager/Manager.cs` 30) |
| Baza | `JupiterManager` → `JupiterShared` → `Carpo.UI.Forms.Jupiter.Base` → `RibbonBase.RibbonBase` (`JupiterManager.cs` 3, `JupitetShared.cs` 16, `Base.cs` 16) |
| Dizajner rasporeda | `Manager.Designer.cs` |

`Manager.SetManagerLayout` (`Manager.cs` 704–780) veže lookup, stablo, master grid i kartice iz `managerlayout`.

## Elementi i kako se provjeravaju

Provjera je usporedba NetGen podloge s ovim kontrolama. Boja, font i skin su izgled. Natpis gumba, redoslijed, naredba, širina stupca, skriveni stupac, sort, checkbox, enable i fokus ostaju NetGen.

| Element | Carpo | Provjera u portu |
|---|---|---|
| Nema Office ribbona | `ribbonControl1` dolazi iz `RibbonBase`, ali manager trake su `bar1` i `bar2` na `StandaloneBarDockControl`, ne stranica ribbona s velikim ikonama (`Manager.Designer.cs` 41–50, 303–318, 381–400) | Na podlozi nema vidljive ribbon stranice. Gumbi granule nisu `BarButtonItem` velike ikone. |
| Lijevo: combo i drvo | `splitMain.Panel1`: `standaloneBarDockControl1` s `bar1` (`bbiLookUpFilter` / `cmbComboEditFilter`), ispod `trMain` (`JTreeListControl`), splitter na 245 (`138–158`, `309–327`) | Combo filtera je lijevo gore. Stablo je ispod njega. Prazan `TREE` i prazan combo i dalje sakriju tu stranu (`Form1.vb` 4310–4316). |
| Master grid | `splitGrid.Panel1` → `gcGla` / `gvGla`, dock fill (`641–672`) | Master puni gornji desni dio. SQL, stupci i `@@id` ostaju NetGen. |
| Mala traka alata desno gore | `bar2` na `standaloneBarDockControl2`: sakrij bočnu traku, raširi, reset, postavke, izvoz. Ikone su 16 px (`388–423`) | Traka je iznad mastera, desno. Nije ribbon. |
| Kartice s detaljem ispod | `splitGrid` je vodoravan (`Horizontal = false`). `Panel2` je `gcDet`. U XML-u uzorka `<tabs>` s karticom Stavke (`AIOPonuda.xml` 234–239, `Manager.cs` 323, 709–729) | Detail je ispod mastera, u kartici. `DETAILGRID` puni tu karticu. `DMS` kartica se ne crta ako NetGen skine `DMS` (`Form1.vb` 2431–2436, implementacijski R64). |
| Veliki flat gumbi na dnu | `pnlButtons` (`JPanelButtonsControl`), `Dock = Bottom`, visina 42, bez obruba (`709–719`). XML `<panelbuttons>` (`AIOPonuda.xml` 375) | Jedan red gumba na dnu desnog dijela. Natpis i naredba su NetGen `CMD` / `BUTTONS`. Nisu u Office ribbonu. |
| Status `Broj slogova` | `bbiBrojSlogova.Caption = "Broj slogova:"` (`JupitetShared.Designer.cs` 122–126). Master i detail imaju `bbiBrojSlogovaMasterGrid` i `bbiBrojSlogovaDetailsGrid` (`Manager.cs` 507–517) | Status pokazuje broj redova mastera. Tekst počinje s `Broj slogova`. |

## Što port ne smije

- Staviti NetGen gumbe u ribbon stranicu s velikim ikonama.
- Sakriti lijevi combo i stablo kad XML ima `TREE` ili `COMBO`.
- Zamijeniti donji red gumba alatnom trakom grida.
- Izostaviti status `Broj slogova` na masteru.
