param( 
    [Parameter(Mandatory)]
    [string] $DefaultAuthScheme # is the server running with -UseDefault parameter?
) 

BeforeAll {
    $uri = "http://localhost:5138/api/"
    $usingForwarder = $DefaultAuthScheme -eq 'forward'
    $haveDefaultScheme = $DefaultAuthScheme -like 'Scheme*'
}


Describe "Tests Auth success" {
    It "Tests Auth A" {
        $result = Invoke-WebRequest "${uri}auth/a" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Auth B with scheme" {
        $result = Invoke-WebRequest "${uri}auth/b-scheme" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Auth A and B" {
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-User" = "UserA";  "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-User" = "UserB";  "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-User" = "UserC";  "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 401
    }

    It "Tests Auth A or B as A" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Auth A or B as B" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Auth A or B as A and B" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Anon" {
        $result = Invoke-WebRequest "${uri}auth/anon" -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Auth UserA RoleC" {
        $result = Invoke-WebRequest "${uri}auth/a-role-c" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Any Role with RoleA" {
        $result = Invoke-WebRequest "${uri}auth/any" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Any Role with RoleB,C" {
        $result = Invoke-WebRequest "${uri}auth/any" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "B,C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }

    It "Tests Any Role with UserB" {
        $result = Invoke-WebRequest "${uri}auth/any" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }
} 

Describe "Test auth denied" {

    It "Tests Auth A and B with only A" {
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Auth A as B" {
        $result = Invoke-WebRequest "${uri}auth/a" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 401
    }

    It "Tests Auth A and B with only B" {
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Auth A or B as B with role C" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Auth A or B as C" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-User" = "UserC"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 401
    }

    It "Tests Auth UserA RoleC wrong user" {
        $result = Invoke-WebRequest "${uri}auth/a-role-c" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 401
    }

    It "Tests Auth UserA RoleC wrong role" {
        $result = Invoke-WebRequest "${uri}auth/a-role-c" -Headers @{ "X-Test-User" = "UserA"; "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Any Role with BadUser" {
        $result = Invoke-WebRequest "${uri}auth/any" -Headers @{ "X-Test-User" = "UserZ"; "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 401
    }
} 

Describe "Tests auth policy possible errors" {

    It "Tests Auth B without a scheme" {
        $result = Invoke-WebRequest "${uri}auth/b" -Headers @{ "X-Test-User" = "UserB"; "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be ($haveDefaultScheme ? 401 : ($usingForwarder ? 200  : 500))
    }

    It "Tests Policy without AuthN scheme" {
        $result = Invoke-WebRequest "${uri}auth" -SkipHttpErrorCheck
        $result.StatusCode | Should -Be ($haveDefaultScheme -or $usingForwarder ? 401 : 500)
    }

    It "Tests Policy without AuthN scheme" {
        $result = Invoke-WebRequest "${uri}auth" -Headers @{ "X-Test-User" = "UserA"; } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be ($haveDefaultScheme -or $usingForwarder ? 200 : 500)
    } 

    It "Tests Policy without AuthN scheme" {
        $result = Invoke-WebRequest "${uri}auth" -Headers @{ "X-Test-User" = "UserB"; } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be ($haveDefaultScheme ? 401 : ($usingForwarder ? 200 : 500))
    } 
} 
