[CmdletBinding()]
param (
    [switch] $Correlation
)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$headers = @{}
if ($Correlation) {
    $headers = @{
        "Content-Type" = "application/json"
        "X-Correlation-Id" = [Guid]::NewGuid().ToString()
        "Accept" = "application/json"
    }
}
$uri = "http://localhost:5138/api/"
(Invoke-RestMethod "${uri}options/monitored" -Headers $headers &&
 Invoke-RestMethod "${uri}options/snapshot"  -Headers $headers &&
 Invoke-RestMethod "${uri}options"  -Headers $headers ) | Format-Table
