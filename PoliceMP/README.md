# FMServer

## Starting the server

1. Open PowerShell as administrator and run `Set-ExecutionPolicy Unrestricted` if you haven't done so already.
2. Download FXServer by running `Get-FXServer.ps1`. (You can specify a version by running `Get-FXServer.ps1 -Version 2967` and list version by running `Get-FXServer.ps1 -ListVersion`). (YOU NEED 7ZIP)
3. The server will be downloaded to `/server/server`.
4. Run the server via `run.cmd`


## Requirements:
 - .Net Framework 4.5.1 Development Pack (https://dotnet.microsoft.com/en-us/download/dotnet-framework/net451)
 - .Net Framework 4.5.2 Development Pack (https://dotnet.microsoft.com/en-us/download/dotnet-framework/net452)
 - .Net Framework 4.6.1 Development Pack (https://dotnet.microsoft.com/en-us/download/dotnet-framework/net461)
 - Node JS (https://nodejs.org/en/download/) (NEEDS TO BE VERSION: 16.20.1)
 - 7Zip (https://7-zip.org/download.html) (Used to extract FiveM artifacts)
<br>
<br>
Got errors for packages? <br>
 - Clean solution
 - Restore NuGet Packages

<br>
 How to make symbolic link? (Not loading other resources)<br>
 1. Admin PowerShell.
 2. Cd to resources e.g. D:\PoliceMP\server\server-data\resources.
 3. New-Item -ItemType SymbolicLink -Path "[PoliceMP-Resources]" -Target "D:\PoliceMP-Resources".
<br>
 (Update-Resources.ps1 should do all this for you)<br>