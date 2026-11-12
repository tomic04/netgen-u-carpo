# KUPDostKnNGBvg — oba Carpo adaptera

XML: `definitions/netgen/BIOVEGA/KUPDostKnNGBvg.xml`. Jedini fajl s `clsCarpoKupUra` i s gumbom koji sam zove `ASSEMBLYLOAD`.

## Forme

| Element | `ONLOADED` / klik | `CarpoForm` |
|---|---|---|
| `ADAPTER_FORM` | `ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok` | `KnjigaUra.frmKnjigaUraInsert.xml` |
| `MULTISELECT_ADAPTER_FORM_KUPURAEDIT` | `ASSEMBLYLOAD(NetGenAdapter.clsCarpoKupUra)#ok` | `KnjigaUra.frmKnjigaUraKupEdit.xml` |
| `ADAPTER_FORM_KUPURA` | `ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok` | `KnjigaUra.frmKnjigaUraKupInsert.xml` |
| `ADAPTER_FORM_KUPOdobravanje` | SQL + `raiserror … +close`, ne assembly | `KUP_Odobravanje.xml` |

Gumb `Odobravanje` na zadnjoj formi: `ONCLICK=" ASSEMBLYLOAD(NetGenAdapter.clsCarpoKnjigaUra)#ok"`. To je `Rafalno` dijaloga (`Form2.vb` 7424), ne `Form_Load`. Vodeći razmak ne smeta `Contains`. `#ok` zatvori formu.

Ime `MULTISELECT_…` na ulazu u `NovaGenerickaForma2` počinje s `multiselect` (`Form1.vb` 4932). Prije forme briše se `TotalGenMultiSelectTemp` po suseru i po operateru. To nije isto što i punjenje iz čekiranih redova na gumbu koji se zove `MULTISELECT…`.

`ONLOADED` s kontrolom ide kroz `Form2_Load` (`5796`), ne kroz `2798`.

## Rupe

- `clsCarpoKupUra` i `clsCarpoKnjigaUra` dijele potpis `totalgen`. Razlikuje se `CarpoForm`.
- Odobravanje ne smije pokrenuti `ASSEMBLYLOAD` iz `ONLOADED`. Tamo je SQL i `+close`. Assembly je na gumbu.
- `KUP_Odobravanje.xml` nije `KnjigaUra.frmKnjigaUraInsert.xml`.
