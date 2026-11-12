# Stavi klijentske procedure (verzija A) i oznaka2 varchar(30) na spin.
# Tablice koje procedura zove, a nema ih, slozi iz sys.columns (jupiter, samo definicija).
# Retci s Jupitera se ne citaju. Postojeca klmasterroba se ne brise.
$ErrorActionPreference = "Continue"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root)) { $root = (Get-Location).Path }
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izvjestaj = Join-Path $outDir "klijenti.md"
$dir = Join-Path $root "scripts\spin\klijenti"

$serveri = @(
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

$tablice = @(
    "BLDokumenti","BLPrometDet","BLPrometGla","BLZakljucci","Blagajne","HRKadrovi","Partneri","VrstaDok",
    "fakture","faktureDob","kntostalo","kntplana","propongla","ssOperateri","FakPartneriUvjeti","parziro",
    "Kulture","OrgShema","Roba","KUPPromet","KUPPrometLog","KUPPrometOdobrenjeLog","KUPStatus","KUPVrstaPoste",
    "KupStatusOdobrenje","LnKUPSktDok","NabNarGla","Valute","kntpromet","kntvrsta","kupnapomene",
    "kupstatusoperateri","ssmeni","StatusiNext","statusi","statusioperateri"
)

$procedure = @(
    "spBLZakljucakQueryNG.A.sql",
    "spFakPartneriUvjetiQuery.A.sql",
    "spKultureNGQuery.A.sql",
    "spKUPPrometQueryN.A.sql",
    "spStatusiQuery.A.sql"
)

function Veza([string]$server, [string]$baza, [string]$ime) {
    $cs = "Server=$server;Database=$baza;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=20;Application Name=$ime"
    $c = New-Object System.Data.SqlClient.SqlConnection $cs
    $c.Open()
    return $c
}

function Celija($row, [string]$ime) { return $row[$ime] }

function Upit($conn, [string]$sql, [string]$p1) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 60
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

function Exec($conn, [string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 60
    $cmd.CommandText = $sql
    [void]$cmd.ExecuteNonQuery()
}

function Skalar($conn, [string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    return $cmd.ExecuteScalar()
}

function TipSql($row) {
    $tip = [string](Celija $row "tip")
    $len = [int](Celija $row "max_length")
    $prec = [int](Celija $row "precision")
    $scale = [int](Celija $row "scale")
    switch ($tip) {
        "varchar" { if ($len -lt 0) { return "varchar(max)" } else { return "varchar($len)" } }
        "char" { return "char($len)" }
        "nvarchar" { if ($len -lt 0) { return "nvarchar(max)" } else { return "nvarchar($($len / 2))" } }
        "nchar" { return "nchar($($len / 2))" }
        "decimal" { return "decimal($prec,$scale)" }
        "numeric" { return "numeric($prec,$scale)" }
        "varbinary" { if ($len -lt 0) { return "varbinary(max)" } else { return "varbinary($len)" } }
        "binary" { return "binary($len)" }
        default { return $tip }
    }
}

$sqlStupci = @'
SELECT c.column_id, c.name, ty.name AS tip, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity, c.is_computed
FROM sys.columns c
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
JOIN sys.objects o ON o.object_id = c.object_id
WHERE o.name = @ime AND o.type = 'U' AND o.is_ms_shipped = 0
ORDER BY c.column_id
'@

$linije = New-Object System.Collections.Generic.List[string]
$linije.Add("# Klijentski objekti na spinu")
$linije.Add("")

$spin = $null
try { $spin = Veza "localhost" "spin" "CarpoNetGen-otvaranje" }
catch {
    $linije.Add("- spin: $($_.Exception.Message)")
    Set-Content -LiteralPath $izvjestaj -Value ($linije -join "`r`n") -Encoding UTF8
    exit 1
}

function JupiterStupci([string]$ime) {
    foreach ($par in $serveri) {
        $c = $null
        try {
            $c = Veza $par[0] $par[1] "CarpoNetGen-definicije"
            $dt = Upit $c $sqlStupci $ime
            if ($dt.Rows.Count -gt 0) {
                return @{ Server = $par[0]; Baza = $par[1]; Rows = $dt }
            }
        } catch {
            $linije.Add("- definicija $ime $($par[0]).$($par[1]): $($_.Exception.Message)")
        } finally {
            if ($c -ne $null) { $c.Close() }
        }
    }
    return $null
}

foreach ($ime in $tablice) {
    $postoji = $null
    try { $postoji = Skalar $spin "SELECT OBJECT_ID(N'dbo.$ime', N'U')" } catch { $linije.Add("- $ime : $($_.Exception.Message)"); continue }
    if ($postoji -ne $null -and $postoji -ne [DBNull]::Value) {
        $linije.Add("- $ime vec postoji")
        continue
    }
    $def = JupiterStupci $ime
    if ($def -eq $null) {
        try {
            Exec $spin "CREATE TABLE dbo.[$ime] (id int NULL);"
            $linije.Add("- $ime stub id int NULL (nema definicije na Jupiteru)")
        } catch {
            $linije.Add("- $ime stub nije kreiran: $($_.Exception.Message)")
        }
        continue
    }
    $dijelovi = New-Object System.Collections.Generic.List[string]
    foreach ($r in $def.Rows.Rows) {
        if (Celija $r "is_computed") { continue }
        $kolona = [string](Celija $r "name")
        if ([string]::IsNullOrWhiteSpace($kolona)) { continue }
        $kol = TipSql $r
        $id = $(if (Celija $r "is_identity") { " IDENTITY(1,1)" } else { "" })
        $nulla = $(if ((Celija $r "is_identity") -or -not (Celija $r "is_nullable")) { "NOT NULL" } else { "NULL" })
        $dijelovi.Add("[$kolona] $kol$id $nulla")
    }
    if ($dijelovi.Count -eq 0) {
        $linije.Add("- $ime nema stupaca za CREATE")
        continue
    }
    try {
        Exec $spin ("CREATE TABLE dbo.[$ime] (" + ($dijelovi -join ", ") + ");")
        $linije.Add("- $ime iz $($def.Server).$($def.Baza), stupaca $($dijelovi.Count)")
    } catch {
        $linije.Add("- $ime CREATE: $($_.Exception.Message)")
    }
}

try {
    $ima = Skalar $spin "SELECT COL_LENGTH(N'dbo.klmasterroba', N'oznaka2')"
    if ($ima -eq $null -or $ima -eq [DBNull]::Value) {
        Exec $spin "ALTER TABLE dbo.klmasterroba ADD oznaka2 varchar(30) NULL;"
        $linije.Add("- oznaka2 varchar(30) NULL dodan (klijenti C, H, K). Tablica nije ponovo kreirana.")
    } else {
        $linije.Add("- oznaka2 vec postoji, duljina $ima. Nije suzen na 30 da se ne rezu vrijednosti.")
    }
} catch {
    $linije.Add("- oznaka2: $($_.Exception.Message)")
}

$tmp = Join-Path $outDir "proc-apply.sql"
foreach ($fajl in $procedure) {
    $put = Join-Path $dir $fajl
    if (-not (Test-Path -LiteralPath $put)) { $linije.Add("- nema $fajl"); continue }
    $tekst = [IO.File]::ReadAllText($put)
    $tekst = [regex]::Replace($tekst, '(?i)\bcreate\s+procedure\b', 'CREATE OR ALTER PROCEDURE', 1)
    [IO.File]::WriteAllText($tmp, $tekst, (New-Object System.Text.UTF8Encoding $false))
    & sqlcmd -S localhost -d spin -E -C -b -f 65001 -i $tmp
    if ($LASTEXITCODE -eq 0) { $linije.Add("- primijenjeno $fajl") }
    else { $linije.Add("- NIJE primijenjeno $fajl, kod $LASTEXITCODE") }
}

if ($spin -ne $null) { $spin.Close() }
Set-Content -LiteralPath $izvjestaj -Value ($linije -join "`r`n") -Encoding UTF8
Write-Host ($linije -join "`n")
exit 0
