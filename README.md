# ioptions-logger-test
Test for IOptions, IConfiguration, and ILogger

## Running the Options

```powershell
(irm http://localhost:5138/api/config &&
 irm http://localhost:5138/api/config/section &&
 irm http://localhost:5138/api/options/monitored &&
 irm http://localhost:5138/api/options/snapshot &&
 irm http://localhost:5138/api/options) | ft
```

## Logging All Levels

```powershell
1..5 | % { irm http://localhost:5138/api/logger -Method POST -Body "{`"level`": $_,`"_Message`":`"hi'`"}" -ContentType 'application/json' }
```

## Generating From the OAS file

```powershell
../swagger-codegen/Invoke-SwaggerGen.ps1 -OASFile ./oas/openapi.yaml -Namespace IOptionTest -OutputFolder /mnt/c/temp/options -RenameController
```

