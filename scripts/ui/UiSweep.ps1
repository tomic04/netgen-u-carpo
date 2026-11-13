# Sve NetGen granule, bez upisa. Noćni job. Hang je dulje od -Sekundi.
param(
    [Parameter(Mandatory = $true)][string]$Exe,
    [Parameter(Mandatory = $true)][string]$XmlRoot,
    [Parameter(Mandatory = $true)][string]$Izvjestaj,
    [int]$Sekundi = 6,
    [int]$Shard = 0,
    [int]$Dijelova = 6
)
$ErrorActionPreference = "Stop"
if (-not (Test-Path -LiteralPath $Exe)) { throw "Nema $Exe" }
if (-not (Test-Path -LiteralPath $XmlRoot)) { throw "Nema $XmlRoot" }
$mapa = Split-Path -Parent $Izvjestaj
if (-not [string]::IsNullOrWhiteSpace($mapa)) { New-Item -ItemType Directory -Force -Path $mapa | Out-Null }
$sid = (Get-Process -Id $PID).SessionId
$kodDat = $Izvjestaj + ".exit"
if (Test-Path -LiteralPath $kodDat) { Remove-Item -LiteralPath $kodDat -Force }
$rad = Split-Path -Parent $Exe
if ($sid -ne 0) {
    $linija = "--sweep `"$XmlRoot`" `"$Izvjestaj`" $Sekundi $Shard $Dijelova"
    $p = Start-Process -FilePath $Exe -ArgumentList $linija -WorkingDirectory $rad -PassThru
    $cekaj = 660000
    if ($Shard -eq 0) { $cekaj = 20000000 }
    if (-not $p.WaitForExit($cekaj)) {
        try { $p.Kill() } catch { }
        throw "sweep timeout"
    }
    if ($p.ExitCode -ne 0) { throw "sweep kod $($p.ExitCode)" }
} else {
    $cmd = $Izvjestaj + ".cmd"
    $bat = "@echo off`r`ncd /d `"$rad`"`r`n`"$Exe`" --sweep `"$XmlRoot`" `"$Izvjestaj`" $Sekundi $Shard $Dijelova`r`necho %ERRORLEVEL%> `"$kodDat`"`r`n"
    [IO.File]::WriteAllText($cmd, $bat)
    $tn = "NetGenSweep"
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    schtasks /Create /TN $tn /TR "`"$cmd`"" /SC ONCE /ST 00:00 /RU ntomic /IT /F
    if ($LASTEXITCODE -ne 0) { throw "schtasks nije dobio interaktivnu sesiju ntomic. Kod $LASTEXITCODE" }
    schtasks /Run /TN $tn | Out-Null
    $rok = (Get-Date).AddMinutes(11)
    if ($Shard -eq 0) { $rok = (Get-Date).AddHours(5) }
    while ((Get-Date) -lt $rok -and -not (Test-Path -LiteralPath $kodDat)) { Start-Sleep -Seconds 5 }
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    if (-not (Test-Path -LiteralPath $kodDat)) { throw "timeout sweep" }
    $kod = [int]((Get-Content -LiteralPath $kodDat -Raw).Trim())
    if ($kod -ne 0) { throw "sweep kod $kod" }
}
if (-not (Test-Path -LiteralPath $Izvjestaj)) { throw "Nema izvjestaja $Izvjestaj" }
Get-Content -LiteralPath $Izvjestaj -TotalCount 40
$global:LASTEXITCODE = 0
exit 0
