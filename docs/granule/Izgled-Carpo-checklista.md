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

## Stanje porta (#33, `cursor/netgen-raspored-3266` `4267122`)

`Podloga` nasljeđuje `Carpo.UI.Forms.Master.Manager`. Ovo odgovara checklisti:

- Ribbon stranice su obrisane, zaglavlje stranica je skriveno (`Podloga.cs` 48–54). Gumbi granule su `SimpleButton` na `pnlButtons`, ne velike ikone ribbona.
- Lijevo su `bbiLookUpFilter` / `cmbComboEditFilter` (`RepositoryItemComboBox`) i `trMain`. Prazan `TREE` i prazan combo sklone lijevu stranu (`OnShown` 165–166, `ShellLayout.cs` 56–63).
- `bar2` ostaje iz dizajnera: sakrij bočnu traku, raširi, reset, postavke, izvoz, ikone 16 px (`Manager.Designer.cs` 381–400).
- `splitGrid.Horizontal = false` (`645`). Detail je ispod mastera.
- `pnlButtons` je `Dock = Bottom`, visina 42, na desnom panelu (`709–719`).
- `bbiBrojSlogova.Caption` je `Broj slogova:` (`Podloga.cs` 72).

Ovo ne odgovara:

- Klik gumba izvrši samo `REFRESHMASTERGRID`. Ostale NetGen naredbe ostanu u `Tag` i uđu u dnevnik (`Podloga.cs` 451–460). Checklista traži da naredba gumba bude NetGen `CMD` / `BUTTONS`.
- Broj detalja u statusu ostane 0 nakon prvog punjenja. `PostaviStatus` ide iz mastera prije `OsvjeziDetail`, a vezanje detalja ne ponovi status (`319–326`, `385–390`, `464–470`).
- Kartica je uvijek jedna, natpis `Stavke` (`Podloga.cs` 62–69). NetGen skine DMS karticu samo kad je `DMSDET` prazan (`Form1.vb` 2433–2437). Port je ne doda ni kad `DMSDET` ima SQL.
- Uključena kvačica ide u `CheckBoxRowSelect` (`379–380`). NetGen sakrije ili pokaže stupac 1 (`Form1.vb` 2043–2046). `CHECKBOXVISIBLE=FALSE` ne doda selektor, pa uobičajeni slučaj izgleda ugašeno.

## Što port ne smije

- Staviti NetGen gumbe u ribbon stranicu s velikim ikonama.
- Sakriti lijevi combo i stablo kad XML ima `TREE` ili `COMBO`.
- Zamijeniti donji red gumba alatnom trakom grida.
- Izostaviti status `Broj slogova` na masteru.
