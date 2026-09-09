$fivemLocation = Join-Path -Path $env:LOCALAPPDATA -ChildPath 'FiveM\FiveM.exe'

Get-Process FiveM -ErrorAction SilentlyContinue | Stop-Process

Start-Process -FilePath $fivemLocation -ArgumentList '-cl1'

Start-Sleep 10
Start-Process -FilePath $fivemLocation -ArgumentList '-cl2'

Register-EngineEvent -SourceIdentifier 'Exiting' -Action {
    Get-Process FiveM | Stop-Process
}

& $PsScriptRoot/Start-Server.ps1

Get-Process | Where-Object Name -like FiveM* | Stop-Process