# STA-12. Red u ssMeni za Statusi.xml.
# Jupiter cita Parametar i tekst iza druge dvotocke preda kao moduleId.
# Form1.ModuleID (1251-1258) to spremi u Mid. Form1.Run (1139) otvori
# remoteDllPath\SharedDLLs\xml\ + Mid.
# Parent se ne gasi: fk_meni ostaje ukljucen. Ako je Parent izracunat iz Nivo,
# Nivo djeteta je Nivo roditelja plus dva znaka, pa izraz pokaze na postojeci MeniID.
# Jupiter se ne cita.
$ErrorActionPreference = "Stop"
$cs = "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-sta12"
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

$fk = Q @"
select fk.name, fk.is_disabled
from sys.foreign_keys fk
join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
where fk.parent_object_id = object_id(N'dbo.ssMeni')
  and fkc.referenced_object_id = object_id(N'dbo.ssMeni')
"@
if ($fk.Rows.Count -eq 0) { throw "ssMeni nema fk na samu sebe. Skripta ne gasi constraint i ne pogađa krivi kljuc." }
$fkIme = [string]$fk.Rows[0].name
if ([int]$fk.Rows[0].is_disabled -eq 1) {
    Write-Host "$fkIme je ugasen. Micem samo testne redove Opis like 'test %' pa ga palim WITH CHECK."
    Exec "delete from ssMeni where Opis like N'test %' and Parametar like N'asm:NetGenDLL.loader:%'" | Out-Null
    Exec "alter table ssMeni with check check constraint [$fkIme]" | Out-Null
}
$opet = Q "select is_disabled from sys.foreign_keys where name = N'$fkIme' and parent_object_id = object_id(N'dbo.ssMeni')"
if ([int]$opet.Rows[0].is_disabled -ne 0) { throw "$fkIme je i dalje ugasen." }

$param = "asm:NetGenDLL.loader:Statusi.xml"
$postoji = Q "select MeniID, rtrim(Nivo) as Nivo, Parent from ssMeni where Parametar = N'$param'"
if ($postoji.Rows.Count -gt 0 -and ($postoji.Rows[0].Parent -is [DBNull])) {
    Exec "delete from ssMeni where Parametar = N'$param'" | Out-Null
    $postoji = Q "select MeniID from ssMeni where Parametar = N'$param'"
}
if ($postoji.Rows.Count -eq 0) {
    $kolone = Q @"
select c.name, c.is_identity, c.is_computed, c.max_length, ty.name as tip
from sys.columns c
join sys.types ty on c.user_type_id = ty.user_type_id
where c.object_id = object_id(N'dbo.ssMeni')
order by c.column_id
"@
    $nivoLen = 0
    $parentComputed = $false
    foreach ($c in $kolone.Rows) {
        if ([string]$c.name -eq "Nivo") {
            $nivoLen = [int]$c.max_length
            if ([string]$c.tip -match "nvar|nchar") { if ($nivoLen -gt 0) { $nivoLen = [int]($nivoLen / 2) } }
        }
        if ([string]$c.name -eq "Parent" -and [int]$c.is_computed -eq 1) { $parentComputed = $true }
    }
    if ($nivoLen -lt 4) { throw "Nivo je prekratak ($nivoLen) da dijete stane uz roditelja." }
    $maxRoditelj = $nivoLen - 2
    $roditelj = Q @"
select top 1 MeniID, rtrim(Nivo) as Nivo
from ssMeni
where len(rtrim(Nivo)) between 2 and $maxRoditelj
order by len(rtrim(Nivo)) desc, MeniID
"@
    if ($roditelj.Rows.Count -eq 0) { throw "Nema reda ssMeni ciji Nivo moze dobiti dijete." }
    $baza = [string]$roditelj.Rows[0].Nivo
    $roditeljId = [int]$roditelj.Rows[0].MeniID
    $kandidat = $null
    for ($i = 1; $i -le 99; $i++) {
        $pokusaj = $baza + ("{0:d2}" -f $i)
        if ($pokusaj.Length -gt $nivoLen) { continue }
        $ima = Q "select count(*) as n from ssMeni where rtrim(Nivo) = N'$pokusaj'"
        if ([int]$ima.Rows[0].n -eq 0) { $kandidat = $pokusaj; break }
    }
    if ($kandidat -eq $null) { throw "Nema slobodnog Nivo ispod $baza." }

    $imena = New-Object System.Collections.Generic.List[string]
    $izrazi = New-Object System.Collections.Generic.List[string]
    foreach ($c in $kolone.Rows) {
        if ([int]$c.is_identity -eq 1 -or [int]$c.is_computed -eq 1) { continue }
        if ([string]$c.tip -eq "timestamp" -or [string]$c.tip -eq "rowversion") { continue }
        $ime = [string]$c.name
        $q = "[" + $ime.Replace("]", "]]") + "]"
        $imena.Add($q)
        if ($ime -eq "Nivo") { $izrazi.Add("N'$kandidat'") }
        elseif ($ime -eq "Parametar") { $izrazi.Add("N'$param'") }
        elseif ($ime -eq "Opis") { $izrazi.Add("N'Statusi'") }
        elseif ($ime -eq "Parent") { $izrazi.Add([string]$roditeljId) }
        else { $izrazi.Add("p." + $q) }
    }
    $sql = "insert into ssMeni (" + ($imena -join ",") + ") select " + ($izrazi -join ",") + " from ssMeni p where p.MeniID = $roditeljId"
    Write-Host $sql
    Exec $sql | Out-Null
    Write-Host "ubacen Statusi.xml Nivo=$kandidat ParentIzracunat=$parentComputed roditelj=$roditeljId"
}

$provjera = Q @"
select c.MeniID, rtrim(c.Nivo) as Nivo, c.Parent, p.MeniID as RoditeljPostoji
from ssMeni c
left join ssMeni p on p.MeniID = c.Parent
where c.Parametar = N'$param'
"@
if ($provjera.Rows.Count -ne 1) { throw "Statusi.xml nije jedan red u ssMeni." }
if ($provjera.Rows[0].Parent -is [DBNull] -or $provjera.Rows[0].RoditeljPostoji -is [DBNull]) {
    throw "Parent ne pokazuje na postojeci MeniID. fk_meni nije gasen."
}
$conn.Close()
Write-Host ("STA-12 ssMeni MeniID=" + $provjera.Rows[0].MeniID + " Nivo=" + $provjera.Rows[0].Nivo + " Parent=" + $provjera.Rows[0].Parent)
