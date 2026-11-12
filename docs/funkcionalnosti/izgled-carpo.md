# Izgled: Carpo ljuska, NetGen logika

Carpo.NetGen je Carpo prozor. Izgled vuče na Carpo. Podaci, naredbe i SQL ostaju NetGen. XML se ne mijenja.

Janus kontrole (grid, stablo) zamjenjuju se DevExpressom. Sve ostalo što se vidi također je DevExpress, a gdje Carpo već ima komponentu, baznu formu, ribbon ili editor, koristi se taj, ne kopija.

## Checklist

- [x] Podloga nasljeđuje `RibbonBase.RibbonBase` (`RibbonBase.v26`), isti ribbon kao Carpo forme (`Carpo.UI.Forms.Jupiter.Base`).
- [x] Skin: `BonusSkins.Register` i `SkinManager.EnableFormSkins`, kao `Base.Start`.
- [x] Gumbi `CMDx` / `BUTTONS` su `BarButtonItem` u grupi Naredbe, istim redom i natpisom. Poznate riječi (unesi, izmjeni, obriši, ispis, osvježi) dobiju DevExpress sliku Add/Edit/Delete/Print/Refresh.
- [x] Master i detail su `GridControl` / `GridView`. Auto-filter red je uključen kao u Carpu. Širina, skriveni stupac, sort, zabrana izmjene i checkbox selekcija ostaju NetGen.
- [x] Stablo je `TreeList`. Combo na podlozi je `LookUpEdit`.
- [ ] Dijalozi INSERT/UPDATE/DELETE: `XtraForm` ili Carpo bazna dijalog forma, editori `TextEdit`, `SpinEdit`, `LookUpEdit`, `DateEdit`, `CheckEdit`, `RadioGroup`, `ButtonEdit`, `SimpleButton`. Raspored `TOP`/`LEFT`/`WIDTH`/`HEIGHT` ostaje, uz skaliranje.
- [ ] Statusna traka, label, group/panel, webbrowser i picturebox: DevExpress ekvivalent (`RibbonStatusBar`, `LabelControl`, `GroupControl`, `PanelControl`, `WebBrowser` samo ako DX nema bolji, `PictureEdit`).

## Što se ne dira

Lanac komandi, parametri, redoslijed, što ide u bazu, koji se redovi prikažu. Funkcionalni defaulti grida (širina, ID stupac skriven, `NoEdit`, više redova, checkbox stupac).
