$commandFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = (Get-Item -Path "../../" -Verbose).FullName
$appPath =(Join-Path $rootFolder 'src/angular')

Set-Location $appPath
Write-Host "Application starting..."
npm install --legacy-peer-deps
npx ng s
Set-Location $commandFolder