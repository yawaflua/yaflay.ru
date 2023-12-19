using Microsoft.AspNetCore.Hosting;
using yaflay.ru;
public class Program
{
    public static void Main() => CreateHostBuilder()
        .Build()
        .Run();
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