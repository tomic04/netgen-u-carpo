# Trag SQL-a koji klijent pošalje na sql-ai, hvatan na serveru (Extended Events), isto za stari NetGen i za Carpo.
# Hvata se samo promet s ovog računala i samo dok je sesija pokrenuta. Sesija nije trajna: Stop je obriše.
#
#   .\Trag.ps1 -Radnja Start
#   ... pokreni NetGenHost ili Carpo.TestShell ...
#   .\Trag.ps1 -Radnja Citaj -Izlaz trag.tsv     (vrijeme, aplikacija, vrsta, greška, tekst)
#   .\Trag.ps1 -Radnja Stop
param(
    [Parameter(Mandatory = $true)][ValidateSet("Start", "Citaj", "Stop")][string]$Radnja,
    [string]$Server = "sql-ai",
    [string]$Baza = "spin",
    [string]$Izlaz = "",
    [string]$Racunalo = $env:COMPUTERNAME
)
$ErrorActionPreference = "Stop"
$sesija = "NetGenUsporedba"

function Veza {
    $c = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=15;Application Name=NetGenUsporedba-trag")
    $c.Open()
    return $c
}
function Izvrsi($c, $sql) {
    $k = $c.CreateCommand(); $k.CommandText = $sql; $k.CommandTimeout = 60
    [void]$k.ExecuteNonQuery()
}

$c = Veza
try {
    if ($Radnja -eq "Start") {
        Izvrsi $c "if exists (select 1 from sys.server_event_sessions where name = N'$sesija') drop event session [$sesija] on server;"
        $filter = "[sqlserver].[client_hostname] = N'$Racunalo' and [sqlserver].[database_name] = N'$Baza' and [sqlserver].[client_app_name] <> N'NetGenUsporedba-trag'"
        $akcije = "action(sqlserver.client_app_name, sqlserver.session_id)"
        Izvrsi $c @"
create event session [$sesija] on server
add event sqlserver.sql_batch_completed($akcije where $filter),
add event sqlserver.rpc_completed($akcije where $filter),
add event sqlserver.error_reported($akcije where $filter and [severity] > 10)
add target package0.ring_buffer(set max_events_limit = 20000, max_memory = 32768)
with (max_dispatch_latency = 1 seconds, startup_state = off);
"@
        Izvrsi $c "alter event session [$sesija] on server state = start;"
        "sesija $sesija pokrenuta ($Racunalo, $Baza)"
    }
    elseif ($Radnja -eq "Stop") {
        Izvrsi $c "if exists (select 1 from sys.server_event_sessions where name = N'$sesija') drop event session [$sesija] on server;"
        "sesija $sesija obrisana"
    }
    else {
        Start-Sleep -Seconds 2
        $k = $c.CreateCommand(); $k.CommandTimeout = 120
        $k.CommandText = "select cast(t.target_data as nvarchar(max)) from sys.dm_xe_sessions s join sys.dm_xe_session_targets t on t.event_session_address = s.address where s.name = N'$sesija' and t.target_name = N'ring_buffer'"
        $xmlTekst = [string]$k.ExecuteScalar()
        if ([string]::IsNullOrEmpty($xmlTekst)) { throw "Sesija $sesija nije pokrenuta." }
        $x = [xml]$xmlTekst
        $redovi = New-Object System.Collections.Generic.List[string]
        foreach ($e in $x.SelectNodes("/RingBufferTarget/event")) {
            $vrsta = $e.GetAttribute("name")
            $app = ""; $spid = ""; $tekst = ""; $greska = ""
            # Vrijednost je u podčvoru <value> (ili <text> za nabrajanja); $d.value zna vratiti XmlElement, pa se čita InnerText.
            $vr = { param($n, $ime) $v = $n.SelectSingleNode($ime); if ($v) { $v.InnerText } else { "" } }
            foreach ($a in $e.SelectNodes("action")) {
                if ($a.GetAttribute("name") -eq "client_app_name") { $app = & $vr $a "value" }
                if ($a.GetAttribute("name") -eq "session_id") { $spid = & $vr $a "value" }
            }
            foreach ($d in $e.SelectNodes("data")) {
                $ime = $d.GetAttribute("name")
                if ($ime -eq "batch_text" -or $ime -eq "statement") { $tekst = & $vr $d "value" }
                if ($ime -eq "message") { $greska = & $vr $d "value" }
                if ($ime -eq "result" -and $vrsta -ne "error_reported") {
                    $r = & $vr $d "text"
                    if ($r -and $r -ne "OK") { $greska = $r }
                }
            }
            if ($vrsta -eq "error_reported") { $tekst = "" }
            $cist = { param($t) (($t -replace "\s+", " ").Trim()) }
            $vrijeme = ([datetime]$e.GetAttribute("timestamp")).ToLocalTime().ToString("HH:mm:ss.fff")
            $redovi.Add(($vrijeme, $app, $spid, $vrsta, (& $cist $greska), (& $cist $tekst)) -join "`t")
        }
        if ($Izlaz) {
            [IO.File]::WriteAllLines($Izlaz, $redovi, (New-Object System.Text.UTF8Encoding($false)))
            "zapisano $($redovi.Count) redaka u $Izlaz"
        } else { $redovi }
    }
}
finally { $c.Close() }
