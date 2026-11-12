# STA-12. Red u ssMeni za Statusi.xml.
# Jupiter cita Parametar i tekst iza druge dvotocke preda kao moduleId.
# Form1.ModuleID (1251-1258) to spremi u Mid. Form1.Run (1139) otvori
# remoteDllPath\SharedDLLs\xml\ + Mid.
# Parent je izracunat iz Nivo. fk_meni se ne gasi. Nivo djeteta je isti oblik
# kao kod reda koji vec ima Parent na postojeci MeniID (isti korak i ista aplikacija).
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
select fk.name, fk.is_disabled, c.name as stupac, ty.name as tip
from sys.foreign_keys fk
join sys.foreign_key_columns fkc on fkc.constraint_object_id = fk.object_id
join sys.columns c on c.object_id = fkc.parent_object_id and c.column_id = fkc.parent_column_id
join sys.types ty on c.user_type_id = ty.user_type_id
where fk.parent_object_id = object_id(N'dbo.ssMeni')
  and fkc.referenced_object_id = object_id(N'dbo.ssMeni')
"@
if ($fk.Rows.Count -eq 0) { throw "ssMeni nema fk na samu sebe. Skripta ne gasi constraint." }
$fkIme = [string]$fk.Rows[0].name
Write-Host ("fk " + $fkIme + " stupac " + $fk.Rows[0].stupac + " tip " + $fk.Rows[0].tip + " disabled=" + $fk.Rows[0].is_disabled)
if ([int]$fk.Rows[0].is_disabled -eq 1) {
    Write-Host "$fkIme je ugasen. Micem samo testne redove Opis like 'test %' pa ga palim WITH CHECK."
    Exec "delete from ssMeni where Opis like N'test %' and Parametar like N'asm:NetGenDLL.loader:%'" | Out-Null
    Exec "alter table ssMeni with check check constraint [$fkIme]" | Out-Null
}
$opet = Q "select is_disabled from sys.foreign_keys where name = N'$fkIme' and parent_object_id = object_id(N'dbo.ssMeni')"
if ([int]$opet.Rows[0].is_disabled -ne 0) { throw "$fkIme je i dalje ugasen." }

$shema = Q @"
select c.name, c.is_identity, c.is_computed, c.max_length, ty.name as tip, cc.definition
from sys.columns c
join sys.types ty on c.user_type_id = ty.user_type_id
left join sys.computed_columns cc on cc.object_id = c.object_id and cc.column_id = c.column_id
where c.object_id = object_id(N'dbo.ssMeni') and c.name in (N'Nivo', N'Parent', N'AplikacijeID')
"@
$nivoLen = 0
foreach ($c in $shema.Rows) {
    Write-Host ("stupac " + $c.name + " tip=" + $c.tip + " computed=" + $c.is_computed + " max=" + $c.max_length)
    if (-not ($c.definition -is [DBNull])) { Write-Host ("definicija " + $c.name + ": " + $c.definition) }
    if ([string]$c.name -eq "Nivo") {
        $nivoLen = [int]$c.max_length
        if ([string]$c.tip -match "nvar|nchar") { if ($nivoLen -gt 0) { $nivoLen = [int]($nivoLen / 2) } }
    }
}
if ($nivoLen -lt 4) { throw "Nivo je prekratak ($nivoLen)." }

$param = "asm:NetGenDLL.loader:Statusi.xml"
$lose = Q "select MeniID, Parent from ssMeni where Parametar = N'$param'"
if ($lose.Rows.Count -gt 0 -and ($lose.Rows[0].Parent -is [DBNull])) {
    Exec "delete from ssMeni where Parametar = N'$param'" | Out-Null
}

function ParentJeNivo([string]$nivo, [string]$parent) {
    if ([string]::IsNullOrEmpty($nivo)) { return $false }
    if ($nivo.Length -eq 2) { return [string]::IsNullOrEmpty($parent) }
    if ([string]::IsNullOrEmpty($parent)) { return $false }
    return $parent -eq $nivo.Substring(0, $nivo.Length - 2)
}

$trenutni = Q "select rtrim(Nivo) as Nivo, rtrim(Parent) as Parent from ssMeni where Parametar = N'$param'"
$vecDobar = $false
if ($trenutni.Rows.Count -eq 1) {
    $tn = [string]$trenutni.Rows[0].Nivo
    $tp = ""
    if (-not ($trenutni.Rows[0].Parent -is [DBNull])) { $tp = [string]$trenutni.Rows[0].Parent }
    if (ParentJeNivo $tn $tp) {
        $imaNivo = Q "select count(*) as n from ssMeni where rtrim(Nivo) = N'$tp'"
        if ($tn.Length -eq 2 -or [int]$imaNivo.Rows[0].n -gt 0) { $vecDobar = $true }
    }
}
if (-not $vecDobar) {
    if ($trenutni.Rows.Count -gt 0) { Exec "delete from ssMeni where Parametar = N'$param'" | Out-Null }
    $roditelji = Q @"
select p.MeniID, rtrim(p.Nivo) as Nivo, p.AplikacijeID, 2 as Korak
from ssMeni p
where len(rtrim(p.Nivo)) between 2 and $($nivoLen - 2)
  and exists (select 1 from ssMeni c where rtrim(c.Parent) = rtrim(p.Nivo))
order by len(rtrim(p.Nivo)), p.MeniID
"@
    if ($roditelji.Rows.Count -eq 0) { throw "Nijedan Nivo nema dijete po formuli left(nivo, len(nivo)-2). Ne gasim fk." }
    Write-Host ("roditelja s djecom: " + $roditelji.Rows.Count + " prvi MeniID=" + $roditelji.Rows[0].MeniID + " Nivo=" + $roditelji.Rows[0].Nivo + " korak=" + $roditelji.Rows[0].Korak)

    $kolone = Q @"
select c.name, c.is_identity, c.is_computed, ty.name as tip
from sys.columns c
join sys.types ty on c.user_type_id = ty.user_type_id
where c.object_id = object_id(N'dbo.ssMeni')
order by c.column_id
"@

    $uspio = $false
    foreach ($r in $roditelji.Rows) {
        $korak = [int]$r.Korak
        if ($korak -lt 1 -or $korak -gt 4) { continue }
        $baza = [string]$r.Nivo
        if (($baza.Length + $korak) -gt $nivoLen) { continue }
        $roditeljId = [int]$r.MeniID
        $kandidat = $null
        $granica = 1
        for ($z = 0; $z -lt $korak; $z++) { $granica *= 10 }
        for ($i = 1; $i -lt $granica; $i++) {
            $pokusaj = $baza + $i.ToString().PadLeft($korak, '0')
            $zauzet = Q "select count(*) as n from ssMeni where rtrim(Nivo) = N'$pokusaj'"
            if ([int]$zauzet.Rows[0].n -eq 0) { $kandidat = $pokusaj; break }
        }
        if ($kandidat -eq $null) { continue }

        $imena = New-Object System.Collections.Generic.List[string]
        $izrazi = New-Object System.Collections.Generic.List[string]
        foreach ($c in $kolone.Rows) {
            if ([int]$c.is_identity -eq 1 -or [int]$c.is_computed -eq 1) { continue }
            if ([string]$c.tip -eq "timestamp" -or [string]$c.tip -eq "rowversion") { continue }
            $ime = [string]$c.name
            $qime = "[" + $ime.Replace("]", "]]") + "]"
            $imena.Add($qime)
            if ($ime -eq "Nivo") { $izrazi.Add("N'$kandidat'") }
            elseif ($ime -eq "Parametar") { $izrazi.Add("N'$param'") }
            elseif ($ime -eq "Opis") { $izrazi.Add("N'Statusi'") }
            elseif ($ime -eq "Parent") { $izrazi.Add([string]$roditeljId) }
            else { $izrazi.Add("p." + $qime) }
        }
        $sql = "insert into ssMeni (" + ($imena -join ",") + ") select " + ($izrazi -join ",") + " from ssMeni p where p.MeniID = $roditeljId"
        Write-Host $sql
        try { Exec $sql | Out-Null } catch {
            Write-Host ("insert nije prosao za Nivo " + $kandidat + ": " + $_.Exception.Message)
            continue
        }
        $prov = Q "select rtrim(Nivo) as Nivo, rtrim(Parent) as Parent from ssMeni where Parametar = N'$param'"
        $pn = ""
        $pp = ""
        if ($prov.Rows.Count -eq 1) {
            $pn = [string]$prov.Rows[0].Nivo
            if (-not ($prov.Rows[0].Parent -is [DBNull])) { $pp = [string]$prov.Rows[0].Parent }
        }
        if ((ParentJeNivo $pn $pp) -and $pp -eq $baza) {
            Write-Host "ubacen Statusi.xml Nivo=$kandidat Parent=$pp (char(6), left nivo)"
            $uspio = $true
            break
        }
        Write-Host ("Parent za Nivo " + $kandidat + " je [" + $pp + "], ocekuje se Nivo [" + $baza + "]. Brisem i idem dalje.")
        Exec "delete from ssMeni where Parametar = N'$param'" | Out-Null
    }
    if (-not $uspio) { throw "Formula left(nivo, len(nivo)-2) nije dala Parent jednak roditeljskom Nivo. fk_meni nije gasen." }
}

# Parent je char(6): len(nivo)=2 -> NULL, inace left(nivo, len(nivo)-2).
# To nije MeniID. MeniID roditelja je red ciji je Nivo jednak tom Parentu.
# fk_meni je na AplikacijeID i ostaje ukljucen.
$provjera = Q @"
select c.MeniID, rtrim(c.Nivo) as Nivo, rtrim(c.Parent) as ParentNivo, p.MeniID as ParentMeniID
from ssMeni c
join ssMeni p on rtrim(p.Nivo) = rtrim(c.Parent)
where c.Parametar = N'$param'
"@
if ($provjera.Rows.Count -lt 1) { throw "Statusi.xml Parent ne vodi na MeniID. fk_meni nije gasen." }
$conn.Close()
Write-Host ("STA-12 ssMeni MeniID=" + $provjera.Rows[0].MeniID + " Nivo=" + $provjera.Rows[0].Nivo + " ParentNivo=" + $provjera.Rows[0].ParentNivo + " ParentMeniID=" + $provjera.Rows[0].ParentMeniID)
