# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")` (`Form2.vb` 9044, 9065, 9088, `Form1.vb` 5988).

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | `PrintDocumentREPX` |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo radi isti `Assembly.Load("NetGenExtensions")`. Ako fusion ne nađe sklop, `AssemblyResolve` učita DLL iz Jupiter Clienta (uključujući podmape) i iz GAC mapa čije ime počinje s `Janus`. DLL se ne commita.

Na sql-ai `NetGenExtensions.dll` jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`. `Janus.Windows.GridEX.v3.dll` jest u `...\SharedAssemblies\Janus\`.

UI test na `tests/Ekstenzije.xml` zove sve tri metode. `OK` je samo kad `ChartGRID`, `PivotGRID` i `PrintDocumentREPX` stvarno prođu. Ako v12.1 datoteke nema na disku, ishod je `OPEN` (nije prolaz testa i nije pad grane).

Pretraga na sql-ai (GAC `C:\Windows\Microsoft.NET\assembly` i `C:\Windows\assembly`, sve mape `Jupiter Client`, `Program Files\DevExpress`, `SharedAssemblies`, backup mape, ostali diskovi osim `C:\Windows` koji je već prođen) obradila je 82933 mape. Ništa se ne kopira u repo i ništa se ne briše.

`PrintDocumentREPX` prolazi. `PivotGRID` i `ChartGRID` ostaju otvoreni. Na stroju jesu `DevExpress.XtraReports.v12.1.dll`, `DevExpress.XtraReports.v12.1.Extensions.dll`, `Janus.Windows.GridEX.v3.dll` 3.5.131.0 `PublicKeyToken=21d5517571b185bf`, `Janus.Data.v3.dll` istog tokena i `JupiterTranslator.dll` 1.0.0.0 bez tokena.

Treba donijeti ove datoteke uz `NetGenExtensions.dll` (`...\SharedAssemblies\Spin`). Sve su verzija **12.1.6.0**, `PublicKeyToken=b88d1754d700e49a`:

| DLL | Verzija | PublicKeyToken |
|---|---|---|
| `DevExpress.Printing.v12.1.Core.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.Data.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.Utils.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.XtraEditors.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.XtraPrinting.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.XtraBars.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.XtraGrid.v12.1.dll` | 12.1.6.0 | b88d1754d700e49a |
| `DevExpress.Reports.v12.1.Designer.dll` | 12.1.6.0 | b88d1754d700e49a |
