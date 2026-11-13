# Cita tijelo dbo.spJeziciQuery samo s localhost/spin.
# Ne otvara jupiter2019 ni jupiter2022. Ne izvrsava proceduru. Ne pise redove.
# Isti filtar kao Citanje.RazlogTijela: komentar, zaglavlje CREATE/ALTER PROC i WITH EXECUTE AS nisu upis.
$ErrorActionPreference = "Continue"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root)) { $root = (Get-Location).Path }
$outDir = Join-Path $root "artifacts\spin"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$izlaz = Join-Path $outDir "jezici-tijelo.md"

function Skini([string]$t) {
    if ([string]::IsNullOrWhiteSpace($t)) { return "" }
    $t = [regex]::Replace($t, '(?s)/\*.*?\*/', ' ')
    $t = [regex]::Replace($t, '(?m)--.*?$', ' ')
    $t = [regex]::Replace($t, '(?i)^\s*(create|alter)\s+(proc|procedure|function|view)\b', ' ')
    $t = [regex]::Replace($t, '(?i)\bwith\s+execute\s+as\s+[A-Za-z_][A-Za-z0-9_]*', ' ')
    $t = [regex]::Replace($t, '(?i)\bexecute\s+as\s+(owner|caller|self)\b', ' ')
    $t = [regex]::Replace($t, "(?i)\bexecute\s+as\s+user\s*=\s*N?'[^']*'", ' ')
    return $t
}

function Pogoci([string]$t) {
    $kljuc = @("insert","update","delete","merge","drop","alter","truncate","exec","execute","into")
    $n = @()
    foreach ($k in $kljuc) {
        $m = [regex]::Match($t, "(?i)\b$k\b")
        if ($m.Success) { $n += $k }
    }
    return $n
}

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("# Tijelo spJeziciQuery na spinu")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Samo localhost, baza spin. Jupiter se ne cita. Procedura se ne pokrece.")
[void]$sb.AppendLine("")

$cs = "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-tijelo;ApplicationIntent=ReadOnly"
try {
    $conn = New-Object System.Data.SqlClient.SqlConnection $cs
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 30
    $cmd.CommandText = "select object_definition(object_id(N'dbo.spJeziciQuery'))"
    $def = $cmd.ExecuteScalar()
    $tekst = ""
    if ($def -ne $null -and $def -isnot [DBNull]) { $tekst = [string]$def }
    if ([string]::IsNullOrWhiteSpace($tekst)) {
        [void]$sb.AppendLine("- tijelo: nema (object_definition je prazan)")
    } else {
        $cisto = Skini $tekst
        $sirovo = Pogoci $tekst
        $ostalo = Pogoci $cisto
        [void]$sb.AppendLine("- duljina tijela: $($tekst.Length)")
        [void]$sb.AppendLine("- pogoci u sirovom tekstu: $(if ($sirovo.Count -eq 0) { 'nema' } else { $sirovo -join ', ' })")
        [void]$sb.AppendLine("- pogoci nakon komentara, zaglavlja i EXECUTE AS: $(if ($ostalo.Count -eq 0) { 'nema' } else { $ostalo -join ', ' })")
        if ($ostalo.Count -eq 0) {
            [void]$sb.AppendLine("- dokaz: tijelo je samo citanje. NetGen poziv salje (Form1.vb 3882, Class1.vb 113-118).")
        } else {
            [void]$sb.AppendLine("- dokaz: tijelo nije samo citanje. Port poziv ne salje. NetGen bi ga poslao.")
        }
    }

    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## Objekti DOS-11")
    $cmd.CommandText = "select name from sys.objects where name in (N'spKntPopuniKontroleNG', N'spOrgShemaQuery', N'spKntOtvStavkeQueryNG') order by name"
    $rd = $cmd.ExecuteReader()
    $ima = @()
    while ($rd.Read()) { $ima += [string]$rd.GetValue(0) }
    $rd.Close()
    [void]$sb.AppendLine("- na spinu: $(if ($ima.Count -eq 0) { 'nijedan' } else { $ima -join ', ' })")
    [void]$sb.AppendLine("- fale: spKntPopuniKontroleNG, spOrgShemaQuery, spKntOtvStavkeQueryNG koji nisu u retku iznad. Tijela se ne izmišljaju.")

    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("## ssMeni")
    $fajlovi = @("FakPar.xml","KntDospjeleStavke.xml","Kulture.xml","KUPDostKN.xml","KUPDostKnNGBvg.xml","KUPNG.xml","MPNarIzdSkl.xml","MPNGPregDokMAIL.xml","RatArkod.xml","RobaWebKlase.xml")
    foreach ($f in $fajlovi) {
        $cmd.Parameters.Clear()
        $cmd.CommandText = "select count(*) from ssMeni where Parametar like @a or Parametar like @b or Parametar like @c"
        $null = $cmd.Parameters.AddWithValue("@a", "%:" + $f)
        $null = $cmd.Parameters.AddWithValue("@b", "%\" + $f)
        $null = $cmd.Parameters.AddWithValue("@c", "%/" + $f)
        $n = [int]$cmd.ExecuteScalar()
        [void]$sb.AppendLine("- ${f}: $n")
    }
    $conn.Close()
} catch {
    [void]$sb.AppendLine("- greska citanja: $($_.Exception.Message)")
}

[System.IO.File]::WriteAllText($izlaz, $sb.ToString(), (New-Object System.Text.UTF8Encoding $false))
Write-Host "Tijelo: $izlaz"
Get-Content -LiteralPath $izlaz
