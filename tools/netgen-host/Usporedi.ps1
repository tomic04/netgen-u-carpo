# Otvori istu granulu u starom NetGenu (NetGenHost) i u Carpo.NetGen (TestShell --ui-smoke) i ispiši SQL koji
# svaki pošalje na sql-ai/spin, hvatan na serveru (Trag.ps1). Prije toga: Izgradi.ps1 i izgrađen TestShell.
#
#   .\Usporedi.ps1 -Xml "definitions\netgen\BIOVEGA\AOPNovi.xml"
#   .\Usporedi.ps1 -Xml ... -Klik "Unesi"     (NetGen: nakon otvaranja klikne gumb podloge; dijalog se zatvori)
#
# Izlaz je u artifacts\bin\usporedba\: <ime>-netgen.tsv, <ime>-carpo.tsv, snimke i poruke NetGena.
param(
    [Parameter(Mandatory = $true)][string]$Xml,
    [int]$Sekundi = 6,
    [string]$Klik = "",
    [string]$Server = "sql-ai",
    [string]$Baza = "spin",
    [int]$Operater = 7,
    [string]$TestShell = ""
)
$ErrorActionPreference = "Stop"
$repo = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$trag = Join-Path $PSScriptRoot "Trag.ps1"
$hostExe = Join-Path $repo "artifacts\bin\netgen-host\NetGenHost.exe"
if (-not (Test-Path $hostExe)) { throw "Nema $hostExe. Pokreni Izgradi.ps1." }
if (-not $TestShell) { $TestShell = Join-Path $repo "artifacts\bin\Carpo.TestShell.exe" }
if (-not (Test-Path $TestShell)) { throw "Nema $TestShell." }
if (-not [IO.Path]::IsPathRooted($Xml)) { $Xml = Join-Path $repo $Xml }
if (-not (Test-Path -LiteralPath $Xml)) { throw "Nema XML-a $Xml" }
$ime = [IO.Path]::GetFileNameWithoutExtension($Xml)
$izl = Join-Path $repo "artifacts\bin\usporedba"
New-Item -ItemType Directory -Force $izl | Out-Null
$env:NETGEN_DLL = Join-Path $repo "artifacts\bin\netgen-host\netgen\NetGenDLL.dll"

function Ispisi($dat) {
    Get-Content -LiteralPath $dat | ForEach-Object {
        $k = $_.Split("`t")
        $vrsta = $k[3].Replace("sql_batch_completed", "sql").Replace("rpc_completed", "rpc").Replace("error_reported", "GRESKA")
        $tekst = if ($vrsta -eq "GRESKA") { $k[4] } else { $k[5] }
        if ($tekst.Length -gt 220) { $tekst = $tekst.Substring(0, 220) }
        "  " + $vrsta.PadRight(6) + " " + $tekst
    }
}

try {
    & $trag -Radnja Start -Server $Server -Baza $Baza | Out-Null
    $env:NETGEN_TIP = "NetGenDLL.Loader"
    $arg = @($Server, $Baza, "$Operater", "`"$Xml`"", "`"$izl\$ime-netgen`"", "$Sekundi")
    if ($Klik) { $arg += "`"$Klik`"" }
    $p = Start-Process -FilePath $hostExe -ArgumentList $arg -PassThru
    if (-not $p.WaitForExit(180000)) { $p.Kill(); "NetGen: ROK" }
    & $trag -Radnja Citaj -Server $Server -Baza $Baza -Izlaz "$izl\$ime-netgen.tsv" | Out-Null

    # Carpo kroz isti pokretač i isti ulaz (Carpo.NetGen.Loader.Run8) kojim ga otvara Jupiter. TestShell --ui-smoke
    # ne otvara ulaznu formu granule, pa usporedba kroz njega nije bila poštena.
    & $trag -Radnja Start -Server $Server -Baza $Baza | Out-Null
    $env:NETGEN_DLL = Join-Path (Split-Path -Parent $TestShell) "Carpo.NetGen.dll"
    $env:NETGEN_TIP = "Carpo.NetGen.Loader"
    $arg[4] = "`"$izl\$ime-carpo`""
    $p = Start-Process -FilePath $hostExe -ArgumentList $arg -PassThru
    if (-not $p.WaitForExit(180000)) { $p.Kill(); "Carpo: ROK" }
    & $trag -Radnja Citaj -Server $Server -Baza $Baza -Izlaz "$izl\$ime-carpo.tsv" | Out-Null
}
finally {
    & $trag -Radnja Stop -Server $Server -Baza $Baza | Out-Null
}

"=== NetGen ($ime)"
if (Test-Path -LiteralPath "$izl\$ime-netgen.poruke.txt") {
    Get-Content -LiteralPath "$izl\$ime-netgen.poruke.txt" | Where-Object { $_ -match "`t(MSGBOX|DIJALOG|IZNIMKA|KLIK)`t" } | ForEach-Object { "  " + ($_.Split("`t")[1..2] -join " ") }
}
Ispisi "$izl\$ime-netgen.tsv"
"=== Carpo ($ime)"
Ispisi "$izl\$ime-carpo.tsv"
