# M5 (XML-01): koji XML-ovi granula postoje u JupiterXML.dbo.netgenxml na jupiter2019/jupiter2022.
# Samo citanje: ime XML-a i velicina tijela (bez dekripcije i bez sadrzaja). Ne mijenja nista.
# Izlaz: artifacts\granule\netgenxml.md (uz izvjestaj punog otvaraca, da se vidi koje granule
# iz ssMeni nemaju datoteku u repou, a postoje u bazi).
$ErrorActionPreference = "Stop"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root)) { $root = (Get-Location).Path }
$outDir = Join-Path $root "artifacts\granule"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$md = Join-Path $outDir "netgenxml.md"
$linije = New-Object System.Collections.Generic.List[string]
$linije.Add("# JupiterXML.dbo.netgenxml")
$linije.Add("")
$linije.Add("Samo citanje (ime i velicina tijela). Application Name=CarpoNetGen-netgenxml.")
$linije.Add("")
foreach ($server in @("jupiter2019", "jupiter2022")) {
    $linije.Add("## $server")
    $linije.Add("")
    try {
        $cs = "Server=$server;Database=JupiterXML;Integrated Security=True;TrustServerCertificate=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-netgenxml"
        $c = New-Object System.Data.SqlClient.SqlConnection $cs
        $c.Open()
        try {
            $cmd = $c.CreateCommand()
            $cmd.CommandTimeout = 30
            $cmd.CommandText = "select xml, datalength(body) as velicina from dbo.netgenxml order by xml"
            $r = $cmd.ExecuteReader()
            $n = 0
            $linije.Add("| XML | bajtova |")
            $linije.Add("|---|---:|")
            while ($r.Read()) {
                $linije.Add("| " + [string]$r["xml"] + " | " + [string]$r["velicina"] + " |")
                $n++
            }
            $r.Close()
            $linije.Insert($linije.Count - $n - 2, "Redaka: $n")
            $linije.Insert($linije.Count - $n - 2, "")
        } finally { $c.Close() }
    } catch {
        $linije.Add("Nije procitano: " + $_.Exception.Message)
    }
    $linije.Add("")
}
[System.IO.File]::WriteAllLines($md, $linije, (New-Object System.Text.UTF8Encoding($false)))
Get-Content -LiteralPath $md -TotalCount 30
