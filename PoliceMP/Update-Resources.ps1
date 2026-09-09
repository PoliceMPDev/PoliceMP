#Requires -RunAsAdministrator

Push-Location $PSScriptRoot\..

$repo = "https://PoliceMP@dev.azure.com/PoliceMP/PoliceMP/_git/PoliceMP-Resources"
$symLink = "$PSScriptRoot\server\server-data\resources\[PoliceMP-Resources]"

try {
    if(-not (Test-Path -LiteralPath PoliceMP-Resources))
    {
        & git clone, $repo 2>&1
    }
    else {
        Push-Location "PoliceMP-Resources"
        try{
            & git checkout master 3>&1
            & git pull 3>&1
        }
        finally {
            Pop-Location
        }
    }

    if((Test-Path -LiteralPath $symLink) -and 
        (Get-Item -LiteralPath $symLink).LinkType -ne "SymbolicLink")
    {
        Remove-Item -LiteralPath $symLink -Recurse
    }

    if(-not (Test-Path -LiteralPath $symLink))
    {
        New-Item -Path (Split-Path $symLink) -Name (Split-Path $symLink -Leaf) -ItemType SymbolicLink -Value (Resolve-Path -Path .\PoliceMP-Resources)
    }

}
finally {
    Pop-Location
}