BeforeAll {
    $uri = "http://localhost:5138/api/"
}


Describe "Tests Auth success" {
    It "Tests Auth C" {
        $result = Invoke-WebRequest "${uri}auth/c" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }
} 
