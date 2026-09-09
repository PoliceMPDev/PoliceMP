[CmdletBinding()]
param(
    # Sets the KeyServer key, and heartbeat settings
    [Parameter()]
    [ValidateSet('local', 'dev', 'prodono', 'prd')]
    $Environment = "local"
)

& $PsScriptRoot/Get-FxServer.ps1

# Build
Push-Location -Path "$PSScriptRoot\src"
try {
    & dotnet build PoliceMp.sln
}
finally {
    Pop-Location
}

$host.UI.RawUI.WindowTitle = "PoliceMP $Environment Server"
Push-Location -Path "$PSScriptRoot\server\server-data"
try {
    & "$PSScriptRoot\server\server\FXServer.exe" +exec server.$Environment.cfg +set onesync on
}
finally {
    Pop-Location
}