# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")`.

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form1.vb` 5988, `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | ispis REPX grida |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Carpo traži `NetGenExtensions.dll` uz exe i u `Jupiter Client\Programs\SharedAssemblies` (`EkstenzijaGrid.Nadji`). Ako ga na sql-ai nema, CI ispiše `NETGEN-EXTENSIONS nije na stroju`. Treba donijeti taj DLL s te tri klase. Ne izmišlja se zamjena.
