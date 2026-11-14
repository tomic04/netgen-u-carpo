# Sve NetGen granule, bez upisa. Noćni job. Hang je dulje od -Sekundi.
param(
    [Parameter(Mandatory = $true)][string]$Exe,
    [Parameter(Mandatory = $true)][string]$XmlRoot,
    [Parameter(Mandatory = $true)][string]$Izvjestaj,
    [int]$Sekundi = 6,
    [int]$Shard = 0,
    [int]$Dijelova = 6,
    [string]$Server = "",
    [string]$Baza = ""
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
$redovi = $Izvjestaj + ".red"
if (Test-Path -LiteralPath $redovi) { Remove-Item -LiteralPath $redovi -Force }
if (Test-Path -LiteralPath ($Izvjestaj + ".iznimka.txt")) { Remove-Item -LiteralPath ($Izvjestaj + ".iznimka.txt") -Force }
# Shard ima ~100 granula; s pragom od 4 s treba i 20 min. Korak u build.yml ima 40 min.
$rokShardMs = 2280000
function Pokazi-Djelomicno {
    if (Test-Path -LiteralPath $redovi) {
        $sve = @(Get-Content -LiteralPath $redovi -Encoding UTF8)
        $nije = @($sve | Where-Object { ($_ -split "`t")[1] -ne "CISTO" })
        Write-Host ("---- sweep redovi (.red): " + $sve.Count + " redaka, " + $nije.Count + " nije CISTO; svaki red ima ukupno/forme/gdi/sat (SWP-01) ----")
        $sve | ForEach-Object { Write-Host $_ }
        if ($sve.Count -gt 0) { Write-Host ("zadnja: " + $sve[$sve.Count - 1]) }
    } else {
        Write-Host "---- nema ni djelomicnog izvjestaja ($redovi) ----"
    }
    $faza = $Izvjestaj + ".faza.txt"
    if (Test-Path -LiteralPath $faza) {
        Write-Host "---- kraj traga (.faza.txt) ----"
        Get-Content -LiteralPath $faza -Tail 70 -Encoding UTF8 | ForEach-Object { Write-Host $_ }
    }
    # SWP-09: 40 najsporijih upita (NgVeza) i broj puklih, s granulom.
    if (Test-Path -LiteralPath $faza) {
        $spori = @(Get-Content -LiteralPath $faza -Encoding UTF8 | Where-Object { $_ -match ' spor upit (\d+) ms' } |
            Sort-Object { [int]([regex]::Match($_, ' spor upit (\d+) ms').Groups[1].Value) } -Descending | Select-Object -First 40)
        if ($spori.Count -gt 0) {
            Write-Host "---- najsporiji upiti ----"
            $spori | ForEach-Object { Write-Host $_ }
        }
        $pukli = @(Get-Content -LiteralPath $faza -Encoding UTF8 | Where-Object { $_ -match ' upit pukao ' })
        Write-Host ("---- pukli upiti: " + $pukli.Count + " (prvih 30) ----")
        $pukli | Select-Object -First 30 | ForEach-Object { Write-Host $_ }
    }
    # SWP-09: trag prvih pet HANG granula (od "granula X" do sljedeće), da se vidi koji korak drži rok.
    if ((Test-Path -LiteralPath $faza) -and (Test-Path -LiteralPath $redovi)) {
        $hangovi = @(Get-Content -LiteralPath $redovi -Encoding UTF8 | Where-Object { ($_ -split "`t")[1] -eq "HANG" } | ForEach-Object { ($_ -split "`t")[0] } | Select-Object -Unique -First 5)
        if ($hangovi.Count -gt 0) {
            $tr = @(Get-Content -LiteralPath $faza -Encoding UTF8)
            foreach ($h in $hangovi) {
                Write-Host "---- trag HANG: $h ----"
                $u = $false; $n = 0
                foreach ($l in $tr) {
                    if ($l -match ' granula (.+)$') { $u = ($Matches[1] -eq $h); if (-not $u -and $n -gt 0) { break } }
                    if ($u -and $n -lt 60) { Write-Host $l; $n++ }
                }
            }
        }
    }
    # SWP-03: cijeli stog iznimaka (TestShell piše .iznimka.txt), najviše 150 redaka.
    $izn = $Izvjestaj + ".iznimka.txt"
    if (Test-Path -LiteralPath $izn) {
        Write-Host "---- iznimke (.iznimka.txt) ----"
        Get-Content -LiteralPath $izn -TotalCount 150 -Encoding UTF8 | ForEach-Object { Write-Host $_ }
    }
}
if ($sid -ne 0) {
    $linija = "--sweep `"$XmlRoot`" `"$Izvjestaj`" $Sekundi $Shard $Dijelova"
    if ($Server -ne "" -and $Baza -ne "") { $linija = $linija + " `"$Server`" `"$Baza`"" }
    $p = Start-Process -FilePath $Exe -ArgumentList $linija -WorkingDirectory $rad -PassThru
    $cekaj = $rokShardMs
    if ($Shard -eq 0) { $cekaj = 20000000 }
    if (-not $p.WaitForExit($cekaj)) {
        try { $p.Kill() } catch { }
        Pokazi-Djelomicno
        throw "sweep timeout"
    }
    if ($p.ExitCode -ne 0) {
        # SWP-05: s "#kraj" u .red sweep je prošao sve granule, a proces je pao pri izlazu. To je nalaz
        # (stog je u .iznimka.txt), ali izvještaj vrijedi i ispisuje se.
        $kraj = (Test-Path -LiteralPath $redovi) -and (@(Get-Content -LiteralPath $redovi -Encoding UTF8) | Where-Object { $_ -like "#kraj*" }).Count -gt 0
        if (-not $kraj) { Pokazi-Djelomicno; throw "sweep kod $($p.ExitCode) (3 = cuvar: granula je drzala UI dulje od 90 s)" }
        Write-Host "::warning::Sweep je gotov, ali proces je pao pri izlazu (kod $($p.ExitCode)); stog je u .iznimka.txt."
    }
} else {
    $cmd = $Izvjestaj + ".cmd"
    $dodatak = ""
    if ($Server -ne "" -and $Baza -ne "") { $dodatak = " `"$Server`" `"$Baza`"" }
    $bat = "@echo off`r`ncd /d `"$rad`"`r`n`"$Exe`" --sweep `"$XmlRoot`" `"$Izvjestaj`" $Sekundi $Shard $Dijelova$dodatak`r`necho %ERRORLEVEL%> `"$kodDat`"`r`n"
    [IO.File]::WriteAllText($cmd, $bat)
    $tn = "NetGenSweep"
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    schtasks /Create /TN $tn /TR "`"$cmd`"" /SC ONCE /ST 00:00 /RU ntomic /IT /F
    if ($LASTEXITCODE -ne 0) { throw "schtasks nije dobio interaktivnu sesiju ntomic. Kod $LASTEXITCODE" }
    schtasks /Run /TN $tn | Out-Null
    $rok = (Get-Date).AddMilliseconds($rokShardMs)
    if ($Shard -eq 0) { $rok = (Get-Date).AddHours(5) }
    while ((Get-Date) -lt $rok -and -not (Test-Path -LiteralPath $kodDat)) { Start-Sleep -Seconds 5 }
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    if (-not (Test-Path -LiteralPath $kodDat)) { Pokazi-Djelomicno; throw "timeout sweep" }
    $kod = [int]((Get-Content -LiteralPath $kodDat -Raw).Trim())
    if ($kod -ne 0) { Pokazi-Djelomicno; throw "sweep kod $kod (3 = cuvar: granula je drzala UI dulje od 90 s)" }
}
if (-not (Test-Path -LiteralPath $Izvjestaj)) { throw "Nema izvjestaja $Izvjestaj" }
Pokazi-Djelomicno
# Glava (brojevi i grupe) pa svi redovi koji nisu CISTO; cisti se samo broje.
$izv = @(Get-Content -LiteralPath $Izvjestaj -Encoding UTF8)
$izv | Where-Object { -not ($_ -match "`tCISTO`t") } | ForEach-Object { Write-Host $_ }
$global:LASTEXITCODE = 0
exit 0
