using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class MyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; } = "";
    public bool ShouldFail { get; internal set; }
}

public class CustomAuthenticationHandler : AuthenticationHandler<MyAuthenticationSchemeOptions>
{
    public CustomAuthenticationHandler(IOptionsMonitor<MyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        // options is null here
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // faked out authentication for testing
        // if there is a Role that matches Options.Name, it is OK, otherwise 401
        Console.WriteLine($"Hi from HandleAuthenticateAsync with {Options.Name}");

        if (Options.ShouldFail)
            return Task.FromResult(AuthenticateResult.Fail("ShouldFail was true"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Options.Name),
        };

        // set a role claims from a header X-Test-Role
        var role = Context.Request.Headers["X-Test-Role"];
        foreach (var rstring in role) {
            if (rstring is null) continue;
            foreach( var r in rstring.Split(",") )
                claims.Add(new Claim(ClaimTypes.Role, r));
        }
        var identity = new ClaimsIdentity(claims, ClaimTypes.Name);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, ClaimTypes.Name)));
    }
}

public static class CustomAuthenticationExtensions
{
    public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string name, bool shouldFail = false)
    {
        return builder.AddScheme<MyAuthenticationSchemeOptions, CustomAuthenticationHandler>(authenticationScheme, options => {
            options.Name = name;
            options.ShouldFail = shouldFail;
        });
    }
}