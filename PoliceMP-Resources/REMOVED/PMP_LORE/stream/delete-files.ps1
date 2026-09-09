# Create the LORE folder if it doesn't exist
$destinationFolder = ".\LORE"
if (!(Test-Path $destinationFolder)) {
    New-Item -ItemType Directory -Path $destinationFolder
}

# Read the list of file names from filelist.txt
Get-Content "filelist.txt" | ForEach-Object {
    $fileName = $_
    # Search for any file with that name (ignoring extension) in the folder and subfolders
    $files = Get-ChildItem -Path . -File -Recurse | Where-Object { $_.BaseName -eq $fileName }

    if ($files) {
        foreach ($file in $files) {
            Move-Item $file.FullName -Destination $destinationFolder -Force
            Write-Host "Moved: $($file.FullName) to $destinationFolder"
        }
    } else {
        Write-Host "$fileName not found"
    }
}
