BeforeAll {
    $uri = "http://localhost:5138/api/"
}

Describe "Tests Auth" {
    # It "Tests Auth A" {
    #     $result = Invoke-WebRequest "${uri}auth/a" -Headers @{ "X-Test-Role" = "A" } -SkipHttpErrorCheck
    #     $result.StatusCode | Should -Be 200
    # }
    # It "Tests Auth B with scheme" {
    #     $result = Invoke-WebRequest "${uri}auth/b-scheme" -Headers @{ "X-Test-Role" = "B" } -SkipHttpErrorCheck
    #     $result.StatusCode | Should -Be 200
    # }
    # It "Tests Auth A and B" {
    #     $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
    #     $result.StatusCode | Should -Be 200
    # }
    It "Tests Auth A or B as A" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }
    It "Tests Auth A or B as B" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }
    It "Tests Auth A or B as A and B" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-Role" = "A,B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 200
    }
    # It "Tests Auth C" {
    #     $result = Invoke-WebRequest "${uri}auth/c" -Headers @{ "X-Test-Role" = "C" } -SkipHttpErrorCheck
    #     $result.StatusCode | Should -Be 500
    # }
    # It "Tests Anon" {
    #     $result = Invoke-WebRequest "${uri}auth/anon" -SkipHttpErrorCheck
    #     $result.StatusCode | Should -Be 200
    # }
}

Describe "Test permission" {

    It "Tests Auth A and B with only A" {
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-Role" = "A" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Auth A and B with only B" {
        $result = Invoke-WebRequest "${uri}auth/a-and-b" -Headers @{ "X-Test-Role" = "B" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }

    It "Tests Auth A or B as C" {
        $result = Invoke-WebRequest "${uri}auth/a-or-b" -Headers @{ "X-Test-Role" = "C" } -SkipHttpErrorCheck
        $result.StatusCode | Should -Be 403
    }
} 

Describe "Tests Auth Policy Errors" {

    It "Tests Auth B without a scheme" {
        $result = Invoke-WebRequest "${uri}auth/b" -Headers @{ "X-Test-Role" = "B" } -SkipHttpErrorCheck
        # Error since policy doesn't have AuthN scheme
        $result.StatusCode | Should -Be 500
    }

    It "Tests Policy without AuthN scheme" {
        $result = Invoke-WebRequest "${uri}auth" -SkipHttpErrorCheck
        # Error with just [Authorize] since no default AuthN scheme
        $result.StatusCode | Should -Be 500
    }
} -skip
