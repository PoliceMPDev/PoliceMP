# Get the current directory
$scriptpath = $MyInvocation.MyCommand.Path
$rootDirectory = Split-Path $scriptpath

# Define a list of file extensions to exclude
$excludeExtensions = @(".fxap", ".ymap", ".lua", ".ymf", ".txt", ".meta")

# Create a hashtable to store file names and their paths
$files = @{}

# Create a global variable to keep track of duplicate files
$global:duplicateCount = 0

# Function to check for duplicate files
function CheckForDuplicateFiles {
    param (
        [string]$path
    )

    # Get all files in the directory and subdirectories, excluding certain extensions
    $filesInFolder = Get-ChildItem -LiteralPath $path -Recurse -File | Where-Object {
        $excludeExtensions -notcontains [System.IO.Path]::GetExtension($_.Name)
    }

    # Check for duplicates
    foreach ($file in $filesInFolder) {
        if ($files.ContainsKey($file.Name)) {
            Write-Host ""
            Write-Host "Duplicate file found:"
            Write-Host "Original: $($files[$file.Name])"
            Write-Host "Duplicate: $($file.FullName)"
            Write-Host ""
            $global:duplicateCount++
        } else {
            $files[$file.Name] = $file.FullName
        }
    }
}

# Start the search from the current directory
CheckForDuplicateFiles -path $rootDirectory

# Display the total count of duplicate files
Write-Host "Total duplicate files found: $global:duplicateCount"
Read-Host "Belter"