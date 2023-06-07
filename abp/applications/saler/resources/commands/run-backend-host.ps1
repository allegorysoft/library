$commandFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = (Get-Item -Path "../../" -Verbose).FullName
$apiPath =(Join-Path $rootFolder 'src/aspnet-core/src/Allegory.Saler.HttpApi.Host')
$migrationPath =(Join-Path $rootFolder 'src/aspnet-core/src/Allegory.Saler.DbMigrator')

Set-Location $migrationPath
Write-Host "Migration starting..."
dotnet run
Set-Location $apiPath
Write-Host "Application starting..."
dotnet run
Set-Location $commandFolder
#Read-Host -Prompt "Press any key to continue"