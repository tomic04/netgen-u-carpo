# Naredbe NetGen i Carpo

Broj je pojavljivanje u `definitions/netgen` (XML). Status je što klik u Carpu radi.

| Naredba | NetGen | Carpo | Broj |
|---|---|---|---|
| OK / OKSTAY / CLOSE / CANCEL / CLOSEPARENT | `Form2.vb` 6884–7037 | izvršava | lanac |
| MSGBOX | `Form2.vb` 7165 | izvršava | lanac |
| CLICK | `Form2.vb` 7151–7155 | izvršava, PerformClick tog gumba | lanac |
| FOREACHROW / SELECTED / BLUEROW | `Form2.vb` 7040–7048, 8310–8394 | izvršava i na podlozi | 239 / 154 / 109 |
| COLORPICKER | `Form2.vb` 7156–7162, `Form1.vb` 6813–6831 | izvršava, ColorDialog | 15 |
| FISKALIZACIJA* | `Form1.vb` 454–520 | izvršava Raverus, samo testni CIS | fiskal granule |
| REFRESHMASTERGRID / REFRESHMASTER | `Form1.vb` 4570 | izvršava | često |
| REFRESHCONTROL | `Form2.vb` 6911–6915 | izvršava, ponovo čita SQL kontrole | malo |
| ASSEMBLYLOAD | `Form2.vb` 7424 | izvršava | rijetko |
| PRINT* / :repx/ | `Form1.vb` 5984, `Form2.vb` 9056 | izvršava Spin.Printing | često |
| EVAL | `Form2.vb` 7090, 8286 | izvršava izraz (ILCalc ugovor) | 0 u XML-u |
| RECALCGRID | `Form2.vb` 8219 | izvršava nad tablicom | 0 |
| TOGGLEGRIDGROUP | `Form2.vb` 6970 | izvršava stanje 0/1 | 0 |
| PREPAREMULTISELECTEDPRINT | `Form2.vb` 8404 | izvršava skup id-eva | 0 |
| PIVOTGRID / CHARTGRID / PRINTREPXGRID | `Form2.vb` 9044, 9062, 9085 | izvršava, Assembly.Load i zove PivotGRID / ChartGRID / PrintDocumentREPX | 0 |
| POSALJIRACUNENASERVER / ZAKLJUCKE / PORUKE | `Form2.vb` 7759, 7936, 8008 | izvršava POST | 0 |
| .VBS / .BAT | `Form2.vb` 7402–7417 | izvršava Process.Start | 0 |
| CREATETEXTFILE* | `Form2.vb` 7170–7399 | izvršava, piše datoteku | 0 |
| DOCK1–5 | `Form2.vb` 6916–6940 | izvršava DockStyle | 0 |
| ISEMPTY / ISEMPTYORZERO* | `Form2.vb` 6941–6946, 7053–7088, 7114–7147 | izvršava, preskok ili pitanje | lanac |
| SLOPP | `Form2.vb` 7494 | prazno, kao komentar u NetGenu | 0 |
| POSLOVNIPROSTOR / BLAGAJNAENABLE / UNCHECK | `Form1.vb` 4626–4801 | izvršava broj poziva / upit | rijetko |
| OPENFORM | `Form1.vb` 6582 | izvršava | lanac |
| GRID_COLUMN_VALUE | `Form1.vb` 6785 komentirano, `Form2.vb` 7007 živo | ne izvršava, kao podloga | 0 |
| BEEP | `Form2.vb` 6825–6827, `Form1.vb` 6579–6581 | izvršava Beep i 300 ms | 0 |
| STARTSEARCH | `Form1.vb` 6592 | nema; i u NetGenu je komentar | 0 |
