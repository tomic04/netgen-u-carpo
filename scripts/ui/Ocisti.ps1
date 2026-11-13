# Ostavi N najnovijih mapa u C:\NetGenCarpo.
# Nikad ne brise mapu iz zadnji.txt ni mapu iz koje se Carpo.TestShell vrti.
# -Sto samo ispisuje sto bi se obrisalo.
param(
    [Parameter(Mandatory = $true)][string]$Korijen,
    [int]$Zadrzi = 5,
    [switch]$Sto
)
$ErrorActionPreference = "Continue"
if (-not (Test-Path -LiteralPath $Korijen)) { exit 0 }

function Dodaj([System.Collections.Generic.HashSet[string]]$skup, [string]$put) {
    if ([string]::IsNullOrWhiteSpace($put)) { return }
    try {
        $puno = [IO.Path]::GetFullPath($put).TrimEnd('\')
        if (Test-Path -LiteralPath $puno) { [void]$skup.Add($puno.ToLowerInvariant()) }
    } catch { }
}

$zastita = New-Object "System.Collections.Generic.HashSet[string]"
$zadnji = Join-Path $Korijen "zadnji.txt"
if (Test-Path -LiteralPath $zadnji) {
    $tekst = Get-Content -LiteralPath $zadnji -Raw
    $m = [regex]::Match($tekst, '(?m)^exe:\s*(.+)\s*$')
    if ($m.Success) {
        $bin = Split-Path -Parent $m.Groups[1].Value.Trim()
        Dodaj $zastita (Split-Path -Parent $bin)
    }
}
Get-Process -Name Carpo.TestShell -ErrorAction SilentlyContinue | ForEach-Object {
    try {
        if ($_.Path) {
            $bin = Split-Path -Parent $_.Path
            Dodaj $zastita (Split-Path -Parent $bin)
        }
    } catch { }
}

$mape = @(Get-ChildItem -LiteralPath $Korijen -Directory -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending)
$zadrzane = 0
foreach ($mapa in $mape) {
    $kljuc = $mapa.FullName.TrimEnd('\').ToLowerInvariant()
    if ($zastita.Contains($kljuc)) {
        Write-Host "cuvam (zadnji ili proces) $($mapa.Name)"
        continue
    }
    if ($zadrzane -lt $Zadrzi) {
        $zadrzane++
        Write-Host "cuvam novu $($mapa.Name)"
        continue
    }
    if ($Sto) {
        Write-Host "DRY-RUN brisem $($mapa.FullName)"
        continue
    }
    Write-Host "brisem $($mapa.FullName)"
    cmd /c "rmdir /s /q `"$($mapa.FullName)`"" | Out-Null
}

$work = $env:GITHUB_WORKSPACE
if (-not [string]::IsNullOrWhiteSpace($work)) {
    $korijenRada = Split-Path -Parent (Split-Path -Parent $work)
    $temp = Join-Path $korijenRada "_temp"
    $granica = (Get-Date).AddDays(-2)
    if (Test-Path -LiteralPath $temp) {
        Get-ChildItem -LiteralPath $temp -Force -ErrorAction SilentlyContinue | Where-Object { $_.LastWriteTime -lt $granica } | ForEach-Object {
            if ($Sto) { Write-Host "DRY-RUN brisem temp $($_.FullName)"; return }
            Write-Host "brisem temp $($_.FullName)"
            Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}
