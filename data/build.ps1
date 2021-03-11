Install-Module ImportExcel -scope CurrentUser
$csvs = Get-ChildItem .\* -Include *.csv
$csvCount = $csvs.Count
Write-Host "Detected the following CSV files: ($csvCount)"
foreach ($csv in $csvs) {
    Write-Host " -"$csv.Name
}

$excelFileName = "all.xlsx"
Write-Host "Creating: $excelFileName"

foreach ($csv in $csvs) {
    $csvPath = ".\" + $csv.Name
    $worksheetName = $csv.Name.Replace(".csv","")
    Write-Host " - Adding $worksheetName to $excelFileName"
    Import-Csv -Path $csvPath -Delimiter ";"  | Export-Excel -Path $excelFileName -WorkSheetname $worksheetName
}