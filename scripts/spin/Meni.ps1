# Testni redovi u dbo.ssMeni na localhost/spin.
# Parametar je asm:NetGenDLL.loader:<xml>. Jupiter iz toga uzme tekst iza
# druge dvotocke i preda ga kao ModuleID. NetGen ga spremi u Mid
# (Form1.vb 1251-1258) i otvori remotedllpath\SharedDLLs\xml\ + Mid (1139).
# Ovo nije klijentski red. Jupiter se ne cita.
$ErrorActionPreference = "Stop"
$cs = "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-ssMeni"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

function Q([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return $dt
}

function Exec([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    return $cmd.ExecuteNonQuery()
}

$ima = Q "select object_id(N'dbo.ssMeni', N'U') as id"
if ($ima.Rows.Count -eq 0 -or $ima.Rows[0].id -is [DBNull]) {
    throw "Na spinu nema dbo.ssMeni. Skripta tablicu ne stvara."
}

$kolone = Q @"
select c.name, c.is_identity, c.is_computed, c.is_nullable, ty.name as tip, c.max_length,
       case when pk.column_id is null then 0 else 1 end as pk
from sys.columns c
join sys.types ty on c.user_type_id = ty.user_type_id
left join (
    select ic.column_id
    from sys.indexes i
    join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
    where i.object_id = object_id(N'dbo.ssMeni') and i.is_primary_key = 1
) pk on pk.column_id = c.column_id
where c.object_id = object_id(N'dbo.ssMeni')
order by c.column_id
"@

$paramKol = $null
foreach ($r in $kolone.Rows) {
    if ([string]$r.name -eq "Parametar") { $paramKol = $r }
}
if ($paramKol -eq $null) { throw "ssMeni nema stupac Parametar." }

$predlozak = Q "select top 1 1 as ima from dbo.ssMeni where Parametar like N'asm:NetGenDLL.loader:%'"
$imaPredlozak = $predlozak.Rows.Count -gt 0

$stavke = @(
    @("FakPar.xml", "test FakPar"),
    @("KntDospjeleStavke.xml", "test Dospjele"),
    @("Kulture.xml", "test Kulture"),
    @("KUPDostKN.xml", "test KUPDostKN"),
    @("KUPDostKnNGBvg.xml", "test KUPDostBvg"),
    @("KUPNG.xml", "test KUPNG"),
    @("MPNarIzdSkl.xml", "test MPNarIzdSkl"),
    @("MPNGPregDokMAIL.xml", "test MPNGPregDok"),
    @("RatArkod.xml", "test RatArkod"),
    @("RobaWebKlase.xml", "test RobaWebKlase")
)

foreach ($s in $stavke) {
    $fajl = $s[0]
    $opis = $s[1].Replace("'", "''")
    $param = "asm:NetGenDLL.loader:" + $fajl
    $max = [int]$paramKol.max_length
    if ([string]$paramKol.tip -eq "nvarchar" -or [string]$paramKol.tip -eq "nchar") {
        if ($max -gt 0) { $max = [int]($max / 2) }
    }
    if ($max -gt 0 -and $param.Length -gt $max) { throw "Parametar ne stane ($max): $param" }

    $f = $fajl.Replace("'", "''")
    $broj = Q @"
select count(*) as n from dbo.ssMeni
where Parametar like N'%:$f' or Parametar like N'%\$f' or Parametar like N'%/$f'
"@
    if ([int]$broj.Rows[0].n -gt 0) {
        Write-Host "vec postoji $fajl"
        continue
    }

    $imena = New-Object System.Collections.Generic.List[string]
    $izrazi = New-Object System.Collections.Generic.List[string]
    foreach ($c in $kolone.Rows) {
        $tip = [string]$c.tip
        if ([int]$c.is_identity -eq 1 -or [int]$c.is_computed -eq 1) { continue }
        if ($tip -eq "timestamp" -or $tip -eq "rowversion") { continue }
        $ime = [string]$c.name
        $qime = "[" + $ime.Replace("]", "]]") + "]"
        $imena.Add($qime)
        if ($ime -eq "Parametar") {
            $izrazi.Add("N'" + $param.Replace("'", "''") + "'")
        } elseif ($ime -eq "Opis") {
            $izrazi.Add("N'" + $opis + "'")
        } elseif ([int]$c.pk -eq 1) {
            $izrazi.Add("(select isnull(max(" + $qime + "), 0) + 1 from dbo.ssMeni)")
        } elseif ($imaPredlozak) {
            $izrazi.Add("p." + $qime)
        } elseif ([int]$c.is_nullable -eq 0) {
            if ($tip -match "char|text") { $izrazi.Add("N''") }
            elseif ($tip -eq "bit") { $izrazi.Add("0") }
            elseif ($tip -match "date|time") { $izrazi.Add("getdate()") }
            elseif ($tip -eq "uniqueidentifier") { $izrazi.Add("newid()") }
            else { $izrazi.Add("0") }
        } else {
            $izrazi.Add("NULL")
        }
    }
    if ($imaPredlozak) {
        $sql = "insert into dbo.ssMeni (" + ($imena -join ",") + ") select top 1 " + ($izrazi -join ",") + " from dbo.ssMeni p where p.Parametar like N'asm:NetGenDLL.loader:%'"
    } else {
        $sql = "insert into dbo.ssMeni (" + ($imena -join ",") + ") values (" + ($izrazi -join ",") + ")"
    }
    Exec $sql | Out-Null
    Write-Host "ubacen testni red $param"
}

$conn.Close()
Write-Host "ssMeni testni redovi gotovi."
