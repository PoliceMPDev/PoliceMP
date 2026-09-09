[CmdletBinding()]
param(
    [Parameter()]
    [string]
    $Tag = "policemp"
)

Push-Location $PSScriptRoot

if(Test-Path -Path out)
{
    Remove-Item out -Recurse | Out-Null
}

& dotnet build src/PoliceMP.Client -o out/resources/PoliceMP/client
& dotnet build src/PoliceMP.Server -o out/resources/PoliceMP/server

Copy-Item out/resources/PoliceMP/client/fxmanifest.lua out/resources/PoliceMP/fxmanifest.lua -Force

Get-ChildItem -Path server/server-data | ? Name -NotIn cache, resources | Copy-Item -Destination out -Force

& docker build . -t $Tag

Pop-Location