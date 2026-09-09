[CmdletBinding()]
param(
    # Sets the KeyServer key, and heartbeat settings
    [Parameter()]
    [ValidateSet('Production', 'Development')]
    $Environment = "Development"
)

$arguments = @(
    '+exec', 'server.cfg'
)

switch($Environment)
{
    'Development' {
        $env:Server__LogLevel = "Trace"
        $arguments += '+set', 'onesync', 'on'
        $arguments += '+set', 'onesync_enabled', '1'
        $arguments += '+set', 'sv_master', 'dont_broadcast_pls'
        $arguments += '+set', 'sv_licensekey', '"yrjccgrbjtdththlmresazevd7h1l3p9"'
    }

    'Production' {
        $arguments += '+set', 'sv_licensekey', '"jzf9i17h2p39wyhxa5japycdm1rv65j9"'
        $arguments += '+set', 'onesync', 'on'
        $arguments += '+set', 'onesync_enabled', '1'
    }
}

$host.UI.RawUI.WindowTitle = "PoliceMP $Environment Server"
Push-Location -Path "$PSScriptRoot\server\server-data"
try {
    & "$PSScriptRoot\server\server\FXServer.exe" $arguments
}
finally {
    Pop-Location
}