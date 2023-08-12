Set-Location $PSScriptRoot
dotnet pack
$sourcePath = Join-Path $PSScriptRoot -ChildPath nupkg
dotnet tool install --global --no-cache --add-source $sourcePath Empowered.Dataverse.Connection.Tool
dotnet tool update --global --no-cache --add-source $sourcePath Empowered.Dataverse.Connection.Tool