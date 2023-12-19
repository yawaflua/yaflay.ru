using Microsoft.AspNetCore.Hosting;
using yaflay.ru;
public class Program
{
    public static void Main(string[] args)
    {
        
        var parsedArgs = args.FirstOrDefault(k => k.StartsWith("/p:")).Replace("/p:", "").Split(";");
        var parse = (string name) => parsedArgs.FirstOrDefault(k => k.StartsWith(name))?.Split("=")[1] ?? null;
        Startup.clientId = parse("clientId");
        Startup.clientSecret = parse("clientSecret");
        Startup.redirectUrl = parse("redirectUrl");
        Startup.connectionString = $"Host={parse("Host")};Username={parse("Username")};Password={parse("Password")};Database={parse("Database")}";
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