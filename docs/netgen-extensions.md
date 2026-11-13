# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")` (`Form2.vb` 9044, 9065, 9088, `Form1.vb` 5988).

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | `PrintDocumentREPX` |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo radi isti `Assembly.Load("NetGenExtensions")`. Ako fusion ne nađe sklop, `AssemblyResolve` učita DLL iz Jupiter Clienta (uključujući podmape) i iz GAC mapa čije ime počinje s `Janus`. DLL se ne commita.

Na sql-ai `NetGenExtensions.dll` jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`. `Janus.Windows.GridEX.v3.dll` jest u `...\SharedAssemblies\Janus\`.

UI test na `tests/Ekstenzije.xml` zove sve tri metode. Ako su DLL-ovi na stroju, ishod je `OK` tek kad `ChartGRID`, `PivotGRID` i `PrintDocumentREPX` stvarno prođu.

Ako pretraga (GAC `C:\Windows\Microsoft.NET\assembly`, sve mape `Jupiter Client`, `Program Files\DevExpress`, `SharedAssemblies`, backup mape, ostali diskovi) ne nađe v12.1 referencu, ishod je `OPEN`, ne `OK` i ne pad grane. Popis koji treba donijeti ispisuje se iz `GetReferencedAssemblies` (ime, verzija, public key token). DLL se ne kopira u repo i ništa se ne briše.

Poznata rupa dok pretraga ne nađe datoteku: `DevExpress.Reports.v12.1.Designer`, verzija 12.1.6.0, `PublicKeyToken=b88d1754d700e49a`. Puni popis v12.1 referenci dolazi s `EXT-NEMA` u logu testa.
