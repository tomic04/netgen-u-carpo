# NetGenExtensions.dll

NetGen ga učitava s `Assembly.Load("NetGenExtensions")`.

| Mjesto | Klasa | Metoda |
|---|---|---|
| `Form1.vb` 5988, `Form2.vb` 9044 | `NetGenExtensions.frmGridExPrintingREPX` | ispis REPX grida |
| `Form2.vb` 9065 | `NetGenExtensions.Form2` | `ChartGRID` |
| `Form2.vb` 9088 | `NetGenExtensions.Form1` | `PivotGRID` |

Na sql-ai DLL jest u `C:\Users\ntomic\Desktop\Jupiter Client\Programs\SharedAssemblies\Spin\NetGenExtensions.dll`.

`CreateInstance` za `frmGridExPrintingREPX` traži `Janus.Windows.GridEX.v3, Version=3.5.131.0, PublicKeyToken=21d5517571b185bf`. Te datoteke nema uz `NetGenExtensions.dll` ni u `SharedAssemblies`. Treba je donijeti u tu mapu da se ispis, pivot i chart mogu konstruirati. DLL se ne commita.
