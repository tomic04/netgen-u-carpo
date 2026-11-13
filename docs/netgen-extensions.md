# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")` (`Form2.vb` 9044, 9065, 9088, `Form1.vb` 5988).

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | `PrintDocumentREPX` |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo radi isti `Assembly.Load("NetGenExtensions")`. Ako fusion ne nađe sklop, `AssemblyResolve` učita DLL iz Jupiter Clienta (uključujući podmape) i iz GAC mapa čije ime počinje s `Janus`. DLL se ne commita.

Na sql-ai `NetGenExtensions.dll` jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`. `Janus.Windows.GridEX.v3.dll` jest u `...\SharedAssemblies\Janus\`.

UI test na `tests/Ekstenzije.xml` zove sve tri metode. `PrintDocumentREPX` na `frmGridExPrintingREPX` prolazi.

`Form1.PivotGRID` i `Form2.ChartGRID` ne uđu u tip: sklop traži `DevExpress.Reports.v12.1.Designer, Version=12.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a`. Te datoteke nema u Jupiter Clientu, nema je u GAC-u, i na stroju nema `NetGen.exe`. Treba je staviti uz `NetGenExtensions.dll`. DLL se ne commita.
