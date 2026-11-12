# Pravi Start u Carpo.TestShell.exe. Nema QA kuke.
# Ako prozor granule ne izadje, ili iskoci MessageBox, izlaz nije 0.
param(
    [Parameter(Mandatory = $true)][string]$Exe,
    [Parameter(Mandatory = $true)][string]$XmlRoot,
    [Parameter(Mandatory = $true)][string]$Shots,
    [string]$Popis,
    [switch]$Klik
)
$ErrorActionPreference = "Stop"
if (-not (Test-Path -LiteralPath $Exe)) { throw "Nema $Exe" }
New-Item -ItemType Directory -Force -Path $Shots | Out-Null

$granule = @(
    "BIOVEGA\OznakaKL.xml",
    "Agronom granule\KntKompVerizne.xml",
    "BIOVEGA\JeziciNG.xml",
    "Agronom granule\Statusi.xml",
    "BIOVEGA\KntBlaZak.xml",
    "Agronom granule\RealizacijaStavkeNG.xml"
)
if (-not [string]::IsNullOrWhiteSpace($Popis)) {
    $granule = @(Get-Content -LiteralPath $Popis | Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and -not $_.StartsWith("#") })
}

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
        $klikArg = ""
        if ($Klik) { $klikArg = " --klik" }
        $linija = "--ui-smoke `"$xml`" `"$png`"$klikArg"
        $p = Start-Process -FilePath $Exe -ArgumentList $linija -WorkingDirectory $rad -PassThru
        $cekaj = 75000
        if ($Klik) { $cekaj = 150000 }
        if (-not $p.WaitForExit($cekaj)) {
            try { $p.Kill() } catch { }
            throw "timeout $xml"
        }
        return $p.ExitCode
    }
    $cmd = Join-Path $Shots ("pokreni-" + [IO.Path]::GetFileNameWithoutExtension($png) + ".cmd")
    $klikArg = ""
    if ($Klik) { $klikArg = " --klik" }
    $bat = "@echo off`r`ncd /d `"$rad`"`r`n`"$Exe`" --ui-smoke `"$xml`" `"$png`"$klikArg`r`necho %ERRORLEVEL%> `"$kodDat`"`r`n"
    [IO.File]::WriteAllText($cmd, $bat)
    $tn = "NetGenUiSmoke"
    schtasks /Delete /TN $tn /F 2>$null | Out-Null
    schtasks /Create /TN $tn /TR "`"$cmd`"" /SC ONCE /ST 00:00 /RU ntomic /IT /F
    if ($LASTEXITCODE -ne 0) { throw "schtasks nije dobio interaktivnu sesiju ntomic. Kod $LASTEXITCODE" }
    schtasks /Run /TN $tn | Out-Null
    $sekundi = 80
    if ($Klik) { $sekundi = 160 }
    $rok = (Get-Date).AddSeconds($sekundi)
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
    if ($kod -ne 0) { throw "UI smoke $ime kod $kod. $tekst" }
    if ($tekst -notlike "OK*") { throw "UI smoke $ime nema OK. $tekst" }
    if (-not (Test-Path -LiteralPath $png)) { throw "UI smoke $ime nema screenshot" }
    $klikDat = $png + ".klik.txt"
    if ($Klik -and (Test-Path -LiteralPath $klikDat)) {
        Get-Content -LiteralPath $klikDat | ForEach-Object { Write-Host $_ }
    }
}
if ($Klik) {
    $izv = Join-Path $Shots "klik-pregled.md"
    $sb = New-Object System.Text.StringBuilder
    [void]$sb.AppendLine("# Klik pregled")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| Granula | Dio | Ime | Ishod | Poruka | NetGen |")
    [void]$sb.AppendLine("|---|---|---|---|---|---|")
    Get-ChildItem -LiteralPath $Shots -Filter *.klik.txt | ForEach-Object {
        Get-Content -LiteralPath $_.FullName | ForEach-Object {
            $c = $_.Split("`t")
            if ($c.Count -lt 6) { return }
            [void]$sb.AppendLine("| $($c[0]) | $($c[1]) | $($c[2]) | $($c[3]) | $($c[4]) | $($c[5]) |")
        }
    }
    [IO.File]::WriteAllText($izv, $sb.ToString())
    Write-Host "Klik pregled: $izv"
}
Write-Host "UI smoke svih $($granule.Count) granula je prosao."
exit 0
