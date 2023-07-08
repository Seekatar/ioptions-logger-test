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
    [string[]]$Tasks,
    [ValidateSet('SchemeA', 'SchemeB', 'SchemeC', 'forward', IgnoreCase = $false)]
    [string] $DefaultAuthScheme
)

Push-Location $PSScriptRoot

$prevSuppress = $env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER
$env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=1

try {

foreach ($task in $Tasks) {
    switch ($task) {
        'watch' {  
            Set-Location src
            dotnet watch run --project .\options-logger-test.csproj -- $DefaultAuthScheme
        }
        'generate' {
            if ((Test-Path ../swagger-codegen/Invoke-SwaggerGen.ps1)) {
                ../swagger-codegen/Invoke-SwaggerGen.ps1 -OASFile ./oas/openapi.yaml -Namespace IOptionTest -OutputFolder /mnt/c/temp/options -RenameController
            } else {
                Write-Warning 'Swagger codegen not found'
            }
        }
        'test' {  
            $container = New-PesterContainer -Path . -Data @{ DefaultAuthScheme = $DefaultAuthScheme ? $DefaultAuthScheme : '-' } # can't be empty
            Invoke-Pester -Container $container
        }
        Default {
            Write-Warning "Unknown task: $task"
        }
    }
    
}

} finally {
    Pop-Location  
    $env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=$prevSuppress
}