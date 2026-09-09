[CmdletBinding()]
param(
    [switch]
    $Developer
)

$fivemLocation = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'FiveM\FiveM.exe'

Get-Process FiveM -ErrorAction SilentlyContinue | Stop-Process

$arguments = ,'-cl1'
if($Developer.IsPresent)
{
    $arguments += '+set', 'moo', '31337'
}

Start-Process -FilePath $fivemLocation -ArgumentList $arguments

Register-EngineEvent -SourceIdentifier 'Exiting' -Action {
    Get-Process FiveM | Stop-Process
}

& $PsScriptRoot/Start-Server.ps1

Get-Process | Where-Object Name -like FiveM* | Stop-Process