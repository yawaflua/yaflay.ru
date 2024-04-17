using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using yawaflua.ru;
public class Program
{
    public static void Main()
    {
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
public static class StaticProgram
{
    public static bool isNull(this object? value) =>
        value == null;
    
}