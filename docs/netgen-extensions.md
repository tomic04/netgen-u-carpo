# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")` (`Form2.vb` 9044, 9065, 9088, `Form1.vb` 5988).

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | `PrintDocumentREPX` |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo radi isti `Assembly.Load("NetGenExtensions")`. Ako fusion ne nađe sklop, `AssemblyResolve` učita DLL iz Jupiter Clienta (uključujući podmape) i iz GAC mapa čije ime počinje s `Janus`. DLL se ne commita.

Na sql-ai `NetGenExtensions.dll` jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`.

`frmGridExPrintingREPX` traži `Janus.Windows.GridEX.v3, Version=3.5.131.0, PublicKeyToken=21d5517571b185bf`. Ako te datoteke nema u Jupiter Clientu ni u GAC-u, tri forme se ne mogu konstruirati. Treba je staviti uz `NetGenExtensions.dll`.
