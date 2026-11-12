# Izgled: Carpo ljuska, NetGen logika

Carpo.NetGen je Carpo prozor. Izgled vuče na Carpo. Podaci, naredbe i SQL ostaju NetGen. XML se ne mijenja.

Janus kontrole (grid, stablo) zamjenjuju se DevExpressom. Sve ostalo što se vidi također je DevExpress, a gdje Carpo već ima komponentu, baznu formu, ribbon ili editor, koristi se taj, ne kopija.

## Checklist

- [x] Podloga nasljeđuje `RibbonBase.RibbonBase` (`RibbonBase.v26`), isti ribbon kao Carpo forme (`Carpo.UI.Forms.Jupiter.Base`).
- [x] Skin: `BonusSkins.Register` i `SkinManager.EnableFormSkins`, kao `Base.Start`.
- [x] Gumbi `CMDx` / `BUTTONS` su `BarButtonItem` u grupi Naredbe, istim redom i natpisom. Poznate riječi (unesi, izmjeni, obriši, ispis, osvježi) dobiju DevExpress sliku Add/Edit/Delete/Print/Refresh.
- [x] Master i detail su `GridControl` / `GridView`. Auto-filter red je uključen kao u Carpu. Širina, skriveni stupac, sort, zabrana izmjene i checkbox selekcija ostaju NetGen.
- [x] Stablo je `TreeList`. Combo na podlozi je `LookUpEdit`.
- [x] Dijalozi su `XtraForm`. Editor po tipu: `TextEdit`, `SpinEdit`, `ButtonEdit`, `DateEdit`, `LookUpEdit`, `CheckedComboBoxEdit`, `CheckEdit`, `CheckEdit` s `RadioGroupIndex` (svaki RADIO drži svoj `TOP`/`LEFT`), `SimpleButton`, `LabelControl`, `GroupControl`, `PanelControl`, `PictureEdit`, `GridControl`, `TreeList`. Raspored je NetGen `SrediKontrolu` (`/ 14`).
- [x] Status na dijalogu je `LabelControl` na dnu. WebBrowser ostaje WinForms jer DevExpress nema preglednik.
- [ ] Klik gumba na dijalogu još ne vrti lanac (lanac je u `Lanac`, kuka ga već pokriva). Combo i grid na dijalogu ne šalju vlastiti SQL pri otvaranju.

## Što se ne dira

Lanac komandi, parametri, redoslijed, što ide u bazu, koji se redovi prikažu. Funkcionalni defaulti grida (širina, ID stupac skriven, `NoEdit`, više redova, checkbox stupac).
