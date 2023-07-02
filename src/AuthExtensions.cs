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

        var user = Context.Request.Headers["X-Test-User"].ElementAtOrDefault(0);
        if (!(user?.Equals($"User{Options.Name}", StringComparison.OrdinalIgnoreCase) ?? false))
            return Task.FromResult(AuthenticateResult.Fail($"'{user}' was not 'User{Options.Name}'"));

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
        Logger.LogInformation("Scheme{handlerName} was authenticated. Set claims on {user}: {claims}", Options.Name, 
                                user, 
                                string.Join(", ", claims.Select(c => c.Type.Split('/').Last() + " = '" + c.Value + "'")));

        var identity = new ClaimsIdentity(claims, ClaimTypes.Name);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, ClaimTypes.Name)));
    }
}
