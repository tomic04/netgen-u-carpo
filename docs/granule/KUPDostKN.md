# KUPDostKN — shell `ASSEMBLYLOAD` i popup s tildom

XML: `definitions/netgen/Agronom granule/KUPDostKN.xml`.

## Tri puta

`MASTERDBLCLICK="ASSEMBLYLOAD(KUPPreglednikDMS.ScanPreview)"`. Nema tildu. Dvostruki klik ide u `NovaGenerickaForma` (`Form1.vb` 4584–4623): `totalgen(id, operater, konekcija, zadnji SQL mastera, ParametriSaGrida)`. Povrat se ne čita. `o.id` se čita samo na `CreateObject` (`5254–5278`).

`CMD10` je isti poziv, isto bez tildu.

Popup `Proknjiži u URU Carpo:ADAPTER_FORM~REFRESHMASTERGRID` ima tildu. `ToolStripMenuItem1_Click` zamijeni `~` i zove `Rafalno(":" + …)` (`5743–5757`), ne `NovaGenerickaForma`. Grana je `:ASSEMBLYLOAD` samo ako segment sam počinje tako. Ovdje prvi segment je `ADAPTER_FORM`, pa se otvori dijalog. Drugi je `REFRESHMASTERGRID`.

## Dijalog

`ADAPTER_FORM` `ONLOADED="ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok"`, `FILLEXPLORER="TRUE"`, jedan `INPUT` `CarpoForm` `DEFAULT="KnjigaUra.frmKnjigaUraInsert.xml"`.

`BrojKontrola` je 1 (`3069`, svaka kontrola, i nevidljiva). `2798` ne zove `ONLOADED`. `ShowDialog` (`4430`) digne `Form2_Load`, a on zove `strONLOADED` (`5796`). `totalgen` dobije `strParametriSvi`, `strParametriSvi` (`Form2.vb` 7424–7456). `#ok` zatvori omotač. `FILLEXPLORER` zatim prepiše master.

## Rupe

- Isti tekst `ASSEMBLYLOAD(...)` na dvostrukom kliku i na `ONLOADED` nema iste argumente (`Form1.vb` 4623` naspram `Form2.vb` 7456).
- Tilda na popupu bira `Rafalno`, ne `NovaGenerickaForma` (`5743`).
- `CarpoForm` mora stići u parametre dijaloga. Shell DMS poziv taj parametar nema.
