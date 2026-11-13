# Otvaranje master SQL-a na spinu. Jupiter se ne dira.
# Ako fali procedura ili stupac, na spinu se doda prazan objekt (samo SELECT / NULL stupac) i pokusa se jos jednom.
$ErrorActionPreference = "Continue"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path (Join-Path $root "definitions"))) {
    $root = (Get-Location).Path
}
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izlaz = Join-Path $outDir "otvaranje.md"
$stubLog = Join-Path $outDir "stubovi-kreirani.md"

$imena = @(
    "OznakaKL","KUPNG","Statusi","KntBlaZak","PrijemniList","Kulture","FakPar","AOPNovi",
    "DobUgovori","Intrastat","JeziciNG","KUPDostKN","KUPDostKnNGBvg","KarticaPartnera",
    "KntBlaPromet","KntDospjeleStavke","KntPDVObrasciEU","LVObracun","MPBlagajna","MPInventura",
    "MPNGPregDokMAIL","MPNarIzdSkl","MPPonuda","MPPorKPNG","MPRACNEFISK"
)

function Zamijeni([string]$sql) {
    if ([string]::IsNullOrWhiteSpace($sql)) { return "" }
    $sql = $sql.Replace("@@operateriid", "7").Replace("@@OPERATERIID", "7").Replace("@@Operateriid", "7")
    $sql = $sql.Replace("@@id", "0").Replace("@@ID", "0").Replace("@@Id", "0")
    $sql = $sql.Replace("@@node", "null").Replace("@@NODE", "null")
    $sql = $sql.Replace("@@detid", "0").Replace("@@DETID", "0")
    return $sql.Trim()
}

function NadjiXml([string]$ime) {
    $korijen = Join-Path $root "definitions\netgen"
    $hit = Get-ChildItem -LiteralPath $korijen -Filter ($ime + ".xml") -Recurse -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($hit -eq $null) { return $null }
    return $hit.FullName
}

$cs = "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-otvaranje"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()
$kreirano = New-Object System.Collections.Generic.List[string]

function Exec([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    return $cmd.ExecuteNonQuery()
}

function Otvori([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = $sql
    $rd = $cmd.ExecuteReader()
    $n = 0
    $kol = $rd.FieldCount
    while ($rd.Read()) { $n++; if ($n -ge 3) { break } }
    $rd.Close()
    return "$n redova, $kol stupaca"
}

function StubProc([string]$ime, [string]$poziv) {
    $seen = @{}
    $decl = @()
    foreach ($m in [regex]::Matches($poziv, '@([A-Za-z_][A-Za-z0-9_]*)')) {
        $n = $m.Groups[1].Value
        $k = $n.ToLowerInvariant()
        if ($seen.ContainsKey($k)) { continue }
        $seen[$k] = $true
        $decl += "@$n nvarchar(200) = NULL"
    }
    $lista = ""
    if ($decl.Count -gt 0) { $lista = "`r`n    " + ($decl -join ",`r`n    ") }
    $sql = "IF OBJECT_ID(N'dbo.$ime', N'P') IS NULL EXEC(N'CREATE PROCEDURE dbo.[$ime]$lista AS BEGIN SET NOCOUNT ON; SELECT CAST(NULL AS int) AS id WHERE 1 = 0; END');"
    Exec $sql | Out-Null
    $kreirano.Add("procedura dbo.$ime (prazan SELECT, ne pise)")
    Write-Host "stub procedura $ime"
}

function StubStupac([string]$tablica, [string]$stupac) {
    $cisto = $tablica.Trim().Trim('[',']')
    if ($cisto -notmatch '^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)?$') { return }
    $sql = "IF OBJECT_ID(N'$cisto', N'U') IS NOT NULL AND COL_LENGTH(N'$cisto', N'$stupac') IS NULL ALTER TABLE $cisto ADD [$stupac] varchar(50) NULL;"
    Exec $sql | Out-Null
    $kreirano.Add("stupac $cisto.$stupac varchar(50) NULL")
    Write-Host "stub stupac $cisto.$stupac"
}

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("# Otvaranje na spinu")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Jupiter se ne dira. Nedostajuci stupac ili procedura na spinu dobije prazan objekt i otvaranje se ponovi jednom.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Granula | Ishod | Poruka |")
[void]$sb.AppendLine("|---|---|---|")

foreach ($ime in $imena) {
    $put = NadjiXml $ime
    if ($put -eq $null) {
        [void]$sb.AppendLine("| $ime | nema-xml | |")
        Write-Host "$ime nema-xml"
        continue
    }
    [xml]$doc = Get-Content -LiteralPath $put -Encoding UTF8
    $raw = $doc.DocumentElement.GetAttribute("ONLOAD")
    if ([string]::IsNullOrWhiteSpace($raw)) { $raw = $doc.DocumentElement.GetAttribute("STARTUPSTORA") }
    $sql = Zamijeni $raw
    if ($sql.StartsWith(":")) {
        [void]$sb.AppendLine("| $ime | forma | $sql |")
        Write-Host "$ime forma"
        continue
    }
    if ([string]::IsNullOrWhiteSpace($sql)) {
        [void]$sb.AppendLine("| $ime | prazno | |")
        Write-Host "$ime prazno"
        continue
    }
    $provjera = $sql.Replace("~~", ",")
    if ($provjera.Trim().StartsWith("@") -or $provjera.IndexOf("@@") -gt 0) {
        [void]$sb.AppendLine("| $ime | preskok | ExecSql ne salje dok ostane @@ |")
        Write-Host "$ime preskok"
        continue
    }
    $ishod = "pad"
    $poruka = ""
    for ($pokusaj = 1; $pokusaj -le 2; $pokusaj++) {
        try {
            $poruka = Otvori $sql
            $ishod = "otvoreno"
            break
        } catch {
            $poruka = $_.Exception.InnerException
            if ($poruka -eq $null) { $poruka = $_.Exception.Message } else { $poruka = $poruka.Message }
            if ($pokusaj -eq 2) { break }
            $m = [regex]::Match([string]$poruka, "Could not find stored procedure '([^']+)'")
            if ($m.Success) { StubProc ($m.Groups[1].Value -replace '^dbo\.', '') $sql; continue }
            $c = [regex]::Match([string]$poruka, "Invalid column name '([^']+)'")
            $f = [regex]::Match($sql, '(?i)\bfrom\s+(\[?[A-Za-z_][A-Za-z0-9_]*\]?(?:\.\[?[A-Za-z_][A-Za-z0-9_]*\]?)?)')
            if ($c.Success -and $f.Success) { StubStupac $f.Groups[1].Value $c.Groups[1].Value; continue }
            break
        }
    }
    $kratko = ([string]$poruka -replace "\|", "/" -replace "`r|`n", " ")
    if ($kratko.Length -gt 180) { $kratko = $kratko.Substring(0, 180) }
    [void]$sb.AppendLine("| $ime | $ishod | $kratko |")
    Write-Host "$ime $ishod $kratko"
}

$klijenti = Join-Path $root "xml\klijenti"
if (Test-Path -LiteralPath $klijenti) {
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## Klijentski XML")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| Datoteka | Ishod | Poruka |")
    [void]$sb.AppendLine("|---|---|---|")
    $enc = [System.Text.Encoding]::GetEncoding(1250)
    Get-ChildItem -LiteralPath $klijenti -Filter *.xml | Sort-Object Name | ForEach-Object {
        $ime = $_.Name
        $tekst = $enc.GetString([System.IO.File]::ReadAllBytes($_.FullName))
        $doc = New-Object System.Xml.XmlDocument
        $doc.XmlResolver = $null
        try { $doc.LoadXml($tekst) } catch {
            [void]$sb.AppendLine("| $ime | xml | $($_.Exception.Message) |")
            return
        }
        $raw = $doc.DocumentElement.GetAttribute("ONLOAD")
        if ([string]::IsNullOrWhiteSpace($raw)) { $raw = $doc.DocumentElement.GetAttribute("STARTUPSTORA") }
        $sql = Zamijeni $raw
        if ([string]::IsNullOrWhiteSpace($sql)) {
            [void]$sb.AppendLine("| $ime | prazno | nema ONLOAD ni STARTUPSTORA |")
            return
        }
        if ($sql.StartsWith(":")) {
            [void]$sb.AppendLine("| $ime | forma | $sql |")
            return
        }
        $provjera = $sql.Replace("~~", ",")
        if ($provjera.Trim().StartsWith("@") -or $provjera.IndexOf("@@") -gt 0) {
            [void]$sb.AppendLine("| $ime | preskok | ExecSql ne salje dok ostane @@ |")
            return
        }
        try {
            $poruka = Otvori $sql
            [void]$sb.AppendLine("| $ime | otvoreno | $poruka |")
        } catch {
            $poruka = $_.Exception.Message
            $kratko = ([string]$poruka -replace "\|", "/" -replace "`r|`n", " ")
            if ($kratko.Length -gt 180) { $kratko = $kratko.Substring(0, 180) }
            [void]$sb.AppendLine("| $ime | pad | $kratko |")
        }
    }
}

$conn.Close()
[System.IO.File]::WriteAllText($izlaz, $sb.ToString(), (New-Object System.Text.UTF8Encoding $false))
$stubTxt = if ($kreirano.Count -eq 0) { "Nijedan novi stub." } else { ($kreirano -join "`r`n") }
[System.IO.File]::WriteAllText($stubLog, $stubTxt, (New-Object System.Text.UTF8Encoding $false))
Write-Host "Izvjestaj: $izlaz"
exit 0
