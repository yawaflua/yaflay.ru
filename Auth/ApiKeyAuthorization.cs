using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using yawaflua.ru.Database.Tables;
using yawaflua.ru.Models;
using yawaflua.ru.Models.Tables;


namespace yawaflua.ru.Auth;

public class ApiKeyAuthantication : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private AppDbContext ctx;
    private IMemoryCache cache;
    public ApiKeyAuthantication(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AppDbContext ctx,
        IMemoryCache cache
    ) : base(options, logger, encoder, clock)
    {
        this.ctx = ctx;
        this.cache = cache;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Host.Value.StartsWith("api"))
            return AuthenticateResult.NoResult();
        if (!Request.Headers.TryGetValue("Authorization", out var apiKeyHeaderValues) && Request.Host.Value.StartsWith("api"))
            return AuthenticateResult.Fail("API Key was not provided.");

        string? providedApiKey = apiKeyHeaderValues.FirstOrDefault()?.Replace("Bearer ", "");
        Console.WriteLine("APIKEY: " + providedApiKey);

        if (FindApiKey(providedApiKey, out ApiKey? apiKey))
        {
            var claims = new[]
            {
                new Claim("Bearer", apiKey.Type.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        Console.WriteLine("im there");
        return AuthenticateResult.Fail("Invalid API Key provided.");
    }

    private bool FindApiKey(string? providedApiKey, out ApiKey? apiKey)
    {
        var fromCache = cache.Get<ApiKey>($"apiKey-{providedApiKey}");
        if (fromCache == null)
        {
            Console.WriteLine($"Im there: {fromCache}, {providedApiKey}");
            apiKey = ctx.ApiKeys.Find(providedApiKey);
            if (apiKey != null)
            {
                cache.Set($"apiKey-{providedApiKey}", (object)apiKey, DateTime.Now.AddMinutes(10));
            }
        }
        else
        {
            apiKey = fromCache;
        }
        return ctx.ApiKeys.Any(k => k.Key == providedApiKey);
    }
}