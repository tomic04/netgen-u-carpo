# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")` (`Form2.vb` 9044, 9065, 9088, `Form1.vb` 5988).

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | `PrintDocumentREPX` |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo radi isti `Assembly.Load("NetGenExtensions")`. Ako fusion ne nađe sklop, `AssemblyResolve` učita DLL iz Jupiter Clienta (uključujući podmape) i iz GAC mapa čije ime počinje s `Janus`. DLL se ne commita.

Na sql-ai `NetGenExtensions.dll` jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`. `Janus.Windows.GridEX.v3.dll` jest u `...\SharedAssemblies\Janus\`.

`PrintDocumentREPX` se s tim pozove. `Form1.PivotGRID` i `Form2.ChartGRID` traže još `DevExpress.Reports.v12.1.Designer, Version=12.1.6.0, PublicKeyToken=b88d1754d700e49a`. Carpo tu datoteku traži uz `NetGen.exe` i u `DevExpress 12.1` mapama. Ako je nema, treba je staviti uz `NetGenExtensions.dll`. DLL se ne commita.
