$csvFile = Get-ChildItem -Path "..\data\*" -Include "*.csv"
Import-Csv $csvFile -Header (1..31) -Delimiter ";" | Select-Object -ExpandProperty  '4' -Unique | Sort-Object