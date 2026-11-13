# Razlike NetGen / Carpo

Pregled stavku po stavku. Port koji prati NetGen nije rupa. Ostale stavke su uklonjene jer ih `Carpo.NetGen` već radi, uz test u `Carpo.NetGen.GridTest` ili `tests/Carpo.NetGen.Granule`.

## Neizbježno

### R59 Izgled je DevExpress

- **NetGen:** podloga je WinForms `Form1`. Gumbi stoje na dnu (`Form1.vb` 3312, 3827–3848). Master je Janus kad `DXGRID` nije true (`Form1.vb` 1906–1908): `HeaderLines`, prazni retci, row header (`Form1.designer.vb` 640–655). Čvor crta `ImageList2` indeks 0 (`575–579`).
- **Zašto ostaje:** kontrole su DevExpress. Boja, stil gumba, položaj dolje, prazni retci, row header, visina zaglavlja, font, obrub i slika čvora se ne crtaju kao Janus. Natpis, redoslijed, naredba, širina, skriveni stupac, sort, edit, kvačica, raspored dijaloga, tab, enable i fokus i dalje prate NetGen.
- **Test:** `OKL-38`

### R109 Prazan CIS URL ne postaje produkcija

- **NetGen:** prazan URL postane `https://cis.porezna-uprava.hr:8449/FiskalizacijaService` (`Form1.vb` 377).
- **Zašto ostaje:** Carpo taj URL ne šalje. SOAP ide samo na testni CIS. Ostali Raverus koraci su isti (`GenerirajUuid`, zaštitni kod, `PosaljiSoapPorukuCertifikatDatoteka`, `DohvatiJir`).
- **Test:** `fiskal-izvrsi-test`, `fiskal-uuid-spin`

### R110 `PrintLink` nema tri NetGen zaglavlja

- **NetGen:** `IspisGRIDREPX` šalje `"master"`, `"detail"` ili `"lodani"` i `PageHeaderLeft` / `Center` / `Right` (`Form1.vb` 5995–6003, `Form2.vb` 9049–9056).
- **Zašto ostaje:** `Spin.Printing.Start.PrintLink` prima `caption`, `header`, `subHeader` i `isLandscape`. Nema tri svojstva `PageHeader*` ni riječi `lodani`. Centar ide u `caption`, lijevo u `header`, desno u `subHeader`.
- **Test:** `print-master`, `print-lodani`

### R85 `spMPTrgovineNG` nema `@operateriid`

- **NetGen:** šalje `@operateriid` (`Form1` parametar operatera).
- **Zašto ostaje:** XML se ne mijenja. Procedura na spinu nema taj parametar. Isti tekst bi pao i u NetGenu.

### R86 `MPKasaPolog` ima dvostruke stupce

- **NetGen:** SQL ima `odsat`, `odmin`, `dosat`, `domin` dvaput.
- **Zašto ostaje:** XML se ne mijenja. Spin javi `Ambiguous column name`. Isti tekst.

### R87 `oznaka2` nije stupac

- **NetGen:** `OznakaKL` čita `klmasterroba.oznaka2`.
- **Zašto ostaje:** XML se ne mijenja. Ako stupca nema, `ExecSql` padne i grid ostane prazan (`Form1` punjenje mastera).

### R88 Prazan master je podatak

- **NetGen:** prazan rezultat ne otvara drugi upit.
- **Zašto ostaje:** Carpo šalje isti predložak. Prazno je podatak ili čekanje na čvor, ne drugačiji SQL.

### R89 Jupiter ne izvršava proceduru koja piše

- **NetGen:** `JeZabranaPisanja` gleda prvu riječ (`Class1`). `spKntZakljucivanjeTem @action=1` nije `insert`, pa bi se poziv poslao. Tijelo piše u trajnu tablicu.
- **Zašto ostaje:** smoke na `jupiter2019` i `jupiter2022` ne pokreće proceduru čije tijelo piše. XML i Jupiter se ne diraju.
- **Test:** `zastita-zakljucivanje`

### R101 i R102 Spin nema prava tijela

- **NetGen:** zove proceduru iz XML-a. Ako je nema, padne.
- **Zašto ostaje:** na spinu je stub `SELECT … WHERE 1=0` dok ne stignu prava tijela. Jupiter se ne čita. Popis je u `docs/granule/spin-objekti.md`.

## Uklonjeno

95 stavki je bilo u registru. Devet gore ostaje. Ostale su uklonjene jer port prati NetGen:

| Stavke | Gdje |
|---|---|
| Naslov ` v[verzija]` | `NetGenXmlReader.NaslovIzCaptiona`, `Form1.vb` 3524–3525 |
| Dva G u agregatu se ne čitaju, `"0"` se čita | `NetGenXmlReader` 194–197, `MasterGrid.cs` 447–448, `Form1.vb` 7496–7506 |
| Statički čvor stabla je susjed | `Stablo.cs` 134–142, `Form1.vb` 4347–4356 |
| `SETDEFAULTS` reže na `#` | `Rijetke.SetDefaults`, `Form1.vb` 3862–3866 |
| Prazan broj je `0` | `Ponasanje.BrojParametar`, `Form2.vb` 3601 |
| `+focus` i `spRaiserror` | `Lanac.cs`, `Class1.vb` 187–204, `Form2.vb` 7511–7515 |
| `~` postaje `#` | `Lanac.Segmenti`, `Form1.vb` 4558 |
| Combo stupac 0/1, `ALLOWEMPTY` samo `true` | `ComboDijalog.cs`, `Form2.vb` 2402–2431 |
| `TYPE` je `InStr` | `NgGrana.cs`, `Form2.vb` 1256–2707 |
| Tipfeler se ne ispravlja | `Ponasanje`, `Form2.vb` 2252, 2403, 2949 |
| `firstday*`, `yearmonth` samo na podlozi, `TABINDEX` s razmakom | `Ponasanje.DatumPocetni`, `Stablo.GodinaMjesec`, `Raspored.cs` |
| Stablo stupci 0–3, id 0 se preskače | `Stablo.cs` 241–295, `Form1.vb` 4450–4498 |
| Dijalog lanac, combo, grid, `NEXTFORM`, širina `/14`, `Ime.Start` nije DLL | `Dijalog.cs`, `DijalogTok.cs`, `MasterGrid.VbJedinica`, `Form2.vb` 1052, 1635–1646, 5445–5452 |
| `REFRESHMASTER`, `FOREACH` na podlozi, `OKSTAY`, `CLOSEPARENT`, `BUTTONS`, `CLICK` | `docs/naredbe-stanje.md` |
