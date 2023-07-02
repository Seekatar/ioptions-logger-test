[CmdletBinding()]
param(
    [ArgumentCompleter({
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameters)
        $runFile = (Join-Path (Split-Path $commandAst -Parent) run.ps1)
        if (Test-Path $runFile) {
            Get-Content $runFile |
                    Where-Object { $_ -match "^\s+'([\w+-]+)'\s*{" } |
                    ForEach-Object {
                        if ( !($fakeBoundParameters[$parameterName]) -or
                            (($matches[1] -notin $fakeBoundParameters.$parameterName) -and
                             ($matches[1] -like "$wordToComplete*"))
                            )
                        {
                            $matches[1]
                        }
                    }
        }
     })]
    [string[]]$Tasks
)

Push-Location $PSScriptRoot

try {

foreach ($task in $Tasks) {
    switch ($task) {
        'watchDefault' {  
            Set-Location src
            dotnet watch run --project .\options-logger-test.csproj -- -UseDefault
        }
        'watch' {  
            Set-Location src
            dotnet watch run --project .\options-logger-test.csproj
        }
        'generate' {
            if ((Test-Path ../swagger-codegen/Invoke-SwaggerGen.ps1)) {
                ../swagger-codegen/Invoke-SwaggerGen.ps1 -OASFile ./oas/openapi.yaml -Namespace IOptionTest -OutputFolder /mnt/c/temp/options -RenameController
            } else {
                Write-Warning 'Swagger codegen not found'
            }
        }
        'testDefault' {  
            $container = New-PesterContainer -Path . -Data @{ usedDefault='true' }
            Invoke-Pester -Container $container
        }
        'test' {  
            $container = New-PesterContainer -Path . -Data @{ usedDefault='false' }
            Invoke-Pester -Container $container
        }
        Default {
            Write-Warning "Unknown task: $task"
        }
    }
    
}

} finally {
    Pop-Location  
}