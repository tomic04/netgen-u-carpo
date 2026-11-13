# Faza C2: lanac naredbi dijaloga (`Form2.Rafalno`)

Datum: 2026-11-13. Grana: `pregled/lanac-c2` (na `pregled/sql-c1`).

## Metoda

`tools/diff/lanac_vb.py` doslovno prenosi redoslijed `ElseIf` grana iz `Form2.vb` 6821–7509. Grana se bira po `Trim.ToUpper.Contains`, pa redoslijed određuje ishod kad segment sadrži više ključnih riječi.

`tools/diff/LanacVoz.cs` izvršava C# `DijalogTok.Koraci` pod Mono.

Usporedba je nad **5 885 jedinstvenih segmenata**: svi `ON*` događaji kontrola u formama iz 609 granula, podijeljeni na `#`. Za pokretanje treba `tools/diff/stubs/` (nadomjestak `PrintBridge` bez DevExpressa).

## Rezultat

| | Prije | Poslije |
|---|---:|---:|
| Segmenti koje VB i Carpo prepoznaju drugačije | 159 | **2** |

Preostala 2 su greške autora XML-a, gdje Carpo radi ono što je autor htio: `:printgrid(grid)` i `:FISKALIZACIJA(...)`. VB oba otvara kao formu tog imena, koja ne postoji. Prijedlog: razlika R.

## Nalazi

| ID | Ozbiljnost | Što | NetGen | Carpo prije | Status |
|---|---|---|---|---|---|
| **LAN-01** | P0 | `ASSEMBLYLOAD(...)` u lancu s više segmenata | `Form2.vb` 7424: učita sklop i zove `totalgen` | prepoznat kao SQL i poslan serveru, a DLL se ne zove. Pogađa spremanje DMS-a (`select 'savedms' as akcija #ASSEMBLYLOAD(DmsNetGen.clsDms) #OK`, 10+ granula), `MPBlagajna` (slanje računa), `MPOdjavaNarNG` (Excel), `Narudzbe` | **ispravljeno**: korak `assembly`, `AssemblyPoziv` s potpisom dijaloga, pad prekida lanac |
| **LAN-02** | P1 | `POSLOVNIPROSTOR(...)` u lancu | `Form2.vb` 7489 | poslan kao SQL | **ispravljeno**: bilježi se kao na podlozi (fiskalni dio je R109/R111) |
| **LAN-03** | P0 | Segment s `@@` se zamijeni **prije** prepoznavanja | `Form2.vb` 6812–6819 | prepoznavanje na sirovom tekstu: `:@@imereporta1(@id=…)` otvara formu doslovno nazvanu `@@imereporta1`, a `@@izvjestaj(...)` ide serveru kao SQL. Pogađa ispise u PNNalozi, PNObracun, LVObracun, HRManagerGO i JOPPD | **ispravljeno**: izvršitelj lanca zamijeni pa ponovno prepozna segment |
| **CB-01** | P0 | Međuspremnik podloge (SETDEFAULTS) u dijalogu i na gumbu | `cls1.CB` je globalan, pa ga `NadjiVrijednostuPrevParametrima` vidi svugdje | dijalog i gumb nisu vidjeli CB, pa je `@@imereporta1` iz SETDEFAULTS postao `null` | **ispravljeno**: `SqlKontekst.Clipboard`, koji `ZaFormu` prenosi u dijalog |
| LAN-04 | P2 | SQL koji u tekstu vraća `':REPX/…'` | `Form2.vb` 7509: SQL | prepoznat kao predložak ispisa | **ispravljeno**: predložak samo kad segment počinje s `:` |
| **ISP-02** | P1 | `:RS/…`, `:GG/…`, `:rsgen:…`, `:vrstadokid-…`, `:….clsReport` (14 segmenata u dijalozima, više na podlozi) | `NovaGenerickaForma2` → ispis (`Form2.vb` 4276) | `OtvoriFormu` ih ne poznaje ("nema forme"), a `PrintBridge.Ispisi` zna samo za `repx` i ispis grida | **otvoreno → faza C8 (ispis)** |
| ISP-03 | P2 | `IspisMotora.JeIspisImena` | `Substring(0, 5) = "RSGEN"` | `tri == "RSGEN"` uspoređuje 3 znaka s 5, pa nikad nije istina | **otvoreno → C8** (bez ISP-02 ispravak ne mijenja ishod) |

## Testovi

| Test | Što |
|---|---|
| `lan01-assembly-nije-sql` | `select 1 as x#ASSEMBLYLOAD(...)#OK`: ASSEMBLYLOAD ne ide serveru |
| `lan03-token-u-ispis` | `:@@imereporta1(@id=5)#CLOSE` uz CB `imereporta1=repx/rptX` ide u ispis `rptX`, a ne u SQL |
| `tools/diff/lanac_diff.py` | prepoznavanje svih 5 885 segmenata korpusa protiv VB modela |

## Što je još u C2

- Lanac podloge (`Form1.Rafalno`, `Form1.vb` 6546–6850) i `GumbTok`/`Podloga`: ista usporedba prepoznavanja.
- Učinak pojedinih grana (MSGBOX, ISEMPTY*, EVAL, CREATETEXTFILE*, REFRESHCONTROL, DOCK*) protiv `Form2.vb`, grana po grana.
- `FOREACH*` na dijalogu (`ForEachSelected` / `BlueRow` / `Row`).
