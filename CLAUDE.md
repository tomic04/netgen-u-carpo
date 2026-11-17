# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A port of **NetGen** (a 20-year-old VB.NET engine that renders XML "granule" definitions as forms and grids inside the Jupiter Client host) to **Carpo** (the C# / DevExpress 26.1 stack). The goal is behavioural parity: Carpo has to open the same NetGen XML and do what NetGen does.

| Path | Role |
|---|---|
| `netgen/` | Original NetGenDLL (VB.NET 4.8). **The reference implementation, in maintenance mode.** Has its own [netgen/CLAUDE.md](netgen/CLAUDE.md). Read it before you touch anything there. |
| `Devex_26/` | Carpo solution (`Carpo.sln`, .NET Framework, DevExpress 26.1). The port lives in `Devex_26/Carpo.NetGen`. |
| `definitions/netgen/` | Corpus of real NetGen XML granule files, grouped by client folder (`BIOVEGA`, `Agronom granule`, `Centar tehnike`, `Zito`, …). Tests and sweeps run over it. `Zito` (888 files, added 17. 11. 2026.) has no matching database on the servers used for testing, so it is covered by the parser, layout and sweep checks only. |
| `definitions/carpo/`, `definitions/Prepisane_granule/` | Native Carpo XML, and granules rewritten by hand (`Carpo/` and `Netgen/`). |
| `xml/klijenti/` | Client-specific versions of granules that were missing from the corpus (see `IZVOR.md`). They do not replace `definitions/netgen`. |
| `tests/Carpo.NetGen.Granule/` | Per-granule QA checklist runner (console exe). |
| `scripts/spin/` | Sets up the local `spin` SQL database used by tests: stubs, data transfer, client procedure versions. |
| `scripts/ui/`, `scripts/ci/` | UI smoke/click/sweep drivers and the CI guard scripts. |
| `tools/` | Python/Mono tooling: corpus catalog, differential tests against VB (`tools/diff`), local checks without Windows (`tools/provjera`). |
| `docs/` | Port status and analysis. See below. |
| `NetGenBot/`, `carpobot/` | Separate knowledge packages for agents that write NetGen or Carpo XML (rules, cheatsheets, sample granules). They are not part of the build. |

Docs, code comments, commit messages and user-facing strings are in **Croatian**. Write new ones in Croatian too.

## Carpo.NetGen architecture

`Devex_26/Carpo.NetGen` re-implements `netgen/Form1.vb` (main "podloga" form), `Form2.vb` (dialog form), `Class1.vb` (SQL, log, print helpers) and `Loader.vb`/`frm.vb` (entry points). Class and file comments cite the VB line ranges they port (e.g. `Form2.vb 3406-3901`). Use them to find the original behaviour.

- **Entry points (`Ulaz/`)**: `Loader` and `Frm` expose the same public *fields* (not properties) and `Run8` as the VB classes, because Jupiter sets them by name via `ssMeni` (`asm:Carpo.NetGen.Loader:<xml>`). `NgUlaz` resolves the XML path to `<remoteDllPath>\SharedDLLs\xml\<moduleId>`. `moduleId` may contain a subfolder. `TestXmlPath`/`BezProzora` exist only for TestShell. The AppDomain stays off, as it is in NetGen.
- **`Xml/`**: `NetGenXmlReader` parses into `Model/NgModel`. `IncludeXml`/`IncludeForme` handle `INCLUDE`.
- **`Grid/`**: master/detail grids, tree (`Stablo`), SQL text handling. `SqlTekst.Odluka` decides whether SQL is sent (empty, starts with `@`, unresolved `@@` → not sent, same as `Class1.vb` ExecSql). `ZastitaSql` blocks writes on test servers.
- **`Komande/`**: parsers and executors for command strings in `ONCLICK`/`ONSUBMIT`/`ONLOAD`/`CMD`… (`#`-separated chains, `Lanac`, `RafalNaredba`, `DijalogTok`, `GumbTok`, fiskalizacija, FOREACHROW, …). Most are pure static functions, so they can be tested without UI.
- **`Ui/`**: `Podloga*` (partial class split over several files) is the Form1 equivalent. `Dijalog*` covers the Form2 equivalents.
- **`Poziv/`, `Print/`**: late-bound assembly calls (ASSEMBLYLOAD, DMS, fiskal) and printing through Spin.Printing.
- **`TestHooks/`**: `GranulaSesija`/`GranulaIzvrsenje`, used only by tests. CI fails if production code references them.

Gap tracking: [docs/razlike-netgen-carpo.md](docs/razlike-netgen-carpo.md) lists the *intentional* differences from NetGen, each with a reason and a test id. [docs/naredbe-stanje.md](docs/naredbe-stanje.md) gives the status of each command. `docs/pregled-funkcija*.md` holds the function-by-function port status. `docs/granule/*.md` holds per-granule QA checklists. When you fix a divergence, remove its entry from `razlike-netgen-carpo.md`. When a divergence is intentional, add an entry.

## Hard rules

- **Do not modify NetGen XML in `definitions/netgen`** to make a test pass. Carpo has to handle the XML as it is. Do not change test paths to make a test pass either.
- **`netgen/` VB is reference-only** for port work. Change it only on explicit request, following the rules in `netgen/CLAUDE.md` (minimal changes, `On Error`/`Option Strict Off` style, UTF-8 BOM, version bump).
- **Never write to `jupiter2019` / `jupiter2022`.** Reading from them is allowed (`scripts/spin/Prenesi.ps1` reads object definitions there), but nothing may be written, and that includes error logging such as `spNetGenErrorLog`. Tests, TestShell and `scripts/spin` connect to the `spin` database on `sql-ai` by name (not `localhost`), so they run the same from the CI runner and from a developer machine. `sql-ai` is the user's own server, so prefer it for anything you need from a database. Missing procedures are stubbed there (`SELECT … WHERE 1=0`); the list of stubs is in `docs/granule/spin-objekti.md`.
- Writes (`insert`/`update`/…) are intentionally not sent on spin, and fiskalizacija goes to the **test** CIS only.
- A doc status of `portano` (or `nedostajalo-portano`) needs a UI test row in [tests/ui/katalog.txt](tests/ui/katalog.txt) (`UI<TAB>granula<TAB>test`), and TestShell or `scripts/ui` must mention both the test and the granule. `scripts/ci/portano-ui.py` enforces this.
- No dead public/internal methods in `Carpo.NetGen`: a method called only from tests fails CI (`scripts/ci/mrtve-metode.py`). Justified exceptions go in `scripts/ci/mrtve-iznimke.txt`, with a reason.

## Build

Windows only. You need VS 2026 MSBuild (`C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe`), DevExpress 26.1 and the Jupiter `SharedAssemblies` DLLs. None of the DLLs are committed. `Devex_26/Directory.Build.props` defaults `SharedAssembliesDir` to `C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies` and `DevExpressBinDir` to `C:\Program Files\DevExpress 26.1\Components\Bin\Framework`. Override them with `/p:`.

Projects reference each other through **HintPath to built DLLs in `SharedAssembliesDir\Spin`**, not through project references. A parallel `msbuild Carpo.sln` can therefore fail. CI builds them one at a time in this order and copies each output DLL into the refs folder before building the next:

Structure → Common → Data → Core → UI → Gen (`Carpo.csproj`) → XmlGen → CMS → NetGen → NetGen.Corpus → NetGen.GridTest → NetGen.Defaults → `tests/Carpo.NetGen.Granule` → Lite → Specific → Designer → TestShell

```powershell
& $msb Devex_26\Carpo.NetGen\Carpo.NetGen.csproj /t:Build /m:1 /v:minimal /p:Configuration=Debug /p:Platform=AnyCPU /p:SharedAssembliesDir=<refs> /p:OutputPath=<out>\ /p:DevExpressBinDir=<dx>
```

To build NetGen (VB) without deploying it: see `netgen/CLAUDE.md`. `build.bat` there also deploys the DLL.

## Tests

None of these use a test framework. They are console exes that print `PASS id`/`FAIL id` lines and exit non-zero on failure. There is no filter for running a single test. To run one, run the exe and grep for the test id, or temporarily call only that test function from `Main`.

| Exe (from build output) | What it does |
|---|---|
| `Carpo.NetGen.GridTest.exe` | Main unit and integration suite (`Devex_26/Carpo.NetGen.GridTest/Program.cs`, ~5k lines). Tests are `T("id", cond, detail)` calls, grouped in functions called from `Main`. Add a new group by calling it from `Main`, or add a separate class such as `ParametriOnSubmit.Izvrsi(T)`. Spin-DB tests run only with `CARPO_SPIN=1`. |
| `Carpo.NetGen.GridTest.exe --raspored definitions\netgen <out.tsv>` | Lays out every form of every granule without a DB. Fails on a frame or panel covering a field, or on a form that does not build. |
| `Carpo.NetGen.Corpus.exe definitions\netgen definitions\Prepisane_granule\Netgen --izvjestaj x.txt --tsv x.tsv` | Parser smoke over the corpus. Only invariant failures fail the run. A single bad XML goes into FAILED_LIST. |
| `Carpo.NetGen.Granule.exe <repo-root> --izvjestaj <file>` | Per-granule QA checklist. Also `--uzorak <md>` (open a sample of granules), `--ssmeni <md>` (open every `ssMeni` entry, nightly). Spin connection comes from the `SPIN_SQL` env var. |
| `Carpo.TestShell.exe` | WinForms host. With no args it is an interactive shell. With `--ui-smoke`, `--sweep <xmlroot> <report> [sec shard parts server db]`, `--ui-ext` or `--ui-boja` it runs automated UI passes. Drive it through `scripts/ui/UiSmoke.ps1 -Exe … -XmlRoot … -Shots … [-Popis scripts/ui/klik-kratko.txt] [-Klik]`. |

TestShell runtime logs: `C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\Spin\LogoviTestShella\` (newest `Log_Carpo.TestShell___*.txt`, or `...\Spin\Log\`). **When a runtime error is reported, read the newest log before you theorise**, and quote the relevant log lines.

### Checks without Windows or DevExpress (see [tools/provjera/README.md](tools/provjera/README.md))

```bash
python3 tools/provjera/cs_sintaksa.py <file.cs> ...   # C# syntax + duplicate members (tree-sitter); doesn't see partial classes across files
mcs -target:library -out:/tmp/ng.dll -r:System.Data.dll -r:System.Xml.dll -r:System.Windows.Forms.dll -r:System.Drawing.dll $(cat tools/provjera/mono-datoteke.txt)
python3 scripts/ci/mrtve-metode.py                     # dead-method / TestHooks guard (same as CI)
python3 scripts/ci/portano-ui.py                       # portano-needs-UI-test guard (same as CI)
```

`tools/diff/sql_diff.py` and `tools/diff/lanac_diff.py` are differential tests. They compare a literal Python model of the VB code (`vb_oracle.py`, `lanac_vb.py`) with the C# code running under Mono, over every SQL text and command chain in the corpus. `tools/provjera/KorpusParseri.cs` runs the ~50 pure parsers over every command string in the corpus to look for exceptions and infinite loops. Run it over new XML folders before a sweep.

## Comparing with the real NetGen (`tools/netgen-host`)

When the question is "what does NetGen do here", measure it instead of reading VB:

```powershell
.\tools\netgen-host\Izgradi.ps1      # builds netgen\ (no deploy) and NetGenHost.exe into artifacts\bin\netgen-host
.\tools\netgen-host\Usporedi.ps1 -Xml "definitions\netgen\BIOVEGA\AOPNovi.xml"
```

`Usporedi.ps1` opens the granule in the old NetGen and in Carpo (TestShell `--ui-smoke`) against `sql-ai/spin` and prints the SQL each one sent, in order.

- `NetGenHost.exe` opens a granule through `NetGenDLL.Loader.Run8` the way Jupiter does, records every MsgBox and dialog NetGen shows, closes them, takes a screenshot and exits. With `-Klik "<caption>"` it clicks that button on the main form after opening.
- `Trag.ps1` captures the SQL on the server: an Extended Events session named `NetGenUsporedba` on `sql-ai`, filtered to this machine and the `spin` database. The session exists only between `Start` and `Stop`.
- Building NetGen needs the `DevExpress131` folder of the full NetGen2026 clone (`-DevExpress131`, default `C:\TFS\Core Techonologies\NetGen2026\DevExpress131`) and the .NET Framework MSBuild. The newer VB compiler rejects `Form2.vb` with BC36595.
- The Janus licence dialog appears on every NetGen start. The host closes it, which takes about 12 seconds.

## CI (`.github/workflows/build.yml`, self-hosted runner `sql-ai`)

Each push and PR runs: the guard scripts → spin DB setup (`scripts/spin/*.sql`, `*.ps1`) → sequential build → corpus smoke → GridTest (`CARPO_SPIN=1`) + defaults + Granule checklist and sample → form layout → click review (`klik-kratko.txt`; `klik-puno.txt` nightly or with `workflow_dispatch klik=puno`). Details are in [docs/ci.md](docs/ci.md). `nocni-otvaranje.yml` opens every corpus file and every `ssMeni` row each night.

Commit-message tags switch a push to a UI **sweep** instead of the normal suite:
- `[sweep-N]` (N 0–6; 0 = whole corpus in one process, 1–6 = one sixth)
- `[ct-sweep-N:Baza]`: Centar tehnike granules against a backup on jupiter2019
- `[kl-sweep(-N):Server:Baza:Mapa]`: any `definitions/netgen/<Mapa>` against any server/DB

Skip list: `scripts/ui/sweep-preskoci.txt`.

## Encoding

NetGen `.vb` files are UTF-8 **with BOM**, and the BOM must be kept. Croatian characters (`š`, `ž`, `č`, …) are everywhere in paths, XML and strings. If you see mojibake (`Å¡`), re-read the file with the right encoding before you edit it. Run `build.bat` and scripts with Croatian paths from PowerShell, not Git Bash.

## Devex_26 Cursor rules (still apply)

- `Devex_26/Carpo_*.md` are audit/changelog docs for the wider Carpo DevExpress 26 migration. **B6 VirtualServerMode is a parked, separate project** (`Carpo_B6_VirtualServerMode_Spike.md`). Mention it in one short line when a wave wraps up, when large-grid or paging performance comes up, or when the user asks what is still open. Do not implement it unless the user explicitly opens it.
- In the original TFS setup, "check-in" means only the DEVEX26 folders. Always give `tf status`/`tf checkin` an explicit path, and never run them on the whole workspace.
