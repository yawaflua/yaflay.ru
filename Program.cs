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
        .ConfigureWebHost(webHost => {
            webHost.UseStartup<Startup>();
            webHost.UseKestrel(kestrelOptions => { kestrelOptions.ListenAnyIP(80); });
        });

    }
}