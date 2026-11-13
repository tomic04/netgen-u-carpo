# Pravi Start u Carpo.TestShell.exe. Nema QA kuke.
# Ako prozor granule ne izadje, ili iskoci MessageBox, izlaz nije 0.
param(
    [Parameter(Mandatory = $true)][string]$Exe,
    [Parameter(Mandatory = $true)][string]$XmlRoot,
    [Parameter(Mandatory = $true)][string]$Shots
)
$ErrorActionPreference = "Stop"
if (-not (Test-Path -LiteralPath $Exe)) { throw "Nema $Exe" }
New-Item -ItemType Directory -Force -Path $Shots | Out-Null

$granule = @(
    "BIOVEGA\OznakaKL.xml",
    "Agronom granule\KntKompVerizne.xml",
    "BIOVEGA\JeziciNG.xml", # nije Manager: nema stabla ni detaila
    "Agronom granule\Statusi.xml",
    "BIOVEGA\KntBlaZak.xml",
    "Agronom granule\RealizacijaStavkeNG.xml" # nije Manager: ulaz je dijalog Parametri
)

function Pokreni([string]$xml, [string]$png) {
    $sid = (Get-Process -Id $PID).SessionId
    $izlaz = $png + ".txt"
    $kodDat = $png + ".exit"
    if (Test-Path -LiteralPath $izlaz) { Remove-Item -LiteralPath $izlaz -Force }
    if (Test-Path -LiteralPath $kodDat) { Remove-Item -LiteralPath $kodDat -Force }
    if (Test-Path -LiteralPath $png) { Remove-Item -LiteralPath $png -Force }
    $rad = Split-Path -Parent $Exe
    if ($sid -ne 0) {
        # Jedan string. Polje argumenata u Windows PowerShellu 5.1 ne stavlja navodnike,
        # pa se "Agronom granule" rasiječe i TestShell otvori krivi XML.
        $linija = "--ui-smoke `"$xml`" `"$png`""
        $p = Start-Process -FilePath $Exe -ArgumentList $linija -WorkingDirectory $rad -PassThru
        if (-not $p.WaitForExit(75000)) {
            try { $p.Kill() } catch { }
            return -1
        }
        return $p.ExitCode
    }
    $cmd = Join-Path $Shots ("pokreni-" + [IO.Path]::GetFileNameWithoutExtension($png) + ".cmd")
    $bat = "@echo off`r`ncd /d `"$rad`"`r`n`"$Exe`" --ui-smoke `"$xml`" `"$png`"`r`necho %ERRORLEVEL%> `"$kodDat`"`r`n"
    [IO.File]::WriteAllText($cmd, $bat)
    $tn = "NetGenUiSmoke"
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    schtasks /Create /TN $tn /TR "`"$cmd`"" /SC ONCE /ST 00:00 /RU ntomic /IT /F
    if ($LASTEXITCODE -ne 0) { throw "schtasks nije dobio interaktivnu sesiju ntomic. Kod $LASTEXITCODE" }
    schtasks /Run /TN $tn | Out-Null
    $rok = (Get-Date).AddSeconds(80)
    while ((Get-Date) -lt $rok -and -not (Test-Path -LiteralPath $kodDat)) { Start-Sleep -Seconds 1 }
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    if (-not (Test-Path -LiteralPath $kodDat)) { throw "timeout interaktivno $xml" }
    return [int]((Get-Content -LiteralPath $kodDat -Raw).Trim())
}

foreach ($rel in $granule) {
    $xml = Join-Path $XmlRoot $rel
    if (-not (Test-Path -LiteralPath $xml)) { throw "Nema XML $xml" }
    $ime = [IO.Path]::GetFileNameWithoutExtension($rel)
    $png = Join-Path $Shots ($ime + ".png")
    Write-Host "UI smoke $rel"
    $kod = Pokreni $xml $png
    $tekst = ""
    if (Test-Path -LiteralPath ($png + ".txt")) { $tekst = Get-Content -LiteralPath ($png + ".txt") -Raw }
    Write-Host $tekst
    $snimkaGreska = $png + ".snimka.txt"
    if (Test-Path -LiteralPath $snimkaGreska) { Write-Host (Get-Content -LiteralPath $snimkaGreska -Raw) }
    $prozori = $png + ".prozori.txt"
    if (Test-Path -LiteralPath $prozori) { Write-Host (Get-Content -LiteralPath $prozori -Raw) }
    $iznimka = $png + ".iznimka.txt"
    if (Test-Path -LiteralPath $iznimka) { Write-Host (Get-Content -LiteralPath $iznimka -Raw) }
    if ($kod -eq -1) { throw "timeout $xml" }
    if ($kod -ne 0) { throw "UI smoke $ime kod $kod. $tekst" }
    if ($tekst -notlike "OK*") { throw "UI smoke $ime nema OK. $tekst" }
    if (-not (Test-Path -LiteralPath $png)) { throw "UI smoke $ime nema screenshot" }
    if ($ime -eq "OznakaKL" -or $ime -eq "KntKompVerizne" -or $ime -eq "Statusi") {
        if ($tekst -notlike "*podforma=*") { throw "UI smoke $ime nije kliknuo podformu. $tekst" }
        if (-not (Test-Path -LiteralPath ($png + ".podforma.png"))) { throw "UI smoke $ime nema snimku podforme" }
    }
    if ($ime -eq "RealizacijaStavkeNG" -and $tekst -notlike "*klik=Odustani*") {
        throw "UI smoke $ime nije kliknuo Odustani. $tekst"
    }
}
Write-Host "UI smoke svih $($granule.Count) granula je prosao."
# Start-Process ne postavlja LASTEXITCODE. Prazan kod workflow cita kao pad.
$global:LASTEXITCODE = 0
