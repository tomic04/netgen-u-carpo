# MPBlagajna — `PRINTMASTER` i `vrstadokid-`

XML: `definitions/netgen/BIOVEGA/MPBlagajna.xml`.

## Ispis liste

`CMD5` je `PRINTMASTER`. `NovaGenerickaForma2` vidi prefiks i zove `IspisGRIDREPX`, pa `Exit Sub` (`Form1.vb` 4951–4955). `PRINTREPXMASTER` je ista funkcija (`4945–4948`). Janus `Button7_Click` je komentiran.

`IspisGRIDREPX` (`5984–6003`):

- učita `NetGenExtensions.frmGridExPrintingREPX`
- zaglavlje iz `PAGEHEADERLEFT/CENTER/RIGHT`; `##` i `!!` su novi red
- prazan centar reže `Me.Text` do `v[` (`5996–5997`). Ovdje je `PAGEHEADERCENTER=""`, a `VERSION` nema, pa `IndexOf("v[")` baci i OERN ostavi centar prazan
- `PAGEHEADERLEFT="Trgovina: @@trgovina"`
- `PrintDocumentREPX("master", GridEX1, …, strGlobalSQLSERVERlogin, ParametriSaGrida, "")`
- `PAGEORIENTATION` se ovdje ne čita

## Ispis dokumenta

`CMD4` je `MULTISELECTISPIS_FORM`. Ime počinje s `MULTISELECT`, pa se prije forme čisti `TotalGenMultiSelectTemp` (`4932–4936`).

Forma nema `INPUT`. `BrojKontrola = 0`, pa `ONLOADED` ide odmah (`Form2.vb` 2798) i `ShowDialog` se ne zove (`4430`).

`ONLOADED=":vrstadokid-mpblagajnagla"`. Vodeća `:` u `Rafalno` otvara `NovaGenerickaForma2` bez dvotočke (`6882–6883`). `vrstadokid-` nije forma: `PrintRpt` (`Class1.vb` 443–462) pokuša `Spin.Printing.v23`. Ako `SpinPrintingDizePopUp` prođe, zove se `IspisiMePrekoSpinPrintinga`. Inače SQL čita `templates` iz `vrstadok` (`471–537`).

`IspisiMePrekoSpinPrintinga` (`297–304`) učita v23, u `Catch` v9.1. Klasa je `Spin.Printing.Start`. Ako parametar već ima `@id=`, ne dodaje drugi (`320–327`). Inače pred svaki id doda `@id=<vrijednost>,`. Više od jednog id-a zove `PrintMulti`, inače `Print`, s hwnd. Pad s hwnd pokuša ponovo bez hwnd (`362–369`).

Port: `Carpo.NetGen.Print.PrintBridge` je v26 i `rptDevRepx` (`Print/PrintBridge.cs` 20–21). v23 i v9.1 se ne zovu (`docs/analiza/00-sazetak.md`).

## Rupe

- `PRINTMASTER` i `vrstadokid-` nisu isti ispis. Prvi je REPX grida, drugi je predložak vrste dokumenta.
- Prazna forma ne smije otvoriti dijalog, ali mora odraditi `ONLOADED`.
- `MULTISELECT` u imenu forme briše temp tablicu i kad je posao ispis.
- v26 nije ono što ovaj VB učitava. Potpis `Print`/`PrintMulti` mora ostati, assembly ne.
