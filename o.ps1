[CmdletBinding()]
param (
    [switch] $Correlation
)
$headers = @{}
if ($Correlation) {
    $headers = @{
        "Content-Type" = "application/json"
        "X-Correlation-Id" = [Guid]::NewGuid().ToString()
        "Accept" = "application/json"
    }
}
$uri = "http://localhost:5138/api/"
# (Invoke-RestMethod "${uri}options/monitored" -Headers $headers &&
#  Invoke-RestMethod "${uri}options/snapshot"  -Headers $headers &&
#  Invoke-RestMethod "${uri}options"  -Headers $headers ) | Format-Table

$clientId = [Guid]::NewGuid()
$marketId = 1
$result = Invoke-WebRequest "${uri}options/throw/$clientId/$marketId" -Headers $headers -SkipHttpErrorCheck -useBasicParsing
"Status is $($result.StatusCode)"
$result.Headers
# [System.Text.Encoding]::UTF8.GetString($result.Content) | ConvertFrom-Json