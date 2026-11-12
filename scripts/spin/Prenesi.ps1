# Cita definicije s jupiter2019/jupiter2022 (samo sys.columns i sys.sql_modules).
# Ne izvrsava procedure i ne cita retke tablica. Na spinu kreira objekte.
# Application Name=CarpoNetGen-definicije / CarpoNetGen-otvaranje.
$ErrorActionPreference = "Stop"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path (Join-Path $root "definitions"))) {
    $root = (Get-Location).Path
}
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izvori = Join-Path $outDir "izvori.md"
$otvaranje = Join-Path $outDir "otvaranje.md"

$red = @(
    @("jupiter2019", "Biovega"),
    @("jupiter2019", "CentarTehnike"),
    @("jupiter2019", "Setcor"),
    @("jupiter2019", "Enna"),
    @("jupiter2019", "Ireks"),
    @("jupiter2022", "Panissimo"),
    @("jupiter2022", "Tusak"),
    @("jupiter2022", "CentarTehnike"),
    @("jupiter2022", "Setcor")
)

function Veza([string]$server, [string]$baza, [string]$ime) {
    $cs = "Server=$server;Database=$baza;Integrated Security=True;Pooling=false;Connect Timeout=15;Application Name=$ime"
    $c = New-Object System.Data.SqlClient.SqlConnection $cs
    $c.Open()
    return $c
}

function Upit($conn, [string]$sql, [string]$p1) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    if ($p1 -ne $null) {
        $p = $cmd.Parameters.Add("@ime", [System.Data.SqlDbType]::NVarChar, 256)
        $p.Value = $p1
    }
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return $dt
}

function Pise([string]$def) {
    if ([string]::IsNullOrWhiteSpace($def)) { return $false }
    $t = [regex]::Replace($def, '(?s)/\*.*?\*/', ' ')
    $t = [regex]::Replace($t, '(?m)--.*$', ' ')
    return [regex]::IsMatch($t, '(?i)\b(insert|update|delete|merge|truncate)\b')
}

function TipStupca($row) {
    $tip = [string]$row.tip
    $len = [int]$row.max_length
    $prec = [int]$row.precision
    $scale = [int]$row.scale
    $nulla = $(if ($row.is_nullable) { "NULL" } else { "NOT NULL" })
    switch ($tip) {
        "varchar" { if ($len -lt 0) { $s = "varchar(max)" } else { $s = "varchar($len)" } }
        "char" { $s = "char($len)" }
        "nvarchar" { if ($len -lt 0) { $s = "nvarchar(max)" } else { $s = "nvarchar($($len / 2))" } }
        "nchar" { $s = "nchar($($len / 2))" }
        "decimal" { $s = "decimal($prec,$scale)" }
        "numeric" { $s = "numeric($prec,$scale)" }
        default { $s = $tip }
    }
    return "$s $nulla"
}

$nadjeni = @{}
$spin = Veza "localhost" "spin" "CarpoNetGen-otvaranje"

function NadjiStupac {
    foreach ($par in $red) {
        $c = $null
        try {
            $c = Veza $par[0] $par[1] "CarpoNetGen-definicije"
            $dt = Upit $c "SELECT ty.name AS tip, c.max_length, c.precision, c.scale, c.is_nullable FROM sys.columns c JOIN sys.types ty ON c.user_type_id = ty.user_type_id JOIN sys.objects o ON o.object_id = c.object_id WHERE o.name = @ime AND c.name = N'oznaka2' AND o.type IN ('U','V')" "klmasterroba"
            if ($dt.Rows.Count -gt 0) {
                return @{ Server = $par[0]; Baza = $par[1]; Tip = (TipStupca $dt.Rows[0]) }
            }
        } catch {
            Write-Host "definicija stupca $($par[0]).$($par[1]): $($_.Exception.Message)"
        } finally {
            if ($c -ne $null) { $c.Close() }
        }
    }
    return $null
}

function NadjiProc([string]$ime) {
    if ($nadjeni.ContainsKey($ime)) { return $nadjeni[$ime] }
    foreach ($par in $red) {
        $c = $null
        try {
            $c = Veza $par[0] $par[1] "CarpoNetGen-definicije"
            $dt = Upit $c "SELECT m.definition FROM sys.sql_modules m JOIN sys.objects o ON o.object_id = m.object_id WHERE o.type = 'P' AND o.name = @ime" $ime
            if ($dt.Rows.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace([string]$dt.Rows[0].definition)) {
                $def = [string]$dt.Rows[0].definition
                $info = @{ Server = $par[0]; Baza = $par[1]; Def = $def; Pise = (Pise $def) }
                $nadjeni[$ime] = $info
                return $info
            }
        } catch {
            Write-Host "definicija $ime $($par[0]).$($par[1]): $($_.Exception.Message)"
        } finally {
            if ($c -ne $null) { $c.Close() }
        }
    }
    $nadjeni[$ime] = $null
    return $null
}

function StaviStupac($info) {
    if ($info -eq $null) { return }
    $tip = $info.Tip
    $cmd = $spin.CreateCommand()
    $cmd.CommandTimeout = 30
    $mekse = $tip -replace 'NOT NULL', 'NULL'
    $cmd.CommandText = "IF OBJECT_ID(N'dbo.klmasterroba', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.klmasterroba', N'oznaka2') IS NULL ALTER TABLE dbo.klmasterroba ADD oznaka2 $mekse;"
    try { [void]$cmd.ExecuteNonQuery() } catch { Write-Host "oznaka2: $($_.Exception.Message)" }
    Write-Host "spin klmasterroba.oznaka2 = $mekse iz $($info.Server).$($info.Baza)"
}

function StaviProc([string]$ime, $info) {
    if ($info -eq $null) { return $false }
    $sql = $info.Def.Trim()
    $sql = [regex]::Replace($sql, '(?i)^create\s+proc', 'CREATE OR ALTER PROC', 1)
    $drop = $spin.CreateCommand()
    $drop.CommandTimeout = 30
    $drop.CommandText = "IF OBJECT_ID(N'dbo.$ime', N'P') IS NOT NULL DROP PROCEDURE dbo.[$ime];"
    [void]$drop.ExecuteNonQuery()
    $cmd = $spin.CreateCommand()
    $cmd.CommandTimeout = 60
    $cmd.CommandText = $info.Def
    try {
        [void]$cmd.ExecuteNonQuery()
    } catch {
        try {
            $cmd2 = $spin.CreateCommand()
            $cmd2.CommandTimeout = 60
            $cmd2.CommandText = $sql
            [void]$cmd2.ExecuteNonQuery()
        } catch {
            Write-Host "nije kreirano $ime : $($_.Exception.Message)"
            return $false
        }
    }
    $oznaka = $(if ($info.Pise) { "PISANJE" } else { "samo-citanje" })
    Write-Host "spin $ime iz $($info.Server).$($info.Baza) $oznaka"
    return $true
}

$imena = @(
    "OznakaKL","KUPNG","Statusi","KntBlaZak","PrijemniList","Kulture","FakPar","AOPNovi",
    "DobUgovori","Intrastat","JeziciNG","KUPDostKN","KUPDostKnNGBvg","KarticaPartnera",
    "KntBlaPromet","KntDospjeleStavke","KntPDVObrasciEU","LVObracun","MPBlagajna","MPInventura",
    "MPNGPregDokMAIL","MPNarIzdSkl","MPPonuda","MPPorKPNG","MPRACNEFISK"
)

function NadjiXml([string]$ime) {
    $korijen = Join-Path $root "definitions\netgen"
    $hit = Get-ChildItem -LiteralPath $korijen -Filter ($ime + ".xml") -Recurse -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($hit -eq $null) { return $null }
    return $hit.FullName
}

function SqlGranule([string]$put) {
    [xml]$doc = Get-Content -LiteralPath $put -Encoding UTF8
    $el = $doc.DocumentElement
    $raw = $el.GetAttribute("ONLOAD")
    if ([string]::IsNullOrWhiteSpace($raw)) { $raw = $el.GetAttribute("STARTUPSTORA") }
    return $raw
}

function Zamijeni([string]$sql) {
    if ([string]::IsNullOrWhiteSpace($sql)) { return "" }
    $sql = $sql.Replace("@@operateriid", "7").Replace("@@OPERATERIID", "7").Replace("@@Operateriid", "7")
    $sql = $sql.Replace("@@id", "0").Replace("@@ID", "0").Replace("@@Id", "0")
    $sql = $sql.Replace("@@node", "null").Replace("@@NODE", "null")
    $sql = $sql.Replace("@@detid", "0").Replace("@@DETID", "0")
    return $sql.Trim()
}

function OtvoriJednu([string]$sql) {
    $cmd = $spin.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    $rd = $cmd.ExecuteReader()
    $n = 0
    while ($rd.Read()) { $n++; if ($n -ge 3) { break } }
    $rd.Close()
    return $n
}

$stupac = NadjiStupac
StaviStupac $stupac

$poznate = @("spKultureNGQuery","spFakPartneriUvjetiQuery","spKUPPrometQueryN","spStatusiQuery","spBLZakljucakQueryNG")
foreach ($p in $poznate) {
    $info = NadjiProc $p
    if ($info -ne $null) { StaviProc $p $info }
}

$sbIz = New-Object System.Text.StringBuilder
[void]$sbIz.AppendLine("# Izvor definicija")
[void]$sbIz.AppendLine("")
[void]$sbIz.AppendLine("Samo ``sys.columns`` i ``sys.sql_modules``. Procedure nisu izvrsene na Jupiteru.")
[void]$sbIz.AppendLine("")
if ($stupac -ne $null) {
    [void]$sbIz.AppendLine("- ``klmasterroba.oznaka2`` = ``$($stupac.Tip)`` iz ``$($stupac.Server).$($stupac.Baza)``")
} else {
    [void]$sbIz.AppendLine("- ``klmasterroba.oznaka2`` nije nadjen u poznatim bazama")
}

$izvjestaj = @()
for ($krug = 1; $krug -le 4; $krug++) {
    $izvjestaj = @()
    $fali = @{}
    foreach ($ime in $imena) {
        $put = NadjiXml $ime
        if ($put -eq $null) {
            $izvjestaj += @{ Ime = $ime; Ishod = "nema-xml"; Poruka = "" }
            continue
        }
        $raw = SqlGranule $put
        $sql = Zamijeni $raw
        if ($sql.StartsWith(":")) {
            $izvjestaj += @{ Ime = $ime; Ishod = "forma"; Poruka = $sql }
            continue
        }
        if ([string]::IsNullOrWhiteSpace($sql)) {
            $izvjestaj += @{ Ime = $ime; Ishod = "prazno"; Poruka = "" }
            continue
        }
        try {
            $n = OtvoriJednu $sql
            $izvjestaj += @{ Ime = $ime; Ishod = "otvoreno"; Poruka = "$n redova" }
        } catch {
            $msg = $_.Exception.Message
            $izvjestaj += @{ Ime = $ime; Ishod = "pad"; Poruka = $msg }
            $m = [regex]::Match($msg, "Could not find stored procedure '([^']+)'")
            if ($m.Success) { $fali[$m.Groups[1].Value] = $true }
            $m2 = [regex]::Match($msg, "Invalid object name '([^']+)'")
            if ($m2.Success) { $fali[$m2.Groups[1].Value] = $true }
        }
    }
    if ($fali.Count -eq 0) { break }
    $novi = $false
    foreach ($imeProc in @($fali.Keys)) {
        $cisto = ($imeProc -replace '^dbo\.', '')
        $info = NadjiProc $cisto
        if ($info -ne $null) {
            StaviProc $cisto $info
            $novi = $true
        } else {
            Write-Host "nema definicije $cisto"
        }
    }
    if (-not $novi) { break }
}

foreach ($ime in $nadjeni.Keys) {
    $info = $nadjeni[$ime]
    if ($info -eq $null) { continue }
    $oznaka = $(if ($info.Pise) { "pise" } else { "ne-pise" })
    [void]$sbIz.AppendLine("- ``$ime`` iz ``$($info.Server).$($info.Baza)`` ($oznaka)")
}
[System.IO.File]::WriteAllText($izvori, $sbIz.ToString(), (New-Object System.Text.UTF8Encoding $false))

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("# Otvaranje na spinu")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Granula | Ishod | Poruka |")
[void]$sb.AppendLine("|---|---|---|")
foreach ($r in $izvjestaj) {
    $por = ($r.Poruka -replace "\|", "/" -replace "`r|`n", " ")
    if ($por.Length -gt 200) { $por = $por.Substring(0, 200) }
    [void]$sb.AppendLine("| $($r.Ime) | $($r.Ishod) | $por |")
    Write-Host "$($r.Ime) $($r.Ishod) $por"
}
[System.IO.File]::WriteAllText($otvaranje, $sb.ToString(), (New-Object System.Text.UTF8Encoding $false))
$spin.Close()
Write-Host "Izvori: $izvori"
Write-Host "Otvaranje: $otvaranje"
