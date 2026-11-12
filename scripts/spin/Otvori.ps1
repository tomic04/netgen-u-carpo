# Otvaranje master SQL-a granula na spinu. Ne dira jupiter.
# Application Name=CarpoNetGen-otvaranje. Jedna veza, jedna naredba.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not (Test-Path (Join-Path $root "definitions"))) {
    $root = (Get-Location).Path
}
$cs = "Server=localhost;Database=spin;Integrated Security=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-otvaranje"
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izlaz = Join-Path $outDir "otvaranje.md"

function Zamijeni([string]$sql) {
    if ([string]::IsNullOrWhiteSpace($sql)) { return "" }
    $sql = $sql.Replace("@@operateriid", "7").Replace("@@OPERATERIID", "7").Replace("@@Operateriid", "7")
    $sql = $sql.Replace("@@id", "0").Replace("@@ID", "0").Replace("@@Id", "0")
    $sql = $sql.Replace("@@node", "null").Replace("@@NODE", "null")
    $sql = $sql.Replace("@@detid", "0").Replace("@@DETID", "0")
    return $sql.Trim()
}

$stavke = @(
    @{ Ime = "OznakaKL"; Dat = "definitions\netgen\BIOVEGA\OznakaKL.xml"; Atr = "STARTUPSTORA" },
    @{ Ime = "KUPNG"; Dat = "definitions\netgen\Agronom granule\KUPNG.xml"; Atr = "ONLOAD" },
    @{ Ime = "Statusi"; Dat = "definitions\netgen\Agronom granule\Statusi.xml"; Atr = "ONLOAD" },
    @{ Ime = "KntBlaZak"; Dat = "definitions\netgen\BIOVEGA\KntBlaZak.xml"; Atr = "ONLOAD" },
    @{ Ime = "Kulture"; Dat = "definitions\netgen\BIOVEGA\Kulture.xml"; Atr = "ONLOAD" },
    @{ Ime = "FakPar"; Dat = "definitions\netgen\BIOVEGA\FakPar.xml"; Atr = "ONLOAD" },
    @{ Ime = "AOPNovi"; Dat = "definitions\netgen\BIOVEGA\AOPNovi.xml"; Atr = "ONLOAD" }
)

$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("# Otvaranje na spinu")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Jedna veza, ``Application Name=CarpoNetGen-otvaranje``. Jupiter se ne dira.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Granula | Ishod | Poruka |")
[void]$sb.AppendLine("|---|---|---|")

foreach ($s in $stavke) {
    $put = Join-Path $root $s.Dat
    $ishod = "nema-xml"
    $poruka = $s.Dat
    if (Test-Path -LiteralPath $put) {
        [xml]$doc = Get-Content -LiteralPath $put -Encoding UTF8
        $raw = $doc.DocumentElement.GetAttribute($s.Atr)
        if ([string]::IsNullOrWhiteSpace($raw)) { $raw = $doc.DocumentElement.GetAttribute("ONLOAD") }
        if ([string]::IsNullOrWhiteSpace($raw)) { $raw = $doc.DocumentElement.GetAttribute("STARTUPSTORA") }
        $sql = Zamijeni $raw
        if ($sql.StartsWith(":")) {
            $ishod = "forma"
            $poruka = $sql
        } elseif ([string]::IsNullOrWhiteSpace($sql)) {
            $ishod = "prazno"
            $poruka = ""
        } else {
            try {
                $cmd = $conn.CreateCommand()
                $cmd.CommandTimeout = 30
                $cmd.CommandText = $sql
                $rd = $cmd.ExecuteReader()
                $n = 0
                while ($rd.Read()) { $n++; if ($n -ge 5) { break } }
                $rd.Close()
                $ishod = "otvoreno"
                $poruka = "$n redova (uzorak do 5)"
            } catch {
                $ishod = "pad"
                $poruka = ($_.Exception.Message -replace "\|", "/" -replace "`r|`n", " ")
                if ($poruka.Length -gt 180) { $poruka = $poruka.Substring(0, 180) }
            }
        }
    }
    [void]$sb.AppendLine("| $($s.Ime) | $ishod | $poruka |")
    Write-Host "$($s.Ime) $ishod $poruka"
}
$conn.Close()
[System.IO.File]::WriteAllText($izlaz, $sb.ToString(), (New-Object System.Text.UTF8Encoding $false))
Write-Host "Izvjestaj: $izlaz"
