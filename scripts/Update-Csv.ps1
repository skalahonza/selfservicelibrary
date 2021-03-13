Param
(
    [parameter(Position=0, Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
    [String]$Downloadurl = 'http://cyber.felk.cvut.cz/novakpe/Skola/Knihovna/ToNewDB/',
    [parameter(Position=1, Mandatory=$true,ValueFromPipelineByPropertyName=$true)]
    [alias('DownloadPath')]
    [String]$DownloadToFolder = 'C:\Temp\test'
)

function Join-UriParts ($base, $path)
{
    return [Uri]::new([Uri]::new($base), $path).ToString()
}

$HttpContent = Invoke-WebRequest -URI $Downloadurl
$csvs = $HttpContent.Links | Where-Object href -match "csv" | Select-Object -Expand href 

$WebClient = New-Object System.Net.WebClient

foreach ($file in $csvs) {    
    $source = Join-UriParts $Downloadurl $file
    $destination = Join-Path $DownloadToFolder $file
    Write-Host "Downloading $source to $destination"
    $WebClient.DownloadFile($source, (Resolve-Path $destination))
}