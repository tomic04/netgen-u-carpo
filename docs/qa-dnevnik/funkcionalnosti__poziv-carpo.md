# Poziv Carpo subforme iz NetGena

Zaseban element, **nakon stabla i master-detaila**. Isti XML ostaje. Carpo.NetGen ga izvršava u istom procesu (isti solution), ne kao vanjski `Assembly.Load` tuđeg DLL-a. Ovaj dokument je checklista, ne implementacija.

U korpusu (633 XML-a) Carpo se ne zove kao `Carpo.Gen.Start`. Zove se adapter `NetGenAdapter`, a skriveni `INPUT` `CarpoForm` nosi ime Carpo XML-a.

## Pozivi u korpusu

8 pogodaka, 4 datoteke. Ostali `ASSEMBLYLOAD` (DMS, Excel, PDF) nisu ovaj element.

| Klasa | Pogodaka | Datoteke |
|---|---|---|
| `NetGenAdapter.clsCarpoKnjigaUra` | 7 | `Agronom granule/KUPDostKN.xml`, `Agronom granule/KUPNG.xml`, `BIOVEGA/KUPDostKnNGBvg.xml`, `BIOVEGA/KUPNGNGBvg.xml` |
| `NetGenAdapter.clsCarpoKupUra` | 1 | `BIOVEGA/KUPDostKnNGBvg.xml` |

Izvornik adaptera **nije u ovom repou**. Vidljiv je samo NetGen ugovor i polje `CarpoForm`.

Primjer (`KUPNG.xml`): gumb `Proknjiži u URU Carpo:ADAPTER_FORM~REFRESHMASTERGRID`. Forma:

```xml
<ADAPTER_FORM ... ONLOADED="ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok" FILLEXPLORER="TRUE">
  <INPUT TYPE="text" NAME="CarpoForm" ... DEFAULT="KnjigaUra.frmKnjigaUraInsert.xml" VISIBLE="false"/>
</ADAPTER_FORM>
```

`~` u naredbi gumba postaje `#` prije routera.

## Kako NetGen zove

`ASSEMBLYLOAD(ImeAssembly.ImeKlase)`. Assembly je sve lijevo od zadnje točke, `CreateInstance` cijelog imena, zatim late-bound `totalgen` s pet argumenata. Nema COM `ProgId` na ovim XML-ovima (točkasti `CreateObject` u `Form1.vb` 5254 je drugi put i u korpusu ne zove Carpo).

| Mjesto | Argumenti `totalgen` |
|---|---|
| Podloga, `NovaGenerickaForma` `Form1.vb` 4584–4623 | `id`, `operater`, konekcija, zadnji SQL mastera, `ParametriSaGrida` |
| Podloga, `:ASSEMBLYLOAD` `Form1.vb` 6602–6661 | isto, uz CB log prije i poslije |
| Podloga, `Rafalno` `Form1.vb` 6681–6727 | `id`, `operater`, konekcija, zadnji SQL mastera, `""` |
| Dijalog `Form2.vb` 7424–7456 | `id`, `operater`, konekcija, `strParametriSvi`, `strParametriSvi` |

Adapter forme su dijalog, pa vrijedi zadnji red. `strParametriSvi` uključuje kontrole forme, među njima `CarpoForm=<DEFAULT>`.

- **Konekcija.** `Conn.ConnectionString`. Ako ne sadrži `driver`, prefiks je `Driver={SQL Server};`.
- **Id i operater.** `GetID()` (podloga: kolona `id`, inače Janus ćelija 2) i `cls1.OperateriID`.
- **Prozor.** `totalgen` ne dobiva hwnd. Nema `SetParent`. Dijete je u istom procesu (`Assembly.Load`).
- **Povrat.** `totalgen` se ne sprema. `o.id` se čita samo na `CreateObject` putu (`Form1.vb` 5275–5278), ne na `ASSEMBLYLOAD`.
- **Greška.** `Load` ili `CreateInstance` padne: CB log i `Exit Sub` (ostatak `#` lanca staje).
- **Nakon uspjeha.** Lanac ide dalje. `#ok` zatvara NetGen dijalog. `REFRESHMASTERGRID` na gumbu ponovo šalje zadnji SQL mastera.
- **`ONLOADED`.** `Form2.vb` 2798 zove `strONLOADED` samo kad je `BrojKontrola = 0`. Adapter forma ima jedan `INPUT`, pa ta grana ne radi i forma se prikaže (`4430`). `Form2_Load` (`5796`) ipak zove isti `strONLOADED` kad se dijalog otvori (VB ne razlikuje velika slova u imenu). `ASSEMBLYLOAD#ok` na `ONLOADED` zato ide nakon prikaza, ne u grani s nula kontrola. Forma bez ijednog `INPUT` (npr. ispis `vrstadokid-`) ide samo kroz `2798` i `ShowDialog` se preskoči.

## Checklist (kad element dođe na red)

- [ ] Isti zapis `ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)` i `clsCarpoKupUra` radi bez izmjene XML-a.
- [ ] Pet argumenata ostaje kako tablica gore kaže, uključujući prazan zadnji argument na `Rafalno` podloge i `strParametriSvi` na dijalogu.
- [ ] `CarpoForm` iz `DEFAULT` dođe do Carpo XML-a (`KnjigaUra.frmKnjigaUraInsert.xml` i ostali iz ta četiri fajla).
- [ ] Poziv je unutar procesa: `Carpo.Gen` / postojeći `Run`, ne novi `Assembly.Load` i ne drugi Jupiter.
- [ ] Konekcija i operater su isti kao granule. `jupiter2022` se ne otvara.
- [ ] Greška učitavanja prekida lanac i ostavlja poruku, forma se ne ruši cijela.
- [ ] `#ok` zatvori NetGen omotač. `REFRESHMASTERGRID` iza toga osvježi master već zamijenjenim SQL-om.
- [ ] Povratna vrijednost `totalgen` se i dalje ne čita.
- [ ] `ONLOADED` i dalje ne krene sam kad forma ima kontrolu.
