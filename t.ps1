[CmdletBinding()]
param (
    [switch] $Correlation
)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$headers = @{
    "Accept" = "application/json"
}
if ($Correlation) {
    $headers = @{
        "Content-Type"     = "application/json"
        "X-Correlation-Id" = [Guid]::NewGuid().ToString()
        "Accept"           = "application/json"
    }
}
$uri = "http://localhost:5138/api/"

$clientId = [Guid]::NewGuid()
$marketId = 1
Write-Host "------------ Throw "
$result = Invoke-WebRequest "${uri}throw/details/$clientId/$marketId" -Headers $headers -SkipHttpErrorCheck -useBasicParsing
"Status is $($result.StatusCode)"
Write-Host $result.Headers
Write-Host ($result.ToString() | ConvertFrom-Json | out-string)

Write-Host "------------ Throw Log"
$result = Invoke-WebRequest "${uri}throw/details-log/$clientId/$marketId" -Headers $headers -SkipHttpErrorCheck -useBasicParsing
"Status is $($result.StatusCode)"
Write-Host $result.Headers
Write-Host ($result.ToString() | ConvertFrom-Json | out-string)

Write-Host "------------ Throw Invoke-Logged"
$result = Invoke-WebRequest "${uri}throw/details-scope/$clientId/$marketId" -Headers $headers -SkipHttpErrorCheck -useBasicParsing
"Status is $($result.StatusCode)"
Write-Host $result.Headers
Write-Host ($result.ToString() | ConvertFrom-Json | out-string)

Write-Host "------------ NotImpl"
$result = Invoke-WebRequest "${uri}throw/not-implemented/$clientId/$marketId" -Headers $headers -SkipHttpErrorCheck -useBasicParsing
"Status is $($result.StatusCode)"
Write-Host $result.Headers
Write-Host ($result.ToString() | ConvertFrom-Json | out-string)