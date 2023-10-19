using Microsoft.Extensions.Options;

namespace yaflay.ru
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup()
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "m.")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages(k => { k.RootDirectory = "/Pages"; });
            services.AddControllersWithViews();
            services.AddDirectoryBrowser();

            
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add services to the container.
            // app.Services.AddRazorPages();


            // Configure the HTTP request pipeline.
            
            app.UseStaticFiles();
            

            app.UseRouting();

            app.UseAuthorization();
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}
