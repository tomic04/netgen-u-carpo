# Testni redovi u ssMeni na localhost/spin.
# Parametar je asm:NetGenDLL.loader:<xml>. Jupiter iz toga uzme tekst iza
# druge dvotocke i preda ga kao ModuleID. NetGen ga spremi u Mid
# (Form1.vb 1251-1258) i otvori remotedllpath\SharedDLLs\xml\ + Mid (1139).
# Ovo nije klijentski red. Jupiter se ne cita.
$ErrorActionPreference = "Stop"
$cs = "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-ssMeni;ApplicationIntent=ReadOnly"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

function Q([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return ,$dt
}

function Exec([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    return $cmd.ExecuteNonQuery()
}

$meta = Q @"
select top 1 o.object_id, o.type, schema_name(o.schema_id) as shema
from sys.objects o
where o.name = N'ssMeni' and o.type in ('U','V','SN')
order by case o.type when 'U' then 0 when 'V' then 1 else 2 end
"@
if ($meta.Rows.Count -gt 0) {
    $oid = $meta.Rows[0].object_id
    Write-Host ("ssMeni je " + $meta.Rows[0].shema + " tip " + $meta.Rows[0].type)
    if ([string]$meta.Rows[0].type -eq "SN") {
        $base = Q "select object_id(base_object_name) as id from sys.synonyms where object_id = $oid"
        if ($base.Rows.Count -gt 0 -and -not ($base.Rows[0].id -is [DBNull])) {
            $oid = $base.Rows[0].id
        }
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
    where i.object_id = $oid and i.is_primary_key = 1
) pk on pk.column_id = c.column_id
where c.object_id = $oid
order by c.column_id
"@
} else {
    $probe = $conn.CreateCommand()
    $probe.CommandText = "select top 0 * from ssMeni"
    $rd = $probe.ExecuteReader()
    $schema = $rd.GetSchemaTable()
    $rd.Close()
    if ($schema -eq $null -or $schema.Rows.Count -eq 0) {
        throw "Na spinu nema objekta ssMeni. Skripta ga ne stvara."
    }
    Write-Host "ssMeni se vidi upitom, ne preko sys.objects. Stupaca: $($schema.Rows.Count)"
    $kolone = New-Object System.Data.DataTable
    [void]$kolone.Columns.Add("name", [string])
    [void]$kolone.Columns.Add("is_identity", [int])
    [void]$kolone.Columns.Add("is_computed", [int])
    [void]$kolone.Columns.Add("is_nullable", [int])
    [void]$kolone.Columns.Add("tip", [string])
    [void]$kolone.Columns.Add("max_length", [int])
    [void]$kolone.Columns.Add("pk", [int])
    foreach ($srow in $schema.Rows) {
        $nr = $kolone.NewRow()
        $nr.name = [string]$srow["ColumnName"]
        $nr.is_identity = $(if ($srow["IsIdentity"] -eq $true) { 1 } else { 0 })
        $nr.is_computed = $(if ($srow["IsReadOnly"] -eq $true -and $srow["IsIdentity"] -ne $true) { 1 } else { 0 })
        $nr.is_nullable = $(if ($srow["AllowDBNull"] -eq $true) { 1 } else { 0 })
        $nr.tip = [string]$srow["DataTypeName"]
        $len = 0
        if ($srow["ColumnSize"] -isnot [DBNull]) { $len = [int]$srow["ColumnSize"] }
        $nr.max_length = $len
        $nr.pk = $(if ($srow.Table.Columns.Contains("IsKey") -and $srow["IsKey"] -eq $true) { 1 } else { 0 })
        [void]$kolone.Rows.Add($nr)
    }
}

$paramKol = $null
foreach ($r in $kolone.Rows) {
    if ([string]$r.name -eq "Parametar") { $paramKol = $r }
}
if ($paramKol -eq $null) { throw "ssMeni nema stupac Parametar." }

$predlozak = Q "select top 1 1 as ima from ssMeni where Parametar like N'asm:NetGenDLL.loader:%'"
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

$def = Q "select definition from sys.computed_columns where object_id = object_id(N'dbo.ssMeni') and name = N'Parent'"
if ($def.Rows.Count -gt 0) { Write-Host ("Parent = " + $def.Rows[0].definition) }
$brisi = @()
foreach ($s0 in $stavke) { $brisi += "N'asm:NetGenDLL.loader:" + $s0[0].Replace("'", "''") + "'" }
Exec ("delete from ssMeni where Parametar in (" + ($brisi -join ",") + ")") | Out-Null
try {
    Exec "alter table ssMeni with check check constraint fk_meni" | Out-Null
    Write-Host "fk_meni je with check"
} catch {
    Write-Host ("fk_meni with check nije prosao, constraint ostaje ukljucen za nove retke: " + $_.Exception.Message)
}

$uzor = Q "select top 1 rtrim(Nivo) as nivo, AplikacijeID from ssMeni where Parent is not null and len(rtrim(Nivo)) >= 4 order by Nivo"
if ($uzor.Rows.Count -eq 0) { throw "Nema reda s Parentom iz kojeg se cita duljina Nivo." }
$bazaNivo = [string]$uzor.Rows[0].nivo
$prefiks = $bazaNivo.Substring(0, $bazaNivo.Length - 2)
$app = $uzor.Rows[0].AplikacijeID
Write-Host ("Nivo prefiks roditelja " + $prefiks + " aplikacija " + $app)

function SlobodniNivo() {
    for ($i = 0; $i -lt 100; $i++) {
        $k = $prefiks + ("{0:d2}" -f $i)
        $n = Q "select count(*) as n from ssMeni where AplikacijeID = $app and rtrim(Nivo) = N'$k'"
        if ([int]$n.Rows[0].n -eq 0) { return $k }
    }
    throw "Nema slobodnog Nivo pod $prefiks"
}

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
select count(*) as n from ssMeni
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
        if ($tip -eq "timestamp" -or $tip -eq "rowversion") { continue }
        $ime = [string]$c.name
        $qime = "[" + $ime.Replace("]", "]]") + "]"
        if ([int]$c.is_identity -eq 1 -or [int]$c.is_computed -eq 1) { continue }
        $imena.Add($qime)
        if ($ime -eq "Parametar") {
            $izrazi.Add("N'" + $param.Replace("'", "''") + "'")
        } elseif ($ime -eq "Opis") {
            $izrazi.Add("N'" + $opis + "'")
        } elseif ($ime -eq "Nivo") {
            $dulj = [int]$c.max_length
            if ($tip -match "nvar|nchar") { if ($dulj -gt 0) { $dulj = [int]($dulj / 2) } }
            $kod = SlobodniNivo
            if ($dulj -gt 0 -and $kod.Length -gt $dulj) { $kod = $kod.Substring(0, $dulj) }
            $izrazi.Add("N'" + $kod + "'")
        } elseif ([int]$c.pk -eq 1) {
            $izrazi.Add("(select isnull(max(" + $qime + "), 0) + 1 from ssMeni)")
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
        $sql = "insert into ssMeni (" + ($imena -join ",") + ") select top 1 " + ($izrazi -join ",") + " from ssMeni p where p.Parametar like N'asm:NetGenDLL.loader:%'"
    } else {
        $sql = "insert into ssMeni (" + ($imena -join ",") + ") values (" + ($izrazi -join ",") + ")"
    }
    Write-Host $sql
    Exec $sql | Out-Null
    Write-Host "ubacen testni red $param nivo $kod"
}

$conn.Close()
Write-Host "ssMeni testni redovi gotovi."
