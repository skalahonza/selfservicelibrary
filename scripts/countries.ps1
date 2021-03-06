$csvFile = Get-ChildItem -Path "..\data\*" -Include "*.csv"
Import-Csv $csvFile -Header (1..31) -Delimiter ";" | Select-Object -ExpandProperty  '14' -Unique