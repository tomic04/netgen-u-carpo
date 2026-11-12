# Na spinu osigura tablicu klmasterroba i jedan testni red ako je prazna.
# Definicija stupaca cita se s jupiter2019/jupiter2022 (samo sys.columns).
# Retci s Jupitera se ne citaju. Ako definicije nema, ostaje stub stupaca iz XML-a.
# Application Name=CarpoNetGen-definicije / CarpoNetGen-otvaranje.
$ErrorActionPreference = "Continue"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path (Join-Path $root "definitions"))) {
    $root = (Get-Location).Path
}
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izvjestaj = Join-Path $outDir "retci.md"

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
    $cs = "Server=$server;Database=$baza;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=$ime"
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

function Exec($conn, [string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    return $cmd.ExecuteNonQuery()
}

function Skalar($conn, [string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    $v = $cmd.ExecuteScalar()
    if ($v -eq $null -or $v -eq [DBNull]::Value) { return $null }
    return $v
}

function Celija($row, [string]$ime) {
    if ($row -eq $null -or [string]::IsNullOrEmpty($ime)) { return $null }
    return $row[$ime]
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

function Literal([string]$ime, [string]$tip) {
    $t = $tip.ToLowerInvariant()
    switch ($ime) {
        "Naziv" { return "N'NetGen test'" }
        "sortkey" { return "N'01'" }
        "oznaka" { return "N'NG'" }
        "oznaka2" { return "N'NG'" }
    }
    if ($t -match 'int|bit|decimal|numeric|money|float|real') { return "0" }
    if ($t -match 'date|time') { return "'20000101'" }
    if ($t -eq "uniqueidentifier") { return "NEWID()" }
    if ($t -match 'char|text') { return "N''" }
    if ($t -match 'binary') { return "0x" }
    return $null
}

$linije = New-Object System.Collections.Generic.List[string]
$linije.Add("# Testni retci na spinu")
$linije.Add("")
$linije.Add("S Jupitera samo sys.columns. Retci tablica se tamo ne citaju.")
$linije.Add("")

$spin = $null
try {
    $spin = Veza "localhost" "spin" "CarpoNetGen-otvaranje"
} catch {
    $linije.Add("- spin: $($_.Exception.Message)")
    Set-Content -LiteralPath $izvjestaj -Value ($linije -join "`r`n") -Encoding UTF8
    exit 1
}

$stupci = $null
$izvor = ""
foreach ($par in $red) {
    $c = $null
    try {
        $c = Veza $par[0] $par[1] "CarpoNetGen-definicije"
        $sqlStupci = @'
SELECT c.column_id, c.name, ty.name AS tip, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity, c.is_computed
FROM sys.columns c
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
JOIN sys.objects o ON o.object_id = c.object_id
WHERE o.name = @ime AND o.type = 'U' AND o.is_ms_shipped = 0
ORDER BY c.column_id
'@
        $dt = Upit $c $sqlStupci "klmasterroba"
        if ($dt.Rows.Count -gt 0) {
            $stupci = $dt
            $izvor = "$($par[0]).$($par[1])"
            break
        }
    } catch {
        $linije.Add("- definicija $($par[0]).$($par[1]): $($_.Exception.Message)")
    } finally {
        if ($c -ne $null) { $c.Close() }
    }
}

$ima = Skalar $spin "SELECT OBJECT_ID(N'dbo.klmasterroba', N'U')"
if ($stupci -ne $null) {
    $linije.Add("- klmasterroba definicija: $izvor, stupaca $($stupci.Rows.Count)")
    if ($ima -eq $null) {
        $dijelovi = New-Object System.Collections.Generic.List[string]
        foreach ($r in $stupci.Rows) {
            if (Celija $r "is_computed") { continue }
            $ime = [string](Celija $r "name")
            if ([string]::IsNullOrWhiteSpace($ime)) { continue }
            $kol = TipSql $r
            $id = $(if (Celija $r "is_identity") { " IDENTITY(1,1)" } else { "" })
            $nulla = $(if ((Celija $r "is_identity") -or -not (Celija $r "is_nullable")) { "NOT NULL" } else { "NULL" })
            $dijelovi.Add("[$ime] $kol$id $nulla")
        }
        $sql = "CREATE TABLE dbo.klmasterroba (" + ($dijelovi -join ", ") + ");"
        try {
            Exec $spin $sql | Out-Null
            $linije.Add("- kreirana dbo.klmasterroba iz $izvor")
        } catch {
            $linije.Add("- CREATE nije prosao: $($_.Exception.Message)")
        }
    } else {
        foreach ($r in $stupci.Rows) {
            if (Celija $r "is_computed") { continue }
            $ime = [string](Celija $r "name")
            if ([string]::IsNullOrWhiteSpace($ime)) { continue }
            $postoji = Skalar $spin "SELECT COL_LENGTH(N'dbo.klmasterroba', N'$ime')"
            if ($postoji -ne $null) { continue }
            $kol = TipSql $r
            try {
                Exec $spin "ALTER TABLE dbo.klmasterroba ADD [$ime] $kol NULL;" | Out-Null
                $linije.Add("- dodan stupac $ime $kol NULL iz $izvor")
            } catch {
                $linije.Add("- ALTER $ime : $($_.Exception.Message)")
            }
        }
    }
} else {
    $linije.Add("- klmasterroba nije nadjena u poznatim bazama. Stub stupaca iz OznakaKL SELECT-a.")
    if ($ima -eq $null) {
        try {
            $sqlStub = @'
CREATE TABLE dbo.klmasterroba (
    klmasterrobaid int IDENTITY(1,1) NOT NULL,
    sortkey varchar(20) NULL,
    Naziv varchar(100) NULL,
    oznaka varchar(20) NULL,
    oznaka2 varchar(50) NULL
);
'@
            Exec $spin $sqlStub | Out-Null
            $linije.Add("- kreiran stub dbo.klmasterroba")
        } catch {
            $linije.Add("- stub CREATE: $($_.Exception.Message)")
        }
    } else {
        foreach ($par in @(
            @("sortkey", "varchar(20)"),
            @("Naziv", "varchar(100)"),
            @("oznaka", "varchar(20)"),
            @("oznaka2", "varchar(50)")
        )) {
            $postoji = Skalar $spin "SELECT COL_LENGTH(N'dbo.klmasterroba', N'$($par[0])')"
            if ($postoji -ne $null) { continue }
            try {
                Exec $spin "ALTER TABLE dbo.klmasterroba ADD [$($par[0])] $($par[1]) NULL;" | Out-Null
                $linije.Add("- stub stupac $($par[0])")
            } catch {
                $linije.Add("- stub ALTER $($par[0]): $($_.Exception.Message)")
            }
        }
    }
}

$broj = 0
try {
    $b = Skalar $spin "SELECT COUNT(*) FROM dbo.klmasterroba"
    if ($b -ne $null) { $broj = [int]$b }
} catch {
    $linije.Add("- COUNT: $($_.Exception.Message)")
    $broj = -1
}

if ($broj -eq 0) {
    $sqlMeta = @'
SELECT c.name, ty.name AS tip, c.is_nullable, c.is_identity, c.is_computed
FROM sys.columns c
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
JOIN sys.objects o ON o.object_id = c.object_id
WHERE o.object_id = OBJECT_ID(N'dbo.klmasterroba')
ORDER BY c.column_id
'@
    $meta = Upit $spin $sqlMeta $null
    $imena = New-Object System.Collections.Generic.List[string]
    $vrijednosti = New-Object System.Collections.Generic.List[string]
    $moze = $true
    foreach ($r in $meta.Rows) {
        if ((Celija $r "is_identity") -or (Celija $r "is_computed")) { continue }
        $tip = [string](Celija $r "tip")
        if ($tip -eq "timestamp" -or $tip -eq "rowversion") { continue }
        $ime = [string](Celija $r "name")
        if ([string]::IsNullOrWhiteSpace($ime)) { continue }
        $lit = Literal $ime $tip
        $nullable = [bool](Celija $r "is_nullable")
        if ($nullable -and $ime -ne "Naziv" -and $ime -ne "sortkey" -and $ime -ne "oznaka" -and $ime -ne "oznaka2") { continue }
        if ($lit -eq $null) {
            if (-not $nullable) { $moze = $false; $linije.Add("- nema literala za NOT NULL $ime ($tip)") }
            continue
        }
        $imena.Add("[$ime]")
        $vrijednosti.Add($lit)
    }
    if ($moze -and $imena.Count -gt 0) {
        $ins = "INSERT INTO dbo.klmasterroba (" + ($imena -join ", ") + ") VALUES (" + ($vrijednosti -join ", ") + ");"
        try {
            Exec $spin $ins | Out-Null
            $linije.Add("- ubacen jedan red jer je tablica bila prazna. OKL-58 trazi RowCount > 0 (Form1.vb 2059).")
        } catch {
            $linije.Add("- INSERT: $($_.Exception.Message)")
        }
    }
} elseif ($broj -gt 0) {
    $linije.Add("- klmasterroba vec ima $broj redova. Novi red nije ubacen.")
}

if ($spin -ne $null) { $spin.Close() }
Set-Content -LiteralPath $izvjestaj -Value ($linije -join "`r`n") -Encoding UTF8
Write-Host ($linije -join "`n")
exit 0
