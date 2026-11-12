# Izgled: Carpo ljuska, NetGen logika

Carpo.NetGen je Carpo prozor. Izgled vuče na Carpo. Podaci, naredbe i SQL ostaju NetGen. XML se ne mijenja.

Janus kontrole (grid, stablo) zamjenjuju se DevExpressom. Sve ostalo što se vidi također je DevExpress, a gdje Carpo već ima komponentu, baznu formu, ribbon ili editor, koristi se taj, ne kopija.

## Odluka: Manager nije obrazac za svaku granulu

NetGen ima nekoliko custom obrazaca, sličnih jedan drugome. Carpo predložak nije cilj sam po sebi. Manager (stablo lijevo, master, detalj na kartici Stavke, gumbi dolje) ide samo na granule čiji XML stvarno ima lijevi panel (`TREE` ili `COMBO`), `DETAILGRID` i gumbe. Ostale zadržavaju NetGen raspored: pozicije, dock i paneli. Kontrole su i dalje DevExpress, skin je Carpo. Popis je `docs/granule/tipovi-formi.md`.

## Checklist

- [x] Podloga nasljeđuje `Carpo.UI.Forms.Master.Manager` samo kao ljusku. Kartica Stavke stavlja se samo kad je tip `manager`. Inače detail, ako ga XML ima, ostaje grid ispod mastera, a prazno stablo i prazan detail se sklope. Combo je `bbiLookUpFilter`, stablo `trMain`, master `gcGla`, gumbi u `pnlButtons`, status `Broj slogova`.
- [x] Skin: `BonusSkins.Register` i `SkinManager.EnableFormSkins`, kao `Base.Start`.
- [x] Gumbi `CMDx` / `BUTTONS` su flat gumbi u donjem `pnlButtons`, istim redom i natpisom. Ribbon stranica se ne puni.
- [x] Master i detail su `GridControl` / `GridView`. Auto-filter red je uključen kao u Carpu. Širina, skriveni stupac, sort, zabrana izmjene i checkbox selekcija ostaju NetGen.
- [x] Stablo je `trMain` na lijevoj strani Managera, combo je `bbiLookUpFilter`. Detail je kartica Stavke ispod mastera.
- [x] Dijalozi su `XtraForm`. Editor po tipu: `TextEdit`, `SpinEdit`, `ButtonEdit`, `DateEdit`, `LookUpEdit`, `CheckedComboBoxEdit`, `CheckEdit`, `CheckEdit` s `RadioGroupIndex` (svaki RADIO drži svoj `TOP`/`LEFT`), `SimpleButton`, `LabelControl`, `GroupControl`, `PanelControl`, `PictureEdit`, `GridControl`, `TreeList`. Raspored je NetGen `SrediKontrolu` (`/ 14`).
- [x] Status na dijalogu je `LabelControl` na dnu. WebBrowser ostaje WinForms jer DevExpress nema preglednik.
- [x] Filter forma je isti `XtraForm` sa skinom. `BORDERSTYLE=3` je fiksni dijalog. Combo, datum, broj, check i gumb nose Carpo default (null tekst `...`, maska datuma, broj desno, gumb 7 pt). Stupac grida crta `FORMAT` i poravnanje. Status ribbana pokazuje server, bazu i operatera.
- [x] Klik gumba na dijalogu vrti lanac (`OK`, `OKStay`, `CLOSE`). SELECT se šalje. Procedura koja piše se ne šalje.
- [x] Combo i grid na dijalogu pri otvaranju pošalju vlastiti `SELECT`. Poziv procedure čeka provjeru tijela.

## Što se ne dira

Lanac komandi, parametri, redoslijed, što ide u bazu, koji se redovi prikažu. Funkcionalni defaulti grida (širina, ID stupac skriven, `NoEdit`, više redova, checkbox stupac).
