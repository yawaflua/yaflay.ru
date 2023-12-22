using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using yaflay.ru;
public class Program
{
    public static void Main()
    {
        Func<string, string?> parse = (string name) => Environment.GetEnvironmentVariable(name) ?? null;

        Startup.clientId = parse("CLIENTID");
        Startup.clientSecret = parse("CLIENTSECRET");
        Startup.redirectUrl = parse("REDIRECTURL");
        Startup.connectionString = $"Host={parse("PSQL_HOST")};Username={parse("PSQL_USER")};Password={parse("PSQL_PASSWORD")};Database={parse("PSQL_DATABASE")}";
        Startup.ownerId = new[] { parse("OWNERID") };
        Startup.readmeFile = parse("READMEFILE");
        CreateHostBuilder()
            .Build()
            .Run();
    }
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webHost => {
            webHost.UseStartup<Startup>();
            webHost.UseStaticWebAssets();
            webHost.UseKestrel(kestrelOptions => { kestrelOptions.ListenAnyIP(80);});
        });

    }
}