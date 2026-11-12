# Stavi klijentsko tijelo dbo.spFakPartneriUvjetiQuery (verzija A) na spin.
# Ne cita jupiter2019 ni jupiter2022. Tijelo se ne mijenja.
$ErrorActionPreference = "Stop"
$root = $env:GITHUB_WORKSPACE
if ([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path (Join-Path $root "scripts"))) {
    $root = (Get-Location).Path
}
$sql = Join-Path $root "scripts\spin\klijenti\spFakPartneriUvjetiQuery.A.sql"
if (-not (Test-Path -LiteralPath $sql)) { throw "Nema $sql" }

sqlcmd -S localhost -d spin -E -C -b -Q "IF OBJECT_ID(N'dbo.spFakPartneriUvjetiQuery', N'P') IS NOT NULL DROP PROCEDURE dbo.spFakPartneriUvjetiQuery;"
if ($LASTEXITCODE -ne 0) { throw "drop spFakPartneriUvjetiQuery nije prosao" }

sqlcmd -S localhost -d spin -E -C -b -f 65001 -i $sql
if ($LASTEXITCODE -ne 0) { throw "spFakPartneriUvjetiQuery.A.sql nije prosao" }

Write-Host "spin spFakPartneriUvjetiQuery = klijentska verzija A"
