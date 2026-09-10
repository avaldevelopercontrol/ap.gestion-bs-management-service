$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    Write-Host '== Repository hygiene =='
    git diff --check
    if ($LASTEXITCODE -ne 0) { throw 'git diff --check failed.' }

    $sqlFiles = Get-ChildItem .\src -Recurse -Filter '*Sql.cs' -File
    if ($sqlFiles) {
        $sqlFiles | ForEach-Object { Write-Host $_.FullName }
        throw 'Analytics migration must not leave *Sql.cs files.'
    }

    $sourceFiles = Get-ChildItem .\src -Recurse -Include '*.cs','*.csproj' -File |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }

    $legacyPersistence = $sourceFiles | Select-String -Pattern @(
        '\bDapper\b',
        'IAnalyticsQueryExecutor',
        'SqlClientAnalyticsQueryExecutor',
        'IAnalyticsDbConnectionFactory',
        'SqlAnalyticsDbConnectionFactory',
        'Microsoft\.Data\.SqlClient'
    )

    if ($legacyPersistence) {
        $legacyPersistence | ForEach-Object { Write-Host $_.Path ':' $_.LineNumber ':' $_.Line.Trim() }
        throw 'Legacy Dapper/SqlClient Analytics persistence was detected.'
    }

    $manualReadSql = Get-ChildItem .\src -Recurse -Filter '*.cs' -File |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } |
        Select-String -CaseSensitive -Pattern '\b(SELECT|UPDATE|DELETE FROM|MERGE)\b'

    if ($manualReadSql) {
        $manualReadSql | ForEach-Object { Write-Host $_.Path ':' $_.LineNumber ':' $_.Line.Trim() }
        throw 'Manual read/update SQL was detected in C# source.'
    }

    Write-Host '== Release build and tests =='
    dotnet restore .\API.BS.GestionManagement.slnx
    dotnet build .\API.BS.GestionManagement.slnx -c Release --no-restore
    dotnet test .\src\GesMgmt.UnitTests\GesMgmt.UnitTests.csproj -c Release --no-build

    Write-Host 'Analytics EF Core integration verification OK.'
}
finally {
    Pop-Location
}
