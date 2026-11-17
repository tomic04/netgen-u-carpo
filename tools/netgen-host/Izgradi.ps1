# Izgradi stari NetGen iz netgen\ (bez deploya, izvor se ne mijenja) i pokretač NetGenHost.exe.
# Izlaz: artifacts\bin\netgen-host\ (NetGenHost.exe, netgen\NetGenDLL.dll s ovisnostima). Mapa je u .gitignore (bin/).
#
# DevExpress 13.1, Janus i ostale ovisnosti starog NetGena nisu u repozitoriju: uzimaju se iz mape DevExpress131
# punog klona NetGen2026 (parametar -DevExpress131).
param(
    [string]$DevExpress131 = "C:\TFS\Core Techonologies\NetGen2026\DevExpress131"
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$izlaz = Join-Path $repo "artifacts\bin\netgen-host"
$netgen = Join-Path $izlaz "netgen\"
$obj = Join-Path $izlaz "obj\"
if (-not (Test-Path $DevExpress131)) { throw "Nema mape $DevExpress131 (DevExpress 13.1 i Janus za stari NetGen)." }
New-Item -ItemType Directory -Force $izlaz | Out-Null

# netgen\CLAUDE.md: NetGen se gradi MSBuildom iz .NET Frameworka; noviji VB prevoditelj javlja BC36595 u Form2.vb.
$msb = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
& $msb (Join-Path $repo "netgen\NetGenDLL.vbproj") /t:Build /nologo /v:minimal /clp:ErrorsOnly /p:Configuration=Debug `
    "/p:OutputPath=$netgen" "/p:BaseIntermediateOutputPath=$obj" "/p:IntermediateOutputPath=$obj" "/p:ReferencePath=$DevExpress131"
if ($LASTEXITCODE -ne 0) { throw "Build NetGenDLL nije prošao." }

$csc = Get-ChildItem "C:\Program Files\Microsoft Visual Studio" -Recurse -Filter csc.exe -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -like "*\MSBuild\Current\Bin\Roslyn\csc.exe" } | Select-Object -First 1
if (-not $csc) { throw "Nema csc.exe (Visual Studio MSBuild\Current\Bin\Roslyn)." }
# x86: GridExPrinting.dll starog NetGena je 32-bitni.
& $csc.FullName -nologo -target:winexe -platform:x86 "-out:$(Join-Path $izlaz 'NetGenHost.exe')" `
    -r:System.dll -r:System.Core.dll -r:System.Drawing.dll -r:System.Windows.Forms.dll (Join-Path $PSScriptRoot "NetGenHost.cs")
if ($LASTEXITCODE -ne 0) { throw "Build NetGenHost nije prošao." }
$v = [Reflection.AssemblyName]::GetAssemblyName((Join-Path $netgen "NetGenDLL.dll")).Version
"NetGenDLL $v i NetGenHost.exe su u $izlaz"
