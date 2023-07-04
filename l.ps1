param( [int] $logCount = 100)

$uri = "http://localhost:5138/api/"
1..6 | % { "Calling with logLevel $_"; irm "${uri}logger" -Method POST -Body "{`"level`": $_,`"Text`":`"hi'`"}" -ContentType 'application/json' -Headers @{ "X-Test-Role" = "A" } }

irm "${uri}logger/$logCount" -Method POST -Body "{`"level`": 3,`"Text`":`"hi'`"}" -ContentType 'application/json'

