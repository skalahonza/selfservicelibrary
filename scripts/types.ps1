param ($csvFile)
Import-Csv $csvFile -Delimiter ";" | Select-Object DruhPublikace -Unique