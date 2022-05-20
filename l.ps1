$uri = "http://localhost:5138/api/"
1..6 | % { irm "${uri}logger" -Method POST -Body "{`"level`": $_,`"_Message`":`"hi'`"}" -ContentType 'application/json' }
