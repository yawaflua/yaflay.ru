using Microsoft.AspNetCore.Hosting;
using yaflay.ru;
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(args[0]);
        var parse = (string name) => Environment.GetEnvironmentVariable(name) ?? null;
        Startup.clientId = parse("clientId");
        Startup.clientSecret = parse("clientSecret");
        Startup.redirectUrl = parse("redirectUrl");
        Startup.connectionString = $"Host={parse("Host")};Username={parse("Username")};Password={parse("Password")};Database={parse("Database")}";
        Console.WriteLine(parse("CLIENTID"));
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