using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace IOptionTest;

public class MyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string Name { get; set; } = "";
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
        // X-Test-User must match the name to pass AUTHN.
        // X-Test-Role is a comma separated list of roles to add to the claims
        Logger.LogInformation("Hi from HandleAuthenticateAsync named {handlerName}", Options.Name);

        var user = Context.Request.Headers["X-Test-User"];
        if (!(user.ElementAtOrDefault(0)?.Equals($"User{Options.Name}", StringComparison.OrdinalIgnoreCase) ?? false))
            return Task.FromResult(AuthenticateResult.Fail($"'{user.ElementAtOrDefault(0)}' was not 'User{Options.Name}'"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Options.Name),
        };

        var role = Context.Request.Headers["X-Test-Role"];
        foreach (var rstring in role)
        {
            if (rstring is null) continue;
            foreach (var r in rstring.Split(","))
                claims.Add(new Claim(ClaimTypes.Role, r));
        }
        Logger.LogInformation("{handlerName} set claims: {claims}", Options.Name, string.Join(", ", claims.Select(c => c.Type + ":" + c.Value)));

        var identity = new ClaimsIdentity(claims, ClaimTypes.Name);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, ClaimTypes.Name)));
    }
}
