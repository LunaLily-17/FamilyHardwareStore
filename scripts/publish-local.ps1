$output = ".\artifacts\publish"
dotnet publish .\src\HardwareStore.Web -c Release -o $output
Write-Host "Published to $output"
